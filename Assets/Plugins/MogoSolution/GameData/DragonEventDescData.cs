#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DragonQualityData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013.12.9
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class DragonEventDescData : GameData
#else
    public class DragonEventDescData : GameData<DragonEventDescData>
#endif
	{
        public int desc { get; protected set; }
       
        public static readonly string fileName = "xml/DragonEventDesc";
#if UNITY_IPHONE
		public static Dictionary<int, DragonEventDescData> dataMap { get; set; }
#endif
    }
}