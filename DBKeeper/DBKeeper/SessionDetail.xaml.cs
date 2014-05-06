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
    public partial class SessionDetail : Window
    {
        private string CommonSessionId;                          // セッションID 
        private ServerSettings CommonServerSettings;             // サーバーの設定情報

        public SessionDetail(string paramSessionId, ServerSettings paramServerSettings)
        {
            InitializeComponent();

            CommonSessionId = paramSessionId;
            CommonServerSettings = paramServerSettings;

            if (CommonSessionId == null)
            {
                MessageBox.Show("不正なパラメータです。", "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            if ( CommonServerSettings == null)
            {
                MessageBox.Show("不正なパラメータです。", "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            // セッションの詳細を表示
            ViewSessionDetail(CommonSessionId);
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

        /// <summary>
        /// セッションの詳細を表示
        /// </summary>
        private void ViewSessionDetail(string paramSessionId)
        {
            DBAccess dbAccess = new DBAccess();                 // DB接続用共通クラス

            string getSessionSQL = "DBCC INPUTBUFFER(" + paramSessionId + ");";
            string refErrorMessage = "";
            string sessionDetail = "";

            // セッション情報を取得する
            DataSet tmpDataSet = dbAccess.GetDataSet(getSessionSQL, CommonServerSettings.ConnectionString, ref refErrorMessage);

            if (refErrorMessage != "")
            {
                MessageBox.Show(refErrorMessage, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            DataTable tmpDataTable = tmpDataSet.Tables[0];

            sessionDetail = tmpDataTable.Rows[0]["EventInfo"].ToString();

            // 画面に表示
            SessionDetailText.Text = sessionDetail;
        }

        /// <summary>
        /// 「強制終了」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KillSessionButton_Click(object sender, RoutedEventArgs e)
        {
            
            string confirmMessage = "セッションID: " + CommonSessionId + " を強制終了します。よろしいですか？";
            
            // 確認メッセージの表示
            MessageBoxResult mResult = MessageBox.Show(confirmMessage, "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (mResult == MessageBoxResult.Yes)
            {
                // セッションの強制終了処理呼び出し
                KillSession(CommonSessionId);

                // 画面を閉じる
                Close();
            }

        }

        /// <summary>
        /// セッションの強制終了
        /// </summary>
        /// <param name="paramSessionId">セッションID</param>
        private void KillSession(string paramSessionId)
        {
            DBAccess dbAccess = new DBAccess();                                 // データベースアクセス用共通クラス

            string killCommand = "Kill " + paramSessionId + ";";               // コマンド用SQL
            string errorMessage = "";

            // 実行
            errorMessage = dbAccess.ExecuteCommand(killCommand, CommonServerSettings.ConnectionString);

            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
