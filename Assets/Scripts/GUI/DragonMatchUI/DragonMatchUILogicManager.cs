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
using Mogo.Game;

public class DragonMatchPlayerInfo
{
    public string level;
    public string name;
    public string power;
    public string tong;
    public string dragon;
    public string status;
    public string hitTimes;
    public string hitObtainGold;
    public string hitObtainExp;
}

public class DragonMatchUILogicManager : UILogicManager 
{
    private static DragonMatchUILogicManager m_instance;
    public static DragonMatchUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DragonMatchUILogicManager();                
            }

            return DragonMatchUILogicManager.m_instance;
        }
    }

    #region 事件

    public void Initialize()
    {
        DragonMatchUIViewManager.Instance.DRAGONMATCHUICLOSEUP += OnCloseUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUISTARTMATCHUP += OnStartMatchUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIRECORDUP += OnRecordUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIEXPLOREUP += OnExploreUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIREFRESHUP += OnRefreshUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIHITTIMEADDUP += OnHitTimesAddUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIROUNDTIMEADDUP += OnRoundTimesAddUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIHITCDCLEARUP += OnHitCDClearUp;
		DragonMatchUIViewManager.Instance.DRAGONMATCHUIBeginTreasureUp += OnBeginTreasureUp;
		DragonMatchUIViewManager.Instance.DRAGONMATCHUIBeginRMB1UP += OnBeginRMB1UP;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIBeginRMB2UP += OnBeginRMB2UP;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIEndTreasureUp += OnEndTreasureUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIPLAYERUP += OnPlayerUp;

        DragonMatchUIViewManager.Instance.PLAYERINFOUIHITUP += OnPlayerInfoUIHit;
        DragonMatchUIViewManager.Instance.EXPLOREUIExploreUP += OnExploreUIExploreUp;
        DragonMatchUIViewManager.Instance.BEGINTREASUREUICLOSEUP += OnBeginTreasureUICloseUp;

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
        SetBinding<byte>(EntityMyself.ATTR_LEVEL, DragonMatchUIViewManager.Instance.SetPlayerLevel);
        SetBinding<uint>(EntityMyself.ATTR_DIAMOND, DragonMatchUIViewManager.Instance.SetPlayerDiamond);
        SetBinding<byte>(EntityMyself.ATTR_VIP_LEVEL, DragonMatchUIViewManager.Instance.SetPlayerVipLevel);
        SetBinding<uint>(EntityParent.ATTR_FIGHT_FORCE, DragonMatchUIViewManager.Instance.SetPlayerPower);
    }

    public override void Release()
    {
        base.Release();
        DragonMatchUIViewManager.Instance.DRAGONMATCHUICLOSEUP -= OnCloseUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUISTARTMATCHUP -= OnStartMatchUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIRECORDUP -= OnRecordUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIEXPLOREUP -= OnExploreUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIREFRESHUP -= OnRefreshUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIHITTIMEADDUP -= OnHitTimesAddUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIROUNDTIMEADDUP -= OnRoundTimesAddUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIHITCDCLEARUP -= OnHitCDClearUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIBeginTreasureUp -= OnBeginTreasureUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIBeginRMB1UP -= OnBeginRMB1UP;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIBeginRMB2UP -= OnBeginRMB2UP;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIEndTreasureUp -= OnEndTreasureUp;
        DragonMatchUIViewManager.Instance.DRAGONMATCHUIPLAYERUP -= OnPlayerUp;

        DragonMatchUIViewManager.Instance.PLAYERINFOUIHITUP -= OnPlayerInfoUIHit;
        DragonMatchUIViewManager.Instance.EXPLOREUIExploreUP -= OnExploreUIExploreUp;
        DragonMatchUIViewManager.Instance.BEGINTREASUREUICLOSEUP -= OnBeginTreasureUICloseUp;
    }

    /// <summary>
    /// 点击关闭按钮
    /// </summary>
    void OnCloseUp()
    {
        LoggerHelper.Debug("OnDragonMatchUICloseUp");
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    /// <summary>
    /// 点击开始比赛按钮
    /// </summary>
    void OnStartMatchUp()
    {
        LoggerHelper.Debug("OnStartMatchUp");
        //MogoUIManager.Instance.ShowChooseDragonUI(null, true);
        DragonMatchManager.Instance.OnStartMatch();
    }

    /// <summary>
    /// 点击袭击事件按钮
    /// </summary>
    void OnRecordUp()
    {
        LoggerHelper.Debug("OnRecordUp");
       
        DragonMatchManager.Instance.OnShowRecord();

    }

    /// <summary>
    /// 点击探索按钮
    /// </summary>
    void OnExploreUp()
    {
        LoggerHelper.Debug("OnExploreUp");
        DragonMatchManager.Instance.OnSearch();
        
    }

    /// <summary>
    /// 点击刷新按钮
    /// </summary>
    void OnRefreshUp()
    {
        LoggerHelper.Debug("OnRefreshUp");
        DragonMatchManager.Instance.OnRefreshOtherPlayers();
    }

    /// <summary>
    /// 点击增加袭击次数按钮
    /// </summary>
    void OnHitTimesAddUp()
    {
        LoggerHelper.Debug("OnHitTimesAddUp");
        DragonMatchManager.Instance.OnHitTimesAddUp();
    }

    /// <summary>
    /// 点击增加环数按钮
    /// </summary>
    void OnRoundTimesAddUp()
    {
        LoggerHelper.Debug("OnRoundTimesAddUp");
    }

    /// <summary>
    /// 点击消除袭击CD按钮
    /// </summary>
    void OnHitCDClearUp()
    {
        LoggerHelper.Debug("OnHitCDClearUp");
        DragonMatchManager.Instance.OnHitCDClearUp();
    }

    /// <summary>
    /// 点击宝箱按钮
    /// </summary>
    void OnBeginTreasureUp()
    {
        LoggerHelper.Debug("OnBeginTreasureUp");
        DragonMatchManager.Instance.OnBeginTreasureUp();
        DragonMatchUIViewManager.Instance.ShowDragonMatchBeginTreasureUI(true);
    }

    /// <summary>
    /// 宝箱奖励界面点击关闭
    /// </summary>
    void OnBeginTreasureUICloseUp()
    {
        LoggerHelper.Debug("OnBeginTreasureUICloseUp");
        DragonMatchUIViewManager.Instance.ShowDragonMatchBeginTreasureUI(false);
    }


    /// <summary>
    /// 点击比赛RMB功能1按钮
    /// </summary>
    void OnBeginRMB1UP()
    {
        LoggerHelper.Debug("OnBeginRMB1UP");
        DragonMatchManager.Instance.OnBuffClick(0);
    }

    /// <summary>
    /// 点击比赛RMB功能2按钮
    /// </summary>
    void OnBeginRMB2UP()
    {
        LoggerHelper.Debug("OnBeginRMB2UP");
        DragonMatchManager.Instance.OnBuffClick(1);
    }

    /// <summary>
    /// 比赛结束后点击宝箱领取奖励
    /// </summary>
    void OnEndTreasureUp()
    {
        LoggerHelper.Debug("OnEndTreasureUp");
        DragonMatchManager.Instance.OnEndTreasureUp();
    }

    /// <summary>
    /// 点击界面上的玩家
    /// </summary>
    /// <param name="player"></param>
    void OnPlayerUp(int player)
    {
        LoggerHelper.Debug("OnPlayerUp");
        DragonMatchManager.Instance.OnSelectOtherPlayer(player);
        
    }

    /// <summary>
    /// 玩家界面上点击袭击按钮
    /// </summary>
    void OnPlayerInfoUIHit()
    {
        LoggerHelper.Debug("OnPlayerInfoUIHit");
        DragonMatchManager.Instance.OnAttackOtherPlayer();
    }

    /// <summary>
    /// 探索界面上点击探索按钮
    /// </summary>
    void OnExploreUIExploreUp()
    {
        LoggerHelper.Debug("OnExploreUIExploreUp");
        DragonMatchManager.Instance.OnDoSearch();
    }
  
    #endregion
}
