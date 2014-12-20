using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBKeeper.Classes.Common;
using System.Windows.Threading;
using System.Threading;
using System.Collections;
using System.Windows.Media.Animation;

namespace DBKeeper
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public DateTime Server01BeforeExecTIme = new DateTime();
        public DateTime Server02BeforeExecTIme = new DateTime();
        public DateTime Server03BeforeExecTIme = new DateTime();
        public DateTime Server04BeforeExecTIme = new DateTime();

        public AppSettings GlobalCommonSettings;

        public ServerSettings CommonServer01Settings;
        public ServerSettings CommonServer02Settings;
        public ServerSettings CommonServer03Settings;
        public ServerSettings CommonServer04Settings;

        System.Object sync01Object = new System.Object();
        System.Object sync02Object = new System.Object();
        System.Object sync03Object = new System.Object();
        System.Object sync04Object = new System.Object();

        bool monitor01Running = true;
        bool monitor02Running = true;
        bool monitor03Running = true;
        bool monitor04Running = true;

        ArrayList currentBlockingSidList01 = new ArrayList();                    // ブロッキングSIDリスト
        ArrayList currentBlockingSidList02 = new ArrayList();                    // ブロッキングSIDリスト
        ArrayList currentBlockingSidList03 = new ArrayList();                    // ブロッキングSIDリスト
        ArrayList currentBlockingSidList04 = new ArrayList();                    // ブロッキングSIDリスト

        Hashtable chkBlockingSidList01 = new Hashtable();                       // ブロッキングSIDリスト(チェック用)
        Hashtable chkBlockingSidList02 = new Hashtable();                       // ブロッキングSIDリスト(チェック用)
        Hashtable chkBlockingSidList03 = new Hashtable();                       // ブロッキングSIDリスト(チェック用)
        Hashtable chkBlockingSidList04 = new Hashtable();                       // ブロッキングSIDリスト(チェック用)

        Hashtable bufBlockingSidList01 = new Hashtable();                       // ブロッキングSIDリストの退避先
        Hashtable bufBlockingSidList02 = new Hashtable();                       // ブロッキングSIDリストの退避先
        Hashtable bufBlockingSidList03 = new Hashtable();                       // ブロッキングSIDリストの退避先
        Hashtable bufBlockingSidList04 = new Hashtable();                       // ブロッキングSIDリストの退避先

        
        public MainWindow()
        {
            InitializeComponent();

            // タイマー用クラスの生成
            DispatcherTimer dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
            
            // 共通クラスの生成と設定ファイル読み込み
            AppSettings commonCls = new AppSettings();
            GlobalCommonSettings = commonCls;

            CommonServer01Settings = commonCls.server01Settings;
            CommonServer02Settings = commonCls.server02Settings;
            CommonServer03Settings = commonCls.server03Settings;
            CommonServer04Settings = commonCls.server04Settings;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
            DateTime nowTime = DateTime.Now;

            CpuMeter01.MeterTitle = "CPU";
            CpuHistory01.Title = "CPU使用率(%)";
            MemoryMeter01.MeterTitle = "Memory";
            BufferCacheHitRate01.MeterTitle = "Buffer Cache HitRate";
            ProcedureCacheHitRate01.MeterTitle = "Proc Cache HitRate";
            Disk_I_O_Meter01.BarTitle = "Disk I/O";
            WaitHistory01.Title = "待機中のタスク";
            if (CommonServer01Settings.MonitoringStatus == "On")
            {
                Server1Title.Content = CommonServer01Settings.HostName;
                BeforeExecTime01.Content = nowTime.ToString();
            }
            else
            {
                Server1Title.Content = "N/A";
            }

            CpuMeter02.MeterTitle = "CPU";
            CpuHistory02.Title = "CPU使用率(%)";
            MemoryMeter02.MeterTitle = "Memory";
            BufferCacheHitRate02.MeterTitle = "Buffer Cache HitRate";
            ProcedureCacheHitRate02.MeterTitle = "Proc Cache HitRate";
            Disk_I_O_Meter02.BarTitle = "Disk I/O";
            WaitHistory02.Title = "待機中のタスク";
            if (CommonServer02Settings.MonitoringStatus == "On")
            {
                Server2Title.Content = CommonServer02Settings.HostName;
                BeforeExecTime02.Content = nowTime.ToString();
            }
            else
            {
                Server2Title.Content = "N/A";
            }

            CpuMeter03.MeterTitle = "CPU";
            CpuHistory03.Title = "CPU使用率(%)";
            MemoryMeter03.MeterTitle = "Memory";
            BufferCacheHitRate03.MeterTitle = "Buffer Cache HitRate";
            ProcedureCacheHitRate03.MeterTitle = "Proc Cache HitRate";
            Disk_I_O_Meter03.BarTitle = "Disk I/O";
            WaitHistory03.Title = "待機中のタスク";
            if (CommonServer03Settings.MonitoringStatus == "On")
            {
                Server3Title.Content = CommonServer03Settings.HostName;
                BeforeExecTime03.Content = nowTime.ToString();
            }
            else
            {
                Server3Title.Content = "N/A";
            }

            CpuMeter04.MeterTitle = "CPU";
            CpuHistory04.Title = "CPU使用率(%)";
            MemoryMeter04.MeterTitle = "Memory";
            BufferCacheHitRate04.MeterTitle = "Buffer Cache HitRate";
            ProcedureCacheHitRate04.MeterTitle = "Proc Cache HitRate";
            Disk_I_O_Meter04.BarTitle = "Disk I/O";
            WaitHistory04.Title = "待機中のタスク";
            if (CommonServer04Settings.MonitoringStatus == "On")
            {
                Server4Title.Content = CommonServer04Settings.HostName;
                BeforeExecTime04.Content = nowTime.ToString();
            }
            else
            {
                Server4Title.Content = "N/A";
            }

            /*
            Thread server01Thread = new Thread(new ThreadStart(Worker01Thread));
            Thread server02Thread = new Thread(new ThreadStart(Worker02Thread));
            Thread server03Thread = new Thread(new ThreadStart(Worker03Thread));
            Thread server04Thread = new Thread(new ThreadStart(Worker04Thread));

            server01Thread.Name = "server01Monitoring";
            server02Thread.Name = "server02Monitoring";
            server03Thread.Name = "server03Monitoring";
            server04Thread.Name = "server04Monitoring";

            server01Thread.Start();
            server02Thread.Start();
            server03Thread.Start();
            server04Thread.Start();
            */

        }
        
        /// <summary>
        /// Timerイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (CommonServer01Settings.MonitoringStatus == "On")
            {
                /*
                Thread server01Thread = new Thread(new ThreadStart(Worker01Thread));
                server01Thread.Name = "server01Monitoring";
                server01Thread.Start();
                // server01Thread.Join();
                */
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Worker01Thread));
            }

            if (CommonServer02Settings.MonitoringStatus == "On")
            {
                /*
                Thread server02Thread = new Thread(new ThreadStart(Worker02Thread));
                server02Thread.Name = "server02Monitoring";
                server02Thread.Start();
                // server02Thread.Join();
                */
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Worker02Thread));
            }

            if (CommonServer03Settings.MonitoringStatus == "On")
            {
                /*
                Thread server03Thread = new Thread(new ThreadStart(Worker03Thread));
                server03Thread.Name = "server03Monitoring";
                server03Thread.Start();
                // server03Thread.Join();
                */
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Worker03Thread));
            }

            if (CommonServer04Settings.MonitoringStatus == "On")
            {
                /*
                Thread server04Thread = new Thread(new ThreadStart(Worker04Thread));
                server04Thread.Name = "server04Monitoring";
                server04Thread.Start();
                // server04Thread.Join();
                */
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(Worker04Thread));
            }
            //Server01Monitoring();
            //Server02Monitoring();
            //Server03Monitoring();
            //Server04Monitoring();
        }

        private void Worker01Thread(object obj)
        {
            lock (sync01Object)
            {
                Dispatcher.BeginInvoke(new Action(Server01Monitoring), null);
            }
        }

        private void Worker02Thread(object obj)
        {
            lock (sync02Object)
            {
                Dispatcher.BeginInvoke(new Action(Server02Monitoring), null);
            }
        }

        private void Worker03Thread(object obj)
        {
            lock (sync03Object)
            {
                Dispatcher.BeginInvoke(new Action(Server03Monitoring), null);
            }
        }

        private void Worker04Thread(object obj)
        {
            lock (sync04Object)
            {
                Dispatcher.BeginInvoke(new Action(Server04Monitoring), null);
            }
        }

        /// <summary>
        /// Server01の監視
        /// </summary>
        /// <param name="timeSpanSecond">前回実行からの時間</param>
        private void Server01Monitoring()
        {
            double cpuUseage = 0;                       // CPU使用率
            double bufferCacheHitRate = 0;              // バッファキャッシュヒット率
            double procedureCacheHitRate = 0;           // プロシージャキャッシュヒット率
            double memoryUseage = 0;                    // メモリ使用率
            double diskIoBusy = 0;                      // ディスクへの物理アクセスカウント
            int blockingCount = 0;                      // ブロッキングセッション数

            DateTime nowTime;                           // 現在時刻
            DateTime beforeExecTime;                    // 前回実行時刻

            TimeSpan checkTimeSpan;                     // 前回実行時刻との時間差

            double timeSpan;                            // 前回実行時刻との時間差

            DBAccess dbAccess = new DBAccess();         // データベースアクセスクラス

            string memoryText = "";                     // メモリ使用量のテキスト表示用

            bool isChecked = false;                     // チェックしたかどうか
            
            System.Diagnostics.Debug.WriteLine("Server01Monitoring Start:" + DateTime.Now.ToString());

            // ================================================================
            nowTime = DateTime.Now;
            beforeExecTime = DateTime.Parse(BeforeExecTime01.Content.ToString());
            checkTimeSpan = nowTime - beforeExecTime;

            timeSpan = checkTimeSpan.Seconds;

            memoryText = "";
            // ================================================================

            try
            {
                if (timeSpan >= CommonServer01Settings.CpuCheckCycle)
                {
                    // CPUの使用率を取得する
                    cpuUseage = dbAccess.GetCpuUseage(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName);

                    CpuMeter01.MeterValue = cpuUseage;
                    CpuHistory01.PercentageValue = cpuUseage;

                    isChecked = true;
                }
            }
            catch(Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server01" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　CPU使用率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer01Settings.BufferCacheCheckCycle)
                {
                    // バッファキャッシュヒット率を取得する
                    bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName);

                    BufferCacheHitRate01.MeterValue = bufferCacheHitRate;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server01" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　バッファキャッシュヒット率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer01Settings.ProcedureCacheCheckCycle)
                {
                    // プロシージャキャッシュヒット率を取得する
                    procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName);

                    ProcedureCacheHitRate01.MeterValue = procedureCacheHitRate;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server01" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　プロシージャキャッシュヒット率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer01Settings.MemoryCheckCycle)
                {
                    // メモリ使用率を取得
                    memoryUseage = dbAccess.GetMemoryUseage(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName, ref memoryText);

                    MemoryMeter01.MeterValue = memoryUseage;
                    MemoryMeter01.MeterValueText = memoryText;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server01" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　メモリ使用率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer01Settings.HddIoCheckCycle)
                {
                    // ディスクI/Oビジー率の取得
                    diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer01Settings.ConnectionString);
                    Disk_I_O_Meter01.MeterValue = (int)diskIoBusy;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server01" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　ディスクI/Oビジー率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer01Settings.BlockingCheckCycle)
                {
                    string loginInfo = "";                   // ログイン情報

                    // ブロッキング数の取得
                    blockingCount = dbAccess.GetBlockingCount(CommonServer01Settings.ConnectionString, ref currentBlockingSidList01, ref loginInfo);

                    bool isHited = false;                       // ヒットしたかどうか

                    for (int i = 0; i < currentBlockingSidList01.Count; i++)
                    {
                        // HashTableのキー(SID)が含まれていなかったらHashTableを初期化
                        if (!chkBlockingSidList01.ContainsKey(currentBlockingSidList01[i].ToString()))
                        {
                            chkBlockingSidList01.Add(currentBlockingSidList01[i].ToString(), 1);
                        }
                        else
                        {
                            // 含まれていた場合はカウンタをインクリメント
                            string keySid = currentBlockingSidList01[i].ToString();

                            int currentValue = (int)chkBlockingSidList01[keySid] + 1;

                            chkBlockingSidList01[keySid] = currentValue;

                        }
                    }

                    if (currentBlockingSidList01.Count == 0)
                    {
                        chkBlockingSidList01.Clear();
                    }

                    // ブロッキングリストをコピー
                    bufBlockingSidList01 = (Hashtable)chkBlockingSidList01.Clone();

                    // 今回取得分と前回取得分のSIDを取得し、前回取得分の中に存在しない場合は、該当のSIDを削除


                    foreach (string key in chkBlockingSidList01.Keys)
                    {
                        // フラグ初期化
                        isHited = false;

                        for (int i = 0; i < currentBlockingSidList01.Count; i++)
                        {

                            if (key == currentBlockingSidList01[i].ToString())
                            {
                                isHited = true;                                 // フラグを立てる
                                break;
                            }
                        }

                        if (isHited == false)
                        {
                            bufBlockingSidList01.Remove(key);
                        }
                    }

                    // ブロキングリスト(退避)から複製
                    chkBlockingSidList01.Clear();
                    chkBlockingSidList01 = (Hashtable)bufBlockingSidList01.Clone();

                    int maxBlockingCount = 0;

                    // ブロッキング取得回数の最大値を検索
                    foreach (string key in chkBlockingSidList01.Keys)
                    {
                        if (maxBlockingCount < (int)chkBlockingSidList01[key])
                        {
                            maxBlockingCount = (int)chkBlockingSidList01[key];
                        }
                    }

                    // ブロッキング検出回数により警告色へ移行
                    BlockingAlert01.CheckBlockingAlert(maxBlockingCount);

                    // ログイン情報を表示
                    BlockingInfo01.Text = loginInfo;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server01" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　ブロッキング数チェック" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // 待機中のタスク数をカウント
            if (timeSpan >= CommonServer01Settings.BlockingCheckCycle)
            {
                try
                {
                    int waitingTaskCount = 0;                       // 待機中のタスク数

                    waitingTaskCount = dbAccess.GetWaitingTaskCount(CommonServer01Settings.ConnectionString);

                    WaitHistory01.PercentageValue = waitingTaskCount;

                    string titleString = "待機中のタスク" + "(" + waitingTaskCount.ToString() + ")";

                    WaitHistory01.Title = titleString;

                }
                catch (Exception ex)
                {
                    string errorMsg = "エラーが発生しました。" + "\n";
                    errorMsg += "監視対象：" + "\n";
                    errorMsg += "　Server01" + "\n";
                    errorMsg += "エラー発生個所:" + "\n";
                    errorMsg += "　待機タスク数チェック" + "\n";
                    errorMsg += "エラー詳細:" + "\n";
                    errorMsg += "　" + ex.Message;

                    MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (isChecked == true)
            {
                BeforeExecTime01.Content = DateTime.Now.ToString();
            }

            System.Diagnostics.Debug.WriteLine("Server01Monitoring End:" + DateTime.Now.ToString());
        }

        /// <summary>
        /// Server02の監視
        /// </summary>
        /// <param name="timeSpanSecond">前回実行してからの間隔(秒)</param>
        private void Server02Monitoring()
        {
            double cpuUseage = 0;                       // CPU使用率
            double bufferCacheHitRate = 0;              // バッファキャッシュヒット率
            double procedureCacheHitRate = 0;           // プロシージャキャッシュヒット率
            double memoryUseage = 0;                    // メモリ使用率
            double diskIoBusy = 0;                      // ディスクへの物理アクセスカウント
            int blockingCount = 0;                      // ブロッキングセッション数

            DateTime nowTime;                           // 現在時刻
            DateTime beforeExecTime;                    // 前回実行時刻

            TimeSpan checkTimeSpan;                     // 前回実行時刻との時間差

            double timeSpan;                            // 前回実行時刻との時間差

            DBAccess dbAccess = new DBAccess();         // データベースアクセスクラス

            string memoryText = "";                     // メモリ使用量のテキスト表示用

            bool isChecked = false;                     // チェックしたかどうか

            // ================================================================
            nowTime = DateTime.Now;
            beforeExecTime = DateTime.Parse(BeforeExecTime02.Content.ToString());
            checkTimeSpan = nowTime - beforeExecTime;

            timeSpan = checkTimeSpan.Seconds;

            memoryText = "";
            // ================================================================

            try
            {
                if (timeSpan >= CommonServer02Settings.CpuCheckCycle)
                {
                    // CPUの使用率を取得する
                    cpuUseage = dbAccess.GetCpuUseage(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName);

                    CpuMeter02.MeterValue = cpuUseage;
                    CpuHistory02.PercentageValue = cpuUseage;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server02" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　CPU使用率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer02Settings.BufferCacheCheckCycle)
                {
                    // バッファキャッシュヒット率を取得する
                    bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName);

                    BufferCacheHitRate02.MeterValue = bufferCacheHitRate;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server02" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　バッファキャッシュヒット率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer02Settings.ProcedureCacheCheckCycle)
                {
                    // プロシージャキャッシュヒット率を取得する
                    procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName);

                    ProcedureCacheHitRate02.MeterValue = procedureCacheHitRate;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server02" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　プロシージャキャッシュヒット率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer02Settings.MemoryCheckCycle)
                {
                    // メモリ使用率を取得
                    memoryUseage = dbAccess.GetMemoryUseage(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName, ref memoryText);

                    MemoryMeter02.MeterValue = memoryUseage;
                    MemoryMeter02.MeterValueText = memoryText;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server02" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　メモリ使用率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer02Settings.HddIoCheckCycle)
                {
                    // ディスクI/Oビジー率の取得
                    diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer02Settings.ConnectionString);
                    Disk_I_O_Meter02.MeterValue = (int)diskIoBusy;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server02" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　ディスクI/Oビジー率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer02Settings.BlockingCheckCycle)
                {
                    string loginInfo = "";                      // ログイン情報

                    // ブロッキング数の取得
                    blockingCount = dbAccess.GetBlockingCount(CommonServer02Settings.ConnectionString, ref currentBlockingSidList02, ref loginInfo);

                    bool isHited = false;                       // ヒットしたかどうか

                    for (int i = 0; i < currentBlockingSidList02.Count; i++)
                    {
                        // HashTableのキー(SID)が含まれていなかったらHashTableを初期化
                        if (!chkBlockingSidList02.ContainsKey(currentBlockingSidList02[i].ToString()))
                        {
                            chkBlockingSidList02.Add(currentBlockingSidList02[i].ToString(), 1);
                        }
                        else
                        {
                            // 含まれていた場合はカウンタをインクリメント
                            string keySid = currentBlockingSidList02[i].ToString();

                            int currentValue = (int)chkBlockingSidList02[keySid] + 1;

                            chkBlockingSidList02[keySid] = currentValue;

                        }
                    }

                    if (currentBlockingSidList02.Count == 0)
                    {
                        chkBlockingSidList02.Clear();
                    }

                    // ブロッキングリストをコピー
                    bufBlockingSidList02 = (Hashtable)chkBlockingSidList02.Clone();

                    // 今回取得分と前回取得分のSIDを取得し、前回取得分の中に存在しない場合は、該当のSIDを削除
                    foreach (string key in chkBlockingSidList02.Keys)
                    {
                        // フラグ初期化
                        isHited = false;

                        for (int i = 0; i < currentBlockingSidList02.Count; i++)
                        {

                            if (key == currentBlockingSidList02[i].ToString())
                            {
                                isHited = true;                                 // フラグを立てる
                                break;
                            }
                        }

                        if (isHited == false)
                        {
                            bufBlockingSidList02.Remove(key);
                        }
                    }

                    // ブロキングリスト(退避)から複製
                    chkBlockingSidList02.Clear();
                    chkBlockingSidList02 = (Hashtable)bufBlockingSidList02.Clone();

                    int maxBlockingCount = 0;

                    // ブロッキング取得回数の最大値を検索
                    foreach (string key in chkBlockingSidList02.Keys)
                    {
                        if (maxBlockingCount < (int)chkBlockingSidList02[key])
                        {
                            maxBlockingCount = (int)chkBlockingSidList02[key];
                        }
                    }

                    // ブロッキング検出回数により警告色へ移行
                    BlockingAlert02.CheckBlockingAlert(maxBlockingCount);

                    // ログイン情報の表示
                    BlockingInfo02.Text = loginInfo;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server02" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　ブロッキング取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // 待機中のタスク数をカウント
            if (timeSpan >= CommonServer02Settings.BlockingCheckCycle)
            {
                try
                {
                    int waitingTaskCount = 0;                       // 待機中のタスク数

                    waitingTaskCount = dbAccess.GetWaitingTaskCount(CommonServer02Settings.ConnectionString);

                    WaitHistory02.PercentageValue = waitingTaskCount;

                    string titleString = "待機中のタスク" + "(" + waitingTaskCount.ToString() + ")";

                    WaitHistory02.Title = titleString;

                }
                catch (Exception ex)
                {
                    string errorMsg = "エラーが発生しました。" + "\n";
                    errorMsg += "監視対象：" + "\n";
                    errorMsg += "　Server02" + "\n";
                    errorMsg += "エラー発生個所:" + "\n";
                    errorMsg += "　待機タスク数チェック" + "\n";
                    errorMsg += "エラー詳細:" + "\n";
                    errorMsg += "　" + ex.Message;

                    MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (isChecked == true)
            {
                BeforeExecTime02.Content = DateTime.Now.ToString();
            }
        }

        /// <summary>
        /// Server03の監視
        /// </summary>
        /// <param name="timeSpanSecond">前回実行してからの間隔(秒)</param>
        /// <returns>bool:チェックしたかどうか</returns>
        private void Server03Monitoring()
        {
            double cpuUseage = 0;                       // CPU使用率
            double bufferCacheHitRate = 0;              // バッファキャッシュヒット率
            double procedureCacheHitRate = 0;           // プロシージャキャッシュヒット率
            double memoryUseage = 0;                    // メモリ使用率
            double diskIoBusy = 0;                      // ディスクへの物理アクセスカウント
            int blockingCount = 0;                      // ブロッキングセッション数

            DateTime nowTime;                           // 現在時刻
            DateTime beforeExecTime;                    // 前回実行時刻

            TimeSpan checkTimeSpan;                     // 前回実行時刻との時間差

            double timeSpan;                            // 前回実行時刻との時間差

            DBAccess dbAccess = new DBAccess();         // データベースアクセスクラス

            string memoryText = "";                     // メモリ使用量のテキスト表示用

            bool isChecked = false;                     // チェックしたかどうか

            // ================================================================
            nowTime = DateTime.Now;
            beforeExecTime = DateTime.Parse(BeforeExecTime03.Content.ToString());
            checkTimeSpan = nowTime - beforeExecTime;

            timeSpan = checkTimeSpan.Seconds;

            memoryText = "";
            // ================================================================

            try
            {
                if (timeSpan >= CommonServer03Settings.CpuCheckCycle)
                {
                    // CPUの使用率を取得する
                    cpuUseage = dbAccess.GetCpuUseage(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName);

                    CpuMeter03.MeterValue = cpuUseage;
                    CpuHistory03.PercentageValue = cpuUseage;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server03" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　CPU使用率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer03Settings.BufferCacheCheckCycle)
                {
                    // バッファキャッシュヒット率を取得する
                    bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName);

                    BufferCacheHitRate03.MeterValue = bufferCacheHitRate;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server03" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　バッファキャッシュヒット率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer03Settings.ProcedureCacheCheckCycle)
                {
                    // プロシージャキャッシュヒット率を取得する
                    procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName);

                    ProcedureCacheHitRate03.MeterValue = procedureCacheHitRate;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server03" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　プロシージャキャッシュヒット率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer03Settings.MemoryCheckCycle)
                {
                    // メモリ使用率を取得
                    memoryUseage = dbAccess.GetMemoryUseage(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName, ref memoryText);

                    MemoryMeter03.MeterValue = memoryUseage;
                    MemoryMeter03.MeterValueText = memoryText;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server03" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　メモリ使用率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer03Settings.HddIoCheckCycle)
                {
                    // ディスクI/Oビジー率の取得
                    diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer03Settings.ConnectionString);
                    Disk_I_O_Meter03.MeterValue = (int)diskIoBusy;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server03" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　ディスクI/Oビジー率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer03Settings.BlockingCheckCycle)
                {
                    string loginInfo = "";                      // ログイン情報

                    // ブロッキング数の取得
                    blockingCount = dbAccess.GetBlockingCount(CommonServer03Settings.ConnectionString, ref currentBlockingSidList03, ref loginInfo);

                    bool isHited = false;                       // ヒットしたかどうか

                    for (int i = 0; i < currentBlockingSidList03.Count; i++)
                    {
                        // HashTableのキー(SID)が含まれていなかったらHashTableを初期化
                        if (!chkBlockingSidList03.ContainsKey(currentBlockingSidList03[i].ToString()))
                        {
                            chkBlockingSidList03.Add(currentBlockingSidList03[i].ToString(), 1);
                        }
                        else
                        {
                            // 含まれていた場合はカウンタをインクリメント
                            string keySid = currentBlockingSidList03[i].ToString();

                            int currentValue = (int)chkBlockingSidList03[keySid] + 1;

                            chkBlockingSidList03[keySid] = currentValue;

                        }
                    }

                    if (currentBlockingSidList03.Count == 0)
                    {
                        chkBlockingSidList03.Clear();
                    }

                    // ブロッキングリストをコピー
                    bufBlockingSidList03 = (Hashtable)chkBlockingSidList03.Clone();

                    // 今回取得分と前回取得分のSIDを取得し、前回取得分の中に存在しない場合は、該当のSIDを削除
                    foreach (string key in chkBlockingSidList03.Keys)
                    {
                        // フラグ初期化
                        isHited = false;

                        for (int i = 0; i < currentBlockingSidList03.Count; i++)
                        {

                            if (key == currentBlockingSidList03[i].ToString())
                            {
                                isHited = true;                                 // フラグを立てる
                                break;
                            }
                        }

                        if (isHited == false)
                        {
                            bufBlockingSidList03.Remove(key);
                        }
                    }

                    // ブロキングリスト(退避)から複製
                    chkBlockingSidList03.Clear();
                    chkBlockingSidList03 = (Hashtable)bufBlockingSidList03.Clone();

                    int maxBlockingCount = 0;

                    // ブロッキング取得回数の最大値を検索
                    foreach (string key in chkBlockingSidList03.Keys)
                    {
                        if (maxBlockingCount < (int)chkBlockingSidList03[key])
                        {
                            maxBlockingCount = (int)chkBlockingSidList03[key];
                        }
                    }

                    // ブロッキング検出回数により警告色へ移行
                    BlockingAlert03.CheckBlockingAlert(maxBlockingCount);

                    // ログイン情報の表示
                    BlockingInfo03.Text = loginInfo;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server03" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　ブロッキング数取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (isChecked == true)
            {
                BeforeExecTime03.Content = DateTime.Now.ToString();
            }

            // 待機中のタスク数をカウント
            if (timeSpan >= CommonServer03Settings.BlockingCheckCycle)
            {
                try
                {
                    int waitingTaskCount = 0;                       // 待機中のタスク数

                    waitingTaskCount = dbAccess.GetWaitingTaskCount(CommonServer03Settings.ConnectionString);

                    WaitHistory03.PercentageValue = waitingTaskCount;

                    string titleString = "待機中のタスク" + "(" + waitingTaskCount.ToString() + ")";

                    WaitHistory03.Title = titleString;

                }
                catch (Exception ex)
                {
                    string errorMsg = "エラーが発生しました。" + "\n";
                    errorMsg += "監視対象：" + "\n";
                    errorMsg += "　Server03" + "\n";
                    errorMsg += "エラー発生個所:" + "\n";
                    errorMsg += "　ブロッキング数チェック" + "\n";
                    errorMsg += "エラー詳細:" + "\n";
                    errorMsg += "　" + ex.Message;

                    MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Server04の監視
        /// </summary>
        /// <param name="timeSpanSecond">前回実行してからの間隔(秒)</param>
        /// 
        private void Server04Monitoring()
        {
            double cpuUseage = 0;                       // CPU使用率
            double bufferCacheHitRate = 0;              // バッファキャッシュヒット率
            double procedureCacheHitRate = 0;           // プロシージャキャッシュヒット率
            double memoryUseage = 0;                    // メモリ使用率
            double diskIoBusy = 0;                      // ディスクへの物理アクセスカウント
            int blockingCount = 0;                      // ブロッキングセッション数

            DateTime nowTime;                           // 現在時刻
            DateTime beforeExecTime;                    // 前回実行時刻

            TimeSpan checkTimeSpan;                     // 前回実行時刻との時間差

            double timeSpan;                            // 前回実行時刻との時間差

            DBAccess dbAccess = new DBAccess();         // データベースアクセスクラス

            string memoryText = "";                     // メモリ使用量のテキスト表示用

            bool isChecked = false;                     // チェックしたかどうか

            // ================================================================
            nowTime = DateTime.Now;
            beforeExecTime = DateTime.Parse(BeforeExecTime04.Content.ToString());
            checkTimeSpan = nowTime - beforeExecTime;

            timeSpan = checkTimeSpan.Seconds;

            memoryText = "";
            // ================================================================

            try
            {
                if (timeSpan >= CommonServer04Settings.CpuCheckCycle)
                {
                    // CPUの使用率を取得する
                    cpuUseage = dbAccess.GetCpuUseage(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName);

                    CpuMeter04.MeterValue = cpuUseage;
                    CpuHistory04.PercentageValue = cpuUseage;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server04" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　CPU使用率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer04Settings.BufferCacheCheckCycle)
                {
                    // バッファキャッシュヒット率を取得する
                    bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName);

                    BufferCacheHitRate04.MeterValue = bufferCacheHitRate;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server04" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　バッファキャッシュヒット率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer04Settings.ProcedureCacheCheckCycle)
                {
                    // プロシージャキャッシュヒット率を取得する
                    procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName);

                    ProcedureCacheHitRate04.MeterValue = procedureCacheHitRate;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server04" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　プロシージャキャッシュヒット率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer04Settings.MemoryCheckCycle)
                {
                    // メモリ使用率を取得
                    memoryUseage = dbAccess.GetMemoryUseage(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName, ref memoryText);

                    MemoryMeter04.MeterValue = memoryUseage;
                    MemoryMeter04.MeterValueText = memoryText;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server04" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　メモリ使用率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer04Settings.HddIoCheckCycle)
                {
                    // ディスクI/Oビジー率の取得
                    diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer04Settings.ConnectionString);
                    Disk_I_O_Meter04.MeterValue = (int)diskIoBusy;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server04" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　ディスクI/Oビジー率取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                if (timeSpan >= CommonServer04Settings.CpuCheckCycle)
                {
                    string loginInfo = "";                  // ログイン情報
                    // ブロッキング数の取得
                    blockingCount = dbAccess.GetBlockingCount(CommonServer04Settings.ConnectionString, ref currentBlockingSidList04, ref loginInfo);

                    bool isHited = false;                       // ヒットしたかどうか

                    for (int i = 0; i < currentBlockingSidList04.Count; i++)
                    {
                        // HashTableのキー(SID)が含まれていなかったらHashTableを初期化
                        if (!chkBlockingSidList04.ContainsKey(currentBlockingSidList04[i].ToString()))
                        {
                            chkBlockingSidList04.Add(currentBlockingSidList04[i].ToString(), 1);
                        }
                        else
                        {
                            // 含まれていた場合はカウンタをインクリメント
                            string keySid = currentBlockingSidList04[i].ToString();

                            int currentValue = (int)chkBlockingSidList04[keySid] + 1;

                            chkBlockingSidList04[keySid] = currentValue;

                        }
                    }

                    if (currentBlockingSidList04.Count == 0)
                    {
                        chkBlockingSidList04.Clear();
                    }

                    // ブロッキングリストをコピー
                    bufBlockingSidList04 = (Hashtable)chkBlockingSidList04.Clone();

                    // 今回取得分と前回取得分のSIDを取得し、前回取得分の中に存在しない場合は、該当のSIDを削除


                    foreach (string key in chkBlockingSidList04.Keys)
                    {
                        // フラグ初期化
                        isHited = false;

                        for (int i = 0; i < currentBlockingSidList04.Count; i++)
                        {

                            if (key == currentBlockingSidList04[i].ToString())
                            {
                                isHited = true;                                 // フラグを立てる
                                break;
                            }
                        }

                        if (isHited == false)
                        {
                            bufBlockingSidList04.Remove(key);
                        }
                    }

                    // ブロキングリスト(退避)から複製
                    chkBlockingSidList04.Clear();
                    chkBlockingSidList04 = (Hashtable)bufBlockingSidList04.Clone();

                    int maxBlockingCount = 0;

                    // ブロッキング取得回数の最大値を検索
                    foreach (string key in chkBlockingSidList04.Keys)
                    {
                        if (maxBlockingCount < (int)chkBlockingSidList04[key])
                        {
                            maxBlockingCount = (int)chkBlockingSidList04[key];
                        }
                    }

                    // ブロッキング検出回数により警告色へ移行
                    BlockingAlert04.CheckBlockingAlert(maxBlockingCount);

                    // ログイン情報の表示
                    BlockingInfo04.Text = loginInfo;

                    isChecked = true;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "エラーが発生しました。" + "\n";
                errorMsg += "監視対象：" + "\n";
                errorMsg += "　Server04" + "\n";
                errorMsg += "エラー発生個所:" + "\n";
                errorMsg += "　ブロッキング数取得" + "\n";
                errorMsg += "エラー詳細:" + "\n";
                errorMsg += "　" + ex.Message;

                MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // 待機中のタスク数をカウント
            if (timeSpan >= CommonServer04Settings.BlockingCheckCycle)
            {
                try
                {
                    int waitingTaskCount = 0;                       // 待機中のタスク数

                    waitingTaskCount = dbAccess.GetWaitingTaskCount(CommonServer04Settings.ConnectionString);

                    WaitHistory04.PercentageValue = waitingTaskCount;

                    string titleString = "待機中のタスク" + "(" + waitingTaskCount.ToString() + ")";

                    WaitHistory04.Title = titleString;
                }
                catch (Exception ex)
                {
                    string errorMsg = "エラーが発生しました。" + "\n";
                    errorMsg += "監視対象：" + "\n";
                    errorMsg += "　Server04" + "\n";
                    errorMsg += "エラー発生個所:" + "\n";
                    errorMsg += "　ブロッキング数チェック" + "\n";
                    errorMsg += "エラー詳細:" + "\n";
                    errorMsg += "　" + ex.Message;

                    MessageBox.Show(errorMsg, "エラーが発生しました", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (isChecked == true)
            {
                BeforeExecTime04.Content = DateTime.Now.ToString();
            }
        }

        /// <summary>
        /// セッションのリストを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBlockingList01(object sender, MouseButtonEventArgs e)
        {
            OpenBlockingList(CommonServer01Settings.MonitoringStatus, "01", BlockingAlert01.IsAlertDisp);
        }

        /// <summary>
        /// セッションのリストを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBlockingList02(object sender, MouseButtonEventArgs e)
        {
            OpenBlockingList(CommonServer02Settings.MonitoringStatus, "02", BlockingAlert02.IsAlertDisp);
        }

        /// <summary>
        /// セッションのリストを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBlockingList03(object sender, MouseButtonEventArgs e)
        {
            OpenBlockingList(CommonServer03Settings.MonitoringStatus, "03", BlockingAlert03.IsAlertDisp);
        }

        /// <summary>
        /// セッションのリストを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBlockingList04(object sender, MouseButtonEventArgs e)
        {
            OpenBlockingList(CommonServer04Settings.MonitoringStatus, "04", BlockingAlert04.IsAlertDisp);
        }

        /// <summary>
        /// セッションリスト画面の呼び出し
        /// </summary>
        /// <param name="monitoringStatus">監視ステータス(On/Off)</param>
        /// <param name="windowNo">ウィンドウの番号(01/02/03/04)</param>
        private void OpenBlockingList(string monitoringStatus, string windowNo, bool isAlert)
        {
            if (monitoringStatus == "Off")
            {
                return;
            }

            /*
            SessionList blockingList = new SessionList(windowNo);
            blockingList.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            blockingList.Owner = this;

            blockingList.Show();
             * */

            if (isAlert == true)
            {
                BlockingTree blockingTree = new BlockingTree(windowNo);
                blockingTree.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                blockingTree.Show();
            }
            else
            {
                SessionList blockingList = new SessionList(windowNo);
                blockingList.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                blockingList.Owner = this;

                blockingList.Show();
            }
        }

        /// <summary>
        /// クエリーモニター画面呼び出しボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowQueryMonitor1Button_Click(object sender, RoutedEventArgs e)
        {
            OpenQueryMonitor(CommonServer01Settings.MonitoringStatus, "01");
        }

        /// <summary>
        /// クエリーモニター画面の呼び出し
        /// </summary>
        /// <param name="monitoringStatus"></param>
        /// <param name="windowNo"></param>
        private void OpenQueryMonitor(string monitoringStatus, string windowNo)
        {
            if (monitoringStatus == "Off")
            {
                return;
            }

            QueryMonitor queryMonitor = new QueryMonitor(windowNo);
            queryMonitor.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            queryMonitor.Owner = this;

            queryMonitor.Show();
        }

        /// <summary>
        /// クエリーモニター画面呼び出しボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowQueryMonitor2Button_Click(object sender, RoutedEventArgs e)
        {
            OpenQueryMonitor(CommonServer02Settings.MonitoringStatus, "02");
        }

        /// <summary>
        /// クエリーモニター画面呼び出しボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowQueryMonitor3Button_Click(object sender, RoutedEventArgs e)
        {
            OpenQueryMonitor(CommonServer03Settings.MonitoringStatus, "03");
        }

        /// <summary>
        /// クエリーモニター画面呼び出しボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowQueryMonitor4Button_Click(object sender, RoutedEventArgs e)
        {
            OpenQueryMonitor(CommonServer04Settings.MonitoringStatus, "04");
        }
        
        /*
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation01 = new DoubleAnimation();
            animation01.From = 80;
            animation01.To = 0;
            animation01.Duration = new Duration(TimeSpan.FromMilliseconds(1500));
            Storyboard.SetTargetProperty(animation01, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTarget(animation01, this.BlockingInfo01);
            var storyBoard = new Storyboard();
            storyBoard.FillBehavior = FillBehavior.HoldEnd;
            storyBoard.Children.Add(animation01);

            storyBoard.Begin();
        }
         * */
    }
}
