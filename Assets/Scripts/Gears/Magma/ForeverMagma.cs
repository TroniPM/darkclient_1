using UnityEngine;
using System.Collections;

using Mogo.Util;

public class ForeverMagma : GearParent
{
    public int createMagmaTime;
    public float radius;
    public int creationProbability;
    public int nextMagmaRemainTime;

    protected bool enable;
    protected uint createMagmaTimer;

    protected MagmaGear gear;

    void Start()
    {
        gearType = "ForeverMagma";
        enable = true;

        createMagmaTimer = uint.MaxValue;

        if (gameObject.GetComponent<Collider>() == null)
        {
            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(2, 2, 2);
        }

        gear = GetComponent<MagmaGear>();

        TryCreateNewMagma();
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(createMagmaTimer);
    }

    protected virtual void TryCreateNewMagma()
    {
        uint nextCreateMagmaTime = (uint)RandomHelper.GetRandomInt((int)(0.5 * createMagmaTime), (int)(1.5 * createMagmaTime));

        createMagmaTimer = TimerHeap.AddTimer((uint)nextCreateMagmaTime, 0, CreateNewMagma);
    }

    protected virtual void CreateNewMagma()
    {
        SubAssetCacheMgr.GetGearInstance("fx_scene_magma_98.prefab", (newName, newID, newMagma) =>
        {
            var magmaGo = newMagma as GameObject;
            if (magmaGo == null)
                return;

            magmaGo.layer = 17;

            var magma = magmaGo.AddComponent<Magma>();
            magma.createMagmaTime = createMagmaTime;
            magma.radius = radius;
            magma.creationProbability = (int)(creationProbability);
            magma.remainTime = nextMagmaRemainTime;
            magma.nextMagmaRemainTime = RandomHelper.GetRandomInt((int)(0.5 * nextMagmaRemainTime), (int)(1.5 * nextMagmaRemainTime));

            var boxCollider = magmaGo.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(2, 2, 2);

            if (gear != null)
            {
                var magmaGear = magmaGo.AddComponent<MagmaGear>();
                magmaGear.burnTime = gear.burnTime;

                magmaGear.magmaDamagePercentage = gear.magmaDamagePercentage;
                magmaGear.magmaDamageMin = gear.magmaDamageMin;
                magmaGear.magmaDamageMax = gear.magmaDamageMax;
                magmaGear.magmaDamageInterval = gear.magmaDamageInterval;

                magmaGear.magmaCanAttackDummy = gear.magmaCanAttackDummy;

                magmaGear.onFireDamagePercentage = gear.onFireDamagePercentage;
                magmaGear.onFireDamageMin = gear.onFireDamageMin;
                magmaGear.onFireDamageMax = gear.onFireDamageMax;
                magmaGear.onFireDamageInterval = gear.onFireDamageInterval;

                magmaGear.onFireCanAttackDummy = gear.onFireCanAttackDummy;

                var container = magmaGo.AddComponent<MagmaInvisibleContainer>();
                container.magmaGear = magmaGear;
                container.skillActionList = new int[1] { 20112 };
            }

            magmaGo.transform.position = RandomHelper.GetRandomVector3InRangeCircle(radius) + transform.position;

            createMagmaTimer = TimerHeap.AddTimer(0, 0, TryCreateNewMagma);
        });
    }
}
