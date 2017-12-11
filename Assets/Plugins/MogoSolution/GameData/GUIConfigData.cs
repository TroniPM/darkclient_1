using System.Collections;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class GUIConfigData : GameData<GUIConfigData>
    {
        public string prefab { get; set; }
        public List<int> guard { get; set; }
        static public readonly string fileName = "xml/gui_config";
        //static public Dictionary<int, GUIConfigData> dataMap { get; set; }
        static public bool GetGuard(int id)
        {
            return true;
        }
    }
}
