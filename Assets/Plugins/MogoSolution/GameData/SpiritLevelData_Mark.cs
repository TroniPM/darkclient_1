using UnityEngine;
using System.Collections;
using Mogo.GameData;
using System.Collections.Generic;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class SpiritLevelData_Mark : GameData
#else
    public class SpiritLevelData_Mark : GameData<SpiritLevelData_Mark>
#endif
	{
        public int cost { get; protected set; }
        public int slot_num { get; protected set; }

        public static readonly string fileName = "xml/SpiritLevelData_Mark";
#if UNITY_IPHONE
        public static Dictionary<int, SpiritLevelData_Mark> dataMap { get; set; }
#endif
	}
}
