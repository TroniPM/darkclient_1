/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoParentUI
// 创建者：Charles
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class MogoParentUI : MonoBehaviour 
{
    #region 公共变量
    #endregion

    #region 私有变量
    #endregion

	// Use this for initialization
	void Start () {

	 
	}
    public void AddButtonListener(string event_name, string control_name, Action action)
    {
        switch (event_name.ToLower())
        {
            case "onclicked":
                transform.Find(control_name).GetComponentsInChildren<MogoButton>(true)[0].clickHandler += action;
                break;
            default:
                break;
        }

    }
}
