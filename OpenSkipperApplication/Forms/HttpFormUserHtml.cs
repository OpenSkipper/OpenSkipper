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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Parameters;
using CANStreams;
using CANDefinitions;
using System.Net;
using System.IO;
using System.Web;
using System.Globalization;
using OpenSkipperApplication.Properties;
using System.Diagnostics;

namespace OpenSkipperApplication.Forms
{
    // To start web server for other computers on network, you need to set user rights for listening port. Set the rights by opening commnd prompt as administrator and give a command:
    // netsh http add urlacl url=http://*:80/ sddl=D:(A;;GX;;;S-1-1-0)
    // To give listen rights to everyone. 

    public partial class HttpFormUserHtml : Form
    {
        private HttpListener listener = new HttpListener();
        private HttpListener feedListener = new HttpListener();
        private Parameter dummyParameter;

        private HtmlPages htmlPages;
        const string feedExt = "ParamData";
        private string m_primaryPrefix;
        private string m_strPort;


        private class HtmlParameter
        {
            public readonly string ID;
            private Parameter[] p;
            private string StreamsToMatch;
            private bool NumericResult;
            private bool CallValueOnly;
            private bool CallHistory;
            private string CallName;

            public HtmlParameter(string[] Parts)
            {
                string[] Params = Parts[0].Split(new string[] { "," }, StringSplitOptions.None);

                p = new Parameter[0];
                ID = Params[0];
                CallValueOnly = false;
                CallHistory = false;
                CallName = "u";
                StreamsToMatch = "Any";
                NumericResult = false;

                for (int i = 1; i < Parts.Length; i++)
                {
                    string[] AttrVal = Parts[i].Split(new string[] { "=" }, StringSplitOptions.None);
                    string Attr = AttrVal[0];
                    string Val = AttrVal[0];
                    if (AttrVal.Length < 2)
                    {
                        Attr = "ID";
                    }
                    else
                    {
                        Val = AttrVal[1];
                    }

                    switch (Attr)
                    {
                        case "ID": ID = Val; break;
                        case "Streams": StreamsToMatch = Val; break;
                        case "ResultType": NumericResult = (Val == "Numeric" ? true : false); break;
                        case "CallName": CallName = Val; break;
                        case "CallType":
                            {
                                string[] TypeStrs = Val.Split(new string[] { "," }, StringSplitOptions.None);
                                foreach (string StrValueType in TypeStrs)
                                {
                                    switch (StrValueType)
                                    {
                                        case "ValueOnly": CallValueOnly = true; break;
                                        case "History": CallHistory = true; break;
                                    }
                                }
                                break;
                            }
                    }
                }

                for (int i = 0; i < Params.Length; i++) AddParam(Params[i]);

            }

            private void AddParam(string name)
            {
                Parameter new_p = Definitions.ParamCol.GetCopy(name, StreamsToMatch);
                if (new_p != null)
                {
                    Array.Resize(ref p, p.Length + 1);
                    p[p.Length - 1] = new_p;

                    if (StreamManager.Streams != null)
                    {
                        foreach (CANStreamer stream in StreamManager.Streams)
                        {
                            if (MatchesStream(stream))
                            {
                                // Disconnect to avoid hooked twice.
                                new_p.Disconnect(stream);  // Do we actually need to disconnect?
                                new_p.Connect(stream);
                            }
                        }
                    }
                }
                else
                {
                    Array.Resize(ref p, p.Length + 1);
                    p[p.Length - 1] = new DummyParameter();
                }
            }

            public bool MatchesStream(CANStreamer stream)
            {
                return StreamsToMatch.Contains("Any") || StreamsToMatch.Contains(stream.Name);
            }

