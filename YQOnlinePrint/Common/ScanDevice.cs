using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YQOnlinePrint.Common
{
    public class ScanDevice : DeviceBase
    {
        public ScanDevice(string no, string name, string comname, int maxBarcodeCount) : base(no, name)
        {
            this.ComName = comname;
            this.MaxBarcodeCount = maxBarcodeCount;
        }

        /// <summary>
        /// 最大扫码个数
        /// </summary>
        public int MaxBarcodeCount { get; set; } = 1;

        ///// <summary>
        /////IP地址 端口默认9004
        ///// </summary>
        //public string IP { get; set; }

        /// <summary>
        ///IP地址 端口默认9004
        /// </summary>
        public string ComName { get; set; }

        //public int Port { get; set; } = 9004;

        private string _Data;
       
        /// <summary>
        /// 扫描到的有效条码，以竖线分割
        /// </summary>
        public string Data { get => _Data; set => Set(ref _Data, value); }

        private string _ScanTime;
        public string ScanTime { get => _ScanTime; set => Set(ref _ScanTime, value); }
    }
}
