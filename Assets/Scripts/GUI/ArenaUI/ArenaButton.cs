using UnityEngine;
using System.Collections;
using Mogo.Util;
public class ArenaButton : MonoBehaviour
{
    public int id;

    void OnClick()
    {
        EventDispatcher.TriggerEvent<int>(Events.ArenaEvent.TabSwitch, id);
    }
}