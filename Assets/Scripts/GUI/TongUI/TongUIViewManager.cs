/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TongUIViewManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：公会ui
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using System;

public enum MyTongMemberUITab
{
    MyTongMemberTab1 = 0,
    MyTongMemberTab2 = 1,
    MyTongMemberTab3 = 2,
}

public class TongUIViewManager : MogoUIParent
{
    public class Event
    {
        public readonly static string OnSelectTong = "TongUIViewManager.OnSelectTong";//<int>
        public readonly static string OnTongShow = "TongUIViewManager.OnTongShow";
        public readonly static string OnCreateTong = "TongUIViewManager.OnCreateTong";
        public readonly static string OnJoinTong = "TongUIViewManager.OnJoinTong";
        public readonly static string OnDismissTong = "TongUIViewManager.OnDismissTong";
        public readonly static string OnModifyNotice = "TongUIViewManager.OnModifyNotice";

        public readonly static string OnShowMember = "TongUIViewManager.OnShowMember";
        public readonly static string OnShowSkill = "TongUIViewManager.OnShowSkill";
        public readonly static string OnShowDragon = "TongUIViewManager.OnShowDragon";
        public readonly static string OnShowWar = "TongUIViewManager.OnShowWar";

        public readonly static string OnSelectMemberTab = "TongUIViewManager.OnSelectMemberTab";//<int>
        public readonly static string OnSelectMemberListGrid = "TongUIViewManager.OnSelectMemberListGrid";//<int>

        public readonly static string OnRefuseApplicant = "TongUIViewManager.OnRefuseApplicant";//<int>
        public readonly static string OnAcceptApplicant = "TongUIViewManager.OnAcceptApplicant";//<int>
        public readonly static string OnMemberPromote = "TongUIViewManager.OnMemberPromote";//<int>
        public readonly static string OnMemberDemote = "TongUIViewManager.OnMemberDemote";//<int>
        public readonly static string OnAbdicate = "TongUIViewManager.OnAbdicate";//<int>
        public readonly static string OnMemberKick = "TongUIViewManager.OnMemberKick";//<int>
        public readonly static string OnSkillUpgrde = "TongUIViewManager.OnSkillUpgrde";//<int>
        public readonly static string OnMagicUp = "TongUIViewManager.OnMagicUp";//<int>
        public readonly static string OnCreateTongDone = "TongUIViewManager.OnCreateTongDone";//<string>

        public readonly static string OnInviteRecommender = "TongUIViewManager.OnInviteRecommender";//<int>
        public readonly static string OnRefreshRecommender = "TongUIViewManager.OnRefreshRecommender";//<int>


    }

    public class TongData
    {
        public string name;
        public string level;
        public string num;
    }

    public class ApplicantData
    {
        public string vocationIcon;
        public string name;
        public string level;
        public string power;
    }

    public class PresenterData
    {
        public string vocationIcon;
        public string name;
        public string level;
        public string power;
    }

    public class MemberData
    {
        public string vocationIcon;
        public string name;
        public string level;
        public string position;
        public string contribution;
        public string power;
        public string date;
    }

    public class TongSkillData
    {
        public string name;
        public string icon;
        public int starNum;
        public string effect1;
        public string effect2;
        public string cost;
    }

    static public TongUIViewManager Instance;
    Dictionary<string, Action<int>> m_buttonAction = new Dictionary<string, Action<int>>();
    Dictionary<int, UILabel> m_myTongMemberTabLabelList = new Dictionary<int, UILabel>(); 

    void Awake()
    {
        base.Init();
        Instance = this;
        InitTongListUIObj();
        InitMyTongUIObj();
        InitButtonAction();

        HideAll();
        //ShowTongList();
        //ShowCreateTong("X500");
        //ShowMyTong();
        //ShowDragonPower("3次", 999, 2000, 40, 50, 69);
        //ShowSkillList();
        //ShowMemberManager();
        //ShowMyTongApplicantList();

    }

