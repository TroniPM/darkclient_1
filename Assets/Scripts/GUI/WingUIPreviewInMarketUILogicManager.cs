using UnityEngine;
using System.Collections;

public class WingUIPreviewInMarketUILogicManager : MFUILogicUnit
{
    static WingUIPreviewInMarketUILogicManager m_instance;

    public static WingUIPreviewInMarketUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new WingUIPreviewInMarketUILogicManager();
            }

            return WingUIPreviewInMarketUILogicManager.m_instance;
        }
    }

    System.Action dirtyAct;
    string title;
    System.Collections.Generic.List<string> attrList = new System.Collections.Generic.List<string>();
    System.Collections.Generic.List<string> attrTotalList = new System.Collections.Generic.List<string>();

    System.Action ShowBuyBtnAct;
    System.Action ShowResetBtnAct;

    public void SetUIDirty()
    {
        dirtyAct = MFUIUtils.SafeDoAction(WingUIPreviewInMarketUIViewManager.Instance,
            () => { WingUIPreviewInMarketUIViewManager.Instance.SetUIDirty(); });
    }

    public override void FillBufferedData()
    {
        if (dirtyAct != null)
            dirtyAct();

        SetTitle(title);
        SetTipAttr(attrList);
        SetWingAttr(attrTotalList);

        if (ShowBuyBtnAct != null)
        {
            ShowBuyBtnAct();
        }

        if (ShowResetBtnAct != null)
        {
            ShowResetBtnAct();
        }
    }

    public void SetTitle(string name)
    {
        title = (string)MFUIUtils.SafeSetValue(WingUIPreviewInMarketUIViewManager.Instance,
            () =>
            {
                WingUIPreviewInMarketUIViewManager.Instance.SetTitle(name);
                WingUIPreviewInMarketUIViewManager.Instance.SetTipTitle(name);
            },name);
    }

    public void SetTipAttr(System.Collections.Generic.List<string> listAttr)
    {
        attrList = (System.Collections.Generic.List<string>)MFUIUtils.SafeSetValue(WingUIPreviewInMarketUIViewManager.Instance,
            () =>
            {
                WingUIPreviewInMarketUIViewManager.Instance.SetTipAttr(listAttr);
            }, listAttr);
    }

    public void SetWingAttr(System.Collections.Generic.List<string> listAttr)
    {
        attrTotalList = (System.Collections.Generic.List<string>)MFUIUtils.SafeSetValue(WingUIPreviewInMarketUIViewManager.Instance,
            () =>
            {
                WingUIPreviewInMarketUIViewManager.Instance.SetWingAttr(listAttr);
            }, listAttr);
    }

    public void ShowBuyBtn(bool isShow)
    {
        ShowBuyBtnAct = MFUIUtils.SafeDoAction(WingUIPreviewInMarketUIViewManager.Instance,
            () =>
            {
                WingUIPreviewInMarketUIViewManager.Instance.ShowBuyBtn(isShow);
            });
    }

    public void ShowResetBtn(bool isShow)
    {
        ShowResetBtnAct = MFUIUtils.SafeDoAction(WingUIPreviewInMarketUIViewManager.Instance,
            () =>
            {
                WingUIPreviewInMarketUIViewManager.Instance.ShowResetBtn(isShow);
            });
    }
}
