/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TongManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：邮件管理器
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.Game;
using System;
using System.Collections.Generic;
using Mogo.GameData;

public class TongManager
{
    public static void HandleErrorCode(UInt16 errorCode)
    {
        MogoMsgBox.Instance.ShowFloatingTextQueue(LanguageData.GetContent(errorCode + 170000));
    }

    #region 错误码

    const UInt16 ERROR_CREATE_GUILD_LEVEL_TOO_LOW = 1;   //创建公会时等级太低
    const UInt16 ERROR_CREATE_GUILD_NOT_ENOUGH_DIAMOND = 2;   //钻石不足，不能创建关卡
    const UInt16 ERROR_CREATE_GUILD_NAME_ALREADY_USED = 3;   //公会名字已经被占用
    const UInt16 ERROR_CREATE_GUILD_ALREADY_IN_GUILD = 4;   //创建者已经处于一个公会，不能再创建
    const UInt16 ERROR_SET_ANNOUNCEMENT_NO_GUILD = 5;   //玩家没有公会不允许设置公会公告
    const UInt16 ERROR_SET_ANNOUNCEMENT_NO_RIGHT = 6;   //权限不足，不能设置公会公告
    const UInt16 ERROR_GET_ANNOUNCEMENT_NO_GUILD = 7;   //玩家没有公会不允许获取公会公告
    const UInt16 ERROR_GET_GUILD_DETAILED_INFO_NO_GUILD = 8;   //没有公会，不能获取详细信息
    const UInt16 ERROR_APPLY_TO_JOIN_ALREADY_IN_GUILD = 9;   //已经属于一个公会，不能再申请
    const UInt16 ERROR_APPLY_TO_JOIN_GUILD_NOT_EXIT = 10;  //指定的公会不存在，不能申请加入
    const UInt16 ERROR_GET_GUILD_MESSAGE_COUNT_NO_GUILD = 11;  //获取公会消息数量时发现没有公会
    const UInt16 ERROR_GET_GUILD_MESSAGE_COUNT_NO_RIGHT = 12;  //获取公会消息数量时发现没有权限
    const UInt16 ERROR_GET_GUILD_MESSAGES_NO_GUILD = 13;  //获取公会消息数据时发现玩家没有公会
    const UInt16 ERROR_GET_GUILD_MESSAGES_NO_RIGHT = 14;  //获取公会消息数据时发现玩家没有权限
    const UInt16 ERROR_ANSWER_APPLY_NO_GUILD = 15;  //回应公会信息时发现客户端没有公会
    const UInt16 ERROR_ANSWER_APPLY_NO_RIGHT = 16;  //回应公会信息时发现客户端没有权限
    const UInt16 ERROR_ANSWER_APPLY_NO_MESSAGE = 17;  //公会没有消息
    const UInt16 ERROR_ANSWER_APPLY_MESSAGE_NOT_EXIT = 18;  //指定的公会消息不存在
    const UInt16 ERROR_INVITE_NO_RIGHT = 19;  //没有权限，不能邀请玩家
    const UInt16 ERROR_INVITE_NO_EXIT = 20;  //被邀请的玩家不存在
    const UInt16 ERROR_INVITE_ALREADY_IN_GUILD = 21;  //对方已经处于一个公会中，不能邀请
    const UInt16 ERROR_INVITE_NOT_EXIT = 22;  //邀请不存在
    const UInt16 ERROR_QUIT_ONLY_NORMAL_MEMBER = 23;  //只有更普通成员才能退出
    const UInt16 ERROR_QUIT_NO_GUILD = 24;  //没有公会，不能退出
    const UInt16 ERROR_APPLY_TO_JOIN_TOO_MUCH_MEMBERS = 25;  //公会人数已经超过了上限
    const UInt16 ERROR_ANSWER_INVITE_TOO_MUCH_MEMBERS = 26;  //回应邀请时公会人数已经超过了上限
    const UInt16 ERROR_ANSWER_APPLY_TOO_MUCH_MEMBERS = 27;  //回应申请时公会人数已经超过上限
    const UInt16 ERROR_PROMOTE_NO_RIGHT = 28;  //没有权限不能升级
    const UInt16 ERROR_PROMOTE_NOT_IN_GUILD = 29;  //要被等级提升的玩家不在公会
    const UInt16 ERROR_PROMOTE_NOT_MYSELF = 30;  //不能提升自己
    const UInt16 ERROR_PROMOTE_FULL = 31;  //位置已满，不能再提升
    const UInt16 ERROR_DEMOTE_NOT_MYSELF = 32;  //不能把自己降职
    const UInt16 ERROR_DEMOTE_NO_RIGHT = 33;  //没有权限不能降职
    const UInt16 ERROR_DEMOTE_FULL = 34;  //位置已满，不能再降级
    const UInt16 ERROR_EXPEL_NOT_MYSELF = 35;  //不能开除自己
    const UInt16 ERROR_EXPEL_NO_RIGHT = 36;  //没有权限不能开除
    const UInt16 ERROR_DEMISE_NOT_MYSELF = 37;  //不能转让给我自己
    const UInt16 ERROR_DEMISE_NO_RIGHT = 38;  //没有权限，不能转让
    const UInt16 ERROR_DISMISS_NO_RIGHT = 39;  //没有权限解散公会
    const UInt16 ERROR_DISMISS_ALREADY_IN_DELETED = 40;  //已经进入冻结状态不能再删除
    const UInt16 ERROR_THAW_NOT_ENOUGH_DIAMOND = 41;  //不够钻石解冻
    const UInt16 ERROR_THAW_NO_NEED = 42;  //没有冻结，无须解冻
    const UInt16 ERROR_THAW_NO_GUILD = 43;  //没有公会，无须解冻
    const UInt16 ERROR_ANSWER_APPLY_STATUS_FREEZE = 44;  //公会处于冻结状态中，不能接受申请
    const UInt16 ERROR_ANSWER_INVITE_STATUS_FREEZE = 45;  //公会处于冻结状态中，不能接受邀请
    const UInt16 ERROR_APPLY_TO_JOIN_STATUS_FREEZE = 46;        //公会处于冻结状态，不能申请加入
    const UInt16 ERROR_RECHARGE_NOT_ENOUGH_DIAMOND = 47;  //不够钻石解冻
    const UInt16 ERROR_RECHARGE_NOT_ENOUGH_GOLD = 48;  //没有冻结，无须解冻
    const UInt16 ERROR_RECHARGE_NO_GUILD = 49;  //没有公会，无须解冻
    const UInt16 ERROR_RECHARGE_OVER_LIMIT = 50;  //公会处于冻结状态中，不能接受申请
    const UInt16 ERROR_RECHARGE_CAN_NOT = 51;  //公会处于冻结状态中，不能接受邀请
    const UInt16 ERROR_GET_GUILD_MEMBERS_NO_GUILD = 52;        //公会处于冻结状态，不能申请加入
    const UInt16 ERROR_RECHARGE_CD = 53;  //不够钻石解冻
    const UInt16 ERROR_RECHARGE_DAY_TIMES = 54;  //没有冻结，无须解冻
    const UInt16 ERROR_GET_DRAGON_NO_GUILD = 55;  //没有公会，无须解冻
    const UInt16 ERROR_GET_DRAGON_NOT_FULL = 56;  //公会处于冻结状态中，不能接受申请
    const UInt16 ERROR_GET_DRAGON_OVER_TIMES = 57;  //公会处于冻结状态中，不能接受邀请
    const UInt16 ERROR_UPGRADE_GUILD_SKILL_NO_SUCH_TYPE = 58;        //公会处于冻结状态，不能申请加入
    const UInt16 ERROR_UPGRADE_GUILD_SKILL_NO_GUILD = 59;  //没有公会，无须解冻
    const UInt16 ERROR_UPGRADE_GUILD_SKILL_NO_RIGHT = 60;  //公会处于冻结状态中，不能接受申请
    const UInt16 ERROR_UPGRADE_GUILD_SKILL_ALREADY_LIMIT = 61;  //公会处于冻结状态中，不能接受邀请
    const UInt16 ERROR_GET_GUILD_INFO_NO_GUILD = 62;        //公会处于冻结状态，不能申请加入

