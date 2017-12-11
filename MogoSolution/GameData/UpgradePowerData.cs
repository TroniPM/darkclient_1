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
    public enum UpgradePowerSystem
    {
        Equipment = 1, // 装备
        JewelInset = 2, // 镶嵌
        Rune = 3, // 符文
        BodyEnhance = 4, // 强化
        Tong = 5,// 公会
    }

    public class UpgradePowerData : GameData<UpgradePowerData>
    {
        public int name { get; protected set; }
        public int icon { get; protected set; }
        public int hyper { get; protected set; }
        public int level { get; protected set; }
        public int text { get; protected set; }
        public int floatText { get; protected set; }

        static public readonly string fileName = "xml/UpgradePower";
        //static public Dictionary<int, UpgradePowerData> dataMap { get; set; }

        /// <summary>
        /// 获取ID列表
        /// </summary>
        /// <returns></returns>
        static public List<int> GetUpgradePowerIDList()
        {
            List<int> idList = new List<int>();
            foreach (int id in dataMap.Keys)
            {
                idList.Add(id);
            }

            return idList;
        }

        /// <summary>
        /// 通过ID获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static public UpgradePowerData GetUpgradePowerDataByID(int id)
        {
            if (dataMap.ContainsKey(id))
                return dataMap[id];
            return null;
        }
    }
}
