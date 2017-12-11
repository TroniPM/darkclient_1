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

public class InstanceCleanReportGrid : MonoBehaviour 
{
    #region 开放属性

    private string m_contextText = "";
    private Color32 m_contextColor = new Color32(255, 255, 255, 255);
    public void SetContent(string context, Color32 color)
    {
        m_contextText = context;
        m_contextColor = color;

		if (m_lblInstanceCleanReportGridText != null)
        {
            m_lblInstanceCleanReportGridText.text = context;
            m_lblInstanceCleanReportGridText.color = color;
        }       
    }

    private bool m_head = false;
    public bool Head
    {
        set
        {
            m_head = value;

            if (m_spInstanceCleanReportGridImg != null)
            {
                m_spInstanceCleanReportGridImg.enabled = value;
            }            
        }
    } 

    #endregion

    #region 组件定义

    public UILabel m_lblInstanceCleanReportGridText;
    public UISprite m_spInstanceCleanReportGridImg;
    Transform m_Transform;

    #endregion

    void Awake()
    {
        Initialize();
    }
    void Initialize()
    {
        m_Transform = transform;
        m_lblInstanceCleanReportGridText = m_Transform.Find("InstanceCleanReportGridText").GetComponentsInChildren<UILabel>(true)[0];
        m_spInstanceCleanReportGridImg = m_Transform.Find("InstanceCleanReportGridImg").GetComponentsInChildren<UISprite>(true)[0];
    }

    void Start()
    {
        SetContent(m_contextText, m_contextColor);
        Head = m_head;
    }
}
