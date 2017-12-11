/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EquipData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-2-6
// 模块描述：装备外形数据结构
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class EquipData : GameData<EquipData>
    {
        public List<string> prefabPath { get; protected set; }
        public List<string> slot { get; protected set; }
        public List<string> slotInCity { get; protected set; }
        public string mesh { get; protected set; }
        public string material { get; protected set; }
        public int putOnMethod { get; protected set; }
        public List<int> type { get; protected set; }
        public int priority { get; protected set; }
        public int isWeapon { get; protected set; }//大于0便是武器


        public List<int> subEquip { get; protected set; }//用于多见装备合成
        public int suit { get; protected set; }
        public int suitCount { get; protected set; }
        static public readonly string fileName = "xml/Equip";
        //static public Dictionary<int, EquipData> dataMap { get; set; }
    }
}