/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：PlayerInfo
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：主界面玩家信息
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
	void Start () 
    {
        MainUIViewManager.Instance.SetPlayerLevel(60);
	}
}
