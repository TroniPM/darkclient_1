using UnityEngine;
using System.Collections;

public class WingUIBuyDialogLogicManager : MFUILogicUnit
{

    static WingUIBuyDialogLogicManager m_instance;

    public static WingUIBuyDialogLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new WingUIBuyDialogLogicManager();
            }

            return WingUIBuyDialogLogicManager.m_instance;
        }
    }

    System.Action actDirty;
    string m_strTitle;
    string m_strCost;
System.Collections.Generic.List<System.Action> m_listActAttr = new System.Collections.Generic.List<System.Action>();
    string m_strImgName;

    public void SetUIDirty()
    {
        actDirty = MFUIUtils.SafeDoAction(WingUIBuyDialogViewManager.Instance, () =>
        {
            WingUIBuyDialogViewManager.Instance.SetUIDirty();
        });
    }

    public override void FillBufferedData()
    {
        if (actDirty != null)
            actDirty();

        SetBuyDialodTitle(m_strTitle);
        SetBuyDialogCost(m_strCost);

        if (m_listActAttr.Count > 0)
        {
            for (int i = 0; i < m_listActAttr.Count; ++i)
            {
                m_listActAttr[i]();
            }

            m_listActAttr.Clear();
        }

        SetBuyDialogIcon(m_strImgName);
    }

    public void SetBuyDialodTitle(string title)
    {
        m_strTitle = (string)MFUIUtils.SafeSetValue(WingUIBuyDialogViewManager.Instance,
            () => { WingUIBuyDialogViewManager.Instance.SetBuyDialodTitle(title); }, title);
    }

    public void SetBuyDialogCost(string cost)
    {
        m_strCost = (string)MFUIUtils.SafeSetValue(WingUIBuyDialogViewManager.Instance,
            () => { WingUIBuyDialogViewManager.Instance.SetBuyDialogCost(cost); }, cost);
    }

    public void SetBuyDialogAttr(string attr, int id)
    {
        m_listActAttr.Add(MFUIUtils.SafeDoAction(WingUIBuyDialogViewManager.Instance,
            () => { WingUIBuyDialogViewManager.Instance.SetBuyDialogAttr(attr, id); }));
    }

    public void SetBuyDialogIcon(string imgName)
    {
        m_strImgName = (string)MFUIUtils.SafeSetValue(WingUIBuyDialogViewManager.Instance,
            () => { WingUIBuyDialogViewManager.Instance.SetBuyDialogIcon(imgName); }, imgName);
    }
}
