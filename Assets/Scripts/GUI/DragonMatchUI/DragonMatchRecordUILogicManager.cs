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

// 飞龙比赛记录
public struct DragonMatchRecordGridData
{
    public string info;  // 文本信息
    public bool hasRevenged; // 是否已经复仇成功
    public bool canRevenge; // 是否可以复仇
    public bool needTip; // 是否需要提示
}

public class DragonMatchRecordUILogicManager : UILogicManager
{
    private static DragonMatchRecordUILogicManager m_instance;
    public static DragonMatchRecordUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DragonMatchRecordUILogicManager();
            }

            return DragonMatchRecordUILogicManager.m_instance;
        }
    }

    #region 事件

    public static class DragonMatchRecordUIEvent
    {
        public const string RevengeBtnUp = "RevengeBtnUp";
    }

    public void Initialize()
    {
        DragonMatchRecordUIViewManager.Instance.DRAGONMATCHRECORDUICLOSEUP += OnCloseUp;
        EventDispatcher.AddEventListener<int>(DragonMatchRecordUIEvent.RevengeBtnUp, OnRevengeBtnUp);
    }

    public override void Release()
    {
        base.Release();
        DragonMatchRecordUIViewManager.Instance.DRAGONMATCHRECORDUICLOSEUP -= OnCloseUp;
        EventDispatcher.RemoveEventListener<int>(DragonMatchRecordUIEvent.RevengeBtnUp, OnRevengeBtnUp);
    }

    void OnCloseUp()
    {
        LoggerHelper.Debug("OnDragonMatchRecordUICloseUp");
        MogoUIManager.Instance.ShowDragonMatchUI();
    }

    void OnRevengeBtnUp(int index)
    {
        DragonMatchManager.Instance.OnRevenge(index);
    }

    #endregion
}
