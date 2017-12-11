using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class MagmaCongealer : GearParent
{
    void Start()
    {
        gearType = "MagmaCongealer";
        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    #region

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
            EventDispatcher.TriggerEvent(Events.GearEvent.CongealMagma);
    }

    #endregion

}

