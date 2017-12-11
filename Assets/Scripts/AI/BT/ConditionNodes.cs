/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ConditionNodes
// 创建者：Hooke Hu
// 修改者列表：
// 创建日期：
// 模块描述：继承自ConditionNode各种条件节点
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using UnityEngine;

using Mogo.Game;
using Mogo.FSM;
using Mogo.Util;

namespace Mogo.AI
{
    public enum CmpType { lt, le, eq, ge, gt };

    internal class CmpTypeMethod
    {
        public static bool Cmp(CmpType cmp, int lv, int rv)
        {
            switch (cmp)
            {
                case CmpType.lt:
                    {
                        return lv < rv;
                    }
                case CmpType.le:
                    {
                        return lv <= rv;
                    }
                case CmpType.eq:
                    {
                        return lv == rv;
                    }
                case CmpType.ge:
                    {
                        return lv >= rv;
                    }
                case CmpType.gt:
                    {
                        return lv > rv;
                    }
            }
            return false;
        }
    }
    //------------------------------------------------------------
    public class ISEscapeHP : ConditionNode
    {
        protected int escapeHP = 0;

        public ISEscapeHP(int _escapeHP)
        {
            escapeHP = _escapeHP;
        }

        public override bool Proc(EntityParent theOwner)
        {
            //todo判断是否到了跳跑血量,血量由配置中读取
            //Mogo.Util.LoggerHelper.Debug("escape hp condition");
            return theOwner.curHp <= escapeHP;
        }
    }
    //------------------------------------------------------------
    public class HasFightTarget : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            EntityParent enemy = theOwner.GetTargetEntity();
            if (enemy == null || enemy.curHp <= 0)
            {
                theOwner.blackBoard.enemyId = 0;
            }

