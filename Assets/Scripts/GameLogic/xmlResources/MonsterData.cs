/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MonsterData
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-2-6
// 模块描述：怪物数据结构
//----------------------------------------------------------------*/

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

namespace Mogo.GameData
{
    public class MonsterData : GameData<MonsterData>
    {
        public enum MonsterType
        {
            ordinary = 1,
            elite = 2,
            smallBoss = 3,
            bigBoss = 4,
            building = 5,
            crock = 6,
            crudeChest = 7,
            delicateChest = 8,
            PVP = 9
        }

        // Monster 数据
        public int nameCode { get; protected set; }
        public int raidId { get; protected set; }
        public int difficulty { get; protected set; }
        public int monsterType { get; protected set; }
        public int raidType { get; protected set; }
        public int hardType { get; protected set; }




        public int level { get; protected set; }
        public int model { get; protected set; }
        public List<int> bornFx { get; protected set; }
        public List<int> dieFx { get; protected set; }
        public int exp { get; protected set; }

        public int clientTrapId { get; protected set; }
        public int isClient { get; protected set; }
        public int aiId { get; protected set; }
        public int scaleRadius { get; protected set; }
        public List<int> skillIds { get; protected set; }
        public int see { get; protected set; }
        public int speed { get; protected set; }


        public int hpBase { get; protected set; }
        public int attackBase { get; protected set; }
        public int extraHitRate { get; protected set; }
        public int extraCritRate { get; protected set; }
        public int extraTrueStrikeRate { get; protected set; }
        public int extraAntiDefenceRate { get; protected set; }
        public int extraDefenceRate { get; protected set; }
        public int missRate { get; protected set; }

        public int notTurn { get; protected set; }
        public int deadTime { get; protected set; }
        public int bornTime { get; protected set; }
        public List<int> hitShader { get; protected set; }
        public int showHitAct { get; protected set; }
        public List<int> hpShow { get; protected set; }
        public int clientBoss { get; protected set; }

        public Dictionary<int, int> equ { get; protected set; }
        public List<int> gold { get; protected set; }
        public int goldStack { get; protected set; }
        public Dictionary<int, int> extraRandom { get; protected set; }
        public Dictionary<int, int> extraDrop { get; protected set; }
        public int goldChance { get; protected set; }


        public int shader { get; protected set; }


        public bool loadedExtraData { get; set; }

        static public readonly string fileName = "xml/Monster";
        //static public Dictionary<int, MonsterData> dataMap { get; set; }

        public MonsterData()
        {
            hpBase = 1;
            clientBoss = 0;
            loadedExtraData = false;
            see = Mogo.Game.AISpecialEnum.DefaultSee;
        }
        static public bool IsHpShow(int id)
        {
            var data = dataMap.Get(id);
            if (data.hpShow != null && data.hpShow.Count == 1 && data.hpShow[0] == 1)
            {
                return true;
            }
            return false;
        }

        static public MonsterData MercenaryDataFactory(int model, int notTurn,
                    int bornFx,
                    int dieFx,
                    int aiId,
                    int bornTime,
            List<int> skills)
        {
            MonsterData newNode = new MonsterData();
            newNode.model = model;
            newNode.notTurn = notTurn;
            newNode.bornFx = new List<int>() { bornFx };
            newNode.dieFx = new List<int>() { dieFx };
            newNode.aiId = aiId;
            newNode.see = Mogo.Game.AISpecialEnum.DefaultSee;
            newNode.bornTime = bornTime;
            newNode.skillIds = skills;
            newNode.scaleRadius = 30;
            return newNode;
        }

