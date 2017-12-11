using UnityEngine;
using System.Collections;

using Mogo.Util;

public class GlobalAnimationPlayer : GearParent
{
    protected static uint DEFAULT_FRAME_TIME = 20;

    public Animation[] animaList;

    public float speedUpRate;
    public float speedDownRate;

    public int speedUpTime;
    public int speedDownTime;

    protected float globalSpeed { get; set; }
    protected float globalSpeedUpRate { get; set; }
    protected float globalSpeedDownRate { get; set; }

    protected bool isSpeedUp;
    protected bool isSpeedDown;
    protected bool isSpeedStop;

    protected uint timer;

    void Start()
    {
        gearType = "GlobalanimaPlayer";
        ID = (uint)defaultID;

        AddListeners();

        GetAllAnimation();
        CalculateSpeedRate();

        isSpeedUp = false;
        isSpeedDown = false;

        timer = uint.MaxValue;
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(timer);
        RemoveListeners();
    }


    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != GearParent.MogoPlayerTag)
            return;

        if (triggleEnable && animaList != null)
        {
            if (stateOne)
            {
                isSpeedUp = true;
                SpeedUp();
            }
            else
            {
                isSpeedDown = true;
                SpeedDown();
            }
        }
    }

    #endregion


    #region 机关触发

    protected override void SetGearEventEnable(uint enableID)
    {
        base.SetGearEventEnable(enableID);
        if (enableID == ID)
        {
            if (stateOne)
            {
                isSpeedUp = true;
                SpeedUp();
            }
            else
            {
                isSpeedDown = true;
                SpeedDown();
            }
        }
    }

    protected override void SetGearEventDisable(uint disableID)
    {
        base.SetGearEventDisable(disableID);
        if (disableID == ID)
        {
            isSpeedStop = true;
            Stop();
        }
    }

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        if (stateOneID == ID)
        {
            if (enabled)
            {
                base.SetGearEventStateOne(stateOneID);

                isSpeedUp = true;
                SpeedUp();
            }
        }
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            if (enabled)
            {
                base.SetGearEventStateTwo(stateTwoID);

                isSpeedDown = true;
                SpeedDown();
            }
        }
    }

    #endregion


    #region 断线重连

    protected override void SetGearDisable(uint disableID)
    {
        base.SetGearDisable(disableID);
        if (disableID == ID)
        {
            isSpeedStop = true;
            Stop();
        }
    }

    protected override void SetGearStateOne(uint stateOneID)
    {
        if (stateOneID == ID)
        {
            if (enabled)
            {
                base.SetGearStateOne(stateOneID);

                isSpeedUp = true;
                SpeedUp();
            }
        }
    }

    protected override void SetGearStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            if (enabled)
            {
                base.SetGearStateTwo(stateTwoID);

                isSpeedDown = true;
                SpeedDown();
            }
        }
    }

    #endregion


    protected void GetAllAnimation()
    {
        if (animaList == null)
            animaList = GameObject.FindObjectsOfType(typeof(Animation)) as Animation[];
        SetDefaultState();
    }

    protected void SetDefaultState()
    {
        if (animaList == null)
            return;

        if (triggleEnable)
        {
            if (stateOne)
            {
                isSpeedUp = true;
                SpeedUp();
            }
            else
            {
                isSpeedDown = true;
                SpeedDown();
            }
        }
        else
        {
            isSpeedStop = true;
            Stop();
        }
    }

    protected void CalculateSpeedRate()
    {
        globalSpeed = speedUpRate;
        globalSpeedUpRate = (float)DEFAULT_FRAME_TIME * speedUpRate / speedDownTime;
        globalSpeedDownRate = (float)DEFAULT_FRAME_TIME * speedDownRate / speedDownTime;
    }


    protected void SpeedUp()
    {
        if (isSpeedUp)
        {
            globalSpeed += globalSpeedUpRate;
            if (globalSpeed >= speedUpRate)
            {
                globalSpeed = speedUpRate;
                isSpeedUp = false;
            }

            ControlAllAnimation(globalSpeed);

            timer = TimerHeap.AddTimer(DEFAULT_FRAME_TIME, 0, SpeedUp);
        }
    }


    protected void SpeedDown()
    {
        if (isSpeedDown)
        {
            globalSpeed -= globalSpeedDownRate;

            if (globalSpeed <= -speedDownRate)
            {
                globalSpeed = -speedDownRate;
                isSpeedDown = false;
            }

            ControlAllAnimation(globalSpeed);

            timer = TimerHeap.AddTimer(DEFAULT_FRAME_TIME, 0, SpeedDown);
        }
    }

    protected void Stop()
    {
        if (isSpeedStop)
        {
            if (globalSpeed > 0)
            {
                globalSpeed -= globalSpeedDownRate;
                if (globalSpeed <= 0)
                {
                    globalSpeed = 0;
                    isSpeedStop = false;

                    ControlAllAnimation(globalSpeed);
                }
            }
            else
            {
                globalSpeed += globalSpeedUpRate;
                if (globalSpeed >= 0)
                {
                    globalSpeed = 0;
                    isSpeedStop = false;

                    ControlAllAnimation(globalSpeed);
                }
            }

            ControlAllAnimation(globalSpeed);

            timer = TimerHeap.AddTimer(DEFAULT_FRAME_TIME, 0, Stop);
        }
    }

    protected void ControlAllAnimation(float theSpeed)
    {
        if (animaList != null)
        {
            foreach (var anima in animaList)
            {
                if (anima.clip == null)
                    continue;
                anima[anima.clip.name].speed = theSpeed;
                anima.Play();
            }
        }
    }
}