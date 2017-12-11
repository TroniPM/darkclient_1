using System;
using System.Collections;

public class MogoOKCancelBoxQueueUnit
{
    public string BoxText;
    public string OKText;
    public string CancelText;
    public ButtonBgType OKBgType;
    public ButtonBgType CancelBgType;
    public Action<bool> CBAction;
    public Action CBActionOneBtn;

    public void JustDoIt()
    {
        if (CBAction == null)
        {
            MogoGlobleUIManager.Instance.TruelyInfo(BoxText, OKText, CancelText, -1, OKBgType, CancelBgType,CBActionOneBtn);
        }
        else
        {
            MogoGlobleUIManager.Instance.TruelyConfirm(BoxText, CBAction, OKText, CancelText, -1, OKBgType, CancelBgType);
        }
    }
}
