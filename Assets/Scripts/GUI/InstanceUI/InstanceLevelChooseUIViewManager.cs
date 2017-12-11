#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：副本难度选择界面
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using System;
using System.Text;
using Mogo.Game;
using Mogo.Mission;
using Mogo.GameData;

/// <summary>
/// 副本难度
/// </summary>
public enum LevelType
{
	Simple = 0, // 普通
	Difficult = 1, // 地狱
}

public enum FoggyAbyssLevelType
{
    Abyss = 1,      // 深渊
    Hell = 2,       // 地狱
    Purgatory = 3   // 炼狱
}

/// <summary>
/// 副本评价
/// </summary>
public enum InstanceStar
{
	InstanceStarS = 4,
	InstanceStarA = 3,
	InstanceStarB = 2,
	InstanceStarC = 1,
}

 /// <summary>
 /// 副本类型
 /// </summary>
public enum MissionType
{
    Normal = 0, // [普通副本]
    FoggyAbyss = 1, // [特殊副本-迷雾深渊]
}

public class InstanceLevelChooseUIViewManager : MogoUIBehaviour 
{
    private static InstanceLevelChooseUIViewManager m_instance;
	public static InstanceLevelChooseUIViewManager Instance { get { return InstanceLevelChooseUIViewManager.m_instance;	}}

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量

    private UILabel m_lblMissionFoggyAbyssTip;
    private UILabel m_lblInstanceLevelChooseUICostVP;
    private UILabel m_lblInstanceLevelChooseUIReqLevel;
    private GameObject m_goGOInstanceLevelChooseUIDrop;
    private UILabel m_lblInstanceLevelChooseUIDrop;
    private UILabel m_lblInstanceLevelChooseUIBtnClean;
    private UILabel m_lblInstanceLevelChooseUIBtnEnter;
    private GameObject m_goInstanceLevelChooseUIBtnBack;

    private GameObject m_goInstanceLevelChooseUIBtnClean;
    private UISprite m_spInstanceLevelChooseUIBtnCleanBGDown;
    private UISprite m_spInstanceLevelChooseUIBtnCleanBGUp;

    private GameObject m_goInstanceLevelChooseUIBtnEnter;
    private UISprite m_spInstanceLevelChooseUIBtnEnterBGDown;
    private UISprite m_spInstanceLevelChooseUIBtnEnterBGUp;

    private GameObject m_goInstanceLevelChooseUIBtnClose;
    private GameObject m_goInstanceLevelChooseUIBtnHelp;
    private GameObject m_goInstanceLevelChooseUIBtnMissionFlag;
	private GameObject[] m_arrInstanceLevel = new GameObject[2];
    private UITexture m_texInstanceLevelChooseUIStarType;
    private UITexture m_texInstanceLevelChooseUIStarTypeBG;   

    private GameObject m_goInstanceLevelChooseUIBtnEnterPosCenter;
    private GameObject m_goInstanceLevelChooseUIBtnEnterPosRight;

    // 重置窗口
    private GameObject m_goInstanceLevelChooseResetMissionWindow;
    private GameObject m_goInstanceLevelChooseCanNotResetMissionWindow;
    private GameObject m_goInstanceLevelChooseCanResetMissionWindow;
    private UILabel m_lblNInstanceLevelChooseCanNotResetMissionWindowText;

    private GameObject m_goGONInstanceLevelChooseCanResetMissionWindowInfo1;
    private UILabel m_lblNInstanceLevelChooseCanResetMissionWindowInfo1Text;

    private GameObject m_goGONInstanceLevelChooseCanResetMissionWindowInfo2;
    private UILabel m_lblNInstanceLevelChooseCanResetMissionWindowTextNum;
    private UILabel m_lblNInstanceLevelChooseCanResetMissionWindowTipNum;

    private GameObject m_goInstanceLevelChooseLevel;
    private GameObject m_goInstanceLevelChooseLevel0Tip;
    private GameObject m_goInstanceLevelChooseLevel1Tip;
    private UILabel m_lblInstanceLevelChooseLevel0TipText;
    private UILabel m_lblInstanceLevelChooseLevel1TipText;

    private UISprite m_spInstanceLevelChooseUIBGFG1;

    private GameObject m_goInstanceLevelChooseUIDropLevel0BG;
    private GameObject m_goInstanceLevelChooseUIDropLevel1BG;

    #region 物品奖励UI

    private GameObject m_goGOInstanceLevelChooseUIItemRewardUI;
    private UILabel m_lblInstanceLevelChooseUIItemRewardUITitle;

    // 物品奖励可拖动显示
    private Transform m_tranInstanceLevelChooseUIDragRewardList;
    private Transform m_tranGOInstanceLevelChooseUIDragRewardListSet;
    private Camera m_dragRewardListCamera;
    private MyDragableCamera m_dragRewardListMyDragableCamera;
    private Vector3 m_dragRewardListCameraInitPos;

    private GameObject m_goInstanceLevelPosItem1;
    private GameObject m_goInstanceLevelPosItem2;
    private GameObject m_goInstanceLevelPosItem3;
    private GameObject m_goInstanceLevelPosItem4;

    // 玩家NO.1
    private GameObject m_goGOInstanceLevelChooseUIPlayerNO1;
    private UILabel m_lblInstanceLevelChooseUIPlayerNO1Title;
    private UILabel m_lblInstanceLevelChooseUIPlayerNO1Name;
    private UILabel m_lblInstanceLevelChooseUIPlayerNO1Score;
    private UILabel m_lblInstanceLevelChooseUIPlayerNO1Vocation;

    #endregion

    #region 翻牌奖励UI
    
    private GameObject m_goGOInstanceLevelChooseUICardRewardUI;
    private UILabel m_lblInstanceLevelChooseUICardRewardUITitle;

    // 翻牌奖励可拖动显示
    private Transform m_tranInstanceLevelChooseUICardRewardList;
    private Camera m_dragCardRewardListCamera;
    private MyDragableCamera m_dragCardRewardListDragableCamera;
    private Vector3 m_dragCardRewardListCameraInitPos;    

    #endregion

