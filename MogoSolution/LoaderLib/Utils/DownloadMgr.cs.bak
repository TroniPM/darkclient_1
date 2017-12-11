#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：DownloadMgr
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.5.4
// 模块描述：下载管理器。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;
using UnityEngine;
using System.Linq;
using Mogo.Util;

public class ThreadDownloadBreakPoint
{
    public DownloadMgr Mgr { get; set; }
    public DownloadTask Task { get; set; }
    public ThreadDownloadBreakPoint()
    { }

    public ThreadDownloadBreakPoint(DownloadMgr mgr, DownloadTask task)
    {
        Mgr = mgr;
        Task = task;
    }
    internal void Download()
    {
        Mgr.DownloadFileBreakPoint(Task.Url, Task.FileName);
    }
}

public class DownloadTask
{
    public string Url { get; set; }
    public string FileName { get; set; }
    public Action<int, long, long> TotalProgress { get; set; }
    public Action<int> Progress { get; set; }
    public Action<long> TotalBytesToReceive { get; set; }
    public Action<long> BytesReceived { get; set; }
    public String MD5 { get; set; }
    public Action Finished { get; set; }
    public Action<Exception> Error { get; set; }
    public bool bFineshed = false;//文件是否下载完成
    public bool bDownloadAgain = false;//是否需要从新下载，如果下载出错的时候会从新下
    public void OnTotalBytesToReceive(long size)
    {
        if (TotalBytesToReceive != null)
            TotalBytesToReceive(size);
    }

    public void OnBytesReceived(long size)
    {
        if (BytesReceived != null)
            BytesReceived(size);
    }

    public void OnTotalProgress(int p, long totalSize, long receivedSize)
    {
        if (TotalProgress != null)
            TotalProgress(p, totalSize, receivedSize);
    }

    public void OnProgress(int p)
    {
        if (Progress != null)
            Progress(p);
    }

    public void OnFinished()
    {
        if (Finished != null)
            Finished();
    }

    public void OnError(Exception ex)
    {
        if (Error != null)
            Error(ex);
    }

}

public class DownloadMgr
{
    #region 断点续传

    public static bool BreakPoint { get; set; }

