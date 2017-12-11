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
using System.Collections.Generic;
using System;
using Mogo.GameData;
using System.Text;

public class OccupyTowerPassUIViewManager : MogoUIBehaviour
{
    private static OccupyTowerPassUIViewManager m_instance;
    public static OccupyTowerPassUIViewManager Instance { get { return OccupyTowerPassUIViewManager.m_instance; } }

    private GameObject m_goOccupyTowerPassUIPlayer;
    private List<GameObject> m_listPlayerInfoPos = new List<GameObject>();
    private readonly static int MAX_PLAYER_COUNT = 6;

    private UISprite m_spOccupyTowerPassUITitle;
    private UILabel m_lblOccupyTowerPassUICountDown;

    private GameObject m_goOccupyTowerPassUIResult;
    private UILabel m_lblOccupyTowerPassUIResultText;
    private GameObject m_goOccupyTowerPassUIReward;
    private UILabel m_lblOccupyTowerPassUIRewardText;

    private UILabel m_lblOccupyTowerPassUIPlayerTitleRank;
    private UILabel m_lblOccupyTowerPassUIPlayerTitleAddition;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goOccupyTowerPassUIPlayer = FindTransform("OccupyTowerPassUIPlayer").gameObject;
        m_goOccupyTowerPassUIPlayer.SetActive(false);
        m_listPlayerInfoPos.Clear();
        for (int i = 1; i <= MAX_PLAYER_COUNT; i++)
        {
            GameObject goPlayerPos = FindTransform(string.Format("OccupyTowerPassUIPlayerPos{0}", i)).gameObject;
            m_listPlayerInfoPos.Add(goPlayerPos);
        }

        AddPlayerInfoList(MAX_PLAYER_COUNT, () =>
            {
                SetPlayerInfoListData(m_listPlayerInfoData);
            });

        m_spOccupyTowerPassUITitle = FindTransform("OccupyTowerPassUITitle").GetComponentsInChildren<UISprite>(true)[0];
        m_lblOccupyTowerPassUICountDown = FindTransform("OccupyTowerPassUICountDown").GetComponentsInChildren<UILabel>(true)[0];
        m_goOccupyTowerPassUIResult = FindTransform("OccupyTowerPassUIResult").gameObject;
        m_lblOccupyTowerPassUIResultText = FindTransform("OccupyTowerPassUIResultText").GetComponentsInChildren<UILabel>(true)[0];
        m_goOccupyTowerPassUIReward = FindTransform("OccupyTowerPassUIReward").gameObject;
        m_lblOccupyTowerPassUIRewardText = FindTransform("OccupyTowerPassUIRewardText").GetComponentsInChildren<UILabel>(true)[0];

        m_lblOccupyTowerPassUIPlayerTitleRank = FindTransform("OccupyTowerPassUIPlayerTitleRank").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOccupyTowerPassUIPlayerTitleAddition = FindTransform("OccupyTowerPassUIPlayerTitleAddition").GetComponentsInChildren<UILabel>(true)[0];

