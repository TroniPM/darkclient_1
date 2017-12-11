/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������FriendManager
// �����ߣ�billy zhao
// �޸����б�Win.J H
// �������ڣ�
// ģ�����������ѹ�����
//----------------------------------------------------------------*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mogo.GameData;
using Mogo.Util;
using Mogo.Game;
using System;
using TDBID = System.UInt64;

public class FriendManager
{
    #region ��Ա����
    public static FriendManager Instance;
    public const byte ERROR_FRIEND_SUCCESS = 0;
    /*public const byte ERROR_FRIEND_NOSUCHFRIEND = 1;
    public const byte ERROR_FRIEND_OTHERFRIENDLISTFILLED = 2;
    public const byte ERROR_FRIEND_MYFRIENDLISTFILLED = 3;
    public const byte ERROR_FRIEND_ALREADYMYFRIEND = 4;
    public const byte ERROR_FRIEND_NONOTE = 5;*/
    public const byte ERR_FRIEND_NOT_EXISTS             = 1;
    public const byte ERR_FRIEND_REQ_NOT_EXISTS         = 2;
    public const byte ERR_FRIEND_ALREADY_HAS            = 3;
    public const byte ERR_FRIEND_FULL                   = 4;
    public const byte ERR_FRIEND_NOT_MY_FRIEND          = 5;
    public const byte ERR_FRIEND_BLESS_CDING            = 6; //�Ե����˵�ף��CD��
    public const byte ERR_FRIEND_BLESS_GET_FULL         = 7; //��ȡ��ף������
    public const byte ERR_FRIEND_BLESS_NOT_EXISTS       = 8; //ף��������
    public const byte ERR_FRIEND_RECV_BLESS_FULL        = 9; //ף����ȡ�ɹ������տ���ȡף������

    public const string ON_FRIEND_ADD = "FriendAddReq";
    public const string ON_FRIEND_DEL = "FriendDelReq";
    public const string ON_FRIEND_LIST = "FriendListReq";
    public const string ON_FRIEND_SEARCH = "FriendResearchReq";

    public const string ON_FRIENDREQ_LIST = "FriendReqListReq";
    public const string ON_FRIEND_ACCETPREQ = "FriendAcceptReq";
    public const string ON_FRIEND_REJECTREQ = "FriendRejectReq";
    public const string ON_FRIEND_NOTESEND = "FriendSendNoteReq";

    public const string ON_FRIEND_NOTEREAD = "FriendReadNoteReq";
    //����
    public const string ON_FRIEND_BLESS = "FriendBlessReq";
    public const string ON_FRIEND_BLESS_RECV = "FriendRecvBlessReq";
    public const string ON_FRIEND_BLESS_RECV_ALL = "FriendRecvAllBlessReq";

    //ûʵ��
    public const string ON_FRIEND_NOTETIP = "FriendNoteReq";
    public const string ON_FRIEND_NOTEDEL = "FriendNotedel";


    EntityMyself myself;
    //Dictionary<int, Dictionary<int, FriendData>> friendDataDic = new Dictionary<int, Dictionary<int, FriendData>>();

    public static List<FriendMessageGridData> m_friendList = new List<FriendMessageGridData>();
    public bool IsfriendListDirty { get; set; } //���ڿ��ƺ����б��ˢ��

    static List<FriendQuestAddGridData> m_acceptFriendList = new List<FriendQuestAddGridData>();
    public bool IsAccepteFriendListDirty { get; set; } //���ڿ��ƺ��������б��ˢ��

    static FriendData m_lastResearchFriend = new FriendData();


    int EnergyGetToday { get; set; } //��������ȡ����
    int RefreshNextTime { get; set; } //��һ������ʱ��
    #endregion

    #region ��Ҫ����
    public FriendManager(EntityMyself _myself)
    {
        myself = _myself;
        AddListeners();
        //������Ϸ��ʱ�����������Ϣ,add by Win.J H
        ReqFriendList();
        FriendReqListReq();
        Instance = this;
    }

