/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：Constants
// 创建者：Steven Yang
// 修改者列表：Joe Mo
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using System.Collections.Generic;
using Mogo.GameData;
using Mogo.AI;
using Mogo.AI.BT;
using System;
using System.Reflection;


namespace Mogo.Game
{
    public class UIConfig
    {
        public const int WIDTH = 1280;
    }

    public enum Vocation : byte
    {
        Warrior = 1,         // 战士
        Assassin = 2,       // 刺客
        Archer = 3,         // 弓箭手
        Mage = 4,           // 法师
        Others = 5           // 其他
    }

    public enum Gender : byte
    {
        Female = 0,         // 女性
        Male = 1            // 男性 
    }

    public enum ItemCode
    {
        GOLD = 2
    }
    public class EquipSlotName
    {
        public const int STR_ID = 155;
        static public string[] names;
        static EquipSlotName()
        {
            names = new string[10];
            for (int i = 0; i < 10; i++)
            {
                names[i] = LanguageData.dataMap[STR_ID + i].content;
            }
        }

        // 通过装备Slot获取对应的装备类型名
        static public string GetEquipSlotNameBySlot(int equipSlot)
        {
            if (equipSlot == 11)// names[9]-武器
                return names[9];
            else if (equipSlot == 10 || equipSlot == 9)// names[8]-戒指
                return names[8];
            else if (equipSlot >= 1 && equipSlot <= 8)// 0 - 7
                return names[equipSlot - 1];
            else
                return null;
        }
    }


    public class BodyName
    {
        public const int STR_ID = 145;
        static public string[] names;
        static BodyName()
        {
            names = new string[10];
            for (int i = 0; i < 10; i++)
            {
                names[i] = LanguageData.dataMap[STR_ID + i].content;
            }
        }
    }

    public class BodyIcon
    {
        public const int ICON_ID = 30101;
        static public string[] icons;
        static BodyIcon()
        {
            icons = new string[10];
            for (int i = 0; i < 10; i++)
            {
                icons[i] = IconData.dataMap[ICON_ID + i].path;
            }
        }
    }

    public class BodyTextIcon
    {
        public const int ICON_ID = 31001;
        static public string[] icons;
        static BodyTextIcon()
        {
            icons = new string[10];
            for (int i = 0; i < 10; i++)
            {
                icons[i] = IconData.dataMap[ICON_ID + i].path;
            }
        }
    }

    public class EquipSlotTextIcon
    {
        public const int ICON_ID = 31101;
        static public string[] icons;
        static EquipSlotTextIcon()
        {
            icons = new string[12];
            icons[0] = IconData.blankBox;
            for (int i = 1; i < 10; i++)
            {
                icons[i] = IconData.dataMap[ICON_ID + i - 1].path;
            }
            icons[10] = IconData.dataMap[ICON_ID + 8].path;
            icons[11] = IconData.dataMap[ICON_ID + 9].path;
        }
    }

    public class EquipSlotIcon
    {
        static public string[] icons;
        static EquipSlotIcon()
        {
            icons = new string[12];
            icons[0] = IconData.blankBox;
            for (int i = 1; i < 10; i++)
            {
                icons[i] = BodyIcon.icons[i - 1];
            }
            icons[10] = BodyIcon.icons[8];
            icons[11] = BodyIcon.icons[9];
        }
    }

    public enum EquipType
    {
        Head = 1,           // 头部
        Neck = 2,           // 颈部
        Shoulder = 3,       // 肩部
        Cuirass = 4,        // 胸甲
        Belt = 5,           // 腰带
        Glove = 6,          // 手套
        Cuish = 7,          // 腿甲
        Shoes = 8,          // 靴子
        Ring = 9,           // 戒子
        Weapon = 10         // 武器
    }
    public enum EquipSlot
    {
        Head = 1,           // 头部
        Neck = 2,           // 颈部
        Shoulder = 3,       // 肩部
        Cuirass = 4,        // 胸甲
        Belt = 5,           // 腰带
        Glove = 6,          // 手套
        Cuish = 7,          // 腿甲
        Shoes = 8,          // 靴子
        LeftRing = 9,        // 左戒子
        RightRing = 10,     // 右戒子
        Weapon = 11         // 武器
    }
    public enum WeaponSubType
    {
        none = 0,
        blade = 1,          // 大剑
        fist = 2,           // 拳套
        dagger = 3,         // 匕首
        twinblade = 4,      // 月刃
        staff = 5,          // 法杖
        fan = 6,            // 扇
        bow = 7,            // 弓
        gun = 8,            // 大炮
    };

