using UnityEngine;
using System.Collections;
using Mogo.GameData;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class SpiritMarkData : GameData<SpiritMarkData>
    {
        public int skillid { get; protected set; }
        public string name { get; protected set; }
        public int icon { get; protected set; }
        public int add_point { get; protected set; }

        public static readonly string fileName = "xml/SpiritMarkData";
        //public static Dictionary<int, SpiritMarkData> dataMap { get; set; }
    }
}
