#if UNITY_IPHONE
using UnityEngine;
using Mogo.Util;
using System;
using System.Collections;
using System.Net;
using System.IO;

public class Driver : MonoBehaviour
{
    public Action LevelWasLoaded;
    public static Driver Instance;
    void Start()
    {
        Instance = this;
        ClearRecentDataIfNeed();
        InitGlobalSetting();
        InitConfig();
        CheckVersion(InitGameData);
    }

    void InitConfig()
    {
        DefaultUI.InitLanguageInfo();
        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(4).content);//数据读取中…
        var loadCfgSuccess = SystemConfig.Init();
        VersionManager.Instance.Init();
        VersionManager.Instance.LoadLocalVersion();
        SystemConfig.LoadServerList();
        PluginCallback.Instance.ShowGlobleLoadingUI = MogoForwardLoadingUIManager.Instance.ShowGlobleLoadingUI;
        PluginCallback.Instance.SetLoadingStatus = MogoForwardLoadingUIManager.Instance.SetLoadingStatus;
        PluginCallback.Instance.SetLoadingStatusTip = MogoForwardLoadingUIManager.Instance.SetLoadingStatusTip;
        PluginCallback.Instance.ShowRetryMsgBox = GameObject.Find("MogoDefaultUI/DefaultUICamera2/Anchor").GetComponent<ForwardLoadingMsgBox>().ShowRetryMsgBox;
        PluginCallback.Instance.ShowMsgBox = GameObject.Find("MogoDefaultUI/DefaultUICamera2/Anchor").GetComponent<ForwardLoadingMsgBox>().ShowMsgBox;
        PluginCallback.Instance.Hide = GameObject.Find("MogoDefaultUI/DefaultUICamera2/Anchor").GetComponent<ForwardLoadingMsgBox>().Hide;
        PluginCallback.Instance.IsHide = GameObject.Find("MogoDefaultUI/DefaultUICamera2/Anchor").GetComponent<ForwardLoadingMsgBox>().IsHide;
    }

    void InitGameData()
    {
        InvokeRepeating("Tick", 1, 0.02f);
        UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(gameObject, "Assets/Plugins/IOS/IOSDriver.cs (42,9)", "MogoInitialize");
#if !UNITY_EDITOR
        gameObject.AddComponent<StreamingAssetCacheManager>();
        StreamingAssetCacheManager.Instance.Export();
#endif
    }

    void ClearRecentDataIfNeed()
    {
        UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
    }

    void InitGlobalSetting()
    {
#if !UNITY_EDITOR
        LoggerHelper.CurrentLogLevels = LogLevel.INFO | LogLevel.ERROR | LogLevel.CRITICAL | LogLevel.EXCEPT;// | LogLevel.WARNING 
#endif
        LoggerHelper.Debug("--------------------------------------InitGlobalSetting-----------------------------------------");
        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad(gameObject);
        GameObject.Find("MogoForwardLoadingUIPanel").AddComponent<MogoForwardLoadingUIManager>();
        gameObject.AddComponent<AudioListener>();
#if UNITY_EDITOR
        gameObject.AddComponent<PlatformSdkManager>();
#else
        gameObject.AddComponent<IOSSDKManager>();
#endif
    }

    void CheckVersion(Action callback)
    {
#if !UNITY_EDITOR
        ResourceIndexInfo.Instance.Init(Application.streamingAssetsPath + "/ResourceIndexInfo.txt", () =>
        {
            if (ResourceIndexInfo.Instance.Exist())
            {
                var localVer = Utils.LoadFile(SystemConfig.VersionPath);
                var localVersion = VersionManager.Instance.GetVersionInXML(localVer);
                var pkgVer = Resources.Load(SystemConfig.VERSION_URL_KEY) as TextAsset;
                var pkgVersion = VersionManager.Instance.GetVersionInXML(pkgVer.text);
                if (pkgVersion.ResouceVersionInfo.Compare(localVersion.ResouceVersionInfo) > 0)
                {
                    if (File.Exists(SystemConfig.VersionPath))
                        File.Delete(SystemConfig.VersionPath);
                    var mogoResroucesPath = SystemConfig.ResourceFolder.Substring(0, SystemConfig.ResourceFolder.Length - 1);
                    if (Directory.Exists(mogoResroucesPath))
                        Directory.Delete(mogoResroucesPath, true);
                    if (!File.Exists(SystemConfig.VersionPath))
                    {
                        var ver = Resources.Load(SystemConfig.VERSION_URL_KEY) as TextAsset;
                        if (ver != null)
                            XMLParser.SaveText(SystemConfig.VersionPath, ver.text);
                    }
                }
            }
            var go = new StreamingAssetManager();
            go.AllFinished = () =>
            {
                MogoFileSystem.Instance.Init();
                Action<bool> fileDecompress = (finish) =>
                {
                    Driver.Invoke(() =>
                    {
                        if (finish)
                            DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(3).content);//正在更新本地文件
                        else
                            DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(4).content);//恢复成“数据读取中”
                    });
                };
                Action<int, int, string> taskProgress = (total, index, filename) =>
                {
                    Driver.Invoke(() =>
                    {
                        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(2).content, index + 1, total, filename);
                    });
                };
                Action<int, long, long> progress = (ProgressPercentage, TotalBytesToReceive, BytesReceived) =>
                {
                    Driver.Invoke(() => { DefaultUI.Loading(ProgressPercentage); });
                };
                Action<Exception> error = (ex) =>
                {
                    ForwardLoadingMsgBox.Instance.ShowRetryMsgBox(DefaultUI.dataMap[54].content, (isOk) =>
                    {
                        if (isOk)
                            CheckVersion(callback);
                        else
                            Application.Quit();
                    });
                    LoggerHelper.Except(ex);
                };
                DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(1).content);
                VersionManager.Instance.CheckVersion(fileDecompress, taskProgress, progress, () => { Driver.Invoke(callback); }, error);
            };
            go.FirstExport();
        });
#else
        callback();
#endif
    }

    void Tick()
    {
        TimerHeap.Tick();
        FrameTimerHeap.Tick();
    }

    void OnLevelWasLoaded()
    {
        //在场景初始化时触发场景加载完成事件，以解决场景模型Draw call优化问题。
        if (Driver.Instance.LevelWasLoaded != null)
            Driver.Instance.LevelWasLoaded();
    }

    public static void Invoke(Action action)
    {
        TimerHeap.AddTimer(0, 0, action);
    }
}
#endif