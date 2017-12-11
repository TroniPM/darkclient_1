/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：GearParent
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130217
// 最后修改日期：20130910
// 模块描述：机关父类，每个机关功能通过一个或多个机关子类组合实现
// 代码版本：发布版V3.0
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class GearParent : MonoBehaviour
{
	#region 静态常量

	public static readonly string MogoPlayerTag = "Player";

	public static readonly byte EnableByte = 8;
	public static readonly byte DisableByte = 4;
	public static readonly byte StateOneByte = 2;
	public static readonly byte StateTwoByte = 1;

	#endregion


	#region 动态变量

	public int defaultID = 0;
	public uint ID = 0;
	public string gearType { get; protected set; }

	public bool triggleEnable = false;
	public bool stateOne = false;

	#endregion


	#region 创建与销毁

	void Start()
	{
		gearType = "GearParent";
		ID = (uint)defaultID;
		triggleEnable = true;
		stateOne = true;

		AddListeners();
	}

	void OnDestroy()
	{
		RemoveListeners();
	}

	#endregion


	#region 事件监听

	public virtual void AddListeners()
	{
		EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearEnable, SetGearEnable);
		EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearDisable, SetGearDisable);
		EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearStateOne, SetGearStateOne);
		EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearStateTwo, SetGearStateTwo);

		EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearEventEnable, SetGearEventEnable);
		EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearEventDisable, SetGearEventDisable);
		EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearEventStateOne, SetGearEventStateOne);
		EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearEventStateTwo, SetGearEventStateTwo);
	}

	public virtual void RemoveListeners()
	{
		EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearEnable, SetGearEnable);
		EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearDisable, SetGearDisable);
		EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearStateOne, SetGearStateOne);
		EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearStateTwo, SetGearStateTwo);

		EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearEventEnable, SetGearEventEnable);
		EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearEventDisable, SetGearEventDisable);
		EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearEventStateOne, SetGearEventStateOne);
		EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearEventStateTwo, SetGearEventStateTwo);
	}

	#endregion


	#region 机关事件

	protected virtual void SetGearEventEnable(uint enableID)
	{
		if (enableID == ID)
		{
			triggleEnable = true;
			EventDispatcher.TriggerEvent(Events.GearEvent.UploadAllGear);
		}
	}

	protected virtual void SetGearEventDisable(uint disableID)
	{
		if (disableID == ID)
		{
			triggleEnable = false;
			EventDispatcher.TriggerEvent(Events.GearEvent.UploadAllGear);
		}
	}

	protected virtual void SetGearEventStateOne(uint stateOneID)
	{
		if (stateOneID == ID)
		{
			stateOne = true;
			EventDispatcher.TriggerEvent(Events.GearEvent.UploadAllGear);
		}
	}

	protected virtual void SetGearEventStateTwo(uint stateTwoID)
	{
		if (stateTwoID == ID)
		{
			stateOne = false;
			EventDispatcher.TriggerEvent(Events.GearEvent.UploadAllGear);
		}
	}

	#endregion


	#region 断线重连

	protected virtual void SetGearEnable(uint enableID)
	{
		if (enableID == ID)
		{
			triggleEnable = true;
		}
	}

	protected virtual void SetGearDisable(uint disableID)
	{
		if (disableID == ID)
		{
			triggleEnable = false;
		}
	}

	protected virtual void SetGearStateOne(uint stateOneID)
	{
		if (stateOneID == ID)
		{
			stateOne = true;
		}
	}

	protected virtual void SetGearStateTwo(uint stateTwoID)
	{
		if (stateTwoID == ID)
		{
			stateOne = false;
		}
	}

	#endregion

}
