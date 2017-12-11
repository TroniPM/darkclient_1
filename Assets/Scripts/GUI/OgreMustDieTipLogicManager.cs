using UnityEngine;
using System.Collections;

public class OgreMustDieTipLogicManager : MFUILogicUnit
{
    static OgreMustDieTipLogicManager m_instance;

    public static OgreMustDieTipLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new OgreMustDieTipLogicManager();
            }

            return OgreMustDieTipLogicManager.m_instance;
        }
    }

    System.Action act;
    System.Action actHideSelf;

    public void SetUIDirty()
    {
        act = MFUIUtils.SafeDoAction(OgreMustDieTipViewManager.Instance, () =>
        {
            OgreMustDieTipViewManager.Instance.SetUIDirty();
        });
    }

    public override void FillBufferedData()
    {
        if (act != null)
            act();

        if (actHideSelf != null)
            actHideSelf();
    }

    public void HideSelf()
    {
        actHideSelf = MFUIUtils.SafeDoAction(OgreMustDieTipViewManager.Instance, () =>
        {
            OgreMustDieTipViewManager.Instance.HideSelf();
        });
    }
}
