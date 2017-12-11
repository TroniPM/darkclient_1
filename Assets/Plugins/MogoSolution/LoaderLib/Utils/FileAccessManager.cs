#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：FileAccessManager
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.9.12
// 模块描述：文件访问管理器。
//----------------------------------------------------------------*/
#endregion

using Mogo.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mogo.Util
{
    /// <summary>
    /// 文件访问管理器。
    /// </summary>
    public class FileAccessManager
    {
        public static List<String> GetFileNamesByDirectory(String path)
        {
            return SystemSwitch.UseFileSystem ? MogoFileSystem.Instance.GetFilesByDirectory(path) : Directory.GetFiles(Path.Combine(SystemConfig.ResourceFolder, path)).ToList();
        }

        /// <summary>
        /// 读取文本文件。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static String LoadText(String fileName)
        {
            fileName = fileName.Replace('\\', '/');
            return SystemSwitch.UseFileSystem ? MogoFileSystem.Instance.LoadTextFile(fileName) : Utils.LoadFile(Path.Combine(SystemConfig.ResourceFolder, fileName));
        }

        /// <summary>
        /// 读取数据文件。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] LoadBytes(String fileName)
        {
            fileName = fileName.Replace('\\', '/');
            return SystemSwitch.UseFileSystem ? MogoFileSystem.Instance.LoadFile(fileName) : Utils.LoadByteFile(Path.Combine(SystemConfig.ResourceFolder, fileName));
        }

        public static void DecompressFile(String fileName)
        {
            if (SystemSwitch.UseFileSystem)
            {
                Utils.DecompressToMogoFileAndDirectory(SystemConfig.ResourceFolder, fileName);
                //Utils.DecompressToMogoFile(fileName);
            }
            else
            {
                //LoggerHelper.Debug("FileAccessManager.DecompressFile加压到目录");
                Utils.DecompressToDirectory(SystemConfig.ResourceFolder, fileName);
            }
        }

        public static bool IsFileExist(String fileName)
        {
            fileName = fileName.Replace('\\', '/');
            return SystemSwitch.UseFileSystem ? MogoFileSystem.Instance.IsFileExist(fileName) : File.Exists(Path.Combine(SystemConfig.ResourceFolder, fileName));
        }

        /// <summary>
        /// 读取数据文件。
        /// </summary>
        /// <param name="fileFullNames"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, byte[]>> LoadFiles(List<KeyValuePair<string, string>> fileFullNames)
        {
            return SystemSwitch.UseFileSystem ? MogoFileSystem.Instance.LoadFiles(fileFullNames) : LoadLocalFiles(fileFullNames);
        }

        private static List<KeyValuePair<string, byte[]>> LoadLocalFiles(List<KeyValuePair<string, string>> fileFullNames)
        {
            var result = new List<KeyValuePair<string, byte[]>>();
            foreach (var item in fileFullNames)
            {
                var data = Utils.LoadByteFile(Path.Combine(SystemConfig.ResourceFolder, item.Value));
                result.Add(new KeyValuePair<string, byte[]>(item.Key, data));
            }

            return result;
        }
    }
}