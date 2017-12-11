using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Eddy;
using Eddy.Extensions;
using UnityEngine;
using System.Security;
namespace Mogo.Util
{
    public static class ResourceManager
    {
        public static readonly string MetaFileName = "Meta.xml";
        public static Dictionary<string, Resource> resources = new Dictionary<string, Resource>();
        private static Dictionary<string, ResourceMetaData> metaOfResource = new Dictionary<string, ResourceMetaData>();
        private static Dictionary<string, string> metaOfMeta;

        private static HashSet<String> m_whiteList = new HashSet<String>();

        public static HashSet<String> WhiteList
        {
            get { return m_whiteList; }
            set { m_whiteList = value; }
        }

        public static readonly string LocalDataPath = SystemConfig.ResourceFolder;
        //public static readonly string ExportPath = Application.dataPath
        //    + "/../Export/0.0.0.1/ExportedFiles";
        //public static readonly string RemoteDataPath = SystemConfig.ASSET_FILE_HEAD + ExportPath;

        /// <summary>
        /// 需要直接导出的文件类型
        /// </summary>
        public static string[] ExportableFileTypes = { ".xml" };

        /// <summary>
        /// 需要打包成AssetBundle后导出的文件类型
        /// </summary>
        private static string[] m_packedExportableFileTypes = 
        { 
            ".prefab", 
            ".fbx", 
            ".controller", 
            ".png", 
            ".exr", 
            ".tga", 
            ".tif", 
            ".asset", 
            ".jpg", 
            ".jpeg", 
            ".psd", 
            ".mp3", 
            ".wav", 
            ".mat", 
            ".unity", 
            ".anim",
            ".shader",
            ".ttf"
        };

        static ResourceManager()
        {
            ResourceManager.WhiteList.Add("Meteolite.prefab");
            ResourceManager.WhiteList.Add("FontMa.mat");
            ResourceManager.WhiteList.Add("FontMsyh.prefab");
            ResourceManager.WhiteList.Add("FontMsyh2.prefab");
            ResourceManager.WhiteList.Add("FontMsyh3.prefab");
            //ResourceManager.WhiteList.Add("FZY4JW.TTF");//字体库不再通过Assetbundle加载
            ResourceManager.WhiteList.Add("TeachFontMat.mat");
            ResourceManager.WhiteList.Add("TeachFontMsyh.prefab");

            ResourceManager.WhiteList.Add("MogoBackgroundUI.prefab");
            ResourceManager.WhiteList.Add("MogoBackgroundUI.mat");
            ResourceManager.WhiteList.Add("MogoBackgroundUI.png");
            ResourceManager.WhiteList.Add("MogoButtonUI.prefab");
            ResourceManager.WhiteList.Add("MogoButtonUI.mat");
            ResourceManager.WhiteList.Add("MogoButtonUI.png");
            ResourceManager.WhiteList.Add("MogoNormalMainUI.prefab");
            ResourceManager.WhiteList.Add("MogoNormalMainUIMat.mat");
            ResourceManager.WhiteList.Add("MogoNormalMainUI.png");

            ResourceManager.WhiteList.Add("fb_dt.png");

            //ResourceManager.WhiteList.Add("101_blade.controller");
            //ResourceManager.WhiteList.Add("101_fist.controller");
            //ResourceManager.WhiteList.Add("102_fan.controller");
            //ResourceManager.WhiteList.Add("102_staff.controller");
            //ResourceManager.WhiteList.Add("103_bow.controller");
            //ResourceManager.WhiteList.Add("103_gun.controller");
            //ResourceManager.WhiteList.Add("104_dagger.controller");
            //ResourceManager.WhiteList.Add("104_twinblade.controller");
            //ResourceManager.WhiteList.Add("MogoYellowMat.mat");
            //ResourceManager.WhiteList.Add("MogoRoseRedMat.mat");
            //ResourceManager.WhiteList.Add("MogoRedMat.mat");
            //ResourceManager.WhiteList.Add("MogoPurposeMat.mat");
            //ResourceManager.WhiteList.Add("MogoOrangeMat.mat");
            //ResourceManager.WhiteList.Add("MogoLakeBlueMat.mat");
            //ResourceManager.WhiteList.Add("MogoGreenMat.mat");
            //ResourceManager.WhiteList.Add("MogoGray.mat");
            //ResourceManager.WhiteList.Add("MogoGrassGreenMat.mat");
            //ResourceManager.WhiteList.Add("MogoDragonPurposeMat.mat");
            //ResourceManager.WhiteList.Add("MogoDeepBlueMat.mat");
            //ResourceManager.WhiteList.Add("MogoBlackWhiteMat.mat");
        }