            public string BuildResponseString()
            {
                string responseString = CallName + "(" + (CallValueOnly ? "" : "\"" + ID + "\",");
                string ParamVal="";

                for (int i = 0; i < p.Length; i++)
                {
                    if (CallHistory)
                    {
                        NumericParameter pNum = p[i] as NumericParameter;
                        if (pNum != null) ParamVal = pNum.GetHistory();
                    }
                    else
                    {
                        switch (p[i].State())
                        {
                            case Parameter.ParameterStateEnum.NoDataReceived:
                            case Parameter.ParameterStateEnum.IsNotAvailable:
                                ParamVal = "\"" + p[i].InternalName + "\"";
                                break;
                            case Parameter.ParameterStateEnum.ValidValueReceived:
                                if (NumericResult && (p[i] is Parameters.NumericParameter))
                                {
                                    ParamVal = ((NumericParameter)(p[i])).ToDouble().ToString(CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    ParamVal = "\"" + p[i].ToString().Replace("\r\n", "<br />") + "\"";
                                }
                                break;
                            case Parameter.ParameterStateEnum.IsError:
                                ParamVal = "\"Error\"";
                                break;
                            case Parameter.ParameterStateEnum.Lost:
                                ParamVal = "\"Lost\"";
                                break;
                        }
                    }
                    if (i > 0) responseString += "," + ParamVal; else responseString += ParamVal;
                }
                responseString += ");";

                return responseString;
            }

            public static HtmlParameter TryParam(string ParamDef)
            {
                // Split it to parts [1]=ParamName, [2..]=Options, where options are
                //    Optional ID
                //    Result=string (default)/numeric
                //    CallName=Heading (default u)
                //    CallType=IDAndValue (default)/ValueOnly
                string[] Parts = ParamDef.Split(new string[] { ":" }, StringSplitOptions.None);
                if (Parts.Length < 1) return null;

                HtmlParameter NewPar = new HtmlParameter(Parts);

                if (NewPar.p.Length == 0) NewPar = null;

                return NewPar;
            }
        }

        private class HtmlPage : IDisposable
        {
            // Flag: Has Dispose already been called?
            bool disposed = false;

            private string m_HtmlData;
            private string m_PagePath;
            private DateTime m_PageDateTime;
            public string Name;

            public void Dispose()
            {
                Dispose(true);
            }
            // Protected implementation of Dispose pattern.
            protected virtual void Dispose(bool disposing)
            {
                if (disposed)
                    return;

                if (disposing)
                {
                }
                disposed = true;
            }
            public string PagePath
            { 
                get
                {
                    return m_PagePath;
                }
                set
                {
                    m_PagePath = value;
                    if (File.Exists(PagePath)) m_PageDateTime = File.GetLastWriteTime(PagePath);
                }
            }

            public bool IsChanged()
            {
                if (File.Exists(PagePath)&&(m_PageDateTime == File.GetLastWriteTime(PagePath))) return false;
                return true;
            }

            virtual public void Response(HttpListenerResponse httpResponse)
            {
//                return m_HtmlData;
                byte[] responseBytes = Encoding.UTF8.GetBytes(m_HtmlData);

                //--------------------------------------------------------------------------

                // Send the response
                httpResponse.ContentLength64 = responseBytes.Length;
                httpResponse.ContentEncoding = Encoding.UTF8;
                httpResponse.ContentType = "text/html";
                switch (Path.GetExtension(PagePath)) {
                    case ".css": httpResponse.ContentType = "text/css"; break;
                    case ".js": httpResponse.ContentType = "application/javascript"; break;
                }

                Stream output = httpResponse.OutputStream;
                output.Write(responseBytes, 0, responseBytes.Length);
                output.Close();
            }

            // Constructors
            public HtmlPage(string name, string htmlData)
            {
                Name = name;
                m_HtmlData = htmlData;
                m_PageDateTime = new DateTime();
            }

            public HtmlPage(string name)
            {
                Name = name;
                m_HtmlData = "<HTML>Invalid page</HTML>";
                m_PageDateTime = new DateTime();
            }

            protected static void SetHtmlData(HtmlPage b, string htmlData)
            {
                if (b!=null) b.m_HtmlData = htmlData;
            }
        }

        private class HtmlImagePage : HtmlPage
        {
            private MemoryStream imageData;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (imageData != null))
                {
                    imageData.Dispose();
                }
                base.Dispose(disposing);
            }

            override public void Response(HttpListenerResponse httpResponse)
            {
                imageData.Seek(0, SeekOrigin.Begin);
                httpResponse.ContentLength64 = imageData.Length;
                httpResponse.ContentType = "image/jpeg";
                imageData.CopyTo(httpResponse.OutputStream);
                httpResponse.OutputStream.Close();
            }

