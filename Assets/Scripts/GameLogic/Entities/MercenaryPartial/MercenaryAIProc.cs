using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Task;
using Mogo.Mission;
using Mogo.Util;
using Mogo.GameData;
using Mogo.AI;
using Mogo.AI.BT;

namespace Mogo.Game
{
    public partial class EntityMercenary
    {
        //---------------------------------------非AI模块调用AI相关begin------------------------------------------------

        public int CanThink()
        {
            int error = 0;
            if (IsPVP() && MogoWorld.arenaState != 1)
            { error = 14; return error; }
            if (curHp <= 0)
            { error = 1; return error; }
            if (m_aiRoot == null)
            { error = 12; return error; }
            if (!m_bModelBuilded)
            { error = 2; return error; }
            if (!m_borned)
            { error = 3; return error; }
            if (this.hitAir)//为了boss没
            { error = 4; return error; }
            if (this.stiff)
            { error = 8; return error; }
            if (this.knockDown)
            { error = 5; return error; }
            if (this.hitGround)
            { error = 10; return error; }
            if (blackBoard.aiState == AIState.REST_STATE)
            { error = 6; return error; }
            if (!MogoWorld.mainCameraCompleted)
            { error = 13; return error; }
            if (IsPatrolCD())
            { error = 14; return error; }

            //技能释放完毕没有?
            ulong curTick = GetTickCount();
            ulong skillTimeUp = blackBoard.skillActTime + blackBoard.skillActTick;
            if (curTick < skillTimeUp)
            {
                //Mogo.Util.LoggerHelper.Debug("skillActTime:false");
                { error = 7; return error; }
            }

            ulong LookOnTimeUp = blackBoard.LookOn_ActTime + blackBoard.LookOn_Tick;
            if (curTick >= LookOnTimeUp)
            {
                blackBoard.LookOn_Mode = -1;
            }

            if (blackBoard.LookOn_Mode >= 0)
            {
                { error = 9; return error; }
            }


            return error;
        }

        private void ThinkBefore()
        {


            blackBoard.skillActTime = 0;
            blackBoard.skillActTick = 0;

            if (blackBoard.movePoint != null)
            {
                StopMove();
            }

            if (MogoWorld.theLittleGuyID == ID)
            {
                float curdistance = Vector3.Distance(Transform.position, MogoWorld.thePlayer.Transform.position);
                if (curdistance > 10.0f)
                {
                    RemoveTarget();
                    //this.Transform.GetComponent<NavMeshAgent>().enabled = false;
                    this.Transform.position = MogoWorld.thePlayer.Transform.position + new Vector3(0, 0, 1);
                    //this.Transform.GetComponent<NavMeshAgent>().enabled = true;
                }
            }
        }

        public override void Think(AIEvent triggerEvent)
        {
            LastThinkTick = GetTickCount();

            blackBoard.ChangeEvent(triggerEvent);

            int canThinkError = CanThink();
            if (canThinkError != 0)
            {
                
                Mogo.Util.LoggerHelper.Debug("canThinkError:" + canThinkError + " AIEvent" + triggerEvent + " ID:" + ID);
                return;
            }

            Mogo.Util.LoggerHelper.Debug("aiId" + m_monsterData.aiId + "  event:" + triggerEvent);
            m_aiRoot = AIContainer.container.Get((uint)m_monsterData.aiId);//kevintest

            ThinkBefore();
            m_aiRoot.Proc(this);
            ThinkAfter();
        }

        private void ThinkAfter()
        {

        }

        private void RebornAnimationDelay()
        {
            //LoggerHelper.Error("RebornAnimationDelay end");
            if (this.deathFlag == 0 && this.curHp > 0)
            {
                m_stateFlag = Utils.BitReset(m_stateFlag, StateCfg.NO_HIT_STATE);
                this.m_borned = true;
                Think(AIEvent.Born);
            }
        }

        public override void OnMoveTo(GameObject g, Vector3 v)
        {
            if (g == null) return;
            if (Transform == null) return;
            if (g == Transform.gameObject)
            {
                if (IsPatrolCD())
                    StopMove();

                blackBoard.LookOn_Mode = -1;
                //Mogo.Util.LoggerHelper.Debug("                                    OnMoveTo");

                //速度衰减还原
                blackBoard.speedFactor = 1.0f;

                Think(AIEvent.MoveEnd);

            }
        }

