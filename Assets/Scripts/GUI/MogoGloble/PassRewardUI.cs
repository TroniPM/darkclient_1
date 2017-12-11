using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;

public class PassRewardUI : MogoUIBehaviour
{
    private const float REWARDITEMSPACE = 0.153f;
    private const float LEAVEINSTANCETIME = 7.0f;
    private const float LEAVEINSTANCEWAITTIME = 3.0f;

    private List<GameObject> m_listItem = new List<GameObject>();
    private Transform m_instanceRewardItemList;
    private Camera m_camItemList;
    private Vector3[] m_vec3StartPos = new Vector3[5];
    private MogoButton m_mbOK;

    private GameObject m_goGOInstancePassRewardUI;
    private GameObject m_goInstancePassRewardUILoseText;
    private GameObject m_goInstancePassRewardUIWinText;

    // ʤ������
    private GameObject m_goPassRewardUIWinTextSP;
    private GameObject m_goPassRewardUIWinTextBG;
    private GameObject m_goPassReweardUITopLine;
    private GameObject m_goPassReweardUIBottomLine;

    void Awake()
    {
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_instanceRewardItemList = FindTransform("InstancePassRewardItemList");
        m_camItemList = FindTransform("InstancePassRewardItemListCamera").GetComponentsInChildren<Camera>(true)[0];
        m_camItemList.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        for (int i = 0; i < 5; ++i)
        {
            Mogo.Util.LoggerHelper.Debug("InstancePassRewardItemStartPos/InstancePassReward" +( i + 1).ToString() + "ItemStartPos");
            m_vec3StartPos[i] = FindTransform("InstancePassReward" + (i + 1).ToString() + "ItemStartPos").localPosition;
        }

        m_mbOK = FindTransform("InstancePassRewardUIOK").GetComponentsInChildren<MogoButton>(true)[0];
        m_goGOInstancePassRewardUI = FindTransform("GOInstancePassRewardUI").gameObject;
        m_goGOInstancePassRewardUI.SetActive(false);
        m_goInstancePassRewardUILoseText  = FindTransform("InstancePassRewardUILoseText").gameObject;
        m_goInstancePassRewardUIWinText = FindTransform("InstancePassRewardUIWinText").gameObject;
        m_goInstancePassRewardUIWinText.SetActive(false);

        // ʤ������
        m_goPassRewardUIWinTextSP = FindTransform("PassRewardUIWinTextSP").gameObject;
        m_goPassRewardUIWinTextBG = FindTransform("PassRewardUIWinTextBG").gameObject;
        m_goPassReweardUITopLine = FindTransform("PassRewardUIWinTextBGTopLine").gameObject;
        m_goPassReweardUIBottomLine = FindTransform("PassRewardUIWinTextBGBottomLine").gameObject;
    }

    void Start()
    {
        if (m_mbOK.clickHandler == null)
        {
            m_mbOK.clickHandler = OKAct;
        }
    }

    #region ʤ������

    private void PlayVictoryAnimation()
    {
        PlayWinTextAnim();
        TimerHeap.AddTimer(1000, 0, () => { PlayLineAnim(); });
    }

    /// <summary>
    /// ����"ս��ʤ��"����
    /// </summary>
    private void PlayWinTextAnim()
    {
        m_goPassRewardUIWinTextSP.SetActive(true);
        m_goPassRewardUIWinTextBG.SetActive(true);

        TweenScale ts = m_goPassRewardUIWinTextSP.GetComponent<TweenScale>();
        ts.Reset();
        ts.enabled = true;
        ts.Play(true);
    }

    private void StopPlayWinTextAnim()
    {
        // ����ս��ʤ��
        m_goPassRewardUIWinTextSP.SetActive(false);
        m_goPassRewardUIWinTextBG.SetActive(false);
        m_goPassReweardUIBottomLine.SetActive(false);
        m_goPassReweardUITopLine.SetActive(false);      
    }

    /// <summary>
    /// �������������߶���
    /// </summary>
    private void PlayLineAnim()
    {
        m_goPassReweardUIBottomLine.SetActive(true);
        m_goPassReweardUITopLine.SetActive(true);

        TweenPosition tpBottom = m_goPassReweardUIBottomLine.GetComponent<TweenPosition>();
        tpBottom.Reset();
        tpBottom.enabled = true;
        tpBottom.Play(true);

        TweenPosition tpTop = m_goPassReweardUITopLine.GetComponent<TweenPosition>();
        tpTop.Reset();
        tpTop.callWhenFinished = "OnTPTopLineEnd";
        tpTop.eventReceiver = gameObject;
        tpTop.enabled = true;
        tpTop.Play(true);
    }

