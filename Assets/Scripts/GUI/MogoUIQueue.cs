using UnityEngine;
using System.Collections;
using Mogo.Util;
using MS;
using System;
using System.Collections.Generic;
using System.Linq;

public class MogoUIQueue : MonoBehaviour
{
    private static MogoUIQueue m_instance;

    public static MogoUIQueue Instance
    {
        get
        {
            return MogoUIQueue.m_instance;
        }
    }

    private uint m_nextId;
    // 为去除警告暂时屏蔽以下代码
    //private bool m_isFree = true;
    public List<MogoUIQueueUnit> m_listUI = new List<MogoUIQueueUnit>();

    public bool IsLocking = false;

    void Awake()
    {
        m_instance = transform.GetComponentsInChildren<MogoUIQueue>(true)[0];

        EventDispatcher.AddEventListener<GameObject>("CurrentUIChange", OnCurrentUIChange);
    }

    public void Release()
    {
        EventDispatcher.RemoveEventListener<GameObject>("CurrentUIChange", OnCurrentUIChange);
    }

    public void CheckQueue()
    {
        if (!Locked)
        {
            for (int i = 0; i < m_listUI.Count; ++i)
            {
                if (m_listUI[i].BaseUI == MogoUIManager.Instance.CurrentUI)
                {
                    if (!IsLocking)
                    {
                        Mogo.Util.LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@CheckQueue ");
                        m_listUI[i].JustDoIt();
                        m_listUI.Remove(m_listUI[i]);
                        break;
                    }
                }
            }
        }
    }

    void OnCurrentUIChange(GameObject currentUI)
    {
        Locked = false;
        for (int i = 0; i < m_listUI.Count; ++i)
        {
            if (m_listUI[i].BaseUI == currentUI)
            {
                if (!IsLocking)
                {
                    Mogo.Util.LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@OnCurrentUIChange ");

                    MogoFXManager.Instance.ReleaseAllParticleAnim();
                    m_listUI[i].JustDoIt();
                    m_listUI.Remove(m_listUI[i]);
                    break;
                }
            }
        }

    }

    public bool Locked = false;

    public void PushOne(Action action, GameObject baseUI = null, string unitName = "", ulong pri = 0)
    {

        Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@Pushing " + unitName + " " +
            Locked + " " + IsLocking + " baseUI = " + baseUI + " currentUI = " + MogoUIManager.Instance.CurrentUI);

        var unit = new MogoUIQueueUnit() { Id = ++m_nextId, act = action, Priority = pri, BaseUI = baseUI };

        if (baseUI == null)
        {
            Debug.Log(unitName + " Run 1");
            unit.JustDoIt();
        }
        else if (baseUI == MogoUIManager.Instance.CurrentUI && !Locked && !IsLocking)
        {
            Debug.Log(unitName + " Run 2");
            Locked = true;
            unit.JustDoIt();
        }
        else
        {
            Debug.Log(unitName + " Run 3");
            m_listUI.Add(unit);

            m_listUI = m_listUI.OrderByDescending(t => t.Priority).ToList();
        }
    }
}