    private void InitButtonAction()
    {
        GetUIChild("CreateTongBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["CreateTongBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("CreateTongBtn");
            EventDispatcher.TriggerEvent(Event.OnCreateTong);
        };

        GetUIChild("JoinTongBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["JoinTongBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("JoinTongBtn");
            EventDispatcher.TriggerEvent(Event.OnJoinTong);
        };

        m_buttonAction["TongListGrid(Clone)"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("TongListGrid:" + id);
            EventDispatcher.TriggerEvent(Event.OnSelectTong, id);
        };

        GetUIChild("MyTongDragonBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["MyTongDragonBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongDragonBtn");
            EventDispatcher.TriggerEvent(Event.OnShowDragon);
        };

        GetUIChild("MyTongMemberBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["MyTongMemberBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongMemberBtn");
            EventDispatcher.TriggerEvent(Event.OnShowMember);
        };

        GetUIChild("MyTongSkillBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["MyTongSkillBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongSkillBtn");
            EventDispatcher.TriggerEvent(Event.OnShowSkill);
        };

        GetUIChild("MyTongWarBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["MyTongWarBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongWarBtn");
            EventDispatcher.TriggerEvent(Event.OnShowWar);
        };

        GetUIChild("MyTongNoticeBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["MyTongNoticeBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongNoticeBtn");
            EventDispatcher.TriggerEvent(Event.OnModifyNotice);
        };

        GetUIChild("CancelDismissBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["CancelDismissBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("CancelDismissBtn");
            EventDispatcher.TriggerEvent(Event.OnDismissTong);
        };


        //创建公会界面
        GetUIChild("CreateTongCreateBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["CreateTongCreateBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("CreateTongCreateBtn");
            EventDispatcher.TriggerEvent(Event.OnCreateTongDone, m_createTongInputName.text);
        };

        GetUIChild("CreateTongBackBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["CreateTongBackBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("CreateTongBackBtn");
            m_createTong.gameObject.SetActive(false);

        };

        //公会成员
        AddListener("MyTongMemberTab1", (id) =>
        {
            EventDispatcher.TriggerEvent(Event.OnSelectMemberTab, (int)MyTongMemberUITab.MyTongMemberTab1);
            ShowMyTongMemberList();
        });
        AddListener("MyTongMemberTab2", (id) =>
        {
            EventDispatcher.TriggerEvent(Event.OnSelectMemberTab, (int)MyTongMemberUITab.MyTongMemberTab2);
            ShowMyTongApplicantList();
        });
        AddListener("MyTongMemberTab3", (id) =>
        {
            EventDispatcher.TriggerEvent(Event.OnSelectMemberTab, (int)MyTongMemberUITab.MyTongMemberTab3);
            ShowMyTongPresenterList();
        });

        AddListener("MyTongMemberQuit", (id) =>
        {
            m_myTongMember.gameObject.SetActive(false);
        });


        m_buttonAction["MyTongMemberListGrid(Clone)"] = (id) =>
        {
            m_currentMemberListDown = id;
            EventDispatcher.TriggerEvent(Event.OnSelectMemberListGrid, id);
            ShowMemberControlPanel();
        };

        AddListener("MyTongMemberListCpBtn1", (id) =>
        {
            EventDispatcher.TriggerEvent(Event.OnMemberPromote, m_currentMemberListDown);
        });

        AddListener("MyTongMemberListCpBtn2", (id) =>
        {
            EventDispatcher.TriggerEvent(Event.OnMemberDemote, m_currentMemberListDown);
        });

        AddListener("MyTongMemberListCpBtn3", (id) =>
        {
            EventDispatcher.TriggerEvent(Event.OnAbdicate, m_currentMemberListDown);
        });

        AddListener("MyTongMemberListCpBtn4", (id) =>
        {
            EventDispatcher.TriggerEvent(Event.OnMemberKick, m_currentMemberListDown);
        });

        AddListener("MyTongMemberListCpMaskBg", (id) =>
        {
            m_myTongMemberListCp.gameObject.SetActive(false);
        });


        m_buttonAction["MyTongApplicantListGridBtnOk"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongApplicantListGridBtnOk");
            EventDispatcher.TriggerEvent(Event.OnAcceptApplicant, id);
        };
        m_buttonAction["MyTongApplicantListGridBtnCancel"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongApplicantListGridBtnCancel");
            EventDispatcher.TriggerEvent(Event.OnRefuseApplicant, id);
        };


        GetUIChild("MyTongPresenterListBtnRefresh").gameObject.AddComponent<TongButton>();
        m_buttonAction["MyTongPresenterListBtnRefresh"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongPresenterListBtnRefresh");
            EventDispatcher.TriggerEvent(Event.OnRefreshRecommender);

        };

        GetUIChild("MyTongPresenterListBtnInvite").gameObject.AddComponent<TongButton>();
        m_buttonAction["MyTongPresenterListBtnInvite"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongPresenterListBtnInvite");
            EventDispatcher.TriggerEvent(Event.OnInviteRecommender, m_currentRecommender);

        };

        m_buttonAction["MyTongPresenterListGrid(Clone)"] = (id) =>
        {
            m_currentRecommender = id;
        };

        GetUIChild("MyTongSkillCloseBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["MyTongSkillCloseBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongSkillCloseBtn");
            m_myTongSkillList.gameObject.SetActive(false);

        };

        m_buttonAction["MyTongSkillListGridUpgradeBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongSkillListGridUpgradeBtn");
            EventDispatcher.TriggerEvent(Event.OnSkillUpgrde, id);

        };


        GetUIChild("MyTongDragonBackBtn").gameObject.AddComponent<TongButton>();
        m_buttonAction["MyTongDragonBackBtn"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongDragonBackBtn");
            m_myTongDragon.gameObject.SetActive(false);

        };


        GetUIChild("MyTongDragonBtn1").gameObject.AddComponent<TongButton>().id = 0;
        m_buttonAction["MyTongDragonBtn1"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongDragonBtn1");
            EventDispatcher.TriggerEvent(Event.OnMagicUp, id);

        };

        GetUIChild("MyTongDragonBtn2").gameObject.AddComponent<TongButton>().id = 1;
        m_buttonAction["MyTongDragonBtn2"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongDragonBtn2");
            EventDispatcher.TriggerEvent(Event.OnMagicUp, id);

        };

