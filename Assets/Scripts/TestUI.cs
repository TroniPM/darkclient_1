/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TestUI
// 创建者：Hooke Hu
// 修改者列表：
// 创建日期：
// 模块描述：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;
using Mogo.Game;
using Mogo.Util;
using System.Linq;

public class TestUI : MonoBehaviour
{
    string prefabName = "NPC_1024.prefab";
    List<Object> temps = new List<Object>();
    bool isShowDebug;

    void OnGUI()
    {

        if (GUI.Button(new Rect(120, 20, 50, 30), "switch"))
        {
            isShowDebug = !isShowDebug;
            //LoggerHelper.UploadLogFile();
        }
        //if (GUI.Button(new Rect(20, 70, 50, 30), "kkkk"))
        //{
        //    EventDispatcher.TriggerEvent(Events.OtherEvent.ClientGM, "@OpenGM");
        //}
        if (!isShowDebug)
        {
            return;
        }
        prefabName = GUI.TextField(new Rect(0, 60, 200, 40), prefabName);

        if (GUI.Button(new Rect(0, 100, 100, 100), "create"))
        {
            temps.Clear();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var prefs = prefabName.Split('-');
            for (int i = 0; i < prefs.Length; i++)
            {
                var index = i;
                var item = prefs[index];
                AssetCacheMgr.GetInstance(item, (a, b, c) =>
                {
                    if (index == prefs.Length - 1)
                    {
                        sw.Stop();
                        LoggerHelper.Debug("Load Fin: " + sw.ElapsedMilliseconds);
                    }
                    LoggerHelper.Debug(a);
                    temps.Add(c);
                });
            }
        }

        if (GUI.Button(new Rect(100, 100, 100, 100), "destroy"))
        {
            foreach (var item in temps)
            {
                AssetCacheMgr.ReleaseInstance(item);
            }
        }

        if (GUI.Button(new Rect(0, 200, 100, 100), "Res"))
        {
            AssetCacheMgr.GetResource(prefabName, (a) =>
            {
                LoggerHelper.Debug(a);
            });
        }

        resourceName = GUI.TextField(new Rect(200, 200, 200, 100), resourceName);
        result = GUI.TextField(new Rect(0, 300, 400, 400), result);

        if (GUI.Button(new Rect(100, 200, 50, 50), "Find Root"))
        {
            result = ResourceManager.GetResourceRoots(resourceName).PackList('\n');
        }
        if (GUI.Button(new Rect(150, 200, 50, 50), "PR"))
        {
            LoggerHelper.Info(ResourceManager.resources.Values.ToList().PackList('\n'));
        }
        if (GUI.Button(new Rect(100, 250, 50, 50), "PRD"))
        {
            LoggerHelper.Info(AssetCacheMgr.ResourceDic.PackMap(mapSpriter: '\n'));
        }
        if (GUI.Button(new Rect(150, 250, 50, 50), "PGONM"))
        {
            LoggerHelper.Info(AssetCacheMgr.GameObjectNameMapping.PackMap(mapSpriter: '\n'));
        }
        if (GUI.Button(new Rect(200, 100, 100, 100), "show dep"))
        {
            LoggerHelper.Info(ResourceManager.resources.Values.ToList().PackList('\n'));
        }
    }

    string resourceName = "resourceName";
    string result = "result";

    void OnDoTask(Component com, int i, GameObject g, MissingScriptsCounter counter)
    {
        if (com == null)
        {
            var fullName = GetFullName(g);
            counter.missingCount++;
            //Mogo.Util.LoggerHelper.Debug(fullName + " has an empty script attached in position: " + i, g);
            Debug.Log(fullName + " has an empty script attached in position: " + i, g);
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
}