    #endregion

    #region 通信ID


    public const byte MSG_GET_GUILDS = 36;       //分页获取服务器的公会
    public const byte MSG_GET_GUILDS_COUNT = 37;       //获取公会的个数
    public const byte MSG_CREATE_GUILD = 38;       //创建一个公会
    public const byte MSG_GET_GUILD_INFO = 39;       //获取玩家自己的公会信息
    public const byte MSG_SET_GUILD_ANNOUNCEMENT = 40;       //设置公会公告
    public const byte MSG_GET_GUILD_ANNOUNCEMENT = 41;       //获取公会公告
    public const byte MSG_APPLY_TO_JOIN = 42;       //申请加入指定公会
    public const byte MSG_APPLY_TO_JOIN_NOTIFY = 43;       //获取公会详细信息
    public const byte MSG_GET_GUILD_DETAILED_INFO = 44;       //获取公会消息的数量
    public const byte MSG_GET_GUILD_MESSAGES_COUNT = 45;      //分页获取工会消息
    public const byte MSG_GET_GUILD_MESSAGES = 46;      //回应申请
    public const byte MSG_ANSWER_APPLY = 47;      //邀请好友加入公会
    public const byte MSG_INVITE = 48;      //回应公会邀请
    public const byte MSG_INVITED = 49;      //通知给客户端申请结果
    public const byte MSG_ANSWER_INVITE = 50;      //通知客户端被某公会邀请
    public const byte MSG_APPLY_TO_JOIN_RESULT = 51;      //退出公会
    public const byte MSG_QUIT = 52;      //升级
    public const byte MSG_PROMOTE = 53;      //降职
    public const byte MSG_DEMOTE = 54;      //开除
    public const byte MSG_EXPEL = 55;      //转让
    public const byte MSG_DEMISE = 56;      //解散
    public const byte MSG_DISMISS = 57;      //解冻
    public const byte MSG_THAW = 58;      //解冻
    public const byte MSG_RECHARGE = 59;      //解冻
    public const byte MSG_GET_GUILD_MEMBERS = 60;      //解冻
    public const byte MSG_GET_DRAGON = 61;      //解冻
    public const byte MSG_UPGRADE_GUILD_SKILL = 62;      //解冻
    public const byte MSG_GET_RECOMMEND_LIST = 63;
    public const byte MSG_GET_DRAGON_INFO = 64;

