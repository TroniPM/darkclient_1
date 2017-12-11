// 模块名   :  EquipTipManager
// 创建者   :  莫卓豪
// 创建日期 :  2013-8-13
// 描    述 :  android平台sdk登陆对接


using UnityEngine;
using System.Collections;
using Mogo.Util;
public class PlatformSdkManager : MonoBehaviour
{
    virtual public string Uid
    {
        get
        {
            return "";
        }
        set
        {

        }
    }

    virtual public int RecommendedServerID
    {
        get
        {
            return 0;
        }
    }
    virtual public void Log(string msg)
    {

    }

    void Start()
    {
        Instance = this;
    }
    public bool m_isLoginDone = false;
    virtual public bool IsLoginDone
    {
        get
        {
            return false;
        }
        set
        {
            m_isLoginDone = value;
        }
    }
    public static PlatformSdkManager Instance;
    public System.Action LoginCallBack = null;
#if UNITY_IPHONE
    public const string LOGIN_URL = "http://{0}/cgi-bin/login_ios/{1}?suid={2}&timestamp={3}&plataccount={4}&sign={5}&tocken={6}&port={7}";
#else
    public const string LOGIN_URL = "http://{0}/cgi-bin/login/{1}?suid={2}&timestamp={3}&plataccount={4}&sign={5}&tocken={6}&port={7}";
#endif
    string GetSdkManagerName()
    {
        string name = string.Empty;
#if UNITY_ANDROID
        name = "AndroidSdkManager";
#elif UNITY_IPHONE
        name = "IOSPlatformSDKManager";
#endif
        return name;
    }
    virtual public void Shake(long milliseconds)
    {

    }
    virtual public void StopShake()
    {

    }
    virtual public void RestartGame()
    {
    }

    virtual public void Login()
    {
    }


    virtual public void Charge(int amount, string productName = "", string roleName = "", string level = "", string tongName = "")
    {
    }

    virtual public void GotoGameCenter()
    {
    }

    virtual public void CheckUpdate()
    {
    }

    virtual public void CleanLocalData()
    {
    }

    virtual public void SendLoginLog()
    {
    }

    virtual public void SendStartGameLog()
    {
    }

    virtual public void SendBeforeLoginLog()
    {
    }

    virtual public void OpenForum()
    {
    }

    virtual public void OnSwitchAccount()
    {

    }

    virtual public void SetNetwork()
    {

    }

    virtual public void ShowAssistant()
    {
    }

    virtual public void AddNotificationRecord(int tag)
    {

    }

    virtual public void SetupNotificationData()
    {

    }
    virtual public void OnSetupNotification()
    {
    }

    virtual public void CreateRoleLog(string roleName, string serverId)
    {

    }

    virtual public void RoleLevelLog(string roleName, string serverId)
    {

    }

    virtual public void EnterGameLog(string serverId)
    {
        
    }

    virtual public void SetupInfo()
    {
    }
}


