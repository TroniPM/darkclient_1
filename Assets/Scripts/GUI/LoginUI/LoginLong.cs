/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：LoginLong
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;

public class LoginLong : MonoBehaviour
{
    private GameObject m_onHitPoint;
    private GameObject m_lastFx = null;
    void Start()
    {
        InvokeRepeating("playAni", 0f, 20f);
        m_onHitPoint = GameObject.Find("OnHitPoint");
    }

    void playAni()
    {
        if (GetComponent<Animation>() != null)
        {
            GetComponent<Animation>().Play(AnimationPlayMode.Stop);
            Invoke("onHit", 8.1f);
        }
    }

    void onHit()
    {
        //得到特效资源
        AssetCacheMgr.GetInstance("fx_scenes_long01.prefab", (str, id, obj) =>
        {
            if (this == null) return;
            if (obj == null) return;
            //挂到目标上
            GameObject go = obj as GameObject;
            if (m_onHitPoint == null) return;
            Utils.MountToSomeObjWithoutPosChange(go.transform, m_onHitPoint.transform);
            //卸装之前的
            if (m_lastFx != null)
            {
                AssetCacheMgr.ReleaseInstance(m_lastFx);
                m_lastFx = null;
            }
            //保存现在的
            m_lastFx = go;
        });
    }


    void OnDestroy()
    {
        if (m_lastFx != null)
        {
            AssetCacheMgr.ReleaseInstance(m_lastFx);
            m_lastFx = null;
        }
        m_onHitPoint = null;
    }
}