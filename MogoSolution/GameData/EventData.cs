using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mogo.GameData
{
    public class EventData : GameData<EventData>
    {
        public int title { get; protected set; }
        public int describtion { get; protected set; }
        public int icon { get; protected set; }
        public int reward_type { get; protected set; }
        public int scream_type { get; protected set; }
        public int type { get; protected set; }
        public int name { get; protected set; }

        public string arg_1 { get; protected set; }
        public string arg_2 { get; protected set; }
        public string arg_3 { get; protected set; }
        public string arg_4 { get; protected set; }
        public string arg_5 { get; protected set; }
        public string arg_6 { get; protected set; }

        public int limit_time { get; protected set; }
        public int limit_count { get; protected set; }

        public Dictionary<int, int> conditions { get; protected set; }
        public Dictionary<int, int> task_conditions { get; protected set; }

        public int gold { get; protected set; }
        public int energy { get; protected set; }

        public Dictionary<int, int> items1 { get; protected set; }
        public Dictionary<int, int> items2 { get; protected set; }
        public Dictionary<int, int> items3 { get; protected set; }
        public Dictionary<int, int> items4 { get; protected set; }

        public int rule { get; protected set; }
        public int reminder { get; protected set; }

        public static readonly string fileName = "xml/Event";
        //public static Dictionary<int, EventData> dataMap { get; set; }

        public string Format(params object[] args)
        {
            return LanguageData.dataMap.ContainsKey(rule) ? string.Format(LanguageData.dataMap[rule].content, args) : "";
        }
    }
}
