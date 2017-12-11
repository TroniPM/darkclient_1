/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：InsetUIViewManager
// 创建者：MaiFeo
// 修改者列表：Joe Mo
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;

public class InsetUIViewManager : MogoUIBehaviour
{
    private static InsetUIViewManager m_instance;
    public static InsetUIViewManager Instance { get { return InsetUIViewManager.m_instance; }}
   
    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();

    private const int PACKAGEITEMNUM = 42;
    private const int PACKAGEITEMNUMONEPAGE = 7;
    private const float PACKAGEITEMSPACE = 0.08f;//0.0925f;

    private const int ICONGRIDNUM = 11; // 总共11个格子
    private const int ICON_GRID_ONE_PAGE = 6; // 一页6个格子
    private const float ICONGRIDSPACE = -0.074f;
    private const float ICON_OFFSET_Y = -0.185f;

    private List<UISlicedSprite> m_listItemFG = new List<UISlicedSprite>();
    private List<UISlicedSprite> m_listItemBG = new List<UISlicedSprite>();

    private List<UILabel> m_listItemNum = new List<UILabel>();
    private List<GameObject> m_listEquipmentGrid = new List<GameObject>();
    private List<GameObject> m_listPackageGrid = new List<GameObject>();

    private Transform m_transPackageItemList;
    private Transform m_transInsetDialogIconList;
    private Transform m_transInsetHoleList;
    private Camera m_dragCamera;
    private Camera m_dragIconCamera;
    private MyDragableCamera m_dragableIconCamera;

    private UISlicedSprite m_ssInsetEquipmentIcon;
    private UISlicedSprite m_ssInsetEquipmentIconBG;

    private UISlicedSprite[] m_arrInsetHoleIcon = new UISlicedSprite[4];
    private UISlicedSprite[] m_arrInsetHoleUnloadSign = new UISlicedSprite[4];
    private UISlicedSprite[] m_arrInsetHoleUpSign = new UISlicedSprite[4];
    private UISlicedSprite[] m_arrInsetHoleBGUp = new UISlicedSprite[4];
    private UISlicedSprite[] m_arrInsetHoleBGDown = new UISlicedSprite[4];
    private UILabel[] m_arrInsetHoleTypeName = new UILabel[4];

    private GameObject[] m_arrInsetHoleTooltip = new GameObject[4];

    private GameObject m_goPackageArea;
    private GameObject m_goInsetDialogDiamondTip;

    private UILabel m_lblDiamondTipName;
    private UILabel m_lblDiamondTipLevel;
    private UILabel m_lblDiamondTipType;
    private UILabel m_lblJewelListDesc;
    private UILabel m_lblDiamondTipDesc;
    private UISlicedSprite m_ssDiamondTipIcon;
    private UISlicedSprite m_ssDiamondTipIconBG;

    private GameObject m_jewelHoleObj;
    private List<GameObject> m_jewelHoleList;
    private Vector3 m_jewleHoleOriginalPos;
    private GameObject m_jewelHole1;
    private GameObject m_jewelHole2;
    private GameObject m_jewelHole3;
    private GameObject m_jewelHole4;
    public const float JEWEL_HOLE_GAP = 65;

    private GameObject m_dialog1;
    private GameObject m_dialog2;
    private GameObject m_dialog2InfoDetail;
    private UILabel m_dialog2Title;
    private UISprite m_dialog2EquipIconFg;
    private UILabel m_dialog2EquipName;
    private UILabel m_dialog2LevelNeed;

    private UITexture m_texInsetSucessSign;
    private UITexture m_texComposeSucessSign;
    private UISprite m_dialog2EquipIconBg;

    private List<GameObject> m_gotransformList = new List<GameObject>();
    
    public void SetInsetEquipmentImage(string imgName)
    {
        m_ssInsetEquipmentIcon.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        m_ssInsetEquipmentIcon.spriteName = imgName;
    }

    public void SetInsetEquipmentImageBG(string imgName)
    {
        m_ssInsetEquipmentIconBG.spriteName = imgName;
    }

    public void SetInsetHoleImage(string imgName, int holeID, int color = 0)
    {
        m_arrInsetHoleIcon[holeID].spriteName = imgName;
        MogoUtils.SetImageColor(m_arrInsetHoleIcon[holeID], color);

        if(imgName.Equals("emptyItem"))
            m_arrInsetHoleTypeName[holeID].gameObject.SetActive(true);
        else
            m_arrInsetHoleTypeName[holeID].gameObject.SetActive(false);
    }

