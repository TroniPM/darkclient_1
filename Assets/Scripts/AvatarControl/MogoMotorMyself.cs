// 模块名   :  MogoMotor
// 创建者   :  莫卓豪
// 创建日期 :  2013-1-18
// 描    述 :  人物控制

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Mogo.Util;

public class MogoMotorMyself : MogoMotor
{
    Animator animator;

    public CharacterController characterController;

    private NavMeshPath path;
    private uint m_cornersIdx = 0;
    private uint m_timerIdForNav;
    float m_fCanThinkTime = 0.0f;
    MogoNavHelper m_navHelper;
    private bool m_isMovingOn = false;
    private bool isMovingToTargetWithoutNav = false;

    void Start()
    {
        InvokeRepeating("AdjustPosition", 0, 1);
        SetAngularSpeed(100000);
    }

    void OnDestroy()
    {
        CancelInvoke("AdjustPosition");
    }

    // Use this for initialization
    void Awake()
    {
        characterController = transform.gameObject.GetComponent<CharacterController>();
        enableStick = true;
        hasInited = true;
        animator = GetComponent<Animator>();

        m_navHelper = new MogoNavHelper(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!MogoWorld.inCity)
        {
            if (Time.time > m_fCanThinkTime)
            {
                EventDispatcher.TriggerEvent(Events.AIEvent.DummyThink);

                m_fCanThinkTime = Time.time + 0.1f;
            }
        }

        ApplyGravity();

        if (!canMove)
        {
            //LoggerHelper.Error("!canMove");
            return;
        }
        if (!animator.runtimeAnimatorController)
            return;



        if (isLookingAtTarget)
        {
            //transform.LookAt(targetToLookAt);
            transform.LookAt(new Vector3(targetToLookAt.x, transform.position.y, targetToLookAt.z));
        }

        if (enableStick && ControlStick.instance != null && (ControlStick.instance.isDraging))
        {

            //if (isMovingToTarget) 
            StopNav();
            //m_isMovingOn = false;
            //TimerHeap.DelTimer(m_timerIdForNav);
            if (ControlStick.instance.IsDraging)
            {
                if (Camera.main)
                {
                    if (enableRotation)
                    {
                        ApplyRotation();
                        moveDirection = transform.forward;
                    }
                    else
                    {
                        int i = ControlStick.instance.direction.x > 0 ? 1 : -1;
                        float targetAngleY = i * Vector2.Angle(new Vector2(0, -1), ControlStick.instance.direction) + Camera.main.transform.eulerAngles.y;
                        Vector3 original = transform.eulerAngles;
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngleY, transform.eulerAngles.z);
                        moveDirection = transform.forward;
                        transform.eulerAngles = original;
                    }
                }
            }
            else
            {
                if (enableRotation)
                {
                    ApplyRotation();
                    moveDirection = transform.forward;
                    MogoWorld.thePlayer.Idle();
                }
            }

        }
        else if (isMovingToTarget)
        {
            //Debug.LogError("isMovingToTarget");
            //if (!MogoWorld.inCity)
            //{
            //    //暂时测试用
            //    MoveToWithoutNav(targetToMoveTo);
            //    speed = AccelerateSpeed(speed, targetSpeed);
            //    animator.SetFloat("Speed", speed);
            //}
            //else
            //{

            MoveTo(targetToMoveTo, false);
            //}
        }
        else if (isMovingToTargetWithoutNav)
        {
            //Debug.LogError("isMovingToTargetWithoutNav");
            MoveToWithoutNav(targetToMoveTo);
        }

        if (isDragTo)
        {
            DragTo(targetToDragTo, dragSpeed);
        }
        speed = AccelerateSpeed(speed, targetSpeed);
        animator.SetFloat("Speed", speed);

