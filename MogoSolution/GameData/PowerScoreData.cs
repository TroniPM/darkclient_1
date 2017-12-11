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
    public class PowerScoreData : GameData<PowerScoreData>
    {
        public int level { get; protected set; }
        public List<int> score { get; protected set; }

        static public readonly string fileName = "xml/PowerScore";
        //static public Dictionary<int, PowerScoreData> dataMap { get; set; }

        /// <summary>
        /// 通过玩家等级获得评分数据
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        static public PowerScoreData GetPowerScoreDataByLevel(int level)
        {
            if (dataMap.ContainsKey(level))
                return dataMap[level];
            return null;
        }

        /// <summary>
        /// 通过玩家等级获得装备满分值
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        static public int GetScoreEquipByLevel(int level)
        {
            PowerScoreData powerScoreData = GetPowerScoreDataByLevel(level);
            if (powerScoreData != null)
                return powerScoreData.score[0];
            return 0;
        }

        /// <summary>
        /// 通过玩家等级获得宝石满分值
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        static public int GetScoreDiamondByLevel(int level)
        {
            PowerScoreData powerScoreData = GetPowerScoreDataByLevel(level);
            if (powerScoreData != null)
                return powerScoreData.score[1];
            return 0;
        }

        /// <summary>
        /// 通过玩家等级获得符文满分值
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        static public int GetScoreRuneByLevel(int level)
        {
            PowerScoreData powerScoreData = GetPowerScoreDataByLevel(level);
            if (powerScoreData != null)
                return powerScoreData.score[2];
            return 0;
        }

        /// <summary>
        /// 通过玩家等级获得强化满分值
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        static public int GetScoreBodyEnhanceByLevel(int level)
        {
            PowerScoreData powerScoreData = GetPowerScoreDataByLevel(level);
            if (powerScoreData != null)
                return powerScoreData.score[3];
            return 0;
        }

        /// <summary>
        /// 通过玩家等级获得公会满分值
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        static public int GetScoreTongByLevel(int level)
        {
            PowerScoreData powerScoreData = GetPowerScoreDataByLevel(level);
            if (powerScoreData != null)
                return powerScoreData.score[4];
            return 0;
        }
    }
}
