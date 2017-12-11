#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VLuaTable
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.29
// 模块描述：Lua Table（LuaTable）。
//----------------------------------------------------------------*/
#endregion
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Mogo.Util;
using System.Linq;
namespace Mogo.RPC
{
    /// <summary>
    /// Lua Table（LuaTable）。
    /// </summary>
    public class VLuaTable : VObject
    {
        private static VLuaTable m_instance = new VLuaTable();
        public static VLuaTable Instance
        {
            get
            {
                return m_instance;
            }
        }

        public VLuaTable()
            : base(typeof(LuaTable), VType.V_LUATABLE, 0)
        {
        }
        public VLuaTable(Object vValue)
            : base(typeof(LuaTable), VType.V_LUATABLE, vValue)
        {
        }

        public override byte[] Encode(Object vValue)
        {
            //LuaTable luaTable;
            //Utils.PackLuaTable(vValue, out luaTable);
            //MemoryStream buffer = new MemoryStream();
            //var pker = Packer.Create(buffer);
            //Utils.MsgPackLuaTable(ref luaTable, ref pker);
            //String str = BitConverter.ToString(buffer.ToArray());
            //Byte[] rs = (from x in str.Split(new char[] { '-' }) select (Convert.ToByte(x, 16))).ToArray();
            //UInt16 length = (UInt16)rs.Length;
            //return Utils.FillLengthHead(rs, length);

            //cPickle implementation
            LuaTable value;
            Utils.PackLuaTable(vValue, out value);
            var s = Utils.PackLuaTable(value);
            return VString.Instance.Encode(s);
        }

        public override Object Decode(byte[] data, ref Int32 index)
        {
            //byte[] strData = CutLengthHead(data, ref index);
            //string str = BitConverter.ToString(strData);
            //MemoryStream buffer = new MemoryStream(strData);
            //var uPker = Unpacker.Create(buffer);
            //MessagePackObject pObj = (MessagePackObject)uPker.ReadItem();
            //LuaTable luaTable;
            //bool ret = Utils.MsgUnPackTable(out luaTable, ref pObj);
            //if (ret) return luaTable;
            //else return null;

            //cPicke implementation
            byte[] strData = CutLengthHead(data, ref index);
            LuaTable luaTable;
            return Utils.ParseLuaTable(strData, out luaTable) ? luaTable : null;

        }
    }
}
