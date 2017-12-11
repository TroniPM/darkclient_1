/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������DoorOfBury
// �����ߣ�Joe Mo
// �޸����б�
// �������ڣ�2013.5.23
// ģ������������֮�ŵĴ�����
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
