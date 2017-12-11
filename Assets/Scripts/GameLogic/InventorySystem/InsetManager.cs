/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：InsetManager
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-4-16
// 模块描述：宝石镶嵌系统
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.GameData;
using Mogo.Util;
using Mogo.Game;

public class InsetManager
{
    public const string ON_EQUIP_SELECT = "InsetManager.ON_EQUIP_SELECT";
    public const string ON_JEWEL_SLOT_SELECT = "InsetManager.ON_JEWEL_SLOT_SELECT";
    public const string ON_INSET_JEWEL = "InsetManager.ON_INSET_JEWEL";
    public const string DISASSEMBLE_JEWEL = "InsetManager.DISASSEMBLE_JEWEL"; //分解格子物品
    public const string ON_JEWEL_UPGRADE = "InsetManager.ON_JEWEL_UPGRADE";
    public const string ON_JEWEL_SELECT = "InsetManager.ON_JEWEL_SELECT"; //选择列表显示
    public const string ON_INSET_SHOW = "InsetManager.ON_INSETUI_SHOW";
    public const string ON_JEWEL_DRAG = "InsetManager.ON_JEWEL_DRAG";
    public const string ON_INSET_JEWEL_IN_BAG = "InsetManager.ON_INSET_JEWEL_IN_BAG";
    public const string ON_PUT_ON = "InsetManager.ON_PUT_ON";
    public const string ON_BUY = "InsetManager.ON_BUY";

    public const int ERR_JEWEL_SUCCEED = 0;               // --成功
    public const int ERR_JEWEL_PARA = 1;              // --参数错误
    public const int ERR_JEWEL_CONFIG = 2;               // --配置错误
    public const int ERR_JEWEL_NOT_ENOUGH_DIAMOND = 3;
    public const int ERR_JEWEL_NOT_ENOUGH_MATERIAL = 4;
    public const int ERR_JEWEL_DEL_FAILED = 5;
    public const int ERR_JEWEL_CAN_NOT_INLAY = 6;               // --宝石和装备不匹配
    public const int ERR_JEWEL_LEVEL_ALREADY_MAX = 7;
    public const int ERR_JEWEL_EQUI_NOT_EXISTS = 8;
    public const int ERR_JEWEL_NOT_EXISTS = 9;
    public const int ERR_JEWEL_NO_EMPTY_GRID = 10;               // --背包没有足够的空格
    public const int ERR_JEWEL_SLOT_NOT_EXISTS = 11;
    public const int ERR_JEWEL_SLOT_FULL_OR_NOT_MATCH = 12; //--装备插槽满了或者与镶嵌宝石不匹配
    public const int ERR_JEWEL_EQUI_SLOT_NOT_EXISTS = 13;                //--装备的插槽不存在
    public const int ERR_JEWEL_CAN_NOT_OUTLAY = 14;
    public const int ERR_JEWEL_SLOT_NO_JEWEL = 15;                //--装备的插槽上没有宝石
    public const int ERR_JEWEL_NUM_TOO_MUCH = 16;                //--卖出宝石大于当前格子的数量
    public const int ERR_JEWEL_CAN_NOT_SELL = 17;                //--该宝石不能出售

    //private int m_currentIdx = 0;

    InventoryManager m_inventoryManager;
    List<ItemJewel> m_itemsJewelSorted = new List<ItemJewel>();
    List<ItemJewel> m_currentJewelList = new List<ItemJewel>();
    ItemJewel m_currentJewel;//当前选中的宝石（非当前选到的装备插槽上的宝石）
    ItemJewelData m_currentJewelData;//当前选到的装备插槽上的宝石
    ItemEquipment m_currentEquipment;
    public static InsetManager Instance;
    int m_versionID;

    //跟背包中宝石数据同步，用与合成宝石界面逻辑,<subtype,<level,count>>

    int m_iCurrentSlot = -1;//1~11
    public int CurrentSlot
    {
        get
        {
            return m_iCurrentSlot;
        }
        set
        {
            if (InsetUIViewManager.Instance != null)
            {
                int newIndex = 0;
                int oldIndex = 0;
                if (m_slotToIndexDic.ContainsKey(value))
                    newIndex = m_slotToIndexDic[value];
                if (m_slotToIndexDic.ContainsKey(m_iCurrentSlot))
                    oldIndex = m_slotToIndexDic[m_iCurrentSlot];
                InsetUIViewManager.Instance.HandleInsetTabChange(oldIndex, newIndex);
            }
            m_iCurrentSlot = value;
        }
    }
    private int m_currentJewelSlot = -1;

    private Dictionary<int, int> m_slotToIndexDic = new Dictionary<int, int>();
    private List<int> m_indexToSlotList = new List<int>();

    public class JewelHoleViewData
    {
        public string bgDown = IconData.none;
        public string bgUp = IconData.none;
        public bool canUpgrade = false;
        public bool canRemove = false;
        public string fg = IconData.none;
        public int fgColor = 0;
        public string tipDesc = "";
        public bool isShowTip = false;
        public int holeType = 0;
    }

