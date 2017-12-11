using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using System;
using Mogo.GameData;

public class TimeLimitActivityInfo : MogoUIBehaviour
{
    private UILabel m_lblCDText;
    private UILabel m_lblInfoDesc;
    private UISprite m_spInfoImg;
    private UILabel m_lblRule;
    private UILabel m_lblInfoTitle;
    private GameObject m_goHasReward;

    private GameObject[] m_arrItem = new GameObject[4];

    private GameObject m_goGetBtn;
    private UISprite m_spGetBtnBGUp;
    private GameObject m_goShareBtn;

    private List<InventoryGrid> m_arrItemGrid = new List<InventoryGrid>();

    private MogoCountDown countDown = null;
    private GameObject m_goTimeLimitActivityUIItemListBG;

    /// <summary>
    /// ��ʱ���ϸ��Ϣ-�ڸ��ڵ�Active = falseʱ������Ϣ,���Լ�����Awake
    /// </summary>
    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if(m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;
        m_myTransform = transform;
        FillFullNameData(transform);

        m_lblCDText = FindTransform("TimeLimitActivityUIActivityInfoCDText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblInfoDesc = FindTransform("TimeLimitActivityUIActivityInfoDesc").GetComponentsInChildren<UILabel>(true)[0];
        m_spInfoImg = FindTransform("TimeLimitActivityUIActivityInfoImg").GetComponentsInChildren<UISprite>(true)[0];
        m_lblRule = FindTransform("TimeLimitActivityUIActivityInfoRule").GetComponentsInChildren<UILabel>(true)[0];
        m_lblInfoTitle = FindTransform("TimeLimitActivityUIActivityInfoTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_goHasReward = FindTransform("TimeLimitActivityUIActivityInfoHasReward").gameObject;

        for (int i = 0; i < 4; ++i)
        {
            m_arrItem[i] = FindTransform("TimeLimitActivityUIItem" + i).gameObject;
            m_arrItemGrid.Add(m_arrItem[i].AddComponent<InventoryGrid>());
        }

        m_goGetBtn = FindTransform("TimeLimitActivityUIGetBtn").gameObject;
        m_spGetBtnBGUp = FindTransform("TimeLimitActivityUIGetBtnBGUp").GetComponentsInChildren<UISprite>(true)[0];
        m_goShareBtn = FindTransform("TimeLimitActivityUIShareBtn").gameObject;
        m_goTimeLimitActivityUIItemListBG = FindTransform("TimeLimitActivityUIItemListBG").gameObject;

        Initialize();
    }  

    #region �¼�

    public int ActivityID;

    private void Initialize()
    {
        m_goGetBtn.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnGetBtnUp;
        m_goShareBtn.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += OnShareBtnUp;
    }

    void OnGetBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("GetBtn");
        TimeLimitActivityUILogicManager.Instance.GetActivityReward(ActivityID);
    }

    void OnShareBtnUp()
    {
        Mogo.Util.LoggerHelper.Debug("ShareBtn");
        TimeLimitActivityUILogicManager.Instance.ShareToGetDiamond(ActivityID);
    }  

    #endregion

    #region Grid��Ϣ  

    string m_strCDText;
    public string CDText
    {
        get { return m_strCDText; }
        set
        {
            m_strCDText = value;

            if (m_lblCDText != null)
            {
                m_lblCDText.text = m_strCDText;
                string[] temp = m_strCDText.Split(':');

                if (countDown != null)
                    countDown.Release();

                if (temp.Length == 4)
                {
                    countDown = new MogoCountDown(m_lblCDText, Int32.Parse(temp[0]), Int32.Parse(temp[1]), Int32.Parse(temp[2]), Int32.Parse(temp[3]), "", "�����", "����ʱ", MogoCountDown.TimeStringType.UpToDay);
                    countDown.SetSplitSign(String.Empty, String.Empty, LanguageData.GetContent(7100), LanguageData.GetContent(7101), LanguageData.GetContent(7102), LanguageData.GetContent(7103));
                }
            }
        }
    }

    string m_strInfoDesc;
    public string InfoDesc
    {
        get { return m_strInfoDesc; }
        set
        {
            m_strInfoDesc = value;

            if (m_lblInfoDesc != null)
            {
                m_lblInfoDesc.text = m_strInfoDesc;
            }
        }
    }

    string m_strInfoImgName;
    public string InfoImgName
    {
        get { return m_strInfoImgName; }
        set
        {
            m_strInfoImgName = value;

            if (m_spInfoImg != null)
            {
                m_spInfoImg.spriteName = m_strInfoImgName;
            }
        }
    }

    string m_strRule;
    public string Rule
    {
        get { return m_strRule; }
        set
        {
            m_strRule = value;

            if (m_lblRule != null)
            {
                m_lblRule.text = m_strRule;
            }
        }
    }

    string m_strInfoTitle;
    public string InfoTitle
    {
        get { return m_strInfoTitle; }
        set
        {
            m_strInfoTitle = value;

            if (m_lblInfoTitle != null)
            {
                m_lblInfoTitle.text = m_strInfoTitle;
            }
        }
    }

    public List<int> ListItemID
    {
        get
        {
            List<int> temp = new List<int>();
            foreach (var grid in m_arrItemGrid)
            {
                temp.Add(grid.iconID);
            }
            return temp;
        }
        set
        {
            int count = m_arrItemGrid.Count > value.Count ? value.Count : m_arrItemGrid.Count;
            ShowTimeLimitActivityUIItemListBG(count > 0 ? true : false);

            for (int i = 0; i < count; i++)
            {
                m_arrItemGrid[i].iconID = value[i];
            }

            if (m_arrItem[0] != null)
            {
                for (int i = 0; i < 4; ++i)
                {
                    if (i < count)
                    {
                        m_arrItem[i].SetActive(true);
                        UISprite spFG = m_arrItem[i].transform.Find("TimeLimitActivityUIItem" + i + "FG").GetComponentsInChildren<UISprite>(true)[0];
                        UISprite spBG = m_arrItem[i].transform.Find("TimeLimitActivityUIItem" + i + "BG").GetComponentsInChildren<UISprite>(true)[0];
                        InventoryManager.TrySetIcon(value[i], spFG, 0, null, spBG);

                        ItemParentData data = ItemParentData.GetItem(value[i]);
                        if (data != null && spFG != null)
                        {
                            MogoUtils.SetImageColor(spFG, data.color);
                        }
                    }
                    else
                    {
                        m_arrItem[i].SetActive(false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ���û״̬-���ݲ�ͬ״̬����UI
    /// </summary>
    /// <param name="status"></param>
    public void SetActivityStatus(ActivityStatus status)
    {
        switch (status)
        {
            case ActivityStatus.HasReward:
                {
                    m_lblCDText.effectStyle = UILabel.Effect.Outline;
                    m_lblCDText.effectColor = SystemUIColorManager.WHITE_OUTLINE;
                    m_lblCDText.color = SystemUIColorManager.WHITE;

                    m_lblInfoTitle.effectStyle = UILabel.Effect.Outline;
                    m_lblInfoTitle.effectColor = SystemUIColorManager.WHITE_OUTLINE;
                    m_lblInfoTitle.color = SystemUIColorManager.WHITE;

                    ShowHasRewardFlag(true);
                    ShowFinishedFX(false);

                    // ������ȡ��ť
                    m_goGetBtn.SetActive(false);
                    

                    // ���÷����ť
                    //m_goShareBtn.SetActive(false);
                }
                break;
            case ActivityStatus.HasFinished:
                {
                    m_lblCDText.effectStyle = UILabel.Effect.Outline;
                    m_lblCDText.effectColor = SystemUIColorManager.YELLOW_OUTLINE;
                    m_lblCDText.color = SystemUIColorManager.YELLOW;

                    m_lblInfoTitle.effectStyle = UILabel.Effect.Outline;
                    m_lblInfoTitle.effectColor = SystemUIColorManager.YELLOW_OUTLINE;
                    m_lblInfoTitle.color = SystemUIColorManager.YELLOW;

                    ShowHasRewardFlag(false);
                    ShowFinishedFX(true);

                    // ������ȡ��ť
                    m_goGetBtn.SetActive(true);
                    m_goGetBtn.GetComponentsInChildren<BoxCollider>(true)[0].enabled = true;
                    m_spGetBtnBGUp.spriteName = "btn_03up";

                    // ���÷����ť
                    //m_goShareBtn.SetActive(true);
                }
                break;
            case ActivityStatus.TimeUseUp:
                {
                    m_lblCDText.effectStyle = UILabel.Effect.Outline;
                    m_lblCDText.effectColor = SystemUIColorManager.WHITE_OUTLINE;
                    m_lblCDText.color = SystemUIColorManager.WHITE;

                    m_lblInfoTitle.effectStyle = UILabel.Effect.Outline;
                    m_lblInfoTitle.effectColor = SystemUIColorManager.WHITE_OUTLINE;
                    m_lblInfoTitle.color = SystemUIColorManager.WHITE;

                    ShowHasRewardFlag(false);
                    ShowFinishedFX(false);

                    // ������ȡ��ť
                    m_goGetBtn.SetActive(false);

                    // ���÷����ť
                    //m_goShareBtn.SetActive(false);
                }
                break;
            case ActivityStatus.OtherStatus:
                {
                    m_lblCDText.effectStyle = UILabel.Effect.None;
                    m_lblCDText.color = SystemUIColorManager.BROWN;

                    m_lblInfoTitle.effectStyle = UILabel.Effect.None;
                    m_lblInfoTitle.color = SystemUIColorManager.BROWN;

                    ShowHasRewardFlag(false);
                    ShowFinishedFX(false);

                    // ������ȡ��ť
                    m_goGetBtn.SetActive(true);
                    m_goGetBtn.GetComponentsInChildren<BoxCollider>(true)[0].enabled = false;
                    m_spGetBtnBGUp.spriteName = "btn_03grey";

                    // ���÷����ť
                    //m_goShareBtn.SetActive(true);
                }
                break;
            default:
                break;
        }
    }    
  
    // �Ƿ���ʾ"����ȡ"
    private void ShowHasRewardFlag(bool isShow)
    {
        m_goHasReward.SetActive(isShow);
    }

    // �Ƿ���ʾ��Ʒ�б������û����Ʒʱ����
    private void ShowTimeLimitActivityUIItemListBG(bool isShow)
    {
        m_goTimeLimitActivityUIItemListBG.SetActive(isShow);
    }

    #endregion

    #region ��ʱ�����ȡ��Ч

    /// <summary>
    /// ��ʱ�����ȡ��Ч
    /// </summary>
    /// <param name="isShow"></param>
    /// <returns></returns>
    private void ShowFinishedFX(bool isShow)
    {
        if (isShow)
        {
            AttachFXToTimeLimitActivityInfoUI(() =>
            {
                if (m_goFinishFX != null)
                    m_goFinishFX.SetActive(true);
            });
        }
        else
        {
            ReleaseFXFromTimeLimitActivityInfoUI();
        }
    }

    /// <summary>
    /// ������ʱ�����ȡ��Ч
    /// </summary>
    private GameObject m_goFinishFX = null;
    private void AttachFXToTimeLimitActivityInfoUI(Action action)
    {
        if (m_goFinishFX != null)
        {
            if (action != null)
                action();

            return;
        }

        if (TimeLimitActivityUIViewManager.Instance != null)
            TimeLimitActivityUIViewManager.Instance.AttachFXToTimeLimitActivityInfoUI(m_spInfoImg.transform, (goFinishFX) =>
            {
                m_goFinishFX = goFinishFX;

                if (action != null)
                    action();
            });
    }

    /// <summary>
    /// �ͷ���ʱ�����ȡ��Ч
    /// </summary>
    /// <param name="id"></param>
    public void ReleaseFXFromTimeLimitActivityInfoUI()
    {
        if (m_goFinishFX != null)
            AssetCacheMgr.ReleaseInstance(m_goFinishFX);
    }

    #endregion
}
