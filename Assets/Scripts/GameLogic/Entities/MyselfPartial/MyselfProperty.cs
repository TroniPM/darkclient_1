using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Task;
using Mogo.Mission;
using Mogo.Util;
using Mogo.GameData;

namespace Mogo.Game
{
    public partial class EntityMyself
    {
        #region 私有属性

        private DoorOfBurySystem doorOfBurySystem;
        private BodyEnhanceManager bodyenhanceManager;
        public TaskManager taskManager;
        private MissionManager missionManager;
        private RuneManager runeManager;
        private FriendManager friendManager;
        public MarketManager marketManager;
        private TowerManager towerManager;
        private SanctuaryManager sanctuaryManager;
        private ArenaManager arenaManager;
        private OperationSystem operationSystem;
        private DailyEventSystem dailyEventSystem;
        private RankManager rankManager;
        public CampaignSystem campaignSystem;
        public FumoManager fumoManager;
        public WingManager wingManager;
        public RewardManager rewardManager;
        private OccupyTowerSystem occupyTowerSystem;

        //private int lastRollAttackActionID = 0;
        //private float lastRollAttackTime = 0.0f;
        // 为去除警告暂时屏蔽以下代码
        //private float lastKeyDownTime = 0.0f;
        private Vector3 prePos = Vector3.zero;
        private uint timerID = uint.MaxValue;
        private uint checkDmgID;
        private uint syncHpTimerID = uint.MaxValue;
        private uint skillFailoverTimer = uint.MaxValue;
        // 为去除警告暂时屏蔽以下代码
        //private uint rateTimer = 0;

        private uint m_gold;
        private uint m_diamond;

        private uint m_airDamage;
        // private uint m_nextLevelExp;
        private uint m_airDefense;
        private uint m_allElementsDamage;
        private uint m_allElementsDefense;
        private uint m_antiCrit;
        private uint m_atk;
        private uint m_antiDefense;
        private uint m_antiTrueStrike;
        private uint m_crit;
        private uint m_critExtraAttack;
        private uint m_cdReduce;
        // private uint m_curHp;
        private uint m_def;
        private uint m_earthDamage;
        private uint m_fireDamage;
        private uint m_earthDefense;
        private uint m_fireDefense;
        private uint m_hit;
        private uint m_maxEnery;
        private uint m_pvpAddition;
        private uint m_pvpAnti;
        private uint m_speedAddRate;
        private uint m_trueStrike;
        private uint m_waterDamage;
        private uint m_waterDefense;

        private ushort m_sceneId;
        private ushort m_imap_id;
        private byte m_deathFlag;
        private uint m_anger;
        private uint m_curTask;

        private byte m_loginDays;

        private byte m_loginIsReward;

        private byte m_baseflag;

        private UInt32 m_buyHotSalesLastTime;

        private LuaTable m_wingBag;
        // 为去除警告暂时屏蔽以下代码
        //private System.UInt64 dummyThinkCDEnd = 0;

        #endregion

        #region 公有属性

        public InventoryManager inventoryManager; //背包管理器
        public Dictionary<EquipSlot, ItemEquipment> equips = new Dictionary<EquipSlot, ItemEquipment>(); //身上装备表
        /// <summary>
        /// 道具栏的大小
        /// </summary>
        public byte inventorySize { get; set; }

        public TaskData CurrentTask
        {
            get { return taskManager.PlayerCurrentTask; }
        }

        public override uint curHp
        {
            get
            {
                return base.curHp;
            }
            set
            {
                CheckDmgBase();
                //先判断不等于的时候修改，修改完后确认是玩家修改就trigger
                if (base.curHp != value)
                {
                    if (base.curHp != 0)
                        GuideSystem.Instance.TriggerEvent<uint>(GlobalEvents.ChangeHp, value);

                    base.curHp = value;
                    OnPropertyChanged(BattleAttr.HEALTH, base.curHp);

                    ProcessAutoFightDrinkRedTea();
                    CalcuDmgBase();

                    if (BillboardViewManager.Instance)
                        BillboardViewManager.Instance.SetBillboardBlood(base.PercentageHp, ID);
                }
            }
        }

