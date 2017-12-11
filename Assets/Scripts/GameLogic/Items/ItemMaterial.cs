/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemCom
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public class ItemMaterial : ItemParent
{
    ItemParentInstance materialInstanceData;
    ItemMaterialData materialTemplateData;

    #region 实例数据
    #endregion

    #region 模板数据
    public override int level { get { return materialTemplateData.level; } }
    public int price { get { return materialTemplateData.price; } }
    public int useDescription { get { return materialTemplateData.useDescription; } }
    public override int cdTypes { get { return materialTemplateData.cdTypes; } }
    public override int cdTime { get { return materialTemplateData.cdTime; } }
    public override int useVocation { get { return materialTemplateData.useVocation; } }
    public override int useLevel { get { return materialTemplateData.useLevel; } }
    public override int vipLevel { get { return materialTemplateData.vipLevel; } }
    #endregion

    public ItemMaterial(ItemParentInstance _instanceData)
    {
        this.instanceData = _instanceData;
        materialInstanceData = _instanceData;

        LoggerHelper.Debug("id:" + _instanceData.templeId);
        LoggerHelper.Debug("ItemMaterialData.dataMap:" + ItemMaterialData.dataMap.Count);

        templateData = ItemMaterialData.dataMap[_instanceData.templeId];
        materialTemplateData = (ItemMaterialData)templateData;
    }
}