using UnityEngine;
using System.Collections;
using Mogo.Util;

public class SkillUIButton : MonoBehaviour {

    public int ID = 0;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_SkillUI;
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
    //            if (SkillUIDict.ButtonTypeToEventUp[transform.name] == null)
    //            {
    //                LoggerHelper.Error("No ButtonTypeToEventUp Info");
    //                return;
    //            }

    //            SkillUIDict.ButtonTypeToEventUp[transform.name](ID);
    //        }
    //    }

    //}

    void OnClick()
    {
        if (SkillUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        SkillUIDict.ButtonTypeToEventUp[transform.name](ID);
    }

    public void FakePress(bool isPressed)
    {
        if (SkillUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        SkillUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
}
