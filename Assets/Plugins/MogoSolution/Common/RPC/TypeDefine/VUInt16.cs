#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VUInt16
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：16位无符号整数（UInt16）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 16位无符号整数（UInt16）。
    /// </summary>
    public class VUInt16 : VObject
    {
        private static VUInt16 m_instance = new VUInt16();
        public static VUInt16 Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VUInt16()
            : base(typeof(UInt16), VType.V_UINT16, Marshal.SizeOf(typeof(UInt16)))
        {
        }
        public VUInt16(Object vValue)
            : base(typeof(UInt16), VType.V_UINT16, vValue)
        {
        }

        public override Byte[] Encode(Object vValue)
        {
            var result = BitConverter.GetBytes(Convert.ToUInt16(vValue));
            //Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] result = new Byte[VTypeLength];
            Buffer.BlockCopy(data, index, result, 0, VTypeLength);
            //Array.Reverse(result);
            index += VTypeLength;

            return BitConverter.ToUInt16(result, 0);
        }
    }
}