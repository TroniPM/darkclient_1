/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemEquipment
// 创建者：Steven Yang
// 修改者列表：Joe Mo
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using System.Collections.Generic;
using System;

public class ItemEquipment : ItemParent
{
    ItemEquipmentData equipmentTemplateData;
    ItemEquipmentInstance equipmentInstanceData;

    //需要计算具体属性的值时指定，不实时更新
    private int m_l;
    public override int level
    {
        get { return m_l; }
        set { m_l = value; }
    }//父类已有

    #region 实例数据
    public new int id { get { return equipmentInstanceData.id; } }
    public new int bagType { get { return equipmentInstanceData.bagType; } set { equipmentInstanceData.bagType = value; } }
    public int sourceValue { get { return equipmentInstanceData.sourceKey; } }
    public int sourceKey { get { return equipmentInstanceData.sourceValue; } }
    public int bindingType { get { return equipmentInstanceData.bindingType; } }
    public List<int> jewelSlots
    {
        get
        {
            return equipmentInstanceData.jewelSlots;
        }
        set
        {
            equipmentInstanceData.jewelSlots = value;
        }
    }
    public bool locked { get { return equipmentInstanceData.locked; } set { equipmentInstanceData.locked = value; } }
    public bool isActive { get { return equipmentInstanceData.isActive; } set { equipmentInstanceData.isActive = value; } }
    #endregion

    #region 模板数据
    public int suitId { get { return equipmentTemplateData.suitId; } }
    public int levelNeed { get { return equipmentTemplateData.levelNeed; } }
    public int levelLimit { get { return equipmentTemplateData.levelLimit; } }
    public int score { get { return equipmentTemplateData.AttrDic[level].scores; } }
    public int vocation { get { return equipmentTemplateData.vocation; } }
    public List<int> jewelSlotsType { get { return equipmentTemplateData.jewelSlot; } }
    public Dictionary<int, int> enchant { get { return equipmentTemplateData.enchant; } }
    private Dictionary<int, ItemEquipValues> attrDic;
    public int hpA
    {
        get
        {
            //foreach(KeyValuePair<int,ItemEquipValues> pair in attrDic)
            //{
            //    Mogo.Util.LoggerHelper.Debug(pair.Key+","+pair.Value.hpBase);
            //}
            //Mogo.Util.LoggerHelper.Debug("level:"+level);
            return equipmentTemplateData.AttrDic[level].hpBase;
        }
    }

    public int hitValue { get { return equipmentTemplateData.AttrDic[level].hit; } }
    public int trueStrikeValue { get { return equipmentTemplateData.AttrDic[level].trueStrike; } }
    public int critValue { get { return equipmentTemplateData.AttrDic[level].crit; } }
    public int critExtraAtkValue { get { return equipmentTemplateData.AttrDic[level].critExtraAttack; } }
    public int atkA { get { return equipmentTemplateData.AttrDic[level].attackBase; } }
    public int defA { get { return equipmentTemplateData.AttrDic[level].defenseBase; } }

    public int antiDefValue { get { return equipmentTemplateData.AttrDic[level].antiDefense; } }
    public int antiCritValue { get { return equipmentTemplateData.AttrDic[level].antiCrit; } }
    public int antiTrueStrikeValue { get { return equipmentTemplateData.AttrDic[level].antiTrueStrike; } }
    public int cdReduce { get { return equipmentTemplateData.AttrDic[level].cdReduce; } }
    public int hpAddrate { get { return equipmentTemplateData.AttrDic[level].hpAddRate; } }
    public int pvpAdditionValue { get { return equipmentTemplateData.AttrDic[level].pvpAddition; } }
    public int pvpResistanceValue { get { return equipmentTemplateData.AttrDic[level].pvpAnti; } }
    #endregion

    public ItemEquipment(ItemEquipmentInstance _instanceData)
    {
        try
        {
            this.instanceData = _instanceData;
            equipmentInstanceData = _instanceData;

            LoggerHelper.Debug("id:" + _instanceData.templeId);

            if (!ItemEquipmentData.dataMap.ContainsKey(_instanceData.templeId)) LoggerHelper.Debug("OMG!!!!");
            templateData = ItemEquipmentData.dataMap[_instanceData.templeId];
            //templateData = new ItemEquipmentData();
            equipmentTemplateData = (ItemEquipmentData)templateData;
            //LoggerHelper.Debug("quality:" + quality + ",vocation:" + vocation + ",type:" + type + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            level = levelNeed;

        }
        catch (System.Exception e)
        {

            LoggerHelper.Debug(e);
        }
    }