    const byte MSG_GET_DRAGON_RESP = 252;     //创建公会成功后扣除资源
    const byte MSG_RECHARGE_RESP = 253;     //通知会长和副会长有人申请了
    const byte MSG_SUBMIT_CREATE_GUILD_COST = 254;     //设置玩家的公会ID
    const byte MSG_SET_GUILD_ID = 254;     //设置玩家的公会ID
    #endregion

    #region 后端RPC接口名称
    /*
     <GuildReq>
        <Exposed>
        <Arg>UINT8</Arg>    --id
        <Arg>UINT32</Arg>
        <Arg>UINT32</Arg>
        <Arg>STRING</Arg>
      </GuildReq>
    */

    #endregion

    #region 成员变量
    EntityMyself myself;

    int m_iTongNum = 0;
    public bool IsShowMyTong = true;
    public bool IsShowDragon = false;
    public bool IsShowSkill = false;

    string m_strTongNotice;
    string m_strTongMoney;
    public string m_strTongPeopleNum;
    string m_strTongBossName;
    string m_strTongLevel;
    string m_strTongName;
    public int m_iCurrenDragonPower;
    public Dictionary<int, int> m_dictSkillIDToLevel = new Dictionary<int, int>();

    class TongData
    {
        public UInt64 dbid;
        public string name;
        public int level;
        public int peopleNum;
    }

    class TongMemberData
    {
        public UInt64 dbid;
        public string name;
        public int level;
        public int jobId;
        public int power;
        public int contribute;
        public int date;
    }

    class TongApplicantData
    {
        public UInt64 dbid;
        public string name;
        public int job;
        public int level;
        public int power;
    }

    class TongPresenterData
    {
        public UInt64 dbid;
        public string name;
        public int level;
        public int power;
    }

    class TongSkillData
    {
 
    }

    List<TongData> m_listTongData = new List<TongData>();
    List<TongUIViewManager.TongData> m_listTongUIData = new List<TongUIViewManager.TongData>();

