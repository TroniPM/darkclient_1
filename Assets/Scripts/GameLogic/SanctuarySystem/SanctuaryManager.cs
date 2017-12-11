/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SanctuaryManager
// 创建者：Charles Zuo
// 修改者列表：
// 创建日期：
// 模块描述：圣域守卫战(世界boss)
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Mogo.GameData;
using Mogo.Game;
using Mogo.Util;

public class SanctuaryMyInfoData
{
    public List<int> alreadyGetList { get; set; }
}
public class SanctuaryRankData
{
    public int contribution { get; set; }
    public string name { get; set; }
}
public class SanctuaryRewardData
{
    public int exp { get; set; }
    public int gold { get; set; }
    public Dictionary<int, int> items { get; set; }
    public int contribution { get; set; }
}
public class SanctuaryManager : IEventManager
{
    private EntityMyself m_myself;

    public SanctuaryManager(EntityMyself _myself)
    {
        m_myself = _myself;
        AddListeners();
    }

    public void AddListeners()
    {
        EventDispatcher.AddEventListener(Events.SanctuaryEvent.RefreshRank, RefreshRank);
        EventDispatcher.AddEventListener(Events.SanctuaryEvent.RefreshMyInfo, RefreshMyInfo);
        EventDispatcher.AddEventListener(Events.SanctuaryEvent.EnterSanctuary, EnterSanctuary);
        EventDispatcher.AddEventListener(Events.SanctuaryEvent.BuyExtraTime, BuyExtraTime);
        EventDispatcher.AddEventListener(Events.SanctuaryEvent.CanBuyExtraTime, CanBuyExtraTime);
        EventDispatcher.AddEventListener(Events.SanctuaryEvent.QuerySanctuaryInfo, QuerySanctuaryInfo);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, OnInstanceLeave);
        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, OnInstanceEnter);
    }

    public void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener(Events.SanctuaryEvent.RefreshRank, RefreshRank);
        EventDispatcher.RemoveEventListener(Events.SanctuaryEvent.RefreshMyInfo, RefreshMyInfo);
        EventDispatcher.RemoveEventListener(Events.SanctuaryEvent.EnterSanctuary, EnterSanctuary);
        EventDispatcher.RemoveEventListener(Events.SanctuaryEvent.BuyExtraTime, BuyExtraTime);
        EventDispatcher.RemoveEventListener(Events.SanctuaryEvent.CanBuyExtraTime, CanBuyExtraTime);
        EventDispatcher.RemoveEventListener(Events.SanctuaryEvent.QuerySanctuaryInfo, QuerySanctuaryInfo);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, OnInstanceLeave);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, OnInstanceEnter);
    }

    private void OnInstanceLeave(int sceneID, bool isInstance)
    {
        if (MapData.dataMap.Get(sceneID).type == MapType.WORLDBOSS)
        {
            MainUIViewManager.Instance.SetHpBottleVisible(true);
            MainUIViewManager.Instance.ShowContributeRankDialog(false);
            MogoWorld.thePlayer.RemoveFx(6029);
        }
    }
    private void OnInstanceEnter(int sceneID, bool isInstance)
    {
        if (MapData.dataMap.Get(sceneID).type == MapType.WORLDBOSS)
        {
            MainUIViewManager.Instance.SetHpBottleVisible(false);
            MainUIViewManager.Instance.ShowContributeRankDialog(true);
            MainUIViewManager.Instance.SetContributeRankText(String.Format("当前贡献排名{0}", "："));
            MainUIViewManager.Instance.SetFirstRank(String.Empty);
            MainUIViewManager.Instance.SetSecRank(String.Empty);
            MainUIViewManager.Instance.SetTriRank(String.Empty);
            MogoWorld.thePlayer.PlayFx(6029);
        }
    }
    /// <summary>
    /// 刷新排行版数据
    /// </summary>
    private void RefreshRank()
    {
        m_myself.RpcCall("SanctuaryDefenseRankReq");   
    }

    /// <summary>
    /// 刷新面板个人信息
    /// </summary>
    private void RefreshMyInfo()
    {
        m_myself.RpcCall("SantuaryDefenseMyInfoReq");
    }

    /// <summary>
    /// 进入圣域守卫战
    /// </summary>
    private void EnterSanctuary()
    {
        m_myself.RpcCall("EnterSanctuaryDefenseReq");
    }

    /// <summary>
    /// 购买额外的进入次数
    /// </summary>
    private void BuyExtraTime()
    {
        m_myself.RpcCall("BuySanctuaryDefenseTimeReq");
    }

    /// <summary>
    /// 查询额外的进入次数
    /// </summary>
    private void CanBuyExtraTime()
    {
        m_myself.RpcCall("CanBuySanctuaryDefenseTimeReq");
    }
    /// <summary>
    /// 查询圣域守卫战的开启状态
    /// </summary>
    private void QuerySanctuaryInfo()
    {
        m_myself.RpcCall("SanctuaryDefenseTimeReq");
    }

}