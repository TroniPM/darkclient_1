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
    public class EquipRecommendData : GameData<EquipRecommendData>
    {
        public int star { get; protected set; }
        public int vocation { get; protected set; }
        public List<int> level { get; protected set; }
        public List<int> goodsid { get; protected set; }
        public List<int> access { get; protected set; }
        public List<int> accessType { get; protected set; }

        static public readonly string fileName = "xml/EquipRecommend";
        //static public Dictionary<int, EquipRecommendData> dataMap { get; set; }	

        /// <summary>
        /// 根据玩家职业和等级从配置表中读取data
        /// </summary>
        /// <param name="vocation"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        static public EquipRecommendData GetEquipRecommendData(int vocation, int level)
        {
            foreach (EquipRecommendData data in dataMap.Values)
            {
                if (data.vocation == vocation && level >= data.level[0] && level <= data.level[1])
                    return data;
            }

            return null;
        }        
	}
}
