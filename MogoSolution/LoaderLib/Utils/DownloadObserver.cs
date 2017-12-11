using System;
using UnityEngine;
using System.Collections.Generic;


class DownloadObserver:MonoBehaviour
{
    void SyncDownloadCallBack(String data)
    {
        DownloadManager.Singleton.Buffer = data;
    }

    void AsyncDownloadCallBack()
    {
        
    }
}
