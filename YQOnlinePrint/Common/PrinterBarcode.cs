using System; 
using System.Data; 
using System.Configuration; 
using System.Web; 
//using System.Web.Security; 
//using System.Web.UI; 
//using System.Web.UI.WebControls; 
//using System.Web.UI.WebControls.WebParts; 
//using System.Web.UI.HtmlControls; 
using System.Runtime.InteropServices; 
using System.Collections.Generic; 
using System.Text;


namespace NWPresentation
{
    public class PrinterBarCode
    {
        /// <summary> 
        /// 获取字体编码 
        /// </summary> 
        /// <param name="printText">打印文本 </param> 
        /// <param name="printFont">字体名称 </param> 
        /// <param name="Orientation">旋转方向 </param> 
        /// <param name="height">高度 </param> 
        /// <param name="width">宽度 </param> 
        /// <param name="IsBold">是否粗体 </param> 
        /// <param name="IsItalic">是否斜体 </param> 
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
                //int bold = 0;
                //int italic = 0;
                int count = GETFONTHEX(printText, printFont, 0, height, width, bold, italic, buder);
                if (count > 0)
                {
                    //temp = Marshal.PtrToStringAnsi(ptrIn); 
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
                throw ex;
            }

            return result;
        }


        [DllImport(@"FNTHEX32.dll")]
        public static extern int GETFONTHEX(
                      string chnstr,
                      string fontname,
                      int orient,
                      int height,
                      int width,
                      int bold,
                      int italic,
                      StringBuilder param1);
    }
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

public enum Orientation 
{ 
    Orien0, 
    Orien90, 
    Orien180, 
    Orien270 
}