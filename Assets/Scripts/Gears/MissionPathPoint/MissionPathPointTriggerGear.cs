using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class MissionPathPointTriggerGear : MonoBehaviour
{
    protected static Dictionary<int, MissionPathPointTriggerGear> missionPathPointTriggerGearGroup = new Dictionary<int,MissionPathPointTriggerGear>();

    public int id;
    protected bool hasTrigger;
    protected bool hasIn;

    void Start()
    {
        hasTrigger = false;
        hasIn = false;
        missionPathPointTriggerGearGroup.Add(id, this);
    }

    void OnDestroy()
    {
        missionPathPointTriggerGearGroup.Remove(id);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (!hasIn)
                hasIn = true;

            if (hasTrigger)
                return;

            hasTrigger = true;

            // 事件
            EventDispatcher.TriggerEvent(Events.GearEvent.PathPointTrigger, id);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (!hasIn)
                hasIn = true;

            if (hasTrigger)
                return;

            hasTrigger = true;

            // 事件
            EventDispatcher.TriggerEvent(Events.GearEvent.PathPointTrigger, id);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            hasIn = false;
        }
    }

    public static void ClearMissionPathPointTriggerGear()
    {
        foreach (var missionPathPointTriggerGear in missionPathPointTriggerGearGroup)
        {
            Destroy(missionPathPointTriggerGear.Value);
        }
        missionPathPointTriggerGearGroup.Clear();
    }

    public static bool CheckIfAvatarInMissionTriggerGear(int id)
    {
        if (!missionPathPointTriggerGearGroup.ContainsKey(id))
            return false;
        return missionPathPointTriggerGearGroup[id].hasIn;
    }
}
