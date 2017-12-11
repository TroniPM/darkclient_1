using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Mogo.Util;
using Mogo.GameData;
public class ArenaUIViewManager : MogoParentUI
{
    #region Private
    private Transform m_transform;
    private GameObject m_head;
    private GameObject m_name;
    private GameObject m_title;
    private GameObject m_progress;
    private GameObject m_nextTitle;
    private GameObject m_battleForce;
    private GameObject m_dayPoint;
    private GameObject m_weekPoint;
    public GameObject m_arenaCD;
    private UILabel m_lblArenaCDNum;
    private GameObject m_arenaTimes;
    private GameObject m_revengeBtn;
    private GameObject m_strongBtn;
    private GameObject m_weakBtn;
    private GameObject m_refreshBtn;
    private GameObject m_revengeBuffBtn;
    private GameObject m_goArenaUIBtnEnter;
    private GameObject m_goGOModelSuccesFlag;

    private GameObject m_modelShowCase;
    private UILabel m_lblPlayerDescTitle;
    private UILabel m_lblPlayerDescRange;
    private GameObject m_playerForce;
    private GameObject m_playerLevel;
    private GameObject m_playerName;
    private GameObject m_playerVocation;
    private GameObject m_playerMedalIcon;
    private GameObject m_playerMedalTitle;
    private Dictionary<int, ArenaButton> m_buttonList = new Dictionary<int, ArenaButton>();

    // ����ȷ�Ͽ�
    private GameObject m_goGOArenaUIUseOKCancel;
    private UILabel m_lblArenaUIUseInfoDiamondNum;
    private GameObject m_goArenaUIUseTipEnableBGDown;

    // PVP�ȼ�Tip
    private GameObject m_goGOArenaUIPVPLevelTip;
    private UILabel m_lblArenaUIPVPLevelTipInfoCurTitle;
    private UILabel m_lblArenaUIPVPLevelTipInfoCurPVPNum1;
    private UILabel m_lblArenaUIPVPLevelTipInfoCurPVPNum2;
    private UILabel m_lblArenaUIPVPLevelTipInfoNextTitle;
    private UILabel m_lblArenaUIPVPLevelTipInfoNextPVPNum1;
    private UILabel m_lblArenaUIPVPLevelTipInfoNextPVPNum2;

    // Gold and Diamond
    private UILabel m_lblArenaUIGoldNum;
    private UILabel m_lblArenaUIDiamondNum;
    private GameObject m_goGOBtnRewardNotice;    

    #endregion

