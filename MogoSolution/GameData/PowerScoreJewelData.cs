#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class PowerScoreJewelData : GameData<PowerScoreJewelData>
    {
        public int level { get; protected set; }
        public int score { get; protected set; }

        static public readonly string fileName = "xml/PowerScoreJewel";
        //static public Dictionary<int, PowerScoreJewelData> dataMap { get; set; }

        static public int GetJewelScoreByLevel(int level)
        {
            if (dataMap.ContainsKey(level))
                return dataMap[level].score;

            return 0;
        }
    }
}
