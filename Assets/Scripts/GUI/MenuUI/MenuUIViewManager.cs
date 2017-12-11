/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MenuUIViewManager
// 创建者：MaiFeo
// 修改者列表：Joe Mo
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;

public enum MenuUITab
{
    PlayerTab = 0,
    PackageTab = 1,
    SkillTab = 2,
    TongTab = 3,
    SocialTab = 4,
    SettingsTab = 5,
}

public enum PackageUITab
{
    EquipmentTab = 1,
    JewelTab = 2,
    MaterialTab = 3,
}

public enum PlayerDetailUITab
{
    BasicAttributeTab = 1,
    ElementAttributeTab = 2,
}

public class MenuUIViewManager : MogoUIBehaviour
{
    private static MenuUIViewManager m_instance;
    public static MenuUIViewManager Instance { get { return MenuUIViewManager.m_instance; }}   

    #region 变量

    public static Dictionary<string, string> ButtonTypeToEventDown = new Dictionary<string, string>();
    public bool IsNormalMainUI = false;
    private GameObject m_goEquipmentModel;

    public const int VOLUME = 40;
    public const int DIAMONDHOLENUM = 13;

    private Dictionary<int, UILabel> m_menuTabUpLabelList = new Dictionary<int, UILabel>();
    private Dictionary<int, UILabel> m_menuTabDownLabelList = new Dictionary<int, UILabel>();
    private Dictionary<int, UILabel> m_packageTabLabelList = new Dictionary<int, UILabel>();
    private Dictionary<int, UILabel> m_playerDetailTabLabelList = new Dictionary<int, UILabel>();

    private UILabel m_packageGold;
    private UILabel m_packageDiamon;
    private UILabel m_packageVolume;
    private MyDragableCamera m_packageDragableCamera;
    private MogoSingleButtonList m_packageIcoSingleButtonList;

    private UILabel m_playerName;
    private UILabel m_playerLevel;
    private UILabel m_playerNameAndLevel;

    private SimpleDragCamera m_equipTipCamera;
    private UILabel m_equipDetailName;
    private UILabel m_equipDetailNeedLevel;
    private UILabel m_equipDetailGrowLevel;
    private UISlicedSprite m_equipDetailImageBG;
    private UILabel m_equipDetailLblEquip;
    private UILabel m_equipDetailNeedJob;
    private UILabel m_equipDetailExtra;
    private UISlicedSprite m_equipDetailImageFG;
    private UISlicedSprite m_equipDetailImageUsed;
    private UILabel[] m_equipDetailDiamondHoleInfo = new UILabel[DIAMONDHOLENUM];

    private UILabel m_equipCurrentDetailName;
    private SimpleDragCamera m_equipCurrentTipCamera;
    private UILabel m_equipCurrentDetailNeedLevel;
    private UILabel m_equipCurrentDetailGrowLevel;
    private UILabel m_equipCurrentDetailNeedJob;
    private UILabel m_equipCurrentDetailExtra;
    private UISlicedSprite m_equipCurrentDetailImageFG;
    private UISlicedSprite m_equipCurrentDetailImageBG;
    private UISlicedSprite m_equipCurrentDetailImageUsed;
    private UILabel[] m_equipCurrentDetailDiamondHoleInfo = new UILabel[DIAMONDHOLENUM];

    private UILabel m_lblDialogTitle;

    //private UILabel m_playerDetailBlood;
    //private UILabel m_playerDetailBrokenLevel;
    //private UILabel m_playerDetailCriticalLevel;
    //private UILabel m_playerDetailHitupChance;
    //private UILabel m_playerDetailPhsicalAttack;
    //private UILabel m_playerDetailEarthElemDamage;
    //private UILabel m_playerDetailWindElemDamage;
    //private UILabel m_playerDetailWaterElemDamage;
    //private UILabel m_playerDetailFireElemDamage;
    //private UILabel m_playerDetailExtraCritial;
    //private UILabel m_playerDetailPhsicalDefense;
    //private UILabel m_playerDetailMagicalDefense;
    //private UILabel m_playerDetailEarthElemResistance;
    //private UILabel m_playerDetailWindElemResistance;
    //private UILabel m_playerDetailWaterElemResistance;
    //private UILabel m_playerDetailFireElemResistance;

    private UILabel m_playerDetailCDDecrease;
    private UILabel m_playerDetailCrit;
    private UILabel m_playerDetailDamage;
    private UILabel m_playerDetailHit;
    private UILabel m_playerDetailPVP;
    private UILabel m_playerDetailWreck;
    private UILabel m_playerDetailCritResistance;
    private UILabel m_playerDetailDefense;
    private UILabel m_playerDetailHealth;
    private UILabel m_playerDetailPVE;
    private UILabel m_playerDetailSpeedUp;
    private UILabel m_playerDetailWreckResistance;

    private UILabel m_playerDetailPiercing;
    private UILabel m_playerDetailExplosionBonus;

    private UILabel m_playerDetailEarthElemDamage;
    private UILabel m_playerDetailWindElemDamage;
    private UILabel m_playerDetailWaterElemDamage;
    private UILabel m_playerDetailFireElemDamage;
    private UILabel m_playerDetailGoldElemDamage;

    private UILabel m_playerDetailEarthElemResistance;
    private UILabel m_playerDetailWindElemResistance;
    private UILabel m_playerDetailWaterElemResistance;
    private UILabel m_playerDetailFireElemResistance;
    private UILabel m_playerDetailGoldElemResistance;

    private UIFilledSprite m_playerInfoEXP;
    private UIFilledSprite m_playerInfoEnergy;

    private UILabel m_playerInfoExpNum;
    private UILabel m_playerInfoEnergyNum;
    private UILabel m_playerInfoJob;
    private UILabel m_playerInfoHealth;
    private UILabel m_playerInfoDamage;
    private UILabel m_playerInfoDefense;
    private UILabel m_playerInfoPower;

    private UILabel m_equipSlotBelt;
    private UILabel m_equipSlotBreast;
    private UILabel m_equipSlotHand;
    private UILabel m_equipSlotHead;
    private UILabel m_equipSlotCuish;
    private UILabel m_equipSlotNecklace;
    private UILabel m_equipSlotRingLeft;
    private UILabel m_equipSlotRingRight;
    private UILabel m_equipSlotShoes;
    private UILabel m_equipSlotShouders;
    private UILabel m_equipSlotWeapon;

    private UILabel m_diamondTipLevel;
    private UILabel m_diamondTipType;
    private UILabel m_diamondTipDesc;
    private UILabel m_diamondTipName;
    private UILabel m_diamondMaxStack;
    private UISprite m_diamondTipIconBg;
    private UISlicedSprite m_ssDiamondTipIcon;

    private UILabel m_materialTipName;
    private UILabel m_materialTipLevel;
    private UILabel m_materialTipDecs;
    private UILabel m_materialTipStack;
    private UILabel m_materialTipPrice;
    private UISprite m_materialTipPriceIcon;
    private UISlicedSprite m_materialTipIcon;
    private UISprite m_materialTipIconBg;
    private GameObject m_materialTip;

    private UILabel m_propTipName;
    private UILabel m_propTipDecs;
    private UILabel m_propTipStack;
    private UILabel m_propTipPrice;
    private UISprite m_propTipPriceIcon;
    private UISlicedSprite m_propTipIcon;
    private UISprite m_propTipIconBg;
    private GameObject m_propTip;
    private GameObject m_propTipUseBtn;

    //private GameObject m_packageInfoDetailPlayer;
    //private GameObject m_packageInfoDetailPackage;
    private GameObject m_diamondInfoTip;
    //社交提示
    private GameObject m_socialTip;

    private GameObject[] m_arrNewDiamondHoleIcon = new GameObject[4];
    private GameObject[] m_arrCurrentDiamondHoleIcon = new GameObject[4];

    private List<GameObject> m_lisgGridGO;
    private List<UISlicedSprite> m_listGridFG;
    private List<UISlicedSprite> m_listGridBG;
    private List<UILabel> m_listGridNum;
    private List<UISlicedSprite> m_listGridUp;
    private List<UISprite> m_listGridUpBl;    

    private GameObject basicAttr;
    private GameObject elemAttr;

    private GameObject m_goPackageItemGridList;

    private UIAtlas m_jobAtlas;
    private UITexture m_texShowCharacterWinBG;

    #endregion

    #region 公有方法
   
    public void SetPackageGold(string gold)
    {
        m_packageGold.text = gold;
    }

    public void SetPackageDiamond(string diamond)
    {
        m_packageDiamon.text = diamond;
    }

    public void SetPackageItemCount(string count)
    {
        m_packageVolume.text = string.Concat(count, "/" + VOLUME);
    }

    public void SetPackageGridImage(string name, int id, int color = -1)
    {
        m_listGridFG[id].atlas = MogoUIManager.Instance.GetAtlasByIconName(name);
        m_listGridFG[id].spriteName = name;
        SetImageColor(m_listGridFG[id], color);
    }

    public void SetImageColor(UISprite sprite, int color)
    {
        if (color == -1) return;

        switch (color)
        {
            case 0: sprite.ShowAsWhiteBlack(false); break;
            case 1: sprite.ShowAsLakeBlue(); break;
            case 2: sprite.ShowAsGreen(); break;
            case 3: sprite.ShowAsDeepBlue(); break;
            case 4: sprite.ShowAsPurpose(); break;
            case 5: sprite.ShowAsOrange(); break;
            case 6: sprite.ShowAsRed(); break;
            case 7: sprite.ShowAsYellow(); break;
            case 8: sprite.ShowAsRoseRed(); break;
            case 9: sprite.ShowAsGrassGreen(); break;
            default: break;
        }

        //Mogo.Util.LoggerHelper.Debug(color + " !!!!!!!!!!!!!!!!!!!!!!!!");
    }