        Initialize();
    }

    #region 事件

    public void Initialize()
    {
        ResetAnimation();
        OccupyTowerPassUILogicManager.Instance.Initialize();
        m_uiLoginManager = OccupyTowerPassUILogicManager.Instance;
    }

    public void Release()
    {
        OccupyTowerPassUILogicManager.Instance.Release();
    }

    /// <summary>
    /// 播放标题动画完毕
    /// </summary>
    private void OnPlayTitleAnimEnd()
    {
        TimerHeap.AddTimer(200, 0, () =>
        {
            PlayOccupyTowerPassUIResultAnim();
        });
    }

    /// <summary>
    /// 结果信息
    /// </summary>
    private void OnOccupyTowerPassUIResultAnimEnd()
    {
        TimerHeap.AddTimer(300, 0, () =>
        {
            PlayOccupyTowerPassUIRewardAnim();
        });
    }

    /// <summary>
    /// 奖励信息
    /// </summary>
    private void OnOccupyTowerPassUIRewardAnimEnd()
    {
        TimerHeap.AddTimer(300, 0, () =>
            {
                m_goOccupyTowerPassUIPlayer.SetActive(true);
                BeginCountDown = true;
            });
    }
    
    #endregion

    #region 界面信息

    /// <summary>
    /// 设置结果信息
    /// </summary>
    /// <param name="text"></param>
    public void SetOccupyTowerPassUIResultText(string text)
    {
        m_lblOccupyTowerPassUIResultText.text = text;
    }

    /// <summary>
    /// 设置奖励信息
    /// </summary>
    /// <param name="text"></param>
    private void SetOccupyTowerPassUIRewardText(string text)
    {
        m_lblOccupyTowerPassUIRewardText.text = text;
    }

    /// <summary>
    /// 设置排行Title
    /// </summary>
    /// <param name="text"></param>
    private void SetOccupyTowerPassUIPlayerTitleRank(string text)
    {
        m_lblOccupyTowerPassUIPlayerTitleRank.text = text;
    }

    /// <summary>
    /// 设置加成Title
    /// </summary>
    /// <param name="text"></param>
    private void SetOccupyTowerPassUIPlayerTitleAddition(string text)
    {
        m_lblOccupyTowerPassUIPlayerTitleAddition.text = text;
    }

    #endregion

    #region 动画播放

    private readonly static float TITLE_DURATION = 0.15f;
    private readonly static float RESULT_DURATION = 0.15f;
    private readonly static float REWARD_DURATION = 0.15f;

    /// <summary>
    /// 播放标题动画
    /// </summary>
    public void PlayTitleAnim()
    {
        TweenScale ts = m_spOccupyTowerPassUITitle.GetComponentsInChildren<TweenScale>(true)[0];
        ts.Reset();
        ts.duration = TITLE_DURATION;
        ts.from = new Vector3(4600, 1080, 1);
        ts.to = new Vector3(460, 108, 1);
        ts.eventReceiver = gameObject;
        ts.callWhenFinished = "OnPlayTitleAnimEnd";
        ts.gameObject.SetActive(true);
        ts.enabled = true;
        ts.Play(true);
    }

    /// <summary>
    /// 播放结果信息动画
    /// </summary>
    private void PlayOccupyTowerPassUIResultAnim()
    {
        TweenPosition tp = m_goOccupyTowerPassUIResult.GetComponentsInChildren<TweenPosition>(true)[0];
        tp.Reset();
        tp.duration = RESULT_DURATION;
        tp.from = new Vector3(-5000, m_goOccupyTowerPassUIResult.transform.localPosition.y, 0);
        tp.to = new Vector3(0, m_goOccupyTowerPassUIResult.transform.localPosition.y, 0);
        tp.eventReceiver = gameObject;
        tp.callWhenFinished = "OnOccupyTowerPassUIResultAnimEnd";
        m_goOccupyTowerPassUIResult.SetActive(true);
        tp.enabled = true;
        tp.Play(true);        
    }

    /// <summary>
    /// 播放奖励信息动画
    /// </summary>
    private void PlayOccupyTowerPassUIRewardAnim()
    {
        TweenPosition tp = m_goOccupyTowerPassUIReward.GetComponentsInChildren<TweenPosition>(true)[0];
        tp.Reset();
        tp.duration = REWARD_DURATION;
        tp.from = new Vector3(-5000, m_goOccupyTowerPassUIReward.transform.localPosition.y, 0);
        tp.to = new Vector3(0, m_goOccupyTowerPassUIReward.transform.localPosition.y, 0);
        tp.eventReceiver = gameObject;
        tp.callWhenFinished = "OnOccupyTowerPassUIRewardAnimEnd";
        m_goOccupyTowerPassUIReward.SetActive(true);
        tp.enabled = true;
        tp.Play(true);        
    }

    private void ResetAnimation()
    {
        if (m_spOccupyTowerPassUITitle != null)
            m_spOccupyTowerPassUITitle.gameObject.SetActive(false);

        if (m_goOccupyTowerPassUIResult != null)
            m_goOccupyTowerPassUIResult.SetActive(false);

        if (m_goOccupyTowerPassUIReward != null)
            m_goOccupyTowerPassUIReward.SetActive(false);

        if(m_goOccupyTowerPassUIPlayer != null)
            m_goOccupyTowerPassUIPlayer.SetActive(false);
    }

    #endregion

    #region 倒计时

    /// <summary>
    /// 开始倒数离开副本时间
    /// </summary>
    private bool m_bBeginCountDown = false;
    public bool BeginCountDown
    {
        get
        {
            return m_bBeginCountDown;
        }
        set
        {
            m_bBeginCountDown = value;
        }
    }   


    const float LEAVEINSTANCETIME = 20.0f;
    private float m_fCurrentTime = 0f;
    private float m_fElapseTime = 0f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayTitleAnim();
        }

        if (BeginCountDown)
        {
            m_fCurrentTime += Time.deltaTime;
            m_fElapseTime += Time.deltaTime;

            if (m_fCurrentTime >= LEAVEINSTANCETIME)
            {
                LoggerHelper.Debug("m_fCurrentTime >= LEAVEINSTANCETIME + LEAVEINSTANCEWAITTIME");
                BeginCountDown = false;
                m_fCurrentTime = 0f;
                m_fElapseTime = 0f;
                // MogoUIManager.Instance.ShowMogoNormalMainUI();// test

                EventDispatcher.TriggerEvent(Events.OccupyTowerEvent.ExitOccupyTower);
            }
            else
            {
                if (m_fElapseTime >= 1.0f)
                {
                    LoggerHelper.Debug("isCountingTime");
                    int downCount = (int)LEAVEINSTANCETIME - (int)m_fCurrentTime;
                    if (downCount == 2)
                        EventDispatcher.TriggerEvent(Events.InstanceEvent.StopAutoFight);
                    SetLeaveTime(downCount);
                    m_fElapseTime = 0f;
                }
            }
        }
    }

    /// <summary>
    /// 设置自动退出副本时间
    /// </summary>
    /// <param name="downCount"></param>
    private void SetLeaveTime(int downCount)
    {
        m_lblOccupyTowerPassUICountDown.text = string.Format(LanguageData.GetContent(48904), downCount);
    }

    #endregion

    #region 参加玩家信息

    private Dictionary<int, OccupyTowerPassUIPlayer> m_maplistPlayerInfo = new Dictionary<int, OccupyTowerPassUIPlayer>();
    private List<OccupyTowerPassUIPlayerData> m_listPlayerInfoData = new List<OccupyTowerPassUIPlayerData>();

    private void AddPlayerInfoList(int num, Action act = null)
    {
        for (int i = 0; i < num; ++i)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("OccupyTowerPassUIPlayer.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_goOccupyTowerPassUIPlayer.transform;
                obj.transform.localPosition = m_listPlayerInfoPos[index].transform.localPosition;
                obj.transform.localScale = new Vector3(1f, 1f, 1f);

                OccupyTowerPassUIPlayer gridUI = obj.AddComponent<OccupyTowerPassUIPlayer>();

                if (m_maplistPlayerInfo.ContainsKey(index))
                    AssetCacheMgr.ReleaseInstance(m_maplistPlayerInfo[index].gameObject);
                m_maplistPlayerInfo[index] = gridUI;

                if (m_maplistPlayerInfo.Count == num)
                {
                    if (act != null) act();
                }
            });
        }
    }

    public void SetPlayerInfoListData(List<OccupyTowerPassUIPlayerData> listPlayerInfoData)
    {
        m_listPlayerInfoData = listPlayerInfoData;
        if (m_maplistPlayerInfo.Count != MAX_PLAYER_COUNT)
            return;

        for (int index = 0; index < MAX_PLAYER_COUNT; index++)
        {
            if(index < listPlayerInfoData.Count)
            {
                OccupyTowerPassUIPlayerData gridData = listPlayerInfoData[index];
                OccupyTowerPassUIPlayer gridUI = m_maplistPlayerInfo[index];
                gridUI.gameObject.SetActive(true);
                gridUI.LoadResourceInsteadOfAwake();
                gridUI.SetPlayerName(gridData.playerName);
                gridUI.SetPlayerScore(gridData.playerScore);
                gridUI.SetPlayerAddition(gridData.playerAddition);
                gridUI.SetPlayerCamp(gridData.camp);
            }
            else
            {
                m_maplistPlayerInfo[index].gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region 界面打开和关闭

    /// <summary>
    /// 展示界面信息
    /// </summary>
    /// <param name="IsShow"></param>
    protected override void OnEnable()
    {
        base.OnEnable();
        ResetAnimation();
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyOccupyTowerPassUI();
        }
    }

    #endregion     

    #region Logic

    public void SetOccupyTowerResult(bool isWin)
    {
        if (isWin)
        {
            // 修改图片
            // to do
            m_spOccupyTowerPassUITitle.spriteName = "zdsl";
            SetOccupyTowerPassUIResultText(LanguageData.GetContent(50121));
        }
        else
        {
            // 修改图片
            // to do
            m_spOccupyTowerPassUITitle.spriteName = "fb_zhandoushengli (2)";
            SetOccupyTowerPassUIResultText(LanguageData.GetContent(50122));
        }
    }

    public void SetOccupyTowerReward(Dictionary<int, int> items)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in items)
        {
            sb.Append(ItemParentData.GetNameWithNum(item.Key, item.Value));
            sb.Append(" ");
        }
        SetOccupyTowerPassUIRewardText(sb.ToString());
    }

    public void SetAllAnimationPlay()
    {
        BeginCountDown = true;
        PlayTitleAnim();
        TimerHeap.AddTimer(1000, 0, PlayOccupyTowerPassUIResultAnim);
        TimerHeap.AddTimer(1500, 0, PlayOccupyTowerPassUIRewardAnim);
    }

    #endregion
}
