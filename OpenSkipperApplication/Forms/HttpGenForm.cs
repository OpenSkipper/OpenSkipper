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

namespace OpenSkipperApplication.Forms
{
    public partial class HttpGenForm : Form
    {
        private HttpListener listener = new HttpListener();
        private HttpListener feedListener = new HttpListener();
        private Parameter dummyParameter;

        private Parameter p1;
        private Parameter p2;
        private Parameter p3;
        private Parameter p4;

        public HttpGenForm()
        {
            InitializeComponent();

            dummyParameter = new MultipleSourceNumeric() { InternalName = "<None>" };

            comboBox1.DisplayMember = "InternalName";
            comboBox2.DisplayMember = "InternalName";
            comboBox3.DisplayMember = "InternalName";
            comboBox4.DisplayMember = "InternalName";

            comboBox1.Items.Add(dummyParameter);
            comboBox2.Items.Add(dummyParameter);
            comboBox3.Items.Add(dummyParameter);
            comboBox4.Items.Add(dummyParameter);

            foreach (Parameter p in Definitions.ParamCol.ClonedParameters)
            {
                comboBox1.Items.Add(p);
                comboBox2.Items.Add(p);
                comboBox3.Items.Add(p);
                comboBox4.Items.Add(p);
            }

            comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
            comboBox2.SelectedIndexChanged += new EventHandler(comboBox2_SelectedIndexChanged);
            comboBox3.SelectedIndexChanged += new EventHandler(comboBox3_SelectedIndexChanged);
            comboBox4.SelectedIndexChanged += new EventHandler(comboBox4_SelectedIndexChanged);

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0; 
        }

        void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            p4 = (Parameter)comboBox4.SelectedItem;
        }
        void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            p3 = (Parameter)comboBox3.SelectedItem;
        }
        void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            p2 = (Parameter)comboBox2.SelectedItem;
        }
        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            p1 = (Parameter)comboBox1.SelectedItem;
        }

        // HTTP
        private void chkHTTP_CheckedChanged(object sender, EventArgs e)
        {
            const string primaryPrefix = "http://*:80/";
            const string backupPrefix = "http://localhost:8080/";
            const string feedExt = "feed/";

            if (chkHTTP.CheckState == CheckState.Checked)
            {
                listener = new HttpListener();
                feedListener = new HttpListener();

                listener.Prefixes.Add(primaryPrefix);
                feedListener.Prefixes.Add(primaryPrefix + feedExt);

                try
                {
                    listener.Start();
                    feedListener.Start();

                    listener.BeginGetContext(listenerCallback, null);
                    feedListener.BeginGetContext(feedCallback, null);

                    richTextStatus.Text = "Accepting connections on:";
                    foreach (IPAddress ipAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                        richTextStatus.Text += "\r\nhttp:\\\\" + ipAddress.ToString();
                }
                catch (Exception ex)
                {
                    DialogResult dr = MessageBox.Show("Failed to start Web server: " + ex.Message + "\r\n\r\nListen on localhost? (Depending on your configuration, this may allow you to test the web server using a browser on this machine.)", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (dr == DialogResult.Yes)
                    {
                        try
                        {
                                listener = new HttpListener();
                            feedListener = new HttpListener();

                                listener.Prefixes.Add(backupPrefix);
                            feedListener.Prefixes.Add(backupPrefix + feedExt);

                            listener.Start();
                            feedListener.Start();

                            listener.BeginGetContext(listenerCallback, null);
                            feedListener.BeginGetContext(feedCallback, null);

                            richTextStatus.Text = "Accepting connections on:\r\nhttp://localhost";
                        }
                        catch
                        {
                            MessageBox.Show("Failed to start HTTP listener: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            chkHTTP.CheckState = CheckState.Unchecked;
                        }
                    }
                    else
                    {
                        chkHTTP.CheckState = CheckState.Unchecked;
                    }

                    return;
                }
            }
            else
            {
                if (listener.IsListening)
                    listener.Stop();

                if (feedListener.IsListening)
                    feedListener.Stop();

                richTextStatus.Text = "Web server disabled.";
            }
        }

        private void listenerCallback(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext httpContext = listener.EndGetContext(ar);
                HttpListenerResponse httpResponse = httpContext.Response;

                //--------------------------------------------------------------------------

                CANHandler.ReportHandler.LogInfo("Received listener request: " + httpContext.Request.RawUrl);

                // Construct a response.
                string responseString = string.Format(htmlBlock, ((p1 == dummyParameter) ? "N/A" : p1.DisplayName),
                                                                 ((p2 == dummyParameter) ? "N/A" : p2.DisplayName),
                                                                 ((p3 == dummyParameter) ? "N/A" : p3.DisplayName),
                                                                 ((p4 == dummyParameter) ? "N/A" : p4.DisplayName)) + scriptBlock;

                byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);

                //--------------------------------------------------------------------------

                // Send the response
                httpResponse.ContentLength64 = responseBytes.Length;
                httpResponse.ContentEncoding = Encoding.UTF8;

                Stream output = httpResponse.OutputStream;
                output.Write(responseBytes, 0, responseBytes.Length);
                output.Close();
                
                listener.BeginGetContext(listenerCallback, null);
            }
            catch
            {
                // Occurs when listener is stopped.
                return;
            }
        }
        private void feedCallback(IAsyncResult ar)
        {
            try
            {
                HttpListenerContext httpContext = feedListener.EndGetContext(ar);
                HttpListenerResponse httpResponse = httpContext.Response;

                //--------------------------------------------------------------------------

                CANHandler.ReportHandler.LogInfo("Received web data feed listener request: " + httpContext.Request.RawUrl);

                // Construct a response.
                string responseString = "";
                responseString += "u('t1','" + ((p1 == dummyParameter) ? "-" : p1.ToString().Replace("\r\n", "<br />")) + "');";
                responseString += "u('t2','" + ((p2 == dummyParameter) ? "-" : p2.ToString().Replace("\r\n", "<br />")) + "');";
                responseString += "u('t3','" + ((p3 == dummyParameter) ? "-" : p3.ToString().Replace("\r\n", "<br />")) + "');";
                responseString += "u('t4','" + ((p4 == dummyParameter) ? "-" : p4.ToString().Replace("\r\n", "<br />")) + "');";

                byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);

                //--------------------------------------------------------------------------

                // Send the response
                httpResponse.ContentLength64 = responseBytes.Length;
                httpResponse.ContentEncoding = Encoding.UTF8;

                Stream output = httpResponse.OutputStream;
                output.Write(responseBytes, 0, responseBytes.Length);
                output.Close();

                feedListener.BeginGetContext(feedCallback, null);
            }
            catch
            {
                // Occurs when listener is stopped.
                return;
            }
        }

        private void HttpGenForm_Load(object sender, EventArgs e)
        {

        }

        // HTML
        private string htmlBlock = @"