        GetUIChild("MyTongDragonBtn3").gameObject.AddComponent<TongButton>().id = 2;
        m_buttonAction["MyTongDragonBtn3"] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug("MyTongDragonBtn3");
            EventDispatcher.TriggerEvent(Event.OnMagicUp, id);
        };
    }

    public void ShowMemberControlPanel(bool isShow = true)
    {
        m_myTongMemberListCp.gameObject.SetActive(isShow);
    }

    private void AddListener(string uiName, Action<int> action)
    {
        GetUIChild(uiName).gameObject.AddComponent<TongButton>();

        m_buttonAction[uiName] = (id) =>
        {
            Mogo.Util.LoggerHelper.Debug(uiName);
            action(id);
        };
    }

    private void HideAll()
    {
        m_otherTong.gameObject.SetActive(false);
        m_myTong.gameObject.SetActive(false);
        m_myTongMember.gameObject.SetActive(false);
        m_createTong.gameObject.SetActive(false);
        m_myTongMemberListCp.gameObject.SetActive(false);
        m_myTongMemberList.gameObject.SetActive(false);
        m_myTongApplicantList.gameObject.SetActive(false);
        m_myTongSkillList.gameObject.SetActive(false);
        m_myTongDragon.gameObject.SetActive(false);
    }

    public void SetTitle(string title)
    {
        MenuUIViewManager.Instance.SetDialogTitleText(title);
    }


    private string GetPageNumStr(MyDragableCamera camera, int pageNum)
    {
        //Mogo.Util.LoggerHelper.Debug("camera:" + camera.gameObject.name);
        //Mogo.Util.LoggerHelper.Debug("camera.GetCurrentPage():" + camera.GetCurrentPage() + ",pageNum:" + pageNum);
        int currentPage = camera.GetCurrentPage() + 1;
        if (currentPage > pageNum) currentPage = pageNum;
        return currentPage + "/" + pageNum;
    }

    private int GetPageNum(int count, int gap, float pageHeight)
    {
        return (int)(count * gap / pageHeight) + 1;
    }

    private void OnListPageMove(UILabel lbl, MyDragableCamera camera, int count, int gap, float pageHeight)
    {
        Mogo.Util.LoggerHelper.Debug("OnListPageMove:" + lbl.name + ",count:" + count + ",gap:" + gap + ",pageHeight:" + pageHeight);
        lbl.text = GetPageNumStr(camera, GetPageNum(count, gap, pageHeight));
    }

    #region 公会列表
    Transform m_createTong;
    UILabel m_createTongCostLbl;
    UILabel m_createTongInputName;

    Transform m_otherTong;
    Transform m_tongListCamera;
    MyDragableCamera m_tongListDragCamera;
    Transform m_tongListCameraBegin;
    Transform m_tongListRoot;
    List<TongData> m_tongDataList;
    UILabel m_tongListPageNum;
    List<GameObject> m_tongListGridObj = new List<GameObject>();
    MogoSingleButtonList m_tongListBtnList;
    const string TONG_LIST_GRID_PREFAB = "TongListGrid.prefab";
    const int TONG_LIST_GRID_GAP = 100;
    float tonglistPageHeight;
    const int TONG_LIST_PAGE_GRID_NUM = 5;


    private void InitTongListUIObj()
    {
        m_otherTong = GetUIChild("OtherTong");
        m_tongListCamera = GetUIChild("TongListCamera");
        m_tongListCamera.GetComponent<UIViewport>().sourceCamera = GameObject.Find("Camera").GetComponent<Camera>();
        m_tongListCameraBegin = GetUIChild("TongListPosBegin");
        tonglistPageHeight = GetUIChild("TongListBG").transform.localScale.y;
        m_tongListRoot = GetUIChild("TongList");
        m_tongListBtnList = m_tongListRoot.GetComponent<MogoSingleButtonList>();
        m_tongListPageNum = GetUIChild("TongListPageNum").GetComponent<UILabel>();
        m_tongListDragCamera = m_tongListCamera.GetComponent<MyDragableCamera>();
        m_tongListDragCamera.MovePageDone +=
            (() =>
            {
                OnListPageMove(m_tongListPageNum, m_tongListDragCamera, m_tongDataList.Count, TONG_LIST_GRID_GAP, tonglistPageHeight);
            });
        m_createTong = GetUIChild("CreateTong");
        m_createTongCostLbl = GetUIChild("CreateTongCostNumLbl").GetComponent<UILabel>();
        m_createTongInputName = GetUIChild("CreateTongInputlbl").GetComponent<UILabel>();

        //test data
        //m_tongDataList = new List<TongData>();
        //for (int i = 0; i < 21; i++)
        //{
        //    TongData tong = new TongData() { level = i + "", name = "name" + i, num = (i + 1) + "" };
        //    m_tongDataList.Add(tong);
        //}
    }



    /// <summary>
    /// 设置可加入公会的列表
    /// </summary>
    /// <param name="list"></param>
    public void SetTongList(List<TongData> list)
    {
        m_tongDataList = list;
    }

    /// <summary>
    /// 显示公会列表
    /// </summary>
    public void ShowTongList()
    {
        HideAll();
        ClearOldTongList();
        m_otherTong.gameObject.SetActive(true);
        if (m_tongDataList == null) return;

        AddTongList();
        ResetTongListCamera();

        OnListPageMove(m_tongListPageNum, m_tongListDragCamera, m_tongDataList.Count, TONG_LIST_GRID_GAP, tonglistPageHeight);
        //int currentPage = m_tongListCamera.GetComponent<MyDragableCamera>().GetCurrentPage() + 1;
        //m_tongListPageNum.text = GetPageNumStr(m_tongListDragCamera, (m_tongListGridObj.Count / TONG_LIST_PAGE_GRID_NUM) + 1);
    }

    private void AddTongList()
    {
        m_tongListBtnList.SingleButtonList = new List<MogoSingleButton>();
        for (int i = 0; i < m_tongDataList.Count; i++)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance(TONG_LIST_GRID_PREFAB, (str, id, obj) =>
            {
                GameObject go = obj as GameObject;
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_tongListRoot);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, -index * TONG_LIST_GRID_GAP, go.transform.localPosition.z);

                m_tongListBtnList.SingleButtonList.Add(go.GetComponent<MogoSingleButton>());
                go.GetComponent<MyDragCamera>().RelatedCamera = m_tongListCamera.GetComponent<Camera>();
                go.AddComponent<TongButton>().id = index;

                go.transform.Find("TongListGridName").GetComponent<UILabel>().text = m_tongDataList[index].name;
                go.transform.Find("TongListGridLevel").GetComponent<UILabel>().text = m_tongDataList[index].level;
                go.transform.Find("TongListGridNum").GetComponent<UILabel>().text = m_tongDataList[index].num;

                m_tongListGridObj.Add(go);
            });
        }
    }


    private void ResetTongListCamera()
    {
        int pageNum = (int)(TONG_LIST_GRID_GAP * m_tongDataList.Count / tonglistPageHeight) + 1;


        m_tongListCamera.localPosition = m_tongListCameraBegin.localPosition;

        MyDragableCamera camera = m_tongListCamera.GetComponent<MyDragableCamera>();
        camera.transformList = new List<Transform>();
        for (int i = 0; i < pageNum; i++)
        {
            GameObject go = new GameObject();
            Utils.MountToSomeObjWithoutPosChange(go.transform, m_tongListRoot);
            go.transform.localPosition = new Vector3(m_tongListCameraBegin.localPosition.x, m_tongListCameraBegin.localPosition.y - TONG_LIST_GRID_GAP * TONG_LIST_PAGE_GRID_NUM * i, m_tongListCameraBegin.localPosition.z);
            camera.transformList.Add(go.transform);
        }
        if (pageNum <= 1)
        {
            GameObject go = new GameObject();
            Utils.MountToSomeObjWithoutPosChange(go.transform, m_tongListRoot);
            go.transform.localPosition = new Vector3(m_tongListCameraBegin.localPosition.x, m_tongListCameraBegin.localPosition.y, m_tongListCameraBegin.localPosition.z);
            camera.transformList.Add(go.transform);
        }
    }

    private void ClearOldTongList()
    {
        foreach (GameObject go in m_tongListGridObj)
        {
            AssetCacheMgr.ReleaseInstance(go);
        }
        m_tongListGridObj.Clear();
    }

    /// <summary>
    /// 显示创建公会窗口
    /// </summary>
    /// <param name="num">消耗的砖石数量,"X500"这样传</param>
    public void ShowCreateTong(string num, bool isShow = true)
    {
        m_createTong.gameObject.SetActive(isShow);
        m_createTongCostLbl.text = num;
    }

    #endregion 公会列表

    #region 我的公会
    Transform m_myTong;
    UILabel m_myTongChairmanName;
    UILabel m_myTongNum;
    UILabel m_myTongMoney;
    UILabel m_myTongNotice;

    UISprite m_myTongMemberListIsNew;
    UISprite m_myTongSkillIsNew;
    UISprite m_myTongDragonIsNew;
    UISprite m_myTongWarIsNew;

    Transform m_myTongMember;
    MogoSingleButtonList m_myTongMemberTabList;

    //成员列表
    Transform m_myTongMemberList;
    List<MemberData> m_myTongMemberDataList;
    List<GameObject> m_myTongMemberListObjs = new List<GameObject>();
    Transform m_myTongMemberListCamera;
    MyDragableCamera m_myTongMemberListDragCamera;
    Transform m_myTongMemberListContentRoot;
    MogoSingleButtonList m_myTongMemberListButtonList;
    Transform m_myTongMemberListCameraBegin;
    float m_myTongMemberListPageHeight;
    const int MYTONG_MEMBERLIST_GRID_GAP = 100;
    Transform m_myTongMemberListCp;
    UILabel m_myTongMemberListPageNunm;
    int m_currentMemberListDown = 0;

    //申请者列表
    private List<ApplicantData> m_myTongApplicantDataList;
    private List<GameObject> m_myTongApplicantListObjs = new List<GameObject>();
    Transform m_myTongApplicantList;
    private Transform m_myTongApplicantListCamera;
    private Transform m_myTongApplicantListContentRoot;
    //private MogoSingleButtonList m_myTongApplicantListButtonList;
    private Transform m_myTongApplicantListCameraBegin;
    private float m_myTongApplicantListPageHeight;
    UILabel m_myTongApplicantListPageNunm;
    private MyDragableCamera m_myTongApplicantListDragCamera;


    //推荐者列表
    private Transform m_myTongPresenterList;
    private Transform m_myTongPresenterListCamera;
    private MyDragableCamera m_myTongPresenterListDragCamera;
    private Transform m_myTongPresenterListContentRoot;
    private MogoSingleButtonList m_myTongPresenterListButtonList;
    private Transform m_myTongPresenterListCameraBegin;
    private float m_myTongPresenterListPageHeight;
    private UILabel m_myTongPresenterListPageNunm;
    private List<GameObject> m_myTongPresenterListObjs = new List<GameObject>();
    private List<PresenterData> m_myTongPresenterDataList;
    private int m_currentRecommender = -1;

    //技能列表
    private Transform m_myTongSkillList;
    private Transform m_myTongSkillListCamera;
    private MyDragableCamera m_myTongSkillListDragCamera;
    private Transform m_myTongSkillListContentRoot;
    private Transform m_myTongSkillListCameraBegin;
    private float m_myTongSkillListPageWidth;
    private List<TongSkillData> m_myTongSkillDataList;
    private List<GameObject> m_myTongSkillListObjs = new List<GameObject>();


    //龙鳞水晶
    private Transform m_myTongDragon;
    private UILabel m_myTongDragonLeftTimeLbl;
    private UISprite m_myTongDragonImg;
    private MogoProgressBar m_myTongDragonProgressBar;
    private UILabel m_myTongDragonCostlbl1;
    private UILabel m_myTongDragonCostlbl2;
    private UILabel m_myTongDragonCostlbl3;


    private void InitMyTongUIObj()
    {
        m_myTong = GetUIChild("MyTong");
        m_myTongChairmanName = GetUIChild("MyTongChairmanText").GetComponent<UILabel>();
        m_myTongNum = GetUIChild("MyTongNumText").GetComponent<UILabel>();
        m_myTongMoney = GetUIChild("MyTongMoneyText").GetComponent<UILabel>();
        m_myTongNotice = GetUIChild("MyTongNoticeMsgText").GetComponent<UILabel>();

        m_myTongMemberListIsNew = GetUIChild("MyTongMemberBtnIcon").GetComponent<UISprite>();
        m_myTongSkillIsNew = GetUIChild("MyTongSkillBtnIcon").GetComponent<UISprite>();
        m_myTongDragonIsNew = GetUIChild("MyTongDragonBtnIcon").GetComponent<UISprite>();
        m_myTongWarIsNew = GetUIChild("MyTongWarBtnIcon").GetComponent<UISprite>();

        //公会成员
        m_myTongMember = GetUIChild("MyTongMember");
        m_myTongMemberTabList = GetUIChild("MyTongMemberTab").GetComponent<MogoSingleButtonList>();

        //成员列表
        m_myTongMemberList = GetUIChild("MyTongMemberList");
        m_myTongMemberListCamera = GetUIChild("MyTongMemberListContentCamera");
        m_myTongMemberListCamera.GetComponent<UIViewport>().sourceCamera = GameObject.Find("GlobleUICamera").GetComponent<Camera>();
        m_myTongMemberListPageNunm = GetUIChild("MyTongMemberListPageNum").GetComponent<UILabel>();
        m_myTongMemberListContentRoot = GetUIChild("MyTongMemberListContent");
        m_myTongMemberListButtonList = m_myTongMemberListContentRoot.GetComponent<MogoSingleButtonList>();
        m_myTongMemberListCameraBegin = GetUIChild("MyTongMemberListContentPosBegin");
        m_myTongMemberListPageHeight = GetUIChild("MyTongMemberListContentBG").localScale.y;
        m_myTongMemberListCp = GetUIChild("MyTongMemberListCp");
        m_myTongMemberListDragCamera = m_myTongMemberListCamera.GetComponent<MyDragableCamera>();
        m_myTongMemberListDragCamera.MovePageDone +=
            () =>
            {
                OnListPageMove(m_myTongMemberListPageNunm, m_myTongMemberListDragCamera, m_myTongMemberDataList.Count, 100, m_myTongMemberListPageHeight);
            };
        //test data
        //m_myTongMemberDataList = new List<MemberData>();
        //for (int i = 0; i < 20; i++)
        //{
        //    MemberData data = new MemberData() { contribution = i.ToString(), date = i.ToString(), level = i.ToString(), name = i.ToString(), position = i.ToString(), power = i.ToString(), vocationIcon = i.ToString() };
        //    m_myTongMemberDataList.Add(data);

        //}

        //申请者列表
        m_myTongApplicantList = GetUIChild("MyTongApplicantList");
        m_myTongApplicantListCamera = GetUIChild("MyTongApplicantListContentCamera");
        m_myTongApplicantListDragCamera = m_myTongApplicantListCamera.GetComponent<MyDragableCamera>();
        m_myTongApplicantListCamera.GetComponent<UIViewport>().sourceCamera = GameObject.Find("GlobleUICamera").GetComponent<Camera>();
        m_myTongApplicantListContentRoot = GetUIChild("MyTongApplicantListContent");
        //m_myTongApplicantListButtonList = m_myTongApplicantListContentRoot.GetComponent<MogoSingleButtonList>();
        m_myTongApplicantListCameraBegin = GetUIChild("MyTongApplicantListContentPosBegin");
        m_myTongApplicantListPageHeight = GetUIChild("MyTongApplicantListContentBG").localScale.y;
        m_myTongApplicantListPageNunm = GetUIChild("MyTongApplicantListPageNum").GetComponent<UILabel>();

        m_myTongApplicantListDragCamera.MovePageDone +=
           () =>
           {
               OnListPageMove(m_myTongApplicantListPageNunm, m_myTongApplicantListDragCamera, m_myTongApplicantDataList.Count, 100, m_myTongApplicantListPageHeight);
           };
        //test data
        //m_myTongApplicantDataList = new List<ApplicantData>();
        //for (int i = 0; i < 10; i++)
        //{
        //    ApplicantData data = new ApplicantData() { level = i.ToString(), name = i.ToString(), power = i.ToString(), vocationIcon = i.ToString() };
        //    m_myTongApplicantDataList.Add(data);

        //}

        //推荐列表
        m_myTongPresenterList = GetUIChild("MyTongPresenterList");
        m_myTongPresenterListCamera = GetUIChild("MyTongPresenterListContentCamera");
        m_myTongPresenterListDragCamera = m_myTongPresenterListCamera.GetComponent<MyDragableCamera>();
        m_myTongPresenterListCamera.GetComponent<UIViewport>().sourceCamera = GameObject.Find("GlobleUICamera").GetComponent<Camera>();
        m_myTongPresenterListContentRoot = GetUIChild("MyTongPresenterListContent");
        m_myTongPresenterListButtonList = m_myTongPresenterListContentRoot.GetComponent<MogoSingleButtonList>();
        m_myTongPresenterListCameraBegin = GetUIChild("MyTongPresenterListContentPosBegin");
        m_myTongPresenterListPageHeight = GetUIChild("MyTongPresenterListContentBG").localScale.y;
        m_myTongPresenterListPageNunm = GetUIChild("MyTongPresenterListPageNum").GetComponent<UILabel>();

        m_myTongPresenterListDragCamera.MovePageDone +=
           () =>
           {
               OnListPageMove(m_myTongPresenterListPageNunm, m_myTongPresenterListDragCamera, m_myTongPresenterDataList.Count, 100, m_myTongPresenterListPageHeight);
           };
        //test data
        //m_myTongPresenterDataList = new List<PresenterData>();
        //for (int i = 0; i < 10; i++)
        //{
        //    PresenterData data = new PresenterData() { level = i.ToString(), name = i.ToString(), power = i.ToString(), vocationIcon = i.ToString() };
        //    m_myTongPresenterDataList.Add(data);

        //}


        //技能列表
        m_myTongSkillList = GetUIChild("MyTongSkill");
        m_myTongSkillListCamera = GetUIChild("MyTongSkillListCamera");
        m_myTongSkillListDragCamera = m_myTongSkillListCamera.GetComponent<MyDragableCamera>();
        m_myTongSkillListCamera.GetComponent<UIViewport>().sourceCamera = GameObject.Find("GlobleUICamera").GetComponent<Camera>();
        m_myTongSkillListContentRoot = GetUIChild("MyTongSkillList");
        //m_myTongPresenterListButtonList = m_myTongPresenterListContentRoot.GetComponent<MogoSingleButtonList>();
        m_myTongSkillListCameraBegin = GetUIChild("MyTongSkillListPosBegin");
        m_myTongSkillListPageWidth = GetUIChild("MyTongSkillListBG").localScale.x;

        m_myTongSkillListDragCamera.MovePageDone +=
           () =>
           {
           };
        //test data
        //m_myTongSkillDataList = new List<TongSkillData>();
        //for (int i = 0; i < 8; i++)
        //{
        //    TongSkillData data = new TongSkillData() { name = i.ToString(), effect1 = i.ToString(), effect2 = i.ToString(), icon = i.ToString(), starNum = i };
        //    m_myTongSkillDataList.Add(data);

        //}

        //龙鳞水晶
        m_myTongDragon = GetUIChild("MyTongDragon");
        m_myTongDragonLeftTimeLbl = GetUIChild("MyTongDragonBannerTimeText").GetComponent<UILabel>();
        m_myTongDragonImg = GetUIChild("MyTongDragonImg").GetComponent<UISprite>();
        m_myTongDragonProgressBar = GetUIChild("MyTongDragonProgressBar").GetComponent<MogoProgressBar>();
        m_myTongDragonCostlbl1 = GetUIChild("MyTongDragonBtn1Costlbl").GetComponent<UILabel>();
        m_myTongDragonCostlbl2 = GetUIChild("MyTongDragonBtn2Costlbl").GetComponent<UILabel>();
        m_myTongDragonCostlbl3 = GetUIChild("MyTongDragonBtn3Costlbl").GetComponent<UILabel>();

        m_myTongMemberTabLabelList[(int)MyTongMemberUITab.MyTongMemberTab1] = GetUIChild("MyTongMemberTab1Text").GetComponent<UILabel>();
        m_myTongMemberTabLabelList[(int)MyTongMemberUITab.MyTongMemberTab2] = GetUIChild("MyTongMemberTab2Text").GetComponent<UILabel>();
        m_myTongMemberTabLabelList[(int)MyTongMemberUITab.MyTongMemberTab3] = GetUIChild("MyTongMemberTab3Text").GetComponent<UILabel>();
        foreach (var pair in m_myTongMemberTabLabelList)
        {
            if (pair.Key == (int)MyTongMemberUITab.MyTongMemberTab1)
                MyTongMemberTabDown(pair.Key);
            else
                MyTongMemberTabUp(pair.Key);
        }
    }

    #region Tab Change
    public void HandleMyTongMemberTabChange(int fromTab, int toTab)
    {
        MyTongMemberTabUp(fromTab);
        MyTongMemberTabDown(toTab);
    }

    void MyTongMemberTabUp(int tab)
    {
        if (m_myTongMemberTabLabelList.ContainsKey(tab))
        {
            UILabel fromTabLabel = m_myTongMemberTabLabelList[tab];
            if (fromTabLabel != null)
            {
                //fromTabLabel.color = new Color32(37, 29, 6, 255);
                //fromTabLabel.effectStyle = UILabel.Effect.None;

                fromTabLabel.color = new Color32(255, 255, 255, 255);
                fromTabLabel.effectStyle = UILabel.Effect.Outline;
                fromTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }
    }

    void MyTongMemberTabDown(int tab)
    {
        if (m_myTongMemberTabLabelList.ContainsKey(tab))
        {
            UILabel toTabLabel = m_myTongMemberTabLabelList[tab];
            if (toTabLabel != null)
            {
                toTabLabel.color = new Color32(255, 255, 255, 255);
                toTabLabel.effectStyle = UILabel.Effect.Outline;
                toTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }
    }
    #endregion

    /// <summary>
    /// 管理公会界面会长的名字
    /// </summary>
    /// <param name="name"></param>
    public void SetTongName(string name)
    {
        m_myTongChairmanName.text = name;
    }

    /// <summary>
    /// 管理公会界面公会人数
    /// </summary>
    /// <param name="num"></param>
    public void SetTongNum(string num)
    {
        m_myTongNum.text = num;
    }

    /// <summary>
    /// 管理公会界面公会资产
    /// </summary>
    /// <param name="num"></param>
    public void SetTongMoney(string money)
    {
        m_myTongMoney.text = money;
    }

    public void SetMemberListIsNew(bool isNew)
    {
        m_myTongMemberListIsNew.gameObject.SetActive(isNew);
    }
    public void SetSkillIsNew(bool isNew)
    {
        m_myTongSkillIsNew.gameObject.SetActive(isNew);
    }
    public void SetDragonIsNew(bool isNew)
    {
        m_myTongDragonIsNew.gameObject.SetActive(isNew);
    }
    public void SetWarIsNew(bool isNew)
    {
        m_myTongWarIsNew.gameObject.SetActive(isNew);
    }

    /// <summary>
    /// 管理公会界面公会公告
    /// </summary>
    /// <param name="notice"></param>
    public void SetTongNotice(string notice)
    {
        m_myTongNotice.text = notice;
    }

    /// <summary>
    /// 显示我的公会
    /// </summary>
    public void ShowMyTong()
    {
        HideAll();
        m_myTong.gameObject.SetActive(true);
    }

    /// <summary>.
    /// 显示公会成员列表
    /// </summary>
    /// <param name="index">打开第index个tab</param>
    public void ShowMemberManager(bool isShow = true)
    {
        m_myTongMember.gameObject.SetActive(isShow);
        m_myTongMemberTabList.SetCurrentDownButton(0);
        ShowMyTongMemberList();
    }

    public void ShowMyTongMemberList()
    {
        ClearOldObjList(m_myTongMemberListObjs);
        m_myTongApplicantList.gameObject.SetActive(false);
        m_myTongPresenterList.gameObject.SetActive(false);
        m_myTongMember.gameObject.SetActive(true);
        m_myTongMemberList.gameObject.SetActive(true);
        if (m_myTongMemberDataList == null) return;
        AddList(m_myTongMemberListButtonList,
            m_myTongMemberDataList.Count,
            "MyTongMemberListGrid.prefab",
            (index, go) =>
            {
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_myTongMemberListContentRoot);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, -index * MYTONG_MEMBERLIST_GRID_GAP, go.transform.localPosition.z);

                m_myTongMemberListButtonList.SingleButtonList.Add(go.GetComponent<MogoSingleButton>());
                go.GetComponent<MyDragCamera>().RelatedCamera = m_myTongMemberListCamera.GetComponent<Camera>();
                go.AddComponent<TongButton>().id = index;

                go.transform.Find("MyTongMemberListGridVocation").GetComponent<UISprite>().spriteName = m_myTongMemberDataList[index].vocationIcon;
                go.transform.Find("MyTongMemberListGridName").GetComponent<UILabel>().text = m_myTongMemberDataList[index].name;

                go.transform.Find("MyTongMemberListGridLevel").GetComponent<UILabel>().text = m_myTongMemberDataList[index].level;

                go.transform.Find("MyTongMemberListGridPosition").GetComponent<UILabel>().text = m_myTongMemberDataList[index].position;

                go.transform.Find("MyTongMemberListGridContribution").GetComponent<UILabel>().text = m_myTongMemberDataList[index].contribution;

                go.transform.Find("MyTongMemberListGridPower").GetComponent<UILabel>().text = m_myTongMemberDataList[index].power;

                go.transform.Find("MyTongMemberListGridDate").GetComponent<UILabel>().text = m_myTongMemberDataList[index].date;

                m_myTongMemberListObjs.Add(go);
            });
        int pageNum = (int)(MYTONG_MEMBERLIST_GRID_GAP * m_myTongMemberDataList.Count / m_myTongMemberListPageHeight) + 1;
        ResetListCamera(pageNum, m_myTongMemberListCamera, m_myTongMemberListCameraBegin, m_myTongMemberListContentRoot);

        OnListPageMove(m_myTongMemberListPageNunm, m_myTongMemberListDragCamera, m_myTongMemberDataList.Count, 100, m_myTongMemberListPageHeight);
    }

    private void ResetListCamera(int pageNum, Transform dragCamera, Transform original, Transform root, bool isHorizontal = false, int gap = MYTONG_MEMBERLIST_GRID_GAP, int num = 4)
    {

        dragCamera.localPosition = original.localPosition;

        MyDragableCamera camera = dragCamera.GetComponent<MyDragableCamera>();
        camera.transformList = new List<Transform>();
        for (int i = 0; i < pageNum; i++)
        {
            GameObject go = new GameObject();
            Utils.MountToSomeObjWithoutPosChange(go.transform, root);
            if (isHorizontal)
            {

                go.transform.localPosition = new Vector3(original.localPosition.x + gap * num * i, original.localPosition.y, original.localPosition.z);
            }
            else
            {

                go.transform.localPosition = new Vector3(original.localPosition.x, original.localPosition.y - gap * num * i, original.localPosition.z);
            }
            camera.transformList.Add(go.transform);
        }
        if (pageNum <= 1)
        {
            GameObject go = new GameObject();
            Utils.MountToSomeObjWithoutPosChange(go.transform, root);

            if (isHorizontal)
            {

                go.transform.localPosition = new Vector3(original.localPosition.x, original.localPosition.y, original.localPosition.z);
            }
            else
            {

                go.transform.localPosition = new Vector3(original.localPosition.x, original.localPosition.y, original.localPosition.z);
            }

            camera.transformList.Add(go.transform);


        }
    }

    private void AddList(MogoSingleButtonList btnList, int count, string prefab, Action<int, GameObject> onLoadObj)
    {
        if (btnList != null)
        {

            btnList.SingleButtonList = new List<MogoSingleButton>();
        }
        for (int i = 0; i < count; i++)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance(prefab, (str, id, obj) =>
            {
                GameObject go = obj as GameObject;
                onLoadObj(index, go);
            });
        }
    }

    private void ClearOldObjList(List<GameObject> objs)
    {
        foreach (GameObject go in objs)
        {
            AssetCacheMgr.ReleaseInstance(go);
        }
        objs.Clear();
    }

    /// <summary>
    /// 设置成员列表
    /// </summary>
    /// <param name="memberList"></param>
    public void SetMemberList(List<MemberData> memberList)
    {
        m_myTongMemberDataList = memberList;
    }

    /// <summary>
    /// 设置申请者列表
    /// </summary>
    /// <param name="applicantList"></param>
    public void SetApplicantList(List<ApplicantData> applicantList)
    {
        m_myTongApplicantDataList = applicantList;
    }

    public void ShowMyTongApplicantList()
    {
        ClearOldObjList(m_myTongApplicantListObjs);
        m_myTongMemberList.gameObject.SetActive(false);
        m_myTongPresenterList.gameObject.SetActive(false);
        m_myTongMember.gameObject.SetActive(true);
        m_myTongApplicantList.gameObject.SetActive(true);

        if (m_myTongApplicantDataList == null) return;

        AddList(null,
            m_myTongApplicantDataList.Count,
            "MyTongApplicantListGrid.prefab",
            (index, go) =>
            {
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_myTongApplicantListContentRoot);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, -index * MYTONG_MEMBERLIST_GRID_GAP, go.transform.localPosition.z);

                //m_myTongApplicantListButtonList.SingleButtonList.Add(go.GetComponent<MogoSingleButton>());
                go.GetComponent<MyDragCamera>().RelatedCamera = m_myTongApplicantListCamera.GetComponent<Camera>();
                go.transform.Find("MyTongApplicantListGridBtnOk").gameObject.AddComponent<TongButton>().id = index;
                go.transform.Find("MyTongApplicantListGridBtnCancel").gameObject.AddComponent<TongButton>().id = index;

                go.transform.Find("MyTongApplicantListGridVocation").GetComponent<UISprite>().spriteName = m_myTongApplicantDataList[index].vocationIcon;
                go.transform.Find("MyTongApplicantListGridName").GetComponent<UILabel>().text = m_myTongApplicantDataList[index].name;

                go.transform.Find("MyTongApplicantListGridLevel").GetComponent<UILabel>().text = m_myTongApplicantDataList[index].level;

                go.transform.Find("MyTongApplicantListGridPower").GetComponent<UILabel>().text = m_myTongApplicantDataList[index].power;

                m_myTongApplicantListObjs.Add(go);
            });

        int pageNum = (int)(MYTONG_MEMBERLIST_GRID_GAP * m_myTongApplicantDataList.Count / m_myTongApplicantListPageHeight) + 1;
        ResetListCamera(pageNum, m_myTongApplicantListCamera, m_myTongApplicantListCameraBegin, m_myTongApplicantListContentRoot);

        OnListPageMove(m_myTongApplicantListPageNunm, m_myTongApplicantListDragCamera, m_myTongApplicantDataList.Count, 100, m_myTongApplicantListPageHeight);
    }

    public void ShowMyTongPresenterList()
    {
        ClearOldObjList(m_myTongPresenterListObjs);
        m_myTongApplicantList.gameObject.SetActive(false);
        m_myTongMemberList.gameObject.SetActive(false);
        m_myTongMember.gameObject.SetActive(true);
        m_myTongPresenterList.gameObject.SetActive(true);

        if (m_myTongPresenterDataList == null) return;

        AddList(m_myTongPresenterListButtonList,
            m_myTongPresenterDataList.Count,
            "MyTongPresenterListGrid.prefab",
            (index, go) =>
            {
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_myTongPresenterListContentRoot);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, -index * MYTONG_MEMBERLIST_GRID_GAP, go.transform.localPosition.z);

                m_myTongPresenterListButtonList.SingleButtonList.Add(go.GetComponent<MogoSingleButton>());
                go.GetComponent<MyDragCamera>().RelatedCamera = m_myTongPresenterListCamera.GetComponent<Camera>();
                go.AddComponent<TongButton>().id = index;

                go.transform.Find("MyTongPresenterListGridVocation").GetComponent<UISprite>().spriteName = m_myTongPresenterDataList[index].vocationIcon;
                go.transform.Find("MyTongPresenterListGridName").GetComponent<UILabel>().text = m_myTongPresenterDataList[index].name;

                go.transform.Find("MyTongPresenterListGridLevel").GetComponent<UILabel>().text = m_myTongPresenterDataList[index].level;

                go.transform.Find("MyTongPresenterListGridPower").GetComponent<UILabel>().text = m_myTongPresenterDataList[index].power;

                m_myTongPresenterListObjs.Add(go);
            });

        int pageNum = (int)(MYTONG_MEMBERLIST_GRID_GAP * m_myTongPresenterDataList.Count / m_myTongPresenterListPageHeight) + 1;
        ResetListCamera(pageNum, m_myTongPresenterListCamera, m_myTongPresenterListCameraBegin, m_myTongPresenterListContentRoot);

        OnListPageMove(m_myTongPresenterListPageNunm, m_myTongPresenterListDragCamera, m_myTongPresenterDataList.Count, 100, m_myTongPresenterListPageHeight);
    }


    /// <summary>
    /// 设置推荐者列表
    /// </summary>
    /// <param name="recommendList"></param>
    public void SetRecommendList(List<PresenterData> recommendList)
    {
        m_myTongPresenterDataList = recommendList;
    }

    /// <summary>
    /// 设置技能
    /// </summary>
    /// <param name="skillList"></param>
    public void SetSkillList(List<TongSkillData> skillList)
    {
        m_myTongSkillDataList = skillList;
    }

    /// <summary>
    /// 显示公会技能列表
    /// </summary>
    public void ShowSkillList()
    {
        ClearOldObjList(m_myTongSkillListObjs);
        m_myTongSkillList.gameObject.SetActive(true);

        if (m_myTongSkillDataList == null) return;

        AddList(null,
            m_myTongSkillDataList.Count,
            "MyTongSkillListGrid.prefab",
            (index, go) =>
            {
                Utils.MountToSomeObjWithoutPosChange(go.transform, m_myTongSkillListContentRoot);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x + index * 360, go.transform.localPosition.y, go.transform.localPosition.z);

                go.GetComponent<MyDragCamera>().RelatedCamera = m_myTongSkillListCamera.GetComponent<Camera>();
                go.transform.Find("MyTongSkillListGridUpgradeBtn").gameObject.AddComponent<TongButton>().id = index;

                go.transform.Find("MyTongSkillListGridIcon").GetComponent<UISprite>().spriteName = m_myTongSkillDataList[index].icon;
                go.transform.Find("MyTongSkillListGridBanner").Find("MyTongSkillListGridBannerText").GetComponent<UILabel>().text = m_myTongSkillDataList[index].name;

                go.transform.Find("MyTongSkillListGridStarList").GetComponent<UIStarList>().SetStarNum(m_myTongSkillDataList[index].starNum);

                Transform reward = go.transform.Find("MyTongSkillListGridReward");
                Transform reward1 = reward.Find("MyTongSkillListGridReward1");
                Transform reward2 = reward.Find("MyTongSkillListGridReward2");
                reward1.Find("MyTongSkillListGridReward1Lbl").GetComponent<UILabel>().text = m_myTongSkillDataList[index].effect1;
                reward2.Find("MyTongSkillListGridReward2Lbl").GetComponent<UILabel>().text = m_myTongSkillDataList[index].effect2;

                m_myTongSkillListObjs.Add(go);
            });

        int temp = (m_myTongSkillDataList.Count - 1);
        if (temp < 0) temp = 0;
        int pageNum = (int)(temp / 3) + 1;
        Mogo.Util.LoggerHelper.Debug("pageNum:" + pageNum);
        ResetListCamera(pageNum, m_myTongSkillListCamera, m_myTongSkillListCameraBegin, m_myTongSkillListContentRoot, true, 360, 3);

    }


    /// <summary>
    /// 显示龙晶系统
    /// </summary>
    /// <param name="leftTimes"></param>
    /// <param name="progress"></param>
    /// <param name="maxProgress"></param>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <param name="num3"></param>
    public void ShowDragonPower(string leftTimes, int progress, int maxProgress, int num1, int num2, int num3)
    {
        m_myTongDragonLeftTimeLbl.text = leftTimes;
        m_myTongDragonProgressBar.SetProgress(progress, maxProgress);

        m_myTongDragonCostlbl1.text = "X" + num1;
        m_myTongDragonCostlbl2.text = "X" + num2;
        m_myTongDragonCostlbl3.text = "X" + num3;
        m_myTong.gameObject.SetActive(true);
        m_myTongDragon.gameObject.SetActive(true);
    }

    #endregion 我的公会


    public void OnButtonClick(string name, int id)
    {
        m_buttonAction[name](id);
    }
}