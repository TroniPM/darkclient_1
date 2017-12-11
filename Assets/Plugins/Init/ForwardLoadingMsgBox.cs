/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ForwardLoadingMsgBox
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using System.Diagnostics;
using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;

public class ForwardLoadingMsgBox : MonoBehaviour
{
    static public ForwardLoadingMsgBox Instance;
    OKCancelBox m_msgBox;
    //当前是显示还是隐藏
    bool ishide = true;
    void Awake()
    {
        Instance = this;
        m_msgBox = transform.Find("OKCancel").GetComponent<OKCancelBox>();
    }

    void OnDestroy()
    {
        m_msgBox = null;
    }

    public void ShowRetryMsgBox(string content, Action<bool> onClick)
    {
        ShowMsgBox(DefaultUI.dataMap[50].content, DefaultUI.dataMap[7].content, content, (isOk) =>
        {
            ForwardLoadingMsgBox.Instance.Hide();
            if (onClick != null)
                onClick(isOk);
        });
    }

    public void ShowMsgBox(string okText, string cancelText, string content, Action<bool> onClick)
    {
        //Debug.LogError("ShowMsgBox");
        m_msgBox.gameObject.SetActive(true);
        m_msgBox.SetBoxText(content);
        m_msgBox.SetOKBtnText(okText);
        m_msgBox.SetCancelBtnText(cancelText);
        m_msgBox.SetCallback(onClick);
        ishide = false;
    }
    //带倒计时的消息框,前面几个参数是一样的，最后两个：倒计时;倒计时完成的时间
    public void ShowMsgBoxWithCountDown(string okText, string cancelText, string content, Action<bool> onClick, uint count, Action timeup)
    {
        var countCopy = count;
        ShowMsgBox(okText, cancelText, content, onClick);
        //定时器显示
        var timerid = TimerHeap.AddTimer(0, 500, () =>
        {
            countCopy -= 500;
            m_msgBox.SetBoxText(content + "(" + countCopy / 1000 + ")");
        });
        //定时器完成
        TimerHeap.AddTimer(count, 0, () =>
        {
            Hide();
            if (timeup != null)
                timeup();
            TimerHeap.DelTimer(timerid);
        });
    }
    public void Hide()
    {
        m_msgBox.gameObject.SetActive(false);
        ishide = true;
    }
    public bool IsHide()
    {
        return ishide;
    }
}