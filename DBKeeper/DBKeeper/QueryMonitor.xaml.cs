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
using System.Collections;

namespace DBKeeper
{
    /// <summary>
    /// QueryMonitor.xaml の相互作用ロジック
    /// </summary>
    public partial class QueryMonitor : Window
    {
        private AppSettings GlobalCommonSettings;

        private ServerSettings CommonServerSettings;

        //public string targetServer;

        //----------------------------------------------------------------------メンバ変数
        private DataTable m_table_query_list;                   // 実行中のクエリのリスト
        private DataTable m_table_high_cost_query_list;         // 読込I/Oコストの高いクエリのリスト

        /// <summary>
        /// 実行中のクエリ、読み込みI/Oコストの高いクエリをリストで表示
        /// </summary>
        public QueryMonitor(string targetServer)
        {
            InitializeComponent();

            // 共通クラスの生成と設定ファイル読み込み
            AppSettings commonCls = new AppSettings();
            GlobalCommonSettings = commonCls;

            switch (targetServer)
            {
                case "01":
                    CommonServerSettings = commonCls.server01Settings;
                    break;
                case "02":
                    CommonServerSettings = commonCls.server02Settings;
                    break;
                case "03":
                    CommonServerSettings = commonCls.server03Settings;
                    break;
                case "04":
                    CommonServerSettings = commonCls.server04Settings;
                    break;
                default:
                    CommonServerSettings = commonCls.server01Settings;
                    break;
            }

            // タイトルにホスト名をつける
            this.Title = "【ホスト名:" + CommonServerSettings.HostName + "】" + this.Title;

            try
            {
                InitTables();                   // テーブルの初期化

                ViewSessionList();              // セッションリストの表示
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // グリッドにバインド
            ExecutingQueryList.DataContext = m_table_query_list;
            LogicalReadHighCostQueryList.DataContext = m_table_high_cost_query_list;
        }

        /// <summary>
        /// セッションリストDataGridの初期化
        /// </summary>
        private void InitTables()
        {
            // 行列のサイズ指定
            ExecutingQueryList.Columns[0].MinWidth = 80;
            ExecutingQueryList.Columns[0].MaxWidth = 80;
            ExecutingQueryList.Columns[1].MinWidth = 80;
            ExecutingQueryList.Columns[1].MaxWidth = 80;
            ExecutingQueryList.Columns[2].MinWidth = 80;
            ExecutingQueryList.Columns[2].MaxWidth = 80;
            ExecutingQueryList.Columns[3].MinWidth = 80;
            ExecutingQueryList.Columns[3].MaxWidth = 80;
            ExecutingQueryList.Columns[4].MinWidth = 80;
            ExecutingQueryList.Columns[4].MaxWidth = 800;
            ExecutingQueryList.RowHeight = 20;

            LogicalReadHighCostQueryList.Columns[0].MinWidth = 800;
            LogicalReadHighCostQueryList.Columns[0].MaxWidth = 800;
            LogicalReadHighCostQueryList.Columns[1].MinWidth = 80;
            LogicalReadHighCostQueryList.Columns[1].MaxWidth = 80;
            LogicalReadHighCostQueryList.Columns[2].MinWidth = 80;
            LogicalReadHighCostQueryList.Columns[2].MaxWidth = 80;
            LogicalReadHighCostQueryList.Columns[3].MinWidth = 80;
            LogicalReadHighCostQueryList.Columns[3].MaxWidth = 80;
            LogicalReadHighCostQueryList.Columns[4].MinWidth = 80;
            LogicalReadHighCostQueryList.Columns[4].MaxWidth = 80;
            LogicalReadHighCostQueryList.Columns[5].MinWidth = 80;
            LogicalReadHighCostQueryList.Columns[5].MaxWidth = 80;
            LogicalReadHighCostQueryList.Columns[6].MinWidth = 80;
            LogicalReadHighCostQueryList.Columns[6].MaxWidth = 80;
            LogicalReadHighCostQueryList.Columns[7].MinWidth = 80;
            LogicalReadHighCostQueryList.Columns[7].MaxWidth = 80;
            LogicalReadHighCostQueryList.RowHeight = 20;

            // クエリのリストのテーブル生成
            m_table_query_list = new DataTable("query_list");
            m_table_query_list.Columns.Add(new DataColumn("session_id", typeof(int)));                // セッションID
            m_table_query_list.Columns.Add(new DataColumn("wait_type", typeof(string)));              // 待機種類
            m_table_query_list.Columns.Add(new DataColumn("wait_time", typeof(long)));                // 待機時間
            m_table_query_list.Columns.Add(new DataColumn("last_wait_type", typeof(string)));         // 最終待機種類
            m_table_query_list.Columns.Add(new DataColumn("wait_resource", typeof(string)));          // 待機リソース
            m_table_query_list.Columns.Add(new DataColumn("query_text", typeof(string)));             // 実行中のSQL

            // クエリのリストのテーブル生成
            m_table_high_cost_query_list = new DataTable("high_cost_i_o_list");
            m_table_high_cost_query_list.Columns.Add(new DataColumn("query_text", typeof(string)));             // クエリ
            m_table_high_cost_query_list.Columns.Add(new DataColumn("execution_count", typeof(long)));          // 実行回数
            m_table_high_cost_query_list.Columns.Add(new DataColumn("total_logical_reads", typeof(long)));      // 合計読込回数
            m_table_high_cost_query_list.Columns.Add(new DataColumn("last_logical_reads", typeof(long)));       // 最新読込回数
            m_table_high_cost_query_list.Columns.Add(new DataColumn("total_elapsed_time", typeof(long)));       // 合計経過時間
            m_table_high_cost_query_list.Columns.Add(new DataColumn("last_elapsed_time", typeof(long)));        // 最新経過時間
            m_table_high_cost_query_list.Columns.Add(new DataColumn("last_execution_time", typeof(string)));    // 最終実行時刻
            m_table_high_cost_query_list.Columns.Add(new DataColumn("query_plan", typeof(string)));             // 実行計画

        }

        /// <summary>
        /// 実行中のクエリと読み取りI/Oコストの高いクエリを取得し、DataGridへ反映する
        /// </summary>
        private void ViewSessionList()
        {
            DBAccess dbAccess = new DBAccess();                                                         // データベースアクセス用共通クラス
            DataSet tmpDataSet = new DataSet();
            DataTable tmpDataTable = new DataTable();

            DataSet tmpDataSet2 = new DataSet();
            DataTable tmpDataTable2 = new DataTable();

            string getQueryListSQL = "";
            string errorMessage = "";

            getQueryListSQL =  "select a.session_id ";
            getQueryListSQL += "     , a.wait_type ";
            getQueryListSQL += "     , a.wait_time ";
            getQueryListSQL += "	 , a.last_wait_type ";
            getQueryListSQL += "	 , a.wait_resource ";
            getQueryListSQL += "     , t.text as query_text ";
            getQueryListSQL += "  from sys.dm_exec_requests a ";
            getQueryListSQL += "inner join sys.dm_exec_query_stats b ";
            getQueryListSQL += "   on a.sql_handle = b.sql_handle ";
            getQueryListSQL += "cross apply sys.dm_exec_sql_text(a.sql_handle) t ";
            getQueryListSQL += "where a.session_id > 50";

            // SQL実行
            tmpDataSet = dbAccess.GetDataSet(getQueryListSQL, CommonServerSettings.ConnectionString, ref errorMessage);
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage);
                return;
            }

