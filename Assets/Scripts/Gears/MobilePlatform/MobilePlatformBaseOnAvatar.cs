/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MobilePlatformBaseOnAvatar
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130217
// 最后修改日期：20130220
// 模块描述：受角色驱动的平台移动
// 代码版本：测试版V3.1
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class MobilePlatformBaseOnAvatar : MobilePlatform
{
    private Transform theTransform;
    private bool isMoving;
    private bool isEndMove;

    private bool flag;
    private uint timerID;

    public float range;

    public float flushDefaultTime;


    void Start()
    {
        gearType = "MobilePlatformBaseOnAvatar";
        ID = (uint)defaultID;

        theTransform = transform;

        isMoving = false;
        isEndMove = false;

        flag = false;
        timerID = uint.MaxValue;

        triggleEnable = false;
        stateOne = true;

        sourcePosition = transform.position;
        lastPosition = transform.position;

        pathDiretion = true;
        currentPointIndex = 0;

        doors = transform.GetComponentsInChildren<MobilePlatformDoor>();
        gear = transform.GetComponentInChildren<MobilePlatformGear>();
        motor = gameObject.AddComponent<MogoSimpleMotor>();

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    void Update()
    {
        SendDistance();
    }

    public override void OnPassengerEnter(Collider other)
    {
        // LoggerHelper.Debug("Enter: " + other.gameObject.tag);
        if (other.tag == GearParent.MogoPlayerTag)
        {
            timerID = TimerHeap.AddTimer((uint)flushDefaultTime, 0, SetStart, true);

            if (pathDiretion)
            {
                foreach (MobilePlatformDoor door in doors)
                {
                    door.ChangeBeginGo();
                }
            }
            else
            {
                foreach (MobilePlatformDoor door in doors)
                {
                    door.ChangeBeginBack();
                }
            }
        }
    }

    private void SetStart(bool isStart)
    {
        flag = isStart;
    }

    public override void OnPassengerExit(Collider other)
    {
        SetStart(false);
        TimerHeap.DelTimer(timerID);
        timerID = uint.MaxValue;
        isMoving = false;
        isEndMove = false;
    }

    public override void OnPassengerStay(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            //Debug.LogError("player: " + other.gameObject.name + " " + other.gameObject.transform.position);
            //Debug.LogError("gear: " + name + " " + transform.position);
            if (flag)
            {
                CheckBeginMove(other);
                if (isMoving)
                {
                    SetMove();
                }
            }
        }
    }

    // 检测开始移动与否，取决于某一个半径
    protected bool CheckBeginMove(Collider other)
    {
        if (!isMoving && !isEndMove && Vector3.Distance(other.gameObject.transform.position, transform.position) < range)
        {
            isMoving = true;
            gear.CloseFunction();
            EventDispatcher.TriggerEvent(Events.GearEvent.TrapBegin, gearType);
            foreach (MobilePlatformDoor door in doors)
            {
                door.ChangeMoving();
            }
        }

        return isMoving;
    }

    protected override void SetEndMove()
    {
        EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, gearType);
        isMoving = false;
        isEndMove = true;
        base.SetEndMove();
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            SetStart(false);
            TimerHeap.DelTimer(timerID);
            timerID = uint.MaxValue;
            isMoving = false;
            isEndMove = false;
            base.SetEndMove();
            base.SetGearEventStateTwo(stateTwoID);
        }
    }
}