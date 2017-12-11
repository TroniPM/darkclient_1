using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class IAPUIController
{
    static IAPUIController m_IAPController = null;
    public static IAPUIController Singleton
    {
        get
        {
            if (null == m_IAPController)
            {
                m_IAPController = new IAPUIController();
                m_IAPController.Init();
            }
            return m_IAPController;
        }
    }
    IUIView     m_IAPView = null;
    GameObject m_IAPViewPanel = null;
    GameObject m_PrevUI = null;

    [DllImport("__Internal")]
    extern static void Buy(int nID,byte [] CallBack);

    void Init()
    {
        Mogo.Util.EventDispatcher.AddEventListener(IAPEvents.ShowIAPView, ShowIAPView);
        Mogo.Util.EventDispatcher.AddEventListener(IAPEvents.HideIAPView, HideIAPView);
        Mogo.Util.EventDispatcher.AddEventListener<int>(IAPEvents.BuySomething, BuySomething);
        Mogo.Util.EventDispatcher.AddEventListener(Mogo.Util.Events.OperationEvent.Charge, ShowIAPView);
    }

    void ShowIAPView()
    {
        if (null==m_IAPViewPanel)
        {
            AssetCacheMgr.GetResources(new string[] { "IAPView.prefab" }, (objs) =>
            {
                m_IAPViewPanel = GameObject.Instantiate(objs[0]) as GameObject;
                m_IAPViewPanel.transform.Find("Goods/RMB6/IAPGood").gameObject.AddComponent<IAPGood>().ID = 0;
                m_IAPViewPanel.transform.Find("Goods/RMB30/IAPGood").gameObject.AddComponent<IAPGood>().ID = 1;
                m_IAPViewPanel.transform.Find("Goods/RMB68/IAPGood").gameObject.AddComponent<IAPGood>().ID = 2;
                m_IAPViewPanel.transform.Find("Goods/RMB198/IAPGood").gameObject.AddComponent<IAPGood>().ID = 3;
                m_IAPViewPanel.transform.Find("Goods/RMB328/IAPGood").gameObject.AddComponent<IAPGood>().ID = 4;
                m_IAPViewPanel.transform.Find("Goods/RMB648/IAPGood").gameObject.AddComponent<IAPGood>().ID = 5;
                m_IAPViewPanel.transform.parent = GameObject.Find("MogoMainUI/Camera/Anchor/MogoMainUIPanel").transform;
                m_IAPViewPanel.transform.localPosition = Vector3.zero;
                m_IAPViewPanel.transform.localScale = new Vector3(1, 1, 1);
                m_IAPViewPanel.transform.Find("CloseButton").GetComponent<UIButtonMessage>().target = m_IAPViewPanel.AddComponent<IAPView>().gameObject;
                m_IAPViewPanel.transform.Find("CloseButton").GetComponent<UIButtonMessage>().functionName = "Hide";
                m_PrevUI = MogoUIManager.Instance.CurrentUI;
                MogoUIManager.Instance.CurrentUI.SetActive(false);
                MogoUIManager.Instance.CurrentUI = m_IAPViewPanel;
            });
        }
        else
        {
            m_IAPViewPanel.SetActive(true);
            m_PrevUI = MogoUIManager.Instance.CurrentUI;
            MogoUIManager.Instance.CurrentUI.SetActive(false);
            MogoUIManager.Instance.CurrentUI = m_IAPViewPanel;
        }
    }

    void HideIAPView()
    {
        if (null!=m_IAPViewPanel)
        {
            m_IAPViewPanel.SetActive(false);
        }
        if (null!=m_PrevUI)
        {
            m_PrevUI.SetActive(true);
            MogoUIManager.Instance.CurrentUI = m_PrevUI;
            m_PrevUI = null;
        }
    }

    void BuySomething(int nID)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        Buy(nID, System.Text.Encoding.Default.GetBytes("CallBack"));
#endif
    }
}
