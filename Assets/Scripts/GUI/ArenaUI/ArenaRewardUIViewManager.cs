using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using System.Linq;
using Mogo.GameData;

public enum ArenaRewardType
{
    RewardDay = 1, // �ջ���
    RewardWeek = 2, // �ܻ���
}

public class ArenaRewardUIViewManager : MogoParentUI
{
    #region Private
    private Transform m_transform;
    private GameObject m_head;
    private Transform m_rewardList;
    #endregion

    void Awake()
    {
        m_transform = transform;
        m_transform.localPosition = new Vector3(3000, 0, 0);

        m_rewardList = m_transform.Find("RewardList");
       
        // ����SourceCamera
        Camera sourceCamera = m_transform.Find("ArenaRewardUICamera").GetComponentsInChildren<Camera>(true)[0];
        m_rewardList.GetComponentsInChildren<MogoListImproved>(true)[0].SourceCamera = sourceCamera;

        AddButtonListener("OnClicked", "btnClose", OnClose);

        ArenaRewardUILogicManager.Instance.Initialize(this);
        gameObject.SetActive(false);
    }
    public void OnClose()
    {
        gameObject.SetActive(false);
        //MogoUIManager.Instance.OpenWindow((int)WindowName.Arena,() =>{});
    }

    /// <summary>
    /// ��������(5000����Ϊ0,5000������10000����Һ;������⴦��)
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public int RoundingNum(int num)
    {
        if (num / 10000.0f - (int)(num / 10000) >= 0.5)
            num = (int)((num / 10000) + 1) * 10000;
        else
            num = (int)(num / 10000) * 10000;

        return num;
    }

    struct SortRewardData
    {
        public int rewardID;
        public int needScore;
    }

    /// <summary>
    /// ���еĽ�������
    /// </summary>
    /// <param name="hasGetArenaRewardList"></param>
    /// <param name="allRewardDataList"></param>
    public List<int> SortAllReward(List<int> hasGetArenaRewardList, Dictionary<int, ArenaRewardData> allRewardDataList)
    {
        List<int> allSortRewardList = new List<int>();
        List<SortRewardData> canGetSortRewardList = new List<SortRewardData>();// δ��ȡ�Ľ����б�
        List<SortRewardData> doingSortRewardList = new List<SortRewardData>();// �����еĽ����б�
        List<SortRewardData> hasGetSortRewardList = new List<SortRewardData>();// ����ȡ�Ľ����б�

        // ����(1)δ��ȡ(2)������(3)����ȡ
        // ͬһ����а��������������ִ�С��С��������ע�������в������ջ��ֺ��ܻ��֣�

        // δ��ȡ����
        foreach (KeyValuePair<int, ArenaRewardData> rewardPair in allRewardDataList)
        {
            if (hasGetArenaRewardList.Contains(rewardPair.Key))
                continue;

            ArenaScoreRewardData arenaScoreRewardData = ArenaScoreRewardData.dataMap.Get(rewardPair.Key);
            if (arenaScoreRewardData == null)
                continue;

            SortRewardData sortRewardData;
            sortRewardData.rewardID = rewardPair.Key;
            sortRewardData.needScore = arenaScoreRewardData.score;
            switch ((ArenaRewardType)arenaScoreRewardData.type)
            {
                case ArenaRewardType.RewardDay:
                    {                         
                        if (ArenaUILogicManager.Instance.m_ObtainDayPoint >= arenaScoreRewardData.score)
                            canGetSortRewardList.Add(sortRewardData);
                        else
                            doingSortRewardList.Add(sortRewardData);
                    } break;
                case ArenaRewardType.RewardWeek:
                    {
                        if (ArenaUILogicManager.Instance.m_ObtainWeekPoint >= arenaScoreRewardData.score)
                            canGetSortRewardList.Add(sortRewardData);
                        else
                            doingSortRewardList.Add(sortRewardData);
                    } break;
            }
        }

        // ����ȡ����
        foreach (int rewardID in hasGetArenaRewardList)
        {
            ArenaScoreRewardData arenaScoreRewardData = ArenaScoreRewardData.dataMap.Get(rewardID);
            if (arenaScoreRewardData == null)
                continue;

            SortRewardData sortRewardData;
            sortRewardData.rewardID = rewardID;
            sortRewardData.needScore = arenaScoreRewardData.score;

            hasGetSortRewardList.Add(sortRewardData);
        }

        // δ��ȡ
        canGetSortRewardList.Sort(delegate(SortRewardData a, SortRewardData b)
        {
            if (a.needScore < b.needScore)
                return -1;
            else
                return 1;
        });
        foreach (SortRewardData rewardData in canGetSortRewardList)
        {
            allSortRewardList.Add(rewardData.rewardID);
        }

        // ������
        doingSortRewardList.Sort(delegate(SortRewardData a, SortRewardData b)
        {
            if (a.needScore < b.needScore)
                return -1;
            else
                return 1;
        });
        foreach (SortRewardData rewardData in doingSortRewardList)
        {
            allSortRewardList.Add(rewardData.rewardID);
        }

        // ����ȡ
        hasGetSortRewardList.Sort(delegate(SortRewardData a, SortRewardData b)
        {
            if (a.needScore< b.needScore)
                return -1;
            else
                return 1;
        });
        foreach (SortRewardData rewardData in hasGetSortRewardList)
        {
            allSortRewardList.Add(rewardData.rewardID);
        }

        return allSortRewardList;
    }

