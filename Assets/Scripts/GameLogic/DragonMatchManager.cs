/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DragonMatchManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013/12/5
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using System;
using Mogo.GameData;

public enum DragonMatchState
{
    begin = 0,
    reward = 1,
    ing = 2,
    done = 3
}

/// <summary>
/// AVATAR_BASE_INFO           =  1,  --角色基本信息
/// AVATAR_BASE_ADVERS         =  2,  --角色袭击对手
///
/// AVATAR_DRAGON_STIME        =  1,   --开始时间戳
/// AVATAR_DRAGON_ATKTIMES     =  2,   --袭击别人的次数
/// AVATAR_DRAGON_ATKTIME      =  3,   --袭击别人时间戳
/// AVATAR_DRAGON_CURRRING     =  4,   --当前的环数
/// AVATAR_DRAGON_REVENGE      =  5,   --复仇次数
/// AVATAR_DRAGON_QUALITY      =  6,   --飞龙的品质

/// </summary>
/// <param name="data"></param>
public class DragonMatchData
{
    private int m_time;
    private int m_matchTimeLeft;
    public uint matchTimeLeft
    {
        get
        {
            //Mogo.Util.LoggerHelper.Debug("time:" + MogoTime.Instance.GetSecond());
            int cd = m_matchTimeLeft - (MogoTime.Instance.GetSecond() - m_time);
            if (cd < 0) return 0;
            else return (uint)cd;
        }
        set
        {
            m_time = MogoTime.Instance.GetSecond();
            m_matchTimeLeft = (int)value;
            //Mogo.Util.LoggerHelper.Debug("time:" + time);
        }
    }
    public int currentHitTime { get; set; }
    public uint hitCD { get; set; }
    public int currentRound { get; set; }
    public int revengeTime { get; set; }
    public byte dragonQuality { get; set; }
    public byte hasReward { get; set; }
    public List<DragonPlayerInfo> playerList { get; set; }
    public byte hasExplore { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public DragonMatchState GetState()
    {
        LoggerHelper.Debug(matchTimeLeft);
        LoggerHelper.Debug(hasReward);
        if (matchTimeLeft > 0) return DragonMatchState.ing;
        if (hasReward == 1) return DragonMatchState.reward;
        if (currentRound == DragonBaseData.dataMap[1].dailyConvoyTimes) return DragonMatchState.done;
        return DragonMatchState.begin;
    }


}

/// <summary>
/// ADVERSARY_INFO_DBID        =  1,  --角色dbid
/// ADVERSARY_INFO_FFORCE      =  2,  --角色战斗力
/// ADVERSARY_INFO_GUILD       =  3,  --角色公会名称
/// ADVERSARY_INFO_QUALITY     =  4,  --角色飞龙品质
/// ADVERSARY_INFO_ASTATUS     =  5,  --角色袭击状态
/// ADVERSARY_INFO_ATIMES      =  6,  --角色被成功袭击次数
/// ADVERSARY_INFO_REWARD      =  7,  --角色袭击成功可获得的奖励
/// ADVERSARY_INFO_LEVEL       =  8,  --角色等级
/// ADVERSARY_INFO_NAME        =  9,  --角色名称
/// 10,胸甲id
/// 11,武器id
/// </summary>
public class DragonPlayerInfo
{
    public UInt64 dbid { get; set; }
    public uint power { get; set; }
    public string tongName { get; set; }
    public byte quality { get; set; }
    public byte attackState { get; set; }
    public uint hittedTime { get; set; }
    public Dictionary<int, int> reward { get; set; }
    public int level { get; set; }
    public string name { get; set; }

    public uint cuirass { get; set; }
    public uint weapon { get; set; }
    public uint vocation { get; set; }

    public string GetStatusInfo()
    {
        //Debug.LogError("attackState:" + attackState);
        if (attackState == 0)//
            return LanguageData.GetContent(26305);
        else if (hittedTime >= DragonBaseData.dataMap[1].convoyAttackedTimes)
            return LanguageData.GetContent(26306);
        else return LanguageData.GetContent(26318);
    }
}

public class DragonMatchEventRecordData
{
    public UInt64 dbid { get; set; }
    public byte type { get; set; }
    public byte quality { get; set; }
    public Dictionary<int, int> reward { get; set; }
    public uint timeStamp { get; set; }
    /// <summary>
    /// 0等待复仇，1已复仇，2不需复仇
    /// </summary>
    public byte revengeState { get; set; }
    public string name { get; set; }

    public bool CanRevenge()
    {
        return revengeState == 0;
    }

    public bool HasRevenged()
    {
        return revengeState == 1;
    }

    public string GetInfo()
    {

        int contentId = DragonEventDescData.dataMap.Get(type).desc;
        DragonQualityData dragon = DragonQualityData.GetDragonQualityData(quality);

        int exp = 0;
        int gold = 0;
        if (reward.ContainsKey(2))
            gold = reward[2];
        if (reward.ContainsKey(1))
            exp = reward[1];
        string msg = LanguageData.GetContent(contentId);
        LoggerHelper.Debug("msg:" + msg);
        LoggerHelper.Debug("type:" + type);

        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

        dtStart = dtStart.AddSeconds(timeStamp);

        if (type == 1)
        {

            msg = string.Format(msg, dtStart.ToString("[yyyy-MM-dd HH:mm]"), dragon.GetName(true), gold, exp);
        }
        else
        {
            msg = string.Format(msg, dtStart.ToString("[yyyy-MM-dd HH:mm]"), name, dragon.GetName(true), gold, exp);
        }
        LoggerHelper.Debug(msg);
        return msg;

    }
}

public class DragonMatchManager
{
    public static int COUNTDOWN = 6;
    static DragonMatchManager m_instance;

