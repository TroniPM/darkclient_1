#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VInt16
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：16位整数（Int16）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 16位整数（Int16）。
    /// </summary>
    public class VInt16 : VObject
    {
        private static VInt16 m_instance= new VInt16();
        public static VInt16 Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VInt16()
            : base(typeof(Int16), VType.V_INT16, Marshal.SizeOf(typeof(Int16)))
        {
        }
        public VInt16(Object vValue)
            : base(typeof(Int16), VType.V_INT16, vValue)
        {
        }

        public override byte[] Encode(object vValue)
        {
            var result = BitConverter.GetBytes(Convert.ToInt16(vValue));
            //Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] result = new Byte[VTypeLength];
            //Array.Reverse(result);
            Buffer.BlockCopy(data, index, result, 0, VTypeLength);
            index += VTypeLength;

            return BitConverter.ToInt16(result, 0);
        }
    }
}