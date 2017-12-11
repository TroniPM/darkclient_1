/*----------------------------------------------------------------
// Copyright (C) 2013 广州，爱游
//
// 模块名：AvatarModelData
// 创建者：Joe Mo
// 修改者列表：
// 创建日期：2013-3-18
// 模块描述：角色模型数据
//----------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class AvatarModelData : GameData<AvatarModelData>
    {
        public string vocation { get; protected set; }
        public List<int> nakedEquipList { get; protected set; }
        public List<int> originalRotation { get; protected set; }
        public string prefabName { get; protected set; }
        public float scale { get; protected set; }

        public List<int> bornFx { get; protected set; }
        public List<int> dieFx { get; protected set; }
        public int deadTime { get; protected set; }
        public int bornTime { get; protected set; }
        public int scaleRadius { get; protected set; }
        public int speed { get; protected set; }
        public int notTurn { get; protected set; }

        static public readonly string fileName = "xml/AvatarModel";
        //static public Dictionary<int, AvatarModelData> dataMap { get; set; }
    }
}