using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class TowerSweepCdClearCostData : GameData<TowerSweepCdClearCostData>
    {
        public int times { get; protected set; }
        public int cost { get; protected set; }
        static public readonly string fileName = "xml/TowerSweepCdClearCost";
        //static public Dictionary<int, TowerSweepCdClearCostData> dataMap { get; set; }
    }
}
