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
	public class MogoNotificationData : GameData
#else
    public class MogoNotificationData : GameData<MogoNotificationData>
#endif
	{
        public string time { get; protected set; }
        public int type { get; protected set; }
        public int tag { get; protected set; }
        public int titleBanner { get; protected set; }
        public int title { get; protected set; }
        public int content { get; protected set; }

        public string TitleBanner
        {
            get
            {
                return LanguageData.GetContent(titleBanner);
            }
        }
        public string Title
        {
            get
            {
                return LanguageData.GetContent(title);
            }
        }

        public string Content
        {
            get
            {
                return LanguageData.GetContent(content);
            }
        }

        public static readonly string fileName = "xml/notification";
#if UNITY_IPHONE
		public static Dictionary<int, MogoNotificationData> dataMap { get; set; }
#endif
	}
}