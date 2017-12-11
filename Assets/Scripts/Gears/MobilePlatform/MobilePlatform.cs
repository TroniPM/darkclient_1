/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MobilePlatform
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130218
// 最后修改日期：20130220
// 模块描述：移动平台基类
// 代码版本：测试版V1.3
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;

public class MobilePlatform : GearParent
{

    protected MobilePlatformDoor[] doors;
    protected MobilePlatformGear gear;

    public Vector3[] pathPoints;
    protected int currentPointIndex;
    protected bool pathDiretion;

    protected Vector3 sourcePosition;

    protected Vector3 lastPosition;

    protected MogoSimpleMotor motor;
    void Start()
    {
        gearType = "MobilePlatform";
        triggleEnable = true;
        stateOne = true;

        sourcePosition = transform.position;
        lastPosition = transform.position;

        pathDiretion = true;
        currentPointIndex = 0;

        doors = transform.GetComponentsInChildren<MobilePlatformDoor>();
        gear = transform.GetComponentInChildren<MobilePlatformGear>();
        motor = gameObject.AddComponent<MogoSimpleMotor>();
    }

    protected void SendDistance()
    {
        Vector3 distance = transform.position - lastPosition;
        gear.OnPlatformMove(distance);
        lastPosition = transform.position;
    }

    public virtual void OnPassengerEnter(Collider other)
    {
    }

    public virtual void OnPassengerExit(Collider other)
    {
    }

    public virtual void OnPassengerStay(Collider other)
    {
    }

    protected virtual void WaitForPassenger()
    {
    }

    protected void SetMove()
    {
        if (transform.position == sourcePosition + pathPoints[currentPointIndex])
        // if (Vector3.Distance(transform.position, sourcePosition + pathPoints[currentPointIndex])< 0.01)
        {
            if (pathDiretion)
                currentPointIndex++;
            else
                currentPointIndex--;

            if ((currentPointIndex == pathPoints.Length && pathDiretion) || (currentPointIndex == -1 && !pathDiretion))
            {
                if (pathDiretion)
                    currentPointIndex--;
                else
                    currentPointIndex++;

                SetEndMove();
            }
            else
            {
                SetNextMove();
            }
        }
        else
        {
            SetNextMove();
        }
    }

    protected virtual void SetNextMove()
    {
        try
        {
            if (motor)
                if (transform.parent != null)
                    motor.MoveTo(pathPoints[currentPointIndex] + transform.parent.position);
                else
                    motor.MoveTo(pathPoints[currentPointIndex] + transform.position);
            else
                Debug.LogError("null motor");
        }
        catch (System.Exception ex)
        {
            LoggerHelper.Except(ex);
        }
    }

    protected virtual void SetEndMove()
    {
        pathDiretion = !pathDiretion;

        if (pathDiretion)
        {
            stateOne = true;
            foreach (MobilePlatformDoor door in doors)
            {
                door.ChangeBeginGo();
            }
        }
        else
        {
            stateOne = false;
            foreach (MobilePlatformDoor door in doors)
            {
                door.ChangeBeginBack();
            }
        }
    }

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        base.SetGearEventStateOne(stateOneID);
        if (stateOneID == ID)
        {
            Mogo.Util.LoggerHelper.Debug("In SetGearStateOne " + ID);
            transform.position = sourcePosition + pathPoints[0];
        }
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
        {
            Mogo.Util.LoggerHelper.Debug("In SetGearStateOne " + ID);
            transform.position = sourcePosition + pathPoints[1];
        }
    }

    protected override void SetGearStateOne(uint stateOneID)
    {
        base.SetGearStateOne(stateOneID);
        if (stateOneID == ID)
        {
            Mogo.Util.LoggerHelper.Debug("In SetGearStateOne " + ID);
            transform.position = sourcePosition + pathPoints[0];
        }
    }

    protected override void SetGearStateTwo(uint stateTwoID)
    {
        base.SetGearStateTwo(stateTwoID);
        if (stateTwoID == ID)
        {
            Mogo.Util.LoggerHelper.Debug("In SetGearStateOne " + ID);
            transform.position = sourcePosition + pathPoints[1];
        }
    }
}
