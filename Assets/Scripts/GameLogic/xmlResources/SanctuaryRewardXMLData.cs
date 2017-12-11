using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class SanctuaryRewardXMLData : GameData
#else
    public class SanctuaryRewardXMLData : GameData<SanctuaryRewardXMLData>
#endif
    {
        public int type { get; protected set; }
        public int rank { get; set; }
        public int exp { get; set; }
        public int gold { get; set; }
        public Dictionary<int, int> items { get; set; }
        public int icon { get; set; }
        public int contribution { get; set; }
        public List<int> level { get; set; }
        static public readonly string fileName = "xml/Reward_SanctuaryDefense";
#if UNITY_IPHONE
		static public Dictionary<int, SanctuaryRewardXMLData> dataMap { get; set; }
#endif
        #region 奖励列表

        /// <summary>
        /// 获取今日排名奖励ID列表
        /// </summary>
        /// <returns></returns>
        static public List<int> GetDayRankID()
        {
            var d = (from t in dataMap
                     where t.Value.type == 1
                     select t.Value.id).OrderBy(x => x).ToList();
            return d;
        }

        /// <summary>
        /// 获取本周排名奖励ID列表
        /// </summary>
        /// <returns></returns>
        static public List<int> GetWeekRankID()
        {
            var d = (from t in dataMap
                     where t.Value.type == 2
                     select t.Value.id).OrderBy(x => x).ToList();
            return d;
        }

        /// <summary>
        /// 通过周奖励Index获取对应的奖励名称列表
        /// </summary>
        /// <param name="xmlID"></param>
        /// <returns></returns>
        static public List<string> GetWeekRewardNameList(int weekRankIndex)
        {
            List<int> weekRankIDList = SanctuaryRewardXMLData.GetWeekRankID();
            List<string> tips = new List<string>();

            if (weekRankIndex >= weekRankIDList.Count)
            {
                return tips;
            }

            if (SanctuaryRewardXMLData.dataMap[weekRankIDList[weekRankIndex]].gold > 0)
            {
                tips.Add(String.Concat(LanguageData.MONEY, "  ", SanctuaryRewardXMLData.dataMap[weekRankIDList[weekRankIndex]].gold));
            }

            if (SanctuaryRewardXMLData.dataMap[weekRankIDList[weekRankIndex]].exp > 0)
            {
                tips.Add(String.Concat(LanguageData.EXP, "  ", SanctuaryRewardXMLData.dataMap[weekRankIDList[weekRankIndex]].exp));
            }

            foreach (var item in SanctuaryRewardXMLData.dataMap[weekRankIDList[weekRankIndex]].items)
            {
                tips.Add(String.Concat(ArenaRewardUIViewManager.GetNameByItemID(item.Key), " x ", item.Value));
            }

            return tips;
        }

        /// <summary>
        /// 通过今日奖励Index获取对应的奖励名称列表
        /// </summary>
        /// <param name="xmlID"></param>
        /// <returns></returns>
        static public List<string> GetDayRewardNameList(int dayRankIndex)
        {
            List<int> dayRankIDList = SanctuaryRewardXMLData.GetDayRankID();
            List<string> tips = new List<string>();

            if (dayRankIndex >= dayRankIDList.Count)
            {
                return tips;
            }

            if (SanctuaryRewardXMLData.dataMap[dayRankIDList[dayRankIndex]].gold > 0)
            {
                tips.Add(String.Concat(LanguageData.MONEY, "  ", SanctuaryRewardXMLData.dataMap[dayRankIDList[dayRankIndex]].gold));
            }

            if (SanctuaryRewardXMLData.dataMap[dayRankIDList[dayRankIndex]].exp > 0)
            {
                tips.Add(String.Concat(LanguageData.EXP, "  ", SanctuaryRewardXMLData.dataMap[dayRankIDList[dayRankIndex]].exp));
            }

            foreach (var item in SanctuaryRewardXMLData.dataMap[dayRankIDList[dayRankIndex]].items)
            {
                tips.Add(String.Concat(ArenaRewardUIViewManager.GetNameByItemID(item.Key), " x ", item.Value));
            }

            return tips;
        }

        #endregion

        static public string GetAccuNextRankIcon(int contri)
        {
            var id = dataMap.First(t => t.Value.type == 3 && t.Value.contribution > contri);
            if (dataMap[id.Key].items != null)
            {
                return ItemParentData.GetItem(dataMap[id.Key].items.Keys.ElementAt(0)).Icon;
            }
            else
            {
                return string.Empty;
            }
        }

        static public int GetAccuNextRankItemID(int contri)
        {
            var id = dataMap.First(t => t.Value.type == 3 && t.Value.contribution > contri);
            return dataMap[id.Key].items.Keys.ElementAt(0);
        }

        static public string GetAccuCurRankIcon(int contri)
        {
            var id = dataMap.First(t => t.Value.type == 3 && t.Value.contribution == contri);
            if (dataMap[id.Key].items != null)
            {
                return ItemParentData.GetItem(dataMap[id.Key].items.Keys.ElementAt(0)).Icon;
            }
            else
            {
                return string.Empty;
            }
        }

        static public int GetAccuNextRankContribution(int contri)
        {
            var id = dataMap.First(t => t.Value.type == 3 && t.Value.contribution > contri);
            return id.Value.contribution;
        }

        static public int GetAccuNextGold(int contri)
        {
            var id = dataMap.First(t => t.Value.type == 3 && t.Value.contribution > contri);
            return id.Value.gold;
        }
    }
}
