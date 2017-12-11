using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class RuneUIViewManager : MFUIUnit
{
    private static RuneUIViewManager m_instance;

    public static RuneUIViewManager Instance
    {
        get
        {
       
            return RuneUIViewManager.m_instance;
        }
    }

    private Transform m_myTransform;

    //public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    public Action RUNEUICOMPOSEUP;
    public Action GOTODRAGONUP;
    public Action RUNEUICLOSEUP;

    public Action<int> RUNEUIPACKAGEGRIDUP;
    public Action<int> RUNEUIPACKAGEGRIDUPDOUBLE;
    public Action<int> RUNEUIINSETGRIDUP;
    public Action<int> RUNEUIINSETGRIDUPDOUBLE;

    GameObject[] m_arrInsetGridInfo = new GameObject[11];
    GameObject[] m_arrInsetGridLock = new GameObject[11];

    Transform[] m_arrPackageGrid = new Transform[16];
    Transform[] m_arrInsetGird = new Transform[11];

    UILabel m_lblLifePower;

    GameObject m_goRuneUICursor;

    UITexture m_texInsetBodyBG;

    GameObject m_goRuneUIPackage;

    List<GameObject> m_listRuneItem = new List<GameObject>();
    List<GameObject> m_listRuneInsetItem = new List<GameObject>();

    List<GameObject> m_listRuneInsetLimitSign = new List<GameObject>();

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

    public override void CallWhenCreate()
    {
        SetUIDirty();
        m_lblLifePower = m_myTransform.Find(m_widgetToFullName["RuneUIInsetPowerNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_goRuneUICursor = m_myTransform.Find(m_widgetToFullName["RuneUICursorUVAnim"]).gameObject;
        m_goRuneUIPackage = m_myTransform.Find(m_widgetToFullName["RuneUIPackage"]).gameObject;

        for (int i = 0; i < 11; ++i)
        {
            m_arrInsetGridInfo[i] = m_myTransform.Find(m_widgetToFullName["RuneUIInsetGridInfo" + i]).gameObject;
            m_arrInsetGridLock[i] = m_myTransform.Find(m_widgetToFullName["RuneUIInsetGridLock" + i]).gameObject;
            m_arrInsetGird[i] = m_myTransform.Find(m_widgetToFullName["RuneUIInsetGrid" + i]).gameObject.transform;

            m_listRuneInsetLimitSign.Add(m_myTransform.Find(m_widgetToFullName["RuneUIInsetGrid" + i + "LimitSign"]).gameObject);
        }

        for (int i = 0; i < 16; ++i)
        {
            m_arrPackageGrid[i] = m_myTransform.Find(m_widgetToFullName["RuneUIPacakgeGridBG" + i]);
        }

        m_myTransform.Find(m_widgetToFullName["EquipCamera"]).GetComponentsInChildren<UIViewport>(true)[0].sourceCamera =
            GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_texInsetBodyBG = m_myTransform.Find(m_widgetToFullName["RuneUIInsetBG"]).GetComponentsInChildren<UITexture>(true)[0];
        
    }
   // void Awake()
    public override void CallWhenLoadResources()
    {
        ID = MFUIManager.MFUIID.RuneUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "RuneUI";
        m_myTransform = transform;

        m_instance = this;

        Initialize();
        FillFullNameData(m_myTransform);


        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);

        //Debug.LogError("RuneEnable");
        MogoUIManager.Instance.ShowBillboardList(false);

        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(false);

        MogoUIManager.Instance.GetMainUICamera().clearFlags = CameraClearFlags.SolidColor;

        EventDispatcher.TriggerEvent(Events.RuneEvent.GetRuneBag);
        EventDispatcher.TriggerEvent(Events.RuneEvent.GetBodyRunes);

        Mogo.Util.LoggerHelper.Debug("Loading Model");
        LoadPlayerModel();

        //Debug.LogError("RuneUIEnable");

        if (!SystemSwitch.DestroyResource)
        {
            return;
        }

        if (m_texInsetBodyBG.mainTexture == null)
        {
            AssetCacheMgr.GetResourceAutoRelease("lyfw_fuwenxiangqianbeijng.png", (obj) =>
            {

                m_texInsetBodyBG.mainTexture = (Texture)obj;
            });
        }

        //if (DragonUIViewManager.Instance.AtlasCanRelease != null)
        //{
        //    if (DragonUIViewManager.Instance.AtlasCanRelease.spriteMaterial.mainTexture == null)
        //    {
        //        AssetCacheMgr.GetUIResource("MogoDragonUI.png", (obj) =>
        //        {
        //           DragonUIViewManager.Instance.AtlasCanRelease.spriteMaterial.mainTexture = (Texture)obj;
        //           DragonUIViewManager.Instance.AtlasCanRelease.MarkAsDirty();
        //           m_myTransform.GetComponent<UIPanel>().enabled = false;
        //           m_myTransform.GetComponent<UIPanel>().enabled = true;
        //        });
        //    }
        //}
    }

    public override void CallWhenHide()
    {
        //if (SystemSwitch.DestroyAllUI)
        //{
            Release();
            m_instance = null;
            MFUIManager.GetSingleton().ReleaseRuneUI(m_myGameObject);
        //}
        MFUIUtils.ShowGameObject(false, m_myGameObject);

        //Debug.LogError("RuneUIDisable");
        MogoUIManager.Instance.ShowBillboardList(true);

        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(true);

        MogoUIManager.Instance.GetMainUICamera().clearFlags = CameraClearFlags.Depth;


        DisablePlayerModel();

        if (!SystemSwitch.DestroyResource)
        {
            return;
        }

        m_texInsetBodyBG.mainTexture = null;

        AssetCacheMgr.ReleaseResourceImmediate("lyfw_fuwenxiangqianbeijng.png");

        //DragonUIViewManager.Instance.AtlasCanRelease.spriteMaterial.mainTexture = null;

        //AssetCacheMgr.ReleaseResourceImmediate("MogoDragonUI.png");
    }

    void OnRuneUIComposeUp(int i)
    {
        if (RUNEUICOMPOSEUP != null)
            RUNEUICOMPOSEUP();
    }

    void OnGotoDragonUp(int i)
    {
        if (GOTODRAGONUP != null)
            GOTODRAGONUP();

        //MogoUIManager.Instance.ShowMogoDragonUI();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.DragonUI);
    }

    void OnRuneUICloseUp(int i)
    {
        if (RUNEUICLOSEUP != null)
            RUNEUICLOSEUP();


        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        MogoUIManager.Instance.CurrentUI = MogoUIManager.Instance.m_RuneUI;
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnRuneUIPackageGridUp(int id)
    {
        if (RUNEUIPACKAGEGRIDUP != null)
            RUNEUIPACKAGEGRIDUP(id);

        LoggerHelper.Debug(id);
        for (int i = 0; i < m_listRuneItem.Count; ++i)
        {
            if (m_listRuneItem[i].name == id.ToString())
            {
                //RuneUIViewManager.Instance.ShowRuneUICursor(false);
                MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
                RuneUIViewManager.Instance.ShowRuneUIToolTip(i, false,true);
                break;
            }
        }
    }

    void OnRuneUIPackageGridUpDouble(int id)
    {
        if (RUNEUIPACKAGEGRIDUPDOUBLE != null)
            RUNEUIPACKAGEGRIDUPDOUBLE(id);
    }

    void OnRuneUIInsetGridUp(int id)
    {
        if (RUNEUIINSETGRIDUP != null)
            RUNEUIINSETGRIDUP(id);


        for (int i = 0; i < m_listRuneInsetItem.Count; ++i)
        {
            if (m_listRuneInsetItem[i].name == id.ToString())
            {
                //RuneUIViewManager.Instance.ShowRuneUICursor(false);
                MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
                RuneUIViewManager.Instance.ShowRuneUIToolTip(i, false,false);
                break;
            }
        }
    }

    void OnRuneUIInsetGridUpDouble(int id)
    {
        if (RUNEUIINSETGRIDUPDOUBLE != null)
            RUNEUIINSETGRIDUPDOUBLE(id);
    }

    void OnRuneUIDragBegin(int id)
    {
        for (int i = 0; i < m_listRuneItem.Count; ++i)
        {
            if (m_listRuneItem[i].name == id.ToString())
            {
                //RuneUIViewManager.Instance.ShowRuneUICursor(true);
                string spName = m_listRuneItem[i].transform.Find("RuneUIInsetGridBG").GetComponentsInChildren<UISprite>(true)[0].spriteName;
                MogoGlobleUIManager.Instance.ShowUICursorSprite(spName);
                RuneUIViewManager.Instance.ShowRuneUIToolTip(i, false,true);
                break;
            }
        }
        LoggerHelper.Debug("DragBegin " + id);
    }

    void OnRuneUIInsetDragBegin(int id)
    {
        for (int i = 0; i < m_listRuneInsetItem.Count; ++i)
        {
            if (m_listRuneInsetItem[i].name == id.ToString())
            {
                //RuneUIViewManager.Instance.ShowRuneUICursor(true);
                string spName = m_listRuneInsetItem[i].transform.Find("RuneUIInsetGridBG").GetComponentsInChildren<UISprite>(true)[0].spriteName;
                MogoGlobleUIManager.Instance.ShowUICursorSprite(spName);
                RuneUIViewManager.Instance.ShowRuneUIToolTip(i, false, false);
                break;
            }
        }
        LoggerHelper.Debug("DragBegin " + id);
    }

    void OnRuneUIPackageGridDown(int id)
    {
        for (int i = 0; i < m_listRuneItem.Count; ++i)
        {
            if (m_listRuneItem[i].name == id.ToString())
            {
                RuneUIViewManager.Instance.ShowRuneUIToolTip(i, true,true);
                EventDispatcher.TriggerEvent(Events.RuneEvent.ShowTips, id, i, false, true);
                break;
            }
        }
    }

    void OnRuneUIPackageDragOutside()
    {
        //RuneUIViewManager.Instance.ShowRuneUICursor(false);
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
    }

    void OnRuneUIPackageGridDrag(int oldid, int newid)
    {
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
        //RuneUIViewManager.Instance.ShowRuneUICursor(false);
    }

    void OnRuneUIInsetGridDragToPackageGrid(int oldId, int newId)
    {
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
        //RuneUIViewManager.Instance.ShowRuneUICursor(false);
    }

    void OnRuneUIInsetGridDragOutside()
    {
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
    }

    void OnRuneUIInsetGridDrag(int oldId, int newId)
    {

        //RuneUIViewManager.Instance.ShowRuneUICursor(false);
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
    }

    void OnRuneUIPackageGridDragToInsetGrid(int oldId, int newId)
    {
       // RuneUIViewManager.Instance.ShowRuneUICursor(false);
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
    }

    void OnRuneUIInsetGridDown(int id)
    {
        LoggerHelper.Debug("InsetGridDown");
        for (int i = 0; i < m_listRuneInsetItem.Count; ++i)
        {
            if (m_listRuneInsetItem[i].name == id.ToString())
            {
                RuneUIViewManager.Instance.ShowRuneUIToolTip(i, true, false);
                EventDispatcher.TriggerEvent(Events.RuneEvent.ShowTips, id, i, false, false);
                break;
            }
        }
    }

    void Initialize()
    {

        RuneUILogicManager.Instance.Initialize();

        RuneUIDict.ButtonTypeToEventUp.Add("GotoDragon", OnGotoDragonUp);
        RuneUIDict.ButtonTypeToEventUp.Add("RuneUIClose", OnRuneUICloseUp);
        RuneUIDict.ButtonTypeToEventUp.Add("RuneUIPackageCompose", OnRuneUIComposeUp);

        for (int i = 0; i < 16; ++i)
        {
            RuneUIDict.ButtonTypeToEventUp.Add("RuneUIPacakgeGridBG" + i, OnRuneUIPackageGridUp);
            RuneUIDict.ButtonTypeToEventUp.Add("RuneUIPacakgeGridBG" + i + "Double", OnRuneUIPackageGridUpDouble);
        }

        for (int i = 0; i < 6; ++i)
        {
            RuneUIDict.ButtonTypeToEventUp.Add("RuneUIInsetGrid" + i, OnRuneUIInsetGridUp);
            RuneUIDict.ButtonTypeToEventUp.Add("RuneUIInsetGrid" + i + "Double", OnRuneUIInsetGridUpDouble);
        }

        for (int i = 6; i < 11; ++i)
        {
            RuneUIDict.ButtonTypeToEventUp.Add("RuneUIInsetGrid" + i, OnRuneUIInsetGridUp);
            RuneUIDict.ButtonTypeToEventUp.Add("RuneUIInsetGrid" + i + "Double", OnRuneUIInsetGridUpDouble);
        }
        EventDispatcher.AddEventListener<int>("RuneUIDragBegin", OnRuneUIDragBegin);
        EventDispatcher.AddEventListener<int>("RuneUIInsetDragBegin", OnRuneUIInsetDragBegin);
        EventDispatcher.AddEventListener<int>("RuneUIPackageGridDown", OnRuneUIPackageGridDown);
        EventDispatcher.AddEventListener("RuneUIPackageDragOutside", OnRuneUIPackageDragOutside);
        EventDispatcher.AddEventListener<int, int>("RuneUIPackageGridDrag", OnRuneUIPackageGridDrag);
        EventDispatcher.AddEventListener<int, int>("RuneUIPInsetGridDragToPackageGrid", OnRuneUIInsetGridDragToPackageGrid);
        EventDispatcher.AddEventListener("RuneUIInsetGridDragOutside", OnRuneUIInsetGridDragOutside);
        EventDispatcher.AddEventListener<int, int>("RuneUIInsetGridDrag", OnRuneUIInsetGridDrag);
        EventDispatcher.AddEventListener<int, int>("RuneUIPackageGridDragToInsetGrid", OnRuneUIPackageGridDragToInsetGrid);
        EventDispatcher.AddEventListener<int>("RuneUIInsetGridDown", OnRuneUIInsetGridDown);

    }

    public void Release()
    {
        RuneUILogicManager.Instance.Release();

        RuneUIDict.ButtonTypeToEventUp.Clear();
        EventDispatcher.RemoveEventListener<int>("RuneUIDragBegin", OnRuneUIDragBegin);
        EventDispatcher.RemoveEventListener<int>("RuneUIInsetDragBegin", OnRuneUIInsetDragBegin);
        EventDispatcher.RemoveEventListener<int>("RuneUIPackageGridDown", OnRuneUIPackageGridDown);
        EventDispatcher.RemoveEventListener("RuneUIPackageDragOutside", OnRuneUIPackageDragOutside);
        EventDispatcher.RemoveEventListener<int, int>("RuneUIPackageGridDrag", OnRuneUIPackageGridDrag);
        EventDispatcher.RemoveEventListener<int, int>("RuneUIPInsetGridDragToPackageGrid", OnRuneUIInsetGridDragToPackageGrid);
        EventDispatcher.RemoveEventListener("RuneUIInsetGridDragOutside", OnRuneUIInsetGridDragOutside);
        EventDispatcher.RemoveEventListener<int, int>("RuneUIInsetGridDrag", OnRuneUIInsetGridDrag);
        EventDispatcher.RemoveEventListener<int, int>("RuneUIPackageGridDragToInsetGrid", OnRuneUIPackageGridDragToInsetGrid);
        EventDispatcher.RemoveEventListener<int>("RuneUIInsetGridDown", OnRuneUIInsetGridDown);
    }

    public void SetRuneUILifePower(int power)
    {
        m_lblLifePower.text = power.ToString();
    }

    public void UnLockInsetGrid(int gridId)
    {
        LoggerHelper.Debug(gridId);
        m_arrInsetGridLock[gridId].SetActive(false);
    }

    public void RemoveRuneItem(int gridId)
    {
        for (int i = 0; i < m_listRuneItem.Count; ++i)
        {
            if (m_listRuneItem[i].name == gridId.ToString())
            {
                AssetCacheMgr.ReleaseInstance(m_listRuneItem[i]);
                m_listRuneItem.RemoveAt(i);
                break;
            }
        }
    }

    public void AddPackageGridItem(int level, string type,int gridId,string imgName = "", int color = 0)
    {
        RemoveRuneItem(gridId);

        LoggerHelper.Debug("Add " + gridId);
        AssetCacheMgr.GetUIInstance("RuneUIInsetGridInfotest.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.SetActive(true);
            obj.name = gridId.ToString();
            obj.transform.parent = m_myTransform.Find(m_widgetToFullName["RuneUIPacakgeGridBGList"]);
            obj.transform.localPosition = m_arrPackageGrid[gridId].localPosition;
            obj.transform.localScale = new Vector3(1, 1, 1);

            obj.transform.Find("RuneUIInsetGridLevel").GetComponentsInChildren<UILabel>(true)[0].text = "Lv" + level;
            obj.transform.Find("RuneUIInsetGridName").GetComponentsInChildren<UILabel>(true)[0].text =  type;
            UISprite s = obj.transform.Find("RuneUIInsetGridBG").GetComponentsInChildren<UISprite>(true)[0];
            s.spriteName = imgName;
            MogoUtils.SetImageColor(s, color);

            m_listRuneItem.Add(obj);
        });
    }

    public void AddInsetGridItem(int level, string type, int gridId,string imgName = "", int color = 0)
    {
        Mogo.Util.LoggerHelper.Debug(gridId);
        RemoveRuneInsetItem(gridId);

        LoggerHelper.Debug("AddInset " + gridId);
        AssetCacheMgr.GetUIInstance("RuneUIInsetGridInfotest.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.SetActive(true);
            obj.name = gridId.ToString();
            obj.transform.parent = m_myTransform.Find(m_widgetToFullName["RuneUIInset"]);
            obj.transform.localPosition = m_arrInsetGird[gridId].localPosition;
            obj.transform.localScale = new Vector3(1, 1, 1);

            obj.transform.Find("RuneUIInsetGridLevel").GetComponentsInChildren<UILabel>(true)[0].text = "Lv" + level;
            obj.transform.Find("RuneUIInsetGridName").GetComponentsInChildren<UILabel>(true)[0].text = type;
            UISprite s = obj.transform.Find("RuneUIInsetGridBG").GetComponentsInChildren<UISprite>(true)[0];
            s.spriteName = imgName;
            MogoUtils.SetImageColor(s, color);

            m_listRuneInsetItem.Add(obj);
        });
    }

    public void RemoveRuneInsetItem(int gridId)
    {
        for (int i = 0; i < m_listRuneInsetItem.Count; ++i)
        {
            if (m_listRuneInsetItem[i].name == gridId.ToString())
            {
                AssetCacheMgr.ReleaseInstance(m_listRuneInsetItem[i]);
                m_listRuneInsetItem.RemoveAt(i);
                break;
            }
        }
    }

    public void SetInsetGridInfo()
    {
        //����
    }

    public void ShowRuneUICursor(bool show)
    {
        m_goRuneUICursor.SetActive(show);
    }

    public void ShowRuneItemGridInfo(bool show)
    { }

    public void ShowRuneUIToolTip(int id, bool isShow, bool isPackage)
    {
        if (isPackage)
        {
            m_listRuneItem[id].transform.Find("RuneUIGridToolTip").gameObject.SetActive(isShow);
            m_listRuneItem[id].transform.Find("RuneUIGridToolTip").gameObject.transform.localPosition = 
                m_listRuneItem[id].transform.Find("RuneUIGridToolTipLeft").localPosition;
        }
        else
        {
            m_listRuneInsetItem[id].transform.Find("RuneUIGridToolTip").gameObject.SetActive(isShow);
            m_listRuneInsetItem[id].transform.Find("RuneUIGridToolTip").gameObject.transform.localPosition 
                = m_listRuneInsetItem[id].transform.Find("RuneUIGridToolTipRight").localPosition;
        }
    }

    public void SetDragonUIToolTipName(int id, string name)
    {
        m_listRuneItem[id].transform.Find(
            "RuneUIGridToolTip/RuneUIGridToolTipName/RuneUIGridToolTipNameText").GetComponentsInChildren<UILabel>(true)[0].text = name;
    }

    public void SetRuneUITooltipLevel(int id, string level)
    {
        m_listRuneItem[id].transform.Find(
            "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoLevelNum").GetComponentsInChildren<UILabel>(true)[0].text = level;
    }

    public void SetRuneUITooltipExp(int id, string exp)
    {
        m_listRuneItem[id].transform.Find(
            "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoExpNum").GetComponentsInChildren<UILabel>(true)[0].text = exp;
    }

    public void SetRuneUITooltipCurrentDesc(int id, string desc,bool isPackage)
    {
        if (isPackage)
        {
            m_listRuneItem[id].transform.Find(
                "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoCurrentDetail").GetComponentsInChildren<UILabel>(true)[0].text = desc;
        }
        else
        {
            m_listRuneInsetItem[id].transform.Find(
                "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoCurrentDetail").GetComponentsInChildren<UILabel>(true)[0].text = desc;
        }
    }

    public void SetRuneUITooltipNextDesc(int id, string desc,bool isPackage)
    {
        if (isPackage)
        {
            m_listRuneItem[id].transform.Find(
                "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoNextDetail").GetComponentsInChildren<UILabel>(true)[0].text = desc;
        }
        else
        {
            m_listRuneInsetItem[id].transform.Find(
                "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoNextDetail").GetComponentsInChildren<UILabel>(true)[0].text = desc;
        }

    }

    public void SetBodyUIToolTipName(int id, string name)
    {
        m_listRuneInsetItem[id].transform.Find(
            "RuneUIGridToolTip/RuneUIGridToolTipName/RuneUIGridToolTipNameText").GetComponentsInChildren<UILabel>(true)[0].text = name;
    }

    public void SetBodyUITooltipLevel(int id, string level)
    {
        m_listRuneInsetItem[id].transform.Find(
            "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoLevelNum").GetComponentsInChildren<UILabel>(true)[0].text = level;
    }

    public void SetBodyUITooltipExp(int id, string exp)
    {
        m_listRuneInsetItem[id].transform.Find(
            "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoExpNum").GetComponentsInChildren<UILabel>(true)[0].text = exp;
    }

    public void SetBodyUITooltipCurrentDesc(int id, string desc)
    {
        m_listRuneInsetItem[id].transform.Find(
            "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoCurrentDetail").GetComponentsInChildren<UILabel>(true)[0].text = desc;
    }

    public void SetBodyUITooltipNextDesc(int id, string desc)
    {
        m_listRuneInsetItem[id].transform.Find(
            "RuneUIGridToolTip/RuneUIGridToolTipInfo/RuneUIGridToolTipInfoNextDetail").GetComponentsInChildren<UILabel>(true)[0].text = desc;

    }

    public void LoadPlayerModel()
    {
      
        GameObject obj = MogoWorld.thePlayer.GameObject;

        Transform cam = m_myTransform.Find(m_widgetToFullName["EquipCamera"]);

        cam.position = obj.transform.Find("slot_camera").position + obj.transform.forward * 3.2f;
        cam.forward = -obj.transform.forward;

        Mogo.Util.LoggerHelper.Debug(cam.position + " " + cam.localPosition);

        SetObjectLayer(10, obj);
    }

    public void DisablePlayerModel()
    {
        SetObjectLayer(8, MogoWorld.thePlayer.GameObject);
    }

    public void SetObjectLayer(int layer, GameObject obj)
    {
        if (obj == null || obj.transform == null)
            return;

        if (obj.transform.GetChildCount() == 0)
        {
            obj.layer = layer;
            return;
        }
        else
        {
            for (int i = 0; i < obj.transform.GetChildCount(); ++i)
            {
                SetObjectLayer(layer, obj.transform.GetChild(i).gameObject);
            }
        }
    }

    public void ShowRuneInsetItemLimitSign(int id, bool isShow)
    {
        m_listRuneInsetLimitSign[id].SetActive(isShow);
        m_listRuneInsetLimitSign[id].GetComponentsInChildren<RuneUIInsetLimitSign>(true)[0].IsBeginCountDown = true;
    }

    public void SetRuneInsetItemLimitText(int id, string text)
    {
        m_listRuneInsetLimitSign[id].GetComponentsInChildren<UILabel>(true)[0].text = text;
    }

    public void SetRuneInsetGridLevel(int id, string level)
    {
        m_myTransform.Find(m_widgetToFullName["RuneUIInsetGridLevel" + id + "Text"]).GetComponentsInChildren<UILabel>(true)[0].text = level;
    }

    public void ShowAsPlayerModel()
    {
        GetTransform("EquipCamera").gameObject.SetActive(true);
        GetTransform("RuneUIExtraInsetGrid").gameObject.SetActive(false);
    }

    public void ShowAsExtraGrid()
    {
        GetTransform("EquipCamera").gameObject.SetActive(false);
        GetTransform("RuneUIExtraInsetGrid").gameObject.SetActive(true);
    }

    public override void CallWhenEnable()
    {
       

    }

    public override void CallWhenDisable()
    {
       
    }


    void Start()
    {

        LoadPlayerModel();
    }
}
