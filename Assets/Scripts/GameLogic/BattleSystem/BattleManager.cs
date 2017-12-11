/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：BattleManager
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-2-28
// 模块描述：战斗管理系统
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.FSM;
using Mogo.GameData;


namespace Mogo.Game
{
    static class BattleAttr
    {
        static public string ATTACK = "atk"; // 伤害
        static public string DEFENSE = "def"; //防御
        static public string FIGHT_FORCE = "fightForce"; //防御
        static public string HIT = "hit"; // 命中
        static public string HEALTH = "curHp"; // 生命
        static public string CRIT = "crit"; //暴击
        static public string ANTI_CRIT = "antiCrit"; // 抗暴击
        static public string TRUE_STRIKE = "trueStrike"; // 破击
        static public string ANTI_TRUE_STRIKE = "antiTrueStrike"; // 抗破击
        static public string ANTI_DEFENSE = "antiDefense"; // 穿刺
        static public string CRIT_EXTRA_ATTACK = "critExtraAttack"; // 爆伤加成
        static public string PVP_ADDITION = "pvpAddition"; // PVP强度
        static public string PVP_ANTI = "pvpAnti"; // PVE强度
        static public string CD_REDUCE = "cdReduce"; // CD减少
        static public string SPEED_ADD_RATE = "speedAddRate"; // 速度增加

        static public string EARTH_DAMAGE = "earthDamage"; // 土元素伤害
        static public string AIR_DAMAGE = "airDamage"; // 风元素伤害
        static public string WATER_DAMAGE = "waterDamage"; // 水元素伤害
        static public string FIRE_DAMAGE = "fireDamage"; // 火元素伤害
        static public string ALL_ELEMENTS_DAMAGE = "allElementsDamage"; // 全元素伤害

        static public string EARTH_DEFENSE = "earthDefense"; // 土元素抗性
        static public string AIR_DEFENSE = "airDefense"; // 风元素抗性
        static public string WATER_DEFENSE = "waterDefense"; // 水元素抗性
        static public string FIRE_DEFENSE = "fireDefense"; // 火元素抗性
        static public string ALL_ELEMENTS_DEFENSE = "allElementsDefense"; // 全元素抗性
    }

    public class BattleManager
    {
        protected EntityParent theOwner;
        protected SkillManager skillManager;

        private int currentSpellID = 0;    // 当前正在使用的技能配置数据(未使用技能时为0)
        private int currentHitSpellID = 0; // 受击技能

        public int CurrentSpellID
        {
            get { return currentSpellID; }
            set { currentSpellID = value; }
        }

        public int CurrentHitSpellID
        {
            get { return currentHitSpellID; }
            set { currentHitSpellID = value; }
        }

        public BattleManager(EntityParent _owner, SkillManager _skillManager)
        {
            theOwner = _owner;
            skillManager = _skillManager;
            theOwner.AddUniqEventListener<int>(Events.FSMMotionEvent.OnPrepareEnd, OnPrepareEnd);
            theOwner.AddUniqEventListener<int>(Events.FSMMotionEvent.OnAttackingEnd, OnAttackingEnd);
            theOwner.AddUniqEventListener<int>(Events.FSMMotionEvent.OnHitAnimEnd, OnHitAnimEnd);
            theOwner.AddUniqEventListener(Events.FSMMotionEvent.OnRollEnd, OnRollEnd);

            EventDispatcher.AddEventListener<int, uint, uint, List<int>>(Events.FSMMotionEvent.OnHit, OnHit);
        }

        // 析构函数， 移除监听的各种事件
        virtual public void Clean()
        {
            theOwner.RemoveUniqEventListener<int>(Events.FSMMotionEvent.OnPrepareEnd, OnPrepareEnd);
            theOwner.RemoveUniqEventListener<int>(Events.FSMMotionEvent.OnAttackingEnd, OnAttackingEnd);
            theOwner.RemoveUniqEventListener<int>(Events.FSMMotionEvent.OnHitAnimEnd, OnHitAnimEnd);
            theOwner.RemoveUniqEventListener(Events.FSMMotionEvent.OnRollEnd, OnRollEnd);

            EventDispatcher.RemoveEventListener<int, uint, uint, List<int>>(Events.FSMMotionEvent.OnHit, OnHit);
        }

