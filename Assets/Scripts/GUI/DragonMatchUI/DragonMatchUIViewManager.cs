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
using System;
using Mogo.Util;
using System.Collections.Generic;
using Mogo.GameData;
using Mogo.Game;

public class DragonMatchUIViewManager : MogoUIBehaviour
{
    private static DragonMatchUIViewManager m_instance;
    public static DragonMatchUIViewManager Instance { get { return DragonMatchUIViewManager.m_instance; } }

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量

    private GameObject m_goDragonMatchUIBottom;
    private GameObject m_goDragonMatchUIBottomL;

    // 探索信息框
    private GameObject m_goDragonMatchExploreUI;
    private UILabel m_lblDragonMatchExploreUIIntroduction;
    private UILabel m_lblDragonMatchExploreUITitle;
    private GameObject m_goDragonMatchExploreUIBtnExplore;

    // 玩家信息框
    private GameObject m_goDragonMatchPlayerInfoUI;
    private UILabel m_lblDragonMatchPlayerInfoUILevel;
    private UILabel m_lblDragonMatchPlayerInfoUIName;
    private UILabel m_lblDragonMatchPlayerInfoUIPowerText;
    private UILabel m_lblDragonMatchPlayerInfoUITongText;
    private UILabel m_lblDragonMatchPlayerInfoUIDragonText;
    private UILabel m_lblDragonMatchPlayerInfoUIStatusText;
    private UILabel m_lblDragonMatchPlayerInfoUIHitTimesText;
    private UILabel m_lblDragonMatchPlayerInfoUIHitObtainGold;
    private UILabel m_lblDragonMatchPlayerInfoUIHitObtainExp;
    private GameObject m_goDragonMatchPlayerInfoUIBtnHit;

    // 宝箱奖励框
    private GameObject m_goDragonMatchBeginTreasureUI;
    private UILabel m_lblDragonMatchBeginTreasureUIRewardGoldText;
    private UILabel m_lblDragonMatchBeginTreasureUIRewardExpText;

    // 主界面
    private UILabel m_lblDragonMatchUIPlayerLevelNum;
    private UILabel m_lblDragonMatchUIPlayerDiamnodlNum;
    private UISprite m_spDragonMatchUIPlayerVIPLevelNum1;
    private UISprite m_spDragonMatchUIPlayerVIPLevelNum10;
    private UILabel m_lblDragonMatchUIPlayerPowerNum;

    private UILabel m_lblDragonMatchUIHitTimesNum;
    private UILabel m_lblDragonMatchUIRoundTimesNum;
    private GameObject m_goGODragonMatchUIRoundDOTList;

    private GameObject m_goDragonMatchUIHitCD;
    private UILabel m_lblDragonMatchUIHitCDNum;

    private GameObject m_goDragonMatchUIBtnStartMatch;
    private GameObject m_goDragonMatchUIMatchBegin;
    private UILabel m_lblDragonMatchUIBeginTimeNum;
    private GameObject m_goDragonMatchUIBottomEndMatch;
    private GameObject m_goDragonMatchUIBottomEndBtnTreasure;
    private UISprite m_spDragonMatchUIBottomEndBtnTreasureBGDown;
    private UISprite m_spDragonMatchUIBottomEndBtnTreasureBGUp;

    private GameObject m_goDragonMatchUIBtnExplore;
    private GameObject m_goDragonMatchUIRoundTimesBtnAdd;
    private GameObject m_goDragonMatchUIBtnRecord;
    private GameObject m_goDragonMatchUIBtnRefresh;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        Camera camera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        FindTransform("DragonMatchUICenter").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        FindTransform("DragonMatchUITopL").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        FindTransform("DragonMatchUITopR").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goDragonMatchUIBottom = FindTransform("DragonMatchUIBottom").gameObject;
        m_goDragonMatchUIBottom.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goDragonMatchUIBottomL = FindTransform("DragonMatchUIBottomL").gameObject;
        m_goDragonMatchUIBottomL.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        FindTransform("DragonMatchUIBottomR").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;

        m_goDragonMatchExploreUI = FindTransform("DragonMatchExploreUI").gameObject;
        m_lblDragonMatchExploreUIIntroduction = FindTransform("DragonMatchExploreUIIntroduction").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchExploreUITitle = FindTransform("DragonMatchExploreUITitle").GetComponentsInChildren<UILabel>(true)[0];
        m_goDragonMatchExploreUIBtnExplore = FindTransform("DragonMatchExploreUIBtnExplore").gameObject;

