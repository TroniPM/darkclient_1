using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class NormalMainUIButton : MonoBehaviour 
{
    List<Vector3> m_listInputPos = new List<Vector3>();
    public Camera RelatedCam;

    void Awake()
    {
        //System.Type t = typeof(NormalMainUIButton);
        //var com = GetComponent(t);
        //var prop = t.GetMethod("SetVis");
        //prop.Invoke(com, null);

        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_NormalMainUI;
    }

    //void OnPress(bool isOver)
    //{
    //    if (isOver)
    //    {
    //        m_listInputPos.Clear();
    //    }
    //    else
    //    {
    //        if (RelatedCam == null)
    //        {
    //            RelatedCam = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
    //        }
    //        BoxCollider bc = transform.GetComponentInChildren<BoxCollider>();

    //        RaycastHit hit = new RaycastHit();

    //        switch (Application.platform)
    //        {
    //            case RuntimePlatform.Android:
    //            case RuntimePlatform.IPhonePlayer:
    //                for (int i = 0; i < Input.touchCount; ++i)
    //                {
    //                    m_listInputPos.Add(Input.GetTouch(i).position);
    //                }

    //                break;

    //            case RuntimePlatform.WindowsPlayer:
    //            case RuntimePlatform.WindowsEditor:
    //            case RuntimePlatform.OSXEditor:
    //            case RuntimePlatform.OSXPlayer:
    //                m_listInputPos.Add(Input.mousePosition);
    //                break;

    //        }

    //        for (int i = 0; i < m_listInputPos.Count; ++i)
    //        {
    //            if (bc.Raycast(RelatedCam.ScreenPointToRay(m_listInputPos[i]), out hit, 10000.0f))
    //            {
    //                if (NormalMainUIDict.ButtonTypeToEventUp[transform.name] == null)
    //                {
    //                    LoggerHelper.Error("No ButtonTypeToEventUp Info");
    //                    return;
    //                }
    //                NormalMainUIDict.ButtonTypeToEventUp[transform.name]();

    //                break;
    //            }
    //        }
    //    }

    //}

    void OnClick()
    {
        if (NormalMainUIDict.ButtonTypeToEventUp.ContainsKey(transform.name))
            NormalMainUIDict.ButtonTypeToEventUp[transform.name]();
    }

    public void FakePress(bool isOver)
    {
        if (NormalMainUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }
        NormalMainUIDict.ButtonTypeToEventUp[transform.name]();
    }
}