    public InsetManager(InventoryManager inventoryManager)
    {
        Instance = this;
        m_inventoryManager = inventoryManager;
        AddListener();
        m_versionID = inventoryManager.m_versionId;
    }

    public int GetIndexByJewelId(int id)
    {
        for (int i = 0; i < m_currentJewelList.Count; i++)
        {
            if (m_currentJewelList[i].templateId == id)
            {
                return i;
            }
        }
        return 0;
    }

    private void AddListener()
    {
        EventDispatcher.AddEventListener<int>(ON_EQUIP_SELECT, OnInsetEquipGridUp);
        EventDispatcher.AddEventListener<int>(ON_JEWEL_UPGRADE, UpgradeJewel);
        EventDispatcher.AddEventListener<int>(ON_JEWEL_SELECT, OnJewelSelect);
        EventDispatcher.AddEventListener<int>(DISASSEMBLE_JEWEL, DisassembleJewel);
        EventDispatcher.AddEventListener<int>(ON_INSET_JEWEL, InsetJewel);
        EventDispatcher.AddEventListener(ON_INSET_SHOW, OnInsetShow);
        EventDispatcher.AddEventListener<int>(ON_JEWEL_DRAG, OnJewelDrag);
        EventDispatcher.AddEventListener(ON_PUT_ON, OnPuton);
        EventDispatcher.AddEventListener(ON_BUY, OnBuy);

        EventDispatcher.AddEventListener<int>(ON_JEWEL_SLOT_SELECT, OnJewelSlotSelect);
    }

    private void OnBuy()
    {
        Action act = () => { SwitchToInsetUI(CurrentSlot); };
        EventDispatcher.TriggerEvent(MarketEvent.OpenWithJewel, act);
    }

    private void OnPuton()
    {
        MogoWorld.thePlayer.RpcCall("ExchangeEquipment", m_currentEquipment.id, (byte)m_currentEquipment.gridIndex);
    }

    public void OnJewelSlotSelect(int index)
    {
        Mogo.Util.LoggerHelper.Debug("OnJewelSlotSelect:" + index);
        m_currentJewelSlot = index;
        RefreshInsetRightUI();
    }

    private void OnSwitchInsetUI()
    {
        EquipTipManager.Instance.CloseEquipTip();
        Mogo.Util.LoggerHelper.Debug("OnSwitchInsetUI");
        //选最近那个

        CurrentSlot  = 11;
        //if (!m_inventoryManager.EquipOnDic.ContainsKey(CurrentSlot))
        //{
        //    for (int i = 0; i < 11; i++)
        //    {
        //        int slot;
        //        if (i == 0) slot = 11;
        //        else slot = i;
        //        if (m_inventoryManager.EquipOnDic.ContainsKey(slot))
        //        {
        //            InsetUIViewManager.Instance.SetCurrentDownGrid(i);
        //            CurrentSlot = slot;
        //            break;
        //        }
        //    }
        //}
        RefreshUI();
    }
    public void InsetJewelInBag(int index)
    {
        Mogo.Util.LoggerHelper.Debug("InsetJewelInBag");
        MogoUIManager.Instance.SwitchInsetUI(OnSwitchInsetUI);

        //MogoWorld.thePlayer.RpcCall("JewelInlayReq", index);
    }

    private void OnJewelDrag(int index)
    {
        if (index >= m_currentJewelList.Count) return;
        m_currentJewel = m_currentJewelList[index];
    }

    public void RemoveListener()
    {
        EventDispatcher.RemoveEventListener<int>(ON_EQUIP_SELECT, OnInsetEquipGridUp);
        EventDispatcher.RemoveEventListener<int>(ON_JEWEL_UPGRADE, UpgradeJewel);
        EventDispatcher.RemoveEventListener<int>(ON_JEWEL_SELECT, OnJewelSelect);
        EventDispatcher.RemoveEventListener<int>(DISASSEMBLE_JEWEL, DisassembleJewel);
        EventDispatcher.RemoveEventListener<int>(ON_INSET_JEWEL, InsetJewel);
        EventDispatcher.RemoveEventListener(ON_INSET_SHOW, OnInsetShow);
        EventDispatcher.RemoveEventListener<int>(ON_JEWEL_DRAG, OnJewelDrag);
        EventDispatcher.RemoveEventListener(ON_PUT_ON, OnPuton);
        EventDispatcher.RemoveEventListener(ON_BUY, OnBuy);

        EventDispatcher.RemoveEventListener<int>(ON_JEWEL_SLOT_SELECT, OnJewelSlotSelect);
    }

    private void OnInsetShow()
    {
        Mogo.Util.LoggerHelper.Debug("OnInsetShow");
        CurrentSlot = -1;
        RefreshUI();
    }

