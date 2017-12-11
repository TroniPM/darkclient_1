using UnityEngine;
using System.Collections;
using Mogo.Util;

public class SlidingDoorGear : GearParent
{
    public SlidingDoor door;

    protected bool autoProcess { get; set; }

    void Start()
    {
        gearType = "SlidingDoorGear";
        ID = (uint)defaultID;

        autoProcess = true;

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
                door.MoveOpen();
                MoveDoorBegin();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            door.MoveClose();
            autoProcess = true;
        }
    }

    #endregion


    protected void MoveDoorBegin()
    {
        if (autoProcess)
        {
            autoProcess = false;
            EventDispatcher.TriggerEvent(Events.GearEvent.TrapBegin, gearType);
        }
    }
}
