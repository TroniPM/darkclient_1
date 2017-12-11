using UnityEngine;
using System;
using System.Collections.Generic;

using Mogo.FSM;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
using Mogo.Task;

namespace Mogo.Game
{
    public partial class EntityParent
    {
        #region 公共变量
        public bool stiff = false;
        public bool knockDown = false;
        public bool hitAir = false;
        public bool hitGround = false;
        public Quaternion preQuaternion;
        public bool charging = false;

        public uint hitTimerID = 0;
        public uint revertHitTimerID = 0;
        public uint delayAttackTimerID = 0;
        public bool breakAble = true;

        #region 属性部分也许可以放到下一层子类
        public UInt32 ID = 0;
        public UInt64 dbid = 0;

        public const string ATTR_NAME = "name";
        public const string ATTR_EXP = "PercentageExp";
        public const string ATTR_ENERGY = "PercentageEnergy";
        public const string ATTR_LEVEL = "level";
        public const string ATTR_VIP_LEVEL = "VipLevel";
        public const string ATTR_HP_COUNT = "hpCount";
        public const string ATTR_CHARGE_SUM = "chargeSum";
        public const string ATTR_HP = "PercentageHp";
        public const string ATTR_HPSTRING = "HpString";
        public const string ATTR_ENERGYSTRING = "EnergyString";
        public const string ATTR_SPEED = "speed";
        public const string ATTR_HEAD_ICON = "HeadIcon";
        public const string ATTR_ARENIC_GRADE = "arenicGrade";
        public const string ATTR_ARENIC_CREDIT = "arenicCredit";
        public const string ATTR_FIGHT_FORCE = "fightForce";
        public const string ATTR_GOLD_METALLURGY_LAST_TIMES = "goldMetallurgyLastTimes";
        public const string ATTR_GLOD = "gold";
        public const string ATTR_DIAMOND = "diamond";
        public const int DIZZY_STATE = 1 << 1;
        public const int IMMOBILIZE_STATE = 1 << 3;
        public const int SILENT_STATE = 1 << 4;
        public const int IMMUNITY_STATE = 1 << 10;
        public const int NO_HIT_STATE = 1 << 11;

        /// <summary>
        /// 用于特殊道具是否已经初始化用，继而用于统一显示获得某个物品
        /// </summary>
        protected HashSet<int> m_propHasInitSet = new HashSet<int>();

        private string m_name;
        private uint m_exp;
        private uint m_nextLevelExp = 1;
        private uint m_energy;
        private uint m_maxEnergy;
        private byte m_level;
        private byte m_VipLevel;
        private byte m_buyCount;
        private ushort m_arenicGrade;
        private uint m_fightForce;
        private uint m_arenicCredit;
        private byte m_hpCount;
        private uint m_chargeSum;
        protected uint m_curHp;
        protected uint m_difficulty;
        private uint m_hp = 1;
        private float m_speed;
        public int deviator = 0;

        protected ulong m_stateFlag = 0;
        public bool canCastSpell = true;
        public bool canBeHit = true;
        public bool direction = false; //移动是否反方向
        public float moveSpeedRate = 1; //移动速率
        public bool canMove = true; //能否移动
        public bool immuneShift = false;//是否免疫受击位移等不受控制状态(眩晕，定身，魅惑)

        public float gearMoveSpeedRate = 1;

        public byte m_factionFlag = 0;
        public UInt32 m_ownerEid = 0;

        public float aiRate = 1;

        public int angerStep = 0;

        public int m_spawnPointCfgId;//所在出生点配置ID

        public int m_currentSee = 0;//当前视野范围

        virtual public ulong stateFlag
        {
            get
            {
                return m_stateFlag;
            }
            set
            {
                ////眩晕
                //CheckDizzy(value);

                ////定身
                //CheckImmobilize(value);

                ////无法被击
                //CheckCanBeHitOrNot(value);

                ////免疫
                //CheckImmunity(value);
                StateChange(value);
                m_stateFlag = value;
            }
        }

        #region 装备同步
        protected uint m_loadedWeapon;
        protected uint m_loadedCuirass;
        protected uint m_loadedArmguard;
        protected uint m_loadedLeg;
        #endregion 装备同步

