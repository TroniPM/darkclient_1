#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DefParser
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.1.16
// 模块描述：类接口XML解析器。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Mogo.Util;

namespace Mogo.RPC
{
    /// <summary>
    /// 类接口XML解析器。
    /// </summary>
    public class DefParser : DataLoader
    {
        #region 私有属性

        private const String EL_FLAGS = "Flags";
        private const String EL_TYPE = "Type";
        private const String EL_PARENT = "Parent";
        private const String EL_EXPOSED = "Exposed";
        private const String ATTR_PROPERTIES = "Properties";
        private const String ATTR_CLIENT_METHODS = "ClientMethods";
        private const String ATTR_BASE_METHODS = "BaseMethods";
        private const String ATTR_CELL_METHODS = "CellMethods";
        private const String VALUE_CLIENT = "CLIENT";

        private static DefParser m_instance;

        private Dictionary<String, EntityDef> m_entitysByName;
        private Dictionary<uint, EntityDef> m_entitysByID;

        #endregion

        #region 公有属性

        public static DefParser Instance
        {
            get { return m_instance; }
        }

        public Dictionary<String, EntityDef> EntitysByName
        {
            get { return m_entitysByName; }
        }

        #endregion

        #region 构造函数

        static DefParser()
        {
            m_instance = new DefParser();
        }

        private DefParser()
        {
            m_entitysByName = new Dictionary<string, EntityDef>();
            m_entitysByID = new Dictionary<uint, EntityDef>();
        }

        #endregion

        #region 公有方法

        public void InitEntityData()
        {
            InitEntityData(m_resourcePath + SystemConfig.ENTITY_DEFS_PATH);
        }

