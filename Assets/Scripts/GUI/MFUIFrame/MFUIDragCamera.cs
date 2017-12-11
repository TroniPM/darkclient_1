using UnityEngine;
using System.Collections;

public class MFUIDragCamera : MFUIBehavior 
{
    public System.Collections.Generic.List<GameObject> m_listDragItem = new System.Collections.Generic.List<GameObject>();

    public bool IsHorizontal = true;
    protected TweenPosition m_tp;

    public override void MFUIAwake()
    {
        if (m_myGameObject.GetComponentsInChildren<TweenPosition>(true).Length == 0)
        {
            Vector3 tmpPos = m_myTransform.localPosition;

            m_tp = m_myGameObject.AddComponent<TweenPosition>();
            m_tp.enabled = false;
            m_tp.Reset();
            m_tp.duration = 0.2f;

            m_myTransform.localPosition = tmpPos;
        }
        else
        {
            m_tp = m_myGameObject.GetComponentsInChildren<TweenPosition>(true)[0];
        }
    }

    public override void MFUIStart()
    {
        CallWhenRegisterDragItem();
    }

    public System.Collections.Generic.List<GameObject> GetDragItemList()
    {
        return m_listDragItem;
    }
    public void ClearDragItem()
    {
        m_listDragItem.Clear();
    }

    public void RegisterDragItem(GameObject item)
    {
        m_listDragItem.Add(item);
        CallWhenRegisterDragItem();
    }

    public virtual void MoveHorizontal(bool isForward,float length)
    {
 
    }

    public virtual void MovePerpendicular(bool isForward, float length)
    { }

    public virtual void CallWhenRegisterDragItem()
    {
 
    }

    public void CameraMove(Vector2 dir)
    {
        if (IsHorizontal)
        {
            if (dir.x < 0)
            {
                MoveHorizontal(true, dir.x);
            }
            else if (dir.x > 0)
            {
                MoveHorizontal(false, dir.x);
            }
        }
        else
        {
            if (dir.y < 0)
            {
                MovePerpendicular(false, dir.y);
            }
            else if (dir.y > 0)
            {
                MovePerpendicular(true, dir.y);
            }
        }
    }

    protected void MoveToTarget(Vector3 targetPos)
    {
        m_tp.from = m_myTransform.transform.localPosition;
        m_tp.to = targetPos;
        m_tp.Reset();
        m_tp.enabled = true;
        m_tp.Play(true);
    }
	
}
