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
    /// CircleMeterControl.xaml の相互作用ロジック
    /// </summary>
    public partial class CircleMeterControl : UserControl
    {
        public CircleMeterControl()
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

                double angle = -135;
                if (value > 1)
                {
                    // double lvalue = System.Math.Log10(value);
                    //angle += lvalue * 27;
                    double lvalue = value;

                    angle += lvalue * 2.7;
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
