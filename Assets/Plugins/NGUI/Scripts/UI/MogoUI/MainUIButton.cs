/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MainUIButton
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：主界面按钮
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;

public class MainUIButton : MonoBehaviour 
{
    List<Vector3> m_listInputPos = new List<Vector3>();

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_BattleMainUI;
    }

    //void OnPress(bool isOver)
    //{
    //    if (isOver)
    //    {
    //        m_listInputPos.Clear();
    //    }
    //    else
    //    {
    //        Camera camera = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
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
    //            if (true)
    //            {
    //                if (MainUIDict.ButtonTypeToEventUp[transform.name] == null)
    //                {
    //                    LoggerHelper.Error("No ButtonTypeToEventUp Info");
    //                    return;
    //                }
    //                EventDispatcher.TriggerEvent(MainUIDict.ButtonTypeToEventUp[transform.name]);
    //                LoggerHelper.Debug(transform.name);
    //                break;
    //            }
    //        }
    //    }
        
    //}

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            EventDispatcher.TriggerEvent(MainUIDict.ButtonTypeToEventUp[transform.name]);
        }
    }

    public void FakePress(bool isPressed)
    {
        if (MainUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }
        EventDispatcher.TriggerEvent(MainUIDict.ButtonTypeToEventUp[transform.name]);
    }
}
