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

public enum EnergyNoEnoughUITab
{
    NoneTab = -1,
    BuyEnergyTab = 0,
    LimitActivityTab = 1,
    OtherSystemTab = 2,
}

public class EnergyNoEnoughUIViewManager : MogoUIBehaviour 
{
    private static EnergyNoEnoughUIViewManager m_instance;
    public static EnergyNoEnoughUIViewManager Instance { get { return EnergyNoEnoughUIViewManager.m_instance; } }

    // 体力不足活动项
    private Transform m_tranUIGridlList;
    private Camera m_gridListCamera;
    private Vector3 m_camInitPos;
    private MyDragableCamera m_dragableCamera;

    private UILabel m_lblEnergyNoEnoughUIProgressBarText;
    private UILabel m_lblEnergyNoEnoughUITipText;
    private GameObject m_goEnergyNoEnoughUIProgressBar;
    private MogoSingleButtonList m_chooseWayMogoSingleButtonList;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranUIGridlList = FindTransform("EnergyNoEnoughUIGridList");
        m_gridListCamera = FindTransform("EnergyNoEnoughUIGridListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_camInitPos = m_gridListCamera.transform.localPosition;

        m_dragableCamera = m_gridListCamera.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragableCamera.LeftArrow = FindTransform("EnergyNoEnoughUIArrowUp").gameObject;
        m_dragableCamera.RightArrow = FindTransform("EnergyNoEnoughUIArrowDown").gameObject;

        m_lblEnergyNoEnoughUIProgressBarText = FindTransform("EnergyNoEnoughUIProgressBarText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblEnergyNoEnoughUITipText = FindTransform("EnergyNoEnoughUITipText").GetComponentsInChildren<UILabel>(true)[0];
        SetEnergyRecover();
        m_goEnergyNoEnoughUIProgressBar = FindTransform("EnergyNoEnoughUIProgressBar").gameObject;
        m_chooseWayMogoSingleButtonList = FindTransform("EnergyNoEnoughUIChooseWay").GetComponentsInChildren<MogoSingleButtonList>(true)[0];

        FindTransform("EnergyNoEnoughUIBtnClose").gameObject.AddComponent<EnergyNoEnoughUIButton>();
        FindTransform("ChooseWay1").gameObject.AddComponent<EnergyNoEnoughUIButton>();
        FindTransform("ChooseWay2").gameObject.AddComponent<EnergyNoEnoughUIButton>();
        FindTransform("ChooseWay3").gameObject.AddComponent<EnergyNoEnoughUIButton>();

        m_tabTitleLabelList[0] = FindTransform("ChooseWay1TextTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_tabTitleLabelList[1] = FindTransform("ChooseWay2TextTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_tabTitleLabelList[2] = FindTransform("ChooseWay3TextTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_tabDescLabelList[0] = FindTransform("ChooseWay1TextDesc").GetComponentsInChildren<UILabel>(true)[0];
        m_tabDescLabelList[1] = FindTransform("ChooseWay2TextDesc").GetComponentsInChildren<UILabel>(true)[0];
        m_tabDescLabelList[2] = FindTransform("ChooseWay3TextDesc").GetComponentsInChildren<UILabel>(true)[0];
        for (int i = 0; i <= 2; i++)
        {
             EnergyNoEnoughUITabUp(i);
        }
        CurrentDownTab = (int)EnergyNoEnoughUITab.BuyEnergyTab;

        Initialize();
    }

    #region 事件

    public Action ENERGYNOENOUGHUICLOSEUP;
    public Action<int> ENERGYNOENOUGHUICHOOSEWAY;

    public void Initialize()
    {
        EnergyNoEnoughUIDict.ButtonTypeToEventUp.Add("EnergyNoEnoughUIBtnClose", OnCloseUp);
        EnergyNoEnoughUIDict.ButtonTypeToEventUp.Add("ChooseWay1", OnChooseWay1);
        EnergyNoEnoughUIDict.ButtonTypeToEventUp.Add("ChooseWay2", OnChooseWay2);
        EnergyNoEnoughUIDict.ButtonTypeToEventUp.Add("ChooseWay3", OnChooseWay3);

        EnergyNoEnoughUILogicManager.Instance.Initialize();
        m_uiLoginManager = EnergyNoEnoughUILogicManager.Instance;
    }

    public void Release()
    {
        EnergyNoEnoughUILogicManager.Instance.Release();
        EnergyNoEnoughUIDict.ButtonTypeToEventUp.Clear();
    }

    void OnCloseUp(int i)
    {
        if (ENERGYNOENOUGHUICLOSEUP != null)
            ENERGYNOENOUGHUICLOSEUP();
    }

    void OnChooseWay1(int i)
    {
        if (ENERGYNOENOUGHUICHOOSEWAY != null)
            ENERGYNOENOUGHUICHOOSEWAY((int)EnergyNoEnoughUITab.BuyEnergyTab);
        CurrentDownTab = (int)EnergyNoEnoughUITab.BuyEnergyTab;
    }

    void OnChooseWay2(int i)
    {
        if (ENERGYNOENOUGHUICHOOSEWAY != null)
            ENERGYNOENOUGHUICHOOSEWAY((int)EnergyNoEnoughUITab.LimitActivityTab);
        CurrentDownTab = (int)EnergyNoEnoughUITab.LimitActivityTab;
    }

    void OnChooseWay3(int i)
    {
        if (ENERGYNOENOUGHUICHOOSEWAY != null)
            ENERGYNOENOUGHUICHOOSEWAY((int)EnergyNoEnoughUITab.OtherSystemTab);
        CurrentDownTab = (int)EnergyNoEnoughUITab.OtherSystemTab;
    }

    #endregion   

    #region 体力不足活动项

    readonly static float ITEMSPACEVERTICAL = -125;
    readonly static int GRID_COUNT_ONE_PAGE = 2;
    readonly static float OFFSET_Y = 0.0f;
    private Dictionary<int, EnergyNoEnoughUIGrid> m_mapGrid = new Dictionary<int, EnergyNoEnoughUIGrid>();

    /// <summary>
    /// 设置体力不足活动Grid
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
    /// 添加体力不足活动Grid
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
            AssetCacheMgr.GetUIInstance("EnergyNoEnoughUIGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranUIGridlList;
                obj.transform.localPosition = new Vector3(0, ITEMSPACEVERTICAL * index, 0);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_gridListCamera;
                EnergyNoEnoughUIGrid gridUI = obj.AddComponent<EnergyNoEnoughUIGrid>();

                if (m_mapGrid.ContainsKey(index))
                    AssetCacheMgr.ReleaseInstance(m_mapGrid[index].gameObject);
                m_mapGrid[index] = gridUI;

                // 滑动形式和翻页形式都需要设置
                //if (!m_dragableCamera.IsMovePage)
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
                    // 滑动形式需要处理(翻页不需要设置)
                    if (!m_dragableCamera.IsMovePage)
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
    /// 清除体力不足活动项
    /// </summary>
    private void ClearUIGridList()
    {
        Mogo.Util.LoggerHelper.Debug("ClearEnergyNoEnoughUIGridList " + m_mapGrid.Count);
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
    private void ResetGridListCameraPos()
    {
        m_gridListCamera.transform.localPosition = m_camInitPos;
    }

    /// <summary>
    /// 设置购买体力活动数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="imgName"></param>
    /// <param name="itemName"></param>
    public void SetBuyEnergyGridListData(List<BuyEnergyData> listBuyEnergyData)
    {
        for (int index = 0; index < listBuyEnergyData.Count; index++)
        {
            if (m_mapGrid.ContainsKey(index))
            {
                EnergyNoEnoughUIGrid gridUI = m_mapGrid[index];
                BuyEnergyData gridData = listBuyEnergyData[index];
                gridUI.LoadResourceInsteadOfAwake();
                gridUI.Index = index;
                gridUI.SetBuyEnergyDetail(gridData.title, gridData.icon, gridData.buttonName, gridData.diamond, gridData.addEnergy);
            }
        }
    }

    /// <summary>
    /// 限时活动
    /// </summary>
    /// <param name="listEnergyLimitActivityData"></param>
    public void SetLimitActivityGridListData(List<EnergyLimitActivityData> listEnergyLimitActivityData)
    {
        for (int index = 0; index < listEnergyLimitActivityData.Count; index++)
        {
            if (index < m_mapGrid.Count)
            {
                EnergyNoEnoughUIGrid gridUI = m_mapGrid[index];
                EnergyLimitActivityData gridData = listEnergyLimitActivityData[index];
                gridUI.LoadResourceInsteadOfAwake();
                gridUI.Index = index;
                gridUI.SetLimitActivityDetail(gridData.title, gridData.icon, gridData.buttonName, gridData.limitActivityDesc, gridData.finished);
            }
        }
    }

    /// <summary>
    /// 其他玩法
    /// </summary>
    /// <param name="listNoNeedEnergyData"></param>
    public void SetNoNeedEnergyGridListData(List<NoNeedEnergyData> listNoNeedEnergyData)
    {
        for (int index = 0; index < listNoNeedEnergyData.Count; index++)
        {
            if (index < m_mapGrid.Count)
            {
                EnergyNoEnoughUIGrid gridUI = m_mapGrid[index];
                NoNeedEnergyData gridData = listNoNeedEnergyData[index];
                gridUI.LoadResourceInsteadOfAwake();
                gridUI.Index = index;
                gridUI.SetNoNeedEnergyDetail(gridData.title, gridData.icon, gridData.buttonName, gridData.desc, gridData.requestLevel);
            }
        }
    }

    #endregion

    #region 界面信息

    /// <summary>
    /// 设置体力恢复
    /// </summary>
    public void SetEnergyRecover()
    {          
        EnergyData energyData = EnergyData.dataMap[1];
        m_lblEnergyNoEnoughUITipText.text = string.Format(LanguageData.GetContent(47805), energyData.recoverInterval, energyData.recoverPoints);
    }

    /// <summary>
    /// 设置体力
    /// </summary>
    /// <param name="energyPercentage"></param>
    public void SetEnergy(float energyPercentage)
    {
        m_lblEnergyNoEnoughUIProgressBarText.text = string.Concat(MogoWorld.thePlayer.energy, "/", MogoWorld.thePlayer.maxEnergy);
        SetProgressBar((float)MogoWorld.thePlayer.energy / MogoWorld.thePlayer.maxEnergy);
    }

    /// <summary>
    /// 设置体力
    /// </summary>
    /// <param name="energyPercentage"></param>
    public void SetEnergy(string energy)
    {
        m_lblEnergyNoEnoughUIProgressBarText.text = string.Concat(MogoWorld.thePlayer.energy, "/", MogoWorld.thePlayer.maxEnergy);
        SetProgressBar((float)MogoWorld.thePlayer.energy / MogoWorld.thePlayer.maxEnergy);
    }

    /// <summary>
    /// 设置进度条
    /// </summary>
    /// <param name="value"></param>
    private void SetProgressBar(float value)
    {
        m_goEnergyNoEnoughUIProgressBar.GetComponent<UISlider>().sliderValue = value;
    }

    #endregion

    #region  Tab文字颜色

    private Dictionary<int, UILabel> m_tabTitleLabelList = new Dictionary<int, UILabel>();
    private Dictionary<int, UILabel> m_tabDescLabelList = new Dictionary<int, UILabel>();
    private int m_iCurrentDownTab = (int)EnergyNoEnoughUITab.NoneTab;
    public int CurrentDownTab
    {
        get
        {
            return m_iCurrentDownTab;
        }
        set
        {
            HandleTabChange(m_iCurrentDownTab, value);
            m_iCurrentDownTab = value;
        }
    }

    public void HandleTabChange(int fromTab, int toTab)
    {     
        EnergyNoEnoughUITabUp(fromTab);
        EnergyNoEnoughUITabDown(toTab);
    }

    private void EnergyNoEnoughUITabUp(int tab)
    {
        if (m_tabTitleLabelList.ContainsKey(tab))
        {
            UILabel fromTabLabel = m_tabTitleLabelList[tab];
            if (fromTabLabel != null)
            {
                fromTabLabel.color = new Color32(37, 29, 6, 255);
                fromTabLabel.effectStyle = UILabel.Effect.None;
            }
        }

        if (m_tabDescLabelList.ContainsKey(tab))
        {
            UILabel fromTabLabel = m_tabDescLabelList[tab];
            if (fromTabLabel != null)
            {
                fromTabLabel.color = new Color32(37, 29, 6, 255);
                fromTabLabel.effectStyle = UILabel.Effect.None;
            }
        }
    }

    private void EnergyNoEnoughUITabDown(int tab)
    {
        if (m_tabTitleLabelList.ContainsKey(tab))
        {
            UILabel toTabLabel = m_tabTitleLabelList[tab];
            if (toTabLabel != null)
            {
                toTabLabel.color = new Color32(255, 255, 255, 255);
                toTabLabel.effectStyle = UILabel.Effect.Outline;
                toTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }

        if (m_tabDescLabelList.ContainsKey(tab))
        {
            UILabel toTabLabel = m_tabDescLabelList[tab];
            if (toTabLabel != null)
            {
                toTabLabel.color = new Color32(255, 255, 255, 255);
                toTabLabel.effectStyle = UILabel.Effect.Outline;
                toTabLabel.effectColor = new Color32(53, 22, 2, 255);
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
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        // 默认选中第一标签页
        m_chooseWayMogoSingleButtonList.SetCurrentDownButton(0);
        OnChooseWay1(0);
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyEnergyNoEnoughUI();
        }        
    }

    #endregion
}
