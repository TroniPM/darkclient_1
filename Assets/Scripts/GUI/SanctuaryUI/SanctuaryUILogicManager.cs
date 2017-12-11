using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Mogo.Util;
using Mogo.GameData;
using Mogo.Game;

public class MyRewardData
{
    public int rewardID;
    public string name;
    public int needScore; // 贡献需要达到的分数
    public string progress; // 进度
    public string icon; // 图标
    public bool isEnable; // 是否已经可以领取
    public bool isAlreadyGet; // 是否已经领取过
}

public static class SanctuaryUIEvent
{
    public readonly static string GetMyReward = "SanctuaryUIEvent.GetMyReward"; // 领取我的贡献奖励
}

public class SanctuaryUILogicManager
{
    private static SanctuaryUILogicManager m_instance;

    public static SanctuaryUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SanctuaryUILogicManager();
            }

            return SanctuaryUILogicManager.m_instance;

        }
    }

    public void Initialize()
    {
        EventDispatcher.AddEventListener<int>(SanctuaryUIEvent.GetMyReward, GetMyReward);
    }

    public void Release()
    {
        EventDispatcher.RemoveEventListener<int>(SanctuaryUIEvent.GetMyReward, GetMyReward);
    }

    public uint dayContri { get; set; }
    public uint weekContri { get; set; }
    public uint weekLevel { get; set; }
    private List<int> m_alreadyGetList = new List<int>();
    public List<int> alreadyGetList
    {
        get { return m_alreadyGetList; }
        set { m_alreadyGetList = value; }
    }
    public uint MyWeek { get; set; }
    public uint MyDay { get; set; }
    public List<SanctuaryRankData> weekData { get; set; }
    public List<SanctuaryRankData> dayData { get; set; }
    public List<SanctuaryRankData> battleData { get; set; }

    public void RefreshBattleUI()
    {
        MainUIViewManager.Instance.SetContributeRankText(String.Format(LanguageData.GetContent(46910), "：")); // 当前贡献排名{0}
        if (battleData.Count >= 1)
        {
            // 第1名 : {0}  {1}
            MainUIViewManager.Instance.SetFirstRank(String.Format(LanguageData.GetContent(46911), battleData[0].name, battleData[0].contribution));
        }
        if (battleData.Count >= 2)
        {
            // 第2名 : {0}  {1}
            MainUIViewManager.Instance.SetSecRank(String.Format(LanguageData.GetContent(46912), battleData[1].name, battleData[1].contribution));
        }
        if (battleData.Count >= 3)
        {
            // 第3名 : {0}  {1}
            MainUIViewManager.Instance.SetTriRank(String.Format(LanguageData.GetContent(46913), battleData[2].name, battleData[2].contribution));
        }

        MainUIViewManager.Instance.ShowContributeRankDialog(true);
    }
    public void RefreshUI(int page)
    {
        switch (page)
        {
            case 0:
                {
                    //SanctuaryUIViewManager.Instance.SetCurrentAchieve(weekContri.ToString());
                    //SanctuaryUIViewManager.Instance.SetNextAchieve(MyInfo.nextLvNeedContribution.ToString());
                    //SanctuaryUIViewManager.Instance.SetAcieveReward(SanctuaryRewardXMLData.GetAccuNextRankIcon(weekContri));
                    //SanctuaryUIViewManager.Instance.SetNextAchievementRewardGoldNum(SanctuaryRewardXMLData.GetAccuNextGold(weekContri));
                    var rwd = new List<MyRewardData>();
                    foreach (var item in SanctuaryRewardXMLData.dataMap.
                        Where(x => x.Value.type == 3 && x.Value.level[0] <= weekLevel
                        && weekLevel <= x.Value.level[1]))
                    {
                        if (weekContri >= item.Value.contribution)
                        {
                            //可以领取
                            if (alreadyGetList.Contains(item.Key))
                            {
                                //已经领取过了
                                rwd.Add(new MyRewardData()
                                {
                                    name = string.Concat(ItemParentData.GetItem((int)ItemCode.GOLD).Name, " x ", item.Value.gold),
                                    rewardID = item.Key,
                                    icon = IconData.dataMap.Get(item.Value.icon).path,
                                    isAlreadyGet = true,
                                    isEnable = true,
                                    needScore = item.Value.contribution,
                                    progress = (LanguageData.GetContent(46915, String.Concat(weekContri, '/', item.Value.contribution)))
                                });
                            }
                            else
                            {

                                rwd.Add(new MyRewardData()
                                {
                                    name = string.Concat(ItemParentData.GetItem((int)ItemCode.GOLD).Name, " x ", item.Value.gold),
                                    rewardID = item.Key,
                                    icon = IconData.dataMap.Get(item.Value.icon).path,
                                    isAlreadyGet = false,
                                    isEnable = true,
                                    needScore = item.Value.contribution,
                                    progress = (LanguageData.GetContent(46915, String.Concat(weekContri, '/', item.Value.contribution)))
                                });

                            }
                        }
                        else
                        {

                            rwd.Add(new MyRewardData()
                            {
                                name = string.Concat(ItemParentData.GetItem((int)ItemCode.GOLD).Name, " x ", item.Value.gold),
                                rewardID = item.Key,
                                icon = IconData.dataMap.Get(item.Value.icon).path,
                                isAlreadyGet = false,
                                isEnable = false,
                                needScore = item.Value.contribution,
                                progress = (LanguageData.GetContent(46915, String.Concat(weekContri, '/', item.Value.contribution)))
                            });
                        }


                    }

                    SanctuaryUIViewManager.Instance.GenerateMyRewardList(rwd);
                }
                break;
            case 1:
                {
                    SanctuaryUIViewManager.Instance.ClearRankGridList();
                    for (int i = 0; i < weekData.Count; i++)
                    {
                        RankGridData data = new RankGridData();
                        data.achieve = weekData[i].contribution.ToString();
                        data.name = weekData[i].name;
                        data.rank = (i + 1).ToString();
                        if (data.name.Equals(MogoWorld.thePlayer.name))
                        {
                            data.highLight = true;
                        }
                        else
                        {
                            data.highLight = false;
                        }
                        SanctuaryUIViewManager.Instance.AddRankGrid(data, i);
                    }
                    SanctuaryUIViewManager.Instance.SetPlayerName(MogoWorld.thePlayer.name);
                    SanctuaryUIViewManager.Instance.SetPlayerRank(MyWeek.ToString());
                    SanctuaryUIViewManager.Instance.SetPlayerContribute(weekContri.ToString());
                    var weekRank = SanctuaryRewardXMLData.GetWeekRankID();
                    SanctuaryUIViewManager.Instance.ClearRewardGridList();

                    for (int i = 0; i < weekRank.Count; i++)
                    {
                        RankRewardGridData rrgd = new RankRewardGridData();
                        rrgd.imgName = IconData.dataMap.Get(SanctuaryRewardXMLData.dataMap.Get(weekRank[i]).icon).path;
                        rrgd.text = String.Format("第{0}名宝箱", i + 1);
                        SanctuaryUIViewManager.Instance.AddRewardGrid(rrgd);
                    }
                    break;
                }
            case 2:
                {
                    SanctuaryUIViewManager.Instance.ClearRankGridList();
                    for (int i = 0; i < dayData.Count; i++)
                    {
                        RankGridData data = new RankGridData();
                        data.achieve = dayData[i].contribution.ToString();
                        data.name = dayData[i].name;
                        data.rank = (i + 1).ToString();
                        if (data.name.Equals(MogoWorld.thePlayer.name))
                        {
                            data.highLight = true;
                        }
                        else
                        {
                            data.highLight = false;
                        }
                        SanctuaryUIViewManager.Instance.AddRankGrid(data, i);
                    }
                    SanctuaryUIViewManager.Instance.SetPlayerName(MogoWorld.thePlayer.name);
                    SanctuaryUIViewManager.Instance.SetPlayerRank(MyDay.ToString());
                    SanctuaryUIViewManager.Instance.SetPlayerContribute(dayContri.ToString());
                    var dayRank = SanctuaryRewardXMLData.GetDayRankID();
                    SanctuaryUIViewManager.Instance.ClearRewardGridList();

                    for (int i = 0; i < dayRank.Count; i++)
                    {
                        RankRewardGridData rrgd = new RankRewardGridData();
                        rrgd.imgName = IconData.dataMap.Get(SanctuaryRewardXMLData.dataMap.Get(dayRank[i]).icon).path;
                        rrgd.text = String.Format("第{0}名宝箱", i + 1);
                        SanctuaryUIViewManager.Instance.AddRewardGrid(rrgd);

                    }
                    break;
                }
            default:
                break;
        }
    }

    /// <summary>
    /// 领取我的贡献奖励
    /// </summary>
    /// <param name="rewardID"></param>
    private void GetMyReward(int rewardID)
    {
        MogoWorld.thePlayer.RpcCall("GetWeekCtrbuRewardReq", rewardID);
    }
}
