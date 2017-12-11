using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ActionSoundData : GameData<ActionSoundData>
    {
        public int vocation { get; set; }
        public int action { get; set; }
        public Dictionary<int, int> sound { get; set; }

        public static readonly string fileName = "xml/ActionSound";
    }
}
