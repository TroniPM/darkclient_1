using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;
using Mogo.GameData;

class MarketItemView
{
    private GameObject goItem;

    private UILabel m_lblItemName; // Item名称
    private UILabel m_lblItemNum; // Item数量
    private UISprite m_spItemIcon; // Item图标

    private UISprite m_spOldPriceIcon;
    private UILabel m_lblOldPriceNum;
    private UISprite m_spNewPriceIcon;
    private UILabel m_lblNewPriceNum;
    private GameObject m_goNewPriceLine;

    private UILabel m_lblRebateTitle;
    private GameObject m_goRebateFlagBG;
    private UILabel m_lblRebateFlagText;

    private GameObject limitflag;
    private GameObject hotflag;    
    private GameObject newflag;
    private GameObject delline;        
    
    private MogoButton buttonBuy;
    private MogoButton buttonPreview;
    private GameObject m_goButtonDraw;
    private MogoButton buttonDraw;
    private UILabel m_lblButtonDrawLabel;
    private UISprite m_spButtonDrawBGUp;

    private MarketItem data;

    public MarketItemView(Transform transform)
    {
        goItem = transform.gameObject;

        // 物品信息
        m_lblItemName = transform.Find("GridItemName").GetComponent<UILabel>();
        m_lblItemNum = transform.Find("GridItemNum").GetComponent<UILabel>();
        m_spItemIcon = transform.Find("GridItemIcon").GetComponent<UISprite>();        

        m_lblRebateTitle = transform.Find("GridRebateTitle").GetComponent<UILabel>();
        m_goRebateFlagBG = transform.Find("GridRebateFlag/GridRebateFlagBG").gameObject;
        m_lblRebateFlagText = transform.Find("GridRebateFlag/GridRebateFlagText").GetComponent<UILabel>();

        // 新价格
        m_spNewPriceIcon = transform.Find("GridNewPrice/GridNewPriceIcon").GetComponent<UISprite>();
        m_lblNewPriceNum = transform.Find("GridNewPrice/GridNewPriceNum").GetComponent<UILabel>();
        m_goNewPriceLine = transform.Find("GridNewPrice/GridNewPriceLine").gameObject;

        // 旧价格
        m_spOldPriceIcon = transform.Find("GridOldPrice/GridOldPriceIcon").GetComponent<UISprite>();
        m_lblOldPriceNum = transform.Find("GridOldPrice/GridOldPriceNum").GetComponent<UILabel>();

        // 购买按钮
        buttonBuy = transform.Find("GridButtonBuy").GetComponent<MogoButton>();
        buttonBuy.clickHandler = OnButtonBuyUp;

        //预览按钮
        buttonPreview = transform.Find("GridPreview").GetComponent<MogoButton>();
        buttonPreview.clickHandler = OnButtonPreviewUp;

        // 领取按钮
        m_goButtonDraw = transform.Find("GridButtonDraw").gameObject;
        buttonDraw = m_goButtonDraw.GetComponent<MogoButton>();
        buttonDraw.clickHandler = OnButtonDrawUp;
        m_lblButtonDrawLabel = transform.Find("GridButtonDraw/GridButtonDrawLabel").GetComponentsInChildren<UILabel>(true)[0];
        m_spButtonDrawBGUp = transform.Find("GridButtonDraw/GridButtonDrawBGUp").GetComponentsInChildren<UISprite>(true)[0];

        // 其他
        limitflag = transform.Find("limitflag").gameObject;
        hotflag = transform.Find("hotflag").gameObject;
        newflag = transform.Find("newflag").gameObject;
        delline = transform.Find("delline").gameObject;

        // ChineseData
        transform.Find("GridButtonBuy/GridButtonBuyLabel").GetComponent<UILabel>().text = Mogo.GameData.LanguageData.GetContent(1051); // "购买";
        transform.Find("GridPreview/GridPreviewText").GetComponent<UILabel>().text = Mogo.GameData.LanguageData.GetContent(26804);//预览
    }

    private void OnButtonBuyUp()
    {
        EventDispatcher.TriggerEvent(MarketEvent.OpenBuy, data);
    }

    public void ShowPreviewBtn(bool isShow)
    {
        buttonPreview.gameObject.SetActive(isShow);
    }

