/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
// 模块名：FumoManager
// 创建者：莫卓豪
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class FumoManager
{
    enum ErrorCode
    {
        successful = 0,          //成功
        error_id_not_found = 1,  //未找到相应配置
        error_fumo_info_nil = 2, //附魔信息不存在
        error_fumo_index_not_found = 3,//未找到该部位的附魔信息
        error_pos_item_not_found = 4,//该部位没有道具
        error_cost_not_enough = 5, //材料不够
        error_no_job = 6, //没有未完成的附魔
        error_replace_index_nil = 7, //选择替换附魔的序号有误
        error_have_job = 8, //你有未完成的流程
        error_equip_cant_fumo = 9, //该装备不能附魔
        error_job_save_error = 10//未完成流程数据有误
    }

    public static FumoManager Instance;
    private bool m_isInit = true;
    EntityParent m_Entity;
    Dictionary<int, Dictionary<int, List<float>>> m_data;
    private int m_currentSlot;
    private bool m_isReplacing = false;

    public FumoManager(EntityParent entity)
    {
        m_Entity = entity;
        Instance = this;
        GetFumoInfo();
    }

    private void GetFumoInfo()
    {
        m_Entity.RpcCall("GetFumoInfo", 0);
    }

    public void GetFumoInfoResp(byte slot, LuaTable info)
    {
        //LoggerHelper.Debug("GetFumoInfoResp:" + slot + ",info:" + info);
        //更新info
        object obj;
        if (slot == 0)
        {
            Utils.ParseLuaTable(info, typeof(Dictionary<int, Dictionary<int, List<float>>>), out obj);
            m_data = obj as Dictionary<int, Dictionary<int, List<float>>>;
        }
        else
        {
            Utils.ParseLuaTable(info, typeof(Dictionary<int, List<float>>), out obj);
            m_data[slot] = obj as Dictionary<int, List<float>>;
        }
        if (!m_isInit)
        {
            //提示附魔
            ShowFumoInfo();
        }
        m_isInit = false;
    }

    private void ShowFumoInfo()
    {
        LoggerHelper.Debug("ShowFumoInfo");
        int currentSlot;
        int replaceIndex;
        if (HasFumoProccess(out currentSlot))
        {
            m_currentSlot = currentSlot;

            FumoUIViewManager.Instance.CloseUI();
            if (NeedToReplace(currentSlot, out replaceIndex))
            {
                m_isReplacing = true;
                string newFumo = GetNewFumoDesp(currentSlot);
                string oldFumo = GetToRelpaceFumoDesp(currentSlot);
                string tip = LanguageData.GetContent(1357);//  LanguageData.GetContent(1352, newFumo, oldFumo);

                List<string> oldFumoList = new List<string>();
                oldFumoList.Add(oldFumo + LanguageData.GetContent(1360));

                bool isBetter = IsBetter(currentSlot, replaceIndex);
                string tip2 = string.Empty;
                string cancelBtnName = LanguageData.GetContent(1364);
                if (isBetter)
                {
                    FumoReplaceReq(currentSlot, replaceIndex);
                    tip2 = LanguageData.GetContent(1362, newFumo, oldFumo);
                }
                else
                {
                    tip2 = LanguageData.GetContent(1361, newFumo);
                    FumoReplaceReq(currentSlot, 0);
                }

                FumoUIViewManager.Instance.ShowFull(LanguageData.GetContent(1350, newFumo), oldFumoList, tip, cancelBtnName
, null
, () =>
{
    m_isReplacing = false;
    FumoUIViewManager.Instance.CloseUI();
    InventoryManager.Instance.OnEquipGridUp(m_currentSlot);
}, tip2);
                EquipTipManager.Instance.CloseEquipTip();

            }
            else if (IsFull(currentSlot))
            {
                string cancelBtnName = LanguageData.GetContent(1363);
                LoggerHelper.Debug("IsFull");
                string tip = LanguageData.GetContent(1358);
                string newFumo = GetNewFumoDesp(currentSlot);
                newFumo = LanguageData.GetContent(1350, newFumo);
                List<string> oldFumoList = GetToRelpaceFumoListDesp(currentSlot);
                for (int i = 0; i < oldFumoList.Count; i++)
                {
                    oldFumoList[i] += LanguageData.GetContent(1360);
                }
                FumoUIViewManager.Instance.ShowFull(newFumo, oldFumoList, tip, cancelBtnName
                    , (index) => { FumoReplaceReq(currentSlot, index + 1); }
                    , () => { FumoReplaceReq(currentSlot, 0); });
                EquipTipManager.Instance.CloseEquipTip();
            }

        }
        else if (!m_isReplacing)
        {
            ShowRespInfo(0);
            FumoUIViewManager.Instance.CloseUI();
            EquipTipManager.Instance.CloseEquipTip();

            InventoryManager.Instance.OnEquipGridUp(m_currentSlot);
        }
    }

    private bool IsBetter(int currentSlot, int replaceIndex)
    {
        Dictionary<int, List<float>> dic = m_data[currentSlot];
        float newValue = dic[0][1];
        float oldValue = dic[replaceIndex][1];
        if (oldValue > newValue) return false;
        else return true;
    }

    private string GetToRelpaceFumoDesp(int currentSlot)
    {
        Dictionary<int, List<float>> dic = m_data[currentSlot];
        foreach (KeyValuePair<int, List<float>> pair in dic)
        {
            if (pair.Key == 0) continue;
            if (pair.Value[0] == dic[0][0])
            {
                float num = pair.Value[1];
                string propName = GetPropName(num,pair.Value[0]);
                return propName;// LanguageData.GetContent(1353, num, propName);
            }
        }
        return string.Empty;
    }

    private List<string> GetToRelpaceFumoListDesp(int currentSlot)
    {
        if (!m_data.ContainsKey(currentSlot)) return new List<string>();
        Dictionary<int, List<float>> dic = m_data[currentSlot];
        List<string> despList = new List<string>();

        foreach (KeyValuePair<int, List<float>> pair in dic)
        {
            if (pair.Key == 0) continue;
            float num = pair.Value[1];
            string propName = GetPropName(num,pair.Value[0]);
            despList.Add(propName);
        }
        return despList;

    }

    private string GetNewFumoDesp(int currentSlot)
    {
        Dictionary<int, List<float>> dic = m_data[currentSlot];
        float propId = dic[0][0];
        float num = dic[0][1];
        string propName = GetPropName(num,propId);
        return propName;//LanguageData.GetContent(1353, num, propName);
    }

    private string GetPropName(float num,float propId)
    {
        return LanguageData.GetContent(549 + (int)propId, num);
    }

    private bool NeedToReplace(int slot, out int indexToRelpace)
    {
        Dictionary<int, List<float>> dic = m_data[slot];
        indexToRelpace = 0;
        if (!dic.ContainsKey(0)) return false;
        foreach (KeyValuePair<int, List<float>> pair in dic)
        {
            if (pair.Key == 0) continue;
            if (pair.Value[0] == dic[0][0])
            {
                indexToRelpace = pair.Key;
                return true;
            }
        }
        return false;
    }

    private bool IsFull(int slot)
    {
        ItemEquipment equip = InventoryManager.Instance.EquipOnDic[slot];
        Dictionary<int, int> enhant = equip.enchant;
        int fumoSum = 0;
        foreach (int num in enhant.Values)
        {
            fumoSum = num;
            break;
        }
        if (m_data[slot].Count >= fumoSum) return true;
        return false;
    }

    private bool HasFumoProccess(out int slot)
    {
        foreach (KeyValuePair<int, Dictionary<int, List<float>>> pair in m_data)
        {
            if (pair.Value.ContainsKey(0))
            {
                slot = pair.Key;
                return true;
            }
        }
        slot = 0;
        return false;
    }

    public void Fumo(int slot)
    {
        LoggerHelper.Debug(slot);
        int currentSlot = slot;
        if (HasFumoProccess(out currentSlot))
        {
            //提示附魔
            ShowFumoInfo();
        }
        else
        {
            //FumoReq(slot);
            //m_currentSlot = slot;

            ItemEquipment equip = InventoryManager.Instance.EquipOnDic[slot];
            ItemParentData material = ItemParentData.GetItem(equip.GetEnhantMaterialId());
            if (InventoryManager.Instance.GetItemNumById(material.id) <= 0)
            {
                MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(1385, material.Name));
                return;
            }

            EquipTipManager.Instance.CloseEquipTip();
            string title = LanguageData.GetContent(28120, material.Name);
            List<string> contentList = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                contentList.Add(LanguageData.GetContent(28113 + i));
            }
            FumoUIViewManager.Instance.ShowFumoProcess(title, contentList,
                () =>
                {
                    FumoReq(slot);
                    m_currentSlot = slot;
                }
                , () =>
                {
                    FumoUIViewManager.Instance.CloseUI();
                    InventoryManager.Instance.OnEquipGridUp(m_currentSlot);
                });

        }
    }

    private void FumoReq(int slot)
    {
        LoggerHelper.Debug("fumo:" + slot);
        m_Entity.RpcCall("fumo", slot);
    }

    private void FumoReplaceReq(int slot, int index)
    {
        m_Entity.RpcCall("fumo_replace", slot, index);

    }

    public void FumoResp(byte slot, byte errorId)
    {
        LoggerHelper.Debug("FumoResp:" + errorId);
        if (errorId == 0) return;
        ShowRespInfo(errorId, slot);

    }

    public void ShowRespInfo(int errorId, int slot = 0)
    {
        if (errorId == 5 && slot != 0 && InventoryManager.Instance.EquipOnDic.ContainsKey(slot))
        {
            ItemEquipment equip = InventoryManager.Instance.EquipOnDic.Get(slot);

            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(1385, ItemParentData.GetItem(equip.GetEnhantMaterialId()).Name));
        }
        else if (errorId != 0)
        {

            MogoMsgBox.Instance.ShowFloatingText(LanguageData.GetContent(errorId + 1380));
        }
    }

    public FumoTipUIInfo GetFumoTipUIInfo(int slot)
    {
        int fomoAvaliableNum;
        int currentFomoNum;

        FumoTipUIInfo info = new FumoTipUIInfo();

        fomoAvaliableNum = InventoryManager.Instance.EquipOnDic[slot].GetEnhantSum();
        int quality = InventoryManager.Instance.EquipOnDic[slot].GetEnhantQuality();
        List<string> despList = GetToRelpaceFumoListDesp(slot);
        if (!m_data.ContainsKey(slot))
        {
            currentFomoNum = 0;
        }
        else
        {
            currentFomoNum = despList.Count;
        }
        if (fomoAvaliableNum == 0 && currentFomoNum == 0) return null;

        string hasNotActiveStr = LanguageData.GetContent(1355);
        string hasNotEnhantStr = LanguageData.GetContent(1356);
        int count = Mathf.Max(fomoAvaliableNum, currentFomoNum);

        info.fomoDesp = new List<string>();
        for (int i = 1; i <= count; i++)
        {
            string temp = string.Empty;
            if (i > currentFomoNum)
            {
                temp = hasNotEnhantStr;
            }
            else if (i > fomoAvaliableNum)
            {
                temp = despList[i - 1] + hasNotActiveStr;
            }
            else
            {
                temp = despList[i - 1];
            }
            info.fomoDesp.Add(MogoUtils.GetStrWithQulityColor(temp, quality));
        }

        info.fumoTitle = LanguageData.GetContent(1354, currentFomoNum, fomoAvaliableNum);
        info.fumoTitle = MogoUtils.GetStrWithQulityColor(info.fumoTitle, quality);
        return info;
    }

    public void FumoReplaceResp(byte slot, byte index, byte errorId)
    {
        LoggerHelper.Debug("FumoReplaceResp,index:" + index + ",errorId:" + errorId);
        if (errorId == 0) return;
        ShowRespInfo(errorId);
    }
}