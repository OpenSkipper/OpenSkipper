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

namespace CANReaders
{
    /// <summary>
    /// Represents a message from an Actisense USB device, consisting of a type, payload, and CRC
    /// </summary>
    public class ActisenseMessage
    {
        // Public
        public readonly byte MessageType;
        public readonly byte[] MessagePayload;
        public readonly byte MessageCRC;
        public readonly bool CRCisOK;

        // Constructors
        public ActisenseMessage(byte[] fullMessage, int offset)
        {
            // Format: (offset points to <Protocol>)
            // <Escape><StartOfText>[[<Protocol><Payload length>< ... Payload ... ><CRC>]]<Escape><EndOfText>

            byte payloadSize = fullMessage[offset + 1];

            MessageType = fullMessage[offset];
            MessagePayload = new byte[payloadSize];
            Array.Copy(fullMessage, 2, MessagePayload, 0, payloadSize);
            MessageCRC = fullMessage[payloadSize + 2]; // pass protocol+payloadsize byte+payload bytes

            // Check CRC (CRC byte is chosen such that (sum of all bytes + crc byte) % 256 == 0)
            int byteSum = 0;
            for (int i = 0; i <= payloadSize + 2; i++)
            {
                byteSum += fullMessage[i];
            }

            CRCisOK = ((byteSum % 256) == 0);
        }
    }

} // namespace
