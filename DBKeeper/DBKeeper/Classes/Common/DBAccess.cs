using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace DBKeeper.Classes.Common
{
    class DBAccess
    {
        private const short TABLE_NUM_CPU_BUSY = 1;

        /// <summary>
        /// GetDataSet
        /// </summary>
        /// <param name="SelectSql">対象SQL</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string ParamSelectSql, string ParamConnectionString, ref string RefErrorMessage)
        {
            SqlConnection sqlCon = new SqlConnection();
            SqlCommand sqlComm = new SqlCommand();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet retDataset = new DataSet();

            // 接続文字列の設定
            sqlCon.ConnectionString = ParamConnectionString;

            try
            {
                // コネクションのオープン
                sqlCon.Open();

                // SELECT文の設定
                sqlComm.CommandText = ParamSelectSql;
                sqlComm.Connection = sqlCon;

                // コマンドを設定
                dataAdapter.SelectCommand = sqlComm;

                // コマンド実行
                dataAdapter.Fill(retDataset);
            }
            catch (Exception e)
            {
                RefErrorMessage = e.Message;
            }
            finally
            {
                // コネクションのクローズ
                sqlCon.Close();
                // オブジェクトの破棄
                dataAdapter.Dispose();
                sqlComm.Dispose();
                sqlCon.Dispose();
            }

            return retDataset;

        }

        /// <summary>
        /// Select文を発行しDataReaderクラスを返す
        /// </summary>
        /// <param name="ParamSelectSQL">SELECT文</param>
        /// <param name="ParamConnectionString">接続文字列</param>
        /// <param name="RefErrorMessage">[Ref]エラーメッセージ</param>
        /// <returns></returns>
        public SqlDataReader GetDataReader(string ParamSelectSQL, string ParamConnectionString, ref string RefErrorMessage)
        {
            SqlConnection sqlCon = new SqlConnection();
            SqlCommand sqlComm = new SqlCommand();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlDataReader retDataReader = null;

            // 接続文字列の設定
            sqlCon.ConnectionString = ParamConnectionString;

            try
            {
                // コネクションのオープン
                sqlCon.Open();

                // SELECT文の設定
                sqlComm.CommandText = ParamSelectSQL;
                sqlComm.Connection = sqlCon;

                // コマンドを設定
                dataAdapter.SelectCommand = sqlComm;

                // コマンド実行
                retDataReader = sqlComm.ExecuteReader();
            }
            catch (Exception e)
            {
                RefErrorMessage = e.Message;
            }
            finally
            {
                // コネクションのクローズ
                sqlCon.Close();
                // オブジェクトの破棄
                dataAdapter.Dispose();
                sqlComm.Dispose();
                sqlCon.Dispose();
            }

            return retDataReader;
        }

        /// <summary>
        /// コマンドの実行
        /// </summary>
        /// <param name="paramExecuteSQL">実行SQL</param>
        /// <param name="paramConnectionString">接続文字列</param>
        /// <returns>エラーメッセージ。なければ空文字が返ります。</returns>
        public string ExecuteCommand(string paramExecuteSQL, string paramConnectionString)
        {
            SqlConnection sqlCon = new SqlConnection(paramConnectionString);
            SqlCommand sqlComm = new SqlCommand(paramExecuteSQL, sqlCon);

            string errorMessage = "";                            // エラーメッセージ用

            try
            {
                // コネクションオープン
                sqlComm.Connection.Open();

                // コマンド実行
                sqlComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // エラーメッセージの取得
                errorMessage = ex.Message;
            }
            finally
            {
                // コネクションクローズ
                sqlComm.Connection.Close();

                // オブジェクトの破棄
                sqlCon.Dispose();
                sqlComm.Dispose();
            }

            // 戻り値返却
            return errorMessage;

        }

        /// <summary>
        /// CPU使用率の取得
        /// </summary>
        /// <param name="CurrentConnectionString">対象の接続文字列</param>
        /// <returns>CPU使用率(double)</returns>
        public double GetCpuUseage(string CurrentConnectionString, string InstanceName)
        {
            double retValue = 0;                // 戻り値用
            string selectSQL = "";              // 取得用SQL
            string errorMessage = "";           // エラーメッセージ
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            string dataValue = "";              // DBから取得した値
            
            selectSQL = "select perfCount.object_name" + "\n";
            selectSQL += "     , perfCount.counter_name" + "\n";
            selectSQL += "     , perfCount.instance_name" + "\n";
            selectSQL += "     , case when perfBase.cntr_value = 0 then 0" + "\n";
            selectSQL += "       else ( cast ( perfCount.cntr_value as float ) / perfBase.cntr_value ) * 100";
            selectSQL += "       end as cntr_Value" + "\n";
            selectSQL += "  from ( " + "\n";
            selectSQL += "    select top 1 * from sys.dm_os_performance_counters" + "\n";
            selectSQL += "     where object_Name = '" + InstanceName + ":Resource Pool Stats'" + "\n";
            selectSQL += "       and counter_name = 'CPU Usage %' ) perfCount" + "\n";
            selectSQL += " inner join" + "\n";
            selectSQL += " (" + "\n";
            selectSQL += "    select * from sys.dm_os_performance_counters" + "\n";
            selectSQL += "     where object_Name = '" + InstanceName + ":Resource Pool Stats'" + "\n";
            selectSQL += "       and counter_name = 'CPU Usage % base' ) perfBase" + "\n";
            selectSQL += "    on perfCount.Object_name = perfBase.Object_name" + "\n";
            selectSQL += "   and perfCount.instance_name = perfBase.instance_name" + "\n";
            selectSQL += " where perfCount.instance_name = 'default';";
            
            /*
            selectSQL = "exec sp_monitor;";
             * */

            System.Diagnostics.Debug.WriteLine(selectSQL);

            dataSet = GetDataSet(selectSQL, CurrentConnectionString, ref errorMessage);

            if (errorMessage == "")
            {
                /* sp_monitor で取得した場合の処理
                dataTable = dataSet.Tables[1];
                dataValue = dataTable.Rows[0]["cpu_busy"].ToString();

                string[] splitVal = dataValue.Split('-');
                string tmpVal = splitVal[1].Substring(0, splitVal[1].IndexOf('%'));

                if ( tmpVal == "" )
                {
                    retValue = 0;
                }
                else
                {
                    retValue = double.Parse(tmpVal);
                }
                 * */
                dataTable = dataSet.Tables[0];
                dataValue = dataTable.Rows[0]["cntr_Value"].ToString();

                retValue = double.Parse(dataValue);

                if (retValue > 100)
                {
                    retValue = 100;
                }

            }

            dataSet.Dispose();
            dataTable.Dispose();

            return retValue;
        }

        /// <summary>
        /// メモリ使用率の取得
        /// </summary>
        /// <param name="CurrentConnectionString">対象の接続文字列</param>
        /// <param name="InstanceName">インスタンス名</param>
        /// <param name="MemoryInUse">使用しているメモリのサイズ(MB)</param>
        /// <returns>メモリ使用率</returns>
        public double GetMemoryUseage(string CurrentConnectionString, string InstanceName, ref string MemoryInUse)
        {
            double retValue = 0;                // 戻り値用
            string selectSQL = "";              // 取得用SQL
            string errorMessage = "";           // エラーメッセージ
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            string dataValue = "";              // DBから取得した値
            double memValue = 0;

            selectSQL = "SELECT a.cntr_value as MemoryInUse " + "\n";
            selectSQL += "    , (a.cntr_value * 1.0 / b.cntr_value) * 100.0 AS MemoryUseage" + "\n";
            selectSQL += " FROM sys.dm_os_performance_counters a" + "\n";
            selectSQL += " JOIN (SELECT cntr_value,OBJECT_NAME " + "\n";
            selectSQL += "         FROM sys.dm_os_performance_counters " + "\n";
            selectSQL += "        WHERE counter_name = 'Target Server Memory (KB)'" + "\n";
            selectSQL += "          AND OBJECT_NAME = '" + InstanceName + ":Memory Manager'" + "\n";
            selectSQL += "     ) b" + "\n";
            selectSQL += "    ON a.OBJECT_NAME = b.OBJECT_NAME" + "\n";
            selectSQL += " WHERE a.counter_name = 'Total Server Memory (KB)'" + "\n";
            selectSQL += "   AND a.OBJECT_NAME = '" + InstanceName + ":Memory Manager'" + "\n";

            dataSet = GetDataSet(selectSQL, CurrentConnectionString, ref errorMessage);

            if (errorMessage == "")
            {
                dataTable = dataSet.Tables[0];
                dataValue = dataTable.Rows[0][1].ToString();

                memValue = double.Parse(dataTable.Rows[0][0].ToString()) / 1024;

                if (memValue > 1024)
                {
                    memValue = memValue / 1024;
                    MemoryInUse = memValue.ToString("0.00") + " GB";
                }
                else
                {
                    MemoryInUse = memValue.ToString("0.00") + " MB";
                }

                retValue = double.Parse(dataValue);

            }

            dataSet.Dispose();
            dataTable.Dispose();

            return retValue;
        }

        /// <summary>
        /// バッファキャッシュヒット率の取得
        /// </summary>
        /// <param name="CurrentConnectionString">対象の接続文字列</param>
        /// <param name="InstanceName">インスタンス名</param>
        /// <returns>バッファキャッシュヒット率</returns>
        public double GetBufferCacheHitRate(string CurrentConnectionString, string InstanceName)
        {
            double retValue = 0;                // 戻り値用
            string selectSQL = "";              // 取得用SQL
            string errorMessage = "";           // エラーメッセージ
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            string dataValue = "";              // DBから取得した値
            
            selectSQL = "SELECT (a.cntr_value * 1.0 / b.cntr_value) * 100.0 AS BufferCacheHitRatio" + "\n";
            selectSQL += " FROM sys.dm_os_performance_counters a" + "\n";
            selectSQL += " JOIN (SELECT cntr_value,OBJECT_NAME " + "\n";
            selectSQL += "         FROM sys.dm_os_performance_counters " + "\n";
            selectSQL += "        WHERE counter_name = 'Buffer cache hit ratio base'" + "\n";
            selectSQL += "          AND OBJECT_NAME = '" + InstanceName + ":Buffer Manager'" + "\n";
            selectSQL += "     ) b" + "\n";
            selectSQL += "    ON a.OBJECT_NAME = b.OBJECT_NAME" + "\n";
            selectSQL += " WHERE a.counter_name = 'Buffer cache hit ratio'" + "\n";
            selectSQL += "   AND a.OBJECT_NAME = '" + InstanceName + ":Buffer Manager'" + "\n";
            
            dataSet = GetDataSet(selectSQL, CurrentConnectionString, ref errorMessage);

            if (errorMessage == "")
            {
                dataTable = dataSet.Tables[0];
                dataValue = dataTable.Rows[0][0].ToString();

                retValue = double.Parse(dataValue);

            }

            dataSet.Dispose();
            dataTable.Dispose();

            return retValue;
        }

        /// <summary>
        /// プロシージャキャッシュヒット率の取得
        /// </summary>
        /// <param name="CurrentConnectionString">対象の接続文字列</param>
        /// <returns>プロシージャキャッシュヒット率</returns>
        public double GetProcedureCacheHitRate(string CurrentConnectionString, string InstanceName)
        {
            double retValue = 0;                // 戻り値用
            string selectSQL = "";              // 取得用SQL
            string errorMessage = "";           // エラーメッセージ
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            string dataValue = "";              // DBから取得した値

            selectSQL = "SELECT (a.cntr_value * 1.0 / b.cntr_value) * 100.0 AS ProcedureCacheHitRatio" + "\n";
            selectSQL += " FROM sys.dm_os_performance_counters a" + "\n";
            selectSQL += " JOIN (SELECT cntr_value,OBJECT_NAME " + "\n";
            selectSQL += "         FROM sys.dm_os_performance_counters " + "\n";
            selectSQL += "        WHERE counter_name = 'Cache Hit Ratio Base'" + "\n";
            selectSQL += "          AND OBJECT_NAME = '" + InstanceName + ":Plan Cache'" + "\n";
            selectSQL += "          AND instance_name = 'SQL Plans'" + "\n";
            selectSQL += "     ) b" + "\n";
            selectSQL += "    ON a.OBJECT_NAME = b.OBJECT_NAME" + "\n";
            selectSQL += " WHERE a.counter_name = 'Cache Hit Ratio'" + "\n";
            selectSQL += "   AND a.OBJECT_NAME = '" + InstanceName + ":Plan Cache'" + "\n";
            selectSQL += "   AND a.instance_name = 'SQL Plans'" + "\n";

            dataSet = GetDataSet(selectSQL, CurrentConnectionString, ref errorMessage);

            if (errorMessage == "")
            {
                dataTable = dataSet.Tables[0];
                dataValue = dataTable.Rows[0][0].ToString();

                retValue = double.Parse(dataValue);

            }

            dataSet.Dispose();
            dataTable.Dispose();

            return retValue;
        }

        /// <summary>
        /// ディスクIOビジー率の取得
        /// </summary>
        /// <param name="CurrentConnectionString">対象の接続文字列</param>
        /// <returns>ディスクIOビジー率</returns>
        public double GetDiskIOBusy(string CurrentConnectionString)
        {
            double retValue = 0;                // 戻り値用
            string selectSQL = "";              // 取得用SQL
            string errorMessage = "";           // エラーメッセージ
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            string dataValue = "";              // DBから取得した値

            selectSQL = "declare @ExecTime datetime = current_timestamp;" + "\n";
            selectSQL += "declare @TargetTime datetime = dateadd(minute, -1, @ExecTime);" + "\n";
            selectSQL += "declare @SqlPhysicalStats table (" + "\n";
            selectSQL += "    avg_physical_reads_mb bigint," + "\n";
            selectSQL += "    database_name varchar(255)," + "\n";
            selectSQL += "    creation_time datetime," + "\n";
            selectSQL += "    last_execution_time datetime" + "\n";
            selectSQL += ");" + "\n";
            selectSQL += "insert into @SqlPhysicalStats" + "\n";
            selectSQL += "select ( (a.total_physical_reads / execution_count) * ( 1024 * 8 ) ) / ( 1024 * 1024 ) as [avg_physical_reads_mb]" + "\n";
            selectSQL += "     , c.name" + "\n";
            selectSQL += "     , a.creation_time" + "\n";
            selectSQL += "     , a.last_execution_time" + "\n";
            selectSQL += "  from sys.dm_exec_query_stats a" + "\n";
            selectSQL += " cross apply sys.dm_exec_sql_text(a.plan_handle) b" + "\n";
            selectSQL += " inner join sys.databases c" + "\n";
            selectSQL += "    on b.dbid = c.database_id" + "\n";
            selectSQL += "   and b.dbid > 4" + "\n";
            selectSQL += " where a.last_execution_time > @TargetTime" + "\n";
            selectSQL += ";" + "\n";
            selectSQL += "select max(avg_physical_reads_mb) as [max_avg_physical_reads_mb]" + "\n";
            selectSQL += "  from @SqlPhysicalStats" + "\n";
            
            dataSet = GetDataSet(selectSQL, CurrentConnectionString, ref errorMessage);

            if (errorMessage == "")
            {
                if (dataSet.Tables.Count > 0)
                {
                    dataTable = dataSet.Tables[0];
                    dataValue = dataTable.Rows[0][0].ToString();

                    retValue = double.Parse(dataValue);
                }

            }

            dataSet.Dispose();
            dataTable.Dispose();

            return retValue;
        }

        /// <summary>
        /// ブロッキングセッション数の取得
        /// </summary>
        /// <param name="CurrentConnectionString">対象の接続文字列</param>
        /// <param name="refBlockingSidList">ブロッキングリスト</param>
        /// <returns>ブロッキングセッション数</returns>
        public int GetBlockingCount(string CurrentConnectionString, ref ArrayList refBlockingSidList)
        {
            int retValue = 0;                // 戻り値用
            string selectSQL = "";              // 取得用SQL
            string errorMessage = "";           // エラーメッセージ
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();

            // string dataValue = "";              // DBから取得した値

            ArrayList blockingList = new ArrayList();               // ブロッキングセッションIDのリスト

            /*
            selectSQL = "declare @BlockingInfo table (" + "\n";
            selectSQL += "    LockType nvarchar(20)," + "\n";
            selectSQL += "    DbName nvarchar(20)," + "\n";
            selectSQL += "    DatabaseID int," + "\n";
            selectSQL += "    BlkObjectID bigint," + "\n";
            selectSQL += "    LockReq nvarchar(10)," + "\n";
            selectSQL += "    WaiterSessionID int," + "\n";
            selectSQL += "    WaitTime bigint," + "\n";
            selectSQL += "    WaiterSQL nvarchar(2000)," + "\n";
            selectSQL += "    ExecutingSQL nvarchar(2000)," + "\n";
            selectSQL += "    BlockerSessionID int," + "\n";
            selectSQL += "    BlockerSQL nvarchar(2000)" + "\n";
            selectSQL += "    );" + "\n";
            selectSQL += "" + "\n";
            selectSQL += "insert into @BlockingInfo" + "\n";
            selectSQL += "select t1.resource_type as [lock type]" + "\n";
            selectSQL += "     , db_name(resource_database_id) as [database]" + "\n";
            selectSQL += "     , resource_database_id as [database id]" + "\n";
            selectSQL += "     , t1.resource_associated_entity_id as [blk object]" + "\n";
            selectSQL += "     , t1.request_mode as [lock_req]" + "\n";
            selectSQL += "     , t1.request_session_id as [waiter sid]" + "\n";
            selectSQL += "     , t2.wait_duration_ms as [wait time]" + "\n";
            selectSQL += "     , ( select text from sys.dm_exec_requests as r" + "\n";
            selectSQL += " cross apply sys.dm_exec_sql_text(r.sql_handle)" + "\n";
            selectSQL += " where r.session_id = t1.request_session_id) as waiter_batch" + "\n";
            selectSQL += "     , ( select substring(qt.text, r.statement_start_offset / 2, (case when r.statement_end_offset = -1 then len(convert(nvarchar(max), qt.text)) * 2 " + "\n";
            selectSQL += "       else r.statement_end_offset" + "\n";
            selectSQL += "       end - r.statement_start_offset) / 2)" + "\n";
            selectSQL += "  from sys.dm_exec_requests as r" + "\n";
            selectSQL += " cross apply sys.dm_exec_sql_text(r.sql_handle) as qt" + "\n";
            selectSQL += " where r.session_id = t1.request_session_id) as waiter_stmt" + "\n";
            selectSQL += "     , t2.blocking_session_id as [blocker sid]" + "\n";
            selectSQL += "     , ( select text" + "\n";
            selectSQL += "           from sys.sysprocesses as p" + "\n";
            selectSQL += "          cross apply sys.dm_exec_sql_text(p.sql_handle)" + "\n";
            selectSQL += "          where p.spid = t2.blocking_session_id) as blocker_stmt" + "\n";
            selectSQL += "           from sys.dm_tran_locks as t1" + "\n";
            selectSQL += "              , sys.dm_os_waiting_tasks as t2" + "\n";
            selectSQL += "          where t1.lock_owner_address = t2.resource_address" + "\n";
            selectSQL += ";" + "\n";
            selectSQL += "select count(*) " + "\n";
            selectSQL += "  from @BlockingInfo " + "\n";
            selectSQL += ";";
             * */

            selectSQL = "select blocking_session_id" + "\n";
            selectSQL += "  from sys.dm_os_waiting_tasks" + "\n";
            selectSQL += " where blocking_session_id > 50;";

            dataSet = GetDataSet(selectSQL, CurrentConnectionString, ref errorMessage);

            if (errorMessage == "")
            {
                if (dataSet.Tables.Count > 0)
                {
                    dataTable = dataSet.Tables[0];

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        // ブロッキングセッションIDをArrayListに蓄積
                        blockingList.Add(dataTable.Rows[i]["blocking_session_id"].ToString());
                    }

                    // dataValue = dataTable.Rows[0][0].ToString();
                    // retValue = int.Parse(dataValue);
                    retValue = dataTable.Rows.Count;
                }
            }

            dataSet.Dispose();
            dataTable.Dispose();

            // ブロッキングリストを渡す
            refBlockingSidList = blockingList;

            return retValue;
        }
    }
}
