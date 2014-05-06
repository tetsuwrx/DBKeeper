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
    /// BlockingList.xaml の相互作用ロジック
    /// </summary>
    public partial class BlockingList : Window
    {
        private AppSettings GlobalCommonSettings;

        private ServerSettings CommonServerSettings;

        //public string targetServer;

        //----------------------------------------------------------------------メンバ変数
        private DataTable m_table_session_list;                   // セッションのリスト

        /// <summary>
        /// ブロッキング情報一覧画面
        /// </summary>
        public BlockingList(string targetServer)
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
            BlockingListGrid.DataContext = m_table_session_list;
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
            m_table_session_list.Columns.Add(new DataColumn("workload_group", typeof(string)));         // ワークロードグループ
            m_table_session_list.Columns.Add(new DataColumn("isblocked", typeof(bool)));                // ブロックされているか
            
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
            getSessionSQL += "     , isnull(convert (varchar, w.blocking_session_id), '') as blocking_from";
            getSessionSQL += "     , case when r2.session_id is not null and (r.blocking_session_id = 0 or r.session_id is null) then '1' else '' end as top_block";
            getSessionSQL += "     , s.memory_usage * 8192 / 1024 as memory_usage";
            getSessionSQL += "     , isnull(s.host_name, '') as host_name";
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
            getSessionSQL += " order by s.session_id";

            // SQL実行
            tmpDataSet = dbAccess.GetDataSet(getSessionSQL, CommonServerSettings.ConnectionString, ref errorMessage);
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage);
                return;
            }

            tmpDataTable = tmpDataSet.Tables[0];

            // レコード間ループ
            for (i = 0; i < tmpDataTable.Rows.Count; i++)
            {
                // 行を新規に生成
                DataRow newRow = m_table_session_list.NewRow();

                newRow["session_id"] = tmpDataTable.Rows[i]["session_id"];
                newRow["is_user_process"] = tmpDataTable.Rows[i]["is_user_process"];
                newRow["login_id"] = tmpDataTable.Rows[i]["login_id"];
                newRow["db_name"] = tmpDataTable.Rows[i]["db_name"];
                newRow["task_state"] = tmpDataTable.Rows[i]["task_state"];
                newRow["sql_command"] = tmpDataTable.Rows[i]["sql_command"];
                newRow["application_name"] = tmpDataTable.Rows[i]["application_name"];
                newRow["wait_time_ms"] = tmpDataTable.Rows[i]["wait_time_ms"];
                newRow["wait_type"] = tmpDataTable.Rows[i]["wait_type"];
                newRow["wait_resource"] = tmpDataTable.Rows[i]["wait_resource"];
                newRow["blocking_from"] = tmpDataTable.Rows[i]["blocking_from"];
                newRow["top_block"] = tmpDataTable.Rows[i]["top_block"];
                newRow["memory_usage"] = tmpDataTable.Rows[i]["memory_usage"];
                newRow["host_name"] = tmpDataTable.Rows[i]["host_name"];
                newRow["workload_group"] = tmpDataTable.Rows[i]["workload_group"];

                if ( tmpDataTable.Rows[i]["blocking_from"].ToString() != "" )
                {
                    sessionIdList.Add(tmpDataTable.Rows[i]["blocking_from"].ToString());
                }

                /*
                if (blockingSessionId > 0)
                {
                    newRow.SetField("isblocked", true);
                }
                else
                {
                    newRow.SetField("isblocked", false);
                }
                 * */
                
                // 新規の行をデータグリッドへ反映
                m_table_session_list.Rows.Add(newRow);
            }

            // セッションリスト間ループ
            for (i = 0; i < m_table_session_list.Rows.Count; i++)
            {
                for (int j = 0; j < sessionIdList.Count; j++)
                {
                    if (m_table_session_list.Rows[i]["session_id"].ToString() == sessionIdList[j].ToString())
                    {
                        m_table_session_list.Rows[i].SetField("isblocked", true);
                    }
                    else
                    {
                        m_table_session_list.Rows[i].SetField("isblocked", false);
                    }
                }
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

        private void Row_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // MouseRightButtonUpが発生したRowを選択状態にする
            var row = sender as DataGridRow;
            BlockingListGrid.SelectedIndex = row.GetIndex();
        }

        /// <summary>
        /// DataGrid行ロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlockingListGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow dataGridRow = e.Row;

            // マウス右クリックでカーソルがある行を選択状態にする
            e.Row.MouseRightButtonDown -= new MouseButtonEventHandler(Row_MouseRightButtonDown);
            e.Row.MouseRightButtonDown += new MouseButtonEventHandler(Row_MouseRightButtonDown);

            var dataContext = dataGridRow.Item as DataRowView;

            if (dataContext == null)
                return;

            string sessionId = dataContext.Row[0].ToString();
            
            // 右クリックメニューの生成
            ContextMenu cMenu = new ContextMenu();

            // 「詳細」メニュー項目
            MenuItem item = new MenuItem();
            item.Header = "詳細";
            item.Click += new System.Windows.RoutedEventHandler(delegate(object obj, System.Windows.RoutedEventArgs args) { OpenViewSessionDetailForm(sessionId); });
            cMenu.Items.Add(item);

            // 「強制終了」メニュー項目
            MenuItem item2 = new MenuItem();
            item2.Header = "強制終了";
            item2.Click += new System.Windows.RoutedEventHandler(delegate(object obj, System.Windows.RoutedEventArgs args) { SessionKill(sessionId); });
            cMenu.Items.Add(item2);

            // コンテキストに設定
            ContextMenuService.SetContextMenu(dataGridRow, cMenu);
        }

        /// <summary>
        /// セッションの詳細画面呼び出し
        /// </summary>
        /// <param name="sessionId">セッションID</param>
        private void OpenViewSessionDetailForm(string sessionId)
        {
            SessionDetail sessionDetail = new SessionDetail(sessionId, CommonServerSettings);

            sessionDetail.Owner = this;
            sessionDetail.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            sessionDetail.Title = "【セッションID:" + sessionId + "】セッション詳細";

            // 画面オープン
            sessionDetail.ShowDialog();

            // 帰ってきたら再描画
            RefleshWindow();
        }

        /// <summary>
        /// セッションの強制終了
        /// </summary>
        /// <param name="sessionId">セッションID</param>
        private void SessionKill(string sessionId)
        {
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
            // グリッドのクリア
            m_table_session_list.Rows.Clear();
            BlockingListGrid.DataContext = m_table_session_list;

            // セッションリストの再取得
            ViewSessionList();

            // データコンテキストの反映
            BlockingListGrid.DataContext = m_table_session_list;
        }

    }
}
