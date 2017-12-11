using UnityEngine;
using System.Collections;
using Mogo.Util;

[RequireComponent(typeof(MogoSimpleMotor))]
public class SlidingDoor : GearParent
{
	public Transform target;
	public SlidingDoorGroupGear group;

	public float goSpeed = 1;
	public float comeSpeed = 1;

	protected Vector3 originalPosition { get; set; }
	protected MogoSimpleMotor motor { get; set; }

	void Start()
	{
		gearType = "SlidingDoor";
		originalPosition = transform.position;
		motor = gameObject.AddComponent<MogoSimpleMotor>();

		ID = (uint)defaultID;
		triggleEnable = true;
		stateOne = true;

		goSpeed = goSpeed < 1 ? 1 : goSpeed;
		comeSpeed = comeSpeed < 1 ? 1 : comeSpeed;

		EventDispatcher.AddEventListener<MonoBehaviour>(Events.GearEvent.MotorHandleEnd, DoorMoveEnd);
	}

	void OnDestroy()
	{
		EventDispatcher.RemoveEventListener<MonoBehaviour>(Events.GearEvent.MotorHandleEnd, DoorMoveEnd);
	}

	void OnTriggerEnter(Collider other)
	{
		//取过一遍值就不要分开两次取
		SlidingDoor temp = other.GetComponent<SlidingDoor>();
		if (temp)
		{
			LoggerHelper.Debug("SlidingDoor" + other.name);
			group.DoorCollide(this, temp);
		}
	}

	public void MoveOpen()
	{
		motor.SetSpeed(goSpeed);
		motor.MoveTo(target.position);
		group.SetDoorOpen(this);
	}

	public void MoveClose()
	{
		motor.SetSpeed(comeSpeed);
		motor.MoveTo(originalPosition);
		group.SetDoorClose(this);
	}


	protected void DoorMoveEnd(MonoBehaviour theMotor)
	{
		if (theMotor is MogoSimpleMotor)
		{
			if ((theMotor as MonoBehaviour) == motor)
			{
				EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, gearType);
			}
		}
	}
}