        /// <summary>
        /// 金币
        /// </summary>
        public uint gold
        {
            get { return m_gold; }
            set
            {
                CheckGetSomeThing(2, m_gold, value);

                m_gold = value;
                OnPropertyChanged(ATTR_GLOD, m_gold);

                if (MenuUIViewManager.Instance != null)
                {
                    MenuUILogicManager.Instance.SetInventoryGold((int)m_gold);
                }
                if (SkillUIViewManager.Instance != null)
                {
                    SkillUIViewManager.Instance.SetSkillUIGold(String.Format("{0}", m_gold));
                }
                if (EquipExchangeUIViewManager.Instance != null)
                {
                    EquipExchangeUIViewManager.Instance.SetGold(value.ToString());
                }
                if (runeManager != null)
                {
                    runeManager.UpdateGold();
                }
                if (bodyenhanceManager != null)
                {
                    //Mogo.Util.LoggerHelper.Debug("gold:"+value);
                    bodyenhanceManager.UpDateGold(value);
                }
            }
        }

        /// <summary>
        /// 钻石(RMB)
        /// </summary>
        public uint diamond
        {
            get { return m_diamond; }
            set
            {
                CheckGetSomeThing(3, m_diamond, value);

                m_diamond = value;
                OnPropertyChanged(ATTR_DIAMOND, m_diamond);

                if (MenuUIViewManager.Instance != null)
                {
                    MenuUILogicManager.Instance.SetInventoryDiamond((int)m_diamond);
                }
                if (MarketView.Instance != null)
                {
                    MarketView.Instance.UpdateDiamond((int)m_diamond);
                }
                if (runeManager != null)
                {
                    runeManager.UpdateDiamond();
                }
            }
        }

        public uint airDamage
        {
            get { return m_airDamage; }
            set
            {
                m_airDamage = value;
                OnPropertyChanged(BattleAttr.AIR_DAMAGE, m_airDamage);
            }
        }

        public uint airDefense
        {
            get { return m_airDefense; }
            set
            {
                m_airDefense = value;
                OnPropertyChanged(BattleAttr.AIR_DEFENSE, m_airDefense);
            }
        }

        public uint allElementsDamage
        {
            get { return m_allElementsDamage; }
            set
            {
                m_allElementsDamage = value;
                OnPropertyChanged(BattleAttr.ALL_ELEMENTS_DAMAGE, m_allElementsDamage);
            }
        }

        public uint allElementsDefense
        {
            get { return m_allElementsDefense; }
            set
            {
                m_allElementsDefense = value;
                OnPropertyChanged(BattleAttr.ALL_ELEMENTS_DEFENSE, m_allElementsDefense);
            }
        }

        public uint antiCrit
        {
            get { return m_antiCrit; }
            set
            {
                m_antiCrit = value;
                OnPropertyChanged(BattleAttr.ANTI_CRIT, m_antiCrit);
            }
        }

        public uint atk
        {
            get { return (uint)(m_atk + deviator); }
            set
            {
                CheckDmgBase();
                m_atk = (uint)(value - deviator);
                OnPropertyChanged(BattleAttr.ATTACK, atk);
                CalcuDmgBase();
            }
        }

        public uint antiDefense
        {
            get { return m_antiDefense; }
            set
            {
                m_antiDefense = value;
                OnPropertyChanged(BattleAttr.ANTI_DEFENSE, m_antiDefense);
            }
        }

        public uint antiTrueStrike
        {
            get { return m_antiTrueStrike; }
            set
            {
                m_antiTrueStrike = value;
                OnPropertyChanged(BattleAttr.ANTI_TRUE_STRIKE, m_antiTrueStrike);
            }
        }

        public uint crit
        {
            get { return m_crit; }
            set
            {
                m_crit = value;
                OnPropertyChanged(BattleAttr.CRIT, m_crit);
            }
        }

