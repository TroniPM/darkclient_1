using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Game;
using Mogo.Util;
using Mogo.GameData;

public class RewardManager
{
    private EntityMyself m_self;
    private List<RewardRechargeData> rrds;
    private int currSelect = -1;
    private List<int> chargedReward = new List<int>();
    private int currStep = -1;

    public RewardManager(EntityMyself self)
    {
        m_self = self;
        AddListeners();
    }

    private void AddListeners()
    {
        EventDispatcher.AddEventListener(Events.RewardEvent.OpenReward, OpenReward);
        EventDispatcher.AddEventListener(Events.RewardEvent.WingIcon, WingIcon);
        EventDispatcher.AddEventListener(Events.RewardEvent.ChargeReward, ChargeReward);
        EventDispatcher.AddEventListener(Events.RewardEvent.ElfDiamond, ElfDiamond);
        EventDispatcher.AddEventListener(Events.RewardEvent.LoginReward, LoginReward);
        EventDispatcher.AddEventListener<int>(Events.RewardEvent.GetLoginReward, GetLoginReward);
        EventDispatcher.AddEventListener(Events.RewardEvent.GetChargeReward, GetChargeReward);
        EventDispatcher.AddEventListener<int>(Events.RewardEvent.SelectReward, SelectReward);
    }

