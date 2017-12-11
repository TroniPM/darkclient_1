/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MobilePlatformGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130218
// 最后修改日期：20130220
// 模块描述：移动平台的齿轮，用于检测碰撞，确定乘客，分发平台移动数据
// 代码版本：V1.2
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class MobilePlatformGear : GearParent {

    private MobilePlatform platform;
    private List<Transform> avatars;

    private bool isFirstIn;

    void Start()
    {
        ID = (uint)defaultID;
        gearType = "MobilePlatformGear";

        platform = gameObject.transform.parent.gameObject.GetComponent<MobilePlatform>();
        avatars = new List<Transform>();

        //triggleEnable = true;
        //stateOne = false;

        isFirstIn = true;

        AddListeners();
    }

    void OnDestroy()
    {
        if (MogoWorld.thePlayer != null)
            MogoWorld.thePlayer.SetGravity();
        RemoveListeners();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable && isFirstIn)
            {
                EventDispatcher.TriggerEvent(Events.GearEvent.LiftEnter);

                avatars.Add(other.gameObject.transform);
                other.GetComponent<ActorMyself>().theEntity.RemoveGravity();
                platform.OnPassengerEnter(other);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                avatars.Remove(other.gameObject.transform);

                other.GetComponent<ActorMyself>().theEntity.SetGravity();

                if (avatars.Count == 0)
                    platform.OnPassengerExit(other);

                EventDispatcher.TriggerEvent(Events.GearEvent.UploadAllGear);
            }
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                platform.OnPassengerStay(other);
            }
        }
    }

    public void OnPlatformMove(Vector3 distance)
    {
        if (avatars != null)
            foreach (Transform avatar in avatars)
            {
                // todo
                // avatar.transform.position += distance;
                avatar.GetComponentInChildren<ActorMyself>().SetMoveToDirectly(distance);
            }
    }

    public void CloseFunction()
    {
        isFirstIn = false;
    }
}
