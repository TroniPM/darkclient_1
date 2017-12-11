using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using System;
using Mogo.GameData;
using Mogo.Game;

public class BattleRecordData
{
    public string Title;
    public string OutputText;
    public string OutputNum;
    public string GoldText;
    public string GoldNum;
    public string ExpText;
    public string ExpNum;
};

public class SystemRequestLevel
{    
    public static readonly int MISSIONRANDOM = 35; // �������35������
    public static readonly int OCCPUYTOWER = 30; // PVP
    public static readonly int OGREMUSTDIE = 20; // ���˱�����
    public static readonly int DRAGONMATCH = 15; // ��������
    public static readonly int CILMBTOWER = 15; // ����֮��15������
    public static readonly int DOOROFBURY = 25; // ����֮��
    public static readonly int SANCTUARY = 20; // ʥ������ս

    // TopLeft
    public static readonly int RankingIcon = 30; // ���а�-30

    // TopRight:Right->Left
    public const int AutoTaskPlayIcon = 0; // �Զ�����-0
    public const int DailyTaskIcon = 21;  // ÿ������-21
    public const int ChallengeIcon = 15; // �(ԭ������ս)-15
    public const int ArenaIcon = 23; // ������-23
    public const int ChargeRewardIcon = 0; // ����-0

    // BottomRight:Down->Up
    public const int AttributeIcon = 7; // �ɾ�-7
    public const int MallConsumeIcon = 0; // �̳�-0

    // BottomRight:Right->Left
    public const int PackageIcon = 0; // ����-0
    public const int EquipmentIcon = 4; // ����-4
    public const int DragonIcon = 20; // ����-20
    public const int DiamondToGoldIcon = 17; // ����-17
    public const int SpriteIcon = 30; // ����ϵͳ-30
    public const int WingIcon = 20; // ���(�����ݵȼ�)
}


public class NormalMainUIViewManager : MogoUIBehaviour
{
    private static NormalMainUIViewManager m_instance;
    public static NormalMainUIViewManager Instance { get { return NormalMainUIViewManager.m_instance;} }

    private static int INSTANCE_COUNT = 0; // �첽������Դ����    

    #region ����

    private GameObject m_goAssistantDialog;
    private GameObject m_goSendHelp;
    private GameObject m_goDoorOfBuryOpenTip;
    private GameObject m_goSanctuaryOpenTip;
    private GameObject m_goMissionFoggyAbyssOpenTip;
    private GameObject m_goAutoTaskBtn;  

    // ��ֵ����or�
    private UILabel m_lblChargeRewardIconText;
    private UISprite m_spChargeRewardIconBGUp;

    private List<GameObject> m_listFaceIcon = new List<GameObject>();    

    //�罻����Ϣ��ʾ
    private GameObject m_socialTip;
    // Ϊȥ��������ʱ�������´���
    //bool m_bAssistantDialogShown = false;

    private GameObject m_goGOPVEPlayIconNotice;
    private GameObject m_goGOPVPPlayIconNotice;
    private GameObject m_goGOChargeRewardIconNotice;
    private GameObject m_goMallConsumeIconNotice;

    private UILabel m_lblVIPLevel;
    private UILabel m_lblDoorOfBuryTipTitle;
    private UILabel m_lblDoorOfBuryTip1;
    private UILabel m_lblDoorOfBuryTip2;
    private UILabel m_lblAssistantDialog;
    private UILabel m_lblNewChatLabel;
    private UILabel m_lblPlayerBloodText;

    private UILabel m_lblContributeTipCurrentRewardNum;// ��ǰ����
    private UILabel m_lblContributeTipNextRewardConditionNum;// �¼��������蹱��
    private UILabel m_lblContributeTipNextRewardNum;// �¼�����
    private UILabel m_lblContributeTipTitle;
    private UILabel m_lblContributeTipTodayRank;
    private UILabel m_lblContributeTipWeekRank;
    private UISprite m_spCurrentRewardItem;
    private UISprite m_spNextRewardItem;
    private UISprite m_spVipLevelNum1;
    private UISprite m_spVipLevelNum2;

    private UILabel m_goAssistantDialogText;

    private GameObject m_goContributeTipOKBtn;
    private GameObject m_goContributeTip;

    private GameObject m_goGODiamondToGoldIconLast;
    private UILabel m_lblDiamondToGoldIconLastTimes;

    private GameObject m_goNormalMainUIVIPBuffBtn;
    private GameObject m_goGONormalMainUIVIPBuffInfo;
    private UILabel m_lblNormalMainUIVIPBuffName;
    private UILabel m_lblNormalMainUIVIPBuffInfoLevel;
    private UILabel m_lblNormalMainUIVIPBuffInfoLastTime;
    private UISprite m_spNormalMainUIVIPBuffInfoLevelVIPNum0;
    private UISprite m_spNormalMainUIVIPBuffInfoLevelVIPNum1;

    private GameObject m_goCommunity;
    private UILabel m_goCommunityLabel;
    private UISprite m_spCommunityIconBGUp;
    private UISprite m_spCommunityIconBGDown;

    private GameObject m_goNormalMainUIPlayerExpList;
    private GameObject m_goGONormalMainUIPlayerExpListFG;
    private UILabel m_lblNormalMainUIPlayerExpListNum;
    private List<UISprite> m_listNormalMainUIPlayerExpFG = new List<UISprite>();

    // ����֮�ź;�����CD��ʾ
    private GameObject m_goArenaCDTipBtnOpen;
    private GameObject m_goArenaCDTipCountDown;
    private UILabel m_lblArenaCDTipCountDownName;
    private UILabel m_lblArenaCDTipCountDownNum;

    private GameObject m_goDoorOfBuryOpenCDTipBtnOpen;
    private GameObject m_goDoorOfBuryOpenCDTipCountDown;
    private UILabel m_lblDoorOfBuryOpenCDTipCountDownName;
    private UILabel m_lblDoorOfBuryOpenCDTipCountDownNum;

    private UILabel m_lblUpgradePowerCurrentNum;

    private Camera m_UICamera;

    private UILabel m_energyString;
    private UIFilledSprite m_spNormalMainUIPlayerStrenthFG;

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    
    #endregion        

    #region ���

    // ���а�
    private GameObject m_goNormalMainUIRanking;
    private GameObject m_goNormalMainUIRankingBtn;
    private Transform m_tranNormalMainUIRankingBtnOriginPos;
    private Transform m_tranNormalMainUIRankingBtnPos;
    private GameObject m_goNormalMainUIPlayerVIP;
    private Transform m_tranNormalMainUIPlayerVIPPos1;
    private Transform m_tranNormalMainUIPlayerVIPPos2;

