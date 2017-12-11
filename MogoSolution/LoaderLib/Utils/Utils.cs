#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：Utils
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.2.1
// 模块描述：通用工具类。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using System.Security.Cryptography;
using System.Net;
using System.Runtime.InteropServices;

namespace Mogo.Util
{
    public enum LayerMask
    {
        Default = 1,
        Character = 1 << 8,
        Monster = 1 << 11,
        Npc = 1 << 12,
        Terrain = 1 << 9,
        Trap = 1 << 17
    }

    /// <summary>
    /// 通用工具类。
    /// </summary>
    public static class Utils
    {
        #region 常量

        /// <summary>
        /// 键值分隔符： ‘:’
        /// </summary>
        private const Char KEY_VALUE_SPRITER = ':';
        /// <summary>
        /// 字典项分隔符： ‘,’
        /// </summary>
        private const Char MAP_SPRITER = ',';
        /// <summary>
        /// 数组分隔符： ','
        /// </summary>
        private const Char LIST_SPRITER = ',';

        public const String RPC_HEAD = "RPC_";

        public const String SVR_RPC_HEAD = "SVR_RPC_";

        #endregion

        #region 时间格式化

        /// <summary>
        /// 格式化日期格式。（yyyy-MM-dd HH:mm:ss）
        /// </summary>
        /// <param name="datetime">日期对象</param>
        /// <returns>日期字符串</returns>
        public static String FormatTime(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 格式化日期格式。（yyyy-MM-dd HH:mm:ss）
        /// </summary>
        /// <param name="datetime">日期值</param>
        /// <returns>日期字符串</returns>
        public static String FormatTime(this long datetime)
        {
            DateTime.FromBinary(datetime);
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 时间戳转为C#格式时间。
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns></returns>
        public static DateTime GetTime(this int timeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return startTime.AddSeconds(timeStamp);
        }

        #endregion

        #region 字典转换

        /// <summary>
        /// 将字典字符串转换为键类型与值类型都为整型的字典对象。
        /// </summary>
        /// <param name="strMap">字典字符串</param>
        /// <param name="keyValueSpriter">键值分隔符</param>
        /// <param name="mapSpriter">字典项分隔符</param>
        /// <returns>字典对象</returns>
        public static Dictionary<Int32, Int32> ParseMapIntInt(this String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<Int32, Int32> result = new Dictionary<Int32, Int32>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                int key;
                int value;
                if (int.TryParse(item.Key, out key) && int.TryParse(item.Value, out value))
                    result.Add(key, value);
                else
                    LoggerHelper.Warning(String.Format("Parse failure: {0}, {1}", item.Key, item.Value));
            }
            return result;
        }

        /// <summary>
        /// 将字典字符串转换为键类型为整型，值类型为单精度浮点数的字典对象。
        /// </summary>
        /// <param name="strMap">字典字符串</param>
        /// <param name="keyValueSpriter">键值分隔符</param>
        /// <param name="mapSpriter">字典项分隔符</param>
        /// <returns>字典对象</returns>
        public static Dictionary<Int32, float> ParseMapIntFloat(this String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            var result = new Dictionary<Int32, float>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                int key;
                float value;
                if (int.TryParse(item.Key, out key) && float.TryParse(item.Value, out value))
                    result.Add(key, value);
                else
                    LoggerHelper.Warning(String.Format("Parse failure: {0}, {1}", item.Key, item.Value));
            }
            return result;
        }

        /// <summary>
        /// 将字典字符串转换为键类型为整型，值类型为字符串的字典对象。
        /// </summary>
        /// <param name="strMap">字典字符串</param>
        /// <param name="keyValueSpriter">键值分隔符</param>
        /// <param name="mapSpriter">字典项分隔符</param>
        /// <returns>字典对象</returns>
        public static Dictionary<Int32, String> ParseMapIntString(this String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<Int32, String> result = new Dictionary<Int32, String>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                int key;
                if (int.TryParse(item.Key, out key))
                    result.Add(key, item.Value);
                else
                    LoggerHelper.Warning(String.Format("Parse failure: {0}", item.Key));
            }
            return result;
        }

