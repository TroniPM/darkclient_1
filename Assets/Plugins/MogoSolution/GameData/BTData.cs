using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    /// <summary>
    /// AI配置
    /// </summary>
#if UNITY_IPHONE
	public class BTData : GameData
#else
    public class BTData : GameData<BTData>
#endif
    {
        public int level { get; protected set; }
        public float aoi { get; protected set; }
        public float fightRange { get; protected set; }
        public float patrolRange { get; protected set; }
        public float callRange { get; protected set; }

        public static readonly string fileName = "xml/BT_AI";
#if UNITY_IPHONE
        public static Dictionary<int, BTData> dataMap { get; set; }
#endif
    }
}
