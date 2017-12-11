#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VersionManager
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.5.27
// 模块描述：版本管理。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using UnityEngine;
using Mogo.Util;

/// <summary>
/// 版本管理。
/// </summary>
public class VersionManager
{
    //public VersionCodeInfo ServerVersion { get; private set; }
    //public VersionCodeInfo LocalVersion { get; private set; }
    public VersionManagerInfo ServerVersion { get; private set; }
    public VersionManagerInfo LocalVersion { get; private set; }

    public string defContent = "";
    public byte[] defContentBytes { get; private set; }

    public static VersionManager Instance { get; set; }

    //是否平台的更新下载,只更新apk
    public readonly bool IsPlatformUpdate = false;
    private VersionManager() { }

    static VersionManager()
    {
        Instance = new VersionManager();
    }

    public void Init()
    {
        Mogo.Util.LoggerHelper.Debug("VersionManager Init");
        AddListeners();
    }

    public void AddListeners()
    {
        EventDispatcher.AddEventListener<string>(VersionEvent.AddMD5Content, AddMD5Content);
        EventDispatcher.AddEventListener(VersionEvent.GetContentMD5, GetContentMD5);
    }

    public void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<string>(VersionEvent.AddMD5Content, AddMD5Content);
        EventDispatcher.RemoveEventListener(VersionEvent.GetContentMD5, GetContentMD5);
    }

    /// <summary>
    /// 获取总的Content
    /// </summary>
    /// <param name="newContent">要计算的Content</param>
    private void AddMD5Content(string newContent)
    {
        Mogo.Util.LoggerHelper.Debug("Adding MD5 Content");
        defContent += newContent;
    }

    /// <summary>
    /// 获取MD5
    /// </summary>
    private void GetContentMD5()
    {
#if UNITY_IPHONE
		//use native md5 interface
		defContentBytes =IOSUtils.CreateMD5(Encoding.UTF8.GetBytes(defContent));
#else
        defContentBytes = Mogo.Util.Utils.CreateMD5(Encoding.UTF8.GetBytes(defContent));
#endif
    }

    public void LoadLocalVersion()
    {
        if (File.Exists(SystemConfig.VersionPath))
        {
            var ver = Utils.LoadFile(SystemConfig.VersionPath);
            LocalVersion = GetVersionInXML(ver);
            var programVer = Resources.Load(SystemConfig.VERSION_URL_KEY) as TextAsset;
            if (programVer && !String.IsNullOrEmpty(programVer.text))
                LocalVersion.ProgramVersionInfo = GetVersionInXML(programVer.text).ProgramVersionInfo;
            LoggerHelper.Info("program version : " + LocalVersion.ProgramVersion + " resource version :" + LocalVersion.ResouceVersion);
        }
        else
        {
            LocalVersion = new VersionManagerInfo();
            LoggerHelper.Info("cannot find local version,export from streaming assets");
            var ver = Resources.Load(SystemConfig.VERSION_URL_KEY) as TextAsset;
            if (ver != null)
                XMLParser.SaveText(SystemConfig.VersionPath, ver.text);
        }
    }

    public void ModalCheckNetwork(Action check)
    {
        LoggerHelper.Debug("----------------ModalCheckNetwork----ModalCheckNetwork----ModalCheckNetwork-----------------");
        //无网络的情况不判断了
        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            LoggerHelper.Debug("3G network");
            //3G网络，弹窗口，确定下载还是退出
            System.Collections.Generic.Dictionary<int, DefaultLanguageData> languageData = DefaultUI.dataMap;
            ForwardLoadingMsgBoxLib.Instance.ShowMsgBox(languageData[6].content, languageData[7].content, languageData[5].content, (confirm) =>
            {
                if (confirm)
                {
                    LoggerHelper.Debug("3G network confirm");
                    ForwardLoadingMsgBoxLib.Instance.Hide();
                    check();
                }
                else
                {
                    LoggerHelper.Debug("3G network cancel");
                    ForwardLoadingMsgBoxLib.Instance.Hide();
                    Application.Quit();
                }
            });
        }
        else
        {
            check();
            //LoggerHelper.Debug("--------------wifi------wifi----------wifi-------------");
            //System.Collections.Generic.Dictionary<int, DefaultLanguageData> languageData = DefaultUI.dataMap;
            //ForwardLoadingMsgBoxLib.Instance.ShowMsgBox(languageData[6].content, languageData[7].content, languageData[5].content, (confirm) =>
            //{
            //    if (confirm)
            //    {
            //        LoggerHelper.Debug("wifi network confirm");
            //        ForwardLoadingMsgBoxLib.Instance.Hide();
            //        check();
            //    }
            //    else
            //    {
            //        LoggerHelper.Debug("wifi network cancel");
            //        ForwardLoadingMsgBoxLib.Instance.Hide();
            //        Application.Quit();
            //    }
            //});
        }
    }
    public void BeforeCheck(Action<bool> AsynResult, Action OnError)
    {
        //阻塞，直到超时问题解决
        var checkTimeout = new CheckTimeout();
        checkTimeout.AsynIsNetworkTimeout((success) =>
        {
            if (success)
            {
                DownloadMgr.Instance.AsynDownLoadText(SystemConfig.GetCfgInfoUrl(SystemConfig.VERSION_URL_KEY),
                    (serverVersion) =>
                    {
                        if (File.Exists(SystemConfig.ServerVersionPath))//增加本地版本检查
                        {
                            serverVersion = Utils.LoadFile(SystemConfig.ServerVersionPath);
                            LoggerHelper.Info("serverVersion exist:\n" + serverVersion);
                        }
                        ServerVersion = GetVersionInXML(serverVersion);
                        if (ServerVersion.IsDefault())
                        {
                            if (OnError != null)
                                OnError();
                            return;
                        }
                        Mogo.Util.LoggerHelper.Debug("服务器程序版本: " + ServerVersion.ProgramVersionInfo);
                        Mogo.Util.LoggerHelper.Debug("服务器资源版本: " + ServerVersion.ResouceVersionInfo);
                        Mogo.Util.LoggerHelper.Debug("服务器包列表: " + ServerVersion.PackageList);
                        Mogo.Util.LoggerHelper.Debug("服务器包地址: " + ServerVersion.PackageUrl);
                        Mogo.Util.LoggerHelper.Debug("服务器Apk地址: " + ServerVersion.ApkUrl);
                        Mogo.Util.LoggerHelper.Debug("服务器md5地址: " + ServerVersion.PackageMd5List);
                        var compareProgramVersion = ServerVersion.ProgramVersionInfo.Compare(LocalVersion.ProgramVersionInfo) > 0;//服务程序版本号比本地版本号大
                        var compareResourceVersion = ServerVersion.ResouceVersionInfo.Compare(LocalVersion.ResouceVersionInfo) > 0;//服务器资源版本比本地高
                        AsynResult(compareProgramVersion || compareResourceVersion);
                    },
                    OnError);
            }
            else
            {
                if (OnError != null)
                    OnError();
            }
        });
        //CheckNetworkTimeout();
        //var serverVersion = DownloadMgr.Instance.DownLoadText(SystemConfig.GetCfgInfoUrl(SystemConfig.VERSION_URL_KEY));
        //{
        //    ServerVersion = GetVersionInXML(serverVersion);
        //    Mogo.Util.LoggerHelper.Debug("服务器程序版本: " + ServerVersion.ProgramVersionInfo);
        //    Mogo.Util.LoggerHelper.Debug("服务器资源版本: " + ServerVersion.ResouceVersionInfo);
        //    Mogo.Util.LoggerHelper.Debug("服务器包列表: " + ServerVersion.PackageList);
        //    Mogo.Util.LoggerHelper.Debug("服务器包地址: " + ServerVersion.PackageUrl);
        //    Mogo.Util.LoggerHelper.Debug("服务器Apk地址: " + ServerVersion.ApkUrl);
        //    Mogo.Util.LoggerHelper.Debug("服务器md5地址: " + ServerVersion.PackageMd5List);
        //}
        //var compareProgramVersion = ServerVersion.ProgramVersionInfo.Compare(LocalVersion.ProgramVersionInfo) > 0;//服务程序版本号比本地版本号大
        //var compareResourceVersion = ServerVersion.ResouceVersionInfo.Compare(LocalVersion.ResouceVersionInfo) > 0;//服务器资源版本比本地高
        //return compareProgramVersion || compareResourceVersion;
    }
    public bool CheckAndDownload(Action<bool> fileDecompress, Action<int, int, string> taskProgress, Action<int, long, long> progress, Action finished, Action<Exception> error)
    {
        //更新apk
        if (ServerVersion.ProgramVersionInfo.Compare(LocalVersion.ProgramVersionInfo) > 0)
        {
            LoggerHelper.Debug("服务器apk版本高，下载apk");
            var fileUrl = ServerVersion.ApkUrl; //SystemConfig.GetCfgInfoUrl(SystemConfig.APK_URL_KEY);
            //   /sdcard/
            string apkPath = SystemConfig.ResourceFolder;
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
                //apkPath = "/sdcard/";
                apkPath = Application.persistentDataPath + "/temp/";// +"/temp/";
#elif UNITY_IPHONE
            apkPath=Application.persistentDataPath+"/temp/";
#endif
            if (!Directory.Exists(apkPath))
                Directory.CreateDirectory(apkPath);
            var localFile = String.Concat(apkPath, Utils.GetFileName(fileUrl));
            Action onFinish = () =>
            {
                //LoggerHelper.Debug("APK下载成功，安装");
                //DriverLib.Invoke(() => { InstallApk(localFile); });
            };
            if (!IsPlatformUpdate)
                AsynDownloadApk(taskProgress, localFile, fileUrl, progress, onFinish, error);
            else
            {
                SetPlatformUpdateCallback();
                //如果使用平台更新,先导出后下载
                ExportStreamingAssetWhenDownloadApk(PlatformUpdate);
            }
            return true;
        }

        //更新资源
        if (ServerVersion.ResouceVersionInfo.Compare(LocalVersion.ResouceVersionInfo) > 0)//服务资源版本号比本地版本号大
        {
            Mogo.Util.LoggerHelper.Debug("服务器资源版本号比本地版本号大");
            AsynDownloadUpdatePackage(fileDecompress, taskProgress, progress, finished, error);
            //DownloadUpdatePackage();
            return true;
        }

        if (finished != null)
            finished();
        return false;
    }
    //设置平台更新的回调
    void SetPlatformUpdateCallback()
    {
#if UNITY_ANDROID
        LoggerHelper.Info("Init PlatformUpateCallback");
        DriverLib.Instance.gameObject.AddComponent("PlatformUpdateCallback");
        var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("setUpdateCallBack", "Driver");
#endif
    }

    /// <summary>
    /// 检查版本。
    /// </summary>
    public void CheckVersion(Action<bool> fileDecompress, Action<int, int, string> taskProgress, Action<int, long, long> progress, Action finished, Action<Exception> error)
    {
        BeforeCheck((result) =>
            {
                if (result)
                {
                    ModalCheckNetwork(() => CheckAndDownload(fileDecompress, taskProgress, progress, finished, error));
                }
                else
                {
                    LoggerHelper.Debug("不需要更新apk和pkg");
                    if (finished != null)
                        finished();
                }
            },
            () => { error(new Exception("download version file time out.")); });
    }
    //使用平台更新下载apk时导出资源，先导出资源再下载apk
    internal void ExportStreamingAssetWhenDownloadApk(Action finished)
    {
        ResourceIndexInfo.Instance.Init(Application.streamingAssetsPath + "/ResourceIndexInfo.txt", () =>
        {
            LoggerHelper.Debug("资源索引信息:" + ResourceIndexInfo.Instance.GetLeftFilePathes().Length);
            if (ResourceIndexInfo.Instance.GetLeftFilePathes().Length != 0)
            {
                //下载apk时导出资源
                var go = new StreamingAssetManager
                {
                    AllFinished = finished
                };
                go.UpdateApkExport();
            }
            else
            {
                LoggerHelper.Debug("没有streamingAssets,不需要导出");
                finished();
            }
        });
    }
    /// <summary>
    /// 获取更新包名称。
    /// </summary>
    /// <param name="currentVersion"></param>
    /// <param name="newVersion"></param>
    /// <returns></returns>
    public string GetPackageName(string currentVersion, string newVersion)
    {
        return string.Concat("package", currentVersion, "-", newVersion, ".pkg");
    }

    private void AsynDownloadApk(Action<int, int, string> taskProgress, string fileName, string url, Action<int, long, long> progress, Action finished, Action<Exception> error)
    {
        var task = new DownloadTask
        {
            FileName = fileName,
            Url = url,
            Finished = finished,
            Error = error,
            TotalProgress = progress,
            MD5 = ServerVersion.ApkMd5
        };
        LoggerHelper.Info("down load apk & md5: " + url + " " + ServerVersion.ApkMd5);
        //LoggerHelper.Debug("APK保存的MD5值: " + ServerVersion.ApkMd5);
        var tasks = new List<DownloadTask> { task };
        DownloadMgr.Instance.tasks = tasks;
        //添加taskProgress的回调
        Action<int, int, string> TaskProgress = (total, current, filename) =>
        {
            if (taskProgress != null)
                taskProgress(total, current, filename);
        };
        DownloadMgr.Instance.TaskProgress = TaskProgress;
        StreamingAssetManager go = null;
        ResourceIndexInfo.Instance.Init(Application.streamingAssetsPath + "/ResourceIndexInfo.txt", () =>
            {
                LoggerHelper.Info("资源索引信息:" + ResourceIndexInfo.Instance.GetLeftFilePathes().Length);
                if (ResourceIndexInfo.Instance.GetLeftFilePathes().Length == 0)
                {
                    go = new StreamingAssetManager();
                    go.UpdateProgress = false;
                    go.ApkFinished = true;
                }
                else
                {
                    //下载apk时导出资源
                    go = new StreamingAssetManager { UpdateProgress = false };
                    go.AllFinished = () =>
                    {
                        LoggerHelper.Debug("打开资源导出完成的标识11ApkFinished");
                        go.ApkFinished = true;
                    };
                }
            });
        DownloadMgr.Instance.AllDownloadFinished = () =>
        {
            LoggerHelper.Info("APK download finish, wait for export finish:" + fileName);
            if (go != null)
            {
                go.UpdateProgress = true;
                LoggerHelper.Debug("打开导出进度显示:" + go.ApkFinished);
                //先判断资源导出是否完成，再安装apk,没完成则等待完成
                while (!go.ApkFinished)
                {
                    System.Threading.Thread.Sleep(500);
                }
                LoggerHelper.Info("APK and export download finish.");
                go = null;
            }
            DriverLib.Invoke(() =>
            {
#if UNITY_IPHONE
            Action<bool> InstallIpa = (confirm) =>
            {
                if (confirm)
                {
                    //IOSUtils.UpdateLoader(fileName);
                }
                else
                    Application.Quit();
            };
				ForwardLoadingMsgBoxLib.Instance.ShowMsgBox(DefaultUI.dataMap[11].content,
				                                         DefaultUI.dataMap[7].content,
				                                         DefaultUI.dataMap[12].content,
				                                         InstallIpa);
#else
                InstallApk(fileName);
#endif
            });
            if (finished != null)
                finished();
            LoggerHelper.Debug("apk安装成功");
        };
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        while (go == null)
        {
            System.Threading.Thread.Sleep(50);
            if (stopwatch.ElapsedMilliseconds > 3000)
                break;
            //LoggerHelper.Debug("阻塞直到StreamingAssetManager不为null或超时3秒");
        }
        stopwatch.Stop();
        //开始下载apk同时导出资源文件
        if (go != null && !go.ApkFinished)
        {
            LoggerHelper.Debug("apk下载同时导出资源");
            go.UpdateApkExport();
        }
        DownloadMgr.Instance.CheckDownLoadList();

    }

    private void InstallApk(string apkPath)
    {
        LoggerHelper.Info("Call Install apk: " + apkPath);
        Application.OpenURL(apkPath);
        Application.Quit();
        //#if UNITY_ANDROID
        //        LoggerHelper.Info("UNITY_ANDROID Install apk: " + apkPath);
        //        var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //        var mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //        mainActivity.Call("InstallApk", apkPath);
        //        Application.Quit();
        //        //TimerHeap.AddTimer(1000, 0, Application.Quit);
        //#endif
    }

    public void PlatformUpdate()
    {
#if UNITY_ANDROID
        LoggerHelper.Debug("安卓上更新apk");
        var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("updateVersion");
#endif
    }

    public void OpenUrl()
    {
#if UNITY_ANDROID
        LoggerHelper.Debug("打开一个url");
        var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("OpenUrl");
#endif
    }

    public VersionManagerInfo GetVersionInXML(string xml)
    {
        var children = XMLParser.LoadXML(xml);
        if (children != null && children.Children != null && children.Children.Count != 0)
        {
            var result = new VersionManagerInfo();
            var props = typeof(VersionManagerInfo).GetProperties();
            foreach (SecurityElement item in children.Children)
            {
                var prop = props.FirstOrDefault(t => t.Name == item.Tag);
                if (prop != null)
                    prop.SetValue(result, item.Text, null);
            }
            return result;
        }
        else
            return new VersionManagerInfo();
    }

    private void SaveVersion(VersionManagerInfo version)
    {
        var props = typeof(VersionManagerInfo).GetProperties();
        var root = new System.Security.SecurityElement("root");
        foreach (var item in props)
        {
            root.AddChild(new System.Security.SecurityElement(item.Name, item.GetGetMethod().Invoke(version, null) as String));
        }
        XMLParser.SaveText(SystemConfig.VersionPath, root.ToString());
    }

    private void AsynDownloadUpdatePackage(Action<bool> fileDecompress, Action<int, int, string> taskProgress, Action<int, long, long> progress, Action finished, Action<Exception> error)
    {
        LoggerHelper.Debug("下载包列表");
        //下载packagemd5中对应的包列表和包对应的md5码
        //var packageList = DownloadPackageList();
        //var downloadList = new List<KeyValuePair<String, String>>();
        //var lastVersion = LocalVersion.ResouceVersionInfo;
        //foreach (var item in packageList)
        //{
        //    if (item.Compare(LocalVersion.ResouceVersionInfo) > 0 && item.Compare(ServerVersion.ResouceVersionInfo) <= 0)
        //    {
        //        downloadList.Add(new KeyValuePair<string, string>(item.ToString(), GetPackageName(lastVersion.ToString(), item.ToString())));
        //        LoggerHelper.Debug(item.ToString() + "----更新包----" + GetPackageName(lastVersion.ToString(), item.ToString()));
        //    }
        //    lastVersion = item;
        //}
        //var packageUrl = ServerVersion.PackageUrl;//SystemConfig.GetCfgInfoUrl(SystemConfig.PACKAGE_URL_KEY);

        DownloadPackageInfoList((packageInfoList) =>
        {
            var downloadList = (from packageInfo in packageInfoList
                                where packageInfo.LowVersion.Compare(LocalVersion.ResouceVersionInfo) >= 0 && packageInfo.HighVersion.Compare(ServerVersion.ResouceVersionInfo) <= 0
                                select new KeyValuePair<string, string>(packageInfo.HighVersion.ToString(), packageInfo.Name)).ToList();
            var packageUrl = ServerVersion.PackageUrl;
            if (downloadList.Count != 0)
            {
                LoggerHelper.Debug("开始下载包列表");
                DownDownloadPackageList(fileDecompress, packageUrl, downloadList, taskProgress, progress, finished, error);
            }
            else
            {
                LoggerHelper.Debug("更新包数目为0");
                if (finished != null)
                    finished();
            }

        }, () => { error(new Exception("DownloadPackageInfoList error.")); });
    }
    private void DownDownloadPackageList(Action<bool> fileDecompress, string packageUrl, List<KeyValuePair<String, String>> downloadList, Action<int, int, string> taskProgress, Action<int, long, long> progress, Action finished, Action<Exception> error)
    {
        var allTask = new List<DownloadTask>();
        //收集downloadlist，生成所有下载任务
        for (int i = 0; i < downloadList.Count; i++)
        {
            LoggerHelper.Debug("收集数据包，生成任务:" + i);
            KeyValuePair<String, String> kvp = downloadList[i];
            LoggerHelper.Debug("下载文件名：" + kvp.Value);
            String localFile = String.Concat(SystemConfig.ResourceFolder, kvp.Value);

            Action OnDownloadFinished = () =>
            {
                LoggerHelper.Debug("下载完成，进入完成回调");
                LoggerHelper.Debug("本地文件：" + File.Exists(localFile));
                if (File.Exists(localFile))
                    FileAccessManager.DecompressFile(localFile);
                LoggerHelper.Debug("解压完成，保存版本信息到version.xml");
                if (File.Exists(localFile))
                    File.Delete(localFile);
                LocalVersion.ResouceVersionInfo = new VersionCodeInfo(kvp.Key);
                SaveVersion(LocalVersion);
                //if (taskProgress != null)
                //    taskProgress(downloadList.Count, i + 1);
            };
            String fileUrl = String.Concat(packageUrl, kvp.Value);
            var task = new DownloadTask
            {
                FileName = localFile,
                Url = fileUrl,
                Finished = OnDownloadFinished,
                Error = error,
                TotalProgress = progress
            };
            //task.Error = erroract;
            string fileNoEXtension = kvp.Value;// task.FileName.LastIndexOf('/') != 0 ? task.FileName.Substring(task.FileName.LastIndexOf('/') + 1) : task.FileName;
            LoggerHelper.Debug(task.FileName + "::fileNoEXtension::" + fileNoEXtension);
            if (ServerVersion.PackageMD5Dic.ContainsKey(fileNoEXtension))
            {
                task.MD5 = ServerVersion.PackageMD5Dic[fileNoEXtension];
                allTask.Add(task);
            }
            else
            {
                error(new Exception("down pkg not exit :" + fileNoEXtension));
                return;
                //LoggerHelper.Error("down pkg not exit :" + fileNoEXtension);
            }
        }
        //全部下载完成的回调
        Action AllFinished = () =>
        {
            LoggerHelper.Debug("更新包全部下载完成");
            finished();
        };
        //添加taskProgress的回调
        Action<int, int, string> TaskProgress = (total, current, filename) =>
        {
            if (taskProgress != null)
                taskProgress(total, current, filename);
        };
        //添加文件解压的回调函数
        Action<bool> filedecompress = (decompress) =>
       {
           if (fileDecompress != null)
               fileDecompress(decompress);
       };
        //所有任务收集好了,开启下载
        LoggerHelper.Debug("所有任务收集好了,开启下载");
        DownloadMgr.Instance.tasks = allTask;
        DownloadMgr.Instance.AllDownloadFinished = AllFinished;
        DownloadMgr.Instance.TaskProgress = TaskProgress;
        DownloadMgr.Instance.FileDecompress = filedecompress;
        DownloadMgr.Instance.CheckDownLoadList();
    }

    //获得包信息列表
    private void DownloadPackageInfoList(Action<IEnumerable<PackageInfo>> AsynResult, Action OnError)
    {
        DownloadMgr.Instance.AsynDownLoadText(ServerVersion.PackageMd5List, (content) =>
        {
            var xml = XMLParser.LoadXML(content);
            if (xml == null)
            {
                if (OnError != null)
                    OnError();
            }
            else
            {
                var result = new List<PackageInfo>();
                foreach (var t in xml.Children)
                {
                    var info = new PackageInfo();
                    var item = t as SecurityElement;
                    var packagename = item.Attribute("n");
                    //包名
                    info.Name = packagename;
                    var version = packagename.Substring(7, packagename.Length - 11);
                    var firstversion = version.Substring(0, version.IndexOf('-'));
                    //低版本
                    info.LowVersion = new VersionCodeInfo(firstversion);
                    var endversion = version.Substring(version.IndexOf('-') + 1);
                    //高版本
                    info.HighVersion = new VersionCodeInfo(endversion);
                    //Md5值
                    info.Md5 = item.Text;
                    result.Add(info);
                    ServerVersion.PackageMD5Dic.Add(info.Name, info.Md5);
                }
                AsynResult(result);
            }
        }, OnError);
    }

    #region bak

    //private void DownloadUpdatePackage()
    //{
    //    var pl = DownloadPackageList();
    //    var downloadList = new List<KeyValuePair<String, String>>();
    //    var lastVersion = LocalVersion;
    //    foreach (var item in pl)
    //    {
    //        if (item.Compare(LocalVersion) > 0 && item.Compare(ServerVersion) <= 0)
    //        {
    //            downloadList.Add(new KeyValuePair<string, string>(item.ToString(), GetPackageName(lastVersion.ToString(), item.ToString())));
    //        }
    //        lastVersion = item;
    //    }

    //    var packageUrl = SystemConfig.GetCfgInfoUrl(SystemConfig.PACKAGE_URL_KEY);
    //    var sw = new System.Diagnostics.Stopwatch();
    //    var swDec = new System.Diagnostics.Stopwatch();
    //    sw.Start();
    //    foreach (var item in downloadList)
    //    {
    //        var localFile = String.Concat(SystemConfig.ResourceFolder, item.Value);
    //        LoggerHelper.Debug("localFile: " + localFile);
    //        if (!File.Exists(localFile))
    //        {
    //            var fileUrl = String.Concat(packageUrl, item.Value);
    //            var task = new DownloadTask();
    //            task.FileName = localFile;
    //            task.Url = fileUrl;
    //            DownloadMgr.Instance.DownloadFile(fileUrl, localFile, localFile + "bak");
    //            LoggerHelper.Debug("download: " + fileUrl + " " + sw.ElapsedMilliseconds);
    //        }
    //        swDec.Start();
    //        Utils.DecompressToMogoFile(localFile);
    //        swDec.Stop();
    //        LoggerHelper.Debug("DecompressToMogoFile: " + localFile + " " + sw.ElapsedMilliseconds);
    //        Mogo.Util.LoggerHelper.Debug(item.Value);
    //        if (File.Exists(localFile))
    //            File.Delete(localFile);
    //        SaveVersion(item.Key);
    //    }
    //    sw.Stop();
    //    LoggerHelper.Debug("download Stop: " + sw.ElapsedMilliseconds);
    //    LoggerHelper.Debug("Decompress time: " + swDec.ElapsedMilliseconds);
    //}
    ////获得包列表
    //private List<VersionCodeInfo> DownloadPackageList()
    //{
    //    var result = new List<VersionCodeInfo>();
    //    var content = DownloadMgr.Instance.DownLoadText(ServerVersion.PackageMd5List);
    //    var xml = XMLParser.LoadXML(content);
    //    if (xml != null)
    //    {
    //        foreach (object t in xml.Children)
    //        {
    //            var item = t as SecurityElement;

    //            String packagename = item.Attribute("n");
    //            LoggerHelper.Debug("package包名:" + packagename);
    //            String version = packagename.Substring(7, packagename.Length - 11);
    //            String firstversion = version.Substring(0, version.IndexOf('-'));
    //            LoggerHelper.Debug("低版本：" + firstversion);
    //            result.Add(new VersionCodeInfo(firstversion));
    //            string endversion = version.Substring(version.IndexOf('-') + 1);
    //            LoggerHelper.Debug("高版本：" + endversion);
    //            String packagemd5 = item.Text;
    //            LoggerHelper.Debug("MD5码：" + packagemd5);
    //            //添加package对应的md5码到字典中
    //            ServerVersion.PackageMD5Dic.Add(packagename, packagemd5);
    //            result.Add(new VersionCodeInfo(endversion));

    //            #region 注释

    //            ////如果是第一个，要取得前一个和后一个
    //            //if (i == 0)
    //            //{
    //            //    String packagename = item.Attribute("n");
    //            //    LoggerHelper.Debug("package包名:" + packagename);
    //            //    String version = packagename.Substring(7, packagename.Length - 11);
    //            //    String firstversion = version.Substring(0, version.IndexOf('-'));
    //            //    LoggerHelper.Debug("第一个的前一个版本：" + firstversion);
    //            //    result.Add(new VersionCodeInfo(firstversion));
    //            //    string endversion = version.Substring(version.IndexOf('-') + 1);
    //            //    LoggerHelper.Debug("第一个的后一个版本：" + endversion);
    //            //    String packagemd5 = item.Text;
    //            //    LoggerHelper.Debug("package的MD5码：" + packagemd5);
    //            //    //添加package对应的md5码到字典中
    //            //    ServerVersion.PackageMD5Dic.Add(packagename, packagemd5);
    //            //    result.Add(new VersionCodeInfo(endversion));
    //            //}
    //            //else
    //            //{
    //            //    String packagename = item.Attribute("n");
    //            //    LoggerHelper.Debug("package包名:" + packagename);
    //            //    String version = packagename.Substring(7, packagename.Length - 11);
    //            //    version = version.Substring(version.IndexOf('-') + 1);
    //            //    LoggerHelper.Debug("版本号：" + version);
    //            //    String packagemd5 = item.Text;
    //            //    LoggerHelper.Debug("package的MD5码：" + packagemd5);
    //            //    //添加package对应的md5码到字典中
    //            //    ServerVersion.PackageMD5Dic.Add(packagename, packagemd5);
    //            //    result.Add(new VersionCodeInfo(version));
    //            //}

    //            #endregion

    //        }
    //    }
    //    else
    //        LoggerHelper.Debug("XML为空");
    //    return result;
    //}

    //    //网络超时判断,超时返回true
    //    public bool IsNetworkTimeout()
    //    {
    //        bool bTimeout = true;
    //        for (int i = 0; i < 1; i++)
    //        {
    //            var cfgUrl = Utils.LoadResource(Utils.GetFileNameWithoutExtention(SystemConfig.CFG_FILE));
    //            string serverVersion = DownloadMgr.Instance.DownLoadText(cfgUrl);
    //            if (!String.IsNullOrEmpty(serverVersion))
    //            {
    //                bTimeout = false;
    //                break;
    //            }
    //        }
    //        return bTimeout;
    //    }
    //    //网络超时处理
    //    public void CheckNetworkTimeout()
    //    {
    //        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(13).content);//正在检查网络       
    //        //如果是超时的，暂时阻塞
    //        while (IsNetworkTimeout())
    //        {
    //            //如果已经弹出对话框继续等待
    //            if (!ForwardLoadingMsgBoxLib.Instance.IsHide())
    //            {
    //                //System.Threading.Thread.Sleep(100);
    //                continue;
    //            }
    //            LoggerHelper.Debug("网络超时，请设置网络");
    //            Dictionary<int, DefaultLanguageData> languageData = DefaultUI.dataMap;
    //            ForwardLoadingMsgBoxLib.Instance.ShowMsgBox(languageData[50].content, languageData[51].content, languageData[52].content, (isOk) =>
    //            {
    //                ForwardLoadingMsgBoxLib.Instance.Hide();
    //                if (isOk)
    //                {
    //                    //重试,啥也不做，循环会再次回来判断                    
    //                }
    //                else
    //                {
    //                    //设置网络
    //#if UNITY_ANDROID
    //                    if (Application.platform == RuntimePlatform.Android)
    //                    {
    //                        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //                        AndroidJavaObject m_mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
    //                        m_mainActivity.Call("gotoNetworkSetting");
    //                    }
    //#endif
    //                }
    //            });
    //        }
    //        //直到网络正常
    //        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(4).content);//数据读取中
    //    }

    //添加一个网络超时次数
    //static int TimeOutTimes = 0;
    //把CheckVersion分成两部分,第一部分获得服务器版本和本地进行比较，第二部分是需要更新再更新
    //在两部分之间弹出模态对话框,超时三次就退出

    #endregion
}

