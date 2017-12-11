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
using Mogo.Game;
using Mogo.GameData;
using System.Collections.Generic;
using System.Linq;

// 升级引导数据缓存
public struct UpgradeLevelGuideData
{
    public int ID;
    public string title;
    public string icon;
    public string buttonName;
    public string desc;
    public int requestLevel;
}

public class LevelNoEnoughUILogicManager : UILogicManager 
{
    private static LevelNoEnoughUILogicManager m_instance;
    public static LevelNoEnoughUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new LevelNoEnoughUILogicManager();                
            }

            return LevelNoEnoughUILogicManager.m_instance;
        }
    }

    /// <summary>
    /// 是否在选择副本难度界面中弹出界面,如果是关闭该界面需要回到副本难度界面
    /// </summary>
    public static bool m_IsChooseLevelUI = false; 
    public static bool IsChooseLevelUI
    {
        get { return m_IsChooseLevelUI; }
        set
        {
            m_IsChooseLevelUI = value;
        }
    }

    #region 事件

    public void Initialize()
    {
        LevelNoEnoughUIViewManager.Instance.LEVELNOENOUGHUICLOSEUP += OnCloseUp;
        LevelNoEnoughUIDict.GRIDBTNUP += OnGridBtnUp;

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
        SetBinding<byte>(EntityMyself.ATTR_LEVEL, LevelNoEnoughUIViewManager.Instance.SetLevel);
        SetBinding<float>(EntityMyself.ATTR_EXP, LevelNoEnoughUIViewManager.Instance.SetExp);
    }

    public override void Release()
    {
        base.Release();
        LevelNoEnoughUIViewManager.Instance.LEVELNOENOUGHUICLOSEUP -= OnCloseUp;
        LevelNoEnoughUIDict.GRIDBTNUP -= OnGridBtnUp; 
    }

    public void RpcGetArenaExpReq()
    {
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        MogoWorld.thePlayer.RpcCall("CanGetExpReq");
    }

    public void RpcGetArenaExpResp(int arenaExp)
    {
        SetUpgradeLevelGuide(arenaExp);
        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
    }

    void OnCloseUp()
    {
        LoggerHelper.Debug("OnLevelNoEnoughUICloseUp");
        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, false);

        if (IsChooseLevelUI)
            InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
        else
            MogoUIManager.Instance.ShowMogoNormalMainUI();
    }

    void OnGridBtnUp(int index)
    {
        if (index < m_listUpgradeLevelGuideData.Count)
        {
            // ID = 2(竞技场)
            // ID = 1(每日任务)
            // ID = 5(飞龙大赛)
            // ID = 6(保卫女神)
            // ID = 7(副本)
            switch (m_listUpgradeLevelGuideData[index].ID)
            {
                case 1:
                    if (NormalMainUIViewManager.Instance != null)
                    {
                        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, false);
                        InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
                        InstanceUILogicManager.Instance.OnChooseGridCloseUp();
                        EventDispatcher.TriggerEvent(Events.DailyTaskEvent.OpenDailyTaskUI);
                    }
                    break;

                case 2:
                    if (NormalMainUIViewManager.Instance != null && NormalMainUILogicManager.Instance != null)
                    {
                        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, false);
                        InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
                        InstanceUILogicManager.Instance.OnChooseGridCloseUp();
                        NormalMainUILogicManager.Instance.OnPVPPlayIconUp();
                    }
                    break;

                case 5:
                    {
                        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, false);
                        InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
                        InstanceUILogicManager.Instance.OnChooseGridCloseUp();
                        NormalMainUILogicManager.Instance.OnPVEPlayIconUp();
                    }
                    break;

                case 6:
                    {
                        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, false);
                        InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
                        InstanceUILogicManager.Instance.OnChooseGridCloseUp();
                        NormalMainUILogicManager.Instance.OnPVEPlayIconUp();
                    }
                    break;

                case 7:
                    {
                        // 点击后到 玩家能进入的最后一个副本的大界面,然后在这个副本上加一个小标签"升级推荐"
                        MogoUIManager.Instance.ShowLevelNoEnoughUI(null, false);
                        InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
                        InstanceUILogicManager.Instance.OnChooseGridCloseUp();
                        KeyValuePair<int, int> theLastMission = MogoWorld.thePlayer.GetLastMissionCanEnter();
                        InstanceUILogicManager.Instance.MissionOpenAllTheWay(theLastMission.Key, theLastMission.Value, MissionOpenType.LevelUpgradeGuide);
                    }
                    break;             
            }
        }
    }

    #endregion

    #region 升级引导

    private List<UpgradeLevelGuideData> m_listUpgradeLevelGuideData = new List<UpgradeLevelGuideData>();

    /// <summary>
    /// 升级引导
    /// </summary>
    public void SetUpgradeLevelGuide(int arenaExp = 0)
    {
        m_listUpgradeLevelGuideData.Clear();
        List<int> idList = UpgradeGuideData.GetLevelNoEnoughList();
        for (int i = 0; i < idList.Count; i++)
        {
            UpgradeGuideData xmlData = UpgradeGuideData.GetData(idList[i]);
            UpgradeLevelGuideData upgradeLevelGuideData;
            upgradeLevelGuideData.ID = xmlData.id;
            upgradeLevelGuideData.title = LanguageData.GetContent(xmlData.title);
            upgradeLevelGuideData.icon = IconData.GetIconPath(xmlData.icon);
            upgradeLevelGuideData.buttonName = LanguageData.GetContent(47902);

            bool useMinLevel = true;
            LevelGuideData levelGuideData = new LevelGuideData();
            levelGuideData.level = new List<int>();
            levelGuideData.level.Add(int.MaxValue);

            foreach (var item in LevelGuideData.dataMap)
            {
                if (xmlData.id != item.Value.type)
                    continue;

                if (levelGuideData.level[0] > item.Value.level[0])
                    levelGuideData = item.Value;

                if (MogoWorld.thePlayer.level > item.Value.level[0] 
                    && MogoWorld.thePlayer.level < item.Value.level[1])
                {
                    useMinLevel = false;
                    levelGuideData = item.Value;
                    break;
                }
            }

            if (useMinLevel)
            {
                upgradeLevelGuideData.desc = LanguageData.dataMap.Get(xmlData.describtion).Format(AvatarLevelData.dataMap.Get(levelGuideData.level[0]).expStandard * levelGuideData.rate);
            }
            else
            {
                upgradeLevelGuideData.desc = LanguageData.dataMap.Get(xmlData.describtion).Format(AvatarLevelData.dataMap.Get(MogoWorld.thePlayer.level).expStandard * levelGuideData.rate);
            }

            // ID = 2(竞技场)
            // ID = 1(每日任务)
            // ID = 5(飞龙大赛)
            // ID = 6(保卫女神)
            if (xmlData.id == 2)
            {
                upgradeLevelGuideData.requestLevel = SystemRequestLevel.ArenaIcon;                
            }
            else if (xmlData.id == 1)
            {
                upgradeLevelGuideData.requestLevel = SystemRequestLevel.DailyTaskIcon;             
            }
            else if(xmlData.id == 5)
            {
                upgradeLevelGuideData.requestLevel = SystemRequestLevel.DRAGONMATCH;        
            }
            else if (xmlData.id == 6)
            {
                upgradeLevelGuideData.requestLevel = SystemRequestLevel.OGREMUSTDIE;     
            }
            else if (xmlData.id == 7)
            {
                upgradeLevelGuideData.requestLevel = 0;
            }
            else
            {
                upgradeLevelGuideData.requestLevel = 0;
                upgradeLevelGuideData.desc = string.Empty;
            }

            m_listUpgradeLevelGuideData.Add(upgradeLevelGuideData);
        }

        // 按照开启等级从小到大排序
        m_listUpgradeLevelGuideData.Sort(delegate(UpgradeLevelGuideData a, UpgradeLevelGuideData b)
        {
            if (a.requestLevel < b.requestLevel)
                return -1;
            else
                return 1;
        });

        // 创建UI
        LevelNoEnoughUIViewManager.Instance.SetUIGridList(m_listUpgradeLevelGuideData.Count, () =>
        {
            LevelNoEnoughUIViewManager.Instance.SetGridListData(m_listUpgradeLevelGuideData);
        });
    }
    #endregion
}
