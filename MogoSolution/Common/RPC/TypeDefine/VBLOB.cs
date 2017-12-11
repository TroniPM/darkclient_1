#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VBLOB
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.17
// 模块描述：块数据。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Runtime.InteropServices;

namespace Mogo.RPC
{
    /// <summary>
    /// 布尔数（Boolean）。
    /// </summary>
    public class VBLOB : VObject
    {
        private static VBLOB m_instance = new VBLOB();
        public static VBLOB Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VBLOB()
            : base(typeof(char[]), VType.V_BLOB, 1)
        {
        }
        public VBLOB(Object vValue)
            : base(typeof(char[]), VType.V_BLOB, vValue)
        {
        }

        public override byte[] Encode(object vValue)
        {
            var result = BitConverter.GetBytes((Boolean)vValue);
            //Array.Reverse(result);
            return result;
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            //16位字节长度
            Byte[] len = new Byte[2];
            //Array.Reverse(len);
            Buffer.BlockCopy(data, index, len, 0, 2);
            index += 2;

            int l = BitConverter.ToUInt16(len, 0);

            Byte[] result = new Byte[l];
            //Array.Reverse(result);
            Buffer.BlockCopy(data, index, result, 0, l);
            index += l;

            return result;
        }
    }
}