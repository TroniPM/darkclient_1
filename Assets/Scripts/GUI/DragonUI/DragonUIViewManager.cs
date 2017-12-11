using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;

public class DragonUIViewManager : MFUIUnit
{
    public UIAtlas AtlasCanRelease;

    public Action DIAMONDWISHUP;
    public Action GOLDWISHUP;
    public Action GOTORUNEUP;
    public Action ONEKEYCOMPOSEUP;
    public Action ONEKEYPICKUPUP;
    public Action DRAGONUICLOSEUP;

    public Action<int> DRAGONUIPACKAGEGRIDUP;
    public Action<int> DRAGONUIPACKAGEGRIDUPDOUBLE;

    UILabel m_lblPackageInfoGold;
    UILabel m_lblPackageInfoDiamond;

    UILabel m_lblDiamondWishCost;
    UILabel m_lblGoldWishCost;

    GameObject m_goDragonAnim;
    TweenAlpha m_taDargonAnim;
    TweenScale m_tsDragonAnim;

    GameObject m_goDiamondWishAnim;
    TweenAlpha m_taDiamondWishAnim;
    TweenPosition m_tpDiamondWishAnim;

    GameObject m_goGoldWishAnim;
    TweenAlpha m_taGoldWishAnim;
    TweenPosition m_tpGoldWishAnim;

    GameObject m_goDragonItemBornPoint;
    GameObject m_goDragonItemDeadPoint;

    GameObject m_goDragonUICursor;

    GameObject m_goDragonUIPackage;
    UISprite uiCursor;

    List<GameObject> m_listDragonItem = new List<GameObject>();
    UILabel[] m_listFloatGold = new UILabel[16];
    Transform[] m_arrDragonPackageItem = new Transform[16];
    List<GameObject> m_listDragonGridTooltip = new List<GameObject>();

    int m_iPickUpGridID = -1;

    private static DragonUIViewManager m_instance;

    public static DragonUIViewManager Instance
    {
        get
        {
            return DragonUIViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.DragonUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);

        MFUIResourceManager.GetSingleton().PreLoadResource("DragonUIGridInfotest.prefab", ID);
        MFUIResourceManager.GetSingleton().PreLoadResource("fx_ui_longyujiemian_qiu.prefab", ID);
        MFUIResourceManager.GetSingleton().PreLoadResource("fx_ui_longyujiemian_jb.prefab", ID);

        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(MFUIManager.MFUIID.DragonUI);

        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        m_myGameObject.name = "DragonUI";
    }

