using UnityEngine;
using System;
using System.Collections.Generic;

using Mogo.FSM;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
using Mogo.Task;

namespace Mogo.Game
{
    public partial class EntityParent
    {
        public float tmpTime = 0;
        virtual public void TurnTo(float x, float y, float z)
        {
            if (currentMotionState == MotionState.DEAD) return;
            //Mogo.Util.LoggerHelper.Debug("turnTo:" + y);
            //Transform.localEulerAngles = new Vector3(x, y, z);
            if (motor)
                motor.RotateTo(y);
        }

        virtual public void MoveTo(float x, float z, float dx, float dy, float dz)
        {
            if (!Transform)
                return;
            if (currentMotionState == MotionState.DEAD) return;
            if (Mathf.Abs(x - Transform.position.x) < 0.1f && Mathf.Abs(z - Transform.position.z) < 0.1f)
            {
                TurnTo(dx, dy, dz);
            }
            else
            {
                MoveTo(x, z);
            }
        }

        virtual public void MoveToByAngle(float angleY, float _time, bool isNeedRotation)
        {
            if (currentMotionState == MotionState.DEAD) return;
            this.ChangeMotionState(MotionState.WALKING);
            if (motor)
            {

                motor.isNeedRotation = isNeedRotation;

                //Mogo.Util.LoggerHelper.Debug("MoveToByAngle:                                     " + (Time.time-tmpTime) );
                tmpTime = Time.time;
                motor.MoveToByAngle(angleY, _time);

            }
        }

        virtual public void MoveTo(float x, float z)
        {
            if (currentMotionState == MotionState.DEAD) return;
            MoveTo(x, 0, z);
        }

        virtual public bool MoveToByNav(Vector3 v, int stopDistance)
        {
            if (currentMotionState == MotionState.DEAD) return false;
            if (motor == null || Mathf.Abs(v.x - Transform.position.x) < 0.1f && Mathf.Abs(v.z - Transform.position.z) < 0.1f)
            {
                return false;
            }
            if (motor.MoveToByNav(v, stopDistance / 100f))
            {
                Move();
                return true;
            }
            else
                return false;
        }

        virtual public void MoveTo(float x, float y, float z)
        {
            if (currentMotionState == MotionState.DEAD) return;
            if (motor == null || Mathf.Abs(x - Transform.position.x) < 0.1f && Mathf.Abs(z - Transform.position.z) < 0.1f)
            {
                return;
            }
            Vector3 v = new Vector3(x, y, z);
            if (!(this is EntityMyself) && this is EntityPlayer)
            {
                if (motor.MoveToByNav(v))
                {
                    //Debug.LogWarning("motor.MoveToByNav:" + v);
                    Move();
                }
                else
                {
                    //Debug.LogError("can not find the way!");
                }
            }
            else
            {
                //Mogo.Util.LoggerHelper.Debug("MoveTo in entityParent:" + v);
                if (this is EntityMyself)
                {
                    if (Mathf.Abs(x - Transform.position.x) < 0.3f && Mathf.Abs(z - Transform.position.z) < 0.3f)
                    {
                        return;
                    }
                }
                motor.MoveTo(v);
                Move();
            }

            ////标记怪物路点，临时代码
            /*
            if (Application.platform == RuntimePlatform.WindowsEditor)
                AssetCacheMgr.GetUIInstance("flag.prefab", 5000, (prafab, id, go) =>
                {
                    //Vector3 point;
                    //if (Mogo.Util.MogoUtils.GetPointInTerrain(x, z, out point))
                    (go as GameObject).transform.position = v;
                });
                */
        }

        virtual public void OnMoveTo(GameObject arg1, Vector3 arg2)
        {
            if (arg1 == null) return;
            if (arg1.transform == Transform)
            {
                Idle();
            }
        }

        virtual protected void OnMoveToFalse(GameObject param1, Vector3 param2, float param3)
        {
            //ShowText(2, "OnMoveToFalse");
            //LoggerHelper.Debug("OnMoveToFalse");
            if (this == null) return;
            if (Transform == null) return;
            if (param1 == null) return;
            if (param1.transform == Transform)
            {
                Idle();
            }
        }

        virtual public void Move()
        {
            if ((this is EntityMyself) && (this as EntityMyself).deathFlag == 1)
            {
                return;
            }
            if (battleManger == null)
            {
                ChangeMotionState(MotionState.WALKING);
            }
            else
            {
                this.battleManger.Move();
            }
        }
    }
}
