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
using System.Collections.Generic;

public class OccupyTowerUIViewManager : MogoUIBehaviour
{
    private static OccupyTowerUIViewManager m_instance;
    public static OccupyTowerUIViewManager Instance { get { return OccupyTowerUIViewManager.m_instance; } }

    private GameObject m_goOccupyTowerUIBtnJoin;
    private UISprite m_spOccupyTowerUIBtnJoinBGUp;
    private UILabel m_lblOccupyTowerUIQueueNum;
    private UILabel m_lblOccupyTowerUIScoreTitle;
    private UILabel m_lblOccupyTowerUIScoreNum;

    // 匹配UI
    private GameObject m_goMatchUI;
    private UILabel m_lblMatchUICountDown;
    private UILabel m_lblMatchUIText;
    private GameObject m_goMatchUIOKBtn;
    private GameObject m_goMatchUICancelBtn;
    private UILabel m_lblMatchUICancelBtnText;
    private UILabel m_lblMatchUIOKBtnText;


    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goOccupyTowerUIBtnJoin = FindTransform("OccupyTowerUIBtnJoin").gameObject;
        m_spOccupyTowerUIBtnJoinBGUp = FindTransform("OccupyTowerUIBtnJoinBGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_lblOccupyTowerUIQueueNum = FindTransform("OccupyTowerUIQueueNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOccupyTowerUIScoreTitle = FindTransform("OccupyTowerUIScoreTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOccupyTowerUIScoreNum = FindTransform("OccupyTowerUIScoreNum").GetComponentsInChildren<UILabel>(true)[0];

        // 匹配UI
        m_goMatchUI = FindTransform("OccupyTowerUIMatchUI").gameObject;
        m_lblMatchUICountDown = FindTransform("OccupyTowerUIMatchUICountDown").GetComponentsInChildren<UILabel>(true)[0];
        m_lblMatchUIText = FindTransform("OccupyTowerUIMatchUIText").GetComponentsInChildren<UILabel>(true)[0];
        m_goMatchUIOKBtn = FindTransform("OccupyTowerUIMatchUIOKBtn").gameObject;
        m_goMatchUICancelBtn = FindTransform("OccupyTowerUIMatchUICancelBtn").gameObject;
        m_lblMatchUICancelBtnText = FindTransform("OccupyTowerUIMatchUICancelBtnText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblMatchUIOKBtnText = FindTransform("OccupyTowerUIMatchUIOKBtnText").GetComponentsInChildren<UILabel>(true)[0];

        // I18N
        FindTransform("OccupyTowerUITitleName").GetComponentsInChildren<UILabel>(true)[0].text 
            = LanguageData.GetContent(24034);
        FindTransform("OccupyTowerUIRuleTitle").GetComponentsInChildren<UILabel>(true)[0].text 
            = string.Concat(LanguageData.GetContent(24034), LanguageData.GetContent(48912));
        m_lblMatchUIText.text = LanguageData.GetContent(48908);
        m_lblOccupyTowerUIScoreTitle.text = LanguageData.GetContent(48905);

        SetOccupyTowerUIRule();
        SetIsMatchNow(false);
        Initialize();
    }

    /// <summary>
    /// 设置规则
    /// </summary>
    private List<GameObject> listRule = new List<GameObject>();
    readonly static int RULE_ID_START = 50101;
    readonly static int RULE_NUM = 7;
    private void SetOccupyTowerUIRule()
    {
        listRule.Clear();
        for (int i = 1; i <= RULE_NUM; i++)
        {
            GameObject goRule = FindTransform(string.Format("OccupyTowerUIRule{0}", i)).gameObject;
            goRule.SetActive(false);
            listRule.Add(goRule);
        }

        for (int i = 0; i < RULE_NUM; i++)
        {
            string rule = LanguageData.GetContent(RULE_ID_START + i);
            if (string.IsNullOrEmpty(rule) || rule.Equals("***"))
            {
               listRule[i].SetActive(false);
            }
            else
            {
				listRule[i].transform.Find(string.Format("OccupyTowerUIRule{0}Text", i + 1)).GetComponentsInChildren<UILabel>(true)[0].text = rule;
				listRule[i].SetActive(true);                
            }
        }        
    }

    #region 事件

    public Action OCCUPYTOWERUICLOSEUP;
    public Action OCCUPYTOWERUIJOINUP;

    public void Initialize()
    {
        FindTransform("OccupyTowerUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnCloseUp;
        FindTransform("OccupyTowerUIBtnJoin").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnJoinUp;

        FindTransform("OccupyTowerUIMatchUIOKBtn").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnMatchUIOkUp;
        FindTransform("OccupyTowerUIMatchUICancelBtn").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnMatchUICancelUp;

        OccupyTowerUILogicManager.Instance.Initialize();
        m_uiLoginManager = OccupyTowerUILogicManager.Instance;
    }

    public void Release()
    {
        FindTransform("OccupyTowerUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnCloseUp;
        FindTransform("OccupyTowerUIBtnJoin").GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnJoinUp;

        FindTransform("OccupyTowerUIMatchUIOKBtn").GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnMatchUIOkUp;
        FindTransform("OccupyTowerUIMatchUICancelBtn").GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= OnMatchUICancelUp;

        OccupyTowerUILogicManager.Instance.Release();
    }

    private void OnCloseUp()
    {
        if (OCCUPYTOWERUICLOSEUP != null)
            OCCUPYTOWERUICLOSEUP();
    }

    private void OnJoinUp()
    {
        if (OCCUPYTOWERUIJOINUP != null)
            OCCUPYTOWERUIJOINUP();
    }

    private void OnMatchUIOkUp()
    {
        ShowMatchUICountDown(true);
        SetBeginCountDown(true, 60);
    }

    private void OnMatchUICancelUp()
    {
        SetIsMatchNow(false);
        ShowMatchUI(false);
        EventDispatcher.TriggerEvent(Events.OccupyTowerEvent.LeaveOccupyTower);
    }

    #endregion

    #region 界面信息   

    /// <summary>
    /// 是否正在匹配中
    /// </summary>
    /// <param name="isMatchNow"></param>
    public void SetIsMatchNow(bool isMatchNow)
    {
        // Debug.LogError("SetIsMatchNow: " + isMatchNow);
        if (isMatchNow)
        {
            m_lblMatchUIText.text = LanguageData.GetContent(48908);
            ShowMatchUI(true);
            ShowMatchUICountDown(true);
            SetBeginCountDown(true, 60);
        }
        else
        {
            m_lblMatchUIText.text = LanguageData.GetContent(48908);

            ShowMatchUICountDown(false);

            BeginCountDown = false;
            m_fCurrentTime = 0f;
            m_fElapseTime = 0f;
        }
    }

    /// <summary>
    /// 是否显示加入按钮
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowOccupyTowerUIBtnJoin(bool isShow)
    {
        m_goOccupyTowerUIBtnJoin.SetActive(isShow);
    }

    /// <summary>
    /// 是否开启加入按钮
    /// </summary>
    /// <param name="isLock"></param>
    private void LockOccupyTowerUIBtnJoin(bool isLock)
    {
        if (!isLock)
        {
            m_goOccupyTowerUIBtnJoin.GetComponentsInChildren<BoxCollider>(true)[0].enabled = true;
            m_spOccupyTowerUIBtnJoinBGUp.spriteName = "btn_03up";
        }
        else
        {
            m_goOccupyTowerUIBtnJoin.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;
            m_spOccupyTowerUIBtnJoinBGUp.spriteName = "btn_03grey";
        }
    }

    /// <summary>
    /// 是否显示匹配中
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowOccupyTowerUIQueueING(bool isShow)
    {
        m_lblMatchUIText.gameObject.SetActive(isShow);        
    }

    /// <summary>
    /// 是否显示排队人数
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowOccupyTowerUIQueueNum(bool isShow)
    {
        m_lblOccupyTowerUIQueueNum.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// 设置已获积分
    /// </summary>
    /// <param name="scoreNum"></param>
    public void SetOccupyTowerUIScoreNum(int scoreNum)
    {
        m_lblOccupyTowerUIScoreNum.text = scoreNum.ToString();
    }

    /// <summary>
    /// 设置排队数量
    /// </summary>
    /// <param name="queueNum"></param>
    public void SetOccupyTowerUIQueueNum(int queueNum)
    {
        m_lblOccupyTowerUIQueueNum.text = string.Format(LanguageData.GetContent(48906), queueNum);
    }   

    #endregion

    #region 匹配UI

    /// <summary>
    /// 是否显示匹配UI
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowMatchUI(bool isShow)
    {
        m_goMatchUI.SetActive(isShow);
    }

    /// <summary>
    /// 是否显示匹配UI按钮
    /// true:显示左边按钮，显示右边按钮
    /// false:只显示左边按钮
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowMatchUIButtons(bool isShow)
    {
        m_lblMatchUIOKBtnText.text = LanguageData.GetContent(48910);
        if (isShow)
        {
            m_goMatchUICancelBtn.transform.localPosition = new Vector3(-150,
                m_goMatchUICancelBtn.transform.localPosition.y,
                m_goMatchUICancelBtn.transform.localPosition.z);
            m_lblMatchUICancelBtnText.text = LanguageData.GetContent(48909);

            m_goMatchUIOKBtn.SetActive(true);
            m_goMatchUICancelBtn.SetActive(true);
        }
        else
        {
            m_goMatchUICancelBtn.transform.localPosition = new Vector3(0,
              m_goMatchUICancelBtn.transform.localPosition.y,
              m_goMatchUICancelBtn.transform.localPosition.z);
            m_lblMatchUICancelBtnText.text = LanguageData.GetContent(48911);

            m_goMatchUIOKBtn.SetActive(false);
            m_goMatchUICancelBtn.SetActive(true);
        }
    }

    #region 倒计时

    /// <summary>
    /// 是否显示CD
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowMatchUICountDown(bool isShow)
    {
        m_lblMatchUICountDown.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// 是否倒计时
    /// </summary>
    /// <param name="isCountDown"></param>
    /// <param name="cd"></param>
    public void SetBeginCountDown(bool isCountDown, float cdTime = 0)
    {
        SetCountDownTime((int)cdTime);
        m_fCDTime = cdTime;
        BeginCountDown = isCountDown;        
    }

    /// <summary>
    /// 开始倒数时间
    /// </summary>
    private bool m_bBeginCountDown = false;
    private bool BeginCountDown
    {
        get
        {
            return m_bBeginCountDown;
        }
        set
        {
            m_bBeginCountDown = value;
            ShowMatchUIButtons(!m_bBeginCountDown);
        }
    }

    private float m_fCDTime = 10.0f;
    private float m_fCurrentTime = 0f;
    private float m_fElapseTime = 0f;
    void Update()
    {       
        if (BeginCountDown)
        {
            m_fCurrentTime += Time.deltaTime;
            m_fElapseTime += Time.deltaTime;

            if (m_fCurrentTime >= m_fCDTime)
            {
                LoggerHelper.Debug("CountDown End");
                //BeginCountDown = false;
                //m_fCurrentTime = 0f;
                //m_fElapseTime = 0f;

                EventDispatcher.TriggerEvent(OccupyTowerUIDict.OccupyTowerUEvent.OnJoinCountDownEnd);
            }
            else
            {
                if (m_fElapseTime >= 1.0f)
                {
                    LoggerHelper.Debug("CountDown ING");
                    int downCount = (int)m_fCDTime - (int)m_fCurrentTime;
                    SetCountDownTime(downCount);
                    m_fElapseTime = 0f;
                }
            }
        }
    }

    /// <summary>
    /// 设置自动退出副本时间
    /// </summary>
    /// <param name="downCount"></param>
    private void SetCountDownTime(int downCount)
    {
        m_lblMatchUICountDown.text = downCount.ToString();
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
        SetIsMatchNow(false);
        ShowMatchUI(false);
        ShowOccupyTowerUIQueueNum(false);
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroyOccupyTowerUI();
        }
    }

    #endregion     
}
