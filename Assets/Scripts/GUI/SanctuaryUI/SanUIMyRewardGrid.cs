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
using Mogo.GameData;
using Mogo.Util;

public class SanUIMyRewardGrid : MogoUIBehaviour 
{    
    private UILabel m_lblSanUIRewardGridName;
    private UILabel m_lblSanUIRewardGridNum;
    private UILabel m_lblSanUIRewardGridProgress;
    private UISprite m_spSanUIRewardGridItemFG;

    private GameObject m_goSanUIRewardGridFlagGet;
    private GameObject m_goSanUIRewardGridBtnOK;
    private GameObject m_goSanUIRewardGridNOFinishText;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);
        m_lblSanUIRewardGridName = FindTransform("SanUIRewardGridName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblSanUIRewardGridNum = FindTransform("SanUIRewardGridNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblSanUIRewardGridProgress = FindTransform("SanUIRewardGridProgress").GetComponentsInChildren<UILabel>(true)[0];
        m_spSanUIRewardGridItemFG = FindTransform("SanUIRewardGridItemFG").GetComponentsInChildren<UISprite>(true)[0];

        m_goSanUIRewardGridFlagGet = FindTransform("SanUIRewardGridFlagGet").gameObject;
        m_goSanUIRewardGridBtnOK = FindTransform("SanUIRewardGridBtnOK").gameObject;
        m_goSanUIRewardGridNOFinishText = FindTransform("SanUIRewardGridNOFinishText").gameObject;

        Initialize();
    } 

    #region 事件

    public int rewardID;

    void Initialize()
    {
        FindTransform("SanUIRewardGridBtnOK").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnOK;
    }

    void OnBtnOK()
    {
        EventDispatcher.TriggerEvent(SanctuaryUIEvent.GetMyReward, rewardID);
    }

    #endregion

    #region 界面信息

    public void SetName(string name)
    {
        m_lblSanUIRewardGridName.text = name;
    }

    public void SetNum(int num)
    {
        m_lblSanUIRewardGridNum.text = string.Format(LanguageData.GetContent(46914), num);
    }

    public void SetProgress(string progress)
    {
        m_lblSanUIRewardGridProgress.text = progress;
    }

    public void SetIcon(string icon)
    {
        m_spSanUIRewardGridItemFG.atlas = MogoUIManager.Instance.GetAtlasByIconName(icon);
        m_spSanUIRewardGridItemFG.spriteName = icon;
    }
 
    #endregion

    #region 设置领取按钮状态
  
    /// <summary>
    /// 设置领取按钮状态
    /// </summary>
    /// <param name="isEnable">是否已经可以领取</param>
    /// <param name="isAlreadyGet">是否已经领取过</param>
    public void SetState(bool isEnable, bool isAlreadyGet)
    {     
        if (isEnable)
        {
            if (isAlreadyGet)
            {
                ShowBtnOK(false);
                ShowNOFinishText(false);
                ShowFlagGet(true);
            }
            else
            {
                ShowFlagGet(false);
                ShowNOFinishText(false);
                ShowBtnOK(true);
            }
        }
        else
        {
            ShowFlagGet(false);
            ShowBtnOK(false);
            ShowNOFinishText(true);
        }
    }

    private void ShowFlagGet(bool isShow)
    {
        if (m_goSanUIRewardGridFlagGet != null)
            m_goSanUIRewardGridFlagGet.SetActive(isShow);
    }

    private void ShowNOFinishText(bool isShow)
    {
        if (m_goSanUIRewardGridNOFinishText != null)
            m_goSanUIRewardGridNOFinishText.SetActive(isShow);
    }

    private void ShowBtnOK(bool isShow)
    {
        if (m_goSanUIRewardGridBtnOK != null)
            m_goSanUIRewardGridBtnOK.SetActive(isShow);
    }

    #endregion
}
