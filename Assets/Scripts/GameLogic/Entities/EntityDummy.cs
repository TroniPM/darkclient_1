/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityDummy
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-5-20
// 模块描述：纯客户端怪物
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Mogo.FSM;
using Mogo.Util;
using Mogo.GameData;
using Mogo.AI;
using Mogo.AI.BT;
using Mogo.Game;

using System;
using System.Reflection;
using Mogo.Mission;

namespace Mogo.Game
{
    public class EntityDummy : EntityParent
    {
        private byte m_deathFlag;

        private int m_clientTrapId;

        private GolemAnimation golem;
        private GolemFx golemFx;



        private bool m_borned = false;

        private bool m_bModelBuilded = false;

        private UInt32 m_monsterId;

        private uint fadeTimer = uint.MaxValue;
        private float fadeInTime = 0.3f;
        private float fadeOutTime = 0.3f;
        private uint beginFadeOutTimeForward = 500;
        // 为去除警告暂时屏蔽以下代码
        //#region DEBUG
        //private int m_debugCount = 0;
        //private int m_debugCount2 = 0;
        //#endregion


        private bool isGolem = false;
        public bool IsGolem
        {
            get { return isGolem; }
            protected set { isGolem = value; }
        }

        public byte deathFlag
        {
            get { return m_deathFlag; }
            set
            {

                if (m_deathFlag != value && value == 1)
                {//死亡处理
                    m_borned = false;//怪物死了没关ai
                    if (blackBoard.timeoutId > 0)
                        TimerHeap.DelTimer(blackBoard.timeoutId);
                    //if (m_clientTrapId != 0)
                    //{
                    //    EventDispatcher.TriggerEvent(Events.GearEvent.SetGearEnable, (uint)m_clientTrapId);
                    //}
                    OnDeath(-1);

                    if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 2)
                        MainUIViewManager.Instance.ShowBossTarget(false);
                }
                m_deathFlag = value;
            }
        }

        public int clientTrapId
        {
            get { return m_clientTrapId; }
            set { m_clientTrapId = value; }
        }

        public override ulong stateFlag
        {
            get
            {
                return m_stateFlag;
            }
            set
            {
                base.stateFlag = value;

                byte f = (byte)Utils.BitTest(m_stateFlag, StateCfg.DEATH_STATE);
                //byte f = (byte)(m_stateFlag & 1);
                if (f != deathFlag)
                {
                    deathFlag = f;
                }
            }
        }



        public override int hitStateID
        {
            get
            {
                return (int)m_monsterId;
            }
        }

        protected BehaviorTreeRoot m_aiRoot;

        public List<int> HitShader
        {
            get
            {
                return m_monsterData.hitShader;
            }
        }

        public int ShowHitAct
        {
            get
            {
                return m_monsterData.showHitAct;
            }
        }

        public override string HeadIcon
        {
            get
            {
                if (m_monsterId > 0)
                {
                    if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                        return IconData.dataMap.Get(m_monsterData.hpShow[0]).path;
                    else
                        return base.HeadIcon;
                }
                else
                    return base.HeadIcon;
            }
        }

        public override string name
        {
            get
            {
                if (m_monsterId > 0)
                {
                    if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                        return LanguageData.GetContent(m_monsterData.hpShow[1]);
                    else
                        return base.name;
                }
                else
                    return base.name;
            }
            set
            {
                base.name = value;
            }
        }

        public EntityDummy()
        {

        }

