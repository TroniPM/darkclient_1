using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.Game;

public class MagmaGear : GearParent
{
    public bool magmaCanAttackDummy = false;
    public float magmaDamagePercentage;
    public int magmaDamageMin;
    public int magmaDamageMax;
    public int magmaDamageInterval;

    public bool onFireCanAttackDummy = false;
    public float onFireDamagePercentage;
    public int onFireDamageMin;
    public int onFireDamageMax;
    public int onFireDamageInterval;

    public int burnTime;
    protected bool isBurning;

    protected uint magmaDamageTimer;
    protected uint onFireDamageTimer;
    protected uint swicthTimer;
    protected uint burnTimer;

    protected SfxHandler sfxHandler;

    void Start()
    {
        gearType = "MagmaGear";

        triggleEnable = true;

        isBurning = false;

        magmaDamageTimer = uint.MaxValue;
        onFireDamageTimer = uint.MaxValue;

        sfxHandler = gameObject.AddComponent<SfxHandler>();

        AddListeners();
    }

    void OnDestroy()
    {
        TimerHeap.DelTimer(magmaDamageTimer);
        TimerHeap.DelTimer(onFireDamageTimer);

        TimerHeap.DelTimer(swicthTimer);
        TimerHeap.DelTimer(burnTimer);

        RemoveListeners();
    }


    public override void AddListeners()
    {
        base.AddListeners();
        EventDispatcher.AddEventListener(Events.GearEvent.CongealMagma, CongealMagma);
    }

    public override void RemoveListeners()
    {
        base.RemoveListeners();
        EventDispatcher.AddEventListener(Events.GearEvent.CongealMagma, CongealMagma);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (isBurning)
                {
                    onFireDamageTimer = TimerHeap.AddTimer((uint)0, 0, SetMagmaOnFireDamage);
                }
                else
                {
                    magmaDamageTimer = TimerHeap.AddTimer((uint)0, 0, SetMagmaDamage);
                }
            }
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            TimerHeap.DelTimer(magmaDamageTimer);
            TimerHeap.DelTimer(onFireDamageTimer);
            magmaDamageTimer = uint.MaxValue;
            onFireDamageTimer = uint.MaxValue;
        }
    }


    void OnTriggerStay(Collider other)
    {
    }


    protected void CongealMagma()
    {
        TimerHeap.DelTimer(magmaDamageTimer);
        TimerHeap.DelTimer(onFireDamageTimer);

        TimerHeap.DelTimer(swicthTimer);
        TimerHeap.DelTimer(burnTimer);

        triggleEnable = false;
    }


    public void SetFire()
    {
        if (isBurning)
            return;

        sfxHandler.HandleFx(1102402);

        swicthTimer = TimerHeap.AddTimer(800, 0, () => 
        { 
            TimerHeap.DelTimer(magmaDamageTimer);
            if (magmaDamageTimer != uint.MaxValue)
                onFireDamageTimer = TimerHeap.AddTimer((uint)0, 0, SetMagmaOnFireDamage);
            magmaDamageTimer = uint.MaxValue;
            isBurning = true;
        });

        burnTimer = TimerHeap.AddTimer((uint)RandomHelper.GetRandomInt((int)(burnTime * 0.8), (int)(burnTime * 1.2)), 0, BurnUp);
    }

    protected void BurnUp()
    {
        sfxHandler.RemoveFXs(1102402);
        isBurning = false;

        if (this && gameObject)
        {
            var magma = gameObject.GetComponent<Magma>();
            if (magma != null)
                magma.MagmaBurnUp();
        }
    }

    private void SetMagmaDamage()
    {
        EventDispatcher.TriggerEvent(Events.GearEvent.Damage, MogoWorld.thePlayer.ID, 9003, (int)2, CalcDamage(MogoWorld.thePlayer as EntityParent, magmaDamagePercentage, magmaDamageMin, magmaDamageMax));

        magmaDamageTimer = TimerHeap.AddTimer((uint)magmaDamageInterval, 0, SetMagmaDamage);
    }

    private void SetMagmaOnFireDamage()
    {
        EventDispatcher.TriggerEvent(Events.GearEvent.Damage, MogoWorld.thePlayer.ID, 9003, (int)2, CalcDamage(MogoWorld.thePlayer as EntityParent, onFireDamagePercentage, onFireDamageMin, onFireDamageMax));

        onFireDamageTimer = TimerHeap.AddTimer((uint)magmaDamageInterval, 0, SetMagmaOnFireDamage);
    }

    private int CalcDamage(EntityParent entity, float percentage, int damageMin, int damageMax)
    {
        return (int)(entity.hp * percentage + RandomHelper.GetRandomInt(damageMin, damageMax));
    }
}