            tmpDataTable = tmpDataSet.Tables[0];

            // レコード間ループ
            for (int i = 0; i < tmpDataTable.Rows.Count; i++)
            {
                // 行を新規に生成
                DataRow newRow = m_table_query_list.NewRow();

                newRow["session_id"] = tmpDataTable.Rows[i]["session_id"];
                newRow["wait_type"] = tmpDataTable.Rows[i]["wait_type"];
                newRow["wait_time"] = tmpDataTable.Rows[i]["wait_time"];
                newRow["last_wait_type"] = tmpDataTable.Rows[i]["last_wait_type"];
                newRow["wait_resource"] = tmpDataTable.Rows[i]["wait_resource"];
                newRow["query_text"] = tmpDataTable.Rows[i]["query_text"];
                
                // 新規の行をデータグリッドへ反映
                m_table_query_list.Rows.Add(newRow);
            }

            // 読み取りI/Oコストが高いクエリを取得
            getQueryListSQL = "select TOP 20 qt.text as query_text";
            getQueryListSQL += "      , qs.execution_count ";
            getQueryListSQL += "      , qs.total_logical_reads, qs.last_logical_reads ";
            getQueryListSQL += "      , qs.min_logical_reads, qs.max_logical_reads ";
            getQueryListSQL += "      , qs.total_elapsed_time, qs.last_elapsed_time ";
            getQueryListSQL += "      , qs.min_elapsed_time, qs.max_elapsed_time ";
            getQueryListSQL += "      , qs.last_execution_time ";
            getQueryListSQL += "      , qp.query_plan ";
            getQueryListSQL += "  FROM sys.dm_exec_query_stats qs ";
            getQueryListSQL += " CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt ";
            getQueryListSQL += " CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) qp ";
            getQueryListSQL += " WHERE qt.encrypted=0 ";
            getQueryListSQL += " ORDER BY qs.total_logical_reads DESC ";

