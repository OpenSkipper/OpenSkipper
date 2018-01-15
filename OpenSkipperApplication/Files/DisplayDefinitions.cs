using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using AJMLoaderCode;
using System.Drawing;

namespace DisplayDefinitions
{
    public class DefAssemply
    {
        public string Name { get; set; }

        public DefAssemply()
        {
            Name = "";
        }
    }

    public class DisplayInfo
    {
        private ToolStripMenuItem m_MainStrip;
        private DateTime m_DefinitionFileDateTime;
        private string m_RelativePath;
        
        public enum DisplayType
        {
            undefined,
            TabPage,
            Form
        }

        [XmlIgnore]
        public int Tag { get; set; }
        [XmlIgnore]
        public EventHandler SelectionHandler { get; set; }
        [XmlIgnore]
        public Control Display { get; set; }
        [XmlIgnore]
        public ToolStripMenuItem MenuItem { get; set; }

        public string Name { get; set; }
        public DisplayType Type { get; set; }
        public string Shortcut { get; set; }
        public string DefinitionFile { get; set; }
        public bool TopMost { get; set; }
        public string DefLocation { get; set; }
        public bool StartMinimized { get; set; }

        [XmlIgnore]
        public string RelativePath
        {
            get
            {
                return m_RelativePath;
            }
            set
            {
                m_RelativePath = value;
                UpdateFileDateTime();
            }
        }

        // Constructor
        public DisplayInfo()
        {
            m_MainStrip = null;
            m_DefinitionFileDateTime = new DateTime();

            Name = "";
            Type = DisplayType.undefined;
            Shortcut = "";
            DefinitionFile = "";
            m_RelativePath = "";

            Tag = 0;
            Display = null;
            MenuItem = null;
        }

        public bool IsChanged()
        {
            return ((!File.Exists(DefinitionFileFullPath())) || 
                    (File.GetLastWriteTime(DefinitionFileFullPath()) != m_DefinitionFileDateTime)
                   );
        }

        public string DefinitionFileFullPath()
        {
            if (m_RelativePath == "") return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefinitionFile);
            return Path.Combine(m_RelativePath, DefinitionFile);
        }

        private void UpdateFileDateTime()
        {
            if (File.Exists(DefinitionFileFullPath())) m_DefinitionFileDateTime = File.GetLastWriteTime(DefinitionFileFullPath());
        }

        public void AddToToolStrip(ToolStripMenuItem MainStrip, int tag, EventHandler handler)
        {
            ToolStripMenuItem NewDisplayItem = new ToolStripMenuItem(Name);
            KeysConverter converter = new KeysConverter();
            try
            {
                // Should solve here how to get localized "ctrl+1" names.
                if (Shortcut != "")
                {
                    Keys sc=(Keys)converter.ConvertFromString(Shortcut);
                    NewDisplayItem.ShortcutKeys = sc;
                }
            }
            catch (Exception /*ex*/)
            {
            }
            NewDisplayItem.Click += new System.EventHandler(handler);
            SelectionHandler = handler;
            MainStrip.DropDownItems.Add(NewDisplayItem);
            NewDisplayItem.Tag = tag;
            Tag = tag;
            MenuItem = NewDisplayItem;
            m_MainStrip = MainStrip;
        }

