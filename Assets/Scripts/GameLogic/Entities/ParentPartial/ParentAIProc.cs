using UnityEngine;
using System;
using System.Collections.Generic;


using Mogo.FSM;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
using Mogo.Task;
using Mogo.AI;
using Mogo.AI.BT;

namespace Mogo.Game
{
    public partial class EntityParent
	{
        public bool IsDeath()
        {
            return (Mogo.Util.Utils.BitTest(m_stateFlag, StateCfg.DEATH_STATE) == 1) ? true : false;
        }

        protected ulong m_iLastThinkTick;

        public ulong LastThinkTick
        {
            get { return m_iLastThinkTick; }
            set { m_iLastThinkTick = value; }
        }

        protected MonsterData m_monsterData = new MonsterData();

        public MonsterData MonsterData
        {
            get { return m_monsterData; }
            set { m_monsterData = value; }
        }

        public void StopMove()
        {
            Idle();
            //速度衰减还原
            blackBoard.speedFactor = 1.0f;
            if (motor)
                motor.StopNav();
        }

        public int GetSkillRange(int skillId)
        {
            if (!SkillData.dataMap.ContainsKey(skillId))
            {
                return 0;
            }

            return (int)(SkillData.dataMap[skillId].castRange);
        }

        public int GetTotalActionDuration(int skillId)
        {
            return (int)(SkillData.GetTotalActionDuration(skillId) * aiRate);
        }

        public void FaceToTarget(EntityParent targetEntity)
        {
            Vector3 target = targetEntity.Transform.position;
            Transform.LookAt(new Vector3(target.x, Transform.position.y, target.z));
        }

        public void RemoveTarget()
        {
            blackBoard.enemyId = 0;
        }

        public EntityParent GetTargetEntity()
        {
            if (blackBoard.enemyId == MogoWorld.thePlayer.ID)
            {
                return MogoWorld.thePlayer;
            }
            else if (MogoWorld.Entities.ContainsKey(blackBoard.enemyId))
            {
                return MogoWorld.Entities[blackBoard.enemyId];
            }
            else
                return null;
        }

        public Container GetTargetContainer()
        {
            if (blackBoard.enemyTrap != null)
                return blackBoard.enemyTrap;
            else
                return null;
        }

        public EntityParent GetMercenaryEntity()
        {
            if (MogoWorld.Entities.ContainsKey(MogoWorld.theLittleGuyID))
                return MogoWorld.Entities[MogoWorld.theLittleGuyID];
            else
                return null;
        }

        public virtual float MoveAround()
        {
            EntityParent enemy = GetTargetEntity();
            if (enemy == null)
            {
                //返回出生点				
                ////Mogo.Util.LoggerHelper.Debug("blackBoard.enemyId <= 0");
                return 0.0f;
            }
            //速度衰减
            blackBoard.speedFactor = DummyLookOnParam.SPEED_FACTOR_MODE_2_3;


            float tarAngle = Mogo.Util.RandomHelper.GetRandomFloat(DummyLookOnParam.PER_ANGLE * 0.5f, DummyLookOnParam.PER_ANGLE * 1.5f);

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
            ////Mogo.Util.LoggerHelper.Debug("time:" + time + " len:" + len + " speed:" + (GetIntAttr("speed") / 100.0f) + " r:" + r + " sin:" + (float)Mathf.Sin(halfTarAngle * Mathf.PI / 180.0f));

            motor.SetTargetToLookAt(enemy.Transform);
            MoveToByAngle(angle, time, false);

            return time;
        }

        public virtual float MoveStraight()
        {
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

            motor.SetTargetToLookAt(enemy.Transform);
            MoveToByAngle(angle, time, false);

            return time;
        }



