using UnityEngine;
using System.Collections;
using Mogo.GameData;

public class EnterWaittingMessageBoxViewManager : MFUIUnit
{
    private static EnterWaittingMessageBoxViewManager m_instance;

    public static EnterWaittingMessageBoxViewManager Instance
    {
        get
        {
            return EnterWaittingMessageBoxViewManager.m_instance;
        }
    }

    public override void CallWhenLoadResources()
    {
        ID = MFUIManager.MFUIID.EnterWaittingMessageBox;
        m_instance = this;

        m_myGameObject.name = "EnterWaittingMessageBox";
        AttachLogicUnit(EnterWaittingMessageBoxLogicManager.Instance);
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        MFUIUtils.AttachWidget(m_myTransform, GameObject.Find("MogoMainUIPanel").transform);
        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
    }

    public override void CallWhenCreate()
    {
        RegisterButtonHandler("EnterWaittingMessageBoxCancelButton" );
        SetButtonClickHandler("EnterWaittingMessageBoxCancelButton", OnLeftBtnUp);

        RegisterButtonHandler("EnterWaittingMessageBoxMiddleButton");
        SetButtonClickHandler("EnterWaittingMessageBoxMiddleButton", OnMiddleBtnUp);

        RegisterButtonHandler("EnterWaittingMessageBoxRematchButton");
        SetButtonClickHandler("EnterWaittingMessageBoxRematchButton", OnRightRematchBtnUp);

        RegisterButtonHandler("EnterWaittingMessageBoxReviveButton");
        SetButtonClickHandler("EnterWaittingMessageBoxReviveButton", OnRightReviveBtnUp);

        RegisterButtonHandler("EnterWaittingMessageBoxDiamondButton");
        SetButtonClickHandler("EnterWaittingMessageBoxDiamondButton", OnDiamondBtnUp);

        RegisterButtonHandler("EnterWaittingMessageBoxCloseButton");
        SetButtonClickHandler("EnterWaittingMessageBoxCloseButton", OnCloseBtnUp);

        transform.Find("EnterWaittingMessageBoxCancelButton").gameObject.SetActive(false);
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

    System.Action<int> OnLeftBtnUp;
    System.Action<int> OnMiddleBtnUp;
    System.Action<int> OnRightRematchBtnUp;
    System.Action<int> OnRightReviveBtnUp;
    System.Action<int> OnDiamondBtnUp;
    //System.Action<int> OnCloseBtnUp;

    public void SetText(string text)
    {
        SetLabelText("EnterWaittingMessageBoxText", text);
    }

    public void SetLeftBtnText(string text)
    {
        SetLabelText("EnterWaittingMessageBoxCancelText", text);
    }

    /// <summary>
    /// 重新匹配
    /// </summary>
    /// <param name="text"></param>
    public void SetRightRematchBtnText(string text)
    {
        SetLabelText("EnterWaittingMessageBoxRematchButtonText", text);
    }

    /// <summary>
    /// 复活
    /// </summary>
    /// <param name="text"></param>
    public void SetRightReviveBtnText(string text)
    {
        SetLabelText("EnterWaittingMessageBoxReviveButtonText", text);
    }

    public void SetMiddleBtnText(string text)
    {
        SetLabelText("EnterWaittingMessageBoxMiddleButtonText", text);
    }

    public void SetDiamondBtnText(string text)
    {
        SetLabelText("EnterWaittingMessageBoxDiamondButtonText", text);
    }

    public void ShowWaittingAnim(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetSprite("EnterWaittingMessageBoxLodingAnim").transform.parent.gameObject);
    }

    public void ShowLeftBtn(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform("EnterWaittingMessageBoxCancelButton").gameObject);
    }

    public void ShowCloseBtn(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform("EnterWaittingMessageBoxCloseButton").gameObject);
    }

    public void ShowRightRematchBtn(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform("EnterWaittingMessageBoxRematchButton").gameObject);
    }

    public void ShowRightReviveBtn(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform("EnterWaittingMessageBoxReviveButton").gameObject);
    }

    public void ShowMiddleBtn(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform("EnterWaittingMessageBoxMiddleButton").gameObject);
    }

    public void ShowDiamondBtn(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform("EnterWaittingMessageBoxDiamondButton").gameObject);
    }

    public void SetLeftBtnUpAction(System.Action<int> act)
    {
        OnLeftBtnUp = act;
        SetButtonClickHandler("EnterWaittingMessageBoxCancelButton", OnLeftBtnUp);
    }

    /// <summary>
    /// 重新匹配
    /// </summary>
    /// <param name="act"></param>
    public void SetRightRematchBtnUpAction(System.Action<int> act)
    {
        OnRightRematchBtnUp = act;
        SetButtonClickHandler("EnterWaittingMessageBoxRematchButton", OnRightRematchBtnUp);
    }

    /// <summary>
    /// 复活
    /// </summary>
    /// <param name="act"></param>
    public void SetRightReviveBtnUpAction(System.Action<int> act)
    {
        OnRightReviveBtnUp = act;
        SetButtonClickHandler("EnterWaittingMessageBoxReviveButton", OnRightReviveBtnUp);
    }

    public void SetMiddleBtnUpAction(System.Action<int> act)
    {
        OnMiddleBtnUp = act;
        SetButtonClickHandler("EnterWaittingMessageBoxMiddleButton", OnMiddleBtnUp);
    }

    public void SetDiamondBtnUpAction(System.Action<int> act)
    {
        OnDiamondBtnUp = act;
        SetButtonClickHandler("EnterWaittingMessageBoxDiamondButton", OnDiamondBtnUp);
    }

    /// <summary>
    /// 灰化
    /// </summary>
    /// <param name="isEnable"></param>
    public void SetLeftBtnEnable(bool isEnable)
    {
        GetButtonHandler("EnterWaittingMessageBoxCancelButton").SetEnable(isEnable);
        GetButtonHandler("EnterWaittingMessageBoxCancelButton").GetComponentsInChildren<MogoTwoStatusButton>(true)[0].SetButtonEnable(isEnable);
    }


    public void ResetAllStatus()
    {
        SetLeftBtnEnable(true);

        ShowLeftBtn(false);
        ShowRightRematchBtn(false);
        ShowRightReviveBtn(false);
        ShowMiddleBtn(false);
        ShowWaittingAnim(false);
        ShowDiamondBtn(false);

        SetLeftBtnText("");
        SetRightRematchBtnText("");
        SetRightReviveBtnText("");
        SetMiddleBtnText("");
        SetDiamondBtnText("");

        SetRightRematchBtnUpAction(null);
        SetRightReviveBtnUpAction(null);
        SetLeftBtnUpAction(null);
        SetMiddleBtnUpAction(null);
        SetDiamondBtnUpAction(null);

        ShowCloseBtn(false);
    }

    //兼容旧框架
    void OnCloseBtnUp(int id)
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
    }
}
