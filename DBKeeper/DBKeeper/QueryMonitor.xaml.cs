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
using System.Collections.ObjectModel;
using System.ComponentModel;

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

        private ArrayList BuffDataFileIoList;

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
                ViewExecutingQueryList();              // セッションリストの表示
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// GridViewのタイトル列クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryListViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            // 列ヘッダーを取得
            GridViewColumnHeader columnHeader = sender as GridViewColumnHeader;
            // ヘッダーのタグ名を取得
            string columnTagName = columnHeader.Tag.ToString();
            // ListViewのソート実行
            SortListView(ExecutingQueryListView, columnTagName, false);
        }

        /// <summary>
        /// GridViewのタイトル列クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataFileListViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            // 列ヘッダーを取得
            GridViewColumnHeader columnHeader = sender as GridViewColumnHeader;
            // ヘッダーのタグ名を取得
            string columnTagName = columnHeader.Tag.ToString();
            // ListViewのソート実行
            SortListView(DataFileIOListView, columnTagName, false);
        }

        /// <summary>
        /// 実行中のクエリと読み取りI/Oコストの高いクエリを取得し、DataGridへ反映する
        /// </summary>
        private void ViewExecutingQueryList()
        {
            DBAccess dbAccess = new DBAccess();                                                         // データベースアクセス用共通クラス
            DataSet tmpDataSet = new DataSet();
            DataTable tmpDataTable = new DataTable();

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

            CollectionViewSource view = new CollectionViewSource();
            ObservableCollection<QueryListRecord> queryListRecord = new ObservableCollection<QueryListRecord>();


            // レコード間ループ
            for (int i = 0; i < tmpDataTable.Rows.Count; i++)
            {
                queryListRecord.Add(new QueryListRecord()
                {
                    session_id = tmpDataTable.Rows[i]["session_id"].ToString(),
                    wait_type = tmpDataTable.Rows[i]["wait_type"].ToString(),
                    wait_time = tmpDataTable.Rows[i]["wait_time"].ToString(),
                    last_wait_type = tmpDataTable.Rows[i]["last_wait_type"].ToString(),
                    wait_resource = tmpDataTable.Rows[i]["wait_resource"].ToString(),
                    query_text = tmpDataTable.Rows[i]["query_text"].ToString()

                });
                
                // 新規の行をListViewへ反映
                view.Source = queryListRecord;
                ExecutingQueryListView.DataContext = view;
            }
        }

        /// <summary>
        /// データファイルのI/Oリストを取得
        /// </summary>
        private void ViewDataFileIOList()
        {
            DBAccess dbAccess = new DBAccess();                                                         // データベースアクセス用共通クラス
            DataSet tmpDataSet = new DataSet();
            DataTable tmpDataTable = new DataTable();

            string getQueryListSQL = "";
            string errorMessage = "";
            
            // 読み取りI/Oコストが高いクエリを取得
            getQueryListSQL = "declare @current_collection_time datetime;";
            getQueryListSQL += "set @current_collection_time = GETDATE();";
            getQueryListSQL += "select @current_collection_time as collection_time ";
            getQueryListSQL += "      , d.name AS [Database] ";
            getQueryListSQL += "      , f.physical_name as [File] ";
            getQueryListSQL += "      , (fs.num_of_bytes_read / 1024.0 / 1024.0) as [Total_MB_Read] ";
            getQueryListSQL += "      , (fs.num_of_bytes_written / 1024.0 / 1024.0) as [Total_MB_Written] ";
            getQueryListSQL += "      , (fs.num_of_reads + fs.num_of_writes) as [Total_IO_Count] ";
            getQueryListSQL += "      , fs.io_stall as [Total_IO_Wait_Time] ";
            getQueryListSQL += "  from sys.dm_io_virtual_file_stats(default, default) as fs ";
            getQueryListSQL += " inner join sys.master_files f on fs.database_id = f.database_id ";
            getQueryListSQL += "   and fs.file_id = f.file_id ";
            getQueryListSQL += " inner join sys.databases d on d.database_id = fs.database_id ";
            
            // SQL実行
            tmpDataSet = dbAccess.GetDataSet(getQueryListSQL, CommonServerSettings.ConnectionString, ref errorMessage);
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage);
                return;
            }

            tmpDataTable = tmpDataSet.Tables[0];

            CollectionViewSource view = new CollectionViewSource();
            ObservableCollection<DataFileIoListViewClass> datafileIoList = new ObservableCollection<DataFileIoListViewClass>();

            // レコード間ループ
            for (int i = 0; i < tmpDataTable.Rows.Count; i++)
            {
                double tmp_total_read_mb = 0;
                double tmp_total_write_mb = 0;
                double tmp_total_io_wait_time = 0;

                Double.TryParse(tmpDataTable.Rows[i]["Total_MB_Read"].ToString(),out tmp_total_read_mb);
                Double.TryParse(tmpDataTable.Rows[i]["Total_MB_Written"].ToString(),out tmp_total_write_mb);
                Double.TryParse(tmpDataTable.Rows[i]["Total_IO_Wait_Time"].ToString(),out tmp_total_io_wait_time);

                // 行を新規に生成
                datafileIoList.Add( new DataFileIoListViewClass()
                {
                    database_name = tmpDataTable.Rows[i]["Database"].ToString(),
                    file_name = tmpDataTable.Rows[i]["File"].ToString(),
                    total_read_mb = tmp_total_read_mb,
                    total_write_mb = tmp_total_write_mb,
                    total_io_wait_time = tmp_total_io_wait_time
                });
            }

            view.Source = datafileIoList;
            DataFileIOListView.DataContext = view;
        }

        /// <summary>
        /// 「Close」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
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
            // データ取得＆グリッド反映
            ViewExecutingQueryList();
            ViewDataFileIOList();
        }

        /// <summary>
        /// クエリのテキストを表示する
        /// </summary>
        /// <param name="paramSessionID">セッションID</param>
        /// <param name="queryText">クエリ詳細</param>
        private void ShowQueryText(string paramSessionID, string paramQueryText)
        {
            QueryText queryText = new QueryText(paramQueryText);
            
            queryText.Owner = this;
            queryText.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            queryText.Title = "【セッションID:" + paramSessionID + "】" + queryText.Title;

            queryText.Show();

        }

        /// <summary>
        /// 「実行中のクエリ」グリッドのダブルクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExecutingQueryList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string sessionId = "";                          // セッションID
            string queryText = "";                          // クエリテキスト
            ListView senderView = sender as ListView;
            var dataContext = senderView.Items[senderView.SelectedIndex] as DataRowView;

            // 選択行からセッションID、クエリテキストを取得
            sessionId = dataContext.Row[0].ToString();
            queryText = dataContext.Row[5].ToString();

            // 画面の表示
            ShowQueryText(sessionId, queryText);
        }

        private void DisplayDiskQueryIoListButton_Click(object sender, RoutedEventArgs e)
        {
            // データ取得＆グリッド反映
            ViewDataFileIOList();

        }

        private void DisplayExecutingQueryListButton_Click(object sender, RoutedEventArgs e)
        {
            // データ取得＆グリッド反映
            ViewExecutingQueryList();
        }

        /// <summary>
        /// ListViewのソート
        /// </summary>
        /// <param name="listView">対象のListView</param>
        /// <param name="headerTagName">ソートする列のタグ名</param>
        /// <param name="IsMultiSort">True:複合列でのソート、False:単数列でのソート</param>
        public void SortListView(ListView listView, string headerTagName, bool IsMultiSort)
        {
            // 対象のListViewが何もなければ処理終了
            if (listView.Items == null && listView.Items.Count == 0)
            {
                return;
            }

            // SortDescriptionの取得
            var r = listView.Items.SortDescriptions.Where(x => x.PropertyName == headerTagName);

            ListSortDirection sort;

            // 昇順・降順の設定
            if (r.Count() == 0)
            {
                // 新規に降順として作成
                sort = ListSortDirection.Descending;
            }
            else
            {
                // 既存のSortObjectがある場合は、前回実行結果と比較したうえで逆を設定
                sort = (r.First().Direction == ListSortDirection.Descending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
                listView.Items.SortDescriptions.Remove(r.First());
            }

            // 複合列ソートの判断
            if (IsMultiSort == false)
            {
                // ソート内容の詳細をクリア
                listView.Items.SortDescriptions.Clear();
            }

            listView.Items.SortDescriptions.Add(new SortDescription(headerTagName, sort));
        }
    }

    class QueryListRecord
    {
        public string session_id { get; set; }
        public string wait_type { get; set; }
        public string wait_time { get; set; }
        public string last_wait_type { get; set; }
        public string wait_resource { get; set; }
        public string query_text { get; set; }
    }

    class DataFileIoList
    {
        public string collection_time { get; set; }
        public string database_name { get; set; }
        public string file_name { get; set; }
        public double total_read_mb { get; set; }
        public double total_write_mb { get; set; }
        public string total_io_count { get; set; }
        public double total_io_wait_time { get; set; }
        public string file_size_mb { get; set; }
    }

    class DataFileIoListViewClass
    {
        public string database_name { get; set; }
        public string file_name { get; set; }
        public double total_read_mb { get; set; }
        public double total_write_mb { get; set; }
        public double total_io_wait_time { get; set; }
    }
}
