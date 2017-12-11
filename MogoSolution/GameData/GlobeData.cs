using System;
using System.Collections.Generic;
using UnityEngine;


namespace Mogo.GameData
{
    public class GlobalData : GameData<GlobalData>
    {
        public Int32 loginScene { get; protected set; }
        public Int32 chooseServerScene { get; protected set; }
        public Int32 chooseCharaterScene { get; protected set; }
        public Int32 homeScene { get; protected set; }
        public String loginUI { get; protected set; }
        public String chooseServerUI { get; protected set; }
        public String chooseCharaterUI { get; protected set; }
        public String homeUI { get; protected set; }
        public String battleUI { get; protected set; }
        public String mainCamera { get; protected set; }
        public int tower_all_sweep_vip_level { get; protected set; }
        public static readonly string fileName = "xml/GlobalData";
        //public static Dictionary<int, GlobalData> dataMap { get; set; }
    }
}