    public override void CallWhenCreate()
    {
        SetUIDirty();
        m_lblPackageInfoDiamond = GetLabel("DragonUIPackageInfoDiamondNum");
        m_lblPackageInfoGold = GetLabel("DragonUIPackageInfoGoldNum");

        m_goDragonAnim = GetSprite("DragonUIDragonAnim").gameObject;
        m_taDargonAnim = m_goDragonAnim.GetComponentsInChildren<TweenAlpha>(true)[0];
        m_tsDragonAnim = m_goDragonAnim.GetComponentsInChildren<TweenScale>(true)[0];

        m_goDiamondWishAnim = GetLabel("DiamondWishAnim").gameObject;
        m_taDiamondWishAnim = m_goDiamondWishAnim.GetComponentsInChildren<TweenAlpha>(true)[0];
        m_tpDiamondWishAnim = m_goDiamondWishAnim.GetComponentsInChildren<TweenPosition>(true)[0];

        m_goGoldWishAnim = GetLabel("GoldWishAnim").gameObject;
        m_taGoldWishAnim = m_goGoldWishAnim.GetComponentsInChildren<TweenAlpha>(true)[0];
        m_tpGoldWishAnim = m_goGoldWishAnim.GetComponentsInChildren<TweenPosition>(true)[0];

        m_goDragonItemBornPoint = GetTransform("DragonItemBornPoint").gameObject;
        m_goDragonItemDeadPoint = GetTransform("DragonItemDeadPoint").gameObject;

        m_goDragonUICursor = GetTransform("DragonUICursorUVAnim").gameObject;

        m_lblDiamondWishCost = GetLabel("DiamondWishButtonText");
        m_lblGoldWishCost = GetLabel("GoldWishButtonText");
        m_goDragonUIPackage = GetTransform("DragonUIPackage").gameObject;

        for (int i = 0; i < 16; ++i)
        {
            m_arrDragonPackageItem[i] = GetSprite(string.Concat("DragonUIPacakgeGridBG", i)).transform;
            m_listFloatGold[i] = GetLabel(string.Concat("DragonUIFloatGold", i));
        }

        AtlasCanRelease = m_arrDragonPackageItem[0].GetComponentInChildren<UISprite>().atlas;

        uiCursor = GameObject.Find("MogoGlobleUIPanel").transform.Find("UICursor/UICursorSprite").GetComponentsInChildren<UISprite>(true)[0];

        uiCursor.atlas = AtlasCanRelease;



        DragonUILogicManager.Instance.Initialize();

        DragonUIDict.ButtonTypeToEventUp.Add("DiamondWishButton", OnDiamondWishUp);
        DragonUIDict.ButtonTypeToEventUp.Add("GoldWishButton", OnGoldWishUp);
        DragonUIDict.ButtonTypeToEventUp.Add("GotoRuneIcon", OnGotoRuneIconUp);
        DragonUIDict.ButtonTypeToEventUp.Add("OneKeyComposeIcon", OnOneKeyComposeIconUp);
        DragonUIDict.ButtonTypeToEventUp.Add("OneKeyPickUpIcon", OnOneKeyPickUpIconUp);
        DragonUIDict.ButtonTypeToEventUp.Add("DragonUIClose", OnDragonUICloseUp);

        for (int i = 0; i < 16; ++i)
        {
            DragonUIDict.ButtonTypeToEventUp.Add("DragonUIPacakgeGridBG" + i, OnDragonUIPackageGridBGUp);
            DragonUIDict.ButtonTypeToEventUp.Add("DragonUIPacakgeGridBG" + i + "Double", OnDragonUIPackageGridBGUpDouble);
        }


        EventDispatcher.AddEventListener<int>("DragonUIDragBegin", OnDragonUIDragBegin);
        EventDispatcher.AddEventListener<int, int>("DragonUIPackageGridDrag", OnDragonUIPackageGridDrag);
        EventDispatcher.AddEventListener("DragonUIPackageGridDragOutside", OnDragonUIPackageGridDragOutside);
        EventDispatcher.AddEventListener<int>("DragonUIPackageGridDown", OnDragonUIPackageGridDown);
    }

    int index = 0;

    void OnDiamondWishUp(int i)
    {
        if (DIAMONDWISHUP != null)
            DIAMONDWISHUP();

        ShowDragonAnim();
        //ShowDiamondWishAnim();

        ++index;
    }

    void OnGoldWishUp(int i)
    {
        if (GOLDWISHUP != null)
            GOLDWISHUP();

        ShowDragonAnim();
        //ShowGoldWishAnim();
    }

    void OnGotoRuneIconUp(int i)
    {
        if (GOTORUNEUP != null)
            GOTORUNEUP();

        //MogoUIManager.Instance.ShowMogoRuneUI();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RuneUI);
    }

    void OnOneKeyComposeIconUp(int i)
    {
        if (ONEKEYCOMPOSEUP != null)
            ONEKEYCOMPOSEUP();
    }

    void OnOneKeyPickUpIconUp(int i)
    {
        if (ONEKEYPICKUPUP != null)
            ONEKEYPICKUPUP();
    }