        public static string[] PackedExportableFileTypes
        {
            get { return ResourceManager.m_packedExportableFileTypes; }
            set { ResourceManager.m_packedExportableFileTypes = value; }
        }

        public static List<string> GetResourceRoots(string resourceName)
        {
            var result = new List<string>();
            foreach (var item in resources)
            {
                if (item.Value.dependencies != null && item.Value.dependencies.FirstOrDefault(t => Utils.GetFileNameWithoutExtention(t.RelativePath) == resourceName) != null)
                    result.Add(item.Key);
            }
            return result;
        }

        public static Dictionary<string, string> GetResourcePathMap()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var result = new Dictionary<string, string>();
            foreach (var item in metaOfResource)
            {
                var name = Utils.GetFileName(item.Key);
                if (result.ContainsKey(name))
                    LoggerHelper.Warning("Same resource name; " + name);
                else
                    result.Add(name, item.Key);
            }
            sw.Stop();
            LoggerHelper.Debug("metaOfResource: " + metaOfResource.Count);
            LoggerHelper.Debug("time: " + sw.ElapsedMilliseconds);
            //Mogo.Util.LoggerHelper.Debug(result.PackMap(mapSpriter: '\n'));
            return result;
        }

        public static bool IsExportable(string path)
        {
            var extension = path.Substring(path.LastIndexOf('.'));
            extension = extension.ToLower();
            return ExportableFileTypes.Contains(extension);
        }

        public static bool IsPackedExportable(string path)
        {
            var extension = path.Substring(path.LastIndexOf('.'));
            extension = extension.ToLower();
            return PackedExportableFileTypes.Contains(extension);
        }

        private static List<Resource> GetDeepDependencies(string relativePath)
        {
            var result = new List<Resource>();
            var list = GetDependencies(relativePath);
            if (list != null)
            {
                foreach (var item in list)
                {
                    result.AddRange(GetDeepDependencies(item.RelativePath));
                }
                result.AddRange(list);
            }

            return result;
        }

        private static List<Resource> GetDependencies(string relativePath)
        {
            //LoggerHelper.Debug(relativePath);
            var dependencyPaths = metaOfResource[relativePath].Dependencies;

            if (dependencyPaths == null || dependencyPaths.Count == 0)
                return null;

            var dependencies = new List<Resource>();

            foreach (var dependencyPath in dependencyPaths)
            {
                var res = GetResource(dependencyPath);
                //if (!res.IsLoading)
                dependencies.Add(res);
            }

            return dependencies;
        }

        public static string WithSuffix(string name)
        {
            if (ResourceManager.IsPackedExportable(name))
            {
                return string.Concat(name, SystemConfig.ASSET_FILE_EXTENSION);
            }
            return name;
        }

        #region LoadSync

        /// <summary>
        /// 获取资源依赖
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static List<Resource> GetResourcesInfo(Resource resource)
        {
            var list = new List<Resource>();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            list.AddRange(GetDeepDependencies(resource.RelativePath));
            list.Add(resource);
            list = list.Distinct().ToList();
            sw.Stop();
            //Mogo.Util.LoggerHelper.Debug("sw.ElapsedMilliseconds: " + sw.ElapsedMilliseconds);
            //Mogo.Util.LoggerHelper.Debug("list.Count: " + list.Count);
            return list;
        }

        public static void LoadResource(Resource resource, MonoBehaviour mono, Action loaded)
        {
            if (resource.IsDone)
            {
                if (loaded != null)
                    loaded();
                return;
            }
            var list = GetResourcesInfo(resource);

            //if (SystemSwitch.UseFileSystem)
            //    LoadCreateRequestAssets(list, mono, loaded);
            //else
            mono.StartCoroutine(LoadWWWAssets(list, loaded));
        }

        public static void LoadUIResource(Resource resource, MonoBehaviour mono, Action loaded)
        {
            if (resource.IsDone)
            {
                if (loaded != null)
                    loaded();
                return;
            }
            var list = GetResourcesInfo(resource);

            //if (SystemSwitch.UseFileSystem)
            //    LoadCreateRequestAssets(list, mono, loaded);
            //else
            mono.StartCoroutine(LoadUIWWWAssets(list, loaded));
        }

        private static IEnumerator LoadWWWAssets(List<Resource> list, Action loaded)
        {
            int index = 0;
            while (index < list.Count)
            {
                var res = list[index];
                index++;
                if (res.IsDone)
                {
                    yield return null;
                }
                else
                {
                    var path = WithSuffix(res.RelativePath);
                    ResourceWachter.Instance.Watch(path);
                    string url = String.Concat(SystemConfig.ResourceFolder, path);
                    if (File.Exists(url))
                    {
                        url = String.Concat(SystemConfig.ASSET_FILE_HEAD, url);
                    }
                    else
                    {
                        url = Utils.GetStreamPath(path);

                    }
                    var www = new WWW(url);
                    yield return www;
                    res.www = www;
                    if (res.www.assetBundle != null)
                    {
                        res.www.assetBundle.LoadAllAssets();
                        res.IsDone = true;
                        res.m_object = res.www.assetBundle.mainAsset;
                    }
                }
            }
            if (loaded != null)
                loaded();
        }

