using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.GameData;
using Mogo.Util;

public enum MarketUITab
{
    HotTab = 0,
    JewelTab = 1,
    ItemTab = 2,
    KeyTab = 3,
    WingTab = 4
}

class MarketView : MonoBehaviour
{
    private static MarketView m_instance;
    public static MarketView Instance { get {return MarketView.m_instance; }}

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量

    private UISprite m_spMarketUIMogoUIRefreshCtrl; // 用于刷新MogoUI图集

    private MogoButton keyActivateBtn;
    private MogoButton key;
    private MogoButton m_marketUIBtnClose;
    private MogoButton hot;
    private MogoButton item;
    private MogoButton jewel;
    private MogoButton wing;
    private GameObject m_goMarketUIArrowL;
    private GameObject m_goMarketUIArrowR;
    private MogoButton pre;
    private MogoButton next;
    private MogoButton pay;
    private UILabel rmb;
    private UILabel page;
    private GameObject keyView;
    private MogoList listView;
    public MogoList ListView
    {
        get
        {
            return listView;
        }
    }

    private UIInput keyInput;

    private UIImageButton hotImgBtn;
    private UIImageButton itemImgBtn;
    private UIImageButton jewelImgBtn;
    private UIImageButton keyImgBtn;
    private UIImageButton wingImgBtn;
    private GameObject m_goItemIconNotice;

    private List<MarketItemView> list = new List<MarketItemView>();
    Dictionary<int, UILabel> m_MarketTabLabelList = new Dictionary<int, UILabel>();

    private MarketBuyView buy;

    private int currPage = 0;
    private int pageSize = 4;
    private int CELL_WIDTH = 310;
    private List<MarketItem> items;
    private Action fromOther = null;

    private int m_iTabIdx = 0; //tabbar的选中位置
    private bool m_isActivating = false;
    public int TabIdx
    {
        get
        {
            return m_iTabIdx;
        }
        set
        {
            HandleMarketTabChange(m_iTabIdx, value);
            m_iTabIdx = value;
        }
    }

