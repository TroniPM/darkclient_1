using System.Collections.Generic;


namespace Mogo.GameData
{
    public class SoundData : GameData<SoundData>
    {
        public string path { get; protected set; }

        static public readonly string fileName = "xml/SoundData";
        //public static Dictionary<int, SoundData> dataMap { get; set; }
    }
}