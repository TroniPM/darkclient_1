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

public enum ArenaUIEnemyTab
{   
    Weak = 0,
    Strong,
    Revenge,
}

public class NewArenaUIViewManager : MogoUIBehaviour 
{
    private static NewArenaUIViewManager m_instance;
    public static NewArenaUIViewManager Instance { get { return NewArenaUIViewManager.m_instance; } }

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量    

    private UILabel m_lblArenaUIGoldNum;
    private UILabel m_lblArenaUIDiamondNum;
    private UISprite m_spArenaUITitleProgressBar;
    private UILabel m_lblArenaUITitleProgressBarNum;
    private UISprite m_spArenaUIPlayerMedalIcon;
    private UILabel m_lblArenaUIPlayerMedalTitle;

    private UILabel m_lblArenaUIChallengeTimesNum;
    private GameObject m_goArenaUIChallengeCD;
    public UILabel m_lblArenaUIChallengeCDNum;

    private UILabel m_lblArenaUIPlyNameText;
    private UILabel m_lblArenaUIPlyForceNum;
    private UILabel m_lblArenaUIPlyDayScoreNum;
    private UILabel m_lblArenaUIPlyWeekScoreNum;

    private UILabel m_lblArenaUIEnyForceNum;
    private UILabel m_lblArenaUIEnyLevelNum;

    private UISprite m_spArenaUIEnemyRevengeIcon;
    private UILabel m_lblArenaUIEnemyRevengeName;
    private UISprite m_spArenaUIEnemyStrongIcon;
    private UILabel m_lblArenaUIEnemyStrongName;
    private UISprite m_spArenaUIEnemyWeakIcon;
    private UILabel m_lblArenaUIEnemyWeakName;
    private GameObject m_goArenaUIEnemyArrow;
    private GameObject m_goArenaUIEnemyRevengeArrowPos;
    private GameObject m_goArenaUIEnemyStrongArrowPos;
    private GameObject m_goArenaUIEnemyWeakArrowPos;

    private GameObject m_goArenaUIBtnRefresh;
    private GameObject m_goArenaUIBtnRewardNotice;