    private void OnTPTopLineEnd()
    {
        TimerHeap.AddTimer(500, 0, () => 
        { 
            StopPlayWinTextAnim();
            ShowGOInstancePassRewardUI(true);
        });       
    }    

    #endregion

    #region ����

    /// <summary>
    /// �����Ƿ�ʤ��
    /// </summary>
    private bool m_isWin;
    public bool IsWin
    {
        get { return m_isWin; }
        set
        {
            m_isWin = value;

            if (m_isWin)
            {
                ShowGOInstancePassRewardUI(false);
                m_goInstancePassRewardUILoseText.SetActive(false);
            }
            else
            {
                ShowGOInstancePassRewardUI(true);
                m_goInstancePassRewardUILoseText.SetActive(true);
            }
        }
    }

    /// <summary>
    /// ��ʾ��Ʒ����
    /// </summary>
    private void ShowGOInstancePassRewardUI(bool isShow)
    {
        m_goGOInstancePassRewardUI.SetActive(isShow);
    }
   
    /// <summary>
    /// ��ӽ���Grid
    /// </summary>
    private Action OKAct = null;
    public void FillItemGridData(List<PassRewardGridData> data, Action cb)
    {
        ClearRewardItemList();

        if (m_mbOK == null)
        {
            OKAct = cb;
        }
        else
        {
            if (cb != null)
            {
                m_mbOK.clickHandler = cb;
            }
        }

        for (int i = 0; i < data.Count; ++i)
        {
            int index = i;

            AssetCacheMgr.GetUIInstance("InstanceRewardItem.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_instanceRewardItemList;
                obj.transform.localPosition = new Vector3(REWARDITEMSPACE * index, 0, 0);
                obj.transform.localScale = new Vector3(0.0008f, 0.0008f, 1);

                UISprite spIcon = obj.transform.Find("InstanceRewardItemFG").GetComponentsInChildren<UISprite>(true)[0];
                UISprite spBG = obj.transform.Find("InstanceRewardItemBG").GetComponentsInChildren<UISprite>(true)[0];
                UILabel lblNum = obj.transform.Find("InstanceRewardItemCount").GetComponentsInChildren<UILabel>(true)[0];
                UILabel lblName = obj.transform.Find("InstanceRewardItemText").GetComponentsInChildren<UILabel>(true)[0];
                InventoryManager.SetIcon(data[index].id, spIcon, 0, null, spBG);

                if (lblNum != null)
                {
                    lblNum.text = "";
                    lblName.text = data[index].iconName;
                    lblNum.text = string.Concat("x", data[index].num);
                }

                Mogo.Util.LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ " + spIcon.spriteName);
                m_listItem.Add(obj);

                if (index == data.Count - 1)
                {
                    if (m_camItemList == null)
                    {
                        LoggerHelper.Debug("m_camItemList is null");
                        m_camItemList = FindTransform("InstancePassRewardItemListCamera").GetComponentsInChildren<Camera>(true)[0];
                        m_camItemList.GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
                    }

                    if (m_listItem.Count <= 5)
                    {
                        if (m_camItemList != null)
                            m_camItemList.transform.localPosition = m_vec3StartPos[index];
                        else
                            LoggerHelper.Error("m_camItemList is null");
                    }
                    else
                    {
                        if (m_camItemList != null)
                            m_camItemList.transform.localPosition = m_vec3StartPos[4];
                        else
                            LoggerHelper.Error("m_camItemList is null");
                    }

                    if (IsWin)
                        PlayVictoryAnimation();
                }
            });
        }
    }

    /// <summary>
    /// ��ս����б�
    /// </summary>
    private void ClearRewardItemList()
    {
        for (int i = 0; i < m_listItem.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listItem[i]);
        }

        m_listItem.Clear();
    }  

    #endregion

    #region ����򿪺͹ر�

    protected override void OnEnable()
    {
        base.OnEnable();       
    }

    void OnDisable()
    {
        m_goGOInstancePassRewardUI.SetActive(false);
        m_goInstancePassRewardUILoseText.SetActive(false);
        StopPlayWinTextAnim();
        ShowGOInstancePassRewardUI(false);
    }

    #endregion
}
