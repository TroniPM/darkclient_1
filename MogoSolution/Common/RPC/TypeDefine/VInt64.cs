#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VInt64
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
    public class VInt64 : VObject
    {
        private static VInt64 m_instance = new VInt64();
        public static VInt64 Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VInt64()
            : base(typeof(Int64), VType.V_INT64, Marshal.SizeOf(typeof(Int64)))
        {
        }
        public VInt64(Object vValue)
            : base(typeof(Int64), VType.V_INT64, vValue)
        {
        }

        public override byte[] Encode(object vValue)
        {
            var result = BitConverter.GetBytes(Convert.ToInt64(vValue));
            //Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] result = new Byte[VTypeLength];
            Buffer.BlockCopy(data, index, result, 0, VTypeLength);
            //Array.Reverse(result);
            index += VTypeLength;

            return BitConverter.ToInt64(result, 0);
        }
    }
}