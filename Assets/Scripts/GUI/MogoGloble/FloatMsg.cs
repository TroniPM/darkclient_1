/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：FloatMsg
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-5-15
// 模块描述：MsgBox界面管理
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;

public class FloatMsg : MonoBehaviour
{

    UILabel m_lblText;
    public TweenAlpha m_tweenAlpha;
    public TweenPosition m_tweenPosition;

    Transform m_myTransform;
    public string text = string.Empty;

    void Awake()
    {
        m_myTransform = transform;

        Initialize();

        m_lblText = m_myTransform.GetComponentsInChildren<UILabel>(true)[0];
        m_tweenAlpha = m_myTransform.GetComponentsInChildren<TweenAlpha>(true)[0];
        m_tweenPosition = m_myTransform.GetComponentsInChildren<TweenPosition>(true)[0];
    }


    //void Start()
    //{
    //    m_tweenAlpha.enabled = true;
    //    m_tweenPosition.enabled = true;

    //    m_lblText.text = text;
    //    m_tweenAlpha.Reset();
    //    m_tweenPosition.Reset();

    //    m_tweenPosition.Play(true);
    //    m_tweenPosition.Play(true);
    //}
    void Initialize()
    {

    }

    void Release()
    {

    }

    public void Show(string text)
    {
        m_tweenAlpha.enabled = true;
        m_tweenPosition.enabled = true;

        m_lblText.text = text;
        m_tweenAlpha.Reset();
        m_tweenPosition.Reset();

        m_tweenPosition.Play(true);
        m_tweenPosition.Play(true);

        //m_tweenPosition.eventReceiver = gameObject;
        //m_tweenPosition.callWhenFinished = "OnFinished";
    }

    //public void OnFinished()
    //{
    //    AssetCacheMgr.ReleaseInstance(gameObject);
    //}

   



}
