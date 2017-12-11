/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MarketSystem
// 创建者：Hooke HU
// 修改者列表：
// 创建日期：2013-4-2
// 模块描述：商城管理器
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Game;
using Mogo.Util;
using Mogo.GameData;

public enum MarkItemType
{
    MarketItem = 1,
    LevelPacks = 2,
    WingItem = 3,
}

public class MarketItem
{
    public MarkItemType marketItemType = MarkItemType.MarketItem;
    public string name;
    public string icon;
    public int color;
    public int currNum;
    public MarketData resData;

    public MarketItem()
    {
    }

    public MarketItem(MarketData res)
    {
        resData = res;
        name = GetName(resData.itemId);
        icon = GetIcon(resData.itemId);
        if (res.wing == 1)
        {
            marketItemType = MarkItemType.WingItem;
        }
        currNum = resData.totalCount;
    }

    private string GetName(int id)
    {
        if (ItemEquipmentData.dataMap.ContainsKey(id))
        {
            return LanguageData.dataMap.Get(ItemEquipmentData.dataMap.Get(id).name).content;
        }
        if (ItemJewelData.dataMap.ContainsKey(id))
        {
            return LanguageData.dataMap.Get(ItemJewelData.dataMap.Get(id).name).content;
        }
        if (ItemMaterialData.dataMap.ContainsKey(id))
        {
            return LanguageData.dataMap.Get(ItemMaterialData.dataMap.Get(id).name).content;
        }
        if (WingData.dataMap.ContainsKey(id))
        {
            marketItemType = MarkItemType.WingItem;
            return LanguageData.GetContent(WingData.dataMap.Get(id).name);
        }
        //if (ItemOthersData.dataMap.ContainsKey(id))
        //{
        //    return LanguageData.dataMap.Get(ItemOthersData.dataMap.Get(id).name).content;
        //}
        //if (ItemPearlData.dataMap.ContainsKey(id))
        //{
        //    return LanguageData.dataMap.Get(ItemPearlData.dataMap.Get(id).name).content;
        //}
        return "noitemname";
    }

    private string GetIcon(int id)
    {
        if (ItemEquipmentData.dataMap.ContainsKey(id))
        {
            color = IconData.dataMap.Get(ItemEquipmentData.dataMap.Get(id).icon).color;
            return IconData.dataMap.Get(ItemEquipmentData.dataMap.Get(id).icon).path;
        }
        if (ItemJewelData.dataMap.ContainsKey(id))
        {
            color = IconData.dataMap.Get(ItemJewelData.dataMap.Get(id).icon).color;
            return IconData.dataMap.Get(ItemJewelData.dataMap.Get(id).icon).path;
        }
        if (ItemMaterialData.dataMap.ContainsKey(id))
        {
            color = IconData.dataMap.Get(ItemMaterialData.dataMap.Get(id).icon).color;
            return IconData.dataMap.Get(ItemMaterialData.dataMap.Get(id).icon).path;
        }
        if (WingData.dataMap.ContainsKey(id))
        {
            color = IconData.dataMap.Get(WingData.dataMap.Get(id).icon).color;
            return IconData.dataMap.Get(WingData.dataMap.Get(id).icon).path;
        }
        //if (ItemOthersData.dataMap.ContainsKey(id))
        //{
        //    color = IconData.dataMap.Get(ItemOthersData.dataMap.Get(id).icon).color;
        //    return IconData.dataMap.Get(ItemOthersData.dataMap.Get(id).icon).path;
        //}
        //if (ItemPearlData.dataMap.ContainsKey(id))
        //{
        //    color = IconData.dataMap.Get(ItemPearlData.dataMap.Get(id).icon).color;
        //    return IconData.dataMap.Get(ItemPearlData.dataMap.Get(id).icon).path;
        //}
        return "noicon";
    }
}

public class GiftItem : MarketItem
{
    public LevelGiftData resData;

