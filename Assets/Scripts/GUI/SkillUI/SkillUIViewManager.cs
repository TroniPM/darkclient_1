using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Mogo.Util;
using Mogo.GameData;

public class SkillUIViewManager : MogoUIBehaviour 
{
    private static SkillUIViewManager m_instance;
    public static SkillUIViewManager Instance { get { return SkillUIViewManager.m_instance; } }

    private SkillUIIconGrid[] m_arrSkillIconGrid = new SkillUIIconGrid[5];

    private int m_iCurrentGridID = 0;
    private int m_iCurrentInfoID = 0;

    private UILabel m_lblSkillName;
    private UILabel m_lblSkillNeedLevel;
    private UILabel m_lblSkillDescripe;
    private UILabel m_lblSkillLearnCostGold;
    private UILabel m_lblSkillLearnCostHorner;
    private UILabel m_lblSkillUIGold;
    private UILabel m_lblSkillUIHorner;
    private UILabel m_lblSkillExtraDamage;
    private UILabel m_lblSkillExtraDamageNum;
    private UILabel m_lblSkillDamageRace;
    private UILabel m_lblSkillDamageRaceNum;

    public Action<int> LEARNBTNUP;
    public Action<int> SKILLICONGRIDUP;
    public Action<int> SKILLINFOICONUP;
    public Action<int> SWITCHICONUP;
    public Action<int> WEAPONUP;

    private MogoTwoStatusButton m_learnBtn;

    private GameObject m_goSkillInfoList;

    private UILabel[] m_arrLblSkillInfoIconName = new UILabel[3];

    private UISlicedSprite[] m_arrSSSkillInfoActive = new UISlicedSprite[3];
    private UISlicedSprite[] m_arrSSSkillInfoLock = new UISlicedSprite[3];
    private UISlicedSprite[] m_arrSSSkillInfoFG = new UISlicedSprite[3];

    private UISprite m_spSwitchIcon;

    private UISprite m_spRefreshCtrl;

    private MogoSingleButton m_mtbWeapon0;
    private MogoSingleButton m_mtbWeapon1;

    private UILabel m_lblSkillDialogInfoNeedLevelText;
    private UILabel m_lblSkillDialogInfoLearnCostText;


    #region �¼�

    void OnSkillIconGridUp(int id)
    {
        if (id == m_iCurrentGridID)
            return;

        for(int i = 0;i < 5;++i)
        {
            if (i == id)
            {
                m_arrSkillIconGrid[i].SetIconGridDown(true);
                m_iCurrentGridID = i;
            }
            else
            {
                m_arrSkillIconGrid[i].SetIconGridDown(false);
            }
        }


        if (SKILLICONGRIDUP != null)
            SKILLICONGRIDUP(id);
    }

    void OnSkillUILearnUp(int i)
    {
        if (LEARNBTNUP != null)
            LEARNBTNUP(i);
    }

    void OnSwitchIconUp(int i)
    {
        if (SWITCHICONUP != null)
            SWITCHICONUP(i);
    }

    void OnWeapon0Up(int i)
    {
        if (WEAPONUP != null)
            WEAPONUP(0);
    }

    void OnWeapon1Up(int i)
    {
        if (WEAPONUP != null)
            WEAPONUP(1);
    }

    void OnSkillDialogInfoIconUp(int id)
    {
        //if (m_iCurrentInfoID == id)
        //    return;

        m_iCurrentInfoID = id;

        if (SKILLINFOICONUP != null)
            SKILLINFOICONUP(id);
    }