    void Awake()
    {
        m_instance = this;

        m_spMarketUIMogoUIRefreshCtrl = transform.Find("MarketUIMogoUIRefreshCtrl").GetComponentsInChildren<UISprite>(true)[0];

        keyView = transform.Find("keyView").gameObject;
        keyActivateBtn = keyView.transform.Find("Button").GetComponent<MogoButton>();
        keyInput = keyView.transform.Find("Input").GetComponent<UIInput>();

        // 热销
        hotImgBtn = transform.Find("hot").GetComponent<UIImageButton>();
        hot = transform.Find("hot").GetComponent<MogoButton>();
        hot.clickHandler = HotList;
        hot.pressHandler = HotPress;
        // 等级礼包
        itemImgBtn = transform.Find("item").GetComponent<UIImageButton>();
        item = transform.Find("item").GetComponent<MogoButton>();
        item.clickHandler = ItemList;
        item.pressHandler = ItemPress;
        m_goItemIconNotice = transform.Find("item/ItemIconNotice").gameObject;
        // 宝石
        jewelImgBtn = transform.Find("jewel").GetComponent<UIImageButton>();
        jewel = transform.Find("jewel").GetComponent<MogoButton>();
        jewel.clickHandler = JewelList;
        jewel.pressHandler = JewelPress;
        // 激活码礼包
        keyImgBtn = transform.Find("key").GetComponent<UIImageButton>();
        key = transform.Find("key").GetComponent<MogoButton>();
        key.clickHandler = KeyView;
        key.pressHandler = KeyPress;
        keyActivateBtn.clickHandler = Activate;
        //翅膀
        wingImgBtn = transform.Find("Wing").GetComponent<UIImageButton>();
        wing = transform.Find("Wing").GetComponent<MogoButton>();
        wing.clickHandler = WingView;
        wing.pressHandler = WingPress;

        m_marketUIBtnClose = transform.Find("MarketUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0];
        m_marketUIBtnClose.clickHandler = CloseHandler;

        m_goMarketUIArrowL = transform.Find("MarketUIArrow/MarketUIArrowL").gameObject;
        m_goMarketUIArrowR = transform.Find("MarketUIArrow/MarketUIArrowR").gameObject;

        pay = transform.Find("MarketUIBtnPay").GetComponent<MogoButton>();
        rmb = transform.Find("MarketUIDiamondNum").GetComponent<UILabel>();
        page = transform.Find("MarketUIPageNum").GetComponent<UILabel>();

        m_MarketTabLabelList[(int)MarketUITab.HotTab] = transform.Find("hot/HotLabel").GetComponent<UILabel>();
        m_MarketTabLabelList[(int)MarketUITab.JewelTab] = transform.Find("jewel/JewelLabel").GetComponent<UILabel>();
        m_MarketTabLabelList[(int)MarketUITab.ItemTab] = transform.Find("item/ItemLabel").GetComponent<UILabel>();
        m_MarketTabLabelList[(int)MarketUITab.KeyTab] = transform.Find("key/KeyLabel").GetComponent<UILabel>();
        m_MarketTabLabelList[(int)MarketUITab.WingTab] = transform.Find("Wing/WingLabel").GetComponent<UILabel>();
        foreach (var pair in m_MarketTabLabelList)
        {
            if (pair.Key == (int)MarketUITab.HotTab)
                MarketTabDown(pair.Key);
            else
                MarketTabUp(pair.Key);
        }

        // ChineseData
        if (m_MarketTabLabelList[(int)MarketUITab.HotTab] != null)
            m_MarketTabLabelList[(int)MarketUITab.HotTab].text = LanguageData.GetContent(200011); // "热销";
        if (m_MarketTabLabelList[(int)MarketUITab.JewelTab] != null)
            m_MarketTabLabelList[(int)MarketUITab.JewelTab].text = LanguageData.GetContent(200012); // "宝石";
        if (m_MarketTabLabelList[(int)MarketUITab.ItemTab] != null)
            m_MarketTabLabelList[(int)MarketUITab.ItemTab].text = LanguageData.GetContent(200013); // "道具";
        if (m_MarketTabLabelList[(int)MarketUITab.KeyTab] != null)
            m_MarketTabLabelList[(int)MarketUITab.KeyTab].text = LanguageData.GetContent(200014); // "激活码";
        if (m_MarketTabLabelList[(int)MarketUITab.WingTab] != null)
            m_MarketTabLabelList[(int)MarketUITab.WingTab].text = LanguageData.GetContent(7525); //翅膀
        gameObject.transform.Find("MarketUIBtnPay/MarketUIBtnPayText").GetComponent<UILabel>().text = LanguageData.GetContent(5001); // "充值";

        Transform cell = transform.Find("list/PropCell");
        Transform listStage = transform.Find("list");
        listView = listStage.GetComponent<MogoList>();
        listView.turnPage = TurnPage;

        list.Add(new MarketItemView(cell));

        for (int ii = 1; ii <= pageSize - 1; ii++)
        {
            GameObject copyCell = GameObject.Instantiate(cell.gameObject) as GameObject;
            MarketItemView itemCell = new MarketItemView(copyCell.transform);
            itemCell.AddToParent(listStage, gameObject.transform.rotation);
            list.Add(itemCell);
            itemCell.LocalPosition = cell.localPosition + new Vector3(CELL_WIDTH * ii, 0, 0);
        }

        //pre.clickHandler = PrePage;
        //next.clickHandler = NextPage;
        pay.clickHandler = Pay;

        buy = new MarketBuyView(gameObject.transform.Find("Buy"));
        buy.Close();
        hotImgBtn.SelectedStatus(true);
        keyView.SetActive(false);
        EventDispatcher.AddEventListener<MarketItem>(MarketEvent.OpenBuy, OpenBuy);
        EventDispatcher.AddEventListener<bool>(MarketEvent.LigthArrow, LightArrowHandler);
    }

    private void Activate()
    {
        if (m_isActivating) return;
        m_isActivating = true;
        MogoGlobleUIManager.Instance.ShowGlobleStaticText(true, LanguageData.GetContent(50001));
        string url = SystemConfig.GetActivateKeyUrl(MogoWorld.thePlayer.dbid, keyInput.text);
        //Debug.LogError("url:" + url);


        Action action = () =>
          {
              Mogo.Util.Utils.GetHttp(url,
                  (resp) =>
                  {
                      Driver.Invoke(() =>
                      {
                          MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "");
                          m_isActivating = false;
                          int respCode = int.Parse(resp);

                          if (respCode != 0)
                          {
                              if (respCode == 5)
                                  MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(50007));
                              else
                                  MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(respCode + 50001));
                          }


                      });
                  },
                  (errodCode) =>
                  {
                      Driver.Invoke(() =>
                      {
                          //Debug.LogError("haha");
                          MogoMsgBox.Instance.ShowMsgBox("network error：" + errodCode);
                          m_isActivating = false;
                          MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "");
                      });
                  }
                  );

          };

        action.BeginInvoke(null, null);
    }   

    #region Tab Change

    public void HandleMarketTabChange(int fromTab, int toTab)
    {
        MarketTabUp(fromTab);
        MarketTabDown(toTab);
    }

    void MarketTabUp(int tab)
    {
        if (m_MarketTabLabelList.ContainsKey(tab))
        {
            UILabel fromTabLabel = m_MarketTabLabelList[tab];
            if (fromTabLabel != null)
            {
                fromTabLabel.color = new Color32(37, 29, 6, 255);
                fromTabLabel.effectStyle = UILabel.Effect.None;

                //fromTabLabel.color = new Color32(255, 255, 255, 255);
                //fromTabLabel.effectStyle = UILabel.Effect.Outline;
                //fromTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }
    }

    void MarketTabDown(int tab)
    {
        if (m_MarketTabLabelList.ContainsKey(tab))
        {
            UILabel toTabLabel = m_MarketTabLabelList[tab];
            if (toTabLabel != null)
            {
                toTabLabel.color = new Color32(255, 255, 255, 255);
                toTabLabel.effectStyle = UILabel.Effect.Outline;
                toTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }
    }
    #endregion

    private void LightArrowHandler(bool light)
    {
        //if (light)
        //{
        //    pre.GetComponentInChildren<UISprite>().color = new Color32(22, 22, 22, 255);
        //    next.GetComponentInChildren<UISprite>().color = new Color32(22, 22, 22, 255);
        //}
        //else
        //{
        //    pre.GetComponentInChildren<UISprite>().color = new Color32(255, 255, 255, 255);
        //    next.GetComponentInChildren<UISprite>().color = new Color32(255, 255, 255, 255);
        //}
    }

    private void TurnPage(int sw)
    {
        if (sw == -1)
        {//左翻
            PrePage();
        }
        else
        {//右翻
            NextPage();
        }

    }

    private void OpenBuy(MarketItem data)
    {
        if (data.marketItemType == MarkItemType.WingItem)
        {
            Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.WingEvent.OpenBuy, data.resData.itemId, data.resData.priceNow);
            return;
        }
        buy.SetProp(data);
        buy.Open();
    }

    private void UnSelectedAll()
    {
        if (hotImgBtn == null) return;
        hotImgBtn.SelectedStatus(false);
        itemImgBtn.SelectedStatus(false);
        jewelImgBtn.SelectedStatus(false);
        keyImgBtn.SelectedStatus(false);
        wingImgBtn.SelectedStatus(false);
    }

    public void UpdateDiamond(int diamond)
    {
        if (rmb != null)
        {
            rmb.text = diamond + "";
        }
    }

    public void HotPress(bool press)
    {
        if (press)
        {
            return;
        }
        UnSelectedAll();
        hotImgBtn.SelectedStatus(true);
    }

    public void ItemPress(bool press)
    {
        if (press)
        {
            return;
        }
        UnSelectedAll();
        itemImgBtn.SelectedStatus(true);
    }

    public void JewelPress(bool press)
    {
        if (press)
        {
            return;
        }
        UnSelectedAll();
        jewelImgBtn.SelectedStatus(true);
    }

    private void KeyPress(bool press)
    {
        if (press)
        {
            return;
        }
        UnSelectedAll();
        keyImgBtn.SelectedStatus(true);
    }

    private void WingPress(bool press)
    {
        if(press)
        {
            return;
        }

        UnSelectedAll();
        wingImgBtn.SelectedStatus(true);
    }
   
    private void ShowKeyView()
    {
        keyView.SetActive(true);
    }

    public void FromOther(Action call)
    {
        fromOther = call;
    }

    #region 选择不同的标签页

    /// <summary>
    /// 点击热销Tab
    /// </summary>
    private void HotList()
    {
        HotList(true);
    }

    private void HotList(bool bTriggerEvent)
    {
        if (TabIdx == (int)MarketUITab.HotTab)
            return;

        if (!listView.gameObject.activeSelf) listView.gameObject.SetActive(true);
        keyView.SetActive(false);
        TabIdx = (int)MarketUITab.HotTab;
        currPage = 0;
        listView.FastTweenTo(currPage);

        if (bTriggerEvent)
        {
            EventDispatcher.TriggerEvent(MarketEvent.HotList);
            UpdatePageNum(true);
        }
    }

    /// <summary>
    /// 点击等级礼包Tab
    /// </summary>
    private void ItemList()
    {
        ItemList(true);
    }

    private void ItemList(bool bTriggerEvent)
    {
        if (TabIdx == (int)MarketUITab.ItemTab)
            return;

        MogoWorld.thePlayer.RpcCall("LevelGiftRecordReq");
        if (!listView.gameObject.activeSelf) listView.gameObject.SetActive(true);
        keyView.SetActive(false);
        TabIdx = (int)MarketUITab.ItemTab;
        currPage = 0;
        listView.FastTweenTo(currPage);

        if (bTriggerEvent)
        {
            EventDispatcher.TriggerEvent(MarketEvent.ItemList);
            UpdatePageNum(true);
        }
    }

    /// <summary>
    /// 点击宝石Tab
    /// </summary>
    private void JewelList()
    {
        JewelList(true);
    }

    private void JewelList(bool bTriggerEvent)
    {
        if (TabIdx == (int)MarketUITab.JewelTab)
            return;

        if (!listView.gameObject.activeSelf) listView.gameObject.SetActive(true);
        keyView.SetActive(false);
        TabIdx = (int)MarketUITab.JewelTab;
        currPage = 0;
        listView.FastTweenTo(currPage);

        if (bTriggerEvent)
        {
            EventDispatcher.TriggerEvent(MarketEvent.JewelList);
            UpdatePageNum(true);
        }
    }

    /// <summary>
    /// 点击激活码Tab
    /// </summary>
    private void KeyView()
    {
        KeyView(true);
    }

    private void WingView()
    {
        WingView(true);
    }

    private void WingView(bool bTriggerEvent)
    {
        if(TabIdx == (int)MarketUITab.WingTab)
            return;

        //TabIdx = (int)MarketUITab.WingTab;
        //listView.gameObject.SetActive(false);
        //ShowKeyView();
        if (!listView.gameObject.activeSelf) listView.gameObject.SetActive(true);
        keyView.SetActive(false);
        TabIdx = (int)MarketUITab.WingTab;
        currPage = 0;
        listView.FastTweenTo(currPage);

        if(bTriggerEvent)
        {
            EventDispatcher.TriggerEvent(MarketEvent.WingList);
            UpdatePageNum(false);
        }

        HideArrow();
    }

    private void KeyView(bool bTriggerEvent)
    {
        if (TabIdx == (int)MarketUITab.KeyTab)
            return;

        TabIdx = (int)MarketUITab.KeyTab;
        listView.gameObject.SetActive(false);
        ShowKeyView();

        if (bTriggerEvent)
        {
            UpdatePageNum(false);
        }        
        HideArrow();
    }

    /// <summary>
    /// 是否显示等级礼包领取提示
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowItemIconNotice(bool isShow)
    {
        m_goItemIconNotice.SetActive(isShow);
    }

    #endregion

    #region 设置界面信息

    #region 翻页和箭头设置

    private void PrePage()
    {
        if (currPage <= 0)
        {
            listView.TweenTo(currPage);
            return;
        }
        currPage--;
        listView.TweenTo(currPage);

        UpdatePageNum();
        ShowArrow();
    }

    private void NextPage()
    {
        if ((currPage + 1) * pageSize >= items.Count)
        {
            listView.TweenTo(currPage);
            return;
        }
        currPage++;
        listView.TweenTo(currPage);

        UpdatePageNum();
        ShowArrow();
    }

    /// <summary>
    /// 是否显示箭头(如果页数大于1显示)
    /// </summary>
    private void ShowArrow()
    {
        int maxPageIndex = (int)Math.Ceiling((double)items.Count / (double)pageSize) - 1;
        if(maxPageIndex > 0)
        {
            if(currPage == 0)
            {
                m_goMarketUIArrowL.SetActive(false);
                m_goMarketUIArrowR.SetActive(true);
            }
            else if (currPage == maxPageIndex)
            {
                m_goMarketUIArrowL.SetActive(true);
                m_goMarketUIArrowR.SetActive(false);
            }
            else
            {
                m_goMarketUIArrowL.SetActive(true);
                m_goMarketUIArrowR.SetActive(true);
            }
        }
        else
        {
            m_goMarketUIArrowL.SetActive(false);
            m_goMarketUIArrowR.SetActive(false);
        }       
    }

    /// <summary>
    /// 隐藏提示箭头
    /// </summary>
    private void HideArrow()
    {
        m_goMarketUIArrowL.SetActive(false);
         m_goMarketUIArrowR.SetActive(false);
    }

    /// <summary>
    /// 更新页数信息
    /// </summary>
    private void UpdatePageNum(bool isShow = true)
    {
        page.text = (currPage + 1) + "/" + Math.Ceiling((double)items.Count / (double)pageSize);
        page.gameObject.SetActive(isShow);
    }   

    #endregion

    private void Pay()
    {
#if UNITY_IPHONE
        EventDispatcher.TriggerEvent(IAPEvents.ShowIAPView);
#else
        LoggerHelper.Debug("pay");
        //MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(25570));
        EventDispatcher.TriggerEvent(Events.OperationEvent.Charge);
#endif
    }

    public void SetRMB(int _rmb)
    {
        rmb.text = _rmb + "";
    }

    #endregion

    #region 创建Item

    public void SetItems(List<MarketItem> items)
    {
        this.items = items;
        if (list.Count > items.Count)
        {
            CloseAllItem();
        }
        if (list.Count < items.Count)
        {
            Transform cell = gameObject.transform.Find("list/PropCell");
            Transform listStage = gameObject.transform.Find("list");
            int dc = items.Count - list.Count;
            for (int i = 0; i < dc; i++)
            {
                GameObject copyCell = GameObject.Instantiate(cell.gameObject) as GameObject;
                MarketItemView itemCell = new MarketItemView(copyCell.transform);
                itemCell.AddToParent(listStage, gameObject.transform.rotation);
                list.Add(itemCell);
                itemCell.LocalPosition = cell.localPosition + new Vector3(CELL_WIDTH * (list.Count - 1), 0, 0);
            }
        }
        for (int i = 0; i < items.Count; i++)
        {
            list[i].SetItem(items[i]);
            list[i].Open();
        }

        UpdatePageNum(); // 处理分页问题
        ShowArrow(); // 处理箭头显示
    }

    // 隐藏所有Item
    private void CloseAllItem()
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Close();
        }
    }

    #endregion

    #region 充值按钮特效

    private string m_fx1ChargeButton = "ChargeButtonFX1";

    /// <summary>
    /// 在充值按钮上附加特效
    /// </summary>
    private void AttachChargeButtonAnimation()
    {
        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        MogoFXManager.Instance.AttachParticleAnim("fx_ui_baoxiangxingxing.prefab", m_fx1ChargeButton, pay.gameObject,
            MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
            {
                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
    }

    /// <summary>
    /// 释放充值按钮特效
    /// </summary>
    private void ReleaseChargeButtonAnimation()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fx1ChargeButton);
    }

    #endregion

    #region 通过代码模拟切换Tab

    public void SwitchTab(MarketUITab tab)
    {
        switch (tab)
        {
            case MarketUITab.HotTab:
                HotList(false);
                break;
            case MarketUITab.JewelTab:
                JewelList(false);
                break;
            case MarketUITab.ItemTab:
                ItemList(false);
                break;
            case MarketUITab.KeyTab:
                KeyView(false);
                break;
            case MarketUITab.WingTab:
                WingView(false);
                break;
            default:
                break;
        }
    }

    #endregion

    #region 界面打开和关闭

    public void AddToParent(Transform parent, Quaternion rotation)
    {
        transform.parent = parent;
        gameObject.transform.localPosition = new Vector3(0, 0, -1.5f);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    public void Remove()
    {
        if (transform.gameObject.activeSelf)
        {
            transform.parent = null;
        }
        m_instance = null;
        //GameObject.Destroy(gameObject);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        TimerHeap.AddTimer<int>(200, 0, UpdateDiamond, (int)MogoWorld.thePlayer.diamond);
    }

    public void CloseHandler()
    {
        MogoUIManager.Instance.CloseMarketUI();
        if (fromOther != null)
        {
            fromOther();
            fromOther = null;
        }
    }

    public void Close()
    {        
        listView.gameObject.SetActive(true);
        gameObject.SetActive(false);
        Clear();
    }

    private void Clear()
    {
        hotImgBtn.SelectedStatus(true);
        currPage = 0;
        listView.FastTweenTo(currPage);
        items = null;
    }

    void OnEnable()
    {
        if (m_spMarketUIMogoUIRefreshCtrl != null)
            m_spMarketUIMogoUIRefreshCtrl.RefreshAllShowAs();

        AttachChargeButtonAnimation();
    }

    void OnDisable()
    {
        ReleaseChargeButtonAnimation();        

        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyMarketUI();
        }
    }

    #endregion
}