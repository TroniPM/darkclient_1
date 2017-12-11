using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class FoggyAbyssDoor : GearParent
{
    public static Vector3 FoggyAbyssDoorPosition = new Vector3(22.3f, -1, 51);

    protected SfxHandler handler;
    bool isOpen = false;
    protected bool hasTrigger { get; set; }

    void Start()
    {
        gearType = "FoggyAbyssDoor";

        ID = (uint)defaultID;

        handler = gameObject.AddComponent<SfxHandler>();
        isOpen = false;

        AddListeners();

        EventDispatcher.TriggerEvent(Events.InstanceUIEvent.GetFoggyAbyssMessage);
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    public override void AddListeners()
    {
        base.AddListeners();
        EventDispatcher.AddEventListener(Events.FoggyAbyssEvent.FoggyAbyssOpen, FoggyAbyssOpen);
        EventDispatcher.AddEventListener(Events.FoggyAbyssEvent.FoggyAbyssClose, FoggyAbyssClose);
    }

    public override void RemoveListeners()
    {
        base.RemoveListeners();
        EventDispatcher.RemoveEventListener(Events.FoggyAbyssEvent.FoggyAbyssOpen, FoggyAbyssOpen);
        EventDispatcher.RemoveEventListener(Events.FoggyAbyssEvent.FoggyAbyssOpen, FoggyAbyssClose);
    }

    protected void FoggyAbyssOpen()
    {
        handler.HandleFx(6035);
        isOpen = true;
    }

    protected void FoggyAbyssClose()
    {
        handler.RemoveFXs(6035);
        isOpen = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable && stateOne && !hasTrigger && isOpen)
            {
                hasTrigger = true;
                ControlStick.instance.Reset();
                TimerHeap.AddTimer(20, 0, () =>
                {
                    EventDispatcher.TriggerEvent(InstanceUIEvent.OnNormalMainUIEnterFoggyAbyssUp);
                });
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable && stateOne && !hasTrigger && isOpen)
            {
                hasTrigger = true;
                ControlStick.instance.Reset();
                TimerHeap.AddTimer(20, 0, () =>
                {
                    EventDispatcher.TriggerEvent(InstanceUIEvent.OnNormalMainUIEnterFoggyAbyssUp);
                });
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            hasTrigger = false;
        }
    }
}
