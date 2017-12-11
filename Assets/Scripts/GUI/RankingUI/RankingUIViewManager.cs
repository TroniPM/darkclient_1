#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：排行榜
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using System;
using Mogo.GameData;

public class RankingUIViewManager : MogoUIBehaviour
{
    private static RankingUIViewManager m_instance;
    public static RankingUIViewManager Instance { get { return RankingUIViewManager.m_instance; } }

    private Transform m_tranRankingUITabList;
    private MogoSingleButtonList m_tabMogoSingleButtonList;
    private MogoListImproved m_tabMogoListImproved;

    private Transform m_tranRankingUIMainRankDataList;
    private MogoSingleButtonList m_mainRankDataMogoSingleButtonList;
    private MogoListImproved m_mainRankDataMogoListImproved;

    private Transform m_tranRankingUIPlayerRankDataList;
    private MogoListImproved m_playerRankDataMogoListImproved;
    private UILabel m_lblRankingUIBtnBecameFSText;

    private List<UILabel> m_listRankingUIMainRankTitle = new List<UILabel>();
    private UILabel m_lblRankingUIMyRankText;

    private GameObject m_goGORankingUIMainRankData;
    private GameObject m_goGORankingUIPanelPlayerInfo;
    private GameObject m_goGORankingUIPanelPlayerEquip;

    private GameObject m_goRankingUIMyRankShowReward;//查看排名奖励

    private GameObject m_goGORankingUIWaitingTip;// 数据刷新提示
    private GameObject m_goRankingUIMainRankDataArrow;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranRankingUITabList = m_myTransform.Find(m_widgetToFullName["RankingUITabList"]);
        m_tabMogoSingleButtonList = m_tranRankingUITabList.GetComponentsInChildren<MogoSingleButtonList>(true)[0];
        m_tabMogoListImproved = m_tranRankingUITabList.GetComponentsInChildren<MogoListImproved>(true)[0];

        m_tranRankingUIMainRankDataList = m_myTransform.Find(m_widgetToFullName["RankingUIMainRankDataList"]);
        m_mainRankDataMogoSingleButtonList = m_tranRankingUIMainRankDataList.GetComponentsInChildren<MogoSingleButtonList>(true)[0];
        m_mainRankDataMogoListImproved = m_tranRankingUIMainRankDataList.GetComponentsInChildren<MogoListImproved>(true)[0];