            // SQL実行
            tmpDataSet2 = dbAccess.GetDataSet(getQueryListSQL, CommonServerSettings.ConnectionString, ref errorMessage);
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage);
                return;
            }

            tmpDataTable2 = tmpDataSet2.Tables[0];

            // レコード間ループ
            for (int i = 0; i < tmpDataTable2.Rows.Count; i++)
            {
                // 行を新規に生成
                DataRow newRow = m_table_high_cost_query_list.NewRow();

                newRow["query_text"] = tmpDataTable2.Rows[i]["query_text"];
                newRow["execution_count"] = tmpDataTable2.Rows[i]["execution_count"];
                newRow["total_logical_reads"] = tmpDataTable2.Rows[i]["total_logical_reads"];
                newRow["last_logical_reads"] = tmpDataTable2.Rows[i]["last_logical_reads"];
                newRow["total_elapsed_time"] = tmpDataTable2.Rows[i]["total_elapsed_time"];
                newRow["last_elapsed_time"] = tmpDataTable2.Rows[i]["last_elapsed_time"];
                newRow["last_execution_time"] = tmpDataTable2.Rows[i]["last_execution_time"];
                newRow["query_plan"] = tmpDataTable2.Rows[i]["query_plan"];

                // 新規の行をデータグリッドへ反映
                m_table_high_cost_query_list.Rows.Add(newRow);
            }
        }

        /// <summary>
        /// 「Close」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // 各リストテーブルの内容をクリア
            m_table_query_list.Dispose();
            m_table_high_cost_query_list.Dispose();

            // 画面を閉じる
            Close();
        }

        /// <summary>
        /// 「Refresh」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshWindow();
        }

        /// <summary>
        /// 画面の更新
        /// </summary>
        private void RefreshWindow()
        {
            // 各リストテーブルの内容をクリア
            m_table_query_list.Clear();
            m_table_high_cost_query_list.Clear();

            // データ取得＆グリッド反映
            ViewSessionList();

            // データコンテキストの設定
            ExecutingQueryList.DataContext = m_table_query_list;
            LogicalReadHighCostQueryList.DataContext = m_table_high_cost_query_list;
        }
    }
}