        virtual public void OnHitnew(int _actionID, uint _attackerID, uint woundId, List<int> harm)
        {//新版受击表现处理，目前未接入系统
            if (theOwner.CurrentMotionState == MotionState.DEAD)
                return;
            if (!HitState.dataMap.ContainsKey(theOwner.hitStateID))
            {
                //OnHitold(_actionID, _attackerID, woundId, harm);
                return;
            }
            SkillAction action = SkillAction.dataMap[_actionID];
            if (action.hitSfx != null && action.hitSfx[0] > 0 && MogoWorld.showSkillFx)
            {
                theOwner.PlayFx((int)action.hitSfx[0]);
            }
            List<int> hitAction = action.hitAction;
            if (hitAction == null || hitAction.Count == 0)
            {
                return;
            }
            int _act = Utils.Choice<int>(hitAction);
            bool cfgShow = true;
            if (((theOwner is EntityMonster) && (theOwner as EntityMonster).ShowHitAct != 0) ||
                ((theOwner is EntityDummy) && (theOwner as EntityDummy).ShowHitAct != 0) ||
                ((theOwner is EntityMercenary) && (theOwner as EntityMercenary).ShowHitAct != 0))
            {
                cfgShow = false;
            }
            //string actName = theOwner.CurrActStateName();
            if (_act > 0 && theOwner.curHp > 0 && cfgShow)
            {//如果没填就不做受击表现
                theOwner.ChangeMotionState(MotionState.HIT, _actionID, _attackerID);
            }
            if (!(theOwner is EntityMonster) && action.hitExtraSpeed != 0)
            {
                theOwner.motor.SetExrtaSpeed(-action.hitExtraSpeed);
                theOwner.motor.SetMoveDirection(theOwner.Transform.forward);
                TimerHeap.AddTimer<EntityParent>((uint)(action.hitExtraSl * 1000), 0, (e) => { e.motor.SetExrtaSpeed(0); }, theOwner);
            }
        }
        // 为去除警告暂时屏蔽以下代码
        //private uint stiffTotal = 0;
        virtual public void OnHit(int _actionID, uint _attackerID, uint woundId, List<int> harm)
        {
            if (theOwner.CurrentMotionState == MotionState.DEAD)
                return;
            if (theOwner.currSpellID != -1)
            {//专为旋风斩之类有duration的技能临时做
                SkillData s = SkillData.dataMap[theOwner.currSpellID];
                if (SkillAction.dataMap[s.skillAction[0]].duration > 0)
                {
                    return;
                }
            }
            if (!theOwner.immuneShift)
            {
                theOwner.ClearSkill(true);
            }
            SkillAction action = SkillAction.dataMap[_actionID];
            if (action.hitSfx != null && action.hitSfx[0] > 0)
            {
                theOwner.PlayFx((int)action.hitSfx[0]);
            }
            HitBuff(action);
            List<int> hitAction = action.hitAction;
            if (hitAction == null || hitAction.Count == 0 || theOwner.immuneShift)
            {
                return;
            }
            int _act = Utils.Choice<int>(hitAction);
            bool cfgShow = true;
            if (((theOwner is EntityMonster) && (theOwner as EntityMonster).ShowHitAct != 0) ||
                ((theOwner is EntityDummy) && (theOwner as EntityDummy).ShowHitAct != 0) ||
                ((theOwner is EntityMercenary) && (theOwner as EntityMercenary).ShowHitAct != 0))
            {
                cfgShow = false;
            }
            string actName = theOwner.CurrActStateName();

            if ((theOwner is EntityMyself) && (MapData.dataMap.Get(MogoWorld.thePlayer.sceneId).type == MapType.ARENA))
            {//如果主角在竞技场内受到HIT，无视之，不进受击状态
                List<int> hitAction2 = SkillAction.dataMap[_actionID].hitAction;
                if (hitAction2 == null || hitAction.Count == 0)
                {
                    return;
                }
                int action2 = Utils.Choice<int>(hitAction2);
                if (action2 == ActionConstants.HIT)
                    return;
            }


            if (MogoWorld.showHitAction && _act > 0 && theOwner.curHp > 0 &&
                !actName.EndsWith("getup") &&
                !actName.EndsWith("knockout") &&
                !actName.EndsWith(PlayerActionNames.names[ActionConstants.HIT_GROUND]) &&
                //!theOwner.IsInTransition() &&
                cfgShow)
            {//如果没填就不做受击表现
                theOwner.ChangeMotionState(MotionState.HIT, _actionID, _attackerID);
            }
            if (MogoWorld.showHitEM && !(theOwner is EntityMonster) && action.hitExtraSpeed != 0)
            {
                theOwner.motor.SetExrtaSpeed(-action.hitExtraSpeed);
                theOwner.motor.SetMoveDirection(theOwner.Transform.forward);
                TimerHeap.AddTimer<EntityParent>((uint)(action.hitExtraSl * 1000), 0, (e) => {
                    if (e == null || e.motor == null)
                    {
                        return;
                    }
                    e.motor.SetExrtaSpeed(0); 
                }, theOwner);
            }
        }

