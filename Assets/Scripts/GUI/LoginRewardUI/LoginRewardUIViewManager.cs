using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;

public class LoginRewardGridData
{
    public string NewServerRightText;
    public string OldServerRightText;
    public string LeftGetSignImgName;
    public string RightGetSignImgName;
    public string OldServerItemName;
    public string OldServerItemNum;
    public string OldServerItemCD;
    public string OldServerCostText;
    public string OldServerCostSignImgName;
    public int OldServerItemFGImgName;
    public bool IsOldServer;
    public List<KeyValuePair<int, int>> ListLeftItem;
    public List<string> ListRightItem;
    public List<int> ListLeftItemColor;
    public List<string> ListLeftItemBG;
    public List<int> ListLeftItemNum;

    public List<int> ListLeftItemID;
    public int OldServerItemFGImgID;
}

public class LoginRewardUIViewManager : MogoUIBehaviour
{
    private static LoginRewardUIViewManager m_instance;
    public static LoginRewardUIViewManager Instance { get { return LoginRewardUIViewManager.m_instance; } }
 
    public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    private Camera m_camLoginRewardGridList;
    private MyDragableCamera m_dragableCameraLoginRewardGridList;
    private GameObject m_goLoginRewardGridList;
    private UILabel m_uilLoginTitle;

