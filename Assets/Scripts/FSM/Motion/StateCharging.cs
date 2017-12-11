/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：RandomHelper
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130227
// 最后修改日期：20130228
// 模块描述：各种随机数
// 代码版本：测试版V1.1
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

namespace Mogo.FSM
{
    public class StateCharging : IState
    {
        // 进入该状态
        public void Enter(EntityParent theOwner, params System.Object[] args)
        {
            theOwner.CurrentMotionState = MotionState.CHARGING;
        }

        // 离开状态
        public void Exit(EntityParent theOwner, params System.Object[] args)
        {
        }

        // 状态处理
        public void Process(EntityParent theOwner, params System.Object[] args)
        {
            theOwner.SetAction(4);
        }
    }
}