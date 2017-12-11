using UnityEngine;
using System.Collections;

public class InvitFriendMessageBoxLogicManager : MFUILogicUnit
{

    static InvitFriendMessageBoxLogicManager m_instance;

    public static InvitFriendMessageBoxLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new InvitFriendMessageBoxLogicManager();
            }

            return InvitFriendMessageBoxLogicManager.m_instance;
        }
    }

    System.Action act;

    public void SetUIDirty()
    {        
        act = MFUIUtils.SafeDoAction(InvitFriendMessageBoxViewManager.Instance, () =>
        {
            InvitFriendMessageBoxViewManager.Instance.SetUIDirty();
        });
    }

    public override void FillBufferedData()
    {
        if (act!=null)
        {
            act();
        }
    }
	
}
