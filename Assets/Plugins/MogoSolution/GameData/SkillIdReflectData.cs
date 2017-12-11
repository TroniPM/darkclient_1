/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MonsterData
// 创建者：Kevin Hua
// 修改者列表：
// 创建日期：2013-10-18
// 模块描述：怪物技能数据结构
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class SkillIdReflectData : GameData<SkillIdReflectData>
    {
        public int avatarSkillId { get; protected set; }
        public int mercenarySkillId { get; protected set; }
        public int aiSlot { get; protected set; }

        static public readonly string fileName = "xml/SkillIdReflect";

        public SkillIdReflectData()
        {
        }

        public static int GetReflectSkillId(int srcId)
        {
            foreach (SkillIdReflectData v in dataMap.Values)
            {
                if (v.avatarSkillId == srcId)
                    return v.mercenarySkillId;
            }
            return -1;
        }
    }
}