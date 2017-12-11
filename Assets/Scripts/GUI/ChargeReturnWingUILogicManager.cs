using UnityEngine;
using System.Collections;

public class ChargeReturnWingUILogicManager : MFUILogicUnit
{

    static ChargeReturnWingUILogicManager m_instance;

    public static ChargeReturnWingUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ChargeReturnWingUILogicManager();
            }

            return ChargeReturnWingUILogicManager.m_instance;
        }
    }

    System.Action dirtyAct;
    System.Action showItem0GetBtnAct;
    System.Action showItem1GetBtnAct;
    string topIconName;
    string bottomIconName;

    public override void FillBufferedData()
    {
        if (dirtyAct != null)
            dirtyAct();

        SetTopWingIcon(topIconName);
        SetBottomWingIcon(bottomIconName);

        if (showItem0GetBtnAct != null)
        {
            showItem0GetBtnAct();
        }

        if (showItem1GetBtnAct != null)
        {
            showItem1GetBtnAct();
        }
    }

    public void ClearLogicStatus()
    {
        showItem0GetBtnAct = null;
        showItem1GetBtnAct = null;
    }

    public void SetUIDirty()
    {
        dirtyAct = MFUIUtils.SafeDoAction(ChargeReturnWingUIViewManager.Instance,
            () => { ChargeReturnWingUIViewManager.Instance.SetUIDirty(); });
    }

    public void SetTopWingIcon(string iconName)
    {
        topIconName = (string)MFUIUtils.SafeSetValue(ChargeReturnWingUIViewManager.Instance,
            () => { ChargeReturnWingUIViewManager.Instance.SetTopWingIcon(iconName); },iconName);
    }

    public void SetBottomWingIcon(string iconName)
    {
        bottomIconName = (string)MFUIUtils.SafeSetValue(ChargeReturnWingUIViewManager.Instance,
            () => { ChargeReturnWingUIViewManager.Instance.SetBottomWingIcon(iconName); }, iconName);
    }

    public void ShowItem0GetBtn(bool isShow)
    {
        showItem0GetBtnAct = MFUIUtils.SafeDoAction(ChargeReturnWingUIViewManager.Instance,
            () => { ChargeReturnWingUIViewManager.Instance.ShowItem0GetBtn(true); });
    }

    public void ShowItem1GetBtn(bool isShow)
    {
        showItem1GetBtnAct = MFUIUtils.SafeDoAction(ChargeReturnWingUIViewManager.Instance,
            () => { ChargeReturnWingUIViewManager.Instance.ShowItem1GetBtn(true); });
    }

}