    public GiftItem(LevelGiftData res)
    {
        marketItemType = MarkItemType.LevelPacks;
        resData = res;
        name = GetName(resData.item);
        icon = GetIcon(resData.item);
    }

    private string GetName(int id)
    {
        if (ItemEquipmentData.dataMap.ContainsKey(id))
        {
            return ItemEquipmentData.dataMap[id].Name;//LanguageData.dataMap.Get(ItemEquipmentData.dataMap.Get(id).name).content;
        }
        if (ItemJewelData.dataMap.ContainsKey(id))
        {
            return LanguageData.dataMap.Get(ItemJewelData.dataMap.Get(id).name).content;
        }
        if (ItemMaterialData.dataMap.ContainsKey(id))
        {
            return LanguageData.dataMap.Get(ItemMaterialData.dataMap.Get(id).name).content;
        }
        return "no name";
    }

    private string GetIcon(int id)
    {
        if (ItemEquipmentData.dataMap.ContainsKey(id))
        {
            color = ItemEquipmentData.dataMap[id].color;
            return ItemEquipmentData.dataMap[id].Icon;
        }
        if (ItemJewelData.dataMap.ContainsKey(id))
        {
            color = IconData.dataMap.Get(ItemJewelData.dataMap.Get(id).icon).color;
            return IconData.dataMap.Get(ItemJewelData.dataMap.Get(id).icon).path;
        }
        if (ItemMaterialData.dataMap.ContainsKey(id))
        {
            color = IconData.dataMap.Get(ItemMaterialData.dataMap.Get(id).icon).color;
            return IconData.dataMap.Get(ItemMaterialData.dataMap.Get(id).icon).path;
        }
        return "no icon";
    }
}

public class WingItem : MarketItem
{
    public WingData resData;

    public WingItem(WingData res)
    {
        marketItemType = MarkItemType.WingItem;
        resData = res;
        name = GetName(res.name);
        icon = GetIcon(res.icon);
    }

    private string GetName(int id)
    {
        return LanguageData.GetContent(id);
    }

    private string GetIcon(int id)
    {
        color = IconData.dataMap.Get(id).color;
        return IconData.GetIconPath(id);
    }
}

public class MarketManager
{
    private EntityMyself m_myself;

    private List<MarketItem> items = new List<MarketItem>();
    private List<MarketItem> gifts = new List<MarketItem>();
    private List<int> gotGifts = new List<int>();

    private MarketItem preview = null;
    private MarketItem preBuy = null;

    /// <summary>
    /// 商城界面当前Tab
    /// </summary>
    private MarketUITab m_currentMarketUITab = MarketUITab.HotTab;
    public MarketUITab CurrentMarketUITab
    {
        get { return m_currentMarketUITab; }
        set
        {
            m_currentMarketUITab = value;
        }
    }

    private Action fromOther = null;

    private int version = 0; //未取数据

    public MarketManager(EntityMyself theOwner)
    {
        m_myself = theOwner;
        AddListeners();
    }

    private void AddListeners()
    {
        EventDispatcher.AddEventListener(MarketEvent.DownloadMarket, Download);
        EventDispatcher.AddEventListener(MarketEvent.HotList, ShowHotList);
        EventDispatcher.AddEventListener(MarketEvent.ItemList, ShowItemList);
        EventDispatcher.AddEventListener(MarketEvent.JewelList, ShowJewelList);
        EventDispatcher.AddEventListener<int, MarketItem>(MarketEvent.BuyNum, BuyNumReq);
        EventDispatcher.AddEventListener<Action>(MarketEvent.OpenWithJewel, OpenWithJewelHandler);
        EventDispatcher.AddEventListener(MarketEvent.OpenWithWing, OpenWithWingHandler);
        EventDispatcher.AddEventListener<GiftItem>(MarketEvent.DrawLevelPacks, GetGift);
        EventDispatcher.AddEventListener(MarketEvent.WingList, ShowWingList);
        EventDispatcher.AddEventListener<MarketItem>(MarketEvent.Preview, Preview);
        EventDispatcher.AddEventListener(MarketEvent.PreviewBuy, PreviewBuy);
        EventDispatcher.AddEventListener(MarketEvent.WingBuy, WingBuy);
        EventDispatcher.AddEventListener<MarketItem>(MarketEvent.OpenBuy, ForCacheWingItem);
        EventDispatcher.AddEventListener(Events.WingEvent.ClosePreview, ClosePreview);

        // EventDispatcher.AddEventListener(MarketEvent.DownloadLoginMarket, DownloadLogin);
    }

