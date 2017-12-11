using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class Container : GearParent
{
    public static Dictionary<Transform, Container> containers = new Dictionary<Transform, Container>();

    public static Dictionary<Transform, float> containerRange = new Dictionary<Transform, float>();

    public bool useSpecialData = false;

    public bool isMyselfAttackable;
    public bool isLittelGuyAttackable;
    public bool isMercenarayAttackable;
    public bool isDummyAttackable;

    protected bool defaultMyselfAttackable;
    protected bool defaultLittelGuyAttackable;
    protected bool defaultMercenarayAttackable;
    protected bool defaultDummyAttackable;

    protected float hitRange;

    protected GolemAnimation golemAni;
    protected GolemFx golemFx;

    public static void ClearAllContainers()
    {
        containers.Clear();
        containerRange.Clear();
    }

    void Start()
    {
        gearType = "Container";
        ID = (uint)defaultID;

        containers.Add(transform, this);
        containerRange.Add(transform ,hitRange);

        GetGolemGear();
        OnActivate();
    }

    void OnDestroy()
    {
        containers.Remove(transform);
        containerRange.Remove(transform);
    }

    protected void GetGolemGear()
    {
        golemAni = gameObject.GetComponent<GolemAnimation>();
        golemFx = gameObject.GetComponent<GolemFx>();
    }

    public virtual void OnActivate()
    {
        triggleEnable = true;

        if (golemAni != null)
            golemAni.Activate();

        if (golemFx != null)
            golemFx.Activate();
    }

    public virtual void OnDeath(int skillID)
    {
        if (golemAni != null)
            golemAni.Dying();

        if (golemFx != null)
            golemFx.Dying();

        containers.Remove(transform);
        containerRange.Remove(transform);
    }

    protected virtual void RpcGetDrop()
    {
    }

    public virtual void SetMustBeHit()
    {
        stateOne = false;
    }

    public virtual void SetNotMustBeHit()
    {
        stateOne = true;
    }
}
