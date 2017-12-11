/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ExportAgent
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：
// 最后修改日期：
// 模块描述：
// 代码版本：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class ExportAgent : MonoBehaviour 
{
    public int ID;
    public string type { get; protected set;}
    public Transform theTransform { get; protected set; }
    protected GameObject temp;

    void Start()
    {
        type = "ExportAgent";
        theTransform = transform;

        GameObject go = (GameObject)Instantiate(Resources.Load("Characters/" + ID));
        if (go != null)
        {
            go.transform.position = transform.position;
        }
    }
}
