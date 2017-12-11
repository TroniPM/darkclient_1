using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using System;

public class ChooseCharacterGridData
{
    public string name;
    public string level;
    public string defautText;
    public string headImg;
}

public class JobAttrGridData
{
    public string attrName;
    public int level;
}

public class ChooseServerGridData
{
    public string serverName;
    public ServerType serverStatus;
}

public class NewLoginUIViewManager : MogoUIBehaviour
{
    private static NewLoginUIViewManager m_instance;
    public static NewLoginUIViewManager Instance { get { return NewLoginUIViewManager.m_instance; } }   

    private UILabel m_lblCreateCharacterText;
    private UILabel m_lblJobName;
    private UILabel m_lblJobInfo;
    private UIInput m_lblCharacterNameInput;
    private UILabel m_lblRecommendServerName;
    private GameObject m_goCreateCharacterDetailUIEnterBtn;
    private UILabel m_lblEnter;
    private UISprite m_spCreateCharacterDetailUIEnterBtnBGUp;

    private GameObject m_goCharacterNameInput;
    private GameObject m_goRandomBtn;

    private GameObject m_goChooseCharcterGridList;
    private GameObject m_goJobAttrList;
    private GameObject m_goChooseServerGridList;

    private GameObject m_goCurrentUI;
    private GameObject m_goChooseServerUI;
    private GameObject m_goChooseCharacterUI;
    private GameObject m_goCreateCharacterDetailUI;
    private GameObject m_goCreateCharacterUI;
    private GameObject m_goRecommendServerUI;
    private GameObject m_goCreateCharacterUIBackBtn;
    private GameObject m_goChooseServerGridPageList;

    private MogoTwoStatusButton m_MTBChooseCharacterUIServer;
    private MogoTwoStatusButton m_MTBCreateCharacterUIServer;
    private MogoTwoStatusButton m_MTBLatelyLog0;
    private MogoTwoStatusButton m_MTBLatelyLog1;

    private List<GameObject> m_listChooseServerGridPage = new List<GameObject>();
    private Camera m_camChooseServerGridList;
    private MyDragableCamera m_dragableCameraChooseServerGridList;

    private int m_iCurrentServerGridPage = 1;

    private List<GameObject> m_listChooseCharacterGrid = new List<GameObject>();
    private List<GameObject> m_listJobAttr = new List<GameObject>();
    private List<GameObject> m_listChooseServerGrid = new List<GameObject>();

    private GameObject m_goCreateCharacterDetailUIJobIconList;
  
    const int CHOOSECHARACTERGRIDHEIGHT = 110;
    const int JOBATTRGRIDHEIGHT = 70;
    const int CHOOSESERVERGRIDHEIGHT = 85;
    const int CHOOSESERVERGRIDWIDTH = 410;
    const int CHOOSESERVERGRIDPAGEWIDTH = 1230;

    public Action ENTERGAMEBTNUP;
    public Action DELETECHARACTERBTNUP;
    public Action ChooseCharacterUIServerBtnUp;
    public Action ChooseServerUIBackBtnUp;
    public Action CreateCharacterDetailUIEnterBtnUp;
    public Action CreateCharacterDetailUIBackBtnUp;
    public Action CreateCharacterUIBackBtnUp;

    public Action<int> CreateCharacterDetailUIJobIconUp;
    /// <summary>
    /// 1�ң�-1��
    /// </summary>
    public Action<int> CreateCharacterDetailUISwitch;


    private int m_iChooseServerPageNum = 0;
    private bool inited = false;

