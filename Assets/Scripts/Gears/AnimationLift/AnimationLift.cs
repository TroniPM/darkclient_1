using UnityEngine;
using System.Collections;

using Mogo.Util;

public class AnimationLift : GearParent
{
    public AnimationLiftDoor[] doors;
    public AnimationLiftGear gear;

    public AnimationClip[] animationList;

    public int flushDefaultTime;

    protected uint timerID;

    protected bool pathDiretion;

    protected bool isBeginMoving;
    protected bool isMoving;

    protected bool hasOpen;

    protected Vector3 lastPosition;
    protected Vector3 lastDistance;

    private string animationListName0;
    private string animationListName1;

    private Vector3 distance;

    void Start()
    {
        lastPosition = transform.position;

        timerID = uint.MaxValue;

        pathDiretion = true;

        isBeginMoving = false;
        isMoving = false;

        hasOpen = false;

        animationListName0 = animationList[0].name;
        animationListName1 = animationList[1].name;
	}
	
	void Update ()
    {
        if (isBeginMoving)
        {
            // to do
            if (pathDiretion)
            {
                GetComponent<Animation>().CrossFade(animationListName0);
            }
            else 
            {
                GetComponent<Animation>().CrossFade(animationListName1);
            }

            pathDiretion = !pathDiretion;

            foreach (AnimationLiftDoor door in doors)
            {
                door.ChangeStateMoving();
            }

            SetStart(false);
            SetMoving(true);

            isMoving = true;
            hasOpen = false;
        }

        if (isMoving)
        {
            distance = transform.position - lastPosition;
            gear.OnPlatformMove(distance);
            lastPosition = transform.position;
            foreach (AnimationClip chip in animationList)
            {
                if (GetComponent<Animation>().IsPlaying(chip.name))
                    return;
            }
            SetMoving(false);
        }
        else
        {
            if (!hasOpen && pathDiretion)
            {
                LoggerHelper.Debug("ChangeBeginGo");
                hasOpen = true;
                foreach (AnimationLiftDoor door in doors)
                {
                    door.ChangeBeginGo();
                }
            }
            else if (!hasOpen)
            {
                LoggerHelper.Debug("ChangeBeginBack");
                hasOpen = true;
                foreach (AnimationLiftDoor door in doors)
                {
                    door.ChangeBeginBack();
                }
            }
        }
	}


    public void OnPassengerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (timerID != uint.MaxValue)
                TimerHeap.DelTimer(timerID);

            timerID = TimerHeap.AddTimer((uint)flushDefaultTime, 0, SetStart, true);
        }
    }


    public void OnPassengerStay(Collider other)
    {
    }


    public void OnPassengerExit(Collider other)
    {
        SetStart(false);
        SetMoving(false);
    }


    private void SetStart(bool flag)
    {
        isBeginMoving = flag;
    }


    private void SetMoving(bool flag)
    {
        isMoving = flag;
    }
}
