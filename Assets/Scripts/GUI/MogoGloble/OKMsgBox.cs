/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������OKMsgBox
// �����ߣ�Joe Mo
// �޸����б��
// �������ڣ�2013-5-15
// ģ��������MsgBox�������
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System;

public class OKMsgBox : MonoBehaviour
{

    UILabel m_lblBoxText;
    UILabel m_lblOKBtnText;

    Transform m_myTransform;

    Action m_actCallback;


    void Awake()
    {
        gameObject.SetActive(false);

        m_myTransform = transform;

        Initialize();

        m_lblBoxText = m_myTransform.Find("MsgBoxText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOKBtnText = m_myTransform.Find("MsgBoxBtn/MsgBoxBtnText").GetComponentsInChildren<UILabel>(true)[0];
        m_myTransform.Find("MsgBoxBtn").gameObject.AddComponent<OKCancelBoxButton>();
        m_myTransform.Find("MsgBoxBGMask").gameObject.AddComponent<MsgBoxBGMask>();
    }

    void OnOKButtonUp()
    {
        if(m_actCallback != null)
        {
            m_actCallback();
        }
    }


    void Initialize()
    {
        EventDispatcher.AddEventListener("MsgBoxBtnUp", OnOKButtonUp);
    }

    void Release()
    {
        EventDispatcher.RemoveEventListener("MsgBoxBtnUp", OnOKButtonUp);
    }

    public void SetBoxText(string text)
    {
        m_lblBoxText.text = text;
    }

    public void SetOKBtnText(string text)
    {
        m_lblOKBtnText.text = text;
    }


    public void SetCallback(Action callback)
    {
        m_actCallback = callback;
    }

}
