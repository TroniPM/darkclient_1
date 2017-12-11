/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ClimbTowerUILogicManager
// 创建者：Charles Zuo
// 修改者列表：
// 创建日期：2013
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;
public class ClimbTowerUILogicManager
{
    #region 私有变量
    private static ClimbTowerUILogicManager m_instance;
    private ClimbTowerUIViewManager m_view;
    #endregion
    public static int FAILCOUNT = 99;
    private TowerData m_data = new TowerData();
    public TowerData Data
    {
        get
        {
            return m_data;
        }
        set
        {
            m_data = value;
            m_data.FailCount = FAILCOUNT - m_data.FailCount;
        }
    }

    #region 公共变量
    public static ClimbTowerUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ClimbTowerUILogicManager();
            }
            return ClimbTowerUILogicManager.m_instance;
        }
    }
    #endregion

    public void OnEnterMap()
    {
        LoggerHelper.Debug("OnEnterMap");
        EventDispatcher.TriggerEvent(Events.TowerEvent.EnterMap);
    }
    public void Charge()
    {
#if UNITY_IPHONE
        EventDispatcher.TriggerEvent(IAPEvents.ShowIAPView);
#else
        //MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(25570));
        EventDispatcher.TriggerEvent(Events.OperationEvent.Charge);
#endif
    } 
    public void OnRewardTooltips(int index)
    {
        LoggerHelper.Debug("OnRewardTooltips");
    }

    public void NormalSweep()
    {
        if (m_view.m_sweepReady)
        {
            int cost= 0;
            foreach(var item in TowerSweepCdClearCostData.dataMap.Values)
            {
                if(item.times == Data.NormalSweepUsed)
                {
                    cost = item.cost;
                }
            }
            if (cost>0)
            {
                MogoMessageBox.Confirm(LanguageData.GetContent(812, cost), (rst) =>
                {
                    if (rst)
                    {
                        EventDispatcher.TriggerEvent(Events.TowerEvent.ClearCD);
                        MogoGlobleUIManager.Instance.ConfirmHide();
                    }
                    else
                    {
                        MogoGlobleUIManager.Instance.ConfirmHide();
                    }

                });
            }
            else
            {
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap.Get(799).content);
            }
        }
        else 
        {
            LoggerHelper.Debug("not vip");
            EventDispatcher.TriggerEvent(Events.TowerEvent.NormalSweep);        
        }

        
        
    }
    public void VIPSweep()
    {
        if (MogoWorld.thePlayer.VipLevel >= GlobalData.dataMap[0].tower_all_sweep_vip_level)
        {
            EventDispatcher.TriggerEvent(Events.TowerEvent.SweepAll);
        }
        else
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.dataMap.Get(823).Format(10), (rst) =>
            {
                if (rst)
                {
                    EventDispatcher.TriggerEvent(Events.TowerEvent.VIPSweep);
                    MogoGlobleUIManager.Instance.ConfirmHide();
                }
                else
                {
                    MogoGlobleUIManager.Instance.ConfirmHide();
                }

            });
        }

    }
    public static class TowerEvent
    {
        public const string OpenTowerUI = "TowerEvent.OpenTowerUI";
    }
    public void Initialize(ClimbTowerUIViewManager ViewManager)
    {
        m_view = ViewManager;
    }

    public void Release()
    {
    }

    public void RefreshUI(TowerData data)
    {
        if (m_view!=null)
        {
            m_view.SetView(data);
        }
        else
        {
            //Debug.LogError("m_view null");
        }
        
    }
    public void OpenReport(ReportData data)
    {
        m_view.GenerateBattleReport(data);
        //m_view.ShowBattleReport(true);
    }

    public void SetTowerGridLayout(Action callback)
    {
        m_view.SetTowerGridLayout(callback);
    }
    public void ResourceLoaded()
    {
        m_view.ResourceLoaded();
    }
}
