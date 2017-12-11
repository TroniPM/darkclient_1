#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：LoadAssetBundles
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：2013.3.18
// 模块描述：资源加载管理类。
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;

using UnityEngine;
using Mogo.Util;
using Object = UnityEngine.Object;
using Mogo.Util;

/// <summary>
/// 资源加载管理类。
/// </summary>
public class LoadAssetBundlesMainAsset : MonoBehaviour, ILoadAsset
{
    //private Action<String, int, Object> Loaded;
    private Dictionary<string, ResourceInfo> m_localResourcesDic = new Dictionary<string, ResourceInfo>();
    public Dictionary<String, String> m_filesDic = new Dictionary<string, string>();

    //private bool isLoading = false;
    private Dictionary<Queue<System.Action>, bool> m_isLoading = new Dictionary<Queue<Action>, bool>();
    private Queue<System.Action> m_queue = new Queue<System.Action>();
    private Queue<System.Action> m_secondQueue = new Queue<System.Action>();

    void Awake()
    {
        DontDestroyOnLoad(this);
        AssetCacheMgr.AssetMgr = this;
        m_isLoading.Add(m_queue, false);
        m_isLoading.Add(m_secondQueue, false);
    }

    public void SetPathMap()
    {
        m_filesDic = ResourceManager.GetResourcePathMap();
        //LoggerHelper.Warning(m_filesDic.PackMap(mapSpriter: '\n'));
    }

    public void LoadInstance(string prefab, Action<string, int, Object> loaded)
    {
        LoadInstance(prefab, loaded, null);
    }

    public void LoadInstance(string prefab, Action<string, int, Object> loaded, Action<float> progress)
    {
        LoadAsset(prefab, (obj) =>
        {
            Object go = null;
            int guid = -1;
            if (obj)
            {
                go = GameObject.Instantiate(obj);
                guid = go.GetInstanceID();
            }
            if (loaded != null)
                loaded(prefab, guid, go);
        }, progress);
    }

    public void LoadSceneInstance(string prefab, Action<string, int, Object> loaded, Action<float> progress)
    {
        LoadInstance(prefab, loaded, progress);
    }

    public void LoadAsset(string prefab, Action<Object> loaded)
    {
        LoadAsset(prefab, loaded, null);
    }

    public void LoadAsset(string prefab, Action<Object> loaded, Action<float> progress)
    {
        var path = m_filesDic.Get(prefab);
        //LoggerHelper.Debug(path);
        LoadAssets(path, m_queue, ResourceManager.LoadResource, loaded, (resource) =>
        {
            if (progress != null && resource != null)
                StartCoroutine(ShowProgress(resource, progress));
        });
        //StartCoroutine(WaitAndDo(path, loaded));
    }

    public void LoadUIAsset(string prefab, Action<Object> loaded, Action<float> progress)
    {
        var path = m_filesDic.Get(prefab);
        //LoggerHelper.Debug(path);
        LoadAssets(path, m_queue, ResourceManager.LoadUIResource, loaded, (resource) =>
        {
            if (progress != null && resource != null)
                StartCoroutine(ShowProgress(resource, progress));
        });
        //StartCoroutine(WaitAndDo(path, loaded));
    }

    public void SecondLoadAsset(string prefab, Action<Object> loaded, Action<float> progress)
    {
        var path = m_filesDic.Get(prefab);
        //LoggerHelper.Debug(path);
        LoadAssets(path, m_secondQueue, ResourceManager.LoadResource, loaded, (resource) =>
        {
            if (progress != null && resource != null)
                StartCoroutine(ShowProgress(resource, progress));
        });
        //StartCoroutine(WaitAndDo(path, loaded));
    }

    public void SecondLoadUIAsset(string prefab, Action<Object> loaded, Action<float> progress)
    {
        var path = m_filesDic.Get(prefab);
        //LoggerHelper.Debug(path);
        LoadAssets(path, m_secondQueue, ResourceManager.LoadUIResource, loaded, (resource) =>
        {
            if (progress != null && resource != null)
                StartCoroutine(ShowProgress(resource, progress));
        });
        //StartCoroutine(WaitAndDo(path, loaded));
    }

    public void Release(string prefab)
    {
        if (String.IsNullOrEmpty(prefab))
            return;
        var path = m_filesDic.Get(prefab);
        ResourceManager.ReleaseResource(path);
    }

    public void LoadSceneAsset(string prefab, Action<Object> loaded)
    {
        LoadAsset(prefab, loaded);
    }

    public Object LoadLocalInstance(string prefab)
    {
        return Instantiate(LoadLocalAsset(prefab));
    }

    public Object LoadLocalAsset(string prefab)
    {
        ResourceInfo resourceInfo;
        var flag = m_localResourcesDic.TryGetValue(prefab, out resourceInfo);
        if (!flag)
        {
            resourceInfo = new ResourceInfo();
            resourceInfo.Path = prefab;
            m_localResourcesDic.Add(prefab, resourceInfo);
        }

        resourceInfo.ReferenceCount++;

        return resourceInfo.GameObject;
    }

