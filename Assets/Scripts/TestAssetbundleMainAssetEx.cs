/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：TestAssetbundleMainAsset
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
using Mogo.GameData;

public class TestAssetbundleMainAssetEx : MonoBehaviour
{
    int index;
    string fileName = "Task.prefab";
    GameObject tempModel;
    //string resName = "Resources/Characters/Avatar/101/equip/Materials/equip_warrior_0103_mat.mat";
    ILoadAsset loadAsset;

    void Start()
    {
        MogoFileSystem.Instance.Init();

        loadAsset = gameObject.AddComponent<LoadAssetBundlesMainAsset>();
        ResourceManager.LoadMetaOfMeta(() => { Debug.Log("Init META Success"); }, null);
        //LoadInstance("ArenaUI.prefab", null);
        //LoadInstance("MogoUI.prefab", null);
    }

    void Update()
    {
        TimerHeap.Tick();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "load npcs"))
        {
            var list = (loadAsset as LoadAssetBundlesMainAsset).m_filesDic.Keys.Where(t => t.StartsWith("NPC")).ToList();
            var name = list[index];
            Mogo.Util.LoggerHelper.Debug(name);

            loadAsset.LoadInstance(name, (a, b, c) =>
            {
                loadAsset.Release(name, false);
                //sw.Stop();
                //LoggerHelper.Debug(sw.ElapsedMilliseconds);
            });
            index++;
        }
        if (GUI.Button(new Rect(100, 0, 100, 100), "batching"))
        {
            StaticBatchingUtility.Combine(tempModel);
        }
        fileName = GUI.TextField(new Rect(300, 100, 300, 100), fileName);
        if (GUI.Button(new Rect(300, 0, 50, 50), "load"))
        {
            loadAsset.LoadInstance(fileName,
                (a, b, c) =>
                {
                    //loadAsset.Release(fileName, false);
                    tempModel = c as GameObject;
                    //sw.Stop();
                    //LoggerHelper.Debug(sw.ElapsedMilliseconds);
                },
                (progress) =>
                {
                    Debug.Log(progress);
                });
        }
        if (GUI.Button(new Rect(350, 0, 50, 50), "load UI"))
        {
            loadAsset.LoadUIAsset(fileName, (a) =>
            {
                tempModel = GameObject.Instantiate(a) as GameObject;
                //loadAsset.Release(fileName, false);
                //sw.Stop();
                //LoggerHelper.Debug(sw.ElapsedMilliseconds);
            }, null);
        }
        if (GUI.Button(new Rect(200, 0, 100, 100), "load res"))
        {
            loadAsset.LoadAsset(fileName, (a) =>
            {
                tempModel = a as GameObject;
                //sw.Stop();
                //LoggerHelper.Debug(sw.ElapsedMilliseconds);
            });
        }

        if (GUI.Button(new Rect(400, 0, 100, 100), "unload"))
        {
            loadAsset.Release(fileName);
        }
        if (GUI.Button(new Rect(500, 0, 100, 100), "load scene"))
        {
            Application.LoadLevel(Utils.GetFileNameWithoutExtention(fileName));
        }
        if (GUI.Button(new Rect(600, 0, 100, 100), "ClearAll"))
        {
            ResourceManager.ClearAll();
        }
        if (GUI.Button(new Rect(600, 100, 100, 100), "UnloadUnused"))
        {
            Resources.UnloadUnusedAssets();
        }
        if (GUI.Button(new Rect(600, 200, 100, 100), "GC"))
        {
            System.GC.Collect();

        }
        if (GUI.Button(new Rect(400, 300, 100, 100), "set null"))
        {
            tempModel = null;
        }
        if (GUI.Button(new Rect(500, 300, 100, 100), "Instantiate"))
        {
            GameObject.Instantiate(tempModel);
        }
        if (GUI.Button(new Rect(600, 300, 100, 100), "tempPref"))
        {
            Debug.Log(tempModel);
        }
        if (GUI.Button(new Rect(300, 300, 100, 100), "show resources"))
        {
            Debug.Log(ResourceManager.resources.Values.ToList().PackList('\n'));
        }
        if (GUI.Button(new Rect(200, 300, 100, 100), "Load Meta"))
        {
            ResourceManager.LoadMetaOfMeta(() => { Debug.Log("Init META Success"); }, null);
        }
        if (GUI.Button(new Rect(100, 300, 100, 100), "GameDataControler Init"))
        {
            //LoggerHelper.Debug(FXData.dataMap.Count);
            //LoggerHelper.Debug(GlobalData.dataMap.Count);
            Mogo.GameData.GameDataControler.Init(null, () =>
            {
            });
        }

    }
}