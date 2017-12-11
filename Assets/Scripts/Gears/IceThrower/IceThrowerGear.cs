/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：IceThrowerGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130219
// 最后修改日期：20130220
// 模块描述：冰面的齿轮，用于减速
// 代码版本：V1.1
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using Mogo.Util;

public class IceThrowerGear : GearParent
{
    public float coefficient;

    void Start()
    {
        gearType = "IceThrowerGear";
        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                other.gameObject.GetComponentInChildren<ActorMyself>().SpeedDown(coefficient);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                other.gameObject.GetComponentInChildren<ActorMyself>().SpeedUp();
            }
        }
    }

    #endregion
}
