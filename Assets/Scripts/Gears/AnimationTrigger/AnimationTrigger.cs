using UnityEngine;
using System.Collections;

using Mogo.Util;

public class AnimationTrigger : GearParent
{
    public AnimationClip[] clip;
    public bool isTriggerRepeat;

    protected int i;

    void Start()
    {
        gearType = "AnimationTrigger";
        ID = (uint)defaultID;
        i = 0;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }


    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                GetComponent<Animation>().CrossFade(clip[i].name);

                i++;
                if (i == clip.Length)
                    i = 0;

                if (isTriggerRepeat)
                    triggleEnable = false;
            }
        }
    }

    #endregion


    #region 机关事件

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        GetComponent<Animation>().CrossFade(clip[1].name);
        base.SetGearEventStateOne(stateOneID);
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        GetComponent<Animation>().CrossFade(clip[0].name);
        base.SetGearEventStateTwo(stateTwoID);
    }

    #endregion


    #region 断线重连

    protected override void SetGearStateOne(uint stateOneID)
    {
        GetComponent<Animation>().CrossFade(clip[1].name);
    }

    protected override void SetGearStateTwo(uint stateOneID)
    {
        GetComponent<Animation>().CrossFade(clip[0].name);
    }

    #endregion

}

