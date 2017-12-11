using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.Game;
using System;

public class SpawnPointTriggeGear : GearParent
{
    public int[] preSpawnPointID;
    public int tangetID;

    protected Dictionary<int, bool> idMap { get; set; }

    void Start()
    {
        gearType = "SpawnPointTriggeGear";
        ID = (uint)defaultID;

        //triggleEnable = true;
        //stateOne = true;

        idMap = new Dictionary<int, bool>();

        if (preSpawnPointID != null)
            foreach(int preID in preSpawnPointID)
                idMap.Add(preID, false);

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    public override void AddListeners()
    {
        base.AddListeners();
        EventDispatcher.AddEventListener<int>(Events.GearEvent.SpawnPointDead, SpawnPointDead);
    }

    public override void RemoveListeners()
    {
        base.RemoveListeners();
        EventDispatcher.RemoveEventListener<int>(Events.GearEvent.SpawnPointDead, SpawnPointDead);
    }


    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (stateOne)
                {
                    CheckSpawn();
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
                    CheckSpawn();
                }
            }
        }
    }

    #endregion


    #region 机关事件

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
        {
            CheckSpawn();
        }
    }

    #endregion


    public void SpawnPointDead(int preID)
    {
        if (idMap.ContainsKey(preID))
        {
            idMap[preID] = true;
        }
    }

    public void CheckSpawn()
    {
        foreach (var item in idMap)
            if (!item.Value)
                return;

        stateOne = false;
        EventDispatcher.TriggerEvent(Events.InstanceEvent.SpawnPointStart, tangetID);
    }
}