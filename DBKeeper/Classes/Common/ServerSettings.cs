using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBKeeper.Classes;

namespace DBKeeper.Classes.Common
{
    public class ServerSettings
    {
        private string elemNo;
        private string elemHostName;
        private string elemConnectionString;
        private string elemInstanceName;
        private int elemCpuCheckCycle;
        private int elemMemoryCheckCycle;
        private int elemHDDCheckCycle;
        private int elemHDDIOCheckCycle;
        private int elemBufferCacheCheckCycle;
        private int elemProcedureCacheCheckCycle;
        private int elemBlockingCheckCycle;
        private int elemIndexFragmentationCheckCycle;
        private string elemMonitoring;

        public string SettingsNo
        {
            set { elemNo = value; }
            get { return elemNo; }
        }

        public string HostName
        {
            set { elemHostName = value; }
            get { return elemHostName; }
        }

        public string ConnectionString
        {
            set { elemConnectionString = value; }
            get { return elemConnectionString; }
        }

        public string InstanceName
        {
            set { elemInstanceName = value; }
            get { return elemInstanceName; }
        }

        public int CpuCheckCycle
        {
            set { elemCpuCheckCycle = value; }
            get { return elemCpuCheckCycle; }
        }

        public int MemoryCheckCycle
        {
            set { elemMemoryCheckCycle = value; }
            get { return elemMemoryCheckCycle; }
        }

        public int HddCheckCycle
        {
            set { elemHDDCheckCycle = value; }
            get { return elemHDDCheckCycle; }
        }

        public int HddIoCheckCycle
        {
            set { elemHDDIOCheckCycle = value; }
            get { return elemHDDIOCheckCycle; }
        }

        public int BufferCacheCheckCycle
        {
            set { elemBufferCacheCheckCycle = value; }
            get { return this.elemBufferCacheCheckCycle; }
        }

        public int ProcedureCacheCheckCycle
        {
            set { elemProcedureCacheCheckCycle = value; }
            get { return this.elemProcedureCacheCheckCycle; }
        }

        public int BlockingCheckCycle
        {
            set { elemBlockingCheckCycle = value; }
            get { return this.elemBlockingCheckCycle; }
        }

        public int IndexFragmentationCheckCycle
        {
            set { elemIndexFragmentationCheckCycle = value; }
            get { return this.elemIndexFragmentationCheckCycle; }
        }

        public string MonitoringStatus
        {
            set { elemMonitoring = value; }
            get { return this.elemMonitoring; }
        }
    }
}
