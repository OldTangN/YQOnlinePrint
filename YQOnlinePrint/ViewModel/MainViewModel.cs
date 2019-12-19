using Microsoft.Win32;
using MyLogLib;
using Newtonsoft.Json;
using OmronFinsTCP.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YQOnlinePrint.Common;
using YQOnlinePrint.Model;

namespace YQOnlinePrint.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        #region fields
        private PrintCfg _lt;
        private PrintCfg _rt;
        private PrintCfg _lb;
        private PrintCfg _rb;
        private PrintCfg _bar;
        private string _ZPL;
        private int _DPI = 300;
        private SerialScannerHelper scanner;
        private EtherNetPLC plc;
        private Socket tcpConnection;
        private bool _InitCompleted = false;
        private bool _IsBusy = false;
        #endregion

        #region properties
        public PrintCfg LeftTopCfg { get { return _lt; } set { Set(ref _lt, value); } }
        public PrintCfg LeftBottomCfg { get { return _lb; } set { Set(ref _lb, value); } }
        public PrintCfg RightTopCfg { get { return _rt; } set { Set(ref _rt, value); } }
        public PrintCfg RightBottomCfg { get { return _rb; } set { Set(ref _rb, value); } }
        public PrintCfg BarcodeCfg { get { return _bar; } set { Set(ref _bar, value); } }

        public string ZPL { get { return _ZPL; } set { Set(ref _ZPL, value); } }
        public int DPI { get { return _DPI; } set { Set(ref _DPI, value); } }
        public Action<string> OnShowMsg { get; set; }
        public bool InitCompleted { get { return _InitCompleted; } set { Set(ref _InitCompleted, value); } }
        public bool IsBusy { get { return _IsBusy; } set { Set(ref _IsBusy, value); } }
        #endregion

        #region contruction
        public MainViewModel()
        {
            this.DPI = SysCfg.DPI;
            LeftTopCfg = new PrintCfg(10, 10, 10, 10, false, false, "宋体", "国家电网");
            LeftBottomCfg = new PrintCfg(10, 10, 10, 10, false, false, "宋体", "1540001011500333473753");
            RightTopCfg = new PrintCfg(10, 10, 10, 10, false, false, "宋体", "01150033347375", "NO.");
            RightBottomCfg = new PrintCfg(10, 10, 10, 10, false, false, "宋体", "2019年");
            BarcodeCfg = new PrintCfg(10, 10, 10, 10, false, false, "", "1540001011500333473753", "");
            BarcodeCfg.PropertyChanged += BarcodeCfg_PropertyChanged;
        }
        #endregion

        #region GenerateZPLCmd
        private RelayCommand _GenerateZPLCmd;
        public RelayCommand GenerateZPLCmd => _GenerateZPLCmd ?? (_GenerateZPLCmd = new RelayCommand(GenerateZPL));

        private void GenerateZPL()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("^XA");//开始
            #region 左上 - 汉字
            int x_lefttop = PrintHelper.GetDot(LeftTopCfg.X, DPI);
            int y_lefttop = PrintHelper.GetDot(LeftTopCfg.Y, DPI);
            int w_lefttop = PrintHelper.GetDot(LeftTopCfg.Width, DPI);
            int h_lefttop = PrintHelper.GetDot(LeftTopCfg.Height, DPI);
            var lt_data = PrintHelper.getFontText(LeftTopCfg.PrintHead + LeftTopCfg.PrintBody,
                LeftTopCfg.FontName, LeftTopCfg.Rotate180 ? Orientation.O_180 : Orientation.Zero,
                h_lefttop, w_lefttop, LeftTopCfg.IsBold, false);
            builder.Append(lt_data.GetDateString("LT.GRF"));//下载图像内容
            builder.Append($"^FO{x_lefttop},{y_lefttop},0");//位置
            builder.Append($"^XGLT.GRF,1,1^FS");//打印图像内容 1表示不缩放
            builder.Append("\r\n");
            #endregion
            #region 右上 - 英文+.+数字
            int x_righttop = PrintHelper.GetDot(RightTopCfg.X, DPI);
            int y_righttop = PrintHelper.GetDot(RightTopCfg.Y, DPI);
            int w_righttop = PrintHelper.GetDot(RightTopCfg.Width, DPI);
            int h_righttop = PrintHelper.GetDot(RightTopCfg.Height, DPI);
            builder.Append($"^FO{x_righttop},{y_righttop},0");//位置
            builder.Append($"^A0{(RightTopCfg.Rotate180 ? "I" : "N")},{h_righttop},{w_righttop}");//大小
            string txt_righttop = RightTopCfg.PrintHead + RightTopCfg.PrintBody;
            builder.Append($"^FD{txt_righttop}^FS"); //打印内容
            builder.Append("\r\n");
            #endregion
            #region 左下 - 数字
            int x_leftbottom = PrintHelper.GetDot(LeftBottomCfg.X, DPI);
            int y_leftbottom = PrintHelper.GetDot(LeftBottomCfg.Y, DPI);
            int w_leftbottom = PrintHelper.GetDot(LeftBottomCfg.Width, DPI);
            int h_leftbottom = PrintHelper.GetDot(LeftBottomCfg.Height, DPI);
            builder.Append($"^FO{x_leftbottom},{y_leftbottom},0");//位置
            builder.Append($"^A0{(LeftBottomCfg.Rotate180 ? "I" : "N")},{h_leftbottom},{w_leftbottom}");//大小
            string txt_leftbottom = LeftBottomCfg.PrintHead + LeftBottomCfg.PrintBody;//内容
            builder.Append($"^FD{txt_leftbottom}^FS"); //打印内容
            builder.Append("\r\n");
            #endregion
            #region 右下 - 汉字
            int x_rightbottom = PrintHelper.GetDot(RightBottomCfg.X, DPI);
            int y_rightbottom = PrintHelper.GetDot(RightBottomCfg.Y, DPI);
            int w_rightbottom = PrintHelper.GetDot(RightBottomCfg.Width, DPI);
            int h_rightbottom = PrintHelper.GetDot(RightBottomCfg.Height, DPI);
            builder.Append($"^FO{x_rightbottom},{y_rightbottom},0\r\n");//位置
            var rb_data = PrintHelper.getFontText(RightBottomCfg.PrintHead + RightBottomCfg.PrintBody,
                RightBottomCfg.FontName, RightBottomCfg.Rotate180 ? Orientation.O_180 : Orientation.Zero,
                h_rightbottom, w_rightbottom, RightBottomCfg.IsBold, false);//内容
            builder.Append(rb_data.GetDateString("LB.GRF"));//下载图像内容
            builder.Append($"^FO{x_rightbottom},{y_rightbottom},0");//位置
            builder.Append($"^XGLB.GRF,1,1^FS");//打印图像内容 1表示不缩放
            builder.Append("\r\n");
            #endregion
            #region 条码
            int x_barcode = PrintHelper.GetDot(BarcodeCfg.X, DPI);
            int y_barcode = PrintHelper.GetDot(BarcodeCfg.Y, DPI);
            int w_barcode = PrintHelper.GetDot(BarcodeCfg.Width, DPI);
            int h_barcode = PrintHelper.GetDot(BarcodeCfg.Height, DPI);
            builder.Append($"^FO{x_barcode},{y_barcode},0\r\n");//位置
            builder.Append($"^BY{w_barcode},3.0,{h_barcode}");//尺寸
            string txt_barcode = $"^BC{(BarcodeCfg.Rotate180 ? "I" : "N")},,N,N,N,N^FD{BarcodeCfg.PrintBody}^FS";
            builder.Append(txt_barcode);
            builder.Append("\r\n");
            builder.Append("^XZ");
            #endregion
            this.ZPL = builder.ToString();
        }
        #endregion

        #region SaveCfgCmd
        private RelayCommand _SaveCfgCmd;
        public RelayCommand SaveCfgCmd => _SaveCfgCmd ?? (_SaveCfgCmd = new RelayCommand(SaveCfg));
        private void SaveCfg()
        {
            try
            {
                string fileName = GetCfgFile();
                if (string.IsNullOrEmpty(fileName))
                {
                    return;
                }
                List<PrintCfg> cfgs = new List<PrintCfg>();
                cfgs.Add(LeftTopCfg);
                cfgs.Add(RightTopCfg);
                cfgs.Add(LeftBottomCfg);
                cfgs.Add(RightBottomCfg);
                cfgs.Add(BarcodeCfg);
                string strJson = JsonConvert.SerializeObject(cfgs);
                FileStream fs = new FileStream(fileName, FileMode.Create);
                byte[] buffer = Encoding.UTF8.GetBytes(strJson);
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
                fs.Dispose();
                SysCfg.SetConfiguration("DPI", DPI);
                System.Windows.MessageBox.Show("保存完毕！");
            }
            catch (Exception ex)
            {
                MyLog.WriteLog("保存配置失败！", ex);
                System.Windows.MessageBox.Show("保存失败！");
            }
        }

        #endregion

        #region LoadCfgCmd
        private RelayCommand _LoadCfgCmd;
        public RelayCommand LoadCfgCmd => _LoadCfgCmd ?? (_LoadCfgCmd = new RelayCommand(LoadCfg));

        private void LoadCfg()
        {
            string fileName = GetCfgFile();
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            LoadCfg(fileName);
        }

        public void LoadCfg(string fileName)
        {
            Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(fileName))
                    {
                        ShowMsg($"{fileName}不存在!");
                        return;
                    }
                    ShowMsg("开始加载配置..." + fileName);
                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    byte[] buffer = new byte[200 * 1024];
                    int len = fs.Read(buffer, 0, buffer.Length);
                    byte[] data = new byte[len];
                    Array.Copy(buffer, data, len);
                    string strCfgs = Encoding.UTF8.GetString(data);
                    List<PrintCfg> cfgs = JsonConvert.DeserializeObject<List<PrintCfg>>(strCfgs);
                    if (cfgs == null || cfgs.Count < 5)
                    {
                        ShowMsg("配置文件格式错误或配置数量少于5个!");
                        return;
                    }
                    LeftTopCfg = cfgs[0];
                    RightTopCfg = cfgs[1];
                    LeftBottomCfg = cfgs[2];
                    RightBottomCfg = cfgs[3];
                    BarcodeCfg = cfgs[4];
                    ShowMsg("配置加载完毕!");
                }
                catch (Exception ex)
                {
                    MyLog.WriteLog("加载配置失败！", ex);
                    ShowMsg("加载配置失败！");
                }
            });
        }
        #endregion

        #region AutoCtlCmd
        private RelayCommand _AutoCtlCmd;
        public RelayCommand AutoCtlCmd => _AutoCtlCmd ?? (_AutoCtlCmd = new RelayCommand(AutoCtl, () => !(InitCompleted || IsBusy)));
        private CancellationTokenSource cancellation;
        private void AutoCtl()
        {
            Stop();
            IsBusy = true;
            InitCompleted = false;
            Task.Run(() =>
            {
                try
                {
                    scanner?.DisConnect();
                    scanner = new SerialScannerHelper(new ScanDevice(SysCfg.NO, SysCfg.NAME, SysCfg.SCAN_COM, 1));
                    scanner.OnError += Scanner_OnError;
                    scanner.OnScanned += Scanner_OnScanned;
                    ShowMsg("连接扫码枪...");
                    if (!scanner.Connect())
                    {
                        ShowMsg("连接扫码枪失败！");
                        return;
                    }
                    ShowMsg("连接扫码枪成功!");
                    plc = new EtherNetPLC();
                    try
                    {
                        ShowMsg("连接PLC...");
                        int linkflag = plc.Link(SysCfg.PLC_IP, SysCfg.PLC_PORT, 2000);
                        if (linkflag == -1)
                        {
                            ShowMsg("连接PLC失败！");
                            return;
                        }
                        ShowMsg("连接PLC成功!");
                    }
                    catch (Exception ex)
                    {
                        ShowMsg("连接PLC失败！");
                        MyLog.WriteLog("连接PLC失败！", ex);
                        return;
                    }
                    try
                    {
                        ShowMsg("连接打印机...");
                        tcpConnection = new Socket(SocketType.Stream, ProtocolType.Tcp);
                        tcpConnection.Connect(SysCfg.PRINTER_IP, SysCfg.PRINTER_PORT);
                        if (!tcpConnection.Connected)
                        {
                            ShowMsg("连接打印机失败！");
                            return;
                        }
                        ShowMsg("连接打印机成功!");
                    }
                    catch (Exception ex)
                    {
                        MyLog.WriteLog("连接打印机失败！", ex);
                        ShowMsg("连接打印机失败！");
                        return;
                    }
                    ShowMsg("自动控制已启动!");
                    cancellation = new CancellationTokenSource();
                    Task.Run(new Action(ListenPLC), cancellation.Token);
                    InitCompleted = true;
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }
        private readonly Object LOCK = new object { };
        private void ListenPLC()
        {
            while (true)
            {
                lock(LOCK)
                {
                    try
                    {
                        short reData;
                        plc.ReadWord(PlcMemory.DM, SysCfg.DM_READY, out reData);
                        if (reData != 1)//电表未到位
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        ShowMsg("电表到位,触发扫码!");
                        //改写PLC状态，防止重复扫码
                        plc.WriteWord(PlcMemory.DM, SysCfg.DM_READY, 99);
                        //触发扫码
                        scanner.TriggerScan();//触发扫码，扫码失败会重试一次
                    }
                    catch (Exception ex)
                    {
                        string errMsg = "监听PLC状态失败！";
                        MyLog.WriteLog(errMsg, ex);
                        ShowMsg(errMsg);
                    }
                }
                Thread.Sleep(1000);
            }
        }
        private void Scanner_OnScanned(ScanDevice scanner, string dataMsg)
        {
            string barcode = dataMsg?.Replace("\r", "").Replace("\n", "");
            if (string.IsNullOrEmpty(barcode))
            {
                ShowMsg("扫码为空！");
                return;
            }
            BarcodeCfg.PrintBody = barcode;
            Thread.Sleep(50);
            //生成打印ZPL
            try
            {
                ShowMsg("生成ZPL...");
                GenerateZPL();
            }
            catch (Exception ex)
            {
                string errMsg = "生成ZPL失败！";
                MyLog.WriteLog(errMsg, ex);
                ShowMsg(errMsg);
                return;
            }
            //发送至打印机
            try
            {
                ShowMsg("发送至打印机...");
                byte[] data = Encoding.ASCII.GetBytes(ZPL);
                int len = tcpConnection.Send(data);
            }
            catch (Exception ex)
            {
                string errMsg = "发送至打印机失败！";
                MyLog.WriteLog(errMsg, ex);
                ShowMsg(errMsg);
            }
        }

        private void Scanner_OnError(string errMsg)
        {
            ShowMsg("扫码异常！" + errMsg);
        }
        #endregion

        #region StopCmd
        private RelayCommand _StopCmd;
        public RelayCommand StopCmd => _StopCmd ?? (_StopCmd = new RelayCommand(Stop));

        private void Stop()
        {
            try
            {
                InitCompleted = false;
                cancellation?.Cancel();
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region PrintCmd
        private RelayCommand _PrintCmd;
        public RelayCommand PrintCmd => _PrintCmd ?? (_PrintCmd = new RelayCommand(Print));

        private void Print()
        {
            try
            {
                if (tcpConnection?.Connected != true)
                {
                    ShowMsg("连接打印机...");
                    tcpConnection = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    tcpConnection.Connect(SysCfg.PRINTER_IP, SysCfg.PRINTER_PORT);
                    if (!tcpConnection.Connected)
                    {
                        ShowMsg("连接打印机失败！");
                        return;
                    }
                    ShowMsg("连接打印机成功!");
                }
                int len = tcpConnection.Send(Encoding.ASCII.GetBytes(ZPL));
            }
            catch (Exception ex)
            {
                MyLog.WriteLog("连接打印机失败！", ex);
                ShowMsg("连接打印机失败！");
                return;
            }
        }
        #endregion

        private string GetCfgFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            fileDialog.Filter = "*.ini|*.ini";
            fileDialog.Multiselect = false;
            if (!fileDialog.ShowDialog().GetValueOrDefault())
            {
                return null;
            }
            return fileDialog.FileName;
        }

        private void ShowMsg(string msg)
        {
            try
            {
                OnShowMsg?.Invoke(DateTime.Now.ToString("HH:mm:ss.fff --> ") + msg + "\n");
            }
            catch (Exception)
            {
            }
        }

        private void BarcodeCfg_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PrintBody")
            {
                LeftBottomCfg.PrintBody = BarcodeCfg.PrintBody;
                RightTopCfg.PrintBody = "";
                //右上数字=条码去掉最后1位，再取最后14位数字
                if (BarcodeCfg.PrintBody?.Length >= 15)//
                {
                    RightTopCfg.PrintBody = BarcodeCfg.PrintBody.Substring(BarcodeCfg.PrintBody.Length - 15, 14);
                }
                else
                {
                    RightTopCfg.PrintBody = BarcodeCfg.PrintBody;
                }
            }
        }
    }
}
