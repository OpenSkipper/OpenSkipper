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

namespace CANHandler
{
    public class N0183TimeField : N0183Field
    {
        public override string ToString(string[] segments)
        {
            if (SegmentIndex >= segments.Length)
                return "N/A";

            string raw = segments[SegmentIndex];
            DateTime UTCTime = DateTime.ParseExact(raw, "HHmmss", null);
            return UTCTime.ToLongTimeString();
        }
    }

} // namespace
