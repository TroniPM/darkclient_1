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
using Mogo.GameData;
using Mogo.Util;

public class LevelNoEnoughUIViewManager : MogoUIBehaviour
{
    private static LevelNoEnoughUIViewManager m_instance;
    public static LevelNoEnoughUIViewManager Instance { get { return LevelNoEnoughUIViewManager.m_instance; } }

    // 等级不足活动项
    private Transform m_tranUIGridlList;
    private Camera m_gridListCamera;
    private Vector3 m_camInitPos;
    private MyDragableCamera m_dragableCamera;

    private GameObject m_goLevelNoEnoughUIProgressBar;
    private UILabel m_lblLevelNoEnoughUIProgressBarCurLevel;
    private UILabel m_lblLevelNoEnoughUIProgressBarNextLevel;
    private UILabel m_lblLevelNoEnoughUITipText;

    void Awake()
    {
        m_instance = gameObject.GetComponent<LevelNoEnoughUIViewManager>();
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranUIGridlList = FindTransform("LevelNoEnoughUIGridList");
        m_gridListCamera = FindTransform("LevelNoEnoughUIGridListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_camInitPos = m_gridListCamera.transform.localPosition;

        m_dragableCamera = m_gridListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragableCamera.LeftArrow = FindTransform("LevelNoEnoughUIArrowUp").gameObject;
        m_dragableCamera.RightArrow = FindTransform("LevelNoEnoughUIArrowDown").gameObject;

        m_goLevelNoEnoughUIProgressBar = FindTransform("LevelNoEnoughUIProgressBar").gameObject;
        m_lblLevelNoEnoughUIProgressBarCurLevel = FindTransform("LevelNoEnoughUIProgressBarCurLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblLevelNoEnoughUIProgressBarNextLevel = FindTransform("LevelNoEnoughUIProgressBarNextLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblLevelNoEnoughUITipText = FindTransform("LevelNoEnoughUITipText").GetComponentsInChildren<UILabel>(true)[0];

        FindTransform("LevelNoEnoughUIBtnClose").gameObject.AddComponent<LevelNoEnoughUIButton>();

        Initialize();    
    }

    #region 事件
    public Action LEVELNOENOUGHUICLOSEUP;

    public void Initialize()
    {
        LevelNoEnoughUIDict.ButtonTypeToEventUp.Add("LevelNoEnoughUIBtnClose", OnCloseUp);

        LevelNoEnoughUILogicManager.Instance.Initialize();
        m_uiLoginManager = LevelNoEnoughUILogicManager.Instance;
    }

    public void Release()
    {
        LevelNoEnoughUILogicManager.Instance.Release();
        LevelNoEnoughUIDict.ButtonTypeToEventUp.Clear();
    }

    void OnCloseUp(int i)
    {
        if (LEVELNOENOUGHUICLOSEUP != null)
            LEVELNOENOUGHUICLOSEUP();
    }
  
    #endregion   

    #region 等级不足活动项

    readonly static float ITEMSPACEVERTICAL = -125;
    readonly static int GRID_COUNT_ONE_PAGE = 2;
    readonly static float OFFSET_Y = 0.0f;
    private Dictionary<int, LevelNoEnoughUIGrid> m_mapGrid = new Dictionary<int, LevelNoEnoughUIGrid>();

    /// <summary>
    /// 设置等级不足活动Grid
    /// </summary>
    /// <param name="idList"></param>
    public void SetUIGridList(int num, Action action = null)
    {
        AddUIGridList(num, () =>
        {
            if (action != null)
                action();

            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
        });
    }

    /// <summary>
    /// 添加等级不足活动Grid
    /// </summary>
    /// <param name="num"></param>
    /// <param name="act"></param>
    void AddUIGridList(int num, Action act = null)
    {
        ClearUIGridList();
        ResetGridListCameraPos();

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
            AssetCacheMgr.GetUIInstance("LevelNoEnoughUIGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranUIGridlList;
                obj.transform.localPosition = new Vector3(0, ITEMSPACEVERTICAL * index, 0);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_gridListCamera;
                LevelNoEnoughUIGrid gridUI = obj.AddComponent<LevelNoEnoughUIGrid>();

                if (m_mapGrid.ContainsKey(index))
                    AssetCacheMgr.ReleaseInstance(m_mapGrid[index].gameObject);
                m_mapGrid[index] = gridUI;                         
        
                // 创建翻页位置(滑动形式不需要处理)
                if (m_dragableCamera.IsMovePage)
                {
                    if (index % GRID_COUNT_ONE_PAGE == 0)
                    {
                        GameObject trans = new GameObject();
                        trans.transform.parent = m_gridListCamera.transform;
                        trans.transform.localPosition = new Vector3(0, index / GRID_COUNT_ONE_PAGE * ITEMSPACEVERTICAL * GRID_COUNT_ONE_PAGE, 0);
                        trans.transform.localEulerAngles = Vector3.zero;
                        trans.transform.localScale = new Vector3(1, 1, 1);
                        trans.name = "GridListPosHorizon" + index / GRID_COUNT_ONE_PAGE;
                        m_dragableCamera.transformList.Add(trans.transform);
                        m_dragableCamera.SetArrow();
                    }
                }             

                if (index == num - 1)
                {
                    // 滑动形式和翻页形式都需要设置
                    //if (!m_dragableCamera.IsMovePage)
                    {
                        m_dragableCamera.FPageHeight = ITEMSPACEVERTICAL * GRID_COUNT_ONE_PAGE;
                        m_dragableCamera.MAXY = OFFSET_Y;
                        if (m_mapGrid.Count > GRID_COUNT_ONE_PAGE)
                            m_dragableCamera.MINY = (m_mapGrid.Count - GRID_COUNT_ONE_PAGE) * ITEMSPACEVERTICAL + OFFSET_Y;
                        else
                            m_dragableCamera.MINY = m_dragableCamera.MAXY;
                        m_dragableCamera.SetArrow();
                    }                 

                    if (act != null)
                    {
                        act();
                    }
                }
            });
        }
    }