        private static IEnumerator LoadUIWWWAssets(List<Resource> list, Action loaded)
        {
            int index = 0;
            Dictionary<Material, Resource[]> matPngMapping = null;
            //List<Resource> unloadList = null;
            while (index < list.Count)
            {
                var res = list[index];
                index++;
                if (res.RelativePath.EndsWith("FZY4JW.TTF"))
                {
                    res.IsDone = true;
                    yield return null;
                }
                else if (res.IsDone)
                {
                    yield return null;
                }
                else
                {
                    var path = WithSuffix(res.RelativePath);
                    ResourceWachter.Instance.Watch(path);
                    string url = String.Concat(SystemConfig.ResourceFolder, path);
                    if (File.Exists(url))
                    {
                        url = String.Concat(SystemConfig.ASSET_FILE_HEAD, url);
                    }
                    else
                    {
                        url = Utils.GetStreamPath(path);

                    }
                    var www = new WWW(url);
                    yield return www;
                    res.www = www;
                    if (res.www.assetBundle != null)
                    {
                        res.www.assetBundle.LoadAllAssets();
                        res.IsDone = true;
                        res.m_object = res.www.assetBundle.mainAsset;
                    }
                    if (res.RelativePath.EndsWith(".mat") && (res.RelativePath.Contains("Resources/MogoUI/") || res.RelativePath.Contains("Resources/GUI/")))
                    {
                        //LoggerHelper.Debug("mat: " + res.RelativePath);
                        if (matPngMapping == null)
                            matPngMapping = new Dictionary<Material, Resource[]>();
                        if (res.dependencies == null)
                            LoggerHelper.Warning("null dep: " + res.RelativePath);
                        else
                        {
                            var r = res.dependencies.Where(t => t.RelativePath.EndsWith(".png")).ToArray();
                            var mat = res.m_object as Material;
                            if (r != null && mat != null)
                                matPngMapping.Add(mat, r);
                        }
                    }
                    else if (res.RelativePath.EndsWith(".png") && (res.RelativePath.Contains("Resources/Textures/")))// && list.Count != 1 //  || res.RelativePath.Contains("Resources/Images/")
                    {
                        //if (unloadList == null)
                        //    unloadList = new List<Resource>();
                        //unloadList.Add(res);
                        //LoggerHelper.Debug("png: " + res.RelativePath);
                        //res.Release(false);
                        //res.IsDone = false;
                        //res.m_neverUnloadAsset = true;
                        //res.www.assetBundle.Unload(false);
                        //res.www = null;
                        //www.Dispose();
                        res.Release(false);
                    }
                    else if (res.RelativePath.EndsWith("FontMsyh.prefab") || res.RelativePath.EndsWith("FontMsyh2.prefab")
                        || res.RelativePath.EndsWith("FontMsyh3.prefab") || res.RelativePath.EndsWith("TeachFontMsyh.prefab"))
                    {
                        //LoggerHelper.Error("font prefab: " + res.RelativePath);
                        var prefab = res.m_object as GameObject;
                        if (prefab)
                        {
                            var font = prefab.GetComponent<UIFont>();
                            if (font)
                                font.dynamicFont = Resources.Load("Font/FZY4JW") as Font;
                            else
                                LoggerHelper.Error("null font: " + res.RelativePath);
                        }
                        else
                            LoggerHelper.Error("null font prefab: " + res.RelativePath);
                    }
                }
            }
            if (matPngMapping != null)
            {
                foreach (var item in matPngMapping)
                {
                    if (item.Value != null && item.Value.Length != 0)
                    {
                        if (item.Key.shader.name.EndsWith("ETC"))
                        {
                            LoggerHelper.Warning("ETC shader: " + item.Key.name);
                        }
                        //    for (int i = 0; i < item.Value.Length; i++)
                        //    {
                        //        var tex = item.Value[i];
                        //        if (tex.RelativePath.Contains("_A"))
                        //            item.Key.SetTexture("_AlphaTex", tex.m_object as Texture);
                        //        else
                        //            item.Key.SetTexture("_MainTex", tex.m_object as Texture);
                        //    }
                        //}
                        //else
                        //{
                        for (int i = 0; i < item.Value.Length; i++)//这里很奇怪，赶脚跟打包有关系，不过先不判断一起处理先，这样可以没问题，估计效率影响不会太大
                        {
                            var tex = item.Value[i];
                            if (tex.RelativePath.Contains("_A"))
                                item.Key.SetTexture("_AlphaTex", tex.m_object as Texture);
                            else
                                item.Key.SetTexture("_MainTex", tex.m_object as Texture);
                        }
                        //item.Key.mainTexture = item.Value[0].m_object as Texture;
                        //}
                    }
                }
                matPngMapping.Clear();
            }
            if (list.Count != 0)
            {
                var prefab = list[list.Count - 1];
                if (prefab.RelativePath.EndsWith(".prefab"))
                    prefab.Release(false);
                else
                    LoggerHelper.Warning("root not prefab: " + prefab.RelativePath);
            }
            //if (unloadList != null)
            //{
            //    for (int i = 0; i < unloadList.Count; i++)
            //    {
            //        unloadList[i].Release(false);
            //    }
            //}
            if (loaded != null)
                loaded();
        }

