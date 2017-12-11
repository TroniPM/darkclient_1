#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VUInt64
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.4.23
// 模块描述：64位无符号整数（UInt64）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 64位无符号整数（UInt64）。
    /// </summary>
    public class VUInt64 : VObject
    {
        private static VUInt64 m_instance = new VUInt64();
        public static VUInt64 Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VUInt64()
            : base(typeof(UInt64), VType.V_UINT64, Marshal.SizeOf(typeof(UInt64)))
        {
        }
        public VUInt64(Object vValue)
            : base(typeof(UInt64), VType.V_UINT64, vValue)
        {
        }

        public override byte[] Encode(object vValue)
        {
            var result = BitConverter.GetBytes(Convert.ToUInt64(vValue));
            //Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] result = new Byte[VTypeLength];
            Buffer.BlockCopy(data, index, result, 0, VTypeLength);
            //Array.Reverse(result);
            index += VTypeLength;

            return BitConverter.ToUInt64(result, 0);
        }
    }
}