using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;
public class OneKeyGetBlessGrid : MogoParentUI
{
    private Transform m_Transform;

    private GameObject m_nameObj;

    void Awake()
    {
        m_Transform = transform;        
        m_nameObj = m_Transform.Find("ArenaUIListGridName").gameObject;
      
    }

    void Start()
    {
        Name = Name;
    }


    #region 界面信息

    /// <summary>
    /// 积分奖励名称
    /// </summary>
    private string m_name;
    public string Name
    {
        get { return m_name; }
        set
        {
            m_name = value;
            if (m_nameObj != null)
            {
                m_nameObj.GetComponent<UILabel>().text = value;
            }
        }
    } 

    #endregion
}