    public void ReleaseLocalAsset(string prefab)
    {
        if (String.IsNullOrEmpty(prefab))
            return;
        ResourceInfo resourceInfo;
        var flag = m_localResourcesDic.TryGetValue(prefab, out resourceInfo);
        if (flag)
        {
            resourceInfo.ReferenceCount--;
        }
    }

    public void Release(string prefab, bool releaseAsset)
    {
        if (String.IsNullOrEmpty(prefab))
            return;
        var path = m_filesDic.Get(prefab);
        ResourceManager.ReleaseResource(path, releaseAsset);
    }

    public void ReleaseUnusedAssets()
    {
        //List<string> toRemove=new List<string>();
        //foreach(KeyValuePair<string,ResourceInfo> ri in m_localResourcesDic)
        //{
        //	if(ri.Value.ReferenceCount==1)
        //	{
        //		toRemove.Add(ri.Key);
        //	}
        //}
        //
        //foreach( string str in toRemove)
        //{
        //	m_localResourcesDic.Remove(str);
        //}
    }

    public void ClearLoadAssetTasks()
    {
        m_queue.Clear();
    }

    public void ForceClear()
    {
        //empty for www
    }

    public Object SynLoadInstance(string prefab)
    {
        var obj = SynLoadAsset(prefab);
        if (obj)
            return GameObject.Instantiate(obj);
        else
            return null;
    }

    public Object SynLoadAsset(string prefab)
    {
        var path = m_filesDic.Get(prefab);
        if (String.IsNullOrEmpty(path))
        {
            LoggerHelper.Error("LoadAssets null path.");
            return null;
        }
        var resource = ResourceManager.GetResource(path);
        //ResourceManager.AddReferenceCount(resource);
        if (resource.Object != null)
            return resource.Object;
        else
            return null;
    }

    public void UnloadAsset(string prefab)
    {
        ResourceManager.UnloadAsset(prefab);
    }

    private IEnumerator ShowProgress(Resource resource, Action<float> progress)
    {
        while (resource.IsLoading)
        {
            progress(resource.Progress);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 加载资源。
    /// </summary>
    /// <param name="path"></param>
    /// <param name="loaded"></param>
    private void LoadAssets(string path, Queue<Action> q, Action<Resource, MonoBehaviour, Action> loadResource, Action<Object> loaded, Action<Resource> progress = null)
    {
        if (String.IsNullOrEmpty(path))
        {
            LoggerHelper.Error("LoadAssets null path.");
            if (loaded != null)
                loaded(null);
            return;
        }
        var resource = ResourceManager.GetResource(path);
        ResourceManager.AddReferenceCount(resource);
        if (resource.Object != null)//尝试Asset不为空直接返回，逻辑上与下面代码逻辑重复，确认方案后再看如何优化（其实不优化也可以啦）
        {
            LoggerHelper.Warning("Load loaded asset: " + path);
            if (loaded != null)
                loaded(resource.Object);
            return;
        }

        if (resource.IsDone)//尝试将已经加载完成的提前，以下的代码为重复代码，如果此功能有效再考虑封装
        {
            if (loaded != null)
            {
                if (resource.Object == null)
                {
                    if (!resource.RelativePath.EndsWith(".unity"))
                        LoggerHelper.Error(string.Concat("Can not load: ", resource.RelativePath));
                    loaded(null);
                }
                else
                    loaded(resource.Object);
            }
            return;
        }
        if (m_isLoading[q])
        {
            q.Enqueue(() => WaitAndDo(resource, q, loadResource, loaded, progress));
        }
        else
        {
            m_isLoading[q] = true;
            //Mogo.Util.LoggerHelper.Debug("isLoading true");
            WaitAndDo(resource, q, loadResource, loaded, progress);
        }
    }

    /// <summary>
    /// 调用加载资源等待完成。
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="loaded"></param>
    /// <returns></returns>
    private void WaitAndDo(Resource resource, Queue<Action> q, Action<Resource, MonoBehaviour, Action> loadResource, Action<Object> loaded, Action<Resource> progress = null)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Action act = () =>
        {
            resource.IsLoading = false;
            sw.Stop();
            //Mogo.Util.LoggerHelper.Debug("LoadResource: " + sw.ElapsedMilliseconds);
            try
            {
                if (loaded != null)
                {
                    if (resource.Object == null)
                    {
                        if (!resource.RelativePath.EndsWith(".unity"))
                            LoggerHelper.Error(string.Concat("Can not load: ", resource.RelativePath));
                        loaded(null);
                    }
                    else
                    {
                        loaded(resource.Object);
                        //foreach (var item in ResourceManager.GetResourcesInfo(resource))
                        //{
                        //    item.Release(false);
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Except(ex);
            }

            if (q.Count != 0)
            {
                var action = q.Dequeue();
                action();
            }
            else
            {
                m_isLoading[q] = false;
                //Mogo.Util.LoggerHelper.Debug("isLoading false");
            }
        };

        resource.IsLoading = true;
        loadResource(resource, this, act);
        if (progress != null)
            progress(resource);
        //StartCoroutine(ResourceManager.LoadResource(resource, act));
        //yield return StartCoroutine(res.Wait());
        //act();
    }
}