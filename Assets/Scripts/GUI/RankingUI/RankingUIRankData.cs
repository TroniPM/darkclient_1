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
using TDBID = System.UInt64;
using System.Collections.Generic;

public class RankingUIRankData : MonoBehaviour 
{
    private TDBID m_avatarID;
    public TDBID AvatarID
    {
        get
        {
            return m_avatarID;
        }
        set
        {
            m_avatarID = value;

            IsIdol = m_avatarID == RankManager.Instance.SelfIdolTDBID ? true : false;
        }
    }

    #region 组件定义

    Transform m_Transform;

    private GameObject m_goRankingUIMainRankDataBG;
    private UISprite m_spRankingUIMainRankData1Medal;
    private UILabel m_lblRankingUIMainRankData1;
    private GameObject m_goRankingUIMainRankData2Idol;
    private UILabel m_lblRankingUIMainRankData2;
    private UILabel m_lblRankingUIMainRankData3;
    private UILabel m_lblRankingUIMainRankData4;
    private UILabel m_lblRankingUIMainRankData5;

    List<UILabel> m_listRankData = new List<UILabel>();

    #endregion

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        m_Transform = transform;

        m_goRankingUIMainRankDataBG = m_Transform.Find("RankingUIMainRankDataBG").gameObject;
        m_spRankingUIMainRankData1Medal = m_Transform.Find("RankingUIMainRankData1Medal").GetComponentsInChildren<UISprite>(true)[0];
        m_lblRankingUIMainRankData1 = m_Transform.Find("RankingUIMainRankData1").GetComponentsInChildren<UILabel>(true)[0];
        m_goRankingUIMainRankData2Idol = m_Transform.Find("RankingUIMainRankData2Idol").gameObject;
        m_lblRankingUIMainRankData2 = m_Transform.Find("RankingUIMainRankData2").GetComponentsInChildren<UILabel>(true)[0];
        m_lblRankingUIMainRankData3 = m_Transform.Find("RankingUIMainRankData3").GetComponentsInChildren<UILabel>(true)[0];
        m_lblRankingUIMainRankData4 = m_Transform.Find("RankingUIMainRankData4").GetComponentsInChildren<UILabel>(true)[0];
        m_lblRankingUIMainRankData5 = m_Transform.Find("RankingUIMainRankData5").GetComponentsInChildren<UILabel>(true)[0];
        m_listRankData.Add(m_lblRankingUIMainRankData1);
        m_listRankData.Add(m_lblRankingUIMainRankData2);
        m_listRankData.Add(m_lblRankingUIMainRankData3);
        m_listRankData.Add(m_lblRankingUIMainRankData4);
        m_listRankData.Add(m_lblRankingUIMainRankData5);
    }

    void Start()
    {
        Index = Index;
        IsIdol = IsIdol;
        RankingUIMainRankData1 = RankingUIMainRankData1;
        RankingUIMainRankData2Name = RankingUIMainRankData2Name;
        RankingUIMainRankData3 = RankingUIMainRankData3;
        RankingUIMainRankData4 = RankingUIMainRankData4;
        RankingUIMainRankData5FansCount = RankingUIMainRankData5FansCount;        
    }

    private int m_index;
    public int Index
    {
        get
        {
            return m_index;
        }
        set
        {
            m_index = value;

            if (m_goRankingUIMainRankDataBG != null && m_spRankingUIMainRankData1Medal != null
                && m_lblRankingUIMainRankData1 != null
                && m_lblRankingUIMainRankData2 != null
                && m_lblRankingUIMainRankData3 != null
                && m_lblRankingUIMainRankData4 != null
                && m_lblRankingUIMainRankData5 != null
                )
            {
                if (m_index % 2 == 0)
                    m_goRankingUIMainRankDataBG.SetActive(true);
                else
                    m_goRankingUIMainRankDataBG.SetActive(false);

                if (m_index < 3)
                {
                    // 前3名设置不同文本颜色
                    for (int i = 0; i < m_listRankData.Count; i++)
                    {
                        m_listRankData[i].effectStyle = UILabel.Effect.Outline;
                        m_listRankData[i].color = new Color32(255, 216, 0, 255);
                        m_listRankData[i].effectColor = new Color32(0, 0, 0, 255);
                    }               
     
                    // 前3名显示排名徽章
                    m_spRankingUIMainRankData1Medal.gameObject.SetActive(true);
                    m_listRankData[0].gameObject.SetActive(false);
                    m_spRankingUIMainRankData1Medal.spriteName = string.Concat("phb_", m_index + 1);
                }
                else
                {
                    for (int i = 0; i < m_listRankData.Count; i++)
                    {
                        m_listRankData[i].effectStyle = UILabel.Effect.None;
                        m_listRankData[i].color = new Color32(63, 27, 4, 255);
                    }

                    // 显示排名
                    m_spRankingUIMainRankData1Medal.gameObject.SetActive(false);
                    m_listRankData[0].gameObject.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// 是否是偶像
    /// </summary>
    private bool m_isIdol = false;
    private bool IsIdol
    {
        get { return m_isIdol; }
        set
        {
            m_isIdol = value;

            if (m_goRankingUIMainRankData2Idol != null)
                m_goRankingUIMainRankData2Idol.SetActive(m_isIdol);
        }
    }

    private string m_RankingUIMainRankData1 = "";
    public string RankingUIMainRankData1
    {
        get
        {
            return m_RankingUIMainRankData1;
        }
        set
        {
            m_RankingUIMainRankData1 = value;
            if (m_lblRankingUIMainRankData1 != null)
                m_lblRankingUIMainRankData1.text = value;
        }
    }

    private string m_RankingUIMainRankData2 = "";
    public string RankingUIMainRankData2Name
    {
        get
        {
            return m_RankingUIMainRankData2;
        }
        set
        {
            m_RankingUIMainRankData2 = value;
            if (m_lblRankingUIMainRankData2 != null)
                m_lblRankingUIMainRankData2.text = value;
        }
    }

    private string m_RankingUIMainRankData3 = "";
    public string RankingUIMainRankData3
    {
        get
        {
            return m_RankingUIMainRankData3;
        }
        set
        {
            m_RankingUIMainRankData3 = value;
            if (m_lblRankingUIMainRankData3 != null)
                m_lblRankingUIMainRankData3.text = value;
        }
    }

    private string m_RankingUIMainRankData4 = "";
    public string RankingUIMainRankData4
    {
        get
        {
            return m_RankingUIMainRankData4;
        }
        set
        {
            m_RankingUIMainRankData4 = value;
            if (m_lblRankingUIMainRankData4 != null)
                m_lblRankingUIMainRankData4.text = value;
        }
    }

    private int m_RankingUIMainRankData5 = 0;
    public int RankingUIMainRankData5FansCount
    {
        get
        {
            return m_RankingUIMainRankData5;
        }
        set
        {
            m_RankingUIMainRankData5 = value;

            if (m_lblRankingUIMainRankData5 != null)
            {
                // 临时变更粉丝数(只针对偶像变更)
                if (!string.IsNullOrEmpty(RankManager.Instance.NewIdolName)
                    && RankingUIMainRankData2Name.Equals(RankManager.Instance.NewIdolName))
                {
                    m_lblRankingUIMainRankData5.text = (m_RankingUIMainRankData5 + 1).ToString();
                }
                else if (!string.IsNullOrEmpty(RankManager.Instance.OldIdolName)
                    && RankingUIMainRankData2Name.Equals(RankManager.Instance.OldIdolName))
                {
                    m_lblRankingUIMainRankData5.text = (m_RankingUIMainRankData5 > 0 ? value - 1 : 0).ToString();
                }
                else
                {
                    m_lblRankingUIMainRankData5.text = m_RankingUIMainRankData5.ToString();
                }
            }
        }
    }

    #region 事件

    //void OnClick()
    //{
    //    if (RankingUIDict.RANKINGMAINDATAUP != null)
    //        RankingUIDict.RANKINGMAINDATAUP(AvatarID);
    //}

    bool m_isDragging = false;

    void OnDrag(Vector2 v)
    {
        m_isDragging = true;
    }

    void OnPress(bool isPressed)
    {
        if (!m_isDragging)
        {
            if (!isPressed) // OnClick
            {
                if (RankingUIDict.RANKINGMAINDATAUP != null)
                    RankingUIDict.RANKINGMAINDATAUP(AvatarID);
            }
        }
        else
        {
            if (!isPressed)
                m_isDragging = false;
        }
    }

    #endregion
}
