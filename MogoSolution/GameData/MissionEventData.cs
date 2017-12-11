using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mogo.GameData
{
    public class MissionEventData : GameData<MissionEventData>
    {
        public int type { get; protected set; }
        public List<int> param { get; protected set; }
        public List<int> notifyToClient { get; protected set; }
        public List<int> notifyOtherSpawnPoint { get; protected set; }

        static public readonly string fileName = "xml/MissionEvent";
        //static public Dictionary<int, MissionEventData> dataMap { get; set; }
    }
}