        private static void LoadCreateRequestAssets(List<Resource> list, MonoBehaviour mono, Action loaded)
        {
            //var resources = new List<Resource>();
            //foreach (var res in list)
            //{
            //    if (res.IsDone || res.IsLoading)
            //        continue;
            //    var path = WithSuffix(res.RelativePath);
            //    var fileName = String.Concat(SystemConfig.ResourceFolder, path);
            //    res.fileData = Utils.LoadByteFile(fileName);//FileAccessManager.LoadBytes(path);
            //    if (res.fileData != null && res.fileData.Length != 0)
            //        resources.Add(res);
            //    else
            //        LoggerHelper.Error("Load file failure: " + path);
            //}
            //mono.StartCoroutine(DoLoadCreateRequestAssets(resources, loaded));
            var resources = new List<Resource>();
            foreach (var res in list)
            {
                if (res.IsDone)
                    continue;
                resources.Add(res);
            }
            Action action = () =>
            {
                var loadedResources = new List<Resource>();
                foreach (var res in resources)
                {
                    var path = WithSuffix(res.RelativePath);
                    //var fileName = String.Concat(SystemConfig.ResourceFolder, path);
                    //res.fileData = Utils.LoadByteFile(fileName);//FileAccessManager.LoadBytes(path);
                    res.fileData = FileAccessManager.LoadBytes(path);
                    if (res.fileData != null && res.fileData.Length != 0)
                        loadedResources.Add(res);
                    else
                        LoggerHelper.Error("Load file failure: " + path);
                }
                TimerHeap.AddTimer(0, 0, () => mono.StartCoroutine(DoLoadCreateRequestAssets(loadedResources, loaded)));
            };
#if UNITY_IPHONE
            action();
#else
            action.BeginInvoke(null, null);
#endif
        }

        private static IEnumerator DoLoadCreateRequestAssets(List<Resource> list, Action loaded)
        {
            var createCounter = 0;
            var yieldCount = 0;
            while (createCounter < list.Count)
            {
                var createRequests = new List<Resource>();
                //Profiler.BeginSample("RecordCreateRequests");
                for (int i = 0; i < SystemConfig.Instance.SynCreateRequestCount && createCounter < list.Count; i++)
                {
                    var res = list[createCounter];
                    createCounter++;
                    var cr = AssetBundle.LoadFromMemoryAsync(res.fileData);
                    if (cr == null)
                    {
                        LoggerHelper.Error("Create failure: " + res.RelativePath);
                        continue;
                    }
                    res.createRequest = cr;
                    createRequests.Add(res);
                    res.fileData = null;
                }
                //Profiler.EndSample();
                int index = 0;
                while (index < createRequests.Count)
                {
                    var res = list[yieldCount];
                    yieldCount++;
                    index++;
                    yield return res.createRequest;
                    //Profiler.BeginSample("RecordLoadAll");
                    if (res.createRequest != null && res.createRequest.assetBundle != null)
                    {
                        res.createRequest.assetBundle.LoadAllAssets();
                        res.IsDone = true;
                        res.m_object = res.createRequest.assetBundle.mainAsset;
                    }
                    //Profiler.EndSample();
                }
            }
            if (loaded != null)
                loaded();
        }