    /// <summary>
    /// 设置等级不足活动数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="imgName"></param>
    /// <param name="itemName"></param>
    public void SetGridListData(List<UpgradeLevelGuideData> listUpgradeLevelGuideData)
    {
        for (int index = 0; index < listUpgradeLevelGuideData.Count; index++)
        {
            if (m_mapGrid.ContainsKey(index))
            {
                UpgradeLevelGuideData gridData = listUpgradeLevelGuideData[index];
                LevelNoEnoughUIGrid gridUI = m_mapGrid[index];                
                gridUI.LoadResourceInsteadOfAwake();      
                gridUI.Index = index;
                gridUI.SetLevelUpgradeGuideDetail(gridData.title, gridData.icon, gridData.buttonName, gridData.desc, gridData.requestLevel);
            }
        }
    }

    /// <summary>
    /// 清除等级不足活动项
    /// </summary>
    void ClearUIGridList()
    {
        Mogo.Util.LoggerHelper.Debug("ClearLevelNoEnoughUIGridList " + m_mapGrid.Count);
        for (int i = 0; i < m_mapGrid.Count; ++i)
        {
            if (m_mapGrid.ContainsKey(i))
                AssetCacheMgr.ReleaseInstance(m_mapGrid[i].gameObject);
        }

        m_mapGrid.Clear();
    }
 
    /// <summary>
    /// 重置列表Camera位置
    /// </summary>
    void ResetGridListCameraPos()
    {
        m_gridListCamera.transform.localPosition = m_camInitPos;
    }

    #endregion

    public void SetLevel(byte currentLevel)
    {
        m_lblLevelNoEnoughUIProgressBarCurLevel.text = string.Concat("LV", currentLevel);
        m_lblLevelNoEnoughUIProgressBarNextLevel.text = string.Concat("LV", currentLevel + 1);
    }

    public void SetExp(float expPercentage)
    {
        uint needExp = MogoWorld.thePlayer.nextLevelExp - MogoWorld.thePlayer.exp;
        m_lblLevelNoEnoughUITipText.text = string.Format(LanguageData.GetContent(47901), needExp);
        SetProgressBar((float)MogoWorld.thePlayer.exp / MogoWorld.thePlayer.nextLevelExp);
    }

    /// <summary>
    /// 设置进度条
    /// </summary>
    /// <param name="value"></param>
    private void SetProgressBar(float value)
    {
        m_goLevelNoEnoughUIProgressBar.GetComponent<UISlider>().sliderValue = value;
    }

    #region 界面打开和关闭

    /// <summary>
    /// 展示界面信息
    /// </summary>
    /// <param name="IsShow"></param>
    protected override void OnEnable()
    {
        base.OnEnable();
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        LevelNoEnoughUILogicManager.Instance.RpcGetArenaExpReq();        
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyLevelNoEnoughUI();
        }        
    }

    #endregion 
}
