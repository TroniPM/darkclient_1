using UnityEngine;
using System.Collections;
using Mogo.Util;

public class Speeder : GearParent
{
    public int recoverTime;
    public float speedRate;

    void Start()
    {
        gearType = "Meteorite";

        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }


    #region 机关触发

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        base.SetGearEventStateOne(stateOneID);
        if (stateOneID == ID)
            EventDispatcher.TriggerEvent<int, float>(Events.OtherEvent.ChangeDummyRate, recoverTime, speedRate);
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
            EventDispatcher.TriggerEvent(Events.OtherEvent.ResetDummyRate);
    }

    #endregion
}
