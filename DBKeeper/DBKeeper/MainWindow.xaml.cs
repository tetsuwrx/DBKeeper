﻿using System;
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

            CpuMeter01.MeterTitle = "CPU Useage";
            MemoryMeter01.MeterTitle = "Memory Useage";
            BufferCacheHitRate01.MeterTitle = "Buffer Cache HitRate";
            ProcedureCacheHitRate01.MeterTitle = "Proc Cache HitRate";
            Disk_I_O_Meter01.BarTitle = "Disk I/O";
            if (CommonServer01Settings.MonitoringStatus == "On")
            {
                Server1Title.Content = CommonServer01Settings.HostName;
                BeforeExecTime01.Content = nowTime.ToString();
            }
            else
            {
                Server1Title.Content = "N/A";
            }

            CpuMeter02.MeterTitle = "CPU Useage";
            MemoryMeter02.MeterTitle = "Memory Useage";
            BufferCacheHitRate02.MeterTitle = "Buffer Cache HitRate";
            ProcedureCacheHitRate02.MeterTitle = "Proc Cache HitRate";
            Disk_I_O_Meter02.BarTitle = "Disk I/O";
            if (CommonServer02Settings.MonitoringStatus == "On")
            {
                Server2Title.Content = CommonServer02Settings.HostName;
                BeforeExecTime02.Content = nowTime.ToString();
            }
            else
            {
                Server2Title.Content = "N/A";
            }

            CpuMeter03.MeterTitle = "CPU Useage";
            MemoryMeter03.MeterTitle = "Memory Useage";
            BufferCacheHitRate03.MeterTitle = "Buffer Cache HitRate";
            ProcedureCacheHitRate03.MeterTitle = "Proc Cache HitRate";
            Disk_I_O_Meter03.BarTitle = "Disk I/O";
            if (CommonServer03Settings.MonitoringStatus == "On")
            {
                Server3Title.Content = CommonServer03Settings.HostName;
                BeforeExecTime03.Content = nowTime.ToString();
            }
            else
            {
                Server3Title.Content = "N/A";
            }

            CpuMeter04.MeterTitle = "CPU Useage";
            MemoryMeter04.MeterTitle = "Memory Useage";
            BufferCacheHitRate04.MeterTitle = "Buffer Cache HitRate";
            ProcedureCacheHitRate04.MeterTitle = "Proc Cache HitRate";
            Disk_I_O_Meter04.BarTitle = "Disk I/O";
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

            if (timeSpan >= CommonServer01Settings.CpuCheckCycle)
            {
                // CPUの使用率を取得する
                cpuUseage = dbAccess.GetCpuUseage(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName);

                CpuMeter01.MeterValue = cpuUseage;

                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.BufferCacheCheckCycle)
            {
                // バッファキャッシュヒット率を取得する
                bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName);

                BufferCacheHitRate01.MeterValue = bufferCacheHitRate;

                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.ProcedureCacheCheckCycle)
            {
                // プロシージャキャッシュヒット率を取得する
                procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName);

                ProcedureCacheHitRate01.MeterValue = procedureCacheHitRate;

                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.MemoryCheckCycle)
            {
                // メモリ使用率を取得
                memoryUseage = dbAccess.GetMemoryUseage(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName, ref memoryText);

                MemoryMeter01.MeterValue = memoryUseage;
                MemoryMeter01.MeterValueText = memoryText;

                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.HddIoCheckCycle)
            {
                // ディスクI/Oビジー率の取得
                diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer01Settings.ConnectionString);
                Disk_I_O_Meter01.MeterValue = (int)diskIoBusy;

                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.BlockingCheckCycle)
            {
                // ブロッキング数の取得
                blockingCount = dbAccess.GetBlockingCount(CommonServer01Settings.ConnectionString);
                BlockingAlert01.CheckBlockingAlert(blockingCount);

                isChecked = true;
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

            if (timeSpan >= CommonServer02Settings.CpuCheckCycle)
            {
                // CPUの使用率を取得する
                cpuUseage = dbAccess.GetCpuUseage(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName);

                CpuMeter02.MeterValue = cpuUseage;

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.BufferCacheCheckCycle)
            {
                // バッファキャッシュヒット率を取得する
                bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName);

                BufferCacheHitRate02.MeterValue = bufferCacheHitRate;

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.ProcedureCacheCheckCycle)
            {
                // プロシージャキャッシュヒット率を取得する
                procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName);

                ProcedureCacheHitRate02.MeterValue = procedureCacheHitRate;

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.MemoryCheckCycle)
            {
                // メモリ使用率を取得
                memoryUseage = dbAccess.GetMemoryUseage(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName, ref memoryText);

                MemoryMeter02.MeterValue = memoryUseage;
                MemoryMeter02.MeterValueText = memoryText;

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.HddIoCheckCycle)
            {
                // ディスクI/Oビジー率の取得
                diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer02Settings.ConnectionString);
                Disk_I_O_Meter02.MeterValue = (int)diskIoBusy;

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.CpuCheckCycle)
            {
                // ブロッキング数の取得
                blockingCount = dbAccess.GetBlockingCount(CommonServer02Settings.ConnectionString);
                BlockingAlert02.CheckBlockingAlert(blockingCount);

                isChecked = true;
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

            if (timeSpan >= CommonServer03Settings.CpuCheckCycle)
            {
                // CPUの使用率を取得する
                cpuUseage = dbAccess.GetCpuUseage(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName);

                CpuMeter03.MeterValue = cpuUseage;

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.BufferCacheCheckCycle)
            {
                // バッファキャッシュヒット率を取得する
                bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName);

                BufferCacheHitRate03.MeterValue = bufferCacheHitRate;

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.ProcedureCacheCheckCycle)
            {
                // プロシージャキャッシュヒット率を取得する
                procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName);

                ProcedureCacheHitRate03.MeterValue = procedureCacheHitRate;

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.MemoryCheckCycle)
            {
                // メモリ使用率を取得
                memoryUseage = dbAccess.GetMemoryUseage(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName, ref memoryText);

                MemoryMeter03.MeterValue = memoryUseage;
                MemoryMeter03.MeterValueText = memoryText;

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.HddIoCheckCycle)
            {
                // ディスクI/Oビジー率の取得
                diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer03Settings.ConnectionString);
                Disk_I_O_Meter03.MeterValue = (int)diskIoBusy;

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.CpuCheckCycle)
            {
                // ブロッキング数の取得
                blockingCount = dbAccess.GetBlockingCount(CommonServer03Settings.ConnectionString);
                BlockingAlert03.CheckBlockingAlert(blockingCount);

                isChecked = true;
            }

            if (isChecked == true)
            {
                BeforeExecTime03.Content = DateTime.Now.ToString();
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

            if (timeSpan >= CommonServer04Settings.CpuCheckCycle)
            {
                // CPUの使用率を取得する
                cpuUseage = dbAccess.GetCpuUseage(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName);

                CpuMeter04.MeterValue = cpuUseage;

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.BufferCacheCheckCycle)
            {
                // バッファキャッシュヒット率を取得する
                bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName);

                BufferCacheHitRate04.MeterValue = bufferCacheHitRate;

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.ProcedureCacheCheckCycle)
            {
                // プロシージャキャッシュヒット率を取得する
                procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName);

                ProcedureCacheHitRate04.MeterValue = procedureCacheHitRate;

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.MemoryCheckCycle)
            {
                // メモリ使用率を取得
                memoryUseage = dbAccess.GetMemoryUseage(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName, ref memoryText);

                MemoryMeter04.MeterValue = memoryUseage;
                MemoryMeter04.MeterValueText = memoryText;

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.HddIoCheckCycle)
            {
                // ディスクI/Oビジー率の取得
                diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer04Settings.ConnectionString);
                Disk_I_O_Meter04.MeterValue = (int)diskIoBusy;

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.CpuCheckCycle)
            {
                // ブロッキング数の取得
                blockingCount = dbAccess.GetBlockingCount(CommonServer04Settings.ConnectionString);
                BlockingAlert04.CheckBlockingAlert(blockingCount);

                isChecked = true;
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
            OpenBlockingList(CommonServer01Settings.MonitoringStatus, "01");
        }

        /// <summary>
        /// セッションのリストを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBlockingList02(object sender, MouseButtonEventArgs e)
        {
            OpenBlockingList(CommonServer02Settings.MonitoringStatus, "02");
        }

        /// <summary>
        /// セッションのリストを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBlockingList03(object sender, MouseButtonEventArgs e)
        {
            OpenBlockingList(CommonServer03Settings.MonitoringStatus, "03");
        }

        /// <summary>
        /// セッションのリストを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowBlockingList04(object sender, MouseButtonEventArgs e)
        {
            OpenBlockingList(CommonServer04Settings.MonitoringStatus, "04");
        }

        /// <summary>
        /// セッションリスト画面の呼び出し
        /// </summary>
        /// <param name="monitoringStatus">監視ステータス(On/Off)</param>
        /// <param name="windowNo">ウィンドウの番号(01/02/03/04)</param>
        private void OpenBlockingList(string monitoringStatus, string windowNo)
        {
            if (monitoringStatus == "Off")
            {
                return;
            }

            BlockingList blockingList = new BlockingList(windowNo);
            blockingList.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            blockingList.Owner = this;

            blockingList.Show();
        }
    }
}
