using MyLogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YQOnlinePrint.Common
{
    public class SocketScannerHelper : ScannerHelper
    {
        private Socket socket;

        public SocketScannerHelper(ScanDevice scanSetting)
        {
            this.Scanner = scanSetting;

        }

        private CancellationTokenSource cancellation;
        public override bool Connect()
        {
            try
            {
                DisConnect();
                cancellation = new CancellationTokenSource();
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.ReceiveTimeout = 3000;//3s接收超时
                socket.SendTimeout = 1000;//1s发送超时
                socket.Connect(new IPEndPoint(IPAddress.Parse(Scanner.IP), Scanner.Port));
                return true;
            }
            catch (Exception ex)
            {
                string errMsg = $"连接扫码枪{Scanner.NAME}{Scanner.IP}失败！";
                //MyLog.WriteLog(errMsg, ex, "SCAN");
                RaiseError(errMsg);
            }
            return false;
        }

        protected override string Receive()
        {
            string data = "";
            try
            {
                byte[] buffer = new byte[1024];
                int len = socket.Receive(buffer);
                byte[] bytArrData = buffer.Take(len).ToArray();
                data = Encoding.ASCII.GetString(bytArrData);
                //MyLog.WriteLog(this.Scanner.NAME + this.Scanner.IP + " 扫码值:" + (string.IsNullOrEmpty(data) ? "空" : data), "SCAN");
            }
            catch (SocketException ex)//接收超时异常不处理
            {
                if (!socket.Connected)
                {
                    try
                    {
                        Connect();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                if (!socket.Connected)
                {
                    try
                    {
                        Connect();
                    }
                    catch
                    {
                    }
                }
                string errMsg = $"{Scanner.NAME}{Scanner.IP}接收数据处理异常！" + ex.Message;
                //MyLog.WriteLog(errMsg, ex, "SCAN");
                RaiseError(errMsg);
            }
            return data;
        }

        public override void DisConnect()
        {
            try
            {
                cancellation?.Cancel();
                socket?.Close();
                socket?.Dispose();
            }
            catch (Exception)
            {
            }
        }
        protected override bool Send(byte[] data)
        {
            try
            {
                if (!socket.Connected)
                {
                    Connect();
                }
                if (socket.Connected)
                {
                    //string msg = "";
                    //data.ToList().ForEach(d => msg = msg + " " + d.ToString());
                    //MyLog.WriteLog(this.Scanner.NAME + this.Scanner.IP + " 发送：" + msg, "SCAN");
                    socket.Send(data);
                    return true;
                }
                return false;
            }
            catch (Exception ex1)
            {
                string errMsg = $"向{Scanner.NAME}{Scanner.IP}发送命令失败！（1）";
                //MyLog.WriteLog(errMsg, ex1, "SCAN");
                RaiseError(errMsg);
                try
                {
                    if (!socket.Connected)
                    {
                        Connect();
                    }
                    if (socket.Connected)
                    {
                        socket.Send(data);
                        return true;
                    }
                    return false;
                }
                catch (Exception ex2)
                {
                    string errMsg2 = $"向{Scanner.NAME}{Scanner.IP}发送命令失败！（2）";
                    //MyLog.WriteLog(errMsg2, ex2, "SCAN");
                    RaiseError(errMsg2);
                    return false;
                }
            }
        }
    }
}
