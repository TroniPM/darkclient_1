using UnityEngine;
using System.Collections;
using Mogo.Util;

public class GolemAnimation : GearParent
{
    public AnimationClip standAnimation;
    public AnimationClip hitAnimation;
    public AnimationClip deadAnimation;

    void Start()
    {
        gearType = "GolemAnimation";

        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }


    #region 机关事件

    protected override void SetGearEventEnable(uint enableID)
    {
        base.SetGearEventEnable(enableID);
        if (enableID == ID)
            SetShowState(ID, 0);
    }

    protected override void SetGearEventDisable(uint disableID)
    {
        base.SetGearEventDisable(disableID);
        if (disableID == ID)
            SetShowState(ID, 1);
    }

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        base.SetGearEventStateOne(stateOneID);
        if (stateOneID == ID)
            SetShowState(ID, 2);
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
            SetShowState(ID, 3);
    }

    #endregion


    #region 断线重连

    protected override void SetGearEnable(uint enableID)
    {
        base.SetGearEnable(enableID);
        if (enableID == ID)
            SetShowState(ID, 0);
    }

    protected override void SetGearDisable(uint disableID)
    {
        base.SetGearDisable(disableID);
        if (disableID == ID)
            SetShowState(ID, 1);
    }

    protected override void SetGearStateOne(uint stateOneID)
    {
        base.SetGearStateOne(stateOneID);
        if (stateOneID == ID)
            SetShowState(ID, 2);
    }

    protected override void SetGearStateTwo(uint stateTwoID)
    {
        base.SetGearStateTwo(stateTwoID);
        if (stateTwoID == ID)
            SetShowState(ID, 3);
    }

    #endregion


    public virtual void Activate()
    {
        ID = (uint)defaultID;
        base.SetGearEventEnable(ID);
        base.SetGearEventStateOne(ID);

        Starding();
    }

    public virtual void SetShowState(uint theID, int from)
    {
        // Mogo.Util.LoggerHelper.Debug("SetShowState: " + triggleEnable + " " + stateOne + " " + theID + " from: " + from);

        if (triggleEnable && !stateOne)
            Dead();
        else
            Stood();
    }

    public virtual void Starding()
    {
        if (standAnimation != null)
            gameObject.GetComponent<Animation>().CrossFade(standAnimation.name);
    }

    public virtual void Stood()
    {
        if (standAnimation != null)
        {
            GetComponent<Animation>().playAutomatically = true;
            GetComponent<Animation>()[standAnimation.name].normalizedTime = 1f;
            gameObject.GetComponent<Animation>().CrossFade(standAnimation.name);
        }
    }


    public virtual void Hitting()
    {
        if (hitAnimation != null && triggleEnable)
            gameObject.GetComponent<Animation>().CrossFade(hitAnimation.name);
    }

    public virtual void Hit()
    {
        if (hitAnimation != null)
        {
            if (triggleEnable)
            {
                GetComponent<Animation>().playAutomatically = false;
                GetComponent<Animation>()[hitAnimation.name].normalizedTime = 1f;
                gameObject.GetComponent<Animation>().CrossFade(hitAnimation.name);
            }
        }
    }


    public virtual void Dying()
    {
        if (deadAnimation != null && triggleEnable)
        {
            // Debug.LogError("Dying");
            base.SetGearEventStateTwo(ID);
            gameObject.GetComponent<Animation>().CrossFade(deadAnimation.name);
        }
    }

    public virtual void Dead()
    {
        if (deadAnimation != null)
        {
            //if (triggleEnable)
            {
                GetComponent<Animation>()[deadAnimation.name].normalizedTime = 1f;
                gameObject.GetComponent<Animation>().CrossFade(deadAnimation.name);
            }
        }
    }
}
