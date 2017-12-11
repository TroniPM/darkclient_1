/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������MailManager
// �����ߣ�Win.J H
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

using TDBID = System.UInt64;

public class MailManager
{
    #region ������
    const byte ERR_MAIL_SUCCEED                  = 0;                //�ʼ������ɹ�
    const byte ERR_MAIL_NOT_EXISTS               = 1;                //�ʼ�������
    const byte ERR_MAIL_NO_ATTACHMENT            = 2;                //�ʼ�û�и�������ȡ
    const byte ERR_MAIL_ATTACHMENT_GETED         = 3;                //�ʼ������ѱ���ȡ��
    const byte ERR_MAIL_TIMEOUT                  = 4;                //�ʼ�����
    #endregion

    #region ���RPC�ӿ�����
    /*
    <!--�����ʼ���Ϣ-->
    <MailInfoReq>
        <Exposed/>
    </MailInfoReq>
    <!--�����Ķ��ʼ���Ϣ-->
    <MailReadReq>
        <Exposed/>
        <Arg> UINT32 </Arg>  <!-- mail id -->
    </MailReadReq>
    <!--����ɾ���ʼ���Ϣ-->
    <MailDelReq>
        <Exposed/>
        <Arg> UINT32 </Arg>  <!-- mail id -->
    </MailDelReq>
    <!--������ȡ�ʼ�����-->
    <MailAttachGetReq>
        <Exposed/>
        <Arg> UINT32 </Arg>  <!-- mail id -->
    </MailAttachGetReq>
    */
    public const string ON_MAIL_INFO_REQ       = "MailInfoReq";
    public const string ON_MAIL_READ_REQ       = "MailReadReq";
    public const string ON_MAIL_DEL_REQ        = "MailDelReq";
    public const string ON_MAIL_DEL_All_REQ    = "MailDelAllReq";
    public const string ON_MAIL_ATTACH_GET_REQ = "MailAttachGetReq";
    
    #endregion

    #region ��Ա����
    EntityMyself myself;
    private List<MailInfo> m_mailInfos = new List<MailInfo>();

    private static MailManager m_mailMgr;

    public static MailManager Instance
    {
        get
        {
            if (m_mailMgr == null)
            {
                m_mailMgr = new MailManager(MogoWorld.thePlayer);

            }

            return m_mailMgr; 
        }

    }

    public bool IsMailInfoDirty = true;
    #endregion

    #region �ر���Ա����
    public MailManager(EntityMyself _myself)
    {
        myself = _myself;
        //AddListeners();
        //
        MailInfoReq();
    }

    public List<MailInfo> GetMailInfoList()
    {
        return m_mailInfos;
    }

    //private void AddListeners()
    //{
    //}
    //private void RemoveListeners()
    //{
    //}
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
    public void MailInfoReq()
    {
        myself.RpcCall(ON_MAIL_INFO_REQ);
    }
    public void MailReadReq(TDBID mailId)
    {
        myself.RpcCall(ON_MAIL_READ_REQ, mailId);
    }
    public void MailDelReq(TDBID mailId)
    {
        myself.RpcCall(ON_MAIL_DEL_REQ, mailId);
    }
    public void MailDelAllReq()
    {
        myself.RpcCall(ON_MAIL_DEL_All_REQ);
    }
    public void MailAttachGetReq(TDBID mailId)
    {
        myself.RpcCall(ON_MAIL_ATTACH_GET_REQ, mailId);
    }
    #endregion

    #region ��˻ص��ӿ�
    /*
    <!--�����Ľ��յ��ʼ�֪ͨ-->
    <OnReceiveMailResp>
        <Arg> LUA_TABLE </Arg>  <!-- �ʼ�����Ϣ -->
    </OnReceiveMailResp>
    <!--�����ʼ���Ϣ-->
    <OnMailInfoResp>
        <Arg> LUA_TABLE </Arg>  <!-- �����ʼ�����Ϣ -->
    </OnMailInfoResp>
    <!--�����Ķ��ʼ���Ϣ-->
    <OnMailReadResp>
        <Arg> LUA_TABLE </Arg>  <!-- �ʼ�������Ϣ -->
        <Arg> UINT8 </Arg>  <!-- ������ -->
    </OnMailReadResp>
    <!--����ɾ���ʼ���Ϣ-->
    <OnMailDelResp>
        <Arg> UINT8 </Arg>  <!-- ������ -->
    </OnMailDelResp>
    <!--������ȡ�ʼ�����-->
    <OnMailAttachGetResp>
        <Arg> UINT8 </Arg>  <!-- ������ -->
    </OnMailAttachGetResp>
     */
    public void OnReceiveMailResp(LuaTable aMailInfo)
    {
        object obj;
        if (Utils.ParseLuaTable(aMailInfo, typeof(MailInfo), out obj))
        {
            MailInfo newMailInfo = obj as MailInfo;
            foreach(MailInfo i in m_mailInfos)
            {
                if (i.id == newMailInfo.id)
                {
                    return;
                }
            }
            m_mailInfos.Add(newMailInfo);
            IsMailInfoDirty = true;
            //todo:�����������ϵ�ͷ�������ʼ�ͼ��
            //if (null == SocietyUIViewManager.Instance)
            {
                //������Ǵ�δ�򿪹��罻����ʱ
                FreshTipUI();
            }
            EventDispatcher.TriggerEvent(SocietyUILogicManager.SocietyUIEvent.REFRESHMAILGRIDLIST);
        }
    }

