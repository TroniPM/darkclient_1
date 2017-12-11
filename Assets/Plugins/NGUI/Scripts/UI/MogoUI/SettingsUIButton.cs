using UnityEngine;
using System.Collections;
using Mogo.Util;

public class SettingsUIButton : MonoBehaviour {

    public int ID = -1;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_SettingsUI;
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
                if (SettingsUIDict.ButtonTypeToEventUp[transform.name] == null)
                {
                    LoggerHelper.Error("No ButtonTypeToEventUp Info");
                    return;
                }

                SettingsUIDict.ButtonTypeToEventUp[transform.name](ID);
            }
        }
    }

    public void FakePress(bool isPressed)
    {
        if (SettingsUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        SettingsUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
}
