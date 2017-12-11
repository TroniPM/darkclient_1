/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TipUIManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;
using System;

public class TipUIManager : MogoUIParent
{
    public static TipUIManager Instance;

    /// <summary>
    /// 优先级->tip队列
    /// </summary>
    List<Queue<TipViewData>> m_tipQueueList = new List<Queue<TipViewData>>();
    //Queue<TipViewData> m_equipTipQueue = new Queue<TipViewData>();
    //Queue<TipViewData> m_giftTipQueue = new Queue<TipViewData>();
    //Queue<TipViewData> m_enhanceTipQueue = new Queue<TipViewData>();
    //Queue<TipViewData> m_skillTipQueue = new Queue<TipViewData>();

    UISprite m_iconFg;
    UISprite m_iconBg;
    MogoUIListener m_btn;
    UILabel m_btnLbl;
    UILabel m_tipLbl;
    UIAtlas m_tempAtlas;
    //private bool m_isShowingEquip = false;
    //private bool m_isShowingGift = false;
    //private bool m_isShowingEnhance = false;
    //private bool m_isShowingSkill = false;

    GameObject m_goFlashObj;
    private uint m_currentTimerId = 0;

    void Awake()
    {
        base.Init();
        Instance = this;

        m_iconFg = GetUIChild("HelpTipIconFG").GetComponent<UISprite>();
        m_iconBg = GetUIChild("HelpTipIconBG").GetComponent<UISprite>();
        m_btn = GetUIChild("HelpTipBtn").gameObject.AddComponent<MogoUIListener>();
        m_btnLbl = GetUIChild("HelpTipBtnText").gameObject.GetComponent<UILabel>();
        m_tipLbl = GetUIChild("HelpTipLbl").GetComponent<UILabel>();
        m_goFlashObj = GetUIChild("HelpTipBtnFlashObj").gameObject;


        m_tipQueueList = new List<Queue<TipViewData>>();
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());
        m_tipQueueList.Add(new Queue<TipViewData>());

