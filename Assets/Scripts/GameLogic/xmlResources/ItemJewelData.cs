#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemJewelData
// 创建者：Ash Tang
// 修改者列表：Joe Mo
// 创建日期：2013.2.26
// 模块描述：宝石模板。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ItemJewelData : ItemParentData<ItemJewelData>
    {
        public const int JEWEL_NAME_START_ID = 177;
        public int price { get; protected set; }
        public int propertyEffectId { get; protected set; }
        public int score { get; protected set; }
        public int skillEfectId { get; protected set; }
        public int effectDescription { get; protected set; }
        public string effectDescriptionStr
        {
            get
            {
                return LanguageData.dataMap[effectDescription].Format(PropertyEffectData.GetOneEffect(propertyEffectId));
            }
        }
        public string typeName
        {
            get
            {
                return LanguageData.dataMap[JEWEL_NAME_START_ID + subtype].content;
            }
        }
        public List<int> slotType { get; protected set; }

        public static readonly string fileName = "xml/ItemJewel";
        //public static Dictionary<int, ItemJewelData> dataMap { get; set; }

        private static Dictionary<int, Dictionary<int, ItemJewelData>> m_jewelDic;
        public static Dictionary<int, Dictionary<int, ItemJewelData>> JewelDic
        {
            get
            {
                if (m_jewelDic != null) return m_jewelDic;

                m_jewelDic = new Dictionary<int, Dictionary<int, ItemJewelData>>();

                foreach (ItemJewelData jewel in dataMap.Values)
                {
                    int level = jewel.level;
                    int subtype = jewel.subtype;
                    if (!m_jewelDic.ContainsKey(subtype)) m_jewelDic.Add(subtype, new Dictionary<int, ItemJewelData>());

                    m_jewelDic[subtype][level] = jewel;

                    //初始化maxType
                    if (subtype > maxType) maxType = subtype;
                }

                return m_jewelDic;
            }
        }//<subtype,<level,jewel>>

        /// <summary>
        /// subtype,level,jewel
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, Dictionary<int, ItemJewelData>> GetJewelDic()
        {
            return JewelDic;
        }

        public static int JewelLevelNum
        {
            get
            {
                return JewelDic[1].Count;
            }
        }

        public static int TypeNum
        {
            get
            {
                return JewelDic.Count;
            }
        }

        static private int maxType = 0;
        public static int MaxType
        {
            get
            {
                //Dictionary<int, Dictionary<int, ItemJewelData>> temp = JewelDic;
                return maxType;
            }

        }
    }
}