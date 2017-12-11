#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：Utils
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：工具类。
//----------------------------------------------------------------*/
#endregion
using Mogo.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MsgPack;

namespace Mogo.RPC
{

    /// <summary>
    /// 工具类。
    /// </summary>
    public class Utils
    {
        private Utils()
        {
        }

        #region MsgPack Lua table 转换
        public static bool MsgPackLuaTable(ref LuaTable luatable, ref Packer pker)
        {

            if (luatable == null)
            {
                pker = null;
                return false;
            }
            else
            {
                pker.PackMapHeader(luatable.Count);
            }
            foreach (var item in luatable)
            {
                //object key = item.Key;
                bool kFlag = luatable.IsKeyString(item.Key);
                if (kFlag)
                {
                    pker.PackRawHeader(item.Key.Length);
                    pker.PackRawBody(System.Text.Encoding.UTF8.GetBytes(item.Key));
                }
                else
                {
                    pker.Pack(double.Parse(item.Key.ToString()));
                }

                var valueType = item.Value.GetType();
                if (valueType == typeof(String))
                {

                    pker.PackRawHeader(item.Value.ToString().Length);
                    pker.PackRawBody(System.Text.Encoding.UTF8.GetBytes(item.Value.ToString()));
                }
                else if (valueType == typeof(LuaTable))
                {
                    LuaTable luaTbl = item.Value as LuaTable;
                    MsgPackLuaTable(ref luaTbl, ref pker);
                }
                else if (valueType == typeof(bool))
                {
                    pker.Pack(bool.Parse(item.Value.ToString()));
                }
                else
                {
                    pker.Pack(double.Parse(item.Value.ToString()));
                }
            }
            return true;
        }
        public static bool MsgUnPackTable(out LuaTable luatable, ref MessagePackObject pObj)
        {
            LuaTable result = new LuaTable();
            luatable = result;
            var mPk = pObj.AsDictionary();
            bool isString = false;
            string key;
            object value;
            foreach (var item in mPk)
            {
                //parse for key
                MessagePackObject mKey = item.Key;
                if (mKey.IsRaw)
                {
                    key = mKey.AsString();
                    isString = true;
                }
                else if (true == mKey.IsTypeOf<double>())
                {
                    key = mKey.AsDouble().ToString();
                }
                else
                {
                    LoggerHelper.Error("key type error");
                    return false;
                }
                //parse for value
                MessagePackObject mValue = item.Value;
                if (mValue.IsRaw)
                {
                    value = mValue.AsString();
                }
                else if (mValue.IsDictionary)
                {
                    LuaTable luatbl;
                    MsgUnPackTable(out luatbl, ref mValue);
                    value = luatbl;
                }
                else if (true == mValue.IsTypeOf<bool>())
                {
                    value = mValue.AsBoolean();
                }
                else if (true == mValue.IsTypeOf<double>())
                {
                    value = mValue.AsDouble();
                }
                else
                {
                    LoggerHelper.Error("value type error");
                    return false;
                }
                result.Add(key, isString, value);
                isString = false;
            }
            return true;
        }
        #endregion

        #region Lua table转换

