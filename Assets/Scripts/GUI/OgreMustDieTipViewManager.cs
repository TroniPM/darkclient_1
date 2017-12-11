using UnityEngine;
using System.Collections;
using Mogo.Util;

public class OgreMustDieTipViewManager : MFUIUnit
{
    private static OgreMustDieTipViewManager m_instance;

    public static OgreMustDieTipViewManager Instance
    {
        get
        {
            return OgreMustDieTipViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.OgreMustDieTip;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "OgreMustDieTip";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(OgreMustDieTipLogicManager.Instance);
        RegisterButtonHandler("OgreMustDieOpenTipClose");
        RegisterButtonHandler("OgreMustDieOpenTipCreate");
        RegisterButtonHandler("OgreMustDieOpenTipEnter");

        SetButtonClickHandler("OgreMustDieOpenTipClose", OnCloseBtnUp);
        SetButtonClickHandler("OgreMustDieOpenTipCreate", OnCreateBtnUp);
        SetButtonClickHandler("OgreMustDieOpenTipEnter", OnEnterBtnUp);
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    public override void CallWhenDestroy()
    {
        m_instance = null;
    }

    void OnCloseBtnUp(int id)
    {

        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        MogoUIManager.Instance.ShowMogoNormalMainUI();
        //NormalMainUIViewManager.Instance.SetUIDirty();
    }

    void OnCreateBtnUp(int id)
    {
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.InvitFriendMessageBox);
        InvitFriendMessageBoxLogicManager.Instance.SetUIDirty();
    }

    void OnEnterBtnUp(int id)
    {
        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        //NormalMainUIViewManager.Instance.SetUIDirty();
        // MogoUIManager.Instance.ShowMogoNormalMainUI();

        EventDispatcher.TriggerEvent(Events.CampaignEvent.JoinCampaign);
        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EnterWaittingMessageBox, MFUIManager.MFUIID.None, 0, true);
        //EnterWaittingMessageBoxLogicManager.Instance.SetUIDirty();
    }

    public void HideSelf()
    {
        CallWhenHide();
    }
}