        /// <summary>
        /// 将字典字符串转换为键类型为字符串，值类型为单精度浮点数的字典对象。
        /// </summary>
        /// <param name="strMap">字典字符串</param>
        /// <param name="keyValueSpriter">键值分隔符</param>
        /// <param name="mapSpriter">字典项分隔符</param>
        /// <returns>字典对象</returns>
        public static Dictionary<String, float> ParseMapStringFloat(this String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<String, float> result = new Dictionary<String, float>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                float value;
                if (float.TryParse(item.Value, out value))
                    result.Add(item.Key, value);
                else
                    LoggerHelper.Warning(String.Format("Parse failure: {0}", item.Value));
            }
            return result;
        }

        /// <summary>
        /// 将字典字符串转换为键类型为字符串，值类型为整型的字典对象。
        /// </summary>
        /// <param name="strMap">字典字符串</param>
        /// <param name="keyValueSpriter">键值分隔符</param>
        /// <param name="mapSpriter">字典项分隔符</param>
        /// <returns>字典对象</returns>
        public static Dictionary<String, Int32> ParseMapStringInt(this String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<String, Int32> result = new Dictionary<String, Int32>();
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                int value;
                if (int.TryParse(item.Value, out value))
                    result.Add(item.Key, value);
                else
                    LoggerHelper.Warning(String.Format("Parse failure: {0}", item.Value));
            }
            return result;
        }

        /// <summary>
        /// 将字典字符串转换为键类型为 T，值类型为 U 的字典对象。
        /// </summary>
        /// <typeparam name="T">字典Key类型</typeparam>
        /// <typeparam name="U">字典Value类型</typeparam>
        /// <param name="strMap">字典字符串</param>
        /// <param name="keyValueSpriter">键值分隔符</param>
        /// <param name="mapSpriter">字典项分隔符</param>
        /// <returns>字典对象</returns>
        public static Dictionary<T, U> ParseMapAny<T, U>(this String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            var typeT = typeof(T);
            var typeU = typeof(U);
            var result = new Dictionary<T, U>();
            //先转为字典
            var strResult = ParseMap(strMap, keyValueSpriter, mapSpriter);
            foreach (var item in strResult)
            {
                try
                {
                    T key = (T)GetValue(item.Key, typeT);
                    U value = (U)GetValue(item.Value, typeU);

                    result.Add(key, value);
                }
                catch (Exception)
                {
                    LoggerHelper.Warning(String.Format("Parse failure: {0}, {1}", item.Key, item.Value));
                }
            }
            return result;
        }

        /// <summary>
        /// 将字典字符串转换为键类型与值类型都为字符串的字典对象。
        /// </summary>
        /// <param name="strMap">字典字符串</param>
        /// <param name="keyValueSpriter">键值分隔符</param>
        /// <param name="mapSpriter">字典项分隔符</param>
        /// <returns>字典对象</returns>
        public static Dictionary<String, String> ParseMap(this String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();
            if (String.IsNullOrEmpty(strMap))
            {
                return result;
            }

            var map = strMap.Split(mapSpriter);//根据字典项分隔符分割字符串，获取键值对字符串
            for (int i = 0; i < map.Length; i++)
            {
                if (String.IsNullOrEmpty(map[i]))
                {
                    continue;
                }

                var keyValuePair = map[i].Split(keyValueSpriter);//根据键值分隔符分割键值对字符串
                if (keyValuePair.Length == 2)
                {
                    if (!result.ContainsKey(keyValuePair[0]))
                        result.Add(keyValuePair[0], keyValuePair[1]);
                    else
                        LoggerHelper.Warning(String.Format("Key {0} already exist, index {1} of {2}.", keyValuePair[0], i, strMap));
                }
                else
                {
                    LoggerHelper.Warning(String.Format("KeyValuePair are not match: {0}, index {1} of {2}.", map[i], i, strMap));
                }
            }
            return result;
        }

        /// <summary>
        /// 将字典对象转换为字典字符串。
        /// </summary>
        /// <typeparam name="T">字典Key类型</typeparam>
        /// <typeparam name="U">字典Value类型</typeparam>
        /// <param name="map">字典对象</param>
        /// <returns>字典字符串</returns>
        public static String PackMap<T, U>(this IEnumerable<KeyValuePair<T, U>> map, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
        {
            if (map.Count() == 0)
                return "";
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in map)
                {
                    sb.AppendFormat("{0}{1}{2}{3}", item.Key, keyValueSpriter, item.Value, mapSpriter);
                }
                return sb.ToString().Remove(sb.Length - 1, 1);
            }
        }

        #endregion

        #region 列表转换

        /// <summary>
        /// 将列表字符串转换为类型为 T 的列表对象。
        /// </summary>
        /// <typeparam name="T">列表值对象类型</typeparam>
        /// <param name="strList">列表字符串</param>
        /// <param name="listSpriter">数组分隔符</param>
        /// <returns>列表对象</returns>
        public static List<T> ParseListAny<T>(this String strList, Char listSpriter = LIST_SPRITER)
        {
            var type = typeof(T);
            var list = strList.ParseList(listSpriter);
            var result = new List<T>();
            foreach (var item in list)
            {
                result.Add((T)GetValue(item, type));
            }
            return result;
        }

        /// <summary>
        /// 将列表字符串转换为字符串的列表对象。
        /// </summary>
        /// <param name="strList">列表字符串</param>
        /// <param name="listSpriter">数组分隔符</param>
        /// <returns>列表对象</returns>
        public static List<String> ParseList(this String strList, Char listSpriter = LIST_SPRITER)
        {
            var result = new List<String>();
            if (String.IsNullOrEmpty(strList))
                return result;

            var trimString = strList.Trim();
            if (String.IsNullOrEmpty(strList))
            {
                return result;
            }
            var detials = trimString.Split(listSpriter);//.Substring(1, trimString.Length - 2)
            foreach (var item in detials)
            {
                if (!String.IsNullOrEmpty(item))
                    result.Add(item.Trim());
            }

            return result;
        }

        /// <summary>
        /// 将列表对象转换为列表字符串。
        /// </summary>
        /// <typeparam name="T">列表值对象类型</typeparam>
        /// <param name="list">列表对象</param>
        /// <param name="listSpriter">列表分隔符</param>
        /// <returns>列表字符串</returns>
        public static String PackList<T>(this List<T> list, Char listSpriter = LIST_SPRITER)
        {
            if (list.Count == 0)
                return "";
            else
            {
                StringBuilder sb = new StringBuilder();
                //sb.Append("[");
                foreach (var item in list)
                {
                    sb.AppendFormat("{0}{1}", item, listSpriter);
                }
                sb.Remove(sb.Length - 1, 1);
                //sb.Append("]");

                return sb.ToString();
            }

        }

        public static String PackArray<T>(this T[] array, Char listSpriter = LIST_SPRITER)
        {
            var list = new List<T>();
            list.AddRange(array);
            return PackList(list, listSpriter);
        }

        #endregion

        #region 随机数

        /// <summary>
        /// 创建一个产生不重复随机数的随机数生成器。
        /// </summary>
        /// <returns>随机数生成器</returns>
        public static System.Random CreateRandom()
        {
            long tick = DateTime.Now.Ticks;
            return new System.Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
        }

        public static T Choice<T>(List<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }

            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }

        #endregion

        #region 文件读取

        public static String LoadResource(String fileName)
        {
            var text = Resources.Load(fileName);
            if (text != null)
            {
                var result = text.ToString();
                Resources.UnloadAsset(text);
                return result;
            }
            else
                return String.Empty;
        }

        public static String LoadFile(String fileName)
        {
            //LoggerHelper.Debug(fileName);
            if (File.Exists(fileName))
                using (StreamReader sr = File.OpenText(fileName))
                    return sr.ReadToEnd();
            else
                return String.Empty;
        }

        public static byte[] LoadByteResource(String fileName)
        {
            TextAsset binAsset = Resources.Load(fileName, typeof(TextAsset)) as TextAsset;
            var result = binAsset.bytes;
            Resources.UnloadAsset(binAsset);

            return result;
        }

        public static byte[] LoadByteFile(String fileName)
        {
            if (File.Exists(fileName))
                return File.ReadAllBytes(fileName);
            else
                return null;
        }

        public static string GetStreamPath(string fileName)
        {
            var Path = String.Concat(Application.streamingAssetsPath, "/", fileName);
            if (Application.platform != RuntimePlatform.Android)
                Path = String.Concat(SystemConfig.ASSET_FILE_HEAD, Path);
            return Path;
        }
        #endregion

        #region 类型转换

        /// <summary>
        /// 将字符串转换为对应类型的值。
        /// </summary>
        /// <param name="value">字符串值内容</param>
        /// <param name="type">值的类型</param>
        /// <returns>对应类型的值</returns>
        public static object GetValue(String value, Type type)
        {
            if (type == null)
                return null;
            else if (type == typeof(string))
                return value;
            else if (type == typeof(Int32))
                return Convert.ToInt32(Convert.ToDouble(value));
            else if (type == typeof(float))
                return float.Parse(value);
            else if (type == typeof(byte))
                return Convert.ToByte(Convert.ToDouble(value));
            else if (type == typeof(sbyte))
                return Convert.ToSByte(Convert.ToDouble(value));
            else if (type == typeof(UInt32))
                return Convert.ToUInt32(Convert.ToDouble(value));
            else if (type == typeof(Int16))
                return Convert.ToInt16(Convert.ToDouble(value));
            else if (type == typeof(Int64))
                return Convert.ToInt64(Convert.ToDouble(value));
            else if (type == typeof(UInt16))
                return Convert.ToUInt16(Convert.ToDouble(value));
            else if (type == typeof(UInt64))
                return Convert.ToUInt64(Convert.ToDouble(value));
            else if (type == typeof(double))
                return double.Parse(value);
            else if (type == typeof(bool))
            {
                if (value == "0")
                    return false;
                else if (value == "1")
                    return true;
                else
                    return bool.Parse(value);
            }
            else if (type.BaseType == typeof(Enum))
                return GetValue(value, Enum.GetUnderlyingType(type));
            else if (type == typeof(Vector3))
            {
                Vector3 result;
                ParseVector3(value, out result);
                return result;
            }
            else if (type == typeof(Quaternion))
            {
                Quaternion result;
                ParseQuaternion(value, out result);
                return result;
            }
            else if (type == typeof(Color))
            {
                Color result;
                ParseColor(value, out result);
                return result;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type[] types = type.GetGenericArguments();
                var map = ParseMap(value);
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var item in map)
                {
                    var key = GetValue(item.Key, types[0]);
                    var v = GetValue(item.Value, types[1]);
                    type.GetMethod("Add").Invoke(result, new object[] { key, v });
                }
                return result;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type t = type.GetGenericArguments()[0];
                var list = ParseList(value);
                var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (var item in list)
                {
                    var v = GetValue(item, t);
                    type.GetMethod("Add").Invoke(result, new object[] { v });
                }
                return result;
            }
            else
                return null;
        }

        /// <summary>
        /// 将指定格式(255, 255, 255, 255) 转换为 Color 
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <returns>返回 true/false 表示是否成功</returns>
        public static bool ParseColor(string _inputString, out Color result)
        {
            string trimString = _inputString.Trim();
            result = Color.clear;
            if (trimString.Length < 9)
            {
                return false;
            }
            //if (trimString[0] != '(' || trimString[trimString.Length - 1] != ')')
            //{
            //    return false;
            //}
            try
            {
                string[] _detail = trimString.Split(LIST_SPRITER);//.Substring(1, trimString.Length - 2)
                if (_detail.Length != 4)
                {
                    return false;
                }
                result = new Color(float.Parse(_detail[0]) / 255, float.Parse(_detail[1]) / 255, float.Parse(_detail[2]) / 255, float.Parse(_detail[3]) / 255);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("Parse Color error: " + trimString + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 将指定格式(1.0, 2, 3.4) 转换为 Vector3 
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <returns>返回 true/false 表示是否成功</returns>
        public static bool ParseVector3(string _inputString, out Vector3 result)
        {
            string trimString = _inputString.Trim();
            result = new Vector3();
            if (trimString.Length < 7)
            {
                return false;
            }
            //if (trimString[0] != '(' || trimString[trimString.Length - 1] != ')')
            //{
            //    return false;
            //}
            try
            {
                string[] _detail = trimString.Split(LIST_SPRITER);//.Substring(1, trimString.Length - 2)
                if (_detail.Length != 3)
                {
                    return false;
                }
                result.x = float.Parse(_detail[0]);
                result.y = float.Parse(_detail[1]);
                result.z = float.Parse(_detail[2]);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("Parse Vector3 error: " + trimString + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 将指定格式(1.0, 2, 3.4) 转换为 Vector3 
        /// </summary>
        /// <param name="_inputString"></param>
        /// <param name="result"></param>
        /// <returns>返回 true/false 表示是否成功</returns>
        public static bool ParseQuaternion(string _inputString, out Quaternion result)
        {
            string trimString = _inputString.Trim();
            result = new Quaternion();
            if (trimString.Length < 9)
            {
                return false;
            }
            //if (trimString[0] != '(' || trimString[trimString.Length - 1] != ')')
            //{
            //    return false;
            //}
            try
            {
                string[] _detail = trimString.Split(LIST_SPRITER);//.Substring(1, trimString.Length - 2)
                if (_detail.Length != 4)
                {
                    return false;
                }
                result.x = float.Parse(_detail[0]);
                result.y = float.Parse(_detail[1]);
                result.z = float.Parse(_detail[2]);
                result.w = float.Parse(_detail[3]);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("Parse Quaternion error: " + trimString + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 替换字符串中的子字符串。
        /// </summary>
        /// <param name="input">原字符串</param>
        /// <param name="oldValue">旧子字符串</param>
        /// <param name="newValue">新子字符串</param>
        /// <param name="count">替换数量</param>
        /// <param name="startAt">从第几个字符开始</param>
        /// <returns>替换后的字符串</returns>
        public static String ReplaceFirst(this string input, string oldValue, string newValue, int startAt = 0)
        {
            int pos = input.IndexOf(oldValue, startAt);
            if (pos < 0)
            {
                return input;
            }
            return string.Concat(input.Substring(0, pos), newValue, input.Substring(pos + oldValue.Length));
        }

        #endregion

        #region Lua table转换

        #region 实体转换

        /// <summary>
        /// 将 Lua table 实体转换为 实体
        /// </summary>
        /// <typeparam name="T">Lua table数据对应实体类型</typeparam>
        /// <param name="luaTable">Lua table 实体</param>
        /// <param name="result">实体对象</param>
        /// <returns>返回 true/false 表示是否成功</returns>
        public static bool ParseLuaTable<T>(LuaTable luaTable, out T result)
        {
            object obj;
            if (ParseLuaTable(luaTable, typeof(T), out obj))
            {
                result = (T)obj;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// 将 Lua table 实体转换为 实体
        /// </summary>
        /// <param name="luaTable">Lua table 实体</param>
        /// <param name="type">Lua table数据对应实体类型</param>
        /// <param name="result">实体对象</param>
        /// <returns>返回 true/false 表示是否成功</returns>
        public static bool ParseLuaTable(LuaTable luaTable, Type type, out object result)
        {
            result = null;

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    DecodeDic(luaTable, type, out result);
                }
                else if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    DecodeList(luaTable, type, out result);
                }
            }
            else
            {
                DecodeEntity(luaTable, type, out result);
            }
            return true;
        }

        private static bool DecodeList(LuaTable luaTable, Type type, out object result)
        {
            result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
            Type valueType = type.GetGenericArguments()[0];
            foreach (var item in luaTable)
            {
                object obj = null;
                if (luaTable.IsLuaTable(item.Key))
                    ParseLuaTable(item.Value as LuaTable, valueType, out obj);
                else
                    obj = GetValue(item.Value.ToString(), valueType);
                type.GetMethod("Add").Invoke(result, new object[] { obj });
            }
            return true;
        }

        private static bool DecodeDic(LuaTable luaTable, Type type, out object result)
        {
            result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
            Type valueType = type.GetGenericArguments()[1];
            foreach (var item in luaTable)
            {
                object obj = null;
                if (luaTable.IsLuaTable(item.Key))
                    ParseLuaTable(item.Value as LuaTable, valueType, out obj);
                else
                    obj = GetValue(item.Value.ToString(), valueType);
                type.GetMethod("Add").Invoke(result, new object[] { luaTable.IsKeyString(item.Key) ? item.Key : (object)Int32.Parse(item.Key), obj });
            }
            return true;
        }

        private static bool DecodeEntity(LuaTable luaTable, Type type, out object result)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                LoggerHelper.Error(String.Format("type {0} can not create an entity. luatable: {1}", type.Name, luaTable));
                result = null;
                return false;
            }
            result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
            var props = type.GetProperties();//找出实体所有属性
            foreach (var item in luaTable)
            {
                //var s = item;
                //var x = s.Key + " " + s.Value.ToString();
                //var y = Int32.Parse(s.Value.ToString());
                PropertyInfo valueType = null;
                if (luaTable.IsKeyString(item.Key))
                {
                    valueType = type.GetProperty(item.Key);
                }
                else
                {
                    var i = int.Parse(item.Key) - 1;//Lua Table 的键从1开始
                    if (i < props.Length)
                        valueType = props[i];
                }
                if (valueType != null)
                {
                    object obj = null;
                    if (luaTable.IsLuaTable(item.Key))
                        ParseLuaTable(item.Value as LuaTable, valueType.PropertyType, out obj);
                    else
                        obj = GetValue(item.Value.ToString(), valueType.PropertyType);
                    valueType.SetValue(result, obj, null);
                }
            }
            return true;
        }

        /// <summary>
        /// 将 Lua table 转换为 实体
        /// </summary>
        /// <param name="inputString">Lua table字符串</param>
        /// <param name="type">Lua table数据对应实体类型</param>
        /// <param name="result">实体对象</param>
        /// <returns>返回 true/false 表示是否成功</returns>
        public static bool ParseLuaTable(string inputString, Type type, out object result)
        {
            string trimString = inputString.Trim();
            if (trimString[0] != '{' || trimString[trimString.Length - 1] != '}')
            {
                result = null;
                return false;
            }
            else if (trimString.Length == 2)
            {
                result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                return true;
            }
            int index = 0;
            return DecodeDic(trimString, type, ref index, out result);
        }

        /// <summary>
        /// 解析列表类型 Lua table 值。
        /// </summary>
        /// <param name="inputString">Lua table字符串</param>
        /// <param name="type">Lua table数据对应实体类型</param>
        /// <param name="index">字符串偏移量</param>
        /// <param name="result">列表对象</param>
        /// <returns>返回 true/false 表示是否成功</returns>
        private static bool DecodeDic(string inputString, Type type, ref int index, out object result)
        {
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Dictionary<,>))
            {
                result = null;
                LoggerHelper.Error("Parse LuaTable error: type is not Dictionary: " + type);
                return false;
            }
            result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
            if (!Mogo.RPC.Utils.WaitChar(inputString, '{', ref index))
            {
                return false;
            }
            try
            {
                while (index < inputString.Length)
                {
                    string key;
                    bool isString;
                    object value;
                    Mogo.RPC.Utils.DecodeKey(inputString, ref index, out key, out isString);//匹配键
                    Mogo.RPC.Utils.WaitChar(inputString, '=', ref index);//匹配键值对分隔符
                    var flag = DecodeEntity(inputString, type.GetGenericArguments()[1], ref index, out value);//转换实体
                    if (flag)
                    {
                        type.GetMethod("Add").Invoke(result, new object[] { isString ? key : (object)Int32.Parse(key), value });
                    }
                    if (!Mogo.RPC.Utils.WaitChar(inputString, ',', ref index))
                        break;
                }
                Mogo.RPC.Utils.WaitChar(inputString, '}', ref index);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("Parse LuaTable error: " + inputString + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 解析实体类型 Lua table 值。
        /// </summary>
        /// <param name="inputString">Lua table字符串</param>
        /// <param name="type">Lua table数据对应实体类型</param>
        /// <param name="index">字符串偏移量</param>
        /// <param name="result">实体对象</param>
        /// <returns>返回 true/false 表示是否成功</returns>
        private static bool DecodeEntity(string inputString, Type type, ref int index, out object result)
        {
            result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
            if (!Mogo.RPC.Utils.WaitChar(inputString, '{', ref index))
            {
                return false;
            }
            try
            {
                var props = type.GetProperties();//找出实体所有属性
                while (index < inputString.Length)
                {
                    string key;
                    bool isString;
                    object value;
                    PropertyInfo valueType = null;
                    Mogo.RPC.Utils.DecodeKey(inputString, ref index, out key, out isString);//获取键
                    if (isString)//如果键为字符串，则按名称获取属性
                    {
                        valueType = type.GetProperty(key as string);
                    }
                    else//如果键为整型，则按序号获取属性
                    {
                        var i = int.Parse(key) - 1;//Lua Table 的键从1开始
                        if (i < props.Length)
                        {
                            valueType = props[i];
                        }
                    }

                    Mogo.RPC.Utils.WaitChar(inputString, '=', ref index);
                    if (valueType != null)
                    {
                        var flag = DecodeValue(inputString, valueType.PropertyType, ref index, out value);//根据类型转换并赋值
                        if (flag)
                        {
                            valueType.SetValue(result, value, null);
                        }
                    }
                    if (!Mogo.RPC.Utils.WaitChar(inputString, ',', ref index))
                        break;
                }
                Mogo.RPC.Utils.WaitChar(inputString, '}', ref index);
                return true;
            }
            catch (Exception e)
            {
                LoggerHelper.Error("Parse LuaTable error: " + inputString + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 解析值类型 Lua table 值。
        /// </summary>
        /// <param name="inputString">Lua table字符串</param>
        /// <param name="type">Lua table数据对应实体类型</param>
        /// <param name="index">字符串偏移量</param>
        /// <param name="result">值</param>
        /// <returns>返回 true/false 表示是否成功</returns>
        private static bool DecodeValue(string inputString, Type type, ref int index, out object value)
        {
            var firstChar = inputString[index];
            if (firstChar == 's')
            {
                //key is string
                var szLen = inputString.Substring(index + 1, 3);
                var lenth = Int32.Parse(szLen);
                if (lenth > 0)
                {
                    index += 4;
                    value = inputString.Substring(index, lenth);
                    index += lenth;
                    return true;
                }
                value = "";
                LoggerHelper.Error("Decode Lua Table Value Error: " + index + " " + inputString);
                return false;
            }
            else if (firstChar == '{')//如果第一个字符为花括号，表示接下来的内容为列表或实体类型
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    return DecodeDic(inputString, type, ref index, out value);
                }
                else
                    return DecodeEntity(inputString, type, ref index, out value);
            }
            else
            {
                //key is number
                var i = index;
                while (++index < inputString.Length)
                {
                    if (inputString[index] == ',' || inputString[index] == '}')
                    {
                        if (index > i)
                        {
                            var strValue = inputString.Substring(i, index - i);
                            value = GetValue(strValue, type);
                            return true;
                        }
                    }
                }
                value = GetValue("0", type);
                LoggerHelper.Error("Decode Lua Table Value Error: " + index + " " + inputString);
                return false;
            }
        }

        #endregion

        #endregion

        #region 文件路径处理

        public static string GetFileNameWithoutExtention(string fileName, char separator = '/')
        {
            var name = GetFileName(fileName, separator);
            return GetFilePathWithoutExtention(name);
        }

        public static string GetFilePathWithoutExtention(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf('.'));
        }

        public static string GetDirectoryName(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf('/'));
        }

        public static string GetFileName(string path, char separator = '/')
        {
            return path.Substring(path.LastIndexOf(separator) + 1);
        }

        public static string PathNormalize(this string str)
        {
            return str.Replace("\\", "/").ToLower();
        }

        #endregion

        #region zip

        public static void CompressDirectory(string sourcePath, string outputFilePath, int zipLevel = 0)
        {
            FileStream compressed = new FileStream(outputFilePath, FileMode.OpenOrCreate);
            compressed.CompressDirectory(sourcePath, zipLevel);
        }

        public static void DecompressToDirectory(string targetPath, string zipFilePath)
        {
            if (File.Exists(zipFilePath))
            {
                var compressed = File.OpenRead(zipFilePath);
                compressed.DecompressToDirectory(targetPath);
            }
            else
            {
                LoggerHelper.Error("Zip不存在: " + zipFilePath);
            }
        }

        public static void DecompressToMogoFile(string zipFilePath)
        {
            if (File.Exists(zipFilePath))
            {
                var source = File.OpenRead(zipFilePath);
                MogoFileSystem.Instance.Open();
                MogoFileSystem.Instance.GetAndBackUpIndexInfo();
                using (ZipInputStream decompressor = new ZipInputStream(source))
                {
                    ZipEntry entry;

                    while ((entry = decompressor.GetNextEntry()) != null)
                    {
                        if (entry.IsDirectory)
                            continue;
                        string filePath = entry.Name;
                        if (String.IsNullOrEmpty(filePath))
                            continue;

                        var info = MogoFileSystem.Instance.BeginSaveFile(filePath, entry.Size);

                        byte[] data = new byte[2048];
                        int bytesRead;
                        while ((bytesRead = decompressor.Read(data, 0, data.Length)) > 0)
                        {
                            MogoFileSystem.Instance.WriteFile(info, data, 0, bytesRead);
                        }

                        MogoFileSystem.Instance.EndSaveFile(info);
                    }
                }
                MogoFileSystem.Instance.SaveIndexInfo();
                MogoFileSystem.Instance.CleanBackUpIndex();
                MogoFileSystem.Instance.Close();
            }
            else
            {
                LoggerHelper.Error("Zip file not exist: " + zipFilePath);
            }
        }

        public static void DecompressToMogoFileAndDirectory(string targetPath, string zipFilePath)
        {
            if (File.Exists(zipFilePath))
            {
                var source = File.OpenRead(zipFilePath);
                MogoFileSystem.Instance.Open();
                MogoFileSystem.Instance.GetAndBackUpIndexInfo();
                using (ZipInputStream decompressor = new ZipInputStream(source))
                {
                    ZipEntry entry;

                    while ((entry = decompressor.GetNextEntry()) != null)
                    {
                        if (entry.IsDirectory)
                            continue;
                        string filePath = entry.Name;
                        if (String.IsNullOrEmpty(filePath))
                            continue;
                        byte[] data = new byte[2048];
                        int bytesRead;

                        if (filePath.EndsWith(".u"))
                        {
                            filePath = Path.Combine(targetPath, filePath);

                            string directoryPath = Path.GetDirectoryName(filePath);

                            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                                Directory.CreateDirectory(directoryPath);
                            using (FileStream streamWriter = File.Create(filePath))
                            {
                                while ((bytesRead = decompressor.Read(data, 0, data.Length)) > 0)
                                {
                                    streamWriter.Write(data, 0, bytesRead);
                                }
                            }
                        }
                        else
                        {
                            var info = MogoFileSystem.Instance.BeginSaveFile(filePath, entry.Size);
                            while ((bytesRead = decompressor.Read(data, 0, data.Length)) > 0)
                            {
                                MogoFileSystem.Instance.WriteFile(info, data, 0, bytesRead);
                            }
                            MogoFileSystem.Instance.EndSaveFile(info);
                        }
                    }
                }
                MogoFileSystem.Instance.SaveIndexInfo();
                MogoFileSystem.Instance.CleanBackUpIndex();
                MogoFileSystem.Instance.Close();
            }
            else
            {
                LoggerHelper.Error("Zip file not exist: " + zipFilePath);
            }
        }

        public static void CompressDirectory(this Stream target, string sourcePath, int zipLevel)
        {
            sourcePath = Path.GetFullPath(sourcePath);

            //string parentDirectory = Path.GetDirectoryName(sourcePath);

            int trimOffset = (string.IsNullOrEmpty(sourcePath)
                                  ? Path.GetPathRoot(sourcePath).Length
                                  : sourcePath.Length);


            List<string> fileSystemEntries = new List<string>();

            fileSystemEntries
                .AddRange(Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)
                              .Select(d => d + "\\"));

            fileSystemEntries
                .AddRange(Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories));


            using (ZipOutputStream compressor = new ZipOutputStream(target))
            {
                compressor.SetLevel(zipLevel);

                foreach (string filePath in fileSystemEntries)
                {
                    var trimFile = filePath.Substring(trimOffset);
                    var file = trimFile.StartsWith(@"\") ? trimFile.ReplaceFirst(@"\", "") : trimFile;
                    file = file.Replace(@"\", "/");
                    compressor.PutNextEntry(new ZipEntry(file));

                    if (filePath.EndsWith(@"\"))
                    {
                        continue;
                    }

                    byte[] data = new byte[2048];

                    using (FileStream input = File.OpenRead(filePath))
                    {
                        int bytesRead;

                        while ((bytesRead = input.Read(data, 0, data.Length)) > 0)
                        {
                            compressor.Write(data, 0, bytesRead);
                        }
                    }
                }

                compressor.Finish();
            }
        }

        public static void DecompressToDirectory(this Stream source, string targetPath)
        {
            targetPath = Path.GetFullPath(targetPath);
            using (ZipInputStream decompressor = new ZipInputStream(source))
            {
                ZipEntry entry;

                while ((entry = decompressor.GetNextEntry()) != null)
                {
                    string name = entry.Name;
                    if (entry.IsDirectory && entry.Name.StartsWith("\\"))
                        name = entry.Name.ReplaceFirst("\\", "");

                    string filePath = Path.Combine(targetPath, name);
                    string directoryPath = Path.GetDirectoryName(filePath);

                    if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    if (entry.IsDirectory)
                        continue;

                    byte[] data = new byte[2048];
                    using (FileStream streamWriter = File.Create(filePath))
                    {
                        int bytesRead;
                        while ((bytesRead = decompressor.Read(data, 0, data.Length)) > 0)
                        {
                            streamWriter.Write(data, 0, bytesRead);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create a zip archive.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="directory">The directory to zip.</param> 
        public static void PackFiles(string filename, string directory, string fileFilter)
        {
            try
            {
                FastZip fz = new FastZip();
                fz.CreateEmptyDirectories = true;
                fz.CreateZip(filename, directory, false, fileFilter);
                fz = null;
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }
        }

        public static byte[] UnpackMemory(byte[] zipMemory)
        {
            MemoryStream stream = new MemoryStream(zipMemory);
            stream.Seek(0, SeekOrigin.Begin);
            ZipInputStream s = new ZipInputStream(stream);

            ZipEntry theEntry;
            List<byte> result = new List<byte>();
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string fileName = Path.GetFileName(theEntry.Name);

                if (fileName != String.Empty)
                {
                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            var bytes = new Byte[size];
                            Array.Copy(data, bytes, size);
                            result.AddRange(bytes);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            s.Close();
            stream.Close();
            return result.ToArray();
        }

        /// <summary>
        /// Unpacks the files.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>if succeed return true,otherwise false.</returns>
        public static bool UnpackFiles(string file, string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                ZipInputStream s = new ZipInputStream(File.OpenRead(file));

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (directoryName != String.Empty)
                        Directory.CreateDirectory(dir + directoryName);

                    if (fileName != String.Empty)
                    {
                        FileStream streamWriter = File.Create(dir + theEntry.Name);

                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        streamWriter.Close();
                    }
                }
                s.Close();
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
                return false;
            }
        }

        #endregion

        #region MD5

        public static Byte[] CreateMD5(Byte[] data)
        {

            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(data);
            }
        }

        public static string FormatMD5(Byte[] data)
        {
            return System.BitConverter.ToString(data).Replace("-", "").ToLower();
        }

        /// <summary>
        /// 生成文件的md5(冯委)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static String BuildFileMd5(String filename)
        {
            String filemd5 = null;
            try
            {
                using (var fileStream = File.OpenRead(filename))
                {
                    //UnityEditor.AssetDatabase
                    var md5 = MD5.Create();
                    var fileMD5Bytes = md5.ComputeHash(fileStream);//计算指定Stream 对象的哈希值                            
                    //fileStream.Close();//流数据比较大，手动卸载 
                    //fileStream.Dispose();
                    //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”               
                    filemd5 = FormatMD5(fileMD5Bytes);
                }
            }
            catch (System.Exception ex)
            {
                LoggerHelper.Except(ex);
            }
            return filemd5;
        }

        #endregion

        #region state
        public static ulong BitSet(ulong data, int nBit)
        {
            if (nBit >= 0 && nBit < (int)sizeof(ulong) * 8)
            {
                data |= (ulong)(1 << nBit);
            }

            return data;
        }

        public static ulong BitReset(ulong data, int nBit)
        {
            if (nBit >= 0 && nBit < (int)sizeof(ulong) * 8)
            {
                data &= (ulong)(~(1 << nBit));
            };

            return data;
        }

        public static int BitTest(ulong data, int nBit)
        {
            int nRet = 0;
            if (nBit >= 0 && nBit < (int)sizeof(ulong) * 8)
            {
                data &= (ulong)(1 << nBit);
                if (data != 0) nRet = 1;
            }
            return nRet;
        }
        #endregion

        #region 几何相关

        public static void CircleXYByAngle(float angle, Vector3 O, Vector3 A, out Vector3 rnt)
        {

            float r = Vector3.Distance(O, A);
            //rnt = new Vector3();
            rnt.y = A.y;
            rnt.x = r * (float)Math.Cos(angle) + O.x;
            rnt.z = r * (float)Math.Sin(angle) + O.z;



            //return rnt;
        }

        #endregion

        #region 密钥管理

        [DllImport("key", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetIndexKey(int i);
        [DllImport("key", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetResKey(int i);

        public static byte[] GetResNumber()
        {
            List<byte> result = new List<byte>();

            for (int i = 0; i < 8; i++)
            {
                result.Add((byte)GetResKey(i));
            }
            return result.ToArray();
        }

        public static byte[] GetIndexNumber()
        {
            List<byte> result = new List<byte>();

            for (int i = 0; i < 8; i++)
            {
                result.Add((byte)GetIndexKey(i));
            }
            return result.ToArray();
        }

        #endregion

        static public string GetFullName(Transform rootTransform, Transform currentTransform)
        {
            string fullName = String.Empty;

            while (currentTransform != rootTransform)
            {
                fullName = currentTransform.name + fullName;

                if (currentTransform.parent != rootTransform)
                {
                    fullName = String.Concat('/', fullName);
                }

                currentTransform = currentTransform.parent;
            }

            return fullName;
        }

        /// <summary>
        /// 挂载object在某父上并保持本地坐标、转向、大小不变
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        public static void MountToSomeObjWithoutPosChange(Transform child, Transform parent)
        {
            Vector3 scale = child.localScale;
            Vector3 position = child.localPosition;
            Vector3 angle = child.localEulerAngles;
            child.parent = parent;
            child.localScale = scale;
            child.localEulerAngles = angle;
            child.localPosition = position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="datastr"></param>
        /// <returns>返回字符串</returns>
        public static void SendPostHttp(string Url, string datastr, Action<string> onDone, Action<HttpStatusCode> onFail)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(datastr);
            // 准备请求... 
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
            req.Method = "Post"; //Getor Post
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            Stream stream = req.GetRequestStream();
            // 发送数据 
            stream.Write(data, 0, data.Length);
            stream.Close();

            uint timerId = TimerHeap.AddTimer(15000, 0, () => { onFail(HttpStatusCode.RequestTimeout); });
            HttpWebResponse rep = (HttpWebResponse)req.GetResponse();


            if (rep.StatusCode != HttpStatusCode.OK)
            {
                TimerHeap.DelTimer(timerId);
                onFail(rep.StatusCode);
            }
            else
            {
                TimerHeap.DelTimer(timerId);
                Stream receiveStream = rep.GetResponseStream();
                Encoding encode = System.Text.Encoding.UTF8;
                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, encode);

                Char[] read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                StringBuilder sb = new StringBuilder("");
                while (count > 0)
                {
                    String readstr = new String(read, 0, count);
                    sb.Append(readstr);
                    count = readStream.Read(read, 0, 256);
                }

                rep.Close();
                readStream.Close();

                onDone(sb.ToString());
            }
        }


        public static void GetHttp(string Url, Action<string> onDone, Action<HttpStatusCode> onFail)
        {
            //Debug.LogError("GetHttp");
            // 准备请求... 
            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            HttpWebRequest req = null;
            HttpWebResponse rep = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(Url);
            }
            catch (Exception e)
            {
                //Debug.LogError(e.ToString());
                onFail(HttpStatusCode.NotAcceptable);
                return;
            }

            stopWatch.Stop();
            //Debug.LogError("req Create:" + stopWatch.ElapsedMilliseconds);

            uint timerId = TimerHeap.AddTimer(15000, 0, () => { onFail(HttpStatusCode.RequestTimeout); });
            stopWatch.Start();
            rep = (HttpWebResponse)req.GetResponse();
            stopWatch.Stop();
            //Debug.LogError("req GetResponse:" + stopWatch.ElapsedMilliseconds);

            if (rep.StatusCode != HttpStatusCode.OK)
            {

                stopWatch.Start();
                TimerHeap.DelTimer(timerId);
                onFail(rep.StatusCode);
                stopWatch.Stop();
                //Debug.LogError("req onFail:" + stopWatch.ElapsedMilliseconds);
            }
            else
            {
                TimerHeap.DelTimer(timerId);
                stopWatch.Start();
                Stream receiveStream = rep.GetResponseStream();
                stopWatch.Stop();
                //Debug.LogError("req GetResponseStream:" + stopWatch.ElapsedMilliseconds);

                stopWatch.Start();
                Encoding encode = System.Text.Encoding.UTF8;
                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, encode);

                Char[] read = new Char[256];
                int count = readStream.Read(read, 0, 256);
                StringBuilder sb = new StringBuilder("");
                while (count > 0)
                {
                    String readstr = new String(read, 0, count);
                    sb.Append(readstr);
                    count = readStream.Read(read, 0, 256);
                }

                stopWatch.Stop();
                //Debug.LogError("req readStream:" + stopWatch.ElapsedMilliseconds);

                stopWatch.Start();
                rep.Close();
                readStream.Close();
                stopWatch.Stop();
                //Debug.LogError("req Close:" + stopWatch.ElapsedMilliseconds);


                stopWatch.Start();
                onDone(sb.ToString());
                stopWatch.Stop();
                //Debug.LogError("req onDone:" + stopWatch.ElapsedMilliseconds);
            }
        }

    }
}