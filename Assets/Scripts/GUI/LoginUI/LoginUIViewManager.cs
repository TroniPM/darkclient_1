/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：LoginUIViewManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Game;
using Mogo.Util;

public class LoginUIViewManager : MonoBehaviour
{
    public Action LOGINUILOGINUP;
    public Action LOGINUISIGNUPUP;
    public Action NOTICE_BTN_CLICK;
    public Action ChooseServerUp;
    public Action OnShown;
    public Action OnSwitchAccount;

    private static LoginUIViewManager m_instance;

    public static LoginUIViewManager Instance
    {
        get
        {
            //if (m_instance == null)
            //{
            //    GameObject obj = GameObject.Find("MogoMainUIPanel");

            //    if (obj)
            //    {
            //        m_instance = obj.transform.FindChild("LoginUI").GetComponentsInChildren<LoginUIViewManager>(true)[0];
            //    }
            //}


            return LoginUIViewManager.m_instance;

        }
    }

    private Transform m_myTransform;

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    //public static Dictionary<string, Action> ButtonTypeToEventUp = new Dictionary<string, Action>();

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    GameObject m_chooseCharacterUI;

    UIInput m_userName;
    UIInput m_password;

    UILabel m_lblServerName;

    UITexture m_texLogoFG;
    UITexture m_texLogoBG;

    UITexture m_texLogoFlashObj;

    UISprite m_spResCtrl;
    UIAtlas m_atlasCanRelease;


