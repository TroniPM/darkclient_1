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

public class ChooseDragonUIViewManager : MogoUIBehaviour 
{
    private static ChooseDragonUIViewManager m_instance;
    public static ChooseDragonUIViewManager Instance { get { return ChooseDragonUIViewManager.m_instance; } }

    private Transform m_tranChooseDragonUIGridList;

    private UILabel m_lblChooseDragonUIUpgradeChooseName;
    private UILabel m_lblChooseDragonUIUpgradeChooseNextTitle;
    private UILabel m_lblChooseDragonUIUpgradeChooseNextName;
    private UISprite m_spChooseDragonUIUpgradeNeedFG;
    private UILabel m_lblChooseDragonUIUpgradeNeedNum;    

    private UILabel m_lblChooseDragonUIRewardExp;
    private UILabel m_lblChooseDragonUIRewardGold;
    private UILabel m_lblChooseDragonUIRewardTime;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_tranChooseDragonUIGridList = FindTransform("ChooseDragonUIGridList");
        m_lblChooseDragonUIUpgradeChooseName = FindTransform("ChooseDragonUIUpgradeChooseName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblChooseDragonUIUpgradeChooseNextTitle = FindTransform("ChooseDragonUIUpgradeChooseNextTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_lblChooseDragonUIUpgradeChooseNextName = FindTransform("ChooseDragonUIUpgradeChooseNextName").GetComponentsInChildren<UILabel>(true)[0];
        m_spChooseDragonUIUpgradeNeedFG = FindTransform("ChooseDragonUIUpgradeNeedFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblChooseDragonUIUpgradeNeedNum = FindTransform("ChooseDragonUIUpgradeNeedNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblChooseDragonUIRewardExp = FindTransform("ChooseDragonUIRewardExp").GetComponentsInChildren<UILabel>(true)[0];
        m_lblChooseDragonUIRewardGold = FindTransform("ChooseDragonUIRewardGold").GetComponentsInChildren<UILabel>(true)[0];
        m_lblChooseDragonUIRewardTime = FindTransform("ChooseDragonUIRewardTime").GetComponentsInChildren<UILabel>(true)[0];

        Initialize();        
    }

    #region 事件

    public Action CHOOSEDRAGONUICLOSEUP;
    public Action CHOOSEDRAGONUIUPGRADEUP;
    public Action CHOOSEDRAGONUISTARTMATCHUP;

    public void Initialize()
    {
        FindTransform("ChooseDragonUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCloseUp;
        FindTransform("ChooseDragonUIBtnUpgrade").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnUpgradeUp;
        FindTransform("ChooseDragonUIBtnStartMatch").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnStartMatchUp;

        ChooseDragonUILogicManager.Instance.Initialize();
        m_uiLoginManager = ChooseDragonUILogicManager.Instance;

        SetUIGridList(5, () =>
        {
            SetUIGridDataList(m_listChooseDragonGridData);

        });       
    }

    public void Release()
    {
        ChooseDragonUILogicManager.Instance.Release();
    }

    void OnCloseUp()
    {
        if (CHOOSEDRAGONUICLOSEUP != null)
            CHOOSEDRAGONUICLOSEUP();
    }

    void OnUpgradeUp()
    {
        if (CHOOSEDRAGONUIUPGRADEUP != null)
            CHOOSEDRAGONUIUPGRADEUP();
    }

    void OnStartMatchUp()
    {
        if (CHOOSEDRAGONUISTARTMATCHUP != null)
            CHOOSEDRAGONUISTARTMATCHUP();
    }

    #endregion

    #region 设置界面信息

    /// <summary>
    /// 设置当前选择的飞龙名称,以及可提升的飞龙名称
    /// </summary>
    /// <param name="dragonName"></param>
    public void SetCurrentChooseDragon(string dragonName, string dragonNextName = "")
    {
        m_lblChooseDragonUIUpgradeChooseName.text = dragonName;

        if (string.IsNullOrEmpty(dragonNextName))
        {
            m_lblChooseDragonUIUpgradeChooseNextTitle.text = "";
            m_lblChooseDragonUIUpgradeChooseNextName.text = "";
        }
        else
        {
            m_lblChooseDragonUIUpgradeChooseNextTitle.text = string.Format(LanguageData.GetContent(48205), "");
            m_lblChooseDragonUIUpgradeChooseNextName.text = dragonNextName;
        }        
    }

    /// <summary>
    /// 设置飞龙比赛当前次数
    /// </summary>
    /// <param name="currentTime"></param>
    public void SetRewardCurrentTime(int currentTime)
    {
        m_lblChooseDragonUIRewardTime.text = string.Format(LanguageData.GetContent(48201), currentTime);
    }

    /// <summary>
    /// 设置可获得的经验
    /// </summary>
    /// <param name="exp"></param>
    public void SetRewardExp(int exp)
    {
        m_lblChooseDragonUIRewardExp.text = string.Concat("x", exp);
    }

    /// <summary>
    /// 设置可获得的金币
    /// </summary>
    /// <param name="gold"></param>
    public void SetRewardGold(int gold)
    {
        m_lblChooseDragonUIRewardGold.text = string.Concat("x", gold);
    }

    /// <summary>
    /// 设置提升需要的消耗
    /// </summary>
    /// <param name="needNum">需要的数量</param>
    /// <param name="currentNum">当前的数量</param>
    public void SetUpgradeNeedNum(string txt)
    {
        m_lblChooseDragonUIUpgradeNeedNum.text = txt;
    }
    
    /// <summary>
    /// 设置提升需要消耗的物品图标
    /// </summary>
    /// <param name="icon"></param>
    public void SetUpgradeNeedIcon(string icon)
    {
        MogoUIManager.Instance.TryingSetSpriteName(icon, m_spChooseDragonUIUpgradeNeedFG);
        //m_spChooseDragonUIUpgradeNeedFG.atlas = MogoUIManager.Instance.GetAtlasByIconName(icon);
        //m_spChooseDragonUIUpgradeNeedFG.spriteName = icon;
    }        

    #endregion

    #region 选择飞龙Grid

    readonly static float ITEMSPACEHORIZON = 220;
    private List<ChooseDragonUIGrid> m_listGrid = new List<ChooseDragonUIGrid>();

    private void SetUIGridList(int num, Action action = null)
    {    
        AddUIGridList(num, () =>
        {
            if (action != null)
                action();

            MogoGlobleUIManager.Instance.ShowWaitingTip(false); 
        });
    }

    private void AddUIGridList(int num, Action act = null)
    {     
        for (int i = 0; i < num; ++i)
        {
            int index = i;

            AssetCacheMgr.GetUIInstance("ChooseDragonUIGrid.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranChooseDragonUIGridList;
                obj.transform.localPosition = new Vector3(ITEMSPACEHORIZON * index, 0, 0);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                ChooseDragonUIGrid gridUI = obj.AddComponent<ChooseDragonUIGrid>();
                gridUI.LoadResourceInsteadOfAwake();
                m_listGrid.Add(gridUI);

                if (index == num - 1 && act != null)
                {
                    act();                    
                }
            });
        }
    }

    /// <summary>
    /// 设置选择飞龙信息数据
    /// </summary>
    /// <param name="m_listChooseDragonGridData"></param>
    private List<ChooseDragonGridData> m_listChooseDragonGridData = new List<ChooseDragonGridData>();
    public void SetUIGridDataList(List<ChooseDragonGridData> listChooseDragonGridData)
    {
        m_listChooseDragonGridData = listChooseDragonGridData;
        if (m_listGrid.Count == listChooseDragonGridData.Count)
        {
            SetUIGridDataList();
            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
        }
    }

    /// <summary>
    /// 设置选择飞龙界面信息
    /// </summary>
    private void SetUIGridDataList()
    {
        for (int index = 0; index < m_listChooseDragonGridData.Count; index++)
        {
            if (index < m_listGrid.Count)
            {
                ChooseDragonUIGrid gridUI = m_listGrid[index];
                ChooseDragonGridData gridData = m_listChooseDragonGridData[index];
                gridUI.DragonID = gridData.dragonID;
                gridUI.SetTitleAndImage(gridData.enable, gridData.dragonQuality);
                gridUI.SetFinishTime(gridData.finishTime, gridData.dragonQuality);
                gridUI.SetAdditionReward(gridData.additionReward, gridData.dragonQuality);
                gridUI.ShowBuy(gridData.showBuy);
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
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyChooseDragonUI();
        }
    }

    #endregion

}
