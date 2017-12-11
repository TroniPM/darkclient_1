/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：AutoGateGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130219
// 最后修改日期：20130220
// 模块描述：自动门的触发齿轮
// 代码版本：V1.2
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class AutoGateGear : GearParent {

    protected AutoGateDoor[] doors;

    void Start()
    {
        gearType = "AutoGateGear";
        doors = transform.parent.gameObject.GetComponentsInChildren<AutoGateDoor>();
        triggleEnable = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                foreach (AutoGateDoor door in doors)
                {
                    door.Open();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                foreach (AutoGateDoor door in doors)
                {
                    door.Close();
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
    }
}
