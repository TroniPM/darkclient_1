/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemJewel
// 创建者：Steven Yang
// 修改者列表：Joe Mo
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class ItemJewel : ItemParent
{


    ItemJewelInstance jewelInstanceData;
    ItemJewelData jewelTemplateData;

    #region 实例数据
    public int bindingType
    {
        get
        {
            if (jewelInstanceData == null) return 0;
            return jewelInstanceData.bindingType;
        }
    }
    #endregion


    #region 模板数据
    public int skillEfectId { get { return jewelTemplateData.skillEfectId; } }
    public string effectDescription { get { return jewelTemplateData.effectDescriptionStr; } }
    public override int level { get { return jewelTemplateData.level; } }
    public string typeName { get { return jewelTemplateData.typeName; } }
    public int price { get { return jewelTemplateData.price; } }
    public int propertyEffectId { get { return jewelTemplateData.propertyEffectId; } }
    public int score { get { return jewelTemplateData.score; } }
    public List<int> slotType { get { return jewelTemplateData.slotType; } }
    #endregion


    public ItemJewel(ItemJewelData data)
    {
        templateData = data;
        jewelTemplateData = data;
    }

    public ItemJewel(ItemJewelInstance _instanceData)
    {
        this.instanceData = _instanceData;
        jewelInstanceData = _instanceData;

        LoggerHelper.Debug("id:" + _instanceData.templeId);
        if (!ItemJewelData.dataMap.ContainsKey(_instanceData.templeId))
        {
            LoggerHelper.Error("can not find jewel: " + _instanceData.templeId);
            return;
        }
        templateData = ItemJewelData.dataMap[_instanceData.templeId];
        jewelTemplateData = (ItemJewelData)templateData;
    }
}