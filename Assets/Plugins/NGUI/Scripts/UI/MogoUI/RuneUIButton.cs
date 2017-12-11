using UnityEngine;
using System.Collections;
using Mogo.Util;

public class RuneUIButton : MonoBehaviour
{

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_RuneUI;
    }

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
                if (RuneUIDict.ButtonTypeToEventUp[transform.name] == null)
                {
                    LoggerHelper.Error("No ButtonTypeToEventUp Info");
                    return;
                }

                RuneUIDict.ButtonTypeToEventUp[transform.name](0);
            }
        }

    }

    public void FakePress(bool isPressed)
    {
        if (RuneUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        RuneUIDict.ButtonTypeToEventUp[transform.name](0);
    }
}
