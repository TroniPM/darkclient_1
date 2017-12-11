/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DragonMatchPlayerModelPool
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;

public class DragonMatchPlayerModelPool
{
    static DragonMatchPlayerModelPool m_instance;
    public static DragonMatchPlayerModelPool Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new DragonMatchPlayerModelPool();
            }
            return m_instance;
        }

    }

    public void InitPool(Action onLoaded)
    {
        if (onLoaded != null)
            onLoaded();
    }

    internal void RecirleAll()
    {
        throw new NotImplementedException();
    }
}