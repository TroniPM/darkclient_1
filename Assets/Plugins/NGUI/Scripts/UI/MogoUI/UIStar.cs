/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：UIStar
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;

public class UIStar : MonoBehaviour
{

    public void TurnOn(bool isTurnOn = true)
    {
        Mogo.Util.LoggerHelper.Debug("isTurnOn:" + isTurnOn);
        transform.GetChild(0).gameObject.SetActive(isTurnOn);
    }
}