/*
 *		A simple demo app to show the AmuletUSB object being
 *		used to monitor the Amulet link status
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using AmuletUSB.Properties;
using Microsoft.Win32;
using System.Diagnostics;


namespace AmuletJUSB
{
    public partial class AmuletForm : Form
    {
        public static AmuletUSB Amulet;
        public System.Windows.Forms.Timer timerDeviceChanged,timerRestart;
        private Boolean _lastMicStateOn = false;



        public AmuletForm()
        {
            InitializeComponent();
            initTimers();
            
            Amulet = new AmuletUSB();
            AmuletUSB.USBChanged += AmuletUSB_USBChanged;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            // Populate fields initially
            try
            {
                PollUSBonce();
                groupBoxStatus.Text = "Amulet USB Monitor - version " + Application.ProductVersion;
            }
            catch (Exception err) { MessageBox.Show(err.ToString()); }
        }

        void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                //                                        --- SLEEP EVENT ---
                timerDeviceChanged.Stop();                
                AmuletUSB.USBChanged -= AmuletUSB_USBChanged;
                Amulet.stopIt(); 
                this.Text = "System suspended...";
            }
            else if (e.Mode == PowerModes.Resume)
            {
                newRestartMethod();
                //                                         --- WAKE EVENT ---
             
                this.Text="Resume: Restart in 15 seconds!";
                timerRestart.Start();                
            }
        }

        private void newRestartMethod()
        {
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = "/C ping 127.0.0.1 -n 8 && \"" + Application.ExecutablePath + "\"";
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);
            Application.Exit(); 
        }

              
        void initTimers()
        {
            timerDeviceChanged = new System.Windows.Forms.Timer();
            timerDeviceChanged.Interval = 4000;
            timerDeviceChanged.Tick += new EventHandler(deviceChangeTimer_Tick);                        
        }        

        private void setLinkMessage(String msg)
        {
            lblLinkStatus.Text = "Link Status: " + msg;
        }

        private void setMicMessage(String msg)
        {
            lblMicStatus.Text = "Mic Status: " + msg;
        }

        private void deviceChangeTimer_Tick(object sender, EventArgs e)
        {
            // Stop the timer, and update the USB info
            timerDeviceChanged.Stop();
            //DebugLine("Updating USB status in response to device change");
            PollUSBonce();
        }

        // Our override window message handler, so we can see DeviceChange messages
        const uint WM_DEVICECHANGE = 537;
        const uint DBT_DEVNODES_CHANGED = 0x0007;

        protected override void WndProc(ref Message message)
        {
            try
            {
                // Debug.WriteLine("WndProc: Message = " + message.Msg + ", wParam=" + message.WParam + ", lParam=" + message.LParam);
                uint msg = (uint)message.Msg;
                IntPtr wParam = message.WParam;
                IntPtr lParam = message.LParam;

                switch (msg)
                {
                    case WM_DEVICECHANGE:
                        if (wParam == (IntPtr)DBT_DEVNODES_CHANGED)
                        {
                            //DebugLine("WndProc: Device change, updating...");
                            
                            //REMEMBER TO TURN THIS BACK ON !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!                            
                            timerDeviceChanged.Start();	// Start the timer (if it was already running, no problem; let it keep running)
                            
                            
                            //Debug.WriteLine("WndProc: Device changed, reset audio input device!");
                            //PollUSB();
                        }
                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("error: " + err.ToString());
            }
            finally { }
            base.WndProc(ref message);
        }

        void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();            
        }

        public static void DebugLine(string msg)
        {
            //AmuletUSB.DebugLine(msg);
        }

        // Query USB interface for current link status
        public void PollUSBonce()
        {
            if (Amulet.AmuletConnect())
                setLinkMessage("Connected");
            else
                setLinkMessage("Unplugged");
        }

        public delegate void USBDelegate(AmuletUSB amulet);

        // This event gets generated on the USB polling thread, so
        // pass it back to the main UI thread using this.Invoke()
        // so we can update the UI display safely.
        void AmuletUSB_USBChanged(AmuletUSB sender, object dummy)
        {
            this.Invoke(new USBDelegate(USB_Update), sender);
            // DebugLine("Connected=" + sender.Connected + ", Link=" + sender.RadioActive + " Mic=" + sender.MicActive);
        }

        public void USB_Update(AmuletUSB amulet)
        {
            if (amulet.Connected)
            {
                if (amulet.RadioActive)
                {
                    processMicState(amulet.MicActive);
                    setLinkMessage("Link up");
                    notifyIcon1.Text = "Link up";
                }
                else
                {
                    setLinkMessage("Link down");
                    notifyIcon1.Text = "Link down";
                }
            }
            else
            {
                lblLinkStatus.Text = "Link Status: No Amulet detected";
                processMicState(false);
            }
        }

        private void processMicState(bool isMicOn)
        {
            if (isMicOn != _lastMicStateOn)
            {
                //status changed so send an event message on UDP
                if (isMicOn)
                {
                    setMicMessage("On");
                    mySendUdp("Event&&Amulet.On", (int)numDelayOn.Value);
                }
                else
                {
                    setMicMessage("Off");
                    mySendUdp("Event&&Amulet.Off", (int)numDelayOff.Value);
                }
            }
            _lastMicStateOn = isMicOn;
        }
        private void mySendUdp(string strMessage, int delay)
        {
            String strIP = "255.255.255.255";
            if (radTargetIP.Checked)
            {
                strIP = txtTargetIP.Text;
            }
            Thread.Sleep(delay);
            {
                UdpClient udp = new UdpClient();

                int GroupPort = 33000; ;          //use port specified in options if possible or default to 33000
                try { GroupPort = Convert.ToInt32(numUdpPort.Value); }
                catch {  }

                IPEndPoint groupEP = new IPEndPoint(IPAddress.Parse(strIP), GroupPort);
                //byte[] sendBytes4 = Encoding.ASCII.GetBytes(mymessage);
                byte[] sendBytes4 = Encoding.Default.GetBytes(strMessage);
                udp.Send(sendBytes4, sendBytes4.Length, groupEP);
                udp.Close();
            }
        }
        private void AmuletForm_Load(object sender, EventArgs e)
        {
            numUdpPort.Value = (int)Settings.Default["udpPort"];
            numDelayOff.Value = (int)Settings.Default["delayOff"];
            numDelayOn.Value = (int)Settings.Default["delayOn"];

            radBroadcast.Checked = (Boolean)Settings.Default["broadcast"];
            radTargetIP.Checked = !radBroadcast.Checked;
            txtTargetIP.Text = (String)Settings.Default["targetIP"];
            if (Amulet.Connected)
            {
                this.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void toggleVisible()
        {
            this.Visible = !this.Visible;
            if (this.Visible)
            {
                this.TopMost = true;
                this.TopMost = false;
            }
        }


        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                toggleVisible();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AmuletForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //save settings
            Settings.Default["udpPort"] = (int)numUdpPort.Value;
            Settings.Default["delayOff"] = (int)numDelayOff.Value;
            Settings.Default["delayOn"] = (int)numDelayOn.Value;
            Settings.Default["broadcast"] = (Boolean)radBroadcast.Checked;
            Settings.Default["targetIP"] = (String)txtTargetIP.Text;
            Settings.Default.Save();

            notifyIcon1.Dispose();
            notifyIcon1 = null;
        }

        private void showHideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleVisible();
        }

        private void timerAutoHide_Tick(object sender, EventArgs e)
        {
            timerAutoHide.Stop();

            if (Amulet.Connected)
            {
                this.Visible = false;
            }
        }

        private void radTargetIP_CheckedChanged(object sender, EventArgs e)
        {
            txtTargetIP.Enabled = radTargetIP.Checked;
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            newRestartMethod();
        }
    }
}
