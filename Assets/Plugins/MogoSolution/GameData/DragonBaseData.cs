#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DragonQualityData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013.12.9
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class DragonBaseData : GameData
#else
    public class DragonBaseData : GameData<DragonBaseData>
#endif
    {
        public Dictionary<int, int> upQualityitemCost { get; protected set; }
        public int dailyAttackTimes { get; protected set; }
        public int dailyConvoyTimes { get; protected set; }
        public int convoyAttackedTimes { get; protected set; }
        public int upgradeQualityCost { get; protected set; }
        public int goldDragonPrice { get; protected set; }
        public int attackTimesPrice { get; protected set; }
        public int clearAttackCDPrice { get; protected set; }
        public int revengeTimes { get; protected set; }
        public int cutCompleteTimeFiveMinPrice { get; protected set; }
        public int immediateCompleteConvoyPrice { get; protected set; }
        public int convoyTimesPrice { get; protected set; }
        public int freshAdversaryPrice { get; protected set; }
        public int levelNeed { get; protected set; }


        public static readonly string fileName = "xml/DragonBase";
#if UNITY_IPHONE
        public static Dictionary<int, DragonBaseData> dataMap { get; set; }
#endif

        public static int[] GetCostItem()
        {
            int[] temp = new int[2];
            foreach (KeyValuePair<int, int> pair in dataMap[1].upQualityitemCost)
            {
                temp[0] = pair.Key;
                temp[1] = pair.Value;
                break;
            }
            return temp;
        }
    }
}