using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace YQOnlinePrint.Common
{
    public class PrintHelper
    {
        public static int GetDot(double milimeter, float dpi)
        {
            //1英寸=25.4mm
            //300dpi = 300dot per inch 300点每英寸
            return (int)Math.Round(milimeter * 300 / 25.4, 0);
        }

        /// <summary> 
        /// 获取打印ZPL 
        /// </summary> 
        /// <param name="printText">打印文本 </param> 
        /// <param name="printFont">字体名称 </param> 
        /// <param name="Orientation">旋转方向 </param> 
        /// <param name="height">高度 </param> 
        /// <param name="width">宽度 </param> 
        /// <param name="IsBold">是否粗体 </param> 
        /// <param name="IsItalic">是否斜体 </param> 
        /// <returns>失败返回 null</returns> 
        /// <returns> </returns> 
        public static ConverFontToImageResult getFontText(string printText, string printFont, Orientation Orientation, int height, int width, bool IsBold, bool IsItalic)
        {
            ConverFontToImageResult result = null;
            try
            {
                StringBuilder buder = new StringBuilder(21 * 1024);
                string temp = string.Empty;
                int bold = IsBold ? 1 : 0;
                int italic = IsItalic ? 1 : 0;
                int count = GETFONTHEX(printText, printFont, (int)Orientation, height, width, bold, italic, buder);
                if (count > 0)
                {
                    result = new ConverFontToImageResult();
                    temp = buder.ToString();
                    string[] data = temp.Split(',');

                    result.ImageName = data[0].Replace("~DG", "");
                    result.Instruction = "~DG";
                    result.TotalSize = data[1];
                    result.RowSize = data[2];
                    result.ImageData = data[3];
                }
            }
            catch (Exception ex)
            {
                MyLogLib.MyLog.WriteLog(ex);
            }

            return result;
        }

        /// <summary>
        /// 获取打印ZPL
        /// </summary>
        /// <param name="printText">打印文本</param>
        /// <param name="fontname">字体</param>
        /// <param name="orient">旋转角度0,90,180,270</param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="bold">1=粗体，0=正常</param>
        /// <param name="italic">1=斜体，0=正常</param>
        /// <param name="data">输出ZPL字符串</param>
        /// <returns>0=失败</returns>
        [DllImport("FNTHEX32.dll")]
        public static extern int GETFONTHEX(
                      string printText,
                      string fontname,
                      int orient,
                      int height,
                      int width,
                      int bold,
                      int italic,
                      StringBuilder data);
    }
    public enum Orientation : int
    {
        Zero = 0,
        O_90 = 90,
        O_180 = 180,
        O_270 = 270
    }
    public class ConverFontToImageResult
    {
        private string imageName;
        /// <summary> 
        /// 文件名称 
        /// </summary> 
        public string ImageName
        {
            get { return imageName; }
            set { imageName = value; }
        }

        private string instruction;
        /// <summary> 
        /// 图片指令 
        /// </summary> 
        public string Instruction
        {
            get { return instruction; }
            set { instruction = value; }
        }
        private string imageData;
        /// <summary> 
        /// 图片数据 
        /// </summary> 
        public string ImageData
        {
            get { return imageData; }
            set { imageData = value; }
        }

        private string totalSize;
        /// <summary> 
        /// 总共字节数 
        /// </summary> 
        public string TotalSize
        {
            get { return totalSize; }
            set { totalSize = value; }
        }
        private string rowSize;
        /// <summary> 
        /// 每行字节数 
        /// </summary> 
        public string RowSize
        {
            get { return rowSize; }
            set { rowSize = value; }
        }

        /// <summary> 
        /// 获取包装过的数据字符串 
        /// </summary> 
        /// <returns> </returns> 
        public string GetDateString()
        {
            return string.Format("{0}{1},{2},{3},{4}", this.Instruction, this.ImageName, this.TotalSize, this.RowSize, this.ImageData);
        }


        /// <summary> 
        /// 获取包装过的数据字符串 
        /// </summary> 
        /// <returns> </returns> 
        public string GetDateString(string iamgeName)
        {
            return string.Format("{0}{1},{2},{3},{4}", this.Instruction, iamgeName, this.TotalSize, this.RowSize, this.ImageData);
        }
    }

}
