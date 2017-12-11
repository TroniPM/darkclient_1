using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using Mogo.RPC;

public class OccupyTowerSystem : IEventManager
{
    #region 常量和变量

    public class OccupyTowerPlayer
    {
        public UInt64 dbid { get; set; }
        public string name { get; set; }
        public int camp { get; set; }
    }

    public class OccupyTowerPlayerReport
    {
        public UInt64 dbid { get; set; }
        public int score { get; set; }
    }

    public class OccupyTowerTitleReport
    {
        public int score { get; set; }
        public float percentage { get; set; }
    }

    public readonly static int OccupyTowerOpenLevel = 35;

    public readonly static int OccupyTowerStartHour = 19;
    public readonly static int OccupyTowerStartMinute = 0;

    public readonly static int OccupyTowerFinishHour = 19;
    public readonly static int OccupyTowerFinishMinute = 30;

    public readonly static int OccupyTowerRoundSecond = 600;

    public EntityMyself theOwner;

    protected bool occupyTower = false;
    public bool OccupyTowerOpen
    {
        get { return occupyTower; }
        set { occupyTower = value; }
    }

    public int occupyTowerLastTime = 0;
    public int scorePointSum = 0;

    public int myselfCamp = 0;
    public int enemyCamp = 0;

    public Dictionary<UInt64, OccupyTowerPlayer> playerMessage = new Dictionary<UInt64, OccupyTowerPlayer>();
    Dictionary<UInt64, int> playerScore = new Dictionary<UInt64, int>();

    #endregion


    #region 初始化

    public OccupyTowerSystem(EntityMyself onwer)
    {
        theOwner = onwer;

        AddListeners();
    }