    UISprite m_spResCtrl;
    UIAtlas m_atlasCanRelease;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        Camera cam = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_myTransform.Find(m_widgetToFullName["ChooseCharacterUIBottomLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["ChooseCharacterUIBottomRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["ChooseCharacterUIRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["ChooseCharacterUITop"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["ChooseCharacterUITopRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIBottomLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUITopLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIBottomRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUILeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUITop"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterUIBottomRight"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterUITop"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["ChooseServerUITop"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["ChooseServerUIBottomLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["ChooseServerUITopLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterUIBottomLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterUITopLeft"]).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;

        m_camChooseServerGridList = m_myTransform.Find(m_widgetToFullName["ChooseServerUIServerGridListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camChooseServerGridList.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = cam;

        m_dragableCameraChooseServerGridList = m_camChooseServerGridList.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragableCameraChooseServerGridList.LeftArrow = FindTransform("ChooseServerUIArrowL").gameObject;
        m_dragableCameraChooseServerGridList.RightArrow = FindTransform("ChooseServerUIArrowR").gameObject;

        m_lblCreateCharacterText = m_myTransform.Find(m_widgetToFullName["CreateCharacterUITitleText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblJobName = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobInfoName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblJobInfo = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobInfoDetail"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblCharacterNameInput = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIInput"]).GetComponentsInChildren<UIInput>(true)[0];
        m_lblRecommendServerName = m_myTransform.Find(m_widgetToFullName["RecommendServerUIServerName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goCreateCharacterDetailUIEnterBtn = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIEnterBtn"]).gameObject;
        m_lblEnter = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIEnterBtnText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_spCreateCharacterDetailUIEnterBtnBGUp = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIEnterBtnBGUp"]).GetComponentsInChildren<UISprite>(true)[0];

        m_goCharacterNameInput = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIInput"]).gameObject;
        m_goRandomBtn = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIRandomBtn"]).gameObject;

        m_goChooseCharcterGridList = m_myTransform.Find(m_widgetToFullName["ChooseCharacterUIList"]).gameObject;
        m_goJobAttrList = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobAttrList"]).gameObject;
        m_goChooseServerGridList = m_myTransform.Find(m_widgetToFullName["ChooseServerUIServerGridList"]).gameObject;
        m_goCreateCharacterUIBackBtn = m_myTransform.Find(m_widgetToFullName["CreateCharacterUIBackBtn"]).gameObject;

        m_goChooseCharacterUI = m_myTransform.Find("ChooseCharacterUI").gameObject;
        m_goChooseServerUI = m_myTransform.Find("ChooseServerUI").gameObject;
        m_goCreateCharacterDetailUI = m_myTransform.Find("CreateCharacterDetailUI").gameObject;
        m_goCreateCharacterUI = m_myTransform.Find("CreateCharacterUI").gameObject;
        m_goRecommendServerUI = m_myTransform.Find("RecommendServerUI").gameObject;
        m_goChooseServerGridPageList = m_myTransform.Find(m_widgetToFullName["ChooseServerPageList"]).gameObject;

        m_MTBChooseCharacterUIServer = m_myTransform.Find("ChooseCharacterUI/ChooseCharacterUIBottomLeft/ChooseCharcterUIServerBtn").GetComponentsInChildren<MogoTwoStatusButton>(true)[0];
        m_MTBCreateCharacterUIServer = m_myTransform.Find("CreateCharacterUI/CreateCharacterUIBottomRight/CreateCharacterUIServerBtn").GetComponentsInChildren<MogoTwoStatusButton>(true)[0];
        m_MTBLatelyLog0 = m_myTransform.Find("ChooseServerUI/ChooseServerUILatelyLogBtn0").GetComponentsInChildren<MogoTwoStatusButton>(true)[0];
        m_MTBLatelyLog1 = m_myTransform.Find("ChooseServerUI/ChooseServerUILatelyLogBtn1").GetComponentsInChildren<MogoTwoStatusButton>(true)[0];

        m_goCreateCharacterDetailUIJobIconList = m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobIconList"]).gameObject;

        m_spResCtrl = m_myTransform.Find(m_widgetToFullName["NewLoginUIResCtrl"]).GetComponentsInChildren<UISprite>(true)[0];

        m_atlasCanRelease = m_spResCtrl.atlas;

        m_goCurrentUI = m_goChooseCharacterUI;

        for (int i = 0; i < 4; ++i)
        {
            AssetCacheMgr.GetUIInstance("ChooseCharcterUIGrid.prefab", (prefab, id, go) =>
            {
                GameObject obj = (GameObject)go;

                obj.transform.parent = m_goChooseCharcterGridList.transform;

                obj.transform.localPosition = new Vector3(0, -m_listChooseCharacterGrid.Count * CHOOSECHARACTERGRIDHEIGHT, 0);
                obj.transform.localScale = new Vector3(1, 1, 1);
                //obj.GetComponentsInChildren<ChooseCharacterUIGrid>(true)[0].Id = m_listChooseCharacterGrid.Count;
                obj.AddComponent<ChooseCharacterUIGrid>().Id = m_listChooseCharacterGrid.Count;
                m_goChooseCharcterGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);

                m_listChooseCharacterGrid.Add(obj);

                if (m_listChooseCharacterGrid.Count == 4)
                {
                    TruelyFillChooseCharacterGridData();
                    TruelySetCharacterGridDown();
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                }
            });
        }



        for (int i = 0; i < 3; ++i)
        {
            AssetCacheMgr.GetUIInstance("CreateCharacterDetailUIJobAttr.prefab", (prefab, id, go) =>
            {
                GameObject obj = (GameObject)go;

                obj.transform.parent = m_goJobAttrList.transform;

                obj.transform.localPosition = new Vector3(0, -m_listJobAttr.Count * JOBATTRGRIDHEIGHT, 0);
                obj.transform.localScale = new Vector3(1, 1, 1);

                obj.AddComponent<CreateCharacterDetailUIJobAttr>();

                m_listJobAttr.Add(obj);
            });
        }

        Initialize();
        inited = true;
    }

    void Initialize()
    {
        NewLoginUILogicManager.Instance.Initialize();

        m_myTransform.Find(m_widgetToFullName["CreateCharacterUIServerBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharcterUIServerBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseCharacterUIEnterBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseCharacterUIEnterBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseCharcterUIDeleteBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseCharacterUIDeleteBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseCharcterUIServerBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseCharacterUIServerBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseCharcterUICommunityBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseCharacterCommunityBtnUp;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIBackBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIBackBtnUp;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIRandomBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIRandomBtnUp;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIEnterBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIEnterBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseServerUIBackBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseServerUIBackBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseServerUILatelyLogBtn0"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseServerUILatelyLogBtn0Up;
        m_myTransform.Find(m_widgetToFullName["ChooseServerUILatelyLogBtn1"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChooseServerUILatelyLogBtn1Up;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterUIBackBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterUIBackBtnUp;
        m_myTransform.Find(m_widgetToFullName["RecommendServerUIEnter"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnRecommendServerEnterUp;
        m_myTransform.Find(m_widgetToFullName["RecommendServerUISwitch"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnRecommendServerSwitchUp;

        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobIcon1"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIJobIcon1Up;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobIcon2"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIJobIcon2Up;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobIcon3"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIJobIcon3Up;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobIcon4"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCreateCharacterDetailUIJobIcon4Up;

        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIRightDragArea"]).GetComponentsInChildren<MogoButton>(true)[0].dragOverHandler += OnCreateCharacterDetailUISwitch;



    }

    private void OnCreateCharacterDetailUISwitch(Vector2 vec)
    {
        if (CreateCharacterDetailUISwitch != null)
            CreateCharacterDetailUISwitch(vec.x > 0 ? 1 : -1);
    }

    public void Release()
    {
        inited = false;
        NewLoginUILogicManager.Instance.Release();



        m_myTransform.Find(m_widgetToFullName["CreateCharacterUIServerBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCreateCharcterUIServerBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseCharacterUIEnterBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnChooseCharacterUIEnterBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseCharcterUIDeleteBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnChooseCharacterUIDeleteBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseCharcterUIServerBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnChooseCharacterUIServerBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseCharcterUICommunityBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnChooseCharacterCommunityBtnUp;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIBackBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCreateCharacterDetailUIBackBtnUp;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIRandomBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCreateCharacterDetailUIRandomBtnUp;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIEnterBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCreateCharacterDetailUIEnterBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseServerUIBackBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnChooseServerUIBackBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChooseServerUILatelyLogBtn0"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnChooseServerUILatelyLogBtn0Up;
        m_myTransform.Find(m_widgetToFullName["ChooseServerUILatelyLogBtn1"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnChooseServerUILatelyLogBtn1Up;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterUIBackBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCreateCharacterUIBackBtnUp;
        m_myTransform.Find(m_widgetToFullName["RecommendServerUIEnter"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnRecommendServerEnterUp;
        m_myTransform.Find(m_widgetToFullName["RecommendServerUISwitch"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnRecommendServerSwitchUp;

        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobIcon1"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCreateCharacterDetailUIJobIcon1Up;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobIcon2"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCreateCharacterDetailUIJobIcon2Up;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobIcon3"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCreateCharacterDetailUIJobIcon3Up;
        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIJobIcon4"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCreateCharacterDetailUIJobIcon4Up;

        m_myTransform.Find(m_widgetToFullName["CreateCharacterDetailUIRightDragArea"]).GetComponentsInChildren<MogoButton>(true)[0].dragOverHandler -= OnCreateCharacterDetailUISwitch;


        for (int i = 0; i < m_listChooseCharacterGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listChooseCharacterGrid[i]);
            m_listChooseCharacterGrid[i] = null;
        }

        for (int i = 0; i < m_listJobAttr.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listJobAttr[i]);
            m_listJobAttr[i] = null;
        }

        for (int i = 0; i < m_listChooseServerGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listChooseServerGrid[i]);
            m_listChooseServerGrid[i] = null;
        }

        for (int i = 0; i < m_listChooseServerGridPage.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listChooseServerGridPage[i]);
            m_listChooseServerGridPage[i] = null;
        }
    }

    void OnCreateCharcterUIServerBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("CreateCharcterServerBtnUp");
        if (ChooseCharacterUIServerBtnUp != null)
            ChooseCharacterUIServerBtnUp();
    }

    void OnChooseCharacterUIEnterBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("EnterBtnUp");

        if (ENTERGAMEBTNUP != null)
            ENTERGAMEBTNUP();
    }

    void OnChooseCharacterUIDeleteBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("DeleteBtnUp");

        if (DELETECHARACTERBTNUP != null)
            DELETECHARACTERBTNUP();
    }

    void OnChooseCharacterUIServerBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("ChooseCharcterServerBtnUp");
        if (ChooseCharacterUIServerBtnUp != null)
            ChooseCharacterUIServerBtnUp();
    }

    void OnChooseCharacterCommunityBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("CommunityBtnUp");
    }

    void OnCreateCharacterDetailUIBackBtnUp()
    {
        if (CreateCharacterDetailUIBackBtnUp != null)
            CreateCharacterDetailUIBackBtnUp();
    }

    void OnCreateCharacterDetailUIRandomBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("RandomBtnUp");
        EventDispatcher.TriggerEvent<byte>(Events.UIAccountEvent.OnGetRandomName, (byte)NewLoginUILogicManager.Instance.m_lastSelectedCharacter.vocation);
    }

    void OnCreateCharacterDetailUIEnterBtnUp()
    {
        if (CreateCharacterDetailUIEnterBtnUp != null)
            CreateCharacterDetailUIEnterBtnUp();
    }

    void OnChooseServerUIBackBtnUp()
    {
        if (ChooseServerUIBackBtnUp != null)
            ChooseServerUIBackBtnUp();
    }

    void OnChooseServerUILatelyLogBtn0Up()
    {
        Mogo.Util.LoggerHelper.Debug("LatelyBtn0Up");
    }

    void OnChooseServerUILatelyLogBtn1Up()
    {
        Mogo.Util.LoggerHelper.Debug("LatelyBtn1Up");
    }

    void OnCreateCharacterUIBackBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("BackBtnUp");
        if (CreateCharacterUIBackBtnUp != null)
            CreateCharacterUIBackBtnUp();
    }

    void OnRecommendServerEnterUp()
    {
        Mogo.Util.LoggerHelper.Debug("EnterUp");
    }

    void OnRecommendServerSwitchUp()
    {
        Mogo.Util.LoggerHelper.Debug("SwitchUP");
    }

    void OnCreateCharacterDetailUIJobIcon1Up()
    {
        if (CreateCharacterDetailUIJobIconUp != null)
            CreateCharacterDetailUIJobIconUp(1);
    }

    void OnCreateCharacterDetailUIJobIcon2Up()
    {
        if (CreateCharacterDetailUIJobIconUp != null)
            CreateCharacterDetailUIJobIconUp(2);
    }

    void OnCreateCharacterDetailUIJobIcon3Up()
    {
        if (CreateCharacterDetailUIJobIconUp != null)
            CreateCharacterDetailUIJobIconUp(3);
    }

    void OnCreateCharacterDetailUIJobIcon4Up()
    {
        if (CreateCharacterDetailUIJobIconUp != null)
            CreateCharacterDetailUIJobIconUp(4);
    }

    private void TruelyFillChooseCharacterGridData()
    {
        if (!inited)
        {
            TimerHeap.AddTimer(100, 0, TruelyFillChooseCharacterGridData);
            return;
        }
        for (int i = 0; i < 4; ++i)
        {
            ChooseCharacterUIGrid cg = m_listChooseCharacterGrid[i].GetComponentsInChildren<ChooseCharacterUIGrid>(true)[0];

            if (i < m_listCCGD.Count)
            {
                cg.ShowAsEnable(true);

                cg.Name = m_listCCGD[i].name;
                cg.Level = m_listCCGD[i].level;
                cg.HeadImgName = m_listCCGD[i].headImg;
                cg.DefaultText = m_listCCGD[i].defautText;
            }
            else
            {
                cg.ShowAsEnable(false);
            }
        }
    }

    List<ChooseCharacterGridData> m_listCCGD = new List<ChooseCharacterGridData>();
    public void FillChooseCharacterGridData(List<ChooseCharacterGridData> list)
    {
        m_listCCGD = list;

        if (m_listChooseCharacterGrid != null && m_listChooseCharacterGrid.Count != 0)
        {
            TruelyFillChooseCharacterGridData();
        }
    }

    public void FillJobAttrGridData(List<JobAttrGridData> list)
    {
        for (int i = 0; i < 3; ++i)
        {
            CreateCharacterDetailUIJobAttr ca = m_listJobAttr[i].GetComponentsInChildren<CreateCharacterDetailUIJobAttr>(true)[0];

            ca.AttrName = list[i].attrName;
            ca.SignNum = list[i].level;
        }
    }

    int m_iCurrentGridDown = 0;
    private void TruelySetCharacterGridDown()
    {
        m_goChooseCharcterGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(m_iCurrentGridDown);
    }
    public void SetCharacterGridDown(int gridId)
    {
        if (!inited)
        {
            TimerHeap.AddTimer<int>(100, 0, SetCharacterGridDown, gridId);
            return;
        }
        m_iCurrentGridDown = gridId;
        if (m_goChooseCharcterGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Count > 0)
        {
            m_goChooseCharcterGridList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(m_iCurrentGridDown);
        }
    }

    public void SetCreateCharacterJobDetailName(string name)
    {
        m_lblJobName.text = name;
    }

    public void SetCreateCharacterJobDetailInfo(string info)
    {
        m_lblJobInfo.text = info;
    }

    public string GetCharacterNameInputText()
    {
        return m_lblCharacterNameInput.text;
    }

    public void SetCharacterNameInputText(string text)
    {
        LoggerHelper.Debug(text);
        m_lblCharacterNameInput.text = text;
    }

    public void SetChooseCharacterServerBtnName(string name)
    {
        if (m_MTBChooseCharacterUIServer == null)
        {
            return;
        }
        m_MTBChooseCharacterUIServer.SetButtonText(name);
    }

    public void SetCreateCharacterServerBtnName(string name)
    {
        m_MTBCreateCharacterUIServer.SetButtonText(name);
    }

    public void SetRecommendServerName(string name)
    {
        m_lblRecommendServerName.text = name;
    }

    public void AddChooseServerGrid(ChooseServerGridData cd)
    {
        AssetCacheMgr.GetUIInstance("ChooseServerUIServerBtn.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.transform.parent = m_goChooseServerGridList.transform;

            int page = m_listChooseServerGrid.Count / 12;
            int row = (m_listChooseServerGrid.Count % 12) / 3;
            int col = (m_listChooseServerGrid.Count % 12) % 3;

            if (page + 1 > m_iCurrentServerGridPage)
            {
                GameObject trans = new GameObject();

                trans.transform.parent = m_goChooseServerGridList.transform;
                trans.transform.localPosition = new Vector3(page * CHOOSESERVERGRIDPAGEWIDTH + CHOOSESERVERGRIDWIDTH, -125, 0);
                trans.transform.localEulerAngles = Vector3.zero;
                trans.transform.localScale = new Vector3(1, 1, 1);

                m_dragableCameraChooseServerGridList.transformList.Add(trans.transform);
                m_dragableCameraChooseServerGridList.SetArrow();

                ++m_iCurrentServerGridPage;                
            }

            obj.transform.localPosition = new Vector3(page * CHOOSESERVERGRIDPAGEWIDTH + col * CHOOSESERVERGRIDWIDTH, -row * CHOOSESERVERGRIDHEIGHT, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.AddComponent<MyDragCamera>().RelatedCamera = m_camChooseServerGridList;

            ChooseServerUIGrid sg = obj.AddComponent<ChooseServerUIGrid>();

            sg.Id = m_listChooseServerGrid.Count;
            sg.ServerName = cd.serverName;
            sg.ServerStatus = cd.serverStatus;

            m_listChooseServerGrid.Add(obj);

            if (m_iChooseServerPageNum < page + 1)
            {
                ++m_iChooseServerPageNum;

                int num = m_iChooseServerPageNum;

                AssetCacheMgr.GetUIInstance("ChooseServerPage.prefab", (prefabPage, idPage, goPage) =>
                {
                    GameObject objPage = (GameObject)goPage;

                    objPage.transform.parent = m_goChooseServerGridPageList.transform;
                    objPage.transform.localPosition = new Vector3((num - 1) * 40, 0, 0);
                    objPage.transform.localScale = Vector3.one;
                    m_dragableCameraChooseServerGridList.ListPageDown.Add(objPage.GetComponentsInChildren<UISprite>(true)[1].gameObject);
                    m_goChooseServerGridPageList.transform.localPosition += new Vector3(-20 * (num - 1), 0, 0);

                    if (num == 1)
                    {
                        objPage.GetComponentsInChildren<UISprite>(true)[1].gameObject.SetActive(true);
                    }

                    m_listChooseServerGridPage.Add(objPage);                                     
                });
            }
        });


    }

    public void ShowLatelyServer(string name, int id, bool isShow)
    {
        if (id == 0)
        {
            m_MTBLatelyLog0.SetButtonText(name);
            m_MTBLatelyLog0.gameObject.SetActive(isShow);
        }
        else
        {
            m_MTBLatelyLog1.SetButtonText(name);
            m_MTBLatelyLog1.gameObject.SetActive(isShow);
        }
    }

    public void ClearServerList()
    {
        for (int i = 0; i < m_listChooseServerGrid.Count; ++i)
        {
            int index = i;
            AssetCacheMgr.ReleaseInstance(m_listChooseServerGrid[index]);
        }

        m_listChooseServerGrid.Clear();
        m_camChooseServerGridList.transform.localPosition = new Vector3(410, -125, 0);
    }

    public void ShowCreateCharacterUIBackBtn(bool isShow)
    {
        m_goCreateCharacterUIBackBtn.SetActive(isShow);
    }

    public void ShowChooseCharacterUI()
    {
        if (!inited)
        {
            TimerHeap.AddTimer(100, 0, ShowChooseCharacterUI);
            return;
        }
        m_goCurrentUI.SetActive(false);
        m_goCurrentUI = m_goChooseCharacterUI;
        m_goCurrentUI.SetActive(true);
    }

    public void ShowChooseServerUI()
    {
        m_goCurrentUI.SetActive(false);
        m_goCurrentUI = m_goChooseServerUI;
        m_goCurrentUI.SetActive(true);
    }

    public void ShowCreateCharacterDetailUI()
    {
        m_goCurrentUI.SetActive(false);
        m_goCurrentUI = m_goCreateCharacterDetailUI;
        m_goCurrentUI.SetActive(true);
    }

    public void ShowCreateCharacterUI()
    {
        m_goCurrentUI.SetActive(false);
        m_goCurrentUI = m_goCreateCharacterUI;
        m_goCurrentUI.SetActive(true);
    }

    public void ShowRecommendServerUI()
    {
        m_goCurrentUI.SetActive(false);
        m_goCurrentUI = m_goRecommendServerUI;
        m_goCurrentUI.SetActive(true);
    }

    public void SetEnterButtonLabel(string str, bool enable)
    {
        m_lblEnter.text = str;

        if (enable)
        {
            m_spCreateCharacterDetailUIEnterBtnBGUp.ShowAsWhiteBlack(false);
            m_goCreateCharacterDetailUIEnterBtn.GetComponentsInChildren<BoxCollider>(true)[0].enabled = true;
        }
        else
        {
            m_spCreateCharacterDetailUIEnterBtnBGUp.ShowAsWhiteBlack(true);
            m_goCreateCharacterDetailUIEnterBtn.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;
        }
    }

    public void ShowDiceAndName(bool isShow)
    {
        m_goCharacterNameInput.SetActive(isShow);
        m_goRandomBtn.SetActive(isShow);
    }

    public void SelectCreateCharacterDetailUIJobIcon(int pos)
    {
        //Debug.LogError("pos:" + pos);
        m_goCreateCharacterDetailUIJobIconList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(pos - 1);
    }

    #region ����򿪺͹ر�

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!SystemSwitch.DestroyResource)
        {
            return;
        }

        //if (m_atlasCanRelease != null)
        //{
        //    if (m_atlasCanRelease.spriteMaterial.mainTexture == null)
        //    {
        //        AssetCacheMgr.GetUIResource("MogoLoginUI.png", (obj) =>
        //        {

        //            m_atlasCanRelease.spriteMaterial.mainTexture = (Texture)obj;
        //            m_atlasCanRelease.MarkAsDirty();
        //        });
        //    }
        //}
    }

    public void DestroyUIAndResources()
    {
        //if (true)
        //{
            MogoUIManager.Instance.DestroyNewLoginUI();
        //}
        if (!SystemSwitch.DestroyResource)
        {
            return;
        }

        //m_atlasCanRelease.spriteMaterial.mainTexture = null;
        //AssetCacheMgr.ReleaseResourceImmediate("MogoLoginUI.png");
    }

    void OnDisable()
    {
        DestroyUIAndResources();
    }

    #endregion
}
