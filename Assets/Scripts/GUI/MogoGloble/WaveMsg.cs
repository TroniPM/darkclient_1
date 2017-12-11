/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：WaveMsg
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-7-25
// 模块描述：MsgBox界面管理
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;

public class WaveMsg : MonoBehaviour
{

    UILabel m_lblText;
    Vector3 m_from;
    float m_beginTime = 0f;
    float m_timeLength;
    Vector3 m_to;
    bool m_isWaving = false;
    Action m_onFinised;

    void Awake()
    {

        Initialize();

        m_lblText = GetComponentsInChildren<UILabel>(true)[0];
    }

    void Update()
    {
        if (m_isWaving)
        {
            if ((Time.time - m_beginTime) > m_timeLength)
            {
                m_isWaving = false;
                m_onFinised();
            }
            else
            {
                transform.position = Vector3.Lerp(m_from, m_to, (Time.time - m_beginTime) / m_timeLength);
            }
        }
    }
    void Initialize()
    {

    }

    void Release()
    {

    }

    public void Show(string text, float time, Vector3 from, Vector3 to, Action onFinised)
    {
        m_lblText.text = text;
        transform.position = from;
        m_from = from;
        m_to = to;
        m_beginTime = Time.time;
        m_timeLength = time;
        m_isWaving = true;
        m_onFinised = onFinised;
        //m_tweenPosition.eventReceiver = gameObject;
        //m_tweenPosition.callWhenFinished = "OnFinished";
    }

    //public void OnFinished()
    //{
    //    AssetCacheMgr.ReleaseInstance(gameObject);
    //}

    public void SeText(string text)
    {

    }



}
