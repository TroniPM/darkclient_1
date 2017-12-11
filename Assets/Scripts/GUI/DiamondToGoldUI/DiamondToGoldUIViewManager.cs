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

// UI逻辑绑定步骤
// 1.View继承MogoUIBehaviour;
// 2.View的初始化时设置m_uiLoginManager = xxxxxLogic.Instance;
// 3.Logic继承UILogicManager;
// 4.Logic的Release使用public override void Release()
// 5.Logic的Release调用base.Release();
// 6.在EntityMyself()构造函数中设置SourceItem

public enum DiamondToGoldUseType
{
    UseOne = 1,// 使用一次
    UseAll = 2,// 全部使用
}

public class DiamondToGoldUIViewManager : MogoUIBehaviour 
{
    private static DiamondToGoldUIViewManager m_instance;
    public static DiamondToGoldUIViewManager Instance
    {
        get { return DiamondToGoldUIViewManager.m_instance;}
    }  

    private UILabel m_lblDiamondToGoldUIFromDiamondText2;
    private UILabel m_lblDiamondToGoldUIToGoldText2;
    private UILabel m_lblDiamondToGoldUILastTimesText2;
    private UILabel m_lblDiamondToGoldUIUseAllText1;
    private Transform m_tranGODiamondToGoldChoose;
    private UITexture m_texDiamondToGoldUIBtnTurnBG;

    private GameObject m_goDiamondToGoldUIUseOKCancel;
    private UILabel m_lblDiamondToGoldUIUseInfoDiamondText2;
    private UILabel m_lblDiamondToGoldUIUseInfoGoldText2;
    private GameObject m_goDiamondToGoldUIUseTipEnable;
    private GameObject m_goGODiamondToGoldUIUseTipEnable;
    private GameObject m_goDiamondToGoldUIUseTipEnableBGDown;
    private GameObject m_goDiamondToGoldUIUseAllCheckDown;
    private GameObject m_goDiamondToGoldUIUseOneCheckDown;

    private UILabel m_lblDiamondToGoldUIDiamondNum;
    private UILabel m_lblDiamondToGoldUIGoldNum;

    void Awake()
    {
        m_instance = gameObject.GetComponent<DiamondToGoldUIViewManager>();
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblDiamondToGoldUIFromDiamondText2 = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIFromDiamondText2"]).GetComponent<UILabel>();
        m_lblDiamondToGoldUIToGoldText2 = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIToGoldText2"]).GetComponent<UILabel>();
        m_lblDiamondToGoldUILastTimesText2 = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUILastTimesText2"]).GetComponent<UILabel>();
        m_tranGODiamondToGoldChoose = m_myTransform.Find(m_widgetToFullName["GODiamondToGoldChoose"]);
        m_lblDiamondToGoldUIUseAllText1 = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIUseAllText1"]).GetComponent<UILabel>();
        m_texDiamondToGoldUIBtnTurnBG = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIBtnTurnBG"]).GetComponent<UITexture>();

        m_goDiamondToGoldUIUseOKCancel = m_myTransform.Find(m_widgetToFullName["GODiamondToGoldUIUseOKCancel"]).gameObject;
        m_lblDiamondToGoldUIUseInfoDiamondText2 = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIUseInfoDiamondText2"]).GetComponent<UILabel>();
        m_lblDiamondToGoldUIUseInfoGoldText2 = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIUseInfoGoldText2"]).GetComponent<UILabel>();
        m_goDiamondToGoldUIUseTipEnable = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIUseTipEnable"]).gameObject;
        m_goGODiamondToGoldUIUseTipEnable = m_myTransform.Find(m_widgetToFullName["GODiamondToGoldUIUseTipEnable"]).gameObject;
        m_goDiamondToGoldUIUseTipEnableBGDown = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIUseTipEnableBGDown"]).gameObject;
               
        m_goDiamondToGoldUIUseAllCheckDown = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIUseAllCheckDown"]).gameObject;
        m_goDiamondToGoldUIUseOneCheckDown = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIUseOneCheckDown"]).gameObject;

        m_lblDiamondToGoldUIDiamondNum = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIDiamondNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblDiamondToGoldUIGoldNum = m_myTransform.Find(m_widgetToFullName["DiamondToGoldUIGoldNum"]).GetComponentsInChildren<UILabel>(true)[0];

        IsShowGoldMetallurgyTipDialog = SystemConfig.Instance.IsShowGoldMetallurgyTipDialog;
        Initialize();
    }

