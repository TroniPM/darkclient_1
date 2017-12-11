/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityMyself
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013.2.1
// 模块描述：角色控制对像
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Task;
using Mogo.Mission;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
using Mogo.FSM;

using Mogo.AI;
using Mogo.AI.BT;

public enum VipRealStateEnum
{
    DAILY_GOLD_METALLURGY_TIMES = 1, // 每日已炼金次数
    DAILY_RUNE_WISH_TIMES = 2, // 符文已许愿次数
    DAILY_ENERGY_BUY_TIMES = 3, // 体力已购买次数
    DAILY_EXTRA_CHALLENGE_TIMES = 4, // 每日额外挑战次数
    DAILY_HARD_MOD_RESET_TIMES = 5, // 困难副本进入已重置次数
    DAILY_RAID_SWEEP_TIMES = 6, // 剧情关卡已扫荡次数
    DAILY_TOWER_SWEEP_TIMES = 7, // 试炼之塔已扫荡次数
    DAILY_TIME_STAMP = 8, // 每日玩家时间戳记录
    DAILY_ITEM_CAN_BUY_ENTER_SDTIMES = 9, // 圣域守卫战额外购买次数
    DAILY_MISSION_TIMES = 10,// 每天副本的进入次数
}

namespace Mogo.Game
{
    public partial class EntityMyself : EntityPlayer
    {
        public const string ON_TASK_GUIDE = "EntityMyself.ON_TASK_GUIDE";
        public const string ON_END_TASK_GUIDE = "EntityMyself.ON_END_TASK_GUIDE";
        public const string ON_TASK_MISSION = "EntityMyself.ON_TASK_MISSION";
        public const string ON_VIP_REAL_STATE = "EntityMyself.ON_VIP_REAL_STATE";

        public static float preSkillTime = 0;//上次使用技能时间

        public bool isInCity;

        private int m_iAiId;

        private GameObject m_roundLight;

        protected bool isLoginFirstShow = true;
        public bool IsLoginFirstShow
        {
            get { return isLoginFirstShow; }
            set
            {
                if (operationSystem != null)
                    operationSystem.loginFirstShow = value;
                isLoginFirstShow = value;
            }
        }

        protected bool loginRewardHasGot = false;
        public bool IsLoginRewardHasGot
        {
            get { return loginRewardHasGot; }
            set
            {
                if (operationSystem != null)
                    operationSystem.loginRewardHasGot = value;
                loginRewardHasGot = value;
            }
        }

        protected bool TimeLimitEventHasOpen = false;
        public bool IsTimeLimitEventHasOpen
        {
            get { return TimeLimitEventHasOpen; }
            set
            {
                if (IsTimeLimitEventHasOpen != value)
                {
                    if (operationSystem != null && OperatingUIViewManager.Instance != null)
                        OperatingUIViewManager.Instance.ShowTimeLimitActivityBtn(value);
                    TimeLimitEventHasOpen = value;
                }
                if (TimeLimitEventHasOpen)
                {
                    GuideSystem.Instance.TriggerEvent(GlobalEvents.TimeLimitEvent);
                }
            }
        }

        protected bool AchiementHasOpen = false;
        public bool IsAchiementHasOpen
        {
            get { return AchiementHasOpen; }
            set
            {
                if (IsAchiementHasOpen != value)
                {
                    if (operationSystem != null && OperatingUIViewManager.Instance != null)
                        OperatingUIViewManager.Instance.ShowAttributeRewardBtn(value);
                    AchiementHasOpen = value;
                }
                if (AchiementHasOpen)
                {
                    GuideSystem.Instance.TriggerEvent(GlobalEvents.Achiement);
                }
            }
        }

        private bool isNewPlayer;
        public bool IsNewPlayer
        {
            get { return isNewPlayer; }
            set { isNewPlayer = value; }
        }

        protected int applyMissionID = 0;
        public int ApplyMissionID
        {
            get { return applyMissionID; }
            set { applyMissionID = value; }
        }

        protected int applyMissionLevel = 0;
        public int ApplyMissionLevel
        {
            get { return applyMissionLevel; }
            set { applyMissionLevel = value; }
        }

        protected int curMissionID = 0;
        public int CurMissionID
        {
            get { return curMissionID; }
            set { curMissionID = value; }
        }

        protected int curMissionLevel = 0;
        public int CurMissionLevel
        {
            get { return curMissionLevel; }
            set { curMissionLevel = value; }
        }

        #region 构造函数

        public EntityMyself()
        {
            entityType = "Avatar";
            AddListener();

            VIPInfoUILogicManager.Instance.ItemSource = this;
            MainUILogicManager.Instance.ItemSource = this;
            NormalMainUILogicManager.Instance.ItemSource = this;
            MenuUILogicManager.Instance.ItemSource = this;
            ArenaUILogicManager.Instance.ItemSource = this;
            EnergyUILogicManager.Instance.ItemSource = this;
            DiamondToGoldUILogicManager.Instance.ItemSource = this;
            EnergyNoEnoughUILogicManager.Instance.ItemSource = this;
            LevelNoEnoughUILogicManager.Instance.ItemSource = this;
            UpgradePowerUILogicManager.Instance.ItemSource = this;
            DragonMatchUILogicManager.Instance.ItemSource = this;
            ChooseDragonUILogicManager.Instance.ItemSource = this;
        }

        #endregion

        public int GetEquipSubType()
        {
            int subtype = 0;
            if (InventoryManager.Instance == null || InventoryManager.Instance.EquipOnDic == null)
            {
                return subtype;
            }
            var equip = InventoryManager.Instance.EquipOnDic.Get((int)EquipSlot.Weapon);
            if (equip == null)
                return subtype;
            if (!ItemEquipmentData.dataMap.ContainsKey((int)(equip.templateId)))
            {
                switch (vocation)
                {
                    case Vocation.Warrior:
                        subtype = (int)(WeaponSubType.blade);
                        break;
                    case Vocation.Archer:
                        subtype = (int)(WeaponSubType.bow);
                        break;
                    case Vocation.Assassin:
                        subtype = (int)(WeaponSubType.twinblade);
                        break;
                    case Vocation.Mage:
                        subtype = (int)(WeaponSubType.staff);
                        break;
                    default:
                        LoggerHelper.Error("vocation Error:" + vocation);
                        return subtype;
                }
            }
            else
            {
                subtype = ItemEquipmentData.dataMap.Get((int)(equip.templateId)).subtype;
            }
            return subtype;
        }