        //---------------------------------------AI内部调用相关begin------------------------------------------------

        public override int GetEnemyNum()
        {
            int rnt = 0;
            if (m_factionFlag != MogoWorld.thePlayer.factionFlag && MogoWorld.thePlayer.curHp > 0)
            {//阵营和主角的不一致，找主角
                rnt++;
            }

            //遍历entities 
            foreach (KeyValuePair<uint, Mogo.Game.EntityParent> pair in MogoWorld.Entities)
            {
                EntityParent entity = pair.Value;
                if (entity.Transform == null)
                {
                    continue;
                }

                if (entity.m_factionFlag != m_factionFlag && entity.curHp > 0 && !(entity is EntityMonster))
                {
                    rnt++;
                }
            }

            return rnt;
        }




        //---------------------------------------AI Action相关begin------------------------------------------------


        public override bool ProcMercenaryAOI()
        {
            EntityParent enemy = GetTargetEntity();
            if (enemy != null && enemy.curHp > 0)
            {//目标没死继续杀这个
                return true;
            }
            blackBoard.enemyId = 0;

            //雇佣兵找敌人
            //遍历entities 
            foreach (KeyValuePair<uint, Mogo.Game.EntityParent> pair in MogoWorld.Entities)
            {
                EntityParent entity = pair.Value;

                if (entity.Transform != null && entity.m_factionFlag != m_factionFlag && entity.curHp > 0 && !(entity is EntityMonster))
                {
                    float entityRadius = entity.MonsterData.scaleRadius;
                    if (entityRadius <= 0.1f)
                        entityRadius = (float)entity.GetIntAttr("scaleRadius");

                    //和主人的距离小于10米
                    float entityDistance = Vector3.Distance(entity.Transform.position, MogoWorld.thePlayer.Transform.position) - entityRadius*0.01f;
                    if (entityDistance > 8.0f)
                        continue;

                    blackBoard.enemyId = entity.ID;

                    motor.SetTargetToLookAt(entity.Transform);

                    break;
                }
            }
            if (blackBoard.enemyId <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool ProcPVPAOI()
        {
            EntityParent enemy = GetTargetEntity();
            if (enemy != null && enemy.curHp > 0)
            {//目标没死继续杀这个
                return true;
            }
            blackBoard.enemyId = 0;

            if (MogoWorld.thePlayer.curHp > 0)
            {
                blackBoard.enemyId = MogoWorld.thePlayer.ID;

                motor.SetTargetToLookAt(MogoWorld.thePlayer.Transform);

                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool ProcChooseCastPoint(int skillBagIndex)
        {
            if (m_monsterData.skillIds.Count < skillBagIndex || skillBagIndex <= 0)
            {
                return false;
            }
            skillBagIndex--;
            EntityParent theTarget = GetTargetEntity();

            if (theTarget == null)
                return false;
            int skillId = m_monsterData.skillIds[skillBagIndex];
            if (!SkillData.dataMap.ContainsKey(skillId))
            {
                return false;
            }
            int castRange = GetSkillRange(skillId);

            float entityRadius = theTarget.MonsterData.scaleRadius;
            if (entityRadius <= 0.1f)
                entityRadius = (float)theTarget.GetIntAttr("scaleRadius");

            return GetMovePointStraight((int)(castRange * 0.7f));// + entityRadius * 0.7f

        }


        public override bool ProcCastSpell(int skillBagIndex, int reversal)
        {

            //LoggerHelper.Error("skillBagIndex" + skillBagIndex + " "  + m_monsterData.skillIds.Count);
            if (m_monsterData.skillIds.Count < skillBagIndex || skillBagIndex <= 0)
            {
                return false;
            }

            skillBagIndex--;
            EntityParent enemyEntity = GetTargetEntity();
            if (enemyEntity == null)
            {
                return false;
            }

            if (!NotTurn())
            {

                blackBoard.skillReversal = reversal;
                Vector3 target;
                if (blackBoard.skillReversal > 0)
                {
                    target = Transform.position + (Transform.position - enemyEntity.Transform.position);
                }
                else
                {
                    target = enemyEntity.Transform.position;
                }

                Transform.LookAt(new Vector3(target.x, Transform.position.y, target.z));
            }
            else
            {
            }


            blackBoard.lastCastIndex = skillBagIndex;
            int skillId = m_monsterData.skillIds[skillBagIndex];
            if (!SkillData.dataMap.ContainsKey(skillId))
            {
                return false;//error
            }

            blackBoard.LookOn_LastMode = -1;
            motor.CancleLookAtTarget();
            blackBoard.lastCastCoord = Transform.position;

            this.CastSkill(skillId);

            blackBoard.skillActTime = (uint)GetTotalActionDuration(skillId);

            blackBoard.skillActTick = GetTickCount();

            //ProcEnterCD(1);//kevintestcd
            return true;
        }



        //---------------------------------------AI Condition相关begin------------------------------------------------

        public override bool ProcInSkillRange(int skillBagIndex)
        {
            if (m_monsterData.skillIds.Count < skillBagIndex || skillBagIndex <= 0)
            {
                return false;
            }
            skillBagIndex--;
            if (blackBoard.enemyId == 0)
                return false;

            int skillId = m_monsterData.skillIds[skillBagIndex];
            if (!SkillData.dataMap.ContainsKey(skillId))
                return false;


            EntityParent enemy = GetTargetEntity();
            if (enemy == null)
                return false;

            bool rnt = false;


            if (!SkillData.dataMap.ContainsKey(skillId))
            {
                return false;
            }

            TargetRangeType targetRangeType = 0;

            SkillData tmpSkillData = SkillData.dataMap[skillId];
            //逐个skillaction来判断是否等于TargetRangeType.WorldRange
            foreach (var tmpSkillActionId in tmpSkillData.skillAction)
            {
                if (SkillAction.dataMap[tmpSkillActionId].targetRangeType == (int)TargetRangeType.WorldRange)
                {
                    targetRangeType = TargetRangeType.WorldRange;
                }
            }

            if (targetRangeType == TargetRangeType.WorldRange)
            {
                rnt = skillManager.IsInSkillRange(skillId, blackBoard.enemyId);
            }
            else
            {
                int skillRange = GetSkillRange(skillId);


                int distance = skillRange;
                float entityRadius = enemy.MonsterData.scaleRadius;
                if (entityRadius <= 0.1f)
                    entityRadius = (float)enemy.GetIntAttr("scaleRadius");

                int curdistance = (int)(Vector3.Distance(Transform.position, enemy.Transform.position) * 100 - entityRadius);
           
                if (distance >= curdistance)
                {
                    rnt = true;
                    //if (isLittleGuy)
                    //    Mogo.Util.LoggerHelper.Error("IsInSkillRange:true" + distance + " " + entityRadius + " " + curdistance);
                }
                else
                {
                    rnt = false;
                    //if (isLittleGuy)
                    //    Mogo.Util.LoggerHelper.Error("IsInSkillRange:false" + distance + " " + entityRadius + " " + curdistance);
                }
            }
            return rnt;
        }

        public override bool ProcInSkillCoolDown(int skillBagIndex)
        {
            if (m_monsterData.skillIds.Count < skillBagIndex || skillBagIndex <= 0)
            {
                return false;
            }
            skillBagIndex--;
            int skillId = m_monsterData.skillIds[skillBagIndex];
            if (!SkillData.dataMap.ContainsKey(skillId))
            {
                return false;
            }
            //SkillData skillData = SkillData.dataMap[skillId];

            bool rnt = false;
            MercenaryBattleManager tmpBattleManager = this.battleManger as MercenaryBattleManager;
            rnt = tmpBattleManager.IsCoolDown(skillId);
            return rnt;
        }



        public override bool ProcFollowOwner()
        {
            //SetIntAttr("speed", 500);
            EntityParent enemy = MogoWorld.thePlayer;
            if (enemy == null)
            {
                return false;
            }

            if (enemy.Transform == null)
            {
                LoggerHelper.Error("enemy.Transform == null");
                return false;
            }
            else if (Transform == null)
            {
                LoggerHelper.Error("Transform == null");
                return false;
            }


            int distance = 200;
            int curdistance = (int)(Vector3.Distance(Transform.position, enemy.Transform.position) * 100);

            if (curdistance > 1000)
            {
                RemoveTarget();
                this.Transform.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                this.Transform.position = MogoWorld.thePlayer.Transform.position + new Vector3(0, 0, 1);
                this.Transform.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                return true;
            }

            if (curdistance < 250)
                return false;

            if (distance >= curdistance)
            {
                StopMove();
                return false;
            }
            else
            {
                blackBoard.navTargetDistance = (uint)distance;
                blackBoard.movePoint = enemy.Transform.position;
            }
            float tmpDis = Vector3.Distance(Transform.position, new Vector3(blackBoard.movePoint.x, Transform.position.y, blackBoard.movePoint.z));
            if (tmpDis < 1.0f)
            {
                StopMove();
                return false;
            }

            motor.CancleLookAtTarget();

            if (!MoveToByNav(blackBoard.movePoint, (int)blackBoard.navTargetDistance))
            {
                return false;
            }
            return true;

        }


        public override void ProcLookOn()
        {
            if (m_factionFlag == 0)
            {
                base.ProcLookOn();
                return;
            }

            EntityParent enemy = GetTargetEntity();
            if (enemy == null)
            {
                //Mogo.Util.LoggerHelper.Debug("ProcLookOn enemy null");
                return;
            }
            float curdistance = Vector3.Distance(Transform.position, enemy.Transform.position);
            if (curdistance >= blackBoard.LookOn_DistanceMax)
            { //不能远离了，把远离左右移动概率填0 只能接近，冲锋
                blackBoard.LookOn_ModePercent[1] = 0;
                blackBoard.LookOn_ModePercent[2] = 0;
                blackBoard.LookOn_ModePercent[3] = 0;
                blackBoard.LookOn_ModePercent[4] = 0;
            }
            if (curdistance <= blackBoard.LookOn_DistanceMin)
            { //不能接近了，把接近和冲锋概率填0
                blackBoard.LookOn_ModePercent[0] = 0;
                blackBoard.LookOn_ModePercent[5] = 0;
            }
            if (ProcLastLookOnModeIs(4))
            { //上次使用了休息这次不用休息了
                blackBoard.LookOn_ModePercent[4] = 0;
            }

            int sum = 0;
            for (int i = 0; i < blackBoard.LookOn_ModePercent.Length; i++)
            {
                sum += blackBoard.LookOn_ModePercent[i];
            }
            int randomV = Mogo.Util.RandomHelper.GetRandomInt(sum);
            sum = 0;
            for (int i = 0; i < blackBoard.LookOn_ModePercent.Length; i++)
            {
                sum += blackBoard.LookOn_ModePercent[i];
                if (randomV <= sum)
                {
                    blackBoard.LookOn_Mode = i;
                    break;
                }
            }

            float tmpLookOn_ActTime = 0.0f;
            if (blackBoard.LookOn_Mode == 0 || blackBoard.LookOn_Mode == 1)
            {//接近，远离
                tmpLookOn_ActTime = MoveStraight();
            }
            else if (blackBoard.LookOn_Mode == 2 || blackBoard.LookOn_Mode == 3)
            {//左右绕圈
                tmpLookOn_ActTime = MoveAround();
            }
            else if (blackBoard.LookOn_Mode == 4)
            {//休息
                float timeStanderd = blackBoard.LookOn_ModeInterval[blackBoard.LookOn_Mode];
                float time = Mogo.Util.RandomHelper.GetRandomFloat(timeStanderd * 0.8f, timeStanderd * 1.2f);
                ProcEnterRest((uint)(time * 1000.0f));
            }
            else if (blackBoard.LookOn_Mode == 5)
            {//冲锋
                tmpLookOn_ActTime = MoveAssault();
            }

            if (tmpLookOn_ActTime >= 0.0f)
            {
                blackBoard.LookOn_ActTime = (uint)(tmpLookOn_ActTime * 1000);
                blackBoard.LookOn_Tick = GetTickCount();
            }
        }


        public override float MoveAround()
        {
            if (IsMonster())
            {
                return base.MoveAround();
            }
            //是雇佣兵或者pvp
            EntityParent enemy = GetTargetEntity();
            if (enemy == null)
            {
                //返回出生点				
                //Mogo.Util.LoggerHelper.Debug("blackBoard.enemyId <= 0");
                return 0.0f;
            }
            //速度衰减
            blackBoard.speedFactor = DummyLookOnParam.SPEED_FACTOR_MODE_2_3;


            float tarAngle = Mogo.Util.RandomHelper.GetRandomFloat(DummyLookOnParam.PER_ANGLE * 0.5f, DummyLookOnParam.PER_ANGLE * 1.5f);
            tarAngle *= 2;

            float halfTarAngle = tarAngle / 2;

            Vector3 n = (enemy.Transform.position - Transform.position).normalized;
            float xie = Mathf.Sqrt(n.x * n.x + n.z * n.z);
            float hudu = Mathf.Asin(n.z / xie);
            float angle = hudu / Mathf.PI * 180.0f;
            if (enemy.Transform.position.x > Transform.position.x && enemy.Transform.position.z > Transform.position.z)
            {
                angle = 270 - angle;
            }
            else if (enemy.Transform.position.x < Transform.position.x && enemy.Transform.position.z > Transform.position.z)
            {
                angle = 90 + angle;
            }
            else if (enemy.Transform.position.x < Transform.position.x && enemy.Transform.position.z < Transform.position.z)
            {
                angle = 90 + angle;
            }
            else if (enemy.Transform.position.x > Transform.position.x && Transform.position.z > enemy.Transform.position.z)
            {
                angle = -90 - angle;
            }


            if (blackBoard.LookOn_Mode == 2)
                angle = angle + (90.0f + halfTarAngle);
            else//blackBoard.LookOn_Mode == 3
                angle = angle - (90.0f + halfTarAngle);

            float time = 0.0f;
            float r = Vector3.Distance(enemy.Transform.position, Transform.position);
            float len = (float)Mathf.Sin(halfTarAngle * Mathf.PI / 180.0f) * r * 2;
            time = len / (GetIntAttr("speed") * 0.01f * blackBoard.speedFactor);

            motor.CancleLookAtTarget();
            MoveToByAngle(angle, time, true);

            return time;
        }

        public override float MoveStraight()
        {
            if (IsMonster())
            {//是怪物
                return base.MoveStraight();
            }
            //是雇佣兵或者pvp
            EntityParent enemy = GetTargetEntity();
            if (enemy == null)
            {
                //返回出生点				
                //Mogo.Util.LoggerHelper.Debug("blackBoard.enemyId <= 0");
                return 0.0f;
            }


            Vector3 n = (enemy.Transform.position - Transform.position).normalized;
            float xie = Mathf.Sqrt(n.x * n.x + n.z * n.z);
            float hudu = Mathf.Asin(n.z / xie);
            float angle = hudu / Mathf.PI * 180.0f;
            if (enemy.Transform.position.x > Transform.position.x && enemy.Transform.position.z > Transform.position.z)
            {
                angle = 270 - angle;
            }
            else if (enemy.Transform.position.x < Transform.position.x && enemy.Transform.position.z > Transform.position.z)
            {
                angle = 90 + angle;
            }
            else if (enemy.Transform.position.x < Transform.position.x && enemy.Transform.position.z < Transform.position.z)
            {
                angle = 90 + angle;
            }
            else if (enemy.Transform.position.x > Transform.position.x && Transform.position.z > enemy.Transform.position.z)
            {
                angle = -90 - angle;
            }

            if (blackBoard.LookOn_Mode != 1)
            {
                blackBoard.speedFactor = DummyLookOnParam.SPEED_FACTOR_MODE_0;
                angle -= 180;
            }
            else
            {
                blackBoard.speedFactor = DummyLookOnParam.SPEED_FACTOR_MODE_1;
            }

            float timeStanderd = blackBoard.LookOn_ModeInterval[blackBoard.LookOn_Mode];
            float time = timeStanderd;// Mogo.Util.RandomHelper.GetRandomFloat(timeStanderd * 0.8f, timeStanderd * 1.2f);

            motor.CancleLookAtTarget();
            MoveToByAngle(angle, time, true);

            return time;
        }
    }
}
