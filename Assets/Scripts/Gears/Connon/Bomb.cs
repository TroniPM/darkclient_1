using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.Game;

public class Bomb : GearParent
{
    public bool canAttackDummy = false;

    public float radius;
    public float percentage;
    public int damageMin;
    public int damageMax;

	public float gravity { get; set; }

	public float vx;
	public float vy;

	public Vector3 directionX { get; set; }
	public Vector3 directionY { get; set; }

	protected SfxHandler sfxHandler { get; set; }

	protected uint timerID;

	void Start()
	{
		gearType = "Bomb";
		triggleEnable = true;

		sfxHandler = gameObject.AddComponent<SfxHandler>();

		timerID = uint.MaxValue;

		sfxHandler.HandleFx(6011, null, (go, dbid) =>
		{
			go.transform.position = MogoWorld.thePlayer.Transform.position;
		});
	}

	void OnDestroy()
	{
		TimerHeap.DelTimer(timerID);
	}
	
	void Update ()
	{
		transform.Translate(directionX * vx * Time.deltaTime);
		transform.Translate(directionY * vy * Time.deltaTime);
		vy -= gravity * Time.deltaTime;
	}

	void OnTriggerEnter(Collider other)
	{
		if (triggleEnable)
		{
            if (other.gameObject.layer == 9)
			{
				MogoMainCamera.Instance.Shake(5, 0.1f);
				sfxHandler.HandleFx(500102);
				sfxHandler.RemoveFXs(6011);
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

				if (timerID == uint.MaxValue)
					timerID = TimerHeap.AddTimer(5000, 0, BombDestroy);
				vx = 0;
				vy = 0;
				triggleEnable = false;
			}
		}
	}

	private void BombDestroy()
	{
		sfxHandler.RemoveFXs(500102);
		// sfxHandler.RemoveFXs(6011);

        if (this && this.gameObject)
            AssetCacheMgr.SynReleaseInstance(this.gameObject);
	}

	private int CalcDamage(EntityParent entity)
	{
        return (int)(entity.hp * percentage + RandomHelper.GetRandomInt(damageMin, damageMax));
	}
}
