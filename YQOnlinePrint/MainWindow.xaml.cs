using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YQOnlinePrint.ViewModel;
namespace YQOnlinePrint
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            viewModel = new MainViewModel();
            viewModel.OnShowMsg = ShowMsg;
            this.DataContext = viewModel;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void ShowMsg(string msg)
        {
            rtxtMsg.Dispatcher.Invoke(() =>
            {
                if (rtxtMsg.Document.Blocks.Count > 200)
                {
                    rtxtMsg.Document.Blocks.Clear();
                }
                rtxtMsg.AppendText(msg);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.LoadCfg("cfg.ini");
        }

        private void BtnTestReplace_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = new FileStream("60 18.prn", FileMode.Open);
            StreamReader reader = new StreamReader(fs);
            StringBuilder prnBuilder = new StringBuilder();
            while (!reader.EndOfStream)
            {
                prnBuilder.Append(reader.ReadLine().Trim() + "\n");
            }
            string oriZPL = prnBuilder.ToString();
            viewModel.ZPL = prnBuilder.ToString()
                .Replace("P1", viewModel.RightTopCfg.PrintHead + viewModel.RightTopCfg.PrintBody)
                .Replace("P2", viewModel.BarcodeCfg.PrintHead + viewModel.BarcodeCfg.PrintBody);
            reader.Close();
            reader.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}
