using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;

public class GolemFx : GearParent
{
    public int standFx;
    public int hitFx;
    public int deadFx;

    protected SfxHandler handler { get; set; }

    void Start()
    {
        gearType = "GolemFx";

        ID = (uint)defaultID;

        handler = gameObject.AddComponent<SfxHandler>();

        //EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearEnable, SetGearEnable);
        //EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearDisable, SetGearDisable);
        //EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearStateOne, SetGearStateOne);
        //EventDispatcher.AddEventListener<uint>(Events.GearEvent.SetGearStateTwo, SetGearStateTwo);
    }

    void OnDestroy()
    {
        //EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearEnable, SetGearEnable);
        //EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearDisable, SetGearDisable);
        //EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearStateOne, SetGearStateOne);
        //EventDispatcher.RemoveEventListener<uint>(Events.GearEvent.SetGearStateTwo, SetGearStateTwo);
    }

    //protected override void SetGearEnable(uint enableID)
    //{
    //    base.SetGearEnable(enableID);
    //    if (enableID == ID)
    //        SetShowState(ID);
    //}

    //protected override void SetGearDisable(uint disableID)
    //{
    //    base.SetGearDisable(disableID);
    //    if (disableID == ID)
    //        SetShowState(ID);
    //}

    //protected override void SetGearStateOne(uint stateOneID)
    //{
    //    base.SetGearStateOne(stateOneID);
    //    if (stateOneID == ID)
    //        SetShowState(ID);
    //}

    //protected override void SetGearStateTwo(uint stateTwoID)
    //{
    //    base.SetGearStateTwo(stateTwoID);
    //    if (stateTwoID == ID)
    //        SetShowState(ID);
    //}

    public void Activate()
    {
        ID = (uint)defaultID;
        base.SetGearEventEnable(ID);
        base.SetGearEventStateOne(ID);

        Standing();
    }

    //public void SetShowState(uint theID)
    //{
    //    if (triggleEnable && !stateOne)
    //        Dead();
    //    else
    //        Stood();
    //}

    public void Standing()
    {
        if (FXData.dataMap.ContainsKey(standFx))
        {
            handler.HandleFx(standFx, null, (go, guid) =>
            {
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = new Quaternion(0, 0, 0, 1);
                go.transform.localScale = new Vector3(1, 1, 1);
            });
        }
    }

    public void Stood()
    {
        if (FXData.dataMap.ContainsKey(standFx))
        {
            handler.HandleFx(standFx, null, (go, guid) =>
            {
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = new Quaternion(0, 0, 0, 1);
                go.transform.localScale = new Vector3(1, 1, 1);
            });
        }
    }


    public void Hitting()
    {
        if (FXData.dataMap.ContainsKey(hitFx))
        {
            handler.HandleFx(hitFx, null, (go, guid) =>
            {
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = new Quaternion(0, 0, 0, 1);
                go.transform.localScale = new Vector3(1, 1, 1);
            });
        }
    }

    //public void Hit()
    //{
    //    if (FXData.dataMap.ContainsKey(hitFx) && !hasHandleHit)
    //    {
    //        handler.HandleFx(hitFx);
    //        hasHandleHit = true;

    //        hitTimer = TimerHeap.AddTimer((uint)hitFxRemoveTime, 0, () =>
    //        {
    //            handler.RemoveFXs(hitFx);
    //            hasHandleHit = false;
    //        });
    //    }
    //}


    public void Dying()
    {
        if (FXData.dataMap.ContainsKey(standFx))
        {
            handler.RemoveFXs(standFx);
        }

        if (FXData.dataMap.ContainsKey(hitFx))
        {
            handler.RemoveFXs(hitFx);
        }

        if (FXData.dataMap.ContainsKey(deadFx))
        {
            handler.HandleFx(deadFx, null, (go, guid) =>
            {
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = new Quaternion(0, 0, 0, 1);
                go.transform.localScale = new Vector3(1, 1, 1);
            });
        }
    }

    //public void Dead()
    //{
    //    if (FXData.dataMap.ContainsKey(deadFx) && !hasHandleDead)
    //    {
    //        handler.HandleFx(deadFx);
    //        hasHandleDead = true;

    //        deadTimer = TimerHeap.AddTimer((uint)hitFxRemoveTime, 0, () =>
    //        {
    //            handler.RemoveFXs(deadFx);
    //            hasHandleDead = false;
    //        });
    //    }
    //}
}
