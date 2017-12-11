#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：
// 创建者：HongChengguo
// 修改者列表：
// 创建日期：
// 模块描述：胜利奖励界面
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using System.Collections;
using Mogo.Util;
using System.Collections.Generic;
using System;
using Mogo.GameData;

public class InstancePassRewardUIViewManager : MogoUIBehaviour
{
    private static InstancePassRewardUIViewManager m_instance;
    public static InstancePassRewardUIViewManager Instance { get { return InstancePassRewardUIViewManager.m_instance; } } 

    const float REWARDITEMSPACE = 0.153f;    

    private Transform m_tranInstanceRewardItemList;
    private GameObject m_goGOInstancePassRewardUIText;
    private GameObject m_goGOInstancePassRewardUIItemListArrow;
    private GameObject m_goGOInstancePassRewardUIItemListBG;
    private GameObject m_goInstancePassRewardUIItemListBGPosFrom;
    private GameObject m_goInstancePassRewardUIItemListBGPosTo;
    private GameObject m_goGOInstancePassRewardUI;

    private int m_RewardItemNum;
    private List<GameObject> m_listInstaceRewardItem = new List<GameObject>();
    private Camera m_camRewardItemList;

    private GameObject m_goInstancePassRewardUIBGMask;
    private GameObject m_goInstancePassRewardUIWinText;
    private GameObject m_goInstancePassRewardUIWinTextPosBegin;
    private GameObject m_goInstancePassRewardUIWinTextPosEnd;
    private GameObject m_goInstancePassRewardUIWinTextBG;

    private GameObject m_goInstancePassReweardUITopLine;
    private GameObject m_goInstancePassReweardUIBottomLine;
    private UILabel m_lblInstancePassRewardUILeftTimeText;

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        Camera temp = GameObject.Find("Camera").GetComponentInChildren<Camera>();
        m_myTransform.Find(m_widgetToFullName["InstancePassRewardItemListCamera"]).GetComponent<UIViewport>().sourceCamera = temp;

        m_tranInstanceRewardItemList = m_myTransform.Find(m_widgetToFullName["InstancePassRewardItemList"]);
        m_camRewardItemList = m_myTransform.Find(m_widgetToFullName["InstancePassRewardItemListCamera"]).GetComponentsInChildren<Camera>(true)[0];

        m_goGOInstancePassRewardUIText = m_myTransform.Find(m_widgetToFullName["GOInstancePassRewardUIText"]).gameObject;
        m_goGOInstancePassRewardUIItemListArrow = m_myTransform.Find(m_widgetToFullName["GOInstancePassRewardUIItemListArrow"]).gameObject;
        m_goGOInstancePassRewardUIItemListBG = m_myTransform.Find(m_widgetToFullName["GOInstancePassRewardUIItemListBG"]).gameObject;
        m_goInstancePassRewardUIItemListBGPosFrom = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUIItemListBGPosFrom"]).gameObject;
        m_goInstancePassRewardUIItemListBGPosTo = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUIItemListBGPosTo"]).gameObject;
        m_goGOInstancePassRewardUI = m_myTransform.Find(m_widgetToFullName["GOInstancePassRewardUI"]).gameObject;

        m_goInstancePassRewardUIBGMask = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUIBGMask"]).gameObject;
        m_goInstancePassRewardUIWinText = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUIWinText"]).gameObject;
        m_goInstancePassRewardUIWinTextBG = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUIWinTextBG"]).gameObject;
        m_goInstancePassReweardUIBottomLine = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUIWinTextBGBottomLine"]).gameObject;
        m_goInstancePassReweardUITopLine = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUIWinTextBGTopLine"]).gameObject;
        m_goInstancePassRewardUIWinTextPosBegin = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUIWinTextPosBegin"]).gameObject;
        m_goInstancePassRewardUIWinTextPosEnd = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUIWinTextPosEnd"]).gameObject;
        m_lblInstancePassRewardUILeftTimeText = m_myTransform.Find(m_widgetToFullName["InstancePassRewardUILeftTimeText"]).GetComponentsInChildren<UILabel>(true)[0];

