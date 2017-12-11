using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class InstanceLevelGridPosData : GameData<InstanceLevelGridPosData>
    {
        // ��ͨ�ؿ���0~8������ؿ�(���磺������Ԩ)��9
        public string grid0Pos { get; protected set; }
        public string grid1Pos { get; protected set; }
        public string grid2Pos { get; protected set; }
        public string grid3Pos { get; protected set; }
        public string grid4Pos { get; protected set; }
        public string grid5Pos { get; protected set; }
        public string grid6Pos { get; protected set; }
        public string grid7Pos { get; protected set; }
        public string grid8Pos { get; protected set; }
        public string grid9Pos { get; protected set; }
        static public readonly string fileName = "xml/InstanceLevelGridPos";
        //static public Dictionary<int, InstanceLevelGridPosData> dataMap { get; set; }
    }
}
