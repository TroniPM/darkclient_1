/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EquipTipBtn
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-6-25
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System;

public class EquipTipBtn : MonoBehaviour
{
    Action m_action;
    // Use this for initialization
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(string text, Action action)
    {
        m_action = action;
        UILabel lbl = transform.Find("EquipTipDetaillBtnText").GetComponent<UILabel>();
        if (lbl == null)
        {
            Debug.LogError("name:" + name);
            return;
        }
        lbl.text = text;
    }

    void OnClick()
    {
        if (m_action != null)
        {
            m_action();
        }
    }

    public void FakeClick()
    {
        if (m_action != null)
        {
            m_action();
        }
    }
}
