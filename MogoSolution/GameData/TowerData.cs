/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TowerData
// 创建者：Charles Zuo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class TowerXMLData : GameData<TowerXMLData>
    {
        public Dictionary<int, int> item { get; protected set; }
        public int icon { get; set; }
        static public readonly string fileName = "xml/TowerReward";
        //static public Dictionary<int, TowerXMLData> dataMap { get; set; }
    }
}
