#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MSGIDType
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：远程服务消息标识。
//----------------------------------------------------------------*/
#endregion

namespace Mogo.RPC
{
    /// <summary>
    /// 远程服务消息标识。
    /// </summary>
    public enum MSGIDType : ushort
    {
        //服务器发给客户端的包
        CLIENT_LOGIN_RESP = 1,                                  //服务器发给客户端的账号登录结果
        CLIENT_NOTIFY_ATTACH_BASEAPP = 2,                                  //连接baseapp通知
        CLIENT_ENTITY_ATTACHED = 3,                                  //通知客户端已经attach到一个服务器entity,同时刷数据
        CLIENT_AVATAR_ATTRI_SYNC = 4,                                  //AVATAR相关属性修改同步
        CLIENT_RPC_RESP = 5,                                  //服务器发给客户端的rpc
        CLIENT_ENTITY_POS_SYNC = 6,                                 //服务器告诉客户端坐标变化(move)
        //CLIENT_ENTITY_SPACE_CHANGE        = 7,                                  //服务器告诉客户端场景变化
        CLIENT_AOI_ENTITIES = 8,                                  //服务器告诉客户端aoi范围内的entity
        CLIENT_AOI_NEW_ENTITY = 9,                                  //服务器告诉客户端aoi范围内新增的entity
        CLIENT_ENTITY_CELL_ATTACHED = 10,                                 //服务器打包给客户端的cell_client属性
        CLIENT_OTHER_ENTITY_ATTRI_SYNC = 11,								  //其他entity属性变化同步
        CLIENT_OTHER_ENTITY_POS_SYNC = 12,								  //其他entity坐标变化同步(move)
        CLIENT_OTHER_ENTITY_MOVE_REQ = 13,                                 //服务器转发的其他entity的移动请求
        //CLIENT_OTHER_RPC_RESP             = 14,                                 //对其他客户端entity的rpc
        CLIENT_AOI_DEL_ENTITY = 15,                                 //有entity离开了aoi

        CLIENT_ENTITY_POS_PULL = 16,                                 //服务器告诉客户端坐标变化(拉扯)
        CLIENT_OTHER_ENTITY_POS_PULL = 17,                                 //服务器转发的其他entity的移动请求(拉扯)
        CLIENT_ENTITY_POS_TELEPORT = 18,                                //服务器告诉客户端坐标变化(teleport)
        CLIENT_OTHER_ENTITY_TELEPORT = 19,                                 //服务器转发的其他entity的移动请求(teleport)

        CLIENT_CHECK_RESP = 20,                               //客户端发给服务器的包
        BASEAPP_CLIENT_REFUSE_RELOGIN = 21,                     //服务端拒绝断线重连
        NOTIFY_CLIENT_DEFUSE_LOGIN = 22,                        //服务端拒绝连接

        LOGINAPP_CHECK = 30,                                  //客户端版本校验

        //客户端发给服务器的包
        LOGINAPP_LOGIN = 31,                                 //客户端输入帐户名/密码进行登录验证
        BASEAPP_CLIENT_LOGIN = 32,
        BASEAPP_CLIENT_RPCALL = 33,                                 //客户端发起的远程调用
        BASEAPP_CLIENT_MOVE_REQ = 34,                                 //客户端发起的移动
        BASEAPP_CLIENT_RPC2CELL_VIA_BASE = 35,                          //客户端cell远程调用
        BASEAPP_CLIENT_RELOGIN = 37,                                    //客户端发起断线重连

        //暂定50以下的是客户端和服务器的交互包,需要加密
        MAX_CLIENT_MSGID = 50,

        ALLAPP_ONTICK = 101,                                //心跳消息
        ALLAPP_SETTIME = 102,                                //同步时间消息
        ALLAPP_SHUTDOWN_SERVER = 103,                                //关闭服务器通知

        ////LOGINAPP_LOGIN                    = LOGINAPP + 1,               //客户端输入帐户名/密码进行登录验证
        LOGINAPP_MODIFY_LOGIN_FLAG = MSGType.LOGINAPP + 6,              //修改服务器是否可以登录标记
        LOGINAPP_SELECT_ACCOUNT_CALLBACK = MSGType.LOGINAPP + 7,
        LOGINAPP_NOTIFY_CLIENT_TO_ATTACH = MSGType.LOGINAPP + 8,
    };


