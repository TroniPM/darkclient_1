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
	public class DragonEventsData : GameData
#else
	public class DragonEventsData : GameData<DragonEventsData>
#endif
	{
        public Dictionary<int, int> upQualityitemCost { get; protected set; }
        public int name { get; protected set; }
        public int descpt { get; protected set; }

        public string Descpt
        {
            get
            {
                return LanguageData.GetContent(descpt);
            }
        }

        public static readonly string fileName = "xml/DragonEvents";
#if UNITY_IPHONE
		public static Dictionary<int, DragonEventsData> dataMap { get; set; }
#endif
	}
}