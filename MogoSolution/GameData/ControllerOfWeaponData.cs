/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ControllerOfWeaponData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-4-3
// 模块描述：
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class ControllerOfWeaponData : GameData<ControllerOfWeaponData>
    {
        public string controller { get; protected set; }
        public string controllerInCity { get; protected set; }
        static public readonly string fileName = "xml/ControllerOfWeapon";
        //static public Dictionary<int, ControllerOfWeaponData> dataMap { get; set; }
    }
}