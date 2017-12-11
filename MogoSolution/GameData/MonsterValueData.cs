/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MonsterData
// 创建者：Kevin Hua
// 修改者列表：
// 创建日期：2013-10-18
// 模块描述：怪物数据结构
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class MonsterValueData : GameData<MonsterValueData>
    {
        // MonsterValue 数据
        public int raidId { get; protected set; }
        public int raidType { get; protected set; }
        public int difficulty { get; protected set; }
        public int monsterType { get; protected set; }
        public int hardType { get; protected set; }

        public int hpBase { get; protected set; }
        public int attackBase { get; protected set; }
        public int extraHitRate { get; protected set; }
        public int extraCritRate { get; protected set; }
        public int extraTrueStrikeRate { get; protected set; }
        public int extraAntiDefenceRate { get; protected set; }
        public int extraDefenceRate { get; protected set; }
        public int missRate { get; protected set; }
        public int exp { get; protected set; }

        public Dictionary<int, int> equ { get; protected set; }
        public List<int> gold { get; protected set; }
        public int goldStack { get; protected set; }
        public Dictionary<int, int> extraRandom { get; protected set; }
        public Dictionary<int, int> extraDrop { get; protected set; }
        public int goldChance { get; protected set; }

        public int level { get; protected set; }



        static public readonly string fileName = "xml/MonsterValue";
        //static public Dictionary<int, MonsterValueData> dataMap { get; set; }

        public MonsterValueData()
        {
            raidId = 0;
        }

        public static MonsterValueData GetData(int raidId, int difficulty)
        {
            return dataMap.FirstOrDefault(t => t.Value.raidId == raidId && t.Value.difficulty == difficulty).Value;
        }
    }
}