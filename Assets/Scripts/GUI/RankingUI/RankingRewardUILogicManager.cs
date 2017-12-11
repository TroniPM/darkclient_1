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

public class RankingRewardUILogicManager : UILogicManager
{
    private static RankingRewardUILogicManager m_instance;
    public static RankingRewardUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new RankingRewardUILogicManager();
            }

            return RankingRewardUILogicManager.m_instance;
        }
    }

     #region 事件

    public void Initialize()
    {
        RankingRewardUIViewManager.Instance.RANKINGREWARDUICLOSEUP += OnCloseUp;
    }

    public override void Release()
    {
        base.Release();
        RankingRewardUIViewManager.Instance.RANKINGREWARDUICLOSEUP -= OnCloseUp;
    }

    void OnCloseUp()
    {
        LoggerHelper.Debug("OnRankingRewardUICloseUp");
        MogoUIManager.Instance.ShowRankingRewardUI(null, false);
    }

     #endregion

    #region 圣域守卫战奖励数据

    public void SetSanctuaryReward()
    {
        if (RankingRewardUIViewManager.Instance != null)
        {
            RankingRewardUIViewManager.Instance.SetRankingRewardUITitle(LanguageData.GetContent(46907));

            List<int> weekRankRewardList = SanctuaryRewardXMLData.GetWeekRankID();

            RankingRewardUIViewManager.Instance.SetUIGridList(weekRankRewardList.Count, () =>
                {
                    RankingRewardUIViewManager.Instance.SetSanctuaryGridListData(weekRankRewardList);
                });
        }



     
    }

    #endregion
}
