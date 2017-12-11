#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoInitialize
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.19
// 模块描述：程序初始化控制器。
//----------------------------------------------------------------*/
#endregion

using Mogo.Game;
using Mogo.GameData;
using Mogo.RPC;
using Mogo.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MogoInitialize : MonoBehaviour
{
    private Vector3 soundOffset;
    //bool ok;
    void Start()
    {
        soundOffset = new Vector3(0, 5, 0);
        RegisterEvents();
        LoggerHelper.Debug("MogoInitialize");
        TryToInit();
        Action action = AIContainer.Init;
        action.BeginInvoke(null, null);
    }

    void RegisterEvents()
    {
        IAPUIController iap = IAPUIController.Singleton;

        EventDispatcher.AddEventListener(Events.DailyTaskEvent.OpenDailyTaskUI, DailyTaskSystemController.Singleton.OnOpenDailyTaskUI);
        EventDispatcher.AddEventListener<int>(Events.DailyTaskEvent.DailyTaskJumpToOtherUI, DailyTaskSystemController.Singleton.JumpTo);
    }

    private void TryToInit()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            Init();
        }
        else
        {
            System.Collections.Generic.Dictionary<int, DefaultLanguageData> languageData = DefaultUI.dataMap;
            ForwardLoadingMsgBox.Instance.ShowMsgBox(languageData[50].content, languageData[51].content, languageData[52].content, (isOk) =>
            {
                if (isOk)
                {
                    TryToInit();
                    ForwardLoadingMsgBox.Instance.Hide();
                }
                else
                {
                    if (PlatformSdkManager.Instance != null)
                        PlatformSdkManager.Instance.SetNetwork();
                }
            });
        }
    }

    void Init()
    {
        //启动驱动逻辑
        StartDrive();
        StartCoroutine(StartInit(
           DefaultUI.ShowLoading,
           DefaultUI.Loading));
    }

    void Update()
    {
        if (MogoWorld.thePlayer != null)
            if (MogoWorld.thePlayer.Transform != null)
                transform.position = MogoWorld.thePlayer.Transform.position + soundOffset;
    }

    IEnumerator StartInit(Action onStart, Action<int> progress)
    {
        onStart();
        StartCoroutine(NoticeManager.Instance.DownloadNotice());
        progress(10);
        yield return null;
        if (gameObject.GetComponent<LoadResources>() == null)
            gameObject.AddComponent(SystemSwitch.ReleaseMode ? typeof(LoadAssetBundlesMainAsset) : typeof(LoadResources));
        if (SystemSwitch.ReleaseMode)
        {
            ResourceManager.LoadMetaOfMeta(
                () =>
                {
                    List<string> shaders = new List<string>();
                    if (Directory.Exists(Path.Combine(SystemConfig.ResourceFolder, "Resources/Shader")))
                    {
                        shaders = FileAccessManager.GetFileNamesByDirectory("Resources/Shader");
                        shaders = (from s in shaders
                                   let r = System.IO.Path.GetFileName(s).ReplaceFirst(SystemConfig.ASSET_FILE_EXTENSION, String.Empty)
                                   where r.EndsWith(".shader", StringComparison.CurrentCultureIgnoreCase)
                                   select r).ToList();
                    }
                    else
                    {
                        shaders = ResourceIndexInfo.Instance.GetFileNamesByDirectory("Resources/Shader");
                        shaders = (from s in shaders
                                   let r = Mogo.Util.Utils.GetStreamPath(s)
                                   where r.EndsWith(".shader", StringComparison.CurrentCultureIgnoreCase)
                                   select r).ToList();
                    }

                    AssetCacheMgr.GetResources(shaders.ToArray(), null);

                    StartCoroutine(AfterInit(progress));
                },
                (index, total) =>
                {
                    if (total == 0)
                        return;
                    var proc = ((40 - 10) * index / total) + 10;
                    Driver.Invoke(() => { progress(proc); });
                }
                );
        }
        else
        {
            StartCoroutine(AfterInit(progress));
        }
    }

    IEnumerator AfterInit(Action<int> process)
    {
        process(40);
        yield return null;
        //加载游戏配置数据
        Action onGameDataReady = () =>
        {
            lock (MogoWorld.GameDataLocker)
            {
                MogoWorld.IsGameDataReady = true;
                if (MogoWorld.OnGameDataReady != null)
                {
                    Driver.Invoke(MogoWorld.OnGameDataReady);
                }
            }
        };
        GameDataControler.Init(null, onGameDataReady);
        process(45);
        yield return null;
        //初始化网络模块
        ServerProxy.Instance.Init();
        ServerProxy.Instance.BackToChooseServer = MogoWorld.BackToChooseCharacter;
        process(50);
        yield return null;
        //启动游戏逻辑
        MogoWorld.Init();
        SoundManager.Init();
        MogoWorld.Start();
        NPCManager.Init();
    }

    void StartDrive()
    {
        LoggerHelper.Debug("StartDrive");
        InvokeRepeating("Tick", 1, 0.1f);
    }

    void Tick()
    {
        //if (Input.GetKey(KeyCode.Return))
        //{
        //    MogoMessageBox.Confirm(LanguageData.GetContent((int)LangOffset.Account + (int)AccountCode.QUIT_GAME_CONFIRM), (flag) =>
        //    {
        //        if (flag)
        //        {
        //            Mogo.Util.LoggerHelper.Debug("Return!!!");
        //            Quit();
        //            Application.Quit();
        //        }
        //    });
        //}
        ServerProxy.Instance.Process();
        ServerProxy.Instance.Update();
    }

    void OnApplicationQuit()
    {
        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //    Quit();
        DownloadMgr.Instance.Dispose();
        if (SystemSwitch.ReleaseMode)
        {
            MogoFileSystem.Instance.Close();
        }
        Mogo.Util.LoggerHelper.Debug("Disconnected!!!");
        ServerProxy.Instance.Disconnect();
        ServerProxy.Instance.Release();
        Mogo.Util.LoggerHelper.Debug("Disconnected Finished");
        LoggerHelper.Release();
    }

    void Quit()
    {
        MogoWorld.Logout(0);
        ServerProxy.Instance.Process();
    }

    private float prePause = 0;
    void OnApplicationPause(bool pause)
    {
        //LoggerHelper.Error("pause  " + pause);
        if (pause)
        {
            prePause = Time.realtimeSinceStartup;
        }
        else
        {
            float cur = Time.realtimeSinceStartup;
            MogoWorld.StartTime = cur;
            float pay = cur - prePause;
            if (MogoWorld.thePlayer != null)
            {
                (MogoWorld.thePlayer.skillManager as PlayerSkillManager).Compensation(-prePay);
                prePay = 0;
                (MogoWorld.thePlayer.skillManager as PlayerSkillManager).Compensation(pay);
            }
        }
    }

    private float prePay = 0;
    private float preTick = 0;
    void OnApplicationFocus(bool focus)
    {
        //LoggerHelper.Error("focus   " + focus);
        if (focus)
        {
            if (MogoWorld.thePlayer != null)
            {
                (MogoWorld.thePlayer.skillManager as PlayerSkillManager).Compensation(prePay);
                TimerHeap.AddTimer(1000, 0, () => { prePay = 0; });
            }
        }
    }

}