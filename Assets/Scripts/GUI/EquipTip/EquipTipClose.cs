/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EquipTipClose
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-6-25
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class EquipTipClose : MonoBehaviour
{

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {

        }
        else
        {
            EquipTipManager.Instance.CloseEquipTip();
           
        }
    }
}
