using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;
using Mogo.FSM;

public class CalculateDamage
{

    // 计算攻击伤害。 攻击者可能是 玩家或者 dummy 对象。
    // 受击者，则反之。
    public static List<int> CacuDamage(int hitActionID, uint attackerID, uint victimID)
    {
        bool isDebugInfo = false;
        List<int> result = new List<int>();
        EntityParent attacker = null;
        EntityParent defender = null;
        double levelCorrect = 1.00f;
        if (attackerID == MogoWorld.thePlayer.ID && MogoWorld.Entities.ContainsKey(victimID))
        {
            //玩家打dummy
            attacker = MogoWorld.thePlayer;
            defender = MogoWorld.Entities[victimID] as EntityDummy;
            double levelGap = defender.level - attacker.level;
            if (levelGap>=20)
            {
                levelCorrect = 0.1f;
            }
            else if (levelGap > 10 && levelGap<20)
            {
                levelCorrect = 1 - levelGap * 0.05f;
            }
        }
        else if (victimID == MogoWorld.thePlayer.ID && MogoWorld.Entities.ContainsKey(attackerID))
        {
            //dummy打玩家
            defender = MogoWorld.thePlayer;
            attacker = MogoWorld.Entities[attackerID] as EntityDummy;
            
        }
        else if (MogoWorld.Entities[attackerID] is EntityDummy && MogoWorld.Entities[victimID] is EntityMercenary)
        {
            //dummy打Mercenary
            attacker = MogoWorld.Entities[attackerID] as EntityDummy;
            defender = MogoWorld.Entities[victimID] as EntityMercenary;
           
        }
        else if (MogoWorld.Entities[attackerID] is EntityMercenary)
        {
            //Mercenary打dummy
            attacker = MogoWorld.Entities[attackerID] as EntityMercenary;
            defender = MogoWorld.Entities[victimID] as EntityDummy;
            isDebugInfo = true;
        }
        else
        {
            LoggerHelper.Error("CacuDamage Error. eid not exist.");
            result.Add(-1);
            result.Add(0);
            return result;
        }

        
        if (RandomHelper.GetRandomFloat() > GetHitRate(attacker, defender))
        {
            result.Add(1);
            result.Add(0);
            return result;
        }


        bool critFlag = false;
        bool strikeFlag = false;
        var atk = GetProperty(attacker, "atk"); //角色伤害值
        double extraCritDamage = 0.00f;
        double dmgCorrect = 1.00f;	//减伤修正，破击时为1
        double dmgMultiply = 1.00f;
        if (RandomHelper.GetRandomFloat() <= GetCritRate(attacker, defender))
        {
            //发生暴击
            critFlag = true;
            extraCritDamage = GetProperty(attacker, "critExtraAttack");
            dmgCorrect = GetDmgCorrect(attacker, defender);
            dmgMultiply = 1 + 0.2f;
        }
        else if (RandomHelper.GetRandomFloat() <= GetTrueStrikeRate(attacker, defender))
        {
            //发生破击
            strikeFlag = true;
        }
        else
        {
            dmgCorrect = GetDmgCorrect(attacker, defender);
        }
        // 4 暴击 3破击 2普通攻击 1miss
        int retFlag;
        if (strikeFlag)
        {
            retFlag = 3;
        }
        else if (critFlag)
        {
            retFlag = 4;
        }
        else
        {
            retFlag = 2;
        }
        ///技能数据伤害   对应 伤害*技能比例+技能加成固定值
        SkillAction skillData = SkillAction.dataMap[hitActionID];
        var skillMul = skillData.damageMul;
        var skillAdd = skillData.damageAdd;
        var dmgNormal = ((atk * (skillMul) * dmgMultiply + skillAdd) + extraCritDamage) * dmgCorrect;

        var dmgAll = dmgNormal + GetElemDamage(attacker, defender);

        var damageReduce = GetProperty(defender, "damageReduceRate");
        int pvpCorrect = 1;
        result.Add(retFlag);
        result.Add((int)(Math.Ceiling(dmgAll * (1 - damageReduce) * pvpCorrect * levelCorrect * RandomHelper.GetRandomFloat(0.90f, 1.10f))));
        if (isDebugInfo)
        {
            //LoggerHelper.Error("dmgAll:" + dmgAll + " dmgNormal:" + dmgNormal + " atk:" + atk + " skillMul:" + skillMul + " skillAdd:" + skillAdd + " dmgCorrect:" + dmgCorrect + " damageReduce:" + damageReduce + " result:" + (Math.Ceiling(dmgAll / temp * RandomHelper.GetRandomFloat(0.90f, 1.10f))));
        }
        return result;
    }

    public static Double GetProperty(EntityParent entity, string name)
    {
        Double value;
        var prop = entity.GetType().GetProperty(name);
        if (prop != null)
        {
            value = Convert.ToDouble(prop.GetGetMethod().Invoke(entity, null));
        }
        else
        {
            //先在double里面找一遍，没有的话再在int里找一遍，再没有就0
            value = entity.DoubleAttrs.GetValueOrDefault(name, entity.IntAttrs.GetValueOrDefault(name, 0));
        }
        return value;
    }

    public static double GetHitRate(EntityParent attacker, EntityParent defender)
    {
        var hit = GetProperty(attacker, "hitRate");
        var miss = GetProperty(defender, "missRate");
        return hit - miss > 0 ? hit - miss : 0;
    }

    public static double GetTrueStrikeRate(EntityParent attacker, EntityParent defender)
    {
        //破击率
        var trueStrike = GetProperty(attacker, "trueStrikeRate");
        var antiTrueStrike = GetProperty(defender, "antiTrueStrikeRate");

        return trueStrike - antiTrueStrike > 0 ? trueStrike - antiTrueStrike : 0;
    }

    public static double GetCritRate(EntityParent attacker, EntityParent defender)
    {
        //暴击率
        var crit = GetProperty(attacker, "critRate");
        var antiCrit = GetProperty(defender, "antiCritRate");

        return crit - antiCrit > 0 ? crit - antiCrit : 0;
    }

    public static double GetDmgCorrect(EntityParent attacker, EntityParent defender)
    {
        //伤害修正值（没发生破击时）
        var def = GetProperty(defender, "defenceRate");
        var antiDefense = GetProperty(attacker, "antiDefenseRate");

        return 1 - def + antiDefense;
    }

    public static double GetElemDamage(EntityParent attacker, EntityParent defender)
    {
        //计算元素伤害
        var allElementsDamage = GetProperty(attacker, "allElementsDamage");
        var allElementsDefense = GetProperty(defender, "allElementsDefense");
        var all = allElementsDamage - allElementsDefense;

        var earthDamage = GetProperty(attacker, "earthDamage");
        var earthDefense = GetProperty(defender, "earthDefense");
        var earth = earthDamage - earthDefense + all;
        if (earth < 0) { earth = 0; }

        var airDamage = GetProperty(attacker, "airDamage");
        var airDefense = GetProperty(defender, "airDefense");
        var air = airDamage - airDefense + all;
        if (air < 0) { air = 0; }

        var waterDamage = GetProperty(attacker, "waterDamage");
        var waterDefense = GetProperty(defender, "waterDefense");
        var water = waterDamage - waterDefense + all;
        if (water < 0) { water = 0; }

        var fireDamage = GetProperty(attacker, "fireDamage");
        var fireDefense = GetProperty(defender, "fireDefense");
        var fire = fireDamage - fireDefense + all;
        if (fire < 0) { fire = 0; }

        return earth + air + water + fire;
    }
}
