using Mogo.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mogo.GameData
{
    public class UIMapData : GameData<UIMapData>
    {
        public string control { get; protected set; }
        public int upSoundID { get; protected set; }
        public int downSoundID { get; protected set; } 

        static public readonly string fileName = "xml/UIMap";
        //static public Dictionary<int, UIMapData> dataMap { get; set; }

        public static Dictionary<string, UIMapData> soundIDUINameMap { get; set; }
        public static void FormatDataMapToSoundIDUINameMap()
        {
            soundIDUINameMap = new Dictionary<string, UIMapData>();

            foreach (var item in dataMap)
            {
                if (item.Value.control == null)
                {
                    LoggerHelper.Warning("UIMap: key "  + item.Key + " is Empty!");
                    continue;
                }

                string name = item.Value.control.Split('_')[0];

                if (soundIDUINameMap.ContainsKey(name))
                {
                    LoggerHelper.Warning("UIMap: key " + item.Key + " has same control name: " + name);
                    continue;
                }

                soundIDUINameMap.Add(name, item.Value);
            }
        }
    }
}