    //支持断点续传的下载，如果本地已经有文件的部分，只会下载剩余的部分
    public void DownloadFileBreakPoint(string address, string fileName)
    {
        try
        {
            var resquestUrl = new Uri(address);
            var request = (HttpWebRequest)WebRequest.Create(resquestUrl);
            var response = (HttpWebResponse)request.GetResponse();
            var contentLength = response.ContentLength;
            response.Close();
            request.Abort();
            //剩余文件长度
            var leftSize = contentLength;
            //开始读写的位置
            long position = 0;
            if (File.Exists(fileName))
            {
                LoggerHelper.Debug("需要下载的文件存在：" + fileName);
                using (
                    var sw = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                        FileShare.ReadWrite))
                {
                    leftSize = contentLength - sw.Length;
                    position = sw.Length;
                }
            }

            var partRequest = (HttpWebRequest)WebRequest.Create(resquestUrl);
            if (leftSize > 0)
            {
                partRequest.AddRange((int)position, (int)(position + leftSize));
                var partResponse = (HttpWebResponse)partRequest.GetResponse();
                ////从response中读取字节流
                ReadBytesFromResponse(address, partResponse, position, leftSize, contentLength, fileName);
                partResponse.Close();
            }
            partRequest.Abort();
            ////下载完成
            Finished(address);
        }
        catch (Exception e)
        {
            LoggerHelper.Error("DownloadFileBreakPoint Error：" + e.Message);
            Finished(address, e);
        }
    }

    //从response获取字节流
    internal void ReadBytesFromResponse(string requestURL, WebResponse response, long allFilePointer, long length, long totalSize, string fileName)
    {
        try
        {
            var bufferLength = (int)length;
            var buffer = new byte[bufferLength];

            //本块的位置指针
            var currentChunkPointer = 0;
            var offset = 0;
            using (var respStream = response.GetResponseStream())
            {
                int receivedBytesCount;
                do
                {
                    receivedBytesCount = respStream.Read(buffer, offset, bufferLength - offset);

                    offset += receivedBytesCount;

                    if (receivedBytesCount > 0)
                    {
                        var bufferCopyed = new byte[receivedBytesCount];
                        Buffer.BlockCopy(buffer, currentChunkPointer, bufferCopyed, 0, bufferCopyed.Length);

                        //写数据流到文件中
                        using (var sw = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            sw.Position = allFilePointer;
                            sw.Write(bufferCopyed, 0, bufferCopyed.Length);
                            sw.Close();
                        }

                        var progress = ((int)allFilePointer + bufferCopyed.Length) /
                                         ((float)totalSize);
                        //触发数据到达事件
                        Progress(requestURL, (int)(progress * 100));
                        currentChunkPointer += receivedBytesCount; //本块的位置指针 
                        allFilePointer += receivedBytesCount; //整个文件的位置指针 
                    }
                } while (receivedBytesCount != 0);
            }
        }
        catch (Exception e)
        {
            LoggerHelper.Error("ReadBytesFromResponse Error：" + e.Message);
            Finished(requestURL, e);
        }
    }

    //断点续传的进度回调
    internal void Progress(string url, int progress)
    {
        Action action = () =>
        {
            if (ContainKey(url))
            {
                var task = GetTask(url);
                task.OnTotalProgress(progress, 0, 0);
                //LoggerHelper.Debug("进度：" + progress);
                //添加处理taskProgress
                if (TaskProgress != null)
                {
                    var count = tasks.Count(ta => ta.bFineshed);
                    var filename = task.FileName.Substring(task.FileName.LastIndexOf("/") + 1);
                    TaskProgress(tasks.Count, count, filename);
                    //LoggerHelper.Debug("进度处理：" + filename);
                }
            }
        };
        action.Invoke();
    }
    //断点续传的完成回调
    internal void Finished(string url, Exception e = null)
    {
        LoggerHelper.Debug("Finished111111111111111");
        var task = GetTask(url);
        if (task != null)
        {
            //先校验网络是否异常
            if (e != null)
            {
                LoggerHelper.Error(url + "下载出错:" + e.Message);
                HandleNetworkError(e,
                    () =>
                    {
                        //跳过当前这个下载下一个
                        task.bDownloadAgain = false;
                        task.bFineshed = true;
                        CheckDownLoadList();
                    },
                    () =>
                    {
                        //从新下载这个
                        task.bDownloadAgain = true;
                        task.bFineshed = false;
                        CheckDownLoadList();
                    },
                    () => DownloadFinishWithMd5(task));
            }
            else
            {
                DownloadFinishWithMd5(task);
            }
        }
        else
        {
            LoggerHelper.Debug("Finished Task Null");
        }
    }

    private void DownloadFinishWithMd5(DownloadTask task)
    {
        LoggerHelper.Debug("DownloadFinishWithMd51111");
        //验证MD5
#if UNITY_IPHONE
    //ios下如果封装该方法在一个函数中，调用该函数来产生文件的MD5的时候，就会抛JIT异常。
    //如果直接把这个产生MD5的方法体放在直接执行，就可以正常执行，这个原因还不清楚。
                string md5Compute = null;
				using(System.IO.FileStream fileStream = System.IO.File.OpenRead(task.FileName))
				{
					System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
					byte[] fileMD5Bytes = md5.ComputeHash(fileStream);
                    md5Compute = System.BitConverter.ToString(fileMD5Bytes).Replace("-", "").ToLower();
				}
#else
        var md5Compute = Utils.BuildFileMd5(task.FileName);
#endif
        //md5验证失败
        if (md5Compute.Trim() != task.MD5.Trim())
        {
            //如果md5验证失败，删除原文件
            if (File.Exists(task.FileName))
                File.Delete(task.FileName);
            LoggerHelper.Error("断点MD5验证失败，从新下载：" + task.FileName + "--" + md5Compute + " vs " + task.MD5);
            task.bDownloadAgain = true;
            task.bFineshed = false;
            CheckDownLoadList();
            return;
        }
        //所有通过验证就认为是下载完成
        LoggerHelper.Debug("断点下载验证全部通过，下载完成：" + task.FileName);
        if (FileDecompress != null)
            FileDecompress(true);
        task.bDownloadAgain = false;
        task.bFineshed = true;
        task.Finished();
        if (FileDecompress != null)
            FileDecompress(false);
        LoggerHelper.Debug("断点下载完成后，再次核对下载列表");
        CheckDownLoadList();
    }

    #endregion
    private static DownloadMgr _mInstance;

    public static DownloadMgr Instance
    {
        get { return _mInstance ?? (_mInstance = new DownloadMgr()); }
    }

    //private String m_fileListPath = SystemConfig.FileListPath;
    //private String m_resourceFolder = SystemConfig.ResourceFolder;
    //private String m_serverUrl = "http://192.168.43.253/";//http://192.168.0.2/micro/
    //private String m_filesInfoName = "FilesInfo.txt";
    //private String m_resourcesFolder = "MogoResources/";
    private const string LogPath = "logurl";

    private readonly WebClient _webClient;
    //所有的下载任务
    public List<DownloadTask> tasks = new List<DownloadTask>();
    //添加一个taskProgress的回调,第一个参数总任务数，第二个是当前任务索引,第三个是下载的文件名
    public Action<int, int, string> TaskProgress;
    //全部下载完成的回调函数
    public Action AllDownloadFinished { get; set; }
    //任务完成时解压文件的回调函数
    public Action<bool> FileDecompress { get; set; }
    public DownloadMgr()
    {
        _webClient = new WebClient();
        if (!BreakPoint)
        {
            //webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;
            _webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
            _webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
        }
    }

    static DownloadMgr()
    {
        BreakPoint = true;
    }

    //从下载任务列表中获得对应的任务
    DownloadTask GetTask(String url)
    {
        return tasks.FirstOrDefault(task => url == task.Url);
    }

    //是否包含这个url的任务
    bool ContainKey(String url)
    {
        return tasks.Any(task => url == task.Url);
    }
    //是否显示错误消息
    private const bool _showMsg = true;
    //添加一个网络异常处理的功能,mycontinue跳过当前这个下载下一个
    static void HandleNetworkError(Exception e, Action mycontinue, Action again, Action finished = null)
    {
        LoggerHelper.Except(e);
        Action action = () =>
        {
            //LoggerHelper.Debug("webclient error:" + e.Message);
            if (e.Message.Contains("ConnectFailure") //连接失败
                || e.Message.Contains("NameResolutionFailure") //域名解析失败
                || e.Message.Contains("No route to host")) //找不到主机
            {
                LoggerHelper.Error("-----------------Webclient ConnectFailure-------------");
                //异常时弹出网络设置对话框
                ShowMsgBoxForNetworkDisconnect(again, _showMsg ? ":" + e.Message : "");
            }
            else
                //(404) Not Found
                if (e.Message.Contains("(404) Not Found") || e.Message.Contains("403"))
                {
                    LoggerHelper.Error("-----------------WebClient NotFount-------------");
                    //抛出一个error,并且继续下载下一个
                    //mycontinue();
                    //服务器维护中，请稍后再试
                    ShowMsgForServerMaintenance(again, _showMsg ? ":" + e.Message : "");
                }
                else
                    //Disk full             
                    if (e.Message.Contains("Disk full"))
                    {
                        LoggerHelper.Error("-----------------WebClient Disk full-------------");
                        ShowMsgBoxForDiskFull(again, _showMsg ? ":" + e.Message : "");
                    }
                    else
                        //timed out
                        if (e.Message.Contains("timed out") || e.Message.Contains("Error getting response stream"))
                        {
                            LoggerHelper.Error("-----------------WebClient timed out-------------");
                            //again();
                            ShowMsgForTimeout(again, _showMsg ? ":" + e.Message : "");
                        }
                        else
                            //Sharing violation on path
                            if (e.Message.Contains("Sharing violation on path"))
                            {
                                LoggerHelper.Error("-----------------WebClient Sharing violation on path-------------");
                                again();
                            }
                            else
                            {
                                //LoggerHelper.Debug("-----------------WebClient myfinished-------------");
                                //if (finished != null) finished();
                                //again();
                                ShowMsgForUnknown(again, _showMsg ? ":" + e.Message : "");
                            }
        };
        DriverLib.Invoke(action);
    }
    //未知的网络错误
    private static void ShowMsgForUnknown(Action again, string msg = "")
    {
        var languageData = DefaultUI.dataMap;
        ForwardLoadingMsgBoxLib.Instance.ShowMsgBox(languageData.Get(50).content, languageData.Get(7).content,
            languageData.Get(52).content + msg, //网络异常
            (isOk) =>
            {
                if (isOk)
                {
                    ForwardLoadingMsgBoxLib.Instance.Hide();
                    //重试
                    again();
                }
                else
                {
                    //退出
                    Application.Quit();
                }
            });
    }
    //超时，采用倒计时对话框
    private static void ShowMsgForTimeout(Action again, string msg = "")
    {
        var languageData = DefaultUI.dataMap;
        ForwardLoadingMsgBoxLib.Instance.ShowMsgBox(languageData.Get(50).content, languageData.Get(7).content,
            languageData.Get(52).content + msg, //网络不稳定
            (isOk) =>
            {
                if (isOk)
                {
                    ForwardLoadingMsgBoxLib.Instance.Hide();
                    //重试
                    again();
                }
                else
                {
                    //退出
                    Application.Quit();
                }
            });

        //var languageData = DefaultUI.dataMap;
        //ForwardLoadingMsgBoxLib.Instance.ShowMsgBoxWithCountDown(languageData[50].content, languageData[7].content,
        //    languageData[15].content + msg, //网络不稳定
        //                (isOk) =>
        //                {
        //                    if (isOk)
        //                    {
        //                        ForwardLoadingMsgBoxLib.Instance.Hide();
        //                        //重试
        //                        again();
        //                    }
        //                    else
        //                    {
        //                        //退出
        //                        Application.Quit();
        //                    }
        //                },
        //                10000,//倒计时10秒
        //                () =>
        //                {     //10秒到了，自动重试
        //                    ForwardLoadingMsgBoxLib.Instance.Hide();
        //                    //重试
        //                    again();
        //                 }
        //);
    }
    //服务器维护中，下载文件404/403时使用
    private static void ShowMsgForServerMaintenance(Action again, string msg = "")
    {
        var languageData = DefaultUI.dataMap;
        ForwardLoadingMsgBoxLib.Instance.ShowMsgBox(languageData.Get(50).content, languageData.Get(7).content,
            languageData.Get(14).content + msg, //服务器维护中，请稍后再试
            (isOk) =>
            {
                if (isOk)
                {
                    ForwardLoadingMsgBoxLib.Instance.Hide();
                    //重试
                    again();
                }
                else
                {
                    //退出
                    Application.Quit();
                }
            });
    }
    private static void ShowMsgBoxForNetworkDisconnect(Action again, string msg = "")
    {
        //异常时弹出网络设置对话框
        var languageData = DefaultUI.dataMap;
        ForwardLoadingMsgBoxLib.Instance.ShowMsgBox(languageData.Get(50).content, languageData.Get(51).content, languageData.Get(52).content + msg, (isOk) =>
        {
            if (isOk)
            {
                ForwardLoadingMsgBoxLib.Instance.Hide();
                again();
            }
            else
            {
#if UNITY_ANDROID
                if (Application.platform == RuntimePlatform.Android)
                {
                    var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    var mMainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                    mMainActivity.Call("gotoNetworkSetting");
                }
#endif
            }
        });
    }

    private static void ShowMsgBoxForDiskFull(Action again, string msg = "")
    {
        var languageData = DefaultUI.dataMap;
        ForwardLoadingMsgBoxLib.Instance.ShowMsgBox(languageData.Get(50).content, languageData.Get(7).content, languageData.Get(53).content + msg, (isOk) =>
        {
            if (isOk)
            {
                //重试
                ForwardLoadingMsgBoxLib.Instance.Hide();
                again();
            }
            else
            {
                //退出
                Application.Quit();
            }
        });
    }

    void webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        String keyurl = e.UserState.ToString();
        DownloadTask task = GetTask(keyurl);
        if (task != null)
        {
            //先校验网络是否异常
            if (e.Error != null)
            {
                HandleNetworkError(e.Error,
                    () =>
                    {
                        //跳过当前这个下载下一个
                        task.bDownloadAgain = false;
                        task.bFineshed = true;
                        CheckDownLoadList();
                    },
                    () =>
                    {
                        //从新下载这个
                        task.bDownloadAgain = true;
                        task.bFineshed = false;
                        CheckDownLoadList();
                    });
            }
            else
            {
                //验证MD5
#if UNITY_IPHONE
				//ios下如果封装该方法在一个函数中，调用该函数来产生文件的MD5的时候，就会抛JIT异常。
				//如果直接把这个产生MD5的方法体放在直接执行，就可以正常执行，这个原因还不清楚。
				string md5Compute = null;
				using(System.IO.FileStream fileStream = System.IO.File.OpenRead(task.FileName))
				{
					System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
					byte[] fileMD5Bytes = md5.ComputeHash(fileStream);                                     
					md5Compute = System.BitConverter.ToString(fileMD5Bytes).Replace("-","").ToLower();
				}
#else
                String md5Compute = Utils.BuildFileMd5(task.FileName);
#endif
                //md5验证失败
                if (md5Compute.Trim() != task.MD5.Trim())
                {
                    //如果md5验证失败，删除原文件
                    if (File.Exists(task.FileName))
                        File.Delete(task.FileName);
                    LoggerHelper.Error("MD5验证失败，从新下载：" + task.FileName + "--" + md5Compute + " vs " + task.MD5);
                    task.bDownloadAgain = true;
                    task.bFineshed = false;
                    CheckDownLoadList();
                    return;
                }
                //所有通过验证就认为是下载完成
                LoggerHelper.Debug("下载验证全部通过，下载完成：" + task.FileName);
                if (FileDecompress != null)
                    FileDecompress(true);
                task.bDownloadAgain = false;
                task.bFineshed = true;
                task.Finished();
                if (FileDecompress != null)
                    FileDecompress(false);
                LoggerHelper.Debug("下载完成后，再次核对下载列表");
                CheckDownLoadList();
            }
        }
    }

    void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        var key = e.UserState.ToString();
        if (ContainKey(key))
        {
            var task = GetTask(key);
            task.OnProgress(e.ProgressPercentage);
            task.OnTotalBytesToReceive(e.TotalBytesToReceive);
            task.OnBytesReceived(e.BytesReceived);
            task.OnTotalProgress(e.ProgressPercentage, e.TotalBytesToReceive, e.BytesReceived);
            //添加处理taskProgress
            if (TaskProgress != null)
            {
                int count = tasks.Where(ta => ta.bFineshed).Count();
                string filename = task.FileName.Substring(task.FileName.LastIndexOf("/") + 1);
                TaskProgress(tasks.Count, count, filename);
            }
            System.Threading.Thread.Sleep(100);
        }
    }

    public void UploadLogFile(string fileName, string content)
    {
        var param = string.Format("filename={0}&content={1}", fileName, content);
        var bs = System.Text.Encoding.UTF8.GetBytes(param);
        var req = HttpWebRequest.Create(SystemConfig.GetCfgInfoUrl(LogPath));
        req.Method = "post";
        req.ContentType = "application/x-www-form-urlencoded";
        req.ContentLength = bs.Length;

        using (Stream reqStream = req.GetRequestStream())
        {
            reqStream.Write(bs, 0, bs.Length);
        }
    }

    public bool AsynDownLoadText(DownloadTask task)
    {
        if (!ContainKey(task.Url))
        {
            _webClient.DownloadStringAsync(new Uri(task.Url), task.Url);
            return true;
        }
        return false;
    }

    //开始、继续、从新下载
    public void CheckDownLoadList()
    {
        LoggerHelper.Debug("核对下载列表");
        if (tasks.Count == 0) return;
        var finishedCount = 0;//已经完成的数目
        foreach (var task in tasks)
        {
            LoggerHelper.Debug("核对任务：" + task.FileName);
            if (task.bFineshed && !task.bDownloadAgain)
            {
                LoggerHelper.Debug("已经完成不用从下：" + task.FileName);
                finishedCount++;
            }
            else
            {
                //判断下载文件的文件夹是否存在
                var dirName = Path.GetDirectoryName(task.FileName);
                if (dirName != null && !Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                    LoggerHelper.Debug("下载目录不存在，创建目录：" + dirName);
                }
                //重新下载的，或者新的下载
                if (!BreakPoint)
                {
                    LoggerHelper.Debug("新的下载或从新下载：" + task.FileName);
                    _webClient.DownloadFileAsync(new Uri(task.Url), task.FileName, task.Url);
                }
                else
                {
                    LoggerHelper.Debug("断点下载：" + task.FileName);
                    //Action action = () => DownloadFileBreakPoint(task.Url, task.FileName);
                    //var t = new Thread(action);
                    var download = new ThreadDownloadBreakPoint(this, task);
                    var t = new Thread(download.Download);
                    t.Start();
                    LoggerHelper.Debug("开始断点下载：" + task.FileName);
                }
                break;
            }
        }
        //Action actionFinish = () =>
        //{
        //如果全部完成,修改原来等于判断，不然最后一个或者只有一个下载任务时不会执行自己的finish
        if (finishedCount > tasks.Count - 1)
        {
            LoggerHelper.Debug("下载的数据包数量已经达到要求的,删除所有任务和全部任务完成的回调");
            tasks.Clear();
            tasks = null;
            if (AllDownloadFinished != null)
            {
                AllDownloadFinished();
                AllDownloadFinished = null;
            }
        }
        //};
        //actionFinish.Invoke();
    }

    public string DownLoadText(String url)
    {
        try
        {
            return _webClient.DownloadString(url);
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
            return String.Empty;
        }
    }

    public void AsynDownLoadText(String url, Action<String> asynResult, Action OnError)
    {
        var u = url;
        Action action = () =>
        {
            var result = DownLoadText(u);
            if (String.IsNullOrEmpty(result))
            {
                if (OnError != null)
                    DriverLib.Invoke(OnError);
            }
            else
            {
                if (asynResult != null)
                    DriverLib.Invoke(() => { asynResult(result); });
            }
        };
        action.BeginInvoke(null, null);
    }

    public bool DownloadFile(String url, String localPath, String bakPath)
    {
        if (File.Exists(bakPath))
            File.Delete(bakPath);
        if (File.Exists(localPath))
            File.Move(localPath, bakPath);
        var path = Utils.GetDirectoryName(localPath);
        LoggerHelper.Debug("path: " + localPath);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        try
        {
#if UNITY_IPHONE
            DownloadManager.Singleton.SyncDownloadFileAndSaveAs(url,localPath);
#else
            _webClient.DownloadFile(url, localPath);
#endif
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Except(ex);
            if (File.Exists(bakPath))
            {
                if (File.Exists(localPath))
                    File.Delete(localPath);
                File.Move(bakPath, localPath);
            }
            return false;
        }
        finally
        {
            if (File.Exists(bakPath))
                File.Delete(bakPath);
        }
    }

    public void Dispose()
    {
        _webClient.CancelAsync();
        _webClient.Dispose();
    }
}