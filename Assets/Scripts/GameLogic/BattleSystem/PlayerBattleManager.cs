/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：PlayerBattleManager
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
    public class PlayerBattleManager : BattleManager
    {
        // 为去除警告暂时屏蔽以下代码
        //private float lastAttackTime = 0.0f;
        //       private PlayerSkillManager skillManager;
        private List<int> preCmds = new List<int>();
        private uint timeOutId = 0;
        #region cd 时间管理
        // 初始化 技能数据

        #endregion

        public PlayerBattleManager(EntityParent _owner, SkillManager _skillManager)
            : base(_owner, _skillManager)
        {
            skillManager = _skillManager;
            //EventDispatcher.AddEventListener<int, uint, List<uint>, List<int>>(Events.FSMMotionEvent.OnHit, OnHit);
        }

        // 析构函数， 移除监听的各种事件
        public override void Clean()
        {
            base.Clean();
        }

        // 当受击时的出来
        override public void OnHit(int _spellID, uint _attackerID, uint woundId, List<int> harm)
        {
           
            if (MogoWorld.inCity ||
                theOwner.ID != woundId ||
                !theOwner.canBeHit ||
                theOwner.ID == _attackerID ||
                ((EntityMyself)theOwner).deathFlag > 0 ||
                theOwner.charging)
            {
                return;
            }
            int hitType = harm[0];
            int hitNum = harm[1];
            if (MogoWorld.showFloatBlood && SkillAction.dataMap[_spellID].damageFlag == 1)
            {
                FloatBlood(hitType, hitNum);
            }
            if (!theOwner.breakAble)
            {
                return;
            }
            if (theOwner.curHp > 0/* && theOwner.curHp > harm[1]*/)
            {
                base.OnHit(_spellID, _attackerID, woundId, harm);
            }
            
        }

        override protected void FloatBlood(int type, int num)
        {
            switch (type)
            {
                case 1:
                    {//闪避
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.Miss);
                        break;
                    }
                case 2:
                    {//角色普能受击
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.NormalPlayer);
                        break;
                    }
                case 3:
                    {//破击
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.BrokenAttack);
                        break;
                    }
                case 4:
                    {//暴击
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.CriticalPlayer);
                        break;
                    }
                case 5:
                    {//破击加暴击
                        theOwner.Actor.FloatBlood(num, SplitBattleBillboardType.BrokenAttack);
                        break;
                    }
            }
        }

        override public void OnHitAnimEnd(int nID)
        {
            base.OnHitAnimEnd(nID);
        }

        override public void OnRollEnd()
        {
            theOwner.motor.enableStick = true;
            if (theOwner.CurrentMotionState == MotionState.ROLL)
            {
                theOwner.ChangeMotionState(MotionState.IDLE);
            }
        }

        override public void Move()
        {
            CleanPreSkill();
            if (theOwner.CurrentMotionState == MotionState.LOCKING
                || theOwner.CurrentMotionState == MotionState.ATTACKING
                || theOwner.CurrentMotionState == MotionState.DEAD
                || theOwner.CurrentMotionState == MotionState.PICKING
                || theOwner.CurrentMotionState == MotionState.CHARGING
                || theOwner.CurrentMotionState == MotionState.HIT
                || theOwner.CurrentMotionState == MotionState.ROLL)
            {
                return;
            }
            theOwner.ChangeMotionState(MotionState.WALKING);
        }

        override public void Idle()
        {
            if (theOwner.CurrentMotionState == MotionState.LOCKING
                || theOwner.CurrentMotionState == MotionState.ATTACKING
                || theOwner.CurrentMotionState == MotionState.DEAD
                || theOwner.CurrentMotionState == MotionState.PICKING
                || theOwner.CurrentMotionState == MotionState.PREPARING
                || theOwner.CurrentMotionState == MotionState.CHARGING
                || theOwner.CurrentMotionState == MotionState.ROLL)
            {
                return;
            }

            theOwner.ChangeMotionState(MotionState.IDLE);
        }

        override public void CastSkill(int nSpellID)
        {
            if (theOwner.CurrentMotionState == MotionState.LOCKING
                || theOwner.CurrentMotionState == MotionState.DEAD
                || theOwner.CurrentMotionState == MotionState.PICKING
                || theOwner.CurrentMotionState == MotionState.PREPARING
                || theOwner.CurrentMotionState == MotionState.ROLL)
            {
                return;
            }
            theOwner.ChangeMotionState(MotionState.ATTACKING, nSpellID);
        }

        // 普攻，先进入Entity的GoingToCastSpell
        public void NormalAttack()
        {
            if (MogoWorld.inCity ||
                (theOwner as EntityMyself).deathFlag == 1 ||
                theOwner.stiff ||
                theOwner.charging)
            {
                return;
            }
            if ((skillManager as PlayerSkillManager).IsCommonCooldown())
            {
                preCmds.Add(0);
                return;
            }
            int nextskill = (skillManager as PlayerSkillManager).GetNormalAttackID();
            if (nextskill == theOwner.currSpellID && theOwner.currSpellID != -1)
            {
                preCmds.Add(0);
                return;
            }
            if ((skillManager as PlayerSkillManager).IsSkillCooldown(nextskill))
            {
                (skillManager as PlayerSkillManager).ClearComboSkill();
                preCmds.Add(0);
                return;
            }
            if (!(skillManager as PlayerSkillManager).HasDependence(nextskill))
            {
                CleanPreSkill();
            }
            //if (theOwner.IsInTransition())
            //{
            //    if (timeOutId != 0)
            //    {
            //        TimerHeap.DelTimer(timeOutId);
            //    }
            //    timeOutId = TimerHeap.AddTimer(100, 0, NormalAttack);
            //    return;
            //}
            //TimerHeap.DelTimer(timeOutId);
            //timeOutId = 0;
            /*if (theOwner.currSpellID != -1)
            {
                SkillData s = SkillData.dataMap.Get(theOwner.currSpellID);
                SkillData ns = SkillData.dataMap.Get(nextskill);
                if (s.posi != 0 && ns.posi != 4)
                {//蓄力接普攻，蓄力没完不放普攻
                    (skillManager as PlayerSkillManager).ClearComboSkill();
                    return;
                }
            }*/
            theOwner.motor.TurnToControlStickDir();
            Util.MogoUtils.LookAtTargetInRange(theOwner.Transform, 6, 360);
            (skillManager as PlayerSkillManager).ResetCoolTime(nextskill);
            EntityMyself.preSkillTime = Time.realtimeSinceStartup;
            theOwner.CastSkill(nextskill);
            TimerHeap.AddTimer((uint)((skillManager as PlayerSkillManager).GetCommonCD(nextskill)), 0, NextCmd);
        }

        override public void OnAttacking(int nSkillID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
        {
            skillManager.OnAttacking(nSkillID, ltwm, rotation, forward, position);
        }

        // 蓄力完成。触发一次蓄力攻击
        public void PowerChargeComplete()
        {//没有蓄力
            return;
            //为消除警告而注释以下代码
            //if ((theOwner as EntityMyself).deathFlag == 1 ||
            //    !theOwner.charging)
            //{
            //    return;
            //}
            //theOwner.charging = false;
            //int fxID = RoleData.dataMap[(int)theOwner.vocation].powerSfx[(theOwner as EntityMyself).GetEquipSubType()];
            //theOwner.RemoveFx(fxID);
            //int skillid = (skillManager as PlayerSkillManager).GetPowerAttackID();
            //(skillManager as PlayerSkillManager).ResetCoolTime(skillid);
            //EntityMyself.preSkillTime = Time.realtimeSinceStartup;
            //theOwner.CastSkill(skillid);
        }

        // 蓄力状态
        public void PowerChargeStart()
        {//没有蓄力
            return;
            //为消除警告而注释以下代码
            //if ((theOwner as EntityMyself).deathFlag == 1)
            //{
            //    return;
            //}
            //if (!(theOwner as EntityMyself).ChargetAble())
            //{
            //    return;
            //}
            //if (theOwner.CurrentMotionState == MotionState.HIT
            //    || theOwner.CurrentMotionState == MotionState.ROLL)
            //{
            //    return;
            //}
            //if (theOwner.currSpellID != -1)
            //{
            //    SkillData s = SkillData.dataMap.Get(theOwner.currSpellID);
            //    if (s != null && s.posi != 0)
            //    {//非普攻或空闲状态不能蓄力
            //        return;
            //    }
            //}
            //theOwner.ChangeMotionState(MotionState.IDLE);
            //theOwner.charging = true;
            //int fxID = RoleData.dataMap.Get((int)theOwner.vocation).powerSfx.Get((theOwner as EntityMyself).GetEquipSubType());
            //theOwner.PlayFx(fxID);
            //theOwner.RpcCall("ChargeSkillReq", (System.Int64)(Time.time * 1000));
            //theOwner.ChangeMotionState(MotionState.CHARGING);
        }

        // 蓄力中断，进入idle
        public void PowerChargeInterrupt()
        {//没有蓄力
            return;
            //为消除警告而注释以下代码
            //if ((theOwner as EntityMyself).deathFlag == 1)
            //{
            //    return;
            //}
            //theOwner.charging = false;
            //int fxID = RoleData.dataMap[(int)theOwner.vocation].powerSfx[(theOwner as EntityMyself).GetEquipSubType()];
            //theOwner.RemoveFx(fxID);
            //theOwner.RpcCall("CancelChargeSkillReq");
            ////theOwner.ChangeMotionState(MotionState.IDLE);
            //theOwner.SetAction(0);
            ////theOwner.currSpellID = -1;
        }

        public void SpellOneAttack()
        {
            CleanPreSkill();
            if (MogoWorld.inCity ||
                (theOwner as EntityMyself).deathFlag == 1 ||
                //theOwner.currSpellID != -1 ||
                theOwner.stiff ||
                theOwner.charging)
            {
                return;
            }
            if ((skillManager as PlayerSkillManager).IsCommonCooldown())
            {
                return;
            }
            int skillid = (skillManager as PlayerSkillManager).GetSpellOneID();
            if ((skillManager as PlayerSkillManager).IsSkillCooldown(skillid))
            {
                return;
            }
            //if (theOwner.IsInTransition())
            //{
            //    if (timeOutId != 0)
            //    {
            //        TimerHeap.DelTimer(timeOutId);
            //    }
            //    timeOutId = TimerHeap.AddTimer(100, 0, SpellOneAttack);
            //    return;
            //}
            //TimerHeap.DelTimer(timeOutId);
            //timeOutId = 0;
            (theOwner as EntityMyself).ClearSkill();
            (skillManager as PlayerSkillManager).ClearComboSkill();
            (skillManager as PlayerSkillManager).ResetCoolTime(skillid);
            theOwner.motor.TurnToControlStickDir();
            EntityMyself.preSkillTime = Time.realtimeSinceStartup;
            theOwner.CastSkill(skillid);
            SkillData s = SkillData.dataMap[skillid];
            MainUIViewManager.Instance.SpellOneCD(s.cd[0]);
        }

        public void SpellTwoAttack()
        {
            CleanPreSkill();
            if (MogoWorld.inCity ||
                (theOwner as EntityMyself).deathFlag == 1 ||
                //theOwner.currSpellID != -1 ||
                theOwner.stiff ||
                theOwner.charging)
            {
                return;
            }
            if ((skillManager as PlayerSkillManager).IsCommonCooldown())
            {
                return;
            }
            int skillid = (skillManager as PlayerSkillManager).GetSpellTwoID();
            if ((skillManager as PlayerSkillManager).IsSkillCooldown(skillid))
            {
                return;
            }
            //if (theOwner.IsInTransition())
            //{
            //    if (timeOutId != 0)
            //    {
            //        TimerHeap.DelTimer(timeOutId);
            //    }
            //    timeOutId = TimerHeap.AddTimer(100, 0, SpellTwoAttack);
            //    return;
            //}
            //TimerHeap.DelTimer(timeOutId);
            //timeOutId = 0;
            (theOwner as EntityMyself).ClearSkill();
            (skillManager as PlayerSkillManager).ClearComboSkill();
            (skillManager as PlayerSkillManager).ResetCoolTime(skillid);
            theOwner.motor.TurnToControlStickDir();
            EntityMyself.preSkillTime = Time.realtimeSinceStartup;
            theOwner.CastSkill(skillid);
            SkillData s = SkillData.dataMap[skillid];
            MainUIViewManager.Instance.SpellTwoCD(s.cd[0]);
        }

        public void SpellThreeAttack()
        {
            CleanPreSkill();
            if (MogoWorld.inCity ||
                (theOwner as EntityMyself).deathFlag == 1 ||
                //theOwner.currSpellID != -1 ||
                theOwner.stiff ||
                theOwner.charging)
            {
                return;
            }
            if ((skillManager as PlayerSkillManager).IsCommonCooldown())
            {
                return;
            }
            int skillid = (skillManager as PlayerSkillManager).GetSpellThreeID();
            if ((skillManager as PlayerSkillManager).IsSkillCooldown(skillid))
            {
                return;
            } 
            //if (theOwner.IsInTransition())
            //{
            //    if (timeOutId != 0)
            //    {
            //        TimerHeap.DelTimer(timeOutId);
            //    }
            //    timeOutId = TimerHeap.AddTimer(100, 0, SpellThreeAttack);
            //    return;
            //}
            //TimerHeap.DelTimer(timeOutId);
            //timeOutId = 0;
            (theOwner as EntityMyself).ClearSkill();
            (skillManager as PlayerSkillManager).ClearComboSkill();
            (skillManager as PlayerSkillManager).ResetCoolTime(skillid);
            theOwner.motor.TurnToControlStickDir();
            EntityMyself.preSkillTime = Time.realtimeSinceStartup;
            theOwner.CastSkill(skillid);
            SkillData s = SkillData.dataMap[skillid];
            MainUIViewManager.Instance.SpellThreeCD(s.cd[0]);
        }

        public void NextCmd()
        {
            if (preCmds.Count == 0)
            {
                return;
            }
            preCmds.RemoveAt(0);
            NormalAttack();
        }

        public void CleanPreSkill()
        {
            preCmds.Clear();
        }
    }
}