using System.Collections.Generic;

namespace Mogo.GameData
{
    public class ShaderData : GameData<ShaderData>
    {
        public string name { get; protected set; }
        public string color { get; protected set; }

        static public readonly string fileName = "xml/ShaderData";
        //public static Dictionary<int, ShaderData> dataMap { get; set; }
    }
}