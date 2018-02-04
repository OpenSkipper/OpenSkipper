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

using OpenSkipperApplication.Properties;
using Parameters;
using System;

namespace CANDefinitions
{
    /// <summary>
    /// Holds the application-wide definition collections
    /// </summary>
    public static class Definitions
    {
        // Public static definition collections, modifiable externally only via the Load__Defns(fileName) functions. 
        public static PGNDefnCollection PGNDefnCol { get; private set; }
        public static N0183DefnCollection N0183DefnCol { get; private set; }
        public static ParameterCollection ParamCol { get; private set; }
        public static AISDefnCollection AISDefnCol { get; private set; }

        // Some elements of the program (i.e the extender) are interested in when the parameters are reloaded, in order to rebind controls etc.
        public static event Action ParametersReloaded;

        // Public methods for changing the definition collections in use, will revert to internal definitions if provided with a fileName it cannot load from
        // Returns true, if new file has been loaded
        public static bool LoadPGNDefns(string fileName)
        {
            if ((PGNDefnCol != null) && !PGNDefnCol.IsChanged(fileName)) return false;
            PGNDefnCol = PGNDefnCollection.LoadFromFile(fileName);
            Settings.Default.N2kPath = fileName;
            Settings.Default.Save();
            return true;
        }

        public static bool LoadN0183Defns(string fileName)
        {
            if ((N0183DefnCol != null) && !N0183DefnCol.IsChanged(fileName)) return false;
            N0183DefnCol = N0183DefnCollection.LoadFromFile(fileName);
            Settings.Default.N0183Path = fileName;
            Settings.Default.Save();
            return true;
        }

        public static bool LoadParameters(string fileName)
        {
            if ((ParamCol != null) && !ParamCol.IsChanged(fileName)) return false;
            ParamCol = ParameterCollection.LoadFromFile(fileName);

            if (ParametersReloaded != null)
                ParametersReloaded();

            Settings.Default.ParametersPath = fileName;
            Settings.Default.Save();
            return true;
        }

        public static bool LoadAISDefns(string fileName)
        {
            if ((AISDefnCol != null) && !AISDefnCol.IsChanged(fileName)) return false;
            AISDefnCol = AISDefnCollection.LoadFromFile(fileName);
            Settings.Default.AISPath = fileName;
            Settings.Default.Save();
            return true;
        }
    }

} // namespace
