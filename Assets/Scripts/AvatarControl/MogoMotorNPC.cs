using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;

public class MogoMotorNPC : MonoBehaviour
{
    public float rotateSpeed = 360f;

    private bool isTurningTo = false;
    private Transform turnToTarget;
    private Action turnEndAction;

    private bool direction = false;

    void Awake()
    {
        // EventDispatcher.AddEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, CheckStopTurnTo);
    }


    void Destroy()
    {
        // EventDispatcher.RemoveEventListener<GameObject, Vector3>(MogoMotor.ON_MOVE_TO, CheckStopTurnTo);
    }

    void Update()
    {
        if (turnToTarget)
        {
            float deltaTime = Time.deltaTime;
            float deltaAngle = Vector3.Angle(transform.forward, FixPositon(turnToTarget.position) - transform.position);

            if (deltaAngle == 0)
            {
                StopTurnTo();
            }
            else
            {
                if (rotateSpeed * deltaTime > deltaAngle)
                {
                    StopTurnTo();
                }
                else if (direction)
                {
                    transform.Rotate(0, -rotateSpeed * deltaTime, 0);
                }
                else
                {
                    transform.Rotate(0, rotateSpeed * deltaTime, 0);
                }
            }
        }
    }

    #region 转向

    public void TurnTo(Transform theTarget, Action action = null)
    {
        float deltaAngle = Vector3.Angle(transform.forward, FixPositon(theTarget.position) - transform.position);

        if (deltaAngle > 25 || deltaAngle < -25)
        {
            isTurningTo = true;
            turnToTarget = theTarget;
            turnEndAction = action;

            Vector2 vsrc = new Vector2(transform.forward.x, transform.forward.z);
            Vector2 vdes = new Vector2(theTarget.position.x - transform.forward.x, theTarget.position.z - transform.forward.z);

            if (vsrc.x * vdes.y - vsrc.y * vsrc.x < 0)
                direction = false;
            else
                direction = true;
        }
        else
        {
            action();
        }
    }

    private void CheckStopTurnTo(GameObject theEntityGameObject, Vector3 thePosition)
    {
        if (isTurningTo)
        {
            if (theEntityGameObject == MogoWorld.thePlayer.GameObject)
            {
                StopTurnTo();
            }
        }
    }

    private void StopTurnTo()
    {
        isTurningTo = false;
        transform.LookAt(FixPositon(turnToTarget.position));
        turnToTarget = null;

        if (turnEndAction != null)
            turnEndAction();
    }

    private Vector3 FixPositon(Vector3 srcPosition)
    {
        return new Vector3(srcPosition.x, transform.position.y, srcPosition.z);
    }

    #endregion
}

