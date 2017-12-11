using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.Game;

public class DropRock : GearParent
{
    public bool canAttackDummy = false;

    public float radius;
    public float percentage;
    public int damageMin;
    public int damageMax;

    protected SfxHandler sfxHandler { get; set; }
    protected uint timerID;

    void Start()
    {
        gearType = "DropRock";
        gameObject.AddComponent<Rigidbody>();
        triggleEnable = true;

        sfxHandler = gameObject.AddComponent<SfxHandler>();
        timerID = uint.MaxValue;
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(timerID);
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggleEnable)
        {
            if (other.gameObject.layer == 9)
            {
                MogoMainCamera.Instance.Shake(5, 0.1f);
                sfxHandler.HandleFx(500102);

                SetDamage(MogoUtils.GetEntitiesInRange(transform, radius));

                triggleEnable = false;

                if (timerID == uint.MaxValue)
                    timerID = TimerHeap.AddTimer(5000, 0, RockDestroy);
            }
        }
    }

    private void RockDestroy()
    {
        sfxHandler.RemoveFXs(500102);
        // sfxHandler.RemoveFXs(6011);
        Destroy(this.gameObject);
    }

    private void SetDamage(List<List<uint>> entities)
    {
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

    private int CalcDamage(EntityParent entity)
    {
        return (int)(entity.hp * percentage + RandomHelper.GetRandomInt(damageMin, damageMax));
    }
}
