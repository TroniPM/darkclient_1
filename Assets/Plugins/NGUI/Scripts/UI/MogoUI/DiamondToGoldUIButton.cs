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

public class DiamondToGoldUIButton : MonoBehaviour 
{
    public int ID;

    void Awake()
    {
        gameObject.AddComponent<MogoFakeClick>().ReletedClassType = ReleadClassType.Type_DiamondToGoldUI;      
    }

    void OnClick()
    {
        if (DiamondToGoldUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        DiamondToGoldUIDict.ButtonTypeToEventUp[transform.name](ID);
    }

    public void SetEnable(bool enable)
    {
  
    }

    public void FakePress(bool isPressed)
    {
        if (DiamondToGoldUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        DiamondToGoldUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
}
