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

public enum SpriteUIEnum
{
    SpriteSkillUI,
    SpriteDetailUI,
    SpriteLearnUI,
}

public class SpriteUIViewManager : MogoUIBehaviour
{
    private static SpriteUIViewManager m_instance;
    public static SpriteUIViewManager Instance { get { return SpriteUIViewManager.m_instance; } }

    private static int INSTANCE_COUNT = 0; // 异步加载资源数量    
    
    private Camera m_rootCamera;
    public Camera RootCamera
    {
        get { return m_rootCamera; }
        set
        {
            m_rootCamera = value;
            CalSpriteUISkillDetailSkillModelPos();
        }
    }

    private Camera m_spriteUIBillBoardCamera;

    // 精灵系统SkillUI
    private GameObject m_goSpriteUISkillUI;
    private UILabel m_lblSpriteUISkillUISkillName;

    // 精灵系统领悟UI
    private GameObject m_goSpriteUILearnSkillUI;
    private GameObject m_goGOSpriteUILearnSkillUILearn;
    readonly static int MAX_LEARN_SKILL_NUM = 5; // 领悟技能数量
    private List<Vector3> m_listSpriteLearnUISkillPos = new List<Vector3>();    
    private GameObject m_goSpriteUILearnSkillUIDetailSkill;
    private UISprite m_spDetailSkillIcon;
    private UILabel m_lblDetailSkillLevel;
    private UILabel m_lblDetailSkillName;
    private UILabel m_lblDetailCurrentSkillDesc;
    private GameObject m_goDetailNextSkill;
    private UILabel m_lblDetailNextSkillDesc;
    private GameObject m_goDetailNextSkillReq;
    private UILabel m_lblDetailNextSkillRequest;
    private UISprite m_spDetailNextSkillProgressFG;
    private UILabel m_lblLearnTimesNum;
    

    // 精灵系统觉醒UI
    private GameObject m_goSpriteUISkillDetailUI;
    private GameObject m_goSpriteUISkillDetailSkillPos;
    private Vector3 m_vecSpriteUISkillDetailSkillModelPos; // 通过相机转换得到的位置    
    private GameObject m_goSpriteUISkillDetailUIProgressLineList;
    private GameObject m_goSpriteUISkillDetailGridPieceList;

    private GameObject m_goSpriteUISkillDetailUIMaterial;
    private UILabel m_lblSpriteUISkillDetailUIMaterialNum;
    private GameObject m_goSpriteUISkillDetailUIMaterialNext;
    private UILabel m_lblSpriteUISkillDetailUIMaterialNextText;
    private UILabel m_lblSpriteUISkillDetailUIMaterialNextNum;   
    private GameObject m_goSpriteUISkillDetailUIBtnAwake;
    private UISprite m_spSpriteUISkillDetailUIBtnAwakeBGUp;

    // 学会新技能悬浮UI
    private GameObject m_goSpriteUISuccessUI;
    private UILabel m_lblSpriteUISuccessUIText;
    private UISprite m_spSpriteUISuccessUIIcon;


    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        RootCamera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        FindTransform("SpriteUITopR").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;
        FindTransform("SpriteUISkillUITopL").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;
        FindTransform("SpriteUISkillUITopR").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;
        FindTransform("SpriteUISkillUITop").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;
        FindTransform("SpriteUISkillDetailUIBottomR").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;
        FindTransform("SpriteUISkillDetailUIBottomL").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;
        FindTransform("SpriteUISkillDetailUIBottom").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;
        FindTransform("SpriteUISkillDetailUITopL").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;
        FindTransform("SpriteUISkillDetailUITop").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;
        FindTransform("SpriteUISkillDetailUICenter").GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = RootCamera;


        m_lblSpriteUISkillUISkillName = FindTransform("SpriteUISkillUISkillName").GetComponentsInChildren<UILabel>(true)[0];
        m_spriteUIBillBoardCamera = FindTransform("SpriteUIBillBoardCamera").GetComponentsInChildren<Camera>(true)[0];

        // 精灵系统SkillUI
        m_goSpriteUISkillUI = FindTransform("SpriteUISkillUI").gameObject;        

        //////////////////////////////////////////////////////////////////////////
        // 精灵系统领悟UI
        //////////////////////////////////////////////////////////////////////////
        m_goSpriteUILearnSkillUI = FindTransform("SpriteUILearnSkillUI").gameObject;
        m_goGOSpriteUILearnSkillUILearn = FindTransform("GOSpriteUILearnSkillUILearn").gameObject;
        m_listSpriteLearnUISkillPos.Clear();
        if (MAX_LEARN_SKILL_NUM == 6)
        {
            for (int iLearnSkill = 1; iLearnSkill <= MAX_LEARN_SKILL_NUM; iLearnSkill++)
            {
                GameObject goSkill = FindTransform(string.Format("SpriteUILearnSkill6{0}Pos", iLearnSkill)).gameObject;
                m_listSpriteLearnUISkillPos.Add(goSkill.transform.localPosition);
            }
        }
        else if (MAX_LEARN_SKILL_NUM == 5)
        {
            for (int iLearnSkill = 1; iLearnSkill <= MAX_LEARN_SKILL_NUM; iLearnSkill++)
            {
                GameObject goSkill = FindTransform(string.Format("SpriteUILearnSkill5{0}Pos", iLearnSkill)).gameObject;
                m_listSpriteLearnUISkillPos.Add(goSkill.transform.localPosition);
            }
        }
        
