using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using Mogo.Util;
using Van;

public class OSXTools 
{
	[MenuItem(@"Edit/OSX/Test")]
	public static void Test()
	{
		System.Xml.XmlDocument doc=new System.Xml.XmlDocument();
		System.Xml.XmlElement root=doc.CreateElement("root");
		doc.AppendChild(root);
		System.Xml.XmlElement e=doc.CreateElement("file");
		e.SetAttribute("path","r/a/b/c/d");
		System.Xml.XmlElement e1=doc.CreateElement("dependence");
		e1.SetAttribute("path","r/a/b/c/e");
		e.AppendChild(e1);
		root.AppendChild(e);
		doc.ExsitOrInsert("f/d/s/a/s");
		doc.Save("test.xml");
	}

	[MenuItem(@"Edit/OSX/BuildAssetBundleForSelectedObject")]
	public static void BuildAssetBundleForSelectedObject()
	{
		List<string> prefabs = new List<string> ();
		List<string> models = new List<string> ();
		List<string> controllers = new List<string> ();
		List<string> animations = new List<string> ();
		List<string> mats = new List<string> ();
		List<string> textures = new List<string> ();
		List<string> shaders = new List<string> ();
		List<string> audios = new List<string> ();

		Object [] selectedObjects=Selection.objects;


		foreach(Object obj in selectedObjects)
		{
			string [] dependences=AssetDatabase.GetDependencies(new string[]{AssetDatabase.GetAssetPath(obj)});
			Debug.Log(dependences.Length);

			foreach(string str in dependences)
			{
				if(str.Contains(".png")||str.Contains(".TTF")||str.Contains(".tif")
				   ||str.Contains(".jpg")||str.Contains(".tga")||str.Contains(".jpeg"))
				{
					textures.Add(str);
				}
				else if(str.Contains(".shader"))
				{
					shaders.Add(str);
				}
				else if(str.Contains(".mat"))
				{
					mats.Add(str);
				}
				else if(str.Contains(".prefab"))
				{
					prefabs.Add(str);
				}
				else if(str.Contains(".anim"))
				{
					animations.Add(str);
				}
				else if(str.Contains(".fbx")||str.Contains(".FBX")||str.Contains(".obj"))
				{
					models.Add(str);
				}
				else if(str.Contains(".controller"))
				{
					controllers.Add(str);
				}
				else if(str.Contains(".mp3")||str.Contains(".wav"))
				{
					audios.Add(str);
				}
				if(!str.Contains(".cs"))
					Debug.Log(str);
			}
			int nCount=audios.Count+textures.Count+mats.Count+models.Count+prefabs.Count+animations.Count+controllers.Count+shaders.Count;
			Debug.Log(nCount);
		}
		BuildPipeline.PushAssetDependencies ();
		BuildAssetBundle(textures);
		BuildAssetBundle(audios);
		BuildAssetBundle(shaders);
		BuildPipeline.PushAssetDependencies ();
		BuildAssetBundle(mats);
		BuildPipeline.PushAssetDependencies ();
		BuildAssetBundle(animations);
		BuildPipeline.PushAssetDependencies ();
		BuildAssetBundle(controllers);
		BuildPipeline.PushAssetDependencies ();
		BuildAssetBundle(models);
		BuildPipeline.PushAssetDependencies ();
		BuildAssetBundle(prefabs);
		BuildPipeline.PopAssetDependencies ();
		BuildPipeline.PopAssetDependencies ();
		BuildPipeline.PopAssetDependencies ();
		BuildPipeline.PopAssetDependencies ();
		BuildPipeline.PopAssetDependencies ();
		BuildPipeline.PopAssetDependencies ();
		BuildPipeline.PopAssetDependencies ();
	}

	[MenuItem(@"Edit/OSX/CollectAllResources")]
	public static void CollectAllResources()
	{
		List<string> res=BuildProjectExWizard.GetAllResources();
		System.IO.FileStream fs=System.IO.File.Open("AllResources.txt",System.IO.FileMode.OpenOrCreate);
		System.IO.StreamWriter sw=new System.IO.StreamWriter(fs);
		foreach(string r in res)
		{
			sw.WriteLine(r);
		}
		sw.Flush();
		sw.Close();
		fs.Close();
	}

