using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using TDBID = System.UInt64;

public class FriendMessageGridData
{
    public TDBID id;
    public string headImg;
    public string name;
    public string level;
    public string power;
    public bool isMessage;
    public bool isWishByFriend; // 该好友是否祝福过自己
    public bool isOnline;
    public int degreeNum;
    public bool isWithToFriend; // 自己是否祝福过该好友
};

public class FriendQuestAddGridData
{
    public TDBID id;
    public string headImg;
    public string name;
    public string level;
};

public class MailGridData
{
    public string headImg;
    public string name;
    public string topic;
    public string time;
    public bool isAttachRead;
    public bool isAttachNoRead;
    public bool isNoRead;
    public bool isRead;
    public TDBID mailId;
};

public class MailDetailData
{
    public string title;
    public string time;
    public string reciverName;
    public string senderName;
    public string infoText;
    public List<string> listItemImg;
    public List<int> listItemNum;
    public List<int> listItemID;
    public TDBID mailId;
};

public enum SocietyUITab
{
    FriendTab = 1,
    MailTab = 2,
}

public class SocietyUIViewManager : MogoUIBehaviour
{
    private static SocietyUIViewManager m_instance;
    public static SocietyUIViewManager Instance { get { return SocietyUIViewManager.m_instance; } }
    
    private Dictionary<int, UILabel> m_SocietyTabLabelList = new Dictionary<int, UILabel>(); 

    public Action<TDBID> MAILGRIDUP;
    public Action<TDBID> DELETEMAILGRID;
    public Action<TDBID> DETAILMAILDELETEUP;
    public Action MAILDETAILQUITUP;
    public Action<TDBID> MAILDETAILGETITEMUP;
    public Action<TDBID> MAILFRONTUP;           // 上一封邮件
    public Action<TDBID> MAILNEXTUP;            // 下一封邮件

    public Action ONEKEYGETBLESSUSEOK;

    private Dictionary<TDBID, TDBID> m_mailGridToMailID = new Dictionary<TDBID, TDBID>();    

    private Camera m_camFriendGridList;
    private Camera m_camQuestDialogList;
    private Camera m_camMessageList;
    private Camera m_camMailList;

    private GameObject m_goFriendGridList;
    private GameObject m_goQuestDialogList;
    private GameObject m_goTotalInfoDialog;
    private GameObject m_goMessageDialog;
    private GameObject m_goGlobleDialogBG;
    private GameObject m_goQuestDialog;

    private GameObject m_goFriendGridListTip;
    private UILabel m_lblWishBtnNoWishText;
    private UILabel m_lblWishBtnHasWishText;

    private GameObject m_goAddFriendDialog;
    private GameObject m_goAddFriendInfo;
    private GameObject m_goCantFindFriend;
    private GameObject m_goAddFriendDialogLine;
    private GameObject m_goMessageList;
    private GameObject m_goReturnMessageDialog;
    private GameObject m_goMailList;
    private GameObject m_goMailDetailDialog;
    private GameObject m_goMailGridListDialog;

    private GameObject m_goDeleteReadBtn;
   
    //tips
    private GameObject m_FriendIconTip;
    private GameObject m_MailIconTip;
    //private GameObject m_SocietyTip;
    private GameObject m_FriendReqTip;
    private UILabel m_FriendReqTipNum;

    private UILabel m_lblNoQuestText;
    private UILabel m_lblFriendNum;
    private UIInput m_inputReturnMessage;
    private UILabel m_lblReturnMessageFriendName;
    private UILabel m_lblReciveMessageFriendName;



    //onekeygetbless
    GameObject m_goOneKeyGetBlessUseOKCancel;
    private Transform m_OneKeyGetBlessRewardList;

    #region 邮件详细信息
    bool m_IsShowMailDetailOne = true;

    // 拖动选择上一封/下一封邮件
    private GameObject m_goMailDetailGridList;

    // 仅显示当前邮件详细信息，无法拖动
    private GameObject m_goGOMailDetail;
    private UILabel m_lblMailDetailInfoText;
    private UILabel m_lblMailDetailReciverName;
    private UILabel m_lblMailDetailSenderName;
    private UILabel m_lblMailDetailTimeText;
    private UILabel m_lblMailDetailTitleText;
    private UISprite m_spMailItemGetSign;
    private GameObject m_goOneKeyGetMailItem;
    private UISprite[] m_arrSpMailDetailItem = new UISprite[5];
    private UILabel[] m_arrLblMailDetailItemNum = new UILabel[5];
    private UISprite[] m_arrSpMailDetialItemBG = new UISprite[5];
    private GameObject[] m_arrGoMailDetailItemGetSign = new GameObject[5];
    #endregion

    private UILabel m_lblMailVolumeText;
    private UILabel m_lblNoMailBoxText;
    
    private GameObject m_goMailDetailTriPageLeftBtn;
    private GameObject m_goMailDetailTriPageRightBtn;

    //=>查询回设组件
    private SocietyUIButton m_btnSendQuestBtn;
    private UILabel m_lblAddFriendInputText;
    //AddFriendInfoHeadImg AddFriendInfoLevel AddFriendInfoName AddFriendInfoPower
    private UISprite m_spAddFriendInfoHeadImg;
    private UILabel m_lblAddFriendInfoLevel;
    private UILabel m_lblAddFriendInfoName;
    private UILabel m_lblAddFriendInfoPower;
    //<=

    #region button scripts
    //==>menu 的三个按钮的脚本
    private SocietyUIButton m_btnDeleteBtn;
    private SocietyUIButton m_btnMessageBtn;
    private SocietyUIButton m_btnWishBtn;
    //<==
    //==>send button script
    private SocietyUIButton m_btnSendMessageBtn;
    private SocietyUIButton m_btnReturnMessageBtn;//回复的send
    //<=
    #endregion

    #region 请求和一键领取按钮对象
    private GameObject m_ReqButtun;
    private GameObject m_RecvAllButtun;
    private GameObject m_NoFirendsTip;
    private GameObject m_NoFriendReqTip;
    #endregion

    //留言list top left
    private GameObject m_MessageListTopLfetPoit;   

    private List<GameObject> m_listFriendGrid = new List<GameObject>();
    private List<GameObject> m_listQuestGrid = new List<GameObject>();
    private List<GameObject> m_listMessageLabel = new List<GameObject>();
    private List<GameObject> m_listMailGrid = new List<GameObject>();

