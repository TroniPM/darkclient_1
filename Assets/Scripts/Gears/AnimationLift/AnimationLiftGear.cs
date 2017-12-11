using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationLiftGear : GearParent
{
    public AnimationLift platform;

    private List<Transform> avatars;

    void Start()
    {
        gearType = "AnimationLift";
        avatars = new List<Transform>();
        triggleEnable = true;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                avatars.Add(other.gameObject.transform);
                platform.OnPassengerEnter(other);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                avatars.Remove(other.gameObject.transform);

                if (avatars.Count == 0)
                    platform.OnPassengerExit(other);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                platform.OnPassengerStay(other);
            }
        }
    }

    public void OnPlatformMove(Vector3 distance)
    {
        foreach (Transform avatar in avatars)
        {
            avatar.GetComponent<ActorMyself>().SetMoveToDirectly(distance);
        }
    }
}
