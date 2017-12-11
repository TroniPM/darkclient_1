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
using Mogo.GameData;
using System;

//按玩家情况，显示分为四种状态
//状态一：玩家未达到该副本等级，为未解锁状态
//状态二：玩家解锁该BOSS，但未S通关，为解锁态
//状态三：玩家S通关，但是未领取奖励，为未领奖态
//状态四：玩家S级通关，并且领完奖励
public enum BossTreasureStatus
{
    LevelNoEnough = 1,
    NoHasStarS = 2,
    CanGetReward = 3,
    HasGotReward = 4,
}


public class InstanceBossTreasureUIGrid : MogoUIBehaviour
{
    private UISprite m_spBossFG;
    private UILabel m_lblBossName;

    private GameObject m_goGOBossTreasure;
    private UISprite m_spBossTreasureImageUp;
    private UISprite m_spBossTreasureImageDown;

    private GameObject m_goGOFlag;
    private UISprite m_spFlagImage;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);
		
        m_spBossFG = FindTransform("BossFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblBossName = FindTransform("BossName").GetComponentsInChildren<UILabel>(true)[0];
        m_goGOBossTreasure = FindTransform("GOBossTreasure").gameObject;
        m_spBossTreasureImageUp = FindTransform("BossTreasureImageUp").GetComponentsInChildren<UISprite>(true)[0];
        m_spBossTreasureImageDown = FindTransform("BossTreasureImageDown").GetComponentsInChildren<UISprite>(true)[0];
        m_goGOFlag = FindTransform("GOFlag").gameObject;
        m_spFlagImage = FindTransform("FlagImage").GetComponentsInChildren<UISprite>(true)[0];        

        Initialize();
    }

    #region 事件

    void Initialize()
    {
        FindTransform("GOBossTreasure").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBossTreasureUp;
        FindTransform("GOBossTreasure").GetComponentsInChildren<MogoButton>(true)[0].pressHandler += OnBossTreasurePress;
    }

    void OnClick()
    {
        if (Status == BossTreasureStatus.CanGetReward)
        {
            if (InstanceBossTreasureUIDict.BOSSTREASUREBTNUP != null)
                InstanceBossTreasureUIDict.BOSSTREASUREBTNUP(BossID);
        }
    }

    void OnBossTreasurePress(bool isPressed)
    {
        if (isPressed)
        {
            if (InstanceBossTreasureUIDict.BOSSREWARDUP != null)
                InstanceBossTreasureUIDict.BOSSREWARDUP(BossID);
        }
        else
        {
            if (InstanceBossTreasureUIViewManager.Instance != null)
                InstanceBossTreasureUIViewManager.Instance.ShowInstanceBossTreasureUIRewardUI(false);    
        }    
    }

    void OnBossTreasureUp()
    {
        switch (Status)
        {
            case BossTreasureStatus.LevelNoEnough:
                {
                    MogoMsgBox.Instance.ShowFloatingText(string.Format(LanguageData.GetContent(46977), RequestLevel));
                } break;
            case BossTreasureStatus.NoHasStarS:
                {
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(46978));
                } break;
            case BossTreasureStatus.CanGetReward:
                {
                    if (InstanceBossTreasureUIDict.BOSSTREASUREBTNUP != null)
                        InstanceBossTreasureUIDict.BOSSTREASUREBTNUP(BossID);
                } break;
            case BossTreasureStatus.HasGotReward:
                {
             
                } break;
        }
    }

    /// <summary>
    /// Boss宝箱BossID
    /// </summary>
    private int m_bossID;
    private int BossID
    {
        get { return m_bossID; }
        set
        {
            m_bossID = value;        
        }
    }

    /// <summary>
    /// Boss宝箱状态
    /// </summary>
    private BossTreasureStatus m_status;
    public BossTreasureStatus Status
    {
        get { return m_status; }
        set
        {
            m_status = value;
            SetBossTreasureStatus(m_status);
        }
    }

    /// <summary>
    /// 挑战Boss需要的玩家等级
    /// </summary>
    private int m_requestLevel;
    public int RequestLevel
    {
        get { return m_requestLevel; }
        set
        {
            m_requestLevel = value;
        }
    }

    #endregion

    /// <summary>
    /// 设置Boss宝箱信息
    /// </summary>
    /// <param name="iconName"></param>
    /// <param name="bossName"></param>
    /// <param name="status"></param>
    public void SetBossInfo(int bossID, string iconName, string bossName, BossTreasureStatus status, int bossReqLevel = 0)
    {
        BossID = bossID;
        Status = status;
        RequestLevel = bossReqLevel;

        MogoUIManager.Instance.TryingSetSpriteName(iconName, m_spBossFG);     
        m_lblBossName.text = bossName;        
    }

    /// <summary>
    /// 设置Boss宝箱状态
    /// </summary>
    /// <param name="status"></param>
    private void SetBossTreasureStatus(BossTreasureStatus status)
    {
        switch (status)
        {
            case BossTreasureStatus.LevelNoEnough:
                {
                    m_spBossFG.ShowAsWhiteBlack(true);

                    m_spBossTreasureImageUp.spriteName = "baoxiang03hui_close";
                    m_spBossTreasureImageDown.spriteName = "baoxiang03hui_close";
                    m_goGOBossTreasure.SetActive(true);
                    ShowBossTreasureFinishedFX(false);
                    ShowBossTreasureFinishedAnim(false);
                    
                    m_goGOFlag.SetActive(false);
                }break;
            case BossTreasureStatus.NoHasStarS:
                {
                    m_spBossFG.ShowAsWhiteBlack(false);

                    m_spBossTreasureImageUp.spriteName = "baoxiang01_close";
                    m_spBossTreasureImageDown.spriteName = "baoxiang01_close";
                    m_goGOBossTreasure.SetActive(true);
                    ShowBossTreasureFinishedFX(false);
                    ShowBossTreasureFinishedAnim(false);

                    m_goGOFlag.SetActive(false);
                }break;
            case BossTreasureStatus.CanGetReward:
                {
                    m_spBossFG.ShowAsWhiteBlack(false);
                    m_spBossTreasureImageUp.spriteName = "baoxiang01_close";
                    m_spBossTreasureImageDown.spriteName = "baoxiang01_close";
                    m_goGOBossTreasure.SetActive(true);
                    ShowBossTreasureFinishedFX(true);
                    ShowBossTreasureFinishedAnim(true);

                    m_spFlagImage.spriteName = "stg";
                    m_goGOFlag.SetActive(true);                    
                }break;
            case BossTreasureStatus.HasGotReward:
                {
                    m_spBossFG.ShowAsWhiteBlack(false);

                    m_spBossTreasureImageUp.spriteName = "baoxiang01_open";
                    m_spBossTreasureImageDown.spriteName = "baoxiang01_open";
                    m_goGOBossTreasure.SetActive(true);
                    ShowBossTreasureFinishedFX(false);
                    ShowBossTreasureFinishedAnim(false);

                    m_spFlagImage.spriteName = "jl_yiwancheng";
                    m_goGOFlag.SetActive(true);
                }break;
        }
    }

    #region Boss宝箱特效和动画

    /// <summary>
    /// Boss宝箱可领取特效
    /// </summary>
    /// <param name="isShow"></param>
    /// <returns></returns>
    private void ShowBossTreasureFinishedFX(bool isShow)
    {
        if (isShow)
        {
            AttachFXToBossTreasureUI(() =>
                {
                    if (m_goFinishFX != null)
                        m_goFinishFX.SetActive(true);
                });
        }
        else
        {
            ReleaseFXFromBossTreasureUI();
        }
    }

    /// <summary>
    /// 创建宝箱特效
    /// </summary>
    private GameObject m_goFinishFX = null;
    private void AttachFXToBossTreasureUI(Action action)
    {
        if (m_goFinishFX != null)
        {
            if (action != null)
                action();

            return;
        }

        if (InstanceBossTreasureUIViewManager.Instance != null)
            InstanceBossTreasureUIViewManager.Instance.AttachFXToBossTreasureUI(m_goGOBossTreasure.transform, (goFinishFX) =>
            {
                m_goFinishFX = goFinishFX;

                if (action != null)
                    action();
            });
    }

    /// <summary>
    /// 释放宝箱特效
    /// </summary>
    /// <param name="id"></param>
    public void ReleaseFXFromBossTreasureUI()
    {
        if(m_goFinishFX != null)
            AssetCacheMgr.ReleaseInstance(m_goFinishFX);
    }

    /// <summary>
    /// Boss宝箱可领取动画
    /// </summary>
    /// <param name="isShow"></param>
    /// <returns></returns>
    private void ShowBossTreasureFinishedAnim(bool isShow)
    {
        m_goGOBossTreasure.GetComponentsInChildren<TweenRotation>(true)[0].enabled = isShow;
        if (!isShow)
            ResetBossTreasureRotation();
    }

    private void ResetBossTreasureRotation()
    {
        m_goGOBossTreasure.transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    #endregion
}
