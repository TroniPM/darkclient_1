/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StateAttacking
// 创建者：Steven Yang
// 修改者列表：2013-1-29
// 创建日期：
// 模块描述：Attacking状态
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

namespace Mogo.FSM
{
	public class StateAttacking : IState
	{
		// 进入该状态
		public void Enter(EntityParent theOwner, params Object[] args)
		{
			theOwner.CurrentMotionState = MotionState.ATTACKING;
		}

		// 离开状态
		public void Exit(EntityParent theOwner, params Object[] args)
		{
		}

		// 状态处理
		public void Processold(EntityParent theOwner, params Object[] args)
		{
			if (theOwner is EntityDummy)
			{
				theOwner.animator.applyRootMotion = true;
			}

			if (args.Length != 1)
			{
				LoggerHelper.Error("error spell id");
				return;
			}
			int actionID = (int)(args[0]);
			SkillAction action = SkillAction.dataMap[actionID];
			SkillData s = SkillData.dataMap[theOwner.currSpellID];

			// 回调，基于计时器。 在duration 后切换回 idle 状态
			int duration = action.duration;
			if (duration <= 0 && action.action > 0 && s.skillAction.Count == 1)
			{
                //theOwner.AddCallbackInFrames<int, EntityParent>(
                //    (_actionID, _theOwner) =>
                //    {
                //        _theOwner.TriggerUniqEvent<int>(Events.FSMMotionEvent.OnAttackingEnd, _actionID);
                //    },
                //    actionID,
                //    theOwner);
			}
			else if (duration <= 0 && s.skillAction.Count > 1 && theOwner.hitActionIdx >= (s.skillAction.Count - 1))
			{
				if (SkillAction.dataMap[s.skillAction[0]].duration <= 0)
				{
					theOwner.AddCallbackInFrames<int, EntityParent>(
						(_actionID, _theOwner) =>
						{
							_theOwner.TriggerUniqEvent<int>(Events.FSMMotionEvent.OnAttackingEnd, _actionID);
						},
						actionID,
						theOwner);
				}
			}
			else if (duration > 0 && action.action > 0)
			{
                //theOwner.stateFlag = Mogo.Util.Utils.BitSet(theOwner.stateFlag, 13);//专为旋风斩
                //theOwner.immuneShift = true;
				TimerHeap.AddTimer<int, EntityParent>(
					(uint)duration,
					0,
					(_actionID, _theOwner) =>
					{
						_theOwner.TriggerUniqEvent<int>(Events.FSMMotionEvent.OnAttackingEnd, _actionID);
						MogoMotor theMotor = _theOwner.motor;
						if (_theOwner.Transform)
						{
							theMotor.enableStick = true;
							theMotor.SetExrtaSpeed(0);
							theMotor.SetMoveDirection(UnityEngine.Vector3.zero);
						}
                        //_theOwner.stateFlag = Mogo.Util.Utils.BitReset(_theOwner.stateFlag, 13);//专为旋风斩
                        //_theOwner.immuneShift = false;
                        _theOwner.ChangeMotionState(MotionState.IDLE);
					},
					actionID,
					theOwner);
			}
			// 回调，基于计时器。 在removeSfxTime 后关闭持久的sfx
			if (action.duration > 0)
			{
				TimerHeap.AddTimer<int, EntityParent>(
					(uint)action.duration,
					0,
					(_actionID, _theOwner) =>
					{
						_theOwner.RemoveSfx(_actionID);
					},
					actionID,
					theOwner);
			}
			theOwner.OnAttacking(actionID, theOwner.Transform.localToWorldMatrix, theOwner.Transform.rotation, theOwner.Transform.forward, theOwner.Transform.position);
		}

