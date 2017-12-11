#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：LuaCharaterInfo
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.26
// 模块描述：角色信息实体。
//----------------------------------------------------------------*/
#endregion

namespace Mogo.Game
{
    /// <summary>
    /// 角色信息实体。对应 Account 实体的 avatarsInfo 的 Lua table 模板
    /// </summary>
    public class AvatarInfo
    {
        public ulong DBID { get; set; }
        public string Name { get; set; }
        public int Vocation { get; set; }
        public int Level { get; set; }
        /// <summary>
        /// 武器
        /// </summary>
        public int Weapon { get; set; }
        /// <summary>
        /// 胸甲
        /// </summary>
        public int Cuirass { get; set; }
        /// <summary>
        /// 鞋子
        /// </summary>
        public int Shoes { get; set; }
        /// <summary>
        /// 护手
        /// </summary>
        public int Armguard { get; set; }
        //public EquitmentInfo Equitment { get; set; }
    }
}