    List<TongMemberData> m_listTongMemberData = new List<TongMemberData>();
    List<TongUIViewManager.MemberData> m_listTongMemberUIData = new List<TongUIViewManager.MemberData>();

    List<TongApplicantData> m_listTongApplicantData = new List<TongApplicantData>();
    List<TongUIViewManager.ApplicantData> m_listTongApplicantUIData = new List<TongUIViewManager.ApplicantData>();

    List<TongPresenterData> m_listTongPresenterData = new List<TongPresenterData>();
    List<TongUIViewManager.PresenterData> m_listTongPresenterUIData = new List<TongUIViewManager.PresenterData>();

    List<TongUIViewManager.TongSkillData> m_listTongSkillUIData = new List<TongUIViewManager.TongSkillData>();
    List<int> m_listTongSkillType = new List<int>();

    public void Release()
    {
        m_listTongData.Clear();
        m_listTongUIData.Clear();

        m_tongMgr = null;
    }

    private static TongManager m_tongMgr;

    public static TongManager Instance
    {
        get
        {
            if (m_tongMgr == null)
            {
                m_tongMgr = new TongManager(MogoWorld.thePlayer);

            }

            return m_tongMgr;
        }

    }

    #endregion

    #region 必备成员函数

    public void Init()
    {
    }

    public TongManager(EntityMyself _myself)
    {
        myself = _myself;

        TongUILogicManager.Instance.Initialize();
    }

    public UInt64 GetDBIDFromGridID(int girdID)
    {
        if (girdID >= m_listTongData.Count)
        {
            return 0;
        }

        return m_listTongData[girdID].dbid;
    }

    public UInt64 GetDBIDFromMemberListGridID(int gridID)
    {
        Mogo.Util.LoggerHelper.Debug(gridID + " " + m_listTongMemberData.Count + " @@@@@@@@@@@@@@@@@@@@@@@");
        if (gridID >= m_listTongMemberData.Count)
        {
            return 0;
        }

        return m_listTongMemberData[gridID].dbid;
    }

    public UInt64 GetDBIDFromApplicantListGridID(int gridID)
    {
        if (gridID >= m_listTongApplicantData.Count)
        {
            return 0;
        }

        return m_listTongApplicantData[gridID].dbid;
    }

    public UInt64 GetSkillTypeFromSkillListGridID(int gridID)
    {
        if (gridID >= m_listTongSkillType.Count)
        {
            return 0;
        }

        return (UInt64)m_listTongSkillType[gridID];
    }

    private void ShowErrorView(int errorId, string msg = null)
    {
        //todo:根据errorId显示错误信息
        if (msg != null)
        {
            //如果给了msg根据msg提示显示
        }
        else
        {
            //根据errorId查询错误信息
        }
    }
    #endregion

    #region 调用后端RPC接口

    public void GuildReq(byte id, UInt64 arg1 = 0, UInt64 arg2 = 0, string arg3 = "")
    {
        myself.RpcCall("GuildReq", id, arg1, arg2, arg3);
    }

    #endregion

    #region 后端回调接口
    /*
    <GuildResp>
        <Arg>UINT8</Arg>   -- id
        <Arg>UINT16</Arg>  --errCode
        <Arg>LUA_TABLE</Arg>
     </GuildResp>
     */