    #region 事件
    public Action DIAMONDTOGOLDUIUSEALLUP;
    public Action DIAMONDTOGOLDUIUSEONEUP;
    public Action DIAMONDTOGOLDUICLOSEUP;
    public Action DIAMONDTOGOLDUITRUNUP;

    public Action DIAMONDTOGOLDUIUSEOK;
    public Action DIAMONDTOGOLDUIUSECANCEL;
    public Action DIAMONDTOGOLDUIUSETIPENABLE;

    public void Initialize()
    {
        DiamondToGoldUIDict.ButtonTypeToEventUp.Add("DiamondToGoldUIUseAll", OnDiamondToGoldUseAllUp);
        DiamondToGoldUIDict.ButtonTypeToEventUp.Add("DiamondToGoldUIUseOne", OnDiamondToGoldUseOneUp);
        DiamondToGoldUIDict.ButtonTypeToEventUp.Add("DiamondToGoldUIBtnClose", OnDiamondToGoldCloseUp);
        DiamondToGoldUIDict.ButtonTypeToEventUp.Add("DiamondToGoldUIBtnTurn", OnDiamondToGoldTurnUp);
        DiamondToGoldUIDict.ButtonTypeToEventUp.Add("DiamondToGoldUIBtnTurn2", OnDiamondToGoldTurnUp);        

        DiamondToGoldUIDict.ButtonTypeToEventUp.Add("DiamondToGoldUIUseOKBtn", OnDiamondToGoldUseOK);
        DiamondToGoldUIDict.ButtonTypeToEventUp.Add("DiamondToGoldUIUseCancelBtn", OnDiamondToGoldUseCancel);
        DiamondToGoldUIDict.ButtonTypeToEventUp.Add("DiamondToGoldUIUseTipEnable", OnDiamondToGoldUseTipEnable);         

        DiamondToGoldUILogicManager.Instance.Initialize();
        m_uiLoginManager = DiamondToGoldUILogicManager.Instance;
    }

    public void Release()
    {
        DiamondToGoldUILogicManager.Instance.Release();
        MogoUIManager.Instance.IsDiamondToGoldUILoaded = false;
        DiamondToGoldUIDict.ButtonTypeToEventUp.Clear();
    }

    void OnDiamondToGoldUseAllUp(int i)
    {
        if (DIAMONDTOGOLDUIUSEALLUP != null)
            DIAMONDTOGOLDUIUSEALLUP();
    }

    void OnDiamondToGoldUseOneUp(int i)
    {
        if (DIAMONDTOGOLDUIUSEONEUP != null)
            DIAMONDTOGOLDUIUSEONEUP();
    }

    void OnDiamondToGoldCloseUp(int i)
    {
        if (DIAMONDTOGOLDUICLOSEUP != null)
            DIAMONDTOGOLDUICLOSEUP();
    }

    void OnDiamondToGoldTurnUp(int i)
    {
        if (MogoWorld.thePlayer.VipLevel < NeedVipLevel)
        {
            return;
        }

        if (DIAMONDTOGOLDUITRUNUP != null)
            DIAMONDTOGOLDUITRUNUP();
    }

    void OnDiamondToGoldUseOK(int i)
    {
        if (DIAMONDTOGOLDUIUSEOK != null)
            DIAMONDTOGOLDUIUSEOK();
    }

    void OnDiamondToGoldUseCancel(int i)
    {
        if (DIAMONDTOGOLDUIUSECANCEL != null)
            DIAMONDTOGOLDUIUSECANCEL();
    }

    void OnDiamondToGoldUseTipEnable(int i)
    {
        if (DIAMONDTOGOLDUIUSETIPENABLE != null)
            DIAMONDTOGOLDUIUSETIPENABLE();
    }

    #endregion    

    private DiamondToGoldUseType m_diamondToGoldUseType = DiamondToGoldUseType.UseOne;
    public DiamondToGoldUseType UseType
    {
        get
        {
            return m_diamondToGoldUseType;
        }
        set
        {
            m_diamondToGoldUseType = value;
            SetUseTypeChoose();
        }
    }

