using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class MissionRewardData : GameData<MissionRewardData>
    {
        public string condition { get; protected set; }
        public Dictionary<int, int> rewards { get; protected set; }

        public static Dictionary<int, List<List<int>>> fixCondition { get; protected set; }

        static public readonly string fileName = "xml/MissionReward";
        //static public Dictionary<int, MissionRewardData> dataMap { get; set; }

        public static void FixCondition()
        {
            fixCondition = new Dictionary<int, List<List<int>>>();

            foreach (var data in dataMap)
            {
                string conditionString = data.Value.condition;
                string[] conditionSummary = conditionString.Split(';');
                if (conditionSummary == null)
                    continue;
                if (conditionSummary.Length == 0)
                    continue;

                List<List<int>> summary = new List<List<int>>();

                foreach (string conditionDetailString in conditionSummary)
                {
                    string[] conditionAllDetail = conditionDetailString.Split(',');
                    if (conditionAllDetail == null)
                        continue;

                    if (conditionAllDetail.Length != 3)
                        continue;

                    List<int> temp = new List<int>();

                    foreach (string conditionDetail in conditionAllDetail)
                        temp.Add(Convert.ToInt32(conditionDetail));

                    if (temp.Count == 3)
                        summary.Add(temp);
                }

                if (summary.Count > 0)
                    fixCondition.Add(data.Key, summary);
            }
        }
    }
}
