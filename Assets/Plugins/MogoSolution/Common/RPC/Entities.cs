#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：AttachedEntity
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.4.17
// 模块描述：实体获取数据结构定义。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
namespace Mogo.RPC
{
    public class AttachedInfo
    {
        public uint id { get; set; }
        public ushort typeId { get; set; }
        public EntityDef entity { get; set; }
        public List<KeyValuePair<EntityDefProperties, object>> props { get; set; }
    }

    public class CellAttachedInfo : AttachedInfo
    {
        //public byte face { get; set; }
        public short x { get; set; }
        public short y { get; set; }
        public uint checkFlag { get; set; }
        public UnityEngine.Vector3 position { get { return new UnityEngine.Vector3(x * 0.01f, 0, y * 0.01f); } }//服务器坐标以厘米为单位，客户端以米为单位
        //public UnityEngine.Vector3 rotation { get { return new UnityEngine.Vector3(0, face * 2, 0); } }//服务器朝向值为0-180，客户端直接放大一倍
    }

    public class BaseAttachedInfo : AttachedInfo
    {
        public ulong dbid { get; set; }
    }
}