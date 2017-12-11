using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardUIIconData
{
    public string iconText;
    public string widgetName;
    public System.Action<int> IconUpCB;
}

public class RewardUIViewManager : MFUIUnit
{
    private static RewardUIViewManager m_instance;

    public static RewardUIViewManager Instance
    {
        get
        {
            return RewardUIViewManager.m_instance;
        }
    }


    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.RewardUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "RewardUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(RewardUILogicManager.Instance);

        //RegisterButtonHandler("RewardUIChargeRewardWingIcon");
        //SetButtonClickHandler("RewardUIChargeRewardWingIcon", OnWingIconUp);

        //RegisterButtonHandler("RewardUIChargeRewardIcon");
        //SetButtonClickHandler("RewardUIChargeRewardIcon", OnChargeRewardIconUp);

        //RegisterButtonHandler("RewardUIElfDiamondIcon");
        //SetButtonClickHandler("RewardUIElfDiamondIcon", OnElfDiamondIconUp);

        //RegisterButtonHandler("RewardUILoginRewardIcon");
        //SetButtonClickHandler("RewardUILoginRewardIcon", OnLoginRewardIconUp);

        RegisterButtonHandler("OperatingUICloseBtn");
        SetButtonClickHandler("OperatingUICloseBtn", OnRewardUICloseBtnUp);

        List<RewardUIIconData> list = new List<RewardUIIconData>();
        RewardUIIconData data0 = new RewardUIIconData();
        data0.iconText = "充值送翅膀";
        data0.IconUpCB = OnWingIconUp;
        data0.widgetName = "ChargeReturnWingBtn";
        list.Add(data0);

        RewardUIIconData data1 = new RewardUIIconData();
        data1.iconText = "充值奖励";
        data1.IconUpCB = OnChargeRewardIconUp;
        data1.widgetName = "ChargeRewardBtn";
        list.Add(data1);

        RewardUIIconData data2 = new RewardUIIconData();
        data2.iconText = "精灵宝钻";
        data2.IconUpCB = OnElfDiamondIconUp;
        data2.widgetName = "ElfDiamondBtn";
        list.Add(data2);



        RewardUIIconData data4 = new RewardUIIconData();
        data4.iconText = "限时活动";
        data4.IconUpCB = OnNewTimeLimitActivityBtnUp;
        data4.widgetName = "TimeLimitActivityBtn";
        list.Add(data4);

        //RewardUIIconData data5 = new RewardUIIconData();
        //data5.iconText = "成就奖励";
        //data5.IconUpCB = OnNewAttributeRewardBtnUp;
        //data5.widgetName = "AttributeRewardBtn";
        //list.Add(data5);

        RewardUIIconData data3 = new RewardUIIconData();
        data3.iconText = "登录奖励";
        data3.IconUpCB = OnLoginRewardIconUp;
        data3.widgetName = "LoginRewardBtn";
        list.Add(data3);

        AddIconList(list);

