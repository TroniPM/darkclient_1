/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityMonster
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：服务器端控制的怪物
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using Mogo.FSM;
using Mogo.Util;
using Mogo.GameData;
using System;
using System.Reflection;

namespace Mogo.Game
{
    public class EntityMonster : EntityParent
    {
        public readonly static int Goddess = 5010;
        public readonly static int Tower1 = 42101;
        public readonly static int Tower2 = 42201;

        private int m_model;

        private byte m_deathFlag;

        private int m_clientTrapId;

        private GolemAnimation golem;
        private GolemFx golemFx;

        private UInt32 m_monsterId;

        private uint fadeTimer = uint.MaxValue;

        private uint attachBuildingTime = uint.MaxValue;

        private bool isGolem = false;
        public bool IsGolem
        {
            get { return isGolem; }
            protected set { isGolem = value; }
        }

        public int model
        {
            get { return m_model; }
            set { m_model = value; }
        }
        private bool m_playBornFX = true;

        public bool PlayBornFX
        {
            get { return m_playBornFX; }
            set { m_playBornFX = value; }
        }
        private bool m_BillBoard = true;
        public bool BillBoard
        {
            get { return m_BillBoard; }
            set { m_BillBoard = value; }
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
                        if (golem != null)
                            golem.Hitting();
                        if (golemFx != null)
                            golemFx.Hitting();
                    }
                }

                base.curHp = value;

                if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                    EventDispatcher.TriggerEvent(Events.UIBattleEvent.OnFlushBossBlood, this as EntityParent, (int)base.curHp);

                //EventDispatcher.TriggerEvent<float, uint>(BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, base.PercentageHp, ID);
                BillboardViewManager.Instance.SetBillboardBlood(base.PercentageHp, ID);