    public void FreshTipUI()
    {
        /*
        bool IsFriendTipShow = false;
        foreach (FriendMessageGridData fd in m_friendList)
        {
            if (fd.isMessage || fd.isWish)
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
        */
        bool IsMailTipShow = false;
        foreach (MailInfo mf in MailManager.Instance.GetMailInfoList())
        {
            if (mf.state < MAIL_STATE.NO_ATTACH_READ)
            {
                IsMailTipShow = true;
                break;
            }
        }
        
        //���ѻ����ʼ���Ϣ������ʾ
        if (NormalMainUIViewManager.Instance != null)
        {
            NormalMainUIViewManager.Instance.ShowSocialTip(IsMailTipShow);
        }
        if (MenuUIViewManager.Instance != null)
        {
            MenuUIViewManager.Instance.ShowSocialTip(IsMailTipShow);
        }
        if (SocietyUIViewManager.Instance != null)
        {
            SocietyUIViewManager.Instance.ShowMailTip(IsMailTipShow);
        }
    }

    public void OnMailInfoResp(LuaTable mailInfo)
    {
        LoggerHelper.Debug("OnMailInfoResp");
        m_mailInfos.Clear();
        if (Utils.ParseLuaTable(mailInfo, out m_mailInfos))
        {
            LoggerHelper.Debug("OnMailInfoResp");
        }

        Mogo.Util.LoggerHelper.Debug("MailInfoList.Count = " + m_mailInfos.Count);
        
        //if (null == SocietyUIViewManager.Instance)
        {
            //������Ǵ�δ�򿪹��罻����ʱ
            FreshTipUI();
        }
       
        EventDispatcher.TriggerEvent(SocietyUILogicManager.SocietyUIEvent.REFRESHMAILGRIDLIST);
    }
    
    public void OnMailReadResp(LuaTable aMail, int errorId)
    {
        if (ERR_MAIL_SUCCEED != errorId)
        {
            ShowErrorView(errorId);
            return;
        }
        Mail theMail;
 
        if (Utils.ParseLuaTable(aMail, out theMail))
        {
            //todo:��mail��ʾ��UI��
            EventDispatcher.TriggerEvent<Mail>(SocietyUILogicManager.SocietyUIEvent.READMAILRESP, theMail);

        }
    }
    public void OnMailDelResp(int errorId)
    {
        if (ERR_MAIL_SUCCEED != errorId)
        {
            ShowErrorView(errorId);
            return;
        }
        //
        ShowErrorView(0, "delete succeed.");

        MailManager.Instance.MailInfoReq();

        Mogo.Util.LoggerHelper.Debug("Delete Mail Respppppppppppppppppp");
        IsMailInfoDirty = true;
        SocietyUIViewManager.Instance.ShowMailGridListDialog(true);
        SocietyUIViewManager.Instance.ShowMailDetailDialog(false);
    }
    public void OnMailAttachGetResp(int errorId)
    {
        if (ERR_MAIL_SUCCEED != errorId)
        {
            ShowErrorView(errorId);
            return;
        }
        //
        ShowErrorView(0, "Get attachment succeed.");
        SocietyUIViewManager.Instance.ShowOneKeyGetItemBtn(false);
        SocietyUIViewManager.Instance.PlayMailItemGetSignAnim();

        MailManager.Instance.MailInfoReq();
    }

    public bool GetGridByDBId(TDBID mailDBId, out TDBID mailGridId)
    {
        int index = 0;
        foreach(MailInfo i in m_mailInfos)
        {
            if (i.id == mailDBId)
            {
                mailGridId = (TDBID)index;
                return true;
            }

            index++;
        }
        mailGridId = 0;
        return false;
    }

    public void Clean()
    {
        m_mailInfos = new List<MailInfo>();
    }
    #endregion
}