    public void AddListeners()
    {
        EventDispatcher.AddEventListener(Events.OccupyTowerEvent.GetOccupyTowerStatePoint, GetOccupyTowerStatePoint);

        EventDispatcher.AddEventListener(Events.OccupyTowerEvent.JoinOccupyTower, JoinOccupyTower);
        EventDispatcher.AddEventListener(Events.OccupyTowerEvent.LeaveOccupyTower, LeaveOccupyTower);

        EventDispatcher.AddEventListener(Events.OccupyTowerEvent.ExitOccupyTower, ExitOccupyTower);

        EventDispatcher.AddEventListener(Events.ChallengeUIEvent.CollectChallengeState, SetChallengeState);

        EventDispatcher.AddEventListener(Events.OccupyTowerEvent.SetOccupyTowerUIScorePoint, SetOccupyTowerUIScorePoint);

        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, ResetData);
    }

    public void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener(Events.OccupyTowerEvent.GetOccupyTowerStatePoint, GetOccupyTowerStatePoint);

        EventDispatcher.RemoveEventListener(Events.OccupyTowerEvent.JoinOccupyTower, JoinOccupyTower);
        EventDispatcher.RemoveEventListener(Events.OccupyTowerEvent.LeaveOccupyTower, LeaveOccupyTower);

        EventDispatcher.RemoveEventListener(Events.OccupyTowerEvent.ExitOccupyTower, ExitOccupyTower);

        EventDispatcher.RemoveEventListener(Events.ChallengeUIEvent.CollectChallengeState, SetChallengeState);

        EventDispatcher.RemoveEventListener(Events.OccupyTowerEvent.SetOccupyTowerUIScorePoint, SetOccupyTowerUIScorePoint);

        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, ResetData);
    }

    protected int ConvertToInt32(object value)
    {
        return (int)Mogo.Util.Utils.GetValue(value.ToString(), typeof(int));
    }

    #endregion


    #region RPC及分发

    public void JoinOccupyTowerReq()
    {
        // Debug.LogError("JoinOccupyTowerReq");
        theOwner.RpcCall("DefensePvPReq", (byte)1, (uint)0, (uint)0);
    }

    public void LeaveOccupyTowerReq()
    {
        theOwner.RpcCall("DefensePvPReq", (byte)3, (uint)0, (uint)0);
    }

    public void OccupyTowerStatePointReq()
    {
        theOwner.RpcCall("DefensePvPReq", (byte)4, (uint)0, (uint)0);
    }

    public void DefensePvPApplyResp(ushort arg)
    {
        // Debug.LogError("DefensePvPApplyResp: " + arg);
        int waitingNum = (int)arg;
        OnJoinOccupyTowerSuccess(waitingNum);
    }

    public void DefensePvPOpened()
    {
        // Debug.LogError("DefensePvPOpened");
        theOwner.RpcCall("DefensePvPReq", (byte)2, (uint)0, (uint)0);
    }

    public void DefensePvpEnterResp(uint arg, LuaTable luaTable)
    {
        // Debug.LogError("DefensePvpEnterResp： " + luaTable.ToString());
        EnterOccupyTower(arg, luaTable);
    }

    public void DefensePvpPointRefresh(LuaTable playerScoreluaTable, LuaTable towerBloodAndScoreluaTable)
    {
        FlushPlayerScore(playerScoreluaTable);
        FlushTowerBloodAndScore(towerBloodAndScoreluaTable);
    }

    public void DefensePvpAward(byte arg1, byte arg2)
    {
        bool isWin = true;
        if (arg1 == 0)
            isWin = false;

        int rank = (int)arg2;

        OnOccupyTowerEnd(isWin, rank);
    }

    public void DefensePvPStateResp(int arg1, uint arg2)
    {
        SetOccupyTowerState(arg1);
        SetOccupyTowerScorePoint((int)arg2);
    }

    #endregion


    #region 逻辑

    #region 状态和积分

    protected void GetOccupyTowerStatePoint()
    {
        OccupyTowerStatePointReq();
    }

    protected void SetOccupyTowerState(int state)
    {
        switch (state)
        {
            case -1:
                break;

            case 0:
                OccupyTowerOpen = false;
                break;

            default:
                OccupyTowerOpen = true;
                occupyTowerLastTime = state;
                break;
        }

        SetChallengeUIOccpuyTowerData();
        SetChallengeState();
    }

    protected void SetOccupyTowerScorePoint(int scorePoint)
    {
        scorePointSum = scorePoint;
    }

    protected void SetChallengeUIOccpuyTowerData()
    {
        if (ChallengeUIViewManager.Instance != null)
        {
            if (MogoWorld.thePlayer.level >= SystemRequestLevel.OCCPUYTOWER)
            {
                ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.OccupyTower, false);
                if (OccupyTowerOpen)
                {
                    ChallengeUIViewManager.Instance.AddTimer((int)ChallengeGridID.OccupyTower, (uint)occupyTowerLastTime,
                        (curTime) =>
                        {
                            var span = new TimeSpan(curTime * TimeSpan.TicksPerSecond);
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OccupyTower, SystemUIColorManager.BROWN);
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OccupyTower,
                                string.Concat(((int)(span.TotalMinutes)).ToString("d2"),
                                LanguageData.GetContent(7102),
                                ((int)(span.Seconds)).ToString("d2"),
                                LanguageData.GetContent(7103),
                                LanguageData.GetContent(7135)));
                        },
                        () =>
                        {
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OccupyTower, LanguageData.GetContent(7136)); // 活动已结束
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OccupyTower, SystemUIColorManager.BROWN);
                        });

                    ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OccupyTower, true);
                }
                else
                {
                    Debug.LogError("SetChallengeUIOccpuyTowerData 3");

                    ChallengeUIViewManager.Instance.AddTimer((int)ChallengeGridID.OccupyTower, (uint)0,
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
                                if (timeData.ContainsKey(2))
                                {
                                    time = timeData.Get(2);
                                    string[] nums = time.Split(':');

                                    int hour = ConvertToInt32(nums[0]);
                                    int minute = ConvertToInt32(nums[1]);

                                    ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OccupyTower, SystemUIColorManager.RED);
                                    ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OccupyTower, string.Concat(LanguageData.GetContent(7130), hour.ToString("d2"), LanguageData.GetContent(7101), minute.ToString("d2"), LanguageData.GetContent(7102), LanguageData.GetContent(7131)));
                                    ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OccupyTower, false);
                                }
                                else
                                {
                                    int circleDay = weekDay + 1 > 7 ? weekDay + 1 - 7 : weekDay + 1;
                                    for (; circleDay != weekDay; circleDay = circleDay + 1 > 7 ? circleDay + 1 - 7 : circleDay + 1)
                                    {
                                        var circleTimeData = ActivityTimeData.ActivityTimeFormatDataMap.Get(circleDay);
                                        if (circleTimeData != null)
                                        {
                                            if (circleTimeData.ContainsKey(2))
                                            {
                                                time = circleTimeData.Get(2);
                                                string[] nums = time.Split(':');

                                                int hour = ConvertToInt32(nums[0]);
                                                int minute = ConvertToInt32(nums[1]);

                                                ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OccupyTower, SystemUIColorManager.RED);
                                                ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OccupyTower, string.Concat(LanguageData.GetContent(7130), GetStartWeekDay(circleDay), hour.ToString("d2"), LanguageData.GetContent(7101), minute.ToString("d2"), LanguageData.GetContent(7102), LanguageData.GetContent(7131)));
                                                ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OccupyTower, false);
                                                break;
                                            }
                                        }
                                    }
                                    if (circleDay == weekDay)
                                    {
                                        ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OccupyTower, SystemUIColorManager.RED);
                                        ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OccupyTower, LanguageData.GetContent(7132));
                                        ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OccupyTower, false);
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
                                        if (circleTimeData.ContainsKey(2))
                                        {
                                            time = circleTimeData.Get(2);
                                            string[] nums = time.Split(':');

                                            int hour = ConvertToInt32(nums[0]);
                                            int minute = ConvertToInt32(nums[1]);

                                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OccupyTower, SystemUIColorManager.RED);
                                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OccupyTower, string.Concat(LanguageData.GetContent(7130), GetStartWeekDay(circleDay), hour.ToString("d2"), LanguageData.GetContent(7101), minute.ToString("d2"), LanguageData.GetContent(7102), LanguageData.GetContent(7131)));
                                            ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OccupyTower, false);
                                            break;
                                        }
                                    }
                                }
                                if (circleDay == weekDay)
                                {
                                    ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OccupyTower, SystemUIColorManager.RED);
                                    ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OccupyTower, LanguageData.GetContent(7132));
                                    ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OccupyTower, false);
                                }
                            }
                        });
                }
            }
            else
            {
                Debug.LogError("SetChallengeUIOccpuyTowerData 0");

                ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.OccupyTower, true);
                ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OccupyTower, SystemUIColorManager.RED);
                ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OccupyTower, string.Format(LanguageData.GetContent(28051), SystemRequestLevel.OCCPUYTOWER));
                ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.OccupyTower, false);
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

    protected void SetChallengeState()
    {
        ChallengeUIGridMessage data = new ChallengeUIGridMessage();

        data.challengeID = ChallengeGridID.OccupyTower;
        if (theOwner.level < SystemRequestLevel.OCCPUYTOWER)
        {
            data.state = ChallengeUIGridMessage.ChallengeState.Lock;
        }
        else
        {
            if (OccupyTowerOpen)
                data.state = ChallengeUIGridMessage.ChallengeState.LimitStarted;
            else
                data.state = ChallengeUIGridMessage.ChallengeState.LimitFinished;
        }

        EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, data);
    }

    protected void SetOccupyTowerUIScorePoint()
    {
        OccupyTowerUIViewManager.Instance.SetOccupyTowerUIScoreNum(scorePointSum);
    }

    #endregion

    #region 进入匹配队列、匹配和退出匹配队列

    #region 进入队列

    public void JoinOccupyTower()
    {
        JoinOccupyTowerReq();
        // OnJoinOccupyTowerSuccess(1);
    }

    public void OnJoinOccupyTowerSuccess(int waitingNum)
    {
        OccupyTowerUILogicManager.Instance.JoinSuccess(waitingNum);
    }

    #endregion

    #region 退出队列

    public void LeaveOccupyTower()
    {
        LeaveOccupyTowerReq();
    }

    #endregion

    #endregion

    #region 副本中

    #region 进入副本

    protected void EnterOccupyTower(uint passSecond, LuaTable luaTable)
    {
        playerMessage.Clear();
        List<OccupyTowerPlayer> tempMessage = new List<OccupyTowerPlayer>();
        if (Mogo.Util.Utils.ParseLuaTable(luaTable, out tempMessage))
        {
            foreach (var message in tempMessage)
            {
                playerMessage.Add(message.dbid, message);

                if (message.dbid == theOwner.dbid)
                {
                    myselfCamp = message.camp;
                    enemyCamp = myselfCamp == 1 ? 2 : 1;
                }
            }
        }

        MainUIViewManager.Instance.SetOccupyTowerNoticeOwnBlood(1);
        MainUIViewManager.Instance.SetOccupyTowerNoticeOwnScore(0);
        MainUIViewManager.Instance.SetOccupyTowerNoticeEnemyBlood(1);
        MainUIViewManager.Instance.SetOccupyTowerNoticeEnemyScore(0);

        MainUIViewManager.Instance.BeginCountDown1(true, OccupyTowerRoundSecond - (int)passSecond);
    }

    #endregion

    #region 处理血量和积分

    protected void FlushPlayerScore(LuaTable playerScoreluaTable)
    {
        playerScore.Clear();
        List<OccupyTowerPlayerReport> tempMessage = new List<OccupyTowerPlayerReport>();
        if (Mogo.Util.Utils.ParseLuaTable(playerScoreluaTable, out tempMessage))
        {
            foreach (var item in tempMessage)
                playerScore.Add(item.dbid, item.score);
        }
    }

    protected void FlushTowerBloodAndScore(LuaTable towerBloodAndScoreluaTable)
    {
        Dictionary<int, OccupyTowerTitleReport> tempMessage = new Dictionary<int,OccupyTowerTitleReport>();
        if (Mogo.Util.Utils.ParseLuaTable(towerBloodAndScoreluaTable, out tempMessage))
        {
            MainUIViewManager.Instance.ShowOccupyTowerNotice(true, myselfCamp == 1);
            MainUIViewManager.Instance.SetOccupyTowerNoticeOwnBlood(tempMessage[myselfCamp].percentage);
            MainUIViewManager.Instance.SetOccupyTowerNoticeOwnScore(tempMessage[myselfCamp].score);
            MainUIViewManager.Instance.SetOccupyTowerNoticeEnemyBlood(tempMessage[enemyCamp].percentage);
            MainUIViewManager.Instance.SetOccupyTowerNoticeEnemyScore(tempMessage[enemyCamp].score);
        }
    }

    #endregion

    #region 复活

    #endregion

    #region 结算

    public void OnOccupyTowerEnd(bool isWin, int rank)
    {
        int clientRank = GetClientSelfRank();
        if (clientRank != rank)
        {
            Debug.LogError("GetClientSelfRank: " + clientRank + " Server rank:" + rank);
            return;
        }

        OccupyTowerTeamReward teamReward;
        if (isWin)
            teamReward = OccupyTowerTeamReward.dataMap.First(t => t.Value.level == theOwner.level && t.Value.condition == 1).Value;
        else
            teamReward = OccupyTowerTeamReward.dataMap.First(t => t.Value.level == theOwner.level && t.Value.condition == 2).Value;

        OccupyTowerPersonalReward personalReward = OccupyTowerPersonalReward.dataMap.First(t => t.Value.level == theOwner.level && t.Value.rank == rank).Value;

        Dictionary<int, int> rewards = new Dictionary<int, int>();

        if (teamReward.reward != null)
        {
            foreach (var item in teamReward.reward)
            {
                if (rewards.ContainsKey(item.Key))
                    rewards[item.Key] += item.Value;
                else
                    rewards.Add(item.Key, item.Value);
            }
        }

        if (personalReward.reward != null)
        {
            foreach (var item in personalReward.reward)
            {
                if (rewards.ContainsKey(item.Key))
                    rewards[item.Key] += item.Value;
                else
                    rewards.Add(item.Key, item.Value);
            }
        }

        int goldSum = teamReward.gold + personalReward.gold;
        int expSum = teamReward.exp + personalReward.exp;

        if (rewards.ContainsKey(1))
            rewards[1] += expSum;
        else
            rewards.Add(1, expSum);

        if (rewards.ContainsKey(2))
            rewards[2] += goldSum;
        else
            rewards.Add(2, goldSum);

        SortedDictionary<int, List<ulong>> temp = new SortedDictionary<int, List<ulong>>();
        foreach (var item in playerScore)
        {
            if (!temp.ContainsKey(item.Value))
                temp.Add(item.Value, new List<ulong>());
            temp[item.Value].Add(item.Key);
        }

        List<OccupyTowerPassUIPlayerData> playerData = new List<OccupyTowerPassUIPlayerData>();
        List<OccupyTowerPassUIPlayerData> realPlayerData = new List<OccupyTowerPassUIPlayerData>();
        int tempRank = 0;
        foreach (var item in temp)
        {
            tempRank++;
            foreach(var id in item.Value)
            {
                OccupyTowerPassUIPlayerData tempPlayerData = new OccupyTowerPassUIPlayerData();
                tempPlayerData.playerName = playerMessage[id].name;
                tempPlayerData.playerScore = item.Key;
                tempPlayerData.camp = playerMessage[id].camp;

                // to do
                tempPlayerData.playerAddition = 999;
                playerData.Add(tempPlayerData);
            }
        }

        for (int i = playerData.Count - 1; i >= 0; i--)
            realPlayerData.Add(playerData[i]);

        MogoUIManager.Instance.ShowOccupyTowerPassUI(() =>
        {
            OccupyTowerPassUIViewManager.Instance.SetOccupyTowerResult(isWin);
            OccupyTowerPassUIViewManager.Instance.SetOccupyTowerReward(rewards);
            OccupyTowerPassUIViewManager.Instance.SetPlayerInfoListData(realPlayerData);
            OccupyTowerPassUIViewManager.Instance.PlayTitleAnim();
        });
    }

    protected int GetClientSelfRank()
    {
        int selfScore = playerScore[theOwner.dbid];

        int result = 1;
        foreach (var item in playerScore)
            if (item.Value > selfScore)
                result++;

        return result;
    }

    #endregion

    #region 退出

    protected void ExitOccupyTower()
    {
        myselfCamp = 0;
        enemyCamp = 0;

        playerMessage.Clear();
        playerScore.Clear();

        // EventDispatcher.TriggerEvent(Events.InstanceEvent.ReturnHome);
    }

    #endregion

    #endregion

    #region 重置数据

    protected void ResetData(int missionID, bool isInstance)
    {
        if (MapData.dataMap.Get(missionID).type != MapType.OCCUPY_TOWER)
        {
            MainUIViewManager.Instance.ShowOccupyTowerNotice(false);
        }
    }

    #endregion

    #endregion
}
