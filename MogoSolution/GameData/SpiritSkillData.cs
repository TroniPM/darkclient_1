using UnityEngine;
using System.Collections;
using Mogo.GameData;
using System.Collections.Generic;

namespace Mogo.GameData
{
#if UNITY_IPHONE
	public class SpiritSkillData : GameData
#else
    public class SpiritSkillData : GameData<SpiritSkillData>
#endif
	{
        public int skillid { get; protected set; }
        public string name { get; protected set; }
        public int icon { get; protected set; }
        public int add_point { get; protected set; }

        public static readonly string fileName = "xml/SpiritSkillData";
#if UNITY_IPHONE
        public static Dictionary<int, SpiritSkillData> dataMap { get; set; }
#endif
	}
}