        public bool GetMovePointStraight(int skillRange)
        {
           // LoggerHelper.Error("GetMovePointStraight");
            EntityParent enemy = GetTargetEntity();
            Container enemyTrap = GetTargetContainer();
            if (enemy == null && enemyTrap == null)
                return false;

            if (enemy != null)
            {
                if (enemy.Transform == null)
                {
                    LoggerHelper.Error("enemy.Transform == null");
                }
                else if (Transform == null)
                {
                    LoggerHelper.Error("Transform == null");
                }
                int distance = skillRange;

                float entityRadius = enemy.MonsterData.scaleRadius;
                if (entityRadius <= 0.1f)
                    entityRadius = (float)enemy.GetIntAttr("scaleRadius");

                int curdistance = (int)(Vector3.Distance(Transform.position, enemy.Transform.position) * 100 - entityRadius);

                if (distance >= curdistance)// || curdistance-distance < 0.1f
                {
                   
                    StopMove();
                    return false;
                }
                else
                {
                    blackBoard.navTargetDistance = (uint)skillRange + (uint)entityRadius;
                    blackBoard.movePoint = enemy.Transform.position;
                }

                float tmpDis = Vector3.Distance(Transform.position, new Vector3(blackBoard.movePoint.x, Transform.position.y, blackBoard.movePoint.z));
                if (tmpDis < 1.0f)
                {
                    StopMove();
                    return false;
                }

               
                return true;
            }
            else if (enemyTrap != null)
            {
                if (enemyTrap.transform == null)
                {
                    LoggerHelper.Error("enemy.Transform == null");
                }
                else if (Transform == null)
                {
                    LoggerHelper.Error("Transform == null");
                }
                int distance = skillRange;
                int curdistance = (int)(Vector3.Distance(Transform.position, enemyTrap.transform.position) * 100);

                if (distance >= curdistance)// || curdistance-distance < 0.1f
                {
                    
                    StopMove();
                    return false;
                }
                else
                {
                    blackBoard.navTargetDistance = (uint)skillRange;
                    blackBoard.movePoint = enemyTrap.transform.position;
                }

                float tmpDis = Vector3.Distance(Transform.position, new Vector3(blackBoard.movePoint.x, Transform.position.y, blackBoard.movePoint.z));
                if (tmpDis < 1.0f)
                {
                    StopMove();
                    return false;
                }

                
                return true;
            }
            else
                return false;
        }

        public void ProcessAITimerEvent_RestEnd()
        {
            blackBoard.LookOn_Mode = -1;
            blackBoard.timeoutId = 0;
            blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);
            Think(AIEvent.RestEnd);
        }

