using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;

public class GearControlTrigger : GearParent
{
    public int clientEventID;
    public bool isTriggerRepeat = false;
    public bool hasTrigger { get; protected set; }

    void Start()
    {
        gearType = "GearEventTrigger";
        ID = (uint)defaultID;

        ID = (uint)defaultID;

        hasTrigger = false;

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
                if (!hasTrigger)
                {
                    SetGearControl();
                }
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
                    if (!hasTrigger)
                    {
                        SetGearControl();
                    }
                }
            }
        }
    }

    #endregion


    private void SetGearControl()
    {
        if (ClientEventData.dataMap.ContainsKey(clientEventID))
        {
            stateOne = false;

            if (ClientEventData.dataMap[clientEventID].enable != null)
            {
                foreach (var enableStatus in ClientEventData.dataMap[clientEventID].enable)
                {
                    TimerHeap.AddTimer((uint)enableStatus.Value, 0, SetGearControlEnable, (uint)enableStatus.Key);
                }
            }

            if (ClientEventData.dataMap[clientEventID].disable != null)
            {
                foreach (var enableStatus in ClientEventData.dataMap[clientEventID].disable)
                {
                    TimerHeap.AddTimer((uint)enableStatus.Value, 0, SetGearControlDisable, (uint)enableStatus.Key);
                }
            }

            if (ClientEventData.dataMap[clientEventID].stateOne != null)
            {
                foreach (var enableStatus in ClientEventData.dataMap[clientEventID].stateOne)
                {
                    TimerHeap.AddTimer((uint)enableStatus.Value, 0, SetGearControlStateOne, (uint)enableStatus.Key);
                }
            }

            if (ClientEventData.dataMap[clientEventID].stateTwo != null)
            {
                foreach (var enableStatus in ClientEventData.dataMap[clientEventID].stateTwo)
                {
                    TimerHeap.AddTimer((uint)enableStatus.Value, 0, SetGearControlStateTwo, (uint)enableStatus.Key);
                }
            }

            if (!isTriggerRepeat)
                hasTrigger = true;
        }
    }

    protected void SetGearControlEnable(uint gearID)
    {
        EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventEnable, gearID);
    }

    protected void SetGearControlDisable(uint gearID)
    {
        EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventDisable, gearID);
    }

    protected void SetGearControlStateOne(uint gearID)
    {
        EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventStateOne, gearID);
    }

    protected void SetGearControlStateTwo(uint gearID)
    {
        EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventStateTwo, gearID);
    }

    #region 拓展部分，用于多控制，已经注释

    //public int enableDelay; 
    //public GearParent[] enableGears;

    //public int disableDelay;
    //public GearParent[] disableGears;

    //public int stateOneDelay;
    //public GearParent[] stateOneGears;

    //public int stateTwoDelay;
    //public GearParent[] stateTwoGears;


    //protected void SetGearControlEnable()
    //{
    //    foreach (GearParent gear in enableGears)
    //    {
    //        EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventEnable, gear.ID);
    //    }
    //}

    //protected void SetGearControlDisable()
    //{
    //    foreach (GearParent gear in disableGears)
    //    {
    //        EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventDisable, gear.ID);
    //    }
    //}

    //protected void SetGearControlStateOne()
    //{
    //    foreach (GearParent gear in stateOneGears)
    //    {
    //        EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventStateOne, gear.ID);
    //    }
    //}

    //protected void SetGearControlStateTwo()
    //{
    //    foreach (GearParent gear in stateTwoGears)
    //    {
    //        EventDispatcher.TriggerEvent<uint>(Events.GearEvent.SetGearEventStateTwo, gear.ID);
    //    }
    //}

    #endregion
}
