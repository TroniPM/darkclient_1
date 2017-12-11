using UnityEngine;
using System.Collections;
using Mogo.Util;

public class CommunityUIDialogLabel : MonoBehaviour 
{
    public ulong ID;
    bool m_isDragging = false;

    void OnDrag(Vector2 v)
    {
        m_isDragging = true;
    }

    void OnPress(bool isOver)
    {
        if (!isOver)
        {
            if(!m_isDragging)
                EventDispatcher.TriggerEvent<ulong>("CommunityUIDialogLabelUp", ID);

            m_isDragging = false;
        }
    }
}
