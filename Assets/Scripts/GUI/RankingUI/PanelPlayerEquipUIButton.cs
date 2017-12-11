#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class PanelPlayerEquipUIButton : MonoBehaviour 
{
    private int m_equipSolt;
    public int EquipSolt
    {
        get
        {
            return m_equipSolt;
        }
        set
        {
            m_equipSolt = value;
        }
    }

    void OnClick()
    {
        if (PanelPlayerEquipUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        PanelPlayerEquipUIDict.ButtonTypeToEventUp[transform.name](m_equipSolt);
    }
}