    /// <summary>
    /// ʹ�÷�������������
    /// </summary>
    /// <param name="hasGetArenaRewardList">����ȡ�Ľ����б�</param>
    /// <param name="allRewardDataList">���еĽ����б�</param>
    /// <param name="callback"></param>
    public void AddRewardUnit(List<int> hasGetArenaRewardList, Dictionary<int, ArenaRewardData> allRewardDataList, Action callback)
    {
        List<int> allSortRewardList = SortAllReward(hasGetArenaRewardList, allRewardDataList);        

        m_rewardList.GetComponent<MogoListImproved>().SetGridLayout<ArenaRewardGrid>(allSortRewardList.Count(), m_rewardList.transform,
            () =>
            {
                int iter = 0;
                if (m_rewardList.GetComponent<MogoListImproved>().DataList.Count > 0)
                {
                    foreach (var rewardID in allSortRewardList)
                    {
                        ArenaRewardGrid unit = (ArenaRewardGrid)(m_rewardList.GetComponent<MogoListImproved>().DataList[iter]);
                        unit.RewardID = rewardID;
                        unit.Icon = ArenaScoreRewardData.dataMap.Get(rewardID).reward.Keys.
                            Select(x => ItemParentData.GetItem(x).Icon).ToList();
                        unit.Color = ArenaScoreRewardData.dataMap.Get(rewardID).reward.Keys.
                            Select(x => ItemParentData.GetItem(x).color).ToList();
                        unit.clickHandler = ArenaRewardUILogicManager.Instance.OnGetReward;

                        foreach (KeyValuePair<int, int> pair in ArenaScoreRewardData.dataMap.Get(rewardID).reward)
                        {
                            string name = (GetNameByItemID(pair.Key));

                            int num = allRewardDataList[rewardID].num;
                            string numText = string.Concat(" x ", num);
                            string qualityColor = ItemParentData.GetQualityColorByItemID(pair.Key);
                            numText = string.Concat("[", qualityColor, "]", numText, "[-]");

                            unit.Name = string.Concat(name, numText);
                            break;
                        }

                        bool IsAlreadyGet = false;
                        if (hasGetArenaRewardList.Contains(rewardID))
                            IsAlreadyGet = true;
                        else
                            IsAlreadyGet = false;

                        unit.SetDetailInfo(ArenaScoreRewardData.dataMap.Get(rewardID).type,
                            ArenaScoreRewardData.dataMap.Get(rewardID).score,
                            ArenaUILogicManager.Instance.m_ObtainDayPoint,
                            ArenaUILogicManager.Instance.m_ObtainWeekPoint,
                            IsAlreadyGet);
                        iter++;
                    }
                }
            });
    }

