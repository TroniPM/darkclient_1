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
using System;

public class OKCancelTipUIViewManager : MogoUIBehaviour 
{
    private static OKCancelTipUIViewManager m_instance;
    public static OKCancelTipUIViewManager Instance
    {
        get { return OKCancelTipUIViewManager.m_instance; }
    }

    // 界面信息
    private UILabel m_lblOKCancelTipUIInfoText;
    // 是否不再提示Check
    private UILabel m_lblOKCancelTipUIBtnTipText;
    private GameObject m_goOKCancelTipUIBtnTipBGDown;
    // 右边按钮
    private UILabel m_lblOKCancelTipUIBtnOKText;
    private UISprite m_spOKCancelTipUIBtnOKBGUp;
    private UISprite m_spOKCancelTipUIBtnOKBGDown;
    // 左边按钮
    private UILabel m_lblOKCancelTipUIBtnCancelText;
    private UISprite m_spOKCancelTipUIBtnCancelBGUp;
    private UISprite m_spOKCancelTipUIBtnCancelBGDown;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;
		
		m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblOKCancelTipUIInfoText = FindTransform("OKCancelTipUIInfoText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOKCancelTipUIBtnTipText = FindTransform("OKCancelTipUIBtnTipText").GetComponentsInChildren<UILabel>(true)[0];
        m_goOKCancelTipUIBtnTipBGDown = FindTransform("OKCancelTipUIBtnTipBGDown").gameObject;
        m_lblOKCancelTipUIBtnOKText = FindTransform("OKCancelTipUIBtnOKText").GetComponentsInChildren<UILabel>(true)[0];
        m_spOKCancelTipUIBtnOKBGUp = FindTransform("OKCancelTipUIBtnOKBGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_spOKCancelTipUIBtnOKBGDown = FindTransform("OKCancelTipUIBtnOKBGDown").GetComponentsInChildren<UISprite>(true)[0];
        m_lblOKCancelTipUIBtnCancelText = FindTransform("OKCancelTipUIBtnCancelText").GetComponentsInChildren<UILabel>(true)[0];
        m_spOKCancelTipUIBtnCancelBGUp = FindTransform("OKCancelTipUIBtnCancelBGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_spOKCancelTipUIBtnCancelBGDown = FindTransform("OKCancelTipUIBtnCancelBGDown").GetComponentsInChildren<UISprite>(true)[0];

        Initialize();
    }

    #region 事件
   
    private Action<bool, bool> m_actionCallback;

    void Initialize()
    {
        FindTransform("OKCancelTipUIBtnOK").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnOKUp;
        FindTransform("OKCancelTipUIBtnCancel").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnCancelUp;
        FindTransform("OKCancelTipUIBtnTip").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnTipUp;
    }

    public void Release()
    {

    }

    // 右边按钮
    private void OnBtnOKUp()
    {
        MogoUIManager.Instance.ShowOKCancelTipUI(null, false);
        if (m_actionCallback != null)
            m_actionCallback(true, IsNoTip);
    }

    // 左边按钮
    private void OnBtnCancelUp()
    {
        MogoUIManager.Instance.ShowOKCancelTipUI(null, false);
        if (m_actionCallback != null)
            m_actionCallback(false, IsNoTip);
    }

    // Check按钮
    private void OnBtnTipUp()
    {
        IsNoTip = !IsNoTip;
    }

    #endregion

    #region  界面信息

    /// <summary>
    /// false:提示；true:不再提示
    /// </summary>
    private bool m_IsNoTip = false;
    private bool IsNoTip
    {
        get{ return m_IsNoTip; }
        set
        {
            m_IsNoTip = value;

            if (m_goOKCancelTipUIBtnTipBGDown != null)
            {
                if (m_IsNoTip)
                    m_goOKCancelTipUIBtnTipBGDown.SetActive(true);
                else
                    m_goOKCancelTipUIBtnTipBGDown.SetActive(false);
            }           
        }     
    }
    

    /// <summary>
    /// 设置回调
    /// </summary>
    /// <param name="action"></param>
    public void SetCallback(Action<bool, bool> action)
    {
        m_actionCallback = action;
    }

    /// <summary>
    /// 设置右边按钮背景
    /// </summary>
    /// <param name="OKBgType"></param>
    public void SetBtnOKBg(ButtonBgType OKBgType = ButtonBgType.Blue)
    {
        MogoGlobleUIManager.SetButtonBg(m_spOKCancelTipUIBtnOKBGUp, m_spOKCancelTipUIBtnOKBGDown, OKBgType);
    }

    /// <summary>
    /// 设置左边按钮背景
    /// </summary>
    /// <param name="CancelBgType"></param>
    public void SetBtnCancelBg(ButtonBgType CancelBgType = ButtonBgType.Brown)
    {
        MogoGlobleUIManager.SetButtonBg(m_spOKCancelTipUIBtnCancelBGUp, m_spOKCancelTipUIBtnCancelBGDown, CancelBgType);
    }

    /// <summary>
    /// 设置右边按钮文本
    /// </summary>
    /// <param name="OKText"></param>
    public void SetBtnOKText(string OKText)
    {
        m_lblOKCancelTipUIBtnOKText.text = OKText;
    }

    /// <summary>
    /// 设置左边按钮文本
    /// </summary>
    /// <param name="CancelText"></param>
    public void SetBtnCancelText(string CancelText)
    {
        m_lblOKCancelTipUIBtnCancelText.text = CancelText;
    }

    /// <summary>
    /// 设置提示框信息
    /// </summary>
    /// <param name="infoText"></param>
    public void SetOKCancelInfo(string infoText)
    {
        m_lblOKCancelTipUIInfoText.text = infoText;
    }
 
    #endregion

    #region 界面打开和关闭

    protected override void OnEnable()
    {
        base.OnEnable();
        IsNoTip = false;
    }

    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI && false)
        {
            MogoUIManager.Instance.DestroyOKCancelTipUI();
        }
    }

    #endregion
}
