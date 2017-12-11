/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TongButton
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;

public class TongButton : MonoBehaviour
{
    public int id = 0;
    void OnClick()
    {
        //Mogo.Util.LoggerHelper.Debug(gameObject.name);
        //gameObject.name = gameObject.name.Replace("(Clone)", "");
        //Mogo.Util.LoggerHelper.Debug(gameObject.name);
        TongUIViewManager.Instance.OnButtonClick(gameObject.name, id);

    }
}