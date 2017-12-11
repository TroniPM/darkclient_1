/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：NPCData
// 创建者：Steven Yang
// 修改者列表：
// 创建日期：2013-2-6
// 模块描述：NPC数据结构
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class NPCData : GameData<NPCData>
    {
        public int name { get; protected set; }

        public int mode { get; protected set; }
        public int mapx { get; protected set; }
        public int mapy { get; protected set; }
        public List<float> rotation { get; protected set; }
        public float colliderRange { get; protected set; }

        public int dialogBoxImage { get; protected set; }
        public int tips { get; protected set; }

        public int standbyAction { get; protected set; }
        public Dictionary<int, int> actionList { get; protected set; }
        public List<int> thinkInterval { get; protected set; }
        public List<int> idleTimeRange { get; protected set; }
             
        static public readonly string fileName = "xml/NPCData";
        //static public Dictionary<int, NPCData> dataMap { get; set; }
    }
}