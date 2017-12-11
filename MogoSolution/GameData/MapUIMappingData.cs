using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mogo.GameData
{
    public class MapUIMappingData : GameData<MapUIMappingData>
    {
        public int name { get; protected set; }
        public Dictionary<int, int> grid { get; protected set; }
        public Dictionary<int, int> gridName { get; protected set; }
        public Dictionary<int, int> gridShape { get; protected set; }
        public Dictionary<int, int> gridImage { get; protected set; }
        public List<int> chest { get; protected set; }
        public List<int> bossChest { get; protected set; }

        static public readonly string fileName = "xml/MapUIMapping";
        //static public Dictionary<int, MapUIMappingData> dataMap { get; set; }
    }
}