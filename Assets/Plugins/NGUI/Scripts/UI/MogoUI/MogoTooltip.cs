/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoTooltip
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class MogoTooltip : MonoBehaviour 
{
    #region 私有变量
    private static MogoTooltip m_instance;
    #endregion

    #region 公共变量
    public static MogoTooltip Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = GameObject.Find("MogoTooltip");
                if (obj)
                {
                    m_instance = obj.transform.GetComponentsInChildren<MogoTooltip>(true)[0];
                }
            }

            return m_instance;

        }
    }
    
    #endregion

    void OnClick()
    {
        gameObject.SetActive(false);
    }

    public void ShowTooltip(string title, string text, Transform trans)
    {
        m_instance.transform.GetComponentsInChildren<UILabel>(true)[0].text = text;
        m_instance.transform.parent = trans;
        m_instance.transform.localPosition = new Vector3(0, 0, -10);
        m_instance.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        gameObject.SetActive(true);


    }
}
