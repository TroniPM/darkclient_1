/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：InsetBGMask
// 创建者：
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;

public class InsetBGMask : MonoBehaviour
{
    void OnClick()
    {
        Mogo.Util.LoggerHelper.Debug("OnClick InsetBGMask!");
        InsetManager.Instance.OnJewelSlotSelect(-1);
        
    }
}