        public void ProcessAITimerEvent_CDEnd()
        {
            blackBoard.timeoutId = 0;
            blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);
            Think(AIEvent.CDEnd);
        }

        public void ProcessAITimerEvent_PatrolCDEnd()
        {

            blackBoard.timeoutId = 0;
            blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);
            //Think(AIEvent.CDEnd);
        }

        public System.UInt64 GetTickCount()
        {
            return (System.UInt64)(Time.time * 1000);
        }

        public virtual bool ProcAOI(int searchChance)
        {
            EntityParent littleGuy = GetMercenaryEntity();
            //检测是否需要清除仇恨，例如entity不存在或者死亡 

            //此方法只提供给怪物用，雇佣兵用ProcMercenaryAOI

            //把距离满足条件的活物对手加入仇恨列表
            if (!blackBoard.HasHatred(MogoWorld.thePlayer.ID) && !MogoWorld.thePlayer.IsDeath())
            {
                int tmpDis = (int)(Vector3.Distance(Transform.position, MogoWorld.thePlayer.Transform.position)*100);
                //LoggerHelper.Error("m_currentSee:" + m_currentSee);
                if (tmpDis < m_currentSee)
                { //发现新目标,在距离内,加入仇恨列表
                    blackBoard.EditHatred(MogoWorld.thePlayer.ID, 1);
                    //通知其他同出生点的伙伴们也把新目标加入仇恨列表
                    EventDispatcher.TriggerEvent(Events.AIEvent.WarnOtherSpawnPointEntities, ID, (byte)Mogo.Game.AIWarnEvent.DiscoverSomeOne, MogoWorld.thePlayer.ID, spawnPointCfgId);
                    //LoggerHelper.Error("thePlayer closed!!!!!!!!!!!!");
                }
            }

            if (littleGuy != null && !blackBoard.HasHatred(littleGuy.ID) && !littleGuy.IsDeath())
            {
                int tmpDis = (int)(Vector3.Distance(Transform.position, littleGuy.Transform.position) * 100);
                if (tmpDis < m_currentSee)
                { //发现新目标,在距离内,加入仇恨列表
                    blackBoard.EditHatred(littleGuy.ID, 1);
                    EventDispatcher.TriggerEvent(Events.AIEvent.WarnOtherSpawnPointEntities, ID, Mogo.Game.AIWarnEvent.DiscoverSomeOne, littleGuy.ID, spawnPointCfgId);
                }
            }

            //取一个作为目标,暂时没随机
            if (blackBoard.mHatred.Count > 0)
            {
                foreach (KeyValuePair<uint, int> v in blackBoard.mHatred)
                {
                    if (v.Value <= 0)
                        continue;

                    blackBoard.enemyId = v.Key;

                    if (v.Key == MogoWorld.thePlayer.ID)
                    {
                        //LoggerHelper.Error("                                           see thePlayer !!!!!!!!!!!!");
                        if (!NotTurn()) motor.SetTargetToLookAt(MogoWorld.thePlayer.Transform);
                    }
                    else if (littleGuy != null && v.Key == littleGuy.ID)
                    {
                        if (!NotTurn()) motor.SetTargetToLookAt(littleGuy.Transform);
                    }

                    break;
                }
            }

            if (blackBoard.enemyId == 0)
                return false;
            else
                return true;
        }

        public virtual void ProcThink()
        {
            Think(AIEvent.Self);
        }

        public virtual void ProcMoveTo()
        {
           // LoggerHelper.Error("ProcMoveTo");
            if (!MoveToByNav(blackBoard.movePoint, (int)blackBoard.navTargetDistance))
            { //寻路失败 观望
               // LoggerHelper.Error("ProcMoveTo false");
            }
        }


        public virtual void ProcReinitLastCast()
        {
            blackBoard.lastCastIndex = 0;
        }

        public virtual void ProcEnterRest(uint sec)
        {
            if (blackBoard.timeoutId > 0)
                TimerHeap.DelTimer(blackBoard.timeoutId);
            //Mogo.Util.LoggerHelper.Debug("ProcEnterRest" + sec);
            blackBoard.ChangeState(Mogo.AI.AIState.REST_STATE);
            blackBoard.timeoutId = TimerHeap.AddTimer(sec, 0, ProcessAITimerEvent_RestEnd);//100毫秒用来延缓一点技能CD

        }

        public virtual void ProcEnterCD(int sec)
        {
            if (blackBoard.timeoutId > 0)
                TimerHeap.DelTimer(blackBoard.timeoutId);

            blackBoard.ChangeState(Mogo.AI.AIState.CD_STATE);
            
            int tmpCDTime = sec + (int)blackBoard.skillActTime + 100;
            if (tmpCDTime < 0)
                tmpCDTime = 100;//减去之后延迟不能为0
            blackBoard.skillActTime = (uint)tmpCDTime;
            blackBoard.timeoutId = TimerHeap.AddTimer(blackBoard.skillActTime, 0, ProcessAITimerEvent_CDEnd);//100毫秒用来延缓一点技能CD
        }

        public virtual bool ProcIsTargetCanBeAttack() 
        {
            return true;
        }

        public virtual void ProcEscape(uint sec)
        {
            EntityParent enemy = GetTargetEntity();
            if (enemy == null)
                return;

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
            int shiftAngle = Mogo.Util.RandomHelper.GetRandomInt(-45, 45);
            angle += shiftAngle;

            motor.CancleLookAtTarget();
            MoveToByAngle(angle, sec / 1000.0f, true);
            blackBoard.LookOn_Mode = 6;
        }

        public virtual float MoveAssault()
        { //要冲锋 
            int skillBagIndex = blackBoard.LookOn_Mode5Skill;
            skillBagIndex--;
            int skillId = m_monsterData.skillIds[skillBagIndex];
            if (!SkillData.dataMap.ContainsKey(skillId))
            {
                return 0.0f;
            }

            int castRange = GetSkillRange(skillId);

            //速度衰减
            blackBoard.speedFactor = DummyLookOnParam.SPEED_FACTOR_MODE_5;

            int distanceCastRange = (int)(castRange * 0.9f);//Mogo.Util.RandomHelper.GetRandomInt((int)(castRange * (1 - DummyLookOnParam.CLOSE_MODE_FLOAT_FACTOR)),(int)(castRange * (1 + DummyLookOnParam.CLOSE_MODE_FLOAT_FACTOR)));
            if (GetMovePointStraight(distanceCastRange))
            {
                if (!MoveToByNav(blackBoard.movePoint, (int)blackBoard.navTargetDistance))
                { //寻路失败 观望
                }
                return 0.7f;
            }
            else
            {
                blackBoard.LookOn_Mode = 1;
                return MoveStraight();
            }
        }

        public virtual void ProcLookOn()
        {
            EntityParent enemy = GetTargetEntity();
            if (enemy == null)
            {
                Mogo.Util.LoggerHelper.Debug("ProcLookOn enemy null");
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
            /*
            if (ProcLastLookOnModeIs(4))
            { //上次使用了休息这次不用休息了
                blackBoard.LookOn_ModePercent[4] = 0;
                LoggerHelper.Error("c:" + blackBoard.LookOn_ModePercent[4]);
            }
            */
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
            //Mogo.Util.LoggerHelper.Debug("blackBoard.LookOn_Mode:" + blackBoard.LookOn_Mode);
            float tmpLookOn_ActTime = 0.0f;
            if (blackBoard.LookOn_Mode == 0 || blackBoard.LookOn_Mode == 1)
            {//接近，远离
                tmpLookOn_ActTime = MoveStraight();
                tmpLookOn_ActTime = blackBoard.LookOn_ModeInterval[blackBoard.LookOn_Mode];
            }
            else if (blackBoard.LookOn_Mode == 2 || blackBoard.LookOn_Mode == 3)
            {//左右绕圈
                //LoggerHelper.Error("blackBoard.LookOn_ModeInterval[blackBoard.LookOn_Mode]" + blackBoard.LookOn_ModeInterval[blackBoard.LookOn_Mode]);
                tmpLookOn_ActTime = MoveAround();
                tmpLookOn_ActTime = blackBoard.LookOn_ModeInterval[blackBoard.LookOn_Mode];
            }
            else if (blackBoard.LookOn_Mode == 4)
            {//休息
                float timeStanderd = blackBoard.LookOn_ModeInterval[blackBoard.LookOn_Mode];
                float time = Mogo.Util.RandomHelper.GetRandomFloat(timeStanderd * 0.8f, timeStanderd * 1.2f);
                ProcEnterRest((uint)(time * 1000.0f));
                tmpLookOn_ActTime = time * 1000.0f;
            }
            else if (blackBoard.LookOn_Mode == 5)
            {//冲锋
                tmpLookOn_ActTime = MoveAssault();
            }

            if (tmpLookOn_ActTime >= 0.0f)
            {
                blackBoard.LookOn_ActTime = (uint)(tmpLookOn_ActTime*1000);
                blackBoard.LookOn_Tick = GetTickCount();
            }
        }

        public virtual float MoveToWithOutNav()
        {
            blackBoard.movePoint.y = Transform.position.y;
            Vector3 n = (blackBoard.movePoint - Transform.position).normalized;
            float xie = Mathf.Sqrt(n.x * n.x + n.z * n.z);
            float hudu = Mathf.Asin(n.z / xie);
            float angle = hudu / Mathf.PI * 180.0f;
            if (blackBoard.movePoint.x > Transform.position.x && blackBoard.movePoint.z > Transform.position.z)
            {
                angle = 270 - angle;
            }
            else if (blackBoard.movePoint.x < Transform.position.x && blackBoard.movePoint.z > Transform.position.z)
            {
                angle = 90 + angle;
            }
            else if (blackBoard.movePoint.x < Transform.position.x && blackBoard.movePoint.z < Transform.position.z)
            {
                angle = 90 + angle;
            }
            else if (blackBoard.movePoint.x > Transform.position.x && Transform.position.z > blackBoard.movePoint.z)
            {
                angle = -90 - angle;
            }

            if (blackBoard.LookOn_Mode != 1)
            {
                angle -= 180;
            }
            else
            {
            }

            float time = 0.0f;
            float r = Vector3.Distance(blackBoard.movePoint, Transform.position);
            float len = r;
            time = len / (GetIntAttr("speed") * 0.01f * blackBoard.speedFactor);
            //float timeStanderd = blackBoard.LookOn_ModeInterval[blackBoard.LookOn_Mode];
            //float time = timeStanderd;// Mogo.Util.RandomHelper.GetRandomFloat(timeStanderd * 0.8f, timeStanderd * 1.2f);

            //motor.SetTargetToLookAt(enemy.Transform);//此时是没目标的，这个函数暂时只提供给怪物巡逻用
            MoveToByAngle(angle, time, true);

            return time;
        }

        public virtual bool IsPatrolCD() //{ Mogo.Util.LoggerHelper.Debug("par:IsPatrolCD"); return false; }
        {
            if (blackBoard.aiState == Mogo.AI.AIState.PATROL_CD_STATE)
                return true;
            else
                return false;
        }

        public virtual bool ProcPatrol(int CDTimeMin, int CDTimeMax)// { Mogo.Util.LoggerHelper.Debug("par:ProcPatrol"); return false; }
        {
            if (blackBoard.HatredCount() > 0)
                return false;

            if (IsPatrolCD())
            {//正在巡逻冷却中
                return false;
            }

            int patrolCDTime = Mogo.Util.RandomHelper.GetRandomInt(CDTimeMin, CDTimeMax);//毫秒
           
            //随机出一个blackBoard.movePoint   PatrolSquareRange
            float x = enterX * 0.01f;
            float z = enterZ * 0.01f;
            blackBoard.movePoint = new Vector3(x - Mogo.Game.AISpecialEnum.PatrolSquareRange +
                Mogo.Util.RandomHelper.GetRandomInt(Mogo.Game.AISpecialEnum.PatrolSquareRange * 2),
                Transform.position.y,
                z - Mogo.Game.AISpecialEnum.PatrolSquareRange +
                Mogo.Util.RandomHelper.GetRandomInt(Mogo.Game.AISpecialEnum.PatrolSquareRange * 2));

            //速度衰减
            blackBoard.speedFactor = AISpecialEnum.PATROL_SPEED_FACTOR;

            int moveUseTime = (int)(MoveToWithOutNav() * 1000);//毫秒
            motor.CancleLookAtTarget();

            int totleUseTime = moveUseTime + patrolCDTime;//毫秒

            //冷却totleUseTime时间
            if (blackBoard.timeoutId > 0)
                TimerHeap.DelTimer(blackBoard.timeoutId);

            blackBoard.ChangeState(Mogo.AI.AIState.PATROL_CD_STATE);

            //blackBoard.patrolActTick = GetTickCount();

            int tmpCDTime = totleUseTime + 100;
            if (tmpCDTime < 0)
                tmpCDTime = 100;//减去之后延迟不能为0
            blackBoard.patrolActTime = (uint)tmpCDTime;
            blackBoard.timeoutId = TimerHeap.AddTimer(blackBoard.patrolActTime, 0, ProcessAITimerEvent_PatrolCDEnd);//100毫秒用来延缓一点巡逻CD

            return true;
        }

        public virtual bool IsAngerFull() { Mogo.Util.LoggerHelper.Debug("par:IsAngerFull"); return false; }//怒气已满

        public virtual bool IsPowerFX() { Mogo.Util.LoggerHelper.Debug("par:IsPowerFX"); return false; }//是暴气状态

        public virtual bool PowerFX() { Mogo.Util.LoggerHelper.Debug("par:PowerFX"); return false; }//进入暴气状态
        

        public virtual bool NotTurn() { Mogo.Util.LoggerHelper.Debug("par:NotTurn"); return false; }
        public virtual void Think(AIEvent triggerEvent) { Mogo.Util.LoggerHelper.Debug("par:Think"); return; }
        public virtual bool ProcMercenaryAOI() { Mogo.Util.LoggerHelper.Debug("par:ProcMercenaryAOI"); return false; }
        public virtual bool ProcPVPAOI() { Mogo.Util.LoggerHelper.Debug("par:ProcPVPAOI"); return false; }
        public virtual bool ProcChooseCastPoint(int skillBagIndex) { Mogo.Util.LoggerHelper.Debug("par:ProcChooseCastPoint"); return false; }
        //public virtual void ProcMoveTo() { Mogo.Util.LoggerHelper.Debug("par:ProcMoveTo"); }
        //public virtual void ProcThink() { Mogo.Util.LoggerHelper.Debug("par:ProcThink"); }
        //public virtual void ProcEnterRest(uint sec) { Mogo.Util.LoggerHelper.Debug("par:ProcEnterRest"); }
        //public virtual void ProcEnterCD(int sec) { Mogo.Util.LoggerHelper.Debug("par:ProcEnterCD"); }
        public virtual bool ProcCastSpell(int skillBagIndex, int reversal) { Mogo.Util.LoggerHelper.Debug("par:ProcCastSpell"); return false; }
        //public virtual void ProcReinitLastCast() { Mogo.Util.LoggerHelper.Debug("par:ProcProcReinitLastCastLastCastIs"); }
        public virtual bool ProcInSkillRange(int skillBagIndex) { Mogo.Util.LoggerHelper.Debug("par:ProcInSkillRange"); return false; }
        public virtual bool ProcInSkillCoolDown(int skillBagIndex) { Mogo.Util.LoggerHelper.Debug("par:ProcInSkillCoolDown"); return false; }
        public virtual bool ProcLastCastIs(int skillBagIndex)
        {
            skillBagIndex--;
            return (blackBoard.lastCastIndex == skillBagIndex) ? true : false;
        }
        public virtual int GetEnemyNum() { Mogo.Util.LoggerHelper.Debug("par:GetEnemyNum"); return 0; }

        
        //public virtual void ProcEscape(uint sec) { Mogo.Util.LoggerHelper.Debug("par:ProcEscape"); return; }
        //public virtual void ProcLookOn() { Mogo.Util.LoggerHelper.Debug("par:ProcLookOn"); return; }
        public virtual int GetSkillUseCount(int skillId) { return blackBoard.skillUseCount[skillId]; }
        public virtual bool ProcLastLookOnModeIs(int lookOnMode)
        {
            return (blackBoard.LookOn_LastMode == lookOnMode) ? true : false;
        }
        public virtual bool ProcFollowOwner() { Mogo.Util.LoggerHelper.Debug("par:ProcFollowOwner"); return false; }
        public virtual bool ProcSelectAutoFightMovePoint() { Mogo.Util.LoggerHelper.Debug("par:ProcSelectAutoFightMovePoint"); return false; }
        
        
	}
}
