/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：FSMMotion
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-1-29
// 模块描述：行为状态管理
//----------------------------------------------------------------*/

//using UnityEngine;
using System;
using System.Collections;

using Mogo.Util;
using Mogo.Game;

namespace Mogo.FSM
{
    static public class  MotionStateSet 
    {
        static public StateIdle stateIdle = new StateIdle();
        static public StateWalking stateWalking = new StateWalking();
        static public StateDead stateDead = new StateDead();
        static public StatePicking statePicking = new StatePicking();

        static public StateAttacking stateAttackint = new StateAttacking();
        static public StateHit stateHit = new StateHit();
        static public StatePreparing statePreparing = new StatePreparing();
        static public StateLocking stateLocking = new StateLocking();

        static public StateCharging stateCharging = new StateCharging();
        static public StateRoll stateRoll = new StateRoll();
    }

    public class FSMMotion : FSMParent
    {
        public FSMMotion()
        {
            theFSM.Add(MotionState.IDLE, MotionStateSet.stateIdle);
            theFSM.Add(MotionState.WALKING, MotionStateSet.stateWalking);
            theFSM.Add(MotionState.DEAD, MotionStateSet.stateDead);
            theFSM.Add(MotionState.PICKING, MotionStateSet.statePicking);

            theFSM.Add(MotionState.ATTACKING, MotionStateSet.stateAttackint);
            theFSM.Add(MotionState.HIT, MotionStateSet.stateHit);
            theFSM.Add(MotionState.PREPARING, MotionStateSet.statePreparing);
            theFSM.Add(MotionState.LOCKING, MotionStateSet.stateLocking);

            theFSM.Add(MotionState.CHARGING, MotionStateSet.stateCharging);
            theFSM.Add(MotionState.ROLL, MotionStateSet.stateRoll);
        }

        //
        public override void ChangeStatus(EntityParent theOwner, string newState, params Object[] args)
        {
            if (theOwner.CurrentMotionState == newState && newState != MotionState.ATTACKING)
            {
                return;
            }

            if (theFSM.ContainsKey(newState) == false)
            {
                LoggerHelper.Error("error state in Motion FSM.");
                return;
            }

            theFSM[theOwner.CurrentMotionState].Exit(theOwner, args);
            theFSM[newState].Enter(theOwner, args);
            theFSM[newState].Process(theOwner, args);
        }
    }
}