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
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using OpenSkipperApplication.Properties;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO.Ports;

/// <summary>
/// Provides a strongly-typed dictionary of events
/// </summary>
/// <typeparam name="K">The type of the keys in the dictionary</typeparam>
/// <typeparam name="E">The type of the event handler</typeparam>
public class EventDictionary<K, E> where E : EventArgs
{
    private readonly Dictionary<K, EventHandler<E>> _handlerDict;

    public EventDictionary()
    {
        _handlerDict = new Dictionary<K, EventHandler<E>> { };
    }

    public void RaiseEvent(K key, object sender, E e)
    {
        EventHandler<E> handler;
        if (_handlerDict.TryGetValue(key, out handler) && (handler != null))
            handler(sender, e);
    }
    public EventHandler<E> this[K key]
    {
        get
        {
            EventHandler<E> handler;
            if (_handlerDict.TryGetValue(key, out handler))
                return handler;
            return delegate { };
        }
        set
        {
            _handlerDict[key] = value;
            // TODO : Prevent blank delegates from being left in the dictionary when last handler is removed.
            // It is not a major issue, as the invocation of a blank delegate is not expensive.
        }
    }
}

/// <summary>
/// Provides a XML serializer suited to serializing/deserializing from files
/// </summary>
/// <typeparam name="T">The type of the object to serialize</typeparam>
public class XmlFileSerializer<T>
{
    // Static variables in generic classes are static amongst generics of the specific type T
    private static XmlSerializer _xmlSerializer = new XmlSerializer(typeof(T));

    public bool Serialize(string fileName, T obj)
    {
        try
        {
            // If we serialized directly to a filestream, and the serializer threw an exception, we would be left with a blank/corrupted file
            // So we attempt to serialize to a memory stream first, this is quick and avoids encoding issues with serializing to a string
            MemoryStream ms = new MemoryStream();
            _xmlSerializer.Serialize(ms, obj);

            // Now we can write to the file safely
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                ms.WriteTo(fs);
            }
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show("Error saving to file '" + fileName + "': " + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            return false;
        }

        return true;
    }
    public T Deserialize(string fileName)
    {
        try
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return (T)_xmlSerializer.Deserialize(fs);
            }
        }
        catch (FileNotFoundException)
        {
            // Silently return
            return default(T);
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show("Error loading from file '" + fileName + ": " + ex.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
            return default(T);
        }
    }
    public T Deserialize(Stream stream)
    {
        try
        {
            object obj = _xmlSerializer.Deserialize(stream);
            return (T)obj;
        }
        catch
        {
            return default(T);
        }
    }
}

/// <summary>
/// Provides a version of BindingList<T> that updates on the class-creating thread (via its sync context), as opposed to updating on the thread that caused the change
/// Based on work by Greg Schmitz (see http://groups.google.co.uk/group/microsoft.public.dotnet.languages.csharp/browse_thread/thread/214e55884b16f4d9/f12a3c5980567f06#f12a3c5980567f06)
/// </summary>
/// <typeparam name="T"></typeparam>
public class ThreadedBindingList<T> : BindingList<T>
{
    // This class is primarily for avoiding cross-threading issues with updating UI controls
    // Where a UI control is bound to a bindinglist, we need the updates to be processed on the UI thread, not the thread that caused the change (which may be a non-UI thread)

    private readonly SynchronizationContext _creatorContext = SynchronizationContext.Current;
    
    // Accesing the 'base' member directly from anonymous method results in 'unverifiable' code (Compiler warning), so we use these helper methods (As compiler suggests)
    private void BaseOnAddingNew(AddingNewEventArgs e)
    {
        base.OnAddingNew(e);
    }
    private void BaseOnListChanged(ListChangedEventArgs e)
    {
        base.OnListChanged(e);
    } 

    // Override updating methods to use creator context
    protected override void OnAddingNew(AddingNewEventArgs e)
    {
        if (_creatorContext == null)
        {
            BaseOnAddingNew(e);
        }
        else
        {
            _creatorContext.Send(delegate { BaseOnAddingNew(e); }, null);
        }
    }
    protected override void OnListChanged(ListChangedEventArgs e)
    {
        if (_creatorContext == null)
        {
            BaseOnListChanged(e);
        }
        else
        {
            _creatorContext.Send(delegate { BaseOnListChanged(e); }, null);
        }
    }

}

public class CommonRoutines
{
    public const int WM_NCLBUTTONDOWN = 0xA1;
    public const int HT_CAPTION = 0x2;

    [DllImportAttribute("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd,
                     int Msg, int wParam, int lParam);
    [DllImportAttribute("user32.dll")]
    public static extern bool ReleaseCapture();
    
    static public string ResolveFileNameIfEmpty(string fileName, string DefFileName)
    {
        if (Settings.Default.TryDefinitionsOnAppPath && (fileName == "") && (DefFileName != ""))
        {
            fileName = (File.Exists(DefFileName) ?
                        DefFileName :
                        (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefFileName)) ?
                         Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefFileName) :
                         ""));
        }
        return fileName;
    }

    static private Size FindMinSizeRec(Control control, Size minSize)
    {
        if (control == null) return minSize;

        foreach (Control child in control.Controls)
        {
            //                if (!child.Visible) continue;
            if (child.Location.X + child.Width > minSize.Width) minSize.Width = child.Location.X + child.Width;
            if (child.Location.Y + child.Height > minSize.Height) minSize.Height = child.Location.Y + child.Height;
            minSize = FindMinSizeRec(child, minSize);
        }

        return minSize;
    }

    static public Size FindMinSize(Control control)
    {
        Size minSize = new Size(50, 50); // default some reasonable size
        return FindMinSizeRec(control, minSize);
    }

    static private void SetMouseDownHandler(Control control, MouseEventHandler e)
    {
        if (control.GetType()!=typeof(TextBox)) control.MouseDown += new MouseEventHandler(e);
    }

    static public void SetMouseDownHandlers(Control control, MouseEventHandler e)
    {
        foreach (Control child in control.Controls)
        {
            SetMouseDownHandler(child,e);
            SetMouseDownHandlers(child, e);
        }
        SetMouseDownHandler(control,e);
    }

    static public bool TestIsPortFree(string portName)
    {
        try
        {
            SerialPort serialPortTest = new SerialPort(); ;
            serialPortTest.PortName = portName;
            serialPortTest.BaudRate = 19200;
            serialPortTest.Open();
            serialPortTest.Close();
            return true;
        }
        catch
        {
            return false;
        }
    }

}