    private void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener(Events.RewardEvent.OpenReward, OpenReward);
        EventDispatcher.RemoveEventListener(Events.RewardEvent.WingIcon, WingIcon);
        EventDispatcher.RemoveEventListener(Events.RewardEvent.ChargeReward, ChargeReward);
        EventDispatcher.RemoveEventListener(Events.RewardEvent.ElfDiamond, ElfDiamond);
        EventDispatcher.RemoveEventListener(Events.RewardEvent.LoginReward, LoginReward);
        EventDispatcher.RemoveEventListener<int>(Events.RewardEvent.GetLoginReward, GetLoginReward);
        EventDispatcher.RemoveEventListener(Events.RewardEvent.GetChargeReward, GetChargeReward);
        EventDispatcher.RemoveEventListener<int>(Events.RewardEvent.SelectReward, SelectReward);
    }

    public void Clean()
    {
        RemoveListeners();
    }

    private void OpenReward()
    {
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);

        RewardUILogicManager.Instance.SetUIDirty();
    }

    public void UpdateDoneChargeReward(LuaTable charged)
    {
        List<int> temp = new List<int>();
        foreach (var i in charged)
        {
            temp.Add(int.Parse((string)i.Key));
        }
        chargedReward = temp;
        UpdateChargeRewardView();
    }

    private void WingIcon()
    {
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
        RewardUILogicManager.Instance.SetUIDirty();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.ChargeReturnWingUI, MFUIManager.MFUIID.None, 0, true);
        //ChargeReturnWingUILogicManager.Instance.SetTopWingIcon("");
        ChargeReturnWingUILogicManager.Instance.SetUIDirty();
    }

    private void ChargeReward()
    {
        m_self.RpcCall("get_done_recharge");
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
        RewardUILogicManager.Instance.SetUIDirty();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.NewChargeRewardUI, MFUIManager.MFUIID.None, 0, true);
        UpdateChargeRewardView();
    }

    private void UpdateChargeRewardView()
    {
        initrrd();
        if (m_self.chargeSum > 0)
        {
            List<string> prices = new List<string>();
            int curr = 0;
            for (int i = 0; i < rrds.Count; i++)
            {
                if (m_self.chargeSum >= rrds[i].money)
                {
                    curr = i;
                    NewChargeRewardUILogicManager.Instance.SetProgressSize(500);
                }
                if (chargedReward.Contains(rrds[i].id))
                {
                    prices.Add(LanguageData.GetContent(7802));
                }
                else
                {
                    prices.Add(rrds[i].money + "");
                }
            }
            currStep = curr;
            int size = CalChargePrg(curr, rrds[curr]);
            NewChargeRewardUILogicManager.Instance.SetProgressSize(size);
            NewChargeRewardUILogicManager.Instance.AddChargePriceItem(prices);
            NewChargeRewardUILogicManager.Instance.ShowAsCharged(true);
            var items = GetChargeReward(rrds[curr]);
            List<int> ids = new List<int>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (ItemParentData.GetItem(item.Key) != null)
                    {
                        ids.Add(item.Key);
                    }
                }
                NewChargeRewardUILogicManager.Instance.SetItemList(ids);
            }
            //TimerHeap.AddTimer(500, 0, UpdateChargeBtnSt);
            //NewChargeRewardUILogicManager.Instance.ShowGetBtn(true);
        }
        else
        {
            NewChargeRewardUILogicManager.Instance.ShowAsCharged(false);
            var items = GetDefaultChargeReward();
            List<int> ids = new List<int>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (ItemParentData.GetItem(item.Key) != null)
                    {
                        ids.Add(item.Key);
                    }
                }
                NewChargeRewardUILogicManager.Instance.SetItemList(ids);
            }
        }
        NewChargeRewardUILogicManager.Instance.SetUIDirty();
    }

    private int CalChargePrg(int step, RewardRechargeData data)
    {
        int rst = 0;
        int head = 48; //头长度48
        //int end = 54; //尾长度1/2中间长度
        int mid = 108; //中间长度108
        int currSum = step + 1;
        int moneymore = (int)(m_self.chargeSum - (uint)rrds[step].money);
        int fragmoney = 0;
        float pct = 0;
        if (currSum < rrds.Count)
        {
            fragmoney = rrds[currSum].money - rrds[step].money;
            pct = (float)moneymore / (float)fragmoney;
        }
        else
        {
            pct = 0;
        }
        if (step == 0 && m_self.chargeSum <= rrds[step].money)
        {
            if (m_self.chargeSum == rrds[step].money)
            {
                rst = head;
            }
            else
            {
                rst = (int)((float)head * pct);
            }
        }
        else
        {
            rst = head + step * mid + (int)((float)mid * pct);
        }
        return rst;
    }

    private void UpdateChargeBtnSt()
    {
        for (int i = 0; i < rrds.Count; i++)
        {
            if (i > currStep)
            {
                NewChargeRewardUILogicManager.Instance.ShowProgressGridIconHighLight(false, i);
            }
            else
            {
                NewChargeRewardUILogicManager.Instance.ShowProgressGridIconHighLight(true, i);
            }
        }
    }

    private void initrrd()
    {
        if (rrds == null)
        {
            rrds = new List<RewardRechargeData>();
            foreach (var item in RewardRechargeData.dataMap)
            {
                rrds.Add(item.Value);
            }
            rrds.Sort(rrdSort);
        }
    }

    private int rrdSort(RewardRechargeData a, RewardRechargeData b)
    {
        if (a.money == b.money)
        {
            return 0;
        }
        if (a.money > b.money)
        {
            return 1;
        }
        return -1;
    }

    private Dictionary<int, int> GetChargeReward(RewardRechargeData rrd)
    {
        switch (m_self.vocation)
        {
            case Vocation.Warrior:
                return rrd.items1;
            case Vocation.Assassin:
                return rrd.items2;
            case Vocation.Archer:
                return rrd.items3;
            case Vocation.Mage:
                return rrd.items4;
            default:
                return null;
        }
    }

    protected Dictionary<int, int> GetDefaultChargeReward()
    {
        switch (m_self.vocation)
        {
            case Vocation.Warrior:
                return RewardRechargeData.dataMap[1].items1;
            case Vocation.Assassin:
                return RewardRechargeData.dataMap[1].items2;
            case Vocation.Archer:
                return RewardRechargeData.dataMap[1].items3;
            case Vocation.Mage:
                return RewardRechargeData.dataMap[1].items4;
            default:
                return null;
        }
    }

    private void ElfDiamond()
    {
        m_self.RpcCall("DiamondMineInfoReq");
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
        RewardUILogicManager.Instance.SetUIDirty();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.ElfDiamondUI, MFUIManager.MFUIID.None, 0, true);
        ElfDiamondUILogicManager.Instance.SetCostDiamondNum("x 500");
        ElfDiamondUILogicManager.Instance.SetDiamondNumCanGet("1");
        ElfDiamondUILogicManager.Instance.SetTotalCostDiamondNum(LanguageData.GetContent(7808, 666));
        ElfDiamondUILogicManager.Instance.SetTotalGotDiamondNum(LanguageData.GetContent(7809, 777));
        ElfDiamondUILogicManager.Instance.SetUIDirty();
    }

    private void LoginReward()
    {
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
        RewardUILogicManager.Instance.SetUIDirty();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.NewLoginRewardUI, MFUIManager.MFUIID.None, 0, true);
        List<NewLoginRewardGridData> rst = new List<NewLoginRewardGridData>();
        foreach (var item in RewardLoginData.dataMap)
        {
            if (item.Value.level.Count != 2)
            {
                LoggerHelper.Debug("RewardLoginData level wrong, id: " + item.Key);
                continue;
            }
            if (m_self.level < item.Value.level[0] || m_self.level > item.Value.level[1])
            {
                continue;
            }
            NewLoginRewardGridData d = new NewLoginRewardGridData();
            d.strDays = LanguageData.GetContent(47003, item.Value.days);
            switch(MogoWorld.thePlayer.vocation)
            {
                case Vocation.Warrior:
                    foreach(var i in item.Value.items1)
                    {
                        d.listItemID.Add(i.Key);
                        d.listItemNum.Add(i.Value);
                    }
                    break;
                case Vocation.Assassin:
                    foreach (var i in item.Value.items2)
                    {
                        d.listItemID.Add(i.Key);
                        d.listItemNum.Add(i.Value);
                    }
                    break;
                case Vocation.Archer:
                    foreach (var i in item.Value.items3)
                    {
                        d.listItemID.Add(i.Key);
                        d.listItemNum.Add(i.Value);
                    }
                    break;
                case Vocation.Mage:
                    foreach (var i in item.Value.items4)
                    {
                        d.listItemID.Add(i.Key);
                        d.listItemNum.Add(i.Value);
                    }
                    break;
            }
            if (m_self.login_days < item.Value.days)
            {
                d.IsGot = false;
                d.IsSendToMailBox = false;
            }
            else if (m_self.login_days == item.Value.days)
            {
                d.IsGot = m_self.IsLoginRewardHasGot;
                d.IsSendToMailBox = false;
            }
            else
            {
                d.IsGot = true;
                d.IsSendToMailBox = true;
            }
            rst.Add(d);
        }
        NewLoginRewardUILogicManager.Instance.RefreshGridList(rst);
        NewLoginRewardUILogicManager.Instance.SetUIDirty();
    }

    private void GetLoginReward(int idx)
    {
        m_self.RpcCall("get_reward_login");
    }

    private void GetChargeReward()
    {
        if (currSelect == -1)
        {
            return;
        }
        m_self.RpcCall("get_reward_recharge", rrds[currSelect].id);
    }

    private void SelectReward(int idx)
    {
        initrrd();
        var items = GetChargeReward(rrds[idx]);
        List<int> ids = new List<int>();
        if (items != null)
        {
            foreach (var item in items)
            {
                if (ItemParentData.GetItem(item.Key) != null)
                {
                    ids.Add(item.Key);
                }
            }
            NewChargeRewardUILogicManager.Instance.SetItemList(ids);
        }
        if (chargedReward.Contains(rrds[idx].id))
        {
            NewChargeRewardUILogicManager.Instance.ShowGetBtn(false);
        }
        else
        {
            NewChargeRewardUILogicManager.Instance.ShowGetBtn(true);
        }
        if (idx > currStep)
        {
            NewChargeRewardUILogicManager.Instance.ShowGetBtn(false);
        }
        currSelect = idx;
    }
}

