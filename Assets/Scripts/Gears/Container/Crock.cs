using UnityEngine;
using System.Collections;

using Mogo.Util;

public class Crock : Container
{
    protected uint deleteTimer;

    void Start()
    {
        gearType = "Crock";
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

        hitRange = 0;

        containers.Add(transform, this);
        containerRange.Add(transform, hitRange);

        deleteTimer = uint.MaxValue;
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(deleteTimer);
        containers.Remove(transform);
        containerRange.Remove(transform);
    }

    public override void OnDeath(int skillID)
    {
        GetComponent<Animation>().Play();
        containers.Remove(transform);
        containerRange.Remove(transform);
        RpcGetDrop();

        deleteTimer = TimerHeap.AddTimer(1000, 0, DeleteCrock);
    }

    protected override void RpcGetDrop()
    {
        EventDispatcher.TriggerEvent(Events.GearEvent.CrockBroken, transform.position);
    }

    protected void DeleteCrock()
    {
        if (this && this.gameObject)
            Destroy(this.gameObject);
    }

}