        public virtual string name
        {
            get { return m_name; }
            set
            {
                m_name = value;
                //EventDispatcher.TriggerEvent<string, uint>
                //(BillboardLogicManager.BillboardLogicEvent.UPDATEBILLBOARDNAME, name, ID);
                OnPropertyChanged(ATTR_NAME, value);
            }
        }

        /// <summary>
        /// 当前经验
        /// </summary>
        public uint exp
        {
            get { return m_exp; }
            set
            {
                CheckGetSomeThing(1, m_exp, value);

                m_exp = value;
                OnPropertyChanged(ATTR_EXP, PercentageExp);
            }
        }

        /// <summary>
        /// 升级所需经验：计算经验在base做
        /// </summary>
        public uint nextLevelExp
        {
            get { return m_nextLevelExp; }
            set
            {
                m_nextLevelExp = value;
                OnPropertyChanged(ATTR_EXP, PercentageExp);
            }
        }

        public float PercentageExp
        {
            get
            {
                return (float)exp / (float)(nextLevelExp == 0 ? 1 : nextLevelExp);

            }
        }


        /// <summary>
        /// 当前体力
        /// </summary>
        public uint energy
        {

            get { return m_energy; }
            set
            {
                CheckGetSomeThing(6, m_energy, value);


                m_energy = value;
                OnPropertyChanged(ATTR_ENERGYSTRING, EnergyString);
                OnPropertyChanged(ATTR_ENERGY, PercentageEnergy);
            }
        }

        protected void CheckGetSomeThing(int itemId, uint oldValue, uint newValue)
        {
            Mogo.Util.LoggerHelper.Debug("oldValue:" + oldValue);
            Mogo.Util.LoggerHelper.Debug("newValue:" + newValue);
            if (!m_propHasInitSet.Contains(itemId))
            {
                m_propHasInitSet.Add(itemId);
                return;
            }

            if (!MogoWorld.thePlayer.isInCity) return;

            if (itemId == 99)
            {
                if (newValue > oldValue)
                    InventoryManager.Instance.ShowFightPowerChange(newValue - oldValue, true);
                else if (newValue < oldValue)
                    InventoryManager.Instance.ShowFightPowerChange(oldValue - newValue, false);
            }
            else if (newValue > oldValue)
            {
                uint change = newValue - oldValue;
                InventoryManager.Instance.ShowGetSomething(itemId, change);
                GuideSystem.Instance.TriggerEvent<uint>(GlobalEvents.ArenicCredit, change);
            }

        }

        /// <summary>
        /// 体力上限：角色等级
        /// </summary>
        public uint maxEnergy
        {
            get
            {
                if (AvatarLevelData.dataMap.ContainsKey(level))
                    m_maxEnergy = (uint)AvatarLevelData.dataMap[level].maxEnergy;
                return m_maxEnergy;
            }
            set
            {
                m_maxEnergy = value;
                OnPropertyChanged(ATTR_ENERGY, PercentageEnergy);

                if (MenuUIViewManager.Instance != null)
                {
                    MenuUIViewManager.Instance.SetPlayerInfoEnergy(PercentageEnergy);
                    MenuUIViewManager.Instance.SetPlayerInfoEnergyNum(energy + "/" + m_maxEnergy);
                }
            }
        }

        public float PercentageEnergy
        {
            get
            {
                return ((float)energy) / (float)(maxEnergy == 0 ? 1 : maxEnergy);
            }
        }

        public string EnergyString
        {
            get
            {
                return energy.ToString();
            }
        }

