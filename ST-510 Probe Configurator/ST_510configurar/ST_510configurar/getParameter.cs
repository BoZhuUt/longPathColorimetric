using System;
using System.IO;
using System.IO.Ports;
using Newtonsoft.Json;
using System.Collections;
using Modbus.Device;
using System.Windows.Forms;

namespace ST_510configurar
{
	class probeRegister
	{
		public probeRegister(string regName,uint regAddr,uint regSize,string regType)
		{
		 name = regName;
		 addr = regAddr;
		 size = regSize;
		 type = regType;
		}
        public probeRegister()
        {

        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private uint addr;

        public uint Addr
        {
            get { return addr; }
            set { addr = value; }
        }
        private uint size;

        public uint Size
        {
            get { return size; }
            set { size = value; }
        }
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        private string value = "0";

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
    class probeRegisterChange
    {
        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        private string value;

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
    class getParameter
    {
        public string Id;
        public string Date;
        public string Major;
        public string Minor;
        ArrayList RegisterList = new ArrayList();
        public ArrayList getprobeRegister(string fileAddr)
        {
            string jsonText = File.ReadAllText(fileAddr);
            JsonReader reader = new JsonTextReader(new StringReader(jsonText));
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.Value.ToString().Equals("id"))
                    {
                        reader.Read();
                        Id = reader.Value.ToString();
                    }
                    if (reader.Value.Equals("date"))
                    {
                        reader.Read();
                        Date = reader.Value.ToString();
                    }
                    if (reader.Value.Equals("major"))
                    {
                        reader.Read();
                        Major = reader.Value.ToString();
                    }
                    if (reader.Value.Equals("minor"))
                    {
                        reader.Read();
                        Minor = reader.Value.ToString();
                    }
                    if (reader.Value.Equals("name"))
                    {
                        probeRegister pr = new probeRegister();
                        reader.Read();
                        pr.Name = reader.Value.ToString();
                        reader.Read();
                        reader.Read();
                        pr.Addr = Convert.ToUInt32(reader.Value);
                        reader.Read();
                        reader.Read();
                        pr.Size = Convert.ToUInt32(reader.Value);
                        reader.Read();
                        reader.Read();
                        pr.Type = reader.Value.ToString();
                        RegisterList.Add(pr);
                    }
                }
            }
            return RegisterList;
        }
    }
    class TargetConfig
    {
        SerialPort COM_485 = new SerialPort();
        private string PortName_;
        private int BaudRate_;
        private Parity parity_;

        public byte Sequence_Number = 0;
        public void COM_485_Config(string PortName, int BaudRate, Parity parity)
        {
            //COM_485.DataReceived += new SerialDataReceivedEventHandler(COM_485_DataReceived);
            // recivetimer.Elapsed += new System.Timers.ElapsedEventHandler(recivetimer_Elapsed);
            COM_485.ReadTimeout = 1000;
            PortName_ = PortName;
            BaudRate_ = BaudRate;
            parity_ = parity;
            COM_485.ReceivedBytesThreshold = 1;
        }
    }
    //class GetResult
    //{
    //    public GetResult(string comm_status,ushort[] probe_data)
    //    {
    //        status = comm_status;
    //        data = probe_data;
    //    }
    //    public GetResult()
    //    {

    //    }
    //    private string status;

    //    public string Status
    //    {
    //        get { return status; }
    //        set { status = value; }
    //    }
    //    private ushort[] data;

    //    public ushort[] Data
    //    {
    //        get { return data; }
    //        set { data = value; }
    //    }
    //}
    class OperateReg
    {
        //public GetResult parameterGetting(byte addr, UInt16 reg_addr, byte reg_lenth, SerialPort port)
        //{
        //    GetResult Result = new GetResult();
        //    Result.Status = "Success";
        //    IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
        //    master.Transport.ReadTimeout = 1000;
        //    master.Transport.WriteTimeout = 1000;

        //    if (!port.IsOpen) port.Open();
           
        //    try
        //    {
        //        Result.Data = master.ReadHoldingRegisters(addr, reg_addr, reg_lenth);
        //    }
        //    catch (InvalidOperationException)
        //    {
        //        Result.Status = "Port not Found";
        //    }
        //    catch (TimeoutException)
        //    {
        //        Result.Status = "Time out";
        //    }
        //    catch (Modbus.SlaveException ex)
        //    {
        //        Result.Status = "unknown exception :" + ex.SlaveExceptionCode.ToString();
        //    }                                