    public int GetEnhantSum()
    {
        //Debug.LogError("GetEnhantSum");
        if (enchant == null)
        {
            //Debug.LogError("enchant == null");
            return 0;
        }
        foreach (int num in enchant.Values)
        {
            return num;
        }
        return 0;
    }

    public int GetEnhantMaterialId()
    {
        //Debug.LogError("GetEnhantSum");
        if (enchant == null)
        {
            //Debug.LogError("enchant == null");
            return 0;
        }
        foreach (int id in enchant.Keys)
        {
            return id;
        }
        return 0;
    }

    public int GetEnhantQuality()
    {
        //Debug.LogError("GetEnhantSum");
        if (enchant == null)
        {
            //Debug.LogError("enchant == null");
            return 1;
        }
        foreach (int id in enchant.Keys)
        {
            return ItemParentData.GetItem(id).quality;
        }
        return 1;
    }

    public List<string> GetAttrDescriptionList(int _level)
    {
        level = _level;
        if (level < levelNeed) level = levelNeed;
        if (level > levelLimit) level = levelLimit;

        Dictionary<int, string> attrs = new Dictionary<int, string>();

        //这里顺序对应属性编号顺序
        if (hpA > 0) attrs.Add(0, LanguageData.dataMap[201].Format(hpA));
        if (hpAddrate > 0) attrs.Add(1, LanguageData.dataMap[211].Format(hpAddrate / 100f));
        if (atkA > 0) attrs.Add(2, LanguageData.dataMap[202].Format(atkA));
        if (defA > 0) attrs.Add(3, LanguageData.dataMap[203].Format(defA));
        if (hitValue > 0) attrs.Add(4, LanguageData.dataMap[205].Format(hitValue));
        if (critValue > 0) attrs.Add(5, LanguageData.dataMap[204].Format(critValue));
        if (trueStrikeValue > 0) attrs.Add(6, LanguageData.dataMap[206].Format(trueStrikeValue));
        if (critExtraAtkValue > 0) attrs.Add(7, LanguageData.dataMap[207].Format(critExtraAtkValue));
        if (antiDefValue > 0) attrs.Add(8, LanguageData.dataMap[208].Format(antiDefValue));
        if (antiCritValue > 0) attrs.Add(9, LanguageData.dataMap[209].Format(antiCritValue));
        if (antiTrueStrikeValue > 0) attrs.Add(10, LanguageData.dataMap[210].Format(antiTrueStrikeValue));
        if (cdReduce > 0) attrs.Add(11, LanguageData.dataMap[212].Format(cdReduce / 100f));
        if (pvpAdditionValue > 0) attrs.Add(12, LanguageData.dataMap[213].Format(pvpAdditionValue));
        if (pvpResistanceValue > 0) attrs.Add(13, LanguageData.dataMap[214].Format(pvpResistanceValue));

        EquipValuesSortInfoData sortInfo = EquipValuesSortInfoData.dataMap.Get(type);
        List<string> attrsSort = new List<string>();
        for (int i = 0; i < sortInfo.sort.Count; i++)
        {
            if (!attrs.ContainsKey(sortInfo.sort[i] - 1)) continue;
            attrsSort.Add(attrs[sortInfo.sort[i] - 1]);
        }

        return attrsSort;
    }

    public string levelDesp
    {
        get
        {
            string desp = string.Empty;
            int index = 0;
            if (levelNeed == levelLimit)
            {
                index = 911;
                desp = levelNeed.ToString();
                if (levelNeed > MogoWorld.thePlayer.level)
                {
                    return MogoUtils.GetRedString(LanguageData.GetContent(index, desp));
                }
            }
            else
            {
                index = 913;
                int levelShow = Math.Max(MogoWorld.thePlayer.level, levelNeed);
                levelShow = Math.Min(levelShow, levelLimit);
                desp = levelShow + "/" + levelLimit;
            }
            return LanguageData.GetContent(index, desp);
        }
    }

    public int GetScore(int level)
    {
        return equipmentTemplateData.GetScore(level);
    }
}