using System;
using System.Collections.Generic;
using UnityEngine;
using Mogo.Util;

namespace Mogo.GameData
{
    public class LevelGuideData : GameData<LevelGuideData>
    {
        public int type { get; set; }
        public List<int> level { get; set; }
        public int rate { get; set; }

        public static readonly string fileName = "xml/LevelGuide";
    }
}
