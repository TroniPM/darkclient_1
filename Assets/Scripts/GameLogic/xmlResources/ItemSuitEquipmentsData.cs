#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemSuitEquipmentsData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013.7.2
// 模块描述：套装数据模板。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    /// <summary>
    /// 装备模板。
    /// </summary>
#if UNITY_IPHONE
	public class ItemSuitEquipmentsData : GameData
#else
    public class ItemSuitEquipmentsData : GameData<ItemSuitEquipmentsData>
#endif
	{
        public Dictionary<int, int> suitId { get; protected set; }
        public int name { get; protected set; }
        public int demandNum { get; protected set; }
        public int costs { get; protected set; }
        public List<int> vocation1 { get; protected set; }
        public List<int> vocation2 { get; protected set; }
        public List<int> vocation3 { get; protected set; }
        public List<int> vocation4 { get; protected set; }

        public static readonly string fileName = "xml/ItemSuitEquipments";
#if UNITY_IPHONE
        public static Dictionary<int, ItemSuitEquipmentsData> dataMap { get; set; }
#endif
        ///// <summary>
        ///// <suitId,suitDataList>
        ///// </summary>
        //private static Dictionary<int, List<ItemSuitEquipmentsData>> m_suitDic;

        ///// <summary>
        ///// <suitId,suitDataList>
        ///// </summary>
        //public static Dictionary<int, List<ItemSuitEquipmentsData>> SuitDic
        //{
        //    get
        //    {
        //        if (m_suitDic == null)
        //        {
        //            m_suitDic = new Dictionary<int, List<ItemSuitEquipmentsData>>();
        //            foreach (ItemSuitEquipmentsData data in dataMap.Values)
        //            {
        //                if (!m_suitDic.ContainsKey(data.suitId))
        //                {
        //                    m_suitDic[data.suitId] = new List<ItemSuitEquipmentsData>();
        //                }
        //                m_suitDic[data.suitId].Add(data);
        //            }

        //            foreach (List<ItemSuitEquipmentsData> list in m_suitDic.Values)
        //            {
        //                list.Sort(delegate(ItemSuitEquipmentsData a, ItemSuitEquipmentsData b)
        //                {
        //                    if (a.demandNum < b.demandNum) return -1;
        //                    else return 1;
        //                });
        //            }
        //        }
        //        return m_suitDic;
        //    }
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="suitId">套装id</param>
        /// <returns></returns>
        public static string GetSuitName(int suitId)
        {
            int nameId = dataMap.Get(suitId).name;
            return LanguageData.dataMap[nameId].content;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="suitId">套装id</param>
        /// <returns></returns>
        public static int GetSuitMaxNum(int suitId)
        {
            //List<ItemSuitEquipmentsData> list = SuitDic[suitId];
            //int max = -1;
            //foreach (int i in dataMap.Get(suitId).suitId.Keys)
            //{
            //    if (data.demandNum > max)
            //    {
            //        max = data.demandNum;
            //    }
            //}
            //return max;
            //return list[list.Count - 1].demandNum;

            return dataMap.Get(suitId).vocation1.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="suitID">suitID</param>
        /// <param name="suitNum">已穿套装数量</param>
        /// <returns></returns>
        public static List<string> GetSuitAttrList(int suitId, int suitNum)
        {
            Dictionary<int, int> dic = dataMap.Get(suitId).suitId;
            List<string> suitAttrList = new List<string>();

            foreach (KeyValuePair<int, int> pair in dic)
            {
                PropertyEffectData prop = PropertyEffectData.dataMap.Get(pair.Value);
                string str = prop.GetOneEffectStr();
                str = LanguageData.GetContent(1138, pair.Key, str);
                if (suitNum < pair.Key)
                {
                    str = string.Concat("[555555]", str, "[-]");
                }
                suitAttrList.Add(str);
            }

            return suitAttrList;
            //return null;
        }

        //public string GetAttr()
        //{
        //    string attr = "";

        //    //if (hpBase > 0) attr = LanguageData.dataMap[201].Format(hpBase);
        //    //else if (hpAddRate > 0) attr = LanguageData.dataMap[250].Format(hpAddRate / 100f);
        //    //else if (attackBase > 0) attr = LanguageData.dataMap[202].Format(attackBase);
        //    //else if (attackAddRate > 0) attr = LanguageData.dataMap[251].Format(attackAddRate / 100f);
        //    //else if (defenseBase > 0) attr = LanguageData.dataMap[203].Format(defenseBase / 100f);
        //    //else if (speedAddRate > 0) attr = LanguageData.dataMap[253].Format(speedAddRate / 100f);
        //    //else if (hit > 0) attr = LanguageData.dataMap[205].Format(hit);
        //    //else if (crit > 0) attr = LanguageData.dataMap[204].Format(crit);
        //    //else if (trueStrike > 0) attr = LanguageData.dataMap[206].Format(trueStrike);
        //    //else if (critExtraAttack > 0) attr = LanguageData.dataMap[207].Format(critExtraAttack);
        //    //else if (antiDefense > 0) attr = LanguageData.dataMap[208].Format(antiDefense);
        //    //else if (antiTrueStrike > 0) attr = LanguageData.dataMap[210].Format(antiTrueStrike);
        //    //else if (damageReduce > 0) attr = LanguageData.dataMap[252].Format(damageReduce);
        //    //else if (cdReduce > 0) attr = LanguageData.dataMap[212].Format(cdReduce);
        //    //else if (extraHitRate > 0) attr = LanguageData.dataMap[256].Format(extraHitRate / 100f);
        //    //else if (extraCritRate > 0) attr = LanguageData.dataMap[255].Format(extraCritRate / 100f);
        //    //else if (extraTrueStrikeRate > 0) attr = LanguageData.dataMap[257].Format(extraTrueStrikeRate / 100f);
        //    //else if (pvpAddition > 0) attr = LanguageData.dataMap[258].Format(pvpAddition);
        //    //else if (pvpAnti > 0) attr = LanguageData.dataMap[259].Format(pvpAnti);
        //    //else if (extraExpRate > 0) attr = LanguageData.dataMap[260].Format(extraExpRate / 100f);
        //    //else if (extraGoldRate > 0) attr = LanguageData.dataMap[261].Format(extraGoldRate / 100f);

        //    return string.Concat(demandNum, "件 ", attr);

        //}

        public List<int> GetEquipList(int vocation)
        {
            //Debug.LogError(vocation);
            switch (vocation)
            {
                case 1:
                    //Debug.LogError("vocation1");
                    return this.vocation1;
                case 2:
                    return this.vocation2;
                case 3:
                    return this.vocation3;
                case 4:
                    return this.vocation4;
            }
            //Debug.LogError("null");
            return null;
        }
    }
}