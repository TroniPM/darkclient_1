/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MarketSystem
// 创建者：Hooke HU
// 修改者列表：
// 创建日期：2013-4-2
// 模块描述：翅膀管理器
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Game;
using Mogo.Util;
using Mogo.GameData;

public class Wing
{
    public int id;
    public int level;
    public int exp;
    public int active;
}

class WingProperty
{
    public int hpBase;
    public float hit;
    public float crit;
    public int critExtraAttack;
    public float trueStrike;
    public int antiDefense;

    public int defenseBase;
    public int earthDefense;
    public int airDefense;
    public int waterDefense;
    public int fireDefense;
    public int allElementsDefense;

    public int attackBase;
    public int earthDamage;
    public int airDamage;
    public int waterDamage;
    public int fireDamage;
    public int allElementsDamage;

    public int pvpAddition;
    public int pvpAnti;

    public float hpAddRate;
    public int attackAddRate;
    public int speedAddRate;
    public int damageReduceRate;
    public int extraHitRate;
    public int extraCritRate;
    public int extraTrueStrikeRate;
    public int extraExpRate;
    public int extraGoldRate;

    public int antiCrit;
    public int antiTrueStrike;
    public int cdReduce;

    public void AddProperty(PropertyEffectData data)
    {
        hpBase += data.hpBase;
        hit += data.hit;
        crit += data.crit;
        critExtraAttack += data.critExtraAttack;
        trueStrike += data.trueStrike;
        antiDefense += data.antiDefense;

        defenseBase += data.defenseBase;
        earthDefense += data.earthDefense;
        airDefense += data.airDefense;
        waterDefense += data.waterDefense;
        fireDefense += data.fireDefense;
        allElementsDefense += data.allElementsDefense;

        attackBase += data.attackBase;
        earthDamage += data.earthDamage;
        airDamage += data.airDamage;
        waterDamage += data.waterDamage;
        fireDamage += fireDamage;
        allElementsDamage += allElementsDamage;

        pvpAddition += data.pvpAddition;
        pvpAnti += data.pvpAnti;

        hpAddRate += data.hpAddRate;
        attackAddRate += data.attackAddRate;
        speedAddRate += data.speedAddRate;
        damageReduceRate += data.damageReduceRate;
        extraHitRate += data.extraHitRate;
        extraCritRate += data.extraCritRate;
        extraTrueStrikeRate += data.extraTrueStrikeRate;
        extraExpRate += data.extraExpRate;
        extraGoldRate += data.extraGoldRate;

        antiCrit += data.antiCrit;
        antiTrueStrike += data.antiTrueStrike;
        cdReduce += data.cdReduce;
    }

    public List<string> GetPropertEffect()
    {
        List<string> rst = new List<string>();
        if (hpBase > 0) rst.Add(LanguageData.dataMap[26813].Format(hpBase));
        if (attackBase > 0) rst.Add(LanguageData.dataMap[26814].Format(attackBase));
        if (defenseBase > 0) rst.Add(LanguageData.dataMap[26815].Format(defenseBase));
        if (crit > 0) rst.Add(LanguageData.dataMap[26816].Format(crit));
        if (trueStrike > 0) rst.Add(LanguageData.dataMap[26817].Format(trueStrike));
        if (critExtraAttack > 0) rst.Add(LanguageData.dataMap[26818].Format(critExtraAttack));
        if (antiDefense > 0) rst.Add(LanguageData.dataMap[26819].Format(antiDefense));
        if (antiCrit > 0) rst.Add(LanguageData.dataMap[26820].Format(antiCrit));
        if (antiTrueStrike > 0) rst.Add(LanguageData.dataMap[26821].Format(antiTrueStrike));
        if (extraCritRate > 0) rst.Add(LanguageData.dataMap[26822].Format(extraCritRate));
        if (extraTrueStrikeRate > 0) rst.Add(LanguageData.dataMap[26823].Format(extraTrueStrikeRate));
        if (damageReduceRate > 0) rst.Add(LanguageData.dataMap[26824].Format(damageReduceRate));
        if (pvpAddition > 0) rst.Add(LanguageData.dataMap[26825].Format(pvpAddition));
        if (pvpAnti > 0) rst.Add(LanguageData.dataMap[26826].Format(pvpAnti));
        return rst;
    }
}

public class WingManager
{
    private EntityMyself m_self;
    private Dictionary<int, Wing> m_wings = new Dictionary<int, Wing>();
    private List<WingGridData> m_grids = new List<WingGridData>();
    private List<WingGridData> m_common = new List<WingGridData>();
    private List<WingGridData> m_magic = new List<WingGridData>();
    private WingData tipData = null;
    private int page = 0; //0为common 1为magic
    private int dressed = -1;
    private bool opened = false;

