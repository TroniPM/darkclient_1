/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityMercenary
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

namespace Mogo.Game
{
    public partial class EntityMercenary : EntityParent
    {
        public EntityMercenary()
        {
            isLittleGuy = false;
        }

        public override int hitStateID
        {
            get
            {
                return (int)monsterId;
            }
        }

        override protected void CheckImmobilize(ulong value)
        {
            //免疫
        }

        override protected void CheckDizzy(ulong value)
        {
            //免疫
        }

        public override bool NotTurn()
        {
            if (m_monsterData != null && m_monsterData.notTurn == 1)
                return true;
            else
                return false;
        }

        public override void CastSkill(int nSkillID)
        {
            (battleManger as MercenaryBattleManager).SetCoolDown(nSkillID);
            base.CastSkill(nSkillID);
            ulong t = (ulong)Time.realtimeSinceStartup;
            Int16 x = (Int16)(Transform.position.x * 100);
            Int16 z = (Int16)(Transform.position.z * 100);
            byte face = (byte)(Transform.eulerAngles.y * 0.5);
            UInt16 sid = (UInt16)nSkillID;
            List<int> l = new List<int>();
            MogoWorld.thePlayer.RpcCall("MercenaryUseSkillReq", t, ID, x, z, face, sid, l);
        }

        public bool IsMonster()
        {
            return m_monsterId > 0;
        }

        public bool IsMercenary()
        {
            return m_monsterId <= 0 && m_factionFlag >= 0;
        }

        public bool IsPVP()
        {
            return m_monsterId <= 0 && m_factionFlag == 0;
        }

