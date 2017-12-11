using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class SpawnPointLevelData : GameData<SpawnPointLevelData>
    {
        public List<int> monsterId { get; protected set; }
        public List<int> monsterNumber { get; protected set; }
        static public readonly string fileName = "xml/SpawnPointLevel";
        //static public Dictionary<int, SpawnPointLevelData> dataMap { get; set; }
    }
}

