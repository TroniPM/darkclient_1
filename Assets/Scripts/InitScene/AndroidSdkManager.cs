// ģ����   :  AndroidSdkManager
// ������   :  Ī׿��
// �������� :  2013-8-13
// ��    �� :  androidƽ̨sdk��½�Խ�

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;
using Mogo.GameData;
#if UNITY_ANDROID
public class AndroidSdkManager : PlatformSdkManager
{
    private AndroidJavaClass m_config;
    private AndroidJavaObject m_mainActivity;

    virtual public int RecommendedServerID
    {
        get
        {
            return int.Parse(m_config.GetStatic<string>("targetServerId"));
        }
    }


    public override string Uid
    {
        get
        {
            return m_config.GetStatic<string>("suid");
        }
        set
        {
            m_config.SetStatic<string>("suid", value);
        }
    }
    public override bool IsLoginDone
    {
        get
        {
            return m_mainActivity.Call<bool>("isLoginned");
            //m_config.GetStatic<bool>("isLoginDone");
        }
        //set
        //{
        //    m_config.SetStatic<bool>("isLoginDone", value);
        //}
    }

    void Start()
    {
        Instance = this;
        m_config = new AndroidJavaClass("com.ahzs.utils.Config");
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        m_mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");

        //SendStartGameLog();
        m_mainActivity.Call("init", name);

    }

    override public void Login()
    {
        //AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //jo.Call("login");
        MogoGlobleUIManager.Instance.ShowGlobleStaticText(true, "Login...");
        SendBeforeLoginLog();
        //MogoMsgBox.Instance.ShowFloatingTextQueue("ShowLoginUI");
        m_mainActivity.Call("login");
    }

    override public void Charge(int amount,string productName ="",string roleName = "",string leve= "",string tongName = "")
    {
        //MogoMsgBox.Instance.ShowFloatingText("amount:" + amount);
        if (MogoWorld.thePlayer == null) return;
        //AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject m_mainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //m_mainActivity.Call("charge", "1", 2, "123");
        //MogoMsgBox.Instance.ShowFloatingText("SelectedServer:" + SystemConfig.Instance.SelectedServer.ToString() + ",dbid:" + MogoWorld.thePlayer.dbid.ToString());
        m_mainActivity.Call("charge", SystemConfig.Instance.SelectedServer.ToString(), amount, MogoWorld.thePlayer.dbid.ToString(),productName,roleName,
            leve,tongName);
    }

    override public void GotoGameCenter()
    {
        m_mainActivity.Call("gotoGameCenter");
    }

    override public void CheckUpdate()
    {
        m_mainActivity.Call("updateVersion");
    }

    override public void CleanLocalData()
    {
        m_mainActivity.Call("clean");
    }

    public override void RoleLevelLog(string roleName, string serverId)
    {
        m_mainActivity.Call("roleLevelLog", roleName, serverId);
    }

    public override void CreateRoleLog(string roleName, string serverId)
    {
        m_mainActivity.Call("createRoleLog", roleName, serverId);
    }

    public override void EnterGameLog(string serverId)
    {
        m_mainActivity.Call("enterGameLog",serverId);
    }

    override public void SendLoginLog()
    {
        m_mainActivity.Call("sendLoginLog", SystemConfig.Instance.SelectedServer.ToString());
    }

    override public void SendStartGameLog()
    {
        m_mainActivity.Call("activityOpenLog");
    }

    override public void SendBeforeLoginLog()
    {
        m_mainActivity.Call("activityBeforeLoginLog");
    }

    override public void OpenForum()
    {
        m_mainActivity.Call("loginForm");
    }

    override public void OnSwitchAccount()
    {
        MogoMsgBox.Instance.ShowFloatingText("switchAccount");
        //CleanLocalData();
        //Login();
        m_mainActivity.Call("switchAccount");
    }

    public override void RestartGame()
    {
        m_mainActivity.Call("restartGame");
    }

    public override void Shake(long milliseconds)
    {
        m_mainActivity.Call("Shake", milliseconds);
    }
    public override void StopShake()
    {
        m_mainActivity.Call("StopShake");
    }

    public override void SetNetwork()
    {
        m_mainActivity.Call("gotoNetworkSetting");
    }

    public override void Log(string msg)
    {
        m_mainActivity.Call("showLog", msg);
    }

