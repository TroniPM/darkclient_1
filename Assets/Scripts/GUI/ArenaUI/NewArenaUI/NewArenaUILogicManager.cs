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
using Mogo.Game;
using Mogo.Util;
using Mogo.GameData;
using System;

public class NewArenaUILogicManager : UILogicManager 
{
    private static NewArenaUILogicManager m_instance;
    public static NewArenaUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new NewArenaUILogicManager();
            }

            return NewArenaUILogicManager.m_instance;
        }
    }

    private ArenaUIEnemyTab m_currentEnemyTab = ArenaUIEnemyTab.Weak;
    public ArenaUIEnemyTab CurrentEnemyTab
    {
        get { return m_currentEnemyTab; }
        set
        {
            m_currentEnemyTab = value;
        }
    }

    #region 事件

    public void Initialize()
    {
        NewArenaUIViewManager.Instance.ARENAUICLOSEUP += OnCloseUp;
        NewArenaUIViewManager.Instance.ARENAUIREWARDUP += OnBtnRewardUp;
        NewArenaUIViewManager.Instance.ARENAUIADDTIMESUP += OnBtnAddTimesUp;
        NewArenaUIViewManager.Instance.ARENAUICLEARCDUP += OnBtnClearCDUp;
        NewArenaUIViewManager.Instance.ARENAUIENTERUP += OnBtnEnterUp;
        NewArenaUIViewManager.Instance.ARENAUIREFRESHUP += OnBtnRefreshUp;

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource

        // 设置钻石数量
        SetBinding<uint>(EntityMyself.ATTR_DIAMOND, NewArenaUIViewManager.Instance.SetDiamondNum);
        // 设置金币数量
        SetBinding<uint>(EntityMyself.ATTR_GLOD, NewArenaUIViewManager.Instance.SetGoldNum);
        SetBinding<string>(EntityMyself.ATTR_NAME, NewArenaUIViewManager.Instance.SetPlyNameText);
        SetBinding<uint>(EntityMyself.ATTR_ARENIC_CREDIT, NewArenaUIViewManager.Instance.SetPlayerMedalProgressValue);
        SetBinding<ushort>(EntityMyself.ATTR_ARENIC_GRADE, NewArenaUIViewManager.Instance.SetPlayerMedalTitle);
    }

    public override void Release()
    {
        base.Release();
        NewArenaUIViewManager.Instance.ARENAUICLOSEUP -= OnCloseUp;        
    }

    /// <summary>
    /// 关闭Btn
    /// </summary>
    void OnCloseUp()
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    /// <summary>
    /// 积分奖励Btn
    /// </summary>
    void OnBtnRewardUp()
    {
        MogoUIManager.Instance.OpenWindow((int)WindowName.ArenaReward,
           () =>
           {
               EventDispatcher.TriggerEvent(Events.ArenaEvent.GetArenaRewardInfo);
           },  null,  true);
    }

    /// <summary>
    /// 增加挑战次数Btn
    /// </summary>
    void OnBtnAddTimesUp()
    {
        MogoMessageBox.Confirm(LanguageData.GetContent(24002,
             PriceListData.dataMap.
             Get(8).priceList[1]),
             (flag) =>
             {
                 if (flag)
                     EventDispatcher.TriggerEvent(Events.ArenaEvent.AddArenaTimes);
             });
    }

    /// <summary>
    /// 清除CDBtn
    /// </summary>
    void OnBtnClearCDUp()
    {
        MogoMessageBox.Confirm(LanguageData.GetContent(812,
            PriceListData.dataMap.
            Get(5).priceList[1] * Math.Ceiling(TimerManager.GetTimer(NewArenaUIViewManager.Instance.m_lblArenaUIChallengeCDNum.gameObject).GetSeconds() / 60.0f)),
            (flag) =>
            {
                if (flag)
                    EventDispatcher.TriggerEvent(Events.ArenaEvent.ClearArenaCD);
            });
    }

    /// <summary>
    /// 挑战Btn
    /// </summary>
    void OnBtnEnterUp()
    {
        EventDispatcher.TriggerEvent<int>(Events.ArenaEvent.Challenge, (int)CurrentEnemyTab);
    }

    /// <summary>
    /// 刷新
    /// </summary>
    void OnBtnRefreshUp()
    {

    }

    #endregion

    #region 逻辑

    /// <summary>
    /// 是否显示领取奖励提示
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowRewardNotice(bool isShow)
    {
        if (NewArenaUIViewManager.Instance != null)
        {
            NewArenaUIViewManager.Instance.ShowRewardNotice(isShow);
        }
    }

    #endregion
}
