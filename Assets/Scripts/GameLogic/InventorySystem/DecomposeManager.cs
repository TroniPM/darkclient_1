/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DecomposeManager
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-4-17
// 模块描述：装备分解系统
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mogo.GameData;
using Mogo.Util;
using Mogo.Game;

public class DecomposeManager
{
    public const string ON_DECOMPOSE_SHOW = "DecomposeManager.ON_DECOMPOSE_SHOW";
    public const string ON_EQUIP_SELECT = "DecomposeManager.ON_EQUIP_SELECT";
    public const string ON_LOCK_CHANGE = "DecomposeManager.ON_LOCK_CHANGE";
    public const string ON_DECOMPOSE = "DecomposeManager.ON_DECOMPOSE";
    public const string ON_CHECK_UP = "DecomposeManager.ON_CHECK_UP";
    public const string ON_DECOMPOSE_INBAG = "DecomposeManager.ON_DECOMPOSE_INBAG";
    public const string ON_CHOOSE_EQUIP_UP = "DecomposeManager.ON_CHOOSE_EQUIP_UP";
    //static public string lockInfoKey = "";

    const int QULITY_NEED_TIP = 5;//大于5都要提示
    Dictionary<int, ItemParent> m_equipmentDic;
    List<ItemEquipment> m_equipmentList = new List<ItemEquipment>();

    HashSet<int> m_checkSet = new HashSet<int>();
    //static HashSet<int> m_lockSet = new HashSet<int>();

    ItemParent m_currentEquipment;
    InventoryManager m_inventory;
    public static DecomposeManager Instance;
    int m_versionID;

    bool m_isDecomposeInDecUI = true;

    public DecomposeManager(Dictionary<int, ItemParent> _equipmentDic, InventoryManager _inventory)
    {
        Instance = this;
        m_inventory = _inventory;
        m_equipmentDic = _equipmentDic;
        m_versionID = m_inventory.m_versionId;
        AddListener();

    }

    //static public void InitLockSet()
    //{
    //    lockInfoKey = MogoWorld.thePlayer.dbid + "";
    //    HashSet<int> lockSet = new HashSet<int>();
    //    String lockInfo = PlayerPrefs.GetString(lockInfoKey, "");
    //    if (lockInfo == "")
    //    {
    //        m_lockSet = lockSet;
    //        return;
    //    }

    //    string[] temp = lockInfo.Split(',');
    //    for (int i = 0; i < temp.Length; i++)
    //    {
    //        lockSet.Add(int.Parse(temp[i]));
    //    }
    //    m_lockSet = lockSet;
    //}

    //private void SaveLockDic()
    //{
    //    string lockInfo = "";
    //    int count = 0;
    //    foreach (int index in m_lockSet)
    //    {
    //        lockInfo += index;
    //        count++;
    //        if (count != m_lockSet.Count)
    //            lockInfo += ",";
    //    }
    //    PlayerPrefs.SetString(lockInfoKey, lockInfo);
    //    PlayerPrefs.Save();
    //}

    private void AddListener()
    {
        EventDispatcher.AddEventListener(ON_DECOMPOSE_SHOW, OnDecomposeShow);
        EventDispatcher.AddEventListener<int>(ON_EQUIP_SELECT, OnEquipSelect);
        EventDispatcher.AddEventListener(ON_LOCK_CHANGE, OnLockChange);
        EventDispatcher.AddEventListener(ON_DECOMPOSE, OnDecompose);
        EventDispatcher.AddEventListener<int>(ON_CHECK_UP, OnCheckUp);
        EventDispatcher.AddEventListener<int>(ON_CHOOSE_EQUIP_UP, OnChooseEquipUp);
    }

    public void RemoveListener()
    {
        EventDispatcher.RemoveEventListener(ON_DECOMPOSE_SHOW, OnDecomposeShow);
        EventDispatcher.RemoveEventListener<int>(ON_EQUIP_SELECT, OnEquipSelect);
        EventDispatcher.RemoveEventListener(ON_LOCK_CHANGE, OnLockChange);
        EventDispatcher.RemoveEventListener(ON_DECOMPOSE, OnDecompose);
        EventDispatcher.RemoveEventListener<int>(ON_CHECK_UP, OnCheckUp);
        EventDispatcher.RemoveEventListener<int>(ON_CHOOSE_EQUIP_UP, OnChooseEquipUp);
    }