public class VersionManagerInfo
{
    public VersionCodeInfo ProgramVersionInfo;
    public VersionCodeInfo ResouceVersionInfo;
    public string ProgramVersion
    {
        get
        {
            return ProgramVersionInfo.ToString();
        }
        set
        {
            ProgramVersionInfo = new VersionCodeInfo(value);
        }
    }
    public string ResouceVersion
    {
        get
        {
            return ResouceVersionInfo.ToString();
        }
        set
        {
            ResouceVersionInfo = new VersionCodeInfo(value);
        }
    }
    public String PackageList { get; set; }
    public String PackageUrl { get; set; }
    public String ApkUrl { get; set; }
    public String ApkMd5 { get; set; }
    public String PackageMd5List { get; set; }//package的md5码
    //package包对应的md5码
    public Dictionary<String, String> PackageMD5Dic = new Dictionary<string, string>();
    public VersionManagerInfo()
    {
        ProgramVersionInfo = new VersionCodeInfo("0.0.0.1");
        ResouceVersionInfo = new VersionCodeInfo("0.0.0.0");
        PackageList = String.Empty;
        PackageUrl = String.Empty;
        ApkUrl = String.Empty;
    }
    public void ReadMd5FromXML(String packageMD5Content)
    {
        var xml = XMLParser.LoadXML(packageMD5Content);
        if (xml == null)
        {
            return;
        }
        foreach (SecurityElement item in xml.Children)
        {
            PackageMD5Dic[item.Attribute("n")] = item.Text;
        }
    }

