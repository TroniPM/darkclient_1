using UnityEngine;
using System.Collections;

using Mogo.Util;

public class AnimationReverseTrigger : GearParent
{
    public AnimationClip clip;
    public bool isTriggerRepeat;

    void Start()
    {
        gearType = "AnimationTrigger";
        //triggleEnable = true;
        //stateOne = true;

        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }


    #region ��ײ����

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                GetComponent<Animation>()[clip.name].speed = 1;
                GetComponent<Animation>().CrossFade(clip.name);

                if (!isTriggerRepeat)
                    triggleEnable = false;
            }
        }
    }

    #endregion


    #region ���ش���

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        if (stateOneID == ID)
        {
            GetComponent<Animation>()[clip.name].normalizedTime = 1f;
            GetComponent<Animation>()[clip.name].speed = -1;
            GetComponent<Animation>().Play();
            base.SetGearEventStateOne(stateOneID);
        }
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            GetComponent<Animation>()[clip.name].normalizedTime = 0f;
            GetComponent<Animation>()[clip.name].speed = 1;
            GetComponent<Animation>().Play();
            base.SetGearEventStateTwo(stateTwoID);
        }
    }

    #endregion


    #region ��������

    protected override void SetGearStateOne(uint stateOneID)
    {
        if (stateOneID == ID)
        {
            GetComponent<Animation>()[clip.name].normalizedTime = 1f;
            GetComponent<Animation>()[clip.name].speed = -1;
            GetComponent<Animation>().Play();
            base.SetGearStateOne(stateOneID);
        }
    }

    protected override void SetGearStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            GetComponent<Animation>()[clip.name].normalizedTime = 0f;
            GetComponent<Animation>()[clip.name].speed = 1;
            GetComponent<Animation>().Play();
            base.SetGearStateTwo(stateTwoID);
        }
    }

    #endregion
}
