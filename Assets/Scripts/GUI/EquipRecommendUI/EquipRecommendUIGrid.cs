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
using Mogo.GameData;

public class EquipRecommendUIGrid : MogoUIBehaviour 
{         
    private UISprite m_spCurrentEquipImg;
    private UISprite m_spCurrentEquipImgBG;
    private UILabel m_lblCurrentEquipText;
    private UILabel m_lblCurrentEquipScoreText;
    private GameObject m_goCurrentEquipButton;

    private UISprite m_spRecommendEquipImg;
    private UISprite m_spRecommendEquipImgBG;
    private UILabel m_lblRecommendEquipText;
    private UILabel m_lblRecommendEquipScoreText;
    private GameObject m_goRecommendEquipButton; 

    private UILabel m_lblScoreUpgradeNum;
    private GameObject m_goLinkBtn;
    private UILabel m_lblLinkBtnText;
    private EquipRecommendUIGridButton linkButton = null;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_spCurrentEquipImg = m_myTransform.Find(m_widgetToFullName["CurrentEquipImg"]).GetComponentsInChildren<UISprite>(true)[0];
        m_spCurrentEquipImgBG = m_myTransform.Find(m_widgetToFullName["CurrentEquipImgBG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_lblCurrentEquipText = m_myTransform.Find(m_widgetToFullName["CurrentEquipText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblCurrentEquipScoreText = FindTransform("CurrentEquipScoreText").GetComponentsInChildren<UILabel>(true)[0];
        m_goCurrentEquipButton = FindTransform("CurrentEquipButton").gameObject;

        m_spRecommendEquipImg = m_myTransform.Find(m_widgetToFullName["RecommendEquipImg"]).GetComponentsInChildren<UISprite>(true)[0];
        m_spRecommendEquipImgBG = m_myTransform.Find(m_widgetToFullName["RecommendEquipImgBG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_lblRecommendEquipText = m_myTransform.Find(m_widgetToFullName["RecommendEquipText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblRecommendEquipScoreText = FindTransform("RecommendEquipScoreText").GetComponentsInChildren<UILabel>(true)[0];
        m_goRecommendEquipButton = FindTransform("RecommendEquipButton").gameObject;

        m_lblScoreUpgradeNum = m_myTransform.Find(m_widgetToFullName["ScoreUpgradeNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goLinkBtn = m_myTransform.Find(m_widgetToFullName["LinkBtn"]).gameObject;
        m_lblLinkBtnText = m_myTransform.Find(m_widgetToFullName["LinkBtnText"]).GetComponentsInChildren<UILabel>(true)[0];
        linkButton = m_myTransform.Find(m_widgetToFullName["LinkBtn"]).gameObject.AddComponent<EquipRecommendUIGridButton>();
    }    

    #region 事件

    #endregion

    private int m_access;
    private int m_accessType;
    public void SetEquipRecommendAccess(int access, int accessType)
    {
        m_access = access;
        m_accessType = accessType;

        // access_i<10000的 材料兑换  accsee_i>=10000&&access_i<20000  副本掉落  access_i>=20000 试炼之塔
        if (m_access < 10000)
            SetLinkBtnText(LanguageData.GetContent(47700));
        else if (m_access >= 10000 && m_access < 20000)
            SetLinkBtnText(LanguageData.GetContent(47701));
        else if (m_access >= 20000 && m_access < 30000)
            SetLinkBtnText(LanguageData.GetContent(47702));
        else if(m_access >= 30000)
            SetLinkBtnText(LanguageData.GetContent(47704));

        if (linkButton == null)
            linkButton = m_myTransform.Find(m_widgetToFullName["LinkBtn"]).gameObject.AddComponent<EquipRecommendUIGridButton>();
        linkButton.Access = m_access;
        linkButton.AccessType = m_accessType;
    }

    /// <summary>
    /// 设置当前装备ID
    /// </summary>
    /// <param name="item_id"></param>
    public void SetCurrentEquipID(int item_id)
    {
        if (m_goCurrentEquipButton != null)
        {
            m_goCurrentEquipButton.GetComponentsInChildren<InventoryGrid>(true)[0].iconID = item_id;
        }        
    }

    /// <summary>
    /// 设置当前装备信息
    /// </summary>
    /// <param name="iconName"></param>
    /// <param name="color"></param>
    /// <param name="quality"></param>
    public void SetCurrentEquip(string iconName, int color = 0, int quality = 1)
    {
        string qulityIcon = IconData.GetIconByQuality(quality);
        SetCurrentEquipFG(iconName, color);
        SetCurrentEquipBG(qulityIcon);
    }

    private void SetCurrentEquipFG(string imgName, int color = 0)
    {
        if (m_spCurrentEquipImg != null)
        {
            m_spCurrentEquipImg.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            m_spCurrentEquipImg.spriteName = imgName;
            MogoUtils.SetImageColor(m_spCurrentEquipImg, color);
        }
    }

    private void SetCurrentEquipBG(string imgName, int color = 0)
    {
        if (m_spCurrentEquipImgBG != null)
        {
            m_spCurrentEquipImgBG.spriteName = imgName;
            MogoUtils.SetImageColor(m_spCurrentEquipImgBG, color);
        }
    }

    /// <summary>
    /// 设置当前装备名称
    /// </summary>
    /// <param name="name"></param>
    public void SetCurrentEquipName(string name, bool bOutline = false)
    {
        if (m_lblCurrentEquipText != null)
        {
            m_lblCurrentEquipText.text = name;
            if (bOutline)
            {
                m_lblCurrentEquipText.effectStyle = UILabel.Effect.Outline;
                m_lblCurrentEquipText.effectColor = new Color32(50, 39, 9, 255);
                m_lblCurrentEquipText.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                m_lblCurrentEquipText.effectStyle = UILabel.Effect.None;
                m_lblCurrentEquipText.color = new Color32(63, 27, 4, 255);
            }
        }
    }

    /// <summary>
    /// 设置当前装备得分
    /// </summary>
    /// <param name="score"></param>
    public void SetCurrentEquipScore(int score)
    {
        if (m_lblCurrentEquipScoreText != null)
            m_lblCurrentEquipScoreText.text = string.Format(LanguageData.GetContent(47703), score);
    }    

    /// <summary>
    /// 设置推荐装备ID
    /// </summary>
    /// <param name="item_id"></param>
    public void SetRecommendEquipID(int item_id)
    {
        if (m_goRecommendEquipButton != null)
            m_goRecommendEquipButton.GetComponentsInChildren<InventoryGrid>(true)[0].iconID = item_id;
    }

    /// <summary>
    /// 设置推荐装备信息
    /// </summary>
    /// <param name="iconName"></param>
    /// <param name="color"></param>
    /// <param name="quality"></param>
    public void SetRecommendEquip(string iconName, int color = 0, int quality = 1)
    {
        string qulityIcon = IconData.GetIconByQuality(quality);
        SetRecommendEquiptFG(iconName, color);
        SetRecommendEquipBG(qulityIcon);
    }

    private void SetRecommendEquiptFG(string imgName, int color = 0)
    {
        if (m_spRecommendEquipImg != null)
        {
            m_spRecommendEquipImg.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            m_spRecommendEquipImg.spriteName = imgName;
            MogoUtils.SetImageColor(m_spRecommendEquipImg, color);
        }
    }

    private void SetRecommendEquipBG(string imgName, int color = 0)
    {
        if (m_spRecommendEquipImgBG != null)
        {
            m_spRecommendEquipImgBG.spriteName = imgName;
            MogoUtils.SetImageColor(m_spRecommendEquipImgBG, color);
        }
    }

    /// <summary>
    /// 设置推荐装备名称
    /// </summary>
    /// <param name="name"></param>
    /// <param name="bOutline"></param>
    public void SetRecommendEquipName(string name, bool bOutline = false)
    {
        if (m_lblRecommendEquipText != null)
        {
            m_lblRecommendEquipText.text = name;
            if (bOutline)
            {
                m_lblRecommendEquipText.effectStyle = UILabel.Effect.Outline;
                m_lblRecommendEquipText.effectColor = new Color32(50, 39, 9, 255);
                m_lblRecommendEquipText.color = new Color32(255, 255, 255, 255);
            }
            else
            {
                m_lblRecommendEquipText.effectStyle = UILabel.Effect.None;
                m_lblRecommendEquipText.color = new Color32(63, 27, 4, 255);
            }
        }
    }

    /// <summary>
    /// 设置当前装备得分
    /// </summary>
    /// <param name="score"></param>
    public void SetRecommendEquipScore(int score)
    {
        if (m_lblRecommendEquipScoreText != null)
        {
            m_lblRecommendEquipScoreText.text = string.Format(LanguageData.GetContent(47703), score);
        }
    }    

    /// <summary>
    /// 设置可以提升分数
    /// </summary>
    /// <param name="recommendScore"></param>
    /// <param name="currentScore"></param>
    public void SetScoreUpgradeNum(int recommendScore, int currentScore)
    {
        if (m_lblScoreUpgradeNum != null)
        {
            int upgradeScore = recommendScore - currentScore;
            if (upgradeScore > 0)
                m_lblScoreUpgradeNum.text = string.Concat("+", upgradeScore);
            else
                m_lblScoreUpgradeNum.text = upgradeScore.ToString();
        }
    }

    private void SetLinkBtnText(string name)
    {
        if (m_lblLinkBtnText != null)
        {
            m_lblLinkBtnText.text = name;
        }
    }
}
