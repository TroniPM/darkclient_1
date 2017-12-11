using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class FoggyAbyssData : GameData<FoggyAbyssData>
    {
        public List<int> level { get; set; }
        public string difficulty1 { get; set; }
        public string difficulty2 { get; set; }
        public string difficulty3 { get; set; }

        public static readonly string fileName = "xml/Mwsy";
    }
}
