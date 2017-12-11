using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class UIFXData : GameData<UIFXData>
    {
        public float fadetime { get; protected set; }
        public float duration { get; protected set; }
        public string attachedWidget { get; protected set; }
        public string fxPrefab { get; protected set; }
        public int logicType { get; protected set; }
        public int renderType { get; protected set; }
        public int programType { get; protected set; }
        public string goName { get; protected set; }
        static public readonly string fileName = "xml/UIFX";
        //static public Dictionary<int, UIFXData> dataMap { get; set; }
    }
}
