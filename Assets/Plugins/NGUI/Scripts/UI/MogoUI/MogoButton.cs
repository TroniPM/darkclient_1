using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;

public class MogoButton : MonoBehaviour
{
    public Action clickHandler;
    public Action<bool> selectHandler = null;
    public Action<bool> pressHandler;
    public Action doubleClickHandler = null;
    public Action<Vector2> dragHandler;


    public Action<Vector2> dragOverHandler;
    public Action<GameObject> dropHandler = null;
    private bool isDragging = false;
    private Vector2 dragDelta;
    public bool IsDragCancel = true;

    void OnSelect(bool selected)
    {
        if (selectHandler != null)
        {
            selectHandler(selected);
        }
    }
    void OnClickNew()
    {
        if (transform.GetComponentsInChildren<MogoTwoStatusButton>(true).Length > 0)
        {
            if (transform.GetComponentsInChildren<MogoTwoStatusButton>(true)[0].Clickable)
            {
                if (clickHandler != null)
                {
                    clickHandler();
                }
            }
        }
        else
        {
            if (clickHandler != null)
            {
                clickHandler();
            }
        }
    }

    void OnClick()
    {
        if (!IsDragCancel)
        {
            if (clickHandler != null)
            {
                clickHandler();
            }
        }
    }

    void OnPress(bool press)
    {
        if (!IsDragCancel)
            return;

        if (press)
        {
            if (pressHandler != null)
            {
                pressHandler(press);
            }
        }
        else
        {
            if (isDragging)
            {
                if (pressHandler != null)
                {
                    pressHandler(press);
                }
                if (dragOverHandler != null)
                {
                    dragOverHandler(dragDelta);
                }
            }
            else
            {
                if (pressHandler != null)
                {
                    pressHandler(press);
                }
                OnClickNew();
            }
            isDragging = false;
        }
    }

    void OnDrag(Vector2 delta)
    {
        isDragging = true;
        dragDelta = delta;
        if (dragHandler != null)
        {
            dragHandler(delta);
        }
    }


    void OnDrop(GameObject drop)
    {
        if (dropHandler != null)
        {
            dropHandler(drop);
        }
    }

    public void FakeClick()
    {
        if (clickHandler != null)
        {
            clickHandler();
        }
    }
}
