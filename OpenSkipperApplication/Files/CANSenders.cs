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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CANHandler;

namespace CANSenders
{
    public enum OutgoingProtocolEnum
    {
        NMEA0183, Xml, N2KDatagram //, Actisense
    }

    /// <summary>
    /// Base class for an object which supports converting a frame to a byte[] corresponding a specific protocol
    /// </summary>
    public abstract class FrameSender
    {
        public static FrameSender Create(OutgoingProtocolEnum format)
        {
            switch (format)
            {
                case OutgoingProtocolEnum.NMEA0183:
                    return new N0183Sender();

             //   case OutgoingProtocolEnum.Actisense:
             //       throw new NotImplementedException("Actisense sender not yet implemented");

                case OutgoingProtocolEnum.Xml:
                    return new XmlFrameSender();

                case OutgoingProtocolEnum.N2KDatagram:
                    return new N2KDatagramSender();

                default:
                    throw new Exception("Unrecognised outgoing format: '" + format.ToString() + "'");
            }
        }

        public abstract byte[] GetBytes(Frame frame);
    }

    /// <summary>
    /// Converts all frame types into NMEA 0183 format
    /// </summary>
    public class N0183Sender : FrameSender
    {
        public override byte[] GetBytes(Frame frame)
        {
            N0183Frame n0183Frame = frame as N0183Frame;
            if (n0183Frame != null)
                return Encoding.ASCII.GetBytes(n0183Frame.FullMessage + "\r\n");
            
            N2kFrame n2kFrame = frame as N2kFrame;
            if (n2kFrame != null)
                return Encoding.ASCII.GetBytes(FrameConversion.PackN2k(n2kFrame).FullMessage + "\r\n");

            AISFrame aisFrame = frame as AISFrame;
            if (aisFrame != null)
            {
                // TODO : Breaking up message if too long

                int padbits = 6 * aisFrame.AISData.AISString.Length - 8 * aisFrame.AISData.AISBytes.Length; // [0, 5]
                string message = "!AIVDM,1,1,,A," + aisFrame.AISData.AISString + "," + padbits;

                // Add checksum
                byte checksum = 0;
                for (int i = 1; i < message.Length; i++)
                    checksum ^= Convert.ToByte(message[i]);
                message += "*" + checksum.ToString("X2");
                
                return Encoding.ASCII.GetBytes(message + "\r\n");
            }

            throw new Exception("Attempt to send unknown frame type: '" + frame.GetType().Name + "'");
        }
    }

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

    /// <summary>
    /// Converts NMEA 2000 frames into NMEA 2000 format, should only be used with UDP datagrams or a protocol where length of each transmission is known
    /// </summary>
    public class N2KDatagramSender : FrameSender
    {
        public override byte[] GetBytes(Frame frame)
        {
            N2kFrame n2kFrame = frame as N2kFrame;
            if (frame == null)
            {
                ReportHandler.LogWarning("Attempt to send non-N2k frame as N2k datagram");
                return null;
            }

            byte[] datagram = new byte[4 + n2kFrame.Data.Length];
            datagram[0] = n2kFrame.Header.Byte0;
            datagram[1] = n2kFrame.Header.Byte1;
            datagram[2] = n2kFrame.Header.Byte2;
            datagram[3] = n2kFrame.Header.Byte3;
            Array.Copy(n2kFrame.Data, 0, datagram, 4, n2kFrame.Data.Length);

            return datagram;
        }
    }

    /*public class ActisenseSender : FrameSender
    {
    }
     */
}
