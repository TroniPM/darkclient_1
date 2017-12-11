using UnityEngine;
using System.Collections;
using Mogo.Util;

public class PackageItemBoxUpSign : MonoBehaviour {

    public int id = 0;

    void OnClick()
    {
        EventDispatcher.TriggerEvent(MenuUIDict.MenuUIEvent.PACKAGEGRIDUPSIGNUP, id);
    }
}