        /// <summary>
        /// 角色等级
        /// </summary>
        public virtual byte level
        {
            get { return m_level; }
            set
            {
                //先判断不等于的时候修改，修改完后确认是玩家修改就trigger
                if (m_level != value)
                {
                    // 赋值前判断，这里的等级已经变成virtual，其实应该在MyselfProperty重写level
                    if (this.GetType() == typeof(EntityMyself))
                    {
                        var entity = this as EntityMyself;
                        entity.HandleLevelChangeCheck(value);

                        if (m_level != 0)
                        {
                            if (sfxHandler)
                                sfxHandler.HandleFx(6012);
                            //MogoGlobleUIManager.Instance.ShowUpgreateSign(true);
                            MogoFXManager.Instance.AttachUpgrateFX();
                            GuideSystem.Instance.TriggerEvent<byte>(GlobalEvents.ChangeLevel, value);
                            TipManager.Instance.OnLevelUp(value);
                            PlatformSdkManager.Instance.RoleLevelLog(MogoWorld.thePlayer.name, SystemConfig.Instance.SelectedServer.ToString());
                            if ((this as EntityMyself).marketManager != null)
                            {
                                (this as EntityMyself).marketManager.GiftRecordReq();
                            }
                        }
                    }

                    m_level = value;

                    // 赋值后判断，这里的等级已经变成virtual，其实应该在MyselfProperty重写level，请不要再移到赋值前判断处理里面！
                    if (this.GetType() == typeof(EntityMyself))
                    {
                        var entity = this as EntityMyself;
                        entity.HandleLevelChangeResult();
                    }

                    if (this.GetType() == typeof(EntityMyself) || this.GetType() == typeof(EntityPlayer))
                    {
                        if (BillboardViewManager.Instance != null)
                            BillboardViewManager.Instance.SetHead(this);
                    }

                    OnPropertyChanged(ATTR_LEVEL, value);
                }

            }
        }
        public byte VipLevel
        {
            get { return m_VipLevel; }
            set
            {
                m_VipLevel = value;
                OnPropertyChanged(ATTR_VIP_LEVEL, value);
            }
        }
        public byte buyCount
        {
            get { return m_buyCount; }
            set
            {
                m_buyCount = value;
            }
        }
        public ushort arenicGrade
        {
            get { return m_arenicGrade; }
            set
            {
                m_arenicGrade = value;
                OnPropertyChanged(ATTR_ARENIC_GRADE, value);
            }
        }
        public uint fightForce
        {
            get { return m_fightForce; }
            set
            {
                CheckGetSomeThing(99, m_fightForce, value);
                m_fightForce = value;
                OnPropertyChanged(ATTR_FIGHT_FORCE, value);
            }
        }
        public uint arenicCredit
        {
            get { return m_arenicCredit; }
            set
            {
                CheckGetSomeThing(11, m_arenicCredit, value);
                m_arenicCredit = value;

                OnPropertyChanged(ATTR_ARENIC_CREDIT, value);
            }
        }
        //角色血瓶数量
        public byte hpCount
        {
            get { return m_hpCount; }
            set
            {
                m_hpCount = value;
                OnPropertyChanged(ATTR_HP_COUNT, value);
            }
        }

        public uint chargeSum
        {
            get { return m_chargeSum; }
            set
            {
                m_chargeSum = value;
                OnPropertyChanged(ATTR_CHARGE_SUM, value);
            }
        }
        /// <summary>
        /// 生命值
        /// </summary>
        virtual public uint curHp
        {
            get { return (uint)(m_curHp + deviator); }
            set
            {
                //先判断不等于的时候修改，修改完后确认是玩家修改就trigger
                if (curHp != value)
                {
                    m_curHp = (uint)(value - deviator);
                    OnPropertyChanged(ATTR_HPSTRING, HpString);
                    OnPropertyChanged(ATTR_HP, PercentageHp);
                }
            }
        }

        /// <summary>
        /// 生命上限
        /// </summary>
        public uint hp
        {
            get { return m_hp; }
            set
            {
                m_hp = value;
                OnPropertyChanged(ATTR_HP, PercentageHp);
                OnPropertyChanged(ATTR_HPSTRING, HpString);
                //EventDispatcher.TriggerEvent<float, uint>
                // (BillboardLogicManager.BillboardLogicEvent.SETBILLBOARDBLOOD, PercentageHp, ID);
            }
        }
        private uint m_ElfEquipSkillId;
        public uint ElfEquipSkillId
        {
            get { return m_ElfEquipSkillId; }
            set
            {
                m_ElfEquipSkillId = value;
            }
        }
        public String HpString
        {
            get
            {
                return String.Concat((int)PercentageHp > 1 ? 100 :
                    ((int)(PercentageHp * 100) == 0 ? (curHp == 0 ? 0 : 1) : (int)(PercentageHp * 100)), "%");
            }
        }

