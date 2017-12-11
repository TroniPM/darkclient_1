/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MogoGUIViewManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;

public class MogoGUIViewManager : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    private static MogoGUIViewManager m_instance;

    public static MogoGUIViewManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = Object.FindObjectOfType(typeof(MogoGUIViewManager)) as MogoGUIViewManager;

                if (m_instance == null)
                {
                    GameObject go = new GameObject("_MogoGUIViewManager");
                    DontDestroyOnLoad(go);
                    m_instance = go.AddComponent<MogoGUIViewManager>();
                }
            }

            return MogoGUIViewManager.m_instance;
        }
    }

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    public static Dictionary<string, string> ButtonTypeToEventUp = new Dictionary<string, string>();

    public static Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    private Transform m_myTransform;

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        LoggerHelper.Debug(widgetName + " " + fullName);
        m_widgetToFullName.Add(widgetName, fullName);
    }

    private string GetFullName(Transform currentTransform)
    {
        string fullName = "";

        while (currentTransform != m_myTransform)
        {
            fullName = currentTransform.name + fullName;

            if (currentTransform.parent != m_myTransform)
            {
                fullName = "/" + fullName;
            }

            currentTransform = currentTransform.parent;
        }

        return fullName;
    }

    public void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }

    public void SetUIText(string UIName, string text)
    {
        var l = m_myTransform.Find(UIName).GetComponentsInChildren<UILabel>(true);
        if (l != null)
        {
            l[0].text = text;
            l[0].transform.localScale = new Vector3(18, 18, 18);
        }
    }

    public void SetUITexture(string UIName, string imageName)
    {
        var s = m_myTransform.Find(UIName).GetComponentsInChildren<UISlicedSprite>(true);
        if (s != null)
            s[0].spriteName = imageName;
    }

    // 初始化
    void Start()
    {
        m_myTransform = transform;

        //ButtonTypeToEventDown.Add("Special", MainUILogicManager.MainUIEvent.SPECIALDOWN);
        //ButtonTypeToEventDown.Add("Move", MainUILogicManager.MainUIEvent.MOVEDOWN);
        //ButtonTypeToEventDown.Add("Affect", MainUILogicManager.MainUIEvent.AFFECTDOWN);
        //ButtonTypeToEventDown.Add("Output", MainUILogicManager.MainUIEvent.OUTPUTDOWN);
        //ButtonTypeToEventDown.Add("NormalAttack", MainUILogicManager.MainUIEvent.NORMALATTACTDOWN);
        //ButtonTypeToEventDown.Add("Task", MainUILogicManager.MainUIEvent.TASKDOWN);
        //ButtonTypeToEventDown.Add("Community", MainUILogicManager.MainUIEvent.COMMUNITYDOWN);
        //ButtonTypeToEventDown.Add("PlayerInfo", MainUILogicManager.MainUIEvent.PLAYERINFODOWN);

        //ButtonTypeToEventUp.Add("Special", MainUILogicManager.MainUIEvent.SPECIALUP);
        //ButtonTypeToEventUp.Add("Move", MainUILogicManager.MainUIEvent.MOVEUP);
        //ButtonTypeToEventUp.Add("Affect", MainUILogicManager.MainUIEvent.AFFECTUP);
        //ButtonTypeToEventUp.Add("Output", MainUILogicManager.MainUIEvent.OUTPUTUP);
        //ButtonTypeToEventUp.Add("NormalAttack", MainUILogicManager.MainUIEvent.NORMALATTACTUP);
        //ButtonTypeToEventUp.Add("Task", MainUILogicManager.MainUIEvent.TASKUP);
        //ButtonTypeToEventUp.Add("Community", MainUILogicManager.MainUIEvent.COMMUNITYUP);
        //ButtonTypeToEventUp.Add("PlayerInfo", MainUILogicManager.MainUIEvent.PLAYERINFOUP);
    }
}
