/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：NoticeTitle
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-7-31
// 模块描述：标题点击响应
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class NoticeTitle : MonoBehaviour {

    public int index =0;

    void OnClick()
    {
        MogoNotice.Instance.OnSelectTitle(index);
    }
}
