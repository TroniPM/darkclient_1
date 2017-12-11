using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mogo.FSM;
using Mogo.Util;
using Mogo.GameData;
using Mogo.AI;
using Mogo.AI.BT;
using Mogo.Game;

namespace Mogo.Game
{


    public partial class EntityMyself
    {
        //
        private bool m_bLockMove = false;
        public bool LockMove
        {
            get
            {
                return m_bLockMove;
            }
            set
            {
                //if (value != m_bLockMove)
                {//这里肯定是服务器下发
                    m_bLockMove = value;
                    //ShowText(2, "LockMove:" + LockMove);
                    //LoggerHelper.Debug("LockMove:" + m_bLockMove);

                }
            }
        }
        //
        public int m_iGuideProgress = 1;
        //
        public byte m_iFBProgress = 1;
        public byte FBProgress
        {
            get
            {
                return m_iFBProgress;
            }
            set
            {
                if (value != m_iFBProgress)
                {//这里肯定是服务器下发
                    m_iFBProgress = value;
                }
            }
        }
        //等待陷阱结束标志
        private int m_bWaitTrapEnd = 1;
        public int WaitTrapEnd
        {
            get { return m_bWaitTrapEnd; }
            set
            {
                //m_bWaitTrapEnd = value;
                if (value == 0)
                {
                    //Mogo.Util.LoggerHelper.Debug("m_bWaitTrapEnd = 0   0");
                    m_bWaitTrapEnd = 0;
                    if (LockMove)
                        return;
                    //Mogo.Util.LoggerHelper.Debug("m_bWaitTrapEnd = 0   1");
                    int totleCorners = MissionPathPointData.missionPathPoint.Length;
                    float minDis = 9999.0f;
                    int tarKey = 0;
                    for (int key = 1; key <= totleCorners; key++)
                    {
                        Vector2 tmpTarPoint = new Vector2(MissionPathPointData.missionPathPoint[key - 1].pathPoint[0],
                            MissionPathPointData.missionPathPoint[key - 1].pathPoint[2]);

                        float dis = Vector2.Distance(new Vector2(Transform.position.x, Transform.position.z), tmpTarPoint) - 0.1f;

                        if (dis <= minDis)
                        {
                            tarKey = key;
                            minDis = dis;
                        }
                    }
                    //Mogo.Util.LoggerHelper.Debug("m_bWaitTrapEnd = 0    2");


                    if (m_iFBProgress != tarKey)
                    {
                        m_iFBProgress = (byte)tarKey;
                    }

                    //开放此路点的关联路点
                    List<int> tmpdeleteList = MissionPathPointData.missionPathPoint[tarKey - 1].deleteList;
                    if (tmpdeleteList.Count > 0)
                        for (int i = 0; i < tmpdeleteList.Count; i++)
                        {
                            if (MissionPathPointData.missionPathPoint[tmpdeleteList[i] - 1].isEnable != 0)
                            {
                                MissionPathPointData.SetType(tmpdeleteList[i] - 1, 0);
                            }
                        }


                    MissionPathPointData.SetType(tarKey - 1, 1);
                    //Mogo.Util.LoggerHelper.Debug("111" + tarKey + " " + m_iGuideProgress + " " + totleCorners + " " + MissionPathPointData.missionPathPoint[tarKey - 1].isNormalType);
                    //如果这个点是传送点并且是当前m_iGuideProgress，那么要指向下一个
                    //MissionPathPointData.missionPathPoint[tarKey - 1].isNormalType == 1 &&
                    if (m_iGuideProgress == tarKey && m_iGuideProgress < totleCorners)
                    {
                        //Mogo.Util.LoggerHelper.Debug("222" + tarKey);
                        //int totleCorners = MissionPathPointData.missionPathPoint.Length;
                        for (int key = (MogoWorld.thePlayer.m_iGuideProgress + 1); key <= totleCorners; key++)
                        {
                            if (MissionPathPointData.missionPathPoint[key - 1].isPointer == 1)
                            {
                                //Mogo.Util.LoggerHelper.Debug("333" + tarKey);
                                MogoWorld.thePlayer.m_iGuideProgress = key;
                                break;
                            }
                        }
                        if (MogoWorld.thePlayer.m_iGuideProgress != 0)
                        {
                            //Mogo.Util.LoggerHelper.Debug("444" + tarKey);
                            MogoFXManager.Instance.UpdatePointerToTarget(new Vector3(
                                    MissionPathPointData.missionPathPoint[MogoWorld.thePlayer.m_iGuideProgress - 1].pathPoint[0],
                                    MogoWorld.thePlayer.Transform.position.y,
                                    MissionPathPointData.missionPathPoint[MogoWorld.thePlayer.m_iGuideProgress - 1].pathPoint[2]));
                            //检测指引消失/出现
                            EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
                        }
                        else
                        {
                            //找不到下一个pointer
                        }
                    }

                }
                else if (m_bWaitTrapEnd == 0 && value == 1)
                {
                    m_bWaitTrapEnd = 1;
                    LockMove = false;
                    //MissionPathPointData.SetType(ProgressPointIndex - 1, 1);
                    ProgressPointIndex = -1;
                    if (AutoFight == AutoFightState.RUNNING)
                        Think(AIEvent.AvatarPosSync);
                }

            }
        }

