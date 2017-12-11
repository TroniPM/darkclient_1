using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public enum ArenaTab
{
    Weak = 1,
    Strong = 2,
    Revenge = 3,
}

public class ArenaUILogicManager : UILogicManager
{
    #region 私有变量

    private static ArenaUILogicManager m_instance;
    private ArenaUIViewManager m_view;
    private int m_curTab=1;
    private Dictionary<int, ArenaPlayerData> m_playerData = new Dictionary<int, ArenaPlayerData>();
    private int m_weakMin;
    private int m_weakMax;
    private int m_strongMin;
    private int m_strongMax;

    #endregion

    #region 公共变量

    public static ArenaUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new ArenaUILogicManager();                
            }
            return m_instance;
        }
    }

    #endregion

    public void Initialize(ArenaUIViewManager view)
    {
        m_view = view;
        EventDispatcher.AddEventListener<int>(Events.ArenaEvent.TabSwitch, HandleTabChange);

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
        SetBinding<string>(EntityMyself.ATTR_NAME, m_view.SetNameText);
        SetBinding<uint>(EntityMyself.ATTR_ARENIC_CREDIT, m_view.SetProgressValue);
        SetBinding<ushort>(EntityMyself.ATTR_ARENIC_GRADE,m_view.SetHeadImage);
        SetBinding<ushort>(EntityMyself.ATTR_ARENIC_GRADE, m_view.SetCurTitleText);
        SetBinding<ushort>(EntityMyself.ATTR_ARENIC_GRADE, m_view.SetNextTitleText);
        SetBinding<uint>(EntityMyself.ATTR_FIGHT_FORCE, m_view.SetBattleForceText);
        SetBinding<uint>(EntityMyself.ATTR_GLOD, m_view.SetGoldNum);
        SetBinding<uint>(EntityMyself.ATTR_DIAMOND, m_view.SetDiamondNum);
    }
    private void HandleTabChange(int page)
    {
        if (m_curTab!=page)
        {
            m_view.HandleTabChange(m_curTab,page);
            m_curTab = page;
            SetArenaDesc();
            if(m_playerData.ContainsKey(m_curTab))
            {
                SetArenaPlayerData(m_playerData[m_curTab], m_curTab);
            }
        }
    }

    public void SetDefaultPage(int page)
    {
        if (m_curTab != page)
        {
            m_view.HandleTabChange(m_curTab, page);
            m_curTab = page;
        }
    }
    public void OnClose()
    {
        MogoUIManager.Instance.ShowMogoNormalMainUI();
        if (m_view != null)
        {
            m_view.CloseAllModel();
        }
        
    }
    public void OnEnter()
    {
        EventDispatcher.TriggerEvent<int>(Events.ArenaEvent.Challenge,m_curTab);
    }
    public void OnRefresh()
    {
        switch (m_curTab)
        {
            case 1:
                EventDispatcher.TriggerEvent(Events.ArenaEvent.RefreshWeak);
                break;
            case 2:
                EventDispatcher.TriggerEvent(Events.ArenaEvent.RefreshStrong);
                break;
            default:
                break;
        }
    }
 
    public void OnAddTimes()
    {
        MogoMessageBox.Confirm(LanguageData.GetContent(24002,
            PriceListData.dataMap.
            Get(8).priceList[1]),
            (flag) =>
        {
            if (flag)
                EventDispatcher.TriggerEvent(Events.ArenaEvent.AddArenaTimes);
        });
        
    }
    public void OnClearCD()
    {
        MogoMessageBox.Confirm(LanguageData.GetContent(812, 
            PriceListData.dataMap.
            Get(5).priceList[1] * Math.Ceiling(TimerManager.GetTimer(m_view.m_arenaCD).GetSeconds() / 60.0f)), 
            (flag) =>
        {
            if (flag)
                EventDispatcher.TriggerEvent(Events.ArenaEvent.ClearArenaCD);
        });
        
    }
    public void OnReward()
    {
        //OnClose();
        MogoUIManager.Instance.OpenWindow((int)WindowName.ArenaReward,
            () =>
            {
                EventDispatcher.TriggerEvent(Events.ArenaEvent.GetArenaRewardInfo);
            },
            null,
            true
            );

    }

    #region  鼓舞

    /// <summary>
    /// 鼓舞
    /// </summary>
    public void OnRevenge()
    {
        bool hasRevengeBuff = false;
        if (m_arenaPersonalData != null && m_arenaPersonalData.buff != 0)
            hasRevengeBuff = true;

        // 已鼓舞
        if (hasRevengeBuff)
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(25038));
        }
        // 未鼓舞
        else
        {
            if (m_view.IsShowArenaRevengeTipDialog)
            {
                // 设置需要消耗的钻石
                int costDiamond = PriceListData.dataMap.Get(9).priceList[1];
                if (costDiamond > MogoWorld.thePlayer.diamond)
                {
                    MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(170002));
                    return;
                }

                m_view.SetArenaUIUseInfoDiamondNum(costDiamond);
                m_view.ShowRevengeOKCancelBox(true);
            }
            else
            {
                EventDispatcher.TriggerEvent(Events.ArenaEvent.RefreshRevenge);
            }        
        }      
    }

    /// <summary>
    /// 取消鼓舞
    /// </summary>
    public void OnUseCancel()
    {
        m_view.ShowRevengeOKCancelBox(false);
    }

    /// <summary>
    /// 确定鼓舞
    /// </summary>
    public void OnUseOK()
    {
        EventDispatcher.TriggerEvent(Events.ArenaEvent.RefreshRevenge);
        m_view.ShowRevengeOKCancelBox(false);
    }

    /// <summary>
    /// 鼓舞不再提示框
    /// </summary>
    public void OnUseTipEnable()
    {
        m_view.IsShowArenaRevengeTipDialog = !m_view.IsShowArenaRevengeTipDialog;

        if (m_view.IsShowArenaRevengeTipDialog == false)
        {
            Mogo.Util.SystemConfig.Instance.IsShowArenaRevengeTipDialog = false;
            Mogo.Util.SystemConfig.Instance.ArenaRevengeTipDialogDisableDay = MogoTime.Instance.GetCurrentDateTime().Day;
            Mogo.Util.SystemConfig.SaveConfig();
        }
    }

    #endregion

    #region PVP等级

    public void OnShwoPVPLevelTip()
    {
        m_view.ShowPVPLevelTip(true);
    }

    public void OnClosePVPLevelTip()
    {
        m_view.ShowPVPLevelTip(false);
    }

    #endregion

    public void SetAttrData()
    {
        if (m_view != null)
        {
            m_view.SetNameText(MogoWorld.thePlayer.name);
            m_view.SetBattleForceText(MogoWorld.thePlayer.fightForce);
            m_view.SetCurTitleText(MogoWorld.thePlayer.arenicGrade);
            m_view.SetNextTitleText(MogoWorld.thePlayer.arenicGrade);
            m_view.SetProgressValue(MogoWorld.thePlayer.arenicCredit);
            m_view.SetHeadImage(MogoWorld.thePlayer.arenicGrade);

            // 设置PVP等级信息
            m_view.SetPVPLevelTipInfoCurTitle(ArenaLevelData.GetCurTitle(MogoWorld.thePlayer.arenicGrade));
            m_view.SetPVPLevelTipInfoCurPVPNum1(ArenaLevelData.GetCurPVPAddition(MogoWorld.thePlayer.arenicGrade));
            m_view.SetPVPLevelTipInfoCurPVPNum2(ArenaLevelData.GetCurPVPAnti(MogoWorld.thePlayer.arenicGrade));

            m_view.SetPVPLevelTipInfoNextTitle(ArenaLevelData.GetNextTitle(MogoWorld.thePlayer.arenicGrade));
            m_view.SetPVPLevelTipInfoNextPVPNum1(ArenaLevelData.GetNextPVPAddition(MogoWorld.thePlayer.arenicGrade));
            m_view.SetPVPLevelTipInfoNextPVPNum2(ArenaLevelData.GetNextPVPAnti(MogoWorld.thePlayer.arenicGrade));
        }
    }

    /// <summary>
    /// 竞技场敌方数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="tab"></param>
    public void SetArenaPlayerData(ArenaPlayerData data, int tab)
    {
        if (m_view != null)
        {
            m_playerData[tab] = data;
            if (m_curTab == tab)
            {
                m_view.SetArenaPlayerData(data, tab);
                m_view.SetModelShow(m_playerData[m_curTab].vocation, m_playerData[m_curTab].equip,true);
            
                if (m_curTab == (int)ArenaTab.Revenge)
                {
                    m_view.SetArenaDesc(LanguageData.dataMap.Get(274).Format(m_playerData[m_curTab].name));
                    if (m_arenaPersonalData.beatEnemy != 0)
                        m_view.SetRevengeInfo(true);
                }
            }
        }
    }

    public void SetArenaDesc()
    {
        if (m_curTab == (int)ArenaTab.Weak)
        {
            m_view.SetArenaDesc(m_weakMin, m_weakMax, m_curTab);
            m_view.SetRefreshPrice(PriceListData.dataMap.Get(6).priceList[1], m_curTab);
        }
        else if (m_curTab == (int)ArenaTab.Strong)
        {
            m_view.SetArenaDesc(m_strongMin, m_strongMax, m_curTab);
            m_view.SetRefreshPrice(PriceListData.dataMap.Get(7).priceList[1], m_curTab);
        }
        else if (m_curTab == (int)ArenaTab.Revenge)
        {
            m_view.SetRefreshPrice(PriceListData.dataMap.Get(9).priceList[1], m_curTab);
        }
    }

    #region 设置竞技场数据

    // 竞技场玩家自己数据
    public int m_ObtainDayPoint = 0; // 积分奖励界面调用数据
    public int m_ObtainWeekPoint = 0; // 积分奖励界面调用数据
    public ArenaPersonalData m_arenaPersonalData;

    public void SetArenaPersonalData(ArenaPersonalData data)
    {
        if (m_view != null)
        {
            m_arenaPersonalData = data;
            m_ObtainDayPoint = data.dayScore;
            m_ObtainWeekPoint = data.weekScore;

            m_view.SetArenaPersonalData(data);
            m_weakMin = data.weak[1];
            m_weakMax = data.weak[0];
            m_strongMin = data.strong[1];
            m_strongMax = data.strong[0];
            SetArenaDesc();         
        }
    }

    #endregion

    /// <summary>
    /// 是否显示领取奖励提示
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowRewardNotice(bool isShow)
    {
        if (m_view != null)
        {
            m_view.ShowRewardNotice(isShow);
        }
    }
}
