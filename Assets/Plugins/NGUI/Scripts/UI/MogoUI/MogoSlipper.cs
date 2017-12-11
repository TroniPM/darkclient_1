using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class MogoSlipper : MonoBehaviour
{
    private Transform m_transform;
    private UISprite m_bgLeft;
    private UISprite m_bgRight;
    void Awake()
    {
        m_transform = transform;
        var ssList = m_transform.GetComponentsInChildren<UISprite>(true);
        m_bgLeft = ssList[0];
        m_bgRight = ssList[1];
        EventDispatcher.AddEventListener<byte>(UIEvent.SlipPage, OnPageChange);
    }
    void OnPageChange(byte Signal)
    {
        switch (Signal)
        { 
            case (byte)1:  //left begin
                m_bgRight.color = new Color32(22, 22, 22,255);
                m_bgLeft.color = new Color32(22, 22, 22, 255);
                break;
            case (byte)2: //right begin
                m_bgRight.color = new Color32(22, 22, 22, 255);
                m_bgLeft.color = new Color32(22, 22, 22,255);
                break;
            case (byte)0:
            default:
                m_bgRight.color = new Color32(255, 255, 255,255);
                m_bgLeft.color = new Color32(255, 255, 255,255);
                break;
        }
    }
}
