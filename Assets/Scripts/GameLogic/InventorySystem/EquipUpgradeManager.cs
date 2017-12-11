/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EquipUpgradeManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：装备升级管理
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;
using System.Collections.Generic;

public class EquipUpgradeManager
{
    static public EquipUpgradeManager Instance;

    public EquipUpgradeManager()
    {
        Instance = this;
        AddEventListener();
    }

    private void AddEventListener()
    {
        EventDispatcher.AddEventListener(EquipUpgradeViewManager.ON_UPGRADE, OnUpgrade);
        EventDispatcher.AddEventListener<int>(EquipUpgradeViewManager.ON_CLICK_MATERIAL, OnClickMaterial);
    }

    public void RemoveListener()
    {
        EventDispatcher.RemoveEventListener(EquipUpgradeViewManager.ON_UPGRADE, OnUpgrade);
        EventDispatcher.RemoveEventListener<int>(EquipUpgradeViewManager.ON_CLICK_MATERIAL, OnClickMaterial);
    }

    ItemEquipment m_equip;
    public ItemEquipment Equip
    {
        set
        {
            m_equip = value;
        }
    }
    public bool CanUpgrade(ItemParent item)
    {
        return item.effectId > 0;
    }


    public void ShowUI()
    {
        EquipUpgradeViewManager.EquipUpgradeViewData data = GetViewdata(m_equip);
        EquipUpgradeViewManager.Instance.SetViewData(data);
        EquipUpgradeViewManager.Instance.ShowMainUI();
    }

    private EquipUpgradeViewManager.EquipUpgradeViewData GetViewdata(ItemEquipment m_equip)
    {

        EquipUpgradeViewManager.EquipUpgradeViewData viewData = new EquipUpgradeViewManager.EquipUpgradeViewData();
        viewData.oldEquipId = m_equip.templateId;

        ItemEffectData itemEffect = ItemEffectData.dataMap.Get(m_equip.effectId);
        ItemEquipmentData newEquip = null;
        foreach (int id in itemEffect.reward1.Keys)
        {
            newEquip = ItemEquipmentData.dataMap.Get(id);
            viewData.newEquipId = newEquip.id;
            break;
        }
        viewData.materialIdList = new List<int>();
        viewData.materilNumStrList = new List<string>();
        foreach (KeyValuePair<int, int> pair in itemEffect.costId)
        {
            Mogo.Util.LoggerHelper.Debug(pair.Key);
            ItemParentData item = ItemParentData.GetItem(pair.Key);
            if (item.itemType > 0)
            {
                viewData.materialIdList.Add(item.id);
                string numStr = string.Empty;
                int num = InventoryManager.Instance.GetItemNumByIdAndType(item.id, item.itemType);
                int needNum = pair.Value;
                numStr = string.Concat(num, "/", needNum);
                if (needNum > num)
                {
                    numStr = MogoUtils.GetRedString(numStr);
                }
                viewData.materilNumStrList.Add(numStr);
            }
            else
            {
                viewData.needGold = "X" + pair.Value;
            }
        }

        int level = MogoWorld.thePlayer.level;
        viewData.power = LanguageData.GetContent(190000, newEquip.GetScore(level) - m_equip.GetScore(level));

        return viewData;
    }

    private void OnUpgrade()
    {
        Mogo.Util.LoggerHelper.Debug("onUpgrade:" + m_equip.templateId);
        MogoWorld.thePlayer.RpcCall("UseItemReq", m_equip.id, m_equip.gridIndex, 1);
    }

    private void OnClickMaterial(int index)
    {
        Mogo.Util.LoggerHelper.Debug("OnClickMaterial:" + index);
    }
}