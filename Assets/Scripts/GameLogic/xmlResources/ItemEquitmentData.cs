#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemEquitmentData
// 创建者：Ash Tang
// 修改者列表：Joe Mo
// 创建日期：2013.2.26
// 模块描述：装备模板。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Game;

namespace Mogo.GameData
{
    /// <summary>
    /// 装备模板。
    /// </summary>
    public class ItemEquipmentData : ItemParentData<ItemEquipmentData>
    {
        public int mode { get; protected set; }
        public sbyte vocation { get; protected set; }
        public int levelNeed { get; protected set; }
        public int levelLimit { get; protected set; }
        public List<int> jewelSlot { get; protected set; }
        public int suitId { get; protected set; }
        public Dictionary<int, int> enchant { get; protected set; }

        public static readonly string fileName = "xml/ItemEquipment";
        //public static Dictionary<int, ItemEquipmentData> dataMap { get; set; }

        private Dictionary<int, ItemEquipValues> m_attrDic;
        public Dictionary<int, ItemEquipValues> AttrDic
        {
            get
            {
                if (m_attrDic == null)
                {
                    //Mogo.Util.LoggerHelper.Debug("quality:" + quality);
                    //Mogo.Util.LoggerHelper.Debug("vocation:" + vocation);
                    //Mogo.Util.LoggerHelper.Debug("type:" + type);
                    //Mogo.Util.LoggerHelper.Debug("levelNeed:" + levelNeed);
                    int v = vocation;
                    if (!ItemEquipValues.GetValuesDic()[quality].ContainsKey(vocation))
                        v = 5;
                    if (ItemEquipValues.GetValuesDic()[quality][v][type].ContainsKey(levelNeed))
                        m_attrDic = ItemEquipValues.GetValuesDic()[quality][v][type];
                    else
                        m_attrDic = ItemEquipValues.GetValuesDic()[quality][5][type];

                }
                return m_attrDic;

            }
        }

        /// <summary>
        /// 正常装备名
        /// </summary>
        public override string Name
        {
            get
            {
                return GetName();
            }
        }

        /// <summary>
        /// 获取装备名(仅用于聊天系统的处理)
        /// </summary>
        /// <param name="tmpID"></param>
        /// <returns></returns>
        public static string GetCommunityEquipNameByID(int tmpID)
        {
            string name = "";
            if (dataMap.ContainsKey(tmpID))
            {
                name = dataMap[tmpID].GetName(true);
            }

            return name;
        }

        /// <summary>
        /// 获取装备名
        /// </summary>
        /// <param name="bCommunity">false:正常装备名；true:用于聊天系统的特殊装备名</param>
        /// <returns></returns>
        private string GetName(bool bCommunity = false)
        {
            if (name <= 0) name = 6000;
            //Debug.LogError("getName!!");
            string qualityStr = LanguageData.dataMap[6000 + quality].content;
            string themeStr = "";
            string slotStr = "";
            int themeIndex;
            int slotIndex;

            //非戒指项链
            if (type != 2 && type != 9)
            {
                //int level = levelNeed + 4;
                themeIndex = levelNeed / 5;
                slotIndex = 0;

                switch (vocation)
                {
                    case (int)Vocation.Warrior:
                        //这里是取巧方法，若等级排列有变化即不成立
                        themeIndex += 6045;
                        break;
                    case (int)Vocation.Assassin:
                        themeIndex += 6060;
                        break;
                    case (int)Vocation.Archer:
                        themeIndex += 6075;
                        break;
                    case (int)Vocation.Mage:
                        themeIndex += 6090;
                        break;
                }

                //非武器时部位名
                if (type != 10)
                {
                    slotIndex = 6016 + (type > 1 ? type - 1 : type) + (vocation - 1) * 7;
                }
                else
                {
                    slotIndex = 6007 + subtype - 1;
                }
            }
            //戒指项链
            else
            {
                //int level = levelNeed + 4;
                themeIndex = levelNeed / 5 + 6105;
                if (type == 2) slotIndex = 6015;
                else slotIndex = 6016;
            }

            themeStr = LanguageData.dataMap[themeIndex].content;
            slotStr = LanguageData.dataMap[slotIndex].content;

            string strName = "";
            QualityColorData colorData = QualityColorData.dataMap[quality];
            if (!bCommunity)
            {
                //Debug.LogError("name:" + name);
                //Debug.LogError("qualityStr:" + qualityStr);
                //Debug.LogError("themeStr:" + themeStr);
                //Debug.LogError("slotStr:" + slotStr);
                strName = string.Concat("[", colorData.color, "]", LanguageData.dataMap[name].Format(qualityStr, themeStr, slotStr), "[-]");
            }
            else
            {
                strName = string.Concat("LV", levelNeed, " ", LanguageData.dataMap[name].Format(qualityStr, themeStr, slotStr));
                strName = string.Format(LanguageData.GetContent(47304), strName);
                strName = string.Concat("[", colorData.color, "]", strName, "[-]");
            }

            //Debug.LogError("getName:" + strName);
            return strName;
        }

        public int GetScore(int level)
        {
            int lev;
            if (levelNeed == levelLimit)
            {
                lev = levelNeed;
            }
            else
            {
                lev = Math.Max(level, levelNeed);
                lev = Math.Min(lev, levelLimit);
            }

            return AttrDic[lev].scores;
        }

        public List<string> GetAttrDescriptionList(int level)
        {
            Dictionary<int, ItemEquipValues> attrDic;
            if (ItemEquipValues.GetValuesDic()[quality][vocation][type].ContainsKey(level))
                attrDic = ItemEquipValues.GetValuesDic()[quality][vocation][type];
            else
                attrDic = ItemEquipValues.GetValuesDic()[quality][5][type];

            ItemEquipValues value = attrDic[level];
            List<string> attrs = new List<string>();

            if (value.hpBase > 0) attrs.Add(LanguageData.dataMap[201].Format(value.hpBase));
            if (value.attackBase > 0) attrs.Add(LanguageData.dataMap[202].Format(value.attackBase));
            if (value.defenseBase > 0) attrs.Add(LanguageData.dataMap[203].Format(value.defenseBase));
            if (value.crit > 0) attrs.Add(LanguageData.dataMap[204].Format(value.crit));
            if (value.hit > 0) attrs.Add(LanguageData.dataMap[205].Format(value.hit));
            if (value.trueStrike > 0) attrs.Add(LanguageData.dataMap[206].Format(value.trueStrike));
            if (value.critExtraAttack > 0) attrs.Add(LanguageData.dataMap[207].Format(value.critExtraAttack));
            if (value.antiDefense > 0) attrs.Add(LanguageData.dataMap[208].Format(value.antiDefense));
            if (value.antiCrit > 0) attrs.Add(LanguageData.dataMap[209].Format(value.antiCrit));
            if (value.antiTrueStrike > 0) attrs.Add(LanguageData.dataMap[210].Format(value.antiTrueStrike));
            if (value.cdReduce > 0) attrs.Add(LanguageData.dataMap[211].Format(value.cdReduce));
            if (value.pvpAddition > 0) attrs.Add(LanguageData.dataMap[212].Format(value.pvpAddition));
            if (value.pvpAnti > 0) attrs.Add(LanguageData.dataMap[213].Format(value.pvpAnti));

            return attrs;
        }

        /// <summary>
        /// 通过ID获取data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static public ItemEquipmentData GetItemEquipmentData(int id)
        {
            if (dataMap.ContainsKey(id))
                return dataMap[id];
            return null;
        }
    }
}