    public enum ServerInfo : ushort
    {
        NONE = 0,
        LOGINAPP = 1,
        BASEAPPMGR = 2,
        DBMGR = 3,
        TIMERD = 4,
        CLIENT = 5,

        //PROXY         = 1,
        //CELLAPPMGR    = 4,

        BASEAPP = 6,
        CELLAPP = 7,

        MULTI_MIN_ID = 11,		//可能启动多个进程的服务器最小id从这里开始

        MAILBOX_RESERVE_SIZE = 30,	//预设30个服务器进程
    };

    public enum MSGType : ushort
    {
        LOGINAPP = ServerInfo.LOGINAPP << 12,      //0x2000
        BASEAPPMGR = ServerInfo.BASEAPPMGR << 12,    //0x3000
        //CELLAPPMGR   = CELLAPPMGR << 12,    //0x4000
        BASEAPP = ServerInfo.BASEAPP << 12,       //0x5000
        CELLAPP = ServerInfo.CELLAPP << 12,       //0x6000
        DBMGR = ServerInfo.DBMGR << 12,         //0x7000
    };

    public enum MissionReq : byte
    {
        ENTER_MISSION = 1,                      //进入指定难度的关卡副本
        EXIT_MISSION = 2,                       //退出关卡副本
        START_MISSION = 3,                      //客户端通知副本开始
        GET_STAR_MISSION = 4,                   //客户端获取所有副本的星数
        RESET_MISSION_TIMES = 5,                //客户端请求重置玩家的挑战次数
        GET_MISSION_TIMES = 6,                  //客户端请求获取已挑战次数
        GET_FINISHED_MISSIONS = 7,              //客户端请求已完成的副本关卡信息
        GET_NOTIFY_TO_CLENT_EVENT = 8,          //服务器通知客户端事件发生
        NOTIFY_TO_CLIENT_RESULT = 9,            //通知客户端结果
        GET_MISSION_LEFT_TIME = 10,             //客户端获取副本关卡的剩余时间
        SPAWNPOINT_START = 11,                  //客户端通知服务器指定刷怪点开始刷怪
        SPAWNPOINT_STOP = 12,                   //客户端通知服务器指定刷怪点停止刷怪
        NOTIFY_TO_CLIENT_RESULT_SUCCESS = 13,   //服务器通知客户端成功，并下发通关奖励池列表
        NOTIFY_TO_CLIENT_RESULT_FAILED = 14,    //服务器通知客户端失败
        GET_MISSION_REWARDS = 15,               //客户端获取奖励池信息
        CLIENT_MISSION_INFO = 16,               //客户端设置的副本关卡状态，服务器无须理解其格式
        KILL_MONSTER_EXP = 19,                  //杀怪经验获得billboard显示
        QUIT_MISSION = 20,                      //杀怪经验获得billboard显示
        ADD_FRIEND_DEGREE = 21,                 //杀怪经验获得billboard显示
        NOTIFY_TO_CLENT_SPAWNPOINT = 22,        //服务器通知客户端刷怪点的怪已经死了
        UPLOAD_COMBO = 23,                      //客户端上传连击数
        GET_MISSION_TRESURE_REWARDS = 24,       //客户端获取已经拿到的关卡副本宝箱奖励
        REVIVE = 25,                            //复活
        GET_REVIVE_TIMES = 26,                  //客户端获取已复活次数

        GET_MISSION_SWEEP_LIST = 66,            //获取副本的怪物和奖励
        GET_RESET_TIMES = 67,                   //客户端获取关卡总的已重置次数
        GET_RESET_TIMES_BY_MISSION = 68,
        MSG_SWEEP_MISSION = 18, // 副本扫荡

        NOTIFY_TO_CLIENT_TO_UPLOAD_COMBO = 69,  //上传连击
        GO_TO_INIT_MAP = 71,                    //直接退出
        MSG_GET_SWEEP_TIMES = 72,       // 获取可扫荡次数
        GET_MISSION_TRESURE = 73,               //获取副本关卡奖励

        CREATE_CLIENT_DROP = 74,
        NOTIFY_TO_CLIENT_MISSION_REWARD = 75,
        UPLOAD_COMBO_AND_BOTTLE = 76,
        NOTIFY_TO_CLIENT_TO_LOAD_ITNI_MAP = 79,
        GET_MISSION_RECORD = 80,                 //获取关卡的最优记录
        NOTIFY_TO_CLIENT_MISSION_INFO = 81,

        GET_ACQUIRED_MISSION_BOSS_TREASURE = 82,
        GET_MISSION_BOSS_TREASURE = 83,

