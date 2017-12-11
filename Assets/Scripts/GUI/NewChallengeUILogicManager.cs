using UnityEngine;
using System.Collections;
using Mogo.GameData;
using Mogo.Util;
using System;
using System.Collections.Generic;
using System.Text;

public enum ChallengeGridID
{
    ClimbTower = 0, // 试炼之塔
    DoorOfBury = 2, // 湮灭之门
    Sanctuary = 1, // 圣域守卫战
    DragonMatch = 3, // 飞龙大赛
    OgreMustDie = 4, // 兽人必死
    OccupyTower = 5, // 占塔
}

public class NewChallengeUILogicManager : MFUILogicUnit
{

    static NewChallengeUILogicManager m_instance;
    System.Action m_actSetDirty;

    private NewChallengeUILogicManager()
    {
        RegisterCallback();
    }
    private void RegisterCallback()
    {
        //NewChallengeUIViewManager.Instance.SetGridClickHandle(
        //    () => { if (Verify(ChallengeGridID.ClimbTower)) OnClimbTowerClicked(); },
        //    SystemRequirementData.dataMap.Get((int)ChallengeGridID.ClimbTower).pos);
        //NewChallengeUIViewManager.Instance.SetGridClickHandle(
        //    () => { if (Verify(ChallengeGridID.DoorOfBury)) OnDoorOfBuryClicked(); },
        //    SystemRequirementData.dataMap.Get((int)ChallengeGridID.DoorOfBury).pos);
        //NewChallengeUIViewManager.Instance.SetGridClickHandle(
        //    () => { if (Verify(ChallengeGridID.Sanctuary)) OnSanctuaryClicked(); },
        //    SystemRequirementData.dataMap.Get((int)ChallengeGridID.Sanctuary).pos);
        //NewChallengeUIViewManager.Instance.SetGridClickHandle(
        //    () => { if (Verify(ChallengeGridID.DragonMatch)) OnDragonMatchClicked(); },
        //    SystemRequirementData.dataMap.Get((int)ChallengeGridID.DragonMatch).pos);
        //NewChallengeUIViewManager.Instance.SetGridClickHandle(
        //    () => { if (Verify(ChallengeGridID.OgreMustDie)) OnOgreMustDieClicked(); },
        //    SystemRequirementData.dataMap.Get((int)ChallengeGridID.OgreMustDie).pos);
    }
    private bool Verify(ChallengeGridID id)
    {
        if (MogoWorld.thePlayer.level >= SystemRequirementData.dataMap.Get((int)id).requestLevel)
        {
            return true;
        }
        else
        {
            MogoMsgBox.Instance.ShowFloatingText(string.Format(LanguageData.GetContent(47117), SystemRequirementData.dataMap.Get((int)id).requestLevel));
            LevelNoEnoughUILogicManager.IsChooseLevelUI = false;
            MogoUIManager.Instance.ShowLevelNoEnoughUI(null, true);
            return false;
        }
    }
    public static NewChallengeUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new NewChallengeUILogicManager();
            }

            return NewChallengeUILogicManager.m_instance;
        }
    }

    public void SetUIDirty()
    {
        m_actSetDirty = MFUIUtils.SafeDoAction(NewChallengeUIViewManager.Instance, () =>
        { NewChallengeUIViewManager.Instance.SetUIDirty(); });
    }

    public override void FillBufferedData()
    {
        if (m_actSetDirty != null)
            m_actSetDirty();
    }

    public void AddChallenge(ChallengeGridID id, int leftCount)
    {
        NewChallengeUIViewManager.Instance.SetGridName(LanguageData.dataMap.Get(SystemRequirementData.dataMap.Get((int)id).title).content, SystemRequirementData.dataMap.Get((int)id).pos);
    }
    public void AddActivity(ChallengeGridID id)
    {
        NewChallengeUIViewManager.Instance.SetGridName(LanguageData.dataMap.Get(SystemRequirementData.dataMap.Get((int)id).title).content, SystemRequirementData.dataMap.Get((int)id).pos);
    }

    private void OnClimbTowerClicked()
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
    private void OnDoorOfBuryClicked()
    {
        EventDispatcher.TriggerEvent(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.ENTERDOOROFBURY);
    }
    private void OnSanctuaryClicked()
    {
        if (nextTimeSec == 0)
        {
            if (canEnterTime == 0)
            {
                EventDispatcher.TriggerEvent(Events.SanctuaryEvent.CanBuyExtraTime);
            }
            else
            {
                MogoUIManager.Instance.ShowMogoNormalMainUI();
                EventDispatcher.TriggerEvent(Events.SanctuaryEvent.EnterSanctuary);
            }
        }
        else
        {
            EventDispatcher.TriggerEvent(Events.SanctuaryEvent.RefreshRank);
            EventDispatcher.TriggerEvent(Events.SanctuaryEvent.RefreshMyInfo);
        }
    }
    private void OnDragonMatchClicked()
    {
        DragonMatchManager.Instance.OnShow();
    }
    private void OnOgreMustDieClicked()
    {
        if (!OgreMustDieOpen)
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
                temp.Append(" ");
                temp.Append(LanguageData.GetContent(26058));
                if (data.Value.items != null)
                {
                    foreach (var itemData in data.Value.items)
                    {
                        temp.Append(ItemParentData.GetNameWithNum(itemData.Key, itemData.Value));
                        temp.Append(" ");
                    }
                }
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
    public bool OgreMustDieOpen = false;
    public uint nextTimeSec { get; set; }
    public byte canEnterTime { get; set; }
    public uint endTime { get; set; }
    public void RefreshUI(ChallengeGridID index)
    {
        if (MogoWorld.thePlayer.level < SystemRequirementData.dataMap.Get((int)index).requestLevel)
        {
            NewChallengeUIViewManager.Instance.SetGridEnable(SystemRequirementData.dataMap.Get((int)index).pos, false);
            NewChallengeUIViewManager.Instance.SetGridStatus(SystemRequirementData.dataMap.Get((int)index).pos, string.Format("{0}级解锁", SystemRequirementData.dataMap.Get((int)index).requestLevel));
            //ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.ClimbTower, SystemUIColorManager.RED);
        }
        else
        {
            NewChallengeUIViewManager.Instance.SetGridEnable(SystemRequirementData.dataMap.Get((int)index).pos, true);

            switch (index)
            {
                case ChallengeGridID.ClimbTower:
                    {
                        NewChallengeUIViewManager.Instance.SetGridStatus((int)ChallengeGridID.ClimbTower, LanguageData.GetContent(26345, ClimbTowerUILogicManager.Instance.Data.FailCount));
                        ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.ClimbTower, SystemUIColorManager.BROWN);
                    }
                    break;
                case ChallengeGridID.DoorOfBury:
                    {

                    }
                    break;
                case ChallengeGridID.Sanctuary:
                    {

                        if (nextTimeSec == 0) // 可挑战：结束倒计时中
                        {

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

                                    EventDispatcher.TriggerEvent(Events.SanctuaryEvent.QuerySanctuaryInfo);
                                });
                        }
                        else // CD倒计时
                        {
                            var span = new TimeSpan(nextTimeSec * TimeSpan.TicksPerSecond);
                            var midTime = MogoTime.Instance.GetCurrentDateTime() + span;
                            ChallengeUIViewManager.Instance.SetChallengeTextColor((int)ChallengeGridID.Sanctuary, SystemUIColorManager.RED);
                            ChallengeUIViewManager.Instance.SetChallengeText((int)ChallengeGridID.Sanctuary, midTime.
                                ToString(string.Format("{0}HH{1}mm{2}{3}",
                                LanguageData.GetContent(7130),
                                LanguageData.GetContent(7101),
                                LanguageData.GetContent(7102),
                                LanguageData.GetContent(7131))));
                            ChallengeUIViewManager.Instance.ShowEnterTipFX((int)ChallengeGridID.Sanctuary, false);

                        }
                    }
                    break;
                case ChallengeGridID.DragonMatch:
                    {

                    }
                    break;
                case ChallengeGridID.OgreMustDie:
                    {

                    }
                    break;
                default:
                    break;
            }
        }
    }
}