    private void OnInsetEquipGridUp(int index)
    {
        LoggerHelper.Debug("OnInsetEquipGridUp:" + (index + 1));
        //if (index == 0) CurrentSlot = 11;
        //else CurrentSlot = index;
        CurrentSlot = m_indexToSlotList[index];
        //InsetUIViewManager.Instance.SetCurrentDownGrid(index);
        m_currentJewelSlot = -1;
        RefreshInsetRightUI();

    }

    private void UpgradeJewel(int idx)
    {
        LoggerHelper.Debug("UpgradeJewel");
        int jewelId = (m_inventoryManager.EquipOnDic[CurrentSlot].jewelSlots)[idx];
        m_currentJewelData = ItemJewelData.dataMap[jewelId];
        MogoWorld.thePlayer.RpcCall("JewelCombineInEquiReq", CurrentSlot, idx);
    }

    private void OnJewelSelect(int idx)
    {
        LoggerHelper.Debug("OnJewelSelect:" + idx);
        if (idx >= m_currentJewelList.Count)
            return;
        m_currentJewel = m_currentJewelList[idx];


        ShowJewelTip(m_currentJewel.templateId);
    }

    public void ShowJewelTip(int _jewelId)
    {

        List<ButtonInfo> btnList = new List<ButtonInfo>();
        int textId = 905;
        ButtonInfo btn = new ButtonInfo() { action = OnInsetJewel, text = LanguageData.GetContent(textId), id = textId };
        btnList.Add(btn);
        InventoryManager.Instance.ShowJewelTip(_jewelId, btnList);
        //ItemJewelData jewel = ItemJewelData.dataMap[_jewelId];
        //InsetUIViewManager view = InsetUIViewManager.Instance;

        //view.SetDiamondTipLevel(jewel.level + "");
        //view.SetDiamondTipType(jewel.typeName);
        //view.SetDiamondTipDesc(jewel.effectDescriptionStr);
        //view.SetDiamondTipName(jewel.Name);
        //view.SetDiamondTipIcon(jewel.Icon, jewel.color);
        //view.SetDiamondTipIconBG(IconData.GetIconByQuality(jewel.quality));
        //view.ShowInsetDialogDiamondTip(true);

    }

    private void DisassembleJewel(int idx)
    {
        LoggerHelper.Debug("DisassembleJewel:" + idx);
        int jewelId = (m_inventoryManager.EquipOnDic[CurrentSlot].jewelSlots)[idx];
        m_currentJewelData = ItemJewelData.dataMap[jewelId];
        MogoWorld.thePlayer.RpcCall("JewelOutlayReq", CurrentSlot, idx);
    }

    private void OnInsetJewel()
    {
        //无选定那个槽
        InsetJewel(-1);
    }

    private void InsetJewel(int index)
    {
        if (!CanJewelInset(index)) return;

        if (index < 0)
        {
            if (m_currentJewelSlot >= 0)
            {
                Mogo.Util.LoggerHelper.Debug("JewelInlayIntoSlotReq");
                MogoWorld.thePlayer.RpcCall("JewelInlayIntoSlotReq", (byte)CurrentSlot, (byte)m_currentJewelSlot, (byte)m_currentJewel.gridIndex);
            }
            else
            {
                Mogo.Util.LoggerHelper.Debug("JewelInlayReq");
                MogoWorld.thePlayer.RpcCall("JewelInlayReq", (byte)CurrentSlot, (byte)m_currentJewel.gridIndex);
            }

        }
        else
        {
            MogoWorld.thePlayer.RpcCall("JewelInlayIntoSlotReq", (byte)CurrentSlot, (byte)index, (byte)m_currentJewel.gridIndex);
        }

    }

    private bool CanJewelInset(int index)
    {
        return true;
        //为消除警告而注释以下代码
        //if (!m_inventoryManager.EquipOnDic.ContainsKey(CurrentSlot))
        //{
        //    ShowInfoByErrorId(ERR_JEWEL_EQUI_NOT_EXISTS);
        //    return false; ;
        //}
        //Mogo.Util.LoggerHelper.Debug("m_currentSlot:" + CurrentSlot);
        //ItemEquipment equipment = m_inventoryManager.EquipOnDic[CurrentSlot];
        //if (equipment.jewelSlotsType == null || equipment.jewelSlotsType.Count <= 0)
        //{
        //    ShowInfoByErrorId(ERR_JEWEL_EQUI_SLOT_NOT_EXISTS);
        //    return false;
        //}
        //bool canInset = false;
        //if (index < 0)
        //{
        //    for (int i = 0; i < equipment.jewelSlots.Count; i++)
        //    {
        //        if (canInset) break;
        //        if (equipment.jewelSlots[i] != -1) continue;
        //        if (i < equipment.jewelSlotsType.Count)
        //        {
        //            foreach (int type in m_currentJewel.slotType)
        //            {
        //                if (type != equipment.jewelSlotsType[i]) continue;
        //                canInset = true;
        //                break;
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    if (index < equipment.jewelSlots.Count)
        //    {
        //        if (equipment.jewelSlots[index] == -1)
        //        {
        //            foreach (int type in m_currentJewel.slotType)
        //            {
        //                if (type != equipment.jewelSlotsType[index]) continue;
        //                canInset = true;
        //                break;
        //            }
        //        }
        //    }
        //}

        //if (!canInset)
        //{
        //    ShowInfoByErrorId(ERR_JEWEL_SLOT_FULL_OR_NOT_MATCH);
        //    return false;
        //}

        //return true;
    }

