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

public class OccupyTowerUILogicManager : UILogicManager
{
    private static OccupyTowerUILogicManager m_instance;
    public static OccupyTowerUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new OccupyTowerUILogicManager();
            }

            return OccupyTowerUILogicManager.m_instance;
        }
    }

    #region 事件

    public void Initialize()
    {
        OccupyTowerUIViewManager.Instance.OCCUPYTOWERUICLOSEUP += OnCloseUp;
        OccupyTowerUIViewManager.Instance.OCCUPYTOWERUIJOINUP += OnJoinUp;

        EventDispatcher.AddEventListener(OccupyTowerUIDict.OccupyTowerUEvent.OnJoinCountDownEnd, OnJoinCountDownEnd);

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
    }

    public override void Release()
    {
        base.Release();

        OccupyTowerUIViewManager.Instance.OCCUPYTOWERUICLOSEUP -= OnCloseUp;
        OccupyTowerUIViewManager.Instance.OCCUPYTOWERUIJOINUP -= OnJoinUp;

        EventDispatcher.RemoveEventListener(OccupyTowerUIDict.OccupyTowerUEvent.OnJoinCountDownEnd, OnJoinCountDownEnd);
    }

    private void OnCloseUp()
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    private void OnJoinUp()
    {
        LoggerHelper.Debug("OnJoinUp");
        EventDispatcher.TriggerEvent(Events.OccupyTowerEvent.JoinOccupyTower);
    }

    public void JoinSuccess(int waitingNum)
    {
        OccupyTowerUIViewManager.Instance.SetOccupyTowerUIQueueNum(waitingNum);
        // OccupyTowerUIViewManager.Instance.SetOccupyTowerUIScoreNum(0);

        OccupyTowerUIViewManager.Instance.SetIsMatchNow(true);

        // OccupyTowerUIViewManager.Instance.SetBeginCountDown(true, 10.0f);
        // OccupyTowerUIViewManager.Instance.ShowMatchUICountDown(false);
    }

    private void OnJoinCountDownEnd()
    {
        LoggerHelper.Debug("OnJoinCountDownEnd");
        OccupyTowerUIViewManager.Instance.SetIsMatchNow(false);
    }

    public void EnterOccupyTower()
    {
        OccupyTowerUIViewManager.Instance.SetIsMatchNow(false);
        OccupyTowerUIViewManager.Instance.ShowMatchUI(false);
    }

    #endregion	
}
