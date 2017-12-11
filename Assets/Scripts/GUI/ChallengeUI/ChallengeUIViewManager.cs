using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using System;



public class ChallengeUIViewManager : MogoUIBehaviour
{
    private static ChallengeUIViewManager instance;
    public static ChallengeUIViewManager Instance { get { return ChallengeUIViewManager.instance; } }

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量

    public readonly static int GRIDNUM = 6;
    private string[] gridImgName = new string[GRIDNUM];
    protected List<ChallengeUIGrid> m_listChallengeUIGrid = new List<ChallengeUIGrid>();

    UISprite m_spRefreshCtrl; 

    private Transform m_tranChallengeGridList;
    private Camera m_gridListCamera;
    private MyDragableCamera m_gridListMyDragableCamera;
    private GameObject m_goChallengeUIPageDOTList;

    void Awake()
    {
        instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);        

        m_spRefreshCtrl = FindTransform("ChallengeUIRefreshCtrl").GetComponentsInChildren<UISprite>(true)[0];

        m_tranChallengeGridList = FindTransform("ChallengeGridList");
        m_gridListCamera = FindTransform("ChallengeGridListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_gridListCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("Camera").GetComponentInChildren<Camera>();
        m_gridListMyDragableCamera = m_gridListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_gridListMyDragableCamera.LeftArrow = FindTransform("ChallengeUIArrowL").gameObject;
        m_gridListMyDragableCamera.RightArrow = FindTransform("ChallengeUIArrowR").gameObject;
        m_goChallengeUIPageDOTList = FindTransform("ChallengeUIPageDOTList").gameObject;

        gridImgName[(int)ChallengeGridID.ClimbTower] = "tz-slzt";
        gridImgName[(int)ChallengeGridID.DoorOfBury] = "tz-jmzm";
        gridImgName[(int)ChallengeGridID.Sanctuary] = "tz-tfjl";
        //gridImgName[(int)ChallengeGridID.DragonMatch] = "tz-tfjl";
        //gridImgName[(int)ChallengeGridID.OgreMustDie] = "tz-tfjl";
        gridImgName[(int)ChallengeGridID.DragonMatch] = "tz-flds";
        gridImgName[(int)ChallengeGridID.OgreMustDie] = "tz-zjns";
        gridImgName[(int)ChallengeGridID.OccupyTower] = "tz-3v3"; 

        Initialize();
    }    

    #region 事件

    public Action CLOSEUP;
    public Action<int> ENTERUP;

    void Initialize()
    {
        ChallengeUILogicManager.Instance.Initialize();

        ChallengeUIDict.ButtonTypeToEventUp.Add("ChallengeCloseButton", OnCloseUp);
        EventDispatcher.AddEventListener<int>(Events.ChallengeUIEvent.Enter, OnEnterUp);
    }

    public void Release()
    {
        ChallengeUILogicManager.Instance.Release();

        EventDispatcher.RemoveEventListener<int>(Events.ChallengeUIEvent.Enter, OnEnterUp);
        ChallengeUIDict.ButtonTypeToEventUp.Clear();
    }

    void OnEnterUp(int id)
    {
        if (ENTERUP != null)
        {
            ENTERUP(id);
        }
    }

    void OnCloseUp(int i)
    {
        if (CLOSEUP != null)
            CLOSEUP();
    }

    #endregion

    #region 创建Grid

    readonly static private int ITEMSPACE = 350;
    readonly static private int OFFSET_X = -355;
    readonly static private int GRID_COUNT_ONE_PAGE = 3;

    private int m_iDotPageNum = 0;

