/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：FSMParent
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-1-29
// 模块描述：行为状态管理
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Game;

namespace Mogo.FSM
{
    public abstract class FSMParent
    {
        #region 公共变量

        #endregion

        #region 私有变量

        protected Dictionary<string, IState> theFSM = new Dictionary<string, IState>();

        #endregion

        public FSMParent()
        {
        }


        public virtual void ChangeStatus(EntityParent theOwner, string newState, params Object[] args)
        {
        }
    }
}