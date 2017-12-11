#if !UNITY_IPHONE
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：Driver
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;

using Mogo.Util;
using System;
using System.Collections;
using System.Net;
using System.IO;

public class Driver : MonoBehaviour
{
    private static bool bLoodLib = false;
    public static String FileName
    {
        get
        {
            return "MogoRes";
        }
    }
    //C#的DES只支持64bits的Key
    //http://msdn.microsoft.com/en-us/library/system.security.cryptography.des.key(VS.80).aspx
    public static byte[] Number
    {
        get
        {
            return Utils.GetResNumber(); //"231,20,185,13,20,127,81,79";
        }
    }
    public static bool IsRunOnAndroid = false;
    public Action LevelWasLoaded;
    public static Driver Instance;

    void Awake()
    {
#if !UNITY_EDITOR
        LoggerHelper.CurrentLogLevels = LogLevel.INFO | LogLevel.ERROR | LogLevel.CRITICAL | LogLevel.EXCEPT;// | LogLevel.WARNING 
#endif
        LoggerHelper.Info("--------------------------------------Game Start!-----------------------------------------");
        SystemSwitch.InitSystemSwitch();
        Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
        DontDestroyOnLoad(transform.gameObject);
        Instance = this;
        gameObject.AddComponent<DriverLib>();
        Application.targetFrameRate = 30;
        DefaultUI.InitLanguageInfo();
        GameObject.Find("MogoForwardLoadingUIPanel").AddComponent<MogoForwardLoadingUIManager>();
        TryToInit();
        InvokeRepeating("Tick", 1, 0.02f);
        gameObject.AddComponent<AudioListener>();
    }

    void TryToInit()
    {
        InitLoaderLib();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            var checkTimeout = new CheckTimeout();
            DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(13).content);//正在检查网络
            checkTimeout.AsynIsNetworkTimeout((result) =>
            {
                Invoke(() =>
                {
                    if (result)
                        DoInit();
                    else
                    {
                        ForwardLoadingMsgBox.Instance.ShowRetryMsgBox(DefaultUI.dataMap[52].content, (isOk) =>
                        {
                            if (isOk)
                                TryToInit();
                            else
                                Application.Quit();
                        });
                    }
                });
            });

        }
        else
        {
            var languageData = DefaultUI.dataMap;
            ForwardLoadingMsgBox.Instance.ShowMsgBox(languageData[50].content, languageData[51].content, languageData[52].content, (OnOkButton) =>
            {
                if (OnOkButton)
                {
                    ForwardLoadingMsgBox.Instance.Hide();
                    TryToInit();
                }
                else
                {
#if UNITY_ANDROID&&!UNITY_EDITOR
                    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject m_mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                    m_mainActivity.Call("gotoNetworkSetting");
#endif
                }
            });
        }

    }

    void DoInit()
    {
        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(4).content);//数据读取中…
        var loadCfgSuccess = SystemConfig.Init();
        if (!loadCfgSuccess)
        {
            ForwardLoadingMsgBox.Instance.ShowRetryMsgBox(DefaultUI.dataMap[53].content, (isOk) =>
            {
                if (isOk)
                    DoInit();
                else
                    Application.Quit();
            });
            return;
        }
        if (SystemSwitch.ReleaseMode)
        {
            //第一次导出资源
            ResourceIndexInfo.Instance.Init(Application.streamingAssetsPath + "/ResourceIndexInfo.txt", () =>
            {
                //如果是完整包(判断ResourceIndexInfo.txt是否存在),再判断apk的资源版本是否比本地资源版本高，如果高删除MogoResource和version.xml
                if (ResourceIndexInfo.Instance.Exist())
                {
                    LoggerHelper.Debug("-------------------------Exist ResourceIndexInfo.txt--------------------------");
                    var localVer = Utils.LoadFile(SystemConfig.VersionPath);
                    var localVersion = VersionManager.Instance.GetVersionInXML(localVer);

                    var pkgVer = Resources.Load(SystemConfig.VERSION_URL_KEY) as TextAsset;
                    var pkgVersion = VersionManager.Instance.GetVersionInXML(pkgVer.text);
                    if (pkgVersion.ResouceVersionInfo.Compare(localVersion.ResouceVersionInfo) > 0)
                    {
                        //删除version.xml
                        if (File.Exists(SystemConfig.VersionPath))
                            File.Delete(SystemConfig.VersionPath);
                        //删除MogoResource文件夹
                        var mogoResroucesPath = SystemConfig.ResourceFolder.Substring(0, SystemConfig.ResourceFolder.Length - 1);
                        if (Directory.Exists(mogoResroucesPath))
                            Directory.Delete(mogoResroucesPath, true);
                        //删除后再导出version
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
                    LoggerHelper.Debug("firstExport finish,start checkversion");
                    if (SystemSwitch.UseFileSystem)
                    {
                        try
                        {
                            MogoFileSystem.Instance.Init();
                        }
                        catch (Exception ex)
                        {
                            LoggerHelper.Except(ex);
                        }
                    }
                    VersionManager.Instance.Init();
                    VersionManager.Instance.LoadLocalVersion();
                    CheckVersion(CheckVersionFinish);
                };
                go.FirstExport();
            });
        }
        else
        {
            SystemConfig.LoadServerList();
            VersionManager.Instance.Init();
            VersionManager.Instance.LoadLocalVersion();
            gameObject.AddComponent("MogoInitialize");
            IsRunOnAndroid = false;
            gameObject.AddComponent("PlatformSdkManager");
        }
    }

    public void CheckVersionFinish()
    {
        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(4).content);//数据读取中
        SystemConfig.LoadServerList();
