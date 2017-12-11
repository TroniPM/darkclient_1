/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：InstanceLevelGrid
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.GameData;

public class NewInstanceGrid : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public int id = -1;
    private Transform m_myTransform;

    private UISprite m_spIconFG;
    private UISprite m_spMark0;
    private UISprite m_spMark1;

    private GameObject m_goGridTip;
    private UILabel m_lblNewInstanceUIChooseLevelGridTipText;

    void Awake()
    {
        m_myTransform = transform;

        m_spIconFG = m_myTransform.parent.Find("NewInstanceUIChooseLevelGridIcon/NewInstanceUIChooseLevelGridIconUp").GetComponentsInChildren<UISprite>(true)[0];
        m_spMark0 = m_myTransform.parent.Find("NewInstanceUIChooseLevelGridMarkList/NewInstanceUIChooseLevelGridMark0/NewInstanceUIChooseLevelGridMark0FG").GetComponentsInChildren<UISprite>(true)[0];
        m_spMark1 = m_myTransform.parent.Find("NewInstanceUIChooseLevelGridMarkList/NewInstanceUIChooseLevelGridMark1/NewInstanceUIChooseLevelGridMark1FG").GetComponentsInChildren<UISprite>(true)[0];

        m_goGridTip = m_myTransform.parent.Find("NewInstanceUIChooseLevelGridTip").gameObject;
        m_lblNewInstanceUIChooseLevelGridTipText = m_myTransform.parent.Find("NewInstanceUIChooseLevelGridTip/NewInstanceUIChooseLevelGridTipText").GetComponentsInChildren<UILabel>(true)[0];
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(m_strIconName))
            SetIcon(m_strIconName);
        ShowGridTip(m_bCurrentShow, TipTextID);

        if (m_iMark0 != -1)
        {
            ShowMarks(m_iMark0, m_iMark1);
            ShowGridTip(m_bCurrentShow, TipTextID);
        }

        EventDispatcher.TriggerEvent("InstanceGridAwakeEnd");
    }

    #region 事件

    void OnClick()
    {
        if (id < InstanceMapWithMissionGrid.FoggyAbyssMissionIndex)
        {
            // [普通关卡]
            EventDispatcher.TriggerEvent(InstanceUIEvent.OnChooseNormalGridUp, id);            
        }
        else if (id == InstanceMapWithMissionGrid.FoggyAbyssMissionIndex)
        {
            // [特殊关卡-迷雾深渊]
            EventDispatcher.TriggerEvent(InstanceUIEvent.OnChooseFoggyAbyssGridUp);            
        }        
    }

    #endregion

    #region 界面信息

    public void ShowStars(int num)
    {
        m_spMark0.spriteName = "fb_s";
        m_spMark1.spriteName = "fb_b";
    }

    string m_strIconName = "";
    public void SetIcon(string iconName)
    {
        if (m_spIconFG == null)
        {
            m_strIconName = iconName;
        }
        else
        {
            m_spIconFG.spriteName = iconName;
        }
    }

    public void SetEnable(bool isEnable)
    {
        gameObject.transform.parent.gameObject.SetActive(isEnable);
    }

    /// <summary>
    /// 显示副本提示文本
    /// </summary>
    private bool m_bCurrentShow = false;
    private int TipTextID = 0;
    public void ShowGridTip(bool isShow, int tipTextID = 0)
    {
        TipTextID = tipTextID;
        if (TipTextID == 0)
            TipTextID = 46979;

        if (m_goGridTip == null)
        {
            m_bCurrentShow = isShow;
        }
        else
        {
            m_bCurrentShow = isShow;
            m_goGridTip.SetActive(isShow);

            if (isShow)
                m_lblNewInstanceUIChooseLevelGridTipText.text = LanguageData.GetContent(TipTextID);
        }
    }

    #endregion

    #region 设置星级

    /// <summary>
    /// 设置星级
    /// </summary>
    public void ShowMarksByComplexData(int complexData)
    {
        int mark0 = complexData / 10;
        int mark1 = complexData % 10;

        ShowMarks(mark0, mark1);
    }

    /// <summary>
    /// 设置星级
    /// </summary>
    int m_iMark0 = -1;
    int m_iMark1 = -1;
    public void ShowMarks(int mark0, int mark1)
    {
        if (m_spMark0 == null)
        {
            m_iMark0 = mark0;
            m_iMark1 = mark1;
        }
        else
        {
            switch (mark0)
            {
                case 0:
                    m_spMark0.gameObject.SetActive(false);
                    break;

                case 1:
                    m_spMark0.gameObject.SetActive(true);
                    m_spMark0.spriteName = "fb_c";
                    break;

                case 2:
                    m_spMark0.gameObject.SetActive(true);
                    m_spMark0.spriteName = "fb_b";
                    break;

                case 3:
                    m_spMark0.gameObject.SetActive(true);
                    m_spMark0.spriteName = "fb_a";
                    break;

                case 4:
                    m_spMark0.gameObject.SetActive(true);
                    m_spMark0.spriteName = "fb_s";
                    break;
            }

            switch (mark1)
            {
                case 0:
                    m_spMark1.gameObject.SetActive(false);
                    m_spMark1.transform.parent.parent.localPosition = new Vector3(25, 0, 0);
                    break;

                case 1:
                    m_spMark1.gameObject.SetActive(true);
                    m_spMark1.spriteName = "fb_c";
                    m_spMark1.transform.parent.parent.localPosition = Vector3.zero;
                    break;

                case 2:
                    m_spMark1.gameObject.SetActive(true);
                    m_spMark1.spriteName = "fb_b";
                    m_spMark1.transform.parent.parent.localPosition = Vector3.zero;
                    break;

                case 3:
                    m_spMark1.gameObject.SetActive(true);
                    m_spMark1.spriteName = "fb_a";
                    m_spMark1.transform.parent.parent.localPosition = Vector3.zero;
                    break;

                case 4:
                    m_spMark1.gameObject.SetActive(true);
                    m_spMark1.spriteName = "fb_s";
                    m_spMark1.transform.parent.parent.localPosition = Vector3.zero;
                    break;
            }
        }
    }

    #endregion   
}
