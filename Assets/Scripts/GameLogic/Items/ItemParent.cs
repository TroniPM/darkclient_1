/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ItemParent
// 创建者：Steven Yang
// 修改者列表：Joe Mo
// 创建日期：
// 模块描述：道具实例数据结构
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;

public abstract class ItemParent
{
    protected ItemParentInstance instanceData;
    protected ItemParentData templateData;

    #region 实例数据
    public int id { get { return instanceData.id; } }
    public int gridIndex { get { return instanceData.gridIndex; } set { instanceData.gridIndex = value; } }
    public int templateId { get { return instanceData.templeId; } set { templateId = value; } }
    public int stack { get { return instanceData.stack; } set { stack = value; } }
    public int bagType { get { return instanceData.bagType; } }
    #endregion

    #region 模板数据.
    public string name { get { return templateData.Name; } }
    public int description { get { return templateData.description; } }
    public string Description { get { return templateData.Description; } }
    public string icon { get { return templateData.Icon; } }
    public int color { get { return templateData.color; } }
    public int look { get { return templateData.look; } }
    public int type { get { return templateData.type; } }
    public byte subtype { get { return templateData.subtype; } }
    public byte quality { get { return templateData.quality; } }
    public short maxStack { get { return templateData.maxStack; } }
    public int itemType { get { return templateData.itemType; } }
    public int effectId { get { return templateData.effectId; } }

    public virtual int useLevel { get { return templateData.useLevel; } }
    public virtual int vipLevel { get { return templateData.vipLevel; } }
    public virtual int cdTypes { get { return templateData.cdTypes; } }
    public virtual int cdTime { get { return templateData.cdTime; } }
    public virtual int useVocation { get { return templateData.useVocation; } }
    public virtual int level { get { return templateData.level; } set { } }
    #endregion
}