        Initialize();
    }

    #region 事件
    public Action INSTANCEPASSOKUP;
    public Action INSTANCEPASSMAKEFRIENDUP;

    public void Initialize()
    {
        EventDispatcher.TriggerEvent("InstanceUILoadPartEnd");
    }

    public void Release()
    {

    }

    #endregion

    /// <summary>
    /// 隐藏UI
    /// </summary>
    public void HideAll()
    {
        // 隐藏大背景
        ShowInstancePassRewardUIMaskBG(false);        

        // 隐藏“掉落奖励”文字
        m_goGOInstancePassRewardUIText.SetActive(false);
        // 隐藏物品奖励背景
        m_goGOInstancePassRewardUIItemListBG.SetActive(false);
        // 隐藏箭头
        m_goGOInstancePassRewardUIItemListArrow.SetActive(false);

        //m_goInstancePassRewardUIWinText.SetActive(false);
        //m_goInstancePassReweardUIBottomLine.SetActive(false);
        //m_goInstancePassReweardUITopLine.SetActive(false);
    }

    public void ShowInstancePassRewardUI(bool isShow)
    {
        MainUIViewManager.Instance.LockOut = true;
        TimerHeap.AddTimer(3000, 0, ShowInstancePassRewardUIEnable);
    }

    void ShowInstancePassRewardUIEnable()
    {
        MainUIViewManager.Instance.LockOut = false;
        PlayVictoryAnimationOutSide();    

        MogoUIManager.Instance.LoadMogoInstancePassRewardUI(true);
        BeginCountDown = true;
        MogoUIManager.Instance.ShowMogoBattleMainUIWithInstancePassRewardUI(true);
    }

    public void ShowInstancePassRewardUIMaskBG(bool isShow)
    {
        //m_goInstancePassRewardUIBGMask.SetActive(isShow);
    }

    #region 物品奖励

    /// <summary>
    /// 添加奖励物品
    /// </summary>
    /// <param name="num"></param>
    /// <param name="act"></param>
    public void AddRewardItem(int num, Action act = null)
    {
        ClearRewardItemList();

        for (int i = 0; i < num; ++i)
        {          
            int index = i;

            AssetCacheMgr.GetUIInstance("InstanceRewardItem.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_tranInstanceRewardItemList;
                obj.transform.localPosition = new Vector3(REWARDITEMSPACE * index, 0, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);                            

                obj.SetActive(false);
                m_listInstaceRewardItem.Add(obj);              

                Mogo.Util.LoggerHelper.Debug("AddRewardItem !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! " + index + " " + num);

                //obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_camRewardItemList;

                if (m_listInstaceRewardItem.Count >= 5)
                {
                    m_camRewardItemList.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX =
                        m_myTransform.Find(m_widgetToFullName["InstancePassRewardItemListTransform0"]).localPosition.x +
                        (m_listInstaceRewardItem.Count - 5) * REWARDITEMSPACE;
                }
                else
                {
                    m_camRewardItemList.GetComponentsInChildren<MyDragableCamera>(true)[0].MAXX =
                       m_myTransform.Find(m_widgetToFullName["InstancePassRewardItemListTransform0"]).localPosition.x;
                }
            
                if (index == num - 1)
                {
                    if (act != null)
                    {
                        act();
                    }
                }
            });
        }
    }

    /// <summary>
    /// 设置物品信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="imgName"></param>
    /// <param name="itemName"></param>
    public void SetRewardItemData(int index, int id, int count, string itemName)
    {
        if (id > 0)
        {
            UISprite spIcon = m_listInstaceRewardItem[index].transform.Find("InstanceRewardItemFG").GetComponentsInChildren<UISprite>(true)[0];
            UISprite spBG = m_listInstaceRewardItem[index].transform.Find("InstanceRewardItemBG").GetComponentsInChildren<UISprite>(true)[0];
            UILabel lblNum = m_listInstaceRewardItem[index].transform.Find("InstanceRewardItemCount").GetComponentsInChildren<UILabel>(true)[0];

            InventoryManager.SetIcon(id, spIcon, count, lblNum, spBG);
            m_listInstaceRewardItem[index].transform.Find("InstanceRewardItemText").GetComponentsInChildren<UILabel>(true)[0].text = itemName;
            if (string.IsNullOrEmpty(lblNum.text) == false)
                lblNum.text = string.Concat("x", lblNum.text);

            m_listInstaceRewardItem[index].SetActive(false);
        }        
    }

    /// <summary>
    /// 清除奖励物品
    /// </summary>
    void ClearRewardItemList()
    {
        Mogo.Util.LoggerHelper.Debug("ClearRewardItemList " + m_listInstaceRewardItem.Count);
        for (int i = 0; i < m_listInstaceRewardItem.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listInstaceRewardItem[i]);
        }

        m_listInstaceRewardItem.Clear();
    }

    #endregion

    #region 播放动画

    readonly static float DelayTime = 0.1f;
    readonly static float DropTextDuration = 0.3f;
    readonly static float RewardItemBGDuration = 0.25f;//0.45f;
    readonly static float RewardItemDuration = 0.2f;
    readonly static float ArrowDuration = 0.1f;
    readonly static float UIOutBeforeDuration = 0.8f;
    readonly static float UIOutDuration = 0.55f;

    /// <summary>
    /// 调用播放奖励动画
    /// </summary>
    public void PlayerRewardAnimationOutSide()
    {
        MogoUIManager.Instance.LoadMogoInstancePassRewardUI(false);
        ResetVictoryPostionAnimation();
        MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.BattlePassUI);
        // PlayRewardAnimationBefore();
        // StartCoroutine(PlayerAnimation());
    }

    /// <summary>
    /// 播放奖励动画之前需要处理
    /// </summary>
    void PlayRewardAnimationBefore()
    {        
        ResetVictoryPostionAnimation();        

        // 3秒后播放结算界面结算动画
        TimerHeap.AddTimer(3000, 0, () =>
        {
            MogoUIManager.Instance.LoadMogoInstancePassUI(true);
            InstancePassUIViewManager.Instance.PlayResultOutside();
            //BillboardViewManager.Instance.ShowBillboard(MogoWorld.thePlayer.ID, true);
        });
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="delayTime"></param>
    /// <returns></returns>
    IEnumerator PlayerAnimation(float delayTime = 0.3f)
    {
        yield return new WaitForSeconds(0.01f);

        // 显示大背景
        ShowInstancePassRewardUIMaskBG(true);

        // "战斗胜利往上移"
        //TweenPosition winTP = m_goInstancePassRewardUIWinText.GetComponent<TweenPosition>();
        //if (winTP != null)
        //    winTP.enabled = true;

        // 显示“掉落奖励”文字
        m_goGOInstancePassRewardUIText.SetActive(true);
        yield return new WaitForSeconds(DropTextDuration);

        // 物品奖励背景从左至右切进屏幕
        m_goGOInstancePassRewardUIItemListBG.SetActive(true);
        TweenPosition itemBGTP = m_goGOInstancePassRewardUIItemListBG.GetComponent<TweenPosition>();
        if (itemBGTP != null)
            itemBGTP.enabled = true;
        yield return new WaitForSeconds(RewardItemBGDuration + DelayTime);

        // 物品奖励从左至右显示（动画效果：印章式撞击屏幕，且加UI特效）
        for (int i = 0; i < m_listInstaceRewardItem.Count; i++)
        {
            m_listInstaceRewardItem[i].SetActive(true);
            if (i < 5)
            {
                TweenScale ts = m_listInstaceRewardItem[i].GetComponent<TweenScale>();
                if (ts != null)
                    ts.enabled = true;
                yield return new WaitForSeconds(RewardItemDuration + DelayTime);
            }
        }

        // 显示箭头
        yield return new WaitForSeconds(0.5f);
        m_goGOInstancePassRewardUIItemListArrow.SetActive(true);
        yield return new WaitForSeconds(ArrowDuration);

        // 整个画面连胜利(除了背景)从左到右切出去
        yield return new WaitForSeconds(UIOutBeforeDuration);
        TweenPosition uiTP = m_goGOInstancePassRewardUI.GetComponent<TweenPosition>();
        if (uiTP != null)
            uiTP.enabled = true;
        yield return new WaitForSeconds(UIOutDuration);
    }

    public void PlayVictoryAnimationOutSide()
    {
        ResetVictoryScaleAnimation(() =>
        {
            ResetRewardAnimation(() =>
            {
                PlayVictoryAnimation();
            });
        });            
    }

    /// <summary>
    /// 播放胜利动画
    /// </summary>
    void PlayVictoryAnimation()
    {
        // 隐藏玩家名字
        BillboardViewManager.Instance.ShowBillboard(MogoWorld.thePlayer.ID, false);
        BillboardViewManager.Instance.ShowBillboard(MogoWorld.theLittleGuyID, false);

        PlayWinTextAnim();

        TimerHeap.AddTimer(1000, 0, () => { PlayLineAnim(); });

        //TweenScale winTS = m_goInstancePassRewardUIWinText.GetComponent<TweenScale>();
        //if (winTS != null)
        //{
        //    m_goInstancePassRewardUIWinText.SetActive(true);
        //    winTS.enabled = true;
        //}        
    }

    /// <summary>
    /// 重置胜利缩放动画
    /// </summary>
    void ResetVictoryScaleAnimation(Action action)
    {
        //TweenScale winTS = m_goInstancePassRewardUIWinText.GetComponent<TweenScale>();
        //if (winTS != null)
        //{
        //    //销毁该脚本   
        //    UnityEngine.Object Script = (UnityEngine.Object)winTS;
        //    Destroy(Script);
        //}
        
        //winTS = m_goInstancePassRewardUIWinText.AddComponent<TweenScale>();
        //winTS.transform.localPosition = m_goInstancePassRewardUIWinTextPosBegin.transform.localPosition;
        //winTS.transform.localScale = new Vector3(1600, 624, 1);
        //winTS.from = new Vector3(1600, 624, 1);
        //winTS.to = new Vector3(800, 312, 1);
        //winTS.duration = 0.1f;
        //winTS.style = UITweener.Style.Once;
        //winTS.animationCurve = new AnimationCurve(new Keyframe(0, 0), /*new Keyframe(0.5f, 0.5f),*/ new Keyframe(1, 1));
        //winTS.eventReceiver = gameObject;
        //winTS.callWhenFinished = "OnTSEnd";
        //winTS.enabled = false;

        action();
    }

    /// <summary>
    /// 重置胜利位移动画
    /// </summary>
    void ResetVictoryPostionAnimation()
    {
        // 重置“战斗胜利”
        //TweenPosition winTP = m_goInstancePassRewardUIWinText.GetComponent<TweenPosition>();
        //if (winTP != null)
        //{
        //    //销毁该脚本   
        //    UnityEngine.Object Script = (UnityEngine.Object)winTP;
        //    Destroy(Script);
        //}

        //winTP = m_goInstancePassRewardUIWinText.AddComponent<TweenPosition>();
        //winTP.from = m_goInstancePassRewardUIWinTextPosBegin.transform.localPosition;
        //winTP.to = m_goInstancePassRewardUIWinTextPosEnd.transform.localPosition;
        //winTP.duration = RewardItemBGDuration;
        //winTP.style = UITweener.Style.Once;
        //winTP.animationCurve = new AnimationCurve(new Keyframe(0, 0), /*new Keyframe(0.5f, 0.5f),*/ new Keyframe(1, 1));
        //winTP.eventReceiver = gameObject;
        //winTP.callWhenFinished = "OnTPEnd";
        //winTP.enabled = false;

       
    }

    /// <summary>
    /// 重置奖励动画
    /// </summary>
    void ResetRewardAnimation(Action action)
    {
        // 隐藏“掉落奖励”文字
        // 隐藏物品奖励背景
        // 隐藏箭头
        HideAll();

        // 设置物品奖励背景动画（需要每次移除动画脚本，重新添加）
        m_goGOInstancePassRewardUIItemListBG.SetActive(false);
        TweenPosition itemBGTP = m_goGOInstancePassRewardUIItemListBG.GetComponent<TweenPosition>();
        if (itemBGTP != null)
        {
            //销毁该脚本   
            UnityEngine.Object Script = (UnityEngine.Object)itemBGTP;
            Destroy(Script);
        }
        itemBGTP = m_goGOInstancePassRewardUIItemListBG.AddComponent<TweenPosition>();
        itemBGTP.from = m_goInstancePassRewardUIItemListBGPosFrom.transform.localPosition;
        itemBGTP.to = m_goInstancePassRewardUIItemListBGPosTo.transform.localPosition;
        itemBGTP.duration = RewardItemBGDuration;
        itemBGTP.style = UITweener.Style.Once;
        itemBGTP.animationCurve = new AnimationCurve(new Keyframe(0, 0), /*new Keyframe(0.5f, 0.5f),*/ new Keyframe(1, 1));
        itemBGTP.enabled = false;

        // 设置物品奖励动画，隐藏物品奖励
        for (int i = 0; i < m_listInstaceRewardItem.Count; i++)
        {
            if (i < 5)
            {
                Transform transform = m_listInstaceRewardItem[i].transform;
                TweenScale ts = m_listInstaceRewardItem[i].AddComponent<TweenScale>();
                ts.from = new Vector3(transform.localScale.x * 2.0f, transform.localScale.y * 2.0f, transform.localScale.z * 2.0f);
                ts.to = transform.localScale;
                ts.duration = RewardItemDuration;
                ts.style = UITweener.Style.Once;
                ts.enabled = false;
            }
            m_listInstaceRewardItem[i].SetActive(false);
        }

        // 设置整个画面动画(连胜利除了背景从左到右切出去)
        m_goGOInstancePassRewardUI.transform.localPosition = new Vector3(0, 0, 0);
        TweenPosition uiTP = m_goGOInstancePassRewardUI.GetComponent<TweenPosition>();
        if (uiTP != null)
        {
            //销毁该脚本   
            UnityEngine.Object Script = (UnityEngine.Object)uiTP;
            Destroy(Script);
        }
        uiTP = m_goGOInstancePassRewardUI.AddComponent<TweenPosition>();
        uiTP.from = new Vector3(0, 0, 0);
        uiTP.to = new Vector3(2000, 0, 0);
        uiTP.duration = UIOutDuration;
        uiTP.style = UITweener.Style.Once;
        uiTP.animationCurve = new AnimationCurve(new Keyframe(0, 0), /*new Keyframe(0.5f, 0.5f),*/ new Keyframe(1, 1));
        uiTP.enabled = false;

        action();
    }

    /// <summary>
    /// TweenScale播放动画结束调用
    /// </summary>
    /// <param name="ts"></param>
    void OnTSEnd(TweenScale ts)
    {
        if (ts != null)
        {
            ts.enabled = false;

            //销毁该脚本   
            UnityEngine.Object Script = ts.gameObject.GetComponentsInChildren<TweenScale>(true)[0];
            Destroy(Script);
        }
    }

    /// <summary>
    /// TweenPosition播放动画结束调用
    /// </summary>
    /// <param name="tp"></param>
    void OnTPEnd(TweenPosition tp)
    {
        if (tp != null)
        {
            tp.enabled = false;

            //销毁该脚本   
            UnityEngine.Object Script = tp.gameObject.GetComponentsInChildren<TweenPosition>(true)[0];
            Destroy(Script);
        }
    }

    #endregion

    #region 倒数离开副本时间

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
            ShowLeftTimeText(m_bBeginCountDown);
        }
    }   

    /// <summary>
    /// 设置离开副本时间
    /// </summary>
    /// <param name="time"></param>    
    private void SetLeaveTime(int time)
    {
        m_lblInstancePassRewardUILeftTimeText.text = time + LanguageData.GetContent(46967);
    }

    private void ShowLeftTimeText(bool isShow)
    {
        if (isShow == true)
            SetLeaveTime((int)LEAVEINSTANCETIME);
        m_lblInstancePassRewardUILeftTimeText.gameObject.SetActive(isShow);
    }

    public void PlayWinTextAnim()
    {
        m_goInstancePassRewardUIWinText.SetActive(true);
        m_goInstancePassRewardUIWinTextBG.SetActive(true);

        TweenScale ts = m_goInstancePassRewardUIWinText.GetComponent<TweenScale>();
        ts.Reset();
        ts.enabled = true;
        ts.Play(true);

        EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, m_goInstancePassRewardUIWinText.name);
    }

    public void PlayLineAnim()
    {
        m_goInstancePassReweardUIBottomLine.SetActive(true);
        m_goInstancePassReweardUITopLine.SetActive(true);

        TweenPosition tpBottom = m_goInstancePassReweardUIBottomLine.GetComponent<TweenPosition>();
        tpBottom.Reset();
        tpBottom.enabled = true;
        tpBottom.Play(true);

        TweenPosition tpTop = m_goInstancePassReweardUITopLine.GetComponent<TweenPosition>();
        tpTop.Reset();
        tpTop.enabled = true;
        tpTop.Play(true);
    }

    /// <summary>
    /// Update
    /// </summary>   
    const float LEAVEINSTANCETIME = 3.0f;
    private float m_fCurrentTime = 0f;
    private float m_fElapseTime = 0f;
    void Update()
    {
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

                // 关闭MainUI,并重置摇杆
                MogoUIManager.Instance.m_MainUI.SetActive(false);

                if (ControlStick.instance != null)
                    ControlStick.instance.Reset();

                //m_goInstancePassRewardUIWinText.SetActive(false);//隐藏"胜利",不播放位移动画,避免遮挡玩家视野
                m_goInstancePassRewardUIWinText.SetActive(false);
                m_goInstancePassReweardUIBottomLine.SetActive(false);
                m_goInstancePassReweardUITopLine.SetActive(false);
                m_goInstancePassRewardUIWinTextBG.SetActive(false);
                EventDispatcher.TriggerEvent(Events.InstanceEvent.GetCurrentReward);
                MogoMainCamera.Instance.PlayVictoryCG();

                TimerHeap.AddTimer(2000, 0, PlayerRewardAnimationOutSide);
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

        // Test UI
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    AddRewardItem(15);
        //}
        //else if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    PlayerAnimationOutSide();
        //}
    }

    #endregion
}
