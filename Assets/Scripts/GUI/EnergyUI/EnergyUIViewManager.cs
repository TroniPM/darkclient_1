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
using Mogo.Game;

public class EnergyUIViewManager : MogoUIBehaviour 
{	
    private static EnergyUIViewManager m_instance;
    public static EnergyUIViewManager Instance { get { return EnergyUIViewManager.m_instance; } }

    private GameObject m_goEnergyUIBtnBuyAll;
    private GameObject m_goEnergyUIBtnBuyOne;
    private UILabel m_lblEnergyUIBtnBuyAllText2;
    private UILabel m_lblEnergyUIBtnBuyOneText2;
    private UISprite m_spEnergyUIBtnBuyAllBGUp;
    private UISprite m_spEnergyUIBtnBuyOneBGUp;

    private UILabel m_lblEnergyUITipAddTimesText;
    private UILabel m_lblEnergyUITipLastTimesText;
    private UILabel m_lblEnergyUITipRefreshText;
    private UILabel m_lblEnergyUIProgressBarText;

    private GameObject m_goEnergyUIBtnCharge;

    private UISprite m_spEnergyUIVIPLevelImgVIPLevel;
    private UISprite m_spEnergyUIVIPLevelImgVIPLevel2;
    private GameObject m_goEnergyUIProgressBar;

    private GameObject m_goGOEnergyUIUseOKCancel;
    private UILabel m_lblEnergyUIUseInfoDiamondNum;
    private UILabel m_lblEnergyUIUseInfoEnergyNum;
    private GameObject m_goEnergyUIUseTipEnableBGDown;

    private UILabel m_lblEnergyUIDiamondNum;

