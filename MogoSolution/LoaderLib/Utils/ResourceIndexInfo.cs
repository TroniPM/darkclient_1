using System.Security;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mogo.Util;

public class ResourceIndexInfo
{
    //key:filename ==>value:fullpath
    Dictionary<string, string> m_ResourceIndexes = new Dictionary<string, string>();
    //meta缓存列表
    public List<string> MetaList = new List<string>();
    IEnumerator ReadTxtFile(string strResourceIndexFile, Action finished, Action fail = null)
    {
        #region 读取meta到列表
        if(!SystemSwitch.UseFileSystem)
        {
            LoggerHelper.Debug("--------------------CacheMetaList----------------------");
            MetaList.Clear();
            string metaindexfile = Application.streamingAssetsPath + "/Meta.xml";
            WWW metawww = null;
            metawww = Application.platform == RuntimePlatform.Android
                ? new WWW(metaindexfile)
                : new WWW(SystemConfig.ASSET_FILE_HEAD + metaindexfile);
            if (null != metawww)
            {
                yield return metawww;
                MetaList.Add("Meta.xml");
                if (!String.IsNullOrEmpty(metawww.text))
                {
                    var children = XMLParser.LoadXML(metawww.text);
                    foreach (SecurityElement item in children.Children)
                    {
                        string path = item.Attribute("path");
                        if (path != null)
                            MetaList.Add(path);
                        else
                            LoggerHelper.Error("Path not exit in Meta.xml");
                    }
                }
                else
                {
                    LoggerHelper.Debug("Meta.xml not exit in StreamingAssets");
                }
            }
        }

        #endregion

        WWW www = null;
        try
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                www = new WWW(strResourceIndexFile);
            }
            else
            {
                www = new WWW(SystemConfig.ASSET_FILE_HEAD + strResourceIndexFile);
            }
        }
        catch (Exception e)
        {
            www = null;
            LoggerHelper.Debug(e + " this message is not harmless,this means the APK is not the most first APK of our game");
        }

        if (null != www)
        {
            yield return www;

            string[] szStr = www.text.Split('\n');

            int nPos = -1;
            for (int i = szStr.Length; i > 0; i--)
            {
                nPos = szStr[i - 1].LastIndexOf("/");
                if (nPos <= 0)
                {
                    if (szStr[i - 1].EndsWith("xml"))
                    {
                        if (!m_ResourceIndexes.ContainsKey(szStr[i - 1]))
                        {
                            m_ResourceIndexes.Add(szStr[i - 1], szStr[i - 1]);
                        }
                    }
                }
                else
                {
                    if (!szStr[i - 1].EndsWith("xml"))
                    {
                        if (!m_ResourceIndexes.ContainsKey(szStr[i - 1].Substring(nPos + 1, szStr[i - 1].Length - nPos - 1)))
                        {
                            m_ResourceIndexes.Add(szStr[i - 1].Substring(nPos + 1, szStr[i - 1].Length - nPos - 1), szStr[i - 1]);
                        }
                        else
                        {
                            //Mogo.Util.LoggerHelper.Debug(szStr[i - 1]);
                        }
                    }
                    else
                    {
                        if (!m_ResourceIndexes.ContainsKey(szStr[i - 1]))
                        {
                            m_ResourceIndexes.Add(szStr[i - 1], szStr[i - 1]);
                        }
                    }
                }
            }
            finished();
            www.Dispose();
        }
        else
        {
            fail();
        }
    }

    public bool Exist()
    {
        return m_ResourceIndexes.Count == 0 ? false : true;
    }

    public void Init(string strResourceIndexFile, Action finished, Action fail = null)
    {
        if (m_ResourceIndexes.Count == 0)
        {
            DriverLib.Instance.StartCoroutine(ReadTxtFile(strResourceIndexFile, () =>
            {
                if (!File.Exists(SystemConfig.VersionPath))
                {
                    var ver = Resources.Load(SystemConfig.VERSION_URL_KEY) as TextAsset;
                    if (ver != null)
                        XMLParser.SaveText(SystemConfig.VersionPath, ver.text);
                }
                finished();
            }, fail));
        }
        else
        {
            finished();
        }
    }

    public string[] GetAllFullPathes()
    {
        List<string> list = new List<string>();
        foreach (var i in m_ResourceIndexes)
        {
            list.Add(i.Value);
        }
        return list.ToArray();
    }

    public List<string> GetFirstTimeResourceFilePathes()
    {
        List<string> list = new List<string>();
        foreach (var i in m_ResourceIndexes)
        {
            if (i.Value.StartsWith("data/"))
            {
                list.Add(i.Value);
            }
        }
#if !UNITY_IPHONE
        list.Add(DriverLib.FileName);
#endif
        return list;
    }

    public string[] GetLeftFilePathes()
    {
        List<string> list = new List<string>();
        foreach (var i in m_ResourceIndexes)
        {
            if (!i.Value.StartsWith("data/")
#if !UNITY_IPHONE
                && (!i.Value.Contains(DriverLib.FileName))
#endif
                )
            {
                list.Add(i.Value);
            }
        }
        return list.ToArray();
    }


    public List<string> GetFileNamesByDirectory(string strDir)
    {
        List<string> list = new List<string>();
        foreach (KeyValuePair<string, string> i in m_ResourceIndexes)
        {
            if (i.Value.StartsWith(strDir))
            {
                list.Add(i.Value);
            }
        }
        return list;
    }
    public string GetFullPath(string strFileName)
    {
        if (m_ResourceIndexes.ContainsKey(strFileName))
        {
            return m_ResourceIndexes[strFileName];
        }
        else
        {
            return "";
        }
    }

    public bool Exsit(string strFileName)
    {
        if (m_ResourceIndexes.ContainsKey(strFileName))
        {
            return true;
        }
        return false;
    }

    public void Destroy()
    {
        m_ResourceIndexes.Clear();
        m_Instacne = null;
    }

    static ResourceIndexInfo m_Instacne = null;
    public static ResourceIndexInfo Instance
    {
        get
        {
            if (null == m_Instacne)
            {
                m_Instacne = new ResourceIndexInfo();
            }
            return m_Instacne;
        }
    }
}
