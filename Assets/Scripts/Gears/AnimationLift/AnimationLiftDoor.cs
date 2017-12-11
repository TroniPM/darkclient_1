using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationLiftDoor: GearParent
{
    public Vector3 beginGo;
    public Vector3 moving;
    public Vector3 beginBack;

    protected Transform theTransform;

    void Start()
    {
        gearType = "MobilePlatformDoor";
        theTransform = transform;
    }

    public void ChangeBeginGo()
    {
        theTransform.localPosition = beginGo;
    }

    public void ChangeBeginBack()
    {
        theTransform.localPosition = beginBack;
    }

    public void ChangeStateMoving()
    {
        theTransform.localPosition = moving;
    }
}