        public static IEnumerator LoadResource(Resource resource, Action loaded)
        {
            if (resource.IsDone)
            {
                if (loaded != null)
                    loaded();
                yield break;
            }
            var list = new List<Resource>();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            list.AddRange(GetDeepDependencies(resource.RelativePath));
            list.Add(resource);
            list = list.Distinct().ToList();
            sw.Stop();
            //Mogo.Util.LoggerHelper.Debug("sw.ElapsedMilliseconds: " + sw.ElapsedMilliseconds);
            int index = 0;
            //Mogo.Util.LoggerHelper.Debug("list.Count: " + list.Count);
            while (index < list.Count)
            {
                var res = list[index];
                index++;
                if (res.IsDone)
                {
                    yield return null;
                }
                else
                {
                    var path = WithSuffix(res.RelativePath);

                    if (SystemSwitch.UseFileSystem)
                    {
                        //var fileName = String.Concat(SystemConfig.ResourceFolder, path);
                        //var resourceData = Utils.LoadByteFile(fileName);
                        var resourceData = FileAccessManager.LoadBytes(path);
                        var cr = AssetBundle.LoadFromMemoryAsync(resourceData);
                        yield return cr;
                        res.createRequest = cr;
                        if (res.createRequest.assetBundle != null)
                            res.createRequest.assetBundle.LoadAllAssets();
                    }
                    else
                    {
                        var www = new WWW(String.Concat(SystemConfig.ASSET_FILE_HEAD, SystemConfig.ResourceFolder, path));
                        //Mogo.Util.LoggerHelper.Debug(res.RelativePath);
                        yield return www;
                        res.www = www;
                        if (res.www.assetBundle != null)
							res.www.assetBundle.LoadAllAssets();
                    }
                }
            }
            if (loaded != null)
                loaded();
        }

        public static Resource GetResource(string relativePath)
        {
            var refrence = resources.GetValueOrDefault(relativePath);

            if (refrence != null)
            {
                return refrence;
            }

            var resource = new Resource();
            resource.RelativePath = relativePath;
            resource.dependencies = GetDependencies(relativePath);
            resources[relativePath] = resource;
            return resource;
        }

        public static void AddReferenceCount(Resource res)
        {
            var resList = ResourceManager.GetResourcesInfo(res);
            foreach (var item in resList)
            {
                item.referenceCount++;
            }
        }

        #endregion
        #region LoadMeta

        public static void LoadMetaOfMeta(Action cb, Action<int, int> process)
        {
            metaOfMeta = new Dictionary<string, string>();
            //meta现已全部拷到sd卡
            //if (!File.Exists(Path.Combine(SystemConfig.ResourceFolder, ResourceManager.MetaFileName)))
            //{
            //    Driver.Instance.StartCoroutine(Driver.Instance.LoadWWWText(Utils.GetStreamPath(ResourceManager.MetaFileName),
            //        (xmlText) =>
            //        {
            //            var xml = XMLParser.LoadXML(xmlText);
            //            if (xml == null)
            //            {
            //                if (cb != null)
            //                {
            //                    cb();
            //                }
            //                return;
            //            }
            //            foreach (SecurityElement item in xml.Children)
            //            {
            //                metaOfMeta[item.Attribute("path")] = item.Attribute("md5");
            //            }

            //            RecursiveLoadMetaOfResource(metaOfMeta.Keys.ToList(), 0, metaOfMeta.Keys.Count, () =>
            //            {
            //                AssetCacheMgr.AssetMgr.SetPathMap();
            //                if (cb != null)
            //                {
            //                    cb();
            //                }
            //            }, process);
            //        }));
            //}
            //else
            //{

            Action action = () =>
            {
                var xml = XMLParser.LoadXML(FileAccessManager.LoadText(ResourceManager.MetaFileName));
                if (xml == null)
                {
                    if (cb != null)
                        Driver.Invoke(cb);
                    return;
                }
                for (int i = 0; i < xml.Children.Count; i++)
                {
                    SecurityElement item = xml.Children[i] as SecurityElement;
                    metaOfMeta[item.Attribute("path")] = item.Attribute("md5");
                }
                RecursiveLoadMetaOfResource(metaOfMeta.Keys.ToList(), 0, metaOfMeta.Keys.Count, () =>
                {
                    AssetCacheMgr.AssetMgr.SetPathMap();
                    if (cb != null)
                        Driver.Invoke(cb);
                }, process);
            };
            action.BeginInvoke(null, null);
            //}
        }

        public static void RecursiveLoadMetaOfResource(List<string> path, int index, int count, Action cb, Action<int, int> process)
        {
            if (index < count)
            {
                LoadMetaOfResource(path[index],
                    () =>
                    {
                        if (process != null)
                            process(index, count);
                        RecursiveLoadMetaOfResource(path, ++index, count, cb, process);
                        //System.Threading.Thread.Sleep(100);
                    });
            }
            else
            {
                if (cb != null)
                {
                    cb();
                }
            }
        }

