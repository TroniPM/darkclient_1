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

public class UpgradePowerUIButton : MonoBehaviour 
{
    public int ID;   

    void OnClick()
    {
        if (UpgradePowerUIDict.ButtonTypeToEventUp[transform.name] == null)
        {
            LoggerHelper.Error("No ButtonTypeToEventUp Info");
            return;
        }

        UpgradePowerUIDict.ButtonTypeToEventUp[transform.name](ID);
    }
}
