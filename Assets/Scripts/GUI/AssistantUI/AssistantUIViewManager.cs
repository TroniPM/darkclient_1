using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class AssistantUIViewManager : MonoBehaviour
{

    private static AssistantUIViewManager m_instance;

    public static AssistantUIViewManager Instance
    {
        get
        {
            return AssistantUIViewManager.m_instance;
        }
    }

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    public Action<int> ASSISTANTSKILLGRIDUP;
    public Action<int> ASSISTANTMINTMARKGRIDUP;

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        m_widgetToFullName.Add(widgetName, fullName);
    }

    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";

        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != m_myTransform)
            {
                fullName = "/" + fullName;
            }

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }


    private Transform m_myTransform;

    public AssistantUISkillGridTip SkillGridTip;
    public AssistantUIMintmarkGridTip MintmarkGridTip;
    public AssistantUISkillGrowTip SkillGrowTip;
    public AssistantUIMintmarkGrowTip MintmarkGrowTip;

    public string AssistantUIModelName;

    GameObject m_goElementMintmarkTip;
    GameObject m_goSkillsContractTip;
    GameObject m_goSkillsContractGrowTip;
    GameObject m_goElementMintmarkGrowTip;
    GameObject m_goSkillsContractSkillList;
    GameObject m_goElementMintmarkList;
    Vector3 m_vec3AssistantUIIconListStartPos;
    Camera m_camAssistantUISkillContractGridList;
    Camera m_camAssistantUIMintMarkGridList;
    List<AssistantUISkillGrid> m_listSkillsContractGrid = new List<AssistantUISkillGrid>();
    List<AssistantUIMintmarkGrid> m_listElementMintmarkGrid = new List<AssistantUIMintmarkGrid>();

    GameObject m_goRTTModelTopLeft;
    GameObject m_goRTTModelBottomRight;

    GameObject m_goRTTModel;

    UILabel m_lblSkillsContractMoney;
    UILabel m_lblSkillsContractLevel;
    UILabel m_lblSkillsContractCost;

    UILabel m_lblElementMintmarkMoney;
    UILabel m_lblElementMintmarkLevel;
    UILabel m_lblElementMintmarkCost;

    UISprite m_spInitativeFG;
    UISprite m_spPassive0FG;
    UISprite m_spPassive1FG;

    GameObject m_goGlobleDialogMask;

    AssistantUIButton m_goSkillsContractInitative;
    AssistantUIButton m_goSkillsContractPassive0;
    AssistantUIButton m_goSkillsContractPassive1;

    AssistantUIButton[] m_goMintmarkBodyGrid = new AssistantUIButton[5];

    public Action SKILLSCONTRACTGROWTIPBTNUP;
    public Action CONTRACTGROWBTNUP;
    public Action ELEMENTMINTMARKGROWTIPBTNUP;
    public Action MINTMARKGROWBTNUP;

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    //public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    void OnAssistantUICloseButtonUp(int i)
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
        ShowAssistantRTTModel(false);
    }

    void OnElementMintmarkButtonUp(int i)
    {
        MogoUIManager.Instance.SwitchElementHintmarkUI();
        ShowElementMintmarkGridList(true);
        ShowSkillsContractGridList(false);
        m_camAssistantUIMintMarkGridList.gameObject.SetActive(true);
        m_camAssistantUISkillContractGridList.gameObject.SetActive(false);
    }

    void OnSkillsContrackButtonUp(int i)
    {
        MogoUIManager.Instance.SwitchSkillsContractUI();
        ShowElementMintmarkGridList(false);
        ShowSkillsContractGridList(true);
        m_camAssistantUIMintMarkGridList.gameObject.SetActive(false);
        m_camAssistantUISkillContractGridList.gameObject.SetActive(true);
    }

    void OnSkillsContractInitativeUp(int i)
    {
        LoggerHelper.Debug("InitativeUp");
    }

    void OnSkillsContractPassive0Up(int i)
    {
        LoggerHelper.Debug("Passive0Up");
    }

    void OnSkillsContractPassive1Up(int i)
    {
        LoggerHelper.Debug("Passive1Up");
    }

    void OnElementMintmarkTipCloseUp(int i)
    {
        ShowElemtMintmarkTip(false);
    }

    void OnSkillsContractTipCloseUp(int i)
    {
        ShowSkillsContractTip(false);
    }

    void OnAssistantUISkillGridUp(int id)
    {
        if (ASSISTANTSKILLGRIDUP != null)
            ASSISTANTSKILLGRIDUP(id);
    }

    void OnAssistantUIMintmarkGridUp(int id)
    {
        if (ASSISTANTMINTMARKGRIDUP != null)
            ASSISTANTMINTMARKGRIDUP(id);
    }

    void OnSkillsContractGrowTipCloseUp(int i)
    {
        ShowSkillsContractGrowTip(false);
    }

    void OnSkillsContractGrowButtonUp(int i)
    {
        if (SKILLSCONTRACTGROWTIPBTNUP != null)
        {
            SKILLSCONTRACTGROWTIPBTNUP();
        }
    }

    void OnContractGrowButtonUp(int i)
    {
        if (CONTRACTGROWBTNUP != null)
            CONTRACTGROWBTNUP();
    }


    void OnElemtMintmarkElementUp(int id)
    {
        LoggerHelper.Debug("ElementUp" + id);
    }

    void OnElementMintmarkGrowButtonUp(int i)
    {

        if (ELEMENTMINTMARKGROWTIPBTNUP != null)
            ELEMENTMINTMARKGROWTIPBTNUP();
    }

    void OnMintmarkGrowButtonUp(int i)
    {
        if (MINTMARKGROWBTNUP != null)
            MINTMARKGROWBTNUP();
    }

    void OnElementMintmarkGrowTipCloseUp(int i)
    {
        ShowElementMintmarkGrowTip(false);
    }

    public void ShowElemtMintmarkTip(bool isShow)
    {
        m_goElementMintmarkTip.SetActive(isShow);
        m_goGlobleDialogMask.SetActive(isShow);
    }

    public void ShowSkillsContractTip(bool isShow)
    {
        m_goSkillsContractTip.SetActive(isShow);
        m_goGlobleDialogMask.SetActive(isShow);
    }

    public void AddSkillContractGrid(bool isPassive, string iconName)
    {
        string prefabName = "";

        if (isPassive)
        {
            prefabName = "AssistantUIGridSphere.prefab";
        }
        else
        {
            prefabName = "AssistantUIGridQuad.prefab";
        }

        AssetCacheMgr.GetUIInstance(prefabName, (prefab, guid, go) =>
        {
            GameObject obj = (GameObject)go;
            AssistantUISkillGrid grid = obj.AddComponent<AssistantUISkillGrid>();
            obj.transform.parent = m_goSkillsContractSkillList.transform;
            obj.transform.localPosition = new Vector3(m_vec3AssistantUIIconListStartPos.x + m_listSkillsContractGrid.Count * 0.11f, 0, 0);
            obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
            obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camAssistantUISkillContractGridList;
            grid.SetGridIconName(iconName);
            grid.Id = m_listSkillsContractGrid.Count;

            m_listSkillsContractGrid.Add(grid);


            m_camAssistantUISkillContractGridList.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX = 0.385f + (m_listSkillsContractGrid.Count - 8) * 0.11f;

            EventDispatcher.TriggerEvent<int>("LoadSkillContractGridDone", grid.Id);
        });
    }

    public void AddElementMintmarkGrid(string iconName)
    {

        AssetCacheMgr.GetUIInstance("AssistantUIGridSphere.prefab", (prefab, guid, go) =>
        {
            GameObject obj = (GameObject)go;
            AssistantUIMintmarkGrid grid = obj.AddComponent<AssistantUIMintmarkGrid>();
            obj.transform.parent = m_goElementMintmarkList.transform;
            obj.transform.localPosition = new Vector3(m_vec3AssistantUIIconListStartPos.x + m_listElementMintmarkGrid.Count * 0.11f, 0, 0);
            obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
            obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camAssistantUIMintMarkGridList;

            grid.SetGridIconName(iconName);
            grid.Id = m_listElementMintmarkGrid.Count;

            m_listElementMintmarkGrid.Add(grid);


            m_camAssistantUIMintMarkGridList.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX = 0.385f + (m_listElementMintmarkGrid.Count - 8) * 0.11f;
        });
    }

    public void ClearSkillsContractGridList()
    {
        for (int i = 0; i < m_listSkillsContractGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listSkillsContractGrid[i].gameObject);
        }

        m_listSkillsContractGrid.Clear();
        m_camAssistantUISkillContractGridList.transform.localPosition = m_vec3AssistantUIIconListStartPos;
    }

    public void ClearElementMintmarkList()
    {
        for (int i = 0; i < m_listElementMintmarkGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listElementMintmarkGrid[i].gameObject);
        }

        m_listElementMintmarkGrid.Clear();
        m_camAssistantUIMintMarkGridList.transform.localPosition = m_vec3AssistantUIIconListStartPos;
    }

    //public void SetSkillGridEnable(int id, bool isEnable)
    //{
    //    m_listSkillsContractGrid[id].SetEnable(isEnable);
    //}

    public void ShowSkillsContractGrowTip(bool isShow)
    {
        m_goSkillsContractGrowTip.SetActive(isShow);
        m_goGlobleDialogMask.SetActive(isShow);
    }

    public void ShowElementMintmarkGrowTip(bool isShow)
    {
        m_goElementMintmarkGrowTip.SetActive(isShow);
        m_goGlobleDialogMask.SetActive(isShow);
    }

    public void ShowSkillsContractGridList(bool isShow)
    {
        m_goSkillsContractSkillList.SetActive(isShow);
    }

    public void ShowElementMintmarkGridList(bool isShow)
    {
        m_goElementMintmarkList.SetActive(isShow);
    }

    public void SetSkillsContractMoney(string money)
    {
        m_lblSkillsContractMoney.text = money;
    }

    public void SetSkillsContractLevel(string level)
    {
        m_lblSkillsContractLevel.text = level;
    }

    public void SetElementMintmarkCost(string cost)
    {
        m_lblElementMintmarkCost.text = cost;
    }

    public void SetElementMintmarkMoney(string money)
    {
        m_lblElementMintmarkMoney.text = money;
    }

    public void SetElementMintmarkLevel(string level)
    {
        m_lblElementMintmarkLevel.text = level;
    }

    public void SetSkillsContractCost(string cost)
    {
        m_lblSkillsContractCost.text = cost;
    }

    public void ShowSkillsContractPassive0Lock(bool isShow)
    {
        m_goSkillsContractPassive0.ShowLock(isShow);
    }

    public void ShowSkillsContractPassive1Lock(bool isShow)
    {
        m_goSkillsContractPassive1.ShowLock(isShow);
    }

    public void ShowSkillsContractPassive0Icon(bool isShow)
    {
        m_goSkillsContractPassive0.ShowFG(isShow);
    }

    public void ShowSkillsCOntractPassive1Icon(bool isShow)
    {
        m_goSkillsContractPassive1.ShowFG(isShow);
    }

    public void ShowSkillsContractInitativeLock(bool isShow)
    {
        m_goSkillsContractInitative.ShowLock(isShow);
    }

    public void ShowSkillsContractInitativeIcon(bool isShow)
    {
        m_goSkillsContractInitative.ShowFG(isShow);
    }

    public void ShowMintmarkBodyGridLock(int id, bool isShow)
    {
        m_goMintmarkBodyGrid[id].ShowLock(isShow);
    }

    public void ShowMintmarkBodyGridIcon(int id, bool isShow)
    {
        m_goMintmarkBodyGrid[id].ShowFG(isShow);
    }

    public void SetSkillsContractGridEnable(int gridId, bool isEnable)
    {
        m_listSkillsContractGrid[gridId].SetEnable(isEnable);
    }

    public void SetSkillsContractGridIcon(int gridId, string iconName)
    {
        m_listSkillsContractGrid[gridId].SetGridIconName(iconName);
    }

    public void ShowElementMintmarkGridLock(int id, bool isShow)
    {
        m_listElementMintmarkGrid[id].SetEnable(isShow);
    }

    public void ShowElementMintmarkGridIcon(int id, bool isShow)
    {
        m_listElementMintmarkGrid[id].ShowGridIcon(isShow);
    }

    public void ShowAssistantRTTModel(bool isShow)
    {
        if (isShow)
            MogoFXManager.Instance.AlphaFadeIn(m_goRTTModel, 2);
        else
            MogoFXManager.Instance.AlphaFadeOut(m_goRTTModel,2);

        m_goRTTModel.SetActive(isShow);
    }

    void Awake()
    {

        m_myTransform = transform;

        m_instance = m_myTransform.GetComponentsInChildren<AssistantUIViewManager>(true)[0];

        Initialize();

        FillFullNameData(m_myTransform);

        Camera cam = GameObject.Find("Camera").GetComponentInChildren<Camera>();
        m_myTransform.GetChild(0).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.GetChild(1).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.GetChild(2).GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = cam;
        m_myTransform.Find(m_widgetToFullName["AssistantUISkillContractGridListCamera"]).GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = cam;
        m_myTransform.Find(m_widgetToFullName["AssistantUIMintmarkGridListCamera"]).GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = cam;

        m_goElementMintmarkTip = m_myTransform.Find(m_widgetToFullName["ElementMintmarkTip"]).gameObject;
        MintmarkGridTip = new AssistantUIMintmarkGridTip(m_goElementMintmarkTip);

        m_goSkillsContractGrowTip = m_myTransform.Find(m_widgetToFullName["SkillsContractGrowTip"]).gameObject;
        SkillGrowTip = new AssistantUISkillGrowTip(m_goSkillsContractGrowTip);

        m_goSkillsContractTip = m_myTransform.Find(m_widgetToFullName["SkillsContractTip"]).gameObject;
        SkillGridTip = new AssistantUISkillGridTip(m_goSkillsContractTip);

        m_goElementMintmarkGrowTip = m_myTransform.Find(m_widgetToFullName["ElementMintmarkGrowTip"]).gameObject;
        MintmarkGrowTip = new AssistantUIMintmarkGrowTip(m_goElementMintmarkGrowTip);

        m_goSkillsContractSkillList = m_myTransform.Find(m_widgetToFullName["AssistantUISkillsContractGridList"]).gameObject;
        m_goElementMintmarkList = m_myTransform.Find(m_widgetToFullName["AssistantUIElementMintmarkGridList"]).gameObject;

        m_vec3AssistantUIIconListStartPos = m_myTransform.Find(m_widgetToFullName["AssistantUIGridIconListStartPos"]).transform.localPosition;
        m_camAssistantUISkillContractGridList = m_myTransform.Find(m_widgetToFullName["AssistantUISkillContractGridListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camAssistantUIMintMarkGridList = m_myTransform.Find(m_widgetToFullName["AssistantUIMintmarkGridListCamera"]).GetComponentsInChildren<Camera>(true)[0];

        m_goGlobleDialogMask = m_myTransform.Find(m_widgetToFullName["GlobleDialogBG"]).gameObject;

        m_lblSkillsContractMoney = m_myTransform.Find(m_widgetToFullName["SkillsContractGrowTipMoneyText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillsContractLevel = m_myTransform.Find(m_widgetToFullName["SkillsContractGrowTipLevel"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillsContractCost = m_myTransform.Find(m_widgetToFullName["SkillsContractGrowTipCostNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_lblElementMintmarkMoney = m_myTransform.Find(m_widgetToFullName["ElementMintmarkGrowTipMoneyText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblElementMintmarkLevel = m_myTransform.Find(m_widgetToFullName["ElementMintmarkGrowTipLevel"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblElementMintmarkCost = m_myTransform.Find(m_widgetToFullName["ElementMintmarkGrowTipCostNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_goSkillsContractPassive0 = m_myTransform.Find(m_widgetToFullName["SkillsContractPassive0"]).GetComponentsInChildren<AssistantUIButton>(true)[0];
        m_goSkillsContractPassive1 = m_myTransform.Find(m_widgetToFullName["SkillsContractPassive1"]).GetComponentsInChildren<AssistantUIButton>(true)[0];
        m_goSkillsContractInitative = m_myTransform.Find(m_widgetToFullName["SkillsContractInitative"]).GetComponentsInChildren<AssistantUIButton>(true)[0];

        for (int i = 0; i < 5; ++i)
        {
            m_goMintmarkBodyGrid[i] = m_myTransform.Find(m_widgetToFullName["ElementMintmarkElement" + i]).GetComponentsInChildren<AssistantUIButton>(true)[0];
        }

        m_goRTTModelTopLeft = m_myTransform.Find(m_widgetToFullName["AssistantModelRTTopLeft"]).gameObject;
        m_goRTTModelBottomRight = m_myTransform.Find(m_widgetToFullName["AssistantModelRTBottomRight"]).gameObject;

        AddAssistantModel(AssistantUIModelName);

        AddSkillContractGrid(true, "jl_keyinsuipian");
        AddSkillContractGrid(true, "jl_keyinsuipian");
        AddSkillContractGrid(false, "jl_keyinsuipian");
        AddSkillContractGrid(false, "jl_keyinsuipian");
        AddSkillContractGrid(true, "jl_keyinsuipian");
        AddSkillContractGrid(true, "jl_keyinsuipian");
        AddSkillContractGrid(false, "jl_keyinsuipian");
        AddSkillContractGrid(false, "jl_keyinsuipian");
        AddSkillContractGrid(true, "jl_keyinsuipian");
        AddSkillContractGrid(false, "jl_keyinsuipian");

        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");
        AddElementMintmarkGrid("jl_keyinsuipian");

        
    }

    //void Start()
    //{

    //    gameObject.SetActive(false);


    //}

    void Initialize()
    {
        AssistantUILogicManager.Instance.Initialize();

        AssistantUIDict.ButtonTypeToEventUp.Add("AssistantUICloseButton", OnAssistantUICloseButtonUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ElementMintmarkButton", OnElementMintmarkButtonUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("SkillsContractButton", OnSkillsContrackButtonUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("SkillsContractInitative", OnSkillsContractInitativeUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("SkillsContractPassive0", OnSkillsContractPassive0Up);
        AssistantUIDict.ButtonTypeToEventUp.Add("SkillsContractPassive1", OnSkillsContractPassive1Up);
        AssistantUIDict.ButtonTypeToEventUp.Add("SkillsContractTipClose", OnSkillsContractTipCloseUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ElementMintmarkTipClose", OnElementMintmarkTipCloseUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("SkillsContractGrowTipClose", OnSkillsContractGrowTipCloseUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("SkillsContractGrowButton", OnSkillsContractGrowButtonUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ContractGrowButton", OnContractGrowButtonUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ElementMintmarkElement0", OnElemtMintmarkElementUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ElementMintmarkElement1", OnElemtMintmarkElementUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ElementMintmarkElement2", OnElemtMintmarkElementUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ElementMintmarkElement3", OnElemtMintmarkElementUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ElementMintmarkElement4", OnElemtMintmarkElementUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ElementMintmarkGrowButton", OnElementMintmarkGrowButtonUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("MintmarkGrowButton", OnMintmarkGrowButtonUp);
        AssistantUIDict.ButtonTypeToEventUp.Add("ElementMintmarkGrowTipClose", OnElementMintmarkGrowTipCloseUp);


        EventDispatcher.AddEventListener<int>("AssistantUISkillGridUp", OnAssistantUISkillGridUp);
        EventDispatcher.AddEventListener<int>("AssistantUIMintmarkGridUp", OnAssistantUIMintmarkGridUp);

    }

    public void AddAssistantModel(string name)
    {
        AssetCacheMgr.GetUIInstance(name, (prefab, guid, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.GetComponentsInChildren<CharacterController>(true)[0].enabled = false;
            LoadAssistantUIModel(obj);

           

      
        });
    }

    void LoadAssistantUIModel(GameObject obj)
    {
        if (obj == null)
            return;

        AssetCacheMgr.GetUIInstance("RTTModel.prefab", (prefab, guid, go) =>
        {
            GameObject goTmp = (GameObject)go;
            goTmp.transform.localPosition = new Vector3(1000, 1000, 1000);

            obj.transform.parent = goTmp.transform;
            goTmp.name = "AssistantRTTModel";

            Camera rttCam = goTmp.GetComponentsInChildren<Camera>(true)[0];
            rttCam.transform.localEulerAngles = new Vector3(25, 180, 0);
            rttCam.transform.localPosition = new Vector3(0, 2.5f, 2);
            UIViewport vp = rttCam.GetComponentsInChildren<UIViewport>(true)[0];

            vp.topLeft = m_goRTTModelTopLeft.transform;
            vp.bottomRight = m_goRTTModelBottomRight.transform;
            vp.sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];



            m_goRTTModel = goTmp;
            obj.transform.localPosition = Vector3.zero ; 
            //ShowAssistantRTTModel(false);
        });
    }

    public void Release()
    {
        AssistantUILogicManager.Instance.Release();

        EventDispatcher.RemoveEventListener<int>("AssistantUISkillGridUp", OnAssistantUISkillGridUp);
        EventDispatcher.RemoveEventListener<int>("AssistantUIMintmarkGridUp", OnAssistantUIMintmarkGridUp);

        AssistantUIDict.ButtonTypeToEventUp.Clear();
    }
}