    private void AddListeners()
    {
        EventDispatcher.AddEventListener<TDBID>(ON_FRIEND_ADD, AddFriendByID);
        EventDispatcher.AddEventListener<TDBID>(ON_FRIEND_DEL, DelFriendByID);
        //EventDispatcher.AddEventListener(ON_FRIEND_LIST, ReqFriendList);
        EventDispatcher.AddEventListener<string>(ON_FRIEND_SEARCH, ResearchFriendByName);

        EventDispatcher.AddEventListener(ON_FRIENDREQ_LIST, FriendReqListReq);
        EventDispatcher.AddEventListener<TDBID>(ON_FRIEND_ACCETPREQ, AcceptFriendReq);
        EventDispatcher.AddEventListener<TDBID>(ON_FRIEND_REJECTREQ, RejectFriendReq);
        EventDispatcher.AddEventListener<TDBID, string>(ON_FRIEND_NOTESEND, SendFriendNote);

        EventDispatcher.AddEventListener<TDBID>(ON_FRIEND_NOTEREAD, ReadFriendReq);
        EventDispatcher.AddEventListener<TDBID>(ON_FRIEND_BLESS, BlessReq);
        EventDispatcher.AddEventListener<TDBID>(ON_FRIEND_BLESS_RECV, RecvBlessReq);
        EventDispatcher.AddEventListener(ON_FRIEND_BLESS_RECV_ALL, RecvAllBlessReq);

        EventDispatcher.AddEventListener(ON_FRIEND_NOTETIP, ReqFriendNote);
        EventDispatcher.AddEventListener<TDBID>(ON_FRIEND_NOTEDEL, DelFriendNote);   
        
    }

    public void RemoveListener()
    {
        EventDispatcher.RemoveEventListener<TDBID>(ON_FRIEND_ADD, AddFriendByID);
        EventDispatcher.RemoveEventListener<TDBID>(ON_FRIEND_DEL, DelFriendByID);
        //EventDispatcher.RemoveEventListener(ON_FRIEND_LIST, ReqFriendList);
        EventDispatcher.RemoveEventListener<string>(ON_FRIEND_SEARCH, ResearchFriendByName);

        EventDispatcher.RemoveEventListener(ON_FRIENDREQ_LIST, FriendReqListReq);
        EventDispatcher.RemoveEventListener<TDBID>(ON_FRIEND_ACCETPREQ, AcceptFriendReq);
        EventDispatcher.RemoveEventListener<TDBID>(ON_FRIEND_REJECTREQ, RejectFriendReq);
        EventDispatcher.RemoveEventListener<TDBID, string>(ON_FRIEND_NOTESEND, SendFriendNote);

        EventDispatcher.RemoveEventListener<TDBID>(ON_FRIEND_NOTEREAD, ReadFriendReq);
        EventDispatcher.RemoveEventListener<TDBID>(ON_FRIEND_BLESS, BlessReq);
        EventDispatcher.RemoveEventListener<TDBID>(ON_FRIEND_BLESS_RECV, RecvBlessReq);
        EventDispatcher.RemoveEventListener(ON_FRIEND_BLESS_RECV_ALL, RecvAllBlessReq);

        EventDispatcher.RemoveEventListener(ON_FRIEND_NOTETIP, ReqFriendNote);
        EventDispatcher.RemoveEventListener<TDBID>(ON_FRIEND_NOTEDEL, DelFriendNote); 
    }

    /// <summary>
    /// ˢ��UI
    /// </summary>
    private void RefreshUI()
    {
        LoggerHelper.Debug("Refresh ui");
    }

