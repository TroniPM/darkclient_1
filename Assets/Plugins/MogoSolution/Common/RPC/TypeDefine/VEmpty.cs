#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VEmpty
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.2.4
// 模块描述：空类型。
//----------------------------------------------------------------*/
#endregion
using System;

namespace Mogo.RPC
{
    /// <summary>
    /// 空类型。
    /// </summary>
    public class VEmpty : VObject
    {
        private static VEmpty m_instance = new VEmpty();
        public static VEmpty Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VEmpty()
            : base(typeof(object), VType.V_TYPE_ERR, 1) { }
        public VEmpty(Object vValue)
            : base(typeof(object), VType.V_TYPE_ERR, vValue) { }

        public override byte[] Encode(object vValue)
        {
            return new byte[0];
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            return new object();
        }
    }
}