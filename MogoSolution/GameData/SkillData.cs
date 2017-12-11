/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SpellData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-2-6
// 模块描述：技能数据结构
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class SkillData : GameData<SkillData>
    {
        // Spell 数据

        // 通用属性
        public int name { get; protected set; }
        public int level { get; protected set; }
        public int desc { get; protected set; }
        public int icon { get; protected set; }
        public int posi { get; protected set; }
        public int vocation { get; protected set; }
        public int groupID { get; protected set; }
        public int weapon { get; protected set; }
        public List<int> dependSkill { get; protected set; }

        // 学习限制类
        public int learnLimit { get; protected set; }
        public int limitLevel { get; protected set; }
        public int limitVocation { get; protected set; }
        public float extraHarm { get; protected set; }
        public float extraRate { get; protected set; }
        public int canLearn { get; protected set; }
        public int skillTreeID { get; protected set; }
        public int nextSkill { get; protected set; }
        public int moneyCost { get; protected set; }
        public int pvpCreditCost { get; protected set; }

        // 施放条件类
        public List<int> cd { get; protected set; }
        public int castTime { get; protected set; }
        public int castRange { get; protected set; }
        public List<int> dependBuff { get; protected set; }
        public List<int> independBuff { get; protected set; }

        public int activeBuffMode { get; protected set; }
        public int activeBuff { get; protected set; }
        public List<int> skillAction { get; protected set; }

        public int totalActionDuration = 0;

        static public readonly string fileName = "xml/SkillData";
        //static public Dictionary<int, SkillData> dataMap { get; set; }

        public static SkillData GetStudySkillByVocationAndWeapon(int vocation, int weapon, int posi)
        {
            foreach (var item in dataMap)
            {
                if (item.Value.limitVocation == vocation &&
                    item.Value.weapon == weapon &&
                    //item.Value.canLearn == 1 &&
                    item.Value.posi == posi)
                {
                    return item.Value;
                }
            }
            return null;
        }


        public static SkillData GetSkillByVLP(int vocation, int posi, int level)
        {
            foreach (var item in dataMap)
            {
                if (item.Value.limitVocation == vocation &&
                    item.Value.weapon == 0 && 
                    //item.Value.canLearn == 1 &&
                    item.Value.posi == posi &&
                    item.Value.level == level)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static SkillData GetSkillByVWLP(int vocation, int weapon, int posi, int level)
        {
            foreach (var item in dataMap)
            {
                if (item.Value.limitVocation == vocation &&
                    item.Value.weapon == weapon &&
                    //item.Value.canLearn == 1 &&
                    item.Value.posi == posi &&
                    item.Value.level == level)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public static int GetTotalActionDuration(int skillId)
        {
            SkillData tmpSkillData = dataMap[skillId];
            if (tmpSkillData.totalActionDuration == 0)
            {
                foreach (var tmpSkillActionId in tmpSkillData.skillAction)
                {
                    tmpSkillData.totalActionDuration += SkillAction.dataMap[tmpSkillActionId].actionBeginDuration;
                    tmpSkillData.totalActionDuration += SkillAction.dataMap[tmpSkillActionId].actionEndDuration;
                }
            }
            return tmpSkillData.totalActionDuration;
        }
    }

}