	[MenuItem(@"Edit/OSX/BuildAssetBundleWithOutMeta")]
	public static void BuildAssetBundleWithOutMeta()
	{
		string [] argv=System.Environment.GetCommandLineArgs();

		if(argv.Length!=0)
		{
			for(int i=5;i<argv.Length;i++)
			{
				BundleExporter.ExportBundleForCloud(new string[]{argv[i]},"MogoResourcesOSX");
				using(System.IO.FileStream fs=System.IO.File.Open("BuildResourceList.txt",System.IO.FileMode.OpenOrCreate | System.IO.FileMode.Append))
				{
					System.IO.StreamWriter sw=new System.IO.StreamWriter(fs);
					sw.WriteLine(argv[i]);
					sw.Flush();
					sw.Close();
					fs.Close();
				}
			}
		}
	}
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
			EditorUtility.DisplayProgressBar("Build File Index Info", currentCount.ToString() + "/" + totalCount.ToString() + 
			                                 "  "+item.Name, currentCount / totalCount);
			sb.Append(item.FullName.Replace("\\", "/").Replace(strRootFolder + "/", ""));
			sb.Append('\n');
		}
		string path = Path.Combine(strDstFolder, "ResourceIndexInfo.txt");
		XMLParser.SaveText(path, sb.ToString());
		EditorUtility.ClearProgressBar();
		EditorUtility.DisplayDialog("Build File Index Info", "File Index Info Saved In " + path, "ok");
	}
	[MenuItem("Edit/OSX/Build File Index Info")]
	public static void MenuBuildFileIndexInfo()
	{
		BuildFileIndexInfo(@"MogoResourcesOSX", @"MogoResourcesOSX");
	}
	[MenuItem(@"Edit/OSX/Make Meta for All")]
	public static void MakeMetaForAll()
	{
		List<string> res=new List<string>();//=BuildProjectExWizard.GetAllResources();
		System.IO.FileStream fs=System.IO.File.Open("AllResources.txt",System.IO.FileMode.OpenOrCreate);
		System.IO.StreamReader sr=new System.IO.StreamReader(fs);
		string temp=sr.ReadLine();
		while(null!=temp)
		{
			res.Add(temp);
			temp=sr.ReadLine();
		}
		sr.Close();
		fs.Close();
		int nIndex=0;
		int nCount=res.Count;
		for(;nIndex<nCount;nIndex++)
		{
			Debug.Log("Export meta ------------"+nIndex+"/"+nCount+"---------------"+res[nIndex]);
			BundleExporter.MakeMeta(new string[]{res[nIndex]},"MogoResourcesOSX");
		}
	}
	[MenuItem(@"Edit/OSX/Make Meta for Selected Object")]
	public static void MakeMetaForSelectedObject()
	{
		Object [] selectedObjects=Selection.objects;
		BundleExporter.MakeMeta(new string[]{AssetDatabase.GetAssetPath(selectedObjects[0])},"MogoResourcesOSX");
	}
	static HashSet<string> CreateMeta(List<string> datas,HashSet<string> res,Dictionary<string,XmlDocument> metas)
	{
		string targetPath=null;
		foreach(string str in datas)
		{
			targetPath=str.Replace("Assets","MogoResourcesOSX")+".u";
			targetPath=targetPath.Substring(0,targetPath.LastIndexOf("/"));
			targetPath.ExsitOrCreate();
			XmlDocument doc=null;
			if(!metas.ContainsKey(targetPath+"/Meta.xml"))
			{
				doc=new XmlDocument();
				doc.AppendChild(doc.CreateElement("root"));
				metas.Add(targetPath+"/Meta.xml",doc);
			}
			else
			{
				doc=metas[targetPath+"/Meta.xml"];
			}
			XmlElement e=doc.ExsitOrInsert(str);
			string [] ds=AssetDatabase.GetDependencies(new string[]{str});
			foreach(string d in ds)
			{
				if(!res.Contains(d.Replace("Assets/",""))&&!e.ExsitDependence(d.Replace("Assets/","")))
				{
					XmlElement dep=doc.CreateElement("dependence");
					dep.SetAttribute("path",d.Replace("Assets/",""));
				}
			}
			res.Add(str);
		}
		return res;
	}

	static void BuildAssetBundle(List<string> datas)
	{
		BuildAssetBundleOptions opts = BuildAssetBundleOptions.CollectDependencies |
			BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.DeterministicAssetBundle;
		string targetPath=null;
		foreach(string str in datas)
		{
			targetPath=str.Replace("Assets","MogoResourcesOSX")+".u";
			targetPath=targetPath.Substring(0,targetPath.LastIndexOf("/"));
			targetPath.ExsitOrCreate();
			BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(str),null,
			                               str.Replace("Assets","MogoResourcesOSX")+".u",opts,
			                               EditorUserBuildSettings.activeBuildTarget);
		}
	}
	static void CleanUpAllCaches()
	{
	}
}

namespace Van
{
	public static class Extension
	{
		public static XmlElement ExsitOrInsert(this XmlDocument doc,string strData)
		{
			foreach(XmlElement e in doc.GetElementsByTagName("file"))
			{
				if(e.GetAttribute("path")==strData)
				{
					return e;
				}
			}
			XmlElement xe=doc.CreateElement("file");
			xe.SetAttribute("path",strData);
			doc.DocumentElement.AppendChild(xe);
			return xe;
		}
		public static bool ExsitDependence(this XmlElement e,string dependence)
		{
			foreach(XmlElement c in e.ChildNodes)
			{
				if(c.GetAttribute("path")==dependence)
				{
					return true;
				}
			}
			return false;
		}

		public static void ExsitOrCreate(this string t)
		{
			if(!System.IO.Directory.Exists(t))
			{
				System.IO.Directory.CreateDirectory(t);
			}
		}
	}
}
