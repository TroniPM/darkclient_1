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
    public class UpgradeGuideData : GameData<UpgradeGuideData>
    {
        public int type { get; protected set; }
        public int title { get; protected set; }
        public int describtion { get; protected set; }
        public int icon { get; protected set; }

        static public readonly string fileName = "xml/UpgradeGuide";
        //static public Dictionary<int, UpgradeGuideData> dataMap { get; set; }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static public UpgradeGuideData GetData(int id)
        {
            if (dataMap.ContainsKey(id))
                return dataMap[id];
            return null;
        }

        /// <summary>
        /// 获取体力不足其他玩法列表
        /// type = 2
        /// </summary>
        /// <returns></returns>
        static public List<int> GetNoNeedEnergyList()
        {
            List<int> idList = new List<int>();
            foreach (KeyValuePair<int, UpgradeGuideData> pair in dataMap)
            {
                if (pair.Value.type == 2)
                    idList.Add(pair.Key);
            }

            return idList;
        }

        /// <summary>
        /// 获取等级不足引导列表
        /// type = 1
        /// </summary>
        /// <returns></returns>
        static public List<int> GetLevelNoEnoughList()
        {
            List<int> idList = new List<int>();
            foreach (KeyValuePair<int, UpgradeGuideData> pair in dataMap)
            {
                if (pair.Value.type == 1)
                    idList.Add(pair.Key);
            }

            return idList;
        }
    }
}
