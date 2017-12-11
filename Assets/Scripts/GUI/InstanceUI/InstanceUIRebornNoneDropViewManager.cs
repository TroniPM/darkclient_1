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
using System;
using Mogo.GameData;

public class InstanceUIRebornNoneDropViewManager : MogoUIBehaviour 
{
	private static InstanceUIRebornNoneDropViewManager m_instance;
    public static InstanceUIRebornNoneDropViewManager Instance { get { return InstanceUIRebornNoneDropViewManager.m_instance;}}

    private GameObject m_tranInstanceRebornNoneDropUI;

    private GameObject m_instanceRebornUIDeadth;
    private GameObject m_instanceRebornUIDeadthStoneNoEnough;
    private GameObject m_instanceRebornUIDeadthTimesZero;
    private GameObject m_instanceRebornUICantReborn;

    private GameObject m_instanceRebornUILeft;
    private UILabel m_instanceRebornUILeftText;
    private GameObject m_instanceRebornUIReborn;

    private Transform m_instanceButtonPosLeft;
    private Transform m_instanceButtonPosCenter;
    private Transform m_instanceButtonPosRight;

    // 复活石不足
    private UILabel m_lblDeadthStoneNoEnoughText1;
    private UILabel m_lblDeadthStoneNoEnoughText2;

    // 不可复活
    private UILabel m_lblCantRebornText;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranInstanceRebornNoneDropUI = gameObject;
		
		m_instanceRebornUIDeadth = m_myTransform.Find(m_widgetToFullName["InstanceRebornUIDeadth"]).gameObject;
		m_instanceRebornUIDeadthStoneNoEnough = m_myTransform.Find(m_widgetToFullName["InstanceRebornUIDeadthStoneNoEnough"]).gameObject;
 		m_instanceRebornUIDeadthTimesZero = m_myTransform.Find(m_widgetToFullName["InstanceRebornUIDeadthTimesZero"]).gameObject;
        m_instanceRebornUICantReborn = FindTransform("InstanceRebornUICantReborn").gameObject;
		
		m_instanceRebornUILeft = m_myTransform.Find(m_widgetToFullName["InstanceRebornUILeft"]).gameObject;
		m_instanceRebornUILeftText = m_myTransform.Find(m_widgetToFullName["InstanceRebornUILeftText"]).GetComponent<UILabel>();
		m_instanceRebornUIReborn = m_myTransform.Find(m_widgetToFullName["InstanceRebornUIReborn"]).gameObject;
		
		m_instanceButtonPosLeft = m_myTransform.Find(m_widgetToFullName["InstanceButtonPosLeft"]);
		m_instanceButtonPosCenter = m_myTransform.Find(m_widgetToFullName["InstanceButtonPosCenter"]);
		m_instanceButtonPosRight = m_myTransform.Find(m_widgetToFullName["InstanceButtonPosRight"]);

        m_lblDeadthStoneNoEnoughText1 = m_myTransform.Find(m_widgetToFullName["InstanceRebornUIDeadthStoneNoEnoughText1"]).GetComponent<UILabel>();
        m_lblDeadthStoneNoEnoughText2 = m_myTransform.Find(m_widgetToFullName["InstanceRebornUIDeadthStoneNoEnoughText2"]).GetComponent<UILabel>();

        m_lblCantRebornText = FindTransform("InstanceRebornUICantRebornText").GetComponentsInChildren<UILabel>(true)[0];

        // ChineseData
        m_lblDeadthStoneNoEnoughText2.text = LanguageData.GetContent(1105); // 小提示
        m_lblCantRebornText.text = LanguageData.GetContent(48517); // 不可复活

        Initialize();
    }

    #region 事件

    public Action INSTANCEREBORNLEFTUP;
    public Action INSTANCEREBORNUP;

    public void Initialize()
    {
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceRebornUILeft", OnRebornUILeftUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceRebornUIReborn", OnRebornUIRebornUp);

        InstanceUIRebornNoneDropLogicManager.Instance.Initialize();
    }

    public void Release()
    {
        InstanceUIRebornNoneDropLogicManager.Instance.Release();
    }

    void OnRebornUILeftUp(int i)
    {
        if (INSTANCEREBORNLEFTUP != null)
            INSTANCEREBORNLEFTUP();
    }

    void OnRebornUIRebornUp(int i)
    {
        if (INSTANCEREBORNUP != null)
        {
            INSTANCEREBORNUP();
        }
    }

    #endregion 
	
    public enum RebornUIState
    {
        /// <summary>
        /// 死亡，可以复活。
        /// </summary>
        Death,
        /// <summary>
        /// 复活次数用尽，无法复活。
        /// </summary>
        TimesZero,
        /// <summary>
        /// 复活石不足，无法复活。
        /// </summary>
        StoneNoEnough,
        /// <summary>
        /// 硬性要求，不能复活。
        /// </summary>
        CantReborn
    }

	public void ChangeRebornUIState(RebornUIState state)
	{
        m_instanceRebornUIDeadth.SetActive(false);
        m_instanceRebornUIDeadthStoneNoEnough.SetActive(false);
        m_instanceRebornUIDeadthTimesZero.SetActive(false);
        m_instanceRebornUICantReborn.SetActive(false);

		switch(state)
		{
		case RebornUIState.Death:
		{
			m_instanceRebornUIDeadth.SetActive(true);			
			m_instanceRebornUILeft.transform.localPosition = m_instanceButtonPosLeft.localPosition;
			m_instanceRebornUILeftText.text = LanguageData.dataMap[23002].content;
			m_instanceRebornUILeft.SetActive(true);
			m_instanceRebornUIReborn.SetActive(true);
		}break;
		case RebornUIState.TimesZero:
		{
			m_instanceRebornUIDeadthTimesZero.SetActive(true);			
			m_instanceRebornUILeft.transform.localPosition = m_instanceButtonPosCenter.localPosition;
			m_instanceRebornUILeftText.text = LanguageData.dataMap[23001].content;
			m_instanceRebornUILeft.SetActive(true);
			m_instanceRebornUIReborn.SetActive(false);
			
		}break;
		case RebornUIState.StoneNoEnough:
		{
			m_instanceRebornUIDeadthStoneNoEnough.SetActive(true);			
			m_instanceRebornUILeft.transform.localPosition = m_instanceButtonPosCenter.localPosition;
			m_instanceRebornUILeftText.text = LanguageData.dataMap[23001].content;
			m_instanceRebornUILeft.SetActive(true);
			m_instanceRebornUIReborn.SetActive(false);
		}break;
        case RebornUIState.CantReborn:
            {
                m_instanceRebornUICantReborn.SetActive(true);
                m_instanceRebornUILeft.transform.localPosition = m_instanceButtonPosCenter.localPosition;
                m_instanceRebornUILeftText.text = LanguageData.dataMap[23001].content;
                m_instanceRebornUILeft.SetActive(true);
                m_instanceRebornUIReborn.SetActive(false);
            } break;
		default:break;
		}
	}
}