        private int m_iNormalAttackCastSkillId = 0;

        //ai根节点
        protected BehaviorTreeRoot m_aiRoot;

        //自动战斗开关
        private AutoFightState m_iAutoFightState = AutoFightState.IDLE;

        public AutoFightState AutoFight
        {

            get { return m_iAutoFightState; }
            set
            {
                if (m_iAutoFightState == value && value == AutoFightState.IDLE)
                {
                    return;
                }
                m_iAutoFightState = value;
                if (value != AutoFightState.RUNNING)
                {
                    LoggerHelper.Debug("AutoFight 1:" + m_iAutoFightState);
                    LockMove = false;
                    if (value == AutoFightState.IDLE)
                        StopMove();
                    //显示 自动战斗   自动战斗中Active false
                    if (MainUIViewManager.Instance)
                        MainUIViewManager.Instance.UpdateAutoFight(false);
                }
                else
                {
                    LockMove = false;
                    //显示 取消托管  自动战斗中Active true
                    ProgressPointIndex = -1;//解决中途手动然后再自动战斗往回走
                    if (MainUIViewManager.Instance)
                        MainUIViewManager.Instance.UpdateAutoFight(true);
                    //LoggerHelper.Debug("AutoFight 2:" + m_iAutoFightState);
                    Think(AIEvent.AvatarPosSync);
                }

            }
        }

        //进度路点index
        private int m_iProgressPointIndex = -1;

        public int ProgressPointIndex
        {
            get { return m_iProgressPointIndex; }
            set
            {
                m_iProgressPointIndex = value;
                //LoggerHelper.Debug("ProgressPointIndex 0:" + m_iProgressPointIndex);
            }
        }

        private void DummyThink()
        {
            //LoggerHelper.Error("dummy  AvatarPosSync");
            if (AutoFight != AutoFightState.RUNNING)
                return;


            Think(AIEvent.AvatarPosSync);
        }

        public void ProcCGBegin()
        {
            //LoggerHelper.Debug("ProcCGBegin 1");
            MogoWorld.CGing = true;
            //让小伙伴隐身
            EntityParent littleGuy = GetMercenaryEntity();
            if (littleGuy != null)
            {
                littleGuy.GameObject.SetActive(false);

            }

            //LoggerHelper.Debug("ProcCGBegin 2");
            //检测指引消失/出现
            EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
            //控制托管
            StopMove();
            if (AutoFight == AutoFightState.RUNNING)
            {
                //LoggerHelper.Debug("ProcCGBegin 3");
                //LoggerHelper.Debug("ProcCGBegin");
                AutoFight = AutoFightState.PAUSE;
            }
        }

        public void ProcCGEnd()
        {
            MogoWorld.CGing = false;
            //让小伙伴现身
            EntityParent littleGuy = GetMercenaryEntity();
            if (littleGuy != null)
            {
                littleGuy.GameObject.SetActive(true);
            }
            //检测指引消失/出现
            EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
            //控制托管
            if (AutoFight == AutoFightState.PAUSE)
            {
                //LoggerHelper.Debug("ProcCGEnd");
                AutoFight = AutoFightState.RUNNING;
            }
        }

        public void ProcTrapBegin(string gearType)
        {

            //Mogo.Util.LoggerHelper.Debug("*************************************  TrapBegin   " + Transform.position + " " + gearType);
            if (MogoWorld.canAutoFight)
            {
                if (gearType == "MobilePlatformBaseOnAvatar")
                {
                    StopMove();
                    return;
                }

                WaitTrapEnd = 0;

                if (gearType == "MobilePlatformBaseOnAvatar" || gearType == "TeleportPointSrc" || gearType == "Connon")
                {
                    ProcCGBegin();
                }


            }

        }

        public void ProcTrapEnd(string gearType)
        {
            //Mogo.Util.LoggerHelper.Debug("*************************************  TrapEnd" + " " + gearType);
            if (MogoWorld.canAutoFight)
            {
                if (gearType == "MobilePlatformBaseOnAvatar")
                {
                    LockMove = false;
                }
                WaitTrapEnd = 1;
                if (gearType == "MobilePlatformBaseOnAvatar" || gearType == "TeleportPointDes" || gearType == "Connon")
                {
                    ProcCGEnd();
                }
            }
        }

        private void ProcLiftEnter()
        {
            //Mogo.Util.LoggerHelper.Debug("*************************************  ProcLiftEnter");

            if (MogoWorld.canAutoFight)
            {
                //ProcCGBegin();
            }

        }

        private void OnBattleBtnPressed()
        {
            AutoFight = AutoFightState.IDLE;
        }


