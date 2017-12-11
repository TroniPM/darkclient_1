using UnityEngine;
using System.Collections;

using Mogo.Util;

// going to fuck this

public class CameraStasiser : GearParent
{
    public bool isFocus = true;
    public int delayTime = 0;

    protected uint timer;

    private int cnt = 0;

    void Start()
    {
        gearType = "CameraChanger";
        ID = (uint)defaultID;

        timer = uint.MaxValue;

        AddListeners();
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(timer);
        RemoveListeners();
    }

    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                timer = TimerHeap.AddTimer((uint)delayTime, 0, ChanerCamera);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (timer == uint.MaxValue)
                {
                    timer = TimerHeap.AddTimer((uint)delayTime, 0, ChanerCamera);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                TimerHeap.DelTimer(timer);
                timer = uint.MaxValue;
            }
        }
    }

    #endregion


    #region 机关事件

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        if (stateOneID == ID)
        {
            base.SetGearEventStateOne(stateOneID);
            ChanerCamera();
        }
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        if (stateTwoID == ID)
        {
            base.SetGearEventStateTwo(stateTwoID);
            ResetCamera();
        }
    }

    #endregion


    private void ChanerCamera()
    {
        if (MogoMainCamera.Instance == null)
        {//处理断线重连后摄像头还未初始化
            if (cnt > 10)
            {//超时
                return;
            }
            timer = TimerHeap.AddTimer(500, 0, ChanerCamera);
            cnt++;
            return;
        }

        if (isFocus)
            MogoMainCamera.Instance.LockSight2();
        else
            MogoMainCamera.Instance.CurrentState = MogoMainCamera.NONE;
    }

    private void ResetCamera()
    {
        MogoMainCamera.Instance.LockSight();
    }
}