    private bool m_IsShowGoldMetallurgyTipDialog;
    public bool IsShowGoldMetallurgyTipDialog
    {
        get
        {
            return m_IsShowGoldMetallurgyTipDialog;
        }
        set
        {
            m_IsShowGoldMetallurgyTipDialog = value;
            if (m_IsShowGoldMetallurgyTipDialog == false)
            {			 
                // 判断是否需要重置(每天第一次登陆重置为true)
                if (MogoTime.Instance.GetCurrentDateTime().Day != Mogo.Util.SystemConfig.Instance.GoldMetallurgyTipDialogDisableDay)
                {
                    m_IsShowGoldMetallurgyTipDialog = true;
                    Mogo.Util.SystemConfig.Instance.IsShowGoldMetallurgyTipDialog = true;
					Mogo.Util.SystemConfig.Instance.GoldMetallurgyTipDialogDisableDay = MogoTime.Instance.GetCurrentDateTime().Day;
                    Mogo.Util.SystemConfig.SaveConfig();
                }
            }
            OnUseTipClick();
        }
    }

    /// <summary>
    /// 展示界面信息
    /// </summary>
    /// <param name="IsShow"></param>
    public void Show(bool IsShow)
    {
        if (IsShow)
        {            
            if (MogoWorld.thePlayer.m_VipRealStateMap != null)
            {
                SetVipRealState();
                SetUseTypeChoose();
                ShowChooseTypeBtnWhenVipLevelReach();
                SetDiamondNum(MogoWorld.thePlayer.diamond);
                SetGoldNum(MogoWorld.thePlayer.gold);
            }
            else
            {
                LoggerHelper.Debug("VipRealStateMap is null");
            }
        }
        else
        {

        }
    }

    /// <summary>
    /// 设置VIP相关
    /// </summary>
    public void SetVipRealState()
    {     
        SetLastUseTimes(CalLastUseTimes()); 
		SetDiamondToGoldCostAndReward();
    }

