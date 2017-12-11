using UnityEngine;
using System.Collections;

public class InvitFriendMessageBoxViewManager : MFUIUnit
{

    private static InvitFriendMessageBoxViewManager m_instance;

    public static InvitFriendMessageBoxViewManager Instance
    {
        get
        {
            return InvitFriendMessageBoxViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        AttachLogicUnit(InvitFriendMessageBoxLogicManager.Instance);
        ID = MFUIManager.MFUIID.InvitFriendMessageBox;
        m_myGameObject.name = "InvitFriendMessageBox";
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        RegisterButtonHandler("InvitFriendMessageBoxClose");
        RegisterButtonHandler("InvitFriendMessageBoxOK");
        RegisterButtonHandler("InvitFriendMessageBoxCancel");

        SetButtonClickHandler("InvitFriendMessageBoxClose", OnCloseUp);
        SetButtonClickHandler("InvitFriendMessageBoxOK", OnOKUp);
        SetButtonClickHandler("InvitFriendMessageBoxCancel", OnCancelUp);
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

    public void SetCostNum(string num)
    {
        SetLabelText("InvitFriendMessageBoxCost", string.Concat("* ", num));
    }

    void OnCloseUp(int id)
    {
        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        MogoUIManager.Instance.ShowMogoNormalMainUI();
        NormalMainUIViewManager.Instance.SetUIDirty();
    }

    void OnOKUp(int id)
    {
        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        MogoUIManager.Instance.ShowMogoNormalMainUI();
        NormalMainUIViewManager.Instance.SetUIDirty();
    }

    void OnCancelUp(int id)
    {
        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        MogoUIManager.Instance.ShowMogoNormalMainUI();
        NormalMainUIViewManager.Instance.SetUIDirty();
    }
	
}
