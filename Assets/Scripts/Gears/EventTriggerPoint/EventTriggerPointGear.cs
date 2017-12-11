/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EventTriggerPoint
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130220
// 最后修改日期：20130220
// 模块描述：事件触发点，当前默认操作是使Avatar定身一定时间
// 代码版本：V1.0
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class EventTriggerPointGear : GearParent
{
    public bool isTriggleOnce;
    public int defaultDelayTime;

    protected bool isTrigging;

    void Start()
    {
        gearType = "EventTriggerPoint";
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
        if (isTrigging)
        {
            if (triggleEnable)
            {
                SetControl(other.gameObject, false);
                TimerHeap.AddTimer((uint)defaultDelayTime, 0, SetControl, other.gameObject, true);
            }
        }
    }

    #endregion


    protected void SetControl(GameObject theGameObject, bool isMovable)
    {
        theGameObject.GetComponentInChildren<ActorMyself>().SetControl(isMovable);
        if (isTriggleOnce && isMovable)
            isTrigging = false;
    }
}

