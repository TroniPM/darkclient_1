#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VUInt8
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：8位无符号整数（byte）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 8位无符号整数（byte）。
    /// </summary>
    public class VUInt8 : VObject
    {
        private static VUInt8 m_instance = new VUInt8();
        public static VUInt8 Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VUInt8()
            : base(typeof(byte), VType.V_UINT8, Marshal.SizeOf(typeof(byte)))
        {
        }
        public VUInt8(Object vValue)
            : base(typeof(byte), VType.V_UINT8, vValue)
        {
        }

        public override byte[] Encode(object vValue)
        {
            return new byte[1] { Convert.ToByte(vValue) };
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            return data[index++];
        }
    }
}