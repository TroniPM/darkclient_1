/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：IState
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-1-29
// 模块描述：状态接口
//----------------------------------------------------------------*/

using System;
using Mogo.Game;

namespace Mogo.FSM
{
    public interface IState
    {
        // 进入该状态
        void Enter(EntityParent entity, params Object[] args);

        // 离开状态
        void Exit(EntityParent owner, params Object[] args);

        // 状态处理
        void Process(EntityParent owner, params Object[] args);
    }
}