        public override void CreateModel()
        {
            if (MogoWorld.inCity)//城里不允许创建
                return;

            if (clientTrapId == 0)
            {
                if (m_monsterId > 0)
                {
                    //Mogo.Util.LoggerHelper.Error("CreateModel:MogoWorld.thePlayer.ApplyMissionID" + MogoWorld.thePlayer.ApplyMissionID + " dif:" + (MogoWorld.thePlayer.ApplyMissionID == Mogo.Game.RandomFB.RAIDID ? MogoWorld.thePlayer.level : (int)m_difficulty));
                    m_monsterData = MonsterData.GetData((int)m_monsterId, MogoWorld.thePlayer.ApplyMissionID == Mogo.Game.RandomFB.RAIDID ? MogoWorld.thePlayer.level : (int)m_difficulty);

                    SetIntAttr("speed", m_monsterData.speed);
                    CreateActualModel();
                }
                else
                {
                    object skills = ObjectAttrs.GetValueOrDefault("skillBag", null);
                    if (skills == null)
                    {
                        return;
                    }

                    LuaTable idTable = (LuaTable)skills;
                    List<int> args;
                    Mogo.Util.Utils.ParseLuaTable(idTable, out args);
                    List<int> tmpSkills = new List<int>();
                    for (int i = 0; i < args.Count; i++)
                    {
                        //tmpSkills.Add(2061);
                        //Mogo.Util.LoggerHelper.Error("tmpSkills.Add:" + (int)args[i]);
                        tmpSkills.Add((int)args[i]);
                    }
                    if (tmpSkills[0] == 2001)
                    {//战士大剑
                        //tmpSkills[1] = 2004;
                        //tmpSkills[2] = 2005;
                        //tmpSkills[3] = 2001;// 2006;
                        //tmpSkills[4] = 2021;//位移技能
                        //tmpSkills[6] = 2015;//咆哮
                    }
                    if (tmpSkills[0] == 2057)
                    {//刺客匕首
                        //tmpSkills[1] = 2060;
                        //tmpSkills[2] = 2061;
                        //tmpSkills[3] = 2062;
                        //tmpSkills[4] = 2071;//位移技能
                        //tmpSkills[6] = 2074;
                    }
                    if (tmpSkills[0] == 2101)
                    {//弓手弓
                        //tmpSkills[1] = 2104;
                        //tmpSkills[2] = 2105;
                        //tmpSkills[3] = 2106;
                        //tmpSkills[4] = 2121;//位移技能
                        //tmpSkills[6] = 2113;
                    }
                    if (tmpSkills[0] == 2151)
                    {//法师法杖
                        //tmpSkills[1] = 2154;
                        //tmpSkills[2] = 2155;
                        //tmpSkills[3] = 2156;
                        //tmpSkills[4] = 2171;//位移技能
                        //tmpSkills[6] = 2165;
                    }

                    if (m_factionFlag == 0)
                    { //是离线pvp玩家

                        switch (vocation)
                        {
                            case Vocation.Warrior:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10201, 5000, tmpSkills);
                                break;
                            case Vocation.Assassin:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10202, 5000, tmpSkills);
                                break;
                            case Vocation.Archer:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10203, 5000, tmpSkills);
                                break;
                            case Vocation.Mage:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10204, 5000, tmpSkills);
                                break;
                            default:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10011, 5000, tmpSkills);
                                break;
                        }
                        SetIntAttr("speed", 400);//临时对雇佣兵
                    }
                    else
                    {//是小伙伴
                        switch (vocation)
                        {
                            case Vocation.Warrior:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10101, 1000, tmpSkills);
                                break;
                            case Vocation.Assassin:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10102, 1000, tmpSkills);
                                break;
                            case Vocation.Archer:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10103, 1000, tmpSkills);
                                break;
                            case Vocation.Mage:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10104, 1000, tmpSkills);
                                break;
                            default:
                                m_monsterData = MonsterData.MercenaryDataFactory(0, 0, 0, 0, 10002, 1000, tmpSkills);
                                break;
                        }
                        SetIntAttr("speed", 600);//临时对雇佣兵
                    }



                    CreatePlayerActualModel();
                }
            }
            else
            {
                //AttachBuildingModel();
            }



        }
        /*
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
                            // motor = GameObject.AddComponent<MogoMotorServer>();
                            animator = GameObject.GetComponent<Animator>();

                            ActorDummy ap = GameObject.GetComponent<ActorDummy>();
                            if (ap == null)
                                ap = GameObject.AddComponent<ActorDummy>();

                            ap.theEntity = this;
                            this.Actor = ap;
                        }
                    }
                }
        */
        public void CreatePlayerActualModel()
        {

            isCreatingModel = true;
            AssetCacheMgr.ReleaseLocalInstance(GameObject);
            AvatarModelData data = AvatarModelData.dataMap.Get((int)vocation);
            SubAssetCacheMgr.GetPlayerInstance(data.prefabName,
                (prefab, guid, go) =>
                {
                    var gameObject = go as GameObject;
                    var actor = gameObject.AddComponent<ActorMercenary>();
                    actor.InitEquipment((int)vocation);
                    motor = gameObject.AddComponent<MogoMotorServer>();

                    CharacterController controller = gameObject.GetComponent<CharacterController>();
                    controller.radius = m_monsterData.scaleRadius / 100f;
                    controller.height = EntityColiderHeight;
                    float centerY = (controller.height > controller.radius * 2) ? (controller.height * 0.5f) : (controller.radius);
                    controller.center = new Vector3(0, centerY, 0);

                    animator = gameObject.GetComponent<Animator>();

                    sfxHandler = gameObject.AddComponent<SfxHandler>();

                    actor.theEntity = this;

                    GameObject = gameObject;
                    Transform = gameObject.transform;
                    this.Actor = actor;
                    //Transform.localScale = scale;
                    Transform.tag = "OtherPlayer";
                    Transform.gameObject.layer = 18;
                    Vector3 targetToLookAt = MogoWorld.thePlayer.Transform.position;
                    Transform.LookAt(new Vector3(targetToLookAt.x, Transform.position.y, targetToLookAt.z));

                    UpdatePosition();

                    base.CreateModel();
                    LoadEquip();
                    //gameObject.SetActive(false);
                    gameObject.SetActive(true);
                    isCreatingModel = false;
                    if (m_factionFlag == 0)
                        BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, true);
                    else
                        BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, false);

                    MogoFXManager.Instance.AddShadow(Transform.gameObject, ID);

                    #region 血条

                    if (isLittleGuy)
                    {
                        MainUILogicManager.Instance.SetMercenaryMessageInit(this);
                    }
                    else if (IsPVP())
                    {
                        //logic
                    }
                    else
                    {
                        hp = (uint)m_monsterData.hpBase;
                        //curHp = (uint)m_monsterData.hpBase;
                        if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                            MainUILogicManager.Instance.SetBossMessageInit(this, m_monsterData.hpShow[2]);
                    }

                    #endregion


                    //开始执行出生流程
                    m_bModelBuilded = true;

                    m_aiRoot = AIContainer.container.Get((uint)m_monsterData.aiId);

                    uint waitTime = (uint)m_monsterData.bornTime;
                    if (waitTime <= 1)//容错
                        waitTime = 3000;
                    if (blackBoard.timeoutId > 0)
                    {
                        TimerHeap.DelTimer(blackBoard.timeoutId);
                    }
                    //LoggerHelper.Error("RebornAnimationDelay begin:" + waitTime);

                    m_currentSee = m_monsterData.see;
                    blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);
                    TimerHeap.AddTimer(waitTime, 0, RebornAnimationDelay);
                    Actor.ActChangeHandle = ActionChange;
                }

            );
        }

        private void LoadEquip()
        {
            //Mogo.Util.LoggerHelper.Debug("m_loadedCuirass" + m_loadedCuirass + " " + m_loadedArmguard + " " + m_loadedLeg + " " + loadedWeapon);
            if (m_loadedCuirass != 0)
            {
                Equip((int)m_loadedCuirass);
            }

            if (m_loadedArmguard != 0)
            {
                Equip((int)m_loadedArmguard);
            }

            if (m_loadedLeg != 0)
            {
                Equip((int)m_loadedLeg);
            }

            if (loadedWeapon != 0)
            {
                //Mogo.Util.LoggerHelper.Debug("id:" + dbid);
                //Mogo.Util.LoggerHelper.Debug("load weapon:" + loadedWeapon);
                Equip((int)loadedWeapon);
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
                    ActorMercenary ap = GameObject.AddComponent<ActorMercenary>();
                    ap.theEntity = this;
                    this.Actor = ap;
                    UpdatePosition();
                    if (data.originalRotation != null && data.originalRotation.Count == 3)
                    {
                        Transform.eulerAngles = new Vector3(data.originalRotation[0], data.originalRotation[1], data.originalRotation[2]);
                    }
                    else
                    {
                        Vector3 targetToLookAt = MogoWorld.thePlayer.Transform.position;
                        Transform.LookAt(new Vector3(targetToLookAt.x, Transform.position.y, targetToLookAt.z));
                    }
                    motor.canTurn = !NotTurn();
                    base.CreateModel();
                    motor.SetAngularSpeed(400f);
                    motor.acceleration = 2f;

                    if (!NotTurn())
                    {
                        Vector3 bornTargetToLookAt = MogoWorld.thePlayer.position;
                        Transform.LookAt(new Vector3(bornTargetToLookAt.x, Transform.position.y, bornTargetToLookAt.z));
                    }


                    #region Shader
                    if (ShaderData.dataMap.ContainsKey(m_monsterData.shader)
                        && GameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true) != null
                        && ID != MogoWorld.theLittleGuyID)
                    {
                        MogoFXManager.Instance.SetObjShader(GameObject, ShaderData.dataMap[m_monsterData.shader].name, ShaderData.dataMap[m_monsterData.shader].color);
                        MogoFXManager.Instance.AlphaFadeIn(GameObject, fadeInTime);
                    }
                    #endregion
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
                    {
                        TimerHeap.DelTimer(blackBoard.timeoutId);
                    }

                    m_currentSee = m_monsterData.see;
                    blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);
                    TimerHeap.AddTimer(waitTime, 0, RebornAnimationDelay);
                    TimerHeap.AddTimer(waitTime, 0, BornedHandler);
                    Actor.ActChangeHandle = ActionChange;
                }
            );
        }

        private void BornedHandler()
        {
            AddShadow();
            BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, MonsterData.IsHpShow((int)m_monsterData.id));
            //EventDispatcher.TriggerEvent<float, uint>(BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, base.PercentageHp, ID);
            BillboardViewManager.Instance.SetBillboardBlood(base.PercentageHp, ID);
            if (isLittleGuy)
            {
                MainUILogicManager.Instance.SetMercenaryMessageInit(this);
            }
            else
            {
                hp = (uint)m_monsterData.hpBase;
                //curHp = (uint)m_monsterData.hpBase;
                if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                    MainUILogicManager.Instance.SetBossMessageInit(this, m_monsterData.hpShow[2]);
            }
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
            ActorMercenary ap = go.AddComponent<ActorMercenary>();
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
            battleManger = new MercenaryBattleManager(this, skillManager);
            sfxManager = new SfxManager(this);

            m_updateCoordTimerID = TimerHeap.AddTimer<bool>(1000, 1000, SyncPos, true);
            EventDispatcher.AddEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.AddEventListener(Events.OtherEvent.CallTeammate, OnHelp);

            EventDispatcher.AddEventListener(Events.AIEvent.DummyThink, DummyThink);
            EventDispatcher.AddEventListener<GameObject>(Events.AIEvent.DummyStiffEnd, DummyStiffEnd);
            EventDispatcher.AddEventListener(Events.AIEvent.ProcessBossDie, ProcessBossDie);

            EventDispatcher.AddEventListener<byte, uint>(Events.AIEvent.SomeOneDie, ProcessSomeOneDie);//阵营，eid
            EventDispatcher.AddEventListener<uint, byte, uint, int>(Events.AIEvent.WarnOtherSpawnPointEntities, ProcessWarnOtherSpawnPointEntities);//eid, AIWarnEvent类型

            if (MogoWorld.theLittleGuyID == ID)
            {
                isLittleGuy = true;
            }
        }


        // 对象从场景中删除， 在这里释放资源
        override public void OnLeaveWorld()
        {
            TimerHeap.DelTimer(m_updateCoordTimerID);

            MogoFXManager.Instance.RemoveShadow(ID);//kevin：这里会出错导致怪物不消失

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
            EventDispatcher.RemoveEventListener<uint, byte, uint, int>(Events.AIEvent.WarnOtherSpawnPointEntities, ProcessWarnOtherSpawnPointEntities);//eid, AIWarnEvent类型
            skillManager.Clean();
            battleManger.Clean();
            buffManager.Clean();

            if (MogoWorld.theLittleGuyID == ID)
            {
                MogoWorld.theLittleGuyID = 0;
                MainUIViewManager.Instance.ShowMember1(false);
            }

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
                var iid = GameObject.GetInstanceID();
                if (GameObject && MogoWorld.GameObjects.ContainsKey(iid))
                    MogoWorld.GameObjects.Remove(iid);

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
            SetIntAttr("speed", cfg.speed);
            SetIntAttr("scaleRadius", cfg.scaleRadius);

        }

        //听到怪物呼叫帮助
        private void OnHelp()
        {
        }

        override public void OnDeath(int actionID)
        {
            //if (currentMotionState == MotionState.HIT)
            //{//受击状态不做死亡表现，受击完再表现
            //    return;
            //}
            if (animator == null)
            {
                return;
            }
            StopMove();
            motor.CancleLookAtTarget();

            battleManger.OnDead(actionID);
            try
            {
                sfxHandler.RemoveAllFX();
                if (m_monsterData != null && m_monsterData.dieFx != null)
                    foreach (var item in m_monsterData.dieFx)
                        sfxHandler.HandleFx(item);
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
                var cc = Actor.GetComponent<Collider>() as CharacterController;
                if (cc)
                    cc.radius = 0.3f;
            }
            BillboardLogicManager.Instance.RemoveBillboard(ID);
            if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                MainUIViewManager.Instance.ShowBossTarget(false);
            #region Shader

            if (ShaderData.dataMap.ContainsKey(m_monsterData.shader)
                && GameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true) != null
                && ID != MogoWorld.theLittleGuyID)
            {
                fadeTimer = TimerHeap.AddTimer((uint)(m_monsterData.deadTime - beginFadeOutTimeForward) > 0 ? (uint)(m_monsterData.deadTime - beginFadeOutTimeForward) : 0, 0, () =>
                {
                    //LoggerHelper.Error("Entity fadeout 1");
                    MogoFXManager.Instance.AlphaFadeOut(GameObject, fadeOutTime);
                });
            }

            #endregion

            if (m_factionFlag == 0)
            {//雇佣兵不会死
                TimerHeap.AddTimer<EntityParent>((uint)m_monsterData.deadTime, 0, (e_) =>
                {
                    EventDispatcher.TriggerEvent<uint>(Events.FrameWorkEvent.AOIDelEvtity, ID);
                }, this);
            }
            if (IsPVP())
            {
                EventDispatcher.TriggerEvent(Events.OtherEvent.BossDie);
            }
        }

        public void Revive()
        {
            blackBoard.ChangeState(Mogo.AI.AIState.THINK_STATE);
            hitAir = false;
            stiff = false;
            knockDown = false;
            hitGround = false;
            m_borned = false;
            m_stateFlag = Utils.BitSet(m_stateFlag, StateCfg.NO_HIT_STATE);

            currentMotionState = Mogo.FSM.MotionState.IDLE;

            SetAction(ActionConstants.REVIVE);
            SetSpeed(0);
            motor.SetSpeed(0);
            TimerHeap.AddTimer(ActionTime.REVIVE, 0, ReviveEnd);
        }

        public void ReviveEnd()
        {
            m_borned = true;
            m_stateFlag = Utils.BitReset(m_stateFlag, StateCfg.NO_HIT_STATE);
        }


        private void DummyThink()
        {
            //LoggerHelper.Debug("dummy  AvatarPosSync", false, 1);
            if (GetTickCount() - LastThinkTick > 500)
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
            if (m_factionFlag == 0)
            {//是怪物则要死
                stateFlag = Mogo.Util.Utils.BitSet(stateFlag, StateCfg.DEATH_STATE);
            }
        }

        public void SyncPos(bool force = false)
        {
            if (!Transform)
            {
                TimerHeap.DelTimer(m_updateCoordTimerID);
                return;
            }

            Int16 x = (Int16)(Transform.position.x * 100);
            Int16 z = (Int16)(Transform.position.z * 100);
            byte face = (byte)(Transform.eulerAngles.y * 0.5);
            MogoWorld.thePlayer.RpcCall("UpdateMercenaryCoord", ID, x, z, face, curHp);
        }

        protected void AddShadow()
        {
            if (Transform && Transform.gameObject)
                MogoFXManager.Instance.AddShadow(Transform.gameObject, ID);
        }
    }
}
