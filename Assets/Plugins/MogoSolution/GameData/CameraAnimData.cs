/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：CameraAnimData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-6-7
// 模块描述：
//----------------------------------------------------------------*/

using Mogo.Util;
using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class CameraAnimData : GameData
#else
    public class CameraAnimData : GameData<CameraAnimData>
#endif
	{
        public float xSwing { get; set; }
        public int xRate { get; set; }

        public float ySwing { get; set; }
        public int yRate { get; set; }

        public float zSwing { get; set; }
        public int zRate { get; set; }

        static public readonly string fileName = "xml/CameraAnim";
#if UNITY_IPHONE
        static public Dictionary<int, CameraAnimData> dataMap { get; set; }
#endif
	}
}
