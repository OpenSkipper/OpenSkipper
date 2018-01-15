//http://www.developerfusion.com/article/4369/windows-form-designer-generated-code/
// states that the components member is added to a form, and if the component has a creator of form
// Public Sub New(ByVal c As IContainer), then the "components" Form member is instantiated and passed to the component's constructor:

namespace AJMLoaderCode
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Xml;
    using DisplayDefinitions;
    using OpenSkipperApplication.Properties;
    //using System.Configuration;

    /// <summary>
    /// A form that can contain components, such as extenders etc
    /// This mimics the code used by the Visual Studio designer
    /// </summary>
    public class FormWithComponents : Form {

        private bool m_AllowClose = false;
        private ToolStripMenuItem mimizeToolStripMenuItem;
        private ToolStripMenuItem topmostToolStripMenuItem;
        private MenuStrip menuStrip;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public string DefLocation;
        public ToolStripMenuItem MenuDisplaysToolStripMenuItem;
        public ToolStripMenuItem displaysToolStripMenuItem;
        public bool StartMinimized { get; set; }

        public System.ComponentModel.IContainer Components { 
            get {return components; }
            set { components = value;  } 
        }
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void mnuChangeDisplay_Click(object sender, EventArgs e)
        {
            if (displaysToolStripMenuItem != null)
            {
                ToolStripMenuItem senderItem = sender as ToolStripMenuItem;
                foreach (ToolStripMenuItem item in displaysToolStripMenuItem.DropDownItems)
                {
                    if (item.Tag == senderItem.Tag)
                    {
                        item.PerformClick();
                        return;
                    }
                }
            }
        }

        // Since we use same displaysToolStripMenuItem for all Forms, we need to add it on form activating.
        // We can not just add displaysToolStripMenuItem for all.
        // See. http://stackoverflow.com/questions/8307959/toolstripmenuitem-for-multiple-contextmenustrip
        private void FormWithComponents_Activated(object sender, EventArgs e)
        {
            if (displaysToolStripMenuItem != null)
            {
                this.ContextMenuStrip.Items.Add(displaysToolStripMenuItem);

                // Shortcut keys on ContectMenu did not work for some reason. There is discussing about it in 
                // http://stackoverflow.com/questions/12125821/handling-shortcuts-from-context-menu
                // So here we solve this so that we add same items to invisible menuStrip. Then
                // every thing works fine.
//                ToolStripManager.Merge(this.ContextMenuStrip, menuStrip); // Why this did not work
                
                MenuDisplaysToolStripMenuItem.DropDownItems.Clear();
                KeysConverter converter = new KeysConverter();
                foreach (ToolStripMenuItem item in displaysToolStripMenuItem.DropDownItems)
                {
                    ToolStripMenuItem newItem=new ToolStripMenuItem(item.Name);
                    string shortCut = converter.ConvertToString(item.ShortcutKeys);

                    newItem.ShortcutKeys = (Keys)converter.ConvertFromString(shortCut);
                    newItem.Text = item.Text;
                    newItem.Tag = item.Tag;
                    MenuDisplaysToolStripMenuItem.DropDownItems.Add(newItem);
                    newItem.Click += new EventHandler(mnuChangeDisplay_Click);
                }
            }
        }

        private void FormWithComponents_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_AllowClose)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void FormWithComponents_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Save the current position to settings
            string CurPositions = Settings.Default.UserFormPositions;
            SetFormLocation(this.Name, (this.WindowState == FormWindowState.Normal?this.Location:this.RestoreBounds.Location), ref CurPositions);
            Settings.Default.UserFormPositions = CurPositions;
        }

        private void StartMoveDisplay(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CommonRoutines.ReleaseCapture();
                CommonRoutines.SendMessage(Handle, CommonRoutines.WM_NCLBUTTONDOWN, CommonRoutines.HT_CAPTION, 0);
            }
        }

        private void mimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void TopMostChanged(object sender, EventArgs e)
        {
            this.TopMost = topmostToolStripMenuItem.Checked;
        }

        private Point GetFormLocation(string fName, string str)
        {
            string[] FormPositions = str.Split(new string[] { "|" }, StringSplitOptions.None);
            foreach (string item in FormPositions)
            {
                string[] PosPair = item.Split(new string[] { "=" }, StringSplitOptions.None);
                if ((PosPair.Length >= 2) && (PosPair[0] == fName)) 
                {
                    return (Point)((new PointConverter()).ConvertFromInvariantString(PosPair[1]));
                }
            }

            try
            {
                return (DefLocation != null ? (Point)((new PointConverter()).ConvertFromInvariantString(DefLocation)) : new Point(100, 100));
            }
            catch
            {
                return new Point(100, 100);
            }
        }

        private void SetFormLocation(string fName, Point loc, ref string str)
        {
            string[] FormPositions = str.Split(new string[] { "|" }, StringSplitOptions.None);
            str = "";
            // Build the string from others
            foreach (string item in FormPositions)
            {
                string[] PosPair = item.Split(new string[] { "=" }, StringSplitOptions.None);
                if ((PosPair.Length >= 2) && (PosPair[0] != fName))
                {
                    str = (str == "" ? item : str += "|" + item);
                }
            }

            // Add this
            string PosDef = fName + "=" + (new PointConverter()).ConvertToInvariantString(loc);
            str = (str == "" ? PosDef : str += "|" + PosDef);
        }

        private void FormWithComponents_Load(object sender, EventArgs e)
        {
            // Restore location from settings.
            Location = GetFormLocation(Name, Settings.Default.UserFormPositions);
            this.WindowState = (StartMinimized ? FormWindowState.Minimized : FormWindowState.Normal);
        }

        public void ForceClose()
        {
            m_AllowClose = true;
            Close();
        }

        public void SetDefSize()
        {
            this.FormBorderStyle = FormBorderStyle.None; // FormBorderStyle.FixedToolWindow;
            this.ClientSize = CommonRoutines.FindMinSize(this);
            CommonRoutines.SetMouseDownHandlers(this, StartMoveDisplay);
        }

        public void TopMostState(bool state)
        {
            topmostToolStripMenuItem.Checked = state;
        }

        public FormWithComponents()
        {
            FormClosing += new FormClosingEventHandler(FormWithComponents_FormClosing);
            FormClosed += new FormClosedEventHandler(FormWithComponents_FormClosed);
            this.ContextMenuStrip = new ContextMenuStrip();
            this.Activated += new EventHandler(FormWithComponents_Activated);
            this.Load += FormWithComponents_Load;

            mimizeToolStripMenuItem = new ToolStripMenuItem("mimizeToolStripMenuItem");
            mimizeToolStripMenuItem.Text = "Minimize";
            mimizeToolStripMenuItem.Click += new System.EventHandler(mimizeToolStripMenuItem_Click);
            this.ContextMenuStrip.Items.Add(mimizeToolStripMenuItem);

            topmostToolStripMenuItem = new ToolStripMenuItem("topmostToolStripMenuItem");
            topmostToolStripMenuItem.Text = "Show on top";
            topmostToolStripMenuItem.CheckedChanged += new System.EventHandler(TopMostChanged);
            topmostToolStripMenuItem.CheckOnClick = true;
            this.ContextMenuStrip.Items.Add(topmostToolStripMenuItem);

            // We need menuStrip and MenuDisplaysToolStripMenuItem for shortcut handling. See comments on FormWithComponents_Activated.
            MenuDisplaysToolStripMenuItem = new ToolStripMenuItem("MenuDisplaysToolStripMenuItem");
            MenuDisplaysToolStripMenuItem.Text = "Displays";

            menuStrip = new MenuStrip();
            menuStrip.Items.Add(MenuDisplaysToolStripMenuItem);
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
        }
    }

    
    /// <summary>
    /// Inherits from BasicDesignerLoader. It can persist the HostSurface
    /// to an Xml file and can also parse the Xml file to re-create the
    /// RootComponent and all the components that it hosts.
    /// </summary>
	public class AJMLoader
	{
		private string fileName;
		private XmlDocument xmlDocument;
		private static readonly Attribute[] propertyAttributes = new Attribute[] {
			DesignOnlyAttribute.No
		};
        public DefAssemply[] Assemblies { get; set; }

        // AJM The form we are creating, which will contain the components as well
        // FormWithComponents form;
		
		#region Constructors

		/// Empty constructor simply creates a new form.
		public AJMLoader()
		{
		}

        /// <summary>
        /// This constructor takes a file name.  This file
		/// should exist on disk and consist of XML that
		/// can be read by a data set.
        /// </summary>
        public AJMLoader(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}

			this.fileName = fileName;
		}
		#endregion

        //// Called by the host when we load a document.
        //public object PerformLoad(string fileName)
        //{
        //    // The loader will put error messages in here.
        //    ArrayList errors = new ArrayList();

        //    // If no filename was passed in, just create a form and be done with it.  If a file name
        //    // was passed, read it.
        //    if (fileName == null)
        //    {
        //        if (rootComponentType == typeof(Form))
        //        {
        //            return new Form();
        //        }
        //        else if (rootComponentType == typeof(UserControl))
        //        {
        //            return new UserControl();
        //        }
        //        else if (rootComponentType == typeof(Component))
        //        {
        //            return new Component();
        //        }
        //        else
        //        {
        //            throw new Exception("Undefined Host Type: " + rootComponentType.ToString());
        //        }
        //    }
        //    else
        //    {
        //        return ReadFile(fileName, errors);
        //    }


        //    // We've just loaded a document, so you can bet we need to flush changes.
        //    dirty = true;
        //    unsaved = false;
        //}

		
		#region Helper methods

        /// <summary>
        /// Simple helper method that returns true if the given type converter supports
		/// two-way conversion of the given type.
        /// </summary>
        private bool GetConversionSupported(TypeConverter converter, Type conversionType)
		{
			return (converter.CanConvertFrom(conversionType) && converter.CanConvertTo(conversionType));
		}

        
		#endregion

		#region Serialize - Flush

        //private XmlNode WriteObject(XmlDocument document, IDictionary nametable, object value)
        //{
        //    IDesignerHost idh = (IDesignerHost)this.host.GetService(typeof(IDesignerHost));
        //    Debug.Assert(value != null, "Should not invoke WriteObject with a null value");

        //    XmlNode node = document.CreateElement("Object");
        //    XmlAttribute typeAttr = document.CreateAttribute("type");

        //    typeAttr.Value = value.GetType().AssemblyQualifiedName;
        //    node.Attributes.Append(typeAttr);

        //    IComponent component = value as IComponent;

        //    if (component != null && component.Site != null && component.Site.Name != null)
        //    {
        //        XmlAttribute nameAttr = document.CreateAttribute("name");

        //        nameAttr.Value = component.Site.Name;
        //        node.Attributes.Append(nameAttr);
        //        Debug.Assert(nametable[component] == null, "WriteObject should not be called more than once for the same object.  Use WriteReference instead");
        //        nametable[value] = component.Site.Name;
        //    }

        //    bool isControl = (value is Control);

        //    if (isControl)
        //    {
        //        XmlAttribute childAttr = document.CreateAttribute("children");

        //        childAttr.Value = "Controls";
        //        node.Attributes.Append(childAttr);
        //    }

        //    if (component != null)
        //    {
        //        if (isControl)
        //        {
        //            foreach (Control child in ((Control)value).Controls)
        //            {
        //                if (child.Site != null && child.Site.Container == idh.Container)
        //                {
        //                    node.AppendChild(WriteObject(document, nametable, child));
        //                }
        //            }
        //        }// if isControl

        //        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, propertyAttributes);

        //        if (isControl)
        //        {
        //            PropertyDescriptor controlProp = properties["Controls"];

        //            if (controlProp != null)
        //            {
        //                PropertyDescriptor[] propArray = new PropertyDescriptor[properties.Count - 1];
        //                int idx = 0;

        //                foreach (PropertyDescriptor p in properties)
        //                {
        //                    if (p != controlProp)
        //                    {
        //                        propArray[idx++] = p;
        //                    }
        //                }

        //                properties = new PropertyDescriptorCollection(propArray);
        //            }
        //        }

        //        WriteProperties(document, properties, value, node, "Property");

        //        EventDescriptorCollection events = TypeDescriptor.GetEvents(value, propertyAttributes);
        //        IEventBindingService bindings = host.GetService(typeof(IEventBindingService)) as IEventBindingService;

        //        if (bindings != null)
        //        {
        //            properties = bindings.GetEventProperties(events);
        //            WriteProperties(document, properties, value, node, "Event");
        //        }
        //    }
        //    else
        //    {
        //        WriteValue(document, value, node);
        //    }

        //    return node;
        //}
        //private void WriteProperties(XmlDocument document, PropertyDescriptorCollection properties, object value, XmlNode parent, string elementName)
        //{
        //    foreach (PropertyDescriptor prop in properties)
        //    {
        //        if (prop.Name == "AutoScaleBaseSize")
        //        {
        //            string _DEBUG_ = prop.Name;
        //        }

        //        if (prop.ShouldSerializeValue(value))
        //        {
        //            string compName = parent.Name;
        //            XmlNode node = document.CreateElement(elementName);
        //            XmlAttribute attr = document.CreateAttribute("name");

        //            attr.Value = prop.Name;
        //            node.Attributes.Append(attr);

        //            DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)prop.Attributes[typeof(DesignerSerializationVisibilityAttribute)];

        //            switch (visibility.Visibility)
        //            {
        //                case DesignerSerializationVisibility.Visible :
        //                    if (!prop.IsReadOnly && WriteValue(document, prop.GetValue(value), node))
        //                    {
        //                        parent.AppendChild(node);
        //                    }

        //                    break;

        //                case DesignerSerializationVisibility.Content :
        //                    object propValue = prop.GetValue(value);

        //                    if (typeof(IList).IsAssignableFrom(prop.PropertyType))
        //                    {
        //                        WriteCollection(document, (IList)propValue, node);
        //                    }
        //                    else
        //                    {
        //                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(propValue, propertyAttributes);

        //                        WriteProperties(document, props, propValue, node, elementName);
        //                    }

        //                    if (node.ChildNodes.Count > 0)
        //                    {
        //                        parent.AppendChild(node);
        //                    }

        //                    break;

        //                default :
        //                    break;
        //            }
        //        }
        //    }
        //}
        //private XmlNode WriteReference(XmlDocument document, IComponent value)
        //{
        //    IDesignerHost idh = (IDesignerHost)this.host.GetService(typeof(IDesignerHost));

        //    Debug.Assert(value != null && value.Site != null && value.Site.Container == idh.Container, "Invalid component passed to WriteReference");

        //    XmlNode node = document.CreateElement("Reference");
        //    XmlAttribute attr = document.CreateAttribute("name");

        //    attr.Value = value.Site.Name;
        //    node.Attributes.Append(attr);
        //    return node;
        //}
        //private bool WriteValue(XmlDocument document, object value, XmlNode parent)
        //{
        //    IDesignerHost idh = (IDesignerHost)this.host.GetService(typeof(IDesignerHost));

        //    // For empty values, we just return.  This creates an empty node.
        //    if (value == null)
        //    {
        //        return true;
        //    }

        //    TypeConverter converter = TypeDescriptor.GetConverter(value);

        //    if (GetConversionSupported(converter, typeof(string)))
        //    {
        //        parent.InnerText = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(string));
        //    }
        //    else if (GetConversionSupported(converter, typeof(byte[])))
        //    {
        //        byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(byte[]));

        //        parent.AppendChild(WriteBinary(document, data));
        //    }
        //    else if (GetConversionSupported(converter, typeof(InstanceDescriptor)))
        //    {
        //        InstanceDescriptor id = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(InstanceDescriptor));

        //        parent.AppendChild(WriteInstanceDescriptor(document, id, value));
        //    }
        //    else if (value is IComponent && ((IComponent)value).Site != null && ((IComponent)value).Site.Container == idh.Container)
        //    {
        //        parent.AppendChild(WriteReference(document, (IComponent)value));
        //    }
        //    else if (value.GetType().IsSerializable)
        //    {
        //        BinaryFormatter formatter = new BinaryFormatter();
        //        MemoryStream stream = new MemoryStream();

        //        formatter.Serialize(stream, value);

        //        XmlNode binaryNode = WriteBinary(document, stream.ToArray());

        //        parent.AppendChild(binaryNode);
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //private void WriteCollection(XmlDocument document, IList list, XmlNode parent)
        //{
        //    foreach (object obj in list)
        //    {
        //        XmlNode node = document.CreateElement("Item");
        //        XmlAttribute typeAttr = document.CreateAttribute("type");

        //        typeAttr.Value = obj.GetType().AssemblyQualifiedName;
        //        node.Attributes.Append(typeAttr);
        //        WriteValue(document, obj, node);
        //        parent.AppendChild(node);
        //    }
        //}
        //private XmlNode WriteBinary(XmlDocument document, byte[] value)
        //{
        //    XmlNode node = document.CreateElement("Binary");

        //    node.InnerText = Convert.ToBase64String(value);
        //    return node;
        //}
        //private XmlNode WriteInstanceDescriptor(XmlDocument document, InstanceDescriptor desc, object value)
        //{
        //    XmlNode node = document.CreateElement("InstanceDescriptor");
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    MemoryStream stream = new MemoryStream();

        //    formatter.Serialize(stream, desc.MemberInfo);

        //    XmlAttribute memberAttr = document.CreateAttribute("member");

        //    memberAttr.Value = Convert.ToBase64String(stream.ToArray());
        //    node.Attributes.Append(memberAttr);
        //    foreach (object arg in desc.Arguments)
        //    {
        //        XmlNode argNode = document.CreateElement("Argument");

        //        if (WriteValue(document, arg, argNode))
        //        {
        //            node.AppendChild(argNode);
        //        }
        //    }

        //    if (!desc.IsComplete)
        //    {
        //        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(value, propertyAttributes);

        //        WriteProperties(document, props, value, node, "Property");
        //    }

        //    return node;
        //}

        #endregion

		#region DeSerialize - Load

        /// <summary>
		/// This method is used to parse the given file.  Before calling this 
		/// method the host member variable must be setup.  This method will
		/// create a data set, read the data set from the XML stored in the
		/// file, and then walk through the data set and create components
		/// stored within it.  The data set is stored in the persistedData
		/// member variable upon return.
		/// 
		/// This method never throws exceptions.  It will set the successful
		/// ref parameter to false when there are catastrophic errors it can't
		/// resolve (like being unable to parse the XML).  All error exceptions
		/// are added to the errors array list, including minor errors.
        /// </summary>
        public bool ReadFile(string fileName, IContainer extenders, out ArrayList errors, out FormWithComponents f)
		{
            errors = new ArrayList();

            FormWithComponents form = new FormWithComponents();

			// Anything unexpected is a fatal error.
			//
			try
			{
				// The main form and items in the component tray will be at the
				// same level, so we have to create a higher super-root in order
				// to construct our XmlDocument.
				StreamReader sr = new StreamReader(fileName);
				string cleandown = sr.ReadToEnd();

				cleandown = "<DOCUMENT_ELEMENT>" + cleandown + "</DOCUMENT_ELEMENT>";

				XmlDocument doc = new XmlDocument();

				doc.LoadXml(cleandown);

				// We walk through all the high level objects, being the form and its associated components
                // We do not read the form's contents at this stage, but will read that later
				//
                XmlNode formXMLNode = null;
				foreach (XmlNode node in doc.DocumentElement.ChildNodes)
				{
					if (node.Name.Equals("Object"))
					{
						Object obj = ReadMainObject(node, errors);
                        // We expect the first object to be a form
                        var objAsForm = obj as FormWithComponents;
                        if (objAsForm != null) {
                            Debug.Assert(objAsForm != null, "There must only one form, and it must be the first object.");
                            form = objAsForm;
                            formXMLNode = node;    // Remember this to read in the rest of the data later
                            // Read in rest of the object information
                            // ReadEvents(obj, node, errors); We don't handle events
                        } else {
                            // We have a non-visual component
                            // Read in the rest of the object, allowing any already loaded extenders to be used to store properties
                            ReadProperties(obj, node, errors, extenders);
                            ReadChildObjects(obj, node, errors, extenders);
                            // ReadEvents(obj, node, errors); We don't handle events
                            // And now add it as a component on our form
                            IComponent objAsComponent = obj as IComponent;
                            Debug.Assert(objAsComponent != null, "All ojects except the first object must be components.");
                            // Add the component to the form's Components
                            Debug.Assert(form != null, "There must a form as the first object.");
                            if (form.Components == null) {
                                form.Components = new System.ComponentModel.Container();
                            }
                            form.Components.Add(objAsComponent);
                        }
					}
					else
					{
						errors.Add(string.Format("Node type {0} is not allowed here.", node.Name));
					}
				}
                // Now read in the rest of the main form, handling any extenders (created above) that store properties
                if (formXMLNode != null) {
                    XmlAttribute nameAttr = formXMLNode.Attributes["name"];
                    form.Text = "Open Skipper - ";
                    if (nameAttr != null) form.Text += nameAttr.Value; else form.Text += "(empty)";
                    ReadProperties(form, formXMLNode, errors, extenders);
                    ReadChildObjects(form, formXMLNode, errors, extenders);
                    form.SetDefSize();
                }

			}
			catch (Exception ex)
			{
				errors.Add(ex);
			}
            if (errors.Count > 0) {
                MessageBox.Show("Errors have occured: " + errors.ToString());
            }
            f = form;
			return true;
		}

        /// <summary>
        /// This reads in a form file definition, and adds the contents of the form to an existing tab control on the given form
        /// 
        /// This method never throws exceptions.  It will set the successful
        /// ref parameter to false when there are catastrophic errors it can't
        /// resolve (like being unable to parse the XML).  All error exceptions
        /// are added to the errors array list, including minor errors.
        /// </summary>
        public bool ReadFileAsNewTabPage(string fileName, TabControl tabControl, IContainer extenders, out ArrayList errors, out TabPage tabPage)
        {

            errors = new ArrayList();
            tabPage = null;

            if (tabControl == null) {
                errors.Add("Tab control has to be defined");
                return false;
            }

            // Anything unexpected is a fatal error.
            try {
                // The main form and items in the component tray will be at the
                // same level, so we have to create a higher super-root in order
                // to construct our XmlDocument.
                StreamReader sr = new StreamReader(fileName);
                string cleandown = sr.ReadToEnd();

                cleandown = "<DOCUMENT_ELEMENT>" + cleandown + "</DOCUMENT_ELEMENT>";
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(cleandown);

                // We walk through all the high level objects, looking for the form node (which should be the first)
                // We do not read the form's contents at this stage, but will read that later
                //
                XmlNode formXMLNode = null;
                FormWithComponents newForm = null;
                foreach (XmlNode node in doc.DocumentElement.ChildNodes) {
                    if (node.Name.Equals("Object")) {
                        Object obj = ReadMainObject(node, errors);
                        // We expect the first object to be a form
                        var objAsForm = obj as FormWithComponents;
                        if (objAsForm != null) {
                            Debug.Assert(objAsForm != null, "There must only one form, and it must be the first object.");
                            newForm = objAsForm;   // Remember this form
                            formXMLNode = node;    // Remember this node to use when reading in the rest of the data later
                            ReadProperties(newForm, formXMLNode, errors, null); // Read in properites of the form so we can copy some of these to the tab page
                            // We don't read in any of the events or the properties for the form
                        }
                    } else {
                        errors.Add(string.Format("Node type {0} is not allowed here.", node.Name));
                    }
                }
                // Now read in the rest of the main form, handling any extenders (created above) that store properties
                if (formXMLNode != null) {
                    // Create a new tab page to our tab control.
                    tabPage = new TabPage("Tab"+newForm.Name);
                    tabPage.Text = newForm.Name;
                    tabPage.BackColor = newForm.BackColor;
                    // Read in the children (controls) from the file, storing them in the new tab page
                    // The controls read in can store their properties using any extenders on the existing form
                    ReadChildObjects(tabPage, formXMLNode, errors, extenders);
                    tabControl.TabPages.Add(tabPage);
                }

            }
            catch (Exception ex) {
                errors.Add(ex.Message);
            }
            if (errors.Count > 0) {
                string errorString  = "Errors have occured while loading the tab page: ";
                foreach (string s in errors) {
                    errorString += "\n"+s;
                }
                MessageBox.Show( errorString);
            }
            return errors.Count == 0;
        }

        public bool ReadFileAsNewTabPage(string fileName, TabControl tabControl, IContainer extenders, out ArrayList errors)
        {
            TabPage tabPage;
            return ReadFileAsNewTabPage(fileName, tabControl, extenders, out errors, out tabPage);
        }

        private void ReadEvent(XmlNode childNode, object instance, ArrayList errors)
        {
            IEventBindingService bindings = null; // host.GetService(typeof(IEventBindingService)) as IEventBindingService;

            if (bindings == null)
            {
                errors.Add("Unable to contact event binding service so we can't bind any events");
                return;
            }

            XmlAttribute nameAttr = childNode.Attributes["name"];

            if (nameAttr == null)
            {
                errors.Add("No event name");
                return;
            }

            XmlAttribute methodAttr = childNode.Attributes["method"];

            if (methodAttr == null || methodAttr.Value == null || methodAttr.Value.Length == 0)
            {
                errors.Add(string.Format("Event {0} has no method bound to it"));
                return;
            }

            EventDescriptor evt = TypeDescriptor.GetEvents(instance)[nameAttr.Value];

            if (evt == null)
            {
                errors.Add(string.Format("Event {0} does not exist on {1}", nameAttr.Value, instance.GetType().FullName));
                return;
            }

            PropertyDescriptor prop = bindings.GetEventProperty(evt);

            Debug.Assert(prop != null, "Bad event binding service");
            try
            {
                prop.SetValue(instance, methodAttr.Value);
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
            }
        }

		private object ReadInstanceDescriptor(XmlNode node, ArrayList errors, IContainer extenders)
		{
			// First, need to deserialize the member
			//
			XmlAttribute memberAttr = node.Attributes["member"];

			if (memberAttr == null)
			{
				errors.Add("No member attribute on instance descriptor");
				return null;
			}

			byte[] data = Convert.FromBase64String(memberAttr.Value);
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream(data);
			MemberInfo mi = (MemberInfo)formatter.Deserialize(stream);
			object[] args = null;

			// Check to see if this member needs arguments.  If so, gather
			// them from the XML.
			if (mi is MethodBase)
			{
				ParameterInfo[] paramInfos = ((MethodBase)mi).GetParameters();

				args = new object[paramInfos.Length];

				int idx = 0;

				foreach (XmlNode child in node.ChildNodes)
				{
					if (child.Name.Equals("Argument"))
					{
						object value;

						if (!ReadValue(child, TypeDescriptor.GetConverter(paramInfos[idx].ParameterType), errors, extenders, out value))
						{
							return null;
						}

						args[idx++] = value;
					}
				}

				if (idx != paramInfos.Length)
				{
					errors.Add(string.Format("Member {0} requires {1} arguments, not {2}.", mi.Name, args.Length, idx));
					return null;
				}
			}

			InstanceDescriptor id = new InstanceDescriptor(mi, args);
			object instance = id.Invoke();

			// Ok, we have our object.  Now, check to see if there are any properties, and if there are, 
			// set them.
			//
			foreach (XmlNode prop in node.ChildNodes)
			{
				if (prop.Name.Equals("Property"))
				{
                    ReadProperty(prop, instance, errors, extenders); // We do not allow extenders to add properties to InstanceDescriptors
				}
			}

			return instance;
		}
		/// Reads the "Object" tags. This returns an instance of the
		/// newly created object. Returns null if there was an error.

        /// Create the main object defined by the XML node, but do not create the children
        private object ReadMainObject(XmlNode node, ArrayList errors) {
            XmlAttribute typeAttr = node.Attributes["type"];
            Type type;

            if (typeAttr == null)
            {
                errors.Add("<Object> tag is missing required type attribute");
                return null;
            }

            // We convert a standard Form into a form with components
            if ((typeAttr.Value == "System.Windows.Forms.Form, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089") |
               (typeAttr.Value == "System.Windows.Forms.Form")) {
                typeAttr.Value = "AJMLoaderCode.FormWithComponents";
            }

            type = Type.GetType(typeAttr.Value);
            if (type == null) type = typeof(Control).Assembly.GetType(typeAttr.Value, true);

            if (type == null)
            {
                errors.Add(string.Format("Type {0} could not be loaded.", typeAttr.Value));
                return null;
            }

            object instance;
            instance = Activator.CreateInstance(type);

            // Got an object. Set its name property if one is given
            //
            // This can be null if there is no name for the object.
            //
            XmlAttribute nameAttr = node.Attributes["name"];
            if (nameAttr != null) {
                try {
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(instance)["Name"];
                    if (prop != null) {
                        prop.SetValue(instance, nameAttr.Value);
                    }
                }
                catch (Exception ex) {
                    errors.Add(ex.Message);
                }
            }

            return instance;
        }

        private ActivationContext ActivationContext(Type controlType)
        {
            throw new NotImplementedException();
        }
        
        private void ReadChildObjects(object instance, XmlNode node, ArrayList errors, IContainer extenders)
		{
			// Got the main object, now we must process it.  Check to see if this tag
			// offers a child collection for us to add children to.
			//
			XmlAttribute childAttr = node.Attributes["children"];
			IList childList = null;

			if (childAttr != null)
			{
				PropertyDescriptor childProp = TypeDescriptor.GetProperties(instance)[childAttr.Value];

				if (childProp == null)
				{
					errors.Add(string.Format("The children attribute lists {0} as the child collection but this is not a property on {1}", childAttr.Value, instance.GetType().FullName));
				}
				else
				{
					childList = childProp.GetValue(instance) as IList;
					if (childList == null)
					{
						errors.Add(string.Format("The property {0} was found but did not return a valid IList", childProp.Name));
					}
				}
			}

			// Now, walk the rest of the tags under this element.
			//
			foreach (XmlNode childNode in node.ChildNodes)
			{
				if (childNode.Name.Equals("Object"))
				{
					// Another object.  In this case, create the object, and
					// parent it to ours using the children property.  If there
					// is no children property, bail out now.
					if (childAttr == null)
					{
						errors.Add("Child object found but there is no children attribute");
						continue;
					}

					// no sense doing this if there was an error getting the property.  We've already reported the
					// error above.
					if (childList != null)
					{
						object childInstance = ReadObject(childNode, errors, extenders);

						childList.Add(childInstance);
					}
				}
			}
		}

        private void ReadProperties(object instance, XmlNode node, ArrayList errors, IContainer extenders) {
            // Got the main object, now we must process it.  Walk the rest of the tags under this element, processing the Properties tags
            foreach (XmlNode childNode in node.ChildNodes) {
                if (childNode.Name.Equals("Property")) {
                    // A property.  Ask the property to parse itself.
                    //
                    ReadProperty(childNode, instance, errors, extenders);
                }
            }
        }

        private void ReadEvents(object instance, XmlNode node, ArrayList errors) {
            // Got the main object, now we must process it.  Walk the rest of the tags under this element, processing the Event tags
            //
            foreach (XmlNode childNode in node.ChildNodes) {
                if (childNode.Name.Equals("Event")) {
                    // An event.  Ask the event to parse itself.
                    //
                    ReadEvent(childNode, instance, errors);
                }
            }
        }
        /// Read and create an object from an XML definition
        private object ReadObject(XmlNode node, ArrayList errors, IContainer extenders)
		{
            object instance = ReadMainObject(node, errors);
            ReadChildObjects(instance, node, errors, extenders);
            ReadProperties(instance, node, errors, extenders);
            ReadEvents(instance, node, errors);
            return instance;
        }

        #endregion

        #region ExtenderSupport

        /// <summary>
        ///  Using the given extender, set the property with name 'propertyName' of 'instance' to 'value'
        /// </summary>
        /// <param name="ex">ExtenderProvider managing the property</param>
        /// <param name="instance">The object whose property value will be set</param>
        /// <param name="propertyName">The name of the property being set</param>
        /// <param name="value">The new value for the property</param>
        /// <returns>true if the property was successfully set</returns>
        Type GetExtenderPropertyType(IExtenderProvider ex, string propertyName) {
            try {
                // Look for a method called SetX(,), and getr a type converter for the second argument
                Type t = ex.GetType();
                MethodInfo method = t.GetMethod("Set" + propertyName); // , new Type[] { typeof(Control), typeof(string) });

                Object[] att = method.GetCustomAttributes(typeof(DesignerSerializationVisibilityAttribute), true);
                if (att.Length>0) // We have found the attribute
                {
                    DesignerSerializationVisibilityAttribute visibility = att[0] as DesignerSerializationVisibilityAttribute;
                }

                if (method != null) {
                    
                    var param1 = method.GetParameters()[1];
                    Type paramType = param1.ParameterType;
                    return paramType;

                    
                    //return TypeDescriptor.GetConverter(paramType);
                }
                return null;
            }
            catch {
                return null;
            }
        }

        DesignerSerializationVisibilityAttribute
        GetExtenderPropertyDesignerSerializationVisibilityAttribute(IExtenderProvider ex, string propertyName) {
            try {
                // We get this property off the GET method for the property (which seems to be where .net gets this when it calculates this attribute for this property 
                Type t = ex.GetType();
                MethodInfo method = t.GetMethod("Get" + propertyName); // , new Type[] { typeof(Control), typeof(string) });
                if (method != null) {
                    Object[] att = method.GetCustomAttributes(typeof(DesignerSerializationVisibilityAttribute), true);
                    if (att.Length > 0) // We have found the attribute
                    {
                        return att[0] as DesignerSerializationVisibilityAttribute;
                    }
                }
                return DesignerSerializationVisibilityAttribute.Default;
            }
            catch {
                return DesignerSerializationVisibilityAttribute.Default;
            }
        }

        //TypeConverter GetExtenderPropertyTypeConverter(IExtenderProvider ex, string propertyName) {
        //    return GetExtenderPropertyType(ex,propertyName)
        //    try {
        //        // Look for a method called SetX(,), and getr a type converter for the second argument
        //        Type t = ex.GetType();
        //        MethodInfo method = t.GetMethod("Set" + propertyName); // , new Type[] { typeof(Control), typeof(string) });

        //        if (method != null) {
        //            var param1 = method.GetParameters()[1];
        //            Type paramType = param1.ParameterType;

        //            AttributeCollection attrs = method.Attributes();

        //            DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)method.Attributes[typeof(DesignerSerializationVisibilityAttribute)];

        //            return TypeDescriptor.GetConverter(paramType);
        //        }
        //        return null;
        //    }
        //    catch {
        //        return null;
        //    }
        //}

        
        /// <summary>
        ///  Using the given extender, set the property with name 'propertyName' of 'instance' to 'value'
        /// </summary>
        /// <param name="ex">ExtenderProvider managing the property</param>
        /// <param name="instance">The object whose property value will be set</param>
        /// <param name="propertyName">The name of the property being set</param>
        /// <param name="value">The new value for the property</param>
        /// <returns>true if the property was successfully set</returns>
        bool SetExtenderPropertyValue(IExtenderProvider ex, object instance, string propertyName, object value) {
            try {
                if (ex.CanExtend(instance)) {
                    // To set a property X via an an extender, we call extender.SetX(instance, value)
                    // or we call Does this class support this property?

                    // Look for a method called SetX(Control,string);
                    // TODO Do we need to allow for different 
                    Type t = ex.GetType();
                    MethodInfo method = t.GetMethod("Set" + propertyName); // , new Type[] { typeof(Control), typeof(string) });
                    if (method != null) {
                        Object result = method.Invoke(ex, new object[] { instance, value });
                        Debug.Print("{0} returned \"{1}\".", method, result);
                        return true;
                    }
                }
                return false;
            }
            catch {
                return false;
            }
        }

        object GetExtenderPropertyValue(IExtenderProvider ex, object instance, string propertyName) {
            try {
                if (ex.CanExtend(instance)) {
                    // To set a property X via an an extender, we call extender.SetX(instance, value)
                    // or we call Does this class support this property?

                    // Look for a method called SetX(Control,string);
                    // TODO Do we need to allow for different 
                    Type t = ex.GetType();
                    MethodInfo method = t.GetMethod("Get" + propertyName); // , new Type[] { typeof(Control), typeof(string) });
                    if (method != null) {
                        return method.Invoke(ex, new object[] { instance });
                    }
                }
                return null;
            }
            catch {
                return null;
            }
        }

        bool ExtenderAddsProperty(IExtenderProvider ex, object instance, string propertyName) {
            if (ex.CanExtend(instance)) {
                // Does this class support this property?
                // We have to search all the ProvidePropertyAttribute's that have been added to this class
                Type type = ex.GetType();
                foreach (ProvidePropertyAttribute att in type.GetCustomAttributes(typeof(ProvidePropertyAttribute), true))
                {
                    if (att.PropertyName == propertyName) {
                        return true;
                    }
                }
            }
            return false;
        }