    public void GuildResp(byte id, UInt16 errCode, LuaTable respInfo)
    {

        if (errCode != 0)
        {
            HandleErrorCode(errCode);
        }

        switch (id)
        {
            case MSG_GET_GUILDS:
                //应lua_table：({1=数量, 2={1={1=公会dbid,2=名称,3=等级,4=人数}, ...}})
                if (errCode == 0)
                {
                    int tongNum = int.Parse((string)respInfo["1"]);

                    m_listTongUIData.Clear();
                    m_listTongData.Clear();

                    for (int i = 0; i < tongNum; ++i)
                    {
                        TongData temp = new TongData();

                        //uint.Parse((string)((LuaTable)((LuaTable)respInfo["2"])[i.ToString()])["1"]);
                        temp.dbid = uint.Parse((string)((LuaTable)((LuaTable)respInfo["2"])[(i + 1).ToString()])["1"]);
                        temp.name = (string)((LuaTable)((LuaTable)respInfo["2"])[(i + 1).ToString()])["2"];
                        temp.level = int.Parse((string)((LuaTable)((LuaTable)respInfo["2"])[(i + 1).ToString()])["3"]);
                        temp.peopleNum = int.Parse((string)((LuaTable)((LuaTable)respInfo["2"])[(i + 1).ToString()])["4"]);

                        TongUIViewManager.TongData td = new TongUIViewManager.TongData();
                        td.level = temp.level.ToString();
                        td.name = temp.name;
                        td.num = temp.peopleNum.ToString();

                        m_listTongUIData.Add(td);
                        m_listTongData.Add(temp);
                    }

                    TongUIViewManager.Instance.SetTongList(m_listTongUIData);
                    TongUIViewManager.Instance.ShowTongList();
                }
                break;

            case MSG_GET_GUILDS_COUNT:
                Mogo.Util.LoggerHelper.Debug("获取公会数量返回");
                if (errCode == 0)
                {
                    m_iTongNum = int.Parse((string)respInfo["1"]);

                    GuildReq(MSG_GET_GUILDS, 1, (uint)m_iTongNum);

                    Mogo.Util.LoggerHelper.Debug("获取公会数量  " + m_iTongNum);
                }
                break;

            case MSG_CREATE_GUILD:
                if (errCode == 0)
                {
                    //创建公会成功
                    //le：({1=公会名, 2=公会人数， 3=公会职位})

                    //m_strTongName = (string)respInfo["1"];
                    //m_strTongPeopleNum = ((int)respInfo["2"]).ToString();

                    GuildReq(MSG_GET_GUILD_INFO);
                    Mogo.Util.LoggerHelper.Debug("创建公会成功");

                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("创建公会失败 errCode = " + errCode);
                }
                break;

            case MSG_GET_GUILD_INFO:
                if (errCode == 0 && (string)respInfo["1"] != "")
                {
                    //切换到公会详细界面
                    m_strTongName = (string)respInfo["1"];
                    GuildReq(MSG_GET_GUILD_DETAILED_INFO);
                    IsShowMyTong = true;
                    IsShowDragon = false;
                    IsShowSkill = false;

                    Mogo.Util.LoggerHelper.Debug("获取公会信息成功 " + errCode + " " + (string)respInfo["1"]);
                }
                //else if (errCode == ERROR_GET_GUILD_DETAILED_INFO_NO_GUILD)
                else
                {
                    GuildReq(MSG_GET_GUILDS_COUNT);
                    //切换到公会列表

                    Mogo.Util.LoggerHelper.Debug("获取公会信息失败 " + errCode);
                }
                break;

            case MSG_SET_GUILD_ANNOUNCEMENT:
                if (errCode == 0)
                {
                    //修改公告成功
                    GuildReq(MSG_GET_GUILD_ANNOUNCEMENT);
                }
                break;

            case MSG_GET_GUILD_ANNOUNCEMENT:
                if (errCode == 0)
                {
                    m_strTongNotice = (string)respInfo["1"];

                    TongUIViewManager.Instance.SetTongNotice(m_strTongNotice);
                    //获取公告成功
                }
                break;

            case MSG_APPLY_TO_JOIN:
                if (errCode == 0)
                {
                    //申请加入公会成功
                    Mogo.Util.LoggerHelper.Debug("尝试申请加入公会成功");

                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("尝试申请加入公会失败 " + errCode);
                }
                break;

            case MSG_APPLY_TO_JOIN_NOTIFY:
                break;

            case MSG_GET_GUILD_DETAILED_INFO:
                if (errCode == 0)
                {
                    //获取公会详细信息成功
                    m_strTongNotice = (string)respInfo["1"];
                    m_strTongMoney = (string)respInfo["2"];
                    m_strTongLevel = (string)respInfo["3"];
                    m_strTongPeopleNum = (string)respInfo["4"];
                    m_strTongBossName = (string)respInfo["5"];
                    m_iCurrenDragonPower = int.Parse((string)respInfo["6"]);

                    foreach (var item in GuildSkillData.dataMap)
                    {
                        if (!m_dictSkillIDToLevel.ContainsKey(item.Value.type))
                        {
                            Mogo.Util.LoggerHelper.Debug(item.Value.type.ToString() + " " + (LuaTable)respInfo["7"]);
                            m_dictSkillIDToLevel.Add(item.Value.type, int.Parse((string)((LuaTable)respInfo["7"])[item.Value.type.ToString()]));
                        }
                    }

                    TongUIViewManager.Instance.SetTitle(string.Concat(m_strTongName, "   Lv " ,m_strTongLevel));
                    TongUIViewManager.Instance.SetTongMoney("公会资金:" + m_strTongMoney);
                    TongUIViewManager.Instance.SetTongNum("公会人数:" + m_strTongPeopleNum+"/"+ GuildLevelData.dataMap[int.Parse(m_strTongLevel)].memberCount);
                    TongUIViewManager.Instance.SetTongName("会长:" + m_strTongBossName);
                    TongUIViewManager.Instance.SetTongNotice(m_strTongNotice);

                    if (IsShowMyTong)
                    {
                        TongUIViewManager.Instance.ShowMyTong();
                    }
                    else if (IsShowDragon)
                    {

                        foreach (var item in GuildDragonData.dataMap)
                        {
                            if (item.Value.guild_level.ToString() == m_strTongLevel)
                            {
                                int diamond = item.Value.diamond_recharge_cost;
                                int gold = item.Value.gold_recharge_cost;
                                TongUIViewManager.Instance.ShowDragonPower("88", m_iCurrenDragonPower, item.Value.dragon_limit, gold, diamond, diamond);
                                break;
                            }
                        }
                    }
                    else if (IsShowSkill)
                    {
                        m_listTongSkillUIData.Clear();

                        foreach (var item in GuildSkillData.dataMap)
                        {
                            TongUIViewManager.TongSkillData data = new TongUIViewManager.TongSkillData();
                            data.cost = item.Value.money.ToString();
                            data.effect1 = item.Value.add.ToString();
                            data.effect2 = item.Value.add.ToString();
                            //data.name = item.Value.type.ToString();

                            switch (item.Value.type)
                            {
                                case 1:
                                    data.name = LanguageData.GetContent(48405); // "攻击技能";
                                    break;

                                case 2:
                                    data.name = LanguageData.GetContent(48405); // "防守技能";
                                    break;

                                case 3:
                                    data.name = LanguageData.GetContent(48405); // "生命技能";
                                    break;
                            }
                            data.starNum = m_dictSkillIDToLevel[item.Value.type];

                            m_listTongSkillUIData.Add(data);

                            m_listTongSkillType.Add(item.Value.type);
                        }

                        TongUIViewManager.Instance.SetSkillList(m_listTongSkillUIData);
                        TongUIViewManager.Instance.ShowSkillList();
                    }
                    else
                    {
                        TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_MEMBERS, 1,
                                           uint.Parse(TongManager.Instance.m_strTongPeopleNum));
                    }

                    Mogo.Util.LoggerHelper.Debug("获取公会详细信息成功");
                }
                else
                {

                    Mogo.Util.LoggerHelper.Debug("获取公会详细信息失败 " + errCode);
                }
                break;

            case MSG_GET_GUILD_MESSAGES_COUNT:
                if (errCode == 0)
                {


                    int count = int.Parse((string)(respInfo["1"]));
                    Mogo.Util.LoggerHelper.Debug("获取公会请求信息数量成功 " + count);

                    GuildReq(MSG_GET_GUILD_MESSAGES, 1, (uint)count, "1");
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("获取公会请求信息数量失败 " + errCode);
                }
                break;

            case MSG_GET_GUILD_MESSAGES:
                if (errCode == 0)
                {
                    m_listTongApplicantData.Clear();
                    m_listTongApplicantUIData.Clear();

                    int count = int.Parse((string)(respInfo["1"]));

                    for (int i = 0; i < count; ++i)
                    {
                        TongApplicantData data = new TongApplicantData();

                        data.dbid = UInt64.Parse((string)((LuaTable)((LuaTable)respInfo["2"])[(i + 1).ToString()])["1"]);
                        data.name = (string)((LuaTable)((LuaTable)respInfo["2"])[(i + 1).ToString()])["2"];
                        data.job = int.Parse((string)((LuaTable)((LuaTable)respInfo["2"])[(i + 1).ToString()])["3"]);
                        data.level = int.Parse((string)((LuaTable)((LuaTable)respInfo["2"])[(i + 1).ToString()])["4"]);
                        data.power = int.Parse((string)((LuaTable)((LuaTable)respInfo["2"])[(i + 1).ToString()])["5"]);

                        m_listTongApplicantData.Add(data);

                        TongUIViewManager.ApplicantData uidata = new TongUIViewManager.ApplicantData();

                        uidata.name = data.name;
                        uidata.level = data.level.ToString();
                        uidata.power = data.power.ToString();
                        uidata.vocationIcon = IconData.GetHeadImgByVocation(data.job);

                        m_listTongApplicantUIData.Add(uidata);


                    }

                    TongUIViewManager.Instance.SetApplicantList(m_listTongApplicantUIData);
                    TongUIViewManager.Instance.ShowMyTongApplicantList();

                    Mogo.Util.LoggerHelper.Debug("获取公会请求列表成功 " + count);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("获取公会请求列表失败 " + errCode);
                }
                break;

            case MSG_ANSWER_APPLY:
                if (errCode == 0)
                {
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_MESSAGES_COUNT, 1);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("回应申请失败 " + errCode);
                }
                break;

