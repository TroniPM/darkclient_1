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

public class DragonMatchRecordUIGrid : MogoUIBehaviour 
{
    private GameObject m_goDragonMatchRecordUIGridHasRevenge;
    private GameObject m_goDragonMatchRecordUIGridTip;
    private GameObject m_goDragonMatchRecordUIGridBtnRevenge;
    private UILabel m_lblDragonMatchRecordUIGridText;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goDragonMatchRecordUIGridHasRevenge = FindTransform("DragonMatchRecordUIGridHasRevenge").gameObject;
        m_goDragonMatchRecordUIGridTip = FindTransform("DragonMatchRecordUIGridTip").gameObject;
        m_goDragonMatchRecordUIGridBtnRevenge = FindTransform("DragonMatchRecordUIGridBtnRevenge").gameObject;
        m_lblDragonMatchRecordUIGridText = FindTransform("DragonMatchRecordUIGridText").GetComponentsInChildren<UILabel>(true)[0];

        Initialize();
    }

    #region  事件

    public int Index;

    private void Initialize()
    {
        m_goDragonMatchRecordUIGridBtnRevenge.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnRevengeUp;
    }

    private void OnBtnRevengeUp()
    {
        EventDispatcher.TriggerEvent<int>(DragonMatchRecordUILogicManager.DragonMatchRecordUIEvent.RevengeBtnUp, Index);
    }    

    #endregion

    /// <summary>
    /// 是否已经复仇
    /// </summary>
    public bool HasRevenged
    {
        set
        {
            if (m_goDragonMatchRecordUIGridHasRevenge != null)
                m_goDragonMatchRecordUIGridHasRevenge.SetActive(value);
        }
    }

    /// <summary>
    /// 是否可以复仇
    /// </summary>
    public bool CanRevenge
    {
        set
        {
            if (m_goDragonMatchRecordUIGridBtnRevenge != null)
                m_goDragonMatchRecordUIGridBtnRevenge.SetActive(value);
        }
    }

    /// <summary>
    /// 是否需要提示
    /// </summary>
    public bool NeedTip
    {
        set
        {
            if(m_goDragonMatchRecordUIGridTip != null)
                m_goDragonMatchRecordUIGridTip.SetActive(value);
        }
    }

    /// <summary>
    /// 设置记录信息
    /// </summary>
    public string InfoString
    {
        set
        {
            if (m_lblDragonMatchRecordUIGridText != null)
                m_lblDragonMatchRecordUIGridText.text = value;
        }
    }
}