    public void ShowInsetHoleUnLoadSign(bool show, int holeID)
    {
        m_arrInsetHoleUnloadSign[holeID].transform.parent.gameObject.SetActive(show);
    }

    public void ShowInsetHoleUpdateSign(bool show, int holeID)
    {
        m_arrInsetHoleUpSign[holeID].transform.parent.gameObject.SetActive(show);
    }

    public void SetInsetPackageItemImage(int gridId, string imgName, int color = 0)
    {
        //Debug.LogError("gridId:" + gridId + ",imgName:" + imgName + ",color:" + color);
        m_listItemFG[gridId].spriteName = imgName;
        MogoUtils.SetImageColor(m_listItemFG[gridId], color);
        //InventoryManager.SetIcon(itemId, m_listItemFG[gridId], 0, null, m_listItemBG[gridId]);
    }
    public void SetInsetPackageItemImage(int gridId, int itemID)
    {
        InventoryManager.SetIcon(itemID, m_listItemFG[gridId], 0, null, null/*m_listItemBG[gridId]*/);
    }

    public void SetInsetPackageItemNum(int gridId, int num)
    {
        if (num != 0)
        {
            m_listItemNum[gridId].gameObject.SetActive(true);
            m_listItemNum[gridId].text = num.ToString();
        }
        else
        {
            m_listItemNum[gridId].gameObject.SetActive(false);
        }
    }

    public void SetEquipmentName(int gridId, string name)
    {
        m_listEquipmentGrid[gridId].GetComponentsInChildren<UILabel>(true)[0].text = name;
    }

    public void SetEquipmentIcon(int gridId, string imgName, int color = 0)
    {
        UISprite sp = m_listEquipmentGrid[gridId].transform.Find("StrenthenDialogIconGridImg").GetComponentsInChildren<UISprite>(true)[0];
        sp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        sp.spriteName = imgName;
        MogoUtils.SetImageColor(m_listEquipmentGrid[gridId].transform.Find("StrenthenDialogIconGridImg").GetComponentsInChildren<UISprite>(true)[0], color);
    }
    public void SetEquipmentIconBG(int gridId, string imgName)
    {
        UISprite sp = m_listEquipmentGrid[gridId].transform.Find("StrenthenDialogIconGridImgBG").GetComponentsInChildren<UISprite>(true)[0];
        sp.spriteName = imgName;
    }

    public void ShowEquipmentUpSign(int gridId, bool show = true)
    {
        m_listEquipmentGrid[gridId].transform.Find("StrenthenDialogIconGridUp").gameObject.SetActive(show);
    }

    public void SetCurrentInsetHoleGrid(int gridId)
    {
        m_transInsetHoleList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(gridId);
    }