        /// <summary>
        /// 转换复杂类型的对象到LuaTable，不支持基础类型直接转换。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool PackLuaTable(object target, out LuaTable result)
        {
            var type = target.GetType();
            if (type == typeof(LuaTable))
            {
                result = target as LuaTable;
                return true;
            }
            if (type.IsGenericType)
            {//容器类型
                //目前只支持列表与字典的容器类型转换
                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    return PackLuaTable(target as IDictionary, out result);
                }
                else
                {
                    return PackLuaTable(target as IList, out result);
                }
            }
            else
            {//实体类型
                result = new LuaTable();
                try
                {
                    var props = type.GetProperties(~BindingFlags.Static);
                    for (int i = 0; i < props.Length; i++)
                    {
                        var prop = props[i];
                        if (IsBaseType(prop.PropertyType))
                            result.Add(i + 1, prop.GetGetMethod().Invoke(target, null));
                        else
                        {
                            LuaTable lt;
                            var value = prop.GetGetMethod().Invoke(target, null);
                            var flag = PackLuaTable(value, out lt);
                            if (flag)
                                result.Add(i + 1, lt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.Error("PackLuaTable entity error: " + ex.Message);
                }
            }
            return true;
        }

        /// <summary>
        /// 转换列表类型的对象到LuaTable。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool PackLuaTable(IList target, out LuaTable result)
        {
            Type[] types = target.GetType().GetGenericArguments();
            result = new LuaTable();
            try
            {
                for (int i = 0; i < target.Count; i++)
                {
                    if (IsBaseType(types[0]))
                    {
                        if (types[0] == typeof(bool))
                            result.Add(i + 1, (bool)target[i] ? 1 : 0);
                        else
                            result.Add(i + 1, target[i]);
                    }
                    else
                    {
                        LuaTable value;
                        var flag = PackLuaTable(target[i], out value);
                        if (flag)
                            result.Add(i + 1, value);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("PackLuaTable list error: " + ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 转换字典类型的对象到LuaTable。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool PackLuaTable(IDictionary target, out LuaTable result)
        {
            Type[] types = target.GetType().GetGenericArguments();
            result = new LuaTable();
            try
            {
                foreach (DictionaryEntry item in target)
                {
                    if (IsBaseType(types[1]))
                    {
                        object value;
                        if (types[1] == typeof(bool))//判断值是否布尔类型，是则做特殊转换
                            value = (bool)item.Value ? 1 : 0;
                        else
                            value = item.Value;

                        if (types[0] == typeof(int))//判断键是否为整型，是则标记键为整型，转lua table字符串时有用
                            result.Add(item.Key.ToString(), false, value);
                        else
                            result.Add(item.Key.ToString(), value);
                    }
                    else
                    {
                        LuaTable value;
                        var flag = PackLuaTable(item.Value, out value);
                        if (flag)
                        {
                            if (types[0] == typeof(int))
                                result.Add(item.Key.ToString(), false, value);
                            else
                                result.Add(item.Key.ToString(), value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("PackLuaTable dictionary error: " + ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 判断类型是否为基础类型。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsBaseType(Type type)
        {
            if (type == typeof(byte) || type == typeof(sbyte)
                || type == typeof(short) || type == typeof(ushort)
                || type == typeof(int) || type == typeof(uint)
                || type == typeof(Int64) || type == typeof(UInt64)
                || type == typeof(float) || type == typeof(double)
                || type == typeof(string) || type == typeof(bool))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 将 Lua table 打包成字符串
        /// </summary>
        /// <param name="luaTable"></param>
        /// <returns></returns>
        public static String PackLuaTable(LuaTable luaTable)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            if (luaTable != null)
            {
                foreach (var item in luaTable)
                {
                    //拼键
                    if (luaTable.IsKeyString(item.Key))
                        sb.Append(EncodeString(item.Key));
                    else
                        sb.Append(item.Key);
                    sb.Append('=');

                    //拼值
                    var valueType = item.Value.GetType();
                    if (valueType == typeof(String))
                        sb.Append(EncodeString(item.Value as String));
                    else if (valueType == typeof(LuaTable))
                        sb.Append(PackLuaTable(item.Value as LuaTable));
                    else
                        sb.Append(item.Value.ToString());
                    sb.Append(',');
                }
                if (luaTable.Count != 0)//若lua table为空则不删除
                    sb.Remove(sb.Length - 1, 1);//去掉最后一个逗号
            }
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        /// 将字符串转成 Lua table 可识别的格式。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static String EncodeString(String value)
        {
            if (value.Length > 999)
                LoggerHelper.Warning("PackLuaTable EncodeString overflow: " + value);
            return String.Concat('s', Encoding.UTF8.GetBytes(value).Length.ToString("000"), value);
        }

        /// <summary>
        /// 将 Lua table 字符串转换为 Lua table 实体
        /// </summary>
        /// <param name="inputString">Lua table 字符串</param>
        /// <param name="result">实体对象</param>
        /// <returns>返回 true/false 表示是否成功</returns>
        public static bool ParseLuaTable(string inputString, out LuaTable result)
        {
            string trimString = inputString.Trim();
            if (trimString[0] != '{' || trimString[trimString.Length - 1] != '}')
            {
                result = null;
                return false;
            }
            else if (trimString.Length == 2)
            {
                result = new LuaTable();
                return true;
            }

            var index = 0;
            object obj;
            var flag = DecodeLuaTable(inputString, ref index, out obj);
            if (flag)
                result = obj as LuaTable;
            else
                result = null;
            return flag;
        }

        /// <summary>
        /// 将 Byte流 转换为 Lua table 实体
        /// </summary>
        /// <param name="inputString">Byte[]</param>
        /// <param name="result">实体对象</param>
        /// <returns>返回 true/false 表示是否成功</returns>
        public static bool ParseLuaTable(byte[] inputString, out LuaTable result)
        {
            //inputString = Encoding.UTF8.GetBytes(str.Trim());
            //string trimString = inputString.Trim();
            if (inputString[0] != '{' || inputString[inputString.Length - 1] != '}')
            {
                result = null;
                return false;
            }
            else if (inputString.Length == 2)
            {
                result = new LuaTable();
                return true;
            }

            var index = 0;
            object obj;
            var flag = DecodeLuaTable(inputString, ref index, out obj);
            if (flag)
                result = obj as LuaTable;
            else                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
                result = null;
            return flag;
        }
        private static bool DecodeLuaTable(string inputString, ref int index, out object result)
        {
            var luaTable = new LuaTable();
            result = luaTable;
            if (!WaitChar(inputString, '{', ref index))
            {
                return false;
            }
            try
            {
                if (WaitChar(inputString, '}', ref index))//如果下一个字符为右大括号表示为空Lua table
                    return true;
                while (index < inputString.Length)
                {
                    string key;
                    bool isString;
                    object value;
                    DecodeKey(inputString, ref index, out key, out isString);//匹配键
                    WaitChar(inputString, '=', ref index);//匹配键值对分隔符
                    var flag = DecodeLuaValue(inputString, ref index, out value);//转换实体
                    if (flag)
                    {
                        luaTable.Add(key, isString, value);
                    }
                    if (!WaitChar(inputString, ',', ref index))
                        break;
                }
                WaitChar(inputString, '}', ref index);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("Parse LuaTable error: " + inputString + e.ToString());
                return false;
            }
        }
        private static bool DecodeLuaTable(byte[] inputString, ref int index, out object result)
        {
            var luaTable = new LuaTable();
            result = luaTable;
            if (!WaitChar(inputString, '{', ref index))
            {
                return false;
            }
            try
            {
                if (WaitChar(inputString, '}', ref index))//如果下一个字符为右大括号表示为空Lua table
                    return true;
                while (index < inputString.Length)
                {
                    string key;
                    bool isString;
                    object value;
                    DecodeKey(inputString, ref index, out key, out isString);//匹配键
                    WaitChar(inputString, '=', ref index);//匹配键值对分隔符
                    var flag = DecodeLuaValue(inputString, ref index, out value);//转换实体
                    if (flag)
                    {
                        luaTable.Add(key, isString, value);
                    }
                    if (!WaitChar(inputString, ',', ref index))
                        break;
                }
                WaitChar(inputString, '}', ref index);
                return true;
                
            }
            catch (Exception e)
            {
                LoggerHelper.Error("Parse LuaTable error: " + inputString + e.ToString());
                return false;
            }
        }
        private static bool DecodeLuaValue(string inputString, ref int index, out object value)
        {
            var firstChar = inputString[index];
            if (firstChar == 's')
            {
                //value is string
                var szLen = inputString.Substring(index + 1, 3);
                var lenth = Int32.Parse(szLen);
                index += 4;
                if (lenth > 0)
                {
                    value = inputString.Substring(index, lenth);
                    index += lenth;
                    return true;
                }
                else
                {
                    value = "";
                    return true;
                }
                //LoggerHelper.Error("Decode Lua Table Value Error: " + index + " " + inputString);
                //return false;
            }
            else if (firstChar == '{')//如果第一个字符为花括号，表示接下来的内容为列表或实体类型
            {
                return DecodeLuaTable(inputString, ref index, out value);
            }
            else
            {
                //value is number
                var i = index;
                while (++index < inputString.Length)
                {
                    if (inputString[index] == ',' || inputString[index] == '}')
                    {
                        if (index > i)
                        {
                            value = inputString.Substring(i, index - i);
                            return true;
                        }
                    }
                }
                LoggerHelper.Error("Decode Lua Table Value Error: " + index + " " + inputString);
                value = null;
                return false;
            }
        }

        public static bool DecodeKey(byte[] inputString, ref int index, out string key, out bool isString)
        {
            if (inputString[index] == 's')
            {
                //key is string
                var szLen = Encoding.UTF8.GetString(inputString, index + 1, 3);
                var length = Int32.Parse(szLen);
                if (length > 0)
                {
                    index += 4;
                    key = Encoding.UTF8.GetString(inputString, index, length);
                    isString = true;
                    index += length;
                    return true;
                }
                key = "";
                isString = true;
                LoggerHelper.Error("Decode Lua Table Key Error: " + index + " " + inputString);
                return false;
            }
            else
            {
                //key is number
                int offset = 0;
                while (index + offset < inputString.Length && inputString[index + offset] != '=')
                {
                    offset++;
                }

                if (offset > 0)
                {
                    key = Encoding.UTF8.GetString(inputString, index, offset);
                    //value = Int32.Parse(strValue);
                    index = index + offset;
                    isString = false;
                    return true;
                }
                else
                {
                    key = "-1";
                    isString = false;
                    LoggerHelper.Error("Decode Lua Table Key Error: " + index + " " + inputString);
                    return false;
                }
            }
        }
        private static bool DecodeLuaValue(byte[] inputString, ref int index, out object value)
        {
            var firstChar = inputString[index];
            if (firstChar == 's')
            {
                //value is string
                var szLen = Encoding.UTF8.GetString(inputString, index + 1, 3);
                var length = Int32.Parse(szLen);
                index += 4;
                if (length > 0)
                {
                    value = Encoding.UTF8.GetString(inputString, index, length);
                    index += length;
                    return true;
                }
                else
                {
                    value = "";
                    return true;
                }
                //LoggerHelper.Error("Decode Lua Table Value Error: " + index + " " + inputString);
                //return false;
            }
            else if (firstChar == '{')//如果第一个字符为花括号，表示接下来的内容为列表或实体类型
            {
                return DecodeLuaTable(inputString, ref index, out value);
            }
            else
            {
                //value is number
                var i = index;
                while (++index < inputString.Length)
                {
                    if (inputString[index] == ',' || inputString[index] == '}')
                    {
                        if (index > i)
                        {
                            value = Encoding.UTF8.GetString(inputString, i, index-i);
                            return true;
                        }
                    }
                }
                LoggerHelper.Error("Decode Lua Table Value Error: " + index + " " + inputString);
                value = null;
                return false;
            }
        }
        /// <summary>
        /// 解析 Lua table 的键。
        /// </summary>
        /// <param name="inputString">Lua table字符串</param>
        /// <param name="index">字符串偏移量</param>
        /// <param name="result">键</param>
        /// <returns>返回 true/false 表示是否成功</returns>
        public static bool DecodeKey(string inputString, ref int index, out string key, out bool isString)
        {
            if (inputString[index] == 's')
            {
                //key is string
                var szLen = inputString.Substring(index + 1, 3);
                var lenth = Int32.Parse(szLen);
                if (lenth > 0)
                {
                    index += 4;
                    key = inputString.Substring(index, lenth);
                    isString = true;
                    index += lenth;
                    return true;
                }
                key = "";
                isString = true;
                LoggerHelper.Error("Decode Lua Table Key Error: " + index + " " + inputString);
                return false;
            }
            else
            {
                //key is number
                var szLen = inputString.IndexOf('=', index);
                if (szLen > -1)
                {
                    var lenth = szLen - index;
                    key = inputString.Substring(index, lenth);
                    //value = Int32.Parse(strValue);
                    index = szLen;
                    isString = false;
                    return true;
                }
                else
                {
                    key = "-1";
                    isString = false;
                    LoggerHelper.Error("Decode Lua Table Key Error: " + index + " " + inputString);
                    return false;
                }
            }
        }



        /// <summary>
        /// 判断下个字符是否为期望的字符。
        /// </summary>
        /// <param name="inputString">Lua table字符串</param>
        /// <param name="c">期望的字符</param>
        /// <param name="index">字符串偏移量</param>
        /// <returns>返回 true/false 表示是否为期望的字符</returns>
        public static bool WaitChar(string inputString, char c, ref int index)
        {
            var szLen = inputString.IndexOf(c, index);
            if (szLen == index)
            {
                index++;
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判断下个字符是否为期望的字符。
        /// </summary>
        /// <param name="inputString">Lua table字符串</param>
        /// <param name="c">期望的字符</param>
        /// <param name="index">字符串偏移量</param>
        /// <returns>返回 true/false 表示是否为期望的字符</returns>
        public static bool WaitChar(byte[] inputString, char c, ref int index)
        {
            if ((byte)c == inputString[index])
            {
                index++;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 填充数据长度头。
        /// </summary>
        /// <param name="srcData">源二进制数组</param>
        /// <returns>填充二进制数组长度到头部的数据</returns>
        public static Byte[] FillLengthHead(Byte[] srcData)
        {
            return FillLengthHead(srcData, (UInt16)srcData.Length);
        }

        /// <summary>
        /// 填充数据长度头。
        /// </summary>
        /// <param name="srcData">源二进制数组</param>
        /// <param name="length">源二进制数组数据长度</param>
        /// <returns>填充二进制数组长度到头部的数据</returns>
        public static Byte[] FillLengthHead(Byte[] srcData, UInt16 length)
        {
            Byte[] lengthByteArray = BitConverter.GetBytes(length);//将字符串长度转换为二进制数组
            //Array.Reverse(lengthByteArray);
            Byte[] result = new Byte[length + lengthByteArray.Length];//申请存放字符串长度和字符串内容的空间
            Buffer.BlockCopy(lengthByteArray, 0, result, 0, lengthByteArray.Length);//将长度的二进制数组拷贝到目标空间
            Buffer.BlockCopy(srcData, 0, result, lengthByteArray.Length, length);//将字符串的二进制数组拷贝到目标空间
            return result;
        }
    }
}