        private void PathPointTrigger(int triggerKey)
        {
            //检测是否此点已关闭，类型不是传送门

            //Mogo.Util.LoggerHelper.Debug("PathPointTrigger 0:" + (MissionPathPointData.missionPathPoint[triggerKey - 1].isEnable == 1) +
            //    (MissionPathPointData.missionPathPoint[triggerKey - 1].isNormalType == 1));
            if ((MissionPathPointData.missionPathPoint[triggerKey - 1].isEnable == 1) ||
                (MissionPathPointData.missionPathPoint[triggerKey - 1].isNormalType == 1))
                return;
            //Mogo.Util.LoggerHelper.Debug("PathPointTrigger 0:" + triggerKey + " " + (MissionPathPointData.missionPathPoint[triggerKey - 1].isEnable == 1) +
            //    (MissionPathPointData.missionPathPoint[triggerKey - 1].isNormalType == 1));
            //更新副本进度到服务器
            if (m_iFBProgress != triggerKey)
            {
                m_iFBProgress = (byte)triggerKey;
            }

            //开放此路点的关联路点
            List<int> tmpdeleteList = MissionPathPointData.missionPathPoint[triggerKey - 1].deleteList;
            if (tmpdeleteList.Count > 0)
                for (int i = 0; i < tmpdeleteList.Count; i++)
                {
                    if (MissionPathPointData.missionPathPoint[tmpdeleteList[i] - 1].isEnable != 0)
                    {
                        MissionPathPointData.SetType(tmpdeleteList[i] - 1, 0);
                    }
                }

            //此点设置为关闭
            //Mogo.Util.LoggerHelper.Debug("PathPointTrigger 2.1" + m_iGuideProgress + triggerKey);
            MissionPathPointData.SetType(triggerKey - 1, 1);
            //ProgressPointIndex = triggerKey;//10105bug
            //如果这个点是传送点并且是当前m_iGuideProgress，那么要指向下一个
            int totleCorners = MissionPathPointData.missionPathPoint.Length;
            if (m_iGuideProgress == triggerKey && m_iGuideProgress <= totleCorners)
            {
                //Mogo.Util.LoggerHelper.Debug("333:" + triggerKey + " " + m_iGuideProgress + " " + totleCorners);
                if (m_iGuideProgress == totleCorners)
                { //踩到是最后一个corner，让指针消失,一般最后一个点都不会是传送门，所以最后点只写在这里了
                    MogoWorld.touchLastPathPoint = true;
                    //检测指引消失/出现
                    EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
                }

                else
                {
                    for (int key = (MogoWorld.thePlayer.m_iGuideProgress + 1); key <= totleCorners; key++)
                    {
                        if (MissionPathPointData.missionPathPoint[key - 1].isPointer == 1)
                        {
                            MogoWorld.thePlayer.m_iGuideProgress = key;
                            break;
                        }
                    }
                    if (MogoWorld.thePlayer.m_iGuideProgress != 0)
                    {
                        MogoFXManager.Instance.UpdatePointerToTarget(new Vector3(
                                MissionPathPointData.missionPathPoint[MogoWorld.thePlayer.m_iGuideProgress - 1].pathPoint[0],
                                MogoWorld.thePlayer.Transform.position.y,
                                MissionPathPointData.missionPathPoint[MogoWorld.thePlayer.m_iGuideProgress - 1].pathPoint[2]));
                        //检测指引消失/出现
                        EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);
                    }
                    else
                    {
                        //找不到下一个pointer
                    }
                }
            }

            if (AutoFight == AutoFightState.RUNNING)
            {
                if (MissionPathPointData.missionPathPoint[triggerKey - 1].isNormalType == 2)
                {

                    //LoggerHelper.Error("22222222222222222222222222222222222222222222");
                    //踩在电梯点,走到电梯延伸点


                    Move();
                    (motor as MogoMotorMyself).MoveToWithoutNav(new Vector3(
                            MissionPathPointData.missionPathPoint[triggerKey - 1].movePosition[0],
                            MissionPathPointData.missionPathPoint[triggerKey - 1].movePosition[1],
                            MissionPathPointData.missionPathPoint[triggerKey - 1].movePosition[2]
                        ));
                    LockMove = true;

                    //DummyThink();
                }
            }

        }

        private void DirActive()
        {
            if (MogoWorld.CGing || MogoWorld.hasMonster || MogoWorld.touchLastPathPoint || MogoWorld.thePlayer.isInCity || !MogoWorld.canAutoFight || MogoWorld.connoning)
            {
                //LoggerHelper.Debug("Dir 1:" + MogoWorld.CGing + " " + MogoWorld.hasMonster + " " + MogoWorld.touchLastPathPoint + " " + MogoWorld.thePlayer.isInCity + " " + !MogoWorld.canAutoFight + " " + MogoWorld.connoning);
                MogoFXManager.Instance.DisatblePointerToTarget();
            }
            else
            {
                //LoggerHelper.Debug("Dir 2:" + MogoWorld.CGing + " " + MogoWorld.hasMonster + " " + MogoWorld.touchLastPathPoint + " " + MogoWorld.thePlayer.isInCity + " " + !MogoWorld.canAutoFight + " " + MogoWorld.connoning);
                MogoFXManager.Instance.AblePointerToTarget();
            }
        }