    public bool IsDefault()
    {
        if (ProgramVersionInfo.Compare(new VersionCodeInfo("0.0.0.1")) == 0
            && ResouceVersionInfo.Compare(new VersionCodeInfo("0.0.0.0")) == 0
            && PackageList == String.Empty && PackageUrl == String.Empty && ApkUrl == String.Empty)
        {
            return true;
        }
        else
            return false;
    }

    public override string ToString()
    {
        return ResouceVersionInfo.ToString();
    }
}

/// <summary>
/// 从包列表取出来的单个包信息
/// </summary>
public class PackageInfo
{
    //包名
    public string Name;
    //低版本
    public VersionCodeInfo LowVersion;
    //高版本
    public VersionCodeInfo HighVersion;
    //md5值
    public string Md5;
}

/// <summary>
/// 版本号。
/// </summary>
public class VersionCodeInfo
{
    private List<int> m_tags = new List<int>();

    /// <summary>
    /// 构造函数。
    /// </summary>
    /// <param name="version">版本号字符串。</param>
    public VersionCodeInfo(String version)
    {
        if (string.IsNullOrEmpty(version))
            return;
        var versions = version.Split('.');
        for (int i = 0; i < versions.Length; i++)
        {
            int v;
            if (int.TryParse(versions[i], out v))
                m_tags.Add(v);
            else
                m_tags.Add(v);
        }
    }

