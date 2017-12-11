/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ActorDummy
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-5-20
// 模块描述：佣兵
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;
using Mogo.FSM;

public class ActorMercenary : ActorParent<EntityMercenary>
{
	protected Transform m_billboardTrans;


	protected override void Awake()
	{
		m_billboardTrans = transform.Find("slot_billboard");
        base.Awake();
	}

	// 每帧调用
	void Update()
	{
        ActChange();
		if (m_billboardTrans != null && theEntity != null)
		{
            //EventDispatcher.TriggerEvent<Vector3, uint>(BillboardViewManager.BillboardViewEvent.UPDATEBILLBOARDPOS,

            //  m_billboardTrans.position, theEntity.ID);
            BillboardViewManager.Instance.UpdateBillboardPos(m_billboardTrans.position, theEntity.ID);
		}
	}
}
