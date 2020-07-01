using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace ST_510configurar
{
    public partial class UpgradeConnectionForm : Form
    {
        string PortName;
        public UpgradeConnectionForm(string s)
        {
            InitializeComponent();
            PortName = s;
        }

        private void UpgradeConnectionForm_Load(object sender, EventArgs e)
        {
            BootSerialPort.PortName = PortName;
            BootSerialPort.BaudRate = 9600;
            BootSerialPort.Parity = Parity.None;       ; 
            BootSerialPort.Open();
            BootTimer.Interval = 10;
            BootTimer.Enabled = true;
        }
        bool isGetBootinfo = false; bool isSendAck = false;

        private void CancelButton_Click(object sender, EventArgs e)
        {
            BootSerialPort.Close();
            this.Close();
        }

        private void UpgradeConnection_FormClosing(object sender, FormClosingEventArgs e)
        {
            BootTimer.Enabled = false;
            BootSerialPort.Close();
        }
        int TimeCounter = 0;
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }
        private void BootSerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            TimeCounter = 0;
            isGetBootinfo = true;
            string rev = BootSerialPort.ReadExisting();
            if (rev.Contains("...") && !isSendAck)
            {
                //Thread.Sleep(1000);
                BootSerialPort.Write("a");
                isSendAck = true;
                //BootTimer.Enabled = true;
            }
            ProbeIfoRichTextBox.AppendText(rev);
            ProbeIfoRichTextBox.SelectionStart = ProbeIfoRichTextBox.Text.Length;
            ProbeIfoRichTextBox.ScrollToCaret();
        }

        private void BootTimer_Tick_1(object sender, EventArgs e)
        {
            if (TimeCounter > 10000)
            {
                TimeCounter = 0;
            }
            TimeCounter++;
            if (TimeCounter > 150 && isGetBootinfo)
            {
                if (ProbeIfoRichTextBox.Text.Contains("Download Image To the Porbe Internal Flash") || ProbeIfoRichTextBox.Text.Length > 100)
                {
                    BootTimer.Enabled = false;
                    byte[] buf = new byte[1];
                    BootSerialPort.Write("1");
                    //int length=BootSerialPort.Read(buf, 0, 1);
                    //Delay(2000);
                    this.DialogResult = DialogResult.Yes;
                    BootSerialPort.Close();
                    this.Close();
                }
                //else if (ProbeIfoRichTextBox.Text.Length > 100)
                //{
                //    BootTimer.Enabled = false;
                //    this.DialogResult = DialogResult.Yes;
                //    BootSerialPort.Close();
                //    this.Close();
                //}
                //else 
                //{
                //    BootTimer.Enabled = false;
                //    this.DialogResult = DialogResult.No;
                //    BootSerialPort.Close();
                //    this.Close();
                //}

            }
        }
    }
}
