#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：NewBehaviourScript
// 创建者：
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/
#endregion

using System;
using Mogo.Util;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExportScenesManager
{
    #region 属性

    public const string ExportPath = "Export";

    /// <summary>
    /// 存放保存每个版本导出的文件记录的信息。
    /// </summary>
    public const string SubExportedFileList = "ExportedFileList";
    /// <summary>
    /// 存放最新版本所有输出的资源。
    /// </summary>
    public const string SubMogoResources = "MogoResources";
    /// <summary>
    /// 存放每个版本变化的已输出资源记录。
    /// </summary>
    public const string SubPackageVersion = "PackageVersion";
    /// <summary>
    /// 存放每个版本变化的原资源记录。
    /// </summary>
    public const string SubVersion = "Version";
    /// <summary>
    /// 存放打包后输出的资源包。
    /// </summary>
    public const string SubUpdatePackage = "UpdatePackage";

    /// <summary>
    /// 存放保存每个版本导出的文件记录的信息。
    /// </summary>
    private const string ExportedFileList = ExportPath + "/" + SubExportedFileList;
    /// <summary>
    /// 存放最新版本所有输出的资源。
    /// </summary>
    private const string MogoResources = ExportPath + "/" + SubMogoResources;
    /// <summary>
    /// 存放每个版本变化的已输出资源记录。
    /// </summary>
    private const string PackageVersion = ExportPath + "/" + SubPackageVersion;
    /// <summary>
    /// 存放每个版本变化的原资源记录。
    /// </summary>
    private const string Version = ExportPath + "/" + SubVersion;
    /// <summary>
    /// 存放打包后输出的资源包。
    /// </summary>
    private const string UpdatePackage = ExportPath + "/" + SubUpdatePackage;

    /// <summary>
    /// 记录需要被删除的多余的祖先关系。
    /// </summary>
    private static Queue<KeyValuePair<Dictionary<string, MogoResourceInfo>, string>> m_deleteQueue = new Queue<KeyValuePair<Dictionary<string, MogoResourceInfo>, string>>();
    /// <summary>
    /// 在导出时排列好导出顺序的资源。
    /// </summary>
    private static Stack<KeyValuePair<MogoResourceInfo, int>> m_resourceStack = new Stack<KeyValuePair<MogoResourceInfo, int>>();
    /// <summary>
    /// 一次导出中选中的资源。
    /// </summary>
    private static Dictionary<string, MogoResourceInfo> m_allResources = new Dictionary<string, MogoResourceInfo>();
    /// <summary>
    /// 临时存放没选中但有被依赖的资源。
    /// </summary>
    private static Dictionary<string, MogoResourceInfo> m_leftResources = new Dictionary<string, MogoResourceInfo>();
    /// <summary>
    /// 当前版本中资源的版本信息。
    /// </summary>
    private static Dictionary<string, VersionInfo> m_fileVersions = new Dictionary<string, VersionInfo>();
    /// <summary>
    /// 存储有更新的文件。
    /// </summary>
    private static Dictionary<string, VersionInfo> m_updatedFiles = new Dictionary<string, VersionInfo>();
    /// <summary>
    /// 导出资源配置数据。
    /// </summary>
    private static List<BuildResourcesInfo> m_buildResourcesInfoList;
    /// <summary>
    /// 拷贝资源配置数据。
    /// </summary>
    private static List<CopyResourcesInfo> m_copyResourcesInfoList;
    /// <summary>
    /// 全局导出选项。（一般不需修改）
    /// </summary>
    private static readonly BuildAssetBundleOptions options =
        BuildAssetBundleOptions.CollectDependencies |
        BuildAssetBundleOptions.CompleteAssets |
        BuildAssetBundleOptions.DeterministicAssetBundle;

    public static BuildAssetBundleOptions Options
    {
        get { return ExportScenesManager.options; }
    }

    /// <summary>
    /// 标记导出目标平台。
    /// </summary>
    private static BuildTarget m_currentBuildTarget = BuildTarget.StandaloneWindows64;

    public static BuildTarget CurrentBuildTarget
    {
        get { return ExportScenesManager.m_currentBuildTarget; }
        set { ExportScenesManager.m_currentBuildTarget = value; }
    }

    /// <summary>
    /// 当前版本中资源的版本信息。
    /// </summary>
    public static Dictionary<string, VersionInfo> FileVersions
    {
        get { return ExportScenesManager.m_fileVersions; }
        set { ExportScenesManager.m_fileVersions = value; }
    }

    /// <summary>
    /// 存储有更新的文件。
    /// </summary>
    public static Dictionary<string, VersionInfo> UpdatedFiles
    {
        get { return ExportScenesManager.m_updatedFiles; }
        set { ExportScenesManager.m_updatedFiles = value; }
    }

    /// <summary>
    /// 导出资源配置数据。
    /// </summary>
    public static List<BuildResourcesInfo> BuildResourcesInfoList
    {
        get { return ExportScenesManager.m_buildResourcesInfoList; }
        set { ExportScenesManager.m_buildResourcesInfoList = value; }
    }

    /// <summary>
    /// 拷贝资源配置数据。
    /// </summary>
    public static List<CopyResourcesInfo> CopyResourcesInfoList
    {
        get { return ExportScenesManager.m_copyResourcesInfoList; }
        set { ExportScenesManager.m_copyResourcesInfoList = value; }
    }

    #endregion

    #region 切换输出平台

    [MenuItem("BuildTarget/Android")]
    public static void BuildTargetAndroid()
    {
        m_currentBuildTarget = BuildTarget.Android;
    }

    [MenuItem("BuildTarget/iOS")]
    public static void BuildTargetiOS()
    {
		m_currentBuildTarget = BuildTarget.iOS;
    }

    [MenuItem("BuildTarget/PC")]
    public static void BuildTargetPC()
    {
        m_currentBuildTarget = BuildTarget.StandaloneWindows64;
    }

    public static void AutoSwitchTarget()
    {
        LogDebug(EditorUserBuildSettings.activeBuildTarget);
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                CurrentBuildTarget = BuildTarget.Android;
                break;
            case BuildTarget.StandaloneWindows:
                CurrentBuildTarget = BuildTarget.StandaloneWindows;
                break;
            case BuildTarget.StandaloneWindows64:
                CurrentBuildTarget = BuildTarget.StandaloneWindows64;
                break;
			case BuildTarget.iOS:
				CurrentBuildTarget = BuildTarget.iOS;
                break;
            default:
                break;
        }
    }

    #endregion

    #region 小工具

    [MenuItem("Tools/Check FX texture")]
    public static void CheckFXTexture()
    {
        var selection = GetSelections("Assets/Resources/Fx/fx_prefab");
        //Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var item in selection)
        {
            var go = item as GameObject;
            if (go)
                foreach (Transform p in go.transform)
                {
					ParticleSystem particleSystem = p.GetComponent<ParticleSystem> ();
					if (particleSystem)
                    {
						Renderer renderer = particleSystem.GetComponent<Renderer> ();
						if (renderer && renderer.sharedMaterial)
                        {
							if (!renderer.sharedMaterial.mainTexture)
                            {
                                sb.AppendLine(go + "null texture.");
                            }
                        }
                        else
                        {
                            sb.AppendLine(go + "null renderer.");
                        }
                    }
                }
        }
        LogDebug(sb.ToString());
    }

    [MenuItem("Tools/Update Texture To Advance")]
    public static void UpdateTextureToAdvance()
    {
        Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        foreach (var item in selection)
        {
            var txt = item as Texture;
            if (txt)
            {
                string path = AssetDatabase.GetAssetPath(txt);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                textureImporter.textureType = TextureImporterType.Default;
                textureImporter.mipmapEnabled = false;
                LogDebug(txt.name);
            }
        }
    }

    [MenuItem("Tools/Update Texture To mipmap")]
    public static void UpdateTextureToMipmap()
    {
        Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        foreach (var item in selection)
        {
            var txt = item as Texture;
            if (txt)
            {
                string path = AssetDatabase.GetAssetPath(txt);
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                textureImporter.textureType = TextureImporterType.Default;
                textureImporter.mipmapEnabled = true;
                LogDebug(txt.name);
            }
        }
    }

    [MenuItem("Tools/Set Player Shader")]
    public static void SetPlayerShader()
    {
        //LogDebug(AssetDatabase.GetAssetPath(Selection.activeObject));
        Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

        var shader = AssetDatabase.LoadAssetAtPath("Assets/Resources/Shader/PlayerShader.shader", typeof(Object)) as Shader;
        if (!shader)
        {
            LogError("player shader not exist.");
            return;
        }
        LogDebug(shader.name);
        foreach (var item in selection)
        {
            var mat = item as Material;
            if (mat)
            {
                mat.shader = shader;
            }
        }
        LogDebug(string.Format("Set {0} mats to player shader.", selection.Length));
    }
    [MenuItem("Tools/CopyBuildInShaders")]
    public static void CopyBuildInShaders()
    {
        string unityver = Application.unityVersion;
        LogDebug("UnityVersion:"+unityver);
        //4.3.0f4,比较版本
        //第一个.索引
        int firstDotIndex = unityver.IndexOf(".");   
        int firstV = int.Parse(unityver.Substring(0, firstDotIndex));
        int secondDotIndex = unityver.IndexOf(".",firstDotIndex+1);
        int secondV = int.Parse(unityver.Substring(firstDotIndex + 1, secondDotIndex - firstDotIndex-1));
        LogDebug("first:" + firstV + ":second:" + secondV);
        bool needCopy = (firstV > 4) || (firstV == 4 && secondV>=2);
        if (needCopy)
        {
            LogDebug("需要拷贝shader");
            string clientDir = new DirectoryInfo(Application.dataPath).Parent.FullName.Replace("\\", "/");
            var srcDir = clientDir + "/shader_version/builtin_shaders-4.3.0";
            var dstDir = clientDir + "/Assets/Resources/builtin_shaders-4.3.0";
            LogDebug("原目录："+srcDir);
            LogDebug("目标目录："+dstDir);
            DirectoryCopy(srcDir, dstDir, true,true);
        }
    }

    [MenuItem("Tools/Handle Build-In Shaders(谨慎使用)")]
    public static void HandleBuildInShaders()
    {
        var sb = new System.Text.StringBuilder();
        var projectPath = new DirectoryInfo(Application.dataPath).Parent.FullName.Replace("\\", "/") + "/";
        //首先找到所有的prefab
        var root = Application.dataPath + "/Resources/Scences";//"D:/mogo/client20131009/Assets/Resources/Scences/10004_City";//Application.dataPath;
        var prefabFiles = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
        var selection = (from f in prefabFiles
                         let file = f.ReplaceFirst(projectPath, "")
                         let go = AssetDatabase.LoadAssetAtPath(file, typeof(Object))
                         where go != null
                         select go).ToArray();
        //Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        LogDebug("selection 总数" + selection.Count());
        //用于打印的shader type
        List<string> shaderTypesList = new List<string>();

        foreach (var g in selection)
        {
            ChangeShaderInGo(g,ref shaderTypesList);
        }
        LogDebug("shader type 总数：" + shaderTypesList.Count);
        foreach (var item in shaderTypesList)
        {
            LogDebug(item);
        }
    }

    private static string ChangeShaderInGo(Object go,ref List<string> list)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var g = go as GameObject;
        if (g == null)
            return null;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            var com = components[i];
            if (com is MeshRenderer)
            {
                foreach (Material ml in (com as MeshRenderer).sharedMaterials)
                {
                    var shader = ml.shader;
                    if (!list.Contains(shader.name))
                        list.Add(shader.name);

                    Shader newshader = Shader.Find(shader.name);
                    if (newshader != null)
                        ml.shader = newshader;
                }
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            sb.AppendLine(ChangeShaderInGo(childT.gameObject,  ref list));
        }
        return sb.ToString();
    }

    [MenuItem("Tools/Set Monster Shader")]
    public static void SetMonsterShader()
    {
        //LogDebug(AssetDatabase.GetAssetPath(Selection.activeObject));
        Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

        var shader = AssetDatabase.LoadAssetAtPath("Assets/Resources/Shader/MonsterShader.shader", typeof(Object)) as Shader;
        if (!shader)
        {
            LogError("Monster shader not exist.");
            return;
        }
        LogDebug(shader.name);
        foreach (var item in selection)
        {
            var mat = item as Material;
            if (mat)
            {
                //LoggerHelper.Debug(mat);
                mat.shader = shader;
                mat.SetColor("_Color", new UnityEngine.Color(1, 1, 1, 1));
            }
        }
        LogDebug(string.Format("Set {0} mats to Monster shader.", selection.Length));
    }

    [MenuItem("Tools/Rename Anim")]
    public static void RenameAnim()
    {
        if (!EditorUtility.DisplayDialog("Conform", "确认你选对了目录？？\n真的要改名咯～", "冇问题!", "我不要!!!!!"))
        {
            return;
        }
        Object[] selection = Selection.GetFiltered(typeof(UnityEngine.AnimationClip), SelectionMode.DeepAssets);
        foreach (var item in selection)
        {
            var path = AssetDatabase.GetAssetPath(item);
            var file = new FileInfo(path);
            var type = file.Directory.Parent.Name;
            var head = string.Concat(type, '_');
            var target = Path.Combine(file.Directory.FullName, string.Concat(head, file.Name));
            if (!file.Name.StartsWith(head))
            {
                var meta = new FileInfo(string.Concat(file.FullName, ".meta"));
                meta.MoveTo(string.Concat(target, ".meta"));
                file.MoveTo(target);
            }
            LogDebug(target);
        }
    }

    [MenuItem("Tools/Check Mash")]
    public static void CheckMash()
    {
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        foreach (var item in selection)
        {
            LogDebug(item.name);
            var trans = (item as GameObject).transform;
            foreach (Transform go in trans)
            {
                var mr = go.GetComponent<MeshRenderer>();
                if (mr)
                {
                    var count = mr.sharedMaterials.Length;
                    //if (count > 1)
                    LogDebug(go.name + " : " + count);
                }
            }
        }
    }

    [MenuItem("Tools/Check Same Name")]
    public static void CheckSameName()
    {
        var root = Application.dataPath;
        System.Action action = () =>
        {
            var allFiles = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories);
            var sb = new System.Text.StringBuilder();
            //LogDebug(Path.GetFileName(allFiles[0]));
            LogDebug(allFiles.Length);
            var keys = new HashSet<string>();
            foreach (var item in allFiles)
            {
                if (item.EndsWith(".meta") || item.EndsWith(".cs") || item.EndsWith(".asset") || item.EndsWith(".xml") || item.Contains(".svn"))
                    continue;
                if (!keys.Add(Path.GetFileName(item)))
                {
                    LogDebug(item);
                    sb.AppendLine(item);
                }
            }
            Mogo.Util.XMLParser.SaveText("CheckSameNameResult.txt", sb.ToString());
        };
        action.BeginInvoke(null, null);
    }

    [MenuItem("Tools/Check Legal Name")]
    public static void CheckLegalName()
    {
        var path = Application.dataPath;
        var files = GetFileList(path);

        LogDebug("Files count: " + files.Count);
        var names = new List<String>();
        foreach (var item in files)
        {
            var name = Path.GetFileName(item).ToLower();
            if (!System.Text.RegularExpressions.Regex.IsMatch(name, "^([a-zA-Z0-9_-]+).([a-zA-Z0-9]+)$"))
                names.Add(item);
        }
        LogDebug("not legal:  \n" + names.ToList().PackList('\n'));
        Mogo.Util.XMLParser.SaveText(path + "/../CheckLegalName.txt", names.ToList().PackList('\n'));

        LogDebug("finished!!!");
    }

    [MenuItem("Tools/Check Project")]
    public static void CheckProject()
    {
        GetFindScriptIngoreList();
        bool gotProblems = false;
        if (SynCheckSameName(Application.dataPath))
            gotProblems = true;
        if (FindScriptInPrefab("Assets/Resources/GUI", "Assets/Resources/Scences", "Assets/Resources/MogoUI", "Assets/Resources/GUI"))
            gotProblems = true;
        if (CheckMissingRender())
            gotProblems = true;
        if (CheckMissingMesh())
            gotProblems = true;
        if (CheckUseAnimator())
            gotProblems = true;
        if (CheckMissingAnimation())
            gotProblems = true;
        EditorUtility.DisplayDialog("CheckProject", gotProblems ? "Got proprems!" : "OK~", "ok");
    }

    #region  丢失检查（冯委）
    ///丢材质的prefab/////////////////////////////////////////////////////////////////////////////////
    [MenuItem("Tools/检查丢失材质的prefab")]
    public static bool CheckMissingRender()
    {
        var sb = new System.Text.StringBuilder();
        var projectPath = new DirectoryInfo(Application.dataPath).Parent.FullName.Replace("\\", "/") + "/";
        MissingScriptsCounter counter = new MissingScriptsCounter();
        //首先找到所有的prefab
        var root = Application.dataPath + "/Resources/Scences";//"D:/mogo/client20131009/Assets/Resources/Scences/10004_City";//Application.dataPath;
        var prefabFiles = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
        var selection = (from f in prefabFiles
                         let file = f.ReplaceFirst(projectPath, "")
                         let go = AssetDatabase.LoadAssetAtPath(file, typeof(Object))
                         where go != null
                         select go).ToArray();
        if (selection.Length == 0)
        {
            LogDebug("没有找到prefab文件，请检查目录" + root);
            return true;
        }
        foreach (var g in selection)
        {
            sb.AppendLine(FindMatInGo(g, counter));
        }
        var path = ExportScenesManager.GetFolderPath("") + "//丢材质物件列表.txt";
        Mogo.Util.XMLParser.SaveText(path.Replace("\\", "/"), sb.ToString());
        LogDebug("丢材质的物件总数" + counter.missingCount);
        return counter.missingCount != 0;
    }
    //丢材质对应的findingo**************************************
    private static string FindMatInGo(Object go, MissingScriptsCounter counter)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var g = go as GameObject;
        if (g == null)
            return null;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            var com = components[i];
            if (com is MeshRenderer)
            {
                foreach (Material ml in (com as MeshRenderer).sharedMaterials)
                {
                    if (ml == null)
                    {
                        var fullName = FindMissingScriptsRecursively.GetFullName(g);
                        counter.missingCount++;
                        sb.AppendLine(fullName);
                        LogDebug(fullName);
                    }
                }
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            sb.AppendLine(FindMatInGo(childT.gameObject, counter));
        }
        return sb.ToString();
    }

    //丢mesh的prefab//////////////////////////////////////////////////////////////////////////////////
    [MenuItem("Tools/检查丢失mesh的prefab")]
    public static bool CheckMissingMesh()
    {
        var sb = new System.Text.StringBuilder();
        var projectPath = new DirectoryInfo(Application.dataPath).Parent.FullName.Replace("\\", "/") + "/";
        MissingScriptsCounter counter = new MissingScriptsCounter();
        //首先找到所有的prefab
        var root = Application.dataPath + "/Resources/Scences";//"D:/mogo/client20131009/Assets/Resources/Scences/10004_City";//Application.dataPath;
        var prefabFiles = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
        var selection = (from f in prefabFiles
                         let file = f.ReplaceFirst(projectPath, "")
                         let go = AssetDatabase.LoadAssetAtPath(file, typeof(Object))
                         where go != null
                         select go).ToArray();
        if (selection.Length == 0)
        {
            LogDebug("没有找到prefab文件，请检查目录" + root);
            return true;
        }
        foreach (var g in selection)
        {
            sb.AppendLine(FindMeshInGo(g, counter));
        }
        var path = ExportScenesManager.GetFolderPath("") + "//丢mesh的物件列表.txt";
        Mogo.Util.XMLParser.SaveText(path.Replace("\\", "/"), sb.ToString());
        LogDebug("丢mesh的物件总数" + counter.missingCount);
        return counter.missingCount != 0;
    }
    //丢mesh对应的findingo**************************************
    private static string FindMeshInGo(Object go, MissingScriptsCounter counter)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var g = go as GameObject;
        if (g == null)
            return null;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            var com = components[i];
            if (com is MeshFilter)
            {
                Mesh ms = (com as MeshFilter).sharedMesh;
                {
                    if (ms == null)
                    {
                        var fullName = FindMissingScriptsRecursively.GetFullName(g);
                        counter.missingCount++;
                        sb.AppendLine(fullName);
                        LogDebug(fullName);
                    }
                }
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            sb.AppendLine(FindMeshInGo(childT.gameObject, counter));
        }
        return sb.ToString();
    }

    ///////检查使用animator的prefab//////////////////////////////////////////////////////////////////
    [MenuItem("Tools/检查使用animator的prefab")]
    public static bool CheckUseAnimator()
    {
        var sb = new System.Text.StringBuilder();
        var projectPath = new DirectoryInfo(Application.dataPath).Parent.FullName.Replace("\\", "/") + "/";
        MissingScriptsCounter counter = new MissingScriptsCounter();
        //首先找到所有的prefab
        var root = Application.dataPath + "/Resources/Scences";//"D:/mogo/client20131009/Assets/Resources/Scences/10004_City";//Application.dataPath;
        var prefabFiles = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
        var selection = (from f in prefabFiles
                         let file = f.ReplaceFirst(projectPath, "")
                         let go = AssetDatabase.LoadAssetAtPath(file, typeof(Object))
                         where go != null
                         select go).ToArray();
        if (selection.Length == 0)
        {
            LogDebug("没有找到prefab文件，请检查目录" + root);
            return true;
        }
        foreach (var g in selection)
        {
            sb.AppendLine(FindAnimatorInGo(g, counter));
        }
        var path = ExportScenesManager.GetFolderPath("") + "/使用了animator的物件列表.txt";
        Mogo.Util.XMLParser.SaveText(path.Replace("\\", "/"), sb.ToString());
        LogDebug("使用了animator的物件总数" + counter.missingCount);
        return counter.missingCount != 0;
    }
    //使用了animator对应的findingo*****************************
    private static string FindAnimatorInGo(Object go, MissingScriptsCounter counter)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var g = go as GameObject;
        if (g == null)
            return null;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            var com = components[i];
            if (com is Animator)
            {
                var fullName = FindMissingScriptsRecursively.GetFullName(g);
                counter.missingCount++;
                sb.AppendLine(fullName);
                LogDebug(fullName);
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            sb.AppendLine(FindAnimatorInGo(childT.gameObject, counter));
        }
        return sb.ToString();
    }

    /// /丢animation的prefab/////////////////////////////////////////////////////////////////////////
    [MenuItem("Tools/检查丢动画的prefab")]
    public static bool CheckMissingAnimation()
    {
        var sb = new System.Text.StringBuilder();
        var projectPath = new DirectoryInfo(Application.dataPath).Parent.FullName.Replace("\\", "/") + "/";
        MissingScriptsCounter counter = new MissingScriptsCounter();
        //首先找到所有的prefab
        var root = Application.dataPath + "/Resources/Scences";//"D:/mogo/client20131009/Assets/Resources/Scences/10004_City";
        //LogDebug(Application.dataPath);
        var prefabFiles = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
        var filesNoTraps = (from f in prefabFiles
                            where f.Contains("Traps") == false
                            select f).ToArray();
        var selection = (from f in filesNoTraps
                         let file = f.ReplaceFirst(projectPath, "")
                         let go = AssetDatabase.LoadAssetAtPath(file, typeof(Object))
                         where go != null
                         select go).ToArray();
        if (selection.Length == 0)
        {
            LogDebug("没有找到prefab文件，请检查目录" + root);
            return true;
        }
        foreach (var g in selection)
        {
            sb.AppendLine(FindAnimInGo(g, counter));
        }
        var path = ExportScenesManager.GetFolderPath("") + "//丢动画的物件列表.txt";
        Mogo.Util.XMLParser.SaveText(path.Replace("\\", "/"), sb.ToString());
        LogDebug("丢动画的物件总数" + counter.missingCount);
        return counter.missingCount != 0;
    }
    //丢动画对应的findingo*************************************
    private static string FindAnimInGo(Object go, MissingScriptsCounter counter)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var g = go as GameObject;
        if (g == null)
            return null;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            var com = components[i];
            if (com is Animation)
            {
                Animation acom = com as Animation;
                if (acom.clip == null)
                {
                    var fullName = FindMissingScriptsRecursively.GetFullName(g);
                    counter.missingCount++;
                    sb.AppendLine(fullName);
                    LogDebug(fullName);
                }
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            sb.AppendLine(FindAnimInGo(childT.gameObject, counter));
        }
        return sb.ToString();
    }

    //添加一个根据选中的文件生成md5的菜单
    [MenuItem("Tools/生成选中文件的MD5值")]
    public static void BuildPackageMd5()
    {
        string root = Application.dataPath;

        var file = EditorUtility.OpenFilePanel("select file", root, "");

        LogDebug(file + "的MD5值是：");
        LogDebug(Utils.BuildFileMd5(file));

        //Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

        //foreach (var item in selection)
        //{
        //    var path = root.Substring(0, root.Length - 6) + AssetDatabase.GetAssetPath(item);
        //    LogDebug(path + "的MD5值是：");
        //    LogDebug(Utils.BuildFileMd5(path));
        //}
    }
    //自动生成MD5，根据本地pkg文件夹中pkg文件生成对应MD5，同时生成或者修改version.xml和packagemd5.xml
    //这个函数需要一个配置文件md5config.xml，放在assets目录下，文件内容：
    /*
     <root>
    <VersionFile>C:/Users/fengwei/AppData/LocalLow/aiyou4399/Mogo/version3.xml</VersionFile>
    <PackageMd5>C:/Users/fengwei/AppData/LocalLow/aiyou4399/Mogo/packagemd5.xml</PackageMd5>  
    <LocalPackageDir>D:/mogo/client20131022/Assets/packages/</LocalPackageDir>
    <PkgVersionStart>1</PkgVersionStart>
    <PkgVersionEnd>18</PkgVersionEnd>
    <LocalApk>C:/Users/fengwei/AppData/LocalLow/aiyou4399/Mogo/Mogo3.apk</LocalApk>
    </root>
     */
    [MenuItem("Tools/更新apk、pkg及对应md5")]
    public static void UpdatePkgWithMd5()
    {
        Md5Update md5update = new Md5Update();
        md5update.InitConfig();
    }
    /// <summary>
    /// 更新version.xml和packagemd5.xml中对应的apkmd5值和pkg的md5
    /// </summary>
    class Md5Update
    {
        //version.xml对应的url
        string versionFile = null;
        //packagemd5.xml对应的url
        string packageMd5 = null;
        //本地package的存放目录
        string localPackageDir = null;
        //pkg文件及对应的md5值
        List<KeyValuePair<string, string>> pkgmd5 = new List<KeyValuePair<string, string>>();
        //apk对应的md5
        string apkMd5 = null;
        //pkg的最高版本
        VersionCodeInfo pkgversion = new VersionCodeInfo("0.0.0.1");
        //apk的路径
        string localApk = null;
        //获得本地配置相关数据，缓存到对象中
        public void InitConfig()
        {
            //先从配置文件中读取pkg目录、version路径、packagemd5.xml路径
            string root = (new DirectoryInfo(Application.dataPath).Parent).ToString().Replace("\\", "/") + "/md5config.xml";
            SecurityElement content = XMLParser.LoadXML(Utils.LoadFile(root));
            foreach (SecurityElement item in content.Children)
            {
                if (item.Tag == "VersionFile")
                    versionFile = item.Text;
                if (item.Tag == "PackageMd5")
                    packageMd5 = item.Text;
                if (item.Tag == "LocalPackageDir")
                    localPackageDir = item.Text;
                if (item.Tag == "LocalApk")
                    localApk = item.Text;
            }
            BuildMd5();
            LogDebug("pkg和apk的md5值更新完成");
            LogDebug("version文件位置：" + versionFile);
            LogDebug("packageMd5文件位置：" + packageMd5);
        }
        public class MyCompare : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                var px = x.Name;
                var py = y.Name;
                LogDebug("版本xx：" + px + "版本yy:" + py);
                string pxNum = px.Substring(7, px.LastIndexOf("-") - 7);
                string pyNum = py.Substring(7, py.LastIndexOf("-") - 7);
                LogDebug("版本x：" + pxNum + "版本y:" + pyNum);
                VersionCodeInfo vx = new VersionCodeInfo(pxNum);
                VersionCodeInfo vy = new VersionCodeInfo(pyNum);
                return vx.Compare(vy);
            }
        }
        //生成对应的md5
        public void BuildMd5()
        {
            MyCompare mycompare = new MyCompare();
            //找到目录中所有的pkg文件
            DirectoryInfo dir = new DirectoryInfo(localPackageDir);
            var selection = (from file in dir.GetFiles("*.pkg")
                             select file).ToList();
            selection.Sort(mycompare);
            //生成pkg对应的md5
            foreach (FileInfo pkg in selection)
            {
                GetHVersionFromPkg(pkg.Name);
                string md5 = Utils.BuildFileMd5(pkg.ToString().Replace("\\", "/"));
                pkgmd5.Add(new KeyValuePair<string, string>(pkg.Name, md5));
            }
            //生成apk对应的md5
            apkMd5 = Utils.BuildFileMd5(localApk);
            SavePkgMd5();
        }
        //从一个pkg文件获得资源较大的版本号,传人是pkg的名字，不含路径package0.0.0.2-0.0.0.3.pkg,要和pkgversion比较，大的会存储下来
        void GetHVersionFromPkg(string pkgname)
        {
            var vci = GetVersionCodeFromPKGName(pkgname);
            if (pkgversion.Compare(vci) < 0)
                pkgversion = vci;
        }

        VersionCodeInfo GetVersionCodeFromPKGName(string pkgname)
        {
            string right = pkgname.Substring(pkgname.LastIndexOf("-") + 1);
            VersionCodeInfo vci = new VersionCodeInfo(right.Substring(0, right.Length - 4));
            return vci;
        }

        //根据pkgmd5生成配置文件
        public void SavePkgMd5()
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(packageMd5, Encoding.Default);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteStartElement("pl");
            foreach (KeyValuePair<string, string> kvp in pkgmd5)
            {
                xmlWriter.WriteStartElement("p");
                string pkgname = kvp.Key.Substring(kvp.Key.LastIndexOf("/") + 1);
                xmlWriter.WriteAttributeString("n", pkgname);
                xmlWriter.WriteString(kvp.Value);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
            SavePkgVersion();
        }
        //同时修改version.xml中包的版本
        public void SavePkgVersion()
        {
            //首先定义几个version.xml中的信息
            string programVersion = null, PackageUrl = null, ApkUrl = null, PackageMd5List = null;
            //首先把version中数据读到securityelement中
            if (File.Exists(versionFile))
            {
                SecurityElement content = XMLParser.LoadXML(Utils.LoadFile(versionFile));
                foreach (SecurityElement se in content.Children)
                {
                    if (se.Tag == "ProgramVersion")
                        programVersion = se.Text;
                    if (se.Tag == "PackageUrl")
                        PackageUrl = se.Text;
                    if (se.Tag == "ApkUrl")
                        ApkUrl = se.Text;
                    if (se.Tag == "PackageMd5List")
                        PackageMd5List = se.Text;
                }
            }
            else
            {
                programVersion = "0.0.0.30";
                PackageUrl = "http://192.168.200.102/package/";
                ApkUrl = "http://192.168.200.102/Mogo.apk";
                PackageMd5List = "http://192.168.200.102/packagemd5.xml";
            }

            //再把securityelement数据保存回去,修改内容：pkg的版本号，apk的md5值
            XmlTextWriter xmlWriter = new XmlTextWriter(versionFile, Encoding.Default);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteStartElement("root");

            xmlWriter.WriteStartElement("ProgramVersion");
            xmlWriter.WriteString(programVersion);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ResouceVersion");
            xmlWriter.WriteString(pkgversion.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PackageUrl");
            xmlWriter.WriteString(PackageUrl);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ApkUrl");
            xmlWriter.WriteString(ApkUrl);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ApkMd5");
            xmlWriter.WriteString(apkMd5);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("PackageMd5List");
            xmlWriter.WriteString(PackageMd5List);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }
    }
    #endregion

    private static List<String> GetFileList(string path)
    {
        var result = new List<String>();
        var ignoreList = new List<string>() { "entries", "format", "wc.db", "ic_launcher.png" };
        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(t => !t.EndsWith(".cs") && !t.EndsWith(".meta") && !t.EndsWith(".xml") && !t.EndsWith(".svn-base")
            && !t.Contains("SpawnPointGearAgent") && !t.Contains("TrapStudio") && !t.Contains("Resources\\Default") && !t.Contains("Plugins\\Android") && !t.Contains("UITextureRGBA32")).ToList();// && !t.Contains("UITextureRGBA32")

        foreach (var item in files)
        {
            var name = Path.GetFileName(item).ToLower();
            if (ignoreList.Contains(name))
                continue;
            result.Add(item);
        }

        return result;
    }

    public static bool SynCheckSameName(string path)
    {
        bool result = false;

        var files = GetFileList(path);
        var hash = new Dictionary<string, string>();
        var sameNames = new HashSet<string>();
        foreach (var item in files)
        {
            var name = Path.GetFileName(item).ToLower();
            if (hash.ContainsKey(name))
            {
                //var path1 = "Assets" + item.ReplaceFirst(Application.dataPath, "");
                //var mat1 = AssetDatabase.LoadAssetAtPath(path1, typeof(Material)) as Material;
                //var path2 = "Assets" + hash[name].ReplaceFirst(Application.dataPath, "");
                //var mat2 = AssetDatabase.LoadAssetAtPath(path2, typeof(Material)) as Material;
                //if (mat1 && mat2)
                //{
                //    if (mat1.shader.name.ToLower().Contains("diffuse"))
                //    {
                //        AssetDatabase.DeleteAsset(path1);
                //    }
                //    else if (mat2.shader.name.ToLower().Contains("diffuse"))
                //    {
                //        AssetDatabase.DeleteAsset(path2);
                //        hash[name] = item;
                //    }
                //}
                //else
                //{
                //    LogError("mat not exist" + item + ":" + hash[name]);
                //}

                sameNames.Add(name);
                result = true;
            }
            else
            {
                hash.Add(name, item);
            }
        }
        LogDebug("Delete  \n" + sameNames.ToList().PackList('\n'));
        Mogo.Util.XMLParser.SaveText(path + "/../FindSameName.txt", sameNames.ToList().PackList('\n'));

        LogDebug("finished!!!");
        return result;
    }

    public static HashSet<string> IngoreFiles = new HashSet<string>();

    private static Object[] GetSelections(params string[] rootPaths)
    {
        var files = new List<string>();
        foreach (var rootPath in rootPaths)
        {
            files.AddRange(Directory.GetFiles(rootPath, "*.prefab", SearchOption.AllDirectories));
        }

        var projectPath = new DirectoryInfo(Application.dataPath).Parent.FullName.Replace("\\", "/");

        var selection = (from f in files
                         let file = f.ReplaceFirst(projectPath, "")
                         let go = AssetDatabase.LoadAssetAtPath(file, typeof(Object))
                         where go != null
                         select go).ToArray();
        return selection;
    }

    public static bool FindScriptInPrefab(params string[] rootPaths)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        var selection = GetSelections(rootPaths);

        var counter = new MissingScriptsCounter();
        LogDebug("selection count: " + selection.Length);

        for (counter.currentIndex = 0; counter.currentIndex < selection.Length; counter.currentIndex++)
        {
            var g = selection[counter.currentIndex];
            sb.Append(FindInGO(g, counter));
        }
        var path = ExportScenesManager.GetFolderPath("") + "//ScriptUsingInfo.txt";
        Mogo.Util.XMLParser.SaveText(path.Replace("\\", "/"), sb.ToString());
        LogDebug("Find finished, total script using is " + counter.missingCount + ", please check 'ScriptUsingInfo.txt' in project folder.");
        return counter.missingCount != 0;
    }

    private static string FindInGO(Object go, MissingScriptsCounter counter)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        var g = go as GameObject;
        if (g == null)
            return string.Empty;
        counter.goCount++;
        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            counter.componentsCount++;
            var com = components[i];
            var typeName = com.GetType().Name;
            if (com is MonoBehaviour && !ExportScenesManager.IngoreFiles.Contains(typeName))
            {
                var fullName = FindMissingScriptsRecursively.GetFullName(g);
                counter.missingCount++;
                sb.AppendLine(string.Concat(fullName, ": ", typeName));
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {
            //Mogo.Util.LoggerHelper.Debug("Searching " + childT.name  + " " );
            sb.Append(FindInGO(childT.gameObject, counter));
        }
        return sb.ToString();
    }

    public static void GetFindScriptIngoreList()
    {
        var files = Directory.GetFiles(Application.dataPath + "/Plugins", "*.cs", SearchOption.AllDirectories).ToList();
        files.AddRange(Directory.GetFiles(Application.dataPath + "/Plugins", "*.js", SearchOption.AllDirectories));
        IngoreFiles.Clear();
        foreach (var item in files)
        {
            var name = Mogo.Util.Utils.GetFileNameWithoutExtention(item.Replace('\\', '/'));
            IngoreFiles.Add(name);
        }
        LogDebug("IngoreFiles count: " + IngoreFiles.Count);
    }

    #endregion

    #region 资源测试

    [MenuItem("Assetbundle/Test Assetbundle")]
    public static void TestAssetbundle()
    {
        //var path = GetFolderPath("ExportTemp");
        ////LogDebug(AssetDatabase.LoadAssetAtPath("Assets/temp/1.prefab", typeof(object)).name);
        //BuildPipeline.PushAssetDependencies();
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/Shader/MonsterShader.shader", typeof(object)) }, new string[1] { "MonsterShader.shader" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/Shader/MonsterShader.shader" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);

        ////BuildPipeline.PushAssetDependencies();
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/22/Materials/n1013.png", typeof(object)) }, new string[1] { "n1013.png" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/22/Materials/n1013.png" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);
        //BuildPipeline.PushAssetDependencies();
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/22/Materials/n1013.mat", typeof(object)) }, new string[1] { "n1013.mat" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/22/Materials/n1013.mat" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);
        ////BuildPipeline.PushAssetDependencies();

        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/22/Materials/n1014.png", typeof(object)) }, new string[1] { "n1014.png" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/22/Materials/n1014.png" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);
        //BuildPipeline.PushAssetDependencies();
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/22/Materials/n1014.mat", typeof(object)) }, new string[1] { "n1014.mat" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/22/Materials/n1014.mat" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);
        ////BuildPipeline.PushAssetDependencies();

        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/22/Materials/n1050.png", typeof(object)) }, new string[1] { "n1050.png" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/22/Materials/n1050.png" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);
        //BuildPipeline.PushAssetDependencies();
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/22/Materials/n1050.mat", typeof(object)) }, new string[1] { "n1050.mat" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/22/Materials/n1050.mat" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);
        ////BuildPipeline.PushAssetDependencies();

        //BuildPipeline.PushAssetDependencies();
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/22/1.FBX", typeof(object)) }, new string[1] { "1.FBX" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/22/1.FBX" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);

        //BuildPipeline.PushAssetDependencies();
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/1.prefab", typeof(object)) }, new string[1] { "1.prefab" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/1.prefab" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/2.prefab", typeof(object)) }, new string[1] { "2.prefab" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/2.prefab" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);
        //BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadAssetAtPath("Assets/temp/3.prefab", typeof(object)) }, new string[1] { "3.prefab" + SystemConfig.ASSET_FILE_EXTENSION }, path + "/Assets/temp/3.prefab" + SystemConfig.ASSET_FILE_EXTENSION, options, m_currentBuildTarget);

        //BuildPipeline.PopAssetDependencies();
        //BuildPipeline.PopAssetDependencies();
        //BuildPipeline.PopAssetDependencies();
        //BuildPipeline.PopAssetDependencies();
        //BuildPipeline.PopAssetDependencies();
        //BuildPipeline.PopAssetDependencies();
        //var root = GetPath();
        //BuildMogoAssetBundleInfo(root);
        //LoggerHelper.Debug("MogoResource: " + m_assetBundleDic.Count + " m_allRecources.Count: " + m_allRecources.Count);
    }

    [MenuItem("Assetbundle/Find Same Name")]
    public static void FindSameName()
    {
        var path = Application.dataPath;
        LogDebug(path);
        System.Action action = () =>
        {
            var ignoreList = new List<string>() { "entries", "format", "wc.db", "ic_launcher.png" };
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(t => !t.EndsWith(".cs") && !t.EndsWith(".meta") && !t.EndsWith(".xml") && !t.EndsWith(".svn-base")
                && !t.Contains("SpawnPointGearAgent") && !t.Contains("TrapStudio")).ToList();

            var hash = new HashSet<string>();
            var sameNames = new HashSet<string>();
            foreach (var item in files)
            {
                var name = Path.GetFileName(item).ToLower();
                if (ignoreList.Contains(name))
                    continue;
                if (!hash.Add(name))
                {
                    sameNames.Add(name);
                }
            }


            LogDebug(sameNames.ToList().PackList('\n'));
            Mogo.Util.XMLParser.SaveText(path + "/../FindSameName.txt", sameNames.ToList().PackList('\n'));

            LogDebug("finished!!!");
        };
        action.BeginInvoke(null, null);
    }

    [MenuItem("Assetbundle/Test UI PackEx")]
    public static void TestUIPackEx()
    {
        //var sw = new Stopwatch();
        //sw.Start();
        //List<string> list = new List<string>();
        //var root = Application.dataPath + "/Resources/Characters/NPC";
        //LogDebug(root);
        //list.AddRange(Directory.GetFiles(root, "*.prefab", SearchOption.TopDirectoryOnly));
        //LogDebug(list.Count);
        //var selection = new List<Object>();
        //foreach (var item in list)
        //{
        //    var path = "Assets" + item.ReplaceFirst(Application.dataPath, "");
        //    //LogDebug(path);
        //    selection.Add(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        //}
        //LogDebug(selection.Count);
        //var res = LoadResourcesToDic(selection.ToArray());

        //LogDebug("res " + res.Count);

        //var listMogoRes = new List<MogoResourceInfo>();
        ////BuildResourceTreeEx(res[AssetDatabase.GetAssetPath(selection[0])]);
        //foreach (var item in res)
        //{
        //    var ex = item.Value.GetCopy();
        //    listMogoRes.Add(BuildResourceTreeEx(ex));
        //}
        //LogDebug("listMogoRes.Count: " + listMogoRes.Count);

        //Dictionary<string, MogoResourceInfo> m_allResources = new Dictionary<string, MogoResourceInfo>();
        //foreach (var item in listMogoRes)
        //{
        //    if (!m_allResources.ContainsKey(item.Path))
        //        m_allResources.Add(item.Path, item);
        //    else
        //        LogWarning("exist: " + item.Path);
        //    var suns = item.GetSonInfoRecursively();
        //    foreach (var sun in suns)
        //    {
        //        if (!m_allResources.ContainsKey(sun.Path))
        //            m_allResources.Add(sun.Path, sun);
        //        else
        //        {
        //            foreach (var sunParent in sun.Parents)
        //            {
        //                m_allResources[sun.Path].Parents[sunParent.Key] = sunParent.Value;
        //            }
        //        }
        //    }
        //}
        //sw.Stop();
        //LogDebug(sw.ElapsedMilliseconds);
    }

    #endregion

    #region 资源打包

    /// <summary>
    /// 打包更新的文件。
    /// </summary>
    /// <param name="saveFolder">存放文件夹。</param>
    /// <param name="fileList">更新文件列表。</param>
    /// <param name="currentVersion">当前版本号。</param>
    /// <param name="newVersion">目标版本号。</param>
    public static void PackUpdatedFiles(string saveFolder, List<string> fileList, string currentVersion, string newVersion)
    {
        var tempExport = GetFolderPath("tempExport");

        foreach (var item in fileList)
        {
            var path = item.Replace(saveFolder, "");
            var newPath = string.Concat(tempExport, path);
            var di = Path.GetDirectoryName(newPath);
            if (!Directory.Exists(di))
                Directory.CreateDirectory(di);
            if (File.Exists(newPath))
                continue;
            File.Copy(item, newPath);
        }
        ZIPFile(tempExport, currentVersion, newVersion);
        Directory.Delete(tempExport, true);
    }

    public static void ZIPFile(string sourcePath, string currentVersion, string newVersion)
    {
        var targetPath = GetFolderPath(UpdatePackage);
        ZIPFile(sourcePath, targetPath, currentVersion, newVersion);
    }

    public static void ZIPFile(string sourcePath, string targetPath, string currentVersion, string newVersion)
    {
        ZIPFileWithFileName(sourcePath, targetPath, VersionManager.Instance.GetPackageName(currentVersion, newVersion));
    }

    public static void ZIPFileWithFileName(string sourcePath, string targetPath, string fileName, int zipLevel = 5)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        LogDebug("ZIPFile sourcePath: " + sourcePath);
        LogDebug("ZIPFile targetPath: " + targetPath);
        LogDebug("ZIPFile fileName: " + fileName);
        var zipPath = Path.Combine(targetPath, fileName);
        if (File.Exists(zipPath))
            File.Delete(zipPath);
        Utils.CompressDirectory(sourcePath, zipPath, zipLevel);

        sw.Stop();
        var t = sw.ElapsedMilliseconds;
        LogDebug(t);
    }

    #endregion

    #region 新版本管理

    public static byte[] CRC16_C(byte[] data)
    {
        byte CRC16Lo;
        byte CRC16Hi;   //CRC寄存器 
        byte CL; byte CH;       //多项式码&HA001 
        byte SaveHi; byte SaveLo;
        byte[] tmpData;
        int Flag;
        CRC16Lo = 0xFF;
        CRC16Hi = 0xFF;
        CL = 0x01;
        CH = 0xA0;
        tmpData = data;
        for (int i = 0; i < tmpData.Length; i++)
        {
            CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或 
            for (Flag = 0; Flag <= 7; Flag++)
            {
                SaveHi = CRC16Hi;
                SaveLo = CRC16Lo;
                CRC16Hi = (byte)(CRC16Hi >> 1);      //高位右移一位 
                CRC16Lo = (byte)(CRC16Lo >> 1);      //低位右移一位 
                if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1 
                {
                    CRC16Lo = (byte)(CRC16Lo | 0x80);   //则低位字节右移后前面补1 
                }             //否则自动补0 
                if ((SaveLo & 0x01) == 0x01) //如果LSB为1，则与多项式码进行异或 
                {
                    CRC16Hi = (byte)(CRC16Hi ^ CH);
                    CRC16Lo = (byte)(CRC16Lo ^ CL);
                }
            }
        }
        byte[] ReturnData = new byte[2];
        ReturnData[0] = CRC16Hi;       //CRC高位 
        ReturnData[1] = CRC16Lo;       //CRC低位 
        return ReturnData;
    }

    private static VersionCodeInfo GetLastVersion()
    {
        var currentVersion = new VersionCodeInfo("0.0.0.0");
        var folders = Directory.GetDirectories(GetFolderPath(ExportPath));
        foreach (var item in folders)
        {
            var version = new VersionCodeInfo(item);
            if (version.Compare(currentVersion) > 0)
                currentVersion = version;
        }
        return currentVersion;
    }

    private static VersionCodeInfo GetUpperVersion(VersionCodeInfo currentVersion)
    {
        var nextVersion = currentVersion.GetUpperVersion();
        var targetPath = Path.Combine(ExportPath, nextVersion);
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);
        else
            LogWarning(string.Format("varsion {0} already exist.", nextVersion));
        return new VersionCodeInfo(nextVersion);
    }

    [MenuItem("MogoVersion/Show Current Version")]
    public static void ShowCurrentVersion()
    {
        var sw = new Stopwatch();
        var data = Utils.LoadByteFile(@"E:\workspace\SVN\mogo\src\branches\Integration\Export\package0.0.0.0-0.0.0.1.pkg");
        sw.Start();
        var res = Utils.FormatMD5(Utils.CreateMD5(data)); //CRC16_C(data);
        sw.Stop();
        LogDebug(sw.ElapsedMilliseconds);
        LogDebug(res);

        //var desCrypto = DESCryptoServiceProvider.Create();
        //LogDebug(desCrypto.ValidKeySize(256));
        //var key = desCrypto.Key.PackArray();
        //LogDebug(key);
        //var bytes = key.ParseListAny<byte>();
        //LogDebug(bytes.Count);
        //LogDebug(bytes.PackList());
        //var sw = new Stopwatch();
        //sw.Start();
        //var libData = Utils.LoadByteFile(Path.Combine(GetFolderPath("Export"), "MogoLib.dll"));
        //var enData = DESCrypto.Encrypt(libData, Driver.Number.ParseListAny<byte>().ToArray());
        //XMLParser.SaveBytes(Path.Combine(GetFolderPath("Export"), Driver.FileName), enData);
        //sw.Stop();
        //LogDebug(sw.ElapsedMilliseconds);
        //LogDebug("Current Version: " + GetLastVersion());
    }

    [MenuItem("MogoVersion/Make Upper Version")]
    public static void MakeUpperVersion()
    {
        //var libData = Utils.LoadByteFile(Path.Combine(GetFolderPath("Export"), Driver.FileName));
        //var deData = DESCrypto.Decrypt(libData, Driver.Number.ParseListAny<byte>().ToArray());
        //XMLParser.SaveBytes(Path.Combine(GetFolderPath("Export"), Driver.FileName + "de.dll"), deData);
        var next = GetUpperVersion(GetLastVersion());
        LogDebug("Upper Version: " + next);
    }

    [MenuItem("MogoVersion/Revert Version")]
    public static void RevertVersion()
    {
        if (!EditorUtility.DisplayDialog("Conform", "Conform Revert?", "Sure!", "No!!!!!"))
        {
            return;
        }
        BackupVersion(GetLastVersion());
    }

    public static void BackupVersion(VersionCodeInfo ver)
    {
        var targetPath = Path.Combine(ExportPath, ver.ToString());
        var bakFullName = Path.Combine(ExportPath, "bak/" + ver.ToString() + System.DateTime.Now.ToString(" yy-MM-dd-HH-mm-ss") + ".zip");
        if (!Directory.Exists(Path.Combine(ExportPath, "bak")))
        {
            Directory.CreateDirectory(Path.Combine(ExportPath, "bak"));
        }

        Utils.CompressDirectory(targetPath, bakFullName);
        Directory.Delete(targetPath, true);
    }

    /// <summary>
    /// 读取版本信息。
    /// </summary>
    /// <param name="version"></param>
    public static void LoadVersion()
    {
        m_fileVersions.Clear();
        m_updatedFiles.Clear();
        var folders = Directory.GetDirectories(GetFolderPath(ExportPath));
        foreach (var item in folders)
        {
            var fileVersions = new Dictionary<string, VersionInfo>();
            LoadVersionFile(SubVersion, Path.Combine(GetFolderPath(ExportPath), item), fileVersions);
            foreach (var version in fileVersions)
            {
                m_fileVersions[version.Key] = version.Value;
            }
        }

        LogDebug("Load Version finished. ");
    }

    public static void SaveVersion()
    {
        var curVersion = GetLastVersion().ToString();
        var curVersions = from t in m_fileVersions
                          where t.Value.Version == curVersion
                          select t;
        SaveVersionFile(SubVersion, Path.Combine(GetFolderPath(ExportPath), curVersion), curVersions);
    }

    public static void BuildVersionResourceManually(string resFolder, params string[] extentions)
    {
        var currentVersion = GetLastVersion();
        ExportScenesManager.LoadVersion();
        //var newVersion = currentVersion.GetUpperVersion();
        var versionFolder = Path.Combine(GetFolderPath(ExportPath), currentVersion.ToString());
        var folder = Path.Combine(versionFolder, SubMogoResources);//获取输出目录
        if (folder.Length == 0)
            return;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);//Selection.objects;
        var expordedFiles = new List<string>();
        expordedFiles.AddRange(ExportResources(selection, currentVersion.ToString(), folder, resFolder, extentions));//导出资源，并将重新输出的资源返回并记录

        SaveExportedFileList(expordedFiles, SubExportedFileList, versionFolder, versionFolder);//记录此次导出的文件信息。
        LogError("SaveExportedFileList time: " + sw.ElapsedMilliseconds);
        var uFiles = FindUpdatedFiles(folder);//获取有更新的资源。
        var updatedFiles = uFiles.Keys.ToList();
        LogError("FindUpdatedFiles time: " + sw.ElapsedMilliseconds);
        PackUpdatedFiles(folder, updatedFiles, currentVersion.GetLowerVersion(), currentVersion.ToString());//打包更新的文件。
        LogError("PackUpdatedFiles time: " + sw.ElapsedMilliseconds);
        LogError("Total time: " + sw.ElapsedMilliseconds);
        sw.Stop();
        SaveVersion();
    }

    [MenuItem("Mogo/Get File Extentions")]
    public static void GetFileExtentions()
    {
        var path = Application.dataPath;

        System.Action action = () =>
        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            var exts = (from f in files
                        let ext = Path.GetExtension(f).ToLower()
                        where ext != ".meta" && ext != ".cs"
                        select ext).Distinct();
            foreach (var item in exts)
            {
                LogDebug(item);
            }
        };

        action.BeginInvoke(null, null);
    }

    [MenuItem("Mogo/Build Prefab")]
    public static void BuildPrefabManually()
    {
        BuildVersionResourceManually("prefab", ".prefab");
    }

    [MenuItem("Mogo/Build Scene")]
    public static void BuildSceneManually()
    {
        BuildVersionResourceManually("scene", ".unity");
    }

    [MenuItem("Mogo/Build FBX")]
    public static void BuildFBXManually()
    {
        BuildVersionResourceManually("fbx", ".FBX");
    }

    [MenuItem("Mogo/Build exr")]
    public static void BuildExrManually()
    {
        BuildVersionResourceManually("scene", ".exr");
    }

    [MenuItem("Mogo/Build mat")]
    public static void BuildMatManually()
    {
        BuildVersionResourceManually("materials", ".mat");
    }

    [MenuItem("Mogo/Build controller")]
    public static void BuildControllerManually()
    {
        BuildVersionResourceManually("controller", ".controller");
    }

    [MenuItem("Mogo/Build Scene Unity")]
    public static void BuildSceneUnity()
    {
        var root = Application.dataPath + "/Resources/Scences";
        var af = Directory.GetFiles(root, "*.unity", SearchOption.TopDirectoryOnly);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.2"), "scene", af.ToList(), ".unity");//GetLastVersion();
    }

    [MenuItem("Mogo/Build Scene Exr")]
    public static void BuildSceneExr()
    {
        var root = Application.dataPath + "/Resources/Scences";
        var af = Directory.GetFiles(root, "*.exr", SearchOption.AllDirectories);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.3"), "scene", af.ToList(), ".exr");//GetLastVersion();
    }

    [MenuItem("Mogo/Build Scene Prefab")]
    public static void BuildScenePrefab()
    {
        var root = Application.dataPath + "/Resources/Scences";
        var af = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
        var left = new List<string>();
        foreach (var item in af)
        {
            if (!item.Contains("SpawnPointGearAgent") && !item.Contains("TrapStudio"))
                left.Add(item);
        }
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.18"), "prefab", left, ".prefab");//GetLastVersion();
    }

    [MenuItem("Mogo/Build Avatar Prefab")]
    public static void BuildAvatarPrefab()
    {
        var root = Application.dataPath + "/Resources/Characters/Avatar";
        var af = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.6"), "prefab", af.ToList(), ".prefab");//GetLastVersion();
    }

    [MenuItem("Mogo/Build Avatar Mat")]
    public static void BuildAvatarMat()
    {
        var root = Application.dataPath + "/Resources/Characters/Avatar";
        var af = Directory.GetFiles(root, "*.mat", SearchOption.AllDirectories);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.8"), "mat", af.ToList(), ".mat");//GetLastVersion();
    }

    [MenuItem("Mogo/Build Avatar FBX")]
    public static void BuildAvatarFBX()
    {
        var root = Application.dataPath + "/Resources/Characters/Avatar";
        var af = Directory.GetFiles(root, "*.FBX", SearchOption.AllDirectories);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.10"), "FBX", af.ToList(), ".FBX");//GetLastVersion();
    }

    [MenuItem("Mogo/Build NPC")]
    public static void BuildNPC()
    {
        var root = Application.dataPath + "/Resources/Characters/NPC";
        var af = Directory.GetFiles(root, "*.prefab", SearchOption.TopDirectoryOnly);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.1"), "prefab", af.ToList(), ".prefab");//GetLastVersion();
    }

    [MenuItem("Mogo/Build FX")]
    public static void BuildFX()
    {
        var root = Application.dataPath + "/Resources/FX";
        var af = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.5"), "prefab", af.ToList(), ".prefab");//GetLastVersion();
    }

    [MenuItem("Mogo/Build Item")]
    public static void BuildItem()
    {
        var root = Application.dataPath + "/Resources/Characters/Item";
        var af = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.12"), "prefab", af.ToList(), ".prefab");//GetLastVersion();
    }

    [MenuItem("Mogo/Build Gear")]
    public static void BuildGear()
    {
        var root = Application.dataPath + "/Resources/Gear";
        var af = Directory.GetFiles(root, "*.prefab", SearchOption.TopDirectoryOnly);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.13"), "prefab", af.ToList(), ".prefab");//GetLastVersion();
    }

    [MenuItem("Mogo/Build UI")]
    public static void BuildUIManually()
    {
        var uiRoot = Application.dataPath + "/Resources/GUI";
        var red = Path.Combine(GetFolderPath("ResourceDef"), "UIOrder.txt");
        var files = Utils.LoadFile(red);
        var fs = files.Split('\n');
        var firstList = new List<string>();
        for (int i = 0; i < fs.Length - 1; i++)
        {
            var fullName = uiRoot + "/" + fs[i].Trim() + ".prefab";
            //fullName = fullName.Replace("/", "\\");
            //LogDebug(File.Exists(fullName));
            if (File.Exists(fullName))
                firstList.Add(fullName);
            //LogDebug(fullName);
        }
        LogDebug(firstList.Count);
        var left = new List<string>();
        var af = Directory.GetFiles(uiRoot, "*.prefab", SearchOption.TopDirectoryOnly);
        foreach (var item in af)
        {
            var fullName = item.Replace("\\", "/");
            //LogDebug(fullName);
            //var name = Path.GetFileNameWithoutExtension(fullName);
            //LogDebug(name);
            if (firstList.Contains(fullName))
                continue;
            else
                left.Add(fullName);
        }
        LogDebug(left.Count);
        firstList.AddRange(left);
        BuildResourceManuallyEx(new VersionCodeInfo("0.0.0.16"), "prefab", firstList, ".prefab");//GetLastVersion();
        //BuildVersionResourceManually("prefab", ".prefab");
    }

    private static void BuildResourceManuallyEx(VersionCodeInfo currentVersion, string resFolder, List<string> targets, params string[] extentions)
    {
        ExportScenesManager.LoadVersion();
        //var newVersion = currentVersion.GetUpperVersion();
        var versionFolder = Path.Combine(GetFolderPath(ExportPath), currentVersion.ToString());
        var folder = Path.Combine(versionFolder, SubMogoResources);//获取输出目录
        if (folder.Length == 0)
            return;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        //Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);//Selection.objects;
        var expordedFiles = new List<string>();
        foreach (var item in targets)
        {
            var path = "Assets" + item.ReplaceFirst(Application.dataPath, "");
            LogDebug(path);
            var o = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            LogDebug(o);
            var target = folder + "/" + Path.GetFileNameWithoutExtension(item);
            if (!Directory.Exists(target))//已经创建过就不导了，暂时
                expordedFiles.AddRange(ExportResourcesEx(new Object[] { o }, currentVersion.ToString(), target, resFolder, false, extentions));//导出资源，并将重新输出的资源返回并记录
        }

        SaveExportedFileList(expordedFiles, SubExportedFileList, versionFolder, versionFolder);//记录此次导出的文件信息。
        LogError("SaveExportedFileList time: " + sw.ElapsedMilliseconds);
        //var uFiles = FindUpdatedFiles(folder);//获取有更新的资源。
        //var updatedFiles = uFiles.Keys.ToList();
        LogError("FindUpdatedFiles time: " + sw.ElapsedMilliseconds);
        LogError("PackUpdatedFiles time: " + sw.ElapsedMilliseconds);
        LogError("Total time: " + sw.ElapsedMilliseconds);
        sw.Stop();
        SaveVersion();
    }

    /// <summary>
    /// 导出资源。
    /// </summary>
    /// <param name="selection">待导出的资源。</param>
    /// <param name="parentFolder">资源存放目录。</param>
    /// <param name="folder">资源类型目录。</param>
    /// <param name="extentions">目标导出资源后缀。</param>
    /// <returns>所有导出出来的资源的路径。</returns>
    public static List<string> ExportResourcesEx(Object[] selection, string newVersion, string parentFolder, string folder, bool isPopInBuild = false, params string[] extentions)
    {
        var prefabsFolder = Path.Combine(parentFolder, folder);
        var roots = GetRootEx(selection);
        var list = new List<string>();
        //BeginBuildAssetBundles();
        foreach (var item in roots)
        {
            foreach (var extention in extentions)
            {
                if (item.Path.EndsWith(extention, System.StringComparison.OrdinalIgnoreCase))
                {
                    LogDebug("root: " + item.Path);
                    var assets = GetResourceAssets(item);
                    LogDebug("assets.Count: " + assets.Count);
                    List<Object> updatedObj = UpdateVersion(newVersion, assets);//比对资源版本，过滤出有更新的资源。
                    if (updatedObj.Count != 0)//若无更新，则跳过导出该资源
                        list.AddRange(BuildAssetBundles(parentFolder, item, isPopInBuild));

                    var path = Path.Combine(prefabsFolder, Utils.GetFileNameWithoutExtention(item.Path) + ".xml");
                    if (File.Exists(path) && updatedObj.Count == 0)//资源依赖信息存在且资源无更新，则不更新资源依赖信息
                        continue;
                    SecurityElement se = new SecurityElement("r");
                    se.AddChild(BuildResourceInfoXML(item));
                    var directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    XMLParser.SaveText(path, se.ToString());
                    LogDebug(string.Format("Build {0}: {1}", folder, item.Path));
                    break;
                }
            }
        }
        //EndBuildAssetBundles();
        LogDebug("Export resources finished.");
        return list;
    }

    /// <summary>
    /// 导出资源信息。
    /// </summary>
    /// <param name="selection">待导出的资源。</param>
    /// <param name="parentFolder">资源存放目录。</param>
    /// <param name="folder">资源类型目录。</param>
    /// <param name="extentions">目标导出资源后缀。</param>
    /// <returns>所有导出出来的资源的路径。</returns>
    public static List<string> ExportResourcesInfo(Object[] selection, string newVersion, string parentFolder, string folder, bool isPopInBuild = false, params string[] extentions)
    {
        var prefabsFolder = Path.Combine(parentFolder, folder);
        var roots = GetRootEx(selection);
        var list = new List<string>();
        //BeginBuildAssetBundles();
        foreach (var item in roots)
        {
            foreach (var extention in extentions)
            {
                if (item.Path.EndsWith(extention, System.StringComparison.OrdinalIgnoreCase))
                {
                    LogDebug("root: " + item.Path);
                    var assets = GetResourceAssets(item);
                    LogDebug("assets.Count: " + assets.Count);

                    var path = Path.Combine(prefabsFolder, Utils.GetFileNameWithoutExtention(item.Path) + ".xml");
                    SecurityElement se = new SecurityElement("r");
                    se.AddChild(BuildResourceInfoXML(item));
                    var directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    XMLParser.SaveText(path, se.ToString());
                    LogDebug(string.Format("Build {0}: {1}", folder, item.Path));
                    break;
                }
            }
        }
        //EndBuildAssetBundles();
        LogDebug("Export resources finished.");
        return list;
    }

    public static void PackManually(VersionCodeInfo currentVersion)
    {
        var versionFolder = Path.Combine(GetFolderPath(ExportPath), currentVersion.ToString());
        var folder = Path.Combine(versionFolder, SubMogoResources);//获取输出目录

        var targetPath = versionFolder + "/target";

        var folders = new DirectoryInfo(folder).GetDirectories().OrderBy(t => t.CreationTime);
        var exported = new List<string>();
        foreach (var folderPath in folders)
        {
            LogDebug("folderPath: " + folderPath);
            var files = Directory.GetFiles(folderPath.FullName, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var subPath = file.ReplaceFirst(folderPath.FullName, "");
                var target = targetPath + subPath;
                if (!File.Exists(target))
                {
                    //LogDebug("target: " + target);
                    if (!Directory.Exists(Utils.GetDirectoryName(target.Replace("\\", "/"))))
                        Directory.CreateDirectory(Utils.GetDirectoryName(target.Replace("\\", "/")));
                    File.Copy(file, target);
                    exported.Add(target);
                }
                //else
                //    LogDebug("Same File: " + subPath);
            }
        }

        PackUpdatedFiles(targetPath, exported, currentVersion.GetLowerVersion(), currentVersion.ToString());//打包更新的文件。
    }

    [MenuItem("Mogo/Pack UI")]
    public static void PackUIManually()
    {
        PackManually(new VersionCodeInfo("0.0.0.16"));//GetLastVersion();
    }

    [MenuItem("Mogo/Pack Wizard")]
    public static void PackWizard()
    {
        EditorWindow.GetWindow<PackWizard>(false, "Pack Wizard", true);
    }

    #endregion

    #region 版本管理

    [MenuItem("Mogo/Clear Resources")]
    public static void ClearResources()
    {
        if (EditorUtility.DisplayDialog("Conform", "Conform Clear?", "Sure!", "No!!!!!"))
        {
            Directory.Delete(GetFolderPath(ExportedFileList), true);
            Directory.Delete(GetFolderPath(MogoResources), true);
            Directory.Delete(GetFolderPath(PackageVersion), true);
            Directory.Delete(GetFolderPath(Version), true);
            Directory.Delete(GetFolderPath(UpdatePackage), true);
            EditorUtility.DisplayDialog("Info", "Clear finished.", "ok");
        }
    }

    [MenuItem("Mogo/Build Files Version &%b")]
    private static void BuildFilesVersion()
    {
        m_buildResourcesInfoList = LoadBuildResourcesInfo();
        m_copyResourcesInfoList = LoadCopyResourcesInfo();
        if (m_buildResourcesInfoList == null || m_copyResourcesInfoList == null)
            return;
        var files = Directory.GetFiles(GetFolderPath(Version));
        var wizard = EditorWindow.GetWindow<BuildProjectVersionWizard>(false, "Build Pro Ver.", true);
        if (files.Length != 0)
        {
            var currentVersion = new VersionCodeInfo("0.0.0.0");
            foreach (var item in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(item);
                var fileVersion = new VersionCodeInfo(fileName);
                if (fileVersion.Compare(currentVersion) > 0)
                    currentVersion = fileVersion;
            }
            wizard.currentVersion = currentVersion.ToString();
            wizard.newVersion = currentVersion.GetUpperVersion();
        }
        ExportScenesManager.LoadVersion(wizard.currentVersion);
    }

    private static void BuildResourceManually(string resFolder, params string[] extentions)
    {
        var currentVersion = new VersionCodeInfo("0.0.0.0");
        var files = Directory.GetFiles(GetFolderPath(Version));
        foreach (var item in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(item);
            var fileVersion = new VersionCodeInfo(fileName);
            if (fileVersion.Compare(currentVersion) > 0)
                currentVersion = fileVersion;
        }
        ExportScenesManager.LoadVersion(currentVersion.ToString());
        var newVersion = currentVersion.GetUpperVersion();
        var folder = GetPath();//获取输出目录
        if (folder.Length == 0)
            return;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);//Selection.objects;
        var expordedFiles = new List<string>();
        expordedFiles.AddRange(ExportResources(selection, newVersion.ToString(), folder, resFolder, extentions));//导出资源，并将重新输出的资源返回并记录

        SaveExportedFileList(expordedFiles, newVersion, folder);//记录此次导出的文件信息。
        LogError("SaveExportedFileList time: " + sw.ElapsedMilliseconds);
        var uFiles = FindUpdatedFiles(currentVersion.ToString(), newVersion, folder);//获取有更新的资源。
        var updatedFiles = uFiles.Keys.ToList();
        LogError("FindUpdatedFiles time: " + sw.ElapsedMilliseconds);
        PackUpdatedFiles(folder, updatedFiles, currentVersion.ToString(), newVersion);//打包更新的文件。
        LogError("PackUpdatedFiles time: " + sw.ElapsedMilliseconds);
        LogError("Total time: " + sw.ElapsedMilliseconds);
        sw.Stop();
        SaveVersion(newVersion);
    }

    [MenuItem("Mogo/Show Depends")]
    public static void TestDepends()
    {
        var subResources = EditorUtility.CollectDependencies(new[] { Selection.activeObject });
        //var res = AssetDatabase.GetDependencies(new string[] { AssetDatabase.GetAssetPath(Selection.activeObject) });

        var res = from s in subResources
                  let obj = AssetDatabase.GetAssetPath(s)
                  select obj;
        LogDebug(res.Distinct().ToArray().PackArray('\n'));
    }

    [MenuItem("Mogo/Show Unversion Files")]
    public static void ShowUnVersionFiles()
    {
        LoadVersion();
        LogDebug(m_fileVersions.Count);
        var subResources = AssetDatabase.GetDependencies(new string[] { AssetDatabase.GetAssetPath(Selection.activeObject) });
        var res = new List<string>();
        foreach (var item in subResources)
        {
            if (!m_fileVersions.ContainsKey(item) && !item.EndsWith(".cs"))
            {
                res.Add(item);
            }
        }
        LogDebug(res.PackList('\n'));
    }

    /// <summary>
    /// 获取项目目录下文件夹路径。
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static string GetFolderPath(string folder = "")
    {
        var root = new DirectoryInfo(Application.dataPath);
        var path = Path.Combine(root.Parent.FullName, folder).Replace("\\", "/");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }

    /// <summary>
    /// 读取版本信息。
    /// </summary>
    /// <param name="version"></param>
    public static void LoadVersion(string version)
    {
        m_fileVersions.Clear();
        m_updatedFiles.Clear();
        LoadVersionFile(version, GetFolderPath(Version), m_fileVersions);
        LogDebug("Load Version finished. " + version);
    }

    /// <summary>
    /// 比对资源版本，过滤出有更新的资源。
    /// </summary>
    /// <param name="version">目标版本。</param>
    /// <param name="selection">待过滤的资源。</param>
    /// <returns>过滤后的资源。</returns>
    public static List<Object> UpdateVersion(string version, List<Object> selection)
    {
        var root = Application.dataPath.Replace("Assets", "");
        var updatedFilesAsset = new List<Object>();
        foreach (var go in selection)
        {
            var path = AssetDatabase.GetAssetPath(go);
            if (IsIngoreResource(path))
                continue;
            var fileName = string.Concat(root, path);
            if (File.Exists(fileName))
            {
                var md5 = GetFileMD5(fileName);
                if (m_fileVersions.ContainsKey(path) && m_fileVersions[path].MD5 == md5)
                    continue;//没有变化的文件忽略
                var vi = new VersionInfo() { Path = path, MD5 = md5, Version = version, Asset = go };
                m_fileVersions[path] = vi;
                m_updatedFiles[path] = vi;
                updatedFilesAsset.Add(go);
            }
            else
            {
                LogWarning("file not exist: " + fileName);
            }
        }

        LogDebug("updatedFilesAsset count: " + updatedFilesAsset.Count);
        return updatedFilesAsset;
    }

    private static Dictionary<string, VersionInfo> UpdatePackageVersion(string version, string resourcePath, Dictionary<string, VersionInfo> versions)
    {
        var result = new Dictionary<string, VersionInfo>();
        var allFiles = Directory.GetFiles(resourcePath, "*.*", SearchOption.AllDirectories);
        foreach (var path in allFiles)
        {
            var md5 = GetFileMD5(path);
            if (versions.ContainsKey(path) && versions[path].MD5 == md5)
                continue;//没有变化的文件忽略
            var vi = new VersionInfo() { Path = path, MD5 = md5, Version = version };
            versions[path] = vi;
            if (result.ContainsKey(path))
                LogWarning("Same Key: " + path);
            else
                result.Add(path, vi);
        }
        return result;
    }

    public static void SaveVersion(string version)
    {
        SaveVersionFile(version, GetFolderPath(Version), m_fileVersions);
    }

    /// <summary>
    /// 获取有更新的资源。
    /// </summary>
    /// <param name="currentVersion">当前资源版本。</param>
    /// <param name="newVersion">目标资源版本。</param>
    /// <param name="sourcePath">资源根目录。</param>
    /// <returns>有更新的资源信息。</returns>
    private static Dictionary<string, VersionInfo> FindUpdatedFiles(string currentVersion, string newVersion, string sourcePath)
    {
        var versions = new Dictionary<string, VersionInfo>();
        var path = GetFolderPath(PackageVersion);
        //var folder = sourcePath.Replace("/", "\\");
        LoadVersionFile(currentVersion, path, versions, sourcePath);
        var result = UpdatePackageVersion(newVersion, sourcePath, versions);
        LogDebug("Update file count: " + result.Count);
        SaveVersionFile(newVersion, path, versions, sourcePath);

        return result;
    }

    /// <summary>
    /// 获取有更新的资源。
    /// </summary>
    /// <param name="currentVersion">当前资源版本。</param>
    /// <param name="newVersion">目标资源版本。</param>
    /// <param name="sourcePath">资源根目录。</param>
    /// <returns>有更新的资源信息。</returns>
    private static Dictionary<string, VersionInfo> FindUpdatedFiles(string sourcePath)
    {
        var versions = new Dictionary<string, VersionInfo>();
        var currentVersion = GetLastVersion().ToString();
        var versionFolder = Path.Combine(GetFolderPath(ExportPath), currentVersion);
        var resourceFolder = Path.Combine(versionFolder, SubMogoResources);
        LoadVersionFile(SubPackageVersion, versionFolder, versions, sourcePath);
        var result = UpdatePackageVersion(SubPackageVersion, resourceFolder, versions);
        LogDebug("Update file count: " + result.Count);
        SaveVersionFile(SubPackageVersion, versionFolder, versions, sourcePath);

        return result;
    }

    public static void LoadVersionFile(string fileName, string path, Dictionary<string, VersionInfo> versions, string root = "")
    {
        var fileVersion = Path.Combine(path, fileName) + ".txt";
        if (File.Exists(fileVersion))
        {
            var filesInfo = Utils.LoadFile(fileVersion);
            var files = filesInfo.Split('\n');
            var total = files.Length - 1;
            for (int i = 0; i < total; i += 3)
            {
                var v = new VersionInfo() { Path = string.Concat(root, files[i]), MD5 = files[i + 1], Version = files[i + 2] };
                versions.Add(v.Path, v);
            }
        }
    }

    public static void SaveVersionFile(string fileName, string path, IEnumerable<KeyValuePair<string, VersionInfo>> versions, string root = "")
    {
        var sb = new StringBuilder();
        foreach (var item in versions)
        {
            string tempPath = string.IsNullOrEmpty(root) ? item.Value.Path : item.Value.Path.Replace(root, "");
            sb.Append(tempPath);
            sb.Append("\n");
            sb.Append(item.Value.MD5);
            sb.Append("\n");
            sb.Append(item.Value.Version);
            sb.Append("\n");
        }
        var fileVersion = Path.Combine(path, fileName) + ".txt";
        XMLParser.SaveText(fileVersion.Replace("\\", "/"), sb.ToString());
        LogDebug("Save Version finished. " + fileVersion + " " + path);
    }

    public static List<BuildResourcesInfo> LoadBuildResourcesInfoFromPath(string path)
    {
        //LogDebug(path);
        var xml = XMLParser.LoadXML(Utils.LoadFile(path));
        if (xml == null)
        {
            EditorUtility.DisplayDialog("Error", "Load Build Resources Info Error.", "ok");
            return null;
        }
        var result = new List<BuildResourcesInfo>();

        foreach (SecurityElement item in xml.Children)
        {
            var info = new BuildResourcesInfo();
            info.check = true;
            info.name = (item.Children[0] as SecurityElement).Text;
            info.type = (item.Children[1] as SecurityElement).Text;
            info.packLevel = (item.Children[2] as SecurityElement).Text.Split(' ');
            info.isMerge = int.Parse((item.Children[3] as SecurityElement).Text);
            info.isPopInBuild = bool.Parse((item.Children[4] as SecurityElement).Text);
            info.extentions = (item.Children[5] as SecurityElement).Text.Split(' ');
            var folders = item.Children[6] as SecurityElement;
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
    /// 获取输出资源信息,添加一个参数xmlindex,来代替ForBuild.xml，不同的索引对应不同文件，默认为0，也就是用默认的ForBuild.xml
    /// </summary>
    /// <returns></returns>
    public static List<BuildResourcesInfo> LoadBuildResourcesInfo(int xmlindex = 0)
    {
        string path = string.Concat(GetFolderPath("ResourceDef"), "\\ForBuild.xml").PathNormalize(); ;
        switch (xmlindex)
        {
            case 0:
                {
                    //默认值
                    path = string.Concat(GetFolderPath("ResourceDef"), "\\ForBuild.xml").PathNormalize();
                    break;
                }
            case 1:
                {
                    path = string.Concat(GetFolderPath("ResourceDef"), "\\1.xml").PathNormalize();
                    break;
                }
            case 2:
                {
                    path = string.Concat(GetFolderPath("ResourceDef"), "\\2.xml").PathNormalize();
                    break;
                }
            case 3:
                {
                    path = string.Concat(GetFolderPath("ResourceDef"), "\\3.xml").PathNormalize();
                    break;
                }
            case 4:
                {
                    path = string.Concat(GetFolderPath("ResourceDef"), "\\4.xml").PathNormalize();
                    break;
                }
        }

        //LogDebug(path);
        var xml = XMLParser.LoadXML(Utils.LoadFile(path));
        if (xml == null)
        {
            EditorUtility.DisplayDialog("Error", "Load Build Resources Info Error.", "ok");
            return null;
        }
        var result = new List<BuildResourcesInfo>();

        foreach (SecurityElement item in xml.Children)
        {
            var info = new BuildResourcesInfo();
            info.check = true;
            info.name = (item.Children[0] as SecurityElement).Text;
            info.type = (item.Children[1] as SecurityElement).Text;
            info.packLevel = (item.Children[2] as SecurityElement).Text.Split(' ');
            info.isMerge = int.Parse((item.Children[3] as SecurityElement).Text);
            info.isPopInBuild = bool.Parse((item.Children[4] as SecurityElement).Text);
            info.extentions = (item.Children[5] as SecurityElement).Text.Split(' ');
            var folders = item.Children[6] as SecurityElement;
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
        var path = string.Concat(GetFolderPath("ResourceDef"), "\\ForCopy.xml").PathNormalize();
        //LogDebug(path);
        var xml = XMLParser.LoadXML(Utils.LoadFile(path));
        if (xml == null)
        {
            EditorUtility.DisplayDialog("Error", "Load Copy Resources Info Error.", "ok");
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

    public static string GetFileMD5(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                return System.BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
            }
        }
    }

    /// <summary>
    /// 记录此次导出的文件信息。
    /// </summary>
    /// <param name="list">文件路径信息。</param>
    /// <param name="fileName">文件名。</param>
    /// <param name="folderPath">资源存放目录，用于替换掉文件路径的根目录信息，只保存相对目录。</param>
    private static void SaveExportedFileList(List<string> list, string fileName, string folderPath)
    {
        SaveExportedFileList(list, fileName, folderPath, GetFolderPath(ExportedFileList));
    }

    /// <summary>
    /// 记录此次导出的文件信息。
    /// </summary>
    /// <param name="list">文件路径信息。</param>
    /// <param name="fileName">文件名。</param>
    /// <param name="folderPath">资源存放目录，用于替换掉文件路径的根目录信息，只保存相对目录。</param>
    public static void SaveExportedFileList(List<string> list, string fileName, string folderPath, string fileFolder)
    {
        var sb = new StringBuilder();
        foreach (var item in list)
        {
            sb.Append(item.Replace(folderPath, ""));
            sb.Append("\n");
        }
        var fileVersion = Path.Combine(fileFolder, fileName) + ".txt";
        fileVersion = fileVersion.Replace("\\", "/");
        if (!File.Exists(fileVersion))
        {
            XMLParser.SaveText(fileVersion, sb.ToString());
            LogDebug("Save ExportedFileList finished. " + fileVersion);
        }
        else
        {
            var text = Utils.LoadFile(fileVersion);
            XMLParser.SaveText(fileVersion, text + "\n" + sb.ToString());
            LogDebug("File exist: " + fileVersion);
        }
    }

    #endregion

    #region 根据类型导出资源

    /// <summary>
    /// 自动导出资源，根据选定的目录与资源类型进行打包
    /// </summary>
    /// <param name="currentVersion"></param>
    /// <param name="newVersion"></param>
    public static void Export(string currentVersion, string newVersion)
    {
        var folder = GetPath();//获取输出目录
        if (folder.Length == 0)
            return;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var shader = GetFromRoot<Object>("Shader");//获取Shader，此为公用资源，为了方便，直接每次都获取
        LogError("Get shader time: " + sw.ElapsedMilliseconds);
        var expordedFiles = new List<string>();

        foreach (var res in m_buildResourcesInfoList)//遍历导出资源配置信息
        {
            if (!res.check)//没被选中的跳过
                continue;
            var objs = new List<Object>();
            foreach (var f in res.folders)//遍历此类资源所找目录
            {
                if (!f.check)//没被选中的跳过
                    continue;
                var list = GetFromRoot<Object>(f.path, f.deep, res.extentions);
                objs.AddRange(list);
            }
            //foreach (var item in objs)
            //{
            //    LogWarning(item.name);
            //}
            objs.AddRange(shader);
            LogError(string.Format("Get {0} time: {1}", res.type, sw.ElapsedMilliseconds));

            expordedFiles.AddRange(ExportResources(objs.ToArray(), newVersion, folder, res.type, res.extentions));//导出资源，并将重新输出的资源返回并记录
            LogError(string.Format("Export {0} time: {1}", res.type, sw.ElapsedMilliseconds));
        }

        foreach (var res in m_copyResourcesInfoList)
        {
            if (!res.check)
                continue;
            CopyFolder(Path.Combine(folder, res.targetPath), Application.dataPath + res.sourcePath, res.extention);
        }
        LogError("Copy config time: " + sw.ElapsedMilliseconds);

        var lib = new FileInfo(Path.Combine(GetFolderPath("Export"), "MogoLib.dll"));
        lib.CopyTo(Path.Combine(folder, "MogoLib.dll"), true);

        SaveExportedFileList(expordedFiles, newVersion, folder);//记录此次导出的文件信息。
        LogError("SaveExportedFileList time: " + sw.ElapsedMilliseconds);
        var files = FindUpdatedFiles(currentVersion, newVersion, folder);//获取有更新的资源。
        var updatedFiles = files.Keys.ToList();
        LogError("FindUpdatedFiles time: " + sw.ElapsedMilliseconds);
        PackUpdatedFiles(folder, updatedFiles, currentVersion, newVersion);//打包更新的文件。
        LogError("PackUpdatedFiles time: " + sw.ElapsedMilliseconds);
        LogError("Total time: " + sw.ElapsedMilliseconds);
        sw.Stop();
        SaveVersion(newVersion);
    }

    /// <summary>
    /// 根据路径获取目录下对应类型的所有资源以及所有依赖。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="root">相对路径</param>
    /// <param name="deep">是否遍历子目录</param>
    /// <returns></returns>
    public static List<Object> GetFromRoot<T>(string root, bool deep = true, params string[] extentions) where T : Object
    {
        var result = GetAtPath<T>(Path.Combine(Application.dataPath, root), deep, extentions);
        LogDebug(root + ": " + result.Count);
        return result;
    }

    /// <summary>
    /// 根据路径获取目录下对应类型的所有资源以及所有依赖。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">绝对路径</param>
    /// <param name="deep">是否遍历子目录</param>
    /// <returns></returns>
    public static List<Object> GetAtPath<T>(string path, bool deep = true, params string[] extentions) where T : Object
    {
        var al = new List<Object>();
        if (!Directory.Exists(path))
            return al;
        string[] fileEntries = Directory.GetFiles(path);
        string localPath = "Assets" + path.Replace(Application.dataPath, "").Replace("/", "\\");
        foreach (string fileName in fileEntries)
        {
            if (fileName.EndsWith(".meta"))
                continue;
            var isIngore = true;
            foreach (var ext in extentions)
            {
                if (fileName.EndsWith(ext, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    isIngore = false;
                    break;
                }
            }
            if (isIngore)
                continue;
            var name = Path.Combine(localPath, Path.GetFileName(fileName));
            T t = (T)AssetDatabase.LoadAssetAtPath(name, typeof(T));
            if (t)
                al.Add(t);
        }

        if (deep)
        {
            var dics = Directory.GetDirectories(path);
            foreach (var item in dics)
            {
                al.AddRange(GetAtPath<T>(item));
            }
        }

        return al;
    }

    private static void ExportResources(string parentFolder, string folder, params string[] extentions)
    {
        Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        ExportResources(selection, "0.0.0.0", parentFolder, folder, extentions);
    }

    private static List<Object> GetResourceAssets(MogoResourceInfo info)
    {
        var result = new List<Object>();
        result.Add(info.Asset);
        foreach (var item in info.SubResource)
        {
            result.AddRange(GetResourceAssets(item.Value));
        }
        return result;
    }

    /// <summary>
    /// 导出资源。
    /// </summary>
    /// <param name="selection">待导出的资源。</param>
    /// <param name="parentFolder">资源存放目录。</param>
    /// <param name="folder">资源类型目录。</param>
    /// <param name="extentions">目标导出资源后缀。</param>
    /// <returns>所有导出出来的资源的路径。</returns>
    private static List<string> ExportResources(Object[] selection, string newVersion, string parentFolder, string folder, params string[] extentions)
    {
        var prefabsFolder = Path.Combine(parentFolder, folder);
        var roots = GetRootEx(selection);
        var list = new List<string>();
        //BeginBuildAssetBundles();
        foreach (var item in roots)
        {
            foreach (var extention in extentions)
            {
                if (item.Path.EndsWith(extention, System.StringComparison.OrdinalIgnoreCase))
                {
                    LogDebug("root: " + item.Path);
                    var assets = GetResourceAssets(item);
                    LogDebug("assets.Count: " + assets.Count);
                    List<Object> updatedObj = UpdateVersion(newVersion, assets);//比对资源版本，过滤出有更新的资源。
                    if (updatedObj.Count != 0)//若无更新，则跳过导出该资源
                        list.AddRange(BuildAssetBundles(parentFolder, item));

                    var path = Path.Combine(prefabsFolder, Utils.GetFileNameWithoutExtention(item.Path) + ".xml");
                    if (File.Exists(path) && updatedObj.Count == 0)//资源依赖信息存在且资源无更新，则不更新资源依赖信息
                        continue;
                    SecurityElement se = new SecurityElement("r");
                    se.AddChild(BuildResourceInfoXML(item));
                    var directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                    XMLParser.SaveText(path, se.ToString());
                    LogDebug(string.Format("Build {0}: {1}", folder, item.Path));
                    break;
                }
            }
        }
        //EndBuildAssetBundles();
        LogDebug("Export resources finished.");
        return list;
    }

    /// <summary>
    /// 拷贝目录资源。
    /// </summary>
    /// <param name="targetPath"></param>
    /// <param name="sourcePath"></param>
    /// <param name="extention"></param>
    public static void CopyFolder(string targetPath, string sourcePath, string extention)
    {
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);
        var files = Directory.GetFiles(sourcePath);
        foreach (var item in files)
        {
            if (item.EndsWith(extention, System.StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(item, Path.Combine(targetPath, Path.GetFileName(item)), true);
            }
        }
    }

    #endregion

    #region 资源导出逻辑

    /// <summary>
    /// 用户选择资源输出目录
    /// </summary>
    /// <returns></returns>
    private static string GetPath()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath);
        var path = new DirectoryInfo(Path.Combine(dir.Parent.FullName, MogoResources));
        if (!path.Exists)
            path.Create();
        return EditorUtility.SaveFolderPanel("Select folder", path.Parent.FullName, path.Name);
    }

    /// <summary>
    /// 构造资源树型结构
    /// </summary>
    /// <param name="resource">资源根节点</param>
    /// <returns>带子节点的树（有的话）</returns>
    private static MogoResourceInfo BuildResourceTreeEx(MogoResourceInfo resource)
    {
        var subResources = AssetDatabase.GetDependencies(new string[] { resource.Path });
        if (subResources.Length <= 1)//一条数据时是只包含自己
            return resource;
        //LogDebug("subResources.Length: " + resource.MogoResourceInfo.Path + subResources.Length);
        var resDic = new Dictionary<string, MogoResourceInfo>();
        for (int i = 0; i < subResources.Length; i++)
        {
            var subPath = subResources[i];
            //空字符代表已经被移除，还有在字符串为自身和资源为忽略资源时跳过
            if (string.IsNullOrEmpty(subPath) || subPath == resource.Path || IsIngoreResource(subPath))
                continue;

            //构造资源对象
            MogoResourceInfo info;
            if (!m_leftResources.ContainsKey(subPath))
            {
                info = new MogoResourceInfo();
                info.Path = subPath;
                info.Asset = AssetDatabase.LoadAssetAtPath(subPath, typeof(Object));
                m_leftResources.Add(subPath, info);
            }
            else
            {
                info = m_leftResources[subPath];
            }
            var ex = info.GetCopy();

            var res = BuildResourceTreeEx(ex);//构造子节点的子树
            resDic[subPath] = res;//缓存子树数据
            var list = res.GetSonRecursively();//获取子节点所有节点信息（即所有子孙）

            foreach (var item in list)//在父的工作列表中删除自己的子孙
            {
                if (item == subPath)
                    continue;

                for (int j = 0; j < subResources.Length; j++)
                {
                    if (subResources[j] == item)
                    {
                        subResources[j] = string.Empty;
                        break;
                    }
                }
            }
        }

        foreach (var item in subResources)
        {
            //空字符代表已经被移除，还有在字符串为自身和资源为忽略资源时跳过
            if (string.IsNullOrEmpty(item) || item == resource.Path || IsIngoreResource(item))
            {
                continue;
            }
            //剩下的为直属孩子资源
            resDic[item].Parents.Add(resource.Path, resource);
            resource.SubResource.Add(item, resDic[item]);
        }
        return resource;
    }

    private static List<MogoResourceInfo> GetRootEx(Object[] selection)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        m_allResources.Clear();
        m_leftResources.Clear();
        var res = LoadResourcesToDic(selection);

        LogDebug("res " + res.Count);

        var list = new List<MogoResourceInfo>();
        foreach (var item in res)
        {
            var ex = item.Value.GetCopy();
            list.Add(BuildResourceTreeEx(ex));
        }

        //foreach (var item in list)
        //{
        //    LogDebug(item.Print());
        //}
        var roots = new List<MogoResourceInfo>();
        foreach (var item in list)//找出所有根资源
        {
            if (File.Exists(item.Path))//不是文件不打包
            {
                roots.Add(item);
            }
        }
        LogDebug("Find root..." + roots.Count);
        sw.Stop();
        LogDebug("GetRootEx time..." + sw.ElapsedMilliseconds);
        return roots;
    }

    private static List<MogoResourceInfo> GetRoot(Object[] selection)
    {
        m_allResources.Clear();
        m_leftResources.Clear();
        m_allResources = LoadResourcesToDic(selection);
        LogDebug("LoadResourcesToDic: " + m_allResources.Count);

        foreach (var item in m_allResources)//构造树型结构
        {
            BuildResourcesTree(item.Value, 0);
        }
        LogDebug("BuildResourcesTree...");

        LogDebug("Left resources: " + m_leftResources.Count);
        foreach (var item in m_leftResources)
        {
            LogDebug(item.Key);
            m_allResources.Add(item.Key, item.Value);
        }

        m_deleteQueue.Clear();
        foreach (var item in m_allResources)//标识出需删除的多余的父子和祖先关系
        {
            foreach (var parent in item.Value.Parents)
            {
                RemoveInAncestor(item.Value, parent.Value);
            }
        }
        LogDebug("RemoveInAncestor...");

        LogDebug("deleteQueue: " + m_deleteQueue.Count);
        while (m_deleteQueue.Count != 0)//执行删除多余的父子和祖先关系的删除操作
        {
            var del = m_deleteQueue.Dequeue();
            del.Key.Remove(del.Value);
        }
        LogDebug("DeleteInAncestor...");

        var roots = new List<MogoResourceInfo>();
        foreach (var item in m_allResources)//找出所有根资源
        {
            if (item.Value.Parents.Count == 0)
            {
                if (File.Exists(item.Key))//不是文件不打包
                {
                    //if (item.Value.SubResource.Count != 0)
                    //{
                    //LogDebug(item);
                    roots.Add(item.Value);
                }
                //}
            }
        }
        LogDebug("Find root..." + roots.Count);

        return roots;
    }

    private static List<MogoResourceInfo> GetRoot()
    {
        Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        return GetRoot(selection);
    }

    /// <summary>
    /// 构造资源信息配置。
    /// </summary>
    /// <param name="info">资源信息实例。</param>
    /// <returns>资源信息配置。</returns>
    private static SecurityElement BuildResourceInfoXML(MogoResourceInfo info)
    {
        SecurityElement se = new SecurityElement("k");
        se.AddChild(new SecurityElement("p", info.Path));
        foreach (var item in info.SubResource.Values)
        {
            var child = BuildResourceInfoXML(item);
            se.AddChild(child);
        }
        return se;
    }

    /// <summary>
    /// 初始化资源实体。
    /// </summary>
    /// <param name="selection"></param>
    private static Dictionary<string, MogoResourceInfo> LoadResourcesToDic(Object[] selection)
    {
        Dictionary<string, MogoResourceInfo> allResources = new Dictionary<string, MogoResourceInfo>();
        foreach (var go in selection)
        {
            MogoResourceInfo info = new MogoResourceInfo();
            info.Path = AssetDatabase.GetAssetPath(go);//.Replace("Assets/", "");
            info.Asset = go;
            if (allResources.ContainsKey(info.Path))
                LogWarning("Info exist: " + info.Path);
            else
                allResources.Add(info.Path, info);
        }
        return allResources;
    }

    /// <summary>
    /// 构造资源树。
    /// </summary>
    /// <param name="resource">资源节点实例</param>
    /// <param name="deep">资源关系层次</param>
    private static void BuildResourcesTree(MogoResourceInfo resource, int deep)
    {
        var subResources = AssetDatabase.GetDependencies(new string[] { resource.Path });
        if (subResources.Length > 1)
        {
            ++deep;
            //LogDebug(path);
            foreach (var subPath in subResources)
            {
                if (subPath == resource.Path || IsIngoreResource(subPath))
                    continue;
                if (m_allResources.ContainsKey(subPath))
                {
                    var sub = m_allResources[subPath];
                    sub.Deep = sub.Deep < deep ? deep : sub.Deep;
                    if (!resource.SubResource.ContainsKey(subPath))
                    {
                        resource.SubResource.Add(subPath, sub);//加儿子
                        sub.Parents[resource.Path] = resource;//加父亲
                    }
                    BuildResourcesTree(sub, deep);
                }
                else
                {
                    //LogWarning("Can not find resource: " + subPath + " in " + resource.Path);
                    if (!m_leftResources.ContainsKey(subPath))
                    {
                        MogoResourceInfo info = new MogoResourceInfo();
                        info.Path = subPath;//.Replace("Assets/", "");
                        info.Asset = AssetDatabase.LoadAssetAtPath(subPath, typeof(Object));
                        info.Parents = new Dictionary<string, MogoResourceInfo>();
                        info.SubResource = new Dictionary<string, MogoResourceInfo>();
                        info.Deep = info.Deep < deep ? deep : info.Deep;
                        if (!resource.SubResource.ContainsKey(subPath))
                        {
                            resource.SubResource.Add(subPath, info);//加儿子
                            info.Parents[resource.Path] = resource;//加父亲
                        }
                        m_leftResources.Add(subPath, info);
                        BuildResourcesTree(info, deep);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 判断该资源是否不独立导出
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool IsIngoreResource(string path)
    {
        var filter = new List<string>() { ".cs" };
        foreach (var item in filter)
        {
            if (path.EndsWith(item, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 去除祖先子中多余的资源关系。
    /// </summary>
    /// <param name="self">自身资源实例</param>
    /// <param name="parent">父亲资源实例</param>
    private static void RemoveInAncestor(MogoResourceInfo self, MogoResourceInfo parent)
    {
        if (parent != null && parent.Parents != null)
        {
            foreach (var ancestor in parent.Parents)
            {
                if (ancestor.Value.SubResource.ContainsKey(self.Path))
                {
                    m_deleteQueue.Enqueue(new KeyValuePair<Dictionary<string, MogoResourceInfo>, string>(ancestor.Value.SubResource, self.Path));
                    if (self.Parents.ContainsKey(ancestor.Key))
                        m_deleteQueue.Enqueue(new KeyValuePair<Dictionary<string, MogoResourceInfo>, string>(self.Parents, ancestor.Key));
                }
                RemoveInAncestor(self, ancestor.Value);
            }
        }
    }

    /// <summary>
    /// 打印资源信息。
    /// </summary>
    /// <param name="info"></param>
    private static void PrintAll(MogoResourceInfo info)
    {
        StringBuilder s = new StringBuilder();
        s.Append(Path.GetFileName(info.Path) + " " + info.Deep + "  " + info.Parents.Count + "  " + info.SubResource.Count);
        foreach (var item in info.Parents)
        {
            s.Append(" " + Path.GetFileName(item.Key));
        }
        LogDebug(s.ToString());
        foreach (var item in info.SubResource)
        {
            PrintAll(item.Value);
        }
    }

    /// <summary>
    /// 用于导出时记录已经入栈的文件。
    /// </summary>
    private static HashSet<string> stackFile = new HashSet<string>();
    /// <summary>
    /// 记录管线堆栈入栈深度，用于出栈计数。
    /// </summary>
    private static int currentStackDeep;

    /// <summary>
    /// 开始导出一批资源，初始化资源。
    /// </summary>
    private static void BeginBuildAssetBundles()
    {
        m_resourceStack.Clear();
        stackFile.Clear();
        currentStackDeep = 0;
    }

    /// <summary>
    /// 导出一批资源结束，退出资源管线堆栈。
    /// </summary>
    private static void EndBuildAssetBundles()
    {
        LogError("stackFile count: " + stackFile.Count);
        while (currentStackDeep > 0)
        {
            BuildPipeline.PopAssetDependencies();
            popCount++;
            currentStackDeep--;
        }
        BuildPipeline.PopAssetDependencies();
        popCount++;
    }

    [MenuItem("Mogo/Show PackTest")]
    private static void PackTest()
    {
        System.IO.Directory.CreateDirectory("AssetBundles");
        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadMainAssetAtPath("Assets/Characters/1001/Textures/blade04 copy.png") },
            new string[1] { "blade04 copy" }, "AssetBundles/blade04 copy.png", options, m_currentBuildTarget);
        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadMainAssetAtPath("Assets/Characters/1001/Materials/blade04 copy.mat") },
            new string[1] { "blade04 copy" }, "AssetBundles/blade04 copy.mat", options, m_currentBuildTarget);
        BuildPipeline.PopAssetDependencies();

        BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadMainAssetAtPath("Assets/Characters/1001/Textures/body_22.png") },
            new string[1] { "body_22" }, "AssetBundles/body_22.png", options, m_currentBuildTarget);
        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadMainAssetAtPath("Assets/Characters/1001/Materials/body_22.mat") },
            new string[1] { "body_22" }, "AssetBundles/body_22.mat", options, m_currentBuildTarget);
        BuildPipeline.PopAssetDependencies();

        BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadMainAssetAtPath("Assets/Characters/1001/model/Materials/01 - Defaultdf.mat") },
            new string[1] { "01 - Defaultdf" }, "AssetBundles/01 - Defaultdf.mat", options, m_currentBuildTarget);
        BuildPipeline.PushAssetDependencies();
        BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadMainAssetAtPath("Assets/Characters/1001/model/blade_girl.FBX") },
            new string[1] { "blade_girl" }, "AssetBundles/blade_girl.FBX", options, m_currentBuildTarget);
        BuildPipeline.PushAssetDependencies();

        BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { AssetDatabase.LoadMainAssetAtPath("Assets/Characters/1001/blade_girl.prefab") },
          new string[1] { "blade_girl" }, "AssetBundles/blade_girl.prefab", options, m_currentBuildTarget);
        BuildPipeline.PopAssetDependencies();
        BuildPipeline.PopAssetDependencies();
        BuildPipeline.PopAssetDependencies();

    }

    public static int pushCount;
    public static int popCount;

    /// <summary>
    /// 生成资源文件
    /// </summary>
    /// <param name="saveFolder">资源保存路径</param>
    /// <param name="info">资源实例</param>
    private static List<string> BuildAssetBundles(string saveFolder, MogoResourceInfo info, bool isPopInBuild = false)
    {
        BeginBuildAssetBundles();
        PushToStack(info, 0);
        List<string> exportedList = new List<string>();
        bool isFirstRes = true;
        int lastDeep = -1;
        BuildPipeline.PushAssetDependencies();
        pushCount++;
        while (m_resourceStack.Count != 0)
        {
            var resource = m_resourceStack.Pop();
            var path = Path.Combine(saveFolder, string.Concat(resource.Key.Path, SystemConfig.ASSET_FILE_EXTENSION));//.unity3d
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (isFirstRes)
            {
                lastDeep = resource.Value;
                isFirstRes = false;
            }
            var deepDistance = lastDeep - resource.Value;
            if (deepDistance == 1)//父子关系
            {
                //LogDebug(string.Concat("BuildAssetBundles: PushAssetDependencies parent", lastDeep, " ", resource.Value));
                BuildPipeline.PushAssetDependencies();
                pushCount++;
                currentStackDeep++;
            }
            else if (deepDistance == 0)//同级关系
            {
            }
            else//子树结束
            {
                if (isPopInBuild)
                {
                    LogDebug(string.Concat("BuildAssetBundles: PopAssetDependencies end", lastDeep, " ", resource.Value));
                    BuildPipeline.PopAssetDependencies();
                    popCount++;
                    currentStackDeep--;
                }
            }
            lastDeep = resource.Value;

            if (resource.Key.Path.EndsWith(".unity"))
            {
                var scene = BuildPipeline.BuildStreamedSceneAssetBundle(new string[1] { resource.Key.Path }, path, m_currentBuildTarget);
                LogDebug("BuildStreamedSceneAssetBundle: " + scene);
                BuildPipeline.PushAssetDependencies();
                currentStackDeep++;
            }
            else
            {
                var res = BuildPipeline.BuildAssetBundleExplicitAssetNames(new Object[1] { resource.Key.Asset }, new string[1] { resource.Key.Path }, path, options, m_currentBuildTarget);
                if (!res)
                    LogWarning("BuildAssetBundle error: " + resource.Key.Path);
                exportedList.Add(path);
            }
            LogDebug(string.Concat("BuildAssetBundles: ", resource.Key, " ", resource.Value));
        }
        EndBuildAssetBundles();
        return exportedList;
    }

    /// <summary>
    /// 将资源推入堆栈。
    /// </summary>
    /// <param name="info">资源实例</param>
    /// <param name="deep">资源深度</param>
    private static void PushToStack(MogoResourceInfo info, int deep)
    {
        if (stackFile.Contains(info.Path))//重复资源不入盏
        {
            return;
        }
        stackFile.Add(info.Path);
        m_resourceStack.Push(new KeyValuePair<MogoResourceInfo, int>(info, deep));//深度优先入栈
        deep++;
        foreach (var item in info.SubResource)
        {
            PushToStack(item.Value, deep);
        }
    }

    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite = false)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        // If the destination directory doesn't exist, create it. 
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            try
            {
                file.CopyTo(temppath, overwrite);
            }
            catch (Exception e)
            {
                continue;
            }
        }

        // If copying subdirectories, copy them and their contents to new location. 
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs, overwrite);
            }
        }
    }

    #endregion

    #region 输出日志

    public static void LogDebug(object content)
    {
        System.Action<object> action = (msg) =>
        {
            UnityEngine.Debug.Log(msg);
            SendMsg(content.ToString());
        };
        action.BeginInvoke(content, null, null);
    }

    public static void LogWarning(object content)
    {
        System.Action<object> action = (msg) =>
        {
            UnityEngine.Debug.LogWarning(msg);
            SendMsg(content.ToString());
        };
        action.BeginInvoke(content, null, null);
    }

    public static void LogError(object content)
    {
        System.Action<object> action = (msg) =>
        {
            UnityEngine.Debug.LogError(msg);
            SendMsg(content.ToString());
        };
        action.BeginInvoke(content, null, null);
    }

    private static Socket server;
    private static IPEndPoint ipep;

    [MenuItem("DebugMsg/Open Log File")]
    private static void OpenLogFile()
    {
        var logPath = Application.persistentDataPath + "/log";
        var fileName = new DirectoryInfo(logPath).GetFiles().OrderBy(t => t.LastWriteTime).Last().FullName;
        System.Diagnostics.Process.Start(fileName);
    }

    [MenuItem("DebugMsg/Open Log Folder")]
    private static void OpenLogFolder()
    {
        var logPath = Application.persistentDataPath + "/log";
        System.Diagnostics.Process.Start("explorer.exe", logPath.Replace('/', '\\'));
    }

    [MenuItem("DebugMsg/Start")]
    public static void InitMsg()
    {
        if (server == null)
        {
            //设置服务IP，设置TCP端口号
            ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 13245);

            //定义网络类型，数据连接类型和网络协议UDP
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
    }

    private static void SendMsg(string msg)
    {
        if (server != null)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            server.SendTo(data, data.Length, SocketFlags.None, ipep);
        }
    }

    [MenuItem("DebugMsg/Close")]
    public static void CloseMsg()
    {
        server.Close();
        server = null;
    }

    #endregion

    #region bak


    /// <summary>
    /// 自动导出资源，根据固定目录与资源类型进行打包
    /// </summary>
    /// <param name="currentVersion"></param>
    /// <param name="newVersion"></param>
    public static void AutoExport(string currentVersion, string newVersion)
    {
        var folder = GetPath();
        if (folder.Length == 0)
            return;
        Stopwatch sw = new Stopwatch();
        sw.Start();
        var shader = GetFromRoot<Object>("Shader");
        LogError("Get shader time: " + sw.ElapsedMilliseconds);
        List<Object> result;
        var expordedFiles = new List<string>();

        //打包装备，fbx类型
        var fbx = GetFromRoot<Object>("Resources/Characters/Avatar/101/suit");
        fbx.AddRange(GetFromRoot<Object>("Resources/Characters/Avatar/103/suit"));
        fbx.AddRange(GetFromRoot<Object>("Resources/Characters/Avatar/104/suit"));
        fbx.AddRange(shader);
        LogError("Get suit time: " + sw.ElapsedMilliseconds);
        result = UpdateVersion(newVersion, fbx);
        expordedFiles.AddRange(ExportResources(result.ToArray(), folder, "fbx", ".FBX"));
        LogError("Export suit time: " + sw.ElapsedMilliseconds);

        //打包贴图，mat类型
        List<Object> mat = GetFromRoot<Material>("Resources/Characters/Avatar");
        mat.AddRange(GetFromRoot<Material>("Resources/Fx/weapon_trail"));
        mat.AddRange(shader);
        LogError("Get mat time: " + sw.ElapsedMilliseconds);
        result = UpdateVersion(newVersion, mat);
        expordedFiles.AddRange(ExportResources(result.ToArray(), folder, "materials", ".mat"));
        LogError("Export mat time: " + sw.ElapsedMilliseconds);

        //打包角色、特效、机关资源，prefab类型
        List<Object> characters = new List<Object>();
        characters.AddRange(GetFromRoot<GameObject>("Resources/Characters/Avatar"));
        characters.AddRange(GetFromRoot<GameObject>("Resources/Characters/NPC"));
        characters.AddRange(GetFromRoot<GameObject>("Resources/Fx"));
        characters.AddRange(GetFromRoot<GameObject>("Fx"));
        characters.AddRange(GetFromRoot<GameObject>("Resources/Gear"));
        characters.AddRange(shader);
        result = UpdateVersion(newVersion, characters);
        expordedFiles.AddRange(ExportResources(result.ToArray(), folder, "prefab", ".prefab"));

        //打包动作，controller类型
        var controllers = GetFromRoot<RuntimeAnimatorController>("Resources/Characters/Avatar");
        controllers.AddRange(shader);
        LogError("Get controller time: " + sw.ElapsedMilliseconds);
        result = UpdateVersion(newVersion, controllers);
        expordedFiles.AddRange(ExportResources(result.ToArray(), folder, "controller", ".controller"));
        LogError("Export controller time: " + sw.ElapsedMilliseconds);

        //打包UI资源，prefab类型
        List<Object> prefab = new List<Object>();
        prefab.AddRange(GetFromRoot<GameObject>("Resources/Golbal"));
        prefab.AddRange(GetFromRoot<GameObject>("Resources/GUI"));
        prefab.AddRange(GetFromRoot<GameObject>("Resources/Font"));
        prefab.AddRange(GetFromRoot<GameObject>("Resources/MogoUI"));
        prefab.AddRange(GetFromRoot<GameObject>("Resources/Textures"));
        //prefab.AddRange(GetFromRoot<GameObject>("Resources", false));
        prefab.AddRange(shader);
        LogError("Get UI time: " + sw.ElapsedMilliseconds);
        result = UpdateVersion(newVersion, prefab);
        expordedFiles.AddRange(ExportResources(result.ToArray(), folder, "prefab", ".prefab"));
        LogError("Export UI time: " + sw.ElapsedMilliseconds);

        //打包场景资源，prefab类型
        List<Object> scene = new List<Object>();
        scene.AddRange(GetFromRoot<Object>("Compond/01_Forest"));
        scene.AddRange(GetFromRoot<Object>("Compond/02_Desert"));
        scene.AddRange(GetFromRoot<Object>("Compond/04_Dungeons"));
        scene.AddRange(GetFromRoot<Object>("Compond/06_Town"));
        scene.AddRange(GetFromRoot<Object>("Resources/Particles"));
        scene.AddRange(GetFromRoot<Object>("Resources/Scences"));
        result = UpdateVersion(newVersion, scene);
        expordedFiles.AddRange(ExportResources(result.ToArray(), folder, "prefab", ".prefab", ".exr"));

        CopyFolder(Path.Combine(folder, "xml"), Application.dataPath + "/Resources/xml", ".xml");
        CopyFolder(Path.Combine(folder, "entity_defs"), Application.dataPath + "/Resources/entity_defs", ".xml");
        LogError("Copy config time: " + sw.ElapsedMilliseconds);

        SaveExportedFileList(expordedFiles, newVersion, folder);
        LogError("SaveExportedFileList time: " + sw.ElapsedMilliseconds);
        var files = FindUpdatedFiles(currentVersion, newVersion, folder);
        var exFiles = files.Keys.ToList();
        LogError("FindUpdatedFiles time: " + sw.ElapsedMilliseconds);
        PackUpdatedFiles(folder, exFiles, currentVersion, newVersion);
        LogError("PackUpdatedFiles time: " + sw.ElapsedMilliseconds);
        LogError("Total time: " + sw.ElapsedMilliseconds);
        sw.Stop();
    }


    #region build file index
    public static void BuildFileIndexInfo(string strRootFolder, string strDstFolder, bool includeTextData = true)
    {
        if (strRootFolder.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Dst Folder Is Empty", "OK");
            return;
        }
        EditorUtility.DisplayProgressBar("Build File Index Info", "Traversal To Get All Files", 0.0f);
        DirectoryInfo info = new DirectoryInfo(strRootFolder);
        FileInfo[] files = info.GetFiles("*.*", SearchOption.AllDirectories);
        StringBuilder sb = new StringBuilder();
        float currentCount = 0.0f;
        float totalCount = files.Length;
        foreach (FileInfo item in files)
        {
            if (!includeTextData && (item.FullName.EndsWith(".xml", System.StringComparison.OrdinalIgnoreCase) || item.FullName.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase)))
                continue;
            if (item.FullName.Contains("ResourceIndexInfo.txt"))
                continue;
            currentCount += 1.0f;
            EditorUtility.DisplayProgressBar("Build File Index Info", currentCount.ToString() + "/" + totalCount.ToString() + item.Name, currentCount / totalCount);
            sb.Append(item.FullName.Replace("\\", "/").Replace(strRootFolder + "/", ""));
            sb.Append('\n');
        }
        string path = Path.Combine(strDstFolder, "ResourceIndexInfo.txt");
        XMLParser.SaveText(path, sb.ToString());
        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Build File Index Info", "File Index Info Saved In " + path, "ok");
    }
    [MenuItem("Mogo/Build File Index Info")]
    public static void MenuBuildFileIndexInfo()
    {
        BuildFileIndexInfo(@"E:\Mogo\client20131202\MogoResources", @"E:/");
    }
    #endregion

    //[MenuItem("Mogo/Build Files Info")]
    public static void BuildFilesInfo()
    {
        var folder = GetPath();
        if (folder.Length == 0)
            return;
        DirectoryInfo info = new DirectoryInfo(folder);
        var files = info.GetFiles("*.*", SearchOption.AllDirectories);
        LogDebug("Build Files Info files count: " + files.Length);
        StringBuilder sb = new StringBuilder();
        var replacement = folder + '/';
        LogDebug(replacement);
        foreach (var item in files)
        {
            sb.Append(item.FullName.Replace('\\', '/').Replace(replacement, ""));
            sb.Append('\n');
        }
        var path = Path.Combine(info.Parent.FullName, "FilesInfo.txt");
        XMLParser.SaveText(path, sb.ToString());
        LogDebug("Build Files Info finished. " + path);
    }

    //[MenuItem("Mogo/Export Shader")]
    public static void ExportShader()
    {
        var folder = GetPath();
        if (folder.Length == 0)
            return;
        ExportResources(folder, "shader", ".shader");
    }

    //[MenuItem("Mogo/Export FBX")]
    public static void ExportFBX()
    {
        var folder = GetPath();
        if (folder.Length == 0)
            return;
        ExportResources(folder, "fbx", ".FBX");
    }

    //[MenuItem("Mogo/Export Controllers")]
    public static void ExportControllers()
    {
        var folder = GetPath();
        if (folder.Length == 0)
            return;
        ExportResources(folder, "controller", ".controller");
    }

    //[MenuItem("Mogo/Export Materials")]
    public static void ExportMaterials()
    {
        var folder = GetPath();
        if (folder.Length == 0)
            return;
        ExportResources(folder, "materials", ".mat");
    }

    //[MenuItem("Mogo/Export Prefabs")]
    public static void ExportPrefabs()
    {
        var folder = GetPath();
        if (folder.Length == 0)
            return;
        ExportResources(folder, "prefabs", ".prefab", ".exr");
    }

    //[MenuItem("Mogo/Export Scenes")]
    public static void ExportScenes()
    {
        var folder = GetPath();
        if (folder.Length == 0)
            return;

        var scenesFolder = Path.Combine(folder, "scenes");
        var roots = GetRoot();
        foreach (var item in roots)
        {
            if (item.Path.EndsWith(".unity"))
            {
                BuildAssetBundles(folder, item);
                SecurityElement se = new SecurityElement("root");
                se.AddChild(BuildResourceInfoXML(item));
                var path = Path.Combine(scenesFolder, Utils.GetFileNameWithoutExtention(item.Path) + ".xml");
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                XMLParser.SaveText(path, se.ToString());
                LogDebug("Build scene: " + item.Path);
            }
        }
        LogDebug("Create resource info file...");
    }

    //[MenuItem("Mogo/Export Assets Resource")]
    public static void ExportAssetsResource()
    {
        var folder = GetPath();
        if (folder.Length == 0)
            return;

        var roots = GetRoot();
        foreach (var item in roots)
        {
            BuildAssetBundles(folder, item);
            SecurityElement se = new SecurityElement("root");
            se.AddChild(BuildResourceInfoXML(item));
            XMLParser.SaveText(Path.Combine(folder, Utils.GetFileNameWithoutExtention(item.Path) + SystemConfig.CONFIG_FILE_EXTENSION), se.ToString());
            LogDebug("Build Resource: " + item.Path);
        }
        LogDebug("Create resource info file...");
    }


    //static HashSet<string> resourceDepends = new HashSet<string>();

    //private static List<MogoResourceInfoEx> BuildResourceTreeEx(MogoResourceInfoEx resource)
    //{
    //    var subResources = AssetDatabase.GetDependencies(new string[] { resource.MogoResourceInfo.Path });
    //    var list = new List<MogoResourceInfoEx>();
    //    if (subResources.Length > 1)
    //    {
    //        //LogDebug(path);
    //        foreach (var subPath in subResources)
    //        {
    //            if (subPath == resource.MogoResourceInfo.Path || IsIngoreResource(subPath))
    //                continue;
    //            //LogWarning("Can not find resource: " + subPath + " in " + resource.Path);
    //            if (!resourceDepends.Contains(subPath))
    //            {
    //                resourceDepends.Add(subPath);
    //                MogoResourceInfo info;
    //                if (!m_leftResources.ContainsKey(subPath))
    //                {
    //                    info = new MogoResourceInfo();
    //                    info.Path = subPath;//.Replace("Assets/", "");
    //                    info.Asset = AssetDatabase.LoadAssetAtPath(subPath, typeof(Object));
    //                    m_leftResources.Add(subPath, info);
    //                }
    //                else
    //                {
    //                    info = m_leftResources[subPath];
    //                }
    //                var ex = new MogoResourceInfoEx();
    //                ex.MogoResourceInfo = info;
    //                resource.SubResource.Add(subPath, ex);//加儿子
    //                ex.Parents[resource.MogoResourceInfo.Path] = resource;//加父亲

    //                list.Add(ex);
    //            }
    //        }
    //        var tempList = list.ToArray();
    //        foreach (var item in tempList)
    //        {
    //            list.AddRange(BuildResourceTreeEx(item));
    //        }
    //    }
    //    return list;
    //}

    //public class MogoResourceInfoEx
    //{
    //    public MogoResourceInfoRoot MogoResourceInfo { get; set; }
    //    public Dictionary<string, MogoResourceInfoEx> Parents { get; set; }
    //    public Dictionary<string, MogoResourceInfoEx> SubResource { get; set; }

    //    public MogoResourceInfoEx()
    //    {
    //        Parents = new Dictionary<string, MogoResourceInfoEx>();
    //        SubResource = new Dictionary<string, MogoResourceInfoEx>();
    //    }

    //    public List<string> GetSonRecursively()
    //    {
    //        var list = new List<string>();
    //        list.Add(MogoResourceInfo.Path);
    //        foreach (var item in SubResource)
    //        {
    //            list.AddRange(item.Value.GetSonRecursively());
    //        }
    //        return list;
    //    }

    //    public string Print()
    //    {
    //        return Print(this);
    //    }

    //    private string Print(MogoResourceInfoEx info)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        sb.AppendLine(string.Concat(info.MogoResourceInfo.Path));
    //        if (info.SubResource != null && info.SubResource.Count != 0)
    //        {
    //            sb.AppendLine("push");
    //            foreach (var item in info.SubResource)
    //            {
    //                sb.Append(item.Value.Print());
    //            }
    //            sb.AppendLine("pop");
    //        }

    //        return sb.ToString();
    //    }
    //}


    #endregion
}