<HTML>
    <HEAD>
        <meta http-equiv=""Content-Protocol"" content=""text/html; charset=UTF-8"" />
    </HEAD>
    <BODY>
        <center>
            Open Skipper Live Feed
            <br><br>
            <table border=""1"">
                <tr>
                    <td align=center><font size=""5"" color=""red""><span name=""t2"" id=""t1""></span></font><br>{0}</td>
                    <td align=center><font size=""5"" color=""red""><span name=""t2"" id=""t2""></span></font><br>{1}</td>
                </tr>
                <tr>
                    <td align=center><font size=""5"" color=""red""><span name=""t3"" id=""t3""></span></font><br>{2}</td>
                    <td align=center><font size=""5"" color=""red""><span name=""t4"" id=""t4""></span></font><br>{3}</td>
                </tr>
            </table> 
            <br>
        </center>
        
        <!--- The following is used to show errors to the user; it is blank if no error occurs. --->
        <span name=""error"" id=""error""></span><br>
    </BODY>
</HTML>
";

        private string scriptBlock = @"
<script type=""text/javascript"" language=""javascript"">
   var http_request = false;
   function makeRequest(url, parameters) {
      http_request = false;
      if (window.XMLHttpRequest) { // Mozilla, Safari,...
         http_request = new XMLHttpRequest();
         if (http_request.overrideMimeType) {
         	// set type accordingly to anticipated content type
            //http_request.overrideMimeType('text/xml');
            http_request.overrideMimeType('text/html');
         }
      } else if (window.ActiveXObject) { // IE
         try {
            http_request = new ActiveXObject(""Msxml2.XMLHTTP"");
         } catch (e) {
            try {
               http_request = new ActiveXObject(""Microsoft.XMLHTTP"");
            } catch (e) {}
         }
      }
      if (!http_request) {
         alert('Cannot create XMLHTTP instance');
         return false;
      }
      http_request.onreadystatechange = alertContents;
      http_request.open('GET', url + parameters, true);
      http_request.send(null);
   }

   // AJM Test of using POST insead of GET
   function makePOSTRequest(url, parameters) {
      http_request = false;
      if (window.XMLHttpRequest) { // Mozilla, Safari,...
         http_request = new XMLHttpRequest();
         if (http_request.overrideMimeType) {
         	// set type accordingly to anticipated content type
            //http_request.overrideMimeType('text/xml');
            http_request.overrideMimeType('text/html');
         }
      } else if (window.ActiveXObject) { // IE
         try {
            http_request = new ActiveXObject(""Msxml2.XMLHTTP"");
         } catch (e) {
            try {
               http_request = new ActiveXObject(""Microsoft.XMLHTTP"");
            } catch (e) {}
         }
      }
      if (!http_request) {
         alert('Cannot create XMLHTTP instance');
         return false;
      }
      
      http_request.onreadystatechange = alertContents;
      http_request.open('POST', url, true);
      http_request.setRequestHeader(""Content-type"", ""application/x-www-form-urlencoded"");
      http_request.setRequestHeader(""Content-length"", parameters.length);
      http_request.setRequestHeader(""Connection"", ""close"");
      http_request.send(parameters);
   }
	function u(a,b) {
	   document.getElementById(a).innerHTML = b;
	}
   function alertContents() {
      if (http_request.readyState == 4) {
         if (http_request.status == 200) {
            //alert(http_request.responseText);
			var response = """"; // This is needed for IE8 to work
            response = http_request.responseText;
            // Display the returned string for debugging purposes
            // document.getElementById('response').innerHTML = response;
            // We execute the Javascript commands returned by the server
            eval(response);
            // Clear any previous error
            document.getElementById('error').innerHTML = """";
         } else {
            // alert('There was a problem with the request.');
            document.getElementById('error').innerHTML = ""Error: No response received when requesting updated data from the server."";
         }
      }
   }
var tester = 0;
function UpdateLiveData() {
	// The {1} is replaced by a counter that distinguishes individual connections
	// We send this counter value as a parameter in our 
	// makeRequest('feed', '?id={1}&v=1'); // This uses the GET method
	makePOSTRequest('feed', ''); // This uses the POST method
	setTimeout(""UpdateLiveData()"",1000) 
}

UpdateLiveData();

</script>";

        private void richTextStatus_LinkClicked(object sender, LinkClickedEventArgs e) {
            try {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch (Exception ex) {
                MessageBox.Show("An error occurred opening the URL: \r\n" + ex.ToString() );
            }
        }
    }
}
