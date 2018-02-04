/*
	Copyright (C) 2009-2010, Andrew Mason <amas008@users.sourceforge.net>
	Copyright (C) 2009-2010, Jason Drake <jdra@users.sourceforge.net>

	This file is part of Open Skipper.
	
	Open Skipper is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Open Skipper is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace CANStreams
{
    /// <summary>
    /// The StreamManager static class manages all the streams in the program, including but not limited to serialization and managing connectors.
    /// </summary>
    public static class StreamManager
    {
        // Constants
        public static readonly string StreamsFileName = Application.StartupPath + "\\Streams.xml";
        public static readonly string ConnectorsFileName = Application.StartupPath + "\\Connectors.xml";

        // Public
        public static BindingList<CANStreamer> Streams { get { return _streams; } }
        public static BindingList<Connector> Connectors { get { return _connectors; } }

        public static event Action<CANStreamer> NewStream;

        // Private
        private static Dictionary<int, bool> _ids;
        private static BindingList<CANStreamer> _streams;
        private static BindingList<Connector> _connectors;
        private static Dictionary<KeyValuePair<CANStreamer, CANStreamer>, Connector> _connectorMap;
        private static Random _random;
        private static XmlFileSerializer<BindingList<CANStreamer>> _streamsSerializer = new XmlFileSerializer<BindingList<CANStreamer>>();
        private static XmlFileSerializer<BindingList<Connector>> _connectorsSerializer = new XmlFileSerializer<BindingList<Connector>>();

        // Constructor
        static StreamManager()
        {
            _ids = new Dictionary<int, bool> { };
            _streams = new BindingList<CANStreamer> { };
            _connectors = new BindingList<Connector> { };
            _connectorMap = new Dictionary<KeyValuePair<CANStreamer, CANStreamer>, Connector> { };
            _random = new Random();
        }

        // Public methods
        public static void AddStream(CANStreamer stream)
        {
            int id;
            do
            {
                id = _random.Next(1000, int.MaxValue); // Range lets us know if an ID has not been set
            } while (_ids.ContainsKey(id));

            stream.ID = id;
            _ids.Add(id, true);
            _streams.Add(stream);

            // Fire event
            var e = NewStream;
            if (e != null)
                e(stream);
        }

        public static void RemoveStream(CANStreamer stream)
        {
            _ids.Remove(stream.ID);
            _streams.Remove(stream);

            List<Connector> toRemove = new List<Connector> { };
            foreach (Connector c in _connectors)
                if (c.fromID == stream.ID || c.toID == stream.ID)
                    toRemove.Add(c);

            foreach (Connector c in toRemove)
                RemoveConnector(c._fromStream, c._toStream);
        }

        public static void AddConnector(CANStreamer fromStream, CANStreamer toStream)
        {
            var kvp = new KeyValuePair<CANStreamer, CANStreamer>(fromStream, toStream);
            if (!_connectorMap.ContainsKey(kvp))
            {
                Connector newConnector = new Connector(fromStream, toStream);
                _connectors.Add(newConnector);
                _connectorMap.Add(kvp, newConnector);
            }
        }

        public static void RemoveConnector(CANStreamer fromStream, CANStreamer toStream)
        {
            var kvp = new KeyValuePair<CANStreamer, CANStreamer>(fromStream, toStream);
            Connector connector;
            if (_connectorMap.TryGetValue(kvp, out connector))
            {
                connector.Disconnect();
                _connectorMap.Remove(kvp);
                _connectors.Remove(connector);
            }
        }

        public static bool ConnectionExists(CANStreamer fromStream, CANStreamer toStream)
        {
            return _connectorMap.ContainsKey(new KeyValuePair<CANStreamer, CANStreamer>(fromStream, toStream));
        }

        public static void DisconnectLogfiles()
        {
            foreach (CANStreamer stream in _streams)
                if (stream is CANStreamer_Logfile && stream.ConnectionState == ConnectionStateEnum.Connected)
                    stream.Disconnect();
        }

        public static void SaveToFiles()
        {
            _streamsSerializer.Serialize(StreamsFileName, _streams);
            _connectorsSerializer.Serialize(ConnectorsFileName, _connectors);
        }

        public static void LoadFromFiles()
        {
            // Deserialize streams/connectors to temp arrays, due to need for proper initialization/validation
            BindingList<CANStreamer> streamsTemp = _streamsSerializer.Deserialize(StreamsFileName) ?? new BindingList<CANStreamer> { };
            BindingList<Connector> connectorsTemp = _connectorsSerializer.Deserialize(ConnectorsFileName) ?? new BindingList<Connector> { };

            // Step 1 : Add streams (Must be done before connectors can be done)
            foreach (CANStreamer stream in streamsTemp)
            {
                _streams.Add(stream);

                var e = NewStream;
                if (e != null)
                    e(stream);
            }

            // Step 2 : Do connector work (Now that we have our streams/ids)
            foreach (Connector connector in connectorsTemp)
            {
                CANStreamer fromStream = null;
                CANStreamer toStream = null;

                foreach (CANStreamer stream in _streams)
                {
                    if (stream.ID == connector.fromID)
                        fromStream = stream;

                    if (stream.ID == connector.toID)
                        toStream = stream;
                }

                if (fromStream != null && toStream != null)
                    AddConnector(fromStream, toStream);
            }

            // Step 3 : Start up streams
            foreach (CANStreamer stream in streamsTemp)
            {
                if (stream.LastState == ConnectionStateEnum.Connected)
                    stream.ConnectStream();
            }
        }
    }

} // namespace
