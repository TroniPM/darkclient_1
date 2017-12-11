/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StateRoll
// 创建者：Steven Yang
// 修改者列表：2013-5-31
// 创建日期：
// 模块描述：翻滚状态
//----------------------------------------------------------------*/

using System;
using System.Collections;

using Mogo.Util;
using Mogo.Game;

namespace Mogo.FSM
{
    public class StateRoll : IState
    {
        // 进入该状态
        public void Enter(EntityParent theOwner, params Object[] args)
        {
            theOwner.CurrentMotionState = MotionState.ROLL;
        }

        // 离开状态
        public void Exit(EntityParent theOwner, params Object[] args)
        {
        }

        // 状态处理
        public void Process(EntityParent theOwner, params Object[] args)
        {
            LoggerHelper.Debug("process roll state");

            // 播放动画
            if (theOwner.animator != null)
            {
                theOwner.animator.SetInteger("Action", 30);
                theOwner.AddCallbackInFrames((t) =>
                {
                    t.animator.SetInteger("Action", -30);
                }, theOwner);
            }
        }
    }
}