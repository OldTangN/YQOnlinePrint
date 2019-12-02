using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YQOnlinePrint.Common
{
    public class SysCfg
    {
        /// <summary>
        /// NO
        /// </summary>
        public static string NO => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// NAME
        /// </summary>
        public static string NAME => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// SCAN_COM
        /// </summary>
        public static string SCAN_COM => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// PLC_IP
        /// </summary>
        public static string PLC_IP => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// PLC_PORT
        /// </summary>
        public static short PLC_PORT => ConfigurationUtil.GetConfiguration(short.Parse, () => (short)0);

        /// <summary>
        /// 托盘到位检测的DM地址
        /// </summary>
        public static short DM_READY => ConfigurationUtil.GetConfiguration(short.Parse, () => (short)0);


        /// <summary>
        /// 打印机端口
        /// </summary>
        public static int PRINTER_PORT => ConfigurationUtil.GetConfiguration(int.Parse, () => 0);

        /// <summary>
        /// 打印机IP
        /// </summary>
        public static string PRINTER_IP => ConfigurationUtil.GetConfiguration(Convert.ToString, () => "");

        /// <summary>
        /// 打印机DPI
        /// </summary>
        public static int DPI => ConfigurationUtil.GetConfiguration(int.Parse, () => 300);

        /// <summary>
        /// 心跳间隔
        /// <para>默认1000ms</para>
        /// </summary>
        public static int HEARTBEAT_TIMESPAN => ConfigurationUtil.GetConfiguration(int.Parse, () => 1000);

        public static bool SetConfiguration(string key, object val)
        {
            return ConfigurationUtil.SetConfiguration(key, val.ToString());
        }
    }
}
