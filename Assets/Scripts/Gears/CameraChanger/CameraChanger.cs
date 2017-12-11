using UnityEngine;
using System.Collections;

using Mogo.Util;

public class CameraChanger : GearParent
{
    public float distance;
    public float rotationY;
    public float rotationX;
    public float moveTime;
    public float rotateTime;

    public int delayTime = 0;

    protected uint timer;
    protected GameObject player;

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
                // ControlStick.instance.Reset();
                timer = TimerHeap.AddTimer((uint)delayTime, 0, ChanerCamera);

                player = other.gameObject;
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
                    // ControlStick.instance.Reset();
                    timer = TimerHeap.AddTimer((uint)delayTime, 0, ChanerCamera);

                    player = other.gameObject;
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

        MogoMainCamera.Instance.ChangeLockSightData(distance, rotationY, rotationX, moveTime, rotateTime, MogoMainCamera.Instance.CurrentState == MogoMainCamera.LOCK_SIGHT);

        //MogoMainCamera.Instance.CloseToTarget(MogoWorld.thePlayer.Transform, distance, rotationY, rotationX, moveTime, rotateTime, () =>
        //{
        //    MogoMainCamera.Instance.ChangeLockSightData(distance, rotationY, rotationX, moveTime, rotateTime);
        //});
    }
}
