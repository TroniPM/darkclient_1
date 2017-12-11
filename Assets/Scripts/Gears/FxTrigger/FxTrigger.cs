using UnityEngine;
using System.Collections;

using Mogo.Util;

public class FxTrigger : GearParent
{
	public int fxID;
	public bool isTriggerRepeat;
	protected SfxHandler handler;
	public float xScale = 1f;
	public float yScale = 1f;
	public float zScale = 1f;

	void Start()
	{
		gearType = "FxTrigger";
		triggleEnable = true;
		stateOne = true;        
		ID = (uint)defaultID;
		handler = gameObject.AddComponent<SfxHandler>();

		AddListeners();
	}

	void OnDestroy()
	{
		RemoveListeners();
	}


	#region 碰撞触发

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == GearParent.MogoPlayerTag)
		{
			if (triggleEnable)
			{
				stateOne = false;

				if (!isTriggerRepeat)
					triggleEnable = false;

				handler.HandleFx(fxID, null,
					(go, guid) =>
					{
						go.transform.parent = transform.parent;
						go.transform.localPosition = Vector3.zero;
						go.transform.localRotation = new Quaternion(0, 0, 0, go.transform.localRotation.w);
						go.transform.localScale = new Vector3(xScale, yScale, zScale);
					});
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == GearParent.MogoPlayerTag)
		{
			if (triggleEnable)
			{
				if (stateOne)
				{
					stateOne = false;

					if (!isTriggerRepeat)
						triggleEnable = false;

					handler.HandleFx(fxID, null,
						(go, guid) =>
						{
							go.transform.parent = transform.parent;
							go.transform.localPosition = Vector3.zero;
							go.transform.localRotation = new Quaternion(0, 0, 0, go.transform.localRotation.w);
							go.transform.localScale = new Vector3(xScale, yScale, zScale);
						});
				}
			}
		}
	}

	#endregion


	#region 机关事件

	protected override void SetGearEventStateOne(uint stateOneID)
	{
		if (stateOneID == ID)
		{
			handler.RemoveFXs(fxID);
			base.SetGearEventStateOne(stateOneID);
		}
	}

	protected override void SetGearEventStateTwo(uint stateTwoID)
	{
		if (stateTwoID == ID)
		{
			handler.HandleFx(fxID, null,
				(go, guid) =>
				{
					go.transform.parent = transform.parent;
					go.transform.localPosition = Vector3.zero;
					go.transform.localRotation = new Quaternion(0, 0, 0, go.transform.localRotation.w);
					go.transform.localScale = new Vector3(xScale, yScale, zScale);
				});
			base.SetGearEventStateTwo(stateTwoID);
		}
	}

	#endregion


	#region 断线重连

	protected override void SetGearStateOne(uint stateOneID)
	{
		if (stateOneID == ID)
		{
			handler.RemoveFXs(fxID);
			base.SetGearStateOne(stateOneID);
		}
	}

	protected override void SetGearStateTwo(uint stateTwoID)
	{
		if (stateTwoID == ID)
		{
			handler.HandleFx(fxID, null,
				(go, guid) =>
				{
					go.transform.parent = transform.parent;
					go.transform.localPosition = Vector3.zero;
					go.transform.localRotation = new Quaternion(0, 0, 0, go.transform.localRotation.w);
					go.transform.localScale = new Vector3(xScale, yScale, zScale);
				});
			base.SetGearStateTwo(stateTwoID);
		}
	}

	#endregion

}