//    static void Main()
//    {
//        Type t = typeof(String);

//        MethodInfo substr = t.GetMethod("Substring", 
//            new Type[] { typeof(int), typeof(int) });

//        Object result = 
//            substr.Invoke("Hello, World!", new Object[] { 7, 5 });
//        Console.WriteLine("{0} returned \"{1}\".", substr, result);
//    }
//}

        /// Find an extender that adds a property 'name' to an 'instance' 
        IExtenderProvider FindExtender(IContainer components, object instance, string propertyName) {
            if (components == null) {
                return null;
            }
            foreach (var c in components.Components) {
                IExtenderProvider ex = c as IExtenderProvider;
                if (ex != null) {
                    if (ExtenderAddsProperty(ex, instance, propertyName)) {
                        // Does this class support this property?
                        return ex;
                    }
                }
            }
            return null;
        }

        #endregion

        #region PropertyHandlers

        private string GetDefAssembly(string aValue)
        {
            foreach (DefAssemply DefA in Assemblies)
            {
                if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(DefA.Name, aValue, CompareOptions.IgnoreCase) >= 0) return DefA.Name;
            }
            return aValue;
        }

        private Type GetAssemblyType(string aValue, string type)
        {
            bool IsFullAssembly=(CultureInfo.InvariantCulture.CompareInfo.IndexOf(aValue, "version", CompareOptions.IgnoreCase)>=0);
            string assemblyStr = (IsFullAssembly ? aValue : GetDefAssembly(aValue));
            string assemblyPartName=(IsFullAssembly ?aValue.Split(new string[] { "," }, StringSplitOptions.None)[0]:aValue);
            Assembly a=null;
            try
            {
                a = Assembly.Load(assemblyStr);
            }
            catch
            {
            }
            try
            {
                if (a == null) a = Assembly.LoadWithPartialName(assemblyPartName); // LoadWithPartialName deprecated
            }
            catch
            {
            }
            if (a == null) return null;
            return a.GetType(assemblyPartName + "." + type);
        }

		/// Parses the given XML node and sets the resulting property value.
        /// Extended by AJM to also try setting the property via an extender component
        private void ReadProperty(XmlNode node, object instance, ArrayList errors, IContainer extenders)
		{
			XmlAttribute nameAttr = node.Attributes["name"];

			if (nameAttr == null)
			{
				errors.Add("Property has no name");
				return;
			}
            string propertyName = nameAttr.Value;

			PropertyDescriptor prop = TypeDescriptor.GetProperties(instance)[propertyName];
            IExtenderProvider extender = null;

            if (prop == null)
			{
                // Try to find an extender
                extender = FindExtender(extenders, instance, propertyName);
                if (extender == null) {
                    errors.Add(string.Format("Property {0} does not exist on {1} nor on any extenders", propertyName, instance.GetType().FullName));
                    return;
                }
            }

			// Get the type of this property.  We have three options:
			// 1.  A normal read/write property.
			// 2.  A "Content" property.
			// 3.  A collection.
            //
            // TODO AJM: We have partially implemented extender property setting; it only works for simple types (ie not isContent)
			//
			bool isContent = (prop != null ? prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content) // a normal property
                                            : GetExtenderPropertyDesignerSerializationVisibilityAttribute(extender, propertyName) == DesignerSerializationVisibilityAttribute.Content);    // an extender property. TODO: Properly check if the extender has DesignerSerializationVisibilityAttribute.Content

			XmlAttribute typeAttr = node.Attributes["type"];
            XmlAttribute assemplyaAttr = node.Attributes["assembly"];
            TypeConverter converter;
            Type type;

            if (typeAttr == null)
            {
                converter = (prop != null) ? prop.Converter :
                                          TypeDescriptor.GetConverter(GetExtenderPropertyType(extender, propertyName));
            }
            else
            {
                type = GetAssemblyType(assemplyaAttr.Value,typeAttr.Value);
                converter = TypeDescriptor.GetConverter(type);//typeof(System.Windows.Forms.HorizontalAlignment));
            }

			if (isContent)
			{
                object value = (prop != null) ? prop.GetValue(instance) : GetExtenderPropertyValue(extender, instance, propertyName);

				// Handle the case of a content property that is a collection.
				//
				if (value is IList)
				{
					foreach (XmlNode child in node.ChildNodes)
					{
						if (child.Name.Equals("Item"))
						{
							object item;
                            typeAttr = child.Attributes["type"];

							if (typeAttr == null)
							{
								errors.Add("Item has no type attribute");
								continue;
							}

							type = Type.GetType(typeAttr.Value);

							if (type == null)
							{
								errors.Add(string.Format("Item type {0} could not be found.", typeAttr.Value));
								continue;
							}

							if (ReadValue(child, TypeDescriptor.GetConverter(type), errors, extenders, out item))
							{
								try
								{
									((IList)value).Add(item);
								}
								catch (Exception ex)
								{
									errors.Add(ex.Message);
								}
							}
						}
						else
						{
							errors.Add(string.Format("Only Item elements are allowed in collections, not {0} elements.", child.Name));
						}
					}
				}
				else
				{
					// Handle the case of a content property that consists of child properties.
					//
					foreach (XmlNode child in node.ChildNodes)
					{
						if (child.Name.Equals("Property"))
						{
							ReadProperty(child, value, errors, extenders);
						}
						else
						{
							errors.Add(string.Format("Only Property elements are allowed in content properties, not {0} elements.", child.Name));
						}
					}
				}
			}
            else if (converter.GetType() == typeof(ArrayConverter)) 
            // Is there better way?
            // converter.ConvertFromInvariantString can not convert string to vector.
            // Using list with DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
            // makes definition xml so long and hard to read.
            // So for vector we have own handling.
            {
                object value;
                XmlAttribute itemTypeAttr = node.Attributes["itemType"];
                if (itemTypeAttr == null)
                {
                    errors.Add("VectoreItem has no itemType attribute");
                    return;
                }

                if (assemplyaAttr == null)
                {
                    type = Type.GetType(itemTypeAttr.Value);
                }
                else
                {
                    type = GetAssemblyType(assemplyaAttr.Value,itemTypeAttr.Value);
                }

                if (type == null)
                {
                    errors.Add(string.Format("Item type {0} could not be found.", itemTypeAttr.Value));
                    return;
                }

				try
				{
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Text)
                        {
                            value = prop.GetValue(instance);
                            string[] Separators = new string[] { ";" };
                            string[] strValues = node.InnerText.Split(Separators,StringSplitOptions.None);
                            TypeConverter ValueConverter=TypeDescriptor.GetConverter(type);
                            for (int i = 0; ((i<((IList)value).Count)&&(i < strValues.Length)); i++) ((IList)value)[i] = ValueConverter.ConvertFromInvariantString(strValues[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(string.Format("Failed to set property: {0}. Error: {1}.", prop.DisplayName, ex.Message));
                }
            }
            else
			{
				object value;

                if (ReadValue(node, converter, errors, extenders, out value))
				{
					// ReadValue succeeded.  Fill in the property value.
					//
					try
					{
                        if (prop != null) {
						    prop.SetValue(instance, value);
                        } else {
                            SetExtenderPropertyValue(extender, instance, propertyName, value);
                        }
					}
					catch (Exception ex)
					{
                        errors.Add(string.Format("Failed to set property: {0}. Error: {1}.",propertyName, ex.Message));
					}
				}
			}
		}

		/// Generic function to read an object value.  Returns true if the read
		/// succeeded.
		private bool ReadValue(XmlNode node, TypeConverter converter, ArrayList errors, IContainer extenders, out object value)
		{
			try
			{
				foreach (XmlNode child in node.ChildNodes)
				{
					if (child.NodeType == XmlNodeType.Text)
					{
                        value = converter.ConvertFromInvariantString(node.InnerText);
						return true;
					}
					else if (child.Name.Equals("Binary"))
					{
						byte[] data = Convert.FromBase64String(child.InnerText);

						// Binary blob.  Now, check to see if the type converter
						// can convert it.  If not, use serialization.
						//
						if (GetConversionSupported(converter, typeof(byte[])))
						{
							value = converter.ConvertFrom(null, CultureInfo.InvariantCulture, data);
							return true;
						}
						else
						{
							BinaryFormatter formatter = new BinaryFormatter();
							MemoryStream stream = new MemoryStream(data);

							value = formatter.Deserialize(stream);
							return true;
						}
					}
					else if (child.Name.Equals("InstanceDescriptor"))
					{
						value = ReadInstanceDescriptor(child, errors, extenders);
						return (value != null);
					}
					else
					{
						errors.Add(string.Format("Unexpected element type {0}", child.Name));
						value = null;
						return false;
					}
				}

				// If we get here, it is because there were no nodes.  No nodes and no inner
				// text is how we signify null.
				//
				value = null;
				return true;
			}
			catch (Exception ex)
			{
				errors.Add(ex.Message);
				value = null;
				return false;
			}
		}

		#endregion

		#region Public methods

		/// This method writes out the contents of our designer in XML.
		/// It writes out the contents of xmlDocument.
/*		public string GetCode()
		{
			// Flush();
			
			StringWriter sw;
			sw = new StringWriter();

			XmlTextWriter xtw = new XmlTextWriter(sw);

			xtw.Formatting = Formatting.Indented;
			xmlDocument.WriteTo(xtw);

			string cleanup = sw.ToString().Replace("<DOCUMENT_ELEMENT>", "");

			cleanup = cleanup.Replace("</DOCUMENT_ELEMENT>", "");
			sw.Close();
			return cleanup;
		}
*/
		public void Save()
		{
			Save(false);
		}

        /// <summary>
		/// Save the current state of the loader. If the user loaded the file
		/// or saved once before, then he doesn't need to select a file again.
		/// Unless this is being called as a result of "Save As..." being clicked,
		/// in which case forceFilePrompt will be true.
        /// </summary>
        public void Save(bool forceFilePrompt)
		{
			try
			{
				// Flush();

				int filterIndex = 3;

				if ((fileName == null) || forceFilePrompt)
				{
					SaveFileDialog dlg = new SaveFileDialog();

					dlg.DefaultExt = "xml";
					dlg.Filter = "XML Files|*.xml";
					if (dlg.ShowDialog() == DialogResult.OK)
					{
						fileName = dlg.FileName;
						filterIndex = dlg.FilterIndex;
					}
				}

				if (fileName != null)
				{
					switch (filterIndex)
					{
						case 1 :
							{
								// Write out our xmlDocument to a file.
								StringWriter sw = new StringWriter();
								XmlTextWriter xtw = new XmlTextWriter(sw);

								xtw.Formatting = Formatting.Indented;
								xmlDocument.WriteTo(xtw);

								// Get rid of our artificial super-root before we save out
								// the XML.
								//
								string cleanup = sw.ToString().Replace("<DOCUMENT_ELEMENT>", "");

								cleanup = cleanup.Replace("</DOCUMENT_ELEMENT>", "");
								xtw.Close();

								StreamWriter file = new StreamWriter(fileName);

								file.Write(cleanup);
								file.Close();
							}
							break;
					}
					// unsaved = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error during save: " + ex.ToString());
			}
		}

		#endregion

    }// class
}// namespace

