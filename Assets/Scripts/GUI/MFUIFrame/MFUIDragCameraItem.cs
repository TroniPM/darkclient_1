using UnityEngine;
using System.Collections;

public class MFUIDragCameraItem : MFUIBehavior
{
    public MFUIDragCamera DragCamera;

    bool m_bIsDragging = false;
    Vector2 m_vec2Dir;

    public override void MFUIAwake()
    {
        //MFUIUtils.SafeDoAction(DragCamera, () => { DragCamera.RegisterDragItem(m_myGameObject); });
    }

    public override void MFUIDrag(Vector2 dir)
    {
        if (!m_bIsDragging)
        {
            m_bIsDragging = true;
            m_vec2Dir = dir;
        }
    }

    public override void MFUIPress(bool isPressed)
    {
        if (!isPressed)
        {
            m_bIsDragging = false;
            MFUIUtils.SafeDoAction(DragCamera, () => { DragCamera.CameraMove(m_vec2Dir); });
        }
    }
}