    //UICheckbox m_cbSaveUserName;
    //UICheckbox m_cbSavePassword;
    //UICheckbox m_cbAutoLogin;

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        m_widgetToFullName.Add(widgetName, fullName);
    }

    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";

        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != m_myTransform)
            {
                fullName = "/" + fullName;
            }

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }

    private void SetUIText(string UIName, string text)
    {
        var l = m_myTransform.Find(UIName).GetComponentsInChildren<UILabel>(true);
        if (l != null)
        {
            l[0].text = text;
            l[0].transform.localScale = new Vector3(18, 18, 18);
        }
    }

    private void SetUITexture(string UIName, string imageName)
    {
        var s = m_myTransform.Find(UIName).GetComponentsInChildren<UISlicedSprite>(true);
        if (s != null)
            s[0].spriteName = imageName;
    }

    void OnLoginUILoginUp()
    {
        if (LOGINUILOGINUP != null)
            LOGINUILOGINUP();
    }

    void OnLoginUISignUpUp()
    {
        if (LOGINUISIGNUPUP != null)
            LOGINUISIGNUPUP();
    }

    public string GetUserName()
    {
        return m_userName.text;
    }

    public string GetPassword()
    {
        return m_password.text;
    }

    public void SetUserName(string name)
    {
        m_userName.text = name;
    }

    public void SetPassword(string password)
    {
        m_password.text = password;
        m_password.label.password = true;
    }

    void OnSwitchUp()
    {
        if (ChooseServerUp != null)
            ChooseServerUp();
        //Mogo.Util.LoggerHelper.Debug("Damn.....................................");
    }

    void Awake()
    {
        m_instance = this;

        Initialize();

        m_myTransform = transform;
        FillFullNameData(m_myTransform);


        m_userName = m_myTransform.Find(m_widgetToFullName["LoginUIUserNameInput"]).GetComponentsInChildren<UIInput>(true)[0];
        m_password = m_myTransform.Find(m_widgetToFullName["LoginUIUserPasswordInput"]).GetComponentsInChildren<UIInput>(true)[0];
        //m_chooseCharacterUI = m_myTransform.parent.FindChild("ChooseCharacterUI").gameObject;

        //m_cbSaveUserName = m_myTransform.FindChild(m_widgetToFullName["LoginUISaveUserName"]).GetComponentsInChildren<UICheckbox>(true)[0];
        //m_cbSavePassword = m_myTransform.FindChild(m_widgetToFullName["LoginUISavePassword"]).GetComponentsInChildren<UICheckbox>(true)[0];
        //m_cbAutoLogin = m_myTransform.FindChild(m_widgetToFullName["LoginUIAutoLogin"]).GetComponentsInChildren<UICheckbox>(true)[0];

        m_lblServerName = m_myTransform.Find(m_widgetToFullName["RecommendServerUIServerName"]).GetComponentsInChildren<UILabel>(true)[0];

        m_myTransform.Find(m_widgetToFullName["RecommendServerUISwitch"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSwitchUp;

        m_texLogoBG = m_myTransform.Find(m_widgetToFullName["LoginUILogoBG"]).GetComponentsInChildren<UITexture>(true)[0];
        m_texLogoFG = m_myTransform.Find(m_widgetToFullName["LoginUILogoFG"]).GetComponentsInChildren<UITexture>(true)[0];
        m_texLogoFlashObj = m_myTransform.Find(m_widgetToFullName["LoginUILogoFlashTex"]).GetComponentsInChildren<UITexture>(true)[0];

        //AssetCacheMgr.GetUIResource("logo.png", (obj) => { m_texLogo.mainTexture = (Texture)obj; });

        m_spResCtrl = m_myTransform.Find(m_widgetToFullName["LoginUIResCtrl"]).GetComponentsInChildren<UISprite>(true)[0];

        m_atlasCanRelease = m_spResCtrl.atlas;

        //gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (OnShown != null)
            OnShown();

        //if (!SystemSwitch.DestroyResource)
        //    return;

        //if (m_texLogoFG.mainTexture == null)
        //{
        //    //AssetCacheMgr.GetUIResource("logo.png", (obj) => { m_texLogo.mainTexture = (Texture)obj; });
        //    AssetCacheMgr.GetUIResource("logofg.png", (obj) =>
        //    {
        //        m_texLogoFG.mainTexture = (Texture)obj;
        //        //m_texLogoFlashObj.material.SetTexture("_MainTex", (Texture)obj);
        //        m_texLogoFG.material.SetFloat("_AlphaFactor", 1);
        //    });
        //    AssetCacheMgr.GetUIResource("logobg.png", (obj) =>
        //    {
        //        m_texLogoBG.mainTexture = (Texture)obj;
        //        m_texLogoBG.material.SetFloat("_AlphaFactor", 1);
        //    });
        //    //AssetCacheMgr.GetUIResource("FlashLight.png", (obj) => { m_texLogo.material.SetTexture("_LightTex", (Texture)obj); });
        //}

        //if (m_atlasCanRelease != null)
        //{
        //    if (m_atlasCanRelease.spriteMaterial.mainTexture == null)
        //    {
        //        AssetCacheMgr.GetUIResource("MogoLoginUI.png", (obj) =>
        //        {

        //            m_atlasCanRelease.spriteMaterial.mainTexture = (Texture)obj;
        //            m_atlasCanRelease.MarkAsDirty();
        //        });
        //    }
        //}
    }

    void Initialize()
    {
        LoginUILogicManager.Instance.Initialize();

        LoginUIDict.ButtonTypeToEventUp.Add("LoginUILogin", OnLoginUILoginUp);
        LoginUIDict.ButtonTypeToEventUp.Add("LoginUISignUp", OnLoginUISignUpUp);
        LoginUIDict.ButtonTypeToEventUp.Add("LoginUINotice", OnLoginUINotice);
    }

    private void OnLoginUINotice()
    {
        if (NOTICE_BTN_CLICK != null)
            NOTICE_BTN_CLICK();
    }

    public void Release()
    {
        LoginUILogicManager.Instance.Release();
        m_myTransform.Find(m_widgetToFullName["RecommendServerUISwitch"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnSwitchUp;

        LoginUIDict.ButtonTypeToEventUp.Clear();
    }

    public bool IsSaveUserName()
    {
        //return m_cbSaveUserName.isChecked;
        return true;
    }

    public bool IsSaveUserPassword()
    {
        //return m_cbSavePassword.isChecked;
        return true;
    }

    public bool IsAutoLogin()
    {
        //return m_cbAutoLogin.isChecked;
        return false;
    }

    public void CheckSaveUserName(bool check)
    {
        //m_cbSaveUserName.isChecked = check;
    }

    public void CheckSavePassword(bool check)
    {
        //m_cbSavePassword.isChecked = check;
    }

    public void CheckAutoLogin(bool check)
    {
        //m_cbAutoLogin.isChecked = check;
    }

    public void SetServerName(string name)
    {
        m_lblServerName.text = name;
    }

    public void SwitchToAndroidMode()
    {
        Mogo.Util.LoggerHelper.Debug("SwitchToAndroidMode");
        m_userName.gameObject.AddComponent<MogoUIListener>().MogoOnClick = OnSwitchAccount;
        m_userName.enabled = false;
    }

    public void ReleaseUIAndResources()
    {
        //if (true)
        //{
            this.gameObject.SetActive(false);
            MogoUIManager.Instance.DestroyLoginUI();
        //}
    }

    void OnDisable()
    {
        //ReleaseUIAndResources();
        //if (!SystemSwitch.DestroyResource)
        //{
        //    return;
        //}

        //if (m_texLogoFG == null)
        //    return;

        //m_texLogoFG.mainTexture = null;
        //AssetCacheMgr.ReleaseResourceImmediate("logofg.png");
        //AssetCacheMgr.ReleaseResourceImmediate("logobg.png");

        //if (m_texLogoBG.material)
        //    m_texLogoBG.material.SetFloat("_AlphaFactor", 0);
        //if (m_texLogoFG.material)
        //    m_texLogoFG.material.SetFloat("_AlphaFactor", 0);
        //AssetCacheMgr.ReleaseResourceImmediate("FlashLight.png");

        
    }
}
