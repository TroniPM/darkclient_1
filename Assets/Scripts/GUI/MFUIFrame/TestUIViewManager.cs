using UnityEngine;
using System.Collections;

public class TestUIViewManager : MFUIUnit
{
    void OnItemButtonClick(int id)
    {
        GetSprite("TestPanelitemImage").spriteName = "Dark";
    }

    void OnItemButtonPress(bool isPressed, int id)
    {
        if (isPressed)
        {
            GetLabel("TestPanelitemName").text = "Down";
        }
        else
        {
            GetLabel("TestPanelitemName").text = "Up";
        }
    }

    void OnItemButtonDrag(Vector2 dragDir, int id)
    {
        SetLabelText("TestPanelitemName", "Fuck");
    }

    public override void CallWhenCreate()
    {
        MFUIUtils.MFUIDebug("Register BattleMainUI");
        RegisterButtonHandler("TestPanelitemButton");

        SetLabelText("TestPanelitemName", "EquipmentName");
        SetSpriteImage("TestPanelitemImage", "NGUI");
        SetButtonClickHandler("TestPanelitemButton", OnItemButtonClick);
        SetButtonPressHandler("TestPanelitemButton", OnItemButtonPress);
        SetButtonDragHandler("TestPanelitemButton", OnItemButtonDrag);

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
        ID = MFUIManager.MFUIID.BattleMainUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);

    }
}