    private void RemoveListeners()
    {
        EventDispatcher.RemoveEventListener(MarketEvent.DownloadMarket, Download);
        EventDispatcher.RemoveEventListener(MarketEvent.HotList, ShowHotList);
        EventDispatcher.RemoveEventListener(MarketEvent.ItemList, ShowItemList);
        EventDispatcher.RemoveEventListener(MarketEvent.JewelList, ShowJewelList);
        EventDispatcher.RemoveEventListener<int, MarketItem>(MarketEvent.BuyNum, BuyNumReq);
        EventDispatcher.RemoveEventListener<Action>(MarketEvent.OpenWithJewel, OpenWithJewelHandler);
        EventDispatcher.RemoveEventListener(MarketEvent.OpenWithWing, OpenWithWingHandler);
        EventDispatcher.RemoveEventListener<GiftItem>(MarketEvent.DrawLevelPacks, GetGift);
        EventDispatcher.RemoveEventListener(MarketEvent.WingList, ShowWingList);
        EventDispatcher.RemoveEventListener<MarketItem>(MarketEvent.Preview, Preview);
        EventDispatcher.RemoveEventListener(MarketEvent.PreviewBuy, PreviewBuy);
        EventDispatcher.RemoveEventListener(MarketEvent.WingBuy, WingBuy);
        EventDispatcher.RemoveEventListener<MarketItem>(MarketEvent.OpenBuy, ForCacheWingItem);
        EventDispatcher.RemoveEventListener(Events.WingEvent.ClosePreview, ClosePreview);

        // EventDispatcher.RemoveEventListener(MarketEvent.DownloadLoginMarket, DownloadLogin);
    }

    public void Clean()
    {
        RemoveListeners();
    }

    private int updateCnt = 0;
    public void Download()
    {
        updateCnt = 0;
        if (version == 0)
        {//没数据去请求
            DownloadForce();
            return;
        }

        items.Clear();
        foreach (var i in MarketData.dataMap)
        {
            items.Add(new MarketItem(i.Value));
        }
        foreach (var i in items)
        {
            if (i.resData.mode == 2)
            {//限购
                GetLimitNum(i.resData.marketVersion, i.resData.id);
            }
        }

        ShowMarketUIByTab();     
        MogoWorld.thePlayer.RpcCall("MarketVersionCheck", version);
        ShowHint();
    }

    public void DownloadForce()
    {
        if (updateCnt > 10)
        {
            return;
        }
        updateCnt++;
        m_myself.RpcCall("TransferMarketDataReq");
        //ActorMyself actor = m_myself.Actor as ActorMyself;
        //actor.StartCoroutine(DownloadMarket());
    }

    public void UpdateItems(int version, LuaTable props)
    {
        this.version = version;
        MarketData.dataMap.Clear();
        items.Clear();
        foreach (var p in props)
        {
            int key = Int32.Parse(p.Key);
            MarketData.InsertMarketData(key, version, (p.Value as LuaTable));
            items.Add(new MarketItem(MarketData.dataMap[key]));
        }
        foreach (var i in items)
        {
            if (i.resData.mode == 2)
            {//限购
                GetLimitNum(i.resData.marketVersion, i.resData.id);
            }
        }

        ShowMarketUIByTab();
        ShowHint();
    }

