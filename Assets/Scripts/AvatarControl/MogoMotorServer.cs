// 模块名   :  MogoMotor
// 创建者   :  莫卓豪
// 创建日期 :  2012-1-18
// 描    述 :  移动控制（服务器调用版）

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Mogo.Util;

//[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class MogoMotorServer : MogoMotor
{
    private CharacterController characterController;
    private Animator animator;
    const int SPEED = 4;//估计速度，动作位移时无法得到具体速度
    private bool isMovingToInATime = false;
    private float moveTimeBegin;
    private float moveTime;

    private NavMeshPath path = new NavMeshPath();
    private int m_cornersIdx;
    MogoNavHelper m_navHelper;

    /// <summary>d
    /// 设置animator目标速度，motor根据acceleration加速
    /// 用于跑步停止动作融合
    /// </summary>
    /// <param name="_speed"></param>
    public override void SetSpeed(float _speed)
    {
        ////Mogo.Util.LoggerHelper.Debug("setSpeed:" + _speed);
        moveSpeed = _speed;
        targetSpeed = moveSpeed;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        path = new NavMeshPath();
        gravity = 20f;//防止同时存在NavMeshAgent和CharacterController时发生震荡现象
        m_navHelper = new MogoNavHelper(transform);

        InvokeRepeating("AdjustPosition", 0, 1);
    }

    void OnDestroy()
    {
        CancelInvoke("AdjustPosition");
    }
    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;
        //Time.timeScale = 0.1f;
        //LoggerHelper.Debug("speed:" + speed);
        //moveDirection = Vector3.zero;
        Hover();
        ApplyGravity();

        if (isLookingAtTarget && canTurn)
        {
            if (targetToLookAtTransform != null)
            {
                RotateTo(targetToLookAtTransform.position);
            }
            else
            {
                RotateTo(targetToLookAt);
            }
        }
        if (isMovingToTarget)
        {
            //if (isTurning) animator.SetFloat("Speed", 0);
            MoveTo(targetToMoveTo, false);
        }
        if (isNaving)
        {
            MoveToByNav(targetToMoveTo, m_stopDistance, false);
        }

        if (isDragTo)
        {
            DragTo(targetToDragTo, dragSpeed);
        }

        if (isRotatingTo)
        {
            ApplyRotation(rotationY);
        }

        if (isMovingToInATime)
        {

            //Mogo.Util.LoggerHelper.Debug("Time.time - moveTimeBegin:" + (Time.time - moveTimeBegin));
            //Mogo.Util.LoggerHelper.Debug("moveTime:" + (moveTime));

            if (Time.time - moveTimeBegin > moveTime)
            {
                isMovingToInATime = false;
                extraSpeed = 0;
                EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
            }
            else
            {
                targetSpeed = 0.4f;
            }
        }

        //speed = AccelerateSpeed(speed, targetSpeed);
        //animator.SetFloat("Speed", speed);
        Move();

    }

    private void ApplyGravity()
    {
        if (IsGrounded())
        {
            verticalSpeed = 0.0f;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }

    }

    private void Move()
    {
        if (characterController != null && characterController.enabled == true)
        {
            collisionFlags = characterController.Move((extraSpeed * moveDirection + new Vector3(0, verticalSpeed, 0)) * Time.deltaTime);
        }

        SetAnimationByDirection(moveDirection);
    }

    float speedZ = 0;
    float speedX = 0;
    float targetZ = 0;
    float targetX = 0;
    int lastFrameDirection = 0;//0无，1234前后左右

    private void SetAnimationByDirection(Vector3 moveDirection)
    {

        //animator.GetFloat(""
        targetX = 0;
        targetZ = 0;
        if (targetSpeed > 0)
        {
            float angle = Vector3.Angle(moveDirection, transform.forward);
            if (angle <= 45)
            {
                lastFrameDirection = 1;
                targetZ = targetSpeed;
            }
            else if (angle > 135)
            {
                lastFrameDirection = 2;
                targetZ = -targetSpeed;
            }
            else
            {
                angle = Vector3.Angle(moveDirection, transform.right);
                if (angle <= 45)
                {
                    targetX = targetSpeed;
                    lastFrameDirection = 4;
                }
                else if (angle > 135)
                {
                    targetX = -targetSpeed;
                    lastFrameDirection = 3;
                }
            }
        }

        speedZ = AccelerateSpeed(speedZ, targetZ);
        speedX = AccelerateSpeed(speedX, targetX);
        animator.SetFloat("Speed", speedZ);
        animator.SetFloat("SpeedX", speedX);
    }

    public override void TeleportTo(Vector3 destination)
    {
        Vector3 movement = destination - transform.position;
        collisionFlags = characterController.Move(movement);
    }

    override public bool MoveToByNav(Vector3 _v, float _stopDistance = 0f, bool needToAdjustPosY = true)
    {

        if (!canMove) return false;
        if (needToAdjustPosY)
        {
            bool hasHit = MogoUtils.GetPointInTerrain(_v.x, _v.z, out _v);

            if (!hasHit)
            {
                EventDispatcher.TriggerEvent(ON_MOVE_TO_FALSE, transform.gameObject, targetToMoveTo, 0f);
                LoggerHelper.Warning("!hasHit:" + _v);
                return false;
            }
        }

        //计算路线
        if (!isNaving || needToAdjustPosY)//|| (targetToMoveTo - v).magnitude < 0.05f
        {
            if (m_navHelper != null)
            {
                //Debug.LogError("targetToMoveTO:" + _v);
                path = m_navHelper.GetPathByTarget(_v);
                targetToMoveTo = _v;
                m_cornersIdx = 1;
                m_stopDistance = _stopDistance;
            }
            else
            {
                EventDispatcher.TriggerEvent(ON_MOVE_TO_FALSE, transform.gameObject, targetToMoveTo, 0f);
                return false;
            }

        }

        //Mogo.Util.LoggerHelper.Debug("path.corners.Length:" + path.corners.Length);
        if (path == null)
        {
            StopNav();
            EventDispatcher.TriggerEvent(ON_MOVE_TO_FALSE, transform.gameObject, targetToMoveTo, 0f);
            return false;
        } 
        if (path.corners.Length < 2)
        {
            EventDispatcher.TriggerEvent(ON_MOVE_TO_FALSE, transform.gameObject, targetToMoveTo, 0f);
            LoggerHelper.Warning("path.corners.Length < 2:" + targetToMoveTo);
            StopNav();
            //EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
            return false;
        }

        isNaving = true;
        moveDirection = (path.corners[m_cornersIdx] - transform.position).normalized;
        RotateTo(path.corners[m_cornersIdx]);
        //transform.LookAt(new Vector3(path.corners[m_cornersIdx].x, transform.position.y, path.corners[m_cornersIdx].z));
        float dis = Vector3.Distance(transform.position, path.corners[m_cornersIdx]);
        float step = extraSpeed * Time.deltaTime;

        if (step + 0.1f > dis && m_cornersIdx < path.corners.Length - 1)
        {
            collisionFlags = characterController.Move(path.corners[m_cornersIdx] - transform.position);
            if ((path.corners[m_cornersIdx] - transform.position).magnitude > 0.3f)
            {
                path = m_navHelper.GetPathByTarget(targetToMoveTo);
                m_cornersIdx = 1;
            }
            else
            {
                m_cornersIdx++;
            }

            moveDirection = Vector3.zero;
            //transform.LookAt(new Vector3(path.corners[m_cornersIdx].x, transform.position.y, path.corners[m_cornersIdx].z));
            //collisionFlags = characterController.Move(moveDirection * dis);

            //transform.position = new Vector3(path.corners[m_cornersIdx].x, transform.position.y, path.corners[m_cornersIdx].z);
            //m_cornersIdx++;

        }
        else if (m_cornersIdx == path.corners.Length - 1 && step + 0.1f + m_stopDistance > dis)
        {
           
            //Debug.LogError("StopNav");
            float tempDis;
            //for (int i = 0; i < 5 && tempDis > 0.1; i++, dis = Vector3.Distance(transform.position, targetToMoveTo))
            //{
            tempDis = dis - m_stopDistance;
            tempDis = tempDis > 0 ? tempDis : 0;
            collisionFlags = characterController.Move((path.corners[m_cornersIdx] - transform.position).normalized * tempDis);
            //}
            StopNav();
            tempDis = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetToMoveTo.x, targetToMoveTo.z));
            if (tempDis > 0.2 + m_stopDistance)
            {
                //MogoWorld.thePlayer.Idle();
                EventDispatcher.TriggerEvent(ON_MOVE_TO_FALSE, transform.gameObject, targetToMoveTo, tempDis);
            }
            else
            {
                EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
            }
        }
        return true;
    }

    /// <summary>
    /// 在需要回调到达目标时的地方使用：EventDispatcher.AddEventListener<GameObject, Vector3>(ON_MOVE_TO, func);
    /// </summary>
    /// <param name="v"></param>
    /// <param name="space"></param>
    public override void MoveTo(Vector3 v, bool needToAdjustPosY = true)
    {
        if (!canMove) return;
        if (needToAdjustPosY)
        {
            //Mogo.Util.LoggerHelper.Debug("MoveTo:" + v + ",speed:" + extraSpeed);
            bool hasHit = MogoUtils.GetPointInTerrain(v.x, v.z, out v);

            if (!hasHit)
                return;
        }

        //v = hitInfo.point;
        //Vector3 position = transform.position;
        //if (Mathf.Abs(v.x - position.x) < 0.1f && Mathf.Abs(v.y - position.y) < 0.1f)
        //{
        //    StopNav();
        //    return;
        //}
        targetToMoveTo = v;
        isMovingToTarget = true;

        //transform.LookAt(new Vector3(targetToMoveTo.x, transform.position.y, targetToMoveTo.z));
        if (isNeedRotation) RotateTo(targetToMoveTo);


        float dis = Vector3.Distance(transform.position, targetToMoveTo);

        //动作带位移时候转向需要把移动停下来
        //if (dis < extraSpeed * turnAroundTime * 2 + 1 && isTurning)//
        //{
        //    targetSpeed = 0;
        //    moveDirection = Vector3.zero;
        //}
        //else
        //{
        //    targetSpeed = moveSpeed;
        //    moveDirection = transform.forward.normalized;
        //}
        moveDirection = targetToMoveTo - transform.position;
        moveDirection = moveDirection.normalized;

        //到达目的地
        float step = extraSpeed * Time.deltaTime;
        //LoggerHelper.Debug("SPEED * speed:" + SPEED * speed);
        //Mogo.Util.LoggerHelper.Debug("dis:" + dis);
        //Mogo.Util.LoggerHelper.Debug("step+0.05f:" + (step + 0.05f));
        if (dis < step + 0.05f)
        {
            if (canTurn && isNeedRotation)
            {
                transform.LookAt(new Vector3(targetToMoveTo.x, transform.position.y, targetToMoveTo.z));
            }
            transform.position = new Vector3(targetToMoveTo.x, transform.position.y, targetToMoveTo.z);
            StopNav();
            EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
        }

        //else if (isTurning && dis < 0.5f)
        //{
        //    LoggerHelper.Debug("is too close to turn around,so just stop it");
        //    transform.LookAt(new Vector3(targetToMoveTo.x, transform.position.y, targetToMoveTo.z));
        //    transform.position = new Vector3(targetToMoveTo.x, transform.position.y, targetToMoveTo.z);
        //    StopNav();
        //    EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
        //}
        //else
        //{
        //    characterController.Move(direction * step);
        //    //transform.Translate(direction * step, Space.World);
        //}
    }

    public override void MoveToByAngle(float angleY, float _time)
    {
        if (!canMove) return;
        Vector3 original = transform.eulerAngles;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, angleY, transform.eulerAngles.z);
        moveDirection = transform.forward;
        transform.eulerAngles = original;

        if (isNeedRotation) RotateTo(angleY);


        moveTime = _time;
        moveTimeBegin = Time.time;
        isMovingToInATime = true;
    }

    public override void StopNav()
    {
        //transform.position = targetToMoveTo;
        isMovingToTarget = false;
        isRotatingTo = false;
        moveDirection = Vector3.zero;
        extraSpeed = 0f;
        //targetSpeed = 0f;
        //moveSpeed = 0f;
        //speed = 0f;


        //navAgent.Stop();
        //navAgent.ResetPath();
        isNaving = false;
    }

    private float upTime = 0;
    private float downTime = 0;
    private float HOVER_G = 5;
    public void HitHover(int time)
    {
        gravity = -HOVER_G;
        upTime += time * 0.5f;
        downTime += time * 0.5f;
        Hover();
    }

    public void HitFly()
    {
    }

    private void Hover()
    {
        if (upTime <= 0 && downTime <= 0)
        {
            return;
        }
        float t = Time.deltaTime * 1000;
        upTime -= t;
        if (upTime > 0)
        {//上升阶段
            return;
        }
        gravity = HOVER_G;
        downTime -= t;
        if (downTime > 0)
        {
            return;
        }
        //navAgent.enabled = true;
        upTime = 0;
        downTime = 0;
        gravity = 0;
        verticalSpeed = 0;
        gravity = 20;
    }
}