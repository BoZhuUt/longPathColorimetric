
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using Modbus.Device;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Collections;

namespace ST_510configurar
{
    public partial class ST510ProbeConfigurator : Form
    {
        PyxisDevice pST510 = new PyxisDevice("ST510", "PTSA", "ppb", 200, 0, 200, 0);
        PyxisDevice pCyclo = new PyxisDevice("Cyclo", "Cyclo", "ppb", 200, 0, 200, 0);
        PyxisDevice pBleach = new PyxisDevice("Bleach", "Bleach", "%", 2, 0, 2, 0);
        PyxisDevice pClO2 = new PyxisDevice("ClO2", "ClO2", "ppb", 200, 0, 200, 0);
        PyxisDevice pOIW = new PyxisDevice("OIW", "OIW", "ppb", 200, 0, 200, 0);
        PyxisDevice pNDSA = new PyxisDevice("NDSA", "NDSA", "ppm", 20, 0, 20, 0);
        PyxisDevice pChlo = new PyxisDevice("Chlo", "Chlo", "ppm", 20, 0, 20, 0);
        PyxisDevice pHT = new PyxisDevice("HT", "Fluorescein", "ppb", 300, 0, 300, 0);
       PyxisDevice pSPA505L = new PyxisDevice("SPA505L", "CL-F", " ", 300, 0, 300, 0);

        PyxisDevice currentDevice = null;

        const UInt32 PN_ST510 = 50687;
        const UInt32 PN_Cyclo = 50620;
        //const UInt32 PN_TBleach = 50213;
        //const UInt32 PN_ClO2 = 50214;
        const UInt32 PN_TBleach = 50224;  //510电路板做的漂水
        const UInt32 PN_ClO2 = 50225; //510电路板做的ClO2
        const UInt32 PN_Chlo = 50501;
        const UInt32 PN_540NDSA = 50621;  //510电路板做的540SSN，测NDSA
        const UInt32 PN_HM520N_OIW_THERMO = 52113;//510电路板做的水中油，用于Thermo公司
        const UInt32 PN_HT = 65535;  //高温荧光素探头暂未申请pn号
        const UInt32 PN_SPA505L = 12345;

        enum SenorType
        {
            ST510,
            ST540SS,//NDSA
            Chlo,
            Cyclo,
            TOIW,
            TBleach,
            TClO2,
            HT,
            SPA505L,
            Unknow
        }
        SenorType senortype = SenorType.Unknow;

