using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using Mogo.RPC;


public class CampaignSystem : IEventManager
{
    #region 常量和变量

    public enum CampaignResult : ushort
    {
        win = 0,
        lose = 1,
        campaigning = 2
    }

    public static int OGRE_MUST_DIE_OPEN_LEVEL = 20;

    public static int MATCH_COUNT_DOWN = 30;
    public static int ENTER_COUNT_DOWN = 5;
    public static int REVIVE_COUNT_DOWN = 30;

    public EntityMyself theOwner;

    protected bool ogreMustDieOpen;
    protected bool OgreMustDieOpen
    {
        get { return ogreMustDieOpen; }
        set 
        {
            ogreMustDieOpen = value;
            if (ChallengeUILogicManager.Instance != null)
            {
                // Debug.LogError("value: " + value);
                ChallengeUILogicManager.Instance.OgreMustDieOpen = value;
                ChallengeUILogicManager.Instance.RefreshUI((int)ChallengeGridID.OgreMustDie);

                SetChallengeState();
            }
        }
    }
    protected int ogreMustDieLastTime;

    protected uint countDownTimer;
    protected uint messageTimer;
    protected uint towerDefenceCountDownTimer;
    protected bool delayShowOgreMustDieTip;
    protected bool hasReceiveCountDown;

    #endregion


    #region 初始化

    public CampaignSystem(EntityMyself onwer)
    {
        theOwner = onwer;
        countDownTimer = uint.MaxValue;
        messageTimer = uint.MaxValue;
        towerDefenceCountDownTimer = uint.MaxValue;
        delayShowOgreMustDieTip = false;
        hasReceiveCountDown = false;

        OgreMustDieOpen = false;

        AddListeners();
    }

