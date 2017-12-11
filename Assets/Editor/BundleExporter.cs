using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Mogo.Util;
public class BundleExporter
{
    public static readonly string bundlePath = "Assets/";
    private static int exportTimes = 0;
    /// <summary>
    /// 保存资源层次关系。
    /// </summary>
    private static Dictionary<string, int> m_dependencyTree = new Dictionary<string, int>();
    /// <summary>
    /// 保存每个资源的依赖信息。
    /// </summary>
    public static Dictionary<string, List<string>> dependenciesDic = new Dictionary<string, List<string>>();
    private static List<string> fileList = new List<string>();
    private static readonly BuildAssetBundleOptions options =
    BuildAssetBundleOptions.CollectDependencies |
    //包含所有依赖关系，应该是将这个物体或组件所使用的其他资源一并打包
    BuildAssetBundleOptions.CompleteAssets |
    BuildAssetBundleOptions.DeterministicAssetBundle;
    public static BuildAssetBundleOptions Options
    {
        get { return options; }
    }

    public static void ExportAllBundles()
    {
        ExportScenesManager.AutoSwitchTarget();
        string path = "Assets/Resources/GUI";
        var rootPath = ExportScenesManager.GetFolderPath(ExportScenesManager.SubMogoResources);
        fileList.Clear();
        GetSubDir(path);
        var files = fileList.Select(x => x.Replace('\\', '/')).Where(x => x.EndsWith(".prefab")).ToArray();
        ExportScenesManager.LogDebug(files.PackArray('\n'));
        BuildBundleWithRoot(files, rootPath, true);
    }

    [MenuItem("Assets/Export Bundles")]
    public static void ExportBundles()
    {
        ExportScenesManager.AutoSwitchTarget();
        //ExportScenesManager.CurrentBuildTarget = BuildTarget.StandaloneWindows64;
        var rootPath = ExportScenesManager.GetFolderPath(ExportScenesManager.SubMogoResources);
        var selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        var paths = (from s in selection
                     let path = AssetDatabase.GetAssetPath(s)
                     where File.Exists(path)
                     select path).ToArray();
        foreach (var item in paths)
        {
            ExportBundle(new string[] { item }, rootPath);
        }
        //var shederPath = ExportScenesManager.GetFolderPath("Shader_PC");
        //ExportScenesManager.DirectoryCopy(shederPath, rootPath, true, true);
        //var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //if (File.Exists(path))
        //    ExportBundleIfNeeded(path, rootPath);
        //else if (Directory.Exists(path))
        //    ExportDirectoryIfNeeded(path);
    }

	public static void MakeMeta(string [] files,string exportRootPath)
	{
		Clean();
		BuildDependencyTree(files);
		var sorted = (from dependency in m_dependencyTree
		              group dependency by dependency.Value into g
		              select new { Layer = g.Key, Dependencies = from pair in g select pair.Key })
			.OrderByDescending((x) => x.Layer);
		for (int i = 0; i < sorted.Count(); ++i)
		{
			foreach (string dependency in sorted.ElementAt(i).Dependencies)
			{
				var relativePath = GetRelativePath(dependency);
				var exportPath = string.Concat(exportRootPath, "/", relativePath);
				var exportDirectory = Path.GetDirectoryName(exportPath);
				if (!Directory.Exists(exportDirectory))
					Directory.CreateDirectory(exportDirectory);
				IEnumerable<string> lowerDependencies = null;
				
				if (i < sorted.Count() && i > 0 && dependenciesDic.ContainsKey(dependency) && dependenciesDic[dependency].Count != 0)//
					lowerDependencies = sorted.ElementAt(i - 1).Dependencies.Where(t => dependenciesDic[dependency].Contains(t));//修改成获取精确依赖
				if (ExportBundleWithoutMetaDataIOS(dependency, Mogo.Util.ResourceManager.WithSuffix(exportPath)))
				{
					WriteMetaData(exportRootPath, relativePath, lowerDependencies, exportRootPath);
				}
			}
		}
	}

