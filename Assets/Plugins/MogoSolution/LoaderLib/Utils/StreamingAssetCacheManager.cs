#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：StreamingAssetCacheManager
// 创建者：冯委
// 修改者列表：
// 创建日期：2014.1.13
// 模块描述：游戏运行中缓存asset到sd卡
//----------------------------------------------------------------*/
#endregion
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Mogo.Util;
using UnityEngine;

public class StreamingAssetCacheManager : MonoBehaviour
{
    public static StreamingAssetCacheManager Instance { get; private set; }

    static StreamingAssetCacheManager()
    {
        Instance=new StreamingAssetCacheManager();
    }
    //是否导出
    private static bool _bExportable;
    //暂停
    public void Pause()
    {
        LoggerHelper.Debug(".................StreamingAssetCacheManager Pause..............");
        _bExportable = false;
    }
    //开启导出
    public void Export()
    {
        LoggerHelper.Debug(".................StreamingAssetCacheManager Export..............");
        _bExportable = true;
    }
    //每隔多少帧导出一个
    public static int FrameCount = 10;
    //导出一个
    private static IEnumerator ExportItem()
    {
        Stopwatch exportTime = new Stopwatch();
        exportTime.Start();
        string[] allPathes=null;
        ResourceIndexInfo.Instance.Init(Application.streamingAssetsPath + "/ResourceIndexInfo.txt",
            () =>
            {
                allPathes = ResourceIndexInfo.Instance.GetLeftFilePathes();
                LoggerHelper.Debug("Export Items Count:"+allPathes.Length);
            });
        var sw = new Stopwatch();
        sw.Start();
        while (allPathes==null||allPathes.Length==0)
        {
            if (sw.ElapsedMilliseconds > 3000)
            {
                LoggerHelper.Info("Cache assets timeout");
                break;
            }
            LoggerHelper.Debug("allPathes==null"); 
        }
        sw.Stop();
        LoggerHelper.Debug("allPathes!=null"); 
        foreach (var srcName in allPathes)
        {
            //非空闲时不导出
            while (!_bExportable)
            {
                LoggerHelper.Debug("Game busy,Not Export:" + _bExportable);
                yield return 0;
            }

            var dstPath = String.Concat(SystemConfig.ResourceFolder, srcName);
            ////暂时用于测试的，最后会关掉
            //if (File.Exists(dstPath))
            //    File.Delete(dstPath);
            if (!File.Exists(dstPath))
            {
                var srcPath = Utils.GetStreamPath(srcName);
                LoggerHelper.Debug("Export：" + srcPath);
                var www = new WWW(srcPath);
                yield return www;
                LoggerHelper.Debug("Export Finish："+srcPath);
                if (www.bytes != null && www.bytes.Length != 0)
                {
                    var tempFile = dstPath + "_temp";
                    XMLParser.SaveBytes(tempFile, www.bytes);
                    File.Move(tempFile, dstPath);
                    LoggerHelper.Debug("Cache asset to sd card：" + dstPath);
                }
                else
                    LoggerHelper.Error(string.Format("file not exist: {0}", srcName));
                //每隔多少帧  
                var frameCount = FrameCount;
                while (frameCount-- >= 0)
                    yield return 0;
            }
        }
        LoggerHelper.Info("Cache all assets finish,all time:" + exportTime.ElapsedMilliseconds/1000);
        exportTime.Stop();
        yield return 0;
    }

    void Start()
    {
        StartCoroutine(ExportItem());
    }
}