    void OnDragonUICloseUp(int i)
    {
        if (DRAGONUICLOSEUP != null)
            DRAGONUICLOSEUP();

        
        //MogoUIManager.Instance.ReleaseUIFucking();

        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        MogoUIManager.Instance.CurrentUI = MogoUIManager.Instance.m_DragonUI;

        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnDragonUIPackageGridBGUp(int id)
    {
        if (DRAGONUIPACKAGEGRIDUP != null)
            DRAGONUIPACKAGEGRIDUP(id);


        for (int i = 0; i < m_listDragonItem.Count; ++i)
        {
            if (m_listDragonItem[i].name == id.ToString())
            {
                MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
                //DragonUIViewManager.Instance.ShowDragonUICursor(false);
                // DragonUIViewManager.Instance.ShowDragonUIToolTip(i, false);
                break;
            }
        }
    }

    void OnDragonUIPackageGridBGUpDouble(int id)
    {
        if (DRAGONUIPACKAGEGRIDUPDOUBLE != null)
            DRAGONUIPACKAGEGRIDUPDOUBLE(id);
    }


    void OnDragonUIDragBegin(int id)
    {
        for (int i = 0; i < m_listDragonItem.Count; ++i)
        {
            if (m_listDragonItem[i].name == id.ToString())
            {
                string spName = m_listDragonItem[i].transform.Find("DragonUIGridBG").GetComponentsInChildren<UISprite>(true)[0].spriteName;
                MogoGlobleUIManager.Instance.ShowUICursorSprite(spName);
                //DragonUIViewManager.Instance.ShowDragonUICursor(true);
                DragonUIViewManager.Instance.ShowDragonUIToolTip(i, false);

                break;
            }
        }
        LoggerHelper.Debug("DragBegin " + id);
    }

    void OnDragonUIPackageGridDrag(int newId, int OldId)
    {
        //DragonUIViewManager.Instance.ShowDragonUICursor(false);
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
    }

    void OnDragonUIPackageGridDragOutside()
    {
        //DragonUIViewManager.Instance.ShowDragonUICursor(false);
        MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
    }

    void OnDragonUIPackageGridDown(int id)
    {
        for (int i = 0; i < m_listDragonItem.Count; ++i)
        {
            if (m_listDragonItem[i].name == id.ToString())
            {
                // DragonUIViewManager.Instance.ShowDragonUIToolTip(i, true);
                EventDispatcher.TriggerEvent(Events.RuneEvent.ShowTips, id, i, true, false);
                break;
            }
        }
    }
    public void Release()
    {
        DragonUILogicManager.Instance.Release();

        DragonUIDict.ButtonTypeToEventUp.Clear();


        EventDispatcher.RemoveEventListener<int>("DragonUIDragBegin", OnDragonUIDragBegin);
        EventDispatcher.RemoveEventListener<int, int>("DragonUIPackageGridDrag", OnDragonUIPackageGridDrag);
        EventDispatcher.RemoveEventListener("DragonUIPackageGridDragOutside", OnDragonUIPackageGridDragOutside);
        EventDispatcher.RemoveEventListener<int>("DragonUIPackageGridDown", OnDragonUIPackageGridDown);

        foreach (var item in m_listDragonItem)
        {
            AssetCacheMgr.ReleaseInstance(item);
        }

    }

    public void SetPackageDiamondNum(int num)
    {
        m_lblPackageInfoDiamond.text = num.ToString();
    }

    public void SetPackageGoldNum(int num)
    {
        m_lblPackageInfoGold.text = num.ToString();
    }

    public void ShowFloatGold(int gridId)
    {
        m_listFloatGold[gridId].gameObject.SetActive(true);

        m_listFloatGold[gridId].gameObject.GetComponentsInChildren<TweenAlpha>(true)[0].Reset();
        m_listFloatGold[gridId].gameObject.GetComponentsInChildren<TweenPosition>(true)[0].Reset();

        m_listFloatGold[gridId].gameObject.GetComponentsInChildren<TweenAlpha>(true)[0].Play(true);
        m_listFloatGold[gridId].gameObject.GetComponentsInChildren<TweenPosition>(true)[0].Play(true);
    }

    void ShowDragonAnim()
    {
        m_goDragonAnim.SetActive(true);

        m_taDargonAnim.Reset();
        m_tsDragonAnim.Reset();

        m_taDargonAnim.Play(true);
        m_tsDragonAnim.Play(true);
    }

    public void ShowDiamondWishAnim()
    {
        m_goDiamondWishAnim.SetActive(true);

        m_taDiamondWishAnim.Reset();
        m_tpDiamondWishAnim.Reset();

        m_taDiamondWishAnim.Play(true);
        m_tpDiamondWishAnim.Play(true);
    }

    public void ShowGoldWishAnim()
    {
        m_goGoldWishAnim.SetActive(true);

        m_taGoldWishAnim.Reset();
        m_tpGoldWishAnim.Reset();

        m_taGoldWishAnim.Play(true);
        m_tpGoldWishAnim.Play(true);
    }

    public void ShowFloatGoldAnim(int gridId)
    {
        for (int i = 0; i < m_listDragonItem.Count; ++i)
        {
            if (m_listDragonItem[i].name == gridId.ToString())
            {
                GameObject goAnim = m_listDragonItem[i].transform.Find("DragonUIGridGoldAnim").gameObject;
                goAnim.SetActive(true);

                goAnim.GetComponentsInChildren<TweenAlpha>(true)[0].Reset();
                goAnim.GetComponentsInChildren<TweenPosition>(true)[0].Reset();

                goAnim.GetComponentsInChildren<TweenAlpha>(true)[0].Play(true);
                goAnim.GetComponentsInChildren<TweenPosition>(true)[0].Play(true);

                break;
            }
        }
    }

    public void WishAddDragonItem(int level, string type, int gridId, string imgName = "", int color = 0)
    {
        LoggerHelper.Debug("Add " + gridId);
        AssetCacheMgr.GetUIInstance("DragonUIGridInfotest.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.SetActive(true);
            obj.name = gridId.ToString();
            obj.transform.parent = GetTransform("DragonUIPackageList");// m_myTransform.FindChild(m_widgetToFullName["DragonUIPackageList"]);
            obj.transform.localPosition = new Vector3(862, -22, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            TweenPosition tp = obj.GetComponentsInChildren<TweenPosition>(true)[0];

            tp.from = m_goDragonItemBornPoint.transform.localPosition;
            tp.to = m_arrDragonPackageItem[gridId].parent.localPosition;
            tp.duration = 0.5f;

            obj.transform.Find("DragonUIGridLevel").GetComponentsInChildren<UILabel>(true)[0].text = "Lv" + level;
            obj.transform.Find("DragonUIGridName").GetComponentsInChildren<UILabel>(true)[0].text = type;
            UISprite s = obj.transform.Find("DragonUIGridBG").GetComponentsInChildren<UISprite>(true)[0];
            s.spriteName = imgName;
            MogoUtils.SetImageColor(s, color);

            m_listDragonItem.Add(obj);
        });
    }


    public void RemoveDragonItem(int gridId)
    {
        for (int i = 0; i < m_listDragonItem.Count; ++i)
        {
            if (m_listDragonItem[i].name == gridId.ToString())
            {
                AssetCacheMgr.ReleaseInstance(m_listDragonItem[i]);
                m_listDragonItem.RemoveAt(i);
                break;
            }
        }
    }

    public void AddDragonItem(int level, string type, int gridId, string imgName = "", int color = 0)
    {
        RemoveDragonItem(gridId);

        LoggerHelper.Debug("Add " + gridId);
        AssetCacheMgr.GetUIInstance("DragonUIGridInfotest.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.SetActive(true);
            obj.name = gridId.ToString();
            obj.transform.parent = GetTransform("DragonUIPackageList");// m_myTransform.FindChild(m_widgetToFullName["DragonUIPackageList"]);
            obj.transform.localPosition = m_arrDragonPackageItem[gridId].parent.localPosition;
            obj.transform.localScale = new Vector3(1, 1, 1);

            TweenPosition tp = obj.GetComponentsInChildren<TweenPosition>(true)[0];

            tp.enabled = false;

            obj.transform.Find("DragonUIGridLevel").GetComponentsInChildren<UILabel>(true)[0].text = "Lv" + level;

            obj.transform.Find("DragonUIGridName").GetComponentsInChildren<UILabel>(true)[0].text = type;

            UISprite s = obj.transform.Find("DragonUIGridBG").GetComponentsInChildren<UISprite>(true)[0];
            s.spriteName = imgName;
            MogoUtils.SetImageColor(s, color);

            m_listDragonItem.Add(obj);
        });
    }



