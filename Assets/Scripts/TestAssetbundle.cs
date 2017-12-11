/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TestAssetbundle
// 创建者：Ash Tang
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using Mogo.Game;
using Mogo.Util;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using UnityEngine;
using System.Linq;

public class TestAssetbundle : MonoBehaviour
{
    /// <summary>
    /// 存放被调用资源信息。（name为主键）
    /// </summary>
    private Dictionary<string, AssetBundleInfo> m_assetBundleDic = new Dictionary<string, AssetBundleInfo>();
    /// <summary>
    /// 存放所有资源信息。（path为主键）
    /// </summary>
    private Dictionary<string, AssetBundleInfo> m_allRecources = new Dictionary<string, AssetBundleInfo>();
    private bool m_isLoading;
    /// <summary>
    /// 资源加载等待队列
    /// </summary>
    private Queue<System.Action> m_resourceLoadQueue = new Queue<System.Action>();
    private readonly object m_resourceLoadQueueLocker = new object();

    private List<Object> objects = new List<Object>();

    string prefabName = "AssistantUI.prefab";
    int index;

    void OnGUI()
    {
        LoggerHelper.Debug("haha", "haha1");
        prefabName = GUI.TextField(new Rect(0, 60, 200, 40), prefabName);

        if (GUI.Button(new Rect(100, 100, 100, 100), "load info"))
        {
            LoadAssetBundleInfo();
        }
        if (GUI.Button(new Rect(0, 100, 100, 100), "create"))
        {
            LoadAssetBundleInfo(prefabName, (abi) =>
            {
                objects.Add(abi.GetInstance());
            });
        }
        if (GUI.Button(new Rect(0, 200, 100, 100), "create all"))
        {
            foreach (var item in m_assetBundleDic)
            {
                LoadAssetBundleInfo(item.Key, (abi) =>
                {
                    objects.Add(abi.GetInstance());
                });
            }
        }
        if (GUI.Button(new Rect(0, 300, 100, 100), "check missing"))
        {
            foreach (var item in objects)
            {
                FindInGO(item, new MissingScriptsCounter());
            }
        }
        if (GUI.Button(new Rect(0, 400, 100, 100), "Create next"))
        {
            var list = m_assetBundleDic.Keys.ToList();
            LoadAssetBundleInfo(list[index], (abi) =>
            {
                objects.Add(abi.GetInstance());
            });

            index++;
        }
    }

    private Dictionary<string, string> m_dicMap = new Dictionary<string, string>();

    void Start()
    {
        //foreach (var item in m_allRecources)
        //{
        //    LoggerHelper.Debug(item.Key + " " + item.Value.Path);
        //}
        //var path = GetFolderPath("ExportTemp");
        //LoggerHelper.Debug(path);
        //var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        //LoggerHelper.Debug(files.Length);
        //foreach (var item in files)
        //{
        //    StartCoroutine(StartLoad(item.Replace("\\", "/")));
        //}
        //StartCoroutine(WaitAndDo("Resources/GUI/ArenaUI.prefab"));
        //StartCoroutine(WaitAndDo("Resources/GUI/ArenaUI.prefab"));
        //LoadAssets("Resources/GUI/ArenaUI.prefab");
        //LoadAssets("Resources/GUI/MogoUI.prefab");
        //LoadAssets("Resources/GUI/ArenaUI.prefab");
        //LoadAssets("Resources/GUI/ArenaUI.prefab");
        //LoadAssets("Resources/GUI/ArenaUI.prefab");
        //LoadAssets("Resources/GUI/ArenaUI.prefab");
        //LoadAssets("Resources/GUI/ArenaUI.prefab");
        //LoadAssets("Resources/GUI/ArenaUI.prefab");
    }
    // 为去除警告暂时屏蔽以下代码
    //bool isLoading = false;
    Queue<System.Action> queue = new Queue<System.Action>();

    //void LoadAssets(string name)
    //{
    //    if (isLoading)
    //        queue.Enqueue(() => StartCoroutine(WaitAndDo(name)));
    //    else
    //        StartCoroutine(WaitAndDo(name));
    //}

    //IEnumerator WaitAndDo(string path)
    //{
    //    isLoading = true;
    //    var res = ResourceManager.GetResource(path);
    //    yield return StartCoroutine(res.Wait());

    //    GameObject.Instantiate(res.Object);

    //    if (queue.Count != 0)
    //    {
    //        var action = queue.Dequeue();
    //        action();
    //    }
    //    else
    //    {
    //        isLoading = false;
    //    }
    //}
    private IEnumerator StartLoad(string fileName)
    {
        WWW w = new WWW(SystemConfig.ASSET_FILE_HEAD + fileName);
        yield return w;

        var go = w.assetBundle.LoadAsset(Utils.GetFileName(fileName));
        if (fileName.EndsWith("1.prefab.u"))
        {
            GameObject.Instantiate(go);
        }
        if (fileName.EndsWith("2.prefab.u"))
        {
            GameObject.Instantiate(go);
        }
        if (fileName.EndsWith("3.prefab.u"))
        {
            GameObject.Instantiate(go);
        }
    }

