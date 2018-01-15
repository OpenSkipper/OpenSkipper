using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using CANHandler;
using CANStreams;
using KeesFileHandler;
using Parameters;
using OpenSkipperApplication;
using OpenSkipperApplication.Forms;
using OpenSkipperApplication.Properties;

namespace OpenSkipperApplication
{
    static class Constants
    {
        // public static string LogfileFilter = "XML Log File (*.xml)|*.xml|Kees Log File (*.*)|*.*|Weather Log File (*.*)|*.*|NMEA 0183 Log File (*.txt)|*.txt";
        public static string XMLPGNDefnFileFilter = "XML NMEA 2000 Definition Files (.N2kDfn.xml)|*.N2kDfn.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string XMLN0183DefnFileFilter = "XML NMEA 0183 Definition Files (.N0183Dfn.xml)|*.N0183Dfn.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string XMLParameterFileFilter = "XML Parameter Files (.N2kParams.xml)|*.N2kParams.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string KeesXMLPGNDefnFileFilter = "XML Kees Definition Files (_explain.xml)|*_explain.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string XMLAISDefnFileFilter = "XML AIS Definition Files (.AISDfn.xml)|*.AISDfn.xml|XML Files (.xml)|*.xml|All Files (*.*)|*.*";
        public static string XMLDisplayDefnFileFilter = "XML Files (.xml)|*.xml";
    }

    static class Program
    {
        //[DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        //private static extern int AllocConsole();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //AllocConsole();
            //Console.WriteLine("Hello World!"); // outputs to console window

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
        }
    }
}
