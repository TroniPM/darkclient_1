/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：EntityNpcAgent
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：
// 最后修改日期：
// 模块描述：
// 代码版本：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Mogo.Game;
using Mogo.GameData;
using Mogo.Util;

public class NpcExportAgent : ExportAgent
{
    public int name_i;
    public int mode_i;
    public int tips_l;

    public float mapx_i { get; protected set; }
    public float mapy_i { get; protected set; }
    public string rotation_l { get; protected set; }

    public int dialogBoxImage_i;
    public int standbyAction_i;

    void Start()
    {
        type = "NPC";
        theTransform = transform;
        temp = gameObject;

        if (AvatarModelData.dataMap == null)
        {
            GameDataControler.Init();
        }

        if (!AvatarModelData.dataMap.ContainsKey(mode_i) && mode_i != 150000)
        {
            LoggerHelper.Debug("mode_i wrong: " + mode_i);
            return;
        }

        Mogo.Util.LoggerHelper.Debug("mode_i: " + AvatarModelData.dataMap[mode_i].prefabName);

        if (mode_i != 150000)
        {
            AssetCacheMgr.GetNoCacheInstance(AvatarModelData.dataMap[mode_i].prefabName,
                (prefab, guid, go) =>
                {
                    if (go == null)
                    {
                        LoggerHelper.Debug("prefabName wrong");
                    }
                    temp = go as GameObject;
                    temp.transform.position = transform.position;
                    temp.transform.rotation = transform.rotation;
                });
        }
        else
        {
            temp.transform.position = transform.position;
            temp.transform.rotation = transform.rotation;
        }

        mapx_i = temp.transform.position.x;
        mapy_i = temp.transform.position.z;
        rotation_l = (new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z)).ToString();
    }

    void Update()
    {
        mapx_i = temp.transform.position.x;
        mapy_i = temp.transform.position.z;
        rotation_l = (new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z)).ToString();
    }
}
