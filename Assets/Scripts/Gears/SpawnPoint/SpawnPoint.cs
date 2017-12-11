using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using System.Text;

public class SpawnPoint : GearParent
{
    public SpawnPointTriggeGear triggeGear;

    public Transform[] monsterSpawnPoint;

    public List<Transform> realMonsterSpawnPoint
    {
        get
        {
            List<Transform> temp = new List<Transform>();
            foreach (Transform point in monsterSpawnPoint)
            {
                foreach (Transform child in point)
                {
                    temp.Add(child);
                }
            }
            return temp;
        }
    }

    public int triggerType = 0;

    public List<int> preSpawnPointId;
    public List<int> levelId;

    public float triggeRangex { get; protected set; }
    public float triggeRangey { get; protected set; }

    public float triggeRangeLength { get; protected set; }
    public float triggeRangeWidth { get; protected set; }

    public float homerangex { get; protected set; }
    public float homerangey { get; protected set; }

    public float homerangeLength { get; protected set; }
    public float homerangeWidth { get; protected set; }

    void Start() 
    {
        gearType = "SpawnPoint";
        ID = (uint)defaultID;

        if (triggeGear != null)
        {
            triggeRangex = triggeGear.transform.position.x;
            triggeRangey = triggeGear.transform.position.z;
            triggeRangeLength = triggeGear.transform.lossyScale.x;
            triggeRangeWidth = triggeGear.transform.lossyScale.z;
        }
        else
        {
            triggeRangex = 0;
            triggeRangey = 0;
            triggeRangeLength = 0;
            triggeRangeWidth = 0;
        }

        homerangex = transform.position.x;
        homerangey = transform.position.z;
        homerangeLength = transform.lossyScale.x;
        homerangeWidth  = transform.lossyScale.z;
	}
}