        public uint critExtraAttack
        {
            get { return m_critExtraAttack; }
            set
            {
                m_critExtraAttack = value;
                OnPropertyChanged(BattleAttr.CRIT_EXTRA_ATTACK, m_critExtraAttack);
            }
        }

        public uint cdReduce
        {
            get { return m_cdReduce; }
            set
            {
                m_cdReduce = value;
                OnPropertyChanged(BattleAttr.CD_REDUCE, m_cdReduce);
            }
        }

        public uint def
        {
            get { return (uint)(m_def + deviator); }
            set
            {
                CheckDmgBase();
                m_def = (uint)(value - deviator);
                OnPropertyChanged(BattleAttr.DEFENSE, def);
                CalcuDmgBase();
            }
        }

        public uint earthDamage
        {
            get { return m_earthDamage; }
            set
            {
                m_earthDamage = value;
                OnPropertyChanged(BattleAttr.EARTH_DAMAGE, m_earthDamage);
            }
        }

        public uint fireDamage
        {
            get { return m_fireDamage; }
            set
            {
                m_fireDamage = value;
                OnPropertyChanged(BattleAttr.FIRE_DAMAGE, m_fireDamage);
            }
        }

        public uint earthDefense
        {
            get { return m_earthDefense; }
            set
            {
                m_earthDefense = value;
                OnPropertyChanged(BattleAttr.EARTH_DEFENSE, m_earthDefense);
            }
        }


        public uint fireDefense
        {
            get { return m_fireDefense; }
            set
            {
                m_fireDefense = value;
                OnPropertyChanged(BattleAttr.FIRE_DEFENSE, m_fireDefense);
            }
        }


        public uint hit
        {
            get { return m_hit; }
            set
            {
                m_hit = value;
                OnPropertyChanged(BattleAttr.HIT, m_hit);
            }
        }

        public uint maxEnery
        {
            get { return m_maxEnery; }
            set
            {
                m_maxEnery = value;

                if (MenuUIViewManager.Instance != null)
                {
                    MenuUIViewManager.Instance.SetPlayerInfoEnergy(PercentageEnergy);
                    MenuUIViewManager.Instance.SetPlayerInfoEnergyNum(energy + "/" + m_maxEnery);
                }

                // to do
            }
        }

        public uint pvpAddition
        {
            get { return m_pvpAddition; }
            set
            {
                m_pvpAddition = value;
                OnPropertyChanged(BattleAttr.PVP_ADDITION, m_pvpAddition);
            }
        }

        public uint pvpAnti
        {
            get { return m_pvpAnti; }
            set
            {
                m_pvpAnti = value;
                OnPropertyChanged(BattleAttr.PVP_ANTI, m_pvpAnti);
            }
        }

        public uint speedAddRate
        {
            get { return m_speedAddRate; }
            set
            {
                m_speedAddRate = value;
                OnPropertyChanged(BattleAttr.SPEED_ADD_RATE, m_speedAddRate);
            }
        }

        public uint trueStrike
        {
            get { return m_trueStrike; }
            set
            {
                m_trueStrike = value;
                OnPropertyChanged(BattleAttr.TRUE_STRIKE, m_trueStrike);
            }
        }

        public uint waterDamage
        {
            get { return m_waterDamage; }
            set
            {
                m_waterDamage = value;
                OnPropertyChanged(BattleAttr.WATER_DAMAGE, m_waterDamage);
            }
        }

        public uint waterDefense
        {
            get { return m_waterDefense; }
            set
            {
                m_waterDefense = value;
                OnPropertyChanged(BattleAttr.WATER_DEFENSE, m_waterDefense);
            }
        }

