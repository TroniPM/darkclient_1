/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：InstanceUIButton
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class InstanceUIButton : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public int ID;

    private UISlicedSprite m_ssBG;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_InstanceUI;

        if (transform.Find("InstanceLevel" + ID + "BG") != null)
        {
            m_ssBG = transform.Find("InstanceLevel" + ID + "BG").GetComponentsInChildren<UISlicedSprite>(true)[0];
        }
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
    //            if (InstanceUIDict.ButtonTypeToEventUp[transform.name] == null)
    //            {
    //                LoggerHelper.Error("No ButtonTypeToEventUp Info");
    //                return;
    //            }

    //            InstanceUIDict.ButtonTypeToEventUp[transform.name](ID);
    //        }
    //    }

    //}

    void OnClick()
    {
        if (InstanceUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        InstanceUIDict.ButtonTypeToEventUp[transform.name](ID);
    }

    public void SetEnable(bool enable)
    {
        //if (!m_ssBG)
        //{
        //    if (transform.FindChild("InstanceLevel" + ID + "BG") != null)
        //    {
        //        m_ssBG = transform.FindChild("InstanceLevel" + ID + "BG").GetComponentsInChildren<UISlicedSprite>(true)[0];
        //    }
        //}

        //if(m_ssBG)
        //{
        //    if (enable)
        //    {
        //        m_ssBG.color = new Color32(255, 255, 255, 255);
        //    }
        //    else
        //    {
        //        m_ssBG.color = new Color32(128, 128, 128, 255);
        //    }
        //}
    }

    public void FakePress(bool isPressed)
    {
        if (InstanceUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        InstanceUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
}
