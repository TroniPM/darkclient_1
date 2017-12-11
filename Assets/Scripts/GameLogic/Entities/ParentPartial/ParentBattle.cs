using UnityEngine;
using System;
using System.Collections.Generic;

using Mogo.FSM;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
using Mogo.Task;

namespace Mogo.Game
{
    public partial class EntityParent
	{
        virtual public void ClearSkill(bool remove = false, bool naturalEnd = false)
        {
            TimerHeap.DelTimer(hitTimerID);
            TimerHeap.DelTimer(delayAttackTimerID);
            if (currSpellID != -1)
            {
                if (SkillAction.dataMap.ContainsKey(currHitAction) && remove)
                {
                    RemoveSfx(currHitAction);
                }
                SkillData s;
                if (SkillData.dataMap.TryGetValue(currSpellID, out s) && remove)
                {
                    foreach (var i in s.skillAction)
                    {
                        RemoveSfx(i);
                    }
                }
                currHitAction = -1;
            }
            //for (int i = 0; i < hitTimer.Count; i++)
            //{
            //    TimerHeap.DelTimer(hitTimer[i]);
            //}
            hitTimer.Clear();
            //if (Actor)
            //{
                //Actor.AnimatorStateChanged = null;
            //}
            MogoMotor theMotor = motor;
            if (Transform)
            {
                theMotor.enableStick = true;
                theMotor.enableRotation = true;
                theMotor.SetExrtaSpeed(0);
                theMotor.SetMoveDirection(Vector3.zero);
            }
            ChangeMotionState(MotionState.IDLE);
            currSpellID = -1;
            //if (this is EntityMyself && naturalEnd)
            //{
            //    (battleManger as PlayerBattleManager).NextCmd();
            //}
        }

        virtual public void ActionChange(string actionName, bool start)
        {//单个hitAction时会调用到
            if (actionName.EndsWith("ready") && start)
            {
                ClearSkill();
            }
            else if (walkingCastSkill && !actionName.EndsWith("ready") && !actionName.EndsWith("run") && !start)
            {//加入跑步中使用技能时会出现进这条件，所以加非判断过滤
                ClearSkill();
                SetSpeed(0);
            }
        }

        virtual public void PlayerActionChange(string actionName, bool start)
        {//主角以技能动作融合结束为技能结束点
            if (actionName.EndsWith(skillActName) && !start)
            {
                ClearSkill();
            }
        }

        virtual public void ActionChange(string preActName, string currActName)
        {//每帧检查动作切换
            if (skillActName == null)
            {
                skillActName = "";
            }
            if((currActName.EndsWith("ready") || currActName.EndsWith("run")) && currSpellID != -1)
            //if (!currActName.EndsWith(skillActName) && currSpellID != -1 && skillActName != "")
            {
                ClearSkill(false, true);
                if (stiff)
                {
                    ClearHitAct();
                }
                return;
            }
            if ((currActName.EndsWith("ready") ||
                currActName.EndsWith("run") ||
                currActName.EndsWith("run_left") ||
                currActName.EndsWith("run_right") ||
                currActName.EndsWith("run_back")) &&
                (preActName.EndsWith("hit") ||
                preActName.EndsWith("getup") ||
                preActName.EndsWith("push")))
            {
                ClearHitAct();
                return;
            }
            if (preActName.EndsWith("ready") && currActName.EndsWith("run") && stiff)
            {//容错处理
                ClearHitAct();
            }
            if((currActName.EndsWith("ready") ||
                currActName.EndsWith("run") ||
                currActName.EndsWith("run_left") ||
                currActName.EndsWith("run_right") ||
                currActName.EndsWith("run_back")) && stiff)
            {//容错处理
                ClearHitAct();
            }
        }

        virtual public void CleanCharging()
        {
            if (this is EntityMyself && charging)
            {
                (this as EntityMyself).PowerChargeInterrupt();
            }
        }

        virtual public void CastSkill(int nSkillID, Vector3 r)
        {//带方向的释放方法
            CastSkill(nSkillID);
        }

