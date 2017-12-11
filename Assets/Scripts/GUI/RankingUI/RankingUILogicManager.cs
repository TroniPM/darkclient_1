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
using TDBID = System.UInt64;

public class RankingUILogicManager : UILogicManager
{
    private static RankingUILogicManager m_instance;
    public static RankingUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new RankingUILogicManager();
            }

            return RankingUILogicManager.m_instance;
        }
    }

    // 排行榜单列表
    private List<int> m_sortTabIDList = new List<int>();
    public List<int> SortTabIDList
    {
        get
        {
            return m_sortTabIDList;
        }
        set
        {
            m_sortTabIDList = value;
        }        
    }

    // 当前查看的玩家TDBID
    private TDBID m_currentTDBID;
    public TDBID CurrentTDBID
    {
        get { return m_currentTDBID; }
        set
        {
            m_currentTDBID = value;
        }
    }

    #region 事件

    public void Initialize()
    {
        RankingUIViewManager.Instance.RANKINGUICLOSEUP += OnCloseUp;
        RankingUIViewManager.Instance.RANKINGUIBACKTORANK += OnBackToRankUp;
        RankingUIViewManager.Instance.RANKINGUIBECAMEFS += OnBecameFSUp;
        RankingUIViewManager.Instance.RANKINGUIPAGEUP += OnRankingPageUp;
        RankingUIViewManager.Instance.RANKINGUISHOWREWARDUP += OnShowRewardUp;
        RankingUIDict.RANKINGTABUP += OnRankingTabUp;
        RankingUIDict.RANKINGMAINDATAUP += OnRankingMainDataUp;
        

        // 获取通过priority进行排序，并创建左列榜单列表
        SortTabIDList = RankData.GetSortRankIDList();
        if (RankingUIViewManager.Instance != null)
            RankingUIViewManager.Instance.GenerateRankingTabList(SortTabIDList);
    }

    public override void Release()
    {
        base.Release();
        RankingUIViewManager.Instance.RANKINGUICLOSEUP -= OnCloseUp;
        RankingUIViewManager.Instance.RANKINGUIBACKTORANK -= OnBackToRankUp;
        RankingUIViewManager.Instance.RANKINGUIBECAMEFS -= OnBecameFSUp;
        RankingUIViewManager.Instance.RANKINGUIPAGEUP -= OnRankingPageUp;
        RankingUIViewManager.Instance.RANKINGUISHOWREWARDUP -= OnShowRewardUp;
        RankingUIDict.RANKINGTABUP -= OnRankingTabUp;
        RankingUIDict.RANKINGMAINDATAUP -= OnRankingMainDataUp;
    }

    void OnCloseUp()
    {
        LoggerHelper.Debug("OnRankingUICloseUp");
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnBackToRankUp()
    {
        RankingUIViewManager.Instance.SwitchRankingMainDataUI();
    }   

    void OnBecameFSUp()
    {
        EventDispatcher.TriggerEvent(RankEvent.OnIsIdolChangedTodayReq, CurrentTDBID);
    }

    void OnShowRewardUp(int tabIndex)
    {
        if (tabIndex < m_sortTabIDList.Count)
        {
            RankData xmlRankData = RankData.GetRankDataByID(m_sortTabIDList[tabIndex]);
            if (xmlRankData != null)
            {
                switch (xmlRankData.id)
                {
                    case 1: // 战力榜	1
                        break;

                    case 2: // 等级榜	2
                        break;

                    case 3: // 荣誉榜	3
                        break;

                    case 4: // 积分榜	4
                        break;

                    case 5: // 贡献榜	5
                        MogoUIManager.Instance.ShowRankingRewardUI(() =>
                        {
                            RankingRewardUILogicManager.Instance.SetSanctuaryReward();
                        }, true);
                        break;

                    case 6:  // 试炼榜	6
                        break;

                    case 7: // S榜	7
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 点击不同的排行榜
    /// </summary>
    /// <param name="i"></param>
    void OnRankingTabUp(int iTabIndex)
    {
        RankingUIViewManager.Instance.CurrentTabIndex = iTabIndex;  
		RankingUIViewManager.Instance.CurrentPage = RankingUIViewManager.DefaultPage;
        RankingUIViewManager.Instance.ResetMainRankDataUI();        
		OnRankingPageUp(RankingUIViewManager.DefaultPage);
        EventDispatcher.TriggerEvent(RankEvent.GetHasOnRank, m_sortTabIDList[RankingUIViewManager.Instance.CurrentTabIndex]);
        RankingUIViewManager.Instance.SwitchRankingMainDataUI(); // 切回排行数据面板
    }

    /// <summary>
    /// 滚页更新数据
    /// </summary>
    /// <param name="iPage"></param>
    void OnRankingPageUp(int iPage)
    {
        if (RankingUIViewManager.Instance.CurrentTabIndex < m_sortTabIDList.Count)
        {
            EventDispatcher.TriggerEvent(RankEvent.GetRankList, m_sortTabIDList[RankingUIViewManager.Instance.CurrentTabIndex], RankingUIViewManager.Instance.CurrentPage);            
        }
    }

    /// <summary>
    /// 点击玩家，获取玩家详细信息
    /// </summary>
    /// <param name="index"></param>
    void OnRankingMainDataUp(TDBID tdbID)
    {
        // 通过index获取玩家ID；
        // 如果为玩家自己,获取本地数据显示；
        // 如果为其他玩家，则从服务器获取数据
        RankingUIViewManager.Instance.SwitchPlayerDetailInfoUI();
        CurrentTDBID = tdbID;
        EventDispatcher.TriggerEvent(RankEvent.GetRankAvatarInfo, tdbID);        
    }

    /// <summary>
    /// 已有偶像，弹出变更偶像确认框
    /// </summary>
    public void OnChangeIdolEnable(string oldIdolName, string newIdolName)
    {
        if (string.IsNullOrEmpty(oldIdolName))
        {
            EventDispatcher.TriggerEvent(RankEvent.OnChangeIdolReq, CurrentTDBID); // 确定变更偶像
        }
        else
        {
            string context = string.Format(LanguageData.GetContent(47404), oldIdolName, newIdolName);
            MogoGlobleUIManager.Instance.Confirm(context, (isOK) =>
            {
                if (isOK)
                {
                    MogoGlobleUIManager.Instance.ConfirmHide();
                    EventDispatcher.TriggerEvent(RankEvent.OnChangeIdolReq, CurrentTDBID); // 确定变更偶像
                }
                else
                {
                    MogoGlobleUIManager.Instance.ConfirmHide();
                }
            }, "OK", "CANCEL");
        }     
    }

    #endregion
}
