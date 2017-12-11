/*----------------------------------------------------------------
// Copyright (C) 2013 ���ݣ�����
//
// ģ������MainUIViewManager
// �����ߣ�MaiFeo
// �޸����б��
// �������ڣ�2013.2.21
// ģ��������������UI����ģ�����
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using System;
using Mogo.Game;
using Mogo.GameData;

/// <summary>
/// ��������ʱλ��
/// </summary>
public enum MogoCountDownTarget
{
    None,
    ClimbTower,
    OgreMustDie,
}

public class BloodColorSpriteName
{
    public const string Green = "zdjm-shengmingtiaolvse";
    public const string GreenAnim = "zd-xt-lvq";
    public const string Blue = "zdjm-shengmingtiaolanse";
    public const string BlueAnim = "zd-xt-lanq";
    public const string Purple = "zdjm-shengmingtiaozise";
    public const string PurpleAnim = "zd-xt-ziq";
    public const string Yellow = "zdjm-shengmingtiaohuangse";
    public const string YellowAnim = "zd-xt-huangq";
}

public class MainUIViewManager : MogoUIBehaviour
{
    private static MainUIViewManager m_instance;
    public static MainUIViewManager Instance { get { return MainUIViewManager.m_instance; } }

    private float m_fNormalAttackHover = 0f;
    private bool m_bNormalAttackDown = false;
    // Ϊȥ��������ʱ�������´���
    //private bool m_bNormalAttackPowerUp = false;
    private bool m_bChargingStart = false;
    private GameObject m_goSelfAttack;
    private GameObject m_goCancelManaged;

    private UIFilledSprite m_fsNormalAttackCD;
    private float m_fNormalAttackPowerTime = 1.5f;

    // ս�����ťCD����
    private float m_fSpellOneCD = 0;
    private float m_fSpellTwoCD = 0;
    private float m_fSpellThreeCD = 0;
    private float m_fHPBottleCD = 0;
    private float m_fSpriteSkillCD = 0;

    // ս�����ťCD����
    private float m_fSpellOneCnt = 0;
    private float m_fSpellTwoCnt = 0;
    private float m_fSpellThreeCnt = 0;
    private float m_fHPBottleCnt = 0;
    private float m_fSpriteSkillCnt = 0;

    private UILabel m_lblPlayerBlood;

    private UILabel m_lblSelfAttack;
    private UISprite m_spSelfAttack;

    private UILabel m_lblInstanceCountDown;

    private bool isLockOut = false;
    public bool LockOut
    {
        get { return isLockOut; }
        set { isLockOut = value; }
    }

    bool m_bIsNeedSecondAnim = false;

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    //public static Dictionary<string, string> ButtonTypeToEventUp = new Dictionary<string, string>();

    private void SetUIText(string UIName, string text)
    {
        //var l = m_myTransform.FindChild(UIName).GetComponentInChildren<UILabel>() as UILabel;
        var l = m_myTransform.Find(UIName).GetComponentsInChildren<UILabel>(true);
        if (l != null)
        {
            l[0].text = text;
            //l[0].transform.localScale = new Vector3(18, 18, 18);
        }

        //l.text = text;
        //l.m_myTransform.localScale = new Vector3(15, 15, 15);
    }

    private void SetUITexture(string UIName, string imageName)
    {
        var s = m_myTransform.Find(UIName).GetComponentsInChildren<UISlicedSprite>(true);
        if (s != null)
            s[0].spriteName = imageName;
    }

    UISlicedSprite m_ssPlayerHead;
    /// <summary>
    /// �������ͷ��
    /// </summary>
    /// <param name="imageName"></param>

    public void SetPlayerHeadImage(string imageName)
    {
        //SetUITexture(m_widgetToFullName["MainUIPlayerHead"], imageName);
        m_ssPlayerHead.spriteName = imageName;
    }

    ///// <summary>
    ///// �����������
    ///// </summary>
    ///// <param name="playerName"></param>

    //public void SetPlayerName(string playerName)
    //{      
    //    SetUIText(m_widgetToFullName["PlayerNameText"], playerName);
    //}

    /// <summary>
    /// ������ҵȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetPlayerLevel(byte level)
    {
        SetUIText(m_widgetToFullName["MainUIPlayerLevelText"], level.ToString());
    }

    /// <summary>
    /// �������Ѫ��
    /// </summary>
    /// <param name="blood"></param>
    /// 
    private UIFilledSprite m_fsPlayerBloodFG;
    private MogoBloodAnim m_bloodAnim;
    private MogoBloodAnim m_bossBloodAnim0;
    private MogoBloodAnim m_smallBossBloodAnim0;

    public void SetPlayerBlood(float blood)
    {
        //SetPlayerBloodText(MogoWorld.thePlayer.curHp + "/" + MogoWorld.thePlayer.hp);
        //var s = m_myTransform.FindChild(m_widgetToFullName["MainUIPlayerBloodFG"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        //if (s != null)
        //    s.fillAmount = blood;

        m_bloodAnim.PlayBloodAnim(blood);
        m_fsPlayerBloodFG.fillAmount = blood;
    }

    public void ResetPlayerBloodAnim()
    {
        if (m_bloodAnim != null)
        {
            m_bloodAnim.gameObject.SetActive(false);
            m_bloodAnim.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// �������ŭ��ֵ
    /// </summary>
    private UIFilledSprite m_fsPlayerAnger;
    public void SetPlayerAnger(float anger)
    {
        m_fsPlayerAnger.fillAmount = 0.12f + 0.58f * anger; //0.12f��Ӧfillamont 0 , 0.7f��Ӧ1
    }


    #region ���ﾭ����

    readonly static int PlayerExpListCount = 10; // ���������
    readonly static int PlayerExpPadding = 5;// �����֮��ļ��
    readonly static int OffsetX = 60; // ���ھ���Iconƫ�Ƶ�Xֵ
    int PlayerExpLength = 0;

    void AddPlayerExpFG()
    {
        PlayerExpLength = (int)((1280.0f - OffsetX - (PlayerExpListCount + 1) * PlayerExpPadding) / PlayerExpListCount);
        m_listMainUIPlayerExpFG.Clear();

        for (int i = 0; i < PlayerExpListCount; i++)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("PlayerExpGrid.prefab", (prefab, guid, go) =>
            {
                GameObject temp = (GameObject)go;
                temp.transform.parent = m_goGOMainUIPlayerExpListFG.transform;
                temp.transform.localPosition = new Vector3(index * (PlayerExpLength + PlayerExpPadding) - 640 + OffsetX + PlayerExpPadding, 0, 0);
                temp.transform.localScale = Vector3.one;
                temp.name = "NormalMainUIPlayerExpGrid" + index;
                UISprite sp = temp.transform.Find("PlayerExpFG").GetComponentsInChildren<UISprite>(true)[0];
                sp.transform.localScale = new Vector3(PlayerExpLength, sp.transform.localScale.y, sp.transform.localScale.z);
                sp.fillAmount = 0;
                m_listMainUIPlayerExpFG.Add(sp);
            });
        }
    }


    /// <summary>
    /// ������Ҿ���(TopLeft)
    /// </summary>
    /// <param name="exp"></param>
    UIFilledSprite m_fsPlayerExp;
    public void SetPlayerExp(float exp)
    {
        if (m_fsPlayerExp != null)
            m_fsPlayerExp.fillAmount = exp * 0.7f;

        m_lblMainUIPlayerExpListNum.text = (int)(exp * 100) + "%";
        SetPlayerExpFG();
    }

    /// <summary>
    /// ������Ҿ���(Bottom)
    /// </summary>
    void SetPlayerExpFG()
    {
        float iOneExp = (float)MogoWorld.thePlayer.nextLevelExp / (float)PlayerExpListCount;
        float currentExp = (float)MogoWorld.thePlayer.exp;

        int i = 0;
        for (; i < PlayerExpListCount; i++)
        {
            if (currentExp >= iOneExp * (i + 1))
            {
                m_listMainUIPlayerExpFG[i].fillAmount = 1;
            }
            else
            {
                m_listMainUIPlayerExpFG[i].fillAmount = (currentExp - iOneExp * i) / iOneExp;
                break;
            }
        }

        for (i = i + 1; i < PlayerExpListCount; i++)
        {
            m_listMainUIPlayerExpFG[i].fillAmount = 0;
        }
    }

    #endregion

    //public void SetMiniMapImage(string imageName)
    //{
    //    SetUITexture("TopRight/MiniMap", imageName);
    //}

    #region ���½�ս����ť

    #region ս����ťCD

    public float HpBottleCD
    {
        get { return m_fHPBottleCD; }
        set { m_fHPBottleCD = 1000 * value; }
    }

    public void SpellOneCD(int cd)
    {
        m_fSpellOneCD = cd;
    }

    public void SpellTwoCD(int cd)
    {
        m_fSpellTwoCD = cd;
    }

    public void SpellThreeCD(int cd)
    {
        m_fSpellThreeCD = cd;
    }

    public void SpirteSkillCD(int cd)
    {
        m_fSpriteSkillCD = cd;
    }

    #endregion

    #region ����

    GameObject m_goSpecialFX;
    bool isPowerFXLoaded = false;
    public void ShowSpecialSkillIcon(bool isShow)
    {
        GetTransform("Special").gameObject.SetActive(isShow);
        if (isShow)
        {
            if (isPowerFXLoaded)
                return;
            EventDispatcher.TriggerEvent(SettingEvent.UIDownPlaySound, "SpecialSkillIconShow");

            MogoFXManager.Instance.AttachParticleAnim("fx_ui_powercharge.prefab", "PowerChargeFX",
                GetTransform("Special").position, MogoUIManager.Instance.GetMainUICamera(), 0, 0, 0, () =>
                {
                    GameObject go = GameObject.Find("PowerChargeFX");
                    m_goSpecialFX = go;

                    if (MogoUIManager.Instance.m_MainUI.activeSelf == false)
                    {
                        MogoFXManager.Instance.ReleaseParticleAnim("PowerChargeFX");
                        return;
                    }
                    else
                    {
                        float width = 0.00075f * GameObject.Find("MogoMainUIPanel").transform.localScale.x - 0.075f;
                        go.transform.localScale = new Vector3(width, width, 1);
                    }
                });

            GetTransform("MainUIPlayerAngerFX").gameObject.SetActive(true);
            isPowerFXLoaded = true;
        }
        else
        {
            MogoFXManager.Instance.ReleaseParticleAnim("PowerChargeFX");
            m_goSpecialFX = null;

            GetTransform("MainUIPlayerAngerFX").gameObject.SetActive(false);
            isPowerFXLoaded = false;
        }
    }

    void ShowSpecialSkillFX(bool isShow)
    {
        if (m_goSpecialFX != null)
            m_goSpecialFX.SetActive(isShow);
    }

    #endregion

    #region Ѫƿ��ť

    /// <summary>
    /// ������Ʒ1ͼ��
    /// </summary>
    /// <param name="imageName"></param>
    public void SetItem1Image(string imageName)
    {
        SetUITexture(m_widgetToFullName["MainUIItem1BGUp"], imageName);
    }

    /// <summary>
    /// ������Ʒ1����
    /// </summary>
    /// <param name="num"></param>
    public void SetItem1Num(byte num)
    {
        LoggerHelper.Debug(num);
        SetUIText(m_widgetToFullName["MainUIItem1Num"], num.ToString());
    }
    UIFilledSprite m_hpBottleCD;
    /// <summary>
    /// ������Ʒ1CD
    /// </summary>
    /// <param name="cd"></param>
    public void SetItem1CD(int cd)
    {
        if (m_hpBottleCD != null)
            m_hpBottleCD.fillAmount = (float)cd / 100.0f;
    }

    #endregion

    #region ����1��ť

    /// <summary>
    /// �������ư�ťͼ��
    /// </summary>
    /// <param name="imageName"></param>
    public void SetAffectImage(string imageName)
    {
        m_myTransform.Find(m_widgetToFullName["AffectBG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["AffectDown"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["AffectUp"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["AffectFG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();

        SetUITexture(m_widgetToFullName["AffectBG"], imageName);
        SetUITexture(m_widgetToFullName["AffectFG"], "bb_daojugeguangzhe");
    }

    UIFilledSprite m_fsAffectCD;
    /// <summary>
    /// ��������CD
    /// </summary>
    /// <param name="cd"></param>
    public void SetAffectCD(int cd)
    {
        UISprite spCD = m_myTransform.Find(m_widgetToFullName["AffectCD"]).GetComponentsInChildren<UISprite>(true)[0];
        spCD.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        spCD.spriteName = "cd";
        spCD.color = new Color32(255, 255, 255, 255);
        m_fsAffectCD.fillAmount = (float)cd / 100.0f;
    }

    /// <summary>
    /// ��������CD����
    /// </summary>
    /// <param name="down"></param>
    private UISprite spAffectCDDown1;
    private UISprite spAffectCDDown10;
    private void SetAffectCountDown(int down)
    {
        if (spAffectCDDown1 == null || spAffectCDDown10 == null)
        {
            spAffectCDDown1 = FindTransform("AffectCountDown").GetComponentsInChildren<UISprite>(true)[0];
            spAffectCDDown10 = FindTransform("AffectCountDown10").GetComponentsInChildren<UISprite>(true)[0];
        }        
       
        if (down > 0)
        {
            if (down >= 10)
            {
                spAffectCDDown10.spriteName = "red_" + down / 10;
                spAffectCDDown10.MakePixelPerfect();
                spAffectCDDown10.gameObject.SetActive(true);

                spAffectCDDown1.spriteName = "red_" + down % 10;
                spAffectCDDown1.MakePixelPerfect();
                spAffectCDDown1.transform.localPosition = new Vector3(10, spAffectCDDown1.transform.localPosition.y, spAffectCDDown1.transform.localPosition.z);
                spAffectCDDown1.gameObject.SetActive(true);
            }
            else
            {
                spAffectCDDown1.spriteName = "red_" + down;
                spAffectCDDown1.MakePixelPerfect();
                spAffectCDDown1.transform.localPosition = new Vector3(0, spAffectCDDown1.transform.localPosition.y, spAffectCDDown1.transform.localPosition.z);
                spAffectCDDown1.gameObject.SetActive(true);
                spAffectCDDown10.gameObject.SetActive(false);
            }            
        }
        else
        {
            spAffectCDDown1.gameObject.SetActive(false);
            spAffectCDDown10.gameObject.SetActive(false);
        }
    }

    #endregion

    #region ����2��ť

    /// <summary>
    /// ���������ťͼ��
    /// </summary>
    /// <param name="imageName"></param>
    public void SetOutputImage(string imageName)
    {
        m_myTransform.Find(m_widgetToFullName["OutputBG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["OutputDown"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["OutputUp"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["OutputFG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        SetUITexture(m_widgetToFullName["OutputBG"], imageName);
        SetUITexture(m_widgetToFullName["OutputFG"], "bb_daojugeguangzhe");
    }

    UIFilledSprite m_fsOutputCD;
    /// <summary>
    /// �������CD
    /// </summary>
    /// <param name="cd"></param>
    public void SetOutputCD(int cd)
    {
        UISprite spCD = m_myTransform.Find(m_widgetToFullName["OutputCD"]).GetComponentsInChildren<UISprite>(true)[0];
        spCD.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        spCD.spriteName = "cd";
        spCD.color = new Color32(255, 255, 255, 255);
        m_fsOutputCD.fillAmount = (float)cd / 100.0f;
    }

    /// <summary>
    /// �������CD����
    /// </summary>
    /// <param name="down"></param>
    private UISprite spOutputCDDown1;
    private UISprite spOutputCDDown10;
    private void SetOutputCountDown(int down)
    {
        if (spOutputCDDown1 == null || spOutputCDDown10 == null)
        {
            spOutputCDDown1 = FindTransform("OutputCountDown").GetComponentsInChildren<UISprite>(true)[0];
            spOutputCDDown10 = FindTransform("OutputCountDown10").GetComponentsInChildren<UISprite>(true)[0];
        }      
      
        if (down > 0)
        {
            if (down >= 10)
            {
                spOutputCDDown10.spriteName = "red_" + down / 10;
                spOutputCDDown10.MakePixelPerfect();
                spOutputCDDown10.gameObject.SetActive(true);

                spOutputCDDown1.spriteName = "red_" + down % 10;
                spOutputCDDown1.MakePixelPerfect();
                spOutputCDDown1.transform.localPosition = new Vector3(10, spOutputCDDown1.transform.localPosition.y, spOutputCDDown1.transform.localPosition.z);
                spOutputCDDown1.gameObject.SetActive(true);
            }
            else
            {
                spOutputCDDown1.spriteName = "red_" + down;
                spOutputCDDown1.MakePixelPerfect();
                spOutputCDDown1.transform.localPosition = new Vector3(0, spOutputCDDown1.transform.localPosition.y, spOutputCDDown1.transform.localPosition.z);
                spOutputCDDown1.gameObject.SetActive(true);
                spOutputCDDown10.gameObject.SetActive(false);
            }            
        }
        else
        {
            spOutputCDDown1.gameObject.SetActive(false);
            spOutputCDDown10.gameObject.SetActive(false);
        }
    }

    #endregion

    #region ����3��ť

    /// <summary>
    /// ����λ�ư�ťͼ��
    /// </summary>
    /// <param name="imageName"></param>
    public void SetMoveImage(string imageName)
    {
        m_myTransform.Find(m_widgetToFullName["MoveBG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["MoveDown"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["MoveUp"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["MoveFG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        SetUITexture(m_widgetToFullName["MoveBG"], imageName);
        SetUITexture(m_widgetToFullName["MoveFG"], "bb_daojugeguangzhe");
    }

    UIFilledSprite m_fsMoveCD;
    /// <summary>
    /// ����λ��CD
    /// </summary>
    /// <param name="cd"></param>
    public void SetMoveCD(int cd)
    {
        UISprite spCD = m_myTransform.Find(m_widgetToFullName["MoveCD"]).GetComponentsInChildren<UISprite>(true)[0];
        spCD.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        spCD.spriteName = "cd";
        spCD.color = new Color32(255, 255, 255, 255);
        m_fsMoveCD.fillAmount = (float)cd / 100.0f;
    }

    /// <summary>
    /// ����λ��CD����
    /// </summary>
    /// <param name="down"></param>
    private UISprite spMoveCDDown1;
    private UISprite spMoveCDDown10;
    private void SetMoveCountDown(int down)
    {
        if (spMoveCDDown1 == null || spMoveCDDown10 == null)
        {
            spMoveCDDown1 = FindTransform("MoveCountDown").GetComponentsInChildren<UISprite>(true)[0];
            spMoveCDDown10 = FindTransform("MoveCountDown10").GetComponentsInChildren<UISprite>(true)[0];
        }    
        
        if (down > 0)
        {
            if (down >= 10)
            {
                spMoveCDDown10.spriteName = "red_" + down / 10;
                spMoveCDDown10.MakePixelPerfect();
                spMoveCDDown10.gameObject.SetActive(true);

                spMoveCDDown1.spriteName = "red_" + down % 10;
                spMoveCDDown1.MakePixelPerfect();
                spMoveCDDown1.transform.localPosition = new Vector3(10, spMoveCDDown1.transform.localPosition.y, spMoveCDDown1.transform.localPosition.z);
                spMoveCDDown1.gameObject.SetActive(true);
            }
            else
            {
                spMoveCDDown1.spriteName = "red_" + down;
                spMoveCDDown1.MakePixelPerfect();
                spMoveCDDown1.transform.localPosition = new Vector3(0, spMoveCDDown1.transform.localPosition.y, spMoveCDDown1.transform.localPosition.z);
                spMoveCDDown1.gameObject.SetActive(true);
                spMoveCDDown10.gameObject.SetActive(false);
            }          
        }
        else
        {
            spMoveCDDown1.gameObject.SetActive(false);
            spMoveCDDown10.gameObject.SetActive(false);
        }
    }

    #endregion

    #region ��ͨ������ť

    public void SetNormalAttackIconByID(int id)
    {
        string imgName = "";

        switch (id)
        {
            case 1:
                imgName = "dajian";
                break;

            case 2:
                imgName = "quantao";
                break;

            case 3:
                imgName = "bishou";
                break;

            case 4:
                imgName = "yueren";
                break;

            case 5:
                imgName = "fazhan";
                break;

            case 6:
                imgName = "shanzi";
                break;

            case 7:
                imgName = "gongjian";
                break;

            case 8:
                imgName = "nupao";
                break;
        }

        SetNormalAttackIcon(imgName);
    }
    public void SetNormalAttackIcon(string imgName)
    {
        m_myTransform.Find(m_widgetToFullName["NormalAttackBGDown"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["NormalAttackBGUp"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();

        m_myTransform.Find(m_widgetToFullName["NormalAttackBGDown"]).GetComponentsInChildren<UISprite>(true)[0].spriteName = "jinengdiquan_2";
        m_myTransform.Find(m_widgetToFullName["NormalAttackBGUp"]).GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
    }

    /// <summary>
    /// ������ͨ������ťͼ��
    /// </summary>
    /// <param name="imageName"></param>
    public void SetNormalAttackImage(string imageName)
    {
        SetUITexture(m_widgetToFullName["NormalAttackUp"], imageName);
    }

    /// <summary>
    /// ������ͨ����CD
    /// </summary>
    /// <param name="cd"></param>
    public void SetNormalAttackCD(int cd)
    {
        m_fsNormalAttackCD.fillAmount = (float)cd / 100.0f;
    }

    #endregion

    #region ŭ����ť

    /// <summary>
    /// �������ⰴťͼ��
    /// </summary>
    /// <param name="imageName"></param>
    public void SetSpeicalImage(string imgName)
    {
        m_myTransform.Find(m_widgetToFullName["SpecialBG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["SpecialDown"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["SpecialUp"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_myTransform.Find(m_widgetToFullName["SpecialFG"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        SetUITexture(m_widgetToFullName["SpecialBG"], imgName);
        //SetUITexture(m_widgetToFullName["SpecialFG"], "bb_daojugeguangzhe");
        SetSpriteImage("SpecialFG", "bb_daojugeguangzhe");
    }

    UIFilledSprite m_fsSpecialCD;
    /// <summary>
    ///�������ⰴťCD
    /// </summary>
    /// <param name="cd"></param>
    public void SetSpecialCD(int cd)
    {
        //UISprite spCD = m_myTransform.FindChild(m_widgetToFullName["OutputCD"]).GetComponentsInChildren<UISprite>(true)[0];
        m_fsSpecialCD.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_fsSpecialCD.spriteName = "cd";
        m_fsSpecialCD.color = new Color32(255, 255, 255, 255);
        m_fsSpecialCD.fillAmount = (float)cd / 100.0f;
    }

    #endregion

    #region ���鼼�ܰ�ť

    /// <summary>
    /// ���þ��鰴ťͼ��
    /// </summary>
    /// <param name="imageName"></param>
    public void SetSpriteSkillImage(string imageName)
    {
        //FindTransform("MainUISpriteSkillBtnBG").GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        //FindTransform("MainUISpriteSkillBtnDown").GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        //FindTransform("MainUISpriteSkillBtnUp").GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        //FindTransform("MainUISpriteSkillBtnFG").GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        SetUITexture(m_widgetToFullName["MainUISpriteSkillBtnBG"], imageName);
        SetUITexture(m_widgetToFullName["MainUISpriteSkillBtnFG"], "bb_daojugeguangzhe");
    }

    UIFilledSprite m_fsSpriteSkillCD;
    /// <summary>
    /// ���þ��鰴ťCD
    /// </summary>
    /// <param name="cd"></param>
    public void SetSpriteSkillCD(int cd)
    {
        UISprite spCD = FindTransform("MainUISpriteSkillBtnCD").GetComponentsInChildren<UISprite>(true)[0];
        spCD.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        spCD.spriteName = "cd";
        spCD.color = new Color32(255, 255, 255, 255);
        m_fsSpriteSkillCD.fillAmount = (float)cd / 100.0f;
    }

    /// <summary>
    /// ���þ��鰴ťCD����
    /// </summary>
    /// <param name="down"></param>
    private UISprite spSpriteSkillCDDown1;
    private UISprite spSpriteSkillCDDown10;
    private void SetSpriteSkillCountDown(int down)
    {
        if (spSpriteSkillCDDown1 == null || spSpriteSkillCDDown10 == null)
        {
            spSpriteSkillCDDown1 = FindTransform("MainUISpriteSkillCountDown").GetComponentsInChildren<UISprite>(true)[0];
            spSpriteSkillCDDown10 = FindTransform("MainUISpriteSkillCountDown10").GetComponentsInChildren<UISprite>(true)[0];
        }

        if (down > 0)
        {
            if (down >= 10)
            {
                spSpriteSkillCDDown10.spriteName = "red_" + down / 10;
                spSpriteSkillCDDown10.MakePixelPerfect();
                spSpriteSkillCDDown10.gameObject.SetActive(true);

                spSpriteSkillCDDown1.spriteName = "red_" + down % 10;
                spSpriteSkillCDDown1.MakePixelPerfect();
                spSpriteSkillCDDown1.transform.localPosition = new Vector3(10, spSpriteSkillCDDown1.transform.localPosition.y, spSpriteSkillCDDown1.transform.localPosition.z);
                spSpriteSkillCDDown1.gameObject.SetActive(true);
            }
            else
            {
                spSpriteSkillCDDown1.spriteName = "red_" + down;
                spSpriteSkillCDDown1.MakePixelPerfect();
                spSpriteSkillCDDown1.transform.localPosition = new Vector3(0, spSpriteSkillCDDown1.transform.localPosition.y, spSpriteSkillCDDown1.transform.localPosition.z);
                spSpriteSkillCDDown1.gameObject.SetActive(true);
                spSpriteSkillCDDown10.gameObject.SetActive(false);
            }
        }
        else
        {
            spSpriteSkillCDDown1.gameObject.SetActive(false);
            spSpriteSkillCDDown10.gameObject.SetActive(false);
        }
    }

    #endregion

    #endregion

    /// <summary>
    /// ��������ťͼƬ
    /// </summary>
    /// <param name="imageName"></param>
    public void SetTaskImage(string imageName)
    {
        SetUITexture(m_widgetToFullName["TaskUp"], imageName);
    }

    /// <summary>
    /// ����������Ϣ����
    /// </summary>
    /// <param name="text"></param>
    public void SetTaskInfoText(string text)
    {
        text = text.Replace("{", "[FF0000]");
        text = text.Replace("}", "[-]");
        SetUIText(m_widgetToFullName["TaskInfoText"], text);
    }

    #region ���Ͻǹ�����Ϣ

    UIFilledSprite m_fsNormalTargetBlood;
    /// <summary>
    /// ����ѡ�з�bossѪ��
    /// </summary>
    /// <param name="blood"></param>
    public void SetNormalTargetBlood(float bloodProgress)
    {
        m_fsNormalTargetBlood.fillAmount = bloodProgress; ;
    }


    private UIFilledSprite[] m_fsBossTargetBloodList = new UIFilledSprite[5]; // ��BossѪ��
    private UIFilledSprite[] m_fsSmallBossTargetBloodList = new UIFilledSprite[5]; // СBossѪ��


    /// <summary>
    /// ����ѡ��bossѪ��
    /// </summary>
    /// <param name="bloodProgress"></param>
    public void SetBossTargetBlood(float bloodProgress, int level)
    {
        if (m_curBossType == (int)MonsterData.MonsterType.bigBoss)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (i < 4 - level)
                {
                    m_fsBossTargetBloodList[i].gameObject.SetActive(false);
                }
                else
                {
                    m_fsBossTargetBloodList[i].gameObject.SetActive(true);
                }
            }

            LoggerHelper.Debug("level: " + (4 - level) + "bloodProgress: " + bloodProgress);

            if (m_bossBloodAnim0 != null)
            {
                m_bossBloodAnim0.PlayBloodAnim(bloodProgress, level, true, () => { ShowBossTarget(false); });
            }
            m_fsBossTargetBloodList[4 - level].fillAmount = bloodProgress;
        }
        else/* if (m_curBossType == (int)MonsterData.MonsterType.smallBoss)*/
        {
            for (int i = 0; i < 4; ++i)
            {
                if (i < 4 - level)
                {
                    m_fsSmallBossTargetBloodList[i].gameObject.SetActive(false);
                }
                else
                {
                    m_fsSmallBossTargetBloodList[i].gameObject.SetActive(true);
                }
            }

            LoggerHelper.Debug("level: " + (4 - level) + "bloodProgress: " + bloodProgress);

            if (m_smallBossBloodAnim0 != null)
            {
                m_smallBossBloodAnim0.PlayBloodAnim(bloodProgress, level, true, () => { ShowBossTarget(false, m_curBossType); });
            }
            m_fsSmallBossTargetBloodList[4 - level].fillAmount = bloodProgress;
        }
    }

    /// <summary>
    /// ����ѡ�з�boss����
    /// </summary>
    /// <param name="name"></param>
    public void SetNormalTargetName(string name)
    {
        SetUIText(m_widgetToFullName["NormalTargetNameText"], name);
    }

    /// <summary>
    /// ����ѡ��boss����
    /// </summary>
    /// <param name="name"></param>
    public void SetBossTargetName(string name, int type = (int)MonsterData.MonsterType.bigBoss)
    {
        if (type == (int)MonsterData.MonsterType.bigBoss)
            SetUIText(m_widgetToFullName["BossTargetNameText"], name);
        else/* if (type == (int)MonsterData.MonsterType.smallBoss)*/
            SetUIText(m_widgetToFullName["SmallBossTargetNameText"], name);
    }

    /// <summary>
    /// ����ѡ�з�boss�ȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetNormalTargetLevel(int level)
    {
        SetUIText(m_widgetToFullName["NormalTargetLevelText"], level.ToString());
    }

    /// <summary>
    /// ����ѡ��boss�ȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetBossTargetLevel(int level, int type = (int)MonsterData.MonsterType.bigBoss)
    {
        if (type == (int)MonsterData.MonsterType.bigBoss)
            SetUIText(m_widgetToFullName["BossTargetLevelText"], level.ToString());
        else/* if (type == (int)MonsterData.MonsterType.smallBoss)*/
            SetUIText(m_widgetToFullName["SmallBossTargetLevelText"], level.ToString());
    }

    /// <summary>
    /// ����ѡ��bossͷ��
    /// </summary>
    /// <param name="imageName"></param>
    public void SetBossTargetFace(string imageName, int type = (int)MonsterData.MonsterType.bigBoss)
    {
        if (type == (int)MonsterData.MonsterType.bigBoss)
            SetUITexture(m_widgetToFullName["BossTargetFaceFG"], imageName);
        else/* if (type == (int)MonsterData.MonsterType.smallBoss)*/
            SetUITexture(m_widgetToFullName["SmallBossTargetFaceFG"], imageName);
    }

    #endregion

    #region ������Ϣ

    /// <summary>
    /// ���õ�1����������
    /// </summary>
    /// <param name="name"></param>
    public void SetMember1Name(string name)
    {
        SetUIText(m_widgetToFullName["TeamMember1NameText"], name);
    }

    /// <summary>
    /// ���õ�2����������
    /// </summary>
    /// <param name="name"></param>
    public void SetMember2Name(string name)
    {
        SetUIText(m_widgetToFullName["TeamMember2NameText"], name);
    }

    /// <summary>
    /// ���õ�3����������
    /// </summary>
    /// <param name="name"></param>
    public void SetMember3Name(string name)
    {
        SetUIText(m_widgetToFullName["TeamMember3NameText"], name);
    }

    /// <summary>
    /// ���õ�1�����ѵȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetMember1Level(int level)
    {
        SetUIText(m_widgetToFullName["TeamMember1LevelText"], level.ToString());
    }

    /// <summary>
    /// ���õ�2�����ѵȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetMember2Level(int level)
    {
        SetUIText(m_widgetToFullName["TeamMember2LevelText"], level.ToString());
    }

    /// <summary>
    /// ���õ�3�����ѵȼ�
    /// </summary>
    /// <param name="level"></param>
    public void SetMember3Level(int level)
    {
        SetUIText(m_widgetToFullName["TeamMember3LevelText"], level.ToString());
    }

    private GameObject m_member1;
    public void ShowMember1(bool isShow)
    {
        m_member1.SetActive(isShow);
    }

    private UISprite m_member1Image;
    public void SetMember1Image(string vocationString)
    {
        m_member1Image.spriteName = vocationString;
    }

    private UIFilledSprite m_fsMember1Blood;
    /// <summary>
    /// ���õ�1������Ѫ��
    /// </summary>
    /// <param name="blood"></param>
    public void SetMember1Blood(int blood)
    {

        m_fsMember1Blood.fillAmount = (float)blood / 100.0f;
    }

    private GameObject m_member2;
    public void ShowMember2(bool isShow)
    {
        m_member2.SetActive(isShow);
    }

    private UISprite m_member2Image;
    public void SetMember2Image(string vocationString)
    {
        m_member2Image.spriteName = vocationString;
    }

    private UIFilledSprite m_fsMember2Blood;
    /// <summary>
    /// ���õ�2������Ѫ��
    /// </summary>
    /// <param name="blood"></param>
    public void SetMember2Blood(int blood)
    {
        m_fsMember2Blood.fillAmount = (float)blood / 100.0f;
    }

    private GameObject m_member3;
    public void ShowMember3(bool isShow)
    {
        m_member3.SetActive(isShow);
    }

    private UISprite m_member3Image;
    public void SetMember3Image(string vocationString)
    {
        m_member3Image.spriteName = vocationString;
    }

    private UIFilledSprite m_fsMember3Blood;
    /// <summary>
    /// ���õ�3������Ѫ��
    /// </summary>
    /// <param name="blood"></param>
    public void SetMember3Blood(int blood)
    {
        m_fsMember3Blood.fillAmount = (float)blood / 100.0f;
    }

    #endregion

    /// <summary>
    /// ��ʾ��bossѡ����Ϣ
    /// </summary>
    /// <param name="show"></param>
    public void ShowNormalTarget(bool show)
    {
        var go = m_myTransform.Find(m_widgetToFullName["NormalTarget"]).gameObject;
        if (go == null)
            return;

        if (show)
        {
            go.SetActive(true);
        }
        else
        {
            go.SetActive(false);
        }
    }

    int m_curBossType = (int)MonsterData.MonsterType.bigBoss;
    /// <summary>
    /// ��ʾbossѡ����Ϣ
    /// </summary>
    /// <param name="show"></param>
    public void ShowBossTarget(bool show, int level = 0, int type = (int)MonsterData.MonsterType.bigBoss)
    {
        //////Mogo.Util.LoggerHelper.Debug("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ShowBossTarget");
        GameObject goBoss = m_myTransform.Find(m_widgetToFullName["BossTarget"]).gameObject;
        GameObject goSmallBoss = m_myTransform.Find(m_widgetToFullName["SmallBossTarget"]).gameObject;

        if (goBoss == null || goSmallBoss == null)
            return;

        m_curBossType = type;

        if (show)
        {
            if (type == (int)MonsterData.MonsterType.bigBoss)
                goBoss.SetActive(true);
            else/* if (type == (int)MonsterData.MonsterType.smallBoss)*/
                goSmallBoss.SetActive(true);
        }
        else
        {
            goBoss.SetActive(false);
            goSmallBoss.SetActive(false);
        }

        if (type == (int)MonsterData.MonsterType.bigBoss)
        {
            for (int i = 0; i < 5; ++i)
            {
                if (i < level)
                {
                    m_fsBossTargetBloodList[i].gameObject.SetActive(false);
                    m_fsBossTargetBloodList[i].fillAmount = 1;
                }
                else
                {
                    m_fsBossTargetBloodList[i].gameObject.SetActive(true);
                    m_fsBossTargetBloodList[i].fillAmount = 1;
                }
            }
        }
        else/* if (type == (int)MonsterData.MonsterType.smallBoss)*/
        {
            for (int i = 0; i < 5; ++i)
            {
                if (i < level)
                {
                    m_fsSmallBossTargetBloodList[i].gameObject.SetActive(false);
                    m_fsSmallBossTargetBloodList[i].fillAmount = 1;
                }
                else
                {
                    m_fsSmallBossTargetBloodList[i].gameObject.SetActive(true);
                    m_fsSmallBossTargetBloodList[i].fillAmount = 1;
                }
            }
        }
    }

    public void SetControllStickEnable(bool isEnable)
    {
        m_myTransform.Find(m_widgetToFullName["Controller"]).GetComponentsInChildren<ControlStick>(true)[0].enabled = isEnable;
        m_myTransform.Find(m_widgetToFullName["Controller"]).GetComponentsInChildren<BoxCollider>(true)[0].enabled = isEnable;
        m_myTransform.Find(m_widgetToFullName["MainUIBG"]).GetComponentsInChildren<TouchControll>(true)[0].enabled = !isEnable;
        m_myTransform.Find(m_widgetToFullName["MainUIBG"]).GetComponentsInChildren<BoxCollider>(true)[0].enabled = !isEnable;
    }

    public void ShowSelfAttack(bool isShow)
    {
        m_goSelfAttack.SetActive(isShow);
    }

    public void ShowCancelManaged(bool isShow)
    {
        m_goCancelManaged.SetActive(isShow);
    }

    // ����ս����������Ѫ��Text
    public void SetPlayerBloodText(string blood)
    {
        if (m_lblPlayerBlood != null)
        {
            if (MogoWorld.thePlayer != null)
                m_lblPlayerBlood.text = MogoWorld.thePlayer.curHp + "/" + MogoWorld.thePlayer.hp;

            int id = 120;
            // ����Ѫ��С��20%��ʾ
            if (MogoWorld.thePlayer.curHp < MogoWorld.thePlayer.hp * 0.2f
                && MogoWorld.thePlayer.curHp > 0
                && !StoryManager.Instance.IsCurrentInCG())
            {
                if (GuideSystem.Instance.guideTimes.ContainsKey(id) && GuideSystem.Instance.guideTimes[id] >= GuideXMLData.dataMap.Get(id).guideTimes)
                {
                    //������ʾ������
                    MogoGlobleUIManager.Instance.ShowBattlePlayerBloodTipPanel(true);
                }
                else if (IsHpBottleVisible())
                {
                    TeachUILogicManager.Instance.TruelySetTeachUIFocus(269, LanguageData.GetContent(9148), false, 0);
                    GuideSystem.Instance.SaveGuide(id);
                }

            }
            else
            {
                MogoGlobleUIManager.Instance.ShowBattlePlayerBloodTipPanel(false);
            }
        }
    }  

    private void OnTaskUp()
    {
        GameObject go = m_myTransform.Find(m_widgetToFullName["TaskInfo"]).gameObject;
        if (go.activeSelf)
        {
            go.SetActive(false);
        }
        else
        {
            go.SetActive(true);
        }
    }

    void OnNormalAttackDown()
    {
        if (!MainUILogicManager.Instance.IsAttackable)
            return;

        m_bNormalAttackDown = true;
        EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.NORMALATTACK);
    }

    void OnNormalAttackUp()
    {
        if (!MainUILogicManager.Instance.IsAttackable)
            return;

        //        if (m_bNormalAttackPowerUp)
        //        {
        //            EventDispatcher.TriggerEvent(MainUILogicManager.MainUIEvent.NORMALATTACK);
        //        }
        //        if (m_bChargingStart)
        //        {
        //            EventDispatcher.TriggerEvent(MainUILogicManager.MainUIEvent.POWERCHARGEINTERRUPT);
        //        }

        if (m_fNormalAttackHover > m_fNormalAttackPowerTime)
        {
            //         m_bNormalAttackPowerUp = true;
            m_bChargingStart = false;
            EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.POWERCHARGECOMPLETE);

        }
        else if (m_bChargingStart)
        {
            m_bChargingStart = false;
            EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.POWERCHARGEINTERRUPT);
        }

        m_bNormalAttackDown = false;
        m_fsNormalAttackCD.fillAmount = 0;
        m_fNormalAttackHover = 0;
        //       m_bNormalAttackPowerUp = false;
    }

    void OnPlayerInfoBGUp()
    {
        if (MogoUIManager.Instance.CurrentUI != MogoUIManager.Instance.m_InstanceUI
            && MogoUIManager.Instance.CurrentUI != MogoUIManager.Instance.m_InstancePassRewardUI
            && MogoWorld.thePlayer.sceneId != 10100
            && MapData.dataMap.Get(MogoWorld.thePlayer.sceneId).type != MapType.TOWERDEFENCE
            && MapData.dataMap.Get(MogoWorld.thePlayer.sceneId).type != MapType.OCCUPY_TOWER 
            && !isLockOut)
        {
            MogoUIManager.Instance.ShowMogoBattleMenuUI();
        }
    }

    void OnCommunityUp()
    {
        LoggerHelper.Debug("CommunityUp");
        if (!isLockOut)
            EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.COMMUNITY);
    }

    void OnCancelManagedUp()
    {
        LoggerHelper.Debug("CancelManaged");
    }

    void OnBloodAnim0Finished()
    {
        if (m_bIsNeedSecondAnim)
        {

        }
    }

    void OnBloodAnim1Finished()
    {

    }

    private ComboAttack m_comboAttack;
    public void SetComboAttackNum(int num)
    {
        if (num > 0)
        {
            m_comboAttack.gameObject.SetActive(true);
            m_comboAttack.SetComboAttackNum(num);
        }
        else
        {
            m_comboAttack.gameObject.SetActive(false);
        }
    }

    #region ռ��

    #region ռ�����װ�

    private readonly static int MAX_OCCUPY_TOWER_RANK = 6;
    private GameObject m_goOccupyTowerRank;
    private UILabel m_lblOccupyTowerRankTitle;
    private List<UILabel> m_listOccupyTowerRank = new List<UILabel>();

    /// <summary>
    /// �Ƿ���ʾռ�����װ�
    /// �뿪����ʱ����
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowOccupyTowerRank(bool isShow)
    {
        if (m_goOccupyTowerRank != null)
            m_goOccupyTowerRank.SetActive(isShow);
    }

    /// <summary>
    /// ����Title
    /// </summary>
    /// <param name="title"></param>
    public void SetOccupyTowerRankTitle(string title)
    {
        m_lblOccupyTowerRankTitle.text = title;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="ranklist"></param>
    public void SetOccupyTowerRankList(List<string> ranklist)
    {
        for (int i = 0; i < m_listOccupyTowerRank.Count; i++)
        {
            if (i < ranklist.Count)
                m_listOccupyTowerRank[i].text = ranklist[i];
            else
                m_listOccupyTowerRank[i].text = "";
        }
    }

    #endregion

    #region ռ��ս����Ϣ

    private GameObject m_goOccupyTowerNotice;
    private UILabel m_lblOccupyTowerNoticeCountDownNum;
    private UISprite m_spOccupyTowerNoticeEnemyBloodFG;
    private UISprite m_spOccupyTowerNoticeOwnBloodFG;   
    private UILabel m_lblOccupyTowerNoticeEnemyBloodNum;
    private UILabel m_lblOccupyTowerNoticeOwnBloodNum;
    private UILabel m_lblOccupyTowerNoticeEnemyScore;
    private UILabel m_lblOccupyTowerNoticeOwnScore;

    private UISprite m_spOccupyTowerNoticeEnemyBloodAnim;
    private UISprite m_spOccupyTowerNoticeOwnBloodAnim;
    private MogoBloodAnim m_occupyTowerNoticeEnemyBloodAnim;
    private MogoBloodAnim m_occupyTowerNoticeOwnBloodAnim;

    /// <summary>
    /// �Ƿ����λ��(Ĭ���Լ������󣬵з�����)
    /// </summary>
    private bool m_IsExchangePosition = false;
    private bool IsExchangePosition
    {
        get { return m_IsExchangePosition; }
        set
        {
            m_IsExchangePosition = value;
            if (!m_IsExchangePosition)
            {
                m_spOccupyTowerNoticeOwnBloodFG.spriteName = BloodColorSpriteName.Blue;
                m_spOccupyTowerNoticeOwnBloodAnim.spriteName = BloodColorSpriteName.BlueAnim;

                m_spOccupyTowerNoticeEnemyBloodFG.spriteName = BloodColorSpriteName.Purple;
                m_spOccupyTowerNoticeEnemyBloodAnim.spriteName = BloodColorSpriteName.PurpleAnim;
            }
            else
            {
                m_spOccupyTowerNoticeOwnBloodFG.spriteName = BloodColorSpriteName.Purple;
                m_spOccupyTowerNoticeOwnBloodAnim.spriteName = BloodColorSpriteName.PurpleAnim;

                m_spOccupyTowerNoticeEnemyBloodFG.spriteName = BloodColorSpriteName.Blue;
                m_spOccupyTowerNoticeEnemyBloodAnim.spriteName = BloodColorSpriteName.BlueAnim;
            }
        }
    }

    /// <summary>
    /// �Ƿ���ʾռ��ս����Ϣ
    /// </summary>
    /// <param name="isShow"></param>    
    public void ShowOccupyTowerNotice(bool isShow, bool isExchangePosition = false)
    {
        IsExchangePosition = isExchangePosition;
        m_goOccupyTowerNotice.SetActive(isShow);
        if (isShow)
            SetMainUIPlayerInfoPos(true);
        else
            SetMainUIPlayerInfoPos(false);
    }

    /// <summary>
    /// �����ҷ��÷�
    /// </summary>
    /// <param name="score"></param>
    public void SetOccupyTowerNoticeOwnScore(int score)
    {
        if (!IsExchangePosition)
        {
            m_lblOccupyTowerNoticeOwnScore.text = string.Format(LanguageData.GetContent(48902), score);
        }
        else
        {
            m_lblOccupyTowerNoticeEnemyScore.text = string.Format(LanguageData.GetContent(48902), score);
        }        
    }

    /// <summary>
    /// ���õз��÷�
    /// </summary>
    /// <param name="score"></param>
    public void SetOccupyTowerNoticeEnemyScore(int score)
    {
        if (!IsExchangePosition)
        {
            m_lblOccupyTowerNoticeEnemyScore.text = string.Format(LanguageData.GetContent(48903), score);
        }
        else
        {
            m_lblOccupyTowerNoticeOwnScore.text = string.Format(LanguageData.GetContent(48903), score);
        }        
    }

    /// <summary>
    /// �����ҷ�Ѫ��
    /// </summary>
    /// <param name="percentNum"></param>
    public void SetOccupyTowerNoticeOwnBlood(float percentNum)
    {
        percentNum = Math.Min(percentNum, 1);
        percentNum = Math.Max(percentNum, 0);
        if (!IsExchangePosition)
        {
            m_spOccupyTowerNoticeOwnBloodFG.fillAmount = percentNum;
            m_lblOccupyTowerNoticeOwnBloodNum.text = string.Format("{0}%", (int)(percentNum * 100));
            m_occupyTowerNoticeOwnBloodAnim.PlayBloodAnim(percentNum, 4, false);
        }
        else
        {
            m_spOccupyTowerNoticeEnemyBloodFG.fillAmount = percentNum;
            m_lblOccupyTowerNoticeEnemyBloodNum.text = string.Format("{0}%", (int)(percentNum * 100));
            m_occupyTowerNoticeEnemyBloodAnim.PlayBloodAnim(percentNum);
        }        
    }

    /// <summary>
    /// ���õз�Ѫ��
    /// </summary>
    /// <param name="percentNum"></param>
    public void SetOccupyTowerNoticeEnemyBlood(float percentNum)
    {
        percentNum = Math.Min(percentNum, 1);
        percentNum = Math.Max(percentNum, 0);
        if (!IsExchangePosition)
        {
            m_spOccupyTowerNoticeEnemyBloodFG.fillAmount = percentNum;
            m_lblOccupyTowerNoticeEnemyBloodNum.text = string.Format("{0}%", (int)(percentNum * 100));
            m_occupyTowerNoticeEnemyBloodAnim.PlayBloodAnim(percentNum);
        }
        else
        {
            m_spOccupyTowerNoticeOwnBloodFG.fillAmount = percentNum;
            m_lblOccupyTowerNoticeOwnBloodNum.text = string.Format("{0}%", (int)(percentNum * 100));
            m_occupyTowerNoticeOwnBloodAnim.PlayBloodAnim(percentNum, 4, false);
        }        
    }

    /// <summary>
    /// ռ������ʱ
    /// </summary>   
    private MogoCountDown m_occupyTowerCountDown = null;
    public void BeginCountDown1(bool isShow, int secondsNum)
    {
        if (isShow)
        {
            if (m_occupyTowerCountDown != null)
                m_occupyTowerCountDown.Release();

            m_occupyTowerCountDown = new MogoCountDown(m_lblOccupyTowerNoticeCountDownNum, secondsNum,
                "", "", "", MogoCountDown.TimeStringType.UpToMinutes, () =>
                {

                });
        }
        else
        {
            if (m_occupyTowerCountDown != null)
                m_occupyTowerCountDown.Release();
        }
    } 

    #endregion

    #region ռ�������Ͻ������Ϣλ�õ���

    private GameObject m_goMainUIPlayerInfo;
    private Vector3 m_vecMainUIPlayerInfoPosDefault;
    private Vector3 m_vecMainUIPlayerInfoPosPVP;

    /// <summary>
    /// �������Ͻ������Ϣλ��
    /// </summary>
    public void SetMainUIPlayerInfoPos(bool isPVP = false)
    {
        if (isPVP)
        {
            m_goMainUIPlayerInfo.gameObject.SetActive(false);
            //m_goMainUIPlayerInfo.transform.localPosition = m_vecMainUIPlayerInfoPosPVP;
        }
        else
        {
            m_goMainUIPlayerInfo.gameObject.SetActive(true);
            //m_goMainUIPlayerInfo.transform.localPosition = m_vecMainUIPlayerInfoPosDefault;
        }
    }

    #endregion

    #endregion

    #region ħ�����װ�

    private UILabel m_lblContributeRankText;
    private UILabel m_lblFirstRank;
    private UILabel m_lblSecRank;
    private UILabel m_lblTriRank;

    /// <summary>
    /// �Ƿ���ʾħ�����װ�
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowContributeRankDialog(bool isShow)
    {
        m_lblContributeRankText.transform.parent.gameObject.SetActive(isShow);
    }

    /// <summary>
    /// ����Title
    /// </summary>
    /// <param name="text"></param>
    public void SetContributeRankText(string text)
    {
        m_lblContributeRankText.text = text;
    }

    /// <summary>
    /// ���õ�һ��
    /// </summary>
    /// <param name="text"></param>
    public void SetFirstRank(string text)
    {
        m_lblFirstRank.text = text;
    }

    /// <summary>
    /// ���õڶ���
    /// </summary>
    /// <param name="text"></param>
    public void SetSecRank(string text)
    {
        m_lblSecRank.text = text;
    }

    /// <summary>
    /// ���õ�����
    /// </summary>
    /// <param name="text"></param>
    public void SetTriRank(string text)
    {
        m_lblTriRank.text = text;
    }

    #endregion

    private UILabel m_lblComboAttackNum;
    private UILabel m_lblDamageNum;

    private GameObject m_goAutoFight;
    private GameObject m_goAutoFightText;
    private GameObject m_goAutoFightStateText;

    private GameObject m_goDiamondProtectBtn;
    private GameObject m_goBuildingProtectBtn;

    private UISprite m_spNormalAttackBG;
    private UISprite m_spNormalAttackFG;

    public void ShowAutoFight(bool active)
    {
        if (active)
            m_goAutoFight.SetActive(true);
        else
            m_goAutoFight.SetActive(false);
    }

    public void SetManagedComboAttackNum(string text)
    {
        m_lblComboAttackNum.text = text;
    }

    public void SetManagedDamageNum(string text)
    {
        m_lblDamageNum.text = text;
    }

    public void ShowManagedBattleInfo(bool isShow)
    {
        m_lblDamageNum.transform.parent.gameObject.SetActive(isShow);
    }

    private GameObject m_goMainUIBottom;
    private GameObject m_goMainUIBottomLeft;
    private GameObject m_goMainUIBottomRight;
    private GameObject m_goMainUITop;
    private GameObject m_goMainUITopLeft;
    private GameObject m_goMainUITopRight;
    private GameObject m_goController;

    private GameObject m_goNormalAttackBtn;
    private GameObject m_goSkill0Btn;
    private GameObject m_goSkill1Btn;
    private GameObject m_goSkill2Btn;
    private GameObject m_goBottle;
    private GameObject m_goSpriteSkillBtn;
    private GameObject m_goCommunityBtn;

    private GameObject m_goMainUIPlayerExpList;// ��ɫ������
    private GameObject m_goGOMainUIPlayerExpListFG;
    private UILabel m_lblMainUIPlayerExpListNum;
    private List<UISprite> m_listMainUIPlayerExpFG = new List<UISprite>();

    #region ���

    public override void CallWhenLoadResources()
    {
        m_instance = this;
        ID = MFUIManager.MFUIID.BattleMainUI;
        MFUIManager.GetSingleton().RegisterUI(ID, m_myGameObject);
        FillFullNameData(m_myTransform);

        m_goMainUIBottom = m_myTransform.Find(m_widgetToFullName["Bottom"]).gameObject;
        m_goMainUIBottomLeft = m_myTransform.Find(m_widgetToFullName["BottomLeft"]).gameObject;
        m_goMainUIBottomRight = m_myTransform.Find(m_widgetToFullName["BottomRight"]).gameObject;
        m_goMainUITop = m_myTransform.Find("Top").gameObject;
        m_goMainUITopLeft = m_myTransform.Find(m_widgetToFullName["TopLeft"]).gameObject;
        m_goMainUITopRight = m_myTransform.Find(m_widgetToFullName["TopRight"]).gameObject;
        m_goController = m_myTransform.Find("BottomLeft/Controller").gameObject;
        m_goController.AddComponent<ControlStick>();

        // ���ø����ֿؼ���Camera
        Camera camera = GameObject.Find("Camera").GetComponentsInChildren<Camera>(true)[0];
        m_goMainUIBottom.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUIBottomLeft.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUIBottomRight.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUITop.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUITopLeft.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goMainUITopRight.GetComponentsInChildren<UIAnchor>(true)[0].uiCamera = camera;
        m_goController.GetComponentsInChildren<ControlStick>(true)[0].RelatedCamera = camera;

        m_myTransform.Find(m_widgetToFullName["MainUIBG"]).gameObject.AddComponent<TouchControll>();

        m_fsNormalAttackCD = m_myTransform.Find(m_widgetToFullName["NormalAttackCD"]).GetComponentsInChildren<UIFilledSprite>(true)[0];

        m_goSelfAttack = m_myTransform.Find(m_widgetToFullName["SelfAttack"]).gameObject;
        m_goCancelManaged = m_myTransform.Find(m_widgetToFullName["CancelManagedBtn"]).gameObject;

        m_lblPlayerBlood = m_myTransform.Find(m_widgetToFullName["MainUIPlayerBloodText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_fsPlayerBloodFG = m_myTransform.Find(m_widgetToFullName["MainUIPlayerBloodFG"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_ssPlayerHead = m_myTransform.Find(m_widgetToFullName["MainUIPlayerHead"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_fsPlayerAnger = m_myTransform.Find(m_widgetToFullName["MainUIPlayerAngerFG"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsPlayerExp = m_myTransform.Find(m_widgetToFullName["MainUIPlayerExpFG"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsAffectCD = m_myTransform.Find(m_widgetToFullName["AffectCD"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsAffectCD.color = new Color(0.5f, 0.5f, 0.5f);
        m_fsOutputCD = m_myTransform.Find(m_widgetToFullName["OutputCD"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsOutputCD.color = new Color(0.5f, 0.5f, 0.5f);
        m_fsMoveCD = m_myTransform.Find(m_widgetToFullName["MoveCD"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsMoveCD.color = new Color(0.5f, 0.5f, 0.5f);
        m_fsSpriteSkillCD = FindTransform("MainUISpriteSkillBtnCD").GetComponentsInChildren<UIFilledSprite>(true)[0];
        m_fsSpriteSkillCD.color = new Color(0.5f, 0.5f, 0.5f);
        m_hpBottleCD = m_myTransform.Find(m_widgetToFullName["HpCD"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        //m_hpBottleCD.atlas = MogoUIManager.Instance.GetSkillIconAtlas();
        m_fsSpecialCD = m_myTransform.Find(m_widgetToFullName["SpecialCD"]).GetComponentInChildren<UIFilledSprite>() as UIFilledSprite;
        m_fsNormalTargetBlood = m_myTransform.Find(m_widgetToFullName["NormalTargetBloodFG"]).GetComponentsInChildren<UIFilledSprite>(true)[0];
        //m_fsBossTargetBlood = m_myTransform.FindChild(m_widgetToFullName["BossTargetBloodFG"]).GetComponentsInChildren<UIFilledSprite>(true)[0];

        m_member1 = m_myTransform.Find(m_widgetToFullName["TeamMember1"]).gameObject;
        m_member2 = m_myTransform.Find(m_widgetToFullName["TeamMember2"]).gameObject;
        m_member3 = m_myTransform.Find(m_widgetToFullName["TeamMember3"]).gameObject;

        m_member1Image = m_myTransform.Find(m_widgetToFullName["TeamMember1LevelBG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_member2Image = m_myTransform.Find(m_widgetToFullName["TeamMember2LevelBG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_member3Image = m_myTransform.Find(m_widgetToFullName["TeamMember3LevelBG"]).GetComponentsInChildren<UISprite>(true)[0];

        m_fsMember1Blood = m_myTransform.Find(m_widgetToFullName["TeamMember1BloodFG"]).GetComponentsInChildren<UIFilledSprite>(true)[0];
        m_fsMember2Blood = m_myTransform.Find(m_widgetToFullName["TeamMember2BloodFG"]).GetComponentsInChildren<UIFilledSprite>(true)[0];
        m_fsMember3Blood = m_myTransform.Find(m_widgetToFullName["TeamMember3BloodFG"]).GetComponentsInChildren<UIFilledSprite>(true)[0];

        m_lblSelfAttack = m_myTransform.Find(m_widgetToFullName["SelfAttackText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_spSelfAttack = m_myTransform.Find(m_widgetToFullName["SelfAttackSprite"]).GetComponentsInChildren<UISprite>(true)[0];

        //m_comboAttack = m_myTransform.FindChild(m_widgetToFullName["ComboAttack"]).GetComponentsInChildren<ComboAttack>(true)[0];
        m_comboAttack = m_myTransform.Find(m_widgetToFullName["ComboAttack"]).gameObject.AddComponent<ComboAttack>();


        m_bloodAnim = m_fsPlayerBloodFG.transform.parent.GetComponentsInChildren<MogoBloodAnim>(true)[0];
        m_bossBloodAnim0 = m_myTransform.Find(m_widgetToFullName["BossTargetBlood"]).GetComponentsInChildren<MogoBloodAnim>(true)[0];
        m_smallBossBloodAnim0 = m_myTransform.Find(m_widgetToFullName["SmallBossTargetBlood"]).GetComponentsInChildren<MogoBloodAnim>(true)[0];

        for (int i = 0; i < 5; ++i)
        {
            //////Mogo.Util.LoggerHelper.Debug(i + " !!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            m_fsBossTargetBloodList[i] = m_myTransform.Find(m_widgetToFullName["BossTargetBloodFG" + i]).GetComponentsInChildren<UIFilledSprite>(true)[0];
            m_fsSmallBossTargetBloodList[i] = m_myTransform.Find(m_widgetToFullName["SmallBossTargetBloodFG" + i]).GetComponentsInChildren<UIFilledSprite>(true)[0];
        }

        // ���Ͻ������ϢPos
        m_goMainUIPlayerInfo = FindTransform("MainUIPlayerInfo").gameObject;
        m_vecMainUIPlayerInfoPosDefault = FindTransform("MainUIPlayerInfoPosDefault").transform.localPosition;
        m_vecMainUIPlayerInfoPosPVP = FindTransform("MainUIPlayerInfoPosPVP").transform.localPosition;

        // ռ�����װ�
        m_goOccupyTowerRank = FindTransform("OccupyTowerRank").gameObject;
        m_lblOccupyTowerRankTitle = FindTransform("OccupyTowerRankTitle").GetComponentsInChildren<UILabel>(true)[0];
        m_listOccupyTowerRank.Clear();
        for (int rank = 1; rank <= MAX_OCCUPY_TOWER_RANK; rank++)
        {
            UILabel rankLabel = FindTransform(string.Format("OccupyTowerRank{0}", rank)).GetComponentsInChildren<UILabel>(true)[0];
            m_listOccupyTowerRank.Add(rankLabel);
        }

        // ռ��ս����Ϣ
        m_goOccupyTowerNotice = FindTransform("OccupyTowerNotice").gameObject;
        m_lblOccupyTowerNoticeCountDownNum = FindTransform("OccupyTowerNoticeCountDownNum").GetComponentsInChildren<UILabel>(true)[0];
        m_spOccupyTowerNoticeEnemyBloodFG = FindTransform("OccupyTowerNoticeEnemyBloodFG").GetComponentsInChildren<UISprite>(true)[0];
        m_spOccupyTowerNoticeOwnBloodFG = FindTransform("OccupyTowerNoticeOwnBloodFG").GetComponentsInChildren<UISprite>(true)[0];
        m_spOccupyTowerNoticeEnemyBloodAnim = FindTransform("OccupyTowerNoticeEnemyBloodAnim").GetComponentsInChildren<UISprite>(true)[0];
        m_spOccupyTowerNoticeOwnBloodAnim = FindTransform("OccupyTowerNoticeOwnBloodAnim").GetComponentsInChildren<UISprite>(true)[0];
        m_lblOccupyTowerNoticeEnemyBloodNum = FindTransform("OccupyTowerNoticeEnemyBloodNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOccupyTowerNoticeOwnBloodNum = FindTransform("OccupyTowerNoticeOwnBloodNum").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOccupyTowerNoticeEnemyScore = FindTransform("OccupyTowerNoticeEnemyScore").GetComponentsInChildren<UILabel>(true)[0];
        m_lblOccupyTowerNoticeOwnScore = FindTransform("OccupyTowerNoticeOwnScore").GetComponentsInChildren<UILabel>(true)[0];
        m_occupyTowerNoticeEnemyBloodAnim = FindTransform("OccupyTowerNoticeEnemyBlood").GetComponentsInChildren<MogoBloodAnim>(true)[0];
        m_occupyTowerNoticeOwnBloodAnim = FindTransform("OccupyTowerNoticeOwnBlood").GetComponentsInChildren<MogoBloodAnim>(true)[0];

        m_lblContributeRankText = m_myTransform.Find(m_widgetToFullName["CurrentRankText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblFirstRank = m_myTransform.Find(m_widgetToFullName["FirstRank"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblSecRank = m_myTransform.Find(m_widgetToFullName["SecondRank"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblTriRank = m_myTransform.Find(m_widgetToFullName["TriRank"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblComboAttackNum = m_myTransform.Find(m_widgetToFullName["ComboAttackNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblDamageNum = m_myTransform.Find(m_widgetToFullName["DamageNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_lblInstanceCountDown = m_myTransform.Find(m_widgetToFullName["InstanceCountDown"]).GetComponentsInChildren<UILabel>(true)[0];

        m_goAutoFight = m_myTransform.Find(m_widgetToFullName["AutoFightBtn"]).gameObject;
        m_goAutoFightText = m_myTransform.Find(m_widgetToFullName["AutoFightBtnText"]).gameObject;

        m_goNormalAttackBtn = m_myTransform.Find(m_widgetToFullName["NormalAttack"]).gameObject;
        m_spNormalAttackFG = m_goNormalAttackBtn.transform.Find("NormalAttackBGUp").GetComponent<UISprite>();
        m_spNormalAttackBG = m_goNormalAttackBtn.transform.Find("NormalAttackBGDown").GetComponent<UISprite>();
        m_goSkill0Btn = m_myTransform.Find(m_widgetToFullName["Move"]).gameObject;
        m_goSkill1Btn = m_myTransform.Find(m_widgetToFullName["Affect"]).gameObject;
        m_goSkill2Btn = m_myTransform.Find(m_widgetToFullName["Output"]).gameObject;
        m_goSkill2Btn.AddComponent<MogoDebugWidget>();

        m_goBottle = m_myTransform.Find(m_widgetToFullName["MainUIItem"]).gameObject;
        m_goSpriteSkillBtn = FindTransform("MainUISpriteSkillBtn").gameObject;
        m_goCommunityBtn = m_myTransform.Find(m_widgetToFullName["Community"]).gameObject;

        UILabel lblText = m_goAutoFightText.transform.GetComponentsInChildren<UILabel>(true)[0];
        if (LanguageData.dataMap.ContainsKey(180000))
        {
            lblText.text = LanguageData.dataMap[180000].content;
        }

        m_goAutoFightStateText = m_myTransform.Find(m_widgetToFullName["AutoFightStateText"]).gameObject;
        m_goAutoFightStateText.SetActive(false);

        // ��ɫ������(Bottom)
        m_goMainUIPlayerExpList = m_myTransform.Find(m_widgetToFullName["MainUIPlayerExpList"]).gameObject;
        m_goGOMainUIPlayerExpListFG = m_myTransform.Find(m_widgetToFullName["GOMainUIPlayerExpListFG"]).gameObject;
        m_lblMainUIPlayerExpListNum = m_myTransform.Find(m_widgetToFullName["MainUIPlayerExpListNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_goMainUIPlayerExpList.SetActive(true);
        AddPlayerExpFG();

        // ����֮����ʾ��Ϣ
        m_goGOClimbTowerCurrent = m_myTransform.Find(m_widgetToFullName["GOClimbTowerCurrent"]).gameObject;
        m_spClimbTowerCurrentNum1 = m_myTransform.Find(m_widgetToFullName["ClimbTowerCurrentNum1"]).GetComponentsInChildren<UISprite>(true)[0];
        m_spClimbTowerCurrentNum2 = m_myTransform.Find(m_widgetToFullName["ClimbTowerCurrentNum2"]).GetComponentsInChildren<UISprite>(true)[0];
        m_spClimbTowerCurrentNum3 = m_myTransform.Find(m_widgetToFullName["ClimbTowerCurrentNum3"]).GetComponentsInChildren<UISprite>(true)[0];

        // ���Ͻǵ���ʱ
        m_goTheCountDown1 = FindTransform("TheCountDown1").gameObject;
        m_goTheCountDown2 = FindTransform("TDCountDown").gameObject;

        m_lblTheCountDown1Num = FindTransform("TheCountDown1Num").GetComponentsInChildren<UILabel>(true)[0];
        m_lblTheCountDown2Num = FindTransform("TDCountDown").GetComponentsInChildren<UILabel>(true)[0];

        m_CDPosClimbTower = FindTransform("CDPosClimbTower").transform.localPosition;

        m_goDiamondProtectBtn = m_myTransform.Find(m_widgetToFullName["DiamondProtectBtn"]).gameObject;
        m_goBuildingProtectBtn = m_myTransform.Find(m_widgetToFullName["BuildingProtectBtn"]).gameObject;       

        // ���鼼��ʩ��
        m_goMainUISpriteSkill = FindTransform("MainUISpriteSkill").gameObject;
        LoadFingerTailUI();

        Initialize();

        MFUIGameObjectPool.GetSingleton().NotRegisterGameObjectList(ID);
        m_myGameObject.SetActive(false);

        if (MogoWorld.thePlayer != null)
        {
            MogoWorld.thePlayer.UpdateSkillToManager();
        }

        #region ����Ѫ��

        go_tdNotice = m_myTransform.Find(m_widgetToFullName["TDNotice"]).gameObject;
        go_tdTip = m_myTransform.Find(m_widgetToFullName["TDTip"]).gameObject;
        sp_tdBlood = m_myTransform.Find(m_widgetToFullName["TDBloodFG"]).gameObject.GetComponentsInChildren<UISprite>(true)[0];
        lbl_tdWave = m_myTransform.Find(m_widgetToFullName["TDWaveText"]).gameObject.GetComponentsInChildren<UILabel>(true)[0];
        lbl_tdTip = m_myTransform.Find(m_widgetToFullName["TDTipText"]).gameObject.GetComponentsInChildren<UILabel>(true)[0];
    
        #endregion
    }
   
    UIAnchor[] temp;
    public override void CallWhenCreate()
    {
        if (SystemConfig.Instance.IsDragMove)
        {
            MogoUIManager.Instance.ChangeSettingToControlStick();
        }
        else
        {
            MogoUIManager.Instance.ChangeSettingToTouch();
        }

        temp = m_myTransform.GetComponentsInChildren<UIAnchor>(true);

        //for (int i = 0; i < temp.Length; ++i)
        //{
        //    temp[i].enabled = false;
        //}

        RegisterButtonHandler("ShortCutCommunityUIBtn");
        SetButtonClickHandler("ShortCutCommunityUIBtn", OnShortCutCommunityUIBtnUp);
    }

    public override void CallWhenShow()
    {
        ShowSpecialSkillFX(true);

        if (temp.Length > 0)
        {
            TimerHeap.AddTimer(2000, 0, () =>
            {
                for (int i = 0; i < temp.Length; ++i)
                {
                    temp[i].enabled = false;
                }
            });
        }
    }

    #endregion    

    #region �¼�

    void Initialize()
    {
        MainUILogicManager.Instance.Initialize();
        m_uiLoginManager = MainUILogicManager.Instance;

        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.TASKUP, OnTaskUp);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.PLAYERINFOBGUP, OnPlayerInfoBGUp);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.NORMALATTACKDOWN, OnNormalAttackDown);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.NORMALATTACTUP, OnNormalAttackUp);

        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.COMMUNITYUP, OnCommunityUp);
        EventDispatcher.AddEventListener(MainUIDict.MainUIEvent.CANCELMANAGEDUP, OnCancelManagedUp);

        MainUIDict.ButtonTypeToEventUp.Add("Special", MainUIDict.MainUIEvent.SPECIALUP);
        MainUIDict.ButtonTypeToEventUp.Add("Move", MainUIDict.MainUIEvent.MOVEUP);
        MainUIDict.ButtonTypeToEventUp.Add("Affect", MainUIDict.MainUIEvent.AFFECTUP);
        MainUIDict.ButtonTypeToEventUp.Add("Output", MainUIDict.MainUIEvent.OUTPUTUP);
        MainUIDict.ButtonTypeToEventUp.Add("NormalAttack", MainUIDict.MainUIEvent.NORMALATTACTUP);
        MainUIDict.ButtonTypeToEventUp.Add("MainUISpriteSkillBtn", MainUIDict.MainUIEvent.SPRITESKILLUP);
        MainUIDict.ButtonTypeToEventUp.Add("Task", MainUIDict.MainUIEvent.TASKUP);
        MainUIDict.ButtonTypeToEventUp.Add("Community", MainUIDict.MainUIEvent.COMMUNITYUP);
        MainUIDict.ButtonTypeToEventUp.Add("MainUIPlayerInfoBG", MainUIDict.MainUIEvent.PLAYERINFOBGUP);
        MainUIDict.ButtonTypeToEventUp.Add("MainUIItem1", MainUIDict.MainUIEvent.MAINUIITEM1BGUP);
        MainUIDict.ButtonTypeToEventUp.Add("TaskInfo", MainUIDict.MainUIEvent.TASKINFOUP);
        MainUIDict.ButtonTypeToEventUp.Add("CancelManagedBtn", MainUIDict.MainUIEvent.CANCELMANAGEDUP);
        m_goAutoFight.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += ActAutoFightDown;
        m_goBuildingProtectBtn.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += BuildingProtectBtnDown;
        m_goDiamondProtectBtn.GetComponentsInChildren<MogoButton>(true)[0].clickHandler += DiamondProtectBtnDown;
    }

    public void Release()
    {
        MainUILogicManager.Instance.Release();

        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.TASKUP, OnTaskUp);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.PLAYERINFOBGUP, OnPlayerInfoBGUp);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.NORMALATTACKDOWN, OnNormalAttackDown);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.NORMALATTACTUP, OnNormalAttackUp);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.CANCELMANAGEDUP, OnCancelManagedUp);
        EventDispatcher.RemoveEventListener(MainUIDict.MainUIEvent.COMMUNITYUP, OnCommunityUp);

        m_goAutoFight.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= ActAutoFightDown;

        m_goBuildingProtectBtn.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= BuildingProtectBtnDown;
        m_goDiamondProtectBtn.GetComponentsInChildren<MogoButton>(true)[0].clickHandler -= DiamondProtectBtnDown;
        MainUIDict.ButtonTypeToEventUp.Clear();
    }

    #endregion

    /// <summary>
    /// ���þ���������ʱ
    /// </summary>
    /// <param name="text"></param>
    /// <param name="bTextType"></param>
    public void SetSelfAttackText(string text, bool bTextType = true)
    {
        if (bTextType)
        {
            m_lblSelfAttack.text = text;
            m_spSelfAttack.gameObject.SetActive(false);
        }
        else
        {
            if (!string.IsNullOrEmpty(text))
            {

                m_spSelfAttack.spriteName = string.Concat("bj-", text);
                m_spSelfAttack.gameObject.SetActive(true);
                m_lblSelfAttack.text = string.Empty;
            }
            else
            {
                m_spSelfAttack.gameObject.SetActive(false);
                m_lblSelfAttack.text = string.Empty;
            }

        }
    }

    bool m_bIsShowNormalAttackCD = false;

    public void EnableNormalAttackCD(bool isEnable)
    {
        m_bIsShowNormalAttackCD = isEnable;
    }
    // Ϊȥ��������ʱ�������´���
    //int m_iComboAttackNum = 0;

    //static int i = 0;
    void Update()
    {
        if (m_bIsShowNormalAttackCD == true)
        {
            if (m_bNormalAttackDown)
            {
                m_fNormalAttackHover += Time.deltaTime;

                m_fsNormalAttackCD.fillAmount = m_fNormalAttackHover / m_fNormalAttackPowerTime;

                if (m_fNormalAttackHover > 0.5f && !m_bChargingStart)
                {
                    m_bChargingStart = true;
                    LoggerHelper.Debug("here to trigger powerupstart");
                    EventDispatcher.TriggerEvent(MainUIDict.MainUIEvent.POWERCHARGESTART);
                }
            }
        }
        UpdateCD();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            List<string> list = new List<string>();
            list.Add("�Ұ�4399");
            list.Add("���˸�ȥ");
            list.Add("fuckme");
            list.Add("�ﱤ��");
            AddShortCutCommunityGrid(list);
        }    
    }

    private void UpdateCD()
    {
        // ������ͨ������CD��ʾ,���о��鼼����CD��ʾ
        // ����1
        if (m_fSpellOneCD > 0)
        {
            m_fSpellOneCnt += Time.deltaTime * 1000;
            int p = (int)((m_fSpellOneCnt / m_fSpellOneCD) * 100);
            int iDown = (int)((m_fSpellOneCD - m_fSpellOneCnt) / 1000);
            if (m_fSpellOneCD % m_fSpellOneCnt > 0)
                iDown += 1;
            if (m_fSpellOneCnt >= m_fSpellOneCD)
            {
                m_fSpellOneCD = 0;
                m_fSpellOneCnt = 0;
                SetOutputCD(0);
                SetOutputCountDown(0);
            }
            else
            {
                SetOutputCD(100 - p);
                SetOutputCountDown(iDown);
            }
        }
        // ����2
        if (m_fSpellTwoCD > 0)
        {
            m_fSpellTwoCnt += Time.deltaTime * 1000;
            int p = (int)((m_fSpellTwoCnt / m_fSpellTwoCD) * 100);
            int iDown = (int)((m_fSpellTwoCD - m_fSpellTwoCnt) / 1000);
            if (m_fSpellTwoCD % m_fSpellTwoCnt > 0)
                iDown += 1;
            if (m_fSpellTwoCnt >= m_fSpellTwoCD)
            {
                m_fSpellTwoCD = 0;
                m_fSpellTwoCnt = 0;
                SetAffectCD(0);
                SetAffectCountDown(0);
            }
            else
            {
                SetAffectCD(100 - p);
                SetAffectCountDown(iDown);
            }
        }
        // ����3
        if (m_fSpellThreeCD > 0)
        {
            m_fSpellThreeCnt += Time.deltaTime * 1000;
            int p = (int)((m_fSpellThreeCnt / m_fSpellThreeCD) * 100);
            int iDown = (int)((m_fSpellThreeCD - m_fSpellThreeCnt) / 1000);
            if (m_fSpellThreeCD % m_fSpellThreeCnt > 0)
                iDown += 1;
            if (m_fSpellThreeCnt >= m_fSpellThreeCD)
            {
                m_fSpellThreeCD = 0;
                m_fSpellThreeCnt = 0;
                SetMoveCD(0);
                SetMoveCountDown(0);
            }
            else
            {
                SetMoveCD(100 - p);
                SetMoveCountDown(iDown);
            }
        }
        // ���鼼��
        if (m_fSpriteSkillCD > 0)
        {
            m_fSpriteSkillCnt += Time.deltaTime * 1000;
            int p = (int)((m_fSpriteSkillCnt / m_fSpriteSkillCD) * 100);
            int iDown = (int)((m_fSpriteSkillCD - m_fSpriteSkillCnt) / 1000);
            if (m_fSpriteSkillCD % m_fSpriteSkillCnt > 0)
                iDown += 1;
            if (m_fSpriteSkillCnt >= m_fSpriteSkillCD)
            {
                m_fSpriteSkillCD = 0;
                m_fSpriteSkillCnt = 0;
                SetSpriteSkillCD(0);
                SetSpriteSkillCountDown(0);
            }
            else
            {
                SetSpriteSkillCD(100 - p);
                SetSpriteSkillCountDown(iDown);
            }
        }
        // Ѫƿ
        if (m_fHPBottleCD > 0)
        {
            m_fHPBottleCnt += Time.deltaTime * 1000;
            int p = (int)((m_fHPBottleCnt / m_fHPBottleCD) * 100);
            SetItem1CD(100 - p);
            if (m_fHPBottleCnt >= m_fHPBottleCD)
            {
                m_fHPBottleCD = 0;
                m_fHPBottleCnt = 0;
                SetItem1CD(0);
            }
        }
    }

    public void ResetUIData()
    {
        m_fNormalAttackHover = 0;
        m_bChargingStart = false;
    }

    public void ResetUIStates()
    {
        //////Mogo.Util.LoggerHelper.Debug("ResetUIStates");
        ResetUIData();

        // ս����ذ�ť
        m_fSpellOneCnt = 0;
        m_fSpellTwoCnt = 0;
        m_fSpellThreeCnt = 0;
        m_fHPBottleCnt = 0;
        m_fSpriteSkillCnt = 0;

        // ս����ذ�ť
        m_fSpellOneCD = 0;
        m_fSpellTwoCD = 0;
        m_fSpellThreeCD = 0;
        m_fHPBottleCD = 0;
        m_fSpriteSkillCD = 0;

        // ս����ذ�ť
        m_fsAffectCD.fillAmount = 0;
        m_fsMoveCD.fillAmount = 0;
        m_fsNormalAttackCD.fillAmount = 0;
        m_fsOutputCD.fillAmount = 0;
        m_hpBottleCD.fillAmount = 0;
        m_fsSpriteSkillCD.fillAmount = 0;

        // ս����ذ�ť
        SetAffectCountDown(0);
        SetMoveCountDown(0);
        SetOutputCountDown(0);
        SetSpriteSkillCountDown(0);

        ShowBossTarget(false);
        ShowMember1(false);
        ShowMember2(false);
        ShowMember3(false);

        ShowBottle(true);
        ShowCommunityBtn(true);
        ShowController(true);
        ShowNormalAttackButton(true);
        ShowPlayerImage(true);
        ShowSkillButton(true, 0);
        ShowSkillButton(true, 1);
        ShowSkillButton(true, 2);
        MainUILogicManager.Instance.ShowSpriteSkillButton();
    }

    public void ActAutoFightDown()
    {
        if (MogoWorld.thePlayer.AutoFight == AutoFightState.RUNNING)
        {
            MogoWorld.thePlayer.AutoFight = AutoFightState.IDLE;
            m_goAutoFightText.GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.dataMap[180000].content;

        }
        else
        {
            MogoWorld.thePlayer.AutoFight = AutoFightState.RUNNING;
            m_goAutoFightText.GetComponentsInChildren<UILabel>(true)[0].text = LanguageData.dataMap[180001].content;

        }
    }
    public void SetHpBottleVisible(bool show)
    {
        //Debug.LogError("SetHpBottleVisible:"+show);
        m_myTransform.Find(m_widgetToFullName["MainUIItem1"]).gameObject.SetActive(show);
    }
    public bool IsHpBottleVisible()
    {
        return m_myTransform.Find(m_widgetToFullName["MainUIItem1"]).gameObject.activeSelf;
    }
    public void UpdateAutoFight(bool isActive)
    {
        if (isActive)
        {
            UILabel lblText = m_goAutoFightText.transform.GetComponentsInChildren<UILabel>(true)[0];
            //lblText.text = "";

            //string msg = String.Empty;
            if (LanguageData.dataMap.ContainsKey(180001))
            {
                lblText.text = LanguageData.dataMap[180001].content;
            }

            m_goAutoFightStateText.SetActive(true);
        }
        else
        {
            UILabel lblText = m_goAutoFightText.transform.GetComponentsInChildren<UILabel>(true)[0];
            //lblText.text = "";

            //string msg = String.Empty;
            if (LanguageData.dataMap.ContainsKey(180000))
            {
                lblText.text = LanguageData.dataMap[180000].content;
            }

            m_goAutoFightStateText.SetActive(false);
        }
    }

    public void DiamondProtectBtnDown()
    { }

    public void BuildingProtectBtnDown()
    { }

    public void ShowController(bool isShow)
    {
        m_goMainUIBottomLeft.SetActive(isShow);
    }

    public void ShowSkillButton(bool isShow, int id)
    {
        switch (id)
        {
            case 0:
                m_goSkill0Btn.SetActive(isShow);
                break;

            case 1:
                m_goSkill1Btn.SetActive(isShow);
                break;

            case 2:
                m_goSkill2Btn.SetActive(isShow);
                break;
        }
    }

    public void ShowSpriteSkillButton(bool isShow)
    {
        m_goSpriteSkillBtn.SetActive(isShow);
    }

    public void ShowNormalAttackButton(bool isShow)
    {
        m_goNormalAttackBtn.SetActive(isShow);
    }

    public void ShowPlayerImage(bool isShow)
    {
        m_goMainUITopLeft.SetActive(isShow);
        m_goMainUIPlayerExpList.SetActive(isShow);
    }

    public void ShowCommunityBtn(bool isShow)
    {
        //m_goCommunityBtn.SetActive(isShow);
        m_goCommunityBtn.SetActive(false);
    }

    public void ShowBottle(bool isShow)
    {
        m_goBottle.SetActive(isShow);
    }

    public void NormalAttackShowAsNormal()
    {
        m_spNormalAttackBG.spriteName = "zdjm_putongjineng_down";
        m_spNormalAttackFG.spriteName = "zdjm_putongjineng_up";
    }

    public void NormalAttackShowAsPower()
    {
        m_spNormalAttackBG.spriteName = "zdjm_nuqi_down";
        m_spNormalAttackFG.spriteName = "zdjm_nuqi_up";
    }

    public void ShowDiamondProtectBtn(bool isShow)
    {
        m_goDiamondProtectBtn.SetActive(isShow);
    }

    public void ShowBuildingProtectBtn(bool isSHow)
    {
        m_goBuildingProtectBtn.SetActive(isSHow);
    }

    public void ShowInstanceCountDown(bool isShow)
    {
        m_lblInstanceCountDown.gameObject.SetActive(isShow);
    }

    public void SetInstanceCountDown(string text)
    {
        m_lblInstanceCountDown.text = text;
    }

    #region ����֮��ս����ʾ��Ϣ

    private GameObject m_goGOClimbTowerCurrent;
    private UISprite m_spClimbTowerCurrentNum1;
    private UISprite m_spClimbTowerCurrentNum2;
    private UISprite m_spClimbTowerCurrentNum3;

    // ����֮����ǰ����
    private int m_climbTowerCurrentNum = 0;
    public int ClimbTowerCurrentNum
    {
        get { return m_climbTowerCurrentNum; }
        set
        {
            m_climbTowerCurrentNum = value;
        }
    }

    /// <summary>
    /// �Ƿ���ʾ����֮����ǰ��Ϣ
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowClimbTowerCurrentInfo(bool isShow)
    {
        m_goGOClimbTowerCurrent.SetActive(isShow);

        if (!isShow)
        {
            ClimbTowerCurrentNum = 0;
        }
    }

    /// <summary>
    /// ��������֮����ǰ����
    /// </summary>
    /// <param name="num"></param>    
    public void SetClimbTowerCurrentNum(int num)
    {
        bool bPlayAnimation = (num == ClimbTowerCurrentNum ? false : true);

        m_spClimbTowerCurrentNum1.gameObject.SetActive(false);
        m_spClimbTowerCurrentNum2.gameObject.SetActive(false);
        m_spClimbTowerCurrentNum3.gameObject.SetActive(false);

        if (num < 10)
        {
            m_spClimbTowerCurrentNum1.spriteName = "vip_" + num;

            PlayClimbTowerCurrentNum1Animation(bPlayAnimation);
        }
        else if (num < 100)
        {
            m_spClimbTowerCurrentNum2.spriteName = "vip_" + num / 10;
            m_spClimbTowerCurrentNum1.spriteName = "vip_" + num % 10;

            PlayClimbTowerCurrentNum1Animation(bPlayAnimation);
            PlayClimbTowerCurrentNum2Animation(bPlayAnimation);
        }
        else
        {
            m_spClimbTowerCurrentNum3.spriteName = "vip_" + num / 100;
            m_spClimbTowerCurrentNum2.spriteName = "vip_" + (num % 100) / 10;
            m_spClimbTowerCurrentNum1.spriteName = "vip_" + num % 10;

            PlayClimbTowerCurrentNum1Animation(bPlayAnimation);
            PlayClimbTowerCurrentNum2Animation(bPlayAnimation);
            PlayClimbTowerCurrentNum3Animation(bPlayAnimation);
        }

        ClimbTowerCurrentNum = num;
    }

    private float CLIMBTOWER_DURATION = 0.15f;

    private void PlayClimbTowerCurrentNum1Animation(bool bPlay = false)
    {
        if (bPlay)
        {
            TweenScale ts = m_spClimbTowerCurrentNum1.GetComponentsInChildren<TweenScale>(true)[0];
            ts.Reset();
            ts.duration = CLIMBTOWER_DURATION;
            ts.Play(true);
        }

        m_spClimbTowerCurrentNum1.gameObject.SetActive(true);
    }

    private void PlayClimbTowerCurrentNum2Animation(bool bPlay = false)
    {
        if (bPlay)
        {
            TweenScale ts = m_spClimbTowerCurrentNum2.GetComponentsInChildren<TweenScale>(true)[0];
            ts.Reset();
            ts.duration = CLIMBTOWER_DURATION;
            ts.Play(true);
        }

        m_spClimbTowerCurrentNum2.gameObject.SetActive(true);
    }

    private void PlayClimbTowerCurrentNum3Animation(bool bPlay = false)
    {
        if (bPlay)
        {
            TweenScale ts = m_spClimbTowerCurrentNum3.GetComponentsInChildren<TweenScale>(true)[0];
            ts.Reset();
            ts.duration = CLIMBTOWER_DURATION;
            ts.Play(true);
        }

        m_spClimbTowerCurrentNum3.gameObject.SetActive(true);
    }

    #endregion

    #region ����ʱ

    // ���Ͻǵ���ʱ
    private GameObject m_goTheCountDown1;
    private UILabel m_lblTheCountDown1Num;

    private GameObject m_goTheCountDown2;
    private UILabel m_lblTheCountDown2Num;

    private Vector3 m_CDPosClimbTower;
    private Vector3 m_CDPosOgreMustDie;

    /// <summary>
    /// ����֮������ʱ,���˱�������ʱ(���Ͻ�)
    /// </summary>   
    private MogoCountDown m_theCountDown1 = null;
    private MogoCountDown m_theCountDown2 = null;

    public void BeginCountDown1(bool isShow, MogoCountDownTarget type = MogoCountDownTarget.None, int theHour = 0, int theMinutes = 0, int theSecond = 0)
    {
        if (isShow)
        {
            switch (type)
            {
                case MogoCountDownTarget.ClimbTower:
                    m_goTheCountDown1.transform.localPosition = m_CDPosClimbTower;

                    m_goTheCountDown1.SetActive(true);
                    if (m_theCountDown1 != null)
                        m_theCountDown1.Release();

                    m_theCountDown1 = new MogoCountDown(m_lblTheCountDown1Num, theHour, theMinutes, theSecond,
                        "", "", "", MogoCountDown.TimeStringType.UpToHour, () =>
                        {

                        });
                    break;

                case MogoCountDownTarget.OgreMustDie:
                    m_goTheCountDown2.transform.localPosition = m_CDPosClimbTower;

                    m_goTheCountDown2.SetActive(true);
                    if (m_theCountDown2 != null)
                        m_theCountDown2.Release();

                    m_theCountDown2 = new MogoCountDown(m_lblTheCountDown2Num, theHour, theMinutes, theSecond,
                        "", "", "", MogoCountDown.TimeStringType.UpToHour, () =>
                        {

                        });
                    break;
            }
        }
        else
        {
            m_goTheCountDown1.SetActive(false);
            if (m_theCountDown1 != null)
                m_theCountDown1.Release();

            m_goTheCountDown2.SetActive(false);
            if (m_theCountDown2 != null)
                m_theCountDown2.Release();
        }
    }

    #endregion

    #region ���˱���

    public void ShowProtectDiamondTip(bool isShow)
    {
        m_myTransform.Find(m_widgetToFullName["ProtectDiamondTip"]).gameObject.SetActive(isShow);
    }

    public void ShowAttackOgreTip(bool isShow)
    {
        m_myTransform.Find(m_widgetToFullName["AttackOgreTip"]).gameObject.SetActive(isShow);
    }

    public void SetAttackOgreTipIcon(string imgName)
    {
        //m_myTransform.FindChild(m_widgetToFullName["AttackOgreTipIcon"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        m_myTransform.Find(m_widgetToFullName["AttackOgreTipIcon"]).GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
    }

    public void SetProtectDiamondTipIcon(string imgName)
    {
        //m_myTransform.FindChild(m_widgetToFullName["ProtectDiamondTipIcon"]).GetComponentsInChildren<UISprite>(true)[0].atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        m_myTransform.Find(m_widgetToFullName["ProtectDiamondTipIcon"]).GetComponentsInChildren<UISprite>(true)[0].spriteName = imgName;
    }

    public void SetAttackOgreTipText(string text)
    {
        m_myTransform.Find(m_widgetToFullName["AttackOgreTipText"]).GetComponentsInChildren<UILabel>(true)[0].text = text;
    }

    public void SetProtectDiamondTipText(string text)
    {
        m_myTransform.Find(m_widgetToFullName["ProtectDiamondTipText"]).GetComponentsInChildren<UILabel>(true)[0].text = text;
    }

    #endregion

    #region ����򿪺͹ر�

    void OnDisable()
    {
        //ShowSpecialSkillIcon(false);
        ShowSpecialSkillFX(false);
        MogoGlobleUIManager.Instance.ShowBattlePlayerBloodTipPanel(false);

        //ResetUIStates();
        //MogoFXManager.Instance.ReleaseAllParticleAnim();
    }

    #endregion

    #region ����

    private List<GameObject> m_listCommunityGrid = new List<GameObject>();
    private List<string> m_listCommunityGridText = new List<string>();

    public void EmptyCommunityGridList()
    {
        for (int i = 0; i < m_listCommunityGrid.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(m_listCommunityGrid[i].gameObject);
        }

        m_listCommunityGrid.Clear();
    }

    public void EmptyCommunityMessageList()
    {
        for (int i = 0; i < 6; ++i)
        {
            GetLabel(string.Concat("ShortCutMessage", i)).gameObject.SetActive(false);
        }

        m_queueMessage.Clear();
    }

    public void AddShortCutCommunityGrid(List<string> shortcutText)
    {
        m_listCommunityGridText = shortcutText;
        for (int i = 0; i < shortcutText.Count; ++i)
        {
            int index = i;
            AssetCacheMgr.GetUIInstance("ShortCutGrid.prefab", (name, id, go) =>
            {
                GameObject gameObj = (GameObject)go;

                MFUIUtils.AttachWidget(gameObj.transform, GetTransform("ShortCutGridList"));

                gameObj.transform.localPosition = new Vector3(0, 48+60 * (index), 0);
                gameObj.transform.localScale = Vector3.one;
                gameObj.transform.Find("ShortCutGridText").GetComponentsInChildren<UILabel>(true)[0].text = shortcutText[index];

                gameObj.GetComponent<MFUIButtonHandler>().ID = index;
                gameObj.GetComponent<MFUIButtonHandler>().ClickHandler = OnShortCutCommunityGridUp;
                m_listCommunityGrid.Add(gameObj);
            });
        }

        GetSprite("ShortCutGridListBG").transform.localScale = new Vector3(300, shortcutText.Count * 61, 1);
    }

    void RefreshShortCutMessageList()
    {
        string[] messageArr = m_queueMessage.ToArray();
        for (int i = 0; i < 6; ++i)
        {
            //Debug.LogError(messageArr[i] + " " + i);
            if (i < m_queueMessage.Count)
            {
                UILabel lbl = GetLabel(string.Concat("ShortCutMessage", i));
                lbl.text = messageArr[i];
                lbl.gameObject.SetActive(true);
            }
            else
            {
                GetLabel(string.Concat("ShortCutMessage", i)).gameObject.SetActive(false);
            }
        }
    }

    Queue<string> m_queueMessage = new Queue<string>();

    public void AddShortCutMessage(string name,int id)
    {
        string message = m_listCommunityGridText[id];
        message = name + message;
        //Debug.LogError(message);
        if (m_queueMessage.Count >= 6)
        {
            m_queueMessage.Dequeue();
            m_queueMessage.Enqueue(message);
        }
        else
        {
            m_queueMessage.Enqueue(message);
        }
        RefreshShortCutMessageList();
    }

    public void ShowShortCutCommunityUI(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform("ShortCutCommunityUI").gameObject);
    }

    public void ShowShortCutGridList(bool isShow)
    {
        MFUIUtils.ShowGameObject(isShow, GetTransform("ShortCutGridList").gameObject);
    }

    public ChannelId CurChannel = ChannelId.TOWER_DEFENCE;
    protected bool m_bIsSendCDing = false;

    void OnShortCutCommunityGridUp(int id)
    {
        if (m_bIsSendCDing)
        {
            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(7526));
            return;
        }
        TimerHeap.AddTimer(5000, 0, () => { m_bIsSendCDing = false; });
        m_bIsSendCDing = true;
        MogoWorld.thePlayer.RpcCall("Chat", (byte)CurChannel, (ulong)0, id.ToString());
        ShowShortCutGridList(false);
    }

    void OnShortCutCommunityUIBtnUp(int id)
    {
        ShowShortCutGridList(!GetTransform("ShortCutGridList").gameObject.activeSelf);
    }

    #endregion

    #region ���鼼��ʩ��

    private GameObject m_goMainUISpriteSkill;    

    /// <summary>
    /// ������ָ����
    /// </summary>
    private GameObject m_goFingerTailUI;
    bool IsFingerTailUILoaded = false;
    public void LoadFingerTailUI()
    {      
        if (!IsFingerTailUILoaded)
        {
            IsFingerTailUILoaded = true;

            AssetCacheMgr.GetUIInstance("FingerTailUI.prefab", (prefab, guid, go) =>
            {
                GameObject obj = (GameObject)go;
                obj.transform.parent = m_goMainUISpriteSkill.transform;
                obj.transform.localPosition = new Vector3(0, 0, 0);
                obj.transform.localScale = new Vector3(1, 1, 1);
                FingerTailUIViewManager view = obj.AddComponent<FingerTailUIViewManager>();
                view.LoadResourceInsteadOfAwake();               
            });
        }
    }

    /// <summary>
    /// �Ƿ���ʾ���鼼���������
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowMainUISpriteSkillPanel(bool isShow)
    {
        m_goMainUISpriteSkill.SetActive(isShow);
    }

    #endregion

    #region ����Ѫ��

    GameObject go_tdNotice;
    UISprite sp_tdBlood;
    UILabel lbl_tdWave;

    GameObject go_tdTip;
    UILabel lbl_tdTip;

    public void ShowTDBlood(bool isShow)
    {
        go_tdNotice.SetActive(isShow);
    }

    public void SetTDBlood(float percentage)
    {
        float p;
        if (percentage > 1)
            p = 1;
        else if (percentage < 0)
            p = 0;
        else
            p = percentage;

        sp_tdBlood.fillAmount = p;
    }

    public void SetTDWaveText(string text)
    {
        lbl_tdWave.text = text;
    }

    public void ShowTDTip(bool isShow, string text = "")
    {
        go_tdTip.SetActive(isShow);
        if (isShow)
        {
            lbl_tdTip.text = text;
        }
        else
        {
            lbl_tdTip.text = String.Empty;
        }
    }

    #endregion

}
