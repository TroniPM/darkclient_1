using UnityEngine;
using System;
using System.Collections.Generic;

using Mogo.FSM;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
using Mogo.Task;

namespace Mogo.Game
{
    public partial class EntityParent
    {
        public void SetActionByStateFlagInIdleState()
        {
            if ((m_stateFlag & DIZZY_STATE) != 0)
            {
                SetAction(16);
                AddCallbackInFrames(() => { SetAction(999); });
            }
        }
        public bool CanMove()
        {
            return canMove;
            //return ((stateFlag & DIZZY_STATE) == 0) && ((stateFlag & IMMOBILIZE_STATE) == 0);
        }

        virtual protected void CheckImmunity(ulong value)
        {
            if (IsStateReset(m_stateFlag, value, IMMUNITY_STATE))
            {

            }
            else if ((m_stateFlag & IMMUNITY_STATE) != 0)
            {
                if (motor)
                {
                    motor.enableStick = true;
                    motor.canMove = true;
                }
                canCastSpell = true;

                if (currentMotionState == MotionState.IDLE)
                {
                    if (this is EntityPlayer && MogoWorld.inCity)
                    {
                        SetAction(0);//应为Animator的图没有直线连-1，所以要绕一下
                        TimerHeap.AddTimer<int>(500, 0, SetAction, -1);
                    }
                    else
                    {
                        SetAction(0);
                    }
                }
            }
        }

        virtual protected void CheckCanBeHitOrNot(ulong value)
        {
            if (IsStateReset(m_stateFlag, value, NO_HIT_STATE))
            {
                canBeHit = true;
            }
            else if ((m_stateFlag & NO_HIT_STATE) != 0)
            {
                canBeHit = false;
            }
        }

        virtual protected void CheckImmobilize(ulong value)
        {
            if (IsStateReset(m_stateFlag, value, IMMOBILIZE_STATE))
            {
                if (motor)
                {
                    motor.canMove = true;
                    motor.enableStick = true;
                }
            }
            else if ((m_stateFlag & IMMOBILIZE_STATE) != 0)
            {
                if (motor)
                {
                    motor.canMove = false;
                    motor.enableStick = false;
                    motor.isMovingToTarget = false;
                }
            }
        }

        bool IsStateReset(ulong oldState, ulong newState, ulong stateMask)
        {
            if ((oldState & stateMask) != 0 && (newState & stateMask) == 0)
                return true;
            else
                return false;
        }

        virtual protected void CheckDizzy(ulong value)
        {
            if (IsStateReset(m_stateFlag, value, DIZZY_STATE))
            {
                if (motor)
                {
                    motor.enableStick = true;
                    motor.canMove = true;
                }
                canCastSpell = true;

                if (currentMotionState == MotionState.IDLE)
                {
                    if (this is EntityPlayer && MogoWorld.inCity)
                    {
                        SetAction(0);//应为Animator的图没有直线连-1，所以要绕一下
                        TimerHeap.AddTimer<int>(500, 0, SetAction, -1);
                    }
                    else
                    {
                        SetAction(0);
                    }
                }
            }
            else if ((value & DIZZY_STATE) != 0)
            {
                //不能移动,施法
                if (motor)
                {
                    Mogo.Util.LoggerHelper.Debug("眩晕");
                    motor.enableStick = false;
                    motor.canMove = false;
                    motor.isMovingToTarget = false;
                }
                canCastSpell = false;
                SetAction(16);
                AddCallbackInFrames(() => { SetAction(999); });
            }
        }

        virtual protected void StateChange(ulong value)
        {
            SkillState oldSt = null;
            SkillState newSt = null;
            for (int i = 0; i < 64; i++)
            {//循环64个状态
                int o = Mogo.Util.Utils.BitTest(m_stateFlag, i);
                int n = Mogo.Util.Utils.BitTest(value, i);
                if (o == 1)
                {
                    SkillState _o = null;
                    SkillState.dataMap.TryGetValue(i, out _o);
                    if ((oldSt == null) ||
                        (_o != null && _o.showPriority >= oldSt.showPriority))
                    {
                        oldSt = _o;
                    }
                }
                if (n == 1)
                {
                    SkillState _n = null;
                    SkillState.dataMap.TryGetValue(i, out _n);
                    if ((newSt == null) ||
                        (_n != null && _n.showPriority >= newSt.showPriority))
                    {
                        newSt = _n;
                    }
                }
                if (o == n)
                {
                    continue;
                }
                if (o == 1)
                {//删除状态
                    StateProc(i, false);
                }
                else
                {//新加状态
                    StateProc(i, true);
                }
            }
            if (!immuneShift)
            {
                StateActionPriority(oldSt, newSt);
            }
        }

        private void StateActionPriority(SkillState oldSt, SkillState newSt)
        {
            if (oldSt == null && newSt == null)
            {
                return;
            }
            if (oldSt != null && newSt == null)
            {
                StateAction(oldSt, false);
                return;
            }
            if (oldSt == null && newSt != null)
            {
                StateAction(newSt, true);
                return;
            }
            if ((oldSt.showPriority < newSt.showPriority) || (oldSt.id < newSt.id))
            {
                StateAction(oldSt, false);
                AddCallbackInFrames<SkillState, bool>((s, b) => { StateAction(s, b); }, newSt, true, 5);
            }
        }

        private void StateProc(int stateId, bool add)
        {//以下的处理，如果长了要分开处理
            SkillState s = null;
            if (!SkillState.dataMap.TryGetValue(stateId, out s))
            {
                return;
            }
            if (s.direction >= 1)
            {//填有值时才处理，这样才不会状态冲突
                direction = add;
            }
            if (s.moveAble >= 1)
            {
                canMove = !add;
                if (motor)
                {
                    motor.enableStick = !add;
                    motor.canMove = !add;   
                    //motor.isMovingToTarget = !add;
                    if (add)
                    {
                        motor.StopNav();
                    }

                }
                if (currentMotionState == MotionState.WALKING)
                {
                    SetSpeed(0);
                    motor.SetSpeed(0);
                    motor.speed = 0;
                    currentMotionState = MotionState.IDLE;
                }
            }
            if (stateId == StateCfg.BATI_STATE)
            {
                if (add)
                {
                    immuneShift = true;
                }
                else
                {
                    immuneShift = false;
                }
            }

            if (s.attackAble >= 1)
            {
                canCastSpell = !add;
            }
            if (s.moveSpeedRate > 0)
            {
                if (add)
                {
                    moveSpeedRate = s.moveSpeedRate * gearMoveSpeedRate;
                }
                else
                {
                    moveSpeedRate = 1;
                }
            }
            if (s.hittable >= 1)
            {
                canBeHit = add;
            }

            //if (!immuneShift)
            //    StateAction(s, add);
        }

        private void StateAction(SkillState s, bool add)
        {
            if (s.act > 0)
            {
                if (add)
                {
                    SetAction(s.act);
                    AddCallbackInFrames(() => { SetAction(-2); });
                }
                else
                {
                    SetAction(0);
                }
            }
            if (s.sfx > 0)
            {
                if (add)
                {
                    PlayFx(s.sfx);
                }
                else
                {
                    RemoveFx(s.sfx);
                }
            }
        }
    }
}
