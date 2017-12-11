using UnityEngine;
using System.Collections;
using Mogo.Util;

public class GolemAnimationEx : GolemAnimation
{
    public int[] bloodCondition;
    public GolemAnimation[] golemAnimations;

    private int index{get; set;}

    void Start()
    {
        gearType = "GolemAnimation";

        ID = (uint)defaultID;

        index = 0;
        for (int i = 1; i < golemAnimations.Length; i++)
        {
            golemAnimations[i].gameObject.SetActive(false);
        }

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    public override void AddListeners()
    {
        base.AddListeners();
        EventDispatcher.AddEventListener<GolemAnimation, int>(Events.MonsterEvent.TowerDamage, ChangeGolem);
    }

    public override void RemoveListeners()
    {
        base.RemoveListeners();
        EventDispatcher.RemoveEventListener<GolemAnimation, int>(Events.MonsterEvent.TowerDamage, ChangeGolem);
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

    protected void ChangeGolem(GolemAnimation theGolem, int currenState)
    {
        if (theGolem != this)
            return;

        if (bloodCondition == null)
            return;

        int temp = 0;
        for (int i = 0; i <  bloodCondition.Length; i++)
        {
            if (currenState > bloodCondition[i])
            {
                temp = i - 1 < 0 ? 0 : i - 1;
                break;
            }
        }

        if (temp != index)
        {
            index = temp;
            for (int i = 0; i < golemAnimations.Length; i++)
            {
                if (index != i)
                    golemAnimations[i].gameObject.SetActive(false);
                else
                {
                    golemAnimations[i].gameObject.SetActive(true);
                    Activate();
                }
            }
        }
    }

    public override void Activate()
    {
        if (golemAnimations.Length > index)
        {
            golemAnimations[index].Activate();
        }
    }

    public override void Starding()
    {
        if (golemAnimations.Length > index)
        {
            golemAnimations[index].Starding();
        }
    }

    public override void Stood()
    {
        if (golemAnimations.Length > index)
        {
            golemAnimations[index].Stood();
        }
    }

    public override void Hitting()
    {
        if (golemAnimations.Length > index)
        {
            golemAnimations[index].Hitting();
        }
    }

    public override void Hit()
    {
        if (golemAnimations.Length > index)
        {
            golemAnimations[index].Hit();
        }
    }

    public override void Dying()
    {
        if (golemAnimations.Length > index)
        {
            golemAnimations[index].Dying();
        }
    }

    public override void Dead()
    {
        if (golemAnimations.Length > index)
        {
            golemAnimations[index].Dead();
        }
    }
}
