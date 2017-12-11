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
using System.Collections.Generic;
using Mogo.GameData;

public class BossTreasureGridData
{
    public int id;
    public string iconName;
    public string bossName;
    public int level;
    public BossTreasureStatus status;
    // public List<string> reward;
}

public class InstanceBossTreasureUILogicManager : UILogicManager
{
    private static InstanceBossTreasureUILogicManager m_instance;
    public static InstanceBossTreasureUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new InstanceBossTreasureUILogicManager();
            }

            return InstanceBossTreasureUILogicManager.m_instance;
        }
    }

    #region 事件

    public void Initialize()
    {
        InstanceBossTreasureUIViewManager.Instance.BOSSTREASUREUICLOSEUP += OnCloseUp;
        InstanceBossTreasureUIDict.BOSSTREASUREBTNUP += OnBossTreasureBtnUp;
        InstanceBossTreasureUIDict.BOSSREWARDUP += OnBossRewardUp;
    }

    public override void Release()
    {
        base.Release();
        InstanceBossTreasureUIViewManager.Instance.BOSSTREASUREUICLOSEUP -= OnCloseUp;
        InstanceBossTreasureUIDict.BOSSTREASUREBTNUP -= OnBossTreasureBtnUp;
        InstanceBossTreasureUIDict.BOSSREWARDUP += OnBossRewardUp;
    }

    void OnCloseUp()
    {
        LoggerHelper.Debug("OnInstanceBossTreasureUICloseUp");
        MogoUIManager.Instance.ShowInstanceBossTreasureUI(null, false);
    }

    void OnBossTreasureBtnUp(int bossID)
    {
        EventDispatcher.TriggerEvent(Events.InstanceEvent.GetBossChestRewardReq, bossID);
    }

    /// <summary>
    /// Boss宝箱奖励
    /// </summary>
    /// <param name="bossID"></param>
    void OnBossRewardUp(int bossID)
    {
        if (InstanceBossTreasureUIViewManager.Instance != null)
        {
            InstanceBossTreasureUIViewManager.Instance.ShowInstanceBossTreasureUIRewardUI(true);

            var data = BossChestData.dataMap.Get(bossID);
            List<string> rewardString = new List<string>();
            foreach (var item in data.reward)
                rewardString.Add(string.Concat(ItemParentData.GetItem(item.Key).Name, " * ", item.Value));
            InstanceBossTreasureUIViewManager.Instance.SetInstanceBossTreasureUIRewardList(rewardString);
        }
    }

    #endregion

    private List<BossTreasureGridData> m_listBossTreasureGridData  = new List<BossTreasureGridData>();
    
    /// <summary>
    /// 设置Boss宝箱信息
    /// </summary>
    /// <param name="listBossTreasureGridData"></param>
    public void SetBossTreasureList(List<BossTreasureGridData> listBossTreasureGridData)
    {
        m_listBossTreasureGridData = listBossTreasureGridData;
        if (InstanceBossTreasureUIViewManager.Instance != null)
        {
            InstanceBossTreasureUIViewManager.Instance.SetBossTreasureGridList(m_listBossTreasureGridData.Count, ()=>
            {
                InstanceBossTreasureUIViewManager.Instance.SetBossTreasureGridListData(m_listBossTreasureGridData);
            });
        }
    }
    
}
