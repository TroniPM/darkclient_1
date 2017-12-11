using UnityEngine;
using System.Collections;
using Mogo.Util;

public class OKCancelBoxButton : MonoBehaviour
{
    //void OnPress(bool isOver)
    //{
    //    if (isOver)
    //    {
    //    }
    //    else
    //    {
    //        Camera camera = GameObject.Find("MogoMainUI").transform.GetChild(1).GetComponentInChildren<Camera>();
    //        BoxCollider bc = transform.GetComponentInChildren<BoxCollider>();

    //        RaycastHit hit = new RaycastHit();

    //        if (bc.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 10000.0f))
    //        {
    //            EventDispatcher.TriggerEvent(transform.name + "Up");
    //        }
    //    }

    //}

    void OnClick()
    {
        EventDispatcher.TriggerEvent(transform.name + "Up");
 
    }
}
