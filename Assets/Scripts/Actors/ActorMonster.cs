/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ActorMonster
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;
using Mogo.FSM;

public class ActorMonster : ActorParent<EntityMonster>
{
    protected Transform m_billboardTrans;

    // 初始化
    void Start()
    {
        gameObject.layer = 11;
        m_billboardTrans = transform.Find("slot_billboard");
    }

    // 每帧调用
    void Update()
    {
        ActChange();
        if (m_billboardTrans != null && theEntity != null)
        {
            //EventDispatcher.TriggerEvent<Vector3, uint>(BillboardViewManager.BillboardViewEvent.UPDATEBILLBOARDPOS,

            // m_billboardTrans.position, theEntity.ID);

            BillboardViewManager.Instance.UpdateBillboardPos(m_billboardTrans.position, theEntity.ID);
        }
    }
}