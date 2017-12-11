/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoUIAnimPowerup
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;

public class MogoUIAnimPowerup : MonoBehaviour
{
    public float duration = 0.2f;
    public float peek = 1.3f;

    void Start()
    {
        TweenScale scale = gameObject.AddComponent<TweenScale>();
        scale.from = Vector3.zero;
        scale.to = transform.localScale;
        scale.duration = duration;
        scale.animationCurve.AddKey(0.9f, peek);
        scale.enabled = true;
        scale.Reset();
        scale.onFinished = (UITweener t) =>
        {
            Destroy(this);
            Destroy(scale);
        };
    }
}