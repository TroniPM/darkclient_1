/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VIPInfoUILogicManager
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
using Mogo.Game;

public class VIPInfoUILogicManager : UILogicManager
{
    #region 私有变量
    private static VIPInfoUILogicManager m_instance;
    private VIPInfoUIViewManager m_view;
    #endregion

    #region 公共变量
    public static VIPInfoUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new VIPInfoUILogicManager();                
            }
            return VIPInfoUILogicManager.m_instance;
        }
    }
    #endregion


    public void Initialize(VIPInfoUIViewManager ViewManager)
    {
        m_view = ViewManager;
        LoggerHelper.Debug("InitializeInitialize");

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
        SetBinding<byte>(EntityMyself.ATTR_VIP_LEVEL, m_view.SetViewData);
        SetBinding<uint>(EntityMyself.ATTR_CHARGE_SUM, m_view.SetPlayerChargeSum);
    }
    public override void Release()
    {
        base.Release();
    }


}
