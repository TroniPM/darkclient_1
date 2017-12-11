using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ConnonSwitcherGear : GearParent
{
	public Connon connon;
	protected bool isStay;

	void Start()
	{
		gearType = "ConnonSwitcherGear";
        ID = (uint)defaultID;

        AddListeners();
	}

	void OnDestroy()
	{
        RemoveListeners();
	}


    #region 碰撞触发

    void OnTriggerEnter(Collider other)
	{
		if (other.tag == GearParent.MogoPlayerTag)
		{
			if (triggleEnable)
			{
				stateOne = false;
                Switch(other.gameObject);
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == GearParent.MogoPlayerTag)
		{
			if (triggleEnable)
			{
				if (stateOne)
				{
					stateOne = false;
                    Switch(other.gameObject);
				}
			}
		}
    }

    #endregion


    #region 机关事件

    protected override void SetGearEventEnable(uint enableID)
    {
        base.SetGearEventEnable(enableID);
		if (enableID == ID)
		{
        var collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
		}
    }

    protected override void SetGearEventDisable(uint disableID)
    {
        base.SetGearEventDisable(disableID);
				if (disableID == ID)
		{
        var collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }
		}
    }

    #endregion


    public void Switch(GameObject go)
    {
        connon.SetState();
        connon.SetBarrelRotate(go);
        EventDispatcher.TriggerEvent(Events.GearEvent.UploadAllGear);
    }
}
