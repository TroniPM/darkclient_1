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
using System.Collections.Generic;
using Mogo.Util;

public class ModelShowCaseLogicManager 
{
    private static ModelShowCaseLogicManager m_instance;
    public static ModelShowCaseLogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ModelShowCaseLogicManager();
            }

            return ModelShowCaseLogicManager.m_instance;
        }
    }

    // 模型列表
    public Dictionary<int, EtyAvatar> AvatarList = new Dictionary<int, EtyAvatar>();
    public Dictionary<int, UIViewport> ViewportList = new Dictionary<int, UIViewport>();

    /// <summary>
    /// 显示或隐藏模型
    /// </summary>
    /// <param name="vocation"></param>
    /// <param name="isShow"></param>
    public void ShowModel(int vocation, bool isShow)
    {
        if (AvatarList.ContainsKey(vocation))
        {
            if (AvatarList[vocation].gameObject != null)
                AvatarList[vocation].gameObject.SetActive(isShow);
            else
                LoggerHelper.Debug("Model gameObject is null !");
        }
    }

    /// <summary>
    /// 隐藏所有的模型
    /// </summary>
    public void CloseAllModel()
    {
        if (AvatarList != null)
        {
            foreach (var item in AvatarList)
            {
                if (item.Value.gameObject != null)
                    item.Value.gameObject.SetActive(false);
                else
                    LoggerHelper.Debug("Model gameObject is null !");
            }
        }       
    }  
}
