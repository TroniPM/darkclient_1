#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：GameData
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.26
// 模块描述：配置数据抽象类。
//----------------------------------------------------------------*/
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Mogo.Util;
using System.Diagnostics;
using HMF;

namespace Mogo.GameData
{
    public abstract class GameData
    {
        public int id { get; protected set; }

        protected static Dictionary<int, T> GetDataMap<T>()
        {
            Dictionary<int, T> dataMap;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var type = typeof(T);
            var fileNameField = type.GetField("fileName");
            if (fileNameField != null)
            {
                var fileName = fileNameField.GetValue(null) as String;
                var result = GameDataControler.Instance.FormatData(fileName, typeof(Dictionary<int, T>), type);
                dataMap = result as Dictionary<int, T>;
            }
            else
            {
                dataMap = new Dictionary<int, T>();
            }
            sw.Stop();
            LoggerHelper.Info(String.Concat(type, " time: ", sw.ElapsedMilliseconds));
            return dataMap;
        }
    }
    public abstract class GameData<T> : GameData where T : GameData<T>
    {
        private static Dictionary<int, T> m_dataMap;

        public static Dictionary<int, T> dataMap
        {
            get
            {
                if (m_dataMap == null)
                    m_dataMap = GetDataMap<T>();
                return m_dataMap;
            }
            set { m_dataMap = value; }
        }
    }

    public class GameDataControler : DataLoader
    {
        private List<Type> m_defaultData = new List<Type>() { typeof(GlobalData), typeof(MapData), typeof(LanguageData), typeof(UIMapData),
                                                                typeof(SoundData), typeof(InstanceLevelGridPosData), typeof(MapUIMappingData) };

        private static GameDataControler m_instance;

        public static GameDataControler Instance
        {
            get { return m_instance; }
        }

        static GameDataControler()
        {
            m_instance = new GameDataControler();
        }

        public static void Init(Action<int, int> progress = null, Action finished = null)
        {
            if (SystemSwitch.UseHmf)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                m_instance.LoadData(m_instance.m_defaultData, m_instance.FormatHmfData, null);
                sw.Stop();
                LoggerHelper.Info("InitSynHmfData time: " + sw.ElapsedMilliseconds);
                if (m_isPreloadData)
                {
                    Action action = () => { m_instance.InitAsynData(m_instance.FormatHmfData, progress, finished); };
                    if (SystemSwitch.ReleaseMode)
                        action.BeginInvoke(null, null);
                    else
                        action();
                }
                else
                {
                    finished();
                }
            }
            else
            {
                m_instance.LoadData(m_instance.m_defaultData, m_instance.FormatXMLData, null);
                if (m_isPreloadData)
                {
                    Action action = () => { m_instance.InitAsynData(m_instance.FormatXMLData, progress, finished); };
                    if (SystemSwitch.ReleaseMode)
                        action.BeginInvoke(null, null);
                    else
                        action();
                }
                else
                {
                    finished();
                }
            }
        }