    void Awake()
    {
        m_transform = transform;
        m_head = m_transform.Find("leftPanel/imgHead").gameObject;
        m_name = m_transform.Find("leftPanel/lblName").gameObject;
        m_title = m_transform.Find("leftPanel/lblTitle").gameObject;
        m_progress = m_transform.Find("leftPanel/GameObject/ProgressBar").gameObject;        
        m_nextTitle = m_transform.Find("leftPanel/GameObject/lblNextTitle").gameObject;
        m_battleForce = m_transform.Find("leftPanel/lblForce").gameObject;
        m_dayPoint = m_transform.Find("leftPanel/lblDay").gameObject;
        m_weekPoint = m_transform.Find("leftPanel/lblWeek").gameObject;
        m_arenaCD = m_transform.Find("leftPanel/btnClearCD").gameObject;
        m_lblArenaCDNum = m_transform.Find("leftPanel/btnClearCD/BtnClearCDNum").GetComponentsInChildren<UILabel>(true)[0];
        m_arenaTimes = m_transform.Find("leftPanel/btnAddTimes/text").gameObject;
        m_revengeBtn = m_transform.Find("EnemyList/Revenge").gameObject;
        m_buttonList[3] = m_revengeBtn.AddComponent<ArenaButton>();
        m_strongBtn = m_transform.Find("EnemyList/Strong").gameObject;
        m_buttonList[2] = m_strongBtn.AddComponent<ArenaButton>();
        m_weakBtn = m_transform.Find("EnemyList/Weak").gameObject;
        m_refreshBtn = m_transform.Find("btnRefresh").gameObject;
        m_revengeBuffBtn = m_transform.Find("btnRevenge").gameObject;
        m_goArenaUIBtnEnter = m_transform.Find("ArenaUIBtnEnter").gameObject;
        m_goGOModelSuccesFlag = m_transform.Find("GOModelSuccesFlag").gameObject;
        m_buttonList[1] = m_weakBtn.AddComponent<ArenaButton>();
        foreach (var item in m_buttonList)
        {
            item.Value.id = item.Key;
            var data = ArenaCreditReward4ChallengeData.dataMap.
                First(x => x.Value.type == item.Key &&
                    MogoWorld.thePlayer.arenicGrade >= x.Value.level[0]
                        && MogoWorld.thePlayer.arenicGrade <= x.Value.level[1]).Value;
            item.Value.transform.Find("RewardText").GetComponent<UILabel>().text = LanguageData.dataMap.Get(25034).Format(data.win[12], data.win[11]);
        }
        if (MogoUIManager.Instance.WaitingWidgetName == "Weak")
        {
            TimerHeap.AddTimer(100, 0, () => { EventDispatcher.TriggerEvent("WaitingWidgetFinished"); });
        }
        m_modelShowCase = m_transform.Find("ModelShowCase").gameObject;
        var sc = m_modelShowCase.AddComponent<ModelShowCase>();
        sc.left = m_transform.Find("Model/left");
        sc.right = m_transform.Find("Model/right");
        //sc.LoadCreateCharacter();
        m_lblPlayerDescTitle = m_transform.Find("player/PlayerDescTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_lblPlayerDescRange = m_transform.Find("player/PlayerDescRange").GetComponentsInChildren<UILabel>(true)[0];
        m_playerForce = m_transform.Find("player/playerForce").gameObject;
        m_playerLevel = m_transform.Find("player/playerLevel").gameObject;
        m_playerName = m_transform.Find("player/playerName").gameObject;
        m_playerVocation = m_transform.Find("player/playerVocation").gameObject;
        m_playerMedalIcon = m_transform.Find("player/MedalGO/MedalIcon").gameObject;
        m_playerMedalTitle = m_transform.Find("player/MedalGO/MedalTitle").gameObject;

        m_goGOArenaUIUseOKCancel = m_transform.Find("GOArenaUIUseOKCancel").gameObject;
        m_lblArenaUIUseInfoDiamondNum = m_transform.Find("GOArenaUIUseOKCancel/GOArenaUIUseInfo/ArenaUIUseInfoDiamondNum").GetComponentsInChildren<UILabel>(true)[0];
        m_goArenaUIUseTipEnableBGDown = m_transform.Find("GOArenaUIUseOKCancel/GOArenaUIUseTipEnable/ArenaUIUseTipEnable/ArenaUIUseTipEnableBGDown").gameObject;
        IsShowArenaRevengeTipDialog = SystemConfig.Instance.IsShowArenaRevengeTipDialog;

        m_goGOArenaUIPVPLevelTip = m_transform.Find("GOArenaUIPVPLevelTip").gameObject;
        m_lblArenaUIPVPLevelTipInfoCurTitle = m_transform.Find("GOArenaUIPVPLevelTip/GOArenaUIPVPLevelTipInfo/ArenaUIPVPLevelTipInfoCurTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIPVPLevelTipInfoCurPVPNum1 = m_transform.Find("GOArenaUIPVPLevelTip/GOArenaUIPVPLevelTipInfo/ArenaUIPVPLevelTipInfoCurPVPNum1").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIPVPLevelTipInfoCurPVPNum2 = m_transform.Find("GOArenaUIPVPLevelTip/GOArenaUIPVPLevelTipInfo/ArenaUIPVPLevelTipInfoCurPVPNum2").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIPVPLevelTipInfoNextTitle = m_transform.Find("GOArenaUIPVPLevelTip/GOArenaUIPVPLevelTipInfo/ArenaUIPVPLevelTipInfoNextTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIPVPLevelTipInfoNextPVPNum1 = m_transform.Find("GOArenaUIPVPLevelTip/GOArenaUIPVPLevelTipInfo/ArenaUIPVPLevelTipInfoNextPVPNum1").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIPVPLevelTipInfoNextPVPNum2 = m_transform.Find("GOArenaUIPVPLevelTip/GOArenaUIPVPLevelTipInfo/ArenaUIPVPLevelTipInfoNextPVPNum2").GetComponentsInChildren<UILabel>(true)[0];

        m_lblArenaUIGoldNum = m_transform.Find("leftPanel/GOArenaUIGold/ArenaUIGoldNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblArenaUIDiamondNum = m_transform.Find("leftPanel/GOArenaUIDiamond/ArenaUIDiamondNum").GetComponentsInChildren<UILabel>(true)[0];

        m_goGOBtnRewardNotice = m_transform.Find("leftPanel/GOBtnRewardNotice").gameObject;

        // ChineseData
        m_transform.Find("leftPanel/GameObject/lblCurTitle").GetComponentsInChildren<UILabel>(true)[0].text
            = LanguageData.GetContent(46003);

        AddButtonListener("OnClicked", "btnClose", ArenaUILogicManager.Instance.OnClose);
        AddButtonListener("OnClicked", "ArenaUIBtnEnter", ArenaUILogicManager.Instance.OnEnter);
        AddButtonListener("OnClicked", "btnRefresh", ArenaUILogicManager.Instance.OnRefresh);
        AddButtonListener("OnClicked", "btnRevenge", ArenaUILogicManager.Instance.OnRevenge);
        AddButtonListener("OnClicked", "leftPanel/btnAddTimes", ArenaUILogicManager.Instance.OnAddTimes);
        AddButtonListener("OnClicked", "leftPanel/btnClearCD", ArenaUILogicManager.Instance.OnClearCD);
        AddButtonListener("OnClicked", "leftPanel/btnReward", ArenaUILogicManager.Instance.OnReward);
        AddButtonListener("OnClicked", "GOArenaUIUseOKCancel/ArenaUIUseCancelBtn", ArenaUILogicManager.Instance.OnUseCancel);
        AddButtonListener("OnClicked", "GOArenaUIUseOKCancel/ArenaUIUseOKBtn", ArenaUILogicManager.Instance.OnUseOK);
        AddButtonListener("OnClicked", "GOArenaUIUseOKCancel/GOArenaUIUseTipEnable/ArenaUIUseTipEnable", ArenaUILogicManager.Instance.OnUseTipEnable);
        AddButtonListener("OnClicked", "leftPanel/ShwoPVPLevelTipBtn", ArenaUILogicManager.Instance.OnShwoPVPLevelTip);
        AddButtonListener("OnClicked", "GOArenaUIPVPLevelTip/ArenaUIPVPLevelTipCloseBtn", ArenaUILogicManager.Instance.OnClosePVPLevelTip);
        
        ArenaUILogicManager.Instance.Initialize(this);
        gameObject.SetActive(false);
    }

    public void HandleTabChange(int fromPage, int toPage)
    {
        if (toPage == 3)
        {
            m_refreshBtn.SetActive(false);
            m_revengeBuffBtn.SetActive(true);
        }
        else
        {
            m_refreshBtn.SetActive(true);
            m_revengeBuffBtn.SetActive(false);
            m_goArenaUIBtnEnter.SetActive(true);
            m_goGOModelSuccesFlag.SetActive(false);
        }
        m_buttonList[fromPage].transform.Find("RewardText").GetComponent<UILabel>().color = new Color32(143, 143, 143, 255);
        m_buttonList[fromPage].transform.Find("EnemyTypeText").GetComponent<UILabel>().color = new Color32(102, 85, 72, 255);
        m_buttonList[toPage].transform.Find("RewardText").GetComponent<UILabel>().color = new Color32(255, 255, 255, 255);
        m_buttonList[toPage].transform.Find("EnemyTypeText").GetComponent<UILabel>().color = new Color32(126, 49, 0, 255);
    }

    /// <summary>
    /// �Ƿ���ʾ��ȡ������ʾ
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowRewardNotice(bool isShow)
    {
        m_goGOBtnRewardNotice.SetActive(isShow);
    }    

    /// <summary>
    /// ���ý������
    /// </summary>
    /// <param name="gold"></param>
    public void SetGoldNum(uint gold)
    {
        if (m_lblArenaUIGoldNum != null)
            m_lblArenaUIGoldNum.text = gold.ToString();
    }

    /// <summary>
    /// ������ʯ����
    /// </summary>
    /// <param name="diamond"></param>
    public void SetDiamondNum(uint diamond)
    {
        if (m_lblArenaUIDiamondNum != null)
            m_lblArenaUIDiamondNum.text = diamond.ToString();
    }

    public void SetHeadImage(ushort level)
    {
        LoggerHelper.Debug(ArenaLevelData.GetCurTitleIcon(level));
        m_head.GetComponent<UISlicedSprite>().spriteName = ArenaLevelData.GetCurTitleIcon(level);
        if (m_head.GetComponent<UISlicedSprite>().atlas.GetSprite(ArenaLevelData.GetCurTitleIcon(level)) == null)
            m_head.GetComponent<UISlicedSprite>().spriteName = "gradeicon00";
    }

    public void SetNameText(String text)
    {
        m_name.GetComponent<UILabel>().text = text;
    }

    public void SetProgressValue(uint cur)
    {
        int next = ArenaLevelData.GetNextLevelCreditNeed(MogoWorld.thePlayer.arenicGrade, (int)cur);
        m_progress.GetComponent<UISlider>().sliderValue = ((float)cur) / next;
        m_progress.GetComponentsInChildren<UILabel>(true)[0].text = String.Concat(cur, '/', next);
    }

    public void SetCurTitleText(ushort level)
    {
        m_title.GetComponent<UILabel>().text = ArenaLevelData.GetCurTitle(level);
    }

    public void SetNextTitleText(ushort level)
    {
        m_nextTitle.GetComponent<UILabel>().text = ArenaLevelData.GetNextTitle(level);
    }

    public void SetBattleForceText(uint text)
    {
        m_battleForce.GetComponent<UILabel>().text = text.ToString();
    }

    public void SetDayPointText(int text)
    {
        m_dayPoint.GetComponent<UILabel>().text = text.ToString();
    }

    public void SetWeekPointText(int text)
    {
        m_weekPoint.GetComponent<UILabel>().text = text.ToString();
    }

    public void SetArenaCDText(uint cdTime)
    {
        if (cdTime > 0)
        {
            m_arenaCD.SetActive(true);
        }
        TimerManager.GetTimer(m_arenaCD).StartTimer(cdTime,
            (sec) => 
            { 
                uint  min = (sec % 3600) / 60;
                uint second = (sec % 3600) % 60;
                m_lblArenaCDNum.text = String.Concat(min.ToString("d2"), ":", second.ToString("d2")); 
            },
            () => 
            { 
                m_arenaCD.SetActive(false); 
            });
    }

    public void SetArenaTimesText(String text)
    {
        m_arenaTimes.GetComponent<UILabel>().text = text;
    }

    public void SetBuffText(int atk, int hp)
    {
        m_revengeBuffBtn.transform.Find("buff").GetComponentsInChildren<UILabel>(true)[0].text = atk.ToString() + hp.ToString();
    }

    /// <summary>
    /// ���ñ���ս�������
    /// </summary>
    /// <param name="data"></param>
    /// <param name="level"></param>
    public void SetArenaPlayerData(ArenaPlayerData data, int tab)
    {
        m_playerForce.GetComponent<UILabel>().text = LanguageData.dataMap.Get(25008).Format(data.fightForce);
        m_playerLevel.GetComponent<UILabel>().text = "LV" + data.level.ToString();
        m_playerName.GetComponent<UILabel>().text = data.name;
        m_playerVocation.GetComponent<UILabel>().text = LanguageData.dataMap.Get(data.vocation).content;
        SetArenaPlayerMedal(data.arenaLevel);
    }

    /// <summary>
    /// ����ˢ�»����
    /// </summary>
    /// <param name="price"></param>
    /// <param name="level"></param>
    public void SetRefreshPrice(int price, int level)
    {
        if (level != 3)
        {
            m_refreshBtn.transform.Find("num").GetComponentsInChildren<UILabel>(true)[0].text = "X" + price.ToString();
        }
        else
        {
            m_revengeBuffBtn.transform.Find("num").GetComponentsInChildren<UILabel>(true)[0].text = "X" + price.ToString();
        }
    }

    public void SetArenaDesc(int min, int max, int tab)
    {
        m_lblPlayerDescTitle.text = LanguageData.dataMap.Get(46752 + tab).content;
        m_lblPlayerDescRange.text =LanguageData.dataMap.Get(46756).Format(min, max);   
    }

    public void SetArenaDesc(string text)
    {
        m_lblPlayerDescTitle.text = text;
        m_lblPlayerDescRange.text = "";
    }

    /// <summary>
    /// ���ñ���ս���ѫ�ºͳƺ�
    /// </summary>
    /// <param name="level"></param>
    void SetArenaPlayerMedal(int level)
    {
        m_playerMedalIcon.GetComponent<UISlicedSprite>().spriteName = ArenaLevelData.GetCurTitleIcon(level);
        if (m_playerMedalIcon.GetComponent<UISlicedSprite>().atlas.GetSprite(ArenaLevelData.GetCurTitleIcon(level)) == null)
            m_playerMedalIcon.GetComponent<UISlicedSprite>().spriteName = "gradeicon00";
        m_playerMedalTitle.GetComponent<UILabel>().text = ArenaLevelData.GetCurTitle(level);
    }

    /// <summary>
    /// ���þ���������Լ�������
    /// </summary>
    /// <param name="data"></param>
    public void SetArenaPersonalData(ArenaPersonalData data)
    {
        SetDayPointText(data.dayScore);
        SetWeekPointText(data.weekScore);
        SetArenaTimesText(data.challengeTimes.ToString());
        SetArenaCDText((uint)data.cd);
    }

    #region �������з�ģ��

    public void SetModelShow(int vocation, List<int> weaponList, bool isShow)
    {
        m_modelShowCase.GetComponent<ModelShowCase>().LoadCreateCharacter(vocation, weaponList,
            () =>
            {
                ModelShowCaseLogicManager.Instance.ShowModel(vocation, isShow);
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            });
    }

    public void CloseAllModel()
    {
          ModelShowCaseLogicManager.Instance.CloseAllModel();
    }

    #endregion

    #region ����

    bool m_IsShowArenaRevengeTipDialog;
    public bool IsShowArenaRevengeTipDialog
    {
        get
        {
            return m_IsShowArenaRevengeTipDialog;
        }
        set
        {
            m_IsShowArenaRevengeTipDialog = value;
            if (m_IsShowArenaRevengeTipDialog == false)
            {
                // �ж��Ƿ���Ҫ����(ÿ���һ�ε�½����Ϊtrue)
                if (MogoTime.Instance.GetCurrentDateTime().Day != Mogo.Util.SystemConfig.Instance.ArenaRevengeTipDialogDisableDay)
                {
                    m_IsShowArenaRevengeTipDialog = true;
                    Mogo.Util.SystemConfig.Instance.IsShowArenaRevengeTipDialog = true;
                    Mogo.Util.SystemConfig.Instance.ArenaRevengeTipDialogDisableDay = MogoTime.Instance.GetCurrentDateTime().Day;
                    Mogo.Util.SystemConfig.SaveConfig();
                }
            }
            OnUseTipClick();
        }
    }

    /// <summary>
    /// ��ʾ/���ع���ȷ�Ͽ�
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowRevengeOKCancelBox(bool isShow)
    {
        m_goGOArenaUIUseOKCancel.SetActive(isShow);
    }

    /// <summary>
    /// ���ù�����Ҫ����ʯ
    /// </summary>
    /// <param name="num"></param>
    public void SetArenaUIUseInfoDiamondNum(int num)
    {
        m_lblArenaUIUseInfoDiamondNum.text = "x" + num;
    }

    /// <summary>
    /// �����Ƿ��Ժ���ʾ
    /// </summary>
    void OnUseTipClick()
    {
        if (IsShowArenaRevengeTipDialog)
            m_goArenaUIUseTipEnableBGDown.SetActive(false);
        else
            m_goArenaUIUseTipEnableBGDown.SetActive(true);
    }

    #endregion

    #region PVP�ȼ�Tip

    /// <summary>
    /// ��ʾ/����PVP�ȼ�Tip
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowPVPLevelTip(bool isShow)
    {
        m_goGOArenaUIPVPLevelTip.SetActive(isShow);
    }

    /// <summary>
    /// ���õ�ǰTitle
    /// </summary>
    /// <param name="title"></param>
    public void SetPVPLevelTipInfoCurTitle(string title)
    {
        m_lblArenaUIPVPLevelTipInfoCurTitle.text = title;
    }

    /// <summary>
    /// ���õ�ǰPVPǿ��
    /// </summary>
    /// <param name="num"></param>
    public void SetPVPLevelTipInfoCurPVPNum1(int num)
    {
        m_lblArenaUIPVPLevelTipInfoCurPVPNum1.text = "+" + num;
    }

    /// <summary>
    /// ���õ�ǰPVP����
    /// </summary>
    /// <param name="num"></param>
    public void SetPVPLevelTipInfoCurPVPNum2(int num)
    {
        m_lblArenaUIPVPLevelTipInfoCurPVPNum2.text = "+" + num;
    }

    /// <summary>
    /// �����¼�Title
    /// </summary>
    /// <param name="title"></param>
    public void SetPVPLevelTipInfoNextTitle(string title)
    {
        m_lblArenaUIPVPLevelTipInfoNextTitle.text = title;
    }

    /// <summary>
    /// �����¼�PVPǿ��
    /// </summary>
    /// <param name="num"></param>
    public void SetPVPLevelTipInfoNextPVPNum1(int num)
    {
        m_lblArenaUIPVPLevelTipInfoNextPVPNum1.text = "+" + num;
    }

    /// <summary>
    /// �����¼�PVP����
    /// </summary>
    /// <param name="num"></param>
    public void SetPVPLevelTipInfoNextPVPNum2(int num)
    {
        m_lblArenaUIPVPLevelTipInfoNextPVPNum2.text = "+" + num;
    }

    #endregion

    /// <summary>
    /// �����б����ܺ��ɫģ�������ӡ��ѻ��ܡ�ͬʱ���� �����·��ġ�������衱�͡���ս����ť
    /// </summary>
    /// <param name="hasRevenged"></param>
    public void SetRevengeInfo(bool hasRevengeSuccessed = false)
    {
        if (hasRevengeSuccessed)
        {
            m_revengeBuffBtn.SetActive(false);
            m_goArenaUIBtnEnter.SetActive(false);
            m_goGOModelSuccesFlag.SetActive(true);
        }
        else
        {
            m_revengeBuffBtn.SetActive(true);
            m_goArenaUIBtnEnter.SetActive(true);
            m_goGOModelSuccesFlag.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (MogoWorld.thePlayer != null)
        {
            SetGoldNum(MogoWorld.thePlayer.gold);
            SetDiamondNum(MogoWorld.thePlayer.diamond);
        }
    }
}
