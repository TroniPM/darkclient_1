using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ShakeScreenGear : GearParent
{
    public int shakeID;
    public float shakeTime;

    void Start()
    {
        gearType = "ShakeScreenGear";
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
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            CheckShake();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            CheckShake();
        }
    }

    #endregion


    #region 机关事件

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
            Shake();
    }

    #endregion


    protected void CheckShake()
    {
        if (triggleEnable)
        {
            if (stateOne)
            {
                stateOne = false;
                Shake();
            }
        }
    }

    protected void Shake()
    {
        if (MogoMainCamera.Instance != null)
            MogoMainCamera.Instance.Shake(shakeID, shakeTime); ;
    }
}
