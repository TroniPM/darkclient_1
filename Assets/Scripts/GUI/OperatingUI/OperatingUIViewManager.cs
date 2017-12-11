using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public enum OperatingUITab
{  
    NoneTab = -1,
    ChargeRewardTab= 0,
    TimeLimitActivityTab = 1,
    LoginRewardTab = 2,
    AttributeRewardTab = 3,
}

public class OperatingUIViewManager : MogoUIBehaviour 
{
    private static OperatingUIViewManager m_instance;
    public static OperatingUIViewManager Instance { get { return OperatingUIViewManager.m_instance; } }

    private GameObject m_goIconList;
    private GameObject m_goAttributeRewardBtn;
    private GameObject m_goLoginRewardBtn;
    private GameObject m_goTimeLimitActivityBtn;
    private GameObject m_goChargeRewardBtn;

    private GameObject m_goGOAttributeRewardBtnTip;
    private GameObject m_goGOLoginRewardBtnTip;
    private GameObject m_goGOTimeLimitActivityBtnTip;

    private UISprite m_spOperatingUIRefreshCtrl; // ����ˢ��MogoUIͼ��
    private UISprite m_spResCtrl;
    private UIAtlas m_atlsCanRelease;

    private Dictionary<int, UILabel> m_tabLabelList = new Dictionary<int, UILabel>();  
 
    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goIconList = m_myTransform.Find(m_widgetToFullName["OperatingUIIconList"]).gameObject;
        Initialize();

        m_goAttributeRewardBtn = m_myTransform.Find(m_widgetToFullName["AttributeRewardBtn"]).gameObject;
        m_goLoginRewardBtn = m_myTransform.Find(m_widgetToFullName["LoginRewardBtn"]).gameObject;
        m_goTimeLimitActivityBtn = m_myTransform.Find(m_widgetToFullName["TimeLimitActivityBtn"]).gameObject;
        m_goChargeRewardBtn = m_myTransform.Find(m_widgetToFullName["ChargeRewardBtn"]).gameObject;

        m_goGOAttributeRewardBtnTip = m_myTransform.Find(m_widgetToFullName["GOAttributeRewardBtnTip"]).gameObject;
        m_goGOLoginRewardBtnTip = m_myTransform.Find(m_widgetToFullName["GOLoginRewardBtnTip"]).gameObject;
        m_goGOTimeLimitActivityBtnTip = FindTransform("GOTimeLimitActivityBtnTip").gameObject;

        m_tabLabelList[0] = m_myTransform.Find(m_widgetToFullName["ChargeRewardBtnText"]).GetComponent<UILabel>();
        m_tabLabelList[1] = m_myTransform.Find(m_widgetToFullName["TimeLimitActivityBtnText"]).GetComponent<UILabel>();
        m_tabLabelList[2] = m_myTransform.Find(m_widgetToFullName["LoginRewardBtnText"]).GetComponent<UILabel>();
        m_tabLabelList[3] = m_myTransform.Find(m_widgetToFullName["AttributeRewardBtnText"]).GetComponent<UILabel>();

        m_spOperatingUIRefreshCtrl = FindTransform("OperatingUIRefreshCtrl").GetComponentsInChildren<UISprite>(true)[0];
        m_spResCtrl = m_myTransform.Find(m_widgetToFullName["OperatingUIResCtrl"]).GetComponentsInChildren<UISprite>(true)[0];
        m_atlsCanRelease = m_spResCtrl.atlas;

        if (MogoWorld.thePlayer.IsLoginFirstShow && !MogoWorld.thePlayer.IsLoginRewardHasGot)
        {
            ShowChargeRewardBtn(false);
            ShowLoginRewardBtn(false);
            ShowTimeLimitActivityBtn(false);
            ShowAttributeRewardBtn(false);
            MogoWorld.thePlayer.IsLoginFirstShow = false;
        }
        else
        {
            ShowChargeRewardBtn(true);
            ShowLoginRewardBtn(true);
            ShowAttributeRewardBtn(MogoWorld.thePlayer.IsAchiementHasOpen);
            ShowTimeLimitActivityBtn(MogoWorld.thePlayer.IsTimeLimitEventHasOpen);
        }