    private IEnumerator DownloadMarket()
    {
        MarketData.dataMap.Clear();
        items.Clear();
        WWW d = new WWW(SystemConfig.GetCfgInfoUrl(SystemConfig.MARKET_URL_KEY));
        yield return d;
        var xml = XMLParser.LoadXML(d.text);
        var map = XMLParser.LoadIntMap(xml, SystemConfig.MARKET_URL_KEY);
        var type = typeof(MarketData);
        var props = type.GetProperties();
        foreach (var item in map)
        {
            var t = new MarketData();
            foreach (var prop in props)
            {
                if (prop.Name == "id")
                {
                    prop.SetValue(t, item.Key, null);
                }
                else
                {
                    if (item.Value.ContainsKey(prop.Name))
                    {
                        var value = Utils.GetValue(item.Value[prop.Name], prop.PropertyType);
                        prop.SetValue(t, value, null);
                    }
                }
            }
            if (!MarketData.dataMap.ContainsKey(item.Key))
            {
                MarketData.dataMap.Add(item.Key, t);
            }
            version = t.marketVersion;
            items.Add(new MarketItem(t));
        }
        foreach (var i in items)
        {
            if (i.resData.mode == 2)
            {//限购
                GetLimitNum(i.resData.marketVersion, i.resData.id);
            }
        }

        ShowMarketUIByTab();   
    }

    /// <summary>
    /// 通过Tab显示商城UI
    /// </summary>
    private void ShowMarketUIByTab()
    {
        switch (CurrentMarketUITab)
        {
            case MarketUITab.JewelTab:
                {
                    ShowJewelList();
                    MarketView.Instance.FromOther(fromOther);
                    fromOther = null;
                    CurrentMarketUITab = MarketUITab.HotTab;
                }
                break;
            case MarketUITab.HotTab:
                {
                    ShowHotList();
                }
                break;
            case MarketUITab.ItemTab:
                {
                    ShowItemList();
                }
                break;
            case MarketUITab.WingTab:
                {
                    ShowWingList();
                }
                break;
            default:
                {
                    ShowHotList();
                }
                break;
        }    
    }

    private void UpdateMarket()
    {

    }

    private void OpenWithJewelHandler(Action call)
    {
        MogoUIManager.Instance.SwitchToMarket(MarketUITab.JewelTab);        
        fromOther = call;
    }

