using UnityEngine;
using System.Collections;

public class OperatingUILogicManager
{
    private static OperatingUILogicManager m_instance;

    public static OperatingUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new OperatingUILogicManager();
            }

            return OperatingUILogicManager.m_instance;

        }
    }

    int m_currentPage = (int)OperatingUITab.NoneTab;
    public int CurrentPage
    {
        get
        {
            return m_currentPage;
        }
        set
        {
            OperatingUIViewManager.Instance.HandleTabChange(m_currentPage, value);
            m_currentPage = value;
        }
    }

    public void Initialize()
    {
        CurrentPage = (int)OperatingUITab.NoneTab; 
    }

    public void Release()
    {
        CurrentPage = (int)OperatingUITab.NoneTab; 
    }
}
