#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：XMLParser
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.2.1
// 模块描述：XML解析器。
//----------------------------------------------------------------*/
#endregion

using System;
using System.IO;
using System.Security;
using System.Collections.Generic;
using Mono.Xml;

namespace Mogo.Util
{
    /// <summary>
    /// XML解析器。
    /// </summary>
    public class XMLParser
    {
        /// <summary>
        /// 从指定的 URL 加载 map 数据。
        /// </summary>
        /// <param name="fileName">文件的 URL，该文件包含要加载的 XML 文档</param>
        /// <param name="key">XML 文档关键字</param>
        /// <returns>map 数据</returns>
        public Dictionary<String, Dictionary<String, String>> LoadMap(String fileName, out String key)
        {
            key = Path.GetFileNameWithoutExtension(fileName);
            var xml = Load(fileName);
            return LoadMap(xml);
        }

        /// <summary>
        /// 从指定的 URL 加载 map 数据。
        /// </summary>
        /// <param name="fileName">文件的 URL，该文件包含要加载的 XML 文档</param>
        /// <param name="map">map 数据</param>
        /// <returns>是否加载成功</returns>
        public Boolean LoadMap(String fileName, out Dictionary<String, Dictionary<String, String>> map)
        {
            try
            {
                var xml = Load(fileName);
                map = LoadMap(xml);
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
                map = null;
                return false;
            }
        }