        public static void LoadMetaOfResource(string path, Action cb = null)
        {
            //meta现已全部拷到sd卡
            //if (!File.Exists(Path.Combine(SystemConfig.ResourceFolder, path)))
            //{
            //    Driver.Instance.StartCoroutine(Driver.Instance.LoadWWWText(Utils.GetStreamPath(path),
            //        (xmlText) =>
            //        {
            //            var xml = XMLParser.LoadXML(xmlText);
            //            if (xml == null)
            //            {
            //                if (cb != null)
            //                {
            //                    cb();
            //                }
            //                return;
            //            }
            //            foreach (SecurityElement item in xml.Children)
            //            {
            //                var meta = new ResourceMetaData();
            //                meta.RelativePath = item.Attribute("path");
            //                meta.MD5 = item.Attribute("md5");

            //                var dependencies = item.Children;
            //                if (dependencies != null && dependencies.Count > 0)
            //                {
            //                    meta.Dependencies = new List<string>();

            //                    foreach (SecurityElement dependency in dependencies)
            //                    {
            //                        meta.Dependencies.Add(dependency.Attribute("path"));
            //                    }
            //                }
            //                metaOfResource[meta.RelativePath] = meta;
            //            }
            //            if (cb != null)
            //            {
            //                cb();
            //            }
            //        }));
            //}
            //else
            //{
            var xml = XMLParser.LoadXML(FileAccessManager.LoadText(path));
            if (xml == null)
            {
                if (cb != null)
                {
                    cb();
                }
                return;
            }
            for (int i = 0; i < xml.Children.Count; i++)
            {
                SecurityElement item = xml.Children[i] as SecurityElement;
                var meta = new ResourceMetaData();
                meta.RelativePath = item.Attribute("path");
                meta.MD5 = item.Attribute("md5");

                var dependencies = item.Children;
                if (dependencies != null && dependencies.Count > 0)
                {
                    meta.Dependencies = new List<string>();

                    foreach (SecurityElement dependency in dependencies)
                    {
                        meta.Dependencies.Add(dependency.Attribute("path"));
                    }
                }
                metaOfResource[meta.RelativePath] = meta;
            }
            if (cb != null)
            {
                cb();
            }
            //}
        }

        #endregion
        #region Release

        public static void UnloadUnusedAssets()//暂时无用
        {
            var allRes = resources.Values.ToArray();
            for (int i = 0; i < allRes.Length; i++)
            {
                var res = allRes[i];
                if (res.referenceCount == 0 && res.m_object)
                {
                    res.Release(true);
                }
            }
        }
        public static void ClearAll()
        {
            foreach (var item in resources)
            {
                ReleaseResource(item.Value, false);
            }
        }
        public static void ForceRelease(string relativePath)
        {
            if (SystemSwitch.ReleaseMode && SystemSwitch.DestroyResource)
            {
                if (String.IsNullOrEmpty(relativePath))
                {
                    LoggerHelper.Error("ReleaseResource null path.");
                    return;
                }
                var refrence = resources.GetValueOrDefault(relativePath);

                if (refrence != null)
                {
                    var list = GetResourcesInfo(refrence);
                    list.Reverse();
                    foreach (var item in list)
                    {
                        ReleaseResource(item, true);
                    }
                }
            }
        }
        public static void UnloadAsset(string relativePath)
        {
            if (String.IsNullOrEmpty(relativePath))
            {
                LoggerHelper.Error("ReleaseResource null path.");
                return;
            }
            var refrence = resources.GetValueOrDefault(relativePath);

            if (refrence != null)
            {
                //if (refrence.referenceCount <= 0)
                //    return;
                //LoggerHelper.Error("Unload true: " + refrence.RelativePath);
                var list = GetResourcesInfo(refrence);
                list.Reverse();
                foreach (var item in list)
                {
                    item.UnloadAsset();
                }
            }
            else
            {
                //Debug.LogError("Resource to free was not in the Dictionary:" + relativePath);
            }
        }
        public static void ReleaseResource(string relativePath)
        {
            if (String.IsNullOrEmpty(relativePath))
            {
                LoggerHelper.Error("ReleaseResource null path.");
                return;
            }
            var refrence = resources.GetValueOrDefault(relativePath);

            if (refrence != null)
            {
                //if (refrence.referenceCount <= 0)
                //    return;
                //LoggerHelper.Error("Unload true: " + refrence.RelativePath);
                var list = GetResourcesInfo(refrence);
                list.Reverse();
                //Debug.Log(list.PackList('\n'));
                foreach (var item in list)
                {
                    item.referenceCount--;
                    if (item.referenceCount <= 0)
                    {
                        ReleaseResource(item, true);
                    }
                    else
                        LoggerHelper.Debug("Not 0: " + item.RelativePath);
                }
                //Debug.Log(list.PackList('\n'));
            }
            else
            {
                //Debug.LogError("Resource to free was not in the Dictionary:" + relativePath);
            }
        }
        public static void ReleaseResource(string relativePath, bool releaseAsset)
        {
            if (String.IsNullOrEmpty(relativePath))
            {
                LoggerHelper.Error("ReleaseResource null path.");
                return;
            }
            var refrence = resources.GetValueOrDefault(relativePath);

            if (refrence != null)
            {
                //if (refrence.referenceCount <= 0)
                //    return;
                //LoggerHelper.Error("Unload releaseAsset(" + releaseAsset + "): " + refrence.RelativePath);
                var list = GetResourcesInfo(refrence);
                list.Reverse();
                foreach (var item in list)
                {
                    ReleaseResource(item, releaseAsset);
                }
            }
        }
        private static void ReleaseResource(Resource resource, bool releaseAsset)
        {
            if (resource.RelativePath.EndsWith(".shader", StringComparison.InvariantCultureIgnoreCase))
                return;
            if (m_whiteList.Contains(Utils.GetFileName(resource.RelativePath)))
                return;
            resource.Release(releaseAsset);
            resource.IsDone = false;
            resource.referenceCount = 0;
        }

