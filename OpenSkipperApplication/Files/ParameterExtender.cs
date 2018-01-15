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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Parameters;
using System.Reflection;
using CANStreams;
using CANDefinitions;

namespace OpenSkipperApplication
{
    [ProvideProperty("StreamsToMatch", typeof(Control))]
    [ProvideProperty("ParameterName", typeof(Control))]
    [ProvideProperty("PropertyToSet", typeof(Control))]
    public partial class ParameterExtender : Component, IExtenderProvider
    {
        public class Properties
        {
            public string StreamsToMatch;
            public string ParameterName;
            public string PropertyToSet;
        }
        
        // Vars
        private Dictionary<Control, Properties> _controlProperties;
        private List<ParameterLink> _parameterLinks;
        private Timer updater;

        // Constructors
        public ParameterExtender()
        {
            _controlProperties = new Dictionary<Control, Properties> { };

            // Setup event hooks
            Definitions.ParametersReloaded += new Action(Definitions_ParametersReloaded);
            StreamManager.NewStream += new Action<CANStreamer>(StreamManager_NewStream);

            InitializeComponent();

            // AJM To allow forms to be loaded dynamically, we fire the following event, which also adds the streams
            // Definitions_ParametersReloaded();
        }

        public ParameterExtender(IContainer container)
            : this()
        {
            container.Add(this);
        }

        // Clean up any resources being used including the timer, and remove the event hooks
        // A call to this has been added to the designer code's Dispose() method
        protected void DisposeEx(bool disposing) {
            if (disposing) {
                // Remove event handlers
                Definitions.ParametersReloaded -= new Action(Definitions_ParametersReloaded);
                StreamManager.NewStream -= new Action<CANStreamer>(StreamManager_NewStream);
                // Turn off the timer
                if (updater != null) updater.Stop();
            }
        }

        //// This is called when doing an XML load after the form is loaded, and before it is shown.
        //// This gets around the problem of the properties not being set on the objects when the extender is initialised.
        public void InitialiseExtender() {
            Definitions_ParametersReloaded();
        }

        // Extender needs to be notified of changes in parameters, so it can set up clones
        void Definitions_ParametersReloaded()
        {
            Initialize(Definitions.ParamCol);

            if (StreamManager.Streams != null) {
                foreach (CANStreamer stream in StreamManager.Streams)
                    StreamManager_NewStream(stream);
            }
        }

        // Extender needs to be notified of new streams, so it can connect to them
        void StreamManager_NewStream(CANStreams.CANStreamer stream)
        {
            if (_parameterLinks != null)
                foreach (ParameterLink paramLink in _parameterLinks)
                    if (paramLink.MatchesStream(stream))
                        paramLink.parameter.Connect(stream);
        }

        private void GetPropAndIndex(string PropertyToSet, out string pToSet, out object[] index)
        {
            pToSet = PropertyToSet;
            index = null;
            int iStart = PropertyToSet.IndexOf("[", 0);
            if (iStart < 0) return;
            int iEnd = PropertyToSet.IndexOf("]", iStart);
            if (iEnd < 0) return;
            string strIndex = PropertyToSet.Substring(iStart + 1, iEnd - iStart - 1);
            if (strIndex.Trim() == "") return;
            pToSet = PropertyToSet.Substring(0, iStart);
            int intIndex;
            if (int.TryParse(strIndex, out intIndex))
            {
                index = new object[] { intIndex };
            }
            else
            {
                index = new object[] { strIndex };
            }
        }

        // This links a control to a Parameter. It is called whenever the properties of a control are set,
        // which should only occur once at run time. THus, this will be called as the form is being loaded,
        // before the form is fully initialized. (They should never be changed at runtime). 
        // TODO: Add locks on operations that now happen at run time, potentially while the form is updating
        private void LinkControlToNewParameter(Control control, Properties properties) {
            if (DesignMode) return;
            if (_parameterLinks == null) return;

            // Basic checks
            if (!(string.IsNullOrEmpty(properties.ParameterName) ||
                  string.IsNullOrEmpty(properties.PropertyToSet) ||
                  string.IsNullOrEmpty(properties.StreamsToMatch))) {

                string[] ParameterNames = properties.ParameterName.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] PropertiesToSet = properties.PropertyToSet.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] StreamssToMatch = properties.StreamsToMatch.Split(new string[] { ";" }, StringSplitOptions.None);