        LoadSpriteUILearnSkillList(() =>
            {
                AddToSingleButtonList();
                SetListSpriteUILearnSkill(m_listSpriteLearnSkillData);
            });
        m_goSpriteUILearnSkillUIDetailSkill = FindTransform("SpriteUILearnSkillUIDetailSkill").gameObject;
        m_spDetailSkillIcon = FindTransform("SpriteUILearnSkillUIDetailSkillIcon").GetComponentsInChildren<UISprite>(true)[0];
        m_lblDetailSkillLevel = FindTransform("SpriteUILearnSkillUIDetailSkillLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDetailSkillName = FindTransform("SpriteUILearnSkillUIDetailSkillName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblDetailCurrentSkillDesc = FindTransform("SpriteUILearnSkillUIDetailCurrentSkillDesc").GetComponentsInChildren<UILabel>(true)[0];
        m_goDetailNextSkill = FindTransform("SpriteUILearnSkillUIDetailNextSkill").gameObject;
        m_lblDetailNextSkillDesc = FindTransform("SpriteUILearnSkillUIDetailNextSkillDesc").GetComponentsInChildren<UILabel>(true)[0];
        m_goDetailNextSkillReq = FindTransform("SpriteUILearnSkillUIDetailNextSkillReq").gameObject;
        m_lblDetailNextSkillRequest = FindTransform("SpriteUILearnSkillUIDetailNextSkillRequest").GetComponentsInChildren<UILabel>(true)[0];
        m_spDetailNextSkillProgressFG = FindTransform("SpriteUILearnSkillUIDetailNextSkillProgressFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblLearnTimesNum = FindTransform("SpriteUILearnSkillUILearnTimesNum").GetComponentsInChildren<UILabel>(true)[0];

        //////////////////////////////////////////////////////////////////////////
        // 精灵系统觉醒UI
        //////////////////////////////////////////////////////////////////////////
        m_goSpriteUISkillDetailUI = FindTransform("SpriteUISkillDetailUI").gameObject;
        m_goSpriteUISkillDetailUI.SetActive(false);
        m_goSpriteUISkillDetailSkillPos = FindTransform("SpriteUISkillDetailSkillPos").gameObject;                
        m_goSpriteUISkillDetailUIProgressLineList = FindTransform("SpriteUISkillDetailUIProgressLineList").gameObject;
        m_goSpriteUISkillDetailGridPieceList = FindTransform("SpriteUISkillDetailGridPieceList").gameObject;

        m_goSpriteUISkillDetailUIMaterial = FindTransform("SpriteUISkillDetailUIMaterial").gameObject;
        m_lblSpriteUISkillDetailUIMaterialNum = FindTransform("SpriteUISkillDetailUIMaterialNum").GetComponentsInChildren<UILabel>(true)[0];
        m_goSpriteUISkillDetailUIMaterialNext = FindTransform("SpriteUISkillDetailUIMaterialNext").gameObject;
        m_lblSpriteUISkillDetailUIMaterialNextText = FindTransform("SpriteUISkillDetailUIMaterialNextText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblSpriteUISkillDetailUIMaterialNextNum = FindTransform("SpriteUISkillDetailUIMaterialNextNum").GetComponentsInChildren<UILabel>(true)[0];
        m_goSpriteUISkillDetailUIBtnAwake = FindTransform("SpriteUISkillDetailUIBtnAwake").gameObject;
        m_spSpriteUISkillDetailUIBtnAwakeBGUp = FindTransform("SpriteUISkillDetailUIBtnAwakeBGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_listDetailUIPos.Clear();
        for (int i = 1; i <= PROCESS_LINE_NUM + 1; i++)
        {
            m_listDetailUIPos.Add(FindTransform(string.Format("SpriteUISkillDetailGridPiecePos{0}", i)));
        }

        // 加载属性球
        LoadSpriteSkillDetailUIPieceList(() =>
            {
                SetSpriteSkillDetailUIPieceData(m_listSpriteSkillPieceData);
            });

        // 学会新技能悬浮UI
        m_goSpriteUISuccessUI = FindTransform("SpriteUISuccessUI").gameObject;
        m_lblSpriteUISuccessUIText = FindTransform("SpriteUISuccessUIText").GetComponentsInChildren<UILabel>(true)[0];
        m_spSpriteUISuccessUIIcon = FindTransform("SpriteUISuccessUIIconFG").GetComponentsInChildren<UISprite>(true)[0];

        Initialize();
    }

    #region 事件

    public Action SPRITEUICLOSEUP;
    public Action SPRITEUISWITCHTODETAILEND;
    public Action SPRITEUISKILLUP;

    public Action SPRITEUILEARNSKILLUP;
    public Action SPRITEUILEARNSKILLUIBACKUP;
    public Action SPRITEUILEARNSKILLUIRESETUP;
    public Action SPRITEUILEARNSKILLUIUPGRADEUP;
    public Action SPRITEUILEARNSKILLUIEQUIPUP;
    public Action<int> SPRITEUILEARNSKILLCHOOSEUP;

    public void Initialize()
    {
        FindTransform("SpriteUIBtnClose").GetComponentsInChildren<MogoButton>(true)[0].clickHandler+= OnSpriteUICloseUp;
        FindTransform("SpriteUIBtnSkill").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSpriteSkillUISkillUp;

        FindTransform("SpriteUISkillDetailUIBtnBack").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSpriteDetailUIBackUp;
        FindTransform("SpriteUISkillDetailUIBtnAwake").GetComponentsInChildren<MogoButton>(true)[0].pressHandler += OnSpriteDetailUIAwakePress;

        FindTransform("SpriteUILearnSkillUIBtnLearn").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSpriteLearnUILearnSkillUp;
        FindTransform("SpriteUILearnSkillUIBtnBack").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSpriteLearnUIBackUp;
        FindTransform("SpriteUILearnSkillUIBtnEquip").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSpriteLearnUIEquipUp;
        FindTransform("SpriteUILearnSkillUIBtnReset").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSpriteLearnUIResetUp;
        FindTransform("SpriteUILearnSkillUIBtnUpgrade").GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnSpriteLearnUIUpgradeUp;       

        SpriteUILogicManager.Instance.Initialize();
        m_uiLoginManager = SpriteUILogicManager.Instance;
    }

    public void Release()
    {
        SpriteUILogicManager.Instance.Release();
    }

    /// <summary>
    /// 点击关闭按钮
    /// </summary>
    void OnSpriteUICloseUp()
    {
        if (SPRITEUICLOSEUP != null)
            SPRITEUICLOSEUP();
    }

    #region 技能UI事件

    /// <summary>
    /// 切换到觉醒界面
    /// </summary>
    void OnSpriteSkillUISwitchToSpriteDetailUIEnd()
    {
        if (SPRITEUISWITCHTODETAILEND != null)
            SPRITEUISWITCHTODETAILEND();
    }

    /// <summary>
    /// 点击精灵技能按钮
    /// </summary>
    void OnSpriteSkillUISkillUp()
    {
        if (SPRITEUISKILLUP != null)
            SPRITEUISKILLUP();
    }

    #endregion

    #region 技能觉醒UI事件

    void OnSpriteDetailUIBackUp()
    {
        EventDispatcher.TriggerEvent(SpriteUIDict.SpriteUIEvent.OnBackUp);
    }

    private bool IsAwakePress = false;
    void OnSpriteDetailUIAwakePress(bool isPress)
    {
        IsAwakePress = isPress;
        EventDispatcher.TriggerEvent<bool>(SpriteUIDict.SpriteUIEvent.OnAwakePress, isPress);
    }

    #endregion

    #region 技能领悟UI事件

    /// <summary>
    /// 点击领悟UI领悟按钮
    /// </summary>
    void OnSpriteLearnUILearnSkillUp()
    {
        if (SPRITEUILEARNSKILLUP != null)
            SPRITEUILEARNSKILLUP();
    }

    /// <summary>
    /// 点击领悟UI返回按钮
    /// </summary>
    void OnSpriteLearnUIBackUp()
    {
        if (SPRITEUILEARNSKILLUIBACKUP != null)
            SPRITEUILEARNSKILLUIBACKUP();
    }

    /// <summary>
    /// 点击领悟UI装备按钮
    /// </summary>
    void OnSpriteLearnUIEquipUp()
    {
        if (SPRITEUILEARNSKILLUIEQUIPUP != null)
            SPRITEUILEARNSKILLUIEQUIPUP();
    }

    /// <summary>
    /// 点击领悟UI重置按钮
    /// </summary>
    void OnSpriteLearnUIResetUp()
    {
        if (SPRITEUILEARNSKILLUIRESETUP != null)
            SPRITEUILEARNSKILLUIRESETUP();
    }

    /// <summary>
    /// 点击领悟UI升级按钮
    /// </summary>
    void OnSpriteLearnUIUpgradeUp()
    {
        if (SPRITEUILEARNSKILLUIUPGRADEUP != null)
            SPRITEUILEARNSKILLUIUPGRADEUP();
    } 

    #endregion

    #endregion

    #region Update

    // 学会新技能悬浮UI
    const float LEARN_SUCCESS_EXIST_TIME = 3.0f;
    private float m_fLearnSuccessCurrentTime = 0f;

    void Update()
    {
        // 觉醒
        if (IsAwakePress)
        {
            EventDispatcher.TriggerEvent<bool>(SpriteUIDict.SpriteUIEvent.OnAwakePress, IsAwakePress);
        }

        // 更新精灵技能名字Pos
        UpdateSpriteSkillNamePos();

        // 学会新技能悬浮UI
        if (LearnSuccessUIBeginCountDown)
        {
            m_fLearnSuccessCurrentTime += Time.deltaTime;

            if (m_fLearnSuccessCurrentTime >= LEARN_SUCCESS_EXIST_TIME)
            {
                LearnSuccessUIBeginCountDown = false;                
            }
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    t1 += 0.1f;
        //    if (t1 >= 1)
        //    {
        //        t0++;
        //        t1 = 0;
        //    }
        //    ShowSkillLineFX(t0, t1);
        //}
    }

    static int t0 = 0;
    static float t1 = 0;

    #endregion

    #region 界面信息

    /// <summary>
    /// 当前的精灵UI
    /// </summary>
    private SpriteUIEnum m_currentSpriteUI = SpriteUIEnum.SpriteSkillUI;
    public SpriteUIEnum CurrentSpriteUI
    {
        get { return m_currentSpriteUI; }
        set
        {
            m_currentSpriteUI = value;
        }
    }

    public bool IsInSpriteDetailUI()
    {
        if (CurrentSpriteUI == SpriteUIEnum.SpriteDetailUI)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 觉醒UI技能Index
    /// </summary>
    private int m_spriteDetailUISpriteSkillIndex = -1;
    public int SpriteDetailUISpriteSkillIndex
    {
        get { return m_spriteDetailUISpriteSkillIndex; }
        set
        {
            m_spriteDetailUISpriteSkillIndex = value;
        }
    }

    /// <summary>
    /// 领悟UI技能Index
    /// </summary>
    private int m_spriteLearnUISpriteSkillIndex = -1;
    public int SpriteLearnUISpriteSkillIndex
    {
        get { return m_spriteLearnUISpriteSkillIndex; }
        set
        {
            m_spriteLearnUISpriteSkillIndex = value;
        }
    }

    /// <summary>
    /// 技能UI的技能是否点击-在播放切换UI过程中不可点击
    /// </summary>
    private bool m_isSpriteSkillClickEnable = true;
    public bool IsSpriteSkillClickEnable
    {
        get { return m_isSpriteSkillClickEnable; }
        set
        {
            m_isSpriteSkillClickEnable = value;
        }
    }

    #endregion

    #region 技能UI和技能觉醒UI切换、技能UI和技能领悟UI切换

    /// <summary>
    /// 从技能觉醒UI切换到技能UI
    /// </summary>
    public void SwitchSpriteSkillDetailUIToSpriteSkillUI()
    {
        CurrentSpriteUI = SpriteUIEnum.SpriteSkillUI;
        ResetSpriteSkillUI();
        ShowSpriteSkillList(true);
        SpriteNPCActionChange(SpriteAnimatorController.ActionStatus.ActionNormal);
        
        m_goSpriteUISkillDetailUI.SetActive(false);
        m_goSpriteUISkillUI.SetActive(true);
    }

    /// <summary>
    /// 从技能UI切换到技能觉醒UI
    /// </summary>
    public void SwitchSpriteSkillUIToSpriteSkillDetailUI()
    {
        CurrentSpriteUI = SpriteUIEnum.SpriteDetailUI;
        ShowSpriteSkillList(false, SpriteDetailUISpriteSkillIndex);
        LockAwakeButtonByUnLockLevel();

        m_goSpriteUISkillUI.SetActive(false);
        m_goSpriteUISkillDetailUI.SetActive(true);
    }

    /// <summary>
    /// 从技能UI切换到技能领悟UI
    /// </summary>
    public void SwitchSpriteSkillUIToSpriteLearnSkillUI()
    {
        CurrentSpriteUI = SpriteUIEnum.SpriteLearnUI;
        m_goSpriteUILearnSkillUI.SetActive(true);
    }

    /// <summary>
    /// 从技能领悟UI切换到技能UI
    /// </summary>
    public void SwitchSpriteLearnSkillUIToSpriteSkillUI()
    {
        CurrentSpriteUI = SpriteUIEnum.SpriteSkillUI;
        ResetSpriteSkillUI();
        ShowSpriteSkillList(true);

        m_goSpriteUILearnSkillUI.SetActive(false);
        m_goSpriteUISkillUI.SetActive(true);
    }

    /// <summary>
    /// 播放切换到技能详细UI动画
    /// </summary>
    /// <param name="detaiIndex"></param>
    public void PlaySwitchToSpriteSkillDetailUI(int detaiIndex)
    {
        SpriteDetailUISpriteSkillIndex = detaiIndex;

        for (int index = 0; index < MaxSpriteSkillNum; index++)
        {
            if (m_maplistSpriteSkill.ContainsKey(index))
            {
                if (index == detaiIndex)
                {
                    TweenPosition tp = m_maplistSpriteSkill[index].GetComponentsInChildren<TweenPosition>(true)[0];
                    tp.Reset();
                    tp.eventReceiver = gameObject;
                    tp.callWhenFinished = "OnSpriteSkillUISwitchToSpriteDetailUIEnd";
                    tp.duration = SWITCH_TO_DETAIL_DURATION;
                    tp.from = m_listSpriteUISkillPos[index];
                    tp.to = new Vector3(m_goSpriteSceneModelPos.transform.localPosition.x,
                        m_goSpriteSceneModelPos.transform.localPosition.y,
                        m_listSpriteUISkillPos[index].z);
                    tp.enabled = true;
                    tp.Play(true);
                }
                else
                {
                    TweenAlpha ta = m_maplistSpriteSkill[index].GetComponentsInChildren<TweenAlpha>(true)[0];
                    ta.Reset();
                    ta.eventReceiver = gameObject;
                    ta.callWhenFinished = "";
                    ta.duration = SWITCH_TO_DETAIL_DURATION;
                    ta.from = 1;
                    ta.to = 0;
                    ta.enabled = true;
                    ta.Play(true);
                }
            }
        }
    }

    #endregion      

    #region  精灵系统领悟UI

    #region 加载技能学习Prefab

    private Dictionary<int, SpriteUILearnSkill> m_maplistSpriteUILearnSkill = new Dictionary<int, SpriteUILearnSkill>();

    /// <summary>
    /// 加载技能学习Prefab
    /// </summary>
    /// <param name="action"></param>
    private void LoadSpriteUILearnSkillList(Action action = null)
    {
        for (int i = 0; i < MAX_LEARN_SKILL_NUM; i++)
        {
            //加载billBoard
            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

            int index = i;
            AssetCacheMgr.GetUIInstance("SpriteUILearnSkill.prefab",
                (str, id, obj) =>
                {
                    GameObject goLearnSkill = obj as GameObject;
                    goLearnSkill.transform.parent = m_goGOSpriteUILearnSkillUILearn.transform;
                    goLearnSkill.transform.localPosition = m_listSpriteLearnUISkillPos[index];
                    goLearnSkill.transform.localScale = new Vector3(1, 1, 1);

                    if (m_maplistSpriteUILearnSkill.ContainsKey(index))
                        AssetCacheMgr.ReleaseInstance(m_maplistSpriteUILearnSkill[index].gameObject);
                    m_maplistSpriteUILearnSkill[index] = goLearnSkill.AddComponent<SpriteUILearnSkill>();
                    m_maplistSpriteUILearnSkill[index].LoadResourceInsteadOfAwake();

                    INSTANCE_COUNT--;
                    if (INSTANCE_COUNT <= 0) MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                    if (m_maplistSpriteUILearnSkill.Count == MAX_LEARN_SKILL_NUM)
                    {
                        if (action != null) action();
                    }
                });
        }
    }

    private void AddToSingleButtonList()
    {
        MogoSingleButtonList singleButtonList = m_goGOSpriteUILearnSkillUILearn.GetComponentsInChildren<MogoSingleButtonList>(true)[0];
        singleButtonList.SingleButtonList.Clear();
        for (int index = 0; index < MAX_LEARN_SKILL_NUM; index++)
        {
            if (m_maplistSpriteUILearnSkill.ContainsKey(index))
                singleButtonList.SingleButtonList.Add(m_maplistSpriteUILearnSkill[index].GetComponentsInChildren<MogoSingleButton>(true)[0]);
        }
    }

    #endregion

    #region 领悟特效

    private string m_fx1SkillLearn = "SkillPieceAwakeFx";
    private string SkillLearnFXName = "fx_ui_NewInstanceUIChooseLevelQuitBtnDown3.prefab";

    public void ShowSkillLearnFX(int index)
    {
        TimerHeap.AddTimer((uint)(2000), 0, () => { ReleaseSkillLearnFX(); });
        MogoFXManager.Instance.AttachParticleAnim(SkillLearnFXName, m_fx1SkillLearn, m_maplistSpriteUILearnSkill[index].transform.position,
            MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
            {

            });
    }

    /// <summary>
    /// 释放领悟特效
    /// </summary>
    private void ReleaseSkillLearnFX()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fx1SkillLearn);
    }

    #endregion

    #region 领悟时转圈

    #endregion

    /// <summary>
    /// 设置精灵系统领悟UI的学习技能列表
    /// </summary>
    /// <param name="listSpriteLearnSkillData"></param>
    private List<SpriteLearnSkillData> m_listSpriteLearnSkillData = new List<SpriteLearnSkillData>();
    public void SetListSpriteUILearnSkill(List<SpriteLearnSkillData> listSpriteLearnSkillData)
    {
        m_listSpriteLearnSkillData = listSpriteLearnSkillData;
        if(m_maplistSpriteUILearnSkill.Count == MAX_LEARN_SKILL_NUM)
        {
            for(int index = 0; index < MAX_LEARN_SKILL_NUM; index++)
            {
                if(index < listSpriteLearnSkillData.Count && m_maplistSpriteUILearnSkill.ContainsKey(index))
                {
                    m_maplistSpriteUILearnSkill[index].Index = index;
                    m_maplistSpriteUILearnSkill[index].SetLearnSkillName(listSpriteLearnSkillData[index].name);
                    m_maplistSpriteUILearnSkill[index].ShowLearnSkillFlag(listSpriteLearnSkillData[index].isEquipped);
                    m_maplistSpriteUILearnSkill[index].SetLearnSkillIcon(listSpriteLearnSkillData[index].icon, listSpriteLearnSkillData[index].hasLearned);
                }                
            }            
        }       
    }

    /// <summary>
    /// 设置当前技能
    /// </summary>
    /// <param name="skillName">技能名称</param>
    /// <param name="iconName">技能Icon</param>
    /// <param name="skillLevel">技能等级</param>
    public void SetDetailCurrentSkill(string skillName, string iconName, int skillLevel)
    {
        if (m_lblDetailSkillLevel != null && m_lblDetailSkillName != null && m_spDetailSkillIcon != null)
        {
            m_lblDetailSkillLevel.text = string.Concat("LV", skillLevel);
            m_lblDetailSkillName.text = skillName;
            m_spDetailSkillIcon.spriteName = iconName;
        }     
    }    

    /// <summary>
    /// 设置当前技能描述
    /// </summary>
    /// <param name="desc"></param>
    public void SetDetailCurrentSkillDesc(string desc)
    {
        m_lblDetailCurrentSkillDesc.text = desc;
    }

    /// <summary>
    /// 设置当前技能下级描述
    /// </summary>
    /// <param name="desc"></param>
    public void SetDetailNextSkillDesc(string desc)
    {
        m_lblDetailNextSkillDesc.text = desc;
    }

    /// <summary>
    /// 设置下级技能升级需求
    /// </summary>
    /// <param name="request"></param>
    public void SetDetailNextSkillRequest(string request)
    {
        m_lblDetailNextSkillRequest.text = request;
    }

    /// <summary>
    /// 设置下级技能所需的进度
    /// </summary>
    /// <param name="progress"></param>
    public void SetDetailNextSkillProgress(float progress)
    {
        progress = Math.Min(progress, 1);
        progress = Math.Max(0, progress);
        m_spDetailNextSkillProgressFG.fillAmount = progress;
    }

    /// <summary>
    /// 设置当前可领悟次数
    /// </summary>
    /// <param name="num"></param>
    public void SetLearnTimesNum(int num)
    {
        m_lblLearnTimesNum.text = num.ToString();
    }

    /// <summary>
    /// 设置当前技能
    /// </summary>
    /// <param name="spriteName"></param>
    /// <param name="name"></param>
    /// <param name="level"></param>
    public void SetCurrentSkill(string spriteName, string name, int level)
    {
        m_goSpriteUILearnSkillUIDetailSkill.transform.Find("SpriteUILearnSkillUIDetailSkillIcon").GetComponentsInChildren<UISprite>(true)[0].spriteName = spriteName;
        m_goSpriteUILearnSkillUIDetailSkill.transform.Find("SpriteUILearnSkillUIDetailSkillName").GetComponentsInChildren<UILabel>(true)[0].text = name;
        m_goSpriteUILearnSkillUIDetailSkill.transform.Find("SpriteUILearnSkillUIDetailSkillLevel").GetComponentsInChildren<UILabel>(true)[0].text = string.Concat("LV", level);
    }

    /// <summary>
    /// 是否显示下一等级技能信息
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowDetailNextSkillInfo(bool isShow)
    {
        m_goDetailNextSkill.SetActive(isShow);
        m_goDetailNextSkillReq.SetActive(isShow);
    }

    #region 学会新技能悬浮UI
    
    /// <summary>
    /// 是否显示学会新技能悬浮UI
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowSpriteUISuccessUI(bool isShow)
    {
        LearnSuccessUIBeginCountDown = isShow;
        m_goSpriteUISuccessUI.SetActive(isShow);
    }

    /// <summary>
    /// 设置学会新技能悬浮UI提示文字
    /// </summary>
    /// <param name="text"></param>
    public void SetSpriteUISuccessUIText(string text)
    {
        m_lblSpriteUISuccessUIText.text = text;
    }

    /// <summary>
    /// 设置学会新技能悬浮UI技能Icon
    /// </summary>
    /// <param name="spriteName"></param>
    public void SetSpriteUISuccessUIIcon(string spriteName)
    {
        MogoUIManager.Instance.TryingSetSpriteName(spriteName, m_spSpriteUISuccessUIIcon);
    }

    /// <summary>
    /// 开始倒数学会新技能悬浮UI悬浮时间
    /// </summary>
    private bool m_bLearnSuccessUIBeginCountDown = false;
    public bool LearnSuccessUIBeginCountDown
    {
        get
        {
            return m_bLearnSuccessUIBeginCountDown;
        }
        set
        {
            m_bLearnSuccessUIBeginCountDown = value;
            if(!value)
                m_fLearnSuccessCurrentTime = 0f;
        }
    }   
      
    #endregion

    #endregion

    #region 精灵技能觉醒UI

    /// <summary>
    /// 设置女神之泪数量
    /// </summary>
    /// <param name="num"></param>
    public void SetMaterialCurrentNum(int num)
    {        
        m_lblSpriteUISkillDetailUIMaterialNum.text = num.ToString();
    }

    /// <summary>
    /// 是否显示下级所需女神之泪
    /// </summary>
    /// <param name="isShow"></param>
    /// <param name="nextNum"></param>
    public void ShowMaterialNextNum(bool isShow, int nextNum = 0)
    {
        m_goSpriteUISkillDetailUIMaterialNext.SetActive(isShow);
        m_lblSpriteUISkillDetailUIMaterialNextNum.text = nextNum.ToString();
    }

    /// <summary>
    /// 取消觉醒按钮按下状态
    /// </summary>
    public void CancelAwakePress()
    {
        IsAwakePress = false;
    }

    /// <summary>
    /// 觉醒按钮是否开启(通过精灵技能解锁等级)
    /// </summary>
    public void LockAwakeButtonByUnLockLevel()
    {
        if (SpriteDetailUISpriteSkillIndex < m_listSpriteSkillData.Count)
        {
            if (MogoWorld.thePlayer.level >= m_listSpriteSkillData[SpriteDetailUISpriteSkillIndex].unlockLevel)
            {
                LockAwakeButton(false);
            }
            else
            {
                LockAwakeButton(true);
            }
        }
    }

    /// <summary>
    /// 觉醒按钮是否开启
    /// </summary>
    /// <param name="isLock"></param>
    private void LockAwakeButton(bool isLock)
    {
        if(!isLock)
        {
            m_goSpriteUISkillDetailUIBtnAwake.GetComponentsInChildren<BoxCollider>(true)[0].enabled = true;
            m_spSpriteUISkillDetailUIBtnAwakeBGUp.spriteName = "btn_04up";
        }
        else
        {
            m_goSpriteUISkillDetailUIBtnAwake.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;
            m_spSpriteUISkillDetailUIBtnAwakeBGUp.spriteName = "btn_03grey";
        }        
    }

    #region 进度线列表

    private readonly static int PROCESS_LINE_NUM = 6; // 显示最后一条线
    private List<Transform> m_listDetailUIPos = new List<Transform>();

    /// <summary>
    /// 加载觉醒进度条列表
    /// </summary>
    /// <param name="action"></param>
    private Dictionary<int, GameObject> m_maplistGOProgressLine = new Dictionary<int, GameObject>();
    private Dictionary<int, UISprite> m_maplistProgressLine = new Dictionary<int, UISprite>();
    private void LoadSpriteProgressLineList(Action action = null)
    {
        for (int i = 0; i < PROCESS_LINE_NUM; i++)
        {           
            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

            int index = i;
            AssetCacheMgr.GetUIInstance("SpriteUIProgressLine.prefab",
                (str, id, obj) =>
                {
                    GameObject goProgressLine = obj as GameObject;
                    goProgressLine.transform.parent = m_goSpriteUISkillDetailUIProgressLineList.transform;
                    goProgressLine.transform.localPosition = new Vector3(100*index - 200, -50*index+ 200, -1);
                    goProgressLine.transform.localScale = new Vector3(1, 1, 1);
                    goProgressLine.name = string.Format("SpriteUIProgressLine{0}", index);

                    if (m_maplistGOProgressLine.ContainsKey(index))
                        AssetCacheMgr.ReleaseInstance(m_maplistGOProgressLine[index]);
                    m_maplistGOProgressLine[index] = goProgressLine;
                    m_maplistProgressLine[index] = goProgressLine.transform.Find("SpriteUIProgressLineFG").GetComponentsInChildren<UISprite>(true)[0];

                    INSTANCE_COUNT--;
                    if (INSTANCE_COUNT <= 0) MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                    if (m_maplistGOProgressLine.Count == PROCESS_LINE_NUM)
                    {
                        if (action != null) action();
                    }
                });
        }
    }

    /// <summary>
    /// 计算进度线位置
    /// </summary>
    private readonly static float LINE_LENGTH_SPACE = 0.0f;
    private readonly static float LINE_LENGTH = 400.0f + LINE_LENGTH_SPACE;    
    private void CalSpriteProgressLinePos()
    {
        if (m_maplistGOProgressLine.Count != PROCESS_LINE_NUM || m_listDetailUIPos.Count != PROCESS_LINE_NUM + 1)
            return;

        for (int round = 0; round < PROCESS_LINE_NUM; round++)
        {
            float distance = Vector3.Distance(m_listDetailUIPos[round].localPosition, m_listDetailUIPos[round + 1].localPosition);
            GameObject goProgressLine = m_maplistGOProgressLine[round];
            goProgressLine.transform.localScale = new Vector3((1.0f / LINE_LENGTH) * distance, 1, 1);
            goProgressLine.transform.localPosition = m_listDetailUIPos[round].localPosition;
            Vector3 targetDir = m_listDetailUIPos[round + 1].localPosition - m_listDetailUIPos[round].localPosition;
            Vector3 x_axis = new Vector3(1, 0, 0);
            float angle = Vector3.Angle(targetDir, x_axis);

            Vector3 z_axis = new Vector3(0, 0, -1);
            if (m_listDetailUIPos[round + 1].localPosition.y > m_listDetailUIPos[round].localPosition.y)
                z_axis = new Vector3(0, 0, 1);

            goProgressLine.transform.localRotation = Quaternion.AngleAxis(angle, z_axis);      
            
            // 微调位置
            goProgressLine.transform.localPosition += new Vector3(0, 20, 0);
        }
    }

    /// <summary>
    /// 设置觉醒进度
    /// </summary>
    /// <param name="index"></param>
    /// <param name="progress"></param>
    public void SetAwakeProgressList(int index, float progress)
    {
        if (IsAwakePress)
            ShowSkillLineFX(index, progress);
        if (IsAwakePress && progress == 1.0f)
            ShowSkillPieceAwakeFX(index + 1);

        for (int i = 0; i < m_maplistProgressLine.Count; i++)
        {
            if (i < index)
            {
                SetAwakeProgress(i, 1.0f);
            }
            else if (i == index)
            {
                SetAwakeProgress(i, progress);             
            }
            else if (i > index)
            {
                SetAwakeProgress(i, 0.0f);
            }
        }
    }

    /// <summary>
    /// 设置每条进度线的值
    /// </summary>
    /// <param name="index"></param>
    /// <param name="progress"></param>
    private void SetAwakeProgress(int index, float progress)
    {
        if (m_maplistProgressLine.ContainsKey(index))
        {
            progress = Math.Max(0, progress);
            progress = Math.Min(1, progress);
            m_maplistProgressLine[index].fillAmount = progress;                    
        }
    }

    #endregion

    #region 属性球

    /// <summary>
    /// 加载属性球(使用贴图)
    /// </summary>
    private readonly static int SKILL_PIECE_NUM = 7;
    private Dictionary<int, SpriteUISkillPiece> m_maplistSkillPiece = new Dictionary<int, SpriteUISkillPiece>();
    private void LoadSpriteSkillDetailUIPieceList(Action action = null)
    {
        for (int i = 0; i < SKILL_PIECE_NUM; i++)
        {
            //加载billBoard
            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);

            int index = i;
            AssetCacheMgr.GetUIInstance("SpriteUIDetailSkillPiece.prefab",
                (str, id, obj) =>
                {
                    GameObject goSkillPiece = obj as GameObject;
                    goSkillPiece.transform.parent = m_goSpriteUISkillDetailGridPieceList.transform;
                    goSkillPiece.transform.localPosition = m_listDetailUIPos[index].localPosition;
                    goSkillPiece.transform.localScale = new Vector3(1, 1, 1);

                    if (m_maplistSkillPiece.ContainsKey(index))
                        AssetCacheMgr.ReleaseInstance(m_maplistSkillPiece[index].gameObject);
                    m_maplistSkillPiece[index] = goSkillPiece.AddComponent<SpriteUISkillPiece>();
                    m_maplistSkillPiece[index].LoadResourceInsteadOfAwake();

                    INSTANCE_COUNT--;
                    if (INSTANCE_COUNT <= 0) MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                    if (m_maplistSkillPiece.Count == SKILL_PIECE_NUM)
                    {
                        if (action != null) action();
                    }
                });
        }
    }

    /// <summary>
    /// 设置精灵技能属性球列表
    /// </summary>
    /// <param name="listSpriteSkillDetailData"></param>
    private List<SpriteSkillPieceData> m_listSpriteSkillPieceData = new List<SpriteSkillPieceData>();
    public void SetSpriteSkillDetailUIPieceData(List<SpriteSkillPieceData> listSpriteSkillPieceData)
    {
        m_listSpriteSkillPieceData = listSpriteSkillPieceData;
        if (m_maplistSkillPiece.Count == SKILL_PIECE_NUM)
        {
            for (int index = 0; index < SKILL_PIECE_NUM; index++)
            {                
                if (m_maplistSkillPiece.ContainsKey(index) && index < m_listSpriteSkillPieceData.Count)
                {
                    SpriteUISkillPiece gridUI = m_maplistSkillPiece[index];
                    SpriteSkillPieceData gridData = m_listSpriteSkillPieceData[index];
                    gridUI.Index = index;
                    gridUI.SetSkillPiece(gridData.isAwake, gridData.name, gridData.icon);
                }
            }
        }
    }

    #endregion

    #region 属性球激活特效

    private string m_fx1SkillPieceAwake = "SkillPieceAwakeFx";
    private string SkillPieceAwakeFXName = "fx_ui_NewInstanceUIChooseLevelQuitBtnUp_b.prefab";
    private readonly static uint SkillPieceAwakeFXLiftTime = 2000;

    private void ShowSkillPieceAwakeFX(int index)
    {
        if (m_maplistSkillPiece.ContainsKey(index))
        {
            GameObject goFX = MogoFXManager.Instance.FindParticeAnim(m_fx1SkillPieceAwake);
            if (goFX != null)
                return;

            TimerHeap.AddTimer((uint)(SkillPieceAwakeFXLiftTime), 0, () => { ReleaseSkillPieceAwakeFX(); });
            MogoFXManager.Instance.AttachParticleAnim(SkillPieceAwakeFXName, m_fx1SkillPieceAwake, m_maplistSkillPiece[index].transform.position,
                MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
                {

                });     
        }       
    }

    /// <summary>
    /// 释放属性球激活特效
    /// </summary>
    private void ReleaseSkillPieceAwakeFX()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fx1SkillPieceAwake);
    }

