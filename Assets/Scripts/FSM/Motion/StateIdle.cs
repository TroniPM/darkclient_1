/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StateIdle
// 创建者：Steven Yang
// 修改者列表：2013-1-29
// 创建日期：
// 模块描述：Idle状态
//----------------------------------------------------------------*/

using System;
using System.Collections;

using Mogo.Util;
using Mogo.Game;

namespace Mogo.FSM
{
    public class StateIdle : IState
    {
        // 进入该状态
        public void Enter(EntityParent theOwner, params Object[] args)
        {
            theOwner.CurrentMotionState = MotionState.IDLE;
        }

        // 离开状态
        public void Exit(EntityParent theOwner, params Object[] args)
        {
        }

        // 状态处理
        public void Process(EntityParent theOwner, params Object[] args)
        {
            // 播放 idle 动画
            if (theOwner == null)
            {
                return;
            }
            if (theOwner.CanMove() && theOwner.motor != null)
            {
                theOwner.motor.enableStick = true;
            }
            MogoMotor theMotor = theOwner.motor;
            if (theOwner is EntityMonster)
            {
                theOwner.ApplyRootMotion(false);
            }
            // 设置 速度
            if (theMotor != null)
            {
                theMotor.SetSpeed(0.0f);
            }
            //theMotor.SetExrtaSpeed(0f);
            if (theOwner.charging)
            {
                return;
            }
            if (theOwner is EntityPlayer && MogoWorld.inCity)
            {
                theOwner.SetAction(-1);
            }
            else
            {
                theOwner.SetAction(0);
            }
            theOwner.SetActionByStateFlagInIdleState();
        }
    }
}