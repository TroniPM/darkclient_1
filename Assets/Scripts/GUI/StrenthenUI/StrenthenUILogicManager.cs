/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StrenthenUILogicManager
// 创建者：MaiFeo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using Mogo.Util;
public class StrenthenUILogicManager
{
    #region 公共变量

    #endregion

    #region 私有变量

    #endregion


    private static StrenthenUILogicManager m_instance;

    public static StrenthenUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new StrenthenUILogicManager();
            }

            return StrenthenUILogicManager.m_instance;

        }
    }

    void OnStrenthenUp()
    {
        LoggerHelper.Debug("StrenthenUp");
        EventDispatcher.TriggerEvent(BodyEnhanceManager.ON_ENHANCE);
    }

    void OnStrenthenEquipmentUp(int id)
    {
        LoggerHelper.Debug(id + " Up");
        EventDispatcher.TriggerEvent<int>(BodyEnhanceManager.ON_SELECT_SLOT, id);

    }

    void OnMaterialTipUp()
    {
        StrenthenUIViewManager.Instance.IsChangeMaterialTipBG = false;
        StrenthenUIViewManager.Instance.ShowMaterialObtainTip(true);
    }

    void OnMaterialObtainTipCloseUp()
    {
        StrenthenUIViewManager.Instance.ShowMaterialObtainTip(false);
    }

    public void Initialize()
    {
        StrenthUIDict.STRENTHENUP += OnStrenthenUp;
        StrenthUIDict.STRENTHENEQUIPMENTGRIDUP += OnStrenthenEquipmentUp;
        StrenthUIDict.MATERIALTIPUP += OnMaterialTipUp;
        StrenthUIDict.MATERIALOBTAINTIPCLOSEUP += OnMaterialObtainTipCloseUp;
    }

    public void Release()
    {

        StrenthUIDict.STRENTHENUP -= OnStrenthenUp;
        StrenthUIDict.STRENTHENEQUIPMENTGRIDUP -= OnStrenthenEquipmentUp;
        StrenthUIDict.MATERIALTIPUP -= OnMaterialTipUp;
        StrenthUIDict.MATERIALOBTAINTIPCLOSEUP -= OnMaterialObtainTipCloseUp;
    }
}