    #endregion

    #region 进度线特效

    private string m_fx1SkillLine = "SkillLineFx";
    private string SkillLineFXName = "fx_ui_NewInstanceUIChooseLevelQuitBtnUp.prefab";
    private uint m_iSkillLineFXTimerID;
    private readonly static uint SkillLineFXLifeTime = 200;

    /// <summary>
    /// 显示进度线特效
    /// </summary>
    /// <param name="index"></param>
    private void ShowSkillLineFX(int index, float progress)
    {        
        if (CurrentSpriteUI != SpriteUIEnum.SpriteDetailUI)
            return;

        progress = Math.Max(0, progress);
        progress = Math.Min(1, progress);
        GameObject goFX = MogoFXManager.Instance.FindParticeAnim(m_fx1SkillLine);
        if (goFX != null)
        {
            TimerHeap.DelTimer(m_iSkillLineFXTimerID);
            m_iSkillLineFXTimerID = TimerHeap.AddTimer((uint)(SkillLineFXLifeTime), 0, () => { ReleaseSkillLineFX(); });
        }
        else
        {
            m_iSkillLineFXTimerID = TimerHeap.AddTimer((uint)(SkillLineFXLifeTime), 0, () => { ReleaseSkillLineFX(); });
            MogoFXManager.Instance.AttachParticleAnim(SkillLineFXName, m_fx1SkillLine, CalSkillLineFXPos(index, progress),
                MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
                {

                });
        }

        RefreshSkillLineFXPos(index, progress);
    }

