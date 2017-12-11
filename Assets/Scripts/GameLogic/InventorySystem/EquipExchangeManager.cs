/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EquipExchangeManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013/9/26
// 模块描述：装备兑换管理类
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;

public class EquipExchangeManager
{
    public static EquipExchangeManager Instance = new EquipExchangeManager();
    private int m_currentLevel;
    private int m_currentSubtype;

    private Dictionary<int, int> m_materilaIdDic = new Dictionary<int, int>();
    private List<List<EquipExchangeViewData>> m_currentViewData;
    private int m_currentTabIndex;

    EquipExchangeManager()
    {
        EquipExchangeUIViewManager.Instance.OnTabSelect = OnTabSelect;
    }


    /// <summary>
    /// 打开对应的装备兑换界面
    /// </summary>
    /// <param name="level">等级(金的话等级填1)</param>
    /// <param name="subtype">类型：1红，2紫，3金</param>
    static public int ClimbTowerEquipSubType = 1; // 试炼之塔装备兑换
    static public int InstanceEquipSubType = 2; // 副本装备兑换
    static public int GoldEquipSubType = 3; // 金色装备兑换
    public void ShowUI(int level,int subtype)
    {
        m_currentLevel =level;
        m_currentSubtype = subtype;
        m_currentViewData = GetViewData(level);

        int index = 0;
        bool isShowTab;
        if (subtype > 2)//金装
        {
            index = 0;
            isShowTab = false;
        }
        else
        {
            index = subtype - 1;
            isShowTab = true;
        }
        m_currentTabIndex = index;

        EquipExchangeUIViewManager.Instance.Show(index, m_currentViewData[index], isShowTab);
        UpdateGold(MogoWorld.thePlayer.gold);
        UpdateMaterial(index);
    }

    private void OnTabSelect(int index)
    {
        EquipExchangeUIViewManager.Instance.Show(index, m_currentViewData[index]);
        UpdateMaterial(index);
        m_currentTabIndex = index;
    }

    private List<List<EquipExchangeViewData>> GetViewData(int level)
    {
        List<List<EquipExchangeViewData>> dataList = new List<List<EquipExchangeViewData>>();
        dataList.Add(new List<EquipExchangeViewData>());
        dataList.Add(new List<EquipExchangeViewData>());
        Mogo.Util.LoggerHelper.Debug("item.level:" + level);
        HashSet<int> dataSet = ItemExchangeData.DataDic[level];
        foreach (int i in dataSet)
        {
            ItemExchangeData exchangeData = ItemExchangeData.dataMap.Get(i);
            Mogo.Util.LoggerHelper.Debug("exchangeData.GetRewardByVocation(MogoWorld.thePlayer.vocation):" + exchangeData.GetRewardByVocation(MogoWorld.thePlayer.vocation));
            ItemParentData itemReward = ItemParentData.GetItem(exchangeData.GetRewardByVocation(MogoWorld.thePlayer.vocation));
            EquipExchangeViewData viewData = new EquipExchangeViewData()
            {
                goldNum = "x" + exchangeData.GoldNum,
                title = itemReward.Name,
                itemId = itemReward.id,
                materialNum = "x" + exchangeData.MeterailNum,
                materialId = exchangeData.MeterialId,
                onExchange = () => { OnExchange(exchangeData.id); }
            };
            if (exchangeData.GoldNum <= 0) viewData.goldNum = string.Empty;
            if (exchangeData.GoldNum > MogoWorld.thePlayer.gold)
                viewData.goldNum = MogoUtils.GetRedString(viewData.goldNum);
            if (exchangeData.MeterailNum > InventoryManager.Instance.GetItemNumById(exchangeData.MeterialId))
                viewData.materialNum = MogoUtils.GetRedString(viewData.materialNum);
            if (exchangeData.subtype < 3)
            {
                dataList[exchangeData.subtype - 1].Add(viewData);
                m_materilaIdDic[exchangeData.subtype - 1] = exchangeData.MeterialId;
            }
            else
            {
                dataList[0].Add(viewData);
                m_materilaIdDic[0] = exchangeData.MeterialId;
            }
        }
        return dataList;
    }

    private void OnExchange(int id)
    {
        MogoWorld.thePlayer.RpcCall("PurpleExchangeReq", id);
    }

    /// <summary>
    /// 属性同步那边调用
    /// </summary>
    /// <param name="gold"></param>
    public void UpdateGold(uint gold)
    {
        EquipExchangeUIViewManager.Instance.SetGold(gold.ToString());
    }

    /// <summary>
    /// 兑换成功时候调用
    /// </summary>
    public void UpdateMaterial(int index)
    {
        //Debug.LogError(index);
        EquipExchangeUIViewManager.Instance.SetMaterial(m_materilaIdDic[index], InventoryManager.Instance.GetItemNumById(m_materilaIdDic[index]).ToString());
    }

    public void PurpleExchangeResp(int errorCode)
    {
        if (errorCode == 0)
        {
            m_currentViewData = GetViewData(m_currentLevel);
            //Debug.LogError("m_currentTabIndex:" + m_currentTabIndex);
            EquipExchangeUIViewManager.Instance.RefreshUI(m_currentViewData[m_currentTabIndex]);
            UpdateMaterial(m_currentTabIndex);
            //EquipExchangeUIViewManager.Instance.Hide();
        }
    }
}