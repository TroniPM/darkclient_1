/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StateWalking
// 创建者：Steven Yang
// 修改者列表：2013-1-29
// 创建日期：
// 模块描述：Walking状态
//----------------------------------------------------------------*/

using System;
using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;

namespace Mogo.FSM
{
    public class StateWalking : IState
    {
        // 进入该状态
        public void Enter(EntityParent theOwner, params System.Object[] args)
        {
            theOwner.CurrentMotionState = MotionState.WALKING;
            if (theOwner is EntityMyself)
            {
                theOwner.animator.speed = theOwner.moveSpeedRate * theOwner.gearMoveSpeedRate;
            }
        }

        // 离开状态
        public void Exit(EntityParent theOwner, params System.Object[] args)
        {
            theOwner.ApplyRootMotion(true);
            theOwner.SetSpeed(1);
            if (theOwner is EntityMonster)
            {
                theOwner.motor.SetExrtaSpeed(0);
                theOwner.motor.isMovingToTarget = false;
                return;
            }
            if (theOwner is EntityDummy)
            {
                theOwner.ApplyRootMotion(false);
            }
            if (theOwner is EntityMyself)
            {
                theOwner.animator.speed = 1;
            }
        }

        // 状态处理
        public void Process(EntityParent theOwner, params System.Object[] args)
        {
            MogoMotor theMotor = theOwner.motor;

            if (theOwner is EntityMonster || (theOwner is EntityPlayer && !(theOwner is EntityMyself)))
            {
                theOwner.ApplyRootMotion(false);
                theOwner.SetSpeed(1);
                theMotor.SetSpeed(0.4f);


                //服务器没有同步其他玩家的速度，这里暂时硬编码处理，
                //待确定其他玩家与怪物移动位移的控制方案再修改(程序控制还是动作位移)
                if (theOwner.speed == 0)
                {
                    theMotor.SetExrtaSpeed(6);
                }
                else
                {
                    theMotor.SetExrtaSpeed(theOwner.speed);
                }
                return;
            }
            else if (theOwner is EntityDummy || theOwner is EntityMercenary)
            {
                theOwner.ApplyRootMotion(false);
                //theOwner.SetSpeed(1);
                theMotor.SetSpeed(0.4f * theOwner.aiRate);
                theMotor.SetExrtaSpeed(theOwner.GetIntAttr("speed") * 0.01f * theOwner.blackBoard.speedFactor * theOwner.aiRate);
            }
            else
            {
                theOwner.ApplyRootMotion(true);
                // theOwner.SetSpeed(1);
                theMotor.SetSpeed(0.4f);
            }
            theMotor.isMovable = true;
        }

    }
}