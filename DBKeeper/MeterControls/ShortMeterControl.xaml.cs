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
    /// ShortMeterControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ShortMeterControl : UserControl
    {
        public ShortMeterControl()
        {
            InitializeComponent();
        }

        public string MeterTitle
        {
            get { return tbTitle.Text; }
            set
            {
                string titleValue = value;
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                int num = sjisEnc.GetByteCount(titleValue);

                tbTitle.Text = titleValue;

                if (num > 22)
                {
                    tbTitle.FontSize--;
                }
            }
        }

        public double MeterValue
        {
            set
            {
                tbValue.Text = (value / 100).ToString("0.0%");

                double angle = -90;
                if (value > 1)
                {
                    // double lvalue = System.Math.Log10(value);
                    //angle += lvalue * 27;
                    double lvalue = value;

                    angle += lvalue * 1.2;
                }
                Duration duration = new Duration(TimeSpan.FromMilliseconds(500));
                DoubleAnimation animation = new DoubleAnimation(angle, duration);
                indicatorRotation.BeginAnimation(RotateTransform.AngleProperty, animation);
            }
            get
            {
                return double.Parse(tbValue.Text);
            }
        }
    }
}
