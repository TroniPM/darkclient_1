/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：LoadingTipsData
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-9-22
// 模块描述：加载提示
//----------------------------------------------------------------*/

using Mogo.Util;
using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class LoadingTipsData : GameData<LoadingTipsData>
    {
        public string content { get; protected set; }

        static public readonly string fileName = "xml/loadingTips";
        //static public Dictionary<int, LoadingTipsData> dataMap { get; set; }

        static public string GetTip(int tipId)
        {
            if (tipId == 0)
            {
                tipId = RandomHelper.GetRandomInt(0, dataMap.Count) + 1;
            }
            if (dataMap.ContainsKey(tipId))
            {
                return dataMap.Get(tipId).content;
            }
            return string.Empty;
        }

    }
}