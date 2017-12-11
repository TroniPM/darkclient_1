/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemEffect
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-9-13
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
	public class ItemEffectData : GameData
#else
    public class ItemEffectData : GameData<ItemEffectData>
#endif
	{
        public Dictionary<int, int> reward1 { get; protected set; }
        public Dictionary<int, int> costId { get; protected set; }

        static public readonly string fileName = "xml/ItemEffect";
#if UNITY_IPHONE
		static public Dictionary<int, ItemEffectData> dataMap { get; set; }
#endif
    }
}
