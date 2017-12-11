/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MonsterBattleManager
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-5-20
// 模块描述：客户端控制的怪物， 客户端战斗系统
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.FSM;
using Mogo.GameData;

namespace Mogo.Game
{
    public class DummyBattleManager : BattleManager
    {

        private Dictionary<uint, int> hatedList = new Dictionary<uint, int>();
        private EntityPlayer theTarget = MogoWorld.thePlayer as EntityPlayer;

        private int commonCD = 0;
        private Dictionary<int, int> skillCD = new Dictionary<int,int>();
        private Dictionary<int, float> lastSkillTime = new Dictionary<int, float>();
        // 为去除警告暂时屏蔽以下代码
        //private int lastSkillId = 0;

        public DummyBattleManager(EntityParent _theOwner, SkillManager _skillManager)
            : base(_theOwner, _skillManager)
        {
        }

        public override void Clean()
        {
            base.Clean();
        }

        override public void OnHit(int _spellID, uint _attackerID, uint woundId, List<int> harm)
        {
            if (MogoWorld.inCity ||
                theOwner.ID != woundId ||
                theOwner.ID == _attackerID ||
                !theOwner.canBeHit ||
                (theOwner.GetType() == typeof(EntityDummy) && (theOwner as EntityDummy).IsGolem)
                )
            {
                return;
            }
            int hitType = harm[0];
            int hitNum = harm[1];
            if (harm[0] != 1)
            {
                base.OnHit(_spellID, _attackerID, woundId, harm);
            }
            
            if (MogoWorld.showHitShader && (theOwner as EntityDummy).HitShader != null)
            {
                int _h = Utils.Choice<int>((theOwner as EntityDummy).HitShader);
                if (_h == 1)
                {//等于1闪白
                    MogoFXManager.Instance.GetHit(theOwner.GameObject, 0.2f, MogoFXManager.HitColorType.HCT_WHITE);
                }
                else if(_h == 2)
                {//等于2闪红
                    MogoFXManager.Instance.GetHit(theOwner.GameObject, 0.2f, MogoFXManager.HitColorType.HCT_RED);
                }
            }
            if ( theOwner.curHp > 0)
            {
                if (hatedList.ContainsKey(_attackerID))
                {
                    hatedList[_attackerID] += 1;
                }
                else
                {
                    hatedList[_attackerID] = 1;
                }

            }
            if (MogoWorld.showFloatBlood && SkillAction.dataMap[_spellID].damageFlag == 1)
            {
                FloatBlood(hitType, hitNum);
            }

            //受击时怪物集中精神扩大视野
            if (!theOwner.blackBoard.HasHatred(_attackerID))
            {
                theOwner.currentSee = 10000;//10000cm 意志在于一定能看见玩家
                theOwner.blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);//不还原状态的话，如果在巡逻冷却状态那么就会延迟到结束才会发生追杀AI
                theOwner.Think(Mogo.AI.AIEvent.AvatarPosSync);
            }
            
        }

        override public void OnDead(int actionID)
        {
            LoggerHelper.Debug("DummyBattleManager OnDead");
            base.OnDead(actionID);
        }

        override public void CastSkill(int actionID)
        {
            if (theOwner.CurrentMotionState == MotionState.LOCKING
                || theOwner.CurrentMotionState == MotionState.DEAD
                || theOwner.CurrentMotionState == MotionState.PICKING
                || theOwner.CurrentMotionState == MotionState.PREPARING)
            {
                LoggerHelper.Debug("the entity is busy" + theOwner.CurrentMotionState);
                return;
            }
            //theOwner.ChangeMotionState(MotionState.PREPARING, nSpellID);
            theOwner.ChangeMotionState(MotionState.ATTACKING, actionID);
        }

        public void SetCoolDown(int skillId)
        {
            SkillData s = SkillData.dataMap[skillId];
            commonCD = s.cd[3];
            float t = Time.realtimeSinceStartup;
            if (!skillCD.ContainsKey(skillId))
            {
                skillCD.Add(skillId, s.cd[0]);
                lastSkillTime.Add(skillId, t);
            }
            else
            {
                lastSkillTime[skillId] = t;
            }
        }

        public bool IsCoolDown(int skillId)
        {
            int t = (int)(Time.realtimeSinceStartup * 1000);
            float preT = 0;
            if (lastSkillTime.ContainsKey(skillId))
            {
                preT = lastSkillTime.Get<int, float>(skillId);
            }
            t = t - (int)(preT * 1000);
            if (t < commonCD)
            {
                return false;
            }
            if (skillCD.ContainsKey(skillId) && t < skillCD[skillId])
            {
                return false;
            }
            return true;
        }

        public void FindTarget()
        {
            //在仇恨列表中找
            foreach (uint id in hatedList.Keys)
            {
                EntityParent target = MogoWorld.Entities[id];
                if (Vector3.Distance(target.Transform.position, theOwner.Transform.position) <= 10)
                {
                    theTarget = target as EntityPlayer;
                    return;
                }
            }

            //看范围内有无其他敌人
            //List<GameObject> targets = MogoUtils.GetEntitiesInRange(theOwner.transform, 10, Util.LayerMask.Character);
            //if (targets.Count > 0)
            //{
            //    //取最近那一个作为目标
            //    MogoUtils.SortByDistance(theOwner.transform, targets);
            //    theTarget = targets[0].GetComponent<ActorParent>().GetEntity() as EntityPlayer;
            //    return;
            //}

            theTarget = null;


        }

        
    }
}