    public WingManager(EntityMyself owner)
    {
        m_self = owner;
        AddListeners();
        //UpdateWing(m_self.wingBag);
    }

    private void AddListeners()
    {
        EventDispatcher.AddEventListener(Events.WingEvent.Open, Open);
        EventDispatcher.AddEventListener(Events.WingEvent.Close, Close);
        EventDispatcher.AddEventListener<int>(Events.WingEvent.Buy, Buy);
        EventDispatcher.AddEventListener<int>(Events.WingEvent.Active, Active);
        EventDispatcher.AddEventListener<int>(Events.WingEvent.PutOn, PutOn);
        EventDispatcher.AddEventListener<int>(Events.WingEvent.Undo, Undo);
        EventDispatcher.AddEventListener<int>(Events.WingEvent.UnLock, UnLock);
        EventDispatcher.AddEventListener<int>(Events.WingEvent.Upgrade, Upgrade);
        EventDispatcher.AddEventListener(Events.WingEvent.CommonWing, CommonWing);
        EventDispatcher.AddEventListener(Events.WingEvent.MagicWing, MagicWing);
        EventDispatcher.AddEventListener<int>(Events.WingEvent.OpenTip, OpenTip);
        EventDispatcher.AddEventListener<int, int>(Events.WingEvent.OpenBuy, OpenBuy);
        EventDispatcher.AddEventListener<int>(Events.WingEvent.OpenUpgrade, OpenUpgrade);
        EventDispatcher.AddEventListener(Events.WingEvent.TipBuyClick, TipBuyClick);
    }

