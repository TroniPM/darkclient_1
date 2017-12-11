/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：SwampGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130219
// 最后修改日期：20130219
// 模块描述：沼泽的齿轮，用于减速，提供减血功能
// 代码版本：V1.0
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using Mogo.Util;

public class SwampGear : GearParent
{
	public float coefficient;
    //public float damage;
    //public int damageInterval;

	protected Dictionary<GameObject, uint> avatars;

    void Start()
    {
        gearType = "SwampGear";
        avatars = new Dictionary<GameObject, uint>();

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

	void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
		{
            if (triggleEnable)
            {
                // avatars.Add(other.gameObject, TimerHeap.AddTimer(0, damageInterval, SetDamage, other.gameObject, damage));
                // other.gameObject.GetComponentInChildren<ActorMyself>().SpeedDown(coefficient);

                MogoWorld.thePlayer.SetSpeedReduce(coefficient);
            }
		}
	}

	void OnTriggerExit(Collider other)
	{
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
		{
            if (triggleEnable)
            {
                //uint timerID;
                //if (avatars.TryGetValue(other.gameObject, out timerID))
                //{
                //    TimerHeap.DelTimer(timerID);
                //}
                //avatars.Remove(other.gameObject);
                //other.gameObject.GetComponentInChildren<ActorMyself>().SpeedUp();

                MogoWorld.thePlayer.SetSpeedRecover();
            }
		}
	}

	void OnTriggerStay(Collider other)
	{
	}

    //private void SetDamage(GameObject gameObject, float damage = 0)
    //{
    //    gameObject.GetComponentInChildren<ActorMyself>().SetDamage(damage);
    //}
}
