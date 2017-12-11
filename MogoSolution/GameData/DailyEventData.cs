using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class DailyEventData : GameData<DailyEventData>
    {
        public int title { get; protected set; }
        public int icon { get; protected set; }
        public int group { get; protected set; }
        public List<int> level { get; protected set; }
        public List<int> viplevel { get; protected set; }
        public List<int> finish { get; protected set; }
        public int exp { get; protected set; }
        public int gold { get; protected set; }

        static public readonly string fileName = "xml/day_task";
        //static public Dictionary<int, DailyEventData> dataMap { get; set; }
    }
}