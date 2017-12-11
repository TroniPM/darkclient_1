/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：QualityColorData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-7-31
// 模块描述：
//----------------------------------------------------------------*/

using Mogo.Util;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Mogo.GameData
{
    public class QualityColorData : GameData<QualityColorData>
    {
        public string color { get; protected set; }
        static public readonly string fileName = "xml/QualityColor";

        //static public Dictionary<int, QualityColorData> dataMap { get; set; }
    }

    public struct SystemUIColorManager
    {
        // 红色
        public static readonly Color32 RED = new Color32(193, 10, 1, 255);
        // 绿色
        public static readonly Color32 GREEN = new Color32(96, 254, 0, 255);
        public static readonly Color32 GREEN_OUTLINE = new Color32(13, 52, 0, 255);
        // 黄色
        public static readonly Color32 YELLOW = new Color32(255, 210, 0, 255);
        public static readonly Color32 YELLOW_OUTLINE = new Color32(68, 41, 0, 255);
        // 褐色
        public static readonly Color32 BROWN = new Color32(63, 27, 4, 255);
        // 浅褐色
        public static readonly Color32 LIGHTBROWN = new Color32(100, 72, 56, 255);
        // 白色
        public static readonly Color32 WHITE = new Color32(255, 255, 255, 255);
        public static readonly Color32 WHITE_OUTLINE = new Color32(0, 0, 0, 255);

        // UI标题使用颜色
        public static readonly Color32 UITITLE = new Color32(255, 255, 255, 255);
        public static readonly Color32 UITITLE_OUTLINE = new Color32(50, 39, 9, 255);
    }
}