    private TDBID m_iCurrentDownFriendGrid = 0;
    private TDBID m_iCurrentDownMailGrid = 0;
    private TDBID m_iCurrentMailDbid = 0;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_camFriendGridList = m_myTransform.Find(m_widgetToFullName["FriendGirdListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camFriendGridList.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        Mogo.Util.LoggerHelper.Debug(m_camFriendGridList.name);

        m_goFriendGridList = m_myTransform.Find(m_widgetToFullName["FriendGridList"]).gameObject;

        m_camQuestDialogList = m_myTransform.Find(m_widgetToFullName["QuestDialogListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camQuestDialogList.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];


        m_camMessageList = m_myTransform.Find(m_widgetToFullName["MessageListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camMessageList.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_camMailList = m_myTransform.Find(m_widgetToFullName["MailGridListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camMailList.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_goQuestDialogList = m_myTransform.Find(m_widgetToFullName["QuestDialogList"]).gameObject;

        m_goTotalInfoDialog = m_myTransform.Find(m_widgetToFullName["TotalInfoDialog"]).gameObject;
        
        m_goGlobleDialogBG = m_myTransform.Find(m_widgetToFullName["FriendDialogGlobleListBG"]).gameObject;
        m_goQuestDialog = m_myTransform.Find(m_widgetToFullName["QuestDialog"]).gameObject;

        m_lblNoQuestText = m_myTransform.Find(m_widgetToFullName["QuestDialogNoQuestText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblFriendNum = FindTransform("FriendUIFriendNum").GetComponentsInChildren<UILabel>(true)[0];
       
        m_goFriendGridListTip = m_myTransform.Find(m_widgetToFullName["FriendGridListTip"]).gameObject;
        m_lblWishBtnNoWishText = FindTransform("WishBtnNoWishText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblWishBtnHasWishText = FindTransform("WishBtnHasWishText").GetComponentsInChildren<UILabel>(true)[0];

        //==>查询
        m_goAddFriendDialog = m_myTransform.Find(m_widgetToFullName["AddFriendDialog"]).gameObject;
        m_goAddFriendInfo = m_myTransform.Find(m_widgetToFullName["AddFriendInfo"]).gameObject;
        m_lblAddFriendInputText = m_myTransform.Find(m_widgetToFullName["AddFriendInputText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_goCantFindFriend = m_myTransform.Find(m_widgetToFullName["CantFindFriend"]).gameObject;
        m_goAddFriendDialogLine = m_myTransform.Find(m_widgetToFullName["AddFriendDialogLine"]).gameObject;

        m_spAddFriendInfoHeadImg = m_myTransform.Find(m_widgetToFullName["AddFriendInfoHeadImg"]).GetComponentsInChildren<UISprite>(true)[0];
        m_lblAddFriendInfoLevel  = m_myTransform.Find(m_widgetToFullName["AddFriendInfoLevel"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblAddFriendInfoName   = m_myTransform.Find(m_widgetToFullName["AddFriendInfoName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblAddFriendInfoPower  = m_myTransform.Find(m_widgetToFullName["AddFriendInfoPower"]).GetComponentsInChildren<UILabel>(true)[0];

        m_btnSendQuestBtn = m_myTransform.Find(m_widgetToFullName["SendQuestBtn"]).GetComponentsInChildren<SocietyUIButton>(true)[0];
        //<==
        //==>message view
        //show
        m_goMessageDialog = m_myTransform.Find(m_widgetToFullName["MessageDialog"]).gameObject;
        m_goMessageList = m_myTransform.Find(m_widgetToFullName["MessageList"]).gameObject;
        m_lblReciveMessageFriendName = m_myTransform.Find(m_widgetToFullName["MessageDialogName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_btnReturnMessageBtn = m_myTransform.Find(m_widgetToFullName["ReturnMessageBtn"]).GetComponentsInChildren<SocietyUIButton>(true)[0];
        //send
        m_inputReturnMessage = m_myTransform.Find(m_widgetToFullName["ReturnMessageDialogInput"]).GetComponentsInChildren<UIInput>(true)[0];
        m_lblReturnMessageFriendName = m_myTransform.Find(m_widgetToFullName["ReturnMessageDialogName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goReturnMessageDialog = m_myTransform.Find(m_widgetToFullName["ReturnMessageDialog"]).gameObject;
        m_btnSendMessageBtn = m_myTransform.Find(m_widgetToFullName["SendMessageBtn"]).GetComponentsInChildren<SocietyUIButton>(true)[0];
        //<==
        //==>menu 的三个按钮的脚本
        m_btnDeleteBtn = m_myTransform.Find(m_widgetToFullName["DeleteBtn"]).GetComponentsInChildren<SocietyUIButton>(true)[0];
        m_btnMessageBtn = m_myTransform.Find(m_widgetToFullName["MessageBtn"]).GetComponentsInChildren<SocietyUIButton>(true)[0];
        m_btnWishBtn = m_myTransform.Find(m_widgetToFullName["WishBtn"]).GetComponentsInChildren<SocietyUIButton>(true)[0];
        //<==
        //tips
        m_FriendIconTip = m_myTransform.Find(m_widgetToFullName["FriendTip"]).gameObject;
        m_MailIconTip = m_myTransform.Find(m_widgetToFullName["MailTip"]).gameObject;
        //m_SocietyTip = m_myTransform.FindChild(m_widgetToFullName["FrientReqTip"]).gameObject;
        m_FriendReqTip = m_myTransform.Find(m_widgetToFullName["FrientReqTip"]).gameObject;
        //private UILabel m_FriendReqTipNum;
        m_FriendReqTipNum = m_myTransform.Find(m_widgetToFullName["FriendReqNumText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_ReqButtun = m_myTransform.Find(m_widgetToFullName["ToFriendAcceptButton"]).gameObject;
        m_RecvAllButtun = m_myTransform.Find(m_widgetToFullName["FriendRecvAllBlessButton"]).gameObject;
        m_NoFirendsTip = m_myTransform.Find(m_widgetToFullName["NoFirendsTip"]).gameObject;
        m_NoFriendReqTip = m_myTransform.Find(m_widgetToFullName["NoQuestInfosText"]).gameObject;

        m_MessageListTopLfetPoit = m_myTransform.Find(m_widgetToFullName["MessageDialogListBGTopLeft"]).gameObject;
        
        //==>mail
        m_goMailList = m_myTransform.Find(m_widgetToFullName["MailGridList"]).gameObject;
        m_goMailDetailDialog = m_myTransform.Find(m_widgetToFullName["MailDetailDialog"]).gameObject;
        m_goMailGridListDialog = m_myTransform.Find(m_widgetToFullName["MailGridListDialog"]).gameObject;

        m_goDeleteReadBtn = m_myTransform.Find(m_widgetToFullName["DeleteReadBtn"]).gameObject;

        // 可拖动邮件
        m_goMailDetailGridList = m_myTransform.Find(m_widgetToFullName["MailDetailGridList"]).gameObject;
        m_goMailDetailGridList.SetActive(!m_IsShowMailDetailOne);

        // 仅显示当前邮件详细信息
        m_goGOMailDetail = m_myTransform.Find(m_widgetToFullName["GOMailDetail"]).gameObject;
        m_lblMailDetailInfoText = m_myTransform.Find(m_widgetToFullName["MailDetailInfoText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailDetailReciverName = m_myTransform.Find(m_widgetToFullName["MailDetailRecvierName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailDetailSenderName = m_myTransform.Find(m_widgetToFullName["MailDetailSenderName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailDetailTimeText = m_myTransform.Find(m_widgetToFullName["MailDetailTimeText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblMailDetailTitleText = m_myTransform.Find(m_widgetToFullName["MailDetailTitleText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_spMailItemGetSign = m_myTransform.Find(m_widgetToFullName["MailDetailItemGetSign"]).GetComponentsInChildren<UISprite>(true)[0];
        m_goOneKeyGetMailItem = m_myTransform.Find(m_widgetToFullName["MailDetailOneKeyGetBtn"]).gameObject;
        for (int i = 0; i < 5; ++i)
        {
            m_arrSpMailDetailItem[i] = m_myTransform.Find(m_widgetToFullName["MailDetailItem" + i + "FG"]).GetComponentsInChildren<UISprite>(true)[0];
            m_arrLblMailDetailItemNum[i] = m_myTransform.Find(m_widgetToFullName["MailDetailItem" + i + "Num"]).GetComponentsInChildren<UILabel>(true)[0];
            m_arrSpMailDetialItemBG[i] = m_myTransform.Find(m_widgetToFullName["MailDetailItem" + i + "BG"]).GetComponentsInChildren<UISprite>(true)[0];
            m_arrGoMailDetailItemGetSign[i] = m_myTransform.Find(m_widgetToFullName["MailDetailItem" + i + "GetSign"]).gameObject;
        }
        m_goGOMailDetail.SetActive(m_IsShowMailDetailOne);


        m_lblMailVolumeText = m_myTransform.Find(m_widgetToFullName["MailBoxVolume"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblNoMailBoxText = m_myTransform.Find(m_widgetToFullName["NoMailBoxText"]).GetComponentsInChildren<UILabel>(true)[0];       

        m_goMailDetailTriPageLeftBtn = m_myTransform.Find(m_widgetToFullName["MailDetailTriPageLeftBtn"]).gameObject;
        m_goMailDetailTriPageRightBtn = m_myTransform.Find(m_widgetToFullName["MailDetailTriPageRightBtn"]).gameObject;      

        m_SocietyTabLabelList[(int)SocietyUITab.FriendTab] = m_myTransform.Find(m_widgetToFullName["FriendIconText"]).GetComponent<UILabel>();
        m_SocietyTabLabelList[(int)SocietyUITab.MailTab] = m_myTransform.Find(m_widgetToFullName["MailIconText"]).GetComponent<UILabel>();
        foreach (var pair in m_SocietyTabLabelList)
        {
            if (pair.Key == (int)SocietyUITab.FriendTab)
                SocietyTabDown(pair.Key);
            else
                SocietyTabUp(pair.Key);
        }

        //onekeygetbless
        m_goOneKeyGetBlessUseOKCancel = m_myTransform.Find(m_widgetToFullName["OneKeyGetBless"]).gameObject;

        m_OneKeyGetBlessRewardList = m_myTransform.Find(m_widgetToFullName["OneKeyGetBlessRewardList"]);
        // 设置SourceCamera
        //Camera sourceCamera = m_myTransform.FindChild("OneKeyBlessRewardListCamera").GetComponentsInChildren<Camera>(true)[0];
        //m_OneKeyGetBlessRewardList.GetComponentsInChildren<MogoListImproved>(true)[0].SourceCamera = sourceCamera;
        //<==mail
        Initialize();

        I18n();
    }

    #region 事件

    

    void Initialize()
    {
        SocietyUILogicManager.Instance.Initialize();

        SocietyUIDict.ButtonTypeToEventUp.Add("FriendRecvAllBlessButton", OnRecvAllBlessBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("ToFriendAcceptButton", OnToFriendAcceptBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("ToFriendAddButton", OnToFriendAddBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("QuestDialogQuit", OnQuestDialogQuitBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("DeleteBtn", OnDeleteBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MessageBtn", OnMessageBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("WishBtn", OnWishBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("FriendGridListTipMaksBG", OnFriendGridListTipMaskUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("FindFriendBtn", OnFindFriendBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("AddFriendDialogQuit", OnAddFriendDialogQuitUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("SendQuestBtn", OnSendQuestBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MessageDialogQuit", OnMessageDialogQuitUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("ReturnMessageBtn", OnReturnMessageBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("ReturnMessageDialogQuit", OnReturnMessageDialogQuitUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("SendMessageBtn", OnSendMessageBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailIcon", OnMailIconUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("FriendIcon", OnFriendIconUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailDeleteBtn", OnMailDetailDeleteBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailOneKeyGetBtn", OnMailDetailOneKeyGetBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailQuitBtn", OnMailDetailQuitBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailItem0", OnMailDetailItemUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailItem1", OnMailDetailItemUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailItem2", OnMailDetailItemUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailItem3", OnMailDetailItemUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailItem4", OnMailDetailItemUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("DeleteReadBtn", OnMailDeleteReadBtnUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailTriPageLeftBtn", OnMailFrontUp);
        SocietyUIDict.ButtonTypeToEventUp.Add("MailDetailTriPageRightBtn", OnMailNextUp);

        SocietyUIDict.ButtonTypeToEventUp.Add("OneKeyGetBlessUseOKBtn", OnOneKeyGetBlessUseOKBtn);//kevin


        EventDispatcher.AddEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.FRIENDMESSAGEGRIDUP, OnFriendMessageGridUp);
        EventDispatcher.AddEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.LEAVEMESSAGESIGNUP, OnLeaveMessageSignUp);
        EventDispatcher.AddEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.WISHSTRENTHSIGNUP, OnWishStrenthSignUp);
        EventDispatcher.AddEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.ACCEPTADDFRIENDUP, OnAcceptAddFriendUp);
        EventDispatcher.AddEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.REGECTADDFRIENDUP, OnRegectAddFriendUp);
        EventDispatcher.AddEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.MAILGRIDUP, OnMailGridUp);
        m_RecvAllButtun.SetActive(false);
        m_ReqButtun.SetActive(false);
    }

    public void Release()
    {
        SocietyUILogicManager.Instance.Release();

        EventDispatcher.RemoveEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.FRIENDMESSAGEGRIDUP, OnFriendMessageGridUp);
        EventDispatcher.RemoveEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.LEAVEMESSAGESIGNUP, OnLeaveMessageSignUp);
        EventDispatcher.RemoveEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.WISHSTRENTHSIGNUP, OnWishStrenthSignUp);
        EventDispatcher.RemoveEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.ACCEPTADDFRIENDUP, OnAcceptAddFriendUp);
        EventDispatcher.RemoveEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.REGECTADDFRIENDUP, OnRegectAddFriendUp);
        EventDispatcher.RemoveEventListener<TDBID>(SocietyUILogicManager.SocietyUIEvent.MAILGRIDUP, OnMailGridUp);

        ClearFriendGridList();
        ClearQuestDialogList();
        ClearMessageDialogList();
        ClearMailGridList();
        ClearReturnMessageText();

        m_iCurrentDownMailGrid = 0;
        m_iCurrentDownFriendGrid = 0;

        SocietyUIDict.ButtonTypeToEventUp.Clear();
    }

    /// <summary>
    /// ChineseData
    /// </summary>
    private void I18n()
    {
        m_myTransform.Find(m_widgetToFullName["FriendIconText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.TITLE);
        //添加按钮
        m_myTransform.Find(m_widgetToFullName["ToFriendAddButtonText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.BTN_ADD); 
        //请求按钮
        m_myTransform.Find(m_widgetToFullName["ToFriendAcceptButtonText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.BTN_REQ);
        //一键领取按钮
        m_myTransform.Find(m_widgetToFullName["FriendRecvAllBlessButtonText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.BTN_RECV_ALL); 
        //祝福按钮
        m_lblWishBtnNoWishText.text = LanguageData.GetContent(ContentDefine.Friend.WISH_BUTTON);
        m_lblWishBtnHasWishText.text = LanguageData.GetContent(ContentDefine.Friend.WISH_BUTTON_WISHED);
        //留言按钮
        m_myTransform.Find(m_widgetToFullName["MessageBtnText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.MESSAGE_BUTTON); 
        //删除按钮
        m_myTransform.Find(m_widgetToFullName["DeleteBtnText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.DEL_BUTTON); 
        //回赠按钮
        //m_myTransform.FindChild(m_widgetToFullName["FriendRecvAllBlessButtonText"]).GetComponentsInChildren<UILabel>(true)[0].text
        //    = LanguageData.dataMap[ContentDefine.Friend.RETURN_BUTTON].Format();
        //发送请求按钮
        m_myTransform.Find(m_widgetToFullName["SendQuestBtnText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.SEND_BUTTON); 
        //查找按钮
        m_myTransform.Find(m_widgetToFullName["FindFriendBtnText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.BTN_FIND); 
        //NAME TITLE
        m_myTransform.Find(m_widgetToFullName["QuestDialogName"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.NAME_TITLE); 
        //ADD_TITLE
        m_myTransform.Find(m_widgetToFullName["AddFriendText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.ADD_TITLE); 
        //CantFindFriendText
        m_myTransform.Find(m_widgetToFullName["CantFindFriendText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.NOT_EXISTS_TIP); 
        //ReturnMessageBtnText
        m_myTransform.Find(m_widgetToFullName["ReturnMessageBtnText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.BTN_RETURN);

        m_NoFirendsTip.transform.GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.NO_FRIENDS);

        //NO_REQ
        m_NoFriendReqTip.transform.GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Friend.NO_REQ); //dataMap[ContentDefine.Friend.NO_REQ].Format();

        m_myTransform.Find(m_widgetToFullName["MailIconText"]).GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(ContentDefine.Mail.TITLE); //dataMap[ContentDefine.Mail.TITLE].Format();

    }

    void ReleasePreLoadResources()
    {
        AssetCacheMgr.ReleaseResourceImmediate("FriendItem.prefab");
        AssetCacheMgr.ReleaseResourceImmediate("FriendAcceptItem.prefab");
        AssetCacheMgr.ReleaseResourceImmediate("SoceityUIMessageGrid.prefab");
        AssetCacheMgr.ReleaseResourceImmediate("MailGrid.prefab");
    }

    void OnRecvAllBlessBtnUp(TDBID i)
    {
        LoggerHelper.Debug("RecvAllBless OnRecvAllBlessBtnUp");
        //ClearFriendGridList();
        //ShowNoQuestText(true);
        EventDispatcher.TriggerEvent(FriendManager.ON_FRIEND_BLESS_RECV_ALL);
    }

    void OnToFriendAcceptBtnUp(TDBID i)
    {
        LoggerHelper.Debug("Accept");
        ShowGlobleDialogBG(true);
        ShowQuestDialog(true);
        ShowNoQuestText(false);
    }

    void OnToFriendAddBtnUp(TDBID i)
    {
        LoggerHelper.Debug("Add");
        ShowAddFriendDialog(true);
        ShowGlobleDialogBG(true);
        ShowAddFriendDialogLine(false);
        ShowCantFindFriend(false);
        ShowAddFriendInfo(false);
        ShowNoQuestText(false);

    }

    void OnQuestDialogQuitBtnUp(TDBID i)
    {
        LoggerHelper.Debug("Quit");

        ShowGlobleDialogBG(false);
        ShowQuestDialog(false);
    }

    void OnDeleteBtnUp(TDBID i)
    {
        LoggerHelper.Debug("Delete");
        ShowFriendGridListTip(false);
        string msg = LanguageData.GetContent(ContentDefine.Friend.DEL_CONFIRM, FriendManager.Instance.GetFriendInfo(i).name);
        //= LanguageData.dataMap[ContentDefine.Friend.DEL_CONFIRM].Format(FriendManager.Instance.GetFriendInfo(i).name);

        MogoGlobleUIManager.Instance.Confirm(msg, (rst) =>
        {
            if (!rst)
            {
                EventDispatcher.TriggerEvent<TDBID>(FriendManager.ON_FRIEND_DEL, i);
                MogoGlobleUIManager.Instance.ConfirmHide();
            }
            else
            {
                MogoGlobleUIManager.Instance.ConfirmHide();
            }
        }, LanguageData.GetContent(25562), LanguageData.GetContent(25568), -1, ButtonBgType.Blue, ButtonBgType.Brown);
    }

    void OnMessageBtnUp(TDBID i)
    {
        LoggerHelper.Debug("Message");
        //发送留言
        m_btnSendMessageBtn.id = i;
        string name = FriendManager.Instance.GetFriendInfo(i).name;
        SetReturnMessageFriendName(name);
        ShowFriendGridListTip(false);
        ShowReturnMessageDialog(true);
        ShowTotleInfoDialog(false);
    }

    void OnWishBtnUp(TDBID i)
    {
        LoggerHelper.Debug("Wish");

        ShowFriendGridListTip(false);
        EventDispatcher.TriggerEvent<TDBID>(FriendManager.ON_FRIEND_BLESS, i);
    }

    void OnFriendGridListTipMaskUp(TDBID i)
    {
        ShowFriendGridListTip(false);
    }

    void OnFindFriendBtnUp(TDBID i)
    {
        LoggerHelper.Debug("FindFriend");
        EventDispatcher.TriggerEvent<string>(FriendManager.ON_FRIEND_SEARCH, m_lblAddFriendInputText.text);
    }

    void OnAddFriendDialogQuitUp(TDBID i)
    {
        LoggerHelper.Debug("Quit");
        ShowAddFriendDialog(false);
        ShowGlobleDialogBG(false);
    }

    void OnSendQuestBtnUp(TDBID i)
    {
        LoggerHelper.Debug("SendQuest");
        EventDispatcher.TriggerEvent<TDBID>(FriendManager.ON_FRIEND_ADD, i);
    }
    //查看留言返回
    void OnMessageDialogQuitUp(TDBID i)
    {
        LoggerHelper.Debug("OnMessageDialogQuitUp Quit");
        ClearMessageDialogList();
        ShowMessageDialog(false);

        //ShowFriendGridListTip(false);
        //ShowReturnMessageDialog(false);
        ShowTotleInfoDialog(true);
        //Tips();
    }
    //查看留言回复
    void OnReturnMessageBtnUp(TDBID i)
    {
        LoggerHelper.Debug("OnReturnMessageBtnUp Return");
        m_btnSendMessageBtn.id = i;
        string name = FriendManager.Instance.GetFriendInfo(i).name;
        SetReturnMessageFriendName(name);
        ShowMessageDialog(false);
        ShowReturnMessageDialog(true);
        //Tips();
    }
    //写留言返回按钮事件
    public void OnReturnMessageDialogQuitUp(TDBID i)
    {
        LoggerHelper.Debug("OnReturnMessageDialogQuitUp Quit");
        ShowReturnMessageDialog(false);
        ShowMessageDialog(false);

        //ShowFriendGridListTip(true);
        //ShowReturnMessageDialog(false);
        ShowTotleInfoDialog(true);
        //Tips();
    }

    void OnSendMessageBtnUp(TDBID i)
    {
        LoggerHelper.Debug("Send " + m_inputReturnMessage.text);
        EventDispatcher.TriggerEvent<TDBID, string>(FriendManager.ON_FRIEND_NOTESEND, i, m_inputReturnMessage.text);
        ClearReturnMessageText();
    }

    void OnFriendIconUp(TDBID i)
    {
        m_myTransform.Find("SocialSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);
        HandleSocietyTabChange((int)SocietyUITab.MailTab, (int)SocietyUITab.FriendTab);
    }

    void OnMailIconUp(TDBID i)
    {
        ChangeToMailTab();
    }

    public void ChangeToMailTab()
    {
        m_myTransform.Find("SocialSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);
        HandleSocietyTabChange((int)SocietyUITab.FriendTab, (int)SocietyUITab.MailTab);
    }

    void OnMailDetailDeleteBtnUp(TDBID i)
    {
        LoggerHelper.Debug("Delete");

        //ShowMailGridListDialog(true);
        //ShowMailDetailDialog(false);

        if (DETAILMAILDELETEUP != null)
        {
            Mogo.Util.LoggerHelper.Debug(m_iCurrentMailDbid);
            DETAILMAILDELETEUP(m_iCurrentMailDbid);
            m_iCurrentMailDbid = 0;
        }
    }

    void OnMailDetailOneKeyGetBtnUp(TDBID i)
    {
        LoggerHelper.Debug("OneKeyGet");

        if (MAILDETAILGETITEMUP != null && m_iCurrentMailDbid != 0)
        {
            MAILDETAILGETITEMUP(m_iCurrentMailDbid);
        }
    }

    void OnMailDetailQuitBtnUp(TDBID i)
    {
        LoggerHelper.Debug("Quit");

        ShowMailGridListDialog(true);
        ShowMailDetailDialog(false);
        if (MAILDETAILQUITUP != null)
        {
            MAILDETAILQUITUP();
        }
    }

    void OnMailDetailItemUp(TDBID id)
    {
        LoggerHelper.Debug(id);
    }

    void OnMailDeleteReadBtnUp(TDBID i)
    {
        //LoggerHelper.Debug("Delete " + i + " " + m_mailGridToMailID[m_iCurrentDownMailGrid]);
        MailManager.Instance.MailDelAllReq();
    }

    void OnFriendMessageGridUp(TDBID id)
    {
        LoggerHelper.Debug(id);
        //no use
        m_iCurrentDownFriendGrid = id;
        //设置menu的id
        m_btnDeleteBtn.id = id;
        m_btnMessageBtn.id = id;
        m_btnWishBtn.id = id;

        FriendMessageGridData gridData = FriendManager.Instance.GetFriendInfo(id);
        if(gridData != null)
            ShowFriendGridListTip(true, gridData.isWithToFriend);
    }

    void OnLeaveMessageSignUp(TDBID id)
    {
        LoggerHelper.Debug(id);
        m_btnReturnMessageBtn.id = id;
        ShowMessageDialog(true);
        ShowTotleInfoDialog(false);
        EventDispatcher.TriggerEvent<TDBID>(FriendManager.ON_FRIEND_NOTEREAD, id);
    }

    void OnWishStrenthSignUp(TDBID id)
    {
        LoggerHelper.Debug(id);
        EventDispatcher.TriggerEvent<TDBID>(FriendManager.ON_FRIEND_BLESS_RECV, id);
    }

    void OnAcceptAddFriendUp(TDBID id)
    {
        LoggerHelper.Debug(id);
        EventDispatcher.TriggerEvent<TDBID>(FriendManager.ON_FRIEND_ACCETPREQ, id);
    }

    void OnRegectAddFriendUp(TDBID id)
    {
        LoggerHelper.Debug(id);
        EventDispatcher.TriggerEvent<TDBID>(FriendManager.ON_FRIEND_REJECTREQ, id);
    }

    void OnMailGridUp(TDBID id)
    {
        LoggerHelper.Debug(id);
        m_iCurrentDownMailGrid = Convert.ToUInt64(id);
        m_iCurrentMailDbid = m_mailGridToMailID[m_iCurrentDownMailGrid];
        if (MAILGRIDUP != null)
            MAILGRIDUP(m_mailGridToMailID[m_iCurrentDownMailGrid]);
    }

    void OnMailFrontUp(TDBID id)
    {
        LoggerHelper.Debug(id);
        if (m_iCurrentDownMailGrid > 0)
        {
            m_iCurrentDownMailGrid = m_iCurrentDownMailGrid - 1;
            MAILFRONTUP(m_mailGridToMailID[m_iCurrentDownMailGrid]);
        }
    }

    void OnMailNextUp(TDBID id)
    {
        LoggerHelper.Debug(id);
        TDBID maxNum = Convert.ToUInt64(m_mailGridToMailID.Count);
        if (m_iCurrentDownMailGrid < maxNum - 1)
        {
            m_iCurrentDownMailGrid = m_iCurrentDownMailGrid + 1;
            MAILNEXTUP(m_mailGridToMailID[m_iCurrentDownMailGrid]);
        }
    }


    public void AddOneKeyGetBlessListUnit(List<TDBID> blessFriendList = null, Action callback = null)
    {
        ShowOneKeyGetBlessUseOKCancel(true);

        //测试用FriendManager.m_friendList其实要用blessFriendList
        m_OneKeyGetBlessRewardList.GetComponent<MogoListImproved>().SetGridLayout<OneKeyGetBlessGrid>(blessFriendList.Count, m_OneKeyGetBlessRewardList.transform,
            () =>
            {
                int iter = 0;
                if (m_OneKeyGetBlessRewardList.GetComponent<MogoListImproved>().DataList.Count > 0)
                {
                    foreach (TDBID id in blessFriendList)
                    {
                        FriendMessageGridData fd = FriendManager.Instance.GetFriendInfo(id);

                        //LoggerHelper.Error("AddOneKeyGetBlessListUnit:" + fd.name);
                        OneKeyGetBlessGrid unit = (OneKeyGetBlessGrid)(m_OneKeyGetBlessRewardList.GetComponent<MogoListImproved>().DataList[iter]);
                        unit.Name = "获得[13FFD5]" + fd.name + "[-]的祝福,体力+3";

                        iter++;
                    }
                }
            });
    }

    void OnOneKeyGetBlessUseOKBtn(TDBID id)
    {
        LoggerHelper.Debug(id);
        if (ONEKEYGETBLESSUSEOK != null)
            ONEKEYGETBLESSUSEOK();
    }  

    #endregion      

    #region 界面信息

    #region 好友

    public void ShowQuestDialog(bool isShow)
    {
        m_goQuestDialog.SetActive(isShow);
        if(isShow)
            RefreshFriendQuestList();
    }

    public void ShowTotleInfoDialog(bool isShow)
    {
        m_goTotalInfoDialog.SetActive(isShow);
    }

    public void ShowGlobleDialogBG(bool isShow)
    {
        m_goGlobleDialogBG.SetActive(isShow);
    }

    public void ShowNoQuestText(bool isShow)
    {
        m_lblNoQuestText.gameObject.SetActive(isShow);
    }

    public void ShowFriendGridListTip(bool isShow, bool isWish = false)
    {
        m_goFriendGridListTip.SetActive(isShow);

        if (isWish)
        {
            m_lblWishBtnHasWishText.gameObject.SetActive(true);
            m_lblWishBtnNoWishText.gameObject.SetActive(false);
        }
        else
        {
            m_lblWishBtnHasWishText.gameObject.SetActive(false);
            m_lblWishBtnNoWishText.gameObject.SetActive(true);
        }
    }

    public void ShowAddFriendDialog(bool isShow)
    {
        m_goAddFriendDialog.SetActive(isShow);
    }

    public void ShowAddFriendInfo(bool isShow)
    {
        m_goAddFriendInfo.SetActive(isShow);
    }

    public void ShowCantFindFriend(bool isShow)
    {
        m_goCantFindFriend.SetActive(isShow);
    }

    public void ShowAddFriendDialogLine(bool isShow)
    {
        m_goAddFriendDialogLine.SetActive(isShow);
    }

    public void ShowMessageDialog(bool isShow)
    {
        m_goMessageDialog.SetActive(isShow);
    }

    public void ShowReturnMessageDialog(bool isShow)
    {
        m_goReturnMessageDialog.SetActive(isShow);
    }

    public void SetReturnMessageFriendName(string name)
    {
        m_lblReturnMessageFriendName.text = name;
    }

    public void SetReciveMessageFriendName(string name)
    {
        m_lblReciveMessageFriendName.text = name;
    }

    public void FriendResearchResp(FriendData fd, int msg)
    {
        if (FriendManager.ERROR_FRIEND_SUCCESS != msg)
        {
            m_goCantFindFriend.SetActive(true);
            m_goAddFriendDialogLine.SetActive(true);
            m_goAddFriendInfo.SetActive(false);
            m_btnSendQuestBtn.id = 0;
        }
        else
        {
            m_goCantFindFriend.SetActive(false);
            m_goAddFriendDialogLine.SetActive(false);
            //m_spAddFriendInfoHeadImg
            //m_lblAddFriendInfoLevel.text = LanguageData.dataMap[ContentDefine.Friend.LV].Format(fd.level);
            m_lblAddFriendInfoLevel.text = LanguageData.GetContent(ContentDefine.Friend.LV, fd.level);
            m_lblAddFriendInfoName.text = fd.name;
            //m_lblAddFriendInfoPower.text = LanguageData.dataMap[ContentDefine.Friend.POWER].Format(fd.fight);
            m_lblAddFriendInfoPower.text = LanguageData.GetContent(ContentDefine.Friend.POWER, fd.fight);
            m_goAddFriendInfo.SetActive(true);
            m_btnSendQuestBtn.id = fd.id;
        }
        UISprite s = m_myTransform.Find(m_widgetToFullName["AddFriendInfoHeadImg"]).GetComponentsInChildren<UISprite>(true)[0];
        if (s != null)
        {
            string imageName = "";
            if (fd.vocation == 1)
            {
                imageName = "zhanshi";
            }
            else if (fd.vocation == 2)
            {
                imageName = "cike";
            }
            else if (fd.vocation == 3)
            {
                imageName = "gongjianshou";
            }
            else if (fd.vocation == 4)
            {
                imageName = "fashi";
            }
            else
            {

            }
            s.spriteName = imageName;
        }
    }

    public void ShowMailDetailDialog(bool isShow)
    {
        m_goMailDetailDialog.SetActive(isShow);
    }

    public void ShowMailGridListDialog(bool isShow)
    {
        m_goMailGridListDialog.SetActive(isShow);
    }

    public void RefreshFriendList()
    {
        Tips();
        if (!FriendManager.Instance.IsfriendListDirty || !m_goFriendGridList.activeSelf)
            return;
        ClearFriendGridList();
        m_RecvAllButtun.SetActive(false);
        m_NoFirendsTip.SetActive(true);
        
        List<FriendMessageGridData> friendList = FriendManager.Instance.GetFriendList();
        Comparison<FriendMessageGridData> com = new Comparison<FriendMessageGridData>(
               (a, b) =>
               {
                   if (a.isMessage && !(b.isMessage))
                   {
                       return -1;
                   }
                   else if (!(a.isMessage) && b.isMessage)
                   {
                       return 1;
                   }
                   else if (a.isWishByFriend && !(b.isWishByFriend))
                   {
                       return -1;
                   }
                   else if (!(a.isWishByFriend) && b.isWishByFriend)
                   {
                       return 1;
                   }
                   else
                   {
                       int bFight = Convert.ToInt32(b.power);
                       int aFight = Convert.ToInt32(a.power);
                       return bFight - aFight;
                   }
               }
               );
        friendList.Sort(com);

        foreach (FriendMessageGridData fd in friendList)
        {
            if (m_NoFirendsTip.activeSelf)
            {
                m_NoFirendsTip.SetActive(false);
            }

            AddFriendGrid(fd);
        }

        ShowFriendNum(true, friendList.Count);
        FriendManager.Instance.IsfriendListDirty = false;
    }

    public void RefreshFriendQuestList()
    {
        Tips();
        if (!FriendManager.Instance.IsAccepteFriendListDirty || !m_goQuestDialog.activeSelf)
            return;
        ClearQuestDialogList();
        foreach (FriendQuestAddGridData fd in FriendManager.Instance.GetAcceptFriendList())
        {
            AddQuestGrid(fd);
        }
        FriendManager.Instance.IsAccepteFriendListDirty = false;
    }

    public void RefreshFriendMessageList(string name, List<FriendNoteData> args)
    {
        Tips();
        SetReciveMessageFriendName(name);
        ClearMessageDialogList();
        float posx = m_MessageListTopLfetPoit.transform.localPosition.x;
        foreach (FriendNoteData fd in args)
        {
            //string timeString = fd.time.GetTime().FormatTime();
            AddMessageDialogTime(fd.time, 0);
            AddMessageDialogLabel(fd.content, posx);
        }
    }

    public void Tips()
    {
        bool IsFriendTipShow = false;
        bool IsRecvAllBtnShow = false;
        foreach (FriendMessageGridData fd in FriendManager.Instance.GetFriendList())
        {
            if (fd.isMessage)
            {
                IsFriendTipShow = true;
            }
            if (fd.isWishByFriend)
            {
                IsFriendTipShow = true;
                IsRecvAllBtnShow = true;
            }
            if (IsRecvAllBtnShow && IsFriendTipShow)
            {
                break;
            }
        }
        m_RecvAllButtun.SetActive(IsRecvAllBtnShow);

        int count = FriendManager.Instance.GetAcceptFriendList().Count;
        if (count > 0)
        {
            m_FriendReqTip.SetActive(true);
            m_FriendReqTipNum.text = Convert.ToString(count);
            m_ReqButtun.SetActive(true);
            IsFriendTipShow = true;
            m_NoFriendReqTip.SetActive(false);
        }
        else
        {
            m_FriendReqTip.SetActive(false);
            m_ReqButtun.SetActive(false);
            m_NoFriendReqTip.SetActive(true);
        }
        m_FriendIconTip.SetActive(IsFriendTipShow);

        bool IsMailTipShow = false;
        foreach (MailInfo mf in MailManager.Instance.GetMailInfoList())
        {
            if (mf.state < MAIL_STATE.NO_ATTACH_READ)
            {
                IsMailTipShow = true;
                break;
            }
        }
        ShowMailTip(IsMailTipShow);
        //好友或者邮件信息的有提示
        if (NormalMainUIViewManager.Instance != null)
        {
            NormalMainUIViewManager.Instance.ShowSocialTip(IsFriendTipShow || IsMailTipShow);
        }
        if (MenuUIViewManager.Instance != null)
        {
            MenuUIViewManager.Instance.ShowSocialTip(IsFriendTipShow || IsMailTipShow);
        }
    }

    /// <summary>
    /// 是否显示好友数量
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="friendNum"></param>
    readonly static int MAX_FRIEND_COUNT = 50;
    public void ShowFriendNum(bool isShow, int friendNum = 0)
    {
        m_lblFriendNum.gameObject.SetActive(isShow);
        m_lblFriendNum.text = string.Concat(friendNum, "/", MAX_FRIEND_COUNT);
        if (friendNum <= 0)
            m_lblFriendNum.gameObject.SetActive(false);
    }

    /// <summary>
    /// 设置是否显示使用确定框
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowOneKeyGetBlessUseOKCancel(bool isShow)
    {
        if (m_goOneKeyGetBlessUseOKCancel != null)
        {
            m_goOneKeyGetBlessUseOKCancel.SetActive(isShow);
        }
    }

    #endregion  

    #region 邮件

    public void ShowMailTip(bool bShow)
    {
        m_MailIconTip.SetActive(bShow);
    }

    public void ShowDeleteReadBtn(bool isShow)
    {
        m_goDeleteReadBtn.SetActive(isShow);
    }

    /// <summary>
    /// 是否显示邮件数量
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowMailVolume(bool isShow)
    {
        m_lblMailVolumeText.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// 设置邮件数量
    /// </summary>
    /// <param name="volume"></param>
    public void SetMailVolume(string volume)
    {
        m_lblMailVolumeText.text = volume;
    }

    public void ShowNoMailBoxText(bool isShow)
    {
        m_lblNoMailBoxText.gameObject.SetActive(isShow);
    }

    public void PlayMailItemGetSignAnim()
    {
        //TweenScale m_ts = m_spMailItemGetSign.GetComponentsInChildren<TweenScale>(true)[0];
        //m_spMailItemGetSign.gameObject.SetActive(true);
        //m_ts.enabled = true;
        //m_ts.Reset();
        //m_ts.from = new Vector3(192, 78, 1);
        //m_ts.to = new Vector3(96, 39, 1);
        //m_ts.Play(true);

        for (int i = 0; i < 5; i++)
        {
            if (i < m_itemCount)
            {
                TweenScale ts = m_arrGoMailDetailItemGetSign[i].GetComponentsInChildren<TweenScale>(true)[0];
                ts.gameObject.SetActive(true);
                ts.enabled = true;
                ts.Reset();
                ts.from = new Vector3(192, 78, 1);
                ts.to = new Vector3(96, 39, 1);
                ts.Play(true);
            }
            else
            {
                m_arrGoMailDetailItemGetSign[i].SetActive(false);
            }
        }
    }

    public void ShowMailItemGetSign(bool isShow = true)
    {
        //m_spMailItemGetSign.gameObject.SetActive(isShow);
        //m_spMailItemGetSign.transform.localScale = new Vector3(96, 39, 1);

        for (int i = 0; i < 5; i++)
        {
            if (i < m_itemCount)
            {
                m_arrGoMailDetailItemGetSign[i].SetActive(isShow);
                m_arrGoMailDetailItemGetSign[i].transform.localScale = new Vector3(96, 39, 1);
            }
            else
            {
                m_arrGoMailDetailItemGetSign[i].SetActive(false);
            }
        }
    }

    public void ShowOneKeyGetItemBtn(bool isShow)
    {
        m_goOneKeyGetMailItem.SetActive(isShow);
    }

    public void ResetDefaultSocietyPage()
    {
        m_myTransform.Find(m_widgetToFullName["FriendIcon"]).GetComponentsInChildren<MogoFakeClick>(true)[0].FakeIt();
    }

    #endregion

    #endregion

    #region 邮件详细信息
    MailDetailData m_MailDetailData;
    int m_itemCount = 0;
    public void FillMailDetailData(MailDetailData md)
    {
		if(m_IsShowMailDetailOne == false)
		{
            m_MailDetailData = md;
			GenerateMailDetailGrid();
            FillMailDetailGirdData(md);
		}
		
        m_lblMailDetailInfoText.text = md.infoText;
        m_lblMailDetailReciverName.text = md.reciverName;
        m_lblMailDetailSenderName.text = md.senderName;
        m_lblMailDetailTimeText.text = Utils.GetTime(int.Parse(md.time)).ToString("yyyy-MM-dd");
        m_lblMailDetailTitleText.text = md.title;

        m_itemCount = md.listItemImg.Count;
        for (int i = 0; i < 5; ++i)
        {
            if (i < m_itemCount)
            {
                m_arrSpMailDetailItem[i].atlas = MogoUIManager.Instance.GetAtlasByIconName(md.listItemImg[i]);
                //m_arrSpMailDetailItem[i].spriteName = md.listItemImg[i];

                InventoryManager.SetIcon(md.listItemID[i], m_arrSpMailDetailItem[i], 0, null, m_arrSpMailDetialItemBG[i]);

                if (md.listItemNum[i] == 1)
                {
                    m_arrLblMailDetailItemNum[i].gameObject.SetActive(false);
                }
                else
                {
                    m_arrLblMailDetailItemNum[i].gameObject.SetActive(true);
                }

                m_arrLblMailDetailItemNum[i].text = md.listItemNum[i].ToString();
                m_arrSpMailDetailItem[i].transform.parent.gameObject.SetActive(true);
            }
            else
            {
                m_arrSpMailDetailItem[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
    
    /// <summary>
    /// 创建邮件详细信息列表
    /// </summary>
    /// <param name="data"></param>
    bool m_IsMailDetailGridGenerated = false;
    public void GenerateMailDetailGrid()
    {
        if (m_IsMailDetailGridGenerated == false)
        {
            m_IsMailDetailGridGenerated = true;
            m_goMailDetailGridList.GetComponentsInChildren<MogoListImproved>(true)[0].SetGridLayout<MailDetailGrid>(5, m_goMailDetailGridList.transform, MailDetailGridLoaded);
        }       
    }

    /// <summary>
    /// 初始化邮件详细信息列表
    /// </summary>
    void MailDetailGridLoaded()
    {
        // 一共创建5封邮件，当前邮件显示在第三个位置
        var m_DataList = m_goMailDetailGridList.GetComponentsInChildren<MogoListImproved>(true)[0].DataList;
        for (int i = 0; i < m_DataList.Count; i++)
        {
            MailDetailGrid mailDetailGrid = (MailDetailGrid)m_DataList[i];
            mailDetailGrid.ClearMailDetailData();
        }

        MailDetailGrid curMailDetailGrid = (MailDetailGrid)m_DataList[2];
        curMailDetailGrid.FillMailDetailData(m_MailDetailData);

        m_goMailDetailGridList.gameObject.SetActive(true);
        m_goMailDetailGridList.GetComponentsInChildren<MogoListImproved>(true)[0].TweenTo(2, true); 
    }

     /// <summary>
    /// 设置邮件详细信息列表中邮件
    /// </summary>
    public void FillMailDetailGirdData(MailDetailData md)
    {
        var m_DataList = m_goMailDetailGridList.GetComponentsInChildren<MogoListImproved>(true)[0].DataList;
        for (int i = 0; i < m_DataList.Count; i++)
        {
            MailDetailGrid mailDetailGrid = (MailDetailGrid)m_DataList[i];
            if (mailDetailGrid.m_MailId == md.mailId)
                mailDetailGrid.FillMailDetailData(md);
        }
    }
		

    #endregion    

    #region Tab Change
    public void HandleSocietyTabChange(int fromTab, int toTab)
    {
        SocietyTabUp(fromTab);
        SocietyTabDown(toTab);
    }

    void SocietyTabUp(int tab)
    {
        if (m_SocietyTabLabelList.ContainsKey(tab))
        {
            UILabel fromTabLabel = m_SocietyTabLabelList[tab];
            if (fromTabLabel != null)
            {
                fromTabLabel.color = new Color32(37, 29, 6, 255);
                fromTabLabel.effectStyle = UILabel.Effect.None;
            }
        }
    }

    void SocietyTabDown(int tab)
    {
        if (m_SocietyTabLabelList.ContainsKey(tab))
        {
            UILabel toTabLabel = m_SocietyTabLabelList[tab];
            if (toTabLabel != null)
            {
                toTabLabel.color = new Color32(255, 255, 255, 255);
                toTabLabel.effectStyle = UILabel.Effect.Outline;
                toTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }
    }
    #endregion

    #region 创建好友Grid

    public void AddFriendGrid(FriendMessageGridData fd)
    {
        AssetCacheMgr.GetUIInstance("FriendItem.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camFriendGridList;

            obj.transform.parent = m_goFriendGridList.transform;

            obj.transform.localPosition = new Vector3(0, -m_listFriendGrid.Count * 105, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            //FriendMessageGrid fg = obj.GetComponentsInChildren<FriendMessageGrid>(true)[0];

            FriendMessageGrid fg = obj.AddComponent<FriendMessageGrid>();

            fg.Id = fd.id;
            fg.WishStrenth = fd.isWishByFriend;
            if (fd.isWishByFriend && !m_RecvAllButtun.activeSelf)
            {
                m_RecvAllButtun.SetActive(true);
            }
            fg.LevelMessage = fd.isMessage;
            fg.HasWish = fd.isWithToFriend;
            fg.DegreeNum = fd.degreeNum;
            //fg.FightingPower = LanguageData.dataMap[ContentDefine.Friend.POWER].Format(fd.power);
            fg.FightingPower = LanguageData.GetContent(ContentDefine.Friend.POWER, fd.power);
            fg.FriendHeadImg = fd.headImg;
            //fg.FriendLevel = LanguageData.dataMap[ContentDefine.Friend.LV].Format(fd.level);
            fg.FriendLevel = LanguageData.GetContent(ContentDefine.Friend.LV, fd.level);
            fg.FriendName = fd.name;
            if (fd.isOnline)
            {
                //todo:在线显示
            }
            else
            {
                //todo:离线提示
            }
            m_listFriendGrid.Add(obj);

            obj.GetComponentsInChildren<MogoSingleButton>(true)[0].ButtonListTransform = m_goFriendGridList.transform;

            m_goFriendGridList.transform.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);

            //obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camFriendGridList;

            if (m_listFriendGrid.Count >= 4)
            {
                m_camFriendGridList.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -158 - 105 * (m_listFriendGrid.Count - 4);
            }
            else
            {
                m_camFriendGridList.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -158;
            }
        });
    }

    public void AddQuestGrid(FriendQuestAddGridData fd)
    {
        AssetCacheMgr.GetUIInstance("FriendAcceptItem.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camQuestDialogList;

            obj.transform.parent = m_goQuestDialogList.transform;

            obj.transform.localPosition = new Vector3(0, -m_listQuestGrid.Count * 108, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            //FriendQuestAddGrid fg = obj.GetComponentsInChildren<FriendQuestAddGrid>(true)[0];
            FriendQuestAddGrid fg = obj.AddComponent<FriendQuestAddGrid>();

            fg.Id = fd.id;
            fg.FriendHeadImg = fd.headImg;
            //fg.FriendLevel = LanguageData.dataMap[ContentDefine.Friend.LV].Format(fd.level);
            fg.FriendLevel = LanguageData.GetContent(ContentDefine.Friend.LV, fd.level);
            fg.FriendName = fd.name;

            m_listQuestGrid.Add(obj);

            //obj.GetComponentsInChildren<MogoSingleButton>(true)[0].ButtonListTransform = m_goQuestDialogList.transform;

            //m_goQuestDialogList.transform.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);

            m_camQuestDialogList.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -220 - 108 * (m_listQuestGrid.Count - 5);
        });
    }

    public void ClearFriendGridList()
    {
        for (int i = 0; i < m_listFriendGrid.Count; ++i)
        {
            int index = i;
            m_listFriendGrid[index].GetComponentsInChildren<FriendMessageGrid>(true)[0].Release();
            AssetCacheMgr.ReleaseInstance(m_listFriendGrid[index]);
        }

        m_listFriendGrid.Clear();
        m_camFriendGridList.transform.localPosition = new Vector3(0, -158, 0);
    }

    public void ClearQuestDialogList()
    {
        for (int i = 0; i < m_listQuestGrid.Count; ++i)
        {
            int index = i;
            m_listQuestGrid[index].GetComponentsInChildren<FriendQuestAddGrid>(true)[0].Release();
            AssetCacheMgr.ReleaseInstance(m_listQuestGrid[index]);
        }

        m_listQuestGrid.Clear();
        m_camQuestDialogList.transform.localPosition = new Vector3(0, -220, 0);
    }

    #endregion

    #region 创建邮件Grid

    readonly static float ONE_MESSAGE_GRID_Y = 36;
    readonly static float MESSAGE_GRID_BEGIN_Y = -30;

    public void AddMessageDialogLabel(string text, float x)
    {
        AssetCacheMgr.GetUIInstance("SoceityUIMessageGrid.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.transform.parent = m_goMessageList.transform;

            obj.transform.localPosition = new Vector3(x, MESSAGE_GRID_BEGIN_Y - m_listMessageLabel.Count * ONE_MESSAGE_GRID_Y, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            UILabel lblContent = obj.transform.Find("MessageGridText").GetComponentsInChildren<UILabel>(true)[0];
            lblContent.text = text;
            lblContent.color = new Color32(63, 27, 4, 255);

            GameObject goBG = obj.transform.Find("MessageGridBG").gameObject;
            goBG.SetActive(false);

            m_listMessageLabel.Add(obj);

            m_camMessageList.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -185 - ONE_MESSAGE_GRID_Y * (m_listMessageLabel.Count - 15);
        });
    }

    public void AddMessageDialogTime(int time, float x)
    {
        AssetCacheMgr.GetUIInstance("SoceityUIMessageGrid.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.transform.parent = m_goMessageList.transform;

            obj.transform.localPosition = new Vector3(x, MESSAGE_GRID_BEGIN_Y - m_listMessageLabel.Count * ONE_MESSAGE_GRID_Y, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            string timeString = time.GetTime().FormatTime();

            UILabel lblContent = obj.transform.Find("MessageGridText").GetComponentsInChildren<UILabel>(true)[0];
            lblContent.text = timeString;
            lblContent.color = new Color32(255, 255, 255, 255);

            GameObject goBG = obj.transform.Find("MessageGridBG").gameObject;
            goBG.SetActive(true);

            m_listMessageLabel.Add(obj);

            m_camMessageList.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -185 - ONE_MESSAGE_GRID_Y * (m_listMessageLabel.Count - 15);
        });
    }

    public void AddMailGrid(MailGridData md)
    {
        AssetCacheMgr.GetUIInstance("MailGrid.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.transform.parent = m_goMailList.transform;

            obj.transform.localPosition = new Vector3(0, -m_listMailGrid.Count * 108, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camMailList;

            //MailGrid mg = obj.GetComponentsInChildren<MailGrid>(true)[0];
            MailGrid mg = obj.AddComponent<MailGrid>();

            mg.MailGridName = md.name;
            mg.MailGridTopic = md.topic;
            mg.MailGridTime = md.time;
            mg.AttachNoRead = md.isAttachNoRead;
            mg.AttachRead = md.isAttachRead;
            mg.Read = md.isRead;
            mg.NoRead = md.isNoRead;
            mg.Id = m_listMailGrid.Count;
            TDBID idx = Convert.ToUInt64(m_listMailGrid.Count);
            m_mailGridToMailID[idx] = md.mailId;

            m_listMailGrid.Add(obj);

            if (m_listMailGrid.Count - 4 >= 0)
            {
                m_camMailList.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -172 - (m_listMailGrid.Count - 4) * 108;
            }
            else
            {
                m_camMailList.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -172;
            }
        });
    }

    public void ClearMessageDialogList()
    {
        for (int i = 0; i < m_listMessageLabel.Count; ++i)
        {
            int index = i;

            AssetCacheMgr.ReleaseInstance(m_listMessageLabel[index]);
        }

        m_listMessageLabel.Clear();
        m_camMessageList.transform.localPosition = new Vector3(0, -185, 0);
    }

    public void ClearReturnMessageText()
    {
        m_inputReturnMessage.text = "";
    }

    public void ClearMailGridList()
    {
        for (int i = 0; i < m_listMailGrid.Count; ++i)
        {
            int index = i;

            AssetCacheMgr.ReleaseInstance(m_listMailGrid[index]);
        }

        m_listMailGrid.Clear();
        m_mailGridToMailID.Clear();

        m_camMailList.transform.localPosition = new Vector3(0, -172, 0);
    }   

    #endregion     
  
    #region 界面打开和关闭

    protected override void OnEnable()
    {
        base.OnEnable();
        ResetDefaultSocietyPage();
    }

    public void DestroyAllUIAndResources()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroySocietyUI();
            ReleasePreLoadResources();
        }
    }
    void OnDisable()
    {
        DestroyAllUIAndResources();
    }

    #endregion
}