    private Camera m_ArenaUIPlyModelCamera;
   

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_lblArenaUIGoldNum = FindTransform("ArenaUIGoldNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIDiamondNum = FindTransform("ArenaUIDiamondNum").GetComponentsInChildren<UILabel>(true)[0];
        m_spArenaUITitleProgressBar = FindTransform("ArenaUITitleProgressBar").GetComponentsInChildren<UISprite>(true)[0];
        m_lblArenaUITitleProgressBarNum = FindTransform("ArenaUITitleProgressBarNum").GetComponentsInChildren<UILabel>(true)[0];
        m_spArenaUIPlayerMedalIcon = FindTransform("ArenaUIPlayerMedalIcon").GetComponentsInChildren<UISprite>(true)[0];
        m_lblArenaUIPlayerMedalTitle = FindTransform("ArenaUIPlayerMedalTitle").GetComponentsInChildren<UILabel>(true)[0];

        m_lblArenaUIChallengeTimesNum = FindTransform("ArenaUIChallengeTimesNum").GetComponentsInChildren<UILabel>(true)[0];
        m_goArenaUIChallengeCD = FindTransform("ArenaUIChallengeCD").gameObject;
        m_lblArenaUIChallengeCDNum = FindTransform("ArenaUIChallengeCDNum").GetComponentsInChildren<UILabel>(true)[0];

        m_lblArenaUIPlyNameText = FindTransform("ArenaUIPlyNameText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIPlyForceNum = FindTransform("ArenaUIPlyForceNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIPlyDayScoreNum = FindTransform("ArenaUIPlyDayScoreNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIPlyWeekScoreNum = FindTransform("ArenaUIPlyWeekScoreNum").GetComponentsInChildren<UILabel>(true)[0];

        m_lblArenaUIEnyForceNum = FindTransform("ArenaUIEnyForceNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIEnyLevelNum = FindTransform("ArenaUIEnyLevelNum").GetComponentsInChildren<UILabel>(true)[0];

        m_spArenaUIEnemyRevengeIcon = FindTransform("ArenaUIEnemyRevengeIcon").GetComponentsInChildren<UISprite>(true)[0];
        m_lblArenaUIEnemyRevengeName = FindTransform("ArenaUIEnemyRevengeName").GetComponentsInChildren<UILabel>(true)[0];
        m_spArenaUIEnemyStrongIcon = FindTransform("ArenaUIEnemyStrongIcon").GetComponentsInChildren<UISprite>(true)[0];
        m_lblArenaUIEnemyStrongName = FindTransform("ArenaUIEnemyStrongName").GetComponentsInChildren<UILabel>(true)[0];
        m_spArenaUIEnemyWeakIcon = FindTransform("ArenaUIEnemyWeakIcon").GetComponentsInChildren<UISprite>(true)[0];
        m_lblArenaUIEnemyWeakName = FindTransform("ArenaUIEnemyWeakName").GetComponentsInChildren<UILabel>(true)[0];
        m_goArenaUIEnemyArrow = FindTransform("ArenaUIEnemyArrow").gameObject;
        m_goArenaUIEnemyRevengeArrowPos = FindTransform("ArenaUIEnemyRevengeArrowPos").gameObject;
        m_goArenaUIEnemyStrongArrowPos = FindTransform("ArenaUIEnemyStrongArrowPos").gameObject;
        m_goArenaUIEnemyWeakArrowPos = FindTransform("ArenaUIEnemyWeakArrowPos").gameObject;

        m_goArenaUIBtnRefresh = FindTransform("ArenaUIBtnRefresh").gameObject;
        m_goArenaUIBtnRewardNotice = FindTransform("ArenaUIBtnRewardNotice").gameObject;

        // 玩家自己模型
        m_ArenaUIPlyModelCamera = FindTransform("ArenaUIPlyModelCamera").GetComponentsInChildren<Camera>(true)[0];
        m_ArenaUIPlyModelCamera.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = 
            GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        LoadPlayerModelByVocation();

        // 敌方模型
        m_goArenaUIEnyModelShowCase = FindTransform("ArenaUIEnyModelShowCase").gameObject;
        m_modelShowCase = m_goArenaUIEnyModelShowCase.AddComponent<ModelShowCase>();
        m_modelShowCase.left = FindTransform("ArenaUIEnyModelViewTL");
        m_modelShowCase.right = FindTransform("ArenaUIEnyModelViewBR");

		
		Initialize();
    }

    #region 事件

    public Action ARENAUICLOSEUP;
    public Action ARENAUIREWARDUP;
    public Action ARENAUIADDTIMESUP;
    public Action ARENAUICLEARCDUP;
    public Action ARENAUIENTERUP;
    public Action ARENAUIREFRESHUP;
    public Action ARENAUICHOOSEREVENGE;
    public Action ARENAUICHOOSESTRONG;
    public Action ARENAUICHOOSEWEAK;

    public void Initialize()
    {
        FindTransform("ArenaUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnCloseUp;
        FindTransform("ArenaUIBtnReward").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnRewardUp;
        FindTransform("ArenaUIBtnAddTimes").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnAddTimesUp;
        FindTransform("ArenaUIBtnClearCD").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnClearCDUp;
        FindTransform("ArenaUIBtnEnter").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnEnterUp;
        FindTransform("ArenaUIBtnRefresh").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnBtnRefreshUp;

        FindTransform("ArenaUIEnemyRevenge").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnEnemyRevengeUp;
        FindTransform("ArenaUIEnemyStrong").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnEnemyStrongUp;
        FindTransform("ArenaUIEnemyWeak").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnEnemyWeakUp;

        NewArenaUILogicManager.Instance.Initialize();
        m_uiLoginManager = NewArenaUILogicManager.Instance;
    }

    public void Release()
    {
        NewArenaUILogicManager.Instance.Release();
    }

    /// <summary>
    /// 关闭Btn
    /// </summary>
    void OnBtnCloseUp()
    {
        if (ARENAUICLOSEUP != null)
            ARENAUICLOSEUP();
    }

    /// <summary>
    /// 积分奖励Btn
    /// </summary>
    void OnBtnRewardUp()
    {
        if (ARENAUIREWARDUP != null)
            ARENAUIREWARDUP();
    }

    /// <summary>
    /// 增加挑战次数Btn
    /// </summary>
    void OnBtnAddTimesUp()
    {
        if (ARENAUIADDTIMESUP != null)
            ARENAUIADDTIMESUP();
    }

    /// <summary>
    /// 清除CDBtn
    /// </summary>
    void OnBtnClearCDUp()
    {
        if (ARENAUICLEARCDUP != null)
            ARENAUICLEARCDUP();
    }

    /// <summary>
    /// 挑战Btn
    /// </summary>
    void OnBtnEnterUp()
    {
        if (ARENAUIENTERUP != null)
            ARENAUIENTERUP();
    }

    /// <summary>
    /// 刷新
    /// </summary>
    void OnBtnRefreshUp()
    {
        if (ARENAUIREFRESHUP != null)
            ARENAUIREFRESHUP();
    }

    void OnEnemyRevengeUp()
    {
        RefreshEnemyArrowPos(ArenaUIEnemyTab.Revenge);
    }

    void OnEnemyStrongUp()
    {
        RefreshEnemyArrowPos(ArenaUIEnemyTab.Strong);
    }

    void OnEnemyWeakUp()
    {
        RefreshEnemyArrowPos(ArenaUIEnemyTab.Weak);
    }

    #endregion  

    #region 界面信息

    /// <summary>
    /// 设置钻石数量
    /// </summary>
    /// <param name="num"></param>
    public void SetDiamondNum(uint num)
    {
        m_lblArenaUIDiamondNum.text = num.ToString();
    }

    /// <summary>
    /// 设置金币数量
    /// </summary>
    /// <param name="num"></param>
    public void SetGoldNum(uint num)
    {
        m_lblArenaUIGoldNum.text = num.ToString();
    }

    /// <summary>
    /// 设置玩家勋章
    /// </summary>
    /// <param name="spriteName"></param>
    public void SetPlayerMedalIcon(string spriteName)
    {
        m_spArenaUIPlayerMedalIcon.spriteName = spriteName;
    }

    /// <summary>
    /// 设置玩家勋章名称
    /// </summary>
    /// <param name="title"></param>
    public void SetPlayerMedalTitle(ushort grade)
    {
        m_lblArenaUIPlayerMedalTitle.text = ArenaLevelData.GetCurTitle(grade);
    }

    /// <summary>
    /// 设置玩家勋章进度
    /// </summary>
    /// <param name="curCredit"></param>
    public void SetPlayerMedalProgressValue(uint curCredit)
    {
        int nextCredit = ArenaLevelData.GetNextLevelCreditNeed(MogoWorld.thePlayer.arenicGrade, (int)curCredit);
        float progress = ((float)curCredit) / nextCredit;
        progress = Math.Min(1, progress);
        progress = Math.Max(0, progress);
        m_spArenaUITitleProgressBar.fillAmount = progress;
        m_lblArenaUITitleProgressBarNum.text = String.Concat(curCredit, '/', nextCredit);
    }

    /// <summary>
    /// 是否显示领取奖励提示
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowRewardNotice(bool isShow)
    {
        m_goArenaUIBtnRewardNotice.SetActive(isShow);
    }    

    /// <summary>
    /// 是否显示立即刷新按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowArenaUIBtnRefresh(bool isShow)
    {
        m_goArenaUIBtnRefresh.SetActive(isShow);
    }

    /// <summary>
    /// 刷新敌人箭头位置
    /// </summary>
    /// <param name="enemy"></param>
    public void RefreshEnemyArrowPos(ArenaUIEnemyTab enemy)
    {
        switch (enemy)
        {
            case ArenaUIEnemyTab.Revenge:
                m_goArenaUIEnemyArrow.transform.localPosition = m_goArenaUIEnemyRevengeArrowPos.transform.localPosition;
                break;
            case ArenaUIEnemyTab.Strong:
                m_goArenaUIEnemyArrow.transform.localPosition = m_goArenaUIEnemyStrongArrowPos.transform.localPosition;
                break;
            case ArenaUIEnemyTab.Weak:
                m_goArenaUIEnemyArrow.transform.localPosition = m_goArenaUIEnemyWeakArrowPos.transform.localPosition;
                break;
        }
    }

    #endregion

    #region 玩家自己

    /// <summary>
    /// 设置玩家自己的数据
    /// </summary>
    /// <param name="data"></param>
    public void SetArenaPersonalData(ArenaPersonalData data)
    {
        SetArenaUIPlyDayScoreNum(data.dayScore.ToString());
        SetArenaUIPlyWeekScoreNum(data.weekScore.ToString());
        SetArenaUIChallengeTimesNum(data.challengeTimes.ToString());
        SetArenaCDText((uint)data.cd);
    }

    /// <summary>
    /// 设置玩家姓名
    /// </summary>
    /// <param name="name"></param>
    public void SetPlyNameText(string name)
    {
        m_lblArenaUIPlyNameText.text = name;
    }

    /// <summary>
    /// 设置玩家战斗力
    /// </summary>
    /// <param name="num"></param>
    private void SetlArenaUIPlyForceNum(string num)
    {
        m_lblArenaUIPlyForceNum.text = num;
    }

    /// <summary>
    /// 设置玩家日积分
    /// </summary>
    /// <param name="num"></param>
    private void SetArenaUIPlyDayScoreNum(string num)
    {
        m_lblArenaUIPlyDayScoreNum.text = num;
    }

    /// <summary>
    /// 设置玩家周积分
    /// </summary>
    /// <param name="num"></param>
    private void SetArenaUIPlyWeekScoreNum(string num)
    {
        m_lblArenaUIPlyWeekScoreNum.text = num;
    }

    /// <summary>
    /// 设置挑战次数
    /// </summary>
    /// <param name="num"></param>
    private void SetArenaUIChallengeTimesNum(string num)
    {
        m_lblArenaUIChallengeTimesNum.text = num;
    }

    /// <summary>
    /// 设置CD时间
    /// </summary>
    /// <param name="cdTime"></param>
    public void SetArenaCDText(uint cdTime)
    {
        if (cdTime > 0)
        {
            m_goArenaUIChallengeCD.SetActive(true);
        }
        else
        {
            m_goArenaUIChallengeCD.SetActive(false);
        }

        TimerManager.GetTimer(m_goArenaUIChallengeCD).StartTimer(cdTime,
            (sec) =>
            {
                uint min = (sec % 3600) / 60;
                uint second = (sec % 3600) % 60;
                m_lblArenaUIChallengeCDNum.text = String.Concat(min.ToString("d2"), ":", second.ToString("d2"));
            },
            () =>
            {
                m_goArenaUIChallengeCD.SetActive(false);
            });
    }

    #region 自己模型

    public void LoadPlayerModel(float fOffect = 0, float xOffset = 0, float yOffset = 0, float zOffset = 0, float fLookatOffsetY = 0f,
       float fLookatOffsetX = 0f)
    {
        if (m_ArenaUIPlyModelCamera == null)
            return;

        GameObject obj = MogoWorld.thePlayer.GameObject;
        Transform cam = m_ArenaUIPlyModelCamera.transform;

        Vector3 pos = obj.transform.Find("slot_camera").position;
        cam.position = pos + obj.transform.forward * fOffect;
        cam.position += obj.transform.right * xOffset;
        cam.position += obj.transform.up * yOffset;
        cam.LookAt(pos + new Vector3(fLookatOffsetX, fLookatOffsetY, 0));

        SetObjectLayer(10, obj);
    }

    public void SetObjectLayer(int layer, GameObject obj)
    {
        if (!obj)
            return;

        obj.layer = layer;

        foreach (Transform item in obj.transform)
        {
            SetObjectLayer(layer, item.gameObject);
        }
    }

    private void LoadPlayerModelByVocation()
    {
        if (MogoWorld.thePlayer.vocation == Mogo.Game.Vocation.Assassin)
        {
            LoadPlayerModel(2.43f, 0.49f, 0.42f, 0, 0.08f, 0f);
        }
        else if (MogoWorld.thePlayer.vocation == Mogo.Game.Vocation.Archer)
        {
            LoadPlayerModel(2.75f, 0, 0.1f, 0);
        }
        else if (MogoWorld.thePlayer.vocation == Mogo.Game.Vocation.Mage)
        {
            LoadPlayerModel(2.6f, 0, 0.15f, 0);
        }
        else
        {
            LoadPlayerModel(2.82f, 0.53f, 0.59f, 0, 0.02f, 0.02f);
        }
    }

    public void DisablePlayerModel()
    {
        SetObjectLayer(8, MogoWorld.thePlayer.GameObject);
    }

    #endregion

    #endregion

    #region 敌方玩家

    /// <summary>
    /// 设置玩家自己的数据
    /// </summary>
    /// <param name="data"></param>
    public void SetArenaEnemyData(ArenaPlayerData data, ArenaUIEnemyTab enemyTab)
    {
        SetArenaUIEnyForceNum(data.fightForce.ToString());
        SetArenaUIEnyLevelNum(data.level.ToString());
    }

    /// <summary>
    /// 设置战斗力
    /// </summary>
    /// <param name="num"></param>
    private void SetArenaUIEnyForceNum(string num)
    {
        m_lblArenaUIEnyForceNum.text = num;
    }

    /// <summary>
    /// 设置等级
    /// </summary>
    /// <param name="num"></param>
    private void SetArenaUIEnyLevelNum(string num)
    {
        m_lblArenaUIEnyLevelNum.text = num;
    }

    #region 敌方模型

    private GameObject m_goArenaUIEnyModelShowCase;
    private ModelShowCase m_modelShowCase;

    public void SetModelShow(int vocation, List<int> weaponList, bool isShow)
    {
        INSTANCE_COUNT++;
        MogoGlobleUIManager.Instance.ShowWaitingTip(true);
        m_modelShowCase.LoadCreateCharacter(vocation, weaponList,
            () =>
            {
                Transform tranModel = FindTransform("ArenaUIEnyModelShowCaseCamera");
                tranModel.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera =
                    GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
                m_modelShowCase.SetCamera(tranModel);
                ModelShowCaseLogicManager.Instance.ShowModel(vocation, isShow);
                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
    }

    public void CloseAllModel()
    {
        ModelShowCaseLogicManager.Instance.CloseAllModel();
    }

    #endregion

    #endregion

    #region 界面打开和关闭

    /// <summary>
    /// 展示界面信息
    /// </summary>
    /// <param name="IsShow"></param>
    protected override void OnEnable()
    {
        base.OnEnable();
        MogoGlobleUIManager.Instance.ShowWaitingTip(false);

        LoadPlayerModelByVocation();
        SetModelShow(2, new List<int>(), true);
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        DisablePlayerModel();
        CloseAllModel();
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroySpriteUI();
        }
    }

    #endregion   
}
