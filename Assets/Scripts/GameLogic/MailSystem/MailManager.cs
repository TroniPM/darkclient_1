/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MailManager
// 创建者：Win.J H
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

using TDBID = System.UInt64;

public class MailManager
{
    #region 错误码
    const byte ERR_MAIL_SUCCEED                  = 0;                //邮件操作成功
    const byte ERR_MAIL_NOT_EXISTS               = 1;                //邮件不存在
    const byte ERR_MAIL_NO_ATTACHMENT            = 2;                //邮件没有附件可领取
    const byte ERR_MAIL_ATTACHMENT_GETED         = 3;                //邮件附件已被领取过
    const byte ERR_MAIL_TIMEOUT                  = 4;                //邮件过期
    #endregion

    #region 后端RPC接口名称
    /*
    <!--申请邮件信息-->
    <MailInfoReq>
        <Exposed/>
    </MailInfoReq>
    <!--申请阅读邮件信息-->
    <MailReadReq>
        <Exposed/>
        <Arg> UINT32 </Arg>  <!-- mail id -->
    </MailReadReq>
    <!--申请删除邮件信息-->
    <MailDelReq>
        <Exposed/>
        <Arg> UINT32 </Arg>  <!-- mail id -->
    </MailDelReq>
    <!--申请领取邮件附件-->
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

    #region 成员变量
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

    #region 必备成员函数
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

    #region 后端回调接口
    /*
    <!--被动的接收到邮件通知-->
    <OnReceiveMailResp>
        <Arg> LUA_TABLE </Arg>  <!-- 邮件简单信息 -->
    </OnReceiveMailResp>
    <!--申请邮件信息-->
    <OnMailInfoResp>
        <Arg> LUA_TABLE </Arg>  <!-- 所有邮件简单信息 -->
    </OnMailInfoResp>
    <!--申请阅读邮件信息-->
    <OnMailReadResp>
        <Arg> LUA_TABLE </Arg>  <!-- 邮件具体信息 -->
        <Arg> UINT8 </Arg>  <!-- 错误码 -->
    </OnMailReadResp>
    <!--申请删除邮件信息-->
    <OnMailDelResp>
        <Arg> UINT8 </Arg>  <!-- 错误码 -->
    </OnMailDelResp>
    <!--申请领取邮件附件-->
    <OnMailAttachGetResp>
        <Arg> UINT8 </Arg>  <!-- 错误码 -->
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
            //todo:激活主界面上的头像闪动邮件图标
            //if (null == SocietyUIViewManager.Instance)
            {
                //该情况是从未打开过社交界面时
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
        
        //好友或者邮件信息的有提示
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
            //该情况是从未打开过社交界面时
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
            //todo:把mail显示在UI上
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
