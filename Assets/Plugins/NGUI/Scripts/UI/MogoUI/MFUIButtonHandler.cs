using UnityEngine;
using System.Collections;
using System;

public class MFUIButtonHandler : MonoBehaviour {

    public int ID = -1;

    public Action<int> ClickHandler;
    public Action<bool, int> PressHandler;
    public Action<Vector2, int> DragHandler;

    public bool DragCancel = false;

    bool m_bIsEnable = true;
    bool m_bIsDraging = false;

    void OnClick()
    {
        if(DragCancel)
            return;

        if (ClickHandler != null && m_bIsEnable)
        {
            ClickHandler(ID);
        }
    }

    void OnPress(bool isPressed)
    {
        if (PressHandler != null && m_bIsEnable)
        {
            PressHandler(isPressed, ID);
        }

        if (!isPressed)
        {
            if (DragCancel)
            {
                if (!m_bIsDraging)
                {
                    if (ClickHandler != null && m_bIsEnable)
                    {
                        ClickHandler(ID);
                    }
                }
            }

            m_bIsDraging = false;
        }
    }

    void OnDrag(Vector2 dragDir)
    {
        m_bIsDraging = true;
        if (DragHandler != null && m_bIsEnable)
        {
            DragHandler(dragDir, ID);
        }
    }

    public void FakeClick()
    {
        if (ClickHandler != null && m_bIsEnable)
        {
            ClickHandler(ID);
        }
    }

    public void SetEnable(bool isEnable)
    {
        m_bIsEnable = isEnable;
    }

}
