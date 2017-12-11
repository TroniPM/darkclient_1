/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������MobilePlatform
// �����ߣ�Key Pan
// �޸����б�Key Pan
// �������ڣ�20130218
// ����޸����ڣ�20130220
// ģ���������ƶ�ƽ̨����
// ����汾�����԰�V1.3
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

[RequireComponent(typeof(MogoSimpleMotor))]
public class PathPointVisableMobilePlatform : GearParent {

    protected MobilePlatformDoor[] doors;
    protected MobilePlatformGear gear;

    public Transform[] pathPointTransform;

	protected List<Vector3> pathPoints;
	protected int currentPointIndex;
	protected bool pathDiretion;

	protected Vector3 lastPosition;

    void Start()
	{
        gearType = "PathPointVisableMobilePlatform";

        lastPosition = transform.position;

        doors = transform.GetComponentsInChildren<MobilePlatformDoor>();
        gear = transform.GetComponentInChildren<MobilePlatformGear>();

        pathPoints.Add(transform.position);

        foreach (Transform pathPoint in pathPointTransform)
        {
            pathPoints.Add(pathPoint.position);
        }
    }

	protected void SendDistance()
	{
		Vector3 distance = transform.position - lastPosition;
		gear.OnPlatformMove(distance);
		lastPosition = transform.position;
	}

	public virtual void OnPassengerEnter(Collider other)
	{
	}

	public virtual void OnPassengerExit(Collider other)
	{
	}

	public virtual void OnPassengerStay(Collider other)
	{
	}

	protected virtual void WaitForPassenger()
	{
	}

	protected void SetMove()
    {
		if (transform.position == pathPoints[currentPointIndex])
		{
			if (pathDiretion)
				currentPointIndex++;
			else
				currentPointIndex--;

			if ((currentPointIndex == pathPoints.Count && pathDiretion) || (currentPointIndex == -1 && !pathDiretion))
			{
				if (pathDiretion)
					currentPointIndex--;
				else
					currentPointIndex++;

				SetEndMove();
			}
			else
			{
				SetNextMove();
			}
		}
		else
		{
			SetNextMove();
		}
	}

	protected virtual void SetNextMove()
	{
        foreach (MobilePlatformDoor door in doors)
        {
            door.ChangeMoving();
        }

        gameObject.GetComponent<MogoSimpleMotor>().MoveTo(pathPoints[currentPointIndex]);
	}

	protected virtual void SetEndMove()
	{
		pathDiretion = !pathDiretion;

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
	}
}
