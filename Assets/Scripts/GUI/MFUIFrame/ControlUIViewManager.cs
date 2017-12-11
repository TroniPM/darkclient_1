using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlUIViewManager : MFUIUnit
{
    void OnTestPanelBtnClick(int id)
    {

        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.CityMainUI);
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnTestPanel1BtnClick(int id)
    {
        
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattleMainUI);
    }

    void OnTestLoaded()
    {

        goTest1 = MFUIGameObjectPool.GetSingleton().GetGameObject("goTest1");
        MFUIUtils.AttachWidget(goTest1.transform, m_myTransform.parent);
    }

    void OnTest1Loaded()
    {

        goTest = MFUIGameObjectPool.GetSingleton().GetGameObject("goTest");
        MFUIUtils.AttachWidget(goTest.transform, m_myTransform.parent);
    }

    public override void CallWhenCreate()
    {
        RegisterButtonHandler("TestPanelBtn");
        RegisterButtonHandler("TestPanel1Btn");

        SetButtonClickHandler("TestPanelBtn", OnTestPanelBtnClick);
        SetButtonClickHandler("TestPanel1Btn", OnTestPanel1BtnClick);

    }

    public override void CallWhenLoadResources()
    {
        ID = MFUIManager.MFUIID.None;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    GameObject goTest;
    GameObject goTest1;
}