        //AI相关
        private void ThinkBefore()
        {
            blackBoard.skillActTime = 0;
            blackBoard.skillActTick = 0;

            if (blackBoard.movePoint != null)
            {
                StopMove();
            }
        }

        public int CanThink()
        {
            int error = 0;
            //if (GetTickCount() - LastThinkTick <= 500)//自动战斗普攻连续打
            //{ error = 15; return error; }
            if (curHp <= 0)
            { error = 1; return error; }
            if (AutoFight != AutoFightState.RUNNING)
            { error = 11; return error; }
            if (m_aiRoot == null)
            { error = 12; return error; }
            if (LockMove == true)
            { error = 13; return error; }
            if (WaitTrapEnd == 0)
            { error = 14; return error; }


            return error;
        }

        public override void Think(AIEvent triggerEvent)
        {
            //LoggerHelper.Debug("Think");

            blackBoard.ChangeEvent(triggerEvent);

            int canThinkError = CanThink();
            if (canThinkError != 0)
            {
                //Mogo.Util.LoggerHelper.Debug("canThinkError:" + canThinkError + " AIEvent" + triggerEvent + " ID:" + ID);
                return;
            }

            //LastThinkTick = GetTickCount();

            //Mogo.Util.LoggerHelper.Debug("aiId:" + m_iAiId + "  event:" + triggerEvent);
            //m_aiRoot = AIContainer.container.Get((uint)30002);//kevintest
            ThinkBefore();
            m_aiRoot.Proc(this);
            ThinkAfter();
        }

        private void ThinkAfter()
        {

        }

