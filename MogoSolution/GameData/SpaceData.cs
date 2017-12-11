using System;
using System.Collections.Generic;
using UnityEngine;


namespace Mogo.GameData
{
    public class SpaceData
    {
        public int id { get; set; }
        public string type { get; set; }
        public int triggerType { get; set; }
        public List<int> monsterSpawntPoint { get; set; }
        public List<int> preSpawnPointId { get; set; }
        public List<int> levelID { get; set; }

        public static Dictionary<int, SpaceData> dataMap = new Dictionary<int, SpaceData>();
    }
}
