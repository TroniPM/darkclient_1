using System.Collections.Generic;

namespace Mogo.GameData
{
    public class EquipFXResData : GameData<EquipFXResData>
    {
        public string level { get; protected set; }
        public string material { get; protected set; }
        public string particleSys { get; protected set; }
        public string EquipSlot { get; protected set; }

        static public readonly string fileName = "xml/EquipFXRes";
        //public static Dictionary<int, ShaderData> dataMap { get; set; }
    }
}