        public override bool ProcAOI(int searchChance)
        {
            Container oldEnemyTrap = blackBoard.enemyTrap;


            blackBoard.enemyId = 0;
            blackBoard.enemyTrap = null;


            //遍历entities 
            float minDis = 50.0f;//和自己的距离小于minDis米
            foreach (KeyValuePair<uint, Mogo.Game.EntityParent> pair in MogoWorld.Entities)
            {
                EntityParent entity = pair.Value;

                if (entity.Transform != null && entity.m_factionFlag != m_factionFlag && entity.curHp > 0 && !(entity is EntityPlayer))// && !(entity is EntityMonster)
                {
                    float entityDistance = Vector3.Distance(entity.Transform.position, Transform.position);
                    if ((!(entity is EntityMonster) && entityDistance < minDis) || (entity is EntityMonster && entityDistance < 13.0f))
                    {
                        blackBoard.enemyId = entity.ID;
                        minDis = entityDistance;
                        Transform.LookAt(new Vector3(entity.Transform.position.x, Transform.position.y, entity.Transform.position.z));
                    }

                }
            }
            if (blackBoard.enemyId <= 0)
            {
                //找罐子，有就跑过去
                minDis = 5.0f;//和自己的距离小于minDis米
                // 为去除警告暂时屏蔽以下代码
                //bool oldEnemyTrapNotDie = false;
                foreach (KeyValuePair<Transform, Container> pair in Container.containers)
                {
                    /*
                    if (oldEnemyTrap != null && oldEnemyTrap.transform == pair.Key)
                    {//目标还没被打烂

                        oldEnemyTrapNotDie = true;
                        blackBoard.enemyTrap = oldEnemyTrap;
                        LoggerHelper.Error("old hit trap:" + blackBoard.enemyTrap.transform.position);
                        return true;
                    }
                     */
                    if (pair.Value.triggleEnable && (pair.Value is Crock || pair.Value is Chest))
                    {
                        //LoggerHelper.Debug("(((((((((((((( " + pair.Value.gameObject.name);

                        if (pair.Value.stateOne)
                        {
                            //可打可不打，看距离
                            float entityDistance = Vector3.Distance(pair.Value.transform.position, Transform.position);
                            float entityDistanceY = Math.Abs(pair.Value.transform.position.y - Transform.position.y);
                            if (entityDistance < minDis && entityDistanceY < 0.5f)//战士普攻不够5米，跑过去打，远程职业5米能打中
                            {
                                minDis = entityDistance;
                                blackBoard.enemyTrap = pair.Value;
                                Transform.LookAt(new Vector3(pair.Value.transform.position.x, Transform.position.y, pair.Value.transform.position.z));
                            }

                        }
                        else
                        {
                            //一定要打

                            float entityDistance = Vector3.Distance(pair.Value.transform.position, Transform.position);
                            float entityDistanceY = Math.Abs(pair.Value.transform.position.y - Transform.position.y);
                            if (entityDistance < minDis && entityDistanceY < 0.5f)
                            {
                                minDis = entityDistance;
                                blackBoard.enemyTrap = pair.Value;
                                Transform.LookAt(new Vector3(pair.Value.transform.position.x, Transform.position.y, pair.Value.transform.position.z));
                            }
                        }
                    }
                }


                if (blackBoard.enemyTrap != null)
                {
                    //LoggerHelper.Error("want hit trap:" + blackBoard.enemyTrap.transform.position);
                    return true;
                }
                else
                {
                    //LoggerHelper.Error("no hit trap");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public override bool ProcInSkillCoolDown(int skillBagIndex)
        {
            skillBagIndex--;
            int skillid = 0;
            switch (skillBagIndex)
            {
                case 0:
                    {
                        return true;//默认普攻没冷却
                    }
                //为消除警告而注释以下代码
                    //break;
                case 4:
                    {
                        skillid = (skillManager as PlayerSkillManager).GetSpellOneID();
                    }
                    break;
                case 5:
                    {
                        skillid = (skillManager as PlayerSkillManager).GetSpellTwoID();
                    }
                    break;
                case 6:
                    {
                        skillid = (skillManager as PlayerSkillManager).GetSpellThreeID();
                    }
                    break;
            }

            if ((skillManager as PlayerSkillManager).IsSkillCooldown(skillid))
            {
                //LoggerHelper.Debug("nSkillCoolDown:" + skillBagIndex + " " + skillid + " false");
                return false;
            }
            else
            {
                //LoggerHelper.Debug("nSkillCoolDown:" + skillBagIndex + " " + skillid + " true");
                return true;
            }
        }

        //        int debugCount = 0;
        public override bool ProcCastSpell(int skillBagIndex, int reversal)
        {


            skillBagIndex--;
            switch (skillBagIndex)
            {
                case 0:
                    {
                        //ShowText(2, "ProcCastSpell:" + skillBagIndex + "  " + debugCount++);

                        ((PlayerBattleManager)battleManger).NormalAttack();
                    }
                    break;
                case 4:
                    {
                        //LoggerHelper.Debug("SpellOneAttack:" + skillBagIndex);
                        ((PlayerBattleManager)battleManger).SpellOneAttack();
                    }
                    break;
                case 5:
                    {
                        //LoggerHelper.Debug("SpellTwoAttack:" + skillBagIndex);
                        ((PlayerBattleManager)battleManger).SpellTwoAttack();
                    }
                    break;
                case 6:
                    {
                        //LoggerHelper.Debug("SpellThreeAttack:" + skillBagIndex);
                        ((PlayerBattleManager)battleManger).SpellThreeAttack();
                    }
                    break;
            }

            return true;
        }

        public override void ProcEnterCD(int sec)
        {
            if (blackBoard.timeoutId > 0)
                TimerHeap.DelTimer(blackBoard.timeoutId);

            blackBoard.ChangeState(Mogo.AI.AIState.CD_STATE);
            int tmpCDTime = sec + 100;
            if (tmpCDTime < 0)
                tmpCDTime = 100;//减去之后延迟不能为0
            blackBoard.timeoutId = TimerHeap.AddTimer((uint)tmpCDTime, 0, ProcessAITimerEvent_CDEnd);//100毫秒用来延缓一点技能CD

        }

        public override bool ProcChooseCastPoint(int skillBagIndex)
        {
            skillBagIndex--;
            EntityParent theTarget = GetTargetEntity();
            Container enemyTrap = GetTargetContainer();
            if (theTarget == null && enemyTrap == null)
                return false;

            int skillId = 0;
            switch (skillBagIndex)
            {
                case 0:
                    {
                        skillId = m_iNormalAttackCastSkillId;
                    }
                    break;
                case 4:
                    {
                        skillId = (skillManager as PlayerSkillManager).GetSpellOneID();
                    }
                    break;
                case 5:
                    {
                        skillId = (skillManager as PlayerSkillManager).GetSpellTwoID();
                    }
                    break;
                case 6:
                    {
                        skillId = (skillManager as PlayerSkillManager).GetSpellThreeID();
                    }
                    break;
            }

            int castRange = GetSkillRange(skillId);


            return GetMovePointStraight((int)(castRange * 0.8f));

        }



        public override bool ProcInSkillRange(int skillBagIndex)
        {

            skillBagIndex--;
            if (blackBoard.enemyId == 0 && GetTargetContainer() == null)
                return false;

            int skillId = 0;
            switch (skillBagIndex)
            {
                case 0:
                    {
                        skillId = m_iNormalAttackCastSkillId;
                    }
                    break;
                case 4:
                    {
                        skillId = (skillManager as PlayerSkillManager).GetSpellOneID();
                    }
                    break;
                case 5:
                    {
                        skillId = (skillManager as PlayerSkillManager).GetSpellTwoID();
                    }
                    break;
                case 6:
                    {
                        skillId = (skillManager as PlayerSkillManager).GetSpellThreeID();
                    }
                    break;
            }


            var enemy = GetTargetEntity();
            Container enemyTrap = GetTargetContainer();
            if (enemy == null && enemyTrap == null)
                return false;



            int skillRange = GetSkillRange(skillId);


            int distance = skillRange;
            float entityRadius = (enemy != null) ? enemy.MonsterData.scaleRadius : 100;
            if (entityRadius <= 0.1f)
                entityRadius = (float)enemy.GetIntAttr("scaleRadius");

            int curdistance = (int)(Vector3.Distance(Transform.position, (enemy != null) ? enemy.Transform.position : enemyTrap.transform.position) * 100 - entityRadius);

            bool rnt = false;

            if (distance >= curdistance)
            {
                //看着对手
                Transform.LookAt(new Vector3((enemy != null) ? enemy.Transform.position.x : enemyTrap.transform.position.x,
                    Transform.position.y,
                    (enemy != null) ? enemy.Transform.position.z : enemyTrap.transform.position.z));
                rnt = true;
                //Mogo.Util.LoggerHelper.Debug("IsInSkillRange:true");
            }
            else
            {
                rnt = false;
                //Mogo.Util.LoggerHelper.Debug("IsInSkillRange:false");
            }

            //LoggerHelper.Debug("inskillrange:" + skillBagIndex + " " + rnt + "    " + distance + " vs " + curdistance + " :" + (distance >= curdistance));

            return rnt;
        }

        //private int m_debugCount = 0;
        public override void ProcMoveTo()
        {
            motor.SetStopDistance((float)blackBoard.navTargetDistance * 0.01f);
            Move();
            motor.MoveTo(blackBoard.movePoint, true);
        }

        public int debugcount = 0;
        public override bool ProcSelectAutoFightMovePoint()
        {
            //选择一个托管路点
            //应该是当前点之后的点才需要轮询           
            //选择最近的点

            //Mogo.Util.LoggerHelper.Error("ProcSelectAutoFightMovePoint " + debugcount++);
            int totleCorners = MissionPathPointData.missionPathPoint.Length;
            float minDis = 9999.0f;
            Vector3 tarPoint = new Vector3(0, 0, 0);
            bool hasGetPoint = false;

            if (ProgressPointIndex != -1)
            {
                for (int key = ProgressPointIndex; key <= totleCorners; key++)
                {
                    if (MissionPathPointData.missionPathPoint[key - 1].isEnable == 0)
                    {
                        ProgressPointIndex = key;
                        break;
                    }
                }
            }

            if (ProgressPointIndex == -1)
            {
                //LoggerHelper.Debug("go new:");
                //Mogo.Util.LoggerHelper.Debug("1" + debugcount++);
                for (int key = 1; key <= totleCorners; key++)
                {
                    //if (MissionPathPointData.missionPathPoint[key - 1].isEnable != 0)
                    //    continue;

                    Vector2 tmpTarPoint = new Vector2(MissionPathPointData.missionPathPoint[key - 1].pathPoint[0],
                        MissionPathPointData.missionPathPoint[key - 1].pathPoint[2]);

                    if (MissionPathPointData.missionPathPoint[key - 1].isNormalType == 2)
                    {//是电梯 
                        //Mogo.Util.LoggerHelper.Debug("2 " + debugcount++);
                        if (MissionPathPointTriggerGear.CheckIfAvatarInMissionTriggerGear(key))
                        {
                            //Mogo.Util.LoggerHelper.Debug("3 " + debugcount++);
                            //踩在电梯点,走到电梯延伸点
                            Move();
                            (motor as MogoMotorMyself).MoveToWithoutNav(new Vector3(
                                MissionPathPointData.missionPathPoint[key - 1].movePosition[0],
                                MissionPathPointData.missionPathPoint[key - 1].movePosition[1],
                                MissionPathPointData.missionPathPoint[key - 1].movePosition[2]
                                ));


                            LockMove = true;
                            return true;
                        }
                    }
                    else if (MissionPathPointData.missionPathPoint[key - 1].isNormalType == 0)
                    {
                        float dis = Vector2.Distance(new Vector2(Transform.position.x, Transform.position.z), tmpTarPoint) - MissionPathPointData.missionPathPoint[key - 1].range;
                        if (dis <= 0.0f)
                        {
                            //在点的范围内
                            if (MissionPathPointData.missionPathPoint[key - 1].isEnable == 0)
                            {
                                //走去下一个点
                                ProgressPointIndex = key + 1;
                            }
                            break;
                        }
                    }
                }

                if (ProgressPointIndex > 0)
                {
                    //Mogo.Util.LoggerHelper.Debug("111 3");
                    hasGetPoint = true;
                }
                else
                {
                    //Mogo.Util.LoggerHelper.Debug("111 4");
                    //找最近点
                    for (int key = 1; key <= totleCorners; key++)
                    {
                        if (MissionPathPointData.missionPathPoint[key - 1].isEnable != 0)
                            continue;

                        int tmpkey = key;

                        Vector2 tmpTarPoint = new Vector2(MissionPathPointData.missionPathPoint[tmpkey - 1].pathPoint[0],
                            MissionPathPointData.missionPathPoint[tmpkey - 1].pathPoint[2]);

                        float dis = Vector2.Distance(new Vector2(Transform.position.x, Transform.position.z), tmpTarPoint) - MissionPathPointData.missionPathPoint[tmpkey - 1].range;

                        if (dis < minDis)
                        {
                            //Mogo.Util.LoggerHelper.Debug("111 5");
                            //tarPoint = new Vector3(tmpTarPoint.x, Transform.position.y, tmpTarPoint.y);
                            minDis = dis;
                            ProgressPointIndex = tmpkey;
                            hasGetPoint = true;
                        }
                    }
                }
                //Mogo.Util.LoggerHelper.Debug("111 6");
                //找到点的话，找它的最根点
                if (hasGetPoint)
                {
                    //Mogo.Util.LoggerHelper.Debug("111 7");
                    //LoggerHelper.Debug("******************:" + ProgressPointIndex +  "  priid:" + MissionPathPointData.missionPathPoint[ProgressPointIndex - 1].preID +  " count:" + debugcount++);

                    while (MissionPathPointData.missionPathPoint[ProgressPointIndex - 1].preID > 0)
                    {
                        ProgressPointIndex = MissionPathPointData.missionPathPoint[ProgressPointIndex - 1].preID;
                    }
                    //LoggerHelper.Debug("------------------:" + ProgressPointIndex + " count:" + debugcount++);
                    tarPoint = new Vector3(MissionPathPointData.missionPathPoint[ProgressPointIndex - 1].pathPoint[0],
                                        Transform.position.y,
                                        MissionPathPointData.missionPathPoint[ProgressPointIndex - 1].pathPoint[2]);
                }


            }
            else
            {
                //LoggerHelper.Error("old:" + ProgressPointIndex);
                int key = ProgressPointIndex;
                Vector2 tmpTarPoint = new Vector3(MissionPathPointData.missionPathPoint[key - 1].pathPoint[0], MissionPathPointData.missionPathPoint[key - 1].pathPoint[2]);

                //LoggerHelper.Error("else 0:" + key);
                float dis = Vector2.Distance(new Vector2(Transform.position.x, Transform.position.z), tmpTarPoint);
                dis -= 0.1f;
                if (dis <= 0.0f)
                {//在点的范围内,寻找下个路点
                    if (MissionPathPointData.missionPathPoint[key - 1].isNormalType == 2)
                    {
                        //LoggerHelper.Error("else 1:" + key);
                        //踩在电梯点,走到电梯延伸点
                        Move();
                        (motor as MogoMotorMyself).MoveToWithoutNav(new Vector3(
                                MissionPathPointData.missionPathPoint[key - 1].movePosition[0],
                                MissionPathPointData.missionPathPoint[key - 1].movePosition[1],
                                MissionPathPointData.missionPathPoint[key - 1].movePosition[2]
                            ));
                        LockMove = true;
                        return true;
                    }
                    else if (MissionPathPointData.missionPathPoint[key - 1].isNormalType == 1)
                    {
                        //走到传送点，等外部传送
                        //ShowText(2, "telport:" + key);
                        return true;
                    }
                    else
                    {
                        //LoggerHelper.Error("else 2:" + key);
                        key = ProgressPointIndex + 1;
                        if (key <= totleCorners)
                        {
                            tmpTarPoint = new Vector2(MissionPathPointData.missionPathPoint[key - 1].pathPoint[0], MissionPathPointData.missionPathPoint[key - 1].pathPoint[2]);

                            tarPoint = new Vector3(tmpTarPoint.x, Transform.position.y, tmpTarPoint.y);
                            ProgressPointIndex = key;
                            hasGetPoint = true;
                        }
                    }
                }
                else
                {//继续走此路点   
                    // LoggerHelper.Debug("go on:" + ProgressPointIndex);
                    tarPoint = new Vector3(tmpTarPoint.x, Transform.position.y, tmpTarPoint.y);
                    hasGetPoint = true;
                }

            }
            //Mogo.Util.LoggerHelper.Debug("111 8 ");
            if (hasGetPoint)
            {
                //Mogo.Util.LoggerHelper.Debug("111 9");
                //ShowText(2, "ProgressIndex:" + ProgressPointIndex);
                //LoggerHelper.Debug("tarPoint:" + tarPoint + " ProgressIndex:" + ProgressPointIndex);
                blackBoard.movePoint = tarPoint;
                blackBoard.navTargetDistance = 0;//越接近越好
                ProcMoveTo();
            }
            //else
            //    AutoFight = AutoFightState.IDLE;



            return hasGetPoint;
        }

        public bool ProcMoveToDrop()
        {
            //雇佣兵找敌人
            //遍历entities 
            EntityParent dropEntity;
            EntityParent tarDropEntity = null;
            float minDis = 15.0f;//和自己的距离小于minDis米
            foreach (KeyValuePair<uint, Mogo.Game.EntityParent> pair in MogoWorld.Entities)
            {
                dropEntity = pair.Value;

                if (dropEntity.Transform != null && dropEntity is EntityDrop)
                {
                    float entityDistance = Vector2.Distance(new Vector2(dropEntity.Transform.position.x, dropEntity.Transform.position.z), new Vector2(Transform.position.x, Transform.position.z)) - 0.5f;
                    if (entityDistance < minDis && entityDistance > 0.0f)//踩在目标点周围0.3米内不算，因为认为已经踩到了,忽略它
                    {
                        minDis = entityDistance;
                        tarDropEntity = dropEntity;
                        //Transform.LookAt(new Vector3(entity.Transform.position.x, Transform.position.y, entity.Transform.position.z));
                    }
                }
            }

            if (tarDropEntity != null)
            {

                blackBoard.movePoint = tarDropEntity.Transform.position;
                blackBoard.navTargetDistance = 0;//越接近越好
                ProcMoveTo();
                return true;
            }
            else
                return false;
        }

        public void ProcessAutoFightDrinkRedTea()
        {
            if (curHp <= 0)
                return;
            if (AutoFight != AutoFightState.RUNNING)
                return;
            if (hpCount <= 0)
                return;
            if (MogoWorld.thePlayer.sceneId == 30001 || (MogoWorld.thePlayer.sceneId >= 40001 && MogoWorld.thePlayer.sceneId <= 40010))
                return;

            float percentHp = (float)curHp / hp;

            if (percentHp <= 0.25f)
            {
                MogoWorld.thePlayer.RpcCall("CliEntityActionReq", ID, 2, curHp, 0);
                if (MogoWorld.IsClientMission)
                {
                    buffManager.ClientAddBuff(1);
                    UseHpBottleResp(0);
                    hpCount--;
                    return;
                }
                this.RpcCall("UseHpBottleReq", 1);
            }
        }

        override protected void OnMoveToFalse(GameObject param1, Vector3 param2, float param3)
        {
            //ShowText(2, "OnMoveToFalse");
            //LoggerHelper.Debug("OnMoveToFalse");
            if (param1.transform == GameObject.transform)
            {
                AutoFight = AutoFightState.IDLE;
            }
        }

        public void UpdateAutoFightInfo()
        {
            if (MogoWorld.canAutoFight)
            {
                int weaponType = GetEquipSubType();
                m_iNormalAttackCastSkillId = 0;
                if (weaponType != 0)
                {
                    int normalOneSkillId = (this.skillManager as PlayerSkillManager).GetNormalOne();
                    int reflectNormalComboSkillId = SkillIdReflectData.GetReflectSkillId(normalOneSkillId);
                    m_iNormalAttackCastSkillId = reflectNormalComboSkillId;
                    //LoggerHelper.Error("normalOneSkillId" + normalOneSkillId + " reflectNormalComboSkillId:" + m_iNormalAttackCastSkillId);
                    switch (vocation)
                    {
                        case Vocation.Warrior:
                            m_iAiId = 10001;
                            m_aiRoot = AIContainer.container.Get((uint)m_iAiId);
                            //m_iNormalAttackCastSkillId = (((int)WeaponSubType.CLAYMORE) == weaponType) ? 2001 : 2007;
                            break;
                        case Vocation.Assassin:
                            m_iAiId = 10002;
                            m_aiRoot = AIContainer.container.Get((uint)m_iAiId);
                            //m_iNormalAttackCastSkillId = (((int)WeaponSubType.SICKLE) == weaponType) ? 2057 : 2051;
                            break;
                        case Vocation.Archer:
                            m_iAiId = 10003;
                            m_aiRoot = AIContainer.container.Get((uint)m_iAiId);
                            //m_iNormalAttackCastSkillId = (((int)WeaponSubType.BOW) == weaponType) ? 2101 : 2107;
                            break;
                        case Vocation.Mage:
                            m_iAiId = 10004;
                            m_aiRoot = AIContainer.container.Get((uint)m_iAiId);
                            //m_iNormalAttackCastSkillId = (((int)WeaponSubType.STAFF) == weaponType) ? 2151 : 2157;
                            break;
                        default:
                            m_aiRoot = null;
                            //m_iNormalAttackCastSkillId = 0;
                            break;
                    }
                }
            }
        }

        override public bool IsAngerFull()
        {//怒气已满
            if (Anger >= 200 && (skillManager as PlayerSkillManager).isAnger == false)
                return true;
            else
                return false;
        }

        override public bool IsPowerFX()
        {//是暴气状态
            if ((skillManager as PlayerSkillManager).isAnger == true)
                return true;
            else
                return false;
        }

        override public bool PowerFX()
        {//进入暴气状态
            EnterAngerSt();
            return true;
        }

        
    }

}
