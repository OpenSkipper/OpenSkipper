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

using OpenSkipperApplication;
using System;

namespace CANReaders
{
    public class N2kRawMessage
    {
        /// <summary>
        /// Maximum data length for a NMEA2000 message.
        /// With fast packet, the 1st frame can have 6 byte and 
        /// the remaining 31 frames can have 7 bytes
        /// </summary>
        public const int MaxDataLen = 223;

        /// <summary>
        /// This is the CanID, which is not to be confused with a PGN.
        /// The CanID is encoded with the PGN, Priority, Source and Destination.
        /// </summary>
        public int CanID { get; set; }
        public DateTime Timestamp { get; set; }
        public string Direction { get; set; }
        public byte Priority { get; set; }
        public int Pgn { get; set; }
        public byte Destination { get; set; }
        public byte Source { get; set; }
        public byte Lenght { get; set; }
        public byte[] Data { get; set; }

        public N2kRawMessage()
        {
        }
        public N2kRawMessage(int canID)
        {
            CanID = canID;
            Parse();
        }
        public N2kRawMessage(string hexCanID)
        {
            CanID = hexCanID.HexToInt();
            Parse();
        }

        /// <summary>
        /// Takes an extended frame CAN ID and parses its NMEA 2000 values
        /// </summary>
        public void Parse()
        {
            ParseCanID(CanID, out int priority, out int pgn, out int source, out int destination);

            Priority = priority.ToByte();
            Pgn = pgn;
            Destination = destination.ToByte();
            Source = source.ToByte();
        }

        #region CAN ID Parsing

        /// <summary>
        /// Takes an extended frame CAN ID and parses its NMEA 2000 values
        /// </summary>
        /// <param name="canid">Numeric CAN ID</param>
        /// <param name="priority">Priority</param>
        /// <param name="pgn">PGN</param>
        /// <param name="source">Source</param>
        /// <param name="destination">Destination</param>
        /// <remarks>
        /// Thanks to Kees Verruijt, Harlingen, The Netherlands for the CANboat project!
        /// Original Source: https://github.com/canboat/canboat/blob/master/common/common.c
        /// </remarks>
        public static void ParseCanID(int canid, out int priority, out int pgn, out int source, out int destination)
        {
            #region Documentation
            /*
            * ------------------------------------------------------------------------------------------------
            * Table 1 - Mapping of ISO 11783 into CAN's Arbitration and Control Fields
            * ------------------------------------------------------------------------------------------------ 
            * 29 Bit Identifiers
            * ------------------------------------------------------------------------------------------------
            * CAN ISO 11783 Bit Number
            * SOF SOF*          1
            * ID 28   P 3       2
            * ID 27   P 2       3
            * ID 26   P 1       4
            * ID 23   R 1       5
            * ID 24   DP        6
            * ID 23   PF 8      7
            * ID 22   PF 7      8
            * ID 21   PF 6      9
            * ID 20   PF 5     10
            * ID 19   PF 4     11
            * ID 18   PF 3     12
            * SRR(r) SRR*      13
            * IDE(r) IDE*      14
            * ID 17   PF 2     15
            * ID 16   PF 1     16
            * ID 13   PS 8     17
            * ID 14   PS 7     18
            * ID 13   PS 6     19
            * ID 12   PS 5     20
            * ID 11   PS 4     21
            * ID 10   PS 3     22
            * ID 9    PS 2     23
            * ID 8    PS 1     24
            * ID 7    SA 8     25
            * ID 6    SA 7     26
            * ID 3    SA 6     27
            * ID 4    SA 5     28
            * ID 3    SA 4     29
            * ID 2    SA 3     30
            * ID 1    SA 2     31
            * ID 0    SA 1     32
            * RTR(x) RTR*      33
            * r 1     r 1*     34
            * r 0     r 0*     35
            * DLC 4   DLC 4    36
            * DLC 3   DLC 3    37
            * DLC 2   DLC 2    38
            * DLC 1   DLC 1    39
            * ------------------------------------------------------------------------------------------------
            * Notes:
            *   SOF - Start of Frame Bit P# - ISO 11783 Priority Bit #n
            *   ID## - Identifier Bit #n R# - ISO 11783 Reserved Bit #n
            *   SRR - Substitute Remote Request SA# - ISO 11783 Source Address Bit #n
            *   RTR - Remote Transmission Request Bit DP - ISO 11783 Data Page
            *   IDE - Identifier Extension Bit PF# - ISO 11783 PDU Format Bit #n
            *   r# - CAN Reserved Bit #n PS# - ISO 11783 PDU Specific Bit #n
            *   DLC# - Data Length Code Bit #n *CAN Defined Bit, Unchanged in ISO 11783
            *   (d) - dominant bit 1 Required format of proprietary 11 bit identifiers
            *   (r) - recessive bit
            * ------------------------------------------------------------------------------------------------
            */
            #endregion

            byte PF = (byte)(canid >> 16);
            byte PS = (byte)(canid >> 8);
            byte DP = Convert.ToByte((byte)(canid >> 24) & 1);

            source = (byte)canid >> 0;
            priority = (byte)((canid >> 26) & 0x7);

            if (PF < 240)
            {
                /* PDU1 format, the PS contains the destination address */
                destination = PS;
                pgn = (DP << 16) + (PF << 8);
            }
            else
            {
                /* PDU2 format, the destination is implied global and the PGN is extended */
                destination = 0xff;
                pgn = (DP << 16) + (PF << 8) + PS;
            }
        }

        /// <summary>
        /// Given NMEA 2000 fields produces the extended frame CAN ID
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="pgn"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        /// <remarks>
        /// Thanks to Kees Verruijt, Harlingen, The Netherlands for the CANboat project!
        /// Original Source: https://github.com/canboat/canboat/blob/master/common/common.c
        /// </remarks>
        public static int CreateCanID(int priority, int pgn, int source, int destination)
        {
            int canId = Convert.ToInt32(source);

            if ((byte)pgn == 0)
            { 
                // PDU 1 (assumed if 8 lowest bits of the PGN are 0)
                canId += destination << 8;
                canId += pgn << 8;
                canId += priority << 26;
            }
            else
            { 
                // PDU 2
                canId += pgn << 8;
                canId += priority << 26;
            }

            return canId;
        }

        #endregion

    } // class

} // namespace
