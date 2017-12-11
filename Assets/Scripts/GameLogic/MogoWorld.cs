/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoWorld
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-2-7
// 模块描述：游戏全局数据管理器
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mogo.Util;
using Mogo.RPC;
using Mogo.Game;
using Mogo.GameData;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using Mogo.FSM;

public class MogoWorld
{
    #region Properties

    static private int monsterCount = 0;
    static private int playerCount = 0;
    static private int dummyCount = 0;
    static private int mercenaryCount = 0;
    static private int enemyCount = 0;
    static private uint chgDummyRateTimer = 0;

    private static bool sceneChanged = false;
    private static int orgId = 0;
    private static bool islogining;
    public static float StartTime = 0; //游戏从暂停恢复的时间点

    // 异步机关数量，确定进场景之前机关准备妥当
    static private int asyncGearNum = 0;

    /// <summary>
    /// 根据entity id索引的实体
    /// </summary>
    static private Dictionary<uint, EntityParent> entities = new Dictionary<uint, EntityParent>();
    /// <summary>
    /// 根据模型实例id索引的实体
    /// </summary>
    static private Dictionary<int, EntityParent> gameObjects = new Dictionary<int, EntityParent>();
    static private Dictionary<uint, Vector3> dropPoints = new Dictionary<uint, Vector3>();

    static public LoginInfo loginInfo;

    static public string passport;
    static public string password;
    static public EntityAccount theAccount;
    static public EntityMyself thePlayer;
    static public EntityParent m_currentEntity;
    static public uint theLittleGuyID;
    static public ScenesManager scenesManager;
    static public GlobalData globalSetting;

    static public int arenaState = 0;
    static public bool isLoadingScene;
    static public bool inCity = true;
    static public bool showClientGM = false;
    static public bool showSkillFx = true;
    static public bool showHitAction = true;
    static public bool showHitEM = true;
    static public bool showHitShader = true;
    static public bool showFloatBlood = true;
    static public bool unhurtMe = false;
    static public bool unhurtDummy = false;
    static public bool pauseAI = false;
    static public bool showFloatText = true;
    static public bool isReConnect = false; //是否断线重连
    static public bool rc = false; //断线重连
    static public bool beKick = false; //是否被踢下线
    static public string reConnectKey = ""; //重连key
    static public string baseLoginKey = ""; //登陆key
    static public string baseIP = "";
    static public int basePort = 0;
    static public string gmcontent = "";
    static public bool mainCameraCompleted = false;
    static public bool canAutoFight = false;
    static public bool CGing = false;
    static public bool hasMonster = false;
    static public bool touchLastPathPoint = false;
    static public bool connoning = false;
    static public bool showDebug = false;// 调试窗口-FPS

    static public readonly object GameDataLocker = new object();
    static public bool IsGameDataReady;
    static public Action OnGameDataReady;
    static public bool isFirstTimeInCity = true;
    static public bool BeginLogin { get; private set; }

    //用于断线重连时前端资源未准备好缓存
    static public CellAttachedInfo me = null;
    static public List<CellAttachedInfo> others = new List<CellAttachedInfo>();

    static public Dictionary<uint, EntityParent> Entities
    {
        get { return entities; }
    }

    static public Dictionary<int, EntityParent> GameObjects
    {
        get { return gameObjects; }
    }

    static public Dictionary<uint, Vector3> DropPoints
    {
        get { return dropPoints; }
    }

    static public int MonsterCount
    {
        get { return monsterCount; }
    }

    static public int DummyCount
    {
        get { return dummyCount; }
    }

    static public int MercenaryCount
    {
        get { return mercenaryCount; }
    }

    static public int PlayerCount
    {
        get { return playerCount; }
    }

    static public int EnemyCount
    {
        get { return enemyCount; }
    }

    static public bool IsClientMission
    {
        get { return ServerProxy.SomeToLocal; }
    }

    static public bool isClientPositionSync;

    static public bool IsClientPositionSync
    {
        get { return isClientPositionSync; }
        set
        {
            // Debug.LogError("value: " + value);
            isClientPositionSync = value;
        }
    }

    #endregion

    // 游戏全局数据管理
    static MogoWorld()
    {
        AddListeners();
        new GMManager();
    }

    // 处理entity 进入场景事件
    static public void OnEnterWorld(EntityParent entity)
    {
        int oldEnemyCount = enemyCount;
        if (entities.ContainsKey(entity.ID))
        {
            LoggerHelper.Error("Space has the same id:" + entity.ID);
            return;
        }
        if (entity is EntityPlayer)
        {
            playerCount++;
        }
        else if (entity is EntityMonster)
        {
            monsterCount++;
            enemyCount++;
        }
        else if (entity is EntityDummy)
        {
            if (entity.MonsterData.monsterType <= 4)
            {
                dummyCount++;
                enemyCount++;
            }
        }
        else if (entity is EntityMercenary)
        {
            mercenaryCount++;
            if (entity.m_factionFlag == 0)
            {
                enemyCount++;//非雇佣兵
            }
        }
        entities.Add(entity.ID, entity);

        if (oldEnemyCount != enemyCount)
        {
            //通知显示指引
            MogoWorld.hasMonster = true;
            EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
        }
    }

    // 处理 entity 离开场景事件
    static private void OnLeaveWorld(uint eid)
    {
        if (!entities.ContainsKey(eid))
        {
            return;
        }

        EntityParent entity = entities[eid];

        if (entity is EntityPlayer)
        {
            playerCount--;
        }
        else if (entity is EntityMonster)
        {
            monsterCount--;
            enemyCount--;
        }
        else if (entity is EntityDummy)
        {
            if (entity.MonsterData.monsterType <= 4)
            {
                dummyCount--;
                enemyCount--;
            }
        }
        else if (entity is EntityMercenary)
        {
            mercenaryCount--;
            if (entity.m_factionFlag == 0)
            {
                enemyCount--;//非雇佣兵
            }
        }

        entities.Remove(eid);
        if (enemyCount <= 0)
        {
            //通知指引消失
            MogoWorld.hasMonster = false;
            EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
        }
    }

    #region 框架协议处理函数

    // 帐号登录回调函数
    static public void OnLoginResp(LoginResult result)
    {
        if (result != LoginResult.SUCCESS)
        {
            MogoMessageBox.RespError(LangOffset.Account, (int)result);
            //scenesManager.LoadCharacterScene(() =>
            //{
            //    NewLoginUILogicManager.Instance.ShowChooseServerUIFormLogin();
            //}, false);
            //MogoUIManager.Instance.ShowNewLoginUI(() =>
            //{
            //    NewLoginUILogicManager.Instance.ShowChooseServerUIFormLogin();
            //});
        }
        else
        {

        }
    }

    // 协议处理函数： 登录 baseapp
    static private void OnBaseLogin(string ip, int port, string token)
    {
        ServerProxy.Instance.Disconnect();
        LoggerHelper.Debug("base ip: " + ip + " port: " + port);
        ServerProxy.Instance.Connect(ip, port);
        ServerProxy.Instance.BaseLogin(token);
        baseIP = ip;
        basePort = port;
    }


