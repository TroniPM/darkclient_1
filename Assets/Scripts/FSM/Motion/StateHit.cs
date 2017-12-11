/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StateHit
// 创建者：Steven Yang
// 修改者列表：2013-1-29
// 创建日期：
// 模块描述：受击状态
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.GameData;

using Mogo.Util;

namespace Mogo.FSM
{
    public class StateHit : IState
    {
        // 进入该状态
        public void Enter(EntityParent theOwner, params Object[] args)
        {
            theOwner.CurrentMotionState = MotionState.HIT;
            if (theOwner is EntityDummy)
            {
                theOwner.ApplyRootMotion(true);
            }
        }

        // 离开状态
        public void Exit(EntityParent theOwner, params Object[] args)
        {
            if (theOwner is EntityDummy)
            {
                theOwner.ApplyRootMotion(false);
            }
        }

        // 状态处理
        public void Process(EntityParent theOwner, params Object[] args)
        {
            if (theOwner == null || theOwner.Transform == null)
                return;

            if (theOwner.IsDeath())
                return;

            if (args.Length < 2)
            {
                LoggerHelper.Error("error skill id");
                return;
            }
            int hitActionID = (int)(args[0]);
            uint attackID = (uint)(args[1]);
            EntityParent attacker = null;
            if (MogoWorld.Entities.ContainsKey(attackID))
            {
                attacker = MogoWorld.Entities[attackID];
            }
            if (attackID == MogoWorld.thePlayer.ID)
            {
                attacker = MogoWorld.thePlayer;
            }
            if (theOwner.motor != null)
            {
                theOwner.motor.enableStick = false;
            }
            //if (attacker == null)
            //{//没有受击者
            //    return;
            //}
            List<int> hitAction = SkillAction.dataMap[hitActionID].hitAction;
            if (hitAction == null || hitAction.Count == 0)
            {
                return;
            }
            int action = Utils.Choice<int>(hitAction);
            if (action == ActionConstants.HIT)
            {
                HitActionRule(theOwner, action, hitActionID);
                return;
            }
            if (!(theOwner is EntityPlayer))
            {
                if (((theOwner is EntityMonster) && theOwner.GetIntAttr("notTurn") == 0) || theOwner is EntityDummy)
                {
                    if (theOwner.motor == null)
                    {
                        return;
                    }
                    if (attacker != null && attacker.Transform != null)
                    {
                        theOwner.motor.SetTargetToLookAt(attacker.Transform.position);
                    }
                }
            }
            else
            {
                if (theOwner.Transform == null)
                {
                    return;
                }
                theOwner.preQuaternion = theOwner.Transform.localRotation;
                if (attacker != null && attacker.Transform != null)
                    theOwner.motor.SetTargetToLookAt(attacker.Transform.position);
            }
            HitActionRule(theOwner, action, hitActionID);
        }

        private void HitActionRule(EntityParent theOwner, int act, int hitActionID)
        {
            int action = act;
            string actName = theOwner.CurrActStateName();
            if (actName.EndsWith(PlayerActionNames.names[ActionConstants.KNOCK_DOWN]))
            {//击飞状态，受非浮空打击，一直倒地
                action = ActionConstants.KNOCK_DOWN;
                return;
            }
            if (actName.EndsWith(PlayerActionNames.names[ActionConstants.HIT_AIR]))
            {//浮空受击
                action = ActionConstants.HIT_AIR;
            }
            else if (actName.EndsWith(PlayerActionNames.names[ActionConstants.HIT_GROUND]) || actName.EndsWith("knockout"))
            {
                action = ActionConstants.HIT_GROUND;
            }
            else if ((theOwner is EntityMyself) && (action == ActionConstants.HIT_AIR || action == ActionConstants.KNOCK_DOWN))
            {
                int random = Mogo.Util.RandomHelper.GetRandomInt(0, 100);
                if (random <= 70)
                {//主角受到倒地状态影响时，只有30%机会成功，否则变成普通受击 By.铭
                    action = ActionConstants.HIT;
                }
            }

            theOwner.SetAction(action);
            EventDispatcher.TriggerEvent(Events.LogicSoundEvent.OnHitYelling, theOwner as EntityParent, action);

            // 回调函数， 切换到 idle
            TimerHeap.AddTimer<EntityParent, int>(100, 0,
                (_theOwner, _hitAct) =>
                {
                    if (_theOwner == null)
                    {
                        return;
                    }
                    if (_theOwner.motor != null)
                    {
                        _theOwner.motor.CancleLookAtTarget();
                    }
                    //_theOwner.TriggerUniqEvent<int>(Events.FSMMotionEvent.OnHitAnimEnd, _hitAct);
                    if (_theOwner is EntityMyself)
                    {
                        _theOwner.SetSpeed(0);
                        _theOwner.motor.SetSpeed(0);
                    }
                },
                theOwner, hitActionID
            );
        }

        private void HitRule(EntityParent theOwner, int act, int hitActionID)
        {
            int action = act;
            HitState h = null;
            if (!HitState.dataMap.TryGetValue(theOwner.hitStateID, out h))
            {//如果没有配置，用回原来接口
                HitActionRule(theOwner, action, hitActionID);
                return;
            }
            string actName = theOwner.CurrActStateName();
            if (theOwner.currSpellID != -1)
            {
                action = h.GetSkillAct(action);
            }
            else if (actName.EndsWith(PlayerActionNames.names[ActionConstants.HIT]))
            {
                action = h.GetHitAct(action);
            }
            else if(actName.EndsWith(PlayerActionNames.names[ActionConstants.PUSH]))
            {
                action = h.GetPushAct(action);
            }
            else if(actName.EndsWith(PlayerActionNames.names[ActionConstants.KNOCK_DOWN]))
            {
                action = h.GetKnockDownAct(action);
            }
            else if(actName.EndsWith(PlayerActionNames.names[ActionConstants.HIT_AIR]))
            {
                action = h.GetHitAirAct(action);
            }
            else if (actName.EndsWith("knockout"))
            {
                action = h.GetKnockOutAct(action);
            }
            else if (actName.EndsWith("getup"))
            {
                action = h.GetGetUpAct(action);
            }
            else if (theOwner.animator.GetInteger("Action") == ActionConstants.REVIVE)
            {
                action = h.GetReviveAct(action);
            }
            else //(theOwner.CurrentMotionState == MotionState.IDLE)
            {
                action = h.GetIdleAct(action);
            }
            if (action != 0)
            {
                theOwner.SetAction(action);
            }
            // 回调函数， 切换到 idle
            theOwner.AddCallbackInFrames<EntityParent, int>(
                (_theOwner, _hitAct) =>
                {
                    _theOwner.motor.CancleLookAtTarget();
                    _theOwner.TriggerUniqEvent<int>(Events.FSMMotionEvent.OnHitAnimEnd, _hitAct);
                    if (_theOwner is EntityMyself)
                    {
                        _theOwner.SetSpeed(0);
                        _theOwner.motor.SetSpeed(0);
                    }
                },
                theOwner, hitActionID
            );
        }
    }
}