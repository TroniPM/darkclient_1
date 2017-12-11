#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VObject
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：类型声明抽象类。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Mogo.RPC
{
    /// <summary>
    /// 类型声明抽象类。
    /// </summary>
    public abstract class VObject
    {
        #region Properties

        private VType m_vType;
        private Object m_vValue;
        private Type m_vValueType;
        private Int32 m_vTypeLength;

        /// <summary>
        /// 类型标识。
        /// </summary>
        public VType VType
        {
            get { return m_vType; }
        }

        /// <summary>
        /// 表示类型声明。
        /// </summary>
        public Type VValueType
        {
            get { return m_vValueType; }
        }

        /// <summary>
        /// 类型的二进制长度。
        /// </summary>
        public Int32 VTypeLength
        {
            get { return m_vTypeLength; }
        }

        /// <summary>
        /// 值。
        /// </summary>
        public Object VValue
        {
            get { return m_vValue; }
            set { m_vValue = value; }
        }

        #endregion

        #region Constructor

        private VObject(Type vValueType)
        {
            m_vValueType = vValueType;
        }

        private VObject(Type vValueType, VType vType)
            : this(vValueType)
        {
            m_vType = vType;
        }

        protected VObject(Type vValueType, VType vType, Int32 vTypeLength)
            : this(vValueType, vType)
        {
            m_vTypeLength = vTypeLength;
        }       

        protected VObject(Type vValueType, VType vType, Object vValue)
            : this(vValueType, vType)
        {
            m_vValue = vValue;
        }

        #endregion

        #region Functions

        /// <summary>
        /// 将数据编码成二进制数组。
        /// </summary>
        /// <param name="vValue">源数据</param>
        /// <returns>二进制数组</returns>
        public abstract Byte[] Encode(Object vValue);

        /// <summary>
        /// 将二进制数组解码成数据。
        /// </summary>
        /// <param name="data">源二进制数组</param>
        /// <param name="index">对应数据索引偏移</param>
        /// <returns>数据</returns>
        public abstract Object Decode(Byte[] data, ref Int32 index);

        /// <summary>
        /// 去掉数据长度信息，返回对应数据的二进制数组，并进行相应的索引偏移。
        /// </summary>
        /// <param name="srcData">源数据</param>
        /// <param name="index">索引引用</param>
        /// <returns>对应数据的二进制数组</returns>
        protected Byte[] CutLengthHead(Byte[] srcData, ref Int32 index)
        {
            Int32 length = (Int32)(UInt16)VUInt16.Instance.Decode(srcData, ref index);
            Byte[] result = new Byte[length];
            Buffer.BlockCopy(srcData, index, result, 0, length);
            ////Array.Reverse(result);
            index += length;

            return result;
        }

        #endregion
    }
}