    public enum BattleModeType
    {
        ClientGuiding = 1,     // 客户端先行的
        ServerGuiding = 2      // 服务器端先行的
    }

    public enum TargetType
    {
        Enemy = 0,          // 敌人
        Myself = 1,         // 自己
        TeamMember = 2,     // 队友
        Ally = 3            // 友方
    }

    public enum TargetRangeType
    {
        LineRange = 3,
        SectorRange = 0,
        CircleRange = 1,
        SingeTarget = 2,
        WorldRange = 6
    }

    public enum CharacterCode : short
    {
        INVALID_NAME = 14,                    //名字不合法
        INPUT_NAME = 20,                    //请输入名字
        NOT_SELECTED = 501,                //未选定角色
        CREATE_CHARACTER = 502,                //创建角色
        DELETE_CHARACTER = 506,                //删除角色确认
        QUIT_GAME_CONFIRM = 601,                //确认退出游戏？
    };

    public enum IconOffset
    {
        Avatar = 14000
    }

    /// <summary>
    /// 中文表分段偏移。
    /// </summary>
    public enum LangOffset
    {
        /// <summary>
        /// 帐号管理。
        /// </summary>
        Account = 20999,
        /// <summary>
        /// 角色管理。
        /// </summary>
        Character = 21000,
        /// <summary>
        /// 游戏启动
        /// </summary>
        StartGame = 21200
    }

    public class AIContainer
    {
        static public Dictionary<uint, BehaviorTreeRoot> container = new Dictionary<uint, BehaviorTreeRoot>();

        static AIContainer()
        {
        }

        public static void Init()
        {
            Assembly ass = typeof(AIContainer).Assembly;
            var types = ass.GetTypes();
            foreach (var item in types)
            {
                if (item.Namespace == "Mogo.AI.BT")
                {
                    var type = item.BaseType;
                    if (type == typeof(BehaviorTreeRoot))
                    {
                        var id = item.Name.Replace("BT", String.Empty);
                        uint key;
                        if (UInt32.TryParse(id, out key))
                        {
                            var p = item.GetProperty("Instance");
                            var value = p.GetGetMethod().Invoke(null, null) as BehaviorTreeRoot;
                            container.Add(key, value);
                        }
                    }
                }
            }

        }
    }

    public class ActionConstants
    {
        public readonly static int CITY_IDLE = -1; //主城的站立
        public readonly static int COPY_IDLE = 0; //副本的站立
        public readonly static int HIT = 11; //受击
        public readonly static int HIT_AIR = 12; //浮空
        public readonly static int HIT_GROUND = 13; //倒地受击
        public readonly static int KNOCK_DOWN = 14; //击飞
        public readonly static int PUSH = 15; //后退
        public readonly static int STUN = 16;
        public readonly static int DIE = 17; //死亡
        public readonly static int REVIVE = 19; //复活
        public readonly static int DIE_KNOCK_DOWN = 37; //击飞死亡
        public readonly static int DIE_AIR = 38; //浮空死亡
    }

    public class PlayerActionNames
    {//角色动作ID对照名字表,用于技能结束判断
        public readonly static Dictionary<int, string> names = new Dictionary<int, string>()
        {
            {-1, "idle"},
            {0, "ready"},
            {1, "attack_1"},
            {2, "attack_2"},
            {3, "attack_3"},
            {4, "powercharge"},
            {5, "powerattack_1"},
            {6, "powerattack_2"},
            {7, "powerattack_3"},
            {8, "skill_1"},
            {9, "skill_2"},
            {10, "rush"},
            {11, "hit"},
            {12, "hitair"},
            {13, "hitground"},
            {14, "knockdown"},
            {15, "push"},
            {16, "stun"},
            {17, "die"},
        };