        private void HitBuff(SkillAction action)
        {
            if (action.targetAddBuff != null)
            {
                foreach (int id in action.targetAddBuff)
                {
                    theOwner.ClientAddBuff(id);
                }
            }
            if (action.targetDelBuff != null)
            {
                foreach (int id in action.targetDelBuff)
                {
                    theOwner.ClientDelBuff(id);
                }
            }
        }

        virtual protected void FloatBlood(int type, int num)
        {
            switch (type)
            {
                case 1:
                    {//闪避
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.Miss);
                        break;
                    }
                case 2:
                    {//怪物普通受击
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.NormalMonster);
                        break;
                    }
                case 3:
                    {//破击
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.BrokenAttack);
                        break;
                    }
                case 4:
                    {//暴击
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.CriticalMonster);
                        break;
                    }
                case 5:
                    {//破击加暴击
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.BrokenAttack);
                        break;
                    }
            }
        }

        virtual public void OnHitAnimEnd(int nID)
        {
            if (theOwner.CurrentMotionState == MotionState.HIT)
            {
                theOwner.SetAction(0);
            }
        }

        // 播放式： 攻击动作。
        virtual public void OnAttacking(int nSkillID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
        {
            skillManager.OnAttacking(nSkillID, ltwm, rotation, forward, position);
        }
        
        virtual public void OnDead(int hitActionID)
        {
            theOwner.ChangeMotionState(MotionState.DEAD, hitActionID);
        }

        // 技能准备完毕。
        // 根据技能的不同， 进入不同的状态
        virtual public void OnPrepareEnd(int _nSpellID)
        {
            theOwner.ChangeMotionState(MotionState.ATTACKING, _nSpellID);
        }

        // 攻击结束， 进入idle 状态
        virtual public void OnAttackingEnd(int _actionID)
        {
            if (theOwner.charging)
            {
                return;
            }
            theOwner.SetAction(0);
        }

        virtual public void OnRollEnd()
        {
            theOwner.motor.enableStick = true;
            theOwner.ChangeMotionState(MotionState.IDLE);
        }

        // Walk状态
        virtual public void Move()
        {
            if (theOwner.CurrentMotionState == MotionState.LOCKING
                || theOwner.CurrentMotionState == MotionState.ATTACKING
                || theOwner.CurrentMotionState == MotionState.DEAD
                || theOwner.CurrentMotionState == MotionState.HIT
                || theOwner.CurrentMotionState == MotionState.PICKING
                || theOwner.CurrentMotionState == MotionState.ROLL)
            {
                return;
            }
            theOwner.ChangeMotionState(MotionState.WALKING);
        }

        // Idle状态
        virtual public void Idle()
        {
            if (theOwner.CurrentMotionState == MotionState.LOCKING
                || theOwner.CurrentMotionState == MotionState.ATTACKING
                || theOwner.CurrentMotionState == MotionState.DEAD
                || theOwner.CurrentMotionState == MotionState.PICKING
                || theOwner.CurrentMotionState == MotionState.PREPARING
                || theOwner.CurrentMotionState == MotionState.ROLL)
            {
                return;
            }
            theOwner.ChangeMotionState(MotionState.IDLE);
        }

        // 主动释放技能。直接进入PREPARING放技能
        virtual public void CastSkill(int actionID)
        {
            if (theOwner.CurrentMotionState == MotionState.LOCKING
                || theOwner.CurrentMotionState == MotionState.DEAD
                || theOwner.CurrentMotionState == MotionState.PICKING
                || theOwner.CurrentMotionState == MotionState.PREPARING
                || theOwner.CurrentMotionState == MotionState.ROLL)
            {
                LoggerHelper.Debug("the entity is busy" + theOwner.CurrentMotionState);
                return;
            }
            //theOwner.ChangeMotionState(MotionState.PREPARING, nkillID);
            theOwner.ChangeMotionState(MotionState.ATTACKING, actionID);
        }

        // 翻滚
        virtual public void Roll()
        {
            theOwner.ChangeMotionState(MotionState.ROLL);
        }


    }
}