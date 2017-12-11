using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class LevelGiftData : GameData<LevelGiftData>
	{
        public int item { get; protected set; }
        public int price { get; protected set; }
        public int level { get; protected set; }
        public int vocation { get; protected set; }

        static public readonly string fileName = "xml/LevelGiftData";
        //static public Dictionary<int, LevelGiftData> dataMap { get; set; }
	}
}
