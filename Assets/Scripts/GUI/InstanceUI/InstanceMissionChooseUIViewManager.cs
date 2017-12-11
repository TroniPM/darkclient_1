#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：2013/10/30
// 模块描述：副本关卡选择界面，可拖动
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;
using System;

public enum InstanceMissionChooseUIStatus
{
    /// <summary>
    /// 普通副本
    /// </summary>
    Normal,
    /// <summary>
    /// 随机副本
    /// </summary>
    Random,
}

public class InstanceMissionChooseUIViewManager : MogoUIBehaviour 
{
    private static InstanceMissionChooseUIViewManager m_instance;
    public static InstanceMissionChooseUIViewManager Instance { get { return InstanceMissionChooseUIViewManager.m_instance; } }
    
    private static int INSTANCE_COUNT = 0; // 异步加载资源数量

    private UILabel m_lblSweepNum;
    private UILabel m_lblResetNum;    
    private Camera m_dragCamera;
    private MyDragableCamera m_dragableCameraMapList;
    private GameObject m_goInstanceMissionChooseUIMapListGrid;
    private Transform m_tranInstanceMissionChooseUIPageDOT;

    private Camera m_camInstanceMissionChooseUI;
    public Camera CamInstanceMissionChooseUI
    {
        get { return m_camInstanceMissionChooseUI; }
    }

    private UICamera m_uicamInstanceMissionChooseUI;
    public UICamera UICamInstanceMissionChooseUI
    {
        get { return m_uicamInstanceMissionChooseUI;}
    }    
    
    private GameObject m_goGOInstanceMissionChooseUI;
    private GameObject m_goGOInstanceMissionChooseUIMapList;
    private GameObject m_goGOInstanceMissionChooseUINormal;
    private GameObject m_goGOInstanceMissionChooseUIRandom;

    private MogoSingleButtonList m_chooseMogoSingleButtonList;
    private UILabel m_lblInstanceMissionChooseUIChooseNormalText;
    private UILabel m_lblInstanceMissionChooseUIChooseNormalTextDown;
    private UILabel m_lblInstanceMissionChooseUIChooseRandomText;
    private UILabel m_lblInstanceMissionChooseUIChooseRandomTextDown;
    private UILabel m_lblInstanceMissionChooseUIRandomBtnEnterTip;

    private UISprite m_spInstanceMissionChooseUIChooseRandomBGUp;
    private UISprite m_spInstanceMissionChooseUIChooseRandomBGDown;
    private GameObject m_goInstanceMissionChooseUIRandomFx;

