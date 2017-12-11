using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.Game;
using System.Collections.Generic;

public class NormalMainUILogicManager : UILogicManager
{
    private static NormalMainUILogicManager m_instance;

    public static NormalMainUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new NormalMainUILogicManager();
            }

            return NormalMainUILogicManager.m_instance;

        }
    }

    void OnActivityPlayIconUp()
    {
        LoggerHelper.Debug("Activity");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }
    
    //void RefreshAssistantUI()
    //{
    //    string skillId = "1";

    //    var ltSS = MogoWorld.thePlayer.GetObjectAttr("SpiritSkill") as LuaTable;
    //    var ltSSS = MogoWorld.thePlayer.GetObjectAttr("SelectedSpiritSkill") as LuaTable;

    //    var ltSM = MogoWorld.thePlayer.GetObjectAttr("SpiritMark") as LuaTable;
    //    var ltSSM = MogoWorld.thePlayer.GetObjectAttr("SelectedSpiritMark") as LuaTable;

    //    //var ltSS = MogoWorld.theAccount.GetObjectAttr("SpiritSkill") as LuaTable;
    //    //var ltSSS = MogoWorld.theAccount.GetObjectAttr("SelectedSpiritSkill") as LuaTable;

    //    //LoggerHelper.Debug(ltSS + " " + ltSSS);
    //    LoggerHelper.Debug(ltSS.Count);

    //    if (ltSS != null && ltSSS != null && ltSM != null && ltSSM != null)
    //    {
    //        int lockAttr = System.Convert.ToInt32(skillId);
    //        LoggerHelper.Debug("Here");

    //        //if (lockAttr == 0)
    //        //{
    //        //    AssistantUIViewManager.Instance.SetSkillsContractGridEnable(gridId, false);
    //        //}
    //        //else
    //        //{
    //        //    AssistantUIViewManager.Instance.SetSkillsContractGridEnable(gridId, true);
    //        //}

    //        for (int i = 1; i < 4; ++i)
    //        {
    //            switch (i)
    //            {
    //                case 1:
    //                    if (!ltSSS.ContainsKey(i.ToString()))
    //                    {
    //                        LoggerHelper.Debug("Initative Not Open");
    //                    }
    //                    else if ((string)ltSSS[i.ToString()] == "0")
    //                    {
    //                        LoggerHelper.Debug("Initative No Skill");

    //                    }
    //                    else
    //                    {
    //                        LoggerHelper.Debug("Initative Have Skill");

    //                    }
    //                    break;

    //                case 2:
    //                    if (!ltSSS.ContainsKey(i.ToString()))
    //                    {
    //                        LoggerHelper.Debug("Passive0 Not Open");
    //                        AssistantUIViewManager.Instance.ShowSkillsContractPassive0Lock(true);
    //                        AssistantUIViewManager.Instance.ShowSkillsContractPassive0Icon(false);
    //                    }
    //                    else if ((string)ltSSS[i.ToString()] == "0")
    //                    {
    //                        LoggerHelper.Debug("Passive0 No Skill");
    //                        AssistantUIViewManager.Instance.ShowSkillsContractPassive0Lock(false);
    //                        AssistantUIViewManager.Instance.ShowSkillsContractPassive0Icon(false);

    //                    }
    //                    else
    //                    {
    //                        LoggerHelper.Debug("Passive0 Have Skill");
    //                        AssistantUIViewManager.Instance.ShowSkillsContractPassive0Lock(false);
    //                        AssistantUIViewManager.Instance.ShowSkillsContractPassive0Icon(true);

    //                    }
    //                    break;

    //                case 3:
    //                    if (!ltSSS.ContainsKey(i.ToString()))
    //                    {
    //                        LoggerHelper.Debug("Passive1 Not Open");
    //                        AssistantUIViewManager.Instance.ShowSkillsContractPassive1Lock(true);
    //                        AssistantUIViewManager.Instance.ShowSkillsCOntractPassive1Icon(false);
    //                    }
    //                    else if ((string)ltSSS[i.ToString()] == "0")
    //                    {
    //                        LoggerHelper.Debug("Passive1 No Skill");
    //                        AssistantUIViewManager.Instance.ShowSkillsContractPassive1Lock(false);
    //                        AssistantUIViewManager.Instance.ShowSkillsCOntractPassive1Icon(false);
    //                    }
    //                    else
    //                    {
    //                        LoggerHelper.Debug("Passive1 Have Skill");
    //                        AssistantUIViewManager.Instance.ShowSkillsContractPassive1Lock(false);
    //                        AssistantUIViewManager.Instance.ShowSkillsCOntractPassive1Icon(true);
    //                    }
    //                    break;

    //            }
    //        }

    //        lockAttr = System.Convert.ToInt32(skillId);
    //        LoggerHelper.Debug("Here2");

    //        //if (lockAttr == 0)
    //        //{
    //        //    AssistantUIViewManager.Instance.SetSkillsContractGridEnable(gridId, false);
    //        //}
    //        //else
    //        //{
    //        //    AssistantUIViewManager.Instance.SetSkillsContractGridEnable(gridId, true);
    //        //}

    //        for (int i = 1; i < 6; ++i)
    //        {
    //            if (!ltSSS.ContainsKey(i.ToString()))
    //            {
    //                LoggerHelper.Debug("Not Open" + " " + i);
    //                //AssistantUIViewManager.Instance.ShowElementMintmarkGridLock(true);
    //                AssistantUIViewManager.Instance.ShowMintmarkBodyGridLock(i - 1, true);
    //                AssistantUIViewManager.Instance.ShowMintmarkBodyGridIcon(i - 1, false);
    //            }
    //            else if ((string)ltSSS[i.ToString()] == "0")
    //            {
    //                LoggerHelper.Debug("No Mintmark " + i);
    //                AssistantUIViewManager.Instance.ShowMintmarkBodyGridLock(i - 1, false);
    //                AssistantUIViewManager.Instance.ShowMintmarkBodyGridIcon(i - 1, false);
    //            }
    //            else
    //            {
    //                LoggerHelper.Debug("Have Mintmark " + i);
    //                AssistantUIViewManager.Instance.ShowMintmarkBodyGridLock(i - 1, false);
    //                AssistantUIViewManager.Instance.ShowMintmarkBodyGridIcon(i - 1, true);
    //            }

    //        }


    //        for (int i = 1; i < 7; ++i)
    //        {
    //            switch ((string)ltSS[i.ToString()])
    //            {
    //                case "0":
    //                    AssistantUIViewManager.Instance.SetSkillsContractGridEnable(i - 1, false);
    //                    break;

    //                case "1":
    //                    AssistantUIViewManager.Instance.SetSkillsContractGridEnable(i - 1, true);
    //                    break;
    //            }
    //        }

    //        int index = 1;
    //        foreach (var item in ltSM)
    //        {
    //            LoggerHelper.Debug(ltSM.Count);
    //            switch ((string)item.Value)
    //            {
    //                case "0":
    //                    AssistantUIViewManager.Instance.ShowElementMintmarkGridLock(index - 1, false);
    //                    break;

    //                case "1":
    //                    AssistantUIViewManager.Instance.ShowElementMintmarkGridLock(index - 1, true);
    //                    break;
    //            }

    //            ++index;

    //        }
    //        MogoUIManager.Instance.ShowMogoAssistantUI();

    //    }
    //    else
    //    {
    //        LoggerHelper.Debug("No Server Data!!!!!!!!!!!!!!!!!!!!");
    //    }
    //}

    void OnAssistantPlayIconUp()
    {
        LoggerHelper.Debug("Assistant");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
        MogoUIManager.Instance.ShowMogoAssistantUI();
    }

    void OnChargeRewardIconUp()
    {
        LoggerHelper.Debug("OnChargeRewardIconUp");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
        //MogoUIManager.Instance.ShowMogoOperatingUI();
        EventDispatcher.TriggerEvent(Events.RewardEvent.OpenReward);
    }

    void OnAutoTaskPlayIconUp()
    {
        LoggerHelper.Debug("AutoTask " + MogoWorld.thePlayer.CurrentTask);

        NormalMainUIViewManager.Instance.ShowAssistantDialog(false);

        if (MogoWorld.thePlayer.CurrentTask != null)
        {
            switch (MogoWorld.thePlayer.CurrentTask.conditionType)
            {
                case 0:
                    LoggerHelper.Debug("AutoTask " + MogoWorld.thePlayer.CurrentTask.pathPoint);
                    EventDispatcher.TriggerEvent(EntityMyself.ON_TASK_GUIDE);
                    break;
                case 1:
                    if (MogoWorld.thePlayer.CurrentTask.condition.Count < 2)
                    {
                        LoggerHelper.Debug("Task Condition List Num.");
                        return;
                    }
                    int missionID = MogoWorld.thePlayer.CurrentTask.condition[0];
                    int level = MogoWorld.thePlayer.CurrentTask.condition[1];
                    EventDispatcher.TriggerEvent(EntityMyself.ON_TASK_MISSION, missionID, level);
                    break;
            }
        }
        else
        {
            // MogoMsgBox.Instance.ShowFloatingText("All Task Complete!");
            OnInstancePlayIconUp();
            InstanceUILogicManager.Instance.SetMapID(SystemConfig.Instance.LastMap > InstanceMissionChooseUIViewManager.Instance.MapOpenPage - 1 ? 0 : SystemConfig.Instance.LastMap);
        }
    }

    public void OnPVEPlayIconUp()
    {
        Mogo.Util.LoggerHelper.Debug("PVE");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        MogoUIManager.Instance.OpenWindow((int)WindowName.Challenge, 
            ()=> 
            {
                ChallengeUIViewManager.Instance.SetGridLayout(
                    ()=>
                    {
                        if (MogoUIManager.Instance.WaitingWidgetName == "ChallengeGrid0")
                        {
                            TimerHeap.AddTimer(100, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
                        }
                        EventDispatcher.TriggerEvent(Events.TowerEvent.GetInfo);
                        EventDispatcher.TriggerEvent(Events.SanctuaryEvent.QuerySanctuaryInfo);
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                    }
                    );
            }
            );
        
    }

    public void OnPVPPlayIconUp()
    {
        //竞技场打开 修改打开方式为       点击-开loading-发数据包-收数据包-显示UI-关loading
        LoggerHelper.Debug("PVP");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);

        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        EventDispatcher.TriggerEvent(Events.ArenaEvent.EnterArena);
    }

    void OnEquipmentComsumeIconUp()
    {
        LoggerHelper.Debug("Equipment");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
        InventoryManager.Instance.CurrentView = InventoryManager.View.BodyEnhanceView;
        InventoryManager.Instance.m_currentEquipmentView = InventoryManager.View.BodyEnhanceView;
        MogoUIManager.Instance.ShowMogoEquipmentUI();
        
    }

    void OnDragonConsumeIconUp()
    {
        LoggerHelper.Debug("Dragon");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    void OnMallConsumeIconUp()
    {
        MogoUIManager.Instance.SwitchToMarket(MarketUITab.HotTab);
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    void OnDiamondToGoldIconUp()
    {
        MogoUIManager.Instance.ShowDiamondToGoldUI(null);
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    void OnSpriteIconUp()
    {
        System.Action callback = (
            () =>
            {
                ElfSystem.Instance.OnRequestInfo();
                ElfSystem.Instance.OnOpenUI(new List<int>());
            });
        MogoUIManager.Instance.ShowSpriteUI(callback);
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    void OnWingIconUp()
    {
        EventDispatcher.TriggerEvent(Events.WingEvent.Open);
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    void OnPackageIconUp()
    {
        MogoUIManager.Instance.ShowMogoMenuUI();
        MogoUIManager.Instance.SwitchPackageUI();
        InventoryManager.Instance.RefreshPackageUI();
        if (MenuUIViewManager.Instance != null)
            MenuUIViewManager.Instance.FakeShowPackageUI();
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    void OnAttributeIconUp()
    {
        MogoUIManager.Instance.SwitchAttributeRewardUI(true);
        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
        //RewardUIViewManager.Instance.SetUIDirty();
        //MogoUIManager.Instance.SwitchNewAttributeRewardUI(true);
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }     

    void OnNormalMainUIPlayerInfoUp()
    {
        LoggerHelper.Debug("NormalMenuUIUp");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
        MogoUIManager.Instance.ShowMogoMenuUI();
    }

    void OnNormalMainUICommunityUp()
    {
        LoggerHelper.Debug("CommunityUp");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, "NormalMainUICommunity");
        MogoUIManager.Instance.ShowMogoCommuntiyUI();
    }

    public void GearInstancePlayIconUp()
    {
        OnInstancePlayIconUp();
        InstanceUILogicManager.Instance.SetMapID(SystemConfig.Instance.LastMap > InstanceMissionChooseUIViewManager.Instance.MapOpenPage - 1 ? 0 : SystemConfig.Instance.LastMap);
    }

    public void OnInstancePlayIconUp()
    {
        if (InstanceMissionChooseUIViewManager.SHOW_MISSION_BY_DRAG)
        {
            MogoUIManager.Instance.ShowInstanceMissionChooseUI();
        }
        else
        {
            MogoUIManager.Instance.ShowNewInstanceChooseMissionUI();
        }
        
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    void OnEnergyBtnUp()
    {
        MogoUIManager.Instance.ShowEnergyUI(null);
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    void OnRankingBtnUp()
    {
		MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        MogoUIManager.Instance.ShowRankingUI(null);        
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    void OnUpgradePowerBtnUp()
    {
        //MogoUIManager.Instance.ShowNewArenaUI(null);
        //MogoUIManager.Instance.ShowOccupyTowerUI(null);
        //MogoUIManager.Instance.ShowOccupyTowerPassUI(null);
        //MogoUIManager.Instance.ShowSpriteUI(null);
        //MogoUIManager.Instance.ShowEnergyNoEnoughUI(null);
        //MogoUIManager.Instance.ShowLevelNoEnoughUI(null);
        //MogoUIManager.Instance.ShowInstanceBossTreasureUI(null);
        //MogoUIManager.Instance.ShowDragonMatchUI();
        MogoUIManager.Instance.ShowUpgradePowerUI(null);
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
    }

    #region  事件    

    public void Initialize()
    {
        NormalMainUIViewManager.Instance.ACTIVITYPLAYICONUP += OnActivityPlayIconUp;
        NormalMainUIViewManager.Instance.ASSISTANTPLAYICONUP += OnAssistantPlayIconUp;
        NormalMainUIViewManager.Instance.AUTOTASKPLAYICONUP += OnAutoTaskPlayIconUp;
        NormalMainUIViewManager.Instance.PVEPLAYICONUP += OnPVEPlayIconUp;
        NormalMainUIViewManager.Instance.PVPPLAYICONUP += OnPVPPlayIconUp;
        NormalMainUIViewManager.Instance.INSTANCEPLAYICONUP += OnInstancePlayIconUp;
        NormalMainUIViewManager.Instance.CHARGEREWARDICONUP += OnChargeRewardIconUp;

        NormalMainUIViewManager.Instance.EQUIPMENTCONSUMEICONUP += OnEquipmentComsumeIconUp;
        NormalMainUIViewManager.Instance.DRAGONCONSUMEICONUP += OnDragonConsumeIconUp;
        NormalMainUIViewManager.Instance.MALLCONSUMEICONUP += OnMallConsumeIconUp;
        NormalMainUIViewManager.Instance.DIAMONDTOGOLDICONUP += OnDiamondToGoldIconUp;
        NormalMainUIViewManager.Instance.SPRITEICONUP += OnSpriteIconUp;
        NormalMainUIViewManager.Instance.WINGICONUP += OnWingIconUp;
        NormalMainUIViewManager.Instance.PACKAGEICONUP += OnPackageIconUp;
        NormalMainUIViewManager.Instance.ATTRIBUTEICONUP += OnAttributeIconUp;

        NormalMainUIViewManager.Instance.NORMALMAINUIPLAYERINFOUP += OnNormalMainUIPlayerInfoUp;

        NormalMainUIViewManager.Instance.NORMALMAINUICOMMUNITYUP += OnNormalMainUICommunityUp;

        NormalMainUIViewManager.Instance.ENERGYUP += OnEnergyBtnUp;
        NormalMainUIViewManager.Instance.RANKINGUP += OnRankingBtnUp;
        NormalMainUIViewManager.Instance.UPGRADEPOWERUP += OnUpgradePowerBtnUp;


        SetBinding<string>(EntityMyself.ATTR_ENERGYSTRING, NormalMainUIViewManager.Instance.SetPlayerEnergyText);
        SetBinding<string>(EntityMyself.ATTR_HPSTRING, NormalMainUIViewManager.Instance.SetPlayerBloodText);
        SetBinding<float>(EntityMyself.ATTR_HP, NormalMainUIViewManager.Instance.SetPlayerBlood);
        SetBinding<float>(EntityMyself.ATTR_EXP, NormalMainUIViewManager.Instance.SetPlayerExp);
        SetBinding<byte>(EntityMyself.ATTR_LEVEL, NormalMainUIViewManager.Instance.SetPlayerLevel);
        SetBinding<byte>(EntityMyself.ATTR_VIP_LEVEL, NormalMainUIViewManager.Instance.SetPlayerVIP);
        SetBinding<string>(EntityMyself.ATTR_HEAD_ICON, NormalMainUIViewManager.Instance.SetPlayerHeadImage);
        SetBinding<int>(EntityMyself.ATTR_GOLD_METALLURGY_LAST_TIMES, NormalMainUIViewManager.Instance.SetGoldMetallurgyLastTimes);
        SetBinding<uint>(EntityParent.ATTR_FIGHT_FORCE, NormalMainUIViewManager.Instance.SetPlayerCurrentPower);

        EventDispatcher.AddEventListener<int>(Events.NormalMainUIEvent.ShowChallegeIconTip, TryShowChallegeIconTip);
        EventDispatcher.AddEventListener<int>(Events.NormalMainUIEvent.HideChallegeIconTip, TryHideChallegeIconTip);
        EventDispatcher.AddEventListener<int>("NormalMainUIEvent.ShowSanctuaryOpenTip", TryShowChallegeIconTip);
        EventDispatcher.AddEventListener<int>("NormalMainUIEvent.HideSanctuaryOpenTip", TryHideChallegeIconTip);
        EventDispatcher.AddEventListener(Events.NormalMainUIEvent.ShowArenaIconTip, ShowArenaIconTip);
        EventDispatcher.AddEventListener(Events.NormalMainUIEvent.HideArenaIconTip, HideArenaIconTip);
        EventDispatcher.AddEventListener<bool>(Events.NormalMainUIEvent.ShowMallConsumeIconTip, ShowMallConsumeIconTip);
        EventDispatcher.AddEventListener("OperatingUIRefreshTip", OnOperatingUIRefreshTip);

        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, OnInstanceLoaded);  
    }

    public override void Release()
    {
        base.Release();
        NormalMainUIViewManager.Instance.ACTIVITYPLAYICONUP -= OnActivityPlayIconUp;
        NormalMainUIViewManager.Instance.ASSISTANTPLAYICONUP -= OnAssistantPlayIconUp;
        NormalMainUIViewManager.Instance.AUTOTASKPLAYICONUP -= OnAutoTaskPlayIconUp;
        NormalMainUIViewManager.Instance.PVEPLAYICONUP -= OnPVEPlayIconUp;
        NormalMainUIViewManager.Instance.PVPPLAYICONUP -= OnPVPPlayIconUp;

        NormalMainUIViewManager.Instance.EQUIPMENTCONSUMEICONUP -= OnEquipmentComsumeIconUp;
        NormalMainUIViewManager.Instance.DRAGONCONSUMEICONUP -= OnDragonConsumeIconUp;
        NormalMainUIViewManager.Instance.MALLCONSUMEICONUP -= OnMallConsumeIconUp;
        NormalMainUIViewManager.Instance.DIAMONDTOGOLDICONUP -= OnDiamondToGoldIconUp;
        NormalMainUIViewManager.Instance.SPRITEICONUP -= OnSpriteIconUp;
        NormalMainUIViewManager.Instance.WINGICONUP -= OnWingIconUp;

        NormalMainUIViewManager.Instance.NORMALMAINUIPLAYERINFOUP -= OnNormalMainUIPlayerInfoUp;
        NormalMainUIViewManager.Instance.NORMALMAINUICOMMUNITYUP -= OnNormalMainUICommunityUp;
        NormalMainUIViewManager.Instance.INSTANCEPLAYICONUP -= OnInstancePlayIconUp;
        NormalMainUIViewManager.Instance.CHARGEREWARDICONUP -= OnChargeRewardIconUp;
        NormalMainUIViewManager.Instance.ENERGYUP -= OnEnergyBtnUp;
        NormalMainUIViewManager.Instance.RANKINGUP -= OnRankingBtnUp;
        NormalMainUIViewManager.Instance.UPGRADEPOWERUP -= OnUpgradePowerBtnUp;

        EventDispatcher.RemoveEventListener<int>(Events.NormalMainUIEvent.ShowChallegeIconTip, TryShowChallegeIconTip);
        EventDispatcher.RemoveEventListener<int>(Events.NormalMainUIEvent.HideChallegeIconTip, TryHideChallegeIconTip);
        EventDispatcher.RemoveEventListener<int>("NormalMainUIEvent.ShowSanctuaryOpenTip", TryShowChallegeIconTip);
        EventDispatcher.RemoveEventListener<int>("NormalMainUIEvent.HideSanctuaryOpenTip", TryHideChallegeIconTip);
        EventDispatcher.RemoveEventListener(Events.NormalMainUIEvent.ShowArenaIconTip, ShowArenaIconTip);
        EventDispatcher.RemoveEventListener(Events.NormalMainUIEvent.HideArenaIconTip, HideArenaIconTip);
        EventDispatcher.RemoveEventListener<bool>(Events.NormalMainUIEvent.ShowMallConsumeIconTip, ShowMallConsumeIconTip);
        EventDispatcher.RemoveEventListener("OperatingUIRefreshTip", OnOperatingUIRefreshTip);

        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, OnInstanceLoaded);  
    }

    #endregion

    public void SetAssistantDialogTextHide(uint ms)
    {
        TimerHeap.AddTimer(ms, 0, NormalMainUIViewManager.Instance.ShowAssistantDialog, false);
    }

    protected bool doorOfBuryShow = false;
    protected bool sanctuaryDefenseShow = false;

    protected void TryShowChallegeIconTip(int i)
    {
        switch (i)
        {
            case (int)ChallengeGridID.DoorOfBury:
                doorOfBuryShow = true;
                break;

            case (int)ChallengeGridID.Sanctuary:
                sanctuaryDefenseShow = true;
                break;

            default: 
                break;
        }

        NormalMainUIViewManager.Instance.ShowChallegeIconTip();
    }

    protected void TryHideChallegeIconTip(int i)
    {
        switch (i)
        {
            case (int)ChallengeGridID.DoorOfBury:
                doorOfBuryShow = false;
                break;

            case (int)ChallengeGridID.Sanctuary:
                sanctuaryDefenseShow = false;
                break;

            default:
                break;
        }

        if (!doorOfBuryShow && !sanctuaryDefenseShow)
            NormalMainUIViewManager.Instance.HideChallegeIconTip();
    }

    protected void ShowArenaIconTip()
    {
        NormalMainUIViewManager.Instance.ShowArenaIconTip();
    }

    protected void HideArenaIconTip()
    {
        NormalMainUIViewManager.Instance.HideArenaIconTip();
    }

    protected void ShowMallConsumeIconTip(bool isShow)
    {
        NormalMainUIViewManager.Instance.ShowMallConsumeIconTip(isShow);
    }

    #region 活动/充值奖励

    bool m_bAttributeRewardTip = false;
    public bool AttributeRewardTip
    {
        get { return m_bAttributeRewardTip;}
        set
        {
            m_bAttributeRewardTip = value;
            OnOperatingUIRefreshTip();
            OnNormailMainUIRefreshTip();         
        }
    }

    bool m_bLoginRewardTip = false;
    public bool LoginRewardTip
    {
        get { return m_bLoginRewardTip; }
        set
        {
            m_bLoginRewardTip = value;
            OnOperatingUIRefreshTip();
            OnNormailMainUIRefreshTip();        
        }
    }

    bool m_bTimeLimitActivityTip = false;
    public bool TimeLimitActivityTip
    {
        get { return m_bTimeLimitActivityTip; }
        set
        {
            m_bTimeLimitActivityTip = value;
            OnOperatingUIRefreshTip();
            OnNormailMainUIRefreshTip();
        }
    }

    /// <summary>
    /// 主界面运营活动Tip
    /// </summary>
    void OnNormailMainUIRefreshTip()
    {
        if(!AttributeRewardTip && !LoginRewardTip && !TimeLimitActivityTip)
            NormalMainUIViewManager.Instance.ShowChargeRewardIconTip(false);
        else
            NormalMainUIViewManager.Instance.ShowChargeRewardIconTip(true);
    }

    /// <summary>
    /// 运营系统运营活动Tip
    /// </summary>
    void OnOperatingUIRefreshTip()
    {
        if (OperatingUIViewManager.Instance != null)
        {
            OperatingUIViewManager.Instance.ShowAttributeRewardBtnTip(AttributeRewardTip);
            OperatingUIViewManager.Instance.ShowLoginRewardBtnTip(LoginRewardTip);
            OperatingUIViewManager.Instance.ShowTimeLimitActivityBtnTip(TimeLimitActivityTip);
        }
    }

    #endregion

    #region 战力提升引导

    private void OnInstanceLoaded(int sceneID, bool isInstance)
    {
        // 解决泛红没消失Bug,容错
        if(sceneID == MogoWorld.globalSetting.homeScene)
        {
            MogoGlobleUIManager.Instance.ShowBattlePlayerBloodTipPanel(false);
        }

        // 在副本中死亡
        // 在竞技场中死亡
        if (MogoWorld.thePlayer.HasDeadBefore && sceneID == MogoWorld.globalSetting.homeScene)
        {
            if (UpgradePowerUIViewManager.CalIsShowUpgradePowerTipDialog())
            {
                MogoUIQueue.Instance.PushOne(() => { TruelyShowUpgradePowerUI(); }, null, "ShowUpgradePowerUI");
            }
        }
    }

    private void TruelyShowUpgradePowerUI()
    {
        MogoUIManager.Instance.ShowUpgradePowerUI(()=>
        {
            MogoUIQueue.Instance.Locked = false;
        });
    }

    #endregion
}

