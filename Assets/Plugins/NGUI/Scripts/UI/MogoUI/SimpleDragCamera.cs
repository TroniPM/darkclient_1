/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SimpleDragCamera
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class SimpleDragCamera : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public Camera RelatedCamera;
    //bool m_bDraging = false;
    //Vector2 m_vec2Drag;
    //Vector3 m_vec3DragBegin;

    //public GameObject m_goDraggableArea;
    public Transform beginTransform;
    public Transform endTransform;

    public float height = 0;

    // 初始化
    void Start()
    {
    }

    public void Reset()
    {
        RelatedCamera.transform.localPosition = new Vector3(RelatedCamera.transform.localPosition.x, beginTransform.localPosition.y, RelatedCamera.transform.localPosition.z);
    }

    void OnDrag(Vector2 v)
    {
        if (height < 0) return;
        float y = RelatedCamera.transform.localPosition.y - v.y;

        if (y > beginTransform.localPosition.y)
            y = beginTransform.localPosition.y;

        if (y < beginTransform.localPosition.y - height)
            y = beginTransform.localPosition.y - height;

        RelatedCamera.transform.localPosition = new Vector3(RelatedCamera.transform.localPosition.x, y, RelatedCamera.transform.localPosition.z);



    }

    public void MoveFromBeginByHeight(int height)
    {
        RelatedCamera.transform.localPosition = new Vector3(RelatedCamera.transform.localPosition.x, beginTransform.localPosition.y + height, RelatedCamera.transform.localPosition.z);
    }
}
