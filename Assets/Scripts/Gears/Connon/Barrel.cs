using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class Barrel : GearParent
{
    public Connon connon;
    public float angleSpeed;

    protected Transform target { get; set; }
    protected bool isAiming;
    protected bool isRotating;
    protected SfxHandler sfxHandler { get; set; }

    protected bool direction { get; set; }

    void Start()
    {
        gearType = "Barrel";
        isAiming = false;
        isRotating = false;
        sfxHandler = transform.Find("SfxHanlder").gameObject.AddComponent<SfxHandler>();
    }

    void Update()
    {
        if (isAiming)
        {
            transform.LookAt(FixPositon(target.position));
        }
        else if (isRotating)
        {
            float deltaTime = Time.deltaTime; 
            float deltaAngle = Vector3.Angle(transform.forward, FixPositon(target.position) - transform.position);

            if (deltaAngle == 0)
            {
                isRotating = false;
                connon.SetFirePerson(MogoWorld.thePlayer.Transform.gameObject);
            }
            else
            {
                if (angleSpeed * deltaTime > deltaAngle)
                {
                    Mogo.Util.LoggerHelper.Debug(angleSpeed * deltaTime + "         " + deltaAngle);
                    isRotating = false;
                    connon.SetFirePerson(MogoWorld.thePlayer.Transform.gameObject);
                    // transform.LookAt(FixPositon(target.position));
                }
                else
                {
                    if (direction)
                        transform.Rotate(0, angleSpeed * deltaTime, 0);
                    else
                        transform.Rotate(0, -angleSpeed * deltaTime, 0);
                }
            }
        }
    }

    public void AimTarget(Transform go)
    {
        target = go.transform;
        // LoggerHelper.Debug(FixPositon(target.position));
        isAiming = true;
    }

    public void Rest()
    {
        isAiming = false;
    }

    protected Vector3 FixPositon(Vector3 srcPosition)
    {
        Vector3 desPosition = new Vector3(srcPosition.x, transform.position.y, srcPosition.z);
        return desPosition;
    }

    public void PlaySfx()
    {
        sfxHandler.HandleFx(500101);
    }

    public void SetRotate(Transform go)
    {
        isAiming = false;
        isRotating = true;
        target = go.transform;

        Vector2 vsrc = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 vdes = new Vector2(target.position.x - transform.forward.x, target.position.z - transform.forward.z);

        if (vsrc.x * vdes.y - vsrc.y * vsrc.x < 0)
            direction = true;
        else
            direction = false;
    }
}
