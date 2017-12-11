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
using System;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;

public class DragonMatchRecordUIViewManager : MogoUIBehaviour 
{
    private static DragonMatchRecordUIViewManager m_instance;
    public static DragonMatchRecordUIViewManager Instance { get { return DragonMatchRecordUIViewManager.m_instance; } }

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量

    // 飞龙大赛记录列表
    private Transform m_tranUIGridlList;
    private Camera m_gridListCamera;
    private Vector3 m_camInitPos;
    private MyDragableCamera m_gridListMyDragableCamera;
    private GameObject m_goDragonMatchRecordUIArrow;

    private UILabel m_lblDragonMatchRecordUITime;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranUIGridlList = FindTransform("DragonMatchRecordUIGridList");
        m_gridListCamera = FindTransform("DragonMatchRecordUIGridListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_camInitPos = m_gridListCamera.transform.localPosition;
        m_gridListMyDragableCamera = m_gridListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_goDragonMatchRecordUIArrow = FindTransform("DragonMatchRecordUIArrow").gameObject;
        m_lblDragonMatchRecordUITime = FindTransform("DragonMatchRecordUITime").GetComponentsInChildren<UILabel>(true)[0];

        Initialize();
    }

    #region 事件

    public Action DRAGONMATCHRECORDUICLOSEUP;

    public void Initialize()
    {
        FindTransform("DragonMatchRecordUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCloseUp;

        DragonMatchRecordUILogicManager.Instance.Initialize();
        m_uiLoginManager = DragonMatchRecordUILogicManager.Instance;
    }

    public void Release()
    {
        DragonMatchRecordUILogicManager.Instance.Release();
    }

    void OnCloseUp()
    {
        if (DRAGONMATCHRECORDUICLOSEUP != null)
            DRAGONMATCHRECORDUICLOSEUP();
    }

    #endregion

    #region 界面信息

    /// <summary>
    /// 设置今日可复仇次数
    /// </summary>
    /// <param name="currentTimes"></param>
    /// <param name="maxTimes"></param>
    public void SetRevengeTimes(int currentTimes, int maxTimes)
    {
        string timeString = string.Concat(currentTimes, "/", maxTimes);
        m_lblDragonMatchRecordUITime.text = string.Format(LanguageData.GetContent(48200), timeString);
    }    

    #endregion

    #region 飞龙大赛记录Grid

    readonly static float ITEMSPACEVERTICAL = -85;
    readonly static float OFFSET_Y = 0;
    readonly static int GRID_COUNT_ONE_PAGE = 6;
    private List<DragonMatchRecordUIGrid> m_listGrid = new List<DragonMatchRecordUIGrid>();

    /// <summary>
    /// 设置记录Grid
    /// </summary>
    /// <param name="idList"></param>
    public void SetUIGridList(int num, Action action = null)
    {
        if (num == 0)
        {
            if (action != null)
                action();
        }

        AddUIGridList(num, () =>
        {
            if (action != null)
                action();
        });
    }

    /// <summary>
    /// 添加记录Grid
    /// </summary>
    /// <param name="num"></param>
    /// <param name="act"></param>
    void AddUIGridList(int num, Action act = null)
    {
        ClearUIGridList();
        ResetGridListCameraPos();
        ShowUIArrow(num);

        // 删除翻页位置
        m_gridListMyDragableCamera.DestroyMovePagePosList();
        m_gridListMyDragableCamera.FPageHeight = ITEMSPACEVERTICAL * GRID_COUNT_ONE_PAGE;

        if (num == 0)
        {
            if (act != null)
                act();

            return;
        }

        for (int i = 0; i < num; ++i)
        {
            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

            int index = i;
            AssetCacheMgr.GetUIInstance("DragonMatchRecordUIGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranUIGridlList;
                obj.transform.localPosition = new Vector3(0, ITEMSPACEVERTICAL * index, 0);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_gridListCamera;
                DragonMatchRecordUIGrid grid = obj.AddComponent<DragonMatchRecordUIGrid>();
                m_listGrid.Add(grid);

                m_gridListMyDragableCamera.MAXY = OFFSET_Y;
                if (m_listGrid.Count > GRID_COUNT_ONE_PAGE)
                    m_gridListMyDragableCamera.MINY = (m_listGrid.Count - GRID_COUNT_ONE_PAGE) * ITEMSPACEVERTICAL + OFFSET_Y;
                else
                    m_gridListMyDragableCamera.MINY = m_gridListMyDragableCamera.MAXY;

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                // 创建翻页位置
                if (index % GRID_COUNT_ONE_PAGE == 0)
                {
                    GameObject trans = new GameObject();
                    trans.transform.parent = m_gridListCamera.transform;
                    trans.transform.localPosition = new Vector3(0, index / GRID_COUNT_ONE_PAGE * ITEMSPACEVERTICAL * GRID_COUNT_ONE_PAGE + OFFSET_Y, 0);
                    trans.transform.localEulerAngles = Vector3.zero;
                    trans.transform.localScale = new Vector3(1, 1, 1);
                    trans.name = "GridListPosHorizon" + index / GRID_COUNT_ONE_PAGE;
                    m_gridListMyDragableCamera.transformList.Add(trans.transform);
                }

                if (index == num - 1)
                {
                    if (act != null)
                    {
                        act();
                    }
                }
            });
        }
    }  

    /// <summary>
    /// 清除记录Grid
    /// </summary>
    void ClearUIGridList()
    {
        Mogo.Util.LoggerHelper.Debug("ClearDragonMatchRecordUIGridList " + m_listGrid.Count);
        for (int i = 0; i < m_listGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listGrid[i].gameObject);
        }

        m_listGrid.Clear();
    }

    /// <summary>
    /// 设置箭头是否显示
    /// </summary>
    /// <param name="num"></param>
    void ShowUIArrow(int num)
    {
        if (num > GRID_COUNT_ONE_PAGE)
            m_goDragonMatchRecordUIArrow.SetActive(true);
        else
            m_goDragonMatchRecordUIArrow.SetActive(false);
    }

    /// <summary>
    /// 重置列表Camera位置
    /// </summary>
    void ResetGridListCameraPos()
    {
        m_gridListCamera.transform.localPosition = m_camInitPos;
    }

    /// <summary>
    /// 设置比赛记录数据
    /// </summary>
    public void SetGridListData(List<DragonMatchRecordGridData> listRecordGridData)
    {
        for (int index = 0; index < listRecordGridData.Count; index++)
        {
            if (index < m_listGrid.Count)
            {
                DragonMatchRecordUIGrid gridUI = m_listGrid[index];
                DragonMatchRecordGridData gridData = listRecordGridData[index];

                gridUI.LoadResourceInsteadOfAwake();
                gridUI.Index = index;
                gridUI.HasRevenged = gridData.hasRevenged;
                gridUI.CanRevenge = gridData.canRevenge;
                gridUI.NeedTip = gridData.needTip;
                gridUI.InfoString = gridData.info;
            }
        }
    }

    #endregion

    #region 界面打开和关闭

    /// <summary>
    /// 展示界面信息
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();        
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyDragonMatchRecordUI();
        }        
    }

    #endregion  
}