        GetTransform("RewardUIIconListCam").GetComponentsInChildren<UIViewport>(true)[0].sourceCamera =
            MogoUIManager.Instance.GetMainUICamera();
        OnWingIconUp(0);
    }

    void OnWingIconUp(int id)
    {
        Debug.LogError("Wing");
        if (MogoUIManager.Instance.m_NewTimeLimitActivityUI != null)
            MogoUIManager.Instance.m_NewTimeLimitActivityUI.SetActive(false);

        if (MogoUIManager.Instance.m_NewAttributeRewardUI != null)
            MogoUIManager.Instance.m_NewAttributeRewardUI.SetActive(false);
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.RewardEvent.WingIcon);
    }

    void OnChargeRewardIconUp(int id)
    {
        Debug.LogError("OnChargeRewardIconUp");
        if (MogoUIManager.Instance.m_NewTimeLimitActivityUI != null)
            MogoUIManager.Instance.m_NewTimeLimitActivityUI.SetActive(false);
        if (MogoUIManager.Instance.m_NewAttributeRewardUI != null)
            MogoUIManager.Instance.m_NewAttributeRewardUI.SetActive(false);
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.RewardEvent.ChargeReward);
    }

    void OnElfDiamondIconUp(int id)
    {
        Debug.LogError("ElfDiamond");
        if (MogoUIManager.Instance.m_NewTimeLimitActivityUI != null)
            MogoUIManager.Instance.m_NewTimeLimitActivityUI.SetActive(false);
        if (MogoUIManager.Instance.m_NewAttributeRewardUI != null)
            MogoUIManager.Instance.m_NewAttributeRewardUI.SetActive(false);
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.RewardEvent.ElfDiamond);
    }

    void OnLoginRewardIconUp(int id)
    {
        Debug.LogError("LoginReward");
        if (MogoUIManager.Instance.m_NewTimeLimitActivityUI != null)
            MogoUIManager.Instance.m_NewTimeLimitActivityUI.SetActive(false);
        if (MogoUIManager.Instance.m_NewAttributeRewardUI != null)
            MogoUIManager.Instance.m_NewAttributeRewardUI.SetActive(false);
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.RewardEvent.LoginReward);
    }

    void OnRewardUICloseBtnUp(int id)
    {
        Debug.LogError("Close");
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
        RewardUILogicManager.Instance.SetUIDirty();
        if (MogoUIManager.Instance.m_NewTimeLimitActivityUI != null)
            MogoUIManager.Instance.m_NewTimeLimitActivityUI.SetActive(false);

        if (MogoUIManager.Instance.m_NewAttributeRewardUI != null)
            MogoUIManager.Instance.m_NewAttributeRewardUI.SetActive(false);

        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnNewTimeLimitActivityBtnUp(int id)
    {
        Debug.LogError("TimeLimit");
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
        RewardUILogicManager.Instance.SetUIDirty();
        if (MogoUIManager.Instance.m_NewAttributeRewardUI != null)
            MogoUIManager.Instance.m_NewAttributeRewardUI.SetActive(false);
        MogoUIManager.Instance.SwitchNewTimeLimitActivityUI(true);
    }

    void OnNewAttributeRewardBtnUp(int id)
    {
        Debug.LogError("AttributeReward");
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.RewardUI);
        RewardUILogicManager.Instance.SetUIDirty();
        if (MogoUIManager.Instance.m_NewTimeLimitActivityUI != null)
            MogoUIManager.Instance.m_NewTimeLimitActivityUI.SetActive(false);
        MogoUIManager.Instance.SwitchNewAttributeRewardUI(true);
    }

    public override void CallWhenUnloadResources()
    {
        m_instance = null;
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
        MogoUIManager.Instance.GetMainUICamera().clearFlags = CameraClearFlags.SolidColor;
        MogoMainCamera.instance.SetActive(false);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        MogoUIManager.Instance.GetMainUICamera().clearFlags = CameraClearFlags.Depth;
        MogoMainCamera.instance.SetActive(true);
    }

    List<GameObject> m_listIcon = new List<GameObject>();

    public void AddIconList(List<RewardUIIconData> list)
    {
        for(int i = 0;i < list.Count;++i)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("RewardUIIcon.prefab",(name,id,obj)=>
            {
                GameObject iconGo = (GameObject)obj;
                iconGo.transform.parent = GetTransform("RewardUIIconList");
                iconGo.transform.Find("RewardUIIconText").GetComponentsInChildren<UILabel>(true)[0].text = list[index].iconText;
                iconGo.transform.Find("RewardUIIconTextFG").GetComponentsInChildren<UILabel>(true)[0].text = list[index].iconText;
                iconGo.name = list[index].widgetName;
                iconGo.GetComponentsInChildren<MFUIButtonHandler>(true)[0].ClickHandler = list[index].IconUpCB;
                iconGo.GetComponentsInChildren<MogoSingleButton>(true)[0].ButtonListTransform = GetTransform("RewardUIIconList");
                iconGo.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = GetTransform("RewardUIIconListCam").GetComponentsInChildren<Camera>(true)[0];
                GetTransform("RewardUIIconList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Add(iconGo.GetComponentsInChildren<MogoSingleButton>(true)[0]);
                iconGo.transform.localPosition = new Vector3(0, -120 * index, 0);
                iconGo.transform.localScale = Vector3.one;

                if (list.Count <= 5)
                {

                    GetTransform("RewardUIIconListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -245f;
                }
                else
                {

                    GetTransform("RewardUIIconListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -245f - 120 * (list.Count - 5);
                }
                m_listIcon.Add(iconGo);

                if (index == 0)
                {
                    m_listIcon[0].transform.Find("RewardUIIconBGDown").gameObject.SetActive(true);
                    m_listIcon[0].transform.Find("RewardUIIconText").gameObject.SetActive(false);
                    m_listIcon[0].transform.Find("RewardUIIconTextFG").gameObject.SetActive(true);
                }
            });
        }

    }

    public void EmptyIconList()
    {
        for (int i = 0; i < m_listIcon.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listIcon[i]);
            m_listIcon[i] = null;
        }

        m_listIcon.Clear();
        GetTransform("RewardUIIconList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].SingleButtonList.Clear();
        GetTransform("RewardUIIconListCam").GetComponentsInChildren<MyDragableCamera>(true)[0].MINY = -245f;
    }
}
