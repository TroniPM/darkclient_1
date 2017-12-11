/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：GearEffectListener
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：20130219
// 最后修改日期：20130220
// 模块描述：提供接口给各种齿轮调用
 * 注意：当前的处理方式是服务器的相关管理调用这边的函数进行Avatar数值的改变
 * 而不是本地自行处理数据
// 代码版本：V1.3
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;


public class GearEffectListener : MonoBehaviour
{
    private MogoMotorMyself mogoMotor;

    private float preSpeed;

    void Start()
    {
        TimerHeap.AddTimer(100, 0, SetData);
    }

    private void SetData()
    {
        mogoMotor = transform.GetComponent<MogoMotorMyself>();
        preSpeed = mogoMotor.speed;
    }

    public void SetControl(bool curIsMovable)
    {
        mogoMotor.isMovable = curIsMovable;
    }

    public void SetMoveTo(Vector3 target, bool bLookAtTarget = false)
    {
    //    mogoMotor.MoveTo(target, bLookAtTarget);
    }

    public void SetMoveToDirectly(Vector3 distance)
    {
        transform.position += distance; 
    }

    public void SpeedDown(float coefficient)
    {
        if (mogoMotor.speed == preSpeed)
        {
            preSpeed = mogoMotor.speed;
            mogoMotor.SetSpeed(preSpeed * coefficient);
        }
    }

    public void SpeedUp()
    {
        if (mogoMotor.speed != preSpeed)
        {
            mogoMotor.SetSpeed(preSpeed);
        }
    }

    public void SetDamage(float damage)
    {
        LoggerHelper.Debug("Damage: " + damage);
    }

    public void Teleport(int targetSceneId, string targetLabel)
    {
        // Rpc.Call();
        LoggerHelper.Debug("Teleport To Scene " + targetSceneId + " label " + targetLabel);
    }
}