    public void SetCurrentDownGrid(int gridId)
    {
        m_transInsetDialogIconList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(gridId);

        if (gridId >= 5)
        {
            m_dragIconCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].SetCurrentPage(1);
        }
        else
        {
            m_dragIconCamera.GetComponentsInChildren<MyDragableCamera>(true)[0].SetCurrentPage(0);
        }
    }

    public void ShowInsetHoleToolTip(int id, string text, bool isShow)
    {
        m_arrInsetHoleTooltip[id].GetComponentsInChildren<UILabel>(true)[0].text = text;
        m_arrInsetHoleTooltip[id].SetActive(isShow);
    }

    public void ShowInsetDialogDiamondTip(bool isShow)
    {
        m_goInsetDialogDiamondTip.SetActive(isShow);
    }

    public void SetDiamondTipName(string name)
    {
        m_lblDiamondTipName.text = name;
    }

    public void SetDiamondTipLevel(string level)
    {
        m_lblDiamondTipLevel.text = level;
    }

    public void SetDiamondTipType(string type)
    {
        m_lblDiamondTipType.text = type;
    }

    public void SetJewelListTitle(string title)
    {
        m_lblJewelListDesc.text = title;
    }

    public void SetDiamondTipDesc(string desc)
    {
        m_lblDiamondTipDesc.text = desc;
    }

    public void SetDiamondTipIcon(string iconName, int color = 0)
    {
        m_ssDiamondTipIcon.spriteName = iconName;
        MogoUtils.SetImageColor(m_ssDiamondTipIcon, color);

    }

    public void ShowInsetDialog1(bool b)
    {
        m_dialog1.SetActive(b);
    }

    public void ShowInsetDialog2(bool b, bool isShowEquipDetail = false)
    {
        m_dialog2.SetActive(b);
        m_dialog2InfoDetail.SetActive(isShowEquipDetail);
    }
    public void ShowInsetDialog2EquipDetail(bool b)
    {
        m_dialog2InfoDetail.SetActive(b);
    }

    public void SetDialog2Title(string str)
    {
        m_dialog2Title.text = str;
    }

    public void SetDialog2EquipImageFg(string image)
    {
        m_dialog2EquipIconFg.atlas = MogoUIManager.Instance.GetAtlasByIconName(image);
        m_dialog2EquipIconFg.spriteName = image;
    }

    public void SetDialog2EquipName(string name)
    {
        m_dialog2EquipName.text = name;
    }

    public void SetDialog2EquipLevelNeed(string str)
    {
        m_dialog2LevelNeed.text = str;
    }

    public void SetEquipmentTextImg(int gridId, string imgName)
    {
        m_listEquipmentGrid[gridId].transform.Find("StrenthenDialogIconGridTextImg").GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
    }

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        Initialize();

        m_transPackageItemList = m_myTransform.Find(m_widgetToFullName["InsetDialogPackageItemList"]);
        m_transInsetDialogIconList = m_myTransform.Find(m_widgetToFullName["InsetDialogIconList"]);
        m_transInsetHoleList = m_myTransform.Find(m_widgetToFullName["InsetDialogBody"]);

        m_dragCamera = FindTransform("InsetDialogPackageItemListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_dragCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_dragIconCamera = FindTransform("InsetDialogIconListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_dragIconCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_dragIconCamera.GetComponentsInChildren<UIViewport>(true)[0].topLeft = GameObject.Find("EquipmentUIIconListBGTopLeft").transform;
        m_dragIconCamera.GetComponentsInChildren<UIViewport>(true)[0].bottomRight = GameObject.Find("EquipmentUIIconListBGBottomRight").transform;

        m_dragableIconCamera = m_dragIconCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragableIconCamera.LeftArrow = FindTransform("InsetDialogIconListArrowU").gameObject;
        m_dragableIconCamera.RightArrow = FindTransform("InsetDialogIconListArrowD").gameObject;

        m_ssInsetEquipmentIcon = m_myTransform.Find(m_widgetToFullName["InsetDialogEquipFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];

        m_ssInsetEquipmentIconBG = m_myTransform.Find(m_widgetToFullName["InsetDialogEquipBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_goPackageArea = m_myTransform.Find(m_widgetToFullName["InsetDialogPackageListPos"]).gameObject;
        m_goInsetDialogDiamondTip = m_myTransform.Find(m_widgetToFullName["DiamondInfoTip"]).gameObject;

        m_lblDiamondTipDesc = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailEffectNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblDiamondTipLevel = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblDiamondTipName = m_myTransform.Find(m_widgetToFullName["DiamondInfoNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblDiamondTipType = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailTypeNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_ssDiamondTipIcon = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_ssDiamondTipIconBG = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailImageBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];

        m_jewelHoleObj = m_myTransform.Find(m_widgetToFullName["InsetDialogHole"]).gameObject;
        m_jewelHole1 = m_myTransform.Find(m_widgetToFullName["InsetDialogHole0"]).gameObject;
        m_jewelHole2 = m_myTransform.Find(m_widgetToFullName["InsetDialogHole1"]).gameObject;
        m_jewelHole3 = m_myTransform.Find(m_widgetToFullName["InsetDialogHole2"]).gameObject;
        m_jewelHole4 = m_myTransform.Find(m_widgetToFullName["InsetDialogHole3"]).gameObject;

        m_myTransform.Find(m_widgetToFullName["InsetDialog1BGMask"]).gameObject.AddComponent<InsetBGMask>();
        m_jewelHoleList = new List<GameObject>();
        m_jewelHoleList.Add(m_jewelHole1);
        m_jewelHoleList.Add(m_jewelHole2);
        m_jewelHoleList.Add(m_jewelHole3);
        m_jewelHoleList.Add(m_jewelHole4);
        m_jewleHoleOriginalPos = m_myTransform.Find(m_widgetToFullName["InsetDialogHolePos"]).localPosition;

        m_dialog1 = m_myTransform.Find(m_widgetToFullName["InsetDialog1"]).gameObject;
        m_dialog2 = m_myTransform.Find(m_widgetToFullName["InsetDialog2"]).gameObject;
        m_dialog2InfoDetail = m_myTransform.Find(m_widgetToFullName["InsetDialog2Info"]).gameObject;
        m_dialog2Title = m_myTransform.Find(m_widgetToFullName["InsetDialog2TopTitle"]).GetComponentsInChildren<UILabel>(true)[0]; ;
        m_dialog2EquipIconFg = m_myTransform.Find(m_widgetToFullName["InsetDialog2IconFg"]).GetComponentsInChildren<UISprite>(true)[0];
        m_dialog2EquipIconBg = m_myTransform.Find(m_widgetToFullName["InsetDialog2IconBg"]).GetComponentsInChildren<UISprite>(true)[0];
        m_dialog2EquipName = m_myTransform.Find(m_widgetToFullName["InsetDialog2EquipName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_dialog2LevelNeed = m_myTransform.Find(m_widgetToFullName["InsetDialog2NeedLevel"]).GetComponentsInChildren<UILabel>(true)[0];

        m_lblJewelListDesc = m_myTransform.Find(m_widgetToFullName["InsetDialogPackageText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_texInsetSucessSign = m_myTransform.Find(m_widgetToFullName["InsetUISucessSign"]).GetComponentsInChildren<UITexture>(true)[0];
        m_texComposeSucessSign = m_myTransform.Find(m_widgetToFullName["InsetUIComposeSucessSign"]).GetComponentsInChildren<UITexture>(true)[0];

        m_myTransform.Find(m_widgetToFullName["InsetDialog1Buy"]).gameObject.AddComponent<MogoUIListener>().MogoOnClick =
        () =>
        {
            EventDispatcher.TriggerEvent(InsetManager.ON_BUY);
        };

        for (int i = 0; i < 4; ++i)
        {
            m_arrInsetHoleIcon[i] = m_myTransform.Find(m_widgetToFullName["InsetDialogHole" + i + "FG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
            m_arrInsetHoleUnloadSign[i] = m_myTransform.Find(m_widgetToFullName["InsetDialogHole" + i + "Unload"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
            m_arrInsetHoleUpSign[i] = m_myTransform.Find(m_widgetToFullName["InsetDialogHole" + i + "Update"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
            m_arrInsetHoleTooltip[i] = m_myTransform.Find(m_widgetToFullName["InsetDialogHole" + i + "ToolTip"]).gameObject;
            m_arrInsetHoleBGDown[i] = m_myTransform.Find(m_widgetToFullName["InsetDialogHole" + i + "BG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
            m_arrInsetHoleBGUp[i] = m_myTransform.Find(m_widgetToFullName["InsetDialogHole" + i + "BGUp"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
            m_arrInsetHoleTypeName[i] = m_myTransform.Find(m_widgetToFullName["InsetDialogHole" + i + "TypeName"]).GetComponentsInChildren<UILabel>(true)[0];
        }

        for (int i = 0; i < 6; ++i)
        {
            m_gotransformList.Add(m_myTransform.Find(m_widgetToFullName["transformList" + i]).gameObject);
        }

        for (int i = 0; i < 6; i++)
        {
            if (i > 0)
            {
                m_gotransformList[i].transform.localPosition = new Vector3(
                    m_gotransformList[0].transform.localPosition.x + PACKAGEITEMNUMONEPAGE * PACKAGEITEMSPACE * i,
                    m_gotransformList[i].transform.localPosition.y,
                    m_gotransformList[i].transform.localPosition.z);
            }
        }

        bool m_bIsAllLoaded = false;

        for (int i = 0; i < ICONGRIDNUM; ++i)
        {
            int index = i;

            AssetCacheMgr.GetUIInstance("StrenthenDialogIconGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_transInsetDialogIconList;
                obj.transform.localPosition = new Vector3(0, ICONGRIDSPACE * index, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragIconCamera;
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].m_goDraggableArea = m_goPackageArea;
                obj.name = "InsetIconListIcon" + index.ToString();
                var s = m_transInsetDialogIconList.GetComponentsInChildren<MogoSingleButtonList>(true)[0] as MogoSingleButtonList;
                s.SingleButtonList.Add(obj.GetComponentsInChildren<MogoSingleButton>(true)[0]);
                obj.AddComponent<InsetUIEquipmentGrid>().id = index;

                m_listEquipmentGrid.Add(obj);

                if (m_listEquipmentGrid.Count == ICONGRIDNUM)
                {
                    // 滑动形式需要处理(翻页不需要设置)
                    if (!m_dragableIconCamera.IsMovePage)
                    {
                        m_dragableIconCamera.FPageHeight = ICONGRIDSPACE * ICON_GRID_ONE_PAGE;
                        m_dragableIconCamera.MAXY = ICON_OFFSET_Y;
                        if (m_listEquipmentGrid.Count > ICON_GRID_ONE_PAGE)
                            m_dragableIconCamera.MINY = (m_listEquipmentGrid.Count - ICON_GRID_ONE_PAGE) * ICONGRIDSPACE + ICON_OFFSET_Y;
                        else
                            m_dragableIconCamera.MINY = m_dragableIconCamera.MAXY;
                    }                 
                }

                if (!m_bIsAllLoaded && m_listPackageGrid.Count == PACKAGEITEMNUM && m_listEquipmentGrid.Count == ICONGRIDNUM)
                {
                    EventDispatcher.TriggerEvent(InsetManager.ON_INSET_SHOW);

                    if (MogoUIManager.Instance.InsetUILoaded != null)
                    {
                        MogoUIManager.Instance.InsetUILoaded();
                        MogoUIManager.Instance.InsetUILoaded = null;
                    }               

                    m_bIsAllLoaded = true;                 
                }

                if (index == 0)
                    InsetTabDown(index);
                else
                    InsetTabUp(index);
            });
        }

        for (int i = 0; i < PACKAGEITEMNUM; ++i)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("InsetDialogPackageGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.name = "InsetDialogPackageGrid" + index;
                obj.transform.parent = m_transPackageItemList;
                obj.transform.localPosition = new Vector3(PACKAGEITEMSPACE * index - 0.012f, 0, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);
                m_listItemFG.Add(obj.transform.GetComponentsInChildren<UISlicedSprite>(true)[1]);
                m_listItemNum.Add(obj.transform.GetComponentsInChildren<UILabel>(true)[0]);
                m_listItemBG.Add(obj.transform.GetComponentsInChildren<UISlicedSprite>(true)[0]);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_dragCamera;

                obj.AddComponent<InsetUIPackageGrid>().ID = index;

                m_listPackageGrid.Add(obj);

                if (!m_bIsAllLoaded && m_listPackageGrid.Count == PACKAGEITEMNUM && m_listEquipmentGrid.Count == ICONGRIDNUM)
                {
                    EventDispatcher.TriggerEvent(InsetManager.ON_INSET_SHOW);

                    if (MogoUIManager.Instance.InsetUILoaded != null)
                    {
                        //Debug.LogError("InsetUILoaded != null");
                        MogoUIManager.Instance.InsetUILoaded();
                        MogoUIManager.Instance.InsetUILoaded = null;
                    }
                    //else
                    //{
                    //    Debug.LogError("InsetUILoaded == null");
                    //}

                    m_bIsAllLoaded = true;
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                }
            });
        }
    }

    #region 事件

    void OnPutOn(int ID)
    {
        EventDispatcher.TriggerEvent(InsetManager.ON_PUT_ON);
    }

    void OnInsetHoleUp(int ID)
    {

        if (InsetUIDict.INSETDIAMONDGRIDUP != null)
            InsetUIDict.INSETDIAMONDGRIDUP(ID);
    }

    void OnInsetHoleUpDouble(int ID)
    {
        if (InsetUIDict.INSETDIAMONDGRIDUPDOUBLE != null)
            InsetUIDict.INSETDIAMONDGRIDUPDOUBLE(ID);
    }

    void OnInsetHoleUnLoadUp(int ID)
    {
        if (InsetUIDict.INSETDIAMONDUNLOADUP != null)
            InsetUIDict.INSETDIAMONDUNLOADUP(ID);
    }

    void OnInsetHoleUpdateUp(int ID)
    {

        if (InsetUIDict.INSETDIAMONDUPDATEUP != null)
            InsetUIDict.INSETDIAMONDUPDATEUP(ID);
    }

    void OnInsetPackageGridDragBegin(int ID)
    {
        if (InsetUIDict.INSETPACKAGEGRIDDRAGBEGIN != null)
            InsetUIDict.INSETPACKAGEGRIDDRAGBEGIN(ID);

    }

    void OnInsetPackageGridDrag(int newGrid, int oldGrid)
    {

        if (InsetUIDict.INSETPACKAGEGRIDDRAG != null)
            InsetUIDict.INSETPACKAGEGRIDDRAG(newGrid, oldGrid);
    }

    void OnInsetDialogDiamondTipInsetUp(int i)
    {
        if (InsetUIDict.INSETDIALOGDIAMONDTIPINSETUP != null)
            InsetUIDict.INSETDIALOGDIAMONDTIPINSETUP(i);
    }

    void OnInsetDialogDiamondTipCloseUp(int i)
    {
        ShowInsetDialogDiamondTip(false);
    }

    void Initialize()
    {
        InsetUILogicManager.Instance.Initialize();

        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole0", OnInsetHoleUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole1", OnInsetHoleUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole2", OnInsetHoleUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole3", OnInsetHoleUp);

        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole0Double", OnInsetHoleUpDouble);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole1Double", OnInsetHoleUpDouble);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole2Double", OnInsetHoleUpDouble);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole3Double", OnInsetHoleUpDouble);

        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole0Unload", OnInsetHoleUnLoadUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole1Unload", OnInsetHoleUnLoadUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole2Unload", OnInsetHoleUnLoadUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole3Unload", OnInsetHoleUnLoadUp);

        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole0Update", OnInsetHoleUpdateUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole1Update", OnInsetHoleUpdateUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole2Update", OnInsetHoleUpdateUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogHole3Update", OnInsetHoleUpdateUp);
        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialog2PutonBtn", OnPutOn);

        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogDiamondInfoTipInset", OnInsetDialogDiamondTipInsetUp);

        InsetUIDict.ButtonTypeToEventUp.Add("InsetDialogDiamondInfoTipClose", OnInsetDialogDiamondTipCloseUp);

        EventDispatcher.AddEventListener<int>("InsetPackageGridDragBegin", OnInsetPackageGridDragBegin);
        EventDispatcher.AddEventListener<int, int>("InsetUIPackageGridDrag", OnInsetPackageGridDrag);
    }

    public void Release()
    {
        InsetUILogicManager.Instance.Release();

        InsetUIDict.ButtonTypeToEventUp.Clear();

        EventDispatcher.RemoveEventListener<int>("InsetPackageGridDragBegin", OnInsetPackageGridDragBegin);
        EventDispatcher.RemoveEventListener<int, int>("InsetUIPackageGridDrag", OnInsetPackageGridDrag);

        for (int i = 0; i < m_listEquipmentGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listEquipmentGrid[i]);
            m_listEquipmentGrid[i] = null;
        }

        for (int i = 0; i < m_listPackageGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listPackageGrid[i]);
            m_listPackageGrid[i] = null;
        }
    }

    #endregion

    public void SetAllJewelSlotUp()
    {
        m_transInsetHoleList.GetComponent<MogoSingleButtonList>().SetAllButtonUp();
    }
    public void SetJewelSlotCurrentDown(int index)
    {
        m_transInsetHoleList.GetComponent<MogoSingleButtonList>().SetCurrentDownButton(index);
    }

    public void SetDiamondTipIconBG(string p)
    {
        m_ssDiamondTipIconBG.spriteName = p;
    }

    public void SetInsetHoleBGDownImg(string img, int id)
    {
        m_arrInsetHoleBGDown[id].spriteName = img;
    }

    public void SetInsetHoleBGUpImg(string img, int id)
    {
        m_arrInsetHoleBGUp[id].spriteName = img;        
    }

    public void SetInsetHoleTypeName(int holeType, int id)
    {
        m_arrInsetHoleTypeName[id].text = LanguageData.GetContent(holeType + 274);
    }

    public void ResetJewelHole()
    {
        m_jewelHoleObj.transform.localPosition = m_jewleHoleOriginalPos;
        foreach (GameObject go in m_jewelHoleList)
        {
            go.SetActive(false);
        }
    }

    public void SetJewelHoleList(List<InsetManager.JewelHoleViewData> dataList)
    {
        ResetJewelHole();
        if (dataList.Count <= 0) return;
        for (int i = 0; i < dataList.Count; i++)
        {
            m_jewelHoleList[i].SetActive(true);
            SetInsetHoleImage(dataList[i].fg, i, dataList[i].fgColor);
            SetInsetHoleBGUpImg(dataList[i].bgUp, i);
            SetInsetHoleBGDownImg(dataList[i].bgDown, i);
            SetInsetHoleTypeName(dataList[i].holeType, i);

            ShowInsetHoleUnLoadSign(dataList[i].canRemove, i);
            ShowInsetHoleToolTip(i, dataList[i].tipDesc, dataList[i].isShowTip);
            ShowInsetHoleUpdateSign(dataList[i].canUpgrade, i);            
        }

        float dx = -JEWEL_HOLE_GAP * (dataList.Count - 1);
        Vector3 temp = new Vector3(m_jewleHoleOriginalPos.x + dx, m_jewleHoleOriginalPos.y, m_jewleHoleOriginalPos.z);
        m_jewelHoleObj.transform.localPosition = temp;
    }

    public void ShowInsetSucessSign(bool isShow)
    {
        m_texInsetSucessSign.gameObject.SetActive(isShow);

        TweenAlpha ta = m_texInsetSucessSign.GetComponentsInChildren<TweenAlpha>(true)[0];
        ta.Reset();
        ta.enabled = true;
        ta.Play(true);

        m_texInsetSucessSign.gameObject.AddComponent<MogoUIAnimPowerup>();
    }

    public void ShowComposeSucessSign(bool isShow)
    {
        m_texComposeSucessSign.gameObject.SetActive(isShow);

        TweenAlpha ta = m_texComposeSucessSign.GetComponentsInChildren<TweenAlpha>(true)[0];
        ta.Reset();
        ta.enabled = true;
        ta.Play(true);
        m_texComposeSucessSign.gameObject.AddComponent<MogoUIAnimPowerup>();
    }

    public void PlayUIFXAnim()
    {
        MogoFXManager.Instance.AttachParticleAnim("fx_ui_gem.prefab", 2000,
                         m_ssInsetEquipmentIcon.transform.position, GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0], 0, 0, 0);
    }

    public void SetDialog2EquipImage(int id)
    {
        InventoryManager.SetIcon(id, m_dialog2EquipIconFg, 0, null, m_dialog2EquipIconBg);
    }

    #region Tab Change
    public void HandleInsetTabChange(int fromTab, int toTab)
    {
        InsetTabUp(fromTab);
        InsetTabDown(toTab);
    }

    void InsetTabUp(int tab)
    {
        UILabel lblFromTab = GetInsetEquipmentGridText(tab);
        if (lblFromTab != null)
        {
            lblFromTab.color = new Color32(37, 29, 6, 255);
            lblFromTab.effectStyle = UILabel.Effect.None;
        }
    }

    void InsetTabDown(int tab)
    {
        UILabel lblToTab = GetInsetEquipmentGridText(tab);
        if (lblToTab != null)
        {
            lblToTab.color = new Color32(255, 255, 255, 255);
            lblToTab.effectStyle = UILabel.Effect.Outline;
            lblToTab.effectColor = new Color32(53, 22, 2, 255);
        }
    }

    UILabel GetInsetEquipmentGridText(int tab)
    {
        UILabel lblGridText = null;

        if (tab < 0 || tab >= m_listEquipmentGrid.Count)
            tab = 0;

        if (tab >= 0 && tab < m_listEquipmentGrid.Count)
        {
            GameObject goEquipmentGrid = m_listEquipmentGrid[tab];
            if (goEquipmentGrid != null)
            {
                Transform tranGridText = goEquipmentGrid.transform.Find("StrenthenDialogIconGridText");
                if (tranGridText != null)
                    lblGridText = tranGridText.GetComponent<UILabel>();
            }
        }

        return lblGridText;
    }
    #endregion

    #region 界面打开和关闭

    public void ReleaseUIAndResources()
    {
        RelasePreLoadResources();
        MogoUIManager.Instance.DestroyInsetUI();
    }

    void RelasePreLoadResources()
    {
        AssetCacheMgr.ReleaseResourceImmediate("StrenthenDialogIconGrid.prefab");
        AssetCacheMgr.ReleaseResourceImmediate("InsetDialogPackageGrid.prefab");
    }

    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            ReleaseUIAndResources();
        }
    }

    #endregion
}
