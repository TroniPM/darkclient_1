using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElfDiamondUIViewManager : MFUIUnit
{
    private static ElfDiamondUIViewManager m_instance;

    public static ElfDiamondUIViewManager Instance
    {
        get
        {
            return ElfDiamondUIViewManager.m_instance;
        }
    }


    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.ElfDiamondUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "ElfDiamondUI";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(ElfDiamondUILogicManager.Instance);

        RegisterButtonHandler("ElfDiamondUIButton");
        SetButtonClickHandler("ElfDiamondUIButton", OnBtnUp);
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

    void OnBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(Mogo.Util.Events.OtherEvent.DiamondMine);
    }
    public void SetDiamondNumCanGet(string num)
    {
        SetLabelText("ElfDiamondUITitleNum", num);
    }

    public void SetTotalCostDiamondNum(string num)
    {
        SetLabelText("ElfDiamondUICostDiamond", num);
    }

    public void SetTotalGotDiamondNum(string num)
    {
        SetLabelText("ElfDiamondUIGotDiamond", num);
    }

    public void SetCostDiamondNum(string num)
    {
        SetLabelText("ElfDiamondUIButtonCost", num);
    }
}
