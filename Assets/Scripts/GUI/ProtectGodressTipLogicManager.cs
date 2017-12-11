using UnityEngine;
using System.Collections;

public class ProtectGodressTipLogicManager : MFUILogicUnit
{

    static ProtectGodressTipLogicManager m_instance;

    System.Action dirtyAct;
    System.Action clearAct;
    System.Action setTipTextAct;
    MFUIResult setRewardListResult = new MFUIResult();

    public static ProtectGodressTipLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ProtectGodressTipLogicManager();
            }

            return ProtectGodressTipLogicManager.m_instance;
        }
    }

    public override void FillBufferedData()
    {
        if (dirtyAct != null)
            dirtyAct();

        if (clearAct != null)
            clearAct();

        if (setTipTextAct != null)
            setTipTextAct();

        if (setRewardListResult.isFailed)
        {
            SetTipRewardList((System.Collections.Generic.List<string>)setRewardListResult.buffer);
        }
    }

    public void SetUIDirty()
    {
        dirtyAct = MFUIUtils.SafeDoAction(ProtectGodressTipViewManager.Instance,
            () => { ProtectGodressTipViewManager.Instance.SetUIDirty(); });
    }

    public void SetTipRewardList(System.Collections.Generic.List<string> rewardlist)
    {
        setRewardListResult = (MFUIResult)MFUIUtils.SafeSetValue_W(ProtectGodressTipViewManager.Instance,
            () => { ProtectGodressTipViewManager.Instance.SetTipRewardList(rewardlist); },
            rewardlist);
    }

    public void ClearRewardList()
    {
        clearAct = MFUIUtils.SafeDoAction(ProtectGodressTipViewManager.Instance,
            () => { ProtectGodressTipViewManager.Instance.ClearRewardList(); });
    }

    public void SetTipText(int id,string text)
    {
        setTipTextAct = MFUIUtils.SafeDoAction(ProtectGodressTipViewManager.Instance,
            () => { ProtectGodressTipViewManager.Instance.SetTipText(id, text); });
    }
}
