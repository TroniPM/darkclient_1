#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VBoolean
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.17
// 模块描述：双精度浮点数（Boolean）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 
    /// </summary>
    public class VDouble : VObject
    {
        private static VDouble m_instance = new VDouble();
        public static VDouble Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VDouble()
            : base(typeof(Double), VType.V_FLOAT64, Marshal.SizeOf(typeof(Double)))
        {
        }
        public VDouble(Object vValue)
            : base(typeof(Double), VType.V_FLOAT64, vValue)
        {
        }

        public override byte[] Encode(object vValue)
        {
            var result = BitConverter.GetBytes(Convert.ToDouble(vValue));
            ////Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] result = new Byte[VTypeLength];
            ////Array.Reverse(result);
            Buffer.BlockCopy(data, index, result, 0, VTypeLength);
            index += VTypeLength;

            return BitConverter.ToDouble(result, 0);
        }
   }
}
