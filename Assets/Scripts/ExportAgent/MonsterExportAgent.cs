/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MonsterExportAgent
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：
// 最后修改日期：
// 模块描述：
// 代码版本：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Game;
using Mogo.Util;

public class MonsterExportAgent : ExportAgent
{
    void Start()
    {    
        type = "Monster";
        //GameObject go = (GameObject)Instantiate(Resources.Load("Characters/" + ID));
        //if (go != null)
        //{
        //    go.transform.position = transform.position;
        //}
    }
}
