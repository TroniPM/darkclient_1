using UnityEngine;
using System.Collections;
using Mogo.Util;

public class FadeGear : GearParent
{
    public float fadeTime;

    void Start()
    {
        gearType = "FadeGear";
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
            FadeIn();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            FadeIn();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            FadeOut();
        }
    }

    #endregion


    #region 机关事件

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        base.SetGearEventStateOne(stateOneID);
        if (stateOneID == ID)
            FadeOut();
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
            FadeIn();
    }

    #endregion


    #region 断线重连

    protected override void SetGearStateOne(uint stateOneID)
    {
        base.SetGearStateOne(stateOneID);
        if (stateOneID == ID)
            FadeOut();
    }

    protected override void SetGearStateTwo(uint stateTwoID)
    {
        base.SetGearStateTwo(stateTwoID);
        if (stateTwoID == ID)
            FadeIn();
    }

    #endregion


    public void FadeIn()
    {
        if (triggleEnable)
        {
            if (stateOne)
            {
                stateOne = false;
                MogoFXManager.Instance.AlphaFadeIn(gameObject, (uint)fadeTime);
            }
        }
    }

    public void FadeOut()
    {
        if (triggleEnable)
        {
            if (!stateOne)
            {
                stateOne = true;
                MogoFXManager.Instance.AlphaFadeOut(gameObject, (uint)fadeTime);
            }
        }
    }
}

