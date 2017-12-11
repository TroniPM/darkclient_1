using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ColliderControl : GearParent
{
    public enum ColliderType : byte
    {
        Cube = 0,
        Sphere = 1,
        Mesh = 2,
        Capsule = 3
    }

    public ColliderType type;
    protected Collider m_collider;

    void Start()
    {
        ID = (uint)defaultID;
        gearType = "ColliderControl";

        m_collider = gameObject.GetComponent<Collider>();

        if (m_collider == null)
        {
            switch (type)
            {
                case ColliderType.Sphere:
                    m_collider = gameObject.AddComponent<SphereCollider>();
                    SphereCollider tempSphereCollider = m_collider as SphereCollider;
                    tempSphereCollider.radius = transform.localScale.y;
                    break;
                case ColliderType.Mesh:
                    m_collider = gameObject.AddComponent<MeshCollider>();
                    break;
                case ColliderType.Capsule:
                    m_collider = gameObject.AddComponent<CapsuleCollider>();
                    CapsuleCollider tempCapsuleCollider = m_collider as CapsuleCollider;
                    tempCapsuleCollider.radius = transform.localScale.x > transform.localScale.z ? transform.localScale.z : transform.localScale.x;
                    tempCapsuleCollider.height = transform.localScale.y;
                    break;
                default:
                    m_collider = gameObject.AddComponent<BoxCollider>();
                    // BoxCollider tempBoxCollider = collider as BoxCollider;
                    // tempBoxCollider.size = transform.localScale;
                    break;
            }
        }

        FlushCollider();

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    protected override void SetGearEventEnable(uint enableID)
    {
        base.SetGearEventEnable(enableID);
        if (enableID == ID)
            FlushCollider();
    }

    protected override void SetGearEventDisable(uint disableID)
    {
        base.SetGearEventDisable(disableID);
        if (disableID == ID)
            FlushCollider();
    }

    protected override void SetGearEventStateOne(uint stateOneID)
    {
        base.SetGearEventStateOne(stateOneID);
        if (stateOneID == ID)
            FlushCollider();
    }

    protected override void SetGearEventStateTwo(uint stateTwoID)
    {
        base.SetGearEventStateTwo(stateTwoID);
        if (stateTwoID == ID)
            FlushCollider();
    }

    protected override void SetGearEnable(uint enableID)
    {
        base.SetGearEnable(enableID);
        if (enableID == ID)
            FlushCollider();
    }

    protected override void SetGearDisable(uint disableID)
    {
        base.SetGearDisable(disableID);
        if (disableID == ID)
            FlushCollider();
    }

    protected override void SetGearStateOne(uint stateOneID)
    {
        base.SetGearStateOne(stateOneID);
        if (stateOneID == ID)
            FlushCollider();
    }

    protected override void SetGearStateTwo(uint stateTwoID)
    {
        base.SetGearStateTwo(stateTwoID);
        if (stateTwoID == ID)
            FlushCollider();
    }

    protected void FlushCollider()
    {
        m_collider.isTrigger = triggleEnable;
        m_collider.enabled = stateOne;
    }
}