    #region ��½�ص�
    public void OnCancel()
    {
        //MogoMsgBox.Instance.ShowFloatingTextQueue("OnCancel");
        MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "Login...");
    }

    public void OnError()
    {
        MogoMsgBox.Instance.ShowFloatingTextQueue("OnError");
        MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "Login...");
    }

    public string PackageName
    {
        get
        {
            return m_mainActivity.Call<string>("getPackageName");
        }

    }
    public string PlatformName
    {
        get
        {
            return SystemConfig.PlatformDic[PackageName];
        }
    }

    public override void SetupInfo()
    {
        if (PlatformName == SystemConfig.PLATFORM_SSJJ)
        {
            SetupInfoSSJJ();
        }
        else if (PlatformName == SystemConfig.PLATFORM_UC)
        {
            SetupInfoUc();
        }
        else
        {
            SetupInfoUnion();
        }
    }

    private void SetupInfoUnion()
    {
        string username = m_config.GetStatic<string>("username");
        // ǩ���ַ�����
        string sessionId = m_config.GetStatic<string>("sessionId");
        // ��Ϸ�н�Ҫʹ�õ���Ψһid�����id��ȡ�� username
        string suid = m_config.GetStatic<string>("suid");

        MogoWorld.loginInfo = new MogoWorld.LoginInfo() { platName = PlatformName, uid = suid, strPlatAccount = username, strPlatId = "1", strSign = sessionId, timestamp = "", token = sessionId };

        LoginUILogicManager.Instance.UserName = username;
        SystemConfig.Instance.Passport = username;
        SystemConfig.SaveConfig();
    }

    private void SetupInfoUc()
    {

        string sessionId = m_config.GetStatic<string>("sessionId");

        MogoWorld.loginInfo = new MogoWorld.LoginInfo() { strPlatId = "1", platName = PlatformName, token = sessionId };
    }

    private void SetupInfoSSJJ()
    {
        string username = m_config.GetStatic<string>("username");
        // ��Ȩʱ������ʱ�����
        string timestamp = m_config.GetStatic<string>("timestamp");
        // ǩ���ַ�����
        string signStr = m_config.GetStatic<string>("signStr");
        // ��Ϸ�н�Ҫʹ�õ���Ψһid�����id��ȡ�� username
        string suid = m_config.GetStatic<string>("suid");
        // ��������id
        string targetServerId = m_config.GetStatic<string>("targetServerId");
        // comeFrom "2"��ʾ�����δ���������룬��1����ʾ����������ʽ������Ϸ��
        string comeFrom = m_config.GetStatic<string>("comeFrom");
        string token = m_config.GetStatic<string>("token");

        MogoWorld.loginInfo = new MogoWorld.LoginInfo() { platName = PlatformName, uid = suid, strPlatAccount = username, strPlatId = "1", strSign = signStr, timestamp = timestamp, token = token };

        LoginUILogicManager.Instance.UserName = username;
        SystemConfig.Instance.Passport = username;
        SystemConfig.SaveConfig();
    }

    public void OnComplete()
    {
        MogoMsgBox.Instance.ShowFloatingText("OnLoginComplete:" + PlatformName);
        if (PlatformName == SystemConfig.PLATFORM_SSJJ)
        {
            OnLoginDoneSSJJ();
        }
        else if (PlatformName == SystemConfig.PLATFORM_UC)
        {
            OnLoginDoneUc();
        }
        else
        {
            OnLoginDoneUnion();
        }
    }

    private void OnLoginDoneUc()
    {
        SetupInfoUc();
    
        MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "Login...");
        //IsLoginDone = true;
        MogoWorld.Login();
        //IsLoginDone = false;
    }

    private void OnLoginDoneUnion()
    {
        SetupInfoUnion();
        MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "Login...");
        //IsLoginDone = true;
        //MogoMsgBox.Instance.ShowFloatingText("username:" + username + ",sessionId" + sessionId + ",suid:" + suid);
        MogoWorld.Login();
        //IsLoginDone = false;
    }

    private void OnLoginDoneSSJJ()
    {
        SetupInfoSSJJ();
        MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "Login...");
        //IsLoginDone = true;
        MogoWorld.Login();
        //IsLoginDone = false;
    }

    public void OnException()
    {
        //MogoMsgBox.Instance.ShowFloatingTextQueue("OnException");
        MogoGlobleUIManager.Instance.ShowGlobleStaticText(false, "Login...");
    }

    #endregion

    #region ϵͳ֪ͨ
    public override void SetupNotificationData()
    {
        string time = string.Empty;
        string type = string.Empty;
        string title = string.Empty;
        string titleBanner = string.Empty;
        string content = string.Empty;
        string tag = string.Empty;
        foreach (MogoNotificationData notification in MogoNotificationData.dataMap.Values)
        {
            if (title == string.Empty)
            {
                time = notification.time;
                type = notification.type + "";
                titleBanner = notification.TitleBanner;
                title = notification.Title;
                content = notification.Content;
                tag = notification.tag + "";
            }
            else
            {
                time += "_" + notification.time;
                type += "_" + notification.type;
                titleBanner += "_" + notification.TitleBanner;
                title += "_" + notification.Title;
                content += "_" + notification.Content;
                tag += "_" + notification.tag;
            }
        }
        m_mainActivity.Call("setupDailyNotification", time,
             type, tag, title,
             titleBanner, content);
    }

    override public void AddNotificationRecord(int tag)
    {
        m_mainActivity.Call("addRecord", tag);
    }

    /// <summary>
    /// ����һ���´�����������ʾ֪ͨ
    /// </summary>
    override public void OnSetupNotification()
    {
        if (MogoWorld.thePlayer == null) return;
        int interval = EnergyData.dataMap[1].recoverInterval;
        uint maxEnergy = MogoWorld.thePlayer.maxEnergy;
        uint energy = MogoWorld.thePlayer.energy;

        long needTime = (maxEnergy - energy) * interval * 60 * 1000;
        m_mainActivity.Call("addNotification", needTime);
    }
    #endregion

    public override void ShowAssistant()
    {
        m_mainActivity.Call("showAssistant", SystemConfig.Instance.SelectedServer.ToString());
    }

    #region Android�������ڻص��¼�
    public void OnStart()
    {
        LoggerHelper.Debug("OnStart");
    }

    public void OnPause()
    {
        LoggerHelper.Debug("OnPause");
    }

    public void OnStop()
    {
        LoggerHelper.Debug("OnStop");
    }

    public void OnRestart()
    {
        LoggerHelper.Debug("OnRestart");
    }

    public void OnDestory()
    {
        LoggerHelper.Debug("OnDestory");
    }

    public void OnResume()
    {
        LoggerHelper.Debug("OnResume");
    }

    #endregion
}

#endif


