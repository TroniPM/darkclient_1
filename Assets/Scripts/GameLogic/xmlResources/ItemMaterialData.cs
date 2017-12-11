#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemComData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013.5.3
// 模块描述：材料数据
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ItemMaterialData : ItemParentData<ItemMaterialData>
    {
        public int price { get; protected set; }
        public int useDescription { get; protected set; }
       
        public static readonly string fileName = "xml/ItemCom";
        //public static Dictionary<int, ItemMaterialData> dataMap { get; set; }

        private static int m_maxLevel = -1;
        public static int MaxLevel
        {
            get
            {
                if (m_maxLevel != -1) return m_maxLevel;
                foreach (ItemMaterialData item in dataMap.Values)
                {
                    if (item.level > m_maxLevel)
                        m_maxLevel = item.level;
                }
                return m_maxLevel;
            }
        }
    }
}