    [MenuItem("Assets/Export Bundles Ex")]
    public static void ExportBundlesEx()
    {
        ExportScenesManager.AutoSwitchTarget();
        var dirs = Directory.GetDirectories(ExportScenesManager.GetFolderPath(ExportScenesManager.ExportPath));
        //if (files.Length != 0)
        //{
        var currentVersion = new VersionCodeInfo("0.0.0.1");
        foreach (var item in dirs)
        {
            var fileVersion = new VersionCodeInfo(new DirectoryInfo(item).Name);
            if (fileVersion.Compare(currentVersion) > 0)
                currentVersion = fileVersion;
        }
        //var m_currentVersion = currentVersion.GetLowerVersion();
        var m_newVersion = currentVersion.ToString();
        var m_newVersionFolder = Path.Combine(ExportScenesManager.GetFolderPath(ExportScenesManager.ExportPath), m_newVersion).Replace("\\", "/");

        var targetPath = m_newVersionFolder + BuildProjectExWizard.ExportFilesPath;

        var selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        var paths = (from s in selection
                     let path = AssetDatabase.GetAssetPath(s)
                     where File.Exists(path)
                     select path).ToArray();
        foreach (string item in paths)
        {
			Debug.Log("ex "+item);
            ExportBundle(new string[] { item }, targetPath);
        }
    }

