using UnityEngine;
using System.Collections;
using Mogo.Util;

public class Meteorite : GearParent
{
	public float speed;
	public float rotationSpeed;

	public int shakeCamAnimID;
	public float shakeCamAnimTime;

	public int deleteTime;

	public Transform target;

	protected MogoSimpleMotor motor { get; set; }
	protected SfxHandler handler { get; set; }

	protected Vector3 originPosition { get; set; }
	protected Transform hitPosition { get; set; }

	protected uint deleteTimer { get; set; }
    protected uint flyBackDelayTimer { get; set; }

	protected int hasHit { get; set; }

	void Start()
	{
		gearType = "Meteorite";

		ID = (uint)defaultID;
		triggleEnable = false;
		stateOne = false;

		motor = gameObject.AddComponent<MogoSimpleMotor>();
		handler = gameObject.AddComponent<SfxHandler>();

		originPosition = transform.position;

		deleteTimer = uint.MaxValue;
        flyBackDelayTimer = uint.MaxValue;

		hasHit = 0;

		transform.LookAt(target);

		AddListeners();
		
		CalculateHitPosition();
	}

	void OnDestroy()
	{
		RemoveListeners();
        TimerHeap.DelTimer(flyBackDelayTimer);
		TimerHeap.DelTimer(deleteTimer);
	}

	public void CalculateHitPosition()
	{   
		RaycastHit hitInfo;

		if (Physics.Raycast(originPosition, transform.forward, out hitInfo, 1000, 1 << 9))
		{
			GameObject temp = new GameObject();
			temp.transform.position = hitInfo.point;
			temp.name = gameObject.name + "_hitPosition";
			hitPosition = temp.transform;
		}
		else
		{
			hitPosition = transform;
		}

		// BeginDrop();
	}

	public void BeginDrop()
	{
		motor.SetSpeed(speed);
		motor.SetRotateSpeed(0, 0, rotationSpeed);

		transform.LookAt(target.position);

		motor.MoveTo(target.position);
	}

	public void FlyBack()
	{
		motor.SetMoveSpeed(new Vector3(0, 0, (originPosition - target.position).z));
		motor.SetRotateSpeed(0, 0, -rotationSpeed);

		transform.LookAt(target.position);

		motor.Move();

		deleteTimer = TimerHeap.AddTimer((uint)deleteTime, 0, () => 
		{
			if (gameObject != null)
				Destroy(this.gameObject);
			
			if (hitPosition != null)
				Destroy(hitPosition.gameObject);
		});
	}

	public void HitGround()
	{
		Mogo.Util.LoggerHelper.Debug("HitGround");

		motor.StopRotate();

		handler.HandleFx(6004, hitPosition, (go, i) => 
		{
			go.transform.parent = hitPosition;
			go.transform.localPosition = Vector3.zero;
		});
		// handler.RemoveFXs(6005);

		transform.Find("fx_fbx_aerolite02/fx_fbx_aerolite_tail").gameObject.SetActive(false);

		TimerHeap.AddTimer(300, 0, () =>
			{
				handler.HandleFx(6006, hitPosition, (go, i) =>
				{
					go.transform.parent = hitPosition;
					go.transform.localPosition = Vector3.zero;
				});
			});

		MogoMainCamera.Instance.Shake(shakeCamAnimID, shakeCamAnimTime);
	}

	public void GatherGround()
	{
		Mogo.Util.LoggerHelper.Debug("GatherGround");

		if (hitPosition != gameObject.transform)
			foreach (Transform t in hitPosition)
				Destroy(t.gameObject);

		handler.HandleFx(6005, hitPosition, (go, i) =>
		{
			go.transform.parent = hitPosition;
			go.transform.localPosition = Vector3.zero;
		});

		handler.RemoveFXs(6004);
		handler.RemoveFXs(6006);
	}

	#region 碰撞触发

	void OnTriggerEnter(Collider other)
	{
		//Mogo.Util.LoggerHelper.Debug(other.gameObject.name + " " + other.gameObject.layer);

		if (other.gameObject.layer == 9)
		{
			if (hasHit == 0 && hitPosition != transform)
			{
				triggleEnable = true;
				HitGround();
				base.SetGearStateOne(ID);
			}

			hasHit++;
		}
	}

	void OnTriggerExit(Collider other)
	{
		Mogo.Util.LoggerHelper.Debug(other.gameObject.name + " " + other.gameObject.layer);

		if (other.gameObject.layer == 9)
		{
			hasHit--;

			if (hasHit == 0 && hitPosition != transform)
			{
				// GatherGround();
				base.SetGearStateTwo(ID);
			}
		}
	}

	#endregion


	#region 机关事件

	protected override void SetGearEventEnable(uint enableID)
	{
		if (enableID == ID)
		{
			base.SetGearEventEnable(enableID);
			BeginDrop();
		}
	}

	protected override void SetGearEventStateOne(uint stateOneID)
	{
		if (stateOneID == ID)
		{
			base.SetGearEventStateOne(stateOneID);
			transform.position = target.position;
			HitGround();
		}
	}

	protected override void SetGearEventStateTwo(uint stateTwoID)
	{
        if (stateTwoID == ID && stateOne)
        {
            base.SetGearEventStateTwo(stateTwoID);
            GatherGround();
            flyBackDelayTimer = TimerHeap.AddTimer(500, 0, () =>
            {
                transform.Find("fx_fbx_aerolite02/fx_fbx_aerolite_tail").gameObject.SetActive(true);

                MogoMainCamera.Instance.Shake(shakeCamAnimID, shakeCamAnimTime);
                FlyBack();
            });
        }
	}

	#endregion


	#region 断线重连

	protected override void SetGearEnable(uint enableID)
	{
		if (enableID == ID)
		{
			base.SetGearEnable(enableID);
			BeginDrop();
		}
	}

	protected override void SetGearStateOne(uint stateOneID)
	{
		if (stateOneID == ID)
		{
			base.SetGearStateOne(stateOneID);
			transform.position = target.position;
			HitGround();
		}
	}

	protected override void SetGearStateTwo(uint stateTwoID)
	{
		if (stateTwoID == ID && stateOne)
		{
			base.SetGearStateTwo(stateTwoID);
			FlyBack();
		}
	}

	#endregion
}

