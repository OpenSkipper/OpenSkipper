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

namespace CANReaders
{
    // TODO : Not completed
    public class XmlFrameReader : FrameReader
    {
        MemoryStream _buffer;

        public XmlFrameReader()
        {
            _buffer = new MemoryStream();
        }

        public override int ProcessBytes(byte[] bytes, int offset, int size)
        {
            // Push all bytes into internal buffer
            _buffer.Write(bytes, offset, size);

            while (_buffer.Length >= 4)
            {
                int xmlLength = BitConverter.ToInt32(_buffer.ToArray(), 0);

                if (_buffer.Length >= 4 + xmlLength)
                {
                    _buffer.Seek(4, SeekOrigin.Begin);

                    XmlReader xmlReader = new XmlTextReader(_buffer);

                    try
                    {
                        xmlReader.Read();
                        string outerXml = xmlReader.ReadOuterXml();
                        StringReader outerReader = new StringReader(outerXml);
                        Frame newPacket = null;

                        switch (xmlReader.Name)
                        {
                            case "N2kMsg":
                                newPacket = (N2kFrame)N2kFrame.XmlSerializer.Deserialize(outerReader);
                                break;

                            case "N0183Msg":
                                newPacket = (N0183Frame)N0183Frame.XmlSerializer.Deserialize(outerReader);
                                break;

                            case "AISMsg":
                                newPacket = (AISFrame)AISFrame.XmlSerializer.Deserialize(outerReader);
                                break;
                        }

                        if (newPacket != null)
                            OnFrameCreated(newPacket);
                    }
                    catch
                    {
                        // Expected. Trying to deserialize partially completed xml
                    }

                    // Remove part we read from memorystream
                    byte[] getBuffer = _buffer.ToArray();
                    _buffer = new MemoryStream();
                    _buffer.Write(getBuffer, 4 + xmlLength, (int)(getBuffer.Length - (4 + xmlLength)));
                }
                else
                {
                    return size;
                }
            }

            return size;

            /*
            long originalSize = _buffer.Length;

            // Push new bytes onto internal buffer
            _buffer.Write(bytes, offset, size);

            XmlReader xmlReader = new XmlTextReader(_buffer);
            try
            {
                xmlReader.Read();
                string outerXml = xmlReader.ReadOuterXml();
                StringReader outerReader = new StringReader(outerXml);

                Frame newPacket = null;
                if (xmlReader.Name == "N2kMsg")
                {
                    newPacket = (N2kFrame)N2kFrame.XmlSerializer.Deserialize(outerReader);
                }
                if (xmlReader.Name == "N0183Msg")
                {
                    newPacket = (N0183Frame)N0183Frame.XmlSerializer.Deserialize(outerReader);
                }
                if (xmlReader.Name == "AISMsg")
                {
                    newPacket = (AISFrame)AISFrame.XmlSerializer.Deserialize(outerReader);
                }

                if (newPacket != null)
                    OnFrameCreated(newPacket);

                long position = _buffer.Position;
                int bytesRead = (int)Math.Max(0, position - originalSize);

                _buffer = new MemoryStream(_buffer.ToArray(), (int)_buffer.Position, (int)(_buffer.Length - _buffer.Position));

                return bytesRead;
            }
            catch
            {
                // Expected. Trying to deserialize partially completed xml
            }
            finally
            {
                _buffer.Seek(0, SeekOrigin.Begin);
            }

            return 0;
            */
        }
    }

} // namespace
