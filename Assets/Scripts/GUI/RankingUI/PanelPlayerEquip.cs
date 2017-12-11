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
using System.Collections.Generic;
using Mogo.Util;
using System;
using Mogo.Game;
using Mogo.GameData;

public class PanelPlayerEquip : MogoUIBehaviour 
{
    /// <summary>
    /// 装备
    /// </summary>
    private GameObject m_goBelt;
    private GameObject m_goBreast;
    private GameObject m_goHand;
    private GameObject m_goHead;
    private GameObject m_goCuish;
    private GameObject m_goNecklace;
    private GameObject m_goRingLeft;
    private GameObject m_goRingRight;
    private GameObject m_goShoes;
    private GameObject m_goShouders;
    private GameObject m_goWeapon;

    /// <summary>
    /// 装备名
    /// </summary>
    private UILabel m_lblEquipSlotBelt;
    private UILabel m_lblEquipSlotBreast;
    private UILabel m_lblEquipSlotHand;
    private UILabel m_lblEquipSlotHead;
    private UILabel m_lblEquipSlotCuish;
    private UILabel m_lblEquipSlotNecklace;
    private UILabel m_lblEquipSlotRingLeft;
    private UILabel m_lblEquipSlotRingRight;
    private UILabel m_lblEquipSlotShoes;
    private UILabel m_lblEquipSlotShouders;
    private UILabel m_lblEquipSlotWeapon;

    private UILabel m_lblPlayerNameAndLevelText;

