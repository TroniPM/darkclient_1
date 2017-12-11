using UnityEngine;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;
using System.Linq;
using System.Text;

public class SubAssetCacheMgr
{
    #region Gear

    private static HashSet<string> m_loadedGearResources = new HashSet<string>();

    public static void GetGearInstance(string resourceName, Action<String, int, Object> loaded)
    {
        AssetCacheMgr.GetNoCacheInstance(resourceName, (resource, guid, go) =>
        {
            m_loadedGearResources.Add(resourceName);
            if (loaded != null)
                loaded(resource, guid, go);
        });
    }

    public static void GetGearResrouce(string resourceName, Action<Object> loaded)
    {
        AssetCacheMgr.GetNoCacheResource(resourceName, (obj) =>
        {
            m_loadedGearResources.Add(resourceName);
            if (loaded != null)
                loaded(obj);
        });
    }

    public static void ReleaseGearResources()
    {
        foreach (var item in m_loadedGearResources)
        {
            AssetCacheMgr.ReleaseResourceImmediate(item);
        }
    }

    #endregion

    #region Character

    private static HashSet<String> m_loadedCharacterResources = new HashSet<string>();

    public static void GetPlayerInstance(string resourceName, Action<String, int, Object> loaded)
    {
        AssetCacheMgr.GetInstance(resourceName, loaded);
    }

    /// <summary>
    /// 获取资源实例。
    /// </summary>
    /// <param name="resourceName">资源文件名（不带路径，带后缀）</param>
    /// <param name="loaded">资源实例加载完成回调</param>
    public static void GetCharacterInstance(string resourceName, Action<String, int, Object> loaded)
    {
        AssetCacheMgr.GetNoCacheInstance(resourceName, (pref, guid, go) =>
        {
            m_loadedCharacterResources.Add(resourceName);
            if (loaded != null)
                loaded(pref, guid, go);
        });
    }

    public static void GetCharacterResourcesAutoRelease(string[] resourcesName, Action<Object[]> loaded, Action<float> progress = null)
    {
        foreach (var item in resourcesName)
        {
            m_loadedCharacterResources.Add(item);
        }
        AssetCacheMgr.GetResourcesAutoRelease(resourcesName, loaded, progress);
    }

    public static void ReleaseCharacterResources()
    {
        foreach (var item in m_loadedCharacterResources)
        {
            AssetCacheMgr.ReleaseResourceImmediate(item);
        }
    }

    #endregion
}