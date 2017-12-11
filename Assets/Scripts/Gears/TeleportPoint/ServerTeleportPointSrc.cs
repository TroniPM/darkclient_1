/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：ConveyerGear
// 创建者：Key Pan
// 修改者列表：Key Pan
// 创建日期：
// 最后修改日期：
// 模块描述：
// 代码版本：
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

using Mogo.Util;

public class ServerTeleportPointSrc : GearParent
{
    public int targetSceneId;
    public float targetX;
    public float targetY;

    // public string targetLabel;

    void Awake()
    {
        gearType = "TeleportPointSrc";
        ID = (uint)defaultID;

        AddListeners();
    }

    void OnDestroy()
    {
        RemoveListeners();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GearParent.MogoPlayerTag)
        {
            if (triggleEnable)
            {
                if (MogoWorld.thePlayer != null)
                    MogoWorld.thePlayer.ProcTrapBegin("TeleportPointSrc");

                EventDispatcher.TriggerEvent<uint>(Events.GearEvent.Teleport, ID);
            }
        }
    }
}