/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EquipValuesSortInfoData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-8-12
// 模块描述：
//----------------------------------------------------------------*/

using Mogo.Util;
using Mogo.Game;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class EquipValuesSortInfoData : GameData
#else
    public class EquipValuesSortInfoData : GameData<EquipValuesSortInfoData>
#endif
	{
        public List<int> sort { get; protected set; }

        static public readonly string fileName = "xml/EquipValuesSortInfo";
#if UNITY_IPHONE
		static public Dictionary<int, EquipValuesSortInfoData> dataMap { get; set; }
#endif
    }
}
