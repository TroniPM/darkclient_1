using Mogo.Util;
using Mogo.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Mogo.GameData;

public class ChallengeUIGridMessage
{
    public enum ChallengeState : byte
    {
        LimitStarted = 0,
        Open = 1,
        LimitFinished = 2,
        Close = 3,
        Lock = 4
    }

    public ChallengeGridID challengeID;
    public ChallengeState state;

    public ChallengeUIGridMessage()
    {
    }
}


public class ChallengeUILogicManager
{
    private static ChallengeUILogicManager instance;

    public static ChallengeUILogicManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChallengeUILogicManager();
            }

            return ChallengeUILogicManager.instance;

        }
    }  

    public bool OgreMustDieOpen = false;

    public void OnCloseUp()
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnEnterUp(int id)
    {
        switch (id)
        {
            case (int)ChallengeGridID.ClimbTower + 1:
                {
                    if (MogoWorld.thePlayer.level >= SystemRequestLevel.CILMBTOWER)
                    {
                        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
                        MogoUIManager.Instance.OpenWindow((int)WindowName.Tower, () =>
                        {
                            ClimbTowerUILogicManager.Instance.SetTowerGridLayout(
                                () =>
                                {
                                    ClimbTowerUILogicManager.Instance.ResourceLoaded();
                                    EventDispatcher.TriggerEvent(Events.TowerEvent.GetInfo);
                                });
                        });
                    }
                    else
                    {
                        MogoMsgBox.Instance.ShowFloatingText(string.Format(LanguageData.GetContent(47117), SystemRequestLevel.CILMBTOWER));
                        LevelNoEnoughUILogicManager.IsChooseLevelUI = false;
                        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, true); // 试炼之塔等级不足，等级提升引导
                    }                  
                }                
                break;

            case (int)ChallengeGridID.DoorOfBury + 1:
                {
                    if (MogoWorld.thePlayer.level >= SystemRequestLevel.DOOROFBURY)
                    {
                        EventDispatcher.TriggerEvent(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.ENTERDOOROFBURY);
                    }
                    else
                    {
                        MogoMsgBox.Instance.ShowFloatingText(string.Format(LanguageData.GetContent(47117), SystemRequestLevel.DOOROFBURY));
                        LevelNoEnoughUILogicManager.IsChooseLevelUI = false;
                        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, true); // 湮灭之门等级不足，等级提升引导
                    }                    
                }                
                break;

            case (int)ChallengeGridID.Sanctuary + 1:
                {
                    if (MogoWorld.thePlayer.level >= SystemRequestLevel.SANCTUARY)
                    {
                        if (nextTimeSec == 0)
                        {
                            if (canEnterTime == 0)
                            {
                                EventDispatcher.TriggerEvent(Events.SanctuaryEvent.CanBuyExtraTime);
                            }
                            else
                            {
                                OnCloseUp();
                                EventDispatcher.TriggerEvent(Events.SanctuaryEvent.EnterSanctuary);
                            }
                        }
                        else
                        {
                            EventDispatcher.TriggerEvent(Events.SanctuaryEvent.RefreshRank);
                            EventDispatcher.TriggerEvent(Events.SanctuaryEvent.RefreshMyInfo);
                        }
                    }
                    else
                    {
                        MogoMsgBox.Instance.ShowFloatingText(string.Format(LanguageData.GetContent(47117), SystemRequestLevel.SANCTUARY));
                        LevelNoEnoughUILogicManager.IsChooseLevelUI = false;
                        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, true); // 圣域守卫战等级不足，等级提升引导
                    }                   
                }                
                break;

            case (int)ChallengeGridID.DragonMatch + 1:
                {
                    DragonMatchManager.Instance.OnShow();
                }
                break;

            case (int)ChallengeGridID.OgreMustDie + 1:
                {
                    if (MogoWorld.thePlayer.level < SystemRequestLevel.OGREMUSTDIE)
                    {
                        Debug.LogError("MogoWorld.thePlayer.level >= OgreMustDieLevel " + (MogoWorld.thePlayer.level >= SystemRequestLevel.OGREMUSTDIE).ToString() + " OgreMustDieOpen: " + OgreMustDieOpen);
                        MogoMsgBox.Instance.ShowFloatingText(string.Format(LanguageData.GetContent(47117), SystemRequestLevel.OGREMUSTDIE));
                        LevelNoEnoughUILogicManager.IsChooseLevelUI = false;
                        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, true); //兽人必死等级不足， 等级提升引导
                    }
                    else if (!OgreMustDieOpen)
                    {
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(28050));
                        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.ProtectGodressTip);
                        List<string> result = new List<string>();
                        foreach (var data in ActivityRewardData.dataMap)
                        {
                            StringBuilder temp = new StringBuilder();
                            temp.Append(LanguageData.dataMap.Get(26057).Format(data.Value.wave));
                            if (data.Value.items != null)
                            {
                                foreach (var itemData in data.Value.items)
                                {
                                    temp.Append(ItemParentData.GetNameWithNum(itemData.Key, itemData.Value));
                                    temp.Append(" ");
                                }
                            }
                            temp.Append("\n");
                            temp.Append(LanguageData.GetContent(26058));
                            if (data.Value.items != null)
                            {
                                foreach (var itemData in data.Value.items)
                                {
                                    temp.Append(ItemParentData.GetNameWithNum(itemData.Key, itemData.Value));
                                    temp.Append(" ");
                                }
                            }
                            temp.Append("\n");
                            temp.Append("\n");
                            result.Add(temp.ToString());
                        }
                        ProtectGodressTipLogicManager.Instance.SetTipRewardList(result);
                        ProtectGodressTipLogicManager.Instance.SetUIDirty();
                    }
                    else
                    {
                        EventDispatcher.TriggerEvent(Events.CampaignEvent.GetCampaignLeftTimes, 1);
                    }
                }
                break;

            case (int)ChallengeGridID.OccupyTower + 1:
                MogoUIManager.Instance.ShowOccupyTowerUI(() => 
                {
                    EventDispatcher.TriggerEvent(Events.OccupyTowerEvent.SetOccupyTowerUIScorePoint);
                });
                break;

            default:
                break;
        }
    }   


    public void Initialize()
    {
        ChallengeUIViewManager.Instance.CLOSEUP += OnCloseUp;
        ChallengeUIViewManager.Instance.ENTERUP += OnEnterUp;

        EventDispatcher.AddEventListener<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, ReceiveChallengeUIGridMessage);
        EventDispatcher.AddEventListener(Events.ChallengeUIEvent.FlushChallengeUIGridSortedResult, FlushChallengeUIGridSortedResult);
        EventDispatcher.AddEventListener(Events.ChallengeUIEvent.CollectChallengeState, SendToSort);
    }

    public void Release()
    {
        ChallengeUIViewManager.Instance.CLOSEUP -= OnCloseUp;
        ChallengeUIViewManager.Instance.ENTERUP -= OnEnterUp;
        EventDispatcher.RemoveEventListener(Events.ChallengeUIEvent.CollectChallengeState, SendToSort);
        EventDispatcher.RemoveEventListener<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, ReceiveChallengeUIGridMessage);
        EventDispatcher.RemoveEventListener(Events.ChallengeUIEvent.FlushChallengeUIGridSortedResult, FlushChallengeUIGridSortedResult);
    }

    public void InitializeData()
    {
        ChallengeUIViewManager.Instance.SetName((int)ChallengeGridID.ClimbTower, LanguageData.dataMap.Get(20001).content);
        ChallengeUIViewManager.Instance.SetName((int)ChallengeGridID.DoorOfBury, LanguageData.dataMap.Get(24031).content);
        ChallengeUIViewManager.Instance.SetName((int)ChallengeGridID.Sanctuary, LanguageData.dataMap.Get(24030).content);
        ChallengeUIViewManager.Instance.SetName((int)ChallengeGridID.DragonMatch, LanguageData.dataMap.Get(24032).content);
        ChallengeUIViewManager.Instance.SetName((int)ChallengeGridID.OgreMustDie, LanguageData.dataMap.Get(24033).content);
        ChallengeUIViewManager.Instance.SetName((int)ChallengeGridID.OccupyTower, LanguageData.dataMap.Get(24034).content);
    }
    public void SendToSort()
    {
        //ChallengeUIGridMessage data = new ChallengeUIGridMessage();
        //data.
        //EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, data); 

    }
    public uint nextTimeSec { get; set; }
    public byte canEnterTime { get; set; }
    public uint endTime { get; set; }
    public void RefreshUI(int index)
    {
        switch ((ChallengeGridID)index)
        {
            case ChallengeGridID.ClimbTower:
                {
                    if (MogoWorld.thePlayer.level >= SystemRequestLevel.CILMBTOWER)
                    {
                        LoggerHelper.Debug("ClimbTowerUILogicManager:" + ClimbTowerUILogicManager.Instance.Data.FailCount);
                        if (ChallengeUIViewManager.Instance!=null)
                        {
                            ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.ClimbTower, false);
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.ClimbTower, LanguageData.GetContent(26345,ClimbTowerUILogicManager.Instance.Data.FailCount));
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.ClimbTower, SystemUIColorManager.BROWN);
                        }
                        ChallengeUIGridMessage data = new ChallengeUIGridMessage();
                        data.challengeID = ChallengeGridID.ClimbTower;
                        data.state = ChallengeUIGridMessage.ChallengeState.Open;
                        EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, data); 
                    }
                    else
                    {
                        if (ChallengeUIViewManager.Instance != null)
                        {
                            ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.ClimbTower, true);
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.ClimbTower, string.Format("{0}级解锁", SystemRequestLevel.CILMBTOWER));
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.ClimbTower, SystemUIColorManager.RED);
                            ChallengeUIGridMessage data = new ChallengeUIGridMessage();
                            data.challengeID = ChallengeGridID.ClimbTower;
                            data.state = ChallengeUIGridMessage.ChallengeState.Lock;
                            EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, data); 
                        }
                    }                    
                }
                break;
            case ChallengeGridID.DoorOfBury:
                break;
            case ChallengeGridID.Sanctuary:
                {
                    ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.Sanctuary, false);
                    if (MogoWorld.thePlayer.level >= SystemRequestLevel.SANCTUARY) // 已解锁
                    {
                        if (nextTimeSec == 0) // 可挑战：结束倒计时中
                        {
                            ChallengeUIGridMessage data = new ChallengeUIGridMessage();
                            data.challengeID = ChallengeGridID.Sanctuary;
                            data.state = ChallengeUIGridMessage.ChallengeState.LimitStarted;
                            EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, data); 
                            ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.Sanctuary, true);
                            ChallengeUIViewManager.Instance.AddTimer(2, endTime, 
                                (curTime) =>
                                {                                   
                                    var span = new TimeSpan(curTime * TimeSpan.TicksPerSecond);
                                    ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.Sanctuary, SystemUIColorManager.BROWN);
                                    ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.Sanctuary, 
                                        Convert.ToDateTime(span.ToString()).
                                        ToString(string.Format("mm{0}ss{1}{2}",
                                        LanguageData.GetContent(7102),
                                        LanguageData.GetContent(7103),
                                        LanguageData.GetContent(7135))));
                                }, 
                                () =>
                                {
                                    ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.Sanctuary, LanguageData.GetContent(7136)); // 活动已结束
                                    ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.Sanctuary, SystemUIColorManager.BROWN);
                                    ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.Sanctuary, false);
                                    ChallengeUIGridMessage data2 = new ChallengeUIGridMessage();
                                    data2.challengeID = ChallengeGridID.Sanctuary;
                                    data2.state = ChallengeUIGridMessage.ChallengeState.LimitFinished;
                                    EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, data2);
                                    EventDispatcher.TriggerEvent(Events.SanctuaryEvent.QuerySanctuaryInfo);
                                });
                        }
                        else // CD倒计时
                        {
                            var span = new TimeSpan(nextTimeSec*TimeSpan.TicksPerSecond);
                            var midTime = MogoTime.Instance.GetCurrentDateTime() + span;
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.Sanctuary, SystemUIColorManager.RED);
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.Sanctuary, midTime.
                                ToString(string.Format("{0}HH{1}mm{2}{3}",
                                LanguageData.GetContent(7130),
                                LanguageData.GetContent(7101),
                                LanguageData.GetContent(7102),
                                LanguageData.GetContent(7131))));
                            ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.Sanctuary, false);
                            ChallengeUIGridMessage data3 = new ChallengeUIGridMessage();
                            data3.challengeID = ChallengeGridID.Sanctuary;
                            data3.state = ChallengeUIGridMessage.ChallengeState.LimitFinished;
                            EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, data3); 
                        }
                    }
                    else // 未解锁
                    {
                        string SanctuaryNoOpen = LanguageData.GetContent(311);
                        ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.Sanctuary, true);
                        ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.Sanctuary, SanctuaryNoOpen);
                        ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.Sanctuary, SystemUIColorManager.RED);
                        ChallengeUIGridMessage data4 = new ChallengeUIGridMessage();
                        data4.challengeID = ChallengeGridID.Sanctuary;
                        data4.state = ChallengeUIGridMessage.ChallengeState.Lock;
                        EventDispatcher.TriggerEvent<ChallengeUIGridMessage>(Events.ChallengeUIEvent.ReceiveChallengeUIGridMessage, data4); 
                    }                  
                }
                break;
            case ChallengeGridID.DragonMatch:
                {

                }
                break;
            case ChallengeGridID.OgreMustDie:
                {
                    if (MogoWorld.thePlayer.level >= SystemRequestLevel.OGREMUSTDIE)
                    {
                        if (ChallengeUIViewManager.Instance != null)
                        {
                            // 
                            ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.OgreMustDie, false);
                        }
                    }
                    else
                    {
                        if (ChallengeUIViewManager.Instance != null)
                        {
                            ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.OgreMustDie, true);
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OgreMustDie, SystemUIColorManager.RED);
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OgreMustDie, string.Format("{0}级解锁", SystemRequestLevel.OGREMUSTDIE));
                        }
                    } 
                }
                break;

            case ChallengeGridID.OccupyTower:
                {
                    if (MogoWorld.thePlayer.level >= SystemRequestLevel.OCCPUYTOWER)
                    {
                        if (ChallengeUIViewManager.Instance != null)
                        {
                            // 
                            ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.OccupyTower, false);
                        }
                    }
                    else
                    {
                        if (ChallengeUIViewManager.Instance != null)
                        {
                            ChallengeUIViewManager.Instance.SetGray((int)ChallengeGridID.OccupyTower, true);
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.OccupyTower, SystemUIColorManager.RED);
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.OccupyTower, string.Format("{0}级解锁", SystemRequestLevel.OCCPUYTOWER));
                        }
                    }
                }
                break;

            default:
                break;
        }
    }

    #region 排序

    bool syncLock = false;
    bool isDirty = false;
    Dictionary<ChallengeGridID, ChallengeUIGridMessage> challengeUIGridMessageSource = new Dictionary<ChallengeGridID,ChallengeUIGridMessage>();

    public void CollectChallengeUIGridMessage()
    {
        // to do

        EventDispatcher.TriggerEvent(Events.ChallengeUIEvent.CollectChallengeState);
    }

    public void ReceiveChallengeUIGridMessage(ChallengeUIGridMessage data)
    {
        if (MogoUIQueue.Instance.IsLocking)
        {
            return;
        }
        // Debug.LogError("ChallengeUIGridMessage: " + data.challengeID + " " + data.state.ToString());

        if (!challengeUIGridMessageSource.ContainsKey(data.challengeID))
            challengeUIGridMessageSource.Add(data.challengeID, data);
        else
            challengeUIGridMessageSource[data.challengeID] = data;

        if (challengeUIGridMessageSource.Count == ChallengeUIViewManager.GRIDNUM)
        {
            if (!syncLock)
            {
                syncLock = true;
                isDirty = false;
                List<ChallengeGridID> sortedResult = new List<ChallengeGridID>();

                do
                {
                    sortedResult = GetChallengeUIGridSortedResult();
                } while (isDirty);

                for (int i = 0; i < sortedResult.Count; i++)
                    ChallengeUIViewManager.Instance.SetGridPos((int)(sortedResult[i]), i);

                syncLock = false;
            }
            else
            {
                isDirty = true;
            }
        }
    }

    public List<ChallengeGridID> GetChallengeUIGridSortedResult()
    {
        Dictionary<ChallengeUIGridMessage.ChallengeState, List<ChallengeGridID>> challengeUIGridMessage = new Dictionary<ChallengeUIGridMessage.ChallengeState, List<ChallengeGridID>>();
        foreach (var message in challengeUIGridMessageSource)
        {
            var data = message.Value;
            if (!challengeUIGridMessage.ContainsKey(data.state))
                challengeUIGridMessage.Add(data.state, new List<ChallengeGridID>());
            if (!challengeUIGridMessage[data.state].Contains(data.challengeID))
                challengeUIGridMessage[data.state].Add(data.challengeID);
        }

        List<ChallengeGridID> result = new List<ChallengeGridID>();
        if (challengeUIGridMessage == null)
            return result;

        if (challengeUIGridMessage.ContainsKey(ChallengeUIGridMessage.ChallengeState.LimitStarted))
            foreach (var id in challengeUIGridMessage[ChallengeUIGridMessage.ChallengeState.LimitStarted])
                result.Add(id);

        if (challengeUIGridMessage.ContainsKey(ChallengeUIGridMessage.ChallengeState.Open))
            foreach (var id in challengeUIGridMessage[ChallengeUIGridMessage.ChallengeState.Open])
                result.Add(id);

        if (challengeUIGridMessage.ContainsKey(ChallengeUIGridMessage.ChallengeState.LimitFinished))
            foreach (var id in challengeUIGridMessage[ChallengeUIGridMessage.ChallengeState.LimitFinished])
                result.Add(id);

        if (challengeUIGridMessage.ContainsKey(ChallengeUIGridMessage.ChallengeState.Close))
            foreach (var id in challengeUIGridMessage[ChallengeUIGridMessage.ChallengeState.Close])
                result.Add(id);

        if (challengeUIGridMessage.ContainsKey(ChallengeUIGridMessage.ChallengeState.Lock))
            foreach (var id in challengeUIGridMessage[ChallengeUIGridMessage.ChallengeState.Lock])
                result.Add(id);

        return result;
    }

    public void FlushChallengeUIGridSortedResult()
    {
        // CollectChallengeUIGridMessage();
    }

    #endregion
}

