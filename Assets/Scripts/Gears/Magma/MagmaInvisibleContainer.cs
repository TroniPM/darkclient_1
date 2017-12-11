using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class MagmaInvisibleContainer : Container
{
    public int[] skillActionList;
    public MagmaGear magmaGear;

    void Start()
    {
        gearType = "MagmaInvisibleContainer";
        ID = (uint)defaultID;

        defaultMyselfAttackable = false;
        defaultLittelGuyAttackable = false;
        defaultMercenarayAttackable = true;
        defaultDummyAttackable = true;

        if (!useSpecialData)
        {
            isMyselfAttackable = defaultMyselfAttackable;
            isLittelGuyAttackable = defaultLittelGuyAttackable;
            isMercenarayAttackable = defaultMercenarayAttackable;
            isDummyAttackable = defaultDummyAttackable;
        }

        hitRange = 2f;

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
        foreach(var tempID in skillActionList)
        {
            if (skillID == tempID)
            {
                if (magmaGear != null)
                    magmaGear.SetFire();
                base.OnDeath(skillID);
                break;
            }
        }
    }
}
