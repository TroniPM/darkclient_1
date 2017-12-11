using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mogo.GameData
{
    public class ElfAreaLimitData : GameData<ElfAreaLimitData>
    {
        public int id { get; set; }
        public int areaId { get; set; }
        public int nameCode { get; set; }
        public int limitLevel { get; set; }
        public static readonly string fileName = "xml/ElfAreaLimit";
    }
}