        Move();
    }

    override public void TurnToControlStickDir()
    {
        if (!ControlStick.instance.IsDraging) return;

        int i = ControlStick.instance.direction.x > 0 ? 1 : -1;
        float targetAngleY = i * Vector2.Angle(new Vector2(0, -1), ControlStick.instance.direction) + Camera.main.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngleY, transform.eulerAngles.z);
    }

    /// <summary>
    /// 设置animator目标速度，motor根据acceleration加速
    /// 用于跑步停止动作融合
    /// </summary>
    /// <param name="_speed"></param>
    public override void SetSpeed(float _speed)
    {
        targetSpeed = _speed;
        moveSpeed = _speed;
    }

    public override void StopNav()
    {
        m_cornersIdx = 1;
        isMovingToTarget = false;
        m_isMovingOn = false;
        TimerHeap.DelTimer(m_timerIdForNav);
    }

    public override bool MoveToByNav(Vector3 v, float stopDistance = 0f, bool needToAdjustPosY = true)
    {
        //Mogo.Util.LoggerHelper.Error("MoveToByNav v:" + v);
        if (!canMove) return false;
        if (m_isMovingOn) return false;

        Mogo.Util.LoggerHelper.Debug("v:" + v);
        if (needToAdjustPosY)
        {
            bool hasHit = MogoUtils.GetPointInTerrain(v.x, v.z, out v);

            if (!hasHit)
                return false;
        }
        Mogo.Util.LoggerHelper.Debug("v:" + v);
        //计算路线
        if (!isMovingToTarget || IsSideCrash())//|| (targetToMoveTo - v).magnitude < 0.05f
        {
            Mogo.Util.LoggerHelper.Debug("IsSideCrash!");
            path = m_navHelper.GetPathByTarget(v);
            Mogo.Util.LoggerHelper.Debug("path:" + path.corners.Length);
            targetToMoveTo = v;
            m_cornersIdx = 1;
        }

        //Mogo.Util.LoggerHelper.Debug("path.corners.Length:" + path.corners.Length);
        if (path.corners.Length < 2)
        {
            Mogo.Util.LoggerHelper.Debug("path.corners.Length < 2");
            StopNav();
            return false;
        }

        isMovingToTarget = true;
        moveDirection = (path.corners[m_cornersIdx] - transform.position).normalized;
        //RotateTo(path.corners[m_cornersIdx]);
        transform.LookAt(new Vector3(path.corners[m_cornersIdx].x, transform.position.y, path.corners[m_cornersIdx].z));
        float dis = Vector3.Distance(transform.position, path.corners[m_cornersIdx]);
        float step = 8 * Time.deltaTime;
        Mogo.Util.LoggerHelper.Debug("dis:" + dis);
        Mogo.Util.LoggerHelper.Debug("step:" + step);
        if (step + 1f > dis && m_cornersIdx < path.corners.Length - 1)
        {
            //collisionFlags = characterController.Move(transform.forward * dis);
            Mogo.Util.LoggerHelper.Debug("m_isMovingOn = true;");
            m_isMovingOn = true;
            StopNav();

            m_timerIdForNav = TimerHeap.AddTimer(400, 0, () =>
            {
                m_isMovingOn = false;
                MoveTo(targetToMoveTo, false);

            });
            return true;
            //collisionFlags = characterController.Move(transform.forward * dis);

            //transform.position = new Vector3(path.corners[m_cornersIdx].x, transform.position.y, path.corners[m_cornersIdx].z);
            //m_cornersIdx++;
            //path = m_navHelper.GetPathByTarget(v);
            //m_cornersIdx = 1;
        }
        else if (m_cornersIdx == path.corners.Length - 1 && step + 0.2f + m_stopDistance > dis)
        {
            Mogo.Util.LoggerHelper.Debug("StopNav");
            float tempDis = 10;

            for (int i = 0; i < 10 && tempDis > 0.1; i++, dis = Vector3.Distance(transform.position, path.corners[m_cornersIdx]))
            {
                transform.LookAt(new Vector3(targetToMoveTo.x, transform.position.y, targetToMoveTo.z));
                tempDis = dis - m_stopDistance;
                tempDis = tempDis > 0 ? tempDis : 0;
                collisionFlags = characterController.Move((path.corners[m_cornersIdx] - transform.position).normalized * tempDis);
            }
            StopNav();
            if (tempDis < 0.3)
            {
                EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
            }

        }
        return true;
    }

    public override void MoveTo(Vector3 v, bool needToAdjustPosY = true)
    {
        //Debug.LogError("MoveTo:" + v);
        if (path != null && m_cornersIdx < path.corners.Length)
            //Mogo.Util.LoggerHelper.Error("path.corners[m_cornersIdx]:" + path.corners[m_cornersIdx]);
            if (!canMove) return;
        if (m_isMovingOn && !needToAdjustPosY) return;
        else
        {
            StopNav();
        }

        if (needToAdjustPosY)
        {
            bool hasHit = MogoUtils.GetPointInTerrain(v.x, v.z, out v);

            if (!hasHit)
            {
                LoggerHelper.Error("there is no Hit!目测没把场景碰撞层设为Terrain:" + v + " at " + MogoWorld.thePlayer.sceneId+" 求美术看看");
                return;
            }
        }
        //Mogo.Util.LoggerHelper.Debug("v:" + v);
        //计算路线
        if (!isMovingToTarget)//|| (targetToMoveTo - v).magnitude < 0.05f
        {
            //Mogo.Util.LoggerHelper.Debug("IsSideCrash!");
            path = m_navHelper.GetPathByTarget(v);
            //Mogo.Util.LoggerHelper.Debug("path:" + path.corners.Length);
            targetToMoveTo = v;
            m_cornersIdx = 1;
        }

        //Mogo.Util.LoggerHelper.Debug("path.corners.Length:" + path.corners.Length);
        if (path.corners.Length < 2)
        {
            //Mogo.Util.LoggerHelper.Error("path.corners.Length < 2");
            StopNav();
            MogoWorld.thePlayer.Idle();
            //Debug.LogError("path.corners.Length < 2");
            //EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
            return;
        }

        isMovingToTarget = true;
        moveDirection = (path.corners[m_cornersIdx] - transform.position).normalized;
        //Mogo.Util.LoggerHelper.Error("MoveTo:" + path.corners[m_cornersIdx]);
        //RotateTo(path.corners[m_cornersIdx]);
        transform.LookAt(new Vector3(path.corners[m_cornersIdx].x, transform.position.y, path.corners[m_cornersIdx].z));
        float dis = Vector3.Distance(transform.position, path.corners[m_cornersIdx]);
        float step = 8 * Time.deltaTime;
        //Mogo.Util.LoggerHelper.Debug("dis:" + dis);
        //Mogo.Util.LoggerHelper.Debug("step:" + step);
        if (step + 0.1f > dis && m_cornersIdx < path.corners.Length - 1)
        {
            //collisionFlags = characterController.Move(transform.forward * dis);
            //Mogo.Util.LoggerHelper.Debug("m_isMovingOn = true;");
            //StopNav();
            //m_isMovingOn = true;

            //m_timerIdForNav = TimerHeap.AddTimer(900, 0, () =>
            // {
            //     m_isMovingOn = false;
            //     MoveTo(targetToMoveTo, false);

            // });
            //return;
            collisionFlags = characterController.Move(transform.forward * dis);

            //transform.position = new Vector3(path.corners[m_cornersIdx].x, transform.position.y, path.corners[m_cornersIdx].z);
            m_cornersIdx++;
            transform.LookAt(new Vector3(path.corners[m_cornersIdx].x, transform.position.y, path.corners[m_cornersIdx].z));
            //path = m_navHelper.GetPathByTarget(v);
            //m_cornersIdx = 1;
        }
        else if (m_cornersIdx == path.corners.Length - 1 && step + 0.1f + m_stopDistance > dis)
        {
            ////Debug.LogError("StopNav");
            //float tempDis = 10;
            //for (int i = 0; i < 5 && tempDis > 0.1; i++, dis = Vector3.Distance(transform.position, targetToMoveTo))
            //{
            //    tempDis = dis - m_stopDistance;
            //    tempDis = tempDis > 0 ? tempDis : 0;
            //    collisionFlags = characterController.Move((targetToMoveTo - transform.position).normalized * tempDis);
            //}
            //StopNav();

            //if (tempDis > 0.3)
            //{
            //    MogoWorld.thePlayer.Idle();
            //    EventDispatcher.TriggerEvent(ON_MOVE_TO_FALSE, transform.gameObject, targetToMoveTo, tempDis);
            //}

            //EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);


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
                MogoWorld.thePlayer.Idle();
                EventDispatcher.TriggerEvent(ON_MOVE_TO_FALSE, transform.gameObject, targetToMoveTo, tempDis);
            }
            else
            {
                EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
            }
        }
        //}
    }

    public override void TeleportTo(Vector3 destination)
    {
        Vector3 movement = destination - transform.position;
        if (MogoWorld.inCity)
        {
            transform.Translate(movement, Space.World);
        }
        else
        {
            collisionFlags = characterController.Move(movement);
        }
    }

    public void MoveToWithoutNav(Vector3 v)
    {
        //Debug.LogError("move to:" + v);
        if (!canMove)
        {
            //Debug.LogError("!canmove");
            return;
        }
        bool hasHit = MogoUtils.GetPointInTerrain(v.x, v.z, out v);

        if (!hasHit)
        {
            //Debug.LogError("!hasHit");
            return;
        }
        isMovingToTarget = false;

        targetToMoveTo = v;
        isMovingToTargetWithoutNav = true;
        //transform.LookAt(new Vector3(targetToMoveTo.x, transform.position.y, targetToMoveTo.z));

        Vector3 direction = targetToMoveTo - transform.position;
        int i = direction.x > 0 ? 1 : -1;
        float targetAngleY = i * Vector2.Angle(new Vector2(0, 1), new Vector2(direction.x, direction.z));// +Camera.main.transform.eulerAngles.y;
        base.ApplyRotation(targetAngleY);
        //transform.LookAt(new Vector3(targetToMoveTo.x, transform.position.y, targetToMoveTo.z));
        float dis = Vector3.Distance(transform.position, targetToMoveTo);
        //if (dis < 8 * speed * turnAroundTime * 2 + 1 && isTurning)//
        //{
        //    targetSpeed = 0;
        //}
        //else
        //{
        //    targetSpeed = moveSpeed;
        //}


        //direction = direction.normalized;
        //到达目的地
        float step = 30 * Time.deltaTime;
        //Debug.LogError("dis:" + dis);
        //Debug.LogError("step + 0.1f:" + (step + 0.1f));
        if (dis < step + 0.1f)
        {
            //Debug.LogError("StopMOve");
            StopMove();
        }
        else
        {
            // characterController.Move(direction * step);
            //transform.Translate(direction * step, Space.World);
        }
    }

    public override void StopMove()
    {
        speed = 0;

        characterController.Move(targetToMoveTo - transform.position);

        isMovingToTargetWithoutNav = false;
        EventDispatcher.TriggerEvent(ON_MOVE_TO, transform.gameObject, targetToMoveTo);
    }

    public override void SetStopDistance(float distance)
    {
        m_stopDistance = distance;
    }

    private void ApplyRotation()
    {
        if (Camera.main == null || MogoWorld.thePlayer == null)
        {
            return;
        }
        int i = ControlStick.instance.direction.x > 0 ? 1 : -1;
        float targetAngleY = i * Vector2.Angle(new Vector2(0, -1), ControlStick.instance.direction) + Camera.main.transform.eulerAngles.y;
        if (MogoWorld.thePlayer.direction) targetAngleY += 180;
        base.ApplyRotation(targetAngleY);
    }

    private void Move()
    {
        //if (moveByProgram && ControlStick.isDraging)
        //{
        //    int i = ControlStick.direction.x > 0 ? 1 : -1;
        //    float targetAngleY = i * Vector2.Angle(new Vector2(0, -1), ControlStick.direction) + Camera.main.transform.eulerAngles.y;
        //    Vector3 original = transform.eulerAngles;
        //    transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetAngleY, transform.eulerAngles.z);
        //    horizontalMovement = speed * transform.forward;
        //    transform.eulerAngles = original;
        //}

        collisionFlags = characterController.Move((extraSpeed * moveDirection + new Vector3(0, verticalSpeed, 0)) * Time.deltaTime);


    }

    public bool CanMoveTo(Vector3 position)
    {
        Vector3 save = transform.position;
        Vector3 movement = position - save;
        //要过滤怪物的碰撞
        Physics.IgnoreLayerCollision(8, 11, true);
        characterController.Move(movement);


        Vector3 d = transform.position - position;
        transform.position = save;
        Physics.IgnoreLayerCollision(8, 11, false);

        if (d.magnitude > 0.3f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void ApplyGravity()
    {
        if (IsGrounded())
        {
            if (isFlying)
            {
                if (Time.time - time < 0.2f) return;
                SetIfFlying(false);
                verticalSpeed = 0.0f;
                //Debug.LogError("Events.GearEvent.MotorHandleEnd");
                EventDispatcher.TriggerEvent(Events.GearEvent.MotorHandleEnd, this as MonoBehaviour);
            }
            else
            {
                verticalSpeed = 0.0f;
            }
        }
        else
            verticalSpeed -= gravity * Time.deltaTime;
    }

    float time = 0;

    public override void SetIfFlying(bool b)
    {
        base.SetIfFlying(b);
        //Debug.LogError("SetIfFlying:" + b);
        if (b)
        {

            time = Time.time;
            enableStick = false;
            enableRotation = false;
            animator.applyRootMotion = false;
        }
        else
        {
            extraSpeed = 0f;
            enableRotation = true;
            animator.applyRootMotion = true;
            enableStick = true;
        }

    }


}