    private bool m_bLoadResourceInsteadOfAwake = false;
    public void LoadResourceInsteadOfAwake()
    {
        if (m_bLoadResourceInsteadOfAwake)
            return;

        m_bLoadResourceInsteadOfAwake = true;

        m_myTransform = transform;
        FillFullNameData(m_myTransform);

        m_goBelt = m_myTransform.Find(m_widgetToFullName["Belt"]).gameObject;
        m_goBelt.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.Belt;        
        m_goBreast = m_myTransform.Find(m_widgetToFullName["BreastPlate"]).gameObject;
        m_goBreast.AddComponent<PanelPlayerEquipUIButton>().EquipSolt =(int)EquipSlot.Cuirass;
        m_goHand = m_myTransform.Find(m_widgetToFullName["HandGuard"]).gameObject;
        m_goHand.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.Glove;
        m_goHead = m_myTransform.Find(m_widgetToFullName["HeadEquip"]).gameObject;
        m_goHead.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.Head;
        m_goCuish = m_myTransform.Find(m_widgetToFullName["Cuish"]).gameObject;
        m_goCuish.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.Cuish;
        m_goNecklace = m_myTransform.Find(m_widgetToFullName["Necklace"]).gameObject;
        m_goNecklace.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.Neck;
        m_goRingLeft = m_myTransform.Find(m_widgetToFullName["RingLeft"]).gameObject;
        m_goRingLeft.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.LeftRing;
        m_goRingRight = m_myTransform.Find(m_widgetToFullName["RingRight"]).gameObject;
        m_goRingRight.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.RightRing;
        m_goShoes = m_myTransform.Find(m_widgetToFullName["Shoes"]).gameObject;
        m_goShoes.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.Shoes;
        m_goShouders = m_myTransform.Find(m_widgetToFullName["Shouders"]).gameObject;
        m_goShouders.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.Shoulder;
        m_goWeapon = m_myTransform.Find(m_widgetToFullName["Weapon"]).gameObject;
        m_goWeapon.AddComponent<PanelPlayerEquipUIButton>().EquipSolt = (int)EquipSlot.Weapon;

        m_lblEquipSlotBelt = m_myTransform.Find(m_widgetToFullName["BeltText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotBreast = m_myTransform.Find(m_widgetToFullName["BreastPlateText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotCuish = m_myTransform.Find(m_widgetToFullName["CuishText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotHand = m_myTransform.Find(m_widgetToFullName["HandGuardText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotHead = m_myTransform.Find(m_widgetToFullName["HeadEquipText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotNecklace = m_myTransform.Find(m_widgetToFullName["NecklaceText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotRingLeft = m_myTransform.Find(m_widgetToFullName["RingLeftText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotRingRight = m_myTransform.Find(m_widgetToFullName["RingRightText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotShoes = m_myTransform.Find(m_widgetToFullName["ShoesText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotShouders = m_myTransform.Find(m_widgetToFullName["ShoudersText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblEquipSlotWeapon = m_myTransform.Find(m_widgetToFullName["WeaponText"]).GetComponentsInChildren<UILabel>(true)[0];

        m_lblPlayerNameAndLevelText = m_myTransform.Find(m_widgetToFullName["PlayerNameAndLevelText"]).GetComponentsInChildren<UILabel>(true)[0];

        Initialize();
    }

    #region 事件

    public Action<int> PANELPLAYEREQUIPUP;

    void Initialize()
    {
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("Belt", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("BreastPlate", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("HandGuard", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("HeadEquip", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("Cuish", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("Necklace", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("RingLeft", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("RingRight", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("Shoes", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("Shouders", OnEquipUp);
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Add("Weapon", OnEquipUp);
    }

    void Start()
    {
       
    }

    public void Release()
    {
        PanelPlayerEquipUIDict.ButtonTypeToEventUp.Clear();
    }

    //void OnEquipUp(int equipSlot)
    //{
    //    if (m_itemsOnEquip.ContainsKey(equipSlot))
    //    {
    //        if (PANELPLAYEREQUIPUP != null)
    //            PANELPLAYEREQUIPUP(m_itemsOnEquip[equipSlot].id);

    //        ItemEquipment item = m_itemsOnEquip[equipSlot];
    //        InventoryManager.Instance.ShowEquipTip(item, null, MogoWorld.thePlayer.level);
    //    }       
    //}

    void OnEquipUp(int equipSlot)
    {
        RankEquipData rankEquipData = GetRankEquipDataBySlot(equipSlot);
        if (rankEquipData != null)
        {
            if (PANELPLAYEREQUIPUP != null)
                PANELPLAYEREQUIPUP(rankEquipData.equipID);

            InventoryManager.Instance.ShowEquipTip(rankEquipData.equipID, null, rankEquipData.jewelSlots, Level);
        }       
    }

    #endregion

    #region 玩家装备

    public readonly static int SLOT_NUM = 11;

    //private Dictionary<int, ItemEquipment> m_itemsOnEquip = new Dictionary<int, ItemEquipment>();//装备中的物件(key为部位id)
    //public void RefreshPlayerEquipmentInfoUI(Dictionary<int, ItemEquipment> itemsOnEquip)
    //{
    //    m_itemsOnEquip = itemsOnEquip;
    //    for (int i = 1; i <= SLOT_NUM; i++)
    //    {
    //        if (m_itemsOnEquip.ContainsKey(i))
    //        {
    //            SetPlayerEquipmentInfo(i, m_itemsOnEquip[i].icon, 0, m_itemsOnEquip[i].quality);
    //        }          
    //    }
    //}

    List<RankEquipData> m_RankEquipDataList = new List<RankEquipData>();
    public void RefreshPlayerEquipmentInfoUI(List<RankEquipData> equipList)
    {
        m_RankEquipDataList = equipList;

        for (int equipSlot = 1; equipSlot <= SLOT_NUM; equipSlot++)
        {
            RankEquipData rankEquipData = GetRankEquipDataBySlot(equipSlot);
            if (rankEquipData != null)
            {
                ItemEquipmentData data = ItemEquipmentData.GetItemEquipmentData(rankEquipData.equipID);
                if(data != null)
                    SetPlayerEquipmentInfo(equipSlot, data.Icon, data.color, data.quality);
            }
            else
            {
                SetPlayerEquipmentInfo(equipSlot, EquipSlotIcon.icons[equipSlot], 10);
            }
        }
    }
    
    private RankEquipData GetRankEquipDataBySlot(int equipSlot)
    {
        for (int i = 0; i < m_RankEquipDataList.Count; i++)
        {
            if (m_RankEquipDataList[i].equipSlot == equipSlot)
                return m_RankEquipDataList[i];
        }

        return null;
    }

    private void SetPlayerEquipmentInfo(int slot, string iconName, int color = 0, int quality = 1)
    {
        EquipSlot type = (EquipSlot)slot;
        if (slot > 9)
            slot -= 1;

        string qulityIcon = IconData.GetIconByQuality(quality);
        switch (type)
        {
            case EquipSlot.Belt:
                SetPlayerBeltFG(iconName, color);
                SetPlayerBeltBG(qulityIcon);
                break;
            case EquipSlot.Cuirass:
                SetPlayerBreastPlateFG(iconName, color);
                SetPlayerBreastPlateBG(qulityIcon);
                break;
            case EquipSlot.Glove:
                SetPlayerHandGuardFG(iconName, color);
                SetPlayerHandGuardBG(qulityIcon);
                break;
            case EquipSlot.Head:
                SetPlayerHeadEquipFG(iconName, color);
                SetPlayerHeadEquipBG(qulityIcon);
                break;
            case EquipSlot.Cuish:
                SetPlayerCuishFG(iconName, color);
                SetPlayerCuishBG(qulityIcon);
                break;
            case EquipSlot.Neck:
                SetPlayerNecklaceFG(iconName, color);
                SetPlayerNecklaceBG(qulityIcon);
                break;
            case EquipSlot.LeftRing:
                SetPlayerRingLeftFG(iconName, color);
                SetPlayerRingLeftBG(qulityIcon);
                break;
            case EquipSlot.RightRing:
                SetPlayerRingRightFG(iconName, color);
                SetPlayerRingRightBG(qulityIcon);
                break;
            case EquipSlot.Shoes:
                SetPlayerShoesFG(iconName, color);
                SetPlayerShoesBG(qulityIcon);
                break;
            case EquipSlot.Shoulder:
                SetPlayerShoudersFG(iconName, color);
                SetPlayerShoudersBG(qulityIcon);
                break;
            case EquipSlot.Weapon:
                SetPlayerWeaponFG(iconName, color);
                SetPlayerWeaponBG(qulityIcon);
                break;
        }
    }

    // true: GetAtlasByIconName; false: TryingSetSpriteName
    readonly static private bool SETFGBYGetAtlasByIconName = false;

    private void SetPlayerBeltFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["BeltFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }

        MogoUtils.SetImageColor(temp, color);
    }

    private void SetPlayerBreastPlateFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["BreastPlateFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }
        
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["BreastPlateFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerCuishFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["CuishFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }
        
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["CuishFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerHandGuardFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["HandGuardFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }
        
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["HandGuardFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerHeadEquipFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["HeadEquipFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }
        
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["HeadEquipFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerNecklaceFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["NecklaceFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }
        
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["NecklaceFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerRingLeftFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["RingLeftFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }
        
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["RingLeftFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerRingRightFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["RingRightFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }
        
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["RingRightFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerShoesFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["ShoesFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }

        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["ShoesFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerShoudersFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["ShoudersFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }

        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["ShoudersFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerWeaponFG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["WeaponFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        if (SETFGBYGetAtlasByIconName)
        {
            temp.atlas = MogoUIManager.Instance.GetAtlasByIconName(imgName);
            temp.spriteName = imgName;
        }
        else
        {
            MogoUIManager.Instance.TryingSetSpriteName(imgName, temp);
        }

        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["WeaponFG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerBeltBG(string imgName, int color = 0)
    {
        UISlicedSprite temp = m_myTransform.Find(m_widgetToFullName["BeltBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0];
        temp.spriteName = imgName;
        MogoUtils.SetImageColor(temp, color);
    }

    private void SetPlayerBreastPlateBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["BreastPlateBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["BreastPlateBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerCuishBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["CuishBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["CuishBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerHandGuardBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["HandGuardBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["HandGuardBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerHeadEquipBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["HeadEquipBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["HeadEquipBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerNecklaceBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["NecklaceBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["NecklaceBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerRingLeftBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["RingLeftBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["RingLeftBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerRingRightBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["RingRightBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["RingRightBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerShoesBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["ShoesBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["ShoesBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerShoudersBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["ShoudersBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["ShoudersBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    private void SetPlayerWeaponBG(string imgName, int color = 0)
    {
        m_myTransform.Find(m_widgetToFullName["WeaponBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0].spriteName = imgName;
        MogoUtils.SetImageColor(m_myTransform.Find(m_widgetToFullName["WeaponBG"]).GetComponentsInChildren<UISlicedSprite>(true)[0], color);
    }

    #endregion

    #region 玩家名和等级

    private int m_level = 0;
    public int Level
    {
        get { return m_level; }
        set
        {
            m_level = value;
        }
    }

    /// <summary>
    /// 设置玩家名和等级
    /// </summary>
    /// <param name="name"></param>
    /// <param name="level"></param>
    public void SetPlayerInfoNameAndLevel(string name, int level)
    {
        Level = level;

        if (m_lblPlayerNameAndLevelText == null)
            m_lblPlayerNameAndLevelText = m_myTransform.Find(m_widgetToFullName["PlayerNameAndLevelText"]).GetComponentsInChildren<UILabel>(true)[0];
        m_lblPlayerNameAndLevelText.text = string.Concat(name, "  LV ", level);
    }

    #endregion
}
