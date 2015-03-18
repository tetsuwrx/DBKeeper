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
using System.Windows.Shapes;
using DBKeeper.Classes.Common;
using System.Data;

namespace DBKeeper
{
    /// <summary>
    /// SessionDetail.xaml の相互作用ロジック
    /// </summary>
    public partial class QueryText : Window
    {
        public QueryText(string paramQueryText)
        {
            InitializeComponent();

            // クエリ詳細を表示
            QueryTextDetail.Text = paramQueryText;
        }

        /// <summary>
        /// 「閉じる」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // 画面を閉じる
            Close();
        }
    }
}
