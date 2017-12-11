using UnityEngine;
using System.Collections;
using Mogo.Util;

public class DragonUIButton : MonoBehaviour 
{
    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_DragonUI;
    }

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
    //            if (DragonUIDict.ButtonTypeToEventUp[transform.name] == null)
    //            {
    //                LoggerHelper.Error("No ButtonTypeToEventUp Info");
    //                return;
    //            }

    //            DragonUIDict.ButtonTypeToEventUp[transform.name](0);
    //        }
    //    }

    //}

    void OnClick()
    {
        DragonUIDict.ButtonTypeToEventUp[transform.name](0);
    }

    public void FakePress(bool isPressed)
    {
        if (DragonUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        DragonUIDict.ButtonTypeToEventUp[transform.name](0);
    }
	

}
