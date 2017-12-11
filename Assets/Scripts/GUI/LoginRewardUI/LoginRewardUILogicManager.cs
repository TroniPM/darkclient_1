using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.GameData;
using Mogo.Util;
using Mogo.Game;


public class LoginRewardUILogicManager
{
    public int loginCircleDay = -1;
    public int loginMarketCircleDay = -1;

    public int lastPage = -1;

    protected int gridLoadedDone = 0;
    public bool isOldServer = false;

    protected int defaultLoginCircleDays = 0;

    private static LoginRewardUILogicManager m_instance;

    public static LoginRewardUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new LoginRewardUILogicManager();
            }

            return LoginRewardUILogicManager.m_instance;

        }
    }

    public void Initialize()
    {
        lastPage = -1;
        gridLoadedDone = 0;

        List<int> temp = new List<int>();
        foreach (var data in RewardLoginData.dataMap)
        {
            if (!temp.Contains(data.Value.days))
            {
                temp.Add(data.Value.days);
            }
        }

        defaultLoginCircleDays = temp.Count;
        // defaultLoginMarketCircleDay = LoginMarketData.dataMap.Count;

        LoggerHelper.Debug("defaultLoginCircleDays: " + defaultLoginCircleDays);
        // LoggerHelper.Debug("defaultLoginMarketCircleDay: " + defaultLoginMarketCircleDay);

        EventDispatcher.AddEventListener<int>("LoadLoginRewardGirdDone", CountGridLoadedDone);
    }

    public void Release()
    {
        loginCircleDay = -1;
        loginMarketCircleDay = -1;

        lastPage = -1;

        gridLoadedDone = 0;
        isOldServer = false;

        defaultLoginCircleDays = 0;

        EventDispatcher.RemoveEventListener<int>("LoadLoginRewardGirdDone", CountGridLoadedDone);
    }

    //public void SetLoginGridNum()
    //{
    //    //int count = RewardLoginData.dataMap.Count;
    //    //for (int i = 0; i < count; i++)
    //    //{
    //    //    SetDefaultGridInfo();
    //    //}

    //    LoginRewardUIViewManager.Instance.EmptyLoginRewardGridList();

    //    foreach (var data in RewardLoginData.dataMap)
    //    {
    //        SetGridInfo(data.Key, data.Value);
    //    }
    //}

    protected void CountGridLoadedDone(int id)
    {
        if (gridLoadedDone != defaultLoginCircleDays)
        {
            gridLoadedDone++;
            LoggerHelper.Debug(gridLoadedDone);
            if (gridLoadedDone == defaultLoginCircleDays)
            {
                gridLoadedDone = 0;
                ShowLoginGrid();
            }
        }
    }

    protected void SetDefaultGridInfo()
    {
        LoginRewardGridData ld = new LoginRewardGridData();

        ld.LeftGetSignImgName = "jl_yiwancheng";
        ld.RightGetSignImgName = "jl_yiwancheng";

        ld.LeftGetSignImgName = "88";
        ld.NewServerRightText = "88";
        ld.OldServerCostSignImgName = "88";
        ld.OldServerCostText = "88";
        ld.OldServerItemCD = "88";
        ld.OldServerItemFGImgName = 88;
        ld.OldServerItemName = "88";
        ld.OldServerItemNum = "88";
        ld.OldServerRightText = "88";

        ld.IsOldServer = isOldServer;

        ld.ListLeftItem = new List<KeyValuePair<int, int>>();
        ld.ListLeftItem.Add(new KeyValuePair<int, int>(0, 0));

        LoginRewardUIViewManager.Instance.AddLoginRewardGrid(ld);
    }

    public void SetGridInfo(int dataID, RewardLoginData data)
    {
        LoginRewardGridData ld = new LoginRewardGridData();

        ld.LeftGetSignImgName = "jl_yiwancheng";
        ld.RightGetSignImgName = "jl_yiwancheng";

        ld.IsOldServer = !isOldServer;
        ld.ListLeftItem = new List<KeyValuePair<int, int>>();
        ld.ListLeftItemID = new List<int>();

        Dictionary<int, int> items;
        switch(MogoWorld.thePlayer.vocation)
        {
            case Vocation.Warrior:
                items = data.items1;
                break;
            case Vocation.Assassin:
                items = data.items2;
                break;
            case Vocation.Archer:
                items = data.items3;
                break;
            case Vocation.Mage:
                items = data.items4;
                break;
            default:
                items = new Dictionary<int, int>();
                break;
        }

        foreach (var item in items)
        {
            if (ItemParentData.GetItem(item.Key) != null)
            {
                ld.ListLeftItemID.Add(item.Key);
                ld.ListLeftItem.Add(item);
            }
            else
                LoggerHelper.Debug("Item" + item.Key + " Not Exist");
        }

        if (data.exp > 0)
        {
            ld.ListLeftItemID.Add(1);
            ld.ListLeftItem.Add(new KeyValuePair<int, int>(1, data.exp));
        }

        if (data.gold > 0)
        {
            ld.ListLeftItemID.Add(2);
            ld.ListLeftItem.Add(new KeyValuePair<int, int>(2, data.gold));
        }

        if (data.energy > 0)
        {
            ld.ListLeftItemID.Add(6);
            ld.ListLeftItem.Add(new KeyValuePair<int, int>(6, data.energy));
        }

        if (true)
        {
            //Mogo.Util.LoggerHelper.Debug("loginMarketCircleDay" + loginMarketCircleDay);

            if (lastPage == -1 || loginCircleDay == -1 || loginMarketCircleDay == -1)
            {
                loginCircleDay = MogoWorld.thePlayer.login_days == 0 ? 1 : (int)(MogoWorld.thePlayer.login_days % defaultLoginCircleDays == 0 ? defaultLoginCircleDays : MogoWorld.thePlayer.login_days % defaultLoginCircleDays); // to do

                // loginMarketCircleDay = MogoWorld.thePlayer.login_days == 0 ? 1 : (int)(MogoWorld.thePlayer.login_days % DefaultLoginMarketCircleDay); // to do
                loginMarketCircleDay = MogoTime.Instance.GetCurrentDateTime().Day;
            }

            // if (data.days == loginCircleDay)
            //{
                int dayOffset = data.days - loginCircleDay;
                System.DateTime tempTime = MogoTime.Instance.GetCurrentDateTime().AddDays(dayOffset);

                if (LoginMarketData.dataMap.ContainsKey(tempTime.Day))
                {
                    var marketData = LoginMarketData.dataMap[tempTime.Day];

                    if (marketData.priceType == 1)
                        ld.OldServerCostSignImgName = "bb_zuanshi";
                    else
                        ld.OldServerCostSignImgName = "bb_zuanshi";

                    var item = ItemParentData.GetItem(marketData.itemId);
                    if (item != null)
                    {
                        ld.OldServerItemFGImgID = marketData.itemId;
                        ld.OldServerItemFGImgName = marketData.itemId;
                        ld.OldServerItemName = item.Name;
                        ld.OldServerRightText = LanguageData.GetContent(47006); // "登陆特惠限购"
                    }
                    else
                    {
                        LoggerHelper.Debug("Item " + marketData.itemId + " Not Exit");
                    }

                    ld.OldServerCostText = marketData.price.ToString();
                }
        }
        //为消除警告而注释以下代码
        //else
        //{
        //    if (data.extra_items != null)
        //    {
        //        ld.NewServerRightText = "新服奖励";
        //        ld.ListRightItem = new List<string>();
        //        foreach (var extra_item in data.extra_items)
        //        {
        //            ld.ListRightItem.Add(ItemParentData.GetItem(extra_item.Key).Icon);
        //        }
        //    }
        //}

        //Mogo.Util.LoggerHelper.Debug("isOldServer" + ld.ListLeftItem.Count);

        LoginRewardUIViewManager.Instance.AddLoginRewardGrid(ld, data.days);
    }

    public void ShowLoginGrid()
    {
        if (lastPage == -1 || loginCircleDay == -1 || loginMarketCircleDay == -1)
        {
            loginCircleDay = MogoWorld.thePlayer.login_days == 0 ? 1 : (int)(MogoWorld.thePlayer.login_days % defaultLoginCircleDays == 0 ? defaultLoginCircleDays : MogoWorld.thePlayer.login_days % defaultLoginCircleDays); // to do

            // loginMarketCircleDay = MogoWorld.thePlayer.login_days == 0 ? 1 : (int)(MogoWorld.thePlayer.login_days % DefaultLoginMarketCircleDay); // to do

            loginMarketCircleDay = MogoTime.Instance.GetCurrentDateTime().Day;

            lastPage = loginCircleDay;
        }

        LoginRewardUIViewManager.Instance.SetLoginTitle(loginCircleDay, true);
        LoginRewardUIViewManager.Instance.SetCurrentPage(lastPage - 1);

        ShowButton();
    }

    public void ShowLoginGrid(int page)
    {
        if (loginCircleDay == -1 || loginMarketCircleDay == -1)
        {
            loginCircleDay = MogoWorld.thePlayer.login_days == 0 ? 1 : (int)(MogoWorld.thePlayer.login_days % defaultLoginCircleDays == 0 ? defaultLoginCircleDays : MogoWorld.thePlayer.login_days % defaultLoginCircleDays); // to do

            // loginMarketCircleDay = MogoWorld.thePlayer.login_days == 0 ? 1 : (int)(MogoWorld.thePlayer.login_days % DefaultLoginMarketCircleDay); // to do
            loginMarketCircleDay = MogoTime.Instance.GetCurrentDateTime().Day;
        }

        LoginRewardUIViewManager.Instance.SetLoginTitle(loginCircleDay, true);
        LoginRewardUIViewManager.Instance.SetCurrentPage(page - 1);

        ShowButton();
    }

    protected void ShowButton()
    {
        // 设置左侧登录奖励和领取按钮
        if (!MogoWorld.thePlayer.IsLoginRewardHasGot)
        {
            LoggerHelper.Debug("ShowLoginGrid IsLoginRewardHasGot True");
            LoginRewardUIViewManager.Instance.ShowLoginGotButtom(loginCircleDay, true);
            LoginRewardUIViewManager.Instance.ShowLeftGetSign(loginCircleDay, false);
            LoginRewardUIViewManager.Instance.ShowRightTitle(loginCircleDay);
        }
        else
        {
            LoggerHelper.Debug("ShowLoginGrid IsLoginRewardHasGot False");
            LoginRewardUIViewManager.Instance.ShowLoginGotButtom(loginCircleDay, false);
            LoginRewardUIViewManager.Instance.ShowLeftGetSign(loginCircleDay, true);
            LoginRewardUIViewManager.Instance.ShowRightTitle(loginCircleDay);
        }

        // 设置右侧限时购买和购买按钮
        if ((MogoTime.Instance.GetCurrentDateTime().Year == MogoTime.Instance.GetDateTimeBySecond((int)MogoWorld.thePlayer.buyHotSalesLastTime).Year && MogoTime.Instance.GetCurrentDateTime().DayOfYear != MogoTime.Instance.GetDateTimeBySecond((int)MogoWorld.thePlayer.buyHotSalesLastTime).DayOfYear)
            || (MogoTime.Instance.GetCurrentDateTime().Year != MogoTime.Instance.GetDateTimeBySecond((int)MogoWorld.thePlayer.buyHotSalesLastTime).Year))
        {
            LoginRewardUIViewManager.Instance.ShowLoginBuyButtom(loginCircleDay, true);
            LoginRewardUIViewManager.Instance.ShowRightBuySign(loginCircleDay, false);
        }
        else
        {
            LoginRewardUIViewManager.Instance.ShowLoginBuyButtom(loginCircleDay, false);
            LoginRewardUIViewManager.Instance.ShowRightBuySign(loginCircleDay, true);
        }
    }

    #region 1.领取登录奖励；2.购买登录商品

    /// <summary>
    /// 领取登录奖励
    /// </summary>
    public void SetRewardGot()
    {
        PlayGetRewardSignAnim(); // 播放"已领取"动画
        LoginRewardUIViewManager.Instance.ShowLoginGotButtom(loginCircleDay, false); // 隐藏领取按钮
    }

    /// <summary>
    /// 播放"已领取"动画
    /// </summary>
    public void PlayGetRewardSignAnim()
    {
        LoggerHelper.Debug("OperationSystem LogInGetReward" + lastPage);
        LoginRewardUIViewManager.Instance.PlayGetSignAnim(true, lastPage - 1);
    }

    /// <summary>
    /// 购买登录商品
    /// </summary>
    public void SetItemBought()
    {
        PlayBuySignAnim(); // 播放"已购买"动画
        LoginRewardUIViewManager.Instance.ShowLoginBuyButtom(loginCircleDay, false); // 隐藏购买按钮
    }

    /// <summary>
    /// 播放"已购买"动画
    /// </summary>
    public void PlayBuySignAnim()
    {
        LoggerHelper.Debug("OperationSystem LogInBuy" + lastPage);
        LoginRewardUIViewManager.Instance.PlayGetSignAnim(false, lastPage - 1);
    }

    #endregion

    public void SetTitleByJudgeDay(int judgeDay)
    {
        LoginRewardUIViewManager.Instance.SetLoginTitle(judgeDay, (judgeDay == loginCircleDay));
    }
}