    /// <summary>
    /// 获取比目前版本高一个版本的字符串。
    /// </summary>
    /// <returns></returns>
    public string GetUpperVersion()
    {
        var lastTag = m_tags[m_tags.Count - 1] + 1;
        var sb = new StringBuilder();
        for (int i = 0; i < m_tags.Count - 1; i++)
        {
            sb.AppendFormat("{0}.", m_tags[i]);
        }
        sb.Append(lastTag);
        return sb.ToString();
    }

    /// <summary>
    /// 获取比目前版本低一个版本的字符串。
    /// </summary>
    /// <returns></returns>
    public string GetLowerVersion()
    {
        var lastTag = m_tags[m_tags.Count - 1] - 1;
        var sb = new StringBuilder();
        for (int i = 0; i < m_tags.Count - 1; i++)
        {
            sb.AppendFormat("{0}.", m_tags[i]);
        }
        sb.Append(lastTag);
        return sb.ToString();
    }

    /// <summary>
    /// 返回无小数点版本号。
    /// </summary>
    /// <returns></returns>
    public string ToShortString()
    {
        var sb = new StringBuilder();
        foreach (var item in m_tags)
        {
            sb.Append(item);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 比较版本号，自己比参数大，返回1，比参数小，返回-1，相等返回0。
    /// </summary>
    /// <param name="info">比较版本号。</param>
    /// <returns>自己比参数大，返回1，比参数小，返回-1，相等返回0。</returns>
    public int Compare(VersionCodeInfo info)
    {
        var count = this.m_tags.Count < info.m_tags.Count ? this.m_tags.Count : info.m_tags.Count;
        for (int i = 0; i < count; i++)
        {
            if (this.m_tags[i] == info.m_tags[i])
                continue;
            else
                return this.m_tags[i] > info.m_tags[i] ? 1 : -1;
        }
        return 0;
    }

    /// <summary>
    /// 获取版本号字符串。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var item in m_tags)
        {
            sb.AppendFormat("{0}.", item);
        }
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
}

public class CheckTimeout
{
    public void AsynIsNetworkTimeout(Action<bool> AsynResult)
    {
        string networkUrl;
        if (File.Exists(SystemConfig.CfgPath))
        {
            var result = SystemConfig.LoadCfgInfo();
            networkUrl = SystemConfig.GetCfgInfoUrl(SystemConfig.VERSION_URL_KEY);
            LoggerHelper.Info("cfg exist. " + result + " networkUrl: " + networkUrl);
        }
        else
            networkUrl = Utils.LoadResource(Utils.GetFileNameWithoutExtention(SystemConfig.CFG_FILE));
        var counter = 0;
        TryAsynDownloadText(networkUrl, counter, AsynResult);
    }

    private void TryAsynDownloadText(string url, int counter, Action<bool> AsynResult)
    {
        DownloadMgr.Instance.AsynDownLoadText(url, (text) =>
        {
            if (!String.IsNullOrEmpty(text))
                AsynResult(true);
            else
                AsynResult(false);
        }, () =>
        {
            AsynResult(false);
        });
        //AsynDownloadText(cfgUrl, (success) =>
        //{
        //    if (success)
        //        AsynResult(true);
        //    else
        //    {
        //        counter++;
        //        if (counter < 1)
        //            TryAsynDownloadText(cfgUrl, counter, AsynResult);
        //        else
        //            AsynResult(false);
        //    }
        //});
    }

    //private void AsynDownloadText(string cfgUrl, Action<bool> AsynResult)
    //{
    //    Action asynDownloadText = () =>
    //    {
    //        string serverVersion = DownloadMgr.Instance.DownLoadText(cfgUrl);
    //        if (!String.IsNullOrEmpty(serverVersion))
    //            AsynResult(true);
    //        else
    //            AsynResult(false);
    //    };
    //    asynDownloadText.BeginInvoke(null, null);
    //}
}