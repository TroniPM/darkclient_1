/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：AutoGateDoor
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130219
// 最后修改日期：20130220
// 模块描述：自动门的门，逐步开关
// 代码版本：V1.2
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

[RequireComponent(typeof(MogoSimpleMotor))]
public class AutoGateDoor : GearParent
{
    public Space coordinate;
    public Vector3 doorOpened;
    public Vector3 doorClosed;
    public int closeDelayTime;

    protected uint closingID;

    void Start()
    {
        gearType = "AutoGateDoor";
        closingID = uint.MaxValue;
    }

    public void Open()
    {
        MoveDoor(doorOpened, coordinate);
    }

    public void Close()
    {
        TimerHeap.DelTimer(closingID);
        closingID = TimerHeap.AddTimer((uint)(closeDelayTime), 0, MoveDoor, doorClosed, coordinate);
    }

    private void MoveDoor(Vector3 target, Space coordinate)
    {
        // todo 
   //     gameObject.GetComponent<MogoSimpleMotor>().MoveTo(target, coordinate);
    }
}