        public void Process(EntityParent theOwner, params Object[] args)
        {
            if (theOwner is EntityDummy)
            {
                theOwner.animator.applyRootMotion = true;
            }

            if (args.Length != 1)
            {
                LoggerHelper.Error("error spell id");
                return;
            }
            int spellID = (int)(args[0]);
            SkillData s = SkillData.dataMap[spellID];
            theOwner.motor.speed = 0;
            theOwner.motor.targetSpeed = 0;
            int baseTime = 0;
            for (int i = 0; i < s.skillAction.Count; i++)
            {
                SkillAction action = SkillAction.dataMap[s.skillAction[i]];
                List<object> args1 = new List<object>();
                args1.Add(s.skillAction[0]);
                args1.Add(theOwner.Transform.localToWorldMatrix);
                args1.Add(theOwner.Transform.rotation);
                args1.Add(theOwner.Transform.forward);
                args1.Add(theOwner.Transform.position);
                if (i == 0)
                {
                    ProcessHit(theOwner, spellID, args1);
                    if (theOwner is EntityMyself)
                    {
                        theOwner.motor.enableStick = action.enableStick > 0;
                    }
                }
                if (i + 1 == s.skillAction.Count)
                {
                    break;
                }
                uint tid = 0;
                List<object> args2 = new List<object>();
                args2.Add(s.skillAction[i + 1]);
                args2.Add(theOwner.Transform.localToWorldMatrix);
                args2.Add(theOwner.Transform.rotation);
                args2.Add(theOwner.Transform.forward);
                args2.Add(theOwner.Transform.position);
                if (action.actionTime > 0)
                {
                    tid = TimerHeap.AddTimer((uint)((baseTime + action.actionTime) / theOwner.aiRate), 0, ProcessHit, theOwner, spellID, args2);
                    baseTime += action.actionTime;
                }
                if (action.nextHitTime > 0)
                {
                    tid = TimerHeap.AddTimer((uint)((baseTime + action.nextHitTime) / theOwner.aiRate), 0, ProcessHit, theOwner, spellID, args2);
                    baseTime += action.nextHitTime;
                }
                theOwner.hitTimer.Add(tid);
            }
        }

        private void ProcessHit(EntityParent theOwner, int spellID, List<object> args)
        {
            int actionID = (int)args[0];
            UnityEngine.Matrix4x4 ltwm = (UnityEngine.Matrix4x4)args[1];
            UnityEngine.Quaternion rotation = (UnityEngine.Quaternion)args[2];
            UnityEngine.Vector3 forward = (UnityEngine.Vector3)args[3];
            UnityEngine.Vector3 position = (UnityEngine.Vector3)args[4];
            if (theOwner is EntityDummy && theOwner.animator != null)
            {
                theOwner.animator.applyRootMotion = true;
            }

            SkillAction action = SkillAction.dataMap[actionID];
            SkillData s = SkillData.dataMap[spellID];

            // 回调，基于计时器。 在duration 后切换回 idle 状态
            int duration = action.duration;
            if (duration <= 0 && s.skillAction.Count > 1)// && theOwner.hitActionIdx >= (s.skillAction.Count - 1))
            {
                if (SkillAction.dataMap[s.skillAction[0]].duration <= 0)
                {
                    theOwner.AddCallbackInFrames<int, EntityParent>(
                        (_actionID, _theOwner) =>
                        {
                            _theOwner.TriggerUniqEvent<int>(Events.FSMMotionEvent.OnAttackingEnd, _actionID);
                        },
                        actionID,
                        theOwner);
                }
            }
            else if (duration > 0 && action.action > 0)
            {
                TimerHeap.AddTimer<int, EntityParent>(
                    (uint)duration,
                    0,
                    (_actionID, _theOwner) =>
                    {
                        _theOwner.TriggerUniqEvent<int>(Events.FSMMotionEvent.OnAttackingEnd, _actionID);
                        MogoMotor theMotor = _theOwner.motor;
                        if (_theOwner.Transform)
                        {
                            theMotor.enableStick = true;
                            theMotor.SetExrtaSpeed(0);
                            theMotor.SetMoveDirection(UnityEngine.Vector3.zero);
                        }
                        //_theOwner.stateFlag = Mogo.Util.Utils.BitReset(_theOwner.stateFlag, 13);//专为旋风斩
                        //_theOwner.immuneShift = false;
                        _theOwner.ChangeMotionState(MotionState.IDLE);
                    },
                    actionID,
                    theOwner);
            }
            // 回调，基于计时器。 在removeSfxTime 后关闭持久的sfx
            if (action.duration > 0)
            {
                TimerHeap.AddTimer<int, EntityParent>(
                    (uint)action.duration,
                    0,
                    (_actionID, _theOwner) =>
                    {
                        _theOwner.RemoveSfx(_actionID);
                    },
                    actionID,
                    theOwner);
            }
            theOwner.OnAttacking(actionID, ltwm, rotation, forward, position);
        }

	}
}