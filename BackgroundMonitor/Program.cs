using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackgroundMonitor.Classes.Common;
using System.Threading;

namespace BackgroundMonitor
{
    class Program
    {
        public AppSettings GlobalCommonSettings;

        public ServerSettings CommonServer01Settings;
        public ServerSettings CommonServer02Settings;
        public ServerSettings CommonServer03Settings;
        public ServerSettings CommonServer04Settings;

        System.Object sync01Object = new System.Object();
        System.Object sync02Object = new System.Object();
        System.Object sync03Object = new System.Object();
        System.Object sync04Object = new System.Object();

        static void Main(string[] args)
        {
            DateTime Server01BeforeExecTime = new DateTime();
            DateTime Server02BeforeExecTime = new DateTime();
            DateTime Server03BeforeExecTime = new DateTime();
            DateTime Server04BeforeExecTime = new DateTime();

            // 前回実行時間をシステム日付で設定
            Server01BeforeExecTime = DateTime.Now;
            Server02BeforeExecTime = DateTime.Now;
            Server03BeforeExecTime = DateTime.Now;
            Server04BeforeExecTime = DateTime.Now;

        }

        /// <summary>
        /// Server01の監視
        /// </summary>
        /// <param name="beforeExecTime">前回実行時間</param>
        /// <returns>処理終了時刻</returns>
        public DateTime Server01Monitoring(DateTime beforeExecTime)
        {
            double cpuUseage = 0;                       // CPU使用率
            double bufferCacheHitRate = 0;              // バッファキャッシュヒット率
            double procedureCacheHitRate = 0;           // プロシージャキャッシュヒット率
            double memoryUseage = 0;                    // メモリ使用率
            double diskIoBusy = 0;                      // ディスクへの物理アクセスカウント
            int blockingCount = 0;                      // ブロッキングセッション数

            DateTime nowTime;                           // 現在時刻
            DateTime returnDateTime = beforeExecTime;   // 戻り値用

            TimeSpan checkTimeSpan;                     // 前回実行時刻との時間差

            double timeSpan;                            // 前回実行時刻との時間差

            DBAccess dbAccess = new DBAccess();         // データベースアクセスクラス

            string memoryText = "";                     // メモリ使用量のテキスト表示用

            bool isChecked = false;                     // チェックしたかどうか

            System.Diagnostics.Debug.WriteLine("Server01Monitoring Start:" + DateTime.Now.ToString());

            Console.WriteLine("Server01Monitoring Start:" + DateTime.Now.ToString());

            // ================================================================
            nowTime = DateTime.Now;
            checkTimeSpan = nowTime - beforeExecTime;

            timeSpan = checkTimeSpan.Seconds;

            memoryText = "";
            // ================================================================

            if (timeSpan >= CommonServer01Settings.CpuCheckCycle)
            {
                // CPUの使用率を取得する
                cpuUseage = dbAccess.GetCpuUseage(CommonServer01Settings.ConnectionString);

                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.BufferCacheCheckCycle)
            {
                // バッファキャッシュヒット率を取得する
                bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName);

                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.ProcedureCacheCheckCycle)
            {
                // プロシージャキャッシュヒット率を取得する
                procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName);

                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.MemoryCheckCycle)
            {
                // メモリ使用率を取得
                memoryUseage = dbAccess.GetMemoryUseage(CommonServer01Settings.ConnectionString, CommonServer01Settings.InstanceName, ref memoryText);

                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.HddIoCheckCycle)
            {
                // ディスクI/Oビジー率の取得
                diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer01Settings.ConnectionString);
                
