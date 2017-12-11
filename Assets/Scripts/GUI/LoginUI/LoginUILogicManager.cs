/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：LoginUILogicManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Game;
using Mogo.RPC;
using Mogo.Util;
using System.Collections.Generic;

public class LoginUILogicManager
{
    public bool IsSaveUserName
    {
        get
        {
            return LoginUIViewManager.Instance.IsSaveUserName();
        }
        set
        {
            if (true)
            {
                LoginUIViewManager.Instance.CheckSaveUserName(value);
            }
        }
    }

    public bool IsSavePassword
    {
        get
        {
            return LoginUIViewManager.Instance.IsSaveUserPassword();
        }
        set
        {
            LoginUIViewManager.Instance.CheckSavePassword(value);
        }
    }

    public bool IsAutoLogin
    {
        get
        {
            return LoginUIViewManager.Instance.IsAutoLogin();
        }
        set
        {
            if (true)
            {
                LoginUIViewManager.Instance.CheckAutoLogin(value);
            }
        }
    }

    public string UserName
    {
        get
        {
            return LoginUIViewManager.Instance.GetUserName();
        }
        set
        {
            LoginUIViewManager.Instance.SetUserName(value);
        }
    }

    public string Password
    {
        get
        {
            return LoginUIViewManager.Instance.GetPassword();
        }
        set
        {
            LoginUIViewManager.Instance.SetPassword(value);
        }
    }

    private static LoginUILogicManager m_instance;

    public static LoginUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new LoginUILogicManager();
            }

            return LoginUILogicManager.m_instance;

        }
    }

    void OnLoginUILoginUp()
    {
        //ServerProxy.Instance.Login(LoginUIViewManager.Instance.GetUserName(),
        //    LoginUIViewManager.Instance.GetPassword(), "20");

        string passport = LoginUIViewManager.Instance.GetUserName();
        string password = LoginUIViewManager.Instance.GetPassword();
        MogoWorld.passport = passport;
        MogoWorld.password = password;

        MogoWorld.Login();

        //SystemConfig.Instance.IsAutoLogin = LoginUILogicManager.Instance.IsAutoLogin;
        //SystemConfig.Instance.IsSavePassport = LoginUILogicManager.Instance.IsSaveUserName;
        //if (SystemConfig.Instance.IsSavePassport)
        //{
        SystemConfig.Instance.Passport = passport;
        //if (SystemConfig.Instance.IsAutoLogin)
        SystemConfig.Instance.Password = password;
        //else
        //    SystemConfig.Instance.Password = " ";
        //}
        //else
        //{
        //    SystemConfig.Instance.Passport = " ";
        //    SystemConfig.Instance.Password = " ";
        //}

        SystemConfig.SaveConfig();
        //MogoUIManager.Instance.ShowMogoChooseCharacterUI();

        //EventDispatcher.TriggerEvent<string, string>(Events.UIAccountEvent.OnLogin, passport, password);

        LoggerHelper.Debug("LoginUp");

    }

    void OnChooseServerUp()
    {
        MogoUIManager.Instance.ShowNewLoginUI(() =>
        {
            NewLoginUILogicManager.Instance.ShowChooseServerUIFormLogin();
        });
    }

    void OnLoginUISignUpUp()
    {

    }

    //void OnS2CLogin(LoginResult result)
    //{
    //    switch (result)
    //    {
    //        case LoginResult.SUCCESS:
    //            LoggerHelper.Debug("Success");
    //            LoginUIViewManager.Instance.ShowChooseCharacterUI();
    //            break;

    //        case LoginResult.RET_ACCOUNT_PASSWD_NOMATCH:
    //            LoggerHelper.Debug("NotMatch");
    //            break;

    //        case LoginResult.NO_SERVICE:
    //            LoggerHelper.Debug("No Service");
    //            break;
    //    }
    //}

    public void Initialize()
    {
        LoginUIViewManager.Instance.LOGINUILOGINUP += OnLoginUILoginUp;
        LoginUIViewManager.Instance.LOGINUISIGNUPUP += OnLoginUISignUpUp;
        LoginUIViewManager.Instance.NOTICE_BTN_CLICK += OnNoticeBtbClick;
        LoginUIViewManager.Instance.ChooseServerUp += OnChooseServerUp;
        LoginUIViewManager.Instance.OnShown = () =>
        {
            var serverInfo = SystemConfig.GetSelectedServerInfo();
            if (serverInfo != null)
                LoginUIViewManager.Instance.SetServerName(serverInfo.name);
        };

        //ServerProxy.Instance.LoginResp += OnS2CLogin;

    }

    private void OnNoticeBtbClick()
    {
        //公告板下载与显示
        //Driver.Instance.StartCoroutine(NoticeManager.ShowNotice(Driver.Instance));
        NoticeManager.Instance.ShowNotice();
    }

    public void Release()
    {
        LoginUIViewManager.Instance.LOGINUILOGINUP -= OnLoginUILoginUp;
        LoginUIViewManager.Instance.LOGINUISIGNUPUP -= OnLoginUISignUpUp;
        LoginUIViewManager.Instance.NOTICE_BTN_CLICK -= OnNoticeBtbClick;
    }

}
