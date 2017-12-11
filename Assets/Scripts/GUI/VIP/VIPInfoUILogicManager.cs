/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������VIPInfoUILogicManager
// �����ߣ�Charles Zuo
// �޸����б�
// �������ڣ�2013
// ģ��������
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.Game;

public class VIPInfoUILogicManager : UILogicManager
{
    #region ˽�б���
    private static VIPInfoUILogicManager m_instance;
    private VIPInfoUIViewManager m_view;
    #endregion

    #region ��������
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

        // ���԰�
        ItemSource = MogoWorld.thePlayer; // ����ĿǰLogic���ܻ����UI�رն�Release�����ڴ���������ItemSource
        SetBinding<byte>(EntityMyself.ATTR_VIP_LEVEL, m_view.SetViewData);
        SetBinding<uint>(EntityMyself.ATTR_CHARGE_SUM, m_view.SetPlayerChargeSum);
    }
    public override void Release()
    {
        base.Release();
    }


}
