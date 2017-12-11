#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VInt32
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：32位整数（Int32）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 32位整数（Int32）。
    /// </summary>
    public class VInt32 : VObject
    {
        private static VInt32 m_instance = new VInt32();
        public static VInt32 Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VInt32()
            : base(typeof(Int32), VType.V_INT32, Marshal.SizeOf(typeof(Int32)))
        {
        }
        public VInt32(Object vValue)
            : base(typeof(Int32), VType.V_INT32, vValue)
        {
        }

        public override Byte[] Encode(Object vValue)
        {
            var result = BitConverter.GetBytes(Convert.ToInt32(vValue));
            //Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] result = new Byte[VTypeLength];
            //Array.Reverse(result);
            Buffer.BlockCopy(data, index, result, 0, VTypeLength);
            index += VTypeLength;

            return BitConverter.ToInt32(result, 0);
        }
    }
}