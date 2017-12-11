using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Text;


public class DownloadManager:MonoBehaviour,IDownloadManager
{
    static IDownloadManager m_DownloadManager=null;
    public static IDownloadManager Singleton
    {
        get
        {
            if (null == m_DownloadManager)
            {
                m_DownloadManager = new DownloadManager();
                GameObject.Find("Driver").AddComponent<DownloadObserver>();
            }
            return m_DownloadManager;
        }
    }
#if UNITY_IPHONE
    [DllImport("__Internal")]
    extern static void SyncDownload(byte [] url,byte [] Callback);
    [DllImport("__Internal")]
    extern static void SyncDownloadFileAndSaveAs(byte[] url, byte[] localPath);
    [DllImport("__Internal")]
    extern static void AsyncDownload(byte[] url);
    [DllImport("__Internal")]
    extern static void AsyncDownloadFileAndSaveAs(byte[] url, byte[] localPath);
#endif
    String m_Temp = String.Empty;
    public String Buffer
    {
        set
        {
            m_Temp = value;
        }
    }

    IEnumerator DownloadString(String url, System.Action<String> callback)
    {
        WWW www = new WWW(url);
        yield return www;
        callback(www.text);
    }
    public void SyncDownload(String url,System.Action<String> callback)
    {
        GameObject.Find("Driver").GetComponent<DownloadObserver>().StartCoroutine(DownloadString(url, callback));
#if UNITY_IPHONE
        //SyncDownload(Encoding.Default.GetBytes(url),Encoding.Default.GetBytes("SyncDownloadCallBack"));
#else
#endif
    }

    public void SyncDownloadFileAndSaveAs(String url, String localPath)
    {
#if UNITY_IPHONE
        SyncDownloadFileAndSaveAs(Encoding.Default.GetBytes(url), Encoding.Default.GetBytes(localPath));
#else

#endif
    }

    public void AsyncDownload(String url,System.Action callback)
    {
        
    }

    public void AsyncDownloadFileAndSaveAs(String url, String localPath)
    {
#if UNITY_IPHONE
        AsyncDownloadFileAndSaveAs(Encoding.Default.GetBytes(url), Encoding.Default.GetBytes(localPath));
#else

#endif
    }
}
