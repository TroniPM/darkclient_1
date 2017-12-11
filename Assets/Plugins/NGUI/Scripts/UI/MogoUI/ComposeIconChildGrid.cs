using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ComposeIconChildGrid : MonoBehaviour {

    public int id;
    public int parentId;
    // 为去除警告暂时屏蔽以下代码
    //bool isDragged = false;

    //void OnDrag(Vector2 drag)
    //{
    //    isDragged = true;
    //}

    //void OnPress(bool isOver)
    //{
    //    if (!isOver)
    //    {
    //        if (!isDragged)
    //        {
    //            EventDispatcher.TriggerEvent<int, int>("ComposeIconChildGridUp", parentId, id);
    //        }

    //        isDragged = false;
    //    }
    //}

    void OnClick()
    {
        EventDispatcher.TriggerEvent<int, int>("ComposeIconChildGridUp", parentId, id);
    }
}
