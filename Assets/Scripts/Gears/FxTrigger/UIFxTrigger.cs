using UnityEngine;
using System.Collections;

using Mogo.Util;

public class UIFxTrigger : GearParent
{
    public int fxID;
    public bool isTriggerRepeat;

    void Start()
    {
        gearType = "UIFxTrigger";
        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
        MogoFXManager.Instance.DetachUIFX(fxID);
    }


    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                stateOne = false;

                if (!isTriggerRepeat)
                    triggleEnable = false;

                MogoFXManager.Instance.HandleUIFX(fxID);
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

                    if (!isTriggerRepeat)
                        triggleEnable = false;

                    MogoFXManager.Instance.HandleUIFX(fxID);
                }
            }
        }
    }

    #endregion


    #region 机关事件

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        if (stateOneID == ID)
        {
            MogoFXManager.Instance.DetachUIFX(fxID);
            base.SetGearEventStateOne(stateOneID);
        }
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            MogoFXManager.Instance.HandleUIFX(fxID);
            base.SetGearEventStateTwo(stateTwoID);
        }
    }

    #endregion


    #region 断线重连

    protected override void SetGearStateOne(uint stateOneID)
    {
        if (stateOneID == ID)
        {
            MogoFXManager.Instance.DetachUIFX(fxID);
            base.SetGearStateOne(stateOneID);
        }
    }

    protected override void SetGearStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            MogoFXManager.Instance.HandleUIFX(fxID);
            base.SetGearStateTwo(stateTwoID);
        }
    }

    #endregion
}