    /// <summary>
    /// ���������Ϣ��ʾ״̬�ͺ����Ϸ�tabҳ��Ϣ��ʾ
    /// </summary>
    public void FreshTipUI()
    {
        bool IsFriendTipShow = false;
        foreach (FriendMessageGridData fd in m_friendList)
        {
            if (fd.isMessage || fd.isWishByFriend)
            {
                IsFriendTipShow = true;
                break;
            }
        }

        int count = FriendManager.Instance.GetAcceptFriendList().Count;
        if (count > 0)
        {
            IsFriendTipShow = true;
        }
        /*
        bool IsMailTipShow = false;
        foreach (MailInfo mf in MailManager.Instance.GetMailInfoList())
        {
            if (mf.state < MAIL_STATE.NO_ATTACH_READ)
            {
                IsMailTipShow = true;
                break;
            }
        }
        */
        //���ѻ����ʼ���Ϣ������ʾ
        if (NormalMainUIViewManager.Instance != null)
        {
            NormalMainUIViewManager.Instance.ShowSocialTip(IsFriendTipShow);
        }
        if (MenuUIViewManager.Instance != null)
        {
            MenuUIViewManager.Instance.ShowSocialTip(IsFriendTipShow);
        }
    }

    private void InitFriendList()
    {//todo

    }

    /// <summary>
    /// �õ�ȫ�ֺ����б�
    /// </summary>
    /// <returns></returns>
    public List<FriendMessageGridData> GetFriendList()
    {
        return m_friendList;
    }

    public List<FriendQuestAddGridData> GetAcceptFriendList()
    {
        return m_acceptFriendList;
    }

    public FriendData GetResearchFriendData()
    {
        return m_lastResearchFriend;
    }

    public FriendMessageGridData GetFriendInfo(TDBID id)
    {
        foreach (FriendMessageGridData fd in m_friendList)
        {
            if (id == fd.id)
                return fd;
        }
        return null;
    }
    #endregion

    #region  RPC���ú�˷���

    public void ResearchFriendByName(string name)
    {
        LoggerHelper.Debug("Research name:" + name);
        myself.RpcCall(ON_FRIEND_SEARCH, name);
    }

    public void AddFriendByID(TDBID DbID)
    {
        LoggerHelper.Debug("Add Friend:" + DbID);
        myself.RpcCall(ON_FRIEND_ADD, DbID);
    }

    public void DelFriendByID(TDBID DbID)
    {
        LoggerHelper.Debug("Del Friend:" + DbID);
        myself.RpcCall(ON_FRIEND_DEL, DbID);
    }

    public void ReqFriendList()
    {
        LoggerHelper.Debug("FriendManager::ReqFriendList");
        myself.RpcCall(ON_FRIEND_LIST);
    }

    /// <summary>
    /// ����Ӻ��ѵ������б�
    /// </summary>
    public void FriendReqListReq()
    {
        LoggerHelper.Debug("FriendManager::ReqFriendReqList");
        myself.RpcCall(ON_FRIENDREQ_LIST);
    }

    public void AcceptFriendReq(TDBID DbID)
    {
        LoggerHelper.Debug("AcceptFriendReq : id" + DbID);
        myself.RpcCall(ON_FRIEND_ACCETPREQ, DbID);
        FriendQuestAddGridData tmp = null;
        foreach (FriendQuestAddGridData fd in m_acceptFriendList)
        {
            if (DbID == fd.id)
            {
                tmp = fd;
            }
        }
        if (tmp != null)
        {
            m_acceptFriendList.Remove(tmp);
            IsAccepteFriendListDirty = true;
            Mogo.Util.LoggerHelper.Debug("reject ok.");
        }
        if (SocietyUIViewManager.Instance)
        {
            Mogo.Util.LoggerHelper.Debug("reject refresh ui.");
            SocietyUIViewManager.Instance.RefreshFriendQuestList();
        }
        else
        {
            //������Ǵ�δ�򿪹��罻����ʱ
            FreshTipUI();
        }
    }


    public void RejectFriendReq(TDBID DbID)
    {
        LoggerHelper.Debug("RejectFriendReq : id" + DbID);
        myself.RpcCall(ON_FRIEND_REJECTREQ, DbID);
        FriendQuestAddGridData tmp = null;
        foreach (FriendQuestAddGridData fd in m_acceptFriendList)
        {
            if (DbID == fd.id)
            {
                tmp = fd ;
            }
        }
        if (tmp != null)
        {
            m_acceptFriendList.Remove(tmp);
            IsAccepteFriendListDirty = true;
            Mogo.Util.LoggerHelper.Debug("reject ok.");
        }
        if (SocietyUIViewManager.Instance)
        {
            Mogo.Util.LoggerHelper.Debug("reject refresh ui.");
            SocietyUIViewManager.Instance.RefreshFriendQuestList();
        }
        else
        {
            //������Ǵ�δ�򿪹��罻����ʱ
            FreshTipUI();
        }
    }