    public void SetPackageGridBG(string name, int id)
    {
        m_listGridBG[id].spriteName = name;
    }

    public void SetPackageGridNum(string num, int id)
    {
        m_listGridNum[id].text = num;
    }

    public void ShowPackageGridUpSign(int id, bool isShow = true)
    {
        m_listGridUp[id].transform.parent.gameObject.SetActive(isShow);
    }

    public void SetPackageGridNumVisible(int id, bool isShow = true)
    {
        m_listGridNum[id].gameObject.SetActive(isShow);
    }

    public void RemovePackageItem(int gridIndex)
    {
        m_listGridFG[gridIndex].atlas = MogoUIManager.Instance.CommonAtlas;
        m_listGridFG[gridIndex].spriteName = Mogo.GameData.IconData.none;
        m_listGridBG[gridIndex].spriteName = Mogo.GameData.IconData.blankBox;

        m_listGridFG[gridIndex].ShowAsWhiteBlack(false);

        m_listGridUpBl[gridIndex].gameObject.SetActive(false);
        ShowPackageGridUpSign(gridIndex, false);
        SetPackageGridNumVisible(gridIndex, false);
    }

    public void ClearPackageItems()
    {
        for (int i = 0; i < VOLUME; ++i)
        {
            RemovePackageItem(i);
        }
    }

    public void ClearPackgeItemsInPage(int page)
    {
        for (int i = page * 10; i < (page + 1) * 10; ++i)
        {
            RemovePackageItem(i);
        }
    }

    public void SetEquipDetailInfoName(string name)
    {
        m_equipDetailName.text = name;
    }

    public void SetEquipCurrentDetailInfoName(string name)
    {
        m_equipCurrentDetailName.text = name;
    }

    public void SetEquipDetailInfoNeedLevel(int level)
    {
        m_equipDetailNeedLevel.text = level.ToString();
    }

    public void SetEquipCurrentDetailInfoNeedLevel(int level)
    {
        m_equipCurrentDetailNeedLevel.text = level.ToString();
    }

    public void SetEquipDetailInfoGrowLevel(string level)
    {
        m_equipDetailGrowLevel.text = level;
    }

    public void SetEquipCurrentDetailInfoGrowLevel(string level)
    {
        m_equipCurrentDetailGrowLevel.text = level;
    }

    public void SetEquipDetailInfoNeedJob(string job)
    {
        m_equipDetailNeedJob.text = job;
    }

    public void SetEquipCurrentDetailInfoNeedJob(string job)
    {
        m_equipCurrentDetailNeedJob.text = job;
    }

    public void SetEquipDetailInfoExtra(string text)
    {
        m_equipDetailExtra.text = text;
    }

    public void SetEquipCurrentDetailInfoExtra(string text)
    {
        m_equipCurrentDetailExtra.text = text;
    }

    public void SetEquipDetailInfoImage(string imgName)
    {
        m_equipDetailImageFG.spriteName = imgName;
    }

    public void SetEquipCurrentDetailInfoImage(string imgName)
    {
        m_equipCurrentDetailImageFG.spriteName = imgName;
    }

    public void ShowEquipDetailInfoImageUsed(bool isShow)
    {
        m_equipDetailImageUsed.gameObject.SetActive(isShow);
    }

    public void SetEquipDetailInfoImageBg(string p)
    {
        m_equipDetailImageBG.spriteName = p;
    }

    public void SetEquipCurrentDetailInfoImageBg(string p)
    {
        m_equipCurrentDetailImageBG.spriteName = p;
    }

    public void ShowEquipCurrentDetailInfoImageUsed(bool isShow)
    {
        m_equipCurrentDetailImageUsed.gameObject.SetActive(isShow);
    }

    public void SetDiamondHoleInfo(string text, int holeIndex)
    {
        m_equipDetailDiamondHoleInfo[holeIndex].text = text;
    }

    public void SetDiamondHoleCurrentInfo(string text, int holeIndex)
    {
        m_equipCurrentDetailDiamondHoleInfo[holeIndex].text = text;
    }

    public void SetPlayerDetailCDReduce(uint CDDecrease)
    {
        m_playerDetailCDDecrease.text = (float)CDDecrease / 10000 + "%";
    }

    public void SetPlayerDetailCrit(uint crit)
    {
        m_playerDetailCrit.text = crit.ToString();
    }

    public void SetPlayerDetailAttack(uint atk)
    {
        m_playerDetailDamage.text = atk.ToString();
    }

    public void SetPlayerDetailHit(uint hit)
    {
        m_playerDetailHit.text = hit.ToString();
    }

    public void SetPlayerDetailPVPAddition(uint PVP)
    {
        m_playerDetailPVP.text = PVP.ToString();
    }

    public void SetPlayerDetailTrueStrike(uint wreck)
    {
        m_playerDetailWreck.text = wreck.ToString();
    }

    public void SetPlayerDetailWindDamage(uint damage)
    {
        m_playerDetailWindElemDamage.text = damage.ToString();
    }

    public void SetPlayerDetailWaterDamage(uint damage)
    {
        m_playerDetailWaterElemDamage.text = damage.ToString();
    }

    public void SetPlayerDetailFireDamage(uint damage)
    {
        m_playerDetailFireElemDamage.text = damage.ToString();
    }

    public void SetPlayerDetailEarthDamage(uint damage)
    {
        m_playerDetailEarthElemDamage.text = damage.ToString();
    }

    public void SetPlayerDetailAllElementsDamage(uint damage)
    {
        m_playerDetailGoldElemDamage.text = damage.ToString();
    }

    public void SetPlayerDetailAntiCrit(uint critResistance)
    {
        m_playerDetailCritResistance.text = critResistance.ToString();
    }

    public void SetPlayerDetailDefense(uint defense)
    {
        m_playerDetailDefense.text = defense.ToString();
    }

    public void SetPlayerDetailHealth(uint health)
    {
        m_playerDetailHealth.text = health.ToString();
    }

    public void SetPlayerDetailPVE(uint PVE)
    {
        m_playerDetailPVE.text = PVE.ToString();
    }

    public void SetPlayerDetailSpeedAddRate(uint speedUp)
    {
        m_playerDetailSpeedUp.text = (float)speedUp / 10000 + "%";
    }

    public void SetPlayerDetailAntiTrueStrike(uint wreckResistance)
    {
        m_playerDetailWreckResistance.text = wreckResistance.ToString();
    }

    public void SetPlayerDetailAntiDefense(uint piercing)
    {
        m_playerDetailPiercing.text = piercing.ToString();
    }

    public void SetPlayerDetailCritExtraAttack(uint explosionBonus)
    {
        m_playerDetailExplosionBonus.text = explosionBonus.ToString();
    }

    public void SetPlayerDetailEarthDefense(uint resistence)
    {
        m_playerDetailEarthElemResistance.text = resistence.ToString();
    }

    public void SetPlayerDetailWindDefense(uint resistence)
    {
        m_playerDetailWindElemResistance.text = resistence.ToString();
    }

    public void SetPlayerDetailWaterDefense(uint resistence)
    {
        m_playerDetailWaterElemResistance.text = resistence.ToString();
    }

    public void SetPlayerDetailFireDefense(uint resistence)
    {
        m_playerDetailFireElemResistance.text = resistence.ToString();
    }

    public void SetPlayerAllElementsDefense(uint resistence)
    {
        m_playerDetailGoldElemResistance.text = resistence.ToString();
    }

    public void SetPlayerInfoExp(float exp)
    {
        m_playerInfoEXP.fillAmount = exp;
    }

    public void SetPlayerInfoEnergy(float energy)
    {
        m_playerInfoEnergy.fillAmount = energy;
    }

    public void SetPlayerInfoExpNum(string num)
    {
        m_playerInfoExpNum.text = num;
    }

    public void SetPlayerInfoEnergyNum(string num)
    {
        m_playerInfoEnergyNum.text = num;
    }

    public void SetPlayerInfoJob(string job)
    {
        m_playerInfoJob.text = job;
    }

    public void SetPlayerInfoHealth(uint health)
    {
        m_playerInfoHealth.text = health.ToString();
    }

    public void SetPlayerInfoAttack(uint atk)
    {
        m_playerInfoDamage.text = atk.ToString();
    }

    public void SetPlayerInfoDefense(uint defense)
    {
        m_playerInfoDefense.text = defense.ToString();
    }

    public void SetPlayerInfoPower(uint power)
    {
        m_playerInfoPower.text = power.ToString();
    }

    public void ShowPackageDetailInfo(bool show)
    {
        m_myTransform.Find(m_widgetToFullName["PackageEquipInfo"]).gameObject.SetActive(show);
        m_equipTipCamera.Reset();
        m_equipCurrentTipCamera.Reset();
    }

    public void ShowPacakgeCurrentDetailInfo(bool show)
    {
        m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfo"]).gameObject.SetActive(show);

        if (show)
        {
            m_equipDetailLblEquip.text = LanguageData.GetContent(47281); // 替换
            m_myTransform.Find(m_widgetToFullName["PacakgeEquipNewInfo"]).localPosition =
                m_myTransform.Find(m_widgetToFullName["PackageEquipNewInfoPos"]).localPosition;
        }
        else
        {
            m_equipDetailLblEquip.text = LanguageData.GetContent(47282); // 装备
            m_myTransform.Find(m_widgetToFullName["PacakgeEquipNewInfo"]).localPosition = Vector3.zero;
        }
    }

    #region 玩家装备

    public void SetPlayerBeltFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["BeltFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(temp, color);
    }

    public void SetPlayerBreastPlateFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["BreastPlateFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["BreastPlateFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerCuishFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["CuishFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["CuishFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerHandGuardFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["HandGuardFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["HandGuardFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerHeadEquipFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["HeadEquipFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["HeadEquipFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerNecklaceFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["NecklaceFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["NecklaceFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerWingFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["WingFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(temp, color);
    }

    public void SetPlayerRingLeftFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["RingLeftFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["RingLeftFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerRingRightFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["RingRightFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["RingRightFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerShoesFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["ShoesFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["ShoesFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerShoudersFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["ShoudersFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["ShoudersFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerWeaponFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["WeaponFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
        Mogo.Util.LoggerHelper.Debug(temp.atlas + " " + imgName);
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["WeaponFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerBeltBG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["BeltBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(temp, color);

    }

    public void SetPlayerBreastPlateBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["BreastPlateBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["BreastPlateBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerCuishBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["CuishBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["CuishBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerHandGuardBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["HandGuardBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["HandGuardBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerHeadEquipBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["HeadEquipBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["HeadEquipBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerNecklaceBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["NecklaceBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["NecklaceBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerWingBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["WingBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["WingBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerRingLeftBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["RingLeftBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["RingLeftBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerRingRightBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["RingRightBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["RingRightBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerShoesBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["ShoesBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["ShoesBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerShoudersBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["ShoudersBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["ShoudersBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    public void SetPlayerWeaponBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["WeaponBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["WeaponBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    #endregion

    public void ShowNewDiamondHoleIcon(int id, bool isShow)
    {
        m_arrNewDiamondHoleIcon[id - 9].SetActive(isShow);
    }

    public void ShowCurrentDiamondHoleIcon(int id, bool isShow)
    {

        m_arrCurrentDiamondHoleIcon[id - 9].SetActive(isShow);
    }

    #region 装备tip
    public const float GAP = 30;
    public List<GameObject> gos = new List<GameObject>();
    public void ShowEquipInfoDetail
        (List<string> attrs, List<string> jewels, string level, string vocation)
    {
        float gap = 0;
        foreach (GameObject go in gos)
        {
            AssetCacheMgr.ReleaseInstance(go);
        }
        gos.Clear();

        Transform root = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailList"]);
        //加载+排版

        int i = 0;
        //属性
        for (i = 0; i < attrs.Count; i++)
        {
            var index = i;
            AssetCacheMgr.GetUIInstance("PackageEquipInfoAttr.prefab",
            (prefab, guid, gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole0Text").GetComponent<UILabel>();
                lable.text = attrs[index];

                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(position.x, position.y - index * GAP, position.z);
                gos.Add(go);
            }
            );
        }
        gap -= i * GAP - 30;

        //宝石
        for (i = 0; i < jewels.Count; i++)
        {
            var index = i;
            AssetCacheMgr.GetUIInstance("PackageEquipInfoDiamon.prefab",
            (prefab, guid, gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole12Text").GetComponent<UILabel>();
                lable.text = jewels[index];

                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(position.x, gap + position.y - index * GAP, position.z);
                gos.Add(go);
            }
            );
        }
        gap -= i * GAP + 30;

        //需求等级等
        Transform detail3 = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetail3"]);
        detail3.localPosition = new Vector3(detail3.localPosition.x, gap, detail3.localPosition.z);
        m_equipDetailNeedJob.GetComponent<UILabel>().text = vocation;
        m_equipDetailNeedLevel.GetComponent<UILabel>().text = level;

        gap -= 200;
        gap = -gap;

        m_equipTipCamera.height = gap - 500;
        //show
        ShowPackageDetailInfo(true);

    }

    public void ShowCurrentEquipInfoDetail
         (List<string> attrs, List<string> jewels, string level, string vocation)
    {
        float gap = 0;

        Transform root = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoList"]);
        //加载+排版

        int i = 0;
        //属性
        for (i = 0; i < attrs.Count; i++)
        {
            var index = i;
            AssetCacheMgr.GetUIInstance("PackageEquipInfoAttr.prefab",
            (prefab, guid, gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole0Text").GetComponent<UILabel>();
                //LoggerHelper.Debug("attrs[i]" + attrs[index]);
                lable.text = attrs[index];//;

                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(position.x, position.y - index * GAP, position.z);
                gos.Add(go);
            }
            );
        }
        gap -= i * GAP - 30;

        //宝石
        for (i = 0; i < jewels.Count; i++)
        {
            var index = i;
            AssetCacheMgr.GetUIInstance("PackageEquipInfoDiamon.prefab",
            (prefab, guid, gameObject) =>
            {
                //PackageEquipInfoDiamonHole0Text
                GameObject go = gameObject as GameObject;
                UILabel lable = go.transform.Find("PackageEquipInfoDiamonHole12Text").GetComponent<UILabel>();
                lable.text = jewels[index];

                Vector3 scale = go.transform.localScale;
                Vector3 position = go.transform.localPosition;
                Vector3 angle = go.transform.localEulerAngles;
                go.transform.parent = root;
                go.transform.localScale = scale;
                go.transform.localEulerAngles = angle;
                go.transform.localPosition = new Vector3(position.x, gap + position.y - index * GAP, position.z);
                gos.Add(go);
            }
            );
        }
        gap -= i * GAP + 30;

        //需求等级等
        Transform detail3 = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoDetail3"]);
        detail3.localPosition = new Vector3(detail3.localPosition.x, gap, detail3.localPosition.z);
        m_equipCurrentDetailNeedJob.GetComponent<UILabel>().text = vocation;
        m_equipCurrentDetailNeedLevel.GetComponent<UILabel>().text = level;

        gap -= 200;
        gap = -gap;

        m_equipCurrentTipCamera.height = gap - 500;
        //show
        ShowPacakgeCurrentDetailInfo(true);
    }

    #endregion

    #endregion

    #region 私有方法

    void Awake()
    {
        m_instance = this;
        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        Initialize();

        m_lisgGridGO = new List<GameObject>();
        m_listGridFG = new List<UISlicedSprite>();
        m_listGridBG = new List<UISlicedSprite>();
        m_listGridUp = new List<UISlicedSprite>();
        m_listGridUpBl = new List<UISprite>();
        m_listGridNum = new List<UILabel>();

        m_equipDetailLblEquip = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailPutUpText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_packageGold = m_myTransform.Find(m_widgetToFullName["GoldText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_packageDiamon = m_myTransform.Find(m_widgetToFullName["DiamonText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_packageVolume = m_myTransform.Find(m_widgetToFullName["PackageInfoNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_packageDragableCamera = m_myTransform.Find(m_widgetToFullName["PackageCamera"]).GetComponentsInChildren<MyDragableCamera>(true)[0];
        m_packageDragableCamera.OutOfBoundsMaxPage += OnOutOfBoundsMaxPage;
        m_packageDragableCamera.OutOfBoundsMinPage += OnOutOfBoundsMinPage;
        m_packageIcoSingleButtonList = FindTransform("PackageIconList").GetComponentsInChildren<MogoSingleButtonList>(true)[0];

        m_playerName = m_myTransform.Find(m_widgetToFullName["PlayerNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerLevel = m_myTransform.Find(m_widgetToFullName["PlayerLevelText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerNameAndLevel = m_myTransform.Find(m_widgetToFullName["PlayerNameAndLevelText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_equipDetailName = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailNeedLevel = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailNeedLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        //m_equipDetailGrowLevel = m_myTransform.FindChild(m_widgetToFullName["PackageEquipInfoDetailGrowLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailNeedJob = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailNeedJobType"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailExtra = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailExtraText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipDetailImageFG = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipDetailImageBG = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailImageBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipDetailImageUsed = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailImageUsed"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipTipCamera = m_myTransform.Find(m_widgetToFullName["PackageEquipInfoDetailBG"]).GetComponentsInChildren<SimpleDragCamera>(true)[0];

        m_equipCurrentDetailImageBG = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoDetailImageBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipCurrentDetailName = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipCurrentDetailNeedLevel = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoDetailNeedLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        //m_equipCurrentDetailGrowLevel = m_myTransform.FindChild(m_widgetToFullName["PackageEquipCurrentInfoDetailGrowLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipCurrentDetailNeedJob = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoDetailNeedJobType"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipCurrentDetailExtra = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoDetailExtraText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipCurrentDetailImageFG = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipCurrentDetailImageUsed = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoDetailImageUsed"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_equipCurrentTipCamera = m_myTransform.Find(m_widgetToFullName["PackageEquipCurrentInfoDetailBG"]).GetComponentsInChildren<SimpleDragCamera>(true)[0];

        m_playerDetailCDDecrease = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeCDDecreaseNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailCrit = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeCritNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailDamage = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeDamageNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailHit = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeHitNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailPVP = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributePVPNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailWreck = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeWreckNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_playerDetailPiercing = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributePiercingNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailExplosionBonus = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeExplosionBonusNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_playerDetailCritResistance = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeCritResistanceNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailDefense = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeDefenceNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailHealth = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeHealthNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailPVE = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributePVENum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailSpeedUp = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeSpeedUpNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailWreckResistance = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeWreckResistanceNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_playerDetailEarthElemDamage = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeEarthElemDamageNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailWindElemDamage = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeWindElemDamageNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailWaterElemDamage = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeWaterElemDamageNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailFireElemDamage = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeFireElemDamageNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailGoldElemDamage = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeGoldElemDamageNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_playerDetailEarthElemResistance = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeEarthElemResistanceNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailWindElemResistance = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeWindElemResistanceNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailWaterElemResistance = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeWaterElemResistanceNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailFireElemResistance = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeFireElemResistenceNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerDetailGoldElemResistance = m_myTransform.Find(m_widgetToFullName["PlayerDetailAttributeGoldElemResistanceNum"]).GetComponentsInChildren<UILabel>(true)[0];

        //m_packageInfoDetailPackage = m_myTransform.FindChild(m_widgetToFullName["PackageEquipInfoDetailPackage"]).gameObject;
        //m_packageInfoDetailPlayer = m_myTransform.FindChild(m_widgetToFullName["PackageEquipInfoDetailPlayer"]).gameObject;


        m_playerInfoEXP = m_myTransform.Find(m_widgetToFullName["PlayerExpFG"]).GetComponentsInChildren<UIFilledSprite>(true)[0];
        m_playerInfoEnergy = m_myTransform.Find(m_widgetToFullName["PlayerEnergyFG"]).GetComponentsInChildren<UIFilledSprite>(true)[0];

        m_playerInfoExpNum = m_myTransform.Find(m_widgetToFullName["PlayerExpNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerInfoEnergyNum = m_myTransform.Find(m_widgetToFullName["PlayerEnergyNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerInfoJob = m_myTransform.Find(m_widgetToFullName["PlayerJobName"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerInfoHealth = m_myTransform.Find(m_widgetToFullName["PlayerBloodNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerInfoDamage = m_myTransform.Find(m_widgetToFullName["PlayerAttackNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerInfoDefense = m_myTransform.Find(m_widgetToFullName["PlayerDefenceNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_playerInfoPower = m_myTransform.Find(m_widgetToFullName["PlayerPowerInfoNum"]).GetComponentsInChildren<UILabel>(true)[0];


        m_diamondTipLevel = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_diamondTipType = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailTypeNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_diamondTipDesc = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailEffectNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_diamondTipName = m_myTransform.Find(m_widgetToFullName["DiamondInfoNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_ssDiamondTipIcon = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_diamondTipIconBg = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailImageBG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_diamondMaxStack = m_myTransform.Find(m_widgetToFullName["DiamondInfoDetailMaxStackNum"]).GetComponentsInChildren<UILabel>(true)[0];

        m_diamondInfoTip = m_myTransform.Find(m_widgetToFullName["DiamondInfoTip"]).gameObject;
        m_socialTip = m_myTransform.Find(m_widgetToFullName["SocialTip"]).gameObject;

        m_materialTipName = m_myTransform.Find(m_widgetToFullName["MaterialInfoNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_materialTipLevel = m_myTransform.Find(m_widgetToFullName["MaterialInfoDetailLevelNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_materialTipStack = m_myTransform.Find(m_widgetToFullName["MaterialInfoDetailStackNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_materialTipDecs = m_myTransform.Find(m_widgetToFullName["MaterialInfoDetailEffectNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_materialTipPrice = m_myTransform.Find(m_widgetToFullName["MaterialInfoDetailPriceNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_materialTipPriceIcon = m_myTransform.Find(m_widgetToFullName["MaterialInfoDetailPriceIcon"]).GetComponentsInChildren<UISprite>(true)[0];
        m_materialTipIcon = m_myTransform.Find(m_widgetToFullName["MaterialInfoDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_materialTipIconBg = m_myTransform.Find(m_widgetToFullName["MaterialInfoDetailImageBG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_materialTip = m_myTransform.Find(m_widgetToFullName["MaterialInfoTip"]).gameObject;

        m_propTipName = m_myTransform.Find(m_widgetToFullName["PropInfoNameText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_propTipStack = m_myTransform.Find(m_widgetToFullName["PropInfoDetailStackNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_propTipDecs = m_myTransform.Find(m_widgetToFullName["PropInfoDetailEffectNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_propTipPrice = m_myTransform.Find(m_widgetToFullName["PropInfoDetailPriceNum"]).GetComponentsInChildren<UILabel>(true)[0];
        m_propTipPriceIcon = m_myTransform.Find(m_widgetToFullName["PropInfoDetailPriceIcon"]).GetComponentsInChildren<UISprite>(true)[0];
        m_propTipIcon = m_myTransform.Find(m_widgetToFullName["PropInfoDetailImageFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        m_propTipIconBg = m_myTransform.Find(m_widgetToFullName["PropInfoDetailImageBG"]).GetComponentsInChildren<UISprite>(true)[0];
        m_propTip = m_myTransform.Find(m_widgetToFullName["PropInfoTip"]).gameObject;
        m_propTipUseBtn = m_myTransform.Find(m_widgetToFullName["PropInfoDetailCompose"]).gameObject;


        m_equipSlotBelt = m_myTransform.Find(m_widgetToFullName["BeltText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotBreast = m_myTransform.Find(m_widgetToFullName["BreastPlateText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotCuish = m_myTransform.Find(m_widgetToFullName["CuishText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotHand = m_myTransform.Find(m_widgetToFullName["HandGuardText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotHead = m_myTransform.Find(m_widgetToFullName["HeadEquipText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotNecklace = m_myTransform.Find(m_widgetToFullName["NecklaceText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotRingLeft = m_myTransform.Find(m_widgetToFullName["RingLeftText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotRingRight = m_myTransform.Find(m_widgetToFullName["RingRightText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotShoes = m_myTransform.Find(m_widgetToFullName["ShoesText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotShouders = m_myTransform.Find(m_widgetToFullName["ShoudersText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_equipSlotWeapon = m_myTransform.Find(m_widgetToFullName["WeaponText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_lblDialogTitle = m_myTransform.Find(m_widgetToFullName["DialogTitle"]).GetComponentsInChildren<UILabel>(true)[0];

        m_goPackageItemGridList = m_myTransform.Find(m_widgetToFullName["PackageItemGridList"]).gameObject;

        m_myTransform.Find(m_widgetToFullName["EquipModelImageBG"]).gameObject.AddComponent<EquipModelImge>();

        m_texShowCharacterWinBG = m_myTransform.Find(m_widgetToFullName["PlayerEquipBGTexBG"]).GetComponentsInChildren<UITexture>(true)[0];


        for (int i = 0; i < DIAMONDHOLENUM; ++i)
        {
            //m_equipDetailDiamondHoleInfo[i] = m_myTransform.FindChild(m_widgetToFullName["PackageEquipInfoDiamonHole" + i + "Text"]).GetComponentsInChildren<UILabel>(true)[0];
            //m_equipCurrentDetailDiamondHoleInfo[i] = m_myTransform.FindChild(m_widgetToFullName["PackageEquipCurrentInfoDiamonHole" + i + "Text"]).GetComponentsInChildren<UILabel>(true)[0];
        }

        //string _GridWidgetName = "Item";

        //for (int i = 0; i < VOLUME; ++i)
        //{
        //    m_listGridFG.Add(m_myTransform.FindChild(m_widgetToFullName[_GridWidgetName + i + "FG"]).GetComponentsInChildren<UISlicedSprite>(true)[0]);
        //    m_listGridUp.Add(m_myTransform.FindChild(m_widgetToFullName[_GridWidgetName + i + "Up"]).GetComponentsInChildren<UISlicedSprite>(true)[0]);
        //    m_listGridNum.Add(m_myTransform.FindChild(m_widgetToFullName[_GridWidgetName + i + "Num"]).GetComponentsInChildren<UILabel>(true)[0]);
        //}

        for (int i = 0; i < 4; ++i)
        {
            //m_arrNewDiamondHoleIcon[i] = m_myTransform.FindChild(m_widgetToFullName["PackageEquipInfoDiamonHole" + (i + 9) + "FG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].gameObject;
            //m_arrCurrentDiamondHoleIcon[i] = m_myTransform.FindChild(m_widgetToFullName["PackageEquipCurrentInfoDiamonHole" + (i + 9) + "FG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].gameObject;
        }

        basicAttr = m_myTransform.Find(m_widgetToFullName["PlayerDetailBasicAttribute"]).gameObject;
        elemAttr = m_myTransform.Find(m_widgetToFullName["PlayerDetailElementAttribute"]).gameObject;

        Camera mySourceCam = GameObject.Find("MogoMainUI").transform.Find("Camera").GetComponentsInChildren<Camera>(true)[0];

        m_myTransform.Find(m_widgetToFullName["PackageCamera"]).GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = mySourceCam;

        m_myTransform.Find(m_widgetToFullName["EquipCamera"]).GetComponentsInChildren<UIViewport>(true)[0].sourceCamera = mySourceCam;

        for (int i = 0; i < 40; ++i)
        {
            AddPacakgeItemGrid(MogoUIManager.Instance.CommonAtlas);
        }

        m_menuTabUpLabelList[(int)MenuUITab.PlayerTab] = m_myTransform.Find(m_widgetToFullName["IconPlayerTextUp"]).GetComponent<UILabel>();
        m_menuTabUpLabelList[(int)MenuUITab.PackageTab] = m_myTransform.Find(m_widgetToFullName["IconPackageTextUp"]).GetComponent<UILabel>();
        m_menuTabUpLabelList[(int)MenuUITab.SkillTab] = m_myTransform.Find(m_widgetToFullName["IconSkillTextUp"]).GetComponent<UILabel>();
        m_menuTabUpLabelList[(int)MenuUITab.TongTab] = m_myTransform.Find(m_widgetToFullName["IconTongTextUp"]).GetComponent<UILabel>();
        m_menuTabUpLabelList[(int)MenuUITab.SocialTab] = m_myTransform.Find(m_widgetToFullName["IconSocialTextUp"]).GetComponent<UILabel>();
        m_menuTabUpLabelList[(int)MenuUITab.SettingsTab] = m_myTransform.Find(m_widgetToFullName["IconSettingsTextUp"]).GetComponent<UILabel>();

        m_menuTabDownLabelList[(int)MenuUITab.PlayerTab] = m_myTransform.Find(m_widgetToFullName["IconPlayerTextDown"]).GetComponent<UILabel>();
        m_menuTabDownLabelList[(int)MenuUITab.PackageTab] = m_myTransform.Find(m_widgetToFullName["IconPackageTextDown"]).GetComponent<UILabel>();
        m_menuTabDownLabelList[(int)MenuUITab.SkillTab] = m_myTransform.Find(m_widgetToFullName["IconSkillTextDown"]).GetComponent<UILabel>();
        m_menuTabDownLabelList[(int)MenuUITab.TongTab] = m_myTransform.Find(m_widgetToFullName["IconTongTextDown"]).GetComponent<UILabel>();
        m_menuTabDownLabelList[(int)MenuUITab.SocialTab] = m_myTransform.Find(m_widgetToFullName["IconSocialTextDown"]).GetComponent<UILabel>();
        m_menuTabDownLabelList[(int)MenuUITab.SettingsTab] = m_myTransform.Find(m_widgetToFullName["IconSettingsTextDown"]).GetComponent<UILabel>();
      
        m_packageTabLabelList[(int)PackageUITab.EquipmentTab] = m_myTransform.Find(m_widgetToFullName["NormalIconText"]).GetComponent<UILabel>();
        m_packageTabLabelList[(int)PackageUITab.JewelTab] = m_myTransform.Find(m_widgetToFullName["GemIconText"]).GetComponent<UILabel>();
        m_packageTabLabelList[(int)PackageUITab.MaterialTab] = m_myTransform.Find(m_widgetToFullName["MaterialIconText"]).GetComponent<UILabel>();
        foreach (var pair in m_packageTabLabelList)
        {
            if (pair.Key == (int)PackageUITab.EquipmentTab)
                PackageTabDown(pair.Key);
            else
                PackageTabUp(pair.Key);
        }

        m_playerDetailTabLabelList[(int)PlayerDetailUITab.BasicAttributeTab] = m_myTransform.Find(m_widgetToFullName["PlayerDetailBasicAttributeButtonText"]).GetComponent<UILabel>();
        m_playerDetailTabLabelList[(int)PlayerDetailUITab.ElementAttributeTab] = m_myTransform.Find(m_widgetToFullName["PlayerDetailElementAttributeButtonText"]).GetComponent<UILabel>();
        foreach (var pair in m_playerDetailTabLabelList)
        {
            if (pair.Key == (int)PlayerDetailUITab.BasicAttributeTab)
                PlayerDetailTabDown(pair.Key);
            else
                PlayerDetailTabUp(pair.Key);
        }
    }


    void Initialize()
    {
        MenuUILogicManager.Instance.Initialize();
        m_uiLoginManager = MenuUILogicManager.Instance;

        MenuUIDict.ButtonTypeToEventUp.Add("PlayerInfoDetail", MenuUIDict.MenuUIEvent.PLAYERINFODETAILUP);
        MenuUIDict.ButtonTypeToEventUp.Add("PlayerInfoEquipFX", MenuUIDict.MenuUIEvent.PLAYERINFOEQUIPFXUP);

        MenuUIDict.ButtonTypeToEventUp.Add("PlayerDetailAttributeClose", MenuUIDict.MenuUIEvent.PLAYERDETAILATTRIBUTECLOSEUP);
        MenuUIDict.ButtonTypeToEventUp.Add("Sort", MenuUIDict.MenuUIEvent.PACKAGESORTUP);
        MenuUIDict.ButtonTypeToEventUp.Add("Resolve", MenuUIDict.MenuUIEvent.PACKAGERESOLVEUP);
        MenuUIDict.ButtonTypeToEventUp.Add("PackageEquipInfoDetailPutUp", MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILPUTUPUP);
        MenuUIDict.ButtonTypeToEventUp.Add("PackageEquipInfoDetailSail", MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSAILUP);
        MenuUIDict.ButtonTypeToEventUp.Add("PackageEquipInfoDetailShowUp", MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSHOWUPUP);
        MenuUIDict.ButtonTypeToEventUp.Add("MenuUIClose", MenuUIDict.MenuUIEvent.MENUUICLOSEUP);
        MenuUIDict.ButtonTypeToEventUp.Add("PackageEquipInfoDetailInset", MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILINSETUP);
        MenuUIDict.ButtonTypeToEventUp.Add("PackageEquipInfoDetailStrenth", MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSTRENTHUP);
        MenuUIDict.ButtonTypeToEventUp.Add("PackageEquipInfoDetailUnLoad", MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILUNLOADUP);
        MenuUIDict.ButtonTypeToEventUp.Add("GemIcon", MenuUIDict.MenuUIEvent.GEMICONUP);
        MenuUIDict.ButtonTypeToEventUp.Add("NormalIcon", MenuUIDict.MenuUIEvent.NORMALICONUP);
        MenuUIDict.ButtonTypeToEventUp.Add("MaterialIcon", MenuUIDict.MenuUIEvent.MATERIALICONUP);
        MenuUIDict.ButtonTypeToEventUp.Add("PackageIcon", MenuUIDict.MenuUIEvent.PACKAGEICONUP);
        MenuUIDict.ButtonTypeToEventUp.Add("PlayerIcon", MenuUIDict.MenuUIEvent.PLAYERICONUP);
        MenuUIDict.ButtonTypeToEventUp.Add("SettingsIcon", MenuUIDict.MenuUIEvent.SETTINGSICONUP);
        MenuUIDict.ButtonTypeToEventUp.Add("SkillIcon", MenuUIDict.MenuUIEvent.SKILLICONUP);
        MenuUIDict.ButtonTypeToEventUp.Add("SocialIcon", MenuUIDict.MenuUIEvent.SOCIALICONUP);
        MenuUIDict.ButtonTypeToEventUp.Add("TongIcon", MenuUIDict.MenuUIEvent.TONGICONUP);
        MenuUIDict.ButtonTypeToEventUp.Add("DiamondInfoDetailInset", MenuUIDict.MenuUIEvent.DIAMONDTIPINSETUP);
        MenuUIDict.ButtonTypeToEventUp.Add("DiamondInfoClose", MenuUIDict.MenuUIEvent.DIAMONDTIPCLOSEUP);
        MenuUIDict.ButtonTypeToEventUp.Add("DiamondInfoDetailCompose", MenuUIDict.MenuUIEvent.DIAMONDTIPCOMPOSEUP);
        MenuUIDict.ButtonTypeToEventUp.Add("DiamondInfoDetailHistory", MenuUIDict.MenuUIEvent.DIAMONDTIPHISTROYUP);
        MenuUIDict.ButtonTypeToEventUp.Add("DiamondInfoDetailBuy", MenuUIDict.MenuUIEvent.DIAMONDTIPBUYUP);

        //Material Tip
        MenuUIDict.ButtonTypeToEventUp.Add("MaterialInfoClose", MenuUIDict.MenuUIEvent.MATERIL_TIP_CLOSE);
        MenuUIDict.ButtonTypeToEventUp.Add("MaterialInfoDetailCompose", MenuUIDict.MenuUIEvent.MATERIL_TIP_ENHANCE);
        MenuUIDict.ButtonTypeToEventUp.Add("MaterialInfoDetailHistory", MenuUIDict.MenuUIEvent.MATERIL_TIP_SELL);

        MenuUIDict.ButtonTypeToEventUp.Add("PropInfoClose", MenuUIDict.MenuUIEvent.PROP_TIP_CLOSE);
        MenuUIDict.ButtonTypeToEventUp.Add("PropInfoDetailCompose", MenuUIDict.MenuUIEvent.PROP_TIP_USE);
        MenuUIDict.ButtonTypeToEventUp.Add("PropInfoDetailHistory", MenuUIDict.MenuUIEvent.PROP_TIP_SELL);

        MenuUIDict.ButtonTypeToEventUp.Add("PlayerDetailBasicAttributeButton", MenuUIDict.MenuUIEvent.PLAYERBASICATTRBUTE);
        MenuUIDict.ButtonTypeToEventUp.Add("PlayerDetailElementAttributeButton", MenuUIDict.MenuUIEvent.PLAYERELEMENTATTRBUTE);

        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PLAYERINFODETAILUP, OnPlayerInfoDetailUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PLAYERINFOEQUIPFXUP, OnPlayerInfoEquipFXUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PLAYERDETAILATTRIBUTECLOSEUP, OnPlayerDetailAttributeCloseUp);
        EventDispatcher.AddEventListener<int>(MenuUIDict.MenuUIEvent.PACKAGEGRIDUP, OnPackageGridUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILPUTUPUP, OnPackageEquipDetailPutUpUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSAILUP, OnPackageEquipDetailSailUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSHOWUPUP, OnPackageEquipDetailShowUpUp);

        EventDispatcher.AddEventListener<int>(MenuUIDict.MenuUIEvent.PLAYERUIEQUPMENTGRIDUP, OnPlayerEquipmentGridUp);

        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.MENUUICLOSEUP, OnMenuUICloseUp);

        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPBUYUP, OnDiamondTipBuyUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPCLOSEUP, OnDiamondTipCloseUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPCOMPOSEUP, OnDiamondTipComposeUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPHISTROYUP, OnDiamondTipHistroyUp);
        EventDispatcher.AddEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPINSETUP, OnDiamondTipInsetUp);

        EventDispatcher.AddEventListener<int>(MenuUIDict.MenuUIEvent.PACKAGEGRIDUPSIGNUP, OnPackageGridUpSignUp);

        m_myTransform.Find(m_widgetToFullName["EquipFXRefreshBtn"]).GetComponentsInChildren<MFUIButtonHandler>(true)[0].ClickHandler = OnEquipFXRefreshBtnUp;
    }

    void OnEquipFXRefreshBtnUp(int id)
    {
        Debug.LogError(id);
    }
    public void Release()
    {
        MenuUILogicManager.Instance.Release();

        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PLAYERINFODETAILUP, OnPlayerInfoDetailUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PLAYERDETAILATTRIBUTECLOSEUP, OnPlayerDetailAttributeCloseUp);
        EventDispatcher.RemoveEventListener<int>(MenuUIDict.MenuUIEvent.PACKAGEGRIDUP, OnPackageGridUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILPUTUPUP, OnPackageEquipDetailPutUpUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSAILUP, OnPackageEquipDetailSailUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PACKAGEEQUIPINFODETAILSHOWUPUP, OnPackageEquipDetailShowUpUp);

        EventDispatcher.RemoveEventListener<int>(MenuUIDict.MenuUIEvent.PLAYERUIEQUPMENTGRIDUP, OnPlayerEquipmentGridUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.PLAYERINFOEQUIPFXUP, OnPlayerInfoEquipFXUp);

        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.MENUUICLOSEUP, OnMenuUICloseUp);

        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPBUYUP, OnDiamondTipBuyUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPCLOSEUP, OnDiamondTipCloseUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPCOMPOSEUP, OnDiamondTipComposeUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPHISTROYUP, OnDiamondTipHistroyUp);
        EventDispatcher.RemoveEventListener(MenuUIDict.MenuUIEvent.DIAMONDTIPINSETUP, OnDiamondTipInsetUp);

        EventDispatcher.RemoveEventListener<int>(MenuUIDict.MenuUIEvent.PACKAGEGRIDUPSIGNUP, OnPackageGridUpSignUp);

        m_instance = null;

        MenuUIDict.ButtonTypeToEventUp.Clear();


        for (int i = 0; i < m_lisgGridGO.Count; i++)
        {
            AssetCacheMgr.ReleaseInstance(m_lisgGridGO[i]);
        }
        m_lisgGridGO.Clear();

        //for (int i = 0; i < m_listGridUp.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listGridUp[i].gameObject);
        //    m_listGridUp[i] = null;
        //}
        m_listGridUp.Clear();

        //for (int i = 0; i < m_listGridUpBl.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listGridUpBl[i].gameObject);
        //    m_listGridUpBl[i] = null;
        //}
        m_listGridUpBl.Clear();

        //for (int i = 0; i < m_listGridNum.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listGridNum[i].gameObject);
        //    m_listGridNum[i] = null;
        //}
        m_listGridNum.Clear();

        //for (int i = 0; i < m_listGridFG.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listGridFG[i].gameObject);
        //    m_listGridFG[i] = null;
        //}
        m_listGridFG.Clear();
        //for (int i = 0; i < m_listGridBG.Count; ++i)
        //{
        //    AssetCacheMgr.ReleaseInstance(m_listGridBG[i].gameObject);
        //    m_listGridBG[i] = null;
        //}
        m_listGridBG.Clear();

        for (int i = 0; i < gos.Count; ++i)
        {
            AssetCacheMgr.ReleaseInstance(gos[i]);
            gos[i] = null;
        }
        gos.Clear();
    }

    #region Tab Change
  
    /// <summary>
    /// 设置Tab文字颜色
    /// </summary>
    private void SettingMenuTabLabel()
    {
        foreach (UILabel label in m_menuTabUpLabelList.Values)
        {
            label.color = new Color32(37, 29, 6, 255);
            label.effectStyle = UILabel.Effect.None;
        }

        foreach (UILabel label in m_menuTabDownLabelList.Values)
        {
            label.color = new Color32(255, 255, 255, 255);
            label.effectStyle = UILabel.Effect.Outline;
            label.effectColor = new Color32(53, 22, 2, 255);
        }
    }
   
    public void HandlePackageTabChange(int fromTab, int toTab)
    {
        PackageTabUp(fromTab);
        PackageTabDown(toTab);
    }

    void PackageTabUp(int tab)
    {
        if (m_packageTabLabelList.ContainsKey(tab))
        {
            UILabel fromTabLabel = m_packageTabLabelList[tab];
            if (fromTabLabel != null)
            {
                fromTabLabel.color = new Color32(37, 29, 6, 255);
                fromTabLabel.effectStyle = UILabel.Effect.None;
            }
        }
    }

    void PackageTabDown(int tab)
    {
        if (m_packageTabLabelList.ContainsKey(tab))
        {
            UILabel toTabLabel = m_packageTabLabelList[tab];
            if (toTabLabel != null)
            {
                toTabLabel.color = new Color32(255, 255, 255, 255);
                toTabLabel.effectStyle = UILabel.Effect.Outline;
                toTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }
    }

    public void HandlePlayerDetailTabChange(int fromTab, int toTab)
    {
        PlayerDetailTabUp(fromTab);
        PlayerDetailTabDown(toTab);
    }

    void PlayerDetailTabUp(int tab)
    {
        if (m_playerDetailTabLabelList.ContainsKey(tab))
        {
            UILabel fromTabLabel = m_playerDetailTabLabelList[tab];
            if (fromTabLabel != null)
            {
                //fromTabLabel.color = new Color32(37, 29, 6, 255);
                //fromTabLabel.effectStyle = UILabel.Effect.None;

                fromTabLabel.color = new Color32(255, 255, 255, 255);
                fromTabLabel.effectStyle = UILabel.Effect.Outline;
                fromTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }
    }

    void PlayerDetailTabDown(int tab)
    {
        if (m_playerDetailTabLabelList.ContainsKey(tab))
        {
            UILabel toTabLabel = m_playerDetailTabLabelList[tab];
            if (toTabLabel != null)
            {
                toTabLabel.color = new Color32(255, 255, 255, 255);
                toTabLabel.effectStyle = UILabel.Effect.Outline;
                toTabLabel.effectColor = new Color32(53, 22, 2, 255);
            }
        }
    }
    #endregion

    private void OnPlayerInfoDetailUp()
    {
        m_myTransform.Find(m_widgetToFullName["PlayerDetailAttribute"]).gameObject.SetActive(true);
        ShowBasicAttr();
        m_myTransform.Find(m_widgetToFullName["EquipModelImage"]).gameObject.SetActive(false);
    }

    private void OnPlayerInfoEquipFXUp()
    {
        MogoWorld.thePlayer.RpcCall("SyncSepcialEffectsReq");
        //MFUIManager.GetSingleton().SwitchUIWithLoad(MFUIManager.MFUIID.EquipFXUI, MFUIManager.MFUIID.None, 0, true);
        //EquipFXUILogicManager.Instance.FillJewelFXGridList();
        //EquipFXUILogicManager.Instance.SetUIDirty();
        //List<EquipFXUIGridData> listinfo= new List<EquipFXUIGridData>();
        //EquipFXUIGridData data = new EquipFXUIGridData();
        //data.gridProgressSize = 0.2f;
        //data.gridProgressText = "20/100";
        //data.gridText = "fuck";
        //listinfo.Add(data);
        //listinfo.Add(data);
        //listinfo.Add(data);
        //listinfo.Add(data);
        //listinfo.Add(data);
        //listinfo.Add(data);
        //listinfo.Add(data);
        //listinfo.Add(data);
        //listinfo.Add(data);
        //listinfo.Add(data);
        //EquipFXUILogicManager.Instance.RefreshFXGrid(listinfo);
        //Debug.LogError(EquipFXUILogicManager.Instance.CalculateStrenthMarks(1)+" Marks!!!!!!!!!!!!!!!");
        //EquipFXUILogicManager.Instance.SetUIDirty();
    }

    private void OnPlayerDetailAttributeCloseUp()
    {
        m_myTransform.Find(m_widgetToFullName["PlayerDetailAttribute"]).gameObject.SetActive(false);
        m_myTransform.Find(m_widgetToFullName["EquipModelImage"]).gameObject.SetActive(true);
    }

    private void OnPackageGridUp(int id)
    {
        //m_packageInfoDetailPackage.SetActive(true);
        //m_packageInfoDetailPlayer.SetActive(false);
    }

    private void OnPackageGridUpSignUp(int id)
    {
        Mogo.Util.LoggerHelper.Debug("Up " + id);
        InventoryManager.Instance.OnEquipUpgradeIconUp(id);
    }

    private void OnPackageEquipDetailPutUpUp()
    {
        LoggerHelper.Debug("PackageEquipDetailPutUpUp");
    }

    private void OnPackageEquipDetailSailUp()
    {
        LoggerHelper.Debug("PackageEquipDetailSailUp");
        EventDispatcher.TriggerEvent(InventoryManager.ON_DECOMPOSE);
    }

    private void OnPackageEquipDetailShowUpUp()
    {
        LoggerHelper.Debug("PackageEquipDetailShowUpUp");
    }

    private void OnPlayerEquipmentGridUp(int id)
    {
        //m_packageInfoDetailPackage.SetActive(false);
        //m_packageInfoDetailPlayer.SetActive(true);
    }

    private void OnMenuUICloseUp()
    {
        if (IsNormalMainUI)
        {
            MogoUIManager.Instance.ShowMogoNormalMainUI();
        }
        else
        {
            MogoUIManager.Instance.ShowMogoBattleMainUI();
        }

        EventDispatcher.TriggerEvent(SettingEvent.SaveVolume);
    }

    private void OnDiamondTipBuyUp()
    {
    }

    private void OnDiamondTipCloseUp()
    {
        ShowDiamondInfoTip(false);
    }

    private void OnDiamondTipComposeUp()
    {
    }

    private void OnDiamondTipHistroyUp()
    {
    }

    void OnDiamondTipInsetUp()
    {
    }

    public void LoadPlayerModel(float fOffect = 0, float xOffset = 0, float yOffset = 0, float zOffset = 0, float fLookatOffsetY = 0f,
        float fLookatOffsetX = 0f)
    {
        //GameObject obj = (GameObject)Instantiate();
        //obj.transform.parent = m_myTransform.FindChild(m_widgetToFullName["EquipModelImage"]);

        //AssetCacheMgr.GetUIInstance(name, (prefab, guid, go) =>
        //{
        //    ((GameObject)go).transform.parent = m_myTransform.FindChild(m_widgetToFullName["EquipModelImage"]);
        //    //AssetCacheMgr.ReleaseInstance(prefab, guid);
        //});
        GameObject obj = MogoWorld.thePlayer.GameObject;

        //m_goEquipmentModel = obj;

        //m_goEquipmentModel.transform.parent = m_myTransform.FindChild(m_widgetToFullName["EquipModelPos"]);
        //m_goEquipmentModel.transform.localPosition = new Vector3(0, 0, 0);
        //m_goEquipmentModel.transform.localScale = new Vector3(1, 1, 1);

        Transform cam = m_myTransform.Find(m_widgetToFullName["EquipCamera"]);

        Vector3 pos = obj.transform.Find("slot_camera").position;
        cam.position = pos + obj.transform.forward * fOffect;
        cam.position += obj.transform.right * xOffset;
        cam.position += obj.transform.up * yOffset;
        //cam.forward = -obj.transform.forward;
        cam.LookAt(pos + new Vector3(fLookatOffsetX, fLookatOffsetY, 0));

        //Debug.Log(fOffect + " " + xOffset + " " + yOffset + " " + zOffset);
        SetObjectLayer(10, obj);
    }

    //public float forward = 2.6f;
    //public float xOffset = 0f;
    //public float yOffset = 0f;
    //public float zOffset = 0f;
    //public bool Runeing = false;
    //public float LookatOffsetY = 0f;
    //public float LookatOffsetX = 0f;

    //void Update()
    //{
    //    if (!Runeing)
    //        return;
    //    GameObject obj = MogoWorld.thePlayer.GameObject;
    //    Transform cam = m_myTransform.FindChild(m_widgetToFullName["EquipCamera"]);

    //    cam.position = obj.transform.FindChild("slot_camera").position + obj.transform.forward * forward;
    //    cam.position += obj.transform.right * xOffset;
    //    cam.position += obj.transform.up * yOffset;
    //    //cam.forward = -obj.transform.forward + new Vector3(0,fLookatOffset,0);
    //    cam.LookAt(obj.transform.FindChild("slot_camera").position + new Vector3(LookatOffsetX, LookatOffsetY, 0));
    //    //cam.LookAt(obj.transform.position);

    //}

    public void DisablePlayerModel()
    {
        SetObjectLayer(8, MogoWorld.thePlayer.GameObject);
    }

    public void SetObjectLayer(int layer, GameObject obj)
    {
        if (!obj)
            return;

        obj.layer = layer;

        foreach (Transform item in obj.transform)
        {
            SetObjectLayer(layer, item.gameObject);
        }
    }

    #region 背包

    /// <summary>
    /// 获取背包当前页
    /// </summary>
    /// <returns></returns>
    public int GetCurrentPage()
    {
        if (m_packageDragableCamera == null)
            m_packageDragableCamera = m_myTransform.Find(m_widgetToFullName["PackageCamera"]).GetComponentsInChildren<MyDragableCamera>(true)[0];

        return m_packageDragableCamera.GetCurrentPage();
    }

    /// <summary>
    /// 设置背包当前页
    /// </summary>
    /// <param name="page"></param>
    public void SetCurrentPage(int page, bool bSetCurrentPage = false)
    {
        if (m_packageDragableCamera == null)
            m_packageDragableCamera = m_myTransform.Find(m_widgetToFullName["PackageCamera"]).GetComponentsInChildren<MyDragableCamera>(true)[0];

        m_packageDragableCamera.SetCurrentPage(page);
        if (bSetCurrentPage)
            m_packageDragableCamera.IsSetCurrentPageByChangeTab = true;
    }

    /// <summary>
    /// 把背包滑动到目标页
    /// </summary>
    public void MoveToPackagePage(int page = 0, bool bSetCurrentPage = false)
    {
        if (m_packageDragableCamera == null)
            m_packageDragableCamera = m_myTransform.Find(m_widgetToFullName["PackageCamera"]).GetComponentsInChildren<MyDragableCamera>(true)[0];

        m_packageDragableCamera.MoveTo(page);
          if (bSetCurrentPage)
            m_packageDragableCamera.IsSetCurrentPageByChangeTab = true;
    }

    /// <summary>
    /// 背包滑过最后一页
    /// </summary>
    private void OnOutOfBoundsMaxPage()
    {
        EventDispatcher.TriggerEvent(MenuUIDict.MenuUIEvent.PACKAGEOUTOFBOUNDSMAXUP);
    }

    /// <summary>
    /// 背包滑过第一页
    /// </summary>
    private void OnOutOfBoundsMinPage()
    {
        EventDispatcher.TriggerEvent(MenuUIDict.MenuUIEvent.PACKAGEOUTOFBOUNDSMINUP);
    }

    /// <summary>
    /// 点击Normal tab,并设置当前页
    /// </summary>
    public void FakeNormalPackage(int page = 0)
    {
        if (m_packageIcoSingleButtonList != null)
            m_packageIcoSingleButtonList.SetCurrentDownButton(0);

        m_myTransform.Find(m_widgetToFullName["NormalIcon"]).GetComponentsInChildren<MogoFakeClick>(true)[0].FakeIt();
        MoveToPackagePage(page, true);
        //SetCurrentPage(page, true);
    }

    /// <summary>
    /// 点击Gem tab,并设置当前页
    /// </summary>
    public void FakeGemPackage(int page = 0)
    {
        if (m_packageIcoSingleButtonList != null)
            m_packageIcoSingleButtonList.SetCurrentDownButton(1);

        m_myTransform.Find(m_widgetToFullName["GemIcon"]).GetComponentsInChildren<MogoFakeClick>(true)[0].FakeIt();
        MoveToPackagePage(page, true);
        //SetCurrentPage(page, true);
    }

    /// <summary>
    /// 点击Material tab,并设置当前页
    /// </summary>
    public void FakeMaterialPackage(int page = 0)
    {
        if (m_packageIcoSingleButtonList != null)
            m_packageIcoSingleButtonList.SetCurrentDownButton(2);

        m_myTransform.Find(m_widgetToFullName["MaterialIcon"]).GetComponentsInChildren<MogoFakeClick>(true)[0].FakeIt();
        MoveToPackagePage(page, true);
        //SetCurrentPage(page, true);
    }

    /// <summary>
    /// 重置为默认tab，默认page
    /// </summary>
    public void ResetDefaultPackagePage()
    {
        if (m_packageIcoSingleButtonList != null)
            m_packageIcoSingleButtonList.SetCurrentDownButton(0);

        SetCurrentPage(0);
        m_myTransform.Find(m_widgetToFullName["NormalIcon"]).GetComponentsInChildren<MogoFakeClick>(true)[0].FakeIt();        
    }

    public void RefreshPackageList()
    {
        m_listGridFG[0].RefreshRelatedPanel(m_listGridFG[0].gameObject);
    }

    public void ShowPackageGridUpSignBL(int id, bool isShow)
    {
        m_listGridUpBl[id].gameObject.SetActive(isShow);
    }

    #endregion

    public void ShowDiamondInfoTip(bool isShow)
    {
        m_diamondInfoTip.SetActive(isShow);
    }

    public void ShowSocialTip(bool isShow)
    {
        m_socialTip.SetActive(isShow);
    }

    public void SetDiamondInfoTipName(string text)
    {
        m_diamondTipName.text = text;
    }
    public void SetDiamondTipMaxStack(string num)
    {
        m_diamondMaxStack.text = num;
    }

    public void SetDiamondInfoTipLevel(string level)
    {
        m_diamondTipLevel.text = level;
    }

    internal void SetDiamondInfoTipIconBg(string p)
    {
        m_diamondTipIconBg.spriteName = p;
    }

    public void SetDiamondInfoTipType(string type)
    {
        m_diamondTipType.text = type;
    }

    public void SetDiamondInfoTipDesc(string desc)
    {
        m_diamondTipDesc.text = desc;
    }

    public void SetDiamondInfoTipIcon(string iconName, int color = 0)
    {
        m_ssDiamondTipIcon.spriteName = iconName;
        SetImageColor(m_ssDiamondTipIcon, color);
    }

    /// <summary>
    /// 创建背包Grid
    /// </summary>
    /// <param name="atlas"></param>
    readonly static private float PACKAGEPAGEWIDTH = 887.5f;
    public void AddPacakgeItemGrid(UIAtlas atlas)
    {
        AssetCacheMgr.GetUIInstance("PackageItemGrid.prefab", (prefab, guid, go) =>
        {
            int page = m_listGridUp.Count / 10;
            int row = (m_listGridUp.Count % 10) / 5;
            int col = (m_listGridUp.Count % 10) % 5;

            GameObject obj = (GameObject)go;
            obj.name = "PackageItemGrid" + m_listGridUp.Count;
            obj.transform.parent = m_goPackageItemGridList.transform;
            obj.transform.localPosition = new Vector3(page * PACKAGEPAGEWIDTH + col * 177.5f, row * -177.5f, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.GetComponentsInChildren<PackageItemBox>(true)[0].id = m_listGridUp.Count;
            obj.transform.Find("PackageItemGridUp").GetComponentsInChildren<PackageItemBoxUpSign>(true)[0].id = m_listGridUp.Count;
            obj.GetComponentsInChildren<MyDragCamera>(true)[0].RelatedCamera = m_myTransform.Find(m_widgetToFullName["PackageCamera"]).GetComponentsInChildren<Camera>(true)[0];

            obj.transform.Find("PackageItemGridFG").GetComponentsInChildren<UISlicedSprite>(true)[0].atlas = atlas;
            obj.transform.Find("PackageItemGridFGFG").GetComponentsInChildren<UISlicedSprite>(true)[0].atlas = atlas;

            m_lisgGridGO.Add(obj);
            m_listGridUp.Add(obj.transform.Find("PackageItemGridUp").GetComponentsInChildren<UISlicedSprite>(true)[0]);
            UISprite tempGridUpBl = obj.transform.Find("PackageItemGridUp2").GetComponentsInChildren<UISprite>(true)[0];
            tempGridUpBl.gameObject.SetActive(false);
            m_listGridUpBl.Add(tempGridUpBl);
            m_listGridNum.Add(obj.transform.Find("PackageItemGridNum").GetComponentsInChildren<UILabel>(true)[0]);
            m_listGridFG.Add(obj.transform.Find("PackageItemGridFG").GetComponentsInChildren<UISlicedSprite>(true)[0]);
            obj.transform.Find("PackageItemGridFG").GetComponentsInChildren<UISlicedSprite>(true)[0].selfRefresh = false;
            m_listGridBG.Add(obj.transform.Find("PackageItemGridBG").GetComponentsInChildren<UISlicedSprite>(true)[0]);

            if (m_packageDragableCamera.transformList.Count > 0)
                m_packageDragableCamera.MAXX = m_packageDragableCamera.transformList[m_packageDragableCamera.transformList.Count - 1].localPosition.x;

            if (m_listGridBG.Count == 40)
            {
                EventDispatcher.TriggerEvent(InventoryManager.ON_INVENTORY_SHOW);
                MogoGlobleUIManager.Instance.ShowWaitingTip(false);
            }

        });
    }

    #endregion

    #region MaterialTip

    public void SetMaterialTipName(string name)
    {
        m_materialTipName.text = name;
    }

    public void SetMaterialTipLevel(string p)
    {
        m_materialTipLevel.text = p;
    }

    public void SetMaterialTipDesc(string p)
    {
        m_materialTipDecs.text = LanguageData.GetContent(47283) + p; // 描述：
    }

    public void SetMaterialTipStack(string p)
    {
        m_materialTipStack.text = p;
    }

    public void SetMaterialTipPrice(string p)
    {
        m_materialTipPrice.text = p;
        float dis = m_materialTipPrice.font.CalculatePrintedSize(m_materialTipPrice.text, false, UIFont.SymbolStyle.None).x * 22;
        Vector3 position = m_materialTipPrice.transform.localPosition;
        m_materialTipPriceIcon.transform.localPosition = new Vector3(position.x + dis + 2, m_materialTipPriceIcon.transform.localPosition.y, position.z);
        //dis += 5 + ;
    }

    public void ShowMaterialTip(bool p)
    {
        m_materialTip.SetActive(p);
    }
    public void SetMaterialTipIcon(string p, int color = 0)
    {
        m_materialTipIcon.spriteName = p;
        SetImageColor(m_materialTipIcon, color);
    }
    public void SetMaterialTipIconBg(string p)
    {
        m_materialTipIconBg.spriteName = p;
    }

    #endregion MaterialTip

    public void ShowBasicAttr()
    {
        basicAttr.SetActive(true);
        elemAttr.SetActive(false);
        HandlePlayerDetailTabChange((int)PlayerDetailUITab.ElementAttributeTab, (int)PlayerDetailUITab.BasicAttributeTab);
    }

    public void ShowElementAttr()
    {
        basicAttr.SetActive(false);
        elemAttr.SetActive(true);
        HandlePlayerDetailTabChange((int)PlayerDetailUITab.BasicAttributeTab, (int)PlayerDetailUITab.ElementAttributeTab);
    }

    public void SetPlayerBeltName(string name)
    {
        m_equipSlotBelt.text = name;
    }

    public void SetPlayerBreastPlateName(string name)
    {
        m_equipSlotBreast.text = name;
    }

    public void SetPlayerHandGuardName(string name)
    {
        m_equipSlotHand.text = name;
    }

    public void SetPlayerHeadEquipName(string name)
    {
        m_equipSlotHead.text = name;
    }

    public void SetPlayerCuishName(string name)
    {
        m_equipSlotCuish.text = name;
    }

    public void SetPlayerNecklaceName(string name)
    {
        m_equipSlotNecklace.text = name;
    }

    public void SetPlayerRingLeftName(string name)
    {
        m_equipSlotRingLeft.text = name;
    }

    public void SetPlayerRingRightName(string name)
    {
        m_equipSlotRingRight.text = name;
    }

    public void SetPlayerShoesName(string name)
    {
        m_equipSlotShoes.text = name;
    }

    public void SetPlayerShoudersName(string name)
    {
        m_equipSlotShouders.text = name;
    }

    public void SetPlayerWeaponName(string name)
    {
        m_equipSlotWeapon.text = name;
    }

    public void SetPlayerInfoName(string name)
    {
        m_playerName.text = name;
    }

    public void SetPlayerInfoLevel(string levelMsg)
    {
        m_playerLevel.text = levelMsg;
    }

    public void SetPlayerInfoNameAndLevel(string name, string level)
    {
        m_playerNameAndLevel.text = name + level;
    }

    public void SetPropTipName(string p)
    {
        m_propTipName.text = p;
    }

    public void SetPropTipDesc(string p)
    {
        m_propTipDecs.text = LanguageData.GetContent(47283) + p; // 描述：
    }

    public void ShowPropTipUseBtn(bool b)
    {
        m_propTipUseBtn.SetActive(b);
    }

    public void SetPropTipStack(string p)
    {
        m_propTipStack.text = p;
    }

    public void SetPropTipPrice(string p)
    {
        m_propTipPrice.text = p;
        float dis = m_propTipPrice.font.CalculatePrintedSize(m_propTipPrice.text, false, UIFont.SymbolStyle.None).x * 22;
        Vector3 position = m_propTipPrice.transform.localPosition;
        m_propTipPriceIcon.transform.localPosition = new Vector3(12/*position.x + dis + 2*/, m_propTipPriceIcon.transform.localPosition.y, position.z);
    }

    public void SetPropTipIcon(string p1, int p2)
    {
        m_propTipIcon.spriteName = p1;
        SetImageColor(m_propTipIcon, p2);
    }

    public void ShowPropTip(bool p)
    {
        m_propTip.SetActive(p);
    }

    public void SetPropTipIconBg(string p)
    {
        m_propTipIconBg.spriteName = p;
    }

    public void SetDialogTitleText(string text)
    {
        m_lblDialogTitle.text = text;
    }

    #region 界面打开和关闭

    protected override void OnEnable()
    {
        base.OnEnable();        
        FakeShowPlayerUI();

        //if (!SystemSwitch.DestroyResource)
        //{
        //    return;
        //}

        //if (m_texShowCharacterWinBG.mainTexture == null)
        //{

        //    AssetCacheMgr.GetUIResource("js_bj.png", (obj) =>
        //    {

        //        m_texShowCharacterWinBG.mainTexture = (Texture)obj;
        //    });
        //}
    }

    void DestroyPreLoadResources()
    {
        AssetCacheMgr.ReleaseResourceImmediate("PackageItemGrid.prefab");
    }

    public void DestroyUIAndResources()
    {
        if (SystemSwitch.DestroyAllUI)
        {
            DestroyPreLoadResources();
            //InventoryManager.Instance.CurrentView = InventoryManager.View.PlayerEquipment;
            InventoryManager.Instance.m_currentPackageView = InventoryManager.View.PlayerEquipment;
            MogoUIManager.Instance.DestroyMenuUI();

        }

        //if (!SystemSwitch.DestroyResource)
        //    return;

        //m_texShowCharacterWinBG.mainTexture = null;
        //AssetCacheMgr.ReleaseResourceImmediate("js_bj.png");
    }

    void OnDisable()
    {
        DestroyUIAndResources();
    }

    #endregion

    #region 模拟切换MenuTab界面

    public void FakeShowPlayerUI()
    {
        FindTransform("IconList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(0);
        FindTransform("PlayerIcon").GetComponentsInChildren<MogoFakeClick>(true)[0].FakeIt();
    }

    public void FakeShowPackageUI()
    {
        FindTransform("IconList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(1);
        FindTransform("PackageIcon").GetComponentsInChildren<MogoFakeClick>(true)[0].FakeIt();
    }

    public void FakeShowSkillUI()
    {
        FindTransform("IconList").GetComponentsInChildren<MogoSingleButtonList>(true)[0].SetCurrentDownButton(2);
        FindTransform("SkillIcon").GetComponentsInChildren<MogoFakeClick>(true)[0].FakeIt();
    }

    #endregion

    public void ShowAllEquipItem(bool isShow)
    {
        m_myTransform.Find(m_widgetToFullName["GOPlayerEquipL"]).gameObject.SetActive(isShow);
        m_myTransform.Find(m_widgetToFullName["GOPlayerEquipM"]).gameObject.SetActive(isShow);
        m_myTransform.Find(m_widgetToFullName["GOPlayerEquipR"]).gameObject.SetActive(isShow);
    }

    public void ShowRefreshEquipFXBtn(bool isShow)
    {
        m_myTransform.Find(m_widgetToFullName["EquipFXRefreshBtn"]).gameObject.SetActive(isShow);
    }

    public void ShowIconListMask(bool isShow)
    {
        m_myTransform.Find(m_widgetToFullName["IconListBGMask"]).gameObject.SetActive(isShow);
    }
}