        virtual public void CastSkill(int nSkillID)
        {
            if (MogoWorld.inCity ||
                (this is EntityDummy && stiff))
            {
                return;
            }
            if (this is EntityMyself)
            {
                if (this.vocation == Vocation.Archer || this.vocation == Vocation.Mage)
                {
                    Util.MogoUtils.LookAtTargetInRange(Transform, 6, 270);
                }
            }
            walkingCastSkill = (currentMotionState == MotionState.WALKING);
            currSpellID = nSkillID;
            //hitActionIdx = 1;
            SkillData s = SkillData.dataMap.Get(currSpellID);
            if (s == null || aiRate == 0)
            {//没有技能配置
                return;
            }
            //if (s.skillAction.Count > 1)
            //{
            //    SkillAction act = SkillAction.dataMap.Get(s.skillAction.Get(0));
            //    if (act == null)
            //    {//没有技能行为配置
            //        return;
            //    }
            //    if (act.actionTime > 0)
            //    {
            //        hitTimerID = TimerHeap.AddTimer((uint)(act.actionTime / aiRate), 0, SkillActionEnd);
            //    }
            //    if (act.nextHitTime > 0)
            //    {
            //        hitTimerID = TimerHeap.AddTimer((uint)(act.nextHitTime / aiRate), 0, SkillActionEnd);
            //    }
            //}
            if (ID == MogoWorld.thePlayer.ID)
            {
                LuaTable table;
                List<int> l = new List<int>();
                Mogo.RPC.Utils.PackLuaTable(l, out table);
                //(this as EntityMyself).SyncPos(true);
                System.Int16 x = (System.Int16)(Transform.position.x * 100.0);
                System.Int16 z = (System.Int16)(Transform.position.z * 100.0);
                byte face = (byte)(Transform.eulerAngles.y * 0.5);
                ulong t = 0;
                DateTime d = DateTime.Now;
                t = (ulong)d.Day * 24 * 3600 * 1000 + (ulong)d.Hour * 3600 * 1000 + (ulong)d.Minute * 60 * 1000 + (ulong)d.Second * 1000 + (ulong)d.Millisecond;
                RpcCall("UseSkillReq", (System.UInt64)t, x, z, face, currSpellID, table);
            }
            //currHitAction = s.skillAction.Get(0);
            //处理跑动中用技能后会前进一步问题
            //motor.speed = 0;
            //motor.targetSpeed = 0;
            //battleManger.CastSkill(s.skillAction.Get(0));
            battleManger.CastSkill(currSpellID);
        }

        virtual public void OnAttacking(int actionID, Matrix4x4 ltwm, Quaternion rotation, Vector3 forward, Vector3 position)
        {
            this.battleManger.OnAttacking(actionID, ltwm, rotation, forward, position);
        }

        //动作播放完时调用
        virtual public void SkillActionEnd()
        {
            SkillData s = null;
            if (!SkillData.dataMap.TryGetValue(currSpellID, out s))
            {
                return;
            }
            List<int> actions = s.skillAction;
            if (hitActionIdx >= actions.Count)
            {//使用技能时重置为0
                return;
            }
            SkillAction action = SkillAction.dataMap.Get(actions.Get(hitActionIdx));
            if (action == null)
            {//技能行为配置不存在
                return;
            }
            currHitAction = actions.Get(hitActionIdx);
            this.battleManger.CastSkill(currHitAction);
            hitActionIdx++;
            if (hitActionIdx >= actions.Count)
            {//技能释放完成，重置当前技能为空闲,多个hitAction时调用到
                //currSpellID = -1;
                //MogoMotor theMotor = motor;
                //if (Transform)
                //{
                //    theMotor.enableStick = true;
                //    //theMotor.enableRotation = true;
                //    TimerHeap.AddTimer<MogoMotor>((uint)action.actionEndDuration, 0, (m) => { m.enableRotation = true; }, theMotor);
                //    theMotor.SetExrtaSpeed(0);
                //    theMotor.SetMoveDirection(Vector3.zero);
                //}
                //ChangeMotionState(MotionState.IDLE);
                return;
            }
            //如果有多个动作，填动作时间
            if (action.actionTime > 0)
            {
                hitTimerID = TimerHeap.AddTimer((uint)(action.actionTime / aiRate), 0, SkillActionEnd);
                return;
            }
            if (action.nextHitTime > 0)
            {
                hitTimerID = TimerHeap.AddTimer((uint)(action.nextHitTime / aiRate), 0, SkillActionEnd);
            }
        }

        virtual public void OnHit(int actionID, uint _attackerID, uint woundId, List<int> harm)
        {
            this.battleManger.OnHit(actionID, _attackerID, woundId, harm);
        }

        virtual public void OnDeath(int hitActionID)
        {
            if (this.battleManger != null)
                this.battleManger.OnDead(hitActionID);
        }

        public string CurrActStateName()
        {
            if (animator == null)
            {
                return "";
            }
            var st = animator.GetCurrentAnimatorClipInfo(0);
            if (st.Length == 0)
            {
                return "";
            }
            return st[0].clip.name;
        }

        virtual public void ProcOnHit()
        {
        }
	}
}