        #endregion

        #region bak

        //public static void Release(Resource resource)
        //{
        //    if (resource == null || resource.dependencies == null)
        //        return;
        //    foreach (var item in resource.dependencies)
        //    {
        //        Release(item);
        //    }
        //    if (resource.MainAsset != null)
        //        resource.MainAsset.Unload(true);
        //}
        //public static string GetExportFileName(string assetPath)
        //{
        //    var bundlePath = assetPath.Replace('/', '-');
        //    return bundlePath;
        //}
        #region LoadMetaOfResourceAsync
        //private static string GetMetaFileName(string relativePath)
        //{
        //    //var directory = relativePath.Substring(0, relativePath.LastIndexOf('/'));
        //    return (Path.GetDirectoryName(relativePath) + '/' + ResourceManager.MetaFileName);
        //}
        //private static IEnumerator LoadMetaOfResourceAsync(string metaPath)
        //{
        //    //if (metaOfResource.ContainsKey(relativePath))
        //    //    yield break;
        //    //var metaPath = GetMetaFileName(relativePath);
        //    if (!metaOfMeta.ContainsKey(metaPath))
        //        yield break;
        //    var localPath = LocalDataPath + "/" + metaPath;
        //    if (!Directory.Exists(Path.GetDirectoryName(localPath)))
        //        Directory.CreateDirectory(Path.GetDirectoryName(localPath));
        //    var file = new FileStream(localPath,
        //        FileMode.OpenOrCreate, FileAccess.ReadWrite);

        //    var md5 = MD5Hash.Get(file);
        //    file.Position = 0;

        //    var document = new XmlDocument();
        //    if (md5 == metaOfMeta[metaPath])
        //    {
        //        document.Load(file);
        //    }
        //    else
        //    {
        //        var www = new WWW(RemoteDataPath + "/" + metaPath);
        //        yield return www;
        //        document.LoadXml(www.text);
        //        file.SetLength(0);
        //        file.Write(www.bytes, 0, www.bytes.Length);
        //    }

        //    file.Close();

        //    LoadMetaOfResource(document);
        //}

        //private static void LoadMetaOfResource(XmlDocument document)
        //{
        //    var files = document.SelectNodes("/root/file");
        //    foreach (var file in files)
        //    {
        //        var element = file as XmlElement;
        //        var meta = new ResourceMetaData();
        //        meta.RelativePath = element.GetAttribute("path");
        //        meta.MD5 = element.GetAttribute("md5");

        //        var dependencies = element.SelectNodes("dependency");

        //        if (dependencies != null && dependencies.Count > 0)
        //            meta.Dependencies = new List<string>();

        //        foreach (var dependency in dependencies)
        //        {
        //            meta.Dependencies.Add((dependency as XmlElement).GetAttribute("path"));
        //        }
        //        metaOfResource[meta.RelativePath] = meta;
        //    }
        //}
        #endregion
        #region LoadMetaOfMetaAsync
        //public static IEnumerator LoadMetaOfMetaAsync()
        //{
        //    if (metaOfMeta != null)
        //        yield break;

        //    WWW www = new WWW(RemoteDataPath + "/Meta.xml");
        //    yield return www;

        //    if (!Directory.Exists(LocalDataPath))
        //        Directory.CreateDirectory(LocalDataPath);
        //    FileStream file = new FileStream(LocalDataPath + "/Meta.xml",
        //        FileMode.Create,
        //        FileAccess.Write);
        //    file.Write(www.bytes, 0, www.bytes.Length);
        //    file.Close();

        //    XmlDocument document = new XmlDocument();
        //    document.LoadXml(www.text);

        //    LoadMetaOfMeta(document);
        //}

        //private static void LoadMetaOfMeta(XmlDocument document)
        //{
        //    var nodes = document.SelectNodes("/root/file");

