#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class EnergyUIButton : MonoBehaviour
{
    public int ID;

    private UISlicedSprite m_ssBG;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_EnergyUI;      
    }  

    void OnClick()
    {
        if (EnergyUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        EnergyUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
  

    public void FakePress(bool isPressed)
    {
        if (EnergyUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        EnergyUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
}
