/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemExchangeData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-9-26
// 模块描述：
//----------------------------------------------------------------*/
using Mogo.Util;
using Mogo.Game;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Mogo.GameData
{
    public class ItemExchangeData : GameData<ItemExchangeData>
    {
        public int level { get; protected set; }
        public int subtype { get; protected set; }
        public int type { get; protected set; }
        public Dictionary<int, int> cost { get; protected set; }
        public Dictionary<int, int> reward { get; protected set; }

        static public readonly string fileName = "xml/ItemExchange";
        //static public Dictionary<int, ItemExchangeData> dataMap { get; set; }

        static private Dictionary<int, HashSet<int>> m_dataDic;//level,set
        public static Dictionary<int, HashSet<int>> DataDic
        {
            get
            {
                if (m_dataDic == null)
                {
                    m_dataDic = new Dictionary<int, HashSet<int>>();
                    foreach (ItemExchangeData data in dataMap.Values)
                    {
                        int level = data.level;
                        if (!m_dataDic.ContainsKey(level))
                        {
                            m_dataDic.Add(level, new HashSet<int>());
                        }
                        m_dataDic[level].Add(data.id);
                    }
                }
                return m_dataDic;
            }
        }

        public int GetRewardByVocation(Vocation vocation)
        {
            return reward[(int)vocation];
        }

        public int MeterialId
        {
            get
            {
                foreach (KeyValuePair<int, int> pair in cost)
                {
                    ItemParentData item = ItemParentData.GetItem(pair.Key);
                    if (item.itemType > 0) return pair.Key;
                }
                return 0;
            }
        }

        public int GoldNum
        {
            get
            {
                int gold = 0;
                if (cost.ContainsKey(2))
                {
                    gold = cost[2];
                }
                return gold;
            }
        }

        public int MeterailNum
        {
            get
            {
                foreach (KeyValuePair<int, int> pair in cost)
                {
                    ItemParentData item = ItemParentData.GetItem(pair.Key);
                    if (item.itemType > 0) return pair.Value;
                }
                return 0;
            }
        }
    }
}
