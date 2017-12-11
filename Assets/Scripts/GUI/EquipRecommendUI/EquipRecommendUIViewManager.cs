#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using System;
using Mogo.Game;
using Mogo.GameData;

public class EquipRecommendUIViewManager : MogoUIBehaviour
{
    private static EquipRecommendUIViewManager m_instance;
    public static EquipRecommendUIViewManager Instance  { get { return EquipRecommendUIViewManager.m_instance; } }

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量
    
    private Transform m_tranEquipRecommendUIEquipList;
    private Camera m_equipListCamera;
    private MyDragableCamera m_dragableCamera;
    private Vector3 m_equipListcameraInitPos;
    private GameObject m_goEquipRecommendUIArrowB;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranEquipRecommendUIEquipList = FindTransform("EquipRecommendUIEquipList");
        m_equipListCamera = FindTransform("EquipRecommendUIEquipListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_equipListCamera.GetComponent<UIViewport>().sourceCamera = GameObject.Find("Camera").GetComponent<Camera>();
        m_dragableCamera = m_equipListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_equipListcameraInitPos = m_equipListCamera.transform.localPosition;
        m_goEquipRecommendUIArrowB = FindTransform("EquipRecommendUIArrowB").gameObject;

        FindTransform("EquipRecommendUIBtnClose").gameObject.AddComponent<EquipRecommendUIButton>();

        Initialize();
    }

    #region 事件

    public Action EQUIPRECOMMENDUICLOSEUP;
    public Action EQUIPRECOMMENDUICALDATALIST;

    public void Initialize()
    {
        EquipRecommendUIDict.ButtonTypeToEventUp.Add("EquipRecommendUIBtnClose", OnCloseUp);

        EquipRecommendUILogicManager.Instance.Initialize();
        m_uiLoginManager = EquipRecommendUILogicManager.Instance;
    }

    public void Release()
    {
        EquipRecommendUILogicManager.Instance.Release();
        EquipRecommendUIDict.ButtonTypeToEventUp.Clear();
    }

    private void OnCloseUp(int i)
    {
        if (EQUIPRECOMMENDUICLOSEUP != null)
            EQUIPRECOMMENDUICLOSEUP();
    }

    #endregion

    #region 装备推荐Grid

    readonly static float ITEMSPACEVERTICAL = -110;
    readonly static int GRID_COUNT_ONE_PAGE = 4;
    readonly static float OFFSET_Y = 0.0f;
    private Dictionary<int, EquipRecommendUIGrid> m_maplistEquipRecommendUIGrid = new Dictionary<int, EquipRecommendUIGrid>();

    /// <summary>
    /// 设置推荐装备Grid列表
    /// </summary>
    /// <param name="idList"></param>
    public void SetEquipRecommendList(int count, Action action)
    {  
        AddEquipRecommendGridList(count, () =>
        {
            if (action != null)
                action();
        });
    }

