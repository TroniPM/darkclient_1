using UnityEngine;
using System.Collections;

public class InvitFriendListUILogicManager : MFUILogicUnit
{

    static InvitFriendListUILogicManager m_instance;

    public static InvitFriendListUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new InvitFriendListUILogicManager();
            }

            return InvitFriendListUILogicManager.m_instance;
        }
    }
}
