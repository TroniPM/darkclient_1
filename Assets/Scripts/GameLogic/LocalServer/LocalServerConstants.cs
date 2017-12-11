using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mogo.GameLogic.LocalServer
{
    public enum EntityType
    {
        Dummy = 1
    }

    public enum LocalServerMissionEventType
    {
        SpawnPoint
    }

    public enum SpawnPointType : int
    {
        step = 0,
        start = 1
    }

    public enum SpawnPointState
    {
        inactive,
        active,
        activeWhenStart
    }

    public enum MonsterState
    {
        invalid,
        sleep,
        active,
        dead
    }

    public enum MissionHandleCode : byte
    {
        ENTER_MISSION = 1,                      //进入指定难度的关卡副本
        EXIT_MISSION = 2,                       //退出关卡副本
        START_MISSION = 3,                      //客户端通知副本开始

        GET_NOTIFY_TO_CLENT_EVENT = 8,          //服务器通知客户端事件发生

        GET_MISSION_LEFT_TIME = 10,             //客户端获取副本关卡的剩余时间
        SPAWNPOINT_START = 11,                  //客户端通知服务器指定刷怪点开始刷怪

        NOTIFY_TO_CLIENT_RESULT_SUCCESS = 13,   //服务器通知客户端成功，并下发通关奖励池列表
        NOTIFY_TO_CLIENT_RESULT_FAILED = 14,    //服务器通知客户端失败
        GET_MISSION_REWARDS = 15,               //客户端获取奖励池信息

        KILL_MONSTER_EXP = 19,                  //杀怪经验获得billboard显示
        QUIT_MISSION = 20,

        NOTIFY_TO_CLENT_SPAWNPOINT = 22,        //服务器通知客户端刷怪点的怪已经死了
        UPLOAD_COMBO = 23,                      //客户端上传连击数
        GET_MISSION_TRESURE_REWARDS = 24,       //客户端获取已经拿到的关卡副本宝箱奖励
        REVIVE = 25,                            //复活
        GET_REVIVE_TIMES = 26,                  //客户端获取已复活次数


        NOTIFY_TO_CLIENT_TO_UPLOAD_COMBO = 69,  //上传连击
        GO_TO_INIT_MAP = 71,                    //直接退出

        GET_MISSION_TRESURE = 73,               //获取副本关卡奖励
        CREATE_CLIENT_DROP = 74,
        UPLOAD_COMBO_AND_BOTTLE = 76            //单机副本结束，上传连击数和使用的药瓶数量，字符串传奖励信息
    }

    public enum CliEntityActionHandleCode : uint
    {
        DummyDie = 1,
        HitAvatar = 2
    }

    public enum CliEntitySkillHandleCode : uint
    {
        SummonToken = 1
    }
}
