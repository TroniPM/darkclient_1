/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StateLocking
// 创建者：Steven Yang
// 修改者列表：2013-1-29
// 创建日期：
// 模块描述：锁定怪物状态
//----------------------------------------------------------------*/

using System;
using System.Collections;
using Mogo.Game;

namespace Mogo.FSM
{
    public class StateLocking : IState
    {
        // 进入该状态
        public void Enter(EntityParent theOwner, params Object[] args)
        {
            theOwner.CurrentMotionState = MotionState.LOCKING;
        }

        // 离开状态
        public void Exit(EntityParent theOwner, params Object[] args)
        {
        }

        // 状态处理
        public void Process(EntityParent theOwner, params Object[] args)
        {
            theOwner.Transform.GetComponent<UnityEngine.Animation>().CrossFade("locking");
            MogoMotor theMotor = theOwner.motor;
            theMotor.enabled = false;
        }

    }
}