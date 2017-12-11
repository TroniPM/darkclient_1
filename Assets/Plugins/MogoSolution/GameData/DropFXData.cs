using System.Collections.Generic;

namespace Mogo.GameData
{
    /// <summary>
    /// 掉落特效。
    /// </summary>
#if UNITY_IPHONE
	public class DropFXData : GameData
#else
    public class DropFXData : GameData<DropFXData>
#endif
	{
        public DropFxType type { get; protected set; }
        public byte quality { get; protected set; }
        public int fx { get; protected set; }

        static public readonly string fileName = "xml/DropFXData";
#if UNITY_IPHONE
        public static Dictionary<int, DropFXData> dataMap { get; set; }
#endif
	}

    public enum DropFxType : byte
    {
        Glow = 1,
        PickUp = 2,
        Vanish = 3
    }
}