    private void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener(Events.WingEvent.Open, Open);
        EventDispatcher.RemoveEventListener(Events.WingEvent.Close, Close);
        EventDispatcher.RemoveEventListener<int>(Events.WingEvent.Buy, Buy);
        EventDispatcher.RemoveEventListener<int>(Events.WingEvent.Active, Active);
        EventDispatcher.RemoveEventListener<int>(Events.WingEvent.PutOn, PutOn);
        EventDispatcher.RemoveEventListener<int>(Events.WingEvent.Undo, Undo);
        EventDispatcher.RemoveEventListener<int>(Events.WingEvent.UnLock, UnLock);
        EventDispatcher.RemoveEventListener<int>(Events.WingEvent.Upgrade, Upgrade);
        EventDispatcher.RemoveEventListener(Events.WingEvent.CommonWing, CommonWing);
        EventDispatcher.RemoveEventListener(Events.WingEvent.MagicWing, MagicWing);
        EventDispatcher.RemoveEventListener<int>(Events.WingEvent.OpenTip, OpenTip);
        EventDispatcher.RemoveEventListener<int, int>(Events.WingEvent.OpenBuy, OpenBuy);
        EventDispatcher.RemoveEventListener<int>(Events.WingEvent.OpenUpgrade, OpenUpgrade);
        EventDispatcher.RemoveEventListener(Events.WingEvent.TipBuyClick, TipBuyClick);
    }

    public void Clean()
    {
        RemoveListeners();
    }

    public void UpdateWing(LuaTable wings)
    {
        List<WingGridData> datas = new List<WingGridData>();
        m_wings.Clear();
        m_grids.Clear();
        m_magic.Clear();
        m_common.Clear();
        int dressed = int.Parse((string)wings["1"]);
        this.dressed = dressed;
        foreach (var item in (LuaTable)wings["2"])
        {
            Wing w = new Wing();
            w.id = int.Parse(item.Key);
            w.level = int.Parse((string)((LuaTable)item.Value)["1"]);
            w.exp = int.Parse((string)((LuaTable)item.Value)["2"]);
            w.active = int.Parse((string)((LuaTable)item.Value)["3"]);
            m_wings.Add(w.id, w);
        }
        foreach (var item in WingData.dataMap)
        {
            WingGridData d = new WingGridData();
            d.wingId = item.Key;
            d.WingName = LanguageData.GetContent(item.Value.name);
            d.WingDescripe = LanguageData.GetContent(item.Value.descrip);
            d.WingIconName = IconData.GetIconPath(item.Value.icon);
            if (m_wings.ContainsKey(item.Key))
            {
                d.WingCurExp = m_wings[item.Key].exp;
                d.WingStarNum = 3;
                d.WingTotalExp = item.Value.GetLevelData(m_wings[item.Key].level).nextLevelExp;
                d.IsHave = true;
                if (item.Value.type == 1)
                {
                    d.WingStatus = "";
                }
                else
                {
                    if (m_wings[item.Key].active == 1)
                    {
                        d.WingStatus = LanguageData.GetContent(26829);
                        d.IsActive = true;
                    }
                    else
                    {
                        d.WingStatus = LanguageData.GetContent(26828);
                        d.IsActive = false;
                    }
                }
                d.WingPrice = "";
            }
            else
            {
                d.WingPrice = "";
                d.WingStarNum = 0;
                d.WingCurExp = 0;
                d.WingTotalExp = 0;
                if (item.Value.type == 1)
                {
                    d.WingStatus = LanguageData.GetContent(26827);
                    d.WingPrice = "" + item.Value.price;
                }
                else
                {
                    d.WingStatus = LanguageData.GetContent(26827);
                }
                d.IsHave = false;
                d.IsActive = false;
                d.IsUsing = false;
            }
            if (item.Value.type == 1)
            {
                if (d.wingId == dressed)
                {
                    d.IsUsing = d.IsHave;
                }
                d.IsActive = d.IsHave;
                m_common.Add(d);
            }
            else
            {
                d.IsHave = true;
                d.IsUsing = true;
                m_magic.Add(d);
            }
            datas.Add(d);
        }
        m_grids = datas;
        CommonWing();
        WingUILogicManager.Instance.SetGold("" + m_self.gold);
        WingUILogicManager.Instance.SetDiamond("" + m_self.diamond);
        UpdateProperty();
        if(opened)
            UpdateUpgradeContent();
    }

    private void UpdateProperty()
    {
        WingProperty wp = new WingProperty();
        foreach (var item in m_wings)
        {
            WingData d = WingData.dataMap.Get(item.Value.id);
            WingLevelData wld = d.GetLevelData(item.Value.level);
            PropertyEffectData p = wld.GetProperEffectData();
            wp.AddProperty(p);
        }
        List<string> attrs = wp.GetPropertEffect();
        WingUILogicManager.Instance.SetWingAttribute(attrs);
    }

    private void Open()
    {
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.WingUI);
        WingUILogicManager.Instance.SetUIDirty();
        UpdateWing(m_self.wingBag);
        opened = true;
    }

    private void Close()
    {
        opened = false;
        MogoUIManager.Instance.ShowMogoNormalMainUI();
        m_self.ResetPreviewWing();
        m_self.UpdateDressWing();
    }

    private void CommonWing()
    {
        page = 0;
        WingUILogicManager.Instance.RefreshWingGridList(m_common);
    }

    private void MagicWing()
    {
        page = 1;
        WingUILogicManager.Instance.RefreshWingGridList(m_magic);
    }

    private void TipBuyClick()
    {
        if (!m_wings.ContainsKey(tipData.id))
        {
            Mogo.Util.EventDispatcher.TriggerEvent(MarketEvent.OpenWithWing);
        }
        else
        {
            PutOn(tipData.id);
        }
    }

    private void OpenTip(int idx)
    {
        WingGridData d = m_grids[idx];
        if (page == 0)
        {
            d = m_common[idx];
        }
        else if (page == 1)
        {
            d = m_magic[idx];
        }
        m_self.PreviewWing(d.wingId);
        WingData wd = WingData.dataMap.Get(d.wingId);
        tipData = wd;
        WingLevelData wld = wd.GetLevelData(1);
        WingLevelData wnld = wd.GetLevelData(2);
        if (m_wings.ContainsKey(wd.id))
        {
            wld = wd.GetLevelData(m_wings[wd.id].level);
            wnld = wd.GetLevelData(m_wings[wd.id].level + 1);
        }
        if (m_wings.ContainsKey(d.wingId))
        {
            WingUIViewManager.Instance.ShowTipUpgradeBtn(wd.type == 1);
            if (wd.type == 1)
            {
                WingUIViewManager.Instance.ShowTipBuyBtn(d.wingId != dressed);
            }
            else
            {
                WingUIViewManager.Instance.ShowTipBuyBtn(d.wingId != dressed && d.IsActive);
            }
            WingUIViewManager.Instance.SetBuyText(LanguageData.GetContent(47281));
        }
        else
        {
            WingUIViewManager.Instance.ShowTipUpgradeBtn(false);
            WingUIViewManager.Instance.ShowTipBuyBtn(false); //wd.type == 1);
            WingUIViewManager.Instance.SetBuyText(LanguageData.GetContent(1026));
        }
        WingUIViewManager.Instance.SetWingTipTitle(LanguageData.GetContent(wd.name));
        List<string> attrs = wld.GetPropertEffect();
        PropertyEffectData a = wld.GetProperEffectData();
        List<string> nextAttrs = null;
        if (wnld != null)
        {
            nextAttrs = wnld.GetPropertEffect();
        }
        for (int i = 0; i < attrs.Count; i++)
        {
            WingUIViewManager.Instance.SetWingTipAttrDescripe("", i);
            WingUIViewManager.Instance.SetWingTipCurAttr(attrs[i], i);
            if (nextAttrs != null)
            {
                WingUIViewManager.Instance.SetWingTipNextAttr(nextAttrs[i], i);
            }
            else
            {
                WingUIViewManager.Instance.SetWingTipNextAttr("", i);
            }
        }
    }

    private void OpenBuy(int id, int diamond)
    {
        int iid = ItemEffectData.dataMap[ItemMaterialData.dataMap[id].effectId].reward1[13];//13是reward1中的翅膀类型
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.WingUIBuyDialog, MFUIManager.MFUIID.None, 0, true);
        WingData d = WingData.dataMap.Get(iid);
        WingLevelData ld = d.GetLevelData(1);
        WingUIBuyDialogLogicManager.Instance.SetBuyDialodTitle(LanguageData.GetContent(d.name));
        List<string> attrs = ld.GetPropertEffect();
        for (int i = 0; i < attrs.Count; i++)
        {
            WingUIBuyDialogLogicManager.Instance.SetBuyDialogAttr(attrs[i], i);
        }
        WingUIBuyDialogLogicManager.Instance.SetBuyDialogCost(" " + diamond);
        WingUIBuyDialogLogicManager.Instance.SetBuyDialogIcon(IconData.dataMap.Get(ItemMaterialData.dataMap[id].icon).path);
        WingUIBuyDialogLogicManager.Instance.SetUIDirty();
    }

    private void OpenUpgrade(int idx)
    {
        UpdateUpgradeContent();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.WingUIUpgradeDialog, MFUIManager.MFUIID.None, 0, true);
    }

    private void UpdateUpgradeContent()
    {
        WingData wd = tipData;
        WingLevelData wld = wd.GetLevelData(m_wings[wd.id].level);
        WingLevelData wnld = wd.GetLevelData(m_wings[wd.id].level + 1);
        WingUIViewManager.Instance.SetWingTipTitle(LanguageData.GetContent(wd.name));
        List<string> attrs = wld.GetPropertEffect();
        List<string> nextAttrs = null;
        if (wnld != null)
        {
            nextAttrs = wnld.GetPropertEffect();
            WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogCost("x" + wnld.trainDiamondCost);
            WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogNextLevel(LanguageData.GetContent(200005, wnld.level));
        }
        WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogCurrentLevel(LanguageData.GetContent(200005, wld.level));
        WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogIcon(IconData.GetIconPath(wd.icon));
        WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogTitle(LanguageData.GetContent(wd.name));
        Wing w = m_wings[wd.id];
        float p = 0;
        if (wld.nextLevelExp == 0)
        {
            p = 1;
            WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogProgressText(w.exp + "/" + w.exp);
        }
        else
        {
            p = (float)w.exp / (float)wld.nextLevelExp;
            WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogProgressText(w.exp + "/" + wld.nextLevelExp);
        }
        WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogProgressSize(p);
        for (int i = 0; i < attrs.Count; i++)
        {
            WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogCurrentAttr(attrs[i], i);
            if (nextAttrs != null)
            {
                WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogNextAttr(nextAttrs[i], i);
            }
            else
            {
                WingUIUpgradeDialogLogicManager.Instance.SetUpgradeDialogNextAttr("", i);
            }
        }
        WingUIUpgradeDialogLogicManager.Instance.SetUIDirty();
    }

    private void Buy(int id)
    {
        m_self.RpcCall("BuyOrdinaryWingReq", id);
    }

    private void Active(int id)
    {
        m_self.RpcCall("MagicWingActiveReq", id);
    }

    private void PutOn(int id)
    {
        m_self.RpcCall("WingExchangeReq", id);
    }

    private void Undo(int id)
    {
    }

    private void UnLock(int id)
    {
        m_self.RpcCall("UnlockMagicWingReq", id);
    }

    private void Upgrade(int id)
    {
        m_self.RpcCall("TrainWingReq", tipData.id);
    }
}
