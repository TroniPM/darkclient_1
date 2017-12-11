using UnityEngine;
using System.Collections;

public class IAPGood : MonoBehaviour
{
    int m_nID = -1;
    public int ID
    {
        set
        {
            m_nID = value;
        }
    }

    void Awake()
    {
        transform.Find("Tier").GetComponent<UIButtonMessage>().target = gameObject;
        transform.Find("Tier").GetComponent<UIButtonMessage>().functionName = "OnBuySomething";
    }

    void OnBuySomething()
    {
        Mogo.Util.EventDispatcher.TriggerEvent<int>(IAPEvents.BuySomething, m_nID);
    }
}
