using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ChallengeUIButton : MonoBehaviour
{
    public int ID;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_ChallengeUI;
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
    //            if (ChallengeUIDict.ButtonTypeToEventUp[transform.name] == null)
    //            {
    //                LoggerHelper.Error("No ButtonTypeToEventUp Info");
    //                return;
    //            }

    //            ChallengeUIDict.ButtonTypeToEventUp[transform.name](ID);
    //        }
    //    }

    //}

    void OnClick()
    {
        ChallengeUIDict.ButtonTypeToEventUp[transform.name](ID);
    }

    public void FakePress(bool isPressed)
    {
        if (ChallengeUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        ChallengeUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
}
