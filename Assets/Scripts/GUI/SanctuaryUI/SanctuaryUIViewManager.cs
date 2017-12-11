using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;

public class RankRewardGridData
{
    public string imgName;
    public string text;
}

public class RankGridData
{
    public string rank;
    public string name;
    public string achieve;
    public bool highLight;
}

public enum SanctuaryUITab
{
    MyAchievementTab = 1,
    WeekRankTab = 2,
    TodayRankTab = 3,
}

public class SanctuaryUIViewManager : MogoUIBehaviour
{
    private static SanctuaryUIViewManager m_instance;
    public static SanctuaryUIViewManager Instance { get { return SanctuaryUIViewManager.m_instance; } }

    private MogoButton m_closeBtn;
    private MogoButton m_myAchieveBtn;
    private MogoButton m_weekRankBtn;
    private MogoButton m_todayRankBtn;
    private MogoButton m_achieveReardBtn;

    private GameObject[] m_goSanctuaryUIDialogList = new GameObject[3];
    private Dictionary<int, UILabel> m_sanctuaryTabLabelList = new Dictionary<int, UILabel>();

    private Camera m_camRankGrid;
    private Camera m_camRewardGrid;

    private Transform m_transRewardGridList;
    private Transform m_transRankGridList;

    private List<GameObject> m_listRewardGrid = new List<GameObject>();
    private List<GameObject> m_listRankGrid = new List<GameObject>();

    private GameObject[] m_arrRankPlayerGrid = new GameObject[5];

    private UILabel m_lblCurrentAchieve;
    private UILabel m_lblNextAchieve;
    private UILabel m_lblNextAchievementRewardGoldNum;

    private UILabel m_lblPlayerRank;
    private UILabel m_lblPlayerName;
    private UILabel m_lblPlayerContribute;

    private UISprite m_spAchieveReward;
    private GameObject m_goRewardTip;

    private int m_iCurrentIcon = 0;

    // �ҵĹ���
    private Transform m_tranMyAchievementDialogRewardList;


    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_closeBtn = m_myTransform.Find(m_widgetToFullName["SanctuaryUIClose"]).GetComponentsInChildren<MogoButton>(true)[0];
        m_myAchieveBtn = m_myTransform.Find(m_widgetToFullName["MyAchievementIcon"]).GetComponentsInChildren<MogoButton>(true)[0];
        m_weekRankBtn = m_myTransform.Find(m_widgetToFullName["WeekRankIcon"]).GetComponentsInChildren<MogoButton>(true)[0];
        m_todayRankBtn = m_myTransform.Find(m_widgetToFullName["TodayRankIcon"]).GetComponentsInChildren<MogoButton>(true)[0];
        m_achieveReardBtn = m_myTransform.Find(m_widgetToFullName["NextAchievementItem"]).GetComponentsInChildren<MogoButton>(true)[0];

        m_goSanctuaryUIDialogList[0] = m_myTransform.Find(m_widgetToFullName["MyAchievementDialog"]).gameObject;
        m_goSanctuaryUIDialogList[1] = m_myTransform.Find(m_widgetToFullName["WeekRankDialog"]).gameObject;
        m_goSanctuaryUIDialogList[2] = m_myTransform.Find(m_widgetToFullName["TodayRankDialog"]).gameObject;