    private void LoadAssetBundleInfo()
    {
        var buildInfo = LoadBuildResourcesInfo();
        //LoggerHelper.Debug(buildInfo.Count);

        foreach (var build in buildInfo)
        {
            if (build.name == "UI")
            {
                var root = new DirectoryInfo(Application.dataPath);
                var path = Path.Combine(root.Parent.FullName, "Export\\0.0.0.1\\" + build.name + "\\MogoResources");
                var dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
                foreach (var item in dirs)
                {
                    var folder = Path.Combine(item, build.type);
                    var defFiles = Directory.GetFiles(folder);
                    //LoggerHelper.Debug(Utils.GetFileNameWithoutExtention(defFiles[0], '\\'));
                    var fileName = Utils.GetFileNameWithoutExtention(defFiles[0], '\\') + build.extentions[0];
                    if (!m_dicMap.ContainsKey(fileName))
                        m_dicMap.Add(fileName, item);
                    else
                        LoggerHelper.Debug("fileName exist: " + fileName);
                    BuildMogoAssetBundleInfo(folder);
                }
            }
        }
        LoggerHelper.Debug("m_allRecources.Count: " + m_allRecources.Count);
        LoggerHelper.Debug("m_assetBundleDic.Count: " + m_assetBundleDic.Count);
    }

    /// <summary>
    /// 根据资源配置信息构造资源依赖字典。
    /// </summary>
    /// <param name="folder">资源配置信息所在文件夹</param>
    private void BuildMogoAssetBundleInfo(string folder)
    {
        var defFiles = Directory.GetFiles(folder);
        foreach (var item in defFiles)
        {
            var xmlText = Utils.LoadFile(item);
            var xml = XMLParser.LoadXML(xmlText);
            foreach (SecurityElement se in xml.Children)
            {
                var asset = GetAssetBundleInfo(se, string.Empty);
                var key = Utils.GetFileName(asset.Path.Replace("\\", "/"));
                //LoggerHelper.Warning(asset.Path + " " + key);
                if (!m_assetBundleDic.ContainsKey(key))
                {
                    m_assetBundleDic[key] = asset;
                }
            }
        }
    }