        SerialPort sp1 = new SerialPort();
        bool isApp = false;
        bool isPortExist = false;
        public ST510ProbeConfigurator()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void ST510ProbeConfigurator_Load(object sender, EventArgs e)
        {
            //加载波特率
            //设置DropDownStyle属性，使控件呈下拉列表状态
            BaudComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            BaudComboBox.Items.Add("300");
            BaudComboBox.Items.Add("600");
            BaudComboBox.Items.Add("1200");
            BaudComboBox.Items.Add("2400");
            BaudComboBox.Items.Add("4800");
            BaudComboBox.Items.Add("9600");
            BaudComboBox.Items.Add("19200");
            BaudComboBox.Items.Add("38400");
            BaudComboBox.Items.Add("115200");

            //加载校验
            ParityComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            ParityComboBox.Items.Add("NONE");
            ParityComboBox.Items.Add("ODD");
            ParityComboBox.Items.Add("EVEN");

            Control.CheckForIllegalCrossThreadCalls = false;    //这个类中我们不检查跨线程的调用是否合法(因为.net 2.0以后加强了安全机制,，不允许在winform中直接跨线程访问控件的属性)

            ////准备就绪              
            //sp1.DtrEnable = true;
            //sp1.RtsEnable = true;
            ////设置数据读取超时为1秒
            //sp1.ReadTimeout = 1000;

            MeasurementGroupBox.Enabled = false;
            CalibrationGroupBox.Enabled = false;
            SnSetUpGroupBox.Enabled = false;
            userInfoGroupBox.Enabled = false;
            mATestGroupBox.Enabled = false;
            RefreshButton.Enabled = false;
            SaveParamButton.Enabled = false;
            saveFactoryParamButton.Enabled = false;
            writeProbeButton.Enabled = false;

            sp1.Close();/////////////////////////////

            ReadCycleComboBox.Text = ReadCycleComboBox.Items[1].ToString();
            //表格属性 
            MeasurementChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            MeasurementChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            // MeasurementChart.ChartAreas[0].AxisY.Title = "PTSA(PPb)";
            // MeasurementChart.ChartAreas[0].AxisY2.Title = "HR(%)";

            MeasurementChart.ChartAreas[0].Area3DStyle.Enable3D = false;

            MeasurementChart.Series.Add("value");
            MeasurementChart.Series["value"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            MeasurementChart.Series["value"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            MeasurementChart.Series["value"].IsValueShownAsLabel = false;

            MeasurementChart.Series["value"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            MeasurementChart.Series["value"].Color = Color.Red;
            MeasurementChart.Series["value"].BorderWidth = 1;
            MeasurementChart.Legends.Add("value");
            MeasurementChart.Legends["value"].DockedToChartArea = "mainChartArea";
            MeasurementChart.Legends["value"].IsDockedInsideChartArea = true;

            MeasurementChart.Series.Add("mA");
            MeasurementChart.Series["mA"].Enabled = false;
            MeasurementChart.Series["mA"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            MeasurementChart.Series["mA"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            MeasurementChart.Series["mA"].IsValueShownAsLabel = false;

            MeasurementChart.Series["mA"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            MeasurementChart.Series["mA"].Color = Color.Orange;
            MeasurementChart.Series["mA"].BorderWidth = 1;
            MeasurementChart.Legends.Add("mA");
            MeasurementChart.Legends["mA"].DockedToChartArea = "mainChartArea";
            MeasurementChart.Legends["mA"].IsDockedInsideChartArea = true;

            MeasurementChart.Series.Add("S365");
            MeasurementChart.Series["S365"].Enabled = false;
            MeasurementChart.Series["S365"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            MeasurementChart.Series["S365"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            MeasurementChart.Series["S365"].IsValueShownAsLabel = false;

            MeasurementChart.Series["S365"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            MeasurementChart.Series["S365"].Color = Color.DarkGreen;
            MeasurementChart.Series["S365"].BorderWidth = 1;
            MeasurementChart.Legends.Add("S365");
            MeasurementChart.Legends["S365"].DockedToChartArea = "mainChartArea";
            MeasurementChart.Legends["S365"].IsDockedInsideChartArea = true;

            MeasurementChart.Series.Add("t365");
            MeasurementChart.Series["t365"].Enabled = false;
            MeasurementChart.Series["t365"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            MeasurementChart.Series["t365"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            MeasurementChart.Series["t365"].IsValueShownAsLabel = false;

            MeasurementChart.Series["t365"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            MeasurementChart.Series["t365"].Color = Color.Blue;
            MeasurementChart.Series["t365"].BorderWidth = 1;
            MeasurementChart.Legends.Add("t365");
            MeasurementChart.Legends["t365"].DockedToChartArea = "mainChartArea";
            MeasurementChart.Legends["t365"].IsDockedInsideChartArea = true;

            MeasurementChart.Series.Add("s420");
            MeasurementChart.Series["s420"].Enabled = false;
            MeasurementChart.Series["s420"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            MeasurementChart.Series["s420"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            MeasurementChart.Series["s420"].IsValueShownAsLabel = false;

            MeasurementChart.Series["s420"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            MeasurementChart.Series["s420"].Color = Color.DarkBlue;
            MeasurementChart.Series["s420"].BorderWidth = 1;
            MeasurementChart.Legends.Add("s420");
            MeasurementChart.Legends["s420"].DockedToChartArea = "mainChartArea";
            MeasurementChart.Legends["s420"].IsDockedInsideChartArea = true;

            MeasurementChart.Series.Add("t420");
            MeasurementChart.Series["t420"].Enabled = false;
            MeasurementChart.Series["t420"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            MeasurementChart.Series["t420"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            MeasurementChart.Series["t420"].IsValueShownAsLabel = false;

            MeasurementChart.Series["t420"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            MeasurementChart.Series["t420"].Color = Color.Purple;
            MeasurementChart.Series["t420"].BorderWidth = 1;
            MeasurementChart.Legends.Add("t420");
            MeasurementChart.Legends["t420"].DockedToChartArea = "mainChartArea";
            MeasurementChart.Legends["t420"].IsDockedInsideChartArea = true;

            //检查是否含有串口

            string[] serialPortName = SerialPort.GetPortNames();

            if (serialPortName.Length != 0)
            {
                PortcomboBox.Items.AddRange(serialPortName);
                PortcomboBox.Text = serialPortName[0];
                ParityComboBox.Text = ParityComboBox.Items[2].ToString();
                BaudComboBox.Text = BaudComboBox.Items[5].ToString();

                isPortExist = true;
            }
            else
            {
                MessageBox.Show("Not Found Port");

                isPortExist = false;
                MeasurementTimer.Enabled = true;
                MeasurementTimer.Start();  //在这里循环检测端口是否存在，导致界面无法显示，所以在中断里检测端口

                // Application.Exit();
            }

            //添加串口项目
            //foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            //{//获取有多少个COM口
            //    //System.Diagnostics.Debug.WriteLine(s);
            //    PortcomboBox.Items.Add(s);
            //}
            //sp1.BaudRate = 9600;
        }
        IModbusSerialMaster master;

        byte modbusAddr;
        string measureValuePath;
        string probeParamPath;
        OperateReg rwReg = new OperateReg();
        bool isConnect = false;
        string swString;
        string strBtleAdverStatus;
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            uint count = 0;
            PortcomboBox.Items.Clear();
            string[] portname = SerialPort.GetPortNames();
            //if (portname.Length != 0)
            //{
            //    PortcomboBox.Items.AddRange(portname);
            //    PortcomboBox.Text = PortcomboBox.Items[0].ToString();
            //    BaudComboBox.Text = BaudComboBox.Items[5].ToString();
            //    ParityComboBox.Text = ParityComboBox.Items[0].ToString();
            //}
            if (ConnectButton.Text == "Connect")
            {
                while ((ConnectButton.Text != "Disconnect") && (++count < 3))
                {
                    ToolStripStatusLabel.Text = "wait a moment ...";

                    if (PortcomboBox.Text != "")
                    {
                        sp1.PortName = PortcomboBox.Text;
                        sp1.BaudRate = int.Parse(BaudComboBox.Text);
                        if (ParityComboBox.Text == "NONE")
                        {
                            sp1.Parity = Parity.None;
                        }
                        else if (ParityComboBox.Text == "ODD")
                        {
                            sp1.Parity = Parity.Odd;
                        }
                        else if (ParityComboBox.Text == "EVEN")
                        {
                            sp1.Parity = Parity.Even;
                        }
                        try
                        {
                            try
                            {
                                byte[] cmd = {
                                     0x00,0x03,0xa0,0x2c,0x00,0x01,0x66,0x12
                                 };
                                byte[] res = new byte[7];
                                sp1.Open();
                                sp1.Write(cmd, 0, cmd.Length);
                                //sp1.ReadTimeout = 5000;
                                sp1.ReadTimeout = 1000;
                                Thread.Sleep(500);
                                int length = sp1.Read(res, 0, 7);
                                sp1.Close();
                                if (length != 0)
                                {
                                    ModbusAddrTextBox.Text = res[0].ToString();
                                    modbusAddr = Convert.ToByte(res[0]);
                                }
                                else
                                {
                                    ToolStripStatusLabel.Text = "get modbusAddr failed,no response.";
                                    return;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                //MessageBox.Show(ex.Message);
                                //ToolStripStatusLabel.Text = "no probe!";
                                sp1.Close();
                                // return;
                            }

                            //get modbus addr
                            sp1.Open();
                            master = ModbusSerialMaster.CreateRtu(sp1);
                            //master.Transport.ReadTimeout = 2000;
                            //master.Transport.WriteTimeout = 2000;
                            master.Transport.ReadTimeout = 1000;
                            master.Transport.WriteTimeout = 1000;
                            try
                            {
                                ushort[] productNum = master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 41005 - 1, 2);
                                UInt32 pn = productNum[1];
                                pn <<= 16;
                                pn += productNum[0];
                                switch (pn)
                                {
                                    // case 0: break;
                                    case PN_ST510:
                                        {
                                            if ((sp1.BaudRate == 19200) && (sp1.Parity == Parity.Odd))
                                            {
                                                sT510TOIWToolStripMenuItem_Click(null, null); //早期510水中油for thermo使用了ST-510的pn
                                            }
                                            else
                                            {
                                                sT510ToolStripMenuItem_Click(null, null);
                                            }
                                            break;
                                        }
                                    case PN_HM520N_OIW_THERMO:
                                        {
                                            sT510TOIWToolStripMenuItem_Click(null, null);
                                            break;
                                        }
                                    case PN_540NDSA:
                                        {
                                            sT540NDSAToolStripMenuItem1_Click(null, null);
                                            break;
                                        }
                                    case PN_Chlo:
                                        {
                                            chloToolStripMenuItem_Click(null, null);
                                            break;
                                        }
                                    case PN_Cyclo:
                                        {
                                            sT510CycloToolStripMenuItem_Click(null, null);
                                            break;
                                        }
                                    case PN_TBleach:
                                        {
                                            sT510TBleachToolStripMenuItem_Click(null, null);
                                            break;
                                        }
                                    case PN_ClO2:
                                        {
                                            sT510TClO2ToolStripMenuItem_Click(null, null);
                                            break;
                                        }
                                    case PN_HT:
                                        {
                                            HTtoolStripMenuItem_Click(null, null);
                                            break;
                                        }
                                    case PN_SPA505L:
                                        {
                                            SPA505LtoolStripMenuItem_Click(null, null);
                                            break;
                                        }
                                    default:
                                        {
                                            typeLabel.Text = "unknow";
                                            senortype = SenorType.Unknow;
                                            break;
                                        }
                                }
                                if (senortype == SenorType.Unknow)
                                {
                                    MessageBox.Show("Unrecognized probe, please select in the drop-down box or check the pn.");
                                }
                                ushort[] serialnum = master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 41015 - 1, 8);
                                char[] snChar = new char[serialnum.Length * 2];
                                for (int i = 0, j = 0; i < serialnum.Length; i++)
                                {
                                    snChar[j] = (char)(serialnum[i] & 0x00FF); j++;
                                    snChar[j] = (char)(serialnum[i] >> 8); j++;
                                }
                                string snString = new string(snChar);
                                SnTextBox.Text = snString;

                                ushort[] swVersion = master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 41031 - 1, 8);
                                char[] swChar = new char[swVersion.Length * 2];
                                for (int i = 0, j = 0; i < swVersion.Length; i++)
                                {
                                    swChar[j] = (char)(swVersion[i] & 0x00FF); j++;
                                    swChar[j] = (char)(swVersion[i] >> 8); j++;
                                }
                                swString = new string(swChar);

                                ushort[] email = master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 45001 - 1, 16);
                                char[] emailChar = new char[email.Length * 2];
                                for (int i = 0, j = 0; i < email.Length; i++)
                                {
                                    emailChar[j] = (char)(email[i] & 0x00FF); j++;
                                    emailChar[j] = (char)(email[i] >> 8); j++;
                                }
                                string emailString = new string(emailChar);
                                userEmailTextBox.Text = emailString;

                                if (senortype == SenorType.HT)
                                {
                                    bleButton.Enabled = false;
                                    ToolStripStatusLabel.Text = "connected " + "Senor Type: " + senortype.ToString() + "    swVer: " + swString;
                                }
                                else if (senortype == SenorType.SPA505L)
                                {
                                    bleButton.Enabled = false;
                                    ToolStripStatusLabel.Text = "connected " + "Senor Type: " + senortype.ToString() + "    swVer: " + swString;
                                }
                                else
                                {
                                    ushort[] btleData = master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 41041 - 1, 3);
                                    ushort btleExistStatus = btleData[0];
                                    ushort btleAdverStatus = btleData[2];
                                    bool strBtleExist;
                                    strBtleAdverStatus = " ";
                                    if (btleExistStatus == 1)
                                    {
                                        //strBtleExist = true;
                                        bleButton.Enabled = true;
                                        //bleButton.Text = "Stop Adver";
                                        switch (btleAdverStatus)
                                        {
                                            case 0:
                                                {
                                                    bleButton.Text = "Start Adver";
                                                    strBtleAdverStatus = "Advertisement is stoped.";
                                                    break;
                                                }

                                            case 1:
                                                {
                                                    bleButton.Text = "Stop Adver";
                                                    strBtleAdverStatus = "Advertising...";
                                                    break;
                                                }
                                            default: break;
                                        }
                                    }
                                    else
                                    {
                                        strBtleExist = false;
                                        bleButton.Enabled = false;
                                        bleButton.Text = "Start Adver";
                                    }

                                    ToolStripStatusLabel.Text = "connected " + "Senor Type: " + senortype.ToString() + "    swVer: " + swString + strBtleAdverStatus;
                                }

                                ConnectButton.Text = "Disconnect";
                                MeasurementGroupBox.Enabled = true;
                                CalibrationGroupBox.Enabled = true;
                                if (senortype == SenorType.HT)
                                {
                                    mATestGroupBox.Enabled = false;
                                }
                                else
                                {
                                    mATestGroupBox.Enabled = true;
                                }
                                FirmUpgradeGroupBox.Enabled = true;
                                SnSetUpGroupBox.Enabled = true;
                                userInfoGroupBox.Enabled = true;
                                isConnect = true;
                                panel3.Enabled = true;
                                RefreshButton.Enabled = true;
                                SaveParamButton.Enabled = true;
                                saveFactoryParamButton.Enabled = true;
                                writeProbeButton.Enabled = true;
                                isApp = true;
                                isPortExist = true;
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message);
                                ToolStripStatusLabel.Text = ex.Message;
                                sp1.Close();
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            //MeasureSerialPort.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No port available!", "Error");
                    }
                }
                if (count >= 3)
                {
                    count = 0;
                    MessageBox.Show("No probe available!");  //第3次连接失败，则不再尝试连接
                }
            }

            else
            {
                MeasurementTimer.Enabled = false;
                sp1.Close();
                ConnectButton.Text = "Connect";
                ToolStripStatusLabel.Text = "No Connection";
                MeasurementGroupBox.Enabled = false;
                CalibrationGroupBox.Enabled = false;
                mATestGroupBox.Enabled = false;
                // FirmUpgradeGroupBox.Enabled = false;
                SnSetUpGroupBox.Enabled = false;
                RefreshButton.Enabled = false;
                //SaveParamButton.Enabled = false;
                //writeProbeButton.Enabled = false;
                panel3.Enabled = false;
                isConnect = false;
                isdataView = false;
            }
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            measureValuePath = SnTextBox.Text + "_measureValue.csv";
            if (ReadButton.Text == "Read")
            {
                try
                {
                    if (!File.Exists(measureValuePath))
                    {
                        //File.Create(SnTextBox.Text + ".csv");
                        //File.WriteAllText(measureValuePath, "date,PTSA,t365,s365,t420,s420,HM,Temp\r\n");
                        if (senortype == SenorType.HT)
                        {
                            File.WriteAllText(measureValuePath, "date,measureValue,temperature,t365,s365,t420,s420\r\n");
                        }
                        else
                        {
                            File.WriteAllText(measureValuePath, "date,measureValue,t365,s365,t420,s420\r\n");
                        }
                    }
                    MeasurementTimer.Interval = int.Parse(ReadCycleComboBox.Text) * 1000;
                    // MeasurementTimer.Interval = 2000;                   
                    ReadButton.Text = "StopReading";
                    ConnectionGroupBox.Enabled = false;
                    CalibrationGroupBox.Enabled = false;
                    FirmUpgradeGroupBox.Enabled = false;
                    SnSetUpGroupBox.Enabled = false;
                    userInfoGroupBox.Enabled = false;
                    SPA505LgroupBox.Enabled = false;
                    ST500CalibrationGroupBox.Enabled = false;
                    panel3.Enabled = false;
                    MeasurementTimer.Enabled = true;
                    MeasurementTimer.Start();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MeasurementTimer.Enabled = false;
                ReadButton.Text = "Read";
                ConnectionGroupBox.Enabled = true;
                CalibrationGroupBox.Enabled = true;
                FirmUpgradeGroupBox.Enabled = true;
                SnSetUpGroupBox.Enabled = true;
                panel3.Enabled = true;
                SPA505LgroupBox.Enabled = true;
                ST500CalibrationGroupBox.Enabled = true;
            }
            /* 每次调用master.Dispose()之后，若读写寄存器，需重新初始化master */
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(sp1); //master.Dispose()
            master.Transport.ReadTimeout = 1000;
            master.Transport.WriteTimeout = 1000;
            if (!sp1.IsOpen) sp1.Open();
        }

        enum runMode : byte
        {
            measureMode = 1,
            mATestMode
        };
        runMode probeMode = runMode.measureMode;

        UInt16 readTimeout = 0;
        UInt16 mATestCount = 0;
        private void MeasurementTimer_Tick_1(object sender, EventArgs e)
        {
            if (isPortExist != true)
            {
                string[] serialPortName = SerialPort.GetPortNames();

                if (serialPortName.Length != 0)
                {
                    PortcomboBox.Items.AddRange(serialPortName);
                    PortcomboBox.Text = serialPortName[0];
                    ParityComboBox.Text = ParityComboBox.Items[2].ToString();
                    BaudComboBox.Text = BaudComboBox.Items[5].ToString();

                    MeasurementTimer.Enabled = false;
                    isPortExist = true;
                }
            }
            else if (probeMode == runMode.mATestMode)
            {
                int timer = (int)((float)(120 * 1000) / MeasurementTimer.Interval) + 2; //mA强制输出功能超时大概是120s
                if (mATestCount < timer) ++mATestCount;
                else
                {
                    MeasurementTimer.Enabled = false;
                    probeMode = runMode.measureMode;
                    mATestCount = 0;
                    ReadButton.Enabled = true;
                }
            }
            else
            {
                try
                {
                    Single PTSA, mA, HM, temp,SPA_T365;

                    ushort[] data = master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 46001 - 1, 8);
                    //Delay(1000);///////////////////////test
                    ushort[] ledData = master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 48019 - 1, 4);

                    ushort[] SPA505LData= master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 48002 - 1, 4);
                    //Delay(500);////////////////////test

                    UInt32 tData = ((UInt32)data[1] << 16) + data[0];
                    byte[] temp_bytes = BitConverter.GetBytes(tData);
                    PTSA = BitConverter.ToSingle(temp_bytes, 0);

                    tData = ((UInt32)data[3] << 16) + data[2];
                    temp_bytes = BitConverter.GetBytes(tData);
                    mA = BitConverter.ToSingle(temp_bytes, 0);

                    tData = ((UInt32)data[5] << 16) + data[4];
                    temp_bytes = BitConverter.GetBytes(tData);
                    temp = BitConverter.ToSingle(temp_bytes, 0);

                    SPA_T365 = SPA505LData[2];
                    //tData = ((UInt32)data[7] << 16) + data[6];
                    //temp_bytes = BitConverter.GetBytes(tData);
                    //HM = BitConverter.ToSingle(temp_bytes, 0);

                    DateTime CrtTime = new DateTime();
                    CrtTime = DateTime.Now;
                    if (senortype == SenorType.TBleach)
                    {
                        ptsaValueLable.Text = (PTSA * 100).ToString("F2") + "%" + " " + mA.ToString() + "mA";
                    }
                    else if (senortype == SenorType.SPA505L)
                    {
                        ptsaValueLable.Text = "Gain:"+SPA505LData[1].ToString() + currentDevice.MeasureUnit + " ct:" + SPA505LData[0].ToString() ;
                    }
                    else
                    {
                        ptsaValueLable.Text = PTSA.ToString("F2") + currentDevice.MeasureUnit + " " + mA.ToString() + "mA";
                    }
                    if (senortype == SenorType.HT)
                    {
                        ValueLabel1.Text = "temp: " + SPA_T365.ToString();
                    }
                    else if (senortype == SenorType.SPA505L)
                    {
                        ValueLabel1.Text = "t: " + SPA505LData[3].ToString() + " dark: "+ SPA505LData[2].ToString();
                    }
                    else
                    {
                        ValueLabel1.Text = "s365: " + ledData[1].ToString() + "adc";
                    }
                    //ValueLabel2.Text = "HM: " + HM.ToString("F1") + "%";
                    //ValueLabel3.Text = "Temp: " + temp.ToString("F1") + " ";

                    MeasurementChart.Series["value"].Points.AddXY(CrtTime, PTSA.ToString());
                    MeasurementChart.Series["mA"].Points.AddXY(CrtTime, mA.ToString());
                    MeasurementChart.Series["S365"].Points.AddXY(CrtTime, ledData[1].ToString());
                    //MeasurementChart.Series["t365"].Points.AddXY(CrtTime, ledData[0].ToString());
                    MeasurementChart.Series["s420"].Points.AddXY(CrtTime, ledData[3].ToString());
                    MeasurementChart.Series["t420"].Points.AddXY(CrtTime, ledData[2].ToString());
                    if (senortype == SenorType.SPA505L)
                    {
                        MeasurementChart.Series["t365"].Points.AddXY(CrtTime, SPA505LData[3].ToString());
                    }
                    else
                    {
                        MeasurementChart.Series["t365"].Points.AddXY(CrtTime, ledData[0].ToString());
                    }

                    // File.AppendAllText(measureValuePath, CrtTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + PTSA.ToString("F4") + "," + ledData[0].ToString() + "," + ledData[1].ToString() + "," + ledData[2].ToString() + "," + ledData[3].ToString() + "," + HM.ToString("F1") + "," + temp.ToString("F1") + "\r\n");
                    if (senortype == SenorType.HT)
                    {
                        File.AppendAllText(measureValuePath, CrtTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + PTSA.ToString("F2") + "," + temp.ToString("F2") + "," + ledData[0].ToString() + "," + ledData[1].ToString() + "," + ledData[2].ToString() + "," + ledData[3].ToString() + "\r\n");
                    }
                    else
                    {
                        File.AppendAllText(measureValuePath, CrtTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "," + PTSA.ToString("F4") + "," + ledData[0].ToString() + "," + ledData[1].ToString() + "," + ledData[2].ToString() + "," + ledData[3].ToString() + "\r\n");
                    }
                    readTimeout = 0;
                }
                catch (System.Exception ex)
                {
                    readTimeout++;
                    if (readTimeout >= 2)
                    {
                        readTimeout = 0;
                        MeasurementTimer.Enabled = false;
                        if (DialogResult.OK == MessageBox.Show("The probe is disconnected. Do you want to close the window ?", "warning", MessageBoxButtons.OKCancel))
                        {
                            Application.Exit();  //原定连续两次读取出错，认为探头已断开连接，关闭界面；实际单次超时较久，不知道原因，取单次超时认为断开连接
                        }
                        else
                        {
                            MeasurementTimer.Enabled = false;
                            ReadButton.Text = "Read";   //界面恢复为断开连接时的样子
                            sp1.Close();
                            ConnectButton.Text = "Connect";
                            ConnectionGroupBox.Enabled = true;
                            ToolStripStatusLabel.Text = "No Connection";
                            MeasurementGroupBox.Enabled = false;
                            CalibrationGroupBox.Enabled = false;
                            SnSetUpGroupBox.Enabled = false;
                            RefreshButton.Enabled = false;
                            //SaveParamButton.Enabled = false;
                            //writeProbeButton.Enabled = false;
                            panel3.Enabled = false;
                            isConnect = false;
                            isdataView = false;  //重新连接会刷新参数表
                        }
                    }

                }
                finally
                {
                }

                //sp1.Close();     //每2秒（syscle*1000）读取一次测量值，此处不必频繁开关端口
                //master.Dispose();
            }
        }
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }

        private void SetModbusAddrButton_Click(object sender, EventArgs e)
        {
            string status;
            try
            {
                //master.WriteSingleRegister(byte.Parse(ModbusAddrTextBox.Text), 42001 - 1, byte.Parse(ModbusAddrSetTextBox.Text));
                status = rwReg.parameterSeting(modbusAddr, 42001 - 1, 1, ModbusAddrSetTextBox.Text, "uint16", sp1);
                MessageBox.Show(status);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DICailbButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("Make sure the solution  PTSA = 0 ppb", "DI Calibration", MessageBoxButtons.OKCancel))
            {
                string status;
                try
                {
                    //master.WriteSingleRegister(byte.Parse(ModbusAddrTextBox.Text), 44004 - 1, 4);
                    //Delay(1000);
                    status = rwReg.parameterSeting(modbusAddr, 44004 - 1, 1, 4, "uint16", sp1);
                    if (status == "Success")
                    {
                        MessageBox.Show("DI Calibration is successful, Calib Slope please...");
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        System.Threading.Thread Tx;
        SerialPort UpgradePort = new SerialPort();
        OpenFileDialog UpgradeOpenFileDialog = new OpenFileDialog();//????????????对的？
        private void UpgradeButton_Click_1(object sender, EventArgs e)
        {
            UpgradeOpenFileDialog.Filter = ".bin|*.bin";
            UpgradeOpenFileDialog.FileName = "bin";
            if (UpgradeButton.Text == "Load File")
            {
                if (UpgradeOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FirmFileTextBox.Text = UpgradeOpenFileDialog.FileName;
                    UpgradeButton.Text = "Upgrade";
                }
            }
            else if (UpgradeButton.Text == "Upgrade")
            {
                string tipString = "wait a moment...";

                try
                {
                    if (isConnect == true)
                    {
                        if (!sp1.IsOpen) sp1.Open();
                        master.WriteSingleRegister(byte.Parse(ModbusAddrTextBox.Text), 44004 - 1, 2);
                    }
                    else
                    {
                        tipString = "please power off 2 seconds,then power on";
                    }
                }
                catch (Exception ex)
                {
                    tipString = "please power off 2 seconds,then power on";
                    MessageBox.Show(ex.Message);
                }
                sp1.Close();
                UpgradeConnectionForm upgradeComForm = new UpgradeConnectionForm(PortcomboBox.Text, tipString);
                if (upgradeComForm.ShowDialog() == DialogResult.Yes)
                {
                    ConnectionGroupBox.Enabled = false;
                    CalibrationGroupBox.Enabled = false;
                    MeasurementGroupBox.Enabled = false;
                    mATestGroupBox.Enabled = false;
                    SnSetUpGroupBox.Enabled = false;
                    userInfoGroupBox.Enabled = false;
                    panel3.Enabled = false;
                    upgradeComForm.Dispose();
                    UpgradePort.PortName = PortcomboBox.Text;
                    UpgradePort.BaudRate = 9600;
                    UpgradePort.Parity = Parity.None;
                    UpgradePort.Open();
                    //UpgradePort.Write("1");
                    Tx = new System.Threading.Thread(new System.Threading.ThreadStart(YmodemTx));
                    Tx.IsBackground = true;
                    Tx.Start();
                    UpgradeButton.Enabled = false;
                    //MessageBox.Show("enter bootloader");
                }
                else
                {
                    MessageBox.Show("Upgrade Abort!");
                    if (ConnectButton.Text == "Disconnect")
                    {
                        CalibrationGroupBox.Enabled = true;
                        MeasurementGroupBox.Enabled = true;
                        SnSetUpGroupBox.Enabled = true;
                        userInfoGroupBox.Enabled = true;
                        if (senortype == SenorType.HT)
                        {
                            mATestGroupBox.Enabled = false;
                        }
                        else
                        {
                            mATestGroupBox.Enabled = true;
                        }
                    }
                    ConnectionGroupBox.Enabled = true;
                    sp1.Open();
                }

            }
        }

        /// <summary>
        /// 计算CRC  
        /// </summary>
        /// <param name="bufDate"></param>
        /// <returns></returns>
        public short ComputeCRC(byte[] bufDate)
        {

            short cksum = 0;

            foreach (byte tmpBuf in bufDate)
            {

                cksum = (short)(cksum ^ (short)tmpBuf << 8);
                for (int i = 8; i != 0; i--)
                {
                    if ((cksum & 0x8000) != 0)
                        cksum = (short)(cksum << 1 ^ 0x1021);
                    else
                        cksum = (short)(cksum << 1);
                }
            }

            return cksum;
        }
        enum TxStatus
        {
            START = 0,
            TXING,
            END,
            EXIT

        };
        public void YmodemTx()
        {
            //DisableInterface();

            byte
            SOH = 0x01,
            STX = 0x02,
            EOT = 0x04,
            ACK = 0x06,
            NAK = 0x15,
            CAN = 0x18,
            C = 0x43;
            byte serialNum = 0;
            List<byte> Txlist = new List<byte>();
            byte[] TXbuf = new byte[1024];
            byte[] RXbuf = new byte[1];
            FileInfo fi = new FileInfo(FirmFileTextBox.Text);
            UpgradeProgressBar.Maximum = (int)(fi.Length / 1024);
            string fileSize = fi.Length.ToString();
            string fileName = fi.Name;
            ByteConverter bc = new ByteConverter();
            byte[] fileSizeByte = System.Text.Encoding.Default.GetBytes(fileSize);
            byte[] fileNameByte = System.Text.Encoding.Default.GetBytes(fileName);
            TxStatus txStatus = TxStatus.START;
            FileStream fs = new FileStream(FirmFileTextBox.Text, FileMode.Open);
            byte[] pktHeader = new byte[3];
            byte[] pktfooter = new byte[2];

            while (txStatus != TxStatus.EXIT)
            {
                switch (txStatus)
                {
                    case TxStatus.START:
                        //int lenth = tcpclient.Client.Receive(RXbuf, 1, SocketFlags.None);
                        int lenth = UpgradePort.Read(RXbuf, 0, 1);
                        if (lenth == 1 && RXbuf[0] == C)
                        {
                            pktHeader[0] = SOH;
                            pktHeader[1] = serialNum;
                            pktHeader[2] = (byte)~serialNum;
                            //tcpclient.Client.Send(pktHeader);
                            UpgradePort.Write(pktHeader, 0, pktHeader.Length);
                            //serialNum;
                            Txlist.Clear();
                            Txlist.AddRange(fileNameByte);
                            Txlist.Add(0x00);
                            Txlist.AddRange(fileSizeByte);
                            for (; Txlist.Count < 128;)
                            {
                                Txlist.Add(0);
                            }
                            //tcpclient.Client.Send(Txlist.ToArray());
                            UpgradePort.Write(Txlist.ToArray(), 0, Txlist.ToArray().Length);
                            short CRCR = ComputeCRC(Txlist.ToArray());
                            pktfooter[0] = (byte)(CRCR >> 8);
                            pktfooter[1] = (byte)CRCR;
                            //tcpclient.Client.Send(pktfooter);
                            UpgradePort.Write(pktfooter, 0, pktfooter.Length);
                            while (RXbuf[0] == 'C')
                            {
                                //lenth = tcpclient.Client.Receive(RXbuf, 1, SocketFlags.None);
                                lenth = UpgradePort.Read(RXbuf, 0, 1);
                            }
                            if (lenth == 1 && RXbuf[0] == ACK)
                            {
                                //lenth = tcpclient.Client.Receive(RXbuf, 1, SocketFlags.None);
                                lenth = UpgradePort.Read(RXbuf, 0, 1);
                                if (lenth == 1 && RXbuf[0] == C)
                                {
                                    txStatus = TxStatus.TXING;
                                    serialNum++;
                                }

                            }

                        }

                        break;
                    case TxStatus.TXING:
                        for (int i = 0; i < 1024; i++)
                        {
                            TXbuf[i] = 0x1A;
                        }
                        int readlenth = fs.Read(TXbuf, 0, 1024);
                        if (readlenth > 0)
                        {
                            pktHeader[0] = STX;
                            pktHeader[1] = serialNum;
                            pktHeader[2] = (byte)~serialNum;
                            //tcpclient.Client.Send(pktHeader);
                            UpgradePort.Write(pktHeader, 0, pktHeader.Length);
                            serialNum++;
                            for (int i = 0; i < 1024; i++)
                            {
                                byte[] temp = new byte[1];
                                temp[0] = TXbuf[i];
                                //tcpclient.Client.Send(temp);
                                UpgradePort.Write(temp, 0, temp.Length);
                            }

                            short CRCR = ComputeCRC(TXbuf);
                            pktfooter[0] = (byte)(CRCR >> 8);
                            pktfooter[1] = (byte)CRCR;
                            //tcpclient.Client.Send(pktfooter);
                            UpgradePort.Write(pktfooter, 0, pktfooter.Length);
                            //lenth = tcpclient.Client.Receive(RXbuf, 1, SocketFlags.None);
                            lenth = UpgradePort.Read(RXbuf, 0, 1);
                            if (lenth == 1 && RXbuf[0] == ACK)
                            {
                                UpgradeProgressBar.Value = serialNum - 2;
                                label11.Text = (UpgradeProgressBar.Value.ToString() + "/" + UpgradeProgressBar.Maximum.ToString());

                            }
                            else
                            {
                                txStatus = TxStatus.EXIT;
                            }

                        }
                        else
                        {
                            txStatus = TxStatus.END;
                        }

                        break;
                    case TxStatus.END:
                        byte[] Eot = new byte[1];
                        Eot[0] = EOT;
                        //tcpclient.Client.Send(Eot);
                        UpgradePort.Write(Eot, 0, Eot.Length);
                        //lenth = tcpclient.Client.Receive(RXbuf, 1, SocketFlags.None);
                        lenth = UpgradePort.Read(RXbuf, 0, 1);
                        if (lenth == 1 && RXbuf[0] == ACK)
                        {
                            pktHeader[0] = SOH;
                            pktHeader[1] = 0x00;
                            pktHeader[2] = 0xff;
                            //tcpclient.Client.Send(pktHeader);
                            UpgradePort.Write(pktHeader, 0, pktHeader.Length);
                            byte[] laspkt = new byte[128];
                            for (int i = 0; i < 128; i++) laspkt[i] = 0;
                            //tcpclient.Client.Send(laspkt);
                            UpgradePort.Write(laspkt, 0, laspkt.Length);
                            short CRCR = ComputeCRC(laspkt);
                            pktfooter[0] = (byte)(CRCR >> 8);
                            pktfooter[1] = (byte)CRCR;
                            //tcpclient.Client.Send(pktfooter);
                            UpgradePort.Write(pktfooter, 0, pktfooter.Length);
                            //lenth = tcpclient.Client.Receive(RXbuf, 1, SocketFlags.None);
                            lenth = UpgradePort.Read(RXbuf, 0, 1);
                            if (lenth == 1 && RXbuf[0] == ACK)
                                txStatus = TxStatus.EXIT;
                            else
                                txStatus = TxStatus.EXIT;
                        }
                        else if (lenth == 1 && RXbuf[0] == NAK)
                        {
                            //tcpclient.Client.Send(Eot);
                            UpgradePort.Write(Eot, 0, Eot.Length);
                            //lenth = tcpclient.Client.Receive(RXbuf, 1, SocketFlags.None);
                            lenth = UpgradePort.Read(RXbuf, 0, 1);
                            if (lenth == 1 && RXbuf[0] == ACK)
                            {
                                pktHeader[0] = SOH;
                                pktHeader[1] = 0x00;
                                pktHeader[2] = 0xff;
                                //tcpclient.Client.Send(pktHeader);
                                UpgradePort.Write(pktHeader, 0, pktHeader.Length);
                                byte[] laspkt = new byte[128];
                                for (int i = 0; i < 128; i++) laspkt[i] = 0;
                                //tcpclient.Client.Send(laspkt);
                                UpgradePort.Write(laspkt, 0, laspkt.Length);
                                short CRCR = ComputeCRC(laspkt);
                                pktfooter[0] = (byte)(CRCR >> 8);
                                pktfooter[1] = (byte)CRCR;
                                //tcpclient.Client.Send(pktfooter);
                                UpgradePort.Write(pktfooter, 0, pktfooter.Length);
                                //lenth = tcpclient.Client.Receive(RXbuf, 1, SocketFlags.None);
                                lenth = UpgradePort.Read(RXbuf, 0, 1);
                                if (lenth == 1 && RXbuf[0] == ACK)
                                    txStatus = TxStatus.EXIT;
                                else
                                    txStatus = TxStatus.EXIT;
                            }
                            else txStatus = TxStatus.EXIT;
                        }
                        break;
                    default: break;
                }

            }
            fs.Close();
            MessageBox.Show("Update Successfully!");
            UpgradePort.Write("3");
            if (ConnectButton.Text == "Disconnect")
            {
                CalibrationGroupBox.Enabled = true;
                MeasurementGroupBox.Enabled = true;
                SnSetUpGroupBox.Enabled = true;
                userInfoGroupBox.Enabled = true;
                if (senortype == SenorType.HT)
                {
                    mATestGroupBox.Enabled = false;
                }
                else
                {
                    mATestGroupBox.Enabled = true;
                }
                UpgradeButton.Enabled = true;
            }
            ConnectionGroupBox.Enabled = true;
            UpgradePort.Close();
            if (isApp)
            {
                sp1.Open();
            }

            //progressBar1.Value = 0;
            //label2.Text = "0/0";
            //tcpclient.Close();
            //button1.Text = "Connect";
            //UpGrade_button.Enabled = false;
            //hiUpgradeButton.Enabled = false;
            //toolStripStatusLabel1.Text = "......";
            //EnableInterface();
            Tx.Abort();
            //Application.Restart();
        }
        //private void FirmFileTextBox_TextChanged_1(object sender, EventArgs e)
        //{

        //}

        private void SnSetButton_Click(object sender, EventArgs e)
        {
            string status;
            if (DialogResult.OK == MessageBox.Show("Make sure the sensor serial number =" + SnTextBox.Text, "Serial Number Set", MessageBoxButtons.OKCancel))
            {
                try
                {
                    //string snStringSet = SnTextBox.Text;
                    //uint[] snUint = new uint[SnTextBox.Text.Length];
                    //ushort[] data = new ushort[SnTextBox.Text.Length / 2];

                    //for (int i = 0, j = 0; i < SnTextBox.Text.Length / 2; i++)
                    //{
                    //    data[i] = (ushort)(snStringSet[j + 1] << 8 | snStringSet[j]);
                    //    j += 2;
                    //}
                    // master.WriteMultipleRegisters(byte.Parse(ModbusAddrTextBox.Text), 41015 - 1, data);
                    status = rwReg.parameterSeting(modbusAddr, 41015 - 1, 8, SnTextBox.Text, "char", sp1);
                    MessageBox.Show(status);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void PTSACailbButton_Click(object sender, EventArgs e)
        {
            //if (Single.Parse(SlopeCalibTextBox.Text) < 0 || Single.Parse(SlopeCalibTextBox.Text) > 200)
            //{
            //    MessageBox.Show("Input a reasonable number,please", "Error");
            //}
            if (DialogResult.OK == MessageBox.Show("Make sure the solution =" + SlopeCalibTextBox.Text + currentDevice.MeasureUnit, "Slope Calibration", MessageBoxButtons.OKCancel))
            {
                try
                {
                    //Single PTSAppb = Single.Parse(SlopeCalibTextBox.Text);
                    //byte[] temp_bytes = BitConverter.GetBytes(PTSAppb);
                    //UInt32 temp_int = 0;
                    //temp_int += (uint)temp_bytes[3] << 3 * 8;
                    //temp_int += (uint)temp_bytes[2] << 2 * 8;
                    //temp_int += (uint)temp_bytes[1] << 1 * 8;
                    //temp_int += (uint)temp_bytes[0] << 0 * 8;
                    //ushort[] data = { (ushort)temp_int, (ushort)(temp_int >> 16) };
                    if (currentDevice == pBleach)
                    {
                        //漂水的表示方式为a%，所以将a乘0.01倍，为真实值
                        Single bleachSolution = (Single.Parse(SlopeCalibTextBox.Text)) * ((Single)0.01);
                        rwReg.parameterSeting(modbusAddr, 44002 - 1, 2, bleachSolution, "float", sp1);
                    }
                    else
                    {
                        rwReg.parameterSeting(modbusAddr, 44002 - 1, 2, SlopeCalibTextBox.Text, "float", sp1);
                    }
                    Delay(1000);
                    string status = rwReg.parameterSeting(modbusAddr, 44004 - 1, 1, 5, "uint16", sp1);
                    Delay(1000);

                    MessageBox.Show(status);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SlopeCalibTextBox_TextChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (SlopeCalibTextBox.Text!="")
            //    {
            //        if (Single.Parse(SlopeCalibTextBox.Text) < 0 || Single.Parse(SlopeCalibTextBox.Text) > 200)
            //        {
            //            MessageBox.Show("校准标液的范围应在0~200ppb");
            //            SlopeCalibTextBox.Text = "100";
            //        }
            //    }

            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    SlopeCalibTextBox.Text = "100";
            //}
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "value")
            {
                MeasurementChart.Series["value"].Enabled = true;
                MeasurementChart.Series["mA"].Enabled = false;
                MeasurementChart.Series["S365"].Enabled = false;
                MeasurementChart.Series["t365"].Enabled = false;
                MeasurementChart.Series["s420"].Enabled = false;
                MeasurementChart.Series["t420"].Enabled = false;
            }
            else if (comboBox1.Text == "mA")
            {
                MeasurementChart.Series["value"].Enabled = false;
                MeasurementChart.Series["mA"].Enabled = true;
                MeasurementChart.Series["S365"].Enabled = false;
                MeasurementChart.Series["t365"].Enabled = false;
                MeasurementChart.Series["s420"].Enabled = false;
                MeasurementChart.Series["t420"].Enabled = false;
            }
            else if (comboBox1.Text == "S365")
            {
                MeasurementChart.Series["value"].Enabled = false;
                MeasurementChart.Series["mA"].Enabled = false;
                MeasurementChart.Series["S365"].Enabled = true;
                MeasurementChart.Series["t365"].Enabled = false;
                MeasurementChart.Series["s420"].Enabled = false;
                MeasurementChart.Series["t420"].Enabled = false;
            }
            else if (comboBox1.Text == "t365")
            {
                MeasurementChart.Series["value"].Enabled = false;
                MeasurementChart.Series["mA"].Enabled = false;
                MeasurementChart.Series["S365"].Enabled = false;
                MeasurementChart.Series["t365"].Enabled = true;
                MeasurementChart.Series["s420"].Enabled = false;
                MeasurementChart.Series["t420"].Enabled = false;
            }
            else if (comboBox1.Text == "s420")
            {
                MeasurementChart.Series["value"].Enabled = false;
                MeasurementChart.Series["mA"].Enabled = false;
                MeasurementChart.Series["S365"].Enabled = false;
                MeasurementChart.Series["t365"].Enabled = false;
                MeasurementChart.Series["s420"].Enabled = true;
                MeasurementChart.Series["t420"].Enabled = false;
            }
            else if (comboBox1.Text == "t420")
            {
                MeasurementChart.Series["value"].Enabled = false;
                MeasurementChart.Series["mA"].Enabled = false;
                MeasurementChart.Series["S365"].Enabled = false;
                MeasurementChart.Series["t365"].Enabled = false;
                MeasurementChart.Series["s420"].Enabled = false;
                MeasurementChart.Series["t420"].Enabled = true;
            }
        }

        //private void SetModbusAddrButton_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        master.WriteSingleRegister(byte.Parse(ModbusAddrTextBox.Text), 42001 - 1, byte.Parse(ModbusAddrSetTextBox.Text));
        //        MessageBox.Show("set ok");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message.ToString());
        //    }
        //}

        /*************************************************************************/


        uint isGetAllValueSuccess = 0;
        bool isdataView = false;
        TargetConfig ST510 = new TargetConfig();
        Size lastFormsize = new Size(975, 700);
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Advant.SelectedTab == tabPage1)
            {
                lastFormsize = this.Size;
                this.Size = new Size(975, 700);
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                this.MaximizeBox = false;
            }
            else
            {
                this.Size = lastFormsize;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                //this.MaximizeBox = true;
            }
            if ((Advant.SelectedTab == tabPage2) && (isConnect == true) && (isdataView == false))
            {
                MeasurementTimer.Enabled = false;
                ReadButton.Text = "Read";
                ConnectionGroupBox.Enabled = true;
                CalibrationGroupBox.Enabled = true;
                FirmUpgradeGroupBox.Enabled = true;
                SnSetUpGroupBox.Enabled = true;
                //RefreshButton.Enabled = false;

                Thread.Sleep(50);
                //            ST510.COM_485_Config("COM4", 9600, Parity.None);////////////////////////////////////////////
                Thread getreg = new Thread(new ThreadStart(getprobeRegAndVaule));
                getreg.IsBackground = true;
                getreg.Start();
                isdataView = true;
            }

        }

        bool isimportantDoing = false;
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            while (isimportantDoing) ;
            isimportantDoing = true;

            Thread.Sleep(50);
            // ST510.COM_485_Config("COM4", 9600, Parity.None);
            getprobeRegAndVaule();
        }

        private void SaveParamButton_Click(object sender, EventArgs e)
        {
            probeParamPath = SnTextBox.Text + "_probeParam.csv";
            if (File.Exists(probeParamPath))
            {
                File.Delete(probeParamPath); //因为无法覆盖上一次的参数，所以先删除上一次保存的文件
            }
            if (!File.Exists(probeParamPath))
            {
                //File.Create(SnTextBox.Text + ".csv");
                File.WriteAllText(probeParamPath, "Name,Addr,Size,Type,Value\r\n");
            }
            exportProbeRegAndValue();
        }

        ArrayList probeRegList = new ArrayList();

        bool isbing = false;
        private void getprobeRegAndVaule()
        {
            Thread.Sleep(500);

            probeRegMap.DataSource = null;
            probeRegList.Clear();

            getParameter get = new getParameter();
            switch (senortype)
            {
                case SenorType.ST510:
                    {
                        probeRegList = get.getprobeRegister("paramST510.json");   //arrayList
                        break;
                    }
                case SenorType.Cyclo:
                    {
                        probeRegList = get.getprobeRegister("paramCyclo.json");
                        break;
                    }
                case SenorType.TBleach:
                    {
                        probeRegList = get.getprobeRegister("paramTBleach.json");
                        break;
                    }
                case SenorType.TClO2:
                    {
                        probeRegList = get.getprobeRegister("paramTClO2.json");
                        break;
                    }
                case SenorType.TOIW:
                    {
                        probeRegList = get.getprobeRegister("paramTOIW.json");
                        break;
                    }
                case SenorType.ST540SS:
                    {
                        probeRegList = get.getprobeRegister("paramST540NDSA.json");
                        break;
                    }
                case SenorType.HT:
                    {
                        probeRegList = get.getprobeRegister("paramHT.json");
                        break;
                    }
                default:
                    {
                        probeRegList = get.getprobeRegister("paramST510.json");   //arrayList
                        break;
                    }
            }

            probeRegister temp = new probeRegister();  //temp= {name,addr,size,type,value}
            progressBar1.Maximum = probeRegList.Count;
            progressBar1.Value = 0;
            for (int j = 0; j < probeRegList.Count; j++, progressBar1.Value++)
            {
                float persent = (float)(progressBar1.Value + 1) / (float)progressBar1.Maximum;
                progressLable.Text = ((uint)(persent * 100)).ToString() + "%";
                temp = (probeRegister)probeRegList[j];

                string[] str = rwReg.parameterGettingContinuous(byte.Parse(ModbusAddrTextBox.Text), (ushort)(temp.Addr - 1), (byte)temp.Size, temp.Type, sp1);
                //Delay(500);/////////////////////test
                if (str[0] == "Success") { temp.Value = str[1]; }
                else { MessageBox.Show(str[1]); break; }

                probeRegList[j] = temp;
            }

            sp1.Close();

            probeRegMap.DataSource = probeRegList;
            BindingSource bindsource = new BindingSource();
            int z = probeRegMap.ColumnCount;
            for (int i = 0; i < 4; i++)
            {
                probeRegMap.Columns[i].ReadOnly = true;
                probeRegMap.Columns[i].DefaultCellStyle.ForeColor = System.Drawing.Color.Blue;
            }
            isimportantDoing = false;
            //doingStatus_l.Text = "";
            //RefreshButton.Enabled = true;
            panel3.Enabled = true;
            if (!isbing)
            {
                isbing = true;
                Thread.CurrentThread.Abort();
            }
        }
        private void setProbeRegAndValue()
        {
            string result = "Success";
            ArrayList regchange = new ArrayList();
            ArrayList postion = new ArrayList();
            probeRegister temp = new probeRegister();
            foreach (DictionaryEntry de in regChangeHash)
            {
                temp = (probeRegister)probeRegList[(int)de.Key];
                temp.Value = de.Value.ToString();
                regchange.Add(temp); postion.Add((int)de.Key);
            }
            progressBar1.Maximum = regchange.Count; progressBar1.Value = 0;
            for (int i = 0; i < regchange.Count && result == "Success"; i++, progressBar1.Value++)
            {
                float persent = (float)(progressBar1.Value + 1) / (float)progressBar1.Maximum;
                progressLable.Text = ((uint)(persent * 100)).ToString() + "%";
                temp = (probeRegister)regchange[i];
                //if (temp.Type == "uint16" || temp.Type == "uint32")
                //{
                //    try
                //    {
                //        result = rwReg.parameterSeting(modbusAddr, (ushort)(temp.Addr - 1), (byte)temp.Size, temp.Value, temp.Type, sp1);
                //        if (result == "Success") { probeRegMap[4, (int)postion[i]].Style.ForeColor = System.Drawing.Color.Black; }
                //        else
                //        {
                //            MessageBox.Show(result); break;
                //        }
                //    }
                //    catch (FormatException)
                //    {
                //        MessageBox.Show("parameter error"); break;
                //    }

                //}
                //else if (temp.Type == "float")
                //{
                //    try
                //    {
                //        result = rwReg.parameterSeting(modbusAddr, (ushort)(temp.Addr - 1), (byte)temp.Size, Convert.ToSingle(temp.Value), temp.Type, sp1);
                //        if (result == "Success") { probeRegMap[4, (int)postion[i]].Style.ForeColor = System.Drawing.Color.Black; }
                //        else
                //        {
                //            MessageBox.Show(result); break;
                //        }
                //    }
                //    catch (FormatException)
                //    {
                //        MessageBox.Show("parameter error"); break;
                //    }
                //}
                try
                {
                    result = rwReg.parameterSeting(modbusAddr, (ushort)(temp.Addr - 1), (byte)temp.Size, temp.Value, temp.Type, sp1);
                    //status = rwReg.parameterSeting(modbusAddr, 45001 - 1, 16, userEmailTextBox.Text, "char", sp1);
                    if (result == "Success")
                    {
                        probeRegMap[4, (int)postion[i]].Style.ForeColor = System.Drawing.Color.Black;
                        //MessageBox.Show(result);
                    }
                    else
                    {
                        MessageBox.Show(result); break;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("parameter error"); break;
                }
            }

            ToolStripStatusLabel.Text = result;
            isimportantDoing = false;
            Thread.Sleep(50);
            ToolStripStatusLabel.Text = "...";
            Thread.CurrentThread.Abort();
        }
        public string exportProbeRegAndValue()
        {
            int j;
            string result = "Success";
            ArrayList probeRegL = new ArrayList();
            getParameter get = new getParameter();
            probeRegL = get.getprobeRegister("probeParam.json");   //arrayList
            probeRegister temp = new probeRegister();  //temp= {name,addr,size,type,value}
            for (j = 0; j < probeRegL.Count; j++)
            {
                temp = (probeRegister)probeRegL[j];

                string[] str = rwReg.parameterGettingContinuous(byte.Parse(ModbusAddrTextBox.Text), (ushort)(temp.Addr - 1), (byte)temp.Size, temp.Type, sp1);
                if (str[0] == "Success")
                {
                    temp.Value = str[1];
                    File.AppendAllText(probeParamPath, temp.Name + "," + temp.Addr.ToString() + "," + temp.Size.ToString() + "," + temp.Type + "," + temp.Value + "\r\n");
                }
                else
                {
                    result = str[1];
                    MessageBox.Show(str[1]); break;
                }
            }
            if (j == probeRegL.Count)
            {
                MessageBox.Show("success");
            }
            sp1.Close();
            return result;
        }
        Hashtable regChangeHash = new Hashtable();
        private void probeRegMap_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            probeRegMap[e.ColumnIndex, e.RowIndex].Style.ForeColor = System.Drawing.Color.Red;
            probeRegisterChange regchge = new probeRegisterChange();
            if (regChangeHash.Contains(e.RowIndex) == true)
            {
                regChangeHash[e.RowIndex] = probeRegMap[e.ColumnIndex, e.RowIndex].Value.ToString();
            }
            else
            {
                regChangeHash.Add(e.RowIndex, probeRegMap[e.ColumnIndex, e.RowIndex].Value.ToString());
            }
        }

        private void writeProbeButton_Click(object sender, EventArgs e)
        {
            while (isimportantDoing) ;
            isimportantDoing = true;
            // ST510.COM_485_Config(PortcomboBox.Text, byte.Parse(BaudComboBox.Text), sp1.Parity);
            Thread setreg = new Thread(new ThreadStart(setProbeRegAndValue));
            setreg.Start();
        }

        private void ST510ProbeConfigurator_SizeChanged(object sender, EventArgs e)
        {
            if (Advant.SelectedTab != tabPage1) lastFormsize = this.Size;
        }

        private void ParityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //private void mARangeTextBox_TextChanged(object sender, EventArgs e)
        //{

        //}

        private void mARange_Click(object sender, EventArgs e)
        {
            mARange mArange = new mARange(currentDevice.MeasureUnit, currentDevice.CalibMaxValue);

            if (mArange.ShowDialog() == DialogResult.OK)
            {
                UInt16 data;
                Single dataFloat;
                string result = "Success";
                if (senortype == SenorType.TBleach)
                {
                    dataFloat = mArange.mAppbFloat * 0.01F;  //漂水浓度用a%显示，故应将客户输入的a乘0.01倍送入float寄存器
                    result = rwReg.parameterSeting(modbusAddr, 48035 - 1, 2, dataFloat, "float", sp1);
                }
                else
                {
                    data = mArange.mAppb;
                    result = rwReg.parameterSeting(modbusAddr, 48010 - 1, 1, data, "uint16", sp1);
                }
                if (result == "Success")
                {
                    MessageBox.Show("The conversion factor set up success");
                }
                else
                {
                    MessageBox.Show(result);
                }
            }
        }

        private void saveFactoryParamButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("Do you want to save factory parameters to Flash ?", "Factory Parameters", MessageBoxButtons.OKCancel))
            {
                try
                {
                    string status = rwReg.parameterSeting(modbusAddr, 44004 - 1, 1, 6, "uint16", sp1);
                    Delay(1000);

                    MessageBox.Show(status);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void bleButton_Click(object sender, EventArgs e)
        {
            try
            {
                string[] data;
                UInt16 adverStatus;
                if (bleButton.Text == "Start Adver")
                {
                    rwReg.parameterSeting(modbusAddr, 41042 - 1, 1, 1, "uint16", sp1);
                    Delay(1000);
                    data = rwReg.parameterGettingContinuous(modbusAddr, 41043 - 1, 1, "uint16", sp1);

                    adverStatus = Convert.ToUInt16(data[1]);
                    if (adverStatus == 1)
                    {
                        MessageBox.Show("The Bluetooth Starts Advertising Successfully");
                        bleButton.Text = "Stop Adver";
                        strBtleAdverStatus = "Advertising...";
                    }
                    else
                    {
                        MessageBox.Show("Try Again,Please");
                    }
                }
                else if (bleButton.Text == "Stop Adver")
                {
                    rwReg.parameterSeting(modbusAddr, 41042 - 1, 1, 0, "uint16", sp1);
                    Delay(1000);
                    data = rwReg.parameterGettingContinuous(modbusAddr, 41043 - 1, 1, "uint16", sp1);

                    adverStatus = Convert.ToUInt16(data[1]);
                    if (adverStatus == 0)
                    {
                        MessageBox.Show("The Bluetooth Stops Advertising Successfully");
                        bleButton.Text = "Start Adver";
                        strBtleAdverStatus = "Advertisement is stoped.";
                    }
                    else
                    {
                        MessageBox.Show("Try Again,Please");
                    }
                }
                ToolStripStatusLabel.Text = "connected " + "Senor Type: " + senortype.ToString() + "    swVer: " + swString + strBtleAdverStatus;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void mATestbutton_Click(object sender, EventArgs e)
        {
            if (ReadButton.Text == "StopReading")
            {
                MeasurementTimer.Enabled = false;
                ReadButton.Text = "Read";
                ConnectionGroupBox.Enabled = true;
                CalibrationGroupBox.Enabled = true;
                FirmUpgradeGroupBox.Enabled = true;
                SnSetUpGroupBox.Enabled = true;
                panel3.Enabled = true;
            }

            if ((mATestTextBox.Text != "") && (Single.Parse(mATestTextBox.Text) > 3.999) && (Single.Parse(mATestTextBox.Text) < 20.001))
            {
                string status;
                try
                {
                    status = rwReg.parameterSeting(modbusAddr, 43015 - 1, 2, mATestTextBox.Text, "float", sp1);
                    Delay(500);
                    if (status == "Success")
                    {
                        status = rwReg.parameterSeting(modbusAddr, 44004 - 1, 1, 7, "uint16", sp1);
                        Delay(500);
                        if (status == "Success")
                        {
                            probeMode = runMode.mATestMode;
                            MeasurementTimer.Enabled = true;
                            MessageBox.Show("The 4-20mA test starts, you can stop it immediately by clicking the 'Stop' button or it will stop after two minutes.");
                            ReadButton.Enabled = false;
                        }
                        else
                        {
                            MessageBox.Show(status);
                        }
                    }
                    else
                    {
                        MessageBox.Show(status);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                MessageBox.Show("Input a value between 4.0 mA and 20.0 mA, please.");
            }
        }

        private void stopmATestbutton_Click(object sender, EventArgs e)
        {
            string status;
            try
            {
                status = rwReg.parameterSeting(modbusAddr, 44004 - 1, 1, 8, "uint16", sp1);
                Delay(500);
                if (status == "Success")
                {
                    MeasurementTimer.Enabled = false;

                    MessageBox.Show("The 4-20mA test stops successfully");

                    probeMode = runMode.measureMode;
                    mATestCount = 0;
                    ReadButton.Enabled = true;
                }
                else
                {
                    MessageBox.Show(status);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void userEmailButton_Click(object sender, EventArgs e)
        {
            string status;
            if (DialogResult.OK == MessageBox.Show("Make sure the email address is " + userEmailTextBox.Text, "Email Address Set", MessageBoxButtons.OKCancel))
            {
                try
                {
                    //string snStringSet = SnTextBox.Text;
                    //uint[] snUint = new uint[SnTextBox.Text.Length];
                    //ushort[] data = new ushort[SnTextBox.Text.Length / 2];

                    //for (int i = 0, j = 0; i < SnTextBox.Text.Length / 2; i++)
                    //{
                    //    data[i] = (ushort)(snStringSet[j + 1] << 8 | snStringSet[j]);
                    //    j += 2;
                    //}
                    // master.WriteMultipleRegisters(byte.Parse(ModbusAddrTextBox.Text), 41015 - 1, data);
                    status = rwReg.parameterSeting(modbusAddr, 45001 - 1, 16, userEmailTextBox.Text, "char", sp1);
                    MessageBox.Show(status);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void setDeviceUnit()
        {
            typeLabel.Text = currentDevice.DeviceName;
            ptsaValueLable.Text = "---" + currentDevice.MeasureUnit;
            SlopeCalibTextBox.Text = " ";
            label6.Text = currentDevice.MeasureUnit;
        }
        private void chloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            senortype = SenorType.Chlo;
            currentDevice = pChlo;
            setDeviceUnit();
        }
        private void sT510CycloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            senortype = SenorType.Cyclo;
            currentDevice = pCyclo;
            setDeviceUnit();
        }

        private void sT510TBleachToolStripMenuItem_Click(object sender, EventArgs e)
        {
            senortype = SenorType.TBleach;
            currentDevice = pBleach;
            setDeviceUnit();
        }

        private void sT510ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            senortype = SenorType.ST510;
            currentDevice = pST510;
            setDeviceUnit();
        }

        private void sT510TClO2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            senortype = SenorType.TClO2;
            currentDevice = pClO2;
            setDeviceUnit();
        }

        private void sT510TOIWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            senortype = SenorType.TOIW;
            currentDevice = pOIW;
            setDeviceUnit();
        }

        private void sT540NDSAToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            senortype = SenorType.ST540SS;
            currentDevice = pNDSA;
            setDeviceUnit();
        }

        private void setStandardModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string status;
            try
            {
                status = rwReg.parameterSeting(modbusAddr, 42003 - 1, 1, 2, "uint16", sp1);//parity even
                if (status == "Success")
                {
                    status = rwReg.parameterSeting(modbusAddr, 42004 - 1, 2, 9600, "uint32", sp1);//baudrate 9600
                    if (status == "Success")
                    {
                        if (senortype == SenorType.TBleach)
                        {
                            status = rwReg.parameterSeting(modbusAddr, 42001 - 1, 1, 32, "uint16", sp1); //modbusaddress,bleach,32
                        }
                        else if (senortype == SenorType.TClO2)
                        {
                            status = rwReg.parameterSeting(modbusAddr, 42001 - 1, 1, 35, "uint16", sp1); //modbusaddress,ClO2,35
                        }
                        else
                        {
                            status = rwReg.parameterSeting(modbusAddr, 42001 - 1, 1, 10, "uint16", sp1); //modbusaddress
                        }
                        MessageBox.Show(status);
                    }
                    else
                    {
                        MessageBox.Show("Failed to set baudrate " + status);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to set parity " + status);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void setCustomizedModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string status;
            try
            {
                status = rwReg.parameterSeting(modbusAddr, 42003 - 1, 1, 1, "uint16", sp1);//parity ODD
                if (status == "Success")
                {
                    status = rwReg.parameterSeting(modbusAddr, 42004 - 1, 2, 19200, "uint32", sp1);//baudrate 19200
                    if (status == "Success")
                    {
                        status = rwReg.parameterSeting(modbusAddr, 42001 - 1, 1, 1, "uint16", sp1); //modbusaddress,1
                        MessageBox.Show(status);
                    }
                    else
                    {
                        MessageBox.Show("Failed to set baudrate " + status);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to set parity " + status);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void HTtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            senortype = SenorType.HT;
            currentDevice = pHT;
            setDeviceUnit();
        }
        private void SPA505LtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            senortype = SenorType.SPA505L;
            currentDevice = pSPA505L;
            setDeviceUnit();
        }

        private void ST500DICailbButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("Make sure the solution  TIG = 0 ppm", "DI Calibration", MessageBoxButtons.OKCancel))
            {
                string status;
                try
                {
                    //master.WriteSingleRegister(byte.Parse(ModbusAddrTextBox.Text), 44004 - 1, 4);
                    //Delay(1000);
                    status = rwReg.parameterSeting(modbusAddr, 44004 - 1, 1, 10, "uint16", sp1);
                    if (status == "Success")
                    {
                        MessageBox.Show("DI Calibration is successful, Calib Slope please...");
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ST500CailbButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("Make sure the solution =" + ST500SlopeCalibTextBox.Text + currentDevice.MeasureUnit, "Slope Calibration", MessageBoxButtons.OKCancel))
            {
                try
                {
                    //Single PTSAppb = Single.Parse(SlopeCalibTextBox.Text);
                    //byte[] temp_bytes = BitConverter.GetBytes(PTSAppb);
                    //UInt32 temp_int = 0;
                    //temp_int += (uint)temp_bytes[3] << 3 * 8;
                    //temp_int += (uint)temp_bytes[2] << 2 * 8;
                    //temp_int += (uint)temp_bytes[1] << 1 * 8;
                    //temp_int += (uint)temp_bytes[0] << 0 * 8;
                    //ushort[] data = { (ushort)temp_int, (ushort)(temp_int >> 16) };
                    rwReg.parameterSeting(modbusAddr, 44009 - 1, 1, ST500SlopeCalibTextBox.Text, "uint16", sp1);
                    Delay(1000);
                    string status = rwReg.parameterSeting(modbusAddr, 44004 - 1, 1, 11, "uint16", sp1);
                    Delay(1000);

                    MessageBox.Show(status);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        class PyxisDevice
        {
            public string DeviceName;
            public string MeasureName;
            public string MeasureUnit;
            public double MeasureMaxValue;
            public double MeasureMinValue;
            public double CalibMaxValue;
            public double CalibMinValue;

            public PyxisDevice(string deviceName, string measureName, string measureUnit, double measureMaxValue, double measureMinValue, double calibMaxValue, double calibMinValue)
            {
                DeviceName = deviceName;
                MeasureName = measureUnit;
                MeasureUnit = measureUnit;
                MeasureMaxValue = measureMaxValue;
                MeasureMinValue = measureMinValue;
                CalibMaxValue = calibMaxValue;
                CalibMinValue = calibMinValue;
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void SPA505LGainButton_Click(object sender, EventArgs e)
        {
            try
            {
                rwReg.parameterSeting(modbusAddr, 48003 - 1, 1, SPA505LGainTextBox.Text, "uint16", sp1);
                Delay(500);
                string status = rwReg.parameterSeting(modbusAddr, 44001 - 1, 1, 1, "uint16", sp1);
                Delay(500);

                MessageBox.Show(status);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SPA505LCt365Button_Click(object sender, EventArgs e)
        {
            try
            {
                rwReg.parameterSeting(modbusAddr, 48002 - 1, 1, SPA505LCt365TextBox.Text, "uint16", sp1);
                Delay(500);
                string status = rwReg.parameterSeting(modbusAddr, 44001 - 1, 1, 1, "uint16", sp1);
                Delay(500);

                MessageBox.Show(status);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

