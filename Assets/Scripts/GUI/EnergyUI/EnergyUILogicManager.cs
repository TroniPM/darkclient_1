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
using System;
using System.Text;
using Mogo.Game;
using Mogo.GameData;

public class EnergyUILogicManager : UILogicManager
{
    private static EnergyUILogicManager m_instance;
    public static EnergyUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EnergyUILogicManager();                
            }

            return EnergyUILogicManager.m_instance;
        }
    }

    int m_BuyTimes = 0;    

    #region 事件
    public void Initialize()
    {
        EnergyUIViewManager.Instance.ENERGYUIBUYALLUP += OnEnergyBuyAllUp;
        EnergyUIViewManager.Instance.ENERGYUIBUYONEUP += OnEnergyBuyOneUp;
        EnergyUIViewManager.Instance.ENERGYUICHARGEUP += OnEnergyChargeUp;
        EnergyUIViewManager.Instance.ENERGYUICLOSEUP += OnEnergyCloseUp;

        EnergyUIViewManager.Instance.ENERGYUIUSEOK += OnEnergyUseOK;
        EnergyUIViewManager.Instance.ENERGYUIUSECANCEL += OnEnergyUseCancel;
        EnergyUIViewManager.Instance.ENERGYUIUSETIPENABLE += OnEnergyUseTipEnable;

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
        SetBinding<string>(EntityMyself.ATTR_ENERGYSTRING, EnergyUIViewManager.Instance.SetEnergy);
        SetBinding<byte>(EntityMyself.ATTR_VIP_LEVEL, EnergyUIViewManager.Instance.SetCurrentVipLevel);
        SetBinding<uint>(EntityMyself.ATTR_DIAMOND, EnergyUIViewManager.Instance.SetDiamondNum);
    }

    public override void Release()
    {       
        base.Release();
        EnergyUIViewManager.Instance.ENERGYUIBUYALLUP -= OnEnergyBuyAllUp;
        EnergyUIViewManager.Instance.ENERGYUIBUYONEUP -= OnEnergyBuyOneUp;
        EnergyUIViewManager.Instance.ENERGYUICHARGEUP -= OnEnergyChargeUp;
        EnergyUIViewManager.Instance.ENERGYUICLOSEUP -= OnEnergyCloseUp;

        EnergyUIViewManager.Instance.ENERGYUIUSEOK -= OnEnergyUseOK;
        EnergyUIViewManager.Instance.ENERGYUIUSECANCEL -= OnEnergyUseCancel;
        EnergyUIViewManager.Instance.ENERGYUIUSETIPENABLE -= OnEnergyUseTipEnable;
    }

    /// <summary>
    /// 全部购买
    /// </summary>
    void OnEnergyBuyAllUp()
    {
        m_BuyTimes = GetEnergyBuyLastTimes();
        EnergyBuy();
    }

    /// <summary>
    /// 一次购买
    /// </summary>
    void OnEnergyBuyOneUp()
    {
        if (GetEnergyBuyLastTimes() == 0)
            m_BuyTimes = 0;
        else
            m_BuyTimes = 1;
        EnergyBuy();
    }

    /// <summary>
    /// 购买体力
    /// </summary>
    void EnergyBuy()
    {
        if (m_BuyTimes == 0)
        {                
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[25103].content);// 提示达到最大购买次数
            return;
        }

        int price = MogoWorld.thePlayer.CalBuyEnergyDiamondCost(m_BuyTimes);
        if(price < 0)
            return;

        if (MogoWorld.thePlayer.diamond < price)
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[25105].content);// 提示钻石不足	
        }
        else
        {
            EnergyUIViewManager.Instance.SetDiamondCost(price);
            EnergyUIViewManager.Instance.SetRewardEnergy(m_BuyTimes);

            if (EnergyUIViewManager.Instance.IsShowBuyEnergyTipDialog)
            {
                EnergyUIViewManager.Instance.ShowUseOKCancel(true);
            }
            else
            {
                EventDispatcher.TriggerEvent<int>(Events.EnergyEvent.BuyEnergy, m_BuyTimes);
            }
           
        }  
    }

    /// <summary>
    /// 确认购买
    /// </summary>
    void OnEnergyUseOK()
    {
        EventDispatcher.TriggerEvent<int>(Events.EnergyEvent.BuyEnergy, m_BuyTimes);
        EnergyUIViewManager.Instance.ShowUseOKCancel(false);
    }

    /// <summary>
    /// 取消购买
    /// </summary>
    void OnEnergyUseCancel()
    {
        EnergyUIViewManager.Instance.ShowUseOKCancel(false);
    }

    /// <summary>
    /// 是否提示
    /// </summary>
    void OnEnergyUseTipEnable()
    {
        EnergyUIViewManager.Instance.IsShowBuyEnergyTipDialog = !EnergyUIViewManager.Instance.IsShowBuyEnergyTipDialog;

        if (EnergyUIViewManager.Instance.IsShowBuyEnergyTipDialog == false)
        {
            Mogo.Util.SystemConfig.Instance.IsShowBuyEnergyTipDialog = false;
            Mogo.Util.SystemConfig.Instance.BuyEnergyTipDialogDisableDay = MogoTime.Instance.GetCurrentDateTime().Day;
            Mogo.Util.SystemConfig.SaveConfig();
        }
    }

    /// <summary>
    /// 充值
    /// </summary>
    void OnEnergyChargeUp()
    {
#if UNITY_IPHONE
        EventDispatcher.TriggerEvent(IAPEvents.ShowIAPView);
#else
        //MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(25570));
        EventDispatcher.TriggerEvent(Events.OperationEvent.Charge);
#endif
    }

    void OnEnergyCloseUp()
    {
        LoggerHelper.Debug("OnEnergyCloseUp");
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    #endregion  

    public void SetVipRealState()
    {
		EnergyUIViewManager.Instance.SetVipRealState();
    }

    int GetEnergyBuyLastTimes()
    {
        int lastTimes = 0;
        if (PrivilegeData.dataMap.ContainsKey(MogoWorld.thePlayer.VipLevel))
        {
            int maxBuyTimes = PrivilegeData.dataMap[MogoWorld.thePlayer.VipLevel].dailyEnergyBuyLimit;
            int curBuyTimes = MogoWorld.thePlayer.m_VipRealStateMap[(int)VipRealStateEnum.DAILY_ENERGY_BUY_TIMES];
            lastTimes = maxBuyTimes - curBuyTimes;
        }

        return lastTimes;
    }
}
