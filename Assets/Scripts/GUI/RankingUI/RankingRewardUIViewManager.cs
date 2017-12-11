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
using Mogo.Util;
using System.Collections.Generic;
using Mogo.GameData;

public class RankingRewardUIViewManager : MogoUIBehaviour 
{
    private static RankingRewardUIViewManager m_instance;
    public static RankingRewardUIViewManager Instance { get { return RankingRewardUIViewManager.m_instance; } }

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量

    // 排行榜奖励
    private Transform m_tranUIGridlList;
    private Camera m_gridListCamera;
    private Vector3 m_camInitPos;
    private MyDragableCamera m_gridListMyDragableCamera;
    private GameObject m_goRankingRewardUIArrow;

    private UILabel m_lblRankingRewardUITitle;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranUIGridlList = FindTransform("RankingRewardUIGridList");
        m_gridListCamera = FindTransform("RankingRewardUIGridListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_camInitPos = m_gridListCamera.transform.localPosition;
        m_gridListMyDragableCamera = m_gridListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_goRankingRewardUIArrow = FindTransform("RankingRewardUIArrow").gameObject;

        m_lblRankingRewardUITitle = FindTransform("RankingRewardUITitle").GetComponentsInChildren<UILabel>(true)[0];

        Initialize();
    }

    #region 事件

    public Action RANKINGREWARDUICLOSEUP;

    public void Initialize()
    {
        FindTransform("RankingRewardUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCloseUp;

        RankingRewardUILogicManager.Instance.Initialize();
        m_uiLoginManager = RankingRewardUILogicManager.Instance;
    }

    public void Release()
    {
        RankingRewardUILogicManager.Instance.Release();
    }

    void OnCloseUp()
    {
        if (RANKINGREWARDUICLOSEUP != null)
            RANKINGREWARDUICLOSEUP();
    }

    #endregion

    #region 界面信息

    /// <summary>
    /// 设置标题
    /// </summary>
    /// <param name="title"></param>
    public void SetRankingRewardUITitle(string title)
    {
        m_lblRankingRewardUITitle.text = title;
    }

    #endregion

    #region 排名奖励Grid

    readonly static float ITEMSPACEVERTICAL = -85;
    readonly static float OFFSET_Y = -200;
    readonly static int GRID_COUNT_ONE_PAGE = 6;
    private Dictionary<int, RankingRewardUIGrid> m_mapGrid = new Dictionary<int, RankingRewardUIGrid>();

    /// <summary>
    /// 设置奖励列表
    /// </summary>
    /// <param name="idList"></param>
    public void SetUIGridList(int num, Action action = null)
    {       
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
        //m_gridListMyDragableCamera.DestroyMovePagePosList();
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
            AssetCacheMgr.GetUIInstance("RankingRewardUIGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranUIGridlList;
                obj.transform.localPosition = new Vector3(0, ITEMSPACEVERTICAL * index, 0);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_gridListCamera;
                RankingRewardUIGrid gridUI = obj.AddComponent<RankingRewardUIGrid>();

                if(m_mapGrid.ContainsKey(index))
                    AssetCacheMgr.ReleaseInstance(m_mapGrid[index].gameObject);
                m_mapGrid[index] = gridUI;

                m_gridListMyDragableCamera.MAXY = OFFSET_Y;
                if (m_mapGrid.Count > GRID_COUNT_ONE_PAGE)
                    m_gridListMyDragableCamera.MINY = (m_mapGrid.Count - GRID_COUNT_ONE_PAGE) * ITEMSPACEVERTICAL + OFFSET_Y;
                else
                    m_gridListMyDragableCamera.MINY = m_gridListMyDragableCamera.MAXY;

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                // 创建翻页位置
                //if (index % GRID_COUNT_ONE_PAGE == 0)
                //{
                //    GameObject trans = new GameObject();
                //    trans.transform.parent = m_gridListCamera.transform;
                //    trans.transform.localPosition = new Vector3(0, index / GRID_COUNT_ONE_PAGE * ITEMSPACEVERTICAL * GRID_COUNT_ONE_PAGE + OFFSET_Y, 0);
                //    trans.transform.localEulerAngles = Vector3.zero;
                //    trans.transform.localScale = new Vector3(1, 1, 1);
                //    trans.name = "GridListPosHorizon" + index / GRID_COUNT_ONE_PAGE;
                //    m_gridListMyDragableCamera.transformList.Add(trans.transform);
                //}

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
        Mogo.Util.LoggerHelper.Debug("ClearDragonMatchRecordUIGridList " + m_mapGrid.Count);
        for (int i = 0; i < m_mapGrid.Count; ++i)
        {
            if (m_mapGrid.ContainsKey(i))
                AssetCacheMgr.ReleaseInstance(m_mapGrid[i].gameObject);
        }

        m_mapGrid.Clear();
    }

    /// <summary>
    /// 设置箭头是否显示
    /// </summary>
    /// <param name="num"></param>
    void ShowUIArrow(int num)
    {
        if (num > GRID_COUNT_ONE_PAGE)
            m_goRankingRewardUIArrow.SetActive(true);
        else
            m_goRankingRewardUIArrow.SetActive(false);
    }

    /// <summary>
    /// 重置列表Camera位置
    /// </summary>
    void ResetGridListCameraPos()
    {
        m_gridListCamera.transform.localPosition = m_camInitPos;
    }

    /// <summary>
    /// 设置圣域守卫战奖励数据
    /// </summary>
    public void SetSanctuaryGridListData(List<int> listSancturyID)
    {
        for (int index = 0; index < listSancturyID.Count; index++)
        {
            if (m_mapGrid.ContainsKey(index))
            {
                RankingRewardUIGrid gridUI = m_mapGrid[index];
                gridUI.LoadResourceInsteadOfAwake();

                string title = string.Format(LanguageData.GetContent(46908), index + 1);
                List<string> tips = SanctuaryRewardXMLData.GetWeekRewardNameList(index);

                string info = "";
                for (int i = 0; i < tips.Count; i++)
                {
                    if (i == 0)
                        info += tips[i];
                    else
                        info += string.Concat(LanguageData.GetContent(46909), tips[i]);
                }

                gridUI.Index = index;
                gridUI.SetRankingReward(title, info);                
            }
        }
    }

    #endregion

    #region 界面打开和关闭

    /// <summary>
    /// 展示界面信息
    /// </summary>
    /// <param name="IsShow"></param>
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
            MogoUIManager.Instance.DestroyRankingRewardUI();
        }
    }

    #endregion
}