    /// <summary>
    /// 根据宝石背包数据得到一个排序好，合并好的宝石列表
    /// </summary>
    private void UpdateTheSortedJewel()
    {
        m_itemsJewelSorted.Clear();
        foreach (ItemParent item in m_inventoryManager.JewelInBag.Values)
        {
            ItemJewel jewel = (ItemJewel)item;
            m_itemsJewelSorted.Add(jewel);
        }
        m_itemsJewelSorted.Sort(delegate(ItemJewel a, ItemJewel b)
        {

            if (a.level > b.level) return -1;
            else if (a.level < b.level) return 1;
            else
            {
                if (a.subtype < b.subtype) return -1;
                else if (a.subtype > b.subtype) return 1;
                else
                {
                    if (a.stack > b.stack) return -1;
                    else return 1;
                }
            }
        });
    }

    private bool CanJewelUpgrade(int jewelTemplateId)
    {
        if (jewelTemplateId == -1) return false;
        ItemJewelData jewel = ItemJewelData.dataMap[jewelTemplateId];
        int level = jewel.level;

        PrivilegeData privilegeData = PrivilegeData.dataMap[MogoWorld.thePlayer.VipLevel];
        if (level + 1 > privilegeData.jewelSynthesisMaxLevel) return false;

        int subtype = jewel.subtype;
        int count = 0;
        if (m_itemsJewelSorted == null)
        {
            return false;
        }
        for (int i = 0; i < m_itemsJewelSorted.Count; i++)
        {
            ItemJewel item = m_itemsJewelSorted[i];
            if (item.level < level) return false;
            if (item.subtype != subtype) continue;
            if (item.level > level) continue;
            count += item.stack;
            if (count >= 2) return true;
        }
        return false;
    }

    public void RefreshUI()
    {
        Mogo.Util.LoggerHelper.Debug("insetManager ui refresh!");
        //if (m_versionID >= m_inventoryManager.m_versionId) return;
        if (InsetUIViewManager.Instance == null) return;

        if (CurrentSlot == -1)
        {
            CurrentSlot = 11;
            //InsetUIViewManager.Instance.SetCurrentDownGrid(0);
        }
        UpdateTheSortedJewel();

        UpdateSlotToIndexDic();
        //else
        //{
            //InsetUIViewManager.Instance.SetCurrentDownGrid(CurrentSlot % 11);
        //}
       

        RefreshInsetLeftUI();
        RefreshInsetRightUI();
        InsetUIViewManager.Instance.SetCurrentDownGrid(m_slotToIndexDic[CurrentSlot]);
        //m_versionID = m_inventoryManager.m_versionId;
    }

    private void UpdateSlotToIndexDic()
    {
        m_slotToIndexDic.Clear();
        m_indexToSlotList.Clear();
        m_indexToSlotList.Add(11);
        m_slotToIndexDic[11] = 0;
        int index = 1;
        for (int slot = 1; slot <= 10; slot++)
        {
            if (!InventoryManager.Instance.EquipOnDic.ContainsKey(slot)) continue;
            m_indexToSlotList.Add(slot);
            m_slotToIndexDic[slot] = index;
            index++;
        }
        for (int slot = 1; slot <= 10; slot++)
        {
            if (InventoryManager.Instance.EquipOnDic.ContainsKey(slot)) continue;
            m_indexToSlotList.Add(slot);
            m_slotToIndexDic[slot] = index;
            index++;
        }
    }

