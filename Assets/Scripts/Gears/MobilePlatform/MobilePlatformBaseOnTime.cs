/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MobilePlatformBaseOnTime
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130217
// 最后修改日期：20130220
// 模块描述：受时间驱动的平台移动
// 代码版本：测试版V3.1
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;

public class MobilePlatformBaseOnTime : MobilePlatform
{
	public int waitTime;
	private uint defaultWaitTime;

    void Start()
	{
		gearType = "MobliPlatformBaseOnTime";


        doors = transform.GetComponentsInChildren<MobilePlatformDoor>();
        gear = transform.GetComponentInChildren<MobilePlatformGear>();
        motor = gameObject.AddComponent<MogoSimpleMotor>();

		defaultWaitTime = (uint)waitTime;

		TimerHeap.AddTimer(defaultWaitTime, 0, WaitForPassenger);
	}
	
	void Update ()
	{
        SendDistance();
	}

	public override void OnPassengerEnter(Collider other)
	{
	}

	public override void OnPassengerExit(Collider other)
	{
	}

	public override void OnPassengerStay(Collider other)
	{
	}

    protected override void WaitForPassenger()
	{
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

		TimerHeap.AddTimer(0, 0, SetMove);
	}

	protected override void SetEndMove()
	{
		base.SetEndMove();
		TimerHeap.AddTimer(defaultWaitTime, 0, WaitForPassenger);
	}

	protected override void SetNextMove()
	{
		base.SetNextMove();
		TimerHeap.AddTimer(0, 0, SetMove);
	}
}