    public static void ExportDirectoryIfNeeded(string path)
    {
        if (!path.StartsWith(bundlePath))
            return;

        ExportDirectory(path);
    }
    public static void ExportBundleIfNeeded(string path, string rootPath)
    {
        if (!path.StartsWith(bundlePath))
            return;

        ExportBundle(new string[] { path }, rootPath);
    }
    public static void BuildBundleWithRoot(string[] roots, string exportRootPath, bool isMerge = false)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        if (isMerge)
        {
            ExportBundle(roots, exportRootPath);
        }
        else
        {
            foreach (var item in roots)
            {
                ExportBundle(new string[] { item }, exportRootPath);
            }
        }
        sw.Stop();
        ExportScenesManager.LogDebug("cost time: " + sw.ElapsedMilliseconds);
    }
    public static void BuildAssetVersion(string[] files, string rootPath)
    {
        Clean();
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        BuildDependencyTree(files);
        sw.Stop();
        ExportScenesManager.LogDebug("BuildDependencyTree time: " + sw.ElapsedMilliseconds);
        //LoggerHelper.Debug(m_dependencyTree.PackMap(':', '\n'));
        var sorted = (from dependency in m_dependencyTree
                      group dependency by dependency.Value into g
                      select new { Layer = g.Key, Dependencies = from pair in g select pair.Key })
                     .OrderByDescending((x) => x.Layer);
        var count = sorted.Count();
        ExportScenesManager.LogDebug("sorted.Count: " + count);
        for (int i = 0; i < count; ++i)
        {
            foreach (var dependency in sorted.ElementAt(i).Dependencies)
            {
                var relativePath = GetRelativePath(dependency);
                //LoggerHelper.Debug("relativePath: " + relativePath);
                //var exportFileName = ResourceManager.GetExportFileName(relativePath);
                var exportPath = string.Concat(rootPath, "/", relativePath);
                //LoggerHelper.Debug("exportPath: " + exportPath);
                //LoggerHelper.Debug(exportPath);
                var exportDirectory = Path.GetDirectoryName(exportPath);
                if (!Directory.Exists(exportDirectory))
                    Directory.CreateDirectory(exportDirectory);
                IEnumerable<string> lowerDependencies = null;

                if (i < sorted.Count() && i > 0 && dependenciesDic.ContainsKey(dependency) && dependenciesDic[dependency].Count != 0)
                    lowerDependencies = sorted.ElementAt(i - 1).Dependencies.Where(t => dependenciesDic[dependency].Contains(t));//修改成获取精确依赖
                WriteMetaData(rootPath, relativePath, lowerDependencies, Application.dataPath, false);
                //LoggerHelper.Debug(exportPath + " 生成成功。");
            }
        }
    }

    private static void Clean()
    {
        exportTimes = 0;
        m_dependencyTree.Clear();
        EditorUtility.UnloadUnusedAssets();
    }
    public static void ClearDependenciesDic()
    {
        dependenciesDic.Clear();
    }

    public static string[] FindDependencyRoot(string[] files)
    {
        var dep = new HashSet<string>();
        foreach (var item in files)
        {
            if (dependenciesDic.ContainsKey(item))//缓存依赖
            {
                foreach (var d in dependenciesDic[item])
                {
                    dep.Add(d);
                }
                //Mogo.Util.LoggerHelper.Debug("Use cache dependencies...." + element.Key);
            }
            else
            {
                var deps = GetDependencies(item).ToList();
                foreach (var d in deps)
                {
                    dep.Add(d);
                }
                dependenciesDic[item] = deps;
            }
        }
        var result = new List<string>();
        foreach (var item in files)
        {
            if (!dep.Contains(item))
                result.Add(item);
        }

        return result.ToArray();
    }

    /// <summary>
    /// 去除路径中Assets部分。
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string GetRelativePath(string path)
    {
        if (!path.StartsWith(bundlePath))
            throw new InvalidOperationException("路径不在Assets/Bundles下。");
        return path.Substring(bundlePath.Length);
    }

    private static void GetSubDir(string path)
    {
        string[] sDir = Directory.GetDirectories(path);//获取子目录的名称
        string[] sFile = Directory.GetFiles(path);//获取文件
        for (int i = 0; i < sFile.Length; i++)
        {
            fileList.Add(sFile[i]);   //将文件加入数组

        }
        for (int i = 0; i < sDir.Length; i++)
        {
            GetSubDir(sDir[i]);
        }
    }

    private static void ExportDirectory(string path)
    {

    }
	public static void ExportBundleForCloud(string[] files, string exportRootPath)
	{
		Clean();
		BuildDependencyTree(files);
		var sorted = (from dependency in m_dependencyTree
		              group dependency by dependency.Value into g
		              select new { Layer = g.Key, Dependencies = from pair in g select pair.Key })
			.OrderByDescending((x) => x.Layer);
		Debug.Log("statck depth will be "+sorted.Count());
		for (int i = 0; i < sorted.Count(); ++i)
		{
			BuildPipeline.PushAssetDependencies();
			
			foreach (string dependency in sorted.ElementAt(i).Dependencies)
			{
				Debug.Log("layer "+i+" build "+dependency);
				ExportScenesManager.LogDebug(dependency);
				var relativePath = GetRelativePath(dependency);
				//var exportFileName = ResourceManager.GetExportFileName(relativePath);
				var exportPath = string.Concat(exportRootPath, "/", relativePath);
				//LoggerHelper.Debug(exportPath);
				var exportDirectory = Path.GetDirectoryName(exportPath);
				if (!Directory.Exists(exportDirectory))
					Directory.CreateDirectory(exportDirectory);
				IEnumerable<string> lowerDependencies = null;
				
				if (i < sorted.Count() && i > 0 && dependenciesDic.ContainsKey(dependency) && dependenciesDic[dependency].Count != 0)//
					lowerDependencies = sorted.ElementAt(i - 1).Dependencies.Where(t => dependenciesDic[dependency].Contains(t));//修改成获取精确依赖
				if (ExportBundleWithoutMetaData(dependency, Mogo.Util.ResourceManager.WithSuffix(exportPath)))
				{
					ExportScenesManager.LogDebug(exportPath + " 导出成功。");
				}
			}
		}
		for (int i = 0; i < sorted.Count(); ++i)
		{
			BuildPipeline.PopAssetDependencies();
		}
	}

    private static void ExportBundle(string[] files, string exportRootPath)
    {
        Clean();
        BuildDependencyTree(files);
        var sorted = (from dependency in m_dependencyTree
                      group dependency by dependency.Value into g
                      select new { Layer = g.Key, Dependencies = from pair in g select pair.Key })
                     .OrderByDescending((x) => x.Layer);
		Debug.Log("statck depth will be "+sorted.Count());
        for (int i = 0; i < sorted.Count(); ++i)
        {
            BuildPipeline.PushAssetDependencies();

            foreach (string dependency in sorted.ElementAt(i).Dependencies)
            {
				Debug.Log("layer "+i+" build "+dependency);
                ExportScenesManager.LogDebug(dependency);
                var relativePath = GetRelativePath(dependency);
                //var exportFileName = ResourceManager.GetExportFileName(relativePath);
                var exportPath = string.Concat(exportRootPath, "/", relativePath);
                //LoggerHelper.Debug(exportPath);
                var exportDirectory = Path.GetDirectoryName(exportPath);
                if (!Directory.Exists(exportDirectory))
                    Directory.CreateDirectory(exportDirectory);
                IEnumerable<string> lowerDependencies = null;

                if (i < sorted.Count() && i > 0 && dependenciesDic.ContainsKey(dependency) && dependenciesDic[dependency].Count != 0)//
                    lowerDependencies = sorted.ElementAt(i - 1).Dependencies.Where(t => dependenciesDic[dependency].Contains(t));//修改成获取精确依赖
                if (ExportBundleWithoutMetaData(dependency, Mogo.Util.ResourceManager.WithSuffix(exportPath)))
                {
                    WriteMetaData(exportRootPath, relativePath, lowerDependencies, exportRootPath);
                    ExportScenesManager.LogDebug(exportPath + " 导出成功。");
                }
            }
        }
        for (int i = 0; i < sorted.Count(); ++i)
        {
            BuildPipeline.PopAssetDependencies();
        }
    }

    public static void BuildDependencyTree(string[] path)
    {
        int layer = 0;
        foreach (var p in path)
        {
            m_dependencyTree[p] = layer;
        }
        while (m_dependencyTree.Values.Any((x) => x == layer))
        {
            var layerElements = (from element in m_dependencyTree
                                 where element.Value == layer
                                 select element).ToList();
            ++layer;
            foreach (var element in layerElements)
            {
                List<string> dep;
                if (dependenciesDic.ContainsKey(element.Key))//缓存依赖
                {
                    dep = dependenciesDic[element.Key];
                    //Mogo.Util.LoggerHelper.Debug("Use cache dependencies...." + element.Key);
                }
                else
                {
                    dep = GetDependencies(element.Key).ToList();
                    dependenciesDic[element.Key] = dep;
                }
                foreach (var dependency in dep)
                {
                    m_dependencyTree[dependency] = layer;
                }
            }
        }
    }

    private static IEnumerable<string> GetDependencies(string path)
    {
        var dependencies = (from objPath in AssetDatabase.GetDependencies(new string[] { path })
                            where objPath.StartsWith(bundlePath)
                            && objPath != path
                            && ResourceManager.IsPackedExportable(objPath)
                            select objPath).Distinct();
        return dependencies;
    }
	private static bool ExportBundleWithoutMetaDataIOS(string path,
	                                                string exportPath)
	{
		if (ResourceManager.IsExportable(path))
		{
			ExportExportable(path, exportPath);
		}
		else if (ResourceManager.IsPackedExportable(path))
		{

		}
		else
		{
			return false;
		}
		
		return true;
	}
    private static bool ExportBundleWithoutMetaData(string path,
        string exportPath)
    {
        if (ResourceManager.IsExportable(path))
        {
            ExportExportable(path, exportPath);
        }
        else if (ResourceManager.IsPackedExportable(path))
        {
            ExportPackedExportable(path, exportPath);
        }
        else
        {
            return false;
        }

        return true;
    }

    private static void ExportExportable(string path, string exportPath)
    {
        FileUtil.CopyFileOrDirectory(path, exportPath);
    }

    private static void ExportPackedExportable(string path, string exportPath)
    {
        System.Diagnostics.Stopwatch watcher = new System.Diagnostics.Stopwatch();
        watcher.Start();
        exportTimes++;
        if (!File.Exists(path)) return;
        if (path.EndsWith(".unity"))
        {
#if UNITY_IPHONE
			BuildPipeline.BuildStreamedSceneAssetBundle(new string[] { path }, exportPath,BuildTarget.iOS);
#else
            BuildPipeline.BuildStreamedSceneAssetBundle(new string[] { path }, exportPath, ExportScenesManager.CurrentBuildTarget);
#endif
		}
        else
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(path);
#if UNITY_IPHONE
			if (!BuildPipeline.BuildAssetBundle(obj, null, exportPath, Options,BuildTarget.iOS))
#else
			if (!BuildPipeline.BuildAssetBundle(obj, null, exportPath, Options, ExportScenesManager.CurrentBuildTarget))
#endif
            
            {
                throw new InvalidOperationException(exportPath + " 导出失败。");
            }
        }
        watcher.Stop();
        ExportScenesManager.LogDebug(exportTimes + "/" + m_dependencyTree.Count + ":" + watcher.ElapsedMilliseconds + "     " + path);
    }

    //private static void WriteMetaData(string relativePath, IEnumerable<string> dependencies)
    //{
    //    WriteMetaData(Preferences.GetString("客户端资源导出路径"), relativePath, dependencies, Preferences.GetString("客户端资源导出路径"));
    //}
    /// <summary>
    /// 生成资源依赖与标记信息文本。
    /// </summary>
    /// <param name="rootPath">文本生成目标根目录。</param>
    /// <param name="relativePath">文本生成相对路径。</param>
    /// <param name="dependencies">资源依赖信息。</param>
    /// <param name="targetPath">资源source根目录。</param>
    /// <param name="needSuffix">是否需要拼导出资源的后缀。</param>
    private static void WriteMetaData(string rootPath, string relativePath, IEnumerable<string> dependencies, string targetPath, bool needSuffix = true)
    {
        //LoggerHelper.Debug(relativePath);
        var xmlFileName = GetXmlFileName(relativePath);
        var xmlPath = String.Concat(rootPath, "/", xmlFileName);

        //var sw = new System.Diagnostics.Stopwatch();
        //sw.Start();
        var doc = GetXmlDocument(xmlPath);
        //Mogo.Util.LoggerHelper.Debug("GetXmlDocument: " + sw.ElapsedMilliseconds);

        var node = InitXmlNode(doc, relativePath);
        //Mogo.Util.LoggerHelper.Debug("InitXmlNode: " + sw.ElapsedMilliseconds);
        SetXmlNodeFilePath(node, relativePath);
        //Mogo.Util.LoggerHelper.Debug("SetXmlNodeFilePath: " + sw.ElapsedMilliseconds);
        SetXmlNodeMD5(node, targetPath, relativePath, needSuffix);
        //Mogo.Util.LoggerHelper.Debug("SetXmlNodeMD5: " + sw.ElapsedMilliseconds);
        SetXmlNodeDependencies(doc, node, dependencies);
        //Mogo.Util.LoggerHelper.Debug("SetXmlNodeDependencies: " + sw.ElapsedMilliseconds);

        doc.Save(xmlPath);
        //sw.Stop();
        //Mogo.Util.LoggerHelper.Debug("Save: " + sw.ElapsedMilliseconds);
        if (xmlFileName != ResourceManager.MetaFileName)
            WriteTotalMeta(rootPath, xmlFileName);
    }
    private static void WriteTotalMeta(string rootPath, string relativePath)
    {
        var xmlFileName = ResourceManager.MetaFileName;
        var xmlPath = String.Concat(rootPath, "/", xmlFileName);

        var doc = GetXmlDocument(xmlPath);

        var node = InitXmlNode(doc, relativePath);
        SetXmlNodeFilePath(node, relativePath);
        SetXmlNodeMD5(node, rootPath, relativePath);
        SetXmlNodeDependencies(doc, node, null);

        doc.Save(xmlPath);
    }
    //private static void WriteTotalMeta(string relativePath)
    //{
    //    WriteTotalMeta(Preferences.GetString("客户端资源导出路径"), relativePath);
    //}
    private static void SetXmlNodeFilePath(XmlElement node, string relativePath)
    {
        node.SetAttribute("path", relativePath);
    }

    //private static void SetXmlNodeMD5(XmlElement node, string relativePath)
    //{
    //    SetXmlNodeMD5(node, Preferences.GetString("客户端资源导出路径"), relativePath);
    //}

    private static void SetXmlNodeMD5(XmlElement node, string rootPath, string relativePath, bool needSuffix = true)
    {
        string md5 = GetFileMD5(String.Concat(rootPath, "/", needSuffix ? ResourceManager.WithSuffix(relativePath) : relativePath));
        node.SetAttribute("md5", md5);
    }

    private static void SetXmlNodeDependencies(XmlDocument doc,
        XmlElement node,
        IEnumerable<string> dependencies)
    {
        if (dependencies == null)
            return;

        foreach (var dependency in dependencies)
        {
            var child = doc.CreateElement("dependency");
            child.SetAttribute("path", GetRelativePath(dependency));
            node.AppendChild(child);
        }
    }

    private static XmlDocument GetXmlDocument(string xmlPath)
    {
        XmlDocument doc = new XmlDocument();

        if (File.Exists(xmlPath))
            doc.Load(xmlPath);
        return doc;
    }

    private static XmlElement InitXmlNode(XmlDocument xmlDoc, string relativePath)
    {
        var root = GetXmlRoot(xmlDoc);

        var node = (root.SelectSingleNode(string.Concat("file[@path = '", relativePath, "']")) as XmlElement);

        if (node == null)
        {
            node = xmlDoc.CreateElement("file");
            root.AppendChild(node);
        }
        else
        {
            node.RemoveAll();
        }
        return node;
    }

    private static XmlNode GetXmlRoot(XmlDocument xmlDoc)
    {
        var root = xmlDoc.SelectSingleNode("root");

        if (root == null)
        {
            root = xmlDoc.CreateElement("root");
            xmlDoc.AppendChild(root);
        }
        return root;
    }

    /// <summary>
    /// 拼接meta文件相对路径。
    /// </summary>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    private static string GetXmlFileName(string relativePath)
    {
        var xmlFileName = Path.GetDirectoryName(relativePath);

        if (xmlFileName != null && xmlFileName.Length > 0)
            xmlFileName += "/";

        xmlFileName += ResourceManager.MetaFileName;
        //xmlFileName = ResourceManager.GetExportFileName(xmlFileName);
        return xmlFileName;
    }

    private static string GetFileMD5(string path)
    {
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var md5 = Eddy.MD5Hash.Get(stream);
        stream.Close();
        return md5;
    }
}
