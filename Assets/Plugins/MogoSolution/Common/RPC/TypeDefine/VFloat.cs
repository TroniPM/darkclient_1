#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VFloat
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.17
// 模块描述：字符串（String）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 32位浮点数（float）。
    /// </summary>
    public class VFloat : VObject
    {
        private static VFloat m_instance = new VFloat();
        public static VFloat Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VFloat()
            : base(typeof(float), VType.V_FLOAT32, Marshal.SizeOf(typeof(float)))
        {
        }
        public VFloat(Object vValue)
            : base(typeof(float), VType.V_FLOAT32, vValue)
        {
        }

        public override byte[] Encode(object vValue)
        {
            var result = BitConverter.GetBytes(Convert.ToSingle(vValue));
            ////Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] result = new Byte[VTypeLength];
            ////Array.Reverse(result);
            Buffer.BlockCopy(data, index, result, 0, VTypeLength);
            index += VTypeLength;

            return BitConverter.ToSingle(result, 0);
        }
    }
}