        public float PercentageHp
        {
            get
            {
                float tempCurHp = curHp;
                float tempHp = hp;
                return tempCurHp / (hp == 0 ? 1 : tempHp);
            }
        }

        /// <summary>
        /// 宝石特效ID
        /// </summary>
        private uint m_loadedJewelId;

        public uint loadedJewelId
        {
            get {return m_loadedJewelId; }
            set
            {
                m_loadedJewelId = value;
            }
        }

        /// <summary>
        /// 装备特效ID
        /// </summary>
        private uint m_loadedEquipId;

        public uint loadedEquipId
        {
            get { return m_loadedEquipId; }
            set
            {
                m_loadedEquipId = value;
            }
        }

        /// <summary>
        /// 强化特效ID
        /// </summary>
        private uint m_loadedStrengthenId;

        public uint loadedStrengthenId
        {
            get { return m_loadedStrengthenId; }
            set
            {
                m_loadedStrengthenId = value;
            }
        }

        /// <summary>
        /// 速度
        /// </summary>
        public float speed
        {
            get { return m_speed; }
            set
            {
                m_speed = value;
                OnPropertyChanged(ATTR_SPEED, value);
            }
        }

        public int revertHP = 1; //生命回复速度
        public int MP = 0; //魔法值
        public int maxMP = 100; //魔法上限
        public int revertMP = 1; //魔法回复速度

        /// <summary>
        /// 玩家穿在身上的武器
        /// </summary>
        public uint loadedWeapon
        {
            get { return m_loadedWeapon; }
            set
            {
                if (value == 0)
                {
                    //RemoveEquip((int)m_loadedWeapon);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("weapon:" + value + ",vocation:" + (int)vocation);
                    //Mogo.Util.LoggerHelper.Debug("entity:" + Transform.name);
                    m_loadedWeapon = value;
                    Equip((int)value);
                }
            }
        }

        /// <summary>
        /// 玩家穿在身上的衣服
        /// </summary>
        public uint loadedCuirass
        {
            get { return m_loadedCuirass; }
            set
            {
                if (value == 0)
                {
                    //RemoveEquip((int)m_loadedCuirass);
                }
                else
                {
                    Equip((int)value);
                }
                m_loadedCuirass = value;
            }
        }
        /// <summary>
        /// 玩家穿在身上的护手
        /// </summary>
        public uint loadedArmguard
        {
            get { return m_loadedArmguard; }
            set
            {
                if (value == 0)
                {
                    //RemoveEquip((int)m_loadedHead);
                }
                else
                {
                    Equip((int)value);
                }
                m_loadedArmguard = value;
            }
        }
        /// <summary>
        /// 玩家穿在身上的腿部
        /// </summary>
        public uint loadedLeg
        {
            get { return m_loadedLeg; }
            set
            {
                if (value == 0)
                {
                    //RemoveEquip((int)m_loadedLeg);
                }
                else
                {
                    Equip((int)value);
                }
                m_loadedLeg = value;
            }
        }
        /// <summary>
        /// 玩家穿在身上的客户端特效
        /// </summary>
        public uint loadedBodyEffect { get; set; }

        /// <summary>
        /// 职业
        /// </summary>
        private Vocation m_vocation;

        /// <summary>
        /// 职业
        /// </summary>
        public Vocation vocation
        {
            get { return m_vocation; }
            set
            {
                m_vocation = value;
                OnPropertyChanged(ATTR_HEAD_ICON, HeadIcon);
            }
        }


