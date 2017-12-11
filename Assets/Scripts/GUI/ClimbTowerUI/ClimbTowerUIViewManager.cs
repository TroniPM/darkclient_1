/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ClimbTowerUIViewManager
// 创建者：Charles
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;
using System.Linq;
public class ClimbTowerUIViewManager : MogoParentUI
{

    #region 私有变量
    private Transform m_transform;
    private UILabel m_highHistory;
    private UILabel m_currentLevel;
    private UILabel m_leftChallengeCount;
    private UILabel m_lblGuide;
    private UILabel m_leftVIPSweepCount;
    private UIAnchor m_closeAnchor;
    private Transform m_rewardList;
    private Transform m_towerList;
    private Transform m_reportList;
    private GameObject m_report;
    private MogoTwoStatusButton m_buttonVIP;
    private MogoTwoStatusButton m_buttonNormal;
    private GameObject m_rewardTip;
    private GameObject m_bigRewardTip;
    private Transform m_rewardCamera;
    private Transform m_battleCamera;
    private UILabel m_rewardTitle;
    private uint m_timerID;
    private ReportData m_reportData;
    public bool m_sweepReady = false;
    private const int RowNum = 1;
    #endregion


    void Awake()
    {
        m_transform = transform;
        gameObject.SetActive(false);
        ClimbTowerUILogicManager.Instance.Initialize(this);
        m_rewardList = m_transform.Find("RightBox/RewardList");
        m_towerList = m_transform.Find("TowerList");
        m_report = m_transform.Find("report").gameObject;
        m_reportList = m_transform.Find("report/BattleReportList");
        m_rewardTitle = m_transform.Find("report/img/lblTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_highHistory = m_transform.Find("RightBox/historyHigh").GetComponentsInChildren<UILabel>(true)[0];
        //m_currentLevel = m_transform.FindChild("RightBox/TriPager/lblCurrentLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_leftChallengeCount = m_transform.Find("LeftBox/lblContainer/lblChallengeCount").GetComponentsInChildren<UILabel>(true)[0];
        m_lblGuide = m_transform.Find("LeftBox/lblContainer/lblGuide").GetComponentsInChildren<UILabel>(true)[0];
        m_leftVIPSweepCount = m_transform.Find("LeftBox/lblContainer/lblVIPSweepCount").GetComponentsInChildren<UILabel>(true)[0];
        m_closeAnchor = m_transform.Find("closeAnchor").GetComponentsInChildren<UIAnchor>(true)[0];
        m_buttonVIP = m_transform.Find("RightBox/btnTowerVIP").GetComponentsInChildren<MogoTwoStatusButton>(true)[0];
        m_buttonNormal = m_transform.Find("RightBox/btnTowerNormal").GetComponentsInChildren<MogoTwoStatusButton>(true)[0];
        AddButtonListener("onClicked", "RightBox/btnEnter", ClimbTowerUILogicManager.Instance.OnEnterMap);
        AddButtonListener("onClicked", "LeftBox/btnTowerCharge", ClimbTowerUILogicManager.Instance.Charge);
        AddButtonListener("onClicked", "RightBox/btnTowerNormal", ClimbTowerUILogicManager.Instance.NormalSweep);
        AddButtonListener("onClicked", "RightBox/btnTowerVIP", ClimbTowerUILogicManager.Instance.VIPSweep);
        AddButtonListener("onClicked", "closeAnchor/btnClose", Close);
        AddButtonListener("onClicked", "report/img/btnClose", closeReport);
        if (MogoUIManager.Instance.WaitingWidgetName == "btnEnter")
        {
            TimerHeap.AddTimer(100, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
        }

        //m_towerList.GetComponent<MogoListImproved>().SetGridLayout<TowerUnit>(7, m_towerList.transform, TowerLoaded);
    }
    void Start()
    {
        m_closeAnchor.uiCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        var m_rewardImg = m_transform.Find("report/img").gameObject;
        m_rewardImg.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
    }
    public void SetTowerGridLayout(Action callback)
    {
        m_rewardList.GetComponent<MogoListImproved>().SetGridLayout<TowerRewardUnit>(TowerXMLData.dataMap.Count, m_rewardList.transform, callback);
    }
    void onClick(int id)
    {
        if (id > 0)
        {
            List<string> reward = new List<string>();
            foreach (var item in TowerXMLData.dataMap[id].item)
            {
                reward.Add(ItemParentData.GetItem(item.Key).Name + "  x" + item.Value);
            }
            SetRewardGridTipData(reward, LanguageData.dataMap.Get(20013).Format(id));
            ShowRewardGridTip(true);
        }

    }
    void onTowerTip(int id)
    {
        if (id > 0)
        {
            List<string> reward = new List<string>();
            for (int i = 0; i < 1; i++)
            {
                reward.Add(LanguageData.GetContent(20300 + id / 10));
            }
            SetBigRewardGridTipData(reward, LanguageData.GetContent(20018, LanguageData.GetContent(20100 + id / 10)));
            ShowBigRewardGridTip(true);
        }

    }
    void Close()
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    public void SetView(TowerData data)
    {
        int vipLevel = MogoWorld.thePlayer.VipLevel;
        TimerManager.GetTimer(m_buttonNormal.gameObject).StartTimer(data.CountDown,
            (sec) => { m_buttonNormal.GetComponentsInChildren<UILabel>(true)[0].text = String.Concat((sec / 3600),":",(sec % 3600) / 60, ":", ((sec % 3600) % 60)); m_sweepReady = true; },
            () => { m_buttonNormal.GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.dataMap.Get(810).content; m_sweepReady = false; });
        m_highHistory.text = LanguageData.GetContent(801, data.Highest);
        //m_currentLevel.text = LanguageData.GetContent(802, data.CurrentLevel);

        if (vipLevel == 0)
        {
            //非VIP
            m_buttonVIP.SetButtonText(LanguageData.GetContent(808));
            m_leftVIPSweepCount.text = LanguageData.GetContent(20019, vipLevel + 1, PrivilegeData.dataMap[vipLevel + 1].dailyTowerSweepLimit);
        }
        else
        {
            int leftCount = PrivilegeData.dataMap[vipLevel].dailyTowerSweepLimit - data.VIPSweepUsed;
            m_buttonVIP.SetButtonText(LanguageData.GetContent(807, leftCount));

            if (vipLevel >= GlobalData.dataMap[0].tower_all_sweep_vip_level)
            {
                m_leftVIPSweepCount.text = LanguageData.GetContent(20021, vipLevel);
                //隐藏普通扫荡，vip扫荡改为全部扫荡
                m_buttonNormal.gameObject.SetActive(false);
                m_buttonVIP.SetButtonText(LanguageData.GetContent(809));
            }
            else if (vipLevel + 1 == GlobalData.dataMap[0].tower_all_sweep_vip_level)
            {
                m_leftVIPSweepCount.text = LanguageData.GetContent(20020, vipLevel + 1);
            }
            else
            {
                m_leftVIPSweepCount.text = LanguageData.GetContent(20019, vipLevel + 1, PrivilegeData.dataMap[vipLevel + 1].dailyTowerSweepLimit);
            }

        }
        m_leftChallengeCount.text = LanguageData.GetContent(803, data.FailCount);
        m_lblGuide.text = LanguageData.dataMap.Get(vipLevel + 20006).content;

        if (data.CurrentLevel > data.Highest)
        {
            m_buttonVIP.Clickable = false;
            m_buttonNormal.Clickable = false;
        }
        else
        {
            m_buttonVIP.Clickable = true;
            m_buttonNormal.Clickable = true;
        }
        int iter = 0;
        var rwdList = m_rewardList.GetComponentsInChildren<MogoListImproved>(true);
        if (rwdList[0].DataList.Count > 0)
        {
            foreach (var towerXML in TowerXMLData.dataMap)
            {
                TowerRewardUnit unit = (TowerRewardUnit)(rwdList[0].DataList[iter]);
                if (data.Items.ContainsValue(towerXML.Key))
                {
                    unit.IsAlreadyGet = true;
                }
                else
                {
                    unit.IsAlreadyGet = false;
                }
                unit.force = LanguageData.GetContent(20099, MissionData.dataMap.FirstOrDefault(
                    x =>
                        MapData.dataMap.Get(x.Value.mission).type == MapType.ClimbTower
                        && x.Value.difficulty == unit.RewardID - 9)
                        .Value.minimumFight.ToString());
                unit.PicID = (unit.RewardID / 10 - 1) % 3;

                if (((data.CurrentLevel % 10 == 0 ? data.CurrentLevel : (data.CurrentLevel / 10 + 1) * 10) == unit.RewardID))
                {
                    unit.TowerDesc = LanguageData.GetContent(802, data.CurrentLevel);
                    unit.forceCurrent = LanguageData.GetContent(20099,
                        (MissionData.dataMap.FirstOrDefault(x =>
                        MapData.dataMap.Get(x.Value.mission).type == MapType.ClimbTower
                        && x.Value.difficulty == data.CurrentLevel)
                        .Value.minimumFight).ToString());
                    unit.Grey = false;
                    unit.HighLight = true;
                    unit.Unlocked = true;
                }
                else if (((data.CurrentLevel % 10 == 0 ? data.CurrentLevel : (data.CurrentLevel / 10 + 1) * 10) < unit.RewardID))
                {
                    unit.TowerDesc = string.Empty;
                    unit.Grey = true;// true;
                    unit.Unlocked = false;
                    unit.HighLight = false;
                }
                else
                {
                    unit.TowerDesc = string.Empty;
                    unit.Grey = false;
                    unit.HighLight = false;
                    unit.Unlocked = true;
                }

                iter++;
            }
        }
        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
    }

    void SetRewardGridTipData(List<string> tipData, string title = "")
    {
        m_rewardTip.GetComponentsInChildren<RewardGridTip>(true)[0].ListReward = tipData;
        m_rewardTip.GetComponentsInChildren<RewardGridTip>(true)[0].TipTitle = title;
    }
    void SetBigRewardGridTipData(List<string> tipData, string title = "")
    {
        m_bigRewardTip.GetComponentsInChildren<RewardGridTip>(true)[0].ListReward = tipData;
        m_bigRewardTip.GetComponentsInChildren<RewardGridTip>(true)[0].TipTitle = title;
    }
    void ShowRewardGridTip(bool isShow)
    {
        m_rewardTip.SetActive(isShow);
    }
    void ShowBigRewardGridTip(bool isShow)
    {
        m_bigRewardTip.SetActive(isShow);
    }
    void TowerLoaded()
    {

    }
    public void ResourceLoaded()
    {
        m_rewardCamera = m_transform.Find("RightBox/RewardList/RewardListCamera");
        if (m_rewardTip == null)
        {
            m_rewardTip = m_transform.Find("RightBox/RewardTip").gameObject;
            m_rewardTip.AddComponent<RewardGridTip>().TipNum = 4;
        }
        if (m_bigRewardTip == null)
        {
            m_bigRewardTip = m_transform.Find("RightBox/BigRewardTip").gameObject;
            m_bigRewardTip.AddComponent<RewardGridTip>().TipNum = 1;
        }

        int iter = 0;
#if UNITY_IPHONE
		foreach (var towerXML in TowerXMLData.dataMap.SortByKey())
#else
        foreach (var towerXML in TowerXMLData.dataMap.OrderBy(x => x.Key))
#endif   
		{
            TowerRewardUnit unit = (TowerRewardUnit)(m_rewardList.GetComponentsInChildren<MogoListImproved>(true)[0].DataList[iter]);
            unit.RewardID = towerXML.Key;
            unit.TowerName = LanguageData.GetContent(20100 + unit.RewardID / 10);
            unit.clickHandler = onClick;
            unit.towerTipHandler = onTowerTip;
            //if (ClimbTowerUILogicManager.Instance.Data.Items.ContainsValue(towerXML.Key))
            //{
            //    unit.IsAlreadyGet = true;
            //}
            //else
            //{
            //    unit.IsAlreadyGet = false;
            //}
            unit.RewardName = LanguageData.GetContent(20200 + unit.RewardID / 10);
            unit.icon = TowerXMLData.dataMap.Get(towerXML.Key).icon;
            iter++;
        }
    }

    void closeReport()
    {
        m_report.SetActive(false);
    }
    void OpenReport()
    {
        m_report.SetActive(true);
    }

    public void GenerateBattleReport(ReportData data)
    {
        m_reportData = data;
        m_reportList.GetComponentsInChildren<MogoListImproved>(true)[0].SetGridLayout<BattleReportUnit>((int)Math.Ceiling((double)data.Items.Count / RowNum) + 2, m_reportList.transform, ReportResourceLoaded);
    }
    void ReportResourceLoaded()
    {

        var m_reportDataList = m_reportList.GetComponentsInChildren<MogoListImproved>(true)[0].DataList;
        int len = m_reportDataList.Count;
        for (int i = 0; i < len; i++)
        {
            BattleReportUnit unit = (BattleReportUnit)m_reportDataList[i];
            if (i == 0)
            {
                unit.content = LanguageData.dataMap.Get(20014).content;
                unit.head = true;
                unit.line = false;
            }
            else if (i == 1)
            {
                if (m_reportData.money > 0)
                {
                    unit.content = String.Concat(LanguageData.MONEY, String.Format("x{0}", m_reportData.money), "  ",
                        LanguageData.EXP, String.Format("x{0}", m_reportData.exp));
                }
                else
                {
                    unit.content = String.Concat(LanguageData.EXP, String.Format("x{0}", m_reportData.exp));
                }
                unit.head = false;
                unit.line = false;
            }
            else if (i > 1 && i < len)
            {
                string[] temp = (from x in m_reportData.Items select String.Concat(ItemParentData.GetItem(x.Key).Name, "x", x.Value)).ToArray();

                unit.head = false;
                if (i == len - 1) //last one
                {
                    unit.content = String.Join(",", temp, RowNum * (i - 2), (len - 2) % RowNum == 0 ? RowNum : (len - 2) % RowNum);
                    unit.line = true;
                }
                else
                {
                    unit.content = String.Join(",", temp, RowNum * (i - 2), RowNum);
                    unit.line = false;
                }

            }
            else
            {
                LoggerHelper.Error("Calculate Error");
            }

        }
        m_rewardTitle.text = LanguageData.dataMap.Get(20015).Format(MogoWorld.thePlayer.name);
        OpenReport();
    }
}

#if UNITY_IPHONE
public static partial class IOSCSharpExtension
{
	//cannot be generic extension like Dictionary<int,T> SortByKey<T>();
	//because this would cause a JIT runtime exception
	public static Dictionary<int,TowerXMLData> SortByKey(this Dictionary<int,TowerXMLData> dic)
	{
		Dictionary<int,TowerXMLData> 	dicRet=new Dictionary<int, TowerXMLData>();
		List<int> 				order=new List<int>();
		foreach(var v in dic)
		{
			order.Add(v.Key);
		}
		order.Sort();
		foreach(var v in order)
		{
			dicRet.Add(v,dic[v]);
		}
		return dicRet;
	}
}
#endif
