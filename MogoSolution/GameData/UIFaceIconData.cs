using System;
using System.Collections.Generic;
using System.Linq;

namespace Mogo.GameData
{
    public class UIFaceIconData : GameData<UIFaceIconData>
    {
        public string facefirst { get; protected set; }
        public string facehead { get; protected set; }
        static public readonly string fileName = "xml/UIFaceIcon";
        //static public Dictionary<int, UIFaceIconData> dataMap { get; set; }
    }
}
