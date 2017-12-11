/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MainUILogicManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：主界面逻辑控制器
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using System.Collections.Generic;

public class MainUILogicManager : UILogicManager
{
    private static MainUILogicManager m_instance;
    public static MainUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new MainUILogicManager();
            }

            return MainUILogicManager.m_instance;

        }
    }

    public bool isNormalAttackPowerUp = false;
    public bool hasShowBossBlood = false;

    private void OnPowerChargeStart()
    {
        if (!IsAttackable)
            return;

        EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnPowerChargeStart);
    }

    private void OnPowerChargeInterrupt()
    {
        if (!IsAttackable)
            return;

        EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnPowerChargeInterrupt);
    }

    private void OnPowerChargeComplete()
    {
        if (!IsAttackable)
            return;

        EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnPowerChargeComplete);
    }

    private void OnNormalAttack()
    {
        if (!IsAttackable)
            return;

        EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnNormalAttack);
    }

    private void OnTaskInfoUp()
    {
        LoggerHelper.Debug("TaskInfoUp");
    }

    private void OnOutputUp()
    {
        if (!IsAttackable)
            return;

        EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpellOneAttack);
    }

    void OnAffectUp()
    {
        if (!IsAttackable)
            return;

        EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpellTwoAttack);
    }

    void OnMoveUp()
    {
        if (!IsAttackable)
            return;

        EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpellThreeAttack);
    }

    void OnSpecialUp()
    {
        if (!IsAttackable)
            return;

        EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnSpellXPAttack);
    }

    public void ShowSpriteSkillButton()
    {
        if (MogoWorld.thePlayer != null && MogoWorld.thePlayer.ElfEquipSkillId > 0)
        {
            MainUIViewManager.Instance.ShowSpriteSkillButton(true);
            MainUIViewManager.Instance.SetSpriteSkillImage(IconData.dataMap.Get(SkillData.dataMap.Get((int)MogoWorld.thePlayer.ElfEquipSkillId).icon).path);
        }
        else
        {
            MainUIViewManager.Instance.ShowSpriteSkillButton(false);
        }
    }

    void OnSpriteSkillUp()
    {
        if (!IsAttackable)
            return;
        MainUIViewManager.Instance.ShowMainUISpriteSkillPanel(true);
        TimerHeap.AddTimer(5000, 0, () => { MainUIViewManager.Instance.ShowMainUISpriteSkillPanel(false); });


    }

    void OnUseHpBottle()
    {
        if (MainUIViewManager.Instance.HpBottleCD == 0)
        {
            MogoWorld.thePlayer.UseHpBottle();
        }

    }

    void OnCommunityUp()
    {
        LoggerHelper.Debug("CommunityUp");
        EventDispatcher.TriggerEvent(EntityMyself.ON_END_TASK_GUIDE);
        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, "Community");
        MogoUIManager.Instance.ShowMogoCommuntiyUI(CommunityUIParent.MainUI, true);
    }


    #region boss血条

    // 默认一筒血为500HP
    protected int defaultBlood = 500;
    protected int defaultBloodCount = 1;

    public void SetBossMessageInit(EntityParent boss, int bloodNum)
    {
        hasShowBossBlood = true;

        // Mogo.Util.LoggerHelper.Debug("Init boss hp: " + boss.hp);
        // if (boss is EntityMercenary)
        //    Mogo.Util.LoggerHelper.Debug("Init boss hpBase: " + (boss as EntityMercenary).MonsterData.hpBase);
        // Mogo.Util.LoggerHelper.Debug("Init boss curHp: " + boss.curHp);       

        MainUIViewManager.Instance.SetBossTargetName(boss.name, boss.MonsterData.monsterType);
        MainUIViewManager.Instance.SetBossTargetFace(boss.HeadIcon, boss.MonsterData.monsterType);
        MainUIViewManager.Instance.SetBossTargetLevel(boss.level, boss.MonsterData.monsterType);

        // 处理血
        defaultBloodCount = bloodNum;
        defaultBlood = (int)boss.MonsterData.hpBase / bloodNum;

        if (defaultBlood == 0)
            LoggerHelper.Error("defaultBlood == 0: " + boss.ID + " " + boss.GameObject.name);

        int currentBloodCount = (int)boss.curHp / defaultBlood;
        int sumOdd = (int)boss.curHp - defaultBloodCount * defaultBlood;

        int currentBloodOdd = (int)boss.curHp % defaultBlood;
        float currentBloodRate = 0;

        if (sumOdd > 0) // 血量有极少量余数
        {
            currentBloodRate = (float)(boss.curHp - (defaultBloodCount - 1) * defaultBlood) / defaultBlood;
        }
        else // 已经扣血
        {
            if (currentBloodCount == 0)
            {
                currentBloodCount = 1;
                currentBloodRate = (float)currentBloodOdd / defaultBlood;
            }
            else if (currentBloodOdd != 0)
            {
                currentBloodCount += 1;
                currentBloodRate = (float)currentBloodOdd / defaultBlood;

            }
            else
            {
                currentBloodOdd = defaultBlood;
                currentBloodRate = 1;
            }
        }

        MainUIViewManager.Instance.ShowBossTarget(true, 4 - (currentBloodCount - 1) % 5, boss.MonsterData.monsterType);
        MainUIViewManager.Instance.SetBossTargetBlood(currentBloodRate >= 1 ? 1 : currentBloodRate, (currentBloodCount - 1) % 5);
    }


    // Flush Blood
    public void FlushBossBlood(EntityParent boss, int bossBlood)
    {
        //Mogo.Util.LoggerHelper.Error("FlushBossBlood boss hp: " + boss.hp);
        // if (boss is EntityMercenary)
        //     Mogo.Util.LoggerHelper.Error("FlushBossBlood boss hpBase: " + (boss as EntityMercenary).MonsterData.hpBase);
        // Mogo.Util.LoggerHelper.Error("FlushBossBlood boss curHp: " + boss.curHp);

        if (bossBlood == 0 && hasShowBossBlood)
        {
            // MainUIViewManager.Instance.ShowBossTarget(false);
            return;
        }

        if (!hasShowBossBlood && boss.MonsterData.hpShow != null && boss.MonsterData.hpShow.Count >= 3)
            SetBossMessageInit(boss, boss.MonsterData.hpShow[2]);

        int currentBloodCount = bossBlood / defaultBlood;
        int currentBloodOdd = bossBlood % defaultBlood;
        float currentBloodRate = 0;

        if (currentBloodOdd != 0)
        {
            currentBloodCount += 1;
            currentBloodRate = (float)currentBloodOdd / defaultBlood;
        }
        else
        {
            if (bossBlood != boss.hp)
                currentBloodCount += 1;
            currentBloodOdd = defaultBlood;
            currentBloodRate = 1;
        }

        MainUIViewManager.Instance.SetBossTargetBlood(currentBloodRate, (currentBloodCount - 1) % 5);
    }

    #endregion


    #region 雇佣兵血条

    public void SetMercenaryMessageInit(EntityMercenary mercenary)
    {
        // MainUIViewManager.Instance.SetMember1Image(IconData.dataMap.Get((int)IconOffset.Avatar + (int)mercenary.vocation).path);

        MainUIViewManager.Instance.SetMember1Name(InstanceUILogicManager.Instance.mercenaryName);
        MainUIViewManager.Instance.SetMember1Level(mercenary.level);
        MainUIViewManager.Instance.SetMember1Blood((int)(mercenary.curHp * 100 / mercenary.hp));

        MainUIViewManager.Instance.ShowMember1(true);
    }

    public void FlushMercenaryBlood(int mercenaryPercentageHP)
    {
        MainUIViewManager.Instance.SetMember1Blood(mercenaryPercentageHP);
    }

    #endregion


    #region 塔防

    public void SetPlayerMessageInit(EntityParent player, int i)
    {
        switch (i)
        {
            case 1:
                MainUIViewManager.Instance.SetMember1Name(player.name);
                MainUIViewManager.Instance.SetMember1Level(player.level);
                MainUIViewManager.Instance.SetMember1Blood((int)(player.curHp * 100 / player.hp));
                MainUIViewManager.Instance.ShowMember1(true);
                break;

            case 2:
                MainUIViewManager.Instance.SetMember2Name(player.name);
                MainUIViewManager.Instance.SetMember2Level(player.level);
                MainUIViewManager.Instance.SetMember2Blood((int)(player.curHp * 100 / player.hp));
                MainUIViewManager.Instance.ShowMember2(true);
                break;

            case 3:
                MainUIViewManager.Instance.SetMember3Name(player.name);
                MainUIViewManager.Instance.SetMember3Level(player.level);
                MainUIViewManager.Instance.SetMember3Blood((int)(player.curHp * 100 / player.hp));
                MainUIViewManager.Instance.ShowMember3(true);
                break;

            default:
                Debug.LogError("Are You Kidding Me ?");
                break;
        }
    }

    public void FlushPlayerBlood(int playerPercentageHP, int i)
    {
        switch (i)
        {
            case 1:
                MainUIViewManager.Instance.SetMember1Blood(playerPercentageHP);
                break;

            case 2:
                MainUIViewManager.Instance.SetMember2Blood(playerPercentageHP);
                break;

            case 3:
                MainUIViewManager.Instance.SetMember3Blood(playerPercentageHP);
                break;

            default:
                Debug.LogError("Are You Kidding Me ?");
                break;
        }
    }

    public void RemovePlayerMessage(int i)
    {
        switch (i)
        {
            case 1:
                MainUIViewManager.Instance.ShowMember1(false);
                break;

            case 2:
                MainUIViewManager.Instance.ShowMember2(false);
                break;

            case 3:
                MainUIViewManager.Instance.ShowMember3(false);
                break;

            default:
                Debug.LogError("Are You Kidding Me ?");
                break;
        }
    }

    public void InitTDBlood(EntityMonster tower)
    {
        MainUIViewManager.Instance.ShowTDBlood(true);
    }

    public void FlushTDBlood(float percentage)
    {
        MainUIViewManager.Instance.SetTDBlood(percentage);
    }

    public void ResetTDBlood()
    {
        MainUIViewManager.Instance.ShowTDBlood(false);
        MainUIViewManager.Instance.ShowTDTip(false);
    }

    #endregion


    #region 快速回复

    protected void EnterInstance(int missionID, bool isInstance)
    {
        MapType type = MapData.dataMap.Get(missionID).type;
        if (type == MapType.TOWERDEFENCE || type == MapType.OCCUPY_TOWER)
        {
            InitBattleCommunityUIShortCutMessage(type);
        }
        else
        {
            ReleaseBattleCommunityUIShortCutMessage();
        }
    }


    public void InitBattleCommunityUIShortCutMessage(MapType type)
    {
        if (!ChatListData.dataMap.ContainsKey((int)type))
            return;

        List<int> chatList = ChatListData.dataMap.Get((int)type).chatList;
        if (chatList == null)
            return;

        List<string> chatString = new List<string>();
        foreach (var chatMessageID in chatList)
            chatString.Add(LanguageData.GetContent(chatMessageID));

        switch (type)
        {
            case MapType.TOWERDEFENCE:
                MainUIViewManager.Instance.CurChannel = ChannelId.TOWER_DEFENCE;
                break;

            case MapType.OCCUPY_TOWER:
                MainUIViewManager.Instance.CurChannel = ChannelId.OCCUPY_TOWER;
                break;
        }

        MainUIViewManager.Instance.ShowShortCutCommunityUI(true);
        MainUIViewManager.Instance.AddShortCutCommunityGrid(chatString);
    }

    public void ReleaseBattleCommunityUIShortCutMessage()
    {
        MainUIViewManager.Instance.ShowShortCutCommunityUI(false);
        MainUIViewManager.Instance.EmptyCommunityGridList();
        MainUIViewManager.Instance.EmptyCommunityMessageList();
    }

    #endregion


    // Use this for initialization
    public void Initialize()
    {
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.TASKINFOUP, OnTaskInfoUp);

        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.AFFECTUP, OnAffectUp);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.OUTPUTUP, OnOutputUp);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.MOVEUP, OnMoveUp);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.SPECIALUP, OnSpecialUp);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.SPRITESKILLUP, OnSpriteSkillUp);

        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.NORMALATTACK, OnNormalAttack);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.POWERCHARGESTART, OnPowerChargeStart);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.POWERCHARGECOMPLETE, OnPowerChargeComplete);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.POWERCHARGEINTERRUPT, OnPowerChargeInterrupt);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.MAINUIITEM1BGUP, OnUseHpBottle);

        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.COMMUNITY, OnCommunityUp);

        EventDispatcher.AddEventListener<EntityParent, int>(Events.UIBattleEvent.OnFlushBossBlood, FlushBossBlood);
        EventDispatcher.AddEventListener<int>(Events.UIBattleEvent.OnFlushMercenaryBlood, FlushMercenaryBlood);

        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, EnterInstance);

        SetBinding<string>(EntityMyself.ATTR_HPSTRING, MainUIViewManager.Instance.SetPlayerBloodText);
        SetBinding<float>(EntityMyself.ATTR_HP, MainUIViewManager.Instance.SetPlayerBlood);
        SetBinding<float>(EntityMyself.ATTR_EXP, MainUIViewManager.Instance.SetPlayerExp);
        SetBinding<byte>(EntityMyself.ATTR_LEVEL, MainUIViewManager.Instance.SetPlayerLevel);
        SetBinding<string>(EntityMyself.ATTR_HEAD_ICON, MainUIViewManager.Instance.SetPlayerHeadImage);
        SetBinding<byte>(EntityMyself.ATTR_HP_COUNT, MainUIViewManager.Instance.SetItem1Num);
    }

    public override void Release()
    {
        base.Release();
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.TASKINFOUP, OnTaskInfoUp);

        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.AFFECTUP, OnAffectUp);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.OUTPUTUP, OnOutputUp);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.MOVEUP, OnMoveUp);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.SPECIALUP, OnSpecialUp);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.SPRITESKILLUP, OnSpriteSkillUp);

        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.NORMALATTACK, OnNormalAttack);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.POWERCHARGESTART, OnPowerChargeStart);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.POWERCHARGECOMPLETE, OnPowerChargeComplete);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.POWERCHARGEINTERRUPT, OnPowerChargeInterrupt);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.MAINUIITEM1BGUP, OnUseHpBottle);

        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.COMMUNITY, OnCommunityUp);

        EventDispatcher.RemoveEventListener<EntityParent, int>(Events.UIBattleEvent.OnFlushBossBlood, FlushBossBlood);
        EventDispatcher.RemoveEventListener<int>(Events.UIBattleEvent.OnFlushMercenaryBlood, FlushMercenaryBlood);
    }

    protected bool isAttackable = true;
    public bool IsAttackable
    {
        get { return isAttackable; }
        set { isAttackable = value; }
    }
}
