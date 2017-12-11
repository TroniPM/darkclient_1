using System;
using System.Collections;
using System.Linq;

public class MFUIQueueManager
{

    private static MFUIQueueManager m_instance;

    public static MFUIQueueManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new MFUIQueueManager();
            }

            return m_instance;
        }
    }

    private System.Collections.Generic.List<MFUIQueueUnit> m_listUI = new System.Collections.Generic.List<MFUIQueueUnit>();

    private uint m_nextUnitID;

    private bool IsLocking = false;

    public void Initialize()
    {
 
    }

    public void LockUIQueue()
    {
        IsLocking = true;
    }

    public void UnlockUIQueue()
    {
        IsLocking = false;
    }

    public void CheckUIQueue()
    {
        for (int i = 0; i < m_listUI.Count; ++i)
        {
            if (m_listUI[i].BaseUIID == MFUIManager.CurrentUI)
            {
                if (!IsLocking)
                {
                    MFUIUtils.MFUIDebug("CheckUIQueue Got One !!!");
                    m_listUI[i].JustDoIt();
                    m_listUI.Remove(m_listUI[i]);
                    break;
                }
            }
        }
    }

    public void PushOne(Action action,MFUIManager.MFUIID baseUI = MFUIManager.MFUIID.None,
        uint pri = 0,string debugText = "")
    {
        MFUIQueueUnit unit = new MFUIQueueUnit()
        {
            UnitID = ++m_nextUnitID,
            act = action,
            UnitPriority = pri,
            BaseUIID = baseUI
        };

        if (baseUI == MFUIManager.MFUIID.None)
        {
            MFUIUtils.MFUIDebug(string.Concat(debugText, " Run 1"));
            unit.JustDoIt();
        }
        else if (baseUI == MFUIManager.CurrentUI && !IsLocking)
        {
            MFUIUtils.MFUIDebug(string.Concat(debugText, " Run 2"));
            unit.JustDoIt();
        }
        else
        {
            MFUIUtils.MFUIDebug(string.Concat(debugText, " Run 3"));
            m_listUI.Add(unit);

            m_listUI = m_listUI.OrderByDescending(t => t.UnitPriority).ToList();
        }
    }
}