        /// <summary>
        /// 从指定的 URL 加载 map 数据。
        /// </summary>
        /// <param name="fileName">文件的 URL，该文件包含要加载的 XML 文档</param>
        /// <param name="map">map 数据</param>
        /// <returns>是否加载成功</returns>
        public static Boolean LoadIntMap(String fileName, bool isForceOutterRecoure, out Dictionary<Int32, Dictionary<String, String>> map)
        {
            try
            {
                SecurityElement xml;
                if (isForceOutterRecoure)
                {
                    xml = LoadOutter(fileName);
                }
                else
                    xml = Load(fileName);
                if (xml == null)
                {
                    LoggerHelper.Error("File not exist: " + fileName);
                    map = null;
                    return false;
                }
                else
                {
                    map = LoadIntMap(xml, fileName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("Load Int Map Error: " + fileName + "  " + ex.Message);
                map = null;
                return false;
            }
        }

        /// <summary>
        /// 从指定的 XML 文档加载 map 数据。
        /// </summary>
        /// <param name="xml">XML 文档</param>
        /// <returns>map 数据</returns>
        public static Dictionary<Int32, Dictionary<String, String>> LoadIntMap(SecurityElement xml, string source)
        {
            var result = new Dictionary<Int32, Dictionary<String, String>>();

            var index = 0;
            foreach (SecurityElement subMap in xml.Children)
            {
                index++;
                if (subMap.Children == null || subMap.Children.Count == 0)
                {
                    LoggerHelper.Warning("empty row in row NO." + index + " of " + source);
                    continue;
                }
                Int32 key = Int32.Parse((subMap.Children[0] as SecurityElement).Text);
                if (result.ContainsKey(key))
                {
                    LoggerHelper.Warning(String.Format("Key {0} already exist, in {1}.", key, source));
                    continue;
                }

                var children = new Dictionary<String, String>();
                result.Add(key, children);
                for (int i = 1; i < subMap.Children.Count; i++)
                {
                    var node = subMap.Children[i] as SecurityElement;
                    //对属性名称部分后缀进行裁剪
                    string tag;
                    if (node.Tag.Length < 3)
                    {
                        tag = node.Tag;
                    }
                    else
                    {
                        var tagTial = node.Tag.Substring(node.Tag.Length - 2, 2);
                        if (tagTial == "_i" || tagTial == "_s" || tagTial == "_f" || tagTial == "_l" || tagTial == "_k" || tagTial == "_m")
                            tag = node.Tag.Substring(0, node.Tag.Length - 2);
                        else
                            tag = node.Tag;
                    }

                    if (node != null && !children.ContainsKey(tag))
                    {
                        if (String.IsNullOrEmpty(node.Text))
                            children.Add(tag, "");
                        else
                            children.Add(tag, node.Text.Trim());
                    }
                    else
                        LoggerHelper.Warning(String.Format("Key {0} already exist, index {1} of {2}.", node.Tag, i, node.ToString()));
                }
            }
            return result;
        }

        /// <summary>
        /// 从指定的 XML 文档加载 map 数据。
        /// </summary>
        /// <param name="xml">XML 文档</param>
        /// <returns>map 数据</returns>
        public static Dictionary<String, Dictionary<String, String>> LoadMap(SecurityElement xml)
        {
            var result = new Dictionary<String, Dictionary<String, String>>();

            foreach (SecurityElement subMap in xml.Children)
            {
                String key = (subMap.Children[0] as SecurityElement).Text.Trim();
                if (result.ContainsKey(key))
                {
                    LoggerHelper.Warning(String.Format("Key {0} already exist, in {1}.", key, xml.ToString()));
                    continue;
                }

                var children = new Dictionary<string, string>();
                result.Add(key, children);
                for (int i = 1; i < subMap.Children.Count; i++)
                {
                    var node = subMap.Children[i] as SecurityElement;
                    if (node != null && !children.ContainsKey(node.Tag))
                    {
                        if (String.IsNullOrEmpty(node.Text))
                            children.Add(node.Tag, "");
                        else
                            children.Add(node.Tag, node.Text.Trim());
                    }
                    else
                        LoggerHelper.Warning(String.Format("Key {0} already exist, index {1} of {2}.", node.Tag, i, node.ToString()));
                }
            }
            return result;
        }

        public static String LoadText(String fileName)
        {
            try
            {
                return SystemSwitch.ReleaseMode ? FileAccessManager.LoadText(fileName) : Utils.LoadResource(fileName);
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
                return "";
            }
        }

        public static byte[] LoadBytes(String fileName)
        {
            return SystemSwitch.ReleaseMode ? FileAccessManager.LoadBytes(fileName) : Utils.LoadByteResource(fileName);
        }

        /// <summary>
        /// 从指定的 URL 加载 XML 文档。
        /// </summary>
        /// <param name="fileName">文件的 URL，该文件包含要加载的 XML 文档。</param>
        /// <exception cref="System.ArgumentNullException">fileName 为 null。</exception>
        /// <exception cref="System.Security.SecurityException">调用方没有所要求的权限。</exception>
        /// <exception cref="System.ArgumentException">文件名为空，只包含空白，或包含无效字符。</exception>
        /// <exception cref="System.UnauthorizedAccessException"> 对 fileName 的访问被拒绝。</exception>
        /// <exception cref="System.IO.PathTooLongException">指定的路径、文件名或者两者都超出了系统定义的最大长度。例如，在基于 Windows 的平台上，路径必须小于 248 个字符，文件名必须小于 260个字符。</exception>
        /// <exception cref="System.NotSupportedException">fileName 字符串中间有一个冒号 (:)。</exception>
        /// <exception cref="System.OutOfMemoryException">内存不足，无法为返回的字符串分配缓冲区。</exception>
        /// <exception cref="System.IO.IOException">发生 I/O 错误。</exception>
        /// <returns>编码安全对象的 XML 对象模型。</returns>
        public static SecurityElement Load(String fileName)
        {
            String xmlText = LoadText(fileName);
            if (String.IsNullOrEmpty(xmlText))
                return null;
            else
                return LoadXML(xmlText);
        }

        public static SecurityElement LoadOutter(String fileName)
        {
            String xmlText = Utils.LoadFile(fileName.Replace('\\', '/'));
            if (String.IsNullOrEmpty(xmlText))
                return null;
            else
                return LoadXML(xmlText);
        }

        /// <summary>
        /// 从指定的字符串加载 XML 文档。
        /// </summary>
        /// <param name="xml">包含要加载的 XML 文档的字符串。</param>
        /// <returns>编码安全对象的 XML 对象模型。</returns>
        public static SecurityElement LoadXML(String xml)
        {
            try
            {
                SecurityParser securityParser = new SecurityParser();
                securityParser.LoadXml(xml);
                return securityParser.ToXml();
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
                return null;
            }
        }

        /// <summary>
        /// 保持 XML 文档。
        /// </summary>
        /// <param name="fileName">文档名称</param>
        /// <param name="xml">XML内容</param>
        public static void SaveBytes(String fileName, byte[] buffer)
        {
            if (!Directory.Exists(Utils.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Utils.GetDirectoryName(fileName));
            }
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (BinaryWriter sw = new BinaryWriter(fs))
                {
                    //开始写入
                    sw.Write(buffer);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                }
                fs.Close();
            }
        }

        /// <summary>
        /// 保存 Text 文档。
        /// </summary>
        /// <param name="fileName">文档名称</param>
        /// <param name="text">XML内容</param>
        public static void SaveText(String fileName, String text)
        {
            if (!Directory.Exists(Utils.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Utils.GetDirectoryName(fileName));
            }
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    //开始写入
                    sw.Write(text);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                }
                fs.Close();
            }
        }
    }
}