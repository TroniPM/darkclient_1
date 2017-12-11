using UnityEngine;
using System.Collections;

public class WingUIUpgradeDialogLogicManager : MFUILogicUnit
{

    static WingUIUpgradeDialogLogicManager m_instance;

    public static WingUIUpgradeDialogLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new WingUIUpgradeDialogLogicManager();
            }

            return WingUIUpgradeDialogLogicManager.m_instance;
        }
    }

    System.Action actDirty;
    string m_strTitle;
    string m_strLevel;
    string m_strNextLevel;
    string m_strCost;
    string m_strImgName;
    string m_strProgressText;
    float m_fProgressSize;
    int m_iStarNum;

    System.Collections.Generic.List<System.Action> m_listActSetCurrentAttr = new System.Collections.Generic.List<System.Action>();
    System.Collections.Generic.List<System.Action> m_listActSetNextAttr = new System.Collections.Generic.List<System.Action>();

    public void SetUIDirty()
    {
        actDirty = MFUIUtils.SafeDoAction(WingUIUpgradeDialogViewManager.Instance, () =>
        {
            WingUIUpgradeDialogViewManager.Instance.SetUIDirty();
        });
    }

    public override void FillBufferedData()
    {
        if (actDirty != null)
            actDirty();

        SetUpgradeDialogTitle(m_strTitle);
        SetUpgradeDialogCurrentLevel(m_strLevel);
        SetUpgradeDialogNextLevel(m_strNextLevel);
        SetUpgradeDialogCost(m_strCost);
        SetUpgradeDialogIcon(m_strImgName);

        if (m_listActSetCurrentAttr.Count > 0)
        {
            for (int i = 0; i < m_listActSetCurrentAttr.Count; ++i)
            {
                m_listActSetCurrentAttr[i]();
            }

            m_listActSetCurrentAttr.Clear();
        }

        if (m_listActSetNextAttr.Count > 0)
        {
            for (int i = 0; i < m_listActSetNextAttr.Count; ++i)
            {
                m_listActSetNextAttr[i]();
            }

            m_listActSetNextAttr.Clear();
        }

        SetUpgradeDialogProgressText(m_strProgressText);
        SetUpgradeDialogProgressSize(m_fProgressSize);
        SetUpgradeDialogStarNum(m_iStarNum);
    }

    public void SetUpgradeDialogTitle(string title)
    {
        m_strTitle = (string)MFUIUtils.SafeSetValue(WingUIUpgradeDialogViewManager.Instance, () => 
        {
            WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogTitle(title);
        },title);
    }

    public void SetUpgradeDialogCurrentLevel(string level)
    {
        m_strLevel = (string)MFUIUtils.SafeSetValue(WingUIUpgradeDialogViewManager.Instance, () =>
            {
                WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogCurrentLevel(level);
            }, level);
    }

    public void SetUpgradeDialogNextLevel(string level)
    {
        m_strNextLevel = (string)MFUIUtils.SafeSetValue(WingUIUpgradeDialogViewManager.Instance, () =>
        {
            WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogNextLevel(level);
        }, level);
    }

    public void SetUpgradeDialogCost(string cost)
    {
        m_strCost = (string)MFUIUtils.SafeSetValue(WingUIUpgradeDialogViewManager.Instance, () =>
        {
            WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogCost(cost);
        }, cost);
    }

    public void SetUpgradeDialogIcon(string imgName)
    {
        m_strImgName = (string)MFUIUtils.SafeSetValue(WingUIUpgradeDialogViewManager.Instance, () =>
        {
            WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogIcon(imgName);
        }, imgName);
    }

    public void SetUpgradeDialogCurrentAttr(string attr, int id)
    {
        m_listActSetCurrentAttr.Add(MFUIUtils.SafeDoAction(WingUIUpgradeDialogViewManager.Instance, () =>
            { WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogCurrentAttr(attr, id); }));
    }

    public void SetUpgradeDialogNextAttr(string attr, int id)
    {
        m_listActSetNextAttr.Add(MFUIUtils.SafeDoAction(WingUIUpgradeDialogViewManager.Instance, () =>
        { WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogNextAttr(attr, id); }));
    }

    public void SetUpgradeDialogProgressText(string text)
    {
        m_strProgressText = (string)MFUIUtils.SafeSetValue(WingUIUpgradeDialogViewManager.Instance, () =>
        {
            WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogProgressText(text);
        }, text);
    }

    public void SetUpgradeDialogProgressSize(float size)
    {
        m_fProgressSize = (float)MFUIUtils.SafeSetValue(WingUIUpgradeDialogViewManager.Instance, () =>
        {
            WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogProgressSize(size);
        }, size);
    }

    public void SetUpgradeDialogStarNum(int num)
    {
        m_iStarNum = (int)MFUIUtils.SafeSetValue(WingUIUpgradeDialogViewManager.Instance, () =>
        {
            WingUIUpgradeDialogViewManager.Instance.SetUpgradeDialogStarNum(num);
        }, num);
    }

}
