// 模块名   :  MogoMotorMonsterClient
// 创建者   :  莫卓豪
// 创建日期 :  2012-3-20
// 描    述 :  怪物控制(怪物单机版)

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Mogo.Util;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class MogoMotorMonsterClient : MogoMotor
{
    private CharacterController characterController;
    private NavMeshPath path;
    private uint cornersIdx = 0;
    private float currentPathPointDistance = 0;

    public NavMeshAgent navAgent;

    // Use this for initialization
    void Start()
    {
        navAgent = transform.GetComponent<NavMeshAgent>();
        characterController = transform.GetComponent<CharacterController>();
        characterController.center = new Vector3(0, 1, 0);
        path = new NavMeshPath();
        GetComponent<Animator>().applyRootMotion = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingToTarget)
        {
            float distance = Vector3.Distance(path.corners[cornersIdx], transform.position);
            if (distance > currentPathPointDistance)
            {
                //如果偏离点太远，重新计算
                MoveTo(navAgent.destination);
                return;
            }
            //暂用0.1代替实际应该传步长，但现在不知道速度（动作带位移）
            if (cornersIdx < path.corners.Length - 1)
            {
                if (distance < 0.1)
                {
                    transform.position = path.corners[cornersIdx];
                    cornersIdx++;
                    //面向下一个转角点
                    moveDirection = (path.corners[cornersIdx] - transform.position);
                    currentPathPointDistance = Vector3.Distance(transform.position, path.corners[cornersIdx]);
                }
            }

            else if (distance < 0.1 + navAgent.stoppingDistance)
            {
                transform.position = path.corners[cornersIdx] - transform.forward * navAgent.stoppingDistance;
                StopNav();
                EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
            }


            ApplyRotation();
        }
        //最后移动方向以moveDirection为准，而非transform.forward
        Move();
    }

    public override void StopNav()
    {
        currentPathPointDistance = 0;
        cornersIdx = 0;
        speed = 0;
        isMovingToTarget = false;
        navAgent.Stop();
        navAgent.ResetPath();
    }

    public override void MoveTo(Vector3 v, bool needToAdjustPosY = true)
    {
        speed = 3f;
        isMovingToTarget = true;
        navAgent.speed = 0.0f;
        navAgent.SetDestination(v);
        path.ClearCorners();
        navAgent.CalculatePath(v, path);
        cornersIdx = 1;
        moveDirection = (path.corners[cornersIdx] - transform.position).normalized;
        currentPathPointDistance = Vector3.Distance(transform.position, path.corners[cornersIdx]);
    }

    public override void SetStopDistance(float distance)
    {
        navAgent.stoppingDistance = distance;
    }

    private void ApplyRotation()
    {
        float targetAngleY;
        if (moveDirection.x > 0)
        {
            targetAngleY = Vector3.Angle(moveDirection, Vector3.forward);
        }
        else
        {
            targetAngleY = Vector3.Angle(moveDirection, Vector3.back) + 180;
        }

        base.ApplyRotation(targetAngleY);
    }

    private void Move()
    {
        characterController.Move(speed * moveDirection * Time.deltaTime);
    }
}