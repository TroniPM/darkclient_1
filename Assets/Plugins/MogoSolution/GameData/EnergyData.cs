using System;
using System.Collections.Generic;

namespace Mogo.GameData
{
    public class EnergyData : GameData<EnergyData>
    {
        //public int maxEnergy { get; protected set; }
        //public int natureInterval { get; protected set; }
        //public int natureDelta { get; protected set; }
        //public float fixedTime { get; protected set; }
        //public int fixedPoint { get; protected set; }
        //public int levelPoint { get; protected set; }
        //public int buyPoint { get; protected set; }

        public int fixedPoints { get; protected set; }
        public int recoverPoints { get; protected set; }
        public int recoverInterval { get; protected set; }

        public static readonly string fileName = "xml/Energy";
        //public static Dictionary<int, EnergyData> dataMap { get; set; }
    }
}