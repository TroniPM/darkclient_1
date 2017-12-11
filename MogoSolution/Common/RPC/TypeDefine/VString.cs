#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VString
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：字符串（String）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Mogo.RPC
{
    /// <summary>
    /// 字符串（String）。
    /// </summary>
    public class VString : VObject
    {
        private static Encoding m_encoding = Encoding.UTF8;
        private static VString m_instance= new VString();
        public static VString Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VString()
            : base(typeof(String), VType.V_STR, 0)
        {
        }
        public VString(Object vValue)
            : base(typeof(String), VType.V_STR, vValue)
        {
        }

        public override byte[] Encode(Object vValue)
        {
            String value = (String)vValue;
            //byte[] encodeValues = m_encoding.GetBytes(value);
            //Array.Reverse(encodeValues);
            Encoder ec = m_encoding.GetEncoder();//获取字符编码
            Char[] charArray = value.ToCharArray();
            Int32 length = ec.GetByteCount(charArray, 0, charArray.Length, false);//获取字符串转换为二进制数组后的长度，用于申请存放空间
            Byte[] encodeValues = new Byte[length];//申请存放空间
            ec.GetBytes(charArray, 0, charArray.Length, encodeValues, 0, true);//将字符串按照特定字符编码转换为二进制数组

            return Utils.FillLengthHead(encodeValues);
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            Byte[] strData = CutLengthHead(data, ref index);
            return m_encoding.GetString(strData);
        }
    }
}
