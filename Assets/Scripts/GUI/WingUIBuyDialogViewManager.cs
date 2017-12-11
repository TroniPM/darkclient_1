using UnityEngine;
using System.Collections;

public class WingUIBuyDialogViewManager : MFUIUnit
{

    private static WingUIBuyDialogViewManager m_instance;

    public static WingUIBuyDialogViewManager Instance
    {
        get
        {
            return WingUIBuyDialogViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.WingUIBuyDialog;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        m_myGameObject.name = "WingUIBuyDialog";
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);


    }

    public override void CallWhenCreate()
    {
        AttachLogicUnit(WingUIBuyDialogLogicManager.Instance);

        RegisterButtonHandler("WingUIBuyDialogCloseBtn");
        SetButtonClickHandler("WingUIBuyDialogCloseBtn", OnBuyDialogCloseBtnUp);

        RegisterButtonHandler("WingUIBuyDialogBuyBtn");
        SetButtonClickHandler("WingUIBuyDialogBuyBtn", OnBuyDialogBuyBtnUp);

    }

    public override void CallWhenUnloadResources()
    {
        m_instance = null;
    }

    public override void CallWhenShow()
    {
        MFUIUtils.ShowGameObject(true, m_myGameObject);
        //MogoMainCamera.instance.SetActive(false);
    }

    public override void CallWhenHide()
    {
        //DisablePlayerModel();
        MFUIUtils.ShowGameObject(false, m_myGameObject);
        //MogoMainCamera.instance.SetActive(true);
        //MFUIGameObjectPool.GetSingleton().DestroyGameObject(m_myGameObject);
    }

    void OnBuyDialogCloseBtnUp(int id)
    {
        Debug.LogError("BuyDialogCloseBtnUp");
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.None);
        //WingUILogicManager.Instance.SetUIDirty();
    }

    void OnBuyDialogBuyBtnUp(int id)
    {
        Mogo.Util.EventDispatcher.TriggerEvent(MarketEvent.WingBuy);
    }

    public void SetBuyDialodTitle(string title)
    {
        SetLabelText("WingUIBuyDialogTitleText", title);
    }

    public void SetBuyDialogCost(string cost)
    {
        SetLabelText("WingUIBuyDialogInfoCostNum", cost);
    }

    public void SetBuyDialogAttr(string attr, int id)
    {
        SetLabelText(string.Concat("WingUIBuyDialogInfoGrid", id, "Text"), attr);
    }

    public void SetBuyDialogIcon(string imgName)
    {
        SetSpriteImage("WingUIBuyDialogIconFG", imgName);
    }

  

}
