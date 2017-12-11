#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：材料获得方式
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;

public class MaterialObtainTip : MogoUIBehaviour 
{
    GameObject m_goMaterialObtainTipMat1Pos;
    GameObject m_goMaterialObtainTipMat2Pos;

    GameObject m_goGOMaterialObtainNormalCrystal;
    GameObject m_goGOMaterialObtainPerfectCrystal;
    GameObject m_goMaterialObtainTipBGLine;

    // 普通水晶
    private UISprite m_spNormalItemBG;
    private UISprite m_spNormalItemFG;
    private UILabel m_lblNormalItemText;
    private UILabel m_lblNormalEquipmentText;

    // 完美水晶
    private UISprite m_spPerfectItemBG;
    private UISprite m_spPerfectItemFG;
    private UILabel m_lblPerfectItemText;
    private GameObject m_goPerfectArenaReqLevelText;
    private UILabel m_lblPerfectArenaReqLevelText;


    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);
        
        m_goMaterialObtainTipMat1Pos = m_myTransform.Find(m_widgetToFullName["MaterialObtainTipMat1Pos"]).gameObject;
        m_goMaterialObtainTipMat2Pos = m_myTransform.Find(m_widgetToFullName["MaterialObtainTipMat2Pos"]).gameObject;
        m_goGOMaterialObtainNormalCrystal = m_myTransform.Find(m_widgetToFullName["GOMaterialObtainNormalCrystal"]).gameObject;
        m_goGOMaterialObtainPerfectCrystal = m_myTransform.Find(m_widgetToFullName["GOMaterialObtainPerfectCrystal"]).gameObject;
        m_goMaterialObtainTipBGLine = m_myTransform.Find(m_widgetToFullName["MaterialObtainTipBGLine"]).gameObject;

        m_spNormalItemBG = FindTransform("NormalItemBG").GetComponentsInChildren<UISprite>(true)[0];
        m_spNormalItemFG = FindTransform("NormalItemFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblNormalItemText = FindTransform("NormalItemText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblNormalEquipmentText = FindTransform("NormalEquipmentText").GetComponentsInChildren<UILabel>(true)[0];

        m_spPerfectItemBG = FindTransform("PerfectItemBG").GetComponentsInChildren<UISprite>(true)[0];
        m_spPerfectItemFG = FindTransform("PerfectItemFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblPerfectItemText = FindTransform("PerfectItemText").GetComponentsInChildren<UILabel>(true)[0];
        m_goPerfectArenaReqLevelText = FindTransform("PerfectArenaReqLevelText").gameObject;
        m_lblPerfectArenaReqLevelText = m_goPerfectArenaReqLevelText.GetComponentsInChildren<UILabel>(true)[0];

        Initialize();
    }

    #region 事件

    void Initialize()
    {
        StrenthUIDict.ButtonTypeToEventUp.Add("MaterialObtainTipBtnClose", OnMaterialObtainTipCloseUp);
    }

    void OnMaterialObtainTipCloseUp()
    {
        if (StrenthUIDict.MATERIALOBTAINTIPCLOSEUP != null)
        {
            StrenthUIDict.MATERIALOBTAINTIPCLOSEUP();
        }
    }

    #endregion  

    public void ShowMaterial(bool refresh, int itemId1, int itemId2 = 0)
    {
        if (refresh == false)
            return;

        m_goGOMaterialObtainNormalCrystal.SetActive(false);
        m_goGOMaterialObtainPerfectCrystal.SetActive(false);
        m_goMaterialObtainTipBGLine.SetActive(false);
        
        // subtype = 1 普通；subtype = 2 完美
        ItemParentData data1 = ItemParentData.GetItem(itemId1);
        if (data1 != null)
        {
            if (data1.subtype == 1)
            {
                m_goGOMaterialObtainNormalCrystal.SetActive(true);
                m_goGOMaterialObtainNormalCrystal.transform.localPosition = m_goMaterialObtainTipMat1Pos.transform.localPosition;

                InventoryManager.SetIcon(itemId1, m_spNormalItemFG);
                SetNormalItemText(data1.level);
                SetNormalEquipmentText(data1.level);
            }
            else if (data1.subtype == 2)
            {
                m_goGOMaterialObtainPerfectCrystal.SetActive(true);
                m_goGOMaterialObtainPerfectCrystal.transform.localPosition = m_goMaterialObtainTipMat1Pos.transform.localPosition;

                InventoryManager.SetIcon(itemId1, m_spPerfectItemFG);
                SetPerfectItemText(data1.level);
                SetPerfectArenaReqLevel();
            }
        }

        if (itemId2 > 0)
        {
            ItemParentData data2 = ItemParentData.GetItem(itemId2);
            if (data2.subtype == 1)
            {
                m_goGOMaterialObtainNormalCrystal.SetActive(true);
                m_goGOMaterialObtainNormalCrystal.transform.localPosition = m_goMaterialObtainTipMat2Pos.transform.localPosition;
                m_goMaterialObtainTipBGLine.SetActive(true);

                InventoryManager.SetIcon(itemId2, m_spNormalItemFG);
                SetNormalItemText(data2.level);
                SetNormalEquipmentText(data2.level);
            }
            else if (data2.subtype == 2)
            {
                m_goGOMaterialObtainPerfectCrystal.SetActive(true);
                m_goGOMaterialObtainPerfectCrystal.transform.localPosition = m_goMaterialObtainTipMat2Pos.transform.localPosition;
                m_goMaterialObtainTipBGLine.SetActive(true);

                InventoryManager.SetIcon(itemId2, m_spPerfectItemFG);
                SetPerfectItemText(data2.level);
                SetPerfectArenaReqLevel();
            }
        }
    }

    /// <summary>
    /// 设置普通水晶title
    /// </summary>
    /// <param name="level"></param>
    private void SetNormalItemText(int level)
    {
        m_lblNormalItemText.text = string.Format(LanguageData.GetContent(48000), level);
    }

    /// <summary>
    /// 设置普通水晶需要分解的装备等级
    /// </summary>
    private void SetNormalEquipmentText(int level)
    {
        int minLevel = 0;
        int maxLevel = 0;

        switch (level)
        {
            case 1:
                minLevel = 1;
                maxLevel = 19;
                break;
            case 2:
                minLevel = 20;
                maxLevel = 29;
                break;
            case 3:
                minLevel = 30;
                maxLevel = 39;
                break;
            case 4:
                minLevel = 40;
                maxLevel = 49;
                break;
            case 5:
                minLevel = 50;
                maxLevel = 70;
                break;
            default:
                break;
        }

        m_lblNormalEquipmentText.text = string.Format(LanguageData.GetContent(48002), minLevel, maxLevel);
    }

    /// <summary>
    /// 设置完美水晶title
    /// </summary>
    /// <param name="level"></param>
    private void SetPerfectItemText(int level)
    {
        m_lblPerfectItemText.text = string.Format(LanguageData.GetContent(48001), level);
    }

    /// <summary>
    /// 完美水晶不足,设置竞技场需求等级
    /// </summary>
    private void SetPerfectArenaReqLevel()
    {
        if (MogoWorld.thePlayer.level < SystemRequestLevel.ArenaIcon)
        {
            m_lblPerfectArenaReqLevelText.text = string.Format(LanguageData.GetContent(48003), SystemRequestLevel.ArenaIcon);
            m_goPerfectArenaReqLevelText.SetActive(true);
        }
        else
        {
            m_goPerfectArenaReqLevelText.SetActive(false);
        }
    }
}