    /// <summary>
    /// 根据资源配置XML构造资源依赖信息。
    /// </summary>
    /// <param name="se">XML数据</param>
    /// <param name="parentFolder"></param>
    /// <returns></returns>
    private AssetBundleInfo GetAssetBundleInfo(SecurityElement se, string parentFolder)
    {
        var path = string.Concat(parentFolder, (se.Children[0] as SecurityElement).Text);//.Replace("\\", "/");//
        if (m_allRecources.ContainsKey(path))
        {
            //LoggerHelper.Warning("Same resource: " + path);
            return m_allRecources[path];
        }
        else
        {
            var ab = new AssetBundleInfo();
            ab.Path = path;
            for (int i = 1; i < se.Children.Count; i++)
            {
                var child = se.Children[i] as SecurityElement;
                ab.SubResource.Add(GetAssetBundleInfo(child, parentFolder));
            }
            m_allRecources.Add(path, ab);
            return ab;
        }
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="loaded"></param>
    private void LoadAssetBundleInfo(string prefab, System.Action<AssetBundleInfo> loaded)
    {
        AssetBundleInfo assetBundleInfo;
        var flag = m_assetBundleDic.TryGetValue(prefab, out assetBundleInfo);
        if (flag)
        {
            if (assetBundleInfo.Asset != null)
            {//资源已加载，直接回调
                //LoggerHelper.Debug("resource loaded: " + assetBundleInfo.Path);
                loaded(assetBundleInfo);
            }
            else
            {
                LoadListInfo loadList = new LoadListInfo();
                loadList.Root = prefab;
                //LoggerHelper.Debug(prefab);
                loadList.LoadList = new List<string>();
                loadList.LoadList.AddRange(GetAllFile(assetBundleInfo));//获取加载资源列表
                //LoggerHelper.Debug(loadList.LoadList.Count);

                LoggerHelper.Debug("FileCount: " + loadList.LoadList.Count);
                if (m_isLoading)
                {
                    //LoggerHelper.Debug("m_resourceLoadQueue Enqueue: " + assetBundleInfo.Path + m_resourceLoadQueue.Count);
                    lock (m_resourceLoadQueueLocker)
                        m_resourceLoadQueue.Enqueue(() => { StartCoroutine(StartLoadAsset(loadList, loaded)); });
                }
                else
                {
                    m_isLoading = true;
                    StartCoroutine(StartLoadAsset(loadList, loaded));
                }
            }
        }
        else
        {
            LoggerHelper.Warning(prefab + " does not exist.");
        }
    }

    /// <summary>
    /// 获取所有需要加载的资源文件信息。
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private HashSet<string> GetAllFile(AssetBundleInfo info)
    {
        HashSet<string> list = new HashSet<string>();
        if (info.Asset == null)
        {
            foreach (var item in info.SubResource)
            {
                var subRes = GetAllFile(item);
                foreach (var set in subRes)
                {
                    if (!list.Add(set))
                        LoggerHelper.Debug("exist 1: " + set);
                }
            }
            if (!list.Add(info.Path))
                LoggerHelper.Debug("exist 2: " + info.Path);
        }
        return list;
    }

    /// <summary>
    /// 开始加载资源。
    /// </summary>
    /// <param name="loadList"></param>
    /// <param name="loaded"></param>
    /// <returns></returns>
    private IEnumerator StartLoadAsset(LoadListInfo loadList, System.Action<AssetBundleInfo> loaded)
    {
        while (true)
        {
            if (loadList.Download != null)
                loadList.Download.Dispose();
            string path = loadList.Next;
            LoggerHelper.Debug("path" + path);
            var key = Utils.GetFileName(path);
            LoggerHelper.Debug("key" + key);
            var info = m_allRecources[path];
            if (info.Asset == null)//再次验证资源是否已加载
            {
                var temp = string.Concat(SystemConfig.ASSET_FILE_HEAD, m_dicMap.Get(Utils.GetFileName(loadList.Root)), "/", path, SystemConfig.ASSET_FILE_EXTENSION);//拼资源路径
                temp = temp.Replace("\\", "/");
                LoggerHelper.Debug(temp);
                loadList.Download = new WWW(temp);
                yield return loadList.Download;//异步加载资源

                //加载完成后缓存起资源引用
                info.Asset = loadList.Download.assetBundle;
            }
            //info.ReferenceCount = 1;
            //LoggerHelper.Debug(key + loadList.Index);
            if (!loadList.HasNext)//加载完成后回调加载完成事件
            {
                var assetInfo = m_assetBundleDic[loadList.Root];
                try
                {
                    if (loaded != null)
                        loaded(assetInfo);
                }
                catch (System.Exception ex)
                {
                    LoggerHelper.Except(ex);
                }
                ContinueLoad();
                yield break;
            }
        }
    }

    /// <summary>
    /// 继续加载队列资源。
    /// </summary>
    private void ContinueLoad()
    {
        lock (m_resourceLoadQueueLocker)
        {
            if (m_resourceLoadQueue.Count != 0)
            {
                //LoggerHelper.Debug("m_resourceLoadQueue Dequeue: " + m_resourceLoadQueue.Count);
                System.Action action = m_resourceLoadQueue.Dequeue();
                action();
            }
            else
                m_isLoading = false;
        }
    }

    /// <summary>
    /// 资源加载控制索引器。
    /// </summary>
    public class LoadListInfo
    {
        /// <summary>
        /// 加载资源名称
        /// </summary>
        public string Root { get; set; }
        public WWW Download { get; set; }
        public List<string> LoadList { get; set; }
        public int Index { get; set; }
        public bool HasNext
        {
            get { return Index < LoadList.Count; }
        }
        public string Next
        {
            get { return LoadList[Index++]; }
        }
    }

    /// <summary>
    /// 资源管理实体类
    /// </summary>
    public class AssetBundleInfo
    {
        private Object m_gameObject;

        public string Path { get; set; }
        public int ReferenceCount { get; set; }
        public AssetBundle Asset { get; set; }
        public bool IsGameObjectLoaded { get { return m_gameObject; } }
        public Object GameObject
        {
            get
            {
                if (!m_gameObject)
                {
                    //LoggerHelper.Debug(Path);
                    m_gameObject = Asset.LoadAsset(Path);
                }
                return m_gameObject;
            }
        }
        public List<AssetBundleInfo> SubResource { get; set; }

        public Object GetInstance()
        {
            try
            {
                if (!Path.EndsWith(".unity"))
                    return UnityEngine.GameObject.Instantiate(GameObject);
                else
                    return null;
            }
            catch (System.Exception ex)
            {
                LoggerHelper.Error("AssetBundle get instance: '" + Path + "' error: " + ex.Message);
                return null;
            }
        }

        public AssetBundleInfo()
        {
            SubResource = new List<AssetBundleInfo>();
        }
    }

    #region BuildInfo


    /// <summary>
    /// 获取输出资源信息。
    /// </summary>
    /// <returns></returns>
    public static List<BuildResourcesInfo> LoadBuildResourcesInfo()
    {
        var path = string.Concat(GetFolderPath("ResourceDef"), "\\ForBuild.xml");
        //LogDebug(path);
        var xml = XMLParser.LoadXML(Utils.LoadFile(path));
        if (xml == null)
        {
            //EditorUtility.DisplayDialog("Error", "Load Build Resources Info Error.", "ok");
            return null;
        }
        var result = new List<BuildResourcesInfo>();

        foreach (SecurityElement item in xml.Children)
        {
            var info = new BuildResourcesInfo();
            info.check = true;
            info.name = (item.Children[0] as SecurityElement).Text;
            info.type = (item.Children[1] as SecurityElement).Text;
            info.copyOrder = (item.Children[2] as SecurityElement).Text;
            info.isPopInBuild = bool.Parse((item.Children[3] as SecurityElement).Text);
            info.extentions = (item.Children[4] as SecurityElement).Text.Split(' ');
            var folders = item.Children[5] as SecurityElement;
            info.folders = new List<BuildResourcesSubInfo>();
            foreach (SecurityElement folder in folders.Children)
            {
                var sub = new BuildResourcesSubInfo()
                {
                    path = (folder.Children[0] as SecurityElement).Text,
                    deep = bool.Parse((folder.Children[1] as SecurityElement).Text),
                    check = true
                };
                info.folders.Add(sub);
            }
            result.Add(info);
        }

        return result;
    }

    /// <summary>
    /// 获取拷贝资源信息。
    /// </summary>
    /// <returns></returns>
    public static List<CopyResourcesInfo> LoadCopyResourcesInfo()
    {
        var path = string.Concat(GetFolderPath("ResourceDef"), "\\ForCopy.xml");
        //LogDebug(path);
        var xml = XMLParser.LoadXML(Utils.LoadFile(path));
        if (xml == null)
        {
            //EditorUtility.DisplayDialog("Error", "Load Copy Resources Info Error.", "ok");
            return null;
        }
        var result = new List<CopyResourcesInfo>();

        foreach (SecurityElement item in xml.Children)
        {
            var info = new CopyResourcesInfo();
            info.check = true;
            info.targetPath = (item.Children[0] as SecurityElement).Text;
            info.sourcePath = (item.Children[1] as SecurityElement).Text;
            info.extention = (item.Children[2] as SecurityElement).Text;
            result.Add(info);
        }

        return result;
    }

    /// <summary>
    /// 获取项目目录下文件夹路径。
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static string GetFolderPath(string folder)
    {
        var root = new DirectoryInfo(Application.dataPath);
        var path = Path.Combine(root.Parent.FullName, folder);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    public class BuildResourcesInfo
    {
        public string name { get; set; }
        public string type { get; set; }
        public string copyOrder { get; set; }
        public bool isPopInBuild { get; set; }
        public string[] extentions { get; set; }
        public bool check { get; set; }
        public List<BuildResourcesSubInfo> folders { get; set; }
    }

    public class BuildResourcesSubInfo
    {
        public string path { get; set; }
        public bool deep { get; set; }
        public bool check { get; set; }
    }

    public class CopyResourcesInfo
    {
        public string targetPath { get; set; }
        public string sourcePath { get; set; }
        public string extention { get; set; }
        public bool check { get; set; }
    }
    #endregion

    #region FindMissing

    void OnDoTask(Component com, int i, GameObject g, MissingScriptsCounter counter)
    {
        if (com == null)
        {
            var fullName = GetFullName(g);
            counter.missingCount++;
            Debug.LogWarning(fullName + " has an empty script attached in position: " + i, g);
        }
    }

    private void FindInGO(Object go, MissingScriptsCounter counter)
    {
        var g = go as GameObject;
        if (g == null)
            return;
        counter.goCount++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            counter.componentsCount++;
            OnDoTask(components[i], i, g, counter);
            //if (components[i] == null)
            //{
            //    var fullName = GetFullName(g);
            //    counter.missingCount++;
            //    Mogo.Util.LoggerHelper.Debug(fullName + " has an empty script attached in position: " + i, g);
            //}
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Mogo.Util.LoggerHelper.Debug("Searching " + childT.name  + " " );
            FindInGO(childT.gameObject, counter);
        }
    }

    private string GetFullName(GameObject go)
    {
        string name = go.name;
        var tempGo = go.transform;

        while (tempGo.parent != null)
        {
            name = string.Concat(tempGo.parent.name, ".", name);
            tempGo = tempGo.parent;
        }
        return name;
    }

    public class MissingScriptsCounter
    {
        public int goCount { get; set; }
        public int componentsCount { get; set; }
        public int missingCount { get; set; }
        public int currentIndex { get; set; }
    }

    #endregion
}