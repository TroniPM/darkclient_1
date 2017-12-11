/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ActorNPC
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Game;
using Mogo.Util;

public class ActorNPC : MonoBehaviour
{
	#region 变量

	public EntityNPC theEntity;

    public Transform billboardTrans = null;

	#endregion

	// 初始化
	void Awake()
	{
		// this.gameObject.AddComponent<CharacterController>();
		//MogoMotor theMotor = this.gameObject.AddComponent<MogoMotor>();
		// EventDispatcher.AddEventListener(Events.NPCEvent.FrushIcon, FrushIcon);

        billboardTrans = transform.Find("slot_billboard");
	}

	void OnBecameVisible()
	{
		theEntity.SetFlush(true);
		EventDispatcher.TriggerEvent<int>(Events.TaskEvent.NPCInSight, (int)theEntity.ID);
	}

	void OnBecameInvisible()
	{
		theEntity.SetFlush(false);
	}

	void OnTriggerEnter(Collider collider)
	{
        if (collider.tag == "Player" && MogoWorld.thePlayer.CurrentTask != null)
		{
			EventDispatcher.TriggerEvent(Events.TaskEvent.CloseToNPC, (int)theEntity.ID);
            EventDispatcher.TriggerEvent(Events.NPCEvent.TurnToPlayer, MogoWorld.thePlayer.CurrentTask.npc, MogoWorld.thePlayer.Transform);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Player")
		{
			EventDispatcher.TriggerEvent(Events.TaskEvent.LeaveFromNPC, (int)theEntity.ID);
		}
	}

    void Update()
    {
        if (MogoWorld.thePlayer != null && MogoWorld.thePlayer.sceneId == MogoWorld.globalSetting.homeScene && billboardTrans != null && theEntity != null)
        {
            //EventDispatcher.TriggerEvent<Vector3, uint>(BillboardViewManager.BillboardViewEvent.UPDATEBILLBOARDPOS, billboardTrans.position, theEntity.ID);
            BillboardViewManager.Instance.UpdateBillboardPos(billboardTrans.position, theEntity.ID);
        }
    }
}