        public override uint curHp
        {
            get
            {
                return base.curHp;
            }
            set
            {

                if (value != 0 && base.curHp != 0 && base.curHp != value)
                {
                    // 居然没人调用过Entity身上的OnHit
                    // OnHit();

                    if (IsGolem)
                    {
                        golem.Hitting();
                        if (golemFx != null)
                            golemFx.Hitting();
                    }
                }

                base.curHp = value;

                if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                    EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnFlushBossBlood, this as EntityParent, (int)base.curHp);



                //EventDispatcher.TriggerEvent<float, uint>(BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, base.PercentageHp, ID);
                if (BillboardViewManager.Instance)
                    BillboardViewManager.Instance.SetBillboardBlood(base.PercentageHp, ID);
            }
        }

        public override bool NotTurn()
        {
            if (m_monsterData != null && m_monsterData.notTurn == 1)
                return true;
            else
                return false;
        }

        public void RunAI()
        {
            TimerHeap.DelTimer(blackBoard.timeoutId);
            blackBoard.timeoutId = TimerHeap.AddTimer(100, 0, _RunAI);
        }

        public void WaitRunAI(uint millsec)
        {
            TimerHeap.DelTimer(blackBoard.timeoutId);
            blackBoard.timeoutId = TimerHeap.AddTimer(millsec, 0, _RunAI);
        }

        public void _RunAI()
        {
            //Mogo.Util.LoggerHelper.Debug("_RunAI() 1");
            //测试行为树流程
            //if (blackBoard.isDead) return;
            m_aiRoot.Proc(this);
        }

        public override void CastSkill(int nSkillID)
        {
            (battleManger as DummyBattleManager).SetCoolDown(nSkillID);
            base.CastSkill(nSkillID);

        }

        public void ReadyCreateModel()
        {//延时产生
            //Mogo.Util.LoggerHelper.Debug("ReadyCreateModel");

            TimerHeap.AddTimer((uint)Mogo.Util.RandomHelper.GetRandomInt(0, 1500), 0, CreateModel);

        }

        public override void CreateModel()
        {
            if (MogoWorld.inCity)//城里不允许创建
                return;

            if (clientTrapId == 0)
            {
                CreateActualModel();

                m_factionFlag = 0;

                //m_stateFlag = Utils.BitSet(m_stateFlag, StateCfg.NO_HIT_STATE);


            }
            else
            {
                IsGolem = true;
                AttachBuildingModel();
            }
        }

        private void AttachBuildingModel()
        {
            GearParent[] gears = (GearParent[])GameObject.FindObjectsOfType(typeof(GearParent));
            foreach (GearParent gear in gears)
            {
                if (gear.ID == (uint)clientTrapId)
                {
                    Transform = gear.transform;
                    GameObject = gear.gameObject;
                    Transform.tag = "Monster";

                    LoggerHelper.Debug("CreateBuildingModel Position: " + Transform.position);

                    // motor = GameObject.AddComponent<MogoMotorServer>();
                    animator = GameObject.GetComponent<Animator>();

                    ActorDummy ap = GameObject.GetComponent<ActorDummy>();
                    if (ap == null)
                        ap = GameObject.AddComponent<ActorDummy>();

                    ap.theEntity = this;
                    this.Actor = ap;

                    golem = gear.gameObject.GetComponentInChildren<GolemAnimation>();
                    golemFx = gear.gameObject.GetComponentInChildren<GolemFx>();

                    // golem = gear as GolemAnimation;

                    golem.Activate();
                    if (golemFx != null)
                        golemFx.Activate();

                    AddBillBoard();

                    // base.CreateModel();
                }
            }
        }

        public override void CreateActualModel()
        {
            AvatarModelData data = AvatarModelData.dataMap.GetValueOrDefault(m_monsterData.model, null);
            if (data == null)
            {
                LoggerHelper.Error("Model not found: " + m_monsterData.model);
                return;
            }
            LoggerHelper.Debug("monster create:" + ID + ",name:" + data.prefabName);
            SubAssetCacheMgr.GetCharacterInstance(data.prefabName,
                (prefab, guid, gameObject) =>
                {
                    if (this.Actor)
                        this.Actor.Release();
                    if (Transform)
                        AssetCacheMgr.ReleaseLocalInstance(Transform.gameObject);
                    GameObject = (gameObject as GameObject);

                    Transform = GameObject.transform;
                    Transform.localScale = scale;
                    if (data.scale > 0)
                    {
                        Transform.localScale = new Vector3(data.scale, data.scale, data.scale);
                    }
                    Transform.tag = "Monster";
                    Transform.gameObject.layer = 11;
                    sfxHandler = GameObject.AddComponent<SfxHandler>();
                    motor = GameObject.AddComponent<MogoMotorServer>();

                    audioSource = GameObject.AddComponent<AudioSource>();
                    audioSource.rolloffMode = AudioRolloffMode.Custom;

                    CharacterController controller = GameObject.GetComponent<CharacterController>();
                    controller.radius = m_monsterData.scaleRadius / 100f;
                    controller.height = EntityColiderHeight;
                    float centerY = (controller.height > controller.radius * 2) ? (controller.height * 0.5f) : (controller.radius);
                    controller.center = new Vector3(0, centerY, 0);

                    animator = GameObject.GetComponent<Animator>();
                    ActorDummy ap = GameObject.AddComponent<ActorDummy>();
                    ap.theEntity = this;
                    this.Actor = ap;
                    UpdatePosition();
                    if (data.originalRotation != null && data.originalRotation.Count == 3)
                    {
                        Transform.eulerAngles = new Vector3(data.originalRotation[0], data.originalRotation[1], data.originalRotation[2]);
                    }
                    else
                    {
                        if (MogoWorld.thePlayer != null && MogoWorld.thePlayer.Transform != null)
                        {
                            Vector3 targetToLookAt = MogoWorld.thePlayer.Transform.position;
                            Transform.LookAt(new Vector3(targetToLookAt.x, Transform.position.y, targetToLookAt.z));
                        }
                    }
                    if (NotTurn())
                    {
                        motor.canTurn = false;
                    }
                    hp = (uint)m_monsterData.hpBase;
                    #region Shader
                    if (ShaderData.dataMap.ContainsKey(m_monsterData.shader)
                        && GameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true) != null)
                    {
                        MogoFXManager.Instance.SetObjShader(GameObject, ShaderData.dataMap[m_monsterData.shader].name, ShaderData.dataMap[m_monsterData.shader].color);
                        MogoFXManager.Instance.AlphaFadeIn(GameObject, fadeInTime);
                    }
                    #endregion

                    base.CreateModel();

                    MogoFXManager.Instance.AddEnemyInScreen(Transform.gameObject, ID);

                    motor.SetAngularSpeed(240f);
                    motor.acceleration = 2f;

                    //GameObject.AddComponent<MogoObjOpt>().ObjType = MogoObjType.Dummy;
                    //if (GameObject.GetComponent<Animation>() != null)
                    //{
                    //    GameObject.GetComponent<Animation>().enabled = false;
                    //} //鸟人动作带位移
                    try
                    {
                        if (m_monsterData != null && m_monsterData.bornFx != null)
                            foreach (var item in m_monsterData.bornFx)
                            {
                                sfxHandler.HandleFx(item);
                            }
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Except(ex);
                    }

                    //开始执行出生流程
                    m_bModelBuilded = true;
                    m_aiRoot = AIContainer.container.Get((uint)m_monsterData.aiId);

                    uint waitTime = (uint)m_monsterData.bornTime;
                    if (waitTime <= 1)//容错
                        waitTime = 3000;
                    if (blackBoard.timeoutId > 0)
                        TimerHeap.DelTimer(blackBoard.timeoutId);

                    //LoggerHelper.Error("RebornAnimationDelay" + waitTime);
                    m_currentSee = m_monsterData.see;
                    blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);
                    blackBoard.timeoutId = TimerHeap.AddTimer(waitTime, 0, RebornAnimationDelay);
                    TimerHeap.AddTimer(waitTime, 0, BornedHandler);
                    Actor.ActChangeHandle = ActionChange;
                }
            );
        }

        private void BornedHandler()
        {
            AddShadow();
            AddBillBoard();
        }

        private void AddBillBoard()
        {
            if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                MainUILogicManager.Instance.SetBossMessageInit(this, m_monsterData.hpShow[2]);
            BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, MonsterData.IsHpShow((int)m_monsterData.id));
            //EventDispatcher.TriggerEvent<float, uint>(BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, base.PercentageHp, ID);
            BillboardViewManager.Instance.SetBillboardBlood(base.PercentageHp, ID);
        }

        public override void CreateDeafaultModel()
        {
            AvatarModelData data = AvatarModelData.dataMap[999];
            LoggerHelper.Debug("monster create:" + ID + ",name:" + data.prefabName);
            GameObject go = AssetCacheMgr.GetLocalInstance(data.prefabName) as GameObject;

            Transform = go.transform;
            Transform.localScale = scale;
            Transform.tag = "Monster";
            motor = go.AddComponent<MogoMotorServer>();
            animator = go.GetComponent<Animator>();
            ActorDummy ap = go.AddComponent<ActorDummy>();
            ap.theEntity = this;
            this.Actor = ap;
            UpdatePosition();
            base.CreateModel();


        }

        // 对象进入场景，在这里初始化各种数据， 资源， 模型等
        // 传入数据。
        override public void OnEnterWorld()
        {
            // todo: 这里会加入数据解析
            buffManager = new BuffManager(this);
            skillManager = new SkillManager(this);
            battleManger = new DummyBattleManager(this, skillManager);
            sfxManager = new SfxManager(this);

            //AddUniqEventListener<GameObject, Vector3>(Events.OtherEvent.OnMoveTo, OnMoveTo);
            EventDispatcher.AddEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.AddEventListener(Events.OtherEvent.CallTeammate, OnHelp);

            EventDispatcher.AddEventListener(Events.AIEvent.DummyThink, DummyThink);
            EventDispatcher.AddEventListener<GameObject>(Events.AIEvent.DummyStiffEnd, DummyStiffEnd);
            EventDispatcher.AddEventListener(Events.AIEvent.ProcessBossDie, ProcessBossDie);

            EventDispatcher.AddEventListener<byte, uint>(Events.AIEvent.SomeOneDie, ProcessSomeOneDie);//阵营，eid
            EventDispatcher.AddEventListener<uint, byte, uint, int>(Events.AIEvent.WarnOtherSpawnPointEntities, ProcessWarnOtherSpawnPointEntities);//eid, AIWarnEvent类型

            EventDispatcher.AddEventListener<uint, int, int, int>(Events.GearEvent.Damage, SetDamage);
        }


        // 对象从场景中删除， 在这里释放资源
        override public void OnLeaveWorld()
        {
            if (!IsGolem)
                MogoFXManager.Instance.RemoveShadow(ID);//kevin：这里会出错导致怪物不消失

            MogoFXManager.Instance.RemoveEnemyInScreen(ID);

            BillboardLogicManager.Instance.RemoveBillboard(ID);

            m_bModelBuilded = false;

            #region Shader

            TimerHeap.DelTimer(fadeTimer);

            #endregion

            #region 大血条

            if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 2)
                MainUIViewManager.Instance.ShowBossTarget(false);

            #endregion

            //RemoveUniqEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.RemoveEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.CallTeammate, OnHelp);

            EventDispatcher.RemoveEventListener(Events.AIEvent.DummyThink, DummyThink);
            EventDispatcher.RemoveEventListener<GameObject>(Events.AIEvent.DummyStiffEnd, DummyStiffEnd);
            EventDispatcher.RemoveEventListener(Events.AIEvent.ProcessBossDie, ProcessBossDie);

            EventDispatcher.RemoveEventListener<byte, uint>(Events.AIEvent.SomeOneDie, ProcessSomeOneDie);//阵营，eid
            EventDispatcher.RemoveEventListener<uint, byte, uint, int>(Events.AIEvent.WarnOtherSpawnPointEntities, ProcessWarnOtherSpawnPointEntities);

            EventDispatcher.RemoveEventListener<uint, int, int, int>(Events.GearEvent.Damage, SetDamage);

            skillManager.Clean();
            battleManger.Clean();
            buffManager.Clean();

            if (Actor)
                Actor.ActChangeHandle = null;

            if (MonsterData.clientTrapId == 0)
            {
                base.OnLeaveWorld();
            }
            else
            {
                EventDispatcher.RemoveEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
                RemoveListener();
                ClearBinding();
                if (GameObject)
                {
                    var iid = GameObject.GetInstanceID();
                    if (MogoWorld.GameObjects.ContainsKey(iid))
                        MogoWorld.GameObjects.Remove(iid);
                }


                GameObject = null;
                Transform = null;
                weaponAnimator = null;
                animator = null;
                motor = null;
                sfxHandler = null;
                audioSource = null;
            }
        }

        public void SetCfg(MonsterData cfg)
        {
            m_monsterData = cfg;
            m_monsterId = (uint)m_monsterData.id;//monster,mercenary是服务器同步下来，dummy是本地赋值
            level = (byte)cfg.level;
            hp = (uint)cfg.hpBase;

            curHp = hp;
            //if (LanguageData.dataMap.ContainsKey(m_monsterData.nameCode))头上显示名字
            //{
            //    name = LanguageData.dataMap[m_monsterData.nameCode].content;
            //}
            SetIntAttr("aiId", cfg.aiId);
            SetIntAttr("scaleRadius", cfg.scaleRadius);
            SetIntAttr("speed", cfg.speed);

            SetIntAttr("hpBase", cfg.hpBase);

            SetIntAttr("attackBase", cfg.attackBase);

            SetIntAttr("defenseBase", 0);

            SetIntAttr("hp", cfg.hpBase);
            SetIntAttr("atk", cfg.attackBase);
            SetIntAttr("def", 0);

            SetDoubleAttr("hitRate", cfg.extraHitRate * 0.0001f);
            SetDoubleAttr("critRate", cfg.extraCritRate * 0.0001f);
            SetDoubleAttr("trueStrikeRate", cfg.extraTrueStrikeRate * 0.0001f);
            SetDoubleAttr("antiDefenseRate", cfg.extraAntiDefenceRate * 0.0001f);
            SetDoubleAttr("defenceRate", cfg.extraDefenceRate * 0.0001f);
            SetDoubleAttr("missRate", cfg.missRate * 0.0001f);
            SetDoubleAttr("damageReduceRate", 0.0f);
            if (cfg.showHitAct > 0)
            {
                stateFlag = Mogo.Util.Utils.BitSet(stateFlag, 13);
            }
        }

        protected void AddShadow()
        {
            if (Transform && Transform.gameObject)
                MogoFXManager.Instance.AddShadow(Transform.gameObject, ID);
        }

        //听到怪物呼叫帮助
        private void OnHelp()
        {
        }

        override public void OnDeath(int actionID)
        {
            if (IsGolem)
            {
                golem.Dying();
                if (golemFx != null)
                    golemFx.Dying();

                EventDispatcher.TriggerEvent<uint>(Events.FrameWorkEvent.AOIDelEvtity, ID);
            }
            else
            {
                //if (currentMotionState == MotionState.HIT)
                //{//受击状态不做死亡表现，受击完再表现
                //    return;
                //}

                MogoFXManager.Instance.RemoveEnemyInScreen(ID);

                BillboardLogicManager.Instance.RemoveBillboard(ID);
                StopMove();

                if (motor)
                {
                    motor.CancleLookAtTarget();
                    motor.gravity = 0;
                }

                battleManger.OnDead(actionID);
                try
                {
                    if (sfxHandler)
                    {
                        sfxHandler.RemoveAllFX();
                        if (m_monsterData != null && m_monsterData.dieFx != null)
                            foreach (var item in m_monsterData.dieFx)
                                sfxHandler.HandleFx(item);
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Except(ex);
                }
                if (motor)
                {
                    motor.gravity = 0;
                }
                if (Actor)
                {
					Collider collider = Actor.GetComponent<Collider> ();
					var cc = collider as CharacterController;
                    if (cc)
                        cc.radius = 0;
                }

                #region Shader

                if (m_monsterData != null && GameObject)
                {
                    if (ShaderData.dataMap.ContainsKey(m_monsterData.shader)
                        && GameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true) != null)
                    {
                        fadeTimer = TimerHeap.AddTimer((uint)(m_monsterData.deadTime - beginFadeOutTimeForward) > 0 ? (uint)(m_monsterData.deadTime - beginFadeOutTimeForward) : 0, 0, () =>
                        {
                            MogoFXManager.Instance.AlphaFadeOut(GameObject, fadeOutTime);
                        });
                    }
                }

                #endregion

                TimerHeap.AddTimer<EntityParent>((uint)m_monsterData.deadTime, 0, (e_) =>
                {
                    EventDispatcher.TriggerEvent<uint>(Events.FrameWorkEvent.AOIDelEvtity, ID);
                }, this);
            }
        }

        private void DummyThink()
        {
            if (GetTickCount() - LastThinkTick > 600)
                Think(AIEvent.AvatarPosSync);
        }

        private void DummyStiffEnd(GameObject g)
        {
            if (g == null) return;
            if (Transform == null) return;
            if (g == Transform.gameObject)
            {
                blackBoard.LookOn_Mode = -1;
                //LoggerHelper.Debug("dummy  DummyStiffEnd", false, 1);
                Think(AIEvent.StiffEnd);
            }
        }

        private void ProcessSomeOneDie(byte factionFlag, uint eid)
        {
            if (factionFlag != this.m_factionFlag && blackBoard.HasHatred(eid))
            {
                blackBoard.EditHatred(eid, 0);
            }
        }

        private void ProcessWarnOtherSpawnPointEntities(uint eid, byte AIWarnEvent, uint targetEid, int spawnPointCfgId)
        {
            switch (AIWarnEvent)
            {
                case (byte)Mogo.Game.AIWarnEvent.DiscoverSomeOne:
                    //eid通知同spawnPoint的同伴实体把目标加入仇恨列表
                    if (eid != ID && spawnPointCfgId == this.spawnPointCfgId)
                    {
                        //LoggerHelper.Error("Other Guy EditHatred:" + targetEid);
                        blackBoard.EditHatred(targetEid, 1);
                        blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);//不还原状态的话，如果在巡逻冷却状态那么就会延迟到结束才会发生追杀AI
                        Think(AIEvent.AvatarPosSync);
                    }
                    //接下来可能需要即时思考作出反应，可能要执行Think
                    break;
                default:
                    break;
            }
        }

        private void ProcessBossDie()
        {
            //LoggerHelper.Debug("dummy  ProcessBossDie", false, 1);
            stateFlag = Mogo.Util.Utils.BitSet(stateFlag, StateCfg.DEATH_STATE);
        }


        //---------------------------------------非AI模块调用AI相关begin------------------------------------------------


        public int CanThink()
        {
            if (MogoWorld.pauseAI)
            {//为profiler关闭AI使用
                return 1;
            }
            int error = 0;
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
        }

        public override void Think(AIEvent triggerEvent)
        {
            LastThinkTick = GetTickCount();
            //return;
            blackBoard.ChangeEvent(triggerEvent);

            int canThinkError = CanThink();
            if (canThinkError != 0)
            {
                //Mogo.Util.LoggerHelper.Error("canThinkError:" + canThinkError + " AIEvent" + triggerEvent + " ID:" + ID);
                return;
            }

            //Mogo.Util.LoggerHelper.Debug("aiId" + m_monsterData.aiId + "  event:" + triggerEvent);

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
                //m_stateFlag = Utils.BitReset(m_stateFlag, StateCfg.NO_HIT_STATE);
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


            return GetMovePointStraight((int)(castRange * 0.7f));

        }


        public override bool ProcCastSpell(int skillBagIndex, int reversal)
        {
            if (m_monsterData.skillIds.Count < skillBagIndex || skillBagIndex <= 0)
            {
                return false;
            }
            skillBagIndex--;
            EntityParent enemyEntity = GetTargetEntity();
            if (enemyEntity == null)
                return false;

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
                //Mogo.Util.LoggerHelper.Debug("notTurn");
            }


            blackBoard.lastCastIndex = skillBagIndex;
            int skillId = m_monsterData.skillIds[skillBagIndex];
            if (!SkillData.dataMap.ContainsKey(skillId))
                return false;//error

            blackBoard.LookOn_LastMode = -1;
            motor.CancleLookAtTarget();
            this.CastSkill(skillId);

            blackBoard.skillActTime = (uint)GetTotalActionDuration(skillId);

            blackBoard.skillActTick = GetTickCount();

            blackBoard.LookOn_LastMode = -1;

            blackBoard.lastCastCoord = Transform.position;

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
                    //Mogo.Util.LoggerHelper.Debug("IsInSkillRange:true");
                }
                else
                {
                    rnt = false;
                    //Mogo.Util.LoggerHelper.Debug("IsInSkillRange:false");
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
            DummyBattleManager tmpBattleManager = this.battleManger as DummyBattleManager;
            rnt = tmpBattleManager.IsCoolDown(skillId);

            return rnt;
        }

        protected void SetDamage(uint damageID, int action, int type, int damage)
        {
            if (damageID != ID)
                return;

            if (MogoWorld.inCity || MissionManager.HasWin)
                return;

            if (deathFlag > 0 || curHp == 0)
                return;

            curHp = curHp >= (uint)damage ? curHp - (uint)damage : 0;

            if (curHp <= 0)
            {
                MogoWorld.thePlayer.RpcCall("CliEntityActionReq", ID, (uint)1, (uint)(Transform.position.x * 100.0f), (uint)(Transform.position.z * 100.0f));
                OnDeath(action);
                stateFlag = Mogo.Util.Utils.BitSet(stateFlag, StateCfg.DEATH_STATE);

                if (MogoWorld.showFloatBlood)
                {
                    switch (type)
                    {
                        case 2:
                            Actor.FloatBlood(damage, SplitBattleBillboardType.NormalMonster);
                            break;
                    }
                }

                return;
            }

            List<int> harm = new List<int>();
            harm.Add(type);
            harm.Add(damage);
            OnHit(action, 0, ID, harm);
        }
    }
}