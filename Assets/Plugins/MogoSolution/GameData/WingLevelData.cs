/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SpellData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-2-6
// 模块描述：翅膀数据结构
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
	public class WingLevelData : GameData<WingLevelData>
	{
        public int wingId { get; protected set; }
        public int level { get; protected set; }
        public int quality { get; protected set; }
        public int nextLevelExp { get; protected set; }
        public int propertEffectId { get; protected set; }
        public int trainDiamondCost { get; protected set; }
        public Dictionary<int, int> trainCost { get; protected set; }
        public Dictionary<int, int> trainWeight { get; protected set; }
        public Dictionary<int, int> trainValue { get; protected set; }
        public int levelExpAdd { get; protected set; }
        public int scale { get; protected set; }
        public Dictionary<int, int> buffId { get; protected set; }
        public int buffDesc { get; protected set; }

        static public readonly string fileName = "xml/WingLevel";

        static public WingLevelData GetLevelData(int id, int level)
        {
            WingLevelData rst = null;
            foreach (var item in dataMap)
            {
                if (item.Value.wingId == id && item.Value.level == level)
                {
                    rst = item.Value;
                    break;
                }
            }
            return rst;
        }

        public List<string> GetPropertEffect()
        {
            List<string> rst = new List<string>();
            PropertyEffectData d = PropertyEffectData.dataMap.Get(propertEffectId);
            if (d.hpBase > 0) rst.Add(LanguageData.dataMap[26813].Format(d.hpBase));
            if (d.attackBase > 0) rst.Add(LanguageData.dataMap[26814].Format(d.attackBase));
            if (d.defenseBase > 0) rst.Add(LanguageData.dataMap[26815].Format(d.defenseBase));
            if (d.crit > 0) rst.Add(LanguageData.dataMap[26816].Format(d.crit));
            if (d.trueStrike > 0) rst.Add(LanguageData.dataMap[26817].Format(d.trueStrike));
            if (d.critExtraAttack > 0) rst.Add(LanguageData.dataMap[26818].Format(d.critExtraAttack));
            if (d.antiDefense > 0) rst.Add(LanguageData.dataMap[26819].Format(d.antiDefense));
            if (d.antiCrit > 0) rst.Add(LanguageData.dataMap[26820].Format(d.antiCrit));
            if (d.antiTrueStrike > 0) rst.Add(LanguageData.dataMap[26821].Format(d.antiTrueStrike));
            if (d.extraCritRate > 0) rst.Add(LanguageData.dataMap[26822].Format(d.extraCritRate));
            if (d.extraTrueStrikeRate > 0) rst.Add(LanguageData.dataMap[26823].Format(d.extraTrueStrikeRate));
            if (d.damageReduceRate > 0) rst.Add(LanguageData.dataMap[26824].Format(d.damageReduceRate));
            if (d.pvpAddition > 0) rst.Add(LanguageData.dataMap[26825].Format(d.pvpAddition));
            if (d.pvpAnti > 0) rst.Add(LanguageData.dataMap[26826].Format(d.pvpAnti));
            return rst;
        }

        public PropertyEffectData GetProperEffectData()
        {
            return PropertyEffectData.dataMap.Get(propertEffectId);

        }
	}
}