    private void OnCheckUp(int index)
    {
        if (!m_checkSet.Contains(index))
            m_checkSet.Add(index);
        else
        {
            m_checkSet.Remove(index);
        }

        if (m_checkSet.Count > 10)
        {
            m_checkSet.Remove(index);
            MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(1078), (rst) =>
            {
                if (rst)
                {
                    OnDecompose();
                }
                MogoGlobleUIManager.Instance.ConfirmHide();

            }, LanguageData.GetContent(1085), LanguageData.GetContent(1086));

            return;
        }
        else
        {
            DecomposeUIViewManager.Instance.SetCheckGridUp(index, m_checkSet.Contains(index));
        }
    }

    private void OnChooseEquipUp(int index)
    {
        if (DecomposeUIViewManager.Instance == null) return;
        DecomposeUIViewManager.Instance.SetEquipChoose(index);
        m_checkSet.Clear();
        RefreshUI();
    }

    private void DecomposeEquipList(List<ItemEquipment> list)
    {
        m_isDecomposeInDecUI = true;
        foreach (ItemEquipment equip in list)
        {
            DecomposeReq(equip.id, equip.gridIndex);
        }
    }

    //按下分解按钮时
    private void OnDecompose()
    {
        LoggerHelper.Debug("OnDecompose");
        if (m_checkSet == null || m_checkSet.Count <= 0)
        {
            MogoMsgBox.Instance.ShowMsgBox(LanguageData.GetContent(1077));
            return;
        }

        //得到装备列表
        List<ItemEquipment> normalEquipList = new List<ItemEquipment>();
        List<ItemEquipment> advancedEquipList = new List<ItemEquipment>();
        foreach (int index in m_checkSet)
        {
            LoggerHelper.Debug("m_equipmentList[pair.Key].gridIndex:" + m_equipmentList[index].gridIndex);
            if (m_equipmentList[index].quality > QULITY_NEED_TIP)
            {
                advancedEquipList.Add(m_equipmentList[index]);
            }
            else
            {
                normalEquipList.Add(m_equipmentList[index]);
            }
        }
        //如果有暗金装备提示
        if (advancedEquipList.Count > 0)
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(1079), (rst) =>
            {
                if (rst)
                {
                    DecomposeEquipList(advancedEquipList);
                }
                DecomposeEquipList(normalEquipList);

                MogoGlobleUIManager.Instance.ConfirmHide();

            }, LanguageData.GetContent(1076), LanguageData.GetContent(1086));
        }
        else//没就全部分解
        {
            DecomposeEquipList(normalEquipList);
        }

    }

    private void OnLockChange()
    {
        int index = m_currentEquipment.gridIndex;
        Mogo.Util.LoggerHelper.Debug("OnLockChange:" + index);
        //bool isLock;
        //isLock = ((ItemEquipment)m_currentEquipment).locked = !((ItemEquipment)m_currentEquipment).locked;

        //if (m_lockSet.Contains(index))
        //{
        //    m_lockSet.Remove(index);
        //    isLock = false;
        //}
        //else
        //{
        //    m_lockSet.Add(index);
        //    isLock = true;
        //}


        MogoWorld.thePlayer.RpcCall("ReqForLock", m_currentEquipment.id, (byte)index);
        //SaveLockDic();
    }

    public void LockChange(byte index, byte errorId)
    {
        Mogo.Util.LoggerHelper.Debug("LockChange:" + index + ",errorid:" + errorId);
        EquipTipManager.Instance.CloseEquipTip();
        if (errorId != 0) return;
        bool isLock = ((ItemEquipment)m_equipmentDic[index]).locked = !((ItemEquipment)m_equipmentDic[index]).locked;
        DecomposeUIViewManager.Instance.ShowPackageGridLock(isLock, index);

        for (int i = 0; i < m_equipmentList.Count; i++)
        {
            if (m_equipmentList[i].gridIndex != index) continue;
            if (isLock) m_checkSet.Remove(i);
            DecomposeUIViewManager.Instance.ShowPackageGridCheck(!isLock, i);
        }


        DecomposeUIViewManager.Instance.ShowPacakgeGridCheckFG(false, index);
        RefreshUI();
    }

    private void OnDecomposeShow()
    {        
        //if (m_versionID >= m_inventory.m_versionId) return;
        DecomposeUILogicManager.Instance.ChooseDecomposeEquipType = ChooseDecomposeEquip.Waste;
    }

    public void RefreshUI()
    {
        if (DecomposeUIViewManager.Instance == null) return;
        for (int i = 0; i < BodyName.names.Length; i++)
        {
            DecomposeUIViewManager.Instance.SetPackageGridImage(BodyName.names[i], i);
        }

        UpdateEquipmentList();


        for (int i = 0; i < 40; i++)
        {
            string icon = IconData.none;
            bool isLock = false;
            bool hasCheckBox = false;
            int quality = 0;
            if (m_equipmentList.Count > i)
            {
                icon = m_equipmentList[i].icon;

                hasCheckBox = true;
                quality = m_equipmentList[i].quality;
                if (((ItemEquipment)m_equipmentList[i]).locked)
                {
                    isLock = true;
                    hasCheckBox = false;
                }

                if (m_checkSet.Count < 10)
                {
                    if (!isLock)
                        OnCheckUp(i);
                }
            }

            DecomposeUIViewManager.Instance.ShowPackageGridCheck(hasCheckBox, i);
            DecomposeUIViewManager.Instance.ShowPacakgeGridCheckFG(m_checkSet.Contains(i), i);
            DecomposeUIViewManager.Instance.ShowPackageGridLock(isLock, i);
            DecomposeUIViewManager.Instance.SetPackageGridImage(icon, i);
            if (quality != 0)
            {
                DecomposeUIViewManager.Instance.SetPackageGridImageBG(IconData.GetIconByQuality(quality), i);
            }
            else
            {
                DecomposeUIViewManager.Instance.SetPackageGridImageBG(IconData.blankBox, i);
            }

        }

    }

    private void UpdateEquipmentList()
    {
        HashSet<int> gridIndexCheckSet = new HashSet<int>();
        foreach (int index in m_checkSet)
        {
            gridIndexCheckSet.Add(m_equipmentList[index].gridIndex);
        }
        m_checkSet.Clear();

        m_equipmentList.Clear();
        foreach (ItemParent item in m_equipmentDic.Values)
        {
            // 物品类型不是装备
            if (item.itemType != 1) continue;

            // 当前选择垃圾装备,并且该装备比已穿装备好
            if (DecomposeUILogicManager.Instance.ChooseDecomposeEquipType == ChooseDecomposeEquip.Waste
                && !InventoryManager.Instance.IsRubbish(item.templateId,4))
            {
                continue;
            }

            m_equipmentList.Add((ItemEquipment)item);
        }


        m_equipmentList.Sort(delegate(ItemEquipment a, ItemEquipment b)
        {
            if (a.quality > b.quality) return 1;
            else if (a.quality < b.quality) return -1;
            else
            {
                if (a.level >= b.level) return 1;
                else return -1;
            }
        });

        for (int i = 0; i < m_equipmentList.Count; i++)
        {
            if (gridIndexCheckSet.Contains(m_equipmentList[i].gridIndex))
            {
                m_checkSet.Add(i);
            }
        }
    }

    private void OnEquipSelect(int index)
    {
        if (index >= m_equipmentList.Count) return;
        m_currentEquipment = m_equipmentList[index];
        ShowEquipInfo(m_currentEquipment);
    }

    public void ShowEquipInfo(ItemParent item)
    {
        List<ButtonInfo> btnList = new List<ButtonInfo>();
        ButtonInfo btn = new ButtonInfo() { action = OnLockChange };
        int textId = 0;
        if (((ItemEquipment)item).locked)
        {
            textId = 1081;
        }
        else
        {
            textId = 1082;
        }
        btn.text = LanguageData.GetContent(textId);
        btn.id = textId;
        btnList.Add(btn);
        InventoryManager.Instance.ShowEquipTip(item as ItemEquipment, btnList, MogoWorld.thePlayer.level);
    }

    public void DecomposeResp(byte index, byte errorId, byte hasJewel)
    {
        ShowErrorInfo(errorId);
        if (errorId != 0) 
            return;

        DecomposeUILogicManager.Instance.ChooseDecomposeEquipType = DecomposeUILogicManager.Instance.ChooseDecomposeEquipType;

        //if (m_checkDic.ContainsKey(index))
        //{
        //    m_checkDic.Remove(index);
        //}
        EquipTipManager.Instance.CloseEquipTip();

        if (hasJewel > 0)
        {
            if (m_isDecomposeInDecUI)
            {
                DecomposeUIViewManager.Instance.ShowDecomposeJewlTip();
            }
            else
            {
                MogoMsgBox.Instance.ShowMsgBox(LanguageData.GetContent(1088));
            }
        }
    }

    private void ShowErrorInfo(byte errorId)
    {
        int index = 470;
        string msg = "";
        switch (errorId)
        {
            case 0:
                msg = LanguageData.dataMap[index].content;
                return;
            case 1:
                msg = LanguageData.dataMap[index + 1].content;
                break;
            case 2:
                msg = LanguageData.dataMap[index + 2].content;
                break;
            case 3:
                msg = LanguageData.dataMap[index + 3].content;
                break;
            case 4:
                msg = LanguageData.dataMap[index + 4].content;
                break;
            case 5:
                msg = LanguageData.dataMap[index + 5].content;
                break;
            case 6:
                msg = LanguageData.dataMap[index + 6].content;
                break;
        }

        MogoMsgBox.Instance.ShowMsgBox(msg);
    }

    //分解单个
    public void Decompose(int id, int gridIndex)
    {
        m_isDecomposeInDecUI = false;

        LoggerHelper.Debug("DecomposeQuality:" + m_equipmentDic[gridIndex].quality);
        if (((ItemEquipment)m_equipmentDic[gridIndex]).locked)
        {
            MogoMsgBox.Instance.ShowMsgBox(LanguageData.GetContent(1083));
            return;
        }
        if (m_equipmentDic[gridIndex].quality > QULITY_NEED_TIP)
        {
            MogoGlobleUIManager.Instance.Confirm(LanguageData.GetContent(1084, m_equipmentDic[gridIndex].name), (rst) =>
            {
                if (rst)
                {
                    DecomposeReq(id, gridIndex);
                    MogoGlobleUIManager.Instance.ConfirmHide();
                }
                else
                {
                    MogoGlobleUIManager.Instance.ConfirmHide();
                }
            });
        }
        else
        {
            DecomposeReq(id, gridIndex);
        }

    }
    static public void DecomposeReq(int id, int gridIndex)
    {
        MogoWorld.thePlayer.RpcCall("DecomposeEquipment", id, gridIndex);
    }
}