                if (m_monsterId == Goddess)
                {
                    MainUILogicManager.Instance.FlushTDBlood(base.PercentageHp);
                }
                else if (m_monsterId == Tower1 || m_monsterId == Tower2)
                {
                    EventDispatcher.TriggerEvent(Events.MonsterEvent.TowerDamage, golem, (int)(base.PercentageHp * 100));
                }
            }
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

        public override string HeadIcon
        {
            get
            {
                if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                    return IconData.dataMap.Get(m_monsterData.hpShow[0]).path;
                else
                    return String.Empty;
            }
        }

        public override string name
        {
            get
            {
                if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                    return LanguageData.GetContent(m_monsterData.hpShow[1]);
                else
                    return String.Empty;
            }
        }

        public override byte level
        {
            get
            {
                return (byte)m_monsterData.level;
            }
        }

        public byte deathFlag
        {
            get { return m_deathFlag; }
            set
            {
                Mogo.Util.LoggerHelper.Debug("Building Dead");

                if (m_deathFlag != value && value == 1)
                {
                    OnDeath(-1);

                    if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 2)
                        MainUIViewManager.Instance.ShowBossTarget(false);

                }

                m_deathFlag = value;
            }
        }

        override public ulong stateFlag
        {
            get
            {
                return m_stateFlag;
            }
            set
            {
                base.stateFlag = value;
                m_stateFlag = value;
                byte f = (byte)Utils.BitTest(m_stateFlag, (int)StateCfg.DEATH_STATE);
                //byte f = (byte)(m_stateFlag & 1);
                if (f != deathFlag)
                {
                    deathFlag = f;
                }
            }
        }

        public int clientTrapId
        {
            get { return m_clientTrapId; }
            set
            {
                m_clientTrapId = value;
                if (m_clientTrapId != 0)
                    IsGolem = true;
            }
        }

        public UInt32 monsterId
        {
            get { return m_monsterId; }
            set
            {
                m_monsterId = value;
                //MonsterData.GetData((int)m_monsterId, MogoWorld.thePlayer.ApplyMissionID == Mogo.Game.RandomFB.RAIDID ? MogoWorld.thePlayer.level : (int)m_difficulty);

                m_monsterData = MonsterData.GetData((int)m_monsterId, (int)m_difficulty);
                //LoggerHelper.Error("m_monsterId " + m_monsterId + "m_difficulty" + m_difficulty);
                //LoggerHelper.Error("m_monsterData " + m_monsterData.hpBase + " curHp:" + curHp);
                //LoggerHelper.Error("ogoWorld.thePlayer.sceneId " + MogoWorld.thePlayer.sceneId);
                //LoggerHelper.Error("MogoWorld.thePlayer.ApplyMissionID == 42000 ? MogoWorld.thePlayer.level : 0 = " + (MogoWorld.thePlayer.sceneId == 42000 ? (int)m_difficulty : 0));
            }
        }

        public byte factionFlag
        {
            get
            {
                return m_factionFlag;
            }
            set
            {
                m_factionFlag = value;
                Mogo.Util.LoggerHelper.Debug("kevin factionFlag Mon:" + m_factionFlag);
            }
        }

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

        public override bool NotTurn()
        {
            if (m_monsterData != null && m_monsterData.notTurn == 1)
                return true;
            else
                return false;
        }

        public EntityMonster()
        {
        }

        override public void OnMoveTo(GameObject g, Vector3 v)
        {
            if (g == null) return;
            if (Transform == null) return;
            if (g == Transform.gameObject)
            {
                //            LoggerHelper.Debug("OnMoveTo");
                ChangeMotionState(MotionState.IDLE);
            }
        }

        public override void CreateModel()
        {
            if (clientTrapId == 0)
            {
                //CreateDeafaultModel();
                CreateActualModel();
            }
            else
            {
                LoggerHelper.Debug("CreateBuildingModel");

                IsGolem = true;
                attachBuildingTime = TimerHeap.AddTimer(500, 0, AttachBuildingModel);
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

                    ActorMonster ap = GameObject.GetComponent<ActorMonster>();
                    if (ap == null)
                        ap = GameObject.AddComponent<ActorMonster>();

                    ap.theEntity = this;
                    this.Actor = ap;

                    golem = gear.gameObject.GetComponentInChildren<GolemAnimation>();
                    golemFx = gear.gameObject.GetComponentInChildren<GolemFx>();

                    // golem = gear as GolemAnimation;

                    if (golem != null)
                        golem.Activate();
                    if (golemFx != null)
                        golemFx.Activate();

                    BornedHandler();

                    // base.CreateModel();
                }
            }
        }

        public override void CreateActualModel()
        {
            AvatarModelData data = AvatarModelData.dataMap.GetValueOrDefault(model, null);
            if (data == null)
            {
                LoggerHelper.Error("Model not found: " + model);
                return;
            }
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
                    Transform.tag = "Monster";
                    Transform.gameObject.layer = 11;

                    sfxHandler = GameObject.AddComponent<SfxHandler>();
                    motor = GameObject.AddComponent<MogoMotorServer>();

                    audioSource = GameObject.AddComponent<AudioSource>();
                    audioSource.rolloffMode = AudioRolloffMode.Custom;

                    if (m_monsterData.notTurn == 0)
                    {
                        motor.SetAngularSpeed(360f);
                    }
                    else
                    {
                        motor.canMove = false;
                        motor.canTurn = false;
                    }
                    Mogo.Util.LoggerHelper.Debug(" m_monsterData.scaleRadius:" + m_monsterData.scaleRadius);
                    CharacterController controller = GameObject.GetComponent<CharacterController>();
                    controller.radius = m_monsterData.scaleRadius / 100f;
                    controller.height = EntityColiderHeight;
                    float centerY = (controller.height > controller.radius * 2) ? (controller.height * 0.5f) : (controller.radius);
                    controller.center = new Vector3(0, centerY, 0);
                    animator = GameObject.GetComponent<Animator>();
                    //animator.speed = 0.3f;
                    ActorMonster ap = GameObject.AddComponent<ActorMonster>();
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
                    base.CreateModel();
                    animator.applyRootMotion = false;

                    #region Shader
                    if (ShaderData.dataMap.ContainsKey(m_monsterData.shader)
                        && (GameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true) != null || GameObject.GetComponentsInChildren<MeshRenderer>(true)!= null))
                    {
                        MogoFXManager.Instance.SetObjShader(GameObject, ShaderData.dataMap[m_monsterData.shader].name, ShaderData.dataMap[m_monsterData.shader].color);
                        MogoFXManager.Instance.AlphaFadeIn(GameObject, 0.3f);
                    }
                    #endregion

                    if (data.scale > 0)
                    {
                        Transform.localScale = new Vector3(data.scale, data.scale, data.scale);
                    }
                    try
                    {
                        if (m_monsterData != null && m_monsterData.bornFx != null && PlayBornFX)
                            foreach (var item in m_monsterData.bornFx)
                            {
                                sfxHandler.HandleFx(item);
                            }
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Except(ex);
                    }
                    uint waitTime = (uint)m_monsterData.bornTime;
                    if (waitTime <= 1)//容错
                        waitTime = 3000;
                    BornedHandler();
                    Actor.ActChangeHandle = ActionChange;
                }
            );
        }

        private void BornedHandler()
        {
            AddShadow();

            
            hp = (uint)m_monsterData.hpBase;
            

            if (BillBoard)
            {
                BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, MonsterData.IsHpShow((int)m_monsterData.id));
            }
            if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 3)
                MainUILogicManager.Instance.SetBossMessageInit(this, m_monsterData.hpShow[2]);

            if (m_monsterId == Goddess)
                MainUILogicManager.Instance.InitTDBlood(this);
        }

        protected void AddShadow()
        {
            if (Transform && Transform.gameObject)
                MogoFXManager.Instance.AddShadow(Transform.gameObject, ID);
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
            ActorMonster ap = go.AddComponent<ActorMonster>();
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

            skillManager = new SkillManager(this);
            battleManger = new MonsterBattleManager(this, skillManager);
            sfxManager = new SfxManager(this);

            //AddUniqEventListener<GameObject, Vector3>(Events.OtherEvent.OnMoveTo, OnMoveTo);
            EventDispatcher.AddEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.AddEventListener(Events.OtherEvent.CallTeammate, OnHelp);
            //CreateModel();
        }

        // 对象从场景中删除， 在这里释放资源
        override public void OnLeaveWorld()
        {
            Mogo.Util.LoggerHelper.Debug("Monster RemoveShadow");
            MogoFXManager.Instance.RemoveShadow(ID);
            BillboardLogicManager.Instance.RemoveBillboard(ID);

            #region Shader

            TimerHeap.DelTimer(fadeTimer);

            #endregion

            #region 大血条

            if (m_monsterData != null && m_monsterData.hpShow != null && m_monsterData.hpShow.Count >= 2)
                MainUIViewManager.Instance.ShowBossTarget(false);

            #endregion

            TimerHeap.DelTimer(attachBuildingTime);

            //RemoveUniqEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.RemoveEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, OnMoveTo);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.CallTeammate, OnHelp);
            skillManager.Clean();
            battleManger.Clean();

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


        // 从未被调用
        public override void OnHit(int actionID, uint _attackerID, uint woundId, List<int> harm)
        {
            if (IsGolem)
            {
                if (golem != null)
                    golem.Hitting();
            }
            else
            {
                base.OnHit(actionID, _attackerID, woundId, harm);
            }
        }

        public override void OnDeath(int actionID)
        {
            if (IsGolem)
            {
                if (golem != null)
                    golem.Dying();
                if (golemFx != null)
                    golemFx.Dying();
            }
            else
            {
                base.OnDeath(actionID);
            }
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
                var cc = Actor.GetComponent<Collider>() as CharacterController;
                if (cc)
                    cc.radius = 0;
            }
            #region Shader

            if (m_monsterData != null && ShaderData.dataMap.ContainsKey(m_monsterData.shader)
                && GameObject && GameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true) != null)
            {
                fadeTimer = TimerHeap.AddTimer((uint)(m_monsterData.deadTime - 500), 0, () =>
                {
                    MogoFXManager.Instance.AlphaFadeOut(GameObject, 0.3f);
                });
            }

            #endregion
        }

        //听到怪物呼叫帮助
        private void OnHelp()
        {
        }
    }
}