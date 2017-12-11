using UnityEngine;
using System.Collections;

public class TestUIViewManager1 : MFUIUnit
{
    void OnItemButtonClick(int id)
    {
        GetSprite("TestPanel1itemImage").spriteName = "Dark";
    }

    void OnItemButtonPress(bool isPressed, int id)
    {
        if (isPressed)
        {
            GetLabel("TestPanel1itemName").text = "Down";
        }
        else
        {
            GetLabel("TestPanel1itemName").text = "Up";
        }
    }

    void OnItemButtonDrag(Vector2 dragDir, int id)
    {
        SetLabelText("TestPanel1itemName", "Fuck");
    }

    public override void CallWhenCreate()
    {

        MFUIUtils.MFUIDebug("Register CityMainUI");
        RegisterButtonHandler("TestPanel1itemButton");

        SetLabelText("TestPanel1itemName", "EquipmentName");
        SetSpriteImage("TestPanel1itemImage", "NGUI");
        SetButtonClickHandler("TestPanel1itemButton", OnItemButtonClick);
        SetButtonPressHandler("TestPanel1itemButton", OnItemButtonPress);
        SetButtonDragHandler("TestPanel1itemButton", OnItemButtonDrag);

    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);

    }

    public override void CallWhenHide()
    {
        MFUIUtils.ShowGameObject(false, m_myGameObject);

    }

    public override void CallWhenLoadResources()
    {
        ID = MFUIManager.MFUIID.CityMainUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);

    }
}