    void Awake()
    {
        m_myTransform = transform;
        FillFullNameData(m_myTransform);
        m_instance = m_myTransform.GetComponentsInChildren<InstanceMissionChooseUIViewManager>(true)[0];

        m_lblSweepNum = m_myTransform.Find(m_widgetToFullName["InstanceLeftChallengeNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblResetNum = m_myTransform.Find(m_widgetToFullName["InstanceLeftResetNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_camInstanceMissionChooseUI = FindTransform("InstanceMissionChooseUICamera").GetComponentsInChildren<Camera>(true)[0];
        m_uicamInstanceMissionChooseUI = m_myTransform.Find(m_widgetToFullName["InstanceMissionChooseUICamera"]).GetComponentsInChildren<UICamera>(true)[0];
  
        m_goInstanceMissionChooseUIMapListGrid = m_myTransform.Find(m_widgetToFullName["InstanceMissionChooseUIMapListGrid"]).gameObject;
        m_tranInstanceMissionChooseUIPageDOT = FindTransform("InstanceMissionChooseUIPageDOT");
        m_goGOInstanceMissionChooseUI = m_myTransform.Find(m_widgetToFullName["GOInstanceMissionChooseUI"]).gameObject;
        m_goGOInstanceMissionChooseUI.SetActive(false);

        m_goGOInstanceMissionChooseUIMapList = FindTransform("GOInstanceMissionChooseUIMapList").gameObject;
        m_goGOInstanceMissionChooseUINormal = FindTransform("GOInstanceMissionChooseUINormal").gameObject;
        m_goGOInstanceMissionChooseUIRandom = FindTransform("GOInstanceMissionChooseUIRandom").gameObject;

        m_chooseMogoSingleButtonList = FindTransform("InstanceMissionChooseUIChoose").GetComponentsInChildren<MogoSingleButtonList>(true)[0];
        m_chooseMogoSingleButtonList.SetCurrentDownButton(0);
        m_lblInstanceMissionChooseUIChooseNormalText = FindTransform("InstanceMissionChooseUIChooseNormalText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblInstanceMissionChooseUIChooseNormalTextDown = FindTransform("InstanceMissionChooseUIChooseNormalTextDown").GetComponentsInChildren<UILabel>(true)[0];
        m_lblInstanceMissionChooseUIChooseRandomText = FindTransform("InstanceMissionChooseUIChooseRandomText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblInstanceMissionChooseUIChooseRandomTextDown = FindTransform("InstanceMissionChooseUIChooseRandomTextDown").GetComponentsInChildren<UILabel>(true)[0];
        m_lblInstanceMissionChooseUIRandomBtnEnterTip = FindTransform("InstanceMissionChooseUIRandomBtnEnterTip").GetComponentsInChildren<UILabel>(true)[0];

        m_spInstanceMissionChooseUIChooseRandomBGUp = FindTransform("InstanceMissionChooseUIChooseRandomBGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_spInstanceMissionChooseUIChooseRandomBGDown = FindTransform("InstanceMissionChooseUIChooseRandomBGDown").GetComponentsInChildren<UISprite>(true)[0];

        m_dragCamera = m_myTransform.Find(m_widgetToFullName["InstanceMissionChooseUIMapListCamera"]).GetComponentsInChildren<Camera>(true)[0];
        Camera SourceCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_dragCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = SourceCamera;

        m_dragableCameraMapList = m_dragCamera.transform.GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_dragableCameraMapList.LeftArrow = FindTransform("InstanceMissionChooseUIArrowL").gameObject;
        m_dragableCameraMapList.RightArrow = FindTransform("InstanceMissionChooseUIArrowR").gameObject;

        FillNewInstanceUIChooseGridData();

        // 随机副本特效
        m_goInstanceMissionChooseUIRandomFx = FindTransform("InstanceMissionChooseUIRandomFx").gameObject;

        // ChineseData
        m_lblInstanceMissionChooseUIChooseRandomText.text = LanguageData.GetContent(46975);
        m_lblInstanceMissionChooseUIChooseRandomTextDown.text = LanguageData.GetContent(46975);
        m_lblInstanceMissionChooseUIRandomBtnEnterTip.text = LanguageData.GetContent(46976);

        Initialize();
    }

    #region 事件

    public Action<int> MAPPAGEMOVEDONE;
    public Action BOSSCHESTBTNUP;
    public Action MISSIONRANDOMENTERUP;

    void Initialize()
    {
        InstanceUIDict.ButtonTypeToEventUp.Add("NewInstanceBossChestButton", OnBossChestButtonUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceMissionChooseUIChooseNormal", OnChooseNormalUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceMissionChooseUIChooseRandom", OnChooseRandomUp);
        InstanceUIDict.ButtonTypeToEventUp.Add("InstanceMissionChooseUIRandomBtnEnter", OnMissionRandomEnterUp);        
    }

    void MovePageDone()
    {
        if (MAPPAGEMOVEDONE != null)
            MAPPAGEMOVEDONE(GetCurrentMapPage());
    }

    void OnBossChestButtonUp(int i)
    {
        if (BOSSCHESTBTNUP != null)
            BOSSCHESTBTNUP();
    }

    /// <summary>
    /// 选择普通副本
    /// </summary>
    /// <param name="i"></param>
    void OnChooseNormalUp(int i)
    {
        SwitchMissionNormal();
    }

    /// <summary>
    /// 选择随机副本
    /// </summary>
    /// <param name="i"></param>
    void OnChooseRandomUp(int i)
    {
        SwithcMissionRandom();
    }

    /// <summary>
    /// 点击进入随机副本
    /// </summary>
    /// <param name="i"></param>
    void OnMissionRandomEnterUp(int i)
    {
        if (MISSIONRANDOMENTERUP != null)
            MISSIONRANDOMENTERUP();
    }
        
    #endregion

    #region 普通副本和随机副本切换

    /// <summary>
    /// 当前UI状态
    /// </summary>
    private InstanceMissionChooseUIStatus m_currentUIStatus = InstanceMissionChooseUIStatus.Normal;
    public InstanceMissionChooseUIStatus CurrentUIStatus
    {
        get { return m_currentUIStatus; }
        set
        {
            m_currentUIStatus = value;
            if(m_currentUIStatus == InstanceMissionChooseUIStatus.Random)
                ShowRandomEnterFx(true);
            else
                ShowRandomEnterFx(false);
        }
    }

    /// <summary>
    /// 切换到普通副本
    /// </summary>
    private void SwitchMissionNormal()
    {
        CurrentUIStatus = InstanceMissionChooseUIStatus.Normal;
        m_chooseMogoSingleButtonList.SetCurrentDownButton(0);
        m_goGOInstanceMissionChooseUIMapList.SetActive(true);
        m_goGOInstanceMissionChooseUINormal.SetActive(true);
        m_goGOInstanceMissionChooseUIRandom.SetActive(false);
    }

    /// <summary>
    /// 切换到随机副本
    /// </summary>
    private void SwithcMissionRandom()
    {
        if (MogoWorld.thePlayer != null && MogoWorld.thePlayer.level >= SystemRequestLevel.MISSIONRANDOM)
        {
            CurrentUIStatus = InstanceMissionChooseUIStatus.Random;
            m_chooseMogoSingleButtonList.SetCurrentDownButton(1);
            m_goGOInstanceMissionChooseUIMapList.SetActive(false);
            m_goGOInstanceMissionChooseUINormal.SetActive(false);
            m_goGOInstanceMissionChooseUIRandom.SetActive(true);
        }
        else
        {
            m_chooseMogoSingleButtonList.SetCurrentDownButton(0);
            MogoMsgBox.Instance.ShowFloatingText(string.Format(LanguageData.GetContent(28051), SystemRequestLevel.MISSIONRANDOM));
        }        
    }

    #endregion

    #region 副本逻辑

    #region 控制变量

    public readonly static bool SHOW_MISSION_BY_DRAG = true;
    readonly static int MAX_MAP_NUM = 7; // 地图数量
    private Dictionary<int, InstanceMapWithMissionGrid> m_maplistMapWithMissionGrid = new Dictionary<int, InstanceMapWithMissionGrid>();

    int m_MapOpenPage = 0;
    public int MapOpenPage
    {
        get
        {
            return m_MapOpenPage;
        }
    }

    public int GetCurrentMapPage()
    {
        return m_goInstanceMissionChooseUIMapListGrid.GetComponentsInChildren<MyDragableCamera>(true)[0].GetCurrentPage();
    }

    public void SetCurrentMapPage(int page)
    {
        m_goInstanceMissionChooseUIMapListGrid.GetComponentsInChildren<MyDragableCamera>(true)[0].SetCurrentPage(page);
    }

    public void SetMapOpenPage(int count)
    {
        m_MapOpenPage = count;
        m_dragableCameraMapList.transformList.Clear();
        for (int i = 0; i < count && i < m_maplistInstanceMapWithMissionGridPos.Count; i++)
        {
            if (m_maplistInstanceMapWithMissionGridPos.ContainsKey(i))
                m_dragableCameraMapList.transformList.Add(m_maplistInstanceMapWithMissionGridPos[i]);
            else
                LoggerHelper.Error("m_maplistInstanceMapWithMissionGridPos no contains key = " + i);
        }

        AddMapDOTPageList();
    }

    /// <summary>
    /// 创建页点
    /// </summary>
    private int m_iMapPageNum = 0;
    private void AddMapDOTPageList()
    {
        m_dragableCameraMapList.DestroyDOTPageList();
        m_iMapPageNum = 0;
        if (MapOpenPage <= 1)
            return;

        for (int i = 0; i < MapOpenPage; i++)
        {
            // 创建页数点
            ++m_iMapPageNum;
            int num = m_iMapPageNum;
            AssetCacheMgr.GetUIInstance("ChooseServerPage.prefab", (prefabPage, idPage, goPage) =>
            {
                GameObject objPage = (GameObject)goPage;

                objPage.transform.parent = m_tranInstanceMissionChooseUIPageDOT;
                objPage.transform.localPosition = new Vector3((num - 1) * 40, 0, 0);
                objPage.transform.localScale = Vector3.one;
                objPage.name = "MapPageNum" + num;
                m_dragableCameraMapList.ListPageDown.Add(objPage.GetComponentsInChildren<UISprite>(true)[1].gameObject);
                m_tranInstanceMissionChooseUIPageDOT.localPosition = new Vector3(-20 * (num - 1), m_tranInstanceMissionChooseUIPageDOT.localPosition.y, 0);

                // 选择当前页
                if (num - 1 == m_dragableCameraMapList.GetCurrentPage())
                    objPage.GetComponentsInChildren<UISprite>(true)[1].gameObject.SetActive(true);
                else
                    objPage.GetComponentsInChildren<UISprite>(true)[1].gameObject.SetActive(false);
                m_dragableCameraMapList.GODOTPageList = m_tranInstanceMissionChooseUIPageDOT.gameObject;
                m_dragableCameraMapList.SetCurrentPage(m_dragableCameraMapList.GetCurrentPage());
            });
        }
    }
  
    #endregion

    #region 创建MAX_MAP_NUM个地图

    /// <summary>
    /// 创建MAX_MAP_NUM个地图大关卡
    /// </summary>
    private Dictionary<int, Transform> m_maplistInstanceMapWithMissionGridPos = new Dictionary<int, Transform>();
    public void FillNewInstanceUIChooseGridData()
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        m_maplistMapWithMissionGrid.Clear();

        int num = MAX_MAP_NUM;
        for (int i = 0; i < num; ++i)
        {
            int index = i;

            AssetCacheMgr.GetUIInstance("InstanceMapWithMissionGrid.prefab", (prefab, guid, gameObject) =>
            {
                GameObject go = (GameObject)gameObject;

                go.transform.parent = m_goInstanceMissionChooseUIMapListGrid.transform;
                go.transform.localPosition = new Vector3(index * 1280, 0, 0);
                go.transform.localScale = Vector3.one;
                go.name = string.Concat("InstanceMapWithMissionGrid", index);
                InstanceMapWithMissionGrid mapGrid = go.AddComponent<InstanceMapWithMissionGrid>();                
                mapGrid.gameObject.SetActive(true);
				mapGrid.ResetNewInstanceUIChooseLevelGridPos(index + 1);
                mapGrid.FillNewInstanceUIChooseLevelGridData(10, index);
                m_maplistMapWithMissionGrid[index] = mapGrid;

                MyDragCamera camera = go.AddComponent<MyDragCamera>();
                camera.RelatedCamera = m_dragCamera;

                // 创建翻页位置
                GameObject trans = new GameObject();
                trans.transform.parent = m_dragCamera.transform;
                trans.transform.localPosition = new Vector3(index * 1.0f * 1280, 0, 0);
                trans.transform.localEulerAngles = Vector3.zero;
                trans.transform.localScale = new Vector3(1, 1, 1);
                trans.name = "InstanceMapWithMissionGridPos" + index;
                m_maplistInstanceMapWithMissionGridPos[index] = trans.transform;

                if (index == num - 1)
                {
                    MogoUIManager.Instance.ShowInstanceMissionChooseUI(false);
                    m_goGOInstanceMissionChooseUI.SetActive(true);
                }
            });
        }

        m_dragableCameraMapList.MAXX = num * 1.0f * 1280 + 1280;
        m_dragableCameraMapList.MovePageDone += MovePageDone;
    }

    #endregion

    #region 普通关卡

    /// <summary>
    /// 重新设置关卡位置
    /// </summary>
    public void ResetChooseLevelGridPos(int mapId, int level)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].ResetNewInstanceUIChooseLevelGridPos(level);
    }

    /// <summary>
    /// 隐藏所有关卡
    /// </summary>
    public void HideChooseLevelGridList(int mapId)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].HideNewInstanceUIChooseLevelGridList();
    }