        /// <summary>
        /// 性别
        /// </summary>
        public string GetSexString(TaskManager.DialogueRelationship relationship)
        {
            switch (relationship)
            {
                case TaskManager.DialogueRelationship.YoungerToOlder:
                    switch (vocation)
                    {
                        case Vocation.Warrior:
                        case Vocation.Archer:
                            return LanguageData.GetContent(150006);

                        case Vocation.Mage:
                        case Vocation.Assassin:
                            return LanguageData.GetContent(150007);

                        default:
                            return "";
                    }

                case TaskManager.DialogueRelationship.OlderToYounger:
                    return "";
                case TaskManager.DialogueRelationship.You:
                    return "";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 头像图标
        /// </summary>
        public virtual String HeadIcon
        {
            get
            {
                if (this.GetType() == typeof(EntityMyself))
                    return IconData.dataMap.Get((int)IconOffset.Avatar + (int)vocation).path;
                else
                    return String.Empty;
            }
        }

        /// <summary>
        /// 难度
        /// </summary>
        virtual public uint difficulty
        {
            get { return m_difficulty; }
            set
            {
                m_difficulty = value;
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender gender { get; set; }
        /// <summary>
        /// 回调方法缓存
        /// </summary>
        private List<KeyValuePair<string, Action<object[]>>> m_respMethods = new List<KeyValuePair<string, Action<object[]>>>();

        #endregion

        /// <summary>
        /// 实体定义名。
        /// </summary>
        public string entityType;

        public Mogo.RPC.EntityDef entity;
        public bool isClientFirst = true;
        public Animator weaponAnimator = null;
        public Transform Transform { get; set; }
        public GameObject GameObject { get; set; }
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = new Vector3(1, 1, 1);
        public ActorParent Actor { get; set; }
        public Animator animator;
        public MogoMotor motor;
        public SfxHandler sfxHandler;

        public AudioSource audioSource;

        protected bool isSrcSpeed = true;
        protected float srcSpeed;

        public AI.BTBlackBoard blackBoard = new AI.BTBlackBoard();

        public bool isCloseMotionBlur = false;

        public short m_iEnterX;
        public short m_iEnterZ;

        //public bool hasCache = false; //是否有缓存的数据未刷新显示

        public string CurrentMotionState
        {
            get { return currentMotionState; }
            set
            {
                currentMotionState = value;
            }
        }

        virtual public int hitStateID
        {
            get { return 0; }
        }

        public int spawnPointCfgId
        {
            get { return m_spawnPointCfgId; }
            set { m_spawnPointCfgId = value; }
        }

        public short enterX
        {
            get { return m_iEnterX; }
            set
            {
                m_iEnterX = value;
            }
        }

        public short enterZ
        {
            get { return m_iEnterZ; }
            set
            {
                m_iEnterZ = value;
            }
        }

        public int currentSee
        {
            get { return m_currentSee; }
            set
            {
                m_currentSee = value;
            }
        }

        #endregion

        #region 私有变量
        public const float EntityColiderHeight = 1.5f;
        public SkillManager skillManager;
        protected BattleManager battleManger;
        protected SfxManager sfxManager;
        protected BuffManager buffManager;

        protected FSMMotion fsmMotion = new FSMMotion();
        protected string currentMotionState = MotionState.IDLE;

        private Dictionary<string, object> objectAttrs = new Dictionary<string, object>();
        private Dictionary<string, int> intAttrs = new Dictionary<string, int>();
        private Dictionary<string, double> doubleAttrs = new Dictionary<string, double>();
        private Dictionary<string, string> stringAttrs = new Dictionary<string, string>();

        public Dictionary<string, object> ObjectAttrs
        {
            get { return objectAttrs; }
            set { objectAttrs = value; }
        }

        public Dictionary<string, int> IntAttrs
        {
            get { return intAttrs; }
            set { intAttrs = value; }
        }

        public Dictionary<string, double> DoubleAttrs
        {
            get { return doubleAttrs; }
            set { doubleAttrs = value; }
        }

        public Dictionary<string, string> StringAttrs
        {
            get { return stringAttrs; }
            set { stringAttrs = value; }
        }
        private readonly static HashSet<TypeCode> m_intSet = new HashSet<TypeCode>() { TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32 };
        private readonly static HashSet<TypeCode> m_doubleSet = new HashSet<TypeCode>() { TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double };

        public int currSpellID = -1; //小于等于0为当前空闲
        public int hitActionIdx = 0; //当前技能hitAction索引
        public int currHitAction = -1; //当前hitAction ID
        public List<uint> hitTimer = new List<uint>(); //技能hit的timer id
        public bool walkingCastSkill = false; //跑动中使用技能
        public string skillActName = ""; //当前技能动作名
        protected bool hadSyncProp = false; //同步完初始化属性
        #endregion
    }
}
