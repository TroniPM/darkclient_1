using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ComposeIconGrid : MonoBehaviour {

    public int id;
    // 为去除警告暂时屏蔽以下代码
    //bool m_bIsDragging = false;

    //void OnPress(bool isOver)
    //{
    //    if (!m_bIsDragging)
    //    {
    //        if (!isOver)
    //        {
    //            EventDispatcher.TriggerEvent<int>("ComposeIconGridUp", id);
    //        }
    //    }

    //    m_bIsDragging = false;
    //}

    //void OnDrag(Vector2 v)
    //{
    //    m_bIsDragging = true;
    //}

    void OnClick()
    {
        EventDispatcher.TriggerEvent<int>("ComposeIconGridUp", id);
    }
}
