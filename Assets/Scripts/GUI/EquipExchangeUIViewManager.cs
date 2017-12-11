/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EquipExchangeUIViewManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013/9/26
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using System;

public class EquipExchangeViewData
{
    public int goldIconId;
    public string title;
    public int itemId;
    public string goldNum;
    public string materialNum;
    public int materialId;
    public Action onExchange;
}

public class EquipExchangeUIViewManager : MogoUIParent
{
    public static EquipExchangeUIViewManager Instance;

    private MogoSingleButtonList m_tab;
    private MogoListView m_listView;

    private Transform m_tranDragableCamera;
    private MyDragableCamera m_dragableCamera;

    private UILabel m_lblGoldNum;
    private UISprite m_imgMaterial;
    private UILabel m_lblMaterilNum;

    private List<GameObject> m_objList = new List<GameObject>();

    public Action<int> OnTabSelect;

    private const string GRID_PREFAB_NAME = "EquipExchangeEquipGrid.prefab";
    private const int GRID_GAP = 310;
    private const int GRID_NUM_PER_PAGE = 3;

    void Awake()
    {
        Instance = this;
        base.Init();
        m_tab = GetUIChild("EquipExchangeTab").GetComponent<MogoSingleButtonList>();

        Transform listRoot = GetUIChild("EquipExchangeEquipList");

        m_tranDragableCamera = GetUIChild("EquipExchangeEquipListCamera");
        m_dragableCamera = m_tranDragableCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragableCamera.LeftArrow = GetUIChild("EquipExchangeArrowLeft").gameObject;
        m_dragableCamera.RightArrow = GetUIChild("EquipExchangeArrowRight").gameObject;

        Transform dragCameraBegin = GetUIChild("EquipExchangeEquipListCameraPosBegin");
        GetUIChild("EquipExchangeContainerCloseBtn").gameObject.AddComponent<MogoUIListener>().MogoOnClick = () => { Hide(); };

        m_listView = new MogoListView(m_tranDragableCamera, listRoot, dragCameraBegin, GRID_PREFAB_NAME,
            true, GRID_GAP, GRID_NUM_PER_PAGE, m_objList);

        m_lblGoldNum = GetUIChild("EquipExchangeResourceGoldNumLbl").GetComponent<UILabel>();
        m_lblMaterilNum = GetUIChild("EquipExchangeResourceDiamondNumLbl").GetComponent<UILabel>();
        m_imgMaterial = GetUIChild("EquipExchangeResourceDiamondIcon").GetComponent<UISprite>();

        GetUIChild("EquipExchangeTabBtn1").gameObject.AddComponent<MogoUIListener>().MogoOnClick = () =>
        {
            if (OnTabSelect != null) OnTabSelect(0);
        };
        GetUIChild("EquipExchangeTabBtn2").gameObject.AddComponent<MogoUIListener>().MogoOnClick = () =>
        {
            if (OnTabSelect != null) OnTabSelect(1);
        };


        gameObject.SetActive(false);
    }

    public void SetGold(string p)
    {
        m_lblGoldNum.text = p;
    }

    public void SetMaterial(int itemID, string num)
    {
        InventoryManager.SetIcon(itemID, m_imgMaterial, 0, null, null);
        m_lblMaterilNum.text = num;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(int idx, List<EquipExchangeViewData> list, bool isShowTab = true)
    {
        gameObject.SetActive(true);
        if (isShowTab)
        {
            m_tab.gameObject.SetActive(true);

            //切换到indexTab
            m_tab.SetCurrentDownButton(idx);
        }
        else
        {
            m_tab.gameObject.SetActive(false);
        }

        m_listView.AddList(null,
            list.Count,
            (index, go) =>
            {
                EquipExchangeViewData viewData = list[index];
                RefreshGrid(go, viewData);

                //m_objList.Add(go);
            });
    }

    private void RefreshGrid(GameObject go, EquipExchangeViewData viewData)
    {
        go.transform.Find("EquipExchangeEquipGridBtn").gameObject.AddComponent<MogoUIListener>().MogoOnClick = viewData.onExchange;

        Transform tUp = go.transform.Find("EquipExchangeEquipGridThings1");
        UISprite fg = tUp.Find("EquipExchangeEquipGridEquipFG").GetComponent<UISprite>();
        UISprite bg = tUp.Find("EquipExchangeEquipGridEquipBG").GetComponent<UISprite>();
        InventoryManager.SetIcon(viewData.itemId, fg, 0, null, bg);
        tUp.Find("EquipExchangeEquipGridEquipName").GetComponent<UILabel>().text = viewData.title;
        tUp.Find("EquipExchangeEquipGridEquipFGFG").GetComponent<MyDragCamera>().RelatedCamera = m_tranDragableCamera.GetComponent<Camera>();

        //显示tip
        tUp.Find("EquipExchangeEquipGridEquipFGFG").gameObject.AddComponent<MogoUIListener>().MogoOnClick = () =>
        {
            InventoryManager.Instance.ShowItemTip(viewData.itemId,true);
        };

        Transform tDown = go.transform.Find("EquipExchangeEquipGridThings2");
        //Debug.LogError(viewData.materialId);
        InventoryManager.SetIcon(viewData.materialId, tDown.Find("EquipExchangeEquipGridDiamondRoot").Find("EquipExchangeEquipGridDiamond").GetComponent<UISprite>(), 0, null, null);
        //t2.FindChild("EquipExchangeEquipGridDiamond").GetComponent<UISprite>().spriteName = viewData.materilaIcon;

        tDown.Find("EquipExchangeEquipGridDiamondRoot").Find("EquipExchangeEquipGridDiamondNum").GetComponent<UILabel>().text = viewData.materialNum;

        if (viewData.goldNum.Equals(string.Empty))
        {
            tDown.Find("EquipExchangeEquipGridGoldRoot").gameObject.SetActive(false);
        }
        else
        {
            tDown.Find("EquipExchangeEquipGridGoldRoot").Find("EquipExchangeEquipGridGoldNum").GetComponent<UILabel>().text = viewData.goldNum;
        }

        if (viewData.goldIconId != 0)
        {
            InventoryManager.SetIcon(viewData.goldIconId, tDown.Find("EquipExchangeEquipGridGoldRoot").Find("EquipExchangeEquipGridImage").GetComponent<UISprite>());

        }
    }

    public void RefreshUI(List<EquipExchangeViewData> viewDataList)
    {
        if (viewDataList.Count != m_objList.Count)
        {
            Debug.LogError("viewDataList.Count != m_objList.Count");
            Debug.LogError("viewDataList.count:" + viewDataList.Count);
            Debug.LogError("m_objList.count:" + m_objList.Count);
            return;
        }

        for (int i = 0; i < viewDataList.Count; i++)
        {
            RefreshGrid(m_objList[i], viewDataList[i]);
        }
    }
}