                isChecked = true;
            }

            if (timeSpan >= CommonServer01Settings.BlockingCheckCycle)
            {
                // ブロッキング数の取得
                blockingCount = dbAccess.GetBlockingCount(CommonServer01Settings.ConnectionString);
                
                isChecked = true;
            }

            if (isChecked == true)
            {
                // 前回実行時間を記憶
                returnDateTime = DateTime.Now;
            }

            // 処理終了メッセージを表示
            Console.WriteLine("Server01Monitoring End:" + DateTime.Now.ToString());

            System.Diagnostics.Debug.WriteLine("Server01Monitoring End:" + DateTime.Now.ToString());

            return returnDateTime;
        }

        /// <summary>
        /// Server02の監視
        /// </summary>
        /// <param name="beforeExecTime">前回実行時間</param>
        /// <returns>処理終了時刻</returns>
        private DateTime Server02Monitoring(DateTime beforeExecTime)
        {
            double cpuUseage = 0;                       // CPU使用率
            double bufferCacheHitRate = 0;              // バッファキャッシュヒット率
            double procedureCacheHitRate = 0;           // プロシージャキャッシュヒット率
            double memoryUseage = 0;                    // メモリ使用率
            double diskIoBusy = 0;                      // ディスクへの物理アクセスカウント
            int blockingCount = 0;                      // ブロッキングセッション数

            DateTime nowTime;                           // 現在時刻
            DateTime returnDateTime = beforeExecTime;   // 戻り値用

            TimeSpan checkTimeSpan;                     // 前回実行時刻との時間差

            double timeSpan;                            // 前回実行時刻との時間差

            DBAccess dbAccess = new DBAccess();         // データベースアクセスクラス

            string memoryText = "";                     // メモリ使用量のテキスト表示用

            bool isChecked = false;                     // チェックしたかどうか

            System.Diagnostics.Debug.WriteLine("Server02Monitoring Start:" + DateTime.Now.ToString());

            Console.WriteLine("Server02Monitoring Start:" + DateTime.Now.ToString());

            // ================================================================
            nowTime = DateTime.Now;
            checkTimeSpan = nowTime - beforeExecTime;

            timeSpan = checkTimeSpan.Seconds;

            memoryText = "";
            // ================================================================

            if (timeSpan >= CommonServer02Settings.CpuCheckCycle)
            {
                // CPUの使用率を取得する
                cpuUseage = dbAccess.GetCpuUseage(CommonServer02Settings.ConnectionString);

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.BufferCacheCheckCycle)
            {
                // バッファキャッシュヒット率を取得する
                bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName);

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.ProcedureCacheCheckCycle)
            {
                // プロシージャキャッシュヒット率を取得する
                procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName);

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.MemoryCheckCycle)
            {
                // メモリ使用率を取得
                memoryUseage = dbAccess.GetMemoryUseage(CommonServer02Settings.ConnectionString, CommonServer02Settings.InstanceName, ref memoryText);

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.HddIoCheckCycle)
            {
                // ディスクI/Oビジー率の取得
                diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer02Settings.ConnectionString);

                isChecked = true;
            }

            if (timeSpan >= CommonServer02Settings.BlockingCheckCycle)
            {
                // ブロッキング数の取得
                blockingCount = dbAccess.GetBlockingCount(CommonServer02Settings.ConnectionString);

                isChecked = true;
            }

            if (isChecked == true)
            {
                // 前回実行時間を記憶
                returnDateTime = DateTime.Now;
            }

            // 処理終了メッセージを表示
            Console.WriteLine("Server02Monitoring End:" + DateTime.Now.ToString());

            System.Diagnostics.Debug.WriteLine("Server02Monitoring End:" + DateTime.Now.ToString());

            return returnDateTime;
        }

        /// <summary>
        /// Server03の監視
        /// </summary>
        /// <param name="beforeExecTime">前回実行時間</param>
        /// <returns>処理終了時刻</returns>
        private DateTime Server03Monitoring(DateTime beforeExecTime)
        {
            double cpuUseage = 0;                       // CPU使用率
            double bufferCacheHitRate = 0;              // バッファキャッシュヒット率
            double procedureCacheHitRate = 0;           // プロシージャキャッシュヒット率
            double memoryUseage = 0;                    // メモリ使用率
            double diskIoBusy = 0;                      // ディスクへの物理アクセスカウント
            int blockingCount = 0;                      // ブロッキングセッション数

            DateTime nowTime;                           // 現在時刻
            DateTime returnDateTime = beforeExecTime;   // 戻り値用

            TimeSpan checkTimeSpan;                     // 前回実行時刻との時間差

            double timeSpan;                            // 前回実行時刻との時間差

            DBAccess dbAccess = new DBAccess();         // データベースアクセスクラス

            string memoryText = "";                     // メモリ使用量のテキスト表示用

            bool isChecked = false;                     // チェックしたかどうか

            System.Diagnostics.Debug.WriteLine("Server03Monitoring Start:" + DateTime.Now.ToString());

            Console.WriteLine("Server03Monitoring Start:" + DateTime.Now.ToString());

            // ================================================================
            nowTime = DateTime.Now;
            checkTimeSpan = nowTime - beforeExecTime;

            timeSpan = checkTimeSpan.Seconds;

            memoryText = "";
            // ================================================================

            if (timeSpan >= CommonServer03Settings.CpuCheckCycle)
            {
                // CPUの使用率を取得する
                cpuUseage = dbAccess.GetCpuUseage(CommonServer03Settings.ConnectionString);

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.BufferCacheCheckCycle)
            {
                // バッファキャッシュヒット率を取得する
                bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName);

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.ProcedureCacheCheckCycle)
            {
                // プロシージャキャッシュヒット率を取得する
                procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName);

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.MemoryCheckCycle)
            {
                // メモリ使用率を取得
                memoryUseage = dbAccess.GetMemoryUseage(CommonServer03Settings.ConnectionString, CommonServer03Settings.InstanceName, ref memoryText);

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.HddIoCheckCycle)
            {
                // ディスクI/Oビジー率の取得
                diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer03Settings.ConnectionString);

                isChecked = true;
            }

            if (timeSpan >= CommonServer03Settings.BlockingCheckCycle)
            {
                // ブロッキング数の取得
                blockingCount = dbAccess.GetBlockingCount(CommonServer03Settings.ConnectionString);

                isChecked = true;
            }

            if (isChecked == true)
            {
                // 前回実行時間を記憶
                returnDateTime = DateTime.Now;
            }

            // 処理終了メッセージを表示
            Console.WriteLine("Server03Monitoring End:" + DateTime.Now.ToString());

            System.Diagnostics.Debug.WriteLine("Server03Monitoring End:" + DateTime.Now.ToString());

            return returnDateTime;
        }

        /// <summary>
        /// Server04の監視
        /// </summary>
        /// <param name="beforeExecTime">前回実行時間</param>
        /// <returns>処理終了時刻</returns>
        private DateTime Server04Monitoring(DateTime beforeExecTime)
        {
            double cpuUseage = 0;                       // CPU使用率
            double bufferCacheHitRate = 0;              // バッファキャッシュヒット率
            double procedureCacheHitRate = 0;           // プロシージャキャッシュヒット率
            double memoryUseage = 0;                    // メモリ使用率
            double diskIoBusy = 0;                      // ディスクへの物理アクセスカウント
            int blockingCount = 0;                      // ブロッキングセッション数

            DateTime nowTime;                           // 現在時刻
            DateTime returnDateTime = beforeExecTime;   // 戻り値用

            TimeSpan checkTimeSpan;                     // 前回実行時刻との時間差

            double timeSpan;                            // 前回実行時刻との時間差

            DBAccess dbAccess = new DBAccess();         // データベースアクセスクラス

            string memoryText = "";                     // メモリ使用量のテキスト表示用

            bool isChecked = false;                     // チェックしたかどうか

            System.Diagnostics.Debug.WriteLine("Server04Monitoring Start:" + DateTime.Now.ToString());

            Console.WriteLine("Server04Monitoring Start:" + DateTime.Now.ToString());

            // ================================================================
            nowTime = DateTime.Now;
            checkTimeSpan = nowTime - beforeExecTime;

            timeSpan = checkTimeSpan.Seconds;

            memoryText = "";
            // ================================================================

            if (timeSpan >= CommonServer04Settings.CpuCheckCycle)
            {
                // CPUの使用率を取得する
                cpuUseage = dbAccess.GetCpuUseage(CommonServer04Settings.ConnectionString);

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.BufferCacheCheckCycle)
            {
                // バッファキャッシュヒット率を取得する
                bufferCacheHitRate = dbAccess.GetBufferCacheHitRate(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName);

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.ProcedureCacheCheckCycle)
            {
                // プロシージャキャッシュヒット率を取得する
                procedureCacheHitRate = dbAccess.GetProcedureCacheHitRate(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName);

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.MemoryCheckCycle)
            {
                // メモリ使用率を取得
                memoryUseage = dbAccess.GetMemoryUseage(CommonServer04Settings.ConnectionString, CommonServer04Settings.InstanceName, ref memoryText);

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.HddIoCheckCycle)
            {
                // ディスクI/Oビジー率の取得
                diskIoBusy = dbAccess.GetDiskIOBusy(CommonServer04Settings.ConnectionString);

                isChecked = true;
            }

            if (timeSpan >= CommonServer04Settings.BlockingCheckCycle)
            {
                // ブロッキング数の取得
                blockingCount = dbAccess.GetBlockingCount(CommonServer04Settings.ConnectionString);

                isChecked = true;
            }

            if (isChecked == true)
            {
                // 前回実行時間を記憶
                returnDateTime = DateTime.Now;
            }

            // 処理終了メッセージを表示
            Console.WriteLine("Server04Monitoring End:" + DateTime.Now.ToString());

            System.Diagnostics.Debug.WriteLine("Server04Monitoring End:" + DateTime.Now.ToString());

            return returnDateTime;
        }
    }
}