    public void PickUpDragonItem(int gridId)
    {

        for (int i = 0; i < m_listDragonItem.Count; ++i)
        {
            if (m_listDragonItem[i].name == gridId.ToString())
            {
                m_iPickUpGridID = i;

                TweenPosition tp = m_listDragonItem[i].GetComponentsInChildren<TweenPosition>(true)[1];
                tp.from = m_listDragonItem[i].transform.localPosition;
                tp.to = m_goDragonItemDeadPoint.transform.localPosition;

                tp.enabled = true;
                tp.eventReceiver = gameObject;
                tp.callWhenFinished = "OnTPEnd";

                TweenColor tc = m_listDragonItem[i].transform.Find("DragonUIGridAnimSlot/MogoUVAnimationWithShader").GetComponentsInChildren<TweenColor>(true)[0];
                tc.enabled = true;


                TweenAlpha ta = m_listDragonItem[i].transform.Find("DragonUIGridName").GetComponentsInChildren<TweenAlpha>(true)[0];
                ta.enabled = true;

                ta = m_listDragonItem[i].transform.Find("DragonUIGridLevel").GetComponentsInChildren<TweenAlpha>(true)[0];
                ta.enabled = true;
            }
        }
    }

    void OnTPEnd(UITweener t)
    {
        LoggerHelper.Debug("Destroy");


        for (int i = 0; i < m_listDragonItem.Count; ++i)
        {
            if (m_listDragonItem[i].name == t.name)
            {
                AssetCacheMgr.ReleaseInstance(m_listDragonItem[i]);
                m_listDragonItem.RemoveAt(i);
            }
        }

    }