    private void RefreshInsetLeftUI()
    {
        for (int i = 0; i < m_indexToSlotList.Count; i++)
        {
            int slot = m_indexToSlotList[i];
            InsetUIViewManager.Instance.SetEquipmentName(i, EquipSlotName.names[slot > 9 ? slot - 2 : slot - 1]);
            InsetUIViewManager.Instance.ShowEquipmentUpSign(i, false);

            if (m_inventoryManager.EquipOnDic.ContainsKey(slot))
            {
                InsetUIViewManager.Instance.SetEquipmentIcon(i, m_inventoryManager.EquipOnDic[slot].icon);
                InsetUIViewManager.Instance.SetEquipmentIconBG(i, IconData.GetIconByQuality(m_inventoryManager.EquipOnDic[slot].quality));
                bool hasJewelCanUpgrade = false;
                ItemEquipment equip = m_inventoryManager.EquipOnDic[slot];
                for (int j = 0; j < equip.jewelSlots.Count; j++)
                {
                    if (CanJewelUpgrade(equip.jewelSlots[j]))
                    {
                        hasJewelCanUpgrade = true;
                        break;
                    }
                }
                InsetUIViewManager.Instance.ShowEquipmentUpSign(i, hasJewelCanUpgrade);
            }
            else
            {
                //InsetUIViewManager.Instance.SetEquipmentIcon(i, IconData.blankBox);
                InsetUIViewManager.Instance.SetEquipmentIcon(i, EquipSlotIcon.icons[slot], 10);
                InsetUIViewManager.Instance.SetEquipmentIconBG(i, IconData.blankBox);
            }

        }
        //for (int i = 0; i < InventoryManager.SLOT_NUM; i++)
        //{
        //if (i == 0)
        //{
        //    InsetUIViewManager.Instance.SetEquipmentName(i, EquipSlotName.names[9]);
        //    InsetUIViewManager.Instance.SetEquipmentTextImg(i, EquipSlotTextIcon.icons[11]);
        //}
        //else
        //{
        //    InsetUIViewManager.Instance.SetEquipmentName(i, EquipSlotName.names[(i - 1) > 8 ? (i - 2) : (i - 1)]);
        //    InsetUIViewManager.Instance.SetEquipmentTextImg(i, EquipSlotTextIcon.icons[i]);
        //}

        //InsetUIViewManager.Instance.ShowEquipmentUpSign(i, false);

        //非武器，因为武器的提前打乱了slot与显示顺序的关系
        //if (i != 0)
        //{
        //    if (m_inventoryManager.EquipOnDic.ContainsKey(i))
        //    {
        //        InsetUIViewManager.Instance.SetEquipmentIcon(i, m_inventoryManager.EquipOnDic[i].icon);
        //        InsetUIViewManager.Instance.SetEquipmentIconBG(i, IconData.GetIconByQuality(m_inventoryManager.EquipOnDic[i].quality));
        //        bool hasJewelCanUpgrade = false;
        //        ItemEquipment equip = m_inventoryManager.EquipOnDic[i];
        //        for (int j = 0; j < equip.jewelSlots.Count; j++)
        //        {
        //            if (CanJewelUpgrade(equip.jewelSlots[j]))
        //            {
        //                hasJewelCanUpgrade = true;
        //                break;
        //            }
        //        }
        //        InsetUIViewManager.Instance.ShowEquipmentUpSign(i, hasJewelCanUpgrade);
        //    }
        //    else
        //    {
        //        //InsetUIViewManager.Instance.SetEquipmentIcon(i, IconData.blankBox);
        //        InsetUIViewManager.Instance.SetEquipmentIcon(i, EquipSlotIcon.icons[i], 10);
        //        InsetUIViewManager.Instance.SetEquipmentIconBG(i, IconData.blankBox);
        //    }
        //}
        //else//武器
        //{
        //    if (m_inventoryManager.EquipOnDic.ContainsKey(11))
        //    {
        //        InsetUIViewManager.Instance.SetEquipmentIcon(0, m_inventoryManager.EquipOnDic[11].icon);
        //        InsetUIViewManager.Instance.SetEquipmentIconBG(i, IconData.GetIconByQuality(m_inventoryManager.EquipOnDic[11].quality));
        //        bool hasJewelCanUpgrade = false;
        //        ItemEquipment equip = m_inventoryManager.EquipOnDic[11];
        //        for (int j = 0; j < equip.jewelSlots.Count; j++)
        //        {
        //            if (CanJewelUpgrade(equip.jewelSlots[j]))
        //            {
        //                hasJewelCanUpgrade = true;
        //                break;
        //            }
        //        }
        //        InsetUIViewManager.Instance.ShowEquipmentUpSign(0, hasJewelCanUpgrade);
        //    }
        //    else
        //    {
        //        //InsetUIViewManager.Instance.SetEquipmentIcon(i, IconData.blankBox);
        //        InsetUIViewManager.Instance.SetEquipmentIcon(0, EquipSlotIcon.icons[11], 10);
        //        InsetUIViewManager.Instance.SetEquipmentIconBG(i, IconData.blankBox);
        //    }
        //    }

        //}
    }

    private void RefreshInsetRightUI()
    {
        if (m_inventoryManager.EquipOnDic.ContainsKey(CurrentSlot))
        {
            InsetUIViewManager.Instance.ShowInsetDialog1(true);
            InsetUIViewManager.Instance.ShowInsetDialog2(false);

            //有装备时界面
            RefreshDialog1UI();
        }
        else
        {
            InsetUIViewManager.Instance.ShowInsetDialog1(false);
            InsetUIViewManager.Instance.ShowInsetDialog2(true, true);
            //无装备时界面
            RefreshDialog2UI();
        }
    }

