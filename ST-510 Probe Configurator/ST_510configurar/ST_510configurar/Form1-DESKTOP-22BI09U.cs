
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

namespace ST_510configurar
{
    public partial class ST510ProbeConfigurator : Form
    {
        SerialPort sp1 = new SerialPort();
        bool isApp = false;
        public ST510ProbeConfigurator()
        {
            InitializeComponent();
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
            ParityComboBox.Items.Add("1EVEN");

            //检查是否含有串口
            string[] serialPortName = SerialPort.GetPortNames();

            if (serialPortName == null)
            {
                MessageBox.Show("本机没有串口！", "Error");
                return;
            }

            //添加串口项目
            foreach (string s in System.IO.Ports.SerialPort.GetPortNames())
            {//获取有多少个COM口
                //System.Diagnostics.Debug.WriteLine(s);
                PortcomboBox.Items.Add(s);
            }
            //sp1.BaudRate = 9600;

            Control.CheckForIllegalCrossThreadCalls = false;    //这个类中我们不检查跨线程的调用是否合法(因为.net 2.0以后加强了安全机制,，不允许在winform中直接跨线程访问控件的属性)


            ////准备就绪              
            //sp1.DtrEnable = true;
            //sp1.RtsEnable = true;
            ////设置数据读取超时为1秒
            //sp1.ReadTimeout = 1000;

            sp1.Close();
            PortcomboBox.Text = serialPortName[0];
            ParityComboBox.Text = ParityComboBox.Items[0].ToString();
            BaudComboBox.Text = BaudComboBox.Items[5].ToString();
            ReadCycleComboBox.Text = ReadCycleComboBox.Items[1].ToString();
            //表格属性 
            MeasurementChart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            MeasurementChart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            MeasurementChart.ChartAreas[0].AxisY.Title = "PTSA(PPb)";
            MeasurementChart.ChartAreas[0].AxisY2.Title = "HR(%)";

            MeasurementChart.ChartAreas[0].Area3DStyle.Enable3D = false;
            MeasurementChart.Series.Add("PTSA(PPb)");
            MeasurementChart.Series["PTSA(PPb)"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            MeasurementChart.Series["PTSA(PPb)"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            MeasurementChart.Series["PTSA(PPb)"].IsValueShownAsLabel = false;

            MeasurementChart.Series["PTSA(PPb)"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            MeasurementChart.Series["PTSA(PPb)"].Color = Color.Green;
            MeasurementChart.Series["PTSA(PPb)"].BorderWidth = 1;
            MeasurementChart.Legends.Add("PTSA(PPb)");

            MeasurementChart.Series.Add("HR(%)");
            MeasurementChart.Series["HR(%)"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            MeasurementChart.Series["HR(%)"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            MeasurementChart.Series["HR(%)"].IsValueShownAsLabel = false;

            MeasurementChart.Series["HR(%)"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            MeasurementChart.Series["HR(%)"].Color = Color.Blue;
            MeasurementChart.Series["HR(%)"].BorderWidth = 1;
            MeasurementChart.Legends.Add("HR(%)");

            MeasurementChart.Series.Add("Temp(^C)");
          //  MeasurementChart.Series["Temp(^C)"].YAxisType = AxisType.Secondary;
            MeasurementChart.Series["Temp(^C)"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            MeasurementChart.Series["Temp(^C)"].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Time;
            MeasurementChart.Series["Temp(^C)"].IsValueShownAsLabel = false;

            MeasurementChart.Series["Temp(^C)"].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            MeasurementChart.Series["Temp(^C)"].Color = Color.Red;
            MeasurementChart.Series["Temp(^C)"].BorderWidth = 1;
            MeasurementChart.Legends.Add("Temp(^C)");

            //MeasurementChart.Legends["PTSA(PPb)"].DockedToChartArea = "mainChartArea";
            //MeasurementChart.Legends["Temp(^C)"].DockedToChartArea = "mainChartArea";
            //MeasurementChart.Legends["Conductivity"].DockedToChartArea = "mainChartArea";

            MeasurementChart.Legends["PTSA(PPb)"].IsDockedInsideChartArea = true;
            MeasurementChart.Legends["Temp(^C)"].IsDockedInsideChartArea = true;
            MeasurementChart.Legends["HR(%)"].IsDockedInsideChartArea = true;


        }
        IModbusSerialMaster master;
        enum SenorType
        {
            ST_510,
            Unknow

        }
        SenorType senortype = SenorType.Unknow;

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void ConnectionButton_Click(object sender, EventArgs e)
        {
            if (ConnectButton.Text == "Connect")
            {
                sp1.PortName = PortcomboBox.Text;
                sp1.BaudRate = int.Parse(BaudComboBox.Text);
                if (ParityComboBox.Text == "None")
                {
                    sp1.Parity = Parity.None;
                }
                else if (ParityComboBox.Text == "Odd")
                {
                    sp1.Parity = Parity.Odd;
                }
                else if (ParityComboBox.Text == "Even")
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
                        sp1.ReadTimeout = 5000;
                        Thread.Sleep(500);
                        int length = sp1.Read(res, 0, 7);
                        sp1.Close();
                        if (length != 0)
                        {
                            ModbusAddrTextBox.Text = res[0].ToString();
                        }
                        else
                        {
                            ToolStripStatusLabel.Text = "find error0";
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        ToolStripStatusLabel.Text = "find error1";
                        sp1.Close();
                    }

                    //get modbus addr
                    sp1.Open();
                    master = ModbusSerialMaster.CreateRtu(sp1);
                    master.Transport.ReadTimeout = 2000;
                    master.Transport.WriteTimeout = 2000;
                    try
                    {
                        ushort[] productNum = master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 41005 - 1, 2);
                        UInt32 pn = productNum[1];
                        pn <<= 16;
                        pn += productNum[0];
                        switch (pn)
                        {
                            // case 0: break;
                            case 53501:
                                {
                                    senortype = SenorType.ST_510;
                                    MeasurementChart.ChartAreas[0].AxisY.Title = "PTSA(PPb)";
                                    //Font nettype = ValueLabel1.Font;
                                    //nettype.Size

                                    //ValueLabel1.Font.Size = 12;
                                    break;
                                }
                            default: break;
                        }
                        ushort[] serialnum = master.ReadHoldingRegisters(byte.Parse(ModbusAddrTextBox.Text), 41015 - 1, 8);
                        char[] snChar = new char[serialnum.Length * 2];
                        for (int i = 0, j = 0; i < serialnum.Length; i++)
                        {
                            snChar[j] = (char)(serialnum[i] & 0x00FF); j++;
                            snChar[j] = (char)(serialnum[i] >> 8); j++;
                        }
                        string snString = new string(snChar);

                        ModbusAddrSetTextBox.Text = snString;
                        ToolStripStatusLabel.Text = snString + "connected " + "Senor Type:" + senortype.ToString();
                        ConnectButton.Text = "Disconnect";
                        MeasurementGroupBox.Enabled = true;
                        CalibrationGroupBox.Enabled = true;
                        FirmUpgradeGroupBox.Enabled = true;
                        SnSetUpGroupBox.Enabled = true;
                        isApp = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
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
                MeasurementTimer.Enabled = false;
                sp1.Close();
                ConnectButton.Text = "Connect";
                ToolStripStatusLabel.Text = "No Connection";
                MeasurementGroupBox.Enabled = false;
                CalibrationGroupBox.Enabled = false;
                FirmUpgradeGroupBox.Enabled = false;
                SnSetUpGroupBox.Enabled = false;
            }
          }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            if (ReadButton.Text == "Read")
            {
                try
                {
                    if (!File.Exists(SnTextBox.Text + ".csv"))
                    {
                        //File.Create(SnTextBox.Text + ".csv");
                        File.WriteAllText(SnTextBox.Text + ".csv", "date,PTSA,HM,Temp\r\n");
                    }
                    MeasurementTimer.Interval = int.Parse(ReadCycleComboBox.Text) * 1000;
                    MeasurementTimer.Enabled = true;
                    ReadButton.Text = "StopReading";
                    ConnectionGroupBox.Enabled = false;
                    CalibrationGroupBox.Enabled = false;
                    FirmUpgradeGroupBox.Enabled = false;
                    SnSetUpGroupBox.Enabled = false;
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
            }
        }
        private void MeasurementTimer_Tick(object sender, EventArgs e)
        {
            try
            {


                Single PTSA, HM, temp;
                ushort[] data = master.ReadInputRegisters(byte.Parse(ModbusAddrTextBox.Text), 46001 - 1, 8);

                UInt32 tData= ((UInt32)data[1] << 16) + data[0];
                byte[] temp_bytes = BitConverter.GetBytes(tData);
                PTSA = BitConverter.ToSingle(temp_bytes, 0);

                tData = ((UInt32)data[5] << 16) + data[4];
                temp_bytes = BitConverter.GetBytes(tData);
                temp = BitConverter.ToSingle(temp_bytes, 0);

                tData = ((UInt32)data[7] << 16) + data[6];
                temp_bytes = BitConverter.GetBytes(tData);
                HM = BitConverter.ToSingle(temp_bytes, 0);

                DateTime CrtTime = new DateTime();
                CrtTime = DateTime.Now;

                if (senortype == SenorType.ST_510 || senortype == SenorType.Unknow)
                {
                    ValueLabel1.Text = "PTSA: " + PTSA.ToString() + "ppb";
                    ValueLabel2.Text = "HM: " + HM.ToString() + "%";
                    ValueLabel3.Text = "temp: " + temp.ToString() + " ";
                }
                

               File.AppendAllText(SnTextBox.Text + ".csv", CrtTime.ToString() + "," + PTSA.ToString("F2") + "," + HM.ToString("F1") + "," + temp.ToString("F1") + "\r\n");

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }
    }
    }

