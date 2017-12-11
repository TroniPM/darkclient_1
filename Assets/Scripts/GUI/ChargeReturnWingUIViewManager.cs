using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChargeReturnWingUIViewManager : MFUIUnit
{
    private static ChargeReturnWingUIViewManager m_instance;

    public static ChargeReturnWingUIViewManager Instance
    {
        get
        {
            return ChargeReturnWingUIViewManager.m_instance;
        }
    }


    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.ChargeReturnWingUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "ChargeReturnWingUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(ChargeReturnWingUILogicManager.Instance);

        RegisterButtonHandler("ChargeReturnWingUIChargeBtn");
        SetButtonClickHandler("ChargeReturnWingUIChargeBtn", OnChargeBtnUp);

        RegisterButtonHandler("ChargeReturnWingUIItem0");
        SetButtonClickHandler("ChargeReturnWingUIItem0", OnItem0Up);

        RegisterButtonHandler("ChargeReturnWingUIItem1");
        SetButtonClickHandler("ChargeReturnWingUIItem1", OnItem1Up);

        RegisterButtonHandler("ChargeReturnWingUIItem0GetBtn");
        SetButtonClickHandler("ChargeReturnWingUIItem0GetBtn", OnItem0GetBtnUp);

        RegisterButtonHandler("ChargeReturnWingUIItem1GetBtn");
        SetButtonClickHandler("ChargeReturnWingUIItem1GetBtn", OnItem1GetBtnUp);

      
    }

    public override void CallWhenUnloadResources()
    {
        m_instance = null;
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);
    }

    void OnChargeBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.OtherEvent.Charge);
    }

    void OnItem0Up(int id)
    {
        Debug.LogError("Item0");
    }

    void OnItem1Up(int id)
    {
        Debug.LogError("Item1");
    }

    void OnItem0GetBtnUp(int id)
    {
        Debug.LogError("Get0Btn");
    }

    void OnItem1GetBtnUp(int id)
    {
        Debug.LogError("Get1Btn");
    }

    public void SetTopWingIcon(string iconName)
    {
        SetSpriteImage("ChargeReturnWingUIItem0FG", iconName);
    }

    public void SetBottomWingIcon(string iconName)
    {
        SetSpriteImage("ChargeReturnWingUIItem1FG",iconName);
    }

    public void ShowItem0GetBtn(bool isShow)
    {
        GetTransform("ChargeReturnWingUIItem0GetBtn").gameObject.SetActive(isShow);
    }

    public void ShowItem1GetBtn(bool isShow)
    {
        GetTransform("ChargeReturnWingUIItem1GetBtn").gameObject.SetActive(isShow);
    }
}
