using MyLogLib;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YQOnlinePrint.Common
{
    public class SerialScannerHelper : ScannerHelper
    {

        private SerialPort serial;

        public SerialScannerHelper(ScanDevice scanSetting)
        {
            this.Scanner = scanSetting;

        }

        private List<byte> Buffer = new List<byte>();

        //private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    lock (Buffer)
        //    {
        //        int len = serial.BytesToRead;
        //        byte[] buffer = new byte[len];
        //        serial.Read(buffer, 0, len);
        //        Buffer.AddRange(buffer);//断帧、粘包处理
        //        int idx_0a;//0x0d 0x0a 结束符
        //        while ((idx_0a = Buffer.IndexOf(0x0a)) != -1)//断帧、粘包处理
        //        {
        //            byte[] data = Buffer.Take(idx_0a - 1).ToArray();//截取0x0d 0x0a前报文
        //            Buffer.RemoveRange(0, idx_0a + 1);
        //            string strCodes = Encoding.ASCII.GetString(data);//TODO:编码格式
        //            RaiseScanned(this.Scanner, strCodes);
        //        }
        //    }
        //}
        protected override string Receive()
        {
            string data = "";
            try
            {
                byte[] buffer = new byte[1024];
                int len = serial.Read(buffer, 0, buffer.Length);
                byte[] bytArrData = buffer.Take(len).ToArray();
                data = Encoding.ASCII.GetString(bytArrData);
                //MyLog.WriteLog(this.Scanner.NAME + this.Scanner.IP + " 扫码值:" + (string.IsNullOrEmpty(data) ? "空" : data), "SCAN");
            }
            catch (Exception ex)
            {
                string errMsg = $"{Scanner.NAME}{Scanner.ComName}接收数据处理异常！" + ex.Message;
                //MyLog.WriteLog(errMsg, ex, "SCAN");
                RaiseError(errMsg);
            }
            return data;
        }
        private CancellationTokenSource cancellation;
        public override bool Connect()
        {
            try
            {
                DisConnect();
                serial = new SerialPort(Scanner.ComName, 115200, Parity.None, 8, StopBits.One);
                //serial.DataReceived += SerialPort_DataReceived;
                serial.Open();
                return true;
            }
            catch (Exception ex)
            {
                string errMsg = $"连接扫码枪{Scanner.ComName}失败！";
                MyLog.WriteLog(errMsg, ex);
                RaiseError(errMsg);
            }
            return false;
        }

        public override void DisConnect()
        {
            try
            {
                cancellation?.Cancel();
                serial?.Close();
                serial?.Dispose();
            }
            catch (Exception)
            {
            }
        }

        protected override bool Send(byte[] data)
        {
            try
            {
                serial.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                string errMsg = $"向{Scanner.ComName}发送命令失败！";
                MyLog.WriteLog(errMsg, ex);
                RaiseError(errMsg);
            }
            return true;
        }
    }
}
