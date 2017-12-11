using UnityEngine;
using System.Collections;

using Mogo.Util;

public class LightMapSwitcher : GearParent
{
    public int lightMapID;

    void Start()
    {
        gearType = "CGTrigger";
        // triggleEnable = true;
        stateOne = true;
        ID = (uint)defaultID;

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
                stateOne = false;
                EventDispatcher.TriggerEvent(Events.GearEvent.SwitchLightMapFog, lightMapID);
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
                    stateOne = false;
                    EventDispatcher.TriggerEvent(Events.GearEvent.SwitchLightMapFog, lightMapID);
                }
            }
        }
    }

    #endregion


    #region 机关事件

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            EventDispatcher.TriggerEvent(Events.GearEvent.SwitchLightMapFog, lightMapID);
            base.SetGearEventStateTwo(stateTwoID);
        }
    }

    #endregion
}