    private Transform m_tranEnergyUIBtnCharge; // 充值按钮

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goEnergyUIBtnBuyAll = FindTransform("EnergyUIBtnBuyAll").gameObject;
        m_goEnergyUIBtnBuyOne = FindTransform("EnergyUIBtnBuyOne").gameObject;
        m_lblEnergyUIBtnBuyAllText2 = FindTransform("EnergyUIBtnBuyAllText2").GetComponent<UILabel>();
        m_lblEnergyUIBtnBuyOneText2 = FindTransform("EnergyUIBtnBuyOneText2").GetComponent<UILabel>();
        m_spEnergyUIBtnBuyAllBGUp = FindTransform("EnergyUIBtnBuyAllBGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_spEnergyUIBtnBuyOneBGUp = FindTransform("EnergyUIBtnBuyOneBGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_lblEnergyUITipAddTimesText = m_myTransform.Find(m_widgetToFullName["EnergyUITipAddTimesText"]).GetComponent<UILabel>();
        m_lblEnergyUITipLastTimesText = m_myTransform.Find(m_widgetToFullName["EnergyUITipLastTimesText"]).GetComponent<UILabel>();
        m_lblEnergyUITipRefreshText = m_myTransform.Find(m_widgetToFullName["EnergyUITipRefreshText"]).GetComponent<UILabel>();
        m_lblEnergyUIProgressBarText = m_myTransform.Find(m_widgetToFullName["EnergyUIProgressBarText"]).GetComponent<UILabel>();

        m_goEnergyUIBtnCharge = m_myTransform.Find(m_widgetToFullName["EnergyUIBtnCharge"]).gameObject;

        m_spEnergyUIVIPLevelImgVIPLevel = m_myTransform.Find(m_widgetToFullName["EnergyUIVIPLevelImgVIPLevel"]).GetComponent<UISprite>();
        m_spEnergyUIVIPLevelImgVIPLevel2 = m_myTransform.Find(m_widgetToFullName["EnergyUIVIPLevelImgVIPLevel2"]).GetComponent<UISprite>();

        m_goEnergyUIProgressBar = m_myTransform.Find(m_widgetToFullName["EnergyUIProgressBar"]).gameObject; ;

        m_goGOEnergyUIUseOKCancel = m_myTransform.Find(m_widgetToFullName["GOEnergyUIUseOKCancel"]).gameObject;
        m_lblEnergyUIUseInfoDiamondNum = m_myTransform.Find(m_widgetToFullName["EnergyUIUseInfoDiamondNum"]).GetComponent<UILabel>();
        m_lblEnergyUIUseInfoEnergyNum = m_myTransform.Find(m_widgetToFullName["EnergyUIUseInfoEnergyNum"]).GetComponent<UILabel>();
        m_goEnergyUIUseTipEnableBGDown = m_myTransform.Find(m_widgetToFullName["EnergyUIUseTipEnableBGDown"]).gameObject;

        m_lblEnergyUIDiamondNum = m_myTransform.Find(m_widgetToFullName["EnergyUIDiamondNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_tranEnergyUIBtnCharge = FindTransform("EnergyUIBtnCharge");

        IsShowBuyEnergyTipDialog = SystemConfig.Instance.IsShowBuyEnergyTipDialog;

        Initialize();
    }

    #region 事件

    public Action ENERGYUIBUYALLUP;
    public Action ENERGYUIBUYONEUP;
    public Action ENERGYUICHARGEUP;
    public Action ENERGYUICLOSEUP;
    public Action ENERGYUIUSEOK;
    public Action ENERGYUIUSECANCEL;
    public Action ENERGYUIUSETIPENABLE;

    public void Initialize()
    {
        EnergyUIDict.ButtonTypeToEventUp.Add("EnergyUIBtnBuyAll", OnEnergyBuyAllUp);
        EnergyUIDict.ButtonTypeToEventUp.Add("EnergyUIBtnBuyOne", OnEnergyBuyOneUp);
        EnergyUIDict.ButtonTypeToEventUp.Add("EnergyUIBtnCharge", OnEnergyChargeUp);
        EnergyUIDict.ButtonTypeToEventUp.Add("EnergyUIBtnClose", OnEnergyCloseUp);

        EnergyUIDict.ButtonTypeToEventUp.Add("EnergyUIUseOKBtn", OnEnergyUseOK);
        EnergyUIDict.ButtonTypeToEventUp.Add("EnergyUIUseCancelBtn", OnEnergyUseCancel);
        EnergyUIDict.ButtonTypeToEventUp.Add("EnergyUIUseTipEnable", OnEnergyUseTipEnable);   

        EnergyUILogicManager.Instance.Initialize();
		m_uiLoginManager = EnergyUILogicManager.Instance;
    }

    public void Release()
    {
        EnergyUILogicManager.Instance.Release();
        MogoUIManager.Instance.IsEnergyUILoaded = false;
        EnergyUIDict.ButtonTypeToEventUp.Clear();
    }

    void OnEnergyBuyAllUp(int i)
    {
        if (ENERGYUIBUYALLUP != null)
            ENERGYUIBUYALLUP();
    }

    void OnEnergyBuyOneUp(int i)
    {
        if (ENERGYUIBUYONEUP != null)
            ENERGYUIBUYONEUP();
    }

    void OnEnergyChargeUp(int i)
    {
        if (ENERGYUICHARGEUP != null)
            ENERGYUICHARGEUP();
    }

    void OnEnergyCloseUp(int i)
    {
        if (ENERGYUICLOSEUP != null)
            ENERGYUICLOSEUP();
    }

    void OnEnergyUseOK(int i)
    {
        if (ENERGYUIUSEOK != null)
            ENERGYUIUSEOK();
    }

    void OnEnergyUseCancel(int i)
    {
        if (ENERGYUIUSECANCEL != null)
            ENERGYUIUSECANCEL();
    }

    void OnEnergyUseTipEnable(int i)
    {
        if (ENERGYUIUSETIPENABLE != null)
            ENERGYUIUSETIPENABLE();
    }

    #endregion

    #region 确认框

    /// <summary>
    /// 购买体力时是否弹确认框
    /// </summary>
    bool m_IsShowBuyEnergyTipDialog;
    public bool IsShowBuyEnergyTipDialog
    {
        get
        {
            return m_IsShowBuyEnergyTipDialog;
        }
        set
        {
            m_IsShowBuyEnergyTipDialog = value;
            if (m_IsShowBuyEnergyTipDialog == false)
            {
                // 判断是否需要重置(每天第一次登陆重置为true)
                if (MogoTime.Instance.GetCurrentDateTime().Day != Mogo.Util.SystemConfig.Instance.BuyEnergyTipDialogDisableDay)
                {
                    m_IsShowBuyEnergyTipDialog = true;
                    Mogo.Util.SystemConfig.Instance.IsShowBuyEnergyTipDialog = true;
                    Mogo.Util.SystemConfig.Instance.BuyEnergyTipDialogDisableDay = MogoTime.Instance.GetCurrentDateTime().Day;
                    Mogo.Util.SystemConfig.SaveConfig();
                }
            }
            OnUseTipClick();
        }
    }

    /// <summary>
    /// 设置是否以后不提示
    /// </summary>
    private void OnUseTipClick()
    {
        if (IsShowBuyEnergyTipDialog)
            m_goEnergyUIUseTipEnableBGDown.SetActive(false);
        else
            m_goEnergyUIUseTipEnableBGDown.SetActive(true);
    }

    /// <summary>
    /// 设置是否显示使用确定框
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowUseOKCancel(bool isShow)
    {
        m_goGOEnergyUIUseOKCancel.SetActive(isShow);        
    }

    #endregion

    #region 界面信息

    /// <summary>
    /// 设置当前VIP等级
    /// </summary>
    /// <param name="level"></param>
    public void SetCurrentVipLevel(byte level)
    {
        if (level < 10)
        {
            m_spEnergyUIVIPLevelImgVIPLevel.spriteName = level.ToString();
            m_spEnergyUIVIPLevelImgVIPLevel2.gameObject.SetActive(false);
        }
        else if (level >= 10)
        {
            m_spEnergyUIVIPLevelImgVIPLevel.spriteName ="1";
            m_spEnergyUIVIPLevelImgVIPLevel2.spriteName = (level - 10).ToString();
            m_spEnergyUIVIPLevelImgVIPLevel2.gameObject.SetActive(true);
        }        
    }

    /// <summary>
    /// 设置剩余购买次数
    /// </summary>
    /// <param name="times"></param>
    public void SetLastTimes(int times)
    {
        m_lblEnergyUITipLastTimesText.text = string.Format(LanguageData.GetContent(47504), times);
    }

    /// <summary>
    /// 设置体力
    /// </summary>
    /// <param name="energy"></param>
    public void SetEnergy(int energy)
    {
        m_lblEnergyUIProgressBarText.text = string.Format(LanguageData.GetContent(47503), energy);
    }

    /// <summary>
    /// 设置体力
    /// </summary>
    /// <param name="energy"></param>
	public void SetEnergy(string energy)
    {
        m_lblEnergyUIProgressBarText.text = string.Format(LanguageData.GetContent(47503), energy);
    }
	

    /// <summary>
    /// 设置全部购买体力说明
    /// </summary>
    /// <param name="times"></param>
    /// <param name="oneEnergy"></param>
    public void SetBuyAllText2(int times)
    {
        if (times > 0)
        {
            EnergyData energyData = EnergyData.dataMap[1];
            int oneEnergy = energyData.fixedPoints;
            m_lblEnergyUIBtnBuyAllText2.text = string.Format(LanguageData.GetContent(47500), times, oneEnergy);

            m_goEnergyUIBtnBuyAll.GetComponentsInChildren<BoxCollider>(true)[0].enabled = true;
            m_spEnergyUIBtnBuyAllBGUp.spriteName = "tongyong_butten_green_up";
        }
        else
        {
            m_lblEnergyUIBtnBuyAllText2.text = string.Format(LanguageData.GetContent(47501), 0);

            m_goEnergyUIBtnBuyAll.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;
            m_spEnergyUIBtnBuyAllBGUp.spriteName = "btn_03grey";
        }
    }

    /// <summary>
    /// 设置一次购买体力说明
    /// </summary>
    /// <param name="oneEnergy"></param>
    public void SetBuyOneText2(int times)
    {
        if (times > 0)
        {
            EnergyData energyData = EnergyData.dataMap[1];
            int oneEnergy = energyData.fixedPoints;
            m_lblEnergyUIBtnBuyOneText2.text = string.Format(LanguageData.GetContent(47501), oneEnergy);

            m_goEnergyUIBtnBuyOne.GetComponentsInChildren<BoxCollider>(true)[0].enabled = true;
            m_spEnergyUIBtnBuyOneBGUp.spriteName = "tongyong_butten_green_up";
        }
        else
        {
            m_lblEnergyUIBtnBuyOneText2.text = string.Format(LanguageData.GetContent(47501), 0);

            m_goEnergyUIBtnBuyOne.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;
            m_spEnergyUIBtnBuyOneBGUp.spriteName = "btn_03grey";
        }
    }

    /// <summary>
    /// 设置体力购买次数刷新时间
    /// </summary>
    /// <param name="time"></param>
    public void SetTipRefresh(int time)
    {
        m_lblEnergyUITipRefreshText.text = string.Format(LanguageData.GetContent(47502), time);
    }

    /// <summary>
    /// 设置进度条
    /// </summary>
    /// <param name="value"></param>
    public void SetProgressBar(float value)
    {
        m_goEnergyUIProgressBar.GetComponent<UISlider>().sliderValue = value;
    }

    /// <summary>
    /// 设置VIP相关
    /// </summary>
    public void SetVipRealState()
    {
        int lastTimes = MogoWorld.thePlayer.CalBuyEnergyLastTimes();
        SetLastTimes(lastTimes);
        SetBuyAllText2(lastTimes);
        SetBuyOneText2(lastTimes);
    }

    /// <summary>
    /// 设置花费钻石
    /// </summary>
    /// <param name="cost"></param>
    public void SetDiamondCost(int cost)
    {
        m_lblEnergyUIUseInfoDiamondNum.text = "x" + cost;
    }

    /// <summary>
    /// 设置获得体力
    /// </summary>
    /// <param name="energy"></param>
    public void SetRewardEnergy(int times)
    {
        if (EnergyData.dataMap.ContainsKey(1))
        {
            EnergyData energyData = EnergyData.dataMap[1];
            int oneEnergy = energyData.fixedPoints;
            int energy = times * oneEnergy;
            m_lblEnergyUIUseInfoEnergyNum.text = energy.ToString();
        }      
    }

    /// <summary>
    /// 设置玩家钻石数量
    /// </summary>
    /// <param name="diamond"></param> 
    public void SetDiamondNum(uint diamond)
    {
        m_lblEnergyUIDiamondNum.text = diamond.ToString();
    }

    #endregion

    #region 充值按钮特效

    private string m_fx1ChargeButton = "ChargeButtonFX1";

    /// <summary>
    /// 在充值按钮上附加特效
    /// </summary>
    private void AttachChargeButtonAnimation()
    {
        if (m_tranEnergyUIBtnCharge == null)
        {
            LoggerHelper.Error("m_tranEnergyUIBtnCharge is null");
            return;
        }

        MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        MogoFXManager.Instance.AttachParticleAnim("fx_ui_baoxiangxingxing.prefab", m_fx1ChargeButton, m_tranEnergyUIBtnCharge.position,
            MogoUIManager.Instance.GetMainUICamera().GetComponent<Camera>(), 0, 0, 0, () =>
            {
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
    }

    /// <summary>
    /// 释放充值按钮特效
    /// </summary>
    private void ReleaseChargeButtonAnimation()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fx1ChargeButton);
    }

    #endregion

    #region 界面打开和关闭

    public void ReleaseUIAndResources()
    {
        ReleasePreLoadResources();
        MogoUIManager.Instance.DestroyEnergyUI();
    }

    void ReleasePreLoadResources()
    {
 
    }

    /// <summary>
    /// 展示界面信息
    /// </summary>
    /// <param name="IsShow"></param>
    protected override void OnEnable()
    {
 	    base.OnEnable();
        AttachChargeButtonAnimation();
        int refreshTime = 0;    // 体力购买刷新时间,可改为读表
        SetVipRealState();
        SetTipRefresh(refreshTime);
    }   

    void OnDisable()
    {
        ReleaseChargeButtonAnimation(); // 释放充值按钮特效

        ReleaseUIAndResources();
    }

    #endregion
}
