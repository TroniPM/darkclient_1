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

public class InstanceBossTreasureUIViewManager : MogoUIBehaviour 
{
    private static InstanceBossTreasureUIViewManager m_instance;
    public static InstanceBossTreasureUIViewManager Instance { get { return InstanceBossTreasureUIViewManager.m_instance; } }

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量

    // BOSS宝箱
    private Transform m_tranInstanceBossTreasureUIGridlList;
    private Camera m_gridListCamera;
    private Vector3 m_camRewardItemListInitPos;
    private MyDragableCamera m_gridListMyDragableCamera;

    // BOSS宝箱特效
    private Camera m_gridListCameraFX;
    private Transform m_tranInstanceBossTreasureUIGridListFX;

    // BOSS奖励UI
    private GameObject m_goInstanceBossTreasureUIRewardUI;
    private List<GameObject> m_listInstanceBossTreasureUIRewardUIInfo = new List<GameObject>();
    private List<UILabel> m_listInstanceBossTreasureUIRewardUIInfoText = new List<UILabel>();

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranInstanceBossTreasureUIGridlList = m_myTransform.Find(m_widgetToFullName["InstanceBossTreasureUIGridList"]);
        m_gridListCamera = m_myTransform.Find(m_widgetToFullName["InstanceBossTreasureUIGridListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camRewardItemListInitPos = m_gridListCamera.transform.localPosition;

        m_gridListMyDragableCamera = m_gridListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_gridListMyDragableCamera.LeftArrow = FindTransform("InstanceBossTreasureUIArrowL").gameObject;
        m_gridListMyDragableCamera.RightArrow = FindTransform("InstanceBossTreasureUIArrowR").gameObject;

        FindTransform("InstanceBossTreasureUIBtnOK").gameObject.AddComponent<InstanceBossTreasureUIButton>();

        m_gridListCameraFX = FindTransform("InstanceBossTreasureUIGridListCameraFX").GetComponentsInChildren<Camera>(true)[0];
        m_tranInstanceBossTreasureUIGridListFX = FindTransform("InstanceBossTreasureUIGridListFX");

        // Boss宝箱奖励UI
        m_goInstanceBossTreasureUIRewardUI = FindTransform("InstanceBossTreasureUIRewardUI").gameObject;
        m_goInstanceBossTreasureUIRewardUI.SetActive(false);
        m_listInstanceBossTreasureUIRewardUIInfo.Clear();
        m_listInstanceBossTreasureUIRewardUIInfoText.Clear();
        for (int i = 1; i <= MAX_REWARD_COUNT; i++)
        {
            GameObject goReward = FindTransform("InstanceBossTreasureUIRewardUIInfo" + i).gameObject;
            m_listInstanceBossTreasureUIRewardUIInfo.Add(goReward);
            m_listInstanceBossTreasureUIRewardUIInfoText.Add(goReward.GetComponentsInChildren<UILabel>(true)[0]);
        }

        // ChineseData
        FindTransform("InstanceBossTreasureUITitle").GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.GetContent(46982);
        FindTransform("InstanceBossTreasureUITip").GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.GetContent(46983);

        Initialize();
    }

    #region 事件
    public Action BOSSTREASUREUICLOSEUP;

    public void Initialize()
    {
        InstanceBossTreasureUIDict.ButtonTypeToEventUp.Add("InstanceBossTreasureUIBtnOK", OnBossTreasureCloseUp);       

        InstanceBossTreasureUILogicManager.Instance.Initialize();
        m_uiLoginManager = InstanceBossTreasureUILogicManager.Instance;
    }

    public void Release()
    {
        InstanceBossTreasureUILogicManager.Instance.Release();
        MogoUIManager.Instance.IsEnergyUILoaded = false;
        InstanceBossTreasureUIDict.ButtonTypeToEventUp.Clear();
    }

    void OnBossTreasureCloseUp(int i)
    {
        if (BOSSTREASUREUICLOSEUP != null)
            BOSSTREASUREUICLOSEUP();
    }

    #endregion

    #region BOSS宝箱

    readonly static float ITEMSPACEHORIZON = 162;
    readonly static int BOSSTREASURE_COUNT_ONE_PAGE = 4;
    private Dictionary<int, InstanceBossTreasureUIGrid> m_mapBossTreasure = new Dictionary<int, InstanceBossTreasureUIGrid>();

    /// <summary>
    /// 设置Boss宝箱奖励
    /// </summary>
    /// <param name="idList"></param>
    public void SetBossTreasureGridList(int num, Action action = null)
    {
        if (m_mapBossTreasure.Count == num)
        {
            if (action != null)
                action();

            return;
        }

        AddBossTreasureGridList(num, () =>
        {
            if (action != null)
                action();

            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
        });
    }

    /// <summary>
    /// 添加Boss宝箱奖励
    /// </summary>
    /// <param name="num"></param>
    /// <param name="act"></param>
    void AddBossTreasureGridList(int num, Action act = null)
    {
        ClearBossTreasureGridList();
        ResetGridListCameraPos();

        // 删除翻页位置
        if (m_gridListMyDragableCamera.transformList != null)
        {
            for (int i = 0; i < m_gridListMyDragableCamera.transformList.Count; ++i)
            {
                Destroy(m_gridListMyDragableCamera.transformList[i].gameObject);
            }

            m_gridListMyDragableCamera.transformList.Clear();
        }

        for (int i = 0; i < num; ++i)
        {
            int index = i;

            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);            

            AssetCacheMgr.GetUIInstance("InstanceBossTreasureUIGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranInstanceBossTreasureUIGridlList;
                obj.transform.localPosition = new Vector3(ITEMSPACEHORIZON * index, 0, 0);
                obj.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_gridListCamera;
                InstanceBossTreasureUIGrid gridUI = obj.AddComponent<InstanceBossTreasureUIGrid>();
                m_mapBossTreasure[index] = gridUI;
                 
                if (m_mapBossTreasure.Count >= BOSSTREASURE_COUNT_ONE_PAGE)
                {
                    m_gridListMyDragableCamera.MAXX =
                        (m_mapBossTreasure.Count - BOSSTREASURE_COUNT_ONE_PAGE) * ITEMSPACEHORIZON;
                }                  

                // 创建翻页位置
                if (index % BOSSTREASURE_COUNT_ONE_PAGE == 0)
                {
                    GameObject trans = new GameObject();
                    trans.transform.parent = m_gridListCamera.transform;
                    trans.transform.localPosition = new Vector3(index / BOSSTREASURE_COUNT_ONE_PAGE * ITEMSPACEHORIZON * BOSSTREASURE_COUNT_ONE_PAGE, 0, 0);
                    trans.transform.localEulerAngles = Vector3.zero;
                    trans.transform.localScale = new Vector3(1, 1, 1);
                    trans.name = "GridListPosHorizon" + index / BOSSTREASURE_COUNT_ONE_PAGE;
                    m_gridListMyDragableCamera.transformList.Add(trans.transform);
                    m_gridListMyDragableCamera.SetCurrentPage(m_gridListMyDragableCamera.GetCurrentPage());
                }

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);

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
    /// 设置Boss宝箱信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="imgName"></param>
    /// <param name="itemName"></param>
    public void SetBossTreasureGridListData(List<BossTreasureGridData> listBossTreasureGridData)
    {
        for (int index = 0; index < listBossTreasureGridData.Count; index++)
        {
            if (m_mapBossTreasure.ContainsKey(index))
            {
                InstanceBossTreasureUIGrid gridUI = m_mapBossTreasure[index];
                BossTreasureGridData gridData = listBossTreasureGridData[index];
                gridUI.LoadResourceInsteadOfAwake();
                gridUI.SetBossInfo(gridData.id, gridData.iconName, gridData.bossName, gridData.status, gridData.level);
            }                     
        }      
    }

    /// <summary>
    /// 清除Boss宝箱
    /// </summary>
    void ClearBossTreasureGridList()
    {
        Mogo.Util.LoggerHelper.Debug("ClearBossTreasureGridList " + m_mapBossTreasure.Count);
        for (int i = 0; i < m_mapBossTreasure.Count; ++i)
        {
            if (m_mapBossTreasure.ContainsKey(i))
                AssetCacheMgr.ReleaseInstance(m_mapBossTreasure[i].gameObject);
        }

        m_mapBossTreasure.Clear();
    }

    /// <summary>
    /// 重置列表Camera位置
    /// </summary>
    void ResetGridListCameraPos()
    {
        m_gridListCamera.transform.localPosition = m_camRewardItemListInitPos;
    }

    #endregion

    #region BOSS宝箱特效
    
    public void AttachFXToBossTreasureUI(Transform tranBossTreasure, Action<GameObject> action)
    {
        Vector3 pos = m_gridListCamera.WorldToScreenPoint(tranBossTreasure.position);
        pos = m_gridListCameraFX.ScreenToWorldPoint(pos);

        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);        

        AssetCacheMgr.GetUIInstance("fx_ui_baoxiangxingxing.prefab", (prefab, id, go) =>
        {
            GameObject goFinishedFx = null;
            goFinishedFx = (GameObject)go;
            goFinishedFx.name = "BossTreasureFinishedFx";
            goFinishedFx.transform.parent = m_tranInstanceBossTreasureUIGridListFX;
            goFinishedFx.transform.position = pos + new Vector3(0, 0, 0);
            goFinishedFx.transform.localScale = new Vector3(1f, 1f, 1f);

            INSTANCE_COUNT--;
            if (INSTANCE_COUNT <= 0)
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);

            if (action != null)
                action(goFinishedFx);
        });
    }

