/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemInstance
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：
// 模块描述：装备实例数据
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using System.Collections.Generic;

public class ItemEquipmentInstance : ItemParentInstance
{
    public int sourceValue { get; set; }
    public int sourceKey { get; set; }
    public int bindingType { get; set; }
    public List<int> jewelSlots = new List<int>();
    public bool locked = false;
    public bool isActive = false;
}

