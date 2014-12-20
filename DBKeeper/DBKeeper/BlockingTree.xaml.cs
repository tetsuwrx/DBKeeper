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
    /// BlockingTree.xaml の相互作用ロジック
    /// </summary>
    public partial class BlockingTree : Window
    {
        private AppSettings GlobalCommonSettings;

        private ServerSettings CommonServerSettings;
        private string WINDOW_NO;

        DataTable tempSessionList = new DataTable();

        public BlockingTree(string targetServer)
        {
            InitializeComponent();

            // 共通クラスの生成と設定ファイル読み込み
            AppSettings commonCls = new AppSettings();
            GlobalCommonSettings = commonCls;

            WINDOW_NO = targetServer;

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

            // セッションのリストをメモリ上に保存
            tempSessionList = GetSessionList(CommonServerSettings.ConnectionString);

            MakeBlockingTreeList();

        }

        /// <summary>
        /// TreeViewの作成
        /// </summary>
        private void MakeBlockingTreeList()
        {
            DataTable tempDataTable = new DataTable();
            
            tempDataTable = GetBlockingList(CommonServerSettings.ConnectionString);

            for (int i = 0; i < tempDataTable.Rows.Count; i++)
            {
                string sessionId1 = "";
                string sessionId2 = "";
                string sessionId3 = "";
                string sessionId4 = "";
                string sessionId5 = "";
                TreeViewItem childItem01 = new TreeViewItem();
                TreeViewItem childItem02 = new TreeViewItem();
                TreeViewItem childItem03 = new TreeViewItem();
                TreeViewItem childItem04 = new TreeViewItem();
                TreeViewItem childItem05 = new TreeViewItem();
                /* ===================================== */
                // 第１階層～第５階層分のセッションIDを取得
                /* ===================================== */
                try
                {
                    sessionId1 = tempDataTable.Rows[i][0].ToString();
                    sessionId2 = tempDataTable.Rows[i][1].ToString();
                    sessionId3 = tempDataTable.Rows[i][2].ToString();
                    sessionId4 = tempDataTable.Rows[i][3].ToString();
                    sessionId5 = tempDataTable.Rows[i][4].ToString();
                }
                catch
                {
                    // 何もしない
                }

                /* ===================================== */
                // 第１階層
                /* ===================================== */
                childItem01.Header = sessionId1;

                /* ===================================== */
                // 第２階層
                /* ===================================== */
                childItem02.Header = sessionId2;

                /* ===================================== */
                // 第３階層
                /* ===================================== */
                childItem03.Header = sessionId3;

                /* ===================================== */
                // 第４階層
                /* ===================================== */
                childItem04.Header = sessionId4;

                /* ===================================== */
                // 第５階層
                /* ===================================== */
                childItem05.Header = sessionId5;

                if (sessionId5 != "")
                {
                    childItem04.Items.Add(childItem05);
                    childItem04.IsExpanded = true;
                }

                if (sessionId4 != "")
                {
                    childItem03.Items.Add(childItem04);
                    childItem03.IsExpanded = true;
                }

                if (sessionId3 != "")
                {
                    childItem02.Items.Add(childItem03);
                    childItem02.IsExpanded = true;
                }

                if ( sessionId2 != "" )
                {
                    childItem01.Items.Add(childItem02);
                    childItem01.IsExpanded = true;
                }

                // TreeViewに追加
                TreeViewBlocking.Items.Add(childItem01);
            }
        }

        /// <summary>
        /// セッションリストを取得する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <returns>セッションリストが入ったDataTable</returns>
        private DataTable GetSessionList(string connectionString)
        {
            DBAccess dbAccess = new DBAccess();                                                         // データベースアクセス用共通クラス
            DataSet tmpDataSet = new DataSet();
            DataTable tmpDataTable = new DataTable();

            string getSessionSQL = "";
            string errorMessage = "";

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
            getSessionSQL += " order by s.session_id";

            // SQL実行
            tmpDataSet = dbAccess.GetDataSet(getSessionSQL, CommonServerSettings.ConnectionString, ref errorMessage);
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage);
                return tmpDataTable;
            }

            tmpDataTable = tmpDataSet.Tables[0];

            return tmpDataTable;
        }

        /// <summary>
        /// ブロッキングリストを取得する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <returns>セッションリストが入ったDataTable</returns>
        private DataTable GetBlockingList(string connectionString)
        {
            DBAccess dbAccess = new DBAccess();                                                         // データベースアクセス用共通クラス
            DataSet tmpDataSet = new DataSet();
            DataTable tmpDataTable = new DataTable();

            string getSessionSQL = "";
            string errorMessage = "";

            // SQL設定
            getSessionSQL = "with parent_ids as ( ";
            getSessionSQL += "select blocking_session_id ";
            getSessionSQL += "     , session_id ";
            getSessionSQL += "	 , wait_duration_ms ";
            getSessionSQL += "  from sys.dm_os_waiting_tasks ";
            getSessionSQL += " where session_id > 50 ";
            getSessionSQL += "   and blocking_session_id is not null ";
            getSessionSQL += "), no2_levels as ( ";
            getSessionSQL += "select blocking_session_id ";
            getSessionSQL += "     , session_id ";
            getSessionSQL += "  from sys.dm_os_waiting_tasks ";
            getSessionSQL += " where session_id > 50 ";
            getSessionSQL += "   and blocking_session_id is not null ";
            getSessionSQL += "), no3_levels as ( ";
            getSessionSQL += "select blocking_session_id ";
            getSessionSQL += "     , session_id ";
            getSessionSQL += "  from sys.dm_os_waiting_tasks ";
            getSessionSQL += " where session_id > 50 ";
            getSessionSQL += "   and blocking_session_id is not null ";
            getSessionSQL += "), no4_levels as ( ";
            getSessionSQL += "select blocking_session_id ";
            getSessionSQL += "     , session_id ";
            getSessionSQL += "  from sys.dm_os_waiting_tasks ";
            getSessionSQL += " where session_id > 50 ";
            getSessionSQL += "   and blocking_session_id is not null ";
            getSessionSQL += ") ";
            getSessionSQL += "select no1.blocking_session_id as no1_session_id ";
            getSessionSQL += "     , no1.session_id as no2_session_id ";
            getSessionSQL += "	 , no2.session_id as no3_session_id ";
            getSessionSQL += "	 , no3.session_id as no4_session_id ";
            getSessionSQL += "	 , no4.session_id as no5_session_id ";
            getSessionSQL += "  from parent_ids as no1 ";
            getSessionSQL += "  left join no2_levels as no2 ";
            getSessionSQL += "    on no1.session_id = no2.blocking_session_id ";
            getSessionSQL += "  left join no3_levels as no3 ";
            getSessionSQL += "    on no2.session_id = no3.blocking_session_id ";
            getSessionSQL += "  left join no4_levels as no4 ";
            getSessionSQL += "    on no3.session_id = no4.blocking_session_id ";
            getSessionSQL += " order by no1.wait_duration_ms desc ";
            getSessionSQL += "; ";

            // SQL実行
            tmpDataSet = dbAccess.GetDataSet(getSessionSQL, CommonServerSettings.ConnectionString, ref errorMessage);
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage);
                return tmpDataTable;
            }

            tmpDataTable = tmpDataSet.Tables[0];

            return tmpDataTable;
        }

        /// <summary>
        /// SQL情報を取得する
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        /// <param name="sessionId">セッションID</param>
        /// <returns>SQLテキスト</returns>
        private string GetSqlText(string connectionString, string sessionId)
        {
            DBAccess dbAccess = new DBAccess();                                                         // データベースアクセス用共通クラス
            DataSet tmpDataSet = new DataSet();
            DataTable tmpDataTable = new DataTable();

            string getSessionSQL = "";
            string errorMessage = "";
            string returnValue = "";

            // SQL設定
            getSessionSQL += "select b.text as sql_text ";
            getSessionSQL += "  from sys.dm_exec_requests a ";
            getSessionSQL += " cross apply sys.dm_exec_sql_text( a.sql_handle ) b ";
            getSessionSQL += " where a.session_id = " + sessionId + " ";
            getSessionSQL += "union all ";
            getSessionSQL += "select b.text as sql_text ";
            getSessionSQL += "  from sys.dm_exec_connections a ";
            getSessionSQL += " cross apply sys.dm_exec_sql_text( a.most_recent_sql_handle )b ";
            getSessionSQL += " where a.session_id = " + sessionId + " ";

            // SQL実行
            tmpDataSet = dbAccess.GetDataSet(getSessionSQL, CommonServerSettings.ConnectionString, ref errorMessage);
            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage);
                return returnValue;
            }

            tmpDataTable = tmpDataSet.Tables[0];

            returnValue = tmpDataTable.Rows[0][0].ToString();

            return returnValue;
        }

        /// <summary>
        /// 「Close」ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 画面を閉じる
            Close();
        }

        /// <summary>
        /// 選択項目クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // TreeViewの選択された項目
            TreeViewItem selectedItem = (TreeViewItem)TreeViewBlocking.SelectedItem;

            if (selectedItem == null)
            {
                return;
            }

            // 選択されているセッションIDを取得
            string sessionId = selectedItem.Header.ToString();

            string login_id = "";                                       // ログインID
            string db_name = "";                                        // データベース名
            string task_state = "";                                     // タスクの状態
            string application_name = "";                               // プログラム名
            string sql_command = "";                                    // 実行中のSQL
            string session_Info = "";                                    // セッション情報

            // メモリ上に格納してあるセッションのリストから詳細情報を取得
            for (int i = 0; i < tempSessionList.Rows.Count; i++)
            {
                // 該当のセッションIDから必要な情報を取得
                if (sessionId == tempSessionList.Rows[i]["session_id"].ToString())
                {
                    login_id = tempSessionList.Rows[i]["login_id"].ToString();
                    db_name = tempSessionList.Rows[i]["db_name"].ToString();
                    task_state = tempSessionList.Rows[i]["task_state"].ToString();
                    application_name = tempSessionList.Rows[i]["application_name"].ToString();
                    sql_command = GetSqlText(CommonServerSettings.ConnectionString, sessionId);

                    break;
                }
            }

            session_Info = "セッションID:" + sessionId + "\n";
            session_Info += "--------------------------------------------------------------" + "\n";
            session_Info += "ログインID:" + login_id + "\n";
            session_Info += "--------------------------------------------------------------" + "\n";
            session_Info += "タスクの状態:" + task_state + "\n";
            session_Info += "--------------------------------------------------------------" + "\n";
            session_Info += "プログラム名:" + application_name + "\n";
            session_Info += "--------------------------------------------------------------" + "\n";
            session_Info += "実行中のSQL:" + "\n";
            session_Info += "--------------------------------------------------------------" + "\n"; 
            session_Info += sql_command;

            SessionInfo.Text = session_Info;

            
        }

        /// <summary>
        /// セッションの強制終了確認
        /// </summary>
        /// <param name="sessionId">セッションID</param>
        private void ConfirmKillSession(string sessionId)
        {
            string confirmMessage = "セッションID: " + sessionId + " を強制終了します。よろしいですか？";

            // 確認メッセージの表示
            MessageBoxResult mResult = MessageBox.Show(confirmMessage, "確認", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (mResult == MessageBoxResult.Yes)
            {
                // セッションの強制終了処理呼び出し
                KillSession(sessionId);

                // TreeViewのクリア
                TreeViewBlocking.BeginInit();
                TreeViewBlocking.Items.Clear();
                TreeViewBlocking.EndInit();

                // セッションのリストをメモリ上に保存
                tempSessionList = new DataTable();
                tempSessionList = GetSessionList(CommonServerSettings.ConnectionString);

                // TreeView再作成
                MakeBlockingTreeList();

                
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
            else
            {
                MessageBox.Show("セッションID[" + paramSessionId + "]の強制終了が完了しました", "お知らせ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// TreeView右クリックメニュー選択イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            // セッションID
            string sessionId = "";

            try
            {
                // 選択された項目
                TreeViewItem selectedItem = (TreeViewItem)TreeViewBlocking.SelectedItem;

                // セッションID
                sessionId = selectedItem.Header.ToString();
            }
            catch
            {
                // 何もしない
            }

            // 未選択状態の場合は処理を終了する
            if (sessionId == "")
            {
                return;
            }

            // 確認画面呼び出し
            ConfirmKillSession(sessionId);
        }

        private void ViewSessionListButton_Click(object sender, RoutedEventArgs e)
        {
            SessionList blockingList = new SessionList(WINDOW_NO);

            blockingList.Show();
        }
    }
}
