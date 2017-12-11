#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;
using System.Collections.Generic;

public class EquipRecommendGridData
{
    public int equipSlot; // 装备槽 
    public int access; // 获得途径ID
    public int accessType;
    public int recommendEquipID; // 推荐装备ID
    public ItemEquipment currentItemEquipment; // 当前身上装备，没有为null
}

public class EquipRecommendUILogicManager : UILogicManager
{
    private static EquipRecommendUILogicManager m_instance;
    public static EquipRecommendUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EquipRecommendUILogicManager();
            }

            return EquipRecommendUILogicManager.m_instance;
        }
    }

    #region 事件

    public void Initialize()
    {
        EquipRecommendUIViewManager.Instance.EQUIPRECOMMENDUICLOSEUP += OnCloseUp;
        EquipRecommendUIViewManager.Instance.EQUIPRECOMMENDUICALDATALIST += CalEquipRecommendGridDataList;
        EquipRecommendUIDict.EQUIPRECOMMENDLINKBTNUP += OnRecommendEquipLinkBtnUp;        
    }

    public override void Release()
    {
        base.Release();
        EquipRecommendUIViewManager.Instance.EQUIPRECOMMENDUICLOSEUP -= OnCloseUp;
        EquipRecommendUIViewManager.Instance.EQUIPRECOMMENDUICALDATALIST -= CalEquipRecommendGridDataList;
        EquipRecommendUIDict.EQUIPRECOMMENDLINKBTNUP -= OnRecommendEquipLinkBtnUp;
    }

    void OnCloseUp()
    {
        LoggerHelper.Debug("OnEquipRecommendUICloseUp");
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    /// <summary>
    /// 据获得途径字段来决定按钮跳转(access_i<10000 材料兑换  accsee_i>=10000&&access_i<20000  副本掉落  access_i>=20000 试炼之塔)
    /// 1表示跳转到30级紫装兑换界面     accessType = 1,副本兑换     accessType = 2,试炼之塔兑换
    /// 2表示跳转到40级紫装兑换界面
    /// 3表示跳转到50级紫装兑换界面
    /// 4表示跳转到60级紫装兑换界面
    /// 5表示跳转到70级紫装兑换界面
    /// 6表示跳转到80级紫装兑换界面
    /// 7表示跳转到90级紫装兑换界面
    /// 8表示跳转到100级紫装兑换界面
    /// 10000以上表示副本ID，点击之后跳转到对应的副本
    /// </summary>
    /// <param name="access"></param>
    void OnRecommendEquipLinkBtnUp(int access, int accessType)
    {
        if (access >= 1 && access <= 8)
        {
            // accessType = 2,副本兑换     accessType = 1,试炼之塔兑换
            if (accessType == 2)
                EquipExchangeManager.Instance.ShowUI(30 + (access - 1) * 10, EquipExchangeManager.InstanceEquipSubType);
            else if (accessType == 1)
                EquipExchangeManager.Instance.ShowUI(30 + (access - 1) * 10, EquipExchangeManager.ClimbTowerEquipSubType);
                
        }      
        else if(access == 20)
        {
            EquipExchangeManager.Instance.ShowUI(1, EquipExchangeManager.GoldEquipSubType);
        }
        else if (access >= 10000 && access < 20000)
        {
            // 副本
            if (MogoWorld.thePlayer.CheckCurrentMissionEnterable(access, accessType))
                InstanceUILogicManager.Instance.MissionOpenAllTheWay(access, accessType, MissionOpenType.Drop);
            else
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(46981));
        }
        else if (access >= 20000 && access < 30000)
        {
            // 试炼之塔
            if (MogoWorld.thePlayer.level < SystemRequestLevel.CILMBTOWER)
            {
                MogoMsgBox.Instance.ShowFloatingText(string.Format(LanguageData.GetContent(47117), SystemRequestLevel.CILMBTOWER));
                return;
            }

            MogoUIManager.Instance.OpenWindow((int)WindowName.Tower, () => { EventDispatcher.TriggerEvent(Events.TowerEvent.GetInfo); });
        }
        else if (access >= 30000)
        {
            // 湮灭之门
            if (MogoWorld.thePlayer.level < SystemRequestLevel.DOOROFBURY)
            {
                MogoMsgBox.Instance.ShowFloatingText(string.Format(LanguageData.GetContent(47117), SystemRequestLevel.DOOROFBURY));
                return;
            }

            NormalMainUILogicManager.Instance.OnPVEPlayIconUp(); 
        }
    }

    #endregion   

    #region 统计可以推荐的装备列表数据

    private List<EquipRecommendGridData> m_equipRecommendGridDataList = new List<EquipRecommendGridData>();

    /// <summary>
    /// 统计可以推荐的装备列表数据
    /// </summary>
    private void CalEquipRecommendGridDataList()
    {
        m_equipRecommendGridDataList.Clear();

        EquipRecommendData xmlData = EquipRecommendData.GetEquipRecommendData((int)MogoWorld.thePlayer.vocation, (int)MogoWorld.thePlayer.level);
        Dictionary<int, ItemEquipment> EquipOnDic = InventoryManager.Instance.EquipOnDic;
        if (xmlData != null)
        {
            for (int equipSlot = 1; equipSlot <= InventoryManager.SLOT_NUM; equipSlot++)
            {
                ItemEquipment itemEquipment = null;
                int recommendScore = GetEquipRecommendScoreBySlot(xmlData, equipSlot);
                int currentScore = 0;
                if (EquipOnDic.ContainsKey(equipSlot))
                {
                    itemEquipment = EquipOnDic[equipSlot];
                    currentScore = itemEquipment.GetScore(MogoWorld.thePlayer.level);                    
                }

                // 如果当前装备评分小于推荐装备评分，加入列表(若角色所穿装备评分<推荐装备评分，显示在推荐列表里；否则不显示)
                if (currentScore < recommendScore)
                {
                    EquipRecommendGridData gridData = new EquipRecommendGridData();
                    gridData.equipSlot = equipSlot;
                    gridData.access = GetEquipRecommendAccessBySlot(xmlData, equipSlot);
                    gridData.accessType = GetEquipRecommendAccessTypeBySlot(xmlData, equipSlot);
                    gridData.recommendEquipID = GetEquipRecommendGoodIDBySlot(xmlData, equipSlot);
                    gridData.currentItemEquipment = itemEquipment;
                    m_equipRecommendGridDataList.Add(gridData);
                }
            }
        }

        if (EquipRecommendUIViewManager.Instance != null)
        {
            EquipRecommendUIViewManager.Instance.SetEquipRecommendList(m_equipRecommendGridDataList.Count, () =>
                {
                    EquipRecommendUIViewManager.Instance.SetEquipRecommendDataList(m_equipRecommendGridDataList);
                });
        }
    }

    /// <summary>
    /// 通过装备槽获取对应的推荐装备评分
    /// </summary>
    /// <param name="xmlData"></param>
    /// <param name="equipSlot"></param>
    /// <returns></returns>
    private int GetEquipRecommendScoreBySlot(EquipRecommendData xmlData, int equipSlot)
    {
        int recommendEquipID = GetEquipRecommendGoodIDBySlot(xmlData, equipSlot);
        ItemEquipmentData recommendEquipData = ItemEquipmentData.GetItemEquipmentData(recommendEquipID);
        if (recommendEquipData != null)
            return recommendEquipData.GetScore(MogoWorld.thePlayer.level);
        else
            return 0;
    }

    /// <summary>
    /// 通过装备槽获取对应的推荐装备
    /// </summary>
    /// <param name="xmlData"></param>
    /// <param name="equipSlot"></param>
    /// <returns></returns>
    private int GetEquipRecommendGoodIDBySlot(EquipRecommendData xmlData, int equipSlot)
    {
        if (equipSlot - 1 < xmlData.goodsid.Count)
            return xmlData.goodsid[equipSlot - 1];
        return 0;
    }

    /// <summary>
    /// 通过装备槽获取对应的推荐装备获取路径
    /// </summary>
    /// <param name="xmlData"></param>
    /// <param name="equipSlot"></param>
    /// <returns></returns>
    private int GetEquipRecommendAccessBySlot(EquipRecommendData xmlData, int equipSlot)
    {
        if (equipSlot - 1 < xmlData.access.Count)
            return xmlData.access[equipSlot - 1];
        return 0;
    }

    /// <summary>
    /// 通过装备槽获取对应的推荐装备获取路径Type
    /// </summary>
    /// <param name="xmlData"></param>
    /// <param name="equipSlot"></param>
    /// <returns></returns>
    private int GetEquipRecommendAccessTypeBySlot(EquipRecommendData xmlData, int equipSlot)
    {
        if (equipSlot - 1 < xmlData.accessType.Count)
            return xmlData.accessType[equipSlot - 1];
        return 0;
    }

    #endregion
}