        /// <summary>
        /// 进行读取数据准备工作和调用处理方法
        /// </summary>
        /// <param name="formatData">格式化数据方法</param>
        /// <param name="progress">处理进度回调</param>
        /// <param name="finished">处理完成回调</param>
        private void InitAsynData(Func<string, Type, Type, object> formatData, Action<int, int> progress, Action finished)
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                List<Type> gameDataType = new List<Type>();
                Assembly ass = typeof(GameDataControler).Assembly;
                var types = ass.GetTypes();
                foreach (var item in types)
                {
                    if (item.Namespace == "Mogo.GameData")
                    {
                        var type = item.BaseType;
                        while (type != null)
                        {
                            //#if UNITY_IPHONE
                            if (type == typeof(GameData) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(GameData<>)))//type == typeof(GameData) || 
                            //#else
                            //                                if ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(GameData<>)))
                            //#endif
                            {
                                if (!m_defaultData.Contains(item))
                                    gameDataType.Add(item);
                                break;
                            }
                            else
                            {
                                type = type.BaseType;
                            }
                        }
                    }
                }
                LoadData(gameDataType, formatData, progress);
                sw.Stop();
                LoggerHelper.Debug("Asyn GameData init time: " + sw.ElapsedMilliseconds);
                GC.Collect();
                if (finished != null)
                    finished();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("InitData Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 加载数据逻辑
        /// </summary>
        /// <param name="gameDataType">加载数据列表</param>
        /// <param name="formatData">处理数据方法</param>
        /// <param name="progress">数据处理进度</param>
        private void LoadData(List<Type> gameDataType, Func<string, Type, Type, object> formatData, Action<int, int> progress)
        {
            var count = gameDataType.Count;
            var i = 1;
            foreach (var item in gameDataType)
            {
                var p = item.GetProperty("dataMap", ~BindingFlags.DeclaredOnly);
                var fileNameField = item.GetField("fileName");
                if (p != null && fileNameField != null)
                {
                    var fileName = fileNameField.GetValue(null) as String;
                    var result = formatData(String.Concat(m_resourcePath, fileName, m_fileExtention), p.PropertyType, item);
                    p.GetSetMethod().Invoke(null, new object[] { result });
                }
                if (progress != null)
                    progress(i, count);
                i++;
            }
        }

        public object FormatData(string fileName, Type dicType, Type type)
        {
            if (SystemSwitch.UseHmf)
                return FormatHmfData(String.Concat(m_resourcePath, fileName, m_fileExtention), dicType, type);
            else
                return FormatXMLData(String.Concat(m_resourcePath, fileName, m_fileExtention), dicType, type);
        }

        #region xml

        private object FormatXMLData(string fileName, Type dicType, Type type)
        {
            object result = null;
            try
            {
                //LoggerHelper.Debug(fileName);
                //var dicType = dicProp.PropertyType;
                result = dicType.GetConstructor(Type.EmptyTypes).Invoke(null);
                Dictionary<Int32, Dictionary<String, String>> map;//int32 为 id, string 为 属性名, string 为 属性值
                if (XMLParser.LoadIntMap(fileName, m_isUseOutterConfig, out map))
                {
                    var props = type.GetProperties();//获取实体属性
                    foreach (var item in map)
                    {
                        var t = type.GetConstructor(Type.EmptyTypes).Invoke(null);//构造实体实例
                        foreach (var prop in props)
                        {
                            if (prop.Name == "id")
                            {
                                prop.SetValue(t, item.Key, null);
                            }
                            else
                            {
                                if (item.Value.ContainsKey(prop.Name))
                                {
                                    var value = Utils.GetValue(item.Value[prop.Name], prop.PropertyType);
                                    prop.SetValue(t, value, null);
                                }
                            }
                        }
                        dicType.GetMethod("Add").Invoke(result, new object[] { item.Key, t });
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("FormatData Error: " + fileName + "  " + ex.Message);
            }

            return result;
        }

        #endregion

        #region hmf

        private object FormatHmfData(string fileName, Type dicType, Type type)
        {
            object result = null;
            try
            {
                //var dicType = dicProp.PropertyType;
                result = result = dicType.GetConstructor(Type.EmptyTypes).Invoke(null);
                //LoggerHelper.Warning("fileName: " + fileName);
                byte[] bs = XMLParser.LoadBytes(fileName);
                System.IO.MemoryStream stream = new MemoryStream(bs);
                stream.Seek(0, SeekOrigin.Begin);

                Hmf h = new Hmf();
                Dictionary<object, object> map = (Dictionary<object, object>)h.ReadObject(stream);

                var props = type.GetProperties();//获取实体属性
                foreach (var item in map)
                {
                    var t = type.GetConstructor(Type.EmptyTypes).Invoke(null);//构造实体实例
                    foreach (var prop in props)
                    {
                        if (prop.Name == "id")
                        {
                            var v = Utils.GetValue((string)item.Key, prop.PropertyType);
                            prop.SetValue(t, v, null);
                        }
                        else
                        {
                            Dictionary<object, object> m = (Dictionary<object, object>)item.Value;
                            if (m.ContainsKey((object)prop.Name))
                            {
                                var value = Utils.GetValue((string)m[(object)prop.Name], prop.PropertyType);
                                prop.SetValue(t, value, null);
                            }
                        }
                    }
                    var v1 = Utils.GetValue((string)item.Key, typeof(Int32));
                    dicType.GetMethod("Add").Invoke(result, new object[] { v1, t });
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("FormatData Error: " + fileName + "  " + ex.Message);
            }
            return result;
        }

        #endregion
    }
}

public abstract class DataLoader
{
    protected static readonly bool m_isPreloadData = true;
    protected readonly String m_resourcePath;
    protected readonly String m_fileExtention;
    protected readonly bool m_isUseOutterConfig;
    protected Action<int, int> m_progress;
    protected Action m_finished;

    protected DataLoader()
    {
        m_isUseOutterConfig = SystemConfig.IsUseOutterConfig;
        if (m_isUseOutterConfig)
        {
            m_resourcePath = String.Concat(SystemConfig.OutterPath, SystemConfig.CONFIG_SUB_FOLDER);
            m_fileExtention = SystemConfig.XML;
        }
        else
        {
            m_resourcePath = SystemConfig.CONFIG_SUB_FOLDER;//兼容文件模块
            m_fileExtention = SystemConfig.CONFIG_FILE_EXTENSION;
        }
    }
}
