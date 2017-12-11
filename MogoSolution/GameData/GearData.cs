using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class GearData : GameData<GearData>
    {
        public int map { get; protected set; }
        public string type { get; protected set; }
        public string gameObjectName { get; protected set; }

        public string argNames { get; protected set; }
        public string argTypes { get; protected set; }
        public string args { get; protected set; }

        static public readonly string fileName = "xml/GearData";
        //static public Dictionary<int, GearData> dataMap { get; set; }
    }
}