    void Awake()
	{
        m_instance = this;
		m_myTransform = transform;
		FillFullNameData(m_myTransform);

        m_lblInstanceLevelChooseUIItemRewardUITitle = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIItemRewardUITitle"]).GetComponent<UILabel>();
        m_lblInstanceLevelChooseUICardRewardUITitle = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUICardRewardUITitle"]).GetComponent<UILabel>();

        m_lblMissionFoggyAbyssTip = FindTransform("InstanceLevelChooseUIMissionFoggyAbyssTip").GetComponentsInChildren<UILabel>(true)[0];
		m_lblInstanceLevelChooseUICostVP = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUICostVP"]).GetComponent<UILabel>();        
        m_lblInstanceLevelChooseUIReqLevel = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIReqLevel"]).GetComponent<UILabel>();
        m_goGOInstanceLevelChooseUIDrop = m_myTransform.Find(m_widgetToFullName["GOInstanceLevelChooseUIDrop"]).gameObject;
		m_lblInstanceLevelChooseUIDrop = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIDrop"]).GetComponent<UILabel>();		

		m_goInstanceLevelChooseUIBtnBack = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIBtnBack"]).gameObject;		
		m_goInstanceLevelChooseUIBtnClose = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIBtnClose"]).gameObject;		
		m_goInstanceLevelChooseUIBtnHelp = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIBtnHelp"]).gameObject;
        m_goInstanceLevelChooseUIBtnMissionFlag = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIBtnRW"]).gameObject;        

        // 评价星级
		m_texInstanceLevelChooseUIStarType = FindTransform("InstanceLevelChooseUIStarType").GetComponent<UITexture>();
        m_texInstanceLevelChooseUIStarTypeBG = FindTransform("InstanceLevelChooseUIStarTypeBG").GetComponent<UITexture>();

        // 扫荡按钮
        m_goInstanceLevelChooseUIBtnClean = FindTransform("InstanceLevelChooseUIBtnClean").gameObject;
        m_lblInstanceLevelChooseUIBtnClean = FindTransform("InstanceLevelChooseUIBtnCleanText").GetComponent<UILabel>();
		m_spInstanceLevelChooseUIBtnCleanBGDown = FindTransform("InstanceLevelChooseUIBtnCleanBGDown").GetComponent<UISprite>();
		m_spInstanceLevelChooseUIBtnCleanBGUp = FindTransform("InstanceLevelChooseUIBtnCleanBGUp").GetComponent<UISprite>();

        // 进入按钮
        m_goInstanceLevelChooseUIBtnEnter = FindTransform("InstanceLevelChooseUIBtnEnter").gameObject;
        m_lblInstanceLevelChooseUIBtnEnter = FindTransform("InstanceLevelChooseUIBtnEnterText").GetComponent<UILabel>();
		m_spInstanceLevelChooseUIBtnEnterBGDown = FindTransform("InstanceLevelChooseUIBtnEnterBGDown").GetComponent<UISprite>();
		m_spInstanceLevelChooseUIBtnEnterBGUp = FindTransform("InstanceLevelChooseUIBtnEnterBGUp").GetComponent<UISprite>();
        m_goInstanceLevelChooseUIBtnEnterPosCenter = FindTransform("InstanceLevelChooseUIBtnEnterPosCenter").gameObject;
        m_goInstanceLevelChooseUIBtnEnterPosRight = FindTransform("InstanceLevelChooseUIBtnEnterPosRight").gameObject;

        //////////////////////////////////////////////////////////////////////////公共UI

        // 难度选择
        m_goInstanceLevelChooseLevel = FindTransform("InstanceLevelChooseLevel").gameObject;
		for (int i = 0; i < 2; ++i)
		{
			m_arrInstanceLevel[i] = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseLevel" + i]).gameObject;
			NewInstanceLevelGrid grid = m_arrInstanceLevel[i].AddComponent<NewInstanceLevelGrid>();
			grid.id = i;
			grid.parentNameFlag = "InstanceLevelChooseLevel";
			grid.MyStart();
		}

        // 当前提示(当前任务，掉落副本)
        m_goInstanceLevelChooseLevel0Tip = FindTransform("InstanceLevelChooseLevel0Tip").gameObject;
        m_goInstanceLevelChooseLevel1Tip = FindTransform("InstanceLevelChooseLevel1Tip").gameObject;
        m_lblInstanceLevelChooseLevel0TipText = FindTransform("InstanceLevelChooseLevel0TipText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblInstanceLevelChooseLevel1TipText = FindTransform("InstanceLevelChooseLevel1TipText").GetComponentsInChildren<UILabel>(true)[0];

        //////////////////////////////////////////////////////////////////////////物品奖励UI
        m_goGOInstanceLevelChooseUIItemRewardUI = FindTransform("GOInstanceLevelChooseUIItemRewardUI").gameObject;

        // 物品奖励        
        m_tranInstanceLevelChooseUIDragRewardList = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIDragRewardList"]);
        m_dragRewardListCamera = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIDragRewardListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_dragRewardListCameraInitPos = m_dragRewardListCamera.transform.localPosition;
        m_dragRewardListMyDragableCamera = m_dragRewardListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragRewardListMyDragableCamera.LeftArrow = FindTransform("InstanceLevelChooseUIArrowL").gameObject;
        m_dragRewardListMyDragableCamera.RightArrow = FindTransform("InstanceLevelChooseUIArrowR").gameObject;
        m_goInstanceLevelPosItem1 = m_myTransform.Find(m_widgetToFullName["InstanceLevelPosItem1"]).gameObject;
        m_goInstanceLevelPosItem2 = m_myTransform.Find(m_widgetToFullName["InstanceLevelPosItem2"]).gameObject;
        m_goInstanceLevelPosItem3 = m_myTransform.Find(m_widgetToFullName["InstanceLevelPosItem3"]).gameObject;
        m_goInstanceLevelPosItem4 = m_myTransform.Find(m_widgetToFullName["InstanceLevelPosItem4"]).gameObject;
        m_tranGOInstanceLevelChooseUIDragRewardListSet = m_myTransform.Find(m_widgetToFullName["GOInstanceLevelChooseUIDragRewardListSet"]);

        // 玩家NO.1
        m_goGOInstanceLevelChooseUIPlayerNO1 = m_myTransform.Find(m_widgetToFullName["GOInstanceLevelChooseUIPlayerNO1"]).gameObject;
        m_lblInstanceLevelChooseUIPlayerNO1Title = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIPlayerNO1Title"]).GetComponent<UILabel>();
        m_lblInstanceLevelChooseUIPlayerNO1Name = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIPlayerNO1Name"]).GetComponent<UILabel>();
        m_lblInstanceLevelChooseUIPlayerNO1Score = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIPlayerNO1Score"]).GetComponent<UILabel>();
        m_lblInstanceLevelChooseUIPlayerNO1Vocation = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIPlayerNO1Vocation"]).GetComponent<UILabel>();


        //////////////////////////////////////////////////////////////////////////翻牌奖励UI
        m_goGOInstanceLevelChooseUICardRewardUI = FindTransform("GOInstanceLevelChooseUICardRewardUI").gameObject;       

         // 翻牌奖励可拖动显示
        m_tranInstanceLevelChooseUICardRewardList = FindTransform("InstanceLevelChooseUICardRewardList");
        m_dragCardRewardListCamera = FindTransform("InstanceLevelChooseUICardRewardListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_dragCardRewardListDragableCamera = m_dragCardRewardListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragCardRewardListDragableCamera.LeftArrow = FindTransform("InstanceLevelChooseUICardRewardArrowL").gameObject;
        m_dragCardRewardListDragableCamera.RightArrow = FindTransform("InstanceLevelChooseUICardRewardArrowR").gameObject;
        m_dragCardRewardListCameraInitPos = m_dragCardRewardListDragableCamera.transform.localPosition;

        m_spInstanceLevelChooseUIBGFG1 = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIBGFG1"]).GetComponentsInChildren<UISprite>(true)[0];
        m_goInstanceLevelChooseUIDropLevel0BG = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIDropLevel0BG"]).gameObject;
        m_goInstanceLevelChooseUIDropLevel1BG = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIDropLevel1BG"]).gameObject;

        //////////////////////////////////////////////////////////////////////////重置窗口
        m_goInstanceLevelChooseResetMissionWindow = FindTransform("NInstanceLevelChooseResetMissionWindow").gameObject;
		m_goInstanceLevelChooseResetMissionWindow.SetActive(false);
        m_goInstanceLevelChooseCanNotResetMissionWindow = FindTransform("NInstanceLevelChooseCanNotResetMissionWindow").gameObject;
        m_goInstanceLevelChooseCanResetMissionWindow = FindTransform("NInstanceLevelChooseCanResetMissionWindow").gameObject;
        m_lblNInstanceLevelChooseCanNotResetMissionWindowText = FindTransform("NInstanceLevelChooseCanNotResetMissionWindowText").GetComponent<UILabel>();
        m_lblNInstanceLevelChooseCanResetMissionWindowInfo1Text = FindTransform("NInstanceLevelChooseCanResetMissionWindowInfo1Text").GetComponent<UILabel>();
        m_lblNInstanceLevelChooseCanResetMissionWindowTextNum = FindTransform("NInstanceLevelChooseCanResetMissionWindowTextNum").GetComponent<UILabel>();
        m_lblNInstanceLevelChooseCanResetMissionWindowTipNum = FindTransform("NInstanceLevelChooseCanResetMissionWindowTipNum").GetComponent<UILabel>();
        m_goGONInstanceLevelChooseCanResetMissionWindowInfo1 = FindTransform("GONInstanceLevelChooseCanResetMissionWindowInfo1").gameObject;
        m_goGONInstanceLevelChooseCanResetMissionWindowInfo2 = FindTransform("GONInstanceLevelChooseCanResetMissionWindowInfo2").gameObject;

        I18N();
		Initialize();
	}     

	#region 事件

    public Action SWITCHTOITEMREWARDUIUP;
    public Action SWITCHTOCARDREWARDUIUP;

	public Action CHOOSELEVELUIBACKUP;
	public Action CHOOSELEVELUICLOSEUP;
	public Action CHOOSELEVELUICLEANUP;
	public Action CHOOSELEVELUIHELPUP;
    public Action CHOOSELEVELUIMISSIONFLAGUP;

	public Action INSTANCECHOOSELEVEL0UP;
	public Action INSTANCECHOOSELEVEL1UP;

    public Action<int> CHOOSELEVELREWARDITEMUP;
    public Action CANRESETMISSIONWINDOWCONFIRMUP;
    public Action RESETMISSIONUP;

    void I18N()
    {
        m_lblMissionFoggyAbyssTip.text = LanguageData.GetContent(48506);
    }

	void Initialize()
	{
		InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUIBtnBack", OnChooseLevelUISwtichToItemRewardUIUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUIBtnCardReward", OnChooseLevelUISwtichToCardRewardUIUp);        
		InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUIBtnClean", OnChooseLevelUICleanUp);
		InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUIBtnClose", OnChooseLevelUICloseUp);
		InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUIBtnEnter", OnChooseLevelUIEnterUp);
		InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUIBtnHelp", OnChooseLevelUIHelpUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseUIBtnRW", OnChooseLevelUIMissionFlagUp);
        
		InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseLevel0", OnInstanceChooseLeve0Up);
		InstanceUIDict.ButtonTypeToEventUp.Add("InstanceLevelChooseLevel1", OnInstanceChooseLeve1Up);

        InstanceUIDict.ButtonTypeToEventUp.Add("NInstanceLevelChooseCanNotResetMissionWindowConfirm", OnCanNotResetMissionWindowConfirmUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("NInstanceLevelChooseCanResetMissionWindowConfirm", OnCanResetMissionWindowConfirmUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("NInstanceLevelChooseCanResetMissionWindowCancel", OnCanResetMissionWindowCancelUp);

		EventDispatcher.TriggerEvent("InstanceUILoadPartEnd");
	}

	public void Release()
	{
	   // InstanceLevelChooseUILogicManager.Instance.Release();
		//InstanceUIDict.ButtonTypeToEventUp.Clear();
	}

    /// <summary>
    /// 回退按钮
    /// 点击回到物品奖励UI
    /// </summary>
    /// <param name="i"></param>
	void OnChooseLevelUISwtichToItemRewardUIUp(int i)
	{
        if (SWITCHTOITEMREWARDUIUP != null)
            SWITCHTOITEMREWARDUIUP();

        SwitchToItemRewardUI();
	}

    /// <summary>
    /// 翻牌奖励按钮
    /// </summary>
    /// <param name="i"></param>
    void OnChooseLevelUISwtichToCardRewardUIUp(int i)
    {
        if (SWITCHTOCARDREWARDUIUP != null)
            SWITCHTOCARDREWARDUIUP();

        SwitchToCardRewardUI();
    }

	void OnChooseLevelUICloseUp(int i)
	{
		if(CHOOSELEVELUICLOSEUP != null)
			CHOOSELEVELUICLOSEUP();
	}

	void OnChooseLevelUICleanUp(int i)
	{
		if(CHOOSELEVELUICLEANUP != null)
			CHOOSELEVELUICLEANUP();
	}

    #region 点击挑战按钮

    void OnChooseLevelUIEnterUp(int i)
	{
        switch (CurrentMode)
        {
            case MissionType.Normal:
                {
                    OnEnterNormal();
                }
                break;

            case MissionType.FoggyAbyss:
                {
                    OnEnterFoggyAbyss();
                }
                break;
        }      
	}

    void OnEnterNormal()
    {
        if (m_EnterTimes > 0)
        {
            EventDispatcher.TriggerEvent(InstanceUIEvent.OnEnterNormalUp);
        }
        else
        {
            switch ((LevelType)m_LevelChooseType)
            {
                case LevelType.Simple:
                    {
                        EventDispatcher.TriggerEvent(InstanceUIEvent.OnEnterNormalUp);
                    } break;
                case LevelType.Difficult:
                    {
                        if (RESETMISSIONUP != null)
                            RESETMISSIONUP();
                    } break;
            }
        }
    }

    void OnEnterFoggyAbyss()
    {
        EventDispatcher.TriggerEvent(InstanceUIEvent.OnEnterFoggyAbyssUp);
    }

    #endregion

    void OnChooseLevelUIHelpUp(int i)
	{
		if(CHOOSELEVELUIHELPUP != null)
			CHOOSELEVELUIHELPUP();
	}

    void OnChooseLevelUIMissionFlagUp(int i)
    {
        if (CHOOSELEVELUIMISSIONFLAGUP != null)
            CHOOSELEVELUIMISSIONFLAGUP();
    }

	void OnInstanceChooseLeve0Up(int i)
	{
		if(INSTANCECHOOSELEVEL0UP != null)
			INSTANCECHOOSELEVEL0UP();
	}

	void OnInstanceChooseLeve1Up(int i)
	{
		if (INSTANCECHOOSELEVEL1UP != null)
			INSTANCECHOOSELEVEL1UP();
	}

    void OnChooseLevelUIRewardItemUp(int i)
    {
        if (CHOOSELEVELREWARDITEMUP != null)
            CHOOSELEVELREWARDITEMUP(i);
    }

   void OnCanNotResetMissionWindowConfirmUp(int i)
   {
       ShowCanNotResetMissionWindow(false);
   }

    void OnCanResetMissionWindowConfirmUp(int i)
    {
        if (CANRESETMISSIONWINDOWCONFIRMUP != null)
            CANRESETMISSIONWINDOWCONFIRMUP();

        ShowCanResetMissionWindow(false);
    }

    void OnCanResetMissionWindowCancelUp(int i)
    {
        ShowCanResetMissionWindow(false);
    }

	#endregion
  
    #region 界面信息设置

    private void SetUITexture(string UIName, string imageName)
    {
        var s = m_myTransform.Find(UIName).GetComponentsInChildren<UISlicedSprite>(true);
        if (s != null)
            s[0].spriteName = imageName;
    }

    private void SetUIText(string UIName, string text)
    {
        var l = m_myTransform.Find(UIName).GetComponentsInChildren<UILabel>(true);
        if (l != null)
        {
            l[0].text = text;
            l[0].transform.localScale = new Vector3(30, 30, 30);
        }
    }

    /// <summary>
    /// 是否显示助阵按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBtnHelp(bool isShow)
    {
        m_goInstanceLevelChooseUIBtnHelp.SetActive(false);
    } 

    /// <summary>
    /// 设置解锁等级
    /// </summary>
    /// <param name="recommendLevel"></param>
    public void SetInstanceLevelRecommendLevel(string recommendLevel, bool bShow = true)
    {
        m_lblInstanceLevelChooseUIReqLevel.text = recommendLevel + LanguageData.GetContent(46965);
        m_lblInstanceLevelChooseUIReqLevel.gameObject.SetActive(bShow);
    }

    /// <summary>
    /// 设置体力消耗
    /// </summary>
    /// <param name="vp"></param>
    /// <param name="bShow"></param>
    public void SetInstanceLevelCostVP(int vp, bool bShow = true)
    {
        m_lblInstanceLevelChooseUICostVP.text = LanguageData.GetContent(46964) + vp;
        m_lblInstanceLevelChooseUICostVP.gameObject.SetActive(bShow);
    }

    /// <summary>
    /// 设置副本名称
    /// </summary>
    /// <param name="name"></param>
    public void SetInstanceChooseGridTitle(string name, int level)
    {
        string itemRewardTitle = name;
        string cardRewardTitle = LanguageData.GetContent(46984);

        switch (CurrentMode)
        {
            case MissionType.Normal:
                switch ((LevelType)level)
                {
                    case LevelType.Simple:
                        {
                            itemRewardTitle = string.Concat(itemRewardTitle, LanguageData.GetContent(46968));
                            cardRewardTitle = string.Concat(cardRewardTitle, LanguageData.GetContent(46968));
                        } break;
                    case LevelType.Difficult:
                        {
                            itemRewardTitle = string.Concat(itemRewardTitle, LanguageData.GetContent(46969));
                            cardRewardTitle = string.Concat(cardRewardTitle, LanguageData.GetContent(46969));
                        } break;
                }
                break;

            case MissionType.FoggyAbyss:
                switch ((FoggyAbyssLevelType)level)
                {
                    case FoggyAbyssLevelType.Abyss:
                        itemRewardTitle = string.Concat(itemRewardTitle, LanguageData.GetContent(48514));
                        cardRewardTitle = string.Concat(cardRewardTitle, LanguageData.GetContent(48514));
                        break;

                    case FoggyAbyssLevelType.Hell:
                        itemRewardTitle = string.Concat(itemRewardTitle, LanguageData.GetContent(48515));
                        cardRewardTitle = string.Concat(cardRewardTitle, LanguageData.GetContent(48515));
                        break;

                    case FoggyAbyssLevelType.Purgatory:
                        itemRewardTitle = string.Concat(itemRewardTitle, LanguageData.GetContent(48516));
                        cardRewardTitle = string.Concat(cardRewardTitle, LanguageData.GetContent(48516));
                        break;
                }
                break;
        }

        m_lblInstanceLevelChooseUIItemRewardUITitle.text = itemRewardTitle;
        m_lblInstanceLevelChooseUICardRewardUITitle.text = cardRewardTitle;
    }

    /// <summary>
    /// 设置副本评价
    /// </summary>
    /// <param name="num"></param>
    public void ShowStars(int num)
    {
        string texName = "";
        switch ((InstanceStar)num)
        {
            case InstanceStar.InstanceStarS:
                {
                    texName = "fb-ds.png";

                    if (CurrentMode == MissionType.Normal)
                        ShowInstanceLevelChooseUIBtnClean(true);
                }
                break;
            case InstanceStar.InstanceStarA:
                {
                    texName = "fb-da.png";

                    if (CurrentMode == MissionType.Normal)
                        ShowInstanceLevelChooseUIBtnClean(true);
                }
                break;
            case InstanceStar.InstanceStarB:
                {
                    texName = "fb-db.png";

                    if (CurrentMode == MissionType.Normal)
                        ShowInstanceLevelChooseUIBtnClean(true);
                }
                break;
            case InstanceStar.InstanceStarC:
                {
                    texName = "fb-dc.png";

                    if (CurrentMode == MissionType.Normal)
                        ShowInstanceLevelChooseUIBtnClean(false);
                }
                break;
            default:
                {
                    texName = "";

                    if (CurrentMode == MissionType.Normal)
                        ShowInstanceLevelChooseUIBtnClean(false);
                }
                break;
        }

        m_texInstanceLevelChooseUIStarType.mainTexture = null;

        if (!string.IsNullOrEmpty(texName))
        {
            AssetCacheMgr.GetResourceAutoRelease(texName, (obj) =>
            {
                if (obj != null)
                {
                    m_texInstanceLevelChooseUIStarType.mainTexture = obj as Texture;
                    m_texInstanceLevelChooseUIStarType.transform.localScale = new Vector3(
                        512,512,1);
                }
            });
            m_texInstanceLevelChooseUIStarType.gameObject.SetActive(true);

            if (m_texInstanceLevelChooseUIStarTypeBG.mainTexture == null)
            {
                AssetCacheMgr.GetResourceAutoRelease("fb_jx.png", (obj) =>
                {
                    if (obj != null)
                    {
                        m_texInstanceLevelChooseUIStarTypeBG.mainTexture = obj as Texture;
                        //m_texInstanceLevelChooseUIStarTypeBG.transform.localScale = new Vector3(
                        //    430,512,1);
                    }
                });
            }
            m_texInstanceLevelChooseUIStarTypeBG.gameObject.SetActive(true);
        }
        else
        {
            m_texInstanceLevelChooseUIStarType.gameObject.SetActive(false);
            m_texInstanceLevelChooseUIStarTypeBG.gameObject.SetActive(false);
        }
    }   

    /// <summary>
    /// 设置掉落信息
    /// </summary>
    /// <param name="info"></param>
    public void SetDropInfo(string info)
    {
        m_lblInstanceLevelChooseUIDrop.text = info;
    }

    /// <summary>
    /// 设置是否显示任务标记
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowMissionFlagBtn(bool isShow)
    {
        m_goInstanceLevelChooseUIBtnMissionFlag.SetActive(isShow);
    }  

    /// <summary>
    /// 设置玩家NO.1
    /// </summary>
    /// <param name="isShow">如果没有NO.1的信息则不显示</param>
    /// <param name="name"></param>
    /// <param name="score"></param>
    /// <param name="vocation"></param>
    public void SetInstanceLevelChooseUIPlayerNO1(bool isShow, string name = "", string score = "", string vocation = "")
    {
        if (m_goGOInstanceLevelChooseUIPlayerNO1 == null)
            m_goGOInstanceLevelChooseUIPlayerNO1 = m_myTransform.Find(m_widgetToFullName["GOInstanceLevelChooseUIPlayerNO1"]).gameObject;
        if (m_lblInstanceLevelChooseUIPlayerNO1Name == null)
            m_lblInstanceLevelChooseUIPlayerNO1Name = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIPlayerNO1Name"]).GetComponent<UILabel>();
        if (m_lblInstanceLevelChooseUIPlayerNO1Score == null)
            m_lblInstanceLevelChooseUIPlayerNO1Score = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIPlayerNO1Score"]).GetComponent<UILabel>();
        if (m_lblInstanceLevelChooseUIPlayerNO1Vocation == null)
            m_lblInstanceLevelChooseUIPlayerNO1Vocation = m_myTransform.Find(m_widgetToFullName["InstanceLevelChooseUIPlayerNO1Vocation"]).GetComponent<UILabel>();

        m_goGOInstanceLevelChooseUIPlayerNO1.SetActive(isShow);
        m_lblInstanceLevelChooseUIPlayerNO1Name.text = name;
        m_lblInstanceLevelChooseUIPlayerNO1Score.text = score;
        m_lblInstanceLevelChooseUIPlayerNO1Vocation.text = vocation;
    }

    /// <summary>
    /// 是否显示[特殊副本-迷雾深渊]tip
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowMissionFoggyAbyssTip(bool isShow)
    {
        m_lblMissionFoggyAbyssTip.gameObject.SetActive(isShow);
    }

    #endregion

    #region 挑战按钮和扫荡按钮

    /// <summary>
    /// 设置挑战按鈕文字[次数]
    /// </summary>
    /// <param name="missionType">副本类型</param>
    /// <param name="times">[普通副本]剩余挑战次数</param>
    /// <param name="level">[普通副本]副本难度</param>
    /// <param name="isFoggyAbyssSuccess">[特殊副本-迷雾深渊]是否挑战完毕</param>
    int m_EnterTimes = 0;
    public void SetEnterTimes(MissionType missionType, int times, int level, bool isFoggyAbyssSuccess = false)
    {
        switch (missionType)
        {
            case MissionType.Normal:
                {
                    SetEnterNormalTimes(times, level);
                }
                break;

            case MissionType.FoggyAbyss:
                {
                    SetEnterFoggyAbyssTimes(isFoggyAbyssSuccess);
                }
                break;
        }        
    }

    /// <summary>
    /// 设置[普通副本]挑战按鈕文字[次数]
    /// </summary>
    private void SetEnterNormalTimes(int times, int level)
    {
        m_EnterTimes = times;
        m_LevelChooseType = level;

        if (m_EnterTimes > 0)
        {
            // "挑战{0}"
            m_lblInstanceLevelChooseUIBtnEnter.text = LanguageData.GetContent(46960, m_EnterTimes);
        }
        else
        {
            switch ((LevelType)level)
            {
                case LevelType.Simple:
                    {
                        // 普通难度无法重置挑战-"挑战{0}"
                        m_lblInstanceLevelChooseUIBtnEnter.text = LanguageData.GetContent(46960, 0);
                    } break;
                case LevelType.Difficult:
                    {
                        // 精英难度可以重置挑战-"重置挑战"
                        m_lblInstanceLevelChooseUIBtnEnter.text = LanguageData.GetContent(46961);
                    } break;
                default:
                    {
                        // 其他难度-"挑战{0}"
                        m_lblInstanceLevelChooseUIBtnEnter.text = LanguageData.GetContent(46960, 0);
                    } break;
            }
        }
    }

    /// <summary>
    /// 设置[特殊副本-迷雾深渊]挑战按鈕文字
    /// </summary>
    private void SetEnterFoggyAbyssTimes(bool isFoggyAbyssSuccess)
    {
        if(!isFoggyAbyssSuccess)
            m_lblInstanceLevelChooseUIBtnEnter.text = LanguageData.GetContent(48507);
        else
            m_lblInstanceLevelChooseUIBtnEnter.text = LanguageData.GetContent(48508);
    }

    /// <summary>
    /// 设置挑战按钮是否可用
    /// </summary>
    /// <param name="enable"></param>
    public void SetEnterEnable(bool enable)
    {
        m_spInstanceLevelChooseUIBtnEnterBGUp.ShowAsWhiteBlack(!enable);
        m_goInstanceLevelChooseUIBtnEnter.GetComponentsInChildren<BoxCollider>(true)[0].enabled = enable;
    }

    /// <summary>
    /// 是否显示扫荡按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowInstanceLevelChooseUIBtnClean(bool isShow)
    {
        if (isShow)
        {
            m_goInstanceLevelChooseUIBtnClean.gameObject.SetActive(true);
            m_goInstanceLevelChooseUIBtnEnter.transform.localPosition = m_goInstanceLevelChooseUIBtnEnterPosRight.transform.localPosition;
        }
        else
        {
            m_goInstanceLevelChooseUIBtnClean.gameObject.SetActive(false);
            m_goInstanceLevelChooseUIBtnEnter.transform.localPosition = m_goInstanceLevelChooseUIBtnEnterPosCenter.transform.localPosition;
        }
    }   

    /// <summary>
    /// 设置扫荡次数
    /// </summary>
    /// <param name="times"></param>
    public void SetCleanTimes(int times)
    {
        m_lblInstanceLevelChooseUIBtnClean.text = LanguageData.GetContent(46962, times);//"扫荡{0}";
    }

    /// <summary>
    /// 设置扫荡按钮是否可用
    /// </summary>
    /// <param name="enable"></param>
    public void SetCleanEnable(bool enable)
    {
        m_spInstanceLevelChooseUIBtnCleanBGDown.ShowAsWhiteBlack(!enable);
        m_spInstanceLevelChooseUIBtnCleanBGUp.ShowAsWhiteBlack(!enable);

        m_goInstanceLevelChooseUIBtnClean.GetComponentsInChildren<BoxCollider>(true)[0].enabled = enable;
    }

    #endregion

    #region 难度选择按钮

    private int m_LevelChooseType = (int)LevelType.Simple;

    /// <summary>
    /// 是否显示难度选择按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBtnLevelChoose(bool isShow = true)
    {
        m_goInstanceLevelChooseLevel.SetActive(isShow);
    }

    /// <summary>
    /// 设置难度是否开启
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="enable"></param>
    public void SetBtnLevelChooseEnable(int gridId, bool enable)
    {
        LoggerHelper.Debug("Setting InstanceLevelGridEnable!!!!!!!!!!!!!!!!!!!!" + gridId + " " + enable);
        if (gridId >= 0 && gridId < m_arrInstanceLevel.Length)
        {
            m_arrInstanceLevel[gridId].GetComponentsInChildren<NewInstanceLevelGrid>(true)[0].SetEnable(enable);
        }
    }

    /// <summary>
    /// 设置难度选择
    /// </summary>
    /// <param name="id"></param>
    public void SelectedBtnLevelChoose(int id)
    {
        //Debug.LogError("SetInstanceLevelChoose: " + id);
        if (id == -1)
        {
            for (int i = 0; i < 2; i++)
            {
                m_arrInstanceLevel[i].GetComponent<NewInstanceLevelGrid>().SetChoose(false);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                if (i == id)
                    m_arrInstanceLevel[i].GetComponent<NewInstanceLevelGrid>().SetChoose(true);
                else
                    m_arrInstanceLevel[i].GetComponent<NewInstanceLevelGrid>().SetChoose(false);
            }
        }

        switch ((LevelType)id)
        {
            case LevelType.Simple:
                {
                    m_goInstanceLevelChooseUIDropLevel0BG.SetActive(true);
                    m_goInstanceLevelChooseUIDropLevel1BG.SetActive(false);
                    m_spInstanceLevelChooseUIBGFG1.spriteName = "ty_tip";
                } break;
            case LevelType.Difficult:
                {
                    m_goInstanceLevelChooseUIDropLevel0BG.SetActive(false);
                    m_goInstanceLevelChooseUIDropLevel1BG.SetActive(true);
                    m_spInstanceLevelChooseUIBGFG1.spriteName = "jingyingfuben";
                } break;
        }
    }

    /// <summary>
    /// 设置当前任务提示
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="level"></param>
    public void ShowBtnLevelChooseTip(bool isShow, int level = 0, int tipTextID = 46979)
    {
        m_lblInstanceLevelChooseLevel0TipText.text = LanguageData.GetContent(tipTextID);
        m_lblInstanceLevelChooseLevel1TipText.text = LanguageData.GetContent(tipTextID);

        if (isShow)
        {
            switch ((LevelType)level)
            {
                case LevelType.Simple:
                    {
                        m_goInstanceLevelChooseLevel0Tip.SetActive(true);
                        m_goInstanceLevelChooseLevel1Tip.SetActive(false);
                    } break;
                case LevelType.Difficult:
                    {
                        m_goInstanceLevelChooseLevel0Tip.SetActive(false);
                        m_goInstanceLevelChooseLevel1Tip.SetActive(true);
                    } break;
                default:
                    {
                        m_goInstanceLevelChooseLevel0Tip.SetActive(false);
                        m_goInstanceLevelChooseLevel1Tip.SetActive(false);
                    }
                    break;
            }
        }
        else
        {
            m_goInstanceLevelChooseLevel0Tip.SetActive(false);
            m_goInstanceLevelChooseLevel1Tip.SetActive(false);
        }
    }

    #endregion

    #region  重置界面

    /// <summary>
    /// 是否打开不可重置界面
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowCanNotResetMissionWindow(bool isShow)
    {
        m_goInstanceLevelChooseResetMissionWindow.SetActive(isShow);
        m_goInstanceLevelChooseCanNotResetMissionWindow.SetActive(isShow);
    }

    /// <summary>
    /// 是否打开可重置界面
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowCanResetMissionWindow(bool isShow)
    {
        m_goInstanceLevelChooseResetMissionWindow.SetActive(isShow);
        m_goInstanceLevelChooseCanResetMissionWindow.SetActive(isShow);
    }

    /// <summary>
    /// 设置可重置界面Info
    /// </summary>
    public void SetCanResetMissionWindowText(string text)
    {       
        m_lblNInstanceLevelChooseCanResetMissionWindowInfo1Text.text = text;

        m_goGONInstanceLevelChooseCanResetMissionWindowInfo2.SetActive(false);
        m_goGONInstanceLevelChooseCanResetMissionWindowInfo1.SetActive(true);
    }

    /// <summary>
    /// 设置可重置界面消耗和剩余重置次数
    /// </summary>
    /// <param name="cost"></param>
    /// <param name="lastTimes"></param>
    public void SetCanResetMissionWindowCostAndTimes(int cost, int lastTimes)
    {
        m_lblNInstanceLevelChooseCanResetMissionWindowTextNum.text = cost.ToString();
        m_lblNInstanceLevelChooseCanResetMissionWindowTipNum.text = lastTimes.ToString();

        m_goGONInstanceLevelChooseCanResetMissionWindowInfo1.SetActive(false);
        m_goGONInstanceLevelChooseCanResetMissionWindowInfo2.SetActive(true);
    }

    /// <summary>
    /// 设置不可可重置界面Info
    /// </summary>
    public void SetCanNotResetMissionWindowText(string text)
    {
        m_lblNInstanceLevelChooseCanNotResetMissionWindowText.text = text;
    }     

    #endregion       

    #region 物品奖励

    readonly static float ITEMREWARD_SPACE = 138;
    readonly static int ITEMREWARD_OffsetX = -205;
    readonly static int ITEMREWARD__COUNT_ONE_PAGE = 4;
    private Dictionary<int, GameObject> m_maplistGORewardItem = new Dictionary<int, GameObject>();

    /// <summary>
    /// 设置物品奖励
    /// </summary>
    /// <param name="idList"></param>
    public void SetRewardItemIDList(List<int> idList)
    {
        AddRewardItemList(idList.Count, () =>
        {
            SetRewardItemDataList(idList);
        });
    }

    /// <summary>
    /// 添加奖励物品
    /// </summary>
    /// <param name="num"></param>
    /// <param name="act"></param>
    void AddRewardItemList(int num, Action act = null)
    {
        ClearDragRewardItemList();
        ResetRewardItemListCameraPos();
        SetDragRewardListSetPos(num);

        // 删除翻页位置
        m_dragRewardListMyDragableCamera.DestroyMovePagePosList();       

        for (int i = 0; i < num; ++i)
        {
            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

            int index = i;
            AssetCacheMgr.GetUIInstance("InstanceRewardItem.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranGOInstanceLevelChooseUIDragRewardListSet;
                obj.transform.localPosition = new Vector3(ITEMREWARD_SPACE * index + ITEMREWARD_OffsetX, 0, 0);
                obj.transform.localScale = new Vector3(0.8f,  0.8f, 1);

                Transform tranCount = obj.transform.Find("InstanceRewardItemCount");
                if (tranCount != null)
                    tranCount.gameObject.SetActive(false);

                Transform tranName = obj.transform.Find("InstanceRewardItemText");
                if (tranName != null)
                    tranName.gameObject.SetActive(false);

                obj.SetActive(true);
                if (m_maplistGORewardItem.ContainsKey(index))
                    AssetCacheMgr.ReleaseInstance(m_maplistGORewardItem[index]);
                m_maplistGORewardItem[index] = (obj);
                Mogo.Util.LoggerHelper.Debug("AddRewardItem !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + index + " " + num);
                m_dragRewardListMyDragableCamera.SetArrow();

                // 奖励数量大于4时,创建拖动效果
                if (num > ITEMREWARD__COUNT_ONE_PAGE)
                {
                    MyDragCamera myDragCamera = obj.AddComponent<MyDragCamera>();
                    myDragCamera.RelatedCamera = m_dragRewardListCamera;

                    if (m_maplistGORewardItem.Count >= ITEMREWARD__COUNT_ONE_PAGE)
                    {
                        m_dragRewardListMyDragableCamera.MAXX =
                            ITEMREWARD_OffsetX + (m_maplistGORewardItem.Count - ITEMREWARD__COUNT_ONE_PAGE) * ITEMREWARD_SPACE;
                    }
                    else
                    {
                        m_dragRewardListMyDragableCamera.MAXX = ITEMREWARD_OffsetX;
                    }

                    // 创建翻页位置
                    if (index % ITEMREWARD__COUNT_ONE_PAGE == 0)
                    {
                        GameObject trans = new GameObject();
                        trans.transform.parent = m_dragRewardListCamera.transform;
                        trans.transform.localPosition = new Vector3(index / ITEMREWARD__COUNT_ONE_PAGE * ITEMREWARD_SPACE * ITEMREWARD__COUNT_ONE_PAGE, 0, 0);
                        trans.transform.localEulerAngles = Vector3.zero;
                        trans.transform.localScale = new Vector3(1, 1, 1);
                        trans.name = "DragRewardListPosHorizon" + index / ITEMREWARD__COUNT_ONE_PAGE;
                        m_dragRewardListMyDragableCamera.transformList.Add(trans.transform);
                        m_dragRewardListMyDragableCamera.SetCurrentPage(0);
                        m_dragRewardListMyDragableCamera.SetArrow();
                    }
                }

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                if (index == num - 1)
                {
                    if (act != null)
                    {
                        act();
                    }
                }
            });
        }
    }

    /// <summary>
    /// 设置物品信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="imgName"></param>
    /// <param name="itemName"></param>
    private void SetRewardItemDataList(List<int> idList)
    {
        for (int index = 0; index < idList.Count; index++)
        {
            int id = idList[index];
            if (id > 0)
            {
                InventoryGrid inventoryGrid = m_maplistGORewardItem[index].GetComponent<InventoryGrid>();
                if (inventoryGrid == null)
                    inventoryGrid = m_maplistGORewardItem[index].AddComponent<InventoryGrid>();
                inventoryGrid.iconID = id;

                UISprite spIcon = m_maplistGORewardItem[index].transform.Find("InstanceRewardItemFG").GetComponentsInChildren<UISprite>(true)[0];
                UISprite spBG = m_maplistGORewardItem[index].transform.Find("InstanceRewardItemBG").GetComponentsInChildren<UISprite>(true)[0];
                InventoryManager.SetIcon(id, spIcon, 0, null, spBG);
            }
        }      
    }

    /// <summary>
    /// 清除奖励物品
    /// </summary>
    void ClearDragRewardItemList()
    {
        Mogo.Util.LoggerHelper.Debug("ClearRewardItemList " + m_maplistGORewardItem.Count);
        for (int i = 0; i < m_maplistGORewardItem.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_maplistGORewardItem[i]);
        }

        m_maplistGORewardItem.Clear();
    }
   
    /// <summary>
    /// 设置物品列表的位置
    /// </summary>
    /// <param name="num"></param>
    void SetDragRewardListSetPos(int num)
    {
        switch (num)
        {
            case 1:
                m_tranGOInstanceLevelChooseUIDragRewardListSet.localPosition = m_goInstanceLevelPosItem1.transform.localPosition;
                break;
            case 2:
                m_tranGOInstanceLevelChooseUIDragRewardListSet.localPosition = m_goInstanceLevelPosItem2.transform.localPosition;
                break;
            case 3:
                m_tranGOInstanceLevelChooseUIDragRewardListSet.localPosition = m_goInstanceLevelPosItem3.transform.localPosition;
                break;
            case 4:
                m_tranGOInstanceLevelChooseUIDragRewardListSet.localPosition = m_goInstanceLevelPosItem4.transform.localPosition;
                break;
            default:
                m_tranGOInstanceLevelChooseUIDragRewardListSet.localPosition = m_goInstanceLevelPosItem4.transform.localPosition;
                break;
        }
    }

    /// <summary>
    /// 重置物品奖励相机位置
    /// </summary>
    void ResetRewardItemListCameraPos()
    {
        m_dragRewardListCamera.transform.localPosition = m_dragRewardListCameraInitPos;
    }

    #endregion

    #region 翻牌奖励

    readonly static float CARDREWARD_SPACE = 150;
    readonly static int CARDREWARD_OffsetX = -225;
    readonly static int CARDREWARD_COUNT_ONE_PAGE = 4;
    private Dictionary<int, GameObject> m_maplistGOCardReward = new Dictionary<int, GameObject>();
  
    /// <summary>
    /// 添加翻牌奖励
    /// </summary>
    /// <param name="num"></param>
    /// <param name="act"></param>
    public void AddDragCardReward(int num, Action act = null)
    {
        ClearCardRewardList();
        ResetCardRewardListCameraPos();

        // 删除翻页位置
        m_dragCardRewardListDragableCamera.DestroyMovePagePosList();

        for (int i = 0; i < num; ++i)
        {
            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

            int index = i;
            AssetCacheMgr.GetUIInstance("InstanceLevelChooseUICardReward.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranInstanceLevelChooseUICardRewardList;
                obj.transform.localPosition = new Vector3(CARDREWARD_SPACE * index + CARDREWARD_OffsetX, 0, 0);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);        

                obj.SetActive(true);
                if (m_maplistGOCardReward.ContainsKey(index))
                    AssetCacheMgr.ReleaseInstance(m_maplistGOCardReward[index]);
                m_maplistGOCardReward[index] = (obj);
                m_dragCardRewardListDragableCamera.SetArrow();

                // 奖励数量大于4时,创建拖动效果
                if (num > CARDREWARD_COUNT_ONE_PAGE)
                {
                    MyDragCamera dragCamera = obj.GetComponentsInChildren<MyDragCamera>(true)[0];
                    dragCamera.RelatedCamera = m_dragCardRewardListCamera;

                    if (m_maplistGOCardReward.Count >= CARDREWARD_COUNT_ONE_PAGE)
                    {
                        m_dragCardRewardListDragableCamera.MAXX =
                            ITEMREWARD_OffsetX + (m_maplistGOCardReward.Count - CARDREWARD_COUNT_ONE_PAGE) * CARDREWARD_SPACE;
                    }
                    else
                    {
                        m_dragCardRewardListDragableCamera.MAXX = ITEMREWARD_OffsetX;
                    }

                    // 创建翻页位置
                    if (index % ITEMREWARD__COUNT_ONE_PAGE == 0)
                    {
                        GameObject trans = new GameObject();
                        trans.transform.parent = m_dragCardRewardListCamera.transform;
                        trans.transform.localPosition = new Vector3(index * CARDREWARD_SPACE, 0, 0);
                        trans.transform.localEulerAngles = Vector3.zero;
                        trans.transform.localScale = new Vector3(1, 1, 1);
                        trans.name = "DragCardRewardListPosHorizon" + index / CARDREWARD_COUNT_ONE_PAGE;
                        m_dragCardRewardListDragableCamera.transformList.Add(trans.transform);
                        m_dragCardRewardListDragableCamera.SetCurrentPage(0);
                        m_dragCardRewardListDragableCamera.SetArrow();
                    }
                }

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                if (index == num - 1)
                {
                    if (act != null)
                    {
                        act();
                    }
                }
            });
        }
    }  

    /// <summary>
    /// 清除翻牌奖励
    /// </summary>
    void ClearCardRewardList()
    {
        Mogo.Util.LoggerHelper.Debug("ClearRewardItemList " + m_maplistGORewardItem.Count);
        for (int i = 0; i < m_maplistGOCardReward.Count; ++i)
        {
            if (m_maplistGOCardReward.ContainsKey(i))
                AssetCacheMgr.ReleaseInstance(m_maplistGOCardReward[i]);
        }

        m_maplistGOCardReward.Clear();
    }

    /// <summary>
    /// 重置翻牌相机位置
    /// </summary>
    void ResetCardRewardListCameraPos()
    {
        m_dragCardRewardListCamera.transform.localPosition = m_dragCardRewardListCameraInitPos;
    }

    /// <summary>
    /// 设置翻牌奖励名称
    /// </summary>
    /// <param name="list"></param>
    public void SetCardRewardDataList(List<int> itemIds, List<string> itemNames)
    {
        if (m_maplistGOCardReward.Count != itemIds.Count)      
            return;

        for (int i = 0; i < itemIds.Count; i++)
        {
            if (m_maplistGOCardReward.ContainsKey(i))
            {
                UILabel lblCardRewardName = m_maplistGOCardReward[i].transform.Find("InstanceLevelChooseUICardRewardText").GetComponentsInChildren<UILabel>(true)[0];
                UISprite spCardRewardItemBG = m_maplistGOCardReward[i].transform.Find("InstanceLevelChooseUICardRewardItemBox").GetComponentsInChildren<UISprite>(true)[0];
                UISprite spCardRewardItemIcon = m_maplistGOCardReward[i].transform.Find("InstanceLevelChooseUICardRewardItemFG").GetComponentsInChildren<UISprite>(true)[0];

                if (i < itemIds.Count && i < itemNames.Count)
                {
                    m_maplistGOCardReward[i].SetActive(true);
                    //m_maplistGOCardReward[i].GetComponentsInChildren<InventoryGrid>(true)[0].iconID = itemIds[i];
                    InventoryManager.SetIcon(itemIds[i], spCardRewardItemIcon, 0, null, spCardRewardItemBG);
                    lblCardRewardName.text = itemNames[i];
                }
                else
                {
                    m_maplistGOCardReward[i].SetActive(false);
                    lblCardRewardName.text = "";
                }
            }           
        }        
    }    

    #endregion

    #region 物品奖励和翻牌奖励切换

    private bool IsItemRewardUI = true;

    public void SwitchToItemRewardUI()
    {
        IsItemRewardUI = true;
        m_goGOInstanceLevelChooseUIItemRewardUI.SetActive(true);
        m_goGOInstanceLevelChooseUICardRewardUI.SetActive(false);
    }

    public void SwitchToCardRewardUI()
    {
        IsItemRewardUI = false;
        m_goGOInstanceLevelChooseUIItemRewardUI.SetActive(false);
        m_goGOInstanceLevelChooseUICardRewardUI.SetActive(true);
    }

    #endregion

    #region 切换模式[切换关卡类型]

    public MissionType CurrentMode = MissionType.Normal;

    /// <summary>
    /// 切换关卡类型
    /// </summary>
    /// <param name="mode"></param>
    public void ChangeMode(MissionType mode)
    {
        CurrentMode = mode;

        switch (mode)
        {
            case MissionType.Normal:
                ChangeModeToNormal();
                break;

            case MissionType.FoggyAbyss:
                ChangeModeToFoggyAbyss();
                break;
        }
    }

    /// <summary>
    /// 切换到[普通关卡]
    /// </summary>
    protected void ChangeModeToNormal()
    {
        ShowBtnLevelChoose(true);
        ShowMissionFoggyAbyssTip(false);
    }

    /// <summary>
    /// 切换到[特殊关卡-迷雾深渊]
    /// </summary>
    protected void ChangeModeToFoggyAbyss()
    {
        ShowBtnLevelChoose(false);
        ShowInstanceLevelChooseUIBtnClean(false);
        ShowMissionFlagBtn(false);
        ShowBtnLevelChooseTip(false);
        SetInstanceLevelCostVP(0, false);
        SetInstanceLevelRecommendLevel(String.Empty, false);
        ShowBtnHelp(false);
        InstanceUILogicManager.Instance.SetMercenaryGrid(null);
        SetInstanceLevelChooseUIPlayerNO1(false);
        ShowMissionFoggyAbyssTip(true);
    }

    #endregion

    #region 界面打开和关闭

    protected override void OnEnable()
    {
        base.OnEnable();
		if(m_goInstanceLevelChooseResetMissionWindow != null)
        	m_goInstanceLevelChooseResetMissionWindow.SetActive(false);
		
        EventDispatcher.TriggerEvent(Events.InstanceEvent.GetSweepTimes);
        //OnChooseLevelUISwtichToItemRewardUIUp(0); // 默认切换到物品奖励界面
    }

    #endregion    
}