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
    public class DragonQualityData : GameData<DragonQualityData>
    {
        public int quality { get; protected set; }
        public int name { get; protected set; }
        public Dictionary<int,int> convoyCompleteTime { get; protected set; }
        public int rewardAddition { get; protected set; }

        /// <summary>
        /// 飞龙名称
        /// </summary>
        /// <param name="bHasColor">是否加上颜色值</param>
        /// <returns></returns>
        public string GetName(bool bHasColor = true)
        {
            string dragonName = LanguageData.GetContent(name);

            if (bHasColor)
            {
                int q = quality;
                if (!QualityColorData.dataMap.ContainsKey(q))
                    q = 1;

                string qualityColor = ItemParentData.GetQualityColorByQuality(q);
                dragonName = string.Concat("[", qualityColor, "]", dragonName, "[-]");
            }        

            return dragonName;
        }

        public static readonly string fileName = "xml/DragonQuality";
        //public static Dictionary<int, DragonQualityData> dataMap { get; set; }

        static List<DragonQualityData> m_DataList;
        public static List<DragonQualityData> GetDataList()
        {
            if (m_DataList == null)
            {
                m_DataList = new List<DragonQualityData>();
                foreach (DragonQualityData data in dataMap.Values)
                {
                    m_DataList.Add(data);
                }
                m_DataList.Sort(
                   (DragonQualityData a, DragonQualityData b) =>
                    {
                        if (a.quality < b.quality) return -1;
                        else return 1;
                    });
            }
            return m_DataList;

        }

        public static DragonQualityData GetDragonQualityData(int quality)
        {
            foreach (DragonQualityData data in dataMap.Values)
            {
                if (data.quality == quality) return data;
            }
            return null;
        }
    }
}