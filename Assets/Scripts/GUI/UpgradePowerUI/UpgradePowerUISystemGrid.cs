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
using System.Collections.Generic;
using Mogo.Util;

public class UpgradePowerUISystemGrid : MogoUIBehaviour 
{
    // 进度条和星
    private GameObject m_goGOUpgradePowerUIScore;
    private UISprite m_spProgressFG;
    private GameObject m_goGOStar;
    private List<GameObject> m_listStar = new List<GameObject>();
    // 提升系统图标
    private UISprite m_spSystemIconFG;
    private UILabel m_lblSystemIconText;
    // 推荐提示
    private GameObject m_goUpgradePowerUISystemGridTip;
    // 系统未开启
    private GameObject m_goGOUpgradePowerUINoOpen;
    private UILabel m_lblUpgradePowerUINoOpenText;

    void Awake()
    {
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goGOUpgradePowerUIScore = m_myTransform.Find(m_widgetToFullName["GOUpgradePowerUIScore"]).gameObject;
        m_spProgressFG = m_myTransform.Find(m_widgetToFullName["ProgressFG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_goGOStar = m_myTransform.Find(m_widgetToFullName["GOStar"]).gameObject;
        m_listStar.Add(m_myTransform.Find(m_widgetToFullName["Star1"]).gameObject);
        m_listStar.Add(m_myTransform.Find(m_widgetToFullName["Star2"]).gameObject);
        m_listStar.Add(m_myTransform.Find(m_widgetToFullName["Star3"]).gameObject);
        m_spSystemIconFG = m_myTransform.Find(m_widgetToFullName["SystemIconFG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_lblSystemIconText = m_myTransform.Find(m_widgetToFullName["SystemIconText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goUpgradePowerUISystemGridTip = m_myTransform.Find(m_widgetToFullName["UpgradePowerUISystemGridTip"]).gameObject;
        m_goGOUpgradePowerUINoOpen = m_myTransform.Find(m_widgetToFullName["GOUpgradePowerUINoOpen"]).gameObject;
        m_lblUpgradePowerUINoOpenText = m_myTransform.Find(m_widgetToFullName["UpgradePowerUINoOpenText"]).GetComponentsInChildren<UILabel>(true)[0];
    }

    void Start()
    {
        StarNum = StarNum;
        SystemName = SystemName;
        SystemIconName = SystemIconName;
    }

    #region 事件

    private bool m_bSystemOpen = false;
    private string m_noOpenFloatText = "";
    void OnClick()
    {
        if (!m_bSystemOpen)
        {
            MogoMsgBox.Instance.ShowFloatingText(m_noOpenFloatText);
            return;
        }

        if (UpgradePowerUIDict.SYSTEMGRIDUP != null)
            UpgradePowerUIDict.SYSTEMGRIDUP(XMLID);
    }

    #endregion

    private int m_xmlID;
    public int XMLID
    {
        get { return m_xmlID; }
        set
        {
            m_xmlID = value;
        }
    }

    /// <summary>
    /// 设置星级
    /// </summary>
    private int m_starNum;
    public int StarNum
    {
        get{ return m_starNum;}
        set
        {
            m_starNum = value;
            if (m_listStar.Count == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i < m_starNum)
                        m_listStar[i].SetActive(true);
                    else
                        m_listStar[i].SetActive(false);
                }
            }           
        }       
    }

    /// <summary>
    /// 提升系统名称
    /// </summary>
    private string m_systemName;
    public string SystemName
    {
        get { return m_systemName; }
        set
        {
            m_systemName = value;
            if (m_lblSystemIconText != null)
                m_lblSystemIconText.text = m_systemName;
        }
    }

    /// <summary>
    /// 提升系统图标
    /// </summary>
    private string m_systemIconName;
    public string SystemIconName
    {
        get { return m_systemIconName; }
        set
        {
            m_systemIconName = value;
            if (m_spSystemIconFG != null)
            {
                
                if (m_spSystemIconFG.atlas != null && m_spSystemIconFG.atlas.GetSprite(value) != null)
                {
                    m_spSystemIconFG.spriteName = value;
                }
                else
                {
                    MogoUIManager.Instance.TryingSetSpriteName(value, m_spSystemIconFG);
                    //m_spSystemIconFG.atlas = MogoUIManager.Instance.GetAtlasByIconName(value);
                    //m_spSystemIconFG.spriteName = value;
                }
            }
        }
    }

    /// <summary>
    /// 进度值
    /// </summary>
    private float m_progressValue;
    public float ProgressValue
    {
        get { return m_progressValue; }
        set
        {
            m_progressValue = value;
            if (m_spProgressFG != null)
            {
                m_spProgressFG.fillAmount = m_progressValue;
                if (m_spProgressFG.fillAmount <= 0)
                    m_spProgressFG.fillAmount = 0.00001f;
            }
        }
    }

    /// <summary>
    /// 提升系统已开启设置
    /// </summary>
    /// <param name="starNum"></param>
    /// <param name="progressValue"></param>
    public void SetSystemHasOpen(int starNum, float progressValue)
    {
        m_bSystemOpen = true;
        ShowGOUpgradePowerUIScore(true);
        ShowGOUpgradePowerUINoOpen(false);
        StarNum = starNum;
        ProgressValue = progressValue;
    }    

    /// <summary>
    /// 提升系统未开启设置
    /// </summary>
    /// <param name="noOpenText"></param>
    public void SetSystemNoOpen(string noOpenText, string noOpenFloatText)
    {
        m_bSystemOpen = false;
        ShowGOUpgradePowerUIScore(false);
        ShowGOUpgradePowerUINoOpen(true);
        SetUpgradePowerUINoOpenText(noOpenText);
        m_noOpenFloatText = noOpenFloatText;
    }

    /// <summary>
    /// 是否推荐
    /// </summary>
    /// <param name="isRecommend"></param>
    public void SetSystemIsRecommend(bool isRecommend)
    {
        if (m_goUpgradePowerUISystemGridTip == null)
            m_goUpgradePowerUISystemGridTip = m_myTransform.Find(m_widgetToFullName["UpgradePowerUISystemGridTip"]).gameObject;
        m_goUpgradePowerUISystemGridTip.SetActive(isRecommend);
    }

    private void SetUpgradePowerUINoOpenText(string text)
    {
        if(m_lblUpgradePowerUINoOpenText == null)
            m_lblUpgradePowerUINoOpenText = m_myTransform.Find(m_widgetToFullName["UpgradePowerUINoOpenText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblUpgradePowerUINoOpenText.text = text;
    }

    private void ShowGOUpgradePowerUIScore(bool isShow)
    {
        if (m_goGOUpgradePowerUIScore == null)
            m_goGOUpgradePowerUIScore = m_myTransform.Find(m_widgetToFullName["GOUpgradePowerUIScore"]).gameObject;
        m_goGOUpgradePowerUIScore.SetActive(isShow);
    }

    private void ShowGOUpgradePowerUINoOpen(bool isShow)
    {
        if (m_goGOUpgradePowerUINoOpen == null)
            m_goGOUpgradePowerUINoOpen = m_myTransform.Find(m_widgetToFullName["GOUpgradePowerUINoOpen"]).gameObject;
        m_goGOUpgradePowerUINoOpen.SetActive(isShow);
    }
}
