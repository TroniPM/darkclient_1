using System.Collections.Generic;

namespace Mogo.GameData
{
    /// <summary>
    /// 掉落模型。
    /// </summary>
    public class DropModelData : GameData<DropModelData>
    {
        public int type { get; protected set; }
        public byte subtype { get; protected set; }
        public string prefab { get; protected set; }

        static public readonly string fileName = "xml/DropModelData";
        //public static Dictionary<int, DropModelData> dataMap { get; set; }
    }
}