using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;

namespace BackgroundMonitor.Classes.Common
{
    public class AppSettings
    {
        private const string SettingXmlFileName = "DBKeeper.xml";
        private const string Server01ResultXmlFileName = "DBKeeperResult01.xml";
        private const string Server02ResultXmlFileName = "DBKeeperResult02.xml";
        private const string Server03ResultXmlFileName = "DBKeeperResult03.xml";
        private const string Server04ResultXmlFileName = "DBKeeperResult04.xml";

        public const string SystemConnectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=DBKeeper;Integrated Security=True;";

        public ServerSettings server01Settings = new ServerSettings();
        public ServerSettings server02Settings = new ServerSettings();
        public ServerSettings server03Settings = new ServerSettings();
        public ServerSettings server04Settings = new ServerSettings();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AppSettings()
        {
            string BasePath;
            string XmlFilePath;

            string ElementNo = "1";

            // 設定ファイルはプログラムと同じ位置に配置されているので、実行している場所を取得
            BasePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            XmlFilePath = BasePath + "\\" + SettingXmlFileName;

            // 設定XMLファイルを読み込み
            XmlReader xmlReader = XmlReader.Create(XmlFilePath);

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    switch (xmlReader.LocalName)
                    {
                        case "No":
                            ElementNo = xmlReader.ReadString();

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.SettingsNo = ElementNo;
                                    break;
                                case "2":
                                    server02Settings.SettingsNo = ElementNo;
                                    break;
                                case "3":
                                    server03Settings.SettingsNo = ElementNo;
                                    break;
                                case "4":
                                    server04Settings.SettingsNo = ElementNo;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "HostName":
                            
                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.HostName = xmlReader.ReadString();
                                    break;
                                case "2":
                                    server02Settings.HostName = xmlReader.ReadString();
                                    break;
                                case "3":
                                    server03Settings.HostName = xmlReader.ReadString();
                                    break;
                                case "4":
                                    server04Settings.HostName = xmlReader.ReadString();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "ConnectionString":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.ConnectionString = xmlReader.ReadString();
                                    break;
                                case "2":
                                    server02Settings.ConnectionString = xmlReader.ReadString();
                                    break;
                                case "3":
                                    server03Settings.ConnectionString = xmlReader.ReadString();
                                    break;
                                case "4":
                                    server04Settings.ConnectionString = xmlReader.ReadString();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "InstanceName":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.InstanceName = xmlReader.ReadString();
                                    break;
                                case "2":
                                    server02Settings.InstanceName = xmlReader.ReadString();
                                    break;
                                case "3":
                                    server03Settings.InstanceName = xmlReader.ReadString();
                                    break;
                                case "4":
                                    server04Settings.InstanceName = xmlReader.ReadString();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "CpuCheckCycle":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.CpuCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "2":
                                    server02Settings.CpuCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "3":
                                    server03Settings.CpuCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "4":
                                    server04Settings.CpuCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "MemoryCheckCycle":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.MemoryCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "2":
                                    server02Settings.MemoryCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "3":
                                    server03Settings.MemoryCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "4":
                                    server04Settings.MemoryCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "HDDCheckCycle":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.HddCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "2":
                                    server02Settings.HddCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "3":
                                    server03Settings.HddCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "4":
                                    server04Settings.HddCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "HDDIOCheckCycle":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.HddIoCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "2":
                                    server02Settings.HddIoCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "3":
                                    server03Settings.HddIoCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "4":
                                    server04Settings.HddIoCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "BufferCacheCheckCycle":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.BufferCacheCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "2":
                                    server02Settings.BufferCacheCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "3":
                                    server03Settings.BufferCacheCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "4":
                                    server04Settings.BufferCacheCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "ProcedureCacheCheckCycle":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.ProcedureCacheCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "2":
                                    server02Settings.ProcedureCacheCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "3":
                                    server03Settings.ProcedureCacheCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "4":
                                    server04Settings.ProcedureCacheCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "BlockingCheckCycle":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.BlockingCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "2":
                                    server02Settings.BlockingCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "3":
                                    server03Settings.BlockingCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "4":
                                    server04Settings.BlockingCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "IndexFragmentationCheckCycle":

                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.IndexFragmentationCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "2":
                                    server02Settings.IndexFragmentationCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "3":
                                    server03Settings.IndexFragmentationCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                case "4":
                                    server04Settings.IndexFragmentationCheckCycle = int.Parse(xmlReader.ReadString());
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case "Monitoring":
                            switch (ElementNo)
                            {
                                case "1":
                                    server01Settings.MonitoringStatus = xmlReader.ReadString();
                                    break;
                                case "2":
                                    server02Settings.MonitoringStatus = xmlReader.ReadString();
                                    break;
                                case "3":
                                    server03Settings.MonitoringStatus = xmlReader.ReadString();
                                    break;
                                case "4":
                                    server04Settings.MonitoringStatus = xmlReader.ReadString();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            server01Settings.ResultFileName = BasePath + "\\" + Server01ResultXmlFileName;
            server02Settings.ResultFileName = BasePath + "\\" + Server02ResultXmlFileName;
            server03Settings.ResultFileName = BasePath + "\\" + Server03ResultXmlFileName;
            server04Settings.ResultFileName = BasePath + "\\" + Server04ResultXmlFileName;

        }
    }
}
