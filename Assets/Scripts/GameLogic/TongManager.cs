/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������TongManager
// �����ߣ�MaiFeo
// �޸����б�
// �������ڣ�
// ģ���������ʼ�������
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

    #region ������

    const UInt16 ERROR_CREATE_GUILD_LEVEL_TOO_LOW = 1;   //��������ʱ�ȼ�̫��
    const UInt16 ERROR_CREATE_GUILD_NOT_ENOUGH_DIAMOND = 2;   //��ʯ���㣬���ܴ����ؿ�
    const UInt16 ERROR_CREATE_GUILD_NAME_ALREADY_USED = 3;   //���������Ѿ���ռ��
    const UInt16 ERROR_CREATE_GUILD_ALREADY_IN_GUILD = 4;   //�������Ѿ�����һ�����ᣬ�����ٴ���
    const UInt16 ERROR_SET_ANNOUNCEMENT_NO_GUILD = 5;   //���û�й��᲻�������ù��ṫ��
    const UInt16 ERROR_SET_ANNOUNCEMENT_NO_RIGHT = 6;   //Ȩ�޲��㣬�������ù��ṫ��
    const UInt16 ERROR_GET_ANNOUNCEMENT_NO_GUILD = 7;   //���û�й��᲻�����ȡ���ṫ��
    const UInt16 ERROR_GET_GUILD_DETAILED_INFO_NO_GUILD = 8;   //û�й��ᣬ���ܻ�ȡ��ϸ��Ϣ
    const UInt16 ERROR_APPLY_TO_JOIN_ALREADY_IN_GUILD = 9;   //�Ѿ�����һ�����ᣬ����������
    const UInt16 ERROR_APPLY_TO_JOIN_GUILD_NOT_EXIT = 10;  //ָ���Ĺ��᲻���ڣ������������
    const UInt16 ERROR_GET_GUILD_MESSAGE_COUNT_NO_GUILD = 11;  //��ȡ������Ϣ����ʱ����û�й���
    const UInt16 ERROR_GET_GUILD_MESSAGE_COUNT_NO_RIGHT = 12;  //��ȡ������Ϣ����ʱ����û��Ȩ��
    const UInt16 ERROR_GET_GUILD_MESSAGES_NO_GUILD = 13;  //��ȡ������Ϣ����ʱ�������û�й���
    const UInt16 ERROR_GET_GUILD_MESSAGES_NO_RIGHT = 14;  //��ȡ������Ϣ����ʱ�������û��Ȩ��
    const UInt16 ERROR_ANSWER_APPLY_NO_GUILD = 15;  //��Ӧ������Ϣʱ���ֿͻ���û�й���
    const UInt16 ERROR_ANSWER_APPLY_NO_RIGHT = 16;  //��Ӧ������Ϣʱ���ֿͻ���û��Ȩ��
    const UInt16 ERROR_ANSWER_APPLY_NO_MESSAGE = 17;  //����û����Ϣ
    const UInt16 ERROR_ANSWER_APPLY_MESSAGE_NOT_EXIT = 18;  //ָ���Ĺ�����Ϣ������
    const UInt16 ERROR_INVITE_NO_RIGHT = 19;  //û��Ȩ�ޣ������������
    const UInt16 ERROR_INVITE_NO_EXIT = 20;  //���������Ҳ�����
    const UInt16 ERROR_INVITE_ALREADY_IN_GUILD = 21;  //�Է��Ѿ�����һ�������У���������
    const UInt16 ERROR_INVITE_NOT_EXIT = 22;  //���벻����
    const UInt16 ERROR_QUIT_ONLY_NORMAL_MEMBER = 23;  //ֻ�и���ͨ��Ա�����˳�
    const UInt16 ERROR_QUIT_NO_GUILD = 24;  //û�й��ᣬ�����˳�
    const UInt16 ERROR_APPLY_TO_JOIN_TOO_MUCH_MEMBERS = 25;  //���������Ѿ�����������
    const UInt16 ERROR_ANSWER_INVITE_TOO_MUCH_MEMBERS = 26;  //��Ӧ����ʱ���������Ѿ�����������
    const UInt16 ERROR_ANSWER_APPLY_TOO_MUCH_MEMBERS = 27;  //��Ӧ����ʱ���������Ѿ���������
    const UInt16 ERROR_PROMOTE_NO_RIGHT = 28;  //û��Ȩ�޲�������
    const UInt16 ERROR_PROMOTE_NOT_IN_GUILD = 29;  //Ҫ���ȼ���������Ҳ��ڹ���
    const UInt16 ERROR_PROMOTE_NOT_MYSELF = 30;  //���������Լ�
    const UInt16 ERROR_PROMOTE_FULL = 31;  //λ������������������
    const UInt16 ERROR_DEMOTE_NOT_MYSELF = 32;  //���ܰ��Լ���ְ
    const UInt16 ERROR_DEMOTE_NO_RIGHT = 33;  //û��Ȩ�޲��ܽ�ְ
    const UInt16 ERROR_DEMOTE_FULL = 34;  //λ�������������ٽ���
    const UInt16 ERROR_EXPEL_NOT_MYSELF = 35;  //���ܿ����Լ�
    const UInt16 ERROR_EXPEL_NO_RIGHT = 36;  //û��Ȩ�޲��ܿ���
    const UInt16 ERROR_DEMISE_NOT_MYSELF = 37;  //����ת�ø����Լ�
    const UInt16 ERROR_DEMISE_NO_RIGHT = 38;  //û��Ȩ�ޣ�����ת��
    const UInt16 ERROR_DISMISS_NO_RIGHT = 39;  //û��Ȩ�޽�ɢ����
    const UInt16 ERROR_DISMISS_ALREADY_IN_DELETED = 40;  //�Ѿ����붳��״̬������ɾ��
    const UInt16 ERROR_THAW_NOT_ENOUGH_DIAMOND = 41;  //������ʯ�ⶳ
    const UInt16 ERROR_THAW_NO_NEED = 42;  //û�ж��ᣬ����ⶳ
    const UInt16 ERROR_THAW_NO_GUILD = 43;  //û�й��ᣬ����ⶳ
    const UInt16 ERROR_ANSWER_APPLY_STATUS_FREEZE = 44;  //���ᴦ�ڶ���״̬�У����ܽ�������
    const UInt16 ERROR_ANSWER_INVITE_STATUS_FREEZE = 45;  //���ᴦ�ڶ���״̬�У����ܽ�������
    const UInt16 ERROR_APPLY_TO_JOIN_STATUS_FREEZE = 46;        //���ᴦ�ڶ���״̬�������������
    const UInt16 ERROR_RECHARGE_NOT_ENOUGH_DIAMOND = 47;  //������ʯ�ⶳ
    const UInt16 ERROR_RECHARGE_NOT_ENOUGH_GOLD = 48;  //û�ж��ᣬ����ⶳ
    const UInt16 ERROR_RECHARGE_NO_GUILD = 49;  //û�й��ᣬ����ⶳ
    const UInt16 ERROR_RECHARGE_OVER_LIMIT = 50;  //���ᴦ�ڶ���״̬�У����ܽ�������
    const UInt16 ERROR_RECHARGE_CAN_NOT = 51;  //���ᴦ�ڶ���״̬�У����ܽ�������
    const UInt16 ERROR_GET_GUILD_MEMBERS_NO_GUILD = 52;        //���ᴦ�ڶ���״̬�������������
    const UInt16 ERROR_RECHARGE_CD = 53;  //������ʯ�ⶳ
    const UInt16 ERROR_RECHARGE_DAY_TIMES = 54;  //û�ж��ᣬ����ⶳ
    const UInt16 ERROR_GET_DRAGON_NO_GUILD = 55;  //û�й��ᣬ����ⶳ
    const UInt16 ERROR_GET_DRAGON_NOT_FULL = 56;  //���ᴦ�ڶ���״̬�У����ܽ�������
    const UInt16 ERROR_GET_DRAGON_OVER_TIMES = 57;  //���ᴦ�ڶ���״̬�У����ܽ�������
    const UInt16 ERROR_UPGRADE_GUILD_SKILL_NO_SUCH_TYPE = 58;        //���ᴦ�ڶ���״̬�������������
    const UInt16 ERROR_UPGRADE_GUILD_SKILL_NO_GUILD = 59;  //û�й��ᣬ����ⶳ
    const UInt16 ERROR_UPGRADE_GUILD_SKILL_NO_RIGHT = 60;  //���ᴦ�ڶ���״̬�У����ܽ�������
    const UInt16 ERROR_UPGRADE_GUILD_SKILL_ALREADY_LIMIT = 61;  //���ᴦ�ڶ���״̬�У����ܽ�������
    const UInt16 ERROR_GET_GUILD_INFO_NO_GUILD = 62;        //���ᴦ�ڶ���״̬�������������

    #endregion

    #region ͨ��ID


    public const byte MSG_GET_GUILDS = 36;       //��ҳ��ȡ�������Ĺ���
    public const byte MSG_GET_GUILDS_COUNT = 37;       //��ȡ����ĸ���
    public const byte MSG_CREATE_GUILD = 38;       //����һ������
    public const byte MSG_GET_GUILD_INFO = 39;       //��ȡ����Լ��Ĺ�����Ϣ
    public const byte MSG_SET_GUILD_ANNOUNCEMENT = 40;       //���ù��ṫ��
    public const byte MSG_GET_GUILD_ANNOUNCEMENT = 41;       //��ȡ���ṫ��
    public const byte MSG_APPLY_TO_JOIN = 42;       //�������ָ������
    public const byte MSG_APPLY_TO_JOIN_NOTIFY = 43;       //��ȡ������ϸ��Ϣ
    public const byte MSG_GET_GUILD_DETAILED_INFO = 44;       //��ȡ������Ϣ������
    public const byte MSG_GET_GUILD_MESSAGES_COUNT = 45;      //��ҳ��ȡ������Ϣ
    public const byte MSG_GET_GUILD_MESSAGES = 46;      //��Ӧ����
    public const byte MSG_ANSWER_APPLY = 47;      //������Ѽ��빫��
    public const byte MSG_INVITE = 48;      //��Ӧ��������
    public const byte MSG_INVITED = 49;      //֪ͨ���ͻ���������
    public const byte MSG_ANSWER_INVITE = 50;      //֪ͨ�ͻ��˱�ĳ��������
    public const byte MSG_APPLY_TO_JOIN_RESULT = 51;      //�˳�����
    public const byte MSG_QUIT = 52;      //����
    public const byte MSG_PROMOTE = 53;      //��ְ
    public const byte MSG_DEMOTE = 54;      //����
    public const byte MSG_EXPEL = 55;      //ת��
    public const byte MSG_DEMISE = 56;      //��ɢ
    public const byte MSG_DISMISS = 57;      //�ⶳ
    public const byte MSG_THAW = 58;      //�ⶳ
    public const byte MSG_RECHARGE = 59;      //�ⶳ
    public const byte MSG_GET_GUILD_MEMBERS = 60;      //�ⶳ
    public const byte MSG_GET_DRAGON = 61;      //�ⶳ
    public const byte MSG_UPGRADE_GUILD_SKILL = 62;      //�ⶳ
    public const byte MSG_GET_RECOMMEND_LIST = 63;
    public const byte MSG_GET_DRAGON_INFO = 64;

    const byte MSG_GET_DRAGON_RESP = 252;     //��������ɹ���۳���Դ
    const byte MSG_RECHARGE_RESP = 253;     //֪ͨ�᳤�͸��᳤����������
    const byte MSG_SUBMIT_CREATE_GUILD_COST = 254;     //������ҵĹ���ID
    const byte MSG_SET_GUILD_ID = 254;     //������ҵĹ���ID
    #endregion

    #region ���RPC�ӿ�����
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

    #region ��Ա����
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

    #region �ر���Ա����

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
        //todo:����errorId��ʾ������Ϣ
        if (msg != null)
        {
            //�������msg����msg��ʾ��ʾ
        }
        else
        {
            //����errorId��ѯ������Ϣ
        }
    }
    #endregion

    #region ���ú��RPC�ӿ�

    public void GuildReq(byte id, UInt64 arg1 = 0, UInt64 arg2 = 0, string arg3 = "")
    {
        myself.RpcCall("GuildReq", id, arg1, arg2, arg3);
    }

    #endregion

    #region ��˻ص��ӿ�
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
                //Ӧlua_table��({1=����, 2={1={1=����dbid,2=����,3=�ȼ�,4=����}, ...}})
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
                Mogo.Util.LoggerHelper.Debug("��ȡ������������");
                if (errCode == 0)
                {
                    m_iTongNum = int.Parse((string)respInfo["1"]);

                    GuildReq(MSG_GET_GUILDS, 1, (uint)m_iTongNum);

                    Mogo.Util.LoggerHelper.Debug("��ȡ��������  " + m_iTongNum);
                }
                break;

            case MSG_CREATE_GUILD:
                if (errCode == 0)
                {
                    //��������ɹ�
                    //le��({1=������, 2=���������� 3=����ְλ})

                    //m_strTongName = (string)respInfo["1"];
                    //m_strTongPeopleNum = ((int)respInfo["2"]).ToString();

                    GuildReq(MSG_GET_GUILD_INFO);
                    Mogo.Util.LoggerHelper.Debug("��������ɹ�");

                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("��������ʧ�� errCode = " + errCode);
                }
                break;

            case MSG_GET_GUILD_INFO:
                if (errCode == 0 && (string)respInfo["1"] != "")
                {
                    //�л���������ϸ����
                    m_strTongName = (string)respInfo["1"];
                    GuildReq(MSG_GET_GUILD_DETAILED_INFO);
                    IsShowMyTong = true;
                    IsShowDragon = false;
                    IsShowSkill = false;

                    Mogo.Util.LoggerHelper.Debug("��ȡ������Ϣ�ɹ� " + errCode + " " + (string)respInfo["1"]);
                }
                //else if (errCode == ERROR_GET_GUILD_DETAILED_INFO_NO_GUILD)
                else
                {
                    GuildReq(MSG_GET_GUILDS_COUNT);
                    //�л��������б�

                    Mogo.Util.LoggerHelper.Debug("��ȡ������Ϣʧ�� " + errCode);
                }
                break;

            case MSG_SET_GUILD_ANNOUNCEMENT:
                if (errCode == 0)
                {
                    //�޸Ĺ���ɹ�
                    GuildReq(MSG_GET_GUILD_ANNOUNCEMENT);
                }
                break;

            case MSG_GET_GUILD_ANNOUNCEMENT:
                if (errCode == 0)
                {
                    m_strTongNotice = (string)respInfo["1"];

                    TongUIViewManager.Instance.SetTongNotice(m_strTongNotice);
                    //��ȡ����ɹ�
                }
                break;

            case MSG_APPLY_TO_JOIN:
                if (errCode == 0)
                {
                    //������빫��ɹ�
                    Mogo.Util.LoggerHelper.Debug("����������빫��ɹ�");

                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("����������빫��ʧ�� " + errCode);
                }
                break;

            case MSG_APPLY_TO_JOIN_NOTIFY:
                break;

            case MSG_GET_GUILD_DETAILED_INFO:
                if (errCode == 0)
                {
                    //��ȡ������ϸ��Ϣ�ɹ�
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
                    TongUIViewManager.Instance.SetTongMoney("�����ʽ�:" + m_strTongMoney);
                    TongUIViewManager.Instance.SetTongNum("��������:" + m_strTongPeopleNum+"/"+ GuildLevelData.dataMap[int.Parse(m_strTongLevel)].memberCount);
                    TongUIViewManager.Instance.SetTongName("�᳤:" + m_strTongBossName);
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
                                    data.name = LanguageData.GetContent(48405); // "��������";
                                    break;

                                case 2:
                                    data.name = LanguageData.GetContent(48405); // "���ؼ���";
                                    break;

                                case 3:
                                    data.name = LanguageData.GetContent(48405); // "��������";
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

                    Mogo.Util.LoggerHelper.Debug("��ȡ������ϸ��Ϣ�ɹ�");
                }
                else
                {

                    Mogo.Util.LoggerHelper.Debug("��ȡ������ϸ��Ϣʧ�� " + errCode);
                }
                break;

            case MSG_GET_GUILD_MESSAGES_COUNT:
                if (errCode == 0)
                {


                    int count = int.Parse((string)(respInfo["1"]));
                    Mogo.Util.LoggerHelper.Debug("��ȡ����������Ϣ�����ɹ� " + count);

                    GuildReq(MSG_GET_GUILD_MESSAGES, 1, (uint)count, "1");
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("��ȡ����������Ϣ����ʧ�� " + errCode);
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

                    Mogo.Util.LoggerHelper.Debug("��ȡ���������б�ɹ� " + count);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("��ȡ���������б�ʧ�� " + errCode);
                }
                break;

            case MSG_ANSWER_APPLY:
                if (errCode == 0)
                {
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_MESSAGES_COUNT, 1);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("��Ӧ����ʧ�� " + errCode);
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
                    //�����Ӧ

                    int result = int.Parse((string)respInfo["1"]);

                    string tongName = (string)respInfo["2"];

                    if (result == 0)
                    {
                        MogoGlobleUIManager.Instance.Info(tongName + " JoinReq Success");
                        Mogo.Util.LoggerHelper.Debug("����ɹ�");
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
                    Mogo.Util.LoggerHelper.Debug("����ְλ�ɹ�");
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);
                    TongManager.Instance.IsShowMyTong = false;
                    TongManager.Instance.IsShowDragon = false;
                    TongManager.Instance.IsShowSkill = false;
                    TongUIViewManager.Instance.ShowMemberControlPanel(false);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("����ְλʧ�� " + errCode);
                }
                break;

            case MSG_DEMOTE:
                if (errCode == 0)
                {
                    Mogo.Util.LoggerHelper.Debug("����ְλ�ɹ�");
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);
                    TongManager.Instance.IsShowMyTong = false;
                    TongManager.Instance.IsShowDragon = false;
                    TongManager.Instance.IsShowSkill = false;
                    TongUIViewManager.Instance.ShowMemberControlPanel(false);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("����ְλʧ�� " + errCode);
                }
                break;

            case MSG_EXPEL:
                if (errCode == 0)
                {
                    Mogo.Util.LoggerHelper.Debug("�����ɹ�");
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);
                    TongManager.Instance.IsShowMyTong = false;
                    TongManager.Instance.IsShowDragon = false;
                    TongManager.Instance.IsShowSkill = false;
                    TongUIViewManager.Instance.ShowMemberControlPanel(false);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("����ʧ�� " + errCode);
                }
                break;

            case MSG_DEMISE:
                if (errCode == 0)
                {
                    Mogo.Util.LoggerHelper.Debug("ת�óɹ�");
                    TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);
                    TongManager.Instance.IsShowMyTong = false;
                    TongManager.Instance.IsShowDragon = false;
                    TongManager.Instance.IsShowSkill = false;
                    TongUIViewManager.Instance.ShowMemberControlPanel(false);
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("ת��ʧ�� " + errCode);
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
                    Mogo.Util.LoggerHelper.Debug("��ȡ�����Ա�б�ɹ�");

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
                                uidata.position = LanguageData.GetContent(48400); // "���᳤";
                                break;

                            case 2:
                                uidata.position = LanguageData.GetContent(48401); // "���᳤1";
                                break;

                            case 3:
                                uidata.position = LanguageData.GetContent(48402); // "���᳤2";
                                break;

                            case 4:
                                uidata.position = LanguageData.GetContent(48403); // "���᳤3";
                                break;

                            default:
                                uidata.position = LanguageData.GetContent(48404); // "��ͨ��Ա";
                                break;
                        }

                        m_listTongMemberUIData.Add(uidata);

                        TongUIViewManager.Instance.SetMemberList(m_listTongMemberUIData);
                        TongUIViewManager.Instance.ShowMyTongMemberList();
                    }
                }
                else
                {
                    Mogo.Util.LoggerHelper.Debug("��ȡ�����Ա�б�ʧ�� " + errCode);
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
                    Mogo.Util.LoggerHelper.Debug("��ȡ�Ƽ��б�ɹ�");

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
                    Mogo.Util.LoggerHelper.Debug("��ȡ�Ƽ��б�ʧ�� " + errCode);
                }
                break;

            default:
                MogoGlobleUIManager.Instance.Info("�ص���Ϣidδ���� --!");
                break;

        }
    }
    #endregion
}
