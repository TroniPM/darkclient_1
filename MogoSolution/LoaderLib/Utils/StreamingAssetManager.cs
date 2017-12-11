#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：FileAccessManager
// 创建者：冯委
// 修改者列表：
// 创建日期：2013.11.4
// 模块描述：StreamAsset管理
//----------------------------------------------------------------*/
#endregion

using UnityEngine;
using Mogo.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Security;
using System.Text;
using System.Security.Cryptography;
/// <summary>
/// streamAsset管理
/// </summary>
public class StreamingAssetManager
{
    /// <summary>
    /// 所有任务完成的回调
    /// </summary>
    public Action AllFinished { get; set; }
    //更新apk时，导出资源是否完成
    public bool ApkFinished = false;
    //所有需要导出的路径数组
    string[] allPathes;
    //批次任务的数目，默认是10
    int taskBatchCount = 10;
    //全局的所有导出的索引
    int index = 0;
    //进度条使用哪种计算方法,true是按全部数计算，false按剩余数计算
    bool way2Compute = true;
    bool bUpdateProgress = true;
    //是否同步进度条
    public bool UpdateProgress
    {
        get { return bUpdateProgress; }
        set
        {
            if (value && allPathes != null)
            {
                taskBatchCount = allPathes.Length - index;
            }
            bUpdateProgress = value;
        }
    }
    public IEnumerator LoadWWW(string path, Action<WWW> loading, Action<Byte[]> loaded)
    {
        while (true)
        {
            WWW www = new WWW(path);
            if (loading != null)
                loading(www);
            yield return www;
            if (loaded != null)
                loaded(www.bytes);
            www = null;
            yield break;
        }
    }

    public IEnumerator Loading(WWW www, Action<float> loading)
    {
        while (www != null && !www.isDone)
        {
            if (loading != null)
                loading(www.progress);
            yield return null;
        }
    }

    private bool CheckNeedFirstExport()
    {
        if (File.Exists(SystemConfig.VersionPath))
        {
            if (SystemSwitch.UseFileSystem)
            {
                if (File.Exists(SystemConfig.ResourceFolder + MogoFileSystem.FILE_NAME))
                    return false;
            }
            else
            {
                if (Directory.Exists(SystemConfig.ResourceFolder + "data")
#if !UNITY_IPHONE
                    && File.Exists(SystemConfig.ResourceFolder + DriverLib.FileName)
#endif
                    )
                    return false;
            }
        }
        return true;
    }

    //每次判断，只导出一次
    public void FirstExport()
    {
        if (!CheckNeedFirstExport())
        {
            LoggerHelper.Debug("firstExport not export");
            AllFinished();
        }
        else
        {
            LoggerHelper.Debug("firstExport export one");
            way2Compute = true;
            List<string> pathList = new List<string>();
            if (SystemSwitch.UseFileSystem)
                pathList.Add(MogoFileSystem.FILE_NAME);
            else
            {
                var datapathes = ResourceIndexInfo.Instance.GetFirstTimeResourceFilePathes();
                var metaPathes = ResourceIndexInfo.Instance.MetaList;
                pathList.AddRange(datapathes);
                pathList.AddRange(metaPathes);
            }
            allPathes = pathList.ToArray();
            taskBatchCount = allPathes.Length;
            DriverLib.Instance.StartCoroutine(ExportAll(allPathes));
        }
    }
    //更新apk时同步导出
    public void UpdateApkExport()
    {
        LoggerHelper.Info("更新apk时导出资源");
        allPathes = null;
        //进度条显示是按照总数来算还是按照剩余来算
        way2Compute = VersionManager.Instance.IsPlatformUpdate;
        allPathes = ResourceIndexInfo.Instance.GetLeftFilePathes();   
        taskBatchCount = allPathes.Length;
        DriverLib.Instance.StartCoroutine(ExportAll(allPathes));
    }