    private void OpenWithWingHandler()
    {
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.None);
        MogoUIManager.Instance.SwitchToMarket(MarketUITab.WingTab);
    }

    private void Preview(MarketItem data)
    {
        preview = data;
        int id = ItemEffectData.dataMap[ItemMaterialData.dataMap[data.resData.itemId].effectId].reward1[13];
        m_myself.PreviewWing(id);
        WingData d = WingData.dataMap.Get(id);
        WingLevelData ld = d.GetLevelData(1);
        List<string> attrs = ld.GetPropertEffect();
        WingUIPreviewInMarketUILogicManager.Instance.SetTitle(LanguageData.GetContent(d.name));
        WingUIPreviewInMarketUILogicManager.Instance.SetTipAttr(attrs);
        WingUIPreviewInMarketUILogicManager.Instance.SetWingAttr(attrs);
        WingUIPreviewInMarketUILogicManager.Instance.SetUIDirty();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.WingPreviewInMarketUI);
    }

    private void PreviewBuy()
    {
        //int id = ItemEffectData.dataMap[ItemMaterialData.dataMap[preview.resData.itemId].effectId].reward1[13];
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.None);
        EventDispatcher.TriggerEvent(Events.WingEvent.OpenBuy, preview.resData.itemId, preview.resData.priceNow);
        preBuy = preview;
    }

    private void WingBuy()
    {
        BuyNumReq(1, preBuy);
    }

    private void ForCacheWingItem(MarketItem data)
    {
        if (data.marketItemType == MarkItemType.WingItem)
        {
            preBuy = data;
        }
    }

    private void ClosePreview()
    {
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.None);
        m_myself.ResetPreviewWing();
        m_myself.UpdateDressWing();
    }

    private void ShowHotList()
    {
        List<MarketItem> list = new List<MarketItem>();
        foreach (var item in items)
        {
            if (item.resData.hot == 1)
            {
                list.Add(item);
            }
        }
        list.Sort(HotSort);
        MarketView.Instance.HotPress(false);
        MarketView.Instance.SetItems(list);
    }

    private int HotSort(MarketItem a, MarketItem b)
    {
        if (a.resData.hotSort == b.resData.hotSort)
        {
            return 0;
        }
        if (a.resData.hotSort > b.resData.hotSort)
        {
            return 1;
        }
        return -1;
    }

    private void ShowJewelList()
    {
        List<MarketItem> list = new List<MarketItem>();
        foreach (var item in items)
        {
            if (item.resData.jewel == 1)
            {
                list.Add(item);
            }
        }
        list.Sort(JewelSort);
        MarketView.Instance.JewelPress(false);
        MarketView.Instance.SetItems(list);
    }

    private int JewelSort(MarketItem a, MarketItem b)
    {
        if (a.resData.jewelSort == b.resData.jewelSort)
        {
            return 0;
        }
        if (a.resData.jewelSort > b.resData.jewelSort)
        {
            return 1;
        }
        return -1;
    }

    private void ShowWingList()
    {
        List<MarketItem> list = new List<MarketItem>();
        foreach (var item in items)
        {
            if (item.resData.wing == 1)
            {
                list.Add(item);
            }
        }
        if (MarketView.Instance != null)
        {
            MarketView.Instance.SetItems(list);
        }
    }

    private int WingSort(MarketItem a, MarketItem b)
    {
        if (a.resData.wingSort == b.resData.wingSort)
        {
            return 0;
        }
        if (a.resData.wingSort > b.resData.wingSort)
        {
            return 1;
        }
        return -1;
    }

    private void ShowItemList()
    {
        //List<MarketItem> list = new List<MarketItem>();
        //foreach (var item in items)
        //{
        //    if (item.resData.item == 1)
        //    {
        //        list.Add(item);
        //    }
        //}
        //list.Sort(ItemSort);
        BuildGifts();
        gifts.Sort(ItemSort);
        if (MarketView.Instance != null)
        {
            MarketView.Instance.ItemPress(false);
            MarketView.Instance.SetItems(gifts);
        }
    }

    private int ItemSort(MarketItem a, MarketItem b)
    {
        if ((a as GiftItem).resData.level == (b as GiftItem).resData.level)
        {
            return 0;
        }
        if ((a as GiftItem).resData.level > (b as GiftItem).resData.level)
        {
            return 1;
        }
        return -1;
    }

    private void BuildGifts()
    {
        gifts.Clear();
        int vocation = (int)MogoWorld.thePlayer.vocation;
        foreach (var item in LevelGiftData.dataMap)
        {
            if (gotGifts.Contains(item.Key))
            {
                continue;
            }
            if (item.Value.vocation != vocation && item.Value.vocation != 0)
            {
                continue;
            }
            GiftItem m = new GiftItem(item.Value);
            gifts.Add(m);
        }
    }

    public void UpdateGifts(List<int> ids)
    {
        gotGifts = ids;
        ShowItemList();
        ShowHint();
    }

    private bool GiftHint()
    {
        for (int i = 0; i < gifts.Count; i++)
        {
            if ((gifts[i] as GiftItem).resData.level <= MogoWorld.thePlayer.level)
            {
                return true;
            }
        }
        return false;
    }

    private void ShowHint()
    {
        if (GiftHint())
        {
            if (MarketView.Instance != null)
            {
                MarketView.Instance.ShowItemIconNotice(true);
            }
            EventDispatcher.TriggerEvent(Events.NormalMainUIEvent.ShowMallConsumeIconTip, true);
        }
        else
        {
            if (MarketView.Instance != null)
            {
                MarketView.Instance.ShowItemIconNotice(false);
            }
            EventDispatcher.TriggerEvent(Events.NormalMainUIEvent.ShowMallConsumeIconTip, false);
        }
    }

    private void GetGift(GiftItem item)
    {
        uint idx = (uint)item.resData.id;
        m_myself.RpcCall("DrawGift", idx);
        TimerHeap.AddTimer(100, 0, GiftRecordReq);
    }

    public void GiftRecordReq()
    {
        m_myself.RpcCall("LevelGiftRecordReq");
    }

    private void BuyNumReq(int num, MarketItem data)
    {
        //请求后端购买,做预判断
        //<MarketBuy>
        //<Exposed/>
        //<Arg>UINT16</Arg>     <!--商城版本号-->
        //<Arg>UINT16</Arg>     <!--栏位号-->
        //<Arg>UINT32</Arg>     <!--道具ID-->
        //<Arg>UINT8</Arg>      <!--道具数量-->
        //<Arg>UINT32</Arg>     <!--当前价格（实际价格）-->
        //<Arg>UINT8</Arg>      <!--购买组数量-->
        //</MarketBuy>
        int price = data.resData.priceOrg;
        if (data.resData.priceNow > 0)
        {
            price = data.resData.priceNow;
        }
        if ((price * num) > MogoWorld.thePlayer.diamond)
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[307].content);
            return;
        }
        m_myself.RpcCall("MarketBuy", data.resData.marketVersion, data.resData.id, data.resData.itemId, data.resData.itemNumber, price, num);
    }

    private void GetLimitNum(int version, int id)
    {
        //<MarketGridDataReq>
        //<Exposed/>
        //<Arg>UINT16</Arg>     <!--商城版本号-->
        //<Arg>UINT16</Arg>     <!--栏位号-->
        //</MarketGridDataReq>
        m_myself.RpcCall("MarketGridDataReq", version, id);
    }

    public void UpdateNum(int id, int num)
    {
        foreach (var i in this.items)
        {
            if (i.resData.id == id)
            {
                i.currNum = num;
            }
        }
    }

    public void DownloadLogin()
    {
        ActorMyself actor = m_myself.Actor as ActorMyself;
        actor.StartCoroutine(DownloadLoginMarket());
    }

    private IEnumerator DownloadLoginMarket()
    {
        LoggerHelper.Debug("DownloadLoginMarket");
        LoginMarketData.dataMap.Clear();

        WWW d = new WWW(SystemConfig.GetCfgInfoUrl(SystemConfig.LOGIN_MARKET_URL_KEY));
        yield return d;

        LoggerHelper.Debug("DownloadLoginMarket d: " + d.text.Length);

        var xml = XMLParser.LoadXML(d.text);
        var map = XMLParser.LoadIntMap(xml, SystemConfig.LOGIN_MARKET_URL_KEY);
        var type = typeof(LoginMarketData);
        var props = type.GetProperties();
        foreach (var item in map)
        {
            var t = new LoginMarketData();
            foreach (var prop in props)
            {
                if (prop.Name == "id")
                {
                    prop.SetValue(t, item.Key, null);
                }
                else
                {
                    if (item.Value.ContainsKey(prop.Name))
                    {
                        var value = Utils.GetValue(item.Value[prop.Name], prop.PropertyType);
                        prop.SetValue(t, value, null);
                    }
                }
            }
            if (!LoginMarketData.dataMap.ContainsKey(item.Key))
            {
                LoggerHelper.Debug("LoginMarketData new " + item.Key + " version" + t.version);
                LoginMarketData.dataMap.Add(item.Key, t);
            }
        }

        EventDispatcher.TriggerEvent(Events.OperationEvent.GetLoginMessage);
    }
}
