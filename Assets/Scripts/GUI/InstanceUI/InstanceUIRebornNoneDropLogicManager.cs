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
using Mogo.GameData;
using System.Collections.Generic;
using System;
using Mogo.Mission;

public class InstanceUIRebornNoneDropLogicManager {

	private static InstanceUIRebornNoneDropLogicManager m_instance;

    public static InstanceUIRebornNoneDropLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new InstanceUIRebornNoneDropLogicManager();
            }

            return InstanceUIRebornNoneDropLogicManager.m_instance;

        }
    }

    void OnRebornUp()
    {
        LoggerHelper.Debug("Reborn Up");
        EventDispatcher.TriggerEvent(Events.InstanceEvent.Reborn);
    }
    void OnRebornLeftUp()
    {
        LoggerHelper.Debug("RebornLeft");
        MainUIViewManager.Instance.ShowBossTarget(false);
        EventDispatcher.TriggerEvent(Events.InstanceEvent.NotReborn);
    }

    public void Initialize()
    {
        InstanceUIRebornNoneDropViewManager.Instance.INSTANCEREBORNUP += OnRebornUp;
        InstanceUIRebornNoneDropViewManager.Instance.INSTANCEREBORNLEFTUP += OnRebornLeftUp;
    }

    public void Release()
    {
        InstanceUIRebornNoneDropViewManager.Instance.INSTANCEREBORNUP -= OnRebornUp;
        InstanceUIRebornNoneDropViewManager.Instance.INSTANCEREBORNLEFTUP -= OnRebornLeftUp;      
    }   
}