            case MSG_INVITE:
                if (errCode == 0)
                {

                }
                break;

            case MSG_INVITED:
                if (errCode == 0)
                {

                }
                break;

            case MSG_ANSWER_INVITE:
                if (errCode == 0)
                {

                }
                break;

            case MSG_APPLY_TO_JOIN_RESULT:
                if (errCode == 0)
                {
                    //申请回应

                    int result = int.Parse((string)respInfo["1"]);

                    string tongName = (string)respInfo["2"];

                    if (result == 0)
                    {
                        MogoGlobleUIManager.Instance.Info(tongName + " JoinReq Success");
                        Mogo.Util.LoggerHelper.Debug("申请成功");
                        GuildReq(MSG_GET_GUILD_INFO);
                    }
                    else
                    {
                        MogoGlobleUIManager.Instance.Info(tongName + " JoinReq Fail");
                    }
                }
                break;

            case MSG_QUIT:
                if (errCode == 0)
                {

                }
                break;

            case MSG_PROMOTE:
                if (errCode == 0)
                {
                    Mogo.Util.LoggerHelper.Debug("提升职位成功");
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);
                    TongManager.Instance.IsShowMyTong = false;
                    TongManager.Instance.IsShowDragon = false;
                    TongManager.Instance.IsShowSkill = false;
                    TongUIViewManager.Instance.ShowMemberControlPanel(false);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("提升职位失败 " + errCode);
                }
                break;

            case MSG_DEMOTE:
                if (errCode == 0)
                {
                    Mogo.Util.LoggerHelper.Debug("减低职位成功");
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);
                    TongManager.Instance.IsShowMyTong = false;
                    TongManager.Instance.IsShowDragon = false;
                    TongManager.Instance.IsShowSkill = false;
                    TongUIViewManager.Instance.ShowMemberControlPanel(false);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("减低职位失败 " + errCode);
                }
                break;

            case MSG_EXPEL:
                if (errCode == 0)
                {
                    Mogo.Util.LoggerHelper.Debug("开除成功");
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);
                    TongManager.Instance.IsShowMyTong = false;
                    TongManager.Instance.IsShowDragon = false;
                    TongManager.Instance.IsShowSkill = false;
                    TongUIViewManager.Instance.ShowMemberControlPanel(false);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("开除失败 " + errCode);
                }
                break;

            case MSG_DEMISE:
                if (errCode == 0)
                {
                    Mogo.Util.LoggerHelper.Debug("转让成功");
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);
                    TongManager.Instance.IsShowMyTong = false;
                    TongManager.Instance.IsShowDragon = false;
                    TongManager.Instance.IsShowSkill = false;
                    TongUIViewManager.Instance.ShowMemberControlPanel(false);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("转让失败 " + errCode);
                }
                break;

            case MSG_DISMISS:
                if (errCode == 0)
                {

                }
                break;

            case MSG_THAW:
                if (errCode == 0)
                {

                }
                break;

            case MSG_RECHARGE:
                if (errCode == 0)
                {

                }
                break;

            case MSG_GET_GUILD_MEMBERS:
                if (errCode == 0)
                {
                    Mogo.Util.LoggerHelper.Debug("获取公会成员列表成功");

                    m_listTongMemberData.Clear();
                    m_listTongMemberUIData.Clear();

                    int count = respInfo.Count;

                    for (int i = 0; i < count; ++i)
                    {
                        TongUIViewManager.MemberData uidata = new TongUIViewManager.MemberData();
                        TongMemberData data = new TongMemberData();

                        data.dbid = uint.Parse((string)((LuaTable)(respInfo[(i + 1).ToString()]))["1"]);
                        data.name = (string)((LuaTable)(respInfo[(i + 1).ToString()]))["2"];
                        data.level = int.Parse((string)((LuaTable)(respInfo[(i + 1).ToString()]))["3"]);
                        data.jobId = int.Parse((string)((LuaTable)(respInfo[(i + 1).ToString()]))["4"]);
                        data.power = int.Parse((string)((LuaTable)(respInfo[(i + 1).ToString()]))["5"]);
                        data.contribute = int.Parse((string)((LuaTable)(respInfo[(i + 1).ToString()]))["6"]);
                        data.date = int.Parse((string)((LuaTable)(respInfo[(i + 1).ToString()]))["7"]);

                        m_listTongMemberData.Add(data);

                        uidata.name = data.name;
                        uidata.level = data.level.ToString();
                        uidata.contribution = data.contribute.ToString();
                        uidata.power = data.power.ToString();
                        uidata.date = Utils.GetTime(data.date).ToString("yyyy-MM-dd");

                        switch (data.jobId)
                        {
                            case 1:
                                uidata.position = LanguageData.GetContent(48400); // "公会长";
                                break;

                            case 2:
                                uidata.position = LanguageData.GetContent(48401); // "副会长1";
                                break;

                            case 3:
                                uidata.position = LanguageData.GetContent(48402); // "副会长2";
                                break;

                            case 4:
                                uidata.position = LanguageData.GetContent(48403); // "副会长3";
                                break;

                            default:
                                uidata.position = LanguageData.GetContent(48404); // "普通成员";
                                break;
                        }

                        m_listTongMemberUIData.Add(uidata);

                        TongUIViewManager.Instance.SetMemberList(m_listTongMemberUIData);
                        TongUIViewManager.Instance.ShowMyTongMemberList();
                    }
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("获取公会成员列表失败 " + errCode);
                }
                break;

            case MSG_GET_DRAGON:
                if (errCode == 0)
                {

                }
                break;

            case MSG_UPGRADE_GUILD_SKILL:
                if (errCode == 0)
                {

                }
                break;

            case MSG_GET_RECOMMEND_LIST:
                if (errCode == 0)
                {
                    Mogo.Util.LoggerHelper.Debug("获取推荐列表成功");

                    m_listTongPresenterUIData.Clear();
                    m_listTongPresenterData.Clear();

                    for (int i = 0; i < respInfo.Count; ++i)
                    {
                        TongPresenterData data = new TongPresenterData();

                        data.dbid = UInt64.Parse((string)((LuaTable)respInfo[(i + 1).ToString()])["1"]);
                        data.name = (string)((LuaTable)respInfo[(i + 1).ToString()])["2"];
                        data.level = int.Parse((string)((LuaTable)respInfo[(i + 1).ToString()])["3"]);
                        data.power = int.Parse((string)((LuaTable)respInfo[(i + 1).ToString()])["4"]);

                        TongUIViewManager.PresenterData uidata = new TongUIViewManager.PresenterData();

                        uidata.level = data.level.ToString();
                        uidata.name = data.name;
                        uidata.power = data.power.ToString();

                        m_listTongPresenterData.Add(data);
                        m_listTongPresenterUIData.Add(uidata);
                    }

                    TongUIViewManager.Instance.SetRecommendList(m_listTongPresenterUIData);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("获取推荐列表失败 " + errCode);
                }
                break;

            default:
                MogoGlobleUIManager.Instance.Info("回调消息id未定义 --!");
                break;

        }
    }
    #endregion
}
