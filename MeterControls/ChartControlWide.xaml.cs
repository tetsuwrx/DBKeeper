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
using System.Collections;

namespace MeterControls
{
    /// <summary>
    /// ChartControlWide.xaml の相互作用ロジック
    /// </summary>
    public partial class ChartControlWide : UserControl
    {
        private const int MAX_HISTORY_COUNT = 61;                               // 履歴の保持件数
        private const int GRAPH_HEIGHT = 60;                                    // グラフの高さ

        //private double[] PercentageHistory = new double[MAX_HISTORY_COUNT];     // CPU使用率の履歴を配列で取得
        private ArrayList PercentageHistory = new ArrayList();                  // CPU使用率の履歴を配列で取得

        private double[] GraphXList = new double[MAX_HISTORY_COUNT] { 449, 441, 434, 426, 419, 411, 404, 396, 389, 381, 374, 366, 359, 351, 344, 336, 329, 321, 314, 306, 299, 291, 284, 276, 269, 261, 254, 246, 239, 231, 224, 216, 209, 201, 194, 187, 180, 172, 165, 157, 150, 142, 135, 127, 120, 112, 105, 97, 90, 82, 75, 67, 60, 52, 45, 37, 30, 22, 15, 7, 0 };     // グラフの横軸の位置リスト

        private int HISTORY_COUNT = 0;                                          // 履歴取得件数

        private double currentValue = 0;                                        // 現在の値

        public ChartControlWide()
        {
            InitializeComponent();

            // 履歴の初期化
            for (int i = 0; i < MAX_HISTORY_COUNT; i++)
            {
                double defaultVal = 0.0;
                PercentageHistory.Add(defaultVal);
            }
        }

        public double PercentageValue
        {
            set
            {
                if (HISTORY_COUNT >= MAX_HISTORY_COUNT)
                {
                    // 履歴のカウントを初期化
                    HISTORY_COUNT = 0;

                    // 履歴の値を初期化
                    //PercentageHistory = new ArrayList();
                }

                // 現在の設定値を取得
                currentValue = value;

                // 履歴として保存
                // PercentageHistory[HISTORY_COUNT] = currentValue;
                // 最後の要素を削除
                PercentageHistory.RemoveAt(MAX_HISTORY_COUNT - 1);
                // 最新の値を挿入
                PercentageHistory.Insert(0, currentValue);

                // グラフの位置をプロット
                DisplayGraph.Points.Clear();

                /* ---------------------------------------------------- */
                // 最初の3点は固定値を設定
                Point pointVal0 = new Point();
                pointVal0.X = 0;
                pointVal0.Y = 8;

                DisplayGraph.Points.Add(pointVal0);

                pointVal0.X = 0;
                pointVal0.Y = 60;

                DisplayGraph.Points.Add(pointVal0);

                pointVal0.X = 981;
                pointVal0.Y = 60;

                DisplayGraph.Points.Add(pointVal0);

                for (int i = 0; i < MAX_HISTORY_COUNT; i++)
                {
                    int graphValue = 0;

                    // 渡されたパーセンテージから高さを計算
                    graphValue = GRAPH_HEIGHT - (int)(GRAPH_HEIGHT * ((double)PercentageHistory[i] / 100));

                    if (graphValue < 2)
                    {
                        graphValue = 2;
                    }

                    Point pointValTemp = new Point();

                    pointValTemp.X = GraphXList[i];
                    pointValTemp.Y = graphValue;

                    DisplayGraph.Points.Add(pointValTemp);

                }

                // 履歴保持のカウントアップ
                HISTORY_COUNT++;

            }

            get
            {
                return currentValue;
            }
        }

        /// <summary>
        /// タイトルを設定・取得する
        /// </summary>
        public string Title
        {
            set
            {
                TitleLabel.Text = value;
            }
            get
            {
                return TitleLabel.Text.ToString();
            }

        }
    }
}
