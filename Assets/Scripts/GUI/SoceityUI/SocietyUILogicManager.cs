using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.GameData;
using TDBID = System.UInt64;

public class SocietyUILogicManager
{

    private static SocietyUILogicManager m_instance;

    public static SocietyUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new SocietyUILogicManager();
            }

            return SocietyUILogicManager.m_instance;

        }
    }

    //private MailManager m_mailMgr;

    //public MailManager GetMailManager()
    //{
    //    if (m_mailMgr == null)
    //    {
    //        m_mailMgr = MogoWorld.thePlayer.GetMailManager();
    //    }

    //    return m_mailMgr;
    //}

    public static class SocietyUIEvent
    {
        public const string FRIENDMESSAGEGRIDUP = "SocietyUI_FriendMessageGridUp";
        public const string LEAVEMESSAGESIGNUP = "SocietyUI_LeaveMessageSignUp";
        public const string WISHSTRENTHSIGNUP = "SocietyUI_WishStrenthSignUp";
        public const string ACCEPTADDFRIENDUP = "SocietyUI_AcceptAdddFriendUp";
        public const string REGECTADDFRIENDUP = "SocietyUI_RegectAddFriendUp";
        public const string MAILGRIDUP = "SocietyUI_MailGridUp";
        public const string SOCIETY_UI_OPEN = "SocietyUIOpen";
        public const string REFRESHMAILGRIDLIST = "SocietyUI_RefreshMailGridList";
        public const string READMAILRESP = "SocietyUI_ReadMailResp";
        public const string SHOWFRONTMAILUP = "SocietyUI_ShowFrontMailUp";
        public const string SHOWNEXTMAILUP = "SocietyUI_ShowNextMailUp";
    }


    void OnRefreshMailGridList()
    {                                  
        LoggerHelper.Debug("RefreshMailGridList " + MailManager.Instance.IsMailInfoDirty );

        if (MailManager.Instance.GetMailInfoList().Count > 0)
        {
            SocietyUIViewManager.Instance.ShowDeleteReadBtn(true);
            SocietyUIViewManager.Instance.ShowMailVolume(true);
            SocietyUIViewManager.Instance.ShowNoMailBoxText(false);
            SocietyUIViewManager.Instance.SetMailVolume(MailManager.Instance.GetMailInfoList().Count.ToString() + "/60");
        }
        else
        {
            SocietyUIViewManager.Instance.ShowDeleteReadBtn(false);
            SocietyUIViewManager.Instance.ShowMailVolume(false);
            SocietyUIViewManager.Instance.ShowNoMailBoxText(true);
        }


        if (true)
        {
            MailManager.Instance.IsMailInfoDirty = false;

            SocietyUIViewManager.Instance.ClearMailGridList();
            List<Mogo.Game.MailInfo> m_listMailInfo = MailManager.Instance.GetMailInfoList();
            //sort

            System.Comparison<Mogo.Game.MailInfo> com = new System.Comparison<Mogo.Game.MailInfo>(
              (a, b) =>
              {
                  if (a.state == b.state) return b.time - a.time;

                  if (a.state == MAIL_STATE.ATTACH_NO_READ) return -1;

                  if (b.state == MAIL_STATE.ATTACH_NO_READ) return 1;


                  if (a.state == MAIL_STATE.NO_ATTACH_NO_READ) return -1;

                  if (b.state == MAIL_STATE.NO_ATTACH_NO_READ) return 1;


                  if (a.state == MAIL_STATE.ATTACH_READ) return -1;

                  if (b.state == MAIL_STATE.ATTACH_READ) return 1;
                      

                  //默认按时间降序
                  return b.time - a.time;
              }
              );
            m_listMailInfo.Sort(com);

            Mogo.Util.LoggerHelper.Debug(m_listMailInfo.Count);


            for (int i = 0; i < m_listMailInfo.Count; ++i)
            {
                MailGridData md = new MailGridData();
                if (m_listMailInfo[i].mailType == MAIL_TYPE.ID)
                {
                    try
                    {
                        int fromId = System.Int32.Parse(m_listMailInfo[i].from);
                        md.name = LanguageData.GetContent(fromId);
                        int titleId = System.Int32.Parse(m_listMailInfo[i].title);
                        md.topic = LanguageData.GetContent(titleId);
                    }
                    catch (System.Exception ex)
                    {
                    	md.name = m_listMailInfo[i].from;
                        md.topic = m_listMailInfo[i].title;
                        LoggerHelper.Except(ex);
                    }
                }
                else
                {
                    md.name = m_listMailInfo[i].from;
                    md.topic = m_listMailInfo[i].title;
                }
                
                md.mailId = m_listMailInfo[i].id;

                switch (m_listMailInfo[i].state)
                {
                    case MAIL_STATE.NO_ATTACH_NO_READ:
                        md.isNoRead = true;
                        md.isAttachNoRead = false;
                        md.isAttachRead = false;
                        md.isRead = false;
                        break;

                    case MAIL_STATE.ATTACH_NO_READ:
                        md.isNoRead = false;
                        md.isAttachNoRead = true;
                        md.isAttachRead = false;
                        md.isRead = false;
                        break;

                    case MAIL_STATE.NO_ATTACH_READ:
                        md.isNoRead = false;
                        md.isAttachNoRead = false;
                        md.isAttachRead = false;
                        md.isRead = true;
                        break;

                    case MAIL_STATE.ATTACH_READ:
                        md.isNoRead = false;
                        md.isAttachNoRead = false;
                        md.isAttachRead = true;
                        md.isRead = false;
                        break;

                    case MAIL_STATE.RECV_ATTACH_READ:
                        md.isNoRead = false;
                        md.isAttachNoRead = false;
                        md.isAttachRead = false;
                        md.isRead = true;
                        break;

                }
                md.time = Utils.GetTime(m_listMailInfo[i].time).ToString("MM/dd");
                SocietyUIViewManager.Instance.AddMailGrid(md);
            }

            MailManager.Instance.IsMailInfoDirty = false;
        }
    }

    public Dictionary<TDBID, Mail> m_mailList = new Dictionary<TDBID, Mail>();// 邮件详细内容列表，用于拖动切换邮件
    void OnReadMailResp(Mail mail)
    {
        m_mailList[mail.mailId] = mail;

        LoggerHelper.Debug(mail.from + " " + mail.to);
        SocietyUIViewManager.Instance.ShowMailGridListDialog(false);
        SocietyUIViewManager.Instance.ShowMailDetailDialog(true);

        MailDetailData md = new MailDetailData();

        if (mail.state == MAIL_STATE.NO_ATTACH_NO_READ || mail.state == MAIL_STATE.NO_ATTACH_READ)
        {
            SocietyUIViewManager.Instance.ShowOneKeyGetItemBtn(false);
            SocietyUIViewManager.Instance.ShowMailItemGetSign(false);
        }
        else if (mail.state == MAIL_STATE.ATTACH_READ || mail.state == MAIL_STATE.ATTACH_NO_READ)
        {
            SocietyUIViewManager.Instance.ShowOneKeyGetItemBtn(true);
            SocietyUIViewManager.Instance.ShowMailItemGetSign(false);
        }
        else
        {
            SocietyUIViewManager.Instance.ShowOneKeyGetItemBtn(false);
            SocietyUIViewManager.Instance.ShowMailItemGetSign(true);
        }

        if (mail.mailType == MAIL_TYPE.ID)
        {
            try
            {
                int fromId = System.Int32.Parse(mail.from);
                md.senderName = LanguageData.GetContent(fromId);
                int titleId = System.Int32.Parse(mail.title);
                md.title = LanguageData.GetContent(titleId);
                int textId = System.Int32.Parse(mail.text);
                md.infoText = LanguageData.GetContent(textId, mail.args.ToArray());
            }
            catch (System.Exception ex)
            {
                md.infoText = mail.text;
                md.senderName = mail.from;
                md.title = mail.title;
            }
        }
        else
        {
            md.infoText = mail.text;
            md.senderName = mail.from;
            md.title = mail.title;
        } 
        md.reciverName = mail.to;
        
        md.time = mail.time.ToString();
       
        md.listItemImg = new List<string>();
        md.listItemNum = new List<int>();
        md.listItemID = new List<int>();

        Mogo.Util.LoggerHelper.Debug(mail);

        for (int i = 0; i < mail.items.Count; ++i)
        {
          //  Mogo.Util.LoggerHelper.Debug(ItemParentData.GetItem(mail.items[i]).Icon);
            ItemParentData data = ItemParentData.GetItem(mail.items[i]);
            if (data != null)
            {
                md.listItemImg.Add(data.Icon);
                md.listItemID.Add(mail.items[i]);
            }
            else
                md.listItemImg.Add("");
            Mogo.Util.LoggerHelper.Debug(mail.nums.Count);
            md.listItemNum.Add(mail.nums[i]);
        }

        md.mailId = mail.mailId;
        SocietyUIViewManager.Instance.FillMailDetailData(md);

        if (mail.state == MAIL_STATE.NO_ATTACH_NO_READ || mail.state == MAIL_STATE.NO_ATTACH_READ)
        {
            SocietyUIViewManager.Instance.ShowOneKeyGetItemBtn(false);
            SocietyUIViewManager.Instance.ShowMailItemGetSign(false);
        }
        else if (mail.state == MAIL_STATE.ATTACH_READ || mail.state == MAIL_STATE.ATTACH_NO_READ)
        {
            SocietyUIViewManager.Instance.ShowOneKeyGetItemBtn(true);
            SocietyUIViewManager.Instance.ShowMailItemGetSign(false);
        }
        else
        {
            SocietyUIViewManager.Instance.ShowOneKeyGetItemBtn(false);
            SocietyUIViewManager.Instance.ShowMailItemGetSign(true);
        }
    }

    void OnMailGridUp(TDBID mailId)
    {
        LoggerHelper.Debug("MailGridUp " + mailId);
        MailManager.Instance.MailReadReq(mailId);

    }

    void OnDeleteMailGrid(TDBID mailId)
    {
        MailManager.Instance.MailDelReq(mailId);
    }

    void OnDetailMailDeleteUp(TDBID mailId)
    {
        MailManager.Instance.MailDelReq(mailId);
    }

    void OnMailDetialQuitUp()
    {
        MailManager.Instance.MailInfoReq();
    }

    void OnMailDetailGetItemUp(TDBID mailId)
    {
        MailManager.Instance.MailAttachGetReq(mailId);
    }

    void OnMailFrontUp(TDBID mailId)
    {
		LoggerHelper.Debug(mailId);
		OnMailGridUp(mailId);
    }

    void OnMailNextUp(TDBID mailId)
    {
		LoggerHelper.Debug(mailId);
		OnMailGridUp(mailId);
    }

    void OnOneKeyGetBlessUseOKBtn()//kevin
    {
        SocietyUIViewManager.Instance.ShowOneKeyGetBlessUseOKCancel(false);
    }

    public void Initialize()
    { 
        EventDispatcher.AddEventListener(SocietyUIEvent.REFRESHMAILGRIDLIST, OnRefreshMailGridList);
        EventDispatcher.AddEventListener<Mail>(SocietyUIEvent.READMAILRESP, OnReadMailResp);

        SocietyUIViewManager.Instance.MAILGRIDUP += OnMailGridUp;
        SocietyUIViewManager.Instance.DELETEMAILGRID += OnDeleteMailGrid;
        SocietyUIViewManager.Instance.DETAILMAILDELETEUP += OnDetailMailDeleteUp;
        SocietyUIViewManager.Instance.MAILDETAILQUITUP += OnMailDetialQuitUp;
        SocietyUIViewManager.Instance.MAILDETAILGETITEMUP += OnMailDetailGetItemUp;
        SocietyUIViewManager.Instance.MAILFRONTUP += OnMailFrontUp;
        SocietyUIViewManager.Instance.MAILNEXTUP += OnMailNextUp;

        SocietyUIViewManager.Instance.ONEKEYGETBLESSUSEOK += OnOneKeyGetBlessUseOKBtn;//kevin

        EventDispatcher.AddEventListener(SocietyUIEvent.SOCIETY_UI_OPEN, OnSocietyUIOpen);
       
    }

    public void Release()
    {
        EventDispatcher.RemoveEventListener(SocietyUIEvent.REFRESHMAILGRIDLIST, OnRefreshMailGridList);
        EventDispatcher.RemoveEventListener<Mail>(SocietyUIEvent.READMAILRESP, OnReadMailResp);
        SocietyUIViewManager.Instance.MAILGRIDUP -= OnMailGridUp;
        SocietyUIViewManager.Instance.DELETEMAILGRID -= OnDeleteMailGrid;
        SocietyUIViewManager.Instance.DETAILMAILDELETEUP -= OnDetailMailDeleteUp;
        SocietyUIViewManager.Instance.MAILDETAILQUITUP -= OnMailDetialQuitUp;
        SocietyUIViewManager.Instance.MAILDETAILGETITEMUP -= OnMailDetailGetItemUp;
        SocietyUIViewManager.Instance.MAILFRONTUP -= OnMailFrontUp;
        SocietyUIViewManager.Instance.MAILNEXTUP -= OnMailNextUp;

        SocietyUIViewManager.Instance.ONEKEYGETBLESSUSEOK -= OnOneKeyGetBlessUseOKBtn;//kevin

        EventDispatcher.RemoveEventListener(SocietyUIEvent.SOCIETY_UI_OPEN, OnSocietyUIOpen);
    }

    #region 调用后端方法


    //void OnSerarchFriend(string name)
    //{
    //    EventDispatcher.TriggerEvent<string>(FriendManager.ON_FRIEND_SEARCH, name);
    //}

    //void OnFriendAdd(int id)
    //{
    //    EventDispatcher.TriggerEvent<int>(FriendManager.ON_FRIEND_ADD, id);
    //}


    //void OnFriendDel(int id)
    //{
    //    EventDispatcher.TriggerEvent<int>(FriendManager.ON_FRIEND_DEL, id);
    //}

    //void OnFriendList()
    //{
    //    EventDispatcher.TriggerEvent(FriendManager.ON_FRIEND_LIST);
    //}

    //void OnFriendNoteTip()
    //{
    //    EventDispatcher.TriggerEvent(FriendManager.ON_FRIEND_NOTETIP);
    //}

    //void OnFriendNoteDel(int id)
    //{
    //    EventDispatcher.TriggerEvent<int>(FriendManager.ON_FRIEND_NOTETIP,id);
    //}

    //void OnFriendListREQ()
    //{
    //    EventDispatcher.TriggerEvent(FriendManager.ON_FRIEND_NOTETIP);
    //}

    //void OnFriendAcceptREQ(int id)
    //{
    //    EventDispatcher.TriggerEvent<int>(FriendManager.ON_FRIEND_ACCETPREQ, id);
    //}

    //void OnFriendRejectreq(int id)
    //{
    //    EventDispatcher.TriggerEvent<int>(FriendManager.ON_FRIEND_REJECTREQ, id);
    //}

    //void OnFriendNoteSend(int id, string context)
    //{
    //    EventDispatcher.TriggerEvent<int, string>(FriendManager.ON_FRIEND_NOTESEND, id, context);
    //}

    //void OnFriendNoteRead(int id)
    //{
    //    EventDispatcher.TriggerEvent<int>(FriendManager.ON_FRIEND_NOTEREAD, id);
    //}
    #endregion

    #region 事件触发回调接口
    void OnSocietyUIOpen()
    {
        if (SocietyUIViewManager.Instance == null)
        {
            return;
        }

        if (FriendManager.Instance.IsfriendListDirty)
        {
            SocietyUIViewManager.Instance.RefreshFriendList();
        }
        //if (FriendManager.Instance.IsAccepteFriendListDirty)
        //{
        //    SocietyUIViewManager.Instance.RefreshFriendQuestList();
        //}
        OnRefreshMailGridList();
    }
    #endregion
}
