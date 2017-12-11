// 模块名   :  MogoMotor
// 创建者   :  莫卓豪
// 创建日期 :  2012-3-20
// 描    述 :  Motor父类

using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class MogoMotor : MonoBehaviour
{
    public const string ON_MOVE_TO = "MogoMotor.ON_MOVE_TO";
    public const string ON_MOVE_TO_FALSE = "MogoMotor.ON_MOVE_TO_FALSE";

    protected Vector3 moveDirection = new Vector3();
    public float extraSpeed = 0;//附加速度
    protected bool isLookingAtTarget = false;
    protected Vector3 targetToLookAt;
    protected Transform targetToLookAtTransform;
    public float verticalSpeed = 0.0f;
    public Vector3 targetToDragTo;

    public float dragSpeed = 0f;
    public float speed = 0.0f;
    public float gravity = 20.0f;
    protected CollisionFlags collisionFlags;
    public float angularSpeedRate = 1.0f;
    private float acturalAngularSpeed = 1440f;
    private float angularSpeed = 1440f;
    protected float turnAroundTime = 0.5f;
    public float acceleration = 2;
    public float targetSpeed;
    public bool isMovable;
    public bool enableStick = false;
    public bool isMovingToTarget = false;
    public bool hasInited = false;
    public bool enableRotation = true;//是否启用摇杆控制人物面朝
    public Vector3 targetToMoveTo;
    protected bool isTurning = false;
    protected float moveSpeed = 0;
    protected bool isNaving = false;
    protected float m_stopDistance = 0f;

    public bool canMove = true;
    //设置目标方向
    public bool isRotatingTo = false;
    protected float rotationY = 0;

    protected bool isFlying = false;
    protected bool isDragTo = false;
    //public bool moveByProgram = false;
    public bool canTurn = true;
    public bool isNeedRotation = true;

    protected void AdjustPosition()
    {
        if (MogoWorld.isLoadingScene)
            return;

        if (transform.position.y < -100 && transform.position.y > -9000)
        {
            Vector3 temp;
            bool hasHit = MogoUtils.GetPointInTerrain(transform.position.x, transform.position.z, out temp);
            if (!hasHit)
            {
                if (MogoWorld.inCity)
                {
                    foreach (EntityParent e in MogoWorld.Entities.Values)
                    {
                        if (e != null && e.Transform && e is EntityPlayer)
                        {
                            hasHit = MogoUtils.GetPointInTerrain(e.Transform.position.x, e.Transform.position.z, out temp);
                            if (!hasHit) continue;
                            transform.position = e.Transform.position;
                            return;
                        }
                    }

                }
                else
                {
                    foreach (EntityParent e in MogoWorld.Entities.Values)
                    {
                        if (e != null && e.Transform && (e is EntityDummy || e is EntityMonster || e is EntityMercenary))
                        {
                            hasHit = MogoUtils.GetPointInTerrain(e.Transform.position.x, e.Transform.position.z, out temp);
                            if (!hasHit) continue;
                            transform.position = e.Transform.position;
                            return;
                        }
                    }
                }
                float x = MapData.dataMap[MogoWorld.thePlayer.sceneId].enterX;
                float z = MapData.dataMap[MogoWorld.thePlayer.sceneId].enterY;
                MogoUtils.GetPointInTerrain(transform.position.x, transform.position.z, out temp);
                transform.position = temp;
            }
            else
            {
                transform.position = temp;
            }
        }
    }

    virtual public void SetIfFlying(bool b)
    {
        isFlying = b;
    }

    public void SetAngularSpeed(float speed)
    {
        angularSpeed = speed;
        acturalAngularSpeed = angularSpeed * angularSpeedRate;
        turnAroundTime = acturalAngularSpeed / 360f;
    }

    public void HandleAiRateChange(float aiRate)
    {
        angularSpeedRate = aiRate;
        acturalAngularSpeed = angularSpeed * aiRate;
    }

    public void SetExrtaSpeed(float _extraSpeed)
    {
        extraSpeed = _extraSpeed;
    }

    public void SetTargetToLookAt(Transform t)
    {
        isNeedRotation = true;
        targetToLookAtTransform = t;
        isLookingAtTarget = true;
    }

    public void SetTargetToLookAt(Vector3 _targetToLookAt)
    {
        targetToLookAt = _targetToLookAt;
        targetToLookAtTransform = null;
        isLookingAtTarget = true;
        
    }

    public void CancleLookAtTarget()
    {
        isLookingAtTarget = false;
    }

    public bool IsLookingAtTarget()
    {
        return isLookingAtTarget;
    }

    public void SetMoveDirection(Vector3 _direction, Space space = Space.World)
    {
        _direction = _direction.normalized;

        switch (space)
        {
            case Space.Self:
                this.moveDirection = transform.TransformDirection(_direction);
                break;
            case Space.World:
                this.moveDirection = _direction;
                break;
        }
    }
    virtual public void TurnToControlStickDir()
    {
    }

    public virtual void SetSpeed(float _speed)
    {
        this.speed = _speed;
    }

    public virtual void MoveToByAngle(float angleY, float _time)
    {
    }

    public virtual void StopNav()
    {

    }

    virtual public void StopMove()
    {

    }

    virtual public bool MoveToByNav(Vector3 v, float stopDistance = 0f, bool needToAdjustPosY = true)
    {
        return false;
    }

    public virtual void MoveTo(Vector3 v, bool needToAdjustPosY = true)
    {
    }


    public virtual void SetStopDistance(float distance)
    {
    }

    protected float AccelerateSpeed(float originalSpeed, float _targetSpeed)
    {
        if (Mathf.Abs(originalSpeed - _targetSpeed) < acceleration * Time.deltaTime)
        {
            originalSpeed = _targetSpeed;
        }
        else if (originalSpeed - _targetSpeed > 0)
        {
            originalSpeed -= acceleration * Time.deltaTime;
        }
        else
        {
            originalSpeed += acceleration * Time.deltaTime;
        }
        return originalSpeed;
    }

    public void RotateTo(float targetAngleY)
    {
        if (!canTurn) return;
        isRotatingTo = true;
        rotationY = targetAngleY;
    }

    protected void ApplyRotation(float targetAngleY)
    {
        float m = targetAngleY % 360;
        float n = transform.eulerAngles.y % 360;
        if (m < 0) m += 360;
        if (n < 0) n += 360;
        if (m - n < -180) m += 360;
        float dY = (m - n);

        float angularStep = Time.deltaTime * acturalAngularSpeed;


        if (Mathf.Abs(dY) < angularStep)
        {
            transform.eulerAngles = new Vector3(0, targetAngleY, 0);
            isTurning = false;
            isRotatingTo = false;
        }
        else
        {
            int j = (dY > 0 && dY < 180) ? 1 : -1;
            transform.Rotate(new Vector3(0, j * angularStep, 0), Space.Self);
            isTurning = true;
        }
    }

    protected bool IsGrounded()
    {
        return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
    }
    protected bool IsSideCrash()
    {
        return (collisionFlags & CollisionFlags.CollidedSides) != 0;
    }

    virtual public void TeleportTo(Vector3 destination)
    {

        transform.position = destination;
    }

    virtual public void DragTo(Vector3 destination, float speed = 100f)
    {
        isDragTo = true;
        targetToDragTo = destination;
        dragSpeed = speed;

        Vector3 dir = destination - transform.position;
        float dis = dir.magnitude;
        dir = dir.normalized;
        float step = speed * Time.deltaTime;
        if (step > dis)
        {
            transform.position = destination;
            isDragTo = false;
        }
        else
        {
            transform.Translate(dir * step, Space.World);
        }
    }

    public void RotateTo(Vector3 target)
    {
        if (!canTurn) return;
        Vector3 dir = target - transform.position;
        int i = dir.x > 0 ? 1 : -1;
        float targetAngleY = i * Vector2.Angle(new Vector2(0, 1), new Vector2(dir.x, dir.z));
        RotateTo(targetAngleY);
    }
}