            if (theOwner.blackBoard.enemyId != 0)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "HasFightTarget:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "HasFightTarget:" + "false");
			}

            return theOwner.blackBoard.enemyId != 0;
        }
    }
    //------------------------------------------------------------
    public class InSkillRange : ConditionNode
    {
        protected int m_iSkillId = 0;

        public InSkillRange(int skillId)
        {
            m_iSkillId = skillId;
        }

        public override bool Proc(EntityParent theOwner)
        {
            bool rnt = theOwner.ProcInSkillRange(m_iSkillId);
            if (rnt)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "InSkillRange:" + m_iSkillId  + " true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "InSkillRange:" + m_iSkillId + " false");
			}

            return rnt;
        }
    }
    //------------------------------------------------------------
    public class InSkillCoolDown : ConditionNode
    {
        protected int m_iSkillId = 0;

        public InSkillCoolDown(int skillId)
        {
            m_iSkillId = skillId;
        }

        public override bool Proc(EntityParent theOwner)
        {
            bool rnt = theOwner.ProcInSkillCoolDown(m_iSkillId);
            if (rnt)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "InSkillCoolDown:" + m_iSkillId + " true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "InSkillCoolDown:" + m_iSkillId + " false");
			}

            return rnt;
        }
    }
    //------------------------------------------------------------
    public class LastCastIs : ConditionNode
    {
        protected int m_iSkillId = 0;

        public LastCastIs(int skillId)
        {
            m_iSkillId = skillId;
        }

        public override bool Proc(EntityParent theOwner)
        {
            bool rnt = theOwner.ProcLastCastIs(m_iSkillId);
            if (rnt)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "LastCastIs:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "LastCastIs:" + "false");
			}

            return rnt;
        }
    }
    //------------------------------------------------------------
    public class ISRest : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            if (theOwner.blackBoard.aiState == AIState.REST_STATE)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "ISRest:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "ISRest:" + "false");
			}
            return theOwner.blackBoard.aiState == AIState.REST_STATE;
        }
    }
    //------------------------------------------------------------
    public class ISCD : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            if (theOwner.blackBoard.aiState == AIState.CD_STATE)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "ISCD:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "ISCD:" + "false");
			}
            return theOwner.blackBoard.aiState == AIState.CD_STATE;
        }
    }
    //------------------------------------------------------------
    public class IsEventMoveEnd : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            if (theOwner.blackBoard.aiEvent == AIEvent.MoveEnd)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventMoveEnd:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventMoveEnd:" + "false");
			}
            return theOwner.blackBoard.aiEvent == AIEvent.MoveEnd;
        }
    }
    //------------------------------------------------------------
    public class IsEventBorn : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            if (theOwner.blackBoard.aiEvent == AIEvent.Born)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventBorn:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventBorn:" + "false");
			}
            return theOwner.blackBoard.aiEvent == AIEvent.Born;
        }
    }
    //------------------------------------------------------------
    public class IsEventAvatarDie : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            if (theOwner.blackBoard.aiEvent == AIEvent.AvatarDie)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventAvatarDie:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventAvatarDie:" + "false");
			}
            return theOwner.blackBoard.aiEvent == AIEvent.AvatarDie;
        }
    }
    //------------------------------------------------------------
    public class IsEventCDEnd : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            if (theOwner.blackBoard.aiEvent == AIEvent.CDEnd)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventCDEnd:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventCDEnd:" + "false");
			}
            return theOwner.blackBoard.aiEvent == AIEvent.CDEnd;
        }
    }
    //------------------------------------------------------------
    public class IsEventRestEnd : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            if (theOwner.blackBoard.aiEvent == AIEvent.RestEnd)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventRestEnd:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventRestEnd:" + "false");
			}
            return theOwner.blackBoard.aiEvent == AIEvent.RestEnd;
        }
    }
    //------------------------------------------------------------
    public class IsEventBeHit : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            if (theOwner.blackBoard.aiEvent == AIEvent.BeHit)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventBeHit:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventBeHit:" + "false");
			}
            return theOwner.blackBoard.aiEvent == AIEvent.BeHit;
        }
    }
    //------------------------------------------------------------
    public class IsEventAvatarPosSync : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            if (theOwner.blackBoard.aiEvent == AIEvent.AvatarPosSync)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventAvatarPosSync:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsEventAvatarPosSync:" + "false");
			}
            return theOwner.blackBoard.aiEvent == AIEvent.AvatarPosSync;
        }
    }
    //------------------------------------------------------------
    public class CmpRate : ConditionNode
    {
        protected CmpType _cmp;
        protected int _rate;

        public CmpRate(CmpType cmp, int rate)
        {
            _cmp = cmp;
            _rate = rate;
        }

        public override bool Proc(EntityParent theOwner)
        {
            //todo计算概率
            int rate = Mogo.Util.RandomHelper.GetRandomInt(0, 100);
            return CmpTypeMethod.Cmp(_cmp, rate, _rate);
        }
    }
    //------------------------------------------------------------
    public class CmpSelfHP : ConditionNode
    {
        protected CmpType _cmp;
        protected int _percent;

        public CmpSelfHP(CmpType cmp, int percent)
        {
            _cmp = cmp;
            _percent = percent;
        }

        public override bool Proc(EntityParent theOwner)
        {
            //todo检查自身血量
            int percent = (int)(theOwner.PercentageHp * 100);
            return CmpTypeMethod.Cmp(_cmp, percent, _percent);
        }
    }
    //------------------------------------------------------------
    public class CmpEnemyHP : ConditionNode
    {
        protected CmpType _cmp;
        protected int _percent;

        public CmpEnemyHP(CmpType cmp, int percent)
        {
            _cmp = cmp;
            _percent = percent;
        }

        public override bool Proc(EntityParent theOwner)
        {
            //todo检查目标血量
            if (!MogoWorld.Entities.ContainsKey(theOwner.blackBoard.enemyId))
            {//没有目标
                return false;
            }
            EntityParent target = MogoWorld.Entities[theOwner.blackBoard.enemyId];
            int percent = (int)(target.PercentageHp * 100);
            return CmpTypeMethod.Cmp(_cmp, percent, _percent);
        }
    }
    //------------------------------------------------------------
    public class CmpTeammateNum : ConditionNode
    {
        protected CmpType _cmp;
        protected int _num;

        public CmpTeammateNum(CmpType cmp, int num)
        {
            _cmp = cmp;
            _num = num;
        }

        public override bool Proc(EntityParent theOwner)
        {
            //todo检查队友数量
            int num = MogoWorld.MonsterCount;
            return CmpTypeMethod.Cmp(_cmp, num, _num);
        }
    }
    //------------------------------------------------------------
    public class CmpEnemyNum : ConditionNode
    {
        protected CmpType _cmp;
        protected int _num;

        public CmpEnemyNum(CmpType cmp, int num)
        {
            _cmp = cmp;
            _num = num;
        }

        public override bool Proc(EntityParent theOwner)
        {
            //todo检查敌人数量
            int num = theOwner.GetEnemyNum();
            bool rnt = CmpTypeMethod.Cmp(_cmp, num, _num);
            if (rnt == true)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "CmpEnemyNum:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "CmpEnemyNum:" + "false");
			}
            return rnt;
        }
    }
    //------------------------------------------------------------
    public class IsTargetCanBeAttack : ConditionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            //todo检查敌人数量
            bool rnt = theOwner.ProcIsTargetCanBeAttack();
            if (rnt == true)
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsTargetCanBeAttack:" + "true");
			}
            else
			{
                //Mogo.Util.LoggerHelper.Debug("AI:" + "IsTargetCanBeAttack:" + "false");
			}
            return rnt;
        }
    }
    //------------------------------------------------------------
    public class CmpSkillUseCount : ConditionNode
    {
        protected int _skillId;
        protected CmpType _cmp;
        protected int _useCount;

        public CmpSkillUseCount(int skillId, CmpType cmp, int useCount)
        {
            _skillId = skillId;
            _cmp = cmp;
            _useCount = useCount;
        }

        public override bool Proc(EntityParent theOwner)
        {
            int tmpSkillUseCount = theOwner.GetSkillUseCount(_skillId);
            bool rnt =  CmpTypeMethod.Cmp(_cmp, tmpSkillUseCount, _useCount);
            if (rnt == true)
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "CmpSkillUseCount:" + _skillId + " true");
            }
            else
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "CmpSkillUseCount:" + _skillId + "false");
            }
            return rnt;
        }
    }
    //------------------------------------------------------------
    public class LastLookOnModeIs : ConditionNode
    {
        protected int m_iLookOnMode = 0;

        public LastLookOnModeIs(int lookOnMode)
        {
            m_iLookOnMode = lookOnMode;
        }

        public override bool Proc(EntityParent theOwner)
        {
            bool rnt = theOwner.ProcLastLookOnModeIs(m_iLookOnMode);
            if (rnt)
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "LastLookOnModeIs:" + "true");
            }
            else
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "LastLookOnModeIs:" + "false");
            }

            return rnt;
        }
    }
    //------------------------------------------------------------
    public class CmpLastSkillCoord : ConditionNode
    {
        protected CmpType _cmp;
        protected int _distance;

        public CmpLastSkillCoord(CmpType cmp, int distance)
        {
            _cmp = cmp;
            _distance = distance;
        }

        public override bool Proc(EntityParent theOwner)
        {
            //todo检查与之前使用技能时候所在坐标距离
            float testDis = Vector3.Distance(theOwner.blackBoard.lastCastCoord, theOwner.Transform.position);
            bool rnt = CmpTypeMethod.Cmp(_cmp, (int)(testDis*100), _distance);
            if (rnt)
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "CmpLastSkillCoord:" + "true");
            }
            else
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "CmpLastSkillCoord:" + "false");
            }

            return rnt;
        }
    }
    //------------------------------------------------------------
    public class CmpTargetDistance : ConditionNode
    {
        protected CmpType _cmp;
        protected int _distance;

        public CmpTargetDistance(CmpType cmp, int distance)
        {
            _cmp = cmp;
            _distance = distance;
        }

        public override bool Proc(EntityParent theOwner)
        {
            //todo检查与之前使用技能时候所在坐标距离
            EntityParent target = theOwner.GetTargetEntity();
            if (target == null || target.Transform == null)
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "CmpTargetDistance:" + "false");
                return false;
            }
            float testDis = Vector3.Distance(target.Transform.position, theOwner.Transform.position);
            bool rnt = CmpTypeMethod.Cmp(_cmp, (int)(testDis * 100), _distance);
            if (rnt)
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "CmpTargetDistance:" + "true");
            }
            else
            {
                //Mogo.Util.LoggerHelper.Debug("AI:" + "CmpTargetDistance:" + "false");
            }

            return rnt;
        }
    }
    //------------------------------------------------------------
    public class IsPatrolCD : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            bool rnt =  theOwner.IsPatrolCD();
            if (rnt)
            {
                //Mogo.Util.LoggerHelper.Error("AI:" + "IsPatrolCD:" + "true");
            }
            else
            {
                //Mogo.Util.LoggerHelper.Error("AI:" + "IsPatrolCD:" + "false");
            }
            return rnt;
        }
    }
    //------------------------------------------------------------
    public class IsAngerFull : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            bool rnt = theOwner.IsAngerFull();
            if (rnt)
            {
                //Mogo.Util.LoggerHelper.Error("AI:" + "IsAngerFull:" + "true");
            }
            else
            {
                //Mogo.Util.LoggerHelper.Error("AI:" + "IsAngerFull:" + "false");
            }
            return rnt;
        }
    }
    //------------------------------------------------------------
    public class IsPowerFX : ActionNode
    {
        public override bool Proc(EntityParent theOwner)
        {
            bool rnt = theOwner.IsPowerFX();
            if (rnt)
            {
                //Mogo.Util.LoggerHelper.Error("AI:" + "IsPowerFX:" + "true");
            }
            else
            {
                //Mogo.Util.LoggerHelper.Error("AI:" + "IsPowerFX:" + "false");
            }
            return rnt;
        }
    }
}
