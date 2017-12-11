/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ArenaManager
// 创建者：Charles Zuo
// 修改者列表：
// 创建日期：
// 模块描述：个人竞技场
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.Util;
using Mogo.GameData;

public class ArenaPlayerData
{
    public string name { get; set; }
    public int level { get; set; }
    public int vocation { get; set; }
    public int fightForce { get; set; }
    public List<int> equip { get; set; }
    public int arenaLevel { get; set; } // 竞技场等级
}

public class ArenaPersonalData
{
    public int dayScore { get; set; }
    public int weekScore { get; set; }
    public int challengeTimes { get; set; }
    public int beatEnemy { get; set; } // 是否击败复仇
    public int cd { get; set; }
    public int buyTimes { get; set; }
    public int buff { get; set; } // 鼓舞Buff 
    public List<int> weak { get; set; }
    public List<int> strong { get; set; }
}

public class ArenaRewardData
{
    public int itemid { get; set; }
    public int num { get; set; }
}

static public class ArenaSystemEvent
{
    public readonly static string CanGetScoreRewardsReq = "ArenaEvent.CanGetScoreRewardsReq";
}

public class ArenaManager : IEventManager
{
    private EntityMyself m_myself;
    public static int COUNTDOWN = 6;
    public ArenaManager(EntityMyself _myself)
    {
        m_myself = _myself;
        AddListeners();        
    }

