using UnityEngine;
using System.Collections;

using Mogo.Util;

public class Magma : GearParent
{
    public int createMagmaTime;
    public float radius;
    public int creationProbability;
    public int nextMagmaRemainTime;

    public int remainTime;

    protected uint createMagmaTimer;
    protected uint disbaleMagmaTimer;

    protected MagmaGear gear;

    void Start()
    {
        gearType = "Magma";
        triggleEnable = true;

        createMagmaTimer = uint.MaxValue;
        disbaleMagmaTimer = uint.MaxValue;

        disbaleMagmaTimer = TimerHeap.AddTimer((uint)nextMagmaRemainTime, 0, SetDisable);

        gear = GetComponent<MagmaGear>();

        TryCreateNewMagma();
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(createMagmaTimer);
        TimerHeap.DelTimer(disbaleMagmaTimer);

        if (this && gameObject)
            AssetCacheMgr.SynReleaseInstance(this.gameObject);
    }


    protected void TryCreateNewMagma()
    {
        if (triggleEnable)
        {
            int probability = RandomHelper.GetRandomInt(0, 100);
            if (probability < creationProbability)
            {
                return;
            }
            createMagmaTimer = TimerHeap.AddTimer((uint)createMagmaTime, 0, CreateNewMagma);
        }
    }

    protected void CreateNewMagma()
    {
        if (triggleEnable)
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

                    magmaGear.magmaCanAttackDummy = gear.magmaCanAttackDummy;

                    magmaGear.magmaDamagePercentage = gear.magmaDamagePercentage;
                    magmaGear.magmaDamageMin = gear.magmaDamageMin;
                    magmaGear.magmaDamageMax = gear.magmaDamageMax;
                    magmaGear.magmaDamageInterval = gear.magmaDamageInterval;

                    magmaGear.onFireCanAttackDummy = gear.onFireCanAttackDummy;

                    magmaGear.onFireDamagePercentage = gear.onFireDamagePercentage;
                    magmaGear.onFireDamageMin = gear.onFireDamageMin;
                    magmaGear.onFireDamageMax = gear.onFireDamageMax;
                    magmaGear.onFireDamageInterval = gear.onFireDamageInterval;

                    var container = magmaGo.AddComponent<MagmaInvisibleContainer>();
                    container.magmaGear = magmaGear;
                    container.skillActionList = new int[1] { 20112 };
                }

                magmaGo.transform.position = RandomHelper.GetRandomVector3InRangeCircle(radius) + transform.position;

               createMagmaTimer = TimerHeap.AddTimer(0, 0, TryCreateNewMagma);
            });
        }
    }

    protected void SetEnable()
    {
        triggleEnable = true;
    }

    protected void SetDisable()
    {
        triggleEnable = false;

        // 1.5个时间后所有创生完毕，2个时间确保没问题
        disbaleMagmaTimer = TimerHeap.AddTimer((uint)(2 * nextMagmaRemainTime), 0, DestoryMagma);
    }

    protected void DestoryMagma()
    {
        if (this && gameObject)
            AssetCacheMgr.SynReleaseInstance(this.gameObject);
    }

    public void MagmaBurnUp()
    {
        TimerHeap.DelTimer(disbaleMagmaTimer);
        DestoryMagma();
    }
}