    public void SendFriendNote(TDBID DbID, string context)
    {
        LoggerHelper.Debug("SendFriendNote : id" + DbID + "  context : " + context);
        int strLen = context.Length;
        if (strLen > 300)
        {
            ShowTextID(2, 738);
            return;
        }
        myself.RpcCall(ON_FRIEND_NOTESEND, DbID, context);
    }

    public void ReadFriendReq(TDBID DbID)
    {
        LoggerHelper.Debug("RejectFriendReq : id" + DbID);
        myself.RpcCall(ON_FRIEND_NOTEREAD, DbID);
    }

    public void BlessReq(TDBID DbID)
    {
        LoggerHelper.Debug("BlessReq : id" + DbID);
        myself.RpcCall(ON_FRIEND_BLESS, DbID);
    }

    public void RecvBlessReq(TDBID DbID)
    {
        LoggerHelper.Debug("RecvBlessReq : id" + DbID);
        myself.RpcCall(ON_FRIEND_BLESS_RECV, DbID);
    }

    public void RecvAllBlessReq()
    {
        LoggerHelper.Debug("RecvAllBlessReq ");
        myself.RpcCall(ON_FRIEND_BLESS_RECV_ALL);
    }
    //
    public void ReqFriendNote()
    {
        LoggerHelper.Debug("FriendNote");
        myself.RpcCall("FriendNoteReq");
    }

    public void DelFriendNote(TDBID DbID)
    {
        LoggerHelper.Debug("DelFriendNote : id" + DbID);
        myself.RpcCall("FriendNotedel", DbID);
    }
    #endregion

    #region �����ص�
    public void OnFriendRecvBeAddResp(LuaTable luatable)
    {
        //��������һ�κ��������б�
        FriendReqData fr;
        FriendQuestAddGridData fa = new FriendQuestAddGridData();
        if (Utils.ParseLuaTable(luatable, out fr))
        {
            fa.id = fr.id;
            fa.level = Convert.ToString(fr.level);
            fa.name = fr.name;
            fa.headImg = "";//base on fr.vocation
        }
        else
        {
            LoggerHelper.Error("OnFriendRecvBeAddResp");
            return;
        }
        //todo:��ʾ�к�������
        foreach (FriendQuestAddGridData d in m_acceptFriendList)
        {
            if(d.id == fa.id)
            {
                return;
            }
        }
        m_acceptFriendList.Add(fa);
        IsAccepteFriendListDirty = true;
        if (SocietyUIViewManager.Instance)
        {
            SocietyUIViewManager.Instance.RefreshFriendQuestList();
        }
        else
        {
            //������Ǵ�δ�򿪹��罻����ʱ
            FreshTipUI();
        }
    }
    /*
    public void OnFriendTipResp(UInt32 id, UInt32 isOnline)
    {
        foreach (FriendMessageGridData fd in m_friendList)
        {
            //
            if (id == fd.id)
            {
                fd.isOnline = isOnline == 0 ? true : false;
                IsfriendListDirty = true;
                break;
            }
        }
        if (SocietyUIViewManager.Instance)
        {
            SocietyUIViewManager.Instance.RefreshFriendList();
        }
        else
        {
            //������Ǵ�δ�򿪹��罻����ʱ
            FreshTipUI();
        }
    }
    */
    public void OnFriendRecvNoteResp(LuaTable luatable)
    {
        FriendNoteData fnd;
        if (Utils.ParseLuaTable(luatable, out fnd))
        {

        }
        else
        {
            LoggerHelper.Error("OnFriendRecvNoteResp");
            return;
        }
        foreach (FriendMessageGridData fd in m_friendList)
        {
            //
            if (fnd.id == fd.id)
            {
                fd.isMessage = true;
                //todo:��ʾ�к�������
                IsfriendListDirty = true;
                break;
            }
        }
        if (SocietyUIViewManager.Instance)
        {
            SocietyUIViewManager.Instance.RefreshFriendList();
        }
        else
        {
            //������Ǵ�δ�򿪹��罻����ʱ
            FreshTipUI();
        }
    }
    public void OnFriendBeBlessResp(LuaTable luatable)
    {
        List<TDBID> blessId;
        if (Utils.ParseLuaTable(luatable, out blessId))
        {

        }
        else
        {
            LoggerHelper.Error("OnFriendRecvNoteResp");
            return;
        }
        foreach (FriendMessageGridData fd in m_friendList)
        {
            //
            if (blessId[0] == fd.id)
            {
                fd.isWishByFriend = true;
                //todo:��ʾ�к���ף��
                IsfriendListDirty = true;
                break;
            }
        }
        if (SocietyUIViewManager.Instance)
        {
            SocietyUIViewManager.Instance.RefreshFriendList();
        }
        else
        {
            //������Ǵ�δ�򿪹��罻����ʱ
            FreshTipUI();
        }
    }