        public readonly static string IDLE = "idle";
        public readonly static string READY = "ready";
        public readonly static string ATTACK_1 = "attack_1";
        public readonly static string ATTACK_2 = "attack_2";
        public readonly static string ATTACK_3 = "attack_3";
        public readonly static string POWERCHARGE = "powercharge";
        public readonly static string POWERATTACK_1 = "powerattack_1";
        public readonly static string POWERATTACK_2 = "powerattack_2";
        public readonly static string POWERATTACK_3 = "powerattack_3";
        public readonly static string SKILL_1 = "skill_1";
        public readonly static string SKILL_2 = "skill_2";
        public readonly static string RUSH = "rush";
        public readonly static string HIT = "hit";
        public readonly static string HITAIR = "hitair";
        public readonly static string HITGROUND = "hitground";
        public readonly static string KNOCKDOWN = "knockdown";
        public readonly static string PUSH = "push";
        public readonly static string STUN = "stun";
        public readonly static string DIE = "die";
        public readonly static string DIE_HITAIR = "dir_hitair";
        public readonly static string DIE_KNOCKDOWN = "dir_knockdown";
        public readonly static string GETUP = "getup";
    }

    public class ActionTime
    {//动作时间，毫秒
        public readonly static uint HIT = 600;
        public readonly static uint HIT_AIR = 3500;
        public readonly static uint KNOCK_DOWN = 3500;
        public readonly static uint PUSH = 1000;
        public readonly static uint HIT_GROUND = 3000;
        public readonly static uint REVIVE = 2500;
    }

    public class StateCfg
    {
        public readonly static int DEATH_STATE = 0;             //死亡状态       
        public readonly static int DIZZY_STATE = 1;             //眩晕状态       
        public readonly static int POSSESS_STATE = 2;             //魅惑状态       
        public readonly static int IMMOBILIZE_STATE = 3;             //定身状态       
        public readonly static int SILENT_STATE = 4;             //沉默状态       
        public readonly static int STIFF_STATE = 5;             //僵直状态       
        public readonly static int FLOAT_STATE = 6;             //浮空状态       
        public readonly static int DOWN_STATE = 7;             //击倒状态       
        public readonly static int BACK_STATE = 8;             //击退状态       
        public readonly static int UP_STATE = 9;             //击飞状态       
        public readonly static int IMMUNITY_STATE = 10;            //免疫状态       
        public readonly static int NO_HIT_STATE = 11;            //无法被击中状态 
        public readonly static int SLOW_DOWN_STATE = 12;            //无法被击中状态 
        public readonly static int BATI_STATE = 13;            //霸体状态 
    }

    public class DummyLookOnParam
    {
        public readonly static float CLOSE_MODE_FLOAT_FACTOR = 0.2f; //接近模式距离浮动系数(random(技能castRange*(1-CLOSE_MODE_FLOAT_FACTOR), 技能castRange*(1+CLOSE_MODE_FLOAT_FACTOR)))
        public readonly static float REFER = 1.6f;              //接近远离界定距离系数（技能castRange*REFER）
        public readonly static float PER_ANGLE = 40.0f;         //绕圈模式每次绕的角度 
        public readonly static float SPEED_FACTOR_MODE_0 = 0.5f;
        public readonly static float SPEED_FACTOR_MODE_1 = 0.4f;
        public readonly static float SPEED_FACTOR_MODE_2_3 = 0.4f;
        public readonly static float SPEED_FACTOR_MODE_5 = 1.6f;
    }

    public enum CliEntityType : int
    {
        CLI_ENTITY_TYPE_DUMMY = 1,
        CLI_ENTITY_TYPE_JUG = 2,
        CLI_ENTITY_TYPE_DROP = 3,
        CLI_ENTITY_TYPE_SPAWNPOINT = 4,
    }

    public enum AutoFightState : byte
    {
        IDLE = 1,
        PAUSE = 2,
        RUNNING = 3,
    }

    public class RandomFB
    {
        public readonly static int RAIDID = 50000;
    }

    public class SpecialMonsterId
    {
        public readonly static int TowerDefCrystalId = 5010;
    }

    public class AISpecialEnum
    {
        public readonly static int PatrolSquareRange = 3;//5米 单位m 正方形半径
        public readonly static int DefaultSee = 5000;//50米 单位cm
        public readonly static float PATROL_SPEED_FACTOR = 0.5f;//巡逻速度衰减
        
    }

    public enum AIWarnEvent : byte
    {
        DiscoverSomeOne = 1,
    }
}
