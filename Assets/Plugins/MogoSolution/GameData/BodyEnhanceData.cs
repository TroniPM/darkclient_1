/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：BodyEnhanceData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-4-10
// 模块描述：身体强化
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class BodyEnhanceData : GameData<BodyEnhanceData>
    {
        public int pos { get; protected set; }
        public int level { get; protected set; }
        public int star { get; protected set; }
        public float enhanceRate { get; protected set; }
        public int propertyEffectId { get; protected set; }//<职业，效果>
        public int characterLevel { get; protected set; }
        public Dictionary<int, int> material { get; protected set; }
        public int gold { get; protected set; }
        public int progress { get; protected set; } 

        static public readonly string fileName = "xml/BodyEnhanceData";
        //static public Dictionary<int, BodyEnhanceData> dataMap { get; set; }
    }
}