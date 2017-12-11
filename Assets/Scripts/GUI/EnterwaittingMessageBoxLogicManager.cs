using UnityEngine;
using System.Collections;

public class EnterWaittingMessageBoxLogicManager : MFUILogicUnit
{

    static EnterWaittingMessageBoxLogicManager m_instance;

    public static EnterWaittingMessageBoxLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EnterWaittingMessageBoxLogicManager();
            }

            return EnterWaittingMessageBoxLogicManager.m_instance;
        }
    }

    System.Action act;
    public int currentCode = 0;

    public void SetUIDirty(int specialCode = 0)
    {
        currentCode = specialCode;

        act = MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance, () =>
        {
            EnterWaittingMessageBoxViewManager.Instance.SetUIDirty();
        });
    }

    public override void FillBufferedData()
    {
        if (act != null)
            act();

        if (m_actResetStatus != null)
            m_actResetStatus();

        SetText(m_strText);
        SetLeftBtnText(m_strLeftBtnText);
        SetRightRematchBtnText(m_strRightRematchBtnText);
        SetRightReviveBtnText(m_strRightReviveBtnText);
        SetMiddleBtnText(m_strMiddleBtnText);

        if (m_actShowWaittingAnim != null)
            m_actShowWaittingAnim();

        if (m_actShowLeftBtn != null)
            m_actShowLeftBtn();

        if (m_actShowRightBtn != null)
            m_actShowRightBtn();

        if (m_actShowMiddleBtn != null)
            m_actShowMiddleBtn();

        if (m_actShowCloseBtn != null)
            m_actShowCloseBtn();

        if (m_actLeftBtnUp != null)
            SetLeftBtnAction(m_actLeftBtnUp);

        if (m_actRightRematchBtnUp != null)
            SetRightRematchBtnAction(m_actRightRematchBtnUp);

        if (m_actRightReviveBtnUp != null)
            SetRightReviveBtnAction(m_actRightReviveBtnUp);

        if (m_actMiddleBtnUp != null)
            SetMiddleBtnAction(m_actMiddleBtnUp);

        if (m_actDiamondBtnUp != null)
            SetDiamondBtnAction(m_actDiamondBtnUp);

        if (m_actLeftBtnEnable != null)
        {
            m_actLeftBtnEnable();
            m_actLeftBtnEnable = null;
        }
    }

    private string m_strText;
    private string m_strLeftBtnText;
    private string m_strRightRematchBtnText;
    private string m_strRightReviveBtnText;
    private string m_strMiddleBtnText;
    private string m_strDiamondBtnText;

    System.Action m_actShowWaittingAnim;
    System.Action m_actShowLeftBtn;
    System.Action m_actShowRightBtn;
    System.Action m_actShowMiddleBtn;
    System.Action m_actShowDiamondBtn;
    System.Action m_actShowCloseBtn;
    System.Action m_actResetStatus;
    System.Action m_actLeftBtnEnable;

    System.Action<int> m_actLeftBtnUp;
    System.Action<int> m_actRightRematchBtnUp;
    System.Action<int> m_actRightReviveBtnUp;
    System.Action<int> m_actMiddleBtnUp;
    System.Action<int> m_actDiamondBtnUp;

    private bool m_isLeftBtnEnable;

    public void SetText(string text)
    {
        m_strText = (string)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.SetText(text); }, text);
    }

    public void SetLeftBtnText(string text)
    {
        m_strLeftBtnText = (string)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.SetLeftBtnText(text); }, text);
    }

    /// <summary>
    /// 重新匹配
    /// </summary>
    /// <param name="text"></param>
    public void SetRightRematchBtnText(string text)
    {
        m_strRightRematchBtnText = (string)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => 
            { 
                EnterWaittingMessageBoxViewManager.Instance.SetRightRematchBtnText(text);
            } , text);
    }

    /// <summary>
    /// 复活
    /// </summary>
    /// <param name="text"></param>
    /// <param name="isRevive"></param>
    public void SetRightReviveBtnText(string text)
    {
        m_strRightReviveBtnText = (string)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () =>
            {
                EnterWaittingMessageBoxViewManager.Instance.SetRightReviveBtnText(text);
            }, text);
    }

    public void SetMiddleBtnText(string text)
    {
        m_strMiddleBtnText = (string)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.SetMiddleBtnText(text); }, text);
    }

    public void SetDiamondBtnText(string text)
    {
        m_strDiamondBtnText = (string)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.SetDiamondBtnText(text); }, text);
    }

    public void ShowWaittingAnim(bool isShow)
    {
        m_actShowWaittingAnim = MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.ShowWaittingAnim(isShow); });
    }

    public void ShowLeftBtn(bool isShow)
    {
        m_actShowLeftBtn = MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.ShowLeftBtn(isShow); });
    }

    public void ShowRightRematchBtn(bool isShow)
    {
        m_actShowRightBtn = MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.ShowRightRematchBtn(isShow); });
    }

    public void ShowRightReviveBtn(bool isShow)
    {
        m_actShowRightBtn = MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.ShowRightReviveBtn(isShow); });
    }

    public void ShowMiddleBtn(bool isShow)
    {
        m_actShowMiddleBtn = MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.ShowMiddleBtn(isShow); });
    }

    public void ShowCloseBtn(bool isShow)
    {
        m_actShowCloseBtn = MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.ShowCloseBtn(isShow); });
    }

    public void ShowDiamondBtn(bool isShow)
    {
        m_actShowDiamondBtn = MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.ShowDiamondBtn(isShow); });
    }

    public void SetLeftBtnEnable(bool isEnable)
    {
        m_actLeftBtnEnable = MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance,
            () =>
            {
                EnterWaittingMessageBoxViewManager.Instance.SetLeftBtnEnable(isEnable);
            });
    }

    public void SetLeftBtnAction(System.Action<int> act)
    {
        m_actLeftBtnUp = (System.Action<int>)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.SetLeftBtnUpAction(act); }, act);
    }

    public void SetRightRematchBtnAction(System.Action<int> act)
    {
        m_actRightRematchBtnUp = (System.Action<int>)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.SetRightRematchBtnUpAction(act); }, act);
    }

    public void SetRightReviveBtnAction(System.Action<int> act)
    {
        m_actRightReviveBtnUp = (System.Action<int>)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.SetRightReviveBtnUpAction(act); }, act);
    }

    public void SetMiddleBtnAction(System.Action<int> act)
    {
        m_actMiddleBtnUp = (System.Action<int>)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.SetMiddleBtnUpAction(act); }, act);
    }

    public void SetDiamondBtnAction(System.Action<int> act)
    {
        m_actDiamondBtnUp = (System.Action<int>)MFUIUtils.SafeSetValue(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.SetDiamondBtnUpAction(act); }, act);
    }

    public void ResetStatus()
    {
        m_actResetStatus = (System.Action)MFUIUtils.SafeDoAction(EnterWaittingMessageBoxViewManager.Instance,
            () => { EnterWaittingMessageBoxViewManager.Instance.ResetAllStatus(); });
    }
}
