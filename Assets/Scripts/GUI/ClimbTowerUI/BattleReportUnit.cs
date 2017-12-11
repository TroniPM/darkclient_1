/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：BattleReportUnit
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class BattleReportUnit : MonoBehaviour 
{
    #region 开放属性
    public string content
    {
        get { return m_lblReport.text; }
        set {
            if (m_lblReport == null)
            {
                Initialize();
            } 
            m_lblReport.text = value;
        }
    }
    public bool head
    {
        set
        {
            if (m_imgReport == null)
            {
                Initialize();
            }
            m_imgReport.enabled = value;
        }
    }
    public bool line
    {
        set
        {
            if (m_bgReport == null)
            {
                Initialize();
            }
            m_bgReport.enabled = value;
        }
    }
    #endregion
    #region 组件定义
    public UILabel m_lblReport;
    public UISprite m_imgReport;
    public UISlicedSprite m_bgReport;
    Transform m_Transform;
    #endregion

    void Awake()
    {
        Initialize();
    }
    void Initialize()
    {
        m_Transform = transform;
        m_lblReport = m_Transform.Find("txtBattleReport").GetComponentsInChildren<UILabel>(true)[0];
        m_imgReport = m_Transform.Find("imgBattleReport").GetComponentsInChildren<UISprite>(true)[0];
        m_bgReport = m_Transform.Find("bgBattleReport").GetComponentsInChildren<UISlicedSprite>(true)[0];
    }
}
