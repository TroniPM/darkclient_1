/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：buff表
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013-7-10
// 模块描述：
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class SkillBuffData : GameData<SkillBuffData>
    {
        public int name { get; protected set; }
        public int show { get; protected set; }
        public int showPriority { get; protected set; }
        public int removeMode { get; protected set; }
        public int totalTime { get; protected set; }
        public List<int> excludeBuff { get; protected set; }
        public List<int> replaceBuff { get; protected set; }
        public List<int> appendState { get; protected set; }
        public Dictionary<int, int> activeSkill { get; protected set; }
        public Dictionary<string, int> attrEffect { get; protected set; }
        public int sfx { get; protected set; }
        public int notifyEvent { get; protected set; }
        public int vipLevel { get; protected set; }
        public int tips { get; protected set; }

        static public readonly string fileName = "xml/SkillBuff";
        //static public Dictionary<int, SkillBuffData> dataMap { get; set; }

        public static string GetName(int id)
        {
            string rst = "no name";
            if (!dataMap.ContainsKey(id))
            {
                return rst;
            }
            if (!LanguageData.dataMap.ContainsKey(dataMap[id].name))
            {
                return rst;
            }
            return LanguageData.GetContent(dataMap[id].name);
        }
    }
}