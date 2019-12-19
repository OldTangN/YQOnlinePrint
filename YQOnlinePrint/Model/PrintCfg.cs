using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YQOnlinePrint.Common;

namespace YQOnlinePrint.Model
{
    public class PrintCfg : ObservableObject
    {
        #region fields
        private double _X = 0;
        private double _Y = 0;
        private double _Width = 20;
        private double _Height = 20;
        private string _FontName = "宋体";
        private string _PrintBody = "";
        private bool _IsBold = false;
        private bool _Rotate180 = false;
        private string _PrintHead = "";
        #endregion

        #region constructor
        public PrintCfg()
        {

        }
        public PrintCfg(double x, double y, double width, double height, bool bold, bool r180, string font, string body, string head = "")
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.IsBold = bold;
            this.Rotate180 = r180;
            this.FontName = font;
            this.PrintBody = body;
            this.PrintHead = head;
        }
        #endregion

        #region properties
        public bool Rotate180 { get { return _Rotate180; } set { Set(ref _Rotate180, value); } }
        public bool IsBold { get { return _IsBold; } set { Set(ref _IsBold, value); } }
        public string PrintHead { get { return _PrintHead; } set { Set(ref _PrintHead, value); } }
        public string PrintBody { get { return _PrintBody; } set { Set(ref _PrintBody, value); } }
        public string FontName { get { return _FontName; } set { Set(ref _FontName, value); } }
        public double Height { get { return _Height; } set { Set(ref _Height, value); } }
        public double Width { get { return _Width; } set { Set(ref _Width, value); } }
        public double Y { get { return _Y; } set { Set(ref _Y, value); } }
        public double X { get { return _X; } set { Set(ref _X, value); } }
        #endregion
    }
}
