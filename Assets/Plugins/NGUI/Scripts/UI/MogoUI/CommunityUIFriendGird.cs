using UnityEngine;
using System.Collections;
using Mogo.Util;

public class CommunityUIFriendGird : MonoBehaviour 
{
    public ulong ID;
    bool m_bIsDragging = false;

    void OnDrag(Vector2 v)
    {
        m_bIsDragging = true;
        LoggerHelper.Debug("Drag");
    }

    void OnPress(bool isOver)
    {
        if (!isOver)
        {
            if (!m_bIsDragging)
            {
                EventDispatcher.TriggerEvent<ulong>("CommunityUIFriendGridUp", ID);
            }

            m_bIsDragging = false;
        }
    }
}
