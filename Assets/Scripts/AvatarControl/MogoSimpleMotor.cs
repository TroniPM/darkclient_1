// 模块名   :  MogoMotor
// 创建者   :  莫卓豪
// 创建日期 :  2012-1-18
// 描    述 :  位移控制，单纯Translate,无朝向，用于机关陷阱等移动

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MogoSimpleMotor : MonoBehaviour
{
    private float speed = 1f;

    private float rotationSpeedx = 0f;
    private float rotationSpeedy = 0f;
    private float rotationSpeedz = 0f;

    Vector3 targetToMoveTo;
    Vector3 directionToMove;

    bool isMovingToTarget = false;
    bool isRotating = false;
    bool isMove = false;


    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    public void SetMoveSpeed(Vector3 theDirectionToMove)
    {
        directionToMove = theDirectionToMove.normalized;
        isMove = true;
    }

    public void SetRotateSpeed(float theRotationSpeedx, float theRotationSpeedy, float theRotationSpeedz)
    {
        rotationSpeedx = theRotationSpeedx;
        rotationSpeedy = theRotationSpeedy;
        rotationSpeedz = theRotationSpeedz;
        isRotating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingToTarget)
        {
            MoveTo(targetToMoveTo);
        }
        else if (isMove)
            Move();

        if (isRotating)
            Rotate();
    }


    public void StopMoveTo()
    {
        isMovingToTarget = false;
    }

    public void StopMove()
    {
        isMove = false;
    }

    public void StopRotate()
    {
        isRotating = false;
    }

    public void MoveTo(Vector3 v)
    {
        targetToMoveTo = v;

        isMovingToTarget = true;

        float dis = Vector3.Distance(transform.position, targetToMoveTo);

        if (dis > speed * Time.deltaTime)
        {
            Vector3 direction = targetToMoveTo - transform.position;
            direction = direction.normalized;
            transform.position = transform.position + direction * speed * Time.deltaTime;
            //transform.Translate(, Space.Self);
        }
        else
        {
            transform.position = targetToMoveTo;
            isMovingToTarget = false;
            EventDispatcher.TriggerEvent(Events.GearEvent.MotorHandleEnd, this as MonoBehaviour);
        }
    }

    public void Move()
    {
        transform.Translate(Time.deltaTime * directionToMove.x * speed, Time.deltaTime * directionToMove.y * speed, Time.deltaTime * directionToMove.z * speed);
    }

    public void Rotate()
    {
        transform.Rotate(Time.deltaTime * rotationSpeedx, Time.deltaTime * rotationSpeedy, Time.deltaTime * rotationSpeedz);
    }
}
