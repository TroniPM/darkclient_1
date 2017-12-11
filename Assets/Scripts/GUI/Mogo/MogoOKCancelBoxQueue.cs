using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class MogoOKCancelBoxQueue : MonoBehaviour
{
    public List<MogoOKCancelBoxQueueUnit> m_listBox = new List<MogoOKCancelBoxQueueUnit>();

    private static MogoOKCancelBoxQueue m_instance;

    public static MogoOKCancelBoxQueue Instance
    {
        get
        {
            return MogoOKCancelBoxQueue.m_instance;
        }
    }

    void Awake()
    {
        m_instance = transform.GetComponentsInChildren<MogoOKCancelBoxQueue>(true)[0];

        EventDispatcher.AddEventListener("OKCancelBoxClose", GetOne);
    }

    public void Release()
    {
        EventDispatcher.RemoveEventListener("OKCancelBoxClose", GetOne);
    }



    public void PushOne(string boxText, string okText = "OK", string cancelText = "CANCEL", Action<bool> act = null,
        ButtonBgType okBgType = ButtonBgType.Blue, ButtonBgType cancelBgType = ButtonBgType.Brown,Action actOneBtn = null)
    {
        MogoOKCancelBoxQueueUnit unit = new MogoOKCancelBoxQueueUnit()
        {
            BoxText = boxText,
            OKText = okText,
            CancelText = cancelText,
            OKBgType = okBgType,
            CancelBgType = cancelBgType,
            CBAction = act,
            CBActionOneBtn = actOneBtn
        };

        if (MogoGlobleUIManager.Instance.m_goOKCancelBox != null && MogoGlobleUIManager.Instance.m_goOKCancelBox.gameObject.activeSelf)
        {
            m_listBox.Add(unit);
            Mogo.Util.LoggerHelper.Debug("Adding into OKCancel Box Queue");
        }
        else
        {
            unit.JustDoIt();
        }
    }

    public void GetOne()
    {
        if (m_listBox.Count > 0)
        {
            TimerHeap.AddTimer(100, 0, () =>
            {
                m_listBox[0].JustDoIt();

                m_listBox.RemoveAt(0);

                Mogo.Util.LoggerHelper.Debug("GetOne");
            });

            
        }
    }



}