            public void LoadFile(string path)
            {
                FileStream FileData = new FileStream(path, FileMode.Open, FileAccess.Read);
                imageData = new MemoryStream();
                FileData.CopyTo(imageData);
                PagePath = path;
            }

            public HtmlImagePage(string name, string path) : base(name)
            {
                LoadFile(path);
            }
        }

        private class HtmlParametersPage : HtmlPage
        {
            private HtmlParameter[] m_htmlParameters;

            public HtmlParametersPage(string name) : base(name)
            { 
                m_htmlParameters=new HtmlParameter[0];
            }

            private const string ParamTag = "{OpenSkipperParam:";

            public static HtmlParametersPage TryLoad(string htmlData, string name)
            {
                HtmlParametersPage htmlParametersPage = null;
                int i;

                while ((i = CultureInfo.InvariantCulture.CompareInfo.IndexOf(htmlData, ParamTag, CompareOptions.IgnoreCase)) >= 0)
                {
                    // Find end of parameter definition
                    int iEnd = htmlData.IndexOf("}", i);
                    string ParamDef = (iEnd >= 0 ? htmlData.Substring(i + ParamTag.Length, iEnd - i - ParamTag.Length) : "");

                    HtmlParameter NewPar = HtmlParameter.TryParam(ParamDef);

                    // We found parameter definition
                    if (NewPar!=null)
                    {
                        if (htmlParametersPage == null) htmlParametersPage = new HtmlParametersPage(name);
                        Array.Resize(ref htmlParametersPage.m_htmlParameters, htmlParametersPage.m_htmlParameters.Length + 1);
                        htmlParametersPage.m_htmlParameters[htmlParametersPage.m_htmlParameters.Length - 1] = NewPar;
                        htmlData = String.Concat(htmlData.Substring(0, i), NewPar.ID, htmlData.Substring(iEnd + 1));
                    }
                }
                HtmlPage.SetHtmlData(htmlParametersPage, htmlData);

                return htmlParametersPage;
            }

            public void PostResponse(HttpListenerResponse httpResponse)
            {
                string responseString = "";
//                responseString += "u('Tank1','" + ((p1 == dummyParameter) ? "-" : p1.ToString().Replace("\r\n", "<br />")) + "');";
                foreach (HtmlParameter param in m_htmlParameters)
                {
                    responseString += param.BuildResponseString();
                }

                byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);

                //--------------------------------------------------------------------------

                // Send the response
                httpResponse.ContentLength64 = responseBytes.Length;
                httpResponse.ContentEncoding = Encoding.UTF8;
                httpResponse.ContentType = "text/html";

