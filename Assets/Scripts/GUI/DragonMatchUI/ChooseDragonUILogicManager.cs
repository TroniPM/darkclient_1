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
using Mogo.Util;
using System.Collections.Generic;
using System;

public struct ChooseDragonGridData
{
    public int dragonID;
    public bool enable;
    public int dragonQuality;
    public string finishTime;
    public string additionReward;
    public bool showBuy;
}

public class ChooseDragonUILogicManager : UILogicManager
{
    private static ChooseDragonUILogicManager m_instance;
    public static ChooseDragonUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ChooseDragonUILogicManager();
            }

            return ChooseDragonUILogicManager.m_instance;
        }
    }

    #region 事件

    public static Action<int> CHOOSEDRAGONUIGRIDBUY;

    public void Initialize()
    {
        ChooseDragonUIViewManager.Instance.CHOOSEDRAGONUICLOSEUP += OnCloseUp;
        ChooseDragonUIViewManager.Instance.CHOOSEDRAGONUIUPGRADEUP += OnUpgradeUp;
        ChooseDragonUIViewManager.Instance.CHOOSEDRAGONUISTARTMATCHUP += OnStartMatchUp;
        CHOOSEDRAGONUIGRIDBUY += OnChooseDragonBtnBuyUp;

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
    }

    public override void Release()
    {
        base.Release();
        ChooseDragonUIViewManager.Instance.CHOOSEDRAGONUICLOSEUP -= OnCloseUp;
        ChooseDragonUIViewManager.Instance.CHOOSEDRAGONUIUPGRADEUP -= OnUpgradeUp;
        ChooseDragonUIViewManager.Instance.CHOOSEDRAGONUISTARTMATCHUP -= OnStartMatchUp;
        CHOOSEDRAGONUIGRIDBUY -= OnChooseDragonBtnBuyUp;
    }

    void OnCloseUp()
    {
        LoggerHelper.Debug("OnChooseDragonUICloseUp");
        MogoUIManager.Instance.ShowChooseDragonUI(null, false);
        DragonMatchManager.Instance.ShowMainUI();
    }

    void OnUpgradeUp()
    {
        LoggerHelper.Debug("OnUpgradeUp");
        DragonMatchManager.Instance.OnUpgradeDragon();
    }

    void OnStartMatchUp()
    {
        LoggerHelper.Debug("OnStartMatchUp");
        DragonMatchManager.Instance.OnDoStartMatch();
    }

    /// <summary>
    /// 点击购买飞龙
    /// </summary>
    /// <param name="id">飞龙ID</param>
    void OnChooseDragonBtnBuyUp(int id)
    {
        Debug.LogError("OnChooseDragonBtnBuyUp:" + id);
        DragonMatchManager.Instance.OnBuyDragon(id);
    }

    #endregion

    #region 飞龙选择数据

    private List<ChooseDragonGridData> m_listChooseDragonGridData = new List<ChooseDragonGridData>();

    public void SetChooseDragonGridDataList(List<ChooseDragonGridData> listChooseDragonGridData)
    {
        m_listChooseDragonGridData.Clear();
        m_listChooseDragonGridData = listChooseDragonGridData;


        ChooseDragonUIViewManager.Instance.SetUIGridDataList(m_listChooseDragonGridData);
    }

    #endregion
}