#if UNITY_ANDROID
        LoadLib();
#endif
#if UNITY_EDITOR
        gameObject.AddComponent("MogoInitialize");
        gameObject.AddComponent("TestUI");
#else
        if (SystemSwitch.ReleaseMode)
        {
            gameObject.AddComponent<StreamingAssetCacheManager>();
            StreamingAssetCacheManager.Instance.Export();
        }
#endif
    }

    void CheckVersion(Action finished)
    {
        if (SystemSwitch.ReleaseMode)
        {
            //添加一个处理解压文件时的界面提示回调,解压前开始提示，解压后恢复
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
            //第一个参数是总任务数，第二参数是已经完成的任务
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
                        CheckVersion(finished);
                    else
                        Application.Quit();
                });
                LoggerHelper.Except(ex);
            };

            DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(1).content);
            //版本检查
            if (SystemSwitch.ReleaseMode)
                VersionManager.Instance.CheckVersion(fileDecompress, taskProgress, progress, () => { Driver.Invoke(finished); }, error);
            else
                finished();
        }
        else
        {
            finished();
        }
    }

    void Tick()
    {
        TimerHeap.Tick();
        FrameTimerHeap.Tick();
    }

    void InitLoaderLib()
    {
        PluginCallback.Instance.ShowGlobleLoadingUI = MogoForwardLoadingUIManager.Instance.ShowGlobleLoadingUI;
        PluginCallback.Instance.SetLoadingStatus = MogoForwardLoadingUIManager.Instance.SetLoadingStatus;
        PluginCallback.Instance.SetLoadingStatusTip = MogoForwardLoadingUIManager.Instance.SetLoadingStatusTip;
        PluginCallback.Instance.ShowRetryMsgBox = GameObject.Find("MogoDefaultUI/DefaultUICamera2/Anchor").GetComponent<ForwardLoadingMsgBox>().ShowRetryMsgBox;
        PluginCallback.Instance.ShowMsgBox = GameObject.Find("MogoDefaultUI/DefaultUICamera2/Anchor").GetComponent<ForwardLoadingMsgBox>().ShowMsgBox;
        PluginCallback.Instance.Hide = GameObject.Find("MogoDefaultUI/DefaultUICamera2/Anchor").GetComponent<ForwardLoadingMsgBox>().Hide;
        PluginCallback.Instance.IsHide = GameObject.Find("MogoDefaultUI/DefaultUICamera2/Anchor").GetComponent<ForwardLoadingMsgBox>().IsHide;
    }
    void LoadSolutionLib()
    {
        var index = 0;
        var resName = FileName + index;
        while (FileAccessManager.IsFileExist(resName))
        {
            LoggerHelper.Debug("load: " + resName);
            var enData = FileAccessManager.LoadBytes(resName);
            if (enData != null)
            {
                var deData = DESCrypto.Decrypt(enData, Number);
                deData = Utils.UnpackMemory(deData);
                System.Reflection.Assembly.Load(deData);
            }
            index++;
            resName = FileName + index;
        }
    }

    void LoadLib()
    {
        if (!bLoodLib)
            bLoodLib = true;
        else
            return;
        LoadSolutionLib();
        var enData = FileAccessManager.LoadBytes(FileName);
        if (enData != null)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var deData = DESCrypto.Decrypt(enData, Number);
            sw.Stop();
            deData = Utils.UnpackMemory(deData);
            LoggerHelper.Info("Decrypt time: " + sw.ElapsedMilliseconds);
            var ass = System.Reflection.Assembly.Load(deData); //动态库的名称
            //gameObject.AddComponent(ass.GetType("TestUI"));
            if (ass.GetType("MogoInitialize") != null)
                gameObject.AddComponent(ass.GetType("MogoInitialize"));
            else
                LoggerHelper.Error("---------------Reflection MogoInitialize Failed------------------------");
            //gameObject.AddComponent(ass.GetType("TestUI"));

            if (Application.platform == RuntimePlatform.Android && SystemSwitch.UsePlatformSDK)
            {
                IsRunOnAndroid = true;
                if (ass.GetType("AndroidSdkManager") != null)
                    gameObject.AddComponent(ass.GetType("AndroidSdkManager"));
                else
                    LoggerHelper.Error("---------------Reflection AndroidSdkManager Failed------------------------");
                //IsRunOnAndroid = false;
            }
            else
            {
                gameObject.AddComponent(ass.GetType("PlatformSdkManager"));
                IsRunOnAndroid = false;
            }
        }
        else
        {
            LoggerHelper.Error("Missing MogoLib.");
        }
    }

