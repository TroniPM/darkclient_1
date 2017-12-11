/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：FlameThrower
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130219
// 最后修改日期：20130716
// 模块描述：火焰喷射器，用于减血
// 代码版本：V2.0
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using Mogo.Util;

public class FlameThrower : GearParent
{
    public float percentage;
    public int damageMin;
    public int damageMax;

    public int damageInterval;

    protected uint timerID;

    void Start()
    {
        gearType = "FlameThrower";

        ID = (uint)defaultID;
        triggleEnable = true;
        stateOne = true;

        if (percentage < 0)
            percentage = 0;
        else if (percentage > 1)
            percentage = 1;

        if (damageMin < 0)
            damageMin = 0;
        else if (damageMin > damageMax)
            damageMin = damageMax;

        if (damageMax < 0)
            damageMax = 0;

        if (damageInterval < 0)
            damageInterval = 0;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }
    

    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (stateOne && triggleEnable)
            {
                timerID = TimerHeap.AddTimer((uint)damageInterval, 0, SetDamage);
                stateOne = false;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (stateOne && triggleEnable)
            {
                timerID = TimerHeap.AddTimer((uint)damageInterval, 0, SetDamage);
                stateOne = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            TimerHeap.DelTimer(timerID);
            stateOne = true;
        }
    }

    #endregion


    private void SetDamage()
    {
        EventDispatcher.TriggerEvent(Events.GearEvent.Damage, MogoWorld.thePlayer.ID, 9003, (int)2, CalcDamage());
        stateOne = true;
    }

    private int CalcDamage()
    {
        return (int)(MogoWorld.thePlayer.hp * percentage + RandomHelper.GetRandomInt(damageMin, damageMax));
    }
}
