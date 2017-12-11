/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StatePicking
// 创建者：Steven Yang
// 修改者列表：2013-1-29
// 创建日期：
// 模块描述：拾取状态
//----------------------------------------------------------------*/

using System;
using System.Collections;
using Mogo.Game;

namespace Mogo.FSM
{
    public class StatePicking : IState
    {
        // 进入该状态
        public void Enter(EntityParent theOwner, params Object[] args)
        {
            theOwner.CurrentMotionState = MotionState.PICKING;
        }

        // 离开状态
        public void Exit(EntityParent theOwner, params Object[] args)
        {
        }

        // 状态处理
        public void Process(EntityParent theOwner, params Object[] args)
        {
            theOwner.Transform.GetComponent<UnityEngine.Animation>().CrossFade("picking");
            MogoMotor theMotor = theOwner.motor;
            theMotor.isMovable = false;
        }

    }
}