    public void AddListeners()
    {
        EventDispatcher.AddEventListener(Events.CampaignEvent.JoinCampaign, JoinCampaign);
        EventDispatcher.AddEventListener(Events.CampaignEvent.LeaveCampaign, LeaveCampaign);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, EnterCampaign);
        EventDispatcher.AddEventListener(Events.CampaignEvent.ExitCampaign, ExitCampaign);

        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, CheckDelayShowOgreMustDieTip);

        EventDispatcher.AddEventListener<int>(Events.CampaignEvent.GetCampaignLastTime, GetCampaignLastTime);
        EventDispatcher.AddEventListener<int>(Events.CampaignEvent.GetCampaignLeftTimes, GetCampaignLeftTimes);

        EventDispatcher.AddEventListener<EntityParent>(Events.CampaignEvent.SetPlayerMessage, SetPlayerMessage);
        EventDispatcher.AddEventListener<EntityParent>(Events.CampaignEvent.FlushPlayerBlood, FlushPlayerBlood);
        EventDispatcher.AddEventListener<EntityParent>(Events.CampaignEvent.RemovePlayerMessage, RemovePlayerMessage);

        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, CheckShowRewardInCity);

        EventDispatcher.AddEventListener(Events.ChallengeUIEvent.CollectChallengeState, SetChallengeState);

        EventDispatcher.AddEventListener(Events.CampaignEvent.CrystalAttacked, ShowCrystalAttacked);
    }

    public void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener(Events.CampaignEvent.JoinCampaign, JoinCampaign);
        EventDispatcher.RemoveEventListener(Events.CampaignEvent.LeaveCampaign, LeaveCampaign);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, EnterCampaign);
        EventDispatcher.RemoveEventListener(Events.CampaignEvent.ExitCampaign, ExitCampaign);

        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, CheckDelayShowOgreMustDieTip);

        EventDispatcher.RemoveEventListener<int>(Events.CampaignEvent.GetCampaignLastTime, GetCampaignLastTime);
        EventDispatcher.RemoveEventListener<int>(Events.CampaignEvent.GetCampaignLeftTimes, GetCampaignLeftTimes);

        EventDispatcher.RemoveEventListener<EntityParent>(Events.CampaignEvent.SetPlayerMessage, SetPlayerMessage);
        EventDispatcher.RemoveEventListener<EntityParent>(Events.CampaignEvent.FlushPlayerBlood, FlushPlayerBlood);
        EventDispatcher.RemoveEventListener<EntityParent>(Events.CampaignEvent.RemovePlayerMessage, RemovePlayerMessage);

        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, CheckShowRewardInCity);

        EventDispatcher.RemoveEventListener(Events.CampaignEvent.CrystalAttacked, ShowCrystalAttacked);
    }

    protected int ConvertToInt32(object value)
    {
        return (int)Mogo.Util.Utils.GetValue(value.ToString(), typeof(int));
    }

    #endregion


    #region RPC及分发

    public void RpcCampaignReq(CampaignReq handleCode, ushort arg1 = 1, ushort arg2 = 1, string arg3 = "")
    {
        // Debug.LogError("CampaignReq: " + (ushort)handleCode + " " + arg1 + " " + arg2 + " " + arg3);
        theOwner.RpcCall("CampaignReq", (ushort)handleCode, arg1, arg2, arg3);
    }

    public void RpcCampaignResp(ushort handleCode, ushort errorCode, LuaTable luaTable)
    {
        // Debug.LogError("handleCode: " + handleCode + " errorCode: " + errorCode + " luaTable: " + luaTable.ToString());

        if ((CampaignReq)handleCode == CampaignReq.CAMPAIGN_RESULT)
            HandleResp((CampaignReq)handleCode, luaTable, errorCode);
        else if (errorCode != 0)
            HandleErrorCode((CampaignReq)handleCode, (CampaignErrorCode)errorCode, luaTable);
        else
            HandleResp((CampaignReq)handleCode, luaTable);
    }

    protected void HandleErrorCode(CampaignReq handleCode, CampaignErrorCode errorCode, LuaTable luaTable)
    {
        switch (errorCode)
        {
            case CampaignErrorCode.ERR_ACTIVITY_NOT_EXIST:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28000));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_INVITE_SELF:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28001));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_INVITE_NOT_FRIEND:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28002));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_INVITE_NOT_EXIT:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28003));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_INVITE_NOT_ONLINE:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28004));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_INVITE_AC_NOT_EXIST:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28005));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_INVITE_ALLREADY_INVITE:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28006));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_INVITE_RESP_NOT_EXIST:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28007));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_JOIN_NOT_STARTED:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28008));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_JOIN_NOT_EXIST:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28009));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        TimerHeap.DelTimer(countDownTimer);
                        JoinFailedActivityEnd();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_JOIN_LEVEL_NOT_MACTH:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28010));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        JoinFailedEnterFailed();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_JOIN_LEVEL_TIMES_OUT:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28011));
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        TimerHeap.DelTimer(countDownTimer);
                        JoinFailedActivityEnd();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_TOWER_DEFENCE_MATCH_FAIL:
                // Debug.LogError("ERR_ACTIVITY_TOWER_DEFENCE_MATCH_FAIL");
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_MATCH:
                        TimerHeap.DelTimer(countDownTimer);
                        MatchFailedCanNotFindInstance();
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_JOIN_ALLREADY:
                // Debug.LogError("ERR_ACTIVITY_JOIN_ALLREADY");
                switch (handleCode)
                {
                    case CampaignReq.CAMPAIGN_JOIN:
                        HandleResp((CampaignReq)handleCode, luaTable);
                        break;
                }
                break;

            case CampaignErrorCode.ERR_ACTIVITY_GET_ACTIVITY_LEFT_TIME_NOT_EXIT:
                MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(28012));
                SetChallengeUIOgreMustDieData();
                break;

            case CampaignErrorCode.ERR_ACTIVITY_GET_ACTIVITY_LEFT_TIME_NOT_STARTED:
                // Debug.LogError("ERR_ACTIVITY_JOIN_ALLREADY");
                OgreMustDieOpen = false;
                SetChallengeUIOgreMustDieData();
                break;
        }
    }

    protected void HandleResp(CampaignReq handleCode, LuaTable luaTable, ushort auxiliaryArgument = 0)
    {
        switch (handleCode)
        {
            case CampaignReq.CAMPAIGN_NOTIFY_CLIENT_TO_START:
                int startID = ConvertToInt32(luaTable["1"]);
                CampaignActivityStart(startID);
                break;

            case CampaignReq.CAMPAIGN_NOTIFY_CLIENT_TO_FINISH:
                int finishID = ConvertToInt32(luaTable["1"]);
                CampaignActivityFinish(finishID);
                break;

            case CampaignReq.CAMPAIGN_JOIN:
                int matchCountDownTime = ConvertToInt32(luaTable["1"]);
                JoinSuccess(matchCountDownTime);
                break;

            case CampaignReq.CAMPAIGN_MATCH:
                MatchSuccess();
                break;

            case CampaignReq.CAMPAIGN_LEAVE:
                TimerHeap.DelTimer(countDownTimer);
                if (delayShowOgreMustDieTip)
                {
                    CheckDelayShowOgreMustDieTip(theOwner.sceneId, false);
                }
                else
                {
                    MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
                    NormalMainUIViewManager.Instance.SetUIDirty();
                }
                break;

            case CampaignReq.CAMPAIGN_COUNT_DOWN:
                TimerHeap.DelTimer(countDownTimer);
                BattleStartCountDown();
                break;

            case CampaignReq.CAMPAIGN_MISSION_COUNT_DOWN:
                int passTime = ConvertToInt32(luaTable["1"]);
                TowerDefenceStartCountDown(passTime);
                break;

            case CampaignReq.CAMPAIGN_RESULT:
                TimerHeap.DelTimer(countDownTimer);
                HandleReward(luaTable, auxiliaryArgument);
                break;

            case CampaignReq.CAMPAIGN_GET_LEFT_TIMES:
                int leftTimes  =0;
                if (luaTable.ContainsKey("1"))
                {
                    leftTimes= ConvertToInt32(luaTable["1"]);
                    SetOgreMustDieLeftTimes(leftTimes);
                }
                break;

            case CampaignReq.CAMPAIGN_GET_ACTIVITY_LEFT_TIME:
                int lastTime = ConvertToInt32(luaTable["1"]);
                OgreMustDieOpen = true;      
                ogreMustDieLastTime = lastTime;
                SetOgreMustDieFinishTimeEscape();
                SetChallengeUIOgreMustDieData();
                break;

            case CampaignReq.CAMPAIGN_NOTIFY_WAVE_COUNT:
                TimerHeap.DelTimer(countDownTimer);
                MainUIViewManager.Instance.SetSelfAttackText(string.Empty, true);
                TimerHeap.DelTimer(messageTimer);
                int waveCount = ConvertToInt32(luaTable["1"]);
                ShowMonsterWave(waveCount);
                break;
        }
    }

    #endregion


    #region 活动开启和结束

    public void CampaignActivityStart(int startID)
    {
        if (theOwner.level < OGRE_MUST_DIE_OPEN_LEVEL)
            return;

        switch (startID)
        {
            case 1:
                OgreMustDieOpen = true;

                if (theOwner.sceneId == MogoWorld.globalSetting.homeScene 
                    && MFUIManager.CurrentUI != MFUIManager.MFUIID.EnterWaittingMessageBox)
                {
                    EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
                    MogoUIQueue.Instance.PushOne(()=>{
                        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.OgreMustDieTip, MFUIManager.MFUIID.None, 0, true);
                    },MogoUIManager.Instance.m_NormalMainUI,"OgreMustDieTip");
                    OgreMustDieTipLogicManager.Instance.SetUIDirty();
                }
                else
                {
                    delayShowOgreMustDieTip = true;
                }
                break;
        }
    }

    protected void GetCampaignLastTime(int activityID)
    {
        RpcCampaignReq(CampaignReq.CAMPAIGN_GET_ACTIVITY_LEFT_TIME, (ushort)activityID);
    }

    protected void SetOgreMustDieFinishTimeEscape()
    {
        TimerHeap.AddTimer(1000, 0, () => 
        {
            if (ogreMustDieLastTime > 0)
            {
                ogreMustDieLastTime--;
                SetOgreMustDieFinishTimeEscape();
            }
        });
    }

    protected void SetChallengeUIOgreMustDieData()
    {
        // Debug.LogError("SetChallengeUIOgreMustDieData");

        if (ChallengeUIViewManager.Instance != null)
        {
            if (MogoWorld.thePlayer.level >= SystemRequestLevel.OGREMUSTDIE)
            {
                ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.OgreMustDie, false);

                if (OgreMustDieOpen)
                {
                    //int hours = ogreMustDieLastTime / 3600;
                    //int minutes = (ogreMustDieLastTime % 3600) / 60;
                    //int seconds = ogreMustDieLastTime % 60;
                    //Debug.LogError("hours: " + hours + " minutes: " + minutes + " seconds: " + seconds);

                    //ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.OgreMustDie, false);
                    //ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.BROWN);
                    //ChallengeUIViewManager.Instance.BeginCountdown(string.Empty, string.Empty, LanguageData.GetContent(28050), (int)ChallengeGridID.OgreMustDie, hours, minutes, seconds, 999);

                    ChallengeUIViewManager.Instance.AddTimer((int)ChallengeGridID.OgreMustDie, (uint)ogreMustDieLastTime,
                        (curTime) =>
                        {
                            var span = new TimeSpan(curTime * TimeSpan.TicksPerSecond);
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.BROWN);
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OgreMustDie,
                                string.Concat(((int)(span.TotalMinutes)).ToString("d2"),
                                LanguageData.GetContent(7102),
                                ((int)(span.Seconds)).ToString("d2"),
                                LanguageData.GetContent(7103),
                                LanguageData.GetContent(7135)));
                        },
                        () =>
                        {
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OgreMustDie, LanguageData.GetContent(7136)); // 活动已结束
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.BROWN);
                        });

                    ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OgreMustDie, true);
                }
                else
                {
                    ChallengeUIViewManager.Instance.AddTimer((int)ChallengeGridID.OgreMustDie, (uint)0,
                        (curTime) =>
                        {
                        }, 
                        () =>
                        {
                            int weekDay = (int)(MogoTime.Instance.GetCurrentDateTime().DayOfWeek);
                            if (weekDay == 0)
                                weekDay = 7;

                            var timeData = ActivityTimeData.ActivityTimeFormatDataMap.Get(weekDay);
                            string time = string.Empty;

                            if (timeData != null)
                            {
                                if (timeData.ContainsKey(1))
                                {
                                    time = timeData.Get(1);
                                    string[] nums = time.Split(':');

                                    int hour = ConvertToInt32(nums[0]);
                                    int minute = ConvertToInt32(nums[1]);

                                    ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.RED);
                                    ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OgreMustDie, string.Concat(LanguageData.GetContent(7130), hour.ToString("d2"), LanguageData.GetContent(7101), minute.ToString("d2"), LanguageData.GetContent(7102), LanguageData.GetContent(7131)));
                                    ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OgreMustDie, false);
                                }
                                else 
                                {
                                    int circleDay = weekDay + 1 > 7 ? weekDay + 1 - 7 : weekDay + 1;
                                    for (; circleDay != weekDay; circleDay = circleDay + 1 > 7 ? circleDay + 1 - 7 : circleDay + 1)
                                    {
                                        var circleTimeData = ActivityTimeData.ActivityTimeFormatDataMap.Get(circleDay);
                                        if (circleTimeData != null)
                                        {
                                            if (circleTimeData.ContainsKey(1))
                                            {
                                                time = circleTimeData.Get(1);
                                                string[] nums = time.Split(':');

                                                int hour = ConvertToInt32(nums[0]);
                                                int minute = ConvertToInt32(nums[1]);

                                                ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.RED);
                                                ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OgreMustDie, string.Concat(LanguageData.GetContent(7130), GetStartWeekDay(circleDay), hour.ToString("d2"), LanguageData.GetContent(7101), minute.ToString("d2"), LanguageData.GetContent(7102), LanguageData.GetContent(7131)));
                                                ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OgreMustDie, false);
                                                break;
                                            }
                                        }
                                    }
                                    if (circleDay == weekDay)
                                    {
                                        ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.RED);
                                        ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OgreMustDie, LanguageData.GetContent(7132));
                                        ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OgreMustDie, false);
                                    }
                                }
                            }
                            else
                            {
                                int circleDay = weekDay + 1 > 7 ? weekDay + 1 - 7 : weekDay + 1;
                                for (; circleDay != weekDay; circleDay = circleDay + 1 > 7 ? circleDay + 1 - 7 : circleDay + 1)
                                {
                                    var circleTimeData = ActivityTimeData.ActivityTimeFormatDataMap.Get(circleDay);
                                    if (circleTimeData != null)
                                    {
                                        if (circleTimeData.ContainsKey(1))
                                        {
                                            time = circleTimeData.Get(1);
                                            string[] nums = time.Split(':');

                                            int hour = ConvertToInt32(nums[0]);
                                            int minute = ConvertToInt32(nums[1]);

                                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.RED);
                                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OgreMustDie, string.Concat(LanguageData.GetContent(7130), GetStartWeekDay(circleDay), hour.ToString("d2"), LanguageData.GetContent(7101), minute.ToString("d2"), LanguageData.GetContent(7102), LanguageData.GetContent(7131)));
                                            ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OgreMustDie, false);
                                            break;
                                        }
                                    }
                                }
                                if (circleDay == weekDay)
                                {
                                    ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.RED);
                                    ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OgreMustDie, LanguageData.GetContent(7132));
                                    ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OgreMustDie, false);
                                }
                            }
                        });
                }
            }
            else
            {
                ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.OgreMustDie, true);
                ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.RED);
                ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OgreMustDie, string.Format(LanguageData.GetContent(28051), SystemRequestLevel.OGREMUSTDIE));
                ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OgreMustDie, false);
            } 
        }
    }

    protected string GetStartWeekDay(int day)
    {
        string dayOfWeek = string.Empty;
        switch (day)
        {
            case 1:
                dayOfWeek = LanguageData.GetContent(7105);
                break;
            case 2:
                dayOfWeek = LanguageData.GetContent(7106);
                break;
            case 3:
                dayOfWeek = LanguageData.GetContent(7107);
                break;
            case 4:
                dayOfWeek = LanguageData.GetContent(7108);
                break;
            case 5:
                dayOfWeek = LanguageData.GetContent(7109);
                break;
            case 6:
                dayOfWeek = LanguageData.GetContent(7110);
                break;
            default:
                dayOfWeek = LanguageData.GetContent(7118);
                break;
        }
        return string.Concat(LanguageData.GetContent(7116), dayOfWeek);
    }

    public void CheckDelayShowOgreMustDieTip(int missionID, bool isInstance)
    {
        if (theOwner.sceneId == MogoWorld.globalSetting.homeScene 
            && delayShowOgreMustDieTip)
        {
            delayShowOgreMustDieTip = false;
            if (theOwner.level >= OGRE_MUST_DIE_OPEN_LEVEL && MFUIManager.CurrentUI != MFUIManager.MFUIID.EnterWaittingMessageBox)
            {
                MogoUIQueue.Instance.PushOne(() =>
                {
                    MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.OgreMustDieTip, MFUIManager.MFUIID.None, 0, true);
                }, MogoUIManager.Instance.m_NormalMainUI, "CheckDelayShowOgreMustDieTip");
                OgreMustDieTipLogicManager.Instance.SetUIDirty();
            }
        }
    }

    protected void GetCampaignLeftTimes(int activityID)
    {
        RpcCampaignReq(CampaignReq.CAMPAIGN_GET_LEFT_TIMES, (ushort)activityID);
    }

    protected void SetOgreMustDieLeftTimes(int leftTimes)
    {
        // if (MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_ChallengeUI)
        ChallengeUILogicManager.Instance.OnCloseUp();
        OgreMustDieTipLogicManager.Instance.HideSelf();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EnterWaittingMessageBox, MFUIManager.MFUIID.None, 0, true);
        EnterWaittingMessageBoxLogicManager.Instance.ResetStatus();
        EnterWaittingMessageBoxLogicManager.Instance.SetText(string.Concat(LanguageData.GetContent(28052), leftTimes));
        EnterWaittingMessageBoxLogicManager.Instance.ShowMiddleBtn(true);
        EnterWaittingMessageBoxLogicManager.Instance.ShowCloseBtn(true);
        if (leftTimes > 0)
        {
            EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnText(LanguageData.GetContent(28053));
            EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnAction((i) =>
            {
                EventDispatcher.TriggerEvent(Events.CampaignEvent.JoinCampaign);
            });
        }
        else 
        {
            EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnText(LanguageData.GetContent(28054));
            EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnAction((i) =>
            {
                MogoUIManager.Instance.ShowMogoNormalMainUI();
            });
        }
        EnterWaittingMessageBoxLogicManager.Instance.SetUIDirty();
    }

    public void CampaignActivityFinish(int finishID)
    {
        switch (finishID)
        {
            case 1:
                OgreMustDieOpen = false;

                if (delayShowOgreMustDieTip)
                    delayShowOgreMustDieTip = false;
                break;
        }
    }

    public void SetChallengeState()
    {
        ChallengeUIGridMessage data = new ChallengeUIGridMessage();

        data.challengeID = ChallengeGridID.OgreMustDie;
        if (theOwner.level < SystemRequestLevel.OGREMUSTDIE)
        {
            data.state = ChallengeUIGridMessage.ChallengeState.Lock;
        }
        else
        {
            if (OgreMustDieOpen)
                data.state = ChallengeUIGridMessage.ChallengeState.LimitStarted;
            else
                data.state = ChallengeUIGridMessage.ChallengeState.LimitFinished;
        }

        EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, data);
    }

    #endregion


    #region 进入匹配队列、匹配和退出匹配队列

    public void JoinCampaign()
    {
        // Debug.LogError("JoinCampaign");
        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        MogoUIManager.Instance.ShowMogoNormalMainUI();

        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EnterWaittingMessageBox, MFUIManager.MFUIID.None, 0, true);
        EnterWaittingMessageBoxLogicManager.Instance.ResetStatus();
        EnterWaittingMessageBoxLogicManager.Instance.SetText(LanguageData.GetContent(28055));
        EnterWaittingMessageBoxLogicManager.Instance.ShowWaittingAnim(true);
        EnterWaittingMessageBoxLogicManager.Instance.SetUIDirty();
        RpcCampaignReq(CampaignReq.CAMPAIGN_JOIN);
    }

    protected void JoinSuccess(int matchCountDownTime = -1)
    {
        // Debug.LogError("JoinSuccess");
        // MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        // MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EnterWaittingMessageBox, MFUIManager.MFUIID.None, 0, true);

        if (matchCountDownTime != -1)
            MATCH_COUNT_DOWN = matchCountDownTime;

        EnterWaittingMessageBoxLogicManager.Instance.ResetStatus();
        SetMatchCountDown(MATCH_COUNT_DOWN);
        EnterWaittingMessageBoxLogicManager.Instance.ShowWaittingAnim(true);
        EnterWaittingMessageBoxLogicManager.Instance.ShowMiddleBtn(true);
        EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnText(LanguageData.GetContent(28056));
        EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnAction((i) =>
        {
            EventDispatcher.TriggerEvent(Events.CampaignEvent.LeaveCampaign);
        });
        EnterWaittingMessageBoxLogicManager.Instance.SetUIDirty();
    }

    protected void SetMatchCountDown(int countDownTime)
    {
        countDownTimer = TimerHeap.AddTimer(1000, 0, (second) =>
        {
            if (second == 0)
            {
                // MatchFailedCanNotFindInstance();
            }
            else
            {
                EnterWaittingMessageBoxLogicManager.Instance.SetText(LanguageData.GetContent(28057) + second);
                second--;
                SetMatchCountDown(second);
            }
        }, countDownTime);
    }

    protected void JoinFailedEnterFailed()
    {
        // Debug.LogError("JoinFailedEnterFailed");
        // MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        // MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EnterWaittingMessageBox, MFUIManager.MFUIID.None, 0, true);
        EnterWaittingMessageBoxLogicManager.Instance.ResetStatus();
        EnterWaittingMessageBoxLogicManager.Instance.SetText(LanguageData.GetContent(28058));
        EnterWaittingMessageBoxLogicManager.Instance.ShowMiddleBtn(true);
        EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnText(LanguageData.GetContent(28059));
        EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnAction((i) => 
        { 
            MogoUIManager.Instance.ShowMogoNormalMainUI();
        });
        EnterWaittingMessageBoxLogicManager.Instance.SetUIDirty();
    }

    protected void JoinFailedActivityEnd()
    {
        // Debug.LogError("JoinFailedActivityEnd");
        // MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        // MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EnterWaittingMessageBox, MFUIManager.MFUIID.None, 0, true);
        EnterWaittingMessageBoxLogicManager.Instance.ResetStatus();
        EnterWaittingMessageBoxLogicManager.Instance.SetText(LanguageData.GetContent(28060));
        EnterWaittingMessageBoxLogicManager.Instance.ShowMiddleBtn(true);
        EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnText(LanguageData.GetContent(28059));
        EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnAction((i) => 
        {
            MogoUIManager.Instance.ShowMogoNormalMainUI();
        });
        EnterWaittingMessageBoxLogicManager.Instance.SetUIDirty();
    }

    public void MatchSuccess()
    {
        // Debug.LogError("MatchSuccess");
    }

    protected void MatchFailedCanNotFindInstance()
    {
        // Debug.LogError("MatchFailedCanNotFindInstance");

        if (EnterWaittingMessageBoxLogicManager.Instance.currentCode == 1)
            return;

        // MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        // MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EnterWaittingMessageBox, MFUIManager.MFUIID.None, 0, true);
        EnterWaittingMessageBoxLogicManager.Instance.ResetStatus();
        EnterWaittingMessageBoxLogicManager.Instance.SetText(LanguageData.GetContent(28062));
        EnterWaittingMessageBoxLogicManager.Instance.ShowLeftBtn(true);
        EnterWaittingMessageBoxLogicManager.Instance.SetLeftBtnEnable(true);
        EnterWaittingMessageBoxLogicManager.Instance.SetLeftBtnText(LanguageData.GetContent(28063));
        EnterWaittingMessageBoxLogicManager.Instance.SetLeftBtnAction((i) => 
        {
            EventDispatcher.TriggerEvent(Events.CampaignEvent.LeaveCampaign);
        });
        EnterWaittingMessageBoxLogicManager.Instance.ShowRightRematchBtn(true);
        EnterWaittingMessageBoxLogicManager.Instance.SetRightRematchBtnText(LanguageData.GetContent(28064));
        EnterWaittingMessageBoxLogicManager.Instance.SetRightRematchBtnAction((i) =>
        {
            JoinCampaign();
        });
        EnterWaittingMessageBoxLogicManager.Instance.SetUIDirty(1);
    }

    public void LeaveCampaign()
    {
        // Debug.LogError("LeaveConpaign");
        RpcCampaignReq(CampaignReq.CAMPAIGN_LEAVE);
    }

    #endregion


    #region 副本中

    #region 进入塔防，对话框，倒计时

    public void EnterCampaign(int missionID, bool isInstance)
    {
        if (MapData.dataMap.Get(missionID).type == MapType.TOWERDEFENCE)
        {
            if (hasReceiveCountDown)
                hasReceiveCountDown = false;
            else
            {
                // MainUIViewManager.Instance.SetSelfAttackText(LanguageData.GetContent(28100), true);
                MainUIViewManager.Instance.ShowTDTip(true, LanguageData.GetContent(28100));
            }

            MogoWorld.thePlayer.CurMissionID = 30002;
            MogoWorld.thePlayer.CurMissionLevel = 1;
        }
        else
        {
            hasReceiveCountDown = false;

            MainUILogicManager.Instance.ResetTDBlood();
        } 
    }

    public void BattleStartCountDown()
    {
        //MFUIManager.GetSingleton().ManualHideUI(MFUIManager.MFUIID.EnterWaittingMessageBox);
        hasReceiveCountDown = true;
        MainUIViewManager.Instance.SetSelfAttackText(LanguageData.GetContent(28101) + ENTER_COUNT_DOWN, true);
        SetBattleCountDown(ENTER_COUNT_DOWN);
    }

    protected void SetBattleCountDown(int totalSeconds)
    {
        countDownTimer = TimerHeap.AddTimer(1000, 0, (sec) =>
        {
            if (sec > 0)
            {
                MainUIViewManager.Instance.SetSelfAttackText(sec.ToString(), false);
                sec--;
                SetBattleCountDown(sec);
            }
            else
            {
                MainUIViewManager.Instance.SetSelfAttackText(String.Empty, false);
            }
        }, totalSeconds);
    }

    public void TowerDefenceStartCountDown(int passTime)
    {
        // int passTime = MissionData.dataMap.First(t => t.Value.difficulty == 1 && t.Value.scene == theOwner.sceneId).Value.passTime;
        // ShowTowerDefenceCountDown(passTime);
        int hour = passTime / 3600;
        int minute = (passTime % 3600) / 60;
        int second = passTime % 60;

        MainUIViewManager.Instance.BeginCountDown1(true, MogoCountDownTarget.OgreMustDie, hour, minute, second);
    }

    protected void ShowTowerDefenceCountDown(int totalSeconds)
    {
        towerDefenceCountDownTimer = TimerHeap.AddTimer(1000, 0, (sec) =>
        {
            if (sec > 0)
            {
                MainUIViewManager.Instance.ShowInstanceCountDown(true);
                MainUIViewManager.Instance.SetInstanceCountDown(sec.ToString());
                sec--;
                ShowTowerDefenceCountDown(sec);
            }
            else
            {
                MainUIViewManager.Instance.ShowInstanceCountDown(false);
                MainUIViewManager.Instance.SetInstanceCountDown(string.Empty);
            }
        }, totalSeconds);
    }

    #endregion

    #region 显示波数

    public void ShowMonsterWave(int waveCount)
    {
        //MainUIViewManager.Instance.SetSelfAttackText(LanguageData.dataMap[28109].Format(waveCount), true);
        //TimerHeap.DelTimer(messageTimer);
        //messageTimer = TimerHeap.AddTimer(3000, 0, () =>
        //{
        //    MainUIViewManager.Instance.SetSelfAttackText(String.Empty, false);
        //});

        MainUIViewManager.Instance.SetTDWaveText(LanguageData.dataMap[28109].Format(waveCount));

        MainUIViewManager.Instance.ShowTDTip(true, LanguageData.dataMap[28109].Format(waveCount));
        TimerHeap.DelTimer(messageTimer);
        messageTimer = TimerHeap.AddTimer(3000, 0, () =>
        {
            MainUIViewManager.Instance.ShowTDTip(false, String.Empty);
        });
    }

    public void ShowCrystalAttacked()
    {
        //MainUIViewManager.Instance.SetSelfAttackText(LanguageData.GetContent(28112), true);
        //TimerHeap.DelTimer(messageTimer);
        //messageTimer = TimerHeap.AddTimer(3000, 0, () =>
        //{
        //    MainUIViewManager.Instance.SetSelfAttackText(String.Empty, false);
        //});

        MainUIViewManager.Instance.ShowTDTip(true, LanguageData.GetContent(28112));
        TimerHeap.DelTimer(messageTimer);
        messageTimer = TimerHeap.AddTimer(3000, 0, () =>
        {
            MainUIViewManager.Instance.ShowTDTip(false, String.Empty);
        });
    }

    #endregion

    #region 其他玩家

    Dictionary<uint, int> otherPlayer = new Dictionary<uint, int>();

    public void EmptyMapRelation()
    {
        otherPlayer.Clear();
    }

    public void SetPlayerMessage(EntityParent entity)
    {
        if (MapData.dataMap.Get(MogoWorld.thePlayer.sceneId).type != MapType.TOWERDEFENCE
            || !(entity is EntityPlayer)
            || entity.ID == theOwner.ID)
            return;

        if (otherPlayer.ContainsKey(entity.ID))
        {
            switch (otherPlayer[entity.ID])
            {
                case 1:
                    MainUILogicManager.Instance.SetPlayerMessageInit(entity, 1);
                    break;
                case 2:
                    MainUILogicManager.Instance.SetPlayerMessageInit(entity, 2);
                    break;
                case 3:
                    MainUILogicManager.Instance.SetPlayerMessageInit(entity, 3);
                    break;
            }
        }
        else
        {
            if (!otherPlayer.ContainsValue(1))
            {
                MainUILogicManager.Instance.SetPlayerMessageInit(entity, 1);
                otherPlayer.Add(entity.ID, 1);
            }
            else if (!otherPlayer.ContainsValue(2))
            {
                MainUILogicManager.Instance.SetPlayerMessageInit(entity, 2);
                otherPlayer.Add(entity.ID, 2);
            }
            else if (!otherPlayer.ContainsValue(3))
            {
                MainUILogicManager.Instance.SetPlayerMessageInit(entity, 3);
                otherPlayer.Add(entity.ID, 3);
            }
        }
    }

    public void FlushPlayerBlood(EntityParent entity)
    {
        if (!otherPlayer.ContainsKey(entity.ID))
            return;

        MainUILogicManager.Instance.FlushPlayerBlood((int)entity.PercentageHp, otherPlayer[entity.ID]);
    }

    public void RemovePlayerMessage(EntityParent entity)
    {
        if (MapData.dataMap.Get(theOwner.sceneId).type != MapType.TOWERDEFENCE)
            return;

        if (!otherPlayer.ContainsKey(entity.ID))
            return;

        MainUILogicManager.Instance.RemovePlayerMessage(otherPlayer[entity.ID]);
        otherPlayer.Remove(entity.ID);
    }

    #endregion

    #region 复活

    public void ShowTowerDefenceRevive(int hasReviveTimes)
    {
        // Debug.LogError("ShowTowerDefenceRevive");

        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EnterWaittingMessageBox, MFUIManager.MFUIID.None, 0, true);
        EnterWaittingMessageBoxLogicManager.Instance.ResetStatus();
        EnterWaittingMessageBoxLogicManager.Instance.SetText(LanguageData.GetContent(28102));

        var missionData = MissionData.dataMap.First(t => t.Value.difficulty == 1 && t.Value.scene == theOwner.sceneId).Value;

        if (missionData.reviveTimes > hasReviveTimes)
        {
            EnterWaittingMessageBoxLogicManager.Instance.ShowLeftBtn(true);
            EnterWaittingMessageBoxLogicManager.Instance.SetLeftBtnText(LanguageData.GetContent(28103) + " " + REVIVE_COUNT_DOWN.ToString());
            EnterWaittingMessageBoxLogicManager.Instance.SetLeftBtnEnable(false);
            SetReviveCountDown(REVIVE_COUNT_DOWN - 1);
            EnterWaittingMessageBoxLogicManager.Instance.SetLeftBtnAction((i) =>
            {
                ExitCampaign();
            });

            int sum = 0;
            for (int i = 1; i <= hasReviveTimes + 1; i++)
                sum += PriceListData.dataMap[20].priceList[i];

            EnterWaittingMessageBoxLogicManager.Instance.ShowRightReviveBtn(true);
            EnterWaittingMessageBoxLogicManager.Instance.SetRightReviveBtnText(string.Concat("x", sum));
            EnterWaittingMessageBoxLogicManager.Instance.SetRightReviveBtnAction((i) =>
            {
                Revive();
            });
        }
        else
        {
            EnterWaittingMessageBoxLogicManager.Instance.ShowMiddleBtn(true);
            EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnText(LanguageData.GetContent(28105));
            EnterWaittingMessageBoxLogicManager.Instance.SetMiddleBtnAction((i) =>
            {
                ExitCampaign();
            });
        }

        EnterWaittingMessageBoxLogicManager.Instance.SetUIDirty();
    }


    protected void SetReviveCountDown(int totalSeconds)
    {
        countDownTimer = TimerHeap.AddTimer(1000, 0, (sec) =>
        {
            if (sec > 0)
            {
                EnterWaittingMessageBoxLogicManager.Instance.SetLeftBtnText(LanguageData.GetContent(28103) + " " + sec.ToString());
                sec--;
                SetReviveCountDown(sec);
            }
            else
            {
                EnterWaittingMessageBoxLogicManager.Instance.SetLeftBtnText(LanguageData.GetContent(28103));
                EnterWaittingMessageBoxLogicManager.Instance.SetLeftBtnEnable(true);
            }
        }, totalSeconds);
    }

    protected void Revive()
    {
        theOwner.RpcCall("MissionReq", (byte)MissionReq.REVIVE, (ushort)1, (ushort)1, string.Empty);
    }

    public void ReviveSuccess()
    {
        TimerHeap.DelTimer(countDownTimer);
        MogoUIManager.Instance.ShowMogoBattleMainUI();
    }
     
    #endregion

    #region 结算

    bool showRewardInCity;
    int wave = 0;
    int output = 0;
    int exp = 0;
    int gold = 0;
    Dictionary<int, int> rewardItems = new Dictionary<int, int>();

    public void HandleReward(LuaTable luaTable, ushort auxiliaryArgument)
    {
        // Debug.LogError("luatable" + luaTable + " \nauxiliaryArgument: " + auxiliaryArgument);

        wave = ConvertToInt32(luaTable["1"]);
        output = ConvertToInt32(luaTable["2"]);
        exp = ConvertToInt32(luaTable["3"]);
        gold = ConvertToInt32(luaTable["4"]);
        rewardItems = new Dictionary<int,int>();
        if (luaTable.ContainsKey("5"))
            Mogo.Util.Utils.ParseLuaTable<Dictionary<int, int>>((LuaTable)luaTable["5"], out rewardItems);

        Dictionary<int, int> result = new Dictionary<int,int>();
        foreach (var item in rewardItems)
            result.Add(item.Key, item.Value);

        if (theOwner.sceneId == MogoWorld.globalSetting.homeScene)
        {
            if ((CampaignResult)auxiliaryArgument == CampaignResult.campaigning)
            {
                ControlStick.instance.Reset();
                MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattlePassUINoCard);
                BattlePassUINoCardUILogicManager.Instance.SetIsMVP(false);
                BattlePassUINoCardUILogicManager.Instance.SetTitile(string.Empty);
                BattlePassUINoCardUILogicManager.Instance.SetDefenceNum(wave.ToString());
                BattlePassUINoCardUILogicManager.Instance.SetOutPut(output.ToString());
                BattlePassUINoCardUILogicManager.Instance.SetMVPOutput(string.Empty);
                BattlePassUINoCardUILogicManager.Instance.SetMVPName(string.Empty);
                BattlePassUINoCardUILogicManager.Instance.SetRewardItemList(result);
                BattlePassUINoCardUILogicManager.Instance.PlayAnim(false);
                BattlePassUINoCardUILogicManager.Instance.SetUIDirty();
            }
            else
            {
                Debug.LogError("Are you kidding me? : " + auxiliaryArgument);
            }
        }
        else
        {
            MogoUIManager.Instance.ShowMogoBattleMainUI();

            int mvpOutput = ConvertToInt32(luaTable["6"]);
            string mvpName = Convert.ToString(luaTable["7"]);

            //Dictionary<int, int> mvpRewardItems = new Dictionary<int, int>();
            //if (luaTable.ContainsKey("8"))
            //    Mogo.Util.Utils.ParseLuaTable<Dictionary<int, int>>((LuaTable)luaTable["8"], out mvpRewardItems);

            //foreach (var item in mvpRewardItems)
            //{
            //    if (result.ContainsKey(item.Key))
            //        result[item.Key] += item.Value;
            //    else
            //        result.Add(item.Key, item.Value);
            //}

            if ((CampaignResult)auxiliaryArgument == CampaignResult.win)
            {
                ControlStick.instance.Reset();
                MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattlePassUINoCard);
                BattlePassUINoCardUILogicManager.Instance.SetIsMVP(true);
                BattlePassUINoCardUILogicManager.Instance.SetTitile(LanguageData.GetContent(28107));
                BattlePassUINoCardUILogicManager.Instance.SetDefenceNum(wave.ToString());
                BattlePassUINoCardUILogicManager.Instance.SetOutPut(output.ToString());
                BattlePassUINoCardUILogicManager.Instance.SetMVPOutput(mvpOutput.ToString());
                BattlePassUINoCardUILogicManager.Instance.SetMVPName(mvpName);
                BattlePassUINoCardUILogicManager.Instance.SetRewardItemList(result);
                BattlePassUINoCardUILogicManager.Instance.PlayAnim(true);
                BattlePassUINoCardUILogicManager.Instance.SetUIDirty();
            }

            else if ((CampaignResult)auxiliaryArgument == CampaignResult.lose)
            {
                ControlStick.instance.Reset();
                MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattlePassUINoCard);
                BattlePassUINoCardUILogicManager.Instance.SetIsMVP(true);
                BattlePassUINoCardUILogicManager.Instance.SetTitile(LanguageData.GetContent(28108));
                BattlePassUINoCardUILogicManager.Instance.SetDefenceNum(wave.ToString());
                BattlePassUINoCardUILogicManager.Instance.SetOutPut(output.ToString());
                BattlePassUINoCardUILogicManager.Instance.SetMVPOutput(mvpOutput.ToString());
                BattlePassUINoCardUILogicManager.Instance.SetMVPName(mvpName);
                BattlePassUINoCardUILogicManager.Instance.SetRewardItemList(result);
                BattlePassUINoCardUILogicManager.Instance.PlayAnim(true);
                BattlePassUINoCardUILogicManager.Instance.SetUIDirty();
            }
            else if ((CampaignResult)auxiliaryArgument == CampaignResult.campaigning)
            {
                showRewardInCity = true;
            }
        }
    }

    protected void CheckShowRewardInCity(int sceneID, bool isInstance)
    {
        // Debug.LogError("CheckShowRewardInCity");

        if (sceneID != MogoWorld.globalSetting.homeScene
            || !showRewardInCity)
            return;

        showRewardInCity = false;

        List<int> result = new List<int>();
        foreach (var item in rewardItems)
            result.Add(item.Key);

        // Debug.LogError("CheckShowRewardInCity Success");

        MogoUIQueue.Instance.PushOne(() =>
        {
            MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattlePassUINoCard);
            BattlePassUINoCardUILogicManager.Instance.SetTitile(string.Empty);
            BattlePassUINoCardUILogicManager.Instance.SetDefenceNum(wave.ToString());
            BattlePassUINoCardUILogicManager.Instance.SetOutPut(output.ToString());
            BattlePassUINoCardUILogicManager.Instance.SetRewardItemList(result);
            BattlePassUINoCardUILogicManager.Instance.PlayAnim(false);
            BattlePassUINoCardUILogicManager.Instance.SetUIDirty();
        }, MogoUIManager.Instance.m_NormalMainUI, "CheckShowRewardInCity");
    }

    #endregion

    #region 退出

    public void ExitCampaign()
    {
        MainUIViewManager.Instance.BeginCountDown1(false);

        TimerHeap.DelTimer(countDownTimer);
        TimerHeap.DelTimer(messageTimer);
        TimerHeap.DelTimer(towerDefenceCountDownTimer);
        MainUIViewManager.Instance.SetSelfAttackText(String.Empty, false);
        EmptyMapRelation();
        EventDispatcher.TriggerEvent(Events.InstanceEvent.ReturnHome);
    }

    #endregion

    #endregion
}