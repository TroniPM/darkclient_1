/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：CreateCharacterUIViewManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CreateCharacterUIViewManager : MonoBehaviour
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion

    public Action CREATECHARACTERUP;
    public Action CREATECHARACTERBACKUP;
    public Action CREATECHARACTERJOBGRID0UP;
    public Action CREATECHARACTERJOBGRID1UP;
    public Action CREATECHARACTERJOBGRID2UP;
    public Action CREATECHARACTERJOBGRID3UP;

    private static CreateCharacterUIViewManager m_instance;

    public static CreateCharacterUIViewManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject obj = GameObject.Find("MogoMainUIPanel");

                if (obj)
                {
                    m_instance = obj.transform.Find("CreateCharacterUI").GetComponentsInChildren<CreateCharacterUIViewManager>(true)[0];
                }
            }

            return CreateCharacterUIViewManager.m_instance;
        }
    }

    private Transform m_myTransform;
    private GameObject m_myGameObject;

    private GameObject m_chooseCharacterUI;

    private UILabel m_lblNewCharacterName;

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    public static Dictionary<string, Action> ButtonTypeToEventUp = new Dictionary<string, Action>();

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
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

    private void FillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.childCount; ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }

    private void SetUIText(string UIName, string text)
    {
        var l = m_myTransform.Find(UIName).GetComponentsInChildren<UILabel>(true);
        if (l != null)
        {
            l[0].text = text;
            l[0].transform.localScale = new Vector3(18, 18, 18);
        }
    }

    private void SetUITexture(string UIName, string imageName)
    {
        var s = m_myTransform.Find(UIName).GetComponentsInChildren<UISlicedSprite>(true);
        if (s != null)
            s[0].spriteName = imageName;
    }



    public string GetNewCharacterName()
    {
        return m_lblNewCharacterName.text;
    }

    void OnCreateCharacterUp()
    {
        if (CREATECHARACTERUP != null)
        {
            CREATECHARACTERUP();
        }
    }

    void OnCreateCharacterBackUp()
    {
        if (CREATECHARACTERBACKUP != null)
        {
            CREATECHARACTERBACKUP();
        }
    }

    void OnCreateCharacterJobGrid0Up()
    {
        if (CREATECHARACTERJOBGRID0UP != null)
        {
            CREATECHARACTERJOBGRID0UP();
        }
    }

    void OnCreateCharacterJobGrid1Up()
    {
        if (CREATECHARACTERJOBGRID1UP != null)
        {
            CREATECHARACTERJOBGRID1UP();
        }
    }

    void OnCreateCharacterJobGrid2Up()
    {
        if (CREATECHARACTERJOBGRID2UP != null)
        {
            CREATECHARACTERJOBGRID2UP();
        }
    }

    void OnCreateCharacterJobGrid3Up()
    {
        if (CREATECHARACTERJOBGRID3UP != null)
        {
            CREATECHARACTERJOBGRID3UP();
        }
    }

    void Awake()
    {
        gameObject.SetActive(false);

        Initialize();
        m_myGameObject = gameObject;
        m_myTransform = transform;

        FillFullNameData(m_myTransform);

         //m_chooseCharacterUI = m_myTransform.parent.FindChild("ChooseCharacterUI").gameObject;
        m_lblNewCharacterName = m_myTransform.Find(m_widgetToFullName["CreateCharacterUICharacterNameText"]).GetComponentsInChildren<UILabel>(true)[0];
       
    }
    
    void Initialize()
    {
        CreateCharacterUILogicManager.Instance.Initialize();

        ButtonTypeToEventUp.Add("CreateCharacterUICreate", OnCreateCharacterUp);
        ButtonTypeToEventUp.Add("CreateCharacterUIBack", OnCreateCharacterBackUp);
        ButtonTypeToEventUp.Add("CreateCharacterUIJobGrid0", OnCreateCharacterJobGrid0Up);
        ButtonTypeToEventUp.Add("CreateCharacterUIJobGrid1", OnCreateCharacterJobGrid1Up);
        ButtonTypeToEventUp.Add("CreateCharacterUIJobGrid2", OnCreateCharacterJobGrid2Up);
        ButtonTypeToEventUp.Add("CreateCharacterUIJobGrid3", OnCreateCharacterJobGrid3Up);
    }

    public void Release()
    {
        CreateCharacterUILogicManager.Instance.Release();

        ButtonTypeToEventUp.Clear();
    }
}