    /// <summary>
    /// 计算进度线特效Pos
    /// </summary>
    /// <returns></returns>
    private Vector3 CalSkillLineFXPos(int index, float progress)
    {
        if (index < m_listDetailUIPos.Count && index + 1 < m_listDetailUIPos.Count)
        {
            float distance = Vector3.Distance(m_listDetailUIPos[index].position, m_listDetailUIPos[index + 1].position);
            float progressDistance = progress * distance;

            return Vector3.MoveTowards(m_listDetailUIPos[index].transform.position,
                m_listDetailUIPos[index + 1].transform.position,
                progressDistance);
        }
        else
            return m_listDetailUIPos[0].transform.position;
    }

    /// <summary>
    /// 刷新进度线特效Pos
    /// </summary>
    private void RefreshSkillLineFXPos(int index, float progress)
    {
        GameObject goFX = MogoFXManager.Instance.FindParticeAnim(m_fx1SkillLine);
        if (goFX != null)
        {
            //Debug.LogError("ShowSkillLineFX:" + index + " ; " + progress);
            //Debug.LogError(CalSkillLineFXPos(index, progress));
            MogoFXManager.Instance.TransformToFXCameraPos(goFX, CalSkillLineFXPos(index, progress), MogoUIManager.Instance.GetMainUICamera());
        }        
    }