    //IEnumerator Loading(WWW www, Action<float> loading)
    //{
    //    while (www != null && !www.isDone)
    //    {
    //        if (loading != null)
    //            loading(www.progress);
    //        yield return null;
    //    }
    //}
    //IEnumerator LoadWww(string path, Action<WWW> loading, Action<Byte[]> loaded)
    //{
    //    while (true)
    //    {
    //        WWW www = new WWW(path);
    //        if (loading != null)
    //            loading(www);
    //        yield return www;
    //        if (loaded != null)
    //            loaded(www.bytes);
    //        www = null;
    //        yield break;
    //    }
    //}
    string tempFile = SystemConfig.ResourceFolder + "temp";

    IEnumerator ExportAll(string[] paths)
    {
        if (paths.Length == 1)
        {
            LoggerHelper.Debug("ExportAll one");
            //只有一个文件要导出时，一般是大文件，要单独处理进度条
            string fullpath = null;
            string s = paths[0];
            string target = String.Concat(SystemConfig.ResourceFolder, s);
            LoggerHelper.Debug("export target:" + target);
            if (!File.Exists(target))
            {
                LoggerHelper.Debug("target not exit");
                fullpath = Utils.GetStreamPath(s);
                DriverLib.Instance.StartCoroutine(LoadWWW(fullpath,
                    (www) =>
                    {
                        DriverLib.Instance.StartCoroutine(
                            Loading(www, (p) =>
                            {
                                DefaultUI.Loading((int)(p));
                                DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(4).content);
                            })
                            );
                    },
                    (pkg) =>
                    {
                        if (pkg != null && pkg.Length != 0)
                        {
                            tempFile = target + "_temp";
                            XMLParser.SaveBytes(tempFile, pkg);
                            File.Move(tempFile, target);
                            pkg = null;
                        }
                        else
                            LoggerHelper.Error("export one file not exist: " + fullpath);

                        allPathes = null;
                        if (AllFinished != null)
                        {
                            LoggerHelper.Debug("导出一个资源AllFinished");
                            AllFinished();
                        }
                        else
                        {
                            LoggerHelper.Debug("导出一个资源AllFinished为null");
                        }
                    }
                    ));
            }
        }
        else
        {
            LoggerHelper.Debug("ExportAll more than one");
            //文件大于一个时，进度条是文件数量计算的
            while (index < paths.Length)
            {
                string fullpath = null;
                string s = paths[index++];
                string target = String.Concat(SystemConfig.ResourceFolder, s);
                if (!File.Exists(target))
                {
                    fullpath = Utils.GetStreamPath(s);
                    WWW www = new WWW(fullpath);
                    yield return www;
                    if (www.bytes != null && www.bytes.Length != 0)
                    {
                        tempFile = target + "_temp";
                        XMLParser.SaveBytes(tempFile, www.bytes);
                        File.Move(tempFile, target);
                        www = null;
                    }
                    else
                        LoggerHelper.Error("file not exist: " + fullpath);
                }
                //else
                //    yield return null;
                //LoggerHelper.Debug("index: " + index);

                if (UpdateProgress)
                {
                    float progress;
                    if (way2Compute)
                    {
                        progress = (float)(index * 100) / (float)taskBatchCount;
                        DefaultUI.Loading((int)(progress));
                        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(3).content);
                    }
                    else
                    {
                        progress =
                            (float)(((float)(index - (paths.Length - taskBatchCount)) % (float)taskBatchCount) * 100) /
                            (float)taskBatchCount;
                        DefaultUI.Loading((int)(progress));
                        DefaultUI.SetLoadingStatusTip(DefaultUI.dataMap.Get(10).content, Path.GetFileName(s));
                    }
                    DefaultUI.Loading((int)(progress));

                }
            }
            allPathes = null;
            if (AllFinished != null)
            {
                LoggerHelper.Debug("导出资源AllFinished");
                AllFinished();
            }
            else
            {
                LoggerHelper.Debug("导出资源AllFinished为null");
            }
        }

    }
}