using UnityEngine;
using System.Collections;

public class RewardUILogicManager : MFUILogicUnit
{

    static RewardUILogicManager m_instance;

    public static RewardUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new RewardUILogicManager();
            }

            return RewardUILogicManager.m_instance;
        }
    }

    System.Action dirtyAct;

    public override void FillBufferedData()
    {
        if (dirtyAct != null)
            dirtyAct();

    }

    public void SetUIDirty()
    {
        dirtyAct = MFUIUtils.SafeDoAction(RewardUIViewManager.Instance,
            () => { RewardUIViewManager.Instance.SetUIDirty(); });
    }

}
