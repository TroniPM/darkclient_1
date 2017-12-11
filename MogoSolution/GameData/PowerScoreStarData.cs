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
using Mogo.Util;

namespace Mogo.GameData
{
    public class PowerScoreStarData : GameData<PowerScoreStarData>
    {
        public int star { get; protected set; }
        public List<float> range { get; protected set; }

        static public readonly string fileName = "xml/PowerScoreStar";
        //static public Dictionary<int, PowerScoreStarData> dataMap { get; set; }

        /// <summary>
        /// 检查行数据数据
        /// </summary>
        /// <returns></returns>
        readonly static int STARCOUNT = 3;      
        static private bool CheckRowData()
        {
            int starNum = 1;
            for (starNum = 1; starNum <= STARCOUNT; starNum++)
            {
                if (!dataMap.ContainsKey(starNum))
                {
                    LoggerHelper.Error("no contains key = " + starNum);
                    return false;
                }
            }

            if (starNum - 1 == STARCOUNT)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 检查系数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static private bool CheckModulus(int id)
        {
            int starNum = 1;
            for (starNum = 1; starNum <= STARCOUNT; starNum++)
            {
                if (id > dataMap[starNum].range.Count)
                {
                    LoggerHelper.Error("out of bounds, id = " + id);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 通过系数计算星级
        /// </summary>
        /// <param name="modulus"></param>
        /// <returns></returns>
        static public int CalStarNumByModulus(float modulus, int id)
        {
            if (modulus < 0) modulus = 0f;
            if (modulus > 1) modulus = 1f;
            if (!CheckRowData())
                return 0;
            if (!CheckModulus(id))
                return 0;

            if (modulus >= 0 && modulus < dataMap[1].range[id - 1])
                return 1;
            else if (modulus >= dataMap[1].range[id - 1] && modulus < dataMap[2].range[id - 1])
                return 2;
            else if (modulus > dataMap[2].range[id - 1] && modulus <= dataMap[3].range[id - 1])
                return 3;
            else
                return 0;
        }

        /// <summary>
        /// 通过系数计算分数百分比
        /// S=(X-A)/(B-A)
        /// </summary>
        /// <param name="modulus"></param>
        /// <returns></returns>
        static public float CalScoreProgressByModulus(float modulus, int id)
        {
            if (modulus < 0) modulus = 0f;
            if (modulus > 1) modulus = 1f;
            if (!CheckRowData())
                return 0;
            if (!CheckModulus(id))
                return 0;

            float A = 0f;
            float B = 0f;
            if (modulus >= 0 && modulus < dataMap[1].range[id - 1])
            {
                A = 0;
                B = dataMap[1].range[id - 1];
            }
            else if (modulus >= dataMap[1].range[id - 1] && modulus < dataMap[2].range[id - 1])
            {
                A = dataMap[1].range[id - 1];
                B = dataMap[2].range[id - 1];
            }
            else if (modulus > dataMap[2].range[id - 1] && modulus <= dataMap[3].range[id - 1])
            {
                A = dataMap[2].range[id - 1];
                B = dataMap[3].range[id - 1];
            }
            else
            {
                return 0;
            }

            return (modulus - A) / (B - A);
        }
    }
}
