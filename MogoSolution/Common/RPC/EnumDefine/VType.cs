#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：VType
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：类型标识。
//----------------------------------------------------------------*/
#endregion

namespace Mogo.RPC
{
    /// <summary>
    /// 类型标识。
    /// </summary>
    public enum VType
    {
        V_TYPE_ERR = -1,

        V_LUATABLE = 1,
        V_STR = 2,
        V_INT8 = 3,
        V_UINT8 = 4,
        V_INT16 = 5,
        V_UINT16 = 6,
        V_INT32 = 7,
        V_UINT32 = 8,
        V_INT64 = 9,
        V_UINT64 = 10,
        V_FLOAT32 = 11,
        V_FLOAT64 = 12,
        V_ENTITYMB = 13,
        V_ENTITY = 14,
        V_CLSMETHOD = 15,
        V_BLOB = 16,

        //V_LIST = 4,
        //V_MAP = 5,

        V_REDIS_HASH = 22,      //redis hash类型数据
        V_LUA_OBJECT = 23,      //任意lua对象,用在entity_index和newindex,其他地方不支持
        V_ENTITY_POINTER = 24,		//仅用在client rpc处
        V_MAX_VTYPE = 25,
    };
}