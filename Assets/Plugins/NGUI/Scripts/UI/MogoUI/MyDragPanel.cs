using UnityEngine;
using System.Collections;

public class MyDragPanel : MonoBehaviour
{

    public  MyDragablePanel m_myDPanel;
    bool m_bDraging = false;
    Vector2 m_vec2Drag;
    Vector3 m_vec3DragBegin;

    

    void OnDrag(Vector2 v)
    {
        if (m_bDraging == false)
        {
             m_vec2Drag = v;
             m_vec3DragBegin = Input.mousePosition;
             m_bDraging = true;

             UIButtonTween[] btList = transform.GetComponentsInChildren<UIButtonTween>(true);

             if (btList.Length > 0)
             {
                 btList[0].enabled = false;

             }
        }
    }

    void OnPress(bool isPressed)
    {
        //if (isPressed == false && m_bDraging)
        //{
        //    m_bDraging = false;
        //    m_myDCamera.MovePage(m_vec2Drag);
        //}
        if (!isPressed)
        {
            if (m_bDraging)
            {
                m_bDraging = false;
                m_myDPanel.MovePage(m_vec2Drag);
            }
            else
            {
                UIButtonTween[] btList = transform.GetComponentsInChildren<UIButtonTween>(true);

                if (btList.Length > 0)
                {
                    btList[0].enabled = true;

                }
            }
        }
    }

    void Update()
    {
        if (m_bDraging)
        {
			float length ;
			
			if(m_myDPanel.IsHorizontal)
			{
            	length = Input.mousePosition.x - m_vec3DragBegin.x;
			}
			else
			{
				length = Input.mousePosition.y - m_vec3DragBegin.y;				
			}
			
            m_myDPanel.DraggingMove(-length);
        }
    }
}
