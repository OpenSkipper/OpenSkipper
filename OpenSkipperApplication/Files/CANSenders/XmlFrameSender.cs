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

using CANHandler;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CANSenders
{
    /// <summary>
    /// Converts all frame types into an XML format
    /// </summary>
    public class XmlFrameSender : FrameSender
    {
        private XmlWriterSettings _xmlSettings;
        private XmlSerializerNamespaces _blankNamespace;

        public XmlFrameSender()
        {
            _xmlSettings = new XmlWriterSettings() { OmitXmlDeclaration = true };
            _blankNamespace = new XmlSerializerNamespaces();
            _blankNamespace.Add("", "");
        }

        public override byte[] GetBytes(Frame frame)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);
            XmlWriter xmlWriter = XmlWriter.Create(streamWriter, _xmlSettings);

            if (frame is N2kFrame)
                N2kFrame.XmlSerializer.Serialize(xmlWriter, frame, _blankNamespace);
            else if (frame is N0183Frame)
                N0183Frame.XmlSerializer.Serialize(xmlWriter, frame, _blankNamespace);
            else if (frame is AISFrame)
                AISFrame.XmlSerializer.Serialize(xmlWriter, frame, _blankNamespace);
            else
                throw new Exception("Cannot serialize frame of type: '" + frame.GetType().Name + "'");

            byte[] frameBytes = memoryStream.ToArray();
            byte[] packetLength = BitConverter.GetBytes(frameBytes.Length);

            byte[] packetBytes = new byte[4 + frameBytes.Length];
            Array.Copy(packetLength, 0, packetBytes, 0, 4);
            Array.Copy(frameBytes, 0, packetBytes, 4, frameBytes.Length);

            return packetBytes;
        }
    }

} // namespace
