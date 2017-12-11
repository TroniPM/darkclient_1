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
using System;
using Mogo.GameData;

public class ChooseDragonUIGrid : MogoUIBehaviour
{    
    private UISprite m_spChooseDragonUIGrid;
    private UILabel m_lblChooseDragonUIGridTitleName;
    private UILabel m_lblChooseDragonUIGridFinishTime;
    private UILabel m_lblChooseDragonUIGridAdditionReward;
    private GameObject m_goChooseDragonUIGridBtnBuy;
    private GameObject m_goChooseDragonUIGridBGSelected;
    private bool m_bLoadResourceInsteadOfAwake = false;

    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_spChooseDragonUIGrid = FindTransform("ChooseDragonUIGridSprite").GetComponentsInChildren<UISprite>(true)[0];
        m_lblChooseDragonUIGridTitleName = FindTransform("ChooseDragonUIGridTitleName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblChooseDragonUIGridFinishTime = FindTransform("ChooseDragonUIGridFinishTime").GetComponentsInChildren<UILabel>(true)[0];
        m_lblChooseDragonUIGridAdditionReward = FindTransform("ChooseDragonUIGridAdditionReward").GetComponentsInChildren<UILabel>(true)[0];
        m_goChooseDragonUIGridBtnBuy = FindTransform("ChooseDragonUIGridBtnBuy").gameObject;
        m_goChooseDragonUIGridBGSelected = FindTransform("ChooseDragonUIGridBGSelected").gameObject;

        m_goChooseDragonUIGridBtnBuy.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnBuyUp;
    }

    #region 事件

    public int DragonID;
    readonly static int TheLastDragonQuality = 6;

    private void OnBtnBuyUp()
    {
        if (ChooseDragonUILogicManager.CHOOSEDRAGONUIGRIDBUY != null)
            ChooseDragonUILogicManager.CHOOSEDRAGONUIGRIDBUY(DragonID);
    }

    #endregion

    public void SetTitleAndImage(bool dragonEnable, int dragonQuality = 0)
    {
        SetTitle(dragonEnable, dragonQuality);
        SetImage(dragonEnable, dragonQuality);      
    }

    private void SetImage(bool dragonEnable, int dragonQuality)
    {
        m_spChooseDragonUIGrid.ShowAsWhiteBlack(false);
        m_spChooseDragonUIGrid.ShowAsWhiteBlack(!dragonEnable);
        m_goChooseDragonUIGridBGSelected.SetActive(dragonEnable);
        if (dragonEnable)
        {
            MogoUtils.SetImageColor(m_spChooseDragonUIGrid, dragonQuality);
        }
    }

    private void SetTitle(bool dragonEnable, int dragonQuality)
    {
        if (dragonEnable || dragonQuality == TheLastDragonQuality)
        {
            m_lblChooseDragonUIGridTitleName.text = DragonQualityData.GetDragonQualityData(dragonQuality).GetName(true);
            m_lblChooseDragonUIGridTitleName.effectStyle = UILabel.Effect.Outline;
            m_lblChooseDragonUIGridTitleName.effectColor = new Color32(53, 22, 2, 255);
            m_lblChooseDragonUIGridTitleName.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            m_lblChooseDragonUIGridTitleName.text = DragonQualityData.GetDragonQualityData(dragonQuality).GetName(false);
            m_lblChooseDragonUIGridTitleName.effectStyle = UILabel.Effect.None;
            m_lblChooseDragonUIGridTitleName.color = new Color32(63, 27, 4, 255);
        }        
    }

    /// <summary>
    /// 设置完成时间
    /// </summary>
    /// <param name="time"></param>
    public void SetFinishTime(string time, int dragonQuality)
    {
        if (dragonQuality == TheLastDragonQuality)
        {
            string qualityColor = ItemParentData.GetQualityColorByQuality(TheLastDragonQuality);
            time = string.Concat("[", qualityColor, "]", time, "[-]");

            m_lblChooseDragonUIGridFinishTime.effectStyle = UILabel.Effect.Outline;
            m_lblChooseDragonUIGridFinishTime.effectColor = new Color32(53, 22, 2, 255);
            m_lblChooseDragonUIGridFinishTime.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            m_lblChooseDragonUIGridFinishTime.effectStyle = UILabel.Effect.None;
            m_lblChooseDragonUIGridFinishTime.color = new Color32(63, 27, 4, 255);
        }
    
        m_lblChooseDragonUIGridFinishTime.text = time;
    }

    /// <summary>
    /// 设置附加奖励
    /// </summary>
    /// <param name="additionReward"></param>
    public void SetAdditionReward(string additionReward, int dragonQuality)
    {
        if (dragonQuality == TheLastDragonQuality)
        {
            string qualityColor = ItemParentData.GetQualityColorByQuality(TheLastDragonQuality);
            additionReward = string.Concat("[", qualityColor, "]", additionReward, "[-]");

            m_lblChooseDragonUIGridAdditionReward.effectStyle = UILabel.Effect.Outline;
            m_lblChooseDragonUIGridAdditionReward.effectColor = new Color32(53, 22, 2, 255);
            m_lblChooseDragonUIGridAdditionReward.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            m_lblChooseDragonUIGridAdditionReward.effectStyle = UILabel.Effect.None;
            m_lblChooseDragonUIGridAdditionReward.color = new Color32(63, 27, 4, 255);
        }

        m_lblChooseDragonUIGridAdditionReward.text = additionReward;
    }

    /// <summary>
    /// 是否显示购买按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBuy(bool isShow)
    {
        m_goChooseDragonUIGridBtnBuy.SetActive(isShow);
    }
}