    #endregion

    #region ��˻ص�ǰ�˷���

    /// <summary>
    /// �����������б�
    /// </summary>
    /// <param name="luaTable"></param>
    /// <param name="msg"></param>
    public void FriendResearchReqResp(LuaTable luaTable, int msg)
    {
        FriendData obj;
        if (Utils.ParseLuaTable(luaTable, out obj))
        {
            m_lastResearchFriend = obj;
            SocietyUIViewManager.Instance.FriendResearchResp(m_lastResearchFriend, msg);
        }
        else
        {
            m_lastResearchFriend.id = 0;
            m_lastResearchFriend.name = "";
        }
    }

    public void FriendAddReqResp(int msg)
    {
        LoggerHelper.Debug("FriendAddReqResp :" + msg);

    }

    public void FriendDelResp(int msg)
    {
        LoggerHelper.Debug("FriendDelResp : " + msg);
        if(ERROR_FRIEND_SUCCESS != msg)
        {
            //todo:show error info
            return;
        }
        ReqFriendList();
    }

    /// <summary>
    /// �����б�
    /// </summary>
    /// <param name="luaTable"></param>
    /// <param name="msg"></param>
    public void FriendListResp(LuaTable luaTable, int msg)
    {
        List<FriendData> friendData;
        if (Utils.ParseLuaTable(luaTable, out friendData))
        {
            LoggerHelper.Debug("ParseLuaTable friends succeed.");
            //SocietyUIViewManager.Instance.SetFriendList(args);
            /*
            Comparison<FriendData> com = new Comparison<FriendData>(
               (a, b) =>
               {
                   if (a.IsNote > 0 && b.IsNote == 0)
                   {
                       return -1;
                   }
                   else if (a.IsNote == 0 && b.IsNote > 0)
                   {
                       return 1;
                   }
                   else
                   {
                       return b.fight - a.fight;
                   }
               }
               );
            friendData.Sort(com);
            */
            m_friendList.Clear();
            foreach (FriendData fd in friendData)
            {
                FriendMessageGridData fm = new FriendMessageGridData();
                fm.id = fd.id;
                if (fd.IsOnline != 0)
                {
                    fm.headImg = "";//base on the fd.vocation and fd.IsOnline
                }

                if (fd.degree < 10)
                    fm.degreeNum = 1;
                else if (fd.degree < 25)
                    fm.degreeNum = 2;
                else if (fd.degree <45)
                    fm.degreeNum = 3;
                else if (fd.degree < 70)
                    fm.degreeNum = 4;
                else
                    fm.degreeNum = 5;
   
                fm.name = fd.name;
                fm.level = Convert.ToString(fd.level);
                fm.power = Convert.ToString(fd.fight);
                fm.isMessage = fd.IsNote == 0 ? false: true;
                fm.isWishByFriend = fd.IsBlessedByFriend == 0 ? false : true;
                fm.isWithToFriend = fd.IsBlessedToFriend == 0 ? false : true;
                fm.isOnline = fd.IsOnline == 0 ? true : false;
                switch (fd.vocation)
                {
                    case 1:
                        {
                            fm.headImg = "zhanshi";
                            break;
                        }
                    case 2:
                        {
                            fm.headImg = "cike";
                            break;
                        }
                    case 3:
                        {
                            fm.headImg = "gongjianshou";
                            break;
                        }
                    default:
                        {
                            fm.headImg = "fashi";
                            break;
                        }
                }
                m_friendList.Add(fm);
            }
            IsfriendListDirty = true;
            if (SocietyUIViewManager.Instance)
            {
                SocietyUIViewManager.Instance.RefreshFriendList();
            }
            else
            {
                FreshTipUI();
            }
        }
        else
        {
            LoggerHelper.Debug("ParseLuaTable friends failed.");
        }
    }

