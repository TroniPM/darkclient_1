using UnityEngine;
using System.Collections;
using System;
using Mogo.Util;

public class TeachUIFocusWidget : MonoBehaviour
{
  
    //void OnClick()
    //{
    //    Mogo.Util.LoggerHelper.Debug("FocusWidget Down!!!!!!!");
    //    EventDispatcher.TriggerEvent("TeachUIFocusDown");
    //    LoggerHelper.Debug("TeachUIFocusWidget");
    //    EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.ButtonClick);
    //}

    bool m_isDragging = false;
    public bool PressHappen = false;

    public GameObject OriWidget = null;

    void OnDrag(Vector2 v)
    {
        m_isDragging = true;
    }

    void OnPress(bool isPressed)
    {
        if (!isPressed)
        {
            m_isDragging = false;

            if (PressHappen)
            {
                if (OriWidget != null)
                {
                    OriWidget.GetComponent<ControlStick>().Reset();
                    OriWidget.GetComponent<ControlStick>().InitInstance();
                }
                Destroy(gameObject);
            }
        }
        else
        {

            if (PressHappen)
            {
                EventDispatcher.TriggerEvent("TeachUIFocusDown");
                EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.ButtonClick);
            }
        }

    }


    void OnClick()
    {
        if (!m_isDragging)
        {
            if (PressHappen)
                return;

            Mogo.Util.LoggerHelper.Debug("FocusWidget Down!!!!!!!");
            EventDispatcher.TriggerEvent("TeachUIFocusDown");
            LoggerHelper.Debug("TeachUIFocusWidget");
            EventDispatcher.TriggerEvent<SignalEvents>(Events.CommandEvent.CommandEnd, SignalEvents.ButtonClick);
        }
    }
}
