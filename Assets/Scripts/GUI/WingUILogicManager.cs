using UnityEngine;
using System.Collections;

public class WingUILogicManager : MFUILogicUnit
{
    static WingUILogicManager m_instance;

    public static WingUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new WingUILogicManager();
            }

            return WingUILogicManager.m_instance;
        }
    }

    System.Action actDirty;
    System.Action showTipBuyBtn;
    System.Action showTipUpgradeBtn;

    string strGold;
    string strDiamond;
    string buttonName;
    System.Collections.Generic.List<string> listAttr = new System.Collections.Generic.List<string>();
    System.Collections.Generic.List<WingGridData> listData = new System.Collections.Generic.List<WingGridData>();

    public void SetUIDirty()
    {
        actDirty = MFUIUtils.SafeDoAction(WingUIViewManager.Instance, () =>
            {
                WingUIViewManager.Instance.SetUIDirty();
            });
    }

    public void SetBuyText(string name)
    {
        buttonName = (string)MFUIUtils.SafeSetValue(WingUIViewManager.Instance,
            () => { WingUIViewManager.Instance.SetBuyText(name); }, name);
    }

    public void SetGold(string gold)
    {
        strGold = (string)MFUIUtils.SafeSetValue(WingUIViewManager.Instance,
            () => { WingUIViewManager.Instance.SetGold(gold); }, gold);

    }

    public void SetDiamond(string diamond)
    {
        strDiamond = (string)MFUIUtils.SafeSetValue(WingUIViewManager.Instance,
            () => { WingUIViewManager.Instance.SetDiamond(diamond); }, diamond);
    }

    public void SetWingAttribute(System.Collections.Generic.List<string> attrList)
    {
        listAttr = (System.Collections.Generic.List<string>)MFUIUtils.SafeSetValue(
            WingUIViewManager.Instance, () => { WingUIViewManager.Instance.SetWingAttribute(attrList); }, attrList);
    }

    public void RefreshWingGridList(System.Collections.Generic.List<WingGridData> dataList)
    {
        listData = (System.Collections.Generic.List<WingGridData>)MFUIUtils.SafeSetValue(
            WingUIViewManager.Instance, () => { WingUIViewManager.Instance.RefreshWingGridList(dataList); }, dataList);
    }
    public override void FillBufferedData()
    {
        if (actDirty != null)
        {
            actDirty();
        }

        SetGold(strGold);
        SetDiamond(strDiamond);
        SetWingAttribute(listAttr);
        RefreshWingGridList(listData);
    }

    public void ShowTipUpgradeBtn(bool isShow)
    {
        showTipBuyBtn = MFUIUtils.SafeDoAction(WingUIViewManager.Instance,
            () =>
            {
                WingUIViewManager.Instance.ShowTipUpgradeBtn(isShow);
            });
    }

    public void ShowTipBuyBtn(bool isShow)
    {
        showTipUpgradeBtn = MFUIUtils.SafeDoAction(WingUIViewManager.Instance,
            () =>
            {
                WingUIViewManager.Instance.ShowTipBuyBtn(isShow);
            });
    }
}