                Stream output = httpResponse.OutputStream;
                output.Write(responseBytes, 0, responseBytes.Length);
                output.Close();
            }
        }

        private class HtmlPages
        {
            private List<HtmlPage> m_Pages;

            public string RootPath { get; set; }

            public HtmlPage FindPage(string newPage)
            {
                foreach (HtmlPage page in m_Pages) if (page.Name == newPage)
                    {
                        if (!page.IsChanged()) return page;
                        m_Pages.Remove(page);
                        return null;
                    }
                return null;
            }

            public HtmlParametersPage FindParamPage(string url)
            {
//                StringBuilder urlBuilder = new StringBuilder(url);
                string feedurl = (url.Length > 0 ? url.Substring(1) : "");  // Remove  /
                string pageurl = feedurl.Replace(feedExt, "");
                pageurl = (pageurl.Length > 0 ? pageurl.Substring(1) : "");  // Remove  /
                foreach (HtmlPage page in m_Pages)
                {
                    if (((page.Name == pageurl) || (page.Name == feedurl)) &&
                        (page.GetType() == typeof(HtmlParametersPage)))
                    {   
                        if (!page.IsChanged()) return (HtmlParametersPage)page;
                        m_Pages.Remove(page);
                        return null;
                    }
                }
                return null;
            }

            public HtmlPage LoadPage(string newPage, string[] AcceptTypes)
            {
                newPage = newPage.Substring(1);
                HtmlPage htmlPage = FindPage(newPage);

                if (htmlPage != null) return htmlPage;

                string FullPath = RootPath + "/" + newPage; // Path.Combine(RootPath, newPage);
                if (Directory.Exists(FullPath)) // Just a directory, then use default name OpenSkipper.html
                {
                    FullPath = Path.Combine(RootPath, "OpenSkipper.html");
                }

                if (File.Exists(FullPath)) 
                {  string type=((AcceptTypes.Length>0)?
                                (AcceptTypes[0].Split(new string[] {"/"},StringSplitOptions.None)[0]):
                                "text");
                    // How to do this right?
                    if (type=="image")
                    {
                        htmlPage = new HtmlImagePage(newPage, FullPath);
                        htmlPage.PagePath = FullPath;
                    } 
                    else
                    {
                        string htmlData = File.ReadAllText(FullPath);
                        htmlPage = HtmlParametersPage.TryLoad(htmlData, newPage);
                        if (htmlPage == null) htmlPage = new HtmlPage(newPage, htmlData);
                        htmlPage.PagePath = FullPath;
                    }
                }
                else
                {
                    htmlPage= new HtmlPage(newPage);  // Return default page
                }

                m_Pages.Add(htmlPage);

                return htmlPage;
            }

            public HtmlPages()
            {
                RootPath = Settings.Default.WWWRoot;
                if (!Path.IsPathRooted(RootPath)) RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RootPath);
                m_Pages=new List<HtmlPage>();
            }

        }

        public HttpFormUserHtml()
        {
            InitializeComponent();

            dummyParameter = new MultipleSourceNumeric() { InternalName = "<None>" };

            if (Settings.Default.StartWebServerLastState)
            {
                checkBoxLocalOnly.Checked = Settings.Default.StartWebServerLocalOnly;
                chkHTTP.Checked = Settings.Default.StartWebServer;
            }
        }

        private bool CheckExceptionExplainMsg(Exception ex, out string ExplainMsg)
        {
            ExplainMsg = "";
            if ((ex.GetType() == typeof(System.Net.HttpListenerException)) && (((System.Net.HttpListenerException)(ex)).NativeErrorCode == 32))
            {
                ExplainMsg = "There may be other application listening port " + m_strPort
                         + ". Close other listening applications (like Skype) or try to change to other port.";
                return true;
            }
            return false;
        }

        // HTTP
        private void chkHTTP_CheckedChanged(object sender, EventArgs e)
        {
            m_strPort = System.Convert.ToString(Settings.Default.WWWPort);
            if (checkBoxLocalOnly.Checked) m_strPort = (Settings.Default.WWWPort == 80 ? "8080" : m_strPort);
            m_primaryPrefix = (checkBoxLocalOnly.Checked ? "http://localhost:" + m_strPort + "/" : "http://*:" + m_strPort + "/");

            try
            {
                if (chkHTTP.CheckState == CheckState.Checked)
                {
                    try
                    {
                        try
                        {
                            StartListener(m_primaryPrefix);
                        }
                        catch (HttpListenerException ex)
                        {
                            string ExplainMsg;
                            if (CheckExceptionExplainMsg(ex, out ExplainMsg))
                            {
                                MessageBox.Show("Failed to start Web server: "
                                                                  + ex.Message
                                                                  +"\n\r\n\r"
                                                                  + ExplainMsg
                                                                  ,
                                                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                DialogResult dr = MessageBox.Show("Failed to start Web server: "
                                                                  + ex.Message
                                                                  + "\n\nYou may not have rights to start web server for url:" + m_primaryPrefix
                                                                  + "\n\nDo you want to try that OpenSkipper gives rights for url for all users?"
                                                                  ,
                                                                  "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                                if (dr == DialogResult.Yes)
                                {
                                    GiveListenRights();
                                    StartListener(m_primaryPrefix);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string ExplainMsg;
                        CheckExceptionExplainMsg(ex, out ExplainMsg);
                        DialogResult dr = MessageBox.Show("Failed to start Web server: "
                                                          + ex.Message
                                                          + "\n\r\n\r"
                                                          + ExplainMsg
                                                          , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
                }
                else
                {
                    StopListener();
                }
            }
            finally
            {
                SetStatus();
            }
        }

        private void SetStatus()
        {
            if ((listener != null) && (listener.IsListening))
            {
                foreach (IPAddress ipAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                    richTextStatus.Text += "\r\nhttp:\\\\" + ipAddress.ToString();
                chkHTTP.CheckState = CheckState.Checked;
            }
            else
            {
                richTextStatus.Text = "Web server disabled.";
                chkHTTP.CheckState = CheckState.Unchecked;
            }
        }

        private void StopListener()
        {
            try
            {
                if ((listener != null) && (listener.IsListening)) listener.Stop();

                if ((feedListener != null) && (feedListener.IsListening)) feedListener.Stop();

                listener = null;
                feedListener = null;
            }
            catch
            {
            }

        }

        private void StartListener(string primaryPrefix)
        {
            StopListener();

            listener = new HttpListener();
            feedListener = new HttpListener();
            htmlPages = new HtmlPages(); // reset pages, when we start listener

            try
            { 
                listener.Prefixes.Add(primaryPrefix);
                feedListener.Prefixes.Add(primaryPrefix + feedExt + "/");

                listener.Start();
                feedListener.Start();

                listener.BeginGetContext(listenerCallback, null);
                feedListener.BeginGetContext(feedCallback, null);

                richTextStatus.Text = "Accepting connections on:";
            }
            catch (Exception ex)
            {
                StopListener();
                throw ex;
            }
        }

        private void listenerCallback(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext httpContext = listener.EndGetContext(ar);
                HttpListenerResponse httpResponse = httpContext.Response;
                HtmlPage htmlPage = htmlPages.LoadPage(httpContext.Request.RawUrl, httpContext.Request.AcceptTypes);

                //--------------------------------------------------------------------------

                CANHandler.ReportHandler.LogInfo("Received listener request: " + httpContext.Request.RawUrl);

                // Construct a response.
                htmlPage.Response(httpResponse);
            }
            catch //(Exception ex) 
            {   
            }
            // Time to time we got exception something like:
            // {The given networkname isn't availabe anymore} 
            // by using Lumia explorer, which caused that listener context was not
            // restarted. So after exception, we try anyway to start context listener. This seem to work.
            try { listener.BeginGetContext(listenerCallback, null); }
            catch { }
        }

        private void feedCallback(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext httpContext = feedListener.EndGetContext(ar);
                HttpListenerResponse httpResponse = httpContext.Response;

                HtmlParametersPage htmlPage = htmlPages.FindParamPage(httpContext.Request.RawUrl);
                if (htmlPage == null)
                {
                    htmlPages.LoadPage(httpContext.Request.RawUrl, new string[0]);
                    htmlPage = htmlPages.FindParamPage(httpContext.Request.RawUrl);
                }

                //--------------------------------------------------------------------------

                CANHandler.ReportHandler.LogInfo("Received web data feed listener request: " + httpContext.Request.RawUrl);

                if (htmlPage != null)
                {
                    htmlPage.PostResponse(httpResponse);
                }
                else
                {
                    new HtmlParametersPage("Dummy").PostResponse(httpResponse);
                }

//                feedListener.BeginGetContext(feedCallback, null);
            }
            catch //(Exception ex)
            {
            }
            // Time to time we got exception something like:
            // {The given networkname isn't availabe anymore} 
            // by using Lumia explorer, which caused that listener context was not
            // restarted. So after exception, we try anyway to start context listener. This seem to work.
            // Try to find solution later..
            try { feedListener.BeginGetContext(feedCallback, null); }
            catch { }
        }

        private void richTextStatus_LinkClicked(object sender, LinkClickedEventArgs e) {
            try {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch (Exception ex) {
                MessageBox.Show("An error occurred opening the URL: \r\n" + ex.ToString() );
            }
        }

        private void HttpFormUserHtml_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.StartWebServerLocalOnly = checkBoxLocalOnly.Checked;
            Settings.Default.StartWebServer = chkHTTP.Checked;
            Settings.Default.StartWebServerLastState = checkBoxStartLastState.Checked;
            Settings.Default.Save();
        }

        // netsh http add urlacl url=http://*:80/ user=everyone
        private void GiveListenRights()
        {
            string args = @"http add urlacl url=http://*:"+m_strPort+"/ sddl=D:(A;;GX;;;S-1-1-0)";
                //string.Format(@"http add urlacl url={0} user={1}\{2}", address, domain, user);

            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;

            Process.Start(psi).WaitForExit();
        }

        private void HttpFormUserHtml_Load(object sender, EventArgs e)
        {
        }
    }
}
