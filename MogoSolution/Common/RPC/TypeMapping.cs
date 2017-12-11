#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TypeMapping
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：类型映射处理类。
//----------------------------------------------------------------*/
#endregion
using System;
using Mogo.Util;

namespace Mogo.RPC
{
    /// <summary>
    /// 类型映射处理类。
    /// </summary>
    public class TypeMapping
    {
        private TypeMapping()
        {
        }

        /// <summary>
        /// 根据类型字符串返回对应的类型实例。
        /// </summary>
        /// <param name="type">类型字符串</param>
        /// <returns>类型实例</returns>
        public static VObject GetVObject(String type)
        {
            switch (type)
            {
                case ("STRING"):
                    return VString.Instance;
                case ("INT8"):
                    return VInt8.Instance;
                case ("UINT8"):
                    return VUInt8.Instance;
                case ("INT16"):
                    return VInt16.Instance;
                case ("UINT16"):
                    return VUInt16.Instance;
                case ("INT32"):
                    return VInt32.Instance;
                case ("UINT32"):
                    return VUInt32.Instance;
                case ("INT64"):
                    return VInt64.Instance;
                case ("UINT64"):
                    return VUInt64.Instance;
                case ("FLOAT"):
                    return VFloat.Instance;
                case ("FLOAT64"):
                    return VDouble.Instance;
                case ("BOOL"):
                    return VBoolean.Instance;
                case ("BLOB"):
                    return VBLOB.Instance;
                case ("LUA_TABLE"):
                    return VLuaTable.Instance;
                default:
                    {
                        LoggerHelper.Warning(String.Format("Can not find type: {0}.", type));
                        return VEmpty.Instance;
                    }
            }
        }
    }
}