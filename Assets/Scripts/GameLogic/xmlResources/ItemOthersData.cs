#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemOthersData
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.2.26
// 模块描述：其他道具模板。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    /// <summary>
    /// 其他道具模板。
    /// </summary>
    public class ItemOthersData : ItemParentData
    {
        public int price { get; protected set; }
        public int useTimes { get; protected set; }
        public sbyte useCondition { get; protected set; }
        public sbyte effectType { get; protected set; }
        public int effectID { get; protected set; }

        //public static readonly string fileName = "xml/ItemOthers";
        public static Dictionary<int, ItemOthersData> dataMap { get; set; }
    }
}