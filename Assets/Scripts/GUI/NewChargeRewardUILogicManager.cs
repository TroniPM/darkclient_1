using UnityEngine;
using System.Collections;

public class NewChargeRewardUILogicManager : MFUILogicUnit
{

    static NewChargeRewardUILogicManager m_instance;

    public static NewChargeRewardUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new NewChargeRewardUILogicManager();
            }

            return NewChargeRewardUILogicManager.m_instance;
        }
    }

    System.Action dirtyAct;
    System.Action chargedAct;
    System.Action prgressSizeAct;
    System.Action showGetBtnAct;
    System.Action setGridPriceAct;
    System.Action setGridHighLightAct;
    System.Collections.Generic.List<int> m_itemIDList = new System.Collections.Generic.List<int>();
    MFUIResult result = new MFUIResult();

    public override void FillBufferedData()
    {
        if (dirtyAct != null)
            dirtyAct();

        if (chargedAct != null)
            chargedAct();

        if (prgressSizeAct != null)
            prgressSizeAct();

        if (setGridPriceAct != null)
            setGridPriceAct();

        if (setGridHighLightAct != null)
            setGridHighLightAct();

        SetItemList(m_itemIDList);

        if (result.isFailed)
        {
            AddChargePriceItem((System.Collections.Generic.List<string>)result.buffer);
        }
    }

    public void SetUIDirty()
    {
        dirtyAct = MFUIUtils.SafeDoAction(NewChargeRewardUIViewManager.Instance,
            () => { NewChargeRewardUIViewManager.Instance.SetUIDirty(); });

    }

    public void SetItemList(System.Collections.Generic.List<int> itemIDList)
    {
        m_itemIDList = (System.Collections.Generic.List<int>)MFUIUtils.SafeSetValue(NewChargeRewardUIViewManager.Instance,
            () => { NewChargeRewardUIViewManager.Instance.SetItemList(itemIDList); }, itemIDList);
    }

    public void ShowAsCharged(bool isCharged)
    {
        chargedAct = MFUIUtils.SafeDoAction(NewChargeRewardUIViewManager.Instance,
            () => { NewChargeRewardUIViewManager.Instance.ShowAsCharged(isCharged); });
    }

    public void SetProgressSize(float size)
    {
        prgressSizeAct = MFUIUtils.SafeDoAction(NewChargeRewardUIViewManager.Instance,
            () => { NewChargeRewardUIViewManager.Instance.SetProgressSize(size); });
    }

    public void ShowGetBtn(bool isShow)
    {
        showGetBtnAct = MFUIUtils.SafeDoAction(NewChargeRewardUIViewManager.Instance,
            () => { NewChargeRewardUIViewManager.Instance.ShowGetBtn(isShow); });
    }

    public void AddChargePriceItem(System.Collections.Generic.List<string> itemText)
    {
        result = MFUIUtils.SafeSetValue_W(NewChargeRewardUIViewManager.Instance,
            () => { NewChargeRewardUIViewManager.Instance.AddChargePriceItem(itemText); }, itemText);
    }

    public void SetProgressGridText(string text, int id)
    {
        setGridPriceAct = MFUIUtils.SafeDoAction(NewChargeRewardUIViewManager.Instance,
            () => { NewChargeRewardUIViewManager.Instance.SetProgressGridText(text, id); });
    }

    public void ShowProgressGridIconHighLight(bool isHighLight, int id)
    {
        setGridHighLightAct = MFUIUtils.SafeDoAction(NewChargeRewardUIViewManager.Instance,
            () => { NewChargeRewardUIViewManager.Instance.ShowProgressGridIconHighLight(isHighLight, id); });
    }
}