    /// <summary>
    /// 创建推荐装备Grid列表
    /// </summary>
    /// <param name="num"></param>
    /// <param name="act"></param>
    void AddEquipRecommendGridList(int num, Action action = null)
    {
        ClearEquipRecommendGridList();
        ResetCameraPos();
        ShowEquipRecommendGridListArrow(num);

        // 删除翻页位置(滑动形式不需要处理)
        if (m_dragableCamera.IsMovePage)
        {
            if (m_dragableCamera.transformList != null)
            {
                for (int i = 0; i < m_dragableCamera.transformList.Count; ++i)
                    Destroy(m_dragableCamera.transformList[i].gameObject);

                m_dragableCamera.transformList.Clear();
            }
        }       

        for (int i = 0; i < num; ++i)
        {
            int index = i;

            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

            AssetCacheMgr.GetUIInstance("EquipRecommendUIGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranEquipRecommendUIEquipList;
                obj.transform.localPosition = new Vector3(0, ITEMSPACEVERTICAL * index, 0);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_equipListCamera;
                EquipRecommendUIGrid gridUI = obj.AddComponent<EquipRecommendUIGrid>();

                if (m_maplistEquipRecommendUIGrid.ContainsKey(index))
                    AssetCacheMgr.ReleaseInstance(m_maplistEquipRecommendUIGrid[i].gameObject);
                m_maplistEquipRecommendUIGrid[index] = gridUI;

                // 创建翻页位置(滑动形式不需要处理)
                if (m_dragableCamera.IsMovePage)
                {
                    if (index % GRID_COUNT_ONE_PAGE == 0)
                    {
                        GameObject trans = new GameObject();
                        trans.transform.parent = m_equipListCamera.transform;
                        trans.transform.localPosition = new Vector3(0, index / GRID_COUNT_ONE_PAGE * ITEMSPACEVERTICAL * GRID_COUNT_ONE_PAGE, 0);
                        trans.transform.localEulerAngles = Vector3.zero;
                        trans.transform.localScale = new Vector3(1, 1, 1);
                        trans.name = "DragListPosVertical" + index / GRID_COUNT_ONE_PAGE;
                        m_dragableCamera.transformList.Add(trans.transform);
                    }
                }            

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                if (index == num - 1)
                {
                    // 滑动形式和翻页形式都需要设置
                    //if (!m_dragableCamera.IsMovePage)
                    {
                        m_dragableCamera.FPageHeight = ITEMSPACEVERTICAL * GRID_COUNT_ONE_PAGE;
                        m_dragableCamera.MAXY = OFFSET_Y;
                        if (m_maplistEquipRecommendUIGrid.Count > GRID_COUNT_ONE_PAGE)
                            m_dragableCamera.MINY = (m_maplistEquipRecommendUIGrid.Count - GRID_COUNT_ONE_PAGE) * ITEMSPACEVERTICAL + OFFSET_Y;
                        else
                            m_dragableCamera.MINY = m_dragableCamera.MAXY;

                        if (action != null)
                            action();
                    }
                }
            });
        }
    }

    /// <summary>
    /// 设置推荐装备Grid数据
    /// </summary>
    /// <param name="m_equipRecommendGridDataList"></param>
    public void SetEquipRecommendDataList(List<EquipRecommendGridData> m_equipRecommendGridDataList)
    {
        for (int index = 0; index < m_equipRecommendGridDataList.Count; index++)
        {
            if (!m_maplistEquipRecommendUIGrid.ContainsKey(index))
                continue;

            int recommendScore = 0;
            int currentScore = 0;

            EquipRecommendGridData gridData = m_equipRecommendGridDataList[index];
            EquipRecommendUIGrid gridUI = m_maplistEquipRecommendUIGrid[index];

            gridUI.LoadResourceInsteadOfAwake();
            gridUI.SetEquipRecommendAccess(gridData.access, gridData.accessType);

            if (gridData.currentItemEquipment != null)
            {
                gridUI.SetCurrentEquipID(gridData.currentItemEquipment.templateId);
                gridUI.SetCurrentEquip(gridData.currentItemEquipment.icon, gridData.currentItemEquipment.color, gridData.currentItemEquipment.quality);
                gridUI.SetCurrentEquipName(gridData.currentItemEquipment.name, true);

                currentScore = gridData.currentItemEquipment.GetScore(MogoWorld.thePlayer.level);
                gridUI.SetCurrentEquipScore((currentScore));
            }
            else
            {
                gridUI.SetCurrentEquipID(0);
                gridUI.SetCurrentEquip(EquipSlotIcon.icons[gridData.equipSlot], 0);
                gridUI.SetCurrentEquipName(EquipSlotName.GetEquipSlotNameBySlot(gridData.equipSlot), false);
                gridUI.SetCurrentEquipScore(0);
            }

            ItemEquipmentData recommendEquipData = ItemEquipmentData.GetItemEquipmentData(gridData.recommendEquipID);
            if (recommendEquipData != null)
            {
                gridUI.SetRecommendEquipID(recommendEquipData.id);
                gridUI.SetRecommendEquip(recommendEquipData.Icon, recommendEquipData.color, (int)recommendEquipData.quality);
                gridUI.SetRecommendEquipName(recommendEquipData.Name, true);

                recommendScore = recommendEquipData.GetScore(MogoWorld.thePlayer.level);
                gridUI.SetRecommendEquipScore(recommendScore);
            }
            else
            {
                gridUI.SetRecommendEquipID(0);
                gridUI.SetRecommendEquip(EquipSlotIcon.icons[gridData.equipSlot], 0);
                gridUI.SetRecommendEquipName(EquipSlotName.GetEquipSlotNameBySlot(gridData.equipSlot), false);
                gridUI.SetRecommendEquipScore(0);
            }

            gridUI.SetScoreUpgradeNum(recommendScore, currentScore);
        }      
    }

    /// <summary>
    /// 清除推荐装备Grid数据
    /// </summary>
    void ClearEquipRecommendGridList()
    {
        LoggerHelper.Debug(m_maplistEquipRecommendUIGrid.Count);
        for (int i = 0; i < m_maplistEquipRecommendUIGrid.Count; ++i)
        {
            if (m_maplistEquipRecommendUIGrid.ContainsKey(i))
                AssetCacheMgr.ReleaseInstance(m_maplistEquipRecommendUIGrid[i].gameObject);
        }

        m_maplistEquipRecommendUIGrid.Clear();
    }

    /// <summary>
    /// 设置箭头是否显示
    /// </summary>
    /// <param name="num"></param>
    void ShowEquipRecommendGridListArrow(int num)
    {
        m_goEquipRecommendUIArrowB.SetActive(num > GRID_COUNT_ONE_PAGE);
    }  

    void ResetCameraPos()
    {
        m_equipListCamera.transform.localPosition = m_equipListcameraInitPos;
    }

    #endregion

    #region 界面打开和关闭

    /// <summary>
    /// 展示界面信息
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();        

        // 统计可以推荐的装备列表数据
        if (EQUIPRECOMMENDUICALDATALIST != null)
            EQUIPRECOMMENDUICALDATALIST();
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyEquipRecommendUI();
        }        
    }

    #endregion
}
