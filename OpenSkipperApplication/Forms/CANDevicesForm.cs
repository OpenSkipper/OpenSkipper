using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CANDevices;

namespace OpenSkipperApplication.Forms
{
    public partial class CANDevicesForm : Form
    {
        private List<CANDevice> DisplayedDevices; // Holds devics currently displayed
        private bool firstShow = true;
        private bool needUpdate = false;

        public CANDevicesForm()
        {
            InitializeComponent();
            DisplayedDevices = new List<CANDevice> { };
        }

        private void DeviceListChange()
        {
            needUpdate = true;
        }
        private void UpdateDisplay()
        {
            dataGridView1.RowCount = CANDeviceList.Devices.Count + 1; // there is an extra "add a new row" that I cannot remove
            dataGridView1.Refresh();
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDetails();
        }

        private void CANDevicesForm_Load(object sender, EventArgs e)
        {
            this.dataGridView1.VirtualMode = true;
            this.dataGridView1.AllowUserToAddRows = false;  // This seems to be staying as true even tho it is false in the designer

            UpdateDisplay();
            CANDeviceList.DeviceListChange += DeviceListChange;

            // For first showing of form, we should pop up the stream selector before continuing (Showing the decoded stream form)
            if (firstShow)
            {
                firstShow = false;
            }
            timer1.Enabled = true;
        }

        private void CANDevicesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CANDeviceList.DeviceListChange -= DeviceListChange;
        }

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == this.dataGridView1.RowCount - 1)
            {
                UpdateDetails();
                return;
            }

            // Set the cell value to paint using the Customer object retrieved.
            if (e.RowIndex >= 0 && e.RowIndex < CANDeviceList.Devices.Count)
            {
                CANDevice dev;
                lock (CANDeviceList.Lock)
                {
                    dev = CANDeviceList.Devices.ElementAt(e.RowIndex).Value;
                }
                lock (dev.Lock)
                {
                    switch (this.dataGridView1.Columns[e.ColumnIndex].Name)
                    {
                        case "Stream": e.Value = dev.StreamName; break;
                        case "Source": e.Value = dev.Source; dataGridView1.Rows[e.RowIndex].Tag=dev.Source; break;
                        case "UniqueNumber": e.Value = dev.UniqueNumber; break;
                        case "Manufacturer": e.Value = dev.ManufacturerCodeStr; break;
                        case "SerialCode": e.Value = dev.ModelSerialCode; break;
                    }
                }
            }
        }

        // Request a screen update of the GridViews and ProopertyGrid
        private void UpdateDetails()
        {
            int selectedIndex = (dataGridView1.SelectedRows.Count > 0) ? dataGridView1.SelectedRows[0].Index : -1;
            CANDevice dev=null;

            lock (CANDeviceList.Lock)
            {
                if (selectedIndex >= 0 && selectedIndex < CANDeviceList.Devices.Count)
                {
                    if (dataGridView1.Rows[selectedIndex].Tag != null)
                    {
                        int Source = (int)dataGridView1.Rows[selectedIndex].Tag;
                        CANDeviceList.Devices.TryGetValue(Source, out dev);
                    }
                }
            }

            if (dev != null)
            {
                richTextBoxDevice.Text = dev.ToString();
            }
            else
            {
                richTextBoxDevice.Text = "";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (needUpdate)
            {
                UpdateDisplay();
                needUpdate = false;
            }
        }
    }
}
