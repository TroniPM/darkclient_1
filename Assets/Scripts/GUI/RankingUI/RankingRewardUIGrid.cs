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

public class RankingRewardUIGrid : MogoUIBehaviour 
{
    private UILabel m_lblRankingRewardUIGridLine1Text;
    private UILabel m_lblRankingRewardUIGridLine2Text;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblRankingRewardUIGridLine1Text = FindTransform("RankingRewardUIGridLine1Text").GetComponentsInChildren<UILabel>(true)[0];
        m_lblRankingRewardUIGridLine2Text = FindTransform("RankingRewardUIGridLine2Text").GetComponentsInChildren<UILabel>(true)[0];
    }

    #region 设置奖励信息

    /// <summary>
    /// 排名Index
    /// </summary>
    private int m_index;
    public int Index
    {
        get { return m_index; }
        set
        {
            m_index = value;

            if (m_index < 3)
            {
                // 前3名设置不同文本颜色
                m_lblRankingRewardUIGridLine1Text.effectStyle = UILabel.Effect.Outline;
                m_lblRankingRewardUIGridLine1Text.color = new Color32(255, 210, 0, 255);
                m_lblRankingRewardUIGridLine1Text.effectColor = new Color32(68, 41, 0, 255);      
            }
            else
            {
                m_lblRankingRewardUIGridLine1Text.effectStyle = UILabel.Effect.None;
                m_lblRankingRewardUIGridLine1Text.color = new Color32(255, 255, 255, 255);
            }
        }
    }

    /// <summary>
    /// 设置奖励信息
    /// </summary>
    /// <param name="title"></param>
    /// <param name="info"></param>
    public void SetRankingReward(string title, string info)
    {
        m_lblRankingRewardUIGridLine1Text.text = title;
        m_lblRankingRewardUIGridLine2Text.text = info;
    }

    #endregion
}
