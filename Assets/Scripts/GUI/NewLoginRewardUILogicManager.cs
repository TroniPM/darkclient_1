using UnityEngine;
using System.Collections;

public class NewLoginRewardUILogicManager : MFUILogicUnit
{

    static NewLoginRewardUILogicManager m_instance;

    public static NewLoginRewardUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new NewLoginRewardUILogicManager();
            }

            return NewLoginRewardUILogicManager.m_instance;
        }
    }

    System.Action dirtyAct;
    MFUIResult m_refreshGridResult=new MFUIResult();

    public override void FillBufferedData()
    {
        if (dirtyAct != null)
            dirtyAct();

        if (m_refreshGridResult.isFailed)
        {
            RefreshGridList((System.Collections.Generic.List<NewLoginRewardGridData>)m_refreshGridResult.buffer);
        }
    }

    public void SetUIDirty()
    {
        dirtyAct = MFUIUtils.SafeDoAction(NewLoginRewardUIViewManager.Instance,
            () => { NewLoginRewardUIViewManager.Instance.SetUIDirty(); });
    }

    public void RefreshGridList(System.Collections.Generic.List<NewLoginRewardGridData> list)
    {
        m_refreshGridResult = MFUIUtils.SafeSetValue_W(NewLoginRewardUIViewManager.Instance,
            () => { NewLoginRewardUIViewManager.Instance.RefreshGridList(list); }, list);
    }
}
