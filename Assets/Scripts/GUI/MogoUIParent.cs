/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoUIParent
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：UI管理父类，实现一些都要用到的数据初设化
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;

public class MogoUIParent: MonoBehaviour
{
    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();
    private bool m_isInit = false;
    virtual protected void Init()
    {
        if (m_isInit) return;
        FillFullNameData(transform);
        m_isInit = true;
    }

    /// <summary>
    /// 得到子transform
    /// </summary>
    /// <param name="str">go.name,不需带路径</param>
    /// <returns></returns>
    protected Transform GetUIChild(string str)
    {
        //Mogo.Util.LoggerHelper.Debug(str);
        return transform.Find(m_widgetToFullName[str]);
    }

    /// <summary>
    /// 保存子transform在一个dic下
    /// </summary>
    /// <param name="rootTransform"></param>
    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            try
            {
                m_widgetToFullName.Add(rootTransform.GetChild(i).name, Utils.GetFullName(transform, rootTransform.GetChild(i)));
            }
            catch
            {
                Mogo.Util.LoggerHelper.Debug("rootTransform.GetChild(i):" + rootTransform.GetChild(i).name);
            }

            FillFullNameData(rootTransform.GetChild(i));
        }
    }
}