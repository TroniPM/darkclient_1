using UnityEngine;
using System.Collections;

using Mogo.Util;

public class CGTrigger : GearParent
{
    public int cgID;
    public bool isTriggerRepeat;

    void Start()
    {
        gearType = "CGTrigger";
        triggleEnable = true;
        stateOne = true;
        ID = (uint)defaultID;

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

                stateOne = false;
                StoryManager.Instance.PlayStory(cgID);
                if (!isTriggerRepeat)
                    triggleEnable = false;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (stateOne)
                {
                    stateOne = false;
                    StoryManager.Instance.PlayStory(cgID);
                }
            }
        }
    }

    #endregion


    #region 机关事件

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            StoryManager.Instance.PlayStory(cgID);
            base.SetGearEventStateTwo(stateTwoID);
        }
    }

    #endregion
}

