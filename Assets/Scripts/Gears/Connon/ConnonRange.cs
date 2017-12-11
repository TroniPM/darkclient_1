using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ConnonRange : GearParent
{
    public Connon connon;

    void Start()
    {
        gearType = "ConnonRange";
        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    #region Åö×²´¥·¢

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                stateOne = false;
                connon.TriggerEnter(other);
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
                    connon.TriggerEnter(other);
                }

                connon.TriggerStay(other);
            }
            else
            {
                if (!stateOne)
                {
                    stateOne = true;
                    connon.TriggerExit(other);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                stateOne = true;
                connon.TriggerExit(other);
            }
        }
    }

    #endregion
}
