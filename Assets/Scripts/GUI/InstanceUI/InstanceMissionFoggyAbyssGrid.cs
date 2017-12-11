#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：[特殊关卡-迷雾深渊]
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;

public class InstanceMissionFoggyAbyssGrid : MogoUIBehaviour 
{
    private GameObject m_goInstanceMissionFoggyAbyssGridMark;
    private GameObject m_goInstanceMissionFoggyAbyssGridMark0;
    private GameObject m_goInstanceMissionFoggyAbyssGridMark1;
    private GameObject m_goInstanceMissionFoggyAbyssGridMark2;
    private GameObject m_goInstanceMissionFoggyAbyssGridTip;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goInstanceMissionFoggyAbyssGridMark = FindTransform("InstanceMissionFoggyAbyssGridMark").gameObject;
        m_goInstanceMissionFoggyAbyssGridMark0 = FindTransform("InstanceMissionFoggyAbyssGridMark0").gameObject;
        m_goInstanceMissionFoggyAbyssGridMark1 = FindTransform("InstanceMissionFoggyAbyssGridMark1").gameObject;
        m_goInstanceMissionFoggyAbyssGridMark2 = FindTransform("InstanceMissionFoggyAbyssGridMark2").gameObject;
        m_goInstanceMissionFoggyAbyssGridTip = FindTransform("InstanceMissionFoggyAbyssGridTip").gameObject;
        
        // I18N
        FindTransform("InstanceMissionFoggyAbyssGridName").GetComponentsInChildren<UILabel>(true)[0].text 
            = LanguageData.GetContent(48513);
        FindTransform("InstanceMissionFoggyAbyssGridTipText").GetComponentsInChildren<UILabel>(true)[0].text
          = LanguageData.GetContent(48513);
    }

    #region 事件

    void OnClick()
    {
        // [特殊关卡-迷雾深渊]
        EventDispatcher.TriggerEvent(InstanceUIEvent.OnChooseFoggyAbyssGridUp);
    }

    #endregion

    /// <summary>
    /// 设置特殊副本的难度
    /// </summary>
    /// <param name="m_iMarks"></param>
    public void SetMarks(int m_iMarks)
    {
        m_goInstanceMissionFoggyAbyssGridMark0.SetActive(false);
        m_goInstanceMissionFoggyAbyssGridMark1.SetActive(false);
        m_goInstanceMissionFoggyAbyssGridMark2.SetActive(false);

        switch (m_iMarks)
        {           
            case 1:
                m_goInstanceMissionFoggyAbyssGridMark0.SetActive(true);

                m_goInstanceMissionFoggyAbyssGridMark.transform.localPosition = new Vector3(35,
                    m_goInstanceMissionFoggyAbyssGridMark.transform.localPosition.y,
                    m_goInstanceMissionFoggyAbyssGridMark.transform.localPosition.z);
                break;

            case 2:
                m_goInstanceMissionFoggyAbyssGridMark0.SetActive(true);
                m_goInstanceMissionFoggyAbyssGridMark1.SetActive(true);

                m_goInstanceMissionFoggyAbyssGridMark.transform.localPosition = new Vector3(20,
                    m_goInstanceMissionFoggyAbyssGridMark.transform.localPosition.y,
                    m_goInstanceMissionFoggyAbyssGridMark.transform.localPosition.z);
                break;

            case 3:
                m_goInstanceMissionFoggyAbyssGridMark0.SetActive(true);
                m_goInstanceMissionFoggyAbyssGridMark1.SetActive(true);
                m_goInstanceMissionFoggyAbyssGridMark2.SetActive(true);

                m_goInstanceMissionFoggyAbyssGridMark.transform.localPosition = new Vector3(0,
                    m_goInstanceMissionFoggyAbyssGridMark.transform.localPosition.y,
                    m_goInstanceMissionFoggyAbyssGridMark.transform.localPosition.z);
                break;         
        }
    }

    /// <summary>
    /// 是否显示悬浮提示
    /// </summary>
    public void ShowInstanceMissionFoggyAbyssGridTip(bool isShow)
    {
        m_goInstanceMissionFoggyAbyssGridTip.SetActive(isShow);
    }
}