    private void OnButtonPreviewUp()
    {
        EventDispatcher.TriggerEvent(MarketEvent.Preview, data);
    }

    private void OnButtonDrawUp()
    {
        EventDispatcher.TriggerEvent(MarketEvent.DrawLevelPacks, data as GiftItem);
    }

    public Vector3 Position
    {
        get
        {
            return goItem.transform.position;
        }
        set
        {
            goItem.transform.position = value;
        }
    }

    public Vector3 LocalPosition
    {
        get
        {
            return goItem.transform.localPosition;
        }
        set
        {
            goItem.transform.localPosition = value;
        }
    }

    #region 设置Item信息   

    /// <summary>
    /// 设置物品信息
    /// </summary>
    /// <param name="item"></param>
    public void SetItem(MarketItem item)
    {
		InventoryGrid grid = goItem.GetComponent<InventoryGrid>();
		if(grid == null)
			grid = goItem.AddComponent<InventoryGrid>();
        if (item.marketItemType == MarkItemType.MarketItem)
        {
            grid.iconID = item.resData.itemId;
        }
        if (item.marketItemType == MarkItemType.LevelPacks)
        {
            grid.iconID = (item as GiftItem).resData.item;
        }
        if (item.marketItemType == MarkItemType.WingItem)
        {
            grid.iconID = item.resData.itemId;
        }

        // 物品BoxCollider区域添加滑动
        MogoButton mogoButton = goItem.GetComponent<MogoButton>();
        if (mogoButton == null)
            mogoButton = goItem.AddComponent<MogoButton>();
        mogoButton.pressHandler = MarketView.Instance.ListView.PressHandlerOutSide;
        mogoButton.dragHandler = MarketView.Instance.ListView.DragHandlerOutSide;

        ResetItem();
        data = item;

        m_lblItemName.text = data.name; // 设置Grid的名称  

        switch(data.marketItemType)
        {
            case MarkItemType.MarketItem:                   
            case MarkItemType.WingItem:
                //m_lblItemName.effectStyle = UILabel.Effect.None;
                SetMarketItem(); // 商城出售物品,翅膀
                break;
            case MarkItemType.LevelPacks:
                //m_lblItemName.effectStyle = UILabel.Effect.Outline;
                //m_lblItemName.effectColor = new Color32(50, 39, 9, 255);
                SetLevelPacks(); // 等级礼包
                break;
            default:
                break;
        }     
    } 

    /// <summary>
    /// 设置商城购买物品
    /// </summary>
    private void SetMarketItem()
    {
        ShowButtonBuy(true);
        if (data.marketItemType == MarkItemType.WingItem)
        {
            ShowPreviewBtn(true);
        }

        if (data.resData.priceOrg > 0)
        {
            m_lblOldPriceNum.text = data.resData.priceOrg + "";
            m_lblNewPriceNum.text = "x" + data.resData.priceNow;
            m_lblRebateTitle.enabled = true;
            m_spOldPriceIcon.enabled = true;
            m_spNewPriceIcon.enabled = true;
            m_lblOldPriceNum.enabled = true;
        }
        else
        {
            m_lblNewPriceNum.text = "x" + data.resData.priceNow;
            m_lblRebateTitle.enabled = false;
            m_spOldPriceIcon.enabled = false;
            m_lblOldPriceNum.enabled = false;
        }
        m_lblItemNum.text = "x" + data.resData.itemNumber + "";
        m_lblRebateTitle.text = "";

        MogoUIManager.Instance.TryingSetSpriteName(data.icon, m_spItemIcon);
        MogoUtils.SetImageColor(m_spItemIcon, data.color);

        FlagDisable();
        m_lblRebateFlagText.text = "";
        switch (data.resData.label)
        {
            case 1:
                {//新品
                    //newflag.SetActive(true);
                    m_goRebateFlagBG.SetActive(true);
                    m_lblRebateFlagText.text = Mogo.GameData.LanguageData.GetContent(200026); // "新品";
                    break;
                }
            case 2:
                {//热销
                    //hotflag.SetActive(true);
                    m_goRebateFlagBG.SetActive(true);
                    m_lblRebateFlagText.text = Mogo.GameData.LanguageData.GetContent(200011); // "热销";
                    break;
                }
            case 3:
                {//限购
                    //limitflag.SetActive(true);
                    m_goRebateFlagBG.SetActive(true);
                    m_lblRebateFlagText.text = Mogo.GameData.LanguageData.GetContent(200015); // "限购";
                    break;
                }
            case 4:
                {//折扣
                    m_goRebateFlagBG.SetActive(true);
                    m_lblRebateFlagText.text = Mogo.GameData.LanguageData.GetContent(200016); // "折扣";
                    break;
                }
        }
        delline.SetActive(false);
        float r = (float)data.resData.priceNow / (float)data.resData.priceOrg;
        if (r >= 1)
        {
            return;
        }
        r = r * 10;
        m_lblRebateTitle.text = r.ToString("f1") + Mogo.GameData.LanguageData.GetContent(200017);
        delline.SetActive(true);        
    }

