using UnityEngine;
using System.Collections;
using Mogo.Util;

public class DecomposeUIButton : MonoBehaviour
{
    public Camera RelatedCamera;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_DecomposeUI;
    }

    //void OnPress(bool isOver)
    //{
    //    if (isOver)
    //    {
    //    }
    //    else
    //    {
    //        if (RelatedCamera == null)
    //        {
    //            RelatedCamera = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
    //        }
    //        BoxCollider bc = transform.GetComponentInChildren<BoxCollider>();

    //        RaycastHit hit = new RaycastHit();

    //        if (bc.Raycast(RelatedCamera.ScreenPointToRay(Input.mousePosition), out hit, 10000.0f))
    //        {
    //            if (DecomposeUIDict.ButtonTypeToEventUp[transform.name] == null)
    //            {
    //                LoggerHelper.Error("No ButtonTypeToEventUp Info");
    //                return;
    //            }

    //            DecomposeUIDict.ButtonTypeToEventUp[transform.name]();
    //        }
    //    }

    //}

    void OnClick()
    {
        if (DecomposeUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        DecomposeUIDict.ButtonTypeToEventUp[transform.name]();
    }

    public void FakePress(bool isPressed)
    {
        Mogo.Util.LoggerHelper.Debug("FakePress");
        if (DecomposeUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        Mogo.Util.LoggerHelper.Debug("FakePress done");
        DecomposeUIDict.ButtonTypeToEventUp[transform.name]();
    }
}
