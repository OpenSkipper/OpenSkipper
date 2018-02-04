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
    /*MultiReader - Not used (For now), protocols should not be mixed.
    public class MultiReader : FrameReader
    {
        // Private vars
        private readonly FrameReader[] _builders;
        private int _currentBuilderIndex;
        
        // Constructors
        private void Init()
        {
            foreach (FrameReader builder in _builders)
            {
                builder.FrameCreated += this.OnFrameCreated;
            }
        }
        public MultiReader(IncomingProtocolEnum protocol)
        {
            switch (protocol)
            {
                case IncomingProtocolEnum.NMEA0183:
                    _builders = new FrameReader[] { new N0183Reader(), new ActisenseInterface() };
                    break;

                case IncomingProtocolEnum.Xml:
                    _builders = new FrameReader[] { new XmlFrameReader() };
                    break;

                default:
                    throw new Exception("Unknown frame protocol '" + protocol.ToString() + "'");
            }

            Init();
        }
        public MultiReader(FrameReader[] builders)
        {
            _builders = builders;
            Init();
        }

        // Public methods
        public override int ProcessBytes(byte[] bytes, int offset, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int b = 0; b < _builders.Length; b++)
                {
                    int numRead = _builders[_currentBuilderIndex].ProcessBytes(bytes, offset + i, size - i);
                    i += numRead;

                    if (i == size)
                        return size;

                    _currentBuilderIndex = (_currentBuilderIndex + 1) % _builders.Length;
                }

                // Builders failed to read byte, => let for loop skip to next one
            }

            return size;
        }
        */

    /*public List<Frame> GetFrames(byte[] buffer, int index, int size)
        {
            List<Frame> returnList = new List<Frame> { };

            // For each byte given to us...
            for (int readPosition = index; readPosition < (index + size); readPosition++)
            {
                // Loop #builders times (So that we try all builders for this position)
                for (int i = 0; i < _builders.Length; i++)
                {
                    FrameBuilder builder = _builders[_currentBuilderIndex];
                    Frame builtFrame;
                    FrameBuilder.FrameState frameState;
                    do
                    {
                        int originalReadPosition = readPosition;
                        frameState = builder.GetFrame(buffer, ref readPosition, index + size, out builtFrame);

                        switch (frameState)
                        {
                            case FrameBuilder.FrameState.Full:
                                returnList.Add(builtFrame);
                                break;

                            case FrameBuilder.FrameState.Error:
                                // Error ! Revert readPosition.
                                readPosition = originalReadPosition;
                                break;

                            //case FrameBuilder.FrameState.None:
                            // Frame builder hasn't touched anything. Just move to next builder.
                            //   break;
                        }

                        // Loop while this builder is still producing frames
                    }
                    while (frameState == FrameBuilder.FrameState.Full);

                    // Are we at end of list?
                    if (readPosition == (index + size))
                        return returnList;

                    // Next builder.
                    _currentBuilderIndex = (_currentBuilderIndex + 1) % _builders.Length;
                }

                // Builders failed to reach end
                // For loop will step us to the next position (dropping the character)
            }


            return returnList;
        }
        */

} // namespace
