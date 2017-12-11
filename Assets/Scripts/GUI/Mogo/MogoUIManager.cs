using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;
using System;

public class MogoUIManager : MonoBehaviour
{
    #region ���ƿ���

    public readonly static bool IsShowLevel = false; // ���ͷ���������Ƿ���ʾ�ȼ�
    public readonly static bool ISTONGUIOPENED = false; // �Ƿ������

    #endregion

    private static MogoUIManager m_instance;

    public static MogoUIManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = GameObject.Find("MogoMainUIPanel");
                if (obj)
                {
                    m_instance = obj.GetComponentsInChildren<MogoUIManager>(true)[0];
                }
                m_instance.RegisterEnvents();
            }
            
            return MogoUIManager.m_instance;
        }
    }  

    private Transform m_myTransform;

    private Camera m_camUI;

    public string WaitingWidgetName;

    public bool UICanChange = true;

    //public static bool DestroyResource = false;

    public GameObject m_MainUI;
    public GameObject m_MenuUI;
    GameObject m_TaskUI;
    public GameObject m_RuneUI;
    GameObject m_ChatUI;
    GameObject m_LoginUI;
    GameObject m_TongUI;
    public GameObject m_DragonUI;

    public GameObject m_InstanceUI { get; protected set; }
    public GameObject m_InstanceLevelChooseUI { get; protected set; }
    GameObject m_InstanceHelpChooseUI;
    GameObject m_InstancePassUI;
    public GameObject m_InstancePassRewardUI;
    GameObject m_InstanceTreasureChestUI;
    GameObject m_InstanceCleanUI;
    GameObject m_InstanceTaskRewardUI;
    GameObject m_goInstanceRebornUI;
    GameObject m_goInstanceRebornNoneDropUI;

    public GameObject m_CommunityUI;
    GameObject m_EquipmentUI;
    GameObject m_BattleMenuUI;
    public GameObject m_NormalMainUI;
    GameObject m_ChooseServerUI;
    GameObject m_CreateCharacterUI;
    GameObject m_ChooseCharacterUI;
    GameObject m_AssistantUI;
    GameObject m_MarketUI;
    public GameObject m_ChallengeUI { get; protected set; }
    GameObject m_DoorOfBuryUI;
    public GameObject m_NewInstanceChooseLevelUI { get; protected set; }
    public GameObject m_InstanceMissionChooseUI { get; protected set; } // ���϶�����ѡ�����
    public GameObject m_SkillUI;
    GameObject m_SettingUI;
    GameObject m_SocietyUI;
    GameObject m_InsetUI;
    public GameObject m_DecomposeUI;
    public GameObject m_ComposeUI;
    public GameObject m_StrenthUI;
    public GameObject m_NewLoginUI;
    GameObject m_VIPInfoUI;
    public GameObject m_EnergyUI;
    public GameObject m_DiamondToGoldUI;
    public GameObject m_OperatingUI { get; protected set; }
    GameObject m_ChargeRewardUI;
    public GameObject m_TimeLimitActivityUI { get; protected set; }
    public GameObject m_NewTimeLimitActivityUI { get; protected set; }
    GameObject m_LoginRewardUI;
    GameObject m_AttributeRewardUI;
    public GameObject m_NewAttributeRewardUI;
    GameObject m_SanctuaryUI;
    GameObject m_DebugUI;
    public GameObject m_RankingUI; // ���а����
    public GameObject m_RankingRewardUI; // ���а���
    GameObject m_UpgradePowerUI; // ս������ָ������
    GameObject m_EquipRecommendUI; // װ���Ƽ�����
    GameObject m_InstanceBossTreasureUI; // ����BOSS�������
    public GameObject m_LevelNoEnoughUI; // �ȼ�������������
    public GameObject m_EnergyNoEnoughUI; // ����������������
    public GameObject m_DragonMatchUI; // ������������
    public GameObject m_DragonMatchRecordUI; // ����������¼����
    public GameObject m_ChooseDragonUI; // ѡ���������
    public GameObject m_OKCancelTipUI; // ȷ�Ͽ�-����ѡ������ʾ  
    public GameObject m_SpriteUI; // ����ϵͳ
    public GameObject m_OccupyTowerPassUI; // ռ���������
    public GameObject m_OccupyTowerUI; // ռ������
    public GameObject m_NewArenaUI; // �¾���������

    /// <summary>
    /// ��Ļ��ͷ
    /// </summary>
    private GameObject m_subtitleUI;
    private UILabel m_subtitleText;
    private int m_subtitleClip;
    private uint m_intv;
    private String m_subtitleString;   

    /// <summary>
    /// ��ǰUI
    /// </summary>
    private GameObject m_CurrentUI;
    public GameObject CurrentUI
    {
        get { return m_CurrentUI; }
        set
        {
            m_CurrentUI = value;
            EventDispatcher.TriggerEvent<GameObject>("CurrentUIChange", m_CurrentUI);
        }
    }

    void RegisterEnvents()
    {
        EventDispatcher.AddEventListener<GameObject>(Events.MogoUIManagerEvent.SetCurrentUI,SetCurrentUI);
        EventDispatcher.AddEventListener<bool>(Events.MogoUIManagerEvent.ShowInstanceMissionChooseUI, ShowInstanceMissionChooseUI);
        EventDispatcher.AddEventListener(Events.MogoUIManagerEvent.SwitchStrenthUI, SwitchStrenthUI);
        EventDispatcher.AddEventListener<System.Action>(Events.MogoUIManagerEvent.ShowDiamondToGoldUI, ShowDiamondToGoldUI);
        EventDispatcher.AddEventListener<System.Action>(Events.MogoUIManagerEvent.ShowEnergyUI, ShowEnergyUI);
        EventDispatcher.AddEventListener<int>(Events.MogoUIManagerEvent.SwitchToMarket, SwitchToMarketAdapter);
    }

    void SwitchToMarketAdapter(int nDst)
    {
        SwitchToMarket((MarketUITab)nDst);
    }
    public void SetCurrentUI(GameObject currentUI)
    {
        CurrentUI = currentUI;
    }
    /// <summary>
    /// �Ƿ���ʾ��ǰUI
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowCurrentUI(bool isShow)
    {
        if (!isShow)
        {
            if (CurrentUI == m_NormalMainUI && m_CommunityUI != null)
                m_CommunityUI.SetActive(false);
        }

        CurrentUI.SetActive(isShow);        
    }

    bool m_bEnableControlStick = true;

    public bool IsBattleMainUILoaded = false;

    private void TruelyShowMogoBattleMainUI()
    {
        if (!UICanChange)
            return;

        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattleMainUI);
        MainUIViewManager.Instance.SetUIDirty();

        if (m_MainUI == null)
        {
            if (!IsBattleMainUILoaded)
            {
                IsBattleMainUILoaded = true;

                CallWhenUILoad();
                AssetCacheMgr.GetUIInstance("MainUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_MainUI = go as GameObject;
                        m_MainUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_MainUI.transform.localPosition = Vector3.zero;
                        m_MainUI.transform.localScale = new Vector3(1, 1, 1);
                        m_MainUI.AddComponent<MainUIViewManager>();

                        if (CurrentUI != m_MainUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_MainUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            //if (MogoMainCamera.instance)
                            //    MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(true);


                            m_MainUI.GetComponentsInChildren<MainUIViewManager>(true)[0].SetControllStickEnable(m_bEnableControlStick);

                        }
                    }
                );
            }
        }
        else
        {
            if (CurrentUI != m_MainUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_MainUI;
                ShowCurrentUI(true);

                m_camUI.clearFlags = CameraClearFlags.Depth;
                ShowBillboardList(true);

                //if (MogoMainCamera.instance)
                //    MogoMainCamera.instance.SetActive(false);


                m_MainUI.GetComponentsInChildren<MainUIViewManager>(true)[0].SetControllStickEnable(m_bEnableControlStick);
            }
        }
    }

    public void ShowDebugUI(bool isShow)
    {
        if (m_DebugUI != null)
            m_DebugUI.SetActive(isShow);
    }

    public void ShowMogoBattleMainUI()
    {
        //m_CurrentUI.SetActive(false);
        //m_CurrentUI = m_MainUI;
        //m_CurrentUI.SetActive(true);

        //if (MogoMainCamera.instance)
        //    MogoMainCamera.instance.SetActive(true);
        if (!UICanChange)
            return;

        //if (m_CurrentUI == m_MainUI)
        //    return;

        MogoUIQueue.Instance.PushOne(() => { TruelyShowMogoBattleMainUI(); }, null, "ShowMogoBattleMainUI");
    }

    public bool IsMenuUILoaded = false;

    private void TruelyShowMogoMenuUI(Action callback)
    {
        if (!UICanChange)
            return;
        if (m_MenuUI == null)
        {
            if (!IsMenuUILoaded)
            {
                IsMenuUILoaded = true;

                CallWhenUILoad(0, false);

                PreLoadResource("PackageItemGrid.prefab", (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("MenuUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_MenuUI = go as GameObject;
                            m_MenuUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                            m_MenuUI.transform.localPosition = Vector3.zero;
                            m_MenuUI.transform.localScale = new Vector3(1, 1, 1);
                            m_MenuUI.AddComponent<MenuUIViewManager>();

                            if (CurrentUI == m_NormalMainUI)
                            {
                                MenuUIViewManager.Instance.IsNormalMainUI = true;
                            }
                            else if (CurrentUI == m_MainUI)
                            {
                                MenuUIViewManager.Instance.IsNormalMainUI = false;
                                LoggerHelper.Debug("Here");
                            }
                            else
                            {
                                MenuUIViewManager.Instance.IsNormalMainUI = true;
                            }

                            if (callback != null)
                            {
                                callback();
                            }

                            if (CurrentUI != m_MenuUI)
                            {
                                MenuUILogicManager.Instance.UpdateRole(MogoWorld.thePlayer);

                                ShowCurrentUI(false);
                                CurrentUI = m_MenuUI;
                                ShowCurrentUI(true);

                                if (MogoMainCamera.instance)
                                    MogoMainCamera.instance.SetActive(false);
                                m_camUI.clearFlags = CameraClearFlags.SolidColor;

                                ShowBillboardList(false);
                                ShowSocialTip();

                                if (MogoUIManager.Instance.WaitingWidgetName == "PackageIcon" ||
                                MogoUIManager.Instance.WaitingWidgetName == "PlayerIcon" ||
                                MogoUIManager.Instance.WaitingWidgetName == "SettingsIcon" ||
                                MogoUIManager.Instance.WaitingWidgetName == "SkillIcon" ||
                                MogoUIManager.Instance.WaitingWidgetName == "SocialIcon" ||
                                MogoUIManager.Instance.WaitingWidgetName == "TongIcon")
                                {
                                    TimerHeap.AddTimer(100, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
                                }
                            }
                        }
                    );
                });
            }
        }
        else
        {
            if (CurrentUI != m_MenuUI)
            {
                if (CurrentUI == m_NormalMainUI)
                {
                    MenuUIViewManager.Instance.IsNormalMainUI = true;
                }
                else if (CurrentUI == m_MainUI)
                {
                    MenuUIViewManager.Instance.IsNormalMainUI = false;
                    LoggerHelper.Debug("Here");
                }
                else
                {
                    MenuUIViewManager.Instance.IsNormalMainUI = true;
                }

                MenuUILogicManager.Instance.UpdateRole(MogoWorld.thePlayer);

                ShowCurrentUI(false);
                CurrentUI = m_MenuUI;
                ShowCurrentUI(true);

                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(false);
                m_camUI.clearFlags = CameraClearFlags.SolidColor;

                ShowBillboardList(false);

                if (callback != null)
                {
                    callback();
                }

                EventDispatcher.TriggerEvent(InventoryManager.ON_INVENTORY_SHOW);
            }
        }
    }

    public void ShowMogoMenuUI(Action callback = null)
    {
        if (!UICanChange)
            return;
        MogoUIQueue.Instance.PushOne(() => { TruelyShowMogoMenuUI(callback); });

    }

    bool IsLoginUILoaded = false;
    public void ShowMogoLoginUI(Action callback, Action<float> progress = null)
    {
        if (!UICanChange)
            return;
        //if (CurrentUI)
        //    CurrentUI.SetActive(false);
        //CurrentUI = m_LoginUI;
        //CurrentUI.SetActive(true);
        //if (MogoMainCamera.instance)
        //    MogoMainCamera.instance.SetActive(false);

        //m_camUI.clearFlags = CameraClearFlags.Depth;
        //ShowBillboardList(false);

        if (m_LoginUI == null)
        {
            if (!IsLoginUILoaded)
            {
                IsLoginUILoaded = true;

                //CallWhenUILoad(0,false);
                AssetCacheMgr.GetUIInstance("LoginUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_LoginUI = go as GameObject;
                        m_LoginUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_LoginUI.transform.localPosition = new Vector3(313.8f, 0, 0);
                        m_LoginUI.transform.localScale = new Vector3(1, 1, 1);
                        m_LoginUI.AddComponent<LoginUIViewManager>();

                        if (CurrentUI != m_LoginUI)
                        {
                            if (CurrentUI != null)
                            {
                                ShowCurrentUI(false);
                            }
                            CurrentUI = m_LoginUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            //if (MogoMainCamera.instance)
                            //    MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (callback != null)
                            {
                                callback();
                            }

                            //MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                        }
                    }, progress);
            }
        }
        else
        {
            if (CurrentUI != m_LoginUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_LoginUI;
                ShowCurrentUI(true);

                m_camUI.clearFlags = CameraClearFlags.Depth;
                ShowBillboardList(false);

                //if (MogoMainCamera.instance)
                //    MogoMainCamera.instance.SetActive(false);

                if (callback != null)
                {
                    callback();
                }
            }
        }
    }     

    public void ShowMogoEquipmentUI(bool isManualOpen = true)
    {
        if (!UICanChange)
            return;
        if (m_EquipmentUI == null)
        {
            //CallWhenUILoad(0,false);
            AssetCacheMgr.GetUIInstance("EquipmentUI.prefab",
                (prefab, guid, go) =>
                {
                    m_EquipmentUI = go as GameObject;
                    m_EquipmentUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_EquipmentUI.transform.localPosition = Vector3.zero;
                    m_EquipmentUI.transform.localScale = new Vector3(1, 1, 1);
                    m_EquipmentUI.AddComponent<EquipmentUIViewManager>();

                    if (CurrentUI != m_EquipmentUI)
                    {
                        ShowCurrentUI(false);
                        CurrentUI = m_EquipmentUI;
                        ShowCurrentUI(true);


                        if (MogoMainCamera.instance)
                            MogoMainCamera.instance.SetActive(false);
                        m_camUI.clearFlags = CameraClearFlags.SolidColor;

                        if (isManualOpen)
                        {
                            SwitchStrenthUI();
                        }

                        if (WaitingWidgetName == "InsetIcon" || WaitingWidgetName == "ComposeIcon" ||
                            WaitingWidgetName == "DecomposeIcon" || WaitingWidgetName == "StrenthenIcon")
                        {
                            EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                        }

                        ShowBillboardList(false);

                    }
                }
            );
        }
        else
        {
            if (CurrentUI != m_EquipmentUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_EquipmentUI;
                ShowCurrentUI(true);


                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(false);
                m_camUI.clearFlags = CameraClearFlags.SolidColor;

                ShowBillboardList(false);
                if (isManualOpen)
                {
                    SwitchStrenthUI();
                }

            }
        }
    }

    public void ShowMogoEquipmentUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_EquipmentUI == null)
        {

            CallWhenUILoad();
            AssetCacheMgr.GetUIInstance("EquipmentUI.prefab",
                (prefab, guid, go) =>
                {
                    m_EquipmentUI = go as GameObject;
                    m_EquipmentUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_EquipmentUI.transform.localPosition = Vector3.zero;
                    m_EquipmentUI.transform.localScale = new Vector3(1, 1, 1);
                    m_EquipmentUI.AddComponent<EquipmentUIViewManager>();

                    if (CurrentUI != m_EquipmentUI)
                    {
                        ShowCurrentUI(false);
                        CurrentUI = m_EquipmentUI;
                        ShowCurrentUI(true);


                        //if (MogoMainCamera.instance)
                        //    MogoMainCamera.instance.SetActive(false);

                        //m_camUI.clearFlags = CameraClearFlags.SolidColor;



                        ShowBillboardList(false);

                        if (act != null)
                            act();


                    }
                }
            );
        }
        else
        {
            if (CurrentUI != m_EquipmentUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_EquipmentUI;
                ShowCurrentUI(true);


                //if (MogoMainCamera.instance)
                //    MogoMainCamera.instance.SetActive(false);

                //m_camUI.clearFlags = CameraClearFlags.SolidColor;

                ShowBillboardList(false);

                EventDispatcher.TriggerEvent(InventoryManager.ON_EQUIP_COMSUME_SHOW);
            }
        }
    }

    public void ShowMogoCreateCharacterUI()
    {
        if (!UICanChange)
            return;
        ShowCurrentUI(false);
        CurrentUI = m_CreateCharacterUI;
        ShowCurrentUI(true);
        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(false);

        m_camUI.clearFlags = CameraClearFlags.SolidColor;

        ShowBillboardList(false);
    }

    public void ShowMogoChooseCharacterUI()
    {
        if (!UICanChange)
            return;
        ShowCurrentUI(false);
        CurrentUI = m_ChooseCharacterUI;
        ShowCurrentUI(true);
        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(false);

        m_camUI.clearFlags = CameraClearFlags.SolidColor;

        ShowBillboardList(false);
    }

    public bool IsTaskUILoaded = false;

    private void TruleyShowMogoTaskUI()
    {
        if (!UICanChange)
            return;
        if (m_TaskUI == null)
        {
            if (!IsTaskUILoaded)
            {
                IsTaskUILoaded = true;


                CallWhenUILoad(0, false);
                AssetCacheMgr.GetUIInstance("TaskUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_TaskUI = go as GameObject;
                        m_TaskUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_TaskUI.transform.localPosition = Vector3.zero;
                        m_TaskUI.transform.localScale = new Vector3(1, 1, 1);
                        m_TaskUI.AddComponent<TaskUIViewManager>();

                        if (CurrentUI == m_NormalMainUI)
                        {
                            TaskUIViewManager.Instance.IsNormalMainUI = true;
                        }
                        else
                        {
                            TaskUIViewManager.Instance.IsNormalMainUI = false;
                        }

                        if (CurrentUI != m_TaskUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_TaskUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(true);

                            ShowBillboardList(false);
                        }

                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                    }
                );
            }
        }
        else
        {
            if (CurrentUI != m_TaskUI)
            {
                if (CurrentUI == m_NormalMainUI)
                {
                    TaskUIViewManager.Instance.IsNormalMainUI = true;
                }
                else
                {
                    TaskUIViewManager.Instance.IsNormalMainUI = false;
                }
            }

            ShowCurrentUI(false);
            CurrentUI = m_TaskUI;
            ShowCurrentUI(true);

            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(true);

            m_camUI.clearFlags = CameraClearFlags.Depth;

            ShowBillboardList(false);


        }
    }
    public void ShowMogoTaskUI()
    {
        if (!UICanChange)
            return;
        MogoUIQueue.Instance.PushOne(() => { TruleyShowMogoTaskUI(); }, null, "ShowMogoTaskUI");
    }

    private void TruleyShowMogoTaskUI<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        if (!UICanChange)
            return;
        if (m_TaskUI == null)
        {
            if (!IsTaskUILoaded)
            {
                IsTaskUILoaded = true;

                CallWhenUILoad(0, false);
                AssetCacheMgr.GetUIInstance("TaskUI.prefab", (prefab, guid, go) =>
                {
                    m_TaskUI = go as GameObject;
                    m_TaskUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_TaskUI.transform.localPosition = Vector3.zero;
                    m_TaskUI.transform.localScale = new Vector3(1, 1, 1);
                    m_TaskUI.AddComponent<TaskUIViewManager>();

                    //Mogo.Util.LoggerHelper.Debug("Before UI is " + m_CurrentUI.name + "!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    //if (m_CurrentUI == m_NormalMainUI)
                    if (true)
                    {
                        m_TaskUI = go as GameObject;
                        m_TaskUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_TaskUI.transform.localPosition = new Vector3(0, 0, 0);
                        m_TaskUI.transform.localScale = new Vector3(1, 1, 1);

                        if (CurrentUI == m_NormalMainUI)
                        {
                            TaskUIViewManager.Instance.IsNormalMainUI = true;
                        }
                        else
                        {
                            TaskUIViewManager.Instance.IsNormalMainUI = false;
                        }

                        if (CurrentUI != m_TaskUI)
                        {
                            if (CurrentUI == m_MainUI)
                                ShowMogoCommuntiyUI(CommunityUIParent.MainUI, false);

                            ShowCurrentUI(false);
                            CurrentUI = m_TaskUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(true);

                            ShowBillboardList(false);

                            action(arg1, arg2, arg3);
                        }
                    }

                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                }
                );
            }
        }
        else
        {
            if (CurrentUI != m_TaskUI)
            {
                if (CurrentUI == m_MainUI)
                    ShowMogoCommuntiyUI(CommunityUIParent.MainUI, false);

                if (CurrentUI == m_NormalMainUI)
                {
                    TaskUIViewManager.Instance.IsNormalMainUI = true;
                }
                else
                {
                    TaskUIViewManager.Instance.IsNormalMainUI = false;
                }
            }

            ShowCurrentUI(false);
            CurrentUI = m_TaskUI;
            ShowCurrentUI(true);

            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(true);

            m_camUI.clearFlags = CameraClearFlags.Depth;

            ShowBillboardList(false);

            action(arg1, arg2, arg3);
        }
    }

    public void ShowMogoTaskUI<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, GameObject baseUI = null)
    {
        if (!UICanChange)
            return;
        if (baseUI == null)
        {
            MogoUIQueue.Instance.PushOne(() => { TruleyShowMogoTaskUI(action, arg1, arg2, arg3); }, m_NormalMainUI, "TaskUI");
        }
        else
        {
            MogoUIQueue.Instance.PushOne(() => { TruleyShowMogoTaskUI(action, arg1, arg2, arg3); }, baseUI, "TaskUI");
        }
        //TruleyShowMogoTaskUI(action, arg1, arg2, arg3);
    }
    public void ShowGuideTaskUI<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, GameObject baseUI = null)
    {
        if (!UICanChange)
            return;

        if (MogoUIQueue.Instance.IsLocking)
        {
            OpenWindow((int)WindowName.Dialog, () =>
            {
                action(arg1, arg2, arg3);
            }, GameObject.Find("TeachUIPanel").transform);
        }
        else
        {
            if (baseUI == null)
            {
                MogoUIQueue.Instance.PushOne(() =>
                {
                    OpenWindow((int)WindowName.Dialog, () =>
                    {
                        action(arg1, arg2, arg3);
                    }, GameObject.Find("TeachUIPanel").transform);
                }, m_NormalMainUI, "ShowGuideTaskUI");
            }
            else
            {
                MogoUIQueue.Instance.PushOne(() =>
                {
                    OpenWindow((int)WindowName.Dialog, () =>
                    {
                        action(arg1, arg2, arg3);
                    }, GameObject.Find("TeachUIPanel").transform);
                }, baseUI, "ShowGuideTaskUI");
            }

        }
    }
    public bool IsNormalMainUILoaded = false;
    public bool isMainUI()
    {
        if (CurrentUI != m_NormalMainUI)
            return false;
        return true;
    }

    private void TruelyShowMogoNormalMainUI()
    {
        if (!UICanChange)
            return;

        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        NormalMainUIViewManager.Instance.SetUIDirty();

        if (m_NormalMainUI == null)
        {
            if (!IsNormalMainUILoaded)
            {
                IsNormalMainUILoaded = true;
                AssetCacheMgr.GetUIInstance("NormalMainUI.prefab",
                    (prefab, guid, go) =>
                    {


                        m_NormalMainUI = go as GameObject;
                        m_NormalMainUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_NormalMainUI.transform.localPosition = new Vector3(0, 0, 10);
                        m_NormalMainUI.transform.localScale = new Vector3(1, 1, 1);
                        m_NormalMainUI.AddComponent<NormalMainUIViewManager>();

                        ShowCurrentUI(false);
                        CurrentUI = m_NormalMainUI;
                        ShowCurrentUI(true);

                        if (MogoMainCamera.instance)
                            MogoMainCamera.instance.SetActive(true);

                        m_camUI.clearFlags = CameraClearFlags.Depth;

                        ShowBillboardList(true);
                        ShowMogoCommuntiyUI(CommunityUIParent.NormalMainUI, false);
                        ShowSocialTip();

                        NormalMainUIViewManager.Instance.ShowNormalMainUIPlaysIcon(MogoWorld.thePlayer.level);

                        if (WaitingWidgetName == "NormalMainUIPlayerInfoBG" ||
                            WaitingWidgetName == "AutoTaskPlayIcon" ||
                            WaitingWidgetName == "EquipmentConsumeIcon" ||
                            WaitingWidgetName == "ChargeRewardIcon" ||
                            WaitingWidgetName == "DiamondToGoldIcon" ||
                            WaitingWidgetName == "DragonConsumeIcon" ||
                            WaitingWidgetName == "XDailyTaskIcon")
                        {
                            EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                        }


                        CallWhenUILoad();

                    }
                );


            }
        }
        else
        {
            if (true)
            {




                if (CurrentUI != null)
                {
                    ShowCurrentUI(false);
                }
                CurrentUI = m_NormalMainUI;
                ShowCurrentUI(true);


                NormalMainUIViewManager.Instance.ShowCommunityButton(true);

                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(true);

                m_camUI.clearFlags = CameraClearFlags.Depth;

                ShowBillboardList(true);

                EventDispatcher.TriggerEvent("ShowMogoNormalMainUI");

                if (WaitingWidgetName == "NormalMainUIPlayerInfoBG" ||
                            WaitingWidgetName == "AutoTaskPlayIcon" ||
                    WaitingWidgetName == "EquipmentConsumeIcon" ||
                            WaitingWidgetName == "ChargeRewardIcon" ||
                            WaitingWidgetName == "DiamondToGoldIcon" ||
                            WaitingWidgetName == "DragonConsumeIcon" ||
                            WaitingWidgetName == "XDailyTaskIcon")
                {
                    EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                }

                if (MogoWorld.thePlayer != null && MogoWorld.thePlayer.CurrentTask != null)
                    NormalMainUIViewManager.Instance.SetAutoTaskIcon(IconData.dataMap.Get(MogoWorld.thePlayer.CurrentTask.autoIcon).path);
            }
        }
    }
    public void ShowMogoNormalMainUI()
    {
        if (!UICanChange)
            return;
        //if (m_CurrentUI == m_NormalMainUI)
        //    return;

        Mogo.Util.LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ " + MogoWorld.thePlayer);
        if (MogoWorld.thePlayer != null && NormalMainUIViewManager.Instance != null)
        {
            Mogo.Util.LoggerHelper.Debug("############################# " + MogoWorld.thePlayer);
            NormalMainUIViewManager.Instance.ShowNormalMainUIPlaysIcon(MogoWorld.thePlayer.level);
        }
        MogoUIQueue.Instance.PushOne(() => { TruelyShowMogoNormalMainUI(); }, null, "ShowMogoNormalMainUI");
    }

    public bool IsBattleMenuUILoaded = false;

    public void ShowMogoBattleMenuUI()
    {
        if (!UICanChange)
            return;
        if (m_BattleMenuUI == null)
        {
            CallWhenUILoad(0, false);
            if (!IsBattleMenuUILoaded)
            {
                IsBattleMenuUILoaded = true;
                AssetCacheMgr.GetUIInstance("BattleMenuUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_BattleMenuUI = go as GameObject;
                        m_BattleMenuUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_BattleMenuUI.transform.localPosition = Vector3.zero;
                        m_BattleMenuUI.transform.localScale = new Vector3(1, 1, 1);
                        m_BattleMenuUI.AddComponent<BattleMenuUIViewManager>();

                        if (CurrentUI != m_BattleMenuUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_BattleMenuUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            ShowBillboardList(false);
                        }

                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                    }
                );
            }
        }
        else
        {
            if (CurrentUI != m_BattleMenuUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_BattleMenuUI;
                ShowCurrentUI(true);

                m_camUI.clearFlags = CameraClearFlags.Depth;

                ShowBillboardList(false);
            }
        }
    }

    #region �������

    /// <summary>
    /// ����
    /// </summary>
    public bool IsDragonUILoaded = false;
    public void ShowMogoDragonUI()
    {
        return;//MaiFeo
        //Ϊ���������ע�����´���
        //if (!UICanChange)
        //    return;
        //if (m_DragonUI == null)
        //{
        //    if (!IsDragonUILoaded)
        //    {
        //        IsDragonUILoaded = true;
        //        CallWhenUILoad();

        //        AssetCacheMgr.GetUIInstance("DragonUI.prefab",
        //            (prefab, guid, go) =>
        //            {
        //                m_DragonUI = go as GameObject;
        //                m_DragonUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
        //                m_DragonUI.transform.localPosition = Vector3.zero;
        //                m_DragonUI.transform.localScale = new Vector3(1, 1, 1);
        //                m_DragonUI.AddComponent<DragonUIViewManager>();

        //                if (CurrentUI != m_DragonUI)
        //                {
        //                    CurrentUI.SetActive(false);
        //                    CurrentUI = m_DragonUI;
        //                    CurrentUI.SetActive(true);

        //                    EventDispatcher.TriggerEvent(Events.RuneEvent.GetRuneBag);

        //                    if (MogoMainCamera.instance)
        //                        MogoMainCamera.instance.SetActive(false);

        //                    m_camUI.clearFlags = CameraClearFlags.SolidColor;

        //                    ShowBillboardList(false);

        //                }
        //            }
        //        );
        //    }
        //}
        //else
        //{
        //    if (CurrentUI != m_DragonUI)
        //    {
        //        CurrentUI.SetActive(false);
        //        CurrentUI = m_DragonUI;
        //        CurrentUI.SetActive(true);

        //        EventDispatcher.TriggerEvent(Events.RuneEvent.GetRuneBag);

        //        if (MogoMainCamera.instance)
        //            MogoMainCamera.instance.SetActive(false);

        //        m_camUI.clearFlags = CameraClearFlags.SolidColor;

        //        ShowBillboardList(false);
        //    }
        //}

    }

    /// <summary>
    /// ����
    /// </summary>
    public bool IsRuneUILoaded = false;
    public void ShowMogoRuneUI()
    {
        return;//MaiFeo
        //Ϊ���������ע�����´���
        //if (!UICanChange)
        //    return;
        //if (m_RuneUI == null)
        //{
        //    if (!IsRuneUILoaded)
        //    {
        //        IsRuneUILoaded = true;



        //        CallWhenUILoad();
        //        AssetCacheMgr.GetUIInstance("RuneUI.prefab",
        //            (prefab, guid, go) =>
        //            {
        //                m_RuneUI = go as GameObject;
        //                m_RuneUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
        //                m_RuneUI.transform.localPosition = Vector3.zero;
        //                m_RuneUI.transform.localScale = new Vector3(1, 1, 1);
        //                m_RuneUI.AddComponent<RuneUIViewManager>();
        //                m_RuneUI.SetActive(false);

        //                if (CurrentUI != m_RuneUI)
        //                {
        //                    CurrentUI.SetActive(false);
        //                    CurrentUI = m_RuneUI;
        //                    CurrentUI.SetActive(true);

        //                    EventDispatcher.TriggerEvent(Events.RuneEvent.GetRuneBag);
        //                    EventDispatcher.TriggerEvent(Events.RuneEvent.GetBodyRunes);

        //                    if (MogoMainCamera.instance)
        //                        MogoMainCamera.instance.SetActive(false);

        //                    m_camUI.clearFlags = CameraClearFlags.SolidColor;

        //                    ShowBillboardList(false);

        //                }
        //            }
        //        );
        //    }
        //}
        //else
        //{
        //    if (CurrentUI != m_RuneUI)
        //    {
        //        CurrentUI.SetActive(false);
        //        CurrentUI = m_RuneUI;
        //        CurrentUI.SetActive(true);

        //        EventDispatcher.TriggerEvent(Events.RuneEvent.GetRuneBag);
        //        EventDispatcher.TriggerEvent(Events.RuneEvent.GetBodyRunes);

        //        if (MogoMainCamera.instance)
        //            MogoMainCamera.instance.SetActive(false);

        //        m_camUI.clearFlags = CameraClearFlags.SolidColor;

        //        ShowBillboardList(false);

        //    }
        //}
    }

    #endregion

    public void ShowMogoChooseServerUI()
    {
        if (!UICanChange)
            return;
        ShowCurrentUI(false);
        CurrentUI = m_ChooseServerUI;
        ShowCurrentUI(true);

        MogoUIPreManager.Instance.PreShowChooseServerUI();

        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(false);

        m_camUI.clearFlags = CameraClearFlags.Depth;

        ShowBillboardList(false);
    }

    public void ShowChatUI()
    {
        if (!UICanChange)
            return;
        ShowCurrentUI(false);
        CurrentUI = m_ChatUI;
        ShowCurrentUI(true);

        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(false);

        m_camUI.clearFlags = CameraClearFlags.Depth;

        ShowBillboardList(false);
    }

    #region �������UI

    #region �������ͼ����

    public void LoadMogoInstanceUI(Action action, bool isShow)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstanceUI)
        {

            if (SystemSwitch.DestroyAllUI)
                CallWhenUILoad(0, false);

            AssetCacheMgr.GetUIInstance("InstanceUI.prefab",
                (prefab, guid, go) =>
                {
                    m_InstanceUI = go as GameObject;
                    m_InstanceUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstanceUI.transform.localPosition = Vector3.zero;
                    m_InstanceUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstanceUI.AddComponent<InstanceUIViewManager>();

                    if (CurrentUI != null && CurrentUI != m_InstanceUI)
                    {
                        MogoUIPreManager.Instance.PreShowInstanceUI();

                        // InstanceUILogicManager.Instance.SetMapID(InstanceUILogicManager.Instance.mapID);
                    }

                    m_InstanceUI.SetActive(isShow);
                    hasLoadedInstanceUI = true;
                    action();

                    if (SystemSwitch.DestroyAllUI)
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                }
            );
        }
        else
        {
            if (!isShow && m_InstanceUI == null)
            {
                action();
                return;
            }
            m_InstanceUI.SetActive(isShow);

            action();
        }
    }

    public bool hasLoadedInstanceUI = false;
    private void TruelyShowMogoInstanceUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstanceUI)
        {
            AssetCacheMgr.GetUIInstance("InstanceUI.prefab",
                (prefab, guid, go) =>
                {
                    m_InstanceUI = go as GameObject;
                    m_InstanceUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstanceUI.transform.localPosition = Vector3.zero;
                    m_InstanceUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstanceUI.AddComponent<InstanceUIViewManager>();

                    if (CurrentUI != null && CurrentUI != m_InstanceUI)
                    {
                        ShowCurrentUI(false);
                        CurrentUI = m_InstanceUI;
                        ShowCurrentUI(true);

                        // MogoUIPreManager.Instance.PreShowInstanceUI();

                        InstanceUILogicManager.Instance.SetMapID(InstanceUILogicManager.Instance.MapID);

                        //if (MogoMainCamera.instance)
                        //    MogoMainCamera.instance.SetActive(false);

                        //m_camUI.clearFlags = CameraClearFlags.SolidColor;

                        //ShowBillboardList(false);
                    }

                    ShowCurrentUI(isShow);
                    hasLoadedInstanceUI = true;

                    CallWhenUILoad();
                }
            );
        }
        else
        {
            if (CurrentUI != m_InstanceUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_InstanceUI;
                ShowCurrentUI(true);

                // MogoUIPreManager.Instance.PreShowInstanceUI();

                //if (MogoMainCamera.instance)
                //    MogoMainCamera.instance.SetActive(false);

                //m_camUI.clearFlags = CameraClearFlags.SolidColor;

                //ShowBillboardList(false);

                if (InstanceUIViewManager.Instance != null)
                {
                    // InstanceUIViewManager.Instance.ShowInstanceChooseUI(true);

                    // ֻҪ�������ɲ���Ҫ������д
                    //EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateEnterableMissions);
                    //EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateMissionTimes);
                    //EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateMissionStars);

                    InstanceUILogicManager.Instance.SetMapID(InstanceUILogicManager.Instance.MapID);

                    // InstanceUILogicManager.Instance.UpdateGridMessage();
                }

                EventDispatcher.TriggerEvent(Events.InstanceUIEvent.UpdateMapName);
            }
        }
    }

    public void ShowMogoInstanceUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        MogoUIQueue.Instance.PushOne(() => { TruelyShowMogoInstanceUI(isShow); });
    }

    // ����򿪸���UIר��
    private void TruelyShowMogoInstanceUI<T, U>(Action<T, U> action, T arg1, U arg2, bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstanceUI)
        {

            CallWhenUILoad();
            AssetCacheMgr.GetUIInstance("InstanceUI.prefab",
                (prefab, guid, go) =>
                {
                    m_InstanceUI = go as GameObject;
                    m_InstanceUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstanceUI.transform.localPosition = Vector3.zero;
                    m_InstanceUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstanceUI.AddComponent<InstanceUIViewManager>();

                    if (CurrentUI != null && CurrentUI != m_InstanceUI)
                    {
                        ShowCurrentUI(false);
                        CurrentUI = m_InstanceUI;
                        ShowCurrentUI(true);

                        MogoUIPreManager.Instance.PreShowInstanceUI();

                        InstanceUILogicManager.Instance.SetMapID(InstanceUILogicManager.Instance.MapID);

                        action(arg1, arg2);
                    }

                    ShowCurrentUI(isShow);
                    hasLoadedInstanceUI = true;

                }
            );
        }
        else
        {
            if (CurrentUI != m_InstanceUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_InstanceUI;
                ShowCurrentUI(true);

                // MogoUIPreManager.Instance.PreShowInstanceUI();

                //if (MogoMainCamera.instance)
                //    MogoMainCamera.instance.SetActive(false);

                //m_camUI.clearFlags = CameraClearFlags.SolidColor;

                // InstanceUIViewManager.Instance.ShowInstanceChooseUI(true);

                //ShowBillboardList(false);

                if (InstanceUIViewManager.Instance != null)
                {
                    //EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateEnterableMissions);
                    //EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateMissionTimes);
                    //EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateMissionStars);

                    // EventDispatcher.TriggerEvent(Events.InstanceEvent.UpdateMissionMessage, true);

                    InstanceUILogicManager.Instance.SetMapID(InstanceUILogicManager.Instance.MapID);
                    // InstanceUILogicManager.Instance.UpdateGridMessage();
                }

                action(arg1, arg2);
            }
        }
    }

    public void ShowMogoInstanceUI<T, U>(Action<T, U> action, T arg1, U arg2, bool isShow = true)
    {
        if (!UICanChange)
            return;
        MogoUIQueue.Instance.PushOne(() => { TruelyShowMogoInstanceUI(action, arg1, arg2, isShow); });
    }

    #endregion

    #region �����ؿ�ѡ�����(һҳ)

    public bool IsNewInstanceChooseLevelUILoaded = false;
    public void ShowNewInstanceChooseMissionUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_NewInstanceChooseLevelUI == null)
        {
            if (!IsNewInstanceChooseLevelUILoaded)
            {
                IsNewInstanceChooseLevelUILoaded = true;

                AssetCacheMgr.GetUIInstance("NewInstanceUIChooseLevel.prefab",
                    (prefab, guid, go) =>
                    {
                        m_NewInstanceChooseLevelUI = go as GameObject;
                        m_NewInstanceChooseLevelUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_NewInstanceChooseLevelUI.transform.localPosition = Vector3.zero;
                        m_NewInstanceChooseLevelUI.transform.localScale = new Vector3(1, 1, 1);
                        m_NewInstanceChooseLevelUI.AddComponent<NewInstanceUIChooseLevelViewManager>();
                        m_NewInstanceChooseLevelUI.SetActive(true);

                        if (!isShow)
                        {
                            m_NewInstanceChooseLevelUI.SetActive(false);
                            return;
                        }

                        if (CurrentUI != m_NewInstanceChooseLevelUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_NewInstanceChooseLevelUI;
                            ShowCurrentUI(true);

                            //if (MogoMainCamera.instance)
                            //    MogoMainCamera.instance.SetActive(false);

                            //m_camUI.clearFlags = CameraClearFlags.SolidColor;
                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            ShowBillboardList(false);

                        }

                        m_CurrentUI.SetActive(isShow);


                        //CallWhenUILoad();
                    }
                );
            }
        }
        else
        {
            if (!isShow)
            {
                m_NewInstanceChooseLevelUI.SetActive(isShow);
                return;
            }

            if (CurrentUI != m_NewInstanceChooseLevelUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_NewInstanceChooseLevelUI;
                ShowCurrentUI(true);

                //if (MogoMainCamera.instance)
                //    MogoMainCamera.instance.SetActive(false);

                //m_camUI.clearFlags = CameraClearFlags.SolidColor;
                m_camUI.clearFlags = CameraClearFlags.Depth;
                if (InstanceUILogicManager.Instance != null)
                {
                    InstanceUILogicManager.Instance.UpdateGridMessage();
                    InstanceUILogicManager.Instance.UpdateTopMessage();
                    InstanceUILogicManager.Instance.GetFoggyAbyssMessage();
                }

                m_camUI.clearFlags = CameraClearFlags.SolidColor;
            }
        }
    }

    #endregion

    #region �����ؿ�ѡ�����(��ҳ���϶�)

    public bool IsInstanceMissionChooseUILoaded = false;
    public void ShowInstanceMissionChooseUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_InstanceMissionChooseUI == null)
        {
            if (!IsInstanceMissionChooseUILoaded)
            {
                IsInstanceMissionChooseUILoaded = true;

                AssetCacheMgr.GetUIInstance("InstanceMissionChooseUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_InstanceMissionChooseUI = go as GameObject;
                        m_InstanceMissionChooseUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_InstanceMissionChooseUI.transform.localPosition = Vector3.zero;
                        m_InstanceMissionChooseUI.transform.localScale = new Vector3(1, 1, 1);
                        m_InstanceMissionChooseUI.AddComponent<InstanceMissionChooseUIViewManager>();

                        if (!isShow)
                        {
                            if (!InstanceMissionChooseUIViewManager.SHOW_MISSION_BY_DRAG)
                                m_InstanceMissionChooseUI.SetActive(false);
                            return;
                        }

                        if (InstanceMissionChooseUIViewManager.Instance != null && isShow)
                            InstanceMissionChooseUIViewManager.Instance.UICamInstanceMissionChooseUI.enabled = true;

                        if (CurrentUI != m_InstanceMissionChooseUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_InstanceMissionChooseUI;
                            ShowCurrentUI(true);

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);
                            m_camUI.clearFlags = CameraClearFlags.SolidColor;

                            ShowBillboardList(false);
                        }

                        m_CurrentUI.SetActive(isShow);

                        //CallWhenUILoad();
                    }
                );
            }
        }
        else
        {
            if (!isShow)
            {
                m_InstanceMissionChooseUI.SetActive(isShow);
                return;
            }

            if (InstanceMissionChooseUIViewManager.Instance != null && isShow)
                InstanceMissionChooseUIViewManager.Instance.UICamInstanceMissionChooseUI.enabled = true;

            if (CurrentUI != m_InstanceMissionChooseUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_InstanceMissionChooseUI;
                ShowCurrentUI(true);

                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(false);
                m_camUI.clearFlags = CameraClearFlags.SolidColor;

                ShowBillboardList(false);

                if (InstanceUILogicManager.Instance != null)
                {
                    InstanceUILogicManager.Instance.UpdateGridMessage();
                    InstanceUILogicManager.Instance.UpdateTopMessage();
                    InstanceUILogicManager.Instance.GetFoggyAbyssMessage();
                }
            }
        }
    }

    #endregion

    #region �����Ѷ�ѡ�����

    public bool hasLoadedInstanceLevelChooseUI = false;
    public void LoadMogoInstanceLevelChooseUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstanceLevelChooseUI)
        {
            hasLoadedInstanceLevelChooseUI = true;
            if (SystemSwitch.DestroyAllUI)
                CallWhenUILoad(0, false);
            AssetCacheMgr.GetUIInstance("InstanceLevelChooseUI.prefab",
                (prefab, guid, go) =>
                {
                    m_InstanceLevelChooseUI = go as GameObject;
                    m_InstanceLevelChooseUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstanceLevelChooseUI.transform.localPosition = new Vector3(2000, 0, 0);
                    m_InstanceLevelChooseUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstanceLevelChooseUI.AddComponent<InstanceLevelChooseUIViewManager>();

                    if (!isShow)
                    {
                        m_InstanceLevelChooseUI.SetActive(false);
                        return;
                    }

                    m_camUI.clearFlags = CameraClearFlags.SolidColor;
                    if (MogoMainCamera.instance)
                        MogoMainCamera.instance.SetActive(false);

                    if (InstanceMissionChooseUIViewManager.Instance != null && isShow)
                        InstanceMissionChooseUIViewManager.Instance.UICamInstanceMissionChooseUI.enabled = false;

                    //CurrentUI.SetActive(false);
                    CurrentUI = m_InstanceLevelChooseUI;
                    ShowCurrentUI(true);

                    if (SystemSwitch.DestroyAllUI)
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                }
            );
        }
        else
        {
            m_camUI.clearFlags = CameraClearFlags.SolidColor;
            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            if (InstanceMissionChooseUIViewManager.Instance != null && isShow)
                InstanceMissionChooseUIViewManager.Instance.UICamInstanceMissionChooseUI.enabled = false;

            //CurrentUI.SetActive(false);
            CurrentUI = m_InstanceLevelChooseUI;
            ShowCurrentUI(isShow);
        }
    }

    #endregion

    #region ����ɨ������

    public bool hasLoadedInstanceCleanUI = false;
    public void LoadMogoInstanceCleanUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstanceCleanUI)
        {
            hasLoadedInstanceCleanUI = true;
            AssetCacheMgr.GetUIInstance("InstanceCleanUI.prefab",
                (prefab, guid, go) =>
                {
                    m_InstanceCleanUI = go as GameObject;
                    m_InstanceCleanUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstanceCleanUI.transform.localPosition = new Vector3(5000, 0, -10);
                    m_InstanceCleanUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstanceCleanUI.AddComponent<InstanceCleanUIViewManager>();

                    if (!isShow)
                    {
                        m_InstanceCleanUI.SetActive(false);
                        return;
                    }

                    ShowCurrentUI(false);
                    CurrentUI = m_InstanceCleanUI;
                    ShowCurrentUI(true);
                    m_InstanceCleanUI.GetComponentsInChildren<InstanceCleanUIViewManager>(true)[0].Show(true);


                    //CallWhenUILoad();
                }
            );
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_InstanceCleanUI;
            ShowCurrentUI(isShow);
            m_InstanceCleanUI.GetComponentsInChildren<InstanceCleanUIViewManager>(true)[0].Show(true);
        }
    }

    #endregion

    #region ���������������

    public bool hasLoadedInstanceHelpChooseUI = false;
    public void LoadMogoInstanceHelpChooseUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstanceHelpChooseUI)
        {
            hasLoadedInstanceHelpChooseUI = true;

            if (SystemSwitch.DestroyAllUI)
                CallWhenUILoad(0, false);
            AssetCacheMgr.GetUIInstance("InstanceHelpChooseUI.prefab",
                (prefab, guid, go) =>
                {
                    m_InstanceHelpChooseUI = go as GameObject;
                    m_InstanceHelpChooseUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstanceHelpChooseUI.transform.localPosition = new Vector3(5000, 0, -10);
                    m_InstanceHelpChooseUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstanceHelpChooseUI.AddComponent<InstanceHelpChooseUIViewManager>();

                    if (!isShow)
                    {
                        m_InstanceHelpChooseUI.SetActive(false);
                        return;
                    }

                    m_camUI.clearFlags = CameraClearFlags.SolidColor;
                    if (MogoMainCamera.instance)
                        MogoMainCamera.instance.SetActive(false);

                    ShowCurrentUI(false);
                    CurrentUI = m_InstanceHelpChooseUI;
                    ShowCurrentUI(true);

                    if (SystemSwitch.DestroyAllUI)
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                }
            );
        }
        else
        {
            m_camUI.clearFlags = CameraClearFlags.SolidColor;
            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowCurrentUI(false);
            CurrentUI = m_InstanceHelpChooseUI;
            ShowCurrentUI(isShow);
        }
    }

    #endregion

    #region ��������������

    public bool hasLoadedInstanceTaskRewardUI = false;
    public void LoadMogoInstanceTaskRewardUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstanceTaskRewardUI)
        {
            hasLoadedInstanceTaskRewardUI = true;
            AssetCacheMgr.GetUIInstance("InstanceTaskReward.prefab",
                (prefab, guid, go) =>
                {
                    m_InstanceTaskRewardUI = go as GameObject;
                    m_InstanceTaskRewardUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstanceTaskRewardUI.transform.localPosition = new Vector3(4000, 0, 0);
                    m_InstanceTaskRewardUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstanceTaskRewardUI.AddComponent<InstanceTaskRewardUIViewManager>();

                    if (!isShow)
                    {
                        m_InstanceTaskRewardUI.SetActive(false);
                        return;
                    }

                    ShowCurrentUI(false);
                    CurrentUI = m_InstanceTaskRewardUI;
                    ShowCurrentUI(true);


                    //CallWhenUILoad();
                }
            );
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_InstanceTaskRewardUI;
            ShowCurrentUI(isShow);
        }
    }

    #endregion

    #region �����������

    public bool hasLoadedInstanceTreasureChestUI = false;
    public void LoadMogoInstanceTreasureChestUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstanceTreasureChestUI)
        {
            if (SystemSwitch.DestroyAllUI)
                CallWhenUILoad(0, false);
            hasLoadedInstanceTreasureChestUI = true;
            AssetCacheMgr.GetUIInstance("InstanceTreasureChestUI.prefab",
                (prefab, guid, go) =>
                {
                    m_InstanceTreasureChestUI = go as GameObject;
                    m_InstanceTreasureChestUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstanceTreasureChestUI.transform.localPosition = new Vector3(3000, 0, 0);
                    m_InstanceTreasureChestUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstanceTreasureChestUI.AddComponent<InstanceTreasureChestUIViewManager>();

                    if (!isShow)
                    {
                        m_InstanceTreasureChestUI.SetActive(false);
                        return;
                    }

                    //CurrentUI.SetActive(false);
                    CurrentUI = m_InstanceTreasureChestUI;
                    ShowCurrentUI(true);

                    if (SystemSwitch.DestroyAllUI)
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                }
            );
        }
        else
        {
            //CurrentUI.SetActive(false);
            CurrentUI = m_InstanceTreasureChestUI;
            ShowCurrentUI(isShow);
        }
    }

    #endregion

    /// <summary>
    /// ����BOSS����
    /// </summary>
    public bool IsInstanceBossTreasureUILoaded = false;
    public void ShowInstanceBossTreasureUI(Action action, bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_InstanceBossTreasureUI == null)
        {
            if (!IsInstanceBossTreasureUILoaded)
            {
                IsInstanceBossTreasureUILoaded = true;

                AssetCacheMgr.GetUIInstance("InstanceBossTreasureUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_InstanceBossTreasureUI = go as GameObject;
                        m_InstanceBossTreasureUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_InstanceBossTreasureUI.transform.localPosition = new Vector3(3000, 0, 0);
                        m_InstanceBossTreasureUI.transform.localScale = new Vector3(1, 1, 1);
                        m_InstanceBossTreasureUI.AddComponent<InstanceBossTreasureUIViewManager>();

                        if (!isShow)
                        {
                            m_InstanceBossTreasureUI.SetActive(false);
                            return;
                        }

                        //CurrentUI.SetActive(false);
                        //CurrentUI = m_InstanceBossTreasureUI;
                        //CurrentUI.SetActive(true);

                        m_InstanceBossTreasureUI.SetActive(true);

                        //m_camUI.clearFlags = CameraClearFlags.Depth;
                        //if (MogoMainCamera.instance)
                        //    MogoMainCamera.instance.SetActive(false);

                        ShowBillboardList(false);

                        if (action != null)
                        {
                            action();
                        }
                    }
                );
            }
        }
        else
        {
            if (!isShow)
            {
                m_InstanceBossTreasureUI.SetActive(false);
                return;
            }

            //CurrentUI.SetActive(false);
            //CurrentUI = m_InstanceBossTreasureUI;
            //CurrentUI.SetActive(true);

            m_InstanceBossTreasureUI.SetActive(true);

            //m_camUI.clearFlags = CameraClearFlags.Depth;
            //if (MogoMainCamera.instance)
            //    MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (action != null)
            {
                action();
            }
        }
    }

    #region �������ؽ�������

    public bool hasLoadedInstancePassRewardUI = false;
    public void LoadMogoInstancePassRewardUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstancePassRewardUI)
        {
            hasLoadedInstancePassRewardUI = true;

            if (SystemSwitch.DestroyAllUI)
                CallWhenUILoad(0, false);
            AssetCacheMgr.GetUIInstance("InstancePassRewardUI.prefab",
                (prefab, guid, go) =>
                {
                    m_InstancePassRewardUI = go as GameObject;
                    m_InstancePassRewardUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstancePassRewardUI.transform.localPosition = Vector3.zero;
                    m_InstancePassRewardUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstancePassRewardUI.AddComponent<InstancePassRewardUIViewManager>();
                    m_InstancePassRewardUI.GetComponentsInChildren<InstancePassRewardUIViewManager>(true)[0].HideAll();

                    if (!isShow)
                    {
                        m_InstancePassRewardUI.SetActive(false);
                        return;
                    }

                    ShowCurrentUI(false);
                    CurrentUI = m_InstancePassRewardUI;
                    ShowCurrentUI(true);

                    if (SystemSwitch.DestroyAllUI)
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                }
            );
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_InstancePassRewardUI;
            m_InstancePassRewardUI.GetComponentsInChildren<InstancePassRewardUIViewManager>(true)[0].HideAll();
            ShowCurrentUI(isShow);
        }
    }   

    #endregion

    #region �������ֽ���

    public bool hasLoadedInstancePassUI = false;
    public void LoadMogoInstancePassUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (!hasLoadedInstancePassUI)
        {
            hasLoadedInstancePassUI = true;

            CallWhenUILoad();
            AssetCacheMgr.GetUIInstance("InstancePassUI.prefab",
                (prefab, guid, go) =>
                {
                    m_InstancePassUI = go as GameObject;
                    m_InstancePassUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_InstancePassUI.transform.localPosition = new Vector3(0, -30, 0);
                    m_InstancePassUI.transform.localScale = new Vector3(1, 1, 1);
                    m_InstancePassUI.AddComponent<InstancePassUIViewManager>();
                    m_InstancePassUI.GetComponentsInChildren<InstancePassUIViewManager>(true)[0].HideAll();

                    if (!isShow)
                    {
                        m_InstancePassUI.SetActive(false);
                        return;
                    }

                    ShowCurrentUI(false);
                    CurrentUI = m_InstancePassUI;
                    ShowCurrentUI(true);
                }
            );
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_InstancePassUI;
            m_InstancePassUI.GetComponentsInChildren<InstancePassUIViewManager>(true)[0].HideAll();
            ShowCurrentUI(isShow);
        }
    }

    #endregion

    #region �����������

    public void ShowMogoInstanceRebornUI(bool isShow = true, Action act = null)
    {
        if (!UICanChange)
            return;
        if (isShow)
            MogoFXManager.Instance.SceneCameraTurnGray();
        else
            MogoFXManager.Instance.SceneCameraTurnColor();

        //MogoUIQueue.Instance.PushOne(() => { TruelyShowMogoInstanceRebornUI(isShow, act); },null,"ShowMogoInstanceRebornUI");
        MogoUIQueue.Instance.PushOne(() => { TruelyShowMogoInstanceRebornNoneDropUI(isShow, act); }, null, "ShowMogoInstanceRebornNoneDropUI");
    }

    /// <summary>
    /// �ɸ������(����ʹ��)
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="act"></param>
    public bool hasLoadedInstanceRebornUI = false;
    private void TruelyShowMogoInstanceRebornUI(bool isShow = true, Action act = null)
    {
        if (!UICanChange)
            return;
        if (m_goInstanceRebornUI == null && !hasLoadedInstanceRebornUI)
        {
            Mogo.Util.LoggerHelper.Debug("TruelyShowMogoInstanceReborn.......................CurrentUI..................m_goInstanceRebornUI");
            hasLoadedInstanceRebornUI = true;

            CallWhenUILoad();
            AssetCacheMgr.GetUIInstance("InstanceRebornUI.prefab",
                (prefab, guid, go) =>
                {
                    m_goInstanceRebornUI = go as GameObject;
                    m_goInstanceRebornUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_goInstanceRebornUI.transform.localPosition = Vector3.zero;
                    m_goInstanceRebornUI.transform.localScale = new Vector3(1, 1, 1);
                    m_goInstanceRebornUI.AddComponent<InstanceUIRebornViewManager>();

                    if (CurrentUI != null && CurrentUI != m_goInstanceRebornUI)
                    {
                        ShowCurrentUI(false);
                        CurrentUI = m_goInstanceRebornUI;
                        ShowCurrentUI(true);

                        ShowCurrentUI(isShow);
                    }

                    if (act != null)
                    {
                        act();
                    }

                }
            );
        }
        else
        {
            Mogo.Util.LoggerHelper.Debug("TruelyShowMogoInstanceReborn@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@CurrentUI@@@@@@@@@m_goInstanceRebornUI");
            if (CurrentUI != m_goInstanceRebornUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_goInstanceRebornUI;
                ShowCurrentUI(true);

                if (act != null)
                {
                    act();
                }
            }
        }

    }

    /// <summary>
    /// �¸������
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="act"></param>
    public bool hasLoadedInstanceRebornNoneDropUI = false;
    private void TruelyShowMogoInstanceRebornNoneDropUI(bool isShow = true, Action act = null)
    {
        if (!UICanChange)
            return;
        if (m_goInstanceRebornNoneDropUI == null && !hasLoadedInstanceRebornNoneDropUI)
        {
            Mogo.Util.LoggerHelper.Debug("TruelyShowMogoInstanceRebornNoneDrop.......................CurrentUI..................m_goInstanceRebornNoneDropUI");
            hasLoadedInstanceRebornNoneDropUI = true;

            CallWhenUILoad(0, false);
            AssetCacheMgr.GetUIInstance("InstanceRebornNoneDropUI.prefab",
                (prefab, guid, go) =>
                {
                    m_goInstanceRebornNoneDropUI = go as GameObject;
                    m_goInstanceRebornNoneDropUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                    m_goInstanceRebornNoneDropUI.transform.localPosition = Vector3.zero;
                    m_goInstanceRebornNoneDropUI.transform.localScale = new Vector3(1, 1, 1);
                    InstanceUIRebornNoneDropViewManager manager = m_goInstanceRebornNoneDropUI.AddComponent<InstanceUIRebornNoneDropViewManager>();
                    manager.ChangeRebornUIState(InstanceUIRebornNoneDropViewManager.RebornUIState.Death);

                    if (CurrentUI != null && CurrentUI != m_goInstanceRebornNoneDropUI)
                    {
                        ShowCurrentUI(false);
                        CurrentUI = m_goInstanceRebornNoneDropUI;
                        ShowCurrentUI(true);

                        ShowCurrentUI(isShow);

                        //if (MogoMainCamera.instance)
                        //    MogoMainCamera.instance.SetActive(false);
                        m_camUI.clearFlags = CameraClearFlags.Depth;
                    }

                    if (act != null)
                    {
                        act();
                    }

                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                }
            );
        }
        else
        {
            if (m_goInstanceRebornNoneDropUI == null)
                return;

            Mogo.Util.LoggerHelper.Debug("TruelyShowMogoInstanceRebornNoneDrop@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@CurrentUI@@@@@@@@@m_goInstanceRebornNoneDropUI");
            if (CurrentUI != m_goInstanceRebornNoneDropUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_goInstanceRebornNoneDropUI;
                ShowCurrentUI(true);

                //if (MogoMainCamera.instance)
                //    MogoMainCamera.instance.SetActive(false);
                m_camUI.clearFlags = CameraClearFlags.Depth;

                if (act != null)
                {
                    act();
                }
            }
        }
    }

    #endregion

    #endregion

    #region ����֮��

    /// <summary>
    /// ����֮��
    /// </summary>
    public bool IsDoorOfBuryUILoaded = false;
    public void ShowMogoDoorOfBuryUI(Action OnLoaded)
    {
        if (!UICanChange)
            return;
        if (m_DoorOfBuryUI == null)
        {
            if (!IsDoorOfBuryUILoaded)
            {
                IsDoorOfBuryUILoaded = true;


                CallWhenUILoad(0, false);
                AssetCacheMgr.GetUIInstance("DoorOfBuryUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_DoorOfBuryUI = go as GameObject;
                        m_DoorOfBuryUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_DoorOfBuryUI.transform.localPosition = Vector3.zero;
                        m_DoorOfBuryUI.transform.localScale = new Vector3(1, 1, 1);
                        m_DoorOfBuryUI.AddComponent<DoorOfBuryUIViewManager>();

                        if (CurrentUI != m_DoorOfBuryUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_DoorOfBuryUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            ShowBillboardList(false);

                        }
                        OnLoaded();
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                    }
                );
            }
        }
        else
        {
            if (CurrentUI != m_DoorOfBuryUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_DoorOfBuryUI;
                ShowCurrentUI(true);

                m_camUI.clearFlags = CameraClearFlags.Depth;

                ShowBillboardList(false);
            }
            OnLoaded();
        }
    }

    #endregion

    #region VIP

    /// <summary>
    /// VIPϵͳ
    /// </summary>
    public bool IsVIPInfoUILoaded = false;
    public void ShowVIPInfoUI()
    {
        if (!UICanChange)
            return;
        if (m_VIPInfoUI == null)
        {
            if (!IsVIPInfoUILoaded)
            {
                IsVIPInfoUILoaded = true;

                CallWhenUILoad(0, false);

                AssetCacheMgr.GetUIInstance("VIPInfoUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_VIPInfoUI = go as GameObject;
                        m_VIPInfoUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_VIPInfoUI.transform.localPosition = Vector3.zero;
                        m_VIPInfoUI.transform.localScale = new Vector3(1, 1, 1);
                        m_VIPInfoUI.AddComponent<VIPInfoUIViewManager>();

                        if (CurrentUI != m_VIPInfoUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_VIPInfoUI;
                            ShowCurrentUI(true);
                            m_VIPInfoUI.GetComponentsInChildren<VIPInfoUIViewManager>(true)[0].Show(true);

                            m_camUI.clearFlags = CameraClearFlags.SolidColor;
                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);
                        }

                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_VIPInfoUI;
            ShowCurrentUI(true);
            m_VIPInfoUI.GetComponentsInChildren<VIPInfoUIViewManager>(true)[0].Show(true);

            m_camUI.clearFlags = CameraClearFlags.SolidColor;
            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);
        }
    }

    #endregion

    #region ��������

    /// <summary>
    /// ��������ϵͳ
    /// </summary>
    public bool IsEnergyUILoaded = false;
    public void ShowEnergyUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_EnergyUI == null)
        {
            if (!IsEnergyUILoaded)
            {
                IsEnergyUILoaded = true;

                AssetCacheMgr.GetUIInstance("EnergyUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_EnergyUI = go as GameObject;
                        m_EnergyUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_EnergyUI.transform.localPosition = Vector3.zero;
                        m_EnergyUI.transform.localScale = new Vector3(1, 1, 1);
                        m_EnergyUI.AddComponent<EnergyUIViewManager>();

                        if (CurrentUI != m_EnergyUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_EnergyUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.SolidColor;
                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (act != null)
                            {
                                act();
                            }
                        }
                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_EnergyUI;
            ShowCurrentUI(true);

            m_camUI.clearFlags = CameraClearFlags.SolidColor;
            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    #endregion

    #region ����

    /// <summary>
    /// ����ϵͳ
    /// </summary>
    public bool IsDiamondToGoldUILoaded = false;
    public void ShowDiamondToGoldUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_DiamondToGoldUI == null)
        {
            if (!IsDiamondToGoldUILoaded)
            {
                IsDiamondToGoldUILoaded = true;

                CallWhenUILoad(0, false);

                AssetCacheMgr.GetUIInstance("DiamondToGoldUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_DiamondToGoldUI = go as GameObject;
                        m_DiamondToGoldUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_DiamondToGoldUI.transform.localPosition = Vector3.zero;
                        m_DiamondToGoldUI.transform.localScale = new Vector3(1, 1, 1);
                        m_DiamondToGoldUI.AddComponent<DiamondToGoldUIViewManager>();

                        if (CurrentUI != m_DiamondToGoldUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_DiamondToGoldUI;
                            ShowCurrentUI(true);
                            m_DiamondToGoldUI.GetComponentsInChildren<DiamondToGoldUIViewManager>(true)[0].Show(true);

                            m_camUI.clearFlags = CameraClearFlags.SolidColor;
                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (act != null)
                            {
                                act();
                            }

                            if (MogoUIManager.Instance.WaitingWidgetName == "DiamondToGoldUIBtnTurn2")
                            {
                                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                            }

                            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                        }

                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_DiamondToGoldUI;
            ShowCurrentUI(true);
            m_DiamondToGoldUI.GetComponentsInChildren<DiamondToGoldUIViewManager>(true)[0].Show(true);

            m_camUI.clearFlags = CameraClearFlags.SolidColor;
            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    #endregion

    #region ���а�

    /// <summary>
    /// ���а�ϵͳ
    /// </summary>
    public bool IsRankingUILoaded = false;
    public void ShowRankingUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_RankingUI == null)
        {
            if (!IsRankingUILoaded)
            {
                IsRankingUILoaded = true;

                AssetCacheMgr.GetUIInstance("RankingUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_RankingUI = go as GameObject;
                        m_RankingUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_RankingUI.transform.localPosition = Vector3.zero;
                        m_RankingUI.transform.localScale = new Vector3(1, 1, 1);
                        m_RankingUI.AddComponent<RankingUIViewManager>();

                        if (CurrentUI != m_RankingUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_RankingUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.SolidColor;

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (act != null)
                            {
                                act();
                            }
                        }

                        CallWhenUILoad();
                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_RankingUI;
            ShowCurrentUI(true);

            m_camUI.clearFlags = CameraClearFlags.SolidColor;

            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    /// <summary>
    /// ���а�ϵͳ
    /// </summary>
    public bool IsRankingRewardUILoaded = false;
    public void ShowRankingRewardUI(Action action, bool isShow)
    {
        if (!UICanChange)
            return;
        if (m_RankingRewardUI == null)
        {
            if (!IsRankingRewardUILoaded)
            {
                IsRankingRewardUILoaded = true;

                AssetCacheMgr.GetUIInstance("RankingRewardUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_RankingRewardUI = go as GameObject;
                        m_RankingRewardUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_RankingRewardUI.transform.localPosition = new Vector3(5000, -8000, 0);
                        m_RankingRewardUI.transform.localScale = new Vector3(1, 1, 1);
                        m_RankingRewardUI.AddComponent<RankingRewardUIViewManager>().LoadResourceInsteadOfAwake(); ;

                        if (!isShow)
                        {
                            m_RankingRewardUI.SetActive(false);
                            return;
                        }
 
                        m_RankingRewardUI.SetActive(true);
                        ShowBillboardList(false);

                        if (action != null)
                            action();
                    }
                );
            }
        }
        else
        {
            if (!isShow)
            {
                m_RankingRewardUI.SetActive(false);
                return;
            }

            m_RankingRewardUI.SetActive(isShow);
            ShowBillboardList(false);

            if (action != null)
                action();
        }
    }

    #endregion

    #region ս����������ϵͳ��װ���Ƽ�ϵͳ

    /// <summary>
    /// ս����������ϵͳ
    /// </summary>
    public bool IsUpgradePowerUILoaded = false;
    public void ShowUpgradePowerUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_UpgradePowerUI == null)
        {
            if (!IsUpgradePowerUILoaded)
            {
                IsUpgradePowerUILoaded = true;

                CallWhenUILoad(0, false);
                AssetCacheMgr.GetUIInstance("UpgradePowerUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_UpgradePowerUI = go as GameObject;
                        m_UpgradePowerUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_UpgradePowerUI.transform.localPosition = Vector3.zero;
                        m_UpgradePowerUI.transform.localScale = new Vector3(1, 1, 1);
                        m_UpgradePowerUI.AddComponent<UpgradePowerUIViewManager>();

                        if (CurrentUI != m_UpgradePowerUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_UpgradePowerUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (act != null)
                            {
                                act();
                            }
                        }
                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_UpgradePowerUI;
            ShowCurrentUI(true);

            m_camUI.clearFlags = CameraClearFlags.Depth;

            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    /// <summary>
    /// װ���Ƽ�ϵͳ
    /// </summary>
    public bool IsEquipRecommendUILoaded = false;
    public void ShowEquipRecommendUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_EquipRecommendUI == null)
        {
            if (!IsEquipRecommendUILoaded)
            {
                IsEquipRecommendUILoaded = true;

                AssetCacheMgr.GetUIInstance("EquipRecommendUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_EquipRecommendUI = go as GameObject;
                        m_EquipRecommendUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_EquipRecommendUI.transform.localPosition = Vector3.zero;
                        m_EquipRecommendUI.transform.localScale = new Vector3(1, 1, 1);
                        m_EquipRecommendUI.AddComponent<EquipRecommendUIViewManager>();

                        if (CurrentUI != m_EquipRecommendUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_EquipRecommendUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (act != null)
                            {
                                act();
                            }
                        }
                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_EquipRecommendUI;
            ShowCurrentUI(true);

            m_camUI.clearFlags = CameraClearFlags.Depth;

            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    #endregion

    #region ����ϵͳ

    /// <summary>
    /// ����ϵͳ
    /// </summary>
    public bool IsSpriteUILoaded = false;
    public void ShowSpriteUI(Action act)
    {
        if (!UICanChange)
            return;

        // ��һ�μ�����Ҫ��Loading����OnEnableȥ�������Դ���ؿ���Loading
        if (MogoUIManager.Instance.m_SpriteUI == null)
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        if (m_SpriteUI == null)
        {
            if (!IsSpriteUILoaded)
            {
                IsSpriteUILoaded = true;

                AssetCacheMgr.GetUIInstance("SpriteUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_SpriteUI = go as GameObject;
                        m_SpriteUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_SpriteUI.transform.localPosition = Vector3.zero;
                        m_SpriteUI.transform.localScale = new Vector3(1, 1, 1);
                        m_SpriteUI.AddComponent<SpriteUIViewManager>();

                        if (CurrentUI != m_SpriteUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_SpriteUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (act != null)
                            {
                                act();
                            }
                        }
                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_SpriteUI;
            ShowCurrentUI(true);

            m_camUI.clearFlags = CameraClearFlags.Depth;

            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    #endregion

    #region (����PVP)ռ��

    /// <summary>
    /// ռ���������
    /// </summary>
    public bool IsOccupyTowerPassUILoaded = false;
    public void ShowOccupyTowerPassUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_OccupyTowerPassUI == null)
        {
            if (!IsOccupyTowerPassUILoaded)
            {
                IsOccupyTowerPassUILoaded = true;

                AssetCacheMgr.GetUIInstance("OccupyTowerPassUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_OccupyTowerPassUI = go as GameObject;
                        m_OccupyTowerPassUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_OccupyTowerPassUI.transform.localPosition = Vector3.zero;
                        m_OccupyTowerPassUI.transform.localScale = new Vector3(1, 1, 1);
                        m_OccupyTowerPassUI.AddComponent<OccupyTowerPassUIViewManager>();

                        if (CurrentUI != m_OccupyTowerPassUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_OccupyTowerPassUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            //if (MogoMainCamera.instance)
                            //    MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (act != null)
                            {
                                act();
                            }
                        }
                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_OccupyTowerPassUI;
            ShowCurrentUI(true);

            m_camUI.clearFlags = CameraClearFlags.Depth;

            //if (MogoMainCamera.instance)
            //    MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    /// <summary>
    /// ռ������
    /// </summary>
    public bool IsOccupyTowerUILoaded = false;
    public void ShowOccupyTowerUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_OccupyTowerUI == null)
        {
            if (!IsOccupyTowerUILoaded)
            {
                IsOccupyTowerUILoaded = true;

                AssetCacheMgr.GetUIInstance("OccupyTowerUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_OccupyTowerUI = go as GameObject;
                        m_OccupyTowerUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_OccupyTowerUI.transform.localPosition = Vector3.zero;
                        m_OccupyTowerUI.transform.localScale = new Vector3(1, 1, 1);
                        m_OccupyTowerUI.AddComponent<OccupyTowerUIViewManager>();

                        if (CurrentUI != m_OccupyTowerUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_OccupyTowerUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (act != null)
                            {
                                act();
                            }
                        }
                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_OccupyTowerUI;
            ShowCurrentUI(true);

            m_camUI.clearFlags = CameraClearFlags.Depth;

            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    #endregion

    #region �ȼ��������������ָ��

    /// <summary>
    /// �ȼ�������������
    /// </summary>
    public bool IsLevelNoEnoughUILoaded = false;
    public void ShowLevelNoEnoughUI(Action action, bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_LevelNoEnoughUI == null)
        {
            if (!IsLevelNoEnoughUILoaded)
            {
                IsLevelNoEnoughUILoaded = true;

                AssetCacheMgr.GetUIInstance("LevelNoEnoughUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_LevelNoEnoughUI = go as GameObject;
                        m_LevelNoEnoughUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_LevelNoEnoughUI.transform.localPosition = new Vector3(8000, -8000, 0);
                        m_LevelNoEnoughUI.transform.localScale = new Vector3(1, 1, 1);
                        m_LevelNoEnoughUI.AddComponent<LevelNoEnoughUIViewManager>();

                        if (!isShow)
                        {
                            m_LevelNoEnoughUI.SetActive(false);
                            return;
                        }

                        ShowCurrentUI(false);
                        CurrentUI = m_LevelNoEnoughUI;
                        ShowCurrentUI(true);

                        m_camUI.clearFlags = CameraClearFlags.Depth;
                        //if (MogoMainCamera.instance)
                        //    MogoMainCamera.instance.SetActive(false);

                        ShowBillboardList(false);

                        if (action != null)
                        {
                            action();
                        }
                    }
                );
            }
        }
        else
        {
            if (!isShow)
            {
                m_LevelNoEnoughUI.SetActive(false);
                return;
            }


            ShowCurrentUI(false);
            CurrentUI = m_LevelNoEnoughUI;
            ShowCurrentUI(isShow);

            m_camUI.clearFlags = CameraClearFlags.Depth;
            //if (MogoMainCamera.instance)
            //    MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (action != null)
            {
                action();
            }
        }
    }

    /// <summary>
    /// ����������������
    /// </summary>
    public bool IsEnergyNoEnoughUILoaded = false;
    public void ShowEnergyNoEnoughUI(Action action, bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_EnergyNoEnoughUI == null)
        {
            if (!IsEnergyNoEnoughUILoaded)
            {
                IsEnergyNoEnoughUILoaded = true;

                AssetCacheMgr.GetUIInstance("EnergyNoEnoughUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_EnergyNoEnoughUI = go as GameObject;
                        m_EnergyNoEnoughUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_EnergyNoEnoughUI.transform.localPosition = new Vector3(8000, -8000, 0);
                        m_EnergyNoEnoughUI.transform.localScale = new Vector3(1, 1, 1);
                        m_EnergyNoEnoughUI.AddComponent<EnergyNoEnoughUIViewManager>();

                        if (!isShow)
                        {
                            m_EnergyNoEnoughUI.SetActive(false);
                            return;
                        }

                        ShowCurrentUI(false);
                        CurrentUI = m_EnergyNoEnoughUI;
                        ShowCurrentUI(true);

                        m_camUI.clearFlags = CameraClearFlags.Depth;
                        //if (MogoMainCamera.instance)
                        //    MogoMainCamera.instance.SetActive(false);

                        ShowBillboardList(false);

                        if (action != null)
                        {
                            action();
                        }
                    }
                );
            }
        }
        else
        {
            if (!isShow)
            {
                m_EnergyNoEnoughUI.SetActive(false);
                return;
            }

            ShowCurrentUI(false);
            CurrentUI = m_EnergyNoEnoughUI;
            ShowCurrentUI(isShow);

            m_camUI.clearFlags = CameraClearFlags.Depth;
            //if (MogoMainCamera.instance)
            //    MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (action != null)
            {
                action();
            }
        }
    }

    #endregion

    #region ������������

    /// <summary>
    /// ������������
    /// </summary>
    public bool IsIDragonMatchUILoaded = false;
    public void ShowDragonMatchUI(Action act = null, bool isShow = true)
    {
        if (!UICanChange)
            return;

        if (m_DragonMatchUI == null)
        {
            if (!IsIDragonMatchUILoaded)
            {
                IsIDragonMatchUILoaded = true;

                AssetCacheMgr.GetUIInstance("DragonMatchUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_DragonMatchUI = go as GameObject;
                        m_DragonMatchUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_DragonMatchUI.transform.localPosition = new Vector3(0, 0, 0);
                        m_DragonMatchUI.transform.localScale = new Vector3(1, 1, 1);
                        m_DragonMatchUI.AddComponent<DragonMatchUIViewManager>();

                        if (!isShow && CurrentUI != m_DragonMatchUI)
                        {
                            m_DragonMatchUI.SetActive(false);
                            return;
                        }

                        if (CurrentUI != m_DragonMatchUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_DragonMatchUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.SolidColor;

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (act != null)
                            {
                                act();
                            }
                        }

                        //CallWhenUILoad();
                    }
                );
            }
        }
        else
        {
            if (!isShow && CurrentUI != m_DragonMatchUI)
            {
                m_DragonMatchUI.SetActive(false);
                return;
            }

            if (CurrentUI != m_DragonMatchUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_DragonMatchUI;
                ShowCurrentUI(true);

                m_camUI.clearFlags = CameraClearFlags.SolidColor;

                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(false);

                ShowBillboardList(false);
            }

            if (act != null)
            {
                act();
            }
        }
    }

    /// <summary>
    /// ����������¼����
    /// </summary>
    public bool IsDragonMatchRecordUILoaded = false;
    public void ShowDragonMatchRecordUI(Action act, bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_DragonMatchRecordUI == null)
        {
            if (!IsDragonMatchRecordUILoaded)
            {
                IsDragonMatchRecordUILoaded = true;

                AssetCacheMgr.GetUIInstance("DragonMatchRecordUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_DragonMatchRecordUI = go as GameObject;
                        m_DragonMatchRecordUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_DragonMatchRecordUI.transform.localPosition = new Vector3(3000, 0, 0);
                        m_DragonMatchRecordUI.transform.localScale = new Vector3(1, 1, 1);
                        DragonMatchRecordUIViewManager view = m_DragonMatchRecordUI.AddComponent<DragonMatchRecordUIViewManager>();

                        if (!isShow)
                        {
                            m_DragonMatchRecordUI.SetActive(false);
                            return;
                        }

                        view.LoadResourceInsteadOfAwake();
                        //CurrentUI.SetActive(false);
                        CurrentUI = m_DragonMatchRecordUI;
                        ShowCurrentUI(true);

                        //m_camUI.clearFlags = CameraClearFlags.SolidColor;

                        //if (MogoMainCamera.instance)
                        //    MogoMainCamera.instance.SetActive(false);

                        ShowBillboardList(false);

                        if (act != null)
                        {
                            act();
                        }
                    }
                );
            }
        }
        else
        {
            if (!isShow)
            {
                m_DragonMatchRecordUI.SetActive(false);
                return;
            }

            //CurrentUI.SetActive(false);
            CurrentUI = m_DragonMatchRecordUI;
            ShowCurrentUI(isShow);

            //m_camUI.clearFlags = CameraClearFlags.SolidColor;

            //if (MogoMainCamera.instance)
            //    MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    /// <summary>
    /// ����ѡ�����
    /// </summary>
    public bool IsChooseDragonUILoaded = false;
    public void ShowChooseDragonUI(Action act, bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_ChooseDragonUI == null)
        {
            if (!IsChooseDragonUILoaded)
            {
                IsChooseDragonUILoaded = true;

                AssetCacheMgr.GetUIInstance("ChooseDragonUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_ChooseDragonUI = go as GameObject;
                        m_ChooseDragonUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_ChooseDragonUI.transform.localPosition = new Vector3(5000, 0, 0);
                        m_ChooseDragonUI.transform.localScale = new Vector3(1, 1, 1);
                        ChooseDragonUIViewManager view = m_ChooseDragonUI.AddComponent<ChooseDragonUIViewManager>();

                        if (!isShow)
                        {
                            m_ChooseDragonUI.SetActive(false);
                            return;
                        }

                        view.LoadResourceInsteadOfAwake();
                        //CurrentUI.SetActive(false);
                        CurrentUI = m_ChooseDragonUI;
                        ShowCurrentUI(true);

                        //m_camUI.clearFlags = CameraClearFlags.SolidColor; 
                        //if (MogoMainCamera.instance)
                        //    MogoMainCamera.instance.SetActive(false);

                        ShowBillboardList(false);

                        if (act != null)
                        {
                            act();
                        }
                    }
                );
            }
        }
        else
        {
            if (!isShow)
            {
                m_ChooseDragonUI.SetActive(false);
                return;
            }

            //CurrentUI.SetActive(false);
            CurrentUI = m_ChooseDragonUI;
            ShowCurrentUI(isShow);

            //m_camUI.clearFlags = CameraClearFlags.SolidColor;
            //if (MogoMainCamera.instance)
            //    MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (act != null)
            {
                act();
            }
        }
    }

    #endregion

    #region ȷ�Ͽ�-����ѡ������ʾ

    public bool IsOKCancelTipUILoaded = false;
    public void ShowOKCancelTipUI(Action action, bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_OKCancelTipUI == null)
        {
            if (!IsOKCancelTipUILoaded)
            {
                IsOKCancelTipUILoaded = true;

                AssetCacheMgr.GetUIInstance("OKCancelTipUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_OKCancelTipUI = go as GameObject;
                        m_OKCancelTipUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_OKCancelTipUI.transform.localPosition = new Vector3(5000, 0, 0);
                        m_OKCancelTipUI.transform.localScale = new Vector3(1, 1, 1);
                        OKCancelTipUIViewManager view = m_OKCancelTipUI.AddComponent<OKCancelTipUIViewManager>();

                        if (!isShow)
                        {
                            m_OKCancelTipUI.SetActive(false);
                            return;
                        }

                        view.LoadResourceInsteadOfAwake();
                        m_OKCancelTipUI.SetActive(true);

                        if (action != null)
                            action();
                    }
                );
            }
        }
        else
        {
            if (!isShow)
            {
                m_OKCancelTipUI.SetActive(false);
                return;
            }

            m_OKCancelTipUI.SetActive(isShow);

            if (action != null)
                action();
        }
    }

    #endregion

    #region ������

    /// <summary>
    /// �¾���������
    /// </summary>
    public bool IsNewArenaUILoaded = false;
    public void ShowNewArenaUI(Action action)
    {
        if (!UICanChange)
            return;

        // ��һ�μ�����Ҫ��Loading����OnEnableȥ�������Դ���ؿ���Loading
        if (MogoUIManager.Instance.m_NewArenaUI == null)
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        if (m_NewArenaUI == null)
        {
            if (!IsNewArenaUILoaded)
            {
                IsNewArenaUILoaded = true;

                AssetCacheMgr.GetUIInstance("NewArenaUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_NewArenaUI = go as GameObject;
                        m_NewArenaUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_NewArenaUI.transform.localPosition = Vector3.zero;
                        m_NewArenaUI.transform.localScale = new Vector3(1, 1, 1);
                        m_NewArenaUI.AddComponent<NewArenaUIViewManager>();

                        if (CurrentUI != m_NewArenaUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_NewArenaUI;
                            ShowCurrentUI(true);

                            m_camUI.clearFlags = CameraClearFlags.SolidColor;

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            ShowBillboardList(false);

                            if (action != null)
                            {
                                action();
                            }
                        }
                    }
                );
            }
        }
        else
        {
            ShowCurrentUI(false);
            CurrentUI = m_NewArenaUI;
            ShowCurrentUI(true);

            m_camUI.clearFlags = CameraClearFlags.SolidColor;

            if (MogoMainCamera.instance)
                MogoMainCamera.instance.SetActive(false);

            ShowBillboardList(false);

            if (action != null)
            {
                action();
            }
        }
    }

    #endregion

    public bool IsTowerUILoaded = false;
    public void ShowUI(bool IsShow)
    {
        if (!UICanChange)
            return;
        //m_CurrentUI.SetActive(IsShow);
        GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0].enabled = IsShow;
        GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<UICamera>(true)[0].enabled = IsShow;
        MogoFXManager.Instance.SetUIFXCameraEnable(IsShow);
    }
    public void ShowSceneUI(bool IsShow)
    {
        if (!UICanChange)
            return;
        if (MogoMainCamera.instance)
        {
            MogoMainCamera.instance.SetActive(IsShow);
        }
    }
    public void ShowSubtitle(string text, float fontSize, float time)
    {
        if (!UICanChange)
            return;
        ShowCurrentUI(false);
        ShowSceneUI(false);
        ShowBillboardList(false);
        MogoUIManager.Instance.ShowUI(true);
        m_subtitleUI = new GameObject();
        m_subtitleUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
        m_subtitleUI.transform.localScale = new Vector3(fontSize, fontSize, 1);
        m_subtitleUI.transform.localPosition = Vector3.zero;
        m_subtitleUI.name = "subtitleUI";
        m_subtitleText = m_subtitleUI.AddComponent<UILabel>();
        m_subtitleText.font = BillboardViewManager.Instance.GetBattleBillboardFont();
        m_subtitleText.pivot = UIWidget.Pivot.Center;
        m_subtitleClip = 1;
        m_intv = (uint)(time / text.Length);
        m_subtitleString = text;
        TimerHeap.AddTimer(m_intv, 0, subtitleTick);
        m_mainUICamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponent<Camera>();
        m_mainUICamera.clearFlags = CameraClearFlags.SolidColor;
        m_mainUICamera.backgroundColor = new UnityEngine.Color(0, 0, 0);
    }
    private void ResetNormal()
    {
        if (!UICanChange)
            return;
        LoggerHelper.Debug("ResetNormal");
        ShowCurrentUI(true);
        ShowSceneUI(true);
        ShowBillboardList(true);
        MogoUIManager.Instance.ShowUI(false);
        GameObject.Destroy(m_subtitleUI);
        m_mainUICamera.clearFlags = CameraClearFlags.Depth;
    }
    private void subtitleTick()
    {
        if (!UICanChange)
            return;
        if (m_subtitleClip > m_subtitleString.Length)
        {
            LoggerHelper.Debug("deltimer");
            TimerHeap.AddTimer(3000, 0, ResetNormal);
        }
        else
        {
            m_subtitleText.text = m_subtitleString.Substring(0, m_subtitleClip);
            m_subtitleClip++;
            TimerHeap.AddTimer(m_intv, 0, subtitleTick);
        }
    }
    /// <summary>
    /// ��дUIģʽ
    /// </summary>
    private Dictionary<int, GameObject> m_UIHashMap = new Dictionary<int, GameObject>();
    private const string m_suffix = ".prefab";
    private const string m_viewsuffix = "ViewManager";
    private const string m_logicsuffix = "LogicManager";
    private bool isWindowLoading = false;
    public bool IsWindowOpen(int id)
    {
        if (!m_UIHashMap.ContainsKey(id))
        {
            return false;
        }
        else
        {
            return CurrentUI == m_UIHashMap[id];
        }
    }
    private void _openUI(int id, Action callback, bool isDialog)
    {
        if (CurrentUI != m_UIHashMap[id])
        {
            if (isDialog)
            {
                m_UIHashMap[id].SetActive(true);
            }
            else
            {
                ShowCurrentUI(false);
                CurrentUI = m_UIHashMap[id];
                ShowCurrentUI(true);
            }
            m_camUI.clearFlags = CameraClearFlags.Depth;
            ShowBillboardList(false);
        }
        if (callback != null)
        {
            callback();
        }
        isWindowLoading = false;
    }
    public void OpenWindow(int id, Action cb = null, Transform parentTrans = null, bool isDialog = false)
    {
        if (MogoUIManager.Instance.UICanChange == false)
            return;
        if (!isWindowLoading)
        {
            isWindowLoading = true;
            if (m_UIHashMap.ContainsKey(id))
            {
                _openUI(id, cb, isDialog);
            }
            else
            {
                AssetCacheMgr.GetUIInstance(GUIConfigData.dataMap[id].prefab + m_suffix,
                    (prefab, guid, go) =>
                    {
                        GameObject obj = go as GameObject;
                        if (parentTrans == null)
                        {
                            parentTrans = GameObject.Find("MogoMainUIPanel").transform;
                        }
                        obj.transform.parent = parentTrans;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localScale = Vector3.one;
                        obj.AddComponent(System.Type.GetType(GUIConfigData.dataMap[id].prefab + m_viewsuffix));
                        m_UIHashMap.Add(id, obj);
                        _openUI(id, cb, isDialog);
                    }
                );
            }
        }
        
    }

    #region �̳ǽ���

    private bool IsMarketUILoaded = false;
    private void ShowMarketUI(Action action = null)
    {
        if (!UICanChange)
            return;
        if (m_MarketUI == null)
        {
            if (!IsMarketUILoaded)
            {
                IsMarketUILoaded = true;

                AssetCacheMgr.GetUIInstance("MarketUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_MarketUI = go as GameObject;
                        m_MarketUI.AddComponent<MarketView>();
                        MarketView.Instance.AddToParent(m_myTransform, m_myTransform.rotation);
                        MarketView.Instance.Open();                     

                        TimerHeap.AddTimer<string>(300, 0, EventDispatcher.TriggerEvent, MarketEvent.DownloadMarket);
                        m_camUI.clearFlags = CameraClearFlags.SolidColor;
                        ShowCurrentUI(false);
                        CurrentUI = m_MarketUI;
                        if (MogoMainCamera.instance)
                            MogoMainCamera.instance.SetActive(false);

                        ShowBillboardList(false);
                        CallWhenUILoad();

                        if (action != null)
                            action();
                    }
                );
            }
            return;
        }

        MarketView.Instance.Open();
        TimerHeap.AddTimer<string>(300, 0, EventDispatcher.TriggerEvent, MarketEvent.DownloadMarket);
        m_camUI.clearFlags = CameraClearFlags.SolidColor;
        ShowCurrentUI(false);
        CurrentUI = m_MarketUI;
        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(false);

        ShowBillboardList(false);

        if (action != null)
            action();
    }

    public void CloseMarketUI()
    {
        if (!UICanChange)
            return;
        if (m_MarketUI != null)
        {
            MarketView.Instance.Close();
            MogoUIManager.Instance.ShowMogoNormalMainUI();
            CurrentUI = m_NormalMainUI;
            ShowCurrentUI(true);
            
            
            m_camUI.clearFlags = CameraClearFlags.Depth;
            if (MogoMainCamera.instance)
            {
                MogoMainCamera.instance.SetActive(true);
            }
            ShowBillboardList(true);
        }
    }

    public void SwitchToMarket(MarketUITab tab = MarketUITab.HotTab)
    {
        ShowMarketUI(() =>
            {
                if (MarketView.Instance != null)
                {
                    MogoWorld.thePlayer.marketManager.CurrentMarketUITab = tab;
                    MarketView.Instance.SwitchTab(MogoWorld.thePlayer.marketManager.CurrentMarketUITab);                                 
                }
            });
    }

    #endregion

    public bool IsNewLoginUILoaded = false;

    private void TruelyShowNewLoginUI(Action cb)
    {
        if (!UICanChange)
            return;
        if (m_NewLoginUI == null)
        {
            if (!IsNewLoginUILoaded)
            {
                IsNewLoginUILoaded = true;

                CallWhenUILoad(0, false);
                AssetCacheMgr.GetUIInstance("NewLoginUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_NewLoginUI = go as GameObject;
                        m_NewLoginUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_NewLoginUI.transform.localPosition = Vector3.zero;
                        m_NewLoginUI.transform.localScale = new Vector3(1, 1, 1);
                        m_NewLoginUI.AddComponent<NewLoginUIViewManager>();

                        if (CurrentUI != m_NewLoginUI)
                        {
                            if (CurrentUI != null)
                            {
                                ShowCurrentUI(false);
                            }
                            CurrentUI = m_NewLoginUI;
                            ShowCurrentUI(true);

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);


                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            ShowBillboardList(false);

                            if (cb != null)
                                cb();

                        }


                    }
                );
            }
        }
        else
        {
            if (CurrentUI != m_NewLoginUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_NewLoginUI;
                ShowCurrentUI(true);

                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(false);

                ShowBillboardList(false);


                m_camUI.clearFlags = CameraClearFlags.Depth;

                if (cb != null)
                    cb();

            }
        }
    }

    public void ShowNewLoginUI(Action cb)
    {
        if (!UICanChange)
            return;
        MogoUIQueue.Instance.PushOne(() => { TruelyShowNewLoginUI(cb); });
    }

    public void SwitchSkillsContractUI()
    {
        if (!UICanChange)
            return;
        if (m_AssistantUI != CurrentUI)
        {
            ShowMogoAssistantUI();
        }

        m_AssistantUI.transform.Find("TopRight/AssistantUIDialogList").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);
    }

    public void SwitchElementHintmarkUI()
    {
        if (!UICanChange)
            return;
        if (m_AssistantUI != CurrentUI)
        {
            ShowMogoAssistantUI();
        }

        m_AssistantUI.transform.Find("TopRight/AssistantUIDialogList").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);
    }

    #region ����ϵͳ

    /// <summary>
    /// װ��ǿ��
    /// </summary>
    public Action StrenthUILoaded;
    public bool IsStrenthUILoaded = false;
    public void SwitchStrenthUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_EquipmentUI != CurrentUI)
            ShowMogoEquipmentUI(false);

        if (m_StrenthUI == null)
        {
            if (!IsStrenthUILoaded)
            {
                IsNewLoginUILoaded = true;


                CallWhenUILoad(0, false);

                PreLoadResource("StrenthenDialogIconGrid.prefab", (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("StrenthUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_StrenthUI = go as GameObject;
                            m_StrenthUI.transform.parent = m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIDialogList/StrenthenDialog").transform;
                            m_StrenthUI.transform.localPosition = Vector3.zero;
                            m_StrenthUI.transform.localEulerAngles = Vector3.zero;
                            m_StrenthUI.transform.localScale = new Vector3(1, 1, 1);

                            if (act != null)
                            {
                                StrenthUILoaded = act;
                            }

                            m_StrenthUI.AddComponent<StrenthenUIViewManager>();

                            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);



                            if (WaitingWidgetName == "StrenthenDialogStrenth")
                            {
                                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                            }

                        }
                    );
                });
            }
        }
        else
        {
            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);

            if (act != null)
            {
                act();
            }

            if (WaitingWidgetName == "StrenthenDialogStrenth")
            {
                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
            }
        }
    }

    public void SwitchStrenthUI()
    {
        if (!UICanChange)
            return;
        if (m_EquipmentUI != CurrentUI)
            ShowMogoEquipmentUI(false);

        if (m_StrenthUI == null)
        {
            if (!IsStrenthUILoaded)
            {
                IsStrenthUILoaded = true;
                CallWhenUILoad(0, false);

                PreLoadResource("StrenthenDialogIconGrid.prefab", (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("StrenthUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_StrenthUI = go as GameObject;
                            m_StrenthUI.transform.parent = m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIDialogList/StrenthenDialog").transform;
                            m_StrenthUI.transform.localPosition = Vector3.zero;
                            m_StrenthUI.transform.localEulerAngles = Vector3.zero;
                            m_StrenthUI.transform.localScale = new Vector3(1, 1, 1);
                            m_StrenthUI.AddComponent<StrenthenUIViewManager>();

                            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);

                            if (WaitingWidgetName == "StrenthenDialogStrenth" ||
                                WaitingWidgetName == "StrenthenIcon")
                            {
                                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                            }

                        }
                    );
                });
            }
        }
        else
        {
            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);

            if (WaitingWidgetName == "StrenthenDialogStrenth")
            {
                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
            }

            EventDispatcher.TriggerEvent(BodyEnhanceManager.ON_SHOW);
        }
    }

    /// <summary>
    /// װ����Ƕ
    /// </summary>
    public Action InsetUILoaded;
    public bool IsInsetUILoaded = false;
    public void SwitchInsetUI(Action act)
    {
        if (!UICanChange)
            return;

        if (m_EquipmentUI != CurrentUI)
            ShowMogoEquipmentUI(false);


        if (m_InsetUI == null)
        {
            if (!IsInsetUILoaded)
            {
                IsInsetUILoaded = true;


                CallWhenUILoad(0, false);

                string[] resName = new string[] { "StrenthenDialogIconGrid.prefab", "InsetDialogPackageGrid.prefab" };

                PreLoadResources(resName, (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("InsetUI.prefab",
                        (prefab, guid, go) =>
                        {
                            if (act != null)
                            {
                                //Debug.LogError("act != null");
                                InsetUILoaded = act;
                                //act();
                            }

                            m_InsetUI = go as GameObject;
                            m_InsetUI.transform.parent = m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIDialogList/InsetDialog").transform;
                            m_InsetUI.transform.localPosition = Vector3.zero;
                            m_InsetUI.transform.localEulerAngles = Vector3.zero;
                            m_InsetUI.transform.localScale = new Vector3(1, 1, 1);
                            m_InsetUI.AddComponent<InsetUIViewManager>();

                            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);


                            if (WaitingWidgetName == "InsetIconListIcon0")
                            {
                                TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });

                            }

                        }
                    );
                });
            }
        }
        else
        {
            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);

            EventDispatcher.TriggerEvent(InsetManager.ON_INSET_SHOW);

            if (act != null)
            {
                act();
            }

            if (WaitingWidgetName == "InsetIconListIcon0")
            {
                TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });

            }
        }
    }

    public void SwitchInsetUI()
    {
        if (!UICanChange)
            return;

        if (m_EquipmentUI != CurrentUI)
            ShowMogoEquipmentUI(false);



        if (m_InsetUI == null)
        {
            CallWhenUILoad(0, false);

            string[] resName = new string[] { "StrenthenDialogIconGrid.prefab", "InsetDialogPackageGrid.prefab" };

            PreLoadResources(resName, (obj) =>
            {
                AssetCacheMgr.GetUIInstance("InsetUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_InsetUI = go as GameObject;
                        m_InsetUI.transform.parent = m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIDialogList/InsetDialog").transform;
                        m_InsetUI.transform.localPosition = Vector3.zero;
                        m_InsetUI.transform.localEulerAngles = Vector3.zero;
                        m_InsetUI.transform.localScale = new Vector3(1, 1, 1);
                        m_InsetUI.AddComponent<InsetUIViewManager>();

                        m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);

                        if (WaitingWidgetName == "InsetIconListIcon0")
                        {
                            TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });

                        }

                    }
                );
            });
        }
        else
        {
            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);

            EventDispatcher.TriggerEvent(InsetManager.ON_INSET_SHOW);

            if (WaitingWidgetName == "InsetIconListIcon0")
            {
                TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });

            }
        }
    }

    /// <summary>
    /// װ���ϳ�
    /// </summary>
    public void SwitchComposeUI()
    {
        if (!UICanChange)
            return;
        if (m_EquipmentUI != CurrentUI)
            ShowMogoEquipmentUI(false);

        if (m_ComposeUI == null)
        {
            CallWhenUILoad(0, false);

            string[] resName = new string[] { "EquipmentUIComposeIconListGrid.prefab", "EquipmentUIComposeIconListGridChild.prefab" }; ;
            PreLoadResources(resName, (obj) =>
            {
                AssetCacheMgr.GetUIInstance("ComposeUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_ComposeUI = go as GameObject;
                        m_ComposeUI.transform.parent = m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIDialogList/ComposeDialog").transform;
                        m_ComposeUI.transform.localPosition = Vector3.zero;
                        m_ComposeUI.transform.localEulerAngles = Vector3.zero;
                        m_ComposeUI.transform.localScale = new Vector3(1, 1, 1);
                        m_ComposeUI.AddComponent<ComposeUIViewManager>();

                        m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);


                        EventDispatcher.TriggerEvent(ComposeManager.ON_COMPOSE_SHOW);

                        if (WaitingWidgetName == "ComposeDialogCompose" || WaitingWidgetName == "ComposeDialogBuy")
                        {
                            EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                        }


                    }
                );
            });
        }
        else
        {

            EventDispatcher.TriggerEvent(ComposeManager.ON_COMPOSE_SHOW);
            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);

            if (WaitingWidgetName == "ComposeDialogCompose" || WaitingWidgetName == "ComposeDialogBuy")
            {
                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
            }
        }
    }

    public Action ComposeUILoaded;
    public bool IsComposeUILoaded = false;
    public void SwitchComposeUI(Action act, bool isSwitch = true)
    {
        if (!UICanChange)
            return;
        if (m_EquipmentUI != CurrentUI)
            ShowMogoEquipmentUI(false);

        if (m_ComposeUI == null)
        {
            if (!IsComposeUILoaded)
            {
                IsComposeUILoaded = true;


                CallWhenUILoad(0, false);

                string[] resName = new string[] { "EquipmentUIComposeIconListGrid.prefab", "EquipmentUIComposeIconListGridChild.prefab" }; ;
                PreLoadResources(resName, (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("ComposeUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_ComposeUI = go as GameObject;
                            m_ComposeUI.transform.parent = m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIDialogList/ComposeDialog").transform;
                            m_ComposeUI.transform.localPosition = Vector3.zero;
                            m_ComposeUI.transform.localEulerAngles = Vector3.zero;
                            m_ComposeUI.transform.localScale = new Vector3(1, 1, 1);
                            m_ComposeUI.AddComponent<ComposeUIViewManager>();

                            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);


                            if (isSwitch)
                            {
                                EventDispatcher.TriggerEvent(ComposeManager.ON_COMPOSE_SHOW);
                            }
                            if (act != null)
                            {

                                ComposeUILoaded = act;
                            }

                            if (WaitingWidgetName == "ComposeDialogCompose" || WaitingWidgetName == "ComposeDialogBuy")
                            {
                                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                            }

                        }
                    );
                });
            }
        }
        else
        {
            if (WaitingWidgetName == "ComposeDialogCompose" || WaitingWidgetName == "ComposeDialogBuy")
            {
                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
            }

            if (isSwitch)
            {
                EventDispatcher.TriggerEvent(ComposeManager.ON_COMPOSE_SHOW);
            }
            else
            {
                if (ComposeUIViewManager.Instance.m_bIsAllLoadDone)
                {
                    if (act != null)
                        act();
                }
                else
                {
                    ComposeUILoaded = act;
                }
                m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);


                return;
            }

            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);


            if (act != null)
            {
                act();
            }
        }
    }

    /// <summary>
    /// װ���ֽ�
    /// </summary>
    public void SwitchDecomposeUI()
    {
        if (!UICanChange)
            return;
        if (m_EquipmentUI != CurrentUI)
            ShowMogoEquipmentUI(false);

        if (m_DecomposeUI == null)
        {
            CallWhenUILoad(0, false);

            string[] resName = new string[] { "DecomposeDialogPackageGrid.prefab", "DecomposeDialogIconGrid.prefab" };

            PreLoadResources(resName, (obj) =>
            {
                AssetCacheMgr.GetUIInstance("DecomposeUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_DecomposeUI = go as GameObject;
                        m_DecomposeUI.transform.parent = m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIDialogList/DecomposeDialog").transform;
                        m_DecomposeUI.transform.localPosition = Vector3.zero;
                        m_DecomposeUI.transform.localEulerAngles = Vector3.zero;
                        m_DecomposeUI.transform.localScale = new Vector3(1, 1, 1);
                        m_DecomposeUI.AddComponent<DecomposeUIViewManager>();

                        m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);

                        if (WaitingWidgetName == "DecomposeDialogPackageGridCheckBG0")
                        {
                            TimerHeap.AddTimer(500, 0, () =>
                            {
                                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                            });
                        }
                    }
                );
            });
        }
        else
        {
            EventDispatcher.TriggerEvent(DecomposeManager.ON_DECOMPOSE_SHOW);
            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);


        }
    }

    public Action DecomposeUILoaded;    
    public bool IsDecomposeUILoaded = false;
    public void SwitchDecomposeUI(Action act)
    {
        if (!UICanChange)
            return;
        if (m_EquipmentUI != CurrentUI)
            ShowMogoEquipmentUI(false);

        if (m_DecomposeUI == null)
        {
            if (!IsDecomposeUILoaded)
            {
                IsDecomposeUILoaded = true;


                CallWhenUILoad(0, false);

                string[] resName = new string[] { "DecomposeDialogPackageGrid.prefab", "DecomposeDialogIconGrid.prefab" };

                PreLoadResources(resName, (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("DecomposeUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_DecomposeUI = go as GameObject;
                            m_DecomposeUI.transform.parent = m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet/EquipmentUIDialogList/DecomposeDialog").transform;
                            m_DecomposeUI.transform.localPosition = Vector3.zero;
                            m_DecomposeUI.transform.localEulerAngles = Vector3.zero;
                            m_DecomposeUI.transform.localScale = new Vector3(1, 1, 1);
                            m_DecomposeUI.AddComponent<DecomposeUIViewManager>();

                            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);

                            if (act != null)
                            {
                                DecomposeUILoaded = act;
                            }

                        }
                    );
                });
            }
        }
        else
        {
            EventDispatcher.TriggerEvent(DecomposeManager.ON_DECOMPOSE_SHOW);
            m_EquipmentUI.transform.Find("EquipmentUIDialogList/EquipmentUIDialogListSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);

            if (act != null)
            {
                act();
            }
        }
    }

    #endregion

    #region MenuUI

    /// <summary>
    /// ����
    /// </summary>
    public void SwitchPackageUI()
    {
        if (!UICanChange)
            return;
        if (m_MenuUI != CurrentUI)
            ShowMogoMenuUI();
        m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);

    }

    /// <summary>
    /// �����Ϣ
    /// </summary>
    public void SwitchPlayerUI()
    {
        if (!UICanChange)
            return;
        if (m_MenuUI != CurrentUI)
            ShowMogoMenuUI();
        m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);
    }

    /// <summary>
    /// ����
    /// </summary>
    public bool IsSkillUILoaded = false;
    public void SwitchSkillUI(Action act = null)
    {
        if (!UICanChange)
            return;
        if (m_MenuUI != CurrentUI)
        {
            ShowMogoMenuUI(() =>
            {
                if (m_SkillUI == null)
                {
                    if (!IsSkillUILoaded)
                    {
                        IsSkillUILoaded = true;

                        CallWhenUILoad(0, false);


                        AssetCacheMgr.GetUIInstance("SkillUI.prefab",
                  (prefab, guid, go) =>
                  {
                      m_SkillUI = go as GameObject;
                      m_SkillUI.transform.parent = m_MenuUI.transform.Find("PropSheet/DialogList/SkillDialog").transform;
                      m_SkillUI.transform.localPosition = Vector3.zero;
                      m_SkillUI.transform.localEulerAngles = Vector3.zero;
                      m_SkillUI.transform.localScale = new Vector3(1, 1, 1);
                      m_SkillUI.AddComponent<SkillUIViewManager>();

                      m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);
                      EventDispatcher.TriggerEvent(Events.SpellEvent.OpenView);

                      if (MogoUIManager.Instance.WaitingWidgetName == "SkillIcon0" ||
                          MogoUIManager.Instance.WaitingWidgetName == "SkillIcon1" ||
                          MogoUIManager.Instance.WaitingWidgetName == "SkillIcon2" ||
                          MogoUIManager.Instance.WaitingWidgetName == "SkillIcon3")
                      {
                          TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
                      }

                      MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007015));
                      if (act != null)
                      {
                          act();
                      }
                      MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                  }
              );



                    }
                }
                else
                {
                    m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);
                    EventDispatcher.TriggerEvent(Events.SpellEvent.OpenView);
                    if (MogoUIManager.Instance.WaitingWidgetName == "SkillIcon0" ||
                                   MogoUIManager.Instance.WaitingWidgetName == "SkillIcon1" ||
                                   MogoUIManager.Instance.WaitingWidgetName == "SkillIcon2" ||
                                   MogoUIManager.Instance.WaitingWidgetName == "SkillIcon3")
                    {
                        TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
                    }

                    MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007015));
                    if (act != null)
                    {
                        act();
                    }
                }
            });
        }
        else
        {

            if (m_SkillUI == null)
            {
                if (!IsSkillUILoaded)
                {
                    IsSkillUILoaded = true;

                    CallWhenUILoad(0, false);


                    AssetCacheMgr.GetUIInstance("SkillUI.prefab",
              (prefab, guid, go) =>
              {
                  m_SkillUI = go as GameObject;
                  m_SkillUI.transform.parent = m_MenuUI.transform.Find("PropSheet/DialogList/SkillDialog").transform;
                  m_SkillUI.transform.localPosition = Vector3.zero;
                  m_SkillUI.transform.localEulerAngles = Vector3.zero;
                  m_SkillUI.transform.localScale = new Vector3(1, 1, 1);
                  m_SkillUI.AddComponent<SkillUIViewManager>();

                  m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);
                  EventDispatcher.TriggerEvent(Events.SpellEvent.OpenView);

                  if (MogoUIManager.Instance.WaitingWidgetName == "SkillIcon0" ||
                      MogoUIManager.Instance.WaitingWidgetName == "SkillIcon1" ||
                      MogoUIManager.Instance.WaitingWidgetName == "SkillIcon2" ||
                      MogoUIManager.Instance.WaitingWidgetName == "SkillIcon3")
                  {
                      TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
                  }

                  MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007015));
                  if (act != null)
                  {
                      act();
                  }

                  MogoGlobleUIManager.Instance.ShowWaitingTip(false);

              }
          );



                }
            }
            else
            {
                m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);
                EventDispatcher.TriggerEvent(Events.SpellEvent.OpenView);
                if (MogoUIManager.Instance.WaitingWidgetName == "SkillIcon0" ||
                               MogoUIManager.Instance.WaitingWidgetName == "SkillIcon1" ||
                               MogoUIManager.Instance.WaitingWidgetName == "SkillIcon2" ||
                               MogoUIManager.Instance.WaitingWidgetName == "SkillIcon3")
                {
                    TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
                }

                MenuUIViewManager.Instance.SetDialogTitleText(LanguageData.GetContent(1007015));
                if (act != null)
                {
                    act();
                }
            }
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    bool isTongUILoaded = false;
    public void SwitchTongUI()
    {
        if (!UICanChange)
            return;
        if (m_MenuUI != CurrentUI)
            ShowMogoMenuUI();
        m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);

        Transform tranTongNoOpen = m_MenuUI.transform.Find("PropSheet/DialogList/TongDialog/GOTongNoOpen");
        if (!ISTONGUIOPENED)
        {
            if (tranTongNoOpen != null)
                tranTongNoOpen.gameObject.SetActive(true);
            return;
        }
        else
        {
            if (tranTongNoOpen != null)
                tranTongNoOpen.gameObject.SetActive(false);
        }

        if (m_TongUI == null)
        {
            if (!isTongUILoaded)
            {
                isTongUILoaded = true;

                CallWhenUILoad();
                AssetCacheMgr.GetUIInstance("TongUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_TongUI = go as GameObject;
                        Utils.MountToSomeObjWithoutPosChange(m_TongUI.transform, m_MenuUI.transform.Find("PropSheet/DialogList/TongDialog").transform);
                        m_TongUI.AddComponent<TongUIViewManager>();
                        m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);

                        EventDispatcher.TriggerEvent(TongUIViewManager.Event.OnTongShow);

                    }
                );
            }
        }
        else
        {
            m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);
            EventDispatcher.TriggerEvent(TongUIViewManager.Event.OnTongShow);
        }

    }

    /// <summary>
    /// �罻
    /// </summary>
    public bool IsSocialUILoaded = false;
    public void SwitchSocialUI(Action act = null)
    {
        if (!UICanChange)
            return;
        if (m_MenuUI != CurrentUI)
        {
            ShowMogoMenuUI();
        }

        if (m_SocietyUI == null)
        {
            if (!IsSocialUILoaded)
            {
                IsSocialUILoaded = true;
                CallWhenUILoad(0, false);

                string[] resName = new string[] { "FriendItem.prefab", "FriendAcceptItem.prefab", "SoceityUIMessageGrid.prefab", "MailGrid.prefab" };

                PreLoadResources(resName, (obj) =>
                {

                    AssetCacheMgr.GetUIInstance("SocietyUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_SocietyUI = go as GameObject;
                            m_SocietyUI.transform.parent = m_MenuUI.transform.Find("PropSheet/DialogList/SocialDialog").transform;
                            m_SocietyUI.transform.localPosition = Vector3.zero;
                            m_SocietyUI.transform.localEulerAngles = Vector3.zero;
                            m_SocietyUI.transform.localScale = new Vector3(1, 1, 1);
                            m_SocietyUI.AddComponent<SocietyUIViewManager>();

                            m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(4);

                            EventDispatcher.TriggerEvent(SocietyUILogicManager.SocietyUIEvent.SOCIETY_UI_OPEN);
                            //EventDispatcher.TriggerEvent(SocietyUILogicManager.SocietyUIEvent.REFRESHMAILGRIDLIST);

                            MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                            if (act != null)
                            {
                                act();
                            }
                        }
                    );
                });
            }
        }
        else
        {
            m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(4);
            EventDispatcher.TriggerEvent(SocietyUILogicManager.SocietyUIEvent.SOCIETY_UI_OPEN);
            if (act != null)
            {
                act();
            }
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    public bool IsSettingsUILoaded = false;
    public void SwitchSettingsUI()
    {
        if (!UICanChange)
            return;
        if (m_MenuUI != CurrentUI)
        {
            ShowMogoMenuUI();
        }

        if (m_SettingUI == null)
        {
            if (!IsSettingsUILoaded)
            {

                CallWhenUILoad(0, false);
                IsSettingsUILoaded = true;

                AssetCacheMgr.GetUIInstance("SettingsUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_SettingUI = go as GameObject;
                        m_SettingUI.transform.parent = m_MenuUI.transform.Find("PropSheet/DialogList/SettingsDialog").transform;
                        m_SettingUI.transform.localPosition = Vector3.zero;
                        m_SettingUI.transform.localEulerAngles = Vector3.zero;
                        m_SettingUI.transform.localScale = new Vector3(1, 1, 1);
                        m_SettingUI.AddComponent<SettingsUIViewManager>();

                        m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(5);
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                    }
                );
            }
        }
        else
        {
            m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(5);
        }
    }

    #endregion

    #region �������

    /// <summary>
    /// ��������
    /// </summary>
    public bool IsCommunityUILoaded = false;
    public void ShowMogoCommuntiyUI(CommunityUIParent communityUIParent = CommunityUIParent.NormalMainUI, bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_CommunityUI == null)
        {
            if (!IsCommunityUILoaded)
            {
                IsCommunityUILoaded = true;

                AssetCacheMgr.GetUIInstance("CommunityUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_CommunityUI = go as GameObject;
                        m_CommunityUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_CommunityUI.transform.localPosition = new Vector3(0, 0, -10);
                        m_CommunityUI.transform.localScale = new Vector3(1, 1, 1);
                        m_CommunityUI.AddComponent<CommunityUIViewManager>();

                        m_CommunityUI.SetActive(isShow);

                        if (communityUIParent == CommunityUIParent.NormalMainUI)
                        {
                            CommunityUIViewManager.Instance.CommunityUIParent = CommunityUIParent.NormalMainUI;
                            if (CurrentUI != m_NormalMainUI)
                            {
                                //CurrentUI.SetActive(false);
                                //CurrentUI = m_NormalMainUI;
                                //CurrentUI.SetActive(true);

                                NormalMainUIViewManager.Instance.ShowCommunityButton(!isShow);
                                m_CommunityUI.SetActive(isShow);

                                if (MogoMainCamera.instance)
                                    MogoMainCamera.instance.SetActive(true);

                                m_camUI.clearFlags = CameraClearFlags.Depth;

                                ShowBillboardList(true);
                            }
                            if (!isShow)
                                EventDispatcher.TriggerEvent("ShowMogoNormalMainUI");
                        }
                        else
                        {
                            CommunityUIViewManager.Instance.CommunityUIParent = CommunityUIParent.MainUI;
                            if (CurrentUI != m_MainUI)
                            {
                                //CurrentUI.SetActive(false);
                                //CurrentUI = m_MainUI;
                                //CurrentUI.SetActive(true);

                                m_CommunityUI.SetActive(isShow);

                                if (MogoMainCamera.instance)
                                    MogoMainCamera.instance.SetActive(true);

                                m_camUI.clearFlags = CameraClearFlags.Depth;
                            }
                        }
                    }
                );
            }
        }
        else
        {
            if (communityUIParent == CommunityUIParent.NormalMainUI)
            {
                CommunityUIViewManager.Instance.CommunityUIParent = CommunityUIParent.NormalMainUI;
                ShowCurrentUI(false);
                CurrentUI = m_NormalMainUI;
                ShowCurrentUI(true);

                NormalMainUIViewManager.Instance.ShowCommunityButton(!isShow);
                m_CommunityUI.SetActive(isShow);

                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(true);
                m_camUI.clearFlags = CameraClearFlags.Depth;

                ShowBillboardList(true);
            }
            else
            {
                CommunityUIViewManager.Instance.CommunityUIParent = CommunityUIParent.MainUI;
                ShowCurrentUI(false);
                CurrentUI = m_MainUI;
                ShowCurrentUI(true);

                m_CommunityUI.SetActive(isShow);

                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(true);
                m_camUI.clearFlags = CameraClearFlags.Depth;
            }
        }
    }

    public void SwitchWorldChannelUI()
    {
        if (!UICanChange)
            return;
        if (m_CommunityUI != CurrentUI)
            ShowMogoCommuntiyUI(CommunityUIViewManager.Instance.CommunityUIParent);

        m_CommunityUI.transform.Find("CommunityUIBottom/CommunityUIDialogSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);
        CommunityUIViewManager.Instance.ShowFriendButton(false);
    }

    public void SwitchTongChannelUI()
    {
        if (!UICanChange)
            return;
        if (m_CommunityUI != CurrentUI)
            ShowMogoCommuntiyUI(CommunityUIViewManager.Instance.CommunityUIParent);

        m_CommunityUI.transform.Find("CommunityUIBottom/CommunityUIDialogSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);
        CommunityUIViewManager.Instance.ShowFriendButton(false);
    }

    public void SwitchPrivateChannelUI()
    {
        if (!UICanChange)
            return;
        if (m_CommunityUI != CurrentUI)
            ShowMogoCommuntiyUI(CommunityUIViewManager.Instance.CommunityUIParent);

        m_CommunityUI.transform.Find("CommunityUIBottom/CommunityUIDialogSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);
        CommunityUIViewManager.Instance.ShowFriendButton(true);
    }

    #endregion

    public bool IsSancturyUILoaded = false;

    public void ShowMogoSanctuaryUI(Action callback)
    {
        if (!UICanChange)
            return;
        if (m_SanctuaryUI == null)
        {
            if (!IsSancturyUILoaded)
            {
                IsSancturyUILoaded = true;

                CallWhenUILoad(0, false);
                AssetCacheMgr.GetUIInstance("SanctuaryUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_SanctuaryUI = go as GameObject;
                        m_SanctuaryUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_SanctuaryUI.transform.localPosition = Vector3.zero;
                        m_SanctuaryUI.transform.localScale = new Vector3(1, 1, 1);
                        m_SanctuaryUI.AddComponent<SanctuaryUIViewManager>();

                        if (CurrentUI != m_SanctuaryUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_SanctuaryUI;
                            ShowCurrentUI(true);
                            

                            //if (mogomaincamera.instance)
                            //    mogomaincamera.instance.setactive(false);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            ShowBillboardList(false);
                            MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                        }
                        if (callback != null)
                        {
                            callback();
                        }
                    }
                );
            }
        }
        else
        {
            if (CurrentUI != m_SanctuaryUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_SanctuaryUI;
                ShowCurrentUI(true);

                //if (MogoMainCamera.instance)
                //    MogoMainCamera.instance.SetActive(false);

                m_camUI.clearFlags = CameraClearFlags.Depth;

                ShowBillboardList(false);

            }
            if (callback != null)
            {
                callback();
            }
        }
    }

    public bool IsAssistantUILoaded = false;

    public void ShowMogoAssistantUI()
    {
        if (!UICanChange)
            return;


        if (m_AssistantUI == null)
        {
            if (!IsAssistantUILoaded)
            {
                IsAssistantUILoaded = true;

                AssetCacheMgr.GetUIInstance("AssistantUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_AssistantUI = go as GameObject;
                        m_AssistantUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_AssistantUI.transform.localPosition = Vector3.zero;
                        m_AssistantUI.transform.localScale = new Vector3(1, 1, 1);
                        m_AssistantUI.AddComponent<AssistantUIViewManager>();

                        if (CurrentUI != m_AssistantUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_AssistantUI;
                            ShowCurrentUI(true);

                            //AssistantUIViewManager.Instance.ShowAssistantRTTModel(true);

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            m_camUI.clearFlags = CameraClearFlags.Depth;

                            ShowBillboardList(false);
                        }


                        CallWhenUILoad();
                    }
                );
            }
        }
        else
        {
            if (CurrentUI != m_AssistantUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_AssistantUI;
                ShowCurrentUI(true);

                AssistantUIViewManager.Instance.ShowAssistantRTTModel(true);

                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(false);

                m_camUI.clearFlags = CameraClearFlags.Depth;

                ShowBillboardList(false);

            }
        }
    }

    #region ��Ӫϵͳ

    /// <summary>
    /// ��Ӫϵͳ
    /// </summary>
    public bool IsOperatingUILoaded = false;
    private void TruelyMogoOperatingUI(int firstShow = 0, Action action = null)
    {
        if (!UICanChange)
            return;
        if (m_OperatingUI == null)
        {
            if (!IsOperatingUILoaded)
            {
                IsOperatingUILoaded = true;

                //CallWhenUILoad();
                AssetCacheMgr.GetUIInstance("OperatingUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_OperatingUI = go as GameObject;
                        m_OperatingUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                        m_OperatingUI.transform.localPosition = Vector3.zero;
                        m_OperatingUI.transform.localScale = new Vector3(1, 1, 1);
                        m_OperatingUI.AddComponent<OperatingUIViewManager>();

                        if (CurrentUI != m_OperatingUI)
                        {
                            ShowCurrentUI(false);
                            CurrentUI = m_OperatingUI;
                            ShowCurrentUI(true);

                            if (MogoMainCamera.instance)
                                MogoMainCamera.instance.SetActive(false);

                            m_camUI.clearFlags = CameraClearFlags.SolidColor;

                            ShowBillboardList(false);

                            switch (firstShow)
                            {
                                case 1:
                                    LoggerHelper.Debug("firstShow SwitchTimeLimitActivityUI");
                                    SwitchTimeLimitActivityUI();
                                    break;
                                case 2:
                                    LoggerHelper.Debug("firstShow SwitchLoginRewardUI");
                                    SwitchLoginRewardUI();
                                    break;
                                case 3:
                                    LoggerHelper.Debug("firstShow SwitchAttributeRewardUI");
                                    SwitchAttributeRewardUI();
                                    break;
                                default:
                                    LoggerHelper.Debug("firstShow SwitchChargeRewardUI");
                                    SwitchChargeRewardUI();
                                    break;

                            }

                            if (MogoUIManager.Instance.WaitingWidgetName == "AttributeRewardBtn" ||
                                MogoUIManager.Instance.WaitingWidgetName == "LoginRewardBtn" ||
                                MogoUIManager.Instance.WaitingWidgetName == "ChargeRewardBtn" ||
                                MogoUIManager.Instance.WaitingWidgetName == "TimeLimitActivityBtn")
                            {
                                EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                            }

                            if (action != null)
                                action();
                        }
                    }
                );
            }
        }
        else
        {
            if (CurrentUI != m_OperatingUI)
            {
                ShowCurrentUI(false);
                CurrentUI = m_OperatingUI;
                ShowCurrentUI(true);

                if (MogoMainCamera.instance)
                    MogoMainCamera.instance.SetActive(false);

                m_camUI.clearFlags = CameraClearFlags.SolidColor;

                ShowBillboardList(false);

                Mogo.Util.LoggerHelper.Debug(MogoWorld.thePlayer.IsAchiementHasOpen + "" + MogoWorld.thePlayer.IsTimeLimitEventHasOpen);

                OperatingUIViewManager.Instance.ShowAttributeRewardBtn(MogoWorld.thePlayer.IsAchiementHasOpen);
                OperatingUIViewManager.Instance.ShowTimeLimitActivityBtn(MogoWorld.thePlayer.IsTimeLimitEventHasOpen);

                //OperatingUIViewManager.Instance.ShowChargeRewardBtn(!MogoWorld.thePlayer.IsLoginFirstShow);
                //OperatingUIViewManager.Instance.ShowLoginRewardBtn(!MogoWorld.thePlayer.IsLoginFirstShow);

                OperatingUIViewManager.Instance.ShowChargeRewardBtn(true);
                OperatingUIViewManager.Instance.ShowLoginRewardBtn(true);

                MogoWorld.thePlayer.IsLoginFirstShow = false;

                //switch (OperatingUILogicManager.Instance.CurrentPage)
                //{
                //    case 0:
                //        EventDispatcher.TriggerEvent(Events.OperationEvent.GetChargeRewardMessage);
                //        break;
                //    case 1:
                //        EventDispatcher.TriggerEvent(Events.OperationEvent.GetActivityMessage);
                //        break;
                //    case 2:
                //        EventDispatcher.TriggerEvent(Events.OperationEvent.GetLoginMessage);
                //        break;
                //    case 3:
                //        EventDispatcher.TriggerEvent(Events.OperationEvent.GetAchievementMessage);
                //        break;
                //    default:
                //        EventDispatcher.TriggerEvent(Events.OperationEvent.GetChargeRewardMessage);
                //        break;

                //}

                switch (firstShow)
                {
                    case 1:
                        LoggerHelper.Debug("firstShow SwitchTimeLimitActivityUI");
                        SwitchTimeLimitActivityUI();
                        break;
                    case 2:
                        LoggerHelper.Debug("firstShow SwitchLoginRewardUI");
                        SwitchLoginRewardUI();
                        break;
                    case 3:
                        LoggerHelper.Debug("firstShow SwitchAttributeRewardUI");
                        SwitchAttributeRewardUI();
                        break;
                    default:
                        LoggerHelper.Debug("firstShow SwitchChargeRewardUI");
                        SwitchChargeRewardUI();
                        break;

                }

                if (MogoUIManager.Instance.WaitingWidgetName == "AttributeRewardBtn" ||
                                MogoUIManager.Instance.WaitingWidgetName == "LoginRewardBtn" ||
                                MogoUIManager.Instance.WaitingWidgetName == "ChargeRewardBtn" ||
                                MogoUIManager.Instance.WaitingWidgetName == "TimeLimitActivityBtn")
                {
                    EventDispatcher.TriggerEvent("WaitingWidgetFinished");
                }

                if (action != null)
                    action();
            }
        }
    }

    public void ShowMogoOperatingUI(int firstShow = 0, bool isManualOpen = true, Action action = null)
    {
        if (!UICanChange)
            return;
        if (isManualOpen)
        {
            MogoUIQueue.Instance.PushOne(() => 
            {
                MogoGlobleUIManager.Instance.ShowWaitingTip(true);
                TruelyMogoOperatingUI(firstShow, action); 
            }, null, "ShowMogoOperatingUI");
        }
        else
        {
            MogoUIQueue.Instance.PushOne(() => 
            {
                MogoGlobleUIManager.Instance.ShowWaitingTip(true);
                TruelyMogoOperatingUI(firstShow, action); 
            }, m_NormalMainUI, "ShowMogoOperatingUI");
        }
        // TruelyMogoOperatingUI(firstShow);
    }

    /// <summary>
    /// ��ֵ����
    /// </summary>
    public bool IsChargeRewardUILoaded = false;
    public void SwitchChargeRewardUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_OperatingUI != CurrentUI)
        {
            ShowMogoOperatingUI();
        }

        OperatingUILogicManager.Instance.CurrentPage = (int)OperatingUITab.ChargeRewardTab;

        if (m_ChargeRewardUI == null)
        {
            CallWhenUILoad(0, false);
            if (!IsChargeRewardUILoaded)
            {
                IsChargeRewardUILoaded = true;

                CallWhenUILoad();
                AssetCacheMgr.GetUIInstance("ChargeRewardUI.prefab",
                    (prefab, guid, go) =>
                    {
                        m_ChargeRewardUI = go as GameObject;
                        m_ChargeRewardUI.transform.parent = m_OperatingUI.transform.Find("OperatingUISheet/OperatingUIDialogList/ChargeRewardDialog").transform;
                        m_ChargeRewardUI.transform.localPosition = new Vector3(0, 0, -2);
                        m_ChargeRewardUI.transform.localEulerAngles = Vector3.zero;
                        m_ChargeRewardUI.transform.localScale = new Vector3(1, 1, 1);
                        m_ChargeRewardUI.SetActive(isShow);
                        m_ChargeRewardUI.AddComponent<ChargeRewardUIViewManager>();

                        m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);

                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                    }
                );
            }
        }
        else
        {
            m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(0);

            EventDispatcher.TriggerEvent(Events.OperationEvent.GetChargeRewardMessage);
        }
    }


    public bool IsNewTimeLimitActivityUILoaded = false;
    public void SwitchNewTimeLimitActivityUI(bool isShow = true)
    {
        if (!UICanChange)
            return;

        if (m_NewTimeLimitActivityUI == null)
        {
            if (!IsNewTimeLimitActivityUILoaded)
            {
                IsNewTimeLimitActivityUILoaded = true;

                CallWhenUILoad(0, false);

                PreLoadResource("TimeLimitActivityGrid.prefab", (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("NewTimeLimitActivityUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_NewTimeLimitActivityUI = go as GameObject;
                            m_NewTimeLimitActivityUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                            m_NewTimeLimitActivityUI.transform.localPosition = new Vector3(134, 0, 0);
                            m_NewTimeLimitActivityUI.transform.localEulerAngles = Vector3.zero;
                            m_NewTimeLimitActivityUI.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                            m_NewTimeLimitActivityUI.AddComponent<TimeLimitActivityUIViewManager>();
                            m_NewTimeLimitActivityUI.SetActive(isShow);

                            //m_OperatingUI.transform.FindChild("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);
                            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                        }
                    );
                });
            }
        }
        else
        {
            //m_OperatingUI.transform.FindChild("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);
            m_NewTimeLimitActivityUI.SetActive(isShow);
            EventDispatcher.TriggerEvent(Events.OperationEvent.GetActivityMessage);
        }
    }

    /// <summary>
    /// ��ʱ�
    /// </summary>
    public bool IsTimeLimitActivityUILoaded = false;
    public void SwitchTimeLimitActivityUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_OperatingUI != CurrentUI)
        {
            ShowMogoOperatingUI(1);
        }

        OperatingUILogicManager.Instance.CurrentPage = (int)OperatingUITab.TimeLimitActivityTab;

        if (m_TimeLimitActivityUI == null)
        {
            if (!IsTimeLimitActivityUILoaded)
            {
                IsTimeLimitActivityUILoaded = true;

                CallWhenUILoad(0, false);

                PreLoadResource("TimeLimitActivityGrid.prefab", (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("TimeLimitActivityUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_TimeLimitActivityUI = go as GameObject;
                            m_TimeLimitActivityUI.transform.parent = m_OperatingUI.transform.Find("OperatingUISheet/OperatingUIDialogList/TimeLimitActivityDialog").transform;
                            m_TimeLimitActivityUI.transform.localPosition = new Vector3(0, 0, -2);
                            m_TimeLimitActivityUI.transform.localEulerAngles = Vector3.zero;
                            m_TimeLimitActivityUI.transform.localScale = new Vector3(1, 1, 1);
                            m_TimeLimitActivityUI.AddComponent<TimeLimitActivityUIViewManager>();
                            m_TimeLimitActivityUI.SetActive(isShow);

                            m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);
                            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                        }
                    );
                });
            }
        }
        else
        {
            m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(1);

            EventDispatcher.TriggerEvent(Events.OperationEvent.GetActivityMessage);
        }
    }

    /// <summary>
    /// ��¼����
    /// </summary>
    public bool IsLoginRewardUILoaded = false;
    public void SwitchLoginRewardUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_OperatingUI != CurrentUI)
        {
            ShowMogoOperatingUI(2);
        }

        OperatingUILogicManager.Instance.CurrentPage = (int)OperatingUITab.LoginRewardTab;

        if (m_LoginRewardUI == null)
        {
            if (!IsLoginRewardUILoaded)
            {
                IsLoginRewardUILoaded = true;

                CallWhenUILoad(0, false);

                PreLoadResource("LoginRewardGrid.prefab", (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("LoginRewardUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_LoginRewardUI = go as GameObject;
                            m_LoginRewardUI.transform.parent = m_OperatingUI.transform.Find("OperatingUISheet/OperatingUIDialogList/LoginRewardDialog").transform;
                            m_LoginRewardUI.transform.localPosition = new Vector3(0, 0, -2);
                            m_LoginRewardUI.transform.localEulerAngles = Vector3.zero;
                            m_LoginRewardUI.transform.localScale = new Vector3(1, 1, 1);
                            m_LoginRewardUI.AddComponent<LoginRewardUIViewManager>();

                            m_LoginRewardUI.SetActive(isShow);

                            m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);


                        }
                    );
                });
            }
        }
        else
        {
            m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(2);

            EventDispatcher.TriggerEvent(Events.OperationEvent.GetLoginMessage);
            // EventDispatcher.TriggerEvent(MarketEvent.DownloadLoginMarket);
        }
    }

     public bool IsNewAttributeRewardUILoaded = false;
     public void SwitchNewAttributeRewardUI(bool isShow = true)
     {
         if (!UICanChange)
             return;

         // OperatingUILogicManager.Instance.CurrentPage = (int)OperatingUITab.AttributeRewardTab;

         if (m_NewAttributeRewardUI == null)
         {
             if (!IsNewAttributeRewardUILoaded)
             {
                 IsNewAttributeRewardUILoaded = true;


                 CallWhenUILoad(0, false);

                 PreLoadResource("AttributeRewardGrid.prefab", (obj) =>
                 {
                     AssetCacheMgr.GetUIInstance("NewAttributeRewardUI.prefab",
                         (prefab, guid, go) =>
                         {
                             m_NewAttributeRewardUI = go as GameObject;
                             m_NewAttributeRewardUI.transform.parent = GameObject.Find("MogoMainUIPanel").transform;
                             m_NewAttributeRewardUI.transform.localPosition = new Vector3(130, 0, 0);
                             m_NewAttributeRewardUI.transform.localEulerAngles = Vector3.zero;
                             m_NewAttributeRewardUI.transform.localScale = new Vector3(0.78f, 0.78f, 1);
                             m_NewAttributeRewardUI.AddComponent<AttributeRewardUIViewManager>();
                             m_NewAttributeRewardUI.SetActive(isShow);


                             MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                         }
                     );
                 });
             }
         }
         else
         {

             EventDispatcher.TriggerEvent(Events.OperationEvent.GetAchievementMessage);
             m_NewAttributeRewardUI.SetActive(isShow);
         }
     }

    /// <summary>
    /// �ɾͽ���
    /// </summary>
    public bool IsAttributeRewardUILoaded = false;
    public void SwitchAttributeRewardUI(bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_OperatingUI != CurrentUI)
        {
            ShowMogoOperatingUI(3);
        }

        OperatingUILogicManager.Instance.CurrentPage = (int)OperatingUITab.AttributeRewardTab;

        if (m_AttributeRewardUI == null)
        {
            if (!IsAttributeRewardUILoaded)
            {
                IsAttributeRewardUILoaded = true;


                CallWhenUILoad(0, false);

                PreLoadResource("AttributeRewardGrid.prefab", (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("AttributeRewardUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_AttributeRewardUI = go as GameObject;
                            m_AttributeRewardUI.transform.parent = m_OperatingUI.transform.Find("OperatingUISheet/OperatingUIDialogList/AttributeRewardDialog").transform;
                            m_AttributeRewardUI.transform.localPosition = new Vector3(0, 0, -2);
                            m_AttributeRewardUI.transform.localEulerAngles = Vector3.zero;
                            m_AttributeRewardUI.transform.localScale = new Vector3(1, 1, 1);
                            m_AttributeRewardUI.AddComponent<AttributeRewardUIViewManager>();
                            m_AttributeRewardUI.SetActive(isShow);

                            m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);

                            MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                            //if (WaitingWidgetName == "AttributeRewardGrid0")
                            //{
                            //    TimerHeap.AddTimer(500, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
                            //}
                        }
                    );
                });
            }
        }
        else
        {
            m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);

            EventDispatcher.TriggerEvent(Events.OperationEvent.GetAchievementMessage);
        }
    }

    public void SwitchAttributeRewardUI(Action action, bool isShow = true)
    {
        if (!UICanChange)
            return;
        if (m_OperatingUI != CurrentUI)
        {
            ShowMogoOperatingUI(3);
        }

        OperatingUILogicManager.Instance.CurrentPage = (int)OperatingUITab.AttributeRewardTab;

        if (m_AttributeRewardUI == null)
        {
            if (!IsAttributeRewardUILoaded)
            {
                IsAttributeRewardUILoaded = true;


                CallWhenUILoad(0, false);
                PreLoadResource("AttributeRewardGrid.prefab", (obj) =>
                {
                    AssetCacheMgr.GetUIInstance("AttributeRewardUI.prefab",
                        (prefab, guid, go) =>
                        {
                            m_AttributeRewardUI = go as GameObject;
                            m_AttributeRewardUI.transform.parent = m_OperatingUI.transform.Find("OperatingUISheet/OperatingUIDialogList/AttributeRewardDialog").transform;
                            m_AttributeRewardUI.transform.localPosition = new Vector3(0, 0, -2);
                            m_AttributeRewardUI.transform.localEulerAngles = new Vector3(0, 0, 0);
                            m_AttributeRewardUI.transform.localScale = new Vector3(1, 1, 1);
                            m_AttributeRewardUI.AddComponent<AttributeRewardUIViewManager>();

                            m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);

                            action();

                            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                            //if (WaitingWidgetName == "AttributeRewardGrid0")
                            //{
                            //    TimerHeap.AddTimer(100, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
                            //}
                        }
                    );
                });
            }
        }
        else
        {
            m_OperatingUI.transform.Find("OperatingUISheet").GetComponentsInChildren<MogoPropSheet>(true)[0].SwitchDialog(3);

            EventDispatcher.TriggerEvent(Events.OperationEvent.GetAchievementMessage);

            action();
        }
    }

    #endregion

    public void ChangeSettingToControlStick()
    {
        //  MainUIViewManager.Instance.SetControllStickEnable(true);

        if (MainUIViewManager.Instance)
        {
            MainUIViewManager.Instance.SetControllStickEnable(true);
        }

        if (NormalMainUIViewManager.Instance)
        {
            NormalMainUIViewManager.Instance.SetControlStickEnable(true);
        }

        m_bEnableControlStick = true;

    }

    public void ChangeSettingToTouch()
    {
        //   MainUIViewManager.Instance.SetControllStickEnable(false);
        //NormalMainUIViewManager.Instance.SetControlStickEnable(false);

        if (MainUIViewManager.Instance)
        {
            MainUIViewManager.Instance.SetControllStickEnable(false);
        }

        if (NormalMainUIViewManager.Instance)
        {
            NormalMainUIViewManager.Instance.SetControlStickEnable(false);
        }

        m_bEnableControlStick = false;
    }

    public void ShowMogoBattleMainUIWithInstancePassUI(bool isShow)
    {
        if (!UICanChange)
            return;
        MogoUIManager.Instance.ShowMogoInstanceUI();

        m_MainUI.SetActive(isShow);

        if (isShow)
            InstanceUIViewManager.Instance.ShowInstanceUIMaskBG(!isShow);

        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(true);

        if (!isShow)
            ControlStick.instance.Reset();

        m_camUI.clearFlags = CameraClearFlags.Depth;
    }

    public void ShowMogoBattleMainUIWithInstancePassRewardUI(bool isShow)
    {
        if (!UICanChange)
            return;
        m_MainUI.SetActive(isShow);

        if (isShow)
            InstancePassRewardUIViewManager.Instance.ShowInstancePassRewardUIMaskBG(!isShow);

        if (MogoMainCamera.instance)
            MogoMainCamera.instance.SetActive(true);

        if (!isShow)
            ControlStick.instance.Reset();

        m_camUI.clearFlags = CameraClearFlags.Depth;
    }

    public void ShowBillboardList(bool isShow)
    {
        BillboardViewManager.Instance.ShowBillboardList(isShow);
    }

    private void ShowSocialTip()
    {
        if (FriendManager.Instance != null)
        {
            FriendManager.Instance.FreshTipUI();
        }
        if (MailManager.Instance != null)
        {
            MailManager.Instance.FreshTipUI();
        }
    }

    void Awake()
    {
        m_myTransform = transform;

        m_camUI = m_myTransform.parent.parent.GetComponentsInChildren<Camera>(true)[0];

        //m_LoginUI = m_myTransform.FindChild("LoginUI").gameObject;
        //m_LoginUI.AddComponent<LoginUIViewManager>();
        //m_ChooseServerUI = m_myTransform.FindChild("ChooseServerUI").gameObject;
        //m_CreateCharacterUI = m_myTransform.FindChild("CreateCharacterUI").gameObject;
        //m_ChooseCharacterUI = m_myTransform.FindChild("ChooseCharacterUI").gameObject;

        // ShowMogoNormalMainUI();

        m_MainUI = m_myTransform.Find("MainUI").gameObject;
        m_NormalMainUI = m_myTransform.Find("NormalMainUI").gameObject;
        m_DebugUI = m_myTransform.Find("DebugUI").gameObject;

        InstanceUIPartCount = 0;
        EventDispatcher.AddEventListener("InstanceUILoadPartEnd", CheckDefaultLoadInstanceUI);

        MogoFXManager.Instance.Initialize();
    }
 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            //TeachUILogicManager.Instance.SetTeachUIFocus(174);
            //TeachUILogicManager.Instance.SetTeachUINoneFocus();
            //TeachUILogicManager.Instance.ShowFingerAnim(true);

            //MogoFXManager.Instance.GetHit(MogoWorld.thePlayer.GameObject, 0.05f, MogoFXManager.HitColorType.HCT_WHITE);

            NormalMainUIViewManager.Instance.IsDebug = true;
            ShowMogoNormalMainUI();
        }

        //if (Input.GetKeyDown(KeyCode.F10))
        //{
        //    MogoFXManager.Instance.HandleUIFX(1);
        //}

        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    MogoFXManager.Instance.DetachUIFX(1);
        //}

        MogoFXManager.Instance.Process();


        if (Input.GetKeyDown(KeyCode.Insert))
        {
            //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.WingUI);

            EventDispatcher.TriggerEvent(Events.WingEvent.Open);
            //WingUILogicManager.Instance.SetUIDirty();

            
            //EventDispatcher.TriggerEvent(Events.RewardEvent.OpenReward);
        }
        if(Input.GetKeyDown(KeyCode.PageDown))
        {
            EventDispatcher.TriggerEvent(Events.RewardEvent.OpenReward);
        }
        //if (Input.GetKeyDown(KeyCode.LeftAlt))
        //{
        //    MogoUIManager.Instance.ShowMogoNormalMainUI();
        //}


    }

    public void CallWhenUILoad(uint deltaTime = 2000, bool defaultTime = true)
    {
        //LoggerHelper.Error("CallWhenUILoad");
        //GetSwitchUICamera().gameObject.SetActive(true);

        //TimerHeap.AddTimer(500, 0, () => { GetSwitchUICamera().gameObject.SetActive(false); });

        //UICamera cam = GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0];
        //cam.enabled = false;

        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //    return;

        MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        if (defaultTime)
        {
            TimerHeap.AddTimer(deltaTime, 0, () => { MogoGlobleUIManager.Instance.ShowWaitingTip(false); ; });
        }
    }    

    #region ��ͼ����

    List<UIAtlas> m_listSpcecityAtlas = new List<UIAtlas>();
    public UIAtlas CommonAtlas;
    public UIAtlas CommonExtraAtlas;
    public UIAtlas MogoNormalMainUIAtlas;
    public UIAtlas SkillIconAtlas;
    public UIAtlas TempAtlas;

    public string GetAtlasNameByJob()
    {
        string atlasName = "";

        switch (MogoWorld.thePlayer.vocation)
        {
            case Mogo.Game.Vocation.Archer:
                atlasName = "MogoArcher";
                break;

            case Mogo.Game.Vocation.Assassin:
                atlasName = "MogoAssasin";
                break;

            case Mogo.Game.Vocation.Mage:
                atlasName = "MogoMage";
                break;

            case Mogo.Game.Vocation.Warrior:
                atlasName = "MogoWarrior";
                break;
        }

        //atlasName = "MogoUI.prefab";
        return atlasName;
    } 

    public void TryingSetSpriteName(string iconName,UISprite sp)
    {
        if (string.IsNullOrEmpty(iconName)) return;
        if (sp.atlas != null && sp.atlas.GetSprite(iconName) != null)
        {
            sp.spriteName = iconName;
            return;
        }
        if (CommonAtlas.GetSprite(iconName) != null)
        {
            sp.atlas = CommonAtlas;
            sp.spriteName = iconName;
            return;
        }
        else if (CommonExtraAtlas != null && CommonExtraAtlas.GetSprite(iconName) != null)
        {
            sp.atlas = CommonExtraAtlas;
            sp.spriteName = iconName;
            return;
        }
        else if (MogoNormalMainUIAtlas != null && MogoNormalMainUIAtlas.GetSprite(iconName) != null)
        {
            sp.atlas = MogoNormalMainUIAtlas;
            sp.spriteName = iconName;
        }
        else
        {
            for (int i = 0; i < m_listSpcecityAtlas.Count; ++i)
            {
                if (m_listSpcecityAtlas[i].GetSprite(iconName) != null)
                {
                    sp.atlas = m_listSpcecityAtlas[i];
                    sp.spriteName = iconName;

                    if (sp.atlas.spriteMaterial.mainTexture == null)
                    {
                        Debug.LogError(m_listSpcecityAtlas[i].name + ".png");

                        AssetCacheMgr.GetUIResource(m_listSpcecityAtlas[i].name + ".png", (obj) =>
                        {
                            sp.atlas.spriteMaterial.mainTexture = (Texture)obj;
                            sp.atlas.MarkAsDirty();
                        });
                    }
                    return;
                }
            }

            if (TempAtlas != null)
            {
                if (TempAtlas.GetSprite(iconName) != null)
                {
                    sp.atlas = TempAtlas;
                    sp.spriteName = iconName;
                    return;
                }
                else
                {
                    AssetCacheMgr.ReleaseResource(TempAtlas.texture);
                    AssetCacheMgr.ReleaseInstance(TempAtlas);
                    TempAtlas = null;
                }
            }

            if (iconName.Length >= 11 && iconName.Substring(0, 11) == "icon_archer")
            {
                string lastCharacter = iconName.Substring(iconName.Length - 1, 1);

                switch (lastCharacter)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                        AssetCacheMgr.GetUIInstance("MogoArcher20EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;

                            TempAtlas = go.GetComponentInChildren<UIAtlas>();

                            go.hideFlags = HideFlags.HideAndDontSave;

                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;

                    case "6":
                    case "7":
                        AssetCacheMgr.GetUIInstance("MogoArcher40EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;

                            TempAtlas = go.GetComponentInChildren<UIAtlas>();

                            go.hideFlags = HideFlags.HideAndDontSave;

                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;
                }
            }
            else if (iconName.Length >= 9 && iconName.Substring(0, 9) == "icon_mage")
            {
                string lastCharacter = iconName.Substring(iconName.Length - 1, 1);

                Mogo.Util.LoggerHelper.Debug("Damn !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + iconName.Substring(iconName.Length - 1, 1));
                switch (lastCharacter)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                        AssetCacheMgr.GetUIInstance("MogoMage20EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;

                            TempAtlas = go.GetComponentInChildren<UIAtlas>();

                            go.hideFlags = HideFlags.HideAndDontSave;

                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;

                    case "6":
                    case "7":
                        AssetCacheMgr.GetUIInstance("MogoMage40EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;

                            TempAtlas = go.GetComponentInChildren<UIAtlas>();

                            go.hideFlags = HideFlags.HideAndDontSave;

                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;
                }
            }
            else if (iconName.Length >= 13 && iconName.Substring(0, 13) == "icon_assassin")
            {
                string lastCharacter = iconName.Substring(iconName.Length - 1, 1);

                switch (lastCharacter)
                {
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                        AssetCacheMgr.GetUIInstance("MogoAssasin20EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;

                            TempAtlas = go.GetComponentInChildren<UIAtlas>();

                            go.hideFlags = HideFlags.HideAndDontSave;

                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;

                    case "6":
                    case "7":
                        AssetCacheMgr.GetUIInstance("MogoAssasin40EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;

                            TempAtlas = go.GetComponentInChildren<UIAtlas>();

                            go.hideFlags = HideFlags.HideAndDontSave;

                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;
                }
            }
            else if (iconName.Length >= 12 && iconName.Substring(0, 12) == "icon_warrior")
            {
                string lastCharacter = iconName.Substring(iconName.Length - 1, 1);

                switch (lastCharacter)
                {
                    case "1":
                    case "2":
                    case "3":

                    case "4":
                    case "5":
                        AssetCacheMgr.GetUIInstance("MogoWarrior20EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;

                            TempAtlas = go.GetComponentInChildren<UIAtlas>();

                            go.hideFlags = HideFlags.HideAndDontSave;

                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                        });
                        break;
                    case "6":
                    case "7":
                        AssetCacheMgr.GetUIInstance("MogoWarrior40EquipUI.prefab", (prefab, guid, gameObject) =>
                        {
                            GameObject go = (GameObject)gameObject;

                            TempAtlas = go.GetComponentInChildren<UIAtlas>();

                            go.hideFlags = HideFlags.HideAndDontSave;

                            sp.atlas = TempAtlas;
                            sp.spriteName = iconName;
                            Debug.LogError("DamnHere");
                        });
                        break;
                }
            }
        }
    }

    public UIAtlas GetAtlasByIconName(string iconName)
    {
        if (CommonAtlas.GetSprite(iconName) != null)
        {
            return CommonAtlas;
        }
        else if (CommonExtraAtlas != null && CommonExtraAtlas.GetSprite(iconName) != null)
        {
            return CommonExtraAtlas;
        }
        else if (MogoNormalMainUIAtlas != null && MogoNormalMainUIAtlas.GetSprite(iconName) != null)
        {
            return MogoNormalMainUIAtlas;
        }
        else if (SkillIconAtlas != null && SkillIconAtlas.GetSprite(iconName) != null)
        {
            return SkillIconAtlas;
        }
        else
        {
            for (int i = 0; i < m_listSpcecityAtlas.Count; ++i)
            {
                if (m_listSpcecityAtlas[i].GetSprite(iconName) != null)
                {
                    if (SystemSwitch.DestroyResource)
                    {
                        if (m_listSpcecityAtlas[i].spriteMaterial.mainTexture == null)
                        {
                            AssetCacheMgr.GetUIResource(string.Concat(m_listSpcecityAtlas[i].name, ".png"),
                                (obj) =>
                                {
                                    m_listSpcecityAtlas[i].spriteMaterial.mainTexture = (Texture)obj;
                                    m_listSpcecityAtlas[i].MarkAsDirty();
                                });
                        }
                    }

                    return m_listSpcecityAtlas[i];
                }
            }


            Mogo.Util.LoggerHelper.Debug("Atlas Do Not Contain the Sprite !!! Please Contact Mr.Deng " + iconName);
            return CommonAtlas;
        }
    }

    public UIAtlas GetSkillIconAtlas()
    {
        return SkillIconAtlas;
    }

    public void ReleaseUIResources()
    {
        //if (CommonAtlas != null)
        //{
        //    AssetCacheMgr.ReleaseInstance(CommonAtlas.gameObject);
        //    AssetCacheMgr.ReleaseResource(CommonAtlas.texture);

        //    Mogo.Util.LoggerHelper.Debug("Release CommonAtlas!!!!!!!!!!!1");
        //}

        //if (SpecifyAtlas != null)
        //{
        //    AssetCacheMgr.ReleaseInstance(SpecifyAtlas.gameObject);
        //    AssetCacheMgr.ReleaseResource(SpecifyAtlas.texture);
        //    Mogo.Util.LoggerHelper.Debug("Release SpecifyAtals !!!!!!!!!!!!!!!!!!!");
        //}

        if (m_listSpcecityAtlas.Count > 0)
        {
            for (int i = 0; i < m_listSpcecityAtlas.Count; ++i)
            {
                AssetCacheMgr.ReleaseInstance(m_listSpcecityAtlas[i].gameObject);
                AssetCacheMgr.ReleaseResource(m_listSpcecityAtlas[i].texture);
                m_listSpcecityAtlas[i] = null;
            }

            m_listSpcecityAtlas.Clear();
        }

        if (SkillIconAtlas != null)
        {
            AssetCacheMgr.ReleaseInstance(SkillIconAtlas.gameObject);
            AssetCacheMgr.ReleaseResource(SkillIconAtlas.texture);
            SkillIconAtlas = null;
        }

        if (TempAtlas != null)
        {
            AssetCacheMgr.ReleaseInstance(TempAtlas.gameObject);
            AssetCacheMgr.ReleaseResource(TempAtlas.texture);
            TempAtlas = null;
        }


    }

    #endregion

    #region Camera����

    private Camera m_mainUICamera;
    private Camera m_msgBoxCamera;
    private Camera m_uiFXCamera;
    private Camera m_switchUICamera;

    public void LockMainCamera(bool isLock)
    {
        GetMainUICamera().enabled = !isLock;
        GetMainUICamera().GetComponentsInChildren<UICamera>(true)[0].enabled = !isLock;

        // Icon�۵���ص��������
        if (NormalMainUIViewManager.Instance != null)
        {
            NormalMainUIViewManager.Instance.ShowNormalBTNListCam(!isLock);
        }      
    }

    public Camera GetMainUICamera()
    {
        if (m_mainUICamera == null)
        {
            m_mainUICamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponent<Camera>();
        }

        return m_mainUICamera;
    }

    public Camera GetMessageBoxCamera()
    {
        if (m_msgBoxCamera == null)
        {
            m_msgBoxCamera = GameObject.Find("MogoMainUI").transform.Find("MessageBoxCamera").GetComponent<Camera>();
        }

        return m_msgBoxCamera;
    }

    public Camera GetUIFXCamera()
    {
        if (m_uiFXCamera == null)
        {
            m_uiFXCamera = GameObject.Find("MogoMainUI").transform.Find("UIFXCamera").GetComponent<Camera>();
        }

        return m_uiFXCamera;
    }

    public Camera GetSwitchUICamera()
    {
        if (m_switchUICamera == null)
        {
            m_switchUICamera = GameObject.Find("MogoMainUI").transform.Find("SwitchUICamera").GetComponent<Camera>();
        }

        return m_switchUICamera;
    }

    #endregion  

    #region ����UI

    public void FirstPreLoadUIResources()
    {
        // Ԥ����
        if (InstanceMissionChooseUIViewManager.SHOW_MISSION_BY_DRAG)
            ShowInstanceMissionChooseUI(false);
        else
            ShowNewInstanceChooseMissionUI(false);
        LoadMogoInstanceLevelChooseUI(false);
        LoadMogoInstanceHelpChooseUI(false);
        //LoadMogoInstancePassUI(false);
        LoadMogoInstancePassRewardUI(false);
        LoadMogoInstanceTreasureChestUI(false);
        LoadMogoInstanceCleanUI(false);
        LoadMogoInstanceTaskRewardUI(false);
        ShowMogoCommuntiyUI(CommunityUIParent.NormalMainUI, false);
    }

    public void SecondPreLoadUIResources()
    {
        //PreLoadResources(new string[] { 
        //    "LoginRewardGrid.prefab", 
        //    "LoginRewardUI.prefab",
        //    "PackageItemGrid.prefab",
        //    "MenuUI.prefab",
        //    "StrenthenDialogIconGrid.prefab",
        //"StrenthUI.prefab",
        //"StrenthenDialogIconGrid.prefab",
        //"InsetDialogPackageGrid.prefab",
        //"InsetUI.prefab",
        //"EquipmentUI.prefab",
        //"EquipmentUIComposeIconListGrid.prefab",
        //"EquipmentUIComposeIconListGridChild.prefab",
        //"ComposeUI.prefab",
        //"DecomposeDialogPackageGrid.prefab",
        //"DecomposeDialogIconGrid.prefab",
        //"DecomposeUI.prefab",
        //"FriendItem.prefab",
        //}, (objs) =>
        //{
        //});
        //var sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        AssetCacheMgr.GetSecondUIResource("fx_ui_powercharge.prefab", null);
        AssetCacheMgr.GetSecondUIResource("LoginRewardGrid.prefab", null);
        AssetCacheMgr.GetSecondUIResource("LoginRewardUI.prefab", null);
        AssetCacheMgr.GetSecondUIResource("PackageItemGrid.prefab", null);
        AssetCacheMgr.GetSecondUIResource("MenuUI.prefab", null);
        AssetCacheMgr.GetSecondUIResource("StrenthenDialogIconGrid.prefab", null);
        AssetCacheMgr.GetSecondUIResource("StrenthUI.prefab", null);
        AssetCacheMgr.GetSecondUIResource("StrenthenDialogIconGrid.prefab", null);
        AssetCacheMgr.GetSecondUIResource("InsetDialogPackageGrid.prefab", null);
        AssetCacheMgr.GetSecondUIResource("InsetUI.prefab", null);
        AssetCacheMgr.GetSecondUIResource("EquipmentUI.prefab", null);
        AssetCacheMgr.GetSecondUIResource("EquipmentUIComposeIconListGrid.prefab", null);
        AssetCacheMgr.GetSecondUIResource("EquipmentUIComposeIconListGridChild.prefab", null);
        AssetCacheMgr.GetSecondUIResource("ComposeUI.prefab", null);
        AssetCacheMgr.GetSecondUIResource("DecomposeDialogPackageGrid.prefab", null);
        AssetCacheMgr.GetSecondUIResource("DecomposeDialogIconGrid.prefab", null);
        AssetCacheMgr.GetSecondUIResource("DecomposeUI.prefab", null);
        //AssetCacheMgr.GetSecondUIResource("FriendItem.prefab", null);
        //AssetCacheMgr.GetSecondUIResource("FriendAcceptItem.prefab", null);
        //AssetCacheMgr.GetSecondUIResource("SoceityUIMessageGrid.prefab", null);
        //AssetCacheMgr.GetSecondUIResource("MailGrid.prefab", null);
        //AssetCacheMgr.GetSecondUIResource("SocietyUI.prefab", null);
        //AssetCacheMgr.GetSecondUIResource("OperatingUI.prefab", null);
        //AssetCacheMgr.GetSecondUIResource("SkillUI.prefab", (obj) =>
        //{
        //    sw.Stop();
        //    LoggerHelper.Info("SecondPreLoadUIResources: " + sw.ElapsedMilliseconds);
        //});
    }

    protected int InstanceUIPartCount = 0;
    protected void CheckDefaultLoadInstanceUI()
    {
        InstanceUIPartCount++;
        if (InstanceUIPartCount != 6)
            return;

        InstanceUIPartCount = 0;
        LoadMogoInstanceUI(() => { }, false);
    }

    public void LoadUIResources()
    {
        AssetCacheMgr.GetUIResource("MogoLakeBlueMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoGreenMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDeepBlueMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoPurposeMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoOrangeMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoRedMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoYellowMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoRoseRedMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoGrassGreenMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoBlackWhiteMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonGreenMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonBlueMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonPurposeMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonOrangeMat.mat", (obj) => { });
        AssetCacheMgr.GetUIResource("MogoDragonDarkGoldMat.mat", (obj) => { });


        AssetCacheMgr.GetUIResource("fx_ui_skill_yes.prefab", (obj) => { });
        AssetCacheMgr.GetUIResource("fx_ui_anliuzhuanquan.prefab", (obj) => { });
        AssetCacheMgr.GetUIResource("ComboAttackNum.prefab", (obj) => { });
        AssetCacheMgr.GetUIResource("fx_ui_icon_open.prefab", null);

        if (MogoUIManager.Instance.SkillIconAtlas == null)
        {
            string atlasName = "";

            switch (MogoWorld.thePlayer.vocation)
            {
                case Mogo.Game.Vocation.Archer:
                    atlasName = "MogoArcherSkillIcon.prefab";
                    break;

                case Mogo.Game.Vocation.Assassin:
                    atlasName = "MogoAssasinSkillIcon.prefab";
                    break;

                case Mogo.Game.Vocation.Mage:
                    atlasName = "MogoMageSkillIcon.prefab";
                    break;

                case Mogo.Game.Vocation.Warrior:
                    atlasName = "MogoWarriorSkillIcon.prefab";
                    break;
            }

            AssetCacheMgr.GetUIInstance(atlasName, (prefab1, guid1, gameObject1) =>
            {
                GameObject go1 = (GameObject)gameObject1;

                MogoUIManager.Instance.SkillIconAtlas = go1.GetComponentInChildren<UIAtlas>();
                go1.hideFlags = HideFlags.HideAndDontSave;

            });
        }

        if (CommonAtlas == null)
        {
            AssetCacheMgr.GetUIInstance("MogoUI.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;

                CommonAtlas = go.GetComponentInChildren<UIAtlas>();
                go.hideFlags = HideFlags.HideAndDontSave;
            });
        }

        if (CommonExtraAtlas == null)
        {
            AssetCacheMgr.GetUIInstance("MogoUIExtra.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;

                CommonExtraAtlas = go.GetComponentInChildren<UIAtlas>();
                go.hideFlags = HideFlags.HideAndDontSave;
            });
        }

        if (MogoNormalMainUIAtlas == null)
        {
            AssetCacheMgr.GetUIInstance("MogoNormalMainUI.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;

                MogoNormalMainUIAtlas = go.GetComponentInChildren<UIAtlas>();
                go.hideFlags = HideFlags.HideAndDontSave;
            });
        }

        if (m_listSpcecityAtlas.Count == 0)
        {

            AssetCacheMgr.GetUIInstance(MogoUIManager.Instance.GetAtlasNameByJob() + "20EquipUI.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;

                go.name = MogoUIManager.Instance.GetAtlasNameByJob() + "20EquipUI";
                m_listSpcecityAtlas.Add(go.GetComponentInChildren<UIAtlas>());

                go.hideFlags = HideFlags.HideAndDontSave;

                if (SystemSwitch.DestroyResource)
                {
                    m_listSpcecityAtlas[m_listSpcecityAtlas.Count - 1].spriteMaterial.mainTexture = null;
                    AssetCacheMgr.ReleaseResourceImmediate(string.Concat(MogoUIManager.Instance.GetAtlasNameByJob(),
                        "20EquipUI.png"));
                }

            });



            AssetCacheMgr.GetUIInstance(MogoUIManager.Instance.GetAtlasNameByJob() + "40EquipUI.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;
                go.name = MogoUIManager.Instance.GetAtlasNameByJob() + "40EquipUI";

                m_listSpcecityAtlas.Add(go.GetComponentInChildren<UIAtlas>());

                go.hideFlags = HideFlags.HideAndDontSave;

                if (SystemSwitch.DestroyResource)
                {
                    m_listSpcecityAtlas[m_listSpcecityAtlas.Count - 1].spriteMaterial.mainTexture = null;
                    AssetCacheMgr.ReleaseResourceImmediate(string.Concat(MogoUIManager.Instance.GetAtlasNameByJob(),
                        "40EquipUI.png"));
                }

            });
        }
    }

    public void PreLoadResources(string[] resName, Action<UnityEngine.Object[]> callBack)
    {
        AssetCacheMgr.GetUIResources(resName, callBack);
    }

    public void PreLoadResource(string resName, Action<UnityEngine.Object> callBack)
    {
        //var sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        AssetCacheMgr.GetUIResource(resName, (obj) =>
        {
            //sw.Stop();
            //LoggerHelper.Info("PreLoadResource: " + resName + " " + sw.ElapsedMilliseconds);
            if (callBack != null)
                callBack(obj);
        });
    }

    #endregion

    #region ж��UI

    public void ReleaseMogoUI()
    {
        Mogo.Util.LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ReleaseMogoUI " + (InstanceUIViewManager.Instance == null));
        if (InstanceUIViewManager.Instance != null)
        {
            hasLoadedInstanceUI = false;
            InstanceUIViewManager.Instance.Release(
                () =>
                {
                    //hasLoadedInstanceUI = false;
                });
        }

        if (BattleMenuUIViewManager.Instance != null)
        {
            BattleMenuUIViewManager.Instance.Release();
        }

        if (BillboardViewManager.Instance != null)
        {
            BillboardViewManager.Instance.Release();
        }

        if (SanctuaryUIViewManager.Instance != null)
        {
            SanctuaryUIViewManager.Instance.Release();
        }

        //ChooseCharacterUIViewManager.Instance.Release();
        //CreateCharacterUIViewManager.Instance.Release();
        LoginUIViewManager.Instance.Release();

        if (MainUIViewManager.Instance != null)
        {
            MainUIViewManager.Instance.Release();
        }

        if (MenuUIViewManager.Instance != null)
        {
            MenuUIViewManager.Instance.Release();
        }

        if (NormalMainUIViewManager.Instance != null)
        {
            NormalMainUIViewManager.Instance.Release();
        }

        if (TaskUIViewManager.Instance != null)
        {
            TaskUIViewManager.Instance.Release();
        }

        if (StrenthenUIViewManager.Instance != null)
        {

            StrenthenUIViewManager.Instance.Release();
        }

        if (ComposeUIViewManager.Instance != null)
        {
            ComposeUIViewManager.Instance.Release();
        }

        if (DecomposeUIViewManager.Instance != null)
        {
            DecomposeUIViewManager.Instance.Release();
        }

        if (InsetUIViewManager.Instance != null)
        {
            InsetUIViewManager.Instance.Release();
        }

        if (RuneUIViewManager.Instance != null)
        {
            RuneUIViewManager.Instance.Release();
        }

        if (DragonUIViewManager.Instance != null)
        {
            DragonUIViewManager.Instance.Release();
        }

        //ChooseServerUIViewManager.Instance.Release();

        if (SkillUIViewManager.Instance != null)
        {
            SkillUIViewManager.Instance.Release();
        }

        if (CommunityUIViewManager.Instance != null)
        {
            CommunityUIViewManager.Instance.Release();
        }

        if (SocietyUIViewManager.Instance != null)
        {
            SocietyUIViewManager.Instance.Release();
        }

        if (AssistantUIViewManager.Instance != null)
        {
            AssistantUIViewManager.Instance.Release();
        }

        if (SettingsUIViewManager.Instance != null)
        {
            SettingsUIViewManager.Instance.Release();
        }

        if (DoorOfBuryUIViewManager.Instance != null)
        {
            DoorOfBuryUIViewManager.Instance.Release();
        }

        if (ChallengeUIViewManager.Instance != null)
        {
            ChallengeUIViewManager.Instance.Release();
        }

        if (EquipmentUIViewManager.Instance != null)
        {
            EquipmentUIViewManager.Instance.Release();
        }

        if (NewLoginUIViewManager.Instance != null)
        {
            NewLoginUIViewManager.Instance.Release();
        }

        if (ClimbTowerUILogicManager.Instance != null)
        {
            ClimbTowerUILogicManager.Instance.Release();
        }

        if (OperatingUIViewManager.Instance != null)
        {
            OperatingUIViewManager.Instance.Release();
        }

        if (BillboardViewManager.Instance != null)
        {
            BillboardViewManager.Instance.Release();
        }

        if (ChargeRewardUIViewManager.Instance != null)
        {
            ChargeRewardUIViewManager.Instance.Release();
        }

        if (LoginRewardUIViewManager.Instance != null)
        {
            LoginRewardUIViewManager.Instance.Release();
        }

        if (AttributeRewardUIViewManager.Instance != null)
        {
            AttributeRewardUIViewManager.Instance.Release();
        }

        if (TimeLimitActivityUIViewManager.Instance != null)
        {
            TimeLimitActivityUIViewManager.Instance.Release();
        }

        if (MogoOKCancelBoxQueue.Instance != null)
        {
            MogoOKCancelBoxQueue.Instance.Release();
        }

        if (MogoUIQueue.Instance != null)
        {
            MogoUIQueue.Instance.Release();
        }

        if (TongUILogicManager.Instance != null)
        {
            TongUILogicManager.Instance.Release();
        }

        if (TeachUIViewManager.Instance != null)
        {
            TeachUIViewManager.Instance.Release();
        }

        if (EnergyUIViewManager.Instance != null)
        {
            EnergyUIViewManager.Instance.Release();
        }

        if (DiamondToGoldUIViewManager.Instance != null)
        {
            DiamondToGoldUIViewManager.Instance.Release();
        }

        //GetComponentsInChildren<MyServer>(true)[0].Release();
    }

    public void DestroyNewLoginUI()
    {
        IsNewLoginUILoaded = false;
        NewLoginUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_NewLoginUI);
        m_NewLoginUI = null;
    }

    public void DestroySkillUI()
    {
        IsSkillUILoaded = false;
        SkillUIViewManager.Instance.Release();
        //AssetCacheMgr.ReleaseInstance(SkillIconAtlas.gameObject);
        AssetCacheMgr.ReleaseInstance(m_SkillUI);
        //AssetCacheMgr.ReleaseInstance(SkillIconAtlas.gameObject);
        //SkillIconAtlas = null;
        //Destroy(m_SkillUI);
        m_SkillUI = null;
    }

    public void DestroySocietyUI()
    {
        IsSocialUILoaded = false;
        SocietyUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_SocietyUI);
        m_SocietyUI = null;

    }

    public void DestroySettingsUI()
    {
        IsSettingsUILoaded = false;
        SettingsUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_SettingUI);
        m_SettingUI = null;
    }

    public void DestroyMenuUI()
    {
        IsMenuUILoaded = false;
        MenuUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_MenuUI);
        m_MenuUI = null;
    }

    public void DestroyChargeRewardUI()
    {
        IsChargeRewardUILoaded = false;
        ChargeRewardUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_ChargeRewardUI);
        m_ChargeRewardUI = null;
    }

    public void DestroyAttributeRewardUI()
    {
        IsAttributeRewardUILoaded = false;
        AttributeRewardUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_AttributeRewardUI);
        m_AttributeRewardUI = null;
    }

    public void DestroyTimeLimitActivityUI()
    {
        IsTimeLimitActivityUILoaded = false;
        TimeLimitActivityUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_TimeLimitActivityUI);
        m_TimeLimitActivityUI = null;
    }

    public void DestroyLoginRewardUI()
    {
        IsLoginRewardUILoaded = false;
        LoginRewardUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_LoginRewardUI);
        m_LoginRewardUI = null;
    }

    public void DestroyStrenthUI()
    {
        IsStrenthUILoaded = false;
        StrenthenUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_StrenthUI);
        m_StrenthUI = null;
    }

    public void DestroyInsetUI()
    {
        IsInsetUILoaded = false;
        InsetUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_InsetUI);
        m_InsetUI = null;
    }

    public void DestroyComposeUI()
    {
        IsComposeUILoaded = false;
        ComposeUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_ComposeUI);
        m_ComposeUI = null;
    }

    public void DestroyDecomposeUI()
    {
        IsDecomposeUILoaded = false;
        DecomposeUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_DecomposeUI);
        m_DecomposeUI = null;
    }

    public void DestroyDiamondToGoldUI()
    {
        IsDiamondToGoldUILoaded = false;
        DiamondToGoldUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_DiamondToGoldUI);
        m_DiamondToGoldUI = null;
    }

    public void DestroyEnergyUI()
    {
        IsEnergyUILoaded = false;
        EnergyUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_EnergyUI);
        m_EnergyUI = null;
    }

    public void DestroyMarketUI()
    {
        IsMarketUILoaded = false;
        MarketView.Instance.Remove();
        AssetCacheMgr.ReleaseInstance(m_MarketUI);
        m_MarketUI = null;
    }

    public void DestroyRankingUI()
    {
        IsRankingUILoaded = false;
        RankingUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_RankingUI);
        m_RankingUI = null;
    }

    public void DestroyRankingRewardUI()
    {
        IsRankingRewardUILoaded = false;
        RankingRewardUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_RankingRewardUI);
        m_RankingRewardUI = null;
    }

    public void DestroyUpgradePowerUI()
    {
        IsUpgradePowerUILoaded = false;
        UpgradePowerUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_UpgradePowerUI);
        m_UpgradePowerUI = null;
    }

    public void DestroyEquipRecommendUI()
    {
        IsEquipRecommendUILoaded = false;
        EquipRecommendUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_EquipRecommendUI);
        m_EquipRecommendUI = null;
    }

    public void DestroyInstanceBossTreasureUI()
    {
        IsInstanceBossTreasureUILoaded = false;
        InstanceBossTreasureUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_InstanceBossTreasureUI);
        m_InstanceBossTreasureUI = null;
    }

    public void DestroyLevelNoEnoughUI()
    {
        IsLevelNoEnoughUILoaded = false;
        LevelNoEnoughUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_LevelNoEnoughUI);
        m_LevelNoEnoughUI = null;
    }

    public void DestroyEnergyNoEnoughUI()
    {
        IsEnergyNoEnoughUILoaded = false;
        EnergyNoEnoughUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_EnergyNoEnoughUI);
        m_EnergyNoEnoughUI = null;
    }

    public void DestroyDragonMatchUI()
    {
        IsIDragonMatchUILoaded = false;
        DragonMatchUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_DragonMatchUI);
        m_DragonMatchUI = null;
    }

    public void DestroyDragonMatchRecordUI()
    {
        IsDragonMatchRecordUILoaded = false;
        DragonMatchRecordUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_DragonMatchRecordUI);
        m_DragonMatchRecordUI = null;
    }

    public void DestroyChooseDragonUI()
    {
        IsChooseDragonUILoaded = false;
        ChooseDragonUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_ChooseDragonUI);
        m_ChooseDragonUI = null;
    }

    public void DestroyOKCancelTipUI()
    {
        IsOKCancelTipUILoaded = false;
        OKCancelTipUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_OKCancelTipUI);
        m_OKCancelTipUI = null;
    }

    public void DestroySpriteUI()
    {
        IsSpriteUILoaded = false;
        SpriteUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_SpriteUI);
        m_SpriteUI = null;
    }

    public void DestroyOccupyTowerPassUI()
    {
        IsOccupyTowerPassUILoaded = false;
        OccupyTowerPassUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_OccupyTowerPassUI);
        m_OccupyTowerPassUI = null;
    }

    public void DestroyOccupyTowerUI()
    {
        IsOccupyTowerUILoaded = false;
        OccupyTowerUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_OccupyTowerUI);
        m_OccupyTowerUI = null;
    }

    public void DestroyLoginUI()
    {
        IsLoginUILoaded = false;
        LoginUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_LoginUI);
        m_LoginUI = null;
    }

    public void DestroyNewArenaUI()
    {
        IsNewArenaUILoaded = false;
        NewArenaUIViewManager.Instance.Release();
        AssetCacheMgr.ReleaseInstance(m_NewArenaUI);
        m_NewArenaUI = null;
    }

    #endregion  
}