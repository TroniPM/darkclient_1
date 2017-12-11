using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class Transporter : GearParent
{
    protected Dictionary<Transform, KeyValuePair<Transform, Vector3>> passengerScales = new Dictionary<Transform,KeyValuePair<Transform,Vector3>>();

    void Start()
    {
        gearType = "Carrousel";
        ID = (uint)defaultID;

        passengerScales = new Dictionary<Transform, KeyValuePair<Transform, Vector3>>();

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (passengerScales.ContainsKey(other.transform))
                    return;

                passengerScales.Add(other.transform, new KeyValuePair<Transform, Vector3>(other.transform.parent, other.transform.localScale));
                other.transform.parent = transform;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (passengerScales.ContainsKey(other.transform))
                    return;

                passengerScales.Add(other.transform, new KeyValuePair<Transform, Vector3>(other.transform.parent, other.transform.localScale));
                other.transform.parent = transform;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (!passengerScales.ContainsKey(other.transform))
                    return;
                other.transform.parent = passengerScales[other.transform].Key;
                other.transform.localScale = passengerScales[other.transform].Value;

                passengerScales.Remove(other.transform);
            }
        }
    }
}
