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
using System.Collections.Generic;
using Mogo.GameData;

// 体力购买数据缓存
public struct BuyEnergyData
{
    public string title;
    public string icon;
    public string buttonName;
    public int diamond;
    public int addEnergy;
}

// 限时领取数据缓存
public struct EnergyLimitActivityData
{
    public string title;
    public string icon;
    public string buttonName;
    public string limitActivityDesc;
    public bool finished;
}

// 其他玩法数据缓存
public struct NoNeedEnergyData
{
    public int ID;
    public string title;
    public string icon;
    public string buttonName;
    public string desc;
    public int requestLevel;
}

public class EnergyNoEnoughUILogicManager : UILogicManager 
{
    private static EnergyNoEnoughUILogicManager m_instance;
    public static EnergyNoEnoughUILogicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new EnergyNoEnoughUILogicManager();                
            }

            return EnergyNoEnoughUILogicManager.m_instance;
        }
    }

    #region 事件

    private EnergyNoEnoughUITab m_currentGuideType = EnergyNoEnoughUITab.BuyEnergyTab;
    public EnergyNoEnoughUITab CurrentGuideType
    {
        get { return m_currentGuideType; }
        set
        {
            m_currentGuideType = value;
        }
    }

    public void Initialize()
    {
        EnergyNoEnoughUIViewManager.Instance.ENERGYNOENOUGHUICLOSEUP += OnCloseUp;
        EnergyNoEnoughUIViewManager.Instance.ENERGYNOENOUGHUICHOOSEWAY += OnChooseWay;
        EnergyNoEnoughUIDict.GRIDBTNUP += OnGridBtnUp;

        // 属性绑定
        ItemSource = MogoWorld.thePlayer; // 由于目前Logic可能会跟随UI关闭而Release，故在此重新设置ItemSource
        SetBinding<float>(EntityMyself.ATTR_ENERGY, EnergyNoEnoughUIViewManager.Instance.SetEnergy);
        SetBinding<string>(EntityMyself.ATTR_ENERGYSTRING, EnergyNoEnoughUIViewManager.Instance.SetEnergy);
    }

    public override void Release()
    {
        base.Release();
        EnergyNoEnoughUIViewManager.Instance.ENERGYNOENOUGHUICLOSEUP -= OnCloseUp;
        EnergyNoEnoughUIViewManager.Instance.ENERGYNOENOUGHUICHOOSEWAY -= OnChooseWay;
        EnergyNoEnoughUIDict.GRIDBTNUP -= OnGridBtnUp;
    }

    void OnCloseUp()
    {
        LoggerHelper.Debug("OnEnergyNoEnoughUICloseUp");
        MogoUIManager.Instance.ShowEnergyNoEnoughUI(null, false);
        InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
    }

    void OnChooseWay(int way)
    {
        CurrentGuideType = (EnergyNoEnoughUITab)way;

        switch ((EnergyNoEnoughUITab)way)
        {
            case EnergyNoEnoughUITab.BuyEnergyTab:
                {
                    SetBuyEnergy();
                }break;
            case EnergyNoEnoughUITab.LimitActivityTab:
                {
                    SetLimitActivity();
                }break;
            case EnergyNoEnoughUITab.OtherSystemTab:
                {
                    SetNoNeedEnergy();
                }break;
        }       
    }

    private void OnGridBtnUp(int index)
    {
        switch ((EnergyNoEnoughUITab)CurrentGuideType)
        {
            case EnergyNoEnoughUITab.BuyEnergyTab:
                {
                    if (index == 0) // 购买一次体力
                    {
                        int lastTimes = MogoWorld.thePlayer.CalBuyEnergyLastTimes();
                        if(lastTimes > 0)
                            EventDispatcher.TriggerEvent<int>(Events.EnergyEvent.BuyEnergy, 1);
                        else
                            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(47810));
                    }
                    else if (index == 1) // 购买全部体力
                    {
                        int lastTimes = MogoWorld.thePlayer.CalBuyEnergyLastTimes();
                        if(lastTimes > 0)
                            EventDispatcher.TriggerEvent<int>(Events.EnergyEvent.BuyEnergy, lastTimes);
                        else
                            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(47810));
                    }
                } break;
            case EnergyNoEnoughUITab.LimitActivityTab:
                {
                    MogoUIManager.Instance.ShowLevelNoEnoughUI(null, false);
                    InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
                    InstanceUILogicManager.Instance.OnChooseGridCloseUp();
                    MogoUIManager.Instance.ShowMogoOperatingUI(1);
                } break;
            case EnergyNoEnoughUITab.OtherSystemTab:
                {
                    if (index < m_listNoNeedEnergyData.Count)
                    {
                        // ID = 4(竞技场)； ID= 3(试炼之塔)
                        if (m_listNoNeedEnergyData[index].ID == 4)
                        {
                            MogoUIManager.Instance.ShowLevelNoEnoughUI(null, false);
                            InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
                            InstanceUILogicManager.Instance.OnChooseGridCloseUp();
                            NormalMainUILogicManager.Instance.OnPVPPlayIconUp();
                        }
                        else if (m_listNoNeedEnergyData[index].ID == 3)
                        {
                            MogoUIManager.Instance.ShowLevelNoEnoughUI(null, false);
                            InstanceUILogicManager.Instance.OnChooseLevelUIBackUp();
                            InstanceUILogicManager.Instance.OnChooseGridCloseUp();
                            MogoUIManager.Instance.OpenWindow((int)WindowName.Tower, 
                                () =>
                                {
                                    ClimbTowerUILogicManager.Instance.SetTowerGridLayout(
                                    () =>
                                    {
                                        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
                                        ClimbTowerUILogicManager.Instance.ResourceLoaded();
                                        EventDispatcher.TriggerEvent(Events.TowerEvent.GetInfo);
                                    });
                                });
                        }
                    }
                } break;
        }       
    }

    #endregion

    #region 购买体力

    private List<BuyEnergyData> m_listBuyEnergyData = new List<BuyEnergyData>();

    public void SetBuyEnergy()
    {
        m_listBuyEnergyData.Clear();
        int lastTimes = MogoWorld.thePlayer.CalBuyEnergyLastTimes();
        EnergyData energyData = EnergyData.dataMap[1];

        // 购买一次
        BuyEnergyData buyOneEnergyData;
        buyOneEnergyData.title = LanguageData.GetContent(47806);
        buyOneEnergyData.icon = IconData.GetIconPath(14);
        if (lastTimes > 0)
        {
            buyOneEnergyData.buttonName = LanguageData.GetContent(47801); // 立即购买
            buyOneEnergyData.diamond = MogoWorld.thePlayer.CalBuyEnergyDiamondCost(1);
            buyOneEnergyData.addEnergy = energyData.fixedPoints * 1;      
        }
        else
        {
            buyOneEnergyData.buttonName = LanguageData.GetContent(47811); // 不可购买
            buyOneEnergyData.diamond = 0;
            buyOneEnergyData.addEnergy = 0; 
        }         

        // 全部购买
        BuyEnergyData buyAllEnergyData;
        buyAllEnergyData.title = LanguageData.GetContent(47807);
        buyAllEnergyData.icon = IconData.GetIconPath(14);
        if (lastTimes > 0)
        {
            buyAllEnergyData.buttonName = LanguageData.GetContent(47801); // 立即购买 
            buyAllEnergyData.diamond = MogoWorld.thePlayer.CalBuyEnergyDiamondCost(lastTimes);
            buyAllEnergyData.addEnergy = energyData.fixedPoints * lastTimes;   
        }
        else
        {
            buyAllEnergyData.buttonName = LanguageData.GetContent(47811); // 不可购买  
            buyAllEnergyData.diamond = 0;
            buyAllEnergyData.addEnergy = 0;
        }          

        m_listBuyEnergyData.Add(buyOneEnergyData);
        m_listBuyEnergyData.Add(buyAllEnergyData);

        EnergyNoEnoughUIViewManager.Instance.SetUIGridList(m_listBuyEnergyData.Count, () =>
            {
                EnergyNoEnoughUIViewManager.Instance.SetBuyEnergyGridListData(m_listBuyEnergyData);
            });
    }

    #endregion

    #region 限时活动 

    private List<EnergyLimitActivityData> m_listLimitActivityData = new List<EnergyLimitActivityData>();

    public void SetLimitActivity()
    {
        m_listLimitActivityData.Clear();
        m_listLimitActivityData = MogoWorld.thePlayer.GetEnergyLimitActivityDataList();

        EnergyNoEnoughUIViewManager.Instance.SetUIGridList(m_listLimitActivityData.Count, () =>
        {
            EnergyNoEnoughUIViewManager.Instance.SetLimitActivityGridListData(m_listLimitActivityData);
        });
    }

    #endregion

    #region 其他玩法(不需要体力玩法)

    private List<NoNeedEnergyData> m_listNoNeedEnergyData = new List<NoNeedEnergyData>();

    /// <summary>
    /// 设置其他玩法
    /// </summary>
    public void SetNoNeedEnergy()
    {
        m_listNoNeedEnergyData.Clear();
        List<int> idList = UpgradeGuideData.GetNoNeedEnergyList();
        for (int i = 0; i < idList.Count; i++)
        {
            UpgradeGuideData xmlData = UpgradeGuideData.GetData(idList[i]);
            NoNeedEnergyData noNeedEnergyData;
            noNeedEnergyData.ID = xmlData.id;
            noNeedEnergyData.title = LanguageData.GetContent(xmlData.title);
            noNeedEnergyData.icon = IconData.GetIconPath(xmlData.icon);
            noNeedEnergyData.buttonName = LanguageData.GetContent(47902);
            noNeedEnergyData.desc = LanguageData.GetContent(xmlData.describtion);

             // ID = 4(竞技场)； ID= 3(试炼之塔)
            if (xmlData.id == 4)
                noNeedEnergyData.requestLevel = SystemRequestLevel.ArenaIcon;
            else if (xmlData.id == 3)
                noNeedEnergyData.requestLevel = SystemRequestLevel.CILMBTOWER;
            else
                noNeedEnergyData.requestLevel = 0;

            m_listNoNeedEnergyData.Add(noNeedEnergyData);
        }

        // 按照开启等级从小到大排序
        m_listNoNeedEnergyData.Sort(delegate(NoNeedEnergyData a, NoNeedEnergyData b)
        {
            if (a.requestLevel < b.requestLevel)
                return -1;
            else
                return 1;
        });

        EnergyNoEnoughUIViewManager.Instance.SetUIGridList(m_listNoNeedEnergyData.Count, () =>
        {
            EnergyNoEnoughUIViewManager.Instance.SetNoNeedEnergyGridListData(m_listNoNeedEnergyData);
        });
    }
    #endregion

    public void SetVipRealState()
    {
        if (CurrentGuideType == EnergyNoEnoughUITab.BuyEnergyTab)
        {
            SetBuyEnergy();
        }
    }
}
