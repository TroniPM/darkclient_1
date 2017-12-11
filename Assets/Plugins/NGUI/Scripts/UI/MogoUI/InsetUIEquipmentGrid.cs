/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：InsetUIEquipmentGrid
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class InsetUIEquipmentGrid : MonoBehaviour
{

    public int id;

    Transform m_myTransform;
    // 为去除警告暂时屏蔽以下代码
    //bool m_bIsDragging = false;

    //void OnDrag()
    //{
    //    m_bIsDragging = true;
    //}


    //void OnPress(bool isPressed)
    //{
    //    Mogo.Util.LoggerHelper.Debug("Pressed");
    //    if (!isPressed)
    //    {
    //        if (!m_bIsDragging)
    //        {
    //            InsetUIDict.INSETUIEQUIPMENTGRIDUP(id);
    //        }
    //        m_bIsDragging = false;
    //    }
    //}

    void OnClick()
    {
        InsetUIDict.INSETUIEQUIPMENTGRIDUP(id);
    }

    public void FakeClick()
    {
        InsetUIDict.INSETUIEQUIPMENTGRIDUP(id);
    }
}
