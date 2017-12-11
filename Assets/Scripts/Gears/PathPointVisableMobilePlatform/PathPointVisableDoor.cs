using UnityEngine;
using System.Collections;
using Mogo.Util;

public class PathPointVisableDoor : GearParent
{
    public Transform beginGoTransform;
    public Transform beginBackTransform;

    protected Vector3 beginGo;
    protected Vector3 beginBack;

    void Start()
    {
        gearType = "MobilePlatformDoor";

        beginGo = beginGoTransform.position;
        beginBack = beginBackTransform.position;
    }

    public void ChangeBeginGo()
    {
        LoggerHelper.Debug("ChangeBeginGo");
        transform.position = beginGo;
    }

    public void ChangeBeginBack()
    {
        LoggerHelper.Debug("ChangeBeginBack");
        transform.position = beginBack;
    }

    public void ChangeMoving(Transform theTransform)
    {
        LoggerHelper.Debug("ChangeMoving");
        transform.position = theTransform.position;
    }
}
