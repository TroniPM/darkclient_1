#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SystemConfig
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.19
// 模块描述：系统参数配置。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Mogo.Util
{
    /// <summary>
    /// 系统参数配置。
    /// </summary>
    public partial class SystemConfig
    {
        #region 常量

        public const String ATIVATE_URL_TEMPLATE = "{0}server={1}&dbid={2}&serial_number={3}";
        public const String ASSET_FILE_HEAD = "file://";
        public const String ASSET_FILE_EXTENSION = ".u";//打包资源必须带个一样的后缀，给它坑得一塌糊涂，没有一致后缀，资源依赖就有问题
        public const String MOGO_RESOURCE = @"/MogoResource.xml";
        public const String DEFINE_LIST_FILE_NAME = "entities";
        public const string SERVER_LIST_URL_KEY = "serverlist";
        public const string VERSION_URL_KEY = "version";
        public const string MARKET_URL_KEY = "market";
        public const string LOGIN_MARKET_URL_KEY = "LoginMarketData";
        //public const string PACKAGE_LIST_URL_KEY = "packagelist";
        //public const string PACKAGE_URL_KEY = "packageurl";
        //public const string APK_URL_KEY = "apkurl";
        public const string CFG_FILE = "cfg.xml";
        public const string XML = ".xml";
        public const string CONFIG_SUB_FOLDER = "data/";

        public const string NOTICE_URL_KEY = "NoticeData";
        public const string NOTICE_CONTENT_KEY = "notice";
        public const string ACTIVATE_URL = "ActivateUrl";

        public static String ENTITY_DEFS_PATH
        {
            get
            {
                return "entity_defs/";
            }
        }
        public static String CONFIG_FILE_EXTENSION
        {
            get
            {
                return SystemSwitch.ReleaseMode ? XML : String.Empty;
            }
        }

        public readonly static String ConfigPath = Application.persistentDataPath + "/config.xml";
        public readonly static String ServerListPath = Application.persistentDataPath + "/server.xml";
        public readonly static String LocalServerListPath = Application.persistentDataPath + "/localserver.xml";
        public readonly static String VersionPath = Application.persistentDataPath + "/version.xml";
        //public readonly static String FileListPath = Application.persistentDataPath + "/FilesInfo.txt";
        public readonly static string CfgPath = String.Concat(Application.persistentDataPath, "/", CFG_FILE);
        public readonly static String PackageMd5Path = Application.persistentDataPath + "/packagemd5.xml";
        public readonly static string ServerVersionPath = String.Concat(Application.persistentDataPath, "/server_version.xml");
        public readonly static string SystemSwitchPath = String.Concat(Application.persistentDataPath, "/SystemSwitch.xml");

        #endregion

        #region 属性

        private static String m_resourceFolder;
        private static LocalSetting m_instance;
        private static int m_selectedServerIndex;

        public static int SelectedServerIndex
        {
            get { return SystemConfig.m_selectedServerIndex; }
            set { SystemConfig.m_selectedServerIndex = value; }
        }

        /// <summary>
        /// Android资源路径。
        /// </summary>
        public static String AndroidPath
        {
            get
            {
                return String.Concat(Application.persistentDataPath, "/MogoResources/");//"/sdcard/MogoResources/";
            }
        }

        /// <summary>
        /// PC资源路径。
        /// </summary>
        public static String PCPath
        {
            get
            {
                var path = Application.dataPath + "/../MogoResources/";
                LoggerHelper.Debug("PcPath: " + path);
                return path;
            }
        }

        /// <summary>
        /// IOS资源路径。
        /// </summary>
        public static String IOSPath
        {
            get
            {
                return String.Concat(Application.persistentDataPath, "/MogoResources/");
            }
        }

        /// <summary>
        /// 资源根目录。
        /// </summary>
        public static String ResourceFolder
        {
            get
            {
                if (m_resourceFolder == null)
                {
                    if (SystemSwitch.ReleaseMode)
                        m_resourceFolder = OutterPath;
                    else
                        m_resourceFolder = String.Empty;
                }
                return m_resourceFolder;
            }
            set { }
        }

        /// <summary>
        /// 外部资源目录。
        /// </summary>
        public static String OutterPath
        {
            get
            {
                LoggerHelper.Debug("Application.platform: " + Application.platform);
                if (Application.platform == RuntimePlatform.Android)
                    return AndroidPath;
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return IOSPath;
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                    return PCPath;
                else if (Application.platform == RuntimePlatform.WindowsEditor)
                    return PCPath;
                else if (Application.platform == RuntimePlatform.OSXEditor)
                    return IOSPath;
                else
                    return "";
            }
        }

        public static bool IsUseOutterConfig
        {
            get
            {
                LoggerHelper.Debug("Application.platform: " + Application.platform);
                if (Application.platform == RuntimePlatform.Android)
                {
                    if (Directory.Exists(String.Concat(AndroidPath, CONFIG_SUB_FOLDER)))
                        return true;
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    if (Directory.Exists(String.Concat(IOSPath, CONFIG_SUB_FOLDER)))
                        return true;
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    if (Directory.Exists(String.Concat(PCPath, CONFIG_SUB_FOLDER)))
                        return true;
                }
                return false;
            }
        }

        public static LocalSetting Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new LocalSetting();
                return m_instance;
            }
            set { m_instance = value; }
        }
        public static List<ServerInfo> ServersList;
        /// <summary>
        /// 服务器URL配置信息。
        /// </summary>
        public static List<CfgInfo> CfgInfo = new List<CfgInfo>();

        #endregion

        #region 公有方法

        public static bool IfServerInfoExist(int id)
        {
            //if (id < ServersList.Count)
            //    return true;
            //else
            //    return false;
            foreach (var item in ServersList)
            {
                if (item.id == id)
                    return true;
            }
            return false;
        }

        public static ServerInfo GetServerInfoByIndex(int index)
        {
            if (index >= 0 && index < ServersList.Count)
                return ServersList[index];
            else
                return null;
        }
        public static int GetServerIndexById(int id)
        {
            //if (Instance.SelectedServer >= 0 && Instance.SelectedServer < ServersList.Count)
            //    return ServersList[Instance.SelectedServer];
            //else
            //    return null;
            for (int i = 0; i < ServersList.Count; i++)
            {
                if (ServersList[i].id == id)
                    return i;
            }
            return -1;
        }

        public static ServerInfo GetRecommentServer()
        {
            foreach (var item in ServersList)
            {
                if (item.flag == (int)ServerType.Recommend)
                    return item;
            }
            foreach (var item in ServersList)
            {
                if (item.flag == (int)ServerType.Hot)
                    return item;
            }
            foreach (var item in ServersList)
            {
                if (item.flag == (int)ServerType.Normal)
                    return item;
            }
            foreach (var item in ServersList)
            {
                if (item.flag == (int)ServerType.Maintain)
                    return item;
            }
            foreach (var item in ServersList)
            {
                if (item.flag == (int)ServerType.Close)
                    return item;
            }

            return null;
        }


        public static ServerInfo GetSelectedServerInfo()
        {
            //if (Instance.SelectedServer >= 0 && Instance.SelectedServer < ServersList.Count)
            //    return ServersList[Instance.SelectedServer];
            //else
            //    return null;
            foreach (var item in ServersList)
            {
                if (item.id == Instance.SelectedServer)
                    return item;
            }
            return null;
        }

        public static bool Init(
#if UNITY_IPHONE
            System.Action callback
#endif
            )
        {
            try
            {
                var result = LoadCfgInfo(
#if UNITY_IPHONE
                    callback
#endif
                    );
#if UNITY_IPHONE
                if (!result)
                    return false;
#endif
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }
            try
            {
                LoadConfig();
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }
            return true;
        }

        public static void LoadServerList()
        {
            try
            {
                List<ServerInfo> servers;
                if (File.Exists(LocalServerListPath))//如果存在
                {
                    servers = LoadXML<ServerInfo>(LocalServerListPath);
                }
                else
                {
                    var url = GetCfgInfoUrl(SERVER_LIST_URL_KEY);
                    if (!String.IsNullOrEmpty(url))
                    {
                        DownloadMgr.Instance.DownloadFile(url, ServerListPath, ServerListPath + "bak");
                    }
                    servers = LoadXML<ServerInfo>(ServerListPath);
                }

                if (servers.Count != 0)
                    ServersList = servers;
                for (int i = 0; i < ServersList.Count; i++)
                {
                    if (ServersList[i].id == Instance.SelectedServer)
                    {
                        SelectedServerIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }
        }

        public static void SaveConfig()
        {
#if UNITY_IPHONE
            SaveXML(Instance, ConfigPath);
#else
            SaveXMLList(ConfigPath, new List<LocalSetting>() { Instance });
#endif
        }

        public static void SetConfig()
        {
            System.Diagnostics.Process.Start("Explorer.exe", Application.persistentDataPath.Replace('/', '\\'));
        }

        public static void SaveCfgInfo()
        {
            SaveXMLList(CfgPath, CfgInfo, "url");
        }

        public static string GetCfgInfoUrl(String key)
        {
            string result = "";
            foreach (var item in CfgInfo)
            {
                if (item.name == key)
                {
                    result = item.url;
                    break;
                }
            }
            return result;
        }

        public static string GetActivateKeyUrl(UInt64 dbid, string key)
        {
            ServerInfo info = GetSelectedServerInfo();
            //"http://192.168.200.102/cgi-bin/card?"
            //Debug.LogError(GetCfgInfoUrl(ACTIVATE_URL));
            return string.Format(ATIVATE_URL_TEMPLATE, GetCfgInfoUrl(ACTIVATE_URL), info.ip, dbid + "", key);
        }

        #endregion

        #region 私有方法

        private static void SaveXMLList<T>(string path, List<T> data, string attrName = "record")
        {
            var root = new System.Security.SecurityElement("root");
            var i = 0;
            var props = typeof(T).GetProperties();
            foreach (var item in data)
            {
                var xml = new System.Security.SecurityElement(attrName);
                foreach (var prop in props)
                {
                    var type = prop.PropertyType;
                    String result = String.Empty;
                    object obj = prop.GetGetMethod().Invoke(item, null);
                    //var obj = prop.GetValue(item, null);
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        result = typeof(Utils).GetMethod("PackMap")
                        .MakeGenericMethod(type.GetGenericArguments())
                        .Invoke(null, new object[] { obj, ':', ',' }).ToString();
                    }
                    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        result = typeof(Utils).GetMethod("PackList")
                        .MakeGenericMethod(type.GetGenericArguments())
                        .Invoke(null, new object[] { obj, ',' }).ToString();
                    }
                    else
                    {
                        result = obj.ToString();
                    }
                    xml.AddChild(new System.Security.SecurityElement(prop.Name, result));
                }
                root.AddChild(xml);
                i++;
            }
            XMLParser.SaveText(path, root.ToString());
        }

        private static void InitConfig()
        {
            if (File.Exists(ConfigPath))
                File.Delete(ConfigPath);
            SaveConfig();
        }

        private static void LoadConfig()
        {
            try
            {
                var instance = LoadXML<LocalSetting>(ConfigPath);
                if (instance == null || instance.Count == 0)
                    InitConfig();
                else
                    Instance = instance[0];
            }
            catch (Exception ex)
            {
                InitConfig();
                LoggerHelper.Except(ex);
            }
        }

        public static bool LoadCfgInfo()
        {
            string cfgStr;
            if (File.Exists(CfgPath))
            {
                cfgStr = Utils.LoadFile(CfgPath);
            }
            else
            {
                var cfgUrl = Utils.LoadResource(Utils.GetFileNameWithoutExtention(CFG_FILE));
                cfgStr = DownloadMgr.Instance.DownLoadText(cfgUrl);
            }
            CfgInfo = LoadXMLText<CfgInfo>(cfgStr);
            return CfgInfo != null && CfgInfo.Count > 0 ? true : false;
        }

        private static List<T> LoadXML<T>(string path)
        {
            var text = Utils.LoadFile(path);
            return LoadXMLText<T>(text);
        }

        private static List<T> LoadXMLText<T>(string text)
        {
            List<T> list = new List<T>();
            try
            {
                if (String.IsNullOrEmpty(text))
                {
                    return list;
                }
                Type type = typeof(T);
                var xml = XMLParser.LoadXML(text);
                Dictionary<Int32, Dictionary<String, String>> map = XMLParser.LoadIntMap(xml, text);
                var props = type.GetProperties(~System.Reflection.BindingFlags.Static);
                foreach (var item in map)
                {
                    var obj = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                    foreach (var prop in props)
                    {
                        if (prop.Name == "id")
                            prop.SetValue(obj, item.Key, null);
                        else
                            try
                            {
                                if (item.Value.ContainsKey(prop.Name))
                                {
                                    var value = Utils.GetValue(item.Value[prop.Name], prop.PropertyType);
                                    prop.SetValue(obj, value, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                LoggerHelper.Debug("LoadXML error: " + item.Value[prop.Name] + " " + prop.PropertyType);
                                LoggerHelper.Except(ex);
                            }
                    }
                    list.Add((T)obj);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
                LoggerHelper.Error("error text: \n" + text);
            }
            return list;
        }
#if UNITY_IPHONE
		private static void SaveXML(object config, string path)
		{
			var root = new System.Security.SecurityElement("root");
			root.AddChild(new System.Security.SecurityElement("record"));
			var xml = root.Children[0] as System.Security.SecurityElement;
			var props = config.GetType().GetProperties();
			foreach (var item in props)
			{
				if (item.Name.Contains("GuideTimes"))
				{
					//dictonary
					var temp = item.GetGetMethod().Invoke(config, null);
					string value="";
					foreach(var v in temp as Dictionary<ulong,string>)
					{
						value=value+v.Key.ToString()+":"+v.Value+",";
					}
					xml.AddChild(new System.Security.SecurityElement(item.Name, value));
				}
				else
				{
					var value = item.GetGetMethod().Invoke(config, null);
					xml.AddChild(new System.Security.SecurityElement(item.Name, value.ToString()));
				}
			}
			XMLParser.SaveText(path, root.ToString());
		}
#endif
        #endregion

        //private static T LoadLuaTable<T>(string path)
        //{
        //    T t = default(T);
        //    //Type type = typeof(T);
        //    var text = Utils.LoadByteFile(path);
        //    if (text == null || text.Length == 0)
        //    {
        //        return t;
        //    }
        //    LuaTable lt;
        //    if (RPC.Utils.ParseLuaTable(text, out lt))
        //    {
        //        object result;
        //        if (Utils.ParseLuaTable(lt, typeof(T), out result))
        //            t = (T)result;
        //    }
        //    return t;
        //}

        //private static void SaveLuaTable(object config, string path)
        //{
        //    LuaTable value;
        //    RPC.Utils.PackLuaTable(config, out value);
        //    var s = RPC.Utils.PackLuaTable(value);
        //    XMLParser.SaveText(path, s);
        //}
    }

    #region 配置用类
    // [Flags]
    public enum ServerType : int
    {
        Hot = 1,
        Normal = 2,
        Close = 3,
        Maintain = 4,
        Recommend = 5
    }
    public class ServerInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public int flag { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string text { get; set; }


        public string GetInfo()
        {
            if (text != null && text != string.Empty)
            {
                return text;
            }
            if (flag == (int)ServerType.Close)
            {
                return "server close";
            }
            if (flag == (int)ServerType.Maintain)
            {
                return "maintaining...";
            }
            return string.Empty;

        }
        public bool CanLogin()
        {
            return (!(flag == (int)ServerType.Close)
                   || (flag == (int)ServerType.Maintain));
        }
    }

    public class CfgInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }




    public class LocalSetting
    {
        private int m_synCreateRequestCount = 5;

        public int id { get; set; }
        public int SelectedServer { get; set; }
        public string Passport { get; set; }
        public string Password { get; set; }
        public bool IsSavePassport { get; set; }
        public bool IsAutoLogin { get; set; }
        public bool IsDragMove { get; set; }
        public bool HasUploadInfo { get; set; }
        public int PlayerCountInScreen { get; set; }
        public float SoundVolume { get; set; }
        public float MusicVolume { get; set; }
        public int GraphicQuality { get; set; }

        public int LastMap { get; set; }

        public Dictionary<ulong, String> GuideTimes { get; set; }
        public bool IsShowGoldMetallurgyTipDialog { get; set; }
        public int GoldMetallurgyTipDialogDisableDay { get; set; }
        public bool IsShowBuyEnergyTipDialog { get; set; }
        public int BuyEnergyTipDialogDisableDay { get; set; }
        public bool IsShowArenaRevengeTipDialog { get; set; }
        public int ArenaRevengeTipDialogDisableDay { get; set; }
        public bool IsShowUpgradePowerTipDialog { get; set; }
        public int UpgradePowerTipDialogDisableDay { get; set; }
        public bool IsShowUpgradeDragonTipDialog { get; set; }
        public int UpgradeDragonTipDialogDisableDay { get; set; }
        /// <summary>
        /// 同时加载CreateRequest最大值
        /// </summary>
        public int SynCreateRequestCount
        {
            get { return m_synCreateRequestCount; }
            set
            {
                if (value > 0)
                    m_synCreateRequestCount = value;
            }
        }

        public string PassportSeenNotice { get; set; }
        public string noticeMd5 { get; set; }

        public string SelectedCharacter { get; set; } //password_index
        public LocalSetting()
        {
            PassportSeenNotice = "";
            noticeMd5 = "";
            Passport = "";
            Password = "";
            IsDragMove = true;
            PlayerCountInScreen = 10;
            SoundVolume = 0.5f;
            MusicVolume = 0.3f;
            GuideTimes = new Dictionary<ulong, String>();
            IsShowGoldMetallurgyTipDialog = true;
            GoldMetallurgyTipDialogDisableDay = 0;
            IsShowBuyEnergyTipDialog = true;
            BuyEnergyTipDialogDisableDay = 0;
            IsShowArenaRevengeTipDialog = true;
            ArenaRevengeTipDialogDisableDay = 0;
            IsShowUpgradePowerTipDialog = true;
            UpgradePowerTipDialogDisableDay = 0;
            IsShowUpgradeDragonTipDialog = true;
            UpgradeDragonTipDialogDisableDay = 0;
            SelectedCharacter = "_0";
            GraphicQuality = 2;
        }
    }
}
    #endregion