    public static DragonMatchManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                Init();

            }
            return m_instance;
        }
    }

    public static void Init()
    {
        m_instance = new DragonMatchManager();
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, OnInstanceLeave);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, OnInstanceEnter);
        EventDispatcher.AddEventListener(DoorOfBurySystem.ON_CHALLENGE_SHOW, OnChallengeShow);

        EventDispatcher.AddEventListener(Events.ChallengeUIEvent.CollectChallengeState, OnCollectChallengeState);
    }

    private static void OnCollectChallengeState()
    {
        OnChallengeShow();
        //DragonBaseData data = DragonBaseData.dataMap.Get(1);
        //if (data.levelNeed > MogoWorld.thePlayer.level)
        //{
        //    SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.Lock);
        //}
        //else
        //{
        //    SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.Close);
        //}

    }

    private static void SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState challengeState)
    {
        ChallengeUIGridMessage msg = new ChallengeUIGridMessage();
        msg.challengeID = ChallengeGridID.DragonMatch;
        msg.state = challengeState;
        EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, msg);
    }

    static private void TimerShow(int totalSeconds)
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


    DragonMatchData m_currentData = new DragonMatchData();
    private int m_startMode = 0;
    private List<DragonMatchRecordGridData> m_RecordViewData;
    private List<DragonMatchEventRecordData> m_RecordData;
    private int m_selectedPlayer;
    private bool m_isShowing = false;
    Dictionary<int, int> Rewards
    {
        get
        {
            return DragonRewardsData.GetReward(MogoWorld.thePlayer.level, m_currentData.currentRound + 1, m_currentData.dragonQuality);
        }

    }

    static private void OnChallengeShow()
    {
        LoggerHelper.Debug("RemainConvoyTimesReq");
        DragonBaseData data = DragonBaseData.dataMap.Get(1);
        if (data.levelNeed > MogoWorld.thePlayer.level)
        {
            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.DragonMatch,
               MogoUtils.GetRedString(LanguageData.dataMap[26346].Format(data.levelNeed)));
            ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.DragonMatch, true);
            ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.DragonMatch, false);
            SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.Lock);
        }
        else
        {
            ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.DragonMatch, false);
            DragonMatchManager.Instance.CheckDragonStatus();
            //RpcCall("RemainConvoyTimesReq");
        }
    }

    public void RemainConvoyTimesResp(byte times)
    {
        LoggerHelper.Debug("RemainConvoyTimesResp:" + times);
        ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.DragonMatch, LanguageData.dataMap[26345].Format(times));

        if (times > 0)
        {
            SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.Open);
        }
        else
        {
            SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.Close);
        }

    }

    public void OnShow()
    {
        DragonBaseData data = DragonBaseData.dataMap.Get(1);
        if (data.levelNeed > MogoWorld.thePlayer.level)
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[26346].Format(data.levelNeed));
            return;
        }
        //请求数据
        m_isShowing = true;
        DragonMatchDataReq();
    }

    private void DragonMatchDataReq()
    {
        LoggerHelper.Debug("DragonMatchDataReq");
        RpcCall("OnlineDragonInfoReq");
    }

    public void OnMatchDataResp(LuaTable data)
    {
        LoggerHelper.Debug(data);
        if (data != null)
            m_currentData = GetData(data);
        if (MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_DragonMatchUI
            || MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_DragonMatchRecordUI
            || MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_ChooseDragonUI
            || m_isShowing)
        {
            m_isShowing = false;
            if (MogoWorld.thePlayer.isInCity)
            {
                //Debug.LogError("m_isShowing:" + m_isShowing);
                ShowMainUI();
            }

        }
        m_startMode = 0;
    }

    public void ShowMainUI()
    {
        LoggerHelper.Debug("ShowMainUI");
        bool showExplore = false;
        bool showStartMatchButton = false;
        bool showPlayerinfo = false;
        bool showBeginInfo = false;
        bool showReward = false;
        bool showRefresh = false;
        //List<string> playersName = new List<string>();
        List<uint> playersID = new List<uint>();
        if (m_currentData.playerList != null)
        {
            for (int i = 0; i < m_currentData.playerList.Count; i++)
            {
                LoggerHelper.Debug(m_currentData.playerList[i].name);
                //playersName.Add(m_currentData.playerList[i].name);
                playersID.Add((uint)m_currentData.playerList[i].dbid);
            }
        }
        EventDispatcher.TriggerEvent<int>(Events.NormalMainUIEvent.HideChallegeIconTip, (int)ChallengeGridID.DragonMatch);
        switch (m_currentData.GetState())
        {
            case DragonMatchState.begin:
                showStartMatchButton = true;
                if (m_currentData.currentRound >= 1)
                    showExplore = true;
                break;
            case DragonMatchState.reward:
                EventDispatcher.TriggerEvent<int>(Events.NormalMainUIEvent.ShowChallegeIconTip, (int)ChallengeGridID.DragonMatch);
                showReward = true;
                break;
            case DragonMatchState.ing:
                showRefresh = true;
                showBeginInfo = true;
                break;
            case DragonMatchState.done:
                break;
        }
        LoggerHelper.Debug("ShowDragonMatchUI");

        // 第一次加载需要弹Loading，在OnEnable去掉，由资源加载控制Loading
        if (MogoUIManager.Instance.m_DragonMatchUI == null)
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        MogoUIManager.Instance.ShowDragonMatchUI(
                   () =>
                   {
                       LoggerHelper.Debug("onShowDragonMatchUI done");
                       DragonMatchUIViewManager.Instance.SetHitTimes(String.Concat(m_currentData.currentHitTime, "/", DragonBaseData.dataMap[1].convoyAttackedTimes));
                       DragonMatchUIViewManager.Instance.SetCurrentRoundTimes(String.Concat(m_currentData.currentRound, "/", DragonBaseData.dataMap[1].dailyConvoyTimes));
                       DragonMatchUIViewManager.Instance.ShowMatchRound(m_currentData.currentRound);

                       DragonMatchUIViewManager.Instance.ShowBtnExplore(showExplore);
                       DragonMatchUIViewManager.Instance.ShowDragonMatchPlayerInfoUI(showPlayerinfo);
                       DragonMatchUIViewManager.Instance.ShowStartMatchButton(showStartMatchButton);

                       DragonMatchUIViewManager.Instance.ShowEndMatchTreasure(showReward);
                       DragonMatchUIViewManager.Instance.ShowBtnRefresh(showRefresh);
                       DragonMatchUIViewManager.Instance.ShowBeginInfo(showBeginInfo, (int)m_currentData.matchTimeLeft, DragonMatchDataReq);

                       //DragonMatchUIViewManager.Instance.ShowDragonMatchPlayerList(playersName);
                       DragonMatchUIViewManager.Instance.SetDragonDataList(m_currentData.playerList);

                       LoggerHelper.Debug("m_currentData.hitCD:" + (int)m_currentData.hitCD);
                       DragonMatchUIViewManager.Instance.ShowHitCD(m_currentData.hitCD > 0, (int)m_currentData.hitCD, DragonMatchDataReq);

                       TipUIManager.Instance.HideAll(TipManager.TIP_TYPE_DRAGON_MATCH);
                   });
    }

    private DragonMatchData GetData(LuaTable luatable)
    {
        object obj;
        Utils.ParseLuaTable(luatable, typeof(DragonMatchData), out obj);
        DragonMatchData data = obj as DragonMatchData;
        return data;
    }

    #region 选龙
    public void OnStartMatch()
    {
        //读取龙的配置显示选龙界面
        LoggerHelper.Debug("ShowChooseDragonUI");
        MogoUIManager.Instance.ShowChooseDragonUI(SetupChooseDragonView, true);
    }

    private void SetupChooseDragonView()
    {
        LoggerHelper.Debug("SetupChooseDragonView");
        List<ChooseDragonGridData> chooseDragonGridDataList = new List<ChooseDragonGridData>();
        List<DragonQualityData> dragonDataList = DragonQualityData.GetDataList();
        for (int i = 0; i < dragonDataList.Count; i++)
        {
            ChooseDragonGridData gridData = new ChooseDragonGridData();
            gridData.dragonID = dragonDataList[i].id;
            gridData.dragonQuality = dragonDataList[i].quality;
            if (dragonDataList[i].quality != m_currentData.dragonQuality)
            {
                gridData.enable = false;
            }
            else
            {
                gridData.enable = true;
            }

            gridData.finishTime = LanguageData.dataMap.Get(26300).Format((dragonDataList[i].convoyCompleteTime[m_currentData.currentRound + 1] / 60));
            gridData.additionReward = LanguageData.GetContent(26301, (dragonDataList[i].rewardAddition / 100));
            if (i == 4 && m_currentData.dragonQuality != 6)
            {
                gridData.showBuy = true;
            }
            else
            {

                gridData.showBuy = false;
            }
            chooseDragonGridDataList.Add(gridData);
        }
        ChooseDragonUILogicManager.Instance.SetChooseDragonGridDataList(chooseDragonGridDataList);

        DragonQualityData currentDragon = DragonQualityData.GetDragonQualityData(m_currentData.dragonQuality);
        string nextDragonName = string.Empty;
        if (m_currentData.dragonQuality < 6)
        {
            nextDragonName = DragonQualityData.GetDragonQualityData(m_currentData.dragonQuality + 1).GetName(true);
        }
        ChooseDragonUIViewManager.Instance.SetCurrentChooseDragon(currentDragon.GetName(true), nextDragonName);
        ChooseDragonUIViewManager.Instance.SetRewardCurrentTime(m_currentData.currentRound + 1);

        ChooseDragonUIViewManager.Instance.SetRewardExp(Rewards[1]);
        ChooseDragonUIViewManager.Instance.SetRewardGold(Rewards[2]);
        int[] cost = DragonBaseData.GetCostItem();
        int costNum = cost[1];
        int costID = cost[0];
        int materialNum = InventoryManager.Instance.GetItemNumById(costID);
        string costStr = string.Empty;

        if (costNum <= materialNum)
        {
            ChooseDragonUIViewManager.Instance.SetUpgradeNeedIcon(ItemParentData.GetItem(costID).Icon);
            costStr = string.Concat(costNum, "/", materialNum);
            ChooseDragonUIViewManager.Instance.SetUpgradeNeedNum(costStr);
        }
        else
        {
            int priceId = DragonBaseData.dataMap[1].upgradeQualityCost;
            int price = PriceListData.GetPrice(priceId, 0);

            costStr = string.Concat(price, "/", MogoWorld.thePlayer.diamond);
            if (price > MogoWorld.thePlayer.diamond)
                costStr = MogoUtils.GetRedString(costStr);
            ChooseDragonUIViewManager.Instance.SetUpgradeNeedIcon(ItemParentData.GetItem(3).Icon);
            ChooseDragonUIViewManager.Instance.SetUpgradeNeedNum(costStr);
        }
    }

    public void OnUpgradeDragon()
    {
        //请求升级龙
        UpgradeDragonReq();
    }

    private void UpgradeDragonReq()
    {
        int[] cost = DragonBaseData.GetCostItem();
        if (InventoryManager.Instance.GetItemNumById(cost[0]) < cost[1])
        {
            //请求购买
            int priceId = DragonBaseData.dataMap[1].upgradeQualityCost;
            int price = PriceListData.GetPrice(priceId, 0);

            // 判断是否需要重置(每天第一次登陆重置为true)
            if (MogoTime.Instance.GetCurrentDateTime().Day != Mogo.Util.SystemConfig.Instance.UpgradeDragonTipDialogDisableDay)
            {
                Mogo.Util.SystemConfig.Instance.IsShowUpgradeDragonTipDialog = true;
                Mogo.Util.SystemConfig.Instance.UpgradeDragonTipDialogDisableDay = MogoTime.Instance.GetCurrentDateTime().Day;
                Mogo.Util.SystemConfig.SaveConfig();
            }

            if (Mogo.Util.SystemConfig.Instance.IsShowUpgradeDragonTipDialog) // 需要显示提示框
            {
                MogoUIManager.Instance.ShowOKCancelTipUI(() =>
                {
                    OKCancelTipUILogicManager.Instance.TruelyConfirm(LanguageData.dataMap[26312].Format(price),
                        (isOK, isNoTip) =>
                        {
                            if (isNoTip)
                            {
                                Mogo.Util.SystemConfig.Instance.IsShowUpgradeDragonTipDialog = false;
                                Mogo.Util.SystemConfig.Instance.UpgradeDragonTipDialogDisableDay = MogoTime.Instance.GetCurrentDateTime().Day;
                                Mogo.Util.SystemConfig.SaveConfig();
                            }

                            if (isOK)
                            {
                                if (price > MogoWorld.thePlayer.diamond)
                                {
                                    ShowLackOfDiamondTip();
                                }
                                else
                                {
                                    //todo请求
                                    MogoWorld.thePlayer.RpcCall("FreshDragonQualityReq");
                                }
                            }
                        }, true);
                }, true);
            }
            else // 不用提示，直接提升
            {
                if (price > MogoWorld.thePlayer.diamond)
                {
                    ShowLackOfDiamondTip();
                }
                else
                {
                    //todo请求
                    MogoWorld.thePlayer.RpcCall("FreshDragonQualityReq");
                }
            }
        }
        else
        {
            MogoWorld.thePlayer.RpcCall("FreshDragonQualityReq");
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorId">
    /// ERR_DRAGON_GOLDED                           = 9,    --已是金色飞龙
    /// ERR_DRAGON_COST_LIMIT                       = 10,   --消耗不足
    /// ERR_DRAGON_FRESH_FAIL                       = 20,   --升级失败
    /// </param>
    /// <param name="quality"></param>
    public void OnUpgradeDragonResp(byte errorId, byte quality)
    {
        LoggerHelper.Debug("OnUpgradeDragonResp:" + errorId);

        if (errorId == 0)
        {
            m_currentData.dragonQuality = quality;
            //刷新选龙界面
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26314));
        }
        else
        {
            switch (errorId)
            {
                case 9:
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26317));
                    break;
                case 10:
                    ShowLackOfDiamondTip();
                    break;
                case 20:
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26313));
                    break;

            }
        }
        SetupChooseDragonView();
    }

    public void OnBuyDragon(int id)
    {
        //请求购买
        int priceId = DragonBaseData.dataMap[1].goldDragonPrice;
        int price = PriceListData.GetPrice(priceId, 0);

        MogoMessageBox.Confirm(LanguageData.dataMap[26311].Format(price),
          (isOk) =>
          {
              if (isOk)
              {
                  //todo请求购买金龙
                  MogoWorld.thePlayer.RpcCall("BuyGoldDragonReq");
              }
          });
    }

    public void OnBuyDragonResp(byte errorId, byte quality)
    {
        LoggerHelper.Debug("OnBuyDragonResp:" + errorId + "," + quality);
        //刷新选龙界面
        if (errorId == 0)
        {
            m_currentData.dragonQuality = quality;
            //刷新选龙界面
            SetupChooseDragonView();
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26314));

        }
        else
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26313));
        }
    }

    public void OnDoStartMatch()
    {
        LoggerHelper.Debug("StartDragonConvoyReq");
        //请求比赛数据
        MogoWorld.thePlayer.RpcCall("StartDragonConvoyReq", m_startMode);

    }
    #endregion

    #region 袭击事件
    public void OnRevenge(int index)
    {
        MogoWorld.thePlayer.RpcCall("DragonRevengeReq", m_RecordData[index].dbid);
    }

    public void OnShowRecord()
    {

        MogoUIManager.Instance.ShowDragonMatchRecordUI(SetupMatchRecord, true);
    }

    private void SetupMatchRecord()
    {
        //Debug.LogError("OnShowRecord");
        LoggerHelper.Debug("SetupMatchRecord");
        MogoWorld.thePlayer.RpcCall("AllDragonEventListReq");
    }

    public void EventListAvatarNameResp(LuaTable luatable)
    {
        //Debug.LogError("EventListAvatarNameResp:" + luatable);
        m_RecordData = GetRecordData(luatable);
        m_RecordData.Sort((DragonMatchEventRecordData a, DragonMatchEventRecordData b) =>
        {
            if (a.timeStamp > b.timeStamp) return -1;
            return 1;
        });
        m_RecordViewData = GetRecordViewData(m_RecordData);
        LoggerHelper.Debug("SetUIGridList");
        DragonMatchRecordUIViewManager.Instance.SetUIGridList(m_RecordViewData.Count, () =>
        {
            LoggerHelper.Debug("SetUIGridList done");
            LoggerHelper.Debug("m_RecordViewData.size:" + m_RecordViewData.Count);
            //for (int i = 0; i < m_RecordViewData.Count; i++)
            //{
            //    LoggerHelper.Debug(m_RecordViewData[i].info);
            //    LoggerHelper.Debug(m_RecordViewData[i].hasRevenged);
            //    LoggerHelper.Debug(m_RecordViewData[i].needTip);
            //    LoggerHelper.Debug(m_RecordViewData[i].canRevenge);
            //}
            DragonMatchRecordUIViewManager.Instance.SetGridListData(m_RecordViewData);
            LoggerHelper.Debug("revengeTime:" + m_currentData.revengeTime);
            DragonMatchRecordUIViewManager.Instance.SetRevengeTimes(m_currentData.revengeTime, DragonBaseData.dataMap.Get(1).revengeTimes);
        });
    }

    private List<DragonMatchEventRecordData> GetRecordData(LuaTable luatable)
    {
        List<DragonMatchEventRecordData> datalist;
        object obj;
        Utils.ParseLuaTable(luatable, typeof(List<DragonMatchEventRecordData>), out obj);
        datalist = obj as List<DragonMatchEventRecordData>;
        return datalist;
    }

    private List<DragonMatchRecordGridData> GetRecordViewData(List<DragonMatchEventRecordData> datalist)
    {
        List<DragonMatchRecordGridData> viewDataList = new List<DragonMatchRecordGridData>();


        for (int i = 0; i < datalist.Count; i++)
        {
            DragonMatchEventRecordData data = datalist[i];
            DragonMatchRecordGridData viewData = new DragonMatchRecordGridData();
            viewData.canRevenge = data.CanRevenge();
            viewData.hasRevenged = data.HasRevenged();
            viewData.info = data.GetInfo();
            viewData.needTip = data.CanRevenge();
            viewDataList.Add(viewData);
        }

        return viewDataList;
    }
    #endregion

    public void OnSelectOtherPlayer(int index)
    {
        LoggerHelper.Debug("OnSelectOtherPlayer:" + index);

        //显示tip
        DragonMatchPlayerInfo playUIInfo = new DragonMatchPlayerInfo();
        DragonPlayerInfo info = m_currentData.playerList[index];
        playUIInfo.dragon = DragonQualityData.GetDragonQualityData(info.quality).GetName(true);
        int exp = 0;
        int gold = 0;
        if (info.reward.ContainsKey(2))
            gold = info.reward[2];
        if (info.reward.ContainsKey(1))
            exp = info.reward[1];
        playUIInfo.hitObtainGold = gold.ToString() + ItemParentData.GetItem(2).Name;
        playUIInfo.hitObtainExp = exp.ToString() + ItemParentData.GetItem(1).Name;

        playUIInfo.hitTimes = string.Concat(info.hittedTime, "/", DragonBaseData.dataMap[1].convoyAttackedTimes);
        playUIInfo.level = "LV" + info.level.ToString();
        playUIInfo.power = info.power.ToString();
        playUIInfo.status = info.GetStatusInfo();
        playUIInfo.tong = info.tongName;
        playUIInfo.name = info.name;
        bool showBtn = (info.dbid != MogoWorld.thePlayer.dbid) && (info.hittedTime < DragonBaseData.dataMap[1].convoyAttackedTimes);
        DragonMatchUIViewManager.Instance.ShowDragonMatchPlayerInfoUIBtnHit(showBtn);
        DragonMatchUIViewManager.Instance.SetDragonMatchPlayerInfo(playUIInfo);
        DragonMatchUIViewManager.Instance.ShowDragonMatchPlayerInfoUI(true);
        m_selectedPlayer = index;
    }

    public void OnAttackOtherPlayer()
    {
        LoggerHelper.Debug("OnAttackOtherPlayer:" + m_selectedPlayer);
        //请求袭击其他玩家
        MogoWorld.thePlayer.RpcCall("DragonAttackReq", m_currentData.playerList[m_selectedPlayer].dbid);
    }

    #region 购买请求

    /// <summary>
    /// ERR_DRAGON_OK                               = 0,    
    ///ERR_DRAGON_ATKBUY_LIMIT                     = 8,    --袭击购买次数达到上限
    ///ERR_DRAGON_COST_LIMIT                       = 10,   --消耗不足
    ///ERR_DRAGON_DEDUCT_WRONG                     = 11,   --扣除失败
    ///ERR_DRAGON_CFG_ERR                          = 12,   --配置错误
    /// </summary>
    /// <param name="errorId"></param>
    public void BuyAtkTimesResp(byte errorId)
    {
        LoggerHelper.Debug("BuyAtktimesResp:" + errorId);
        if (errorId == 0)
        {
            m_currentData.currentHitTime++;
            DragonMatchUIViewManager.Instance.SetHitTimes(String.Concat(m_currentData.currentHitTime, "/", DragonBaseData.dataMap[1].convoyAttackedTimes));
        }
        else
        {
            string msg = string.Empty;
            switch (errorId)
            {
                case 8:
                    msg = LanguageData.GetContent(26330);
                    break;
                case 10:
                    ShowLackOfDiamondTip();
                    break;
                case 11:
                    msg = LanguageData.GetContent(26332);
                    break;
                case 12:
                    msg = LanguageData.GetContent(26333);
                    break;
            }
            MogoMsgBox.Instance.ShowFloatingText(msg);
        }
    }

    /// ERR_DRAGON_ATKCD_END                        = 13,   --袭击cd已结束
    public void ClearAtkCdResp(byte errorId)
    {
        LoggerHelper.Debug("ClearAtkCdResp:" + errorId);
        if (errorId == 0 || errorId == 13)
        {
            m_currentData.hitCD = 0;
            DragonMatchUIViewManager.Instance.ShowHitCD(false, 0);
        }
        else
        {
            MogoMsgBox.Instance.ShowFloatingText(errorId.ToString());
        }
    }

    public void OnBuffClick(int index)
    {
        LoggerHelper.Debug("OnBuffClick:" + index);
        if (index == 0)
        {
            int priceId = DragonBaseData.dataMap[1].cutCompleteTimeFiveMinPrice;
            int price = PriceListData.GetPrice(priceId, 0);

            MogoMessageBox.Confirm(LanguageData.dataMap[26307].Format(price),
         (isOk) =>
         {
             if (isOk)
             {
                 if (price > MogoWorld.thePlayer.diamond)
                 {
                     ShowLackOfDiamondTip();
                 }
                 else
                 {
                     RpcCall("ReduceConvoyTimeReq");
                 }
                 //todo请求缩短5分钟

             }
         });



        }
        else
        {
            int priceId = DragonBaseData.dataMap[1].immediateCompleteConvoyPrice;
            int time = (int)m_currentData.matchTimeLeft / 60;
            if (time == 0) time++;
            int price = PriceListData.GetPrice(priceId, time);
            price *= time;


            MogoMessageBox.Confirm(LanguageData.dataMap[26308].Format(price),
              (isOk) =>
              {
                  if (isOk)
                  {
                      if (price > MogoWorld.thePlayer.diamond)
                      {
                          ShowLackOfDiamondTip();
                      }
                      else
                      {
                          //todo请求立刻完成比赛
                          RpcCall("ImmediateCompleteConvoyReq");
                      }
                  }
              });

        }
    }

    private void ShowLackOfDiamondTip()
    {
        MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26316));
    }

    public void OnRefreshOtherPlayers()
    {
        int priceId = DragonBaseData.dataMap[1].freshAdversaryPrice;
        int price = PriceListData.GetPrice(priceId, 0);

        MogoMessageBox.Confirm(LanguageData.dataMap[26315].Format(price),
          (isOk) =>
          {
              if (isOk)
              {
                  //todo请求刷新其他玩家
                  LoggerHelper.Debug("OnRefreshOtherPlayers");
                  MogoWorld.thePlayer.RpcCall("FreshAdversaryReq");
              }
          });
    }

    public void OnDoRefreshOtherPlayers()
    {
        //请求新的玩家数据
    }

    public void OnHitTimesAddUp()
    {
        int priceId = DragonBaseData.dataMap[1].attackTimesPrice;
        int price = PriceListData.GetPrice(priceId, 0);

        MogoMessageBox.Confirm(LanguageData.dataMap[26310].Format(price),
          (isOk) =>
          {
              if (isOk)
              {
                  LoggerHelper.Debug("BuyAtkTimesReq");
                  //todo请求增加袭击次数
                  RpcCall("BuyAtkTimesReq");

              }
          });
    }

    public void OnHitCDClearUp()
    {
        LoggerHelper.Debug("OnHitCDClearUp");
        int priceId = DragonBaseData.dataMap[1].clearAttackCDPrice;
        //PriceListData priceData = PriceListData.dataMap.Get(priceId);
        int num = DragonMatchUIViewManager.Instance.GetHitCDLastSeconds() / 60;
        if (num == 0) num = 1;
        long price = PriceListData.GetPrice(priceId, 0) * (num);

        MogoMessageBox.Confirm(LanguageData.dataMap[26309].Format(price),
            (isOk) =>
            {
                if (isOk)
                {
                    //todo请求清楚cd
                    RpcCall("ClearAttackCDReq");
                }
            });
    }

    static private void RpcCall(string p, params object[] args)
    {
        MogoWorld.thePlayer.RpcCall(p, args);
    }
    #endregion

    public void OnCDToZero()
    {
        //再次请求数据
        DragonMatchDataReq();
    }

    public void OnGetReward()
    {
        //显示奖励
    }

    public void OnSearch()
    {
        //显示搜索界面
        DragonMatchUIViewManager.Instance.ShowDragonMatchExploreUI(true);
        DragonStationData data = DragonStationData.dataMap.Get(m_currentData.currentRound);
        DragonMatchUIViewManager.Instance.SetlDragonMatchExploreUIIntroduction(data.Descpt);
        DragonMatchUIViewManager.Instance.SetDragonMatchExploreUITitle(data.Name);
        bool showExploreBtn = (m_currentData.hasExplore == 0);
        DragonMatchUIViewManager.Instance.ShowDragonMatchExploreUIBtnExplore(showExploreBtn);

    }

    public void OnDoSearch()
    {
        LoggerHelper.Debug("OnDoSearch");
        //DragonMatchUIViewManager.Instance.ShowDragonMatchExploreUI(false);
        //m_startMode = 1;
        //OnStartMatch();
        RpcCall("ExploreDragonEventReq");
    }

    public void OnBeginTreasureUp()
    {
        LoggerHelper.Debug("OnBeginTreasureUp");
        //打开奖励提示
        RpcCall("FreshConvoyReward");

    }

    public void OnEndTreasureUp()
    {
        LoggerHelper.Debug("OnEndTreasureUp");
        RpcCall("DragonCvyRewardReq");
    }

    /// <summary>
    /// ERR_DRAGON_OK                               = 0,    --开始袭击切换战斗
    /// ERR_DRAGON_UNNEED                           = 7,    --不需要购买
    /// 
    /// ERR_DRAGON_NOEND                            = 15,   --护送还没有结束
    /// ERR_DRAGON_NOGET_REWAED                     = 16,   --奖励没有领取
    /// ERR_DRAGON_CONVOY_TIMES_LIMIT               = 17,   --今日护送次数已用完
    /// ERR_DRAGON_CONVOY_END                       = 18,   --护送已完成
    /// ERR_DRAGON_INFO_LOSE                        = 19,   --角色信息丢失
    /// </summary>
    /// <param name="errorId"></param>
    public void DragonRelatedResp(byte errorId)
    {
        //MogoMsgBox.Instance.ShowFloatingText("DragonRelatedResp:" + errorId);
        LoggerHelper.Debug("DragonRelatedResp:" + errorId);
        switch (errorId)
        {
            case 10: ShowLackOfDiamondTip(); break;

        }

        //throw new NotImplementedException();
    }

    public void FreshConvoyRewardResp(uint exp, uint gold)
    {
        LoggerHelper.Debug("FreshConvoyRewardResp:" + exp + "," + gold);
        DragonMatchUIViewManager.Instance.SetBeginTreasureUIRewardExp((int)exp);
        DragonMatchUIViewManager.Instance.SetBeginTreasureUIRewardGold((int)gold);
    }

    public void DragonCvyRewardResp(byte errorId)
    {
        if (errorId == 0)
        {
            m_currentData.hasReward = 0;
            ShowMainUI();
        }
    }

    #region 袭击场景
    static private void OnInstanceLeave(int sceneID, bool isInstance)
    {
        if (MapData.dataMap.Get(sceneID).type == MapType.ASSAULT)
        {
            MainUIViewManager.Instance.SetHpBottleVisible(true);
            MogoWorld.thePlayer.RemoveFx(6029);
        }
    }

    static private void OnInstanceEnter(int sceneID, bool isInstance)
    {
        if (MapData.dataMap.Get(sceneID).type == MapType.ASSAULT)
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
        else if (!isInstance)
        {
            DragonMatchManager.Instance.CheckDragonStatus();
        }
    }
    #endregion

    #region 提示助手
    private void CheckDragonStatus()
    {
        //Debug.LogError(" RpcCall(\"DragonStatusReq\")");
        RpcCall("DragonStatusReq");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hasReward">是否有奖励可以领取1：可以领取，0：没有奖励</param>
    /// <param name="isConvoying">是否在护送中1：在护送中，0：已完成护送</param>
    /// <param name="leftTime">是否有剩余护送次数1：有剩余护送次数，0：没有剩余护送次数 </param>
    public void DragonStatusResp(byte hasReward, byte isConvoying, byte leftTime)
    {

        DragonBaseData data = DragonBaseData.dataMap.Get(1);
        if (data.levelNeed > MogoWorld.thePlayer.level)
        {
            return;
        }
        //Debug.LogError("DragonStatusResp:" + hasReward + ",isConvoy:" + isConvoying + ",leftTime:" + leftTime);
        if (MogoUIManager.Instance.IsWindowOpen((int)WindowName.Challenge))
        {
            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.DragonMatch, LanguageData.dataMap[26345].Format(leftTime));
            ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.DragonMatch, false);
        }
        EventDispatcher.TriggerEvent<int>(Events.NormalMainUIEvent.HideChallegeIconTip, (int)ChallengeGridID.DragonMatch);

        if (hasReward == 1)
        {
            EventDispatcher.TriggerEvent<int>(Events.NormalMainUIEvent.ShowChallegeIconTip, (int)ChallengeGridID.DragonMatch);
            if (MogoUIManager.Instance.IsWindowOpen((int)WindowName.Challenge))
            {
                ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.DragonMatch, true);
            }
            else
            {
                ShowHasRewardTip();
            }
        }
        else if (isConvoying == 1)
        {

        }

        if (leftTime > 0)
        {
            SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.Open);
            if (isConvoying == 0 && hasReward == 0)
            {
                if (!MogoUIManager.Instance.IsWindowOpen((int)WindowName.Challenge))
                {
                    ShowIsFreeTip();
                }
            }
        }
        else
        {
            SendDataChangeMsg(ChallengeUIGridMessage.ChallengeState.Close);
        }
    }

    private void ShowIsFreeTip()
    {
        TipViewData viewData = new TipViewData();
        viewData.atlasName = "MogoOperatingUI";
        viewData.btnName = LanguageData.GetContent(3040);
        viewData.btnAction = OnShow;
        viewData.icon = "flds128";
        viewData.priority = TipManager.TIP_TYPE_DRAGON_MATCH;
        viewData.tipText = LanguageData.GetContent(3039);
        TipUIManager.Instance.AddTipViewData(viewData);
    }

    private void ShowHasRewardTip()
    {
        TipViewData viewData = new TipViewData();
        viewData.atlasName = "MogoOperatingUI";
        viewData.btnName = LanguageData.GetContent(3038);
        viewData.btnAction = OnShow;
        viewData.icon = "flds128";
        viewData.priority = TipManager.TIP_TYPE_DRAGON_MATCH;
        viewData.tipText = LanguageData.GetContent(3037);
        TipUIManager.Instance.AddTipViewData(viewData);
    }


    #endregion


    /// <summary>
    /// ERR_DRAGON_NOATKED                          = 1,    --袭击对手数据错误
    /// ERR_DRAGON_MAX_ATKED_TIMES                  = 2,    --对手已达最大被袭击次数
    /// ERR_DRAGON_MAX_ATK_TIMES                    = 3,    --已达最大袭击次数
    /// ERR_DRAGON_ATK_CDLIMIT                      = 4,    --袭击CD未结束
    /// ERR_DRAGON_MAX_RVG                          = 5,    --复仇已达最大次数
    /// ERR_DRAGON_NOCONVOY                         = 6,    --对手没有护送飞龙
    /// </summary>
    /// <param name="errorId"></param>
    public void DragonAttackResp(byte errorId)
    {
        if (errorId == 0)
        {
            MogoUIManager.Instance.ShowDragonMatchUI(null, false);
            if (DragonMatchUIViewManager.Instance != null)
                DragonMatchUIViewManager.Instance.ShowDragonMatchPlayerInfoUI(false);
            return;
        }
        if (errorId < 5)
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26319 + errorId));
        }
        else
        {
            switch (errorId)
            {
                case 5:
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26341));
                    break;
                case 6:
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26340));
                    break;
            }
        }
    }

    public void DragonConvoyResp(byte errorId, uint timeLeft)
    {
        LoggerHelper.Debug("DragonConvoyResp:" + errorId + ",timeleft:" + timeLeft);
        if (errorId == 0)
        {
            m_currentData.matchTimeLeft = timeLeft;
            DragonMatchUIViewManager.Instance.ShowBeginInfo(true, (int)m_currentData.matchTimeLeft, DragonMatchDataReq);
        }
    }

    /// <summary>
    /// ERR_EXPORE_CVY_NOTEND                       = 1,    --护送没有结束
    ///ERR_EXPORE_CANNOT_EXPLORE                   = 2,    --今日没有完成护送不能探索
    ///ERR_EXPORE_HAS_EXPLORE                      = 3,    --本站点已经探索过
    ///ERR_EXPORE_EXPLORE_OK                       = 0,    --探索成功  
    /// </summary>
    /// <param name="errorId"></param>
    /// <param name="eventId"></param>
    public void ExploreDragonEventResp(byte errorId, byte eventId)
    {
        LoggerHelper.Debug("ExploreDragonEventResp:" + errorId);
        if (errorId == 0)
        {
            MogoMsgBox.Instance.ShowFloatingText(DragonEventsData.dataMap.Get(eventId).Descpt);
            DragonMatchUIViewManager.Instance.ShowDragonMatchExploreUI(false);
            m_currentData.hasExplore = 1;
        }
        else
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(26350));
        }
    }
}