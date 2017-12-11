#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemPearlData
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.2.26
// 模块描述：元素宝珠模板。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    /// <summary>
    /// 元素宝珠模板。
    /// </summary>
    public class ItemPearlData : ItemParentData
    {
        public int effectDescription { get; protected set; }

        //public static readonly string fileName = "xml/ItemPearl";
        public static Dictionary<int, ItemPearlData> dataMap { get; set; }
    }
}