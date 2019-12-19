using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YQOnlinePrint.Common
{
    public class DeviceBase : ObservableObject
    {

        public DeviceBase(string no, string name)
        {
            this.NO = no;
            this.NAME = name;
        }

        private string _NO;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string NO { get { return _NO; } set { Set(ref _NO, value); } }

        private string _NAME;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string NAME
        {
            get { return _NAME; }
            set { Set(ref _NAME, value); }
        }

        private bool _Enable = false;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get { return _Enable; } set { Set(ref _Enable, value); } }
    }
}
