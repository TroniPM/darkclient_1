using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ActivityTimeData : GameData<ActivityTimeData>
    {
        public int weekDay { get; protected set; }
        public string activityTime { get; protected set; }

        static public readonly string fileName = "xml/ActivityTime";

        protected static Dictionary<int, Dictionary<int, string>> activityTimeFormatDataMap = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> ActivityTimeFormatDataMap
        {
            get 
            {
                if (activityTimeFormatDataMap.Count == 0)
                    FormatActivityTimeData();

                return activityTimeFormatDataMap;
            }
        }

        public static void FormatActivityTimeData()
        {
            foreach (var item in dataMap)
            {
                if (!activityTimeFormatDataMap.ContainsKey(item.Value.weekDay))
                    activityTimeFormatDataMap.Add(item.Value.weekDay, new Dictionary<int, string>());

                if (item.Value.activityTime == null)
                    continue;

                string[] pairs = item.Value.activityTime.Split(',');
                foreach (var pair in pairs)
                {
                    string[] message = pair.Split('@');
                    if (message.Length < 2)
                    {
                        continue;
                    }
                    if (!activityTimeFormatDataMap[item.Value.weekDay].ContainsKey(Convert.ToInt32(message[1])))
                        activityTimeFormatDataMap[item.Value.weekDay].Add(Convert.ToInt32(message[1]), message[0]);
                }
            }
        }
    }
}