        static public MonsterData GetData(int id, int monsterDifficulty = 0)
        {

            //if (id == 42201) Mogo.Util.LoggerHelper.Error("GetData start:" + id + " monsterDifficulty:" + monsterDifficulty);
            MonsterData dst = dataMap[id];
            //if (id == 42201) Mogo.Util.LoggerHelper.Error("GetData 1.1:" + id);
            if (!dst.loadedExtraData)
            {
                //if (id == 42201) Mogo.Util.LoggerHelper.Error("GetData 1.2:" + id);

                if (AvatarModelData.dataMap.ContainsKey(dst.model))
                {
                    //if (id == 42201) Mogo.Util.LoggerHelper.Error("GetData 1.3:" + id);
                    AvatarModelData avatarModelData = AvatarModelData.dataMap[dst.model];
                    dst.bornFx = avatarModelData.bornFx;
                    dst.dieFx = avatarModelData.dieFx;
                    dst.deadTime = avatarModelData.deadTime;
                    dst.bornTime = avatarModelData.bornTime;
                    dst.scaleRadius = avatarModelData.scaleRadius;
                    dst.speed = avatarModelData.speed;
                    dst.notTurn = avatarModelData.notTurn;
                }
                //if (id == 42201) Mogo.Util.LoggerHelper.Error("GetData 2:" + id + " dst.monsterType:" + dst.monsterType);
                foreach (MonsterValueData child in MonsterValueData.dataMap.Values)
                {
                    if ((!(dst.monsterType >= 5 && dst.monsterType <= 8) && child.raidId == dst.raidId && child.difficulty == dst.difficulty && child.monsterType == dst.monsterType) ||
                        (dst.monsterType >= 5 && dst.monsterType <= 8 && child.monsterType == dst.monsterType))
                    {
                        //if (id == 42201) Mogo.Util.LoggerHelper.Error("GetData 3:" + id + " hpBase" + child.hpBase + " atkb:" + child.attackBase);
                        dst.raidId = child.raidId;
                        dst.raidType = child.raidType;
                        dst.difficulty = child.difficulty;
                        dst.monsterType = child.monsterType;
                        dst.hardType = child.hardType;
                        dst.hpBase = child.hpBase;
                        dst.attackBase = child.attackBase;
                        dst.extraHitRate = child.extraHitRate;
                        dst.extraCritRate = child.extraCritRate;
                        dst.extraTrueStrikeRate = child.extraTrueStrikeRate;
                        dst.extraAntiDefenceRate = child.extraAntiDefenceRate;
                        dst.extraDefenceRate = child.extraDefenceRate;
                        dst.missRate = child.missRate;
                        dst.exp = child.exp;
                        dst.level = child.level;
                        dst.equ = child.equ;
                        dst.gold = child.gold;
                        dst.goldStack = child.goldStack;
                        dst.extraRandom = child.extraRandom;
                        dst.extraDrop = child.extraDrop;
                        dst.goldChance = child.goldChance;

                        //if (id == 42201) Mogo.Util.LoggerHelper.Error("GetData:" + id + " raidId:" + child.raidId + " child.difficulty:" + child.difficulty + " monsterType:" + child.monsterType + " hpBase:" + child.hpBase);
                        dst.loadedExtraData = true;
                        break;
                    }
                }

                if (!dst.loadedExtraData)
                { 
                    //没加载成功MonsterValue,单还是让它出来
                    //if (id == 42201) Mogo.Util.LoggerHelper.Error("MonsterValue Loaded Failed monsterId:" + id);
                }
            }


            if (monsterDifficulty <= 0)
            { //是普通副本怪物(非随机属性怪物)

                return dst;
            }
            else
            {
                //是随机怪物
                int rid = id * 10000 + monsterDifficulty * 10 + dst.monsterType;
                if (dataMap.ContainsKey(rid))
                {
                    //if (id == 42201) LoggerHelper.Error("old ram:" + dataMap[rid].hpBase);
                    return dataMap[rid];
                }
                else
                {

                    //需要init随机怪物属性
                    MonsterData ranDst = new MonsterData();
                    ranDst.loadedExtraData = true;
                    ranDst.id = dst.id;
                    ranDst.raidId = dst.raidId;
                    ranDst.difficulty = dst.difficulty;
                    ranDst.monsterType = dst.monsterType;
                    ranDst.model = dst.model;
                    ranDst.clientTrapId = dst.clientTrapId;
                    ranDst.isClient = dst.isClient;
                    ranDst.aiId = dst.aiId;
                    ranDst.scaleRadius = dst.scaleRadius;
                    ranDst.skillIds = dst.skillIds;
                    ranDst.shader = dst.shader;
                    ranDst.showHitAct = dst.showHitAct;
                    ranDst.hpShow = dst.hpShow;
                    ranDst.hitShader = dst.hitShader;
                    ranDst.clientBoss = dst.clientBoss;

                    ranDst.bornFx = dst.bornFx;
                    ranDst.dieFx = dst.dieFx;
                    ranDst.deadTime = dst.deadTime;
                    ranDst.bornTime = dst.bornTime;
                    ranDst.scaleRadius = dst.scaleRadius;
                    ranDst.speed = dst.speed;
                    ranDst.notTurn = dst.notTurn;


                    foreach (MonsterValueData child in MonsterValueData.dataMap.Values)
                    {
                        if ((!(dst.monsterType >= 5 && dst.monsterType <= 8) && child.raidId == Mogo.Game.RandomFB.RAIDID && child.difficulty == monsterDifficulty && child.monsterType == dst.monsterType) ||
                            (dst.monsterType >= 5 && dst.monsterType <= 8 && child.monsterType == dst.monsterType))
                        {
                            //Mogo.Util.LoggerHelper.Error("GetData 3:" + id + " hpBase" + child.hpBase + " atkb:" + child.attackBase);
                            //ranDst.raidId = child.raidId;
                            ranDst.raidType = child.raidType;
                            ranDst.difficulty = child.difficulty;
                            ranDst.monsterType = child.monsterType;
                            ranDst.hardType = child.hardType;
                            ranDst.hpBase = child.hpBase;
                            ranDst.attackBase = child.attackBase;
                            ranDst.extraHitRate = child.extraHitRate;
                            ranDst.extraCritRate = child.extraCritRate;
                            ranDst.extraTrueStrikeRate = child.extraTrueStrikeRate;
                            ranDst.extraAntiDefenceRate = child.extraAntiDefenceRate;
                            ranDst.extraDefenceRate = child.extraDefenceRate;
                            ranDst.missRate = child.missRate;
                            ranDst.exp = child.exp;
                            ranDst.level = child.level;
                            ranDst.equ = child.equ;
                            ranDst.gold = child.gold;
                            ranDst.goldStack = child.goldStack;
                            ranDst.extraRandom = child.extraRandom;
                            ranDst.extraDrop = child.extraDrop;
                            ranDst.goldChance = child.goldChance;

                            //Mogo.Util.LoggerHelper.Error("Random:" + id + " raidId:" + child.raidId + " child.difficulty:" + monsterDifficulty + " monsterType:" + child.monsterType + " hpBase:" + child.hpBase);
                            //LoggerHelper.Error("new ram:" + ranDst.hpBase);
                            break;
                        }
                    }

                    dataMap[rid] = ranDst;
                    return ranDst;
                }
            }
        }

