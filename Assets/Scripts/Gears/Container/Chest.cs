using UnityEngine;
using System.Collections;

using Mogo.Util;

public class Chest : Container
{
    public bool isEasyLuxuriant;
    public bool isHardLuxuriant;

    void Start()
    {
        gearType = "Chest";
        triggleEnable = true;
        stateOne = true;
        ID = (uint)defaultID;

        defaultMyselfAttackable = true;
        defaultLittelGuyAttackable = false;
        defaultMercenarayAttackable = false;
        defaultDummyAttackable = false;

        if (!useSpecialData)
        {
            isMyselfAttackable = defaultMyselfAttackable;
            isLittelGuyAttackable = defaultLittelGuyAttackable;
            isMercenarayAttackable = defaultMercenarayAttackable;
            isDummyAttackable = defaultDummyAttackable;
        }

        hitRange = 0.8f;

        containers.Add(transform, this);
        containerRange.Add(transform, hitRange);

        GetGolemGear();
        OnActivate();
    }

    void OnDestroy()
    {
        containers.Remove(transform);
        containerRange.Remove(transform);
    }

    public override void OnDeath(int skillID)
    {
        base.OnDeath(skillID);
        RpcGetDrop();
    }

    protected override void RpcGetDrop()
    {
        EventDispatcher.TriggerEvent(Events.GearEvent.ChestBroken, isEasyLuxuriant, isHardLuxuriant, transform.position);
    }
}
