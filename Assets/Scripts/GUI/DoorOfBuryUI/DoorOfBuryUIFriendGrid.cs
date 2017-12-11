using UnityEngine;
using System.Collections;
using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class DoorOfBuryUIFriendGrid : MogoUIBehaviour
{
    private UISprite m_spFriendHeadImg;
    private UILabel m_lblFriendName;
    private UILabel m_lblBossName;
    private UILabel m_lblBossLevel;
    private UISprite m_fsBossHPFG;
    private UILabel m_lblBossHPText;
    private UILabel m_lblBattleInfo;
    private UILabel m_lblBattleTime;
    private UILabel m_lblWinText;
    private UILabel m_lblLoseText;
    private UISprite m_spKillImg;
    private UISprite m_spHelpImg;
    private UISprite m_spDoorOfBuryChooseFriendGridBtnBGUp;
    private Camera m_dragCamera;

    private GameObject m_goGODoorOfBuryChooseBattleING;
    private GameObject m_goGODoorOfBuryChooseBattleEND;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_spFriendHeadImg = FindTransform("DoorOfBuryChooseFriendHeadImg").GetComponentsInChildren<UISprite>(true)[0];
        m_lblFriendName = FindTransform("DoorOfBuryChooseFriendName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBossName = FindTransform("DoorOfBuryChooseBossName").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBossLevel = FindTransform("DoorOfBuryChooseBossLevel").GetComponentsInChildren<UILabel>(true)[0];
        m_fsBossHPFG = FindTransform("DoorOfBuryChooseBossHPFG").GetComponentsInChildren<UISprite>(true)[0];
        m_lblBossHPText = FindTransform("DoorOfBuryChooseBossHPText").GetComponentsInChildren<UILabel>(true)[0];
        m_bossHpProgress = FindTransform("DoorOfBuryChooseBossInfo").GetComponentsInChildren<MogoProgressBar>(true)[0];
        m_lblBattleInfo = FindTransform("DoorOfBuryChooseBattleInfo").GetComponentsInChildren<UILabel>(true)[0];
        m_lblBattleTime = FindTransform("DoorOfBuryChooseBattleTime").GetComponentsInChildren<UILabel>(true)[0];
        m_lblWinText = FindTransform("DoorOfBuryChooseWinText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblLoseText = FindTransform("DoorOfBuryChooseLoseText").GetComponentsInChildren<UILabel>(true)[0];
        m_spKillImg = FindTransform("DoorOfBuryChooseKillImg").GetComponentsInChildren<UISprite>(true)[0];
        m_spHelpImg = FindTransform("DoorOfBuryChooseHelpImg").GetComponentsInChildren<UISprite>(true)[0];
        m_spDoorOfBuryChooseFriendGridBtnBGUp = FindTransform("DoorOfBuryChooseFriendGridBtnBGUp").GetComponentsInChildren<UISprite>(true)[0];

        m_goGODoorOfBuryChooseBattleING = FindTransform("GODoorOfBuryChooseBattleING").gameObject;
        m_goGODoorOfBuryChooseBattleEND = FindTransform("GODoorOfBuryChooseBattleEND").gameObject;

        // ChineseData
        m_lblWinText.text = LanguageData.GetContent(47113);
        m_lblLoseText.text = LanguageData.GetContent(47114);
    }

    void Start()
    {
        SetStatus(IsLose, IsWin, IsKill, IsHelp);
    }

    #region �¼�

    public int Id = -1;

    void OnClick()
    {
        EventDispatcher.TriggerEvent<int>(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.FRIENDGRIDUP, Id);
    }

    #endregion    

    #region ���ý�����Ϣ

    string m_strFriendHeadImg;
    public string FriendHeadImg
    {
        get { return m_strFriendHeadImg; }
        set
        {
            m_strFriendHeadImg = value;

            if (m_spFriendHeadImg != null)
            {
                m_spFriendHeadImg.spriteName = value;
            }
        }
    }

    string m_strFriendName;
    public string FriendName
    {
        get { return m_strFriendName; }
        set
        {
            m_strFriendName = value;

            if (m_lblFriendName != null)
            {
                m_lblFriendName.text = value;
            }
        }
    }

    string m_strBossName;
    public string BossName
    {
        get { return m_strBossName; }
        set
        {
            m_strBossName = value;

            if (m_lblBossName != null)
            {
                m_lblBossName.text = value;
            }
        }
    }

    string m_strBossLevel;
    public string BossLevel
    {
        get { return m_strBossLevel; }
        set
        {
            m_strBossLevel = value;

            if (m_lblBossLevel != null)
            {
                m_lblBossLevel.text = value;
            }
        }
    }

    string m_strBossHP;
    public string BossHp
    {
        get { return m_strBossHP; }
        set
        {
            m_strBossHP = value;

            if (m_lblBossHPText != null)
            {
                //m_lblBossHPText.text = value + "%";
                //m_fsBossHPFG.fillAmount = int.Parse(m_strBossHP) * 0.01f;
                m_bossHpProgress.SetProgress(int.Parse(m_strBossHP), 100);
                m_bossHpProgress.SetProgressText(value + "%");
            }
        }
    }

    string m_strBattleInfo;
    public string BattleInfo
    {
        get { return m_strBattleInfo; }
        set
        {
            m_strBattleInfo = value;

            if (m_lblBattleInfo != null)
            {
                m_lblBattleInfo.text = value;
            }
        }
    }

    string m_strBattleTime;
    public string BattleTime
    {
        get { return m_strBattleTime; }
        set
        {
            m_strBattleTime = value;

            if (m_lblBattleTime != null)
            {
                m_lblBattleTime.text = value;
            }
        }
    }

    #endregion

    #region ս��״̬����

    /// <summary>
    /// ����״̬
    /// ״̬1��ʤ��(����ս��->����)
    /// ״̬2��ʧ��
    /// ״̬3��������
    /// </summary>
    /// <param name="isLose"></param>
    /// <param name="isWin"></param>
    /// <param name="isKill"></param>
    /// <param name="isHelp"></param>
    public void SetStatus(bool isLose, bool isWin, bool isKill, bool isHelp)
    {
        IsKill = isKill;
        IsLose = isLose;
        IsWin = isWin;
        IsHelp = isHelp;

        if (m_goGODoorOfBuryChooseBattleING != null && m_goGODoorOfBuryChooseBattleEND != null)
        {
            if (IsWin) // ״̬1��ʤ��(����ս��->����)
            {
                m_goGODoorOfBuryChooseBattleING.SetActive(false);
                m_goGODoorOfBuryChooseBattleEND.SetActive(true);
            }
            else if (IsLose) // ״̬2��ʧ��
            {
                m_goGODoorOfBuryChooseBattleING.SetActive(false);
                m_goGODoorOfBuryChooseBattleEND.SetActive(true);
            }
            else // ״̬3��������
            {
                m_goGODoorOfBuryChooseBattleING.SetActive(true);
                m_goGODoorOfBuryChooseBattleEND.SetActive(false);
            }
        }   
    }       

    /// <summary>
    /// ս��ʤ��
    /// </summary>
    bool m_bIsWin = false;
    private bool IsWin
    {
        get { return m_bIsWin; }
        set
        {
            m_bIsWin = value;

            if (m_lblWinText != null)
            {
                m_lblWinText.gameObject.SetActive(value);
            }
        }
    }

    /// <summary>
    /// ս��ʧ��
    /// </summary>
    bool m_bIsLose = false;
    private bool IsLose
    {
        get { return m_bIsLose; }
        set
        {
            m_bIsLose = value;

            if (m_lblLoseText != null)
                m_lblLoseText.gameObject.SetActive(value);

            // ս��ʧ�ܰѱ�������Ϊ��ɫ
            if (m_bIsLose && m_spFriendHeadImg != null && m_spDoorOfBuryChooseFriendGridBtnBGUp != null)
            {
                //m_spDoorOfBuryChooseFriendGridBtnBGUp.ShowAsWhiteBlack(true);
                m_spFriendHeadImg.ShowAsWhiteBlack(true);
            }
        }
    }

    bool m_bIsKill = false;
    private bool IsKill
    {
        get { return m_bIsKill; }
        set
        {
            m_bIsKill = value;

            if (m_spKillImg != null)
                m_spKillImg.gameObject.SetActive(value);
        }
    }

    bool m_bIsHelp = false;
    private bool IsHelp
    {
        get { return m_bIsHelp; }
        set
        {
            m_bIsHelp = value;

            if (m_spHelpImg != null)
                m_spHelpImg.gameObject.SetActive(value);
        }
    }

    #endregion

    #region CD

    public int hour = 2;
    public int minus = 0;  
    private uint timerId;
    private int counter = 0;
    private MogoProgressBar m_bossHpProgress;

    public void BeginCountDown()
    {
        Callback();
        timerId = TimerHeap.AddTimer(0, 1000 * 60, Callback);
        counter = hour * 60 + minus;
        //Debug.LogError(counter);
    }

    private void Callback()
    {
        if (--counter <= 0)
        {
            TimerHeap.DelTimer(timerId);
            counter = 0;
        }

        BattleTime = counter / 60 + " : " + counter % 60;
        //Debug.LogError(BattleTime);
        EventDispatcher.TriggerEvent<int>(DoorOfBuryUILogicManager.DoorOfBuryUIEvent.FRIENDGRIDCOUNTDOWNEND, Id);
    }

    #endregion
}