        m_camRankGrid = m_myTransform.Find(m_widgetToFullName["WeekRankDialogBodyGridListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_camRewardGrid = m_myTransform.Find(m_widgetToFullName["WeekRankDialogRewardListCamera"]).GetComponentsInChildren<Camera>(true)[0];

        m_camRankGrid.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_camRewardGrid.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_lblCurrentAchieve = m_myTransform.Find(m_widgetToFullName["CurrentAchievementNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblNextAchieve = m_myTransform.Find(m_widgetToFullName["NextAchievementNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblNextAchievementRewardGoldNum = m_myTransform.Find(m_widgetToFullName["NextAchievementRewardGoldNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_spAchieveReward = m_myTransform.Find(m_widgetToFullName["NextAchievementItemFG"]).GetComponentsInChildren<UISprite>(true)[0];

        m_transRankGridList = m_myTransform.Find(m_widgetToFullName["WeekRankDialogBodyGridList"]);
        m_transRewardGridList = m_myTransform.Find(m_widgetToFullName["WeekRankDialogRewardList"]);

        m_goRewardTip = m_myTransform.Find(m_widgetToFullName["WeekRankDialogRewardTip"]).gameObject;
        m_goRewardTip.AddComponent<RewardGridTip>().TipNum = 4;

        m_lblPlayerRank = m_myTransform.Find(m_widgetToFullName["WeekRankDialogBottomRank"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblPlayerName = m_myTransform.Find(m_widgetToFullName["WeekRankDialogBottomName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblPlayerContribute = m_myTransform.Find(m_widgetToFullName["WeekRankDialogBottomAchieve"]).GetComponentsInChildren<UILabel>(true)[0];

        for (int i = 0; i < 5; ++i)
        {
            m_arrRankPlayerGrid[i] = m_myTransform.Find(m_widgetToFullName["WeekRankDialogBodyGrid" + i]).gameObject;
        }

        m_tranMyAchievementDialogRewardList = FindTransform("MyAchievementDialogRewardList");

        m_sanctuaryTabLabelList[(int)SanctuaryUITab.MyAchievementTab] = m_myTransform.Find(m_widgetToFullName["MyAchievementIconText"]).GetComponent<UILabel>();
        m_sanctuaryTabLabelList[(int)SanctuaryUITab.WeekRankTab] = m_myTransform.Find(m_widgetToFullName["WeekRankIconText"]).GetComponent<UILabel>();
        m_sanctuaryTabLabelList[(int)SanctuaryUITab.TodayRankTab] = m_myTransform.Find(m_widgetToFullName["TodayRankIconText"]).GetComponent<UILabel>();
        foreach (var pair in m_sanctuaryTabLabelList)
        {
            if (pair.Key == (int)SanctuaryUITab.MyAchievementTab)
                SanctuaryTabDown(pair.Key);
            else
                SanctuaryTabUp(pair.Key);
        }

        // ChineseData
        FindTransform("SanctuaryUIName").GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.GetContent(24030);

        Initialize();
    }

    #region �¼�

    void Initialize()
    {
        m_closeBtn.clickHandler += OnCloseBtnUp;
        m_myAchieveBtn.clickHandler += OnMyAchieveBtnUp;
        m_weekRankBtn.clickHandler += OnWeekRankBtnUp;
        m_todayRankBtn.clickHandler += OnTodayRankBtnUp;
        m_achieveReardBtn.clickHandler += OnAchieveRewardBtnUp;

        EventDispatcher.AddEventListener<int>(SantuaryUIDict.SanctuaryUIEvent.REWARDGRIDUP, OnRewardGridUp);
        EventDispatcher.AddEventListener<int>(SantuaryUIDict.SanctuaryUIEvent.REWARDGRIDDOWN, OnRewardGridDown);
        EventDispatcher.AddEventListener<int>(SantuaryUIDict.SanctuaryUIEvent.REWARDGRIDDRAG, OnRewardGridDrag);

        SanctuaryUILogicManager.Instance.Initialize();
    }

    public void Release()
    {
        SantuaryUIDict.ButtonTypeToEventUp.Clear();

        m_closeBtn.clickHandler -= OnCloseBtnUp;
        m_myAchieveBtn.clickHandler -= OnMyAchieveBtnUp;
        m_weekRankBtn.clickHandler -= OnWeekRankBtnUp;
        m_todayRankBtn.clickHandler -= OnTodayRankBtnUp;
        m_achieveReardBtn.clickHandler -= OnAchieveRewardBtnUp;

        EventDispatcher.RemoveEventListener<int>(SantuaryUIDict.SanctuaryUIEvent.REWARDGRIDUP, OnRewardGridUp);
        EventDispatcher.RemoveEventListener<int>(SantuaryUIDict.SanctuaryUIEvent.REWARDGRIDDOWN, OnRewardGridDown);
        EventDispatcher.RemoveEventListener<int>(SantuaryUIDict.SanctuaryUIEvent.REWARDGRIDDRAG, OnRewardGridDrag);

        SanctuaryUILogicManager.Instance.Release();
    }

    void OnRewardGridUp(int id)
    {
        ShowRewardGridTip(false);
    }

    void OnRewardGridDown(int i)
    {
        ShowRewardGridTip(true);
        switch ((SanctuaryUITab)CurPageTab)
        {
            case SanctuaryUITab.MyAchievementTab:
                break;
            case SanctuaryUITab.WeekRankTab:
                {
                    List<string> tips = SanctuaryRewardXMLData.GetWeekRewardNameList(i);
                    SanctuaryUIViewManager.Instance.SetRewardGridTipData(tips, String.Format("��{0}������", i + 1));
                }
                break;
            case SanctuaryUITab.TodayRankTab:
                {
                    List<string> tips = SanctuaryRewardXMLData.GetDayRewardNameList(i);
                    SanctuaryUIViewManager.Instance.SetRewardGridTipData(tips, String.Format("��{0}������", i + 1));
                }
                break;
            default:
                break;
        }
    }

    void OnRewardGridDrag(int id)
    {
        ShowRewardGridTip(false);
    }

    void OnCloseBtnUp()
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnMyAchieveBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("MyAchieveIconUp");
        CurPageTab = (int)SanctuaryUITab.MyAchievementTab;
        if (0 != m_iCurrentIcon)
        {
            m_goSanctuaryUIDialogList[m_iCurrentIcon].SetActive(false);
            m_iCurrentIcon = 0;
            m_goSanctuaryUIDialogList[m_iCurrentIcon].SetActive(true);
        }
        SanctuaryUILogicManager.Instance.RefreshUI(0);
    }

    void OnWeekRankBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("WeekRankBtnUp");
        CurPageTab = (int)SanctuaryUITab.WeekRankTab;
        if (1 != m_iCurrentIcon)
        {
            m_goSanctuaryUIDialogList[m_iCurrentIcon].SetActive(false);
            m_iCurrentIcon = 1;
            m_goSanctuaryUIDialogList[m_iCurrentIcon].SetActive(true);
        }
        SanctuaryUILogicManager.Instance.RefreshUI(1);
    }

    void OnTodayRankBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("TodayRankBtnUp");
        CurPageTab = (int)SanctuaryUITab.TodayRankTab;
        if (1 != m_iCurrentIcon)
        {
            m_goSanctuaryUIDialogList[m_iCurrentIcon].SetActive(false);
            m_iCurrentIcon = 1;
            m_goSanctuaryUIDialogList[m_iCurrentIcon].SetActive(true);
        }
        SanctuaryUILogicManager.Instance.RefreshUI(2);
    }

    void OnAchieveRewardBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("Reward");
        int id = SanctuaryRewardXMLData.GetAccuNextRankItemID((int)SanctuaryUILogicManager.Instance.weekContri);
        InventoryManager.Instance.ShowItemTip(id, null);
    }


    #endregion

    #region Tab Change

    int m_curPageTab = 1;
    public int CurPageTab
    {
        get
        {
            return m_curPageTab;
        }
        set
        {
            HandleSanctuaryTabChange(m_curPageTab, value);
            m_curPageTab = value;
        }
    }

    public void HandleSanctuaryTabChange(int fromTab, int toTab)
    {
        SanctuaryTabUp(fromTab);
        SanctuaryTabDown(toTab);
    }

    void SanctuaryTabUp(int tab)
    {
        if (m_sanctuaryTabLabelList.ContainsKey(tab))
        {
            UILabel fromTabLabel = m_sanctuaryTabLabelList[tab];
            if (fromTabLabel != null)
            {
                fromTabLabel.color = new Color32(37, 29, 6, 255);
                fromTabLabel.effectStyle = UILabel.Effect.None;
            }
        }
    }

    void SanctuaryTabDown(int tab)
    {
        if (m_sanctuaryTabLabelList.ContainsKey(tab))
        {
            UILabel toTabLabel = m_sanctuaryTabLabelList[tab];
            if (toTabLabel != null)
            {
                toTabLabel.color = new Color32(255, 255, 255, 255);
                toTabLabel.effectStyle = UILabel.Effect.Outline;
                toTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }
    }

    #endregion

    #region ������Ϣ

    public void SetPlayerRank(string rank)
    {
        m_lblPlayerRank.text = LanguageData.dataMap[24025].Format(rank);
    }

    public void SetPlayerName(string name)
    {
        m_lblPlayerName.text = name;
    }

    public void SetPlayerContribute(string contribute)
    {
        m_lblPlayerContribute.text = contribute;
    }

    public void SetCurrentAchieve(string num)
    {
        m_lblCurrentAchieve.text = num;
    }

    public void SetNextAchieve(string num)
    {
        m_lblNextAchieve.text = LanguageData.dataMap[24024].Format(num);
    }

    public void SetNextAchievementRewardGoldNum(int num)
    {
        m_lblNextAchievementRewardGoldNum.text = "x" + num;
    }

    public void SetAcieveReward(string imgName)
    {
        LoggerHelper.Debug(imgName);
        m_spAchieveReward.spriteName = imgName;
    }

    #endregion

    #region ����Grid

    public void AddRewardGrid(RankRewardGridData rd)
    {
        AssetCacheMgr.GetUIInstance("WeekRankDialogRewardItem.prefab", (prefab, id, go) =>
        {
            GameObject obj = (GameObject)go;

            obj.transform.Find("WeekRankDialogRewardItemFG").GetComponentsInChildren<UISprite>(true)[0].spriteName = rd.imgName;
            obj.transform.Find("WeekRankDialogRewardItemText").GetComponentsInChildren<UILabel>(true)[0].text = rd.text;

            obj.transform.parent = m_transRewardGridList;
            obj.transform.localPosition = new Vector3(m_listRewardGrid.Count * 215, 0, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            RewardGrid rg = obj.AddComponent<RewardGrid>();

            rg.Id = m_listRewardGrid.Count;
            m_listRewardGrid.Add(obj);
            MyDragCamera mdc = obj.AddComponent<MyDragCamera>();

            mdc.RelatedCamera = m_camRewardGrid;
            m_camRewardGrid.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX = 430 + 215 * (m_listRewardGrid.Count - 5);
        });
    }

    public void AddRankGrid(RankGridData rd, int index = 0)
    {
        //AssetCacheMgr.GetUIInstance("WeekRankDialogBodyGrid.prefab", (prefab, id, go) =>
        //{
        //    GameObject obj = (GameObject)go;

        //    obj.transform.parent = m_transRankGridList;
        //    obj.transform.localPosition = new Vector3(0, m_listRankGrid.Count * -60, 0);
        //    obj.transform.localScale = new Vector3(1, 1, 1);

        //    RankGrid rg = obj.AddComponent<RankGrid>();

        //    rg.Id = m_listRankGrid.Count;
        //    rg.Name = rd.name;
        //    rg.IsHighLight = rd.highLight;
        //    rg.Rank = rd.rank;
        //    rg.Achieve = rd.achieve;

        //    m_listRankGrid.Add(obj);

        //    MyDragCamera mdc = obj.AddComponent<MyDragCamera>();

        //    mdc.RelatedCamera = m_camRankGrid;
        //    m_camRankGrid.GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -120 + -60 * (m_listRankGrid.Count - 5);
        //});

        if (index > 4)
        {
            return;
        }
        m_arrRankPlayerGrid[index].SetActive(true);

        m_arrRankPlayerGrid[index].transform.Find(string.Concat("WeekRankDialogBodyGrid", index, "Achieve")).GetComponentsInChildren<UILabel>(true)[0].text = rd.achieve;
        m_arrRankPlayerGrid[index].transform.Find(string.Concat("WeekRankDialogBodyGrid", index, "Name")).GetComponentsInChildren<UILabel>(true)[0].text = rd.name;
        m_arrRankPlayerGrid[index].transform.Find(string.Concat("WeekRankDialogBodyGrid", index, "Rank")).GetComponentsInChildren<UILabel>(true)[0].text = rd.rank;
    }

    public void ClearRewardGridList()
    {
        int index = 0;

        for (int i = 0; i < m_listRewardGrid.Count; ++i)
        {
            index = i;

            AssetCacheMgr.ReleaseInstance(m_listRewardGrid[index]);
        }

        m_listRewardGrid.Clear();

        m_camRewardGrid.transform.localPosition = new Vector3(430, 0, 0);
    }

    public void SetRewardGridTipData(List<string> tipData, string title = "")
    {
        m_goRewardTip.GetComponentsInChildren<RewardGridTip>(true)[0].ListReward = tipData;
        m_goRewardTip.GetComponentsInChildren<RewardGridTip>(true)[0].TipTitle = title;
    }

    public void ShowRewardGridTip(bool isShow)
    {
        m_goRewardTip.SetActive(isShow);
        //m_goRewardTip.transform.position = new Vector3(x, m_goRewardTip.transform.position.y, m_goRewardTip.transform.position.z);  
    }

    public void ClearRankGridList()
    {
        //int index = 0;

        //for (int i = 0; i < m_listRankGrid.Count; ++i)
        //{
        //    index = i;

        //    AssetCacheMgr.ReleaseInstance(m_listRankGrid[index]);
        //}

        //m_listRankGrid.Clear();

        //m_camRankGrid.transform.localPosition = new Vector3(555, -120, 0);

        for (int i = 0; i < m_arrRankPlayerGrid.Length; ++i)
        {
            m_arrRankPlayerGrid[i].SetActive(false);
        }
    }

    #endregion

    #region �ҵĹ���Grid

    private List<MyRewardData> m_myRewardDataList = new List<MyRewardData>();

    /// <summary>
    /// ��������Grid
    /// </summary>
    /// <param name="data"></param>
    public void GenerateMyRewardList(List<MyRewardData> dataList)
    {
        m_myRewardDataList = dataList;
        GenerateMyRewardList(dataList.Count);
    }

    void GenerateMyRewardList(int size)
    {
        m_tranMyAchievementDialogRewardList.GetComponentsInChildren<MogoListImproved>(true)[0].SetGridLayout<SanUIMyRewardGrid>(size, m_tranMyAchievementDialogRewardList.transform, MyRewardListLoaded);
    }

    void MyRewardListLoaded()
    {
        var gridUIList = m_tranMyAchievementDialogRewardList.GetComponentsInChildren<MogoListImproved>(true)[0].DataList;
        for (int i = 0; i < gridUIList.Count && i < m_myRewardDataList.Count; i++)
        {
            SanUIMyRewardGrid gridUI = (SanUIMyRewardGrid)gridUIList[i];
            MyRewardData gridData = m_myRewardDataList[i];
            gridUI.LoadResourceInsteadOfAwake();
            gridUI.rewardID = gridData.rewardID;
            gridUI.SetName(gridData.name);
            gridUI.SetNum(gridData.needScore);
            gridUI.SetProgress(gridData.progress);
            gridUI.SetIcon(gridData.icon);
            gridUI.SetState(gridData.isEnable, gridData.isAlreadyGet);
        }
    }

    #endregion

    #region ����򿪺͹ر�

    protected override void OnEnable()
    {
        base.OnEnable();
        //if (m_myAchieveBtn != null)
         //   m_myAchieveBtn.GetComponentsInChildren<MogoButton>(true)[0].FakeClick();


    }

    #endregion
}