                for (int i=0; i<ParameterNames.Length && i<PropertiesToSet.Length && i<StreamssToMatch.Length; i++)
                {
                    string ParameterName = ParameterNames[i];
                    string PropertyToSet = PropertiesToSet[i];
                    string StreamsToMatch = StreamssToMatch[i];

                    // Get property information (which should have been validated at design time)
                    string pToSet;
                    object[] index;
                    GetPropAndIndex(PropertyToSet, out pToSet, out index);
                    PropertyInfo propertyInfo = control.GetType().GetProperty(pToSet);

                    if (propertyInfo != null)
                    { // We should not get any null's, but check just in case...
                        // Attempt to find parameter
                        Parameter parameterClone;
                        parameterClone = Definitions.ParamCol.GetCopy(ParameterName, StreamsToMatch);

                        if (parameterClone != null)
                        {
                            // Must stop the clock when we change _parameterLinks 
                            // NO WE DON'T because the timer is single threaded!
                            //var timerRunning = (updater != null) && updater.Enabled;
                            //if (timerRunning) updater.Stop();
                            // Add our new parameter
                            _parameterLinks.Add(new ParameterLink(control, parameterClone, propertyInfo, index, StreamsToMatch));
                            //if (timerRunning) updater.Start();
                        }
                    }
                }
                // else Control refers to unknown parameter
            } else {
                // Missing information.
            }
        }

        // Initialization (Effectively resets everything to use supplied parameter collection), and starts up the timer
        public void Initialize(ParameterCollection paramCol)
        {
            // Stop any currently running timer
            if (updater != null) {
                updater.Stop();
            }

            _parameterLinks = new List<ParameterLink> { };
            foreach (KeyValuePair<Control, ParameterExtender.Properties> kvp in _controlProperties)
            {
                LinkControlToNewParameter(kvp.Key, kvp.Value);
            }

            // Start updating using a timer
            if (updater == null) {
                updater = new Timer();
            }
            updater.Tick += new EventHandler(updater_Tick);
            updater.Interval = 50;
            updater.Start();
        }

        private void updater_Tick(object sender, EventArgs e)
        {
            foreach (ParameterLink paramLink in _parameterLinks)
                paramLink.UpdateControl();
        }

        // Can extend
        bool IExtenderProvider.CanExtend(object o)
        {
            if ( (o is Control) && !(o is Form) && !(o is TabControl)
                  && !(o is TabPage) && !(o is MenuStrip) && !(o is StatusStrip))
                return true;
            else
                return false;
        }

        #region EnsurePropertiesExists

        private Properties EnsurePropertiesExists(Control c)
        {
            Properties p;
            if (!_controlProperties.TryGetValue(c, out p))
            {
                p = new Properties();
                _controlProperties.Add(c, p);
            }
            return p;
        }

        #endregion

        #region PropertyToSet

        [DefaultValue(null)]
        [Category("NMEA")]
        [Description("Enter the name of the control's property control to set when displaying an NMEA parameter value")]
        // Note: These attributes need to be on the Get, not the Set method. Otherwise, they don't show up
        public string GetPropertyToSet(Control c)
        {
            return EnsurePropertiesExists(c).PropertyToSet;
        }

        public void SetPropertyToSet(Control c, string propertyName)
        {
            if (propertyName == null || propertyName == "") {
                EnsurePropertiesExists(c).PropertyToSet = null;
                return; // null/"" is valid, it will be handled appropriately on load.
            }

            string[] PropertiesToSet = propertyName.Split(new string[] { ";" }, StringSplitOptions.None);

            foreach (string propertyToSet in PropertiesToSet)
            {
                string propName;
                object[] index;
                GetPropAndIndex(propertyToSet, out propName, out index);
                var propertyInfo = c.GetType().GetProperty(propName);
                if (propertyInfo == null)
                {
                    throw new ArgumentOutOfRangeException("PropertyToSet", propertyName, "'" + propertyName + "' is not a property of this object");
                }
                else if (!propertyInfo.CanWrite)
                {
                    throw new ArgumentOutOfRangeException("PropertyToSet", propertyName, "Property '" + propertyName + "' of this object cannot be changed");
                }

                var type = propertyInfo.PropertyType;
                if (type != typeof(int) &&
                    type != typeof(Single) &&
                    type != typeof(double) &&
                    type != typeof(string) &&
                    type != typeof(Decimal))
                {
                    throw new ArgumentOutOfRangeException("PropertyToSet", propertyName, "Property '" + propertyName + "' is of type " + type.ToString() + " which is not compatible with the NMEA parameter.");
                }
            }

            var properties = EnsurePropertiesExists(c);
            properties.PropertyToSet = propertyName;

            // Because we can add (now) objects at runtime, we try to create the required paramater (which will only succeed after all the properties have been set)
            LinkControlToNewParameter(c, properties);
        }

        #endregion

        #region ParameterName

        [DefaultValue(null)]
        [Category("NMEA")]
        [Description("The name of the parameter to link to")]
        public string GetParameterName(Control c)
        {
            return EnsurePropertiesExists(c).ParameterName;
        }

        public void SetParameterName(Control c, string value)
        {
            // Can't do any existence-checking as parameters are not loaded
            var properties = EnsurePropertiesExists(c);
            properties.ParameterName = value;

            // Because we can add (now) objects at runtime, we try to create the required paramater (which will only succeed after all the properties have been set)
            LinkControlToNewParameter(c, properties);
        }

        #endregion

        #region Streams

        [DefaultValue(null)]
        [Category("NMEA")]
        [Description("The streams to connect to, comma deliminated. Keywords: 'Any'")]
        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)] // AJM Experiment
        public string GetStreamsToMatch(Control c)
        {
            return EnsurePropertiesExists(c).StreamsToMatch;
        }

        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)] // AJM Experiment
        public void SetStreamsToMatch(Control c, string value)
        {
            // No error checking needed
            var properties = EnsurePropertiesExists(c);
            properties.StreamsToMatch = value;

            // Because we can add (now) objects at runtime, we try to create the required paramater (which will only succeed after all the properties have been set)
            LinkControlToNewParameter(c, properties);
        }

        #endregion
    }
}