    static public string GetNameByItemID(int itemID)
    {
        if (ItemParentData.GetItem(itemID) != null)
        {
            return ItemParentData.GetItem(itemID).Name;
        }

        else
        {
            LoggerHelper.Debug(String.Format("cannot find itemID {0} in all item xml", itemID));
            return String.Empty;
        }
    }        

    /// <summary>
    /// ʹ�ÿͻ��˽�������
    /// </summary>
    //public void AddRewardUnit(List<int> hasGetArenaRewardList, Dictionary<int, ArenaRewardData> allRewardDataList, Action callback)
    //{
    //    var filterReward = ArenaScoreRewardData.dataMap.
    //                    Where(x => MogoWorld.thePlayer.level >= x.Value.level[0]
    //                    && MogoWorld.thePlayer.level <= x.Value.level[1]).
    //                    Select(x => x.Value).
    //                    ToList();
    //    List<ArenaScoreRewardData> sortReward = new List<ArenaScoreRewardData>();
    //    foreach (var xml in filterReward)
    //    {
    //        LoggerHelper.Debug(xml.id);
    //        if (!hasGetArenaRewardList.Contains(xml.id))
    //        {
    //            sortReward.Add(xml);
    //        }
    //    }
    //    foreach (var xml in filterReward)
    //    {
    //        if (hasGetArenaRewardList.Contains(xml.id))
    //        {
    //            sortReward.Add(xml);
    //        }
    //    }
    //    m_rewardList.GetComponent<MogoListImproved>().SetGridLayout<ArenaRewardGrid>(filterReward.Count(), m_rewardList.transform,
    //        () =>
    //        {
    //            int iter = 0;
    //            if (m_rewardList.GetComponent<MogoListImproved>().DataList.Count > 0)
    //            {
    //                foreach (var xml in sortReward)
    //                {
    //                    ArenaRewardGrid unit = (ArenaRewardGrid)(m_rewardList.GetComponent<MogoListImproved>().DataList[iter]);
    //                    unit.RewardID = xml.id;
    //                    unit.Icon = ArenaScoreRewardData.dataMap.Get(xml.id).reward.Keys.
    //                        Select(x => ItemParentData.GetItem(x).Icon).ToList();
    //                    unit.Color = ArenaScoreRewardData.dataMap.Get(xml.id).reward.Keys.
    //                        Select(x => ItemParentData.GetItem(x).color).ToList();
    //                    unit.clickHandler = ArenaRewardUILogicManager.Instance.OnGetReward;

    //                    foreach (KeyValuePair<int, int> pair in ArenaScoreRewardData.dataMap.Get(xml.id).reward)
    //                    {
    //                        string name = (LanguageData.GetNameByItemID(pair.Key));
    //                        int num = pair.Value;
    //                        if (pair.Key == 1)
    //                        {
    //                            num = num * AvatarLevelData.GetExpStandard(MogoWorld.thePlayer.level);
    //                            num = RoundingNum(num);
    //                        }
    //                        else if (pair.Key == 2)
    //                        {
    //                            num = num * AvatarLevelData.GetGoldStandard(MogoWorld.thePlayer.level);
    //                            num = RoundingNum(num);
    //                        }

    //                        string numText = string.Concat(" x ", num);
    //                        string qualityColor = ItemParentData.GetQualityColorByItemID(pair.Key);
    //                        numText = string.Concat("[", qualityColor, "]", numText, "[-]");

    //                        unit.Name = string.Concat(name, numText);
    //                        break;
    //                    }

    //                    bool IsAlreadyGet = false;
    //                    if (hasGetArenaRewardList.Contains(xml.id))
    //                        IsAlreadyGet = true;
    //                    else
    //                        IsAlreadyGet = false;

    //                    unit.SetDetailInfo(xml.type,
    //                        ArenaScoreRewardData.dataMap.Get(xml.id).score,
    //                        ArenaUILogicManager.Instance.m_ObtainDayPoint,
    //                        ArenaUILogicManager.Instance.m_ObtainWeekPoint,
    //                        IsAlreadyGet);
    //                    iter++;
    //                }
    //            }
    //        });
    //}
}