        public static int GetMonsterID(int mission, int level, int type)
        {
            return MonsterData.dataMap.FirstOrDefault(t => t.Value.raidId == mission && t.Value.difficulty == level && t.Value.monsterType == type).Key;
        }

        public static void getDrop(Dictionary<int, int> tblDstDropsItem, List<int> tblDstMoney, int monsterId, int vocation)
        {
            //LoggerHelper.Error("monsterId" + monsterId + " vocation:" + vocation);
            MonsterData tmpCfgData = MonsterData.GetData((int)monsterId);

            Dictionary<int, int> tblDstDrops = new Dictionary<int, int>();

            //判断tmpCfgData.equ是否为空
            if (tmpCfgData.equ != null && tmpCfgData.equ.Count > 0)
                foreach (KeyValuePair<int, int> subCfgData in tmpCfgData.equ)
                {
                    //LoggerHelper.Error("subCfgData equ:" + subCfgData.Key + "  " +  subCfgData.Value);
                    if (RandomHelper.GetRandomInt(10000) <= subCfgData.Value)
                    {
                        if (!tblDstDrops.ContainsKey(subCfgData.Key))
                            tblDstDrops[subCfgData.Key] = 0;
                        tblDstDrops[subCfgData.Key] = tblDstDrops[subCfgData.Key] + 1;
                    }
                }

            //判断tmpCfgData.equ是否为空
            if (tmpCfgData.extraRandom != null && tmpCfgData.extraRandom.Count > 0)
                foreach (KeyValuePair<int, int> subExtraRandom in tmpCfgData.extraRandom)
                {
                    //LoggerHelper.Error("subCfgData extraRandom:" + subExtraRandom.Key + "  " + subExtraRandom.Value);
                    //LoggerHelper.Error("subCfgData extraDrop:" + tmpCfgData.extraDrop[subExtraRandom.Key]);
                    if (tmpCfgData.extraDrop.ContainsKey(subExtraRandom.Key) && RandomHelper.GetRandomInt(10000) <= subExtraRandom.Value)
                    {
                        if (!tblDstDropsItem.ContainsKey(subExtraRandom.Key))
                            tblDstDropsItem[subExtraRandom.Key] = 0;
                        tblDstDropsItem[subExtraRandom.Key] = tblDstDropsItem[subExtraRandom.Key] + tmpCfgData.extraDrop[subExtraRandom.Key];
                    }
                }

            foreach (KeyValuePair<int, int> subDstDrops in tblDstDrops)
            {
                for (int i = 0; i < subDstDrops.Value; i++)
                {
                    //创建掉落
                    DropData.getAwards(tblDstDropsItem, subDstDrops.Key, vocation);
                }
            }


            if (tmpCfgData.gold != null &&
                tmpCfgData.gold.Count >= 2 &&
                tmpCfgData.gold[0] < tmpCfgData.gold[1] &&
                RandomHelper.GetRandomInt(10000) < tmpCfgData.goldChance)
            {
                int awardGold = RandomHelper.GetRandomInt(tmpCfgData.gold[0], tmpCfgData.gold[1]);
                //LoggerHelper.Error("awardGold" + awardGold +" "+ tmpCfgData.gold[0] + " " +tmpCfgData.gold[1]);
                int averageGold = (int)System.Math.Ceiling((double)awardGold / tmpCfgData.goldStack);
                for (int i = 0; i < tmpCfgData.goldStack; i++)
                {
                    tblDstMoney.Add(averageGold);
                }
            }


            //debug
            foreach (KeyValuePair<int, int> subDstDropsItem in tblDstDropsItem)
            {
                //LoggerHelper.Error("want getDrop item:" + subDstDropsItem.Key + " num:" + subDstDropsItem.Value);
            }
            foreach (int money in tblDstMoney)
            {
                //LoggerHelper.Error("want getDrop money:" + money);
            }
        }
    }
}
