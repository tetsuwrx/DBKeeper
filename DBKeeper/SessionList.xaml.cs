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
    /// BlockingList.xaml の相互作用ロジック
    /// </summary>
    public partial class SessionList : Window
    {
        private AppSettings GlobalCommonSettings;

        private ServerSettings CommonServerSettings;

        //public string targetServer;

        //----------------------------------------------------------------------メンバ変数
        private DataTable m_table_session_list;                   // セッションのリスト

        /// <summary>
        /// ブロッキング情報一覧画面
        /// </summary>
        public SessionList(string targetServer)
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
                // InitTables();                   // テーブルの初期化

                ViewSessionList();              // セッションリストの表示
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// セッションリストDataGridの初期化
        /// </summary>
        private void InitTables()
        {
            
            // セッションのリストのテーブル生成
            m_table_session_list = new DataTable("session_list");
            m_table_session_list.Columns.Add(new DataColumn("session_id", typeof(int)));                // セッションID
            m_table_session_list.Columns.Add(new DataColumn("is_user_process", typeof(int)));           // ユーザープロセスフラグ(0:システムプロセス、1:ユーザープロセス)
            m_table_session_list.Columns.Add(new DataColumn("login_id", typeof(string)));               // ログインID
            m_table_session_list.Columns.Add(new DataColumn("db_name", typeof(string)));                // データベース名
            m_table_session_list.Columns.Add(new DataColumn("task_state", typeof(string)));             // タスクの状態
            m_table_session_list.Columns.Add(new DataColumn("sql_command", typeof(string)));            // 実行中のSQL
            m_table_session_list.Columns.Add(new DataColumn("application_name", typeof(string)));       // アプリケーション名
            m_table_session_list.Columns.Add(new DataColumn("wait_time_ms", typeof(long)));             // 待機時間(ms)
            m_table_session_list.Columns.Add(new DataColumn("wait_type", typeof(string)));              // 待機種類
            m_table_session_list.Columns.Add(new DataColumn("wait_resource", typeof(string)));          // 待機リソース
            m_table_session_list.Columns.Add(new DataColumn("blocking_from", typeof(string)));          // 待機元
            m_table_session_list.Columns.Add(new DataColumn("top_block", typeof(string)));              // 先頭ブロック
            m_table_session_list.Columns.Add(new DataColumn("memory_usage", typeof(int)));              // メモリ使用量(KB)
            m_table_session_list.Columns.Add(new DataColumn("host_name", typeof(string)));              // ホスト名
            m_table_session_list.Columns.Add(new DataColumn("client_net_address", typeof(string)));              // ホスト名
            m_table_session_list.Columns.Add(new DataColumn("workload_group", typeof(string)));         // ワークロードグループ
            
        }

        /// <summary>
        /// セッションのリストを動的管理ビューを使って取得し、DataGridへ反映する
        /// </summary>
        private void ViewSessionList()
        {
            DBAccess dbAccess = new DBAccess();                                                         // データベースアクセス用共通クラス
            DataSet tmpDataSet = new DataSet();
            DataTable tmpDataTable = new DataTable();

            string getSessionSQL = "";
            string errorMessage = "";

            ArrayList sessionIdList = new ArrayList();

            int i;

            // SQL設定
            getSessionSQL = "select s.session_id as session_id ";
            getSessionSQL += "     , convert(char(1), s.is_user_process) as is_user_process";
            getSessionSQL += "     , s.login_name as login_id";
            getSessionSQL += "     , case when p.dbid = 0 then '' else isnull(db_name(p.dbid),'') end as db_name";
            getSessionSQL += "     , isnull(t.task_state, '') as task_state";
            getSessionSQL += "     , isnull(r.command, '') as sql_command";
            getSessionSQL += "     , isnull(s.program_name, '') as application_name";
            getSessionSQL += "     , isnull(w.wait_duration_ms, 0) as wait_time_ms";
            getSessionSQL += "     , isnull(w.wait_type, '') as wait_type";
            getSessionSQL += "     , isnull(w.resource_description, '') as wait_resource";
            getSessionSQL += "     , isnull( convert(varchar, w.blocking_session_id), '') as blocking_from";
            getSessionSQL += "     , case when r2.session_id is not null and (r.blocking_session_id = 0 or r.session_id is null) then '1' else '' end as top_block";
            getSessionSQL += "     , s.memory_usage * 8192 / 1024 as memory_usage";
            getSessionSQL += "     , isnull(s.host_name, '') as host_name";
            getSessionSQL += "     , isnull(c.client_net_address, '') as client_net_address";
            getSessionSQL += "     , isnull(g.name, '') as workload_group";
            getSessionSQL += "  from sys.dm_exec_sessions s";
            getSessionSQL += "  left outer join sys.dm_exec_connections c";
            getSessionSQL += "    on s.session_id = c.session_id";
            getSessionSQL += "  left outer join sys.dm_exec_requests r ";
            getSessionSQL += "    on s.session_id = r.session_id";
            getSessionSQL += "  left outer join sys.dm_os_tasks t ";
            getSessionSQL += "    on r.session_id = t.session_id and r.request_id = t.request_id";
            getSessionSQL += "  left outer join";
            getSessionSQL += "     ( ";
            getSessionSQL += "         select *, row_number() over (partition by waiting_task_address order by wait_duration_ms desc) as row_num";
            getSessionSQL += "           from sys.dm_os_waiting_tasks";
            getSessionSQL += "     ) w";
            getSessionSQL += "    on t.task_address = w.waiting_task_address and w.row_num = 1";
            getSessionSQL += "  left outer join sys.dm_exec_requests r2";
            getSessionSQL += "    on s.session_id = r2.blocking_session_id";
            getSessionSQL += "  left outer join sys.dm_resource_governor_workload_groups g ";
            getSessionSQL += "    on g.group_id = s.group_id";
            getSessionSQL += "  left outer join sys.sysprocesses p";
            getSessionSQL += "    on s.session_id = p.spid";
            getSessionSQL += " where s.session_id > 50";
            getSessionSQL += "   and isnull( convert(varchar, w.blocking_session_id), '') <> s.session_id";
            getSessionSQL += " order by s.session_id";

            // SQL実行
            tmpDataSet = dbAccess.GetDataSet(getSessionSQL, CommonServerSettings.ConnectionString, ref errorMessage);
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage);
                return;
            }

            tmpDataTable = tmpDataSet.Tables[0];

            CollectionViewSource view = new CollectionViewSource();
            ObservableCollection<SessionListRecord> sessionListRecord = new ObservableCollection<SessionListRecord>();

            // レコード間ループ
            for (i = 0; i < tmpDataTable.Rows.Count; i++)
            {
                // 行を新規に生成
                sessionListRecord.Add(new SessionListRecord()
                {
                    session_id = tmpDataTable.Rows[i]["session_id"].ToString(),
                    is_user_process = tmpDataTable.Rows[i]["is_user_process"].ToString(),
                    login_id = tmpDataTable.Rows[i]["login_id"].ToString(),
                    db_name = tmpDataTable.Rows[i]["db_name"].ToString(),
                    task_state = tmpDataTable.Rows[i]["task_state"].ToString(),
                    sql_command = tmpDataTable.Rows[i]["sql_command"].ToString(),
                    application_name = tmpDataTable.Rows[i]["application_name"].ToString(),
                    wait_time_ms = tmpDataTable.Rows[i]["wait_time_ms"].ToString(),
                    wait_type = tmpDataTable.Rows[i]["wait_type"].ToString(),
                    wait_resource = tmpDataTable.Rows[i]["wait_resource"].ToString(),
                    blocking_from = tmpDataTable.Rows[i]["blocking_from"].ToString(),
                    top_block = tmpDataTable.Rows[i]["top_block"].ToString(),
                    memory_usage = tmpDataTable.Rows[i]["memory_usage"].ToString(),
                    host_name = tmpDataTable.Rows[i]["host_name"].ToString(),
                    client_net_address = tmpDataTable.Rows[i]["client_net_address"].ToString(),
                    workload_group = tmpDataTable.Rows[i]["workload_group"].ToString()
                });

                if ( tmpDataTable.Rows[i]["blocking_from"].ToString() != "" )
                {
                    sessionIdList.Add(tmpDataTable.Rows[i]["blocking_from"].ToString());
                }
                
                view.Source = sessionListRecord;
                BlockingListView.DataContext = view;
            }
        }

        /// <summary>
        /// 「Close」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 画面ロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// セッションの詳細画面呼び出し
        /// </summary>
        private void OpenViewSessionDetailForm()
        {
            string sessionId = "";

            // 選択項目があるかどうかを確認する
            if (BlockingListView.SelectedItems.Count == 0)
            {
                // 選択項目が無いので処理をせず抜ける
                return;
            }

            SessionListRecord record = (SessionListRecord)BlockingListView.SelectedItems[0];

            sessionId = record.session_id;

            SessionDetail sessionDetail = new SessionDetail(sessionId, CommonServerSettings);

            sessionDetail.Owner = this;
            sessionDetail.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            sessionDetail.Title = "【セッションID:" + sessionId + "】セッション詳細";

            // 画面オープン
            sessionDetail.ShowDialog();

            // 帰ってきたら再描画
            //RefleshWindow();
        }

        /// <summary>
        /// セッションの強制終了
        /// </summary>
        private void SessionKill()
        {
            string sessionId = "";
            
            // 選択項目があるかどうかを確認する
            if (BlockingListView.SelectedItems.Count == 0)
            {
                // 選択項目が無いので処理をせず抜ける
                return;
            }

            SessionListRecord record = (SessionListRecord)BlockingListView.SelectedItems[0];

            sessionId = record.session_id;

            string confirmMessage = "セッションID: " + sessionId + " を強制終了します。よろしいですか？";

            // 確認メッセージの表示
            MessageBoxResult mResult = MessageBox.Show(confirmMessage, "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (mResult == MessageBoxResult.Yes)
            {
                // セッションの強制終了処理呼び出し
                KillSession(sessionId);

                // 画面のリフレッシュ
                RefleshWindow();
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

        /// <summary>
        /// 「更新」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefleshButton_Click(object sender, RoutedEventArgs e)
        {
            // 画面の更新
            RefleshWindow();
        }

        /// <summary>
        /// セッションリストを再取得し画面に表示する
        /// </summary>
        private void RefleshWindow()
        {
            // セッションリストの再取得
            ViewSessionList();
        }

        /// <summary>
        /// 右クリックメニュー「詳細」クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewSessionDetail_Click(object sender, RoutedEventArgs e)
        {
            OpenViewSessionDetailForm();
        }

        /// <summary>
        /// 右クリックメニュー「強制終了」クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SessionKill_Click(object sender, RoutedEventArgs e)
        {
            SessionKill();
        }

        /// <summary>
        /// GridViewのタイトル列クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            // 列ヘッダーを取得
            GridViewColumnHeader columnHeader = sender as GridViewColumnHeader;
            // ヘッダーのタグ名を取得
            string columnTagName = columnHeader.Tag.ToString();
            // ListViewのソート実行
            SortListView(BlockingListView, columnTagName, false);
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
                sort = ( r.First().Direction == ListSortDirection.Descending ) ? ListSortDirection.Ascending : ListSortDirection.Descending;
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

    class SessionListRecord
    {
        public string session_id { get; set; }
        public string is_user_process { get; set; }
        public string login_id { get; set; }
        public string db_name { get; set; }
        public string task_state { get; set; }
        public string sql_command { get; set; }
        public string application_name { get; set; }
        public string wait_time_ms { get; set; }
        public string wait_type { get; set; }
        public string wait_resource { get; set; }
        public string blocking_from { get; set; }
        public string top_block { get; set; }
        public string memory_usage { get; set; }
        public string host_name { get; set; }
        public string client_net_address { get; set; }
        public string workload_group { get; set; }
    }
}
