using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class Carrousel : GearParent
{
    public float rotateSpeedYBase;
    public float rotateSpeedYMin;
    public float rotateSpeedYMax;

    public int spinTime;

    protected MogoSimpleMotor motor;
    protected uint timer;

    void Start()
    {
        gearType = "Carrousel";
        ID = (uint)defaultID;

        motor = GetComponent<MogoSimpleMotor>();
        if (motor == null)
            motor = gameObject.AddComponent<MogoSimpleMotor>();

        AddListeners();

        CheckSpin();
    }

    void OnDestroy()
    {
        Stop();
        RemoveListeners();
    }

    protected void CheckSpin()
    {
        if (triggleEnable)
            SetSpin();
        else
            Stop();
    }

    protected void SetSpin()
    {
        int type = RandomHelper.GetRandomInt(-1, 2);

        float curSpeedY = 0;

        switch (type)
        {
            case -1:
                curSpeedY = rotateSpeedYBase * -1 + RandomHelper.GetRandomFloat(rotateSpeedYMin, rotateSpeedYMax);
                break;

            case 1:
                curSpeedY = rotateSpeedYBase + RandomHelper.GetRandomFloat(rotateSpeedYMin, rotateSpeedYMax);
                break;
        }

        motor.SetRotateSpeed(0, curSpeedY, 0);

        uint nextSpinTime = (uint)RandomHelper.GetRandomInt((int)(spinTime * 0.8), (int)(spinTime * 1.2));

        timer = TimerHeap.AddTimer(nextSpinTime, 0, SetSpin);
    }

    protected void Stop()
    {
        TimerHeap.DelTimer(timer);
        motor.StopRotate();
    }

    protected override void SetGearEnable(uint enableID)
    {
        base.SetGearEnable(enableID);
        if (enableID == ID)
            CheckSpin();
    }

    protected override void SetGearDisable(uint disableID)
    {
        base.SetGearDisable(disableID);
        if (disableID == ID)
            CheckSpin();
    }
}
