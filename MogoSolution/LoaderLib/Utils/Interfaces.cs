using System;

public interface IDownloadManager
{
    //Sync download file and return the contents of the file
    void    SyncDownload(String url,System.Action<String> callback);
    //Sync download file and save it to localpath
    void    SyncDownloadFileAndSaveAs(String url, String localPath);
    //Async download file and return the contents of the file
    void    AsyncDownload(String url,System.Action callback);
    //Async download file and save it to localpath
    void    AsyncDownloadFileAndSaveAs(String url, String localPath);
    String Buffer { set; }
}