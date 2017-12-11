using UnityEngine;
using System.Collections;

using Mogo.Util;

public class HealthFountainGear : GearParent
{
    public enum HealthType
    {
        Fixed,
        Percentage
    }

    public HealthType healthType = HealthType.Fixed;
    public float healthArg;

    protected SfxHandler handler;

    void Start()
    {
        gearType = "HealthFountainGear";
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
                if (stateOne)
                {
                    stateOne = false;
                    switch (healthType)
                    {
                        case HealthType.Fixed:
                            // to do 
                            break;

                        case HealthType.Percentage:
                            // to do 
                            break;

                        default:
                            break;
                    }
                }
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
                    switch (healthType)
                    {
                        case HealthType.Fixed:
                            // to do 
                            break;

                        case HealthType.Percentage:
                            // to do 
                            break;

                        default:
                            break;
                    }
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
            switch (healthType)
            {
                case HealthType.Fixed:
                    // to do 
                    break;

                case HealthType.Percentage:
                    // to do 
                    break;

                default:
                    break;
            }
            base.SetGearEventStateTwo(stateTwoID);
        }
    }

    #endregion
}
