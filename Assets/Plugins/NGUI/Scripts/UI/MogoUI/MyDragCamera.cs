/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MyDragCamera
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MyDragCamera : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public Camera RelatedCamera;
    MyDragableCamera m_myDCamera;
    bool m_bDraging = false;
    Vector2 m_vec2Drag;
    Vector3 m_vec3DragBegin;
    Vector2 m_vec2AloneMoveSpeed;
    bool m_isCountDown = false;
    float m_fCountDown = 0;
    bool m_isTryingDraggingMove = false;

    bool m_hasSendSound = false;

    public GameObject m_goDraggableArea;


    // 初始化
    void Start()
    {
        m_myDCamera = RelatedCamera.GetComponentInChildren<MyDragableCamera>() as MyDragableCamera;
    }

    void OnDrag(Vector2 v)
    {
        if (!m_hasSendSound)
        {
            m_hasSendSound = true;
            EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, m_myDCamera.name);
        }

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

            m_vec2AloneMoveSpeed = v;
            //m_myDCamera.AloneMove(v);
        }

        float length;

        if (m_myDCamera.IsHorizontal)
        {
            //length = Input.mousePosition.x - m_vec3DragBegin.x;
            length = v.x;
        }
        else
        {
           //length = Input.mousePosition.y - m_vec3DragBegin.y;
            length = v.y;
        }

        m_myDCamera.DraggingMove(length);

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
            if (m_hasSendSound)
            {
                EventDispatcher.TriggerEvent(SettingEvent.UIUpPlaySound, m_myDCamera.name);
                m_hasSendSound = false;
            }

            if (m_bDraging)
            {
                m_bDraging = false;

                if (!m_isTryingDraggingMove)
                {
                    m_myDCamera.AloneMove(m_vec2AloneMoveSpeed);
                }
                //else
                //{
                    m_myDCamera.OnDraggingMoveDone();
                //}

                m_myDCamera.MovePage(m_vec2Drag);
            }
            else
            {
                UIButtonTween[] btList = transform.GetComponentsInChildren<UIButtonTween>(true);

                if (btList.Length > 0)
                {
                    btList[0].enabled = true;

                }
            }

            m_fCountDown = 0;
            m_isCountDown = false;
            m_isTryingDraggingMove = false;
        }
        else
        {
            m_isCountDown = true;
        }
    }

    void Update()
    {
        if (m_isCountDown)
        {
            m_fCountDown += Time.deltaTime;

            if (m_fCountDown> 0.1f)
            {
                m_fCountDown = 0;
                m_isCountDown = false;
                m_isTryingDraggingMove = true;
                
            }
        }

        //if (m_bDraging)
        //{
        //    float length;

        //    if (m_myDCamera.IsHorizontal)
        //    {
        //        length = Input.mousePosition.x - m_vec3DragBegin.x;
        //    }
        //    else
        //    {
        //        length = Input.mousePosition.y - m_vec3DragBegin.y;
        //    }

        //    m_myDCamera.DraggingMove(length);
        //}
    }
}