        #region 重写方法
        // 对象进入场景，在这里初始化各种数据， 资源， 模型等
        // 传入数据。
        override public void OnEnterWorld()
        {
            base.OnEnterWorld();
            LoggerHelper.Info("Avatar name: " + name);
            // 在调用该函数前， 数据已经通过EntityAttach 和 EntityCellAttach 同步完毕
            CreateModel();
            inventoryManager = new InventoryManager(this);
            bodyenhanceManager = new BodyEnhanceManager(this);
            skillManager = new PlayerSkillManager(this);
            battleManger = new PlayerBattleManager(this, skillManager as PlayerSkillManager);

            doorOfBurySystem = new DoorOfBurySystem();
            runeManager = new RuneManager(this);
            towerManager = new TowerManager(this);
            missionManager = new MissionManager(this);
            taskManager = new TaskManager(this, (int)taskMain);
            marketManager = new MarketManager(this);
            friendManager = new FriendManager(this);
            operationSystem = new OperationSystem(this);
            sanctuaryManager = new SanctuaryManager(this);
            arenaManager = new ArenaManager(this);
            dailyEventSystem = new DailyEventSystem(this);
            rankManager = new RankManager(this);
            campaignSystem = new CampaignSystem(this);
            wingManager = new WingManager(this);
            rewardManager = new RewardManager(this);
            occupyTowerSystem = new OccupyTowerSystem(this);

            TipManager.Init();
            DragonMatchManager.Init();
            fumoManager = new FumoManager(this);

            MailManager.Instance.IsMailInfoDirty = true;
            TongManager.Instance.Init();
            GuideSystem.Instance.AddListeners();
            StoryManager.Instance.AddListeners();
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnNormalAttack, NormalAttack);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnPowerChargeStart, PowerChargeStart);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnPowerChargeInterrupt, PowerChargeInterrupt);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnPowerChargeComplete, PowerChargeComplete);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnSpellOneAttack, SpellOneAttack);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnSpellTwoAttack, SpellTwoAttack);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnSpellThreeAttack, SpellThreeAttack);
            EventDispatcher.AddEventListener(Events.UIBattleEvent.OnSpellXPAttack, SpecialAttack);
            EventDispatcher.AddEventListener<int>(Events.UIBattleEvent.OnSpriteSkill, OnSpriteSkill);

            EventDispatcher.AddEventListener<uint>(Events.GearEvent.Teleport, Teleport);
            EventDispatcher.AddEventListener<uint, int, int, int>(Events.GearEvent.Damage, SetDamage);

            EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);
            EventDispatcher.AddEventListener<ushort>(Events.OtherEvent.MapIdChanged, OnMapChanged);
            EventDispatcher.AddEventListener<ulong>(Events.OtherEvent.Withdraw, Withdraw);
            EventDispatcher.AddEventListener(Events.OtherEvent.DiamondMine, DiamondMine);
            EventDispatcher.AddEventListener(Events.OtherEvent.CheckCharge, CheckCharge);
            EventDispatcher.AddEventListener(Events.OtherEvent.Charge, Charge);

            EventDispatcher.AddEventListener(ON_TASK_GUIDE, TaskGuide);
            EventDispatcher.AddEventListener(ON_END_TASK_GUIDE, EndTaskGuide);
            EventDispatcher.AddEventListener<int, int>(ON_TASK_MISSION, MissionOpen);

            EventDispatcher.AddEventListener(Events.AIEvent.DummyThink, DummyThink);
            EventDispatcher.AddEventListener(Events.StoryEvent.CGBegin, ProcCGBegin);
            EventDispatcher.AddEventListener(Events.StoryEvent.CGEnd, ProcCGEnd);
            EventDispatcher.AddEventListener<string>(Events.GearEvent.TrapBegin, ProcTrapBegin);
            EventDispatcher.AddEventListener<string>(Events.GearEvent.TrapEnd, ProcTrapEnd);
            EventDispatcher.AddEventListener(Events.GearEvent.LiftEnter, ProcLiftEnter);
            EventDispatcher.AddEventListener<int>(Events.GearEvent.PathPointTrigger, PathPointTrigger);
            EventDispatcher.AddEventListener(Events.DirecterEvent.DirActive, DirActive);

            EventDispatcher.AddEventListener<int>(Events.EnergyEvent.BuyEnergy, BuyEnergy);
            EventDispatcher.AddEventListener(ON_VIP_REAL_STATE, OnVIPRealState);

            EventDispatcher.AddEventListener<int>(Events.DiamondToGoldEvent.GoldMetallurgy, GoldMetallurgy);

            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.AFFECTUP, OnBattleBtnPressed);
            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.OUTPUTUP, OnBattleBtnPressed);
            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.MOVEUP, OnBattleBtnPressed);
            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.NORMALATTACK, OnBattleBtnPressed);
            EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.PLAYERINFOBGUP, OnBattleBtnPressed);
            EventDispatcher.AddEventListener("MainUIControllStickPressed", OnBattleBtnPressed);

            EventDispatcher.AddEventListener<Vector3>(Events.GearEvent.CrockBroken, CrockBroken);
            EventDispatcher.AddEventListener<bool, bool, Vector3>(Events.GearEvent.ChestBroken, ChestBroken);

            EventDispatcher.AddEventListener<GameObject, Vector3, float>(MogoMotor.ON_MOVE_TO_FALSE, OnMoveToFalse);
            EventDispatcher.AddEventListener(Events.OtherEvent.BossDie, BossDie);

            EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, SetCampControl);

            timerID = TimerHeap.AddTimer<bool>(1000, 100, SyncPos, true);
            checkDmgID = TimerHeap.AddTimer(0, 1000, CheckDmgBase);
            syncHpTimerID = TimerHeap.AddTimer(10000, 5000, SyncHp);
            skillFailoverTimer = TimerHeap.AddTimer(1000, 3000, SkillFailover);
            TimerHeap.AddTimer(5000, 0, GetServerTickReq);
            //rateTimer = TimerHeap.AddTimer(1000, 3000, CheckRate);
            CheckCharge();
            GetWingBag();
            MogoTime.Instance.InitTimeFromServer();

            MogoUIManager.Instance.LoadUIResources();
            TimerHeap.AddTimer(500, 0, EventDispatcher.TriggerEvent, Events.RuneEvent.GetBodyRunes);
            TimerHeap.AddTimer(500, 0, marketManager.GiftRecordReq);

            if (IsNewPlayer)
            {
                CurMissionID = 10100;
                CurMissionLevel = 1;

                missionManager.EnterMissionReq(CurMissionID, CurMissionLevel);
            }


            if (PlatformSdkManager.Instance)
                TimerHeap.AddTimer(1000, 60000, PlatformSdkManager.Instance.OnSetupNotification);
        }

        // 对象从场景中删除， 在这里释放资源
        override public void OnLeaveWorld()
        {
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnNormalAttack, NormalAttack);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnPowerChargeStart, PowerChargeStart);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnPowerChargeInterrupt, PowerChargeInterrupt);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnPowerChargeComplete, PowerChargeComplete);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnSpellOneAttack, SpellOneAttack);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnSpellTwoAttack, SpellTwoAttack);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnSpellThreeAttack, SpellThreeAttack);
            EventDispatcher.RemoveEventListener(Events.UIBattleEvent.OnSpellXPAttack, SpecialAttack);
            EventDispatcher.RemoveEventListener<int>(Events.UIBattleEvent.OnSpriteSkill, OnSpriteSkill);

            EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.Teleport, Teleport);
            EventDispatcher.RemoveEventListener<uint, int, int, int>(Events.GearEvent.Damage, SetDamage);

            EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, InstanceLoaded);
            EventDispatcher.RemoveEventListener<ushort>(Events.OtherEvent.MapIdChanged, OnMapChanged);
            EventDispatcher.RemoveEventListener<ulong>(Events.OtherEvent.Withdraw, Withdraw);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.DiamondMine, DiamondMine);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.CheckCharge, CheckCharge);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.Charge, Charge);

            EventDispatcher.RemoveEventListener(ON_TASK_GUIDE, TaskGuide);
            EventDispatcher.RemoveEventListener(ON_END_TASK_GUIDE, EndTaskGuide);
            EventDispatcher.RemoveEventListener<int, int>(ON_TASK_MISSION, MissionOpen);

            EventDispatcher.RemoveEventListener(Events.AIEvent.DummyThink, DummyThink);
            EventDispatcher.RemoveEventListener(Events.StoryEvent.CGBegin, ProcCGBegin);
            EventDispatcher.RemoveEventListener(Events.StoryEvent.CGEnd, ProcCGEnd);
            EventDispatcher.RemoveEventListener<string>(Events.GearEvent.TrapBegin, ProcTrapBegin);
            EventDispatcher.RemoveEventListener<string>(Events.GearEvent.TrapEnd, ProcTrapEnd);
            EventDispatcher.AddEventListener(Events.GearEvent.LiftEnter, ProcLiftEnter);
            EventDispatcher.RemoveEventListener<int>(Events.GearEvent.PathPointTrigger, PathPointTrigger);
            EventDispatcher.RemoveEventListener(Events.DirecterEvent.DirActive, DirActive);

            EventDispatcher.RemoveEventListener<int>(Events.EnergyEvent.BuyEnergy, BuyEnergy);
            EventDispatcher.RemoveEventListener(ON_VIP_REAL_STATE, OnVIPRealState);

            EventDispatcher.RemoveEventListener<int>(Events.DiamondToGoldEvent.GoldMetallurgy, GoldMetallurgy);

            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.AFFECTUP, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.OUTPUTUP, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.MOVEUP, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.NORMALATTACK, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.PLAYERINFOBGUP, OnBattleBtnPressed);
            EventDispatcher.RemoveEventListener("MainUIControllStickPressed", OnBattleBtnPressed);

            EventDispatcher.RemoveEventListener<Vector3>(Events.GearEvent.CrockBroken, CrockBroken);
            EventDispatcher.RemoveEventListener<bool, bool, Vector3>(Events.GearEvent.ChestBroken, ChestBroken);

            EventDispatcher.RemoveEventListener<GameObject, Vector3, float>(MogoMotor.ON_MOVE_TO_FALSE, OnMoveToFalse);
            EventDispatcher.RemoveEventListener(Events.OtherEvent.BossDie, BossDie);

            EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceLoaded, SetCampControl);

            GuideSystem.Instance.RemoveListeners();
            StoryManager.Instance.RemoveListeners();
            missionManager.RemoveListeners();
            taskManager.RemoveListeners();
            operationSystem.RemoveListeners();

            doorOfBurySystem.RemoveListener();
            inventoryManager.RemoveListeners();
            bodyenhanceManager.RemoveListener();
            friendManager.RemoveListener();
            sanctuaryManager.RemoveListeners();
            arenaManager.RemoveListeners();
            dailyEventSystem.RemoveListeners();
            rankManager.RemoveListeners();

            campaignSystem.RemoveListeners();

            wingManager.Clean();
            rewardManager.Clean();
            runeManager.Clean();
            marketManager.Clean();
            skillManager.Clean();
            battleManger.Clean();
            MailManager.Instance.Clean();

            TimerHeap.DelTimer(timerID);

            TimerHeap.DelTimer(syncHpTimerID);
            TimerHeap.DelTimer(skillFailoverTimer);

            MogoTime.Instance.ReleaseMogoTimeData();

            MogoUIManager.Instance.ReleaseUIResources();

            TongManager.Instance.Release();
            TipManager.Instance.RemoveListener();

            base.OnLeaveWorld();
        }

        override public void CreateModel()
        {
            //CreateDeafaultModel();
            CreateActualModel();
        }

        override public void CreateActualModel()
        {
            isCreatingModel = true;
            AvatarModelData data = AvatarModelData.dataMap.Get((int)vocation);



            AssetCacheMgr.GetInstanceAutoRelease(data.prefabName, (prefab, id, go) =>
            {
                //this.Actor.Release();
                //AssetCacheMgr.ReleaseLocalInstance(Transform.gameObject);
                var gameObject = go as GameObject;
                gameObject.tag = "Player";
                ActorMyself actor = gameObject.AddComponent<ActorMyself>();

                motor = gameObject.AddComponent<MogoMotorMyself>();
                animator = gameObject.GetComponent<Animator>();
                sfxHandler = gameObject.AddComponent<SfxHandler>();

                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.rolloffMode = AudioRolloffMode.Custom;

                actor.mogoMotor = motor;
                actor.theEntity = this;
                GameObject = gameObject;

                Transform = gameObject.transform;

                // Debug.LogError(GameObject.name + " " + ID + " Myself CreateActualModel: " + Transform.position);

                Transform.gameObject.layer = 8;
                UpdatePosition();
                if (data.scale > 0)
                {
                    Transform.localScale = new Vector3(data.scale, data.scale, data.scale);
                }

                this.Actor = actor;

                //重新穿上装备
                //actor.RemoveOld();
                if (InventoryManager.Instance != null)
                {
                    foreach (ItemEquipment equip in InventoryManager.Instance.EquipOnDic.Values)
                    {
                        Equip(equip.templateId);
                    }
                }
                gameObject.SetActive(false);
                gameObject.SetActive(true);

                AssetCacheMgr.GetInstanceAutoRelease("RoundLight.prefab", (p, i, light) =>
                {
                    m_roundLight = light as GameObject;
                    m_roundLight.transform.parent = Transform;
                    m_roundLight.transform.localPosition = new Vector3(0, 1, 0);
                });
                try
                {
                    if (MogoMainCamera.Instance != null)
                    {
                        var slot = Mogo.Util.MogoUtils.GetChild(GameObject.transform, "slot_camera");
                        MogoMainCamera.Instance.target = slot;
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Except(ex);
                }
                
                isCreatingModel = false;
                Actor.ActChangeHandle = ActionChange;
                UpdateDressWing();
            });

        }

        override public void CreateDeafaultModel()
        {
            AvatarModelData data = AvatarModelData.dataMap[999];

            GameObject gameObject = AssetCacheMgr.GetLocalInstance(data.prefabName) as GameObject;

            gameObject.tag = "Player";
            ActorMyself actor = gameObject.AddComponent<ActorMyself>();
            actor.isNeedInitEquip = false;
            motor = gameObject.AddComponent<MogoMotorMyself>();
            animator = gameObject.GetComponent<Animator>();
            sfxHandler = gameObject.AddComponent<SfxHandler>();

            actor.mogoMotor = motor;
            actor.theEntity = this;
            GameObject = gameObject;
            Transform = gameObject.transform;

            // Debug.LogError(GameObject.name + " " + ID + " Myself CreateDeafaultModel: " + Transform.position);

            UpdatePosition();
            this.Actor = actor;
        }

        public override void UpdatePosition()
        {
            if (Transform)
                GameObject.layer = 8;
            base.UpdatePosition();

            if (!MogoWorld.IsClientMission)
                MogoWorld.IsClientPositionSync = true;
        }

        public override void UpdateView()
        {
            base.UpdateView();
            if (MenuUIViewManager.Instance != null)
            {
                MenuUILogicManager.Instance.UpdateRole(this);
            }
        }

        public void SetLightVisible(bool isActive)
        {
            if (m_roundLight)
                m_roundLight.SetActive(isActive);
        }

        public void SetModelInfo(bool isInstance = false)
        {
            UpdatePosition();
        }

        public void NormalAttack()
        {
            if (!canCastSpell) return;
            ((PlayerBattleManager)battleManger).NormalAttack();
        }

        public void PowerChargeStart()
        {
            if (!canCastSpell) return;
            ((PlayerBattleManager)battleManger).PowerChargeStart();
        }

        public void PowerChargeInterrupt()
        {
            ((PlayerBattleManager)battleManger).PowerChargeInterrupt();
        }

        public void PowerChargeComplete()
        {
            ((PlayerBattleManager)battleManger).PowerChargeComplete();
        }

        public void SpellOneAttack()
        {
            if (!canCastSpell) return;
            ((PlayerBattleManager)battleManger).SpellOneAttack();
        }

        public void SpellTwoAttack()
        {
            if (!canCastSpell) return;
            ((PlayerBattleManager)battleManger).SpellTwoAttack();
        }

        public void SpellThreeAttack()
        {
            if (!canCastSpell) return;
            ((PlayerBattleManager)battleManger).SpellThreeAttack();
        }

        public void SpecialAttack()
        {
            EnterAngerSt();
        }

        /// <summary>
        /// 通过手势施放精灵技能
        /// </summary>
        /// <param name="gestureNameEnum"></param>
        public void OnSpriteSkill(int gestureNameEnum)
        {
            LoggerHelper.Debug(gestureNameEnum);
        }

        public void UpdateSkillToManager()
        {
            object skills = null;
            ObjectAttrs.TryGetValue("skillBag", out skills);
            if (skills == null)
            {
                return;
            }
            (skillManager as PlayerSkillManager).UpdateSkillData(skills);
        }

        public void Revive()
        {
            //LoggerHelper.Error("revive");
            ClearSkill();
            currentMotionState = Mogo.FSM.MotionState.IDLE;
            SetAction(ActionConstants.REVIVE);
            SetSpeed(0);
            motor.SetSpeed(0);
            TimerHeap.AddTimer(500, 0, Stand);
        }

        private void Stand()
        {
            if (MogoWorld.inCity)
            {
                TimerHeap.AddTimer(500, 0, SetAction, -1);
            }
            motor.enableStick = true;
        }

        public override void OnMoveTo(GameObject arg1, Vector3 arg2)
        {
            if (arg1 == null) return;
            base.OnMoveTo(arg1, arg2);
            if (arg1 == Transform.gameObject)
            {
                //LoggerHelper.Error("OnMoveTo");
                SyncPos(true);
                if (AutoFight == AutoFightState.RUNNING)
                {
                    blackBoard.LookOn_Mode = -1;
                    //速度衰减还原
                    blackBoard.speedFactor = 1.0f;

                    Think(AIEvent.MoveEnd);
                }

            }
        }

        public bool ChargetAble()
        {
            return (skillManager as PlayerSkillManager).ChargeAble();
        }

        #endregion

        #region 私有方法

        private GameObject flag;

        public void SyncPos(bool force = false)
        {
            if (!MogoWorld.IsClientPositionSync)
            {
                // Debug.LogError("return SyncPos");
                return;
            }
            if (!Transform)
            {
                TimerHeap.DelTimer(timerID);
                return;
            }
            if (/*!force &&*/ Mathf.Abs(Transform.position.x - prePos.x) < 0.1f && Mathf.Abs(Transform.position.z - prePos.z) < 0.1f)
            {
                return;
            }
            prePos.x = Transform.position.x;
            prePos.z = Transform.position.z;
            System.Int16 x = (System.Int16)(Transform.position.x * 100.0);
            System.Int16 z = (System.Int16)(Transform.position.z * 100.0);
            byte face = (byte)(Transform.eulerAngles.y * 0.5);
            // Debug.LogError("SyncPos: " + Transform.position);
            ServerProxy.Instance.Move(face, (System.UInt16)x, (System.UInt16)z);

        }
        private uint DmgBase = 0;

        public void CalcuDmgBase()
        {
            DmgBase = curHp + def + atk;
        }
        public void CheckDmgBase()
        {

            if (curHp + def + atk != DmgBase)
            {
                //LoggerHelper.Error(curHp + "   " + def + "   " + atk + " d  " + DmgBase + "    " + deviator);
                UsedTool();
            }
        }

        private uint preHp = 0;
        private void SyncHp()
        {
            if (MogoWorld.inCity || !hadSyncProp || deathFlag == 1 || MogoWorld.IsClientMission)
            {
                return;
            }
            if (curHp == 0)
            {
                //血量为0在被怪物杀死时刻发送同步，不需要在这里同步0血量
                return;
            }
            if (preHp == curHp)
            {
                return;
            }
            preHp = curHp;

            MogoWorld.thePlayer.RpcCall("CliEntityActionReq", ID, (uint)2, curHp, (uint)0);
        }
        // 为去除警告暂时屏蔽以下代码
        //private ulong clientChkPreTime = 0;
        //private ulong biosChkPreTime = 0;
        private void CheckRate()
        {
            return;
            //为消除警告而注释以下代码
            //ulong clientCurTime = (ulong)(UnityEngine.Time.realtimeSinceStartup * 1000);
            //DateTime d = System.DateTime.Now;
            //ulong arg = (ulong)((d.DayOfYear * 24 * 3600 + d.Hour * 3600 + d.Minute * 60 + d.Second) * 1000 + d.Millisecond);
            //if (clientChkPreTime != 0 && biosChkPreTime != 0)
            //{
            //    if ((arg - biosChkPreTime) * RATE < (clientCurTime - clientChkPreTime))
            //    {//使用外挂
            //        UsedTool();
            //        return;
            //    }
            //}
            //clientChkPreTime = clientCurTime;
            //biosChkPreTime = arg;
        }

        private void SkillFailover()
        {//操作空闲时定时查询技能状态是否有清除,用于容错跳帧引起的技能状态未清除

            if (MogoWorld.inCity ||
                currSpellID == -1 ||
                deathFlag == 1)
            {
                return;
            }
            if (currentMotionState == MotionState.WALKING ||
                currentMotionState == MotionState.HIT ||
                currentMotionState == MotionState.ROLL ||
                currentMotionState == MotionState.DEAD ||
                //currentMotionState == MotionState.ATTACKING ||
                currentMotionState == MotionState.CHARGING)
            {
                return;
            }
            float t = Time.realtimeSinceStartup;
            if (t - EntityMyself.preSkillTime < 3.0f)
            {
                return;
            }
            ClearSkill();
        }

        private void TaskGuide()
        {
            // int pathPoint = taskManager.GetTaskNearestPathPoint();
            int pathPoint = taskManager.PlayerCurrentTask.pathPoint;

            if (taskManager.PlayerCurrentTask.npc == TaskManager.SkyNPCID)
            {
                taskManager.OnCloseToNPC(TaskManager.SkyNPCID);
            }
            else if (PathPoint.pathPointDic.ContainsKey(pathPoint))
            {
                //Vector3 v = PathPoint.pathPointDic[pathPoint];
                //Move();

                //motor.SetStopDistance(PathPoint.pathPointDistance[pathPoint]);

                //motor.MoveTo(v);



                Vector3 v = PathPoint.pathPointDic[pathPoint];
                int resultID = PathPoint.GetNearestPlace(Transform.position, pathPoint);
                if (resultID != 0)
                {
                    v = PathPoint.tempPathPointDic[resultID];
                    Move();
                    motor.SetStopDistance(PathPoint.tempPathPointDistance[resultID]);
                    motor.MoveTo(v);
                }
                else
                {
                    Move();
                    motor.SetStopDistance(PathPoint.pathPointDistance[pathPoint]);
                    motor.MoveTo(v);
                }
            }
            else
            {
                LoggerHelper.Debug("PathPoint " + pathPoint + " Not Exist!");
            }
        }

        public void EndTaskGuide()
        {
            ChangeMotionState(MotionState.IDLE);
        }

        private void MissionOpen(int theMission, int theLevel)
        {
            missionManager.MissionOpen(theMission, theLevel);
        }

        private void CrockBroken(Vector3 position)
        {
            ContainerBroken(6, position);
        }

        private void ChestBroken(bool easyType, bool hardType, Vector3 position)
        {
            bool judgementType = false;
            switch (curMissionLevel)
            {
                case 1:
                    judgementType = easyType;
                    break;
                default:
                    judgementType = hardType;
                    break;
            }

            if (judgementType)
                ContainerBroken(8, position);
            else
                ContainerBroken(7, position);
        }

        private void ContainerBroken(int type, Vector3 position)
        {
            missionManager.ContainerBroken(type, position);
        }

        private void GetWingBag()
        {
            RpcCall("SyncWingBagReq");
        }

        private void Withdraw(ulong ord_dbid)
        {
            RpcCall("WithdrawReq", ord_dbid);
        }

        private void DiamondMine()
        {
            RpcCall("DiamondMineReq");
        }

        private void CheckCharge()
        {
            RpcCall("CheckChargeReq");
        }

        private void Charge()
        {
#if UNITY_IPHONE
            EventDispatcher.TriggerEvent(IAPEvents.ShowIAPView);
#else
            //MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(25570));
            PlatformSdkManager.Instance.Charge(0);
#endif
        }

        private void OnMapChanged(ushort mapId)
        {
            UpdatePosition();
        }

        private void InstanceLoaded(int missionID, bool isInstance)
        {
            UpdatePosition();
            UpdateView();

            if (motor != null)
            {

                //(motor as MogoMotorMyself).SwitchSetting(false);
            }
            else
            {
                Mogo.Util.LoggerHelper.Debug("motor == null");
            }

            isInCity = !isInstance;
            if (isInCity)
            {
                //检测指引消失/出现
                EventDispatcher.TriggerEvent(Events.DirecterEvent.DirActive);

                
            }
            //if (isInCity)
            //{
            //    SetAction(-1);
            //}
            //else
            //{
            //    SetAction(0);
            //}

            LoggerHelper.Debug("JewlMailReq");
            TimerHeap.AddTimer(2000, 0, SendJewlMailReq);
        }

        public void SendJewlMailReq()
        {
            RpcCall("JewlMailReq");
        }

        override public void MainCameraCompleted()
        {
            base.MainCameraCompleted();
            //BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, false, this);
            //BillboardLogicManager.Instance.SetHead(this);
            //EventDispatcher.TriggerEvent<string, uint>
            //     (BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDNAME, name, ID);//Base里面已经加过 先注释掉这里 MaiFeo

            MogoFXManager.Instance.AddShadow(Transform.gameObject, ID);
        }

        protected void Teleport(uint targetSceneId)
        {
            LoggerHelper.Debug("Teleporting");
            RpcCall("EnterTeleportpointReq", (UInt32)targetSceneId);
        }

        protected void SetDamage(uint damageID, int action, int type, int damage)
        {
            if (damageID != ID)
                return;

            if (MogoWorld.inCity || MissionManager.HasWin)
                return;

            if (deathFlag > 0 || curHp == 0)
                return;

            RpcCall("CliEntityActionReq", ID, (uint)2, curHp < (uint)damage ? (uint)0 : curHp - (uint)damage, (uint)0);

            List<int> harm = new List<int>();
            harm.Add(type);
            harm.Add(damage);
            OnHit(action, 0, ID, harm);
        }

        public EventMessageData GetEventDoingMessage(int id)
        {
            return operationSystem.GetEventDoingMessage(id);
        }

        protected bool isLevelChangeSendMessage = false;

        public void HandleLevelChangeCheck(byte value)
        {
            if (value != level)
            {
                if (AvatarLevelData.dataMap.ContainsKey((int)value))
                {
                    Mogo.Util.LoggerHelper.Debug((uint)AvatarLevelData.dataMap[(int)value].nextLevelExp);
                    nextLevelExp = (uint)AvatarLevelData.dataMap[(int)value].nextLevelExp;
                }
                else
                    LoggerHelper.Debug("Can't Find nextLevelExp with this level: " + value);

                isLevelChangeSendMessage = true;

                if (level != 0)
                {
                    // MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[150012].content);
                    NormalMainUIViewManager.Instance.AddNormalMainUIPlaysIcon(level + 1);
                }
            }
        }

        public void HandleLevelChangeResult()
        {
            if (isLevelChangeSendMessage)
            {
                EventDispatcher.TriggerEvent(Events.OperationEvent.CheckEventOpen, this);
                isLevelChangeSendMessage = false;
            }

            if (level > 10)
                IsTimeLimitEventHasOpen = true;

            if (level > 9)
                IsAchiementHasOpen = true;
        }

        public int GetMaxCombo()
        {
            return (skillManager as PlayerSkillManager).GetMaxCombo();
        }

        public void ResetMaxCombo()
        {
            (skillManager as PlayerSkillManager).ResetMaxCombo();
        }

        public bool CheckCurrentMissionComplete()
        {
            return missionManager.CheckCurrentMissionComplete(CurMissionID, CurMissionLevel);
        }

        public int GetDailyAllEventExp()
        {
            return dailyEventSystem.GetDailyAllEventExp();
        }

        /// <summary>
        /// 获取能进的最大副本
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<int, int> GetLastMissionCanEnter()
        {
            return missionManager.GetLastMissionCanEnter();
        }

        /// <summary>
        /// 判断副本是否可进
        /// </summary>
        /// <returns></returns>
        public bool IsMissionCanEnter(int theMission, int theLevel)
        {
            return missionManager.IsMissionCanEnter(theMission, theLevel);
        }

        private void BossDie()
        {
            UnityEngine.Time.timeScale = 0.1f;
            float f = UnityEngine.Time.fixedDeltaTime;
            UnityEngine.Time.fixedDeltaTime = f * 0.1f;
            TimerHeap.AddTimer(2000, 0, (t) => { UnityEngine.Time.timeScale = 1; UnityEngine.Time.fixedDeltaTime = t; }, f);
        }

        public bool IsOccupyOpen()
        {
            return occupyTowerSystem.OccupyTowerOpen;
        }

        #endregion

        #region 体力购买

        /// <summary>
        /// VipRealState转换后实体数据
        /// </summary>
        public Dictionary<int, int> m_VipRealStateMap;
        public LuaTable VipRealState
        {
            set
            {
                Mogo.Util.Utils.ParseLuaTable(value, out m_VipRealStateMap);
                EventDispatcher.TriggerEvent(EntityMyself.ON_VIP_REAL_STATE);
            }
        }

        /// <summary>
        /// 计算剩余购买次数
        /// </summary>
        /// <returns></returns>
        public int CalBuyEnergyLastTimes()
        {
            int lastTimes = 0;

            if (PrivilegeData.dataMap.ContainsKey(MogoWorld.thePlayer.VipLevel))
            {
                int maxBuyTimes = PrivilegeData.dataMap[MogoWorld.thePlayer.VipLevel].dailyEnergyBuyLimit;
                int curBuyTimes = MogoWorld.thePlayer.m_VipRealStateMap[(int)VipRealStateEnum.DAILY_ENERGY_BUY_TIMES];
                lastTimes = maxBuyTimes - curBuyTimes;
            }

            return lastTimes > 0 ? lastTimes : 0;
        }

        /// <summary>
        /// 通过体力购买次数计算需要的钻石
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public int CalBuyEnergyDiamondCost(int times)
        {
            int currentTimes = m_VipRealStateMap[(int)VipRealStateEnum.DAILY_ENERGY_BUY_TIMES] + 1;
            int price = 0;
            for (int i = 0; i < times; i++)
            {
                if (PriceListData.dataMap[4].priceList.ContainsKey(currentTimes))
                {
                    price += PriceListData.dataMap[4].priceList[currentTimes++];
                }
                else
                {
                    return -1;
                }
            }

            return price;
        }

        private void BuyEnergy(int times)
        {
            if (PriceListData.dataMap.ContainsKey(4))
            {
                int price = CalBuyEnergyDiamondCost(times);

                // 1 = 金币；2 = 钻石；
                if (PriceListData.dataMap[4].currencyType == 1)
                {
                    if (MogoWorld.thePlayer.gold >= price)
                    {
                        if (times == 1)
                            RpcCall("BuyEnergyReq", 0, 1);
                        else
                            RpcCall("BuyEnergyReq", 1, times);
                    }
                    else
                    {
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[25104].content); // 提示金币不足
                    }
                }
                else if (PriceListData.dataMap[4].currencyType == 2)
                {
                    if (MogoWorld.thePlayer.diamond >= price)
                    {
                        if (times == 1)
                            RpcCall("BuyEnergyReq", 0, 1);
                        else
                            RpcCall("BuyEnergyReq", 1, times);
                    }
                    else
                    {
                        MogoMsgBox.Instance.ShowFloatingText(LanguageData.dataMap[25105].content); // 提示钻石不足	
                    }
                }
            }
        }

        private void OnVIPRealState()
        {
            if (EnergyUILogicManager.Instance != null && MogoUIManager.Instance.m_EnergyUI == MogoUIManager.Instance.CurrentUI)
                EnergyUILogicManager.Instance.SetVipRealState();

            if (DiamondToGoldUILogicManager.Instance != null && MogoUIManager.Instance.m_DiamondToGoldUI == MogoUIManager.Instance.CurrentUI)
                DiamondToGoldUILogicManager.Instance.SetVipRealState();

            if (EnergyNoEnoughUILogicManager.Instance != null && MogoUIManager.Instance.m_EnergyNoEnoughUI == MogoUIManager.Instance.CurrentUI)
                EnergyNoEnoughUILogicManager.Instance.SetVipRealState();

            OnPropertyChanged(ATTR_GOLD_METALLURGY_LAST_TIMES, CalGoldMetallurgyLastUseTimes());
        }

        public void VipRealStateResp(LuaTable luaTable)
        {
            VipRealState = luaTable;
        }

        #endregion

        #region 炼金

        /// <summary>
        /// 炼金RPC Request
        /// </summary>
        /// <param name="useCount"></param>
        private void GoldMetallurgy(int useCount)
        {
            RpcCall("GoldMetallurgyReq", (ushort)useCount);
        }

        /// <summary>
        /// 计算炼金剩余使用次数
        /// </summary>
        /// <returns></returns>
        public int CalGoldMetallurgyLastUseTimes()
        {
            int lastUseTimes = 0;

            if ((PrivilegeData.dataMap == null) || (PrivilegeData.dataMap.ContainsKey(VipLevel) == false))
            {
                LoggerHelper.Error("PrivilegeData no contains key = " + MogoWorld.thePlayer.VipLevel);
                return 0;
            }

            if (m_VipRealStateMap == null)
            {
                //LoggerHelper.Error("VipRealStateMap is null");
                return 0;
            }

            if (m_VipRealStateMap.ContainsKey((int)VipRealStateEnum.DAILY_GOLD_METALLURGY_TIMES) == false)
            {
                LoggerHelper.Error("VipRealStateMap no contains key " + (int)VipRealStateEnum.DAILY_GOLD_METALLURGY_TIMES);
                return 0;
            }

            int maxUseTimes = PrivilegeData.dataMap[MogoWorld.thePlayer.VipLevel].dailyGoldMetallurgyLimit;
            int curUseTimes = m_VipRealStateMap[(int)VipRealStateEnum.DAILY_GOLD_METALLURGY_TIMES];
            lastUseTimes = maxUseTimes - curUseTimes;

            return lastUseTimes >= 0 ? lastUseTimes : 0;
        }

        /// <summary>
        /// 计算炼金需要的钻石
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public int CaGoldMetallurgylDiamondCost(int times)
        {
            int GOLD_METALLURGY_ID = 11;// 炼金消耗次数定价表ID
            int currentTimes = MogoWorld.thePlayer.m_VipRealStateMap[(int)VipRealStateEnum.DAILY_GOLD_METALLURGY_TIMES] + 1;

            int cost = 0;
            for (int i = 0; i < times; i++)
            {
                if (!PriceListData.dataMap.ContainsKey(GOLD_METALLURGY_ID))
                {
                    LoggerHelper.Debug("PriceListData no contains key = " + GOLD_METALLURGY_ID);
                    return -1;
                }

                if (!PriceListData.dataMap[GOLD_METALLURGY_ID].priceList.ContainsKey(currentTimes))
                {
                    LoggerHelper.Debug("PriceListData priceList no contains key = " + currentTimes);
                    return -1;
                }

                cost += PriceListData.dataMap[GOLD_METALLURGY_ID].priceList[currentTimes++];
            }

            return cost;
        }

        private void GoldMetallurgyResp(byte flag)
        {
            // 0表示成功
            // 1表示失败
            if (flag == 0)
            {
                if (MogoUIManager.Instance.CurrentUI == MogoUIManager.Instance.m_DiamondToGoldUI)
                    MogoFXManager.Instance.AttachUIFX(10, MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, null);
            }
        }

        #endregion

        /// <summary>
        /// 计算剧情关卡扫荡剩余次数(在MissionManager调用RPC获取)
        /// </summary>
        /// <returns></returns>
        //public int CalRaidSweepLastTimes()
        //{
        //    int lastTimes = 0;
        //    if (PrivilegeData.dataMap.ContainsKey(MogoWorld.thePlayer.VipLevel) && MogoWorld.thePlayer.m_VipRealStateMap != null)
        //    {
        //        int maxTimes = PrivilegeData.dataMap[MogoWorld.thePlayer.VipLevel].dailyRaidSweepLimit;
        //        int curTimes = MogoWorld.thePlayer.m_VipRealStateMap[(int)VipRealStateEnum.DAILY_RAID_SWEEP_TIMES];
        //        lastTimes = maxTimes - curTimes;
        //    }

        //    return lastTimes >= 0 ? lastTimes : 0;
        //}

        public void GetServerTimeStampReq()
        {
            RpcCall("GetServerTimeReq", (ushort)1);
            //GetServerTickReq();
        }

        public void GetServerTimeEscapeReq()
        {
            RpcCall("GetServerTimeReq", (ushort)2);
        }

        public void GetServerTimeZoneReq()
        {
            RpcCall("GetServerTimeReq", (ushort)4);
        }

        public void GetServerTickReq()
        {
            syncInfo[0] = (uint)(UnityEngine.Time.realtimeSinceStartup * 1000);

            RpcCall("GetServerTimeReq", (ushort)5);
            if (send.Count < sn)
            {
                send.Add(UnityEngine.Time.realtimeSinceStartup);
            }
            else
            {
                send.RemoveAt(0);
                send.Add(UnityEngine.Time.realtimeSinceStartup);
            }
        }

        private void PrintList(List<int> l)
        {
            string s = "";
            for (int i = 0; i < l.Count; i++)
            {
                s = s + l[i] + ",";
            }
            LoggerHelper.Error("list " + s);
        }

        private List<float> send = new List<float>();
        private List<uint> recv = new List<uint>();
        private List<float> recvClt = new List<float>();
        private float r_base = 0;
        private float delta_r_base = 0;
        private float sn = 5;

        private int clientPreTime = 0; //前端上次同步时间点，秒
        private int serverPreTime = 0; //后端上次同步时间点, 秒

        /*
         * 用RpcCall("GetServerTimeReq", (ushort)5);<=> GetServerTimeResp  预估前端时刻对应的服务器时刻
         * syncInfo[0] 这次req前端时刻  
         * syncInfo[1] 这次resp前端时刻  
         * syncInfo[2] 史上最精确req前端时刻  
         * syncInfo[3] 史上最精确resp前端时刻  
         * syncInfo[4] 史上最精确(resp-req)前端时刻  
         * syncInfo[5] 史上最精确前端时刻对应的服务器tick
         */
        public uint[] syncInfo = { 0, 0, 0, 0, 99999999, 0 };

        private float RATE = 1.5f; //允许速率
        public void GetServerTimeResp(ushort handleCode, UInt32 arg)
        {
            switch ((int)handleCode)
            {
                case 1:
                    //CheckTime(arg);
                    int clientCurTime = (int)UnityEngine.Time.realtimeSinceStartup;
                    clientPreTime = clientCurTime;
                    serverPreTime = (Int32)arg;
                    MogoTime.Instance.SetTimeFromServer((int)arg);
                    break;
                case 2:
                    MogoTime.Instance.SetEscapeTimeFromServer((int)arg);
                    break;
                case 3:
                    MogoTime.Instance.SetEscapeTimeFromServer((int)arg);
                    break;
                case 4:
                    MogoTime.Instance.SetTimeZone((int)arg);
                    break;
                case 5:
                    syncInfo[1] = (uint)(UnityEngine.Time.realtimeSinceStartup * 1000);
                    uint newDiaotaClientT = syncInfo[1] - syncInfo[0];
                    if (newDiaotaClientT < syncInfo[4])
                    {//始终取最小diaotaT的服务器时间为最准确服务器时间
                        syncInfo[2] = syncInfo[0];
                        syncInfo[3] = syncInfo[1] - newDiaotaClientT / 2;
                        syncInfo[4] = newDiaotaClientT;
                        syncInfo[5] = arg;
                    }
                    CheckTime(arg);
                    TimerHeap.AddTimer(3000, 0, GetServerTickReq);
                    break;
            }
        }

        private int toolCnt = 0;
        private int kickCnt = 0;
        private void CheckTime(uint svrTime)
        {
            recvClt.Add(UnityEngine.Time.realtimeSinceStartup);
            recv.Add(svrTime);
            if (recv.Count < 2)
            {
                return;
            }
            float great = recvClt[1] - send[0];
            float m = (float)(recv[1] - recv[0]) * 0.001f;
            float less = send[1] - recvClt[0];
            //if ((great + 0.99f) >= m)// && m >= (less - 0.99f))
            if(m > less)
            {
                toolCnt = 0;
                recv.RemoveAt(0);
                send.RemoveAt(0);
                recvClt.RemoveAt(0);
                return;
            }

            toolCnt++;
            LoggerHelper.Error("may be used tool cnt: " + toolCnt + "c1 c2 " + send[0] + "," + send[1] + "c1` c2` " + recvClt[0] + "," + recvClt[1] + "s1 s2" + recv[0] + "," + recv[1]);
            recv.RemoveAt(0);
            send.RemoveAt(0);
            recvClt.RemoveAt(0);
            if (toolCnt >= 2)
            {
                toolCnt = 0;
                recv.Clear();
                send.Clear();
                recvClt.Clear();
                if (UnityEngine.Time.realtimeSinceStartup - MogoWorld.StartTime < 8)
                {//游戏从暂停恢复8秒内不检测外挂
                    return;
                }
                UsedTool();
            }
        }

        private void UsedTool()
        {
            AutoFight = AutoFightState.IDLE;
            kickCnt++;
            if (kickCnt >= 3)
            {
                RpcCall("MarkCheat", 1);
                MogoGlobleUIManager.Instance.Info(LanguageData.GetContent(200025),
                                                LanguageData.GetContent(25561),
                                                "",
                                                -1,
                                                ButtonBgType.Blue,
                                                ButtonBgType.Brown,
                                                () => { PlatformSdkManager.Instance.RestartGame(); });
                return;
            }
            RpcCall("MarkCheat", 0);
            MogoGlobleUIManager.Instance.Info(LanguageData.GetContent(200025),
                                                LanguageData.GetContent(25561),
                                                "",
                                                -1,
                                                ButtonBgType.Blue,
                                                ButtonBgType.Brown,
                                                () => { /*PlatformSdkManager.Instance.RestartGame();*/ });
        }

        public override void FixedErr()
        {//修正错误状态，用于回城容错
            if (!MogoWorld.inCity)
            {
                return;
            }
            if (currentMotionState != MotionState.IDLE && currentMotionState != MotionState.WALKING)
            {
                currentMotionState = MotionState.IDLE;
            }
            currSpellID = -1;
            stiff = false;
            knockDown = false;
            hitAir = false;
            hitGround = false;
            charging = false;
            base.FixedErr();
            if (animator == null)
            {
                return;
            }
            if (deathFlag == 0 && animator.GetInteger("Action") == ActionConstants.DIE)
            {
                Revive();
            }
        }

        public bool CheckCurrentMissionEnterable(int missionID, int level)
        {
            return missionManager.CheckCurrentMissionEnterable(missionID, level);
        }

        public bool CheckCurrentMissionEasyComplete(int missionID)
        {
            return missionManager.CheckCurrentMissionEasyComplete(missionID);
        }

        /// <summary>
        /// 获取限时活动中关于体力获取的活动(体力不足引导系统)
        /// </summary>
        /// <returns></returns>
        public List<EnergyLimitActivityData> GetEnergyLimitActivityDataList()
        {
            return operationSystem.GetEnergyLimitActivityDataList();
        }

        public void ClearCmdCache()
        {
            (battleManger as PlayerBattleManager).CleanPreSkill();
        }

        //private void UpdateDressWing()
        //{
        //    int dressed = int.Parse((string)wingBag["1"]);
        //    if (dressed <= 0)
        //    {
        //        return;
        //    }
        //    if (Actor == null)
        //    {
        //        return;
        //    }
        //    WingData d = WingData.dataMap.Get(dressed);
        //    string[] ms = d.modes.Split(',');
        //    string path = ms[(int)vocation - 1];
        //    (Actor as ActorMyself).AddWing(path, () => { });
        //}

        public void PreviewWing(int wid)
        {
            (Actor as ActorMyself).RemoveWing();
            WingData d = WingData.dataMap.Get(wid);
            string[] ms = d.modes.Split(',');
            string path = ms[(int)vocation - 1];
            (Actor as ActorMyself).AddWing(path, () => { (Actor as ActorMyself).SetLayer(10); });
        }

        public void ResetPreviewWing()
        {
            (Actor as ActorMyself).RemoveWing();
        }
        
        
        #region 血条控制

        public void SetCampControl(int missionID, bool isInstance)
        {
            switch (MapData.dataMap.Get(missionID).type)
            {
                case MapType.OCCUPY_TOWER:
                    BillboardLogicManager.Instance.GlobalBillBoardCurrentType = GlobalBillBoardType.OccupyTower;

                    // 自己
                    BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, true, View.HeadBloodColor.Blue, true);
                    BillboardViewManager.Instance.SetHead(this, BillboardViewManager.HeadStatus.PVP, BillboardViewManager.PVPCamp.CampOwn);
                    break;
                default:
                    BillboardLogicManager.Instance.GlobalBillBoardCurrentType = GlobalBillBoardType.Normal;

                    // 自己
                    BillboardLogicManager.Instance.AddInfoBillboard(ID, Transform, this, false);
                    BillboardViewManager.Instance.SetHead(this, BillboardViewManager.HeadStatus.Normal);
                    break;
            }
        }

        #endregion


        #region 血瓶显示与否

        public void CheckShowBottle(int missionID, bool isInstance)
        {
            switch (MapData.dataMap.Get(missionID).type)
            {
                case MapType.FOGGYABYSS:
                case MapType.OCCUPY_TOWER:
                    MainUIViewManager.Instance.SetHpBottleVisible(false);
                    MogoWorld.thePlayer.PlayFx(6029);
                    break;
            }
        }

        #endregion
    }
}