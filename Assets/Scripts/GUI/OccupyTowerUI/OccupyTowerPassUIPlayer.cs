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

public class OccupyTowerPassUIPlayer : MogoUIBehaviour 
{
    private UILabel m_lblOccupyTowerPassUIPlayerName;
    private UILabel m_lblOccupyTowerPassUIPlayerScore;
    private UILabel m_lblOccupyTowerPassUIPlayerAddition;
    private UILabel m_lblOccupyTowerPassUIPlayerColon;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblOccupyTowerPassUIPlayerName = FindTransform("OccupyTowerPassUIPlayerName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOccupyTowerPassUIPlayerScore = FindTransform("OccupyTowerPassUIPlayerScore").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOccupyTowerPassUIPlayerAddition = FindTransform("OccupyTowerPassUIPlayerAddition").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOccupyTowerPassUIPlayerColon = FindTransform("OccupyTowerPassUIPlayerColon").GetComponentsInChildren<UILabel>(true)[0];
    }

    public void SetPlayerName(string name)
    {
        m_lblOccupyTowerPassUIPlayerName.text = name;
    }

    public void SetPlayerScore(int score)
    {
        m_lblOccupyTowerPassUIPlayerScore.text = score.ToString();
    }

    public void SetPlayerAddition(int addition)
    {
        m_lblOccupyTowerPassUIPlayerAddition.text = addition.ToString();
    }

    public void SetPlayerCamp(int camp)
    {
        if (camp == 1)
        {
            m_lblOccupyTowerPassUIPlayerName.color = new Color32(0, 210, 255, 255);
            m_lblOccupyTowerPassUIPlayerScore.color = new Color32(0, 210, 255, 255);
            m_lblOccupyTowerPassUIPlayerAddition.color = new Color32(0, 210, 255, 255);
            m_lblOccupyTowerPassUIPlayerColon.color = new Color32(0, 210, 255, 255);
        }
        else
        {
            m_lblOccupyTowerPassUIPlayerName.color = new Color32(255, 126, 0, 255);
            m_lblOccupyTowerPassUIPlayerScore.color = new Color32(255, 126, 0, 255);
            m_lblOccupyTowerPassUIPlayerAddition.color = new Color32(255, 126, 0, 255);
            m_lblOccupyTowerPassUIPlayerColon.color = new Color32(255, 126, 0, 255);
        }
    }
}
