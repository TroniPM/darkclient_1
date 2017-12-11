using UnityEngine;
using System.Collections;
using Mogo.Util;

public class AssistantUIButton : MonoBehaviour {

    public int ID = -1;

    //void OnPress(bool isOver)
    //{
    //    if (isOver)
    //    {
    //    }
    //    else
    //    {
    //        Camera camera = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
    //        BoxCollider bc = transform.GetComponentInChildren<BoxCollider>();

    //        RaycastHit hit = new RaycastHit();

    //        if (bc.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 10000.0f))
    //        {
    //            if (AssistantUIDict.ButtonTypeToEventUp[transform.name] == null)
    //            {
    //                LoggerHelper.Error("No ButtonTypeToEventUp Info");
    //                return;
    //            }

    //            AssistantUIDict.ButtonTypeToEventUp[transform.name](ID);
    //        }
    //    }
    //}

    void OnClick()
    {
        AssistantUIDict.ButtonTypeToEventUp[transform.name](ID);
    }

    public void ShowLock(bool isShow)
    {
        UISprite sp = transform.GetChild(1).GetComponentsInChildren<UISprite>(true)[0];

        sp.gameObject.SetActive(isShow);
    }

    public void ShowFG(bool isShow)
    {
        UISprite sp = transform.GetChild(2).GetComponentsInChildren<UISprite>(true)[0];
        sp.gameObject.SetActive(isShow);
    }
}
