/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DragonStationData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class DragonStationData : GameData
#else
    public class DragonStationData : GameData<DragonStationData>
#endif
	{
        public int addFactor { get; protected set; }
        public int name { get; protected set; }
        public int descpt { get; protected set; }
        public int rewardAddition { get; protected set; }

        public string Name
        {
            get
            {
                return LanguageData.GetContent(name);
            }
        }

        public string Descpt
        {
            get
            {
                return LanguageData.GetContent(descpt);
            }
        }

        public static readonly string fileName = "xml/DragonStation";
#if UNITY_IPHONE
        public static Dictionary<int, DragonStationData> dataMap { get; set; }
#endif
	}
}