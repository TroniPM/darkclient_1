/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DragonRewardsData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.GameData
{
    public class DragonRewardsData : GameData<DragonRewardsData>
    {
        public List<int> levelRange { get; protected set; }
        public Dictionary<int, int> rewards { get; protected set; }


        public static readonly string fileName = "xml/DragonRewards";
        //public static Dictionary<int, DragonRewardsData> dataMap { get; set; }

        static public Dictionary<int, int> GetReward(int level, int roudNumber,int quality)
        {
            Dictionary<int, int> actualReward;
            foreach (DragonRewardsData data in dataMap.Values)
            {
                if (data.levelRange[0] > level || data.levelRange[1] < level) continue;
                actualReward = new Dictionary<int, int>();
                foreach (KeyValuePair<int, int> pair in data.rewards)
                {
                    actualReward[pair.Key] = (int)(pair.Value * 
                                            (1+DragonStationData.dataMap.Get(roudNumber).addFactor * 0.0001f)
                                             * (1 + DragonQualityData.GetDragonQualityData(quality).rewardAddition *0.0001f));
                }
                return actualReward;
            }
            return null;
        }
    }
}