    public void SetGridLayout(Action callback)
    {
        if (m_listChallengeUIGrid.Count == GRIDNUM)
        {
            if (callback != null)
            {
                callback();                
            }
            EventDispatcher.TriggerEvent(DoorOfBurySystem.ON_CHALLENGE_SHOW);
            EventDispatcher.TriggerEvent(Events.CampaignEvent.GetCampaignLastTime, 1);
            EventDispatcher.TriggerEvent(Events.OccupyTowerEvent.GetOccupyTowerStatePoint);
            ChallengeUILogicManager.Instance.RefreshUI((int)ChallengeGridID.OgreMustDie);
            ChallengeUILogicManager.Instance.CollectChallengeUIGridMessage();
        }
        else
        {
            m_gridListMyDragableCamera.DestroyMovePagePosList(); // 删除翻页位置
            m_gridListMyDragableCamera.DestroyDOTPageList(); // 删除页点

            for (int i = 0; i < GRIDNUM; i++)
            {
                int index = i;

                INSTANCE_COUNT++;
                MogoGlobleUIManager.Instance.ShowWaitingTip(true);

                AssetCacheMgr.GetUIInstance("ChallengeGrid.prefab", (prefab, guid, go) =>
                {
                    GameObject temp = (GameObject)go;
                    temp.AddComponent<ChallengeUIGrid>();
                    temp.transform.parent = m_tranChallengeGridList;
                    temp.transform.localPosition = new Vector3(index * ITEMSPACE + OFFSET_X, 0, 0);
                    temp.transform.localScale = new Vector3(1, 1, 1);
                    temp.name = string.Concat("ChallengeGrid", index);
                    temp.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_gridListCamera;

                    ChallengeUIGrid gridUI = temp.GetComponentInChildren<ChallengeUIGrid>();
                    gridUI.id = index + 1;
                    if (index < GRIDNUM)
                    {
                        gridUI.SetImg(gridImgName[index]);
                    }

                    m_listChallengeUIGrid.Add(gridUI);

                    m_gridListMyDragableCamera.MINX = OFFSET_X;
                    m_gridListMyDragableCamera.MAXX = (m_listChallengeUIGrid.Count - GRID_COUNT_ONE_PAGE) * ITEMSPACE;

                    // 创建翻页位置
                    if (index % GRID_COUNT_ONE_PAGE == 0)
                    {
                        GameObject trans = new GameObject();
                        trans.transform.parent = m_gridListCamera.transform;
                        trans.transform.localPosition = new Vector3(index / GRID_COUNT_ONE_PAGE * ITEMSPACE * GRID_COUNT_ONE_PAGE, 0, 0);
                        trans.transform.localEulerAngles = Vector3.zero;
                        trans.transform.localScale = new Vector3(1, 1, 1);
                        trans.name = "GridListPosHorizon" + index / GRID_COUNT_ONE_PAGE;
                        m_gridListMyDragableCamera.transformList.Add(trans.transform);
                        m_gridListMyDragableCamera.SetCurrentPage(m_gridListMyDragableCamera.GetCurrentPage());

                        // 创建页数点
                        ++m_iDotPageNum;
                        int num = m_iDotPageNum;
                        AssetCacheMgr.GetUIInstance("ChooseServerPage.prefab", (prefabPage, idPage, goPage) =>
                        {
                            GameObject objPage = (GameObject)goPage;

                            objPage.transform.parent = m_goChallengeUIPageDOTList.transform;
                            objPage.transform.localPosition = new Vector3((num - 1) * 40, 0, 0);
                            objPage.transform.localScale = Vector3.one;
                            objPage.name = "ActivityGridPage" + num;
                            m_gridListMyDragableCamera.ListPageDown.Add(objPage.GetComponentsInChildren<UISprite>(true)[1].gameObject);
                            m_goChallengeUIPageDOTList.transform.localPosition = new Vector3(-20 * (num - 1), m_goChallengeUIPageDOTList.transform.localPosition.y, 0);

                            // 选择当前页
                            if (num - 1 == m_gridListMyDragableCamera.GetCurrentPage())
                                objPage.GetComponentsInChildren<UISprite>(true)[1].gameObject.SetActive(true);
                            else
                                objPage.GetComponentsInChildren<UISprite>(true)[1].gameObject.SetActive(false);
                            m_gridListMyDragableCamera.GODOTPageList = m_goChallengeUIPageDOTList;
                            m_gridListMyDragableCamera.SetCurrentPage(m_gridListMyDragableCamera.GetCurrentPage());
                        });
                    }

                    INSTANCE_COUNT--;
                    if (INSTANCE_COUNT <= 0)
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                    if (m_listChallengeUIGrid.Count == GRIDNUM)
                    {
                        if (callback != null)
                        {
                            callback();                            
                        }

                        EventDispatcher.TriggerEvent(DoorOfBurySystem.ON_CHALLENGE_SHOW);
                        ChallengeUILogicManager.Instance.InitializeData();

                        EventDispatcher.TriggerEvent(Events.CampaignEvent.GetCampaignLastTime, 1);
                        EventDispatcher.TriggerEvent(Events.OccupyTowerEvent.GetOccupyTowerStatePoint);

                        ChallengeUILogicManager.Instance.RefreshUI((int)ChallengeGridID.OgreMustDie);
                        ChallengeUILogicManager.Instance.CollectChallengeUIGridMessage();
                    }
                });
            }
        }
    }
  
    #endregion

    #region 其他

    public void BeginCountdown(string countingStr, string stopStr, string endStr, int gridID, int hour, int timesLeft, Action action = null)
    {
        m_listChallengeUIGrid[gridID].BeginCountDown(countingStr, endStr, hour, timesLeft);
    }

    public void BeginCountdown(string countingStr, string stopStr, string endStr, int gridID, int hour, int minute, int timesLeft, Action action = null)
    {
        m_listChallengeUIGrid[gridID].BeginCountDown(countingStr, endStr, hour, minute, timesLeft);
    }

    public void BeginCountdown(string countingStr, string stopStr, string endStr, int gridID, int hour, int minute, int second, int timesLeft, Action action = null)
    {
        m_listChallengeUIGrid[gridID].BeginCountDown(countingStr, endStr, hour, minute, second, timesLeft, action);
    }

    public void SetGray(int gridID,bool isGray = true)
    {
        m_listChallengeUIGrid[gridID].SetGray(isGray);
    }

    public void SetTimesLeft(int listID, int times)
    {
        m_listChallengeUIGrid[listID].SetTimesLeft(times);
    }

    public void SetName(int listID, string name)
    {
        m_listChallengeUIGrid[listID].SetName(name);
    }

    public void SetChallengeText(int listID, string name)
    {
        m_listChallengeUIGrid[listID].SetChallengeText(name);
    }

    public void SetChallengeTextColor(int listID, Color32 color)
    {
        m_listChallengeUIGrid[listID].SetChallengeMessageColor(color);
    }

    public void ShowEnterTipFX(int listID, bool isShow)
    {
        m_listChallengeUIGrid[listID].ShowEnterTipFX(isShow);
    }

    public void AddTimer(int listID, uint sec, Action<uint> onTimer, Action callback)
    {
        TimerManager.GetTimer(m_listChallengeUIGrid[listID].gameObject).StartTimer(sec,onTimer,callback);
    }
    public void SetEndText(int listID, string str)
    {
        m_listChallengeUIGrid[listID].SetTimesLeftText(str);
    }

    #endregion

    #region 界面打开和关闭

    protected override void OnEnable()
    {
        base.OnEnable();
        m_spRefreshCtrl.ShowAsWhiteBlack(true, true);
    }

    public void SetGridPos(int gridId, int posId)
    {
        m_listChallengeUIGrid[gridId].transform.localPosition = new Vector3(posId * ITEMSPACE + OFFSET_X, 0, 0);
    }

    #endregion
}
