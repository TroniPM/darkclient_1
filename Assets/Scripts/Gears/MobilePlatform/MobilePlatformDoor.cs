/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：Door
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130218
// 最后修改日期：20130219
// 模块描述：移动平台的门，作用于面（单向），不绘制，瞬间开关
// 代码版本：V1.1
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

[RequireComponent(typeof(Transform))]
public class MobilePlatformDoor : GearParent
{
    public Vector3 beginGo;
    public Vector3 beginBack;
    public Vector3 moving;

    void Start()
    {
        ID = (uint)defaultID;
        gearType = "MobilePlatformDoor";
    }

    public void ChangeBeginGo()
    {
        LoggerHelper.Debug("ChangeBeginGo");
        transform.localPosition = beginGo;
    }

    public void ChangeBeginBack()
    {
        LoggerHelper.Debug("ChangeBeginBack");
        transform.localPosition = beginBack;
    }

    public void ChangeMoving()
    {
        LoggerHelper.Debug("ChangeMoving");
        transform.localPosition = moving;
    }
}
