using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSkipperApplication
{
    internal static class Constants
    {
        // public static string LogfileFilter = "XML Log File (*.xml)|*.xml|Kees Log File (*.*)|*.*|Weather Log File (*.*)|*.*|NMEA 0183 Log File (*.txt)|*.txt";
        public static string XMLPGNDefnFileFilter = "XML NMEA 2000 Definition Files (.N2kDfn.xml)|*.N2kDfn.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string XMLN0183DefnFileFilter = "XML NMEA 0183 Definition Files (.N0183Dfn.xml)|*.N0183Dfn.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string XMLParameterFileFilter = "XML Parameter Files (.N2kParams.xml)|*.N2kParams.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string KeesXMLPGNDefnFileFilter = "XML Kees Definition Files (_explain.xml)|*_explain.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string XMLAISDefnFileFilter = "XML AIS Definition Files (.AISDfn.xml)|*.AISDfn.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string XMLDisplayDefnFileFilter = "XML Files (.xml)|*.xml";
    }
}