    /// <summary>
    /// ���������б�
    /// </summary>
    /// <param name="luaTable"></param>
    /// <param name="msg"></param>
    public void FriendReqListResp(LuaTable luaTable, int msg)
    {
        m_acceptFriendList.Clear();
        List<FriendReqData> obj;
        if (Utils.ParseLuaTable(luaTable, out obj))
        {
            foreach(FriendReqData frd in obj)
            {
                FriendQuestAddGridData fad = new FriendQuestAddGridData();
                fad.id = frd.id;
                fad.headImg = ""; //base on the frd.vocation
                fad.name = frd.name;
                LoggerHelper.Debug("fad.name = " + fad.name);
                fad.level = Convert.ToString(frd.level);
                switch (frd.vocation)
                {
                    case 1:
                        {
                            fad.headImg = "zhanshi";
                            break;
                        }
                    case 2:
                        {
                            fad.headImg = "cike";
                            break;
                        }
                    case 3:
                        {
                            fad.headImg = "gongjianshou";
                            break;
                        }
                    default:
                        {
                            fad.headImg = "fashi";
                            break;
                        }
                }
                m_acceptFriendList.Add(fad);
            }
            IsAccepteFriendListDirty = true;
            //m_acceptFriendList = ;

            //SocietyUIViewManager.Instance.SetFriendItemList(args);

            //SocietyUIViewManager.Instance.SetFriendTipNum(AcceptFriendList.Count);
            //SocietyUIViewManager.Instance.SetAcceptFriendList(args);
            if (SocietyUIViewManager.Instance)
            {
                SocietyUIViewManager.Instance.RefreshFriendQuestList();
            }
            else
            {
                FreshTipUI();
            }
        }
        
    }

    public void FriendAcceptResp(int msg)
    {
        LoggerHelper.Debug("FriendAcceptResp:" + msg);
        //if (ERROR_FRIEND_SUCCESS == msg)
            //ReqFriendList();
    }

    public void FriendRejectResp(int msg)
    {
        LoggerHelper.Debug("FriendRejectResp: " + msg);
    }

    public void FriendSendNoteResp(int msg)
    {
        if (msg == ERROR_FRIEND_SUCCESS)
        {
            //todo:close window
            SocietyUIViewManager.Instance.OnReturnMessageDialogQuitUp(0);
        }
        LoggerHelper.Debug("FriendSendNoteResp:" + msg);
    }

    public void FriendReadNoteResp(LuaTable luaTable, int msg)
    {
        LoggerHelper.Debug("FriendReadNoteResp: " + msg);
        if (msg != ERROR_FRIEND_SUCCESS)
        {
            return;
        }
        List<FriendNoteData> obj;
        if (Utils.ParseLuaTable(luaTable, out obj))
        {
            if (obj.Count < 1)
                return;
            FriendMessageGridData fd = GetFriendInfo(obj[0].id);
            string name = fd.name;
            fd.isMessage = false;
            IsfriendListDirty = true;
            SocietyUIViewManager.Instance.RefreshFriendMessageList(name, obj);
        }
    }