    static private void OnEntityCellAttached(CellAttachedInfo info)
    {
        LoggerHelper.Debug("OnEntityCellAttached " + info.position);
        EntityParent entity = null;
        var eid = info.id;       // entity uniq id

        if (eid == thePlayer.ID)
        {
            if (isReConnect && !mainCameraCompleted)
            {//断线重连后缓存cellAttachedInfo
                me = info;
                return;
            }
            entity = thePlayer;
            thePlayer.SetEntityCellInfo(info);
            //SwitchScene(thePlayer.sceneId);
        }
        else if (eid == theAccount.ID)
        {
            entity = theAccount;
            theAccount.SetEntityCellInfo(info);
        }
        //LoadHomeScene();
    }

    // 协议处理函数
    static private void OnEntityAttached(BaseAttachedInfo baseInfo)
    {
        if (baseInfo.entity == null)
        {
            LoggerHelper.Error("Entity Attach Error.");
            return;
        }
        switch (baseInfo.entity.Name)
        {
            case "Account":
                {
                    m_currentEntity = theAccount;
                    LoggerHelper.Debug("account attach");
                    Action action = () =>
                    {
                        Mogo.GameLogic.LocalServer.LocalServerResManager.Initialize();
                        MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "");
                        theAccount.SetEntityInfo(baseInfo);
                        theAccount.OnEnterWorld();
                        theAccount.CheckVersionReq();
                        theAccount.entity = baseInfo.entity;
                        isReConnect = false;
                        LoadCharacterScene();

                        if (PlatformSdkManager.Instance)
                            PlatformSdkManager.Instance.SetupNotificationData();
                    };
                    lock (MogoWorld.GameDataLocker)
                    {
                        if (MogoWorld.IsGameDataReady)
                        {
                            Driver.Invoke(action);
                        }
                        else
                        {
                            MogoWorld.OnGameDataReady = action;
                        }
                    }
                    break;
                }
            case "Avatar":
                {
                    LoggerHelper.Debug("self attach");
                    bool ab = false; //用于切换cell连接时容错
                    if (MogoWorld.thePlayer == null)
                    {
                        ab = true;
                        thePlayer = new EntityMyself();
                        thePlayer.deviator = RandomHelper.GetRandomInt(1, 3000);
                        thePlayer.CalcuDmgBase();
                    }
                    m_currentEntity = thePlayer;
                    thePlayer.SetEntityInfo(baseInfo);
                    if (ab)
                    {
                        thePlayer.OnEnterWorld();
                    }
                    //thePlayer.UpdateSkillToManager();
                    thePlayer.entity = baseInfo.entity;
                    // EventDispatcher.TriggerEvent(Events.OperationEvent.GetAllActivity);
                    break;
                }
            default:
                break;
        }
    }

    // 协议处理函数， 新的 entity 进入视野
    static private void AOINewEntity(CellAttachedInfo info)
    {
        // todo:  先硬编码， 以后改成 从 def 中 自动化
        EntityParent entity;
        LoggerHelper.Debug(info.entity.Name);
        if (isReConnect && !mainCameraCompleted)
        {//断线重连场景没加载完,缓存起来
            others.Add(info);
            return;
        }
        if (Entities.ContainsKey(info.id) || (thePlayer != null && thePlayer.ID == info.id))
        {
            LoggerHelper.Debug("has same id entity in world");
            return;
        }
        if ((info.entity.Name == "Monster" ||
            info.entity.Name == "Mercenary" ||
            info.entity.Name == "Drop") && inCity)
        {
            //主城不出现Monster Mercenary，用于切场景的瞬间收到创建协议
            return;
        }
        switch (info.entity.Name)
        {
            case "Avatar"://对应Avatar.def
                if (SystemConfig.Instance.PlayerCountInScreen <= playerCount
                    || (MogoWorld.thePlayer != null && MogoWorld.thePlayer.IsNewPlayer))
                {
                    LoggerHelper.Debug("EntityPlayer over count: " + playerCount);
                    return;
                }
                if (MogoWorld.IsClientMission)
                {
                    return;
                }
                entity = new EntityPlayer();
                entity.vocation = Vocation.Warrior;
                break;
            case "NPC"://对应NPC.def
                // entity = new EntityNPC();
                entity = new EntityParent();
                break;
            case "Monster": //对应Monster.def
                entity = new EntityMonster();
                break;
            case "Mercenary": //对应Monster.def
                entity = new EntityMercenary();
                break;
            case "Drop": //对应Drop.def
                entity = new EntityDrop();
                Vector3 newPosi = MogoWorld.FindEmptyDropPoint(info.position);
                (entity as EntityDrop).SetTweenTarget(newPosi);
                //LoggerHelper.Error("fuck end " + info.id + " n " + newPosi.x + " " + newPosi.y + " " + newPosi.z);
                break;
            case "TeleportPointSrc": //对应TeleportPointSrc.def
                entity = new EntityTeleportSrc();
                break;
            case "Dummy": //CMonster.def
                entity = new EntityDummy();
                break;
            default:
                entity = new EntityParent();
                break;
        }
        entity.ID = info.id;
        entity.entity = info.entity;
        entity.SetEntityCellInfo(info);
        entity.OnEnterWorld();
        if (!isLoadingScene)
            entity.CreateModel();
        //LoggerHelper.Error("AOINewEntity: etype: " + info.entity.Name + " eid: " + entity.ID + " pos: " + entity.position);
        OnEnterWorld(entity);

        EventDispatcher.TriggerEvent(Events.CampaignEvent.SetPlayerMessage, entity);
    }

    static public void CreateDummy(uint eid, int x, int y, int cfgId, int difficulty, int spawnPointCfgId)
    {
        //LoggerHelper.Error("CreateDummy difficulty:" + difficulty);
        if (!MonsterData.dataMap.ContainsKey(cfgId))
        {
            return;
        }
        MonsterData m = MonsterData.GetData(cfgId, MogoWorld.thePlayer.ApplyMissionID == Mogo.Game.RandomFB.RAIDID ? MogoWorld.thePlayer.level : difficulty);
        EntityDummy entity = new EntityDummy();
        CellAttachedInfo info = new CellAttachedInfo();
        info.x = (short)x;
        info.y = (short)y;
        info.id = eid;
        entity.spawnPointCfgId = spawnPointCfgId;
        entity.MonsterData = m;
        entity.ID = eid;
        entity.clientTrapId = m.clientTrapId;
        entity.SetEntityCellInfo(info);
        if (m.clientBoss == 1)
        {//为体验关特制
            entity.stateFlag = 1 << 13;
        }
        entity.SetCfg(m);
        //entity.curHp = (uint)currHp;因为现在不用重新登录刷怪，所以直接前端设一次就够了,现在curHp换成difficulty了，如果大于0则要根据difficulty匹配怪物的能力in MonsterValue
        entity.OnEnterWorld();

        entity.enterX = (short)x;
        entity.enterZ = (short)y;
        OnEnterWorld(entity);

        if (isLoadingScene)//在副本下线重新登录后isLoadingScene没有置回去false 导致dummy刷不出
        {
            //entity.ReadyCreateModel();
        }
        else
        {
            TimerHeap.AddTimer(1000, 0, DelayCreateDummy, entity);
        }
    }

    static private void DelayCreateDummy(EntityDummy e)
    {
        if (!mainCameraCompleted)
        {
            TimerHeap.AddTimer(1000, 0, DelayCreateDummy, e);
            return;
        }
        e.ReadyCreateModel();
    }

    static public void CreateDrop(uint eid, int x, int y, int gold, int itemId, int belongAvatar)
    {
        if (belongAvatar == thePlayer.ID)
        {
            EntityDrop entity = new EntityDrop();
            CellAttachedInfo info = new CellAttachedInfo();
            info.x = (short)x;
            info.y = (short)y;
            info.id = eid;

            Vector3 newPosi = MogoWorld.FindEmptyDropPoint(info.position);
            entity.SetTweenTarget(newPosi);
            entity.ID = info.id;
            entity.entity = info.entity;
            entity.SetEntityCellInfo(info);
            entity.gold = gold;
            entity.itemId = itemId;
            entity.belongAvatar = belongAvatar;
            entity.OnEnterWorld();

            if (!isLoadingScene)
                entity.CreateModel();

            OnEnterWorld(entity);
        }
    }

    // 协议处理函数，  entity 离开视野
    static private void AOIDelEntity(uint eid)
    {
        if (!entities.ContainsKey(eid))
        {
            return;
        }

        EntityParent entity = entities[eid];
        if (entity == null)
            return;
        entity.OnLeaveWorld();
        OnLeaveWorld(eid);

        EventDispatcher.TriggerEvent(Events.CampaignEvent.RemovePlayerMessage, entity);
    }

    #endregion

    #region 网络处理函数

    static public void ConnectServer(string ip, int port)
    {
        bool rst = ServerProxy.Instance.Connect(ip, port);
        LoggerHelper.Debug("connnect rst: " + rst);
    }

    static public void DisConnectServer()
    {
        ServerProxy.Instance.Disconnect();
    }

    // 连接服务器回调函数
    static public void OnConnected(int nErrorID, string message)
    {
        LoggerHelper.Debug("onConnected server. " + message);
        //if (nErrorID == 0)
        //{
        //    theAccount = new EntityAccount();
        //    Driver.Instance.enabled = true;
        //}
    }

    // 处理服务器断开
    static public void OnDisconnectionFromServer()
    {
    }

    // Check MD5
    public static void CheckDefMD5()
    {
        ServerProxy.Instance.CheckDefMD5(VersionManager.Instance.defContentBytes);
    }

    public static void OnCheckDefMD5(DefCheckResult ret)
    {
        switch (ret)
        {
            case DefCheckResult.ENUM_LOGIN_CHECK_NO_SERVICE:
                MogoGlobleUIManager.Instance.Info("No Service!");
                break;

            case DefCheckResult.ENUM_LOGIN_CHECK_ENTITY_DEF_NOMATCH:
                MogoGlobleUIManager.Instance.Info("Entity Def Not Match!");
                break;
        }
    }

    #endregion

    #region 公共方法

    // 系统初始化
    static public void Init()
    {
        LoggerHelper.Debug("init here");
        theAccount = new EntityAccount();
        scenesManager = new ScenesManager();
    }

    // 系统启动，加载场景
    static public void Start()
    {
        if (GlobalData.dataMap == null || !GlobalData.dataMap.ContainsKey(0))
        {
            LoggerHelper.Error("Missing GlobalData!");
            return;
        }
        globalSetting = Mogo.GameData.GlobalData.dataMap[0];

        LoadLoginScene();
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    static public void ClearEntities()
    {
        List<EntityParent> es = new List<EntityParent>();
        foreach (var item in entities)
        {
            es.Add(item.Value);
        }
        foreach (var i in es)
        {
            if (i.ID == thePlayer.ID)
            {
                continue;
            }
            i.OnLeaveWorld();
        }
        entities.Clear();

        playerCount = 0;
        monsterCount = 0;
        dummyCount = 0;
        mercenaryCount = 0;
        enemyCount = 0;
    }

    // 根据 dbid 获取 entity 对象
    static public EntityParent GetEntity(uint dbid)
    {
        if (dbid == thePlayer.ID)
        {
            return thePlayer;
        }
        else if (entities.ContainsKey(dbid))
        {
            return entities[dbid];
        }
        return null;
    }

    static public void Login()
    {
        BeginLogin = true;

        LoggerHelper.Debug("Login");
        var serverInfo = SystemConfig.GetSelectedServerInfo();
        if (serverInfo == null)
        {
            LoggerHelper.Error("Server Info index error: " + SystemConfig.Instance.SelectedServer);
            MogoMessageBox.Info("Server Info Error.");
            return;
        }
        if (!serverInfo.CanLogin())
        {
            MogoMessageBox.Info(serverInfo.GetInfo());
            return;
        }

        Action AfterGetInfo = () =>
        {
            if (islogining) return;
            if (serverInfo.ip == "127.0.0.1")
            {
                ServerProxy.SwitchToLocal();
                theAccount.avatarList = new Dictionary<int, AvatarInfo>();
                theAccount.avatarList.Add(1, new AvatarInfo() { DBID = 1, Name = LanguageData.dataMap[1].content, Vocation = 1 });
                theAccount.avatarList.Add(2, new AvatarInfo() { DBID = 2, Name = LanguageData.dataMap[2].content, Vocation = 2 });
                theAccount.avatarList.Add(3, new AvatarInfo() { DBID = 3, Name = LanguageData.dataMap[3].content, Vocation = 3 });
                theAccount.avatarList.Add(4, new AvatarInfo() { DBID = 4, Name = LanguageData.dataMap[4].content, Vocation = 4 });

                ServerProxy.Instance.Login(LoginInfo.GetPCStrList());
                return;
            }
            else
            {
                ServerProxy.SwitchToRemote();
            }
            if (ServerProxy.Instance.Connected)
            {
                DisConnectServer();
            }


            bool rst = false;
            if (MogoWorld.rc)
            {
                rst = ServerProxy.Instance.Connect(baseIP, basePort);
                LoginReq(rst);
            }
            else if (SystemSwitch.UsePlatformSDK && (RuntimePlatform.Android == Application.platform || RuntimePlatform.IPhonePlayer == Application.platform))
            {
                islogining = true;
                MogoGlobleUIManager.Instance.ShowGlobleStaticText(true, "Login...");
                //得到baseapp信息
                string url = string.Format(PlatformSdkManager.LOGIN_URL, serverInfo.ip, loginInfo.platName, loginInfo.uid, loginInfo.timestamp, loginInfo.strPlatAccount, loginInfo.strSign, loginInfo.token, SystemConfig.GetSelectedServerInfo().port);
                Action action = () =>
                {
                    Mogo.Util.Utils.GetHttp(url,
                        (resp) =>
                        {
                            Driver.Invoke(() =>
                            {
                                string[] baseappInfo = resp.Split(',');
                                int errorId = int.Parse(baseappInfo[0]);
                                if (errorId == 0)
                                {
                                    baseIP = baseappInfo[1];
                                    basePort = int.Parse(baseappInfo[2]);
                                    baseLoginKey = baseappInfo[3];
                                    rst = ServerProxy.Instance.Connect(baseIP, basePort);

                                    if (baseappInfo.Length > 4)
                                    {
                                        String uid = baseappInfo[4];
                                        if (RuntimePlatform.Android == Application.platform)
                                        {
                                            PlatformSdkManager.Instance.Uid = uid;
                                        }
                                    }

                                    if (baseappInfo.Length > 5)
                                    {
                                        String username = baseappInfo[5];
                                        if (RuntimePlatform.Android == Application.platform)
                                        {
                                            LoginUILogicManager.Instance.UserName = username;
                                            SystemConfig.Instance.Passport = username;
                                            SystemConfig.SaveConfig();
                                        }
                                    }

                                    if (PlatformSdkManager.Instance != null)
                                    {
                                        PlatformSdkManager.Instance.SendLoginLog();
                                    }
                                    LoginReq(rst);
                                }
                                else
                                {
                                    MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "");
                                    MogoWorld.OnLoginResp((Mogo.RPC.LoginResult)(errorId));
                                }

                                islogining = false;

                            });
                        },
                        (errodCode) =>
                        {
                            Driver.Invoke(() =>
                            {
                                MogoMsgBox.Instance.ShowMsgBox("network error：" + errodCode);
                                islogining = false;
                                MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "");
                            });

                        }
                        );



                };
                action.BeginInvoke(null, null);
            }
            else
            {
                MogoGlobleUIManager.Instance.ShowGlobleStaticText(true, "Login...");
                Action act = () =>
                {
                    rst = ServerProxy.Instance.Connect(serverInfo.ip, serverInfo.port);

                    Driver.Invoke(() =>
                    {
                        MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "");
                        LoginReq(rst);
                    });
                };
                act.BeginInvoke(null, null);
            }
        };
        if (SystemSwitch.UsePlatformSDK && (RuntimePlatform.IPhonePlayer == Application.platform || RuntimePlatform.Android == Application.platform)
        && !PlatformSdkManager.Instance.IsLoginDone && !rc)
        {
            LoggerHelper.Info("PlatformSdkManager.Instance.Login();");
#if UNITY_IPHONE
            PlatformSdkManager.Instance.LoginCallBack= AfterGetInfo; 
#endif
            PlatformSdkManager.Instance.Login();
            return;
        }

        if (SystemSwitch.UsePlatformSDK)
        {
            PlatformSdkManager.Instance.Log("AfterGetInfo");
            PlatformSdkManager.Instance.SetupInfo();
        }
        AfterGetInfo();
    }

    static public void ShowChooseServerUI(Action loaded = null)
    {
        scenesManager.LoadCharacterScene(() =>
        {
            if (loaded != null)
                loaded();
            NewLoginUILogicManager.Instance.ShowChooseServerUIFormCreateChr();
        }, null, false);
    }

    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="flag">0：返回登录界面。1：返回角色选择界面。</param>
    static public void Logout(byte flag)
    {
        if (thePlayer != null)
            thePlayer.RpcCall("Logout", flag);
    }

    static public void BackToChooseCharacter()
    {
        ClearEntities();
        if (thePlayer != null)
        {
            thePlayer.OnLeaveWorld();
            thePlayer = null;
        }
        Pluto.CurrentEntity = theAccount.entity;
        TimerHeap.AddTimer(100, 0, () =>
        {
            scenesManager.UnloadMainUI();
            scenesManager.LoadCharacterScene(theAccount.UpdateCharacterList, null, false, true);
        });
    }

    static public void SwitchScene(int orgSceneId, int newSceneId)
    {
        thePlayer.AutoFight = AutoFightState.IDLE;
        thePlayer.ProgressPointIndex = -1;
        canAutoFight = false;
        thePlayer.ClearSkill();
        thePlayer.CleanCharging();
        if (!(MapData.dataMap.ContainsKey(newSceneId) && !scenesManager.CheckSameScene(newSceneId)))
        {
            return;
        }
        if (isLoadingScene)
        {//正在加载，返回，等加载完再比较处理
            orgId = orgSceneId;
            sceneChanged = true;
            return;
        }
        MissionPathPointTriggerGear.ClearMissionPathPointTriggerGear();
        ClearEntities();
        isLoadingScene = true;
        MogoWorld.inCity = MapData.dataMap.Get(newSceneId).type == MapType.Normal;
        EventDispatcher.TriggerEvent(Events.InstanceEvent.BeforeInstanceLoaded, orgSceneId, !MogoWorld.inCity);
        if (MapData.dataMap[newSceneId].type == MapType.Normal)
        {
            LoadHomeScene(orgSceneId);
            return;
        }
        GameProcManager.ChangeScene(orgSceneId, newSceneId, true);

        scenesManager.LoadInstance(newSceneId, (isLoadScene) =>
        {
            TimerHeap.AddTimer(0, 0, () =>
            {
                GameProcManager.ChangeScene(orgSceneId, newSceneId, false);

                Container.ClearAllContainers();
                LoadGears(newSceneId, () =>
                {
                    if (orgSceneId != 0)
                        EventDispatcher.TriggerEvent(Events.InstanceEvent.InstanceUnLoaded, orgSceneId, true);
                    EventDispatcher.TriggerEvent(Events.InstanceEvent.InstanceLoaded, newSceneId, true);
                    ResetPlayerAction();
                });

                CGing = false;
                hasMonster = false;
                touchLastPathPoint = false;
                connoning = false;

                if (inCity || newSceneId == 10100 || newSceneId == 10101 || newSceneId == 10102)
                    canAutoFight = false;
                else
                {
                    canAutoFight = MissionPathPointData.LoadMissionPathPointMessage(newSceneId);
                    //LoggerHelper.Error("************************* " + newSceneId + " " + canAutoFight);
                }

                if (!canAutoFight)
                {
                    //关闭自动战斗按钮
                    MainUIViewManager.Instance.ShowAutoFight(false);
                    //检测指引消失/出现
                    EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
                    return;
                }
                //显示自动战斗按钮

                if (MogoWorld.thePlayer.CheckCurrentMissionComplete())
                {
                    MainUIViewManager.Instance.ShowAutoFight(true);
                }
                else if (newSceneId == 30001 || (newSceneId > 40000 && newSceneId < 40010))
                    MainUIViewManager.Instance.ShowAutoFight(true);
                else
                    MainUIViewManager.Instance.ShowAutoFight(false);

                //MainUIViewManager.Instance.ShowAutoFight(true);
                thePlayer.UpdateAutoFightInfo();

                MissionPathPointData.CreateAllMissionPathPointObject();
                //初始化副本进度点为可走点
                thePlayer.FBProgress = 1;
                MissionPathPointData.SetType(thePlayer.FBProgress - 1, 0);
                //初始化指引指向
                // 默认从第一个pointer点开始
                if (MissionPathPointData.missionPathPoint == null)
                {
                    return;
                }
                int totleCorners = MissionPathPointData.missionPathPoint.Length;
                for (int key = 1; key <= totleCorners; key++)
                {
                    if (MissionPathPointData.missionPathPoint[key - 1].isPointer == 1)
                    {
                        thePlayer.m_iGuideProgress = key;
                        Vector3 tmpDir = new Vector3(
                                MissionPathPointData.missionPathPoint[thePlayer.m_iGuideProgress - 1].pathPoint[0],
                                MissionPathPointData.missionPathPoint[thePlayer.m_iGuideProgress - 1].pathPoint[1],
                                MissionPathPointData.missionPathPoint[thePlayer.m_iGuideProgress - 1].pathPoint[2]);
                        MogoFXManager.Instance.UpdatePointerToTarget(tmpDir);
                        break;
                    }
                }
            });
        });
    }

    static public void LoadLoginScene()
    {
        MogoWorld.scenesManager.LoadLoginScene((isLoadScene) =>
        {
            LoginUILogicManager.Instance.IsAutoLogin = SystemConfig.Instance.IsAutoLogin;
            LoginUILogicManager.Instance.IsSaveUserName = SystemConfig.Instance.IsSavePassport;
            LoginUILogicManager.Instance.UserName = SystemConfig.Instance.Passport;
            LoginUILogicManager.Instance.Password = SystemConfig.Instance.Password;


            //公告板下载与显示
            NoticeManager.Instance.AutoShowNotice();

            DefaultUI.HideLoading();
            MogoUIManager.Instance.FirstPreLoadUIResources();

            var serverInfo = SystemConfig.GetSelectedServerInfo();
            if (serverInfo == null)
            {
                serverInfo = SystemConfig.GetRecommentServer();
                if (serverInfo != null)
                    SystemConfig.SelectedServerIndex = SystemConfig.GetServerIndexById(serverInfo.id);
            }
            if (serverInfo != null)
            {
                SystemConfig.Instance.SelectedServer = serverInfo.id;
                LoginUIViewManager.Instance.SetServerName(serverInfo.name);
                SystemConfig.SaveConfig();
            }
        }, DefaultUI.Loading);
    }

    /// <summary>
    /// 加载角色管理场景，并根据角色列表情况显示对应功能界面。
    /// </summary>
    static public void LoadCharacterScene()
    {
        //MogoMsgBox.Instance.ShowFloatingTextQueue("LoadCharacterScene0");
        theAccount.RpcCall("SetPlatAccountReq", SystemConfig.Instance.Passport);
        LoadCharacterScene(theAccount.UpdateCharacterList);
    }

    /// <summary>
    /// 加载角色管理场景，并根据角色列表情况显示对应功能界面。
    /// </summary>
    /// <param name="loaded"></param>
    static public void LoadCharacterScene(Action loaded)
    {
        if (theAccount != null)
        {
            bool isCreateCharacter = theAccount.avatarList == null || theAccount.avatarList.Count == 0;
            if (isCreateCharacter)
                GameProcManager.SetGameProgress(ProcType.ChangeScene, globalSetting.chooseCharaterScene, true);
            //MogoWorld.SetGameProgress(ProgressCode.s10003_GoToCreateCharacter_Begin);

            scenesManager.LoadCharacterScene(() =>
            {
                if (isCreateCharacter)
                    GameProcManager.SetGameProgress(ProcType.ChangeScene, globalSetting.chooseCharaterScene, false);
                //MogoWorld.SetGameProgress(ProgressCode.s10003_GoToCreateCharacter_End);
                if (loaded != null)
                    loaded();
                if (isFirstTimeInCity)//判断是否第一次进王城，是再进行UI的预加载
                {
                    MogoUIManager.Instance.SecondPreLoadUIResources();
                    isFirstTimeInCity = false;
                }

            }, null, isCreateCharacter, true);
        }
    }

    public static List<EntityParent> GetEntitiesButSelf()
    {
        List<EntityParent> rst = new List<EntityParent>();
        foreach (var i in entities)
        {
            if (i.Value is EntityMyself)
            {
                continue;
            }
            rst.Add(i.Value);
        }
        return rst;
    }

    private static Dictionary<uint, Vector3> entitiesPos = new Dictionary<uint, Vector3>();
    public static void RemoveEntitiesPos()
    {
        foreach (var i in entities)
        {
            if (i.Value is EntityMyself)
            {
                continue;
            }
            if (i.Value.Transform == null)
            {
                continue;
            }
            if (entitiesPos.ContainsKey(i.Key))
            {
                entitiesPos.Remove(i.Key);
            }
            entitiesPos.Add(i.Key, i.Value.Transform.position);
            i.Value.Transform.position -= new Vector3(0, 10000, 0);
        }
    }

    public static void ResetEntitiesPos()
    {
        foreach (var i in entities)
        {
            if (i.Value is EntityMyself)
            {
                continue;
            }
            if (!entitiesPos.ContainsKey(i.Key) || i.Value.Transform == null)
            {
                continue;
            }
            i.Value.Transform.position = entitiesPos[i.Key];
        }
        entitiesPos.Clear();
    }

    static public void SwitchLightMapFog(int sceneId)
    {
        scenesManager.SwitchLightMapFog(sceneId, null);
    }

    public static void SetGameProgress(ushort progress)
    {
        LoggerHelper.Info("progress: " + progress);
        if (m_currentEntity != null)
            m_currentEntity.RpcCall("SetProgress", progress);
    }

    #endregion

    #region 私有方法

    private static void AddListeners()
    {
        EventDispatcher.AddEventListener<string, int>(Events.NetworkEvent.Connect, ConnectServer);

        // 增加 框架 协议处理函数
        EventDispatcher.AddEventListener<LoginResult>(Events.FrameWorkEvent.Login, OnLoginResp);
        EventDispatcher.AddEventListener<string, int, string>(Events.FrameWorkEvent.BaseLogin, OnBaseLogin);
        EventDispatcher.AddEventListener<BaseAttachedInfo>(Events.FrameWorkEvent.EntityAttached, OnEntityAttached);
        EventDispatcher.AddEventListener<CellAttachedInfo>(Events.FrameWorkEvent.EntityCellAttached, OnEntityCellAttached);
        EventDispatcher.AddEventListener<CellAttachedInfo>(Events.FrameWorkEvent.AOINewEntity, AOINewEntity);
        EventDispatcher.AddEventListener<uint>(Events.FrameWorkEvent.AOIDelEvtity, AOIDelEntity);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, SceneLoaded);
        EventDispatcher.AddEventListener(Events.OtherEvent.MainCameraComplete, MainCameraCompleted);
        EventDispatcher.AddEventListener<int, float>(Events.OtherEvent.ChangeDummyRate, ChangeDummyRate);
        EventDispatcher.AddEventListener(Events.OtherEvent.ResetDummyRate, DelChangeDummyRate);

        EventDispatcher.AddEventListener<DefCheckResult>(Events.FrameWorkEvent.CheckDef, OnCheckDefMD5);

        EventDispatcher.AddEventListener<int>(Events.GearEvent.SwitchLightMapFog, SwitchLightMapFog);
        EventDispatcher.AddEventListener<string>(Events.FrameWorkEvent.ReConnectKey, ReConnectKeyHandler);
        EventDispatcher.AddEventListener(Events.FrameWorkEvent.ReConnectRefuse, ReConnectRefuseHandler);
        EventDispatcher.AddEventListener(Events.FrameWorkEvent.DefuseLogin, DefuseLoginHandler);
    }

    private static void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener<string, int>(Events.NetworkEvent.Connect, ConnectServer);

        // 增加 框架 协议处理函数
        EventDispatcher.RemoveEventListener<LoginResult>(Events.FrameWorkEvent.Login, OnLoginResp);
        EventDispatcher.RemoveEventListener<string, int, string>(Events.FrameWorkEvent.BaseLogin, OnBaseLogin);
        EventDispatcher.RemoveEventListener<BaseAttachedInfo>(Events.FrameWorkEvent.EntityAttached, OnEntityAttached);
        EventDispatcher.RemoveEventListener<CellAttachedInfo>(Events.FrameWorkEvent.EntityCellAttached, OnEntityCellAttached);
        EventDispatcher.RemoveEventListener<CellAttachedInfo>(Events.FrameWorkEvent.AOINewEntity, AOINewEntity);
        EventDispatcher.RemoveEventListener<uint>(Events.FrameWorkEvent.AOIDelEvtity, AOIDelEntity);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, SceneLoaded);
        EventDispatcher.RemoveEventListener(Events.OtherEvent.MainCameraComplete, MainCameraCompleted);
        EventDispatcher.RemoveEventListener<int, float>(Events.OtherEvent.ChangeDummyRate, ChangeDummyRate);
        EventDispatcher.RemoveEventListener(Events.OtherEvent.ResetDummyRate, DelChangeDummyRate);

        EventDispatcher.RemoveEventListener<DefCheckResult>(Events.FrameWorkEvent.CheckDef, OnCheckDefMD5);

        EventDispatcher.RemoveEventListener<int>(Events.GearEvent.SwitchLightMapFog, SwitchLightMapFog);
        EventDispatcher.RemoveEventListener<string>(Events.FrameWorkEvent.ReConnectKey, ReConnectKeyHandler);
        EventDispatcher.RemoveEventListener(Events.FrameWorkEvent.ReConnectRefuse, ReConnectRefuseHandler);
        EventDispatcher.RemoveEventListener(Events.FrameWorkEvent.DefuseLogin, DefuseLoginHandler);
    }

    static private void ReConnectKeyHandler(string key)
    {
        reConnectKey = key;
    }

    static private void ReConnectRefuseHandler()
    {
        MogoWorld.beKick = true;
        MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(25560), (ok) =>
        {
            MogoGlobleUIManager.Instance.ConfirmHide();
        }, LanguageData.GetContent(25561));
    }

    static private void DefuseLoginHandler()
    {
        MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(25563), (ok) =>
        {
            MogoGlobleUIManager.Instance.ConfirmHide();
        }, LanguageData.GetContent(25561));
    }

    private static void LoginReq(bool rst)
    {
        if (rst)
        {
            //MogoMsgBox.Instance.ShowFloatingTextQueue("rst:" + rst);
            if (MogoWorld.rc)
            {
                LoggerHelper.Error("send rc key " + reConnectKey);
                ServerProxy.Instance.SendReConnectKey(reConnectKey);
                MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "");
                return;
            }
            if (SystemSwitch.UsePlatformSDK && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
                ServerProxy.Instance.BaseLogin(baseLoginKey);
            else
                ServerProxy.Instance.Login(LoginInfo.GetPCStrList());

            CheckDefMD5();
        }
        else
        {
            if (MogoWorld.rc)
            {
                return;
            }
            MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "");
            MogoMessageBox.Info("connect server failed.");
        }
    }

    static private void LoadHomeScene(int orgSceneId)
    {
        isReConnect = false;//进主城就可以把重连状态去除
        ClearEntities();
        if (orgSceneId > 10100)
            AssetCacheMgr.AssetMgr.ClearLoadAssetTasks();
        GameProcManager.ChangeScene(orgSceneId, globalSetting.homeScene, true);

        scenesManager.LoadHomeScene((isLoadScene) =>
        {
            if (isLoadScene)
            {
                Container.ClearAllContainers();
                LoadGears(globalSetting.homeScene, () =>
                {
                    if (orgSceneId != 0)
                        EventDispatcher.TriggerEvent(Events.InstanceEvent.InstanceUnLoaded, orgSceneId, false);
                    EventDispatcher.TriggerEvent(Events.InstanceEvent.InstanceLoaded, globalSetting.homeScene, false);

                    ResetPlayerAction(-1);

                    if (PlatformSdkManager.Instance != null)
                    {
                        PlatformSdkManager.Instance.ShowAssistant();
                    }

                });
            }
            GameProcManager.ChangeScene(orgSceneId, globalSetting.homeScene, false);
        });
    }

    static private void SceneLoaded(int sceneId, bool isInstance)
    {
        try
        {
            foreach (var item in entities)
            {
                item.Value.CreateModel();
                //LoggerHelper.Error("SceneLoaded:" + item.Value.ID);
                item.Value.UpdateView();
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
        }
        isLoadingScene = false;
        foreach (var m in entities)
        {
            if (m.Value is EntityMonster || m.Value is EntityMercenary || m.Value is EntityDummy || (m.Value is EntityPlayer && m.Value.ID != MogoWorld.thePlayer.ID))
            {
                m.Value.UpdatePosition();
            }
        }
        LoggerHelper.Debug("EventDispatcher.theRouter.Count: " + EventDispatcher.TheRouter.Count);
    }

    static private void MainCameraCompleted()
    {
        //if (inCity)
        //{
        //    MogoUIManager.Instance.ShowMogoNormalMainUI();
        //}
        //else
        //{
        //    MogoUIManager.Instance.ShowMogoBattleMainUI();
        //}

        Camera RelatedCam = GameObject.Find("Main_Camera").GetComponentInChildren<Camera>();
        Camera ViewCam = GameObject.Find("MogoMainUI/Camera").GetComponentInChildren<Camera>();
        Camera renderCam = GameObject.Find("MogoMainUI/BillboardCamera").GetComponentInChildren<Camera>();

        ViewCam.GetComponent<BillboardViewManager>().RelatedCamera = RelatedCam;
        ViewCam.GetComponent<BillboardViewManager>().ViewCamera = renderCam;

        //通知主角和其他全部实体场景加载完毕
        thePlayer.MainCameraCompleted();

        mainCameraCompleted = true;
        if (isReConnect)
        {
            if (me != null)
            {
                EventDispatcher.TriggerEvent<CellAttachedInfo>(Events.FrameWorkEvent.EntityCellAttached, me);
                me = null;
            }
            foreach (var i in others)
            {
                EventDispatcher.TriggerEvent<CellAttachedInfo>(Events.FrameWorkEvent.AOINewEntity, i);
            }
            others.Clear();
        }
        if (sceneChanged)
        {//在加载后判断是否加载中变换了场景ID
            if (thePlayer != null && !scenesManager.CheckSameScene(thePlayer.sceneId))
            {
                sceneChanged = false;
                SwitchScene(orgId, thePlayer.sceneId);
            }
        }
        TimerHeap.AddTimer(2000, 0, CityDelayCheck);
    }

    static private void CityDelayCheck()
    {
        thePlayer.FixedErr();
        foreach (var e in Entities)
        {
            if (e.Value is EntityPlayer)
            {
                (e.Value as EntityPlayer).FixedErr();
            }
        }
    }

    static private Vector3 FindEmptyDropPoint(Vector3 src)
    {
        float R = 1.0f; //掉落物半径
        Vector3[] posis = new Vector3[] {
                                        new Vector3(-R, 0, -R),
                                        new Vector3(-R, 0, R),
                                        new Vector3(-R, 0, 0),
                                        new Vector3(0, 0, -R),
                                        new Vector3(0, 0, R),
                                        //Vector3.zero,
                                        new Vector3(R, 0, -R),
                                        new Vector3(R, 0, R),
                                        new Vector3(R, 0, -R)};
        int j = -1;
        for (int i = 0; i < 8; i++)
        {
            Vector3 t = src + posis[i];
            if (!IsHit(t, R))
            {
                Mogo.Util.MogoUtils.GetPointInTerrain(t.x, t.z, out t);
                t = t + new Vector3(0, 0.2f, 0);
                if (t.y > 30)
                {//找不到点时会是50米高,所以做这处理
                    continue;
                }
                if (!(thePlayer.motor as MogoMotorMyself).CanMoveTo(t))
                {
                    continue;
                }
                j = i;
                break;
            }
        }
        if (j == -1)
        {//如果没有位置就掉到主角脚下
            return src;// thePlayer.Transform.position;
        }
        Vector3 rst = Vector3.zero;
        rst = src + posis[j];
        return rst;
    }

    static private bool IsHit(Vector3 v, float r)
    {
        foreach (var item in dropPoints)
        {
            if (Math.Abs(Vector3.Distance(v, item.Value)) <= r * 0.5f)
            {
                return true;
            }
        }
        return false;
    }

    static private void ChangeDummyRate(int time, float rate)
    {
        foreach (var item in Entities)
        {
            if (!(item.Value is EntityDummy))
            {
                continue;
            }
            if (item.Value.animator == null)
            {
                continue;
            }
            item.Value.animator.speed *= rate;
            item.Value.aiRate = rate;
            item.Value.motor.HandleAiRateChange(rate);
        }
        TimerHeap.DelTimer(chgDummyRateTimer);
        chgDummyRateTimer = TimerHeap.AddTimer((uint)time, 0, ResetDummyRate);
    }

    static private void ResetDummyRate()
    {
        foreach (var item in Entities)
        {
            if (!(item.Value is EntityDummy))
            {
                continue;
            }
            if (item.Value.animator)
                item.Value.animator.speed = 1;
            item.Value.aiRate = 1;
            item.Value.motor.HandleAiRateChange(1.0f);
        }
    }

    static private void DelChangeDummyRate()
    {
        TimerHeap.DelTimer(chgDummyRateTimer);
        ResetDummyRate();
    }

    private static void ResetPlayerAction(int action = 0)
    {
        if (ControlStick.instance != null)
            ControlStick.instance.Reset();

        if (MogoWorld.thePlayer != null)
        {
            if (MogoWorld.thePlayer.deathFlag == 1)
            {
                MogoWorld.thePlayer.Revive();
            }
            else
            {
                MogoWorld.thePlayer.SetAction(action);
            }
        }
    }

    #endregion

    #region Gear

    static public void LoadGears(int id, Action action)
    {
        try
        {
            var gearDataMap = GetAllGearDataInMap(id);
            Dictionary<string, GearParent> gearMap = BuildGearMap(gearDataMap);

            // 脚本添加相对于加载资源也是异步的操作，这里填写1是防止填充完脚本之前资源加载完成进入场景
            asyncGearNum = 1;

            if (gearMap.Count == 0)
            {
                CheckAsyncGearEnd(action);
                return;
            }

            foreach (var data in gearDataMap)
            {
                #region 准备工作，依次是找到机关挂载点，取出机关，找到机关类型，建立类型字典，解析参数，建立参数字典

                GameObject go = GameObject.Find(data.Value.gameObjectName);
                if (go == null)
                {
                    // LoggerHelper.Debug("Name Path Wrong" + data.Value.gameObjectName);
                    continue;
                }

                var component = go.GetComponent(data.Value.type);

                Type type = Type.GetType(data.Value.type);

                Dictionary<string, FieldInfo> fieldDict = BuildFieldMap(type);

                string[] argName = data.Value.argNames.Split('|');
                string[] argType = data.Value.argTypes.Split('|');
                string[] arg = data.Value.args.Split('|');

                if (argName.Length != argType.Length || arg.Length != argType.Length || arg.Length != argName.Length)
                {
                    // LoggerHelper.Debug("Length Wrong: " + data.Value.gameObjectName + " name: " + argName.Length + " type: " + argType.Length + " arg: " + arg.Length);
                    continue;
                }

                Dictionary<string, string> argDict = new Dictionary<string, string>();
                for (int i = 0; i < argName.Length; i++)
                {
                    argDict.Add(argName[i], arg[i]);
                }

                #endregion

                #region 根据反射的类型，依次设置机关的值

                foreach (var field in type.GetFields())
                {
                    if (!argDict.ContainsKey(field.Name))
                    {
                        break;
                    }

                    Type fieldType = field.FieldType;
                    string argsString = argDict[field.Name];

                    #region 处理数组

                    if (fieldType.IsArray)
                    {
                        Type elementType = fieldType.GetElementType();
                        string[] argString = argsString.Split(':');

                        if (elementType.IsSubclassOf(typeof(GearParent)) || elementType == typeof(GearParent))
                        {
                            object array = new object();

                            array = fieldType.InvokeMember("Set", BindingFlags.CreateInstance, null, null, new object[] { argString.Length });
                            var setValue = fieldType.GetMethod("SetValue", new Type[2] { typeof(object), typeof(int) });
                            for (int j = 0; j < argString.Length; j++)
                            {
                                setValue.Invoke(array, new object[] { gearMap[argString[j] + elementType.Name], j });
                            }

                            field.SetValue(component, array);
                        }
                        else if (elementType == typeof(GameObject))
                        {
                            List<GameObject> argComponents = new List<GameObject>();

                            for (int j = 0; j < argString.Length; j++)
                            {
                                GameObject argGo = GameObject.Find(argString[j]);
                                if (argGo == null)
                                {
                                    // LoggerHelper.Debug("arg name path wrong: " + data.Value.gameObjectName + " no " + field.Name + " no " + argString[j]);
                                    break;
                                }

                                argComponents.Add(argGo);
                            }

                            field.SetValue(component, argComponents.ToArray());
                        }
                        else if (elementType == typeof(Transform))
                        {
                            List<Transform> argComponents = new List<Transform>();

                            for (int j = 0; j < argString.Length; j++)
                            {
                                GameObject argGo = GameObject.Find(argString[j]);
                                if (argGo == null)
                                {
                                    //  LoggerHelper.Debug("arg name path wrong: " + data.Value.gameObjectName + " no " + field.Name + " no " + argString[j]);
                                    break;
                                }

                                argComponents.Add(argGo.transform);
                            }

                            field.SetValue(component, argComponents.ToArray());
                        }
                        else if (elementType == typeof(AnimationClip))
                        {
                            AnimationClip[] argComponents = new AnimationClip[argString.Length];

                            for (int j = 0; j < argString.Length; j++)
                            {
                                asyncGearNum++;
                                int k = j;
                                SubAssetCacheMgr.GetGearResrouce(argString[j],
                                    (obj) =>
                                    {
                                        argComponents[k] = obj as AnimationClip;
                                        CheckAsyncGearEnd(action);
                                    });
                            }

                            field.SetValue(component, argComponents);
                        }
                        else if (elementType == typeof(Animation))
                        {
                            List<Animation> argComponents = new List<Animation>();

                            for (int j = 0; j < argString.Length; j++)
                            {
                                GameObject argGo = GameObject.Find(argString[j]);
                                if (argGo == null)
                                {
                                    //  LoggerHelper.Debug("arg name path wrong: " + data.Value.gameObjectName + " no " + field.Name + " no " + argString[j]);
                                    break;
                                }

                                argComponents.Add(argGo.GetComponent<Animation>());
                            }

                            field.SetValue(component, argComponents.ToArray());
                        }
                        else if (elementType == (typeof(Vector3)))
                        {
                            List<Vector3> argComponents = new List<Vector3>();

                            for (int j = 0; j < argString.Length; j++)
                            {
                                string[] vs = argString[j].Split(',');
                                Vector3 v = new Vector3(float.Parse(vs[0]), float.Parse(vs[1]), float.Parse(vs[2]));

                                argComponents.Add(v);
                            }

                            field.SetValue(component, argComponents.ToArray());
                        }
                        else if (elementType == (typeof(int)))
                        {
                            List<int> argComponents = new List<int>();

                            for (int j = 0; j < argString.Length; j++)
                            {
                                if (argString[j] != String.Empty)
                                {
                                    int intVal = int.Parse(argString[j]);
                                    argComponents.Add(intVal);
                                }
                            }

                            field.SetValue(component, argComponents.ToArray());
                        }
                        else
                        {
                            System.Object[] obj = new System.Object[argString.Length];

                            for (int j = 0; j < argString.Length; j++)
                            {
                                obj = argString;
                            }

                            field.SetValue(component, obj);
                        }
                    }
                    #endregion

                    #region 处理单个对象
                    else
                    {
                        if (fieldType.IsSubclassOf(typeof(GearParent)))
                        {
                            GameObject argGo = GameObject.Find(argsString);
                            if (argGo == null)
                            {
                                // LoggerHelper.Debug("arg name path wrong: : " + data.Value.gameObjectName + " no " + argsString);
                                break;
                            }

                            if (gearMap.ContainsKey(argsString + fieldType.Name))
                            {
                                var argComponent = gearMap[argsString + fieldType.Name];
                                field.SetValue(component, argComponent);
                            }
                        }
                        else if (fieldType == (typeof(GameObject)))
                        {
                            GameObject argGo = GameObject.Find(argsString);
                            if (argGo == null)
                            {
                                // LoggerHelper.Debug("arg name path wrong: : " + data.Value.gameObjectName + " no " + argsString);
                                break;
                            }
                            field.SetValue(component, argGo);
                        }
                        else if (fieldType == (typeof(Transform)))
                        {
                            GameObject argGo = GameObject.Find(argsString);
                            if (argGo == null)
                            {
                                //  LoggerHelper.Debug("arg name path wrong: : " + data.Value.gameObjectName + " no " + argsString);
                                break;
                            }

                            field.SetValue(component, argGo.transform);
                        }
                        else if (fieldType == (typeof(Animation)))
                        {
                            GameObject argGo = GameObject.Find(argsString);
                            if (argGo == null)
                            {
                                //  LoggerHelper.Debug("arg name path wrong: : " + data.Value.gameObjectName + " no " + argsString);
                                break;
                            }

                            field.SetValue(component, argGo.GetComponent<Animation>());
                        }
                        else if (fieldType == (typeof(AnimationClip)))
                        {
                            FieldInfo tempField = field;
                            if (argsString != "null.anim")
                            {
                                asyncGearNum++;
                                SubAssetCacheMgr.GetGearResrouce(argsString,
                                    (obj) =>
                                    {
                                        tempField.SetValue(component, obj);
                                        CheckAsyncGearEnd(action);
                                    });
                            }
                        }
                        else if (fieldType == (typeof(Vector3)))
                        {
                            string[] vs = argsString.Split(',');
                            Vector3 v = new Vector3(float.Parse(vs[0]), float.Parse(vs[1]), float.Parse(vs[2]));

                            field.SetValue(component, v);
                        }
                        else
                        {
                            //Mogo.Util.LoggerHelper.Debug(component.name + " " + argsString + " " + go.name);
                            field.SetValue(component, Mogo.Util.Utils.GetValue(argsString, fieldType));
                        }
                    }
                    #endregion
                }

                #endregion
            }

            CheckAsyncGearEnd(action);
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
            if (action != null)
                action();
        }
    }

    private static Dictionary<int, GearData> GetAllGearDataInMap(int id)
    {
        Dictionary<int, GearData> gearDataMap = new Dictionary<int, GearData>();
        if (id == 0)
            return gearDataMap;

        int trapID = MapData.dataMap.Get(id).trapID;
        foreach (var data in GearData.dataMap)
        {
            if (data.Value.map == trapID)
                // if (data.Value.map == id)
                gearDataMap.Add(data.Key, data.Value);
        }
        return gearDataMap;
    }

    private static Dictionary<string, GearParent> BuildGearMap(Dictionary<int, GearData> gearDataMap)
    {
        Dictionary<string, GearParent> gearMap = new Dictionary<string, GearParent>();

        foreach (var data in gearDataMap)
        {
            GameObject go = GameObject.Find(data.Value.gameObjectName);

            if (go == null)
            {
                // LoggerHelper.Debug("Name Path Wrong" + data.Value.gameObjectName);
                break;
            }

            Type type = Type.GetType(data.Value.type);

            var component = go.AddComponent(type);
            if (gearMap.ContainsKey(data.Value.gameObjectName + data.Value.type))
                Debug.LogError("gearMap contains key: " + data.Value.gameObjectName + data.Value.type);
            else
                gearMap.Add(data.Value.gameObjectName + data.Value.type, (GearParent)component);
        }
        return gearMap;
    }

    private static Dictionary<string, FieldInfo> BuildFieldMap(Type type)
    {
        Dictionary<string, FieldInfo> fieldDict = new Dictionary<string, FieldInfo>();

        foreach (var field in type.GetFields())
        {
            fieldDict.Add(field.Name, field);
        }

        return fieldDict;
    }

    private static void CheckAsyncGearEnd(Action action)
    {
        asyncGearNum--;
        if (asyncGearNum == 0)
        {
            action();
        }
    }

    #endregion

    public class LoginInfo
    {
        public string uid;
        public string timestamp;
        public string strSign;
        public string strPlatId;
        public string strPlatAccount;
        public string token;
        public string platName;

        public string[] GetStrList()
        {
            string[] strs = new string[6];
            strs[0] = uid;
            strs[1] = timestamp;
            strs[2] = strSign;
            strs[3] = strPlatId;
            strs[4] = strPlatAccount;
            strs[5] = token;

            for (int i = 0; i < strs.Length; i++)
            {
                LoggerHelper.Debug(strs[i]);
            }
            return strs;
        }

        static public string[] GetPCStrList()
        {
            string[] strs = new string[6];
            strs[0] = passport;
            strs[1] = "timestamp";
            strs[2] = "strSign";
            strs[3] = "strPlatId";
            strs[4] = "0";
            strs[5] = "token";

            for (int i = 0; i < strs.Length; i++)
            {
                LoggerHelper.Debug(strs[i]);
            }
            return strs;
        }
    }
}