        if (NormalMainUIViewManager.Instance != null)
            NormalMainUIViewManager.Instance.ShowHelpTip(false);
        //gameObject.SetActive(false);
    }

    //private bool ShowSkillTip()
    //{
    //    if (m_isShowingSkill) return true;
    //    if (m_skillTipQueue.Count <= 0) return false;

    //    TipViewData viewData = m_skillTipQueue.Peek();
    //    SetupTipView(viewData, false);
    //    m_isShowingSkill = true;

    //    return true;
    //}

    private void SetupTipView(TipViewData viewData)
    {
        TimerHeap.DelTimer(m_currentTimerId);
        if (viewData.itemId != 0)
        {
            InventoryManager.SetIcon(viewData.itemId, m_iconFg, 0, null, m_iconBg);
        }
        else
        {
            m_iconBg.spriteName = IconData.blankBox;
            m_iconFg.spriteName = viewData.icon;
            if (viewData.atlasName == "")
            {
                m_iconFg.atlas = MogoUIManager.Instance.GetAtlasByIconName(viewData.icon);
            }
            else
            {

                if (m_tempAtlas != null && m_tempAtlas.name == viewData.atlasName)
                {
                    m_iconFg.atlas = m_tempAtlas;
                }
                else
                {
                    if (m_tempAtlas != null)
                    {
                        //AssetCacheMgr.ReleaseResource(m_tempAtlas.texture);
                        AssetCacheMgr.ReleaseInstance(m_tempAtlas.gameObject, SystemSwitch.DestroyAllUI);
                        m_tempAtlas = null;
                    }

                    AssetCacheMgr.GetUIInstance(viewData.atlasName + ".prefab", (prefab1, guid1, gameObject1) =>
                        {
                            //LoggerHelper.Error("Tip:" + prefab1);
                            GameObject go1 = (GameObject)gameObject1;
                            m_tempAtlas = go1.GetComponentInChildren<UIAtlas>();
                            go1.hideFlags = HideFlags.HideAndDontSave;
                            m_iconFg.atlas = m_tempAtlas;
                        });
                }

            }

        }

        uint timeId = TimerHeap.AddTimer(viewData.existTime, 0, () => { Hide(viewData.priority); });
        m_currentTimerId = timeId;
        m_btn.m_onClick = viewData.btnAction;
        m_btn.m_onClick += () =>
        {
            TimerHeap.DelTimer(timeId);
        };
        m_btnLbl.text = viewData.btnName;
        m_tipLbl.text = viewData.tipText;

    }

    //private bool ShowingEnhanceTip()
    //{
    //    if (m_isShowingEnhance) return true;
    //    if (m_enhanceTipQueue.Count <= 0) return false;

    //    TipViewData viewData = m_enhanceTipQueue.Peek();
    //    SetupTipView(viewData);
    //    m_isShowingEnhance = true;

    //    return true;
    //}

    //private bool ShowingGiftTip()
    //{
    //    if (m_isShowingGift) return true;
    //    if (m_giftTipQueue.Count <= 0) return false;
    //    TipViewData viewData = m_giftTipQueue.Peek();
    //    SetupTipView(viewData);
    //    m_isShowingGift = true;
    //    return true;
    //}

    //private bool ShowingEquipTip()
    //{
    //    if (m_isShowingEquip) return true;
    //    if (m_equipTipQueue.Count <= 0) return false;
    //    TipViewData viewData = m_equipTipQueue.Peek();
    //    SetupTipView(viewData);
    //    m_isShowingEquip = true;
    //    return true;
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">0装备，1礼品，2强化</param>
    public void Hide(int type)
    {
        if (m_tipQueueList[type].Count > 0)
        {
            m_tipQueueList[type].Dequeue();
        }


        if (!HasAnyTip())
        {
            if (m_tempAtlas != null)
            {
                //AssetCacheMgr.ReleaseResource(m_tempAtlas.texture);
                AssetCacheMgr.ReleaseInstance(m_tempAtlas.gameObject, SystemSwitch.DestroyAllUI);
                m_tempAtlas = null;
            }

            if (NormalMainUIViewManager.Instance != null)
                NormalMainUIViewManager.Instance.ShowHelpTip(false);
            //gameObject.SetActive(false);
        }
        else
        {
            ShowTip();
        }
    }

    public void HideAll(int type)
    {
        m_tipQueueList[type].Clear();

        if (!HasAnyTip())
        {
            if (m_tempAtlas != null)
            {
                //AssetCacheMgr.ReleaseResource(m_tempAtlas.texture);
                AssetCacheMgr.ReleaseInstance(m_tempAtlas.gameObject, SystemSwitch.DestroyAllUI);
                m_tempAtlas = null;
            }

            if (NormalMainUIViewManager.Instance != null)
                NormalMainUIViewManager.Instance.ShowHelpTip(false);
            //gameObject.SetActive(false);
        }
        else
        {
            ShowTip();
        }
    }

    bool HasAnyTip()
    {
        foreach (Queue<TipViewData> queue in m_tipQueueList)
        {
            if (queue.Count > 0) return true;
        }
        return false;
        //return m_equipTipQueue.Count > 0 || m_giftTipQueue.Count > 0
        //    || m_enhanceTipQueue.Count > 0 || m_skillTipQueue.Count > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewData"></param>
    /// <param name="type">0装备,1礼品,2强化,3技能</param>
    public void AddTipViewData(TipViewData viewData)
    {
        bool hasAnyTip = HasAnyTip();
        m_tipQueueList[viewData.priority].Enqueue(viewData);
        //switch (type)
        //{
        //    case 0:
        //        m_equipTipQueue.Enqueue(viewData);
        //        break;
        //    case 1:
        //        m_giftTipQueue.Enqueue(viewData);
        //        break;
        //    case 2:
        //        m_enhanceTipQueue.Clear();
        //        m_enhanceTipQueue.Enqueue(viewData);
        //        break;
        //    case 3:
        //        m_skillTipQueue.Clear();
        //        m_skillTipQueue.Enqueue(viewData);
        //        break;
        //}

        //if (!hasAnyTip)
        {
            ShowTip();
        }
    }

    private void ShowTip()
    {
        //Debug.LogError("ShowTip");
        //MogoUIQueue.Instance.PushOne(() =>
        //{

        if (NormalMainUIViewManager.Instance != null)
            NormalMainUIViewManager.Instance.ShowHelpTip(true);
        //gameObject.SetActive(true);
        m_goFlashObj.SetActive(true);

        for (int i = 0; i < m_tipQueueList.Count; i++)
        {
            if (m_tipQueueList[i].Count <= 0) continue;
            TipViewData viewData = m_tipQueueList[i].Peek();
            SetupTipView(viewData);
            break;
        }
        //    if (ShowingEquipTip()) return;
        //if (ShowingGiftTip()) return;
        //if (ShowSkillTip()) return;
        //if (ShowingEnhanceTip()) return;
        //}, 
        //MogoUIManager.Instance.m_NormalMainUI, "ShowHelperTip");
    }
}

/// <summary>
/// 道具表里面的用itemid，纯粹要一个icon的话用icon
/// </summary>
public class TipViewData
{
    /// <summary>
    /// 毫秒
    /// </summary>
    public uint existTime = 20000;
    public int priority = 0;
    public int itemId = 0;
    public string icon;
    public string tipText;
    public string btnName;
    public Action btnAction;
    public string atlasName = "";
}