        //    if (nodes == null)
        //        return;

        //    if (nodes.Count == 0)
        //        return;

        //    metaOfMeta = new Dictionary<string, string>();
        //    foreach (var node in nodes)
        //    {
        //        var element = node as XmlElement;
        //        metaOfMeta[element.GetAttribute("path")] = element.GetAttribute("md5");
        //    }
        //}
        #endregion
        //private static IEnumerator LoadMetaAsync(Resource resource)
        //{
        //    yield return LoadMetaOfMetaAsync();
        //    yield return LoadMetaOfResourceAsync(resource.RelativePath);
        //}
        //internal static IEnumerator LoadAsync(Resource resource)
        //{
        //    if (resource.IsLoading || resource.IsDone)
        //        yield break;

        //    resource.IsLoading = true;

        //    //yield return LoadMetaAsync(resource);
        //    if (!metaOfResource.ContainsKey(resource.RelativePath))
        //        yield break;
        //    resource.dependencies = GetDependencies(resource.RelativePath);
        //    var dependencies = resource.dependencies;

        //    yield return LoadDependenciesAsync(dependencies);
        //    yield return LoadWWWAsync(resource);

        //    //LoggerHelper.Debug(resource.RelativePath + " is loaded.");
        //}
        //private static IEnumerator LoadWWWAsync(Resource resource)
        //{
        //    //var exportFileName = GetExportFileName(resource.RelativePath);
        //    var www = new WWW("file://" + LocalDataPath + "/" + WithSuffix(resource.RelativePath));
        //    //Mogo.Util.LoggerHelper.Debug(resource.RelativePath);
        //    yield return www;

        //    resource.www = www;
        //    if (resource.www.assetBundle != null)
        //        resource.www.assetBundle.LoadAll();

        //    //if (www.error == null)
        //    //{
        //    //    var md5 = MD5Hash.Get(www.bytes);
        //    //    var meta = metaOfResource[resource.RelativePath];
        //    //    if (md5 == meta.MD5)
        //    //    {
        //    //        resource.www = www;
        //    //        yield break;
        //    //    }
        //    //}

        //    //www = new WWW(RemoteDataPath + "/" + WithSuffix(resource.RelativePath));
        //    //yield return www;
        //    //resource.www = www;

        //    //var file = new FileStream(LocalDataPath + "/" + WithSuffix(resource.RelativePath),
        //    //    FileMode.Create,
        //    //    FileAccess.Write);
        //    //file.Write(www.bytes, 0, www.bytes.Length);
        //    //file.Close();
        //}
        //private static IEnumerator LoadDependenciesAsync(List<Resource> dependencies)
        //{
        //    if (dependencies != null)
        //    {
        //        foreach (var dependency in dependencies)
        //        {
        //            yield return dependency.Wait();
        //        }
        //    }
        //}

        #endregion
    }
    //添加一个对资源加载进行监控的类，不同的场景加载的资源会打印到不同的文件中,在时间游戏中可以关闭这个功能
    public class ResourceWachter
    {
        private static ResourceWachter instance = new ResourceWachter();
        public static ResourceWachter Instance
        {
            get { return instance; }
        }
        //是否开启监控
        public bool Enable = false;
        //场景的id
        private int sceneId = -1;
        //老场景id
        private int oldSceneId = -1;
        public int SceneID
        {
            get { return sceneId; }
            set
            {
                //切换场景，保存老场景的资源列表，开启新的监控
                oldSceneId = sceneId;
                //保存老场景所使用文件
                Print(oldSceneId);
                //恢复资源列表
                ResourcePathList.Clear();
                sceneId = value;
            }
        }
        //收集资源的列表名
        List<string> ResourcePathList = new List<string>();
        //添加一条新资源到列表
        public void Watch(string path)
        {
            //开启监控时才添加
            if (Enable && path != null)
            {
                path.Trim();
                if (!ResourcePathList.Contains(path))
                    ResourcePathList.Add(path);
            }
        }
        //打印资源列表到日志
        public void Print(int id)
        {
            if (!Enable) return;
            //首先确定打印的文件名，文件名=路径+"场景ID+"+Id+".txt"
            string filename = Application.dataPath + "/Watcher/" + id + ".txt";
            //文件整个内容
            StringBuilder context = new StringBuilder();
            foreach (string path in ResourcePathList)
            {
                context.AppendLine(path);
            }
            //保存
            SaveText(filename.Replace("\\", "/"), context.ToString());
        }
        //保存文件
        public static void SaveText(String fileName, String text)
        {
            if (!Directory.Exists(Utils.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Utils.GetDirectoryName(fileName));
            }
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    //开始写入
                    sw.Write(text);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                }
                fs.Close();
            }
        }
    }
}