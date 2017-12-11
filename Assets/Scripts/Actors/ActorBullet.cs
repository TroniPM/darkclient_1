/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：Bullet
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-5-27
// 模块描述: 飞行物控制器（用于火球，弓箭等物体的特效播放，追踪目标的显示逻辑）
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.GameData;
using Mogo.Util;

public class ActorBullet : MonoBehaviour
{

    public float speed = 10;
    public Transform target;
    public Vector3 targetPosition;
    public bool isSetup = false;
    //public int boomId;
    // Use this for initialization

    public System.Action OnDestroy = null;



    public void Setup(Transform target, float speed, Vector3 targetPosition)
    {
        this.target = target;

        this.targetPosition = targetPosition;

        if (target != null)
        {
            targetPosition = target.position;
        }
        this.speed = speed;
        //this.boomId = boomId;

        //if (boomId != -1)
        //{
        //    FXData data = FXData.dataMap[boomId];
        //    targetPosition = MogoUtils.GetChild(target, data.slot);

        //}

        isSetup = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSetup) return;
        //射向目标
        if (target != null)
        {
            targetPosition = target.position;
        }
      
        transform.LookAt(targetPosition);
        float step = speed * Time.deltaTime;
        float distance = Vector3.Distance(targetPosition, transform.position);

        if (distance < step)
        {
            //到达目标
            transform.position = targetPosition;

            
            //if (boomId != -1 && target != null)
            //{
            //    target.GetComponent<SfxHandler>().HandleFx(boomId);
            //}
            if (OnDestroy != null)
                OnDestroy();
        }
        else
        {
            transform.Translate(step * transform.forward, Space.World);
        }

    }
}