    #endregion

    #region BOSS宝箱奖励UI

    readonly static int MAX_REWARD_COUNT = 4;

    /// <summary>
    /// 是否显示宝箱奖励UI
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowInstanceBossTreasureUIRewardUI(bool isShow)
    {
        if (m_goInstanceBossTreasureUIRewardUI != null)
            m_goInstanceBossTreasureUIRewardUI.SetActive(isShow);
    }

    /// <summary>
    /// 设置宝箱奖励
    /// </summary>
    /// <param name="list"></param>
    public void SetInstanceBossTreasureUIRewardList(List<string> list)
    {
        if (m_listInstanceBossTreasureUIRewardUIInfo.Count != MAX_REWARD_COUNT
            || m_listInstanceBossTreasureUIRewardUIInfoText.Count != MAX_REWARD_COUNT)
        {
            LoggerHelper.Error("m_listInstanceBossTreasureUIRewardUIInfo no equals MAX_REWARD_COUNT");
            return;
        }

        for (int i = 0; i < MAX_REWARD_COUNT; i++)
        {
            if (i < list.Count)
            {
                m_listInstanceBossTreasureUIRewardUIInfo[i].SetActive(true);
                m_listInstanceBossTreasureUIRewardUIInfoText[i].text = list[i];
            }
            else
            {
                m_listInstanceBossTreasureUIRewardUIInfo[i].SetActive(false);
                m_listInstanceBossTreasureUIRewardUIInfoText[i].text = "";
            }
        }
    }

    #endregion

    #region 界面打开和关闭

    /// <summary>
    /// 打开界面
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        ShowInstanceBossTreasureUIRewardUI(false);
        EventDispatcher.TriggerEvent(Events.InstanceUIEvent.UpdateBossChestMessage);
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyInstanceBossTreasureUI();
        }
        else
        {
            ShowInstanceBossTreasureUIRewardUI(false);
        }
    }

    #endregion   
}
