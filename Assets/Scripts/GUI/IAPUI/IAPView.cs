using UnityEngine;
using System.Collections;

public class IAPView : MonoBehaviour
{
    void Hide() 
    {
        Mogo.Util.EventDispatcher.TriggerEvent(IAPEvents.HideIAPView);
    }
}