        /// <summary>
        /// 初始化实体数据。
        /// </summary>
        /// <param name="xmlPath">实体声明文件路径</param>
        public void InitEntityData(String xmlPath)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            InitEntityData(xmlPath, String.Concat(SystemConfig.DEFINE_LIST_FILE_NAME, m_fileExtention));
            //InitHmfEntityData(xmlPath, String.Concat(SystemConfig.DEFINE_LIST_FILE_NAME, m_fileExtention));
            sw.Stop();
            LoggerHelper.Debug("Getting MD5");
            EventDispatcher.TriggerEvent(VersionEvent.GetContentMD5);
            LoggerHelper.Debug("InitEntityData time: " + sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// 初始化实体数据。
        /// </summary>
        /// <param name="defineFilePath">实体声明文件路径</param>
        /// <param name="defineListFileName">实体声明文件名称</param>
        public void InitEntityData(String defineFilePath, String defineListFileName)
        {
            try
            {
                SecurityElement xmlDoc;
                if (m_isUseOutterConfig)
                    xmlDoc = XMLParser.LoadOutter(defineFilePath + defineListFileName);
                else
                    xmlDoc = XMLParser.Load(defineFilePath + defineListFileName);
                if (xmlDoc == null)
                {
                    LoggerHelper.Error("Entity file load failed: " + defineFilePath + defineListFileName);
                }
                ArrayList xmlRoot = xmlDoc.Children;
                if (xmlRoot != null && xmlRoot.Count != 0)
                {
                    m_entitysByName.Clear();
                    for (int i = 0; i < xmlRoot.Count; i++)
                    {
                        SecurityElement element = xmlRoot[i] as SecurityElement;
                        ParseEntitiesXmlFile(String.Concat(defineFilePath, element.Tag, m_fileExtention), element.Tag, (ushort)(i + 1), !String.IsNullOrEmpty(element.Text));
                    }

                    //找父亲
                    foreach (var child in m_entitysByName.Values)
                    {
                        foreach (var parent in m_entitysByName.Values)
                        {
                            if (child.ParentName == parent.Name)
                            {
                                child.Parent = parent;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DefineParseException(String.Format("Define parse error.\nreason: \n{0}", ex.Message), ex);
            }
        }

        /// <summary>
        /// 根据实体名称查找实体对象。
        /// </summary>
        /// <param name="name">实体名称</param>
        /// <returns>实体对象</returns>
        public EntityDef GetEntityByName(String name)
        {
            EntityDef result;
            m_entitysByName.TryGetValue(name, out result);

            return result;
        }

        /// <summary>
        /// 根据实体标识查找实体对象。
        /// </summary>
        /// <param name="id">实体标识</param>
        /// <returns>实体对象</returns>
        public EntityDef GetEntityByID(ushort id)
        {
            EntityDef result;
            m_entitysByID.TryGetValue(id, out result);

            return result;
        }

        #endregion

        #region 私有方法

        private void ParseEntitiesXmlFile(String fileName, String etyName, ushort entityID, bool needMD5 = false)
        {
            string strContent;
            if (m_isUseOutterConfig)
                strContent = Util.Utils.LoadFile(fileName.Replace('\\', '/'));
            else
                strContent = XMLParser.LoadText(fileName);


            if (needMD5)
            {
                LoggerHelper.Debug("AddMD5Content: " + fileName);
                EventDispatcher.TriggerEvent(VersionEvent.AddMD5Content, strContent);
            }

            SecurityElement xmlDoc = XMLParser.LoadXML(strContent);
            if (xmlDoc == null)
                return;
            //String etyName = Path.GetFileNameWithoutExtension(fileName);
            ParseEntitiesXml(xmlDoc, etyName, entityID);
        }

        private void ParseEntitiesXmlString(String xml, String etyName, ushort entityID)
        {
            SecurityElement xmlDoc = XMLParser.LoadXML(xml);
            ParseEntitiesXml(xmlDoc, etyName, entityID);
        }

        private void ParseEntitiesXml(SecurityElement xmlDoc, String etyName, ushort entityID)
        {
            ArrayList xmlRoot = xmlDoc.Children;
            if (xmlRoot != null && xmlRoot.Count != 0)
            {
                EntityDef etyDef = ParseDef(xmlRoot);
                etyDef.ID = entityID;
                etyDef.Name = etyName;
                if (!m_entitysByName.ContainsKey(etyDef.Name))
                    m_entitysByName.Add(etyDef.Name, etyDef);
                if (!m_entitysByID.ContainsKey(etyDef.ID))
                    m_entitysByID.Add(etyDef.ID, etyDef);
            }
        }

        private EntityDef ParseDef(ArrayList xmlRoot)
        {
            EntityDef etyDef = new EntityDef();
            foreach (SecurityElement element in xmlRoot)
            {
                if (element.Tag == EL_PARENT)
                {
                    etyDef.ParentName = element.Text.Trim();//To do: Trim()优化
                }
                else if (element.Tag == ATTR_CLIENT_METHODS)
                {
                    GetClientMethods(etyDef, element.Children);
                }
                else if (element.Tag == ATTR_BASE_METHODS)
                {
                    GetBaseMethods(etyDef, element.Children);
                }
                else if (element.Tag == ATTR_CELL_METHODS)
                {
                    GetCellMethods(etyDef, element.Children);
                }
                else if (element.Tag == ATTR_PROPERTIES)
                {
                    etyDef.Properties = GetProperties(element.Children);
                    etyDef.PropertiesList = etyDef.Properties.Values.ToList();
                }
            }
            return etyDef;
        }

        private Dictionary<ushort, EntityDefProperties> GetProperties(ArrayList properties)
        {
            Dictionary<ushort, EntityDefProperties> propList = new Dictionary<ushort, EntityDefProperties>();
            if (properties != null)
            {
                short index = -1;//属性索引
                foreach (SecurityElement element in properties)//遍历实体的所有属性
                {
                    SecurityElement flag = null;
                    SecurityElement node = null;
                    foreach (SecurityElement item in element.Children)//遍历属性的字段
                    {
                        if (item.Tag == EL_TYPE)//查找类型字段
                        {
                            node = item;
                        }
                        else if (item.Tag == EL_FLAGS)//查找客户端标记字段
                        {
                            flag = item;
                            break;
                        }
                    }
                    index++;
                    if (flag != null && !CheckFlags(flag.Text.Trim()))//判断是否为客户端使用属性
                        continue;
                    EntityDefProperties p = new EntityDefProperties();
                    p.Name = element.Tag;
                    if (node != null)
                        p.VType = TypeMapping.GetVObject(node.Text.Trim());//To do: Trim()优化

                    if (!propList.ContainsKey((ushort)index))
                        propList.Add((ushort)index, p);
                }
            }
            return propList;
        }

        private void GetClientMethods(EntityDef entity, ArrayList methods)
        {
            Dictionary<ushort, EntityDefMethod> methodByIDList = new Dictionary<ushort, EntityDefMethod>();
            Dictionary<string, EntityDefMethod> methodByNameList = new Dictionary<string, EntityDefMethod>();
            if (methods != null)
            {
                ushort index = 0;//方法索引
                foreach (SecurityElement element in methods)
                {
                    //if (element.NodeType == XmlNodeType.Comment)
                    //    continue;
                    EntityDefMethod m = new EntityDefMethod();
                    m.FuncName = element.Tag;
                    m.FuncID = index;
                    m.ArgsType = new List<VObject>();
                    if (element.Children != null)
                    {
                        foreach (SecurityElement args in element.Children)
                        {
                            //    if (args != null && args.NodeType == XmlNodeType.Element)
                            m.ArgsType.Add(TypeMapping.GetVObject(args.Text.Trim()));//To do: Trim()优化
                        }

                    }
                    //if (!methodList.ContainsKey(index))
                    methodByIDList.Add(m.FuncID, m);
                    if (!methodByNameList.ContainsKey(m.FuncName))
                        methodByNameList.Add(m.FuncName, m);
                    index++;
                }
            }
            entity.ClientMethodsByID = methodByIDList;
            entity.ClientMethodsByName = methodByNameList;
            //return methodList;
        }

        private void GetBaseMethods(EntityDef entity, ArrayList methods)
        {
            Dictionary<ushort, EntityDefMethod> methodByIDList = new Dictionary<ushort, EntityDefMethod>();
            Dictionary<string, EntityDefMethod> methodByNameList = new Dictionary<string, EntityDefMethod>();
            if (methods != null)
            {
                ushort index = 0;//方法索引
                foreach (SecurityElement element in methods)
                {
                    //判断方法参数是否为空，或者第一个Tag是否标记为公开方法
                    if (element.Children != null && element.Children.Count != 0 && (element.Children[0] as SecurityElement).Tag == EL_EXPOSED)
                    {
                        EntityDefMethod m = new EntityDefMethod();
                        m.FuncName = element.Tag;
                        m.FuncID = index;
                        m.ArgsType = new List<VObject>();
                        for (int i = 1; i < element.Children.Count; i++)//跳过第一个Tag字段
                        {
                            SecurityElement args = element.Children[i] as SecurityElement;
                            m.ArgsType.Add(TypeMapping.GetVObject(args.Text.Trim()));//To do: Trim()优化
                        }

                        //if (!methodByIDList.ContainsKey(index))
                        methodByIDList.Add(m.FuncID, m);
                        if (!methodByNameList.ContainsKey(m.FuncName))
                            methodByNameList.Add(m.FuncName, m);
                    }
                    index++;
                }
            }
            entity.BaseMethodsByID = methodByIDList;
            entity.BaseMethodsByName = methodByNameList;
            //return methodByIDList;
        }

        private void GetCellMethods(EntityDef entity, ArrayList methods)
        {
            Dictionary<ushort, EntityDefMethod> methodByIDList = new Dictionary<ushort, EntityDefMethod>();
            Dictionary<string, EntityDefMethod> methodByNameList = new Dictionary<string, EntityDefMethod>();
            if (methods != null)
            {
                ushort index = 0;//方法索引
                foreach (SecurityElement element in methods)
                {
                    //判断方法参数是否为空，或者第一个Tag是否标记为公开方法
                    if (element.Children != null && element.Children.Count != 0 && (element.Children[0] as SecurityElement).Tag == EL_EXPOSED)
                    {
                        EntityDefMethod m = new EntityDefMethod();
                        m.FuncName = element.Tag;
                        m.FuncID = index;
                        m.ArgsType = new List<VObject>();
                        for (int i = 1; i < element.Children.Count; i++)//跳过第一个Tag字段
                        {
                            SecurityElement args = element.Children[i] as SecurityElement;
                            m.ArgsType.Add(TypeMapping.GetVObject(args.Text.Trim()));//To do: Trim()优化
                        }

                        //if (!methodByIDList.ContainsKey(index))
                        methodByIDList.Add(m.FuncID, m);
                        if (!methodByNameList.ContainsKey(m.FuncName))
                            methodByNameList.Add(m.FuncName, m);
                    }
                    index++;
                }
            }
            //entity.BaseMethodsByID = methodByIDList;
            //entity.BaseMethodsByName = methodByNameList;
            entity.CellMethodsByID = methodByIDList;
            entity.CellMethodsByName = methodByNameList;
            //return methodByIDList;
        }

        private Boolean CheckFlags(String flags)
        {
            if (!String.IsNullOrEmpty(flags) && flags.Contains(VALUE_CLIENT))
                return true;
            else
                return false;
        }

        #endregion

        private void InitHmfEntityData(String defineFilePath, String defineFileListName)
        {
            try
            {
                byte[] bs = null;
                if (m_isUseOutterConfig)
                    bs = XMLParser.LoadBytes(defineFilePath + defineFileListName);
                else
                    bs = XMLParser.LoadBytes(defineFilePath + defineFileListName);
                if (bs == null)
                {
                    LoggerHelper.Error("Entity file load failed: " + defineFilePath + defineFileListName);
                }
                HMF.Hmf hmf = new HMF.Hmf();
                System.IO.MemoryStream stream = new MemoryStream(bs);
                stream.Seek(0, SeekOrigin.Begin);
                Dictionary<object, object> def = (Dictionary<object, object>)hmf.ReadObject(stream);
                List<object> elements = (List<object>)def["entities"];
                if (elements != null && elements.Count != 0)
                {
                    m_entitysByName.Clear();
                    for (int i = 0; i < elements.Count; i++)
                    {
                        ParseEntitiesHmfFile(String.Concat(defineFilePath, (string)elements[i], m_fileExtention), (string)elements[i], (ushort)(i + 1));
                    }

                    //找父亲
                    foreach (var child in m_entitysByName.Values)
                    {
                        foreach (var parent in m_entitysByName.Values)
                        {
                            if (child.ParentName == parent.Name)
                            {
                                child.Parent = parent;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DefineParseException(String.Format("Define parse error.\nreason: \n{0}", ex.Message), ex);
            }
        }

        private void ParseEntitiesHmfFile(String fileName, String etyName, ushort entityID, bool needMD5 = false)
        {
            byte[] bs = null;
            if (m_isUseOutterConfig)
                //strContent = Util.Utils.LoadFile(fileName.Replace('\\', '/'));
                bs = Util.Utils.LoadByteFile(fileName.Replace('\\', '/'));
            else
                bs = XMLParser.LoadBytes(fileName);

            HMF.Hmf hmf = new HMF.Hmf();
            System.IO.MemoryStream stream = new MemoryStream(bs);
            stream.Seek(0, SeekOrigin.Begin);
            Dictionary<object, object> def = (Dictionary<object, object>)hmf.ReadObject(stream);
            ParseEntitiesHmf(def, etyName, entityID);
        }

        private void ParseEntitiesHmf(Dictionary<object, object> def, String etyName, ushort entityID)
        {
            if (def != null && def.Count != 0)
            {
                EntityDef etyDef = ParseHmfDef(def);
                etyDef.ID = entityID;
                etyDef.Name = etyName;
                if (!m_entitysByName.ContainsKey(etyDef.Name))
                    m_entitysByName.Add(etyDef.Name, etyDef);
                if (!m_entitysByID.ContainsKey(etyDef.ID))
                    m_entitysByID.Add(etyDef.ID, etyDef);
            }
        }

        private EntityDef ParseHmfDef(Dictionary<object, object> def)
        {
            EntityDef etyDef = new EntityDef();
            foreach (var element in def)
            {
                if ((string)element.Key == EL_PARENT)
                {
                    //etyDef.ParentName = element.Text.Trim();//To do: Trim()优化
                }
                else if ((string)element.Key == ATTR_CLIENT_METHODS)
                {
                    GetHmfClientMethods(etyDef, (List<object>)element.Value);
                }
                else if ((string)element.Key == ATTR_BASE_METHODS)
                {
                    GetHmfBaseMethods(etyDef, (List<object>)element.Value);
                }
                else if ((string)element.Key == ATTR_CELL_METHODS)
                {
                    GetHmfCellMethods(etyDef, (List<object>)element.Value);
                }
                else if ((string)element.Key == ATTR_PROPERTIES)
                {
                    etyDef.Properties = GetHmfProperties((List<object>)element.Value);
                    etyDef.PropertiesList = etyDef.Properties.Values.ToList();
                }
            }
            return etyDef;
        }

        private Dictionary<ushort, EntityDefProperties> GetHmfProperties(List<object> props)
        {
            Dictionary<ushort, EntityDefProperties> propList = new Dictionary<ushort, EntityDefProperties>();
            if (props != null)
            {
                short index = -1;//属性索引
                for (int i = 0; i < props.Count; i++)//遍历实体的所有属性
                {
                    KeyValuePair<object, object> prop = ((Dictionary<object, object>)props[i]).First();
                    List<object> flag = null;
                    List<object> node = null;
                    foreach (var item in (List<object>)prop.Value)//遍历属性的字段
                    {
                        List<object> arg = (List<object>)item;
                        if ((string)arg[0] == EL_TYPE)//查找类型字段
                        {
                            node = arg;
                        }
                        else if ((string)arg[0] == EL_FLAGS)//查找客户端标记字段
                        {
                            flag = arg;
                            break;
                        }
                    }
                    index++;
                    if (flag != null && !CheckFlags(((string)flag[1]).Trim()))//判断是否为客户端使用属性
                        continue;
                    EntityDefProperties p = new EntityDefProperties();
                    p.Name = (string)prop.Key;
                    if (node != null)
                        p.VType = TypeMapping.GetVObject(((string)node[1]).Trim());//To do: Trim()优化

                    if (!propList.ContainsKey((ushort)index))
                        propList.Add((ushort)index, p);
                }
            }
            return propList;
        }

        private void GetHmfClientMethods(EntityDef etyDef, List<object> methods)
        {
            Dictionary<ushort, EntityDefMethod> methodByIDList = new Dictionary<ushort, EntityDefMethod>();
            Dictionary<string, EntityDefMethod> methodByNameList = new Dictionary<string, EntityDefMethod>();
            if (methods != null)
            {
                ushort index = 0;//方法索引
                for (int i = 0; i < methods.Count; i++)
                {
                    EntityDefMethod m = new EntityDefMethod();
                    Dictionary<object, object> fun = (Dictionary<object, object>)methods[i];
                    KeyValuePair<object, object> kv = fun.First();
                    m.FuncName = (string)kv.Key;
                    m.FuncID = index;
                    m.ArgsType = new List<VObject>();
                    if (kv.Value != null)
                    {
                        foreach (var args in (List<object>)kv.Value)
                        {
                            List<object> funargs = (List<object>)args;
                            m.ArgsType.Add(TypeMapping.GetVObject(((string)funargs[1]).Trim()));//To do: Trim()优化
                        }

                    }
                    methodByIDList.Add(m.FuncID, m);
                    if (!methodByNameList.ContainsKey(m.FuncName))
                        methodByNameList.Add(m.FuncName, m);
                    index++;
                }
            }
            etyDef.ClientMethodsByID = methodByIDList;
            etyDef.ClientMethodsByName = methodByNameList;
        }

        private void GetHmfBaseMethods(EntityDef etyDef, List<object> methods)
        {
            Dictionary<ushort, EntityDefMethod> methodByIDList = new Dictionary<ushort, EntityDefMethod>();
            Dictionary<string, EntityDefMethod> methodByNameList = new Dictionary<string, EntityDefMethod>();
            if (methods != null)
            {
                ushort index = 0;//方法索引
                for(int i = 0; i < methods.Count; i++)
                {
                    //判断方法参数是否为空，或者第一个Tag是否标记为公开方法
                    Dictionary<object, object> fun = (Dictionary<object, object>)methods[i];
                    KeyValuePair<object, object> kv = fun.First();
                    List<object> v = (List<object>)kv.Value;
                    if (v != null && v.Count != 0 && (string)((List<object>)v[0])[0] == EL_EXPOSED)
                    {
                        EntityDefMethod m = new EntityDefMethod();
                        m.FuncName = (string)kv.Key;
                        m.FuncID = index;
                        m.ArgsType = new List<VObject>();
                        for (int j = 1; j < v.Count; j++)//跳过第一个Tag字段
                        {
                            List<object> args = (List<object>)v[j];
                            m.ArgsType.Add(TypeMapping.GetVObject(((string)args[1]).Trim()));//To do: Trim()优化
                        }

                        //if (!methodByIDList.ContainsKey(index))
                        methodByIDList.Add(m.FuncID, m);
                        if (!methodByNameList.ContainsKey(m.FuncName))
                            methodByNameList.Add(m.FuncName, m);
                    }
                    index++;
                }
            }
            etyDef.BaseMethodsByID = methodByIDList;
            etyDef.BaseMethodsByName = methodByNameList;
        }

        private void GetHmfCellMethods(EntityDef etyDef, List<object> methods)
        {
            Dictionary<ushort, EntityDefMethod> methodByIDList = new Dictionary<ushort, EntityDefMethod>();
            Dictionary<string, EntityDefMethod> methodByNameList = new Dictionary<string, EntityDefMethod>();
            if (methods != null)
            {
                ushort index = 0;//方法索引
                for(int i = 0; i < methods.Count; i++)
                {
                    //判断方法参数是否为空，或者第一个Tag是否标记为公开方法
                    Dictionary<object, object> fun = (Dictionary<object, object>)methods[i];
                    KeyValuePair<object, object> kv = fun.First();
                    List<object> v = (List<object>)kv.Value;
                    if (v != null && v.Count != 0 && (string)((List<object>)v[0])[0] == EL_EXPOSED)
                    {
                        EntityDefMethod m = new EntityDefMethod();
                        m.FuncName = (string)kv.Key;
                        m.FuncID = index;
                        m.ArgsType = new List<VObject>();
                        for (int j = 1; j < v.Count; j++)//跳过第一个Tag字段
                        {
                            List<object> args = (List<object>)v[j];
                            m.ArgsType.Add(TypeMapping.GetVObject(((string)args[1]).Trim()));//To do: Trim()优化
                        }

                        methodByIDList.Add(m.FuncID, m);
                        if (!methodByNameList.ContainsKey(m.FuncName))
                            methodByNameList.Add(m.FuncName, m);
                    }
                    index++;
                }
            }
            etyDef.CellMethodsByID = methodByIDList;
            etyDef.CellMethodsByName = methodByNameList;
        }
    }
}