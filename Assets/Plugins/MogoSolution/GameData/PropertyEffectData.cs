/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：PropertyEffectData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-4-10
// 模块描述：身体强化
//----------------------------------------------------------------*/

using Mogo.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class PropertyEffectData : GameData<PropertyEffectData>
    {
        public int hpBase { get; protected set; }
        public float hit { get; protected set; }
        public float crit { get; protected set; }
        public int critExtraAttack { get; protected set; }
        public float trueStrike { get; protected set; }
        public int antiDefense { get; protected set; }

        public int defenseBase { get; protected set; }
        public int earthDefense { get; protected set; }
        public int airDefense { get; protected set; }
        public int waterDefense { get; protected set; }
        public int fireDefense { get; protected set; }
        public int allElementsDefense { get; protected set; }

        public int attackBase { get; protected set; }
        public int earthDamage { get; protected set; }
        public int airDamage { get; protected set; }
        public int waterDamage { get; protected set; }
        public int fireDamage { get; protected set; }
        public int allElementsDamage { get; protected set; }

        public int pvpAddition { get; protected set; }
        public int pvpAnti { get; protected set; }


        public float hpAddRate { get; protected set; }
        public int attackAddRate { get; protected set; }
        public int speedAddRate { get; protected set; }
        public int damageReduceRate { get; protected set; }
        public int extraHitRate { get; protected set; }
        public int extraCritRate { get; protected set; }
        public int extraTrueStrikeRate { get; protected set; }
        public int extraExpRate { get; protected set; }
        public int extraGoldRate { get; protected set; }

        public int antiCrit { get; protected set; }
        public int antiTrueStrike { get; protected set; }
        public int cdReduce { get; protected set; }

        static public readonly string fileName = "xml/PropertyEffect";
        //static public Dictionary<int, PropertyEffectData> dataMap { get; set; }

        /// <summary>
        /// 得到一个不为0的属性
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public static float GetOneEffect(int _id)
        {
            LoggerHelper.Debug("effectID:" + _id);
            PropertyEffectData data = dataMap[_id];
            var props = data.GetType().GetProperties(~System.Reflection.BindingFlags.Static);
            foreach (var p in props)
            {
                //LoggerHelper.Debug("name" + p.Name);
                if (p.PropertyType != typeof(int) && p.PropertyType != typeof(float)) continue;
                object obj = p.GetGetMethod().Invoke(data, null);
                float i;
                if (p.PropertyType == typeof(int))
                    i = (int)obj;
                else
                    i = (float)obj;
                if ((i) > 0 && i != data.id) return i;
            }
            return 0;
        }

        public static int GetPVPAddition(int _id)
        {
            if (dataMap.ContainsKey(_id))
            {
                return dataMap[_id].pvpAddition;
            }
            else
            {
                LoggerHelper.Error("no contains effectID:" + _id);
                return 0;
            }
        }

        public static int GetPVPAnti(int _id)
        {
            if (dataMap.ContainsKey(_id))
            {
                return dataMap[_id].pvpAnti;
            }
            else
            {
                LoggerHelper.Error("no contains effectID:" + _id);
                return 0;
            }
        }

        public string GetOneEffectStr()
        {
            string attr = "";
            if (hpBase > 0) attr = LanguageData.dataMap[201].Format(hpBase);
            else if (hpAddRate > 0) attr = LanguageData.dataMap[250].Format(hpAddRate / 100f);
            else if (attackBase > 0) attr = LanguageData.dataMap[202].Format(attackBase);
            else if (attackAddRate > 0) attr = LanguageData.dataMap[251].Format(attackAddRate / 100f);
            else if (defenseBase > 0) attr = LanguageData.dataMap[203].Format(defenseBase / 100f);
            else if (speedAddRate > 0) attr = LanguageData.dataMap[253].Format(speedAddRate / 100f);
            else if (hit > 0) attr = LanguageData.dataMap[205].Format(hit);
            else if (crit > 0) attr = LanguageData.dataMap[204].Format(crit);
            else if (trueStrike > 0) attr = LanguageData.dataMap[206].Format(trueStrike);
            else if (critExtraAttack > 0) attr = LanguageData.dataMap[207].Format(critExtraAttack);
            else if (antiDefense > 0) attr = LanguageData.dataMap[208].Format(antiDefense);
            else if (antiTrueStrike > 0) attr = LanguageData.dataMap[210].Format(antiTrueStrike);
            else if (damageReduceRate > 0) attr = LanguageData.dataMap[252].Format(damageReduceRate);
            else if (cdReduce > 0) attr = LanguageData.dataMap[212].Format(cdReduce);
            else if (extraHitRate > 0) attr = LanguageData.dataMap[256].Format(extraHitRate / 100f);
            else if (extraCritRate > 0) attr = LanguageData.dataMap[255].Format(extraCritRate / 100f);
            else if (extraTrueStrikeRate > 0) attr = LanguageData.dataMap[257].Format(extraTrueStrikeRate / 100f);
            else if (pvpAddition > 0) attr = LanguageData.dataMap[258].Format(pvpAddition);
            else if (pvpAnti > 0) attr = LanguageData.dataMap[259].Format(pvpAnti);
            else if (extraExpRate > 0) attr = LanguageData.dataMap[260].Format(extraExpRate / 100f);
            else if (extraGoldRate > 0) attr = LanguageData.dataMap[261].Format(extraGoldRate / 100f);

            return attr; 
        }
    }
}