        /// <summary>
        /// 玩家所在场景地图id
        /// </summary>
        public ushort sceneId
        {
            get { return m_sceneId; }
            set
            {
                if (m_sceneId == value)//重复删除
                    return;
                var orgId = m_sceneId;
                m_sceneId = value;
                LoggerHelper.Info("sceneId: " + m_sceneId);

                if (MainUILogicManager.Instance != null
                    && MainUILogicManager.Instance.hasShowBossBlood)
                    MainUILogicManager.Instance.hasShowBossBlood = false;
                if (orgId != m_sceneId)
                {
                    Anger = 0;
                    angerStep = 0;
                    if (MainUIViewManager.Instance != null)
                    {
                        //MainUIViewManager.Instance.NormalAttackShowAsNormal();
                        //MainUIViewManager.Instance.SetNormalAttackIconByID(MogoWorld.thePlayer.GetEquipSubType());
                        MainUIViewManager.Instance.ShowSpecialSkillIcon(false);
                        RemoveFx(910136);
                    }
                }
                //MogoWorld.SwitchScene(orgId, m_sceneId);
                RealSwitchScene(orgId, m_sceneId);
                //TimerHeap.AddTimer(300, 0, RealSwitchScene, orgId, m_sceneId);
            }
        }

        //处理新手时不先加主城
        private void RealSwitchScene(ushort orgId, ushort newId)
        {
            if ((baseflag % 2) == 1)
            {//新手不进主城
                return;
            }
            MogoWorld.SwitchScene(orgId, newId);
        }
        /// <summary>
        /// 玩家所在的地图分线id
        /// </summary>
        public ushort imap_id
        {
            get { return m_imap_id; }
            set
            {
                if (m_imap_id == value)//重复删除
                    return;
                if (m_imap_id != value)
                {
                    MogoWorld.ClearEntities();
                }
                m_imap_id = value;
                LoggerHelper.Info("imap_id: " + m_imap_id);
                if (sceneId != 0 && MapData.dataMap.Get(sceneId).type == MapType.ClimbTower)
                {
                    EventDispatcher.TriggerEvent<ushort>(Events.OtherEvent.MapIdChanged, value);
                }
                EventDispatcher.TriggerEvent<int>(Events.GearEvent.FlushGearState, (int)InstanceIdentity.TOWER);
            }
        }

        public uint taskMain
        {
            get
            {
                return m_curTask;
            }
            set
            {
                m_curTask = value;
                // taskManager.SyncTask((int)m_curTask);
            }
        }

        public byte deathFlag
        {
            get
            {
                return m_deathFlag;
            }
            set
            {
                m_deathFlag = value;
                if (m_deathFlag > 0)
                {
                    if (sceneId != MogoWorld.globalSetting.homeScene
                        && MapData.dataMap.Get(sceneId).type != MapType.ARENA
                        && MapData.dataMap.Get(sceneId).type != MapType.ASSAULT
                        && MapData.dataMap.Get(sceneId).type != MapType.BURY
                        && MapData.dataMap.Get(sceneId).type != MapType.WORLDBOSS
                        && MapData.dataMap.Get(sceneId).type != MapType.FOGGYABYSS
                        // 处理同归于尽
                        && !MissionManager.HasWin
                        //这一条要在最后
                        && MissionData.GetReviveTimesByMission(CurMissionID, CurMissionLevel) > 0)
                    {
                        missionManager.GetRebornTimesReq();
                    }

                    if (sceneId != MogoWorld.globalSetting.homeScene
                        && MapData.dataMap.Get(sceneId).type == MapType.FOGGYABYSS)
                    {
                        missionManager.FoggyAbyssDead();
                    }

                    if (MapData.IsSceneShowDeadTip(sceneId))
                        HasDeadBefore = true;
                    else
                        HasDeadBefore = false;
                    Anger = 0;
                    this.OnDeath(-1);
                    GuideSystem.Instance.TriggerEvent(GlobalEvents.Death);
                    EventDispatcher.TriggerEvent(Events.AIEvent.SomeOneDie, factionFlag, ID);

                }
            }
        }

        protected bool hasDeadBefore;
        public bool HasDeadBefore
        {
            get { return hasDeadBefore; }
            set { hasDeadBefore = value; }
        }


