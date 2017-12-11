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

public class RankingUITabGrid : MonoBehaviour 
{
    int m_tabID;
    public int TabID
    {
        get
        {
            return m_tabID;
        }
        set
        {
            m_tabID = value;
        }
    }    

    #region 组件定义
    
    Transform m_Transform;
    private UILabel m_lblRankingUITabTextDown;
    private UILabel m_lblRankingUITabTextUp;
    private GameObject m_goRankingUITabReward;

    #endregion

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        m_Transform = transform;
        m_lblRankingUITabTextUp = m_Transform.Find("RankingUITabTextUp").GetComponentsInChildren<UILabel>(true)[0];
        m_lblRankingUITabTextDown = m_Transform.Find("RankingUITabTextDown").GetComponentsInChildren<UILabel>(true)[0];
        m_goRankingUITabReward = m_Transform.Find("RankingUITabReward").gameObject;
    }

    void Start()
    {
        RankingUITabText = RankingUITabText;
        IfReward = IfReward;
    }

    void OnClick()
    {
        if (RankingUIDict.RANKINGTABUP != null)
            RankingUIDict.RANKINGTABUP(TabID);
    }

    private string m_RankingUITabText = "";
    public string RankingUITabText
    {
        get
        {
            return m_RankingUITabText;
        }
        set
        {
            m_RankingUITabText = value;

            if (m_lblRankingUITabTextUp != null)
                m_lblRankingUITabTextUp.text = value;

            if (m_lblRankingUITabTextDown != null)
                m_lblRankingUITabTextDown.text = value;
        }
    }

    private bool m_ifReward;
    public bool IfReward
    {
        get
        {
            return m_ifReward;
        }
        set
        {
            m_ifReward = value;

            if (m_goRankingUITabReward != null)
                m_goRankingUITabReward.SetActive(m_ifReward);
        }
    }
}
