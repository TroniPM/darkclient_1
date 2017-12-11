/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StatePreparing
// 创建者：Steven Yang
// 修改者列表：2013-1-29
// 创建日期：
// 模块描述：攻击之前的准备状态状态
//----------------------------------------------------------------*/

using System;
using System.Collections;

using Mogo.Util;
using Mogo.Game;

namespace Mogo.FSM
{
    public class StatePreparing : IState
    {
        // 进入该状态
        public void Enter(EntityParent theOwner, params Object[] args)
        {
   //         LoggerHelper.Debug("enter prepare state");
            theOwner.CurrentMotionState = MotionState.PREPARING;
        }

        // 离开状态
        public void Exit(EntityParent theOwner, params Object[] args)
        {
        }

        // 状态处理
        public void Process(EntityParent theOwner, params Object[] args)
        {
   //         LoggerHelper.Debug("process prepare state");
            if (args.Length != 1)
            {
                LoggerHelper.Error("error spell id");
                return;
            }
            int skillID = (int)(args[0]);

            // 技能释放准备相关动作
            // 比如 旋转角色， 朝向， 自动靠近目标等
            // 目前先跳过

            if (skillID > 2)
            {
                //         ActorParent actorParent = theOwner.transform.gameObject.GetComponent<ActorParent>();
                //          actorParent.SwitchToIdle();
            }

            // 回调函数， 切换到 idle
            theOwner.AddCallbackInFrames<EntityParent, int>(
                (_theOwner, _skillID) =>
                {
                    _theOwner.TriggerUniqEvent(Events.FSMMotionEvent.OnPrepareEnd, _skillID);
                },
                theOwner, skillID
            ); 
         }
    }
}