        m_goDragonMatchPlayerInfoUI = FindTransform("DragonMatchPlayerInfoUI").gameObject;
        m_lblDragonMatchPlayerInfoUILevel = FindTransform("DragonMatchPlayerInfoUILevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchPlayerInfoUIName = FindTransform("DragonMatchPlayerInfoUIName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchPlayerInfoUIPowerText = FindTransform("DragonMatchPlayerInfoUIPowerText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchPlayerInfoUITongText = FindTransform("DragonMatchPlayerInfoUITongText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchPlayerInfoUIDragonText = FindTransform("DragonMatchPlayerInfoUIDragonText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchPlayerInfoUIStatusText = FindTransform("DragonMatchPlayerInfoUIStatusText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchPlayerInfoUIHitTimesText = FindTransform("DragonMatchPlayerInfoUIHitTimesText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchPlayerInfoUIHitObtainGold = FindTransform("DragonMatchPlayerInfoUIHitObtainGold").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchPlayerInfoUIHitObtainExp = FindTransform("DragonMatchPlayerInfoUIHitObtainExp").GetComponentsInChildren<UILabel>(true)[0];
        m_goDragonMatchPlayerInfoUIBtnHit = FindTransform("DragonMatchPlayerInfoUIBtnHit").gameObject;

        m_goDragonMatchBeginTreasureUI = FindTransform("DragonMatchBeginTreasureUI").gameObject;
        m_lblDragonMatchBeginTreasureUIRewardGoldText = FindTransform("DragonMatchBeginTreasureUIRewardGoldText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchBeginTreasureUIRewardExpText = FindTransform("DragonMatchBeginTreasureUIRewardExpText").GetComponentsInChildren<UILabel>(true)[0];

        m_lblDragonMatchUIPlayerLevelNum = FindTransform("DragonMatchUIPlayerLevelNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchUIPlayerDiamnodlNum = FindTransform("DragonMatchUIPlayerDiamnodlNum").GetComponentsInChildren<UILabel>(true)[0];
        m_spDragonMatchUIPlayerVIPLevelNum1 = FindTransform("DragonMatchUIPlayerVIPLevelNum1").GetComponentsInChildren<UISprite>(true)[0];
        m_spDragonMatchUIPlayerVIPLevelNum10 = FindTransform("DragonMatchUIPlayerVIPLevelNum10").GetComponentsInChildren<UISprite>(true)[0];
        m_lblDragonMatchUIPlayerPowerNum = FindTransform("DragonMatchUIPlayerPowerNum").GetComponentsInChildren<UILabel>(true)[0];

        m_lblDragonMatchUIHitTimesNum = FindTransform("DragonMatchUIHitTimesNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDragonMatchUIRoundTimesNum = FindTransform("DragonMatchUIRoundTimesNum").GetComponentsInChildren<UILabel>(true)[0];
        m_goGODragonMatchUIRoundDOTList = FindTransform("GODragonMatchUIRoundDOTList").gameObject;

        m_goDragonMatchUIHitCD = FindTransform("DragonMatchUIHitCD").gameObject;
        m_lblDragonMatchUIHitCDNum = FindTransform("DragonMatchUIHitCDNum").GetComponentsInChildren<UILabel>(true)[0];

        m_goDragonMatchUIBtnStartMatch = FindTransform("DragonMatchUIBtnStartMatch").gameObject;
        m_goDragonMatchUIMatchBegin = FindTransform("DragonMatchUIMatchBegin").gameObject;
        m_lblDragonMatchUIBeginTimeNum = FindTransform("DragonMatchUIBeginTimeNum").GetComponentsInChildren<UILabel>(true)[0];
        m_goDragonMatchUIBottomEndMatch = FindTransform("DragonMatchUIBottomEndMatch").gameObject;
        m_goDragonMatchUIBottomEndBtnTreasure = FindTransform("DragonMatchUIBottomEndBtnTreasure").gameObject;
        m_spDragonMatchUIBottomEndBtnTreasureBGDown = FindTransform("DragonMatchUIBottomEndBtnTreasureBGDown").GetComponentsInChildren<UISprite>(true)[0];
        m_spDragonMatchUIBottomEndBtnTreasureBGUp = FindTransform("DragonMatchUIBottomEndBtnTreasureBGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_goDragonMatchUIBtnExplore = FindTransform("DragonMatchUIBtnExplore").gameObject;
        m_goDragonMatchUIRoundTimesBtnAdd = FindTransform("DragonMatchUIRoundTimesBtnAdd").gameObject;
        m_goDragonMatchUIRoundTimesBtnAdd.SetActive(false);
        m_goDragonMatchUIBtnRecord = FindTransform("DragonMatchUIBtnRecord").gameObject;
        m_goDragonMatchUIBtnRefresh = FindTransform("DragonMatchUIBtnRefresh").gameObject;

        Initialize();
    }

    #region 事件

    public Action DRAGONMATCHUICLOSEUP;
    public Action DRAGONMATCHUISTARTMATCHUP;
    public Action DRAGONMATCHUIRECORDUP;
    public Action DRAGONMATCHUIEXPLOREUP;
    public Action DRAGONMATCHUIREFRESHUP;
    public Action DRAGONMATCHUIHITTIMEADDUP;
    public Action DRAGONMATCHUIROUNDTIMEADDUP;
    public Action DRAGONMATCHUIHITCDCLEARUP;

    public Action DRAGONMATCHUIBeginTreasureUp;
    public Action DRAGONMATCHUIBeginRMB1UP;
    public Action DRAGONMATCHUIBeginRMB2UP;
    public Action DRAGONMATCHUIEndTreasureUp;

    public Action<int> DRAGONMATCHUIPLAYERUP;

    public Action PLAYERINFOUIHITUP;
    public Action EXPLOREUIExploreUP;

    public Action BEGINTREASUREUICLOSEUP;

    public void Initialize()
    {
        // 飞龙大赛
        FindTransform("DragonMatchUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCloseUp;
        FindTransform("DragonMatchUIBtnStartMatch").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnStartMatchUp;
        FindTransform("DragonMatchUIBtnRecord").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnRecordUp;
        FindTransform("DragonMatchUIBtnExplore").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnExploreUp;
        FindTransform("DragonMatchUIBtnRefresh").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnRefreshUp;
        FindTransform("DragonMatchUIHitTimesBtnAdd").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnHitTimesAddUp;
        FindTransform("DragonMatchUIRoundTimesBtnAdd").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnRoundTimesAddUp;
        FindTransform("DragonMatchUIHitCDBtnClear").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnHitCDClearUp;

        // 宝箱
        FindTransform("DragonMatchUIBeginBtnTreasure").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBeginTreasureUp;
        FindTransform("DragonMatchUIBeginBtnRMB1").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBeginRMB1UP;
        FindTransform("DragonMatchUIBeginBtnRMB2").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBeginRMB2UP;
        FindTransform("DragonMatchUIBottomEndBtnTreasure").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnEndTreasureUp;

        // 玩家信息界面
        FindTransform("DragonMatchPlayerInfoUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnPlayerInfoUIClose;
        FindTransform("DragonMatchPlayerInfoUIBtnHit").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnPlayerInfoUIHit;

        // 探索界面
        FindTransform("DragonMatchExploreUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnExploreUICloseUp;
        FindTransform("DragonMatchExploreUIBtnExplore").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnExploreUIExploreUp;

        // 宝箱奖励界面
        FindTransform("DragonMatchBeginTreasureUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBeginTreasureUICloseUp;

        LoadDragonMatchUIRoundDOTList(() =>
            {
                ShowMatchRound();
            });

        DragonMatchUILogicManager.Instance.Initialize();
        m_uiLoginManager = DragonMatchUILogicManager.Instance;
    }

    public void Release()
    {
        DragonMatchUILogicManager.Instance.Release();
    }

    void OnCloseUp()
    {
        if (DRAGONMATCHUICLOSEUP != null)
            DRAGONMATCHUICLOSEUP();
    }

    void OnStartMatchUp()
    {
        if (DRAGONMATCHUISTARTMATCHUP != null)
            DRAGONMATCHUISTARTMATCHUP();
    }

    void OnRecordUp()
    {
        if (DRAGONMATCHUIRECORDUP != null)
            DRAGONMATCHUIRECORDUP();
    }

    void OnExploreUp()
    {
        if (DRAGONMATCHUIEXPLOREUP != null)
            DRAGONMATCHUIEXPLOREUP();
    }

    void OnRefreshUp()
    {
        if (DRAGONMATCHUIREFRESHUP != null)
            DRAGONMATCHUIREFRESHUP();
    }

    void OnHitTimesAddUp()
    {
        if (DRAGONMATCHUIHITTIMEADDUP != null)
            DRAGONMATCHUIHITTIMEADDUP();
    }

    void OnRoundTimesAddUp()
    {
        if (DRAGONMATCHUIROUNDTIMEADDUP != null)
            DRAGONMATCHUIROUNDTIMEADDUP();
    }

    void OnHitCDClearUp()
    {
        if (DRAGONMATCHUIHITCDCLEARUP != null)
            DRAGONMATCHUIHITCDCLEARUP();
    }

    void OnBeginTreasureUp()
    {
        if (DRAGONMATCHUIBeginTreasureUp != null)
            DRAGONMATCHUIBeginTreasureUp();
    }

    void OnBeginRMB1UP()
    {
        if (DRAGONMATCHUIBeginRMB1UP != null)
            DRAGONMATCHUIBeginRMB1UP();
    }

    void OnBeginRMB2UP()
    {
        if (DRAGONMATCHUIBeginRMB2UP != null)
            DRAGONMATCHUIBeginRMB2UP();
    }

    void OnEndTreasureUp()
    {
        if (DRAGONMATCHUIEndTreasureUp != null)
            DRAGONMATCHUIEndTreasureUp();

        ShowEndMatchTreasure(true, true); // 显示奖励宝箱为打开状态
    }

    void OnPlayer1Up()
    {
        if (DRAGONMATCHUIPLAYERUP != null)
            DRAGONMATCHUIPLAYERUP(0);
    }

    void OnPlayer2Up()
    {
        if (DRAGONMATCHUIPLAYERUP != null)
            DRAGONMATCHUIPLAYERUP(1);
    }

    void OnPlayer3Up()
    {
        if (DRAGONMATCHUIPLAYERUP != null)
            DRAGONMATCHUIPLAYERUP(2);
    }

    void OnPlayer4Up()
    {
        if (DRAGONMATCHUIPLAYERUP != null)
            DRAGONMATCHUIPLAYERUP(3);
    }

    void OnPlayer5Up()
    {
        if (DRAGONMATCHUIPLAYERUP != null)
            DRAGONMATCHUIPLAYERUP(4);
    }

    void OnPlayerInfoUIClose()
    {
        ShowDragonMatchPlayerInfoUI(false);
    }

    void OnPlayerInfoUIHit()
    {
        if (PLAYERINFOUIHITUP != null)
            PLAYERINFOUIHITUP();
    }

    void OnExploreUICloseUp()
    {
        ShowDragonMatchExploreUI(false);
    }

    void OnExploreUIExploreUp()
    {
        if (EXPLOREUIExploreUP != null)
            EXPLOREUIExploreUP();
    }

    void OnBeginTreasureUICloseUp()
    {
        if (BEGINTREASUREUICLOSEUP != null)
            BEGINTREASUREUICLOSEUP();
    }

    #endregion

    #region 主界面信息

    #region 玩家属性

    /// <summary>
    /// 设置玩家等级
    /// </summary>
    /// <param name="level"></param>
    public void SetPlayerLevel(byte level)
    {
        m_lblDragonMatchUIPlayerLevelNum.text = level.ToString();
    }

    /// <summary>
    /// 设置玩家钻石数量
    /// </summary>
    /// <param name="diamond"></param>
    public void SetPlayerDiamond(uint diamond)
    {
        m_lblDragonMatchUIPlayerDiamnodlNum.text = diamond.ToString();
    }

    /// <summary>
    /// 设置玩家VIP等级
    /// </summary>
    /// <param name="vipLevel"></param>
    public void SetPlayerVipLevel(byte vipLevel)
    {
        if (vipLevel < 10)
        {
            m_spDragonMatchUIPlayerVIPLevelNum1.spriteName = "vip_" + vipLevel;
            m_spDragonMatchUIPlayerVIPLevelNum10.gameObject.SetActive(false);
        }
        else if (vipLevel >= 10)
        {
            m_spDragonMatchUIPlayerVIPLevelNum10.spriteName = "vip_1";
            m_spDragonMatchUIPlayerVIPLevelNum1.spriteName = "vip_" + (vipLevel - 10);
            m_spDragonMatchUIPlayerVIPLevelNum10.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 设置玩家战力
    /// </summary>
    /// <param name="power"></param>
    public void SetPlayerPower(uint power)
    {
        m_lblDragonMatchUIPlayerPowerNum.text = power.ToString();
    }

    #endregion

    #region 飞龙大赛相关信息

    /// <summary>
    /// 设置袭击次数
    /// </summary>
    /// <param name="times"></param>
    public void SetHitTimes(string times)
    {
        m_lblDragonMatchUIHitTimesNum.text = times.ToString();
    }

    /// <summary>
    /// 设置当前环数
    /// </summary>
    /// <param name="times"></param>
    public void SetCurrentRoundTimes(string times)
    {
        m_lblDragonMatchUIRoundTimesNum.text = times.ToString();
    }

    /// <summary>
    /// 是否显示袭击CD以及设置袭击CD
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="secondNum">如果显示袭击CD，设置CD秒数</param>
    private MogoCountDown m_hitCDCountDown = null;
    public void ShowHitCD(bool isShow, int secondNum = 0, Action action = null)
    {
        m_goDragonMatchUIHitCD.SetActive(isShow);

        if (isShow)
        {
            if (m_hitCDCountDown != null)
                m_hitCDCountDown.Release();

            m_hitCDCountDown = new MogoCountDown(m_lblDragonMatchUIHitCDNum, secondNum,
          "", "", "", MogoCountDown.TimeStringType.UpToMinutes, () =>
          {
              if (action != null)
                  action();
          });
        }
        else
        {
            if (m_hitCDCountDown != null)
                m_hitCDCountDown.Release();
        }
    }

    /// <summary>
    /// 获取袭击CD剩余秒数
    /// </summary>
    /// <returns></returns>
    public int GetHitCDLastSeconds()
    {
        if (m_hitCDCountDown != null)
            return m_hitCDCountDown.GetLastSeconds();

        return 0;
    }

    /// <summary>
    /// 是否显示开始比赛按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowStartMatchButton(bool isShow)
    {
        m_goDragonMatchUIBtnStartMatch.SetActive(isShow);
    }

    /// <summary>
    /// 是否显示结束比赛宝箱按钮,以及设置宝箱开启或关闭状态
    /// </summary>
    /// <param name="isShow">显示或隐藏奖励宝箱</param>
    /// <param name="isOpen">打开或关闭奖励宝箱</param>
    public void ShowEndMatchTreasure(bool isShow, bool isOpen = false)
    {
        m_goDragonMatchUIBottomEndMatch.SetActive(isShow);

        if (isShow)
        {
            if (isOpen)
            {
                m_spDragonMatchUIBottomEndBtnTreasureBGUp.spriteName = "baoxiang01_open";
                m_spDragonMatchUIBottomEndBtnTreasureBGDown.spriteName = "baoxiang01_open";

                ShowTreasureActiveAnimation(false);
                ShowTreasureActiveRotationAnimation(false);
            }
            else
            {
                m_spDragonMatchUIBottomEndBtnTreasureBGUp.spriteName = "baoxiang01_close";
                m_spDragonMatchUIBottomEndBtnTreasureBGDown.spriteName = "baoxiang01_close";

                ShowTreasureActiveAnimation(true);
                ShowTreasureActiveRotationAnimation(true);
            }
        }
        else
        {
            ShowTreasureActiveAnimation(false);
            ShowTreasureActiveRotationAnimation(false);
        }
    }

    /// <summary>
    /// 是否显示开始比赛信息以及设置比赛剩余时间
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="secondNum">如果显示开始比赛信息，设置剩余时间秒数</param>
    private MogoCountDown m_matchLastTimesCountDown = null;
    public void ShowBeginInfo(bool isShow, int secondNum = 0, Action action = null)
    {
        m_goDragonMatchUIMatchBegin.SetActive(isShow);

        if (isShow)
        {
            if (m_matchLastTimesCountDown != null)
                m_matchLastTimesCountDown.Release();

            m_matchLastTimesCountDown = new MogoCountDown(m_lblDragonMatchUIBeginTimeNum, secondNum,
          "", "", "", MogoCountDown.TimeStringType.UpToHour, () =>
          {
              if (action != null)
                  action();
          });
        }
        else
        {
            if (m_matchLastTimesCountDown != null)
                m_matchLastTimesCountDown.Release();
        }
    }

    #endregion

    #region 飞龙大赛环数

    private List<GameObject> m_listRoundDOTFG = new List<GameObject>();
    private List<GameObject> m_listRoundDOTLine = new List<GameObject>();
    private Transform m_tranDragonMatchUIRoundDOTList;

    bool IsDragonMatchUIRoundDOTListLoaded = false;
    private void LoadDragonMatchUIRoundDOTList(Action action = null)
    {
        if (m_tranDragonMatchUIRoundDOTList == null)
        {
            if (!IsDragonMatchUIRoundDOTListLoaded)
            {
                IsDragonMatchUIRoundDOTListLoaded = true;

                AssetCacheMgr.GetUIInstance("DragonMatchUIRoundDOTList.prefab", (prefab, guid, go) =>
                {
                    GameObject obj = (GameObject)go;
                    obj.transform.parent = m_goGODragonMatchUIRoundDOTList.transform;
                    obj.transform.localPosition = new Vector3(0, 0, 0);
                    obj.transform.localScale = new Vector3(1, 1, 1);
                    m_tranDragonMatchUIRoundDOTList = obj.transform;

                    m_listRoundDOTFG.Clear();
                    m_listRoundDOTLine.Clear();
                    for (int round = 0; round < 10; round++)
                    {
                        Transform tranDOT = m_tranDragonMatchUIRoundDOTList.Find(string.Concat("DragonMatchUIRoundDOT", round));
                        m_listRoundDOTFG.Add(tranDOT.Find("DragonMatchUIRoundDOTFG").gameObject);
                    }

                    Transform tranDOTLine = m_tranDragonMatchUIRoundDOTList.Find("DragonMatchUIRoundDOTLine");
                    for (int round = 0; round < 10; round++)
                    {
                        m_listRoundDOTLine.Add(tranDOTLine.Find(string.Concat("DragonMatchUIRoundDOT", round, "Line")).gameObject);

                        if (round > 0)
                        {
                            UISprite spLine = m_listRoundDOTLine[round].GetComponentsInChildren<UISprite>(true)[0];
                            spLine.pivot = UIWidget.Pivot.TopLeft;
                            float distance = Vector3.Distance(m_listRoundDOTFG[round - 1].transform.parent.localPosition, m_listRoundDOTFG[round].transform.parent.localPosition);
                            spLine.transform.localScale = new Vector3(distance, 3, 1);
                            spLine.transform.localPosition = m_listRoundDOTFG[round - 1].transform.parent.localPosition;

                            Vector3 targetDir = m_listRoundDOTFG[round].transform.parent.localPosition - m_listRoundDOTFG[round - 1].transform.parent.localPosition;
                            Vector3 x_axis = new Vector3(1, 0, 0);
                            float angle = Vector3.Angle(targetDir, x_axis);

                            Vector3 z_axis = new Vector3(0, 0, -1);
                            if (m_listRoundDOTFG[round].transform.parent.localPosition.y > m_listRoundDOTFG[round - 1].transform.parent.localPosition.y)
                                z_axis = new Vector3(0, 0, 1);

                            spLine.transform.localRotation = Quaternion.AngleAxis(angle, z_axis);
                        }
                        else
                        {
                            UISprite spLine = m_listRoundDOTLine[round].GetComponentsInChildren<UISprite>(true)[0];
                            spLine.transform.localScale = new Vector3(0, 0, 0);
                        }
                    }

                    if (action != null)
                        action();
                });
            }
        }
        else
        {
            if (action != null)
                action();
        }
    }

    /// <summary>
    /// 设置环数数据
    /// </summary>
    /// <param name="currentRound"></param>
    private int m_currentRound = 0;
    public void ShowMatchRound(int currentRound)
    {
        m_currentRound = currentRound;
        if (m_listRoundDOTFG.Count == 10)
        {
            ShowMatchRound();
        }
    }

    /// <summary>
    /// 设置环数UI
    /// </summary>
    private void ShowMatchRound()
    {
        for (int round = 0; round < m_listRoundDOTFG.Count; round++)
        {
            if (round < m_currentRound)
            {
                m_listRoundDOTFG[round].SetActive(true);
                m_listRoundDOTLine[round].SetActive(true);
            }
            else
            {
                m_listRoundDOTFG[round].SetActive(false);
                m_listRoundDOTLine[round].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 是否显示探索按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBtnExplore(bool isShow)
    {
        m_goDragonMatchUIBtnExplore.SetActive(isShow);
    }

    /// <summary>
    /// 是否显示袭击事件按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBtnRecord(bool isShow)
    {
        m_goDragonMatchUIBtnRecord.SetActive(isShow);
    }

    /// <summary>
    /// 是否显示刷新按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowBtnRefresh(bool isShow)
    {
        m_goDragonMatchUIBtnRefresh.SetActive(isShow);
    }

    #endregion

    #region 宝箱可领取特效和动画

    private string m_fx1TreasureActive = "TreasureActiveFX1";

    /// <summary>
    /// 在宝箱上附加可领取特效
    /// </summary>
    private void AttachTreasureActiveAnimation(Action action)
    {
        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        MogoFXManager.Instance.AttachParticleAnim("fx_ui_baoxiangxingxing.prefab", m_fx1TreasureActive, m_goDragonMatchUIBottomEndBtnTreasure,
            MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
            {
                if (action != null)
                    action();

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
    }

    /// <summary>
    /// 释放宝箱可领取特效
    /// </summary>
    private void ReleaseTreasureActiveAnimation()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fx1TreasureActive);
    }

    /// <summary>
    /// 显示或隐藏宝箱可领取特效
    /// </summary>
    /// <param name="isShow"></param>
    private bool IsShowTreasureActiveAnimation = false;
    private void ShowTreasureActiveAnimation(bool isShow)
    {
        IsShowTreasureActiveAnimation = isShow;
        GameObject m_goFx = MogoFXManager.Instance.FindParticeAnim(m_fx1TreasureActive);
        if (m_goFx != null)
            m_goFx.SetActive(isShow);
    }

    /// <summary>
    /// 宝箱可领取振动动画
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowTreasureActiveRotationAnimation(bool isShow)
    {
        TweenRotation tr = m_goDragonMatchUIBottomEndMatch.GetComponentsInChildren<TweenRotation>(true)[0];
        if (isShow)
        {
            tr.enabled = true;
            tr.Play(true);
        }
        else
        {
            m_goDragonMatchUIBottomEndMatch.transform.localRotation = new Quaternion(0, 0, 0, 0);
            tr.enabled = false;
        }
    }

    #endregion

    #endregion

    #region 探索框

    /// <summary>
    /// 是否显示探索框
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowDragonMatchExploreUI(bool isShow)
    {
        m_goDragonMatchExploreUI.SetActive(isShow);
    }

    public void SetlDragonMatchExploreUIIntroduction(string introduction)
    {
        m_lblDragonMatchExploreUIIntroduction.text = introduction;
    }

    public void SetDragonMatchExploreUITitle(string title)
    {
        m_lblDragonMatchExploreUITitle.text = title;
    }

    /// <summary>
    /// 隐藏或显示探索界面探索按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowDragonMatchExploreUIBtnExplore(bool isShow)
    {
        m_goDragonMatchExploreUIBtnExplore.SetActive(isShow);
    }

    #endregion

    #region 玩家信息框

    /// <summary>
    /// 是否显示玩家信息框
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowDragonMatchPlayerInfoUI(bool isShow)
    {
        m_goDragonMatchPlayerInfoUI.SetActive(isShow);
    }

    /// <summary>
    /// 设置玩家信息
    /// </summary>
    /// <param name="data"></param>
    public void SetDragonMatchPlayerInfo(DragonMatchPlayerInfo data)
    {
        m_lblDragonMatchPlayerInfoUILevel.text = data.level;
        m_lblDragonMatchPlayerInfoUIName.text = data.name;
        m_lblDragonMatchPlayerInfoUIPowerText.text = data.power;
        m_lblDragonMatchPlayerInfoUITongText.text = data.tong;
        m_lblDragonMatchPlayerInfoUIDragonText.text = data.dragon;
        m_lblDragonMatchPlayerInfoUIStatusText.text = data.status;
        m_lblDragonMatchPlayerInfoUIHitTimesText.text = data.hitTimes;
        m_lblDragonMatchPlayerInfoUIHitObtainGold.text = data.hitObtainGold;
        m_lblDragonMatchPlayerInfoUIHitObtainExp.text = data.hitObtainExp;
    }

    /// <summary>
    /// 显示或隐藏玩家信息框袭击按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowDragonMatchPlayerInfoUIBtnHit(bool isShow)
    {
        m_goDragonMatchPlayerInfoUIBtnHit.SetActive(isShow);
    }


    #endregion

    #region 宝箱奖励框

    /// <summary>
    /// 是否显示宝箱奖励框
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowDragonMatchBeginTreasureUI(bool isShow)
    {
        m_goDragonMatchBeginTreasureUI.SetActive(isShow);
    }

    /// <summary>
    /// 设置宝箱奖励金币
    /// </summary>
    /// <param name="gold"></param>
    public void SetBeginTreasureUIRewardGold(int gold)
    {
        m_lblDragonMatchBeginTreasureUIRewardGoldText.text = string.Format(LanguageData.GetContent(48203), gold);
    }

    /// <summary>
    /// 设置宝箱奖励经验
    /// </summary>
    /// <param name="exp"></param>
    public void SetBeginTreasureUIRewardExp(int exp)
    {
        m_lblDragonMatchBeginTreasureUIRewardExpText.text = string.Format(LanguageData.GetContent(48204), exp);
    }

    #endregion

    #region 场景和飞龙(需要确保加载完成)

    #region 场景

    private Camera m_camScene;
    private GameObject m_goDragonMatchScene;

    /// <summary>
    /// 加载飞龙大赛场景
    /// </summary>
    private bool IsDragonMatchSceneLoaded = false;
    public void LoadDragonMatchScene(Action action = null)
    {
        if (!IsDragonMatchSceneLoaded)
        {
            IsDragonMatchSceneLoaded = true;

            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            AssetCacheMgr.GetInstance("fly.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.transform.localPosition = new Vector3(8888, 8888, 1);
                m_goDragonMatchScene = obj;
                m_camScene = obj.transform.Find("Main Camera").GetComponentsInChildren<Camera>(true)[0];

                // 寻找5飞龙Parent
                m_listDragonParent.Clear();
                m_listDragonOrigPos.Clear();
                m_listDragonTP.Clear();

                for (int i = 1; i <= MAX_DRAGON; i++)
                {
                    GameObject goDragon = obj.transform.Find("NPC_2061_0" + i).gameObject;
                    goDragon.SetActive(false);
                    goDragon.transform.name = string.Concat("DragonMatchUIDragon", i);

                    // 飞龙点击设置
                    DragonMatchUIDragonButton dragonButton = goDragon.AddComponent<DragonMatchUIDragonButton>();
                    dragonButton.Index = i - 1;
                    dragonButton.DragonCamera = m_camScene;
                    m_listDragonParent.Add(goDragon);
                    m_listDragonOrigPos.Add(goDragon.transform.localPosition);

                    // 飞龙随机移动设置
                    TweenPosition tp = goDragon.AddComponent<TweenPosition>();
                    tp.enabled = false;
                    m_listDragonTP.Add(tp);
                    MoveDragonRandom(tp);
                }

                if (action != null)
                    action();

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
        }
    }

    #endregion

    #region 设置飞龙

    /// <summary>
    /// 设置飞龙数据
    /// </summary>
    /// <param name="listPlayerName"></param>
    public void SetDragonDataList(List<DragonPlayerInfo> listDragonPlayerInfo)
    {
        m_listDragonPlayerInfoData = listDragonPlayerInfo;
        if (m_listDragonParent.Count == MAX_DRAGON)
        {
            LoadDragonUIInfoList();
        }
    }

    /// <summary>
    /// 加载飞龙并设置飞龙UI信息
    /// </summary>
    private void LoadDragonUIInfoList()
    {
        //DragonMatchPlayerModelPool.Instance.RecirleAll();
        for (int index = 0; index < m_listDragonParent.Count; index++)
        {
            if (index < m_listDragonPlayerInfoData.Count)
            {
                m_listDragonParent[index].SetActive(true);

                int theLoadDragonIndex = index;

                LoadDragonPrefab(index, m_listDragonPlayerInfoData[index].quality, (go) =>
                {
                    var g = go;

                    INSTANCE_COUNT++;
                    MogoGlobleUIManager.Instance.ShowWaitingTip(true);
                    ////加载角色并骑在龙上 m_listDragonPlayerInfoData
                    AddPlayerToDragon(theLoadDragonIndex, g, () =>
                    {
                        //Debug.LogError("DeleteOldPlayer");
                        DeleteOldPlayer(g);

                      

                        INSTANCE_COUNT--;
                        if (INSTANCE_COUNT <= 0)
                            MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                    });

                    SetDragonUIInfo(theLoadDragonIndex);
                });
            }
            else
            {
                m_listDragonParent[index].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 设置飞龙UI信息
    /// </summary>
    /// <param name="theLoadDragonIndex"></param>
    private void SetDragonUIInfo(int theLoadDragonIndex)
    {
        if (m_maplistDragonPlayerInfoUI.ContainsKey(theLoadDragonIndex))
        {
            uint playerID = (uint)m_listDragonPlayerInfoData[theLoadDragonIndex].dbid;
            Transform tranName = m_listDragonParent[theLoadDragonIndex].transform.Find("slot_billboard");
        }      
    }

    /// <summary>
    /// 随机移动龙
    /// </summary>
    private List<TweenPosition> m_listDragonTP = new List<TweenPosition>();
    readonly static float DragonMoveDuration = 30.0f;
    private void MoveDragonRandom(TweenPosition tp)
    {
        if (tp != null)
        {
            Vector3 fromPos = tp.transform.localPosition;
            tp.Reset();
            tp.style = UITweener.Style.PingPong;
            tp.method = UITweener.Method.EaseInOut;
            tp.duration = DragonMoveDuration;
            tp.eventReceiver = gameObject;
            tp.callWhenFinished = "MoveDragonRandom";
            tp.animationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 0.8f), new Keyframe(0.8f, 0.98f), new Keyframe(1f, 1f));

            DragonMatchUIDragonButton dragonButton = tp.transform.GetComponentsInChildren<DragonMatchUIDragonButton>(true)[0];
            if (dragonButton.Index < m_listDragonOrigPos.Count && dragonButton.Index < m_listDragonParent.Count)
            {
                Vector3 toPos = m_listDragonOrigPos[dragonButton.Index];
                float random_x = UnityEngine.Random.Range(-0.5f, 0.5f);
                float random_y = UnityEngine.Random.Range(-0.3f, 0.4f);
                toPos = toPos + new Vector3(random_x, random_y, 0);

                tp.from = fromPos;
                tp.to = toPos;
                tp.enabled = true;
                tp.Play(true);
            }
        }
    }

    #endregion

    #region 加载飞龙

    readonly static private int MAX_DRAGON = 5;

    private List<GameObject> m_listDragonParent = new List<GameObject>();
    private List<Vector3> m_listDragonOrigPos = new List<Vector3>();

    private List<DragonPlayerInfo> m_listDragonPlayerInfoData = new List<DragonPlayerInfo>();
    private Dictionary<int, DragonUIInfo> m_maplistDragonPlayerInfoUI = new Dictionary<int, DragonUIInfo>();

    public struct DragonUIInfo
    {
        public int index;
        public int quality;
        public GameObject goDragon;
    }

    /// <summary>
    /// 当前加载的龙是否是目标的龙
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool CurrentIsTargetDragon(int index)
    {
        if (m_maplistDragonPlayerInfoUI.ContainsKey(index) && index < m_listDragonPlayerInfoData.Count)
        {
            if (m_maplistDragonPlayerInfoUI[index].quality == m_listDragonPlayerInfoData[index].quality)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 加载飞龙
    /// </summary>
    /// <param name="index"></param>
    /// <param name="quality">绿色飞龙quality = 2</param>
    /// <param name="action"></param>
    private void LoadDragonPrefab(int index, int quality, Action<GameObject> action = null)
    {
        if (index >= m_listDragonParent.Count)
        {
            LoggerHelper.Error("index out of bounds");
            return;
        }

        if (!(quality >= 2 && quality <= 6))
        {
            LoggerHelper.Error("quality out of bounds");
            quality = 2; // 飞龙品质越界，默认为绿色飞龙
        }
      
        if (m_maplistDragonPlayerInfoUI.ContainsKey(index))
        {          
            if (m_maplistDragonPlayerInfoUI[index].quality == quality) // 飞龙已经加载并且品质一样
            {
                if (action != null)
                    action(m_maplistDragonPlayerInfoUI[index].goDragon);

                return;
            }
            else // 飞龙已经加载但品质不一样,先释放旧的飞龙
            {
                AssetCacheMgr.ReleaseInstance(m_maplistDragonPlayerInfoUI[index].goDragon);
                m_maplistDragonPlayerInfoUI.Remove(index);
            }
        }  

        // 加载新的飞龙
        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        AssetCacheMgr.GetUIInstance(string.Concat("NPC_206", quality - 1, ".prefab"), (prefab, guid, go) =>
        {
            GameObject obj = (GameObject)go;
            obj.transform.parent = m_listDragonParent[index].transform;
            obj.transform.localScale = new Vector3(1f, 1f, 1f);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.AngleAxis(180, new Vector3(0, 1, 0));
            obj.transform.name = "DragonMatchUIDragon";

            DragonMatchUIDragonButton dragonButton = obj.AddComponent<DragonMatchUIDragonButton>();
            dragonButton.Index = index;
            dragonButton.DragonCamera = m_camScene;

            obj.SetActive(true);

            DragonUIInfo dragonInfo;
            dragonInfo.index = index;
            dragonInfo.quality = quality;
            dragonInfo.goDragon = obj;

            // 解决异步的两条龙重叠问题
            if (m_maplistDragonPlayerInfoUI.ContainsKey(index))
            {
                if (!CurrentIsTargetDragon(index)) // 飞龙已经加载但品质不一样
                {
                    AssetCacheMgr.ReleaseInstance(m_maplistDragonPlayerInfoUI[index].goDragon);
                    m_maplistDragonPlayerInfoUI.Remove(index);
                    m_maplistDragonPlayerInfoUI[index] = dragonInfo;
                }
                else // 飞龙已经加载并且品质一样
                {
                    AssetCacheMgr.ReleaseInstance(dragonInfo.goDragon);
                }               
            }
            else
            {
                m_maplistDragonPlayerInfoUI[index] = dragonInfo;
            }            

            if (action != null)
                action(obj);

            INSTANCE_COUNT--;
            if (INSTANCE_COUNT <= 0)
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
        });
    }

    /// <summary>
    /// 释放飞龙
    /// </summary>
    private void ReleaseDragon()
    {
        for (int i = 0; i < m_listDragonParent.Count; i++)
        {
            if (m_maplistDragonPlayerInfoUI.ContainsKey(i))
            {
                DeleteOldPlayer(m_maplistDragonPlayerInfoUI[i].goDragon);
                AssetCacheMgr.ReleaseInstance(m_maplistDragonPlayerInfoUI[i].goDragon);
            }            
        }

        m_maplistDragonPlayerInfoUI.Clear();
    }

    #endregion

    #region 玩家模型

    //Dictionary<int, Queue<GameObject>> m_goPlayerDic;

    //private void InitCharactorPool()
    //{
    //    if (m_goPlayerDic != null) return;
    //    m_goPlayerDic = new Dictionary<int, Queue<GameObject>>();
    //    m_goPlayerDic[1] = new Queue<GameObject>();
    //    m_goPlayerDic[2] = new Queue<GameObject>();
    //    m_goPlayerDic[3] = new Queue<GameObject>();
    //    m_goPlayerDic[4] = new Queue<GameObject>();

    //    //如果没创建角色模型池就创建一个
    //    throw new NotImplementedException();
    //}

    private void AddPlayerToDragon(int index, GameObject go, Action ondone)
    {
        //Debug.LogError("AddPlayerToDragon:" + index);
        DragonPlayerInfo info = m_listDragonPlayerInfoData[index];

        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        //Debug.LogError("cuirass:" + (int)info.cuirass + ",weapon:" + (int)info.weapon);
        LoadCharactor((int)info.vocation, (int)info.cuirass, (int)info.weapon, (goPlayer) =>
        {
            //Debug.LogError("LoadCharactor done");
            goPlayer.transform.localPosition = Vector3.zero;
            ondone();
            Utils.MountToSomeObjWithoutPosChange(goPlayer.transform, MogoUtils.GetChild(go.transform, "Bip_ride"));
            goPlayer.transform.localEulerAngles = new Vector3(90, 0, 0);
            goPlayer.GetComponent<Animator>().SetInteger("Action", 61);
            goPlayer.GetComponent<ActorParent>().AddCallbackInFrames(() => { goPlayer.GetComponent<Animator>().SetInteger("Action", 999); });

            //加载billBoard
            INSTANCE_COUNT++;
            AssetCacheMgr.GetInstance("DragonMatchBillBoard.prefab",
                (str, id, obj) =>
                {
                    GameObject billBoardGo = obj as GameObject;
                    Utils.MountToSomeObjWithoutPosChange(billBoardGo.transform, MogoUtils.GetChild(goPlayer.transform, "slot_billboard"));
                    UILabel lbl = billBoardGo.GetComponent<UILabel>();
                    string level = "LV" + m_listDragonPlayerInfoData[index].level;
                    string name =m_listDragonPlayerInfoData[index].name;
                    if (index == 0)
                    {
                        level = string.Concat("[5ef5ff]", level, "[-]");
                        name = string.Concat("[13C5D9]", name, "[-]");
                    }
                    else
                    {
                        level = string.Concat("[84e747]", level, "[-]");
                        name = string.Concat("[11AE21]", name, "[-]");
                    }

                    if (MogoUIManager.IsShowLevel)
                    {
                        lbl.text = level + " " + name;
                    }
                    else
                    {
                        lbl.text = name;
                    }

                    INSTANCE_COUNT--;
                    if (INSTANCE_COUNT <= 0) MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                });


            INSTANCE_COUNT--;
            if (INSTANCE_COUNT <= 0)
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go">龙的go</param>
    private void DeleteOldPlayer(GameObject go)
    {
        if (go == null) return;
        Transform t = MogoUtils.GetChild(go.transform, "Bip_ride");
        foreach (Transform child in t)
        {
            if (child.name == "Bip_ride") continue;
            Animator animator = child.GetComponent<Animator>();
            ActorParent actor = child.GetComponent<ActorParent>();

            if (animator != null)
            {
                AssetCacheMgr.ReleaseResource(animator.runtimeAnimatorController);
            }

            if (actor != null)
            {
                actor.RemoveAll();
            }

            //Debug.LogError("ReleaseInstance:" + child.gameObject.name);
            AssetCacheMgr.ReleaseInstance(child.gameObject);
        }
    }

    /// <summary>
    /// 加载人物模型
    /// </summary>
    /// <param name="vocation"></param>
    /// <param name="cuirass"></param>
    /// <param name="weapon"></param>
    /// <param name="action"></param>
    private void LoadCharactor(int vocation, int cuirass, int weapon, Action<GameObject> action)
    {
        List<int> equipList = new List<int>();
        //cuirass = ItemEquipmentData.dataMap.Get(cuirass).mode;
        //weapon = ItemEquipmentData.dataMap.Get(weapon).mode;
        equipList.Add(ItemEquipmentData.dataMap.Get(weapon).mode);
        equipList.Add(ItemEquipmentData.dataMap.Get(cuirass).mode);
        var modelDAta = AvatarModelData.dataMap.GetValueOrDefault((int)vocation, new AvatarModelData());

        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        //Debug.LogError("GetUIInstance:" + modelDAta.prefabName);
        AssetCacheMgr.GetUIInstance(modelDAta.prefabName, (prefab, id, go) =>
        {
            var ety = new EtyAvatar();
            ety.vocation = vocation;
            ety.equipList = equipList;
            ety.weapon = weapon;
            ety.CreatePosition = Vector3.zero;
            var avatar = (go as GameObject);
            ety.gameObject = avatar;
            avatar.name = "DragonMatchModelPool" + vocation.ToString();
            var cc = avatar.GetComponent<Collider>() as CharacterController;
            cc.radius = 0.5f;
            ety.animator = avatar.GetComponent<Animator>();
            ety.animator.applyRootMotion = false;
            ety.sfxHandler = avatar.AddComponent<SfxHandler>();

            ety.actorParent = avatar.AddComponent<ActorParent>();
            ety.actorParent.InitEquipment(vocation);

            INSTANCE_COUNT--;
            if (INSTANCE_COUNT <= 0)
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            //Debug.LogError("GetUIInstance done");

            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);
            //Debug.LogError("Equip");
            ety.actorParent.Equip(equipList, () =>
            {
                avatar.transform.localPosition = Vector3.zero;
                avatar.transform.localScale = Vector3.one;

                //Debug.LogError("weapon:" + weapon);
                ItemEquipmentData equip = ItemEquipmentData.dataMap.Get(weapon);
                int subtype = equip.subtype;
                int type = equip.type;
                ControllerOfWeaponData controllerData = ControllerOfWeaponData.dataMap[subtype];
                RuntimeAnimatorController controller;
                Animator animator = ety.animator;

                string controllerName = controllerData.controllerInCity;
                if (animator.runtimeAnimatorController != null)
                {
                    if (animator.runtimeAnimatorController.name == controllerName)
                    {
                        action(go as GameObject);
                        Debug.LogError("animator.runtimeAnimatorController.name == controllerName");
                        return;
                    }

                    AssetCacheMgr.ReleaseResource(animator.runtimeAnimatorController);
                }

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                //Debug.LogError("Equip done");

                INSTANCE_COUNT++;
                MogoGlobleUIManager.Instance.ShowWaitingTip(true);
                //Debug.LogError("GetResource:" + controllerName);
                AssetCacheMgr.GetResource(controllerName, (obj) =>
                {
                    controller = obj as RuntimeAnimatorController;
                    animator.runtimeAnimatorController = controller;

                    action(go as GameObject);
                    //(GetEntity() as EntityMyself).UpdateSkillToManager();
                    //EventDispatcher.TriggerEvent<int, int>(InventoryEvent.OnChangeEquip, type, subtype);

                    INSTANCE_COUNT--;
                    if (INSTANCE_COUNT <= 0)
                        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
                });
            });
        });
    }

    private void ReleasePlayerModel()
    {
        for (int i = 0; i < m_listDragonParent.Count; i++)
        {
            if (m_maplistDragonPlayerInfoUI.ContainsKey(i))
                DeleteOldPlayer(m_maplistDragonPlayerInfoUI[i].goDragon);
        }
    }

    #endregion

    #endregion

    #region 界面打开和关闭

    /// <summary>
    /// 展示界面信息
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        MogoGlobleUIManager.Instance.ShowWaitingTip(false);
        RenderSettings.fog = false; // 雾效影响fly场景，故关闭
        EventDispatcher.TriggerEvent(Events.GearEvent.SwitchLightMapFog, 913);

        // 加载场景资源并加载设置飞龙
        LoadDragonMatchScene(() =>
            {
                LoadDragonUIInfoList();
            });

        // 加载宝箱可领取特效
        AttachTreasureActiveAnimation(() =>
            {
                ShowTreasureActiveAnimation(IsShowTreasureActiveAnimation);
            });

        InitCharactorPool();
    }

    /// <summary>
    /// 初始化人物模型池
    /// </summary>
    private void InitCharactorPool()
    {
        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        DragonMatchPlayerModelPool.Instance.InitPool(() =>
        {
            INSTANCE_COUNT--;
            if (INSTANCE_COUNT <= 0)
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
        });
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {        
        // 卸载飞龙
        ReleaseDragon();

        // 卸载飞龙场景
        IsDragonMatchSceneLoaded = false;
        if (m_goDragonMatchScene != null)
            AssetCacheMgr.ReleaseInstance(m_goDragonMatchScene);
        AssetCacheMgr.ReleaseResourceImmediate("fly.prefab");

        // 雾效切换
        RenderSettings.fog = true;
        if (MogoWorld.thePlayer.sceneId == MogoWorld.globalSetting.homeScene)
            EventDispatcher.TriggerEvent(Events.GearEvent.SwitchLightMapFog, MogoWorld.globalSetting.homeScene);

        //ReleasePlayerModel();

        // 卸载宝箱特效
        ReleaseTreasureActiveAnimation();

        ShowHitCD(false); // release CD
        ShowBeginInfo(false); // release CD

        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyDragonMatchUI();
        }
    }

    #endregion
}
