using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.GameData;
using Mogo.Game;

public class MeteoriteBomb : GearParent
{
    public float speed;
    public float rotationSpeed;

    public int shakeCamAnimID;
    public float shakeCamAnimTime;

    public int deleteTime;

    public Transform target;

    protected MogoSimpleMotor motor { get; set; }
    protected SfxHandler handler { get; set; }

    protected Vector3 originPosition { get; set; }
    protected Transform hitPosition { get; set; }

    protected uint deleteTimer { get; set; }

    public bool canAttackDummy = false;

    public float radius;
    public float percentage;
    public int damageMin;
    public int damageMax;

    void Start()
    {
        gearType = "Meteorite";

        ID = (uint)defaultID;
        triggleEnable = true;
        stateOne = false;

        motor = gameObject.AddComponent<MogoSimpleMotor>();
        handler = gameObject.AddComponent<SfxHandler>();

        deleteTimer = uint.MaxValue;

        transform.LookAt(MogoWorld.thePlayer.Transform);

        CalculateHitPosition();

        handler.HandleFx(6011, null, (go, dbid) =>
        {
            go.transform.position = target.position;
        });

        EventDispatcher.AddEventListener<MonoBehaviour>(Events.GearEvent.MotorHandleEnd, MoveEnd);

        EventDispatcher.AddEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, DeleteBomb);
    }

    void OnDestroy()
    {
        EventDispatcher.RemoveEventListener<MonoBehaviour>(Events.GearEvent.MotorHandleEnd, MoveEnd);
        EventDispatcher.RemoveEventListener<int, bool>(Events.InstanceEvent.InstanceUnLoaded, DeleteBomb);

        DeleteBomb();
        TimerHeap.DelTimer(deleteTimer);
    }

    public void CalculateHitPosition()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(originPosition, transform.forward, out hitInfo, 1000, 1 << 9))
        {
            GameObject temp = new GameObject();
            temp.transform.position = hitInfo.point;
            temp.name = gameObject.name + "_hitPosition";
            hitPosition = temp.transform;
        }
        else
        {
            hitPosition = transform;
        }

        BeginDrop();
    }

    public void BeginDrop()
    {
        motor.SetSpeed(speed);
        motor.SetRotateSpeed(0, 0, rotationSpeed);

        transform.LookAt(target.position);

        motor.MoveTo(target.position);
    }

    public void HitGround()
    {
        Mogo.Util.LoggerHelper.Debug("HitGround");

        motor.StopRotate();

        handler.HandleFx(500102);
        handler.RemoveFXs(6011);
        handler.HandleFx(6004, hitPosition, (go, i) =>
        {
            go.transform.parent = hitPosition;
            go.transform.localPosition = Vector3.zero;
        });

        transform.Find("fx_fbx_aerolite02/fx_fbx_aerolite_tail").gameObject.SetActive(false);

        MogoMainCamera.Instance.Shake(shakeCamAnimID, shakeCamAnimTime);

        deleteTimer = TimerHeap.AddTimer((uint)deleteTime, 0, () =>
        {
            DeleteBomb();
        });
    }

    public void HitPlayer()
    {
        Mogo.Util.LoggerHelper.Debug("HitPlayer");

        motor.StopRotate();

        handler.HandleFx(500102);
        handler.RemoveFXs(6011);
        handler.HandleFx(6004, hitPosition, (go, i) =>
        {
            go.transform.parent = hitPosition;
            go.transform.localPosition = Vector3.zero;
        });

        transform.Find("fx_fbx_aerolite02/fx_fbx_aerolite_tail").gameObject.SetActive(false);

        MogoMainCamera.Instance.Shake(shakeCamAnimID, shakeCamAnimTime);

        deleteTimer = TimerHeap.AddTimer((uint)deleteTime, 0, () =>
        {
            DeleteBomb();
        });
    }

    #region 碰撞触发

    void OnTriggerEnter(Collider other)
    {
        if (triggleEnable)
        {
            try
            {
                if (other.gameObject.layer == 9)
                {
                    triggleEnable = false;
                    HitGround();
                    base.SetGearEventStateOne(ID);

                    List<List<uint>> entities = MogoUtils.GetEntitiesInRange(transform, radius);

                    if (entities.Count != 4)
                        return;

                    List<uint> dummyList = entities[0];
                    List<uint> playerList = entities[2];

                    if (canAttackDummy)
                    {
                        foreach (uint id in dummyList)
                        {
                            EventDispatcher.TriggerEvent(Events.GearEvent.Damage, id, 9003, (int)2, CalcDamage(MogoWorld.GetEntity(id) as EntityParent));
                        }
                    }

                    foreach (uint id in playerList)
                    {
                        if (id == MogoWorld.thePlayer.ID)
                        {
                            EventDispatcher.TriggerEvent(Events.GearEvent.Damage, id, 9003, (int)2, CalcDamage(MogoWorld.thePlayer as EntityParent));
                            break;
                        }
                    }
                }
            }
            catch
            {
                Debug.LogError("Undefined tag gameobject name: " + other.gameObject.name);
            }
        }
    }

    #endregion

    protected int CalcDamage(EntityParent entity)
    {
        return (int)(entity.hp * percentage + RandomHelper.GetRandomInt(damageMin, damageMax));
    }

    protected void MoveEnd(MonoBehaviour script)
    {
        if (triggleEnable && script is MogoSimpleMotor)
        {
            if ((script as MogoSimpleMotor) == motor)
            {
                triggleEnable = false;
                HitGround();
                base.SetGearEventStateOne(ID);

                List<List<uint>> entities = MogoUtils.GetEntitiesInRange(transform, radius);

                if (entities.Count != 4)
                    return;

                List<uint> dummyList = entities[0];
                List<uint> playerList = entities[0];

                if (canAttackDummy)
                {
                    foreach (uint id in dummyList)
                    {
                        EventDispatcher.TriggerEvent(Events.GearEvent.Damage, id, 9003, (int)2, CalcDamage(MogoWorld.GetEntity(id) as EntityParent));
                    }
                }

                foreach (uint id in playerList)
                {
                    if (id == MogoWorld.thePlayer.ID)
                    {
                        EventDispatcher.TriggerEvent(Events.GearEvent.Damage, id, 9003, (int)2, CalcDamage(MogoWorld.thePlayer as EntityParent));
                        break;
                    }
                }
            }
        }
    }

    protected void DeleteBomb(int missionID = 0, bool Instance = false)
    {
        handler.RemoveFXs(500102);
        handler.RemoveFXs(6011);
        handler.RemoveFXs(6004);

        if (this && gameObject)
        {
            AssetCacheMgr.SynReleaseInstance(this.gameObject);
        }

        if (hitPosition != null)
            Destroy(hitPosition.gameObject);
    }
}



