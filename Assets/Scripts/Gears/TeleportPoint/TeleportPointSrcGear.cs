/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ConveyerGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：
// 最后修改日期：
// 模块描述：
// 代码版本：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;

public class TeleportPointSrcGear : GearParent
{
	public int targetSceneId;
	public int delayTime;

    public bool camIsFollow = true;
    public bool camIsSpin = false;

    public float distance;
    public float rotationY;
    public float rotationX;

	public float cameraTime = 1;

	protected uint timer;
    protected uint teleportTimer;

	public TeleportPointDesGear desGear;
	protected GameObject player;

	protected bool autoProcess;

	void Start() 
	{
		gearType = "TeleportPointSrc";
		ID = (uint)defaultID;

		timer = uint.MaxValue;
        teleportTimer = uint.MaxValue;

		autoProcess = true;

		AddListeners();
	}

	void OnDestroy()
	{
		RemoveListeners();
        TimerHeap.DelTimer(timer);
        TimerHeap.DelTimer(teleportTimer);
	}


	#region 机关触发

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == GearParent.MogoPlayerTag)
		{
			if (triggleEnable)
			{
				// ControlStick.instance.Reset();
                player = other.gameObject;
				timer = TimerHeap.AddTimer((uint)delayTime, 0, Teleport);
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
                    player = other.gameObject;
					timer = TimerHeap.AddTimer((uint)delayTime, 0, Teleport);
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
        //if (other.tag == GearParent.MogoPlayerTag)
        //{
        //    if (triggleEnable)
        //    {
        //        TimerHeap.DelTimer(timer);

        //        timer = uint.MaxValue;
        //        autoProcess = true;
        //    }
        //}
	}

	#endregion


    #region 机关事件

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        if (stateOneID == ID)
        {
            base.SetGearEventStateOne(stateOneID);
            player = MogoWorld.thePlayer.GameObject;
            Teleport();
        }
    }

    #endregion


	public void ResetTimer()
	{
		timer = uint.MaxValue;
        teleportTimer = uint.MaxValue;
        autoProcess = true;

        teleportTimer = TimerHeap.AddTimer((uint)(cameraTime * 1000), 0, () =>
        {
            EventDispatcher.TriggerEvent(Events.GearEvent.TrapEnd, "TeleportPointDes"); 
        }); 
	}

    protected void Teleport()
    {
        // EventDispatcher.TriggerEvent<uint>(Events.GearEvent.Teleport, ID);
        if (autoProcess)
        {
            autoProcess = false;
            EventDispatcher.TriggerEvent(Events.GearEvent.TrapBegin, gearType);
        }

        teleportTimer = TimerHeap.AddTimer(20, 0, () =>
        {
            #region 进度记录

            GameProcManager.ClientTeleport(MogoWorld.thePlayer.CurMissionID, MogoWorld.thePlayer.CurMissionLevel, (int)ID);

            #endregion

            player.transform.position = desGear.transform.position;
            if (camIsFollow)
            {
                if (camIsSpin)
                    MogoMainCamera.Instance.ChangeLockSightData(distance, rotationY, rotationX, cameraTime, cameraTime, false);
                else
                    MogoMainCamera.Instance.CloseToTarget(player.transform.Find("slot_camera"), MogoMainCamera.Instance.m_distance, MogoMainCamera.Instance.m_rotationY, MogoMainCamera.Instance.m_rotationX, cameraTime, cameraTime, MogoMainCamera.Instance.LockSight);
            }
        });
    }
}