    private List<GameObject> m_listLoginRewardGridGameObject = new List<GameObject>();
    private List<LoginRewardGrid> m_listLoginRewardGrid = new List<LoginRewardGrid>();
    public List<LoginRewardGrid> LoginRewardGridList
    {
        get { return m_listLoginRewardGrid; }
    }

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);
    
        m_camLoginRewardGridList = m_myTransform.Find(m_widgetToFullName["LoginRewardGridListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        m_dragableCameraLoginRewardGridList = m_camLoginRewardGridList.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_goLoginRewardGridList = m_myTransform.Find(m_widgetToFullName["LoginRewardGridList"]).gameObject;
        m_uilLoginTitle = m_myTransform.Find(m_widgetToFullName["LoginRewardDialogTitle"]).GetComponentsInChildren<UILabel>(true)[0];
        m_camLoginRewardGridList.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        // 提示箭头
        m_dragableCameraLoginRewardGridList.LeftArrow = FindTransform("LoginRewardUIArrowL").gameObject;
        m_dragableCameraLoginRewardGridList.RightArrow = FindTransform("LoginRewardUIArrowR").gameObject;

        Initialize();

        EventDispatcher.TriggerEvent(Events.OperationEvent.GetLoginMessage);
        // EventDispatcher.TriggerEvent(MarketEvent.DownloadLoginMarket);
    }

    #region 事件

    void OnBuyBtnUp(int id)
    {
        Mogo.Util.LoggerHelper.Debug("OnBuyBtnUp" + id);
        EventDispatcher.TriggerEvent(Events.OperationEvent.LogInBuy);
    }

    void OnGetBtnUp(int id)
    {
        EventDispatcher.TriggerEvent(Events.OperationEvent.LogInGetReward, id);
    }

    void Initialize()
    {
        LoginRewardUILogicManager.Instance.Initialize();

        m_dragableCameraLoginRewardGridList.MovePageDone += OnMovePageDone;
        EventDispatcher.AddEventListener<int>("LoginRewardUIBuyBtnUp", OnBuyBtnUp);
        EventDispatcher.AddEventListener<int>("LoginRewardUIGetBtnUp", OnGetBtnUp);        
    }

    public void Release()
    {
        ButtonTypeToEventUp.Clear();

        EmptyLoginRewardGridList();

        m_dragableCameraLoginRewardGridList.MovePageDone -= OnMovePageDone;
        EventDispatcher.RemoveEventListener<int>("LoginRewardUIBuyBtnUp", OnBuyBtnUp);
        EventDispatcher.RemoveEventListener<int>("LoginRewardUIGetBtnUp", OnGetBtnUp);

        LoginRewardUILogicManager.Instance.Release();
 
        m_listLoginRewardGrid.Clear();
        for (int i = 0; i < m_listLoginRewardGridGameObject.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listLoginRewardGridGameObject[i]);
            m_listLoginRewardGridGameObject[i] = null;
        }
        m_listLoginRewardGridGameObject.Clear();
    }

    #endregion

    #region 创建Grid

    static int GridIndex = 0;

    public bool IsFirstLoadRewardGrid = true;

    public void AddLoginRewardGrid(LoginRewardGridData ld, int theID = -1)
    {
        if (IsFirstLoadRewardGrid)
        {
            AssetCacheMgr.GetUIInstance("LoginRewardGrid.prefab", (prefab, id, go) =>
            {

                GameObject obj = (GameObject)go;
                LoginRewardGrid lg = obj.AddComponent<LoginRewardGrid>();
                obj.name = "LoginRewardGrid" + m_listLoginRewardGridGameObject.Count;

                if (theID == -1)
                    lg.Id = m_listLoginRewardGridGameObject.Count;
                else
                    lg.Id = theID;

                obj.transform.parent = m_goLoginRewardGridList.transform;
                obj.transform.localPosition = new Vector3((theID - 1) * 1150, 0, 0);
                obj.transform.localScale = new Vector3(1, 1, 1);

                GameObject trans = new GameObject();
                trans.transform.parent = m_goLoginRewardGridList.transform;
                trans.transform.localPosition = new Vector3(GridIndex * 1150, 0, 0);
                trans.transform.localEulerAngles = Vector3.zero;
                trans.transform.localScale = new Vector3(1, 1, 1);
                m_dragableCameraLoginRewardGridList.transformList.Add(trans.transform);

                m_listLoginRewardGrid.Add(lg);
                m_listLoginRewardGridGameObject.Add(obj);
                if (m_listLoginRewardGridGameObject.Count == LoginMessageGridCount)
                {
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                    ShowLoginRewardGrid();
                }

                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camLoginRewardGridList;

                for (int i = 0; i < 6; ++i)
                {
                    obj.transform.Find("LoginRewardGridLeft/LoginRewardGridLeftItemList/LoginRewardGridLeftItem" + i).GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camLoginRewardGridList;
                }

                GridIndex++;


                lg.LeftGetSign = ld.LeftGetSignImgName;
                lg.RightGetSign = ld.RightGetSignImgName;
                lg.NewServerRightText = ld.NewServerRightText;
                lg.OldServerCostSign = ld.OldServerCostSignImgName;
                lg.OldServerCostText = ld.OldServerCostText;
                lg.OldServerItemCD = ld.OldServerItemCD;
                lg.OldServerItemFG = ld.OldServerItemFGImgName;
                lg.OldServerItemName = ld.OldServerItemName;
                lg.OldServerItemNum = ld.OldServerItemNum;
                lg.OldServerRightText = ld.OldServerRightText;
                lg.ListLeftItem = ld.ListLeftItem;
                lg.ListRightItemID = ld.ListLeftItemID;
                lg.IsOldServer = ld.IsOldServer;
                lg.OldServerItemFGImgID = ld.OldServerItemFGImgID;

                EventDispatcher.TriggerEvent<int>("LoadLoginRewardGirdDone", lg.Id);
            });
        }
        else
        {
            //Debug.LogError("Not First");
            SetGridInfo(theID, ld);
        }
    }

    public void EmptyLoginRewardGridList()
    {
        return; // MaiFeo
        //为消除警告而注释以下代码
        //for (int i = 0; i < m_listLoginRewardGridGameObject.Count; ++i)
        //{
        //    int index = i;
        //    m_listLoginRewardGrid[index].Release();
        //    AssetCacheMgr.ReleaseInstance(m_listLoginRewardGridGameObject[index]);
        //}

        //GridIndex = 0;
        //m_listLoginRewardGrid.Clear();
        //m_listLoginRewardGridGameObject.Clear();

        //for (int i = 0; i < m_camLoginRewardGridList.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Count; ++i)
        //{
        //    Destroy(m_camLoginRewardGridList.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList[i].gameObject);
        //}

        //m_camLoginRewardGridList.GetComponentsInChildren<MyDragableCamera>(true)[0].transformList.Clear();
    }

    public void SetGridInfo(int gridID, LoginRewardGridData ld)
    {
        foreach (LoginRewardGrid lg in m_listLoginRewardGrid)
        {
            if (lg.Id == gridID)
            {
                lg.LeftGetSign = ld.LeftGetSignImgName;
                lg.RightGetSign = ld.RightGetSignImgName;
                lg.NewServerRightText = ld.NewServerRightText;
                lg.OldServerCostSign = ld.OldServerCostSignImgName;
                lg.OldServerCostText = ld.OldServerCostText;
                lg.OldServerItemCD = ld.OldServerItemCD;
                lg.OldServerItemFG = ld.OldServerItemFGImgName;
                lg.OldServerItemName = ld.OldServerItemName;
                lg.OldServerItemNum = ld.OldServerItemNum;
                lg.OldServerRightText = ld.OldServerRightText;
                lg.ListLeftItem = ld.ListLeftItem;
                break;
            }
        }
    }

    public int GetCurrentPage()
    {
        return m_dragableCameraLoginRewardGridList.GetCurrentPage();
    }

    public void SetCurrentPage(int page)
    {
        m_dragableCameraLoginRewardGridList.SetCurrentPage(page);
        ShowLoginRewardGrid();
    } 

    #endregion  

    #region 界面信息

    /// <summary>
    /// 播放"已领取""已购买"动画
    /// </summary>
    /// <param name="isLeft"></param>
    /// <param name="id"></param>
    public void PlayGetSignAnim(bool isLeft, int id)
    {
        if (isLeft)
        {
            m_listLoginRewardGridGameObject[id].GetComponentsInChildren<LoginRewardGrid>(true)[0].PlayLeftGetSignAnim();
        }
        else
        {
            //m_listLoginRewardGridGameObject[id].GetComponentsInChildren<LoginRewardGrid>(true)[0].PlayRightGetSignAnim();
            m_listLoginRewardGridGameObject[id].GetComponentsInChildren<LoginRewardGrid>(true)[0].PlayRightBuyAnim();
        }
    }   

    /// <summary>
    /// 设置每页登录奖励的Title
    /// </summary>
    /// <param name="day"></param>
    /// <param name="isCurrentDay"></param>
    public void SetLoginTitle(int day, bool isCurrentDay)
    {
        if (isCurrentDay)
            m_uilLoginTitle.text = string.Format(LanguageData.GetContent(47002), day); // "你已经连续登陆x天"
        else
            m_uilLoginTitle.text = string.Format(LanguageData.GetContent(47003), day); //  "第x天奖励"
    }

    #region  左侧：1.登录奖励Sign；2.领取按钮

    /// <summary>
    /// 是否显示左边的"已领取"
    /// </summary>
    /// <param name="theID"></param>
    /// <param name="isShow"></param>
    public void ShowLeftGetSign(int theID, bool isShow)
    {
        foreach (var item in m_listLoginRewardGrid)
        {
            if (item.Id == theID)
            {
                item.ShowLeftGetSign(isShow);
            }
            else if (item.Id < theID)
            {
                item.ShowLeftGetSign(true);
            }
        }
    }

    /// <summary>
    /// 是否显示领取按钮
    /// </summary>
    /// <param name="theID"></param>
    /// <param name="isShow"></param>
    public void ShowLoginGotButtom(int theID, bool isShow)
    {
        foreach (var item in m_listLoginRewardGrid)
        {
            if (item.Id == theID)
            {
                item.ShowGetBtn(isShow);
            }
            else
            {
                item.ShowGetBtn(false); // 非当天登录奖励不显示领取按钮
            }
        }

        // LoginRewardTip
        NormalMainUILogicManager.Instance.LoginRewardTip = isShow;
    }

    #endregion

    #region 右侧：1.限时购买Sign; 2.购买按钮；3.标题

    /// <summary>
    /// 限时购买标题
    /// </summary>
    /// <param name="theID"></param>
    public void ShowRightTitle(int theID)
    {
        foreach (var item in m_listLoginRewardGrid)
        {
            if (item.Id < theID)
            {
                item.OldServerRightText = LanguageData.GetContent(47005); // "已过期"
                item.SetOldServerRightTextColor(new Color32(193, 10, 1, 255));
            }
            else
            {
                item.OldServerRightText = LanguageData.GetContent(47006); // "登陆特惠限购"
                item.SetOldServerRightTextColor(new Color32(63, 27, 4, 255));
            }
        }
    }

    /// <summary>
    /// 是否显示购买按钮
    /// </summary>
    /// <param name="theID"></param>
    /// <param name="isShow"></param>
    public void ShowLoginBuyButtom(int theID, bool isShow)
    {
        foreach (var item in m_listLoginRewardGrid)
        {
            if (item.Id == theID)
            {
                item.ShowBuyBtn(isShow);
            }
            else
            {
                item.ShowBuyBtn(false);// 非当天登录购买不显示购买按钮
            }
        }
    }

    /// <summary>
    /// 是否显示左边的"已领取"
    /// </summary>
    /// <param name="theID"></param>
    /// <param name="isShow"></param>
    public void ShowRightBuySign(int theID, bool isShow)
    {
        foreach (var item in m_listLoginRewardGrid)
        {
            if (item.Id == theID)
            {
                item.ShowRightBuySign(isShow);
            }     
        }
    }

    #endregion

    #endregion

    #region 激活相邻的N个Grid，其他Active为false(未使用) 

    /// <summary>
    /// 翻页完成
    /// </summary>
    public void OnMovePageDone()
    {
        LoginRewardUILogicManager.Instance.SetTitleByJudgeDay(GetIndexID(GetCurrentPage()));
        ShowLoginRewardGrid();
    }

    public int GetIndexID(int page)
    {
        //return m_listLoginRewardGrid[page].Id;
        return page + 1;
    }

    /// <summary>
    /// 登陆奖励只显示当前页、上两页、下两页
    /// </summary>
    public void ShowLoginRewardGrid()
    {
        return;
        //为消除警告而注释以下代码

        // 下列注释代码目前没有使用(下列代码存在bug,应该根据Id来出来,而不是列表中的索引)
        //int currentPage = GetCurrentPage();
        //for (int i = 0; i < m_listLoginRewardGridGameObject.Count; i++)
        //{
        //    if (i >= currentPage - 2 && i <= currentPage + 2)
        //    {
        //        m_listLoginRewardGridGameObject[i].SetActive(true);
        //    }
        //    else
        //    {
        //        m_listLoginRewardGridGameObject[i].SetActive(false);
        //    }
        //}
    }

    // 登陆奖励的Grid数量
    private int m_loginMessageGridCount = -1;
    public int LoginMessageGridCount
    {
        get
        {
            if (m_loginMessageGridCount == -1)
                m_loginMessageGridCount = CalLoginMessageGridCount();

            return m_loginMessageGridCount;
        }
    }

    /// <summary>
    /// 计算登陆奖励的Grid数量
    /// </summary>
    /// <returns></returns>
    int CalLoginMessageGridCount()
    {
        int gridCount = 0;
        foreach (var data in RewardLoginData.dataMap)
        {
            if (data.Value.level.Count != 2)
            {
                continue;
            }

            if (MogoWorld.thePlayer.level >= data.Value.level[0] && MogoWorld.thePlayer.level <= data.Value.level[1])
            {
                gridCount++;
            }
        }

        return gridCount;
    }

    #endregion

    #region 界面打开和关闭

    void ReleasePreLoadResources()
    {
        AssetCacheMgr.ReleaseResourceImmediate("LoginRewardGrid.prefab");
    }

    public void ReleaseUIAndResources()
    {
        GridIndex = 0;
        ReleasePreLoadResources();
        MogoUIManager.Instance.DestroyLoginRewardUI();
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
