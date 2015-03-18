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

namespace MeterControls
{
    /// <summary>
    /// BarControl.xaml の相互作用ロジック
    /// </summary>
    public partial class BarControl : UserControl
    {
        public BarControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// タイトルの設定
        /// </summary>
        public string BarTitle
        {
            get { return tbTitle.Text; }
            set { tbTitle.Text = value; }
        }

        /// <summary>
        /// バーの値を設定
        /// </summary>
        public int MeterValue
        {
            get
            {
                int returnValue = 0;                               // 戻り値用
                
                SolidColorBrush checkBrush = new SolidColorBrush();     // チェック用
                checkBrush.Color = Colors.White;

                if (BarMeter01.Fill != checkBrush)
                {
                    returnValue = 10;
                }
                else if (BarMeter02.Fill != checkBrush)
                {
                    returnValue = 20;
                }
                else if (BarMeter03.Fill != checkBrush)
                {
                    returnValue = 30;
                }
                else if (BarMeter04.Fill != checkBrush)
                {
                    returnValue = 40;
                }
                else if (BarMeter05.Fill != checkBrush)
                {
                    returnValue = 50;
                }
                else if (BarMeter06.Fill != checkBrush)
                {
                    returnValue = 60;
                }
                else if (BarMeter07.Fill != checkBrush)
                {
                    returnValue = 70;
                }
                else if (BarMeter08.Fill != checkBrush)
                {
                    returnValue = 80;
                }
                else if (BarMeter09.Fill != checkBrush)
                {
                    returnValue = 90;
                }
                else if (BarMeter10.Fill != checkBrush)
                {
                    returnValue = 100;
                }

                return returnValue;
            }
            set
            {
                int checkValue = value;                                  // 取得してきた値

                SolidColorBrush defaultBrush = new SolidColorBrush();       // 初期設定色のブラシ
                defaultBrush.Color = Colors.White;                          // 初期設定食を設定
                SolidColorBrush normalBrush = new SolidColorBrush();        // 設定する色のブラシ
                normalBrush.Color = Colors.Cyan;                            // 通常色を設定
                SolidColorBrush alertBrush = new SolidColorBrush();      // 注意色を設定するブラシ
                alertBrush.Color = Colors.Yellow;                        // 注意色(黄色)を設定
                SolidColorBrush warningBrush = new SolidColorBrush();    // 警告色を設定するブラシ
                warningBrush.Color = Colors.Red;                         // 警告色を設定
                
                // 設定値によって値塗りつぶしを行う
                if (checkValue >= 100)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = normalBrush;
                    BarMeter03.Fill = normalBrush;
                    BarMeter04.Fill = normalBrush;
                    BarMeter05.Fill = normalBrush;
                    BarMeter06.Fill = normalBrush;
                    BarMeter07.Fill = alertBrush;
                    BarMeter08.Fill = alertBrush;
                    BarMeter09.Fill = warningBrush;
                    BarMeter10.Fill = warningBrush; 
                }
                else if (checkValue >= 90)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = normalBrush;
                    BarMeter03.Fill = normalBrush;
                    BarMeter04.Fill = normalBrush;
                    BarMeter05.Fill = normalBrush;
                    BarMeter06.Fill = normalBrush;
                    BarMeter07.Fill = alertBrush;
                    BarMeter08.Fill = alertBrush;
                    BarMeter09.Fill = warningBrush;
                    BarMeter10.Fill = defaultBrush;
                }
                else if (checkValue >= 80)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = normalBrush;
                    BarMeter03.Fill = normalBrush;
                    BarMeter04.Fill = normalBrush;
                    BarMeter05.Fill = normalBrush;
                    BarMeter06.Fill = normalBrush;
                    BarMeter07.Fill = alertBrush;
                    BarMeter08.Fill = alertBrush;
                    BarMeter09.Fill = defaultBrush;
                    BarMeter10.Fill = defaultBrush;
                }
                else if (checkValue >= 70)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = normalBrush;
                    BarMeter03.Fill = normalBrush;
                    BarMeter04.Fill = normalBrush;
                    BarMeter05.Fill = normalBrush;
                    BarMeter06.Fill = normalBrush;
                    BarMeter07.Fill = alertBrush;
                    BarMeter08.Fill = defaultBrush;
                    BarMeter09.Fill = defaultBrush;
                    BarMeter10.Fill = defaultBrush;
                }
                else if (checkValue >= 60)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = normalBrush;
                    BarMeter03.Fill = normalBrush;
                    BarMeter04.Fill = normalBrush;
                    BarMeter05.Fill = normalBrush;
                    BarMeter06.Fill = normalBrush;
                    BarMeter07.Fill = defaultBrush;
                    BarMeter08.Fill = defaultBrush;
                    BarMeter09.Fill = defaultBrush;
                    BarMeter10.Fill = defaultBrush;
                }
                else if (checkValue >= 50)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = normalBrush;
                    BarMeter03.Fill = normalBrush;
                    BarMeter04.Fill = normalBrush;
                    BarMeter05.Fill = normalBrush;
                    BarMeter06.Fill = defaultBrush;
                    BarMeter07.Fill = defaultBrush;
                    BarMeter08.Fill = defaultBrush;
                    BarMeter09.Fill = defaultBrush;
                    BarMeter10.Fill = defaultBrush;
                }
                else if (checkValue >= 40)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = normalBrush;
                    BarMeter03.Fill = normalBrush;
                    BarMeter04.Fill = normalBrush;
                    BarMeter05.Fill = defaultBrush;
                    BarMeter06.Fill = defaultBrush;
                    BarMeter07.Fill = defaultBrush;
                    BarMeter08.Fill = defaultBrush;
                    BarMeter09.Fill = defaultBrush;
                    BarMeter10.Fill = defaultBrush;
                }
                else if (checkValue >= 30)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = normalBrush;
                    BarMeter03.Fill = normalBrush;
                    BarMeter04.Fill = defaultBrush;
                    BarMeter05.Fill = defaultBrush;
                    BarMeter06.Fill = defaultBrush;
                    BarMeter07.Fill = defaultBrush;
                    BarMeter08.Fill = defaultBrush;
                    BarMeter09.Fill = defaultBrush;
                    BarMeter10.Fill = defaultBrush;
                }
                else if (checkValue >= 20)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = normalBrush;
                    BarMeter03.Fill = defaultBrush;
                    BarMeter04.Fill = defaultBrush;
                    BarMeter05.Fill = defaultBrush;
                    BarMeter06.Fill = defaultBrush;
                    BarMeter07.Fill = defaultBrush;
                    BarMeter08.Fill = defaultBrush;
                    BarMeter09.Fill = defaultBrush;
                    BarMeter10.Fill = defaultBrush;
                }
                else if (checkValue >= 10)
                {
                    BarMeter01.Fill = normalBrush;
                    BarMeter02.Fill = defaultBrush;
                    BarMeter03.Fill = defaultBrush;
                    BarMeter04.Fill = defaultBrush;
                    BarMeter05.Fill = defaultBrush;
                    BarMeter06.Fill = defaultBrush;
                    BarMeter07.Fill = defaultBrush;
                    BarMeter08.Fill = defaultBrush;
                    BarMeter09.Fill = defaultBrush;
                    BarMeter10.Fill = defaultBrush;
                }
                else
                {
                    BarMeter01.Fill = defaultBrush;
                    BarMeter02.Fill = defaultBrush;
                    BarMeter03.Fill = defaultBrush;
                    BarMeter04.Fill = defaultBrush;
                    BarMeter05.Fill = defaultBrush;
                    BarMeter06.Fill = defaultBrush;
                    BarMeter07.Fill = defaultBrush;
                    BarMeter08.Fill = defaultBrush;
                    BarMeter09.Fill = defaultBrush;
                    BarMeter10.Fill = defaultBrush;
                }
            }
        }
    }
}