    /// <summary>
    /// ף�����ѳɹ�
    /// </summary>
    /// <param name="friendId"></param>
    public void OnFriendBlessResp(TDBID friendId)
    {
        LoggerHelper.Debug("OnFriendBlessResp:" + friendId);

        FriendMessageGridData gridData = GetFriendInfo(friendId);
        if (gridData != null)
        {
            gridData.isWithToFriend = true;
            IsfriendListDirty = true;
            SocietyUIViewManager.Instance.RefreshFriendList();
        }
    }

    public void OnFriendRecvBlessResp(int energy, TDBID friendId, int msg)
    {
        LoggerHelper.Debug("OnFriendRecvBlessResp:" + msg);
        if (msg == ERROR_FRIEND_SUCCESS)
        {
            int id = ContentDefine.Friend.RECV_WISH_SUCCEED;
            if (LanguageData.dataMap.ContainsKey(id))
            {
                string s = LanguageData.GetContent(id, energy);//dataMap[id].Format(energy);
                ShowText(1, s);
            }
            FriendMessageGridData fd = GetFriendInfo(friendId);
            fd.isWishByFriend = false;
        }
        //��Ϊ����ͼ����ʧ������ˢһ��
        IsfriendListDirty = true;
        SocietyUIViewManager.Instance.RefreshFriendList();
        OnFriendErrorResp((byte)msg);
    }

    public void OnFriendRecvAllBlessResp(int energy, LuaTable luaTable, int msg)
    {
        LoggerHelper.Debug("OnFriendRecvAllBlessResp:" + msg);
        if (msg == ERROR_FRIEND_SUCCESS)
        {
            List<TDBID> obj;
            if (Utils.ParseLuaTable(luaTable, out obj))
            {
                foreach (TDBID id in obj)
                {
                    FriendMessageGridData fd = GetFriendInfo(id);
                    fd.isWishByFriend = false;
                    IsfriendListDirty = true;
                }
            }
            int msgid = ContentDefine.Friend.TEXT_RECV_ALL_BLESS_SUCCEED;
            if (LanguageData.dataMap.ContainsKey(msgid))
            {
                string s = LanguageData.GetContent(msgid, energy);//dataMap[msgid].Format(energy);
                ShowText(2, s);
            }
            IsfriendListDirty = true;
            SocietyUIViewManager.Instance.RefreshFriendList();

            SocietyUIViewManager.Instance.AddOneKeyGetBlessListUnit(obj);
            return;
        }
        OnFriendErrorResp((byte)msg);
    }
    #endregion

    #region ������ʾ
    /// <summary>
    ///�������صĴ�����
    /// </summary>
    /// <param name="errorId"></param>
    public void OnFriendErrorResp(byte errorId)
    {
        LoggerHelper.Debug("errorid:" + errorId);
        switch (errorId)
        {

            case ERROR_FRIEND_SUCCESS:

                break;
            /*case ERROR_FRIEND_NOSUCHFRIEND: break;
            case ERROR_FRIEND_OTHERFRIENDLISTFILLED: break;
            case ERROR_FRIEND_MYFRIENDLISTFILLED: break;
            case ERROR_FRIEND_ALREADYMYFRIEND: break;
            case ERROR_FRIEND_NONOTE: break;
            */
            case ERR_FRIEND_BLESS_GET_FULL:
                {
                    ShowTextID(2, ContentDefine.Friend.TEXT_RECV_BLESS_FULL);
                    break;
                }
            default:
                {
                    ShowTextID(2, errorId);
                    break;
                }
        }
    }

    private void ShowTextID(byte type, uint msgId)
    {
        int id = (int)msgId;
        string msg = "";
        if (LanguageData.dataMap.ContainsKey(id))
        {
            msg = LanguageData.GetContent(id);//dataMap[id].content;
        }
        else
        {
            msg = id + "";
        }

        ShowText(type, msg);
    }

    private void ShowText(byte type, string msg)
    {
        switch (type)
        {
            case 1:
                MogoMsgBox.Instance.ShowMsgBox(msg);
                break;
            case 2:
                MogoMsgBox.Instance.ShowFloatingText(msg);
                break;
            case 3:
                break;
        }
    }
    #endregion

}
