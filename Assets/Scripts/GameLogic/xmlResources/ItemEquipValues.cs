#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemEquipValues
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013.4.22
// 模块描述：装备属性
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
    public class ItemEquipValues : GameData<ItemEquipValues>
    {
        public int quality { get; protected set; }
        public int vocation { get; protected set; }
        public int type { get; protected set; }
        public int level { get; protected set; }
        public int scores { get; protected set; }

        public int hpBase { get; protected set; }
        public int hpAddRate { get; protected set; }
        public int hit { get; protected set; }
        public int trueStrike { get; protected set; }
        public int crit { get; protected set; }
        public int critExtraAttack { get; protected set; }
        public int attackBase { get; protected set; }
        public int defenseBase { get; protected set; }

        public int antiDefense { get; protected set; }
        public int antiCrit { get; protected set; }
        public int antiTrueStrike { get; protected set; }
        public int cdReduce { get; protected set; }
        public int pvpAddition { get; protected set; }
        public int pvpAnti { get; protected set; }


        public static readonly string fileName = "xml/ItemEquipValues";
        //public static Dictionary<int, ItemEquipValues> dataMap { get; set; }

        private static Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, ItemEquipValues>>>> equipValuseDic;


        /// <summary>
        /// [quality][vocation][type][level]->EquipValues
        /// </summary>
        public static Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, ItemEquipValues>>>>
            GetValuesDic()
        {
            if (equipValuseDic != null) return equipValuseDic;
            equipValuseDic = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, ItemEquipValues>>>>();
            int count = 0;
            foreach (ItemEquipValues value in dataMap.Values)
            {
                count++;
                if (!equipValuseDic.ContainsKey(value.quality))
                    equipValuseDic[value.quality] = new Dictionary<int, Dictionary<int, Dictionary<int, ItemEquipValues>>>();

                if (!equipValuseDic[value.quality].ContainsKey(value.vocation))
                    equipValuseDic[value.quality][value.vocation] = new Dictionary<int, Dictionary<int, ItemEquipValues>>();

                if (!equipValuseDic[value.quality][value.vocation].ContainsKey(value.type))
                    equipValuseDic[value.quality][value.vocation][value.type] = new Dictionary<int, ItemEquipValues>();

                equipValuseDic[value.quality][value.vocation][value.type][value.level] = value;

            }
            return equipValuseDic;
        }
    }
}