    /// <summary>
    /// 释放进度线特效
    /// </summary>
    private void ReleaseSkillLineFX()
    {
        MogoFXManager.Instance.ReleaseParticleAnim(m_fx1SkillLine);
    }

    #endregion

    #endregion

    #region 精灵场景

    private readonly static string SPRITEUI_SCENE_NAME = "elfScene.prefab";
    private Camera m_spriteSceneCamera;
    public Camera SpriteSceneCamera
    {
        get { return m_spriteSceneCamera; }
        set
        {
            m_spriteSceneCamera = value;
            CalSpriteUISkillDetailSkillModelPos();
        }
    }

    private GameObject m_goSpriteUIScene;

    /// <summary>
    /// 加载精灵系统场景
    /// </summary>
    private bool IsSpriteUISceneLoaded = false;
    public void LoadSpriteUIScene(Action action = null)
    {
        if (!IsSpriteUISceneLoaded)
        {
            IsSpriteUISceneLoaded = true;

            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);
            AssetCacheMgr.GetInstance(SPRITEUI_SCENE_NAME, (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.transform.localPosition = new Vector3(0, 0, 0);
                m_goSpriteUIScene = obj;
                SpriteSceneCamera = obj.transform.Find("elf/Main Camera").GetComponentsInChildren<Camera>(true)[0];
                SpriteSceneCamera.clearFlags = CameraClearFlags.Depth;
                if (SpriteSceneCamera.GetComponentsInChildren<UICamera>(true).Length <= 0)
                    SpriteSceneCamera.gameObject.AddComponent<UICamera>();

                if (action != null)
                    action();

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
        }
    }

    /// <summary>
    /// 添加SpriteSkill漂移的目标GameObject
    /// </summary>
    private GameObject m_goSpriteSceneModelPos;
    private void AddSpriteUISkillDetailSkillModelPos()
    {
        m_goSpriteSceneModelPos = m_goSpriteUIScene.transform.Find("elf_icon/SpriteSkillCenterPos").gameObject;
        //if (m_goSpriteSceneModelPos == null)
        //{
        //    m_goSpriteSceneModelPos = new GameObject();
        //    m_goSpriteSceneModelPos.name = "SpriteSceneModelPos";
        //    m_goSpriteSceneModelPos.transform.parent = m_goSpriteUIScene.transform.FindChild("elf_icon").transform;
        //    TimerHeap.AddTimer(2000, 0, CalSpriteUISkillDetailSkillModelPos);
        //}       
    }

    /// <summary>
    /// 计算SpriteSkill漂移的目标pos
    /// </summary>
    public void CalSpriteUISkillDetailSkillModelPos()
    {
        return;
        //if (SpriteSceneCamera != null && RootCamera != null)
        //{
        //    m_vecSpriteUISkillDetailSkillModelPos = RootCamera.WorldToScreenPoint(m_goSpriteUISkillDetailSkillPos.transform.position);
        //    m_vecSpriteUISkillDetailSkillModelPos = SpriteSceneCamera.ScreenToWorldPoint(m_vecSpriteUISkillDetailSkillModelPos);

        //    Debug.LogError((RootCamera.name) + " " + SpriteSceneCamera.name);
        //    if (m_goSpriteSceneModelPos != null)
        //        m_goSpriteSceneModelPos.transform.position = m_vecSpriteUISkillDetailSkillModelPos;
        //}
    }

    #endregion

    #region 精灵NPC

    public readonly static string SpriteNPCName = "NPC_150020.prefab";
    private GameObject m_goSpriteNPC;
    private SpriteAnimatorController m_SpriteAnimatorController;

    /// <summary>
    /// 获取精灵NPC并添加动作控制脚本
    /// </summary>
    private void AddComponentToSpriteNPC()
    {
        // NPC不直接放到场景,动态加载
        //m_goSpriteNPC = m_goSpriteUIScene.transform.FindChild(SpriteNPCName).gameObject;

        if (m_goSpriteNPC != null)
        {
            m_SpriteAnimatorController = m_goSpriteNPC.AddComponent<SpriteAnimatorController>();
            m_SpriteAnimatorController.LoadResourceInsteadOfAwake();
        }       
    }

    /// <summary>
    /// 加载精灵系统NPC
    /// </summary>
    private bool IsSpriteUINPCLoaded = false;
    public void LoadSpriteUINPC(Action action = null)
    {
        if (!IsSpriteUINPCLoaded)
        {
            IsSpriteUINPCLoaded = true;

            INSTANCE_COUNT++;
            MogoGlobleUIManager.Instance.ShowWaitingTip(true);
            AssetCacheMgr.GetInstance(SpriteNPCName, (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_goSpriteUIScene.transform;
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.transform.localPosition = new Vector3(0, 0, 0);
                m_goSpriteNPC = obj;          

                if (action != null)
                    action();

                INSTANCE_COUNT--;
                if (INSTANCE_COUNT <= 0)
                    MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
        }
    }

    /// <summary>
    /// 切换精灵NPC动作
    /// </summary>
    /// <param name="status"></param>
    public void SpriteNPCActionChange(SpriteAnimatorController.ActionStatus status)
    {
        if (m_SpriteAnimatorController != null)
        {
            m_SpriteAnimatorController.ActionChange(status);
        }
    }

    #endregion

    #region 精灵技能UI

    #region 精灵技能

    private readonly static int MaxSpriteSkillNum = 5;
    public readonly static string SpriteSkill1Name = "jinlin_Apollo";
    public readonly static string SpriteSkill2Name = "jinlin_Ares";
    public readonly static string SpriteSkill3Name = "jinlin_Athena";
    public readonly static string SpriteSkill4Name = "jinlin_Hades";
    public readonly static string SpriteSkill5Name = "jinlin_Zeus";
    public readonly static string SpriteSkill1FxName = "fx_elf_icon_jinlin_apollo";
    public readonly static string SpriteSkill2FxName = "fx_elf_icon_jinlin_ares";
    public readonly static string SpriteSkill3FxName = "fx_elf_icon_jinlin_athena";
    public readonly static string SpriteSkill4FxName = "fx_elf_icon_jinlin_hades";
    public readonly static string SpriteSkill5FxName = "fx_elf_icon_jinlin_zeus";

    private Dictionary<int, GameObject> m_maplistSpriteSkill = new Dictionary<int, GameObject>();
    private Dictionary<int, UILabel> m_maplistSpriteSkillName = new Dictionary<int, UILabel>();
    private Dictionary<int, GameObject> m_maplistSpriteSkillFx = new Dictionary<int, GameObject>();
    private List<Vector3> m_listSpriteUISkillPos = new List<Vector3>();
    private List<SpriteSkillData> m_listSpriteSkillData = new List<SpriteSkillData>();
    private readonly static float SWITCH_TO_DETAIL_DURATION = 0.5f;   

    /// <summary>
    /// 通过控件名获取精灵技能
    /// </summary>
    private void GetSpriteSkillByName()
    {
        m_maplistSpriteSkill.Clear();
        m_maplistSpriteSkill[0] = m_goSpriteUIScene.transform.Find("elf_icon/" + SpriteSkill1Name).gameObject;
        m_maplistSpriteSkill[1] = m_goSpriteUIScene.transform.Find("elf_icon/" + SpriteSkill2Name).gameObject;
        m_maplistSpriteSkill[2] = m_goSpriteUIScene.transform.Find("elf_icon/" + SpriteSkill3Name).gameObject;
        m_maplistSpriteSkill[3] = m_goSpriteUIScene.transform.Find("elf_icon/" + SpriteSkill4Name).gameObject;
        m_maplistSpriteSkill[4] = m_goSpriteUIScene.transform.Find("elf_icon/" + SpriteSkill5Name).gameObject;

        m_listSpriteUISkillPos.Clear();
        for (int i = 0; i < MaxSpriteSkillNum; i++)
        {
            m_listSpriteUISkillPos.Add(m_maplistSpriteSkill[i].transform.localPosition);
        }
    }
  
    /// <summary>
    /// 在精灵技能上添加所需的控件
    /// </summary>
    private void SetSpriteSkillComponent()
    {
        for (int i = 0; i < MaxSpriteSkillNum; i++)
        {
            if (m_maplistSpriteSkill.ContainsKey(i))
            {
                BoxCollider boxCollider = m_maplistSpriteSkill[i].AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(1, 1, 1);
                boxCollider.center = new Vector3(0, 0.8f, 0);

                SpriteUISkill gridUI = m_maplistSpriteSkill[i].AddComponent<SpriteUISkill>();
                gridUI.Index = i;
                gridUI.SpriteSkillCamera = SpriteSceneCamera;
                gridUI.cc = boxCollider;               

                TweenAlpha tweenAlpha = m_maplistSpriteSkill[i].AddComponent<TweenAlpha>();
                tweenAlpha.enabled = false;
                tweenAlpha.method = UITweener.Method.Linear;
                tweenAlpha.style = UITweener.Style.Once;

                TweenPosition tweenPosition = m_maplistSpriteSkill[i].AddComponent<TweenPosition>();
                tweenPosition.enabled = false;
                tweenPosition.method = UITweener.Method.Linear;
                tweenPosition.style = UITweener.Style.Once;
            }          
        }
    }

    /// <summary>
    /// 加载精灵技能名称BillBorard
    /// </summary>
    /// <param name="action"></param>
    private void LoadSpriteSkillBillBorard(Action action = null)
    {
        for (int i = 0; i < MaxSpriteSkillNum; i++)
        {
            if (m_maplistSpriteSkill.ContainsKey(i))
            {
                //加载billBoard
                INSTANCE_COUNT++;
                MogoGlobleUIManager.Instance.ShowWaitingTip(true);

                int index = i;
                AssetCacheMgr.GetUIInstance("SpriteUIBillBoard.prefab",
                    (str, id, obj) =>
                    {
                        GameObject billBoardGo = obj as GameObject;
                        billBoardGo.transform.parent = m_spriteUIBillBoardCamera.transform;
                        billBoardGo.transform.localScale = new Vector3(22f, 22f, 1);

                        UILabel lblName = billBoardGo.GetComponentsInChildren<UILabel>(true)[0];
                        if (m_maplistSpriteSkillName.ContainsKey(index))
                            AssetCacheMgr.ReleaseInstance(m_maplistSpriteSkillName[index].gameObject);
                        m_maplistSpriteSkillName[index] = lblName;

                        INSTANCE_COUNT--;
                        if (INSTANCE_COUNT <= 0) MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                        if (m_maplistSpriteSkillName.Count == MaxSpriteSkillNum)
                        {
                            if (action != null) action();
                        }
                    });
            }
        }
    }

    /// <summary>
    /// 显示所有精灵技能名称
    /// </summary>
    private void ShowSpriteSkillBillBorard()
    {
        foreach (UILabel lbl in m_maplistSpriteSkillName.Values)
        {
            lbl.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏精灵技能名称
    /// </summary>
    private void HideSpriteSkillBillBorard()
    {
        foreach (UILabel lbl in m_maplistSpriteSkillName.Values)
        {
            lbl.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 加载精灵技能特效
    /// </summary>
    /// <param name="action"></param>
    private void LoadSpriteSkillFx(Action action = null)
    {
        for (int i = 0; i < MaxSpriteSkillNum; i++)
        {
            if (m_maplistSpriteSkill.ContainsKey(i))
            {
                //加载billBoard
                INSTANCE_COUNT++;
                MogoGlobleUIManager.Instance.ShowWaitingTip(true);

                int index = i;
                string skillFxName = "";
                if (index == 0)
                    skillFxName = SpriteSkill1FxName;
                else if(index == 1)
                    skillFxName = SpriteSkill2FxName;
                else if (index == 2)
                    skillFxName = SpriteSkill3FxName;
                else if (index == 3)
                    skillFxName = SpriteSkill4FxName;
                else if (index == 4)
                    skillFxName = SpriteSkill5FxName;

                AssetCacheMgr.GetUIInstance(string.Concat(skillFxName, ".prefab"),
                    (str, id, obj) =>
                    {
                        GameObject goSkillFx = obj as GameObject;
                        goSkillFx.transform.parent = m_maplistSpriteSkill[index].transform.Find("bone_Plane001/Bone001");
                        goSkillFx.transform.localScale = new Vector3(1f, 1f, 1f);
                        goSkillFx.transform.localPosition = new Vector3(0, 0, 0);
                        goSkillFx.transform.localRotation = Quaternion.identity;
                        goSkillFx.SetActive(false);

                        if (m_maplistSpriteSkillFx.ContainsKey(index))
                            AssetCacheMgr.ReleaseInstance(m_maplistSpriteSkillFx[index]);
                        m_maplistSpriteSkillFx[index] = goSkillFx;

                        INSTANCE_COUNT--;
                        if (INSTANCE_COUNT <= 0) MogoGlobleUIManager.Instance.ShowWaitingTip(false);

                        if (m_maplistSpriteSkillFx.Count == MaxSpriteSkillNum)
                        {
                            if (action != null) action();
                        }
                    });
            }           
        }
    }   

    /// <summary>
    /// 更新精灵技能的名字Pos
    /// </summary>
    private void UpdateSpriteSkillNamePos()
    {
        for (int index = 0; index < MaxSpriteSkillNum; index++)
        {
            if (m_maplistSpriteSkill.ContainsKey(index) && m_maplistSpriteSkillName.ContainsKey(index))
            {
                Vector3 pos = SpriteSceneCamera.WorldToScreenPoint(m_maplistSpriteSkill[index].transform.position);
                pos = m_spriteUIBillBoardCamera.ScreenToWorldPoint(pos);
                m_maplistSpriteSkillName[index].transform.position = pos + new Vector3(0, 235, 0);
            }          
        }       
    }

    /// <summary>
    /// 设置精灵技能信息列表
    /// </summary>
    /// <param name="listSpriteSkillData"></param>
    public void SetSpriteSkillListData(List<SpriteSkillData> listSpriteSkillData)
    {
        m_listSpriteSkillData = listSpriteSkillData;
        if (m_maplistSpriteSkill.Count != MaxSpriteSkillNum || m_maplistSpriteSkillFx.Count != MaxSpriteSkillNum)
            return;

        for (int index = 0; index < MaxSpriteSkillNum; index++)
        {           
            if (index < listSpriteSkillData.Count)
            {
                SpriteSkillData gridData = listSpriteSkillData[index];
                if (m_maplistSpriteSkillName.ContainsKey(index))
                {
                    if (MogoWorld.thePlayer.level >= gridData.unlockLevel)
                    {
                        // 等级足够，已填满
                        if (gridData.hasFilled)
                        {
                            if (m_maplistSpriteSkill.ContainsKey(index))
                                m_maplistSpriteSkillName[index].text = gridData.name;

                            if (m_maplistSpriteSkillFx.ContainsKey(index))
                                m_maplistSpriteSkillFx[index].SetActive(true);
                        }
                        // 等级足够，未填满
                        else
                        {
                            if (m_maplistSpriteSkill.ContainsKey(index))
                                m_maplistSpriteSkillName[index].text = gridData.name;

                            if (m_maplistSpriteSkillFx.ContainsKey(index))
                                m_maplistSpriteSkillFx[index].SetActive(false);
                        }                      
                    }
                    // 等级不足
                    else 
                    {
                        if (m_maplistSpriteSkill.ContainsKey(index))
                            m_maplistSpriteSkillName[index].text = string.Format(LanguageData.GetContent(49005), gridData.unlockLevel);

                        if (m_maplistSpriteSkillFx.ContainsKey(index))
                            m_maplistSpriteSkillFx[index].SetActive(false);
                    }
                }

                if (m_maplistSpriteSkill.ContainsKey(index))
                    m_maplistSpriteSkill[index].GetComponentsInChildren<SpriteUISkill>(true)[0].Index = index;
            }         
        }
    }

    /// <summary>
    /// 重置精灵界面-默认在SkillUI
    /// </summary>
    private void ResetSpriteSkillUI()
    {
        CurrentSpriteUI = SpriteUIEnum.SpriteSkillUI;
        IsSpriteSkillClickEnable = true;

        for (int index = 0; index < MaxSpriteSkillNum; index++)
        {
            if (m_maplistSpriteSkill.ContainsKey(index))
            {
                if (index < m_listSpriteUISkillPos.Count)
                    m_maplistSpriteSkill[index].transform.localPosition = m_listSpriteUISkillPos[index];

                TweenAlpha ta = m_maplistSpriteSkill[index].GetComponentsInChildren<TweenAlpha>(true)[0];
                ta.Reset();
                ta.enabled = false;
            }
        }

        m_goSpriteUILearnSkillUI.SetActive(false);
        m_goSpriteUISkillDetailUI.SetActive(false);
        m_goSpriteUISkillUI.SetActive(true);
    }

    /// <summary>
    /// 是否显示精灵技能列表
    /// </summary>
    /// <param name="isShow"></param>
    private void ShowSpriteSkillList(bool isShow, int showSkillIndex = - 1)
    {
        for (int i = 0; i < MaxSpriteSkillNum; i++)
        {
            if (m_maplistSpriteSkill.ContainsKey(i))
            {
                if (i == showSkillIndex)
                    m_maplistSpriteSkill[i].SetActive(true);
                else
                    m_maplistSpriteSkill[i].SetActive(isShow);
            }            

            //if (m_maplistSpriteSkillName.ContainsKey(i))
            //{
            //    if (i == showSkillIndex)
            //        m_maplistSpriteSkillName[i].gameObject.SetActive(true);
            //    else
            //        m_maplistSpriteSkillName[i].gameObject.SetActive(isShow);
            //}
        }

        if (isShow)
            ShowSpriteSkillBillBorard();
        else
            HideSpriteSkillBillBorard();
    }

    #endregion   

    #endregion   

    #region 界面打开和关闭

    private readonly static int SPRITE_SCENE_ID = 914;

    /// <summary>
    /// 展示界面信息
    /// </summary>
    /// <param name="IsShow"></param>
    protected override void OnEnable()
    {
        base.OnEnable();
        MogoGlobleUIManager.Instance.ShowWaitingTip(false);

        // 雾效
        RenderSettings.fog = false; // 雾效影响场景，故关闭
        if (SPRITE_SCENE_ID > 0)
            EventDispatcher.TriggerEvent(Events.GearEvent.SwitchLightMapFog, SPRITE_SCENE_ID);

        // 加载精灵系统场景
        LoadSpriteUIScene(() =>
        {
            LoadSpriteUINPC(() =>
                {
                    AddComponentToSpriteNPC();
                });
            
            GetSpriteSkillByName();
            SetSpriteSkillComponent();
            AddSpriteUISkillDetailSkillModelPos();

            LoadSpriteSkillBillBorard(() =>
                {                    
                    SetSpriteSkillListData(m_listSpriteSkillData);
                    ResetSpriteSkillUI();
                });

            LoadSpriteSkillFx(() =>
                {
                    SetSpriteSkillListData(m_listSpriteSkillData);
                });

            LoadSpriteProgressLineList(() =>
                {
                    CalSpriteProgressLinePos();
                });
        });

        // 重置UI
        ResetSpriteSkillUI();
    }

    /// <summary>
    /// 关闭界面卸载
    /// </summary>
    void OnDisable()
    {
        // 雾效
        RenderSettings.fog = true;
        if (MogoWorld.thePlayer.sceneId == MogoWorld.globalSetting.homeScene)
            EventDispatcher.TriggerEvent(Events.GearEvent.SwitchLightMapFog, MogoWorld.globalSetting.homeScene);

        // 卸载精灵场景
        IsSpriteUISceneLoaded = false;
        // 卸载NPC
        IsSpriteUINPCLoaded = false;
        if (m_goSpriteUIScene != null)
            AssetCacheMgr.ReleaseInstance(m_goSpriteUIScene);
        AssetCacheMgr.ReleaseResourceImmediate(SPRITEUI_SCENE_NAME);

        m_maplistSpriteSkillFx.Clear(); // 精灵技能特效列表

        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroySpriteUI();
        }
    }

    #endregion   
}