    private void RefreshDialog2UI()
    {
        InsetUIViewManager view = InsetUIViewManager.Instance;
        ItemEquipment equip;
        if ((equip = m_inventoryManager.GetRecommendEquipBySlot(CurrentSlot)) != null)
        {
            view.ShowInsetDialog2EquipDetail(true);
            view.SetDialog2Title(LanguageData.GetContent(447));
            view.SetDialog2EquipImage(equip.templateId);
            view.SetDialog2EquipName(equip.name);
            view.SetDialog2EquipLevelNeed(LanguageData.GetContent(911, equip.levelNeed));
            m_currentEquipment = equip;
        }
        else
        {
            view.ShowInsetDialog2EquipDetail(false);
            view.SetDialog2Title(LanguageData.GetContent(448));
            Mogo.Util.LoggerHelper.Debug("m_currentSlot" + CurrentSlot);
            view.SetDialog2EquipImageFg(EquipSlotIcon.icons[CurrentSlot]);
        }
    }

    private void RefreshDialog1UI()
    {
        Mogo.Util.LoggerHelper.Debug("RefreshDialog1UI");
        //更新装备及其已插宝石情况（界面右上）
        InsetUIViewManager view = InsetUIViewManager.Instance;

        //清空
        //for (int i = 0; i < 4; i++)
        //{
        //view.ShowInsetHoleUnLoadSign(false, i);
        //view.ShowInsetHoleUpdateSign(false, i);
        //view.SetInsetHoleImage(IconData.locked, i);
        //view.SetInsetHoleBGUpImg(IconData.locked, i);
        //view.SetInsetHoleBGDownImg(IconData.locked, i);
        //view.ShowInsetHoleToolTip(i, "", false);

        view.ResetJewelHole();


        //}

        view.SetInsetEquipmentImage(EquipSlotIcon.icons[CurrentSlot]);
        view.SetInsetEquipmentImageBG(IconData.blankBox);
        if (m_inventoryManager.EquipOnDic.ContainsKey(CurrentSlot))
        {
            ItemEquipment item = m_inventoryManager.EquipOnDic[CurrentSlot];
            view.SetInsetEquipmentImage(item.icon);
            view.SetInsetEquipmentImageBG(IconData.GetIconByQuality(item.quality));

            List<JewelHoleViewData> dataList = new List<JewelHoleViewData>();
            if (item.jewelSlotsType != null)
            {
                //Mogo.Util.LoggerHelper.Debug("item.id:" + item.templateId);
                for (int i = 0; i < item.jewelSlotsType.Count; i++)
                {
                    if (item.jewelSlotsType[i] <= 0) continue;
                    //Mogo.Util.LoggerHelper.Debug("item.jewelSlotsType[i]:" + item.jewelSlotsType[i]);
                    //view.SetInsetHoleImage(IconData.none, i);
                    //view.SetInsetHoleBGUpImg(IconData.GetJewelSlotIconUpByType(item.jewelSlotsType[i]), i);
                    //view.SetInsetHoleBGDownImg(IconData.GetJewelSlotIconDownByType(item.jewelSlotsType[i]), i);

                    JewelHoleViewData viewData = new JewelHoleViewData()
                    {
                        bgDown = IconData.JewelSlotSelectedIcon,//GetJewelSlotIconDownByType(item.jewelSlotsType[i]),
                        bgUp = IconData.GetJewelSlotIconUpByType(item.jewelSlotsType[i]),
                        holeType = item.jewelSlotsType[i],
                        fg = IconData.none,
                        canRemove = false,
                        canUpgrade = false,
                        fgColor = 0,
                        tipDesc = "",
                    };

                    dataList.Add(viewData);
                }
            }
            //Mogo.Util.LoggerHelper.Debug("m_currentJewelSlot:" + m_currentJewelSlot);
            if (item.jewelSlots != null)
            {
                for (int i = 0; i < item.jewelSlots.Count; i++)
                {
                    int jewelId = item.jewelSlots[i];
                    if (jewelId == -1)
                    {
                        //view.SetInsetHoleImage(IconData.none, i);
                        continue;
                    }
                    ItemJewelData jewel = ItemJewelData.dataMap[jewelId];
                    //view.SetInsetHoleImage(jewel.Icon, i, jewel.color);

                    dataList[i].fg = jewel.Icon;
                    dataList[i].fgColor = jewel.color;

                    if (m_currentJewelSlot == i)
                    {
                        Mogo.Util.LoggerHelper.Debug("view.SetJewelSlotCurrentDown(i);" + i);

                        dataList[i].canRemove = true;
                        dataList[i].tipDesc = jewel.effectDescriptionStr;
                        dataList[i].isShowTip = true;
                        //view.ShowInsetHoleUnLoadSign(true, i);
                        //view.ShowInsetHoleToolTip(i, jewel.effectDescriptionStr, true);
                    }

                    if (CanJewelUpgrade(jewel.id))
                    {
                        dataList[i].canUpgrade = true;
                        //view.ShowInsetHoleUpdateSign(true, i);
                    }
                }
            }

            view.SetJewelHoleList(dataList);

        }
        //else
        //{
        //    view.SetInsetEquipmentImage(IconData.none);
        //    for (int i = 0; i < 4; i++)
        //    {
        //        view.SetInsetHoleImage(IconData.none, i);
        //        view.ShowInsetHoleUnLoadSign(false, i);
        //        view.ShowInsetHoleUpdateSign(false, i);
        //    }
        //}

        if (m_currentJewelSlot < 0) view.SetAllJewelSlotUp();


        //更新可用宝石栏（界面右下）
        int m = 0;
        int n = 0;

        //选中宝石插槽时
        if (m_inventoryManager.EquipOnDic.ContainsKey(CurrentSlot)
            && m_currentJewelSlot != -1
            )
        {
            ItemEquipment item = m_inventoryManager.EquipOnDic[CurrentSlot];

            //装备上真的有该插槽时
            if (item.jewelSlotsType != null
                && m_currentJewelSlot < item.jewelSlotsType.Count)
            {
                view.SetJewelListTitle(LanguageData.dataMap[176].content);
                m_currentJewelList.Clear();
                for (; m < m_itemsJewelSorted.Count; m++)
                {
                    if (!m_itemsJewelSorted[m].slotType.Contains(item.jewelSlotsType[m_currentJewelSlot]))
                        continue;

                    //Debug.LogError("m_itemsJewelSorted[m].icon:" + m_itemsJewelSorted[m].icon);
                    //Debug.LogError("m_itemsJewelSorted[m].color:" + m_itemsJewelSorted[m].color);
                    view.SetInsetPackageItemImage(n, m_itemsJewelSorted[m].templateId);
                    view.SetInsetPackageItemNum(n, m_itemsJewelSorted[m].stack);
                    m_currentJewelList.Add(m_itemsJewelSorted[m]);
                    n++;
                }
                m = n;
            }
            else
            {
                view.SetJewelListTitle(LanguageData.dataMap[446].content);
                for (; m < m_itemsJewelSorted.Count; m++)
                {
                    for (int type = 0; type < item.jewelSlotsType.Count; type++)
                    {
                        if (m_itemsJewelSorted[m].slotType.Contains(item.jewelSlotsType[type]))
                        {
                            view.SetInsetPackageItemImage(n, m_itemsJewelSorted[m].templateId);
                            view.SetInsetPackageItemNum(n, m_itemsJewelSorted[m].stack);
                            m_currentJewelList.Add(m_itemsJewelSorted[m]);
                            n++;
                            break;
                        }
                    }
                }
            }
        }
        else//未选中宝石插槽时
        {
            m_currentJewelList.Clear();
            view.SetJewelListTitle(LanguageData.dataMap[446].content);

            ItemEquipment item = null;
            if (m_inventoryManager.EquipOnDic.ContainsKey(CurrentSlot))
                item = m_inventoryManager.EquipOnDic[CurrentSlot];
            if (item != null)
            {
                for (; m < m_itemsJewelSorted.Count; m++)
                {
                    //Debug.LogError(m + ".color:" + m_itemsJewelSorted[m].color);
                    for (int type = 0; type < item.jewelSlotsType.Count; type++)
                    {
                        if (m_itemsJewelSorted[m].slotType.Contains(item.jewelSlotsType[type]))
                        {
                            view.SetInsetPackageItemImage(n, m_itemsJewelSorted[m].templateId);
                            view.SetInsetPackageItemNum(n, m_itemsJewelSorted[m].stack);
                            m_currentJewelList.Add(m_itemsJewelSorted[m]);
                            n++;
                            break;
                        }
                    }
                }
                m = n;
            }
        }

        if (m == 0)
        {
            view.SetJewelListTitle(LanguageData.dataMap[445].content);
        }

        for (; m < InventoryManager.JEWEL_SORTED_GRID_NUM; m++)
        {
            view.SetInsetPackageItemImage(m, -1);
            view.SetInsetPackageItemNum(m, 0);
        }
    }

