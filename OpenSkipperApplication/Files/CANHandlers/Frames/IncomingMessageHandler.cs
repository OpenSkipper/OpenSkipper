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

using CANDefinitions;
using System;

namespace CANHandler
{
    /// <summary>
    /// Handles both building multi-part messages and applying definitions.
    /// </summary>
    public class IncomingMessageHandler
    {
        // Private fields
        private AISFrame.Builder _AISBuilder;
        private N2kFrame.Builder _N2kBuilder;

        // Constructors
        public IncomingMessageHandler()
        {
            _AISBuilder = new AISFrame.Builder();
            _N2kBuilder = new N2kFrame.Builder();
        }

        // Public methods
        public void Reset()
        {
            _N2kBuilder.Reset();
            _AISBuilder.Reset();
        }

        /// <summary>
        /// Builds and decodes the given frame, returning the completed frame on retrieval of the last frame part.
        /// </summary>
        /// <param name="msg">The raw message</param>
        /// <returns>Fully-built and decoded frame</returns>
        public Frame DecodeMessage(Frame msg)
        {
            // Decoding depends on the type of frame
            if (msg is N2kFrame)
            {
                return _N2kBuilder.AddFrame((N2kFrame)msg);
            }
            else if (msg is N0183Frame)
            {
                N0183Frame n0183msg = (N0183Frame)msg;

                if (n0183msg == null || n0183msg.Header == null)
                {
                    return null;
                }
                if (n0183msg.Header.HeaderText == "!AIVDO" || n0183msg.Header.HeaderText == "!AIVDM")
                {
                    return _AISBuilder.AddFrame(n0183msg);
                }
                else if (n0183msg.Header.HeaderText == FrameConversion.N2kTypeCode)
                {
                    N2kFrame unpackedFrame = FrameConversion.UnpackN2k(n0183msg);
                    unpackedFrame.Defn = Definitions.PGNDefnCol.GetPGNDefn(unpackedFrame);
                    return unpackedFrame;
                }

                msg.Defn = Definitions.N0183DefnCol.GetN0183Defn((N0183Frame)msg);
                return msg;
            }
            else if (msg is AISFrame)
            {
                msg.Defn = Definitions.AISDefnCol.GetAISDefn((AISFrame)msg);
                return msg;
            }
            else
            {
                // We don't know how to decode this frame
                throw new NotSupportedException("Cannot decode frame of type: '" + msg.GetType().Name + "'");
            }
        }
    }

} // namespace
