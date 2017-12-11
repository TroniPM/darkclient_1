using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class StealthyGear : GearParent
{
    protected bool isDefaultVisiable;
    protected List<Transform> result;

    void Start()
    {
        gearType = "StealthyGear";
        ID = (uint)defaultID;
        result = new List<Transform>();

        GetTramformList();

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }


    #region 机关事件

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        base.SetGearEventStateOne(stateOneID);
        if (stateOneID == ID)
            Betray();
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
            Stealthy();
    }

    #endregion


    #region 断线重连

    protected override void SetGearStateOne(uint stateOneID)
    {
        base.SetGearStateOne(stateOneID);
        if (stateOneID == ID)
            Betray();
    }

    protected override void SetGearStateTwo(uint stateTwoID)
    {
        base.SetGearStateTwo(stateTwoID);
        if (stateTwoID == ID)
            Stealthy();
    }

    #endregion


    public void Betray()
    {
        stateOne = true;
        foreach (Transform child in result)
            if (child.gameObject.GetComponent<Renderer>() != null)
                child.gameObject.SetActive(true);
    }

    public void Stealthy()
    {
        stateOne = false;
        foreach (Transform child in result)
            if (child.gameObject.GetComponent<Renderer>() != null)
                child.gameObject.SetActive(false);
    }

    protected void GetTramformList()
    {
        Queue<Transform> temp = new Queue<Transform>();

        result.Add(transform);
        temp.Enqueue(transform);

        while (temp.Count > 0)
        {
            foreach (Transform child in temp.Dequeue())
            {
                result.Add(child);
                temp.Enqueue(child);
            }
        }
    }
}
