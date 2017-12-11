#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VBoolean
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.17
// 模块描述：布尔数（Boolean）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 布尔数（Boolean）。
    /// </summary>
    public class VBoolean : VObject
    {
        private static VBoolean m_instance = new VBoolean();
        public static VBoolean Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VBoolean()
            : base(typeof(Boolean), VType.V_BLOB, 1)
        {
        }
        public VBoolean(Object vValue)
            : base(typeof(Boolean), VType.V_BLOB, vValue)
        {
        }

        public override byte[] Encode(object vValue)
        {
            var result = BitConverter.GetBytes(Convert.ToBoolean(vValue));
            //Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] result = new Byte[VTypeLength];
            //Array.Reverse(result);
            Buffer.BlockCopy(data, index, result, 0, VTypeLength);
            index += VTypeLength;

            return BitConverter.ToBoolean(result, 0);
        }
    }
}