    /// <summary>
    /// 设置等级礼包
    /// </summary>
    private void SetLevelPacks()
    {
        // 设置Grid图标
        MogoUIManager.Instance.TryingSetSpriteName(data.icon, m_spItemIcon);
        MogoUtils.SetImageColor(m_spItemIcon, data.color);

        m_lblNewPriceNum.text = "x" + (data as GiftItem).resData.price;
        m_lblRebateFlagText.text = "";

        // 设置Grid按钮
        ShowButtonDraw(true);
        SetButtonDraw((data as GiftItem).resData.level); // 设置领取按钮状态
    }

    /// <summary>
    /// 是否显示购买按钮
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowButtonBuy(bool isShow)
    {
        buttonBuy.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// 是否显示领取按钮
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowButtonDraw(bool isShow)
    {
        buttonDraw.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// 设置领取按钮
    /// </summary>
    /// <param name="canDraw"></param>
    /// <param name="requestLevel"></param>
    private void SetButtonDraw(int requestLevel)
    {
        if (MogoWorld.thePlayer.level >= requestLevel)
        {
            m_spButtonDrawBGUp.ShowAsWhiteBlack(false);
            m_goButtonDraw.GetComponentsInChildren<BoxCollider>(true)[0].enabled = true;
            m_lblButtonDrawLabel.text = LanguageData.GetContent(48100); // 免费领取
            m_goNewPriceLine.SetActive(true);
        }
        else
        {
            m_spButtonDrawBGUp.ShowAsWhiteBlack(true);
            m_goButtonDraw.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;
            m_lblButtonDrawLabel.text = string.Format(LanguageData.GetContent(48101), requestLevel); // XX级可领取
            m_goNewPriceLine.SetActive(false);
        }        
    }

    #endregion

    private void FlagDisable()
    {
        hotflag.SetActive(false);
        limitflag.SetActive(false);
        m_goRebateFlagBG.SetActive(false);
        newflag.SetActive(false);
    }

    /// <summary>
    /// 重置物品信息
    /// </summary>
    private void ResetItem()
    {
        m_lblItemName.text = "";
        m_lblItemNum.text = "";

        m_spOldPriceIcon.enabled = false;      
        m_lblOldPriceNum.text = "";
        m_lblNewPriceNum.text = "";
        m_goNewPriceLine.SetActive(false);
        
        m_lblRebateTitle.text = "";          
        delline.SetActive(false);
        FlagDisable();

        ShowButtonBuy(false);
        ShowButtonDraw(false);
        ShowPreviewBtn(false);
    }

    public void Clear()
    {
        data = null;
        ResetItem();
    }

    public void Open()
    {
        goItem.SetActive(true);
    }

    public void Close()
    {
        Clear();
        goItem.SetActive(false);
    }

    public void AddToParent(Transform parent, Quaternion rotation)
    {
        goItem.transform.rotation = rotation;
        goItem.transform.parent = parent;
        goItem.transform.localScale = new Vector3(1, 1, 1);
        goItem.transform.localScale = new Vector3(1, 1, 1);
    }

    public void Remove()
    {
        if (goItem == null)
        {
            return;
        }
        goItem.transform.parent = null;
        GameObject.Destroy(goItem);
    }
}