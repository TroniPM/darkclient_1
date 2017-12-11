/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：InstanceUILogicManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;
using System.Collections.Generic;
using System;
using Mogo.Mission;

public class InstanceUIRebornLogicManager
{


    private static InstanceUIRebornLogicManager m_instance;

    public static InstanceUIRebornLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new InstanceUIRebornLogicManager();
            }

            return InstanceUIRebornLogicManager.m_instance;

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
        InstanceUIRebornViewManager.Instance.INSTANCEREBORNUP += OnRebornUp;
        InstanceUIRebornViewManager.Instance.INSTANCEREBORNLEFTUP += OnRebornLeftUp;
    }

    public void Release()
    {
        InstanceUIRebornViewManager.Instance.INSTANCEREBORNUP -= OnRebornUp;
        InstanceUIRebornViewManager.Instance.INSTANCEREBORNLEFTUP -= OnRebornLeftUp;
      
    }


    public void SetInstanceRebornUIReward(List<string> iconPaths, List<string> itemNames)
    {
        InstanceUIRebornViewManager.Instance.AddRebornItem(iconPaths.Count, SetInstanceRebornUIGridRewardMessage, iconPaths, itemNames);
    }

    public void SetInstanceRebornUIGridRewardMessage(List<string> iconPaths, List<string> itemNames)
    {
        for (int i = 0; i < iconPaths.Count; i++)
        {
            InstanceUIRebornViewManager.Instance.SetRebornItemData(i, iconPaths[i], itemNames[i]);
        }
    }

}