public class VersionInfo
{
    public string Path { get; set; }
    public string MD5 { get; set; }
    public string Version { get; set; }
    public Object Asset { get; set; }
}

public class MogoResourceInfoRoot
{
    public string Version { get; set; }
    public int Deep { get; set; }
    public string Path { get; set; }
    public Object Asset { get; set; }
}

public class MogoResourceInfo : MogoResourceInfoRoot
{
    public Dictionary<string, MogoResourceInfo> Parents { get; set; }
    public Dictionary<string, MogoResourceInfo> SubResource { get; set; }

    public MogoResourceInfo()
    {
        Parents = new Dictionary<string, MogoResourceInfo>();
        SubResource = new Dictionary<string, MogoResourceInfo>();
    }

    public override string ToString()
    {
        return Path;
    }

    public MogoResourceInfo GetCopy()
    {
        var info = new MogoResourceInfo();
        info.Path = Path;
        info.Asset = Asset;
        return info;
    }

    public List<string> GetSonRecursively()
    {
        var list = new List<string>();
        list.Add(Path);
        foreach (var item in SubResource)
        {
            list.AddRange(item.Value.GetSonRecursively());
        }
        return list;
    }

    public List<MogoResourceInfo> GetSonInfoRecursively()
    {
        var list = new List<MogoResourceInfo>();
        list.AddRange(SubResource.Values);
        foreach (var item in SubResource)
        {
            list.AddRange(item.Value.GetSonInfoRecursively());
        }
        return list;
    }

    public string Print()
    {
        return Print(this);
    }

    private string Print(MogoResourceInfo info)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(string.Concat(info.Path, " ", info.Deep));
        if (info.SubResource != null && info.SubResource.Count != 0)
        {
            sb.AppendLine("push");
            foreach (var item in info.SubResource)
            {
                sb.Append(item.Value.Print());
            }
            sb.AppendLine("pop");
        }

        return sb.ToString();
    }
}

public class BuildResourcesInfo
{
    public string name { get; set; }
    public string type { get; set; }
    public string[] packLevel { get; set; }
    public int isMerge { get; set; }
    public bool isPopInBuild { get; set; }
    public string[] extentions { get; set; }
    public bool check { get; set; }
    public List<BuildResourcesSubInfo> folders { get; set; }
    //默认构造
    public BuildResourcesInfo()
    { }
    //拷贝构造
    public BuildResourcesInfo(BuildResourcesInfo bri)
    {
        name = bri.name;
        type = bri.type;
        packLevel = bri.packLevel;
        isMerge = bri.isMerge;
        isPopInBuild = bri.isPopInBuild;
        extentions = bri.extentions;
        check = bri.check;
        folders = bri.folders;
    }
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