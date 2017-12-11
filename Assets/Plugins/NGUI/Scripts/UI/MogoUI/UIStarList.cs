/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：UIStarList
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;

public class UIStarList : MonoBehaviour
{
    public List<UIStar> starList;

    public void SetStarNum(int num)
    {
        if (starList == null)
        {
            Mogo.Util.LoggerHelper.Debug("starList == null");
            return;
        }
        for (int i = 0; i < starList.Count; i++)
        {
            if (i < num)
                starList[i].TurnOn(true);
            else
                starList[i].TurnOn(false);
        }
    }
}