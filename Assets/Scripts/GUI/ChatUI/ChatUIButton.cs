using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ChatUIButton : MonoBehaviour
{

    void OnPress(bool isOver)
    {
        if (isOver)
        {
        }
        else
        {
            Camera camera = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
            BoxCollider bc = transform.GetComponentInChildren<BoxCollider>();

            RaycastHit hit = new RaycastHit();

            if (bc.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 10000.0f))
            {
                if (ChatUIViewManager.ButtonTypeToEventUp[transform.name] == null)
                {
                    LoggerHelper.Error("No ButtonTypeToEventUp Info");
                    return;
                }

                ChatUIViewManager.ButtonTypeToEventUp[transform.name]();
            }
        }

    }
}
