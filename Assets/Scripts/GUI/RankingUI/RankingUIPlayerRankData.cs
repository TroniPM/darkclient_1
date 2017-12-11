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

public class RankingUIPlayerRankData : MonoBehaviour 
{  
    #region 组件定义

    Transform m_Transform;
    
    private UILabel m_lblRankingUIPlayerRankDataName;
    private UILabel m_lblRankingUIPlayerRankDataRank;

    #endregion

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        m_Transform = transform;
        
        m_lblRankingUIPlayerRankDataName = m_Transform.Find("RankingUIPlayerRankDataName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblRankingUIPlayerRankDataRank = m_Transform.Find("RankingUIPlayerRankDataRank").GetComponentsInChildren<UILabel>(true)[0];
    }

    void Start()
    {
        RankingUIPlayerRankDataName = RankingUIPlayerRankDataName;
        RankingUIPlayerRankDataRank = RankingUIPlayerRankDataRank;
    }

   
    private string m_RankingUIPlayerRankDataName = "";
    public string RankingUIPlayerRankDataName
    {
        get
        {
            return m_RankingUIPlayerRankDataName;
        }
        set
        {
            m_RankingUIPlayerRankDataName = value;

            if (m_lblRankingUIPlayerRankDataName != null)
            {
                m_lblRankingUIPlayerRankDataName.text = value;
            }
        }
    }

    private string m_RankingUIPlayerRankDataRank = "";
    public string RankingUIPlayerRankDataRank
    {
        get
        {
            return m_RankingUIPlayerRankDataRank;
        }
        set
        {
            m_RankingUIPlayerRankDataRank = value;

            if (m_lblRankingUIPlayerRankDataRank != null)
            {
                m_lblRankingUIPlayerRankDataRank.text = value;
            }
        }
    }	
}
