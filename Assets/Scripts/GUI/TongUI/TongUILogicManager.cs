using UnityEngine;
using System.Collections;
using Mogo.GameData;
using Mogo.Util;
using System;

public class TongUILogicManager
{
    private static TongUILogicManager m_instance;

    public static TongUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TongUILogicManager();
            }

            return TongUILogicManager.m_instance;

        }
    }

    int m_iMyTongMemberTab = (int)MyTongMemberUITab.MyTongMemberTab1;
   public int MyTongMemberTab
   {
       get
       {
           return m_iMyTongMemberTab;
       }
       set
       {
           TongUIViewManager.Instance.HandleMyTongMemberTabChange(m_iMyTongMemberTab, value);
           m_iMyTongMemberTab = value;
       }
   }

    void OnCreateTong()
    {
        TongUIViewManager.Instance.ShowCreateTong("*500", true);
    }

    void OnJoinTog()
    {
        TongManager.Instance.GuildReq(TongManager.MSG_APPLY_TO_JOIN, m_selectTongID);
        Mogo.Util.LoggerHelper.Debug("³¢ÊÔÉêÇë¹«»á " + m_selectTongID);
    }

    UInt64 m_selectTongID = 0;

    void OnSelectTong(int id)
    {
        m_selectTongID = TongManager.Instance.GetDBIDFromGridID(id);

        if (m_selectTongID == 0)
        {
            Mogo.Util.LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ Not Contain GridID");
            return;
        }
    }

    void OnDismissTong()
    { }

    void OnModifyNotice()
    { }



    void OnRefuseApplicant(int id)
    {
        TongManager.Instance.GuildReq(TongManager.MSG_ANSWER_APPLY, 1, TongManager.Instance.GetDBIDFromApplicantListGridID(id));
    }

    void OnAcceptApplicant(int id)
    {
        TongManager.Instance.GuildReq(TongManager.MSG_ANSWER_APPLY, 0, TongManager.Instance.GetDBIDFromApplicantListGridID(id));
    }


    void OnSkillUpgrde(int id)
    {
        TongManager.Instance.GuildReq(TongManager.MSG_UPGRADE_GUILD_SKILL, TongManager.Instance.GetSkillTypeFromSkillListGridID(id));
    }

    void OnMagicUp(int id)
    { 
        //TongManager.Instance.GuildReq(
    }

    void OnCreateTongDone(string str)
    {
        TongManager.Instance.GuildReq(TongManager.MSG_CREATE_GUILD, 0, 0, str);
    }

    void OnTongShow()
    {
        TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_INFO);
        Mogo.Util.LoggerHelper.Debug("C2S GET_GUILD_INFO");
    }

    void OnSelectMemberTab(int id)
    {
        switch ((MyTongMemberUITab)id)
        {
            case MyTongMemberUITab.MyTongMemberTab1:
                TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);
                TongManager.Instance.IsShowMyTong = false;
                TongManager.Instance.IsShowDragon = false;
                TongManager.Instance.IsShowSkill = false;
                MyTongMemberTab = id;
                break;

            case MyTongMemberUITab.MyTongMemberTab2:
                TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_MESSAGES_COUNT, 1);
                MyTongMemberTab = id;
                break;

            case MyTongMemberUITab.MyTongMemberTab3:
                TongManager.Instance.GuildReq(TongManager.MSG_GET_RECOMMEND_LIST);
                MyTongMemberTab = id;
                break;
        }        
    }

    void OnShowDragon()
    {
        TongManager.Instance.IsShowDragon = true;
        TongManager.Instance.IsShowMyTong = false;
        TongManager.Instance.IsShowSkill = false;
        TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);        
    }

    void OnShowMember()
    {
        TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_MEMBERS, 1,
            uint.Parse(TongManager.Instance.m_strTongPeopleNum));
        MyTongMemberTab = (int)MyTongMemberUITab.MyTongMemberTab1;
    }

    void OnShowSkill()
    {
        TongManager.Instance.IsShowSkill = true;
        TongManager.Instance.IsShowDragon = false;
        TongManager.Instance.IsShowMyTong = false;
        TongManager.Instance.GuildReq(TongManager.MSG_GET_GUILD_DETAILED_INFO);      
    }

    void OnShowWar()
    {

    }

    void OnAbdicate(int id)
    {
        TongUIViewManager.Instance.ShowMemberControlPanel(false);
        MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(25567), (isOK) => 
        {
            if (isOK)
            {
                UInt64 dbid = TongManager.Instance.GetDBIDFromMemberListGridID(id);
                TongManager.Instance.GuildReq(TongManager.MSG_DEMISE, dbid);

                MogoGlobleUIManager.Instance.ConfirmHide();
            }
            else
            {
                MogoGlobleUIManager.Instance.ConfirmHide();
            }
        });
    }

    void OnMemberDemote(int id)
    {
        UInt64 dbid = TongManager.Instance.GetDBIDFromMemberListGridID(id);
        TongManager.Instance.GuildReq(TongManager.MSG_DEMOTE,dbid);
    }

    void OnMemberKick(int id)
    {
        UInt64 dbid = TongManager.Instance.GetDBIDFromMemberListGridID(id);
        TongManager.Instance.GuildReq(TongManager.MSG_EXPEL,dbid);
    }
    void OnMemberPromote(int id)
    {
        UInt64 dbid = TongManager.Instance.GetDBIDFromMemberListGridID(id);

        TongManager.Instance.GuildReq(TongManager.MSG_PROMOTE,dbid);
    }

    public void Initialize()
    {
        EventDispatcher.AddEventListener(TongUIViewManager.Event.OnCreateTong, OnCreateTong);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnSelectTong, OnSelectTong);
        EventDispatcher.AddEventListener(TongUIViewManager.Event.OnJoinTong, OnJoinTog);
        EventDispatcher.AddEventListener(TongUIViewManager.Event.OnDismissTong, OnDismissTong);
        EventDispatcher.AddEventListener(TongUIViewManager.Event.OnModifyNotice, OnModifyNotice);
        EventDispatcher.AddEventListener(TongUIViewManager.Event.OnShowDragon, OnShowDragon);
        EventDispatcher.AddEventListener(TongUIViewManager.Event.OnShowMember, OnShowMember);
        EventDispatcher.AddEventListener(TongUIViewManager.Event.OnShowSkill, OnShowSkill);
        EventDispatcher.AddEventListener(TongUIViewManager.Event.OnShowWar, OnShowWar);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnRefuseApplicant, OnRefuseApplicant);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnAcceptApplicant, OnAcceptApplicant);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnAbdicate, OnAbdicate);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnMemberDemote, OnMemberDemote);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnMemberKick, OnMemberKick);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnMemberPromote, OnMemberPromote);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnSkillUpgrde, OnSkillUpgrde);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnMagicUp, OnMagicUp);
        EventDispatcher.AddEventListener<string>(TongUIViewManager.Event.OnCreateTongDone, OnCreateTongDone);
        EventDispatcher.AddEventListener(TongUIViewManager.Event.OnTongShow, OnTongShow);
        EventDispatcher.AddEventListener<int>(TongUIViewManager.Event.OnSelectMemberTab, OnSelectMemberTab);
    }

    public void Release()
    {
        EventDispatcher.RemoveEventListener(TongUIViewManager.Event.OnCreateTong, OnCreateTong);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnSelectTong, OnSelectTong);
        EventDispatcher.RemoveEventListener(TongUIViewManager.Event.OnJoinTong, OnJoinTog);
        EventDispatcher.RemoveEventListener(TongUIViewManager.Event.OnDismissTong, OnDismissTong);
        EventDispatcher.RemoveEventListener(TongUIViewManager.Event.OnModifyNotice, OnModifyNotice);
        EventDispatcher.RemoveEventListener(TongUIViewManager.Event.OnShowDragon, OnShowDragon);
        EventDispatcher.RemoveEventListener(TongUIViewManager.Event.OnShowMember, OnShowMember);
        EventDispatcher.RemoveEventListener(TongUIViewManager.Event.OnShowSkill, OnShowSkill);
        EventDispatcher.RemoveEventListener(TongUIViewManager.Event.OnShowWar, OnShowWar);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnRefuseApplicant, OnRefuseApplicant);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnAcceptApplicant, OnAcceptApplicant);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnAbdicate, OnAbdicate);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnMemberDemote, OnMemberDemote);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnMemberKick, OnMemberKick);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnMemberPromote, OnMemberPromote);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnSkillUpgrde, OnSkillUpgrde);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnMagicUp, OnMagicUp);
        EventDispatcher.RemoveEventListener<string>(TongUIViewManager.Event.OnCreateTongDone, OnCreateTongDone);
        EventDispatcher.RemoveEventListener(TongUIViewManager.Event.OnTongShow, OnTongShow);
        EventDispatcher.RemoveEventListener<int>(TongUIViewManager.Event.OnSelectMemberTab, OnSelectMemberTab);
    }
}
