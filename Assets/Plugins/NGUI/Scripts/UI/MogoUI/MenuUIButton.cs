/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MenuUIButton
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class MenuUIButton : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public Camera RelatedCamera = null;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_MenuUI;
    }
    void OnPress(bool isOver)
    {
        if (isOver)
        {
            //if (MenuUIViewManager.ButtonTypeToEventDown[transform.name] == null)
            //{
            //    LoggerHelper.Error("No ButtonTypeToEventDown Info");
            //    return;
            //}
         //   EventDispatcher.TriggerEvent(MenuUIViewManager.ButtonTypeToEventDown[transform.name]);
        }
        else
        {
            if (RelatedCamera == null)
            {
                RelatedCamera = GameObject.Find("MogoMainUI").transform.GetChild(0).GetComponentInChildren<Camera>();
            }
            BoxCollider bc = transform.GetComponentInChildren<BoxCollider>();

            RaycastHit hit = new RaycastHit();

            if (bc.Raycast(RelatedCamera.ScreenPointToRay(Input.mousePosition), out hit, 1000000.0f))
            {
                if (MenuUIDict.ButtonTypeToEventUp[transform.name] == null)
                {
                    LoggerHelper.Error("No ButtonTypeToEventUp Info");
                    return;
                }
                EventDispatcher.TriggerEvent(MenuUIDict.ButtonTypeToEventUp[transform.name]);
            }
        }

    }

    public void FakePress(bool isPressed)
    {
        if (MenuUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }
        EventDispatcher.TriggerEvent(MenuUIDict.ButtonTypeToEventUp[transform.name]);
    }
}
