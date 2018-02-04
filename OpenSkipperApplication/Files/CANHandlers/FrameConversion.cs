using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CANHandler
{
    /// <summary>
    /// Provides methods for packing an NMEA 2000 message into an NMEA 0183 message using AIS encoding of the bytes, and the reverse conversion.
    /// </summary>
    public static class FrameConversion
    {
        public const string N2kTypeCode = "$NTWOK";

        public static N0183Frame PackN2k(N2kFrame n2kFrame)
        {
            // Step 1 : Pack N2k frame into a byte array
            byte[] n2kBytes = new byte[4 + n2kFrame.Data.Length];
            n2kBytes[0] = n2kFrame.Header.Byte0;
            n2kBytes[1] = n2kFrame.Header.Byte1;
            n2kBytes[2] = n2kFrame.Header.Byte2;
            n2kBytes[3] = n2kFrame.Header.Byte3;
            Array.Copy(n2kFrame.Data, 0, n2kBytes, 4, n2kFrame.Data.Length);

            // Step 2 : Form N0183 string
            string message = N2kTypeCode + "," + AISEncoding.EncodeBytes(n2kBytes, 0, n2kBytes.Length);

            // Step 3 : Add checksum
            byte checksum = 0;
            for (int i = 1; i < message.Length; i++)
            {
                checksum ^= Convert.ToByte(message[i]);
            }
            message += "*" + checksum.ToString("X2");

            // Step 4 : Return new N0183 frame
            return new N0183Frame(message, DateTime.Now);
        }

        public static N2kFrame UnpackN2k(N0183Frame n0183Frame)
        {
            // Step 1 : Get N2k bytes
            byte[] n2kBytes = AISEncoding.GetBytes(n0183Frame.Segments[0]);
            byte[] n2kData = new byte[n2kBytes.Length - 4];
            Array.Copy(n2kBytes, 4, n2kData, 0, n2kData.Length);

            // Step 2 : Return new N2k frame
            N2kHeader n2kHeader = new N2kHeader(n2kBytes[0], n2kBytes[1], n2kBytes[2], n2kBytes[3]);
            return new N2kFrame(n2kHeader, n2kData, DateTime.Now);
        }

    }

} // namespace