    public void AddListeners()
    {
        EventDispatcher.AddEventListener(Events.ArenaEvent.RefreshWeak, RefreshWeak);
        EventDispatcher.AddEventListener(Events.ArenaEvent.RefreshStrong, RefreshStrong);
        EventDispatcher.AddEventListener(Events.ArenaEvent.RefreshRevenge, RefreshRevenge);
        EventDispatcher.AddEventListener(Events.ArenaEvent.RefreshArenaData, RefreshArenaData);
        EventDispatcher.AddEventListener<int>(Events.ArenaEvent.Challenge, Challenge);
        EventDispatcher.AddEventListener(Events.ArenaEvent.EnterArena, EnterArena);
        EventDispatcher.AddEventListener(Events.ArenaEvent.ClearArenaCD, ClearArenaCD);
        EventDispatcher.AddEventListener(Events.ArenaEvent.AddArenaTimes, AddArenaTimes);
        EventDispatcher.AddEventListener(Events.ArenaEvent.GetArenaRewardInfo, GetArenaReward);
        EventDispatcher.AddEventListener<int>(Events.ArenaEvent.GetArenaReward, GetSpecificArenaReward);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, OnInstanceLeave);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, OnInstanceEnter);
        EventDispatcher.AddEventListener(ArenaSystemEvent.CanGetScoreRewardsReq, CanGetScoreRewardsReq);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, OnInstanceLoaded);  
    }

    public void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener(Events.ArenaEvent.RefreshWeak, RefreshWeak);
        EventDispatcher.RemoveEventListener(Events.ArenaEvent.RefreshStrong, RefreshStrong);
        EventDispatcher.RemoveEventListener(Events.ArenaEvent.RefreshRevenge, RefreshRevenge);
        EventDispatcher.RemoveEventListener(Events.ArenaEvent.RefreshArenaData, RefreshArenaData);
        EventDispatcher.RemoveEventListener<int>(Events.ArenaEvent.Challenge, Challenge);
        EventDispatcher.RemoveEventListener(Events.ArenaEvent.EnterArena, EnterArena);
        EventDispatcher.RemoveEventListener(Events.ArenaEvent.ClearArenaCD, ClearArenaCD);
        EventDispatcher.RemoveEventListener(Events.ArenaEvent.AddArenaTimes, AddArenaTimes);
        EventDispatcher.RemoveEventListener(Events.ArenaEvent.GetArenaRewardInfo, GetArenaReward);
        EventDispatcher.RemoveEventListener<int>(Events.ArenaEvent.GetArenaReward, GetSpecificArenaReward);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, OnInstanceLeave);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, OnInstanceEnter);
        EventDispatcher.RemoveEventListener(ArenaSystemEvent.CanGetScoreRewardsReq, CanGetScoreRewardsReq);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, OnInstanceLoaded);  
    }

    private void OnInstanceLeave(int sceneID, bool isInstance)
    {
        if (MapData.dataMap.Get(sceneID).type == MapType.ARENA)
        {
            MainUIViewManager.Instance.SetHpBottleVisible(true);
            MogoWorld.thePlayer.RemoveFx(6029);
        }
    }

    bool IsFristInstanceLoaded = true;
    private void OnInstanceLoaded(int sceneID, bool isInstance)
    {
        if (IsFristInstanceLoaded && sceneID == MogoWorld.globalSetting.homeScene)
        {
            IsFristInstanceLoaded = false;
            EventDispatcher.TriggerEvent(ArenaSystemEvent.CanGetScoreRewardsReq);
        }
    }

    private void OnInstanceEnter(int sceneID, bool isInstance)
    {
        if (MapData.dataMap.Get(sceneID).type == MapType.ARENA)
        {
            //if (NormalMainUIViewManager.Instance != null)
            //    NormalMainUIViewManager.Instance.StopArenaCDTip();

            MainUIViewManager.Instance.SetHpBottleVisible(false);
            MogoWorld.arenaState = 1;
            MainUIViewManager.Instance.ShowNormalAttackButton(false);
            MainUIViewManager.Instance.ShowSkillButton(false, 0);
            MainUIViewManager.Instance.ShowSkillButton(false, 1);
            MainUIViewManager.Instance.ShowSkillButton(false, 2);
            MainUIViewManager.Instance.ShowSpriteSkillButton(false);
            MainUIViewManager.Instance.SetSelfAttackText(LanguageData.dataMap.Get(25029).Format(COUNTDOWN));
            TimerShow(COUNTDOWN);
            MogoWorld.thePlayer.PlayFx(6029);
        }
    }

    private void TimerShow(int totalSeconds)
    {
        TimerHeap.AddTimer<int>(1000, 0, 
            (sec) => 
            {
                if (sec > 0)
                {
                    sec--;
                    MainUIViewManager.Instance.SetSelfAttackText(sec.ToString(), false);
                    TimerShow(sec);
                }
                else 
                {
                    //MogoWorld.arenaState = 1;
                    ClientEventData.TriggerGearEvent(181);
                    MainUIViewManager.Instance.ShowNormalAttackButton(true);
                    MainUIViewManager.Instance.ShowSkillButton(true, 0);
                    MainUIViewManager.Instance.ShowSkillButton(true, 1);
                    MainUIViewManager.Instance.ShowSkillButton(true, 2);
                    MainUILogicManager.Instance.ShowSpriteSkillButton();
                    MainUIViewManager.Instance.SetSelfAttackText(String.Empty, false);
                }
            },
            totalSeconds);
    }

    #region  RPC请求

    /// <summary>
    /// 刷新弱敌
    /// </summary>
    private void RefreshWeak()
    {
        LoggerHelper.Debug("RefreshWeak");
        m_myself.RpcCall("RefreshWeakReq");
    }

    /// <summary>
    /// 刷新强敌
    /// </summary>
    private void RefreshStrong()
    {
        LoggerHelper.Debug("RefreshStrong");
        m_myself.RpcCall("RefreshStrongReq");
    }

    /// <summary>
    /// 仇敌Buff
    /// </summary>
    private void RefreshRevenge()
    {
        LoggerHelper.Debug("RevengeBuff");
        m_myself.RpcCall("RevengeBuffReq");
    }

    /// <summary>
    /// 刷新竞技场数据
    /// </summary>
    private void RefreshArenaData()
    {
        m_myself.RpcCall("RefreshArenaDataReq");
    }

    /// <summary>
    /// 挑战竞技场
    /// </summary>
    private void Challenge(int level)
    {
        LoggerHelper.Debug(level);
        m_myself.RpcCall("ChallengeReq", level);
    }

    /// <summary>
    /// 打开竞技场
    /// </summary>
    private void EnterArena()
    {
        LoggerHelper.Debug("EnterArena");
        m_myself.RpcCall("EnterArenaReq");
    }

    /// <summary>
    /// 清除竞技场CD
    /// </summary>
    private void ClearArenaCD()
    {
        m_myself.RpcCall("ClearArenaCDReq");
    }

    /// <summary>
    /// 增加挑战次数
    /// </summary>
    private void AddArenaTimes()
    {
        m_myself.RpcCall("AddArenaTimesReq");
    }

    /// <summary>
    /// 获取竞技场奖励界面信息
    /// </summary>
    private void GetArenaReward()
    {
        m_myself.RpcCall("GetArenaRewardInfoReq");
    }

    /// <summary>
    /// 获取某一个奖励
    /// </summary>
    /// <param name="id"></param>
    private void GetSpecificArenaReward(int id)
    {
        m_myself.RpcCall("GetArenaRewardReq",id);
    }

    /// <summary>
    /// 是否有奖励未领取
    /// </summary>
    private void CanGetScoreRewardsReq()
    {
        m_myself.RpcCall("CanGetScoreRewardsReq");
    }

   #endregion

    #region 回调

    /// <summary>
    /// 带一个uint8的参数：0表示无可以领取的积分奖励，非零反之
    /// </summary>
    public void CanGetScoreRewardsResp(byte flag)
    {
        if (flag > 0)
        {
            if (ArenaUILogicManager.Instance != null)
                ArenaUILogicManager.Instance.ShowRewardNotice(true);

            if (NewArenaUILogicManager.Instance != null)
                NewArenaUILogicManager.Instance.ShowRewardNotice(true);

            TipViewData viewData = new TipViewData();
            viewData.existTime = 20000;
            viewData.priority = TipManager.TIP_TYPE_ARENA_REWARD;
            viewData.itemId = 0;
            viewData.icon = "zc_jingji_up"; 
            viewData.tipText = LanguageData.GetContent(46757);
            viewData.btnName = LanguageData.GetContent(46758);
            viewData.btnAction = () =>
                {
                    NormalMainUILogicManager.Instance.OnPVPPlayIconUp();
                    ArenaUILogicManager.Instance.OnReward();
                    TipUIManager.Instance.Hide(TipManager.TIP_TYPE_ARENA_REWARD);
                };
            viewData.atlasName = "";

            TipUIManager.Instance.Hide(TipManager.TIP_TYPE_ARENA_REWARD); // 避免多次进入队列
            TipUIManager.Instance.AddTipViewData(viewData);

            EventDispatcher.TriggerEvent(Events.NormalMainUIEvent.ShowArenaIconTip);
        }
        else
        {
            if (ArenaUILogicManager.Instance != null)
                ArenaUILogicManager.Instance.ShowRewardNotice(false);

            if (NewArenaUILogicManager.Instance != null)
                NewArenaUILogicManager.Instance.ShowRewardNotice(false);

            TipUIManager.Instance.Hide(TipManager.TIP_TYPE_ARENA_REWARD);
            EventDispatcher.TriggerEvent(Events.NormalMainUIEvent.HideArenaIconTip);
        }       
    }

    #endregion
}