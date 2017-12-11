/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ComposeUIViewManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class ComposeUIViewManager : MogoUIBehaviour
{
    private static ComposeUIViewManager m_instance;
    public static ComposeUIViewManager Instance { get { return ComposeUIViewManager.m_instance; } }

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();

    public Action COMPOSEBUYUP;
    public Action COMPOSECOMPOSEUP;
    public Action COMPOSECOMPOSENOWUP; 

    private List<GameObject> m_listIconGrid = new List<GameObject>();
    private List<GameObject> m_listIconGridChild = new List<GameObject>();

    private bool[] m_bGridStatus = new bool[9];

    private Transform m_transPanelTable;

    private const float ICONGRIDSPACE = 95f;
    private const float ICONGRIDCHILDSPACE = 77f;
    private const float ICONGRIDCAMERASPACE = 0.06f;
    private const float ICONGRIDCHILDCAMERASPACE = 0.06f;

    private UITexture m_texUIBG;

    private Camera m_dragCamera;
    private MyDragableCamera m_dragableCamera;

    private GameObject m_goGOComposeDialogBodyPearlUp;
    private GameObject m_goGOComposeDialogBodyPearlLeft;
    private GameObject m_goGOComposeDialogBodyPearlRight;
    private GameObject m_goGOComposeDialogBodyPearlFinal;
    private UISlicedSprite m_ssComposePearlUp;
    private UISlicedSprite m_ssComposePearlLeft;
    private UISlicedSprite m_ssComposePearlRight;
    private UISlicedSprite m_ssComposePearlFinal;

    private int m_iOldParentId = -1;

    private GameObject m_goComposeNowBtn;
  
    private void OnComposeBuyUp()
    {
        if (COMPOSEBUYUP != null)
        {
            COMPOSEBUYUP();
        }
    }

    private void OnComposeComposeUp()
    {
        if (COMPOSECOMPOSEUP != null)
        {
            COMPOSECOMPOSEUP();
        }
    }

    private void OnComposeComposeNowUp()
    {
        if (COMPOSECOMPOSENOWUP != null)
        {
            COMPOSECOMPOSENOWUP();
        }
    }

    private void OnComposeIconChildGridUp(int parentId, int childId)
    {
        if (m_iOldParentId != -1 && parentId != m_iOldParentId)
        {
            m_myTransform.Find(m_widgetToFullName["ComposeDialogIconListPanelTable"]).Find(m_iOldParentId + "/EquipmentUIComposeIconListGridChildList").
                GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetAllButtonUp();
        }

        m_iOldParentId = parentId;
    }

    void OnTSEnd()
    {
        int iconNum = ComposeUIViewManager.Instance.GetIconGridNum();
        int iconChild = ComposeUIViewManager.Instance.GetIconGridChildNum();
        float f = m_dragableCamera.transformList[0].localPosition.y - (ICONGRIDSPACE * 0.0008f * (iconNum - 1) + (iconChild - 6) * ICONGRIDCHILDSPACE * 0.0008f);

        m_dragableCamera.MINY = f;
        m_dragableCamera.SetArrow();
        //float cameraPageHeight = Math.Abs(m_dragableCamera.FPageHeight * 1280);
        //float totalLength = iconNum * ICONGRIDSPACE + iconChild * ICONGRIDCHILDSPACE;
    }

    void OnComposeIconGridUp(int id)
    {
        //float fOffect = m_listIconGrid[id].transform.FindChild("EquipmentUIComposeIconListGridChildList").GetChildCount() * ICONGRIDCHILDSPACE * 0.0008f;
        //float fScale = 0.001f;
        //TweenScale ts = m_listIconGrid[id].transform.FindChild("EquipmentUIComposeIconListGridChildList").GetComponentsInChildren<TweenScale>(true)[0];

        //if (m_listIconGrid[id].transform.FindChild("EquipmentUIComposeIconListGridChildList").localScale.y < 0.1f)
        //{
        //    fOffect = -m_listIconGrid[id].transform.FindChild("EquipmentUIComposeIconListGridChildList").GetChildCount() * ICONGRIDCHILDSPACE * 0.0008f;
        //    fScale = 1f;
        //}
        //else
        //{
        //    if (id+1 < m_listIconGrid.Count)
        //    {
        //        fOffect = (m_listIconGrid[id].transform.localPosition - m_listIconGrid[id + 1].transform.localPosition).y - ICONGRIDSPACE * 0.0008f;
        //    }
        //    fScale = 0.001f;
        //}

        //for (int i = id+1; i < m_listIconGrid.Count; ++i)
        //{
        //    TweenPosition tp = m_listIconGrid[i].GetComponentsInChildren<TweenPosition>(true)[0];
        //    tp.from = m_listIconGrid[i].transform.localPosition;
        //    tp.to = tp.from + new Vector3(0, fOffect, 0);
        //    tp.Reset();
        //    tp.Play(true);
        //}

        //ts.from = m_listIconGrid[id].transform.FindChild("EquipmentUIComposeIconListGridChildList").localScale;
        //ts.to = new Vector3(1, fScale, 1);
        //ts.Reset();
        //ts.callWhenFinished = "OnTSEnd";
        //ts.eventReceiver = gameObject;
        //ts.Play(true);
        m_bGridStatus[id] = !m_bGridStatus[id];

        ScaleIconGridList(id);
    }

    void ScaleIconGridList(int id, bool autoScale = true)
    {
        float fOffect = m_listIconGrid[id].transform.Find("EquipmentUIComposeIconListGridChildList").GetChildCount() * ICONGRIDCHILDSPACE * 0.0008f;
        float fScale = 0.001f;
        TweenScale ts = m_listIconGrid[id].transform.Find("EquipmentUIComposeIconListGridChildList").GetComponentsInChildren<TweenScale>(true)[0];

        if (m_listIconGrid[id].transform.Find("EquipmentUIComposeIconListGridChildList").localScale.y < 0.1f)
        {
            fOffect = -m_listIconGrid[id].transform.Find("EquipmentUIComposeIconListGridChildList").GetChildCount() * ICONGRIDCHILDSPACE * 0.0008f;
            fScale = 1f;
        }
        else
        {
            if (autoScale == false)
            {
                return;

            }
            if (id + 1 < m_listIconGrid.Count)
            {
                fOffect = (m_listIconGrid[id].transform.localPosition - m_listIconGrid[id + 1].transform.localPosition).y - ICONGRIDSPACE * 0.0008f;
            }
            fScale = 0.001f;
        }

        for (int i = id + 1; i < m_listIconGrid.Count; ++i)
        {
            TweenPosition tp = m_listIconGrid[i].GetComponentsInChildren<TweenPosition>(true)[0];
            tp.from = m_listIconGrid[i].transform.localPosition;
            tp.to = tp.from + new Vector3(0, fOffect, 0);
            tp.Reset();
            tp.Play(true);
        }

        ts.from = m_listIconGrid[id].transform.Find("EquipmentUIComposeIconListGridChildList").localScale;
        ts.to = new Vector3(1, fScale, 1);
        ts.Reset();
        ts.callWhenFinished = "OnTSEnd";
        ts.eventReceiver = gameObject;
        ts.Play(true);
    }

    public void SetCurrentGridDown(int parentId, int childId)
    {
        //Debug.LogError("SetCurrentGridDown");
        ScaleIconGridList(parentId, false);

        //if (m_iOldParentId != -1)
        //{
        //    m_myTransform.FindChild(m_widgetToFullName["ComposeDialogIconListPanelTable"]).FindChild(m_iOldParentId + "/EquipmentUIComposeIconListGridChildList").
        //        GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(childId);
        //}
        //else
        //{
        m_listIconGrid[parentId].transform.Find("EquipmentUIComposeIconListGridChildList").
        gameObject.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(childId);
        //}

        m_iOldParentId = parentId;
    }

    public void RemoveIconList()
    {
        for (int i = 0; i < m_listIconGridChild.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listIconGridChild[i]);
        }
        for (int i = 0; i < m_listIconGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listIconGrid[i]);
        }

        m_listIconGrid.Clear();
        m_listIconGridChild.Clear();

        m_bIsAllLoadDone = false;
    }

    public int IconGridNum = 0;
    public int IconChildGridNum = 0;

    public void AddIconListGrid(string name, int id)
    {
        //GameObject obj;

        //obj = (GameObject)Instantiate(Resources.Load("GUI/EquipmentUIComposeIconListGrid"));
        //obj.name = name;
        //obj.transform.parent = m_transPanelTable;
        //obj.transform.localPosition = new Vector3(0, 0, 0);
        //// obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;
        //obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 0.0008f);
        //obj.transform.FindChild("EquipmentUIComposeIconListGridChildList").gameObject.AddComponent<MogoSingleButtonList>();
        //obj.transform.FindChild("EquipmentUIComposeIconListGridChildList").gameObject.GetComponentsInChildren<MogoSingleButtonList>(true)[0].IsImage = false;

        //obj.transform.FindChild("EquipmentUIComposeIconListGridParent").GetComponentsInChildren<MyDragCamera>(t rue)[0].RelatedCamera = m_dragCamera;
        //m_listIconGrid.Add(obj);
        AssetCacheMgr.GetUIInstance("EquipmentUIComposeIconListGrid.prefab", (prefab, guid, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.name = id.ToString();
            obj.GetComponentsInChildren<UILabel>(true)[0].text = name;
            obj.transform.parent = m_transPanelTable;
            obj.transform.localPosition = new Vector3(0, -(m_listIconGrid.Count) * ICONGRIDSPACE * 0.0008f, 0);
            // obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;
            obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
            obj.transform.Find("EquipmentUIComposeIconListGridChildList").gameObject.AddComponent<MogoSingleButtonList>().IsAutoResetToFirstPage = false;
            obj.transform.Find("EquipmentUIComposeIconListGridChildList").gameObject.GetComponentsInChildren<MogoSingleButtonList>(true)[0].IsImage = false;

            obj.transform.Find("EquipmentUIComposeIconListGridParent").GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;
            obj.transform.Find("EquipmentUIComposeIconListGridParent").GetComponentsInChildren<ComposeIconGrid>(true)[0].id = id;
            m_listIconGrid.Add(obj);

            int iconNum = ComposeUIViewManager.Instance.GetIconGridNum();
            int iconChild = ComposeUIViewManager.Instance.GetIconGridChildNum();
            float f = m_dragCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList[0].localPosition.y - (ICONGRIDSPACE * 0.0008f * (iconNum - 1) + (iconChild - 6) * ICONGRIDCHILDSPACE * 0.0008f);
            //float f = m_dragCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList[0].localPosition.y - (ICONGRIDSPACE * 0.0008f * (iconNum - 6));

            m_dragCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = f;

            if (IconGridNum == iconNum && IconChildGridNum == GetIconGridChildNum(false))
            {
                m_bIsAllLoadDone = true;
                if (MogoUIManager.Instance.ComposeUILoaded != null)
                {
                    MogoUIManager.Instance.ComposeUILoaded();
                }

                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            }
        });
    }

    public bool m_bIsAllLoadDone = false;
    private GameObject m_goComposeBtn;
    public void AddIconListGridChild(int parent, int id, string name)
    {
        AssetCacheMgr.GetUIInstance("EquipmentUIComposeIconListGridChild.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;

                obj.name = parent.ToString() + "." + id.ToString();
                obj.GetComponentsInChildren<UILabel>(true)[0].text = name;
                obj.GetComponentsInChildren<UILabel>(true)[1].text = name;
                obj.transform.parent = m_listIconGrid[parent].transform.Find("EquipmentUIComposeIconListGridChildList");
                obj.transform.localPosition = new Vector3(0, -(obj.transform.parent.GetChildCount() - 1) * ICONGRIDCHILDSPACE, 0);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;
                obj.transform.GetComponentsInChildren<ComposeIconChildGrid>(true)[0].id = id;
                obj.transform.GetComponentsInChildren<ComposeIconChildGrid>(true)[0].parentId = parent;

                obj.transform.localScale = new Vector3(1, 1, 1);
                m_listIconGridChild.Add(obj);

                m_listIconGrid[parent].transform.Find("EquipmentUIComposeIconListGridChildList").GetComponentsInChildren<MogoSingleButtonList>(true)[0]
                    .SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);

                int iconNum = ComposeUIViewManager.Instance.GetIconGridNum();
                int iconChild = ComposeUIViewManager.Instance.GetIconGridChildNum();
                float f = m_dragableCamera.transformList[0].localPosition.y - (ICONGRIDSPACE * 0.0008f * (iconNum - 1) + (iconChild - 6) * ICONGRIDCHILDSPACE * 0.0008f);

                m_dragableCamera.MINY = f;

                if (IconGridNum == iconNum && IconChildGridNum == GetIconGridChildNum(false))
                {
                    m_bIsAllLoadDone = true;
                    m_dragableCamera.SetArrow();

                    if (MogoUIManager.Instance.ComposeUILoaded != null)
                    {
                        MogoUIManager.Instance.ComposeUILoaded();
                        MogoUIManager.Instance.ComposeUILoaded = null;
                    }

                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                }
            });
    }

    public void RepositionNow()
    {
        m_transPanelTable.GetComponentsInChildren<UITable>(true)[0].repositionNow = true;

        //for (int i = 0; i < 7; ++i)
        //{
        //    if (m_bGridStatus[i] == true)
        //    {
        //        ScaleIconGridList(i);
        //    }
        //}
    }

    public int GetIconGridNum()
    {
        return m_listIconGrid.Count;
    }

    public int GetIconGridChildNum(bool isOpened = true)
    {
        int num = 0;

        if (isOpened)
        {
            for (int i = 0; i < m_listIconGrid.Count; ++i)
            {
                if (m_listIconGrid[i].transform.Find("EquipmentUIComposeIconListGridChildList").localScale.y >= 0.99f)
                {
                    num += m_listIconGrid[i].transform.Find("EquipmentUIComposeIconListGridChildList").GetChildCount();
                }
            }
        }
        else
        {
            for (int i = 0; i < m_listIconGrid.Count; ++i)
            {
                num += m_listIconGrid[i].transform.Find("EquipmentUIComposeIconListGridChildList").GetChildCount();
            }
        }
        return num;
    }


    public void SetComposePearlUpIcon(string imgName, int color = 0, int itemId = 0)
    {
        m_ssComposePearlUp.spriteName = imgName;
        MogoUtils.SetImageColor(m_ssComposePearlUp, color);

        m_goGOComposeDialogBodyPearlUp.GetComponentsInChildren<InventoryGrid>(true)[0].iconID = itemId;
    }

    public void SetComposePearlLeftIcon(string imgName, int color = 0, int itemId = 0)
    {
        m_ssComposePearlLeft.spriteName = imgName;
        MogoUtils.SetImageColor(m_ssComposePearlLeft, color);

        m_goGOComposeDialogBodyPearlLeft.GetComponentsInChildren<InventoryGrid>(true)[0].iconID = itemId;
    }

    public void SetComposePearlRightIcon(string imgName, int color = 0, int itemId = 0)
    {
        m_ssComposePearlRight.spriteName = imgName;
        MogoUtils.SetImageColor(m_ssComposePearlRight, color);

        m_goGOComposeDialogBodyPearlRight.GetComponentsInChildren<InventoryGrid>(true)[0].iconID = itemId;
    }

    public void SetComposePearlFinalIcon(string imgName, int color = 0, int itemId = 0)
    {
        m_ssComposePearlFinal.spriteName = imgName;
        MogoUtils.SetImageColor(m_ssComposePearlFinal, color);

        m_goGOComposeDialogBodyPearlFinal.GetComponentsInChildren<InventoryGrid>(true)[0].iconID = itemId;
    }

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        Initialize();

        m_dragCamera = m_myTransform.Find(m_widgetToFullName["ComposeDialogIconListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_dragCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_dragCamera.GetComponentsInChildren<UIViewport>(true)[0].topLeft = GameObject.Find("EquipmentUIIconListBGTopLeft").transform;
        m_dragCamera.GetComponentsInChildren<UIViewport>(true)[0].bottomRight = GameObject.Find("EquipmentUIIconListBGBottomRight").transform;

        m_dragableCamera = m_dragCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragableCamera.LeftArrow = FindTransform("ComposeDialogIconListArrowU").gameObject;
        m_dragableCamera.RightArrow = FindTransform("ComposeDialogIconListArrowD").gameObject;

        m_transPanelTable = m_myTransform.Find(m_widgetToFullName["ComposeDialogIconListPanelTable"]);

        m_goGOComposeDialogBodyPearlUp = m_myTransform.Find(m_widgetToFullName["GOComposeDialogBodyPearlUp"]).gameObject;
        m_goGOComposeDialogBodyPearlLeft = m_myTransform.Find(m_widgetToFullName["GOComposeDialogBodyPearlLeft"]).gameObject;
        m_goGOComposeDialogBodyPearlRight = m_myTransform.Find(m_widgetToFullName["GOComposeDialogBodyPearlRight"]).gameObject;
        m_goGOComposeDialogBodyPearlFinal = m_myTransform.Find(m_widgetToFullName["GOComposeDialogBodyPearlFinal"]).gameObject;
        m_goGOComposeDialogBodyPearlUp.AddComponent<InventoryGrid>();
        m_goGOComposeDialogBodyPearlLeft.AddComponent<InventoryGrid>();
        m_goGOComposeDialogBodyPearlRight.AddComponent<InventoryGrid>();
        m_goGOComposeDialogBodyPearlFinal.AddComponent<InventoryGrid>();

        m_ssComposePearlLeft = m_myTransform.Find(m_widgetToFullName["ComposeDialogBodyPearlLeft"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_ssComposePearlRight = m_myTransform.Find(m_widgetToFullName["ComposeDialogBodyPearlRight"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_ssComposePearlUp = m_myTransform.Find(m_widgetToFullName["ComposeDialogBodyPearlUp"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_ssComposePearlFinal = m_myTransform.Find(m_widgetToFullName["ComposeDialogBodyPearlFinal"]).GetComponentsInChildren<UISlicedSprite>(true)[0];

        m_goComposeNowBtn = m_myTransform.Find(m_widgetToFullName["ComposeDialogComposeNow"]).gameObject;
        m_goComposeBtn = m_myTransform.Find(m_widgetToFullName["ComposeDialogCompose"]).gameObject;

        for (int i = 0; i < 9; ++i)
        {
            m_bGridStatus[i] = false;
        }

        m_texUIBG = m_myTransform.Find(m_widgetToFullName["ComposeDialogBodyBGThing0"]).GetComponentsInChildren<UITexture>(true)[0];
    }

    #region 事件

    void Initialize()
    {
        ComposeUILogicManager.Instance.Initialize();

        ComposeUIDict.ButtonTypeToEventUp.Add("ComposeDialogBuy", OnComposeBuyUp);
        ComposeUIDict.ButtonTypeToEventUp.Add("ComposeDialogCompose", OnComposeComposeUp);
        ComposeUIDict.ButtonTypeToEventUp.Add("ComposeDialogComposeNow", OnComposeComposeNowUp);
        EventDispatcher.AddEventListener<int, int>("ComposeIconChildGridUp", OnComposeIconChildGridUp);
        EventDispatcher.AddEventListener<int>("ComposeIconGridUp", OnComposeIconGridUp);
    }

    public void Release()
    {
        ComposeUILogicManager.Instance.Release();
        EventDispatcher.RemoveEventListener<int, int>("ComposeIconChildGridUp", OnComposeIconChildGridUp);
        EventDispatcher.RemoveEventListener<int>("ComposeIconGridUp", OnComposeIconGridUp);


        ComposeUIDict.ButtonTypeToEventUp.Clear();

        for (int i = 0; i < m_listIconGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listIconGrid[i]);
            m_listIconGrid[i] = null;
        }

        for (int i = 0; i < m_listIconGridChild.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listIconGridChild[i]);
            m_listIconGridChild[i] = null;
        }
    }

    #endregion

    public void SetComposeNowBtnEnable(bool isEnable)
    {
        m_goComposeNowBtn.GetComponentsInChildren<BoxCollider>(true)[0].enabled = isEnable;
        m_goComposeNowBtn.GetComponentsInChildren<MogoTwoStatusButton>(true)[0].SetButtonEnable(isEnable);
    }

    public void SetComposeBtnEnable(bool isEnable)
    {
        Mogo.Util.LoggerHelper.Debug("isEnable:" + isEnable);
        m_goComposeBtn.GetComponentsInChildren<BoxCollider>(true)[0].enabled = isEnable;
        m_goComposeBtn.GetComponentsInChildren<MogoTwoStatusButton>(true)[0].SetButtonEnable(isEnable);
    }

    public void PlayUIFXAnim()
    {
        Mogo.Util.LoggerHelper.Debug("Damnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn");
        MogoFXManager.Instance.AttachUIFX(7, MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, null);
    }

    #region 界面打开和关闭

    //void OnEnable()
    //{
        //if (!SystemSwitch.DestroyResource)
        //{
        //    return;
        //}

        //if (m_texUIBG.mainTexture == null)
        //{
        //    AssetCacheMgr.GetUIResource("zb_hecheng_ditu.png", (obj) =>
        //    {
        //        m_texUIBG.mainTexture = (Texture)obj;
        //    });
        //}
    //}


    public void ReleaseUIAndResources()
    {
        ReleasePreLoadResources();
        MogoUIManager.Instance.DestroyComposeUI();
    }

    void ReleasePreLoadResources()
    {
        AssetCacheMgr.ReleaseResourceImmediate("EquipmentUIComposeIconListGrid.prefab");
        AssetCacheMgr.ReleaseResourceImmediate("EquipmentUIComposeIconListGridChild.prefab");
    }
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            ReleaseUIAndResources();
        }
        //if (!SystemSwitch.DestroyResource)
        //{
        //    return;
        //}

        //if (m_texUIBG != null)
        //{
        //    m_texUIBG.mainTexture = null;
        //    AssetCacheMgr.ReleaseResourceImmediate("zb_hecheng_ditu.png");
        //}
    }

    #endregion
}
