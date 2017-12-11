/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DoorOfBuryTipUIMgr
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using System;

public class DoorOfBuryTipUIMgr : MogoUIParent
{
    List<TweenScale> m_wordsTween;
  
    public static DoorOfBuryTipUIMgr Instance;
    //private bool m_isLoadDone = false;
    private Action m_onDone;

    public void Initialize()
    {
        base.Init();
        Instance = this;
        if (m_wordsTween == null)
        {
            m_wordsTween = new List<TweenScale>();
            m_wordsTween.Add(GetUIChild("DoorOfBuryOpenTipWord0").GetComponent<TweenScale>());
            m_wordsTween.Add(GetUIChild("DoorOfBuryOpenTipWord1").GetComponent<TweenScale>());
            m_wordsTween.Add(GetUIChild("DoorOfBuryOpenTipWord2").GetComponent<TweenScale>());
            m_wordsTween.Add(GetUIChild("DoorOfBuryOpenTipWord3").GetComponent<TweenScale>());

        }

        gameObject.SetActive(false);
    }
  

    void OnEnable()
    {
        //Debug.LogError("OnEnable");
        //if (!SystemSwitch.DestroyResource)
        //    return;
        int count = 0;
        if (GetUIChild("DoorOfBuryOpenTipWord0").GetComponentsInChildren<UITexture>(true)[0].mainTexture == null)
        {
            //Debug.LogError("null");
            AssetCacheMgr.GetResourceAutoRelease("t_e.png", (obj) =>
            {
                GetUIChild("DoorOfBuryOpenTipWord0").GetComponentsInChildren<UITexture>(true)[0].mainTexture = (Texture)obj;
                count++;
                if (count == 4)
                {
                    //m_isLoadDone = true;
                    DoShowWord();
                }
            });

            AssetCacheMgr.GetResourceAutoRelease("t_m.png", (obj) =>
            {
                GetUIChild("DoorOfBuryOpenTipWord1").GetComponentsInChildren<UITexture>(true)[0].mainTexture = (Texture)obj;
                count++;
                if (count == 4)
                {
                    //m_isLoadDone = true;
                    DoShowWord();
                }
            });

            AssetCacheMgr.GetResourceAutoRelease("t_j.png", (obj) =>
            {
                GetUIChild("DoorOfBuryOpenTipWord2").GetComponentsInChildren<UITexture>(true)[0].mainTexture = (Texture)obj;
                count++;
                if (count == 4)
                {
                    //m_isLoadDone = true;
                    DoShowWord();

                }
            });

            AssetCacheMgr.GetResourceAutoRelease("t_l.png", (obj) =>
            {
                GetUIChild("DoorOfBuryOpenTipWord3").GetComponentsInChildren<UITexture>(true)[0].mainTexture = (Texture)obj;
                count++;
                if (count == 4)
                {

                    //m_isLoadDone = true;
                    DoShowWord();
                }

            });
        }
        else
        {
            //Debug.LogError("!null");
            //m_isLoadDone = true;
            DoShowWord();
        }
    }

    void OnDisable()
    {
        //Debug.LogError("OnDisable");
        if (!SystemSwitch.DestroyResource)
            return;

        GetUIChild("DoorOfBuryOpenTipWord0").GetComponentsInChildren<UITexture>(true)[0].mainTexture = null;

        AssetCacheMgr.ReleaseResourceImmediate("t_e.png");
        AssetCacheMgr.ReleaseResourceImmediate("t_m.png");
        AssetCacheMgr.ReleaseResourceImmediate("t_j.png");
        AssetCacheMgr.ReleaseResourceImmediate("t_l.png");

        //m_isLoadDone = false;
    }

    private void DoShowWord()
    {
        //Debug.LogError("doShowWord");
        for (int i = 0; i < m_wordsTween.Count; i++)
        {
            m_wordsTween[i].gameObject.SetActive(false);
            uint time = (uint)(m_wordsTween[i].duration * 1000f);
            var index = (uint)i;
            TimerHeap.AddTimer(index * time, 0, () =>
            {
                m_wordsTween[(int)index].gameObject.SetActive(true);
                m_wordsTween[(int)index].enabled = true;
                m_wordsTween[(int)index].Reset();
                m_wordsTween[(int)index].Play(true);
                if (index == m_wordsTween.Count - 1)
                {
                    TimerHeap.AddTimer(500, 0, () =>
                    {
                        gameObject.SetActive(false);
                        if (m_onDone != null)
                            m_onDone();
                    });

                }
            });
        }
    }

    public void ShowWord(bool isShow, Action OnDone)
    {
        //Debug.LogError("showWord1");
        if (!isShow) return;
        //Debug.LogError("showWord2");
        m_onDone = OnDone;
        gameObject.SetActive(true);



    }
}