/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemGem
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class ItemPearl : ItemParent
{
    public ItemPearlData ItemProp { get; set; }
    /// <summary>
    /// 镶嵌类型
    /// </summary>
    public sbyte bindingType { get; set; }
    /// <summary>
    /// 镶嵌类型
    /// </summary>
    public sbyte inlayType { get; set; }

}