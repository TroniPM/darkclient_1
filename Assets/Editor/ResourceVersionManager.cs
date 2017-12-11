using Mogo.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

public class ResourceVersionManager
{
    private Dictionary<string, ResourceMetaData> metaOfResource = new Dictionary<string, ResourceMetaData>();
    private Dictionary<string, string> metaOfMeta;
    private string m_rootPath;

    public string[] CompareResource(ResourceVersionManager source)
    {
        var diffOfMetaOfMeta = metaOfMeta.Where(t => !(source.metaOfMeta.ContainsKey(t.Key) && source.metaOfMeta[t.Key] == t.Value));
        //LoggerHelper.Debug(deffOfMetaOfMeta.PackMap(mapSpriter: '\n'));
        foreach (var item in diffOfMetaOfMeta)
        {
            this.LoadMetaOfResource(this.m_rootPath, item.Key);
            source.LoadMetaOfResource(source.m_rootPath, item.Key);
        }
        var diffOfMetaOfResource = metaOfResource.Where(t => !(source.metaOfResource.ContainsKey(t.Key) && source.metaOfResource[t.Key].MD5 == t.Value.MD5));
        var result = diffOfMetaOfResource.Select(t => t.Key).ToArray();
        LoggerHelper.Debug(result.PackArray());

        return result;
    }

    public void InitResource(string rootPath)
    {
        m_rootPath = rootPath;
        //LoggerHelper.Debug("rootPath: " + rootPath);
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        LoadMetaOfMeta(rootPath);
        //LoggerHelper.Debug(metaOfMeta.PackMap(mapSpriter: '\n'));
        //LoggerHelper.Debug(metaOfResource.PackMap(mapSpriter: '\n'));
        sw.Stop();
        //LoggerHelper.Debug("time: " + sw.ElapsedMilliseconds + " metaOfResource.count: " + metaOfResource.Count);
    }

    #region LoadMeta

    public bool LoadMetaOfMeta(string rootPath)
    {
        metaOfMeta = new Dictionary<string, string>();
        var xml = XMLParser.LoadXML(Utils.LoadFile(Path.Combine(rootPath, ResourceManager.MetaFileName)));
        if (xml == null)
        {
            return false;
        }
        foreach (SecurityElement item in xml.Children)
        {
            metaOfMeta[item.Attribute("path")] = item.Attribute("md5");
        }
        return true;
    }
    public bool LoadMetaOfResource(string rootPath, string path)
    {
        var xml = XMLParser.LoadXML(Utils.LoadFile(Path.Combine(rootPath, path)));
        if (xml == null)
        {
            return false;
        }
        foreach (SecurityElement item in xml.Children)
        {
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
        return true;
    }
    #endregion
}