#if false
    void InitResourceFile(Action<int> loading, Action finished)
    {
        var fileName = "/MogoResources.zip";
        var pkgPath = Application.streamingAssetsPath + fileName;
        var output = SystemConfig.ResourceFolder + fileName;
        if (File.Exists(output))//已经拷过出来就不拷了
        {
            var file = new FileInfo(output);
            if (file.Length == 0)
            {
                if (finished != null)
                    finished();
                return;
            }
        }
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            pkgPath = "File://" + pkgPath;
        }

        StartCoroutine(LoadWWW(pkgPath, (www) =>
        {
            StartCoroutine(Loading(www, (p) =>
            {
                if (loading != null)
                    loading((int)(p * 100));
            }));

        }, (pkg) =>
        {
            if (pkg != null && pkg.Length != 0)
            {
                var path = SystemConfig.ResourceFolder;
                Action action = () =>
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    XMLParser.SaveBytes(output, pkg);
                    Utils.DecompressToDirectory(path, output);
                    if (File.Exists(output))
                        File.Delete(output);
                    XMLParser.SaveText(output, "");
                    LoggerHelper.Debug("Save MogoResources, size: " + pkg.Length);
                    Invoke(finished);
                };
                action.BeginInvoke(null, null);
                var ver = Resources.Load(SystemConfig.VERSION_URL_KEY) as TextAsset;
                if (ver != null)
                    XMLParser.SaveText(SystemConfig.VersionPath, ver.text);
            }
            else
            {
                LoggerHelper.Warning("Save MogoResources error.");
                if (finished != null)
                    finished();
            }
        }));
    }

    void InitMogoFile(Action<int> loading, Action finished)
    {
        var fileName = "pkg";
        var target = SystemConfig.ResourceFolder + fileName;
        if (File.Exists(SystemConfig.VersionPath) && File.Exists(target))//pkg文件存在以及版本文件存在才不拷出来，如果只是pkg文件存在，有可能pkg文件不完整
        {
            var localVer = Utils.LoadFile(SystemConfig.VersionPath);
            var localVersion = VersionManager.Instance.GetVersionInXML(localVer);

            var pkgVer = Resources.Load(SystemConfig.VERSION_URL_KEY) as TextAsset;
            var pkgVersion = VersionManager.Instance.GetVersionInXML(pkgVer.text);

            if (pkgVersion.ResouceVersionInfo.Compare(localVersion.ResouceVersionInfo) > 0)
            {
                File.Delete(target);
                File.Delete(SystemConfig.VersionPath);
            }
            else
            {
                if (finished != null)
                    finished();
                return;
            }
        }
        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(0).content);//游戏初次运行初始化中，时间较长，请耐心等待…
        Directory.CreateDirectory(SystemConfig.ResourceFolder);
        //var pkg = Resources.Load(fileName) as TextAsset;
        //if (pkg != null)
        //    XMLParser.SaveBytes(target, pkg.bytes);

        var pkgPath = Application.streamingAssetsPath + "/" + fileName;
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            pkgPath = "File://" + pkgPath;
        }

        StartCoroutine(LoadWWW(pkgPath, (www) =>
        {
            StartCoroutine(Loading(www, (p) =>
            {
                if (loading != null)
                    loading((int)(p * 30));//硬编码获取文件占进度前30%
            }));

        }, (pkg) =>
        {
            //if (loading != null)
            //    loading(1);
            if (pkg != null && pkg.Length != 0)
            {
                Action action = () =>
                {
                    //保存文件
                    MemoryStream stream = new MemoryStream(pkg);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] data = new byte[2048];
                    var pkgLength = pkg.Length;
                    using (FileStream streamWriter = File.Create(target))
                    {
                        int bytesRead;
                        long bytesTotalRead = 0;
                        while ((bytesRead = stream.Read(data, 0, data.Length)) > 0)
                        {
                            streamWriter.Write(data, 0, bytesRead);
                            bytesTotalRead += bytesRead;
                            //Debug.Log("bytesTotalRead: " + bytesTotalRead + " bytesRead: " + bytesRead);
                            if (loading != null)
                                Invoke(() => loading(30 + (int)((bytesTotalRead * 70) / pkgLength)));//硬编码存储文件占进度后70%
                        }
                    }
                    //XMLParser.SaveBytes(target, pkg);
                    Invoke(() =>
                    {
                        LoggerHelper.Debug("Save pkg, size: " + pkg.Length);
                        if (!File.Exists(SystemConfig.VersionPath))
                        {
                            var ver = Resources.Load(SystemConfig.VERSION_URL_KEY) as TextAsset;
                            if (ver != null)
                                XMLParser.SaveText(SystemConfig.VersionPath, ver.text);
                        }
                        pkg = null;
                        GC.Collect();
                        if (finished != null)
                            finished();
                    });
                };
                action.BeginInvoke(null, null);
            }
            else
            {
                LoggerHelper.Warning("Save pkg error.");
                if (finished != null)
                    finished();
            }
        }));
    }
#endif

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