        MWSY_MISSION_NOTIFY_CLIENT = 84,
        MWSY_MISSION_GET_INFO = 85,
        MWSY_MISSION_ENTER = 86
    }

    public enum LoginResult : byte
    {
        SUCCESS = 0,						 //认证成功
        RET_ACCOUNT_PASSWD_NOMATCH = 1,   //帐号密码不匹配
        NO_SERVICE = 2,				     //服务器未开放登陆
        FORBIDDEN_LOGIN = 3,              //被禁止登陆
        TOO_MUCH = 4,                   //服务器人数超过最大数量，不可登录
        TIME_ILLEGAL = 5,                 //本次登录超时 
        SIGN_ILLEGAL = 6,                 //签名非法
        SERVER_BUSY = 7,                  //sdk服务器验证超时
        SDK_VERIFY_FAILED = 8,            //sdk服务器验证失败
        ACCOUNT_ILLEGAL = 9,              //sdk验证成功但是帐号不一样    
        MULTILOGIN = 10,             //重复登陆
        INNER_ERR = 11            //服务器内部错误

    };

    public enum AccountErrorCode : short
    {
        CREATED = 11,                //该帐号下已经创建过角色了
        NAME_TOO_SHORT = 12,                //角色姓名太短,至少为4个字符（2个汉字）
        NAME_TOO_LONG = 13,                //角色姓名太长,至多为16个字符（8个汉字）
        NAME_INVALID = 14,                //角色姓名包含非法字符
        NAME_EXISTS = 15,                //该角色姓名已经被占用
        NAME_BANNED = 16,                //角色姓名包含敏感字
        GENDER = 17,                //角色性别取值错误
        VOCATION = 18,                //角色职业取值错误
        TOO_MUCH = 19,                //超过角色数量
    };

    public enum DefCheckResult : byte
    {
        ENUM_LOGIN_CHECK_SUCCESS = 0,  //ENTITY_DEF检查成功
        ENUM_LOGIN_CHECK_ENTITY_DEF_NOMATCH = 1,  //ENTITY_DEF检查不成功
        ENUM_LOGIN_CHECK_NO_SERVICE = 2,  //服务器未开放登陆
    };

    public enum CampaignReq : ushort
    {
        CAMPAIGN_JOIN = 10004,
        CAMPAIGN_MATCH = 10005,
        CAMPAIGN_LEAVE = 10006,
        CAMPAIGN_RESULT = 10007,

        CAMPAIGN_NOTIFY_CLIENT_TO_START = 10009,
        CAMPAIGN_NOTIFY_CLIENT_TO_FINISH = 10010,

        CAMPAIGN_COUNT_DOWN = 10011,
        CAMPAIGN_MISSION_COUNT_DOWN = 10012,
        
        CAMPAIGN_GET_LEFT_TIMES = 10014,

        CAMPAIGN_GET_ACTIVITY_LEFT_TIME = 10015,
        CAMPAIGN_NOTIFY_WAVE_COUNT = 10016
    }

    public enum CampaignErrorCode : ushort
    {
        ERR_ACTIVITY_NOT_EXIST = 1,
        ERR_ACTIVITY_INVITE_SELF = 2,
        ERR_ACTIVITY_INVITE_NOT_FRIEND = 3,
        ERR_ACTIVITY_INVITE_NOT_EXIT = 4,
        ERR_ACTIVITY_INVITE_NOT_ONLINE = 5,
        ERR_ACTIVITY_INVITE_AC_NOT_EXIST = 6,
        ERR_ACTIVITY_INVITE_ALLREADY_INVITE = 7,
        ERR_ACTIVITY_INVITE_RESP_NOT_EXIST = 8,
        ERR_ACTIVITY_JOIN_NOT_STARTED = 9,
        ERR_ACTIVITY_JOIN_NOT_EXIST = 10,
        ERR_ACTIVITY_JOIN_LEVEL_NOT_MACTH = 11,
        ERR_ACTIVITY_JOIN_LEVEL_TIMES_OUT = 12,
        ERR_ACTIVITY_TOWER_DEFENCE_MATCH_FAIL = 13,
        ERR_ACTIVITY_JOIN_ALLREADY = 14,

        ERR_ACTIVITY_GET_ACTIVITY_LEFT_TIME_NOT_EXIT = 16,
        ERR_ACTIVITY_GET_ACTIVITY_LEFT_TIME_NOT_STARTED = 17
    }
}