        public void Clear()
        {
            if (m_MainStrip!=null) m_MainStrip.DropDownItems.Remove(MenuItem);
            if (Display != null)
            {
                switch (Type)
                {
                    case DisplayInfo.DisplayType.TabPage:
                            TabControl TC = (TabControl)((TabPage)(Display)).Parent;
                            if (TC != null) TC.TabPages.Remove((TabPage)Display);
                            TC = null;
                            break;
                    case DisplayInfo.DisplayType.Form:
                            FormWithComponents f = (FormWithComponents)(Display);
                            if (f != null) f.ForceClose();
                            f = null;
                            break;
                }
            }
        }
    }

    public class DisplaysCollection
    {
        private string m_FileName;
        [XmlIgnore]
        [ReadOnly(true)]
        // The file we loaded the definitions from
        public string FileName {
            get
            {
                return m_FileName;
            }
            set
            {
                m_FileName = value;
                UpdateChildPaths();
            }
        }
        [XmlIgnore]
        [ReadOnly(true)]
        // The file we loaded the definitions from
        public DateTime FileDateTime { get; set; }

        private DisplayInfo[] _Displays;
        private DefAssemply[] _Assemblies;

        [XmlArray]
        [XmlArrayItem("DisplayInfo", typeof(DisplayInfo))]
        public DisplayInfo[] Displays
        {
            get
            {
                return _Displays;
            }
            set
            {
                _Displays = value ?? new DisplayInfo[0];
            }
        }

        [XmlArray]
        [XmlArrayItem("DefAssemply", typeof(DefAssemply))]
        public DefAssemply[] Assemblies
        {
            get
            {
                return _Assemblies;
            }
            set
            {
                _Assemblies = value ?? new DefAssemply[0];
            }
        }

        // Constructor
        public DisplaysCollection()
        {
            Displays = new DisplayInfo[] { };
            m_FileName = "";
            FileDateTime = new DateTime();
        }

        public void ClearDisplays()
        {
            for (int i = 0; i < Displays.Count(); i++)
            {
                if (Displays[i] != null) Displays[i].Clear();
                Displays[i] = null; 
            }
        }

        public void ActivateByTag(int tag)
        {
            Form f;
            foreach (DisplayInfo display in Displays)
            {
                if (display.Tag == tag)
                {
                    switch (display.Type)
                    {
                        case DisplayInfo.DisplayType.TabPage:
                        TabControl TC = (TabControl)((TabPage)(display.Display)).Parent;
                        if (TC != null)
                        {
                            TC.SelectTab((TabPage)display.Display);
                            Control c=TC.Parent; //.Activate();
                            while ((c != null) && !(c is Form)) c = c.Parent;
                            if (c != null)
                            {
                                ((Form)c).Activate();
                                ((Form)c).WindowState = FormWindowState.Normal;
                            }
                        }
                        break;

                        case DisplayInfo.DisplayType.Form:
                        f=(Form)(display.Display);
                        if (f != null)
                        {
                            f.Show();
                            f.WindowState = FormWindowState.Normal;
                            f.Activate();
                        }
                        break;
                    }
                    return;
                }
            }
        }

        public bool IsChanged(string newFileName)
        {
            bool isChanged=(FileName != newFileName || (FileName == "") || (File.GetLastWriteTime(FileName) != FileDateTime));
            if (isChanged) return true; // Main file has changed, so it is
            
            // We need to check through definition file and check is any of display definition file changed.
            foreach (DisplayInfo displayInfo in Displays)
            {
                isChanged |= displayInfo.IsChanged();
            }
            return isChanged;
        }

        // In xml one can use full paths or relative pats. So when setting FileName for this, we update child
        // filenames.
        private void UpdateChildPaths()
        {
            foreach (DisplayInfo displayInfo in Displays)
            {
                displayInfo.RelativePath = Path.GetDirectoryName(FileName);
            }
        }

        // Serialization
        private static XmlFileSerializer<DisplaysCollection> XmlFileSerializer = new XmlFileSerializer<DisplaysCollection>();

        public static DisplaysCollection LoadFromFile(string fileName)
        {
            DisplaysCollection displays = XmlFileSerializer.Deserialize(fileName);
            if (displays != null)
            {
                displays.FileName = fileName;
                displays.FileDateTime = File.GetLastWriteTime(fileName);
                return displays;
            }

            return null;
        }
        public bool SaveToFile(string fileName)
        {
//            using (new WaitCursor())
            {
                if (XmlFileSerializer.Serialize(fileName, this))
                {
                    FileName = fileName;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