    #endregion

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);       

        Initialize();

        for (int i = 0; i < 5; ++i)
        {
            //m_arrSkillIconGrid[i] = m_myTransform.FindChild(m_widgetToFullName["SkillIcon" + i]).GetComponentsInChildren<SkillUIIconGrid>(true)[0];
            m_arrSkillIconGrid[i] = m_myTransform.Find(m_widgetToFullName["SkillIcon" + i]).gameObject.AddComponent<SkillUIIconGrid>();
            m_arrSkillIconGrid[i].ID = i;

            Mogo.Util.LoggerHelper.Debug("Run Herererererer");
        }

        for (int i = 0; i < 3; ++i)
        {
            m_arrLblSkillInfoIconName[i] = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoIcon" + i + "Name"]).GetComponentsInChildren<UILabel>(true)[0];
            m_arrSSSkillInfoActive[i] = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoIcon" + i + "Active"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
            m_arrSSSkillInfoLock[i] = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoIcon" + i + "Lock"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
            m_arrSSSkillInfoFG[i] = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoIcon" + i + "FG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        }

        m_lblSkillName = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillNeedLevel = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoNeedLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillDescripe = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoDescripeText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillLearnCostGold = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoLearnCostGlodText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillLearnCostHorner = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoLearnCostHornerText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillUIGold = m_myTransform.Find(m_widgetToFullName["SkillDialogGoldInfoNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillUIHorner = m_myTransform.Find(m_widgetToFullName["SkillDialogHornerInfoNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillDamageRace = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoDamageRaceText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillDamageRaceNum = FindTransform("SkillDialogInfoDamageRaceNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillExtraDamage = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoExtraDamageText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillExtraDamageNum = FindTransform("SkillDialogInfoExtraDamageNum").GetComponentsInChildren<UILabel>(true)[0];

        m_spSwitchIcon = m_myTransform.Find(m_widgetToFullName["SkillIcon4Switch"]).GetComponentsInChildren<UISprite>(true)[0];

        m_learnBtn = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoLearn"]).GetComponentsInChildren<MogoTwoStatusButton>(true)[0];

        m_goSkillInfoList = m_myTransform.Find(m_widgetToFullName["SkillDialogInfoIconList"]).gameObject;

        m_spRefreshCtrl = m_myTransform.Find(m_widgetToFullName["SkillUIRefreshCtrl"]).GetComponentsInChildren<UISprite>(true)[0];

        m_mtbWeapon0 = m_myTransform.Find(m_widgetToFullName["SkillDialogPageWeapon0"]).GetComponent<MogoSingleButton>();
        m_mtbWeapon1 = m_myTransform.Find(m_widgetToFullName["SkillDialogPageWeapon1"]).GetComponent<MogoSingleButton>();

        m_spRefreshCtrl.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_lblSkillDialogInfoNeedLevelText = FindTransform("SkillDialogInfoNeedLevelText").GetComponentsInChildren<UILabel>(true)[0];
        m_lblSkillDialogInfoLearnCostText = FindTransform("SkillDialogInfoLearnCostText").GetComponentsInChildren<UILabel>(true)[0];
    }

    void Start()
    {
        if (m_weapon0Text != "")
        {
            SetWeapon0PageText(m_weapon0Text);
        }

        if (m_weapon1Text != "")
        {
            SetWeapon1PageText(m_weapon1Text);
        }
    }

    #region �¼�

    void Initialize()
    {
        SkillUILogicManager.Instance.Initialize();

        for (int i = 0; i < 5; ++i)
        {
            SkillUIDict.ButtonTypeToEventUp.Add("SkillIcon" + i,OnSkillIconGridUp);
        }

        for (int i = 0; i < 3; ++i)
        {
            SkillUIDict.ButtonTypeToEventUp.Add("SkillDialogInfoIcon" + i, OnSkillDialogInfoIconUp);
        }

        SkillUIDict.ButtonTypeToEventUp.Add("SkillDialogInfoLearn", OnSkillUILearnUp);
        SkillUIDict.ButtonTypeToEventUp.Add("SkillIcon3Switch", OnSwitchIconUp);
        SkillUIDict.ButtonTypeToEventUp.Add("SkillDialogPageWeapon0", OnWeapon0Up);
        SkillUIDict.ButtonTypeToEventUp.Add("SkillDialogPageWeapon1", OnWeapon1Up);

    }

    public void Release()
    {
        SkillUILogicManager.Instance.Release();

        SkillUIDict.ButtonTypeToEventUp.Clear();
    }

    #endregion

    #region ������Ϣ

    /// <summary>
    /// ��Ҫ�ȼ�
    /// </summary>
    private void SetSkillDialogInfoNeedLevelText(bool isEnough = true)
    {
        if (isEnough)
        {
            m_lblSkillDialogInfoNeedLevelText.text = LanguageData.GetContent(46006);
            m_lblSkillDialogInfoNeedLevelText.effectStyle = UILabel.Effect.None;
        }
        else
        {
            m_lblSkillDialogInfoNeedLevelText.text = MogoUtils.GetRedString(LanguageData.GetContent(46006));
            m_lblSkillDialogInfoNeedLevelText.effectStyle = UILabel.Effect.Outline;
            m_lblSkillDialogInfoNeedLevelText.effectColor = new Color32(50, 39, 9, 255);
        }
    }

    /// <summary>
    /// ������Ҫ����ҵȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetSkillNeedLevel(int level)
    {
        if (MogoWorld.thePlayer.level >= level)
        {
            SetSkillDialogInfoNeedLevelText(true);

            m_lblSkillNeedLevel.text = level.ToString();
            m_lblSkillNeedLevel.effectStyle = UILabel.Effect.None;
        }
        else
        {
            SetSkillDialogInfoNeedLevelText(false);

            m_lblSkillNeedLevel.text = MogoUtils.GetRedString(level.ToString());
            m_lblSkillNeedLevel.effectStyle = UILabel.Effect.Outline;
            m_lblSkillNeedLevel.effectColor = new Color32(50, 39, 9, 255);
        }
    }
    

    public void SetSkillName(string name)
    {
        m_lblSkillName.text = name;
    }    
 
    public void SetLearnBtnText(string text)
    {
        m_learnBtn.gameObject.GetComponentsInChildren<UILabel>(true)[0].text = text;
    }

    public void SetSkillDescripe(string descripe)
    {
        m_lblSkillDescripe.text = descripe;
    }

    public void SetSkillDamageRace(string raceTitle, string raceNum)
    {        
        m_lblSkillDamageRace.text = raceTitle;
        m_lblSkillDamageRaceNum.text = raceNum;
    }

    public void SetSkillExtraDamage(string damageTitle, string damageNum)
    {
        m_lblSkillExtraDamage.text = damageTitle;
        m_lblSkillExtraDamageNum.text = damageNum;
    }

    /// <summary>
    /// ������Ҫ���ĵĽ��
    /// </summary>
    /// <param name="gold"></param>
    public void SetSkillLearnCostGold(int gold)
    {
        if (MogoWorld.thePlayer.gold >= gold)
        {
            SetSkillDialogInfoLearnCostText(true);

            m_lblSkillLearnCostGold.text = gold.ToString();
            m_lblSkillLearnCostGold.effectStyle = UILabel.Effect.Outline;
            m_lblSkillLearnCostGold.effectColor = new Color32(13, 52, 0, 255);
            m_lblSkillLearnCostGold.color = new Color32(96, 254, 0, 255);
        }
        else
        {
            SetSkillDialogInfoLearnCostText(false);

            m_lblSkillLearnCostGold.text = MogoUtils.GetRedString(gold.ToString());
            m_lblSkillLearnCostGold.effectStyle = UILabel.Effect.Outline;
            m_lblSkillLearnCostGold.effectColor = new Color32(50, 39, 9, 255);
        }
    }

    /// <summary>
    /// ��Ҫ����
    /// </summary>
    /// <param name="isEnough"></param>
    private void SetSkillDialogInfoLearnCostText(bool isEnough)
    {
        if (isEnough)
        {
            m_lblSkillDialogInfoLearnCostText.text = LanguageData.GetContent(46007);
            m_lblSkillDialogInfoLearnCostText.effectStyle = UILabel.Effect.None;          
        }
        else
        {
            m_lblSkillDialogInfoLearnCostText.text = MogoUtils.GetRedString(LanguageData.GetContent(46007));
            m_lblSkillDialogInfoLearnCostText.effectStyle = UILabel.Effect.Outline;
            m_lblSkillDialogInfoLearnCostText.effectColor = new Color32(50, 39, 9, 255);
        }
    }

    public void SetSkillLearnCostHorner(int horner)
    {
        m_lblSkillLearnCostHorner.text = horner.ToString();
    }

    public void SetSkillUIGold(string gold)
    {
        m_lblSkillUIGold.text = gold;
    }

    public void SetSkillUIHorner(string horner)
    {
        m_lblSkillUIHorner.text = horner;
    }

    public void SetSkillInfoIconName(string name, int id)
    {
        m_arrLblSkillInfoIconName[id].text = name;
    }

    public void SetSkillInfoIconActive(bool isActive, int id)
    {
        //m_arrSSSkillInfoActive[id].gameObject.SetActive(isActive);

        m_arrSSSkillInfoActive[id].ShowAsWhiteBlack(!isActive);
        m_arrSSSkillInfoFG[id].ShowAsWhiteBlack(!isActive);
    }

    public void ShowSkillInfoIconLock(bool isShow, int id)
    {
        m_arrSSSkillInfoLock[id].gameObject.SetActive(isShow);
    }

    public void SetSkillGridActive(bool isActive, int id)
    {
        m_arrSkillIconGrid[id].SetSkillGridActive(isActive);
    }

    public void SetSkillGridIcon(string imgName, int id)
    {
        m_arrSkillIconGrid[id].SetSkillGridIcon(imgName);
    }

    public int GetCurrentSkillGrid()
    {
        return m_iCurrentGridID;
    }

    public void SetSkillInfoGridIcon(string imgName, int id)
    {
        m_arrSSSkillInfoActive[id].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_arrSSSkillInfoActive[id].spriteName = imgName;

        m_arrSSSkillInfoFG[id].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_arrSSSkillInfoFG[id].spriteName = "bb_daojugeguangzhe";
    }

    public int GetCurrentInfoGrid()
    {
        return m_iCurrentInfoID;
    }

    public void SetLearnBtnEnable(bool isEnable)
    {
        m_learnBtn.SetButtonEnable(isEnable);
    }

    public void ShowSwitchIcon(bool isShow)
    {
        m_spSwitchIcon.gameObject.SetActive(isShow);
    }

    public void SetCurrentDialogInfoIcon(int id)
    {
        m_goSkillInfoList.GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(id);
    }

    public void SetCurrentSkillGridIcon(int id)
    {
        if (id == m_iCurrentGridID)
            return;
        for (int i = 0; i < 5; ++i)
        {
            if (i == id)
            {
                m_arrSkillIconGrid[i].SetIconGridDown(true);
                m_iCurrentGridID = i;
            }
            else
            {
                m_arrSkillIconGrid[i].SetIconGridDown(false);
            }
        }
    }

    public void ShowSkillCanLearnAnim(bool isShow, int id)
    {
        if (isShow)
        {
            if (MogoUIManager.Instance.m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].m_iCurrentId != 2 || MogoUIManager.Instance.CurrentUI != MogoUIManager.Instance.m_MenuUI)
                return;
            //MogoFXManager.Instance.AttachUIFX(id + 9, MogoUIManager.Instance.GetMainUICamera(),4f,-118.5f);
            MogoFXManager.Instance.AttachUIFX(9, MogoUIManager.Instance.GetMainUICamera(), 4f, -118.5f, 0, m_arrSkillIconGrid[id].gameObject, "SkillCanLearn" + id);
        }
        else
        {
            //MogoFXManager.Instance.DetachUIFX(9);
            MogoFXManager.Instance.ReleaseParticleAnim("SkillCanLearn" + id);
        }
    }

    public void ShowSkillOpenAnim(bool isShow, int id)
    {
        if (isShow)
        {
            if (MogoUIManager.Instance.m_MenuUI.transform.Find("PropSheet").GetComponentsInChildren<MogoPropSheet>(true)[0].m_iCurrentId != 2 || MogoUIManager.Instance.CurrentUI != MogoUIManager.Instance.m_MenuUI)
                return;
            //MogoFXManager.Instance.AttachUIFX(id + 9, MogoUIManager.Instance.GetMainUICamera(),4f,-118.5f);
            MogoFXManager.Instance.AttachUIFX(11, MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, m_arrSkillIconGrid[id].gameObject);
        }
        else
        {
            MogoFXManager.Instance.DetachUIFX(11);
        }
    }

    string m_weapon0Text;
    string m_weapon1Text;

    public void SetWeapon0PageText(string text)
    {
        //Debug.LogError(m_mtbWeapon0.name);

        if (m_mtbWeapon0 != null)
        {
            m_mtbWeapon0.transform.Find("SkillDialogPageWeapon0TestUp").GetComponentsInChildren<UILabel>(true)[0].text = text;
            m_mtbWeapon0.transform.Find("SkillDialogPageWeapon0TestDown").GetComponentsInChildren<UILabel>(true)[0].text = text;
        }
        
        m_weapon0Text = text;        
    }

    public void SetWeapon1PageText(string text)
    {
        if (m_mtbWeapon1 != null)
        {
            m_mtbWeapon1.transform.Find("SkillDialogPageWeapon1TestUp").GetComponentsInChildren<UILabel>(true)[0].text = text;
            m_mtbWeapon1.transform.Find("SkillDialogPageWeapon1TestDown").GetComponentsInChildren<UILabel>(true)[0].text = text;
        }
       
        m_weapon1Text = text;        
    }

    public void SetWeaponPageDown(int id)
    {
        m_myTransform.Find(m_widgetToFullName["SkillDialogPageList"]).GetComponent<MogoSingleButtonList>().SetCurrentDownButton(id);
        //m_myTransform.FindChild(m_widgetToFullName[string.Concat("SkillDialogPageWeapon", id)]).GetComponent<MogoFakeClick>().FakeIt();
    }

    #endregion

    #region ����򿪺͹ر�

    public void DestroyUIAndResoruces()
    {
        for (int i = 0; i < 5; ++i)
        {
            ShowSkillCanLearnAnim(false, i);
            ShowSkillOpenAnim(false, i);
        }

        //AssetCacheMgr.ReleaseInstance(MogoUIManager.Instance.SkillIconAtlas);
        if (SystemSwitch.DestroyAllUI)
        {
            MogoUIManager.Instance.DestroySkillUI();
        }
    }

    void OnDisable()
    {
        DestroyUIAndResoruces();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        m_spRefreshCtrl.ShowAsWhiteBlack(true, true);
    }

    #endregion
}