    void OnAllTPEnd()
    {
        LoggerHelper.Debug("Destroy");
        for (int i = 0; i < m_listDragonItem.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listDragonItem[i]);
        }
        m_listDragonItem.Clear();
    }

    public void PickUpAllDragonItem()
    {
        for (int i = 0; i < m_listDragonItem.Count; ++i)
        {
            TweenPosition tp = m_listDragonItem[i].GetComponentsInChildren<TweenPosition>(true)[1];
            tp.from = m_listDragonItem[i].transform.localPosition;
            tp.to = m_goDragonItemDeadPoint.transform.localPosition;

            tp.enabled = true;
            tp.eventReceiver = gameObject;
            tp.callWhenFinished = "OnAllTPEnd";

            TweenColor tc = m_listDragonItem[i].transform.Find("DragonUIGridAnimSlot/MogoUVAnimationWithShader").GetComponentsInChildren<TweenColor>(true)[0];
            tc.enabled = true;


            TweenAlpha ta = m_listDragonItem[i].transform.Find("DragonUIGridName").GetComponentsInChildren<TweenAlpha>(true)[0];
            ta.enabled = true;

            ta = m_listDragonItem[i].transform.Find("DragonUIGridLevel").GetComponentsInChildren<TweenAlpha>(true)[0];
            ta.enabled = true;
        }

        index = 0;
    }

    public void ShowDragonUICursor(bool show)
    {
        //m_goDragonUICursor.SetActive(show);
        MogoGlobleUIManager.Instance.ShowUICursorUIAnim(show);
        //MogoGlobleUIManager.Instance.ShowUICursorSprite(null);
    }

    public void ShowDragonUIToolTip(int id, bool isShow)
    {
        m_listDragonItem[id].transform.Find("DragonUIGridToolTip").gameObject.SetActive(isShow);
    }

    public void SetDragonUIToolTipName(int id, string name)
    {
        m_listDragonItem[id].transform.Find(
            "DragonUIGridToolTip/DragonUIGridToolTipName/DragonUIGridToolTipNameText").GetComponentsInChildren<UILabel>(true)[0].text = name;
    }

    public void SetDragonUITooltipLevel(int id, string level)
    {
        m_listDragonItem[id].transform.Find(
            "DragonUIGridToolTip/DragonUIGridToolTipInfo/DragonUIGridToolTipInfoLevelNum").GetComponentsInChildren<UILabel>(true)[0].text = level;
    }

    public void SetDragonUITooltipExp(int id, string exp)
    {
        m_listDragonItem[id].transform.Find(
            "DragonUIGridToolTip/DragonUIGridToolTipInfo/DragonUIGridToolTipInfoExpNum").GetComponentsInChildren<UILabel>(true)[0].text = exp;
    }

    public void SetDragonUITooltipCurrentDesc(int id, string desc)
    {
        m_listDragonItem[id].transform.Find(
            "DragonUIGridToolTip/DragonUIGridToolTipInfo/DragonUIGridToolTipInfoCurrentDetail").GetComponentsInChildren<UILabel>(true)[0].text = desc;
    }

    public void SetDragonUITooltipNextDesc(int id, string desc)
    {
        m_listDragonItem[id].transform.Find(
            "DragonUIGridToolTip/DragonUIGridToolTipInfo/DragonUIGridToolTipInfoNextDetail").GetComponentsInChildren<UILabel>(true)[0].text = desc;

    }

    public void SetDiamondWishAnimText(string text)
    {
        m_goDiamondWishAnim.GetComponentsInChildren<UILabel>(true)[0].text = text;

    }

    public void SetGlodWishAnimText(string text)
    {
        m_goGoldWishAnim.GetComponentsInChildren<UILabel>(true)[0].text = text;
    }

    public void SetFloatGoldText(string text, int gridId)
    {
        m_listFloatGold[gridId].text = text;
    }

    public void SetDiamondWishCost(string cost)
    {
        m_lblDiamondWishCost.text = cost;
    }

    public void SetGlodWishCost(string cost)
    {
        m_lblGoldWishCost.text = cost;
    }

    public void SetCursorInfo(int level, string type)
    {
        //����
    }

    public void ShowDragonItemGridInfo(bool show)
    {

    }

    Texture tex;



    public override void CallWhenShow()
    {

        EventDispatcher.TriggerEvent(Events.RuneEvent.GetRuneBag);
        MFUIUtils.ShowGameObject(true, m_myGameObject);

        //Debug.LogError("DragonEnable");
        MogoUIManager.Instance.ShowBillboardList(false);

        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(false);

        MogoUIManager.Instance.GetMainUICamera().clearFlags = CameraClearFlags.SolidColor;

        //Mogo.Util.LoggerHelper.Debug("DamnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnEnable");
        if (isFirst)
        {
            //���ӳٵĻ���transform.positionȡֵ����ȷ
            TimerHeap.AddTimer(100, 0, () =>
            {
                MogoFXManager.Instance.AttachUIFX(5, MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, null);
                MogoFXManager.Instance.AttachUIFX(6, MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, null);
                isFirst = false;
            });
        }
        else
        {
            MogoFXManager.Instance.ShowUIFX(5, true);
            MogoFXManager.Instance.ShowUIFX(6, true);
        }

        uiCursor.atlas = AtlasCanRelease;

        if (!SystemSwitch.DestroyResource)
        {
            return;
        }

        if (m_myTransform.Find("DragonIcon").GetComponentsInChildren<UITexture>(true)[0].mainTexture == null)
        {

            AssetCacheMgr.GetResourceAutoRelease("lyfw_ditu.png", (obj) =>
            {

                m_myTransform.Find("DragonIcon").GetComponentsInChildren<UITexture>(true)[0].mainTexture = (Texture)obj;
            });
        }

        //if (AtlasCanRelease != null)
        //{
        //    if (AtlasCanRelease.spriteMaterial.mainTexture == null)
        //    {
        //        AssetCacheMgr.GetUIResource("MogoDragonUI.png", (obj) =>
        //        {

        //            AtlasCanRelease.spriteMaterial.mainTexture = (Texture)obj;
        //            AtlasCanRelease.MarkAsDirty();

        //            m_myTransform.GetComponent<UIPanel>().enabled = false;
        //            m_myTransform.GetComponent<UIPanel>().enabled = true;
        //        });
        //    }
        //}
    }


    public override void CallWhenHide()
    {
        //if (SystemSwitch.DestroyAllUI)
        //{
            m_myTransform.Find("DragonIcon").GetComponentsInChildren<UITexture>(true)[0].mainTexture = null;
            //AtlasCanRelease.spriteMaterial.mainTexture = null;
            AssetCacheMgr.ReleaseResourceImmediate("lyfw_ditu.png");
            Release();
            m_instance = null;
            MFUIManager.GetSingleton().ReleaseDragonUI(m_myGameObject);
        //}

        MFUIUtils.ShowGameObject(false, m_myGameObject);

        //Debug.LogError("DragonDisable");
        MogoUIManager.Instance.ShowBillboardList(true);

        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(true);

        MogoUIManager.Instance.GetMainUICamera().clearFlags = CameraClearFlags.Depth;


        //Mogo.Util.LoggerHelper.Debug("DamnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnDisalbe");
        MogoFXManager.Instance.DetachUIFX(5);
        MogoFXManager.Instance.DetachUIFX(6);

        //MogoFXManager.Instance.ShowUIFX(5, false);
        //MogoFXManager.Instance.ShowUIFX(6, false);
        //uiCursor.atlas = null;
        


        if (!SystemSwitch.DestroyResource)
        {
            return;
        }
        //tex = m_myTransform.FindChild("DragonIcon").GetComponentsInChildren<UITexture>(true)[0].mainTexture;
        //AssetCacheMgr.ReleaseResource(tex);
        // m_myTransform.FindChild("DragonIcon").GetComponentsInChildren<UITexture>(true)[0].mainTexture = null;
        //AssetCacheMgr.ReleaseResource(m_myTransform.FindChild("DragonIcon").GetComponentsInChildren<UITexture>(true)[0].mainTexture);

        m_myTransform.Find("DragonIcon").GetComponentsInChildren<UITexture>(true)[0].mainTexture = null;
        //AtlasCanRelease.spriteMaterial.mainTexture = null;
        AssetCacheMgr.ReleaseResourceImmediate("lyfw_ditu.png");
        //AssetCacheMgr.ReleaseResourceImmediate("MogoDragonUI.png");
    }

    bool isFirst = true;
  //  void OnEnable()
    public override void CallWhenEnable()
    {

    }

   // void OnDisable()
    public override void CallWhenDisable()

    {
        
    }

}