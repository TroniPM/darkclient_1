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
using Mogo.Util;

public class EquipRecommendUIGridButton : MonoBehaviour 
{
     private int m_access;
     public int Access
     {
         get { return m_access; }
         set
         {
             m_access = value;
         }
     }

    private int m_accessType;
    public int AccessType
    {
        get { return m_accessType; }
        set
        {
            m_accessType = value;
        }
    }



    void OnClick()
    {
        if (EquipRecommendUIDict.EQUIPRECOMMENDLINKBTNUP != null)
            EquipRecommendUIDict.EQUIPRECOMMENDLINKBTNUP(Access, AccessType);
    }
}
