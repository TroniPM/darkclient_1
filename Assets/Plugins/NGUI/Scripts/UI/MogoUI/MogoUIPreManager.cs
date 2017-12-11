using UnityEngine;
using System.Collections;
using Mogo.Util;

public class MogoUIPreManager 
{

    private static MogoUIPreManager m_instance;

    public static MogoUIPreManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new MogoUIPreManager();
            }

            return MogoUIPreManager.m_instance;

        }
    }

    public void PreShowInstanceUI()
    {
    }

    public void PreShowChooseServerUI()
    {
 
    }
}
