/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoPropSheetIcon
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MogoPropSheetIcon : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public string KeyWord;

    void OnPress(bool isOver)
    {
        if (!isOver)
        {
            EventDispatcher.TriggerEvent(KeyWord + name + "Up");
        }
    }
}
