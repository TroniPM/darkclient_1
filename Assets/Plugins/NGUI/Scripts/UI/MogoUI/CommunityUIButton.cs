using UnityEngine;
using System.Collections;
using Mogo.Util;

public class CommunityUIButton : MonoBehaviour 
{
    public int ID;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_CommunityUI;
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
    //            if (CommunityUIDict.ButtonTypeToEventUp[transform.name] == null)
    //            {
    //                LoggerHelper.Error("No ButtonTypeToEventUp Info");
    //                return;
    //            }

    //            CommunityUIDict.ButtonTypeToEventUp[transform.name](ID);
    //        }
    //    }
    //}

    void OnClick()
    {
        if (CommunityUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Warning("Dictionary no contain the key : " + transform.name);
            return;
        }

        CommunityUIDict.ButtonTypeToEventUp[transform.name](ID);
    }

    public void FakePress(bool isPressed)
    {
        if (CommunityUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        CommunityUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
}
