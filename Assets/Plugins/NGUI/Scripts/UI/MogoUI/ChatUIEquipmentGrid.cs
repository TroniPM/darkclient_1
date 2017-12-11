using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ChatUIEquipmentGrid : MonoBehaviour
{

    public string LogicText;

    //void OnPress(bool isOver)
    //{
    //    if (isOver)
    //    {

    //    }
    //    else
    //    {
    //        Camera camera = transform.parent.parent.parent.GetComponentsInChildren<Camera>(true)[0];
    //        BoxCollider bc = transform.GetComponentInChildren<BoxCollider>();

    //        RaycastHit hit = new RaycastHit();

    //        if (bc.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 10000.0f))
    //        {
    //            EventDispatcher.TriggerEvent<string>("ChatUIEquipmentGridUp", LogicText);
    //        }
    //    }

    //}

    void OnClick()
    {
        EventDispatcher.TriggerEvent<string>("ChatUIEquipmentGridUp", LogicText);
    }
}
