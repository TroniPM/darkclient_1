using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class SlidingDoorGroupGear : GearParent
{
    protected enum SlidingDoorState
    {
        // Open,
        // Close,
        MovingOpen,
        MovingClose
    }

    public SlidingDoor[] doors;

    protected Dictionary<SlidingDoor, SlidingDoorState> doorStates
    {
        get;
        set;
    }

    void Start()
    {
        gearType = "SlidingDoorGroupGear";
        doorStates = new Dictionary<SlidingDoor, SlidingDoorState>();
        for (int i = 0; i < doors.Length; i++)
        {
            doorStates.Add(doors[i], SlidingDoorState.MovingClose);
        }
        triggleEnable = true;
    }

    public void DoorCollide(SlidingDoor door1, SlidingDoor door2)
    {
        if (!doorStates.ContainsKey(door1) || !doorStates.ContainsKey(door2))
            return;

        if (doorStates[door1] == SlidingDoorState.MovingClose)
        {
            if (doorStates[door2] == SlidingDoorState.MovingOpen)
            {
                door2.MoveClose();
                doorStates[door2] = SlidingDoorState.MovingClose;
                EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, gearType);
            }
        }

        else if (doorStates[door2] == SlidingDoorState.MovingClose)
        {
            if (doorStates[door1] == SlidingDoorState.MovingOpen)
            {
                door1.MoveClose();
                doorStates[door1] = SlidingDoorState.MovingClose;
                EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, gearType);
            }
        }
    }

    public void SetDoorOpen(SlidingDoor slidingDoor)
    {
        doorStates[slidingDoor] = SlidingDoorState.MovingOpen;
    }

    public void SetDoorClose(SlidingDoor slidingDoor)
    {
        doorStates[slidingDoor] = SlidingDoorState.MovingClose;
    }
}
