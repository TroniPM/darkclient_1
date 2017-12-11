/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ConveyPointGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：
// 最后修改日期：
// 模块描述：
// 代码版本：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System;
using Mogo.Util;

public class ServerTeleportPointDes : GearParent
{
    public string lable;

    void Awake()
    {
        gearType = "TeleportPointDes";
        ID = (uint)defaultID;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, "TeleportPointDes");
                triggleEnable = false;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, "TeleportPointDes");
                triggleEnable = false;
            }
        }
    }
}
