/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemInstance
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：
// 模块描述：道具实例数据
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using System.Collections.Generic;
using System;

public class ItemInstance
{
    public int id { get; set; }
    public int templeId { get; set; }
    public int bagType { get; set; }
    public int gridIndex { get; set; }
    public int stack { get; set; }
    public int bindingType { get; set; }
    public Dictionary<int, int> jewelSlots { get; set; }
    public int sourceValue { get; set; }
    public int sourceKey { get; set; }
    public int leftCoolTime { get; set; }
    public Dictionary<int, int> extendInfo { get; set; }

    static public ItemInstance GetEmptyInstance(int _templeId)
    {
        return new ItemInstance() { templeId = _templeId };
    }
}