    public override void CallWhenCreate()
    {
        m_myTransform.Find("BottomLeft/Controller").gameObject.AddComponent<ControlStick>();

        m_goAssistantDialog = m_myTransform.Find(m_widgetToFullName["AssistantDialog"]).gameObject;

        m_goAssistantDialogText = m_myTransform.Find(m_widgetToFullName["AssistantDialogText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goAssistantDialogText.text = LanguageData.GetContent(46008);

        m_goSendHelp = m_myTransform.Find(m_widgetToFullName["SendHelp"]).gameObject;
        m_goDoorOfBuryOpenTip = m_myTransform.Find(m_widgetToFullName["DoorOfBuryOpenTip"]).gameObject;
        m_goDoorOfBuryOpenTipWord = m_myTransform.Find(m_widgetToFullName["DoorOfBuryOpenTipWord"]).gameObject;
        m_goDoorOfBuryOpenTipWord.AddComponent<DoorOfBuryTipUIMgr>().Initialize();
        m_socialTip = m_myTransform.Find(m_widgetToFullName["SocialTip"]).gameObject;
        m_goSanctuaryOpenTip = FindTransform("SanctuaryOpenTip").gameObject;
        m_goMissionFoggyAbyssOpenTip = FindTransform("MissionFoggyAbyssOpenTip").gameObject;

        m_UICamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_myTransform.Find("Bottom").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = m_UICamera;
        m_myTransform.Find("BottomLeft").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = m_UICamera;
        m_myTransform.Find("BottomRight").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = m_UICamera;
        m_myTransform.Find("BottomLeft/Controller").GetComponentsInChildren<ControlStick>(true)[0].RelatedCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_myTransform.Find("Top").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = m_UICamera;
        m_myTransform.Find("TopLeft").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = m_UICamera;
        m_myTransform.Find("TopRight").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = m_UICamera;
        m_myTransform.Find("Center").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = m_UICamera;

        m_myTransform.Find(m_widgetToFullName["NormalMainUIBG"]).gameObject.AddComponent<TouchControll>();

        m_lblTipText = m_myTransform.Find(m_widgetToFullName["BattleRecordTipText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblDoorOfBuryTipTitle = m_myTransform.Find(m_widgetToFullName["DoorOfBuryOpenTipTitleText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblDoorOfBuryTip1 = m_myTransform.Find(m_widgetToFullName["DoorOfBuryOpenTipText1"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblDoorOfBuryTip2 = m_myTransform.Find(m_widgetToFullName["DoorOfBuryOpenTipText2"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblVIPLevel = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlayerVIP"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblAssistantDialog = m_myTransform.Find(m_widgetToFullName["AssistantDialogText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblNewChatLabel = m_myTransform.Find(m_widgetToFullName["NormalMainUICommunityText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblPlayerBloodText = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlayerBloodText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_lblContributeTipTitle = m_myTransform.Find(m_widgetToFullName["ContributeTipTitle"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblContributeTipTodayRank = m_myTransform.Find(m_widgetToFullName["TodayRank"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblContributeTipWeekRank = m_myTransform.Find(m_widgetToFullName["WeekRank"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblContributeTipCurrentRewardNum = m_myTransform.Find(m_widgetToFullName["CurrentRewardNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblContributeTipNextRewardConditionNum = m_myTransform.Find(m_widgetToFullName["NextRewardConditionNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblContributeTipNextRewardNum = m_myTransform.Find(m_widgetToFullName["NextRewardNum"]).GetComponentsInChildren<UILabel>(true)[0];

        // ChineseData
        SetContributeTipChineseData();
        FindTransform("EquipmentConsumeIconText").GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.GetContent(46852);
        FindTransform("PVPPlayIconText").GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.GetContent(46856);
        FindTransform("PVEPlayIconText").GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.GetContent(46857);

        m_spCurrentRewardItem = m_myTransform.Find(m_widgetToFullName["CurrentRewardItemFG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_spNextRewardItem = m_myTransform.Find(m_widgetToFullName["NextRewardItemFG"]).GetComponentsInChildren<UISprite>(true)[0];

        m_goContributeTip = m_myTransform.Find(m_widgetToFullName["ContributeTip"]).gameObject;
        m_goContributeTipOKBtn = m_myTransform.Find(m_widgetToFullName["ContributeTipOK"]).gameObject;

        m_fsPlayerBlood = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlayerBloodFG"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsPlayerAnger = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlayerAngerFG"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsPlayerExp = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlayerExpFG"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;

        m_spVipLevelNum1 = m_myTransform.Find(m_widgetToFullName["VipLevel"]).GetComponentsInChildren<UISprite>(true)[0];
        m_spVipLevelNum2 = m_myTransform.Find(m_widgetToFullName["VipLevel2"]).GetComponentsInChildren<UISprite>(true)[0];

        m_energyString = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlayerStrenthText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_spNormalMainUIPlayerStrenthFG = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlayerStrenthFG"]).GetComponentsInChildren<UIFilledSprite>(true)[0];

        m_goCommunity = m_myTransform.Find(m_widgetToFullName["NormalMainUICommunity"]).gameObject;
        m_goCommunityLabel = m_myTransform.Find(m_widgetToFullName["NormalMainUICommunityText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_spCommunityIconBGUp = m_myTransform.Find(m_widgetToFullName["NormalMainUICommunityIconBGUp"]).GetComponentsInChildren<UISprite>(true)[0];
        m_spCommunityIconBGDown = FindTransform("NormalMainUICommunityIconBGDown").GetComponentsInChildren<UISprite>(true)[0];

        // ������Ư�ư�ť
        {    
            m_maplistGoIconPlays[IconPlays.ControllerIcon] = 
                FindTransform("NoramlMainUIControllerIcon").gameObject;
            m_maplistGoIconPlays[IconPlays.NMCommunityIcon] =
                FindTransform("NMCommunity").gameObject;
            m_maplistGoIconPlays[IconPlays.NMHelpTip] =
                FindTransform("NMHelpTip").gameObject;


            FindTransform("HelpTip").gameObject.AddComponent<TipUIManager>();
        }

        // IconPlaysBottomRightH
        {
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.PackageIcon] =
                FindTransform("NMPackageIcon").gameObject;
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.EquipmentIcon] =
                FindTransform("NMEquipmentConsumeIcon").gameObject;
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.DragonIcon] =
                FindTransform("NMDragonConsumeIcon").gameObject;
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.DiamondToGoldIcon] =
                FindTransform("NMDiamondToGoldIcon").gameObject;
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.SpriteIcon] =
                FindTransform("NMSpriteIcon").gameObject;
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.WingIcon] =
                FindTransform("NMWingIcon").gameObject;
            m_goNMWingIcon2 = FindTransform("NMWingIcon2").gameObject;

            for (int i = 1; i <= IconPlaysBottomRightH.MaxCount; ++i)
            {
                m_listIconPlaysBottomRightHPos.Add(FindTransform("NoramlMainUIBTNListHPos" + i).gameObject);
            }
        }

        // IconPlaysBottomRightV
        {
            m_maplistIconPlaysBottomRightV[IconPlaysBottomRightV.AttributeIcon] =
                FindTransform("NMAttributeIcon").gameObject;
             m_maplistIconPlaysBottomRightV[IconPlaysBottomRightV.MallConsumeIcon] =
                FindTransform("NMMallConsumeIcon").gameObject;

             for (int i = 1; i <= IconPlaysBottomRightV.MaxCount; ++i)
             {
                 m_listIconPlaysBottomRightVPos.Add(FindTransform("NoramlMainUIBTNListVPos" + i).gameObject);
             }
        }

        // IconPlaysTopRightH
        {
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.AutoTaskPlayIcon] =
                FindTransform("AutoTaskPlayIcon").gameObject;
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.DailyTaskIcon] =
                FindTransform("XDailyTaskIcon").gameObject;
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.ChallengeIcon] =
                FindTransform("PVEPlayIcon").gameObject;
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.ArenaIcon] =
                FindTransform("PVPPlayIcon").gameObject;
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.ChargeRewardIcon] =
                FindTransform("ChargeRewardIcon").gameObject;

            for (int i = 0; i < IconPlaysTopRightH.MaxCount; ++i)
             {
                 m_listIconPlaysTopRightHPos.Add(FindTransform("NormalMainUIPlaysListPos" + i).gameObject);
             }
        }     

        // ���а��VIP
        m_goNormalMainUIRanking = FindTransform("NormalMainUIRanking").gameObject;
        m_goNormalMainUIRanking.SetActive(false);
        m_goNormalMainUIRankingBtn = FindTransform("NormalMainUIRankingBtn").gameObject;
        m_tranNormalMainUIRankingBtnOriginPos = FindTransform("NormalMainUIRankingBtnOriginPos");
        m_tranNormalMainUIRankingBtnPos = FindTransform("NormalMainUIRankingBtnPos");
        m_goNormalMainUIPlayerVIP = FindTransform("NormalMainUIPlayerVIP").gameObject;
        m_tranNormalMainUIPlayerVIPPos1 = FindTransform("NormalMainUIPlayerVIPPos1");
        m_tranNormalMainUIPlayerVIPPos2 = FindTransform("NormalMainUIPlayerVIPPos2");

        m_goAutoTaskBtn = m_myTransform.Find(m_widgetToFullName["AutoTaskPlayIcon"]).gameObject;
        m_myTransform.Find(m_widgetToFullName["XDailyTaskIcon"]).gameObject.AddComponent<DailyTaskIcon>();
        m_myTransform.Find(m_widgetToFullName["XDailyTaskIcon"]).BroadcastMessage("HideDailyTaskFinishedNotice", SendMessageOptions.DontRequireReceiver);

        m_goGOPVEPlayIconNotice = FindTransform("GOPVEPlayIconNotice").gameObject;
        m_goGOPVPPlayIconNotice = FindTransform("GOPVPPlayIconNotice").gameObject;
        m_goGOChargeRewardIconNotice = FindTransform("GOChargeRewardIconNotice").gameObject;
        m_goMallConsumeIconNotice = FindTransform("MallConsumeIconNotice").gameObject;
     
        m_goIconPlaysOriginPos = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlaysListOriginPos"]).gameObject;
        m_goIconPlaysNMOriginPos = m_myTransform.Find(m_widgetToFullName["NMBTNListOriginPos"]).gameObject;
        m_goIconPlaysNMHOriginPos = FindTransform("NMBTNListHOriginPos").gameObject;
        m_goIconPlaysNMVOriginPos = FindTransform("NMBTNListVOriginPos").gameObject;

        m_goGODiamondToGoldIconLast = m_myTransform.Find(m_widgetToFullName["GODiamondToGoldIconLast"]).gameObject;
        m_lblDiamondToGoldIconLastTimes = m_myTransform.Find(m_widgetToFullName["DiamondToGoldIconLastTimes"]).GetComponent<UILabel>();

        m_goNormalMainUIVIPBuffBtn = m_myTransform.Find(m_widgetToFullName["NormalMainUIVIPBuffBtn"]).gameObject;
        m_goGONormalMainUIVIPBuffInfo = m_myTransform.Find(m_widgetToFullName["GONormalMainUIVIPBuffInfo"]).gameObject;
        m_lblNormalMainUIVIPBuffName = m_myTransform.Find(m_widgetToFullName["NormalMainUIVIPBuffName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblNormalMainUIVIPBuffInfoLevel = m_myTransform.Find(m_widgetToFullName["NormalMainUIVIPBuffInfoLevel"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblNormalMainUIVIPBuffInfoLastTime = m_myTransform.Find(m_widgetToFullName["NormalMainUIVIPBuffInfoLastTime"]).GetComponentsInChildren<UILabel>(true)[0];
        m_spNormalMainUIVIPBuffInfoLevelVIPNum0 = m_myTransform.Find(m_widgetToFullName["NormalMainUIVIPBuffInfoLevelVIPNum0"]).GetComponentsInChildren<UISprite>(true)[0];
        m_spNormalMainUIVIPBuffInfoLevelVIPNum1 = m_myTransform.Find(m_widgetToFullName["NormalMainUIVIPBuffInfoLevelVIPNum1"]).GetComponentsInChildren<UISprite>(true)[0];

        m_lblChargeRewardIconText = m_myTransform.Find(m_widgetToFullName["ChargeRewardIconText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_spChargeRewardIconBGUp = m_myTransform.Find(m_widgetToFullName["ChargeRewardIconBGUp"]).GetComponentsInChildren<UISprite>(true)[0];

        m_goNormalMainUIPlayerExpList = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlayerExpList"]).gameObject;
        m_goGONormalMainUIPlayerExpListFG = m_myTransform.Find(m_widgetToFullName["GONormalMainUIPlayerExpListFG"]).gameObject;
        m_lblNormalMainUIPlayerExpListNum = m_myTransform.Find(m_widgetToFullName["NormalMainUIPlayerExpListNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goNormalMainUIPlayerExpList.SetActive(true);
        AddPlayerExpFG();

        m_goArenaCDTipBtnOpen = m_myTransform.Find(m_widgetToFullName["ArenaCDTipBtnOpen"]).gameObject;
        m_goArenaCDTipCountDown = m_myTransform.Find(m_widgetToFullName["ArenaCDTipCountDown"]).gameObject;
        m_lblArenaCDTipCountDownName = m_myTransform.Find(m_widgetToFullName["ArenaCDTipCountDownName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaCDTipCountDownNum = m_myTransform.Find(m_widgetToFullName["ArenaCDTipCountDownNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_goDoorOfBuryOpenCDTipBtnOpen = m_myTransform.Find(m_widgetToFullName["DoorOfBuryOpenCDTipBtnOpen"]).gameObject;
        m_goDoorOfBuryOpenCDTipCountDown = m_myTransform.Find(m_widgetToFullName["DoorOfBuryOpenCDTipCountDown"]).gameObject;
        m_lblDoorOfBuryOpenCDTipCountDownName = m_myTransform.Find(m_widgetToFullName["DoorOfBuryOpenCDTipCountDownName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblDoorOfBuryOpenCDTipCountDownNum = m_myTransform.Find(m_widgetToFullName["DoorOfBuryOpenCDTipCountDownNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_lblUpgradePowerCurrentNum = m_myTransform.Find(m_widgetToFullName["UpgradePowerCurrentNum"]).GetComponentsInChildren<UILabel>(true)[0];        

        Initialize();

        //EventDispatcher.TriggerEvent("ShowMogoNormalMainUI");//������Ī������Ķ��� By MaiFeo

        m_lblTipText.text = Mogo.GameData.LanguageData.GetContent(180002);
        m_goNormalMainUIRankingBtnFxPos = FindTransform("NormalMainUIRankingBtnFxPos").gameObject;
        m_lblNormalMainUIVIPBuffBtnText = FindTransform("NormalMainUIVIPBuffBtnText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblNormalMainUIVIPBuffBtnLine = FindTransform("NormalMainUIVIPBuffBtnLine").GetComponentsInChildren<UILabel>(true)[0];

        //////////////////////////////////////////////////////////////////////////
        // ������ͼ��λ��
        //////////////////////////////////////////////////////////////////////////
        m_MainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        m_NoramlMainUIBTNListHCam = FindTransform("NoramlMainUIBTNListHCam").GetComponentsInChildren<Camera>(true)[0];
        m_NoramlMainUIBTNListVCam = FindTransform("NoramlMainUIBTNListVCam").GetComponentsInChildren<Camera>(true)[0];
        m_goNoramlMainUIBTNListH = FindTransform("NoramlMainUIBTNListH").gameObject;
        m_goNoramlMainUIBTNListV = FindTransform("NoramlMainUIBTNListV").gameObject;

        m_listNoramlMainUIBTNListHPos.Clear();
        for (int pos = 1; pos <= NoramlMainUIBTNListH_Num; pos++)
        {
            m_listNoramlMainUIBTNListHPos.Add(FindTransform(string.Format("NormalMainUIBTNListHPos{0}", pos)));
        }

        m_listNoramlMainUIBTNListVPos.Clear();
        for (int pos = 1; pos <= NoramlMainUIBTNListV_Num; pos++)
        {
            m_listNoramlMainUIBTNListVPos.Add(FindTransform(string.Format("NormalMainUIBTNListVPos{0}", pos)));
        }

        InitControllerIconAnim();
        InitNoramlMainUIBTNListHAnim();
        InitNoramlMainUIBTNListVAnim();
        InitCommunityIconAnim();
        InitHelpTipAnim();
    }

    public override void CallWhenLoadResources()
    {
        ID = MFUIManager.MFUIID.CityMainUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_instance = this;
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);

    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
    }

    void Start()
    {
        if (SystemConfig.Instance.IsDragMove)
        {
            MogoUIManager.Instance.ChangeSettingToControlStick();
        }
        else
        {
            MogoUIManager.Instance.ChangeSettingToTouch();
        }

        UIAnchor[] temp = m_myTransform.GetComponentsInChildren<UIAnchor>(true);

        for (int i = 0; i < temp.Length; ++i)
        {
            temp[i].enabled = false;
        }

        // EventDispatcher.TriggerEvent("ShowMogoNormalMainUI");       
    }

    #endregion       

    #region �¼�

    #region Action����

    public Action NORMALMAINUIPLAYERINFOUP;
    public Action ACTIVITYPLAYICONUP;
    public Action ASSISTANTPLAYICONUP;
    public Action AUTOTASKPLAYICONUP;
    public Action PVEPLAYICONUP;
    public Action PVPPLAYICONUP;
    public Action INSTANCEPLAYICONUP;
    public Action CHARGEREWARDICONUP;

    public Action DRAGONCONSUMEICONUP;
    public Action EQUIPMENTCONSUMEICONUP;
    public Action MALLCONSUMEICONUP;
    public Action DIAMONDTOGOLDICONUP;
    public Action SPRITEICONUP;
    public Action WINGICONUP;
    public Action PACKAGEICONUP;
    public Action ATTRIBUTEICONUP;

    public Action NORMALMAINUICOMMUNITYUP;

    public Action CONTRIBUTETIPOKUP;
    public Action ENERGYUP;
    public Action RANKINGUP;
    public Action UPGRADEPOWERUP;

    #endregion

    #region Initialize and Release

    void Initialize()
    {
        NormalMainUILogicManager.Instance.Initialize();
        m_uiLoginManager = NormalMainUILogicManager.Instance;

        EventDispatcher.RemoveEventListener(Events.NormalMainUIViewManagerEvent.PVEPLAYICONUP, PVEPLAYICONUP);
        EventDispatcher.RemoveEventListener(Events.NormalMainUIViewManagerEvent.PVPPLAYICONUP, PVPPLAYICONUP);

        NormalMainUIDict.ButtonTypeToEventUp.Add("NormalMainUIPlayerInfoBG", OnNormalMainUIPlayerInfoUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("ActivityPlayIcon", OnActivityPlayIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("AssistantPlayIcon", OnAssistantPlayIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("AutoTaskPlayIcon", OnAutoTaskPlayIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("PVEPlayIcon", OnPVEPlayIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("PVPPlayIcon", OnPVPPlayIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("InstancePlayIcon", OnInstancePlayIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("XDailyTaskIcon", DailyTaskSystemController.Singleton.OnOpenDailyTaskUI);
        EventDispatcher.AddEventListener("OnDailyTaskCloseButtonClicked", DailyTaskSystemController.Singleton.OnDailyTaskPanelCloseButtonPressed);
        NormalMainUIDict.ButtonTypeToEventUp.Add("ChargeRewardIcon", OnChargeRewardIconUp);

        // ����Button
        NormalMainUIDict.ButtonTypeToEventUp.Add("NoramlMainUIControllerIcon", OnControllerIconUp);

        NormalMainUIDict.ButtonTypeToEventUp.Add("MallConsumeIcon", OnMallConsumeIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("AttributeIcon", OnAttributeIconUp);

        NormalMainUIDict.ButtonTypeToEventUp.Add("DragonConsumeIcon", OnDragonConsumeIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("EquipmentConsumeIcon", OnEquipmentConsumeIconUp);        
        NormalMainUIDict.ButtonTypeToEventUp.Add("DiamondToGoldIcon", OnDiamondToGoldIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("SpriteIcon", OnSpriteIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("WingIcon", OnWingIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("WingIcon2", OnWingIconUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add(PackageIconName, OnPackageIconUp);

        NormalMainUIDict.ButtonTypeToEventUp.Add("NormalMainUICommunity", OnNormalMainUICommunityUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("BattleRecordOKBtn", OnBattleRecordOKBtnUp);

        NormalMainUIDict.ButtonTypeToEventUp.Add("DoorOfBuryOpenEnterBtn", OnDoorOfBuryOpenEnterBtnUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("DoorOfBuryOpenTipClose", OnDoorOfBuryOpenTipCloseUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("SanctuaryOpenTipEnterBtn", OnSanctuaryOpenTipEnterBtnUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("SanctuaryOpenTipClose", OnSanctuaryOpenTipCloseUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("MissionFoggyAbyssOpenTipEnterBtn", OnMissionFoggyAbyssOpenTipEnterBtnUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("MissionFoggyAbyssOpenTipClose", OnMissionFoggyAbyssOpenTipCloseUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("NormalMainUIPlayerVIP", OnVIPShow);
        NormalMainUIDict.ButtonTypeToEventUp.Add("AssistantDialogChangeBtn", OnAssistantDialogChangeBtnUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("NormalMainUIBuyStrenthBtn", OnEnergyBtnUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("NormalMainUIVIPBuffInfoBtnClose", OnVIPBuffInfoBtnUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("NormalMainUIVIPBuffBtn", OnVIPBuffBtnUp);
        NormalMainUIDict.ButtonTypeToEventPress.Add("NormalMainUIVIPBuffBtn", OnVIPBuffBtnPress);

        NormalMainUIDict.ButtonTypeToEventUp.Add("DoorOfBuryOpenCDTipBtnOpen", OnDoorOfBuryOpenCDTipBtnOpenUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("ArenaCDTipBtnOpen", OnArenaCDTipBtnOpenUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("NormalMainUIRankingBtn", OnRankingBtnOpenUp);
        NormalMainUIDict.ButtonTypeToEventUp.Add("UpgradePowerBtnOpen", OnUpgradePowerBtnOpenUp);

        m_goContributeTipOKBtn.transform.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnContributeTipOKUp;

        if (MogoWorld.thePlayer != null && MogoWorld.thePlayer.CurrentTask != null)
            NormalMainUIViewManager.Instance.SetAutoTaskIcon(IconData.dataMap.Get(MogoWorld.thePlayer.CurrentTask.autoIcon).path);
        
    }

    public void Release()
    {
        NormalMainUILogicManager.Instance.Release();

        NormalMainUIDict.ButtonTypeToEventUp.Clear();
        NormalMainUIDict.ButtonTypeToEventPress.Clear();

        m_goContributeTipOKBtn.transform.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnContributeTipOKUp;
    }

    #endregion

    #region �����水ť�¼�

    void OnNormalMainUIPlayerInfoUp()
    {
        if (NORMALMAINUIPLAYERINFOUP != null)
            NORMALMAINUIPLAYERINFOUP();
    }

    void OnActivityPlayIconUp()
    {
        if (ACTIVITYPLAYICONUP != null)
            ACTIVITYPLAYICONUP();
    }

    void OnAssistantPlayIconUp()
    {
        if (ASSISTANTPLAYICONUP != null)
            ASSISTANTPLAYICONUP();

        ////ShowAssistantDialog(!m_bAssistantDialogShown);

        ////m_bAssistantDialogShown = !m_bAssistantDialogShown;

    }

    void OnAutoTaskPlayIconUp()
    {
        if (AUTOTASKPLAYICONUP != null)
            AUTOTASKPLAYICONUP();
    }

    void OnPVEPlayIconUp()
    {
        if (PVEPLAYICONUP != null)
            PVEPLAYICONUP();
    }

    void OnPVPPlayIconUp()
    {
        if (PVPPLAYICONUP != null)
            PVPPLAYICONUP();
    }

    /// <summary>
    /// ���ư�ť
    /// </summary>
    void OnControllerIconUp()
    {
        ShowBottomRightButtons(!IsBottomRightButtonsShow);
    }

    void OnDragonConsumeIconUp()
    {
        if (DRAGONCONSUMEICONUP != null)
            DRAGONCONSUMEICONUP();

        //MogoUIManager.Instance.ShowMogoDragonUI();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.DragonUI);
    }

    void OnEquipmentConsumeIconUp()
    {
        if (EQUIPMENTCONSUMEICONUP != null)
            EQUIPMENTCONSUMEICONUP();
    }

    void OnMallConsumeIconUp()
    {
        if (MALLCONSUMEICONUP != null)
            MALLCONSUMEICONUP();
    }

    void OnAttributeIconUp()
    {
        if (ATTRIBUTEICONUP != null)
            ATTRIBUTEICONUP();
    }

    void OnDiamondToGoldIconUp()
    {
        if (DIAMONDTOGOLDICONUP != null)
            DIAMONDTOGOLDICONUP();
    }

    void OnSpriteIconUp()
    {
        if (SPRITEICONUP != null)
            SPRITEICONUP();
    }

    void OnWingIconUp()
    {
        if (WINGICONUP != null)
            WINGICONUP();
    }

    void OnPackageIconUp()
    {
        if (PACKAGEICONUP != null)
            PACKAGEICONUP();
    }

    void OnInstancePlayIconUp()
    {
        if (INSTANCEPLAYICONUP != null)
            INSTANCEPLAYICONUP();
    }

    void OnChargeRewardIconUp()
    {
        if (CHARGEREWARDICONUP != null)
            CHARGEREWARDICONUP();
    }

    void OnNormalMainUICommunityUp()
    {
        if (NORMALMAINUICOMMUNITYUP != null)
            NORMALMAINUICOMMUNITYUP();
        ShowCommunityButton(false);
    }

    #endregion    

    private void OnAssistantDialogChangeBtnUp()
    {
        OnAutoTaskPlayIconUp();
        ShowAssistantDialog(false);
    }

    private void OnVIPShow()
    {
        LoggerHelper.Debug("OnVipShow");
        MogoUIManager.Instance.ShowVIPInfoUI();
    }

    private void OnEnergyBtnUp()
    {
        LoggerHelper.Debug("OnEnergyBtnUp");
        if (ENERGYUP != null)
            ENERGYUP();
    }

    private void OnVIPBuffBtnUp()
    {
        LoggerHelper.Debug("OnVIPBuffBtnUp");
        ShowVIPBuffInfo(true);
    }

    private void OnVIPBuffBtnPress(bool bPress)
    {
        //ShowVIPBuffInfo(bPress);
    }

    private void OnVIPBuffInfoBtnUp()
    {
        ShowVIPBuffInfo(false);
    }

    private void OnBattleRecordOKBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("OnBattleRecordOKBtnUp");
    }

    /// <summary>
    /// ������CD�����������֮��
    /// </summary>
    private void OnDoorOfBuryOpenCDTipBtnOpenUp()
    {
        StopDoorOfBuryCDTip();
    }

    /// <summary>
    /// ������CD������򿪾�����
    /// </summary>
    private void OnArenaCDTipBtnOpenUp()
    {
        //StopArenaCDTip();
        OnPVPPlayIconUp();
    }

    /// <summary>
    /// �����а�
    /// </summary>
    void OnRankingBtnOpenUp()
    {
        if (RANKINGUP != null)
            RANKINGUP();
    }

    /// <summary>
    /// ��ս��������������
    /// </summary>
    void OnUpgradePowerBtnOpenUp()
    {
        if (UPGRADEPOWERUP != null)
            UPGRADEPOWERUP();
    }

    void OnContributeTipOKUp()
    {
        ShowContributeTip(false);

        if (CONTRIBUTETIPOKUP != null)
        {
            CONTRIBUTETIPOKUP();
        }
    }

    #endregion

    #region ������Ϣ

    private void SetUIText(string UIName, string text)
    {
        //var l = m_myTransform.FindChild(UIName).GetComponentInChildren<UILabel>() as UILabel;
        var l = m_myTransform.Find(UIName).GetComponentsInChildren<UILabel>(true);
        if (l != null)
        {
            l[0].text = text;
            l[0].transform.localScale = new Vector3(18, 18, 18);
        }

        //l.text = text;
        //l.m_myTransform.localScale = new Vector3(15, 15, 15);
    }

    private void SetUITexture(string UIName, string imageName)
    {
        var s = m_myTransform.Find(UIName).GetComponentsInChildren<UISlicedSprite>(true);
        if (s != null)
            s[0].spriteName = imageName;
    }
    /// <summary>
    /// �������ͷ��
    /// </summary>
    /// <param name="imageName"></param>
    public void SetPlayerHeadImage(string imageName)
    {
        SetUITexture(m_widgetToFullName["NormalMainUIPlayerHead"], imageName);
    }

    ///// <summary>
    ///// �����������
    ///// </summary>
    ///// <param name="playerName"></param>

    //public void SetPlayerName(string playerName)
    //{      
    //    SetUIText(m_widgetToFullName["PlayerNameText"], playerName);
    //}

    /// <summary>
    /// ������ҵȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetPlayerLevel(byte level)
    {
        SetUIText(m_widgetToFullName["NormalMainUIPlayerLevelText"], level.ToString());
        ShowChargeRewardIcon(level);
        SetPlayerEnergyText(MogoWorld.thePlayer.EnergyString);
    }

    /// <summary>
    /// ��������ʣ�����
    /// </summary>
    /// <param name="lastTimes"></param>
    public void SetGoldMetallurgyLastTimes(int lastTimes)
    {
        if (m_goGODiamondToGoldIconLast != null)
        {
            if (lastTimes > 0)
            {
                m_goGODiamondToGoldIconLast.SetActive(true);
            }
            else
            {
                m_goGODiamondToGoldIconLast.SetActive(false);
            }

            if (m_lblDiamondToGoldIconLastTimes != null)
                m_lblDiamondToGoldIconLastTimes.text = lastTimes.ToString();
        }
    }

    public void SetPlayerVIP(byte vipLevel)
    {
        if (vipLevel < 10)
        {
            m_spVipLevelNum1.spriteName = "vip_" + vipLevel;
            m_spVipLevelNum2.gameObject.SetActive(false);
        }
        else if (vipLevel >= 10)
        {
            m_spVipLevelNum1.spriteName = "vip_1";
            m_spVipLevelNum2.spriteName = "vip_" + (vipLevel - 10);
            m_spVipLevelNum2.gameObject.SetActive(true);
        }

        if (MogoWorld.thePlayer != null)
            SetGoldMetallurgyLastTimes(MogoWorld.thePlayer.CalGoldMetallurgyLastUseTimes());
    }

    /// <summary>
    /// �������Ѫ��
    /// </summary>
    /// <param name="blood">Ѫ���ٷֱ�</param>
    UIFilledSprite m_fsPlayerBlood;
    public void SetPlayerBlood(float blood)
    {
        //SetPlayerBloodText(MogoWorld.thePlayer.curHp + "/" + MogoWorld.thePlayer.hp);

        if (m_fsPlayerBlood != null)
            m_fsPlayerBlood.fillAmount = blood;
    }

    /// <summary>
    /// �������ŭ��ֵ
    /// </summary>
    UIFilledSprite m_fsPlayerAnger;
    public void SetPlayerAnger(float anger)
    {
        if (m_fsPlayerAnger != null)
        {
            m_fsPlayerAnger.fillAmount = (float)anger * 0.7f;
        }
    }

    #region ���ﾭ����

    readonly static int PlayerExpListCount = 10; // ���������
    readonly static int PlayerExpPadding = 5; // �����֮��ļ��
    readonly static int OffsetX = 60; // ���ھ���Iconƫ�Ƶ�Xֵ
    int PlayerExpLength = 0;

    void AddPlayerExpFG()
    {
        PlayerExpLength = (int)((1280.0f - OffsetX - (PlayerExpListCount + 1) * PlayerExpPadding) / PlayerExpListCount);
        m_listNormalMainUIPlayerExpFG.Clear();

        for (int i = 0; i < PlayerExpListCount; i++)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("PlayerExpGrid.prefab", (prefab, guid, go) =>
            {
                GameObject temp = (GameObject)go;
                temp.transform.parent = m_goGONormalMainUIPlayerExpListFG.transform;
                temp.transform.localPosition = new Vector3(index * (PlayerExpLength + PlayerExpPadding) - 640 + OffsetX + PlayerExpPadding, 0, 0);
                temp.transform.localScale = Vector3.one;
                temp.name = "NormalMainUIPlayerExpGrid" + index;
                UISprite sp = temp.transform.Find("PlayerExpFG").GetComponentsInChildren<UISprite>(true)[0];
                sp.transform.localScale = new Vector3(PlayerExpLength, sp.transform.localScale.y, sp.transform.localScale.z);
                sp.fillAmount = 0;
                m_listNormalMainUIPlayerExpFG.Add(sp);
            });
        }
    }

    /// <summary>
    /// ������Ҿ���(TopLeft)
    /// </summary>
    /// <param name="exp"></param>
    UIFilledSprite m_fsPlayerExp;
    public void SetPlayerExp(float exp)
    {
        if (m_fsPlayerExp != null)
            m_fsPlayerExp.fillAmount = exp * 0.7f;

        m_lblNormalMainUIPlayerExpListNum.text = (int)(exp * 100) + "%";
        SetPlayerExpFG();
    }

    /// <summary>
    /// ������Ҿ���(Bottom)
    /// </summary>
    void SetPlayerExpFG()
    {
        float iOneExp = (float)MogoWorld.thePlayer.nextLevelExp / (float)PlayerExpListCount;
        float currentExp = (float)MogoWorld.thePlayer.exp;

        int i = 0;
        for (; i < PlayerExpListCount; i++)
        {
            if (currentExp >= iOneExp * (i + 1))
            {
                m_listNormalMainUIPlayerExpFG[i].fillAmount = 1;
            }
            else
            {
                m_listNormalMainUIPlayerExpFG[i].fillAmount = (currentExp - iOneExp * i) / iOneExp;
                break;
            }
        }

        for (i = i + 1; i < PlayerExpListCount; i++)
        {
            m_listNormalMainUIPlayerExpFG[i].fillAmount = 0;
        }
    }

    #endregion

    public void SetPlayerEnergyText(string energyText)
    {
        if (MogoWorld.thePlayer != null && AvatarLevelData.dataMap.ContainsKey(MogoWorld.thePlayer.level))
        {
            m_energyString.text = string.Concat(LanguageData.GetContent(25569), MogoWorld.thePlayer.energy, "/", AvatarLevelData.dataMap[MogoWorld.thePlayer.level].maxEnergy);
            SetPlayerEnergyProgress();
        }
    }

    void SetPlayerEnergyProgress()
    {
        if (MogoWorld.thePlayer != null && AvatarLevelData.dataMap.ContainsKey(MogoWorld.thePlayer.level))
        {
            float progress = (float)MogoWorld.thePlayer.energy / AvatarLevelData.dataMap[MogoWorld.thePlayer.level].maxEnergy;
            if (progress > 1)
            {
                m_spNormalMainUIPlayerStrenthFG.spriteName = "zdjm_shengmingtiaozise";
                progress = 1;
            }
            else
            {
                m_spNormalMainUIPlayerStrenthFG.spriteName = "zdjm_shengmingtiaolvse";
            }
            if (m_spNormalMainUIPlayerStrenthFG != null)
                m_spNormalMainUIPlayerStrenthFG.fillAmount = progress;
        }
    }

    public void TruelyShowAssistantDialog(bool isShow)
    {
        if (m_goAssistantDialog != null)
        {
            m_goAssistantDialog.SetActive(isShow);
        }

        MogoUIQueue.Instance.Locked = false;
    }

    public void ShowAssistantDialog(bool isShow)
    {
        m_goAssistantDialog.SetActive(isShow);
        return; //By MaiFeo
        //Ϊ���������ע�����´���
        //if (m_goAssistantDialog != null)
        //{
        //    if (isShow)
        //    {
        //        MogoUIQueue.Instance.PushOne(() => { TruelyShowAssistantDialog(isShow); }, MogoUIManager.Instance.m_NormalMainUI, "ShowAssistantDialog");
        //    }
        //    else
        //    {
        //        m_goAssistantDialog.SetActive(isShow);
        //    }
        //}
    }

    public void ShowCommunityButton(bool isShow)
    {
        m_myTransform.Find(m_widgetToFullName["NormalMainUICommunity"]).gameObject.SetActive(isShow);
    }

    public void ShowSendHelp(bool isShow)
    {
        m_goSendHelp.SetActive(isShow);
    }

    public void SetControlStickEnable(bool isEnable)
    {
        m_myTransform.Find(m_widgetToFullName["Controller"]).GetComponentsInChildren<ControlStick>(true)[0].enabled = isEnable;
        m_myTransform.Find(m_widgetToFullName["Controller"]).GetComponentsInChildren<BoxCollider>(true)[0].enabled = isEnable;
        m_myTransform.Find(m_widgetToFullName["NormalMainUIBG"]).GetComponentsInChildren<TouchControll>(true)[0].enabled = !isEnable;
        m_myTransform.Find(m_widgetToFullName["NormalMainUIBG"]).GetComponentsInChildren<BoxCollider>(true)[0].enabled = !isEnable;
    }

    public void ShowSocialTip(bool isShow)
    {
        m_socialTip.SetActive(isShow);
    }

    public void SetAssistantDialogText(string text)
    {
        m_lblAssistantDialog.text = text;
    }

    public void ShowContributeTip(bool isShow)
    {
        m_goContributeTip.SetActive(isShow);
    }   

    #endregion

    #region ʥ������ս���׿�

    void SetContributeTipChineseData()
    {
        m_myTransform.Find(m_widgetToFullName["CurrentRewardText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(46902);
        m_myTransform.Find(m_widgetToFullName["NextRewardText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(46903);
        m_myTransform.Find(m_widgetToFullName["NextRewardConditionText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(46904);
    }

    public void SetContributeTipTitle(string text)
    {
        m_lblContributeTipTitle.text = text;
    }

    public void SetContributeTipTodayRank(string text)
    {
        m_lblContributeTipTodayRank.text = text;
    }

    public void SetContributeTipWeekRank(string text)
    {
        m_lblContributeTipWeekRank.text = text;
    }

    public void SetContributeTipCurrentItem(string imgName)
    {
        m_spCurrentRewardItem.spriteName = imgName;
    }

    public void SetContributeTipNextItem(string imgName)
    {
        m_spNextRewardItem.spriteName = imgName;
    }

    /// <summary>
    /// ���õ�ǰ��ҽ���
    /// </summary>
    /// <param name="num"></param>
    public void SetContributeTipCurrentRewardNum(int num)
    {
        m_lblContributeTipCurrentRewardNum.text = "x" + num;
    }

    /// <summary>
    /// �����¼��������蹱��
    /// </summary>
    /// <param name="num"></param>
    public void SetContributeTipNextRewardConditionNum(int num)
    {
        m_lblContributeTipNextRewardConditionNum.text = num + LanguageData.GetContent(46906);//"����";
    }

    /// <summary>
    /// �����¼���ҽ���
    /// </summary>
    /// <param name="num"></param>
    public void SetContributeNextRewardNum(int num)
    {
        m_lblContributeTipNextRewardNum.text = "x" + num;
    }

    #endregion

    #region ������ʾ[����֮��][ʥ������ս][���⸱��-������Ԩ]

    #region [����֮��]

    /// <summary>
    /// ��ʾ����֮�ſ�����ʾ(��UI����)
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowDoorOfBuryTip()
    {
        MogoUIQueue.Instance.PushOne(() =>
        {
            TruelyShowDoorOfBuryTip(true);
        }, MogoUIManager.Instance.m_NormalMainUI, "ShowDoorOfBuryTip");
    }

    /// <summary>
    /// �Ƿ���ʾ����֮�ſ�����ʾ
    /// </summary>
    /// <param name="isShow"></param>
    public void TruelyShowDoorOfBuryTip(bool isShow)
    {
        //Debug.LogError("ShowDoorOfBuryTip:"+isShow);
        UITexture tex = m_myTransform.Find(m_widgetToFullName["AssistantHead"]).GetComponentsInChildren<UITexture>(true)[0];

        if (isShow)
        {            
            MogoWorld.thePlayer.RpcCall("OblivionGateReq", 3, 0, 0);

            if (MogoUIManager.Instance != null)
            {
                //MogoUIManager.Instance.ShowMogoCommuntiyUI(CommunityUIParent.NormalMainUI, false);

                MogoUIManager.Instance.m_CommunityUI.SetActive(false);
                NormalMainUIViewManager.Instance.ShowCommunityButton(true);
            }
            m_goDoorOfBuryOpenTipWord.GetComponent<DoorOfBuryTipUIMgr>().ShowWord(isShow, () =>
            {
                if (MogoUIManager.Instance != null)
                {
                    //MogoUIManager.Instance.ShowMogoCommuntiyUI(CommunityUIParent.NormalMainUI, false);

                    MogoUIManager.Instance.m_CommunityUI.SetActive(false);
                    NormalMainUIViewManager.Instance.ShowCommunityButton(true);
                }
                m_goDoorOfBuryOpenTip.SetActive(isShow);

                if (tex.material.GetTexture("_AlphaTex") == null)
                {
                    AssetCacheMgr.GetUIResource("npc-jingling.png", (obj) =>
                    {
                        tex.mainTexture = (Texture)obj;
                    });

                    AssetCacheMgr.GetUIResource("npc-jingling_A.png", (objA) =>
                    {
                        tex.material.SetTexture("_AlphaTex", (Texture)(objA));
                        m_goDoorOfBuryOpenTip.SetActive(isShow);
                    });
                }
                else
                {
                    m_goDoorOfBuryOpenTip.SetActive(isShow);
                }
            });            
        }
        else
        {
            MogoUIQueue.Instance.Locked = false; // UI���н���
            MogoUIQueue.Instance.CheckQueue();

            if (SystemSwitch.DestroyResource)
            {
                //AssetCacheMgr.ReleaseResourceImmediate("npc-jingling.png");
                //AssetCacheMgr.ReleaseResourceImmediate("npc-jingling_A.png");
                tex.mainTexture = null;
                tex.material.SetTexture("_AlphaTex", null);
            }
            m_goDoorOfBuryOpenTip.SetActive(isShow);
        }
    }

    /// <summary>
    /// ��������֮��
    /// </summary>
    private void OnDoorOfBuryOpenEnterBtnUp()
    {
        TruelyShowDoorOfBuryTip(false);
        EventDispatcher.TriggerEvent(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.ENTERDOOROFBURYBYTIP);
    }

    /// <summary>
    /// �ر�����֮����ʾ
    /// </summary>
    private void OnDoorOfBuryOpenTipCloseUp()
    {
        TruelyShowDoorOfBuryTip(false);
    }

    #endregion

    #region [ʥ������ս]

    /// <summary>
    /// ��ʾʥ������ս������ʾ(��UI����)
    /// </summary>
    public void ShowSanctuaryOpenTip()
    {
        MogoUIQueue.Instance.PushOne(() => 
        {
            TruelyShowSanctuaryOpenTip(true);
        }, MogoUIManager.Instance.m_NormalMainUI, "ShowSanctuaryOpenTip");
    }

    /// <summary>
    /// �Ƿ���ʾʥ������ս������ʾ
    /// </summary>
    /// <param name="isShow"></param>
    public void TruelyShowSanctuaryOpenTip(bool isShow)
    {
        UITexture tex = m_myTransform.Find(m_widgetToFullName["SanctuaryOpenTipAvatar"]).GetComponentsInChildren<UITexture>(true)[0];
        if (isShow)
        { 

            if (MogoUIManager.Instance != null)
            {
                MogoUIManager.Instance.m_CommunityUI.SetActive(false);
                NormalMainUIViewManager.Instance.ShowCommunityButton(true);
            }

            if (tex.material.GetTexture("_AlphaTex") == null)
            {
                AssetCacheMgr.GetUIResource("shi_jie_boss.png", (obj) =>
                {
                    tex.mainTexture = (Texture)obj;

                    AssetCacheMgr.GetUIResource("shi_jie_boss_A.png", (objA) =>
                        {
                            tex.material.SetTexture("_AlphaTex", (Texture)objA);
                            m_goSanctuaryOpenTip.SetActive(isShow);
                        });
                });
            }
            else
            {
                m_goSanctuaryOpenTip.SetActive(isShow);
            }            
        }
        else
        {
            MogoUIQueue.Instance.Locked = false; // UI���н���
            MogoUIQueue.Instance.CheckQueue();

            if (SystemSwitch.DestroyResource)
            {
                AssetCacheMgr.ReleaseResourceImmediate("shi_jie_boss.png");
                AssetCacheMgr.ReleaseResourceImmediate("shi_jie_boss_A.png");
                tex.mainTexture = null;
                tex.material.SetTexture("_AlphaTex", null);
            }
            m_goSanctuaryOpenTip.SetActive(isShow);
        }
    }

    /// <summary>
    /// ����ʥ������ս
    /// </summary>
    private void OnSanctuaryOpenTipEnterBtnUp()
    {
        TruelyShowSanctuaryOpenTip(false);
        EventDispatcher.TriggerEvent(Events.SanctuaryEvent.EnterSanctuary);
    }

    /// <summary>
    /// �ر�ʥ������ս������ʾ
    /// </summary>
    private void OnSanctuaryOpenTipCloseUp()
    {
        TruelyShowSanctuaryOpenTip(false);
    }

    #endregion

    #region [���⸱��-������Ԩ]

    /// <summary>
    /// ��ʾ[���⸱��-������Ԩ]������ʾ(��UI����)
    /// </summary>
    public void ShowMissionFoggyAbyssOpenTip()
    {
        MogoUIQueue.Instance.PushOne(() =>
        {
            TruelyShowMissionFoggyAbyssOpenTip(true);
        }, MogoUIManager.Instance.m_NormalMainUI, "ShowMissionFoggyAbyssOpenTip");
    }

    /// <summary>
    /// �Ƿ���ʾ[���⸱��-������Ԩ]������ʾ
    /// </summary>
    /// <param name="isShow"></param>
    public void TruelyShowMissionFoggyAbyssOpenTip(bool isShow)
    {
        UITexture tex = FindTransform("MissionFoggyAbyssOpenTipAvatar").GetComponentsInChildren<UITexture>(true)[0];
        if (isShow)
        {
            if (MogoUIManager.Instance != null)
            {
                MogoUIManager.Instance.m_CommunityUI.SetActive(false);
                NormalMainUIViewManager.Instance.ShowCommunityButton(true);
            }

            if (tex.material.GetTexture("_AlphaTex") == null)
            {
                AssetCacheMgr.GetUIResource("npc-jingling.png", (obj) =>
                {
                    tex.mainTexture = (Texture)obj;

                    AssetCacheMgr.GetUIResource("npc-jingling.png", (objA) =>
                    {
                        tex.material.SetTexture("_AlphaTex", (Texture)objA);
                        m_goMissionFoggyAbyssOpenTip.SetActive(isShow);
                    });
                });
            }
            else
            {
                m_goMissionFoggyAbyssOpenTip.SetActive(isShow);
            }
        }
        else
        {
            MogoUIQueue.Instance.Locked = false; // UI���н���
            MogoUIQueue.Instance.CheckQueue();

            if (SystemSwitch.DestroyResource)
            {
                //AssetCacheMgr.ReleaseResourceImmediate("npc-jingling.png");
                //AssetCacheMgr.ReleaseResourceImmediate("npc-jingling_A.png");
                tex.mainTexture = null;
                tex.material.SetTexture("_AlphaTex", null);
            }
            m_goMissionFoggyAbyssOpenTip.SetActive(isShow);
        }
    }

    /// <summary>
    /// ����[���⸱��-������Ԩ]
    /// </summary>
    private void OnMissionFoggyAbyssOpenTipEnterBtnUp()
    {
        TruelyShowMissionFoggyAbyssOpenTip(false);

        MogoWorld.thePlayer.Move();
        MogoWorld.thePlayer.motor.SetStopDistance(0);
        MogoWorld.thePlayer.motor.MoveTo(FoggyAbyssDoor.FoggyAbyssDoorPosition);
    }
     
    /// <summary>
    /// �ر�[���⸱��-������Ԩ]������ʾ
    /// </summary>
    private void OnMissionFoggyAbyssOpenTipCloseUp()
    {
        TruelyShowMissionFoggyAbyssOpenTip(false);
    }

    #endregion

    #endregion

    #region ����

    public void EmptyCommunityLabel()
    {
        for (int i = 0; i < m_listFaceIcon.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listFaceIcon[i]);
        }

        m_listFaceIcon.Clear();
    }

    public void SetNewChatLabel(string text)
    {
        EmptyCommunityLabel();

        string result = text;
        //MogoGlobleUIManager.Instance.Info(text);

        //for (int i = 0; i < text.Length; ++i)
        //{
        //    if (m_goCommunityLabel.font.CalculatePrintedSize(result.Substring(0, i), true,
        //                                UIFont.SymbolStyle.None).x * m_goCommunityLabel.font.size > 275)
        //    {
        //        m_lblNewChatLabel.text = result.Substring(0, i-1);
        //        MogoGlobleUIManager.Instance.Info(result.Substring(0, i-1));
        //        return;
        //    }
        //}
        int resultOffset = 0;
        for (int i = 0; i < text.Length; ++i, ++resultOffset)
        {
            if (text[i] == '<')
            {
                if (i + 7 <= text.Length - 1)
                {
                    if (text.Substring(i, 7) == "<face=(")
                    {
                        //if (text[i + 9] == '>')
                        //{
                        //    result = result.ReplaceFirst(text.Substring(i, 10), "��");
                        //    i += 9; //ѭ�����ټ�1 ��ȫ�滻<face=()>
                        //}
                        for (int j = i + 7; j <= text.Length - 1; ++j)
                        {
                            if (text[j] == '>')
                            {
                                result = result.ReplaceFirst(text.Substring(i, j + 1 - i), "��");

                                int faceIconId = 0;

                                if (text[j - 3] == '(')
                                {
                                    faceIconId = int.Parse(text.Substring(j - 2, 1));
                                }
                                else if (text[j - 4] == '(')
                                {
                                    faceIconId = int.Parse(text.Substring(j - 3, 2));
                                }


                                if (m_goCommunityLabel.font.CalculatePrintedSize(result.Substring(0, resultOffset), true,
                                    UIFont.SymbolStyle.None).x * m_goCommunityLabel.font.size >= 275)
                                {
                                    m_lblNewChatLabel.text = result.Substring(0, resultOffset);
                                    return;
                                }


                                int tempI = resultOffset;
                                AssetCacheMgr.GetUIInstance("FaceIconInNormalMainUI.prefab", (prefab, guid, go) =>
                                {
                                    GameObject obj = (GameObject)go;
                                    obj.transform.parent = m_goCommunity.transform;

                                    obj.transform.localPosition = m_goCommunityLabel.transform.localPosition
                                        + new Vector3(m_goCommunityLabel.font.CalculatePrintedSize(
                                        result.Substring(0, tempI), true, UIFont.SymbolStyle.None).x * m_goCommunityLabel.font.size, 0, 0);

                                    //obj.transform.localPosition = m_goCommunityLabel.transform.localPosition;
                                    obj.transform.localScale = Vector3.one;
                                    obj.name = "FaceIcon";

                                    obj.GetComponentsInChildren<UISprite>(true)[0].spriteName =
                                        UIFaceIconData.dataMap[faceIconId + 1].facefirst;

                                    m_listFaceIcon.Add(obj);

                                });
                                i += (j - i);

                                break;
                            }
                        }
                    }
                    else if (text.Substring(i, 7) == "<info=(")
                    {
                        string itemName = "";
                        for (int j = i + 7; j <= text.Length - 1; ++j)
                        {
                            if (text[j] == ',')
                            {
                                string itemId = text.Substring(i + 7, j - (i + 7));
                                itemName = ItemParentData.GetItem(int.Parse(itemId)).Name;

                                for (int m = j; m <= text.Length - 1; ++m)
                                {
                                    if (text[m] == '>')
                                    {
                                        result = result.ReplaceFirst(text.Substring(i, m + 1 - i), itemName);
                                        resultOffset += itemName.Length - 1; //ѭ���ټ�1
                                        i += (m - i);
                                        break;
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        m_lblNewChatLabel.text = result;
    }

    /// <summary>
    /// �������찴ť
    /// </summary>
    /// <param name="isReceive"></param>
    private bool IsReceive = false;
    public void SetCommunityIconBG(bool isReceive)
    {
        IsReceive = isReceive;

        if (isReceive)
        {
            if(ChangeCommunityIconBGTimerID != 0)
            {
                Mogo.Util.TimerHeap.DelTimer(ChangeCommunityIconBGTimerID);
                ChangeCommunityIconBGTimerID = 0;
            }
            
            ChangeCommunityIconBG(true);
        }
        else
        {
            m_spCommunityIconBGDown.enabled = false;
        }
    }

    /// <summary>
    /// ���찴ť�����л�
    /// </summary>
    /// <param name="isShowUp"></param>
    uint ChangeCommunityIconBGTimerID = 0;
    private void ChangeCommunityIconBG(bool isShowUp)
    {
        if (IsReceive)
        {
            m_spCommunityIconBGDown.enabled = !isShowUp;
            ChangeCommunityIconBGTimerID = Mogo.Util.TimerHeap.AddTimer<bool>(200, 0, ChangeCommunityIconBG, !isShowUp);
        }
    }    

    #endregion

    #region �м���Ϣ

    public bool IsDebug = false;
    private UILabel m_lblTipText;
    private GameObject m_goDoorOfBuryOpenTipWord;

    #endregion

    #region ������ͼ��Ư�ƺͰ�ť����

    #region ���ݹ���

    public readonly static string PackageIconName = "NMUIPackageIcon";
    public readonly static string EquipmentConsumeIconName = "EquipmentConsumeIcon";
    public readonly static string DragonConsumeIconName = "DragonConsumeIcon";
    public readonly static string DiamondToGoldIconName = "DiamondToGoldIcon";
    public readonly static string SpriteIconName = "SpriteIcon";
    public readonly static string WingIconName = "WingIcon";    

    public enum IconPlaysType
    {
        IconPlaysBottomRightH = 1, // ���½Ǻ���Icon�б�
        IconPlaysBottomRightV = 2, // ���½�����Icon�б�
        IconPlaysTopRightH = 3, // ���ϽǺ���Icon�б�
    }

    public class IconPlays
    {
        public const int ControllerIcon = 0; // ���ư�ť
        public const int NMCommunityIcon = 1; // ���찴ť
        public const int NMHelpTip = 2; // ������ʾ
    }

    public class IconPlaysBottomRightH
    {
        public const int PackageIcon = 0; // ����
        public const int EquipmentIcon = 1; // ����
        public const int DragonIcon = 2; // ����
        public const int DiamondToGoldIcon = 3; // ����
        public const int SpriteIcon = 4; // ����ϵͳ
        public const int WingIcon = 5; // ���

        public const int MaxCount = 6;
    }

    public class IconPlaysBottomRightV
    {
        public const int AttributeIcon = 0; // �ɾ�
        public const int MallConsumeIcon = 1; // �̳�

        public const int MaxCount = 2;
    }

    public class IconPlaysTopRightH
    {
        public const int AutoTaskPlayIcon = 0; // �Զ�����
        public const int DailyTaskIcon = 1; // ÿ������
        public const int ChallengeIcon = 2; // �(ԭ������ս)
        public const int ArenaIcon = 3; // ������
        public const int ChargeRewardIcon = 4; // ����

        public const int MaxCount = 5;
    }   

    // ����Icon
    private Dictionary<int, GameObject> m_maplistGoIconPlays = new Dictionary<int, GameObject>();

    // ���½Ǻ���Icon
    private Dictionary<int, GameObject> m_maplistIconPlaysBottomRightH = new Dictionary<int, GameObject>();
    private List<GameObject> m_listIconPlaysBottomRightHPos = new List<GameObject>();
    private GameObject m_goNMWingIcon2;

    // ���½�����Icon
    private Dictionary<int, GameObject> m_maplistIconPlaysBottomRightV = new Dictionary<int, GameObject>();
    private List<GameObject> m_listIconPlaysBottomRightVPos = new List<GameObject>();

    // ���ϽǺ���Icon
    private Dictionary<int, GameObject> m_maplistIconPlaysTopRightH = new Dictionary<int, GameObject>();
    private List<GameObject> m_listIconPlaysTopRightHPos = new List<GameObject>();    

    #endregion

    #region ͼ��Ư���߼�

    /// <summary>
    /// IconƯ���߼�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <param name="tempFXCamera"></param>
    private void LogicAddIconPlays(IconPlaysType theIconPlaysType, int indexIconPlays, Camera tempFXCamera)
    {
        switch (theIconPlaysType)
        {
            case IconPlaysType.IconPlaysBottomRightH:
                LogicAddIconPlaysBottomRightH(indexIconPlays, tempFXCamera);
                break;
            case IconPlaysType.IconPlaysBottomRightV:
                LogicAddIconPlaysBottomRightV(indexIconPlays, tempFXCamera);
                break;
            case IconPlaysType.IconPlaysTopRightH:
                LogicAddIconPlaysTopRightH(indexIconPlays, tempFXCamera);
                break;
            default:
                break;
        }
    }

    #region ���½Ǻ���IconƯ���߼�

    /// <summary>
    /// ���½Ǻ���Icon֮ǰ�Ŀ����б�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <returns></returns>
    private List<int> GetIconPlaysBottomRightHFront(int indexIconPlays)
    {
        List<int> listFront = new List<int>();
        for (int index = 0; index < IconPlaysBottomRightH.MaxCount && index < indexIconPlays; index++)
        {
            if (m_maplistIconPlaysBottomRightH[index].activeSelf)
            {
                listFront.Add(index);
            }
        }

        return listFront;
    }

    /// <summary>
    /// ���½Ǻ���Icon֮��Ŀ����б�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <returns></returns>
    private List<int> GetIconPlaysBottomRightHBehind(int indexIconPlays)
    {
        List<int> listBehind = new List<int>();
        for (int index = indexIconPlays + 1; index < IconPlaysBottomRightH.MaxCount; index++)
        {
            if (m_maplistIconPlaysBottomRightH[index].activeSelf)
            {
                listBehind.Add(index);
            }
        }

        return listBehind;
    }

    /// <summary>
    /// ���½Ǻ���IconƯ���߼�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <param name="tempFXCamera"></param>
    private void LogicAddIconPlaysBottomRightH(int indexIconPlays, Camera tempFXCamera)
    {
        OpenIconPlaysBottomRight(); // չ��Icon
        m_maplistIconPlaysBottomRightH[indexIconPlays].SetActive(true);
        m_maplistIconPlaysBottomRightH[indexIconPlays].transform.localPosition = m_goIconPlaysNMOriginPos.transform.localPosition;

        List<int> listFront = GetIconPlaysBottomRightHFront(indexIconPlays);
        List<int> listBehind = GetIconPlaysBottomRightHBehind(indexIconPlays);

        TweenPosition tp0 = m_maplistIconPlaysBottomRightH[indexIconPlays].GetComponentsInChildren<TweenPosition>(true)[0];
        //tp0.Reset();
        tp0.callWhenFinished = "";
        tp0.duration = PlaysIconOpenTime;
        tp0.from = m_goIconPlaysNMOriginPos.transform.localPosition;
        tp0.to = m_listIconPlaysBottomRightHPos[listFront.Count].transform.localPosition;
        tp0.eventReceiver = gameObject;
        tp0.callWhenFinished = "OnPlaysIconTPEnd";

        // �����Ѿ������ͼ�������һ��
        for (int index = 0; index < listBehind.Count; index++)
        {
            TweenPosition tpBehind = m_maplistIconPlaysBottomRightH[listBehind[index]].GetComponentsInChildren<TweenPosition>(true)[0];
            tpBehind.callWhenFinished = "";
            tpBehind.duration = PlaysIconOpenTime;
            tpBehind.from = m_listIconPlaysBottomRightHPos[listFront.Count + index].transform.localPosition;
            tpBehind.to = m_listIconPlaysBottomRightHPos[listFront.Count + index + 1].transform.localPosition;
            tpBehind.SetFactor(true);
            tpBehind.Play(true);
        }

        //tp0.enabled = true;
        tp0.SetFactor(true);  // ���ص�����󲥷�
        tp0.Play(true);

        MogoUIQueue.Instance.IsLocking = true;
        TimerHeap.AddTimer(5000, 0, () => { MogoUIManager.Instance.UICanChange = true; });
        MogoUIManager.Instance.UICanChange = false;
        MogoFXManager.Instance.AttachUIFX(2, tempFXCamera, 0, 0, 0, m_maplistIconPlaysBottomRightH[indexIconPlays]);
        TimerHeap.AddTimer(5000, 0, () => { MogoFXManager.Instance.DetachUIFX(2); });
    }

    /// <summary>
    /// ��������⴦��
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <param name="tempFXCamera"></param>
    private void LogicAddIconPlaysWingIcon()
    {
        int indexIconPlays = IconPlaysBottomRightH.WingIcon;
        Camera tempFXCamera = null;
        OpenIconPlaysBottomRight(); // չ��Icon

        List<int> listFront = GetIconPlaysBottomRightHFront(indexIconPlays);
        List<int> listBehind = GetIconPlaysBottomRightHBehind(indexIconPlays);

        GameObject goWingIcon = null;
        if (listFront.Count >= 2)
        {           
            goWingIcon = m_maplistIconPlaysBottomRightH[indexIconPlays];
            tempFXCamera = m_NoramlMainUIBTNListHCam;
        }
        else
        { 
            goWingIcon = m_goNMWingIcon2;
            tempFXCamera = m_MainCamera;
        }

        goWingIcon.SetActive(true);
        goWingIcon.transform.localPosition = m_goIconPlaysNMOriginPos.transform.localPosition;

        TweenPosition tp0 = goWingIcon.GetComponentsInChildren<TweenPosition>(true)[0];
        //tp0.Reset();
        tp0.callWhenFinished = "";
        tp0.duration = PlaysIconOpenTime;
        tp0.from = m_goIconPlaysNMOriginPos.transform.localPosition;
        tp0.to = m_listIconPlaysBottomRightHPos[listFront.Count].transform.localPosition;
        tp0.eventReceiver = gameObject;
        tp0.callWhenFinished = "OnPlaysIconTPEnd";

        // �����Ѿ������ͼ�������һ��
        for (int index = 0; index < listBehind.Count; index++)
        {
            TweenPosition tpBehind = m_maplistIconPlaysBottomRightH[listBehind[index]].GetComponentsInChildren<TweenPosition>(true)[0];
            tpBehind.callWhenFinished = "";
            tpBehind.duration = PlaysIconOpenTime;
            tpBehind.from = m_listIconPlaysBottomRightHPos[listFront.Count + index].transform.localPosition;
            tpBehind.to = m_listIconPlaysBottomRightHPos[listFront.Count + index + 1].transform.localPosition;
            tpBehind.SetFactor(true);
            tpBehind.Play(true);
        }

        //tp0.enabled = true;
        tp0.SetFactor(true);  // ���ص�����󲥷�
        tp0.Play(true);

        MogoUIQueue.Instance.IsLocking = true;
        TimerHeap.AddTimer(5000, 0, () => { MogoUIManager.Instance.UICanChange = true; });
        MogoUIManager.Instance.UICanChange = false;
        MogoFXManager.Instance.AttachUIFX(2, tempFXCamera, 0, 0, 0, goWingIcon);
        TimerHeap.AddTimer(5000, 0, () => { MogoFXManager.Instance.DetachUIFX(2); });
    }

    /// <summary>
    /// �����������⴦��(����ԭ��)
    /// </summary>
    private void LogicAddIconPlaysEquipmentIcon(int indexIconPlays, Camera tempFXCamera)
    {
        OpenIconPlaysBottomRight(); // չ��Icon
        m_maplistIconPlaysBottomRightH[indexIconPlays].SetActive(true);
        m_maplistIconPlaysBottomRightH[indexIconPlays].transform.localPosition = m_goIconPlaysNMOriginPos.transform.localPosition;
   
        TweenPosition tp0 = m_maplistIconPlaysBottomRightH[indexIconPlays].GetComponentsInChildren<TweenPosition>(true)[0];
        //tp0.Reset();
        tp0.callWhenFinished = "";
        tp0.duration = PlaysIconOpenTime;
        tp0.from = m_goIconPlaysNMOriginPos.transform.localPosition;
        tp0.to = m_listIconPlaysBottomRightHPos[IconPlaysBottomRightH.EquipmentIcon].transform.localPosition;
        tp0.eventReceiver = gameObject;
        tp0.callWhenFinished = "OnPlaysIconTPEnd";

        // �����Ѿ������ͼ�������һ��
        if (IsWingIconHasOpened && m_goNMWingIcon2.activeSelf)
        {
            TweenPosition tpBehind = m_goNMWingIcon2.GetComponentsInChildren<TweenPosition>(true)[0];
            tpBehind.callWhenFinished = "";
            tpBehind.duration = PlaysIconOpenTime;
            tpBehind.from = m_listIconPlaysBottomRightHPos[IconPlaysBottomRightH.EquipmentIcon].transform.localPosition;
            tpBehind.to = m_listIconPlaysBottomRightHPos[IconPlaysBottomRightH.EquipmentIcon + 1].transform.localPosition;
            tpBehind.SetFactor(true);
            tpBehind.Play(true);
        }      

        //tp0.enabled = true;
        tp0.SetFactor(true);  // ���ص�����󲥷�
        tp0.Play(true);

        MogoUIQueue.Instance.IsLocking = true;
        TimerHeap.AddTimer(5000, 0, () => { MogoUIManager.Instance.UICanChange = true; });
        MogoUIManager.Instance.UICanChange = false;
        MogoFXManager.Instance.AttachUIFX(2, tempFXCamera, 0, 0, 0, m_maplistIconPlaysBottomRightH[indexIconPlays]);
        TimerHeap.AddTimer(5000, 0, () => { MogoFXManager.Instance.DetachUIFX(2); });
    }

    #endregion

    #region ���½�����IconƯ���߼�

    /// <summary>
    /// ���½�����Icon֮ǰ�Ŀ����б�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <returns></returns>
    private List<int> GetIconPlaysBottomRightVFront(int indexIconPlays)
    {
        List<int> listFront = new List<int>();
        for (int index = 0; index < IconPlaysBottomRightV.MaxCount && index < indexIconPlays; index++)
        {
            if (m_maplistIconPlaysBottomRightV[index].activeSelf)
            {
                listFront.Add(index);
            }
        }

        return listFront;
    }

    /// <summary>
    /// ���½�����Icon֮��Ŀ����б�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <returns></returns>
    private List<int> GetIconPlaysBottomRightVBehind(int indexIconPlays)
    {
        List<int> listBehind = new List<int>();
        for (int index = indexIconPlays + 1; index < IconPlaysBottomRightV.MaxCount; index++)
        {
            if (m_maplistIconPlaysBottomRightV[index].activeSelf)
            {
                listBehind.Add(index);
            }
        }

        return listBehind;
    }

    /// <summary>
    /// ���½Ǻ���IconƯ���߼�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <param name="tempFXCamera"></param>
    private void LogicAddIconPlaysBottomRightV(int indexIconPlays, Camera tempFXCamera)
    {
        OpenIconPlaysBottomRight(); // չ��Icon
        m_maplistIconPlaysBottomRightV[indexIconPlays].SetActive(true);
        m_maplistIconPlaysBottomRightV[indexIconPlays].transform.localPosition = m_goIconPlaysNMOriginPos.transform.localPosition;

        List<int> listFront = GetIconPlaysBottomRightVFront(indexIconPlays);
        List<int> listBehind = GetIconPlaysBottomRightVBehind(indexIconPlays);

        TweenPosition tp0 = m_maplistIconPlaysBottomRightV[indexIconPlays].GetComponentsInChildren<TweenPosition>(true)[0];
        //tp0.Reset();
        tp0.callWhenFinished = "";
        tp0.duration = PlaysIconOpenTime;
        tp0.from = m_goIconPlaysNMOriginPos.transform.localPosition;
        tp0.to = m_listIconPlaysBottomRightVPos[listFront.Count].transform.localPosition;
        tp0.eventReceiver = gameObject;
        tp0.callWhenFinished = "OnPlaysIconTPEnd";

        // �����Ѿ������ͼ�������һ��
        for (int index = 0; index < listBehind.Count; index++)
        {
            TweenPosition tpBehind = m_maplistIconPlaysBottomRightV[listBehind[index]].GetComponentsInChildren<TweenPosition>(true)[0];
            tpBehind.callWhenFinished = "";
            tpBehind.duration = PlaysIconOpenTime;
            tpBehind.from = m_listIconPlaysBottomRightVPos[listFront.Count + index].transform.localPosition;
            tpBehind.to = m_listIconPlaysBottomRightVPos[listFront.Count + index + 1].transform.localPosition;
            tpBehind.SetFactor(true);
            tpBehind.Play(true);
        }

        //tp0.enabled = true;
        tp0.SetFactor(true);  // ���ص�����󲥷�
        tp0.Play(true);

        MogoUIQueue.Instance.IsLocking = true;
        TimerHeap.AddTimer(5000, 0, () => { MogoUIManager.Instance.UICanChange = true; });
        MogoUIManager.Instance.UICanChange = false;
        MogoFXManager.Instance.AttachUIFX(2, tempFXCamera, 0, 0, 0, m_maplistIconPlaysBottomRightV[indexIconPlays]);
        TimerHeap.AddTimer(5000, 0, () => { MogoFXManager.Instance.DetachUIFX(2); });
    }

    #endregion

    #region ���ϽǺ���IconƯ���߼�

    /// <summary>
    /// ���ϽǺ���Icon֮ǰ�Ŀ����б�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <returns></returns>
    private List<int> GetIconPlaysTopRightHFront(int indexIconPlays)
    {
        List<int> listFront = new List<int>();
        for (int index = 0; index < IconPlaysTopRightH.MaxCount && index < indexIconPlays; index++)
        {
            if (m_maplistIconPlaysTopRightH[index].activeSelf)
            {
                listFront.Add(index);
            }
        }

        return listFront;
    }

    /// <summary>
    /// ���ϽǺ���Icon֮��Ŀ����б�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <returns></returns>
    private List<int> GetIconPlaysTopRightHBehind(int indexIconPlays)
    {
        List<int> listBehind = new List<int>();
        for (int index = indexIconPlays + 1; index < IconPlaysTopRightH.MaxCount; index++)
        {
            if (m_maplistIconPlaysTopRightH[index].activeSelf)
            {
                listBehind.Add(index);
            }
        }

        return listBehind;
    }

    /// <summary>
    /// ���ϽǺ���IconƯ���߼�
    /// </summary>
    /// <param name="indexIconPlays"></param>
    /// <param name="tempFXCamera"></param>
    private void LogicAddIconPlaysTopRightH(int indexIconPlays, Camera tempFXCamera)
    {
        m_maplistIconPlaysTopRightH[indexIconPlays].SetActive(true);
        m_maplistIconPlaysTopRightH[indexIconPlays].transform.localPosition = m_goIconPlaysOriginPos.transform.localPosition;

        List<int> listFront = GetIconPlaysTopRightHFront(indexIconPlays);
        List<int> listBehind = GetIconPlaysTopRightHBehind(indexIconPlays);

        TweenPosition tp0 = m_maplistIconPlaysTopRightH[indexIconPlays].GetComponentsInChildren<TweenPosition>(true)[0];
        //tp0.Reset();
        tp0.callWhenFinished = "";
        tp0.duration = PlaysIconOpenTime;
        tp0.from = m_goIconPlaysOriginPos.transform.localPosition;
        tp0.to = m_listIconPlaysTopRightHPos[listFront.Count].transform.localPosition;
        tp0.eventReceiver = gameObject;
        tp0.callWhenFinished = "OnPlaysIconTPEnd";

        // �����Ѿ������ͼ�������һ��
        for (int index = 0; index < listBehind.Count; index++)
        {
            TweenPosition tpBehind = m_maplistIconPlaysTopRightH[listBehind[index]].GetComponentsInChildren<TweenPosition>(true)[0];
            tpBehind.callWhenFinished = "";
            tpBehind.duration = PlaysIconOpenTime;
            tpBehind.from = m_listIconPlaysTopRightHPos[listFront.Count + index].transform.localPosition;
            tpBehind.to = m_listIconPlaysTopRightHPos[listFront.Count + index + 1].transform.localPosition;
            tpBehind.SetFactor(true);
            tpBehind.Play(true);
        }

        //tp0.enabled = true;
        tp0.SetFactor(true);  // ���ص�����󲥷�
        tp0.Play(true);

        MogoUIQueue.Instance.IsLocking = true;
        TimerHeap.AddTimer(5000, 0, () => { MogoUIManager.Instance.UICanChange = true; });
        MogoUIManager.Instance.UICanChange = false;
        MogoFXManager.Instance.AttachUIFX(2, tempFXCamera, 0, 0, 0, m_maplistIconPlaysTopRightH[indexIconPlays]);
        TimerHeap.AddTimer(5000, 0, () => { MogoFXManager.Instance.DetachUIFX(2); });
    }

    #endregion

    #endregion

    #region ͼ��Ư��

    private GameObject m_goIconPlaysOriginPos;
    private GameObject m_goIconPlaysNMOriginPos;
    private GameObject m_goIconPlaysNMHOriginPos;
    private GameObject m_goIconPlaysNMVOriginPos;    

    #region �¿���Icon

    private readonly static float PlaysIconOpenTime = 2.0f; // Icon�������ŵ�ʱ��

    #region �����ݵȼ�����

    // �Ƿ��Ѿ�������Icon(�����ݵȼ�,���⴦��)
    private bool m_IsWingIconHasOpened = false;
    public bool IsWingIconHasOpened
    {
        get { return m_IsWingIconHasOpened; }
        set
        {
            m_IsWingIconHasOpened = value;
        }
    }  

    public void AddWingPlaysIcon()
    {
        MogoUIQueue.Instance.PushOne(() => { TruelyAddWingPlaysIcon(); }, MogoUIManager.Instance.m_NormalMainUI, "AddWingPlaysIcon", 20);
    }

    public void TruelyAddWingPlaysIcon()
    {
        LogicAddIconPlaysWingIcon();
        //LogicAddIconPlays(IconPlaysType.IconPlaysBottomRightH, IconPlaysBottomRightH.WingIcon, m_NoramlMainUIBTNListHCam);        
    }   

    #endregion

    #region ���ݵȼ�����

    public void AddNormalMainUIPlaysIcon(int level)
    {
        //Mogo.Util.LoggerHelper.Debug("Damn Herereereerereererereere@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        MogoUIQueue.Instance.PushOne(() => { TruelyAddNormalMainUIPlaysIcon(level); }, MogoUIManager.Instance.m_NormalMainUI, "AddNormalMainUIPlaysIcon", 20);
        //TruelyAddNormalMainUIPlaysIcon(level);
    }

    public void TruelyAddNormalMainUIPlaysIcon(int level)
    {
        if (TruelyAddIconPlaysBottomRightH(level))
        {            
            return;
        }
        if (TruelyAddIconPlaysBottomRightV(level))
        {
            return;
        }
        if (TruelyAddIconPlaysTopRightH(level))
        {
            return;
        }

        MogoUIQueue.Instance.Locked = false;
        return;
    }

    /// <summary>
    /// �������½Ǻ���Icon
    /// </summary>
    /// <param name="level"></param>
    private bool TruelyAddIconPlaysBottomRightH(int level)
    {
        switch (level)
        {
            // ����
            case SystemRequestLevel.EquipmentIcon:
                    //LogicAddIconPlays(IconPlaysType.IconPlaysBottomRightH, IconPlaysBottomRightH.EquipmentIcon, m_MainCamera);          
                    LogicAddIconPlaysEquipmentIcon(IconPlaysBottomRightH.EquipmentIcon, m_MainCamera);
                    break;
            // ����
            case SystemRequestLevel.DiamondToGoldIcon:
                    LogicAddIconPlays(IconPlaysType.IconPlaysBottomRightH, IconPlaysBottomRightH.DiamondToGoldIcon, m_NoramlMainUIBTNListHCam);
                    break;                  
            // ���(�����ݵȼ�)
            //case SystemRequestLevel.WingIcon:
            //        break;
            // ����
            case SystemRequestLevel.DragonIcon:
                    LogicAddIconPlays(IconPlaysType.IconPlaysBottomRightH, IconPlaysBottomRightH.DragonIcon, m_NoramlMainUIBTNListHCam);
                    break;  
            // ����
            case SystemRequestLevel.SpriteIcon:
                    LogicAddIconPlays(IconPlaysType.IconPlaysBottomRightH, IconPlaysBottomRightH.SpriteIcon, m_NoramlMainUIBTNListHCam);
                    break;  
            default:
                return false;
        }

        return true;
    }

    /// <summary>
    /// �������½�����Icon
    /// </summary>
    /// <param name="level"></param>
    private bool TruelyAddIconPlaysBottomRightV(int level)
    {
        switch (level)
        {
            // �ɾ�
            case SystemRequestLevel.AttributeIcon:
                LogicAddIconPlays(IconPlaysType.IconPlaysBottomRightV, IconPlaysBottomRightV.AttributeIcon, m_NoramlMainUIBTNListVCam);
                break;
            default:
                return false;
        }

        return true;
    }

    /// <summary>
    /// �������ϽǺ���Icon
    /// </summary>
    /// <param name="level"></param>
    private bool TruelyAddIconPlaysTopRightH(int level)
    {
        switch (level)
        {
            // ��ս
            case SystemRequestLevel.ChallengeIcon:
                LogicAddIconPlays(IconPlaysType.IconPlaysTopRightH, IconPlaysTopRightH.ChallengeIcon, m_MainCamera);
                break;
            // ÿ������
            case SystemRequestLevel.DailyTaskIcon:
                LogicAddIconPlays(IconPlaysType.IconPlaysTopRightH, IconPlaysTopRightH.DailyTaskIcon, m_MainCamera);              
                break;
            // ������
            case SystemRequestLevel.ArenaIcon:
                LogicAddIconPlays(IconPlaysType.IconPlaysTopRightH, IconPlaysTopRightH.ArenaIcon, m_MainCamera);    
                break;
            default:
                return false;
        }

        return true;
    }

    /// <summary>
    /// ���а���,���а�ť��VIP��ťƯ��
    /// </summary>
    private static bool IsRankingIconAdded = false;
    private void TruelyAddNormalMainUIRankingIcon()
    {
        if (IsRankingIconAdded)
            return;

        IsRankingIconAdded = true;

        TweenPosition tpRankingIcon = m_goNormalMainUIRankingBtn.GetComponentsInChildren<TweenPosition>(true)[0];
        TweenPosition tpVIPIcon = m_goNormalMainUIPlayerVIP.GetComponentsInChildren<TweenPosition>(true)[0];

        tpRankingIcon.Reset();
        tpVIPIcon.Reset();

        tpRankingIcon.callWhenFinished = "";
        tpVIPIcon.callWhenFinished = "";

        m_goNormalMainUIRanking.SetActive(true);
        m_goNormalMainUIRankingBtn.SetActive(true);

        tpRankingIcon.from = m_tranNormalMainUIRankingBtnOriginPos.localPosition;
        tpRankingIcon.to = m_tranNormalMainUIRankingBtnPos.transform.localPosition;
        tpVIPIcon.from = m_tranNormalMainUIPlayerVIPPos1.localPosition;
        tpVIPIcon.to = m_tranNormalMainUIPlayerVIPPos2.localPosition;

        tpRankingIcon.enabled = true;
        tpVIPIcon.enabled = true;

        tpRankingIcon.Play(true);
        tpVIPIcon.Play(true);


        tpRankingIcon.eventReceiver = gameObject;
        tpRankingIcon.callWhenFinished = "OnPlaysIconTPEnd";
        MogoUIQueue.Instance.IsLocking = true;
        TimerHeap.AddTimer(5000, 0, () => { MogoUIManager.Instance.UICanChange = true; });
        //MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].eventReceiverMask = 0;

        MogoUIManager.Instance.UICanChange = false;
        MogoFXManager.Instance.AttachUIFX(2, GameObject.Find("Camera").GetComponent<Camera>(), 0, 0, 0, m_tranNormalMainUIRankingBtnPos.gameObject);
        TimerHeap.AddTimer(5000, 0, () =>
        {
            MogoFXManager.Instance.DetachUIFX(2);
            ShowRankButtonAnimation(true);
        });
    }

    #endregion

    void OnPlaysIconTPEnd()
    {
        //MogoUIManager.Instance.GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].eventReceiverMask = 1<<10;
        MogoUIManager.Instance.UICanChange = true;
        MogoUIQueue.Instance.IsLocking = false;
        MogoUIQueue.Instance.Locked = false;
        Debug.Log("UnLocking~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~by TPEnd");
        //MogoUIManager.Instance.ShowMogoNormalMainUI();
        MogoUIQueue.Instance.CheckQueue();
        NormalMainUIViewManager.Instance.ShowCommunityButton(true);
        MogoFXManager.Instance.DetachUIFX(2);

        if (MogoWorld.thePlayer.level == SystemRequestLevel.DailyTaskIcon)
        {
            EventDispatcher.TriggerEvent(Events.DailyTaskEvent.GetDailyEventData);
            GuideSystem.Instance.TriggerEvent<uint>(GlobalEvents.OpenGUI, 208);
        }
        if (MogoWorld.thePlayer.level == SystemRequestLevel.DiamondToGoldIcon)
        {
            GuideSystem.Instance.TriggerEvent<uint>(GlobalEvents.OpenGUI, 207);
        }
        if (MogoWorld.thePlayer.level == SystemRequestLevel.ArenaIcon)
        {
            GuideSystem.Instance.TriggerEvent<uint>(GlobalEvents.OpenGUI, 7);
        }
        if (MogoWorld.thePlayer.level == SystemRequestLevel.RankingIcon)
        {
            TruelyAddNormalMainUIRankingIcon();
        }

        // ��ʾ��ˢ��Icon
        if (MogoWorld.thePlayer != null)
        {
            ShowNormalMainUIPlaysIcon(MogoWorld.thePlayer.level);
        }
    }

    #endregion

    #region ��ʾ��ˢ��Icon

    public void ShowNormalMainUIPlaysIcon(int level)
    {
        ShowIconPlaysBottomRightH(level);
        ShowIconPlaysBottomRightV(level);
        ShowIconPlaysTopRightH(level);
        ShowIconPlaysTopLeftH(level);   
    }

    /// <summary>
    /// ���½Ǻ���Icon
    /// </summary>
    /// <param name="level"></param>
    private void ShowIconPlaysBottomRightH(int level)
    {
        // ����
        if (level >= SystemRequestLevel.PackageIcon)
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.PackageIcon].SetActive(true);
        else
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.PackageIcon].SetActive(false);

        // ����
        if (level >= SystemRequestLevel.EquipmentIcon)
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.EquipmentIcon].SetActive(true);
        else
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.EquipmentIcon].SetActive(false);

        // ����
        if (level >= SystemRequestLevel.DragonIcon)
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.DragonIcon].SetActive(true);
        else
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.DragonIcon].SetActive(false);

        // ����
        if (level >= SystemRequestLevel.DiamondToGoldIcon)
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.DiamondToGoldIcon].SetActive(true);
        else
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.DiamondToGoldIcon].SetActive(false);

        // ����ϵͳ
         if (level >= SystemRequestLevel.SpriteIcon)
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.SpriteIcon].SetActive(true);
        else
            m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.SpriteIcon].SetActive(false);

         // ���(�����ݵȼ�)
         if (IsWingIconHasOpened)
         {
             List<int> listFront = GetIconPlaysBottomRightHFront(IconPlaysBottomRightH.WingIcon);
             if (listFront.Count >= 2)
             {
                 m_goNMWingIcon2.SetActive(false);
                 m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.WingIcon].SetActive(true);
             }
             else
             {
                 m_goNMWingIcon2.SetActive(true);
                 m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.WingIcon].SetActive(false);
             }
         }
         else
         {
             m_maplistIconPlaysBottomRightH[IconPlaysBottomRightH.WingIcon].SetActive(false);
             m_goNMWingIcon2.SetActive(false);
         }

        // ˢ��Iconλ��          
         if (IsBottomRightButtonsShow)
             OpenIconPlaysBottomRightH();
         else
             CloseIconPlaysBottomRightH();

        // ˢ��Icon����
        RefreshIconPlaysBottomRightHAnim();
    }

    /// <summary>
    /// ���½�����Icon
    /// </summary>
    /// <param name="level"></param>
    private void ShowIconPlaysBottomRightV(int level)
    {
        // �ɾ�
        if(level >= SystemRequestLevel.AttributeIcon)
            m_maplistIconPlaysBottomRightV[IconPlaysBottomRightV.AttributeIcon].SetActive(true);
        else
            m_maplistIconPlaysBottomRightV[IconPlaysBottomRightV.AttributeIcon].SetActive(false);
            
        // �̳�
        if(level >= SystemRequestLevel.MallConsumeIcon)
            m_maplistIconPlaysBottomRightV[IconPlaysBottomRightV.MallConsumeIcon].SetActive(true);
        else
            m_maplistIconPlaysBottomRightV[IconPlaysBottomRightV.MallConsumeIcon].SetActive(false);
     
        // ˢ��Iconλ��
        if (IsBottomRightButtonsShow)
            OpenIconPlaysBottomRightV();
        else
            CloseIconPlaysBottomRightV();

        // ˢ��Icon����
        RefreshIconPlaysBottomRightVAnim();
    }

    /// <summary>
    /// ���ϽǺ���Icon
    /// </summary>
    /// <param name="level"></param>
    private void ShowIconPlaysTopRightH(int level)
    {
        // �Զ�����
        if(level >= SystemRequestLevel.AutoTaskPlayIcon)
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.AutoTaskPlayIcon].SetActive(true);
        else
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.AutoTaskPlayIcon].SetActive(false);

        // ÿ������
        if(level >= SystemRequestLevel.DailyTaskIcon)
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.DailyTaskIcon].SetActive(true);
        else
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.DailyTaskIcon].SetActive(false);

        // �(ԭ������ս)
        if(level >= SystemRequestLevel.ChallengeIcon)
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.ChallengeIcon].SetActive(true);
        else
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.ChallengeIcon].SetActive(false);

        // ������
        if(level >= SystemRequestLevel.ArenaIcon)
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.ArenaIcon].SetActive(true);
        else
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.ArenaIcon].SetActive(false);

        // ����
        if(level >= SystemRequestLevel.ChargeRewardIcon)
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.ChargeRewardIcon].SetActive(true);
        else
            m_maplistIconPlaysTopRightH[IconPlaysTopRightH.ChargeRewardIcon].SetActive(false);

        // ˢ��Iconλ��
        int index = 0;
        for (int j = 0; j < m_maplistIconPlaysTopRightH.Count; ++j)
        {
            if (m_maplistIconPlaysTopRightH[j].activeSelf)
            {
                m_maplistIconPlaysTopRightH[j].transform.localPosition = m_listIconPlaysTopRightHPos[index++].transform.localPosition;
            }
        }           
    }

    /// <summary>
    /// ���ϽǺ���Icon
    /// </summary>
    /// <param name="level"></param>
    private void ShowIconPlaysTopLeftH(int level)
    {
        if (level >= SystemRequestLevel.RankingIcon)
            m_goNormalMainUIRanking.SetActive(true);
        else
            m_goNormalMainUIRanking.SetActive(false);
        RefreshRankingPos(level);
    }

    #endregion

    #endregion

    #region ˢ��Icon��������

    /// <summary>
    /// ˢ�����½Ǻ���Icon��������
    /// </summary>
    private void RefreshIconPlaysBottomRightHAnim()
    {
        // WingIcon2: ���⴦��
        if (IsWingIconHasOpened && m_goNMWingIcon2.activeSelf)
        {
            TweenPosition tp = m_goNMWingIcon2.GetComponentsInChildren<TweenPosition>(true)[0];
            //tp.Reset();
            tp.callWhenFinished = "";
            tp.duration = ControllerIconRotationTime;
            tp.from = m_listIconPlaysBottomRightHPos[1].transform.localPosition;
            tp.to = m_listIconPlaysBottomRightHPos[0].transform.localPosition;
        }

        int posIndex = 0;
        for (int index = 0; index < IconPlaysBottomRightH.MaxCount; index++)
        {
            if (m_maplistIconPlaysBottomRightH.ContainsKey(index))
            {
                if (!m_maplistIconPlaysBottomRightH[index].activeSelf)
                {
                    continue;
                }              

                if (index == IconPlaysBottomRightH.PackageIcon || index == IconPlaysBottomRightH.EquipmentIcon)
                {
                    m_maplistIconPlaysBottomRightH[index].transform.localPosition = m_listIconPlaysBottomRightHPos[posIndex].transform.localPosition; 
            
                    posIndex++;
                    continue;
                }
                else
                {
                    TweenPosition tp = m_maplistIconPlaysBottomRightH[index].GetComponentsInChildren<TweenPosition>(true)[0];
                    //tp.Reset();
                    tp.callWhenFinished = "";
                    tp.duration = ControllerIconRotationTime;
                    tp.from = m_listIconPlaysBottomRightHPos[posIndex].transform.localPosition;
                    tp.to = new Vector3(NoramlMainUIBTNListH_HideX,
                        m_goNoramlMainUIBTNListH.transform.localPosition.y,
                        m_goNoramlMainUIBTNListH.transform.localPosition.z);

                    posIndex++;
                }                
            }
        }
    }

    /// <summary>
    /// ˢ�����½�����Icon��������
    /// </summary>
    private void RefreshIconPlaysBottomRightVAnim()
    {
        int posIndex = 0;
        for (int index = 0; index < IconPlaysBottomRightV.MaxCount; index++)
        {
            if (m_maplistIconPlaysBottomRightV.ContainsKey(index))
            {
                if (!m_maplistIconPlaysBottomRightV[index].activeSelf)
                {
                    continue;
                }

                if (index == IconPlaysBottomRightV.MallConsumeIcon)
                {
                    TweenPosition tp = m_maplistIconPlaysBottomRightV[index].GetComponentsInChildren<TweenPosition>(true)[0];
                    //tp.Reset();
                    tp.callWhenFinished = "";
                    tp.duration = ControllerIconRotationTime;
                    tp.from = m_listIconPlaysBottomRightVPos[posIndex].transform.localPosition;
                    tp.to = m_listIconPlaysBottomRightVPos[IconPlaysBottomRightV.AttributeIcon].transform.localPosition;

                    posIndex++;
                }
                else
                {
                    TweenPosition tp = m_maplistIconPlaysBottomRightV[index].GetComponentsInChildren<TweenPosition>(true)[0];
                    //tp.Reset();
                    tp.callWhenFinished = "";
                    tp.duration = ControllerIconRotationTime;
                    tp.from = m_listIconPlaysBottomRightVPos[posIndex].transform.localPosition;
                    tp.to = new Vector3(NoramlMainUIBTNListV_HideY,
                        m_goNoramlMainUIBTNListH.transform.localPosition.y,
                        m_goNoramlMainUIBTNListH.transform.localPosition.z);

                    posIndex++;
                }                
            }
        }
    }

    #endregion

    #region ����Icon�۵���չ��(����Icon����)

    /// <summary>
    /// NMAnim:�������½Ǻ���Icon��������
    /// </summary>
    private void PlayIconPlaysBottomRightHAnim()
    {
        // WingIcon2: ���⴦��
        ShowIconPlaysBottomRightHIcon(true, IconPlaysBottomRightH.WingIcon);
        TweenPosition tpWingIcon2 = m_goNMWingIcon2.GetComponentsInChildren<TweenPosition>(true)[0];
        tpWingIcon2.SetFactor(!IsBottomRightButtonsShow);
        tpWingIcon2.Play(!IsBottomRightButtonsShow);

        for (int index = 0; index < IconPlaysBottomRightH.MaxCount; index++)
        {
            if (m_maplistIconPlaysBottomRightH.ContainsKey(index))
            {
                if (!m_maplistIconPlaysBottomRightH[index].activeSelf)
                {
                    continue;
                }
                if (index == IconPlaysBottomRightH.PackageIcon || index == IconPlaysBottomRightH.EquipmentIcon)
                {
                    continue;
                }
                else
                {
                    // special deal : չ��Iconʱ��ʾIcon
                    if (IsBottomRightButtonsShow)
                    {
                        ShowIconPlaysBottomRightHIcon(true, index);
                    }

                    TweenPosition tp = m_maplistIconPlaysBottomRightH[index].GetComponentsInChildren<TweenPosition>(true)[0];
                    tp.SetFactor(!IsBottomRightButtonsShow);                
                    tp.Play(!IsBottomRightButtonsShow);
                }
            }
        }
    }

    /// <summary>
    /// NMAnim:�������½�����Icon��������
    /// </summary>
    private void PlayIconPlaysBottomRightVAnim()
    {
        for (int index = 0; index < IconPlaysBottomRightV.MaxCount; index++)
        {
            if (m_maplistIconPlaysBottomRightV.ContainsKey(index))
            {
                if (!m_maplistIconPlaysBottomRightV[index].activeSelf)
                {
                    continue;
                }
                else
                {
                    // special deal : չ��Iconʱ��ʾIcon
                    if (IsBottomRightButtonsShow)
                    {
                        ShowIconPlaysBottomRightVIcon(true, index);
                    }

                    TweenPosition tp = m_maplistIconPlaysBottomRightV[index].GetComponentsInChildren<TweenPosition>(true)[0];
                    tp.SetFactor(!IsBottomRightButtonsShow);                   
                    tp.Play(!IsBottomRightButtonsShow);                    
                }              
            }
        }
    }

    #endregion

    #region ����Icon�۵���չ��(������Icon����)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OpenIconPlaysBottomRight();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CloseIconPlaysBottomRight();       
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AddWingPlaysIcon();
            IsWingIconHasOpened = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            IsWingIconHasOpened = false;
        }
    }

    #region ����Iconչ��(�����Ŷ���)

    public void OpenIconPlaysBottomRight()
    {
        OpenIconPlaysBottomRightH();
        OpenIconPlaysBottomRightV();
        OpenControllerIcon();
        HideCommunityIcon();
        HideHelpTip();
        IsBottomRightButtonsShow = true;
    }    

    private void OpenIconPlaysBottomRightH()
    {
        IsBottomRightButtonsShow = true;

        // WingIcon2: ���⴦��
        ShowIconPlaysBottomRightHIcon(true, IconPlaysBottomRightH.WingIcon);

        // ����Icon
        int index = 0;
        for (int j = 0; j < m_maplistIconPlaysBottomRightH.Count; ++j)
        {
            if (m_maplistIconPlaysBottomRightH.ContainsKey(j))
            {
                m_maplistIconPlaysBottomRightH[j].GetComponentsInChildren<TweenPosition>(true)[0].SetFactor(BottomRightButtonShowInit);
                if (m_maplistIconPlaysBottomRightH[j].activeSelf)
                {
                    // special deal : չ��Iconʱ��ʾIcon
                    ShowIconPlaysBottomRightHIcon(true, j);
                    m_maplistIconPlaysBottomRightH[j].transform.localPosition = m_listIconPlaysBottomRightHPos[index++].transform.localPosition;
                }
            }           
        }       
    }

    private void OpenIconPlaysBottomRightV()
    {
        IsBottomRightButtonsShow = true;
        int index = 0;
        for (int j = 0; j < m_maplistIconPlaysBottomRightV.Count; ++j)
        {
            if (m_maplistIconPlaysBottomRightV.ContainsKey(j))
            {
                m_maplistIconPlaysBottomRightV[j].GetComponentsInChildren<TweenPosition>(true)[0].SetFactor(BottomRightButtonShowInit);
                if (m_maplistIconPlaysBottomRightV[j].activeSelf)
                {
                    // special deal : չ��Iconʱ��ʾIcon
                    ShowIconPlaysBottomRightVIcon(true, j);
                    m_maplistIconPlaysBottomRightV[j].transform.localPosition = m_listIconPlaysBottomRightVPos[index++].transform.localPosition;
                }
            }          
        }
    }

    private void OpenControllerIcon()
    {        
        GameObject goControllerIcon = m_maplistGoIconPlays[(int)IconPlays.ControllerIcon];
        goControllerIcon.GetComponentsInChildren<TweenRotation>(true)[0].SetFactor(BottomRightButtonShowInit);
        Vector3 z_axis = new Vector3(0, 0, -1);
        goControllerIcon.transform.localRotation = Quaternion.AngleAxis(ControllerIconRotationZShow, z_axis);
    }

    private void HideCommunityIcon()
    {
        m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition = new Vector3(
           m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.x,
           CommunityIconHideY,
           m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.z);
    }

    private void HideHelpTip()
    {
        m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition = new Vector3(HelpTipHideX,
         m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.y,
         m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.z);
    }

    #endregion

    #region ����Icon�۵�(�����Ŷ���)

    private void CloseIconPlaysBottomRight()
    {
        CloseIconPlaysBottomRightH();
        CloseIconPlaysBottomRightV();
        CloseControllerIcon();
        ShowCommunityIcon();
        ShowHelpTip();
        IsBottomRightButtonsShow = false;
    }

    private void CloseIconPlaysBottomRightH()
    {
        IsBottomRightButtonsShow = false;

        // WingIcon2: ���⴦��
        ShowIconPlaysBottomRightHIcon(false, IconPlaysBottomRightH.WingIcon);

        // ����Icon
        int posIndex = 0;
        for (int index = 0; index < IconPlaysBottomRightH.MaxCount; index++)
        {
            if (m_maplistIconPlaysBottomRightH.ContainsKey(index))
            {
                m_maplistIconPlaysBottomRightH[index].GetComponentsInChildren<TweenPosition>(true)[0].SetFactor(BottomRightButtonShowInit);
                if (!m_maplistIconPlaysBottomRightH[index].activeSelf)
                {
                    continue;
                }

                if (index == IconPlaysBottomRightH.PackageIcon || index == IconPlaysBottomRightH.EquipmentIcon)
                {
                    m_maplistIconPlaysBottomRightH[index].transform.localPosition = m_listIconPlaysBottomRightHPos[posIndex].transform.localPosition;

                    posIndex++;
                    continue;
                }
                else
                {
                    ShowIconPlaysBottomRightHIcon(false, index);
                     
                    m_maplistIconPlaysBottomRightH[index].transform.localPosition = new Vector3(NoramlMainUIBTNListH_HideX,
                        m_goNoramlMainUIBTNListH.transform.localPosition.y,
                        m_goNoramlMainUIBTNListH.transform.localPosition.z);

                    posIndex++;
                }
            }
        }       
    }

    private void CloseIconPlaysBottomRightV()
    {
        IsBottomRightButtonsShow = false;
        int posIndex = 0;
        for (int index = 0; index < IconPlaysBottomRightV.MaxCount; index++)
        {
            if (m_maplistIconPlaysBottomRightV.ContainsKey(index))
            {
                m_maplistIconPlaysBottomRightV[index].GetComponentsInChildren<TweenPosition>(true)[0].SetFactor(BottomRightButtonShowInit);
                if (!m_maplistIconPlaysBottomRightV[index].activeSelf)
                {
                    continue;
                }

                if (index == IconPlaysBottomRightV.MallConsumeIcon)
                {
                    m_maplistIconPlaysBottomRightV[index].transform.localPosition = 
                        m_listIconPlaysBottomRightVPos[IconPlaysBottomRightV.AttributeIcon].transform.localPosition;

                    posIndex++;
                }
                else
                {
                    ShowIconPlaysBottomRightVIcon(false, index);                

                    m_maplistIconPlaysBottomRightV[index].transform.localPosition = new Vector3(
                    NoramlMainUIBTNListV_HideY,
                    m_goNoramlMainUIBTNListH.transform.localPosition.y,
                    m_goNoramlMainUIBTNListH.transform.localPosition.z);

                    posIndex++;
                }               
            }
        }
    }

    private void CloseControllerIcon()
    {
        GameObject goControllerIcon = m_maplistGoIconPlays[(int)IconPlays.ControllerIcon];
        //goControllerIcon.GetComponentsInChildren<TweenRotation>(true)[0].SetFactor(!BottomRightButtonShowInit);
        goControllerIcon.GetComponentsInChildren<TweenRotation>(true)[0].SetFactor(BottomRightButtonShowInit);
        Vector3 z_axis = new Vector3(0, 0, -1);
        goControllerIcon.transform.localRotation = Quaternion.AngleAxis(ControllerIconRotationZHide, z_axis);
    }

    private void ShowCommunityIcon()
    {
        m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition = new Vector3(
            m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.x,
            CommunityIconShowY,
            m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.z);
    }

    private void ShowHelpTip()
    {
        m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition = new Vector3(HelpTipShowX,
          m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.y,
          m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.z);
    }

    #endregion

    #region ��ʾ������Icon

    /// <summary>
    /// special deal : �۵�Iconʱ����Icon, չ��Iconʱ��ʾIcon
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="index"></param>
    private void ShowIconPlaysBottomRightHIcon(bool isShow, int index)
    {
        if (index == IconPlaysBottomRightH.DragonIcon)
        {
            m_maplistIconPlaysBottomRightH[index].transform.Find("DragonConsumeIcon").gameObject.SetActive(isShow);
        }
        else if (index == IconPlaysBottomRightH.DiamondToGoldIcon)
        {
            m_maplistIconPlaysBottomRightH[index].transform.Find("DiamondToGoldIcon").gameObject.SetActive(isShow);
        }
        else if (index == IconPlaysBottomRightH.SpriteIcon)
        {
            m_maplistIconPlaysBottomRightH[index].transform.Find("SpriteIcon").gameObject.SetActive(isShow);
        }
        else if (index == IconPlaysBottomRightH.WingIcon)
        {
            // ������⴦��           
            m_maplistIconPlaysBottomRightH[index].transform.Find("WingIcon").gameObject.SetActive(isShow);
            if (m_goNMWingIcon2.activeSelf)
            {
                m_goNMWingIcon2.transform.Find("WingIcon2").gameObject.SetActive(isShow);
            }
        }                     
    }

    /// <summary>
    /// special deal : �۵�Iconʱ����Icon, չ��Iconʱ��ʾIcon
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="index"></param>
    private void ShowIconPlaysBottomRightVIcon(bool isShow, int index)
    {
        if (index == IconPlaysBottomRightV.AttributeIcon)
        {
            m_maplistIconPlaysBottomRightV[index].transform.Find("AttributeIcon").gameObject.SetActive(isShow);
        }        
    }

    #endregion

    #endregion

    #region �����水ť����

    private GameObject m_goNoramlMainUIBTNListH;
    private GameObject m_goNoramlMainUIBTNListV;
    private List<Transform> m_listNoramlMainUIBTNListHPos = new List<Transform>();
    private List<Transform> m_listNoramlMainUIBTNListVPos = new List<Transform>();

    /// <summary>
    /// Ĭ���۵���չ����trueչ����false�۵�
    /// </summary>
    private readonly static bool BottomRightButtonShowInit = false;
    /// <summary>
    /// �Ƿ�չ�����½ǰ�ť��trueչ����false�۵�
    /// </summary>
    private bool m_isBottomRightButtonsShow = BottomRightButtonShowInit;
    public bool IsBottomRightButtonsShow
    {
        get { return m_isBottomRightButtonsShow; }
        set
        {
            m_isBottomRightButtonsShow = value;
        }
    }

    private void ShowBottomRightButtons(bool isShow)
    {
        IsBottomRightButtonsShow = isShow;
        PlayNoramlMainUIBTNListAnim();
    }

    /// <summary>
    /// ���ſ��ư�ť����
    /// </summary>
    /// <param name="isShow"></param>
    private readonly static float ControllerIconRotationZShow = 45;
    private readonly static float ControllerIconRotationZHide = 0;
    private readonly static float ControllerIconRotationTime = 0.3f;
    private void InitControllerIconAnim()
    {
        float fromZ;
        float toZ;
        if (BottomRightButtonShowInit)
        {
            fromZ = ControllerIconRotationZShow;
            toZ = ControllerIconRotationZHide;
        }
        else
        {
            fromZ = ControllerIconRotationZHide;
            toZ = ControllerIconRotationZShow;
        }

        GameObject goControllerIcon = m_maplistGoIconPlays[(int)IconPlays.ControllerIcon];
        TweenRotation tr = goControllerIcon.GetComponentsInChildren<TweenRotation>(true)[0];
        tr.Reset();
        tr.duration = ControllerIconRotationTime;
        tr.from = new Vector3(goControllerIcon.transform.localRotation.x, goControllerIcon.transform.localRotation.y, fromZ);
        tr.to = new Vector3(goControllerIcon.transform.localRotation.x, goControllerIcon.transform.localRotation.y, toZ);

        // ��ʼ��λ��
        Vector3 z_axis = new Vector3(0, 0, -1);
        goControllerIcon.transform.localRotation = Quaternion.AngleAxis(fromZ, z_axis);

        UIButtonTween bt = goControllerIcon.GetComponentsInChildren<UIButtonTween>(true)[0];
        bt.callWhenFinished = "OnControllerIconAnimFinish";
        bt.eventReceiver = gameObject;
    }

    private readonly static int NoramlMainUIBTNListH_Num = 6;
    private readonly static int NoramlMainUIBTNListV_Num = 3;
    private readonly static float NoramlMainUIBTNListH_Space = 120;
    private readonly static float NoramlMainUIBTNListV_Space = -120;
    private readonly static float NoramlMainUIBTNListH_HideX = 0;
    private readonly static float NoramlMainUIBTNListV_HideY = 0;
    private void InitNoramlMainUIBTNListHAnim()
    {
        TweenPosition tpH = m_goNoramlMainUIBTNListH.GetComponentsInChildren<TweenPosition>(true)[0];
        tpH.Reset();
        tpH.duration = ControllerIconRotationTime;
        tpH.from = new Vector3(NoramlMainUIBTNListH_Num * NoramlMainUIBTNListH_Space,
            m_goNoramlMainUIBTNListH.transform.localPosition.y,
            m_goNoramlMainUIBTNListH.transform.localPosition.z);
        tpH.to = new Vector3(NoramlMainUIBTNListH_HideX,
            m_goNoramlMainUIBTNListH.transform.localPosition.y,
            m_goNoramlMainUIBTNListH.transform.localPosition.z);
    }

    private void InitNoramlMainUIBTNListVAnim()
    {
        TweenPosition tpV = m_goNoramlMainUIBTNListV.GetComponentsInChildren<TweenPosition>(true)[0];
        tpV.Reset();
        tpV.duration = ControllerIconRotationTime;
        tpV.from = new Vector3(m_goNoramlMainUIBTNListV.transform.localPosition.x,
            NoramlMainUIBTNListV_Num * NoramlMainUIBTNListV_Space,
            m_goNoramlMainUIBTNListV.transform.localPosition.z);
        tpV.to = new Vector3(m_goNoramlMainUIBTNListV.transform.localPosition.x,
            NoramlMainUIBTNListV_HideY,
            m_goNoramlMainUIBTNListV.transform.localPosition.z);
    }

    private void PlayNoramlMainUIBTNListAnim()
    {
        //m_goNoramlMainUIBTNListH.GetComponentsInChildren<UIButtonTween>(true)[0].Play(true);
        //m_goNoramlMainUIBTNListV.GetComponentsInChildren<UIButtonTween>(true)[0].Play(true);
        PlayIconPlaysBottomRightHAnim();
        PlayIconPlaysBottomRightVAnim();
        PlayCommunityIconAnim();
        PlayHelpTipAnim();
    }

    private void OnControllerIconAnimFinish()
    {
        if (!IsBottomRightButtonsShow)
        {
            CloseIconPlaysBottomRightH();
            CloseIconPlaysBottomRightV();
        }
    }

    #region ���½��������

    private Camera m_MainCamera;
    private Camera m_NoramlMainUIBTNListHCam;
    private Camera m_NoramlMainUIBTNListVCam;

    public void ShowNormalBTNListCam(bool isShow)
    {
        if (m_NoramlMainUIBTNListHCam != null && m_NoramlMainUIBTNListVCam != null)
        {
            if (!isShow)
            {
                m_NoramlMainUIBTNListHCam.enabled = false;                                   
                m_NoramlMainUIBTNListVCam.enabled = false;
            }
            else
            {
                m_NoramlMainUIBTNListVCam.enabled = true;
                m_NoramlMainUIBTNListHCam.enabled = true;
            }
        }        
    }

    #endregion

    #region ���찴ť

    private readonly static float CommunityIconShowY = 54;
    private readonly static float CommunityIconHideY = -200;

    private void InitCommunityIconAnim()
    {
        float fromY;
        float toY;
        if (BottomRightButtonShowInit)
        {
            fromY = CommunityIconHideY;
            toY = CommunityIconShowY;
        }
        else
        {
            fromY = CommunityIconShowY;
            toY = CommunityIconHideY;
        }

        TweenPosition tpH = m_maplistGoIconPlays[IconPlays.NMCommunityIcon].GetComponentsInChildren<TweenPosition>(true)[0];
        tpH.Reset();
        tpH.duration = ControllerIconRotationTime;
        tpH.from = new Vector3(m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.x,
            fromY,
            m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.z);
        tpH.to = new Vector3(m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.x,
            toY,
            m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.z);

        m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition = new Vector3(
           m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.x,
           fromY,
           m_maplistGoIconPlays[IconPlays.NMCommunityIcon].transform.localPosition.z);
    }

    /// <summary>
    /// NMAnim:�����������λ�ƶ���
    /// </summary>
    private void PlayCommunityIconAnim()
    {
        TweenPosition tp = m_maplistGoIconPlays[IconPlays.NMCommunityIcon].GetComponentsInChildren<TweenPosition>(true)[0];
        tp.SetFactor(IsBottomRightButtonsShow);
        tp.Play(IsBottomRightButtonsShow);
    }

    #endregion

    #region ������ʾ

    private readonly static float HelpTipShowX = -9;
    private readonly static float HelpTipHideX = 800;

    private void InitHelpTipAnim()
    {
        float fromY;
        float toY;
        if (BottomRightButtonShowInit)
        {
            fromY = HelpTipHideX;
            toY = HelpTipShowX;
        }
        else
        {
            fromY = HelpTipShowX;
            toY = HelpTipHideX;
        }

        TweenPosition tpH = m_maplistGoIconPlays[IconPlays.NMHelpTip].GetComponentsInChildren<TweenPosition>(true)[0];
        tpH.Reset();
        tpH.duration = ControllerIconRotationTime;
        tpH.from = new Vector3(fromY,
            m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.y,
            m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.z);
        tpH.to = new Vector3(toY,
            m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.y,
            m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.z);

        m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition = new Vector3(fromY,
           m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.y,
           m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.z);
    }

    /// <summary>
    /// NMAnim:����������ʾλ�ƶ���
    /// </summary>
    private void PlayHelpTipAnim()
    {
        if (m_maplistGoIconPlays[IconPlays.NMHelpTip].activeSelf)
        {
            TweenPosition tp = m_maplistGoIconPlays[IconPlays.NMHelpTip].GetComponentsInChildren<TweenPosition>(true)[0];
            tp.SetFactor(IsBottomRightButtonsShow);
            tp.Play(IsBottomRightButtonsShow);
        }
    }

    public void ShowHelpTip(bool isShow)
    {
        if (m_maplistGoIconPlays.ContainsKey(IconPlays.NMHelpTip))
        {
            if (isShow)
            {
                if (!IsBottomRightButtonsShow)
                {
                    m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition = new Vector3(HelpTipShowX,
                     m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.y,
                     m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.z);
                }
                else
                {
                    m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition = new Vector3(HelpTipHideX,
                       m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.y,
                       m_maplistGoIconPlays[IconPlays.NMHelpTip].transform.localPosition.z);
                }
            }

            m_maplistGoIconPlays[IconPlays.NMHelpTip].gameObject.SetActive(isShow);
        }
    }

    #endregion

    #endregion   

    #endregion    

    #region ���Ͻ���Ϣ

    #region �/��ֵ����

    public void ShowChargeRewardIconTip(bool isShow)
    {
        m_goGOChargeRewardIconNotice.SetActive(isShow);
    }

    /// <summary>
    /// ��ʱ�����ǰΪ"��ֵ����"��
    /// ��ʱ������Ϊ"�";
    /// </summary>
    /// <param name="beforeLimitActivity"></param>
    public void ShowChargeRewardIcon(int level)
    {
        bool beforeLimitActivity = true;
        if (level >= 11)
            beforeLimitActivity = false;

        if (beforeLimitActivity)
        {
            if (m_lblChargeRewardIconText != null && m_spChargeRewardIconBGUp != null)
            {
                m_lblChargeRewardIconText.text = LanguageData.GetContent(46850); // "��ֵ����";
                m_spChargeRewardIconBGUp.spriteName = "zc_shoucongjiangli_up";
            }
        }
        else
        {
            if (m_lblChargeRewardIconText != null && m_spChargeRewardIconBGUp != null)
            {
                m_lblChargeRewardIconText.text = LanguageData.GetContent(46851); // "�";
                m_spChargeRewardIconBGUp.spriteName = "zc_huodong_up";
            }
        }
    }

    #endregion

    public void SetAutoTaskText(string name)
    {
        m_goAutoTaskBtn.transform.Find("AutoTaskPlayIconText").GetComponentsInChildren<UILabel>(true)[0].text = name;
    }

    public void SetAutoTaskIcon(string iconName)
    {
        m_goAutoTaskBtn.transform.Find("AutoTaskPlayIconBGUp").GetComponentsInChildren<UISprite>(true)[0].spriteName = iconName;
    }

    public void ShowChallegeIconTip()
    {
        m_goGOPVEPlayIconNotice.SetActive(true);
    }

    public void HideChallegeIconTip()
    {
        m_goGOPVEPlayIconNotice.SetActive(false);
    }

    public void ShowArenaIconTip()
    {
        m_goGOPVPPlayIconNotice.SetActive(true);
    }

    public void HideArenaIconTip()
    {
        m_goGOPVPPlayIconNotice.SetActive(false);
    }

    #endregion

    #region ���½���Ϣ

    public void ShowMallConsumeIconTip(bool isShow)
    {
        m_goMallConsumeIconNotice.SetActive(isShow);
    }

    #endregion   

    #region ���Ͻ���Ϣ

    #region ����֮�ź;�����CD��ʾ

    /// <summary>
    /// ����֮�ſ�ʼ����ʱ
    /// </summary>
    //private MogoCountDown m_doorOfBuryCountDown = null;
    public void DoorOfBuryBeginCountDown(int theHour, int theMinutes, int theSecond)
    {
        return;
        //if (m_doorOfBuryCountDown != null)
        //    m_doorOfBuryCountDown.Release();

        //m_goDoorOfBuryOpenCDTipCountDown.SetActive(true);
        //m_goDoorOfBuryOpenCDTipBtnOpen.SetActive(false);
        //m_doorOfBuryCountDown = new MogoCountDown(m_lblDoorOfBuryOpenCDTipCountDownNum, theHour, theMinutes, theSecond,
        //    "", "", "", MogoCountDown.TimeStringType.UpToHour, () =>
        //    {
        //        ShowDoorOfBuryBtnOpen();
        //    });
    }

    /// <summary>
    /// ��ʾ����֮�Ŵ򿪰�ť
    /// </summary>
    public void ShowDoorOfBuryBtnOpen()
    {
        return;
        //m_goDoorOfBuryOpenCDTipCountDown.SetActive(false);
        //m_goDoorOfBuryOpenCDTipBtnOpen.SetActive(true);
    }

    /// <summary>
    /// ֹͣ����֮��CD��ʾ
    /// </summary>
    public void StopDoorOfBuryCDTip()
    {
        return;
        //m_goDoorOfBuryOpenCDTipCountDown.SetActive(false);
        //m_goDoorOfBuryOpenCDTipBtnOpen.SetActive(false);
    }

    /// <summary>
    /// ��������ʼ����ʱ
    /// </summary>
    //private MogoCountDown m_arenaCountDown = null;
    public void ArenaBeginCountDown(int theHour, int theMinutes, int theSecond)
    {
        return;
        //if (m_arenaCountDown != null)
        //    m_arenaCountDown.Release();

        //m_goArenaCDTipCountDown.SetActive(true);
        //m_goArenaCDTipBtnOpen.SetActive(false);
        //m_arenaCountDown = new MogoCountDown(m_lblArenaCDTipCountDownNum, theHour, theMinutes, theSecond,
        //    "", "", "", MogoCountDown.TimeStringType.UpToHour, () =>
        //    {
        //        ShowArenaBtnOpen();
        //    });
    }

    /// <summary>
    /// ��ʾ�������򿪰�ť
    /// </summary>
    public void ShowArenaBtnOpen()
    {
        return;
        //m_goArenaCDTipCountDown.SetActive(false);
        //m_goArenaCDTipBtnOpen.SetActive(true);
    }

    /// <summary>
    /// ֹͣ������CD��ʾ
    /// </summary>
    public void StopArenaCDTip()
    {
        return;
        //m_goArenaCDTipCountDown.SetActive(false);
        //m_goArenaCDTipBtnOpen.SetActive(false);
    }

    #endregion

    #region VIPBuff

    private UILabel m_lblNormalMainUIVIPBuffBtnText;
    private UILabel m_lblNormalMainUIVIPBuffBtnLine;
    private MogoCountDown m_NormalMainUIVIPBuffCountDown;
    private MogoCountDown m_VIPBuffCountDown = null;

    /// <summary>
    /// �Ƿ���ʾVIPBuff��ť
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowVIPBuffBtn(bool isShow, int buffId = 0, UInt32 time = 0)
    {
        m_goNormalMainUIVIPBuffBtn.SetActive(isShow);
        if (buffId > 0)
        {
            SkillBuffData buff = SkillBuffData.dataMap[buffId];
            if (buff != null)
            {
                SetVIPBuffName("VIPBuff");
                SetVIPBuffLevel(MogoWorld.thePlayer.VipLevel);
                SetVIPBuffLastTime(time);
                SetNormalMainUIVIPBuffLastTime(time, MogoWorld.thePlayer.VipLevel);
            }
        }
        if (isShow == false)
            ShowVIPBuffInfo(false);
    }

    /// <summary>
    /// �Ƿ���ʾVIPBuff��Ϣ
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowVIPBuffInfo(bool isShow)
    {
        m_goGONormalMainUIVIPBuffInfo.SetActive(isShow);
    }

    /// <summary>
    /// ����VIPBuff����
    /// </summary>
    /// <param name="name"></param>
    public void SetVIPBuffName(string name)
    {
        //m_lblNormalMainUIVIPBuffName.text = name;
    }

    /// <summary>
    /// ����VIPBuff�ȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetVIPBuffLevel(int level)
    {
        //m_lblNormalMainUIVIPBuffInfoLevel.text = level.ToString();

        if (level < 10)
        {
            m_spNormalMainUIVIPBuffInfoLevelVIPNum0.spriteName = level.ToString();
            m_spNormalMainUIVIPBuffInfoLevelVIPNum1.gameObject.SetActive(false);
        }
        else if (level >= 10)
        {
            m_spNormalMainUIVIPBuffInfoLevelVIPNum0.spriteName = "1";
            m_spNormalMainUIVIPBuffInfoLevelVIPNum1.spriteName = (level - 10).ToString();
            m_spNormalMainUIVIPBuffInfoLevelVIPNum1.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// ����VIPBuffʣ��ʱ��
    /// </summary>
    /// <param name="time"></param>
    public void SetVIPBuffLastTime(UInt32 time)
    {
        if (time > 0)
        {
            if (m_VIPBuffCountDown != null)
                m_VIPBuffCountDown.Release();

            m_VIPBuffCountDown = new MogoCountDown(m_lblNormalMainUIVIPBuffInfoLastTime, (int)(time / 1000), "", "", "", MogoCountDown.TimeStringType.UpToDayHour,
              () =>
              {
                  ShowVIPBuffBtn(false);
              });
        }
    }

    public void SetNormalMainUIVIPBuffLastTime(UInt32 time, int vipLevel)
    {
        if (time > 0)
        {
            if (m_NormalMainUIVIPBuffCountDown != null)
                m_NormalMainUIVIPBuffCountDown.Release();

            string frontText = string.Format(LanguageData.GetContent(46853), vipLevel);
            m_NormalMainUIVIPBuffCountDown = new MogoCountDown(m_lblNormalMainUIVIPBuffBtnText, (int)(time / 1000), frontText, "", "", MogoCountDown.TimeStringType.UpToDayHour,
              () =>
              {
                  LoggerHelper.Debug("VIPBuff CD is zero");
                  ShowVIPBuffBtn(false);
              });
        }
    }

    #endregion

    #region ���а���Ч

    private GameObject m_goNormalMainUIRankingBtnFxPos;
    private GameObject m_goFx1RankButton;
    private string m_fx1RankButton = "RankButtonFX1";

    /// <summary>
    /// ��ʾ���������а�ť��Ч
    /// </summary>
    private void ShowRankButtonAnimation(bool isShow)
    {
        if (MogoWorld.thePlayer == null)
            return;
        if (MogoWorld.thePlayer.level < SystemRequestLevel.RankingIcon)
            return;

        if (isShow)
        {
            m_goFx1RankButton = MogoFXManager.Instance.FindParticeAnim(m_fx1RankButton);
            if (m_goFx1RankButton != null)
            {
                m_goFx1RankButton.SetActive(true);
            }
            else
            {                
                 AttachRankButtonAnimation();
            }
        }
        else
        {
            m_goFx1RankButton = MogoFXManager.Instance.FindParticeAnim(m_fx1RankButton);
            if (m_goFx1RankButton != null)
            {
                m_goFx1RankButton.SetActive(false);
            }
        }
    }      

    /// <summary>
    /// �����а�ť�ϸ�����Ч
    /// </summary>
    private void AttachRankButtonAnimation()
    {
        if (MogoWorld.thePlayer == null)
            return;
        if (MogoWorld.thePlayer.level < SystemRequestLevel.RankingIcon)
            return;

        m_goFx1RankButton = MogoFXManager.Instance.FindParticeAnim(m_fx1RankButton);
        if (m_goFx1RankButton != null)      
            return;

        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        Vector3 vecParent = m_goNormalMainUIRankingBtnFxPos.transform.position;
        //Debug.LogError(vecParent);
        MogoFXManager.Instance.AttachParticleAnim("fx_ui_baoxiangxingxing.prefab", m_fx1RankButton, vecParent,
            MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
            {
                m_goFx1RankButton = MogoFXManager.Instance.FindParticeAnim(m_fx1RankButton);
                if (m_goFx1RankButton != null)
                {
                    m_goFx1RankButton.SetActive(false);
                  
                    TimerHeap.AddTimer(2000, 0, () =>
                    {
						if(m_goFx1RankButton != null)
						{
	                        Vector3 newVecParent = m_goNormalMainUIRankingBtnFxPos.transform.position;
	                        //Debug.LogError(newVecParent);
	                        MogoFXManager.Instance.TransformToFXCameraPos(m_goFx1RankButton, newVecParent, MogoUIManager.Instance.GetMainUICamera());
	                        if (MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_NormalMainUI)
	                        {
								
	                            	m_goFx1RankButton.SetActive(true);
	                        }
	                        else
	                        {
								
	                            m_goFx1RankButton.SetActive(false);
	                        }
						}
                    });
                }

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
    }
  
    /// <summary>
    /// �ͷ����а�ť��Ч
    /// </summary>
    private void ReleaseRankButtonAnimation()
    {
        if (MogoWorld.thePlayer == null)
            return;
  
        MogoFXManager.Instance.ReleaseParticleAnim(m_fx1RankButton);
    }
  
    /// <summary>
    /// ˢ�����а��VIPλ��
    /// </summary>
    /// <param name="level"></param>
    private void RefreshRankingPos(int level)
    {
        if (level >= SystemRequestLevel.RankingIcon)
        {
            // ���а�
            m_goNormalMainUIRanking.SetActive(true);
            m_goNormalMainUIRankingBtn.transform.localPosition = m_tranNormalMainUIRankingBtnPos.localPosition;
            // VIP
            m_goNormalMainUIPlayerVIP.transform.localPosition = m_tranNormalMainUIPlayerVIPPos2.localPosition;
        }
        else
        {
            // ���а�
            m_goNormalMainUIRanking.SetActive(false);
            // VIP
            m_goNormalMainUIPlayerVIP.transform.localPosition = m_tranNormalMainUIPlayerVIPPos1.localPosition;
        }
    }

    #endregion

    #region ������Ϣ

    /// <summary>
    /// ����ս��ֵ
    /// </summary>
    /// <param name="power"></param>
    public void SetPlayerCurrentPower(uint power)
    {
        if (m_lblUpgradePowerCurrentNum != null)
            m_lblUpgradePowerCurrentNum.text = power.ToString();
    }

    public void SetPlayerBloodText(string text)
    {
        m_lblPlayerBloodText.text = text;
    }

    public void SetContributeTipOKCallBack(Action cb)
    {
        CONTRIBUTETIPOKUP = cb;
    }

    #endregion

    #endregion

    #region ����򿪺͹ر�

    protected override void OnEnable()
    {
        base.OnEnable();

        if (MogoWorld.thePlayer != null)
        {
            SetGoldMetallurgyLastTimes(MogoWorld.thePlayer.CalGoldMetallurgyLastUseTimes());
        }

        if (MainUIViewManager.Instance)
        {
            MainUIViewManager.Instance.ResetUIStates();
        }
       
        ShowRankButtonAnimation(true);   
    }

    void OnDisable()
    {
        //MogoFXManager.Instance.ReleaseAllParticleAnim();
        ShowRankButtonAnimation(false);
    }

    #endregion    
}