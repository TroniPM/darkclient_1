using Mogo.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    public int iconID = 0;

    bool m_bIsDrag = false;

    void Awake()
    {
    }

    void OnPress(bool isOver)
    {
        if (!isOver)
        {
            Mogo.Util.LoggerHelper.Debug("Icon ID: " + iconID);

            if (!m_bIsDrag)
            {
                EventDispatcher.TriggerEvent(InventoryEvent.ShowTip, iconID);
            }

            m_bIsDrag = false;
        }
    }

    void OnDrag(Vector2 vec)
    {
        m_bIsDrag = true;
    }
}
