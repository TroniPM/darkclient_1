/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：MissionData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-2-6
// 模块描述：任务数据结构
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class TaskData : GameData<TaskData>
    {
        public int type { get; protected set; }

        public string name { get; protected set; }
        public int nextId { get; protected set; }
        public int level { get; protected set; }
        public int npc { get; protected set; }

        public Dictionary<int, int> text { get; protected set; }
        public int tiptext { get; protected set; }

        public int pathPoint { get; protected set; }

        public int exp { get; protected set; }
        public int money { get; protected set; }

        public Dictionary<int, int> awards1 { get; protected set; }
        public Dictionary<int, int> awards2 { get; protected set; }
        public Dictionary<int, int> awards3 { get; protected set; }
        public Dictionary<int, int> awards4 { get; protected set; }

        public int time { get; protected set; }

        public int conditionType { get; protected set; }
        public List<int> condition { get; protected set; }

        public int cameraAniId { get; protected set; }
        public int avatarAniId { get; protected set; }

        public int autoIcon { get; protected set; }
        public int isShowNPCTip { get; protected set; }

        static public readonly string fileName = "xml/TaskData";
        //static public Dictionary<int, TaskData> dataMap { get; set; }
    }
}