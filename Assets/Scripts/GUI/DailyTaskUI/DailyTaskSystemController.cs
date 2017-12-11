using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class DailyTaskSystemController : IDailyTaskController
{
    bool m_bShowNotification = false;
    bool m_bBeenInited = false;
    GameObject m_MogoMainUIPanel = null;
    GameObject m_DailyTaskUIPanel = null;
    DailyTaskIcon m_DailyTaskIcon = null;
    DailyTaskFXRoot m_DailyTaskFXRoot = null;
    IUIView     m_DailyTaskView = null;

    public void UpdateView<T>(List<T> datas)
    {
        LoggerHelper.Debug("data count " + datas.Count);
        if (m_DailyTaskView != null && m_DailyTaskUIPanel.activeSelf)
        {
            Driver.Instance.StopCoroutine("TryToUpdateDailyTaskView");
            Driver.Instance.StartCoroutine(TryToUpdateDailyTaskView(datas as List<DailyTaskInfo>));
        }
    }

    public void OnOpenDailyTaskUI()
    {
        EventDispatcher.TriggerEvent<bool>(Events.MogoGlobleUIManagerEvent.ShowWaitingTip, true);
        LoggerHelper.Debug("OnMainUIDailyTaskIconPressed");
        if (!m_bBeenInited && m_DailyTaskUIPanel == null)
        {
            LoggerHelper.Debug("OnMainUIDailyTaskIconPressed 1");
            m_bBeenInited = true;
            AssetCacheMgr.GetResources(new string[] { "DailyTaskUI.prefab", "Task.prefab" }, (objs) =>
            {
                LoggerHelper.Debug("inside the call back AssetCacheMgr.GetResources( DailyTaskUI.prefab Task.prefab");
                InitDailyTaskView(objs);
            });
        }
        else
        {
            LoggerHelper.Debug("OnMainUIDailyTaskIconPressed 2");
            if (m_DailyTaskUIPanel != null)
                AllPanelIsReady();
        }
    }

    public void OnDailyTaskPanelCloseButtonPressed()
    {
        Driver.Instance.StopAllCoroutines();
        EventDispatcher.TriggerEvent<GameObject>(Events.MogoUIManagerEvent.SetCurrentUI, m_MogoMainUIPanel);
        m_MogoMainUIPanel.SetActive(!m_MogoMainUIPanel.activeSelf);
        m_DailyTaskUIPanel.SetActive(!m_DailyTaskUIPanel.activeSelf);
        if (SystemSwitch.DestroyAllUI)
        {
            AssetCacheMgr.SynReleaseInstance(m_DailyTaskUIPanel);
            m_DailyTaskUIPanel = null;
            m_MogoMainUIPanel = null;
            m_DailyTaskView = null;
            m_DailyTaskFXRoot = null;
            AssetCacheMgr.ReleasesResource(new string[] { "DailyTaskUI.prefab", "Task.prefab" });
            m_bBeenInited = false;
        }
    }

    public void ShowNotification()
    {
        if (m_DailyTaskIcon != null)
        {
            m_DailyTaskIcon.ShowDailyTaskFinishedNotice();
        }
        else
        {
            m_DailyTaskIcon = GameObject.Find("MogoMainUI").GetComponentsInChildren<DailyTaskIcon>(true)[0];
            if (null == m_DailyTaskIcon) LoggerHelper.Error("ShowNotification m_DailyTaskIcon is null!");
        }
    }

    public void HideNotification()
    {
        if (m_DailyTaskIcon != null)
        {
            m_DailyTaskIcon.HideDailyTaskFinishedNotice();
        }
        else
        {
            m_DailyTaskIcon = GameObject.Find("MogoMainUI").GetComponentsInChildren<DailyTaskIcon>(true)[0];
            if (null == m_DailyTaskIcon) LoggerHelper.Error("ShowNotification m_DailyTaskIcon is null!");
        }
    }

    public void RegisterDailyTaskIcon(DailyTaskIcon dti)
    {
        m_DailyTaskIcon = dti;
    }

    public void JumpTo(int nParam)
    {
        LoggerHelper.Debug("DailyTaskController Jump to :line 94:" + nParam);
        OnDailyTaskPanelCloseButtonPressed();
        switch (nParam)
        {
            case 11:
                EventDispatcher.TriggerEvent<bool>(Events.MogoUIManagerEvent.ShowInstanceMissionChooseUI, true);
                break;
            case 12:
                EventDispatcher.TriggerEvent<bool>(Events.MogoUIManagerEvent.ShowInstanceMissionChooseUI, true);
                break;
            case 13:
                EventDispatcher.TriggerEvent<bool>(Events.MogoUIManagerEvent.ShowInstanceMissionChooseUI, true);
                break;
            case 14:
                EventDispatcher.TriggerEvent<bool>(Events.MogoUIManagerEvent.ShowInstanceMissionChooseUI, true);
                break;
            case 15:
                EventDispatcher.TriggerEvent(Events.MogoUIManagerEvent.SwitchStrenthUI);
                break;
            case 16:
                EventDispatcher.TriggerEvent<System.Action>(Events.MogoUIManagerEvent.ShowDiamondToGoldUI, null);
                break;
            case 17:
                EventDispatcher.TriggerEvent(Events.NormalMainUIViewManagerEvent.PVEPLAYICONUP);
                break;
            case 18:
                EventDispatcher.TriggerEvent(Events.NormalMainUIViewManagerEvent.PVEPLAYICONUP);
                break;
            case 19:
                EventDispatcher.TriggerEvent(Events.NormalMainUIViewManagerEvent.PVEPLAYICONUP);
                break;
            case 20:
                EventDispatcher.TriggerEvent(Events.NormalMainUIViewManagerEvent.PVEPLAYICONUP);
                break;
            case 21:
                EventDispatcher.TriggerEvent<System.Action>(Events.MogoUIManagerEvent.ShowEnergyUI, null);
                break;
            case 22:
                EventDispatcher.TriggerEvent<int>(Events.MFUIManagerEvent.SwitchUIWithLoad, 3);
                break;
            case 23:
                EventDispatcher.TriggerEvent(Events.NormalMainUIViewManagerEvent.PVPPLAYICONUP);
                break;
            case 24:
                EventDispatcher.TriggerEvent<int>(Events.MogoUIManagerEvent.SwitchToMarket, 0);
                break;
            case 25:
                EventDispatcher.TriggerEvent<int, int>(Events.ComposeManagerEvent.SwitchToCompose, 0, 0);
                break;
            default:
                LoggerHelper.Error("cannot jump to id " + nParam);
                break;
        }
    }

    static DailyTaskSystemController m_Singleton = null;
    public static DailyTaskSystemController Singleton
    {
        get
        {
            if (null == m_Singleton)
            {
                m_Singleton = new DailyTaskSystemController();
            }
            return m_Singleton;
        }
    }

    #region private method
    IEnumerator TryToUpdateDailyTaskView(List<DailyTaskInfo> datas)
    {
        while (m_DailyTaskFXRoot.ChildCount > 0)
        {
            yield return null;
        }
        m_DailyTaskView.CleanUp();
        m_DailyTaskView.UpdateView(datas);
    }

    void AllPanelIsReady()
    {
        m_MogoMainUIPanel.SetActive(false);
        m_DailyTaskUIPanel.SetActive(true);
        EventDispatcher.TriggerEvent<GameObject>(Events.MogoUIManagerEvent.SetCurrentUI, m_DailyTaskUIPanel);
        GameObject.FindGameObjectWithTag("DailyTaskContainerCamera").transform.localPosition = new Vector3(0, 0, 0);
        EventDispatcher.TriggerEvent(Events.DailyTaskEvent.ShowDailyEvent);
        EventDispatcher.TriggerEvent("EntityMyself.ON_END_TASK_GUIDE");
        LoggerHelper.Debug("show daily task panel!");
        EventDispatcher.TriggerEvent<bool>(Events.MogoGlobleUIManagerEvent.ShowWaitingTip, false);
    }

    void InitDailyTaskView<T>(T[] resources)
    {
        m_DailyTaskUIPanel = AssetCacheMgr.SynGetInstance("DailyTaskUI.prefab") as GameObject;
        m_MogoMainUIPanel = GameObject.FindGameObjectWithTag("NormalMainUI");
        m_DailyTaskUIPanel.transform.parent = GameObject.FindGameObjectWithTag("MogoMainUIPanel").transform;
        m_DailyTaskUIPanel.transform.localScale = new Vector3(1, 1, 1);
        m_DailyTaskUIPanel.transform.localPosition = new Vector3(0, 0, 0);
        GameObject.FindGameObjectWithTag("DailyTaskContainerCamera").GetComponent<UIViewport>().sourceCamera = GameObject.Find("MogoMainUI/Camera").GetComponent<Camera>();
        m_DailyTaskFXRoot = GameObject.FindGameObjectWithTag("DailyTaskFXRoot").AddComponent<DailyTaskFXRoot>();
        GameObject.FindGameObjectWithTag("DailyTaskFXCamera").GetComponent<UIViewport>().sourceCamera = GameObject.Find("MogoMainUI/Camera").GetComponent<Camera>();
        GameObject.FindGameObjectWithTag("DailyTaskPanelCloseButton").AddComponent<DailyTaskCloseButton>();
        m_DailyTaskView = GameObject.FindGameObjectWithTag("DailyTaskLists").AddComponent<DailyTaskView>();
        AllPanelIsReady();
    }
    #endregion
}



public interface IDailyTaskController
{

}
