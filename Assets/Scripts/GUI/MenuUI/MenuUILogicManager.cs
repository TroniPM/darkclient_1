/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MenuUILogicManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.Util;
using Mogo.GameData;

public class MenuUILogicManager : UILogicManager
{
    private static MenuUILogicManager m_instance;
    public static MenuUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new MenuUILogicManager();                
            }

            return MenuUILogicManager.m_instance;
        }
    }

    public void SetInventoryGridItem(ItemParent item)
    {
        if (item.maxStack < 1)
        {
            MenuUIViewManager.Instance.SetPackageGridNumVisible(item.gridIndex, false);
        }
        else
        {
            MenuUIViewManager.Instance.SetPackageGridNum(item.stack.ToString(), item.gridIndex);
            MenuUIViewManager.Instance.SetPackageGridNumVisible(item.gridIndex);
        }
        MenuUIViewManager.Instance.SetPackageGridImage(item.icon, item.gridIndex, item.color);
        if (item.itemType == 2)
            MenuUIViewManager.Instance.SetPackageGridBG(IconData.blankBox, item.gridIndex);
        else
            MenuUIViewManager.Instance.SetPackageGridBG(IconData.GetIconByQuality(item.quality), item.gridIndex);
    }

    public void RemoveInventoryGridItem(ItemParent item)
    {
        RemoveInventoryGridItem(item.gridIndex);
    }

    public void RemoveInventoryGridItem(int gridIndex)
    {
        MenuUIViewManager.Instance.RemovePackageItem(gridIndex);
    }

    /// <summary>
    /// 指定背包单元数据
    /// </summary>
    /// <param name="itemList"></param>
    public void SetInventoryItems(List<ItemParent> itemList)
    {
        SetInventoryItemCount(itemList.Count);
        for (int i = 0; i < itemList.Count; ++i)
        {
            SetInventoryGridItem(itemList[i]);
        }

    }

    /// <summary>
    /// 重新设定背包单元数据
    /// </summary>
    /// <param name="itemList"></param>
    public void ResetInventoryItems(List<ItemParent> itemList)
    {
        //System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        //stopWatch.Start();
        MenuUIViewManager.Instance.ClearPackageItems();
        //stopWatch.Stop();
        //Debug.LogError("ClearPackageItems:" + stopWatch.Elapsed.Milliseconds);

        //stopWatch = new System.Diagnostics.Stopwatch();
        //stopWatch.Start();
        SetInventoryItems(itemList);
        MenuUIViewManager.Instance.RefreshPackageList();
        //stopWatch.Stop();
        //Debug.LogError("SetInventoryItems:" + stopWatch.Elapsed.Milliseconds);
    }

    public void ResetInventoryItemsInPage(int page, List<ItemParent> itemList)
    {
        MenuUIViewManager.Instance.ClearPackgeItemsInPage(page);
        SetInventoryItems(itemList);
    }

    public void SetInventoryGold(int gold)
    {
        MenuUIViewManager.Instance.SetPackageGold(gold.ToString());
    }

    public void SetInventoryDiamond(int diamon)
    {
        MenuUIViewManager.Instance.SetPackageDiamond(diamon.ToString());
    }

    public void SetInventoryItemCount(int count)
    {
        MenuUIViewManager.Instance.SetPackageItemCount(count.ToString());
    }

    int m_iCurrentDownIconTab = 0;
    public int CurrentDownIconTab
    {
        get { return m_iCurrentDownIconTab; }
        set
        {
            m_iCurrentDownIconTab = value;
        }
    }

    void OnPackageIconUp()
    {
        if (CurrentDownIconTab != (int)MenuUITab.PackageTab)
        {
            Mogo.Util.LoggerHelper.Debug("PackageIconUp");
            MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007013));
            MogoUIManager.Instance.SwitchPackageUI();
            InventoryManager.Instance.m_currentPackageView = InventoryManager.View.PackageView;
            InventoryManager.Instance.CurrentView = InventoryManager.View.PackageView;
            InventoryManager.Instance.RefreshPackageUI();
            if (InventoryManager.Instance.m_isNeedAutoSortBag)
            {
                InventoryManager.Instance.SortTheBag();
            }

            MenuUIViewManager.Instance.ResetDefaultPackagePage();
            CurrentDownIconTab = (int)MenuUITab.PackageTab;
        }
    }

    void OnPlayerIconUp()
    {
        if (CurrentDownIconTab != (int)MenuUITab.PlayerTab)
        {
            Mogo.Util.LoggerHelper.Debug("PlayerIconUp");

            Mogo.Util.LoggerHelper.Debug("nextLevelExp" + MogoWorld.thePlayer.nextLevelExp);
            Mogo.Util.LoggerHelper.Debug("exp" + MogoWorld.thePlayer.exp);

            MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007014));
            MogoUIManager.Instance.SwitchPlayerUI();
            InventoryManager.Instance.m_currentPackageView = InventoryManager.View.PlayerEquipment;
            InventoryManager.Instance.CurrentView = InventoryManager.View.PlayerEquipment;
            InventoryManager.Instance.RefreshPlayerEquipmentInfoUI();
            CurrentDownIconTab = (int)MenuUITab.PlayerTab;
        }
    }

    void OnSettingsIconUp()
    {
        if (CurrentDownIconTab != (int)MenuUITab.SettingsTab)
        {
            Mogo.Util.LoggerHelper.Debug("SettingsIconUp");

            MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007018));
            MogoUIManager.Instance.SwitchSettingsUI();
            CurrentDownIconTab = (int)MenuUITab.SettingsTab;
        }
    }

    void OnSkillIconUp()
    {
        if (CurrentDownIconTab != (int)MenuUITab.SkillTab)
        {
            MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007015));
            MogoUIManager.Instance.SwitchSkillUI();
            if (SkillUIViewManager.Instance != null)
            {
                EventDispatcher.TriggerEvent(Events.SpellEvent.OpenView);
            }
            CurrentDownIconTab = (int)MenuUITab.SkillTab;
        }
    }

    void OnSocialIconUp()
    {
        if (CurrentDownIconTab != (int)MenuUITab.SocialTab)
        {
            Mogo.Util.LoggerHelper.Debug("SocialIconUp");

            MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007017));
            MogoUIManager.Instance.SwitchSocialUI();
            CurrentDownIconTab = (int)MenuUITab.SocialTab;
        }
    }

    void OnTongIconUp()
    {
        if (CurrentDownIconTab != (int)MenuUITab.TongTab)
        {
            Mogo.Util.LoggerHelper.Debug("TongIconup");

            MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007016));
            MogoUIManager.Instance.SwitchTongUI();
            CurrentDownIconTab = (int)MenuUITab.TongTab;
        }
    }

    private void OnPackageEquipDetailInsetUp()
    {
        Mogo.Util.LoggerHelper.Debug("PackageEquipDetailInsetUp");

        EventDispatcher.TriggerEvent(InventoryManager.ON_EQUIP_INSET);
    }

    private void OnPackageEquipDetailStrenthUp()
    {
        Mogo.Util.LoggerHelper.Debug("PackageEquipDetailStrenthUp");

        EventDispatcher.TriggerEvent(InventoryManager.ON_STRENTHEN_EQUIP);
    }

    private void OnPackageEquipDetailUnLoadUp()
    {
        Mogo.Util.LoggerHelper.Debug("PackageEquipDetailUnLoadUp");
        EventDispatcher.TriggerEvent(InventoryManager.ON_REMOVE_EQUIP);
    }

    #region 背包Tab

    int m_iCurrentPageID = 0;

    private void OnGemIconUp()
    {
        if (m_iCurrentPageID != 1) // 切换Tab
        {
            Mogo.Util.LoggerHelper.Debug("GemIconUp");
            EventDispatcher.TriggerEvent<int>(InventoryManager.ON_PACKAGE_SWITCH, InventoryManager.ITEM_TYPE_JEWEL);
            m_iCurrentPageID = 1;

            MenuUIViewManager.Instance.SetCurrentPage(0);
        }
        else // 当前Tab,滑动到第一页
        {
            Mogo.Util.LoggerHelper.Debug("GemIconUp");
            MenuUIViewManager.Instance.MoveToPackagePage(0);
        }
    }

    private void OnNormalIconUp()
    {
        if (m_iCurrentPageID != 0) // 切换Tab
        {
            Mogo.Util.LoggerHelper.Debug("NormalIconUp");
            EventDispatcher.TriggerEvent<int>(InventoryManager.ON_PACKAGE_SWITCH, InventoryManager.ITEM_TYPE_EQUIPMENT);
            m_iCurrentPageID = 0;

            MenuUIViewManager.Instance.SetCurrentPage(0);
        }
        else // 当前Tab,滑动到第一页
        {
            Mogo.Util.LoggerHelper.Debug("NormalIconUp");
            MenuUIViewManager.Instance.MoveToPackagePage(0);
        }
    }

    private void OnMaterialIconUp()
    {
        if (m_iCurrentPageID != 2) // 切换Tab
        {
            Mogo.Util.LoggerHelper.Debug("MaterialIconUp");
            EventDispatcher.TriggerEvent<int>(InventoryManager.ON_PACKAGE_SWITCH, InventoryManager.ITEM_TYPE_MATERIAL);
            m_iCurrentPageID = 2;

            MenuUIViewManager.Instance.SetCurrentPage(0);
        }
        else // 当前Tab,滑动到第一页
        {
            Mogo.Util.LoggerHelper.Debug("MaterialIconUp");
            MenuUIViewManager.Instance.MoveToPackagePage(0);
        }
    }

    /// <summary>
    /// 背包滑动到最后一页继续向后滑动切换到下一个Tab
    /// </summary>
    private void OnPackageOutOfBoundsMaxPage()
    {
        if (m_iCurrentPageID == 0)
        {
            MenuUIViewManager.Instance.FakeGemPackage(0);
        }
        else if (m_iCurrentPageID == 1)
        {
            MenuUIViewManager.Instance.FakeMaterialPackage(0);
        }
    }

    /// <summary>
    /// 背包滑动到第一页继续向前滑动切换到上一个Tab
    /// </summary>
    private void OnPackageOutOfBoundsMinPage()
    {
        if (m_iCurrentPageID == 2)
        {
            MenuUIViewManager.Instance.FakeGemPackage(3);
        }
        else if (m_iCurrentPageID == 1)
        {
            MenuUIViewManager.Instance.FakeNormalPackage(3);
        }
    }

    #endregion

    private void OnDiamondTipBuyUp()
    {
        Mogo.Util.LoggerHelper.Debug("Buy");
    }
    private void OnDiamondTipCloseUp()
    {
        Mogo.Util.LoggerHelper.Debug("Close");
    }

    private void OnDiamondTipComposeUp()
    {
        Mogo.Util.LoggerHelper.Debug("Compose");
        EventDispatcher.TriggerEvent(InventoryManager.ON_COMPOSE);
    }

    private void OnDiamondTipHistroyUp()
    {
        Mogo.Util.LoggerHelper.Debug("Histroy");
    }

    public LuaTable EquipFXLuaTable;

    void OnSepcialEffectsResp(LuaTable data)
    {
        EquipFXLuaTable = data;
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EquipFXUI, MFUIManager.MFUIID.None, 0, true);
        EquipFXUILogicManager.Instance.FillJewelFXGridList(EquipFXLuaTable);
        EquipFXUILogicManager.Instance.SetUIDirty();
    }

    void OnDiamondTipInsetUp()
    {
        Mogo.Util.LoggerHelper.Debug("Inset");
        MenuUIViewManager.Instance.ShowDiamondInfoTip(false);
        EventDispatcher.TriggerEvent(InventoryManager.ON_INSET_JEWEL);
    }

    void OnNormalMainUIPlayerInfoUp()
    {
        if (CurrentDownIconTab == (int)MenuUITab.SkillTab)
        {
            EventDispatcher.TriggerEvent(Events.SpellEvent.OpenView);
        }
    }

    public void Initialize()
    {
        EventDispatcher.AddEventListener<int>(MenuUIDict.MenuUIEvent.PACKAGEGRIDUP, OnInventoryGridUp);

        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGESORTUP, OnInventorySortUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGERESOLVEUP, OnInventoryDecomposeUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILPUTUPUP, OnInventoryEquipmentDetailEquipUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSAILUP, OnInventoryEquipDetailSailUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSHOWUPUP, OnInventoryEquipDetailShowUpUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFOCLOSEUP, OnPackageEquipDetailCloseUp);

        EventDispatcher.AddEventListener<int>(MenuUIDict.MenuUIEvent.PLAYERUIEQUPMENTGRIDUP, OnEquipmentGridUp);

        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEICONUP, OnPackageIconUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PLAYERICONUP, OnPlayerIconUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.SETTINGSICONUP, OnSettingsIconUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.SKILLICONUP, OnSkillIconUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.SOCIALICONUP, OnSocialIconUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.TONGICONUP, OnTongIconUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILINSETUP, OnPackageEquipDetailInsetUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSTRENTHUP, OnPackageEquipDetailStrenthUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILUNLOADUP, OnPackageEquipDetailUnLoadUp);



        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.GEMICONUP, OnGemIconUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.NORMALICONUP, OnNormalIconUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.MATERIALICONUP, OnMaterialIconUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEOUTOFBOUNDSMAXUP, OnPackageOutOfBoundsMaxPage);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEOUTOFBOUNDSMINUP, OnPackageOutOfBoundsMinPage);

        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPBUYUP, OnDiamondTipBuyUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPCLOSEUP, OnDiamondTipCloseUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPCOMPOSEUP, OnDiamondTipComposeUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPHISTROYUP, OnDiamondTipHistroyUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPINSETUP, OnDiamondTipInsetUp);

        EventDispatcher.AddEventListener<LuaTable>("SyncSepcialEffectsResp", OnSepcialEffectsResp);


        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PLAYERBASICATTRBUTE, OnPlayerBasicAttrUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PLAYERELEMENTATTRBUTE, OnPlayerElementAttrUp);

        NormalMainUIViewManager.Instance.NORMALMAINUIPLAYERINFOUP += OnNormalMainUIPlayerInfoUp;

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
        SetBinding<float>(EntityMyself.ATTR_EXP, SetPlayerExp);
        SetBinding<float>(EntityMyself.ATTR_ENERGY, SetPlayerEnergy);
        SetBinding<string>(EntityMyself.ATTR_NAME, SetPlayerName);
        SetBinding<byte>(EntityMyself.ATTR_LEVEL, SetPlayerLevel);

        SetBinding<uint>(BattleAttr.HEALTH, SetHealth);
        SetBinding<uint>(BattleAttr.ATTACK, SetAttack);
        SetBinding<uint>(BattleAttr.DEFENSE, SetDefence);
        SetBinding<uint>(EntityParent.ATTR_FIGHT_FORCE, MenuUIViewManager.Instance.SetPlayerInfoPower);

        //SetBinding<uint>(BattleAttr.ATTACK, MenuUIViewManager.Instance.SetPlayerDetailAttack);
        //SetBinding<uint>(BattleAttr.DEFENSE, MenuUIViewManager.Instance.SetPlayerDetailDefense);
        SetBinding<uint>(BattleAttr.HIT, MenuUIViewManager.Instance.SetPlayerDetailHit);
        //SetBinding<uint>(BattleAttr.HEALTH, MenuUIViewManager.Instance.SetPlayerDetailHealth);

        SetBinding<uint>(BattleAttr.CRIT, MenuUIViewManager.Instance.SetPlayerDetailCrit);
        SetBinding<uint>(BattleAttr.ANTI_CRIT, MenuUIViewManager.Instance.SetPlayerDetailAntiCrit);
        SetBinding<uint>(BattleAttr.TRUE_STRIKE, MenuUIViewManager.Instance.SetPlayerDetailTrueStrike);
        SetBinding<uint>(BattleAttr.ANTI_TRUE_STRIKE, MenuUIViewManager.Instance.SetPlayerDetailAntiTrueStrike);

        SetBinding<uint>(BattleAttr.ANTI_DEFENSE, MenuUIViewManager.Instance.SetPlayerDetailAntiDefense);
        SetBinding<uint>(BattleAttr.CRIT_EXTRA_ATTACK, MenuUIViewManager.Instance.SetPlayerDetailCritExtraAttack);
        SetBinding<uint>(BattleAttr.PVP_ADDITION, MenuUIViewManager.Instance.SetPlayerDetailPVPAddition);
        SetBinding<uint>(BattleAttr.PVP_ANTI, MenuUIViewManager.Instance.SetPlayerDetailPVE);
        SetBinding<uint>(BattleAttr.CD_REDUCE, MenuUIViewManager.Instance.SetPlayerDetailCDReduce);
        SetBinding<uint>(BattleAttr.SPEED_ADD_RATE, MenuUIViewManager.Instance.SetPlayerDetailSpeedAddRate);

        SetBinding<uint>(BattleAttr.EARTH_DAMAGE, MenuUIViewManager.Instance.SetPlayerDetailEarthDamage);
        SetBinding<uint>(BattleAttr.FIRE_DAMAGE, MenuUIViewManager.Instance.SetPlayerDetailFireDamage);
        SetBinding<uint>(BattleAttr.WATER_DAMAGE, MenuUIViewManager.Instance.SetPlayerDetailWaterDamage);
        SetBinding<uint>(BattleAttr.AIR_DAMAGE, MenuUIViewManager.Instance.SetPlayerDetailWindDamage);
        SetBinding<uint>(BattleAttr.ALL_ELEMENTS_DAMAGE, MenuUIViewManager.Instance.SetPlayerDetailAllElementsDamage);

        SetBinding<uint>(BattleAttr.EARTH_DEFENSE, MenuUIViewManager.Instance.SetPlayerDetailEarthDefense);
        SetBinding<uint>(BattleAttr.FIRE_DEFENSE, MenuUIViewManager.Instance.SetPlayerDetailFireDefense);
        SetBinding<uint>(BattleAttr.WATER_DEFENSE, MenuUIViewManager.Instance.SetPlayerDetailWaterDefense);
        SetBinding<uint>(BattleAttr.AIR_DEFENSE, MenuUIViewManager.Instance.SetPlayerDetailWindDefense);
        SetBinding<uint>(BattleAttr.ALL_ELEMENTS_DEFENSE, MenuUIViewManager.Instance.SetPlayerAllElementsDefense);
    }

    public override void Release()
    {
        base.Release();
        EventDispatcher.RemoveEventListener<int>(MenuUIDict.MenuUIEvent.PACKAGEGRIDUP, OnInventoryGridUp);

        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGESORTUP, OnInventorySortUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGERESOLVEUP, OnInventoryDecomposeUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILPUTUPUP, OnInventoryEquipmentDetailEquipUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSAILUP, OnInventoryEquipDetailSailUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSHOWUPUP, OnInventoryEquipDetailShowUpUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFOCLOSEUP, OnPackageEquipDetailCloseUp);

        EventDispatcher.RemoveEventListener<int>(MenuUIDict.MenuUIEvent.PLAYERUIEQUPMENTGRIDUP, OnEquipmentGridUp);

        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEICONUP, OnPackageIconUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PLAYERICONUP, OnPlayerIconUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.SETTINGSICONUP, OnSettingsIconUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.SKILLICONUP, OnSkillIconUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.SOCIALICONUP, OnSocialIconUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.TONGICONUP, OnTongIconUp);

        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILINSETUP, OnPackageEquipDetailInsetUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSTRENTHUP, OnPackageEquipDetailStrenthUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILUNLOADUP, OnPackageEquipDetailUnLoadUp);


        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.GEMICONUP, OnGemIconUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.NORMALICONUP, OnNormalIconUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.MATERIALICONUP, OnMaterialIconUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEOUTOFBOUNDSMAXUP, OnPackageOutOfBoundsMaxPage);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEOUTOFBOUNDSMINUP, OnPackageOutOfBoundsMinPage);

        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPBUYUP, OnDiamondTipBuyUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPCLOSEUP, OnDiamondTipCloseUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPCOMPOSEUP, OnDiamondTipComposeUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPHISTROYUP, OnDiamondTipHistroyUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPINSETUP, OnDiamondTipInsetUp);

        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PLAYERBASICATTRBUTE, OnPlayerBasicAttrUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PLAYERELEMENTATTRBUTE, OnPlayerElementAttrUp);
        NormalMainUIViewManager.Instance.NORMALMAINUIPLAYERINFOUP -= OnNormalMainUIPlayerInfoUp;

        m_instance = null;
    }

    void OnPlayerElementAttrUp()
    {
        MenuUIViewManager.Instance.ShowElementAttr();
    }

    void OnPlayerBasicAttrUp()
    {
        MenuUIViewManager.Instance.ShowBasicAttr();
    }

    void OnPackageEquipDetailCloseUp()
    {
        Mogo.Util.LoggerHelper.Debug("Close");
        MenuUIViewManager.Instance.ShowPackageDetailInfo(false);
        MenuUIViewManager.Instance.ShowPacakgeCurrentDetailInfo(false);
    }

    private void OnInventoryGridUp(int id)
    {
        Mogo.Util.LoggerHelper.Debug("InventoryGridUp " + id);

        EventDispatcher.TriggerEvent<int>(InventoryManager.ON_ITEMS_SELECTED, id);
    }

    private void OnInventorySortUp()
    {
        Mogo.Util.LoggerHelper.Debug("Sort");
        EventDispatcher.TriggerEvent(InventoryManager.ON_INVENTORY_SORT);
    }

    private void OnInventoryDecomposeUp()
    {
        Mogo.Util.LoggerHelper.Debug("Decompose");
        MogoUIManager.Instance.SwitchDecomposeUI();
        DecomposeManager.Instance.RefreshUI();
    }

    private void OnInventoryEquipmentDetailEquipUp()
    {
        Mogo.Util.LoggerHelper.Debug("InventoryEquipmentDetailEquipUp");
        EventDispatcher.TriggerEvent(InventoryManager.ON_EQUIP);
    }

    private void OnInventoryEquipDetailSailUp()
    {
        Mogo.Util.LoggerHelper.Debug("InventoryEquipDetailSailUp");
    }

    private void OnInventoryEquipDetailShowUpUp()
    {
        Mogo.Util.LoggerHelper.Debug("InventoryEquipDetailShowUpUp");

        //EventDispatcher.TriggerEvent<string>("EquipmentShowupUp", "<info=(1,1,1)>");
        EventDispatcher.TriggerEvent(InventoryManager.ON_EQUIP_SHOW);
    }

    private void OnEquipmentGridUp(int id)
    {
        //Mogo.Util.LoggerHelper.Debug(id);

        EventDispatcher.TriggerEvent<int>(InventoryManager.ON_EQUIP_GRID_UP, id);
        //MenuUIViewManager.Instance.ShowPackageDetailInfo(true);
    }

    public void ShowPackageGridUpSign(int id, bool isShow = true)
    {
        MenuUIViewManager.Instance.ShowPackageGridUpSign(id, isShow);
    }

    public void ShowPackageGridUpSignBL(int id, bool isShow = true)
    {
        MenuUIViewManager.Instance.ShowPackageGridUpSignBL(id, isShow);
    }

    public int GetCurrentPage()
    {
        return MenuUIViewManager.Instance.GetCurrentPage();
    }



    /// <summary>
    /// 更新主角信息
    /// </summary>
    /// <param name="myself"></param>
    public void UpdateRole(EntityMyself myself)
    {
        //概要界面
        string expString = myself.exp + "/" + myself.nextLevelExp;
        string energyString = myself.energy + "/" + myself.maxEnergy;

        SetPlayerName(myself.name);
        SetPlayerLevel(myself.level);

        MenuUIViewManager.Instance.SetPlayerInfoPower(myself.fightForce);

        MenuUIViewManager.Instance.SetPlayerInfoExp(myself.PercentageExp);
        MenuUIViewManager.Instance.SetPlayerInfoEnergy(myself.PercentageEnergy);
        MenuUIViewManager.Instance.SetPlayerInfoExpNum(expString);
        MenuUIViewManager.Instance.SetPlayerInfoEnergyNum(energyString);

        MenuUIViewManager.Instance.SetPlayerInfoJob(LanguageData.dataMap[(int)MogoWorld.thePlayer.vocation].content);

        //MenuUIViewManager.Instance.SetPlayerInfoHealth((int)myself.GetDoubleAttr(BattleAttr.HEALTH));
        //MenuUIViewManager.Instance.SetPlayerInfoDamage((int)myself.GetDoubleAttr(BattleAttr.Attack));
        //MenuUIViewManager.Instance.SetPlayerInfoDefense((int)myself.GetDoubleAttr(BattleAttr.DEFENSE));

        MenuUIViewManager.Instance.SetPlayerInfoHealth(myself.curHp);
        MenuUIViewManager.Instance.SetPlayerInfoAttack(myself.atk);
        MenuUIViewManager.Instance.SetPlayerInfoDefense(myself.def);

        //详细界面
        //MenuUIViewManager.Instance.SetPlayerDetailAttack((int)myself.GetDoubleAttr(BattleAttr.Attack));
        //MenuUIViewManager.Instance.SetPlayerDetailDefense((int)myself.GetDoubleAttr(BattleAttr.DEFENSE));
        //MenuUIViewManager.Instance.SetPlayerDetailHit((int)myself.GetDoubleAttr(BattleAttr.HIT));
        //MenuUIViewManager.Instance.SetPlayerDetailHealth((int)myself.GetDoubleAttr(BattleAttr.HEALTH));
        //MenuUIViewManager.Instance.SetPlayerDetailCrit((int)myself.GetDoubleAttr(BattleAttr.CRIT));
        //MenuUIViewManager.Instance.SetPlayerDetailAntiCrit((int)myself.GetDoubleAttr(BattleAttr.ANTI_CRIT));
        //MenuUIViewManager.Instance.SetPlayerDetailTrueStrike((int)myself.GetDoubleAttr(BattleAttr.TRUE_STRIKE));
        //MenuUIViewManager.Instance.SetPlayerDetailAntiTrueStrike((int)myself.GetDoubleAttr(BattleAttr.TRUE_STRIKE_RESISTANCE));
        //MenuUIViewManager.Instance.SetPlayerDetailAntiDefense((int)myself.GetDoubleAttr(BattleAttr.ANTI_DEFENSE));
        //MenuUIViewManager.Instance.SetPlayerDetailCritExtraAttack((int)myself.GetDoubleAttr(BattleAttr.CRIT_EXTRA_ATTACK));
        //MenuUIViewManager.Instance.SetPlayerDetailPVPAddition((int)myself.GetDoubleAttr(BattleAttr.PVP_ADDITION));
        //MenuUIViewManager.Instance.SetPlayerDetailPVE((int)myself.GetDoubleAttr(BattleAttr.PVP_ANTI));
        //MenuUIViewManager.Instance.SetPlayerDetailCDReduce((int)myself.GetDoubleAttr(BattleAttr.CD_REDUCE));
        //MenuUIViewManager.Instance.SetPlayerDetailSpeedAddRate((int)myself.GetDoubleAttr(BattleAttr.SPEED_ADD_RATE));

        //MenuUIViewManager.Instance.SetPlayerDetailEarthDamage((int)myself.GetDoubleAttr(BattleAttr.EARTH_DAMAGE));
        //MenuUIViewManager.Instance.SetPlayerDetailFireDamage((int)myself.GetDoubleAttr(BattleAttr.FIRE_DAMAGE));
        //MenuUIViewManager.Instance.SetPlayerDetailWaterDamage((int)myself.GetDoubleAttr(BattleAttr.WATER_DAMAGE));
        //MenuUIViewManager.Instance.SetPlayerDetailWindDamage((int)myself.GetDoubleAttr(BattleAttr.AIR_DAMAGE));
        //MenuUIViewManager.Instance.SetPlayerDetailAllElementsDamage((int)myself.GetDoubleAttr(BattleAttr.ALL_ELEMENTS_DAMAGE));

        //MenuUIViewManager.Instance.SetPlayerDetailEarthDefense((int)myself.GetDoubleAttr(BattleAttr.EARTH_DEFENSE));
        //MenuUIViewManager.Instance.SetPlayerDetailFireDefense((int)myself.GetDoubleAttr(BattleAttr.FIRE_DEFENSE));
        //MenuUIViewManager.Instance.SetPlayerDetailWaterDefense((int)myself.GetDoubleAttr(BattleAttr.WATER_DEFENSE));
        //MenuUIViewManager.Instance.SetPlayerDetailWindDefense((int)myself.GetDoubleAttr(BattleAttr.AIR_DEFENSE));
        //MenuUIViewManager.Instance.SetPlayerAllElementsDefense((int)myself.GetDoubleAttr(BattleAttr.ALL_ELEMENTS_DEFENSE));

        MenuUIViewManager.Instance.SetPlayerDetailAttack(myself.atk);
        MenuUIViewManager.Instance.SetPlayerDetailDefense(myself.def);
        MenuUIViewManager.Instance.SetPlayerDetailHit(myself.hit);
        MenuUIViewManager.Instance.SetPlayerDetailHealth(myself.curHp);
        MenuUIViewManager.Instance.SetPlayerDetailCrit(myself.crit);
        MenuUIViewManager.Instance.SetPlayerDetailAntiCrit(myself.antiCrit);
        MenuUIViewManager.Instance.SetPlayerDetailTrueStrike(myself.trueStrike);
        MenuUIViewManager.Instance.SetPlayerDetailAntiTrueStrike(myself.antiTrueStrike);
        MenuUIViewManager.Instance.SetPlayerDetailAntiDefense(myself.antiDefense);
        MenuUIViewManager.Instance.SetPlayerDetailCritExtraAttack(myself.critExtraAttack);
        MenuUIViewManager.Instance.SetPlayerDetailPVPAddition(myself.pvpAddition);
        MenuUIViewManager.Instance.SetPlayerDetailPVE(myself.pvpAnti);
        MenuUIViewManager.Instance.SetPlayerDetailCDReduce(myself.cdReduce);
        MenuUIViewManager.Instance.SetPlayerDetailSpeedAddRate(myself.speedAddRate);

        MenuUIViewManager.Instance.SetPlayerDetailEarthDamage(myself.earthDamage);
        MenuUIViewManager.Instance.SetPlayerDetailFireDamage(myself.fireDamage);
        MenuUIViewManager.Instance.SetPlayerDetailWaterDamage(myself.waterDamage);
        MenuUIViewManager.Instance.SetPlayerDetailWindDamage(myself.airDamage);
        MenuUIViewManager.Instance.SetPlayerDetailAllElementsDamage(myself.allElementsDamage);

        MenuUIViewManager.Instance.SetPlayerDetailEarthDefense(myself.earthDefense);
        MenuUIViewManager.Instance.SetPlayerDetailFireDefense(myself.fireDefense);
        MenuUIViewManager.Instance.SetPlayerDetailWaterDefense(myself.waterDefense);
        MenuUIViewManager.Instance.SetPlayerDetailWindDefense(myself.airDefense);
        MenuUIViewManager.Instance.SetPlayerAllElementsDefense(myself.allElementsDefense);
    }

    protected void SetAttack(uint atk)
    {
        MenuUIViewManager.Instance.SetPlayerInfoAttack(atk);
        MenuUIViewManager.Instance.SetPlayerDetailAttack(atk);
    }

    protected void SetHealth(uint curHp)
    {
        MenuUIViewManager.Instance.SetPlayerInfoHealth(curHp);
        MenuUIViewManager.Instance.SetPlayerDetailHealth(curHp);
    }

    protected void SetDefence(uint def)
    {
        MenuUIViewManager.Instance.SetPlayerInfoDefense(def);
        MenuUIViewManager.Instance.SetPlayerDetailDefense(def);
    }

    protected void SetPlayerEnergy(float theEnergy)
    {
        string energyString = MogoWorld.thePlayer.energy + "/" + MogoWorld.thePlayer.maxEnergy;

        Mogo.Util.LoggerHelper.Debug(energyString + " expString");

        MenuUIViewManager.Instance.SetPlayerInfoEnergy((float)MogoWorld.thePlayer.PercentageEnergy);
        MenuUIViewManager.Instance.SetPlayerInfoEnergyNum(energyString);
    }

    protected void SetPlayerExp(float theExp)
    {
        string expString = MogoWorld.thePlayer.exp + "/" + MogoWorld.thePlayer.nextLevelExp;

        Mogo.Util.LoggerHelper.Debug(expString + " expString");

        MenuUIViewManager.Instance.SetPlayerInfoExp((float)MogoWorld.thePlayer.PercentageExp);
        MenuUIViewManager.Instance.SetPlayerInfoExpNum(expString);
    }

    protected void SetPlayerLevel(byte level)
    {
        //MenuUIViewManager.Instance.SetPlayerInfoLevel("LV " + level);
        MenuUIViewManager.Instance.SetPlayerInfoNameAndLevel(MogoWorld.thePlayer.name, "  LV " + MogoWorld.thePlayer.level);
    }

    protected void SetPlayerName(string name)
    {
        //MenuUIViewManager.Instance.SetPlayerInfoName(name);
        MenuUIViewManager.Instance.SetPlayerInfoNameAndLevel(MogoWorld.thePlayer.name, "  LV " + MogoWorld.thePlayer.level);
    }
}