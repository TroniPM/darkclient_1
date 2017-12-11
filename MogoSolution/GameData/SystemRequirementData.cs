using UnityEngine;
using System.Collections;

namespace Mogo.GameData
{
    public class SystemRequirementData : GameData<SystemRequirementData>
    {
        public int requestLevel { get; set; }
        public int pos { get; set; }
        public int title { get; set; }
        public static readonly string fileName = "xml/SystemRequirement";
    }
}