    public void JewelInlayIntoEqiResp(byte errorId)
    {
        Mogo.Util.LoggerHelper.Debug("JewelInlayIntoEqiResp:" + errorId);

        switch (errorId)
        {
            case 0:
                //string msg = LanguageData.dataMap[443].Format(m_currentJewel.name, m_currentJewel.effectDescription);
                //MogoMsgBox.Instance.ShowFloatingText(msg); 

                InsetUIViewManager.Instance.ShowInsetSucessSign(true);

                break;
            default:
                ShowInfoByErrorId(errorId, ItemJewelData.dataMap[m_currentJewel.templateId]);
                break;
        }
        EquipTipManager.Instance.CloseEquipTip();
        RefreshUI();
        //InsetUIViewManager.Instance.ShowInsetDialogDiamondTip(false);
    }

    public void JewelOutlayResp(byte errorId)
    {
        Mogo.Util.LoggerHelper.Debug("JewelOutlayResp:" + errorId);

        switch (errorId)
        {
            case 0:
                //string msg = LanguageData.dataMap[442].Format(m_currentJewelData.Name);
                //MogoMsgBox.Instance.ShowFloatingText(msg);
                break;
            default:
                ShowInfoByErrorId(errorId);
                break;
        }

        EquipTipManager.Instance.CloseEquipTip();
        RefreshUI();
        //InsetUIViewManager.Instance.ShowInsetDialogDiamondTip(false);
    }

