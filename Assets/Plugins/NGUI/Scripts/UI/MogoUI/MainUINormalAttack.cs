using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MainUINormalAttack : MonoBehaviour
{

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.NORMALATTACKDOWN);
        }
        else
        {
            EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.NORMALATTACTUP);
        }
    }
}
