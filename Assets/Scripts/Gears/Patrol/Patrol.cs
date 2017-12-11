using UnityEngine;
using System;
using System.Collections;

using Mogo.Util;

public class Patrol : GearParent
{
    public enum PatrolType : byte
    {
        Circle = 0,
        Reverse = 1,
        Wander = 2,
        WanderWithWait = 3
    }

    public PatrolType type;
    public Transform[] points;
    public int waitTime;
    public float speed;

    protected Action action;
    protected Transform currentPoint;
    protected int currentPointIndex;
    protected bool isGo;

    protected MogoSimpleMotor motor;

    protected uint waitTimer;

    void Start()
    {
        gearType = "Patrol";
        ID = (uint)defaultID;

        motor = GetComponent<MogoSimpleMotor>();
        if (motor == null)
            motor = gameObject.AddComponent<MogoSimpleMotor>();

        motor.SetSpeed(speed);

        currentPointIndex = 0;
        ResetAction();

        AddListeners();
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(waitTimer);
        action = null;
        RemoveListeners();
    }

    void Update()
    {
        if (action == null)
            return;
        action();
    }

    public override void AddListeners()
    {
        base.AddListeners();
        EventDispatcher.AddEventListener<MonoBehaviour>(Events.GearEvent.MotorHandleEnd, MoveEnd);
    }

    public override void RemoveListeners()
    {
        base.RemoveListeners();
        EventDispatcher.RemoveEventListener<MonoBehaviour>(Events.GearEvent.MotorHandleEnd, MoveEnd);
    }

    protected override void SetGearEventEnable(uint enableID)
    {
        base.SetGearEventEnable(enableID);
        if (enableID == ID)
            ResetAction();
    }

    protected override void SetGearDisable(uint disableID)
    {
        base.SetGearDisable(disableID);
        if (disableID == ID)
            ResetAction();
    }

    protected void ResetAction()
    {
        if (!triggleEnable)
        {
            action = null;
            return;
        }

        currentPoint = points[currentPointIndex];
        action = () =>
        {
            motor.MoveTo(currentPoint.position);
        };
    }

    protected void MoveEnd(MonoBehaviour script)
    {
        if (script != motor)
            return;

        action = null;

        waitTimer = TimerHeap.AddTimer((uint)waitTime, 0, () =>
        {
            switch (type)
            {
                case PatrolType.Circle:
                    currentPointIndex = currentPointIndex + 1 >= points.Length ? 0 : currentPointIndex + 1;
                    break;

                case PatrolType.Reverse:
                    if (isGo)
                    {
                        if (currentPointIndex + 1 >= points.Length)
                        {
                            currentPointIndex--;
                            isGo = false;
                        }
                        else
                            currentPointIndex++;
                    }
                    else
                    {
                        if (currentPointIndex - 1 < 0)
                        {
                            currentPointIndex++;
                            isGo = true;
                        }
                        else
                            currentPointIndex--;
                    }
                    break;

                case PatrolType.Wander:
                    if (points.Length < 2)
                        return;

                    int lastPointIndex = currentPointIndex;
                    do
                    {
                        currentPointIndex = RandomHelper.GetRandomInt(0, points.Length);
                    }
                    while (currentPointIndex == lastPointIndex);
                    break;

                case PatrolType.WanderWithWait:
                    currentPointIndex = RandomHelper.GetRandomInt(0, points.Length);
                    break;
            }

            ResetAction();
        });
    }
}
