/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StateDead
// 创建者：Steven Yang
// 修改者列表：2013-1-29
// 创建日期：
// 模块描述：Dead状态
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.GameData;

using Mogo.Util;
namespace Mogo.FSM
{
    public class StateDead : IState
    {
        // 进入该状态
        public void Enter(EntityParent theOwner, params Object[] args)
        {
            theOwner.CurrentMotionState = MotionState.DEAD;
        }

        // 离开状态
        public void Exit(EntityParent theOwner, params Object[] args)
        {
        }

        // 状态处理
        public void Process(EntityParent theOwner, params Object[] args)
        {
            int action = ActionConstants.DIE;
            theOwner.ApplyRootMotion(true);
            string actName = theOwner.CurrActStateName();
            if (actName.EndsWith(PlayerActionNames.names[ActionConstants.HIT_AIR]) || actName.EndsWith("getup"))
            {
                action = ActionConstants.DIE_KNOCK_DOWN;
                theOwner.SetAction(action);
            }
            else if (actName.EndsWith(PlayerActionNames.names[ActionConstants.HIT_GROUND]) || actName.EndsWith("knockout"))
            {
                action = ActionConstants.DIE;
                theOwner.SetAction(action);
            }
            else
            {
                int hitActionID = (int)(args[0]);
                List<int> deadAction = null;
                if (SkillAction.dataMap.ContainsKey(hitActionID))
                {
                    deadAction = SkillAction.dataMap[hitActionID].deadAction;
                }
                action = ActionConstants.DIE;
                if (deadAction == null || deadAction.Count == 0)
                {
                    action = ActionConstants.DIE;
                }
                else
                {
                    action = Utils.Choice<int>(deadAction);
                }
                theOwner.SetAction(action);
            }

            theOwner.SetSpeed(0);
            EventDispatcher.TriggerEvent(Events.LogicSoundEvent.OnHitYelling, theOwner as EntityParent, action);

            if (theOwner is EntityMyself && theOwner.motor)
            {
                theOwner.motor.enableStick = false;
            }
            //theOwner.AddCallbackInFrames<int>(theOwner.SetAction, 0);
        }

    }
}