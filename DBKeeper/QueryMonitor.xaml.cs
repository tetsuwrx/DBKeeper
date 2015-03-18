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
using System.Windows.Threading;

namespace DBKeeper
{
    /// <summary>
    /// QueryMonitor.xaml の相互作用ロジック
    /// </summary>
    public partial class QueryMonitor : Window
    {
        private AppSettings GlobalCommonSettings;

        private ServerSettings CommonServerSettings;

        // タイマー用クラス生成
        DispatcherTimer dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);

        // 実行カウント
        long CaptureExecutionCount = 0;

        /// <summary>
        /// 実行中のクエリをリストで表示
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

            CpuGraph.Title = "CPU使用率(%)";
            WaitSessionGraph.Title = "待機中のタスク";

            // 「開始」ボタンを押せるよにする
            CaptureStartButton.IsEnabled = true;

            // 「停止」ボタンを押せなくする
            CaptureStopButton.IsEnabled = false;

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

        }

        /// <summary>
        /// Timerイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 実行中のクエリを取得する
                ViewExecutingQueryList(CaptureExecutionCount,CommonServerSettings.InstanceName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 実行中のクエリを取得し、DataGridへ反映する
        /// </summary>
        private void ViewExecutingQueryList(long ExecutionCount, string InstanceName)
        {
            DBAccess dbAccess = new DBAccess();                                                         // データベースアクセス用共通クラス
            DataSet tmpDataSet = new DataSet();
            DataTable tmpDataTable = new DataTable();

            string getQueryListSQL = "";
            string errorMessage = "";
            string dataValue = "";

            double percentageValue = 0.0;

            getQueryListSQL = "select a.session_id ";
            getQueryListSQL += "     , a.wait_type ";
            getQueryListSQL += "     , a.wait_time ";
            getQueryListSQL += "	 , a.last_wait_type ";
            getQueryListSQL += "	 , a.wait_resource ";
            getQueryListSQL += "     , t.text as query_text ";
            getQueryListSQL += "  from sys.dm_exec_requests a ";
            getQueryListSQL += "inner join sys.dm_exec_query_stats b ";
            getQueryListSQL += "   on a.sql_handle = b.sql_handle ";
            getQueryListSQL += "cross apply sys.dm_exec_sql_text(a.sql_handle) t ";
            getQueryListSQL += "where a.session_id > 50;";
            getQueryListSQL += "select perfCount.object_name" + "\n";
            getQueryListSQL += "     , perfCount.counter_name" + "\n";
            getQueryListSQL += "     , perfCount.instance_name" + "\n";
            getQueryListSQL += "     , case when perfBase.cntr_value = 0 then 0" + "\n";
            getQueryListSQL += "       else ( cast ( perfCount.cntr_value as float ) / perfBase.cntr_value ) * 100";
            getQueryListSQL += "       end as cntr_Value" + "\n";
            getQueryListSQL += "  from ( " + "\n";
            getQueryListSQL += "    select top 1 * from sys.dm_os_performance_counters" + "\n";
            getQueryListSQL += "     where object_Name = '" + InstanceName + ":Resource Pool Stats'" + "\n";
            getQueryListSQL += "       and counter_name = 'CPU Usage %' ) perfCount" + "\n";
            getQueryListSQL += " inner join" + "\n";
            getQueryListSQL += " (" + "\n";
            getQueryListSQL += "    select * from sys.dm_os_performance_counters" + "\n";
            getQueryListSQL += "     where object_Name = '" + InstanceName + ":Resource Pool Stats'" + "\n";
            getQueryListSQL += "       and counter_name = 'CPU Usage % base' ) perfBase" + "\n";
            getQueryListSQL += "    on perfCount.Object_name = perfBase.Object_name" + "\n";
            getQueryListSQL += "   and perfCount.instance_name = perfBase.instance_name" + "\n";
            getQueryListSQL += " where perfCount.instance_name = 'default';";
            getQueryListSQL += "select count(1) as cnt" + "\n";
            getQueryListSQL += "  from sys.dm_os_waiting_tasks" + "\n";
            getQueryListSQL += " where session_id > 50;";

            // SQL実行
            tmpDataSet = dbAccess.GetDataSet(getQueryListSQL, CommonServerSettings.ConnectionString, ref errorMessage);
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage);
                return;
            }

            tmpDataTable = tmpDataSet.Tables[0];

            /* 
             * 実行中のクエリを表示
             * */
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

            /*
             * CPU使用率を取得
             * */
            tmpDataTable = tmpDataSet.Tables[1];

            dataValue = tmpDataTable.Rows[0]["cntr_Value"].ToString();

            double.TryParse(dataValue, out percentageValue);

            // グラフに設定
            CpuGraph.PercentageValue = percentageValue;

            /*
             * 待機中のタスク数を取得
             * */
            tmpDataTable = tmpDataSet.Tables[2];

            dataValue = tmpDataTable.Rows[0]["cnt"].ToString();

            double.TryParse(dataValue, out percentageValue);

            // グラフに設定
            WaitSessionGraph.PercentageValue = percentageValue;

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

        /// <summary>
        /// 「開始」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CaptureStartButton_Click(object sender, RoutedEventArgs e)
        {
            // 「開始」ボタンを押せなくする
            CaptureStartButton.IsEnabled = false;

            // 「停止」ボタンを押せるようにする
            CaptureStopButton.IsEnabled = true;

            int timerInterval = 0;

            if (Per1Sec.IsChecked == true)
            {
                timerInterval = 1;
            }
            else if (Per3Sec.IsChecked == true)
            {
                timerInterval = 3;
            }
            else if (Per5Sec.IsChecked == true)
            {
                timerInterval = 5;
            }
            else if (Per10Sec.IsChecked == true)
            {
                timerInterval = 10;
            }

            // 実行回数のクリア
            CaptureExecutionCount = 0;

            // タイマー用クラス設定
            dispatcherTimer.Interval = new TimeSpan(0, 0, timerInterval);
            dispatcherTimer.Start();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CaptureStopButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            
            CaptureStartButton.IsEnabled = true;
            CaptureStopButton.IsEnabled = false;
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
}
