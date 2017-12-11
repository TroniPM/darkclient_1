#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemParentData
// 创建者：Ash Tang
// 修改者列表：Joe Mo
// 创建日期：2013.2.26
// 模块描述：道具基类模板。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;
using System.Diagnostics;

namespace Mogo.GameData
{
    public abstract class ItemParentData<T> : ItemParentData where T : ItemParentData<T>
    {
        private static Dictionary<int, T> m_dataMap;

        public static Dictionary<int, T> dataMap
        {
            get
            {
                if (m_dataMap == null)
                    m_dataMap = GetDataMap<T>();
                return m_dataMap;
            }
            set { m_dataMap = value; }
        }
    }
    
    /// <summary>
    /// 道具基类模板。
    /// </summary>
    public abstract class ItemParentData : GameData
    {
        public int level { get; protected set; }
        public int effectId { get; protected set; }
        public int name { get; protected set; }
        public int description { get; protected set; }
        public int icon { get; protected set; }
        public int look { get; protected set; }
        public int itemType { get; protected set; }
        public int type { get; protected set; }
        public byte subtype { get; protected set; }
        public byte quality { get; protected set; }
        public short maxStack { get; protected set; }
        virtual public string NameWithoutColor
        {
            get
            {
                return LanguageData.dataMap[name].content;
            }
        }
        virtual public string Name
        {
            get
            {
                int q = quality;
                if (!QualityColorData.dataMap.ContainsKey(q))
                    q = 1;

                string qualityColor = GetQualityColorByQuality(q);
                string strName = string.Concat("[", qualityColor, "]", LanguageData.dataMap[name].content, "[-]");
                return strName;
            }
        }

        public string Icon { get { return IconData.dataMap[icon].path; } }//IconData.dataMap[1].path
        public int color { get { return IconData.dataMap[icon].color; } }
        public string Description { get { return LanguageData.dataMap[description].content; } }



        //道具使用相关
        public int useLevel { get; protected set; }
        public int vipLevel { get; protected set; }
        public int cdTypes { get; protected set; }
        public int cdTime { get; protected set; }
        public int useVocation { get; protected set; }

        public static Dictionary<int, ItemParentData> allItemDic;

        /// <summary>
        /// 根据id得到item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static public ItemParentData GetItem(int id)
        {
            if (id <= 0)
            {
                return null;
            }
            if (allItemDic == null)
            {
                allItemDic = new Dictionary<int, ItemParentData>();

                foreach (ItemParentData item in ItemEquipmentData.dataMap.Values)
                {
                    allItemDic.Add(item.id, item);
                }

                foreach (ItemParentData item in ItemJewelData.dataMap.Values)
                {
                    allItemDic.Add(item.id, item);
                }

                foreach (ItemParentData item in ItemMaterialData.dataMap.Values)
                {
                    allItemDic.Add(item.id, item);
                }

                foreach (ItemParentData item in ItemSpecialData.dataMap.Values)
                {
                    allItemDic.Add(item.id, item);
                }
            }
            if (allItemDic.ContainsKey(id))
                return allItemDic[id];
            else
            {
                LoggerHelper.Debug("物品" + id + "不存在");
                return null;
            }
        }

        /// <summary>
        /// 通过物品ID获得物品Quality
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        static public byte GetQualityByItemID(int itemID)
        {
            if (ItemParentData.GetItem(itemID) != null)
            {
                return ItemParentData.GetItem(itemID).quality;
            }

            else
            {
                LoggerHelper.Debug(String.Format("cannot find itemID {0} in all item xml", itemID));
                return 0;
            }
        }

        /// <summary>
        /// 通过物品ID获取QualityColor
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        static public string GetQualityColorByItemID(int itemID)
        {
            int quality = ItemParentData.GetQualityByItemID(itemID);
            if (!QualityColorData.dataMap.ContainsKey(quality))
                quality = 1;

            return GetQualityColorByQuality(quality);
        }

        /// <summary>
        /// 通过Quality获取QualityColor
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static string GetQualityColorByQuality(int quality)
        {
            QualityColorData colorData = QualityColorData.dataMap[quality];
            return colorData.color;
        }

        public static string GetNameWithNum(int id, int num)
        {
            var item = ItemParentData.GetItem(id);

            int q = item.quality;
            if (!QualityColorData.dataMap.ContainsKey(q))
                q = 1;
            string qualityColor = GetQualityColorByQuality(q);

            if (num > 1)
                return string.Concat("[", qualityColor, "]", item.Name, "x", num, "[-]");
            else
                return string.Concat("[", qualityColor, "]", item.Name, "[-]");
        }
    }
}