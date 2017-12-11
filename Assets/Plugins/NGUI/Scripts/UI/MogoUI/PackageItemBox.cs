/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：NewBehaviourScript
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class PackageItemBox : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public int id = 0;
    bool m_isDragging = false;

    void OnDrag(Vector2 v)
    {
        m_isDragging = true;
    }

    void OnPress(bool isPressed)
    {
        if (!m_isDragging)
        {
            if (isPressed)
            {
            }
            else
            {
                EventDispatcher.TriggerEvent(MenuUIDict.MenuUIEvent.PACKAGEGRIDUP, id);
            }
        }
        else
        {
            if (isPressed)
            {

            }
            else
            {
                m_isDragging = false;
            }
        }

    }

    public void FakeClick()
    {
        EventDispatcher.TriggerEvent(MenuUIDict.MenuUIEvent.PACKAGEGRIDUP, id);
    }

    //void OnClick()
    //{
    //    if (!m_isDragging)
    //    {
    //        EventDispatcher.TriggerEvent(MenuUIDict.MenuUIEvent.PACKAGEGRIDUP, id);
    //        m_isDragging = false;
    //    }
    //}

    //void OnPress(bool isPressed)
    //{
    //    if (!isPressed)
    //    {
    //        m_isDragging = false;
    //    }
    //}

}
