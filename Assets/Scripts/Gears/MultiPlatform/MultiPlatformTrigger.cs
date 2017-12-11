using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MultiPlatformTrigger : GearParent
{
    public AnimationClip chip;
    protected bool isFirst;

    void Start()
    {
        gearType = "MultiPlatformTrigger";
        isFirst = true;
        triggleEnable = true;
        stateOne = true;

        AddListeners();
    }



    void OnDestroy()
    {
        RemoveListeners();
    }

    protected override void SetGearStateTwo(uint stateTwoID)
    {
        // to do
        base.SetGearStateTwo(stateTwoID);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (isFirst)
                {
                    gameObject.GetComponent<Animation>().CrossFade(chip.name);
                    isFirst = !isFirst;
                }
            }
        }
    }
}
