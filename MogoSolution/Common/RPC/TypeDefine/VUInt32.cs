#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VUInt32
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：32位无符号整数（UInt32）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 32位无符号整数（UInt32）。
    /// </summary>
    public class VUInt32 : VObject
    {
        private static VUInt32 m_instance = new VUInt32();
        public static VUInt32 Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VUInt32()
            : base(typeof(UInt32), VType.V_UINT32, Marshal.SizeOf(typeof(UInt32)))
        {
        }
        public VUInt32(Object vValue)
            : base(typeof(UInt32), VType.V_UINT32, vValue)
        {
        }

        public override byte[] Encode(object vValue)
        {
            var result = BitConverter.GetBytes(Convert.ToUInt32(vValue));
            //Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] result = new Byte[VTypeLength];
            Buffer.BlockCopy(data, index, result, 0, VTypeLength);
            //Array.Reverse(result);
            index += VTypeLength;

            return BitConverter.ToUInt32(result, 0);
        }
    }
}