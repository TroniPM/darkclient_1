#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;

public class NewInstanceLevelGrid : MonoBehaviour {

    public int id;
    public string parentNameFlag = "InstanceLevelChooseLevel";

    private Dictionary<string, string> m_widgetToFullName = new Dictionary<string, string>();

    public void AddWigetToFullNameData(string widgetName, string fullName)
    {
        if (m_widgetToFullName.ContainsKey(widgetName))
            LoggerHelper.Debug(widgetName);
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
        for (int i = 0; i < rootTransform.GetChildCount(); ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            FillFullNameData(rootTransform.GetChild(i));
        }
    }

    Transform m_myTransform;

    GameObject m_goInstanceLevelLock;
    UISprite m_spInstanceLevelBGDown;
    UISprite m_spInstanceLevelBGUp;

    void Awake()
    {
        m_myTransform = transform;
        FillFullNameData(m_myTransform);  		
		
    }
	
	public void MyStart()
	{
        m_goInstanceLevelLock = m_myTransform.Find(parentNameFlag + id + "Lock").gameObject;
        m_spInstanceLevelBGDown = m_myTransform.Find(parentNameFlag + id + "BGDown").GetComponent<UISprite>();
        m_spInstanceLevelBGUp = m_myTransform.Find(parentNameFlag + id + "BGUp").GetComponent<UISprite>();
	}

    /// <summary>
    /// 设置是否可用
    /// </summary>
    /// <param name="enable"></param>
    public void SetEnable(bool enable)
    {
        //m_spInstanceLevelBGUp.ShowAsWhiteBlack(!enable);
        //m_goInstanceLevelLock.GetComponentsInChildren<UISprite>(true)[0].ShowAsWhiteBlack(!enable);        

        gameObject.GetComponentsInChildren<BoxCollider>(true)[0].enabled = enable;
        if (enable)
        {
            m_spInstanceLevelBGUp.spriteName = "btn_01up";
            m_goInstanceLevelLock.SetActive(false);
        }
        else
        {
            m_spInstanceLevelBGUp.spriteName = "btn_03grey";
            m_goInstanceLevelLock.SetActive(true);
        }
    }

    /// <summary>
    /// 设置是否选中
    /// </summary>
    /// <param name="isChoose"></param>
    public void SetChoose(bool isChoose)
    {
        if (isChoose)
            gameObject.transform.parent.GetComponent<MogoSingleButtonList>().SetCurrentDownButton(id);
    }
}