    static public void ShowInfoByErrorId(byte errorId, ItemJewelData jewel = null)
    {
        int index = 421;
        string msg = "";
        Mogo.Util.LoggerHelper.Debug("haha");
        switch (errorId)
        {
            case 0:
                //Mogo.Util.LoggerHelper.Debug("haha");
                //msg = LanguageData.dataMap[index].Format(jewel.Name);
                //Mogo.Util.LoggerHelper.Debug("haha:" + msg);
                //MogoMsgBox.Instance.ShowFloatingText(msg);
                //Mogo.Util.LoggerHelper.Debug("haha");
                InsetUIViewManager.Instance.ShowInsetSucessSign(true);
                //MogoGlobleUIManager.Instance.ShowComposeSucessSign(true);
                EquipTipManager.Instance.CloseEquipTip();
                //MogoFXManager.Instance.AttachParticleAnim("fx_ui_gem.prefab", 2000,
                //        GameObject.Find("Camera").transform.position, GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0], 0);    

                return;
            case 1:
                msg = LanguageData.dataMap[index + 1].content;
                break;
            case 2:
                msg = LanguageData.dataMap[index + 2].content;
                break;
            case 3:
                msg = LanguageData.dataMap[index + 3].content;
                break;
            case 4:
                msg = LanguageData.dataMap[index + 4].Format(jewel.Name);
                break;
            case 5:
                msg = LanguageData.dataMap[index + 5].content;
                break;
            case 6:
                msg = LanguageData.dataMap[index + 6].Format(jewel.Name);
                break;
            case 7:
                msg = LanguageData.dataMap[index + 7].content;
                break;
            case 8:
                msg = LanguageData.dataMap[index + 8].content;
                break;
            case 9:
                msg = LanguageData.dataMap[index + 9].content;
                break;
            case 10:
                msg = LanguageData.dataMap[index + 10].content;
                break;
            case 11:
                msg = LanguageData.dataMap[index + 11].content;
                break;
            case 12:
                msg = LanguageData.dataMap[index + 12].content;
                break;
            case 13:
                msg = LanguageData.dataMap[index + 13].content;
                break;
            case 14:
                msg = LanguageData.dataMap[index + 14].content;
                break;
            case 15:
                msg = LanguageData.dataMap[index + 15].content;
                break;
            case 16:
                msg = LanguageData.dataMap[index + 16].content;
                break;
            case 17:
                msg = LanguageData.dataMap[index + 17].content;
                break;
        }
        MogoMsgBox.Instance.ShowMsgBox(msg);
        EquipTipManager.Instance.CloseEquipTip();
        //MogoGlobleUIManager.Instance.Confirm(msg, (rst) => { MogoGlobleUIManager.Instance.ConfirmHide(); });
    }

    public void JewelCombineInEquiResp(byte errorId)
    {
        Mogo.Util.LoggerHelper.Debug("JewelCombineInEquiResp:" + errorId);

        switch (errorId)
        {
            case 0:
                //ItemJewelData data = ItemJewelData.JewelDic[m_currentJewelData.subtype][m_currentJewelData.level + 1];
                //string msg = LanguageData.dataMap[444].Format(data.name, data.effectDescriptionStr);
                //MogoMsgBox.Instance.ShowFloatingText(msg);

                InsetUIViewManager.Instance.ShowComposeSucessSign(true);
                //MogoFXManager.Instance.AttachParticleAnim("fx_ui_gem.prefab", 2000,
                //        GameObject.Find("Camera").transform.position, GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0], 0);

                RefreshUI();
                break;
            default:
                ShowInfoByErrorId(errorId);
                break;
        }
        //InsetUIViewManager.Instance.ShowInsetDialogDiamondTip(false);
        EquipTipManager.Instance.CloseEquipTip();
        RefreshUI();
    }

    public void SwitchToInsetUI(int slot)
    {
        //Debug.LogError("SwitchToInsetUI:" + index);

        MogoUIManager.Instance.SwitchInsetUI
        (
            () =>
            {
                InventoryManager.Instance.CurrentView = InventoryManager.View.InsetView;
                CurrentSlot = slot;
                //if (index == 0) CurrentSlot = 11;
                //else CurrentSlot = index;
                //Debug.LogError("CurrentSlot:" + CurrentSlot);
                RefreshUI();
            }
        );
    }
}