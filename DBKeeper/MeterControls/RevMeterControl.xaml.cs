using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace MeterControl
{
    /// <summary>
    /// RevMeterControl.xaml の相互作用ロジック
    /// </summary>
    public partial class RevMeterControl : UserControl
    {
        public RevMeterControl()
        {
            InitializeComponent();
        }

        public string MeterTitle
        {
            get { return tbTitle.Text; }
            set { tbTitle.Text = value; }
        }

        public string MeterValueText
        {
            set
            {
                tbValue.Text = value;
            }
            get
            {
                return tbValue.Text;
            }
        }

        public double MeterValue
        {
            set
            {
                tbValue.Text = (value / 100).ToString("0.0%");

                double angle = 0;

                // 30%までのメモリは丸められているので角度は微調整
                if (value > 0 && value <= 30)
                {
                    // double lvalue = System.Math.Log10(value);
                    //angle += lvalue * 27;
                    double lvalue = value;

                    angle += -134 + (lvalue * 0.9);

                }
                else if (value > 30 && value <= 100)
                {
                    // double lvalue = System.Math.Log10(value);
                    //angle += lvalue * 27;
                    double lvalue = value;

                    angle += (-109 + ((lvalue - 30) * 2.7)) + 1;
                }
                else
                {
                    angle = -135;
                }
                Duration duration = new Duration(TimeSpan.FromMilliseconds(500));
                DoubleAnimation animation = new DoubleAnimation(angle, duration);
                indicatorRotation.BeginAnimation(RotateTransform.AngleProperty, animation);

                if (value >= 100)
                {
                    tbValue.Foreground = Brushes.Red;
                    tbValue.FontWeight = FontWeights.Bold;
                }
                else
                {
                    //tbValue.Foreground = null;
                    tbValue.FontWeight = FontWeights.Normal;
                }
            }
            get
            {
                return double.Parse(tbValue.Text);
            }
        }
    }
}