    /// <summary>
    /// 是否显示炼金按钮，VIP等级 >= 0显示
    /// </summary>
    readonly int NeedVipLevel = 0;
    public void ShowChooseTypeBtnWhenVipLevelReach()
    {
        if (MogoWorld.thePlayer.VipLevel >= NeedVipLevel)
        {
            if (m_tranGODiamondToGoldChoose != null)
                m_tranGODiamondToGoldChoose.gameObject.SetActive(true);
        }
        else
        {
            if (m_tranGODiamondToGoldChoose != null)
                m_tranGODiamondToGoldChoose.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置剩余可炼金次数
    /// </summary>
    /// <param name="times"></param>
    private void SetLastUseTimes(int times)
    {
        if (m_lblDiamondToGoldUILastTimesText2 != null)
            m_lblDiamondToGoldUILastTimesText2.text = times.ToString();
        //m_lblDiamondToGoldUIUseAllText1.text = string.Format("炼金{0:0}次", times);
    }

    /// <summary>
    /// 设置消耗钻石，活动金币
    /// </summary>
    /// <param name="diamondToGoldUseType"></param>
    public void SetDiamondToGoldCostAndReward()
    {
        if (UseType == DiamondToGoldUseType.UseOne)
        {
            SetDiamondCost(1);
            SetGoldReward(1);
        }
        else if (UseType == DiamondToGoldUseType.UseAll)
        {
            int lastUseTimes = 10;
            SetDiamondCost(lastUseTimes);
            SetGoldReward(lastUseTimes);
        }
    }

    /// <summary>
    /// 设置消耗钻石
    /// </summary>
    /// <param name="times"></param>
    private void SetDiamondCost(int times)
    {
        int diamondCost = CalDiamondCost(times);

        if (m_lblDiamondToGoldUIFromDiamondText2 != null)
            m_lblDiamondToGoldUIFromDiamondText2.text = "x" + diamondCost;
        if (m_lblDiamondToGoldUIUseInfoDiamondText2 != null)
            m_lblDiamondToGoldUIUseInfoDiamondText2.text = "x" + diamondCost;
    }

    /// <summary>
    /// 设置获得金币
    /// </summary>
    /// <param name="times"></param>
    private void SetGoldReward(int times)
    {
        int GOLD_METALLURGY_REWARD = 12;// 炼金产出次数定价表ID
        int One_GOLD_METALLURGY_REWARD = PriceListData.dataMap[GOLD_METALLURGY_REWARD].priceList[1];
        int goldReward = times * One_GOLD_METALLURGY_REWARD;

        if (m_lblDiamondToGoldUIToGoldText2 != null)
            m_lblDiamondToGoldUIToGoldText2.text = "x" + goldReward;

        if (m_lblDiamondToGoldUIUseInfoGoldText2 != null)
            m_lblDiamondToGoldUIUseInfoGoldText2.text = "x" + goldReward;
    }

    /// <summary>
    /// 设置使用次数类型
    /// </summary>
    private void SetUseTypeChoose()
    {
        if (UseType == DiamondToGoldUseType.UseOne)
        {
            if (m_tranGODiamondToGoldChoose != null)
                m_tranGODiamondToGoldChoose.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(0);

            if (m_goDiamondToGoldUIUseAllCheckDown != null)
                m_goDiamondToGoldUIUseAllCheckDown.gameObject.SetActive(false);

            if (m_goDiamondToGoldUIUseOneCheckDown != null)
                m_goDiamondToGoldUIUseOneCheckDown.gameObject.SetActive(true);
        }
        else if (UseType == DiamondToGoldUseType.UseAll)
        {
            if (m_tranGODiamondToGoldChoose != null)
                m_tranGODiamondToGoldChoose.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(1);

            if (m_goDiamondToGoldUIUseAllCheckDown != null)
                m_goDiamondToGoldUIUseAllCheckDown.gameObject.SetActive(true);

            if (m_goDiamondToGoldUIUseOneCheckDown != null)
                m_goDiamondToGoldUIUseOneCheckDown.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 计算剩余使用次数
    /// </summary>
    /// <returns></returns>
    public int CalLastUseTimes()
    {
        if (MogoWorld.thePlayer != null)
            return MogoWorld.thePlayer.CalGoldMetallurgyLastUseTimes();

        return -1;
    }

    /// <summary>
    /// 计算需要的钻石
    /// </summary>
    /// <param name="times"></param>
    /// <returns></returns>
    public int CalDiamondCost(int times)
    {
        if (MogoWorld.thePlayer != null)
            return MogoWorld.thePlayer.CaGoldMetallurgylDiamondCost(times);

        return -1;
    }

    /// <summary>
    /// 设置玩家钻石数量
    /// </summary>
    /// <param name="diamond"></param> 
    public void SetDiamondNum(uint diamond)
    {
        m_lblDiamondToGoldUIDiamondNum.text = diamond.ToString();
    }

    /// <summary>
    /// 设置玩家金币数量
    /// </summary>
    /// <param name="gold"></param>
    public void SetGoldNum(uint gold)
    {
        m_lblDiamondToGoldUIGoldNum.text = gold.ToString();
    }

    #region 是否使用确定框

    /// <summary>
    /// 设置是否显示使用确定框
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowUseOKCancel(bool isShow)
    {
        if (m_goDiamondToGoldUIUseOKCancel != null)
            m_goDiamondToGoldUIUseOKCancel.SetActive(isShow);        
    }

    /// <summary>
    /// 设置是否以后不提示
    /// </summary>
    void OnUseTipClick()
    {
        if (IsShowGoldMetallurgyTipDialog)
            m_goDiamondToGoldUIUseTipEnableBGDown.SetActive(false); 
        else
            m_goDiamondToGoldUIUseTipEnableBGDown.SetActive(true); 
    }
  
    #endregion

    private void ShowDiamondToGoldUIBtnTurnBG(bool isShow)
    {        
        if(isShow)
        {
            if (m_texDiamondToGoldUIBtnTurnBG.mainTexture == null)
            {
                AssetCacheMgr.GetResourceAutoRelease("lianjin.png", (obj) =>
                {
                    m_texDiamondToGoldUIBtnTurnBG.mainTexture = (Texture)obj;
                });
            }
        }
        else
        {
            if (m_texDiamondToGoldUIBtnTurnBG != null)
            {
                if (!SystemSwitch.DestroyResource)
                {
                    return;
                }

                m_texDiamondToGoldUIBtnTurnBG.mainTexture = null;
                AssetCacheMgr.ReleaseResourceImmediate("lianjin.png");
            }
        }
    }    

    public void ReleaseUIAndResources()
    {
        RelasePreLoadResources();
        MogoUIManager.Instance.DestroyDiamondToGoldUI();
    }

    void RelasePreLoadResources()
    {
 
    }

    #region OnEnable and OnDisable

    protected override void OnEnable()
    {
        base.OnEnable();
        //ShowDiamondToGoldUIBtnTurnBG(true);
    }

    void OnDisable()
    {
        ReleaseUIAndResources();
        //ShowDiamondToGoldUIBtnTurnBG(false);        
    }

    #endregion
}