        // EventDispatcher.TriggerEvent(Events.OperationEvent.GetChargeRewardMessage);
        EventDispatcher.TriggerEvent("OperatingUIRefreshTip");
    }

    #region �¼�

    public static Dictionary<string, Action<int>> ButtonTypeToEventUp = new Dictionary<string, Action<int>>();

    void Initialize()
    {
        OperatingUILogicManager.Instance.Initialize();

        m_myTransform.Find(m_widgetToFullName["AttributeRewardBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnAttributeRewardBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChargeRewardBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnChargeRewardBtnUp;
        m_myTransform.Find(m_widgetToFullName["LoginRewardBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnLoginRewardBtnUp;
        m_myTransform.Find(m_widgetToFullName["TimeLimitActivityBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnTimeLimitActivityBtnUp;
        m_myTransform.Find(m_widgetToFullName["OperatingUICloseBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCloseBtnUp;
    }

    public void Release()
    {
        ButtonTypeToEventUp.Clear();

        OperatingUILogicManager.Instance.Release();

        m_myTransform.Find(m_widgetToFullName["AttributeRewardBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnAttributeRewardBtnUp;
        m_myTransform.Find(m_widgetToFullName["ChargeRewardBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnChargeRewardBtnUp;
        m_myTransform.Find(m_widgetToFullName["LoginRewardBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnLoginRewardBtnUp;
        m_myTransform.Find(m_widgetToFullName["TimeLimitActivityBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnTimeLimitActivityBtnUp;
        m_myTransform.Find(m_widgetToFullName["OperatingUICloseBtn"]).GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCloseBtnUp;
    }

    #endregion

    public void ShowAttributeRewardBtn(bool isShow)
    {
        m_goAttributeRewardBtn.SetActive(isShow);
    }

    public void ShowChargeRewardBtn(bool isShow)
    {
        m_goChargeRewardBtn.SetActive(isShow);
    }

    public void ShowTimeLimitActivityBtn(bool isShow)
    {
        m_goTimeLimitActivityBtn.SetActive(isShow);
    }

    public void ShowLoginRewardBtn(bool isShow)
    {
        m_goLoginRewardBtn.SetActive(isShow);
    }

    public  void ShowOperatingUIIconList(bool isShow) 
    {
        m_goIconList.SetActive(isShow);
    }

    public void HandleTabChange(int fromTab, int toTab)
    {
        if (fromTab == (int)OperatingUITab.NoneTab)
        {
            List<int> keyList = new List<int>();
            keyList.Clear();
            foreach (int key in m_tabLabelList.Keys)
            {
                keyList.Add(key);
            }

            for (int i = 0; i < keyList.Count; i++)
            {
                if (m_tabLabelList.ContainsKey(keyList[i]))
                {
                    UILabel fromTabLabel = m_tabLabelList[keyList[i]];
                    fromTabLabel.color = new Color32(37, 29, 6, 255);
                    fromTabLabel.effectStyle = UILabel.Effect.None;
                }
            }
        }
        else
        {
            if (m_tabLabelList.ContainsKey(fromTab))
            {
                UILabel fromTabLabel = m_tabLabelList[fromTab];
                fromTabLabel.color = new Color32(37, 29, 6, 255);
                fromTabLabel.effectStyle = UILabel.Effect.None;
            }
        }       

        if(m_tabLabelList.ContainsKey(toTab))
        {
            UILabel toTabLabel = m_tabLabelList[toTab];
            toTabLabel.color = new Color32(255, 255, 255, 255);
            toTabLabel.effectStyle = UILabel.Effect.Outline;
            toTabLabel.effectColor = new Color32(53, 22, 2, 255);
        }
    }

    void OnAttributeRewardBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("AttributeReward");
        MogoUIManager.Instance.SwitchAttributeRewardUI();
    }

    void OnChargeRewardBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("ChargeReward");
        MogoUIManager.Instance.SwitchChargeRewardUI();
    }

    void OnLoginRewardBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("LoginReward");
        MogoUIManager.Instance.SwitchLoginRewardUI();
    }

    void OnTimeLimitActivityBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("ActivityReward");
        MogoUIManager.Instance.SwitchTimeLimitActivityUI();
    }

    void OnCloseBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("Close");
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    public void ShowAttributeRewardBtnTip(bool isShow = false)
    {
        if (m_goGOAttributeRewardBtnTip != null)
            m_goGOAttributeRewardBtnTip.SetActive(isShow);
    }

    public void ShowLoginRewardBtnTip(bool isShow = false)
    {
        if (m_goGOLoginRewardBtnTip != null)
            m_goGOLoginRewardBtnTip.SetActive(isShow);
    }

    public void ShowTimeLimitActivityBtnTip(bool isShow = false)
    {
        if (m_goGOTimeLimitActivityBtnTip != null)
            m_goGOTimeLimitActivityBtnTip.SetActive(isShow);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (m_spOperatingUIRefreshCtrl != null)
            m_spOperatingUIRefreshCtrl.RefreshAllShowAs();

        //return;
        if (!SystemSwitch.DestroyResource)
        {
            return;
        }

        //if (m_atlsCanRelease != null)
        //{
        //    if (m_atlsCanRelease.spriteMaterial.mainTexture == null)
        //    {
        //        AssetCacheMgr.GetUIResource("MogoOperatingUI.png", (obj) =>
        //        {
        //            m_atlsCanRelease.spriteMaterial.mainTexture = (Texture)obj;
        //            m_atlsCanRelease.MarkAsDirty();
        //        });
        //    }
        //}
    }

    void OnDisable()
    {
        //return;
        if (!SystemSwitch.DestroyResource)
        {
            return;
        }

        //m_atlsCanRelease.spriteMaterial.mainTexture = null;
        //AssetCacheMgr.ReleaseResourceImmediate("MogoOperatingUI.png");
    }
}