        public byte login_days
        {
            get
            {
                return m_loginDays;
            }

            set
            {
                if (m_loginDays != value)
                {
                    IsLoginFirstShow = true;
                    m_loginDays = value;
                    LoginRewardUILogicManager.Instance.lastPage = (int)value;
                }
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
                if (animator == null)
                {
                    return;
                }
                byte f = (byte)(m_stateFlag & 1);
                if (f != deathFlag)
                {
                    if (f == 0)
                    {
                        Revive();
                    }
                    deathFlag = f;
                }
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
                SetCampControl(m_sceneId, true);
                //Mogo.Util.LoggerHelper.Debug("kevin factionFlag I:" + m_factionFlag);
            }
        }

        public byte login_is_reward
        {
            get
            {
                return m_loginIsReward;
            }

            set
            {
                m_loginIsReward = value;
                if (value == 0)
                {
                    IsLoginRewardHasGot = false;
                }
                else
                {
                    IsLoginRewardHasGot = true;
                }
            }
        }

        public byte baseflag
        {
            get
            {
                return m_baseflag;
            }

            set
            {
                m_baseflag = value;
                if (value != 0)
                {
                    //Debug.LogError("name:" + name);
                    IsNewPlayer = true;

                    //PlatformSdkManager.Instance.CreateRoleLog(name, SystemConfig.Instance.SelectedServer.ToString());
                }
                   
            }
        }

        public UInt32 buyHotSalesLastTime
        {
            get
            {
                return m_buyHotSalesLastTime;
            }

            set
            {
                LoggerHelper.Debug(value, true, 11);
                m_buyHotSalesLastTime = value;
            }
        }

        private uint reduceTimer = 0;
        public uint Anger
        {
            get
            {
                return m_anger;
            }
            set
            {
                m_anger = value;
                if (MainUIViewManager.Instance != null)
                {
                    MainUIViewManager.Instance.SetPlayerAnger((float)m_anger / 200.0f);
                }
                if (BillboardViewManager.Instance)
                {
                    BillboardViewManager.Instance.SetBillboardAnger((float)m_anger / 200.0f, ID);
                }

                if (m_anger >= 200)
                {
                    m_anger = 200;
                    MainUIViewManager.Instance.ShowSpecialSkillIcon(true);
                }
                if (m_anger <= 0)
                {
                    if (skillManager != null)
                    {
                        (skillManager as PlayerSkillManager).isAnger = false;
                        (skillManager as PlayerSkillManager).ClearComboSkill();
                    }
                    if (MainUIViewManager.Instance != null)
                    {
                        //MainUIViewManager.Instance.NormalAttackShowAsNormal();
                        MainUIViewManager.Instance.SetNormalAttackIconByID(MogoWorld.thePlayer.GetEquipSubType());
                    }
                    RemoveFx(910136);
                    TimerHeap.DelTimer(reduceTimer);
                }
            }
        }

        public void EnterAngerSt()
        {
            RpcCall("StartUseAnger");
            (skillManager as PlayerSkillManager).isAnger = true;
            reduceTimer = TimerHeap.AddTimer(10, 100, ReduceAnger);
            MainUIViewManager.Instance.NormalAttackShowAsPower();
            MainUIViewManager.Instance.ShowSpecialSkillIcon(false);
            PlayFx(910136);
            PlayFx(910135);
        }

        private void ReduceAnger()
        {
            if (Anger < 1)
            {
                Anger = 0;
                //TimerHeap.DelTimer(reduceTimer);
                return;
            }
            Anger -= 1;
        }

        public LuaTable wingBag
        {
            get
            {
                return m_wingBag;
            }
            set
            {
                if (m_wingBag == null && ((LuaTable)value["2"]).Count > 0)
                {
                    NormalMainUIViewManager.Instance.IsWingIconHasOpened = true;
                }
                if (m_wingBag != null && ((LuaTable)m_wingBag["2"]).Count == 0 && ((LuaTable)value["2"]).Count == 1)
                {
                    NormalMainUIViewManager.Instance.AddWingPlaysIcon();
                    NormalMainUIViewManager.Instance.IsWingIconHasOpened = true;
                }
                m_wingBag = value;
                //UpdateDressWing();
                if (wingManager != null)
                {
                    wingManager.UpdateWing(wingBag);
                }
            }
        }

        #endregion
    }
}
