/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoUIListener
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：2013/9/12
// 模块描述：按钮事件监听器
 * 用法实例：
 * 在需要监听事件的按钮上 AddComponent<MogoButtonListener>().MogoOnClick = (() => { //do sth });
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;

public class MogoUIListener : MonoBehaviour
{
    public Action m_onClick;
    public Action<Vector2> MogoOnDrag;
    /// <summary>
    /// 只能设置一个Action,不能使用+=
    /// </summary>
    public Action MogoOnClick
    {
        set
        {
            m_onClick = null;
            m_onClick = value;
        }
    }


    void OnClick()
    {
        if (m_onClick != null)
        {
            m_onClick();
        }
    }

    void OnDrag(Vector2 delta)
    {
        if (MogoOnDrag != null)
        {
            MogoOnDrag(delta);
        }
    }
}