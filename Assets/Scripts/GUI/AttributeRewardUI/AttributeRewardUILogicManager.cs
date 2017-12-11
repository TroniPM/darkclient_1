using UnityEngine;
using System.Collections;

using Mogo.GameData;

public class AttributeRewardLogicManager
{
    public static bool hasInit = false;

    private static AttributeRewardLogicManager m_instance;

    public static AttributeRewardLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new AttributeRewardLogicManager();
            }

            return AttributeRewardLogicManager.m_instance;

        }
    }

    public void Initialize()
    {
        // SetAchievementGridNum();
        hasInit = true;
    }


    public void Release()
    {
    }

    public static class AttributeRewardUIEvent
    {
        public const string AttributeGridShareBtnUp = "AttributeRewardUI_AttributeGridShareBtnUp";
    }

    protected void SetAchievementGridNum()
    {
        //int count = RewardLoginData.dataMap.Count;
        //for (int i = 0; i < count; i++)
        //{
        //    SetDefaultGridInfo();
        //}

        AttributeRewardUIViewManager.Instance.EmptyLoginRewardGridList();

        foreach (var data in RewardAchievementData.dataMap)
        {
            SetGridInfo(data.Key, data.Value, 0);
        }
    }

    public void SetGridInfo()
    {
        AttributeRewardGridData ad = new AttributeRewardGridData();

        ad.Quest = "888888";
        ad.ProcessStatus = 88;
        ad.SignFGName = "bb_zuanshi";
        ad.SignText = "88";
        ad.Title = "88";

        ad.IsShare = true;
        ad.IsFinshed = false;
        ad.IsRunning = false;

        AttributeRewardUIViewManager.Instance.AddAttributeGrid(ad);
    }

    public void SetGridInfo(int id, RewardAchievementData data, int process, bool isShare = true, bool isFinshed = false, bool isRunning = false)
    {
        AttributeRewardGridData ad = new AttributeRewardGridData();

        ad.Quest = LanguageData.dataMap[data.text].Format(data.args[0]);
        ad.SignFGName = "bb_zuanshi";
        ad.SignText = data.diamond.ToString();
        ad.Title = LanguageData.dataMap[data.title].content;

        // 服务器数据
        // if ()
        ad.ProcessStatus = process * 100 / data.args[0] > 100 ? 100 : process * 100 / data.args[0];
        ad.ProcessText = process + "/" + data.args[0];
        ad.IsShare = isShare;
        ad.IsFinshed = isFinshed;
        ad.IsRunning = isRunning;

        AttributeRewardUIViewManager.Instance.AddAttributeGrid(ad, id);
    }

    public void SetGridIsShare(int id, bool isShare = true)
    {
        AttributeRewardUIViewManager.Instance.SetGridIsShare(id, isShare);
    }

    public void SetGridIsFinshed(int id, bool isFinshed = false)
    {
        AttributeRewardUIViewManager.Instance.SetGridIsFinshed(id, isFinshed);
    }

    public void SetGridIsRunning(int id, bool isRunning = false)
    {
        AttributeRewardUIViewManager.Instance.SetGridIsRunning(id, isRunning);
    }
}