    #endregion

    #region 特殊关卡

    /// <summary>
    /// 第一次设置：当逻辑传的值>=0并且m_MissionFoggyAbyss<0
    /// 只是刷新：当逻辑传的值>=0并且m_MissionFoggyAbyss>=0
    /// 清除：当逻辑传的值<0
    /// </summary>
    private int m_MissionFoggyAbyss = -1;
    public int MissionFoggyAbyssMapID
    {
        get { return m_MissionFoggyAbyss; }
    }

    /// <summary>
    /// 是否显示[特殊关卡-迷雾深渊]
    /// </summary>
    /// <param name="mapId">地图ID</param>
    /// <param name="isShow">是否显示</param>    
    /// <param name="iMark">副本难度</param>    
    public void ShowFoggyAbyssMission(int mapId, bool isShow, int iMark = 0)
    {
        if (mapId >= 0 && m_MissionFoggyAbyss < 0)
        {
            if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            {
                m_maplistMapWithMissionGrid[mapId].ShowFoggyAbyssMission(isShow, iMark);
                m_MissionFoggyAbyss = mapId;
            }           
        }
        else if (mapId >= 0 && m_MissionFoggyAbyss >= 0)
        {
            if (m_maplistMapWithMissionGrid.ContainsKey(m_MissionFoggyAbyss))
                m_maplistMapWithMissionGrid[m_MissionFoggyAbyss].ShowFoggyAbyssMission(isShow, iMark);
        }
        else if (mapId < 0)
        {
            for (int i = 0; i < MAX_MAP_NUM; i++)
            {
                if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
                    m_maplistMapWithMissionGrid[i].ShowFoggyAbyssMission(false, iMark);
            }
            m_MissionFoggyAbyss = -1;
        }        
    }