        m_tranRankingUIPlayerRankDataList = m_myTransform.Find(m_widgetToFullName["RankingUIPlayerRankDataList"]);
        m_playerRankDataMogoListImproved = m_tranRankingUIPlayerRankDataList.GetComponentsInChildren<MogoListImproved>(true)[0];
        m_lblRankingUIBtnBecameFSText = m_myTransform.Find(m_widgetToFullName["RankingUIBtnBecameFSText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_listRankingUIMainRankTitle.Add(m_myTransform.Find(m_widgetToFullName["RankingUIMainRankTitle1"]).GetComponentsInChildren<UILabel>(true)[0]);
        m_listRankingUIMainRankTitle.Add(m_myTransform.Find(m_widgetToFullName["RankingUIMainRankTitle2"]).GetComponentsInChildren<UILabel>(true)[0]);
        m_listRankingUIMainRankTitle.Add(m_myTransform.Find(m_widgetToFullName["RankingUIMainRankTitle3"]).GetComponentsInChildren<UILabel>(true)[0]);
        m_listRankingUIMainRankTitle.Add(m_myTransform.Find(m_widgetToFullName["RankingUIMainRankTitle4"]).GetComponentsInChildren<UILabel>(true)[0]);
        m_listRankingUIMainRankTitle.Add(m_myTransform.Find(m_widgetToFullName["RankingUIMainRankTitle5"]).GetComponentsInChildren<UILabel>(true)[0]);
        for (int i = 0; i < m_listRankingUIMainRankTitle.Count; i++)
        {
            m_listRankingUIMainRankTitle[i].effectStyle = UILabel.Effect.None;
            m_listRankingUIMainRankTitle[i].color = new Color32(63, 27, 4, 255);
        }

        m_lblRankingUIMyRankText = m_myTransform.Find(m_widgetToFullName["RankingUIMyRankText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_goGORankingUIMainRankData = m_myTransform.Find(m_widgetToFullName["GORankingUIMainRankData"]).gameObject;
        m_goGORankingUIPanelPlayerInfo = m_myTransform.Find(m_widgetToFullName["GORankingUIPanelPlayerInfo"]).gameObject;
        m_goGORankingUIPanelPlayerEquip = m_myTransform.Find(m_widgetToFullName["GORankingUIPanelPlayerEquip"]).gameObject;

        m_goRankingUIModelShowCase = m_myTransform.Find(m_widgetToFullName["RankingUIModelShowCase"]).gameObject;
        m_modelShowCase = m_goRankingUIModelShowCase.AddComponent<ModelShowCase>();
        m_modelShowCase.left = m_myTransform.Find(m_widgetToFullName["RankingUIModelViewTopLeft"]);
        m_modelShowCase.right = m_myTransform.Find(m_widgetToFullName["RankingUIModelViewBottomRight"]);

        m_goRankingUIMyRankShowReward = m_myTransform.Find(m_widgetToFullName["RankingUIMyRankShowReward"]).gameObject;

        m_goGORankingUIWaitingTip = m_myTransform.Find(m_widgetToFullName["GORankingUIWaitingTip"]).gameObject;
        m_goRankingUIMainRankDataArrow = m_myTransform.Find(m_widgetToFullName["RankingUIMainRankDataArrow"]).gameObject;


        m_goPanelPlayerEquip = m_myTransform.Find(m_widgetToFullName["PanelPlayerEquip"]).gameObject;
        m_panelPlayerEquip = m_goPanelPlayerEquip.AddComponent<PanelPlayerEquip>();

        m_myTransform.Find(m_widgetToFullName["RankingUIBtnClose"]).gameObject.AddComponent<RankingUIButton>();
        m_myTransform.Find(m_widgetToFullName["RankingUIBtnBackToRank"]).gameObject.AddComponent<RankingUIButton>();
        m_myTransform.Find(m_widgetToFullName["RankingUIBtnBecameFS"]).gameObject.AddComponent<RankingUIButton>();
        m_myTransform.Find(m_widgetToFullName["RankingUIMyRankShowReward"]).gameObject.AddComponent<RankingUIButton>();

        Initialize();
    }

    #region 事件

    public Action RANKINGUICLOSEUP;
    public Action RANKINGUIBACKTORANK;
    public Action RANKINGUIBECAMEFS;
    public Action<int> RANKINGUIPAGEUP;
    public Action<int> RANKINGUISHOWREWARDUP;

    public void Initialize()
    {
        // 翻页
        m_mainRankDataMogoListImproved.BackToMaxPage += OnRankMainDataBackToMaxPage;
        m_mainRankDataMogoListImproved.MovePageDone += OnRankMainDataMovePageDone;

        // 滑动
        m_mainRankDataMogoListImproved.BackToLastPoint += OnRankMainDataBackToLastPoint;

        RankingUIDict.ButtonTypeToEventUp.Add("RankingUIBtnClose", OnCloseUp);
        RankingUIDict.ButtonTypeToEventUp.Add("RankingUIBtnBackToRank", OnBackToRankUp);
        RankingUIDict.ButtonTypeToEventUp.Add("RankingUIBtnBecameFS", OnBecameFSUp);
        RankingUIDict.ButtonTypeToEventUp.Add("RankingUIMyRankShowReward", OnShowRewardUp);

        

        RankingUILogicManager.Instance.Initialize();
        m_uiLoginManager = RankingUILogicManager.Instance;
    }

    public void Release()
    {
        m_mainRankDataMogoListImproved.BackToMaxPage -= OnRankMainDataBackToMaxPage;
        m_mainRankDataMogoListImproved.MovePageDone -= OnRankMainDataMovePageDone;

        RankingUILogicManager.Instance.Release();
        RankingUIDict.ButtonTypeToEventUp.Clear();
    }

    void OnCloseUp(int i)
    {
        if (RANKINGUICLOSEUP != null)
            RANKINGUICLOSEUP();
    }

    void OnBackToRankUp(int i)
    {
        if (RANKINGUIBACKTORANK != null)
            RANKINGUIBACKTORANK();
    }

    void OnBecameFSUp(int i)
    {
        if (RANKINGUIBECAMEFS != null)
            RANKINGUIBECAMEFS();
    }

    void OnRankMainDataBackToMaxPage(int maxPage)
    {
        // 翻页未更新已有数据(后续处理，当前只有获取数据，没有更新数据)
        if (RANKINGUIPAGEUP != null)
        {
            // +1为获取下一页数据
            if (maxPage < MAX_PAGE_INDEX)
            {
                CurrentPage = maxPage + 1;
                RANKINGUIPAGEUP(maxPage + 1);
            }
            else
            {
                CurrentPage = maxPage;
            }
        }
    }

    void OnRankMainDataMovePageDone()
    {
        // 如果当前页在标记当前页的前一页，即请求下一页操作，不处理
        if(m_mainRankDataMogoListImproved.CurrentPage != CurrentPage - 1)
            CurrentPage = m_mainRankDataMogoListImproved.CurrentPage;
    }

    /// <summary>
    /// 滑动
    /// </summary>
    void OnRankMainDataBackToLastPoint()
    {
       
    }

    void OnShowRewardUp(int i)
    {
        if (RANKINGUISHOWREWARDUP != null)
            RANKINGUISHOWREWARDUP(CurrentTabIndex);
    }

    #endregion

    #region 左列选项卡

    int m_currentTabIndex = 0;
    public int CurrentTabIndex
    {
        get
        {
            return m_currentTabIndex;
        }
        set
        {
            m_currentTabIndex = value;
            SetRankDataTitle();
            SetRankDataTip();
            SetRewardButton();
        }
    }

    List<int> m_sortTabIDList = new List<int>();

    /// <summary>
    /// 创建选项卡Grid
    /// </summary>
    /// <param name="data"></param>
    public void GenerateRankingTabList(List<int> sortTabIDList)
    {
        if (RankingUILogicManager.Instance == null)
            return;

        m_sortTabIDList = sortTabIDList;
        m_tabMogoListImproved.SetGridLayout<RankingUITabGrid>(m_sortTabIDList.Count, m_tranRankingUITabList.transform, RankingTabListResourceLoaded);
    }

    /// <summary>
    /// 设置选项卡
    /// </summary>
    void RankingTabListResourceLoaded()
    {
        if (RankingUILogicManager.Instance == null)
            return;

        var m_dataList = m_tabMogoListImproved.DataList;

        m_tabMogoSingleButtonList.SingleButtonList.Clear();
        for (int i = 0; i < m_dataList.Count; i++)
        {
            RankingUITabGrid rankingUITabGrid = (RankingUITabGrid)m_dataList[i];
            RankData xmlRankData = RankData.GetRankDataByID(m_sortTabIDList[i]);
            rankingUITabGrid.TabID = i;

            if (xmlRankData != null)
            {
                rankingUITabGrid.RankingUITabText = LanguageData.GetContent(xmlRankData.name);
                if (xmlRankData.ifReward == 1)
                    rankingUITabGrid.IfReward = true;
                else
                    rankingUITabGrid.IfReward = false;
            }

            // SingleButtonList
            m_tabMogoSingleButtonList.SingleButtonList.Add(rankingUITabGrid.GetComponentsInChildren<MogoSingleButton>(true)[0]);

            // BoxCollider区域添加滑动
            MogoButton mogoButton = rankingUITabGrid.GetComponent<MogoButton>();
            if (mogoButton == null)
                mogoButton = rankingUITabGrid.gameObject.AddComponent<MogoButton>();
            mogoButton.pressHandler = m_tabMogoListImproved.PressHandlerOutSide;
            mogoButton.dragHandler = m_tabMogoListImproved.DragHandlerOutSide;

            m_tabMogoSingleButtonList.SetCurrentDownButton(0); // 默认选择第一项
        }
    }

    /// <summary>
    /// 设置排行榜的Title
    /// </summary>
    void SetRankDataTitle()
    {
        if (m_currentTabIndex < m_sortTabIDList.Count)
        {
            RankData xmlRankData = RankData.GetRankDataByID(m_sortTabIDList[m_currentTabIndex]);
            if (xmlRankData != null)
            {
                int index = 0;
                foreach (int title in xmlRankData.title.Values)
                {
                    m_listRankingUIMainRankTitle[index++].text = LanguageData.GetContent(title);
                }

                for (; index < m_listRankingUIMainRankTitle.Count; index++)
                {
                    m_listRankingUIMainRankTitle[index].text = "";
                }
            }
        }
    }

    /// <summary>
    /// 设置排行榜的随机Tip
    /// </summary>
    void SetRankDataTip()
    {
        if (m_currentTabIndex < m_sortTabIDList.Count)
        {
            int tip = RankData.GetRandomTipByID(m_sortTabIDList[m_currentTabIndex]);
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(tip));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void SetRewardButton()
    {
        m_goRankingUIMyRankShowReward.SetActive(false);
        if (m_currentTabIndex < m_sortTabIDList.Count)
        {
            RankData xmlRankData = RankData.GetRankDataByID(m_sortTabIDList[m_currentTabIndex]);
            if (xmlRankData.ifReward == 1)
            {
                m_goRankingUIMyRankShowReward.SetActive(true);
                //m_goRankingUIMyRankShowReward.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;

                //UISprite sp = m_goRankingUIMyRankShowReward.transform.FindChild("RankingUIMyRankShowRewardBGUp").GetComponentsInChildren<UISprite>(true)[0];
                //sp.spriteName = "btn_03grey";                
            }
        }
    }

    #endregion

    #region 右列排名数据

    private readonly static int MAX_PAGE_COUNT = 10; // 最多10页
    private readonly static int MAX_PAGE_INDEX = MAX_PAGE_COUNT - 1; // 最多10页
    private readonly static int ONE_PAGE_COUNT = 10; // 每页10条数据

    // 默认页
    public readonly static int DefaultPage = 0;    

    // 标记当前页
    int m_currentPage = DefaultPage;
    public int CurrentPage
    {
        get { return m_currentPage; }
        set
        {
            m_currentPage = value;
            //Debug.LogError("0.The flag CurrentPage is " + m_currentPage);
        }
    }
   
    private List<RankingMainData> m_rankingMainDataList;

    /// <summary>
    /// 创建排名数据Grid
    /// </summary>
    /// <param name="data"></param>
    public void GenerateRankingMainDataList(List<RankingMainData> rankingMainDataList, bool hasData)
    {
        m_rankingMainDataList = rankingMainDataList;

        // 按照排名进行排序
        m_rankingMainDataList.Sort(delegate(RankingMainData a, RankingMainData b)
        {
            if (a.uniqieRank < b.uniqieRank)
                return -1;
            else
                return 1;
        });

        m_mainRankDataMogoListImproved.SetGridLayout<RankingUIRankData>(m_rankingMainDataList.Count, m_tranRankingUIMainRankDataList.transform, RankingMainDataListResourceLoaded);
        SetMainRankDataArrow(rankingMainDataList.Count, hasData);
    }

    /// <summary>
    /// 设置排名数据
    /// </summary>
    void RankingMainDataListResourceLoaded()
    {
        var m_dataList = m_mainRankDataMogoListImproved.DataList;

        m_mainRankDataMogoSingleButtonList.SingleButtonList.Clear();
        for (int i = 0; i < m_dataList.Count; i++)
        {
            RankingUIRankData rankingUIRankData = (RankingUIRankData)m_dataList[i];
            rankingUIRankData.Index = i;
            rankingUIRankData.RankingUIMainRankData1 = m_rankingMainDataList[i].uniqieRank.ToString();
            rankingUIRankData.RankingUIMainRankData2Name = m_rankingMainDataList[i].recordName;
            rankingUIRankData.RankingUIMainRankData3 = m_rankingMainDataList[i].level;
            rankingUIRankData.RankingUIMainRankData4 = m_rankingMainDataList[i].attrib.ToString("N0");
            rankingUIRankData.RankingUIMainRankData5FansCount = m_rankingMainDataList[i].fansCount;
            rankingUIRankData.AvatarID = m_rankingMainDataList[i].tdbID;

            // SingleButtonList
            m_mainRankDataMogoSingleButtonList.SingleButtonList.Add(rankingUIRankData.GetComponentsInChildren<MogoSingleButton>(true)[0]);

            // BoxCollider区域添加滑动
            MogoButton mogoButton = rankingUIRankData.GetComponent<MogoButton>();
            if (mogoButton == null)
                mogoButton = rankingUIRankData.gameObject.AddComponent<MogoButton>();
            mogoButton.pressHandler = m_mainRankDataMogoListImproved.PressHandlerOutSide;
            mogoButton.dragHandler = m_mainRankDataMogoListImproved.DragHandlerOutSide;
        }

        m_mainRankDataMogoListImproved.StopTween();
        m_mainRankDataMogoListImproved.ResetCameraPos();

        if (m_mainRankDataMogoListImproved.gameObject.activeSelf)
        {
            if (CurrentPage >= 0 && CurrentPage <= m_mainRankDataMogoListImproved.MaxPageIndex)
            {
                //Debug.LogError("1.CurrentPage is " + CurrentPage);
                m_mainRankDataMogoListImproved.TweenTo(CurrentPage, true); // 数据返回时滑动到标记页
            }
            else if (m_playerRankDataMogoListImproved.CurrentPage <= m_playerRankDataMogoListImproved.MaxPageIndex)
            {
                //Debug.LogError("2.CurrentPage is " + m_playerRankDataMogoListImproved.CurrentPage);
                m_mainRankDataMogoListImproved.TweenTo(m_playerRankDataMogoListImproved.CurrentPage, true);
                CurrentPage = m_playerRankDataMogoListImproved.CurrentPage; 
            }
            else
            {
                //Debug.LogError("3.CurrentPage is " + 0);
                m_mainRankDataMogoListImproved.TweenTo(0, true);
                CurrentPage = 0;
            }
        }
        else
        {
            m_mainRankDataMogoListImproved.TweenTo(m_playerRankDataMogoListImproved.CurrentPage, true);
            CurrentPage = m_playerRankDataMogoListImproved.CurrentPage;
        }

        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
        RankingUIViewManager.Instance.ShowRankingUIWaitingTip(false);
    }

    /// <summary>
    /// 变更偶像需要刷新排行UI数据
    /// </summary>
    public void RefreshRankingMainDataList()
    {
        var m_dataList = m_mainRankDataMogoListImproved.DataList;
        for (int i = 0; i < m_dataList.Count; i++)
        {
            RankingUIRankData rankingUIRankData = (RankingUIRankData)m_dataList[i];
            rankingUIRankData.AvatarID = rankingUIRankData.AvatarID;
            rankingUIRankData.RankingUIMainRankData5FansCount = rankingUIRankData.RankingUIMainRankData5FansCount;
        }
    }

    public void ResetMainRankDataUI()
    {
        m_mainRankDataMogoListImproved.ResetCameraPos();
        m_mainRankDataMogoSingleButtonList.SetCurrentDownButton(0);
    }

    private void SetMainRankDataArrow(int num, bool hasData)
    {
        if (!hasData && num <= ONE_PAGE_COUNT)
            m_goRankingUIMainRankDataArrow.SetActive(false);
        else
            m_goRankingUIMainRankDataArrow.SetActive(true);
    }

    #endregion

    #region 玩家详细排名信息

    public readonly static int MAXRANKNUM = 100;// 排行榜最大排行数量

    /// <summary>
    /// 玩家排行榜各榜单数据
    /// </summary>
    struct PlayerRankData
    {
        public string rankName;
        public int rankNum;
    }

    List<PlayerRankData> m_playerRankDataList = new List<PlayerRankData>();// 玩家排行榜各榜单数据列表

    /// <summary>
    /// 创建玩家各榜单排名Grid
    /// </summary>
    /// <param name="data"></param>
    public void GeneratePlayerRankDataList(List<int> rankNumList)
    {
        Dictionary<int, int> m_rankNumMap = new Dictionary<int, int>();// 排行榜ID->排名

        // 服务器返回的玩家排行榜排名是按照xml表顺序的，未经排序，故此处应按priority排序
        int i = 0;
        foreach (int rankID in RankData.dataMap.Keys)
        {
            if (i < rankNumList.Count)
            {
                m_rankNumMap[rankID] = rankNumList[i];
                i++;
            }
            else
            {
                break;
            }
        }

        m_playerRankDataList.Clear();
        for (int rankIndex = 0; rankIndex < m_sortTabIDList.Count; rankIndex++)
        {
            RankData xmlRankData = RankData.GetRankDataByID(m_sortTabIDList[rankIndex]);
            if (xmlRankData != null)
            {
                PlayerRankData playerRankData;
                playerRankData.rankName = LanguageData.GetContent(xmlRankData.name);
                if (m_rankNumMap.ContainsKey(m_sortTabIDList[rankIndex]))
                    playerRankData.rankNum = m_rankNumMap[m_sortTabIDList[rankIndex]];
                else
                    playerRankData.rankNum = 0;

                m_playerRankDataList.Add(playerRankData);
            }
        }

        m_playerRankDataMogoListImproved.SetGridLayout<RankingUIPlayerRankData>(m_playerRankDataList.Count, m_tranRankingUIPlayerRankDataList.transform, PlayerRankDataListResourceLoaded);
    }

    /// <summary>
    /// 设置玩家各榜单排名数据
    /// </summary>
    void PlayerRankDataListResourceLoaded()
    {
        var m_dataList = m_playerRankDataMogoListImproved.DataList;

        for (int i = 0; i < m_dataList.Count; i++)
        {
            RankingUIPlayerRankData rankingUIPlayerRankData = (RankingUIPlayerRankData)m_dataList[i];
            rankingUIPlayerRankData.RankingUIPlayerRankDataName = m_playerRankDataList[i].rankName;
            if (m_playerRankDataList[i].rankNum > 0 && m_playerRankDataList[i].rankNum <= MAXRANKNUM)
                rankingUIPlayerRankData.RankingUIPlayerRankDataRank = m_playerRankDataList[i].rankNum.ToString();
            else
                rankingUIPlayerRankData.RankingUIPlayerRankDataRank = LanguageData.GetContent(47400);

            // BoxCollider区域添加滑动
            //MogoButton mogoButton = rankingUIPlayerRankData.GetComponent<MogoButton>();
            //if (mogoButton == null)
            //    mogoButton = rankingUIPlayerRankData.gameObject.AddComponent<MogoButton>();
            //mogoButton.pressHandler = m_tabMogoListImproved.PressHandlerOutSide;
            //mogoButton.dragHandler = m_tabMogoListImproved.DragHandlerOutSide;
        }
    }

    /// <summary>
    /// 设置玩家自己的排名
    /// </summary>
    /// <param name="rank"></param>
    public void SetMyRankNum(int rank)
    {
        if (rank > 0 && rank <= MAXRANKNUM)
        {
            m_lblRankingUIMyRankText.text = string.Format(LanguageData.GetContent(47401), rank);
        }
        else
        {
            m_lblRankingUIMyRankText.text = string.Format(LanguageData.GetContent(47401), LanguageData.GetContent(47400));
        }
    }

    /// <summary>
    /// 设置粉丝按钮Text
    /// </summary>
    /// <param name="text"></param>
    public void SetBecameFSButtonText(string text)
    {
        m_lblRankingUIBtnBecameFSText.text = text;
    }

    #endregion

    #region 玩家装备展示

    private GameObject m_goPanelPlayerEquip;
    private PanelPlayerEquip m_panelPlayerEquip = null;

    // 由于位置和背景需要调整，玩家装备Panel放弃动态加载
    //bool IsPanelPlayerEquipLoaded = false;
    //private void ShowPanelPlayerEquip(Action actionSetEquip)
    //{
    //    if (m_panelPlayerEquip == null)
    //    {
    //        if (!IsPanelPlayerEquipLoaded)
    //        {
    //            IsPanelPlayerEquipLoaded = true;

    //            AssetCacheMgr.GetUIInstance("PanelPlayerEquip.prefab", (prefab, guid, go) =>
    //            {
    //                GameObject obj = (GameObject)go;
    //                obj.transform.parent = m_goGORankingUIPanelPlayerEquip.transform;
    //                obj.transform.localPosition = Vector3.zero;
    //                obj.transform.localScale = new Vector3(1, 1, 1);
    //                m_panelPlayerEquip = obj.AddComponent<PanelPlayerEquip>();

    //                // SetEquip
    //                actionSetEquip();
    //            });
    //        }
    //    }
    //    else
    //    {
    //        // SetEquip
    //        actionSetEquip();
    //    }
    //}

    public void ShowPanelPlayerEquip(List<RankEquipData> equipList, string name, int level)
    {
        //RankingUIViewManager.Instance.ShowPanelPlayerEquip(() =>
        //{
        //    m_panelPlayerEquip.RefreshPlayerEquipmentInfoUI(equipList);
        //    m_panelPlayerEquip.SetPlayerInfoNameAndLevel(name, level);
        //});

        m_panelPlayerEquip.LoadResourceInsteadOfAwake();
        m_panelPlayerEquip.RefreshPlayerEquipmentInfoUI(equipList);
        m_panelPlayerEquip.SetPlayerInfoNameAndLevel(name, level);
    }

    #endregion

    #region 排行榜模型

    private GameObject m_goRankingUIModelShowCase;
    private ModelShowCase m_modelShowCase;

    public void SetModelShow(int vocation, List<int> weaponList, bool isShow)
    {
        m_modelShowCase.LoadCreateCharacter(vocation, weaponList,
            () =>
            {
                Transform tranModel = FindTransform("RankingUIModelShowCaseCamera");
                tranModel.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = 
                    GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
                m_modelShowCase.SetCamera(tranModel);
                ModelShowCaseLogicManager.Instance.ShowModel(vocation, isShow);
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
    }

    public void CloseAllModel()
    {
        ModelShowCaseLogicManager.Instance.CloseAllModel();
    }

    #endregion

    #region  面板之间的切换

    /// <summary>
    /// 切换显示排行榜数据面板
    /// </summary>
    public void SwitchRankingMainDataUI()
    {
        ShowPlayerInfoUI(false);
        ShowMainRankDataUI(true);
    }

    /// <summary>
    /// 切换显示玩家具体信息面板
    /// </summary>
    public void SwitchPlayerDetailInfoUI()
    {
        ShowMainRankDataUI(false);
        ShowPlayerInfoUI(true);
    }

    /// <summary>
    /// 显示或隐藏排行榜数据面板
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowMainRankDataUI(bool isShow)
    {
        // 切换面板后数据Camera停止滑动
        m_mainRankDataMogoListImproved.StopTween();
        if (CurrentPage >= 0 && CurrentPage <= m_mainRankDataMogoListImproved.MaxPageIndex)
            m_mainRankDataMogoListImproved.TweenTo(CurrentPage, true);

        m_goGORankingUIMainRankData.SetActive(isShow);
    }

    /// <summary>
    /// 显示玩家具体信息面板
    /// </summary>
    private void ShowPlayerInfoUI(bool isShow)
    {
        if (!isShow)
            CloseAllModel();

        m_goGORankingUIPanelPlayerInfo.SetActive(isShow);
    }

    #endregion

    #region 数据刷新等待界面

    bool m_bShowWaitingTip = false;
    public void ShowRankingUIWaitingTip(bool isShow)
    {
        m_bShowWaitingTip = isShow;
        MogoUIManager.Instance.GetSwitchUICamera().gameObject.SetActive(isShow);
        m_goGORankingUIWaitingTip.SetActive(isShow);
    }

    const float MAXWAITINGTIME = 10.0f;
    private float m_fCurrentTime = 0f;
    void Update()
    {
        if (m_bShowWaitingTip)
        {
            m_fCurrentTime += Time.deltaTime;
            if (m_fCurrentTime >= MAXWAITINGTIME)
            {
                ShowRankingUIWaitingTip(false);
                m_fCurrentTime = 0.0f;
            }
        }
        else
        {
            m_fCurrentTime = 0.0f;
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

        SetRankDataTitle();
        SetRankDataTip();

        EventDispatcher.TriggerEvent(RankEvent.OnSelfIdolReq);
        if (RankingUIDict.RANKINGTABUP != null)
            RankingUIDict.RANKINGTABUP(0);
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        CloseAllModel();

        if (SystemSwitch.DestroyAllUI)
        {
            if (m_panelPlayerEquip != null)
                m_panelPlayerEquip.Release();

            MogoUIManager.Instance.DestroyRankingUI();
        }
    }

    #endregion
}