        //    port.Close();
        //    master.Dispose();
        //    return Result;
        //}
        public string[] parameterGettingContinuous(byte addr, UInt16 reg_addr, byte reg_lenth, string type, SerialPort port)
        {
            string[] result = { "Success", "Success" };

            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = 1000;
            master.Transport.WriteTimeout = 1000;
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (InvalidOperationException)
                {
                    result[0] = "Error";
                    result[1] = "Port was open";
                }
                catch (UnauthorizedAccessException)
                {
                    result[0] = "Error";
                    result[1] = "Port was open";
                }
                catch (IOException)
                {
                    result[0] = "Error";
                    result[1] = "Port was not found";
                }
            }
            if (result[0] == "Success")
            {
                if (type == "uint16")
                {
                    try
                    {
                        ushort[] data = master.ReadHoldingRegisters(addr, reg_addr, 1);
                        result[1] = data[0].ToString();
                    }
                    catch (InvalidOperationException)
                    {
                        result[0] = "Error";
                        result[1] = "Port not Found";
                    }
                    catch (TimeoutException)
                    {
                        result[0] = "Error";
                        result[1] = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result[0] = "Error";
                        result[1] = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    }
                    //result = GetPeneralParameter(addr, reg_addr, reg_lenth);
                }
                else if (type == "uint32")
                {
                    try
                    {
                        ushort[] data = master.ReadHoldingRegisters(addr, reg_addr, 2);
                        UInt32 tData = ((UInt32)data[1] << 16) + data[0];
                        result[1] = tData.ToString();
                    }
                    catch (InvalidOperationException)
                    {
                        result[0] = "Error";
                        result[1] = "Port not Found";
                    }
                    catch (TimeoutException)
                    {
                        result[0] = "Error";
                        result[1] = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result[0] = "Error";
                        result[1] = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    }
                }
                else if (type == "float")
                {
                    try
                    {
                        ushort[] data = master.ReadHoldingRegisters(addr, reg_addr, 2);
                        UInt32 tData = ((UInt32)data[1] << 16) + data[0];
                        byte[] temp_bytes = BitConverter.GetBytes(tData);
                        Single temp_float = BitConverter.ToSingle(temp_bytes, 0);
                        result[1] = temp_float.ToString("N4");  //write t1,t2
                    }
                    catch (InvalidOperationException)
                    {
                        result[0] = "Error";
                        result[1] = "Port not Found";
                    }
                    catch (TimeoutException)
                    {
                        result[0] = "Error";
                        result[1] = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result[0] = "Error";
                        result[1] = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    }
                }
                else if (type == "char")
                {
                    try
                    {
                        ushort[] data = master.ReadHoldingRegisters(addr, reg_addr, reg_lenth);
                        char[] arrayChar = new char[data.Length * 2];
                        for (int i = 0, j = 0; i < data.Length; i++)
                        {
                            arrayChar[j] = (char)(data[i] & 0x00FF); j++;
                            arrayChar[j] = (char)(data[i] >> 8); j++;
                        }
                        result[1] = new string(arrayChar);

                    }
                    catch (InvalidOperationException)
                    {
                        result[0] = "Error";
                        result[1] = "Port not Found";
                    }
                    catch (TimeoutException)
                    {
                        result[0] = "Error";
                        result[1] = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result[0] = "Error";
                        result[1] = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    }
                }
            }
            //port.Close();     //连续读取探头参数，不必每次都开关端口
            //master.Dispose();
            return result;
        }
        public string parameterSeting(byte addr, UInt16 reg_addr, byte reg_lenth, UInt32 value, string type, SerialPort port)
        {
            string result = "Success";
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = 1000;
            master.Transport.WriteTimeout = 1000;
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (InvalidOperationException)
                {
                    result = "Port was open";
                }
                catch (UnauthorizedAccessException)
                {
                    result = "Port was open";
                }
                catch (IOException)
                {
                    result = "Port was not found";
                }
            }
            if (result == "Success")
            {
                if (type == "uint16")
                {
                    try
                    {

                        master.WriteSingleRegister(addr, reg_addr, (UInt16)value);

                    }
                    catch (InvalidOperationException)
                    {

                        result = "Port not Found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    }
                }
                else if (type == "uint32")
                {
                    try
                    {
                        ushort[] data = { (ushort)value, (ushort)(value >> 16) };
                        master.WriteMultipleRegisters(addr, reg_addr, data);
                    }
                    catch (InvalidOperationException)
                    {

                        result = "Port not Found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    }
                }
            }

            port.Close();
            master.Dispose();
            //stopwatch.Start();
            //stopwatch.Stop();
            //System.Diagnostics.Debug.WriteLine("modbuswriteTime:" + stopwatch.ElapsedMilliseconds.ToString());
            //stopwatch.Reset();
            return result;
        }
        public string parameterSeting(byte addr, UInt16 reg_addr, byte reg_lenth, Single value, string type, SerialPort port)
        {
            string result = "Success";
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = 1000;
            master.Transport.WriteTimeout = 1000;
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (InvalidOperationException)
                {
                    result = "Port was open";
                }
                catch (UnauthorizedAccessException)
                {
                    result = "Port was open";
                }
                catch (IOException)
                {
                    result = "Port was not found";
                }
            }
            if (result == "Success")
            {
                try
                {
                    byte[] temp_bytes = BitConverter.GetBytes(value);
                    UInt32 temp_int = 0;
                    temp_int += (uint)temp_bytes[3] << 3 * 8;
                    temp_int += (uint)temp_bytes[2] << 2 * 8;
                    temp_int += (uint)temp_bytes[1] << 1 * 8;
                    temp_int += (uint)temp_bytes[0] << 0 * 8;
                    ushort[] data = { (ushort)temp_int, (ushort)(temp_int >> 16) };
                    master.WriteMultipleRegisters(addr, reg_addr, data);
                }
                catch (InvalidOperationException)
                {

                    result = "Port not Found";
                }
                catch (TimeoutException)
                {
                    result = "Time out";
                }
                catch (Modbus.SlaveException ex)
                {
                    result = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                }
            }
            port.Close();
            master.Dispose();
            return result;
        }
        public string parameterSeting(byte addr, UInt16 reg_addr, byte reg_lenth, String value, string type, SerialPort port)
        {
            string result = "Success";
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = 1000;
            master.Transport.WriteTimeout = 1000;
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (InvalidOperationException)
                {
                    result = "Port was open";
                }
                catch (UnauthorizedAccessException)
                {
                    result = "Port was open";
                }
                catch (IOException)
                {
                    result = "Port was not found";
                }
            }
            if (result == "Success")
            {
                if ((type == "uint16")&&(reg_lenth==1))
                {
                    try
                    {
                        master.WriteSingleRegister(addr, reg_addr, Convert.ToUInt16(value));
                    }
                    catch (System.Exception ex)
                    {
                        result = ex.Message;
                    }
                    //catch (InvalidOperationException)
                    //{

                    //    result = "Port not Found";
                    //}
                    //catch (TimeoutException)
                    //{
                    //    result = "Time out";
                    //}
                    //catch (Modbus.SlaveException ex)
                    //{
                    //    result = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    //}
                }
                if ((type == "uint32") && (reg_lenth == 2))
                {
                    try
                    {
                        UInt32 Value = Convert.ToUInt32(value);
                        ushort[] data = { (ushort)Value, (ushort)(Value >> 16) };
                        master.WriteMultipleRegisters(addr,reg_addr, data);
                    }
                    catch (InvalidOperationException)
                    {

                        result = "Port not Found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    }
                }
                if ((type == "float") && (reg_lenth == 2))
                {
                    try
                    {
                        float Value=float.Parse(value);
                        byte[] temp_bytes = BitConverter.GetBytes(Value);
                        UInt32 temp_int = 0;
                        temp_int += (uint)temp_bytes[3] << 3 * 8;
                        temp_int += (uint)temp_bytes[2] << 2 * 8;
                        temp_int += (uint)temp_bytes[1] << 1 * 8;
                        temp_int += (uint)temp_bytes[0] << 0 * 8;
                        ushort[] data = { (ushort)temp_int, (ushort)(temp_int >> 16) };
                        master.WriteMultipleRegisters(addr, reg_addr, data);
                    }
                    catch (InvalidOperationException)
                    {

                        result = "Port not Found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    }
                }
                if ((type=="char")&&((reg_lenth==8||(reg_lenth==16))))  //通常为设置序列号
                {
                    try
                    {
                        ushort[] data = new ushort[value.Length / 2+1];
                        int rest = value.Length % 2;

                        for (int i = 0, j = 0; i < value.Length / 2; i++)
                        {
                            data[i] = (ushort)(value[j + 1] << 8 | value[j]);
                            j += 2;
                        }
                        if (rest == 0)
                        {
                            data[value.Length / 2] = 0;
                        }
                        else
                        {
                            data[value.Length / 2] = value[value.Length - 1];
                        }
                        master.WriteMultipleRegisters(addr, reg_addr, data);
                    }
                    catch (InvalidOperationException)
                    {

                        result = "Port not Found";
                    }
                    catch (TimeoutException)
                    {
                        result = "Time out";
                    }
                    catch (Modbus.SlaveException ex)
                    {
                        result = "unknown exception :" + ex.SlaveExceptionCode.ToString();
                    }
                }
               
            }
            port.Close();
            master.Dispose();
            return result;
        }
    }
}