    /// <summary>
    /// 显示或隐藏[特殊关卡-迷雾深渊]悬浮提示
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="isShow"></param>
    public void ShowMissionFoggyAbyssGridTip(bool isShow)
    {
        if (m_MissionFoggyAbyss >= 0 && m_MissionFoggyAbyss < MAX_MAP_NUM)
        {
            if (m_maplistMapWithMissionGrid.ContainsKey(m_MissionFoggyAbyss))
                m_maplistMapWithMissionGrid[m_MissionFoggyAbyss].ShowMissionFoggyAbyssGridTip(isShow);
        }
    }

    #endregion

    #region 路径

    public void PlayChooseLevelPathAnim(int mapId, int id)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].PlayChooseLevelPathAnim(id);
    }

    public void ShowChooseLevelPath(int mapId, int id, bool isShow = true)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].ShowChooseLevelPath(id, isShow);
    }

    #endregion

    #region 设置关卡信息

    public void ShowMarks(int mapId, int gridId, int complexData)
    {
        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].ShowMissionNormalMarks(gridId, complexData);
    }

    public void ShowMarks(int mapId, int gridId, int normalLevel, int hardLevel)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].ShowMissionNormalMarks(gridId, normalLevel, hardLevel);
    }

    public void SetGridEnable(int mapId, int gridId, bool isEnable)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].SetMissionNormalGridEnable(gridId, isEnable);
    }

    public void SetGridIcon(int mapId, int gridId, string icon)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].SetMissionNormalGridIcon(gridId, icon);
    }

    public void SetGridTitle(int mapId, string title)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].SetMissionMapTitle(title);
    }

    #endregion

    #region 悬浮提示(当前任务,掉落副本)

    public void ShowGridTip(int mapId, int gridId, bool isShow = true, int tipTextID = 0)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].ShowMissionNormalGridTip(gridId, isShow, tipTextID);
    }

    public void HideGridTip(int mapId)
    {
        if (SHOW_MISSION_BY_DRAG == false)
            return;

        if (m_maplistMapWithMissionGrid.ContainsKey(mapId))
            m_maplistMapWithMissionGrid[mapId].HideMissionNormalGridTip();
    }

    #endregion    

    #endregion

    #region 界面信息

    /// <summary>
    /// 设置随机副本是否开启
    /// </summary>
    /// <param name="enable"></param>
    public void SetMissionRandomChooseEnable()
    {
        if (MogoWorld.thePlayer != null && MogoWorld.thePlayer.level >= SystemRequestLevel.MISSIONRANDOM)
        {            
            //m_spInstanceMissionChooseUIChooseRandomBGUp.ShowAsWhiteBlack(false);
            m_spInstanceMissionChooseUIChooseRandomBGUp.spriteName = "fb_sksd_down";
            m_spInstanceMissionChooseUIChooseRandomBGDown.enabled = true;
        }
        else
        {
            //m_spInstanceMissionChooseUIChooseRandomBGUp.ShowAsWhiteBlack(true);
            m_spInstanceMissionChooseUIChooseRandomBGUp.spriteName = "fb_wkq";
            m_spInstanceMissionChooseUIChooseRandomBGDown.enabled = false;
        }
    }

    /// <summary>
    /// 设置大关名称
    /// </summary>
    /// <param name="name"></param>
    public void SetMissionNormalName(string name)
    {
        m_lblInstanceMissionChooseUIChooseNormalText.text = name;
        m_lblInstanceMissionChooseUIChooseNormalTextDown.text = name;
    }

    /// <summary>
    /// 设置剩余扫荡次数
    /// </summary>
    /// <param name="num"></param>
    public void SetSweepNum(int num)
    {
        m_lblSweepNum.text = LanguageData.GetContent(46966) + num;
    }

    /// <summary>
    /// 设置剩余挑战重置次数
    /// </summary>
    /// <param name="num"></param>
    public void SetResetNum(string num)
    {
        m_lblResetNum.text = num;
    }

    #endregion

    #region 宝箱动画

    /// <summary>
    /// 是否播放地图宝箱可领取振动动画
    /// </summary>
    /// <param name="isFinished"></param>
    public void ShowMapChestRotationAnimation(bool isFinished)
    {
        for(int i = 0; i < MAX_MAP_NUM; i++)
        {
            if (m_maplistMapWithMissionGrid.ContainsKey(i))
                m_maplistMapWithMissionGrid[i].ShowMapChestRotationAnimation(isFinished);
        }
    }

    /// <summary>
    /// 是否播放Boss宝箱可领取振动动画
    /// </summary>
    /// <param name="isFinished"></param>
    public void ShowBossChestRotationAnimation(bool isFinished)
    {
        for (int i = 0; i < MAX_MAP_NUM; i++)
        {
            if (m_maplistMapWithMissionGrid.ContainsKey(i))
                m_maplistMapWithMissionGrid[i].ShowBossChestRotationAnimation(isFinished);
        }
    }    

    #endregion

    #region 随机副本入口特效

    private readonly static string RandomEnterFxName = "fx_GOInstanceMissionChooseUIRandom.prefab";
    private string m_fxRandomEnter = "RandomEnterFx";
    private GameObject m_goFxRandomEnter;

    /// <summary>
    /// 显示或隐藏随机副本入口特效
    /// </summary>
    private void ShowRandomEnterFx(bool isShow)
    {
        if (isShow)
        {
            m_goFxRandomEnter = MogoFXManager.Instance.FindParticeAnim(m_fxRandomEnter);
            if (m_goFxRandomEnter != null)
            {
                m_goFxRandomEnter.SetActive(true);
            }
            else
            {
                AttachRandomEnterFx();
            }
        }
        else
        {
            m_goFxRandomEnter = MogoFXManager.Instance.FindParticeAnim(m_fxRandomEnter);
            if (m_goFxRandomEnter != null)
            {
                m_goFxRandomEnter.SetActive(false);
            }
        }
    }      

    /// <summary>
    /// 加载随机副本入口特效
    /// </summary>
    private void AttachRandomEnterFx()
    {
        if (m_goInstanceMissionChooseUIRandomFx == null || CamInstanceMissionChooseUI == null)
            return;

        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);

        Vector3 vecParent = m_goInstanceMissionChooseUIRandomFx.transform.position;
        MogoFXManager.Instance.AttachParticleAnim(RandomEnterFxName, m_fxRandomEnter, vecParent,
            CamInstanceMissionChooseUI, 0, 0, 0, () =>
            {
                m_goFxRandomEnter = MogoFXManager.Instance.FindParticeAnim(m_fxRandomEnter);
                if (m_goFxRandomEnter != null)
                {
                    if (CurrentUIStatus == InstanceMissionChooseUIStatus.Random)
                        m_goFxRandomEnter.SetActive(true);
                    else
                        m_goFxRandomEnter.SetActive(false);
                }

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
    }

    /// <summary>
    /// 释放随机副本入口特效
    /// </summary>
    private void ReleaseRandomEnterFx()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fxRandomEnter);
    }

    #endregion

    #region 界面打开和关闭

    protected override void OnEnable()
    {
        base.OnEnable();
        SwitchMissionNormal();
        SetMissionRandomChooseEnable();        
    }

    void OnDisable()
    {
        ShowRandomEnterFx(false);
    }

    #endregion
}
