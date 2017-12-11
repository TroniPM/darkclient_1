/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DoorOfBury
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013.5.23
// 模块描述：湮灭之门的传送门
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;


public class DoorOfBury : MonoBehaviour
{
    void Awake()
    {
        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.center = Vector3.zero;
        sc.radius = 3;
        sc.isTrigger = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            DoorOfBurySystem.Instance.OnDoorShow();
            ControlStick.instance.Reset();
        }
    }
}
