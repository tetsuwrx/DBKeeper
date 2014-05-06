using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace MeterControls
{
    /// <summary>
    /// BlockingAlert.xaml の相互作用ロジック
    /// </summary>
    public partial class BlockingAlert : UserControl
    {
        private bool AlertDisp = false;                 // 警告表示するかどうか

        public BlockingAlert()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 引数にて渡された値を見て、警告表示をするかどうかを判断する
        /// </summary>
        /// <param name="BlockingCount"></param>
        public void CheckBlockingAlert(int BlockingCount)
        {
            if (BlockingCount > 0)
            {
                AlertDisp = true;

                GradientStopColor.Color = Color.FromRgb(245, 16, 16);           // 赤色
            }
            else
            {
                AlertDisp = false;

                GradientStopColor.Color = Color.FromRgb(16, 16, 245);           // 青色
            }
        }
    }
}
