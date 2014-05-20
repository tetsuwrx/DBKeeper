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

        private int MAX_BLOCKING_COUNT = 20;            // 累積警告回数

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
            // 累積警告回数の数で色分け
            if (BlockingCount > MAX_BLOCKING_COUNT)
            {
                AlertDisp = true;

                GradientStopColor.Color = Color.FromRgb(245, 16, 16);           // 赤色
            }
            else if (BlockingCount > 0 && BlockingCount <= MAX_BLOCKING_COUNT)
            {
                AlertDisp = true;

                GradientStopColor.Color = Color.FromRgb(245, 245, 16);           // 黄色
            }
            else
            {
                AlertDisp = false;

                GradientStopColor.Color = Color.FromRgb(16, 16, 245);           // 青色
            }
        }
    }
}
